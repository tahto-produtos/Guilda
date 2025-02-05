export interface PostCreateModel {
    postMessage: string;
    postReference: number;
    expiredAt?: string;
    highlight?: boolean;
    visibility?: {
        sector?: number[];
        subSector?: number[];
        period?: number[];
        hierarchy?: number[];
        group?: number[];
        userId?: number[];
        client?: number[];
        homeOrFloor?: number[];
    };
}
