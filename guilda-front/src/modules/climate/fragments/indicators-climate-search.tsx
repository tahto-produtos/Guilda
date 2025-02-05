import { Stack, Typography, useTheme } from "@mui/material";
import { Gauge, gaugeClasses } from "@mui/x-charts";
import { ListGroupReasonResponse } from "../use-case/list-group-hierarchy-climate";

export function IndicatorsClimateSearch({
  results,
}: {
  results: ListGroupReasonResponse[];
}) {
  return (
    <Stack direction={"column"} gap={"32px"}>
      <Typography fontWeight={"600"} fontSize={"24px"}>
        Indicadores da pesquisa de clima
      </Typography>
      <Stack direction={"row"} gap={"55px"}>
        {results.map((i, index) => (
          <CardIndicator key={index} data={i} />
        ))}
      </Stack>
    </Stack>
  );
}

function CardIndicator({ data }: { data: ListGroupReasonResponse }) {
  const theme = useTheme();

  return (
    <Stack
      width={"100%"}
      height={"300px"}
      border={`solid 1px #7D7D7D`}
      borderRadius={"16px"}
      justifyContent={"center"}
      alignItems={"center"}
    >
      <Typography fontWeight={"700"} fontSize={"14px"}>
        {data.climate}
      </Typography>
      <Stack
        px={"20px"}
        width={"100%"}
        direction={"row"}
        height={"70%"}
        position={"relative"}
        justifyContent={"center"}
        alignItems={"center"}
      >
        <Gauge
          value={data.percent}
          valueMax={100}
          startAngle={-130}
          endAngle={130}
          sx={{
            [`& .${gaugeClasses.valueText}`]: {
              fontSize: 30,
              transform: "translate(0px, 0px)",
            },
            [`& .${gaugeClasses.valueText}`]: {
              fontSize: 40,
            },
            [`& .${gaugeClasses.valueArc}`]: {
              fill: theme.palette.secondary.main,
            },
            [`& .${gaugeClasses.referenceArc}`]: {
              fill: "#E9EDF0",
            },
          }}
          text={({ value, valueMax }) => ``}
        />
        <Stack
          position={"absolute"}
          justifyContent={"center"}
          alignItems={"center"}
          top={"80px"}
        >
          <Typography fontSize={"30px"} fontWeight={"700"}>
            {data.count}
          </Typography>
          <Typography fontSize={"14px"} fontWeight={"700"}>
            Respostas
          </Typography>
        </Stack>
      </Stack>
      <Typography fontWeight={"700"} fontSize={"12px"} color={"text.secondary"}>
        Corresponde a{" "}
        <Typography
          fontWeight={"700"}
          fontSize={"12px"}
          component={"span"}
          color={"secondary.main"}
        >
          {data.percent}%
        </Typography>{" "}
        da equipe
      </Typography>
    </Stack>
  );
}
