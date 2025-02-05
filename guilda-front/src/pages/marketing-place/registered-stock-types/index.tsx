import Category from "@mui/icons-material/Category";
import Inventory from "@mui/icons-material/Inventory";
import {
    Box,
    Button,
    CircularProgress,
    Pagination,
    Stack,
    TextField,
    Typography,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { useRouter } from "next/router";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import WithoutPermissionCard from "src/components/data-display/without-permission/without-permissions";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";
import { useLoadingState } from "src/hooks";
import { CollaboratorDetailUseCase } from "src/modules/collaborators/use-cases/collaborator-details.use-case";
import Showcase from "src/modules/marketing-place/fragments/showcase";
import { DeleteStock } from "src/modules/marketing-place/use-cases/delete-stock";
import { ListProducts } from "src/modules/marketing-place/use-cases/list-products";
import { ListStockTypes } from "src/modules/marketing-place/use-cases/list-stock-types";
import { ListStocks } from "src/modules/marketing-place/use-cases/list-stocks";
import { Product } from "src/typings/models/product.model";
import { StockType } from "src/typings/models/stock-type.model";
import { Stock } from "src/typings/models/stock.model";
import { DateUtils, getLayout } from "src/utils";
import abilityFor from "src/utils/ability-for";

export default function RegisteredStockTypes() {
    const [searchText, setSearchText] = useState<string>("");
    const { myPermissions } = useContext(PermissionsContext);
    const router = useRouter();
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [page, setPage] = useState<number>(1);
    const handleChangePagination = (
        event: React.ChangeEvent<unknown>,
        value: number
    ) => {
        setPage(value);
    };
    const [totalPages, setTotalPages] = useState<number>(0);
    const [stocks, setStocks] = useState<Array<Stock>>([]);
    const LIMIT_PER_PAGE = 20;
    const [typeList, setTypeList] = useState<StockType[]>([]);

    const handleRedirectToCreateStockTypePage = () =>
        router.push("/marketing-place/create-stock-type");

    const getTypes = async () => {
        startLoading();

        const payload = {
            limit: 999,
            offset: 0,
            searchText: "",
        };

        new ListStockTypes()
            .handle(payload)
            .then((data) => {
                setTypeList(data.items);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    };

    useEffect(() => {
        getTypes();
    }, []);

    const TypeItem = (props: { data: StockType }) => {
        const { data } = props;

        return (
            <Box
                border={`solid 1px ${grey[200]}`}
                display={"flex"}
                width={"100%"}
                borderRadius={2}
                p={2}
                alignItems={"center"}
                justifyContent={"space-between"}
            >
                <Box display={"flex"} alignItems={"center"} gap={2}>
                    <Box>
                        <Typography variant="body2" fontSize={"16px"}>
                            {data.type}
                        </Typography>
                        <Typography
                            variant="body2"
                            fontSize={"12px"}
                            color={grey[600]}
                        >
                            {data.createdAt &&
                                `criado em: ${DateUtils.formatDatePtBRWithTime(
                                    data.createdAt
                                )}`}
                            {}
                        </Typography>
                    </Box>
                </Box>
                {/* <Button
                    color="error"
                    onClick={() => handleDeleteStock(data.id)}
                >
                    Apagar tipo de estoque
                </Button> */}
            </Box>
        );
    };

    return (
        <Card width={"100%"} display={"flex"} flexDirection={"column"}>
            {/* <PageHeader
                title={"Tipos de estoque"}
                headerIcon={<Inventory />}
                addButtonTitle="Novo tipo de estoque"
                addButtonAction={handleRedirectToCreateStockTypePage}
            />
            <Stack px={2} py={4} width={"100%"} gap={2}>
                <Box display={"flex"} flexDirection={"column"} gap={1}>
                    {typeList &&
                        typeList.map((type, index) => (
                            <TypeItem key={index} data={type} />
                        ))}
                </Box>
            </Stack> */}
        </Card>
    );
}

RegisteredStockTypes.getLayout = getLayout("private");
