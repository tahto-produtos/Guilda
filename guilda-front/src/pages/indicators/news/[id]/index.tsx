import { useRouter } from "next/router";
import {
    Box,
    Stack,
    List,
    ListItem,
    ListItemIcon,
    ListItemText,
    Divider,
    TextField,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
    Autocomplete,
} from "@mui/material";
import { useEffect, useState } from "react";
import { Indicator, Sector } from "src/typings";
import { Card, PageHeader } from "src/components";
import { getLayout } from "src/utils";
import { toast } from "react-toastify";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { GoBack } from "src/components/navigation/go-back";
import { DetailsIndicatorUseCase } from "src/modules/indicators/use-cases";
import { DeleteIndicatorUseCase } from "src/modules/indicators/use-cases/delete-indicator.use-case";
import { DateUtils } from "src/utils";
import { LoadingButton } from "@mui/lab";
import { useLoadingState } from "src/hooks";
import CalendarMonth from "@mui/icons-material/CalendarMonth";
import DriveFileRenameOutline from "@mui/icons-material/DriveFileRenameOutline";
import FormatQuote from "@mui/icons-material/FormatQuote";
import Link from "@mui/icons-material/Link";
import Pin from "@mui/icons-material/Pin";
import { BaseModal, ConfirmModal } from "src/components/feedback";
import Calculate from "@mui/icons-material/Calculate";
import ToggleOff from "@mui/icons-material/ToggleOff";
import ToggleOn from "@mui/icons-material/ToggleOn";
import { ChangeIndicatorStatusUseCase } from "src/modules/indicators/use-cases/change-indicator-status.use-case";
import Scale from "@mui/icons-material/Scale";
import { UpdateNewIndicatorUseCase } from "src/modules/indicators/use-cases/update-new-indicator.use-case";
import { PaginationModel } from "src/typings/models/pagination.model";
import { ListSectorsUseCase } from "src/modules";

export default function NewIndicatorsDetails() {
    const { isLoading, startLoading, finishLoading } = useLoadingState();

    const router = useRouter();
    const { id } = router.query;
    const [indicator, setIndicator] = useState<Indicator | null>(null);
    const [modalConfirmVisible, setModalConfirmVisible] =
        useState<boolean>(false);
    const [isOpen, setIsOpen] = useState<boolean>(false);
    const [nameEdit, setNameEdit] = useState<string>("");
    const [descEdit, setDescEdit] = useState<string>("");
    const [expressionEdit, setExpressionEdit] = useState<string>("");
    const [calculationTypeEdit, setCalculationTypeEdit] = useState<string>("");
    const [weightEdit, setWeightEdit] = useState<number>(0);
    const [idEdit, setIdEdit] = useState<number>(0);
    const [sectorsEdit, setSectorsEdit] = useState<Sector[]>([]);
    const [sectors, setSectors] = useState<Sector[]>([]);
    const [sectorSearchValue, setSectorsSearchValue] = useState<string>("");

    function handleOpenEdit() {
        if (!indicator) {
            return;
        }
        setIdEdit(indicator?.id);
        setCalculationTypeEdit(indicator?.calculationType || "");
        setWeightEdit(indicator?.weight || 0);
        setNameEdit(indicator?.name || "");
        setDescEdit(indicator?.description || "");
        setExpressionEdit(indicator?.mathematicalExpression.expression || "");
        setSectorsEdit(indicator.sectors || []);
        setIsOpen(true);
    }

    useEffect(() => {
        getDetails();
    }, [id]);

    async function getDetails() {
        //console.log(id);
        if (id !== undefined) {
            const payload = {
                id: id,
            };

            new DetailsIndicatorUseCase()
                .handle(payload)
                .then((data) => {
                    setIndicator(data);
                })
                .catch(() => {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                });
        }
    }

    const getSectorsList = async () => {
        const pagination: PaginationModel = {
            limit: 20,
            offset: 0,
            searchText: sectorSearchValue,
        };

        new ListSectorsUseCase()
            .handle(pagination)
            .then((data) => {
                setSectors(data.items);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {});
    };

    useEffect(() => {
        getSectorsList();
    }, [sectorSearchValue]);

    const handleDeleteIndicator = async (id: number) => {
        startLoading();
        await new DeleteIndicatorUseCase()
            .handle(id)
            .then(() => {
                toast.success(`Indicador deletado com sucesso`);
                router.push(`/indicators`);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                finishLoading();
            });
    };

    async function handleStatusChange() {
        if (!indicator) {
            return;
        }

        startLoading();

        const statusToSwitch: boolean = indicator.status ? false : true;

        await new ChangeIndicatorStatusUseCase()
            .handle({ indicatorId: indicator.id, status: statusToSwitch })
            .then(() => {
                toast.success(
                    `Indicador ${
                        statusToSwitch ? "ativado" : "inativado"
                    } com sucesso`
                );
                getDetails();
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    }

    async function handleSaveEdit() {
        if (!id) {
            return;
        }

        startLoading();
        
        const sectorsIds = sectorsEdit.map(objeto => objeto.id);
        const concatSectorsIds = sectorsIds.join(';');
        
        const payload = {
            INDICATORID: parseInt(id.toString()),
            NAME: nameEdit,
            DESCRIPTION: descEdit,
            METRIC: expressionEdit,
            CALCTYPE: calculationTypeEdit,
            WEIGHT: weightEdit,
            SECTOR: concatSectorsIds,
        };

        await new UpdateNewIndicatorUseCase()
            .handle(payload)
            .then(() => {
                toast.success(`Indicador editado com sucesso`);
                setIsOpen(false);
                router.push(`/indicators/news`);
                getDetails();
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                finishLoading();
            })
            .finally(() => {
                finishLoading();
            });
    }

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
                    onConfirm={() =>
                        indicator && handleDeleteIndicator(indicator.id)
                    }
                    text={"Você deseja apagar o indicador?"}
                />
                <GoBack pushTo={"/indicators/news"} />
                <PageHeader title={`Detalhes do novo Indicador`} />
                {indicator && (
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
                                    primary="Nome do indicador"
                                    secondary={indicator.name}
                                />
                            </ListItem>
                            <Divider />
                            <ListItem>
                                <ListItemIcon>
                                    <Pin />
                                </ListItemIcon>
                                <ListItemText
                                    primary="Código do indicador"
                                    secondary={indicator.id}
                                />
                            </ListItem>
                            <Divider />
                            <ListItem>
                                <ListItemIcon>
                                    <FormatQuote />
                                </ListItemIcon>
                                <ListItemText
                                    primary="Descrição"
                                    secondary={indicator.description}
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
                                        indicator.createdAt
                                            ? DateUtils.formatDatePtBRWithTime(
                                                  indicator.createdAt
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
                                    primary="Setores Associados"
                                    secondary={
                                        indicator.sectors &&
                                        indicator.sectors.length > 0
                                            ? indicator.sectors?.map(
                                                  (item) => `${item.name}; `
                                              )
                                            : "(Nenhum setor associado)"
                                    }
                                />
                            </ListItem>
                            <Divider />
                            <ListItem>
                                <ListItemIcon>
                                    <Scale />
                                </ListItemIcon>
                                <ListItemText
                                    primary="Peso"
                                    secondary={indicator?.weight || ""}
                                />
                            </ListItem>
                            <Divider />
                            <ListItem>
                                <ListItemIcon>
                                    <Calculate />
                                </ListItemIcon>
                                <ListItemText
                                    primary="Tipo de Cálculo"
                                    secondary={
                                        indicator?.calculationType &&
                                        indicator?.calculationType ==
                                            "BIGGER_BETTER"
                                            ? "Maior Melhor"
                                            : indicator?.calculationType &&
                                              indicator?.calculationType ==
                                                  "LESS_BETTER"
                                            ? "Menor Melhor"
                                            : ""
                                    }
                                />
                            </ListItem>
                            <Divider />
                            <ListItem>
                                <ListItemIcon>
                                    <Calculate />
                                </ListItemIcon>
                                <ListItemText
                                    primary="Métrica do Indicador"
                                    secondary={
                                        indicator?.mathematicalExpression
                                            ?.expression ||
                                        "Sem métrica definida"
                                    }
                                />
                            </ListItem>
                            <Divider />
                            <ListItem>
                                <ListItemIcon>
                                    {indicator?.status ? (
                                        <ToggleOn />
                                    ) : (
                                        <ToggleOff />
                                    )}
                                </ListItemIcon>
                                <ListItemText
                                    primary="Status"
                                    secondary={
                                        indicator?.status ? "Ativo" : "Inativo"
                                    }
                                />
                            </ListItem>
                        </List>
                        {indicator && (
                            <Box
                                display={"flex"}
                                justifyContent={"flex-end"}
                                px={2}
                                py={3}
                                gap={1}
                            >
                                <LoadingButton
                                    loading={isLoading}
                                    variant={"outlined"}
                                    onClick={handleOpenEdit}
                                >
                                    Editar novo indicador
                                </LoadingButton>
                                {/* <LoadingButton
                                    loading={isLoading}
                                    variant={"outlined"}
                                    color={
                                        indicator.status ? "error" : "success"
                                    }
                                    onClick={() => handleStatusChange()}
                                >
                                    {indicator.status
                                        ? "Inativar indicador"
                                        : "Ativar indicador"}
                                </LoadingButton> */}
                                {/* <LoadingButton
                                    loading={isLoading}
                                    variant={"contained"}
                                    color={"error"}
                                    onClick={() => setModalConfirmVisible(true)}
                                >
                                    Apagar Indicador
                                </LoadingButton> */}
                            </Box>
                        )}
                    </Box>
                )}
            </Stack>
            <BaseModal
                width={"540px"}
                open={isOpen}
                title={`Editar indicador`}
                onClose={() => setIsOpen(false)}
            >
                <Box
                    width={"100%"}
                    display={"flex"}
                    flexDirection={"column"}
                    gap={1}
                >
                    {/* <TextField
                        value={idEdit}
                        label={"Código do indicador"}
                        onChange={(e) =>
                            setIdEdit(parseInt(e.target.value.toString()))
                        }
                        fullWidth
                    /> */}
                    <TextField
                        value={nameEdit}
                        label={"Nome do indicador"}
                        onChange={(e) => setNameEdit(e.target.value)}
                        fullWidth
                    />
                    <TextField
                        value={descEdit}
                        label={"Descrição do indicador"}
                        onChange={(e) => setDescEdit(e.target.value)}
                        fullWidth
                    />
                    <Autocomplete
                        multiple
                        size={"small"}
                        value={sectorsEdit}
                        options={sectors}
                        getOptionLabel={(option) => option.name}
                        onChange={(event, value) => {
                            setSectorsEdit(value);
                        }}
                        onInputChange={(e, text) => setSectorsSearchValue(text)}
                        filterOptions={(x) => x}
                        filterSelectedOptions
                        renderInput={(params) => (
                            <TextField
                                {...params}
                                variant="outlined"
                                label="Selecione um ou mais setores "
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
                    <TextField
                        value={expressionEdit}
                        label={"Métrica do Indicador"}
                        onChange={(e) => setExpressionEdit(e.target.value)}
                        fullWidth
                    />
                    <FormControl sx={{ width: "100%" }}>
                        <InputLabel
                            sx={{ backgroundColor: "#fff" }}
                            size="small"
                        >
                            Tipo de cálculo
                        </InputLabel>
                        <Select
                            onChange={(e) =>
                                setCalculationTypeEdit(e.target.value)
                            }
                            value={calculationTypeEdit}
                            size="small"
                        >
                            <MenuItem value={""}>Nenhum</MenuItem>
                            <MenuItem value={"BIGGER_BETTER"}>
                                Maior Melhor
                            </MenuItem>
                            <MenuItem value={"LESS_BETTER"}>
                                Menor Melhor
                            </MenuItem>
                        </Select>
                    </FormControl>
                    <TextField
                        value={weightEdit}
                        label={"Peso"}
                        onChange={(e) =>
                            setWeightEdit(parseInt(e.target.value.toString()))
                        }
                        fullWidth
                    />
                    <LoadingButton
                        variant="contained"
                        loading={isLoading}
                        onClick={handleSaveEdit}
                    >
                        Salvar
                    </LoadingButton>
                </Box>
            </BaseModal>
        </Card>
    );
}

NewIndicatorsDetails.getLayout = getLayout("private");
