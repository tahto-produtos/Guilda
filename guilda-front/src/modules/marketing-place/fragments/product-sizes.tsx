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

export interface Size {
    id: number;
    sizeName: string;
    createdAt: string;
    deletedAt: string;
    createdByCollaboratorId: number;
}

interface IProps {
    getValue?: (input: number | undefined) => void;
    initialOptionId?: number;
    isRequired?: boolean;
}

export function ProductSizes(props: IProps) {
    const { getValue, initialOptionId, isRequired } = props;

    const [data, setData] = useState<Size[]>([]);
    const [selectedOption, setSelectedOption] = useState<Size | null>(null);
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
        new ListSizes()
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

    useEffect(() => {
        getValue && getValue(selectedOption?.id);
    }, [selectedOption]);

    return (
        <Box display={"flex"} gap={2}>
            <Autocomplete
                value={selectedOption}
                placeholder={"Tamanho"}
                fullWidth
                disableClearable={false}
                onChange={(_, item) => setSelectedOption(item)}
                onInputChange={(e, text) => setSearchText(text)}
                isOptionEqualToValue={(option, value) =>
                    option.sizeName === value.sizeName
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
                //             {option.sizeName}
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
                    <TextField {...props} size={"small"} label={"Tamanho"} required={isRequired} />
                )}
                renderTags={() => null}
                getOptionLabel={(option) => option.sizeName}
                options={data}
                sx={{ mb: 0 }}
            />
            {/* <Button
                sx={{ width: "200px" }}
                variant="outlined"
                onClick={() => setIsOpen(true)}
            >
                Novo tamanho
            </Button> */}
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
        </Box>
    );
}
