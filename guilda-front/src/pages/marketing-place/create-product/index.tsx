import CheckCircle from "@mui/icons-material/CheckCircle";
import Add from "@mui/icons-material/Add";
import {
    Autocomplete,
    Box,
    Button,
    CardMedia,
    Checkbox,
    Chip,
    CircularProgress,
    FormControl,
    InputLabel,
    MenuItem,
    Pagination,
    Select,
    Stack,
    TextField,
    Typography,
} from "@mui/material";
import { isAxiosError } from "axios";
import { useRouter } from "next/router";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { BaseModal } from "src/components/feedback";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { useDebounce, useLoadingState } from "src/hooks";
import { AuthVerify } from "src/modules/auth/use-cases/auth-verify/auth-verify.use-case";
import Showcase from "src/modules/marketing-place/fragments/showcase";
import { CreateProductUseCase } from "src/modules/marketing-place/use-cases/create-product.use-case";
import { ListProducts } from "src/modules/marketing-place/use-cases/list-products";
import { EXCEPTION_CODES, Group, Sector } from "src/typings";
import { Product } from "src/typings/models/product.model";
import { getLayout } from "src/utils";
import abilityFor from "src/utils/ability-for";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { grey } from "@mui/material/colors";
import { format, isAfter, set } from "date-fns";
import Visibility from "@mui/icons-material/Visibility";
import { ListStocks } from "src/modules/marketing-place/use-cases/list-stocks";
import { Stock } from "src/typings/models/stock.model";
import { translateVisibiltyName } from "src/utils/translates";
import { ListGroupsUseCase, ListSectorsUseCase } from "src/modules";
import { Hierarchie } from "src/typings/models/hierarchie.model";
import { PaginationModel } from "src/typings/models/pagination.model";
import { ListHierarchiesUseCase } from "src/modules/hierarchies/use-cases/list-hierarchies.use-case";
import { ListCollaboratorsUseCase } from "src/modules/profiles/use-cases/list-collaboratots.use-case";
import { ListCollaboratorsAllUseCase } from "src/modules/collaborators/use-cases/list-collaborators.use-case";
import { ListCategoryUseCase } from "src/modules/marketing-place/use-cases/list-category.use-case";
import { Category } from "../category";
import { Supplier } from "../product-quantity";
import { ListSupplierUseCase } from "src/modules/marketing-place/use-cases/list-suppliers.use-case";
import { ProductSizes } from "src/modules/marketing-place/fragments/product-sizes";
import { ProductDetails } from "src/modules/marketing-place/fragments/product-details";
import {
    GalleryImage,
    ImageGallery,
} from "src/components/data-display/image-gallery/image-gallery";
import { ProductColors } from "src/modules/marketing-place/fragments/product-colors";
import { ProductGroup } from "src/modules/marketing-place/fragments/product-group";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";

export default function CreateProduct() {
    const { myPermissions } = useContext(PermissionsContext);
    const { myUser } = useContext(UserInfoContext);
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [image, setImage] = useState<File | null>(null);
    const [selectedImage, setSelectedImage] = useState<GalleryImage | null>(
        null
    );
    const [visibilityType, setVisibilityType] = useState<
        | "COLLABORATOR"
        | "HIERARCHY"
        | "GROUP"
        | "ATTRIBUTE"
        | "CLIENT"
        | "SECTOR"
    >("COLLABORATOR");
    const [visibilityValue, setVisibilityValue] = useState<string>("");
    const [name, setName] = useState<string>("");
    const [code, setCode] = useState<string>("");
    const [price, setPrice] = useState<number>(0);
    const [saleLimit, setSaleLimit] = useState<number>(0);
    const [productType, setProductType] = useState<"PHYSICAL" | "DIGITAL">(
        "PHYSICAL"
    );
    const [quantity, setQuantity] = useState<number>(0);
    const [vouchers, setVouchers] = useState<string[]>([]);
    const [isVisibilityConfigVisible, setIsVisibilityConfigVisible] =
        useState<boolean>(false);
    const [voucherTextValue, setVoucherTextValue] = useState<string>("");
    const [modalAuthVoucherIsOpen, setModalAuthVoucherIsOpen] =
        useState<boolean>(false);
    const [authVoucherUser, setAuthVoucherUser] = useState<string>("");
    const [authVoucherPassword, setAuthVoucherPassword] = useState<string>("");
    const [isAuthVerified, setIsAuthVerified] = useState<boolean>(false);
    const [status, setStatus] = useState<"Ativo" | "Inativo">("Ativo");
    const [comercialName, setComercialName] = useState<string>("");
    const [publicationDate, setPublicationDate] = useState<
        dateFns | Date | null
    >(null);
    const [expirationDate, setExpirationDate] = useState<dateFns | Date | null>(
        null
    );
    const [validityDate, setValidityDate] = useState<dateFns | Date | null>(
        null
    );
    const [highlight, setHighlight] = useState<boolean>(false);
    const [visibilitys, setVisibilitys] = useState<
        Array<{ type: string; value: string }>
    >([]);
    const [stocks, setStocks] = useState<Stock[]>([]);
    const [selectedStock, setSelectedStock] = useState<number | null>(null);

    const [sector, setSector] = useState<Sector | null>(null);
    const [sectors, setSectors] = useState<Sector[]>([]);
    const [sectorsSearchValue, setSectorsSearchValue] = useState<string>("");
    const debouncedSectorSearchTerm: string = useDebounce<string>(
        sectorsSearchValue,
        400
    );

    const [group, setGroup] = useState<Group | null>(null);
    const [groupsSearchValue, setGroupsSearchValue] = useState<string>("");
    const [groups, setGroups] = useState<Group[]>([]);
    const debouncedGroupsSearchTerm: string = useDebounce<string>(
        groupsSearchValue,
        400
    );

    const [hierarchie, setHierarchie] = useState<Hierarchie | null>(null);
    const [hierarchiesSearchValue, setHierarchiesSearchValue] =
        useState<string>("");
    const [hierarchies, setHierarchies] = useState<Hierarchie[]>([]);
    const debouncedHierarchiesSearchTerm: string = useDebounce<string>(
        hierarchiesSearchValue,
        400
    );

    const [suppliers, setSuppliers] = useState<Supplier[]>([]);
    const [selectedSupplier, setSelectedSupplier] = useState<Supplier | null>(
        null
    );

    const [collaborator, setCollaborator] = useState<{
        id: number;
        name: string;
        registry: string;
    } | null>(null);
    const [collaboratorsSearchValue, setCollaboratorsSearchValue] =
        useState<string>("");
    const [collaborators, setCollaborators] = useState<
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

    const [size, setSize] = useState<number | undefined>(undefined);
    const [type, setType] = useState<number | undefined>(undefined);
    const [detail, setDetail] = useState<number | undefined>(undefined);
    const [productColors, setProductColors] = useState<number | undefined>(
        undefined
    );
    const [productGroup, setProductGroup] = useState<number | undefined>(
        undefined
    );
    const [modalGalleryImagesOpen, setModalGalleryImagesOpen] =
        useState<boolean>(false);

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
            .catch(() => {});
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
            .catch(() => {});
    };

    useEffect(() => {
        getGroupsList();
    }, [debouncedGroupsSearchTerm]);

    const getSectorsList = async () => {
        const pagination = {
            limit: 20,
            offset: 0,
            searchText: sectorsSearchValue,
        };

        new ListSectorsUseCase()
            .handle(pagination)
            .then((data) => {
                setSectors(data.items);
            })
            .catch(() => {});
    };

    useEffect(() => {
        getSectorsList();
    }, [debouncedSectorSearchTerm]);

    const getStocks = async () => {
        startLoading();

        const payload = {
            limit: 99999,
            offset: 0,
            searchText: "",
        };
        const set = new Set();

        new ListStocks()
            .handle(payload)
            .then((data) => {
                setStocks(data.items);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    };

    useEffect(() => {
        getStocks();
    }, []);

    useEffect(() => {
        setAuthVoucherUser("");
        setAuthVoucherPassword("");
        setIsAuthVerified(false);
    }, [modalAuthVoucherIsOpen]);

    const createNewProduct = async () => {
        if (!publicationDate || !selectedStock) {
            return;
        }

        if (expirationDate) {
            if (
                isAfter(
                    new Date(publicationDate.toString()),
                    new Date(expirationDate.toString())
                )
            ) {
                return toast.warning(
                    "Data de expiração deve ser maior que a data de publicação"
                );
            }
        }

        startLoading();

        const formatedPublicationDate = format(
            new Date(publicationDate.toString()),
            "yyyy-MM-dd"
        );

        const formatedExpirationDate =
            expirationDate &&
            format(new Date(expirationDate?.toString()), "yyyy-MM-dd");

        const formatedValidityDate =
            validityDate &&
            format(new Date(validityDate?.toString()), "yyyy-MM-dd");

        const payload = {
            // image: image,
            uploadId: selectedImage && [selectedImage.id.toString()],
            description: name,
            // code: code,
            price: price,
            type: productType,
            vouchers: vouchers,
            status: status,
            comercialName: comercialName,
            publicationDate: formatedPublicationDate,
            expirationDate: formatedExpirationDate,
            validityDate: formatedValidityDate,
            // highlight: highlight ? 1 : 0,
            visibilitys: visibilitys,
            quantity: productType === "DIGITAL" ? vouchers.length : quantity,
            stockId: selectedStock,
            saleLimit: saleLimit,
            categoryId: selectedCategorie?.id || null,
            supplierId: selectedSupplier?.id || null,

            detailId: detail,
            productDetailId: detail,
            sizeId: size,
            typeId: type,
            colorId: productColors,
            // groupId: productGroup,
        };

        new CreateProductUseCase()
            .handle(payload)
            .then((data) => {
                return toast.success(
                    `Produto ${comercialName} criado com sucesso!`
                );
            })
            .catch((e) => {
                const errorCode = e?.response?.data?.code;

                if (errorCode === EXCEPTION_CODES.RESOURCE_ALREADY_EXISTS) {
                    toast.error("Produto já existente");
                } else if (
                    e?.response?.data?.message === "collaborator not found"
                ) {
                    toast.error(
                        `Colaborador: ${e?.response?.data?.keys?.id} não encontrado`
                    );
                } else if (
                    e?.response?.data?.message === "hierarchy not found"
                ) {
                    toast.error(
                        `Hierarquia: ${e?.response?.data?.keys?.id} não encontrada`
                    );
                } else if (e?.response?.data?.message === "group not found") {
                    toast.error(
                        `Grupo: ${e?.response?.data?.keys?.id} não encontrado`
                    );
                } else if (e?.response?.data?.message === "sector not found") {
                    toast.error(
                        `Setor: ${e?.response?.data?.keys?.id} não encontrado`
                    );
                } else if (
                    e?.response?.data?.keys?.productType &&
                    e?.response?.data?.keys?.stockType
                ) {
                    toast.error(
                        `Não é possível associar um produto 
                        ${
                            e?.response?.data?.keys?.productType === "PHYSICAL"
                                ? "físico"
                                : "vitual"
                        }
                        a um estoque ${
                            e?.response?.data?.keys?.stockType === "PHYSICAL"
                                ? "físico"
                                : "vitual"
                        }.`
                    );
                } else {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                }
            })
            .finally(() => {
                finishLoading();
            });
    };

    const handleCreateProduct = () => {
        console.log("teste"); 
        if (
            !name ||
            !price ||
            !comercialName ||
            !publicationDate ||
            selectedCategorie?.id === null ||
            selectedSupplier?.id === null ||
            !detail
        ) {
            return toast.warning("Preencha todos os campos");
        }
        console.log(size, productColors);
        if (
            selectedCategorie?.categoryName.toUpperCase() === "CAMISETA" ||
            selectedCategorie?.categoryName.toUpperCase() === "CAMISETAS" ||
            selectedCategorie?.categoryName.toUpperCase() === "VESTUÁRIO"
        ) {
            if (!size || !productColors) {
                return toast.warning("Preencha todos os campos");
            }
        }

        if (vouchers.length <= 0) {
            createNewProduct();
        } else {
            setModalAuthVoucherIsOpen(true);
        }
    };

    const handleChangeGroupProduct = (_: any, item: any) => {
        if (item) {
            if (
                item.categoryName.toUpperCase() === "CAMISETA" ||
                item.categoryName.toUpperCase() === "CAMISETAS" ||
                item.categoryName.toUpperCase() === "VESTUÁRIO"
            ) {
                setSizeRquired(true);
                setColorRquired(true);
            } else {
                setSizeRquired(false);
                setColorRquired(false);
            }
            setSelectedCategorie(item);
        }
    };

    const handleUpload = (event: any) => {
        const file = event.target.files[0];
        const reader = new FileReader();
        file && reader.readAsDataURL(file);
        reader.onloadend = () => {
            file && setImage(file);
        };
    };

    function handleAuthVerify() {
        if (!myUser) return;

        const payload = {
            username: authVoucherUser,
            password: authVoucherPassword,
            currentUserId: myUser.id,
        };

        new AuthVerify()
            .handle(payload)
            .then((data) => {
                if (data == true) {
                    setIsAuthVerified(true);
                } else {
                    toast.warning("Usuário não autenticado");
                }
            })
            .catch((e) => {
                if (isAxiosError(e)) {
                    const errorCode = e?.response?.data?.code;
                    let message = INTERNAL_SERVER_ERROR_MESSAGE;

                    if (
                        errorCode === EXCEPTION_CODES.PASSWORD_NOT_MATCH ||
                        errorCode === EXCEPTION_CODES.NOT_FOUND
                    ) {
                        message = "Usuário e senha não combinam";
                    }

                    toast.error(message);
                }
            })
            .finally(() => {
                finishLoading();
            });
    }

    const [categoriesList, setCategoriesList] = useState<Category[]>([]);
    const [selectedCategorie, setSelectedCategorie] = useState<Category | null>(
        null
    );
    const [sizeRequired, setSizeRquired] = useState<boolean>(false);
    const [colorRequired, setColorRquired] = useState<boolean>(false);

    function listCategory() {
        new ListCategoryUseCase()
            .handle()
            .then((data) => {
                setCategoriesList(data);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    }

    useEffect(() => {
        listCategory();
    }, []);

    function listSupplier() {
        new ListSupplierUseCase()
            .handle()
            .then((data) => {
                setSuppliers(data);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    }

    useEffect(() => {
        listSupplier();
    }, []);

    return (
        <Card width={"100%"} display={"flex"} flexDirection={"column"}>
            <PageHeader title={"Novo produto"} headerIcon={<Add />} />
            <Stack px={2} py={4} width={"100%"} gap={2}>
                <TextField
                    label="Nome do produto"
                    size="small"
                    value={comercialName}
                    onChange={(e) => setComercialName(e.target.value)}
                    required={true}
                />
                <TextField
                    label="Descrição do produto"
                    size="small"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    required={true}
                />

                <TextField
                    label="Valor do produto"
                    size="small"
                    value={price}
                    type="number"
                    required={true}
                    onChange={(e) => {
                        {
                            setPrice(parseInt(e.target.value.toString()));
                        }
                    }}
                />
                {productType !== "DIGITAL" && (
                    <TextField
                        label="Quantidade do produto"
                        size="small"
                        value={quantity}
                        type="number"
                        onChange={(e) => {
                            {
                                setQuantity(
                                    parseInt(e.target.value.toString())
                                );
                            }
                        }}
                    />
                )}
                <TextField
                    label="Limite de compra por colaborador"
                    size="small"
                    value={saleLimit}
                    type="number"
                    onChange={(e) => {
                        { 
                            setSaleLimit(parseInt(e.target.value.toString()));
                        }
                    }}
                />
                <FormControl sx={{ width: "100%" }}>
                    <InputLabel sx={{ backgroundColor: "#fff", px: 1 }}>
                        Selecionar estoque
                    </InputLabel>
                    <Select
                        onChange={(e) =>
                            setSelectedStock(e.target.value as number)
                        }
                        value={selectedStock}
                    >
                        {stocks.map((item, index) => {
                            if (!item.id) {
                                return null;
                            }
                            return (
                                <MenuItem value={item.id} key={index}>
                                    {item.description}
                                </MenuItem>
                            );
                        })}
                    </Select>
                </FormControl>
                <LocalizationProvider dateAdapter={AdapterDateFns}>
                    <DatePicker
                        label="Data de publicação"
                        value={publicationDate}
                        minDate={new Date()}
                        onChange={(newValue) => setPublicationDate(newValue)}
                        slotProps={{
                            textField: {
                                size: "small",
                                required: true,
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
                        label="Data de expiração (opcional)"
                        value={expirationDate}
                        onChange={(newValue) => setExpirationDate(newValue)}
                        minDate={publicationDate}
                        disabled={!publicationDate}
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

                <Select
                    value={productType}
                    onChange={(e) =>
                        setProductType(e.target.value as "PHYSICAL" | "DIGITAL")
                    }
                >
                    <MenuItem value={"PHYSICAL"}>Produto físico</MenuItem>
                    <MenuItem value={"DIGITAL"}>Produto digital</MenuItem>
                </Select>

                {productType === "DIGITAL" && (
                    <Box
                        display={"flex"}
                        alignItems={"center"}
                        gap={1}
                        width={"100%"}
                    >
                        <LocalizationProvider dateAdapter={AdapterDateFns}>
                            <DatePicker
                                label="Validade do Voucher"
                                value={validityDate}
                                onChange={(newValue) =>
                                    setValidityDate(newValue)
                                }
                                minDate={publicationDate}
                                slotProps={{
                                    textField: {
                                        size: "small",
                                        sx: {
                                            minWidth: "180px",
                                            svg: {
                                                color: grey[500],
                                            },
                                            width: "30%",
                                        },
                                    },
                                }}
                            />
                        </LocalizationProvider>
                        <TextField
                            label="Adicionar voucher"
                            size="small"
                            value={voucherTextValue}
                            sx={{ width: "100%" }}
                            onChange={(e) => {
                                setVoucherTextValue(e.target.value);
                            }}
                        />
                        <Button
                            variant="contained"
                            onClick={() => {
                                if (
                                    vouchers.find(
                                        (item) => item === voucherTextValue
                                    )
                                ) {
                                    return toast.warning(
                                        "Esse código de voucher já foi adicionado"
                                    );
                                }
                                setVouchers((vouchers) => [
                                    ...vouchers,
                                    voucherTextValue,
                                ]);
                            }}
                        >
                            Adicionar
                        </Button>
                    </Box>
                )}
                <Select
                    value={status}
                    onChange={(e) =>
                        setStatus(e.target.value as "Ativo" | "Inativo")
                    }
                >
                    <MenuItem value={"Ativo"}>Ativo</MenuItem>
                    <MenuItem value={"Inativo"}>Inativo</MenuItem>
                </Select>
                {/* <Box display={"flex"} alignItems={"center"}>
                    <Typography fontSize={"14px"}>
                        É um produto destaque?
                    </Typography>
                    <Checkbox
                        checked={highlight}
                        onChange={(e) => setHighlight(e.target.checked)}
                    />
                </Box> */}
                <Card>
                    <PageHeader
                        title="Configurar visibilidade"
                        headerIcon={<Visibility />}
                        addButtonAction={() =>
                            setIsVisibilityConfigVisible(true)
                        }
                        addButtonTitle="Configurar"
                    />
                    {isVisibilityConfigVisible && (
                        <Box p={2} display={"flex"} flexDirection={"column"}>
                            <Box
                                width={"100%"}
                                display={"flex"}
                                gap={1}
                                alignItems={"center"}
                            >
                                <FormControl sx={{ width: "200px" }}>
                                    <InputLabel
                                        sx={{ backgroundColor: "#fff", px: 1 }}
                                    >
                                        Filtrar por tipo:
                                    </InputLabel>
                                    <Select
                                        onChange={(e) =>
                                            setVisibilityType(
                                                e.target.value as
                                                    | "COLLABORATOR"
                                                    | "HIERARCHY"
                                                    | "GROUP"
                                                    | "ATTRIBUTE"
                                                    | "CLIENT"
                                                    | "SECTOR"
                                            )
                                        }
                                        value={visibilityType}
                                        sx={{ width: "100%" }}
                                    >
                                        <MenuItem value={"COLLABORATOR"}>
                                            Colaborador
                                        </MenuItem>
                                        <MenuItem value={"HIERARCHY"}>
                                            Hierarquia
                                        </MenuItem>
                                        <MenuItem value={"GROUP"}>
                                            Grupo
                                        </MenuItem>
                                        <MenuItem value={"SECTOR"}>
                                            Setor
                                        </MenuItem>
                                    </Select>
                                </FormControl>
                                {/* <TextField
                                    label="Valor"
                                    value={visibilityValue}
                                    onChange={(e) =>
                                        setVisibilityValue(e.target.value)
                                    }
                                /> */}
                                {visibilityType == "SECTOR" && (
                                    <Autocomplete
                                        value={sector}
                                        placeholder={"Setor"}
                                        disableClearable={false}
                                        onChange={(e, value) => {
                                            if (value) {
                                                setVisibilityValue(value.name);
                                            } else {
                                                setVisibilityValue("");
                                            }
                                        }}
                                        onInputChange={(e, text) =>
                                            setSectorsSearchValue(text)
                                        }
                                        sx={{
                                            mb: 0,
                                            maxWidth: "400px",
                                            width: "100%",
                                        }}
                                        renderInput={(props) => (
                                            <TextField
                                                {...props}
                                                size={"medium"}
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
                                                                props.InputProps
                                                                    .endAdornment
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
                                )}
                                {visibilityType == "HIERARCHY" && (
                                    <Autocomplete
                                        value={hierarchie}
                                        placeholder={"Hierarquia"}
                                        disableClearable={false}
                                        onChange={(e, value) => {
                                            if (value) {
                                                setVisibilityValue(
                                                    value.levelName
                                                );
                                            } else {
                                                setVisibilityValue("");
                                            }
                                        }}
                                        onInputChange={(e, text) =>
                                            setHierarchiesSearchValue(text)
                                        }
                                        sx={{
                                            mb: 0,
                                            maxWidth: "400px",
                                            width: "100%",
                                        }}
                                        renderInput={(props) => (
                                            <TextField
                                                {...props}
                                                size={"medium"}
                                                label={"Hierarquia"}
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
                                                                props.InputProps
                                                                    .endAdornment
                                                            )}
                                                        </>
                                                    ),
                                                }}
                                            />
                                        )}
                                        isOptionEqualToValue={(option, value) =>
                                            option.levelName === value.levelName
                                        }
                                        getOptionLabel={(option) =>
                                            `${option.id} - ${option.levelName}`
                                        }
                                        options={hierarchies}
                                    />
                                )}
                                {visibilityType == "GROUP" && (
                                    <Autocomplete
                                        value={group}
                                        placeholder={"Grupo"}
                                        disableClearable={false}
                                        onChange={(e, value) => {
                                            if (value) {
                                                setVisibilityValue(value.name);
                                            } else {
                                                setVisibilityValue("");
                                            }
                                        }}
                                        onInputChange={(e, text) =>
                                            setGroupsSearchValue(text)
                                        }
                                        sx={{
                                            mb: 0,
                                            maxWidth: "400px",
                                            width: "100%",
                                        }}
                                        renderInput={(props) => (
                                            <TextField
                                                {...props}
                                                size={"medium"}
                                                label={"Grupo"}
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
                                                                props.InputProps
                                                                    .endAdornment
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
                                        options={groups}
                                    />
                                )}
                                {visibilityType == "COLLABORATOR" && (
                                    <Autocomplete
                                        value={collaborator}
                                        placeholder={"Colaborador"}
                                        disableClearable={false}
                                        onChange={(e, value) => {
                                            if (value) {
                                                setVisibilityValue(value.name);
                                            } else {
                                                setVisibilityValue("");
                                            }
                                        }}
                                        onInputChange={(e, text) =>
                                            setCollaboratorsSearchValue(text)
                                        }
                                        filterOptions={(
                                            options,
                                            { inputValue }
                                        ) =>
                                            options.filter(
                                                (item) =>
                                                    item.id
                                                        .toString()
                                                        .includes(
                                                            inputValue.toString()
                                                        ) ||
                                                    item.name
                                                        .toString()
                                                        .toLowerCase()
                                                        .includes(
                                                            inputValue
                                                                .toString()
                                                                .toLowerCase()
                                                        )
                                            )
                                        }
                                        sx={{
                                            mb: 0,
                                            maxWidth: "400px",
                                            width: "100%",
                                        }}
                                        renderInput={(props) => (
                                            <TextField
                                                {...props}
                                                size={"medium"}
                                                label={"Colaborador"}
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
                                                                props.InputProps
                                                                    .endAdornment
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
                                            `${option.name}`
                                        }
                                        options={collaborators}
                                    />
                                )}
                                <Button
                                    variant="outlined"
                                    onClick={() => {
                                        if (!visibilityValue) {
                                            return toast.warning(
                                                "Adicione um valor"
                                            );
                                        }
                                        if (
                                            visibilityType == "COLLABORATOR" &&
                                            visibilitys.find(
                                                (item) =>
                                                    item.value ==
                                                    visibilityValue
                                            )
                                        ) {
                                            return toast.warning(
                                                "Colaborador já adicionado"
                                            );
                                        }
                                        let arr = visibilitys.slice();
                                        arr.push({
                                            type: visibilityType,
                                            value: visibilityValue,
                                        });
                                        setVisibilitys(arr);
                                    }}
                                >
                                    Adicionar
                                </Button>
                            </Box>
                            <Box
                                width={"100%"}
                                display={"flex"}
                                flexDirection={"column"}
                                gap={1}
                                mt={2}
                            >
                                {visibilitys.map((item, index) => (
                                    <Box
                                        sx={{
                                            display: "flex",
                                            alignItems: "center",
                                            width: "400px",
                                            justifyContent: "space-between",
                                        }}
                                        key={index}
                                    >
                                        <Box display={"flex"} gap={"5px"}>
                                            <Typography>
                                                {translateVisibiltyName(
                                                    item.type
                                                )}
                                            </Typography>
                                            {" - "}
                                            <Typography>
                                                {item.value}
                                            </Typography>
                                        </Box>

                                        <Button
                                            color="error"
                                            onClick={() => {
                                                let arr = visibilitys.slice();
                                                arr.splice(index, 1);
                                                setVisibilitys(arr);
                                            }}
                                        >
                                            remover
                                        </Button>
                                    </Box>
                                ))}
                            </Box>
                        </Box>
                    )}
                </Card>
                {/* ------------------------------------------------------------------------------------------------------------------ */}
                <Autocomplete
                    value={selectedCategorie}
                    placeholder={"Grupo do produto"}
                    filterOptions={(x) => x}
                    disableClearable={false}
                    onChange={handleChangeGroupProduct}
                    isOptionEqualToValue={(option, value) =>
                        option.categoryName === value.categoryName
                    }
                    renderInput={(props) => (
                        <TextField
                            {...props}
                            size={"small"}
                            label={"Grupo do Produto"}
                            required={true}
                        />
                    )}
                    renderTags={() => null}
                    getOptionLabel={(option) => option.categoryName}
                    options={categoriesList}
                    sx={{ mb: 0 }}
                />
                <Autocomplete
                    value={selectedSupplier}
                    placeholder={"Fornecedores"}
                    filterOptions={(x) => x}
                    disableClearable={false}
                    onChange={(_, item) => setSelectedSupplier(item)}
                    isOptionEqualToValue={(option, value) =>
                        option.supplierName === value.supplierName
                    }
                    renderInput={(props) => (
                        <TextField
                            {...props}
                            size={"small"}
                            label={"Fornecedor"}
                            required={true}
                        />
                    )}
                    renderTags={() => null}
                    getOptionLabel={(option) => option.supplierName}
                    options={suppliers}
                    sx={{ mb: 0 }}
                />
                <ProductSizes
                    getValue={(value) => setSize(value)}
                    isRequired={sizeRequired}
                />
                <ProductDetails getValue={(value) => setDetail(value)} />
                <ProductColors
                    getValue={(value) => setProductColors(value)}
                    isRequired={colorRequired}
                />
                {/* <ProductGroup getValue={(value) => setProductGroup(value)} /> */}
                {/* ------------------------------------------------------------------------------------------------------------------ */}
                <Box
                    display={"flex"}
                    alignItems={"center"}
                    gap={1}
                    width={"100%"}
                    flexWrap={"wrap"}
                >
                    {vouchers.map((item, index) => (
                        <Chip
                            label={item}
                            key={index}
                            onDelete={() => {
                                setVouchers((current) =>
                                    current.filter(
                                        (voucher) => voucher !== item
                                    )
                                );
                            }}
                        />
                    ))}
                </Box>
                <input
                    accept="image/*"
                    style={{ display: "none" }}
                    id="image-upload"
                    type="file"
                    onChange={handleUpload}
                />
                {/* <label htmlFor="image-upload">
                    <Button
                        variant="outlined"
                        color="primary"
                        component="span"
                        fullWidth
                    >
                        Imagem do produto
                    </Button>
                </label> */}
                <Button
                    variant="outlined"
                    color="primary"
                    fullWidth
                    onClick={() => setModalGalleryImagesOpen(true)}
                >
                    Imagem do produto
                </Button>
                <BaseModal
                    fullWidth
                    open={modalGalleryImagesOpen}
                    title={`Selecione uma imagem da galeria`}
                    onClose={() => setModalGalleryImagesOpen(false)}
                >
                    <Box
                        width={"100%"}
                        display={"flex"}
                        flexDirection={"column"}
                        gap={1}
                    >
                        <ImageGallery
                            getItemOnClick={(image) => setSelectedImage(image)}
                            onClose={() => setModalGalleryImagesOpen(false)}
                        />
                    </Box>
                </BaseModal>
                {selectedImage && (
                    <Card
                        sx={{
                            width: "280px",
                            height: "280px",
                            p: 5,
                            mt: 2,
                            display: "flex",
                            justifyContent: "center",
                            alignItems: "center",
                            flexDirection: "column",
                            gap: "30px",
                        }}
                    >
                        <CardMedia
                            component="img"
                            alt="Uploaded image"
                            image={selectedImage.url}
                        />
                        <Button
                            variant="contained"
                            color="error"
                            onClick={() => setSelectedImage(null)}
                        >
                            Remover imagem
                        </Button>
                    </Card>
                )}
                {/* {image && (
                    <Card
                        sx={{
                            width: "280px",
                            height: "280px",
                            p: 5,
                            mt: 2,
                            display: "flex",
                            justifyContent: "center",
                            alignItems: "center",
                            flexDirection: "column",
                            gap: "30px",
                        }}
                    >
                        <CardMedia
                            component="img"
                            alt="Uploaded image"
                            image={URL.createObjectURL(image as File)}
                        />
                        <Button
                            variant="contained"
                            color="error"
                            onClick={() => setImage(null)}
                        >
                            Remover imagem
                        </Button>
                    </Card>
                )} */}
                <Box display={"flex"} justifyContent={"flex-end"}>
                    <Button
                        variant="contained"
                        onClick={handleCreateProduct}
                        disabled={
                            isLoading ||
                            abilityFor(myPermissions).cannot(
                                "Cadastrar Produto",
                                "Marketing Place"
                            )
                        }
                    > 
                        Criar produto
                    </Button>
                </Box>
            </Stack>
            <BaseModal
                width={"540px"}
                open={modalAuthVoucherIsOpen}
                title={"Validar login para criação de voucher"}
                onClose={() => setModalAuthVoucherIsOpen(false)}
            >
                {isAuthVerified ? (
                    <Box width={"100%"}>
                        <Box
                            display={"flex"}
                            gap={1}
                            width={"100%"}
                            alignItems={"center"}
                            justifyContent={"center"}
                            py={"50px"}
                        >
                            <CheckCircle color="success" />
                            <Typography>
                                Usuário autenticado com sucesso!
                            </Typography>
                        </Box>
                        <Button
                            variant="contained"
                            onClick={createNewProduct}
                            fullWidth
                            sx={{ mt: 2 }}
                            disabled={!myUser}
                            color="success"
                        >
                            Confirmar criação do produto
                        </Button>
                    </Box>
                ) : (
                    <Box width={"100%"}>
                        <Box
                            display={"flex"}
                            gap={1}
                            width={"100%"}
                            flexDirection={"column"}
                        >
                            <TextField
                                fullWidth
                                label="Nome de usuário"
                                value={authVoucherUser}
                                onChange={(e) =>
                                    setAuthVoucherUser(e.target.value)
                                }
                            ></TextField>
                            <TextField
                                fullWidth
                                type="password"
                                label="Senha"
                                value={authVoucherPassword}
                                onChange={(e) =>
                                    setAuthVoucherPassword(e.target.value)
                                }
                            ></TextField>
                        </Box>
                        <Button
                            variant="contained"
                            onClick={handleAuthVerify}
                            fullWidth
                            sx={{ mt: 2 }}
                            disabled={!myUser}
                        >
                            Validar login
                        </Button>
                    </Box>
                )}
            </BaseModal>
        </Card>
    );
}

CreateProduct.getLayout = getLayout("private");
