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
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class EditNotificationController : ApiController
    {// POST: api/Results
        public class InputModel
        {
            public int NOTIFICATION_ID { get; set; }
            public string TITLE { get; set; }
            public string NOTIFICATION { get; set; }
            public int ACTIVATED { get; set; }
            public DateTime STARTED_AT { get; set; } 
            public DateTime ENDED_AT { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {
            int collaboratorId = 0;
            var token = Request.Headers.Authorization?.Parameter;
            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            collaboratorId = inf.collaboratorId;

            int NotificationID = inputModel.NOTIFICATION_ID;
            string Title = inputModel.TITLE;
            string Notification = inputModel.NOTIFICATION;
            int Active = inputModel.ACTIVATED;
            string dtInicial = inputModel.STARTED_AT.ToString("yyyy-MM-dd HH:mm:ss");
            string dtFinal = inputModel.ENDED_AT.ToString("yyyy-MM-dd HH:mm:ss");

                #region UPDATE GDA_NOTIFICATION
                StringBuilder UpdateNotification = new StringBuilder();
            UpdateNotification.Append("UPDATE GDA_NOTIFICATION SET   ");
            UpdateNotification.Append($"ACTIVE =  {Active} ");
            if (Title != "")
            {
                UpdateNotification.Append($", TITLE =  '{Title}' ");
            }
            if (Notification != "")
            {
                UpdateNotification.Append($", NOTIFICATION =  '{Notification}' ");
            }
            if (dtInicial != "")
            {
                UpdateNotification.Append($", STARTED_AT =  '{dtInicial}' ");
            }
            if (dtFinal != "")
            {
                UpdateNotification.Append($", ENDED_AT =  '{dtFinal}' ");
            }
            UpdateNotification.Append($"WHERE IDGDA_NOTIFICATION = {NotificationID} ");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(UpdateNotification.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"Erro ao ativar GDA_NOTIFICATION: {ex.Message}");
                }
            }
            #endregion


            return Ok("Notificação atualizada com sucesso.");
            
        }
        // Método para serializar um DataTable em JSON


    }
}