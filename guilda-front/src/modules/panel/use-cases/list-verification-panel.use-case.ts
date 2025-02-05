import { addDays, format } from "date-fns";
import { guildaApiClient2 } from "src/services";

export interface ListVerificationPanleUseCaseProps {
    indicatorId: number;
    dateSend: string;
    collaboratorId: number;
}

export class ListVerificationPanelUseCase {
    private client = guildaApiClient2;

    async handle(props: ListVerificationPanleUseCaseProps) {
        const { indicatorId, dateSend, collaboratorId } = props;

        const { data } = await this.client.get(`/ListPainel?idIndicator=${indicatorId}&dtSend=${dateSend}&idCollaborator=${collaboratorId}`);

        return data;
    }
}
