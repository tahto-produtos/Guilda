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
using Iced.Intel;
using System.Threading;
using static ApiRepositorio.Controllers.ReportAcessFeeController;
using static ApiRepositorio.Controllers.ReportNotificationController;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ReportAcessFeeController : ApiController
    {
        public class listReportAcess
        {
            public string DATA_ESCALA { get; set; }
            public string DATA_ACESSO { get; set; }
            public string DATA_ATUALIZACAO { get; set; }
            public string BC { get; set; }
            public string NOME { get; set; }
            public string CARGO { get; set; }
            public string ESCALA { get; set; }
            public string ACESSO { get; set; }
            public string COD_INDICADOR { get; set; }
            public string INDICADOR { get; set; }
            public string TEMPO_LOGADO { get; set; }
            public string MATRICULA_SUPERVISOR { get; set; }
            public string NOME_SUPERVISOR { get; set; }
            public string MATRICULA_COORDENADOR { get; set; }
            public string NOME_COORDENADOR { get; set; }
            public string MATRICULA_GERENTE2 { get; set; }
            public string NOME_GERENTE2 { get; set; }
            public string MATRICULA_GERENTE1 { get; set; }
            public string NOME_GERENTE1 { get; set; }
            public string MATRICULA_DIRETOR { get; set; }
            public string NOME_DIRETOR { get; set; }
            public string MATRICULA_CEO { get; set; }
            public string NOME_CEO { get; set; }
            public string CODGIP { get; set; }
            public string SETOR { get; set; }
            public string CODGIP_SUBSETOR { get; set; }
            public string SUBSETOR { get; set; }
            public string TURNO { get; set; }
            public string SITE { get; set; }
            public string HOME_BASED { get; set; }
            public string CLIENTE { get; set; }
        }
        public class returnResponseAcess
        {
            public string DATA_ESCALA { get; set; }
            public string DATA_ACESSO { get; set; }
            public string DATA_ATUALIZACAO { get; set; }
            public string BC { get; set; }
            public string NOME { get; set; }
            public string CARGO { get; set; }
            public string ESCALA { get; set; }
            public string ACESSO { get; set; }
            public string COD_INDICADOR { get; set; }
            public string INDICADOR { get; set; }
            public string TEMPO_LOGADO { get; set; }
            public string MATRICULA_SUPERVISOR { get; set; }
            public string NOME_SUPERVISOR { get; set; }
            public string MATRICULA_COORDENADOR { get; set; }
            public string NOME_COORDENADOR { get; set; }
            public string MATRICULA_GERENTE2 { get; set; }
            public string NOME_GERENTE2 { get; set; }
            public string MATRICULA_GERENTE1 { get; set; }
            public string NOME_GERENTE1 { get; set; }
            public string MATRICULA_DIRETOR { get; set; }
            public string NOME_DIRETOR { get; set; }
            public string MATRICULA_CEO { get; set; }
            public string NOME_CEO { get; set; }
            public string CODGIP { get; set; }
            public string SETOR { get; set; }
            public string CODGIP_SUBSETOR { get; set; }
            public string SUBSETOR { get; set; }
            public string TURNO { get; set; }
            public string SITE { get; set; }
            public string HOME_BASED { get; set; }
            public string CLIENTE { get; set; }

        }

        #region Input
        public class InputModel
        {
            public List<Sector> Sectors { get; set; }
            public List<Hierarchy> Hierarchies { get; set; }
            public string Order { get; set; }
            public DateTime DataInicial { get; set; }
            public DateTime DataFinal { get; set; }
            public int CollaboratorId { get; set; }
        }

        public class Sector
        {
            public int Id { get; set; }
        }
        public class Hierarchy
        {
            public int Id { get; set; }
        }
        #endregion

        public List<listReportAcess> returnAcessFee(string dtInicial, string dtFinal, string sectors, string hierarchies, string ordem, string collaborators)
        {
            //PREPARAR OS FILTROS
            string filter = "";
            string orderBy = "";

            //FILTRO POR SETOR.
            if (sectors != "")
            {
                filter = filter + $" AND CL.IDGDA_SECTOR IN ({sectors}) ";
            }
            //FILTRO POR COLABORADOR.
            if (collaborators != "")
            {
                filter = filter + collaborators;
            }      
            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtInicial);
            stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFinal);
            stb.AppendFormat("SELECT  ");
            stb.AppendFormat("	   MAX(ESC.CREATED_AT) AS DATA_ESCALA, ");
            stb.AppendFormat("	   MAX(LOG.CREATED_AT) AS DATA_ACESSO, ");
            stb.AppendFormat("	   GETDATE()  AS DATA_ATUALIZACAO, ");
            stb.AppendFormat("       CL.IDGDA_COLLABORATORS AS 'BC',  ");
            stb.AppendFormat("       MAX(CB.NAME) AS NOME,  ");
            stb.AppendFormat("       MAX(CL.CARGO) AS CARGO, ");
            stb.AppendFormat("	   MAX(ESC.ESCALADO) AS ESCALA, ");
            stb.AppendFormat("	   MAX(LOG.LOGIN) AS ACESSO,  ");
            stb.AppendFormat("     MAX(IT.IDGDA_INDICATOR) AS 'COD INDICADOR',  ");
            stb.AppendFormat("     MAX(IT.NAME) AS 'INDICADOR', ");
            stb.AppendFormat("	   MAX(LOG.TEMPO_LOGADO) AS TEMPO_LOGADO, ");
            stb.AppendFormat("	   MAX(CL.MATRICULA_SUPERVISOR) AS 'MATRICULA SUPERVISOR',  ");
            stb.AppendFormat("     MAX(CL.NOME_SUPERVISOR) AS 'NOME SUPERVISOR',  ");
            stb.AppendFormat("     MAX(CL.MATRICULA_COORDENADOR) AS 'MATRICULA COORDENADOR',  ");
            stb.AppendFormat("     MAX(CL.NOME_COORDENADOR) AS 'NOME COORDENADOR',  ");
            stb.AppendFormat("     MAX(CL.MATRICULA_GERENTE_II) AS 'MATRICULA GERENTE II',  ");
            stb.AppendFormat("     MAX(CL.NOME_GERENTE_II) AS 'NOME GERENTE II',  ");
            stb.AppendFormat("     MAX(CL.MATRICULA_GERENTE_I) AS 'MATRICULA GERENTE I',  ");
            stb.AppendFormat("     MAX(CL.NOME_GERENTE_I) AS 'NOME GERENTE I',  ");
            stb.AppendFormat("     MAX(CL.MATRICULA_DIRETOR) AS 'MATRICULA DIRETOR',  ");
            stb.AppendFormat("     MAX(CL.NOME_DIRETOR) AS 'NOME DIRETOR',  ");
            stb.AppendFormat("     MAX(CL.MATRICULA_CEO) AS 'MATRICULA CEO',  ");
            stb.AppendFormat("     MAX(CL.NOME_CEO) AS 'NOME CEO' , ");
            stb.AppendFormat("     MAX(CL.IDGDA_SECTOR) AS CODGIP,  ");
            stb.AppendFormat("     MAX(SEC.NAME) AS SETOR,  ");
            stb.AppendFormat("     MAX(CL.IDGDA_SUBSECTOR) AS CODGIP_SUBSETOR,  ");
            stb.AppendFormat("     MAX(SUBSECTOR.NAME) AS SUBSETOR,  ");
            stb.AppendFormat("	   MAX(CL.PERIODO) AS TURNO, ");
            stb.AppendFormat("	   MAX(CL.SITE) AS SITE,  ");
            stb.AppendFormat("       MAX(CL.HOME_BASED) AS HOME_BASED,  ");
            stb.AppendFormat("	   MAX(CLI.CLIENT)  AS CLIENTE     ");
            stb.AppendFormat("FROM   ");
            stb.AppendFormat("  (SELECT IDGDA_SECTOR,  ");
            stb.AppendFormat("          IDGDA_SUBSECTOR AS IDGDA_SUBSECTOR,  ");
            stb.AppendFormat("          CREATED_AT,  ");
            stb.AppendFormat("          IDGDA_COLLABORATORS,  ");
            stb.AppendFormat("          ACTIVE,  ");
            stb.AppendFormat("          CARGO,  ");
            stb.AppendFormat("          HOME_BASED,  ");
            stb.AppendFormat("          SITE,  ");
            stb.AppendFormat("          PERIODO,  ");
            stb.AppendFormat("          MATRICULA_SUPERVISOR,  ");
            stb.AppendFormat("          NOME_SUPERVISOR,  ");
            stb.AppendFormat("          MATRICULA_COORDENADOR,  ");
            stb.AppendFormat("          NOME_COORDENADOR,  ");
            stb.AppendFormat("          MATRICULA_GERENTE_II,  ");
            stb.AppendFormat("          NOME_GERENTE_II,  ");
            stb.AppendFormat("          MATRICULA_GERENTE_I,  ");
            stb.AppendFormat("          NOME_GERENTE_I,  ");
            stb.AppendFormat("          MATRICULA_DIRETOR,  ");
            stb.AppendFormat("          NOME_DIRETOR,  ");
            stb.AppendFormat("          MATRICULA_CEO,  ");
            stb.AppendFormat("          NOME_CEO, ");
            stb.AppendFormat("		  IDGDA_CLIENT ");
            stb.AppendFormat("   FROM GDA_COLLABORATORS_DETAILS (NOLOCK)  ");
            stb.AppendFormat("   WHERE CREATED_AT >= @DATAINICIAL  ");
            stb.AppendFormat("     AND CREATED_AT <= @DATAFINAL ) AS CL   ");
            stb.AppendFormat("INNER JOIN GDA_CLIENT (NOLOCK) AS CLI ON CLI.IDGDA_CLIENT = CL.IDGDA_CLIENT ");
            stb.AppendFormat("INNER JOIN GDA_INDICATOR (NOLOCK) AS IT ON IT.IDGDA_INDICATOR IN ('10000013')  ");
            stb.AppendFormat("LEFT JOIN  ");
            stb.AppendFormat("  (SELECT COUNT(0) AS 'ESCALADO',  ");
            stb.AppendFormat("          IDGDA_COLLABORATORS,  ");
            stb.AppendFormat("          CREATED_AT  ");
            stb.AppendFormat("   FROM GDA_RESULT (NOLOCK)  ");
            stb.AppendFormat("   WHERE INDICADORID = -1  ");
            stb.AppendFormat("     AND CREATED_AT >= @DATAINICIAL  ");
            stb.AppendFormat("     AND CREATED_AT <= @DATAFINAL  ");
            stb.AppendFormat("     AND RESULT = 1  ");
            stb.AppendFormat("     AND DELETED_AT IS NULL  ");
            stb.AppendFormat("   GROUP BY IDGDA_COLLABORATORS,  ");
            stb.AppendFormat("            CREATED_AT) AS ESC ON ESC.IDGDA_COLLABORATORS = CL.IDGDA_COLLABORATORS  ");
            stb.AppendFormat("AND ESC.CREATED_AT = CL.CREATED_AT  ");
            stb.AppendFormat("LEFT JOIN  ");
            stb.AppendFormat("  (SELECT COUNT(DISTINCT IDGDA_COLLABORATOR) AS 'LOGIN',  ");
            stb.AppendFormat("          IDGDA_COLLABORATOR, ");
            stb.AppendFormat("		  ISNULL(TEMPOLOGIN,0) AS TEMPO_LOGADO, ");
            stb.AppendFormat("          CONVERT(DATE, DATE_ACCESS) AS CREATED_AT  ");
            stb.AppendFormat("   FROM GDA_LOGIN_ACCESS (NOLOCK)  ");
            stb.AppendFormat("   WHERE CONVERT(DATE, DATE_ACCESS) >= @DATAINICIAL  ");
            stb.AppendFormat("     AND CONVERT(DATE, DATE_ACCESS) <= @DATAFINAL  ");
            stb.AppendFormat("   GROUP BY IDGDA_COLLABORATOR, ");
            stb.AppendFormat("			TEMPOLOGIN, ");
            stb.AppendFormat("            CONVERT(DATE, DATE_ACCESS)) AS LOG ON LOG.IDGDA_COLLABORATOR = CL.IDGDA_COLLABORATORS  ");
            stb.AppendFormat("AND LOG.CREATED_AT = CL.CREATED_AT  ");
            stb.AppendFormat("LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS CB ON CB.IDGDA_COLLABORATORS = CL.IDGDA_COLLABORATORS  ");
            stb.AppendFormat("LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = CL.IDGDA_SECTOR  ");
            stb.AppendFormat("LEFT JOIN GDA_SECTOR (NOLOCK) AS SUBSECTOR ON SUBSECTOR.IDGDA_SECTOR = CL.IDGDA_SUBSECTOR  ");
            stb.AppendFormat("WHERE 1 = 1  ");
            stb.AppendFormat("  AND CL.CREATED_AT >= @DATAINICIAL  ");
            stb.AppendFormat("  AND CL.CREATED_AT <= @DATAFINAL  ");
            //stb.AppendFormat("  AND CL.active = 'true'  ");
            stb.AppendFormat($"  {filter}  ");
            stb.AppendFormat("GROUP BY CL.IDGDA_COLLABORATORS,  ");
            stb.AppendFormat("         CONVERT(DATE, CL.CREATED_AT)  ");

            List<listReportAcess> rmams = new List<listReportAcess>();
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                {
                    command.CommandTimeout = 60;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listReportAcess rmam = new listReportAcess();
                            rmam.DATA_ESCALA = reader["DATA_ESCALA"].ToString();
                            rmam.DATA_ACESSO = reader["DATA_ACESSO"].ToString();
                            rmam.DATA_ATUALIZACAO= reader["DATA_ATUALIZACAO"].ToString();
                            rmam.BC = reader["BC"].ToString();
                            rmam.NOME = reader["NOME"].ToString();
                            rmam.CARGO = reader["CARGO"].ToString();
                            rmam.ESCALA = reader["ESCALA"].ToString();
                            rmam.ACESSO = reader["ACESSO"].ToString();
                            rmam.COD_INDICADOR = reader["COD INDICADOR"].ToString();
                            rmam.INDICADOR = reader["INDICADOR"].ToString();
                            rmam.TEMPO_LOGADO = reader["TEMPO_LOGADO"].ToString();
                            rmam.MATRICULA_SUPERVISOR = reader["MATRICULA SUPERVISOR"].ToString();
                            rmam.NOME_SUPERVISOR = reader["NOME SUPERVISOR"].ToString();
                            rmam.MATRICULA_COORDENADOR = reader["MATRICULA COORDENADOR"].ToString();
                            rmam.NOME_COORDENADOR = reader["NOME COORDENADOR"].ToString();
                            rmam.MATRICULA_GERENTE2 = reader["MATRICULA GERENTE II"].ToString();
                            rmam.NOME_GERENTE2 = reader["NOME GERENTE II"].ToString();
                            rmam.MATRICULA_GERENTE1 = reader["MATRICULA GERENTE I"].ToString();
                            rmam.NOME_GERENTE1 = reader["NOME GERENTE I"].ToString();
                            rmam.MATRICULA_DIRETOR = reader["MATRICULA DIRETOR"].ToString();
                            rmam.NOME_DIRETOR = reader["NOME DIRETOR"].ToString();
                            rmam.MATRICULA_CEO = reader["MATRICULA CEO"].ToString();
                            rmam.NOME_CEO = reader["NOME CEO"].ToString();
                            rmam.CODGIP = reader["CODGIP"].ToString();
                            rmam.SETOR = reader["SETOR"].ToString();
                            rmam.CODGIP_SUBSETOR = reader["CODGIP_SUBSETOR"].ToString();
                            rmam.SUBSETOR = reader["SUBSETOR"].ToString();
                            rmam.TURNO = reader["TURNO"].ToString();
                            rmam.SITE = reader["SITE"].ToString();
                            rmam.HOME_BASED = reader["HOME_BASED"].ToString();
                            rmam.CLIENTE = reader["CLIENTE"].ToString();
                            rmams.Add(rmam);
                        }
                    }
                }
                connection.Close();
            }
            return rmams;
        }
        // POST: api/Results
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {
            string dtInicial = inputModel.DataInicial.ToString("yyyy-MM-dd");
            string dtFinal = inputModel.DataFinal.ToString("yyyy-MM-dd");
            string sectorsAsString = string.Join(",", inputModel.Sectors.Select(g => g.Id));
            string hierarchiesAsString = string.Join(",", inputModel.Hierarchies.Select(g => g.Id));
            string CollaboratorId = inputModel.CollaboratorId.ToString();
            DateTime dtTimeInicial = DateTime.ParseExact(dtInicial, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dtTimeFinal = DateTime.ParseExact(dtFinal, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            TimeSpan diff = dtTimeFinal.Subtract(dtTimeInicial);
            int diferencaEmDias = (int)diff.TotalDays;
            if (diferencaEmDias > 31)
            {
                return BadRequest("Selecionar uma data de no maximo 1 mês!");
            }
            //VERIFICA PERFIL ADMINISTRATIVO
            bool adm = Funcoes.retornaPermissao(CollaboratorId);
            string filtro = "";
            int cargoAtual = 0;
            //RETORNA OS IDS ABAIXO PARA FILTRAR APENAS OS ABAIXOS
            if (adm == true)
            {
                cargoAtual = 8;
                filtro = "";
            }
            else
            {
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat(" AND (CL.IDGDA_COLLABORATORS IN ({0}) OR ", CollaboratorId);
                stb.AppendFormat("	    CL.MATRICULA_SUPERVISOR IN ({0}) OR ", CollaboratorId);
                stb.AppendFormat("		CL.MATRICULA_COORDENADOR IN ({0}) OR ", CollaboratorId);
                stb.AppendFormat("		CL.MATRICULA_GERENTE_II IN ({0}) OR ", CollaboratorId);
                stb.AppendFormat("		CL.MATRICULA_GERENTE_I IN ({0}) OR ", CollaboratorId);
                stb.AppendFormat("		CL.MATRICULA_DIRETOR IN ({0}) OR ", CollaboratorId);
                stb.AppendFormat("		CL.MATRICULA_CEO IN ({0})) ", CollaboratorId);
                filtro = stb.ToString();
                cargoAtual = Funcoes.retornaCargoAtual(dtInicial, CollaboratorId);
            }

            //REALIZA A QUERY QUE RETORNA TODAS AS INFORMAÇÕES DOS COLABORADORES QUE TIVERAM MONETIZAÇÃO.
            List<listReportAcess> rmams = new List<listReportAcess>();
            rmams = returnAcessFee(dtInicial, dtFinal, sectorsAsString, hierarchiesAsString, inputModel.Order,  filtro);

            //RETIRANDO OS RESULTADOS DO SUPERVISOR.. ENTENDER COM A TAHTO COMO FICARA ESTA PARTE.
            //rmams = rmams.FindAll(item => item.cargo == "AGENTE").ToList();
            //
            var jsonData = rmams.Select(item => new returnResponseAcess
            {
                DATA_ESCALA = item.DATA_ESCALA,
            DATA_ACESSO = item.DATA_ACESSO,
            DATA_ATUALIZACAO = item.DATA_ATUALIZACAO,
            BC = item.BC,
            NOME = item.NOME,
            CARGO = item.CARGO,
            ESCALA = item.ESCALA,
            ACESSO = item.ACESSO,
            COD_INDICADOR = item.COD_INDICADOR,
            INDICADOR = item.INDICADOR,
            TEMPO_LOGADO = item.TEMPO_LOGADO,
            MATRICULA_SUPERVISOR = item.MATRICULA_SUPERVISOR,
            NOME_SUPERVISOR = item.NOME_SUPERVISOR,
            MATRICULA_COORDENADOR = item.MATRICULA_COORDENADOR,
            NOME_COORDENADOR = item.NOME_COORDENADOR,
            MATRICULA_GERENTE2 = item.MATRICULA_GERENTE2,
            NOME_GERENTE2 = item.NOME_GERENTE2,
            MATRICULA_GERENTE1 = item.MATRICULA_GERENTE1,
            NOME_GERENTE1 = item.NOME_GERENTE1,
            MATRICULA_DIRETOR = item.MATRICULA_DIRETOR,
            NOME_DIRETOR = item.NOME_DIRETOR,
            MATRICULA_CEO = item.MATRICULA_CEO,
            NOME_CEO = item.NOME_CEO,
            CODGIP = (item.CARGO == "AGENTE" || item.CARGO == "SUPERVISOR") ? item.CODGIP : "-",
            SETOR = (item.CARGO == "AGENTE" || item.CARGO == "SUPERVISOR") ? item.SETOR : "-",
            CODGIP_SUBSETOR = (item.CARGO == "AGENTE" || item.CARGO == "SUPERVISOR") ?  item.CODGIP_SUBSETOR : "-" ,
            SUBSETOR = (item.CARGO == "AGENTE" || item.CARGO == "SUPERVISOR") ? item.SUBSETOR : "-",
            TURNO = item.TURNO,
            SITE = item.SITE,
            HOME_BASED = item.HOME_BASED,
            CLIENTE = item.CLIENTE
            }).ToList();

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(jsonData);
        }

        // Método para serializar um DataTable em JSON
    }
}