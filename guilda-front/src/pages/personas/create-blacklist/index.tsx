import { LoadingButton } from "@mui/lab";
import { Button, Stack, TextField, useTheme } from "@mui/material";
import { useContext, useState } from "react";
import { toast } from "react-toastify";
import { PageTitle } from "src/components/data-display/page-title/page-title";
import { ContentArea } from "src/components/surfaces/content-area/content-area";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { useLoadingState } from "src/hooks";
import { CreateBlackList } from "src/modules/blacklist/use-cases/create-blacklist.use-case";
import { getLayout } from "src/utils";

export default function CreateBackListView() {
    const { myUser } = useContext(UserInfoContext);
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [name, setName] = useState<string>("");

    const theme = useTheme();

    async function createWord() {
        if (!myUser) return;

        startLoading();

        new CreateBlackList()
            .handle({
                word: name,
            })
            .then(() => {
                toast.success("Palavra criada com sucesso!");
            })
            .catch((e) => {
                const msg = e?.response?.data?.Message;
                toast.error(msg ? msg : "Erro ao criar a palavra.");
            })
            .finally(() => {
                finishLoading();
            });
    }

    return (
        <ContentCard>
            <ContentArea>
                <PageTitle
                    title="Criar palavra para Blacklist"
                    loading={isLoading}
                />
                <Stack direction={"column"} gap={"20px"} mt={"10px"}>
                    <TextField
                        label="Palavra"
                        fullWidth
                        value={name}
                        onChange={(e) => setName(e.target.value)}
                    />
                    <Stack direction={"row"} justifyContent={"flex-end"}>
                        <LoadingButton
                            variant="contained"
                            onClick={createWord}
                            disabled={!myUser}
                            loading={isLoading}
                        >
                            Salvar palavra
                        </LoadingButton>
                    </Stack>
                </Stack>
            </ContentArea>
        </ContentCard>
    );
}

CreateBackListView.getLayout = getLayout("private");
