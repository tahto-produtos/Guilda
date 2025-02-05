import { Indicator } from "./indicator.model";

export class Sector {
  id!: number;
  name!: string;
  level!: number;
  value!: string;
  createdAt!: string | Date;
  indicators?: Indicator[];
  isSector?: boolean;
  idsGroup?: number[];
}
