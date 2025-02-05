import {
    Button,
    Divider,
    InputAdornment,
    Stack,
    TextField,
    useTheme,
} from "@mui/material";
import { useEffect, useState } from "react";
import { toast } from "react-toastify";
import { PageTitle } from "src/components/data-display/page-title/page-title";
import { SearchIcon } from "src/components/icons/search.icon";
import { ContentArea } from "src/components/surfaces/content-area/content-area";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { useLoadingState } from "src/hooks";
import { ListPersonasUseCase } from "src/modules/personas/use-cases/list-personas.use-case";
import { getLayout } from "src/utils";

export default function CreatePersonaView() {
    const [personas, setPersonas] = useState([]);
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const theme = useTheme();

    // async function ListPersonas() {
    //     startLoading();

    //     new ListPersonasUseCase()
    //         .handle()
    //         .then((data) => {
    //             setPersonas(data);
    //         })
    //         .catch(() => {
    //             toast.error("Erro ao listar as personas.");
    //         })
    //         .finally(() => {
    //             finishLoading();
    //         });
    // }

    // useEffect(() => {
    //     ListPersonas();
    // }, []);

    return (
        <ContentCard>
            <ContentArea>
                <PageTitle title="Criar persona"></PageTitle>
                <Divider />
                <Stack></Stack>
            </ContentArea>
        </ContentCard>
    );
}

CreatePersonaView.getLayout = getLayout("private");
