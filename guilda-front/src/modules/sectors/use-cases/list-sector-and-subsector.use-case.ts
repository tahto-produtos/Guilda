import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";
import {SectorAndSubsector} from "../../../typings";

interface ListSectorsAndSubSectorsUseCaseProps {
    isSubsector?: boolean;
    sector?: string;
}

export class ListSectorsAndSubsectrosUseCase {
    private client = guildaApiClient2;

    async handle(
        props: ListSectorsAndSubSectorsUseCaseProps
    ) {
        const { isSubsector = 0, sector } = props;

        const { data } = await this.client.get<
            unknown,
            AxiosResponse<SectorAndSubsector[]>
        >(
            `/Sectors?sector=${
                sector ? sector : ""
            }&isSubsector=${isSubsector ? 1 : 0}`
        );

        return data;
    }
}
