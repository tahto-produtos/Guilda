import { Autocomplete, TextField } from "@mui/material";
import { useEffect, useState } from "react";
import { useDebounce } from "src/hooks";
import { ListCollaboratorsAllUseCase } from "src/modules/collaborators/use-cases/list-collaborators.use-case";
import { Collaborator } from "src/typings/models/collaborator.model";
import { PaginationModel } from "src/typings/models/pagination.model";

interface AutoCompleteCollaboratorsProps {
    getMultipleValue?: (input: { id: number }[]) => void;
    getValue?: (input: Collaborator | null) => void;
    multiple?: boolean;
}

export function AutoCompleteCollaborators(
    props: AutoCompleteCollaboratorsProps
) {
    const { getValue, multiple, getMultipleValue } = props;

    const [collaborator, setCollaborator] = useState<Collaborator | null>(null);
    const [multiCollaborator, setMultiCollaborator] = useState<
        {
            id: number;
        }[]
    >([]);
    const [collaboratorsSearchValue, setCollaboratorsSearchValue] =
        useState<string>("");
    const [collaborators, setCollaborators] = useState<Collaborator[]>([]);
    const debouncedCollaboratorsSearchTerm: string = useDebounce<string>(
        collaboratorsSearchValue,
        400
    );

    const getCollaboratorsList = async () => {
        const pagination: PaginationModel = {
            limit: 20,
            offset: 0,
            searchText: collaboratorsSearchValue,
        };

        new ListCollaboratorsAllUseCase()
            .handle(pagination)
            .then((data) => {
                setCollaborators(data.items);
            })
            .catch(() => {});
    };

    useEffect(() => {
        getCollaboratorsList();
    }, [debouncedCollaboratorsSearchTerm]);

    useEffect(() => {
        getMultipleValue && getMultipleValue(multiCollaborator);
        getValue && getValue(collaborator);
    }, [collaborator]);

    if (multiple) {
        return (
            <Autocomplete
                multiple
                size={"small"}
                options={collaborators}
                getOptionLabel={(option) => option.name}
                onChange={(event, value) => {
                    setMultiCollaborator(value as Collaborator[]);
                }}
                onInputChange={(e, text) => setCollaboratorsSearchValue(text)}
                filterOptions={(x) => x}
                filterSelectedOptions
                fullWidth
                renderInput={(params) => (
                    <TextField
                        {...params}
                        variant="outlined"
                        label="Selecione um ou mais colaboradores"
                        placeholder="Buscar"
                    />
                )}
                renderOption={(props, option) => {
                    return (
                        <li {...props} key={option.id}>
                            {option.name}
                        </li>
                    );
                }}
                isOptionEqualToValue={(option, value) =>
                    option.name === value.name
                }
            />
        );
    }

    return (
        <Autocomplete
            size={"small"}
            options={collaborators}
            disableClearable={false}
            getOptionLabel={(option) => option.name}
            onChange={(event, value) => {
                setCollaborator(value as Collaborator);
            }}
            onInputChange={(e, text) => setCollaboratorsSearchValue(text)}
            filterOptions={(x) => x}
            filterSelectedOptions
            fullWidth
            renderInput={(params) => (
                <TextField
                    {...params}
                    variant="outlined"
                    label="Selecione um colaborador"
                    placeholder="Buscar"
                />
            )}
            renderOption={(props, option) => {
                return (
                    <li {...props} key={option.id}>
                        {option.name}
                    </li>
                );
            }}
            isOptionEqualToValue={(option, value) => option.name === value.name}
        />
    );
}
