import { LoadingButton } from "@mui/lab";
import { Button, Stack, TextField, useTheme } from "@mui/material";
import { useContext, useState } from "react";
import { toast } from "react-toastify";
import { PageTitle } from "src/components/data-display/page-title/page-title";
import { ContentArea } from "src/components/surfaces/content-area/content-area";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { useLoadingState } from "src/hooks";
import { CreateHobbyUseCase } from "src/modules/personas/use-cases/create-hobby.use-case";
import { DeleteHobbyUseCase } from "src/modules/personas/use-cases/delete-hobby.use-case";
import { getLayout } from "src/utils";

export default function CreateHobbiesView() {
    const { myUser } = useContext(UserInfoContext);
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [name, setName] = useState<string>("");

    const theme = useTheme();

    async function createHobby() {
        if (!myUser) return;

        startLoading();

        new CreateHobbyUseCase()
            .handle({
                createdBy: myUser.id,
                name: name,
            })
            .then(() => {
                toast.success("Hobby criado com sucesso!");
            })
            .catch((e) => {
                const msg = e?.response?.data?.Message;
                toast.error(msg ? msg : "Erro ao criar o hobby.");
            })
            .finally(() => {
                finishLoading();
            });
    }

    return (
        <ContentCard>
            <ContentArea>
                <PageTitle title="Criar Hobby" loading={isLoading} />
                <Stack direction={"column"} gap={"20px"} mt={"10px"}>
                    <TextField
                        label="Hobby"
                        fullWidth
                        value={name}
                        onChange={(e) => setName(e.target.value)}
                    />
                    <Stack direction={"row"} justifyContent={"flex-end"}>
                        <LoadingButton
                            variant="contained"
                            onClick={createHobby}
                            disabled={!myUser}
                            loading={isLoading}
                        >
                            Criar hobby
                        </LoadingButton>
                    </Stack>
                </Stack>
            </ContentArea>
        </ContentCard>
    );
}

CreateHobbiesView.getLayout = getLayout("private");
