import Settings from "@mui/icons-material/Settings";
import { Box, Button, Stack, TextField } from "@mui/material";
import { grey } from "@mui/material/colors";
import { useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { BaseModal } from "src/components/feedback";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { CreateCategoryUseCase } from "src/modules/marketing-place/use-cases/create-category.use-case";
import { DeleteCategoryUseCase } from "src/modules/marketing-place/use-cases/delete-category.use-case";
import { ListCategoryUseCase } from "src/modules/marketing-place/use-cases/list-category.use-case";
import { UpdateCategoryUseCase } from "src/modules/marketing-place/use-cases/update-category.use-case";
import { Category } from "src/pages/marketing-place/category";

export function ProductCategory() {
    const [isOpen, setIsOpen] = useState<boolean>(false);
    const [newCategoryName, setNewCategoryName] = useState<string>("");
    const [categoriesList, setCategoriesList] = useState<Category[]>([]);
    const [selected, setSelected] = useState<Category | null>(null);
    const [editName, setEditName] = useState<string>("");

    function handleCreateCategory() {
        if (!newCategoryName) {
            return toast.warning("Escolha um nome para a categoria.");
        }

        new CreateCategoryUseCase()
            .handle({ categoryName: newCategoryName })
            .then(() => {
                toast.success("Categoria criada com sucesso!");
                listCategory();
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    }

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

    function handleDelete(id: number) {
        new DeleteCategoryUseCase()
            .handle({ id })
            .then(() => {
                toast.success("Categoria apagada com sucesso!");
                listCategory();
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    }

    function handleUpdateCategory(id: number) {
        new UpdateCategoryUseCase()
            .handle({ id, categoryName: editName })
            .then(() => {
                toast.success("Categoria editada com sucesso!");
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
                title={"Grupos"}
                headerIcon={<Settings />}
                addButtonAction={() => setIsOpen(true)}
                addButtonTitle="Novo Grupo"
            />
            <Stack px={2} py={4} width={"100%"} gap={2}>
                {categoriesList.map((item) => (
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
                        {item.categoryName}
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
                title={`Novo Grupo`}
                onClose={() => setIsOpen(false)}
            >
                <Box
                    width={"100%"}
                    display={"flex"}
                    flexDirection={"column"}
                    gap={2}
                >
                    <TextField
                        label="Nome do grupo"
                        value={newCategoryName}
                        onChange={(e) => setNewCategoryName(e.target.value)}
                    />
                    <Button
                        fullWidth
                        variant="contained"
                        onClick={handleCreateCategory}
                        disabled={!newCategoryName}
                    >
                        Criar
                    </Button>
                </Box>
            </BaseModal>
            <BaseModal
                width={"540px"}
                open={!!selected}
                title={`Editar: ${selected?.categoryName}`}
                onClose={() => setSelected(null)}
            >
                <Box
                    width={"100%"}
                    display={"flex"}
                    flexDirection={"column"}
                    gap={2}
                >
                    <TextField
                        label="Novo nome para o grupo"
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
