import { guildaApiClient } from "../../../../services";
import { AxiosResponse } from "axios";
import FormData from "form-data";

interface UpdateGroupUseCaseProps {
    name: string;
    alias: string;
    description: string;
    image: File;
    id: string;
}

export class UpdateGroupUseCase {
    private client = guildaApiClient;

    async handle(props: UpdateGroupUseCaseProps) {
        const { name, alias, description, image, id } = props;
        const form = new FormData();
        form.append("name", name);
        form.append("alias", alias);
        form.append("description", description);
        form.append("image", image);

        const { data } = await this.client.put<
            unknown,
            AxiosResponse,
            FormData
        >(`/groups/${id}`, form);

        return data;
    }
}
