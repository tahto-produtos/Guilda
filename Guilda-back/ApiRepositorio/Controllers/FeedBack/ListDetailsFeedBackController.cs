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
using static ApiRepositorio.Controllers.CreatedNotificationController;
using Antlr.Runtime.Misc;
using static ApiRepositorio.Controllers.LoadMyNotificationController;
using OfficeOpenXml.ConditionalFormatting;
using static TokenService;

//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ListDetailsFeedBackController : ApiController
    {// POST: api/Results

        public class ReturnModelDetailsFeedback
        {
            public int idFeedbackUser { get; set; }
            public string sendFor { get; set; }
            public int sendForId { get; set; }
            public string sendForHierarchy { get; set; }
            public string title { get; set; }
            public string linkFile { get; set; }
            public List<returnModelDetailsFeedbackFiles> files { get; set; }
            public string protocol { get; set; }
            public string status { get; set; }
            public string nameInfraction { get; set; }
            public DateTime? createdAt { get; set; }
            public string content { get; set; }
        }

        public class returnModelDetailsFeedbackFiles
        {
            public string url { get; set; }
        }

        [HttpGet]
        public IHttpActionResult GetResultsModel(int idFeedbackUser)
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

            //INSERÇÃO DO FEEDBACK
            ReturnModelDetailsFeedback rmd = new ReturnModelDetailsFeedback();
            rmd = BancoDetailsFeedback.listDetailFeedback(idFeedbackUser);

            return Ok(rmd);

        }
        // Método para serializar um DataTable em JSON


        #region Banco

        public class BancoDetailsFeedback
        {

            public static ReturnModelDetailsFeedback listDetailFeedback(int idFeedbackUser)
            {
                List<ReturnModelDetailsFeedback> rmds = new List<ReturnModelDetailsFeedback>();

                StringBuilder sb = new StringBuilder();
                sb.Append($"SELECT  ");
                sb.Append($"IDGDA_FEEDBACK_USER,  ");
                sb.Append($"MAX(IDPERSONA_RECEIVED_BY) AS IDPERSONA_RECEIVED_BY,  ");
                sb.Append($"MAX(PU.NAME) AS NAME, ");
                sb.Append($"MAX(CD.CARGO) AS CARGO, ");
                sb.Append($"MAX(F.REASON) AS TITLE, ");
                sb.Append($"FF.FILES AS FILES, ");
                sb.Append($"MAX(FU.PROTOCOL) AS PROTOCOL, ");
                sb.Append($" ");
                sb.Append($"MAX(CASE WHEN  CONVERT(DATE,FU.SUSPENDED_UNTIL) > CONVERT(DATE,GETDATE()) THEN 'Aplicada'  ");
                sb.Append($"    WHEN FU.SIGNED_RECEIVED IS NULL AND FU.SUSPENDED_UNTIL IS NULL THEN 'Aplicada'  ");
                sb.Append($"    ELSE 'Cumprida' END) AS STATUS, ");
                sb.Append($"MAX(PS.PEDAGOGICAL_SCALE) AS NAME_INFRACTION, ");
                sb.Append($"MAX(FU.CREATED_AT) AS CREATED_AT, ");
                sb.Append($"MAX(F.DETAILS) AS DETAILS ");
                sb.Append($"FROM GDA_FEEDBACK_USER (NOLOCK) FU ");
                sb.Append($"LEFT JOIN GDA_PEDAGOGICAL_SCALE PS ON PS.IDGDA_PEDAGOGICAL_SCALE = FU.IDGDA_PEDAGOGICAL_SCALE ");
                sb.Append($"LEFT JOIN GDA_FEEDBACK (NOLOCK) F ON F.IDGDA_FEEDBACK = FU.IDGDA_FEEDBACK ");
                sb.Append($"LEFT JOIN GDA_FEEDBACK_FILES (NOLOCK) FF ON FF.IDGDA_FEEDBACK = FU.IDGDA_FEEDBACK ");
                sb.Append($"LEFT JOIN GDA_PERSONA_USER (NOLOCK) PU ON FU.IDPERSONA_RECEIVED_BY = PU.IDGDA_PERSONA_USER ");
                sb.Append($"LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) PCU ON PCU.IDGDA_PERSONA_USER = FU.IDPERSONA_RECEIVED_BY ");
                sb.Append($"LEFT JOIN  ");
                sb.Append($"  (SELECT CDI2.IDGDA_COLLABORATORS,  ");
                sb.Append($"          MAX(CDI2.MATRICULA_SUPERVISOR) AS MATRICULA_SUPERVISOR,  ");
                sb.Append($"          MAX(CDI2.MATRICULA_COORDENADOR) AS MATRICULA_COORDENADOR,  ");
                sb.Append($"          MAX(CDI2.MATRICULA_GERENTE_II) AS MATRICULA_GERENTE_II,  ");
                sb.Append($"          MAX(CDI2.MATRICULA_GERENTE_I) AS MATRICULA_GERENTE_I,  ");
                sb.Append($"          MAX(CDI2.MATRICULA_DIRETOR) AS MATRICULA_DIRETOR,  ");
                sb.Append($"          MAX(CDI2.MATRICULA_CEO) AS MATRICULA_CEO,  ");
                sb.Append($"          MAX(CDI2.CARGO) AS CARGO,  ");
                sb.Append($"          MAX(CDI2.SITE) AS SITE,  ");
                sb.Append($"          CASE  ");
                sb.Append($"              WHEN MAX(CDI2.IDGDA_SECTOR) IS NULL THEN MAX(CDI2.IDGDA_SUBSECTOR) ");
                sb.Append($"              ELSE MAX(CDI2.IDGDA_SECTOR)  ");
                sb.Append($"          END AS IDGDA_SECTOR  ");
                sb.Append($"   FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CDI2  ");
                sb.Append($"   WHERE CONVERT(DATE, CDI2.CREATED_AT) >= CONVERT(DATE, DATEADD(DAY, -5, GETDATE())) GROUP BY CDI2.IDGDA_COLLABORATORS ) AS CD ON CD.IDGDA_COLLABORATORS = PCU.IDGDA_COLLABORATORS ");
                //sb.Append($"LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) CD ON CD.IDGDA_COLLABORATORS = PCU.IDGDA_COLLABORATORS AND CD.CREATED_AT >= CONVERT(DATE, DATEADD(DAY, -5, GETDATE())) ");
                sb.Append($"WHERE IDGDA_FEEDBACK_USER = {idFeedbackUser} ");
                sb.Append($"GROUP BY IDGDA_FEEDBACK_USER, FF.FILES ");

                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    ReturnModelDetailsFeedback rmd = new ReturnModelDetailsFeedback();
                                    //int.Parse(reader["IDPERSONA_RECEIVED_BY"].ToString());
                                    rmd.idFeedbackUser = reader["IDGDA_FEEDBACK_USER"] != DBNull.Value ? int.Parse(reader["IDGDA_FEEDBACK_USER"].ToString()) : 0;
                                    rmd.sendFor = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "";
                                    rmd.sendForId = reader["IDPERSONA_RECEIVED_BY"] != DBNull.Value ? int.Parse(reader["IDPERSONA_RECEIVED_BY"].ToString()) : 0;
                                    rmd.sendForHierarchy = reader["CARGO"] != DBNull.Value ? reader["CARGO"].ToString() : "";
                                    rmd.title = reader["TITLE"] != DBNull.Value ? reader["TITLE"].ToString() : "";
                                    rmd.linkFile = reader["FILES"] != DBNull.Value ? reader["FILES"].ToString() : "";
                                    rmd.protocol = reader["PROTOCOL"] != DBNull.Value ? reader["PROTOCOL"].ToString() : "";
                                    rmd.status = reader["STATUS"] != DBNull.Value ? reader["STATUS"].ToString() : "";
                                    rmd.nameInfraction = reader["NAME_INFRACTION"] != DBNull.Value ? reader["NAME_INFRACTION"].ToString() : "";
                                    rmd.createdAt = reader["CREATED_AT"] != DBNull.Value ? Convert.ToDateTime(reader["CREATED_AT"].ToString()) : (DateTime?)null;
                                    rmd.content = reader["DETAILS"] != DBNull.Value ? reader["DETAILS"].ToString() : "";

                                    rmds.Add(rmd);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }


                ReturnModelDetailsFeedback rmdReturn = rmds.GroupBy(item => new { item.idFeedbackUser })
                    .Select(grupo => new ReturnModelDetailsFeedback
                    {
                        idFeedbackUser = grupo.Key.idFeedbackUser,
                        sendFor = grupo.First().sendFor,
                        sendForId = grupo.First().sendForId,
                        sendForHierarchy = grupo.First().sendForHierarchy,
                        title = grupo.First().title,
                        linkFile = "",
                        protocol = grupo.First().protocol,
                        status = grupo.First().status,
                        nameInfraction = grupo.First().nameInfraction,
                        createdAt = grupo.First().createdAt,
                        content = grupo.First().content,
                        files = grupo.First().linkFile != null && grupo.First().linkFile != "" ?
                            grupo.Select(r => new returnModelDetailsFeedbackFiles
                            {
                                url = r.linkFile
                            }).ToList() : new List<returnModelDetailsFeedbackFiles>()
                    }).First();


                return rmdReturn;
            }
        }
        #endregion

    }



}