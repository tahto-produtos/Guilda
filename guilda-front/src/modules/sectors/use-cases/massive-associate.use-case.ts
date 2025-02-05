import { AxiosResponse } from "axios";
import FormData from "form-data";
import { guildaApiClient, guildaApiClient2 } from "src/services";

export class MassiveAssociateUseCase {
    private client = guildaApiClient2;

    async handle(file: File) {



         const form = new FormData();
        form.append("FILE", file);

        const { data } = await this.client.post<
            unknown,
            AxiosResponse,
            FormData
        >("/InputMassiveMetric", form);

        return data; 
/*         const form = new FormData();
        form.append("table", file); 

         const { data } = await this.client.post<
            unknown,
            AxiosResponse,
            FormData
        >("/sectors/upload/v3", form);  */

        return data;
    }
}
