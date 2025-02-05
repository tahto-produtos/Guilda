import {
    Autocomplete,
    Box,
    Checkbox,
    FormControl,
    InputLabel,
    MenuItem,
    Select,
    Stack,
    TextField,
    Typography,
} from "@mui/material";
import { useEffect, useState } from "react";
import { toast } from "react-toastify";
import { BaseModal } from "src/components/feedback";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { useDebounce, useLoadingState } from "src/hooks";
import { ListGroupsUseCase } from "src/modules/groups";
import { ListHierarchiesUseCase } from "src/modules/hierarchies/use-cases/list-hierarchies.use-case";
import { Group } from "src/typings";
import { Hierarchie } from "src/typings/models/hierarchie.model";
import { PaginationModel } from "src/typings/models/pagination.model";
import { LoadingButton } from "@mui/lab";
import { Stock } from "src/typings/models/stock.model";
import { Product } from "src/typings/models/product.model";
import { ListStocks } from "../use-cases/list-stocks";
import { StockProduct } from "src/typings/models/stock-product.model";
import { CreateDisplayUseCase } from "../use-cases/create-display.use-case";
import { ListProductsByVisibility } from "../use-cases/list-product-by-visibility.use-case";

interface ModalExportResultsProps {
    open: boolean;
    onClose: () => void;
}

interface ProductFromStock {
    product: { comercialName: string; id: number; description: string };
}

interface Priority {
    name: string;
    id: string;
}

const priorities: Priority[] = [
    { name: "Prioridade", id: "1" },
    // { name: "Dinâmico", id: "2" },
    { name: "Mais vendido", id: "3" },
    { name: "Menos vendido", id: "4" },
    { name: "Antigo no estoque", id: "5" },
    { name: "Lançamento", id: "6" },
];

export function CreateDisplayModal(props: ModalExportResultsProps) {
    const { onClose, open } = props;

    const { finishLoading, isLoading, startLoading } = useLoadingState();

    const [name, setName] = useState<string>("");
    const [priority, setPriority] = useState<string>("1");

    const [stock, setStock] = useState<Stock | null>(null);
    const [stocksSearchValue, setStocksSearchValue] = useState<string>("");
    const [stocks, setStocks] = useState<Stock[]>([]);
    const debouncedStocksSearchTerm: string = useDebounce<string>(
        stocksSearchValue,
        400
    );

    interface ProductFromProductVisibility {
        ProductName: string;
        id: number;
    }

    const [product, setProduct] = useState<ProductFromProductVisibility[]>([]);
    const [productsSearchValue, setProductsSearchValue] = useState<string>("");
    const [products, setProducts] = useState<ProductFromProductVisibility[]>(
        []
    );
    const debouncedProductsSearchTerm: string = useDebounce<string>(
        productsSearchValue,
        400
    );

    const [group, setGroup] = useState<Group[]>([]);
    const [groupsSearchValue, setGroupsSearchValue] = useState<string>("");
    const [groups, setGroups] = useState<Group[]>([]);
    const debouncedGroupsSearchTerm: string = useDebounce<string>(
        groupsSearchValue,
        400
    );

    const [hierarchie, setHierarchie] = useState<Hierarchie[]>([]);
    const [hierarchiesSearchValue, setHierarchiesSearchValue] =
        useState<string>("");
    const [hierarchies, setHierarchies] = useState<Hierarchie[]>([]);
    const debouncedHierarchiesSearchTerm: string = useDebounce<string>(
        hierarchiesSearchValue,
        400
    );

    const getProducts = async () => {
        if (!stock) {
            return;
        }

        const payload = {
            Groups: group.map((item) => {
                return { id: item.id };
            }),
            hierarchies: hierarchie.map((item) => {
                return { id: item.id };
            }),
            productName: debouncedProductsSearchTerm,
            stock: [{ id: stock.id }],
        };

        new ListProductsByVisibility()
            .handle(payload)
            .then((data) => {
                setProducts(data);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    };

    useEffect(() => {
        getProducts();
    }, [group, hierarchie, debouncedProductsSearchTerm, stock]);

    // useEffect(() => {
    //     if (stock) {
    //         const foundStock = stocks.find(
    //             (item) => item.id == stock.id
    //         )?.GdaStockProduct;

    //         setProducts(foundStock as unknown as ProductFromStock[]);
    //     } else {
    //         setProducts([]);
    //         setProduct([]);
    //     }
    // }, [stock]);

    const getStocksList = async () => {
        const pagination = {
            limit: 200,
            offset: 0,
            searchText: stocksSearchValue,
        };

        new ListStocks()
            .handle(pagination)
            .then((data) => {
                setStocks(data.items.filter((item: any) => item.id !== null));
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    };

    useEffect(() => {
        getStocksList();
    }, [debouncedStocksSearchTerm]);

    const getHierarchiesList = async () => {
        const pagination: PaginationModel = {
            limit: 20,
            offset: 0,
            searchText: hierarchiesSearchValue,
        };

        new ListHierarchiesUseCase()
            .handle(pagination)
            .then((data) => {
                setHierarchies(data.items);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    };

    useEffect(() => {
        getHierarchiesList();
    }, [debouncedHierarchiesSearchTerm]);

    const getGroupsList = async () => {
        const pagination: PaginationModel = {
            limit: 20,
            offset: 0,
            searchText: groupsSearchValue,
        };

        new ListGroupsUseCase()
            .handle(pagination)
            .then((data) => {
                setGroups(data.items);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    };

    useEffect(() => {
        getGroupsList();
    }, [debouncedGroupsSearchTerm]);

    async function handleCreate() {
        if (!stock) {
            return toast.warning("Escolha um estoque para criar uma vtirine.");
        }

        if (product.length <= 0 && priority == "1") {
            return toast.warning("Adicione produtos para criar uma vtirine.");
        }

        startLoading();

        const payload = {
            PRIORIDADE: priority,
            NOMECONFIG: name,
            ITENS: product.map((item, index) => {
                return {
                    CODPRODUTO: item.id,
                    POSICAO: index.toString(),
                };
            }),
            HIERARQUIA: hierarchie.map((item) => {
                return {
                    CODHIERARQUIA: item.id,
                };
            }),
            GRUPO: group.map((item) => {
                return {
                    CODGRUPO: item.id,
                };
            }),
            ESTOQUE: [{ CODESTOQUE: stock.id }],
        };

        new CreateDisplayUseCase()
            .handle(payload)
            .then((data) => {
                toast.success("Vitrine criada com sucesso!");
                onClose();
            })
            .catch((data) => {
                const msg = data?.response?.data?.Message;
                if (msg) {
                    return toast.error(msg);
                }
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    }

    return (
        <BaseModal
            width={"540px"}
            open={open}
            title={`Nova vitrine`}
            onClose={onClose}
        >
            <Box width={"100%"} display={"flex"} flexDirection={"column"}>
                <TextField
                    label="Nome da configuração"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    sx={{ mb: 2 }}
                />
                <Autocomplete
                    size={"small"}
                    options={stocks}
                    value={stock}
                    disableClearable={false}
                    getOptionLabel={(option) => option.description}
                    onChange={(event, value) => {
                        setStock(value);
                        setProduct([]);
                    }}
                    onInputChange={(e, text) => setStocksSearchValue(text)}
                    filterOptions={(x) => x}
                    filterSelectedOptions
                    renderInput={(params) => (
                        <TextField
                            {...params}
                            variant="outlined"
                            label="Selecione o estoque"
                            placeholder="Buscar"
                        />
                    )}
                    renderOption={(props, option) => {
                        return (
                            <li {...props} key={option.id}>
                                {option.description}
                            </li>
                        );
                    }}
                    isOptionEqualToValue={(option, value) =>
                        option.description === value.description
                    }
                />
                <Autocomplete
                    multiple
                    size={"small"}
                    options={groups}
                    getOptionLabel={(option) => option.name}
                    onChange={(event, value) => {
                        setProduct([]);
                        setGroup(value);
                    }}
                    onInputChange={(e, text) => setGroupsSearchValue(text)}
                    filterOptions={(x) => x}
                    filterSelectedOptions
                    renderInput={(params) => (
                        <TextField
                            {...params}
                            variant="outlined"
                            label="Quais grupos podem visualizar"
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
                <Autocomplete
                    multiple
                    size={"small"}
                    options={hierarchies}
                    getOptionLabel={(option) => option.levelName}
                    onChange={(event, value) => {
                        setProduct([]);
                        setHierarchie(value);
                    }}
                    onInputChange={(e, text) => setHierarchiesSearchValue(text)}
                    filterOptions={(x) => x}
                    filterSelectedOptions
                    renderInput={(params) => (
                        <TextField
                            {...params}
                            variant="outlined"
                            label="Quais cargos podem visualizar"
                            placeholder="Buscar"
                        />
                    )}
                    renderOption={(props, option) => {
                        return (
                            <li {...props} key={option.id}>
                                {option.levelName}
                            </li>
                        );
                    }}
                    isOptionEqualToValue={(option, value) =>
                        option.levelName === value.levelName
                    }
                />
                {priority == "1" && (
                    <Autocomplete
                        multiple
                        size={"small"}
                        options={products}
                        value={product}
                        getOptionLabel={(option) => option.ProductName}
                        onChange={(event, value) => {
                            setProduct(value);
                        }}
                        onInputChange={(e, text) =>
                            setProductsSearchValue(text)
                        }
                        filterSelectedOptions
                        renderInput={(params) => (
                            <TextField
                                {...params}
                                variant="outlined"
                                label="Produtos selecionados"
                                placeholder="Buscar"
                            />
                        )}
                        renderOption={(props, option) => {
                            return (
                                <li {...props} key={option.id}>
                                    {option.ProductName}
                                </li>
                            );
                        }}
                        isOptionEqualToValue={(option, value) =>
                            option.id === value.id
                        }
                    />
                )}
                <FormControl fullWidth>
                    <InputLabel
                        size="small"
                        sx={{ background: "#fff", px: "5px" }}
                    >
                        Prioridade
                    </InputLabel>
                    <Select
                        size="small"
                        value={priority}
                        onChange={(e) =>
                            setPriority(e.target.value?.toString())
                        }
                    >
                        {priorities.map((p, i) => (
                            <MenuItem value={p.id} key={i}>
                                {p.name}
                            </MenuItem>
                        ))}
                    </Select>
                </FormControl>
                {/* <Box flexDirection={"column"} display={"flex"} gap={1}>
                    {product.map((item, index) => (
                        <SelectedProductItem key={index} item={item} />
                    ))}
                </Box> */}
                <LoadingButton
                    variant="contained"
                    onClick={handleCreate}
                    sx={{ mt: 2 }}
                    loading={isLoading}
                >
                    {isLoading ? "Aguarde..." : "Criar vitrine"}
                </LoadingButton>
            </Box>
        </BaseModal>
    );
}
