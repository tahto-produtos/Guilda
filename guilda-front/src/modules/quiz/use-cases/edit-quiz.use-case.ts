import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services"; 

interface EditQuizUseCaseProps {
    IDGDA_QUIZ?: number;
    TITLE?: string;
    DESCRIPTION?: string;
    REQUIRED?: number;
    IDGDA_COLLABORATOR_DEMANDANT?: number;
    IDGDA_COLLABORATOR_RESPONSIBLE?: number;
    MONETIZATION?: number;
    PERCENT_MONETIZATION?: number;
    STARTED_AT?: string;
    ENDED_AT?: string;
    /* visibility: 
    {
        sector: number[];
        subSector: number[];
        period: number[];
        hierarchy: number[];
        group: number[];
        userId: number[];
        client: number[];
        homeOrFloor: number[];
    } */
}

export class EditQuizUseCase {
    private client = guildaApiClient2;

    async handle(props: EditQuizUseCaseProps) {
        const payload = props;

        const { data } = await this.client.post<
            unknown,
            AxiosResponse
        >(
            `/EditQuiz`,
            payload
        );

        return data;
    }
}
