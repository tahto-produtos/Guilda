import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";
import { Period } from "../../../typings";

interface ListPeriodUseCaseProps {
    codCollaborator?: number;
}

export class ListPeriodUseCase {
    private client = guildaApiClient2;

    async handle(
        props: ListPeriodUseCaseProps
    ) {
        const { codCollaborator } = props;

        const { data } = await this.client.get<
            unknown,
            AxiosResponse<Period[]>
        >(
            `/Period`
        );

        return data;
    }
}
