import { createContext } from "react";

export interface ClimateContextData {}

export const ClimateContext = createContext<ClimateContextData>(
  {} as ClimateContextData
);
