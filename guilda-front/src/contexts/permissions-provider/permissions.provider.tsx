import { ReactNode, useContext, useEffect, useState } from "react";
import { PermissionsContext } from "./permissions.context";
import { UserInfoContext } from "../user-context/user.context";
import Cookies from "js-cookie";
import { PermissionUseCase } from "src/modules/permissions/Permission.use-case";
import { PermissionModel } from "src/typings/models/permission.model";
import { toast } from "react-toastify";

interface IProviderProps {
    children: ReactNode;
}

export function PermissionsProvider({ children }: IProviderProps) {
    const { myUser } = useContext(UserInfoContext);
    const [myPermissions, setMyPermissions] = useState<number[]>([]);

    async function getMyPermissions() {
        const isAdmViewActive = Cookies.get("admViewActive") == "true";

        if (isAdmViewActive) {
            return;
        }

        if (!myUser?.id) {
            return;
        }

        await new PermissionUseCase()
            .handle({ codCollaborator: myUser.id })
            .then((data) => {
                const permissionsArrFiltered = data.filter(
                    (item: { id: number; active: boolean }) =>
                        item.active == true
                );

                const permissionsArr = permissionsArrFiltered.map(
                    (permission: PermissionModel) => {
                        return permission.id;
                    }
                );

                setMyPermissions(permissionsArr);
            })
            .catch(() => {
                toast.error("Erro ao carregar suas permissões");
            })
            .finally(() => {});
    }

    useEffect(() => {
        myUser?.id && getMyPermissions();
    }, [myUser]);

    return (
        <PermissionsContext.Provider
            value={{ myPermissions, setMyPermissions }}
        >
            {children}
        </PermissionsContext.Provider>
    );
}
