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
using static ApiRepositorio.Controllers.ReportQuizController;
using System.Threading;
using static ApiRepositorio.Controllers.IndicatorPerformanceActionEscalationController;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class IndicatorPerformanceActionEscalationController : ApiController
    {// POST: api/Results

        public class listActionEscalation
        {
            public DateTime STARTEDATFROM { get; set; }
            public DateTime STARTEDATTO { get; set; }
            public int INDICATORSID { get; set; }
            public int SECTORSID { get; set; }
            public int SUBSECTORSID { get; set; }
            public int IDHOMEBASED { get; set; }
            public int IDGROUP { get; set; }
            public int IDSITE { get; set; }
            //public int IDHIERARCHY { get; set; }
            public double RESULT { get; set; }
            public double PERCENT { get; set; }
            public double GOAL { get; set; }
        }

        public class inputListIndicatorPerformance
        {
            public DateTime STARTEDATFROM { get; set; }
            public DateTime STARTEDATTO { get; set; }
            public int INDICATORSID { get; set; }
            public int SECTORSID { get; set; }
            public int SUBSECTORSID { get; set; }
            public int IDHOMEBASED { get; set; }
            public int IDGROUP { get; set; }
            public int IDSITE { get; set; }
            //public int IDHIERARCHY { get; set; }
        }
        [HttpPost]
        public IHttpActionResult GetResultsModel([FromBody] inputListIndicatorPerformance inputModel)
        {
            int COLLABORATORID = 0;
            int PERSONAUSERID = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            COLLABORATORID = inf.collaboratorId;
            PERSONAUSERID = inf.personauserId;

            if (inputModel.STARTEDATTO.ToString() == "" || inputModel.STARTEDATFROM.ToString() == "")
            {
                return BadRequest("Selecione uma data!");
            }
            string dtInicialValor = inputModel.STARTEDATFROM.ToString("yyyy-MM-dd");
            string dtFinalValor = inputModel.STARTEDATTO.ToString("yyyy-MM-dd");

            DateTime dtTimeInicial = DateTime.ParseExact(dtInicialValor, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dtTimeFinal = DateTime.ParseExact(dtFinalValor, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            TimeSpan diff = dtTimeFinal.Subtract(dtTimeInicial);
            int diferencaEmDias = (int)diff.TotalDays;
            if (diferencaEmDias > 31)
            {
                return BadRequest("Selecionar uma data de no maximo 1 mês!");
            }

            var jsonData = BancoIndicatorPerformance.relActionEscalation(inputModel);

            // Use o método Ok() para retornar o objeto serializado em JSON
            if (jsonData == null)
            {
                return Ok("Não encontramos resultados para este indicador");
            }
            else
            {
                return Ok(jsonData);
            }
            
        }
        // Método para serializar um DataTable em JSON


    }

    public class BancoIndicatorPerformance
    {

        public static listActionEscalation relActionEscalation(inputListIndicatorPerformance inputModel)
        {

            listActionEscalation rmams = new listActionEscalation();
            rmams = returnActionEscalation(inputModel);

            return rmams;

        }

        public static listActionEscalation returnActionEscalation(inputListIndicatorPerformance inputModel)
        {
            listActionEscalation listEscalation = new listActionEscalation();


            string filter = "";
            string indicatorsAsString = inputModel.INDICATORSID == 0 ? "" : inputModel.INDICATORSID.ToString();
            string sectorsAsString = inputModel.SECTORSID == 0 ? "" : inputModel.SECTORSID.ToString();
            string subsectorAsString = inputModel.SUBSECTORSID == 0 ? "" : inputModel.SUBSECTORSID.ToString();

            string outerFilter = "";
            if (inputModel.IDSITE != 0)
            {
                StringBuilder stb = new StringBuilder();
                stb.Append($"SELECT SITE FROM GDA_SITE (NOLOCK) ");
                stb.Append($"WHERE IDGDA_SITE = {inputModel.IDSITE} ");
                string site = "";

                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.CommandTimeout = 0;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                site = reader["SITE"].ToString();
                            }
                        }
                    }
                    connection.Close();
                }

                outerFilter = $"{outerFilter} AND SITE = '{site}' ";
            }
            if (inputModel.IDGROUP != 0)
            {
                outerFilter = $"{outerFilter} AND IDGDA_GROUP = {inputModel.IDGROUP} ";
            }
            if (inputModel.IDHOMEBASED != 0)
            {
                StringBuilder stb = new StringBuilder();
                stb.Append($"SELECT HOMEFLOOR FROM GDA_HOMEFLOOR (NOLOCK) ");
                stb.Append($"WHERE IDGDA_HOMEFLOOR = {inputModel.IDHOMEBASED} ");
                string homebased = "";

                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.CommandTimeout = 0;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                homebased = reader["HOMEFLOOR"].ToString();
                            }
                        }
                    }
                    connection.Close();
                }

                outerFilter = $"{outerFilter} AND HOME_BASED = '{homebased}' ";
            }



            List<ResultConsolidatedController.HomeResultConsolidated> rmams = new List<ResultConsolidatedController.HomeResultConsolidated>();
            rmams = ResultConsolidatedController.ReturnHomeResultConsolidated("", inputModel.STARTEDATFROM.ToString("yyyy-MM-dd"), inputModel.STARTEDATTO.ToString("yyyy-MM-dd"), "", indicatorsAsString, sectorsAsString, subsectorAsString, "", "", "", true, outerFilter);

            if (inputModel.INDICATORSID == 10000013 || inputModel.INDICATORSID == 10000014)
            {
                string filterEnv = $"'{inputModel.INDICATORSID}'";
                List<ResultConsolidatedController.HomeResultConsolidated> listaIndicadorAcesso = new List<ResultConsolidatedController.HomeResultConsolidated>();
                listaIndicadorAcesso = ResultConsolidatedController.ReturnHomeResultConsolidatedAccess("", inputModel.STARTEDATFROM.ToString("yyyy-MM-dd"), inputModel.STARTEDATTO.ToString("yyyy-MM-dd"), "", inputModel.SECTORSID.ToString(), inputModel.SUBSECTORSID.ToString(), "", "", "", true, filterEnv);
                rmams = rmams.Concat(listaIndicadorAcesso).ToList();
            }

            rmams = rmams.GroupBy(d => new { d.IDINDICADOR }).Select(item => new ResultConsolidatedController.HomeResultConsolidated
            {
                MATRICULA = item.First().MATRICULA,
                CARGO = item.First().CARGO,
                //CODGIP = item.First().CODGIP,
                //SETOR = item.First().SETOR,
                IDINDICADOR = item.Key.IDINDICADOR,
                INDICADOR = item.First().INDICADOR,
                QTD = item.Sum(d => d.QTD),
                META = Math.Round(item.Sum(d => d.META) / item.Count(), 2, MidpointRounding.AwayFromZero),
                FACTOR0 = item.Sum(d => d.FACTOR0),
                FACTOR1 = item.Sum(d => d.FACTOR1),
                //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                min1 = Math.Round(item.Sum(d => d.min1) / item.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                min2 = Math.Round(item.Sum(d => d.min2) / item.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                min3 = Math.Round(item.Sum(d => d.min3) / item.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                min4 = Math.Round(item.Sum(d => d.min4) / item.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                CONTA = item.First().CONTA,
                BETTER = item.First().BETTER,
                //META_MAXIMA_MOEDAS = Math.Round(item.Sum(d => d.META_MAXIMA_MOEDAS) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                META_MAXIMA_MOEDAS = item.Sum(d => d.QTD_MON != 0 ? Math.Round(d.QTD_MON_TOTAL / d.QTD_MON, 0, MidpointRounding.AwayFromZero) : 0),

                SUMDIASLOGADOS = item.Sum(d => d.SUMDIASLOGADOS),
                SUMDIASESCALADOS = item.Sum(d => d.SUMDIASESCALADOS),
                SUMDIASLOGADOSESCALADOS = item.Sum(d => d.SUMDIASLOGADOSESCALADOS),

                //MOEDA_GANHA =  Math.Round(item.Sum(d => d.MOEDA_GANHA) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                MOEDA_GANHA = Math.Round(item.Sum(d => d.MOEDA_GANHA), 0, MidpointRounding.AwayFromZero),
                TYPE = item.First().TYPE,
            }).ToList();

            if (rmams.Count > 0)
            {
                ResultConsolidatedController.HomeResultConsolidated consolidado = rmams[0];
                rmams[0] = ResultConsolidatedController.DoCalculateFinal(consolidado);

                double resultado = rmams[0].RESULTADO;
                double meta = rmams[0].META;
                double percentual = rmams[0].PERCENTUAL;
                string biggerBetter = rmams[0].BETTER;

                listEscalation.STARTEDATFROM = inputModel.STARTEDATFROM;
                listEscalation.STARTEDATTO = inputModel.STARTEDATTO;
                listEscalation.INDICATORSID = inputModel.INDICATORSID;
                listEscalation.SECTORSID = inputModel.SECTORSID;
                listEscalation.SUBSECTORSID = inputModel.SUBSECTORSID;
                listEscalation.IDHOMEBASED = inputModel.IDHOMEBASED;
                listEscalation.IDGROUP = inputModel.IDGROUP;
                listEscalation.IDSITE = inputModel.IDSITE;
                //listEscalation.IDHIERARCHY = inputModel.IDHIERARCHY;

                listEscalation.RESULT = resultado;
                listEscalation.PERCENT = percentual;
                listEscalation.GOAL = meta;
            }
            else
            {
                return null;
            }
            return listEscalation;

        }

    }
}
