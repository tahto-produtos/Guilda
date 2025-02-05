import { AxiosResponse } from "axios";
import { addDays, format, startOfDay } from "date-fns";
import { guildaApiClient2 } from "src/services";
import { Friend } from "src/typings/models/friend.model";

export interface ListFollowUseCaseProps {
    sectors: number[];
}

export class ListFollowSugestionUseCase {
    private client = guildaApiClient2;

    async handle(props: ListFollowUseCaseProps) {
        const { sectors } = props;

        const payload = {
            sectors,
        };

        const { data } = await this.client.post<
            unknown,
            AxiosResponse<Friend[]>
        >(`/PersonaSugestFollow`, payload);

        return data;
    }
}
