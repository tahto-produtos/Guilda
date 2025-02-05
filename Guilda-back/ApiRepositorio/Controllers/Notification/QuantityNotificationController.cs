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
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class QuantityNotificationController : ApiController
    {// POST: api/Results
        public class QuantityNotification
        {
            public int quantity { get; set; }
        }

        [HttpGet]
        public IHttpActionResult GetResultsModel()
        {
            int personaId = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }

            personaId = inf.personauserId;
           

            QuantityNotification quantityNotification = BancoQuantityNotification.returnQuantityNotification(personaId);

            return Ok(quantityNotification);
        }
        // Método para serializar um DataTable em JSON


    }

    public class BancoQuantityNotification
    {
        public static QuantityNotificationController.QuantityNotification returnQuantityNotification(int personauserId)
        {

            QuantityNotificationController.QuantityNotification quantityNotification = new QuantityNotificationController.QuantityNotification();

            //Listo todas as postagens e repostagens do banco
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    //787450
                    //13180

                    string dtAg = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                    StringBuilder stb = new StringBuilder();

                    stb.AppendFormat("SELECT COUNT(0) AS QTD FROM GDA_NOTIFICATION_USER (NOLOCK) AS NU ");
                    stb.AppendFormat("INNER JOIN GDA_NOTIFICATION (NOLOCK) AS N ON N.IDGDA_NOTIFICATION = NU.IDGDA_NOTIFICATION ");
                    stb.AppendFormat("WHERE NU.DELETED_AT IS NULL AND  ");
                    stb.AppendFormat("NU.VIEWED_AT IS NULL AND  ");
                    stb.AppendFormat("NU.IDGDA_PERSONA_USER = {0} AND ", personauserId);
                    stb.AppendFormat("N.DELETED_AT IS NULL  AND ");
                    stb.AppendFormat("NU.DELETED_AT IS NULL ");
                    stb.AppendFormat("AND (N.ENDED_AT IS NULL OR GETDATE() <= N.ENDED_AT) ");

                    using (SqlCommand commandInsert = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                quantityNotification.quantity = reader["QTD"] != DBNull.Value ? Convert.ToInt32(reader["QTD"]) : 0;
                            }
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }


            return quantityNotification;
        }
    }
}