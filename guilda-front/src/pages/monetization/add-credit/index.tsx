import FileOpen from "@mui/icons-material/FileOpen";
import Remove from "@mui/icons-material/Remove";
import Savings from "@mui/icons-material/Savings";
import { LoadingButton } from "@mui/lab";
import {
    Autocomplete,
    Box,
    Button,
    CircularProgress,
    TextField,
    Typography,
    useTheme,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { Stack } from "@mui/system";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { useDebounce, useLoadingState } from "src/hooks";
import { MassiveCredits } from "src/modules/monetization/fragments/massive-credits";
import { MassiveDebits } from "src/modules/monetization/fragments/massive-debits";
import { AddCreditUseCase } from "src/modules/monetization/use-cases/add-credit.use-case";
import { RemoveCreditUseCase } from "src/modules/monetization/use-cases/remove-credit.use-case";
import { DateUtils, SheetBuilder, getLayout } from "src/utils";
import Download from "@mui/icons-material/Download";
import abilityFor from "src/utils/ability-for";
import WithoutPermissionCard from "src/components/data-display/without-permission/without-permissions";
import { AccountBalanceUseCase } from "src/modules/monetization/use-cases/account-balance.use-case";
import { BaseModal } from "src/components/feedback";
import { PaginationModel } from "src/typings/models/pagination.model";
import { ListCollaboratorsAllUseCase } from "src/modules/collaborators/use-cases/list-collaborators.use-case";
import { Collaborator } from "src/typings/models/collaborator.model";
import { ListSectorsUseCase } from "src/modules";
import { Sector } from "src/typings";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";

export default function MonetizationBalance() {
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const { myUser } = useContext(UserInfoContext);
    const [collaborator, setCollaborator] = useState<string>("");
    const [collaboratorDebit, setCollaboratorDebit] = useState<string>("");
    const [amount, setAmount] = useState<number>(0);
    const [amountDebit, setAmountDebit] = useState<number>(0);
    const [reason, setReason] = useState<string>("");
    const [reasonDebit, setReasonDebit] = useState<string>("");
    const { myPermissions } = useContext(PermissionsContext);
    const [modalConfirmDebit, setModalConfirmDebit] = useState<boolean>(false);
    const [modalConfirmCredit, setModalConfirmCredit] =
        useState<boolean>(false);

    const [collaboratorSelected, setCollaboratorSelected] =
        useState<Collaborator | null>(null);
    const [collaboratorSelectedDebit, setCollaboratorSelectedDebit] =
        useState<Collaborator | null>(null);
    const [collaboratorsSearchValue, setCollaboratorsSearchValue] =
        useState<string>("");
    const [collaboratorsSearchValueDebit, setCollaboratorsSearchValueDebit] =
        useState<string>("");
    const debouncedCollaboratorsSearchTermDebit: string = useDebounce<string>(
        collaboratorsSearchValueDebit,
        400
    );
    const [collaborators, setCollaborators] = useState<
        {
            id: number;
            name: string;
            registry: string;
        }[]
    >([]);
    const [collaboratorsDebit, setCollaboratorsDebit] = useState<
        {
            id: number;
            name: string;
            registry: string;
        }[]
    >([]);
    const debouncedCollaboratorsSearchTerm: string = useDebounce<string>(
        collaboratorsSearchValue,
        400
    );
    const [selectedSector, setSelectedSector] = useState<Sector | null>(null);
    const [selectedSectorDebit, setSelectedSectorDebit] =
        useState<Sector | null>(null);
    const [sectors, setSectors] = useState<Sector[]>([]);
    const [sectorsDebit, setSectorsDebit] = useState<Sector[]>([]);
    const [sectorsSearchValue, setSectorsSearchValue] = useState<string>("");
    const debouncedSectorsSearchValue: string = useDebounce<string>(
        sectorsSearchValue,
        400
    );

    const [sectorsSearchValueDebit, setSectorsSearchValueDebit] =
        useState<string>("");
    const debouncedSectorsSearchValueDebit: string = useDebounce<string>(
        sectorsSearchValueDebit,
        400
    );

    const getSectorsListDebit = async () => {
        const pagination = {
            limit: 20,
            offset: 0,
            searchText: sectorsSearchValueDebit,
            deleted: false,
        };

        new ListSectorsUseCase()
            .handle(pagination)
            .then((data) => {
                setSectorsDebit(data.items);
            })
            .catch(() => {
                // toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {});
    };

    useEffect(() => {
        getSectorsListDebit();
    }, [debouncedSectorsSearchValueDebit]);

    const getSectorsList = async () => {
        const pagination = {
            limit: 20,
            offset: 0,
            searchText: sectorsSearchValue,
            deleted: false,
        };

        new ListSectorsUseCase()
            .handle(pagination)
            .then((data) => {
                setSectors(data.items);
            })
            .catch(() => {
                // toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {});
    };

    useEffect(() => {
        getSectorsList();
    }, [debouncedSectorsSearchValue]);

    const getCollaboratorsListDebit = async () => {
        const pagination: PaginationModel = {
            limit: 20,
            offset: 0,
            searchText: collaboratorsSearchValueDebit,
        };

        new ListCollaboratorsAllUseCase()
            .handle(pagination)
            .then((data) => {
                setCollaboratorsDebit(data.items);
            })
            .catch(() => {});
    };

    useEffect(() => {
        getCollaboratorsListDebit();
    }, [debouncedCollaboratorsSearchTermDebit]);

    const getCollaboratorsList = async () => {
        const pagination: PaginationModel = {
            limit: 20,
            offset: 0,
            searchText: collaboratorsSearchValue,
        };

        new ListCollaboratorsAllUseCase()
            .handle(pagination)
            .then((data) => {
                setCollaborators(data.items);
            })
            .catch(() => {});
    };

    useEffect(() => {
        getCollaboratorsList();
    }, [debouncedCollaboratorsSearchTerm]);

    function addCredit() {
        if (!collaboratorSelected) return;

        setIsLoading(true);

        const payload = {
            amount: amount,
            collaborator: collaboratorSelected.id.toString(),
            reason: reason,
        };

        new AddCreditUseCase()
            .handle(payload)
            .then((data) => {
                toast.success("Créditos adicionados com sucesso!");
            })
            .catch((e) => {
                if (e.response.data.message == "collaborator not found") {
                    toast.warning(`O colaborador (${collaborator}) não existe`);
                } else {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                }
            })
            .finally(() => {
                setIsLoading(false);
            });
    }

    function removeCredit() {
        if (!collaboratorSelectedDebit) return;

        setIsLoading(true);

        const payload = {
            amount: amountDebit,
            collaborator: collaboratorSelectedDebit.id.toString(),
            reason: reasonDebit,
        };

        new RemoveCreditUseCase()
            .handle(payload)
            .then((data) => {
                toast.success("Créditos removidos com sucesso!");
            })
            .catch((e) => {
                if (e.response.data.message == "collaborator not found") {
                    toast.warning(`O colaborador (${collaborator}) não existe`);
                } else if (e?.response?.data?.code == "OUT_OF_RANGE") {
                    toast.warning(
                        `O colaborador tem apenas ${e?.response?.data?.keys?.currentBalance}, você não pode remover ${e?.response?.data?.keys?.totalPrice}`
                    );
                } else {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                }
            })
            .finally(() => {
                setIsLoading(false);
            });
    }

    function downloadExampleFile() {
        try {
            let indicatorSheetBuilder = new SheetBuilder();
            const docRows = [
                [
                    "701458",
                    "BERNARDO AUGUSTO DE BRITO",
                    "1889",
                    "SKY - PRE PAGO",
                    "Crédito",
                    "15",
                    "Campanha X",
                    "Metiro pelo dia 15 de agosto",
                ],
                [
                    "701458",
                    "BERNARDO AUGUSTO DE BRITO",
                    "1889",
                    "SKY - PRE PAGO",
                    "Débito",
                    "10",
                    "Campanha Y",
                    "Metiro pelo dia 01 de setembro",
                ],
            ];
            indicatorSheetBuilder
                .setHeader([
                    "Matrícula",
                    "Nome Operador",
                    "Código GIP",
                    "Setor",
                    "Operação",
                    "Moedas",
                    "Motivo",
                    "Observação",
                ])
                .append(docRows)
                .exportAs(`Exemplo_importação_massiva_créditos_débitos`);
            toast.success("Exemplo exportado com sucesso!");
        } catch (error) {
            toast.error("Falha na exportação do exemplo!");
        }
    }

    if (
        abilityFor(myPermissions).cannot(
            "Adicionar Creditos a um colaborador",
            "Monetização"
        )
    ) {
        return (
            <Box
                width={"100%"}
                display={"flex"}
                flexDirection={"column"}
                gap={2}
            >
                <WithoutPermissionCard />
            </Box>
        );
    }

    async function handleRemoveCredits() {
        if (!collaboratorSelectedDebit) return;

        await new AccountBalanceUseCase()
            .handle({ userId: collaboratorSelectedDebit.id })
            .then((data) => {
                const balance = data.balance;

                if (balance - amountDebit <= 0) {
                    setModalConfirmDebit(true);
                } else {
                    removeCredit();
                }
            })
            .catch((e) => {
                console.log(e);
            });
    }

    return (
        <Box width={"100%"} display={"flex"} flexDirection={"column"} gap={2}>
            <BaseModal
                width={"540px"}
                open={modalConfirmDebit}
                title={`Suas informações`}
                onClose={() => setModalConfirmDebit(false)}
            >
                <Box
                    width={"100%"}
                    display={"flex"}
                    flexDirection={"column"}
                    justifyContent={"center"}
                    alignItems={"center"}
                    gap={2}
                >
                    <Typography textAlign={"center"} maxWidth={"400px"}>
                        Você vai zerar a conta do colaborador{" "}
                        {collaboratorDebit} deseja continuar?
                    </Typography>
                    <Box display={"flex"} gap={2} width={"100%"}>
                        <Button
                            variant="contained"
                            color="error"
                            fullWidth
                            onClick={() => removeCredit()}
                        >
                            Sim
                        </Button>
                        <Button
                            variant="contained"
                            color="success"
                            fullWidth
                            onClick={() => setModalConfirmDebit(false)}
                        >
                            Não
                        </Button>
                    </Box>
                </Box>
            </BaseModal>
            <Card
                width={"100%"}
                display={"flex"}
                flexDirection={"column"}
                justifyContent={"space-between"}
            >
                <PageHeader
                    title={`Adicionar créditos a um colaborador`}
                    headerIcon={<Savings />}
                />
                <Stack px={2} py={3} width={"100%"} gap={1}>
                    {/* <TextField
                        type="number"
                        label="Código do Colaborador"
                        value={collaborator}
                        onChange={(e) => setCollaborator(e.target.value)}
                    /> */}
                    <Autocomplete
                        options={collaborators}
                        disableClearable={false}
                        getOptionLabel={(option) => option.name}
                        onChange={(event, value) => {
                            setCollaboratorSelected(value as Collaborator);
                        }}
                        onInputChange={(e, text) =>
                            setCollaboratorsSearchValue(text)
                        }
                        filterOptions={(x) => x}
                        filterSelectedOptions
                        fullWidth
                        renderInput={(params) => (
                            <TextField
                                {...params}
                                variant="outlined"
                                label="Selecione um colaborador"
                                placeholder="Buscar"
                            />
                        )}
                        renderOption={(props, option) => {
                            return (
                                <li {...props} key={option.id}>
                                    {option.name}
                                </li>
                            );
                        }}
                        isOptionEqualToValue={(option, value) =>
                            option.name === value.name
                        }
                        sx={{ mb: 0 }}
                    />
                    <Autocomplete
                        value={selectedSector}
                        placeholder={"Setor"}
                        disableClearable={false}
                        onChange={(e, value) => setSelectedSector(value)}
                        onInputChange={(e, text) => setSectorsSearchValue(text)}
                        sx={{ mb: 0, width: "100%" }}
                        renderInput={(props) => (
                            <TextField
                                {...props}
                                label={"Setor"}
                                InputProps={{
                                    ...props.InputProps,
                                    endAdornment: (
                                        <>
                                            {isLoading ? (
                                                <CircularProgress
                                                    color="primary"
                                                    size={20}
                                                />
                                            ) : (
                                                props.InputProps.endAdornment
                                            )}
                                        </>
                                    ),
                                }}
                            />
                        )}
                        isOptionEqualToValue={(option, value) =>
                            option.name === value.name
                        }
                        getOptionLabel={(option) =>
                            `${option.id} - ${option.name}`
                        }
                        options={sectors}
                    />
                    <TextField
                        type="number"
                        label="Quantidade"
                        value={amount}
                        onChange={(e) => setAmount(parseInt(e.target.value))}
                    />
                    <TextField
                        label="Motivo"
                        value={reason}
                        onChange={(e) => setReason(e.target.value)}
                    />
                    <LoadingButton
                        variant="contained"
                        onClick={() => setModalConfirmCredit(true)}
                        loading={isLoading}
                    >
                        Adicionar créditos
                    </LoadingButton>
                </Stack>
            </Card>
            <Card
                width={"100%"}
                display={"flex"}
                flexDirection={"column"}
                justifyContent={"space-between"}
            >
                <PageHeader
                    title={`Remover créditos de um colaborador`}
                    headerIcon={<Remove />}
                />
                <Stack px={2} py={3} width={"100%"} gap={1}>
                    <Autocomplete
                        options={collaboratorsDebit}
                        disableClearable={false}
                        getOptionLabel={(option) => option.name}
                        onChange={(event, value) => {
                            setCollaboratorSelectedDebit(value as Collaborator);
                        }}
                        onInputChange={(e, text) =>
                            setCollaboratorsSearchValueDebit(text)
                        }
                        filterOptions={(x) => x}
                        filterSelectedOptions
                        fullWidth
                        renderInput={(params) => (
                            <TextField
                                {...params}
                                variant="outlined"
                                label="Selecione um colaborador"
                                placeholder="Buscar"
                            />
                        )}
                        renderOption={(props, option) => {
                            return (
                                <li {...props} key={option.id}>
                                    {option.name}
                                </li>
                            );
                        }}
                        isOptionEqualToValue={(option, value) =>
                            option.name === value.name
                        }
                        sx={{ mb: 0 }}
                    />
                    <Autocomplete
                        value={selectedSectorDebit}
                        placeholder={"Setor"}
                        disableClearable={false}
                        onChange={(e, value) => setSelectedSectorDebit(value)}
                        onInputChange={(e, text) =>
                            setSectorsSearchValueDebit(text)
                        }
                        sx={{ mb: 0, width: "100%" }}
                        renderInput={(props) => (
                            <TextField
                                {...props}
                                label={"Setor"}
                                InputProps={{
                                    ...props.InputProps,
                                    endAdornment: (
                                        <>
                                            {isLoading ? (
                                                <CircularProgress
                                                    color="primary"
                                                    size={20}
                                                />
                                            ) : (
                                                props.InputProps.endAdornment
                                            )}
                                        </>
                                    ),
                                }}
                            />
                        )}
                        isOptionEqualToValue={(option, value) =>
                            option.name === value.name
                        }
                        getOptionLabel={(option) =>
                            `${option.id} - ${option.name}`
                        }
                        options={sectorsDebit}
                    />
                    <TextField
                        type="number"
                        label="Quantidade"
                        value={amountDebit}
                        onChange={(e) =>
                            setAmountDebit(parseInt(e.target.value))
                        }
                    />
                    <TextField
                        label="Motivo"
                        value={reasonDebit}
                        onChange={(e) => setReasonDebit(e.target.value)}
                    />
                    <LoadingButton
                        variant="contained"
                        onClick={handleRemoveCredits}
                        loading={isLoading}
                    >
                        Remover créditos
                    </LoadingButton>
                </Stack>
            </Card>
            <Card
                width={"100%"}
                display={"flex"}
                flexDirection={"column"}
                justifyContent={"space-between"}
            >
                <PageHeader
                    title={`Input Massivo de créditos/débitos`}
                    headerIcon={<FileOpen />}
                    secondaryButtonIcon={<Download />}
                    secondaryButtonTitle="Baixar exemplo"
                    secondayButtonAction={() => downloadExampleFile()}
                />
                <Stack px={2} py={3} width={"100%"} gap={1}>
                    <MassiveCredits />
                </Stack>
            </Card>
            {/* <Card
                width={"100%"}
                display={"flex"}
                flexDirection={"column"}
                justifyContent={"space-between"}
            >
                <PageHeader
                    title={`Input Massivo de débitos`}
                    headerIcon={<FileOpen />}
                    secondaryButtonIcon={<Download />}
                    secondaryButtonTitle="Baixar exemplo"
                    secondayButtonAction={() => downloadExampleFile()}
                />
                <Stack px={2} py={3} width={"100%"} gap={1}>
                    <MassiveDebits />
                </Stack>
            </Card> */}
            <BaseModal
                width={"540px"}
                open={modalConfirmCredit}
                title={`Adicionar créditos`}
                onClose={() => setModalConfirmCredit(false)}
            >
                <Box width={"100%"} display={"flex"} flexDirection={"column"}>
                    <Typography variant="body2" textAlign={"center"} mt={2}>
                        Tem certeza de que deseja inserir {amount} moedas para
                        esse colaborador?
                    </Typography>
                    <Typography variant="body2" textAlign={"center"} mt={2}>
                        {collaboratorSelected?.id} -{" "}
                        {collaboratorSelected?.name}
                    </Typography>
                    <Button
                        color="error"
                        variant="contained"
                        fullWidth
                        sx={{ mt: 4 }}
                        onClick={() => setModalConfirmCredit(false)}
                    >
                        Não
                    </Button>
                    <LoadingButton
                        color="success"
                        variant="contained"
                        fullWidth
                        sx={{ mt: 1 }}
                        onClick={addCredit}
                        loading={isLoading}
                    >
                        Sim
                    </LoadingButton>
                </Box>
            </BaseModal>
        </Box>
    );
}

MonetizationBalance.getLayout = getLayout("private");
