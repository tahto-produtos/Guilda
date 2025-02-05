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
    public class DeletedQuizAnswerController : ApiController
    {// POST: api/Results

        public class InputModel
        {
            public int IDGDA_QUIZ_ANSWER { get; set; }
            public bool VALIDATED { get; set; }
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

            string idLog = Logs.InsertActionLogs("Deletar Resposta Quiz ", "GDA_QUIZ_ANSWER", COLLABORATORID.ToString());
            int IDGDA_QUIZ_ANSWER = inputModel.IDGDA_QUIZ_ANSWER;
            bool VALIDATED = inputModel.VALIDATED;
            if (VALIDATED == false)
            {
                return BadRequest($"Deseja Seguir com a exclusão dessa resposta?");
            }
            else
            {
                #region UPDATE DELETAR GDA_QUIZ_ANSWER
                StringBuilder InsertNotification = new StringBuilder();
                InsertNotification.Append("UPDATE GDA_QUIZ_ANSWER SET  ");
                InsertNotification.Append($"DELETED_AT = GETDATE(), DELETED_BY = {PERSONAUSERID}  ");
                InsertNotification.Append("WHERE  ");
                InsertNotification.Append($"IDGDA_QUIZ_ANSWER = {IDGDA_QUIZ_ANSWER}");
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(InsertNotification.ToString(), connection))
                        {
                            command.ExecuteScalar();
                        }
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"Erro ao Deletar resposta GDA_QUIZ_ANSWER: {ex.Message}");
                    }
                }
                #endregion
                return Ok("Resposta do Quiz deletada com sucesso.");
            }


            
        }
        // Método para serializar um DataTable em JSON

    }
}