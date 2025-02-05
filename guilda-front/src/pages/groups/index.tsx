import { useRouter } from "next/router";
import { Stack, Typography } from "@mui/material";

import { getLayout } from "../../utils";
import { ListGroupsTable, ListGroupsUseCase } from "../../modules";
import { Card, PageHeader } from "../../components";
import { useContext, useState } from "react";
import { TableDataModel } from "../../typings";
import { toast } from "react-toastify";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "../../constants";
import { PaginationModel } from "src/typings/models/pagination.model";
import { useLoadingState } from "src/hooks";
import { Can } from "@casl/react";
import abilityFor from "src/utils/ability-for";
import Block from "@mui/icons-material/Block";
import ViewList from "@mui/icons-material/ViewList";
import { grey } from "@mui/material/colors";
import WithoutPermissionCard from "src/components/data-display/without-permission/without-permissions";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";

export default function Groups() {
    const router = useRouter();
    const { myPermissions } = useContext(PermissionsContext);
    const [tableData, setTableData] = useState<TableDataModel>();
    const [paginationData, setPaginationData] =
        useState<PaginationModel | null>(null);

    const { isLoading, startLoading, finishLoading } = useLoadingState();

    const GetGroups = async (pagination: PaginationModel) => {
        startLoading();

        new ListGroupsUseCase()
            .handle(pagination)
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
                finishLoading();
            });
    };

    const handleRedirectToCreateGroupPage = () => router.push("/groups/create");

    return (
        <Card width={"100%"} display={"flex"} flexDirection={"column"}>
{/*             <PageHeader
                title={"Lista de Grupos"}
                headerIcon={<ViewList />}
                addButtonTitle={"Criar Grupo"}
                addButtonAction={handleRedirectToCreateGroupPage}
                addButtonIsDisabled={abilityFor(myPermissions).cannot(
                    "Editar Grupos",
                    "Grupos"
                )}
            /> */}
            <Stack width={"100%"}>
                <Can
                    I="Ver Grupos"
                    a="Grupos"
                    ability={abilityFor(myPermissions)}
                >
                    <ListGroupsTable
                        tableData={tableData}
                        getTableProps={(pagination: PaginationModel) => {
                            GetGroups(pagination);
                            setPaginationData(pagination);
                        }}
                        isLoading={isLoading}
                    />
                </Can>
                {abilityFor(myPermissions).cannot("Ver Grupos", "Grupos") && (
                    <WithoutPermissionCard />
                )}
            </Stack>
        </Card>
    );
}

Groups.getLayout = getLayout("private");
