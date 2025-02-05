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
using static ApiRepositorio.Controllers.ListSimulatorOperationalCampaignController;
using static TokenService;

//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ListSimulatorOperationalCampaignController : ApiController
    {// POST: api/Results

        public class ReturnSimulatorGetOperationalCampaign
        {
            public double costCampaign { get; set; }
            public List<ReturnSimulatorIndice> indices { get; set; }
        }

        public class ReturnSimulatorIndice
        {
            public string indice { get; set; }
            public int hc { get; set; }
            public int coinsMonth { get; set; }
            public double fullPotentialCoins { get; set; }
            public double fullPotentialTotal { get; set; }
            public double total60 { get; set; }
            public int evol { get; set; }
            public int payMonth { get; set; }
            public double range { get; set; }

        }

        [HttpGet]
        public IHttpActionResult GetResultsModel(int sector)
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

            ReturnSimulatorGetOperationalCampaign returnMyOperationalCampaign = new ReturnSimulatorGetOperationalCampaign();

            returnMyOperationalCampaign = BankSimulatorOperationalCampaign.returnListSimulatorGet(sector);
            
            return Ok(returnMyOperationalCampaign);
        }
        // Método para serializar um DataTable em JSON

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] ReturnSimulatorGetOperationalCampaign inputModel)
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

            ReturnSimulatorGetOperationalCampaign returnMyOperationalCampaign = new ReturnSimulatorGetOperationalCampaign();

            returnMyOperationalCampaign = BankSimulatorOperationalCampaign.returnListSimulatorPost(inputModel);

            return Ok(returnMyOperationalCampaign);
        }

    }

    public class BankSimulatorOperationalCampaign
    {
        public static ReturnSimulatorGetOperationalCampaign returnListSimulatorGet(int sector)
        {
            ReturnSimulatorGetOperationalCampaign lop = new ReturnSimulatorGetOperationalCampaign();
            lop.indices = new List<ReturnSimulatorIndice>();

            double costCampaign = 0;

            string dts = DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd");

            StringBuilder sb = new StringBuilder();

            sb.Append($"SELECT G.NAME,  ");
            sb.Append($"COUNT(DISTINCT IDGDA_COLLABORATORS) AS HC,  ");
            sb.Append($"SUM(CA.SOMA) AS COINS ");
            sb.Append($"FROM GDA_GROUPS (NOLOCK) G ");
            sb.Append($"LEFT JOIN ( ");
            sb.Append($"SELECT IDGDA_COLLABORATORS, MAX(IDGDA_GROUP) AS IDGDA_GROUP FROM GDA_COLLABORATORS_DETAILS (NOLOCK)  ");
            sb.Append($"WHERE CREATED_AT >= CONVERT(DATE, DATEADD(DAY, -2, GETDATE())) AND IDGDA_GROUP != 0 ");
            //sb.Append(" AND ACTIVE = 'true' ");
            sb.Append($"AND IDGDA_SECTOR = {sector} ");
            sb.Append($"GROUP BY IDGDA_COLLABORATORS ");
            sb.Append($") AS CD ON G.ID = CD.IDGDA_GROUP ");
            sb.Append($"LEFT JOIN ( ");
            sb.Append($"SELECT COLLABORATOR_ID, SUM(INPUT) AS SOMA FROM GDA_CHECKING_ACCOUNT (NOLOCK) ");
            sb.Append($"WHERE GDA_INDICATOR_IDGDA_INDICATOR IS NOT NULL ");
            sb.Append($"AND CONVERT(DATE, CREATED_AT) >= '{dts}' ");
            sb.Append($"GROUP BY COLLABORATOR_ID ");
            sb.Append($") AS CA ON CA.COLLABORATOR_ID = CD.IDGDA_COLLABORATORS ");
            sb.Append($"GROUP BY G.NAME  ");

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
                                ReturnSimulatorIndice rsi = new ReturnSimulatorIndice();
                                rsi.indice = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "";
                                rsi.hc = reader["HC"] != DBNull.Value ? Convert.ToInt32(reader["HC"]) : 0;
                                rsi.coinsMonth = reader["COINS"] != DBNull.Value ? Convert.ToInt32(reader["COINS"]) : 0;
                                rsi.fullPotentialCoins = rsi.hc * rsi.coinsMonth;
                                rsi.fullPotentialTotal = rsi.fullPotentialCoins * 0.03;
                                rsi.total60 = (rsi.hc-(rsi.hc * 0.4)) * rsi.coinsMonth * 0.03;
                                costCampaign += rsi.fullPotentialCoins;

                                lop.indices.Add(rsi);
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

            lop.costCampaign = costCampaign;

            return lop;
        }

        public static ReturnSimulatorGetOperationalCampaign returnListSimulatorPost(ReturnSimulatorGetOperationalCampaign inputModel)
        {

            foreach (ReturnSimulatorIndice rsi in inputModel.indices)
            {
                rsi.range = rsi.payMonth + (rsi.payMonth * rsi.evol);
            }

            return inputModel;
        }

    }


}