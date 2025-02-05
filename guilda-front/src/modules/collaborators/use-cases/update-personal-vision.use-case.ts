import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";
import { PaginationModel } from "src/typings/models/pagination.model";

export class UpdatePersonalVision {
    private client = guildaApiClient2;

    async handle(props: any) {
        const payload = {
            isChecked: props.checked,
        };

        const { data } = await this.client.post<unknown, AxiosResponse>(
            `/UpdateCollaboratorVision`,
            payload
        );

        return data;
    }
}
