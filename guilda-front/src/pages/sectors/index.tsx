import { useRouter } from "next/router";
import { Box, Checkbox, Stack, Typography } from "@mui/material";
import { Card, PageHeader } from "../../components";
import { DateUtils, getLayout, SheetBuilder } from "../../utils";
import { useContext, useEffect, useState } from "react";
import { ListSectorsTable } from "src/modules/sectors/tables";
import { TableDataModel } from "src/typings";
import { PaginationModel } from "src/typings/models/pagination.model";
import { toast } from "react-toastify";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { ListSectorsUseCase } from "src/modules";
import Block from "@mui/icons-material/Block";
import Download from "@mui/icons-material/Download";
import ViewList from "@mui/icons-material/ViewList";
import { Can } from "@casl/react";
import abilityFor from "src/utils/ability-for";
import { grey } from "@mui/material/colors";
import WithoutPermissionCard from "src/components/data-display/without-permission/without-permissions";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";

export default function Sectors() {
    const router = useRouter();
    const { myPermissions } = useContext(PermissionsContext);
    const [tableData, setTableData] = useState<TableDataModel>();
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [paginationData, setPaginationData] =
        useState<PaginationModel | null>(null);
    const [associatedIndicators, setAssociatedIndicator] =
        useState<boolean>(false);

    const handleRedirectToCreateSectorPage = () =>
        router.push("/sectors/create");

    const GetSectors = async (pagination: PaginationModel) => {
        setIsLoading(true);

        new ListSectorsUseCase()
            .handle(pagination, associatedIndicators)
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
                setIsLoading(false);
            });
    };

    useEffect(() => {
        paginationData && GetSectors(paginationData);
    }, [associatedIndicators]);

    const exportSectors = async () => {
        setIsLoading(true);

        if (!tableData || !paginationData) {
            return;
        }

        const pagination = {
            limit: tableData.totalItems,
            offset: 0,
            searchText: paginationData.searchText,
            startDate: paginationData.startDate,
            endDate: paginationData.endDate,
        };

        await new ListSectorsUseCase()
            .handle(pagination)
            .then((data) => {
                console.log(data);
                const docRows = data.items.map((item) => {
                    return [
                        item.id.toString(),
                        item.name,
                        item.level.toString(),
                        item.createdAt
                            ? DateUtils.formatDatePtBR(item.createdAt)
                            : "",
                        item.indicators as unknown as string,
                    ];
                });

                let sectorSheetBuilder = new SheetBuilder();
                sectorSheetBuilder
                    .setHeader([
                        "GIP",
                        "Nome",
                        "Level",
                        "Criado em",
                        "Indicadores",
                    ])
                    .append(docRows)
                    .exportAs(
                        `Setores${
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
                setIsLoading(false);
            });
    };

    return (
        <Card width={"100%"} display={"flex"} flexDirection={"column"}>
            <PageHeader
                title={"Lista de Setores"}
                headerIcon={<ViewList />}
                addButtonTitle={"Criar Setor"}
                addButtonAction={handleRedirectToCreateSectorPage}
                secondayButtonAction={exportSectors}
                secondaryButtonTitle={"Exportar Relatório"}
                secondaryButtonIcon={<Download />}
                secondaryButtonIsDisable={
                    !tableData || !paginationData || isLoading
                }
                addButtonIsDisabled={abilityFor(myPermissions).cannot(
                    "Editar Setores",
                    "Setores"
                )}
            />
            <Stack width={"100%"}>
                <Box display={"flex"} alignItems={"center"}>
                    <Typography fontSize={"14px"}>
                        Mostrar apenas setores com indicadores associados
                    </Typography>
                    <Checkbox
                        checked={associatedIndicators}
                        onChange={(e) =>
                            setAssociatedIndicator(e.target.checked)
                        }
                    />
                </Box>
                <Can
                    I="Ver Setores"
                    a="Setores"
                    ability={abilityFor(myPermissions)}
                >
                    {() => (
                        <ListSectorsTable
                            tableData={tableData}
                            getTableProps={(pagination: PaginationModel) => {
                                GetSectors(pagination);
                                setPaginationData(pagination);
                            }}
                            isLoading={isLoading}
                        />
                    )}
                </Can>
                {abilityFor(myPermissions).cannot("Ver Setores", "Setores") && (
                    <WithoutPermissionCard />
                )}
            </Stack>
        </Card>
    );
}

Sectors.getLayout = getLayout("private");
