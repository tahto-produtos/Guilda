import Settings from "@mui/icons-material/Settings";
import { Box, Button, Stack, Tab, Tabs, TextField } from "@mui/material";
import { grey } from "@mui/material/colors";
import { ReactElement, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { BaseModal } from "src/components/feedback";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { ProductCategory } from "src/modules/categories/product-category/product-category";
import { ProductGroups } from "src/modules/categories/product-group/product-group";
import { ProductGroup } from "src/modules/marketing-place/fragments/product-group";
import { CreateCategoryUseCase } from "src/modules/marketing-place/use-cases/create-category.use-case";
import { DeleteCategoryUseCase } from "src/modules/marketing-place/use-cases/delete-category.use-case";
import { ListCategoryUseCase } from "src/modules/marketing-place/use-cases/list-category.use-case";
import { UpdateCategoryUseCase } from "src/modules/marketing-place/use-cases/update-category.use-case";
import { getLayout } from "src/utils";
import SupplierView from "../supplier";
import { ProductSizesView } from "src/modules/categories/sizes/product-sizes-view";
import { ProductColorsView } from "src/modules/categories/product-colors/product-colors-view";
import { ProductDetailsView } from "src/modules/categories/product-details/product-details-view";

export interface Category {
    categoryName: string;
    createdAt: string;
    createdByCollaboratorId: number;
    deletedAt: string;
    id: number;
}

export default function CategoryView() {
    const [value, setValue] = useState(0);

    const handleChange = (event: React.SyntheticEvent, newValue: number) => {
        setValue(newValue);
    };

    const components: { [key in number]: ReactElement } = {
        0: <ProductCategory />,
        // 1: <ProductGroups />,
        2: <SupplierView />,
        3: <ProductSizesView />,
        4: <ProductColorsView />,
        5: <ProductDetailsView />,
    };

    return (
        <Box
            sx={{
                display: "flex",
                flexDirection: "column",
                gap: 2,
                width: "100%",
            }}
        >
            <Card>
                <Tabs
                    value={value}
                    onChange={handleChange}
                    variant="scrollable"
                    scrollButtons="auto"
                    aria-label="scrollable auto tabs example"
                >
                    <Tab label="Grupos " value={0} />
                    {/* <Tab label="Grupos " value={1} /> */}
                    <Tab label="Fornecedores" value={2} />
                    <Tab label="Tamanhos" value={3} />
                    <Tab label="Cores" value={4} />
                    <Tab label="Detalhes" value={5} />
                </Tabs>
            </Card>
            {components[value]}
        </Box>
    );
}

CategoryView.getLayout = getLayout("private");
