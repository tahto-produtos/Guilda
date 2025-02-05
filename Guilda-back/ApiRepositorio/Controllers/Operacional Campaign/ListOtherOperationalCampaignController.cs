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
using static TokenService;

//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ListOtherOperationalCampaignController : ApiController
    {// POST: api/Results

        public class listOtherOperationalCampaign
        {
            public double TOTALCOST { get; set; }
            public int PARTICIPATINGOPERATORS { get; set; }
            public double GRIP { get; set; }

        }


        [HttpGet]
        public IHttpActionResult PostResultsModel(int codCampanha)
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

            listOtherOperationalCampaign rmams = new listOtherOperationalCampaign();
            rmams = BancoListOtherOperationalCampaign.listCampaign(codCampanha);

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(rmams);
        }

    }

    public class BancoListOtherOperationalCampaign
    {

        public static listOtherOperationalCampaign listCampaign(int idCampaign)
        {

            listOtherOperationalCampaign retorno = new listOtherOperationalCampaign();


            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT COUNT(0) AS QTD,  ");
            sb.Append($"SUM(VALUE_COINS) AS VALUE,  ");
            sb.Append($"((SELECT TOP 1 VALUE_COINS FROM GDA_OPERATIONAL_CAMPAIGN_AWARD WHERE IDGDA_OPERATIONAL_CAMPAIGN = {idCampaign} AND RANKING = 1) * COUNT(0)) AS GRIP ");
            sb.Append($"FROM GDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT (NOLOCK) AS UP ");
            sb.Append($"LEFT JOIN GDA_OPERATIONAL_CAMPAIGN_AWARD (NOLOCK) AS CA ON UP.POSITION = CA.RANKING AND UP.IDGDA_OPERATIONAL_CAMPAIGN = CA.IDGDA_OPERATIONAL_CAMPAIGN ");
            sb.Append($"WHERE UP.IDGDA_OPERATIONAL_CAMPAIGN = {idCampaign} ");


            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {



                                retorno.PARTICIPATINGOPERATORS= reader["QTD"] != DBNull.Value ? int.Parse(reader["QTD"].ToString()) : 0;
                                retorno.TOTALCOST = reader["VALUE"] != DBNull.Value ? double.Parse(reader["VALUE"].ToString()) : 0;
                                double MAXVALUE = reader["GRIP"] != DBNull.Value ? double.Parse(reader["GRIP"].ToString()) : 0;

                                if (retorno.PARTICIPATINGOPERATORS != 0 && MAXVALUE != 0)
                                {
                                    retorno.GRIP = (retorno.TOTALCOST / MAXVALUE) * 100;
                                }
                                else
                                {
                                    retorno.GRIP = 0;
                                }

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
