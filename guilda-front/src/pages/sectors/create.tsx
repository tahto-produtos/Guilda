import { useRouter } from "next/router";
import { Box, Modal, Stack } from "@mui/material";
import { LoadingButton } from "@mui/lab";
import { toast } from "react-toastify";
import { isAxiosError } from "axios";
import { EXCEPTION_CODES } from "../../typings";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "../../constants";
import { getLayout } from "../../utils";
import { useLoadingState } from "../../hooks";
import { Card, PageHeader } from "../../components";
import {
    CreateSectorForm,
    CreateSectorFormData,
    CreateSectorUseCase,
} from "src/modules/sectors";
import PlaylistAdd from "@mui/icons-material/PlaylistAdd";

export default function Create() {
    const { isLoading, startLoading, finishLoading } = useLoadingState();

    const router = useRouter();

    const handleCreateGroup = async (
        createSectorFormData: CreateSectorFormData
    ) => {
        startLoading();
        const { name, indicatorsIds, code } = createSectorFormData;

        try {
            await new CreateSectorUseCase().handle({
                name,
                indicatorsIds,
                code,
            });
            toast.success(`Setor (${name}) criado com sucesso`);
            await router.push("/sectors");
        } catch (e) {
            if (isAxiosError(e)) {
                const code = e?.response?.data?.code;
                console.log(e?.response?.data?.Message);
                if (e?.response?.data?.Message) {
                    toast.warning(e?.response?.data?.Message);
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
            <PageHeader title={"Criar Setor"} headerIcon={<PlaylistAdd />} />
            <CreateSectorForm
                id={"create-sector-form"}
                onSubmit={handleCreateGroup}
            />
            <Box display={"flex"} justifyContent={"flex-end"} px={2} py={3}>
                <LoadingButton
                    loading={isLoading}
                    variant={"contained"}
                    type={"submit"}
                    form={"create-sector-form"}
                >
                    Criar Setor
                </LoadingButton>
            </Box>
        </Card>
    );
}

Create.getLayout = getLayout("private");
