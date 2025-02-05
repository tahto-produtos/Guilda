import Star from "@mui/icons-material/Star";
import KeyboardArrowRight from "@mui/icons-material/KeyboardArrowRight";
import {
    Divider,
    LinearProgress,
    Skeleton,
    Stack,
    Typography,
    useTheme,
} from "@mui/material";
import { useContext, useEffect, useState } from "react";
import { ProfileImage } from "src/components/data-display/profile-image/profile-image";
import { ContentCard } from "src/components/surfaces/content-card/content-card";
import { UserInfoContext } from "src/contexts/user-context/user.context";
import { capitalizeText } from "src/utils/capitalizeText";
import { format, isValid } from "date-fns";
import MonetizationOnOutlined from "@mui/icons-material/MonetizationOnOutlined";
import { UserPersonaContext } from "src/contexts/user-persona/user-persona.context";
import { OperationalCampaign, OperationalCampaignDetails } from "src/typings/models/operational-campaign.model";
import { LoadMyOperationalCampaignUseCase } from "src/modules/campaign/use-cases/LoadMyOperationalCampaign.use-case";
import { DetailsOperationalCampaignUseCase } from "src/modules/campaign/use-cases/DetailsOperationalCampaign.use-case";
import { formatCurrency } from "src/utils/format-currency";
import { ListTransactions } from "src/modules/monetization/use-cases/list-transactions.use-case";
import { TableDataModel } from "src/typings";
import { toast } from "react-toastify";
import { INTERNAL_SERVER_ERROR_MESSAGE } from "src/constants";

export function HomeMonetizationCard() {
    const { myUser } = useContext(UserInfoContext);
    const { persona } = useContext(UserPersonaContext);
    const theme = useTheme();
    const [tableData, setTableData] = useState<TableDataModel | null>(null);
    const [myOperationalCampaign, setMyOperationalCampaign] = useState<
        OperationalCampaign[]
    >([]);
    const [detailMyCampaing, setDetailMyCampaing] = useState<OperationalCampaignDetails | null>(null);
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const getMyCampaigns = async () => {
        await new LoadMyOperationalCampaignUseCase()
          .handle({
            STARTEDATFROM: "",
            STARTEDATTO: "",
            ENDEDATFROM: "",
            ENDEDATTO: "",
            NAME: "",
            limit: 5,
            page: 1,
          })
          .then((data) => {
            setMyOperationalCampaign(data.MyOperationalCampaign);
          })
          .catch(() => {})
          .finally(() => {
            
          });
      };
    
      useEffect(() => {
        getMyCampaigns();
      }, []);


      const GetTransactions = async () => {
        if (!myUser) return;

        const endDatePicker = new Date(); // Data atual
        const startDatePicker = new Date(); // Data de 30 dias atrás
        startDatePicker.setDate(endDatePicker.getDate() - 30);

            const customPayload = {
                userId: myUser.id,
                dateMin: format(
                    new Date(startDatePicker.toString()),
                    "yyyy-MM-dd"
                ),
                dateMax: format(
                    new Date(endDatePicker.toString()),
                    "yyyy-MM-dd"
                ),
                filter: "collaborator",
                value: myUser.id.toString(),
                limit: 10,
                offset: 0,
            };

            new ListTransactions()
                .handle(customPayload)
                .then((data) => {
                    setTableData({
                        items: data.checkingAccount,
                        totalItems: data.totalItems,
                    });
                })
                .catch(() => {
                    toast.error(INTERNAL_SERVER_ERROR_MESSAGE);
                })
                .finally(() => {
                    setIsLoading(false);
                });
        
    };

    useEffect(() => {
        myUser && GetTransactions();
    }, [myUser]);


/*       const getDetailsMyCampaigns = async () => {
/*         if (myOperationalCampaign.length === 0) {
          return;
        }
        if (!myOperationalCampaign[0].IDGDA_OPERATIONAL_CAMPAIGN) {
          return;
        } *

        await new DetailsOperationalCampaignUseCase()
          .handle({
            IDGDA_OPERATIONAL_CAMPAIGN: myOperationalCampaign[0]?.IDGDA_OPERATIONAL_CAMPAIGN || 0,
            ISIMPORTANT: true,
          })
          .then((data) => {
            setDetailMyCampaing(data);
          })
          .catch(() => {})
          .finally(() => {});
      }; */
    
/*       useEffect(() => {
        getDetailsMyCampaigns();
      }, [myOperationalCampaign]); */


    return (
        <ContentCard sx={{ height: "190px" }}>
            <Stack direction={"column"}>
                <Stack direction={"row"} gap={"16px"} width={"100%"}>
                    <ProfileImage
                        width="50px"
                        height="50px"
                        color={theme.palette.grey[200]}
                        image={persona?.FOTO || undefined}
                    />
                    <Stack
                        justifyContent={"flex-end"}
                        gap={"20px"}
                        width={"100%"}
                    >
                        {myUser ? (
                            <Typography variant="h2" fontSize={"14px"}>
                                {capitalizeText(myUser.name)}
                            </Typography>
                        ) : (
                            <Skeleton width={"120px"} />
                        )}
                        <Stack
                            direction={"row"}
                            width={"100%"}
                            justifyContent={"space-between"}
                            gap={"10px"}
                            alignItems={"center"}
                        >
                            {/* <Typography variant="body1">Nível: 1</Typography> */}
                            <Stack
                                direction={"row"}
                                gap={"7px"}
                                alignItems={"center"}
                            >
{/*                                 <Star
                                    color="secondary"
                                    fontSize="small"
                                    sx={{ mt: "-1px" }}
                                />
                                <Typography variant="h3" lineHeight={"20px"}>
                                    {detailMyCampaing && detailMyCampaing.punctuation + "/" + detailMyCampaing.mission_Punctuation}
                                </Typography> */}
                            </Stack>
                        </Stack>
                    </Stack>
                </Stack>
                {/* <LinearProgress
                    variant="determinate"
                    value={detailMyCampaing ? detailMyCampaing.mission_Percent : 0}
                    color="secondary"
                    sx={{
                        height: "24px",
                        backgroundColor: theme.palette.grey[200],
                        borderRadius: "4px",
                        mt: "10px",
                    }}
                /> */}
                <Stack
                    direction={"row"}
                    alignItems={"center"}
                    justifyContent={"space-between"}
                    mt={"25px"}
                >
                    <Stack direction={"row"} gap={"15px"} alignItems={"center"}>
                        <MonetizationOnOutlined color="secondary" />
                        <Typography variant="h3">{tableData?.items[0]?.balance ? formatCurrency(tableData?.items[0]?.balance) : ""}</Typography>
                    </Stack>  
{/*                     <Stack direction={"row"} alignItems={"center"}>
                         <Typography
                            variant="body1"
                            fontSize={"14px"}
                            fontWeight={"700"}
                            lineHeight={"10px"}
                            color="secondary"
                        >
                            Saber mais
                        </Typography> 
                        <KeyboardArrowRight
                            sx={{
                                fontSize: "20px",
                                color: theme.palette.grey[600],
                            }}
                        />
                    </Stack> */}
                </Stack>
            </Stack>
        </ContentCard>
    );
}
