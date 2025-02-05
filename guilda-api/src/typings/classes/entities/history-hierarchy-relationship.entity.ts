import { HistoryHierarchyRelationship } from '@prisma/client';
import { ApiProperty } from '@nestjs/swagger';
export class HistoryHierarchyRelationshipEntity
  implements HistoryHierarchyRelationship
{
  @ApiProperty()
  id: number;

  @ApiProperty()
  contractorControlId: number;

  @ApiProperty()
  collaboratorId: number;

  @ApiProperty()
  hierarchyId: number;

  @ApiProperty()
  parentId: number;

  @ApiProperty()
  transactionId: number | null;

  @ApiProperty()
  levelName: string | null;

  @ApiProperty()
  levelWeight: number | null;

  @ApiProperty()
  date: Date | null;

  @ApiProperty()
  createdAt: Date;

  @ApiProperty({ nullable: true })
  deletedAt: Date | null;

  constructor(historyHierarchyRelationship: HistoryHierarchyRelationship) {
    const {
      id,
      contractorControlId,
      collaboratorId,
      hierarchyId,
      parentId,
      createdAt,
      deletedAt,
    } = historyHierarchyRelationship;
    this.id = id;
    this.contractorControlId = contractorControlId;
    this.collaboratorId = collaboratorId;
    this.hierarchyId = hierarchyId;
    this.parentId = parentId;
    this.createdAt = createdAt;
    this.deletedAt = deletedAt;
  }
}
