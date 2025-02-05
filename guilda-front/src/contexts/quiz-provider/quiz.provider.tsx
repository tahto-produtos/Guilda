import { ReactNode, useContext, useEffect, useState } from "react";
import { UserInfoContext } from "../user-context/user.context";
import Cookies from "js-cookie";
import { PermissionUseCase } from "src/modules/permissions/Permission.use-case";
import { PermissionModel } from "src/typings/models/permission.model";
import { toast } from "react-toastify";
import { QuizContext } from "./quiz.context";
import { MyQuizModel } from "src/typings/models/my-quiz.model";
import { useLoadingState } from "src/hooks";
import { LoadMyQuizUseCase } from "src/modules/quiz/use-cases/load-my-quiz.use-case";
import { useRouter } from "next/router";
import { ToastCustom } from "src/components/feedback/toast-custom/toast-custom";
import { ListFeedBackMandatoryUseCase } from "src/modules/feedback/use-cases/list-feedback-mandatory.use-case";

interface IProviderProps {
  children: ReactNode;
}

export function QuizProvider({ children }: IProviderProps) {
  const { myUser } = useContext(UserInfoContext);
  const [quizzes, setQuizzes] = useState<MyQuizModel[]>([]);
  const router = useRouter();
  const { finishLoading, isLoading, startLoading } = useLoadingState();
  const [selectedQuiz, setSelectedQuiz] = useState<MyQuizModel | null>(null);
  const [firstFetch, setFirstFetch] = useState<boolean>(false);
  const [feedbackMandatory, setFeedbackMandatory] = useState<{
    ids: number;
  } | null>(null);
  const userToken = Cookies.get("jwtToken");
  const firstLogin = Cookies.get("firstLogin");

  async function listFeedbackMandatories() {
    startLoading();

    new ListFeedBackMandatoryUseCase()
      .handle()
      .then((data) => {
        if (data[0]) {
          setFeedbackMandatory(data[0]);

          if (userToken && firstLogin !== ("true" || true)) {
            router.push(`/feedback/feedback-details?id=${data[0].ids}&sign=1`);
          }
        } else {
          setFeedbackMandatory(null);
        }
      })
      .catch(() => {})
      .finally(() => {
        finishLoading();
      });
  }

  useEffect(() => {
    selectedQuiz == null && router.push("/");
    selectedQuiz && router.push("/quiz/my-quiz/answer");
  }, [selectedQuiz]);

  useEffect(() => {
    const handleRouteChange = (url: string) => {
      if (
        selectedQuiz &&
        selectedQuiz.REQUIRED &&
        selectedQuiz.STATUS !== "Concluido" &&
        !url.includes("/my-quiz/answer") &&
        !url.includes("/reset-password")
      ) {
        if (userToken && firstLogin !== ("true" || true)) {
          router.push("/quiz/my-quiz/answer");
        }
      } else if (
        feedbackMandatory &&
        !url.includes("/feedback/feedback-details")
      ) {
        if (userToken && firstLogin !== ("true" || true)) {
          router.push(
            `/feedback/feedback-details?id=${feedbackMandatory.ids}&sign=1`
          );
        }
      }
    };

    router.events.on("routeChangeStart", handleRouteChange);

    return () => {
      router.events.off("routeChangeStart", handleRouteChange);
    };
  }, [router.events, selectedQuiz, feedbackMandatory]);

  async function listQuizzes() {

    const admView = Cookies.get("admViewActive")?.toString();
    
    if (admView == "false")
    {
      startLoading();

      new LoadMyQuizUseCase()
        .handle()
        .then((data) => {
          setQuizzes(data);
  
          const foundNotCouncluded = data.find(
            (item) => item.STATUS !== "Concluido"
          );
  
          if (!firstFetch && foundNotCouncluded) {
            ToastCustom({
              subtitle: "",
              title: "Você tem Quizzes não respondidos",
              isQuiz: true,
            });
          }
  
          const foundRequired = data.find(
            (item) => item.REQUIRED && item.STATUS !== "Concluido"
          );
          if (foundRequired) {
            setSelectedQuiz(foundRequired);
          } else {
            setSelectedQuiz(null);
            listFeedbackMandatories();
          }
  
          setFirstFetch(true);
        })
        .catch(() => {
          toast.error("Erro ao listar os hobbies.");
        })
        .finally(() => {
          finishLoading();
        });
    }
  }

  useEffect(() => {
    myUser && listQuizzes();
  }, [myUser]);

  function refreshQuizzes() {
    listQuizzes();
  }

  return (
    <QuizContext.Provider
      value={{
        quizzes,
        refreshQuizzes,
        setSelectedQuiz,
        selectedQuiz,
        listFeedbackMandatories,
      }}
    >
      {children}
    </QuizContext.Provider>
  );
}
