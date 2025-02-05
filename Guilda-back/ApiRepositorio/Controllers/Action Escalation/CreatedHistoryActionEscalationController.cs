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
using DocumentFormat.OpenXml.Wordprocessing;
using static ApiRepositorio.Controllers.CreatedNotificationController;
using Antlr.Runtime.Misc;
using static ApiRepositorio.Controllers.LoadMyNotificationController;
using OfficeOpenXml.ConditionalFormatting;
using static ClosedXML.Excel.XLPredefinedFormat;
using DocumentFormat.OpenXml.Math;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class CreatedHistoryActionEscalationController : ApiController
    {// POST: api/Results

        public class InputModelCreatedHistoryActionEscalation
        {
            public int IDGDA_ESCALATION_ACTION { get; set; }
            public string ACTION_REALIZE { get; set; }
        }
        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModelCreatedHistoryActionEscalation inputModel)
        {
            int collaboratorId = 0;
            int personaid = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            collaboratorId = inf.collaboratorId;
            personaid = inf.personauserId;

            //INSERÇÃO DO FEEDBACK AO USUARIO DESTINADO
            BancoCreatedHistoryActionEscalation.InsertCreatedHistoryActionEscalation(inputModel, personaid);

            return Ok("Historico de ação criada com sucesso.");
            
        }
        // Método para serializar um DataTable em JSON


        #region Banco

        public class BancoCreatedHistoryActionEscalation
        {
           public static void InsertCreatedHistoryActionEscalation(InputModelCreatedHistoryActionEscalation inputModel, int personaId)
            {

                #region INSERT GDA_ESCALATION_HISTORY_ACTION_REALIZE
                StringBuilder sb = new StringBuilder();
                sb.Append("INSERT INTO GDA_ESCALATION_HISTORY_ACTION_REALIZE  ");
                sb.Append("(IDGDA_ESCALATION_ACTION, ACTION_REALIZE, CREATED_AT, CREATED_BY) ");
                sb.Append("VALUES  ");
                sb.Append($"('{inputModel.IDGDA_ESCALATION_ACTION}', '{inputModel.ACTION_REALIZE}', GETDATE(), '{personaId}' )  ");
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                #endregion
            }            
        }
        #endregion

    }



}