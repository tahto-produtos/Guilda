import Add from "@mui/icons-material/Add";
import Storefront from "@mui/icons-material/Storefront";
import {
    Autocomplete,
    Box,
    Button,
    CardMedia,
    MenuItem,
    Pagination,
    Select,
    Stack,
    TextField,
} from "@mui/material";
import { useRouter } from "next/router";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";
import { useLoadingState } from "src/hooks";
import Showcase from "src/modules/marketing-place/fragments/showcase";
import { CreateProductUseCase } from "src/modules/marketing-place/use-cases/create-product.use-case";
import { CreateStockTypeUseCase } from "src/modules/marketing-place/use-cases/create-stock-type.use-case";
import { CreateStockUseCase } from "src/modules/marketing-place/use-cases/create-stock.use-case";
import { ListProducts } from "src/modules/marketing-place/use-cases/list-products";
import { ListStockTypes } from "src/modules/marketing-place/use-cases/list-stock-types";
import { Product } from "src/typings/models/product.model";
import { getLayout } from "src/utils";
import abilityFor from "src/utils/ability-for";

export default function CreateStockType() {
    const { myPermissions } = useContext(PermissionsContext);
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [image, setImage] = useState<File | null>(null);
    const [name, setName] = useState<string>("");
    const [code, setCode] = useState<string>("");
    const [type, setType] = useState<any>(null);
    const [region, setRegion] = useState<string>("");
    const [typeList, setTypeList] = useState<
        Array<{ id: number; type: string; createdAt: string }>
    >([]);

    const handleCreateStock = async () => {
        if (!name) {
            return toast.warning("Escolha um nome");
        }

        startLoading();

        const payload = {
            type: name,
        };

        new CreateStockTypeUseCase()
            .handle(payload)
            .then((data) => {
                toast.success(`Tipo de estoque '${name}' criado com sucesso!`);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    };

    return (
        <Card width={"100%"} display={"flex"} flexDirection={"column"}>
            {/* <PageHeader title={"Novo tipo de estoque"} headerIcon={<Add />} />
            <Stack px={2} py={4} width={"100%"} gap={2}>
                <TextField
                    label="Nome do tipo de estoque"
                    size="small"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                />
                <Box display={"flex"} justifyContent={"flex-end"}>
                    <Button variant="contained" onClick={handleCreateStock}>
                        Criar estoque
                    </Button>
                </Box>
            </Stack> */}
        </Card>
    );
}

CreateStockType.getLayout = getLayout("private");
