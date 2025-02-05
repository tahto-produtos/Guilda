import { addDays, format, startOfDay } from "date-fns";
import { guildaApiClient2 } from "src/services";
import {AxiosResponse} from "axios";
import {Friend} from "../../../typings/models/friend.model";

export interface ListFollowUseCaseProps {
    follow: boolean;
    limit: number;
    page: number;
    filterName: string;
    idPersona: number;
}

export interface ListFollowings {
    TOTALPAGES: number;
    listsFollows: Friend[];
}

export class ListFollowUseCase {
    private client = guildaApiClient2;

    async handle(props: ListFollowUseCaseProps) {
        const { data } = await this.client.get<unknown, AxiosResponse<ListFollowings>>(
            `/PersonaFollow?follow=${props.follow}&idPersona=${props.idPersona}&limit=${props.limit}&page=${props.page}&filterName=${props.filterName}`
        );

        return data;
    }
}
