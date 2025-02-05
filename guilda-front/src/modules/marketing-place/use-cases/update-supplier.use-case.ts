import { guildaApiClient } from "src/services";

interface UpdateSupplierUseCaseProps {
    id: number;
    supplierName: string;
}

export class UpdateSupplierUseCase {
    private client = guildaApiClient;

    async handle(props: UpdateSupplierUseCaseProps) {
        const { supplierName, id } = props;

        const payload = {
            supplierName,
        };

        const { data } = await this.client.put(`/Supplier/${id}`, payload);

        return data;
    }
}
