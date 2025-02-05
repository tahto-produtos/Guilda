import { Sector } from "./sector.model";

export class Indicator {
    id!: number;
    indicatorId!: number;
    name!: string;
    description!: string;
    createdAt?: string;
    sectors?: Sector[];
    status!: boolean;
    mathematicalExpression!: { expression: string };
    calculationType!: string;
    weight!: number;
    type!: string;
}
