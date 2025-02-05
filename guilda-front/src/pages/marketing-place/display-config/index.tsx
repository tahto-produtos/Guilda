import Settings from "@mui/icons-material/Settings";
import Add from "@mui/icons-material/Add";
import { Box, Button, Stack, Typography } from "@mui/material";
import { Card, PageHeader } from "src/components";
import { getLayout } from "src/utils";
import { CreateDisplayModal } from "src/modules/marketing-place/fragments/create-display-modal";
import { useEffect, useState } from "react";
import { ListDisplayConfigUseCase } from "src/modules/marketing-place/use-cases/list-display-config.use-case";
import { toast } from "react-toastify";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { grey } from "@mui/material/colors";
import { BaseModal } from "src/components/feedback";
import { DeleteDisplayConfig } from "src/modules/marketing-place/use-cases/delete-display-config.use-case";
import { EditDisplayModal } from "src/modules/marketing-place/fragments/edit-display-modal";

export interface ConfigDisplay {
    PRIORIDADE: string;
    NOMEPRIORIDADE: string;
    NOMECONFIG: string;
    ITENS: {
        CODPRODUTO: number;
        POSICAO: string;
        NOMEPRODUTO: string;
    }[];
    HIERARQUIA: {
        CODHIERARQUIA: number;
        NOMEHIERARQUIA: string;
    }[];
    GRUPO: {
        CODGRUPO: number;
        NOMEGRUPO: string;
    }[];
    ESTOQUE: {
        CODESTOQUE: number;
        NOMEESTOQUE: string;
    }[];
    IDCONFIG: number;
    STATUS: string;
}

export default function DisplayConfig() {
    const [isOpen, setIsOpen] = useState<boolean>(false);
    const [configsList, setConfigsList] = useState<ConfigDisplay[]>([]);
    const [selectedConfig, setSelectedConfig] = useState<ConfigDisplay | null>(
        null
    );
    const [edit, setEdit] = useState<ConfigDisplay | null>(null);

    const getConfigs = async () => {
        new ListDisplayConfigUseCase()
            .handle()
            .then((data) => {
                setConfigsList(data);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    };

    useEffect(() => {
        getConfigs();
    }, []);

    function handleDelete() {
        if (!selectedConfig) {
            return;
        }

        new DeleteDisplayConfig()
            .handle(selectedConfig.IDCONFIG)
            .then((data) => {
                getConfigs();
                setSelectedConfig(null);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    }

    return (
        <Card width={"100%"} display={"flex"} flexDirection={"column"}>
            <PageHeader
                title={"Configuração vitrine"}
                headerIcon={<Settings />}
                addButtonAction={() => setIsOpen(true)}
                addButtonTitle="Nova vitrine"
            />
            <Stack px={2} py={4} width={"100%"} gap={2}>
                <CreateDisplayModal
                    open={isOpen}
                    onClose={() => {
                        getConfigs();
                        setIsOpen(false);
                    }}
                />
                {edit && (
                    <EditDisplayModal
                        open={edit ? true : false}
                        onClose={() => {
                            getConfigs();
                            setEdit(null);
                        }}
                        displayToEdit={edit}
                    />
                )}
                {configsList?.map((item, index) => (
                    <Box
                        key={index}
                        display={"flex"}
                        gap={2}
                        alignItems={"center"}
                        justifyContent={"space-between"}
                        borderBottom={"solid 1px #efefef"}
                        pb={2}
                    >
                        <Box>
                            <Typography variant="body2" color={grey[600]}>
                                Configuração:
                            </Typography>
                            <Typography>{item.NOMECONFIG}</Typography>
                        </Box>
                        <Box display={"flex"} flexDirection={"row"} gap={2}>
                            <Button
                                variant="outlined"
                                size="small"
                                onClick={() => setEdit(item)}
                            >
                                Editar
                            </Button>
                            <Button
                                variant="contained"
                                size="small"
                                onClick={() => setSelectedConfig(item)}
                            >
                                Ver Detalhes
                            </Button>
                        </Box>
                    </Box>
                ))}
                <BaseModal
                    width={"540px"}
                    open={selectedConfig ? true : false}
                    title={selectedConfig?.NOMECONFIG}
                    onClose={() => setSelectedConfig(null)}
                >
                    <Box
                        width={"100%"}
                        display={"flex"}
                        flexDirection={"column"}
                        gap={2}
                        pb={2}
                    >
                        <Box>
                            <Typography variant="body2" color={grey[600]}>
                                Prioridade:
                            </Typography>
                            <Typography>
                                {selectedConfig?.NOMEPRIORIDADE}
                            </Typography>
                        </Box>
                        <Box>
                            <Typography variant="body2" color={grey[600]}>
                                Produtos:
                            </Typography>
                            {selectedConfig?.ITENS.map((p, i) => (
                                <Typography key={i}>
                                    {p.POSICAO} - {p.NOMEPRODUTO}
                                </Typography>
                            ))}
                        </Box>
                        <Box>
                            <Typography variant="body2" color={grey[600]}>
                                Hierarquias:
                            </Typography>
                            {selectedConfig?.HIERARQUIA.map((p, i) => (
                                <Typography key={i}>
                                    {p.NOMEHIERARQUIA}
                                </Typography>
                            ))}
                        </Box>
                        <Box>
                            <Typography variant="body2" color={grey[600]}>
                                Grupos:
                            </Typography>
                            {selectedConfig?.GRUPO.map((p, i) => (
                                <Typography key={i}>{p.NOMEGRUPO}</Typography>
                            ))}
                        </Box>
                        <Box>
                            <Typography variant="body2" color={grey[600]}>
                                Grupos:
                            </Typography>
                            {selectedConfig?.ESTOQUE.map((p, i) => (
                                <Typography key={i}>{p.NOMEESTOQUE}</Typography>
                            ))}
                        </Box>
                        <Button
                            variant="contained"
                            color="error"
                            onClick={handleDelete}
                        >
                            Apagar configuração
                        </Button>
                    </Box>
                </BaseModal>
            </Stack>
        </Card>
    );
}

DisplayConfig.getLayout = getLayout("private");
