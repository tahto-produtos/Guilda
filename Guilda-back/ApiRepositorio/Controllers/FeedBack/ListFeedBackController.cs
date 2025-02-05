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
using static ApiRepositorio.Controllers.LoadLibraryNotificationController;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ListFeedBackController : ApiController
    {// POST: api/Results

        public class ReturnFeedBack
        {
            public int totalpages { get; set; }
            public List<FeedBack> ListFeedBack { get; set; }

        }

        public class FeedBack
        {
            public int IDGDA_FEEDBACK_USER { get; set; }
            public string NAME { get; set; }
            public string STATUS { get; set; }
            public string DATA_RESPOSTA { get; set; }
            public string PROTOCOLO { get; set; }
            public string PENALIDADE { get; set; }
        }

        public class InputListFeedBack
        {
            public List<int> Hierarchy { get; set; }
            public List<int> Site { get; set; }
            public List<int> Sector { get; set; }
            public List<int> SubSector { get; set; }
            public List<int> Groups { get; set; }
            public List<int> Userid { get; set; }
            public int limit { get; set; }
            public int page { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputListFeedBack inputModel)
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

            //VERIFICA PERFIL ADMINISTRATIVO
            bool adm = Funcoes.retornaPermissao(collaboratorId.ToString());
            if (adm == true)
            {
                //COLOCA O ID DO CEO, POIS TERA A MESMA VISÃO
                collaboratorId = 756399;
            }

            ReturnFeedBack rmams = new ReturnFeedBack();

            rmams = BancoListFeedBack.listFeedBack(inputModel, collaboratorId);

            return Ok(rmams);

        }
        // Método para serializar um DataTable em JSON


        #region Banco

        public class BancoListFeedBack
        {
            public static string filterFeedBack(ListFeedBackController.InputListFeedBack inputModel)
            {
                string ret = "";

                try
                {
                    if (inputModel.Hierarchy.Count > 0)
                    {
                        string hierarchies = string.Join(", ", inputModel.Hierarchy);
                        ret = $"{ret} AND H.IDGDA_HIERARCHY IN ({hierarchies}) ";
                    }

                    if (inputModel.Site.Count > 0)
                    {
                        string sits = string.Join(", ", inputModel.Site);
                        ret = $"{ret} AND S.IDGDA_SITE IN ({sits}) ";
                    }

                    if (inputModel.Sector.Count > 0)
                    {
                        string sectors = string.Join(", ", inputModel.Sector);
                        ret = $"{ret} AND CDI2.IDGDA_SECTOR IN ({sectors}) ";
                    }

                    if (inputModel.Groups.Count > 0)
                    {
                        string groups = string.Join(", ", inputModel.Groups);
                        ret = $"{ret} AND CDI2.IDGDA_GROUP IN ({groups}) ";
                    }

                    if (inputModel.Userid.Count > 0)
                    {
                        string users = string.Join(", ", inputModel.Userid);
                        ret = $"{ret} AND PU.IDGDA_PERSONA_USER IN ('{users}') ";
                    }


                }
                catch (Exception)
                {

                }
                return ret;
            }

            public static ReturnFeedBack listFeedBack(ListFeedBackController.InputListFeedBack inputModel, int personaId)
            {
                ReturnFeedBack retorno = new ReturnFeedBack();
                List<FeedBack> ListFeedBack = new List<FeedBack>();

                string filter = filterFeedBack(inputModel);

                int totalInfo = Funcoes.QuantidadeFeedBack(filter, personaId);
                int totalpage = (int)Math.Ceiling((double)totalInfo / inputModel.limit);
                int offset = (inputModel.page - 1) * inputModel.limit;

                retorno.totalpages = totalpage;


                StringBuilder sb = new StringBuilder();

                sb.Append($"DECLARE @ID INT; SET @ID = '{personaId}'; ");
                sb.Append($"SELECT FU.IDGDA_FEEDBACK_USER AS IDGDA_FEEDBACK_USER, ");
                sb.Append($"       MAX(PU.NAME) AS NAME, ");
                sb.Append($"       MAX(CASE ");
                sb.Append($"               WHEN FU.SIGNED_RECEIVED IS NULL THEN 'Enviado' ");
                sb.Append($"               ELSE 'Assinado' ");
                sb.Append($"           END) AS STATUS, ");
                sb.Append($"       MAX(CASE ");
                sb.Append($"               WHEN FU.RESPONDED_AT IS NULL THEN '-' ");
                sb.Append($"               ELSE CONVERT(varchar,CONVERT(DATE,FU.RESPONDED_AT)) ");
                sb.Append($"           END) AS DATA_RESPOSTA, ");
                sb.Append($"       MAX(FU.PROTOCOL) AS PROTOCOLO, ");
                sb.Append($"       MAX(CASE ");
                sb.Append($"               WHEN CONVERT(DATE,FU.SUSPENDED_UNTIL) > CONVERT(DATE,GETDATE()) THEN 'Aplicada' ");
                sb.Append($"               WHEN FU.SIGNED_RECEIVED IS NULL ");
                sb.Append($"                    AND FU.SUSPENDED_UNTIL IS NULL THEN 'Aplicada' ");
                sb.Append($"               ELSE 'Cumprida' ");
                sb.Append($"           END) AS PENALIDADE, ");
                sb.Append($"       MAX(CDI2.IDGDA_COLLABORATORS) AS IDGDA_COLLABORATORS, ");
                sb.Append($"       MAX(H.IDGDA_HIERARCHY) AS IDGDA_HIERARCHY, ");
                sb.Append($"       MAX(S.IDGDA_SITE) AS IDGDA_SITE, ");
                sb.Append($"       MAX(CDI2.IDGDA_SECTOR) AS IDGDA_SECTOR ");
                sb.Append($" ");
                sb.Append($"FROM GDA_FEEDBACK (NOLOCK) F ");
                sb.Append($"LEFT JOIN GDA_FEEDBACK_USER (NOLOCK) AS FU ON FU.IDGDA_FEEDBACK = F.IDGDA_FEEDBACK ");
                sb.Append($"LEFT JOIN GDA_PERSONA_USER (NOLOCK) AS PU ON PU.IDGDA_PERSONA_USER = FU.IDPERSONA_RECEIVED_BY ");
                sb.Append($"LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCU2 ON PCU2.IDGDA_PERSONA_USER = FU.IDPERSONA_RECEIVED_BY ");
                sb.Append($"LEFT JOIN ");
                sb.Append($"  (SELECT CDI2.IDGDA_COLLABORATORS, ");
                sb.Append($"          CDI2.MATRICULA_SUPERVISOR, ");
                sb.Append($"          CDI2.MATRICULA_COORDENADOR, ");
                sb.Append($"          CDI2.MATRICULA_GERENTE_II, ");
                sb.Append($"          CDI2.MATRICULA_GERENTE_I, ");
                sb.Append($"          CDI2.MATRICULA_DIRETOR, ");
                sb.Append($"          CDI2.MATRICULA_CEO, ");
                sb.Append($"          CDI2.CARGO, ");
                sb.Append($"          CDI2.SITE, CDI2.IDGDA_GROUP, ");
                sb.Append($"          CASE ");
                sb.Append($"              WHEN CDI2.IDGDA_SECTOR IS NULL THEN CDI2.IDGDA_SUBSECTOR ");
                sb.Append($"              ELSE CDI2.IDGDA_SECTOR ");
                sb.Append($"          END AS IDGDA_SECTOR ");
                sb.Append($"   FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CDI2 ");
                sb.Append($"   WHERE CONVERT(DATE, CDI2.CREATED_AT) >= CONVERT(DATE, DATEADD(DAY, -5, GETDATE())) )AS CDI2 ON CDI2.IDGDA_COLLABORATORS = PCU2.IDGDA_COLLABORATORS ");
                sb.Append($"LEFT JOIN GDA_HIERARCHY (NOLOCK) AS H ON H.LEVELNAME = CDI2.CARGO ");
                sb.Append($"LEFT JOIN GDA_SITE (NOLOCK) AS S ON S.SITE = CDI2.SITE ");
                sb.Append($"LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = CDI2.IDGDA_SECTOR ");
                sb.Append($"WHERE IDGDA_FEEDBACK_USER IS NOT NULL ");

                sb.Append($"AND (CDI2.IDGDA_COLLABORATORS = @ID ");
                sb.Append($"OR MATRICULA_SUPERVISOR = @ID ");
                sb.Append($"OR MATRICULA_COORDENADOR = @ID ");
                sb.Append($"OR MATRICULA_GERENTE_II = @ID ");
                sb.Append($"OR MATRICULA_GERENTE_I = @ID ");
                sb.Append($"OR MATRICULA_CEO = @ID) ");

                sb.Append($" {filter} ");
                sb.Append($"GROUP BY FU.IDGDA_FEEDBACK_USER, F.CREATED_AT ");
                sb.Append($"ORDER BY F.CREATED_AT DESC ");
                sb.Append($"OFFSET {offset} ROWS FETCH NEXT {inputModel.limit} ROWS ONLY  ");




                //sb.Append($"DECLARE @ID INT; SET @ID = '{personaId}'; ");
                //sb.Append("SELECT  ");
                //sb.Append("	   FU.IDGDA_FEEDBACK_USER AS IDGDA_FEEDBACK_USER, ");
                //sb.Append("	   MAX(PU.NAME) AS NAME, ");
                //sb.Append("	   MAX(CASE WHEN FU.SIGNED_RECEIVED IS NULL THEN 'Enviado' ELSE 'Assinado' END) AS STATUS, ");
                //sb.Append("	   MAX(CASE WHEN FU.RESPONDED_AT IS NULL THEN  '-' ELSE CONVERT(varchar,CONVERT(DATE,FU.RESPONDED_AT)) END) AS DATA_RESPOSTA, ");
                //sb.Append("	   MAX(FU.PROTOCOL) AS PROTOCOLO, ");
                //sb.Append("	   MAX(CASE WHEN  CONVERT(DATE,FU.SUSPENDED_UNTIL) > CONVERT(DATE,GETDATE()) THEN 'Aplicada' ");
                //sb.Append("       WHEN FU.SIGNED_RECEIVED IS NULL AND FU.SUSPENDED_UNTIL IS NULL THEN 'Aplicada' ");
                //sb.Append("       ELSE 'Cumprida' END) AS PENALIDADE, ");
                //sb.Append("	   MAX(CDI2.IDGDA_COLLABORATORS) AS IDGDA_COLLABORATORS, ");
                //sb.Append("	   MAX(H.IDGDA_HIERARCHY) AS IDGDA_HIERARCHY, ");
                //sb.Append("	   MAX(S.IDGDA_SITE) AS IDGDA_SITE, ");
                //sb.Append("	   MAX(CDI2.IDGDA_SECTOR) AS IDGDA_SECTOR ");
                //sb.Append("FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CD  ");
                //sb.Append("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCU ON PCU.IDGDA_PERSONA_USER = @ID ");
                //sb.Append("LEFT JOIN GDA_FEEDBACK (NOLOCK) AS F ON F.CREATED_BY = PCU.IDGDA_PERSONA_USER ");
                //sb.Append("LEFT JOIN GDA_FEEDBACK_USER (NOLOCK) AS FU ON FU.IDGDA_FEEDBACK = F.IDGDA_FEEDBACK ");
                //sb.Append("LEFT JOIN GDA_PERSONA_USER (NOLOCK) AS PU ON PU.IDGDA_PERSONA_USER = FU.IDPERSONA_RECEIVED_BY ");
                //sb.Append("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCU2 ON PCU2.IDGDA_PERSONA_USER = FU.IDPERSONA_RECEIVED_BY ");
                //sb.Append("LEFT JOIN ( ");
                //sb.Append("            SELECT CDI2.IDGDA_COLLABORATORS, ");
                //sb.Append("			CDI2.MATRICULA_SUPERVISOR, ");
                //sb.Append("			CDI2.MATRICULA_COORDENADOR, ");
                //sb.Append("			CDI2.MATRICULA_GERENTE_II, ");
                //sb.Append("			CDI2.MATRICULA_GERENTE_I, ");
                //sb.Append("			CDI2.MATRICULA_DIRETOR,  ");
                //sb.Append("			CDI2.MATRICULA_CEO, ");
                //sb.Append("			CDI2.CARGO, ");
                //sb.Append("			CDI2.SITE, ");
                //sb.Append("			CASE WHEN CDI2.IDGDA_SECTOR IS NULL THEN CDI2.IDGDA_SUBSECTOR ELSE CDI2.IDGDA_SECTOR END AS IDGDA_SECTOR  ");
                //sb.Append("			FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CDI2  ");
                //sb.Append("			WHERE CONVERT(DATE, CDI2.CREATED_AT) >= CONVERT(DATE, DATEADD(DAY, -1, GETDATE())) ");
                //sb.Append("		   )AS CDI2 ON CDI2.IDGDA_COLLABORATORS = PCU2.IDGDA_COLLABORATORS ");
                //sb.Append("LEFT JOIN GDA_HIERARCHY (NOLOCK) AS H ON H.LEVELNAME = CDI2.CARGO ");
                //sb.Append("LEFT JOIN GDA_SITE (NOLOCK) AS S ON S.SITE = CDI2.SITE ");
                //sb.Append("LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = CDI2.IDGDA_SECTOR ");
                //sb.Append("WHERE CONVERT(DATE, CD.CREATED_AT) >= CONVERT(DATE, DATEADD(DAY, -1, GETDATE())) ");
                //sb.Append("AND IDGDA_FEEDBACK_USER IS NOT NULL ");
                //sb.Append("AND( ");
                //sb.Append(" CD.IDGDA_COLLABORATORS = PCU.IDGDA_COLLABORATORS OR   ");
                //sb.Append("	CD.MATRICULA_SUPERVISOR = PCU.IDGDA_COLLABORATORS OR   ");
                //sb.Append("	CD.MATRICULA_COORDENADOR = PCU.IDGDA_COLLABORATORS OR   ");
                //sb.Append("	CD.MATRICULA_GERENTE_II = PCU.IDGDA_COLLABORATORS OR   ");
                //sb.Append("	CD.MATRICULA_GERENTE_I = PCU.IDGDA_COLLABORATORS OR   ");
                //sb.Append("	CD.MATRICULA_DIRETOR = PCU.IDGDA_COLLABORATORS OR   ");
                //sb.Append("	CD.MATRICULA_CEO = PCU.IDGDA_COLLABORATORS ");
                //sb.Append("	) ");
                //sb.Append($" {filter} ");
                //sb.Append("GROUP BY FU.IDGDA_FEEDBACK_USER ");
                //sb.Append("ORDER BY DATA_RESPOSTA DESC  ");
                //sb.Append($"OFFSET {offset} ROWS FETCH NEXT {inputModel.limit} ROWS ONLY  ");
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
                                    FeedBack FeedBAck = new FeedBack();

                                    FeedBAck.IDGDA_FEEDBACK_USER = reader["IDGDA_FEEDBACK_USER"] != DBNull.Value ? int.Parse(reader["IDGDA_FEEDBACK_USER"].ToString()) : 0;
                                    FeedBAck.NAME = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "";
                                    FeedBAck.STATUS = reader["STATUS"] != DBNull.Value ? reader["STATUS"].ToString() : "";
                                    FeedBAck.DATA_RESPOSTA = reader["DATA_RESPOSTA"] != DBNull.Value ? reader["DATA_RESPOSTA"].ToString() : "";
                                    FeedBAck.PROTOCOLO = reader["PROTOCOLO"] != DBNull.Value ? reader["PROTOCOLO"].ToString() : "";
                                    FeedBAck.PENALIDADE = reader["PENALIDADE"] != DBNull.Value ? reader["PENALIDADE"].ToString() : "";
                                    ListFeedBack.Add(FeedBAck);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }
                retorno.ListFeedBack = ListFeedBack;
                return retorno;
            }
            #endregion

        }


    }
}