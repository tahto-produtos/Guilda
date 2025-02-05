import { guildaApiClient } from "src/services";

interface DeleteCategoryUseCaseProps {
    id: number;
}

export class DeleteCategoryUseCase {
    private client = guildaApiClient;

    async handle(props: DeleteCategoryUseCaseProps) {
        const { data } = await this.client.delete(`/Category/${props.id}`);

        return data;
    }
}
