import { guildaApiClient } from "src/services";

interface UpdateHolidayUseCaseProps {
    id: number;
    city: string;
    type: string;
    holidayDate: any;
}

export class UpdateHolidayUseCase {
    private client = guildaApiClient;

    async handle(props: UpdateHolidayUseCaseProps) {
        const { city, type, holidayDate, id } = props;

        const payload = {
            site: city, type, holidayDate,
        };

        const { data } = await this.client.put(`/Holidays/${id}`, payload);

        return data;
    }
}
