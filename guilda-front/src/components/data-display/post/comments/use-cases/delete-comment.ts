import { guildaApiClient2 } from "src/services";

export class DeleteComment {
    private client = guildaApiClient2;

    async handle(props: { idComment: number, isAdm: boolean}) {
        const { data } = await this.client.delete(
            `/PersonaComment?idComment=${props.idComment}&isAdm=${props.isAdm}`
        );

        return data;
    }
}
