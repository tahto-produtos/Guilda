import { Typography } from "@mui/material";
import { Box } from "@mui/system";

export function NoResultsOverlay (){
    return(
        <Box sx={{
            width: '100%',
            height: '100%',
            backgroundColor: '#fff',
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center',
        }}>
            <Typography variant="body2" color={'#999'}>Resultados não encontrados</Typography>
        </Box>
    )   
}