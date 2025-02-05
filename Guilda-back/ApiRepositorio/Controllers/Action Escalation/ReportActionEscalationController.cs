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
using static ApiRepositorio.Controllers.ReportActionEscalationController;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ReportActionEscalationController : ApiController
    {// POST: api/Results

        public class listActionEscalation
        {
            public int IDINDICADOR { get; set; }
            public string INDICADOR { get; set; }
            public int IDSETOR { get; set; }
            public string SETOR { get; set; }
            public int IDSUBSETOR { get; set; }
            public string SUBSETOR { get; set; }
            public string AGENTE { get; set; }
            public string SUPERVISOR { get; set; }
            public string COORDENADOR { get; set; }
            public string GERENTEII { get; set; }
            public string GERENTEI { get; set; }
            public string DIRETOR { get; set; }
            public string CEO { get; set; }
            public double DISPERSAO_RESULTADO { get; set; }
            public string RESULTADO_STATUS { get; set; }
            public double DIFERENCA_META { get; set; }
            public string ACAO_DETALHADO { get; set; }
            public string DATA_INICIO { get; set; }
            public string DATA_FIM { get; set; }
            public string RESPONSAVEL { get; set; }
            public string STATUS { get; set; }
        }

        public class returnResponseDay
        {

        }

        public class inputListReportActionEscalation
        {
            public DateTime STARTEDATFROM { get; set; }
            public DateTime STARTEDATTO { get; set; }
            public DateTime ENDEDATFROM { get; set; }
            public DateTime ENDEDATTO { get; set; }
            public List<int> INDICATORSID { get; set; }
            public List<int> SECTORSID { get; set; }
            public List<int> SUBSECTORSID { get; set; }
        }
        [HttpPost]
        public IHttpActionResult GetResultsModel([FromBody] inputListReportActionEscalation inputModel)
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

            if (inputModel.STARTEDATTO.ToString() == "" && inputModel.ENDEDATTO.ToString() == "")
            {
                return BadRequest("Selecione uma data!");
            }

            if (inputModel.STARTEDATFROM.ToString() == "" && inputModel.ENDEDATFROM.ToString() == "")
            {
                return BadRequest("Selecione uma data!");
            }

            string dtInicialEm = inputModel.STARTEDATFROM.ToString("yyyy-MM-dd");
            string dtInicialAte = inputModel.STARTEDATTO.ToString("yyyy-MM-dd");

            string dtFinalEm = inputModel.ENDEDATFROM.ToString("yyyy-MM-dd");
            string dtFinalAte = inputModel.ENDEDATTO.ToString("yyyy-MM-dd");

            dtInicialEm = dtInicialEm == "0001-01-01" ? "" : dtInicialEm;
            dtInicialAte = dtInicialAte == "0001-01-01" ? "" : dtInicialAte;

            dtFinalEm = dtFinalEm == "0001-01-01" ? "" : dtFinalEm;
            dtFinalAte = dtFinalAte == "0001-01-01" ? "" : dtFinalAte;

            if (dtInicialAte != "")
            {
                DateTime dtTimeInicial = DateTime.ParseExact(dtInicialEm, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime dtTimeFinal = DateTime.ParseExact(dtInicialAte, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                TimeSpan diff = dtTimeFinal.Subtract(dtTimeInicial);
                int diferencaEmDias = (int)diff.TotalDays;
                if (diferencaEmDias > 31)
                {
                    return BadRequest("Selecionar uma data de no maximo 1 mês!");
                }
            }

            if (dtFinalAte != "")
            {
                DateTime dtTimeInicial = DateTime.ParseExact(dtFinalEm, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime dtTimeFinal = DateTime.ParseExact(dtFinalAte, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                TimeSpan diff = dtTimeFinal.Subtract(dtTimeInicial);
                int diferencaEmDias = (int)diff.TotalDays;
                if (diferencaEmDias > 31)
                {
                    return BadRequest("Selecionar uma data de no maximo 1 mês!");
                }
            }

            var jsonData = BancoReportActionEscalation.relActionEscalation(inputModel);

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(jsonData);
        }
        // Método para serializar um DataTable em JSON


    }

    public class BancoReportActionEscalation
    {

        public static List<ReportActionEscalationController.listActionEscalation> relActionEscalation(inputListReportActionEscalation inputModel)
        {

            List<listActionEscalation> rmams = new List<listActionEscalation>();
            rmams = returnActionEscalation(inputModel);

            //var jsonData = rmams.Select(item => new ReportActionEscalationController.returnResponseDay
            //{

            //}).ToList();

            return rmams;

        }

        public static List<listActionEscalation> returnActionEscalation(inputListReportActionEscalation inputModel)
        {
            string filter = "";
            string indicatorsAsString = string.Join(",", inputModel.INDICATORSID);
            string sectorsAsString = string.Join(",", inputModel.SECTORSID);
            string subsectorAsString = string.Join(",", inputModel.SUBSECTORSID);
            string dtInicialEm = inputModel.STARTEDATFROM.ToString("yyyy-MM-dd");
            string dtInicialAte = inputModel.STARTEDATTO.ToString("yyyy-MM-dd");

            string dtFinalEm = inputModel.ENDEDATFROM.ToString("yyyy-MM-dd");
            string dtFinalAte = inputModel.ENDEDATTO.ToString("yyyy-MM-dd");

            dtInicialEm = dtInicialEm == "0001-01-01" ? "" : dtInicialEm;
            dtInicialAte = dtInicialAte == "0001-01-01" ? "" : dtInicialAte;

            dtFinalEm = dtFinalEm == "0001-01-01" ? "" : dtFinalEm;
            dtFinalAte = dtFinalAte == "0001-01-01" ? "" : dtFinalAte;

            if (dtInicialEm != "")
            {
                filter = $"{filter} AND EA.STARTED_AT >= '{dtInicialEm}' AND EA.ENDED_AT <= '{dtInicialAte}' ";
            }
            if (dtFinalEm != "")
            {
                filter = $"{filter} AND EA.STARTED_AT >= '{dtFinalEm}' AND EA.ENDED_AT <= '{dtFinalAte}' ";
            }
            if (indicatorsAsString != "")
            {
                filter = $"{filter} AND EA.IDGDA_INDICATOR IN ({indicatorsAsString}) ";
            }
            if (sectorsAsString != "")
            {
                filter = $"{filter} AND EA.IDGDA_SECTOR IN ({sectorsAsString}) ";
            }
            if (subsectorAsString != "")
            {
                filter = $"{filter} AND EA.IDGDA_SUBSECTOR IN ({subsectorAsString}) ";
            }

            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT  MAX(EA.IDGDA_INDICATOR) AS IDINDICADOR, MAX(EA.IDGDA_SECTOR) AS IDSETOR, MAX(EA.IDGDA_SUBSECTOR) AS IDSUBSETOR, MAX(I.NAME) AS INDICADOR, MAX(S.NAME) AS SETOR, MAX(S2.NAME) AS SUBSETOR, EA.IDGDA_ESCALATION_ACTION,  ");
            sb.Append($"MAX(EA.DESCRIPTION) AS DESCRIPTION, MAX(EA.STARTED_AT) AS STARTEDAT, MAX(EA.ENDED_AT) AS ENDEDAT, MAX(PURESP.NAME) AS RESPONSAVEL,  ");
            sb.Append($"CASE WHEN CONVERT(DATE,GETDATE()) < CONVERT(DATE,MAX(EA.STARTED_AT)) THEN 'Pendente' ");
            sb.Append($"	 WHEN CONVERT(DATE,GETDATE()) >= CONVERT(DATE,MAX(EA.STARTED_AT)) AND CONVERT(DATE,GETDATE()) <= CONVERT(DATE,MAX(EA.ENDED_AT)) THEN  'Em Andamento' ");
            sb.Append($"	 ELSE 'Concluido'  ");
            sb.Append($"END AS STATUS, ");
            sb.Append($"SUM(CASE WHEN IDGDA_HIERARCHY = 1 THEN 1 ELSE 0 END) AS 'AGENTE',  ");
            sb.Append($"SUM(CASE WHEN IDGDA_HIERARCHY = 2 THEN 1 ELSE 0 END) AS 'SUPERVISOR',  ");
            sb.Append($"SUM(CASE WHEN IDGDA_HIERARCHY = 3 THEN 1 ELSE 0 END) AS 'COORDENADOR',  ");
            sb.Append($"SUM(CASE WHEN IDGDA_HIERARCHY = 4 THEN 1 ELSE 0 END) AS 'GERENTE II',  ");
            sb.Append($"SUM(CASE WHEN IDGDA_HIERARCHY = 5 THEN 1 ELSE 0 END) AS 'GERENTE I',  ");
            sb.Append($"SUM(CASE WHEN IDGDA_HIERARCHY = 6 THEN 1 ELSE 0 END) AS 'DIRETOR',  ");
            sb.Append($"SUM(CASE WHEN IDGDA_HIERARCHY = 7 THEN 1 ELSE 0 END) AS 'CEO'  ");
            sb.Append($"FROM GDA_ESCALATION_ACTION (NOLOCK) EA ");
            sb.Append($"LEFT JOIN GDA_ESCALATION_HISTORY_STAGE (NOLOCK) HS ON EA.IDGDA_ESCALATION_ACTION = HS.IDGDA_ESCALATION_ACTION ");
            sb.Append($"LEFT JOIN GDA_ESCALATION_ACTION_STAGE (NOLOCK) AS EAS ON EAS.IDGDA_ESCALATION_ACTION =  EA.IDGDA_ESCALATION_ACTION ");
            sb.Append($"LEFT JOIN GDA_INDICATOR (NOLOCK) AS I ON I.IDGDA_INDICATOR = EA.IDGDA_INDICATOR ");
            sb.Append($"LEFT JOIN GDA_SECTOR (NOLOCK) AS S ON S.IDGDA_SECTOR = EA.IDGDA_SECTOR ");
            sb.Append($"LEFT JOIN GDA_SECTOR (NOLOCK) AS S2 ON S2.IDGDA_SECTOR = EA.IDGDA_SUBSECTOR ");
            sb.Append($"LEFT JOIN GDA_PERSONA_USER (NOLOCK) AS PURESP ON PURESP.IDGDA_PERSONA_USER = EA.IDGDA_PERSONA_RESPONSIBLE ");
            sb.Append($"WHERE 1 = 1 {filter} ");
            sb.Append($"GROUP BY EA.IDGDA_ESCALATION_ACTION ");

            List<listActionEscalation> lEscs = new List<listActionEscalation>();
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
                            listActionEscalation lEsc = new listActionEscalation();
                            lEsc.IDINDICADOR = reader["IDINDICADOR"] != DBNull.Value ? Convert.ToInt32(reader["IDINDICADOR"].ToString()) : 0;
                            lEsc.INDICADOR = reader["INDICADOR"] != DBNull.Value ? reader["INDICADOR"].ToString() : "";
                            lEsc.IDSETOR = reader["IDSETOR"] != DBNull.Value ? Convert.ToInt32(reader["IDSETOR"].ToString()) : 0;
                            lEsc.SETOR = reader["SETOR"] != DBNull.Value ? reader["SETOR"].ToString() : "";
                            lEsc.IDSUBSETOR = reader["IDSUBSETOR"] != DBNull.Value ? Convert.ToInt32(reader["IDSUBSETOR"].ToString()) : 0;
                            lEsc.SUBSETOR = reader["SUBSETOR"] != DBNull.Value ? reader["SUBSETOR"].ToString() : "";
                            lEsc.AGENTE = reader["AGENTE"] != DBNull.Value ? reader["AGENTE"].ToString() : "";
                            lEsc.SUPERVISOR = reader["SUPERVISOR"] != DBNull.Value ? reader["SUPERVISOR"].ToString() : "";
                            lEsc.COORDENADOR = reader["COORDENADOR"] != DBNull.Value ? reader["COORDENADOR"].ToString() : "";
                            lEsc.GERENTEII = reader["GERENTE II"] != DBNull.Value ? reader["GERENTE II"].ToString() : "";
                            lEsc.GERENTEI = reader["GERENTE I"] != DBNull.Value ? reader["GERENTE I"].ToString() : "";
                            lEsc.DIRETOR = reader["DIRETOR"] != DBNull.Value ? reader["DIRETOR"].ToString() : "";
                            lEsc.CEO = reader["CEO"] != DBNull.Value ? reader["CEO"].ToString() : "";                           
                            lEsc.ACAO_DETALHADO = reader["DESCRIPTION"] != DBNull.Value ? reader["DESCRIPTION"].ToString() : "";
                            lEsc.DATA_INICIO = reader["STARTEDAT"] != DBNull.Value ? Convert.ToDateTime(reader["STARTEDAT"]).ToString("yyyy-MM-dd") : "";
                            lEsc.DATA_FIM = reader["ENDEDAT"] != DBNull.Value ? Convert.ToDateTime(reader["ENDEDAT"]).ToString("yyyy-MM-dd") : "";
                            lEsc.RESPONSAVEL = reader["RESPONSAVEL"] != DBNull.Value ? reader["RESPONSAVEL"].ToString() : "";
                            lEsc.STATUS = reader["STATUS"] != DBNull.Value ? reader["STATUS"].ToString() : "";

                            lEscs.Add(lEsc);
                        }
                    }
                }
                connection.Close();

                foreach (listActionEscalation lesc in lEscs)
                {

                    string setor = lesc.IDSETOR.ToString() == "0" ? "" : lesc.IDSETOR.ToString();
                    string subsetor = lesc.IDSUBSETOR.ToString() == "0" ? "" : lesc.IDSUBSETOR.ToString();

                    List<ResultConsolidatedController.HomeResultConsolidated> rmams = new List<ResultConsolidatedController.HomeResultConsolidated>();
                    rmams = ResultConsolidatedController.ReturnHomeResultConsolidated("", lesc.DATA_INICIO, lesc.DATA_FIM, "", lesc.IDINDICADOR.ToString(), setor, subsetor, "", "", "", true);

                    if (lesc.IDINDICADOR == 10000013 || lesc.IDINDICADOR == 10000014)
                    {
                        string filterEnv = $"'{lesc.IDINDICADOR}'";
                        List<ResultConsolidatedController.HomeResultConsolidated> listaIndicadorAcesso = new List<ResultConsolidatedController.HomeResultConsolidated>();
                        listaIndicadorAcesso = ResultConsolidatedController.ReturnHomeResultConsolidatedAccess("", lesc.DATA_INICIO, lesc.DATA_FIM, "", lesc.IDSETOR.ToString(), lesc.IDSUBSETOR.ToString(), "", "", "", true, filterEnv);
                        rmams = rmams.Concat(listaIndicadorAcesso).ToList();
                    }

                    if (rmams.Count > 0)
                    {
                        ResultConsolidatedController.HomeResultConsolidated itemDistante = rmams.OrderByDescending(item => Math.Abs(item.FACTOR0 - item.FACTOR1)).First();
                        itemDistante = ResultConsolidatedController.DoCalculateFinal(itemDistante);

                        ResultConsolidatedController.HomeResultConsolidated itemProximo = rmams.OrderBy(item => Math.Abs(item.FACTOR0 - item.FACTOR1)).First();
                        itemProximo = ResultConsolidatedController.DoCalculateFinal(itemProximo);

                        lesc.DISPERSAO_RESULTADO = Math.Abs(itemDistante.RESULTADO - itemProximo.RESULTADO);

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

                        ResultConsolidatedController.HomeResultConsolidated consolidado = rmams[0];
                        rmams[0] = ResultConsolidatedController.DoCalculateFinal(consolidado);

                        double resultado = rmams[0].RESULTADO;
                        double meta = rmams[0].META;
                        double percentual = rmams[0].PERCENTUAL;
                        string biggerBetter = rmams[0].BETTER;
                        double atingimento = 0;//100 - percentual;
                        string statusResult = "";
                        if (percentual > 100)
                        {
                            statusResult = "Dentro da meta";
                            atingimento = percentual - 100;
                        }
                        else
                        {
                            statusResult = "Fora da meta";
                            atingimento = 100 - percentual;
                        }
                        lesc.RESULTADO_STATUS = statusResult;
                        lesc.DIFERENCA_META = atingimento;
                    }
                    

                }





            }
            return lEscs;

        }

    }
}
