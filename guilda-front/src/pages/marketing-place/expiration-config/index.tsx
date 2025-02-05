import Settings from "@mui/icons-material/Settings";
import { LocalizationProvider, DatePicker } from "@mui/x-date-pickers";
import {
    Box,
    Button,
    FormControl,
    InputLabel,
    MenuItem,
    Select,
    Stack,
    TextField,
} from "@mui/material";
import { grey } from "@mui/material/colors";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { BaseModal } from "src/components/feedback";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { CreateHolidayUseCase } from "src/modules/marketing-place/use-cases/create-holiday.use-case";
import { DeleteHolidayUseCase } from "src/modules/marketing-place/use-cases/delete-holiday.use-case";
import { ListCityUseCase } from "src/modules/marketing-place/use-cases/list-city.use-case";
import { ListHolidayUseCase } from "src/modules/marketing-place/use-cases/list-holiday.use-case";
import { UpdateHolidayUseCase } from "src/modules/marketing-place/use-cases/update-holiday.use-case";
import { getLayout } from "src/utils";
import { format } from "date-fns";
import { LoadingButton } from "@mui/lab";
import { ListExpirationBasisUseCase } from "src/modules/expiration-config/list-expiration-basis.use-case";
import { EditExpirationBasisUseCase } from "src/modules/expiration-config/edit-expiration-basis.use-case";
import { UserInfoContext } from "src/contexts/user-context/user.context";

export default function ExpirationConfigView() {
    const { myUser } = useContext(UserInfoContext);
    const [homeValue, setHomeValue] = useState<string>("");
    const [presentValue, setPresentValue] = useState<string>("");
    const [isDataFetched, setIsDataFetched] = useState<boolean>(false);

    function listExpirations() {
        new ListExpirationBasisUseCase()
            .handle()
            .then((data) => {
                console.log(data);

                const home = data.find(
                    (item: any) => item.TYPE == "VENCIMENTO_LIBERADO_HOME"
                );
                const present = data.find(
                    (item: any) => item.TYPE == "VENCIMENTO_LIBERADO_PRESENCIAL"
                );

                home && setHomeValue(home.VALUE);
                present && setPresentValue(present.VALUE);

                setIsDataFetched(true);
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    }

    useEffect(() => {
        listExpirations();
    }, []);

    function editExpirations() {
        if (!myUser) return;

        new EditExpirationBasisUseCase()
            .handle({
                collaboratorId: myUser.id,
                home: homeValue,
                present: presentValue,
            })
            .then(() => {
                toast.success("Atualizado com sucesso!");
                listExpirations();
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            });
    }

    return (
        <Card width={"100%"} display={"flex"} flexDirection={"column"}>
            <PageHeader
                title={"Prazos de expiração"}
                headerIcon={<Settings />}
            />
            <Stack px={2} py={4} width={"100%"} gap={2}>
                <TextField
                    label="Home"
                    value={homeValue}
                    onChange={(e) => setHomeValue(e.target.value)}
                    type="number"
                />
                <TextField
                    label="Presencial"
                    value={presentValue}
                    type="number"
                    onChange={(e) => setPresentValue(e.target.value)}
                />
                <LoadingButton onClick={editExpirations} variant="contained">
                    Atualizar
                </LoadingButton>
            </Stack>
        </Card>
    );
}

ExpirationConfigView.getLayout = getLayout("private");
