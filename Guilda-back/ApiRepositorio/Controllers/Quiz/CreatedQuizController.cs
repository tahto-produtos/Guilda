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
using static ApiRepositorio.Controllers.CreatedNotificationController;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class CreatedQuizController : ApiController
    {// POST: api/Results
        public class InputModelCreateQuiz
        {
            public int IDGDA_COLLABORATOR_DEMANDANT { get; set; }
            public int IDGDA_COLLABORATOR_RESPONSIBLE { get; set; }
            public string TITLE { get; set; }
            public string DESCRIPTION { get; set; }
            public string ORIENTATION { get; set; }
            public int REQUIRED { get; set; }
            public int MONETIZATION {  get; set; }
            public int PERCENT_MONETIZATION { get; set; }
            public DateTime? STARTED_AT { get; set; }
            public DateTime? ENDED_AT { get; set; }
            public VisibilityCreateQuiz visibility { get; set; }

        }

        public class VisibilityCreateQuiz
        {
            public List<int> sector { get; set; }
            public List<int> subSector { get; set; }
            public List<int> period { get; set; }
            public List<int> hierarchy { get; set; }
            public List<int> group { get; set; }
            public List<int> userId { get; set; }
            public List<int> client { get; set; }
            public List<int> homeOrFloor { get; set; }
            public List<int> site { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModelCreateQuiz inputModel)
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

            Logs.InsertActionLogs("Insert Quiz ", "GDA_QUIZ", COLLABORATORID.ToString());
            int IDQUIZ = 0;
            int IDGDA_COLLABORATOR_DEMANDANT = inputModel.IDGDA_COLLABORATOR_DEMANDANT;
            int IDGDA_COLLABORATOR_RESPONSIBLE = inputModel.IDGDA_COLLABORATOR_RESPONSIBLE;
            string TITLE = inputModel.TITLE;
            string DESCRIPTION = inputModel.DESCRIPTION;
            int REQUIRED = inputModel.REQUIRED;
            int MONETIZATION = inputModel.MONETIZATION;
            int PERCENT_MONETIZATION = inputModel.PERCENT_MONETIZATION;
            string STARTED_AT = inputModel.STARTED_AT != null ? inputModel.STARTED_AT?.ToString("yyyy-MM-dd HH:mm:ss") : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string ENDED_AT = inputModel.ENDED_AT != null ? inputModel.ENDED_AT?.ToString("yyyy-MM-dd HH:mm:ss") : null;

            string ORIENTATION = inputModel.ORIENTATION != null ? inputModel.ORIENTATION : "";

            string ENDED_AT_FILTER = ENDED_AT != null ? $"'{ENDED_AT}'" : "NULL";


            if (Convert.ToDateTime(STARTED_AT) <= Convert.ToDateTime(DateTime.Now))
            {
                return Ok($"Erro: Selecione uma data e hora de inicio maior que a data atual, para que de tempo de realizar toda a configuração do quiz com as criações de perguntas e respostas, antes de ser enviado.");
            }

            #region INSERT GDA_QUIZ
            StringBuilder InsertQuiz = new StringBuilder();
            InsertQuiz.Append("INSERT INTO GDA_QUIZ  ");
            InsertQuiz.Append("(IDGDA_COLLABORATOR_DEMANDANT, IDGDA_COLLABORATOR_RESPONSIBLE, TITLE, DESCRIPTION, REQUIRED, CREATED_AT, CREATED_BY, STARTED_AT, ENDED_AT, MONETIZATION, PERCENT_MONETIZATION, ORIENTATION)  ");
            InsertQuiz.Append("VALUES  ");
            InsertQuiz.Append($"('{IDGDA_COLLABORATOR_DEMANDANT}', '{IDGDA_COLLABORATOR_RESPONSIBLE}', '{TITLE}', '{DESCRIPTION}', '{REQUIRED}',  GETDATE(), {PERSONAUSERID}, '{STARTED_AT}',{ENDED_AT_FILTER}, '{MONETIZATION}', '{PERCENT_MONETIZATION}', '{ORIENTATION}')  ");
            InsertQuiz.Append("SELECT  @@IDENTITY AS 'IDGDA_QUIZ' ");
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
                                IDQUIZ = Convert.ToInt32(reader["IDGDA_QUIZ"].ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"Erro ao criar GDA_QUIZ: {ex.Message}");
                }
                connection.Close();
            }
            #endregion

            BancoCreateQuiz.insertVisibility(IDQUIZ, inputModel);
            return Ok(IDQUIZ);
            
        }
        // Método para serializar um DataTable em JSON

    }

    #region Banco

    public class BancoCreateQuiz
    {
        public static void insertVisibility(int codQuiz, CreatedQuizController.InputModelCreateQuiz inputPost)
        {
            //Sector
            if (inputPost.visibility.sector.Count > 0)
            {
                foreach (int item in inputPost.visibility.sector)
                {
                    inserVisibilityItem(codQuiz, 1, item);
                }
            }
            //SubSector
            if (inputPost.visibility.subSector.Count > 0)
            {
                foreach (int item in inputPost.visibility.subSector)
                {
                    inserVisibilityItem(codQuiz, 2, item);
                }
            }
            //Period
            if (inputPost.visibility.period.Count > 0)
            {
                foreach (int item in inputPost.visibility.period)
                {
                    inserVisibilityItem(codQuiz, 3, item);
                }
            }
            //Hierarchy
            if (inputPost.visibility.hierarchy.Count > 0)
            {
                foreach (int item in inputPost.visibility.hierarchy)
                {
                    inserVisibilityItem(codQuiz, 4, item);
                }
            }
            //Group
            if (inputPost.visibility.group.Count > 0)
            {
                foreach (int item in inputPost.visibility.group)
                {
                    inserVisibilityItem(codQuiz, 5, item);
                }
            }
            //UserId
            if (inputPost.visibility.userId.Count > 0)
            {
                foreach (int item in inputPost.visibility.userId)
                {
                    inserVisibilityItem(codQuiz, 6, item);
                }
            }
            //Client
            if (inputPost.visibility.client.Count > 0)
            {
                foreach (int item in inputPost.visibility.client)
                {
                    inserVisibilityItem(codQuiz, 7, item);
                }
            }
            //HomeOrFloor
            if (inputPost.visibility.homeOrFloor.Count > 0)
            {
                foreach (int item in inputPost.visibility.homeOrFloor)
                {
                    inserVisibilityItem(codQuiz, 8, item);
                }
            }

            if (inputPost.visibility.site.Count > 0)
            {
                foreach (int item in inputPost.visibility.site)
                {
                    inserVisibilityItem(codQuiz, 9, item);
                }
            }

        }

        public static void inserVisibilityItem(int codQuiz, int visibilityTipe, int idReferer)
        {
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {

                    StringBuilder sbInsert = new StringBuilder();
                    sbInsert.AppendFormat("INSERT INTO GDA_QUIZ_SEND_FILTER (IDGDA_QUIZ, IDGDA_PERSONA_POSTS_VISIBILITY_TYPE, ID_REFERER) ");
                    sbInsert.AppendFormat("VALUES ( ");
                    sbInsert.AppendFormat("{0}, ", codQuiz); //IDGDA_NOTIFICATION
                    sbInsert.AppendFormat("{0}, ", visibilityTipe); //IDGDA_PERSONA_POSTS_VISIBILITY_TYPE
                    sbInsert.AppendFormat("{0} ", idReferer); //ID_REFERER
                    sbInsert.AppendFormat(") ");

                    using (SqlCommand commandInsert = new SqlCommand(sbInsert.ToString(), connection))
                    {
                        commandInsert.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {
                }
                connection.Close();
            }
        }

    }


    #endregion
}