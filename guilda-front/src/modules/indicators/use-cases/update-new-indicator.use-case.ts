import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";

interface UpdateNewIndicatorUseCaseProps {
    INDICATORID: number;
    NAME: string;
    DESCRIPTION: string;
    METRIC?: string;
    CALCTYPE?: string;
    WEIGHT?: number;
    SECTOR: string;
}

export class UpdateNewIndicatorUseCase {
    private client = guildaApiClient2;

    async handle(props: UpdateNewIndicatorUseCaseProps) {
        const {
            INDICATORID,
            NAME,
            DESCRIPTION,
            METRIC,
            CALCTYPE,
            WEIGHT,
            SECTOR,
        } = props;

        var bodyFormData = new FormData();
        bodyFormData.append('INDICATORID', INDICATORID.toString());
        bodyFormData.append('NAME', NAME);
        bodyFormData.append('DESCRIPTION', DESCRIPTION);
        METRIC && bodyFormData.append('METRIC', METRIC.toString());
        CALCTYPE && bodyFormData.append('CALCTYPE', CALCTYPE.toString());
        WEIGHT && bodyFormData.append('WEIGHT', WEIGHT.toString());
        bodyFormData.append('SECTOR', SECTOR.toString());

        const { data } = await this.client.post<unknown, AxiosResponse>(
            `/UpdateNewIndicator`,
            bodyFormData
        );

        return data;
    }
}
