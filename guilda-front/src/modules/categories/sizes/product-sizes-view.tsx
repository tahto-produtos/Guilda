import Settings from "@mui/icons-material/Settings";
import {
    Autocomplete,
    Box,
    Button,
    Stack,
    TextField,
    Typography,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { BaseModal } from "src/components/feedback";
import { CreateProductGroup } from "src/modules/marketing-place/use-cases/create-product-group.use-case";
import { CreateSizes } from "src/modules/marketing-place/use-cases/create-size.use-case";
import { DeleteProductGroup } from "src/modules/marketing-place/use-cases/delete-product-group.use-case";
import { DeleteSizes } from "src/modules/marketing-place/use-cases/delete-sizes.use-case";
import { ListProductGroup } from "src/modules/marketing-place/use-cases/list-product-group.use-case";
import { ListSizes } from "src/modules/marketing-place/use-cases/list-sizes.use-case";

export interface Size {
    id: number;
    sizeName: string;
    createdAt: string;
    deletedAt: string;
    createdByCollaboratorId: number;
}

export function ProductSizesView() {
    const [data, setData] = useState<Size[]>([]);
    const [selectedOption, setSelectedOption] = useState<Size | null>(null);
    const [searchText, setSearchText] = useState<string>("");
    const [isOpen, setIsOpen] = useState<boolean>(false);
    const [newItem, setNewItem] = useState<string>("");

    const getList = async () => {
        new ListSizes()
            .handle(searchText)
            .then((data) => {
                setData(data);
            })
            .catch(() => {});
    };

    useEffect(() => {
        getList();
    }, []);

    async function handleCreate() {
        new CreateSizes()
            .handle(newItem)
            .then((data) => {
                getList();
                toast.success("Nova opção salva com sucesso!");
                setIsOpen(false);
            })
            .catch(() => {
                toast.error("Erro ao salvar nova opção.");
            });
    }

    async function handleDelete(id: number) {
        new DeleteSizes()
            .handle(id)
            .then((data) => {
                getList();
                toast.success("Opção apagada com sucesso!");
                setSelectedOption(null);
            })
            .catch(() => {
                toast.error("Erro ao apagar a opção.");
            });
    }
    return (
        <Card width={"100%"} display={"flex"} flexDirection={"column"}>
            <PageHeader
                title={"Tamanhos"}
                headerIcon={<Settings />}
                addButtonAction={() => setIsOpen(true)}
                addButtonTitle="Novo Tamanho"
            />
            <Stack px={2} py={4} width={"100%"} gap={2}>
                {/* <TextField
                    onChange={(e) => setSearchText(e.target.value)}
                    value={searchText}
                    label="Buscar"
                /> */}
                {data.map((item) => (
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
                        {item.sizeName}
                        <Box display={"flex"} gap={2} alignItems={"center"}>
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
                title={`Novo tamanho`}
                onClose={() => setIsOpen(false)}
            >
                <Box
                    width={"100%"}
                    display={"flex"}
                    flexDirection={"column"}
                    gap={1}
                >
                    <TextField
                        value={newItem}
                        onChange={(e) => setNewItem(e.target.value)}
                        label={"Digite o novo tamanho"}
                    />
                    <Button
                        variant="contained"
                        fullWidth
                        onClick={handleCreate}
                    >
                        Salvar
                    </Button>
                </Box>
            </BaseModal>
        </Card>
    );
}
