import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface CreateQuestionUseCaseProps {
    IDGDA_QUIZ: number;
    IDGDA_QUIZ_QUESTION_TYPE: number;
    QUESTION: string;
    TIME_ANSWER: number;
    file: File[];
}

export class CreateQuestionsUseCase {
    private client = guildaApiClient2;

    async handle(props: CreateQuestionUseCaseProps) {
        const { IDGDA_QUIZ, IDGDA_QUIZ_QUESTION_TYPE, QUESTION, TIME_ANSWER, file} = props;
        
        const payload = {
            IDGDA_QUIZ: IDGDA_QUIZ,
            IDGDA_QUIZ_QUESTION_TYPE: IDGDA_QUIZ_QUESTION_TYPE,
            QUESTION: QUESTION,
            TIME_ANSWER: TIME_ANSWER,
        };
        
        const form = new FormData();
        form.append("JSON", JSON.stringify(payload));
        if (file && file.length > 0) {
            form.append(`file`, file[0]);
        }

        const { data } = await this.client.post<
            unknown,
            AxiosResponse,
            FormData
        >(
            `/CreatedQuizQuestion`,
            form,
        );

        return data;
    }
}
