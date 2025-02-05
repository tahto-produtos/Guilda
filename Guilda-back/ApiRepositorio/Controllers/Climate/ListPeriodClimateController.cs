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

    public class GroupedPeriodClimate
    {
        public string Period { get; set; }

        public List<returnListPeriodClimate> Climates { get; set; }

    }

    public class returnListPeriodClimate
    {
        public int idClimate { get; set; }
        public string climate { get; set; }
        public string image { get; set; }
        public int count { get; set; }
        public double percent { get; set; }
        public string data { get; set; }
    }

    public enum GroupingPeriod
    {
        Day,
        Week,
        Biweekly,
        Month
    }

    //[Authorize]
    public class ListPeriodClimateController : ApiController
    {// POST: api/Results
        [HttpGet]
        public IHttpActionResult PostResultsModel(DateTime STARTEDATFROM, DateTime STARTEDATTO, string PERIOD)
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

            List<GroupedPeriodClimate> rmams = new List<GroupedPeriodClimate>();
            rmams = BankListPeriodClimate.listReportHierarchyClimate(COLLABORATORID, STARTEDATFROM, STARTEDATTO, PERIOD);


            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(rmams);
        }
        // Método para serializar um DataTable em JSON


        #region funcoes

        public static List<returnListPeriodClimate> GetAllClimateGroups()
        {
            List<returnListPeriodClimate> lcg = new List<returnListPeriodClimate>();

            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT IDGDA_CLIMATE, CLIMATE, IMAGE FROM GDA_CLIMATE (NOLOCK) ");

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

                                returnListPeriodClimate lc = new returnListPeriodClimate();
                                lc.idClimate = reader["IDGDA_CLIMATE"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_CLIMATE"]) : 0;
                                lc.climate = reader["CLIMATE"] != DBNull.Value ? reader["CLIMATE"].ToString() : "";
                                lc.image = reader["IMAGE"] != DBNull.Value ? reader["IMAGE"].ToString() : "";
                                lc.count = 0;
                                lc.percent = 0;
                                lcg.Add(lc);
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

            return lcg;
        }

        public static List<GroupedPeriodClimate> GroupByPeriod(List<returnListPeriodClimate> climates, GroupingPeriod period)
        {
            List<returnListPeriodClimate> allClimateGroups = GetAllClimateGroups();

            List<GroupedPeriodClimate> groupByPeriod = climates
                .GroupBy(climate => GetPeriod(climate.data, period))
                .Select(group => new GroupedPeriodClimate
                {
                    Period = group.Key,
                    Climates = allClimateGroups
                                   .Select(climate => new returnListPeriodClimate
                                   {
                                       idClimate = climate.idClimate,
                                       climate = climate.climate,
                                       image = climate.image,
                                       count = group.Count(item => item.idClimate == climate.idClimate),
                                       percent = group.Count(item => item.idClimate != 0) > 0 ? Math.Round(((Convert.ToDouble(group.Count(item => item.idClimate == climate.idClimate)) / Convert.ToDouble(group.Count(item => item.idClimate != 0)) * 100)), 2, MidpointRounding.AwayFromZero) : 0

                                   }).ToList()

                    //Climates = group.ToList().GroupBy(item => item.idClimate)
                    //.Select(itens => new returnListPeriodClimate { 
                    //idClimate = itens.Key,
                    //    climate = itens.First().climate,
                    //    image = itens.First().image,
                    //    count = itens.Sum(i => i.count),
                    //    percent = group.Sum(i => i.count) > 0 ? ((itens.Sum(i => i.count) / group.Sum(i => i.count))*100) : 0
                    //}).ToList()
                })
                .ToList();

            //climates = allClimateGroups
            //               .Select(climate => new climateGroup
            //               {
            //                   id = climate.id,
            //                   name = climate.name,
            //                   count = group.Count(item => item.idClimate == climate.id),
            //                   percent = group.Count(item => item.idClimate != 0) > 0 ? Math.Round(((Convert.ToDouble(group.Count(item => item.idClimate == climate.id)) / Convert.ToDouble(group.Count(item => item.idClimate != 0)) * 100)), 2, MidpointRounding.AwayFromZero) : 0

            //               })



            return groupByPeriod;
        }

        private static string GetPeriod(string date, GroupingPeriod period)
        {
            DateTime dateTime = DateTime.Parse(date);

            switch (period)
            {
                case GroupingPeriod.Day:
                    return dateTime.ToString("yyyy-MM-dd");
                case GroupingPeriod.Week:
                    return $"{dateTime.Year}-W{GetIso8601WeekOfYear(dateTime)}";
                case GroupingPeriod.Biweekly:
                    return $"{dateTime.Year}-BW{GetIso8601BiweekOfYear(dateTime)}";
                case GroupingPeriod.Month:
                    return dateTime.ToString("yyyy-MM");
                default:
                    throw new ArgumentException("Invalid grouping period");
            }
        }

        private static int GetIso8601WeekOfYear(DateTime date)
        {
            return System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        private static int GetIso8601BiweekOfYear(DateTime date)
        {
            return (GetIso8601WeekOfYear(date) + 1) / 2;
        }
        #endregion

    }

    public class BankListPeriodClimate
    {
        public static List<GroupedPeriodClimate> listReportHierarchyClimate(int idCollaborator, DateTime STARTEDATFROM, DateTime STARTEDATTO, string PERIOD)
        {

            List<returnListPeriodClimate> retorno = new List<returnListPeriodClimate>();


            string dtInicial = STARTEDATFROM.ToString("yyyy-MM-dd");
            string dtFinal = STARTEDATTO.ToString("yyyy-MM-dd");

            StringBuilder sb = new StringBuilder();
            sb.Append($"DECLARE @ID INT; SET @ID = '{idCollaborator}'; ");
            sb.Append($"DECLARE @DATE_INI DATE; SET @DATE_INI = '{dtInicial}'; ");
            sb.Append($"DECLARE @DATE_FIM DATE; SET @DATE_FIM = '{dtFinal}'; ");
            sb.Append($"SELECT GC.IDGDA_CLIMATE, GC.CLIMATE, GC.IMAGE, GCU.DATA, ISNULL(GCU_COUNT, 0) AS COUNT ");
            sb.Append($"FROM GDA_CLIMATE GC ");
            sb.Append($"LEFT JOIN ( ");
            sb.Append($"    SELECT IDGDA_CLIMATE, COUNT(*) AS GCU_COUNT, CONVERT(DATE, GDU.CREATED_AT) AS DATA ");
            sb.Append($"    FROM GDA_CLIMATE_USER (NOLOCK) GDU ");
            sb.Append($"	LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCU ON PCU.IDGDA_PERSONA_USER = GDU.IDGDA_PERSONA  ");
            sb.Append($"	LEFT JOIN ( ");
            sb.Append($"		SELECT * FROM GDA_COLLABORATORS_DETAILS (NOLOCK) ");
            sb.Append($"		WHERE CREATED_AT >= CONVERT(DATE, DATEADD(DAY, -1, GETDATE())) ");
            sb.Append($"	) CD ON CD.IDGDA_COLLABORATORS = PCU.IDGDA_PERSONA_USER  ");
            sb.Append($"		AND (CD.IDGDA_COLLABORATORS = @ID OR   ");
            sb.Append($"		CD.MATRICULA_SUPERVISOR = @ID OR   ");
            sb.Append($"		CD.MATRICULA_COORDENADOR = @ID OR   ");
            sb.Append($"		CD.MATRICULA_GERENTE_II = @ID OR   ");
            sb.Append($"		CD.MATRICULA_GERENTE_I = @ID OR   ");
            sb.Append($"		CD.MATRICULA_DIRETOR = @ID OR   ");
            sb.Append($"		CD.MATRICULA_CEO = @ID)  ");
            sb.Append($"    WHERE CONVERT(DATE, GDU.CREATED_AT) >= @DATE_INI AND CONVERT(DATE, GDU.CREATED_AT) <= @DATE_FIM ");
            sb.Append($"    GROUP BY IDGDA_CLIMATE, CONVERT(DATE, GDU.CREATED_AT) ");
            sb.Append($") GCU ON GCU.IDGDA_CLIMATE = GC.IDGDA_CLIMATE ");
            sb.Append($"ORDER BY GC.POSITION; ");


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
                                returnListPeriodClimate rtn = new returnListPeriodClimate();

                                rtn.idClimate = reader["IDGDA_CLIMATE"] != DBNull.Value ? int.Parse(reader["IDGDA_CLIMATE"].ToString()) : 0;
                                rtn.climate = reader["CLIMATE"] != DBNull.Value ? reader["CLIMATE"].ToString() : "";
                                rtn.image = reader["IMAGE"] != DBNull.Value ? reader["IMAGE"].ToString() : "";
                                rtn.count = reader["COUNT"] != DBNull.Value ? int.Parse(reader["COUNT"].ToString()) : 0;
                                rtn.data = reader["DATA"] != DBNull.Value ? reader["DATA"].ToString() : "";
                                if (rtn.data == "")
                                {
                                    continue;
                                }

                                retorno.Add(rtn);
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

            //int totalQuantity = retorno.Sum(item => item.count);




            //List<returnListPeriodClimate> returnLists = retorno
            //             .Select(group => new returnListPeriodClimate
            //             {
            //                 idClimate = group.idClimate,
            //                 climate = group.climate,
            //                 image = group.image,
            //                 count = group.count,
            //                 data = group.data, 
            //                 percent = totalQuantity != 0 ? Math.Round(((double)group.count / totalQuantity) * 100, 1, MidpointRounding.AwayFromZero) : 0
            //             }).ToList();

            List<GroupedPeriodClimate> returnPeriods = new List<GroupedPeriodClimate>();

            GroupingPeriod period = GroupingPeriod.Day;
            if (PERIOD == "mensal")
            {
                period = GroupingPeriod.Month;
            } 
            else if (PERIOD == "diario")
            {
                period = GroupingPeriod.Day;
            }
            else if (PERIOD == "quinzenal")
            {
                period = GroupingPeriod.Biweekly;
            }
            else if (PERIOD == "semanal")
            {
                period = GroupingPeriod.Week;
            }

            List<GroupedPeriodClimate> groupedByWeek = ListPeriodClimateController.GroupByPeriod(retorno, period);


            //List<returnListPeriodClimate> returnLists = retorno
            //    .GroupBy(item => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(Convert.ToDateTime(item.data), CalendarWeekRule.FirstDay, DayOfWeek.Sunday))
            //    .Select(group =>
            //    {
            //         var totalQuantity = group.Sum(item => item.count);
            //         return new returnListPeriodClimate
            //         {
            //             idClimate = group.First().idClimate,
            //             climate = group.First().climate,
            //             image = group.First().image,
            //             count = totalQuantity,
            //             data = group.First().data,
            //             percent = totalQuantity != 0 ? Math.Round(((double)totalQuantity / retorno.Sum(item => item.count)) * 100, 1, MidpointRounding.AwayFromZero) : 0
            //         };
            //     })
            //        .ToList();





            return returnPeriods;
        }

    }

}