import { useState } from "react";

export function useLoadingState(initLoading = false) {
  const [isLoading, setIsLoading] = useState(initLoading);

  const startLoading = () => setIsLoading(true);
  const finishLoading = () => setIsLoading(false);

  return {
    isLoading,
    startLoading,
    finishLoading,
  };
}
