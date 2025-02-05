import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface CreateAnswerUseCaseProps {
    IDGDA_QUIZ_QUESTION: number;
    QUESTION: string;
    RIGHT_ANSWER: boolean;
    file: File[];
}

export class CreateAnswerUseCase {
    private client = guildaApiClient2;

    async handle(props: CreateAnswerUseCaseProps) {
        const { IDGDA_QUIZ_QUESTION, QUESTION, RIGHT_ANSWER, file} = props;
        
        const payload = {
            IDGDA_QUIZ_QUESTION: IDGDA_QUIZ_QUESTION,
            QUESTION: QUESTION,
            RIGHT_ANSWER: RIGHT_ANSWER == true ? 1 : 0,
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
            `/CreatedQuizAnswer`,
            form,
        );

        return data;
    }
}
