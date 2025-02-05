import { guildaApiClient2 } from "src/services";

interface ListProductsByVisibilityProps {
    Groups: { id: number }[];
    hierarchies: { id: number }[];
    productName: string;
    stock: { id: number }[];
}

export class ListProductsByVisibility {
    private client = guildaApiClient2;

    async handle(props: ListProductsByVisibilityProps) {
        const { Groups, hierarchies, productName, stock } = props;

        const payload = {
            Groups,
            hierarchies,
            productName,
            stock,
        };

        const { data } = await this.client.post(`/ProductVisibility`, payload);

        return data;
    }
}
