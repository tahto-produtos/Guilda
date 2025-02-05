import InsertDriveFile from "@mui/icons-material/InsertDriveFile";
import { useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Card, PageHeader } from "src/components";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";
import { useLoadingState } from "src/hooks";
import { DateUtils, SheetBuilder, getLayout } from "src/utils";
import abilityFor from "src/utils/ability-for";
import { PermissionsContext } from "src/contexts/permissions-provider/permissions.context";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { ListMonetizationExpireUseCase } from "src/modules/monetization/use-cases/list-monetization-expire.use-case";
import { ListMonetizationExpiredTable } from "src/modules/monetization/tables/list-monetization-expired";

export default function ConfigMonetizationExpiration() {
    const { myPermissions } = useContext(PermissionsContext);
    const [data, setData] = useState<any>(null);
    const { finishLoading, isLoading, startLoading } = useLoadingState();
    const [limit, setLimit] = useState<number>(100);
    const { myUser } = useContext(UserInfoContext);

    function getMonetizationExport() {
        if (!myUser) return;

        startLoading();

        new ListMonetizationExpireUseCase()
            .handle()
            .then((data) => {
                setData({
                    items: data.returnListExpired,
                    totalItems: data.returnListExpired.length,
                });
            })
            .catch(() => {
                toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
            })
            .finally(() => {
                finishLoading();
            });
    }
    useEffect(() => {
        getMonetizationExport();
    }, []);

    function formatResultDate(dateString: string) {
        if (dateString) {
            const date = dateString.split("T")[0];
            const dateSplited = date.split("-");
            return `${dateSplited[2]}/${dateSplited[1]}/${dateSplited[0]}`;
        } else {
            return "";
        }
    }

    return (
        <Card
            width={"100%"}
            display={"flex"}
            flexDirection={"column"}
            justifyContent={"space-between"}
        >
            <PageHeader
                title={`Extrato a Expirar`}
                headerIcon={<InsertDriveFile />}
            />
            
            <ListMonetizationExpiredTable
                tableData={data || { items: [], totalItems: 0 }}
                isLoading={isLoading}
            />
        </Card>
    );
}

ConfigMonetizationExpiration.getLayout = getLayout("private");
