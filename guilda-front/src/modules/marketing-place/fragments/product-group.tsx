import {
    Autocomplete,
    Box,
    Button,
    TextField,
    Typography,
} from "@mui/material";
import { ListSizes } from "../use-cases/list-sizes.use-case";
import { useEffect, useState } from "react";
import { CreateSizes } from "../use-cases/create-size.use-case";
import { BaseModal } from "src/components/feedback";
import { toast } from "react-toastify";
import { DeleteSizes } from "../use-cases/delete-sizes.use-case";
import { ListProductGroup } from "../use-cases/list-product-group.use-case";
import { CreateProductGroup } from "../use-cases/create-product-group.use-case";
import { DeleteProductGroup } from "../use-cases/delete-product-group.use-case";

export interface Group {
    id: number;
    groupName: string;
    createdAt: string;
    deletedAt: string;
    createdByCollaboratorId: number;
}

interface IProps {
    getValue?: (input: number | undefined) => void;
    initialOptionId?: number;
}

export function ProductGroup(props: IProps) {
    const { getValue, initialOptionId } = props;

    const [data, setData] = useState<Group[]>([]);
    const [selectedOption, setSelectedOption] = useState<Group | null>(null);
    const [searchText, setSearchText] = useState<string>("");
    const [isOpen, setIsOpen] = useState<boolean>(false);
    const [newItem, setNewItem] = useState<string>("");
    const [firstFetch, setFirstFetch] = useState<boolean>(false);

    useEffect(() => {
        if (initialOptionId && firstFetch) {
            const x = data.find((item) => item.id == initialOptionId) || null;
            setSelectedOption(x);
        }
    }, [initialOptionId, firstFetch]);

    const getList = async () => {
        new ListProductGroup()
            .handle(searchText)
            .then((data) => {
                setData(data);
                setFirstFetch(true);
            })
            .catch(() => {});
    };

    useEffect(() => {
        getList();
    }, []);

    async function handleCreate() {
        new CreateProductGroup()
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
        new DeleteProductGroup()
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

    useEffect(() => {
        getValue && getValue(selectedOption?.id);
    }, [selectedOption]);

    return (
        <Box display={"flex"} gap={2}>
            <Autocomplete
                value={selectedOption}
                placeholder={"Grupo"}
                fullWidth
                disableClearable={false}
                onChange={(_, item) => setSelectedOption(item)}
                onInputChange={(e, text) => setSearchText(text)}
                isOptionEqualToValue={(option, value) =>
                    option.groupName === value.groupName
                }
                // renderOption={(props, option) => (
                //     <Box
                //         component="li"
                //         sx={{
                //             "& > img": { mr: 2, flexShrink: 0 },
                //             display: "flex",
                //             flexDirection: "row",
                //             alignItems: "center",
                //         }}
                //         {...props}
                //         gap={4}
                //         justifyContent={"space-between"}
                //     >
                //         <Typography width={"400px"}>
                //             {option.groupName}
                //         </Typography>
                //         <Button
                //             variant={"outlined"}
                //             onClick={() => handleDelete(option.id)}
                //         >
                //             Apagar
                //         </Button>
                //     </Box>
                // )}
                renderInput={(props) => (
                    <TextField {...props} size={"small"} label={"Grupo"} />
                )}
                renderTags={() => null}
                getOptionLabel={(option) => option.groupName}
                options={data}
                sx={{ mb: 0 }}
            />
            {/* <Button
                sx={{ width: "200px" }}
                variant="outlined"
                onClick={() => setIsOpen(true)}
            >
                Novo grupo
            </Button> */}
            <BaseModal
                width={"540px"}
                open={isOpen}
                title={`Novo grupo`}
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
                        label={"Digite o novo grupo"}
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
        </Box>
    );
}
