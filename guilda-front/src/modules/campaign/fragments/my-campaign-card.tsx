import {
  Button,
  CardMedia,
  LinearProgress,
  Stack,
  Typography,
  alpha,
  useTheme,
} from "@mui/material";
import { DetailsOperationalCampaignUseCase } from "../use-cases/DetailsOperationalCampaign.use-case";
import { useEffect, useState } from "react";
import { OperationalCampaignDetails } from "src/typings/models/operational-campaign.model";
import { format } from "date-fns";
import { capitalizeText } from "src/utils/capitalizeText";
import {
  AccountBox,
  AccountBoxOutlined,
  ChevronRight,
  StarBorder,
} from "@mui/icons-material";
import { useRouter } from "next/router";

interface IProps {
  campaignId: number;
  isImportant: boolean | null;
  refresh: boolean;
  forceRefresh: () => void;
}

export function MyCampaignCard(props: IProps) {
  const theme = useTheme();
  const [data, setData] = useState<OperationalCampaignDetails | null>(null);
  const router = useRouter();

  const getMyCampaigns = async () => {
    await new DetailsOperationalCampaignUseCase()
      .handle({
        IDGDA_OPERATIONAL_CAMPAIGN: props.campaignId,
        ISIMPORTANT: props.isImportant,
      })
      .then((data) => {
        setData(data);
      })
      .catch(() => {})
      .finally(() => {});
  };

  useEffect(() => {
    getMyCampaigns();
  }, []);

  useEffect(() => {
    getMyCampaigns();
  }, [props.refresh]);
  return (
    <Stack direction={"column"} gap={"30px"}>
      <Stack flexDirection={"row"} alignItems={"center"} gap={"10px"}>
        <AccountBoxOutlined sx={{ fontSize: "40px" }} />
        <Typography fontSize={"24px"} fontWeight={"600"}>
          Campanha que estou participando
        </Typography>
      </Stack>
      <Stack
        width={"100%"}
        borderRadius={"16px"}
        border={`solid 1px #e1e1e1`}
        flexDirection={"row"}
        overflow={"hidden"}
      >
        <Stack minWidth={"400px"} minHeight={"340px"} bgcolor={"#f1f1f1"}>
          {" "}
          <CardMedia
            component="img"
            image={data?.image}
            sx={{
              width: "100%",
              objectFit: "cover",
              height: "100%",
              borderRadius: "24px",
              backgroundColor: "#f1f1f1",
            }}
          />
        </Stack>
        <Stack p={"24px"} direction={"column"} gap={"16px"} width={"100%"}>
          <Stack
            direction={"row"}
            width={"100%"}
            justifyContent={"space-between"}
          >
            <Stack gap={"26px"} paddingRight={"70px"}>
              <Typography fontSize={"24px"} fontWeight={"500"}>
                {data?.name}
              </Typography>
              <LinearProgress
                value={data?.mission_Punctuation}
                variant="determinate"
                sx={{
                  minHeight: "16px",
                  backgroundColor: "#E5E5E5",
                }}
              />
            </Stack>
            <Stack gap={"0px"}>
              <Typography fontSize={"13px"} fontWeight={"400"}>
                Data de inicio
              </Typography>
              <Typography fontSize={"16px"} fontWeight={"600"}>
                {data?.dtInicio &&
                  format(new Date(data.dtInicio), "dd/MM/yyyy")}
              </Typography>
              <Typography fontSize={"13px"} mt={"10px"} fontWeight={"400"}>
                Data de finalização
              </Typography>
              <Typography fontSize={"16px"} fontWeight={"600"}>
                {data?.dtFim && format(new Date(data.dtFim), "dd/MM/yyyy")}
              </Typography>
            </Stack>
          </Stack>
          <Stack gap={"16px"}>
            <Stack direction={"row"} gap={"10px"} alignItems={"center"}>
              <Typography>Pontos</Typography>
              <Typography fontSize={"20px"} fontWeight={"600"}>
                {data?.punctuation}
              </Typography>
            </Stack>
            <Stack direction={"row"} gap={"10px"} alignItems={"center"}>
              <Typography>Posição no ranking</Typography>
              <Typography fontSize={"20px"} fontWeight={"600"}>
                {data?.position}
              </Typography>
            </Stack>
            <Stack direction={"column"} gap={"10px"}>
              <Typography>Última missão cumprida</Typography>
              <Stack
                direction={"row"}
                justifyContent={"space-between"}
                alignItems={"center"}
                paddingX={"16px"}
                paddingY={"10px"}
                bgcolor={alpha(theme.palette.primary.main, 0.3)}
                borderRadius={"8px"}
              >
                <Stack flexDirection={"row"} gap={"15px"} alignItems={"center"}>
                  <Stack
                    width={"40px"}
                    height={"40px"}
                    justifyContent={"center"}
                    alignItems={"center"}
                    bgcolor={theme.palette.primary.main}
                    borderRadius={"8px"}
                  >
                    <StarBorder
                      sx={{ color: theme.palette.background.default }}
                    />
                  </Stack>
                  <Typography>{data?.mission_Concluded}</Typography>
                </Stack>
                <Typography fontSize={"14px"} fontWeight={"700"}>
                  {capitalizeText(data?.mission_Status || "")}
                </Typography>
              </Stack>
            </Stack>
            <Stack flexDirection={"row"} justifyContent={"flex-end"}>
              <Button
                variant="text"
                endIcon={<ChevronRight color="primary" />}
                color="primary"
                sx={{ color: theme.palette.primary.main, fontWeight: "700" }}
                onClick={() =>
                  router.push(
                    `/campaigns/campaign-details?id=${data?.idCampaign}`
                  )
                }
              >
                Ver mais informações
              </Button>
            </Stack>
          </Stack>
        </Stack>
      </Stack>

      {/* ---- */}

      {/* ---- */}
    </Stack>
  );
}
