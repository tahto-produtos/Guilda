import { guildaApiClient2 } from "src/services";

interface IProps {
    limit: number;
    page: number;
}

export class ListInfractionsUseCase {
    private client = guildaApiClient2;

    async handle(props: IProps) {
        const { data } = await this.client.post(
            `/ListFeedBackInfraction`,
            props
        );

        return data;
    }
}
