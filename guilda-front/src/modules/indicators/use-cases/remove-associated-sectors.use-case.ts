import { guildaApiClient } from "src/services";

export interface RemoveAssociatedSectorsUseCaseProps {
    indicatorId: number;
    sectorId: number;
}

export class RemoveAssociatedSectorsUseCase {
    private client = guildaApiClient;

    async handle(props: RemoveAssociatedSectorsUseCaseProps) {
        const { indicatorId, sectorId } = props;

        const payload = {
            sectorsIds: [sectorId],
        };

        const { data } = await this.client.delete<FormData>(
            `/indicators/${indicatorId}/sectors`,
            { data: payload }
        );

        return data;
    }
}
