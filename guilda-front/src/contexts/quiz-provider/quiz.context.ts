import { createContext } from "react";
import { BasketData } from "src/typings/models/basket-data.model";
import { MyQuizModel } from "src/typings/models/my-quiz.model";
import { MyUser } from "src/typings/models/myuser.model";

export interface QuizContextData {
  quizzes: MyQuizModel[];
  refreshQuizzes: () => void;
  selectedQuiz: MyQuizModel | null;
  setSelectedQuiz: (input: MyQuizModel | null) => void;
  listFeedbackMandatories: () => void;
}

export const QuizContext = createContext<QuizContextData>(
  {} as QuizContextData
);
