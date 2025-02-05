export interface LibraryNotification {
    codNotification: number;
    createdAt: string;
    startedAt: string | null;
    sendedAt: string | null;
    endedAt: string | null;
    title: string;
    notification: string;
    idCreator: number;
    nameCreator: string;
    active: number;
    edit: 0 | 1;
    url: string;
}
