import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";
import { ListQuizResponseModel } from "src/typings";

interface DuplicateQuizUseCaseProps {
    IDGDA_QUIZ: number;
}

export class DuplicateQuizUseCase {
    private client = guildaApiClient2;

    async handle(props: DuplicateQuizUseCaseProps) {
        const payload = props;

        const { data } = await this.client.post<
            unknown,
            AxiosResponse<ListQuizResponseModel>
        >(
            `/DuplicateQuiz`,
            payload
        );

        return data;
    }
}
