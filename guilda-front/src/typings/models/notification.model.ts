export interface NotificationModel {
    codNotification: number;
    dateNotification: string;
    nameUser: string;
    pictureUser: string;
    hierarchyUser: string;
    texto: string;
    link_file: string;
    categoria: string;
    viewedAt: string | null;
    files: {
        url: string;
    }[];
}
