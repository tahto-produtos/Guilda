import SupervisedUserCircleOutlined from "@mui/icons-material/SupervisedUserCircleOutlined";
import DeleteOutline from "@mui/icons-material/DeleteOutline";
import {
    Autocomplete,
    Box,
    Breadcrumbs,
    Button,
    Divider,
    IconButton,
    InputAdornment,
    Link,
    Stack,
    TextField,
    Typography,
    useTheme,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { format, subMonths } from "date-fns";
import { useRouter } from "next/router";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { PageTitle } from "src/components/data-display/page-title/page-title";
import { EmptyState } from "src/components/feedback/empty-state/empty-state";
import { SearchIcon } from "src/components/icons/search.icon";
import { ContentArea } from "src/components/surfaces/content-area/content-area";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { useDebounce, useLoadingState } from "src/hooks";
import { getLayout } from "src/utils";
import { IndicatorsClimateSearch } from "src/modules/climate/fragments/indicators-climate-search";
import {
    ListGroupReason,
    ListGroupReasonResponse,
} from "src/modules/climate/use-case/list-group-hierarchy-climate";
import ConnectWithoutContactOutlined from "@mui/icons-material/ConnectWithoutContactOutlined";
import { capitalizeText } from "src/utils/capitalizeText";
import { HomeOutlined } from "@mui/icons-material";

export default function ClimateOverview() {
    const { myUser } = useContext(UserInfoContext);
    const [startDate, setStartDate] = useState<dateFns | Date | null>(
        subMonths(new Date(), 1)
    );
    const [endDate, setEndDate] = useState<dateFns | Date | null>(new Date());
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [searchText, setSearchText] = useState<string>("");
    const debouncedSearchText: string = useDebounce<string>(searchText, 400);
    const router = useRouter();
    const theme = useTheme();
    const [results, setResults] = useState<ListGroupReasonResponse[]>([]);

    async function getClimateOverview() {
        console.log(startDate, endDate);
        if (!startDate || !endDate) return;

        startLoading();
        new ListGroupReason()
            .handle({
                STARTEDATFROM: format(
                    new Date(startDate.toString()),
                    "yyyy/MM/dd"
                ),
                STARTEDATTO: format(new Date(endDate.toString()), "yyyy/MM/dd"),
            })
            .then((data) => {
                setResults(data);
            })
            .catch(() => {
                toast.error("Erro ao listar as palavras proibidas.");
            })
            .finally(() => {
                finishLoading();
            });
    }

    useEffect(() => {
        getClimateOverview();
    }, [startDate, endDate]);

    return (
        <ContentCard sx={{ p: 0 }}>
            <Stack
                width={"100%"}
                height={"80px"}
                sx={{
                    borderTopLeftRadius: "16px",
                    borderTopRightRadius: "16px",
                }}
                bgcolor={theme.palette.secondary.main}
                pl={"80px"}
                justifyContent={"center"}
            >
                <Breadcrumbs
                    aria-label="breadcrumb"
                    sx={{
                        color: theme.palette.background.default,
                    }}
                >
                    <Link
                        underline="hover"
                        sx={{ display: "flex", alignItems: "center" }}
                        color={theme.palette.background.default}
                        href="/"
                    >
                        <HomeOutlined
                            sx={{
                                mr: 0.5,
                                color: theme.palette.background.default,
                            }}
                        />
                    </Link>
                    <Link
                        sx={{
                            display: "flex",
                            alignItems: "center",

                            textDecoration: "none",
                        }}
                        color={theme.palette.background.default}
                    >
                        <Typography fontWeight={"700"}>Clima</Typography>
                    </Link>
                </Breadcrumbs>
            </Stack>
            <ContentArea sx={{ py: "40px" }}>
                <PageTitle
                    icon={
                        <ConnectWithoutContactOutlined
                            sx={{ fontSize: "30px" }}
                        />
                    }
                    title="Clima"
                    loading={isLoading}
                >
                    <Stack
                        direction={"row"}
                        alignItems={"center"}
                        gap={"16px"}
                        width={"100%"}
                        justifyContent={"flex-end"}
                    >
                        {/* <TextField
              label="Busque um usuário"
              size="small"
              sx={{
                maxWidth: "250px",
                width: "100%",
              }}
              value={searchText}
              onChange={(e) => setSearchText(e.target.value)}
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <SearchIcon
                      width={12}
                      height={12}
                      color={theme.palette.text.primary}
                    />
                  </InputAdornment>
                ),
              }}
            /> */}
                        {/* <Button
              variant="contained"
              // onClick={() => router.push("/personas/create-blacklist")}
            >
              Histórico de respostas
            </Button>
            <Button
              variant="contained"
              // onClick={() => router.push("/personas/create-blacklist")}
            >
              Filtrar
            </Button> */}
                    </Stack>
                </PageTitle>
                <Divider />
                <Stack direction={"column"} gap={"20px"} mt={"30px"}>
                    <Box display={"flex"} gap={2} width={"100%"}>
                        <LocalizationProvider dateAdapter={AdapterDateFns}>
                            <DatePicker
                                label="De"
                                value={startDate}
                                onChange={(newValue) => setStartDate(newValue)}
                                slotProps={{
                                    textField: {
                                        size: "small",
                                        sx: {
                                            minWidth: "180px",
                                            svg: {
                                                color: grey[500],
                                            },
                                            width: "100%",
                                        },
                                    },
                                }}
                            />
                        </LocalizationProvider>
                        <LocalizationProvider dateAdapter={AdapterDateFns}>
                            <DatePicker
                                label="Até"
                                value={endDate}
                                onChange={(newValue) => setEndDate(newValue)}
                                slotProps={{
                                    textField: {
                                        size: "small",
                                        sx: {
                                            minWidth: "180px",
                                            svg: {
                                                color: grey[500],
                                            },
                                            width: "100%",
                                        },
                                    },
                                }}
                            />
                        </LocalizationProvider>
                    </Box>
                    <Stack
                        direction={"row"}
                        alignItems={"center"}
                        gap={"30px"}
                        justifyContent={"space-between"}
                        border={`solid 1px #CED4DA8F`}
                        borderRadius={"16px"}
                        p={"16px"}
                    >
                        <Typography
                            variant="body1"
                            fontSize={"20px"}
                            fontWeight={"400"}
                            alignItems={"center"}
                            display={"flex"}
                            flexDirection={"row"}
                            gap={"24px"}
                        >
                            <SupervisedUserCircleOutlined
                                sx={{ fontSize: "32px" }}
                            />
                            Dados da hierarquia de{" "}
                            {capitalizeText(myUser?.name || "")}
                        </Typography>
                        <Typography
                            variant="body2"
                            fontSize={"16px"}
                            fontFamily={"Montserrat"}
                            fontWeight={"500"}
                            color={"text.secondary"}
                            alignItems={"center"}
                            display={"flex"}
                            flexDirection={"row"}
                            gap={"16px"}
                        >
                            Exibindo{" "}
                            {results.reduce(
                                (accumulator, currentValue) =>
                                    accumulator + currentValue.count,
                                0
                            )}{" "}
                            resultados
                        </Typography>
                    </Stack>
                    <IndicatorsClimateSearch results={results} />
                </Stack>
            </ContentArea>
        </ContentCard>
    );
}

ClimateOverview.getLayout = getLayout("private");
