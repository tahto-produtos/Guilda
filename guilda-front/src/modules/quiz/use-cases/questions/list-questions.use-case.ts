import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";
import { ListQuestionsResponseModel } from "src/typings"; 

interface ListQuestionUseCaseProps {
    quiz: number;
    word: string;
}

export class ListQuestionsUseCase {
    private client = guildaApiClient2;

    async handle(props: ListQuestionUseCaseProps) {
        const { quiz, word } = props;
        console.log(quiz, word);
        const { data } = await this.client.get<
            unknown,
            AxiosResponse<ListQuestionsResponseModel[]>
        >(
            `/ListQuizQuestion?quiz=${quiz}&word=${word}`
        );

        return data;
    }
}
