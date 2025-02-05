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
    public class CreatedQuizQuestionController : ApiController
    {// POST: api/Results

        public class InputModel
        {
            public int IDGDA_QUIZ { get; set; }
            public int IDGDA_QUIZ_QUESTION_TYPE { get; set; }
            public string QUESTION { get; set; }
            public int TIME_ANSWER { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel()
        {
            int COLLABORATORID = 0;
            int PERSONAUSERID = 0;
            string FOTO = "";
            string IDQUESTION = "";
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            COLLABORATORID = inf.collaboratorId;
            PERSONAUSERID = inf.personauserId;

            string idLog = Logs.InsertActionLogs("Insert Pergunta Quiz ", "GDA_QUIZ_QUESTION", COLLABORATORID.ToString());
            string jsonFromFormData = System.Web.HttpContext.Current.Request.Form["json"];
            InputModel Json = JsonConvert.DeserializeObject<InputModel>(jsonFromFormData);
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            List<GalleryResponseModel> pictures = PictureClass.UploadFilesToBlob(files);
            int IDGDA_QUIZ = Json.IDGDA_QUIZ;
            int IDGDA_QUIZ_QUESTION_TYPE = Json.IDGDA_QUIZ_QUESTION_TYPE;
            string QUESTION = Json.QUESTION;
            int TIME_ANSWER = Json.TIME_ANSWER;
            #region INSERT GDA_QUIZ
            StringBuilder InsertQuestion = new StringBuilder();
            InsertQuestion.Append("INSERT INTO GDA_QUIZ_QUESTION  ");
            InsertQuestion.Append("(IDGDA_QUIZ, IDGDA_QUIZ_QUESTION_TYPE, QUESTION, TIME_ANSWER, CREATED_AT, CREATED_BY)  ");
            InsertQuestion.Append("VALUES  ");
            InsertQuestion.Append($"('{IDGDA_QUIZ}', '{IDGDA_QUIZ_QUESTION_TYPE}', '{QUESTION}', '{TIME_ANSWER}', GETDATE(), '{PERSONAUSERID}')  ");
            InsertQuestion.Append("SELECT  @@IDENTITY AS 'IDGDA_QUIZ_QUESTION' ");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(InsertQuestion.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                IDQUESTION = reader["IDGDA_QUIZ_QUESTION"].ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"Erro ao criar GDA_QUIZ_QUESTION: {ex.Message}");
                }
                connection.Close();
            }
            #endregion
            if (files.Count > 0)
            {
                FOTO = pictures.First().url;
                #region INSERT GDA_QUIZ_QUESTION_FILES
                StringBuilder InsertAnswerFiles = new StringBuilder();
                InsertAnswerFiles.Append("INSERT INTO GDA_QUIZ_QUESTION_FILES  ");
                InsertAnswerFiles.Append("(IDGDA_QUIZ_QUESTION, URL ) ");
                InsertAnswerFiles.Append("VALUES  ");
                InsertAnswerFiles.Append($"('{IDQUESTION}', '{FOTO}' )  ");
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(InsertAnswerFiles.ToString(), connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"Erro ao inserir  foto da pegunta GDA_QUIZ_QUESTION_FILES: {ex.Message}");
                    }
                }

                #endregion

            }
            return Ok("Pergunta do Quiz criada com sucesso.");
            
        }
        // Método para serializar um DataTable em JSON

    }
}