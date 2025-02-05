export interface ActionDetails {
  IDGDA_ESCALATION_ACTION: number;
  AUTOMATIC: 0 | 1;
  IDGDA_INDICATOR: number;
  IDGDA_PERSONA_RESPONSIBLE_CREATION: number;
  IDGDA_PERSONA_RESPONSIBLE_ACTION: number;
  IDGDA_SECTOR: number;
  IDGDA_SUBSECTOR: number;
  ACTION_REALIZED: string;
  NAME: string;
  DESCRIPTION: string;
  STARTED_AT: string;
  ENDED_AT: string;
  STAGES: StageAction[];
  COLLABORATORS: ColabsAction[];
  IDGDA_GROUP: 4;
  PERCENT: 10;
  TOLERANCE: 1;
}

export interface StageAction {
  ID_STAGE: number;
  NUMBER_STAGE: number;
  ID_HIERARCHY: number;
  HIERARCHY: string;
}

export interface ColabsAction {
  ID: number;
  NAME: string;
}
