import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";
import { ListAnswerResponseModel } from "src/typings"; 

interface ListAnswerUseCaseProps {
    idquestion: number;
    word: string;
}

export class ListAnswerUseCase {
    private client = guildaApiClient2;

    async handle(props: ListAnswerUseCaseProps) {
        const { idquestion, word } = props;
        
        const { data } = await this.client.get<
            unknown,
            AxiosResponse<ListAnswerResponseModel[]>
        >(
            `/ListQuizAnswer?idquestion=${idquestion}&word=${word}`
        );

        return data;
    }
}
