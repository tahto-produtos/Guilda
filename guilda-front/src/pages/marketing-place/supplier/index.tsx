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
    const [isOpen, setIsOpen] = useState<boolean>(false);
    const [newSupplierName, setNewSupplierName] = useState<string>("");
    const [supplierList, setSupplierList] = useState<Supplier[]>([]);
    const [selected, setSelected] = useState<Supplier | null>(null);
    const [editName, setEditName] = useState<string>("");

    function handleCreateCategory() {
        if (!newSupplierName) {
            return toast.warning("Escolha um nome para o fornecedor.");
        }

        new CreateSupplierUseCase()
            .handle({ supplierName: newSupplierName })
            .then((data) => {
                toast.success("Fornecedor criado com sucesso!");
                listCategory();
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    }

    function listCategory() {
        new ListSupplierUseCase()
            .handle()
            .then((data) => {
                setSupplierList(data);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    }

    useEffect(() => {
        listCategory();
    }, []);

    function handleDelete(id: number) {
        new DeleteSupplierUseCase()
            .handle({ id })
            .then(() => {
                toast.success("Fornecedor apagado com sucesso!");
                listCategory();
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    }

    function handleUpdateCategory(id: number) {
        new UpdateSupplierUseCase()
            .handle({ id, supplierName: editName })
            .then(() => {
                toast.success("Fornecedor editado com sucesso!");
                setSelected(null);
                listCategory();
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    }

    return (
        <Card width={"100%"} display={"flex"} flexDirection={"column"}>
            <PageHeader
                title={"Fornecedores"}
                headerIcon={<Settings />}
                addButtonAction={() => setIsOpen(true)}
                addButtonTitle="Novo Fornecedor"
            />
            <Stack px={2} py={4} width={"100%"} gap={2}>
                {supplierList.map((item) => (
                    <Box
                        key={item.id}
                        display={"flex"}
                        flexDirection={"row"}
                        gap={2}
                        bgcolor={grey[100]}
                        py={1}
                        px={2}
                        justifyContent={"space-between"}
                    >
                        {item.supplierName}
                        <Box display={"flex"} gap={2} alignItems={"center"}>
                            {/* <Button
                                variant="outlined"
                                onClick={() => {
                                    setSelected(item);
                                }}
                            >
                                Editar
                            </Button> */}
                            <Button
                                variant="contained"
                                color="error"
                                onClick={() => handleDelete(item.id)}
                            >
                                Apagar
                            </Button>
                        </Box>
                    </Box>
                ))}
            </Stack>
            <BaseModal
                width={"540px"}
                open={isOpen}
                title={`Novo Fornecedor`}
                onClose={() => setIsOpen(false)}
            >
                <Box
                    width={"100%"}
                    display={"flex"}
                    flexDirection={"column"}
                    gap={2}
                >
                    <TextField
                        label="Nome do fornecedor"
                        value={newSupplierName}
                        onChange={(e) => setNewSupplierName(e.target.value)}
                    />
                    <Button
                        fullWidth
                        variant="contained"
                        onClick={handleCreateCategory}
                        disabled={!newSupplierName}
                    >
                        Criar
                    </Button>
                </Box>
            </BaseModal>
            <BaseModal
                width={"540px"}
                open={!!selected}
                title={`Editar: ${selected?.supplierName}`}
                onClose={() => setSelected(null)}
            >
                <Box
                    width={"100%"}
                    display={"flex"}
                    flexDirection={"column"}
                    gap={2}
                >
                    <TextField
                        label="Novo nome para o fornecedor"
                        value={editName}
                        onChange={(e) => setEditName(e.target.value)}
                    />
                    <Button
                        fullWidth
                        variant="contained"
                        onClick={() =>
                            selected && handleUpdateCategory(selected.id)
                        }
                        disabled={!editName}
                    >
                        Criar
                    </Button>
                </Box>
            </BaseModal>
        </Card>
    );
}

SupplierView.getLayout = getLayout("private");
