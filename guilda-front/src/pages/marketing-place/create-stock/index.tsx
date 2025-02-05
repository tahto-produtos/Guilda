import Add from "@mui/icons-material/Add";
import {
    Autocomplete,
    Box,
    Button,
    FormControl,
    InputLabel,
    MenuItem,
    Select,
    Stack,
    TextField,
} from "@mui/material";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";
import { useLoadingState } from "src/hooks";
import { CreateStockUseCase } from "src/modules/marketing-place/use-cases/create-stock.use-case";
import { ListCityUseCase } from "src/modules/marketing-place/use-cases/list-city.use-case";
import { ListClientUseCase } from "src/modules/marketing-place/use-cases/list-client.use-case";
import { getLayout } from "src/utils";

export default function CreateStock() {
    const { myPermissions } = useContext(PermissionsContext);
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [name, setName] = useState<string>("");
    const [city, setCity] = useState<string>("");
    const [campaign, setCampaign] = useState<string>("");
    const [client, setClient] = useState<string>("");
    const [cityList, setCityList] = useState<any[]>([]);
    const [clientList, setClientList] = useState<any[]>([]);
    const [typeList, setTypeList] = useState<
        Array<{ id: number; type: string; createdAt: string }>
    >([]);
    const [stockType, setStockType] = useState<"PHYSICAL" | "DIGITAL">(
        "PHYSICAL"
    );

    const handleCreateStock = async () => {
        // if (!name || !city || !client) {
        //     return toast.warning("Preencha todos os campos");
        // }

        startLoading();

        const payload = {
            description: name,
            city,
            campaign,
            client,
            type: stockType,
        };

        new CreateStockUseCase()
            .handle(payload)
            .then((data) => {
                toast.success(`Estoque '${name}' criado com sucesso!`);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    };

    async function getCitiesAndClients() {
        startLoading();

        const set = new Set();

        await new ListCityUseCase()
            .handle()
            .then((data) => {
                data.cities.forEach((el: any, index: number) => {
                    if (el.VALUE !== "-") {
                        set.add(el.VALUE);
                    }
                    console.log(set, el.value, el.VALUE, el);
                });
                setCityList(Array.from(set));
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });

        await new ListClientUseCase()
            .handle()
            .then((data) => {
                setClientList(data.clients);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    }

    useEffect(() => {
        getCitiesAndClients();
    }, []);

    return (
        <Card width={"100%"} display={"flex"} flexDirection={"column"}>
            <PageHeader title={"Novo estoque"} headerIcon={<Add />} />
            <Stack px={2} py={4} width={"100%"} gap={2}>
                <TextField
                    label="Nome do estoque"
                    size="small"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                />
                <FormControl sx={{ width: "100%" }}>
                    <InputLabel sx={{ backgroundColor: "#fff", px: 1 }}>
                        Cidade
                    </InputLabel>
                    <Select
                        onChange={(e) => setCity(e.target.value)}
                        value={city}
                    >
                        {cityList.map((item, index) => {
                            return (
                                <MenuItem value={item} key={index}>
                                    {item}
                                </MenuItem>
                            );
                        })}
                    </Select>
                </FormControl>
                <FormControl sx={{ width: "100%" }}>
                    <InputLabel sx={{ backgroundColor: "#fff", px: 1 }}>
                        Cliente
                    </InputLabel>
                    <Select
                        onChange={(e) => setClient(e.target.value)}
                        value={client}
                    >
                        {clientList.map((item, index) => {
                            return (
                                <MenuItem value={item} key={index}>
                                    {item}
                                </MenuItem>
                            );
                        })}
                    </Select>
                </FormControl>
                <TextField
                    label="Campanha"
                    size="small"
                    value={campaign}
                    onChange={(e) => setCampaign(e.target.value)}
                />
                <Select
                    value={stockType}
                    onChange={(e) =>
                        setStockType(e.target.value as "PHYSICAL" | "DIGITAL")
                    }
                >
                    <MenuItem value={"PHYSICAL"}>Produto físico</MenuItem>
                    <MenuItem value={"DIGITAL"}>Produto digital</MenuItem>
                </Select>

                <Box display={"flex"} justifyContent={"flex-end"}>
                    <Button variant="contained" onClick={handleCreateStock}>
                        Criar estoque
                    </Button>
                </Box>
            </Stack>
        </Card>
    );
}

CreateStock.getLayout = getLayout("private");
