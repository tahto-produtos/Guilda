import axios from "axios";
import Cookies from "js-cookie";
import { NEXT_PUBLIC_GUILDA_API_URL, NEXT_PUBLIC_GUILDA_API_URL2 } from "../../../constants/environment-variable.constants";
import { jwtTokenKey } from "../../../constants/cookies-keys.contants";

//const NEXT_PUBLIC_GUILDA_API_URL2 = "https://betha.tahto.com.br:3002/api";

export const guildaApiClient = axios.create({
    baseURL: NEXT_PUBLIC_GUILDA_API_URL,
    headers: {
        common: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${Cookies.get(jwtTokenKey)}`,
        },
    },
});

export const guildaApiClient2 = axios.create({
    baseURL: NEXT_PUBLIC_GUILDA_API_URL2,
    headers: {
        common: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${Cookies.get(jwtTokenKey)}`,
        },
    },
});
