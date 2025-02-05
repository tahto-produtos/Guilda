import { useState } from "react";

export function useDrawer(initializeOpen = false) {
  const [isDrawerOpen, setIsDrawerOpen] = useState(false);

  const handleOpenDrawer = () => setIsDrawerOpen(true);
  const handleCloseDrawer = () => setIsDrawerOpen(false);

  return {
    isDrawerOpen,
    handleOpenDrawer,
    handleCloseDrawer,
  };
}
