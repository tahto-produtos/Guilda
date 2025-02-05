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
    public class ListFeedBackInfractionController : ApiController
    {// POST: api/Results

        public class ReturnFeedBackInfraction
        {
            public int totalpages { get; set; }
            public List<FeedBackInfraction> ListFeedBack { get; set; }

        }

        public class FeedBackInfraction
        {
            public int IDGDA_PEDAGOGICAL_SCALE { get; set; }
            public string NAME { get; set; }
            public string CREATED_AT { get; set; }
            public string TYPE { get; set; }
            public string TYPE_INFRACTION { get; set; }
            public int ORDER { get; set; }
            public string CODE { get; set; }
            public int PERIOD { get; set; }
            public string AMOUNT_COLLABORATORS { get; set; }

        }

        public class InputListFeedBackInfraction
        {
            public int limit { get; set; }
            public int page { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputListFeedBackInfraction inputModel)
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

            ReturnFeedBackInfraction rmams = new ReturnFeedBackInfraction();

 



            rmams = BancoListFeedBackInfraction.listFeedBack(inputModel, personaid);

            return Ok(rmams);

        }
        // Método para serializar um DataTable em JSON


        #region Banco

        public class BancoListFeedBackInfraction
        {
            public static ReturnFeedBackInfraction listFeedBack(ListFeedBackInfractionController.InputListFeedBackInfraction inputModel, int personaId)
            {
                ReturnFeedBackInfraction retorno = new ReturnFeedBackInfraction();
                List<FeedBackInfraction> ListFeedBack = new List<FeedBackInfraction>();

                int totalInfo = QuantidadeFeedBackInfraction();
                int totalpage = (int)Math.Ceiling((double)totalInfo / inputModel.limit);
                int offset = (inputModel.page - 1) * inputModel.limit;

                retorno.totalpages = totalpage;

                StringBuilder sb = new StringBuilder();
                sb.Append($"SELECT PS.IDGDA_PEDAGOGICAL_SCALE, ");
                sb.Append($"       MAX(PS.PEDAGOGICAL_SCALE) AS PEDAGOGICAL_SCALE, ");
                sb.Append($"       MAX(PS.CREATED_AT) AS CREATED_AT, ");
                sb.Append($"       MAX(SG.PEDAGOGICAL_SCALE_GRAVITY) AS MAX_PEDAGOGICAL_SCALE_GRAVITY, ");

                sb.Append($"       MAX(ST.PEDAGOGICAL_SCALE_TYPE) AS MAX_PEDAGOGICAL_SCALE_TYPE, ");
                sb.Append($"       MAX(PS.PEDAGOGICAL_ORDER) AS 'ORDER', ");
                sb.Append($"       MAX(PS.TIME_OFF) AS TIME_OFF, ");
                sb.Append($"       MAX(PS.CODE) AS CODE, ");

                sb.Append($"       COUNT(FU.IDGDA_PEDAGOGICAL_SCALE) AS QTD ");
                sb.Append($"FROM GDA_PEDAGOGICAL_SCALE PS ");
                sb.Append($"LEFT JOIN GDA_FEEDBACK_USER FU ON PS.IDGDA_PEDAGOGICAL_SCALE = FU.IDGDA_PEDAGOGICAL_SCALE ");
                sb.Append($"LEFT JOIN GDA_PEDAGOGICAL_SCALE_GRAVITY SG ON PS.IDGDA_PEDAGOGICAL_SCALE_GRAVITY = SG.IDGDA_PEDAGOGICAL_SCALE_GRAVITY ");
                sb.Append($"LEFT JOIN GDA_PEDAGOGICAL_SCALE_TYPE ST ON PS.IDGDA_PEDAGOGICAL_SCALE_TYPE = ST.IDGDA_PEDAGOGICAL_SCALE_TYPE ");

                sb.Append($"WHERE PS.DELETED_AT IS NULL ");
                sb.Append($"GROUP BY PS.IDGDA_PEDAGOGICAL_SCALE ");
                sb.Append($"ORDER BY MAX(PS.CREATED_AT) ");
                sb.Append($"OFFSET {offset} ROWS FETCH NEXT {inputModel.limit} ROWS ONLY  ");
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
                                    FeedBackInfraction FeedBack = new FeedBackInfraction();

                                    FeedBack.IDGDA_PEDAGOGICAL_SCALE = reader["IDGDA_PEDAGOGICAL_SCALE"] != DBNull.Value ? int.Parse(reader["IDGDA_PEDAGOGICAL_SCALE"].ToString()) : 0;
                                    FeedBack.NAME = reader["PEDAGOGICAL_SCALE"] != DBNull.Value ? reader["PEDAGOGICAL_SCALE"].ToString() : "";
                                    DateTime feedbackCreatedAt;
                                    if (reader["CREATED_AT"] != DBNull.Value && DateTime.TryParse(reader["CREATED_AT"].ToString(), out feedbackCreatedAt))
                                    {
                                        FeedBack.CREATED_AT = feedbackCreatedAt.ToString("dd/MM/yyyy");
                                    }
                                    FeedBack.TYPE = reader["MAX_PEDAGOGICAL_SCALE_GRAVITY"] != DBNull.Value ? reader["MAX_PEDAGOGICAL_SCALE_GRAVITY"].ToString() : "";
                                    FeedBack.AMOUNT_COLLABORATORS = reader["QTD"] != DBNull.Value ? reader["QTD"].ToString() : "";

                                    FeedBack.TYPE_INFRACTION = reader["MAX_PEDAGOGICAL_SCALE_TYPE"] != DBNull.Value ? reader["MAX_PEDAGOGICAL_SCALE_TYPE"].ToString() : "";
                                    FeedBack.ORDER = reader["ORDER"] != DBNull.Value ? int.Parse(reader["ORDER"].ToString()) : 0;
                                    FeedBack.CODE = reader["CODE"] != DBNull.Value ? reader["CODE"].ToString() : "";
                                    FeedBack.PERIOD = reader["TIME_OFF"] != DBNull.Value ? int.Parse(reader["TIME_OFF"].ToString()) : 0;

                                    ListFeedBack.Add(FeedBack);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }
                retorno.ListFeedBack = ListFeedBack;
                return retorno;
            }

            public static int QuantidadeFeedBackInfraction()
            {
                StringBuilder sb = new StringBuilder();
                int total = 0;
                sb.Append($"SELECT COUNT(0) AS QTD ");
                sb.Append($"FROM GDA_PEDAGOGICAL_SCALE PS ");
                sb.Append($"LEFT JOIN GDA_FEEDBACK_USER FU ON PS.IDGDA_PEDAGOGICAL_SCALE = FU.IDGDA_PEDAGOGICAL_SCALE ");
                sb.Append($"LEFT JOIN GDA_PEDAGOGICAL_SCALE_GRAVITY SG ON PS.IDGDA_PEDAGOGICAL_SCALE_GRAVITY = SG.IDGDA_PEDAGOGICAL_SCALE_GRAVITY ");
                sb.Append($"WHERE PS.DELETED_AT IS NULL ");
                sb.Append($"GROUP BY PS.IDGDA_PEDAGOGICAL_SCALE ");
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                        {
                            // Executando o comando e armazenando o resultado na variável 'total'
                            total = (int)command.ExecuteScalar();
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }
                return total;
            }

        }
        #endregion

    }
}