import {
    Autocomplete,
    Box,
    Button,
    TextField,
    Typography,
} from "@mui/material";
import { useEffect, useState } from "react";
import { BaseModal } from "src/components/feedback";
import { toast } from "react-toastify";
import { ListColors } from "../use-cases/list-colors.use-case";
import { CreateColor } from "../use-cases/create-color.use-case";
import { DeleteColors } from "../use-cases/delete-color.use-case";

export interface Color {
    id: number;
    color: string;
    createdAt: string;
    deletedAt: string;
    createdByCollaboratorId: number;
}

interface IProps {
    getValue?: (input: number | undefined) => void;
    initialOptionId?: number;
    isRequired?: boolean;
}

export function ProductColors(props: IProps) {
    const { getValue, initialOptionId, isRequired } = props;

    const [data, setData] = useState<Color[]>([]);
    const [selectedOption, setSelectedOption] = useState<Color | null>(null);
    const [searchText, setSearchText] = useState<string>("");
    const [isOpen, setIsOpen] = useState<boolean>(false);
    const [newItem, setNewItem] = useState<string>("");
    const [firstFetch, setFirstFetch] = useState<boolean>(false);

    useEffect(() => {
        if (initialOptionId && firstFetch) {
            const x = data.find((item) => item.id == initialOptionId) || null;
            console.log("XXXX", data, x);
            setSelectedOption(x);
        }
    }, [initialOptionId, firstFetch]);

    const getList = async () => {
        new ListColors()
            .handle(searchText)
            .then((data) => {
                console.log("COLOR", data);
                setData(data);
                setFirstFetch(true);
            })
            .catch(() => {});
    };

    useEffect(() => {
        getList();
    }, []);

    async function handleCreate() {
        new CreateColor()
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
        new DeleteColors()
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
                placeholder={"Cor"}
                fullWidth
                disableClearable={false}
                onChange={(_, item) => setSelectedOption(item)}
                onInputChange={(e, text) => setSearchText(text)}
                isOptionEqualToValue={(option, value) =>
                    option.color === value.color
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
                //         <Typography width={"400px"}>{option.color}</Typography>
                //         <Button
                //             variant={"outlined"}
                //             onClick={() => handleDelete(option.id)}
                //         >
                //             Apagar
                //         </Button>
                //     </Box>
                // )}
                renderInput={(props) => (
                    <TextField {...props} size={"small"} label={"Cor"} required={isRequired} />
                )}
                renderTags={() => null}
                getOptionLabel={(option) => option.color}
                options={data}
                sx={{ mb: 0 }}
            />
            {/* <Button
                sx={{ width: "200px" }}
                variant="outlined"
                onClick={() => setIsOpen(true)}
            >
                Nova cor
            </Button> */}
            <BaseModal
                width={"540px"}
                open={isOpen}
                title={`Nova cor`}
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
                        label={"Digite a nova cor"}
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
