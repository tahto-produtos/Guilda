import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface ListHierarchyUseCaseProps {
    IDGDA_HIERARCHY: number;
}

export class ListHierarchyUseCase {
    private client = guildaApiClient2;

    async handle(props: ListHierarchyUseCaseProps) {
        const { IDGDA_HIERARCHY } = props;

        const { data } = await this.client.get<unknown, AxiosResponse>(
            `/ListHierarchyByProfile?IDGDA_HIERARCHY=${IDGDA_HIERARCHY}`
        );

        return data;
    }
}
