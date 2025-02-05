import { guildaApiClient } from "src/services";

export interface CreateSupplierUseCaseProps {
    supplierName: string;
}

export class CreateSupplierUseCase {
    private client = guildaApiClient;

    async handle(props: CreateSupplierUseCaseProps) {
        const { supplierName } = props;

        const payload = {
            supplierName,
        };

        const { data } = await this.client.post(`/Supplier`, payload);

        return data;
    }
}
