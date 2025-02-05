import { CircularProgress, Typography } from "@mui/material";
import { grey } from "@mui/material/colors";
import { Box } from "@mui/system";

interface LoadingAppProps {
    error?: boolean;
}

export default function LoadingApp(props: LoadingAppProps) {
    const { error } = props;

    return (
        <Box
            sx={{
                width: "100vw",
                height: "100vh",
                display: "flex",
                justifyContent: "center",
                alignItems: "center",
                gap: 2,
            }}
        >
            <CircularProgress size={20} />
            <Typography variant="body2" sx={{ color: grey[500] }}>
                Carregando informações do usuário...
            </Typography>
        </Box>
    );
}
