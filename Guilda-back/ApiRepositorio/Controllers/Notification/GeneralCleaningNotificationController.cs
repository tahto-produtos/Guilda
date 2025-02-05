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
using static ApiRepositorio.Controllers.MarkNotificationReadController;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class GeneralCleaningNotificationController : ApiController
    {// POST: api/Results
        public class inputGeneralCleaningNotification
        {
            public List<int> ids { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] inputGeneralCleaningNotification inputModel)
        {
            int collaboratorId = 0;
            int personaId = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            collaboratorId = inf.collaboratorId;
            personaId = inf.personauserId;

            BancoGeneralCleaningNotification.GeneralCleaning(inputModel, personaId);

            return Ok("Notificação atualizada com sucesso.");
            
        }
        // Método para serializar um DataTable em JSON

        public class BancoGeneralCleaningNotification
        {
            #region Banco

            public static int GeneralCleaning(inputGeneralCleaningNotification inputModel, int personaId)
            {

                string idEnvs = string.Join(",", inputModel.ids);

                int retornoId = 0;
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        string query = $"UPDATE GDA_NOTIFICATION SET DELETED_AT = GETDATE(), DELETED_BY = {personaId} WHERE IDGDA_NOTIFICATION IN ({idEnvs}) ";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }
                return retornoId;
            }

            #endregion
        }
    }
}