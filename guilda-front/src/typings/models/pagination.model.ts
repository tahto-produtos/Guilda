export interface PaginationModel {
    limit: number;
    offset: number;
    searchText?: string;
    startDate?: string | null;
    endDate?: string | null;
    code?: number;
}
