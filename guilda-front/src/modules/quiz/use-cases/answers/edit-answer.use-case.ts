import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface EditAnswerUseCaseProps {
    IDGDA_QUIZ_ANSWER: number;
    QUESTION: string;
    RIGHT_ANSWER: number;
}

export class EditAnswerUseCase {
    private client = guildaApiClient2;

    async handle(props: EditAnswerUseCaseProps) {
        //const { IDGDA_QUIZ_ANSWER, QUESTION, RIGHT_ANSWER } = props;
    

        const { data } = await this.client.post<
            unknown,
            AxiosResponse
        >(
            `/EditQuizAnswer`,
            props,
        );

        return data;
    }
}
