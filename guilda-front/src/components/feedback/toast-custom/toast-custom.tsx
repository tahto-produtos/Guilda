import { Stack, Typography } from "@mui/material";
import { TypeOptions, toast } from "react-toastify";
import { error } from "console";
import NotificationImportantOutlined from "@mui/icons-material/NotificationImportantOutlined";
import Help from "@mui/icons-material/Help";

const CustomToast = ({
  title,
  subtitle,
  clickAction,
  type,
  isQuiz,
}: {
  type?: TypeOptions;
  title?: string;
  subtitle?: string;
  clickAction?: () => void;
  isQuiz?: boolean;
}) => (
  <Stack
    width={"100%"}
    direction={"row"}
    onClick={() => clickAction && clickAction()}
    alignItems={"center"}
    pb={"6px"}
    gap={"15px"}
    sx={{
      cursor: clickAction ? "pointer" : "default",
    }}
  >
    {isQuiz ? (
      <Help
        sx={{
          color: "#2FAC9F",
        }}
      />
    ) : (
      <NotificationImportantOutlined
        sx={{
          color: "#2FAC9F",
        }}
      />
    )}

    <Stack direction={"column"} gap={"10px"}>
      <Typography
        variant="overline"
        fontSize={"11px"}
        fontWeight={"600"}
        sx={{ color: "#777" }}
        lineHeight={"12px"}
      >
        Nova notificação
      </Typography>
      <Typography
        variant="h3"
        lineHeight={"14px"}
        fontSize={"14px"}
        fontWeight={"600"}
        sx={{ color: "#000" }}
      >
        {subtitle}
      </Typography>
      <Stack direction={"row"} gap={"5px"} alignItems={"center"}>
        <Typography
          variant="body1"
          lineHeight={"12px"}
          fontSize={"12px"}
          fontWeight={"600"}
          sx={{ color: "#000" }}
        >
          Enviado por:
        </Typography>
        <Typography
          variant="body1"
          lineHeight={"12px"}
          fontSize={"12px"}
          fontWeight={"600"}
          sx={{ color: "#000" }}
        >
          {title}
        </Typography>
      </Stack>
    </Stack>
  </Stack>
);

export const ToastCustom = ({
  title,
  subtitle,
  clickAction,
  type,
  isQuiz,
}: {
  type?: TypeOptions;
  title?: string;
  subtitle?: string;
  clickAction?: () => void;
  isQuiz?: boolean;
}) => {
  return toast(
    <CustomToast
      title={title}
      subtitle={subtitle}
      isQuiz={isQuiz}
      type={type}
      clickAction={clickAction}
    />,
    {
      position: "bottom-left",
      hideProgressBar: false,
      autoClose: 10000,
      closeOnClick: false,
      rtl: false,
      pauseOnFocusLoss: true,
      draggable: true,
      pauseOnHover: true,
      progressStyle: {
        background: "#2FAC9F",
      },
    }
  );
};

export default CustomToast;
