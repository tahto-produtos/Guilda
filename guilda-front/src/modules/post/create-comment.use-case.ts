import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface CreateCommentUseCaseProps {
    comment: string;
    codPost: number;
}

export class CreateCommentUseCase {
    private client = guildaApiClient2;

    async handle(props: CreateCommentUseCaseProps) {
        const { codPost, comment } = props;

        const payload = {
            codPost,
            comment,
        };

        const { data } = await this.client.post<unknown, AxiosResponse>(
            `/PersonaComment`,
            payload
        );

        return data;
    }
}
