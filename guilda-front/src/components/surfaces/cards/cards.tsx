import Styled from "@emotion/styled";
import { Box } from "@mui/material";
import { grey } from "@mui/material/colors";

export const Card = Styled(Box)`
    border: 1px solid;
    border-color: ${grey["100"]};
    background-color: white;
    border-radius: 0.3rem;
`;
