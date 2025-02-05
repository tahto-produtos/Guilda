import { ReactNode } from "react";
import { Box } from "@mui/material";

interface PublicLayoutProps {
  children: ReactNode;
}

export function PublicLayout({ children }: PublicLayoutProps) {
  return <Box>{children}</Box>;
}
