import {
  AccountBoxOutlined,
  Announcement,
  AnnouncementOutlined,
} from "@mui/icons-material";
import { Button, CardMedia, Stack, Typography } from "@mui/material";
import { useEffect, useState } from "react";
import { OperationalCampaign } from "src/typings/models/operational-campaign.model";
import { ListOperationalCampaignAvailableUseCase } from "../use-cases/ListOperationalCampaignAvailable.use-case";
import { AssociateOperationalCampaignUseCase } from "../use-cases/AssociateOperationalCampaign.use-case";
import { toast } from "react-toastify";
import { useRouter } from "next/router";

export function AvailableCampaigns() {
  const [data, setData] = useState<OperationalCampaign[]>([]);
  const router = useRouter();

  const getCampaigns = async () => {
    await new ListOperationalCampaignAvailableUseCase()
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
        setData(data.OperationalCampaignAvailable);
      })
      .catch(() => {})
      .finally(() => {});
  };

  useEffect(() => {
    getCampaigns();
  }, []);

  const handleAssociate = async (id: number) => {
    await new AssociateOperationalCampaignUseCase()
      .handle({
        IDGDA_OPERATIONAL_CAMPAIGN: id,
      })
      .then((data) => {
        toast.success("Associação realizada com sucesso!");
      })
      .catch(() => {
        toast.error("Falha ao tentar associar");
      })
      .finally(() => {});
  };

  return (
    <Stack direction={"column"} gap={"30px"}>
      <Stack flexDirection={"row"} alignItems={"center"} gap={"10px"}>
        <AnnouncementOutlined sx={{ fontSize: "40px" }} />
        <Typography fontSize={"24px"} fontWeight={"600"}>
          Campanhas disponíveis
        </Typography>
      </Stack>
      <Stack flexDirection={"row"} flexWrap={"wrap"} gap={"10px"}>
        {data.length > 0 ? (
          data.map((item, index) => (
            <Stack
              key={index}
              width={"350px"}
              borderRadius={"16px"}
              overflow={"hidden"}
              border={"solid 1px #e1e1e1"}
              paddingBottom={"16px"}
              gap={"16px"}
            >
              <Stack
                width={"100%"}
                height={"250px"}
                bgcolor={"#f1f1f1"}
                onClick={() =>
                  router.push(
                    `/campaigns/campaign-details?id=${item?.IDGDA_OPERATIONAL_CAMPAIGN}`
                  )
                }
              >
                {" "}
                <CardMedia
                  component="img"
                  image={item.IMAGE}
                  sx={{
                    width: "449px",
                    objectFit: "cover",
                    height: "100%",
                    borderRadius: "24px",
                    backgroundColor: "#f1f1f1",
                  }}
                />
              </Stack>
              <Typography
                fontWeight={"600"}
                fontSize={"16px"}
                paddingX={"20px"}
              >
                {item.NAME}
              </Typography>
              <Stack flexDirection={"row"} paddingX={"20px"}>
                <Button
                  variant="contained"
                  sx={{ height: "45px", fontWeight: "600" }}
                  onClick={() =>
                    handleAssociate(item.IDGDA_OPERATIONAL_CAMPAIGN)
                  }
                >
                  Solicitar participação
                </Button>
              </Stack>
            </Stack>
          ))
        ) : (
          <Stack>
            <Typography>Nenhuma campanha disponível</Typography>
          </Stack>
        )}
      </Stack>
    </Stack>
  );
}
