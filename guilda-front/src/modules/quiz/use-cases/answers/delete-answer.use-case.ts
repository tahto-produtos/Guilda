import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services"; 

interface DeleteAnswerUseCaseProps {
    IDGDA_QUIZ_ANSWER: number;
    VALIDATED: boolean;
}

export class DeleteAnswerUseCase {
    private client = guildaApiClient2;

    async handle(props: DeleteAnswerUseCaseProps) {
        const payload = props;
        const { data } = await this.client.post<
            unknown,
            AxiosResponse
        >(
            `/DeletedQuizAnswer`,
            payload
        );

        return data;
    }
}
