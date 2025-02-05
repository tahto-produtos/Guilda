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
using static ApiRepositorio.Controllers.LoadLibraryNotificationController;
using static ApiRepositorio.Controllers.LoadLibraryQuizController;
using static ApiRepositorio.Controllers.LoadLibraryActionEscalationController;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class LoadLibraryActionEscalationController : ApiController
    {// POST: api/Results
        public class ReturnActionEscalation
        {
            public int totalpages { get; set; }
            public List<LoadLibraryActionEscalation> LoadActionEscalation { get; set; }
        }
        public class LoadLibraryActionEscalation
        {
            public int IDGDA_ESCALATION_ACTION {  get; set; }
            public string NOME { get; set; }
            public string INDICADOR { get; set; }
            public string STATUS { get; set; }
            public DateTime? startedAt { get; set; }
            public DateTime? endedAt { get; set; }         
        }
        public class InputLibraryActionEscalation
        {
            public DateTime? CREATEDATFROM { get; set; }
            public DateTime? CREATEDATTO { get; set; }
            public DateTime? STARTEDATFROM { get; set; }
            public DateTime? STARTEDATTO { get; set; }
            public DateTime? ENDEDATFROM { get; set; }
            public DateTime? ENDEDATTO { get; set; }
            public string NAME { get; set; }
            public string DESCRIPTION { get; set; }
            public List<int> INDICATOR { get; set; }
            public int LIMIT { get; set; }
            public int PAGE { get; set; }
        }


        [HttpPost]
        public IHttpActionResult GetResultsModel([FromBody] InputLibraryActionEscalation inputModel)
        {
            int COLLABORATORID = 0;
            int PERSONAUSERID = 0;
            var token = Request.Headers.Authorization?.Parameter;
            bool tokn = TokenService.TryDecodeToken(token);
            if (tokn == false)
            {
                return Unauthorized();
            }
            COLLABORATORID = TokenService.InfsUsers.collaboratorId;
            PERSONAUSERID = TokenService.InfsUsers.personauserId;

            ReturnActionEscalation returnQuizz = new ReturnActionEscalation();

            returnQuizz = BancoLibraryActionEscalation.returnLibraryActionEscalation(PERSONAUSERID, inputModel);



            return Ok(returnQuizz);
        }
        // Método para serializar um DataTable em JSON


    }

    public class BancoLibraryActionEscalation
    {

        public static string filterLibraryActionEscalation(LoadLibraryActionEscalationController.InputLibraryActionEscalation inputModel)
        {
            string ret = "";

            try
            {

                if (inputModel.CREATEDATFROM != (DateTime?)null)
                {
                    ret = $"{ret} AND EA.CREATED_AT >= '{inputModel.CREATEDATFROM}' AND EA.CREATED_AT <= '{inputModel.CREATEDATTO}' ";
                }
                if (inputModel.STARTEDATFROM != (DateTime?)null)
                {
                    ret = $"{ret} AND EA.STARTED_AT >= '{inputModel.STARTEDATFROM}' AND EA.STARTED_AT <= '{inputModel.STARTEDATTO}' ";
                }
                if (inputModel.ENDEDATFROM != (DateTime?)null)
                {
                    ret = $"{ret} AND EA.ENDED_AT >= '{inputModel.ENDEDATFROM}' AND EA.ENDED_AT <= '{inputModel.ENDEDATTO}' ";
                }              
                if (inputModel.NAME != "")
                {
                    ret = $"{ret} AND (EA.NAME LIKE '%{inputModel.NAME}%') ";

                }
                if (inputModel.DESCRIPTION != "")
                {
                    ret = $"{ret} AND EA.DESCRIPTION LIKE '%{inputModel.DESCRIPTION}%' ";
                }

                if (inputModel.INDICATOR.Count != 0)
                {
                    string IndicatorAsString = string.Join(",", inputModel.INDICATOR.Select(g => g));

                    ret = $"{ret} AND EA.IDGDA_INDICATOR IN ({IndicatorAsString}) ";
                }


            }
            catch (Exception)
            {

            }

            return ret;
        }


        public static LoadLibraryActionEscalationController.ReturnActionEscalation returnLibraryActionEscalation(int personauserId, LoadLibraryActionEscalationController.InputLibraryActionEscalation inputModel)
        {
            ReturnActionEscalation retorno = new ReturnActionEscalation();
            List<LoadLibraryActionEscalation> listActionEscalation = new List<LoadLibraryActionEscalation>();

            string filter = filterLibraryActionEscalation(inputModel);

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    int totalInfo = Funcoes.QuantidadeActionEscalation(filter);
                    int totalpage = (int)Math.Ceiling((double)totalInfo / inputModel.LIMIT);
                    int offset = (inputModel.PAGE - 1) * inputModel.LIMIT;

                    retorno.totalpages = totalpage;


                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT  ");
                    sb.Append("   EA.IDGDA_ESCALATION_ACTION, ");
                    sb.Append("	  EA.NAME, ");
                    sb.Append("	  IND.NAME AS INDICADOR, ");
                    sb.Append("	  CASE WHEN CONVERT(DATE,GETDATE()) > CONVERT(DATE,EA.ENDED_AT) THEN 'Finalizado' ");
                    sb.Append("	  WHEN CONVERT(DATE,GETDATE()) < CONVERT(DATE,EA.STARTED_AT) THEN 'Não inciado' ");
                    sb.Append("	  ELSE 'Em andamento' END AS STATUS, ");
                    sb.Append("	  EA.STARTED_AT AS DATA_INICIO, ");
                    sb.Append("	  EA.ENDED_AT AS DATA_FIM ");
                    sb.Append("FROM GDA_ESCALATION_ACTION (NOLOCK) EA ");
                    sb.Append("LEFT JOIN GDA_INDICATOR (NOLOCK) AS IND ON IND.IDGDA_INDICATOR = EA.IDGDA_INDICATOR ");
                    sb.Append("WHERE  1=1 ");
                    sb.Append($"{filter} ");
                    sb.Append($"ORDER BY 1 DESC ");
                    sb.Append($"OFFSET {offset} ROWS FETCH NEXT {inputModel.LIMIT} ROWS ONLY ");

                    using (SqlCommand commandInsert = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                LoadLibraryActionEscalationController.LoadLibraryActionEscalation libraryActionEscalation = new LoadLibraryActionEscalationController.LoadLibraryActionEscalation();

                                libraryActionEscalation.IDGDA_ESCALATION_ACTION = reader["IDGDA_ESCALATION_ACTION"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_ESCALATION_ACTION"]) : 0;
                                libraryActionEscalation.NOME = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "";
                                libraryActionEscalation.INDICADOR = reader["INDICADOR"] != DBNull.Value ? reader["INDICADOR"].ToString() : "";
                                libraryActionEscalation.STATUS = reader["STATUS"] != DBNull.Value ? reader["STATUS"].ToString() : "";
                                libraryActionEscalation.startedAt = reader["DATA_INICIO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_INICIO"]) : (DateTime?)null;
                                libraryActionEscalation.endedAt = reader["DATA_FIM"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_FIM"]) : (DateTime?)null;
                                listActionEscalation.Add(libraryActionEscalation);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            retorno.LoadActionEscalation = listActionEscalation;
            return retorno;
        }
    }
}
