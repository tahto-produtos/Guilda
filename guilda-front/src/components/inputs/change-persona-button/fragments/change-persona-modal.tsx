import Check from "@mui/icons-material/Check";
import {
  Autocomplete,
  Box,
  Button,
  CircularProgress,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { isAxiosError } from "axios";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { ProfileImage } from "src/components/data-display/profile-image/profile-image";
import { BaseModal } from "src/components/feedback";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { UserPersonaContext } from "src/contexts/user-persona/user-persona.context";
import { AuthVerify } from "src/modules/auth/use-cases/auth-verify/auth-verify.use-case";
import { EXCEPTION_CODES } from "src/typings";
import { capitalizeText } from "src/utils/capitalizeText";
import { ChangePersonaUseCase } from "./change-persona.use-case";
import Cookies from "js-cookie";
import { CreateAccountPersonaModal } from "src/modules/personas/fragments/create-account-persona/create-account-persona.modal";
import { AddPersonaUserAccountUseCase } from "src/modules/personas/use-cases/add-persona-user-account.use-case";
import { Collaborator } from "src/typings/models/collaborator.model";
import { PaginationModel } from "src/typings/models/pagination.model";
import { useDebounce } from "src/hooks";
import { ListCollaboratorsAllUseCase } from "src/modules/collaborators/use-cases/list-collaborators.use-case";
import { SearchAccountsUseCase } from "src/modules/personas/use-cases/search-accounts.use-case";
import abilityFor from "src/utils/ability-for";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";
interface IProps {
  isOpen: boolean;
  onClose: () => void;
}

export function ChangePersonaModal(props: IProps) {
  const { isOpen, onClose } = props;
  const { myUser } = useContext(UserInfoContext);
  const { loggedAccounts } = useContext(UserPersonaContext);
  const [isOpenNew, setIsOpenNew] = useState(false);
  const { myPermissions } = useContext(PermissionsContext);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const [collaborator, setCollaborator] = useState<{
    id: number;
    name: string;
    registry: string;
  }[]>([]);
  const [collaboratorsSearchValue, setCollaboratorsSearchValue] =
    useState<string>("");
  const [collaborators, setCollaborators] = useState<
    {
      id: number;
      name: string;
      registry: string;
    }[]
  >([]);
  const debouncedCollaboratorsSearchTerm: string = useDebounce<string>(
    collaboratorsSearchValue,
    400
  );
  const [selectedToLink, setSelectedToLink] = useState<number | null>(null);

  const getCollaboratorsList = async () => {
      const pagination = {
        limit: 20,
        page: 1,
        Collaborator: collaboratorsSearchValue,
      };

      await new SearchAccountsUseCase()
      .handle(pagination)
      .then((data) => {
        setCollaborators(data[0].account);
      })
      .catch(() => {
        toast.error("Falha ao carregar colaboradores.");
      });
  };

  useEffect(() => {
    getCollaboratorsList();
  }, [debouncedCollaboratorsSearchTerm]);

  function handleChangePersona(id: number) {
    if (!myUser) {
      return;
    }
    if (isSubmitting) return;
    setIsSubmitting(true);

    new ChangePersonaUseCase()
      .handle({ idPersona: id })
      .then((data) => {
        Cookies.set("persona", id.toString());
        setTimeout(() => {
          window.location.href = window.location.href;
        }, 100); // 100ms de atraso
      })
      .catch((e) => {
        console.log(e);
      })
      .finally(() => {
        setIsSubmitting(false);
      });
  }

  function handleLinkAccount(id: number) {
    if (!myUser) {
      return;
    }

    new AddPersonaUserAccountUseCase()
      .handle({ IDPERSONAUSER: id, ids: collaborator.map((ite) => ite.id) })
      .then((data) => {
        toast.success("Colaboradores vículados com sucesso!");
      })
      .catch((e) => {
        console.log(e);
      })
      .finally(() => {});
  }

  return (
    <BaseModal
      width={"540px"}
      open={isOpen}
      title={`Escolha uma persona para acessar`}
      onClose={onClose}
    >
      <Box
        width={"100%"}
        display={"flex"}
        flexDirection={"column"}
        gap={"30px"}
      >
        {loggedAccounts.map((item, index) => (
          <Stack
            key={index}
            direction={"row"}
            alignItems={"center"}
            justifyContent={"space-between"}
          >
            <Stack direction={"row"} alignItems={"center"} gap={"16px"}>
              <ProfileImage
                name={item.NOME}
                image={item.FOTO}
                width="50px"
                height="50px"
              />
              <Stack gap={"8px"}>
                <Typography variant="body1" fontWeight={"600"}>
                  {capitalizeText(item.NOME)}
                </Typography>

                <Typography variant="body1">
                  {capitalizeText(item.TIPO) == "Personal"? "Pessoal"
                                                : "Comercial"}

                </Typography>
              </Stack>
            </Stack>
            <Stack direction={"row"} gap={"10px"}>
              <Button
                variant="outlined"
                onClick={() => setSelectedToLink(item.IDGDA_PERSONA_USER)}
              >
                Vincular
              </Button>
              <Button
                variant="contained"
                onClick={() => handleChangePersona(item.IDGDA_PERSONA_USER)}
                disabled={isSubmitting}
              >
                Acessar
              </Button>
            </Stack>
          </Stack>
        ))}
        <Stack direction={"row"} justifyContent={"flex-end"} mt={"20px"}>
        {abilityFor(myPermissions).can("Criar conta comercial", "Persona") && (<Button
            variant="contained"
            color="primary"
            onClick={() => setIsOpenNew(true)}
          >
            Criar nova conta
          </Button>
          )}
        </Stack>
        {isOpenNew && (
          <CreateAccountPersonaModal
            isOpen={isOpenNew}
            onClose={() => setIsOpenNew(false)}
          />
        )}
      </Box>
      {selectedToLink && (
        <BaseModal
          width={"540px"}
          open={Boolean(selectedToLink)}
          title={`Vincular colaboradores`}
          onClose={() => setSelectedToLink(null)}
        >
          <Box width={"100%"} display={"flex"} flexDirection={"column"} gap={1}>
            <Autocomplete
              value={collaborator}
              multiple
              placeholder={"Colaborador"}
              disableClearable={false}
              onChange={(e, value) => {
                setCollaborator(value);
              }}
              onInputChange={(e, text) => setCollaboratorsSearchValue(text)}
              filterOptions={(options, { inputValue }) =>
                options.filter(
                  (item) =>
                    item.id.toString().includes(inputValue.toString()) ||
                    item.name
                      .toString()
                      .toLowerCase()
                      .includes(inputValue.toString().toLowerCase())
                )
              }
              sx={{
                mb: 0,
                width: "100%",
              }}
              renderInput={(props) => (
                <TextField
                  {...props}
                  size={"medium"}
                  label={"Colaborador"}
                  InputProps={{
                    ...props.InputProps,
                  }}
                />
              )}
              isOptionEqualToValue={(option, value) =>
                option.name === value.name
              }
              getOptionLabel={(option) => `${option.name}`}
              options={collaborators}
            />
            <Button
              variant="contained"
              onClick={() => handleLinkAccount(selectedToLink)}
            >
              Vincular
            </Button>
          </Box>
        </BaseModal>
      )}
    </BaseModal>
  );
}
