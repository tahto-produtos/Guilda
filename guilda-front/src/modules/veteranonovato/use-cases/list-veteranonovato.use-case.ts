import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";
import { Period } from "../../../typings";

interface ListVeteranoNovatoUseCaseProps {

}

export class ListVeteranoNovatoUseCase {
    private client = guildaApiClient2;

    async handle(
        props: ListVeteranoNovatoUseCaseProps
    ) {
        const { data } = await this.client.get<
            unknown,
            AxiosResponse<Period[]>
        >(
            `/VeteranoNovato`
        );

        return data;
    }
}
