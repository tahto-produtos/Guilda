import { guildaApiClient } from "src/services";

export interface CreateCategoryUseCaseProps {
    categoryName: string;
}

export class CreateCategoryUseCase {
    private client = guildaApiClient;

    async handle(props: CreateCategoryUseCaseProps) {
        const { categoryName } = props;

        const payload = {
            categoryName,
        };

        const { data } = await this.client.post(`/Category`, payload);

        return data;
    }
}
