export interface QuizDetail {
  IDGDA_QUIZ: number;
  TITLE: string;
  QTD_QUESTION: number;
  QTD_ANSWER: number;
  PERCENT: number;
  QUESTION: {
    IDGDA_QUIZ_QUESTION: number;
    IDGDA_QUIZ: number;
    IDTYPE: number;
    ORIENTATION: string;
    IDGDA_TYPE: number;
    TYPE: string;
    QUESTION: string;
    URL_QUESTION: string;
    TIME_ANSWER: number;
    ANSWER: {
      IDGDA_QUIZ_QUESTION: number;
      IDGDA_QUIZ_ANSWER: number;
      QUESTION: string;
      RIGHT_ANSWER: number;
      URL: string;
    }[];
  }[];
}
