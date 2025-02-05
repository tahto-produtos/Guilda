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
using DocumentFormat.OpenXml.Office2019.Presentation;
using System.Threading;
using static ApiRepositorio.Controllers.ReportOperationalCampaignController;
using static ApiRepositorio.Controllers.TesteController;
using static ApiRepositorio.Controllers.IntegracaoAPIResultController;
using static ApiRepositorio.Controllers.SimulatorOperationalCampaignController;
using static TokenService;

//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class DoImportantOperationalCampaignController : ApiController
    {// POST: api/Results

        public class inputImportantOperationalCampaign
        {
            public int idCampaign { get; set; }
        }

        [HttpPost]
        public IHttpActionResult GetResultsModel([FromBody] inputImportantOperationalCampaign inputModel)
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

            doImportantCampaign(inputModel.idCampaign, PERSONAUSERID);

            return Ok("");
        }
        // Método para serializar um DataTable em JSON



        public static void doImportantCampaign(int idCampaign, int idPersona)
        {
            StringBuilder sb2 = new StringBuilder();
            sb2.Append($"UPDATE GDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT SET IMPORTANT = 0 WHERE IDGDA_OPERATIONAL_CAMPAIGN != {idCampaign} AND IDGDA_PERSONA = {idPersona} ");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sb2.ToString(), connection))
                {
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }

            StringBuilder sb = new StringBuilder();
            sb.Append($"UPDATE GDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT SET IMPORTANT = 1 WHERE IDGDA_OPERATIONAL_CAMPAIGN = {idCampaign} AND IDGDA_PERSONA = {idPersona} ");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                {
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

    }


}
