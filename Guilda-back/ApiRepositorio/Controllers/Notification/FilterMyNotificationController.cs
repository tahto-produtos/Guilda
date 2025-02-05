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
    public class FilterMyNotificationController : ApiController
    {// POST: api/Results
        public class FiltersNotification
        {
            public int cod { get; set; }
            public string name { get; set; }
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

            List<FiltersNotification> quantityNotification = BancoFilterNotification.returnFilterNotification(personaId);

            return Ok(quantityNotification);
        }
        // Método para serializar um DataTable em JSON


    }

    public class BancoFilterNotification
    {
        public static List<FilterMyNotificationController.FiltersNotification> returnFilterNotification(int personauserId)
        {

            List<FilterMyNotificationController.FiltersNotification> filtersNotification = new List<FilterMyNotificationController.FiltersNotification>();

            //Adicionando os filtros Lido e Não Lido
            FilterMyNotificationController.FiltersNotification filterLido = new FilterMyNotificationController.FiltersNotification();
            filterLido.cod = 0;
            filterLido.name = "Lido";
            filtersNotification.Add(filterLido);
            FilterMyNotificationController.FiltersNotification filterNaoLido = new FilterMyNotificationController.FiltersNotification();
            filterNaoLido.cod = 0;
            filterNaoLido.name = "Não Lido";
            filtersNotification.Add(filterNaoLido);

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

                    stb.AppendFormat("SELECT IDGDA_NOTIFICATION_TYPE, TYPE FROM GDA_NOTIFICATION_TYPE (NOLOCK) ");
                    stb.AppendFormat("WHERE DELETED_AT IS NULL ");

                    using (SqlCommand commandInsert = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                FilterMyNotificationController.FiltersNotification filter = new FilterMyNotificationController.FiltersNotification();

                                filter.cod = reader["IDGDA_NOTIFICATION_TYPE"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_NOTIFICATION_TYPE"]) : 0;
                                filter.name = reader["TYPE"] != DBNull.Value ? reader["TYPE"].ToString() : "";
                                filtersNotification.Add(filter);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }


            return filtersNotification;
        }
    }
}