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
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ExportFileMonetizationExpiredDateController : ApiController
    {// POST: api/Results


        public class returnExportFileMonetizationExpired
        {
            public string SETORSITE { get; set; }
            public string CODGIPSITE { get; set; }
            public string EXPIREDDAYS { get; set; }
            
        }


        [HttpGet]
        public IHttpActionResult PostResultsModel()
        {
            int collaboratorId = 0;
            int personauserId = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            collaboratorId = inf.collaboratorId;

            List<returnExportFileMonetizationExpired> efme = new List<returnExportFileMonetizationExpired>();

            efme = BancoListExpirationConfig.getListMonetizationExpired();

            return Ok(efme);
        }

        public class BancoListExpirationConfig
        {

            public static List<returnExportFileMonetizationExpired> getListMonetizationExpired()
            {
                List<returnExportFileMonetizationExpired> rms = new List<returnExportFileMonetizationExpired>();

                using (SqlConnection connection = new SqlConnection(Database.Conn))
                {
                    connection.Open();
                    try
                    {
                        StringBuilder stb = new StringBuilder();
                        stb.Append($"SELECT DAYS,  ");
                        stb.Append($"CASE WHEN IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE = 1 THEN 'SETOR' ELSE 'SITE' END AS SETORSITE, ");
                        stb.Append($"CASE WHEN IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE = 2 THEN S.SITE ELSE CAST(ID_REFERER AS VARCHAR) END AS CODGIPSITE ");
                        stb.Append($"FROM GDA_MONETIZATION_CONFIG (NOLOCK) AS MC ");
                        stb.Append($"LEFT JOIN GDA_SITE (NOLOCK) AS S ON S.IDGDA_SITE = MC.ID_REFERER AND MC.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE = 2 ");
                        stb.Append($"WHERE DELETED_AT IS NULL ");

                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    returnExportFileMonetizationExpired rm = new returnExportFileMonetizationExpired();
                                    rm.SETORSITE = reader["SETORSITE"] != DBNull.Value ? reader["SETORSITE"].ToString() : "";
                                    rm.CODGIPSITE = reader["CODGIPSITE"] != DBNull.Value ? reader["CODGIPSITE"].ToString() : "";
                                    rm.EXPIREDDAYS = reader["DAYS"] != DBNull.Value ?reader["DAYS"].ToString() : "";
                                    rms.Add(rm);
                                }
                            }
                        }

                    }
                    catch (Exception)
                    {

                    }


                    connection.Close();

                }
                return rms;
            }


        }
    }
}