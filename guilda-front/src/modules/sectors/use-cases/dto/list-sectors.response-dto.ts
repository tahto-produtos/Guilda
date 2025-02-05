import { Sector } from "src/typings";

export class ListSectorsResponseDto {
  items!: Sector[];
  totalItems!: number;
}
