import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

export interface ListClimateUseCaseProps {
    STARTEDATFROM: string;
    STARTEDATTO: string;
    PERSONASID: number[];
    SECTORSID: number[];
    FLAGRESPONSE: number; //Caso = 1. Só os respondidos
    FLAGNORESPONSE: number; //Caso = 1. Só as não respondidas
    FLAGCANFEEDBACK: number; //Caso = 1. Apenas as que podem e ainda não receberam feedback
    limit: number;
    page: number;
}

export class ListClimateUseCase {
    private client = guildaApiClient2;

    async handle(props: ListClimateUseCaseProps) {


        const { data } = await this.client.post<
            unknown,
            AxiosResponse
        >(`/ListReportHierarchyClimate`, props);

        return data;
    }
}