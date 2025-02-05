import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface EditQuestionUseCaseProps {
    IDGDA_QUIZ_QUESTION: number;
    IDGDA_QUIZ_QUESTION_TYPE: number;
    QUESTION: string;
    TIME_ANSWER: number;
}

export class EditQuestionsUseCase {
    private client = guildaApiClient2;

    async handle(props: EditQuestionUseCaseProps) {
        const { IDGDA_QUIZ_QUESTION, IDGDA_QUIZ_QUESTION_TYPE, QUESTION, TIME_ANSWER} = props;
        
        const payload = {
            IDGDA_QUIZ_QUESTION: IDGDA_QUIZ_QUESTION,
            IDGDA_QUIZ_QUESTION_TYPE: IDGDA_QUIZ_QUESTION_TYPE,
            QUESTION: QUESTION,
            TIME_ANSWER: TIME_ANSWER,
        };

        const { data } = await this.client.post<
            unknown,
            AxiosResponse
        >(
            `/EditQuizQuestion`,
            payload,
        );

        return data;
    }
}
