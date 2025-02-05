import { useRouter } from "next/router";
import {
    Autocomplete,
    Box,
    Stack,
    Typography,
    TextField,
    List,
    ListItem,
    ListItemIcon,
    ListItemText,
    Divider,
    Button,
} from "@mui/material";
import { useContext, useEffect, useState } from "react";
import { Indicator, Sector } from "src/typings";
import { Card, PageHeader } from "src/components";
import { DateUtils, getLayout } from "src/utils";
import { DetailsSectorUseCase } from "src/modules";
import { toast } from "react-toastify";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { GoBack } from "src/components/navigation/go-back";
import CalendarMonth from "@mui/icons-material/CalendarMonth";
import DriveFileRenameOutline from "@mui/icons-material/DriveFileRenameOutline";
import ImportExport from "@mui/icons-material/ImportExport";
import Link from "@mui/icons-material/Link";
import { LoadingButton } from "@mui/lab";
import { useLoadingState } from "src/hooks";
import { BaseModal, ConfirmModal } from "src/components/feedback";
import { DeleteSectorUseCase } from "src/modules/sectors/use-cases/delete-sector.use-case";
import { Can } from "@casl/react";
import abilityFor from "src/utils/ability-for";
import { PaginationModel } from "src/typings/models/pagination.model";
import { ListIndicatorsUseCase } from "src/modules/indicators/use-cases";
import { ConnectIndicatorstoSector } from "src/modules/indicators/use-cases/connect-indicators-to-sector.use-case";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";

export default function SectorDetails() {
    const { isLoading, startLoading, finishLoading } = useLoadingState();
    const router = useRouter();
    const { myPermissions } = useContext(PermissionsContext);
    const { id } = router.query;
    const [sector, setSector] = useState<Sector | null>(null);
    const [modalConfirmVisible, setModalConfirmVisible] =
        useState<boolean>(false);
    const [associateIndicatorModalIsOpen, setAssociateIndicatorModalIsOpen] =
        useState<boolean>(false);
    const [sectors, setSectors] = useState<Sector[]>([]);
    const [indicatorsSearchValue, setIndicatorsSearchValue] =
        useState<string>("");
    const [selectedIndicators, setSelectedIndicators] = useState<Indicator[]>(
        []
    );
    const [indicators, setIndicators] = useState<Indicator[]>([]);
    const [sectorsSearchValue, setSectorsSearchValue] = useState<string>("");
    const [associateIsLoading, setAssociateIsLoading] =
        useState<boolean>(false);
    const [isLoadingIndicators, setIsLoadingIndicators] =
        useState<boolean>(false);

    useEffect(() => {
        if (id !== undefined) {
            const payload = {
                id: id,
            };

            new DetailsSectorUseCase()
                .handle(payload)
                .then((data) => {
                    console.log(data);
                    setSector(data);
                    data.indicators && setSelectedIndicators(data.indicators);
                })
                .catch(() => {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                });
        }
    }, [id]);

    const getIndicatorsList = async () => {
        const pagination: PaginationModel = {
            limit: 20,
            offset: 0,
            searchText: indicatorsSearchValue,
        };

        setIsLoadingIndicators(true);

        new ListIndicatorsUseCase()
            .handle(pagination)
            .then((data) => {
                setIndicators(data.items);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                setIsLoadingIndicators(false);
            });
    };

    useEffect(() => {
        getIndicatorsList();
    }, [indicatorsSearchValue]);

    const handleDeleteIndicator = async (id: number) => {
        startLoading();
        await new DeleteSectorUseCase()
            .handle(id)
            .then((data) => {
                toast.success(`Setor deletado com sucesso`);
                router.push(`/sectors`);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                finishLoading();
            });
    };

    const handleConnectSectorsToIndicator = () => {
        if (selectedIndicators.length <= 0) {
            return toast.warning("Selecione pelo menos um indicador");
        }
        if (sector) {
            setAssociateIsLoading(true);

            new ConnectIndicatorstoSector()
                .handle({
                    sectorId: sector?.id,
                    indicatorsIds: selectedIndicators.map(
                        (sector) => sector.id
                    ),
                })
                .then((data) => {
                    toast.success(`Setores associados com sucesso!`);
                })
                .catch(() => {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                })
                .finally(() => {
                    setAssociateIsLoading(false);
                });
        }
    };

    return (
        <Card
            width={"100%"}
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"space-between"}
        >
            <Stack px={2} py={3} width={"100%"}>
                <ConfirmModal
                    open={modalConfirmVisible}
                    onClose={() => setModalConfirmVisible(false)}
                    onConfirm={() => sector && handleDeleteIndicator(sector.id)}
                    text={"Você deseja apagar o setor?"}
                />
                <GoBack pushTo={"/sectors"} />
                <PageHeader title={`Detalhes do Setor`} />
                {sector && (
                    <Box
                        sx={{
                            width: "100%",
                            flexDirection: "column",
                            display: "flex",
                            gap: "10px",
                        }}
                    >
                        <List dense={true}>
                            <ListItem>
                                <ListItemIcon>
                                    <DriveFileRenameOutline />
                                </ListItemIcon>
                                <ListItemText
                                    primary="Cod. GIP"
                                    secondary={sector.id}
                                />
                            </ListItem>
                            <ListItem>
                                <ListItemIcon>
                                    <DriveFileRenameOutline />
                                </ListItemIcon>
                                <ListItemText
                                    primary="Nome do setor"
                                    secondary={sector.name}
                                />
                            </ListItem>
                            <Divider />
                            <ListItem>
                                <ListItemIcon>
                                    <DriveFileRenameOutline />
                                </ListItemIcon>
                                <ListItemText
                                    primary="Level"
                                    secondary={sector.level}
                                />
                            </ListItem>
                            <Divider />
                            <ListItem>
                                <ListItemIcon>
                                    <CalendarMonth />
                                </ListItemIcon>
                                <ListItemText
                                    primary="Criado em"
                                    secondary={
                                        sector.createdAt
                                            ? DateUtils.formatDatePtBRWithTime(
                                                  sector.createdAt
                                              )
                                            : ""
                                    }
                                />
                            </ListItem>
                            <Divider />
                            <ListItem>
                                <ListItemIcon>
                                    <Link />
                                </ListItemIcon>
                                <ListItemText
                                    primary="Indicadores associados"
                                    secondary={
                                        sector.indicators &&
                                        sector.indicators.length > 0
                                            ? sector.indicators?.map(
                                                  (item) => `${item.name}; `
                                              )
                                            : "(Nenhum indicador associado)"
                                    }
                                />
                                <Button
                                    variant="contained"
                                    onClick={() =>
                                        setAssociateIndicatorModalIsOpen(true)
                                    }
                                    disabled={isLoadingIndicators}
                                >
                                    Editar
                                </Button>
                            </ListItem>
                        </List>
                        <Can
                            I="Editar Setores"
                            a="Setores"
                            ability={abilityFor(myPermissions)}
                        >
                            {() => (
                                <Box
                                    display={"flex"}
                                    justifyContent={"flex-end"}
                                    px={2}
                                    py={3}
                                >
                                    <LoadingButton
                                        loading={isLoading}
                                        variant={"contained"}
                                        color={"error"}
                                        onClick={() =>
                                            setModalConfirmVisible(true)
                                        }
                                    >
                                        Apagar Setor
                                    </LoadingButton>
                                </Box>
                            )}
                        </Can>
                    </Box>
                )}
            </Stack>
            <BaseModal
                width={"540px"}
                open={associateIndicatorModalIsOpen}
                title={`Editar indicadores associados`}
                onClose={() => setAssociateIndicatorModalIsOpen(false)}
            >
                <Box width={"100%"} display={"flex"} flexDirection={"column"}>
                    <Autocomplete
                        multiple
                        disabled={abilityFor(myPermissions).cannot(
                            "Editar Indicadores",
                            "Indicadores"
                        )}
                        size={"small"}
                        value={selectedIndicators}
                        options={indicators}
                        getOptionLabel={(option) => option.name}
                        onChange={(event, value) => {
                            setSelectedIndicators(value);
                        }}
                        onInputChange={(e, text) =>
                            setIndicatorsSearchValue(text)
                        }
                        filterOptions={(x) => x}
                        filterSelectedOptions
                        renderInput={(params) => (
                            <TextField
                                {...params}
                                variant="outlined"
                                label="Selecione um ou mais indicadores "
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
                    />
                    <LoadingButton
                        loading={associateIsLoading}
                        variant={"contained"}
                        onClick={handleConnectSectorsToIndicator}
                        disabled={abilityFor(myPermissions).cannot(
                            "Editar Indicadores",
                            "Indicadores"
                        )}
                    >
                        Associar Setores
                    </LoadingButton>
                </Box>
            </BaseModal>
        </Card>
    );
}

SectorDetails.getLayout = getLayout("private");
