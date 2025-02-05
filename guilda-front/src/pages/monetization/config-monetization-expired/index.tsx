import InsertDriveFile from "@mui/icons-material/InsertDriveFile";
import DownloadRounded from "@mui/icons-material/DownloadRounded";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { useLoadingState } from "src/hooks";
import { DateUtils, SheetBuilder, getLayout } from "src/utils";
import abilityFor from "src/utils/ability-for";
import { LoadingButton } from "@mui/lab";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { ListMonetizationExpireUseCase } from "src/modules/monetization/use-cases/list-monetization-expire.use-case";
import { ListMonetizationExpiredTable } from "src/modules/monetization/tables/list-monetization-expired";
import { Autocomplete, Box, TextField, Stack, Typography, Button, useTheme } from "@mui/material";
import { ListTypeConfigMonetizationUseCase } from "src/modules/monetization/use-cases/list-type-config-monetization.use-case";
import { ListSectorsAndSubsectrosUseCase } from "src/modules";
import { ActionButton } from "src/components/inputs/action-button/action-button";
import { ReplyOutlined } from "@mui/icons-material";
import { ListConfigDayMonetizationUseCase } from "src/modules/monetization/use-cases/list-config-day-monetization.use-case";
import { MonetizationConfigDayUseCase } from "src/modules/monetization/use-cases/monetization-config-day.use-case";
import { SitePersonaResponse, SitePersonaUseCase } from "src/modules/personas/use-cases/site-personas.use-case";
import Download from "@mui/icons-material/Download";
import FileOpen from "@mui/icons-material/FileOpen";
import CheckCircle from "@mui/icons-material/CheckCircle";
import { grey } from "@mui/material/colors";
import { InputMassiveExpirationDateUseCase } from "src/modules/monetization/use-cases/input-massive-expiration-date.use-case"
import { ExportMassiveExpirationDateUseCase } from "src/modules/monetization/use-cases/export-massive-expiration-date.use-case";


export default function ConfigMonetizationExpiration() {
    const { myPermissions } = useContext(PermissionsContext);
    const [typesConfg, setTypesConfig] = useState<any[]>([]);
    const [typeConfigSelected, setTypeConfigSelected] = useState<any>();
    const [sectors, setSectors] = useState<any[]>([]);
    const [sectorSelected, setSectorSelected] = useState<any>();
    const [sectorSearch, setSectorSearch] = useState<string>("");

    const [daysExpired, setDaysExpired] = useState<number>();
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [limit, setLimit] = useState<number>(100);
    const { myUser } = useContext(UserInfoContext);
    const [sites, setSites] = useState<SitePersonaResponse[]>([]);
    const [siteSelected, setSiteSelected] = useState<any>();


    const [success, setSuccess] = useState<{
        totalCreated: number;
        totalUpdated: number;
    } | null>(null);
    const [hasErrors, setHasErrors] = useState<string[]>([]);

    const [file, setFile] = useState<any>(null);
    const theme = useTheme();

    const handleUpload = (event: any) => {
        const file = event.target.files[0];
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onloadend = () => {
            setFile(file);
        };
    };

    const sendFile = async () => {
        startLoading();

        new InputMassiveExpirationDateUseCase()
            .handle(file)
            .then((data) => {
                console.log(data);
                toast.success("Importação concluida com sucesso!");

                setFile(null);

                if (data.failed.length > 0) {
                    toast.warning("Importação não concluida!")
                }
                else {
                    toast.success("Importação concluida com sucesso!");
                }

                setSuccess({
                    totalCreated: data.success.length,
                    totalUpdated: 0,
                });
                setFile(null)
                data.failed && setHasErrors(data.failed);


            })
            .catch(() => {
                toast.error("Não foi possivel realizar o input!");
            })
            .finally(() => {
                finishLoading();
            });
    };

    function exportReport() {
        let formatedStartDate = null;
        let formatedEndDate = null;

        startLoading();


        new ExportMassiveExpirationDateUseCase()
            .handle()
            .then((data) => {
                if (data.length <= 0) {
                    return toast.warning("Sem dados para exportar.");
                }

                const docRows = data.map((item: any) => {
                    return [
                        item.SETORSITE,
                        item.CODGIPSITE,
                        item.EXPIREDDAYS,
                    ];
                });
                let indicatorSheetBuilder = new SheetBuilder();
                indicatorSheetBuilder
                    .setHeader([
                        "Setor/Site",
                        "CodGip/Site",
                        "Dias a expirar",
                    ])
                    .append(docRows)
                    .exportAs(`Relatório configuração de expiração monetização`);
                toast.success("Relatório exportado com sucesso!");
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    }

    function downloadExampleFile2() {
        try {
            const docRows = [
                [
                    "Setor",
                    "1889",
                    "10",
                    "1",
                ],
                [
                    "Site",
                    "CPE",
                    "10",
                    "0",
                ]
            ];
            let indicatorSheetBuilder = new SheetBuilder();
            indicatorSheetBuilder
                .setHeader([
                    "Setor/Site",
                    "CodGip/Site",
                    "Dias a expirar",
                    "Status",
                ])
                .append(docRows)
                .exportAs(`Exemplo_importação_monetização_data`);
            toast.success("Exemplo exportado com sucesso!");
        } catch (error) {
            toast.error("Falha na exportação do exemplo!");
        }
    }

    function getMonetizationExport() {
        if (!myUser) return;

        startLoading();

        new ListTypeConfigMonetizationUseCase()
            .handle()
            .then((data) => {
                setTypesConfig(data);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    }
    useEffect(() => {
        getMonetizationExport();
    }, []);

    function getSectors() {
        if (!myUser) return;

        startLoading();

        new ListSectorsAndSubsectrosUseCase()
            .handle({
                isSubsector: false,
                sector: sectorSearch,
            })
            .then((data) => {
                setSectors(data);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    }
    useEffect(() => {
        getSectors();
    }, [sectorSearch]);

    function getSites() {
        startLoading();

        /*         new SitePersonaUseCase()
                    .handle({
                        codCollaborator: myUser.id,
                    })
                    .then((data) => {
                        setClients(data);
                    })
                    .catch(() => {
                        toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                    })
                    .finally(() => {
                        finishLoading();
                    }); */


        new SitePersonaUseCase()
            .handle()
            .then((data) => {
                setSites(data);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    }
    useEffect(() => {
        getSites();
    }, []);

    function getListMonetizationConfigDay() {
        if (!myUser) return;
        if (!typeConfigSelected) return;

        startLoading();

        const referer = typeConfigSelected.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE == 1 ? sectorSelected.id : siteSelected.id;

        new ListConfigDayMonetizationUseCase()
            .handle({
                referer: referer,
                filtertype: typeConfigSelected.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE,
            })
            .then((data) => {
                if (data.length > 0) {
                    setDaysExpired(data[0].DAYS);
                } else {
                    setDaysExpired(0);
                }
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    }

    useEffect(() => {
        getListMonetizationConfigDay();
    }, [sectorSelected, siteSelected]);

    const handleSaveConfig = async () => {
        if (!myUser) return;
        if (!typeConfigSelected) return;
        if (!daysExpired) return;
        if (typeConfigSelected.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE == 1 && !sectorSelected) return;
        if (typeConfigSelected.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE == 2 && !siteSelected) return;

        startLoading();

        new MonetizationConfigDayUseCase()
            .handle({
                monetizatioConfigType: typeConfigSelected.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE,
                ids: typeConfigSelected.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE == 1 ? [sectorSelected.id] : [siteSelected.id],
                days: daysExpired,
            })
            .then((data) => {
                toast.success(data);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });

    };

    return (
        <>
            <Card
                width={"100%"}
                display={"flex"}
                flexDirection={"column"}
                justifyContent={"space-between"}
            >
                <PageHeader
                    title={`Configurar datas a Expirar`}
                    headerIcon={<FileOpen />}
                    secondaryButtonIcon={<Download />}
                    secondaryButtonTitle="Baixar exemplo"
                    secondayButtonAction={() => downloadExampleFile2()}
                />
                <Stack width={"100%"}>
                    {success && (
                        <Box
                            sx={{
                                width: "100%",
                                display: "flex",
                                justifyContent: "center",
                                alignItems: "center",
                                flexDirection: "column",

                            }}
                        >
                            <Box display={"flex"} alignItems={"center"} gap={1}>

                                {

                                    hasErrors.length > 0 ? (
                                        <>
                                            <CheckCircle color="error" />
                                            <Typography fontWeight={"600"}>
                                                Importação não concluida!
                                            </Typography>
                                        </>
                                    ) : (

                                        <>
                                            <CheckCircle color="success" />
                                            <Typography fontWeight={"600"}>
                                                Importação concluida com sucesso!
                                            </Typography>
                                        </>
                                    )}
                            </Box>

                            <Box display={"flex"} gap={1}>
                                <Typography fontSize={"14px"}>
                                    Total criado:{" "}
                                    <Typography
                                        fontWeight={"500"}
                                        color={"primary"}
                                        fontSize={"14px"}
                                        component={"span"}
                                    >
                                        {success.totalCreated}
                                    </Typography>
                                </Typography>
                                <Typography fontSize={"14px"}>
                                    Total atualizado:{" "}
                                    <Typography
                                        fontWeight={"500"}
                                        color={"primary"}
                                        fontSize={"14px"}
                                        component={"span"}
                                    >
                                        {success.totalUpdated}
                                    </Typography>
                                </Typography>
                            </Box>
                            <Button
                                sx={{ px: "40px", mt: 2 }}
                                variant="contained"
                                onClick={() => {
                                    setSuccess(null);
                                    setHasErrors([]);
                                }}
                            >
                                Importar outro arquivo
                            </Button>
                            {hasErrors.length > 0 && (
                                <Box
                                    display={"flex"}
                                    flexDirection={"column"}
                                    gap={"5px"}
                                    justifyContent={"center"}
                                    alignItems={"center"}
                                    mt={"20px"}
                                    sx={{ overflowY: "auto", width: "80%" }}
                                >
                                    <Typography>Motivo das falhas</Typography>
                                    {hasErrors.map((error, i) => (
                                        <Box
                                            key={i + 1}
                                            display={"flex"}
                                            p={"5px 20px"}
                                            gap={"20px"}
                                            bgcolor={grey[300]}
                                            width="100%"
                                        >
                                            {i + 1} - {error}
                                        </Box>
                                    ))}
                                </Box>
                            )}
                        </Box>
                    )}
                    {file && !success && (
                        <Box
                            sx={{
                                display: "flex",
                                justifyContent: "center",
                                alignItems: "center",
                                gap: 2,
                                px: "20px",
                                mt: "20px",
                            }}
                        >
                            <Box
                                sx={{
                                    backgroundColor: theme.palette.primary.main,
                                    minWidth: "50px",
                                    height: "50px",
                                    borderRadius: "20px",
                                    alignItems: "center",
                                    justifyContent: "center",
                                    display: "flex",
                                }}
                            >
                                <InsertDriveFile
                                    sx={{ fontSize: "30px", color: "#fff" }}
                                />
                            </Box>
                            <Box>
                                <Typography
                                    fontSize={"12px"}
                                    sx={{ color: grey[600] }}
                                >
                                    Arquivo selecionado
                                </Typography>
                                <Typography
                                    fontSize={"14px"}
                                    fontWeight={"500"}
                                >
                                    {file.name}
                                </Typography>
                                <Typography fontSize={"12px"}>
                                    Tamanho: {file.size}k
                                </Typography>
                            </Box>
                        </Box>
                    )}
                    {!success && (
                        <Box width={"100%"} p={"20px 20px"}>
                            <input
                                accept=".xls, .xlsx, .csv"
                                style={{ display: "none" }}
                                id="image-upload"
                                type="file"
                                onChange={(e) => handleUpload(e)}
                            />
                            <label htmlFor="image-upload">
                                <Button
                                    variant="outlined"
                                    color="primary"
                                    component="span"
                                    fullWidth
                                    disabled={isLoading}
                                >
                                    Selecione um arquivo
                                </Button>
                            </label>
                            <LoadingButton
                                onClick={sendFile}
                                fullWidth
                                loading={isLoading}
                                variant="contained"
                                sx={{ mt: 1 }}
                                disabled={!file}
                            >
                                Enviar arquivo
                            </LoadingButton>
                        </Box>
                    )}
                </Stack>


            </Card>

            <Card marginTop={2} display={"flex"} width={"100%"} flexDirection={"column"}>
                <PageHeader
                    title={`Visualizar Manual Config`}
                    headerIcon={<FileOpen />}
                />
                <Autocomplete
                    fullWidth
                    value={typeConfigSelected}
                    options={typesConfg}
                    getOptionLabel={(option) => option.TYPE}
                    onChange={(event, value) => {
                        setTypeConfigSelected(value);
                    }}
                    //onInputChange={(e, text) => setSectorSearch(text)}
                    filterOptions={(x) => x}
                    filterSelectedOptions
                    renderInput={(props) => (
                        <TextField {...props} label={"Tipo de Configuração"} />
                    )}
                    renderOption={(props, option) => {
                        return (
                            <li {...props} key={option.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE}>
                                {option.TYPE}
                            </li>
                        );
                    }}
                    isOptionEqualToValue={(option, value) =>
                        option.TYPE === value.TYPE
                    }
                    sx={{ m: 0 }}
                />
                {typeConfigSelected && typeConfigSelected.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE == 1 ? (
                    <Box padding={1}>
                        <Autocomplete
                            fullWidth
                            value={sectorSelected}
                            options={sectors}
                            getOptionLabel={(option) => option.name}
                            onChange={(event, value) => {
                                setSectorSelected(value);
                            }}
                            onInputChange={(e, text) => setSectorSearch(text)}
                            filterOptions={(x) => x}
                            filterSelectedOptions
                            renderInput={(props) => (
                                <TextField {...props} label={"Setores"} />
                            )}
                            renderOption={(props, option) => {
                                return (
                                    <li {...props} key={option.id}>
                                        {option.id} - {option.name}
                                    </li>
                                );
                            }}
                            isOptionEqualToValue={(option, value) =>
                                option.name === value.name
                            }
                            sx={{ m: 0 }}
                        />
                    </Box>
                )
                    : null}
                {typeConfigSelected && typeConfigSelected.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE == 2 ? (
                    <Box padding={1}>
                        <Autocomplete
                            fullWidth
                            value={siteSelected}
                            options={sites}
                            getOptionLabel={(option) => option.name}
                            onChange={(event, value) => {
                                setSiteSelected(value);
                            }}
                            filterOptions={(x) => x}
                            filterSelectedOptions
                            renderInput={(props) => (
                                <TextField {...props} label={"Site"} />
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
                            sx={{ m: 0 }}
                        />
                    </Box>
                )
                    : null}
                {typeConfigSelected && (
                    <Box padding={1}>
                        <TextField
                            name={"id"}
                            label={"Dias para Expirar"}
                            value={daysExpired}
                            focused={daysExpired ? true : false}
                            type="number"
                            onChange={(e) => setDaysExpired(Number(e.target.value))}
                            size={"small"}
                        />
                    </Box>
                )}
                <Box padding={1}>
                    <ActionButton
                        title="Salvar Configuração"
                        loading={isLoading}
                        isActive={false}
                        size="large"
                        onClick={() => handleSaveConfig()}
                    />
                </Box>
            </Card>

            <Card marginTop={2} display={"flex"} width={"100%"} flexDirection={"column"}>
                <PageHeader
                    title={"Extração configuração datas a expirar"}
                    headerIcon={<DownloadRounded />}
                />

                <Stack width={"100%"}>
                    <Box width={"100%"} p={"20px 20px"}>
                        <Box
                            display={"flex"}
                            flexDirection={"row"}
                            gap={"10px"}
                            mb={2}
                        >
                        </Box>
                        <LoadingButton
                            onClick={exportReport}
                            fullWidth
                            loading={isLoading}
                            variant="contained"
                            sx={{ mt: 1 }}
                        >
                            Exportar relatório
                        </LoadingButton>
                    </Box>
                </Stack>
            </Card>
        </>
    );
}

ConfigMonetizationExpiration.getLayout = getLayout("private");
