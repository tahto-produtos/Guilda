import { guildaApiClient2 } from "../../../../services";
import { AxiosResponse } from "axios";
import {GroupNew} from "../../../../typings/models/group-new.model";

interface ListGroupsNewUseCaseProps {
    codCollaborator?: number;
}

export class ListGroupsNewUseCase {
    private client = guildaApiClient2;

    async handle(props: ListGroupsNewUseCaseProps) {
        const { codCollaborator } = props;

        const { data } = await this.client.get<
            unknown,
            AxiosResponse<GroupNew[]>
        >(`/Group`);

        return data;
    }
}
