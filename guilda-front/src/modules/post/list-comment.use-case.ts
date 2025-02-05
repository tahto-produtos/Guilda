import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface ListCommentUseCaseProps {
    codPost: number;
    limit: number;
    page: number;
    isAdm: boolean;
}

export class ListCommentUseCase {
    private client = guildaApiClient2;

    async handle(props: ListCommentUseCaseProps) {
        const { codPost, limit, page, isAdm } = props;
        const { data } = await this.client.get<unknown, AxiosResponse>(
            `/PersonaComment?codPost=${codPost}&isAdm=${isAdm}&limit=${limit}&page=${page}`
        );

        return data;
    }
}
