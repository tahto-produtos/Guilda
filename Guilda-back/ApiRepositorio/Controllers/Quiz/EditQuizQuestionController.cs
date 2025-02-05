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
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class EditQuizQuestionController : ApiController
    {// POST: api/Results

        public class InputModel
        {
            public int IDGDA_QUIZ_QUESTION { get; set; }
            public int IDGDA_QUIZ_QUESTION_TYPE { get; set; }
            public string QUESTION { get; set; }
            public int TIME_ANSWER { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {
            int COLLABORATORID = 0;
            int PERSONAUSERID = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            COLLABORATORID = inf.collaboratorId;
            PERSONAUSERID = inf.personauserId;

            string idLog = Logs.InsertActionLogs("Edit Pergunta Quiz ", "GDA_QUIZ_QUESTION", COLLABORATORID.ToString());
            int IDGDA_QUIZ_QUESTION = inputModel.IDGDA_QUIZ_QUESTION;
            int IDGDA_QUIZ_QUESTION_TYPE = inputModel.IDGDA_QUIZ_QUESTION_TYPE;
            string QUESTION = inputModel.QUESTION;
            int TIME_ANSWER = inputModel.TIME_ANSWER;
            #region UPDATE  GDA_QUIZ_QUESTION
            int ValidatedTypeQuestion = BankEditQuizQuestion.ValidaType(IDGDA_QUIZ_QUESTION) ;
            if (IDGDA_QUIZ_QUESTION_TYPE != ValidatedTypeQuestion)
            {
                BankEditQuizQuestion.DeletedAnswerQuestion(IDGDA_QUIZ_QUESTION, PERSONAUSERID);
            }
            BankEditQuizQuestion.EditQuestionQuiz(IDGDA_QUIZ_QUESTION_TYPE, QUESTION, TIME_ANSWER, IDGDA_QUIZ_QUESTION);
            #endregion
            return Ok("Pergunta do Quiz atualizada com sucesso.");
            
        }
        // Método para serializar um DataTable em JSON



    }

    public class BankEditQuizQuestion
    {
        public static void EditQuestionQuiz(int IDGDA_QUIZ_QUESTION_TYPE, string QUESTION, int TIME_ANSWER, int IDGDA_QUIZ_QUESTION)
        {
            StringBuilder UpdateQuestion = new StringBuilder();
            UpdateQuestion.Append("UPDATE GDA_QUIZ_QUESTION SET  ");
            UpdateQuestion.Append($"IDGDA_QUIZ_QUESTION_TYPE = '{IDGDA_QUIZ_QUESTION_TYPE}', QUESTION = '{QUESTION}', TIME_ANSWER = '{TIME_ANSWER}'  ");
            UpdateQuestion.Append("WHERE  ");
            UpdateQuestion.Append($"IDGDA_QUIZ_QUESTION = {IDGDA_QUIZ_QUESTION}");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(UpdateQuestion.ToString(), connection))
                    {
                        command.ExecuteScalar();
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                  
                }
            }

        }

        public static void DeletedAnswerQuestion(int IdQuestion, int personaid)
        {
            StringBuilder DeletedAnswer = new StringBuilder();
            DeletedAnswer.Append("UPDATE GDA_QUIZ_ANSWER  ");
            DeletedAnswer.Append("SET DELETED_AT = GETDATE(), ");
            DeletedAnswer.Append($"DELETED_BY  = '{personaid}' ");
            DeletedAnswer.Append($"WHERE IDGDA_QUIZ_QUESTION = {IdQuestion} AND DELETED_AT IS NULL ");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(DeletedAnswer.ToString(), connection))
                    {
                        command.ExecuteScalar();
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    
                }
            }
        }

        public static int ValidaType (int IdQuestion)
        {
           int retorno = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT IDGDA_QUIZ_QUESTION_TYPE FROM GDA_QUIZ_QUESTION (NOLOCK) WHERE IDGDA_QUIZ_QUESTION = {IdQuestion}");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                retorno = reader["IDGDA_QUIZ_QUESTION_TYPE"] != DBNull.Value? Convert.ToInt32(reader["IDGDA_QUIZ_QUESTION_TYPE"].ToString()) :0;
                            }
                        }
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    
                }
            }
            return retorno;
        }
    }
}