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
    public class EditQuizController : ApiController
    {// POST: api/Results
        public class InputModel
        {
            public int IDGDA_QUIZ { get; set; }
            public string TITLE { get; set; }
            public string DESCRIPTION { get; set; }
            public int REQUIRED { get; set; }
            public int MONETIZATION { get; set; }
            public int PERCENT_MONETIZATION { get; set; }
            public int IDGDA_COLLABORATOR_DEMANDANT { get; set; }
            public int IDGDA_COLLABORATOR_RESPONSIBLE { get; set; }
            public DateTime STARTED_AT { get; set; }
            public DateTime ENDED_AT { get; set; }
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

            string idLog = Logs.InsertActionLogs("Atualizar  Quiz ", "GDA_QUIZ", COLLABORATORID.ToString());
            int IDGDA_QUIZ = inputModel.IDGDA_QUIZ;
            string TITLE = inputModel.TITLE;
            string DESCRIPTION = inputModel.DESCRIPTION;
            int REQUIRED = inputModel.REQUIRED;
            int IDGDA_COLLABORATOR_DEMANDANT = inputModel.IDGDA_COLLABORATOR_DEMANDANT;
            int IDGDA_COLLABORATOR_RESPONSIBLE = inputModel.IDGDA_COLLABORATOR_RESPONSIBLE;
            string STARTED_AT = inputModel.STARTED_AT.ToString("yyyy-MM-dd HH:mm:ss");
            string ENDED_AT = inputModel.ENDED_AT.ToString("yyyy-MM-dd HH:mm:ss");
            int MONETIZATION = inputModel.MONETIZATION;
            int PERCENT_MONETIZATION = inputModel.PERCENT_MONETIZATION;

            #region UPDATE GDA_QUIZ
            StringBuilder UpdateQuiz = new StringBuilder();
            UpdateQuiz.Append("UPDATE GDA_QUIZ SET  ");
            UpdateQuiz.Append($"TITLE = '{TITLE}', DESCRIPTION = '{DESCRIPTION}', REQUIRED = '{REQUIRED}', IDGDA_COLLABORATOR_DEMANDANT = '{IDGDA_COLLABORATOR_DEMANDANT}' ,IDGDA_COLLABORATOR_RESPONSIBLE = '{IDGDA_COLLABORATOR_RESPONSIBLE}', STARTED_AT = '{STARTED_AT}', ENDED_AT = '{ENDED_AT}', MONETIZATION = '{MONETIZATION}', PERCENT_MONETIZATION= '{PERCENT_MONETIZATION}'   ");
            UpdateQuiz.Append("WHERE  ");
            UpdateQuiz.Append($"IDGDA_QUIZ = {IDGDA_QUIZ}");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(UpdateQuiz.ToString(), connection))
                    {
                        command.ExecuteScalar();
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    return BadRequest($"Erro ao Atualizar quiz GDA_QUIZ: {ex.Message}");
                }
            }
            #endregion
            return Ok("Quiz atualizado com sucesso.");

        }
    }
}