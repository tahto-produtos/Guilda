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
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ListMyFeedBackController : ApiController
    {// POST: api/Results

        public class ReturnMyFeedBack
        {
            public int totalpages { get; set; }
            public List<MyFeedBack> ListMyFeedBack { get; set; }

        }

        public class MyFeedBack
        {
            public int IDGDA_FEEDBACK_USER { get; set; }
            public string NAME { get; set; }
            public string STATUS { get; set; }
            public string DATA_RESPOSTA { get; set; }
            public string PROTOCOLO { get; set; }
            public string PENALIDADE { get; set; }
        }

        public class InputListMyFeedBack
        {
            public int limit { get; set; }
            public int page { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputListMyFeedBack inputModel)
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

            ReturnMyFeedBack rmams = new ReturnMyFeedBack();

            rmams = BancoListMyFeedBack.listMyFeedBack(personaid, inputModel );

            return Ok(rmams);

        }
        // Método para serializar um DataTable em JSON


        #region Banco

        public class BancoListMyFeedBack
        {
            public static ReturnMyFeedBack listMyFeedBack(int personaId, ListMyFeedBackController.InputListMyFeedBack inputModel)
            {
                ReturnMyFeedBack retorno = new ReturnMyFeedBack();
                List<MyFeedBack> ListMyFeedBack = new List<MyFeedBack>();

                int totalInfo = Funcoes.QuantidadeMyFeedBack(personaId);
                int totalpage = (int)Math.Ceiling((double)totalInfo / inputModel.limit);
                int offset = (inputModel.page - 1) * inputModel.limit;

                retorno.totalpages = totalpage;

                StringBuilder sb = new StringBuilder();

                sb.Append("SELECT FU.IDGDA_FEEDBACK_USER, ");
                sb.Append("	   PU.NAME,  ");
                sb.Append("	   CASE WHEN SIGNED_RECEIVED IS NULL THEN 'Recebido' ELSE 'Assinado' END AS STATUS, ");
                sb.Append("	   CASE WHEN FU.RESPONDED_AT IS NULL THEN  '-' ELSE CONVERT(varchar,CONVERT(DATE,RESPONDED_AT)) END AS DATA_RESPOSTA, ");
                sb.Append("	   PROTOCOL, ");
                sb.Append("	   CASE WHEN  CONVERT(DATE,SUSPENDED_UNTIL) > CONVERT(DATE,GETDATE()) THEN 'Aplicada'  ");
                sb.Append("	   WHEN SIGNED_RECEIVED IS NULL AND SUSPENDED_UNTIL IS NULL THEN 'Aplicada' ");
                sb.Append("	   ELSE 'Cumprida' END AS PENALIDADE ");
                sb.Append("FROM GDA_FEEDBACK_USER (NOLOCK) FU ");
                sb.Append("INNER JOIN GDA_PERSONA_USER (NOLOCK) AS PU ON PU.IDGDA_PERSONA_USER = FU.IDPERSONA_RECEIVED_BY ");
                sb.Append("INNER JOIN GDA_FEEDBACK (NOLOCK) AS F ON F.IDGDA_FEEDBACK = FU.IDGDA_FEEDBACK ");
                sb.Append("WHERE 1=1 ");
                sb.Append($"AND IDPERSONA_RECEIVED_BY = {personaId}  ");
                sb.Append("ORDER BY FU.IDGDA_FEEDBACK_USER DESC ");
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
                                    MyFeedBack MyFeedBack = new MyFeedBack();

                                    MyFeedBack.IDGDA_FEEDBACK_USER = reader["IDGDA_FEEDBACK_USER"] != DBNull.Value ? int.Parse(reader["IDGDA_FEEDBACK_USER"].ToString()) : 0;
                                    MyFeedBack.NAME = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "";
                                    MyFeedBack.STATUS = reader["STATUS"] != DBNull.Value ? reader["STATUS"].ToString() : "";
                                    MyFeedBack.DATA_RESPOSTA = reader["DATA_RESPOSTA"] != DBNull.Value ? reader["DATA_RESPOSTA"].ToString() : "";
                                    MyFeedBack.PROTOCOLO = reader["PROTOCOL"] != DBNull.Value ? reader["PROTOCOL"].ToString() : "";
                                    MyFeedBack.PENALIDADE = reader["PENALIDADE"] != DBNull.Value ? reader["PENALIDADE"].ToString() : "";                                   
                                    ListMyFeedBack.Add(MyFeedBack);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }

                retorno.ListMyFeedBack = ListMyFeedBack;
                return retorno;
            }
            #endregion

        }


    }
}