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
using static ApiRepositorio.Controllers.LoadLibraryNotificationController;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ListMyFeedBackMandatoryController : ApiController
    {// POST: api/Results

        public class ReturnFeedBack
        {
            public int ids { get; set; }

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

            List<ReturnFeedBack> rmams = new List<ReturnFeedBack>();

            rmams = BancoListFeedBackMandatory.listFeedBackMandatory(personaid);

            return Ok(rmams);
        }
        // Método para serializar um DataTable em JSON


        #region Banco

        public class BancoListFeedBackMandatory
        {

        

            public static List<ReturnFeedBack> listFeedBackMandatory(int personaId)
            {
                List<ReturnFeedBack> ListFeedBack = new List<ReturnFeedBack>();
               
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT FU.IDGDA_FEEDBACK_USER, ");
                sb.Append("	   PU.NAME,  ");
                sb.Append("	   PROTOCOL ");
                sb.Append("FROM GDA_FEEDBACK_USER (NOLOCK) FU ");
                sb.Append("INNER JOIN GDA_PERSONA_USER (NOLOCK) AS PU ON PU.IDGDA_PERSONA_USER = FU.IDPERSONA_RECEIVED_BY ");
                sb.Append("INNER JOIN GDA_FEEDBACK (NOLOCK) AS F ON F.IDGDA_FEEDBACK = FU.IDGDA_FEEDBACK ");
                sb.Append("WHERE 1 = 1 ");
                sb.Append($"AND IDPERSONA_RECEIVED_BY = {personaId} AND SIGNED_RECEIVED IS NULL ");
                sb.Append("ORDER BY FU.IDGDA_FEEDBACK_USER DESC ");

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
                                    ReturnFeedBack rb = new ReturnFeedBack();
                                    rb.ids = reader["IDGDA_FEEDBACK_USER"] != DBNull.Value ? int.Parse(reader["IDGDA_FEEDBACK_USER"].ToString()) : 0;

                                    ListFeedBack.Add(rb);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }

                return ListFeedBack;
            }
            #endregion

        }


    }
}