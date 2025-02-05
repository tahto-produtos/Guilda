import { guildaApiClient2 } from "../../../../services";
import { AxiosResponse } from "axios";
import { HomeFloor} from "../../../../typings";

interface ListHomeFloorUseCaseProps {
    codCollaborator?: number;
}

export class ListHomeFloorUseCase {
    private client = guildaApiClient2;

    async handle(props: ListHomeFloorUseCaseProps) {
        const { codCollaborator } = props;

        const { data } = await this.client.get<
            unknown,
            AxiosResponse<HomeFloor[]>
        >(`/HomeFloor`);

        return data;
    }
}
