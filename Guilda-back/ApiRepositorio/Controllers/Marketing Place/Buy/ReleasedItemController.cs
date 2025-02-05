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
using Azure.Storage.Blobs.Specialized;
using static ApiRepositorio.Controllers.BuyItemController;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using DocumentFormat.OpenXml.Math;
using System.Drawing;
using System.Threading;
using ApiC.Class.DowloadFile;
using DocumentFormat.OpenXml.Drawing.Charts;
using static ApiRepositorio.Controllers.ReleasedItemController;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{

    //[Authorize]
    public class ReleasedItemController : ApiController
    {// POST: api/Results

        public class releasedItemModel
        {
            public DateTime? releasedAt { get; set; }
            public string type { get; set; }
            public string nameColaborator { get; set; }
            public string comercialName { get; set; }

            public string idCollaborator { get; set; }
        }

        public class inputOrder
        {
            public int orderId { get; set; }
            public int productId { get; set; }
        }

        public class collaboratorReleasedModel
        {
            public string homeBased { get; set; }
            public string site { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] inputOrder inputModel)
        {
            try
            {
                int collaboratorId = 0;
                int personauserId = 0;
                var token = Request.Headers.Authorization?.Parameter;

                string produtos = "";
                string nomeOperador = "";
                string bcOperador = "";


                InfsUser inf = TokenService.TryDecodeToken(token);
                if (inf == null)
                {
                    return Unauthorized();
                }
                collaboratorId = inf.collaboratorId;
                personauserId = inf.personauserId;

                List<releasedItemModel> bcc = new List<releasedItemModel>();
                bcc = BancoReleasedOrder.returnBbcOrder(inputModel.orderId, inputModel.productId.ToString());
                bcOperador = "BC" + bcc.FirstOrDefault().idCollaborator;
                nomeOperador = bcc.FirstOrDefault().nameColaborator;

                collaboratorReleasedModel cbs = BancoReleasedOrder.getHome(collaboratorId);



                int collaboratorTypeDays = BancoReleasedOrder.getTypeDays(cbs.homeBased); //SELECT * FROM GDA_MKT_CONFIG WHERE DELETED_AT IS NULL AND TYPE = 'VENCIMENTO_LIBERADO_HOME'

                List<string> holidays = new List<string>();
                holidays = BancoReleasedOrder.getholidays(cbs.site); //SELECT * FROM GDA_HOLIDAYS WHERE DELETED_AT IS NULL AND SITE = 'CPE'

                foreach (var el in bcc)
                {
                    DateTime? releasedAt = el?.releasedAt;
                    DateTime? releasedAtSum = releasedAt;
                    int daysToAdd = 0;

                    if (el.type == "PHYSICAL" && releasedAt.HasValue)
                    {
                        // Validar se releasedAt é uma data válida
                        while (daysToAdd < collaboratorTypeDays)
                        {
                            releasedAtSum = releasedAtSum?.AddDays(1);

                            // Verifica se não é sábado ou domingo
                            if (releasedAtSum.HasValue && releasedAtSum.Value.DayOfWeek != DayOfWeek.Saturday && releasedAtSum.Value.DayOfWeek != DayOfWeek.Sunday)
                            {
                                // Converter a data para o fuso horário de São Paulo
                                TimeZoneInfo brazilZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
                                DateTime zonedReleasedAtSum = TimeZoneInfo.ConvertTimeFromUtc(releasedAtSum.Value.ToUniversalTime(), brazilZone);

                                // Verificar se a data após adicionar dias é válida
                                if (zonedReleasedAtSum != DateTime.MinValue)
                                {
                                    // Formatar a data para o formato yyyy-MM-dd
                                    string formattedDate = zonedReleasedAtSum.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

                                    // Verifica se a data não está na lista de feriados
                                    if (!holidays.Contains(formattedDate))
                                    {
                                        daysToAdd++;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"Data após adição de dias não é válida: {releasedAtSum}");
                                    break; // Sai do loop se a data não for válida
                                }
                            }
                        }
                        if (produtos != "")
                        {
                            produtos = $"{produtos}, {el.comercialName} - Expira em: {releasedAtSum}.";
                        }
                        else
                        {
                            produtos = $"{el.comercialName} - Expira em: {releasedAtSum}.";
                        }
                    }

                }

                if (produtos != "")
                {
                    //Insere notificação
                    ScheduledNotification.insertNotificationMktPlace(12, "Novo Pedido Liberado", $"O pedido do {nomeOperador} - {bcOperador} foi liberado. Produtos: {produtos}!", true, personauserId, true, collaboratorId);
                }

                return Ok("Ok");
            }
            catch (Exception)
            {
                return BadRequest("Não foi possivel realizar a compra!");
            }
        }
    }


    public class BancoReleasedOrder
    {

        public static List<ReleasedItemController.releasedItemModel> returnBbcOrder(int idOrder, string idProducts)
        {
            string filter = "";
            if (idProducts != "0")
            {
                filter = $" AND GDA_PRODUCT_IDGDA_PRODUCT = {idProducts} ";
            }



            List<ReleasedItemController.releasedItemModel> bcc = new List<ReleasedItemController.releasedItemModel>();
            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("SELECT OP.RELEASED_AT, TYPE, C.NAME, P.COMERCIAL_NAME, O.ORDER_BY FROM GDA_ORDER (NOLOCK) AS O ");
            stb.AppendFormat("INNER JOIN GDA_ORDER_PRODUCT (NOLOCK) AS OP ON O.IDGDA_ORDER = OP.GDA_ORDER_IDGDA_ORDER ");
            stb.AppendFormat("INNER JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = O.ORDER_BY ");
            stb.AppendFormat("INNER JOIN GDA_PRODUCT (NOLOCK) AS P ON P.IDGDA_PRODUCT = OP.GDA_PRODUCT_IDGDA_PRODUCT ");
            stb.AppendFormat($"WHERE IDGDA_ORDER = {idOrder} ");
            stb.AppendFormat($"{filter} ");


            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ReleasedItemController.releasedItemModel bc = new ReleasedItemController.releasedItemModel();
                                bc.releasedAt = reader["RELEASED_AT"] != DBNull.Value ? Convert.ToDateTime(reader["RELEASED_AT"].ToString()) : (DateTime?)null;
                                bc.type = reader["TYPE"].ToString();
                                bc.nameColaborator = reader["NAME"].ToString();
                                bc.comercialName = reader["COMERCIAL_NAME"].ToString();
                                bc.idCollaborator = reader["ORDER_BY"].ToString();
                                bcc.Add(bc);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return bcc;
        }


        public static collaboratorReleasedModel getHome(int idCollaborator)
        {
            StringBuilder stb = new StringBuilder();
            stb.AppendFormat($"SELECT HOME_BASED, PERIODO, SITE FROM GDA_COLLABORATORS_DETAILS (NOLOCK) ");
            stb.AppendFormat($"WHERE CONVERT(DATE, CREATED_AT) >= DATEADD(DAY, -1, CONVERT(DATE, GETDATE())) ");
            stb.AppendFormat($"AND IDGDA_COLLABORATORS = {idCollaborator} ");

            string homeBased = "";
            string periodo = "";
            string site = "";
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                homeBased = reader["HOME_BASED"].ToString();
                                periodo = reader["PERIODO"].ToString();
                                site = reader["SITE"].ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }


            bool inPersonOrHome = (homeBased == "SIM" || periodo == "MADRUGADA")
                                ? true
                                : (homeBased == "NÃO" && periodo != "MADRUGADA")
                                ? false
                                : true;

            string collaboratorOrderType = inPersonOrHome
                ? "VENCIMENTO_LIBERADO_HOME"
                : "MktConfig.VENCIMENTO_LIBERADO_PRESENCIAL";

            collaboratorReleasedModel cb = new collaboratorReleasedModel();
            cb.homeBased = collaboratorOrderType;
            cb.site = site;

            return cb;
        }

        public static int getTypeDays(string homeBased)
        {
            StringBuilder stb = new StringBuilder();
            stb.AppendFormat($"SELECT VALUE FROM GDA_MKT_CONFIG WHERE DELETED_AT IS NULL AND TYPE = '{homeBased}' ");

            int retorno = 0;

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                retorno = Convert.ToInt32(reader["VALUE"].ToString());

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


        public static List<string> getholidays(string site)
        {
            StringBuilder stb = new StringBuilder();
            stb.AppendFormat($"SELECT HOLYDAY_DATE FROM GDA_HOLIDAYS WHERE DELETED_AT IS NULL AND (SITE = '{site}' OR TYPE = 'NACIONAL') AND CONVERT(DATE, HOLYDAY_DATE) >= CONVERT(DATE, GETDATE()) ");

            List<string> retorno = new List<string>();

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DateTime sss = Convert.ToDateTime(reader["HOLYDAY_DATE"].ToString());

                                retorno.Add(sss.ToString("yyyy-MM-dd"));
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
    }

    // Método para serializar um DataTable em JSON

}