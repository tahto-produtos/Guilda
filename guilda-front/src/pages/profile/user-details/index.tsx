import { Stack } from "@mui/system";
import { Card, PageHeader } from "src/components";
import { getLayout } from "src/utils";
import defineAbilityFor from "../../../permissions/definitions/definitions.ability";
import { useContext } from "react";
import { Can } from "@casl/react";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";

export default function UserDetails() {
    const { myPermissions } = useContext(PermissionsContext);

    return (
        <Card
            width={"100%"}
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"space-between"}
        >
            {/* <Stack px={2} py={3} width={"100%"}>
                <PageHeader title={`Detalhes do usuário`} />
                <Can
                    I="CREATE"
                    a="metric-settings"
                    ability={defineAbilityFor(myPermissions)}
                >
                    {() => (
                        <button onClick={() => console.log("posso")}>
                            METRIC SETTINGS LIBERADO
                        </button>
                    )}
                </Can>
            </Stack> */}
        </Card>
    );
}

UserDetails.getLayout = getLayout("private");
