import { useContext } from "react";
import { Stack } from "@mui/material";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { PostCreate } from "src/components/data-display/post-create/post-create";
import abilityFor from "../../../../utils/ability-for";
import { PermissionsContext } from "../../../../contexts/permissions-provider/permissions.context";

export function HomePostCreate() {
    const { myPermissions } = useContext(PermissionsContext);

    if (abilityFor(myPermissions).cannot("Criar Publicação", "Persona")) {
        return null;
    }

    return (
        <ContentCard sx={{ flexDirection: "column", gap: "28px" }}>
            <Stack gap={"24px"}>
                <PostCreate  />
            </Stack>
        </ContentCard>
    );
}
