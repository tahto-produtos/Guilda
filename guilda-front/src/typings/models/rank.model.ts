import { Group } from "./group.model";

export interface Rank {
  count: number;
  group: Group;
  percent: number;
}

export interface RankConsolidated {
  COUNT: number;
  GROUPNAME: string;
  IDGROUP: number;
  IMAGEMGROUP: string;
  PERCENT: number;
  ALIAS: string;
  DESCRIPTION: string;
}
