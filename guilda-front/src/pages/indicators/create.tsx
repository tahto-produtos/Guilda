import PlaylistAdd from "@mui/icons-material/PlaylistAdd";
import { LoadingButton } from "@mui/lab";
import { TextField } from "@mui/material";
import { Box, Stack } from "@mui/system";
import { isAxiosError } from "axios";
import { useRouter } from "next/router";
import { useContext } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";
import { useLoadingState } from "src/hooks";
import { CreateIndicatorForm } from "src/modules/indicators/forms/create-indicator/create-indicator.form";
import { CreateIndicatorFormData } from "src/modules/indicators/forms/create-indicator/create-indicator.schema";
import { CreateIndicatorUseCase } from "src/modules/indicators/use-cases";
import { EXCEPTION_CODES } from "src/typings";
import { getLayout } from "src/utils";
import abilityFor from "src/utils/ability-for";

export default function Create() {
    const { isLoading, startLoading, finishLoading } = useLoadingState();
    const { myPermissions } = useContext(PermissionsContext);

    const router = useRouter();

    const handleCreateIndicator = async (
        createIndicatorFormData: CreateIndicatorFormData
    ) => {
        startLoading();
        const {
            name,
            description,
            id,
            calculationType,
            expression,
            weight,
            selectedSectors,
            isActive,
        } = createIndicatorFormData;

        try {
            const payload = {
                name: name,
                description: description,
                id: id,
                weight: weight,
                calculationType: calculationType,
                expression: expression,
                sectorsIds: selectedSectors
                    ? selectedSectors.map((item) => item.id)
                    : [],
                status: isActive,
            };

            await new CreateIndicatorUseCase().handle(payload);
            toast.success(`Indicador (${name}) criado com sucesso`);
            await router.push("/indicators");
        } catch (e) {
            if (isAxiosError(e)) {
                const code = e?.response?.data?.code;

                if (code === EXCEPTION_CODES.RESOURCE_ALREADY_EXISTS) {
                    toast.warning(
                        `Já existe um indicador com o nome "${name}" ou com este mesmo código`
                    );
                } else {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                }
            }
        } finally {
            finishLoading();
        }
    };

    return (
        <Card width={"100%"} display={"flex"} flexDirection={"column"}>
            <PageHeader
                title={"Criar Indicador"}
                headerIcon={<PlaylistAdd />}
            />
            <CreateIndicatorForm
                id={"create-indicator-form"}
                onSubmit={handleCreateIndicator}
            />
            <Box display={"flex"} justifyContent={"flex-end"} px={2} py={3}>
                <LoadingButton
                    loading={isLoading}
                    variant={"contained"}
                    type={"submit"}
                    form={"create-indicator-form"}
                    disabled={abilityFor(myPermissions).cannot(
                        "Editar Indicadores",
                        "Indicadores"
                    )}
                >
                    Criar Indicador
                </LoadingButton>
            </Box>
        </Card>
    );
}

Create.getLayout = getLayout("private");
