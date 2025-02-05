import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface CreatePostUseCaseProps {
    message: string;
    expirationDate?: string;
    publicationHighlight?: boolean;
    publicationAllowComments?: boolean;
    idsSectors?: number[];
    idsSubsectors?: number[];
    idsPeriods?: number[];
    idsGroups?: number[];
    idsHierarchies?: number[];
    idsCollaborators?: number[];
    idsClients?: number[];
    idsHomeOrFloors?: number[];
    files?: File[];
    codPostReference?: number;
}

export class CreatePostUseCase {
    private client = guildaApiClient2;

    async handle(props: CreatePostUseCaseProps) {
        const {
            message,
            expirationDate,
            files,
            publicationHighlight,
            publicationAllowComments,
            idsSectors,
            idsSubsectors,
            idsPeriods,
            idsGroups,
            idsHierarchies,
            idsCollaborators,
            idsClients,
            idsHomeOrFloors,
            codPostReference,
        } = props;

        const payload = {
            Post: message,
            codPostReference: codPostReference || 0,
            expiredAt: expirationDate,
            highlight: publicationHighlight ? 1 : 0,
            allowComment: publicationAllowComments ? 1 : 0,
            visibility: {
                sector: idsSectors || [],
                subSector: idsSubsectors || [],
                period: idsPeriods || [],
                hierarchy: [],
                group: idsGroups || [],
                userId: idsCollaborators || [],
                client: idsClients || [],
                homeOrFloor: idsHomeOrFloors || [],
            },
        };

        const form = new FormData();
        form.append("JSON", JSON.stringify(payload));
        if (files && files.length > 0) {
            for (let i = 0; i < files.length; i++) {
                form.append(`FILES[${i}]`, files[i]);
            }
        }
        
        const { data } = await this.client.post<
            unknown,
            AxiosResponse,
            FormData
        >(`/PersonaPost`, form);

        return data;
    }
}
