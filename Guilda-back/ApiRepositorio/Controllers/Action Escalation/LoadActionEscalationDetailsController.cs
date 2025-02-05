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
using static ApiRepositorio.Controllers.LoadActionEscalationDetailsController;
using Microsoft.Extensions.Primitives;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class LoadActionEscalationDetailsController : ApiController
    {// POST: api/Results

        public class ReturnModelModelActionEscalationDetails
        {
            public int IDGDA_ESCALATION_ACTION { get; set; }
            public int AUTOMATIC { get; set; }
            public int IDGDA_INDICATOR { get; set; }
            public int IDGDA_PERSONA_RESPONSIBLE_CREATION { get; set; }
            public int IDGDA_PERSONA_RESPONSIBLE_ACTION { get; set; }
            public int IDGDA_SECTOR { get; set; }
            public int IDGDA_SUBSECTOR { get; set; }
            public int IDGDA_GROUP { get; set; }
            public int PERCENT { get; set; }
            public double TOLERANCE { get; set; }
            public string ACTION_REALIZED { get; set; }
            public string NAME { get; set; }
            public string DESCRIPTION { get; set; }
            public DateTime? STARTED_AT { get; set; }
            public DateTime? ENDED_AT { get; set; }
            public int ID_STAGE { get; set; }
            public int NUMBER_STAGE { get; set; }
            public int ID_HIERARCHY { get; set; }
            public string HIERARCHY { get; set; }



        }

        public class ColaboratorsScalation
        {
            public int ID { get; set; }
            public string NAME { get; set; }
        }

        public class ReturnModelActionEscalationDetails
        {
            public int IDGDA_ESCALATION_ACTION { get; set; }
            public int AUTOMATIC { get; set; }
            public int IDGDA_INDICATOR { get; set; }
            public int IDGDA_PERSONA_RESPONSIBLE_CREATION { get; set; }
            public int IDGDA_PERSONA_RESPONSIBLE_ACTION { get; set; }
            public int IDGDA_SECTOR { get; set; }
            public int IDGDA_SUBSECTOR { get; set; }
            public int IDGDA_GROUP { get; set; }
            public int PERCENT { get; set; }
            public double TOLERANCE { get; set; }
            public string ACTION_REALIZED { get; set; }
            public string NAME { get; set; }
            public string DESCRIPTION { get; set; }
            public DateTime? STARTED_AT { get; set; }
            public DateTime? ENDED_AT { get; set; }
            public List<listEscalationDetailsAction> STAGES { get; set; }

            public List<ColaboratorsScalation> COLLABORATORS { get; set; }
        }

        public class listEscalationDetailsAction
        {
            public int ID_STAGE { get; set; }
            public int NUMBER_STAGE { get; set; }
            public int ID_HIERARCHY { get; set; }
            public string HIERARCHY { get; set; }
        }

        [HttpGet]
        public IHttpActionResult GetResultsModel(int idAction, int automatic)
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

            ReturnModelActionEscalationDetails returnActionEscalation = new ReturnModelActionEscalationDetails();

            returnActionEscalation = BancoLoadActionEscalationDetails.returnActionEscalationDetail(idAction, automatic);

            return Ok(returnActionEscalation);
        }
        // Método para serializar um DataTable em JSON


    }

    public class BancoLoadActionEscalationDetails
    {

        public static ReturnModelActionEscalationDetails returnActionEscalationDetail(int idAction, int automatic)
        {
            ReturnModelActionEscalationDetails returnAg = new ReturnModelActionEscalationDetails();

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {

                    StringBuilder sb = new StringBuilder();

                    if (automatic == 1)
                    {
                        sb.Append("SELECT   ");
                        sb.Append("   EA.IDGDA_ESCALATION_AUTOMATIC_SECTORS AS IDGDA_ESCALATION_ACTION,  ");
                        sb.Append("   '0' AS IDGDA_PERSONA_RESPONSIBLE ,  ");
                        sb.Append("   '0' AS IDGDA_PERSONA_ACTION ,  ");
                        sb.Append("   '' AS NAME ,  ");
                        sb.Append("   MAX(IDGDA_INDICATOR) AS IDGDA_INDICATOR,  ");
                        sb.Append("   MAX(IDGDA_SECTOR) AS IDGDA_SECTOR,  ");
                        sb.Append("   MAX(IDGDA_SUBSECTOR) AS IDGDA_SUBSECTOR,  ");
                        sb.Append("   '' AS DESCRIPTION,  ");
                        sb.Append("   NULL AS DATA_INICIO, ");
                        sb.Append("   NULL AS DATA_FIM, ");
                        sb.Append("   STRING_AGG(CONCAT(PU.NAME, ':', AR.ACTION_REALIZE), ' # ') AS DESCRIPTION_COMPLETE, ");

                        sb.Append("MAX(EAS.IDGDA_HIERARCHY) AS IDGDA_HIERARCHY, ");
                        sb.Append("MAX(HY.LEVELNAME) AS HIERARCHY, ");
                        sb.Append("MAX(EAS.NUMBER_STAGE) AS NUMBER_STAGE, ");
                        sb.Append("IDGDA_ESCALATION_ACTION_STAGE, ");

                        sb.Append("MAX(EA.IDGDA_GROUP) AS IDGDA_GROUP, ");
                        sb.Append("MAX(EA.PERCENTAGE_DETOUR) AS PERCENTAGE_DETOUR, ");
                        sb.Append("MAX(EA.TOLERANCE_RANGE) AS TOLERANCE_RANGE ");


                        sb.Append("FROM GDA_ESCALATION_AUTOMATIC_SECTORS (NOLOCK) EA  ");
                        sb.Append("LEFT JOIN GDA_ESCALATION_HISTORY_ACTION_REALIZE (NOLOCK) AR ON AR.IDGDA_ESCALATION_ACTION = EA.IDGDA_ESCALATION_AUTOMATIC_SECTORS ");
                        sb.Append("LEFT JOIN GDA_PERSONA_USER (NOLOCK) AS PU ON PU.IDGDA_PERSONA_USER = AR.CREATED_BY ");

                        sb.Append("LEFT JOIN GDA_ESCALATION_ACTION_STAGE (NOLOCK) AS EAS ON EAS.IDGDA_ESCALATION_AUTOMATIC_SECTORS = EA.IDGDA_ESCALATION_AUTOMATIC_SECTORS ");
                        sb.Append("LEFT JOIN GDA_HIERARCHY (NOLOCK) AS HY ON EAS.IDGDA_HIERARCHY = HY.IDGDA_HIERARCHY ");

                        sb.Append("WHERE EA.DELETED_AT IS NULL ");
                        sb.Append($"AND EA.IDGDA_ESCALATION_AUTOMATIC_SECTORS = {idAction} ");
                        sb.Append("GROUP BY EA.IDGDA_ESCALATION_AUTOMATIC_SECTORS, EAS.IDGDA_ESCALATION_ACTION_STAGE, EAS.NUMBER_STAGE ");
                        sb.Append("ORDER BY EAS.NUMBER_STAGE ");
                    }
                    else
                    {
                        sb.Append("SELECT  ");
                        sb.Append("   EA.IDGDA_ESCALATION_ACTION, ");
                        sb.Append("   MAX(IDGDA_PERSONA_RESPONSIBLE) AS IDGDA_PERSONA_RESPONSIBLE ,  ");
                        sb.Append("   MAX(IDGDA_PERSONA_ACTION) AS IDGDA_PERSONA_ACTION ,  ");
                        sb.Append("	  MAX(EA.NAME) AS NAME, ");
                        sb.Append("   MAX(IDGDA_INDICATOR) AS IDGDA_INDICATOR,  ");
                        sb.Append("   MAX(IDGDA_SECTOR) AS IDGDA_SECTOR,  ");
                        sb.Append("   MAX(IDGDA_SUBSECTOR) AS IDGDA_SUBSECTOR,  ");
                        sb.Append("   MAX(DESCRIPTION) AS DESCRIPTION,  ");
                        sb.Append("	  MAX(EA.STARTED_AT) AS DATA_INICIO, ");
                        sb.Append("	  MAX(EA.ENDED_AT) AS DATA_FIM, ");
                        sb.Append("   STRING_AGG(CONCAT(PU.NAME, ':', AR.ACTION_REALIZE), ' # ') AS DESCRIPTION_COMPLETE, ");

                        sb.Append("MAX(EAS.IDGDA_HIERARCHY) AS IDGDA_HIERARCHY, ");
                        sb.Append("MAX(HY.LEVELNAME) AS HIERARCHY, ");
                        sb.Append("MAX(EAS.NUMBER_STAGE) AS NUMBER_STAGE, ");
                        sb.Append("IDGDA_ESCALATION_ACTION_STAGE, ");

                        sb.Append("'' AS IDGDA_GROUP, ");
                        sb.Append("'' AS PERCENTAGE_DETOUR, ");
                        sb.Append("'' AS TOLERANCE_RANGE ");

                        sb.Append("FROM GDA_ESCALATION_ACTION (NOLOCK) EA ");
                        sb.Append("LEFT JOIN GDA_ESCALATION_HISTORY_ACTION_REALIZE (NOLOCK) AR ON AR.IDGDA_ESCALATION_ACTION = EA.IDGDA_ESCALATION_ACTION ");
                        sb.Append("LEFT JOIN GDA_PERSONA_USER (NOLOCK) AS PU ON PU.IDGDA_PERSONA_USER = AR.CREATED_BY ");

                        sb.Append("LEFT JOIN GDA_ESCALATION_ACTION_STAGE (NOLOCK) AS EAS ON EAS.IDGDA_ESCALATION_ACTION = EA.IDGDA_ESCALATION_ACTION AND EAS.DELETED_AT IS NULL ");
                        sb.Append("LEFT JOIN GDA_HIERARCHY (NOLOCK) AS HY ON EAS.IDGDA_HIERARCHY = HY.IDGDA_HIERARCHY ");

                        sb.Append("WHERE  1=1 AND EA.DELETED_AT IS NULL ");
                        sb.Append($"AND EA.IDGDA_ESCALATION_ACTION = {idAction} ");
                        sb.Append("GROUP BY EA.IDGDA_ESCALATION_ACTION, EAS.IDGDA_ESCALATION_ACTION_STAGE, EAS.NUMBER_STAGE ");
                        sb.Append("ORDER BY EAS.NUMBER_STAGE ");
                    }


                    List<ReturnModelModelActionEscalationDetails> returnActionEscalations = new List<ReturnModelModelActionEscalationDetails>();

                    using (SqlCommand commandInsert = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ReturnModelModelActionEscalationDetails returnActionEscalation = new ReturnModelModelActionEscalationDetails();

                                returnActionEscalation.IDGDA_ESCALATION_ACTION = reader["IDGDA_ESCALATION_ACTION"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_ESCALATION_ACTION"]) : 0;
                                returnActionEscalation.AUTOMATIC = automatic;


                                returnActionEscalation.IDGDA_INDICATOR = reader["IDGDA_INDICATOR"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_INDICATOR"]) : 0;
                                returnActionEscalation.IDGDA_PERSONA_RESPONSIBLE_CREATION = reader["IDGDA_PERSONA_RESPONSIBLE"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_PERSONA_RESPONSIBLE"]) : 0;
                                returnActionEscalation.IDGDA_PERSONA_RESPONSIBLE_ACTION = reader["IDGDA_PERSONA_ACTION"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_PERSONA_ACTION"]) : 0;
                                returnActionEscalation.IDGDA_SECTOR = reader["IDGDA_SECTOR"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_SECTOR"]) : 0;
                                returnActionEscalation.IDGDA_SUBSECTOR = reader["IDGDA_SUBSECTOR"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_SUBSECTOR"]) : 0;
                                returnActionEscalation.ACTION_REALIZED = reader["DESCRIPTION_COMPLETE"] != DBNull.Value ? reader["DESCRIPTION_COMPLETE"].ToString() : "";
                                returnActionEscalation.NAME = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "";
                                returnActionEscalation.DESCRIPTION = reader["DESCRIPTION"] != DBNull.Value ? reader["DESCRIPTION"].ToString() : "";
                                returnActionEscalation.STARTED_AT = reader["DATA_INICIO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_INICIO"]) : (DateTime?)null;
                                returnActionEscalation.ENDED_AT = reader["DATA_FIM"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_FIM"]) : (DateTime?)null;

                                returnActionEscalation.ID_STAGE = reader["IDGDA_ESCALATION_ACTION_STAGE"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_ESCALATION_ACTION_STAGE"]) : 0;
                                returnActionEscalation.NUMBER_STAGE = reader["NUMBER_STAGE"] != DBNull.Value ? Convert.ToInt32(reader["NUMBER_STAGE"]) : 0;
                                returnActionEscalation.ID_HIERARCHY = reader["IDGDA_HIERARCHY"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_HIERARCHY"]) : 0;
                                returnActionEscalation.HIERARCHY = reader["HIERARCHY"] != DBNull.Value ? reader["HIERARCHY"].ToString() : "";
                                returnActionEscalation.IDGDA_GROUP = reader["IDGDA_GROUP"].ToString() != "" ? Convert.ToInt32(reader["IDGDA_GROUP"]) : 0;
                                returnActionEscalation.PERCENT = reader["PERCENTAGE_DETOUR"].ToString() != "" ? Convert.ToInt32(reader["PERCENTAGE_DETOUR"]) : 0;
                                returnActionEscalation.TOLERANCE = reader["TOLERANCE_RANGE"].ToString() != "" ? Convert.ToDouble(reader["TOLERANCE_RANGE"]) : 0;


                                returnActionEscalations.Add(returnActionEscalation);
                            }
                        }
                    }

                    List<ColaboratorsScalation> COLABS = new List<ColaboratorsScalation>();
                    sb.Clear();
                    sb.Append("SELECT CD.IDGDA_COLLABORATORS, MAX(C.NAME) AS NAME FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CD ");
                    sb.Append("INNER JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = CD.IDGDA_COLLABORATORS ");
                    sb.Append($"WHERE CD.CREATED_AT >= '{returnActionEscalations.First().STARTED_AT}' ");
                    if (returnActionEscalations.First().IDGDA_SECTOR != 0)
                    {
                        sb.Append($"AND IDGDA_SECTOR = '{returnActionEscalations.First().IDGDA_SECTOR}' ");
                    }

                    if (returnActionEscalations.First().IDGDA_SUBSECTOR != 0)
                    {
                        sb.Append($"AND IDGDA_SUBSECTOR = '{returnActionEscalations.First().IDGDA_SUBSECTOR}' ");
                    }
                    sb.Append("GROUP BY CD.IDGDA_COLLABORATORS ");

                    using (SqlCommand commandInsert = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ColaboratorsScalation COLAB = new ColaboratorsScalation();
                                COLAB.ID = Convert.ToInt32(reader["IDGDA_COLLABORATORS"].ToString());
                                COLAB.NAME = reader["NAME"].ToString();

                                COLABS.Add(COLAB);
                            }
                        }
                    }


                    returnAg = returnActionEscalations.GroupBy(item => new { item.IDGDA_ESCALATION_ACTION })
                   .Select(grupo => new ReturnModelActionEscalationDetails
                   {
                       IDGDA_ESCALATION_ACTION = grupo.Key.IDGDA_ESCALATION_ACTION,
                       AUTOMATIC = grupo.First().AUTOMATIC,

                       IDGDA_INDICATOR = grupo.First().IDGDA_INDICATOR,

                       IDGDA_PERSONA_RESPONSIBLE_CREATION = grupo.First().IDGDA_PERSONA_RESPONSIBLE_CREATION,
                       IDGDA_PERSONA_RESPONSIBLE_ACTION = grupo.First().IDGDA_PERSONA_RESPONSIBLE_ACTION,
                       IDGDA_SECTOR = grupo.First().IDGDA_SECTOR,
                       IDGDA_SUBSECTOR = grupo.First().IDGDA_SUBSECTOR,
                       ACTION_REALIZED = grupo.First().ACTION_REALIZED,
                       NAME = grupo.First().NAME,
                       DESCRIPTION = grupo.First().DESCRIPTION,
                       STARTED_AT = grupo.First().STARTED_AT,
                       ENDED_AT = grupo.First().ENDED_AT,
                       IDGDA_GROUP = grupo.First().IDGDA_GROUP,
                       PERCENT = grupo.First().PERCENT,
                       TOLERANCE = grupo.First().TOLERANCE,
                       STAGES = grupo.Select(r => new listEscalationDetailsAction
                       {
                           ID_STAGE = r.ID_STAGE,
                           NUMBER_STAGE = r.NUMBER_STAGE,
                           ID_HIERARCHY = r.ID_HIERARCHY,
                           HIERARCHY = r.HIERARCHY
                       }).ToList(),
                       COLLABORATORS = COLABS,
                   }).First();
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return returnAg;
        }

    }
}
