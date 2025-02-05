import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services"; 

interface DeleteQuestionUseCaseProps {
    IDGDA_QUIZ_QUESTION: number;
    VALIDATED: boolean;
}

export class DeleteQuestionUseCase {
    private client = guildaApiClient2;

    async handle(props: DeleteQuestionUseCaseProps) {
        const payload = props;
        const { data } = await this.client.post<
            unknown,
            AxiosResponse
        >(
            `/DeletedQuizQuestion`,
            payload
        );

        return data;
    }
}
