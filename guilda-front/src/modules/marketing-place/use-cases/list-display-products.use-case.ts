import { guildaApiClient2 } from "src/services";

export class ListDisplayProductsUseCase {
    private client = guildaApiClient2;

    async handle(
        hierarchy: number,
        group: number,
        COLLABORATORID: number | string
    ) {
        const { data } = await this.client.get(
            `/ListDisplayProducts?HIERARQUIA=${hierarchy}&GRUPO=${group}&COLLABORATORID=${COLLABORATORID}`
        );

        return data;
    }
}
