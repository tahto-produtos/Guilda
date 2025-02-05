export interface QuizModel {
  codQuiz: number;
  createdAt: string;
  startedAt: string;
  endedAt: string;
  title: string;
  description: string;
  idCreator: number;
  required: number;
  sendedAt: string;
  nameCreator: string;
  idResponsible: number;
  nameResponsible: string;
  idDemandant: number;
  nameDemandant: string;
  status: string;
  Monetization: number;
  PercentMonetization: number;
}
