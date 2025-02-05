import { guildaApiClient, guildaApiClient2 } from "../../../../services";
import { LoginRequestDto, LoginResponseDto } from "./dto";
import Cookies from "js-cookie";
import {
    INTERNAL_SERVER_ERROR_MESSAGE,
    jwtTokenKey,
} from "../../../../constants";
import { AxiosResponse } from "axios";
import { isAxiosError } from "axios";
import { toast } from "react-toastify";
import { EXCEPTION_CODES } from "../../../../typings";

export class LoginUseCase {
    private client = guildaApiClient2;

    async handle(loginRequestDto: LoginRequestDto) {
        const { Password, Username } = loginRequestDto;

        const passwordAndUsernameMatch =
            Username.toUpperCase() == Password.toUpperCase();

        // const payload = {
        //     username: username.toUpperCase(),
        //     password: passwordAndUsernameMatch
        //         ? password.toUpperCase()
        //         : password,
        // };

        const payload = {
            Username: Username.toUpperCase(),
            Password: Password,
        };

        try {
            console.log("2");
            const { data } = await this.client.post<
                unknown,
                AxiosResponse<LoginResponseDto>,
                LoginRequestDto
            >("/Authentication", payload);

            const { token } = data;
            
            Cookies.set("firstLogin", JSON.stringify(data.fisrtLogin || ""));

            Cookies.set(jwtTokenKey, token);
            this.client.defaults.headers.common.Authorization = `Bearer ${token}`;
            guildaApiClient.defaults.headers.common.Authorization = `Bearer ${token}`;

            setTimeout(() => {
                window.location.href = window.location.href;
              }, 100); // 100ms de atraso

            return data;
        } catch (e) {
            if (isAxiosError(e)) {
                const errorCode = e?.response?.data?.code;
                let message = INTERNAL_SERVER_ERROR_MESSAGE;

                if (
                    errorCode === EXCEPTION_CODES.PASSWORD_NOT_MATCH ||
                    errorCode === EXCEPTION_CODES.NOT_FOUND
                ) {
                    message = "Usuário e senha não combinam";
                } else if(e?.response?.data) {
                    message = e?.response?.data;
                }

                toast.error(message);
            }
        }
    }
}
