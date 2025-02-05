import { guildaApiClient } from "src/services";

interface DeleteHolidayUseCaseProps {
    id: number;
}

export class DeleteHolidayUseCase {
    private client = guildaApiClient;

    async handle(props: DeleteHolidayUseCaseProps) {
        const { data } = await this.client.delete(`/Holidays/${props.id}`);

        return data;
    }
}
