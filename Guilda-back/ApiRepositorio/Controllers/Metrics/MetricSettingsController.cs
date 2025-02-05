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
using Azure.Storage.Blobs.Specialized;
using static ApiRepositorio.Controllers.BuyItemController;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using DocumentFormat.OpenXml.Math;
using System.Drawing;
using System.Threading;
using System.Windows.Interop;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{

    //[Authorize]
    public class MetricSettingsController : ApiController
    {// POST: api/Results

        public class MetricSettingsModel
        {
            public string goal { get; set; }
            public List<MetricSettingsGroupsModel> groups { get; set; }
        }

        public class MetricSettingsGroupsModel
        {
            public int id { get; set; }
            public string name { get; set; }
            public string alias { get; set; }
            public int uploadId { get; set; }
            public string description { get; set; }
            public int order { get; set; }
            public string createdAt { get; set; }
            public MetricModel metrics { get; set; }
        }

        public class MetricModel
        {
            public int id { get; set; }
            public int indicatorId { get; set; }
            public int sectorId { get; set; }
            public int metricMin { get; set; }
            public int monetization { get; set; }
            public int groupId { get; set; }
            public string ended_at { get; set; }
            public string started_at { get; set; }
            public string createdAt { get; set; }
            public string deletedAt { get; set; }
        }

        public class inputMetricSettings
        {
            public int sectorId { get; set; }
            public List<int> indicatorsIds { get; set; }
            public List<metricsSetting> metricSettings { get; set; }
            public string period { get; set; }

        }

        public class metricsSetting
        {
            public int groupId { get; set; }
            public int monetization { get; set; }
            public int metricMin { get; set; }
            public string goal { get; set; }
        }

        public class indicatorGroup
        {
            public int id { get; set; }
            public int groupId { get; set; }
            public string monetization { get; set; }
            public string monetization_night { get; set; }
            public string monetization_latenight { get; set; }
            public string metric_min { get; set; }
            public string metric_min_night { get; set; }
            public string metric_min_latenight { get; set; }
        }
        public class indicatorSector
        {
            public int id { get; set; }
            public string goal { get; set; }
            public string goal_night { get; set; }
            public string goal_latenight { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] inputMetricSettings inputModel)
        {
            try
            {

                int collaboratorId = 0;
                int personauserId = 0;
                var token = Request.Headers.Authorization?.Parameter;

                InfsUser inf = TokenService.TryDecodeToken(token);
                if (inf == null)
                {
                    return Unauthorized();
                }
                collaboratorId = inf.collaboratorId;
                personauserId = inf.personauserId;

                BancoMetricSettings.updateMetricSettings(inputModel, collaboratorId);

                return Ok("Ok");
            }
            catch (Exception)
            {
                return BadRequest("Não foi possivel realizar a coniguração!");

            }
        }

        [HttpGet]
        public IHttpActionResult GetResultsModel(int sectorId, int indicatorId, string period)
        {

            int collaboratorId = 0;
            int personauserId = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            collaboratorId = inf.collaboratorId;
            personauserId = inf.personauserId;

            if (sectorId == 0 || indicatorId == 0 || period == "null")
            {
                return BadRequest("Informe todos os campos!");
            }

            MetricSettingsModel rmams = new MetricSettingsModel();
            rmams = BancoMetricSettings.returnMetricSettings(sectorId, indicatorId, period);

            return Ok(rmams);
        }

        public class BancoMetricSettings
        {


            public static MetricSettingsModel returnMetricSettings(int sectorId, int indicatorId, string period)
            {
                MetricSettingsModel msm = new MetricSettingsModel();
                msm.groups = new List<MetricSettingsGroupsModel>();

                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("SELECT ");
                stb.AppendFormat("GOAL, ");
                stb.AppendFormat("GOAL_NIGHT, ");
                stb.AppendFormat("GOAL_LATENIGHT, ");
                stb.AppendFormat("IG.GROUPID, ");
                stb.AppendFormat("G.NAME, ");
                stb.AppendFormat("G.ALIAS, ");
                stb.AppendFormat("G.UPLOADID, ");
                stb.AppendFormat("G.[DESCRIPTION] AS DESCRIP, ");
                stb.AppendFormat("G.[ORDER] AS ORDEM, ");
                stb.AppendFormat("G.CREATED_AT AS GCREATED_AT, ");
                stb.AppendFormat("IG.ID, ");
                stb.AppendFormat("IG.INDICATOR_ID, ");
                stb.AppendFormat("IG.SECTOR_ID, ");
                stb.AppendFormat("IG.METRIC_MIN, ");
                stb.AppendFormat("IG.MONETIZATION, ");
                stb.AppendFormat("IG.GROUPID, ");
                stb.AppendFormat("IG.METRIC_MIN_NIGHT, ");
                stb.AppendFormat("IG.METRIC_MIN_LATENIGHT, ");
                stb.AppendFormat("IG.MONETIZATION_NIGHT, ");
                stb.AppendFormat("IG.MONETIZATION_LATENIGHT, ");
                stb.AppendFormat("IG.STARTED_AT, ");
                stb.AppendFormat("IG.ENDED_AT, ");
                stb.AppendFormat("IG.CREATED_AT, ");
                stb.AppendFormat("IG.DELETED_AT ");
                stb.AppendFormat("FROM GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS IG ");
                stb.AppendFormat("INNER JOIN GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) AS HIS ON  ");
                stb.AppendFormat("		HIS.DELETED_AT IS NULL  ");
                stb.AppendFormat($"	AND HIS.STARTED_AT <= CONVERT(DATE, GETDATE()) ");
                stb.AppendFormat($"	AND HIS.ENDED_AT >= CONVERT(DATE, GETDATE()) ");
                stb.AppendFormat("	AND HIS.SECTOR_ID = IG.SECTOR_ID  ");
                stb.AppendFormat("	AND HIS.INDICATOR_ID = IG.INDICATOR_ID ");
                stb.AppendFormat("INNER JOIN GDA_GROUPS (NOLOCK) G ON G.ID = IG.GROUPID ");
                stb.AppendFormat($"WHERE IG.STARTED_AT <= CONVERT(DATE, GETDATE()) ");
                stb.AppendFormat($"AND IG.ENDED_AT >= CONVERT(DATE, GETDATE()) ");
                stb.AppendFormat("AND IG.DELETED_AT IS NULL ");
                stb.AppendFormat($"AND IG.SECTOR_ID = {sectorId}  ");
                stb.AppendFormat($"AND IG.INDICATOR_ID = {indicatorId} ");

                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {

                                    MetricSettingsGroupsModel msgm = new MetricSettingsGroupsModel();
                                    msgm.metrics = new MetricModel();

                                    if (period == "Noturno")
                                    {
                                        msm.goal = reader["GOAL_NIGHT"].ToString().Replace(",", ".");
                                        msgm.metrics.metricMin = Convert.ToInt32(reader["METRIC_MIN_NIGHT"].ToString());
                                        msgm.metrics.monetization = Convert.ToInt32(reader["MONETIZATION_NIGHT"].ToString());
                                    }
                                    else if (period == "Madrugada")
                                    {
                                        msm.goal = reader["GOAL_LATENIGHT"].ToString().Replace(",", ".");
                                        msgm.metrics.metricMin = Convert.ToInt32(reader["METRIC_MIN_LATENIGHT"].ToString());
                                        msgm.metrics.monetization = Convert.ToInt32(reader["MONETIZATION_LATENIGHT"].ToString());
                                    }
                                    else
                                    {
                                        msm.goal = reader["GOAL"].ToString().Replace(",", ".");
                                        msgm.metrics.metricMin = Convert.ToInt32(reader["METRIC_MIN"].ToString());
                                        msgm.metrics.monetization = Convert.ToInt32(reader["MONETIZATION"].ToString());
                                    }

                                    msgm.id = Convert.ToInt32(reader["GROUPID"].ToString());
                                    msgm.name = reader["NAME"].ToString();
                                    msgm.alias = reader["ALIAS"].ToString();
                                    msgm.uploadId = Convert.ToInt32(reader["UPLOADID"].ToString());
                                    msgm.description = reader["DESCRIP"].ToString();
                                    msgm.order = Convert.ToInt32(reader["ORDEM"].ToString());
                                    msgm.createdAt = reader["GCREATED_AT"].ToString();
                                    msgm.metrics.id = Convert.ToInt32(reader["ID"].ToString());
                                    msgm.metrics.indicatorId = Convert.ToInt32(reader["INDICATOR_ID"].ToString());
                                    msgm.metrics.sectorId = Convert.ToInt32(reader["SECTOR_ID"].ToString());
                                    msgm.metrics.groupId = Convert.ToInt32(reader["GROUPID"].ToString());
                                    //mm.metricMinNight = Convert.ToInt32(reader["AMOUNT_BUY"].ToString());
                                    //mm.metricMinLatenight = Convert.ToInt32(reader["AMOUNT_BUY"].ToString());
                                    //mm.monetizationNight = Convert.ToInt32(reader["AMOUNT_BUY"].ToString());
                                    //mm.monetizationLatenight = Convert.ToInt32(reader["AMOUNT_BUY"].ToString());
                                    msgm.metrics.ended_at = reader["ENDED_AT"].ToString();
                                    msgm.metrics.started_at = reader["STARTED_AT"].ToString();
                                    msgm.metrics.createdAt = reader["CREATED_AT"].ToString();
                                    msgm.metrics.deletedAt = reader["DELETED_AT"].ToString();

                                    msm.groups.Add(msgm);

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }

                return msm;
            }

            public static bool updateMetricSettings(inputMetricSettings inputModel, int idCollaborator)
            {
                DateTime dataAtual = DateTime.Now;
                DateTime primeiroDiaDoMes = new DateTime(dataAtual.Year, dataAtual.Month, 1);
                string stringPrimeiroDia = primeiroDiaDoMes.ToString("yyyy-MM-dd");

                bool existsIndicator = false;
                bool existsLine = false;
  
                List<indicatorGroup> igs = new List<indicatorGroup>();
                indicatorSector iis = new indicatorSector();


                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("SELECT IG.ID AS ID1, HIS.ID AS ID2, GOAL, GOAL_NIGHT, GOAL_LATENIGHT, GROUPID, MONETIZATION, MONETIZATION_NIGHT, MONETIZATION_LATENIGHT, METRIC_MIN, METRIC_MIN_NIGHT, METRIC_MIN_LATENIGHT ");
                stb.AppendFormat("FROM GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS IG ");
                stb.AppendFormat("INNER JOIN GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) AS HIS ON  ");
                stb.AppendFormat("		HIS.DELETED_AT IS NULL  ");
                stb.AppendFormat($"	AND HIS.CREATED_AT >= '{stringPrimeiroDia}' ");
                stb.AppendFormat("	AND HIS.SECTOR_ID = IG.SECTOR_ID  ");
                stb.AppendFormat("	AND HIS.INDICATOR_ID = IG.INDICATOR_ID ");
                stb.AppendFormat("WHERE IG.DELETED_AT IS NULL  ");
                stb.AppendFormat($"AND IG.CREATED_AT >= '{stringPrimeiroDia}' ");
                stb.AppendFormat($"AND IG.INDICATOR_ID = {inputModel.indicatorsIds.First()} ");
                stb.AppendFormat($"AND IG.SECTOR_ID = {inputModel.sectorId} ");
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    existsLine = true;
                                    string rd = "";
                                    if (inputModel.period == "Madrugada")
                                    {
                                        rd = reader["GOAL_LATENIGHT"] != DBNull.Value ? reader["GOAL_LATENIGHT"].ToString() : "";
                                    }
                                    else if (inputModel.period == "Noturno")
                                    {
                                        rd = reader["GOAL_NIGHT"] != DBNull.Value ? reader["GOAL_NIGHT"].ToString() : "";
                                    }
                                    else
                                    {
                                        rd = reader["GOAL"] != DBNull.Value ? reader["GOAL"].ToString() : "";
                                    }

                                    indicatorGroup ig = new indicatorGroup();
                                    ig.id = Convert.ToInt32(reader["ID1"].ToString());
                                    ig.groupId = Convert.ToInt32(reader["GROUPID"].ToString());
                                    ig.monetization = reader["MONETIZATION"] != DBNull.Value ? reader["MONETIZATION"].ToString() : "NULL";
                                    ig.monetization_night = reader["MONETIZATION_NIGHT"] != DBNull.Value ? reader["MONETIZATION_NIGHT"].ToString() : "NULL";
                                    ig.monetization_latenight = reader["MONETIZATION_LATENIGHT"] != DBNull.Value ? reader["MONETIZATION_LATENIGHT"].ToString() : "NULL";
                                    ig.metric_min = reader["METRIC_MIN"] != DBNull.Value ? reader["METRIC_MIN"].ToString() : "NULL";
                                    ig.metric_min_night = reader["METRIC_MIN_NIGHT"] != DBNull.Value ? reader["METRIC_MIN_NIGHT"].ToString() : "NULL";
                                    ig.metric_min_latenight = reader["METRIC_MIN_LATENIGHT"] != DBNull.Value ? reader["METRIC_MIN_LATENIGHT"].ToString() : "NULL";

                                    igs.Add(ig)
;
                                    iis.id = Convert.ToInt32(reader["ID2"].ToString());
                                    iis.goal = reader["GOAL"] != DBNull.Value ? $"'{reader["GOAL"].ToString().Replace(",",".")}'" : "NULL";
                                    iis.goal_night = reader["GOAL_NIGHT"] != DBNull.Value ? $"'{reader["GOAL_NIGHT"].ToString().Replace(",", ".")}'" : "NULL";
                                    iis.goal_latenight = reader["GOAL_LATENIGHT"] != DBNull.Value ? $"'{reader["GOAL_LATENIGHT"].ToString().Replace(",", ".")}'" : "NULL";

                                    if (rd != "")
                                    {
                                        existsIndicator = true;
                                    }
                                }
                            }
                        }


                        if (existsIndicator)
                        {
                            //Delete
                            stb.Clear();
                            stb.AppendFormat("UPDATE GDA_HISTORY_INDICATOR_SECTORS SET ");
                            stb.AppendFormat("DELETED_AT = GETDATE() ");
                            stb.AppendFormat($"WHERE ID = {iis.id} ");

                            using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                            {
                                command.ExecuteReader();
                            }

                            foreach (indicatorGroup item in igs)
                            {
                                stb.Clear();
                                stb.AppendFormat("UPDATE GDA_HISTORY_INDICATOR_GROUP SET ");
                                stb.AppendFormat("DELETED_AT = GETDATE() ");
                                stb.AppendFormat($"WHERE ID = {item.id} ");

                                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                                {
                                    command.ExecuteReader();
                                }
                            }

                            insertMetrics(stringPrimeiroDia, idCollaborator, inputModel, igs, iis);

                        }
                        else if (existsLine == false)
                        {
                            insertMetrics(stringPrimeiroDia, idCollaborator, inputModel, igs, iis);
                        }
                        else
                        {
                            string ids = String.Join(",", igs.Select(g => g.id)); 

                            string setGoal = "";
                            foreach (metricsSetting item in inputModel.metricSettings)
                            {
                                string set = "";

                                if (inputModel.period == "Madrugada")
                                {
                                    set = $"METRIC_MIN_LATENIGHT = '{item.metricMin.ToString()}', ";
                                    set = $"{set} MONETIZATION_LATENIGHT = '{item.monetization.ToString()}' ";

                                    setGoal = $"GOAL_LATENIGHT = '{item.goal.ToString().Replace(",", ".")}' ";
                                }
                                else if (inputModel.period == "Noturno")
                                {
                                    set = $"METRIC_MIN_NIGHT = '{item.metricMin.ToString()}', ";
                                    set = $"{set} MONETIZATION_NIGHT = '{item.monetization.ToString()}' ";

                                    setGoal = $"GOAL_NIGHT = '{item.goal.ToString().Replace(",", ".")}' ";
                                }
                                else
                                {
                                    set = $"METRIC_MIN = '{item.metricMin.ToString()}', ";
                                    set = $"{set} MONETIZATION = '{item.monetization.ToString()}' ";

                                    setGoal = $"GOAL = '{item.goal.ToString().Replace(",", ".")}' ";
                                }

                                stb.Clear();
                                stb.AppendFormat("UPDATE GDA_HISTORY_INDICATOR_GROUP SET ");
                                stb.AppendFormat($"{set} ");
                                stb.AppendFormat($"WHERE ID IN ({ids}) AND GROUPID = {item.groupId} ");

                                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                                {
                                    command.ExecuteReader();
                                }
                            }

                            //Update
                            stb.Clear();
                            stb.AppendFormat("UPDATE GDA_HISTORY_INDICATOR_SECTORS SET ");
                            stb.AppendFormat($"{setGoal} ");
                            stb.AppendFormat($"WHERE ID = {iis.id} ");

                            using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                            {
                                command.ExecuteReader();
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }

                return true;
            }

            public static bool insertMetrics(string stringPrimeiroDia, int idCollaborator, inputMetricSettings inputModel, List<indicatorGroup> igs, indicatorSector iis)
            {
                DateTime dataAtual = DateTime.Now;

                // Obtém o último dia do mês
                int ultimoDia = DateTime.DaysInMonth(dataAtual.Year, dataAtual.Month);

                // Cria a data com o último dia do mês
                DateTime ultimoDiaDoMes = new DateTime(dataAtual.Year, dataAtual.Month, ultimoDia);
                string stringUltimoDia = ultimoDiaDoMes.ToString("yyyy-MM-dd");


                StringBuilder stb = new StringBuilder();

                string GOAL = "NULL";
                string GOAL_NIGHT = "NULL";
                string GOAL_LATENIGHT = "NULL";

                string MONETIZATION = "NULL";
                string MONETIZATION_NIGHT = "NULL";
                string MONETIZATION_LATENIGHT = "NULL";

                string METRIC_MIN = "NULL";
                string METRIC_MIN_NIGHT = "NULL";
                string METRIC_MIN_LATENIGHT = "NULL";

                foreach (metricsSetting item in inputModel.metricSettings)
                {
                    GOAL = "NULL";
                    GOAL_NIGHT = "NULL";
                    GOAL_LATENIGHT = "NULL";

                    MONETIZATION = "NULL";
                    MONETIZATION_NIGHT = "NULL";
                    MONETIZATION_LATENIGHT = "NULL";

                    METRIC_MIN = "NULL";
                    METRIC_MIN_NIGHT = "NULL";
                    METRIC_MIN_LATENIGHT = "NULL";

                    if (igs.Count > 0)
                    {
                        indicatorGroup ig = igs.Find(s => s.groupId == item.groupId);

                        GOAL = $"{iis.goal.Replace(",", ".")}";
                        GOAL_NIGHT = $"{iis.goal_night.Replace(",", ".")}";
                        GOAL_LATENIGHT = $"{iis.goal_latenight.Replace(",", ".")}";

                        MONETIZATION = ig.monetization;
                        MONETIZATION_NIGHT = ig.monetization_night;
                        MONETIZATION_LATENIGHT = ig.monetization_latenight;

                        METRIC_MIN = ig.metric_min;
                        METRIC_MIN_NIGHT = ig.metric_min_night;
                        METRIC_MIN_LATENIGHT = ig.metric_min_latenight;
                    }

                    if (inputModel.period == "Madrugada")
                    {
                        METRIC_MIN_LATENIGHT = item.metricMin.ToString();
                        MONETIZATION_LATENIGHT = item.monetization.ToString();
                        GOAL_LATENIGHT = $"'{item.goal.ToString().Replace(",", ".")}'";
                    }
                    else if (inputModel.period == "Noturno")
                    {
                        METRIC_MIN_NIGHT = item.metricMin.ToString();
                        MONETIZATION_NIGHT = item.monetization.ToString();
                        GOAL_NIGHT = $"'{item.goal.ToString().Replace(",", ".")}'";
                    }
                    else
                    {
                        METRIC_MIN = item.metricMin.ToString();
                        MONETIZATION = item.monetization.ToString();
                        GOAL = $"'{item.goal.ToString().Replace(",", ".")}'";
                    }

                    stb.Clear();
                    stb.AppendFormat("INSERT INTO GDA_HISTORY_INDICATOR_GROUP (INDICATOR_ID, SECTOR_ID, METRIC_MIN, MONETIZATION, GROUPID, CREATED_AT, DELETED_AT, ENDED_AT, STARTED_AT, METRIC_MIN_NIGHT, METRIC_MIN_LATENIGHT, MONETIZATION_NIGHT, MONETIZATION_LATENIGHT, SECTOR_ID_PARENT) ");
                    stb.AppendFormat("VALUES ( ");
                    stb.AppendFormat($"{inputModel.indicatorsIds.First()}, "); //INDICATOR_ID
                    stb.AppendFormat($"{inputModel.sectorId}, "); //SECTOR_ID
                    stb.AppendFormat($"{METRIC_MIN}, "); //METRIC_MIN
                    stb.AppendFormat($"{MONETIZATION}, "); //MONETIZATION
                    stb.AppendFormat($"{item.groupId}, "); //GROUPID
                    stb.AppendFormat($"GETDATE(), "); //CREATED_AT
                    stb.AppendFormat($"NULL,  "); //DELETED_AT
                    stb.AppendFormat($"'{stringUltimoDia}', "); //ENDED_AT
                    stb.AppendFormat($"'{stringPrimeiroDia}', "); //STARTED_AT
                    stb.AppendFormat($"{METRIC_MIN_NIGHT}, "); //METRIC_MIN_NIGHT
                    stb.AppendFormat($"{METRIC_MIN_LATENIGHT}, "); //METRIC_MIN_LATENIGHT
                    stb.AppendFormat($"{MONETIZATION_NIGHT}, "); //MONETIZATION_NIGHT
                    stb.AppendFormat($"{MONETIZATION_LATENIGHT}, "); //MONETIZATION_LATENIGHT
                    stb.AppendFormat($"NULL "); //SECTOR_ID_PARENT
                    stb.AppendFormat(") ");

                    using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                    {
                        connection.Open();
                        try
                        {
                            using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                        connection.Close();
                    }

                }

                stb.Clear();
                stb.AppendFormat("INSERT INTO GDA_HISTORY_INDICATOR_SECTORS (INDICATOR_ID, SECTOR_ID, CREATED_AT, DELETED_AT, GOAL, ACTIVE, ALTERED_BY, ENDED_AT, STARTED_AT, WEIGHT_BASKET, GOAL_NIGHT, GOAL_LATENIGHT, SECTOR_ID_PARENT, WEIGHT, WEIGHT_NIGHT, WEIGHT_LATENIGHT) ");
                stb.AppendFormat("VALUES ( ");
                stb.AppendFormat($"{inputModel.indicatorsIds.First()}, "); //INDICATOR_ID
                stb.AppendFormat($"{inputModel.sectorId}, "); //SECTOR_ID
                stb.AppendFormat($"GETDATE(), "); //CREATED_AT
                stb.AppendFormat($"NULL, "); //DELETED_AT
                stb.AppendFormat($"{GOAL}, "); //GOAL
                stb.AppendFormat($"1, "); //ACTIVE
                stb.AppendFormat($"{idCollaborator}, "); //ALTERED_BY
                stb.AppendFormat($"'{stringUltimoDia}', "); //ENDED_AT
                stb.AppendFormat($"'{stringPrimeiroDia}', "); //STARTED_AT
                stb.AppendFormat($"NULL, "); //WEIGHT_BASKET
                stb.AppendFormat($"{GOAL_NIGHT}, "); //GOAL_NIGHT
                stb.AppendFormat($"{GOAL_LATENIGHT}, "); //GOAL_LATENIGHT
                stb.AppendFormat($"NULL, "); //SECTOR_ID_PARENT
                stb.AppendFormat($"NULL, "); //WEIGHT
                stb.AppendFormat($"NULL, "); //WEIGHT_NIGHT
                stb.AppendFormat($"NULL "); //WEIGHT_LATENIGHT
                stb.AppendFormat(") ");

                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    connection.Close();
                }

                return true;
            }

        }

        // Método para serializar um DataTable em JSON
    }
}