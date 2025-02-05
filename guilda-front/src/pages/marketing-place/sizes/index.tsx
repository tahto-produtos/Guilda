import Settings from "@mui/icons-material/Settings";
import { Box, Button, Stack, TextField } from "@mui/material";
import { grey } from "@mui/material/colors";
import { useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { BaseModal } from "src/components/feedback";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { CreateCategoryUseCase } from "src/modules/marketing-place/use-cases/create-category.use-case";
import { CreateSupplierUseCase } from "src/modules/marketing-place/use-cases/create-supplier.use-case";
import { DeleteSupplierUseCase } from "src/modules/marketing-place/use-cases/delete-supplier.use-case";
import { ListCategoryUseCase } from "src/modules/marketing-place/use-cases/list-category.use-case";
import { ListSupplierUseCase } from "src/modules/marketing-place/use-cases/list-suppliers.use-case";
import { UpdateSupplierUseCase } from "src/modules/marketing-place/use-cases/update-supplier.use-case";
import { getLayout } from "src/utils";

interface Supplier {
    supplierName: string;
    createdAt: string;
    createdByCollaboratorId: number;
    deletedAt: string;
    id: number;
}

export default function SupplierView() {
    return (
        <Card width={"100%"} display={"flex"} flexDirection={"column"}>
            <PageHeader
                title={"Tamanhos (Produtos)"}
                headerIcon={<Settings />}
            />
            <Stack px={2} py={4} width={"100%"} gap={2}></Stack>
        </Card>
    );
}

SupplierView.getLayout = getLayout("private");
