import { useRouter } from "next/router";
import { Box, Stack } from "@mui/material";
import { LoadingButton } from "@mui/lab";
import { toast } from "react-toastify";
import { isAxiosError } from "axios";

import {
    CreateGroupForm,
    CreateGroupFormSchema,
    CreateGroupUseCase,
} from "../../modules";
import { EXCEPTION_CODES } from "../../typings";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "../../constants";
import { getLayout } from "../../utils";
import { useLoadingState } from "../../hooks";
import { Card, PageHeader } from "../../components";
import Add from "@mui/icons-material/Add";
import PlaylistAdd from "@mui/icons-material/PlaylistAdd";

export default function Create() {
    const { isLoading, startLoading, finishLoading } = useLoadingState();

    const router = useRouter();

    const handleCreateGroup = async (
        createGroupFormData: CreateGroupFormSchema
    ) => {
        startLoading();
        const { name, description, alias, image } = createGroupFormData;
        const imageFile = image as File;

        try {
            await new CreateGroupUseCase().handle({
                name,
                description,
                alias,
                image: imageFile,
            });
            toast.success(`Grupo ${name} criado com sucesso`);
            await router.push("/groups");
        } catch (e) {
            if (isAxiosError(e)) {
                const code = e?.response?.data?.code;

                if (code === EXCEPTION_CODES.RESOURCE_ALREADY_EXISTS) {
                    toast.warning(`Já existe um grupo chamado: "${name}"`);
                } else {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                }
            }
        } finally {
            finishLoading();
        }
    };

    return null;

    return (
        <Card width={"100%"} display={"flex"} flexDirection={"column"}>
            <PageHeader title={"Criar Grupo"} headerIcon={<PlaylistAdd />} />
            <CreateGroupForm
                id={"create-group-form"}
                onSubmit={handleCreateGroup}
            />
            <Box display={"flex"} justifyContent={"flex-end"} px={2} py={3}>
                <LoadingButton
                    loading={isLoading}
                    variant={"contained"}
                    type={"submit"}
                    form={"create-group-form"}
                >
                    Criar Grupo
                </LoadingButton>
            </Box>
        </Card>
    );
}

Create.getLayout = getLayout("private");
