import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface CreateLikeUseCaseProps {
    postId: number;
    isLike?: boolean;
    idReaction: number;
}

export class CreateLikeUseCase {
    private client = guildaApiClient2;

    async handle(props: CreateLikeUseCaseProps) {
        const { postId, isLike, idReaction } = props;

        const payload = {
            idReaction: idReaction,
            flag: isLike,
            isComment: false,
            codPostComment: postId,
        };

        const { data } = await this.client.post<unknown, AxiosResponse>(
            `/PersonaReaction`,
            payload
        );

        return data;
    }
}
