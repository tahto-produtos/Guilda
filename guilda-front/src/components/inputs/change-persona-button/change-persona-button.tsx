import { Button } from "@mui/material";
import { useContext, useState } from "react";
import { ChangePersonaModal } from "./fragments/change-persona-modal";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import abilityFor from "src/utils/ability-for";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";
export function ChangePersonaButton() {
  const [isOpen, setIsOpen] = useState<boolean>(false);
  const { myUser } = useContext(UserInfoContext);
  const { myPermissions } = useContext(PermissionsContext);

  return (
    <>
{/*     {myUser?.isAdmin && abilityFor(myPermissions).can("Ver Indicadores", "Indicadores") && (
        <Button
          variant="contained"
          size="small"
          onClick={() => setIsOpen(true)}
          
        >
          Trocar persona
        </Button>
      )} */}
      {abilityFor(myPermissions).can("Trocar persona", "Persona") && (
        <Button
          variant="contained"
          size="small"
          onClick={() => setIsOpen(true)}
          
        >
          Trocar persona
        </Button>
      )}
      {isOpen && (
        <ChangePersonaModal isOpen={isOpen} onClose={() => setIsOpen(false)} />
      )}
    </>
  );
}
