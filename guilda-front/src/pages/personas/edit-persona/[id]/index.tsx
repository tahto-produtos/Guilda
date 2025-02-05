import { Button, useTheme } from "@mui/material";
import { useRouter } from "next/router";
import { useEffect, useState } from "react";
import { toast } from "react-toastify";
import { PageTitle } from "src/components/data-display/page-title/page-title";
import { ConfirmModal } from "src/components/feedback";
import { ContentArea } from "src/components/surfaces/content-area/content-area";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { useLoadingState } from "src/hooks";
import { EditPersonaComponent } from "src/modules/personas/fragments/edit-persona/edit-persona";
import { DeletePersonasUseCase } from "src/modules/personas/use-cases/delete-personas.use-case";
import { ShowPersonaUseCase } from "src/modules/personas/use-cases/show-persona-user.use-case";
import { Persona } from "src/typings/models/persona.model";
import { getLayout } from "src/utils";

export default function EditPersonaView() {
    const router = useRouter();
    const { id } = router.query;
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const theme = useTheme();
    const [personaUser, setPersonaUser] = useState<Persona | null>(null);
    const [confirmOpen, setConfirmOpen] = useState(false);

    async function showPersonaUser() {
        startLoading();

        await new ShowPersonaUseCase()
            .handle({ id: id as string })
            .then((data) => {
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
        id && showPersonaUser();
    }, [id]);

    async function deletePersona() {
        if (!id) return;

        startLoading();

        await new DeletePersonasUseCase()
            .handle({ IDPERSONAUSER: id as string, VALIDADETED: true })
            .then((data) => {
                toast.success("Persona apagada com sucesso!");
            })
            .catch((e) => {
                toast.error("Erro ao apagar a persona", e);
            })
            .finally(() => {
                finishLoading();
            });
    }

    return (
        <ContentCard>
            <ContentArea>
                <PageTitle title="Editar persona" loading={isLoading}>
                    <Button
                        variant="contained"
                        color="error"
                        disabled={isLoading}
                        onClick={() => setConfirmOpen(true)}
                    >
                        Apagar persona
                    </Button>
                </PageTitle>
                {personaUser && id && (
                    <EditPersonaComponent
                        idPersona={id as string}
                        initialState={personaUser}
                    />
                )}
            </ContentArea>
            <ConfirmModal
                open={confirmOpen}
                onClose={() => setConfirmOpen(false)}
                onConfirm={deletePersona}
                text={"Você deseja apagar a persona?"}
            />
        </ContentCard>
    );
}

EditPersonaView.getLayout = getLayout("private");
