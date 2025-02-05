import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface ReturnHierachyProfileUseCaseProps {
    IDGDA_PROFILE: number;
}

export class ReturnHierachyProfileUseCase {
    private client = guildaApiClient2;

    async handle(props: ReturnHierachyProfileUseCaseProps) {
        const { IDGDA_PROFILE } = props;

        const { data } = await this.client.get<unknown, AxiosResponse>(
            `/ReturnHierarchyProfile?IDGDA_PROFILE=${IDGDA_PROFILE}`
        );

        return data;
    }
}
