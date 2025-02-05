import { Can } from "@casl/react";
import Block from "@mui/icons-material/Block";
import Download from "@mui/icons-material/Download";
import ViewList from "@mui/icons-material/ViewList";
import FormatListBulleted from "@mui/icons-material/FormatListBulleted";
import ViewStream from "@mui/icons-material/ViewStream";
import {
    FormControl,
    InputLabel,
    MenuItem,
    Select,
    Stack,
    Typography,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { useRouter } from "next/router";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import WithoutPermissionCard from "src/components/data-display/without-permission/without-permissions";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { ListIndicatorsTable } from "src/modules/indicators/tables";
import { ListIndicatorsUseCase } from "src/modules/indicators/use-cases";
import { ListIndicatorsAllDetailsUseCase } from "src/modules/indicators/use-cases/list-indicators/all-details-indicators.use-case";
import { TableDataModel } from "src/typings";
import { PaginationModel } from "src/typings/models/pagination.model";
import { DateUtils, getLayout, SheetBuilder } from "src/utils";
import abilityFor from "src/utils/ability-for";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";

export default function Indicators() {
    const router = useRouter();
    const { myPermissions } = useContext(PermissionsContext);
    const [tableData, setTableData] = useState<TableDataModel>();
    const [loading, setLoading] = useState<boolean>(false);
    const [status, setStatus] = useState<string>("All");
    const [paginationData, setPaginationData] =
        useState<PaginationModel | null>(null);

    const handleRedirectToCreateIndicatorPage = () =>
        router.push("/indicators/create");

    const handleRedirectToListNewIndicatorPage = () =>
        router.push("/indicators/news");

    const GetIndicators = async (pagination: PaginationModel) => {
        setLoading(true);

        const definedStatus =
            status == "Ativo" ? true : status == "Inativo" ? false : undefined;

        new ListIndicatorsUseCase()
            .handle(pagination, definedStatus)
            .then((data) => {
                setTableData({
                    items: data.items,
                    totalItems: data.totalItems,
                });
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                setLoading(false);
            });
    };

    useEffect(() => {
        if (paginationData) {
            GetIndicators(paginationData);
        }
    }, [status]);

    const exportIndicators = async () => {
        setLoading(true);

        if (!tableData || !paginationData) {
            return;
        }

        const definedStatus =
            status == "Ativo" ? true : status == "Inativo" ? false : undefined;

        const pagination = {
            limit: tableData.totalItems,
            offset: 0,
            searchText: paginationData.searchText,
            startDate: paginationData.startDate,
            endDate: paginationData.endDate,
        };

        await new ListIndicatorsAllDetailsUseCase()
            .handle(pagination, definedStatus)
            .then((data) => {
                const docRows = data.items.map((item) => {
                    return [
                        item.id?.toString(),
                        item.name,
                        item.description,
                        item.createdAt
                            ? DateUtils.formatDatePtBR(item.createdAt)
                            : "",
                        item.sectors
                            ?.map((sector) => sector.name)
                            .join(" - ")
                            .toString() || "",
                        item.mathematicalExpression.expression,
                        item.status == true ? "Ativo" : "Inativo",
                        item.calculationType == "BIGGER_BETTER"
                            ? "Maior melhor"
                            : item.calculationType == "LESS_BETTER"
                            ? "Menor melhor"
                            : "",
                        item.type,
                    ];
                });

                let indicatorSheetBuilder = new SheetBuilder();
                indicatorSheetBuilder
                    .setHeader([
                        "Código",
                        "Nome",
                        "Descrição",
                        "Criado em",
                        "Setores Associados",
                        "Métricas do Indicador",
                        "Status",
                        "Tipo",
                        "TIPO DO INDICADOR",
                    ])
                    .append(docRows)
                    .exportAs(
                        `Indicadores${
                            pagination.startDate
                                ? `-De-${DateUtils.formatDatePtBR(
                                      pagination.startDate
                                  )}`
                                : ""
                        }${
                            pagination.endDate
                                ? `-Até-${DateUtils.formatDatePtBR(
                                      pagination.endDate
                                  )}`
                                : ""
                        }`
                    );

                toast.success("Relatório exportado com sucesso!");
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                setLoading(false);
            });
    };

    return (
        <Card width={"100%"} display={"flex"} flexDirection={"column"}>
            <PageHeader
                title={"Lista de Indicadores"}
                headerIcon={<ViewList />}
                addButtonTitle={"Criar Indicador"}
                addButtonAction={handleRedirectToCreateIndicatorPage}
                secondayButtonAction={exportIndicators}
                secondaryButtonTitle={"Exportar Relatório"}
                secondaryButtonIcon={<Download />}
                secondaryButtonIsDisable={
                    !tableData || !paginationData || loading
                }
                thirdButtonAction={handleRedirectToListNewIndicatorPage}
                thirdButtonTitle={"Novos indicadores"}
                thirdButtonIcon={<FormatListBulleted fontSize={"small"} />}
                thirdButtonIsDisable={false}
                addButtonIsDisabled={abilityFor(myPermissions).cannot(
                    "Editar Indicadores",
                    "Indicadores"
                )}
            />
            <Stack px={2} py={4} width={"100%"} gap={2}>
                <FormControl sx={{ width: "100%", maxWidth: "120px" }}>
                    <InputLabel sx={{ backgroundColor: "#fff" }} size="small">
                        Status
                    </InputLabel>
                    <Select
                        onChange={(e) => setStatus(e.target.value)}
                        value={status}
                        size="small"
                    >
                        <MenuItem value={"All"}>Todos</MenuItem>
                        <MenuItem value={"Ativo"}>Ativo</MenuItem>
                        <MenuItem value={"Inativo"}>Inativo</MenuItem>
                    </Select>
                </FormControl>
                <Can
                    I="Ver Indicadores"
                    a="Indicadores"
                    ability={abilityFor(myPermissions)}
                >
                    <ListIndicatorsTable
                        tableData={tableData}
                        getTableProps={(pagination: PaginationModel) => {
                            GetIndicators(pagination);
                            setPaginationData(pagination);
                        }}
                        isLoading={loading}
                        enableCodeSearch={true}
                    />
                </Can>
                {abilityFor(myPermissions).cannot(
                    "Ver Indicadores",
                    "Indicadores"
                ) && <WithoutPermissionCard />}
            </Stack>
        </Card>
    );
}

Indicators.getLayout = getLayout("private");
