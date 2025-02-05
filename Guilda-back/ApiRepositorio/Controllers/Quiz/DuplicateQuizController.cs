using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ApiRepositorio.Class;
using ApiRepositorio.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Formatting;
using System.Web;
using ApiC.Class;
using System.Drawing.Imaging;
using System.Net.NetworkInformation;
using System.Web.UI;
using System.Xml.Linq;
using CommandLine;
using DocumentFormat.OpenXml.Spreadsheet;
using static ApiRepositorio.Controllers.FinancialSummaryController;
using static ApiRepositorio.Controllers.SearchAccountsController;
using static ApiRepositorio.Controllers.SendNotificationController;
using Microsoft.Diagnostics.Tracing.Parsers.FrameworkEventSource;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class DuplicateQuizController : ApiController
    {// POST: api/Results

        public class InputModel
        {
            public int IDGDA_QUIZ { get; set; }
        }
        public class DuplicateQuiz
        {
            public int IDGDA_COLLABORATOR_DEMANDANT { get; set; }
            public int IDGDA_COLLABORATOR_RESPONSIBLE { get; set; }
            public string TITLE { get; set; }
            public string DESCRIPTION { get; set; }
            public int REQUIRED { get; set; }
            public int MONETIZATION { get; set; }
            public int PERCENT_MONETIZATION { get; set; }
            public string STARTED_AT { get; set; }
            public string ENDED_AT { get; set; }

        }
        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {
            int COLLABORATORID = 0;
            int PERSONAUSERID = 0;
            int IDQUIZ = inputModel.IDGDA_QUIZ;
            int IDquizDuplicate = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            COLLABORATORID = inf.collaboratorId;
            PERSONAUSERID = inf.personauserId;

            Logs.InsertActionLogs("Duplicate Quiz ", "GDA_QUIZ", COLLABORATORID.ToString());
            List<DuplicateQuiz> QuizDuplicate = new List<DuplicateQuiz>();
            List<Question> ListQuestion = new List<Question>();
            List<Answer> ListAnswer = new List<Answer>();

            #region SELECT GDA_QUIZ
            StringBuilder SelectQuiz = new StringBuilder();
            SelectQuiz.Append("SELECT  ");
            SelectQuiz.Append("Q.IDGDA_QUIZ, ");
            SelectQuiz.Append("Q.IDGDA_COLLABORATOR_DEMANDANT, ");
            SelectQuiz.Append("Q.IDGDA_COLLABORATOR_RESPONSIBLE, ");
            SelectQuiz.Append("Q.TITLE, ");
            SelectQuiz.Append("Q.DESCRIPTION, ");
            SelectQuiz.Append("Q.REQUIRED, ");
            SelectQuiz.Append("Q.STARTED_AT, ");
            SelectQuiz.Append("Q.ENDED_AT, ");
            SelectQuiz.Append("Q.MONETIZATION, ");
            SelectQuiz.Append("Q.PERCENT_MONETIZATION, ");
            SelectQuiz.Append("QQ.IDGDA_QUIZ_QUESTION, ");
            SelectQuiz.Append("QQ.QUESTION AS PERGUNTA, ");
            SelectQuiz.Append("QQ.IDGDA_QUIZ_QUESTION_TYPE, ");
            SelectQuiz.Append("QQ.TIME_ANSWER, ");
            SelectQuiz.Append("ISNULL(QA.IDGDA_QUIZ_ANSWER,0) AS IDGDA_QUIZ_ANSWER,  ");
            SelectQuiz.Append("ISNULL(QA.IDGDA_QUIZ_QUESTION,0) AS PERGUNTA_DA_RESPOSTA, ");
            SelectQuiz.Append("QA.QUESTION AS RESPOSTA, ");
            SelectQuiz.Append("ISNULL(QA.RIGHT_ANSWER,0) AS RIGHT_ANSWER, ");
            SelectQuiz.Append("QAF.URL ");
            SelectQuiz.Append("FROM GDA_QUIZ (NOLOCK) Q ");
            SelectQuiz.Append("LEFT JOIN GDA_QUIZ_QUESTION (NOLOCK) AS QQ ON QQ.IDGDA_QUIZ = Q.IDGDA_QUIZ ");
            SelectQuiz.Append("LEFT JOIN GDA_QUIZ_ANSWER (NOLOCK) AS QA ON QA.IDGDA_QUIZ_QUESTION = QQ.IDGDA_QUIZ_QUESTION ");
            SelectQuiz.Append("LEFT JOIN GDA_QUIZ_ANSWER_FILES (NOLOCK) AS QAF ON QAF.IDGDA_QUIZ_ANSWER = QA.IDGDA_QUIZ_ANSWER ");
            SelectQuiz.Append($"WHERE Q.IDGDA_QUIZ = {IDQUIZ} ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(SelectQuiz.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int questionId = reader["IDGDA_QUIZ_QUESTION"] !=  DBNull.Value ? Convert.ToInt32(reader["IDGDA_QUIZ_QUESTION"].ToString()) : 0;  
                                if (!ListQuestion.Any(q => q.IDGDA_QUIZ_QUESTION == questionId))
                                {
                                    //PEGO AS INFORMAÇÕES DO QUIZ QUE VOU DUPLICAR
                                    if (QuizDuplicate.Count == 0)
                                    {
                                        DuplicateQuiz quiz = new DuplicateQuiz();
                                        quiz.IDGDA_COLLABORATOR_DEMANDANT = reader["IDGDA_COLLABORATOR_DEMANDANT"] != DBNull.Value ? int.Parse(reader["IDGDA_COLLABORATOR_DEMANDANT"].ToString()) : 0;
                                        quiz.IDGDA_COLLABORATOR_RESPONSIBLE = reader["IDGDA_COLLABORATOR_RESPONSIBLE"] != DBNull.Value ?  int.Parse(reader["IDGDA_COLLABORATOR_RESPONSIBLE"].ToString()) : 0;
                                        quiz.TITLE = reader["TITLE"] != DBNull.Value ?  reader["TITLE"].ToString() : "";
                                        quiz.DESCRIPTION = reader["DESCRIPTION"] != DBNull.Value ? reader["DESCRIPTION"].ToString() : "";
                                        quiz.REQUIRED = reader["REQUIRED"] != DBNull.Value ?  int.Parse(reader["REQUIRED"].ToString()) :0;
                                        quiz.STARTED_AT = reader["STARTED_AT"] != DBNull.Value ? reader["STARTED_AT"].ToString() : "";
                                        quiz.ENDED_AT = reader["ENDED_AT"] != DBNull.Value ? reader["ENDED_AT"].ToString() : "";
                                        quiz.MONETIZATION = reader["MONETIZATION"] != DBNull.Value ? int.Parse(reader["MONETIZATION"].ToString()) : 0;
                                        quiz.PERCENT_MONETIZATION = reader["PERCENT_MONETIZATION"] != DBNull.Value ? int.Parse(reader["PERCENT_MONETIZATION"].ToString()) : 0;
                                        QuizDuplicate.Add(quiz);
                                    }
                                    if (reader["IDGDA_QUIZ_QUESTION"] != DBNull.Value && int.Parse(reader["IDGDA_QUIZ_QUESTION"].ToString()) != 0)
                                    {
                                        Question perguntas = new Question();
                                        perguntas.IDGDA_QUIZ_QUESTION = reader["IDGDA_QUIZ_QUESTION"] != DBNull.Value ? int.Parse(reader["IDGDA_QUIZ_QUESTION"].ToString()) : 0;
                                        perguntas.IDGDA_TYPE = reader["IDGDA_QUIZ_QUESTION_TYPE"] != DBNull.Value ? int.Parse(reader["IDGDA_QUIZ_QUESTION_TYPE"].ToString()) : 0;
                                        perguntas.QUESTION = reader["PERGUNTA"] != DBNull.Value ? reader["PERGUNTA"].ToString() : "";
                                        perguntas.TIME_ANSWER = reader["TIME_ANSWER"] != DBNull.Value ? int.Parse(reader["TIME_ANSWER"].ToString()) : 0;
                                        ListQuestion.Add(perguntas);
                                    }
                                }
                                if (reader["IDGDA_QUIZ_ANSWER"] != DBNull.Value && int.Parse(reader["IDGDA_QUIZ_ANSWER"].ToString()) != 0)
                                {
                                    Answer respostas = new Answer();
                                    respostas.IDGDA_QUIZ_ANSWER = reader["IDGDA_QUIZ_ANSWER"] != DBNull.Value ? int.Parse(reader["IDGDA_QUIZ_ANSWER"].ToString()) : 0;
                                    respostas.IDGDA_QUIZ_QUESTION = reader["PERGUNTA_DA_RESPOSTA"] != DBNull.Value ? int.Parse(reader["PERGUNTA_DA_RESPOSTA"].ToString()) : 0;
                                    respostas.QUESTION = reader["RESPOSTA"] != DBNull.Value ? reader["RESPOSTA"].ToString() : "";
                                    respostas.RIGHT_ANSWER = reader["RIGHT_ANSWER"] != DBNull.Value ? int.Parse(reader["RIGHT_ANSWER"].ToString()) : 0;
                                    respostas.URL = reader["URL"] != DBNull.Value ? reader["URL"].ToString() : "";
                                    ListAnswer.Add(respostas);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"Erro ao criar GDA_QUIZ: {ex.Message}");
                }
            }
            #endregion

            #region INSERT QUIZ 
            StringBuilder InsertQuiz = new StringBuilder();
            // Convertendo a data para o formato desejado (string)
            //DateTime startedAtFormatted = Convert.ToDateTime(QuizDuplicate[0].STARTED_AT.ToString());
            // Convertendo para DateTime
            //DateTime dataIni = Convert.ToDateTime(startedAtFormatted.ToString("dd/MM/yyyy HH:mm:ss"));

            // Formatando para o formato desejado (yyyy-MM-dd-HH-mm-ss)
            //string dataIniFormatada = startedAtFormatted.ToString("yyyy-MM-dd HH:mm:ss");

            // Convertendo a data para o formato desejado (string)
            //DateTime endedAtFormatted = Convert.ToDateTime(QuizDuplicate[0].ENDED_AT.ToString());
            // Convertendo para DateTime
            //DateTime dataFim = DateTime.ParseExact(endedAtFormatted, "dd/MM/yyyy HH:mm:ss", null);
            // Formatando para o formato desejado (yyyy-MM-dd-HH-mm-ss)
            //string dataFimFormatada = dataFim.ToString("yyyy-MM-dd HH:mm:ss");

            InsertQuiz.Append("INSERT INTO GDA_QUIZ  ");
            InsertQuiz.Append("(IDGDA_COLLABORATOR_DEMANDANT, IDGDA_COLLABORATOR_RESPONSIBLE, TITLE, DESCRIPTION, REQUIRED, CREATED_AT, CREATED_BY, MONETIZATION, PERCENT_MONETIZATION)  ");
            InsertQuiz.Append("VALUES  ");
            InsertQuiz.Append($"('{QuizDuplicate[0].IDGDA_COLLABORATOR_DEMANDANT}', '{QuizDuplicate[0].IDGDA_COLLABORATOR_RESPONSIBLE}', '{QuizDuplicate[0].TITLE}', '{QuizDuplicate[0].DESCRIPTION}', '{QuizDuplicate[0].REQUIRED}',  GETDATE(), {PERSONAUSERID}, '{QuizDuplicate[0].MONETIZATION}', '{QuizDuplicate[0].PERCENT_MONETIZATION}')  ");
            InsertQuiz.Append("SELECT  @@IDENTITY AS 'IDQUIZ' ");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(InsertQuiz.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                IDquizDuplicate = int.Parse(reader["IDQUIZ"].ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"Erro ao criar GDA_QUIZ: {ex.Message}");
                }
            }
            #endregion

            for (int i = 0; i < ListQuestion.Count; i++)
            {
                #region INSERT QUESTION
                StringBuilder InsertQuestion = new StringBuilder();
                InsertQuestion.Append("INSERT INTO GDA_QUIZ_QUESTION ");
                InsertQuestion.Append("(IDGDA_QUIZ, IDGDA_QUIZ_QUESTION_TYPE, QUESTION, TIME_ANSWER, CREATED_AT, CREATED_BY) ");
                InsertQuestion.Append("VALUES ");
                InsertQuestion.Append($"('{IDquizDuplicate}', '{ListQuestion[i].IDGDA_TYPE}', '{ListQuestion[i].QUESTION}', '{ListQuestion[i].TIME_ANSWER}', GETDATE(), '{PERSONAUSERID}') ");
                InsertQuestion.Append("SELECT  @@IDENTITY AS 'IDQUESTION' ");
                int questionId;
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(InsertQuestion.ToString(), connection))
                        {
                            questionId = Convert.ToInt32(command.ExecuteScalar());
                        }
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"Erro ao inserir pergunta: {ex.Message}");
                    }
                }
                #endregion

                int answerId;
                // Iterar sobre as respostas associadas à pergunta atual e inserir no banco de dados
                foreach (var answer in ListAnswer.Where(a => a.IDGDA_QUIZ_QUESTION == ListQuestion[i].IDGDA_QUIZ_QUESTION))
                {
                    #region INSERT ANSWER
                    StringBuilder InsertAnswer = new StringBuilder();
                    InsertAnswer.Append("INSERT INTO GDA_QUIZ_ANSWER ");
                    InsertAnswer.Append("(IDGDA_QUIZ_QUESTION, QUESTION, RIGHT_ANSWER, CREATED_AT, CREATED_BY) ");
                    InsertAnswer.Append("VALUES ");
                    InsertAnswer.Append($"('{questionId}', '{answer.QUESTION}', '{answer.RIGHT_ANSWER}', GETDATE(), '{PERSONAUSERID}') ");
                    InsertAnswer.Append("SELECT  @@IDENTITY AS 'IDQUIZ' ");
                    using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                    {
                        try
                        {
                            connection.Open();
                            using (SqlCommand command = new SqlCommand(InsertAnswer.ToString(), connection))
                            {
                                answerId = Convert.ToInt32(command.ExecuteScalar());
                            }
                        }
                        catch (Exception ex)
                        {
                            return BadRequest($"Erro ao inserir resposta: {ex.Message}");
                        }
                    }

                    if ( answer.URL != "" )
                    {
                        //INSERIR FOTO AQUI
                        StringBuilder InsertAnswerFiles = new StringBuilder();
                        InsertAnswerFiles.Append("INSERT INTO GDA_QUIZ_ANSWER_FILES ");
                        InsertAnswerFiles.Append("(IDGDA_QUIZ_ANSWER, URL) ");
                        InsertAnswerFiles.Append("VALUES ");
                        InsertAnswerFiles.Append($"('{answerId}', '{answer.URL}') ");
                        using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                        {
                            try
                            {
                                connection.Open();
                                using (SqlCommand command = new SqlCommand(InsertAnswerFiles.ToString(), connection))
                                {
                                    command.ExecuteScalar();
                                }
                            }
                            catch (Exception ex)
                            {
                                return BadRequest($"Erro ao inserir resposta: {ex.Message}");
                            }
                        }
                    }
                    #endregion
                }
            }
            return Ok("Quiz Duplicado com sucesso.");
        }
        // Método para serializar um DataTable em JSON

    }
}