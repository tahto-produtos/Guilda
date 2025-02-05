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
using static ApiRepositorio.Controllers.ReportNotificationController;
using static ApiRepositorio.Controllers.ScoreInputController;
using System.Threading;
using JetBrains.Annotations;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ReportQuizController : ApiController
    {// POST: api/Results
        public class listQuiz
        {
            public string NOME_QUIZ { get; set; }
            public string DESCRICAO_QUIZ { get; set; }
            public string NOME_CRIADOR { get; set; }
            public string MATRICULA_RESPOSTA { get; set; }
            public string NOME_RESPOSTA { get; set; }
            public string HOME_BASED { get; set; }
            public string CARGO { get; set; }
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
            public string CODGIP_REFERENCE { get; set; }
            public string SETOR_REFERENCE { get; set; }
            public string TIPO_PERGUNTA { get; set; }
            public string PERGUNTA { get; set; }
            public string RESPOSTA_CORRETA { get; set; }
            public string RESPOSTA_SELECIONADA { get; set; }
            public string RESPOSTA_DISPONIVEL { get; set; }
            public string DELETADO_EM { get; set; }
            public string DELETADO_POR { get; set; }

        }
        public class returnResponseDay
        {
            public string NOME_QUIZ { get; set; }
            public string DESCRICAO_QUIZ { get; set; }
            public string NOME_CRIADOR { get; set; }
            public string MATRICULA_RESPOSTA { get; set; }
            public string NOME_RESPOSTA { get; set; }
            public string HOME_BASED { get; set; }
            public string CARGO { get; set; }
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
            public string CODGIP_REFERENCE { get; set; }
            public string SETOR_REFERENCE { get; set; }
            public string TIPO_PERGUNTA { get; set; }
            public string PERGUNTA { get; set; }
            public string RESPOSTA_CORRETA { get; set; }
            public string RESPOSTA_SELECIONADA { get; set; }
            public string RESPOSTA_DISPONIVEL { get; set; }
            public string DELETADO_EM { get; set; }
            public string DELETADO_POR { get; set; }
        }
        public class InputModel
        {
            public DateTime? DataInicialCriacaoQuiz { get; set; }
            public DateTime? DataFinalCriacaoQuiz { get; set; }
            public DateTime? DataInicialPublicacaoQuiz { get; set; }
            public DateTime? DataFinalPublicacaoQuiz { get; set; }
            public DateTime? DataInicialRespostaQuiz { get; set; }
            public DateTime? DataFinalRespostaQuiz { get; set; }
            public string Title { get; set; }
            public List<int> Users { get; set; }
            public List<int> Hierachies { get; set; }
            public List<int> Cities { get; set; }
            public List<int> Sites { get; set; }
            public List<int> Clients { get; set; }
            public int idRequest { get; set; }
            public int idCreated { get; set; }

        }

        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {
            int collaboratorId = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }




            bool existeData = false;
            DateTime dtTimeInicial;
            DateTime dtTimeFinal;
            if (inputModel.DataInicialCriacaoQuiz != (DateTime?)null && inputModel.DataFinalCriacaoQuiz != (DateTime?)null)
            {
                dtTimeInicial = DateTime.ParseExact(inputModel.DataInicialCriacaoQuiz?.ToString("yyyy-MM-dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                dtTimeFinal = DateTime.ParseExact(inputModel.DataFinalCriacaoQuiz?.ToString("yyyy-MM-dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture);

                TimeSpan diff = dtTimeFinal.Subtract(dtTimeInicial);
                int diferencaEmDias = (int)diff.TotalDays;
                if (diferencaEmDias > 31)
                {
                    return BadRequest("Selecionar uma data de no maximo 1 mês!");
                }
                existeData = true;
            }
            if (inputModel.DataInicialPublicacaoQuiz != null && inputModel.DataFinalPublicacaoQuiz != null)
            {
                dtTimeInicial = DateTime.ParseExact(inputModel.DataInicialPublicacaoQuiz?.ToString("yyyy-MM-dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                dtTimeFinal = DateTime.ParseExact(inputModel.DataFinalPublicacaoQuiz?.ToString("yyyy-MM-dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture);

                TimeSpan diff = dtTimeFinal.Subtract(dtTimeInicial);
                int diferencaEmDias = (int)diff.TotalDays;
                if (diferencaEmDias > 31)
                {
                    return BadRequest("Selecionar uma data de no maximo 1 mês!");
                }
                existeData = true;
            }
            if (inputModel.DataInicialRespostaQuiz != null && inputModel.DataFinalRespostaQuiz != null)
            {
                dtTimeInicial = DateTime.ParseExact(inputModel.DataInicialRespostaQuiz?.ToString("yyyy-MM-dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                dtTimeFinal = DateTime.ParseExact(inputModel.DataFinalRespostaQuiz?.ToString("yyyy-MM-dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture);

                TimeSpan diff = dtTimeFinal.Subtract(dtTimeInicial);
                int diferencaEmDias = (int)diff.TotalDays;
                if (diferencaEmDias > 31)
                {
                    return BadRequest("Selecionar uma data de no maximo 1 mês!");
                }
                existeData = true;
            }

            if (existeData == false)
            {
                return BadRequest("Selecionar uma data inicial e uma data final!");
            }


            //RELATORIO DE QUIZ
            collaboratorId = inf.collaboratorId;
            var jsonData = relQuiz(inputModel);

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(jsonData);
        }

        public static List<returnResponseDay> relQuiz(InputModel inputModel, bool Thread = false)
        {




            List<listQuiz> rmams = new List<listQuiz>();
            rmams = returnResultQuiz(inputModel, Thread);


            var jsonData = rmams.Select(item => new returnResponseDay
            {
                NOME_QUIZ = item.NOME_QUIZ,
                DESCRICAO_QUIZ = item.DESCRICAO_QUIZ,
                MATRICULA_RESPOSTA = item.MATRICULA_RESPOSTA,
                NOME_RESPOSTA = item.NOME_RESPOSTA,
                HOME_BASED = item.HOME_BASED,
                CARGO = item.CARGO,
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
                CODGIP = (item.CARGO == "AGENTE" || item.CARGO == "SUPERVISOR") ? item.CODGIP_REFERENCE : "-", //APLICAR REGRA DE SUB SETOR E DE HIERARQUIA
                SETOR = (item.CARGO == "AGENTE" || item.CARGO == "SUPERVISOR") ? item.SETOR_REFERENCE : "-",
                CODGIP_REFERENCE = (item.CARGO != "AGENTE" && item.CARGO != "SUPERVISOR") ? "-" : item.CODGIP == item.CODGIP_REFERENCE ? "-" : item.CODGIP,
                SETOR_REFERENCE = (item.CARGO != "AGENTE" && item.CARGO != "SUPERVISOR") ? "-" : item.CODGIP == item.CODGIP_REFERENCE ? "-" : item.SETOR,
                TIPO_PERGUNTA = item.TIPO_PERGUNTA,
                PERGUNTA = item.PERGUNTA,
                RESPOSTA_CORRETA = item.RESPOSTA_CORRETA,
                RESPOSTA_SELECIONADA = item.RESPOSTA_SELECIONADA,
                RESPOSTA_DISPONIVEL = item.RESPOSTA_DISPONIVEL,
                DELETADO_EM = item.DELETADO_EM,
                DELETADO_POR = item.DELETADO_POR,
                NOME_CRIADOR = item.NOME_CRIADOR,
            }).ToList();

            return jsonData;
        }

        public static List<listQuiz> returnResultQuiz(InputModel inputModel, bool Thread = false)
        {

            string DataInicialCriacaoQuiz = inputModel.DataInicialCriacaoQuiz?.ToString("yyyy-MM-dd");
            string DataFinalCriacaoQuiz = inputModel.DataFinalCriacaoQuiz?.ToString("yyyy-MM-dd");
            string DataInicialPublicacaoQuiz = inputModel.DataInicialPublicacaoQuiz?.ToString("yyyy-MM-dd");
            string DataFinalPublicacaoQuiz = inputModel.DataFinalPublicacaoQuiz?.ToString("yyyy-MM-dd");
            string DataInicialRespostaQuiz = inputModel.DataInicialRespostaQuiz?.ToString("yyyy-MM-dd");
            string DataFinalRespostaQuiz = inputModel.DataFinalRespostaQuiz?.ToString("yyyy-MM-dd");

            // Lista para armazenar todas as datas iniciais e finais
            var datasIniciais = new List<DateTime?>
        {
            inputModel.DataInicialCriacaoQuiz,
            inputModel.DataInicialPublicacaoQuiz,
            inputModel.DataInicialRespostaQuiz
        };

            var datasFinais = new List<DateTime?>
        {
            inputModel.DataFinalCriacaoQuiz,
            inputModel.DataFinalPublicacaoQuiz,
            inputModel.DataFinalRespostaQuiz
        };

            // Obter a menor data inicial e a maior data final, ignorando nulos
            string menorDataInicial = datasIniciais.Where(d => d.HasValue).Min()?.ToString("yyyy-MM-dd");
            string maiorDataFinal = datasFinais.Where(d => d.HasValue).Max()?.ToString("yyyy-MM-dd");


            string filter = "";


            //FILTRO POR CRIACAO DE QUIZ
            if (DataInicialCriacaoQuiz != null)
            {
                filter = filter + $" AND CONVERT(DATE, Q.CREATED_AT) >= CONVERT(DATE,'{DataInicialCriacaoQuiz}') AND CONVERT(DATE, Q.CREATED_AT) <= CONVERT(DATE,'{DataFinalCriacaoQuiz}')  ";
            }

            //FILTRO POR DATA DE PUBLICACAO DO QUIZ
            if (DataInicialPublicacaoQuiz != null)
            {
                filter = filter + $" AND CONVERT(DATE, QU.SENDED_AT) >= CONVERT(DATE,'{DataInicialPublicacaoQuiz}') AND CONVERT(DATE, QU.SENDED_AT) <= CONVERT(DATE,'{DataFinalPublicacaoQuiz}')   ";

            }

            // FILTRO POR DATA DE RESPOSTA DO QUIZ
            if (DataInicialRespostaQuiz != null)
            {
                filter = filter + $" AND CONVERT(DATE, QUA.CREATED_AT) >= CONVERT(DATE,'{DataInicialRespostaQuiz}')  AND CONVERT(DATE, QUA.CREATED_AT) <= CONVERT(DATE,'{DataFinalRespostaQuiz}') ";

            }


            string users = inputModel.Users == null ? "" : string.Join(",", inputModel.Users);
            string hierachies = inputModel.Hierachies == null ? "" : string.Join(",", inputModel.Hierachies);
            string cities = inputModel.Cities == null ? "" : string.Join(",", inputModel.Cities);
            string sites = inputModel.Sites == null ? "" : string.Join(",", inputModel.Sites);
            string clients = inputModel.Clients == null ? "" : string.Join(",", inputModel.Clients);

            if (inputModel.Users.Count > 0)
            {
                filter = filter + $"  AND QU.IDGDA_PERSONA_USER IN ({users}) ";
            }
            if (inputModel.Hierachies.Count > 0)
            {
                filter = filter + $" AND HH.IDGDA_HIERARCHY IN ({hierachies}) ";
            }
            if (inputModel.Cities.Count > 0)
            {
                filter = filter + $" AND PUDS.IDGDA_CITY IN ({cities}) ";
            }
            if (inputModel.Sites.Count > 0)
            {
                filter = filter + $" AND PUDS.SITE IN ({sites}) ";
            }
            if (inputModel.Clients.Count > 0)
            {
                filter = filter + $" AND CD.IDGDA_CLIENT IN ({clients}) ";
            }
            if (inputModel.idRequest != 0)
            {
                filter = filter + $" AND Q.IDGDA_COLLABORATOR_DEMANDANT = {inputModel.idRequest} ";
            }
            if (inputModel.idCreated != 0)
            {
                filter = filter + $" AND Q.IDGDA_COLLABORATOR_RESPONSIBLE = {inputModel.idCreated} ";
            }

            if (inputModel.Title != "")
            {
                filter = filter + $" AND Q.TITLE LIKE '%{inputModel.Title}%' ";
            }

            string dtAg = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            StringBuilder stb = new StringBuilder();

            stb.Append("SELECT	  ");
            stb.Append("    Q.TITLE AS TITLE,  ");
            stb.Append("    Q.DESCRIPTION AS DESCRIPTION,  ");
            stb.Append("    PUD.BC AS BC,  ");
            stb.Append("    PUD.NAME AS NAME,  ");
            stb.Append("    CD.HOME_BASED AS HOME_BASED,  ");
            stb.Append("    CD.CARGO AS CARGO,  ");
            stb.Append("    MAX(Q.DELETED_AT) AS DELETED_AT,  ");
            stb.Append("    MAX(PUDD.NAME) AS NAME_DELETED,  ");
            stb.Append("    MAX(PUDC.NAME) AS NAME_CREATED,  ");
            stb.Append("    MAX(CD.MATRICULA_SUPERVISOR) AS 'MATRICULA SUPERVISOR',   ");
            stb.Append("    MAX(CD.NOME_SUPERVISOR) AS 'NOME SUPERVISOR',   ");
            stb.Append("    MAX(CD.MATRICULA_COORDENADOR) AS 'MATRICULA COORDENADOR',   ");
            stb.Append("    MAX(CD.NOME_COORDENADOR) AS 'NOME COORDENADOR',   ");
            stb.Append("    MAX(CD.MATRICULA_GERENTE_II) AS 'MATRICULA GERENTE II',   ");
            stb.Append("    MAX(CD.NOME_GERENTE_II) AS 'NOME GERENTE II',   ");
            stb.Append("    MAX(CD.MATRICULA_GERENTE_I) AS 'MATRICULA GERENTE I',   ");
            stb.Append("    MAX(CD.NOME_GERENTE_I) AS 'NOME GERENTE I',   ");
            stb.Append("    MAX(CD.MATRICULA_DIRETOR) AS 'MATRICULA DIRETOR',   ");
            stb.Append("    MAX(CD.NOME_DIRETOR) AS 'NOME DIRETOR',   ");
            stb.Append("    MAX(CD.MATRICULA_CEO) AS 'MATRICULA CEO',   ");
            stb.Append("    MAX(CD.NOME_CEO) AS 'NOME CEO',  ");
            stb.Append("    MAX(CD.IDGDA_SECTOR) AS COD_GIP,    ");
            stb.Append("    MAX(SEC.NAME) AS SETOR,    ");
            stb.Append("    CASE WHEN max(CD.IDGDA_SUBSECTOR) IS NULL THEN max(CD.IDGDA_SECTOR) ELSE max(CD.IDGDA_SUBSECTOR) END AS COD_GIP_REFERENCE,    ");
            stb.Append("    MAX(SECREFERENCE.NAME) AS SETOR_REFERENCE,  ");
            stb.Append("    MAX(QQT.TYPE) AS TYPE,  ");
            stb.Append("    QQ.QUESTION AS QUESTION,  ");
            //stb.Append("    (  ");
            //stb.Append("        SELECT CASE WHEN QA_sub1.RIGHT_ANSWER = 1 THEN 1 ELSE 0 END  ");
            //stb.Append("        FROM GDA_QUIZ_USER_ANSWER (NOLOCK) QUA_sub1  ");
            //stb.Append("    	INNER JOIN GDA_QUIZ_ANSWER (NOLOCK) QA_sub1 ON QA_sub1.IDGDA_QUIZ_ANSWER = QUA_sub1.IDGDA_QUIZ_ANSWER  ");
            //stb.Append("        WHERE QUA_sub1.IDGDA_QUIZ_QUESTION = QQ.IDGDA_QUIZ_QUESTION  ");
            //stb.Append("        GROUP BY QUA_sub1.IDGDA_QUIZ_QUESTION, 	QA_sub1.RIGHT_ANSWER  ");
            //stb.Append("    ) AS RESPOSTA_CORRETA,  ");
            stb.Append("    (SELECT STRING_AGG(QA_sub1.QUESTION, ', ') ");
            stb.Append("     FROM GDA_QUIZ_ANSWER (NOLOCK) AS QA_sub1  ");
            stb.Append("     WHERE QA_sub1.IDGDA_QUIZ_QUESTION = QQ.IDGDA_QUIZ_QUESTION AND QA_sub1.RIGHT_ANSWER = 1  ");
            stb.Append("     GROUP BY QA_sub1.IDGDA_QUIZ_QUESTION) AS RESPOSTA_CORRETA, ");
            stb.Append("	(  ");
            stb.Append("        SELECT STRING_AGG(CASE WHEN QUA_sub1.NO_ANSWER = 1 THEN 'SEM RESPOSTA' ELSE QA_sub1.QUESTION END, ', ')  ");
            stb.Append("        FROM GDA_QUIZ_USER_ANSWER (NOLOCK) QUA_sub1  ");
            stb.Append("		INNER JOIN GDA_QUIZ_ANSWER (NOLOCK) QA_sub1 ON QA_sub1.IDGDA_QUIZ_ANSWER = QUA_sub1.IDGDA_QUIZ_ANSWER  ");
            stb.Append("        WHERE QUA_sub1.IDGDA_QUIZ_QUESTION = QQ.IDGDA_QUIZ_QUESTION  ");
            stb.Append("        AND QUA_sub1.IDGDA_QUIZ_USER = QU.IDGDA_QUIZ_USER  ");
            stb.Append("        GROUP BY QUA_sub1.IDGDA_QUIZ_QUESTION  ");
            stb.Append("    ) AS RESPOSTAS_SELECIONADAS,  ");
            stb.Append("    (  ");
            stb.Append("        SELECT STRING_AGG(QA_sub2.QUESTION, ', ')  ");
            stb.Append("        FROM GDA_QUIZ_ANSWER QA_sub2  ");
            stb.Append("        WHERE QA_sub2.IDGDA_QUIZ_QUESTION = QQ.IDGDA_QUIZ_QUESTION  ");
            stb.Append("        GROUP BY QA_sub2.IDGDA_QUIZ_QUESTION  ");
            stb.Append("    ) AS RESPOSTAS_DISPONIVEIS  ");
            stb.Append("FROM   ");
            stb.Append("    GDA_QUIZ_USER (NOLOCK) QU  ");
            stb.Append("    LEFT JOIN GDA_PERSONA_USER (NOLOCK) AS PUD ON PUD.IDGDA_PERSONA_USER = QU.IDGDA_PERSONA_USER  ");
            stb.Append("    LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCU ON PCU.IDGDA_PERSONA_USER = QU.IDGDA_PERSONA_USER  ");
            stb.Append("    LEFT JOIN GDA_QUIZ (NOLOCK) AS Q ON Q.IDGDA_QUIZ = QU.IDGDA_QUIZ  ");
            //stb.Append($"    LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CD ON CD.IDGDA_COLLABORATORS = PCU.IDGDA_COLLABORATORS AND CD.CREATED_AT >= '{dtAg}'  ");
            stb.Append("LEFT JOIN  ");
            stb.Append("( ");
            stb.Append("SELECT IDGDA_COLLABORATORS, MAX(CREATED_AT) AS CREATED_AT, MAX(IDGDA_SECTOR) AS IDGDA_SECTOR,  ");
            stb.Append("       MAX(IDGDA_SUBSECTOR) AS IDGDA_SUBSECTOR, MAX(IDGDA_CLIENT) AS IDGDA_CLIENT, MAX(HOME_BASED) AS HOME_BASED,  ");
            stb.Append("       MAX(CARGO) AS CARGO,  ");
            stb.Append("       MAX(MATRICULA_SUPERVISOR) AS MATRICULA_SUPERVISOR, ");
            stb.Append("       MAX(NOME_SUPERVISOR) AS NOME_SUPERVISOR, ");
            stb.Append("       MAX(MATRICULA_COORDENADOR) AS MATRICULA_COORDENADOR, ");
            stb.Append("       MAX(NOME_COORDENADOR) AS NOME_COORDENADOR, ");
            stb.Append("       MAX(MATRICULA_GERENTE_II) AS MATRICULA_GERENTE_II, ");
            stb.Append("       MAX(NOME_GERENTE_II) AS NOME_GERENTE_II, ");
            stb.Append("       MAX(MATRICULA_GERENTE_I) AS MATRICULA_GERENTE_I, ");
            stb.Append("       MAX(NOME_GERENTE_I) AS NOME_GERENTE_I, ");
            stb.Append("	   MAX(MATRICULA_DIRETOR) AS MATRICULA_DIRETOR,  ");
            stb.Append("	   MAX(NOME_DIRETOR) AS NOME_DIRETOR, ");
            stb.Append("	   MAX(MATRICULA_CEO) AS MATRICULA_CEO, ");
            stb.Append("	   MAX(NOME_CEO) AS NOME_CEO ");
            stb.Append(" FROM GDA_COLLABORATORS_DETAILS (NOLOCK)  ");
            stb.Append($" WHERE CREATED_AT >= DATEADD(DAY, -2, CONVERT(DATE,'{menorDataInicial}')) ");
            stb.Append($" AND CREATED_AT <= CONVERT(DATE,'{maiorDataFinal}') ");
            stb.Append(" GROUP BY IDGDA_COLLABORATORS ");
            stb.Append(" ) CD ON CD.IDGDA_COLLABORATORS = PCU.IDGDA_COLLABORATORS ");

            stb.Append("    LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = CD.IDGDA_SECTOR    ");
            stb.Append("    LEFT JOIN GDA_SECTOR (NOLOCK) AS SECREFERENCE ON SECREFERENCE.IDGDA_SECTOR = case WHEN CD.IDGDA_SUBSECTOR IS NULL THEN CD.IDGDA_SECTOR ELSE CD.IDGDA_SUBSECTOR end  ");

            stb.Append("    LEFT JOIN GDA_PERSONA_USER (NOLOCK) AS PUDD ON PUDD.IDGDA_PERSONA_USER = Q.DELETED_BY  ");
            stb.Append("    LEFT JOIN GDA_PERSONA_USER (NOLOCK) AS PUDC ON PUDC.IDGDA_PERSONA_USER = Q.CREATED_BY  ");
            stb.Append("    LEFT JOIN GDA_QUIZ_QUESTION (NOLOCK) AS QQ ON QQ.IDGDA_QUIZ = Q.IDGDA_QUIZ  ");
            stb.Append("    LEFT JOIN GDA_QUIZ_QUESTION_TYPE (NOLOCK) AS QQT ON QQT.IDGDA_QUIZ_QUESTION_TYPE = QQ.IDGDA_QUIZ_QUESTION_TYPE  ");
            stb.Append("    LEFT JOIN GDA_QUIZ_ANSWER (NOLOCK) AS QA ON QA.IDGDA_QUIZ_QUESTION = QQ.IDGDA_QUIZ_QUESTION  ");
            stb.Append("	LEFT JOIN GDA_QUIZ_USER_ANSWER (NOLOCK) AS QUA ON QUA.IDGDA_QUIZ_ANSWER = QA.IDGDA_QUIZ_ANSWER AND QUA.IDGDA_QUIZ_QUESTION = QQ.IDGDA_QUIZ_QUESTION AND QUA.IDGDA_QUIZ_USER = QU.IDGDA_QUIZ_USER  ");

            stb.Append($"LEFT JOIN GDA_HIERARCHY (NOLOCK) AS HH ON HH.LEVELNAME = CD.CARGO ");
            stb.Append($"LEFT JOIN GDA_PERSONA_USER_DETAILS (NOLOCK) AS PUDS ON PUDS.IDGDA_PERSONA_USER = PUD.IDGDA_PERSONA_USER ");


            stb.Append("WHERE    ");
            stb.Append("    1=1  ");
            stb.Append($"    AND QU.ANSWERED = 1 {filter} ");
            stb.Append("GROUP BY   ");
            stb.Append("    Q.TITLE,  ");
            stb.Append("    Q.DESCRIPTION,  ");
            stb.Append("    PUD.BC,   ");
            stb.Append("    PUD.NAME,   ");
            stb.Append("    CD.HOME_BASED,    ");
            stb.Append("    QQ.IDGDA_QUIZ_QUESTION,  ");
            stb.Append("    CD.CARGO,  ");
            stb.Append("    QQ.QUESTION,  ");
            stb.Append("    QU.IDGDA_QUIZ_USER ");
            List<listQuiz> rmams = new List<listQuiz>();
            using (SqlConnection connection = new SqlConnection(Database.retornaConn(Thread)))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                {
                    command.CommandTimeout = 300;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listQuiz rmam = new listQuiz();

                            rmam.NOME_QUIZ = reader["TITLE"].ToString();
                            rmam.DESCRICAO_QUIZ = reader["DESCRIPTION"].ToString();
                            rmam.MATRICULA_RESPOSTA = reader["BC"].ToString();
                            rmam.NOME_RESPOSTA = reader["NAME"].ToString();
                            rmam.HOME_BASED = reader["HOME_BASED"].ToString();
                            rmam.CARGO = reader["CARGO"].ToString();
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
                            rmam.CODGIP = reader["COD_GIP"].ToString();
                            rmam.SETOR = reader["SETOR"].ToString();
                            rmam.CODGIP_REFERENCE = reader["COD_GIP_REFERENCE"].ToString();
                            rmam.SETOR_REFERENCE = reader["SETOR_REFERENCE"].ToString();
                            rmam.TIPO_PERGUNTA = reader["TYPE"].ToString();
                            rmam.PERGUNTA = reader["QUESTION"].ToString();
                            rmam.RESPOSTA_CORRETA = reader["RESPOSTA_CORRETA"] != DBNull.Value ? reader["RESPOSTA_CORRETA"].ToString() == reader["RESPOSTAS_SELECIONADAS"].ToString() ? "CORRETO" : "INCORRETO" : "";
                            rmam.RESPOSTA_SELECIONADA = reader["RESPOSTAS_SELECIONADAS"] != DBNull.Value ? reader["RESPOSTAS_SELECIONADAS"].ToString() : "SEM RESPOSTA";
                            rmam.RESPOSTA_DISPONIVEL = reader["RESPOSTAS_DISPONIVEIS"].ToString();
                            DateTime dateReturnAt;
                            if (reader["DELETED_AT"] != DBNull.Value && DateTime.TryParse(reader["DELETED_AT"].ToString(), out dateReturnAt))
                            {
                                rmam.DELETADO_EM = dateReturnAt.ToString("dd/MM/yyyy");
                            }
                            rmam.DELETADO_POR = reader["NAME_DELETED"] != DBNull.Value ? reader["NAME_DELETED"].ToString() : "";
                            rmam.NOME_CRIADOR = reader["NAME_CREATED"] != DBNull.Value ? reader["NAME_CREATED"].ToString() : "";

                            rmams.Add(rmam);
                        }
                    }
                }
                connection.Close();
            }
            return rmams;
        }
    }
}