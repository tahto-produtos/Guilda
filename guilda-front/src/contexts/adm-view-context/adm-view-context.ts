import { createContext } from "react";
import { Collaborator } from "src/typings/models/collaborator.model";

export interface AdmViewContextData {
    isAdmViewActive?: boolean;
    setActiveUser: (
        input: {
            id: number;
            name: string;
            registry: string;
            token: string;
        } | null
    ) => void;
}

export const AdmViewContext = createContext<AdmViewContextData>(
    {} as AdmViewContextData
);
