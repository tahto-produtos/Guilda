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
    public class ListMonetizationExpiredController : ApiController
    {// POST: api/Results

        public class inputModel
        {
            public int limit { get; set; }
            public int page { get; set; }

        }

        public class returnModel
        {
            public int TOTALPAGES { get; set; }

            public List<listExpired> returnListExpired { get; set; }
        }

        public class listExpired
        {
            public int value { get; set; }
            public DateTime? dataExpiration { get; set; }
        }


        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] inputModel inputModel)
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
            personauserId = inf.personauserId;

            returnModel rm = new returnModel();
            rm.returnListExpired = new List<listExpired>();


            int totalInfo = BancoListExpiration.quantidadeExpiracao(collaboratorId);
            int totalpage = totalInfo / inputModel.limit;
            int offset = (inputModel.page - 1) * inputModel.limit;
            rm.TOTALPAGES = totalpage;


            rm.returnListExpired = BancoListExpiration.getListMonetization(collaboratorId, offset, inputModel.limit);

            return Ok(rm);
        }

        public class BancoListExpiration
        {

            public static int quantidadeExpiracao(int collaboratorId)
            {
                int total = 0;

                using (SqlConnection connection = new SqlConnection(Database.Conn))
                {
                    connection.Open();
                    try
                    {
                        StringBuilder stb = new StringBuilder();
                        stb.Append("SELECT COUNT(DISTINCT DUE_AT) AS CONTA  ");
                        stb.Append("FROM GDA_CHECKING_ACCOUNT (NOLOCK) ");
                        stb.Append($"WHERE COLLABORATOR_ID = {collaboratorId} ");
                        stb.Append("AND DUE_AT IS NOT NULL ");
                        stb.Append("AND (INPUT - INPUT_USED) > 0 ");
                        stb.Append("AND CONVERT(DATE, DUE_AT) >= CONVERT(DATE, GETDATE()) ");
                        //stb.Append("GROUP BY DUE_AT ");

                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    total = reader["CONTA"] != DBNull.Value ? Convert.ToInt32(reader["CONTA"].ToString()) : 0;
                                }
                            }
                        }

                    }
                    catch (Exception)
                    {

                    }


                    connection.Close();

                }
                return total;
            }

            public static List<listExpired> getListMonetization(int collaboratorId, int offset, int limit)
            {
                List<listExpired> rms = new List<listExpired>();

                using (SqlConnection connection = new SqlConnection(Database.Conn))
                {
                    connection.Open();
                    try
                    {
                        StringBuilder stb = new StringBuilder();
                        stb.Append("SELECT SUM(INPUT - INPUT_USED) AS VALOR, DUE_AT AS 'DATA EXPIRAÇÃO'  ");
                        stb.Append("FROM GDA_CHECKING_ACCOUNT (NOLOCK) ");
                        stb.Append($"WHERE COLLABORATOR_ID = {collaboratorId} ");
                        stb.Append("AND DUE_AT IS NOT NULL ");
                        stb.Append("AND (INPUT - INPUT_USED) > 0 ");
                        stb.Append("AND CONVERT(DATE, DUE_AT) >= CONVERT(DATE, GETDATE()) ");
                        stb.Append("GROUP BY DUE_AT ");
                        stb.Append("ORDER BY DUE_AT ASC ");
                        stb.Append($" OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY ");

                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    listExpired rm = new listExpired();
                                    rm.value = reader["VALOR"] != DBNull.Value ? Convert.ToInt32(reader["VALOR"].ToString()) : 0;
                                    rm.dataExpiration = reader["DATA EXPIRAÇÃO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA EXPIRAÇÃO"].ToString()) : (DateTime?)null;

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