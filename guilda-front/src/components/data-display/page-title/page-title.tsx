import { CircularProgress, Stack, Typography } from "@mui/material";
import { ReactNode } from "react";

interface PageTitleProps {
  title: string;
  children?: ReactNode;
  loading?: boolean;
  icon?: ReactNode;
}

export function PageTitle(props: PageTitleProps) {
  const { title, children, loading, icon } = props;

  return (
    <Stack
      direction={"row"}
      alignItems={"center"}
      width={"100%"}
      mb={"24px"}
      justifyContent={"space-between"}
    >
      <Stack
        direction={"row"}
        alignItems={"center"}
        gap={"10px"}
        sx={{ minWidth: "fit-content" }}
      >
        {icon && icon}
        <Typography variant="h1" sx={{ minWidth: "fit-content" }}>
          {title}
        </Typography>
        {loading && <CircularProgress size={16} sx={{ ml: "10px" }} />}
      </Stack>
      {children && (
        <Stack width={"100%"} justifyContent={"flex-end"} direction={"row"}>
          {children}
        </Stack>
      )}
    </Stack>
  );
}
