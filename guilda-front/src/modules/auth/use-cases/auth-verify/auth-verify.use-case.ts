import { guildaApiClient, guildaApiClient2 } from "../../../../services";
import Cookies from "js-cookie";
import {
    INTERNAL_SERVER_ERROR_MESSAGE,
    jwtTokenKey,
} from "../../../../constants";
import { AxiosResponse } from "axios";
import { isAxiosError } from "axios";
import { toast } from "react-toastify";
import { EXCEPTION_CODES } from "../../../../typings";
import { LoginRequestDto, LoginResponseDto } from "../login/dto";
import axios from "axios";
import { NEXT_PUBLIC_GUILDA_API_URL } from "src/constants/environment-variable.constants";

interface AuthVerifyProps {
    username: string;
    password: string;
    currentUserId: number;
}

export class AuthVerify {
    private client = guildaApiClient2;

    async handle(props: AuthVerifyProps) {
        const { username, password, currentUserId } = props;

        const authPayload = {
            Username: username,
            Password: password,
        };
console.log("1");
        const { data } = await this.client.post<
            unknown,
            AxiosResponse<LoginResponseDto>,
            LoginRequestDto
        >("/Authentication", authPayload);
        
        const { token } = data;

        if (token) {
            const tempClient = axios.create({
                baseURL: NEXT_PUBLIC_GUILDA_API_URL,
                headers: {
                    common: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${token}`,
                    },
                },
            });

            const { data } = await tempClient.get<unknown, AxiosResponse>(
                `/me`
            );

            if (currentUserId == data.id) {
                return true;
            } else {
                return false;
            }
        } else {
            return data;
        }
    }
}
