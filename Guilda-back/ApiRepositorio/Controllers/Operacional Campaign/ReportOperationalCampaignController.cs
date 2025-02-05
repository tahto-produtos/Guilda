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
using DocumentFormat.OpenXml.Office2019.Presentation;
using System.Threading;
using static ApiRepositorio.Controllers.ReportOperationalCampaignController;
using static ApiRepositorio.Controllers.TesteController;
using static ApiRepositorio.Controllers.IntegracaoAPIResultController;
using static TokenService;

//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ReportOperationalCampaignController : ApiController
    {// POST: api/Results

        public class listOperationalCampaign
        {
            public string NOME_CAMPANHA { get; set; }
            public string CRIADO_POR { get; set; }
            public string DATA_INICIO { get; set; }
            public string DATA_FIM { get; set; }
            public int IDGDA_INDICATOR { get; set; }
            public string IDGDA_COLLABORATOR { get; set; }
            public string NOME_INDICADOR { get; set; }
            public string NOME_COLABORADOR { get; set; }
            public string RANKING_COLABORADOR { get; set; }
            public string STATUS_CAMPANHA { get; set; }

            public string HIERARCHY { get; set; }
            public string RESULTADO_INICIAL { get; set; }
            public string RESULTADO_ATUAL { get; set; }
            public string PORCENTAGEM_EVOLUCAO { get; set; }           
        }

        public class inputListReportOperationalCampaign
        {
            public DateTime STARTEDATFROM { get; set; }
            public DateTime STARTEDATTO { get; set; }
            public DateTime ENDEDATFROM { get; set; }
            public DateTime ENDEDATTO { get; set; }
            public List<int> INDICATORSID { get; set; }}
    

    [HttpPost]
        public IHttpActionResult GetResultsModel([FromBody] inputListReportOperationalCampaign inputModel)
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



            if (inputModel.STARTEDATTO.ToString() != "")
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

            if (inputModel.ENDEDATTO.ToString() != "")
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

            var jsonData = BancoReportOperationalCampaign.relOperationalCampaign(inputModel);

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(jsonData);
        }
        // Método para serializar um DataTable em JSON


    }

    public class BancoReportOperationalCampaign
    {

        public static List<ReportOperationalCampaignController.listOperationalCampaign> relOperationalCampaign(inputListReportOperationalCampaign inputModel)
        {

            List<listOperationalCampaign> rmams = new List<listOperationalCampaign>();
            rmams = returnOperationalCampaign(inputModel);

            //var jsonData = rmams.Select(item => new ReportActionEscalationController.returnResponseDay
            //{

            //}).ToList();

            return rmams;

        }

        public static List<listOperationalCampaign> returnOperationalCampaign(inputListReportOperationalCampaign inputModel)
        {
            string filter = "";
            string indicatorsAsString = string.Join(",", inputModel.INDICATORSID);
            string dtInicialEm = inputModel.STARTEDATFROM.ToString("yyyy-MM-dd");
            string dtInicialAte = inputModel.STARTEDATTO.ToString("yyyy-MM-dd");
            string dtFinalEm = inputModel.ENDEDATFROM.ToString("yyyy-MM-dd");
            string dtFinalAte = inputModel.ENDEDATTO.ToString("yyyy-MM-dd");


            if (dtInicialEm != "0001-01-01")
            {
                filter = $" AND OC.STARTED_AT >= '{dtInicialEm}' AND OC.STARTED_AT <= '{dtInicialAte}' ";
            }
            if (dtFinalEm != "0001-01-01")
            {
                filter = $"{filter} AND OC.ENDED_AT >= '{dtFinalEm}' AND OC.ENDED_AT <= '{dtFinalAte}' ";
            }
            if (indicatorsAsString != "")
            {
                filter = $"{filter} AND IND.IDGDA_INDICATOR IN ({indicatorsAsString}) ";
            }           

            StringBuilder sb = new StringBuilder();
            //sb.Append("SELECT  ");
            //sb.Append("		OC.NAME, ");
            //sb.Append("		PU1.NAME AS CRIADO_POR, ");
            //sb.Append("		OC.STARTED_AT AS DATA_INICIO, ");
            //sb.Append("		OC.ENDED_AT AS DATA_FINAL, ");
            //sb.Append("		PCU.IDGDA_COLLABORATORS, ");
            //sb.Append("		PU2.NAME AS NOME_COLABORADOR, ");
            //sb.Append("		OCUP.POSITION AS RANKING, ");
            //sb.Append("		CASE WHEN CONVERT(DATE,GETDATE()) < CONVERT(DATE,STARTED_AT) THEN 'EM ABERTO' ");
            //sb.Append("		WHEN CONVERT (DATE,GETDATE()) >= CONVERT(DATE,OC.STARTED_AT) AND CONVERT (DATE,GETDATE()) <= CONVERT(DATE,OC.ENDED_AT) THEN 'EM ANDAMENTO' ");
            //sb.Append("		WHEN CONVERT (DATE,OC.ENDED_AT) < CONVERT(DATE,GETDATE()) THEN 'CONCLUIDO' END AS STATUS_CAMPANHA, ");
            //sb.Append("		IND.IDGDA_INDICATOR, ");
            //sb.Append("		IND.NAME AS INDICADOR ");
            //sb.Append("FROM GDA_OPERATIONAL_CAMPAIGN (NOLOCK) OC ");
            //sb.Append("LEFT JOIN GDA_PERSONA_USER (NOLOCK) AS PU1 ON PU1.IDGDA_PERSONA_USER = OC.CREATED_BY ");
            //sb.Append("LEFT JOIN GDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT (NOLOCK) AS OCUP ON OCUP.IDGDA_OPERATIONAL_CAMPAIGN = OC.IDGDA_OPERATIONAL_CAMPAIGN ");
            //sb.Append("INNER JOIN GDA_PERSONA_USER (NOLOCK) AS PU2 ON PU2.IDGDA_PERSONA_USER = OCUP.IDGDA_PERSONA ");
            //sb.Append("INNER JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCU ON PCU.IDGDA_PERSONA_USER = PU2.IDGDA_PERSONA_USER ");
            //sb.Append("LEFT JOIN GDA_OPERATIONAL_CAMPAIGN_PONTUATION (NOLOCK) AS OCP ON OCP.IDGDA_OPERATIONAL_CAMPAIGN = OC.IDGDA_OPERATIONAL_CAMPAIGN ");
            //sb.Append("LEFT JOIN GDA_INDICATOR (NOLOCK) AS IND ON IND.IDGDA_INDICATOR = OCP.IDGDA_INDICATOR ");
            //sb.Append(" ");
            //sb.Append($"WHERE 1 = 1 {filter} ");



            sb.Append($"SELECT OC.IDGDA_OPERATIONAL_CAMPAIGN, ");
            sb.Append($"	   MAX(OC.NAME) AS NAME, ");
            sb.Append($"        MAX(PU1.NAME) AS CRIADO_POR, ");
            sb.Append($"        MAX(OC.STARTED_AT) AS DATA_INICIO, ");
            sb.Append($"        MAX(OC.ENDED_AT) AS DATA_FINAL, ");
            sb.Append($"       PCU.IDGDA_COLLABORATORS, ");
            sb.Append($"        MAX(PU2.NAME) AS NOME_COLABORADOR, ");
            sb.Append($"        MAX(OCUP.POSITION) AS RANKING, ");
            sb.Append($"	    MAX(HH.IDGDA_HIERARCHY) AS HIERARCHY, ");
            sb.Append($"       CASE ");
            sb.Append($"           WHEN CONVERT(DATE,GETDATE()) < CONVERT(DATE,STARTED_AT) THEN 'EM ABERTO' ");
            sb.Append($"           WHEN CONVERT (DATE,GETDATE()) >= CONVERT(DATE,OC.STARTED_AT) ");
            sb.Append($"                AND CONVERT (DATE,GETDATE()) <= CONVERT(DATE,OC.ENDED_AT) THEN 'EM ANDAMENTO' ");
            sb.Append($"           WHEN CONVERT (DATE,OC.ENDED_AT) < CONVERT(DATE,GETDATE()) THEN 'CONCLUIDO' ");
            sb.Append($"       END AS STATUS_CAMPANHA, ");
            sb.Append($"       MAX(IND.IDGDA_INDICATOR) AS IDGDA_INDICATOR, ");
            sb.Append($"       MAX(IND.NAME) AS INDICADOR ");
            sb.Append($"FROM GDA_OPERATIONAL_CAMPAIGN (NOLOCK) OC ");
            sb.Append($"LEFT JOIN GDA_PERSONA_USER (NOLOCK) AS PU1 ON PU1.IDGDA_PERSONA_USER = OC.CREATED_BY ");
            sb.Append($"LEFT JOIN GDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT (NOLOCK) AS OCUP ON OCUP.IDGDA_OPERATIONAL_CAMPAIGN = OC.IDGDA_OPERATIONAL_CAMPAIGN ");
            sb.Append($"INNER JOIN GDA_PERSONA_USER (NOLOCK) AS PU2 ON PU2.IDGDA_PERSONA_USER = OCUP.IDGDA_PERSONA ");
            sb.Append($"INNER JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCU ON PCU.IDGDA_PERSONA_USER = PU2.IDGDA_PERSONA_USER ");
            sb.Append($"LEFT JOIN GDA_OPERATIONAL_CAMPAIGN_PONTUATION (NOLOCK) AS OCP ON OCP.IDGDA_OPERATIONAL_CAMPAIGN = OC.IDGDA_OPERATIONAL_CAMPAIGN ");
            sb.Append($"LEFT JOIN GDA_INDICATOR (NOLOCK) AS IND ON IND.IDGDA_INDICATOR = OCP.IDGDA_INDICATOR ");
            sb.Append($"LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) CD ON CD.CREATED_AT >= CONVERT(DATE, DATEADD(DAY, -2, GETDATE())) AND CD.IDGDA_COLLABORATORS = PCU.IDGDA_COLLABORATORS ");
            sb.Append($"LEFT JOIN GDA_HIERARCHY (NOLOCK) HH ON HH.LEVELNAME = CD.CARGO ");
            sb.Append($" ");
            sb.Append($"WHERE 1 = 1 {filter} ");
            sb.Append($"  GROUP BY OC.IDGDA_OPERATIONAL_CAMPAIGN, PCU.IDGDA_COLLABORATORS, OC.STARTED_AT, OC.ENDED_AT ");

            List<listOperationalCampaign> lEscs = new List<listOperationalCampaign>();
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
                            listOperationalCampaign lEsc = new listOperationalCampaign();
                            lEsc.NOME_CAMPANHA = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "";
                            lEsc.CRIADO_POR = reader["CRIADO_POR"] != DBNull.Value ? reader["CRIADO_POR"].ToString() : "";
                            lEsc.DATA_INICIO = reader["DATA_INICIO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_INICIO"]).ToString("yyyy-MM-dd") : "";
                            lEsc.DATA_FIM = reader["DATA_FINAL"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_FINAL"]).ToString("yyyy-MM-dd") : "";
                            lEsc.IDGDA_COLLABORATOR = reader["IDGDA_COLLABORATORS"] != DBNull.Value ? reader["IDGDA_COLLABORATORS"].ToString() : "";
                            lEsc.NOME_COLABORADOR = reader["NOME_COLABORADOR"] != DBNull.Value ? reader["NOME_COLABORADOR"].ToString() : "";
                            lEsc.RANKING_COLABORADOR = reader["RANKING"] != DBNull.Value ? reader["RANKING"].ToString() : "";
                            lEsc.STATUS_CAMPANHA = reader["STATUS_CAMPANHA"] != DBNull.Value ? reader["STATUS_CAMPANHA"].ToString() : "";
                            lEsc.HIERARCHY = reader["HIERARCHY"] != DBNull.Value ? reader["HIERARCHY"].ToString() : "";
                            lEsc.IDGDA_INDICATOR = reader["IDGDA_INDICATOR"] != DBNull.Value ? int.Parse(reader["IDGDA_INDICATOR"].ToString()) : 0;
                            lEsc.NOME_INDICADOR = reader["INDICADOR"] != DBNull.Value ? reader["INDICADOR"].ToString() : "";                                                                               
                            lEscs.Add(lEsc);
                        }
                    }
                }
                connection.Close();           
            }
          

            for (int i = 0; i < lEscs.Count; i++)
            {
                listOperationalCampaign user = lEscs[i];

                List<ResultConsolidatedController.HomeResultConsolidated> resultadoPrimeiroDiaCampanha = new List<ResultConsolidatedController.HomeResultConsolidated>();
                resultadoPrimeiroDiaCampanha = ResultConsolidatedController.ReturnHomeResultConsolidated(user.IDGDA_COLLABORATOR, Convert.ToDateTime(user.DATA_INICIO).ToString("yyyy-MM-dd"), Convert.ToDateTime(user.DATA_FIM).ToString("yyyy-MM-dd"), user.HIERARCHY, user.IDGDA_INDICATOR.ToString(), "", "", "", "", "", true, "");

                List<ResultConsolidatedController.HomeResultConsolidated> resultadoDuranteCampanha = new List<ResultConsolidatedController.HomeResultConsolidated>();
                resultadoDuranteCampanha = ResultConsolidatedController.ReturnHomeResultConsolidated(user.IDGDA_COLLABORATOR, Convert.ToDateTime(user.DATA_INICIO).ToString("yyyy-MM-dd"), Convert.ToDateTime(user.DATA_FIM).ToString("yyyy-MM-dd"), user.HIERARCHY, user.IDGDA_INDICATOR.ToString(), "", "", "" , "", "", true, "");

                if (resultadoPrimeiroDiaCampanha.Count ==0)
                {
                    continue;
                }

                ResultConsolidatedController.HomeResultConsolidated primResult = new ResultConsolidatedController.HomeResultConsolidated();
                ResultConsolidatedController.HomeResultConsolidated segundResult = new ResultConsolidatedController.HomeResultConsolidated();
                primResult = resultadoPrimeiroDiaCampanha.Find(kkk => kkk.MATRICULA == user.IDGDA_COLLABORATOR.ToString() && kkk.IDINDICADOR == user.IDGDA_INDICATOR.ToString());
                segundResult = resultadoDuranteCampanha.Find(kkk => kkk.MATRICULA == user.IDGDA_COLLABORATOR.ToString() && kkk.IDINDICADOR == user.IDGDA_INDICATOR.ToString());

                ResultConsolidatedController.DoCalculateFinal(primResult);
                ResultConsolidatedController.DoCalculateFinal(segundResult);

                user.RESULTADO_INICIAL = primResult.RESULTADO.ToString();
                user.RESULTADO_ATUAL = segundResult.RESULTADO.ToString();
                double percentAltered = 0;
                percentAltered = segundResult.PERCENTUAL - primResult.PERCENTUAL;
                user.PORCENTAGEM_EVOLUCAO = percentAltered.ToString();
                lEscs[i] = user;
        }

            return lEscs;
        }

    }
}
