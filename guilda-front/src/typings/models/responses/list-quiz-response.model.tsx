import { QuizModel } from "../quiz.model";

export interface ListQuizResponseModel {
    totalpages: number;
    LoadLibraryQuiz: QuizModel[];
}
