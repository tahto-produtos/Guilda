import { addDays, format, startOfDay } from "date-fns";
import { guildaApiClient2 } from "src/services";

export interface CreateFollowUseCaseProps {
    follow: boolean;
    idFollowed: number;
}

export class CreateFollowUseCase {
    private client = guildaApiClient2;

    async handle(props: CreateFollowUseCaseProps) {
        const { follow, idFollowed } = props;

        const payload = {
            idFollowed: idFollowed.toString(),
            follow,
        };

        const { data } = await this.client.post(`/PersonaFollow`, payload);

        return data;
    }
}
