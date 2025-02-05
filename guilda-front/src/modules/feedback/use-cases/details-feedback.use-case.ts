import { guildaApiClient2 } from "src/services";

export interface FeedbackDetail {
    idFeedbackUser: number;
    sendFor: string;
    sendForId: number;
    sendForHierarchy: string;
    title: string;
    linkFile: string;
    files: {
        url: string;
    }[];
    protocol: string;
    status: string;
    nameInfraction: string;
    createdAt: string;
    content: string;
}

export class DetailsFeedBackUseCase {
    private client = guildaApiClient2;

    async handle(props: { id: number }) {
        const { data } = await this.client.get<FeedbackDetail>(
            `/ListDetailsFeedBack?idFeedbackUser=${props.id}`
        );

        return data;
    }
}
