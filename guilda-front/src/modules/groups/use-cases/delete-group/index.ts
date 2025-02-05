import { guildaApiClient } from "src/services";

export interface DeleteGroupProps {
    id: string;
}

export class DeleteGroup {
    private client = guildaApiClient;

    async handle(props: DeleteGroupProps) {
        const { id } = props;

        const { data } = await this.client.delete(`/groups/${id}`);

        return data;
    }
}
