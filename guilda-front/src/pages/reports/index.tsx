import FileOpen from "@mui/icons-material/FileOpen";
import Report from "@mui/icons-material/Report";
import { Box, Stack } from "@mui/material";
import { Card, PageHeader } from "src/components";
import { getLayout } from "src/utils";

export default function Reports() {
    return (
        <Box display="flex" flexDirection={"column"} width={"100%"}>
            <Card width={"100%"} display={"flex"} flexDirection={"column"}>
                <PageHeader title={"Relatórios"} headerIcon={<FileOpen />} />
                <Stack px={2} py={4} width={"100%"} gap={2}></Stack>
            </Card>
        </Box>
    );
}

Reports.getLayout = getLayout("private");
