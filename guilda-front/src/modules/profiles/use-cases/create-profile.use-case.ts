import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";
import { PaginationModel } from "src/typings/models/pagination.model";
import { PermissionModel } from "src/typings/models/permission.model";
import { ProfileModel } from "src/typings/models/profile.model";

export class CreateProfileUseCase {
    private client = guildaApiClient2;

    async handle(name: string) {
        const form = new FormData();
        form.append("name", name);

        const { data } = await this.client.post<
            unknown,
            AxiosResponse,
            FormData
        >("/CreateProfile", form);

        return data;
    }
}
