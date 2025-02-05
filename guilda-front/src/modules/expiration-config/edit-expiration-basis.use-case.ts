import { guildaApiClient2 } from "src/services";

interface EditExpirationBasisUseCaseProps {
    home: string;
    present: string;
    collaboratorId: number;
}

export class EditExpirationBasisUseCase {
    private client = guildaApiClient2;

    async handle(props: EditExpirationBasisUseCaseProps) {
        const { collaboratorId, home, present } = props;

        const payload = {
            Expiration_Home: home,
            Expiration_Present: present,
            CollabortorID: collaboratorId,
        };

        const { data } = await this.client.post(
            `/InitialExpirationBasis`,
            payload
        );

        return data;
    }
}
