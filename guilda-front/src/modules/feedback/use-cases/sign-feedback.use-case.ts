import { guildaApiClient2 } from "src/services";

export class SignFeedBackUseCase {
    private client = guildaApiClient2;

    async handle(props: { id: number }) {
        const { data } = await this.client.post(`/SignedFeedBack`, {
            IDGDA_FEEDBACK_USER: props.id,
        });

        return data;
    }
}
