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
import Link from "@mui/material/Link";
import { useRouter } from "next/router";
import { UserPersonaContext } from "src/contexts/user-persona/user-persona.context";
import { ChangePersonaButton } from "src/components/inputs/change-persona-button/change-persona-button";
import { LoadMyOperationalCampaignUseCase } from "src/modules/campaign/use-cases/LoadMyOperationalCampaign.use-case";
import { OperationalCampaign, OperationalCampaignDetails } from "src/typings/models/operational-campaign.model";
import { DetailsOperationalCampaignUseCase } from "src/modules/campaign/use-cases/DetailsOperationalCampaign.use-case";

export function HomeUserCard() {
    const { myUser } = useContext(UserInfoContext);
    const { persona, personaShowUser } = useContext(UserPersonaContext);
    const theme = useTheme();
    const router = useRouter();

    const [myOperationalCampaign, setMyOperationalCampaign] = useState<
        OperationalCampaign[]
    >([]);
    const [detailMyCampaing, setDetailMyCampaing] = useState<OperationalCampaignDetails | null>(null);

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

      const getDetailsMyCampaigns = async () => {
        if (myOperationalCampaign.length === 0) {
          return;
        }
        if (!myOperationalCampaign[0].IDGDA_OPERATIONAL_CAMPAIGN) {
          return;
        }

        await new DetailsOperationalCampaignUseCase()
          .handle({
            IDGDA_OPERATIONAL_CAMPAIGN: myOperationalCampaign[0].IDGDA_OPERATIONAL_CAMPAIGN,
            ISIMPORTANT: true,
          })
          .then((data) => {
            setDetailMyCampaing(data);
          })
          .catch(() => {})
          .finally(() => {});
      };
    
      useEffect(() => {
        getDetailsMyCampaigns();
      }, [myOperationalCampaign]);

    return (
        <ContentCard
            sx={{
                flexDirection: "row",
                height: "190px",
                position: "sticky",
                top: 144,
                zIndex: 9,
            }}
            justifyContent={"space-between"}
            alignItems={"center"}
        >
            <Stack
                direction={"row"}
                alignItems={"center"}
                gap="18px"
                width={"100%"}
            >
                {persona ? (
                    <ProfileImage
                        width="110px"
                        height="110px"
                        color={theme.palette.grey[200]}
                        image={persona.FOTO}
                    />
                ) : (
                    <Skeleton
                        variant="circular"
                        width={"110px"}
                        height={"110px"}
                    />
                )}
                <Stack direction={"column"}>
                    {persona ? (
                        <Typography variant="h2" fontSize={"16px"}>
                            {capitalizeText(persona.NOME_SOCIAL)}
                        </Typography>
                    ) : (
                        <Skeleton
                            variant="text"
                            width={"150px"}
                            height={"40px"}
                        />
                    )}
                    {personaShowUser ? (
                        <Typography variant="body1" mt={"8px"}>
                            {personaShowUser.CARGO}
                        </Typography>
                    ) : (
                        <Skeleton
                            variant="text"
                            width={"80px"}
                            height={"30px"}
                        />
                    )}
                    <Stack direction={"row"} alignItems={"center"} mt={"24px"}>
                        <Link
                            href="#"
                            underline="hover"
                            fontSize={"12px"}
                            color="inherit"
                            onClick={() =>
                                router.push(
                                    `/profile/view-profile/${personaShowUser?.ID_PERSON_ACCOUNT}`
                                )
                            }
                        >
                            Ver conta
                            <KeyboardArrowRight sx={{ fontSize: "12px" }} />
                        </Link>
                        <Stack ml={"20px"}>
                            <ChangePersonaButton />
                        </Stack>
                    </Stack>
                </Stack>
            </Stack>
            <Divider orientation="vertical" />
            <Stack width={"100%"} direction={"column"} gap={"24px"} pl={"40px"}>
                <Stack
                    direction={"row"}
                    alignItems={"center"}
                    justifyContent={"space-between"}
                    /* sx={{ cursor: "pointer" }}
                    onClick={() =>
                        router.push(
                            `/campaigns/campaign-details?id=${detailMyCampaing?.idCampaign}`
                        )
                    } */
                >
                    <Typography
                        variant="body1"
                        color="secondary"
                        fontWeight={"600"}
                        fontSize={"16px"}
                    >
                        {detailMyCampaing && detailMyCampaing.name}
                    </Typography>
                    {/* <KeyboardArrowRight color="secondary" /> */}
                </Stack>
                <Stack width={"100%"}>
                    <Stack
                        direction={"row"}
                        alignItems={"center"}
                        justifyContent={"space-between"}
                    >
                        <Stack
                            direction={"row"}
                            alignItems={"center"}
                            gap={"10px"}
                        >
                            <Typography
                                variant="body1"
                                fontWeight={"400"}
                                fontSize={"14px"}
                            >
                               {detailMyCampaing && `Pontos:`}
                            </Typography>
                            <Typography
                                variant="body1"
                                color="secondary"
                                fontWeight={"700"}
                                fontSize={"14px"}
                            >
                                {detailMyCampaing && detailMyCampaing.punctuation + "/" + detailMyCampaing.mission_Punctuation}
                            </Typography>
                        </Stack>
                        <Typography
                            variant="body1"
                            color="secondary"
                            fontWeight={"700"}
                            fontSize={"16px"}
                        >
                            {detailMyCampaing && detailMyCampaing.mission_Percent + `%`}
                        </Typography>
                    </Stack>
                    {detailMyCampaing && (
                    <LinearProgress
                        variant="determinate"
                        value={detailMyCampaing ? detailMyCampaing.mission_Percent : 0}
                        color="secondary"
                        sx={{
                            height: "24px",
                            backgroundColor: theme.palette.grey[300],
                            borderRadius: "4px",
                            mt: "10px",
                        }}
                    />)}
                </Stack>
            </Stack>
        </ContentCard>
    );
}
