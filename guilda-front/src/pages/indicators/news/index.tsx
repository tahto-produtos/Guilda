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
import { ListNewIndicatorsTable } from "src/modules/indicators/tables";
import { ListNewIndicatorsUseCase } from "src/modules/indicators/use-cases";
import { ListIndicatorsAllDetailsUseCase } from "src/modules/indicators/use-cases/list-indicators/all-details-indicators.use-case";
import { TableDataModel } from "src/typings";
import { PaginationModel } from "src/typings/models/pagination.model";
import { DateUtils, getLayout, SheetBuilder } from "src/utils";
import abilityFor from "src/utils/ability-for";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";

export default function NewsIndicators() {
    const router = useRouter();
    const { myPermissions } = useContext(PermissionsContext);
    const [tableData, setTableData] = useState<TableDataModel>();
    const [loading, setLoading] = useState<boolean>(false);
    const [status, setStatus] = useState<string>("All");
    const [paginationData, setPaginationData] =
        useState<PaginationModel | null>(null);

    const GetIndicators = async (pagination: PaginationModel) => {
        setLoading(true);

        const definedStatus =
            status == "Ativo" ? true : status == "Inativo" ? false : undefined;

        new ListNewIndicatorsUseCase()
            .handle(definedStatus)
            .then((data) => {
                const newData = data.map((objeto) => ({
                    ...objeto,
                    id: objeto.INDICATORID,
                }));
                setTableData({
                    items: newData,
                    totalItems: newData.length,
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

        await new ListNewIndicatorsUseCase()
            .handle(definedStatus)
            .then((data) => {
                const docRows = data.map((item) => {
                    return [
                        item.INDICATORID?.toString(),
                        item.NAME,
                        item.DESCRIPTION,
                        item.CREATED_AT
                            ? DateUtils.formatDatePtBR(item.CREATED_AT)
                            : "",
                        item.SECTOR,
                        item.METRIC,
                        item.STATUS == true ? "Ativo" : "Inativo",
                        item.CALCTYPE == "BIGGER_BETTER"
                            ? "Maior melhor"
                            : item.CALCTYPE == "LESS_BETTER"
                            ? "Menor melhor"
                            : "",
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
                    ])
                    .append(docRows)
                    .exportAs(`Novos Indicadores`);

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
                title={"Lista de Novos Indicadores"}
                headerIcon={<ViewList />}
                secondayButtonAction={exportIndicators}
                secondaryButtonTitle={"Exportar Relatório"}
                secondaryButtonIcon={<Download />}
                secondaryButtonIsDisable={
                    !tableData || !paginationData || loading
                }
                addButtonIsDisabled={abilityFor(myPermissions).cannot(
                    "Editar Indicadores",
                    "Indicadores"
                )}
            />
            <Stack px={2} py={4} width={"100%"} gap={2}>
                <ListNewIndicatorsTable
                    tableData={tableData}
                    getTableProps={(pagination: PaginationModel) => {
                        GetIndicators(pagination);
                        //setPaginationData(pagination);
                    }}
                    isLoading={loading}
                    enableCodeSearch={false}
                    hideSearchInput={true}
                    hideDatePicker={true}
                />
                {abilityFor(myPermissions).cannot(
                    "Ver Indicadores",
                    "Indicadores"
                ) && <WithoutPermissionCard />}
            </Stack>
        </Card>
    );
}

NewsIndicators.getLayout = getLayout("private");
