import ChevronLeft from "@mui/icons-material/ChevronLeft";
import { Button } from "@mui/material";
import { grey } from "@mui/material/colors";
import { Box } from "@mui/system";
import { useRouter } from "next/router";

interface GoBackProps {
    pushTo: string;
}

export function GoBack({ pushTo }: GoBackProps) {
    const router = useRouter();

    const handleRedirect = () => router.push(pushTo);

    return (
        <Box
            sx={{
                width: "100%",
                marginBottom: "20px",
                borderBottom: "solid 1px",
                borderColor: grey[100],
                paddingBottom: "20px",
            }}
        >
            <Button
                startIcon={<ChevronLeft />}
                variant={"text"}
                onClick={handleRedirect}
                color={"primary"}
            >
                Voltar
            </Button>
        </Box>
    );
}
