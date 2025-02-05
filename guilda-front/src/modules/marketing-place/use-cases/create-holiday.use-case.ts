import { guildaApiClient } from "src/services";

export interface CreateHolidayUseCaseProps {
    city: string;
    type: string;
    holidayDate: any;
}

export class CreateHolidayUseCase {
    private client = guildaApiClient;

    async handle(props: CreateHolidayUseCaseProps) {
        const { city, type, holidayDate } = props;

        const payload = {
            site: city, type, holidayDate,
        };

        const { data } = await this.client.post(`/Holidays`, payload);

        return data;
    }
}
