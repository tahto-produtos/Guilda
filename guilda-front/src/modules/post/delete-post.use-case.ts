import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface DeletePostUseCaseProps {
    idPost: number;
    isAdm: boolean;
}

export class DeletePostUseCase {
    private client = guildaApiClient2;

    async handle(props: DeletePostUseCaseProps) {
        const { idPost, isAdm } = props;


        const { data } = await this.client.delete<unknown, AxiosResponse>(
            `/PersonaPost?idPost=${idPost}&isAdm=${isAdm}`
        );

        return data;
    }
}
