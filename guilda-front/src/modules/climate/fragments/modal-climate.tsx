import { CardMedia, Stack, Typography, useTheme } from "@mui/material";
import { useEffect, useState } from "react";
import { Climate, ClimateReason } from "src/typings/models/climate.model";
import { ListClimateReasonsUseCase } from "../use-case/list-climate-reason.use-case";
import { DoClimateUseCase } from "../use-case/do-climate.use-case";

interface ModalClimateProps {
  climates: Climate[];
  refresh: () => void;
}

export function ModalClimate(props: ModalClimateProps) {
  const { climates, refresh } = props;
  const [selectedClimate, setSelectedClimate] = useState<Climate | null>(null);
  const [isOptionselectedClimate, setIsOptionselectedClimate] = useState(false);


  const [selectedReason, setSelectedReason] = useState<ClimateReason | null>(
    null
  );
  const [isOptionselectedReason, setIsOptionselectedReason] = useState(false);
  const [reasons, setReasons] = useState<ClimateReason[]>([]);
  const [showReasons, setShowReasons] = useState<boolean>(false);

  const theme = useTheme();

  async function listClimateReasons() {
    if (!selectedClimate) return;

    await new ListClimateReasonsUseCase()
      .handle({
        id: selectedClimate.idClimate,
      })
      .then((data) => {
        if (data.response) {
          setReasons(data.reasons);
        } else {
          doClimate();
        }
      })
      .catch(() => { })
      .finally(() => { });
  }
  useEffect(() => {
    console.log("reasons", reasons);
    if (reasons.length > 0) setShowReasons(true);
    else setShowReasons(false);
  }, [reasons]);

  useEffect(() => {
    selectedClimate && listClimateReasons();
  }, [selectedClimate]);

  const handleClimateClick = (climate: any) => {
    console.log(isOptionselectedClimate);
    if (!isOptionselectedClimate) {
      setSelectedClimate(climate);
      setIsOptionselectedClimate(true);
    }
  };


  async function doClimate(idReason?: number) {
    if (!selectedClimate) return;

    await new DoClimateUseCase()
      .handle({
        idClimate: selectedClimate.idClimate,
        idClimateReason: idReason || 0,
      })
      .then((data) => {
        if (data.response) {
          setReasons(data.reasons);
        }
      })
      .catch(() => { })
      .finally(() => {
        refresh();
      });
  }

  return (
    <Stack
      width={"100%"}
      height={"100vh"}
      sx={{
        background:
          "linear-gradient(90deg, rgba(91, 68, 146, 1) 0%, rgba(48, 166, 152, 1) 100%)",
      }}
      position={"fixed"}
      top={0}
      left={0}
      zIndex={90}
      justifyContent={"center"}
      alignItems={"center"}
    >
      <Stack
        width={"100%"}
        minHeight={"50px"}
        bgcolor={"#fff"}
        py={"64px"}
        justifyContent={"center"}
        alignItems={"center"}
        gap={"24px"}
      >
        <Typography
          color="#FF6600"
          variant="h3"
          fontFamily={"Montserrat"}
          fontSize={"36px"}
          fontWeight={"500"}
        >
          {selectedClimate && reasons
            ? "Qual o motivo?"
            : "Como você está hoje?"}
        </Typography>
        <Typography
          color="#404040"
          variant="h3"
          fontFamily={"Montserrat"}
          fontSize={"20px"}
          fontWeight={"300"}
          maxWidth={"630px"}
          textAlign={"center"}
        >
          {showReasons
            ? `Conta pra gente o motivo de ter escolhido a opção: ${selectedClimate ? selectedClimate.climate : ""}`
            : "Conta pra gente como você esta hoje que podemos ajudar você caso algo não esteja indo como esperado"}
        </Typography>
        <Stack direction={"row"} gap={"100px"} mt={"59px"}>
          {showReasons
            ? reasons.map((reason, index) => (
              <Stack
                key={index}
                alignItems={"center"}
                gap={"35px"}
                sx={{ cursor: "pointer" }}
                onClick={() => doClimate(reason.idClimateReason)}
              >
                <CardMedia
                  component="img"
                  image={reason.image}
                  sx={{
                    width: "200px",
                    objectFit: "contain",
                  }}
                />
                <Typography
                  fontFamily={"Montserrat"}
                  fontSize={"18px"}
                  textAlign={"center"}
                  maxWidth={"200px"}
                >
                  {reason.reason}
                </Typography>
              </Stack>
            ))
            : climates.map((climate, index) => (
              <Stack
                key={index}
                alignItems={"center"}
                gap={"35px"}
                sx={{ cursor: "pointer" }}
                onClick={() => handleClimateClick(climate)}
              >
                <CardMedia
                  component="img"
                  image={climate.image}
                  sx={{
                    width: "200px",
                    objectFit: "contain",
                  }}
                />
                <Typography
                  fontFamily={"Montserrat"}
                  fontSize={"18px"}
                  textAlign={"center"}
                  maxWidth={"200px"}
                >
                  {climate.climate}
                </Typography>
              </Stack>
            ))}
        </Stack>
      </Stack>
    </Stack>
  );
}
