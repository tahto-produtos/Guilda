import { LogIndicatorImport } from '@prisma/client';
import { ApiProperty } from '@nestjs/swagger';

export class LogIndicatorImportEntity implements LogIndicatorImport {
  @ApiProperty()
  id: number;

  @ApiProperty()
  codeImport: number;

  @ApiProperty()
  code: number;

  @ApiProperty()
  name: string;

  @ApiProperty()
  description: string;

  @ApiProperty()
  type: string;

  @ApiProperty()
  status: boolean;

  @ApiProperty()
  weight: number;

  @ApiProperty()
  collaboratorId: number;

  @ApiProperty()
  createdAt: Date;

  @ApiProperty()
  deletedAt: Date;

  constructor(logIndicatorImport: LogIndicatorImport) {
    this.id = logIndicatorImport.id;
    this.codeImport = logIndicatorImport.codeImport;
    this.code = logIndicatorImport.code;
    this.name = logIndicatorImport.name;
    this.description = logIndicatorImport.description;
    this.status = logIndicatorImport.status;
    this.weight = logIndicatorImport.weight;
    this.collaboratorId = logIndicatorImport.collaboratorId;
  }
}
