import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface CreateNotificationUseCaseProps {
    IDGDA_NOTIFICATION_TYPE: number;
    TITLE: string;
    NOTIFICATION: string;
    ACTIVE: boolean;
    STARTED_AT: string;
    ENDED_AT: string;
    files: File[];
    visibility: {
        sector: number[];
        subSector: number[];
        period: number[];
        hierarchy: number[];
        group: number[];
        userId: number[];
        client: number[];
        site: number[];
        homeOrFloor: number[];
    };
}

export class CreateNotificationUseCase {
    private client = guildaApiClient2;

    async handle(props: CreateNotificationUseCaseProps) {
        const {
            ACTIVE,
            ENDED_AT,
            IDGDA_NOTIFICATION_TYPE,
            NOTIFICATION,
            STARTED_AT,
            TITLE,
            files,
            visibility,
        } = props;

        const payload = {
            IDGDA_NOTIFICATION_TYPE,
            TITLE,
            NOTIFICATION,
            ACTIVE: ACTIVE ? 1 : 0,
            STARTED_AT,
            ENDED_AT,
            visibility,
        };

        const form = new FormData();
        form.append("json", JSON.stringify(payload));
        if (files && files.length > 0) {
            for (let i = 0; i < files.length; i++) {
                form.append(`files[${i}]`, files[i]);
            }
        }
        const { data } = await this.client.post<
            unknown,
            AxiosResponse,
            FormData
        >(`/CreatedNotification`, form);

        return data;
    }
}
