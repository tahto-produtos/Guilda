export class CreateIndicatorRequestDto {
    name!: string;
    description!: string;
    id!: number;
    weight?: number;
    calculationType?: string;
    expression?: string;
    sectorsIds!: number[];
    status!: string;
}
