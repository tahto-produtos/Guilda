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
using static ApiRepositorio.Controllers.ListOtherOperationalCampaignController;
using static ApiRepositorio.Controllers.ListAllOperationalCampaignController;
using static TokenService;

//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ListAllOperationalCampaignController : ApiController
    {// POST: api/Results

        public class listAllOperationalCampaign
        {
            public int IDCAMPAIGN { get; set; }
            public string NAME { get; set; }

        }


        [HttpGet]
        public IHttpActionResult PostResultsModel(string campaign)
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

            List<listAllOperationalCampaign> rmams = new List<listAllOperationalCampaign>();
            rmams = BancoListAllOperationalCampaign.listAllCampaign(campaign);

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(rmams);
        }

    }

    public class BancoListAllOperationalCampaign
    {

        public static List<listAllOperationalCampaign> listAllCampaign(string campaign)
        {

            List<listAllOperationalCampaign> retorno = new List<listAllOperationalCampaign>();

            string filter = returnTables.IsInteger(campaign) == true ? $" AND IDGDA_OPERATIONAL_CAMPAIGN = '{campaign}' " : $" AND NAME LIKE '%{campaign}%' ";

            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT IDGDA_OPERATIONAL_CAMPAIGN, NAME  ");
            sb.Append($"FROM GDA_OPERATIONAL_CAMPAIGN (NOLOCK) ");
            sb.Append($"WHERE DELETED_AT IS NULL {filter} ");


            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listAllOperationalCampaign rtn = new listAllOperationalCampaign();
                                rtn.IDCAMPAIGN = int.Parse(reader["IDGDA_OPERATIONAL_CAMPAIGN"].ToString());
                                rtn.NAME = reader["NAME"].ToString();

                                retorno.Add(rtn);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Trate a exceção aqui
                }
                connection.Close();
            }
            return retorno;
        }


    }
}
