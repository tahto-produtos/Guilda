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
using static ApiRepositorio.Controllers.ListFeedBackController;
using static ApiRepositorio.Controllers.ListFeedBackNotReceivedController;
using static ApiRepositorio.Controllers.ReportQuizController;
using System.Threading;
using ThirdParty.Json.LitJson;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ReportFeedBackController : ApiController
    {// POST: api/Results

        public class ListFeedBack
        {
            public string PROTOCOLO { get; set; }
            public string RESPONSAVEL { get; set; }
            public string COLABORADOR { get; set; }
            public string MOTIVO_FEEDBACK { get; set; }
            public string DETAIL { get; set; }
            public DateTime? CREATED_AT { get; set; }
            public DateTime? SIGNED_AT { get; set; }
        }

        public class returnResponseDay
        {
            public string PROTOCOLO { get; set; }
            public string RESPONSAVEL { get; set; }
            public string COLABORADOR { get; set; }
            public string MOTIVO_FEEDBACK { get; set; }
            public string DETAIL { get; set; }
            public DateTime? CREATED_AT { get; set; }
            public DateTime? SIGNED_AT { get; set; }

        }

        public class InputModelListFeedBack
        {
            public DateTime DATE_START { get; set; }
            public DateTime DATE_END { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModelListFeedBack inputModel)
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

            string dtInicial = inputModel.DATE_START.ToString("yyyy-MM-dd");
            string dtFinal = inputModel.DATE_END.ToString("yyyy-MM-dd");

            DateTime dtTimeInicial = DateTime.ParseExact(dtInicial, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dtTimeFinal = DateTime.ParseExact(dtFinal, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            TimeSpan diff = dtTimeFinal.Subtract(dtTimeInicial);
            int diferencaEmDias = (int)diff.TotalDays;
            if (diferencaEmDias > 31)
            {
                return BadRequest("Selecionar uma data de no maximo 1 mês!");
            }


            var jsonData = BancoReportFeedBack.relReportFeedBack(dtInicial, dtFinal);

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(jsonData);
        }


        #region Banco

        public class BancoReportFeedBack 
        { 
            public static List<returnResponseDay> relReportFeedBack(string dtInicial, string dtFinal)
            {
                List<ListFeedBack> rmams = new List<ListFeedBack>();
                rmams = returnReportFeedBack(dtInicial, dtFinal);
                var jsonData = rmams.Select(item => new returnResponseDay
                {
                    PROTOCOLO = item.PROTOCOLO,
                    RESPONSAVEL = item.RESPONSAVEL,
                    COLABORADOR = item.COLABORADOR,
                    MOTIVO_FEEDBACK = item.MOTIVO_FEEDBACK,
                    DETAIL = item.DETAIL,
                    CREATED_AT = item.CREATED_AT,
                    SIGNED_AT = item.SIGNED_AT,
                   
                }).ToList();

                return jsonData;
            }

            public static List<ListFeedBack> returnReportFeedBack(string dtInicial, string dtFinal)
            {

                StringBuilder sb = new StringBuilder();
                sb.Append(" ");
                sb.Append("SELECT MAX(FU.PROTOCOL) AS PROTOCOLO, ");
                sb.Append("	   MAX(PRESP.NAME) AS RESPONSAVEL, ");
                sb.Append("	   MAX(PCOLL.NAME) AS COLABORADOR, ");
                sb.Append("	   MAX(F.REASON) AS MOTIVO_FEEDBACK, ");
                sb.Append("	   MAX(F.DETAILS) AS DETAILS, ");
                sb.Append("	   MAX(F.CREATED_AT) AS CREATED_AT, ");
                sb.Append("	   MAX(FU.RESPONDED_AT) AS RESPONDED_AT ");
                sb.Append("FROM GDA_FEEDBACK_USER (NOLOCK) FU ");
                sb.Append("LEFT JOIN GDA_FEEDBACK (NOLOCK) AS F ON F.IDGDA_FEEDBACK = FU.IDGDA_FEEDBACK ");
                sb.Append("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCU1 ON PCU1.IDGDA_PERSONA_USER = IDPERSONA_SENDED_BY ");
                sb.Append("LEFT JOIN GDA_COLLABORATORS ( NOLOCK) AS PRESP ON PRESP.IDGDA_COLLABORATORS =  PCU1.IDGDA_COLLABORATORS ");
                sb.Append("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCU2 ON PCU2.IDGDA_PERSONA_USER = IDPERSONA_RECEIVED_BY ");
                sb.Append("LEFT JOIN GDA_COLLABORATORS ( NOLOCK) AS PCOLL ON PCOLL.IDGDA_COLLABORATORS =  PCU2.IDGDA_COLLABORATORS ");
                sb.Append($"WHERE FU.CREATED_AT >= '{dtInicial}' AND FU.CREATED_AT <= '{dtFinal}' ");
                sb.Append(" ");
                sb.Append("GROUP BY FU.IDGDA_FEEDBACK_USER ");

                List<ListFeedBack> rmams = new List<ListFeedBack>();
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        command.CommandTimeout = 300;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ListFeedBack rmam = new ListFeedBack();

                                rmam.PROTOCOLO = reader["PROTOCOLO"] != DBNull.Value ? reader["PROTOCOLO"].ToString():"";
                                rmam.RESPONSAVEL = reader["RESPONSAVEL"] != DBNull.Value ? reader["RESPONSAVEL"].ToString() : "";
                                rmam.COLABORADOR = reader["COLABORADOR"] != DBNull.Value ? reader["COLABORADOR"].ToString() : "";
                                rmam.MOTIVO_FEEDBACK = reader["MOTIVO_FEEDBACK"] != DBNull.Value ? reader["MOTIVO_FEEDBACK"].ToString() : "";
                                rmam.DETAIL = reader["DETAILS"] != DBNull.Value ? reader["DETAILS"].ToString() : "";
                                rmam.CREATED_AT = reader["CREATED_AT"] != DBNull.Value ? Convert.ToDateTime(reader["CREATED_AT"].ToString()) : (DateTime?)null;
                                rmam.SIGNED_AT = reader["RESPONDED_AT"] != DBNull.Value ? Convert.ToDateTime(reader["RESPONDED_AT"].ToString()) : (DateTime?)null;
                                rmams.Add(rmam);
                            }
                        }
                    }
                    connection.Close();
                }
                return rmams;
            }
              

        }
        #endregion

    }
}