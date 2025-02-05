import Block from "@mui/icons-material/Block";
import { Typography } from "@mui/material";
import { grey } from "@mui/material/colors";
import { Card } from "src/components/surfaces";

export default function WithoutPermissionCard() {
    return (
        <Card
            py={20}
            display={"flex"}
            justifyContent={"center"}
            alignItems={"center"}
            flexDirection={"column"}
            gap={2}
            color={grey[500]}
        >
            <Block fontSize={"large"} />
            <Typography color={grey[500]}>
                Você não tem permissão para ver este conteúdo
            </Typography>
        </Card>
    );
}
