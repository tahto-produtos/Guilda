import ChangeCircle from "@mui/icons-material/ChangeCircle";
import { Avatar, Box, Button, Typography } from "@mui/material";
import { blue } from "@mui/material/colors";
import { Stack } from "@mui/system";
import { Card, PageHeader } from "src/components";
import { ListBalanceTable } from "src/modules/monetization";
import { getLayout } from "src/utils";

const mock_transactions = {
    items: [
        {
            id: 0,
            input: 100.3,
            output: 0,
            balance: 100.3,
            createdAt: "03-02-2022",
        },
        {
            id: 1,
            input: 0,
            output: 30.1,
            balance: 70.2,
            createdAt: "04-02-2022",
        },
        {
            id: 2,
            input: 300,
            output: 0,
            balance: 370.2,
            createdAt: "05-02-2022",
        },
    ],
    totalItems: 10,
};

export default function Settings() {
    return (
        <Card
            width={"100%"}
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"space-between"}
        >
            <Stack px={2} py={3} width={"100%"}>
                <PageHeader title={`Configurações de monetização`} />
            </Stack>
        </Card>
    );
}

Settings.getLayout = getLayout("private");
