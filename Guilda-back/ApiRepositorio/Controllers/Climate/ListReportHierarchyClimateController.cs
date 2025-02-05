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
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    public class returnReportHierarchyClimate
    {
        public int TOTALPAGES { get; set; }
        public List<returnListsReportHierarchyClimate> resposts { get; set; }
    }

    public class returnListsReportHierarchyClimate
    {
        public int idUserClimate { get; set; }
        public string data { get; set; }
        public string name { get; set; }
        public string BC { get; set; }
        public string climate { get; set; }
        public string reason { get; set; }
        public string applyType { get; set; }
        public bool canApply { get; set; }
        public string nomeSupervisor { get; set; }
        public string nomeCoordenador { get; set; }
        public string nomeGerenteII { get; set; }
        public string nomeGerenteI { get; set; }
        public string nomeDiretor { get; set; }
        public string nomeCeo { get; set; }
    }

    public class inputListReportHierarchyClimate
    {
        public DateTime STARTEDATFROM { get; set; }
        public DateTime STARTEDATTO { get; set; }
        public List<int> PERSONASID { get; set; }
        public List<int> SECTORSID { get; set; }
        public int FLAGRESPONSE { get; set; }
        public int FLAGNORESPONSE { get; set; }
        public int FLAGCANFEEDBACK { get; set; }
        public int limit { get; set; }
        public int page { get; set; }
    }

    //[Authorize]
    public class ListReportHierarchyClimateController : ApiController
    {// POST: api/Results
        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] inputListReportHierarchyClimate inputModel)
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

            bool adm = Funcoes.retornaPermissao(COLLABORATORID.ToString());
            if (adm == true)
            {
                //COLOCA O ID DO CEO, POIS TERA A MESMA VISÃO
                COLLABORATORID = 756399;
            }


            returnReportHierarchyClimate rmams = new returnReportHierarchyClimate();
            rmams = BankListReportHierarchyClimate.listReportHierarchyClimate(COLLABORATORID, inputModel);

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(rmams);
        }
        // Método para serializar um DataTable em JSON
    }

    public class BankListReportHierarchyClimate
    {

        public static int quantidadeReportHierarchy(int idCollaborator, string dtInicial, string dtFinal, string filtro, string filtroSetor)
        {
            int total = 0;

            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                connection.Open();
                try
                {
         
                    StringBuilder sb = new StringBuilder();

                    sb.Append($"DECLARE @ID INT; SET @ID = '{idCollaborator}'; ");
                    sb.Append($"DECLARE @DATE_INI DATE; SET @DATE_INI = '{dtInicial}'; ");
                    sb.Append($"DECLARE @DATE_FIM DATE; SET @DATE_FIM = '{dtFinal}'; ");
                    sb.Append("SELECT COUNT(0) AS CONTA ");
                    sb.Append("FROM ");
                    sb.Append("  (SELECT CDI2.IDGDA_COLLABORATORS, ");
                    sb.Append("          CONVERT(DATE, CDI2.CREATED_AT) AS CREATED_AT, ");
                    sb.Append("          MAX(CDI2.MATRICULA_SUPERVISOR) AS MATRICULA_SUPERVISOR, ");
                    sb.Append("          MAX(CDI2.NOME_SUPERVISOR) AS NOME_SUPERVISOR, ");
                    sb.Append("          MAX(CDI2.MATRICULA_COORDENADOR) AS MATRICULA_COORDENADOR, ");
                    sb.Append("          MAX(CDI2.NOME_COORDENADOR) AS NOME_COORDENADOR, ");
                    sb.Append("          MAX(CDI2.MATRICULA_GERENTE_II) AS MATRICULA_GERENTE_II, ");
                    sb.Append("          MAX(CDI2.NOME_GERENTE_II) AS NOME_GERENTE_II, ");
                    sb.Append("          MAX(CDI2.MATRICULA_GERENTE_I) AS MATRICULA_GERENTE_I, ");
                    sb.Append("          MAX(CDI2.NOME_GERENTE_I) AS NOME_GERENTE_I, ");
                    sb.Append("          MAX(CDI2.MATRICULA_DIRETOR) AS MATRICULA_DIRETOR, ");
                    sb.Append("          MAX(CDI2.NOME_DIRETOR) AS NOME_DIRETOR, ");
                    sb.Append("          MAX(CDI2.MATRICULA_CEO) AS MATRICULA_CEO, ");
                    sb.Append("          MAX(CDI2.NOME_CEO) AS NOME_CEO ");
                    sb.Append("   FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CDI2 ");
                    sb.Append("   WHERE CDI2.CREATED_AT >= @DATE_INI AND CDI2.CREATED_AT <= @DATE_FIM ");
                    sb.Append("   AND (CDI2.IDGDA_COLLABORATORS = @ID ");
                    sb.Append("       OR CDI2.MATRICULA_SUPERVISOR = @ID ");
                    sb.Append("       OR CDI2.MATRICULA_COORDENADOR = @ID ");
                    sb.Append("       OR CDI2.MATRICULA_GERENTE_II = @ID ");
                    sb.Append("       OR CDI2.MATRICULA_GERENTE_I = @ID ");
                    sb.Append("       OR CDI2.MATRICULA_DIRETOR = @ID ");
                    sb.Append("       OR CDI2.MATRICULA_CEO = @ID) ");
                    sb.Append($"    {filtroSetor} ");
                    //sb.Append("     AND CDI2.CARGO = 'AGENTE' ");
                    sb.Append("   GROUP BY CDI2.IDGDA_COLLABORATORS, ");
                    sb.Append("            CONVERT(DATE, CDI2.CREATED_AT) ");

                    sb.Append("  ) AS CD ");
                    sb.Append("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCU ON PCU.IDGDA_COLLABORATORS = CD.IDGDA_COLLABORATORS ");
                    sb.Append("LEFT JOIN GDA_PERSONA_USER (NOLOCK) AS PU ON PU.IDGDA_PERSONA_USER = PCU.IDGDA_PERSONA_USER ");
                    sb.Append("AND PU.IDGDA_PERSONA_USER_TYPE = 1 ");
                    sb.Append("LEFT JOIN GDA_CLIMATE_USER (NOLOCK) AS GCU ON GCU.IDGDA_PERSONA = PU.IDGDA_PERSONA_USER ");
                    sb.Append("AND CONVERT(DATE, GCU.CREATED_AT) = CONVERT(DATE, CD.CREATED_AT) ");
                    sb.Append("LEFT JOIN GDA_CLIMATE (NOLOCK) AS C ON C.IDGDA_CLIMATE = GCU.IDGDA_CLIMATE ");
                    sb.Append("LEFT JOIN GDA_CLIMATE_REASON (NOLOCK) AS CR ON CR.IDGDA_CLIMATE_REASON = GCU.IDGDA_CLIMATE_REASON ");
                    sb.Append("LEFT JOIN GDA_CLIMATE_APPLY_TYPE (NOLOCK) AS AT ON AT.IDGDA_CLIMATE_APPLY_TYPE = GCU.IDGDA_CLIMATE_APPLY_TYPE ");
                    sb.Append("WHERE BC IS NOT NULL ");
                    sb.Append("  AND CONVERT(DATE, CD.CREATED_AT) >= @DATE_INI AND CONVERT(DATE, CD.CREATED_AT) <= @DATE_FIM ");
                    sb.Append($" {filtro} ");
     


                    //sb.Append($"DECLARE @ID INT; SET @ID = '{idCollaborator}'; ");
                    //sb.Append($"DECLARE @DATE_INI DATE; SET @DATE_INI = '{dtInicial}'; ");
                    //sb.Append($"DECLARE @DATE_FIM DATE; SET @DATE_FIM = '{dtFinal}'; ");
                    //sb.Append($"SELECT COUNT(0) AS CONTA ");
                    //sb.Append($"FROM  ");
                    //sb.Append($"(  ");
                    //sb.Append($"SELECT  ");
                    //sb.Append($"CDI2.IDGDA_COLLABORATORS, CONVERT(DATE, CDI2.CREATED_AT) AS CREATED_AT,  ");
                    //sb.Append($"MAX(CDI2.MATRICULA_SUPERVISOR) AS MATRICULA_SUPERVISOR, MAX(CDI2.NOME_SUPERVISOR) AS NOME_SUPERVISOR,  ");
                    //sb.Append($"MAX(CDI2.MATRICULA_COORDENADOR) AS MATRICULA_COORDENADOR,  MAX(CDI2.NOME_COORDENADOR) AS NOME_COORDENADOR,  ");
                    //sb.Append($"MAX(CDI2.MATRICULA_GERENTE_II) AS MATRICULA_GERENTE_II,  MAX(CDI2.NOME_GERENTE_II) AS NOME_GERENTE_II, ");
                    //sb.Append($"MAX(CDI2.MATRICULA_GERENTE_I) AS MATRICULA_GERENTE_I, MAX(CDI2.NOME_GERENTE_I) AS NOME_GERENTE_I,  ");
                    //sb.Append($"MAX(CDI2.MATRICULA_DIRETOR) AS MATRICULA_DIRETOR, MAX(CDI2.NOME_DIRETOR) AS NOME_DIRETOR,  ");
                    //sb.Append($"MAX(CDI2.MATRICULA_CEO) AS MATRICULA_CEO, MAX(CDI2.NOME_CEO) AS NOME_CEO ");
                    //sb.Append($"FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CDI2 ");
                    //sb.Append($"WHERE CDI2.CREATED_AT >= @DATE_INI AND CDI2.CREATED_AT <= @DATE_FIM   ");
                    //sb.Append($"{filtroSetor} ");
                    //sb.Append($"AND CDI2.CARGO = 'AGENTE' ");
                    //sb.Append($"GROUP BY CDI2.IDGDA_COLLABORATORS, CONVERT(DATE, CDI2.CREATED_AT) ");
                    //sb.Append($"UNION ALL ");
                    //sb.Append($"SELECT  ");
                    //sb.Append($"CDI.IDGDA_COLLABORATORS, CONVERT(DATE, CDI.CREATED_AT) AS CREATED_AT,  ");
                    //sb.Append($"MAX(CDI.MATRICULA_SUPERVISOR) AS MATRICULA_SUPERVISOR, MAX(CDI.NOME_SUPERVISOR) AS NOME_SUPERVISOR,  ");
                    //sb.Append($"MAX(CDI.MATRICULA_COORDENADOR) AS MATRICULA_COORDENADOR,  MAX(CDI.NOME_COORDENADOR) AS NOME_COORDENADOR,  ");
                    //sb.Append($"MAX(CDI.MATRICULA_GERENTE_II) AS MATRICULA_GERENTE_II,  MAX(CDI.NOME_GERENTE_II) AS NOME_GERENTE_II, ");
                    //sb.Append($"MAX(CDI.MATRICULA_GERENTE_I) AS MATRICULA_GERENTE_I, MAX(CDI.NOME_GERENTE_I) AS NOME_GERENTE_I,  ");
                    //sb.Append($"MAX(CDI.MATRICULA_DIRETOR) AS MATRICULA_DIRETOR, MAX(CDI.NOME_DIRETOR) AS NOME_DIRETOR,  ");
                    //sb.Append($"MAX(CDI.MATRICULA_CEO) AS MATRICULA_CEO, MAX(CDI.NOME_CEO) AS NOME_CEO ");
                    //sb.Append($"FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CDI ");
                    //sb.Append($"INNER JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CDI2 ON  CONVERT(DATE, CDI.CREATED_AT) = CONVERT(DATE, CDI2.CREATED_AT) AND  ");
                    //sb.Append($"(CDI2.MATRICULA_SUPERVISOR = CDI.IDGDA_COLLABORATORS OR  ");
                    //sb.Append($"		CDI2.MATRICULA_COORDENADOR = CDI.IDGDA_COLLABORATORS OR  ");
                    //sb.Append($"		CDI2.MATRICULA_GERENTE_II = CDI.IDGDA_COLLABORATORS OR  ");
                    //sb.Append($"		CDI2.MATRICULA_GERENTE_I = CDI.IDGDA_COLLABORATORS OR  ");
                    //sb.Append($"		CDI2.MATRICULA_DIRETOR = CDI.IDGDA_COLLABORATORS OR  ");
                    //sb.Append($"		CDI2.MATRICULA_CEO = CDI.IDGDA_COLLABORATORS) AND CDI2.CREATED_AT >= @DATE_INI AND CDI2.CREATED_AT <= @DATE_FIM ");
                    //sb.Append($"WHERE CDI.CREATED_AT >= @DATE_INI AND CDI.CREATED_AT <= @DATE_FIM  ");
                    //sb.Append($"AND CDI.CARGO != 'AGENTE' AND CDI.CARGO IS NOT NULL {filtroSetor} ");
                    //sb.Append($"GROUP BY CDI.IDGDA_COLLABORATORS, CONVERT(DATE, CDI.CREATED_AT) ");
                    //sb.Append($"UNION ALL ");
                    //sb.Append($"SELECT  ");
                    //sb.Append($"CDI2.IDGDA_COLLABORATORS, CONVERT(DATE, GETDATE()) AS CREATED_AT,  ");
                    //sb.Append($"MAX(CDI2.MATRICULA_SUPERVISOR) AS MATRICULA_SUPERVISOR, MAX(CDI2.NOME_SUPERVISOR) AS NOME_SUPERVISOR,  ");
                    //sb.Append($"MAX(CDI2.MATRICULA_COORDENADOR) AS MATRICULA_COORDENADOR,  MAX(CDI2.NOME_COORDENADOR) AS NOME_COORDENADOR,  ");
                    //sb.Append($"MAX(CDI2.MATRICULA_GERENTE_II) AS MATRICULA_GERENTE_II,  MAX(CDI2.NOME_GERENTE_II) AS NOME_GERENTE_II, ");
                    //sb.Append($"MAX(CDI2.MATRICULA_GERENTE_I) AS MATRICULA_GERENTE_I, MAX(CDI2.NOME_GERENTE_I) AS NOME_GERENTE_I,  ");
                    //sb.Append($"MAX(CDI2.MATRICULA_DIRETOR) AS MATRICULA_DIRETOR, MAX(CDI2.NOME_DIRETOR) AS NOME_DIRETOR,  ");
                    //sb.Append($"MAX(CDI2.MATRICULA_CEO) AS MATRICULA_CEO, MAX(CDI2.NOME_CEO) AS NOME_CEO ");
                    //sb.Append($"FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CDI2 ");
                    //sb.Append($"WHERE CDI2.CREATED_AT >= CONVERT(DATE, DATEADD(DAY, -1, GETDATE()))  ");
                    //sb.Append($"{filtroSetor} ");
                    //sb.Append($"AND CDI2.CARGO = 'AGENTE' ");
                    //sb.Append($"GROUP BY CDI2.IDGDA_COLLABORATORS ");
                    //sb.Append($"UNION ALL ");
                    //sb.Append($"SELECT  ");
                    //sb.Append($"CDI.IDGDA_COLLABORATORS, CONVERT(DATE, GETDATE()) AS CREATED_AT,  ");
                    //sb.Append($"MAX(CDI.MATRICULA_SUPERVISOR) AS MATRICULA_SUPERVISOR, MAX(CDI.NOME_SUPERVISOR) AS NOME_SUPERVISOR,  ");
                    //sb.Append($"MAX(CDI.MATRICULA_COORDENADOR) AS MATRICULA_COORDENADOR,  MAX(CDI.NOME_COORDENADOR) AS NOME_COORDENADOR,  ");
                    //sb.Append($"MAX(CDI.MATRICULA_GERENTE_II) AS MATRICULA_GERENTE_II,  MAX(CDI.NOME_GERENTE_II) AS NOME_GERENTE_II, ");
                    //sb.Append($"MAX(CDI.MATRICULA_GERENTE_I) AS MATRICULA_GERENTE_I, MAX(CDI.NOME_GERENTE_I) AS NOME_GERENTE_I,  ");
                    //sb.Append($"MAX(CDI.MATRICULA_DIRETOR) AS MATRICULA_DIRETOR, MAX(CDI.NOME_DIRETOR) AS NOME_DIRETOR,  ");
                    //sb.Append($"MAX(CDI.MATRICULA_CEO) AS MATRICULA_CEO, MAX(CDI.NOME_CEO) AS NOME_CEO ");
                    //sb.Append($"FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CDI ");
                    //sb.Append($"INNER JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CDI2 ON  CONVERT(DATE, CDI.CREATED_AT) = CONVERT(DATE, CDI2.CREATED_AT) AND  ");
                    //sb.Append($"(CDI2.MATRICULA_SUPERVISOR = CDI.IDGDA_COLLABORATORS OR  ");
                    //sb.Append($"		CDI2.MATRICULA_COORDENADOR = CDI.IDGDA_COLLABORATORS OR  ");
                    //sb.Append($"		CDI2.MATRICULA_GERENTE_II = CDI.IDGDA_COLLABORATORS OR  ");
                    //sb.Append($"		CDI2.MATRICULA_GERENTE_I = CDI.IDGDA_COLLABORATORS OR  ");
                    //sb.Append($"		CDI2.MATRICULA_DIRETOR = CDI.IDGDA_COLLABORATORS OR  ");
                    //sb.Append($"		CDI2.MATRICULA_CEO = CDI.IDGDA_COLLABORATORS) AND CDI2.CREATED_AT >= CONVERT(DATE, DATEADD(DAY, -1, GETDATE())) ");
                    //sb.Append($"WHERE CDI.CREATED_AT >= CONVERT(DATE, DATEADD(DAY, -1, GETDATE()))  ");
                    //sb.Append($"AND CDI.CARGO != 'AGENTE' AND CDI.CARGO IS NOT NULL {filtroSetor} ");
                    //sb.Append($"GROUP BY CDI.IDGDA_COLLABORATORS ");
                    //sb.Append($") AS CD ");
                    //sb.Append($"LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCU ON PCU.IDGDA_COLLABORATORS = CD.IDGDA_COLLABORATORS ");
                    //sb.Append($"LEFT JOIN GDA_PERSONA_USER (NOLOCK) AS PU ON PU.IDGDA_PERSONA_USER = PCU.IDGDA_PERSONA_USER AND PU.IDGDA_PERSONA_USER_TYPE = 1  ");
                    //sb.Append($"LEFT JOIN GDA_CLIMATE_USER (NOLOCK) AS GCU ON GCU.IDGDA_PERSONA = PU.IDGDA_PERSONA_USER AND CONVERT(DATE, GCU.CREATED_AT) = CONVERT(DATE, CD.CREATED_AT) ");
                    //sb.Append($"LEFT JOIN GDA_CLIMATE (NOLOCK) AS C ON C.IDGDA_CLIMATE = GCU.IDGDA_CLIMATE  ");
                    //sb.Append($"LEFT JOIN GDA_CLIMATE_REASON (NOLOCK) AS CR ON CR.IDGDA_CLIMATE_REASON = GCU.IDGDA_CLIMATE_REASON ");
                    //sb.Append($"LEFT JOIN GDA_CLIMATE_APPLY_TYPE (NOLOCK) AS AT ON AT.IDGDA_CLIMATE_APPLY_TYPE = GCU.IDGDA_CLIMATE_APPLY_TYPE ");
                    //sb.Append($" ");
                    //sb.Append($"WHERE (CD.IDGDA_COLLABORATORS = @ID OR  ");
                    //sb.Append($"		CD.MATRICULA_SUPERVISOR = @ID OR  ");
                    //sb.Append($"		CD.MATRICULA_COORDENADOR = @ID OR  ");
                    //sb.Append($"		CD.MATRICULA_GERENTE_II = @ID OR  ");
                    //sb.Append($"		CD.MATRICULA_GERENTE_I = @ID OR  ");
                    //sb.Append($"		CD.MATRICULA_DIRETOR = @ID OR  ");
                    //sb.Append($"		CD.MATRICULA_CEO = @ID) ");
                    //sb.Append($"AND BC IS NOT NULL ");
                    //sb.Append($"AND CONVERT(DATE, CD.CREATED_AT) >= @DATE_INI AND CONVERT(DATE, CD.CREATED_AT) <= @DATE_FIM {filtro} ");
                    //sb.Append($"GROUP BY CONVERT(DATE, CD.CREATED_AT), CD.IDGDA_COLLABORATORS, PU.IDGDA_PERSONA_USER ");
                    //sb.Append($"ORDER BY CONVERT(DATE, CD.CREATED_AT), CD.IDGDA_COLLABORATORS ");

                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        command.CommandTimeout = 0;
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

        public static returnReportHierarchyClimate listReportHierarchyClimate(int idCollaborator, inputListReportHierarchyClimate inputModel)
        {

            returnReportHierarchyClimate retorno = new returnReportHierarchyClimate();
            retorno.resposts = new List<returnListsReportHierarchyClimate>();

            string dtInicial = inputModel.STARTEDATFROM.ToString("yyyy-MM-dd");
            string dtFinal = inputModel.STARTEDATTO.ToString("yyyy-MM-dd");

            string personasAsString = string.Join(",", inputModel.PERSONASID);
            string sectorsAsString = string.Join(",", inputModel.SECTORSID);
            string filtro = "";
            string filtroSetor = "";


            filtro = personasAsString != "" ? $"{filtro} AND PU.IDGDA_PERSONA_USER IN ({personasAsString}) " : filtro;
            filtroSetor = sectorsAsString != "" ? $"{filtroSetor} AND CDI2.IDGDA_SECTOR IN ({sectorsAsString}) " : filtroSetor;

            filtro = inputModel.FLAGRESPONSE == 1 ? $"{filtro} AND C.CLIMATE IS NOT NULL " : filtro;
            filtro = inputModel.FLAGNORESPONSE == 1 ? $"{filtro} AND C.CLIMATE IS NULL " : filtro;
            filtro = inputModel.FLAGCANFEEDBACK == 1 ? $"{filtro} AND C.CAN_APPLY = 1 AND GCU.IDGDA_CLIMATE_APPLY_TYPE IS NULL " : filtro;

            int totalInfo = quantidadeReportHierarchy(idCollaborator, dtInicial, dtFinal, filtro, filtroSetor);
            int totalpage = (int)Math.Ceiling((double)totalInfo / inputModel.limit);

            int offset = (inputModel.page - 1) * inputModel.limit;
            retorno.TOTALPAGES = totalpage;

            string filterRowSet = $"OFFSET {offset} ROWS FETCH NEXT {inputModel.limit} ROWS ONLY ";

            retorno.resposts = returnListReportHierarchy(idCollaborator, dtInicial, dtFinal, filtroSetor, filtro, filterRowSet);

            return retorno;
        }

        public static List<returnListsReportHierarchyClimate> returnListReportHierarchy(int idCollaborator, string dtInicial, string dtFinal, string filtroSetor, string filtro, string filterRowSet)
        {

            List<returnListsReportHierarchyClimate> resposts = new List<returnListsReportHierarchyClimate>();

            StringBuilder sb = new StringBuilder();
            sb.Append($"DECLARE @ID INT; SET @ID = '{idCollaborator}'; ");
            sb.Append($"DECLARE @DATE_INI DATE; SET @DATE_INI = '{dtInicial}'; ");
            sb.Append($"DECLARE @DATE_FIM DATE; SET @DATE_FIM = '{dtFinal}'; ");
            sb.Append("SELECT GCU.IDGDA_CLIMATE_USER, ");
            sb.Append("       MAX(C.CAN_APPLY) AS CAN_APPLY, ");
            sb.Append("       CONVERT(DATE, CD.CREATED_AT) AS DATA, ");
            sb.Append("       MAX(PU.NAME) AS NAME, ");
            sb.Append("       MAX(PU.BC) AS BC, ");
            sb.Append("       CD.IDGDA_COLLABORATORS, ");
            sb.Append("       PU.IDGDA_PERSONA_USER, ");
            sb.Append("       CASE ");
            sb.Append("           WHEN MAX(C.CLIMATE) IS NULL THEN 'SEM RESPOSTA' ");
            sb.Append("           ELSE MAX(C.CLIMATE) ");
            sb.Append("       END AS CLIMATE, ");
            sb.Append("       CASE ");
            sb.Append("           WHEN MAX(CR.REASON) IS NULL THEN '' ");
            sb.Append("           ELSE MAX(CR.REASON) ");
            sb.Append("       END AS REASON, ");
            sb.Append("       MAX(AT.TYPE) AS TYPE, ");
            sb.Append("       MAX(CD.NOME_SUPERVISOR) AS NOME_SUPERVISOR, ");
            sb.Append("       MAX(CD.NOME_COORDENADOR) AS NOME_COORDENADOR, ");
            sb.Append("       MAX(CD.NOME_GERENTE_II) AS NOME_GERENTE_II, ");
            sb.Append("       MAX(CD.NOME_GERENTE_I) AS NOME_GERENTE_I, ");
            sb.Append("       MAX(CD.NOME_DIRETOR) AS NOME_DIRETOR, ");
            sb.Append("       MAX(CD.NOME_CEO) AS NOME_CEO ");
            sb.Append("FROM ");
            sb.Append("  (SELECT CDI2.IDGDA_COLLABORATORS, ");
            sb.Append("          CONVERT(DATE, CDI2.CREATED_AT) AS CREATED_AT, ");
            sb.Append("          MAX(CDI2.MATRICULA_SUPERVISOR) AS MATRICULA_SUPERVISOR, ");
            sb.Append("          MAX(CDI2.NOME_SUPERVISOR) AS NOME_SUPERVISOR, ");
            sb.Append("          MAX(CDI2.MATRICULA_COORDENADOR) AS MATRICULA_COORDENADOR, ");
            sb.Append("          MAX(CDI2.NOME_COORDENADOR) AS NOME_COORDENADOR, ");
            sb.Append("          MAX(CDI2.MATRICULA_GERENTE_II) AS MATRICULA_GERENTE_II, ");
            sb.Append("          MAX(CDI2.NOME_GERENTE_II) AS NOME_GERENTE_II, ");
            sb.Append("          MAX(CDI2.MATRICULA_GERENTE_I) AS MATRICULA_GERENTE_I, ");
            sb.Append("          MAX(CDI2.NOME_GERENTE_I) AS NOME_GERENTE_I, ");
            sb.Append("          MAX(CDI2.MATRICULA_DIRETOR) AS MATRICULA_DIRETOR, ");
            sb.Append("          MAX(CDI2.NOME_DIRETOR) AS NOME_DIRETOR, ");
            sb.Append("          MAX(CDI2.MATRICULA_CEO) AS MATRICULA_CEO, ");
            sb.Append("          MAX(CDI2.NOME_CEO) AS NOME_CEO ");
            sb.Append("   FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CDI2 ");
            sb.Append("   WHERE CDI2.CREATED_AT >= @DATE_INI AND CDI2.CREATED_AT <= @DATE_FIM ");
            sb.Append("   AND (CDI2.IDGDA_COLLABORATORS = @ID ");
            sb.Append("       OR CDI2.MATRICULA_SUPERVISOR = @ID ");
            sb.Append("       OR CDI2.MATRICULA_COORDENADOR = @ID ");
            sb.Append("       OR CDI2.MATRICULA_GERENTE_II = @ID ");
            sb.Append("       OR CDI2.MATRICULA_GERENTE_I = @ID ");
            sb.Append("       OR CDI2.MATRICULA_DIRETOR = @ID ");
            sb.Append("       OR CDI2.MATRICULA_CEO = @ID) ");
            sb.Append($"    {filtroSetor} ");
            //sb.Append("     AND CDI2.CARGO = 'AGENTE' ");
            sb.Append("   GROUP BY CDI2.IDGDA_COLLABORATORS, ");
            sb.Append("            CONVERT(DATE, CDI2.CREATED_AT) ");
            
            sb.Append("  ) AS CD ");
            sb.Append("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCU ON PCU.IDGDA_COLLABORATORS = CD.IDGDA_COLLABORATORS ");
            sb.Append("LEFT JOIN GDA_PERSONA_USER (NOLOCK) AS PU ON PU.IDGDA_PERSONA_USER = PCU.IDGDA_PERSONA_USER ");
            sb.Append("AND PU.IDGDA_PERSONA_USER_TYPE = 1 ");
            sb.Append("LEFT JOIN GDA_CLIMATE_USER (NOLOCK) AS GCU ON GCU.IDGDA_PERSONA = PU.IDGDA_PERSONA_USER ");
            sb.Append("AND CONVERT(DATE, GCU.CREATED_AT) = CONVERT(DATE, CD.CREATED_AT) ");
            sb.Append("LEFT JOIN GDA_CLIMATE (NOLOCK) AS C ON C.IDGDA_CLIMATE = GCU.IDGDA_CLIMATE ");
            sb.Append("LEFT JOIN GDA_CLIMATE_REASON (NOLOCK) AS CR ON CR.IDGDA_CLIMATE_REASON = GCU.IDGDA_CLIMATE_REASON ");
            sb.Append("LEFT JOIN GDA_CLIMATE_APPLY_TYPE (NOLOCK) AS AT ON AT.IDGDA_CLIMATE_APPLY_TYPE = GCU.IDGDA_CLIMATE_APPLY_TYPE ");
            sb.Append("WHERE BC IS NOT NULL ");
            sb.Append("  AND CONVERT(DATE, CD.CREATED_AT) >= @DATE_INI AND CONVERT(DATE, CD.CREATED_AT) <= @DATE_FIM ");
            sb.Append($" {filtro} ");
            sb.Append("GROUP BY GCU.IDGDA_CLIMATE_USER, ");
            sb.Append("         CONVERT(DATE, CD.CREATED_AT), ");
            sb.Append("         CD.IDGDA_COLLABORATORS, ");
            sb.Append("         PU.IDGDA_PERSONA_USER ");
            sb.Append("ORDER BY CONVERT(DATE, CD.CREATED_AT), ");
            sb.Append("         CD.IDGDA_COLLABORATORS ");
            sb.Append($"{filterRowSet} ");


            //sb.Append($"SELECT  ");
            //sb.Append($"GCU.IDGDA_CLIMATE_USER, MAX(C.CAN_APPLY) AS CAN_APPLY, CONVERT(DATE, CD.CREATED_AT) AS DATA, ");
            //sb.Append($"MAX(PU.NAME) AS NAME, ");
            //sb.Append($"MAX(PU.BC) AS BC,  ");
            //sb.Append($"CD.IDGDA_COLLABORATORS, ");
            //sb.Append($"PU.IDGDA_PERSONA_USER,  ");
            //sb.Append($"CASE WHEN MAX(C.CLIMATE) IS NULL THEN 'SEM RESPOSTA' ELSE MAX(C.CLIMATE) END AS CLIMATE, ");
            //sb.Append($"CASE WHEN MAX(CR.REASON) IS NULL THEN '' ELSE MAX(CR.REASON) END AS REASON, ");
            //sb.Append($"MAX(AT.TYPE) AS TYPE, ");
            //sb.Append($"MAX(CD.NOME_SUPERVISOR) AS NOME_SUPERVISOR, ");
            //sb.Append($"MAX(CD.NOME_COORDENADOR) AS NOME_COORDENADOR, ");
            //sb.Append($"MAX(CD.NOME_GERENTE_II) AS NOME_GERENTE_II, ");
            //sb.Append($"MAX(CD.NOME_GERENTE_I) AS NOME_GERENTE_I, ");
            //sb.Append($"MAX(CD.NOME_DIRETOR) AS NOME_DIRETOR, ");
            //sb.Append($"MAX(CD.NOME_CEO) AS NOME_CEO ");
            //sb.Append($"FROM  ");
            //sb.Append($"(  ");
            //sb.Append($"SELECT  ");
            //sb.Append($"CDI2.IDGDA_COLLABORATORS, CONVERT(DATE, CDI2.CREATED_AT) AS CREATED_AT,  ");
            //sb.Append($"MAX(CDI2.MATRICULA_SUPERVISOR) AS MATRICULA_SUPERVISOR, MAX(CDI2.NOME_SUPERVISOR) AS NOME_SUPERVISOR,  ");
            //sb.Append($"MAX(CDI2.MATRICULA_COORDENADOR) AS MATRICULA_COORDENADOR,  MAX(CDI2.NOME_COORDENADOR) AS NOME_COORDENADOR,  ");
            //sb.Append($"MAX(CDI2.MATRICULA_GERENTE_II) AS MATRICULA_GERENTE_II,  MAX(CDI2.NOME_GERENTE_II) AS NOME_GERENTE_II, ");
            //sb.Append($"MAX(CDI2.MATRICULA_GERENTE_I) AS MATRICULA_GERENTE_I, MAX(CDI2.NOME_GERENTE_I) AS NOME_GERENTE_I,  ");
            //sb.Append($"MAX(CDI2.MATRICULA_DIRETOR) AS MATRICULA_DIRETOR, MAX(CDI2.NOME_DIRETOR) AS NOME_DIRETOR,  ");
            //sb.Append($"MAX(CDI2.MATRICULA_CEO) AS MATRICULA_CEO, MAX(CDI2.NOME_CEO) AS NOME_CEO ");
            //sb.Append($"FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CDI2 ");
            //sb.Append($"WHERE CDI2.CREATED_AT >= @DATE_INI AND CDI2.CREATED_AT <= @DATE_FIM   ");
            //sb.Append($"{filtroSetor} ");
            //sb.Append($"AND CDI2.CARGO = 'AGENTE' ");
            //sb.Append($"GROUP BY CDI2.IDGDA_COLLABORATORS, CONVERT(DATE, CDI2.CREATED_AT) ");
            //sb.Append($"UNION ALL ");
            //sb.Append($"SELECT  ");
            //sb.Append($"CDI.IDGDA_COLLABORATORS, CONVERT(DATE, CDI.CREATED_AT) AS CREATED_AT,  ");
            //sb.Append($"MAX(CDI.MATRICULA_SUPERVISOR) AS MATRICULA_SUPERVISOR, MAX(CDI.NOME_SUPERVISOR) AS NOME_SUPERVISOR,  ");
            //sb.Append($"MAX(CDI.MATRICULA_COORDENADOR) AS MATRICULA_COORDENADOR,  MAX(CDI.NOME_COORDENADOR) AS NOME_COORDENADOR,  ");
            //sb.Append($"MAX(CDI.MATRICULA_GERENTE_II) AS MATRICULA_GERENTE_II,  MAX(CDI.NOME_GERENTE_II) AS NOME_GERENTE_II, ");
            //sb.Append($"MAX(CDI.MATRICULA_GERENTE_I) AS MATRICULA_GERENTE_I, MAX(CDI.NOME_GERENTE_I) AS NOME_GERENTE_I,  ");
            //sb.Append($"MAX(CDI.MATRICULA_DIRETOR) AS MATRICULA_DIRETOR, MAX(CDI.NOME_DIRETOR) AS NOME_DIRETOR,  ");
            //sb.Append($"MAX(CDI.MATRICULA_CEO) AS MATRICULA_CEO, MAX(CDI.NOME_CEO) AS NOME_CEO ");
            //sb.Append($"FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CDI ");
            //sb.Append($"INNER JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CDI2 ON  CONVERT(DATE, CDI.CREATED_AT) = CONVERT(DATE, CDI2.CREATED_AT) AND  ");
            //sb.Append($"(CDI2.MATRICULA_SUPERVISOR = CDI.IDGDA_COLLABORATORS OR  ");
            //sb.Append($"		CDI2.MATRICULA_COORDENADOR = CDI.IDGDA_COLLABORATORS OR  ");
            //sb.Append($"		CDI2.MATRICULA_GERENTE_II = CDI.IDGDA_COLLABORATORS OR  ");
            //sb.Append($"		CDI2.MATRICULA_GERENTE_I = CDI.IDGDA_COLLABORATORS OR  ");
            //sb.Append($"		CDI2.MATRICULA_DIRETOR = CDI.IDGDA_COLLABORATORS OR  ");
            //sb.Append($"		CDI2.MATRICULA_CEO = CDI.IDGDA_COLLABORATORS) AND CDI2.CREATED_AT >= @DATE_INI AND CDI2.CREATED_AT <= @DATE_FIM ");
            //sb.Append($"WHERE CDI.CREATED_AT >= @DATE_INI AND CDI.CREATED_AT <= @DATE_FIM  ");
            //sb.Append($"AND CDI.CARGO != 'AGENTE' AND CDI.CARGO IS NOT NULL {filtroSetor} ");
            //sb.Append($"GROUP BY CDI.IDGDA_COLLABORATORS, CONVERT(DATE, CDI.CREATED_AT) ");
            //sb.Append($"UNION ALL ");
            //sb.Append($"SELECT  ");
            //sb.Append($"CDI2.IDGDA_COLLABORATORS, CONVERT(DATE, GETDATE()) AS CREATED_AT,  ");
            //sb.Append($"MAX(CDI2.MATRICULA_SUPERVISOR) AS MATRICULA_SUPERVISOR, MAX(CDI2.NOME_SUPERVISOR) AS NOME_SUPERVISOR,  ");
            //sb.Append($"MAX(CDI2.MATRICULA_COORDENADOR) AS MATRICULA_COORDENADOR,  MAX(CDI2.NOME_COORDENADOR) AS NOME_COORDENADOR,  ");
            //sb.Append($"MAX(CDI2.MATRICULA_GERENTE_II) AS MATRICULA_GERENTE_II,  MAX(CDI2.NOME_GERENTE_II) AS NOME_GERENTE_II, ");
            //sb.Append($"MAX(CDI2.MATRICULA_GERENTE_I) AS MATRICULA_GERENTE_I, MAX(CDI2.NOME_GERENTE_I) AS NOME_GERENTE_I,  ");
            //sb.Append($"MAX(CDI2.MATRICULA_DIRETOR) AS MATRICULA_DIRETOR, MAX(CDI2.NOME_DIRETOR) AS NOME_DIRETOR,  ");
            //sb.Append($"MAX(CDI2.MATRICULA_CEO) AS MATRICULA_CEO, MAX(CDI2.NOME_CEO) AS NOME_CEO ");
            //sb.Append($"FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CDI2 ");
            //sb.Append($"WHERE CDI2.CREATED_AT >= CONVERT(DATE, DATEADD(DAY, -1, GETDATE()))  ");
            //sb.Append($"{filtroSetor} ");
            //sb.Append($"AND CDI2.CARGO = 'AGENTE' ");
            //sb.Append($"GROUP BY CDI2.IDGDA_COLLABORATORS ");
            //sb.Append($"UNION ALL ");
            //sb.Append($"SELECT  ");
            //sb.Append($"CDI.IDGDA_COLLABORATORS, CONVERT(DATE, GETDATE()) AS CREATED_AT,  ");
            //sb.Append($"MAX(CDI.MATRICULA_SUPERVISOR) AS MATRICULA_SUPERVISOR, MAX(CDI.NOME_SUPERVISOR) AS NOME_SUPERVISOR,  ");
            //sb.Append($"MAX(CDI.MATRICULA_COORDENADOR) AS MATRICULA_COORDENADOR,  MAX(CDI.NOME_COORDENADOR) AS NOME_COORDENADOR,  ");
            //sb.Append($"MAX(CDI.MATRICULA_GERENTE_II) AS MATRICULA_GERENTE_II,  MAX(CDI.NOME_GERENTE_II) AS NOME_GERENTE_II, ");
            //sb.Append($"MAX(CDI.MATRICULA_GERENTE_I) AS MATRICULA_GERENTE_I, MAX(CDI.NOME_GERENTE_I) AS NOME_GERENTE_I,  ");
            //sb.Append($"MAX(CDI.MATRICULA_DIRETOR) AS MATRICULA_DIRETOR, MAX(CDI.NOME_DIRETOR) AS NOME_DIRETOR,  ");
            //sb.Append($"MAX(CDI.MATRICULA_CEO) AS MATRICULA_CEO, MAX(CDI.NOME_CEO) AS NOME_CEO ");
            //sb.Append($"FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CDI ");
            //sb.Append($"INNER JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CDI2 ON  CONVERT(DATE, CDI.CREATED_AT) = CONVERT(DATE, CDI2.CREATED_AT) AND  ");
            //sb.Append($"(CDI2.MATRICULA_SUPERVISOR = CDI.IDGDA_COLLABORATORS OR  ");
            //sb.Append($"		CDI2.MATRICULA_COORDENADOR = CDI.IDGDA_COLLABORATORS OR  ");
            //sb.Append($"		CDI2.MATRICULA_GERENTE_II = CDI.IDGDA_COLLABORATORS OR  ");
            //sb.Append($"		CDI2.MATRICULA_GERENTE_I = CDI.IDGDA_COLLABORATORS OR  ");
            //sb.Append($"		CDI2.MATRICULA_DIRETOR = CDI.IDGDA_COLLABORATORS OR  ");
            //sb.Append($"		CDI2.MATRICULA_CEO = CDI.IDGDA_COLLABORATORS) AND CDI2.CREATED_AT >= CONVERT(DATE, DATEADD(DAY, -1, GETDATE())) ");
            //sb.Append($"WHERE CDI.CREATED_AT >= CONVERT(DATE, DATEADD(DAY, -1, GETDATE()))  ");
            //sb.Append($"AND CDI.CARGO != 'AGENTE' AND CDI.CARGO IS NOT NULL {filtroSetor} ");
            //sb.Append($"GROUP BY CDI.IDGDA_COLLABORATORS ");
            //sb.Append($") AS CD ");
            //sb.Append($"LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCU ON PCU.IDGDA_COLLABORATORS = CD.IDGDA_COLLABORATORS ");
            //sb.Append($"LEFT JOIN GDA_PERSONA_USER (NOLOCK) AS PU ON PU.IDGDA_PERSONA_USER = PCU.IDGDA_PERSONA_USER AND PU.IDGDA_PERSONA_USER_TYPE = 1  ");
            //sb.Append($"LEFT JOIN GDA_CLIMATE_USER (NOLOCK) AS GCU ON GCU.IDGDA_PERSONA = PU.IDGDA_PERSONA_USER AND CONVERT(DATE, GCU.CREATED_AT) = CONVERT(DATE, CD.CREATED_AT) ");
            //sb.Append($"LEFT JOIN GDA_CLIMATE (NOLOCK) AS C ON C.IDGDA_CLIMATE = GCU.IDGDA_CLIMATE  ");
            //sb.Append($"LEFT JOIN GDA_CLIMATE_REASON (NOLOCK) AS CR ON CR.IDGDA_CLIMATE_REASON = GCU.IDGDA_CLIMATE_REASON ");
            //sb.Append($"LEFT JOIN GDA_CLIMATE_APPLY_TYPE (NOLOCK) AS AT ON AT.IDGDA_CLIMATE_APPLY_TYPE = GCU.IDGDA_CLIMATE_APPLY_TYPE ");
            //sb.Append($" ");
            //sb.Append($"WHERE (CD.IDGDA_COLLABORATORS = @ID OR  ");
            //sb.Append($"		CD.MATRICULA_SUPERVISOR = @ID OR  ");
            //sb.Append($"		CD.MATRICULA_COORDENADOR = @ID OR  ");
            //sb.Append($"		CD.MATRICULA_GERENTE_II = @ID OR  ");
            //sb.Append($"		CD.MATRICULA_GERENTE_I = @ID OR  ");
            //sb.Append($"		CD.MATRICULA_DIRETOR = @ID OR  ");
            //sb.Append($"		CD.MATRICULA_CEO = @ID) ");
            //sb.Append($"AND BC IS NOT NULL ");
            //sb.Append($"AND CONVERT(DATE, CD.CREATED_AT) >= @DATE_INI AND CONVERT(DATE, CD.CREATED_AT) <= @DATE_FIM {filtro} ");
            //sb.Append($"GROUP BY GCU.IDGDA_CLIMATE_USER, CONVERT(DATE, CD.CREATED_AT), CD.IDGDA_COLLABORATORS, PU.IDGDA_PERSONA_USER ");
            //sb.Append($"ORDER BY CONVERT(DATE, CD.CREATED_AT), CD.IDGDA_COLLABORATORS ");
            //sb.Append($"{filterRowSet} ");


            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        command.CommandTimeout = 0;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                returnListsReportHierarchyClimate rtn = new returnListsReportHierarchyClimate();

                                rtn.idUserClimate = reader["IDGDA_CLIMATE_USER"] != DBNull.Value ? int.Parse(reader["IDGDA_CLIMATE_USER"].ToString()) : 0;
                                rtn.data = reader["DATA"] != DBNull.Value ? reader["DATA"].ToString() : "";
                                rtn.name = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "";
                                rtn.BC = reader["BC"] != DBNull.Value ? reader["BC"].ToString() : "";
                                rtn.climate = reader["CLIMATE"] != DBNull.Value ? reader["CLIMATE"].ToString() : "";
                                rtn.reason = reader["REASON"] != DBNull.Value ? reader["REASON"].ToString() : "";
                                rtn.applyType = reader["TYPE"] != DBNull.Value ? reader["TYPE"].ToString() : "";
                                rtn.canApply = reader["CAN_APPLY"] != DBNull.Value ? Convert.ToInt32(reader["CAN_APPLY"]) == 1 ? true : false : false;
                                rtn.nomeSupervisor = reader["NOME_SUPERVISOR"] != DBNull.Value ? reader["NOME_SUPERVISOR"].ToString() : "";
                                rtn.nomeCoordenador = reader["NOME_COORDENADOR"] != DBNull.Value ? reader["NOME_COORDENADOR"].ToString() : "";
                                rtn.nomeGerenteII = reader["NOME_GERENTE_II"] != DBNull.Value ? reader["NOME_GERENTE_II"].ToString() : "";
                                rtn.nomeGerenteI = reader["NOME_GERENTE_I"] != DBNull.Value ? reader["NOME_GERENTE_I"].ToString() : "";
                                rtn.nomeDiretor = reader["NOME_DIRETOR"] != DBNull.Value ? reader["NOME_DIRETOR"].ToString() : "";
                                rtn.nomeCeo = reader["NOME_CEO"] != DBNull.Value ? reader["NOME_CEO"].ToString() : "";

                                resposts.Add(rtn);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Trate a exceção aqui
                }
                connection.Close();
            }


            return resposts;
        }

    }


}