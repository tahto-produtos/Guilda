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
using static ApiRepositorio.Controllers.LoadMyNotificationController;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ListFeedBackNotReceivedController : ApiController
    {// POST: api/Results

        public class FeedBackNotReceived
        {
            public int IDGDA_FEEDBACK_USER { get; set; }
            public string NAME { get; set; }
            public string STATUS { get; set; }
            public string PROTOCOL { get; set; }    
            public string DATA_RESPOSTA { get; set; }   
            public string REASON { get; set; }
        }

        [HttpGet]
        public IHttpActionResult PostResultsModel()
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
            List<FeedBackNotReceived> rmams = new List<FeedBackNotReceived>();

            rmams = BancoListFeedBackNotReceived.listFeedBackNotReceived(personaid);

            return Ok(rmams);

        }
        // Método para serializar um DataTable em JSON


        #region Banco

        public class BancoListFeedBackNotReceived
        {
            public static List<FeedBackNotReceived> listFeedBackNotReceived(int PERSONAID)
            {
                List<FeedBackNotReceived> retorno = new List<FeedBackNotReceived>();
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT FU.IDGDA_FEEDBACK_USER, ");
                sb.Append(" PU.NAME,  ");
                sb.Append("	   CASE WHEN SIGNED_RECEIVED IS NULL THEN 'Enviado' END AS STATUS, ");
                sb.Append("	   CASE WHEN RESPONDED_AT IS NULL THEN '-' END AS DATA_RESPOSTA, ");
                sb.Append("	   PROTOCOL, ");
                sb.Append("	   F.REASON ");
                sb.Append("FROM GDA_FEEDBACK_USER (NOLOCK) FU ");
                sb.Append("INNER JOIN GDA_PERSONA_USER (NOLOCK) AS PU ON PU.IDGDA_PERSONA_USER = FU.IDPERSONA_RECEIVED_BY ");
                sb.Append("INNER JOIN GDA_FEEDBACK (NOLOCK) AS F ON F.IDGDA_FEEDBACK = FU.IDGDA_FEEDBACK ");
                sb.Append("WHERE SIGNED_RECEIVED IS NULL  ");
                sb.Append($"AND IDPERSONA_RECEIVED_BY = {PERSONAID}  ");
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
                                    FeedBackNotReceived FeedBAckNotReceived = new FeedBackNotReceived();

                                    FeedBAckNotReceived.IDGDA_FEEDBACK_USER = reader["IDGDA_FEEDBACK_USER"] != DBNull.Value ? int.Parse(reader["NAME"].ToString()) : 0;
                                    FeedBAckNotReceived.NAME = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "";
                                    FeedBAckNotReceived.STATUS = reader["STATUS"] != DBNull.Value ? reader["STATUS"].ToString() : "";
                                    FeedBAckNotReceived.DATA_RESPOSTA = reader["DATA_RESPOSTA"] != DBNull.Value ? reader["DATA_RESPOSTA"].ToString() : "";
                                    FeedBAckNotReceived.PROTOCOL = reader["PROTOCOL"] != DBNull.Value ? reader["PROTOCOL"].ToString() : "";
                                    FeedBAckNotReceived.REASON = reader["REASON"] != DBNull.Value ? reader["REASON"].ToString() : "";
                                    retorno.Add(FeedBAckNotReceived);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }

                return retorno;
            }
            #endregion

        }


    }
}