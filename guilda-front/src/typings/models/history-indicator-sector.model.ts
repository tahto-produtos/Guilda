import { Group } from "./group.model";


export class HistoryIndicatorSector {
  id!: number;
  indicatorId!: number;
  sectorId!: number;
  groupId!: number;
  metricMin!: number;
  metricMax!: number;
  createdAt!: Date;
  deletedAt!: Date;
  group?: Group;
  metrics?: any;
}
