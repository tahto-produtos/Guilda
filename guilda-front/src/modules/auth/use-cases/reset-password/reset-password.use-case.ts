import { guildaApiClient } from "../../../../services";

interface ResetPasswordUseCaseProps {
    currentPassword: string;
    newPassword: string;
    confirmNewPassword: string;
}

export class ResetPasswordUseCase {
    private client = guildaApiClient;

    async handle(props: ResetPasswordUseCaseProps) {
        const { confirmNewPassword, currentPassword, newPassword } = props;

        const payload = {
            confirmNewPassword,
            currentPassword,
            newPassword,
        };

        const { data } = await this.client.post(
            "/collaborators/reset-password",
            payload
        );

        return data;
    }
}
