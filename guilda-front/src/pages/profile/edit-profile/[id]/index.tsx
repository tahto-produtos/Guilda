import { Breadcrumbs, Button, Link, Typography, useTheme } from "@mui/material";
import { useRouter } from "next/router";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { PageTitle } from "src/components/data-display/page-title/page-title";
import { ContentArea } from "src/components/surfaces/content-area/content-area";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { useLoadingState } from "src/hooks";
import { ShowPersonaUseCase } from "src/modules/personas/use-cases/show-persona-user.use-case";
import { Persona } from "src/typings/models/persona.model";
import { getLayout } from "src/utils";
import { EditProfileComponent } from "../../../../modules/profiles/fragments/edit-profile/edit-profile";
import { LoggedAccountsUseCase } from "../../../../modules/profiles/use-cases/logged-accounts.use-case";
import { UserPersonaContext } from "src/contexts/user-persona/user-persona.context";
import HomeOutlined from "@mui/icons-material/HomeOutlined";

export default function EditProfileView() {
    const router = useRouter();
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const theme = useTheme();
    const [personaUser, setPersonaUser] = useState<Persona | null>(null);
    const { idPersonAccount, personaShowUser } = useContext(UserPersonaContext);

    async function showPersonaUser() {
        startLoading();

        await new ShowPersonaUseCase()
            .handle({ id: idPersonAccount?.toString() })
            .then((data) => {
                console.log("TESTE3", data);
                setPersonaUser(data[0]);
            })
            .catch(() => {
                toast.error("Erro ao carregar informações do perfil.");
            })
            .finally(() => {
                finishLoading();
            });
    }

    useEffect(() => {
        idPersonAccount && showPersonaUser();
    }, [idPersonAccount]);

    return (
        <ContentCard>
            <ContentArea>
                <Breadcrumbs aria-label="breadcrumb">
                    <Link
                        underline="hover"
                        sx={{ display: "flex", alignItems: "center" }}
                        color={theme.palette.text.primary}
                        href="/"
                    >
                        <HomeOutlined
                            sx={{
                                mr: 0.5,
                                color: theme.palette.text.disabled,
                            }}
                        />
                    </Link>
                    <Link
                        sx={{ display: "flex", alignItems: "center" }}
                        color={theme.palette.text.disabled}
                    >
                        <Typography fontWeight={"500"}>
                            {personaShowUser?.NOME_SOCIAL ||
                                personaShowUser?.NOME}
                        </Typography>
                    </Link>
                    <Link
                        sx={{ display: "flex", alignItems: "center" }}
                        color={theme.palette.text.primary}
                    >
                        <Typography fontWeight={"700"}>
                            Edição de perfil
                        </Typography>
                    </Link>
                </Breadcrumbs>
                {personaUser && idPersonAccount && (
                    <EditProfileComponent
                        idPersona={idPersonAccount}
                        initialState={personaUser}
                    />
                )}
            </ContentArea>
        </ContentCard>
    );
}

EditProfileView.getLayout = getLayout("private");
