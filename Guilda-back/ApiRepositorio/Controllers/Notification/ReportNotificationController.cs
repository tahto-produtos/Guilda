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
    public class ReportNotificationController : ApiController
    {// POST: api/Results
        public class listNotification
        {
            public string DATA_ENVIO { get; set; }
            public string TEMA { get; set; }
            public string GRUPO { get; set; }
            public string IDCOLLABORATOR { get; set; }
            public string BC { get; set; }
            public string NOME { get; set; }
            public string CARGO { get; set; }
            public string SITE { get; set; }
            public string NOTIFICACAO { get; set; }
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
            public string COD_GIP_SUB { get; set; }
            public string SUBSETOR { get; set; }
            public string IDCOLABORADOR_ENVIO { get; set; }
            public string NOME_ENVIO { get; set; }
            public string SETOR_ENVIO { get; set; }
            public string DATA_INICIO { get; set; }
            public string DATA_EXPIRACAO { get; set; }
            public string STATUS { get; set; }
            public string IDCOLLABORADOR_EXCLUSAO { get; set; }
            public string NOME_EXCLUSAO { get; set; }
            public string SETOR_EXCLUSAO { get; set; }
            public string CLIENTE { get; set; }
        }
        public class returnResponseDay
        {
            public string DATA_ENVIO { get; set; }
            public string TEMA { get; set; }
            public string GRUPO { get; set; }
            public string IDCOLLABORATOR { get; set; }
            public string BC { get; set; }
            public string NOME { get; set; }
            public string CARGO { get; set; }
            public string SITE { get; set; }
            public string NOTIFICACAO { get; set; }
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
            public string COD_GIP_SUB { get; set; }
            public string SUBSETOR { get; set; }
            public string IDCOLABORADOR_ENVIO { get; set; }
            public string NOME_ENVIO { get; set; }
            public string SETOR_ENVIO { get; set; }
            public string DATA_INICIO { get; set; }
            public string DATA_EXPIRACAO { get; set; }
            public string STATUS { get; set; }
            public string IDCOLLABORADOR_EXCLUSAO { get; set; }
            public string NOME_EXCLUSAO { get; set; }
            public string SETOR_EXCLUSAO { get; set; }
            public string CLIENTE { get; set; }
        }
        public class InputModel
        {
            public List<int> Sectors { get; set; }
            public List<int> Destination { get; set; }
            public List<int> Site { get; set; }
            public List<int> Client { get; set; }
            public List<int> Hierarchies { get; set; }
            public DateTime DataInicial { get; set; }
            public DateTime DataFinal { get; set; }
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
            collaboratorId = inf.collaboratorId;


            string dtInicial = inputModel.DataInicial.ToString("yyyy-MM-dd");
            string dtFinal = inputModel.DataFinal.ToString("yyyy-MM-dd");
            //string groupsAsString = string.Join(",", inputModel.Groups.Select(g => g.Id));
            string sectorsAsString = string.Join(",", inputModel.Sectors);
            string hierarchiesAsString = string.Join(",", inputModel.Hierarchies);
            string destinationNotificationAsString = string.Join(",", inputModel.Destination);
            string siteAsString = string.Join(",", inputModel.Site);
            string ClienteAsString = string.Join(",", inputModel.Client);
            DateTime dtTimeInicial = DateTime.ParseExact(dtInicial, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dtTimeFinal = DateTime.ParseExact(dtFinal, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            TimeSpan diff = dtTimeFinal.Subtract(dtTimeInicial);
            int diferencaEmDias = (int)diff.TotalDays;
            if (diferencaEmDias > 31)
            {
                return BadRequest("Selecionar uma data de no maximo 1 mês!");
            }

            var jsonData = relNotification(dtInicial, dtFinal, destinationNotificationAsString, sectorsAsString, hierarchiesAsString, siteAsString, ClienteAsString);


            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(jsonData);
        }


        //public async Task<HttpResponseMessage> PostResultsModel([FromBody] InputModel inputModel)
        //{
        //    int collaboratorId = 0;
        //    var token = Request.Headers.Authorization?.Parameter;
        //    bool tokn = TokenService.TryDecodeToken(token);
        //    if (!tokn)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.Unauthorized);
        //    }

        //    collaboratorId = TokenService.InfsUsers.collaboratorId;
        //    string dtInicial = inputModel.DataInicial.ToString("yyyy-MM-dd");
        //    string dtFinal = inputModel.DataFinal.ToString("yyyy-MM-dd");

        //    string sectorsAsString = string.Join(",", inputModel.Sectors.Select(g => g.Id));
        //    string hierarchiesAsString = string.Join(",", inputModel.Hierarchies.Select(g => g.Id));
        //    string destinationNotificationAsString = string.Join(",", inputModel.Destination.Select(g => g.Id));
        //    string siteAsString = string.Join(",", inputModel.Site.Select(g => g.Id));
        //    string ClienteAsString = string.Join(",", inputModel.Client.Select(g => g.Id));

        //    DateTime dtTimeInicial = DateTime.ParseExact(dtInicial, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        //    DateTime dtTimeFinal = DateTime.ParseExact(dtFinal, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        //    TimeSpan diff = dtTimeFinal.Subtract(dtTimeInicial);
        //    int diferencaEmDias = (int)diff.TotalDays;

        //    if (diferencaEmDias > 31)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, "Selecionar uma data de no máximo 1 mês!");
        //    }

        //    // Inicialize sua lógica para obter ou gerar os dados
        //    var jsonData = relNotification(dtInicial, dtFinal, destinationNotificationAsString, sectorsAsString, hierarchiesAsString, siteAsString, ClienteAsString);

        //    // Crie uma stream de memória para armazenar os dados serializados
        //    var memoryStream = new MemoryStream();

        //    // Use um StreamWriter para escrever na stream de memória
        //    using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8, 4096, true))
        //    {
        //        // Serializa os dados em JSON e escreve na stream de memória
        //        var jsonSerializer = new JsonSerializer();
        //        jsonSerializer.Serialize(streamWriter, jsonData);
        //    }

        //    // Posicione a stream de memória no início
        //    memoryStream.Seek(0, SeekOrigin.Begin);

        //    // Retorne a stream de memória
        //    return new HttpResponseMessage(HttpStatusCode.OK)
        //    {
        //        Content = new StreamContent(memoryStream)
        //        {
        //            Headers =
        //    {
        //        ContentType = new MediaTypeHeaderValue("application/octet-stream")
        //    }
        //        }
        //    };
        //}

        public static List<returnResponseDay> relNotification(string dtInicial, string dtFinal, string destinationNotificationAsString, string sectorsAsString, string hierarchiesAsString, string siteAsString, string ClienteAsString, bool Thread = false)
        {
            List<listNotification> rmams = new List<listNotification>();
            rmams = returnMonetizationDayMonth(dtInicial, dtFinal, destinationNotificationAsString, sectorsAsString, hierarchiesAsString, siteAsString, ClienteAsString, Thread);


            // Usa a serialização assíncrona para escrever os dados diretamente no Response Body


            var jsonData = rmams.Select(item => new returnResponseDay
            {
                DATA_ENVIO = item.DATA_ENVIO,
                TEMA = item.TEMA,
                GRUPO = item.GRUPO,
                IDCOLLABORATOR = item.IDCOLLABORATOR,
                BC = item.BC,
                NOME = item.NOME,
                CARGO = item.CARGO,
                SITE = item.SITE,
                NOTIFICACAO = item.NOTIFICACAO,
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
                CODGIP = (item.CARGO == "AGENTE" || item.CARGO == "SUPERVISOR") ? item.CODGIP : "-", //APLICAR REGRA DE SUB SETOR E DE HIERARQUIA
                SETOR = (item.CARGO == "AGENTE" || item.CARGO == "SUPERVISOR") ? item.SETOR : "-",
                COD_GIP_SUB = (item.CARGO == "AGENTE" || item.CARGO == "SUPERVISOR") ? item.COD_GIP_SUB : "-",
                SUBSETOR = (item.CARGO == "AGENTE" || item.CARGO == "SUPERVISOR") ? item.SUBSETOR : "-",
                IDCOLABORADOR_ENVIO = item.IDCOLABORADOR_ENVIO,
                NOME_ENVIO = item.NOME_ENVIO,
                SETOR_ENVIO = item.SETOR_ENVIO,
                DATA_INICIO = item.DATA_INICIO,
                DATA_EXPIRACAO = item.DATA_EXPIRACAO,
                STATUS = item.STATUS,
                IDCOLLABORADOR_EXCLUSAO = item.IDCOLLABORADOR_EXCLUSAO,
                NOME_EXCLUSAO = item.NOME_EXCLUSAO,
                SETOR_EXCLUSAO = item.SETOR_EXCLUSAO,
                CLIENTE = item.CLIENTE
            }).ToList();

            return jsonData;
        }

        public static List<listNotification> returnMonetizationDayMonth(string dtInicial, string dtFinal, string destination, string sectors, string hierarchies, string site, string Cliente, bool Thread = false)
        {
            //PREPARAR OS FILTROS
            string filter = "";
            string orderBy = "";

            filter = filter + $" AND CONVERT(DATE, N.CREATED_AT) >= CONVERT(DATE,@DATAINICIAL) ";
            filter = filter + $" AND CONVERT(DATE, N.CREATED_AT) <= CONVERT(DATE,@DATAFINAL) ";
            //filter = filter + $" AND N.DELETED_AT IS NULL ";

            //FILTRO POR SETOR.
            if (sectors != "")
            {
                filter = filter + $" AND CL1.IDGDA_SECTOR IN ({sectors}) ";
            }

            //FILTRO POR DESTINATARIO.
            if (destination != "")
            {
                filter = filter + $" AND CUP.IDGDA_COLLABORATORS IN ({destination}) ";
            }

            //FILTRO POR HERARQUIA.
            if (hierarchies != "")
            {
                filter = filter + $" AND HI.IDGDA_HIERARCHY IN ({hierarchies}) ";
            }

            //FILTRO POR SITE.
            if (site != "")
            {
                filter = filter + $" AND SI.IDGDA_SITE  IN ({site}) ";
            }

            //FILTRO POR CLIENTE.
            if (Cliente != "")
            {
                filter = filter + $" AND CLI.IDGDA_CLIENT IN ({Cliente}) ";
            }


            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtInicial);
            stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFinal);
            stb.AppendFormat("SELECT ");
            stb.AppendFormat("IDGDA_NOTIFICATION_USER, ");
            stb.AppendFormat("MAX(N.CREATED_AT) AS DATA_ENVIO, ");
            stb.AppendFormat("MAX(TITLE) AS TEMA, ");
            stb.AppendFormat("MAX(CL1.IDGDA_GROUP) AS GRUPO, ");
            stb.AppendFormat("MAX(CUP.IDGDA_COLLABORATORS) AS IDGDA_COLLABORATOR, ");
            stb.AppendFormat("MAX(PU.BC) AS BC, ");
            stb.AppendFormat("MAX(PU.NAME) AS NOME, ");
            stb.AppendFormat("MAX(CL1.CARGO) AS CARGO, ");
            stb.AppendFormat("MAX(CL1.SITE) AS SITE, ");
            stb.AppendFormat("MAX(CASE ");
            stb.AppendFormat("        WHEN NU.VIEWED_AT IS NULL THEN 'NAO LIDA' ");
            stb.AppendFormat("        ELSE 'LIDA' ");
            stb.AppendFormat("    END) AS NOTIFICACAO, ");
            stb.AppendFormat("MAX(CL1.MATRICULA_SUPERVISOR) AS 'MATRICULA SUPERVISOR', ");
            stb.AppendFormat("MAX(CL1.NOME_SUPERVISOR) AS 'NOME SUPERVISOR', ");
            stb.AppendFormat("MAX(CL1.MATRICULA_COORDENADOR) AS 'MATRICULA COORDENADOR', ");
            stb.AppendFormat("MAX(CL1.NOME_COORDENADOR) AS 'NOME COORDENADOR', ");
            stb.AppendFormat("MAX(CL1.MATRICULA_GERENTE_II) AS 'MATRICULA GERENTE II', ");
            stb.AppendFormat("MAX(CL1.NOME_GERENTE_II) AS 'NOME GERENTE II', ");
            stb.AppendFormat("MAX(CL1.MATRICULA_GERENTE_I) AS 'MATRICULA GERENTE I', ");
            stb.AppendFormat("MAX(CL1.NOME_GERENTE_I) AS 'NOME GERENTE I', ");
            stb.AppendFormat("MAX(CL1.MATRICULA_DIRETOR) AS 'MATRICULA DIRETOR', ");
            stb.AppendFormat("MAX(CL1.NOME_DIRETOR) AS 'NOME DIRETOR', ");
            stb.AppendFormat("MAX(CL1.MATRICULA_CEO) AS 'MATRICULA CEO', ");
            stb.AppendFormat("MAX(CL1.NOME_CEO) AS 'NOME CEO', ");
            stb.AppendFormat("MAX(CL1.IDGDA_SECTOR) AS COD_GIP, ");
            stb.AppendFormat("MAX(SEC.NAME) AS SETOR, ");
            stb.AppendFormat("MAX(CL1.IDGDA_SUBSECTOR) AS COD_GIP_SUB, ");
            stb.AppendFormat("MAX(SUBSEC.NAME) AS SUBSETOR, ");
            stb.AppendFormat("MAX(CL2.IDGDA_COLLABORATORS) ID_COLLABORATOR_ENVIO, ");
            stb.AppendFormat("MAX(PU.NAME) NOME_ENVIO, ");
            stb.AppendFormat("MAX(CL2.IDGDA_SECTOR) AS SETOR_ENVIO, ");
            stb.AppendFormat("MAX(N.STARTED_AT) AS DATA_INICIO, ");
            stb.AppendFormat("MAX(N.ENDED_AT) AS DATA_EXPIRACAO, ");
            stb.AppendFormat("MAX(CASE ");
            stb.AppendFormat("      WHEN N.ACTIVE = 0 ");
            stb.AppendFormat("           AND N.DELETED_AT IS NULL THEN 'INATIVO' ");
            stb.AppendFormat("      WHEN N.ACTIVE = 1 ");
            stb.AppendFormat("           AND N.STARTED_AT >= @DATAINICIAL ");
            stb.AppendFormat("           AND (N.ENDED_AT <= @DATAFINAL ");
            stb.AppendFormat("                OR N.ENDED_AT IS NULL) THEN 'ATIVO' ");
            stb.AppendFormat("      WHEN N.DELETED_AT IS NOT NULL THEN 'EXCLUIDO' ");
            stb.AppendFormat("      WHEN N.ACTIVE = 1 ");
            stb.AppendFormat("           AND N.STARTED_AT < @DATAFINAL ");
            stb.AppendFormat("           AND N.ENDED_AT < @DATAFINAL THEN 'EXPIRADO' ");
            stb.AppendFormat("  END) AS STATUS, ");
            stb.AppendFormat("MAX(CL3.IDGDA_COLLABORATORS) ID_COLLABORATOR_EXCLUIU, ");
            stb.AppendFormat("MAX(PUD.NAME) NOME_EXCLUIU, ");
            stb.AppendFormat("MAX(CL3.IDGDA_SECTOR) AS SETOR_EXCLUIU, ");
            stb.AppendFormat("MAX(CLI.CLIENT) AS CLIENTE ");
            stb.AppendFormat("FROM GDA_NOTIFICATION (NOLOCK) AS N ");
            stb.AppendFormat("INNER JOIN GDA_NOTIFICATION_USER (NOLOCK) AS NU ON N.IDGDA_NOTIFICATION = NU.IDGDA_NOTIFICATION ");
            stb.AppendFormat(" ");
            stb.AppendFormat("INNER JOIN GDA_PERSONA_USER (NOLOCK) AS PU ON PU.IDGDA_PERSONA_USER = NU.IDGDA_PERSONA_USER ");
            stb.AppendFormat("INNER JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) CUP ON CUP.IDGDA_PERSONA_USER = PU.IDGDA_PERSONA_USER ");
            stb.AppendFormat("INNER JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) CL1 ON CL1.CREATED_AT = CONVERT(DATE, DATEADD(DAY, -1, GETDATE())) AND CL1.IDGDA_COLLABORATORS = CUP.IDGDA_COLLABORATORS ");
            stb.AppendFormat(" ");
            stb.AppendFormat("INNER JOIN GDA_PERSONA_USER (NOLOCK) AS PUC ON PUC.IDGDA_PERSONA_USER = N.CREATED_BY ");
            stb.AppendFormat("INNER JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) CUC ON CUC.IDGDA_PERSONA_USER = PUC.IDGDA_PERSONA_USER ");
            stb.AppendFormat("INNER JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) CL2 ON CL2.CREATED_AT = CONVERT(DATE, DATEADD(DAY, -1, GETDATE())) AND CL2.IDGDA_COLLABORATORS = CUC.IDGDA_COLLABORATORS ");
            stb.AppendFormat(" ");

            stb.AppendFormat("LEFT JOIN GDA_PERSONA_USER (NOLOCK) AS PUD ON PUD.IDGDA_PERSONA_USER = N.DELETED_BY ");
            stb.AppendFormat("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) CUD ON CUD.IDGDA_PERSONA_USER = PUD.IDGDA_PERSONA_USER ");
            stb.AppendFormat("LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) CL3 ON CL3.CREATED_AT = CONVERT(DATE, DATEADD(DAY, -1, GETDATE())) AND CL3.IDGDA_COLLABORATORS = CUD.IDGDA_COLLABORATORS ");
            stb.AppendFormat(" ");
            stb.AppendFormat("LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = CL1.IDGDA_SECTOR ");
            stb.AppendFormat("LEFT JOIN GDA_SECTOR (NOLOCK) AS SUBSEC ON SUBSEC.IDGDA_SECTOR = CL1.IDGDA_SUBSECTOR ");
            stb.AppendFormat("LEFT JOIN GDA_CLIENT (NOLOCK) AS CLI ON CLI.IDGDA_CLIENT = CL1.IDGDA_CLIENT ");
            stb.AppendFormat("LEFT JOIN GDA_SITE (NOLOCK) AS SI ON SI.SITE = CL1.SITE ");
            stb.AppendFormat("LEFT JOIN GDA_HIERARCHY (NOLOCK) AS HI ON HI.LEVELNAME = CL1.CARGO ");
            stb.AppendFormat($"WHERE 1 = 1 AND CL1.CARGO IS NOT NULL {filter} ");
            stb.AppendFormat("GROUP BY IDGDA_NOTIFICATION_USER ");

            #region Antigo
            //StringBuilder stb = new StringBuilder();
            //stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtInicial);
            //stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFinal);
            //stb.AppendFormat("SELECT  ");
            //stb.AppendFormat("	   N.CREATED_AT AS DATA_ENVIO, ");
            //stb.AppendFormat("       MAX(TITLE) AS TEMA, ");
            //stb.AppendFormat("       MAX(CL1.IDGDA_GROUP) AS GRUPO, ");
            //stb.AppendFormat("       PUT.IDGDA_COLLABORATORS AS IDGDA_COLLABORATOR, ");
            //stb.AppendFormat("       PU.BC AS BC, ");
            //stb.AppendFormat("       PU.NAME AS NOME, ");
            //stb.AppendFormat("       MAX(CL1.CARGO) AS CARGO, ");
            //stb.AppendFormat("       MAX(CL1.SITE) AS SITE, ");
            //stb.AppendFormat("       MAX(CASE ");
            //stb.AppendFormat("               WHEN NU.VIEWED_AT IS NULL THEN 'NAO LIDA' ");
            //stb.AppendFormat("               ELSE 'LIDA' ");
            //stb.AppendFormat("           END) AS NOTIFICACAO, ");
            //stb.AppendFormat("       MAX(CL1.MATRICULA_SUPERVISOR) AS 'MATRICULA SUPERVISOR', ");
            //stb.AppendFormat("       MAX(CL1.NOME_SUPERVISOR) AS 'NOME SUPERVISOR', ");
            //stb.AppendFormat("       MAX(CL1.MATRICULA_COORDENADOR) AS 'MATRICULA COORDENADOR', ");
            //stb.AppendFormat("       MAX(CL1.NOME_COORDENADOR) AS 'NOME COORDENADOR', ");
            //stb.AppendFormat("       MAX(CL1.MATRICULA_GERENTE_II) AS 'MATRICULA GERENTE II', ");
            //stb.AppendFormat("       MAX(CL1.NOME_GERENTE_II) AS 'NOME GERENTE II', ");
            //stb.AppendFormat("       MAX(CL1.MATRICULA_GERENTE_I) AS 'MATRICULA GERENTE I', ");
            //stb.AppendFormat("       MAX(CL1.NOME_GERENTE_I) AS 'NOME GERENTE I', ");
            //stb.AppendFormat("       MAX(CL1.MATRICULA_DIRETOR) AS 'MATRICULA DIRETOR', ");
            //stb.AppendFormat("       MAX(CL1.NOME_DIRETOR) AS 'NOME DIRETOR', ");
            //stb.AppendFormat("       MAX(CL1.MATRICULA_CEO) AS 'MATRICULA CEO', ");
            //stb.AppendFormat("       MAX(CL1.NOME_CEO) AS 'NOME CEO', ");
            //stb.AppendFormat("       MAX(CL1.IDGDA_SECTOR) AS COD_GIP, ");
            //stb.AppendFormat("       MAX(SEC.NAME) AS SETOR, ");
            //stb.AppendFormat("       MAX(CL1.COD_GIP_REFERENCE) AS COD_GIP_REFERENCE, ");
            //stb.AppendFormat("       MAX(SECREFERENCE.NAME) AS SETOR_REFERENCE, ");
            //stb.AppendFormat("       MAX(CL2.IDGDA_COLLABORATORS) ID_COLLABORATOR_ENVIO, ");
            //stb.AppendFormat("       MAX(G1.NAME) NOME_ENVIO, ");
            //stb.AppendFormat("       MAX(CL2.IDGDA_SECTOR) AS SETOR_ENVIO, ");
            //stb.AppendFormat("       MAX(N.STARTED_AT) AS DATA_INICIO, ");
            //stb.AppendFormat("       MAX(N.ENDED_AT) AS DATA_EXPIRACAO, ");
            //stb.AppendFormat("       MAX(CASE ");
            //stb.AppendFormat("               WHEN N.ACTIVE = 0 ");
            //stb.AppendFormat("                    AND N.DELETED_AT IS NULL THEN 'INATIVO' ");
            //stb.AppendFormat("               WHEN N.ACTIVE = 1 ");
            //stb.AppendFormat("                    AND N.STARTED_AT >= @DATAINICIAL ");
            //stb.AppendFormat("                    AND (N.ENDED_AT <= @DATAFINAL OR N.ENDED_AT IS NULL) THEN 'ATIVO' ");
            //stb.AppendFormat("               WHEN N.DELETED_AT IS NOT NULL THEN 'EXCLUIDO' ");
            //stb.AppendFormat("               WHEN N.ACTIVE = 1 ");
            //stb.AppendFormat("                    AND N.STARTED_AT < @DATAFINAL ");
            //stb.AppendFormat("                    AND N.ENDED_AT < @DATAFINAL THEN 'EXPIRADO' ");
            //stb.AppendFormat("           END) AS STATUS, ");
            //stb.AppendFormat("       MAX(CL3.IDGDA_COLLABORATORS) ID_COLLABORATOR_EXCLUIU, ");
            //stb.AppendFormat("       MAX(G2.NAME) NOME_EXCLUIU, ");
            //stb.AppendFormat("       MAX(CL3.IDGDA_SECTOR) AS SETOR_EXCLUIU, ");
            //stb.AppendFormat("       MAX(CLI.CLIENT) AS CLIENTE ");
            //stb.AppendFormat("FROM GDA_NOTIFICATION (NOLOCK) N ");
            //stb.AppendFormat("LEFT JOIN GDA_NOTIFICATION_USER (NOLOCK) AS NU ON NU.IDGDA_NOTIFICATION = N.IDGDA_NOTIFICATION ");
            //stb.AppendFormat("LEFT JOIN GDA_PERSONA_USER (NOLOCK) AS PU ON PU.IDGDA_PERSONA_USER = NU.IDGDA_PERSONA_USER ");
            //stb.AppendFormat("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PUT ON PUT.IDGDA_PERSONA_USER = PU.IDGDA_PERSONA_USER ");
            //stb.AppendFormat("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PUT2 ON PUT2.IDGDA_PERSONA_USER = N.CREATED_BY ");
            //stb.AppendFormat("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PUT3 ON PUT3.IDGDA_PERSONA_USER = N.DELETED_BY ");
            //stb.AppendFormat("LEFT JOIN (SELECT IDGDA_COLLABORATORS, ");
            //stb.AppendFormat("				  IDGDA_GROUP, ");
            //stb.AppendFormat("				  CREATED_AT, ");
            //stb.AppendFormat("				  CARGO, ");
            //stb.AppendFormat("				  SITE, ");
            //stb.AppendFormat("				  MATRICULA_SUPERVISOR,  ");
            //stb.AppendFormat("				  NOME_SUPERVISOR, ");
            //stb.AppendFormat("				  MATRICULA_COORDENADOR, ");
            //stb.AppendFormat("				  NOME_COORDENADOR, ");
            //stb.AppendFormat("				  MATRICULA_GERENTE_II, ");
            //stb.AppendFormat("				  NOME_GERENTE_II, ");
            //stb.AppendFormat("				  MATRICULA_GERENTE_I, ");
            //stb.AppendFormat("				  NOME_GERENTE_I, ");
            //stb.AppendFormat("				  MATRICULA_DIRETOR, ");
            //stb.AppendFormat("				  NOME_DIRETOR, ");
            //stb.AppendFormat("				  MATRICULA_CEO, ");
            //stb.AppendFormat("				  NOME_CEO, ");
            //stb.AppendFormat("				  IDGDA_SECTOR, ");
            //stb.AppendFormat("				  IDGDA_CLIENT, ");
            //stb.AppendFormat("				  CASE ");
            //stb.AppendFormat("				      WHEN IDGDA_SUBSECTOR IS NULL THEN IDGDA_SECTOR ");
            //stb.AppendFormat("				      ELSE IDGDA_SUBSECTOR ");
            //stb.AppendFormat("				  END AS COD_GIP_REFERENCE FROM GDA_COLLABORATORS_DETAILS (NOLOCK)   ");
            //stb.AppendFormat("				  WHERE CREATED_AT >= CONVERT(DATE, DATEADD(DAY, -1, GETDATE()))) AS CL1 ON CL1.IDGDA_COLLABORATORS = PUT.IDGDA_COLLABORATORS ");
            //stb.AppendFormat("LEFT JOIN ( SELECT ");
            //stb.AppendFormat("				  IDGDA_COLLABORATORS, ");
            //stb.AppendFormat("				   CASE ");
            //stb.AppendFormat("				      WHEN IDGDA_SUBSECTOR IS NULL THEN IDGDA_SECTOR ");
            //stb.AppendFormat("				      ELSE IDGDA_SUBSECTOR ");
            //stb.AppendFormat("				  END AS IDGDA_SECTOR ");
            //stb.AppendFormat("			FROM GDA_COLLABORATORS_DETAILS (NOLOCK)  ");
            //stb.AppendFormat("			WHERE CREATED_AT >= CONVERT(DATE, DATEADD(DAY, -1, GETDATE()))) AS CL2 ON CL2.IDGDA_COLLABORATORS = PUT2.IDGDA_COLLABORATORS ");
            //stb.AppendFormat("LEFT JOIN ( SELECT ");
            //stb.AppendFormat("				  IDGDA_COLLABORATORS, ");
            //stb.AppendFormat("				   CASE ");
            //stb.AppendFormat("				      WHEN IDGDA_SUBSECTOR IS NULL THEN IDGDA_SECTOR ");
            //stb.AppendFormat("				      ELSE IDGDA_SUBSECTOR ");
            //stb.AppendFormat("				  END AS IDGDA_SECTOR ");
            //stb.AppendFormat("			FROM GDA_COLLABORATORS_DETAILS (NOLOCK)  ");
            //stb.AppendFormat("			WHERE CREATED_AT >= CONVERT(DATE, DATEADD(DAY, -1, GETDATE()))) AS CL3 ON CL3.IDGDA_COLLABORATORS = PUT3.IDGDA_COLLABORATORS ");
            //stb.AppendFormat("LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS G1 ON G1.IDGDA_COLLABORATORS = CL2.IDGDA_COLLABORATORS ");
            //stb.AppendFormat("LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS G2 ON G2.IDGDA_COLLABORATORS = CL3.IDGDA_COLLABORATORS ");
            //stb.AppendFormat("LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = CL1.IDGDA_SECTOR ");
            //stb.AppendFormat("LEFT JOIN GDA_SECTOR (NOLOCK) AS SECREFERENCE ON SECREFERENCE.IDGDA_SECTOR = CL1.COD_GIP_REFERENCE ");
            //stb.AppendFormat("LEFT JOIN GDA_CLIENT (NOLOCK) AS CLI ON CLI.IDGDA_CLIENT = CL1.IDGDA_CLIENT ");
            //stb.AppendFormat("WHERE 1=1 ");
            //stb.AppendFormat(" ");
            //stb.AppendFormat("{0}  ", filter);
            //stb.AppendFormat(" ");
            //stb.AppendFormat("GROUP BY N.CREATED_AT,PU.BC, PU.NAME, PUT.IDGDA_COLLABORATORS ");
            #endregion

            List<listNotification> rmams = new List<listNotification>();
            using (SqlConnection connection = new SqlConnection(Database.retornaConn(Thread)))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                {
                    command.CommandTimeout = 900;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listNotification rmam = new listNotification();
                            rmam.DATA_ENVIO = reader["DATA_ENVIO"] != DBNull.Value ? reader["DATA_ENVIO"].ToString() : "";
                            rmam.TEMA = reader["TEMA"] != DBNull.Value ? reader["TEMA"].ToString() : "";
                            rmam.GRUPO = reader["GRUPO"] != DBNull.Value ? reader["GRUPO"].ToString() : "";
                            rmam.IDCOLLABORATOR = reader["IDGDA_COLLABORATOR"] != DBNull.Value ? reader["IDGDA_COLLABORATOR"].ToString() : "";
                            rmam.BC = reader["BC"] != DBNull.Value ? reader["BC"].ToString() : "";
                            rmam.NOME = reader["NOME"] != DBNull.Value ? reader["NOME"].ToString() : "";
                            rmam.CARGO = reader["CARGO"] != DBNull.Value ? reader["CARGO"].ToString() : "";
                            rmam.SITE = reader["SITE"] != DBNull.Value ? reader["SITE"].ToString() : "";
                            rmam.NOTIFICACAO = reader["NOTIFICACAO"] != DBNull.Value ? reader["NOTIFICACAO"].ToString() : "";
                            rmam.MATRICULA_SUPERVISOR = reader["MATRICULA SUPERVISOR"] != DBNull.Value ? reader["MATRICULA SUPERVISOR"].ToString() : "";
                            rmam.NOME_SUPERVISOR = reader["NOME SUPERVISOR"] != DBNull.Value ? reader["NOME SUPERVISOR"].ToString() : "";
                            rmam.MATRICULA_COORDENADOR = reader["MATRICULA COORDENADOR"] != DBNull.Value ? reader["MATRICULA COORDENADOR"].ToString() : "";
                            rmam.NOME_COORDENADOR = reader["NOME COORDENADOR"] != DBNull.Value ? reader["NOME COORDENADOR"].ToString() : "";
                            rmam.MATRICULA_GERENTE2 = reader["MATRICULA GERENTE II"] != DBNull.Value ? reader["MATRICULA GERENTE II"].ToString() : "";
                            rmam.NOME_GERENTE2 = reader["NOME GERENTE II"] != DBNull.Value ? reader["NOME GERENTE II"].ToString() : "";
                            rmam.MATRICULA_GERENTE1 = reader["MATRICULA GERENTE I"] != DBNull.Value ? reader["MATRICULA GERENTE I"].ToString() : "";
                            rmam.NOME_GERENTE1 = reader["NOME GERENTE I"] != DBNull.Value ? reader["NOME GERENTE I"].ToString() : "";
                            rmam.MATRICULA_DIRETOR = reader["MATRICULA DIRETOR"] != DBNull.Value ? reader["MATRICULA DIRETOR"].ToString() : "";
                            rmam.NOME_DIRETOR = reader["NOME DIRETOR"] != DBNull.Value ? reader["NOME DIRETOR"].ToString() : "";
                            rmam.MATRICULA_CEO = reader["MATRICULA CEO"] != DBNull.Value ? reader["MATRICULA CEO"].ToString() : "";
                            rmam.NOME_CEO = reader["NOME CEO"] != DBNull.Value ? reader["NOME CEO"].ToString() : "";
                            rmam.CODGIP = reader["COD_GIP"] != DBNull.Value ? reader["COD_GIP"].ToString() : "";
                            rmam.SETOR = reader["SETOR"] != DBNull.Value ? reader["SETOR"].ToString() : "";
                            rmam.COD_GIP_SUB = reader["COD_GIP_SUB"] != DBNull.Value ? reader["COD_GIP_SUB"].ToString() : "";
                            rmam.SUBSETOR = reader["SUBSETOR"] != DBNull.Value ? reader["SUBSETOR"].ToString() : "";
                            rmam.IDCOLABORADOR_ENVIO = reader["ID_COLLABORATOR_ENVIO"] != DBNull.Value ? reader["ID_COLLABORATOR_ENVIO"].ToString() : "";
                            rmam.NOME_ENVIO = reader["NOME_ENVIO"] != DBNull.Value ? reader["NOME_ENVIO"].ToString() : "";
                            rmam.SETOR_ENVIO = reader["SETOR_ENVIO"] != DBNull.Value ? reader["SETOR_ENVIO"].ToString() : "";
                            rmam.DATA_ENVIO = reader["DATA_INICIO"] != DBNull.Value ? reader["DATA_INICIO"].ToString() : "";
                            rmam.DATA_EXPIRACAO = reader["DATA_EXPIRACAO"] != DBNull.Value ? reader["DATA_EXPIRACAO"].ToString() : "";
                            rmam.STATUS = reader["STATUS"] != DBNull.Value ? reader["STATUS"].ToString() : "";
                            rmam.IDCOLLABORADOR_EXCLUSAO = reader["ID_COLLABORATOR_EXCLUIU"] != DBNull.Value ? reader["ID_COLLABORATOR_EXCLUIU"].ToString() : "";
                            rmam.NOME_EXCLUSAO = reader["NOME_EXCLUIU"] != DBNull.Value ? reader["NOME_EXCLUIU"].ToString() : "";
                            rmam.SETOR_EXCLUSAO = reader["SETOR_EXCLUIU"] != DBNull.Value ? reader["SETOR_EXCLUIU"].ToString() : "";
                            rmam.CLIENTE = reader["CLIENTE"] != DBNull.Value ? reader["CLIENTE"].ToString() : "";
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