import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface ListLibraryNotificationsProps {
    idCreator: number;
    title: string;

    createdAtFrom: string;
    createdAtTo: string;
    startedAtFrom: string;
    startedAtTo: string;
    endedAtFrom: string;
    nameCreator: string;
    word: string;
    scheduling: boolean;
    type: string;

    limit: number;
    page: number;
}

export class ListLibraryNotifications {
    private client = guildaApiClient2;

    async handle(props: ListLibraryNotificationsProps) {
        const {
            idCreator,
            title,
            createdAtFrom,
            createdAtTo,
            endedAtFrom,
            nameCreator,
            scheduling,
            startedAtFrom,
            startedAtTo,
            word,
            type,
            limit,
            page,
        } = props;

        const payload = {
            createdAtFrom,
            createdAtTo,
            startedAtFrom,
            startedAtTo,
            endedAtFrom,
            title,
            idCreator: idCreator,
            nameCreator,
            word,
            type,
            scheduling: scheduling.toString(),
            limit,
            page,
        };

        const { data } = await this.client.post<unknown, AxiosResponse>(
            `/LoadLibraryNotification`,
            payload
        );

        return data;
    }
}
