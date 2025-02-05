import { guildaApiClient } from "src/services";

interface DeleteSupplierUseCaseProps {
    id: number;
}

export class DeleteSupplierUseCase {
    private client = guildaApiClient;

    async handle(props: DeleteSupplierUseCaseProps) {
        const { data } = await this.client.delete(`/Supplier/${props.id}`);

        return data;
    }
}
