import { guildaApiClient } from "../../../../services";

import CreateGroupRequestDto from "src/modules/groups/dto/create-group.request-dto";
import CreateGroupResponseDto from "src/modules/groups/dto/create-group.response-dto";
import { AxiosResponse } from "axios";
import FormData from "form-data";

export class CreateGroupUseCase {
    private client = guildaApiClient;

    async handle(createGroupDto: CreateGroupRequestDto) {
        const { name, alias, description, image } = createGroupDto;
        const form = new FormData();
        form.append("name", name);
        form.append("alias", alias);
        form.append("description", description);
        form.append("image", image);

        const { data } = await this.client.post<
            unknown,
            AxiosResponse<CreateGroupResponseDto>,
            FormData
        >("/groups", form);

        return data;
    }
}
