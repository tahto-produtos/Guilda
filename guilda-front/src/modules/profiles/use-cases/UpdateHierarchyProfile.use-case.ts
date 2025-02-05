import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface IProps {
    IDGDA_PROFILE: number;
    BASICAPROFILEHIERARCHY: number;
    CONFIRM: boolean;
}

export class UpdateHierarchyProfileUseCase {
    private client = guildaApiClient2;

    async handle(props: IProps) {
        const { BASICAPROFILEHIERARCHY, CONFIRM, IDGDA_PROFILE } = props;

        const payload = {
            BASICAPROFILEHIERARCHY,
            IDGDA_PROFILE,
            CONFIRM: CONFIRM ? "True" : "False",
        };

        const { data } = await this.client.post<unknown, AxiosResponse>(
            `/UpdateHierarchyProfile`,
            payload
        );

        return data;
    }
}
