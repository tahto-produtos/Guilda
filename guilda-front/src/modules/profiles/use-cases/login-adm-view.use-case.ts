import { AxiosResponse } from "axios";
import { guildaApiClient, guildaApiClient2 } from "src/services";
/* import { LoginRequestDto, LoginResponseDto } from "src/modules/auth/use-cases/login/dto/index"; */

export class LoginAdmViewUseCase {
    private client = guildaApiClient2;
/*     private client2 = guildaApiClient2; */
    async handle(props: { profileId: number }) {
        const { profileId } = props;

        const { data } = await this.client.get<unknown, AxiosResponse>(
            `/ProfileLoginVision?username=BC${profileId}`
        );  

        /*   const { data } = await this.client.get<unknown, AxiosResponse>(
            `/profiles/${profileId}/login`
        ); */  

        return data;
    }
}
