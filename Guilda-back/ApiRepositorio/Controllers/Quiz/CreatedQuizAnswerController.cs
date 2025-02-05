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
    public class CreatedQuizAnswerController : ApiController
    {// POST: api/Results

        public class InputModel
        {
            public int IDGDA_QUIZ_QUESTION { get; set; }
            public string QUESTION { get; set; }
            public int RIGHT_ANSWER { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel()
        {
            int COLLABORATORID = 0;
            int PERSONAUSERID = 0;
            string FOTO = "";
            string IDANSWER = "";
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            COLLABORATORID = inf.collaboratorId;
            PERSONAUSERID = inf.personauserId;

            Logs.InsertActionLogs("Insert Resposta Quiz ", "GDA_QUIZ_ANSWER", COLLABORATORID.ToString());
            string jsonFromFormData = System.Web.HttpContext.Current.Request.Form["json"];
            InputModel Json = JsonConvert.DeserializeObject<InputModel>(jsonFromFormData);
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            List<GalleryResponseModel> pictures = PictureClass.UploadFilesToBlob(files);
            int IDGDA_QUIZ_QUESTION = Json.IDGDA_QUIZ_QUESTION;
            string QUESTION = Json.QUESTION;
            int RIGHT_ANSWER = Json.RIGHT_ANSWER;
            #region INSERT GDA_QUIZ_ANSWER
            StringBuilder InsertAnswer = new StringBuilder();
            InsertAnswer.Append("INSERT INTO GDA_QUIZ_ANSWER  ");
            InsertAnswer.Append("(IDGDA_QUIZ_QUESTION, QUESTION, RIGHT_ANSWER, CREATED_AT, CREATED_BY)  ");
            InsertAnswer.Append("VALUES  ");
            InsertAnswer.Append($"('{IDGDA_QUIZ_QUESTION}', '{QUESTION}', '{RIGHT_ANSWER}', GETDATE(), '{PERSONAUSERID}')  ");
            InsertAnswer.Append("SELECT  @@IDENTITY AS 'IDGDA_QUIZ_QUESTION' ");
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
                               IDANSWER = reader["IDGDA_QUIZ_QUESTION"].ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"Erro ao criar resposta GDA_QUIZ_ANSWER: {ex.Message}");
                }
            }
            #endregion
            if (files.Count > 0)
            {
                FOTO = pictures.First().url;
                #region INSERT GDA_QUIZ_ANSWER_FILES
                StringBuilder InsertAnswerFiles = new StringBuilder();
                InsertAnswerFiles.Append("INSERT INTO GDA_QUIZ_ANSWER_FILES  ");
                InsertAnswerFiles.Append("(IDGDA_QUIZ_ANSWER, URL ) ");
                InsertAnswerFiles.Append("VALUES  ");
                InsertAnswerFiles.Append($"('{IDANSWER}', '{FOTO}' )  ");
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
                        return BadRequest($"Erro ao inserir  foto da resposta GDA_QUIZ_ANSWER_FILES: {ex.Message}");
                    }
                }

                #endregion

            }
            return Ok("Resposta do Quiz criada com sucesso.");
            
        }
        // Método para serializar um DataTable em JSON

    }
}