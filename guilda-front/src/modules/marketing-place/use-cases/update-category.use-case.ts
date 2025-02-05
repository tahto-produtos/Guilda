import { guildaApiClient } from "src/services";

interface UpdateCategoryUseCaseProps {
    id: number;
    categoryName: string;
}

export class UpdateCategoryUseCase {
    private client = guildaApiClient;

    async handle(props: UpdateCategoryUseCaseProps) {
        const { categoryName, id } = props;

        const payload = {
            categoryName,
        };

        const { data } = await this.client.put(`/Category/${id}`, payload);

        return data;
    }
}
