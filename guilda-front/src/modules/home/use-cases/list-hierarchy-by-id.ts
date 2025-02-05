import { guildaApiClient2 } from "src/services";

export interface ListHierarchyByIdUseCaseProps {
    CollaboratorId: number;
}

export class ListHierarchyByIdUseCase {
    private client = guildaApiClient2;

    async handle(props: ListHierarchyByIdUseCaseProps) {
        const { CollaboratorId } = props;

        const { data } = await this.client.get(
            `/Hierarchy?codCollaborator=${CollaboratorId}`
        );

        return data;
    }
}
