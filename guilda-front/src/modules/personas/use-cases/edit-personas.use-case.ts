import { AxiosResponse } from "axios";
import { guildaApiClient2 } from "src/services";
import { Hobby } from "src/typings/models/hobby.model";

export interface EditPersonasUseCaseProps {
    IDPERSONAUSER?: string;
    NOME?: string;
    BC?: string;
    EMAIL?: string;
    TELEFONE?: string;
    DATA_NASCIMENTO?: string;
    UF?: number;
    SITE?: string;
    CIDADE?: number;
    WHO_IS?: string;
    NOME_SOCIAL?: string;
    IDADE?: number;
    FOTO?: string;
    MOTIVACOES?: string;
    OBJETIVO?: string;
    HOBBIES?: Hobby[];
    PROFILE_IMAGE?: File;
}

export class EditPersonasUseCase {
    private client = guildaApiClient2;

    async handle(props: EditPersonasUseCaseProps) {
        const payload = props;

        if(payload.HOBBIES && payload.HOBBIES?.length > 0) {
            const newHobbiesArray = payload.HOBBIES.map(hobby => ({
                HOBBY: hobby.HOBBY,
                IDGDA_HOBBIES: hobby.IDGDA_PERSONA_USER_HOBBY ? hobby.IDGDA_PERSONA_USER_HOBBY : hobby.IDGDA_HOBBIES
            }));
            payload.HOBBIES = newHobbiesArray;
        }

        const form = new FormData();
        form.append("json", JSON.stringify(payload));

        if(payload.PROFILE_IMAGE) {
            form.append("file", payload.PROFILE_IMAGE);
        }

        const { data } = await this.client.post<
            unknown,
            AxiosResponse,
            FormData
        >(`/EditPersonaUser`, form);

        return data;
        //return true;
    }
}
