import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";
import { ListQuizResponseModel, ListTypeQuestionResponseModel } from "src/typings";

export class ListTypeQuestionsUseCase {
    private client = guildaApiClient2;

    async handle() {
        const { data } = await this.client.get<
            unknown,
            AxiosResponse<ListTypeQuestionResponseModel[]>
        >(
            `/FilterTypeQuestionQuiz`
        );

        return data;
    }
}
