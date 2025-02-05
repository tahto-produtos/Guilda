export interface PermissionModel {
    id: number;
    action: string;
    resource: string;
    createdAt?: Date | string;
    deletedAt?: Date | string;
}