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
using DocumentFormat.OpenXml.Office2019.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;
using static ApiRepositorio.Controllers.SendQuizController;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class SendAnswerUserController : ApiController
    {// POST: api/Results
        public class InputModel
        {
            public int IDGDA_QUIZ_USER { get; set; }
            public int IDGDA_QUIZ_QUESTION { get; set; }
            public List<int> IDGDA_QUIZ_ANSWER { get; set; }
            //public int IDGDA_QUIZ_ANSWER { get; set; }
            public string ANSWER { get; set; }
            public int NO_ANSWER { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {
            int personauserId = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }

            personauserId = inf.personauserId;

            //BancoQuiz.SendAnswerUser(personauserId, inputModel.IDGDA_QUIZ_USER, inputModel.IDGDA_QUIZ_QUESTION, inputModel.IDGDA_QUIZ_ANSWER, inputModel.ANSWER, inputModel.NO_ANSWER);
            BancoQuiz.SendAnswerUser(personauserId, inputModel);
            return Ok("Resposta enviada com sucesso.");

        }
        // Método para serializar um DataTable em JSON

        public class BancoQuiz
        {
            #region Banco
            public static void SendAnswerUser(int personauserId, InputModel inputModel)
            {
                if (inputModel.IDGDA_QUIZ_ANSWER.Contains(0) && inputModel.ANSWER != "")
                {
                    #region INSERT GDA_QUIZ_ANSWER

                    StringBuilder InsertAnswer = new StringBuilder();
                    InsertAnswer.Append("INSERT INTO GDA_QUIZ_ANSWER  ");
                    InsertAnswer.Append("( IDGDA_QUIZ_QUESTION, QUESTION, RIGHT_ANSWER, CREATED_AT, CREATED_BY)  ");
                    InsertAnswer.Append("VALUES  ");
                    InsertAnswer.Append($"('{inputModel.IDGDA_QUIZ_QUESTION}', '{inputModel.ANSWER}', '1', GETDATE(), '{personauserId}')  ");
                    InsertAnswer.Append("SELECT  @@IDENTITY AS 'IDGDA_QUIZ_ANSWER' ");
                    using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                    {
                        try
                        {
                            connection.Open();
                            using (SqlCommand command = new SqlCommand(InsertAnswer.ToString(), connection))
                            {
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        //IDGDA_QUIZ_ANSWER = int.Parse(reader["IDGDA_QUIZ_ANSWER"].ToString());
                                        inputModel.IDGDA_QUIZ_ANSWER.Clear();
                                        inputModel.IDGDA_QUIZ_ANSWER.Add(int.Parse(reader["IDGDA_QUIZ_ANSWER"].ToString()));
                                        //inputModel.NO_ANSWER = 1;
                                    }

                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                        connection.Close();
                    }
                    #endregion
                }

                #region INSERT GDA_QUIZ_USER_ANSWER
                foreach ( int resposta in inputModel.IDGDA_QUIZ_ANSWER)
                {
                    int respUtil = 0;
                    respUtil = inputModel.NO_ANSWER == 1 ? 0 : resposta;

                    StringBuilder InsertUserAnswer = new StringBuilder();
                    InsertUserAnswer.Append("INSERT INTO GDA_QUIZ_USER_ANSWER  ");
                    InsertUserAnswer.Append("( IDGDA_QUIZ_USER, IDGDA_QUIZ_QUESTION, IDGDA_QUIZ_ANSWER, NO_ANSWER)  ");
                    InsertUserAnswer.Append("VALUES  ");
                    InsertUserAnswer.Append($"('{inputModel.IDGDA_QUIZ_USER}', '{inputModel.IDGDA_QUIZ_QUESTION}', '{respUtil}', '{inputModel.NO_ANSWER}')  ");
                    InsertUserAnswer.Append("SELECT  @@IDENTITY AS 'IDGDA_QUIZ_USER_ANSWER' ");
                    using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                    {
                        try
                        {
                            connection.Open();
                            using (SqlCommand command = new SqlCommand(InsertUserAnswer.ToString(), connection))
                            {
                                command.ExecuteScalar().ToString();
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                
                #endregion
            }
            #endregion Banco
        }
    }
}