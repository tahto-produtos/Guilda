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
import { ListDetails } from "../use-cases/list-details.use-case";
import { CreateDetails } from "../use-cases/create-details.use-case";
import { DeleteDetails } from "../use-cases/delete-details.use-case";

export interface Detail {
    id: number;
    detail: string;
    createdAt: string;
    deletedAt: string;
    createdByCollaboratorId: number;
}

interface IProps {
    getValue?: (input: number | undefined) => void;
    initialOptionId?: number;
}

export function ProductDetails(props: IProps) {
    const { getValue, initialOptionId } = props;

    const [data, setData] = useState<Detail[]>([]);
    const [selectedOption, setSelectedOption] = useState<Detail | null>(null);
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
        new ListDetails()
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
        new CreateDetails()
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
        new DeleteDetails()
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
                placeholder={"Detalhe"}
                fullWidth
                disableClearable={false}
                onChange={(_, item) => setSelectedOption(item)}
                onInputChange={(e, text) => setSearchText(text)}
                isOptionEqualToValue={(option, value) =>
                    option.detail === value.detail
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
                //         <Typography width={"400px"}>{option.detail}</Typography>
                //         <Button
                //             variant={"outlined"}
                //             onClick={() => handleDelete(option.id)}
                //         >
                //             Apagar
                //         </Button>
                //     </Box>
                // )}
                renderInput={(props) => (
                    <TextField {...props} size={"small"} label={"Detalhe"} required={true} />
                )}
                renderTags={() => null}
                getOptionLabel={(option) => option.detail}
                options={data}
                sx={{ mb: 0 }}
            />
            {/* <Button
                sx={{ width: "200px" }}
                variant="outlined"
                onClick={() => setIsOpen(true)}
            >
                Novo detalhe
            </Button> */}
            <BaseModal
                width={"540px"}
                open={isOpen}
                title={`Novo detalhe`}
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
                        label={"Digite o novo detalhe"}
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
