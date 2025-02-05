import { guildaApiClient, guildaApiClient2 } from "src/services";

export interface SectorsByHierachyUseCaseProps {
    codCollaborator: number;
    sector: string;
    dtInicial: string;
    dtfinal: string;
}

export class SectorsByHierachyUseCase {
    private client = guildaApiClient2;

    async handle(props: SectorsByHierachyUseCaseProps) {
        const { codCollaborator, sector, dtInicial, dtfinal } = props;

        const { data } = await this.client.get(
            `/SectorsByHierarchy?codCollaborator=${codCollaborator}&sector=${sector}&dtInicial=${dtInicial}&dtfinal=${dtfinal}`
        );

        return data;
    }
}
