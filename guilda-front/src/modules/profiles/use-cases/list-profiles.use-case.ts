import { AxiosResponse } from "axios";
import { guildaApiClient } from "src/services";
import { PaginationModel } from "src/typings/models/pagination.model";
import { ProfileModel } from "src/typings/models/profile.model";

export class ListProfilesResponseDto {
    items!: ProfileModel[];
}

export class ListProfilesUseCase {
    private client = guildaApiClient;

    async handle(props: PaginationModel) {
        const { limit, offset } = props;

        const { data } = await this.client.get<
            unknown,
            AxiosResponse<ListProfilesResponseDto>
        >(`/profiles?limit=${limit}&offset=${offset}`);

        return data;
    }
}
