import AdminPanelSettings from "@mui/icons-material/AdminPanelSettings";
import { Button, Stack, Typography } from "@mui/material";
import { useContext, useState } from "react";
import { Card, PageHeader } from "src/components";
import { AutoCompleteCollaborators } from "src/components/autocompletes/collaborators/collaborators-autocomplete";
import { AdmViewContext } from "src/contexts/adm-view-context/adm-view-context";
import { LoginAdmViewUseCase } from "src/modules/profiles/use-cases/login-adm-view.use-case";
import { Collaborator } from "src/typings/models/collaborator.model";
import { getLayout } from "src/utils";

export default function AdmView() {
    const [profile, setProfile] = useState<Collaborator | null>(null);
    const { setActiveUser } = useContext(AdmViewContext);

    function handleConfirm() {
        
        if (!profile) {
            return;
        }
    
        new LoginAdmViewUseCase()
            .handle({ profileId: profile.id })
            .then((data) => {
                setActiveUser({
                    id: profile.id,
                    registry: profile.registry,
                    name: profile.name,
                    token: data.token,
                });

            })
            .catch(() => {});

            
    }

    return (
        <Card
            width={"100%"}
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"space-between"}
        >
            <PageHeader
                title={`Visão administrador`}
                headerIcon={<AdminPanelSettings />}
            />
            <Stack direction={"column"} gap={"20px"}>
                <Typography variant="body2">
                    Selecione um usuário para acessar a visão espelhada
                </Typography>
                <AutoCompleteCollaborators
                    getValue={(value) => setProfile(value)}
                />
                <Button
                    onClick={handleConfirm}
                    variant="contained"
                    disabled={!profile}
                >
                    Acessar visão espelhada
                </Button>
            </Stack>
        </Card>
    );
}

AdmView.getLayout = getLayout("private");
