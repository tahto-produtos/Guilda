import { useContext, useState } from "react";
import {
    Box,
    Button,
    CircularProgress,
    Stack,
    TextField,
    Typography,
    useTheme,
} from "@mui/material";
import PermDataSetting from "@mui/icons-material/PermDataSetting";
import { SheetBuilder, getLayout } from "src/utils";
import { Card, PageHeader } from "src/components";
import { useLoadingState } from "src/hooks";
import CheckCircle from "@mui/icons-material/CheckCircle";
import InsertDriveFile from "@mui/icons-material/InsertDriveFile";
import { grey } from "@mui/material/colors";
import { MassiveAssociateUseCase } from "src/modules/sectors/use-cases/massive-associate.use-case";
import { toast } from "react-toastify";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { LoadingButton } from "@mui/lab";
import { MassiveCreateProductUseCase } from "src/modules/marketing-place/use-cases/massive-create-product.use-case";
import Download from "@mui/icons-material/Download";
import abilityFor from "src/utils/ability-for";
import WithoutPermissionCard from "src/components/data-display/without-permission/without-permissions";
import { BaseModal } from "src/components/feedback";
import Check from "@mui/icons-material/Check";
import { AuthVerify } from "src/modules/auth/use-cases/auth-verify/auth-verify.use-case";
import { isAxiosError } from "axios";
import { EXCEPTION_CODES } from "src/typings";
import { AuthVerifyComponent } from "src/components/auth/AuthVerifyComponent/AuthVerifyComponent";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";

export default function MassiveCreateProduct() {
    const { isLoading, startLoading, finishLoading } = useLoadingState();
    const { myPermissions } = useContext(PermissionsContext);
    const [file, setFile] = useState<any>(null);
    const { myUser } = useContext(UserInfoContext);
    const [success, setSuccess] = useState<{
        totalCreated: number;
        totalUpdated?: number;
        totalFailed?: number;
    } | null>(null);
    const theme = useTheme();
    const [hasErrors, setHasErrors] = useState<{ failedReason: string }[]>([]);
    const [authVerifyModal, setAuthVerifyModal] = useState<boolean>(false);

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

        new MassiveCreateProductUseCase()
            .handle(file)
            .then((data) => {
                toast.success("Importação concluida com sucesso!");
                setSuccess({
                    totalCreated: data.success.length,
                    totalFailed: data.failed.length,
                });
                setFile(null);
                data.failed && setHasErrors(data.failed);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    };

    function downloadExampleFile() {
        try {
            const docRows = [
                [
                    "FISICO",
                    "",
                    "",
                    "MAGAZINE LUIZA",
                    "",
                    "",
                    "",
                    "",
                    "o dado é um texto ex: Bicicleta",
                    "o dado é um texto ex: Azul com 16 marchas",
                    "10",
                    "2",
                    "Estoque CPE",
                    "CPE",
                    "G1;G2;G3;G4",
                    "Agente;Coordenador;Supervisor;Gerente I",
                    "SKY",
                    "1889",
                    "18/08/2023",
                    "30/10/2023",
                    "2",
                    "Liberado para venda",
                    "",
                    "300",
                ],
                [
                    "FISICO",
                    "",
                    "1.22.73.190-210",
                    "Produto Livro",
                    "",
                    "",
                    "",
                    "",
                    "o dado é um texto ex: Bicicleta",
                    "o dado é um texto ex: Azul com 16 marchas",
                    "10",
                    "2",
                    "Estoque CPE",
                    "CPE",
                    "G1;G2;G3;G4",
                    "Agente;Coordenador;Supervisor;Gerente I",
                    "SKY",
                    "1889",
                    "18/08/2023",
                    "30/10/2023",
                    "2",
                    "Inativar",
                    "",
                    "300",
                ],
                [
                    "VIRTUAL",
                    "codigodovoucher1; codigodovoucher2; codigodovoucher3",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "o dado é um texto ex: voucher loja X",
                    "o dado é um texto ex: valido em toda a rede",
                    "3",
                    "1",
                    "Estoque Virtual GNA",
                    "GNA",
                    "G1;G2",
                    "Diretor;Gerente II",
                    "OI",
                    "1273",
                    "18/08/2023",
                    "30/10/2023",
                    "1",
                    "Liberado para venda",
                    "31/12/2023",
                    "300",
                ],
            ];
            let indicatorSheetBuilder = new SheetBuilder();
            indicatorSheetBuilder
                .setHeader([
                    "TIPO PRODUTO",
                    "VOUCHERS (separados por ; e sem espaço)",
                    "CODIGO DO PRODUTO",
                    "FORNECEDOR",
                    "GRUPO DE PRODUTOS",
                    "TAMANHO",
                    "COR",
                    "DETALHE",
                    "NOME PRODUTO",
                    "DESCRIÇÃO",
                    "QTDE DISTRIBUIDA",
                    "VALOR",
                    "ESTOQUE",
                    "SITE",
                    "GRUPO (separados por ; e sem espaço)",
                    "HIERARQUIA (separados por ; e sem espaço)",
                    "CLIENTE (separados por ; e sem espaço)",
                    "SETOR (separados por ; e sem espaço)",
                    "DATA DA EXIBIÇÃO",
                    "FIM DA EXIBIÇÃO",
                    "LIMITE DE COMPRA POR COLABORADOR",
                    "STATUS PRODUTO",
                    "DATA DE VALIDADE PRODUTO",
                    "Imagem ID",
                ])
                .append(docRows)
                .exportAs(`Exemplo_importação_massiva_produtos`);
            toast.success("Exemplo exportado com sucesso!");
        } catch (error) {
            toast.error("Falha na exportação do exemplo!");
        }
    }

    if (
        abilityFor(myPermissions).cannot("Cadastrar Produto", "Marketing Place")
    ) {
        return (
            <Card display={"flex"} width={"100%"} flexDirection={"column"}>
                <WithoutPermissionCard />
            </Card>
        );
    }

    return (
        <Card display={"flex"} width={"100%"} flexDirection={"column"}>
            <PageHeader
                title={"Importação massiva de produtos"}
                headerIcon={<PermDataSetting />}
                secondaryButtonIcon={<Download />}
                secondaryButtonTitle="Baixar exemplo"
                secondayButtonAction={() => downloadExampleFile()}
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
                            <CheckCircle color="success" />
                            <Typography fontWeight={"600"}>
                                Importação concluida com sucesso!
                            </Typography>
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
                                Total falhas:{" "}
                                <Typography
                                    fontWeight={"500"}
                                    color={"primary"}
                                    fontSize={"14px"}
                                    component={"span"}
                                >
                                    {success.totalFailed}
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
                            >
                                <Typography>Motivo das falhas</Typography>
                                {hasErrors.map((error, i) => (
                                    <Box
                                        key={i}
                                        display={"flex"}
                                        p={"5px 20px"}
                                        gap={"20px"}
                                        bgcolor={grey[300]}
                                    >
                                        {i} - {error.failedReason}
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
                            <Typography fontSize={"14px"} fontWeight={"500"}>
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
                            onClick={() => {
                                sendFile();
                            }}
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
            <AuthVerifyComponent
                isOpen={authVerifyModal}
                onClose={() => setAuthVerifyModal(false)}
                onVerified={sendFile}
            />
        </Card>
    );
}

MassiveCreateProduct.getLayout = getLayout("private");
