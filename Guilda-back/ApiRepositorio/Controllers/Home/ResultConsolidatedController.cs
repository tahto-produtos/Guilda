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
using System.Runtime.ConstrainedExecution;
using System.Web.UI.WebControls;
using static ApiRepositorio.Controllers.BasketGeneralUserController;
using System.Text.RegularExpressions;
using static ApiRepositorio.Controllers.ResultConsolidatedByHierarchiesController;
using DocumentFormat.OpenXml.Math;
using Amazon.S3.Model;
using ThirdParty.Json.LitJson;
using static ApiRepositorio.Controllers.IntegracaoAPIResultController;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ResultConsolidatedController : ApiController
    {
        public class Sector
        {
            public int Id { get; set; }
        }
        public class SectorGroup
        {
            public string Name { get; set; }
        }
        public class Indicator
        {
            public int Id { get; set; }
        }
        public class Hierarchy
        {
            public int Id { get; set; }
        }
        public class InputModel
        {
            public string codCollaborator { get; set; }
            public DateTime dataInicial { get; set; }
            public DateTime dataFinal { get; set; }
            public string Hierarchies { get; set; }
            public List<Sector> Sectors { get; set; }
            public List<SectorGroup> SectorsGroups { get; set; }
            public List<Sector> periods { get; set; }
            public List<Sector> homeFloors { get; set; }
            public List<Sector> sites { get; set; }
            public List<Sector> SubSectors { get; set; }
            public List<Indicator> Indicators { get; set; }
            public List<Sector> sectorsIds { get; set; }
            public bool? basket { get; set; }
        }
        public class HomeResultConsolidated
        {
            public string MATRICULA { get; set; }
            public string CARGO { get; set; }
            public string IDINDICADOR { get; set; }
            public string INDICADOR { get; set; }
            public double META_MAXIMA_MOEDAS { get; set; }
            public double MOEDA_GANHA { get; set; }
            public double FACTOR0 { get; set; }
            public double FACTOR1 { get; set; }
            public double META { get; set; }
            public string CONTA { get; set; }
            public string BETTER { get; set; }
            public double RESULTADO { get; set; }
            public double PERCENTUAL { get; set; }
            public double QTD { get; set; }
            public double QTD_MON { get; set; }
            public double QTD_MON_TOTAL { get; set; }
            public double QTD_META { get; set; }
            public string DATAPAGAMENTO { get; set; }
            public double min1 { get; set; }
            public double min2 { get; set; }
            public double min3 { get; set; }
            public double min4 { get; set; }
            public string GRUPO { get; set; }
            public int IDGRUPO { get; set; }
            public string IMAGEMGRUPO { get; set; }
            public string CODGIP { get; set; }
            public string SETOR { get; set; }
            public string TYPE { get; set; }
            public bool MONETIZATION { get; set; }
            public int SUMDIASLOGADOS { get; set; }
            public int SUMDIASESCALADOS { get; set; }
            public int SUMDIASLOGADOSESCALADOS { get; set; }


        }
        public class returnResponseDay
        {
            public string MATRICULA { get; set; }
            public string CARGO { get; set; }
            public string IDINDICADOR { get; set; }
            public string INDICADOR { get; set; }
            public double? META { get; set; }
            public string META_HORA { get; set; }
            public double RESULTADO { get; set; }
            public string RESULTADO_HORA { get; set; }
            public double PERCENTUAL { get; set; }
            public double META_MAXIMA_MOEDAS { get; set; }
            public double MOEDA_GANHA { get; set; }
            public string GRUPO { get; set; }
            public string CODGIP { get; set; }
            public string SETOR { get; set; }
            public string TYPE { get; set; }
            public int IDGRUPO { get; set; }
            public string IMAGEMGRUPO { get; set; }
            public bool MONETIZATION { get; set; }
        }

        public class ReturnResponseConsolidade
        {
            public List<returnResponseDay> RESULTS { get; set; }
            public List<Ranking> RANKING { get; set; }
        }

        public class Ranking
        {
            public int IDGROUP { get; set; }
            public string GROUPNAME { get; set; }
            public string DESCRIPTION { get; set; }
            public string ALIAS { get; set; }
            public string IMAGEMGROUP { get; set; }
            public int COUNT { get; set; }
            public double PERCENT { get; set; }
        }

        public class GroupModel
        {
            public int id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public string alias { get; set; }
            public int uploadId { get; set; }
            public string url { get; set; }
        }

        public static List<HomeResultConsolidated> ReturnHomeResultConsolidated(string idCol, string dtinicial, string dtfinal, string hierarchies, string indicators, string sectors, string subsectors, string periods, string homefloors, string sites, bool? bkt, string outerFilters = "")
        {
            string filter = "";
            HomeResultConsolidated rmams = new HomeResultConsolidated();
            rmams.MATRICULA = idCol;

            string filterHis = "";

            //COLABORADOR
            if (hierarchies.ToString().Contains("1"))
            {
                filter = "AND CL.IDGDA_COLLABORATORS = @INPUTID";
            }
            //SUPERVISOR
            if (hierarchies.ToString().Contains("2"))
            {
                filter = "AND CL.MATRICULA_SUPERVISOR = @INPUTID";
            }
            //COORDENADOR
            if (hierarchies.ToString().Contains("3"))
            {
                filter = "AND CL.MATRICULA_COORDENADOR = @INPUTID";
            }
            //GERENTE_II
            if (hierarchies.ToString().Contains("4"))
            {
                filter = "AND CL.MATRICULA_GERENTE_II = @INPUTID";
            }
            //GERENTE_I
            if (hierarchies.ToString().Contains("5"))
            {
                filter = "AND CL.MATRICULA_GERENTE_I = @INPUTID";
            }
            //DIRETOR
            if (hierarchies.ToString().Contains("6"))
            {
                filter = "AND CL.MATRICULA_DIRETOR = @INPUTID";
            }
            //CEO
            if (hierarchies.ToString().Contains("7"))
            {
                filter = "AND CL.MATRICULA_CEO = @INPUTID";
            }

            string filterTBL = "";
            //Periodos
            if (periods != "")
            {
                filterTBL = filterTBL + $" AND IDGDA_PERIOD IN ({periods}) ";
            }

            //Home ou Piso
            if (homefloors != "")
            {
                string hf = homefloors.Replace("1", "'SIM'").Replace("2", "'NÃO'");

                filterTBL = filterTBL + $" AND HOME_BASED IN ({hf}) ";
            }

            //Site
            if (sites != "")
            {
                filterTBL = filterTBL + $" AND S.IDGDA_SITE IN ({sites}) ";
            }

            //SETORES
            string columnName = "";
            if ((sectors != "" || subsectors != ""))// && bkt == true)
            {
                if (subsectors != "")
                {
                    columnName = "IDGDA_SUBSECTOR";
                    filter = filter + $" AND CL.IDGDA_SECTOR IN ({sectors}) AND CL.IDGDA_SUBSECTOR IN ({subsectors}) ";
                    filterHis = filterHis + $" AND SECTOR_ID IN ({subsectors}) ";
                }
                else if (sectors != "")
                {
                    columnName = "IDGDA_SECTOR";
                    filter = filter + $" AND CL.IDGDA_SECTOR IN ({sectors}) ";
                    filterHis = filterHis + $" AND SECTOR_ID IN ({sectors}) ";
                }
            }
            else
            {
                columnName = "IDGDA_SECTOR";
            }
            //INDICADORES
            if (indicators != "")
            {
                filter = filter + $" AND R.INDICADORID IN ({indicators}) ";
            }

            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtinicial);
            stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtfinal);
            stb.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}'; ", idCol);
            #region Comentado
            //stb.Append("SELECT  ");
            //stb.Append("       CONVERT(DATE, R.CREATED_AT) AS CREATED_AT, ");
            //stb.Append("	   COUNT(0) AS QTD, ");
            //stb.Append("SUM(CASE  ");
            //stb.Append("        WHEN CL.PERIODO = 'DIURNO' AND HIG1.MONETIZATION > 0 THEN 1 ");
            //stb.Append("        WHEN CL.PERIODO = 'NOTURNO' AND HIG1.MONETIZATION_NIGHT > 0 THEN 1 ");
            //stb.Append("        WHEN CL.PERIODO = 'MADRUGADA' AND HIG1.MONETIZATION_LATENIGHT > 0 THEN 1 ");
            //stb.Append("        ELSE 0  ");
            //stb.Append("    END) AS QTD_MON, ");
            //stb.Append(" ");
            //stb.Append("SUM(CASE  ");
            //stb.Append("        WHEN CL.PERIODO = 'DIURNO' AND HIG1.MONETIZATION > 0 THEN HIG1.MONETIZATION ");
            //stb.Append("        WHEN CL.PERIODO = 'NOTURNO' AND HIG1.MONETIZATION_NIGHT > 0 THEN HIG1.MONETIZATION_NIGHT ");
            //stb.Append("        WHEN CL.PERIODO = 'MADRUGADA' AND HIG1.MONETIZATION_LATENIGHT > 0 THEN HIG1.MONETIZATION_LATENIGHT ");
            //stb.Append("        ELSE 0  ");
            //stb.Append("    END) AS QTD_MON_TOTAL, ");

            //stb.Append("       CASE ");
            //stb.Append("           WHEN @INPUTID = MAX(CL.IDGDA_COLLABORATORS) THEN 'AGENTE' ");
            //stb.Append("           WHEN @INPUTID = MAX(CL.MATRICULA_SUPERVISOR) THEN 'SUPERVISOR' ");
            //stb.Append("           WHEN @INPUTID = MAX(CL.MATRICULA_COORDENADOR) THEN 'COORDENADOR' ");
            //stb.Append("           WHEN @INPUTID = MAX(CL.MATRICULA_GERENTE_II) THEN 'GERENTE_II' ");
            //stb.Append("           WHEN @INPUTID = MAX(CL.MATRICULA_GERENTE_I) THEN 'GERENTE_I' ");
            //stb.Append("           WHEN @INPUTID = MAX(CL.MATRICULA_DIRETOR) THEN 'DIRETOR' ");
            //stb.Append("           WHEN @INPUTID = MAX(CL.MATRICULA_CEO) THEN 'CEO' ");
            //stb.Append("           ELSE '' ");
            //stb.Append("       END AS CARGO, ");
            //stb.Append("MAX(CL.IDGDA_SECTOR) AS COD_GIP, ");
            //stb.Append("MAX(SEC.NAME) AS SETOR, ");
            //stb.Append("       R.INDICADORID AS 'COD INDICADOR', ");
            //stb.Append("	   MAX(IT.NAME) AS 'INDICADOR', ");

            ////stb.Append("	   MAX(ISNULL(HIS.GOAL,0)) AS META, ");

            //stb.Append("MAX(CASE  ");
            //stb.Append("     WHEN CL.PERIODO = 'DIURNO' THEN HIS.GOAL ");
            //stb.Append("     WHEN CL.PERIODO = 'NOTURNO' THEN HIS.GOAL_NIGHT ");
            //stb.Append("     WHEN CL.PERIODO = 'MADRUGADA' THEN HIS.GOAL_LATENIGHT ");
            //stb.Append("     ELSE 0  ");
            //stb.Append(" END) AS META, ");

            ////stb.Append("	   SUM(ISNULL(HIS.GOAL,0)) AS META, ");
            ////stb.Append("	   SUM(ISNULL(HIS.GOAL_NIGHT,0)) AS META_NOTURNA, ");
            ////stb.Append("	   SUM(ISNULL(HIS.GOAL_LATENIGHT,0)) AS META_MADRUGADA,  ");

            ////stb.Append("	   SUM(ISNULL(HIG1.METRIC_MIN,0)) AS MIN1, ");
            ////stb.Append("	   SUM(ISNULL(HIG2.METRIC_MIN,0)) AS MIN2, ");
            ////stb.Append("	   SUM(ISNULL(HIG3.METRIC_MIN,0)) AS MIN3, ");
            ////stb.Append("	   SUM(ISNULL(HIG4.METRIC_MIN,0)) AS MIN4, ");
            ////stb.Append("	   SUM(ISNULL(HIG1.METRIC_MIN_NIGHT,0)) AS MIN1_NOTURNO, ");
            ////stb.Append("	   SUM(ISNULL(HIG2.METRIC_MIN_NIGHT,0)) AS MIN2_NOTURNO, ");
            ////stb.Append("	   SUM(ISNULL(HIG3.METRIC_MIN_NIGHT,0)) AS MIN3_NOTURNO, ");
            ////stb.Append("	   SUM(ISNULL(HIG4.METRIC_MIN_NIGHT,0)) AS MIN4_NOTURNO, ");
            ////stb.Append("	   SUM(ISNULL(HIG1.METRIC_MIN_LATENIGHT,0)) AS MIN1_MADRUGADA, ");
            ////stb.Append("	   SUM(ISNULL(HIG2.METRIC_MIN_LATENIGHT,0)) AS MIN2_MADRUGADA, ");
            ////stb.Append("	   SUM(ISNULL(HIG3.METRIC_MIN_LATENIGHT,0)) AS MIN3_MADRUGADA, ");
            ////stb.Append("	   SUM(ISNULL(HIG4.METRIC_MIN_LATENIGHT,0)) AS MIN4_MADRUGADA, ");

            ////stb.Append("       SUM(ISNULL(HIG1.METRIC_MIN,0)) AS MIN1, ");
            ////stb.Append("       SUM(ISNULL(HIG2.METRIC_MIN,0)) AS MIN2, ");
            ////stb.Append("       SUM(ISNULL(HIG3.METRIC_MIN,0)) AS MIN3, ");
            ////stb.Append("       SUM(ISNULL(HIG4.METRIC_MIN,0)) AS MIN4, ");

            //stb.Append("SUM( ");
            //stb.Append("CASE  ");
            //stb.Append("     WHEN CL.PERIODO = 'DIURNO' THEN HIG1.METRIC_MIN ");
            //stb.Append("     WHEN CL.PERIODO = 'NOTURNO' THEN HIG1.METRIC_MIN_NIGHT ");
            //stb.Append("     WHEN CL.PERIODO = 'MADRUGADA' THEN HIG1.METRIC_MIN_LATENIGHT ");
            //stb.Append("     ELSE 0  ");
            //stb.Append(" END ");
            //stb.Append(") AS MIN1, ");
            //stb.Append(" SUM( ");
            //stb.Append("CASE  ");
            //stb.Append("     WHEN CL.PERIODO = 'DIURNO' THEN HIG2.METRIC_MIN ");
            //stb.Append("     WHEN CL.PERIODO = 'NOTURNO' THEN HIG2.METRIC_MIN_NIGHT ");
            //stb.Append("     WHEN CL.PERIODO = 'MADRUGADA' THEN HIG2.METRIC_MIN_LATENIGHT ");
            //stb.Append("     ELSE 0  ");
            //stb.Append(" END ");
            //stb.Append(") AS MIN2, ");
            //stb.Append(" SUM( ");
            //stb.Append("CASE  ");
            //stb.Append("     WHEN CL.PERIODO = 'DIURNO' THEN HIG3.METRIC_MIN ");
            //stb.Append("     WHEN CL.PERIODO = 'NOTURNO' THEN HIG3.METRIC_MIN_NIGHT ");
            //stb.Append("     WHEN CL.PERIODO = 'MADRUGADA' THEN HIG3.METRIC_MIN_LATENIGHT ");
            //stb.Append("     ELSE 0  ");
            //stb.Append(" END ");
            //stb.Append(") AS MIN3, ");
            //stb.Append(" SUM( ");
            //stb.Append("CASE  ");
            //stb.Append("     WHEN CL.PERIODO = 'DIURNO' THEN HIG4.METRIC_MIN ");
            //stb.Append("     WHEN CL.PERIODO = 'NOTURNO' THEN HIG4.METRIC_MIN_NIGHT ");
            //stb.Append("     WHEN CL.PERIODO = 'MADRUGADA' THEN HIG4.METRIC_MIN_LATENIGHT ");
            //stb.Append("     ELSE 0  ");
            //stb.Append(" END ");
            //stb.Append(") AS MIN4, ");



            //stb.Append("       CASE ");
            //stb.Append("           WHEN MAX(ME.EXPRESSION) IS NULL THEN '(#FATOR1/#FATOR0)' ");
            //stb.Append("           ELSE MAX(ME.EXPRESSION) ");
            //stb.Append("       END AS CONTA, ");
            //stb.Append("       CASE ");
            //stb.Append("           WHEN MAX(IT.CALCULATION_TYPE) IS NULL THEN 'BIGGER_BETTER' ");
            //stb.Append("           ELSE MAX(IT.CALCULATION_TYPE) ");
            //stb.Append("       END AS BETTER, ");
            //stb.Append("       MAX(IT.TYPE) AS TYPE,");
            //stb.Append("	   sUM(ISNULL(F1.FACTOR,0)) AS FACTOR0, ");
            //stb.Append("       SUM(ISNULL(F2.FACTOR,0)) AS FACTOR1, ");

            //stb.Append("       SUM(ISNULL(HIG1.MONETIZATION,0)) AS META_MAXIMA_MOEDA, ");

            ////stb.Append("       SUM(ISNULL(HIG1.MONETIZATION,0)) AS META_MAXIMA, ");
            ////stb.Append("       SUM(ISNULL(HIG1.MONETIZATION_NIGHT,0)) AS META_MAXIMA_NOTURNA, ");
            ////stb.Append("       SUM(ISNULL(HIG1.MONETIZATION_LATENIGHT,0)) AS META_MAXIMA_MADRUGADA, ");


            //stb.Append("       CASE ");
            //stb.Append("           WHEN SUM(HIG1.MONETIZATION) IS NULL THEN 0 ");
            //stb.Append("           WHEN MAX(MZ.INPUT) IS NULL THEN 0 ");
            //stb.Append("           ELSE MAX(MZ.INPUT) ");
            //stb.Append("       END AS MOEDA_GANHA, ");
            //stb.Append(" MAX(CL.PERIODO) AS TURNO ");
            //stb.Append("FROM GDA_RESULT (NOLOCK) AS R ");
            //stb.Append("LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS CB ON CB.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            ////stb.Append("LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CL ON CL.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            ////stb.Append("AND CL.CREATED_AT = R.CREATED_AT ");
            //stb.Append("LEFT JOIN ");
            //stb.Append("  (SELECT CASE ");
            //stb.Append("              WHEN IDGDA_SUBSECTOR IS NULL THEN IDGDA_SECTOR ");
            //stb.Append("              ELSE IDGDA_SUBSECTOR ");
            //stb.Append("          END AS IDGDA_SECTOR, ");
            //stb.Append("          CREATED_AT, ");
            //stb.Append("          IDGDA_COLLABORATORS, ");
            //stb.Append("          IDGDA_SECTOR_SUPERVISOR, ");
            //stb.Append("          ACTIVE, ");
            //stb.Append("          CARGO, ");
            //stb.Append("          HOME_BASED, ");
            //stb.Append("          SITE, ");
            //stb.Append("          PERIODO, ");
            //stb.Append("          MATRICULA_SUPERVISOR, ");
            //stb.Append("          NOME_SUPERVISOR, ");
            //stb.Append("          MATRICULA_COORDENADOR, ");
            //stb.Append("          NOME_COORDENADOR, ");
            //stb.Append("          MATRICULA_GERENTE_II, ");
            //stb.Append("          NOME_GERENTE_II, ");
            //stb.Append("          MATRICULA_GERENTE_I, ");
            //stb.Append("          NOME_GERENTE_I, ");
            //stb.Append("          MATRICULA_DIRETOR, ");
            //stb.Append("          NOME_DIRETOR, ");
            //stb.Append("          MATRICULA_CEO, ");
            //stb.Append("          NOME_CEO ");
            //stb.Append("   FROM GDA_COLLABORATORS_DETAILS (NOLOCK) ");
            //stb.Append("   WHERE CREATED_AT >= @DATAINICIAL ");
            //stb.Append("     AND CREATED_AT <= @DATAFINAL ) AS CL ON CL.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS ");
            //stb.Append("AND CL.CREATED_AT = R.CREATED_AT ");
            //stb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS IT ON IT.IDGDA_INDICATOR = R.INDICADORID ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
            //stb.Append("AND HIG1.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIG1.SECTOR_ID = CL.IDGDA_SECTOR ");
            //stb.Append("AND HIG1.GROUPID = 1 ");
            //stb.Append("AND CONVERT(DATE,HIG1.STARTED_AT) <= R.CREATED_AT ");
            //stb.Append("AND CONVERT(DATE,HIG1.ENDED_AT) >= R.CREATED_AT ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG2 ON HIG2.DELETED_AT IS NULL ");
            //stb.Append("AND HIG2.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIG2.SECTOR_ID = CL.IDGDA_SECTOR ");
            //stb.Append("AND HIG2.GROUPID = 2 ");
            //stb.Append("AND CONVERT(DATE,HIG2.STARTED_AT) <= R.CREATED_AT ");
            //stb.Append("AND CONVERT(DATE,HIG2.ENDED_AT) >= R.CREATED_AT ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG3 ON HIG3.DELETED_AT IS NULL ");
            //stb.Append("AND HIG3.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIG3.SECTOR_ID = CL.IDGDA_SECTOR ");
            //stb.Append("AND HIG3.GROUPID = 3 ");
            //stb.Append("AND CONVERT(DATE,HIG3.STARTED_AT) <= R.CREATED_AT ");
            //stb.Append("AND CONVERT(DATE,HIG3.ENDED_AT) >= R.CREATED_AT ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG4 ON HIG4.DELETED_AT IS NULL ");
            //stb.Append("AND HIG4.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIG4.SECTOR_ID = CL.IDGDA_SECTOR ");
            //stb.Append("AND HIG4.GROUPID = 4 ");
            //stb.Append("AND CONVERT(DATE,HIG4.STARTED_AT) <= R.CREATED_AT ");
            //stb.Append("AND CONVERT(DATE,HIG4.ENDED_AT) >= R.CREATED_AT ");
            //stb.Append("LEFT JOIN GDA_FACTOR (NOLOCK) AS F1 ON F1.IDGDA_RESULT = R.IDGDA_RESULT ");
            //stb.Append("AND F1.[INDEX] = 1 ");
            //stb.Append("LEFT JOIN GDA_FACTOR (NOLOCK) AS F2 ON F2.IDGDA_RESULT = R.IDGDA_RESULT ");
            //stb.Append("AND F2.[INDEX] = 2 ");
            //stb.Append("LEFT JOIN GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME ON HME.INDICATORID = R.INDICADORID ");
            //stb.Append("AND HME.deleted_at IS NULL ");
            //stb.Append("LEFT JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON ME.ID = HME.MATHEMATICALEXPRESSIONID ");
            //stb.Append("AND ME.DELETED_AT IS NULL ");
            //stb.Append("LEFT JOIN ");
            //stb.Append("  (SELECT SUM(INPUT) - SUM(OUTPUT) AS INPUT, ");
            //stb.Append("          gda_indicator_idgda_indicator, ");
            //stb.Append("          result_date, ");
            //stb.Append("          COLLABORATOR_ID ");
            //stb.Append("   FROM GDA_CHECKING_ACCOUNT ");
            //stb.Append("   WHERE RESULT_DATE >= @DATAINICIAL ");
            //stb.Append("     AND RESULT_DATE <= @DATAFINAL ");
            //stb.Append("	 AND COLLABORATOR_ID = @INPUTID ");
            //stb.Append("   GROUP BY gda_indicator_idgda_indicator, ");
            //stb.Append("            result_date, ");
            //stb.Append("            COLLABORATOR_ID) AS MZ ON MZ.gda_indicator_idgda_indicator = R.INDICADORID ");
            //stb.Append("AND MZ.result_date = R.CREATED_AT ");
            ////stb.Append("AND MZ.COLLABORATOR_ID = R.IDGDA_COLLABORATORS ");

            //stb.Append("LEFT JOIN  ");
            //stb.Append("( ");
            //stb.Append("SELECT (SUM(GOAL) / COUNT(0)) AS GOAL, (SUM(GOAL_NIGHT) / COUNT(0)) AS GOAL_NIGHT, ");
            //stb.Append("(SUM(GOAL_LATENIGHT) / COUNT(0)) AS GOAL_LATENIGHT, ");
            //stb.Append("INDICATOR_ID, MAX(STARTED_AT) AS STARTED_AT, ");
            //stb.Append("MAX(ENDED_AT) AS ENDED_AT FROM GDA_HISTORY_INDICATOR_SECTORS (NOLOCK)  ");
            //stb.Append("WHERE CONVERT(DATE,STARTED_AT) <= @DATAINICIAL  ");
            //stb.Append("AND CONVERT(DATE,ENDED_AT) >= @DATAFINAL  ");
            //stb.Append("AND DELETED_AT IS NULL ");
            //stb.Append("GROUP BY INDICATOR_ID ");
            //stb.Append(") AS HIS ON HIS.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND CONVERT(DATE,HIS.STARTED_AT) <= R.CREATED_AT ");
            //stb.Append("AND CONVERT(DATE,HIS.ENDED_AT) >= R.CREATED_AT ");

            ////stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) AS HIS ON HIS.INDICATOR_ID = R.INDICADORID ");
            ////stb.Append("AND HIS.SECTOR_ID = CL.IDGDA_SECTOR ");
            ////stb.Append("AND CONVERT(DATE,HIS.STARTED_AT) <= R.CREATED_AT ");
            ////stb.Append("AND CONVERT(DATE,HIS.ENDED_AT) >= R.CREATED_AT ");
            ////stb.Append("AND HIS.DELETED_AT IS NULL ");
            //stb.Append("LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = CL.IDGDA_SECTOR ");
            //stb.Append(" ");
            //stb.Append("WHERE 1 = 1 ");
            //stb.Append("  AND R.CREATED_AT >= @DATAINICIAL ");
            //stb.Append("  AND R.CREATED_AT <= @DATAFINAL ");
            //stb.Append("  AND R.DELETED_AT IS NULL ");
            //stb.Append("  AND CL.IDGDA_SECTOR IS NOT NULL ");
            //stb.Append("  AND CL.CARGO IS NOT NULL ");
            //stb.Append("  AND CL.HOME_BASED <> '' ");
            //stb.Append("  AND CL.active = 'true' ");
            //stb.Append("AND R.FACTORS <> '0.000000;0.000000'  ");
            //stb.Append("AND R.FACTORS <> '0.000000; 0.000000' ");
            //stb.AppendFormat("  {0}  ", filter);
            //stb.Append("GROUP BY R.INDICADORID, ");
            //stb.Append("         CONVERT(DATE, R.CREATED_AT) ");
            #endregion

            stb.AppendFormat("SELECT TBl.CREATED_AT, SUM(QTD) AS QTD, ");
            stb.AppendFormat("SUM(CASE ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 1 ");
            stb.AppendFormat("                    AND HIG1.MONETIZATION > 0 THEN QTD ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 2 ");
            stb.AppendFormat("                    AND HIG1.MONETIZATION_NIGHT > 0 THEN QTD ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 3 ");
            stb.AppendFormat("                    AND HIG1.MONETIZATION_LATENIGHT > 0 THEN QTD ");
            stb.AppendFormat("               ELSE 0 ");
            stb.AppendFormat("           END) AS QTD_MON, ");
            stb.AppendFormat("       SUM(CASE ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 1 ");
            stb.AppendFormat("                    AND HIG1.MONETIZATION > 0 THEN (HIG1.MONETIZATION * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 2 ");
            stb.AppendFormat("                    AND HIG1.MONETIZATION_NIGHT > 0 THEN (HIG1.MONETIZATION_NIGHT * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 3 ");
            stb.AppendFormat("                    AND HIG1.MONETIZATION_LATENIGHT > 0 THEN (HIG1.MONETIZATION_LATENIGHT * QTD) ");
            stb.AppendFormat("               ELSE 0 ");
            stb.AppendFormat("           END) AS QTD_MON_TOTAL, ");

            stb.AppendFormat(" 		   SUM(CASE ");
            stb.AppendFormat("              WHEN TBL.IDGDA_PERIOD = 1 ");
            stb.AppendFormat("                  AND HIS.GOAL IS NOT NULL THEN (QTD) ");
            stb.AppendFormat("              WHEN TBL.IDGDA_PERIOD = 2 ");
            stb.AppendFormat("                  AND HIS.GOAL_NIGHT > 0 THEN (QTD) ");
            stb.AppendFormat("              WHEN TBL.IDGDA_PERIOD = 3 ");
            stb.AppendFormat("                  AND HIS.GOAL_LATENIGHT > 0 THEN (QTD) ");
            stb.AppendFormat("              ELSE 0 ");
            stb.AppendFormat("          END) AS QTD_META, ");

            stb.AppendFormat("	   MAX(TBL.IDGDA_SECTOR) AS IDGDA_SECTOR, ");
            stb.AppendFormat("	   MAX(TBL.IDGDA_SUBSECTOR) AS IDGDA_SUBSECTOR, ");
            stb.AppendFormat("	   MAX(SEC.NAME) AS SETOR, ");
            stb.AppendFormat("       MAX(IT.IDGDA_INDICATOR) AS IDGDA_INDICATOR, ");
            stb.AppendFormat("	   MAX(IT.NAME) AS 'INDICADOR', ");
            stb.AppendFormat("       SUM(CASE ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 1 THEN (HIS.GOAL * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 2 THEN (HIS.GOAL_NIGHT * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 3 THEN (HIS.GOAL_LATENIGHT * QTD) ");
            stb.AppendFormat("               ELSE 0 ");
            stb.AppendFormat("           END) AS META, ");
            stb.AppendFormat("       SUM(CASE ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 1 THEN (HIG1.METRIC_MIN * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 2 THEN (HIG1.METRIC_MIN_NIGHT * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 3 THEN (HIG1.METRIC_MIN_LATENIGHT * QTD) ");
            stb.AppendFormat("               ELSE 0 ");
            stb.AppendFormat("           END) AS MIN1, ");
            stb.AppendFormat("       SUM(CASE ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 1 THEN (HIG2.METRIC_MIN * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 2 THEN (HIG2.METRIC_MIN_NIGHT * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 3 THEN (HIG2.METRIC_MIN_LATENIGHT * QTD) ");
            stb.AppendFormat("               ELSE 0 ");
            stb.AppendFormat("           END) AS MIN2, ");
            stb.AppendFormat("       SUM(CASE ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 1 THEN (HIG3.METRIC_MIN * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 2 THEN (HIG3.METRIC_MIN_NIGHT * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 3 THEN (HIG3.METRIC_MIN_LATENIGHT * QTD) ");
            stb.AppendFormat("               ELSE 0 ");
            stb.AppendFormat("           END) AS MIN3, ");
            stb.AppendFormat("       SUM(CASE ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 1 THEN (HIG4.METRIC_MIN * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 2 THEN (HIG4.METRIC_MIN_NIGHT * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 3 THEN (HIG4.METRIC_MIN_LATENIGHT * QTD) ");
            stb.AppendFormat("               ELSE 0 ");
            stb.AppendFormat("           END) AS MIN4, ");
            stb.AppendFormat("		    CASE ");
            stb.AppendFormat("           WHEN MAX(ME.EXPRESSION) IS NULL THEN '(#FATOR1/#FATOR0)' ");
            stb.AppendFormat("           ELSE MAX(ME.EXPRESSION) ");
            stb.AppendFormat("       END AS CONTA, ");
            stb.AppendFormat("       CASE ");
            stb.AppendFormat("           WHEN MAX(IT.CALCULATION_TYPE) IS NULL THEN 'BIGGER_BETTER' ");
            stb.AppendFormat("           ELSE MAX(IT.CALCULATION_TYPE) ");
            stb.AppendFormat("       END AS BETTER, ");
            stb.AppendFormat("       MAX(IT.TYPE) AS TYPE, ");
            stb.AppendFormat("	   SUM(TBL.FACTOR0) AS FACTOR0, ");
            stb.AppendFormat("	   SUM(TBL.FACTOR1) AS FACTOR1, ");
            //stb.AppendFormat("	   SUM(TBL.MOEDA_GANHA) AS MOEDA_GANHA ");

            stb.AppendFormat(" SUM(CASE ");
            stb.AppendFormat("         WHEN TBL.IDGDA_PERIOD = 1 ");
            stb.AppendFormat("              AND HIG1.MONETIZATION > 0 THEN(TBL.MOEDA_GANHA) ");
            stb.AppendFormat("         WHEN TBL.IDGDA_PERIOD = 2 ");
            stb.AppendFormat("              AND HIG1.MONETIZATION_NIGHT > 0 THEN(TBL.MOEDA_GANHA) ");
            stb.AppendFormat("         WHEN TBL.IDGDA_PERIOD = 3 ");
            stb.AppendFormat("              AND HIG1.MONETIZATION_LATENIGHT > 0 THEN(TBL.MOEDA_GANHA) ");
            stb.AppendFormat("         ELSE 0 ");
            stb.AppendFormat("     END) AS MOEDA_GANHA ");

            stb.AppendFormat("FROM  ");
            stb.AppendFormat("( ");
            stb.AppendFormat("SELECT  ");
            stb.AppendFormat("CONVERT(DATE, R.CREATED_AT) AS CREATED_AT, ");
            stb.AppendFormat("       COUNT(0) AS QTD, ");
            stb.AppendFormat("	   CL.IDGDA_PERIOD AS IDGDA_PERIOD, ");
            stb.AppendFormat("     CL.IDGDA_SECTOR AS IDGDA_SECTOR, ");
            stb.AppendFormat("     CL.IDGDA_SUBSECTOR AS IDGDA_SUBSECTOR, ");
            stb.AppendFormat("     CL.IDGDA_SECTOR_SUBSECTOR AS IDGDA_SECTOR_SUBSECTOR, ");
            stb.AppendFormat("     R.INDICADORID AS INDICADORID, ");

            stb.AppendFormat("     MAX(CL.CARGO) AS CARGO, ");
            stb.AppendFormat("	   SUM(FACTORSAG0) AS FACTOR0, ");
            stb.AppendFormat("	   SUM(FACTORSAG1) AS FACTOR1, ");
            stb.AppendFormat("	   SUM(MZ.INPUT) AS MOEDA_GANHA ");
            stb.AppendFormat("FROM GDA_RESULT (NOLOCK) AS R ");
            stb.AppendFormat("LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS CB ON CB.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");

            stb.AppendFormat("LEFT JOIN ");
            stb.AppendFormat("  (SELECT ");
            stb.AppendFormat("   CD.IDGDA_SECTOR, ");
            stb.AppendFormat("   CD.IDGDA_SUBSECTOR, ");

            stb.AppendFormat("   CASE WHEN CD.IDGDA_SUBSECTOR IS NOT NULL THEN CD.IDGDA_SUBSECTOR ");
            stb.AppendFormat("   ELSE CD.IDGDA_SECTOR END AS IDGDA_SECTOR_SUBSECTOR, ");

            stb.AppendFormat("   CD.CREATED_AT, ");
            stb.AppendFormat("	 CD.CARGO, ");
            stb.AppendFormat("	 CD.IDGDA_PERIOD, ");
            stb.AppendFormat("   CD.IDGDA_COLLABORATORS, ");
            stb.AppendFormat("   CD.MATRICULA_SUPERVISOR, ");
            stb.AppendFormat("   CD.MATRICULA_COORDENADOR, ");
            stb.AppendFormat("   CD.MATRICULA_GERENTE_II, ");
            stb.AppendFormat("   CD.MATRICULA_GERENTE_I, ");
            stb.AppendFormat("   CD.MATRICULA_DIRETOR, ");
            stb.AppendFormat("   CD.MATRICULA_CEO ");
            stb.AppendFormat("   FROM GDA_COLLABORATORS_DETAILS (NOLOCK) CD ");
            stb.AppendFormat("   LEFT JOIN GDA_SITE (NOLOCK) S ON CD.SITE = S.SITE ");
            stb.AppendFormat("   WHERE CD.CREATED_AT >= @DATAINICIAL ");
            stb.AppendFormat("     AND CD.CREATED_AT <= @DATAFINAL ");
            //stb.AppendFormat("	AND active = 'true' ");
            stb.AppendFormat($"	AND CD.HOME_BASED <> '' {filterTBL} ");
            stb.AppendFormat($"	AND CD.CARGO IS NOT NULL {outerFilters} ) AS CL ON CL.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS ");
            stb.AppendFormat("AND CL.CREATED_AT = R.CREATED_AT ");

            stb.AppendFormat(" ");
            stb.AppendFormat("LEFT JOIN ");
            stb.AppendFormat("  (SELECT SUM(INPUT) - SUM(OUTPUT) AS INPUT, ");
            stb.AppendFormat("          gda_indicator_idgda_indicator, ");
            stb.AppendFormat("          result_date, ");
            stb.AppendFormat("          COLLABORATOR_ID ");
            stb.AppendFormat("   FROM GDA_CHECKING_ACCOUNT ");
            stb.AppendFormat("   WHERE RESULT_DATE >= @DATAINICIAL ");
            stb.AppendFormat("     AND RESULT_DATE <= @DATAFINAL ");

            stb.AppendFormat("   GROUP BY gda_indicator_idgda_indicator, ");
            stb.AppendFormat("            result_date, ");
            stb.AppendFormat("            COLLABORATOR_ID) AS MZ ON MZ.gda_indicator_idgda_indicator = R.INDICADORID ");
            stb.AppendFormat("AND MZ.result_date = R.CREATED_AT ");
            stb.AppendFormat("AND MZ.COLLABORATOR_ID = R.IDGDA_COLLABORATORS ");
            stb.AppendFormat(" ");
            stb.AppendFormat("WHERE 1 = 1 ");
            stb.AppendFormat("  AND R.CREATED_AT >= @DATAINICIAL ");
            stb.AppendFormat("  AND R.CREATED_AT <= @DATAFINAL ");
            stb.AppendFormat("  AND R.DELETED_AT IS NULL ");
            stb.AppendFormat($"  AND CL.{columnName} IS NOT NULL ");

            stb.AppendFormat("  AND R.FACTORS <> '0.000000;0.000000' ");
            stb.AppendFormat("  AND R.FACTORS <> '0.000000; 0.000000' ");
            stb.AppendFormat("  {0}  ", filter);
            stb.AppendFormat($"GROUP BY R.INDICADORID, CL.IDGDA_SECTOR, CL.IDGDA_SUBSECTOR, CL.IDGDA_SECTOR_SUBSECTOR, ");
            stb.AppendFormat("         CONVERT(DATE, R.CREATED_AT), IDGDA_PERIOD ");
            stb.AppendFormat(") AS TBL ");
            stb.AppendFormat("INNER JOIN GDA_INDICATOR (NOLOCK) AS IT ON IT.IDGDA_INDICATOR = TBL.INDICADORID AND IT.DELETED_AT IS NULL AND (IT.STATUS IS NULL OR IT.STATUS = 1) ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
            stb.AppendFormat("AND HIG1.INDICATOR_ID = TBL.INDICADORID ");
            stb.AppendFormat($"AND HIG1.SECTOR_ID = TBL.IDGDA_SECTOR_SUBSECTOR ");
            stb.AppendFormat("AND HIG1.GROUPID = 1 ");
            stb.AppendFormat("AND CONVERT(DATE,HIG1.STARTED_AT) <= TBL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIG1.ENDED_AT) >= TBL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG2 ON HIG2.DELETED_AT IS NULL ");
            stb.AppendFormat("AND HIG2.INDICATOR_ID = TBL.INDICADORID ");
            stb.AppendFormat($"AND HIG2.SECTOR_ID = TBL.IDGDA_SECTOR_SUBSECTOR ");
            stb.AppendFormat("AND HIG2.GROUPID = 2 ");
            stb.AppendFormat("AND CONVERT(DATE,HIG2.STARTED_AT) <= TBL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIG2.ENDED_AT) >= TBL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG3 ON HIG3.DELETED_AT IS NULL ");
            stb.AppendFormat("AND HIG3.INDICATOR_ID = TBL.INDICADORID ");
            stb.AppendFormat($"AND HIG3.SECTOR_ID = TBL.IDGDA_SECTOR_SUBSECTOR ");
            stb.AppendFormat("AND HIG3.GROUPID = 3 ");
            stb.AppendFormat("AND CONVERT(DATE,HIG3.STARTED_AT) <= TBL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIG3.ENDED_AT) >= TBL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG4 ON HIG4.DELETED_AT IS NULL ");
            stb.AppendFormat("AND HIG4.INDICATOR_ID = TBL.INDICADORID ");
            stb.AppendFormat($"AND HIG4.SECTOR_ID = TBL.IDGDA_SECTOR_SUBSECTOR ");
            stb.AppendFormat("AND HIG4.GROUPID = 4 ");
            stb.AppendFormat("AND CONVERT(DATE,HIG4.STARTED_AT) <= TBL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIG4.ENDED_AT) >= TBL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN ");
            stb.AppendFormat("  (SELECT (SUM(GOAL) / COUNT(0)) AS GOAL, ");
            stb.AppendFormat("          (SUM(GOAL_NIGHT) / COUNT(0)) AS GOAL_NIGHT, ");
            stb.AppendFormat("          (SUM(GOAL_LATENIGHT) / COUNT(0)) AS GOAL_LATENIGHT, ");
            stb.AppendFormat("          INDICATOR_ID, SECTOR_ID, ");
            stb.AppendFormat("          CONVERT(DATE,STARTED_AT) AS STARTED_AT, ");
            stb.AppendFormat("          CONVERT(DATE,ENDED_AT) AS ENDED_AT ");
            stb.AppendFormat("   FROM GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) ");

            stb.AppendFormat("   WHERE ((CONVERT(DATE,STARTED_AT) <= @DATAINICIAL AND CONVERT(DATE,ENDED_AT) >= @DATAINICIAL) ");
            stb.AppendFormat("     OR  (CONVERT(DATE,STARTED_AT) <= @DATAFINAL AND CONVERT(DATE,ENDED_AT) >= @DATAFINAL)) ");
            stb.AppendFormat($"     AND DELETED_AT IS NULL ");

            stb.AppendFormat("   GROUP BY SECTOR_ID, INDICATOR_ID, CONVERT(DATE,STARTED_AT), CONVERT(DATE,ENDED_AT)) AS HIS ON HIS.SECTOR_ID = TBL.IDGDA_SECTOR_SUBSECTOR AND HIS.INDICATOR_ID = TBL.INDICADORID ");
            stb.AppendFormat("AND CONVERT(DATE,HIS.STARTED_AT) <= TBL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIS.ENDED_AT) >= TBL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME ON HME.INDICATORID = TBL.INDICADORID ");
            stb.AppendFormat("AND HME.deleted_at IS NULL ");
            stb.AppendFormat("LEFT JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON ME.ID = HME.MATHEMATICALEXPRESSIONID ");
            stb.AppendFormat("AND ME.DELETED_AT IS NULL ");
            stb.AppendFormat($"LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = TBL.{columnName} ");

            stb.AppendFormat("GROUP BY TBL.INDICADORID, ");
            stb.AppendFormat("         CONVERT(DATE, TBL.CREATED_AT) ");



            List<HomeResultConsolidated> Listhrc = new List<HomeResultConsolidated>();

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                {
                    command.CommandTimeout = 0;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                HomeResultConsolidated hrc = new HomeResultConsolidated();
                                hrc.MATRICULA = idCol;
                                //hrc.CARGO = reader["CARGO"].ToString();
                                hrc.CODGIP = reader[columnName].ToString();
                                //hrc.SETOR = reader["SETOR"].ToString();
                                hrc.IDINDICADOR = reader["IDGDA_INDICATOR"].ToString();
                                hrc.INDICADOR = reader["INDICADOR"].ToString();
                                //hrc.META = Double.Parse(reader["META"].ToString());


                                //Regra Nova Descomentar
                                if (reader["META"] != DBNull.Value && Double.Parse(reader["QTD_META"].ToString()) > 0)
                                {

                                    hrc.META = Double.Parse(reader["META"].ToString());
                                    //hrc.META = Math.Round(Double.Parse(reader["META"].ToString()) / Double.Parse(reader["QTD_META"].ToString()), 1);
                                }
                                else
                                {
                                    hrc.META = 0;
                                }

                                //hrc.META = reader["META"] != DBNull.Value ? double.Parse(reader["META"].ToString()) : 0;


                                hrc.min1 = reader["min1"] != DBNull.Value && Double.Parse(reader["QTD_META"].ToString()) > 0 ? double.Parse(reader["min1"].ToString()) : 0;
                                hrc.min2 = reader["min2"] != DBNull.Value && Double.Parse(reader["QTD_META"].ToString()) > 0 ? double.Parse(reader["min2"].ToString()) : 0;
                                hrc.min3 = reader["min3"] != DBNull.Value && Double.Parse(reader["QTD_META"].ToString()) > 0 ? double.Parse(reader["min3"].ToString()) : 0;
                                hrc.min4 = reader["min4"] != DBNull.Value && Double.Parse(reader["QTD_META"].ToString()) > 0 ? double.Parse(reader["min4"].ToString()) : 0;
                                hrc.CONTA = reader["CONTA"].ToString();
                                hrc.BETTER = reader["BETTER"].ToString();
                                hrc.FACTOR0 = reader["FACTOR0"].ToString() != "" ? double.Parse(reader["FACTOR0"].ToString()) : 0;
                                hrc.FACTOR1 = reader["FACTOR1"].ToString() != "" ? double.Parse(reader["FACTOR1"].ToString()) : 0;

                                //hrc.META_MAXIMA_MOEDAS = reader["META_MAXIMA_MOEDA"].ToString() != "" ? double.Parse(reader["META_MAXIMA_MOEDA"].ToString()) : 0; 

                                //if (reader["turno"].ToString() == "DIURNO")
                                //{

                                //    hrc.min1 = reader["MIN1"].ToString() != "" ? double.Parse(reader["MIN1"].ToString()) : 0;
                                //    hrc.min2 = reader["MIN2"].ToString() != "" ? double.Parse(reader["MIN2"].ToString()) : 0;
                                //    hrc.min3 = reader["MIN3"].ToString() != "" ? double.Parse(reader["MIN3"].ToString()) : 0;
                                //    hrc.min4 = reader["MIN4"].ToString() != "" ? double.Parse(reader["MIN4"].ToString()) : 0;
                                //    hrc.META = reader["META"].ToString() != "" ? double.Parse(reader["META"].ToString()) : 0;
                                //    hrc.META_MAXIMA_MOEDAS = reader["META_MAXIMA"].ToString() != "" ? int.Parse(reader["META_MAXIMA"].ToString()) : 0;
                                //}
                                //else if (reader["turno"].ToString() == "NOTURNO")
                                //{

                                //    hrc.min1 = reader["MIN1_NOTURNO"].ToString() != "" ? double.Parse(reader["MIN1_NOTURNO"].ToString()) : 0;
                                //    hrc.min2 = reader["MIN2_NOTURNO"].ToString() != "" ? double.Parse(reader["MIN2_NOTURNO"].ToString()) : 0;
                                //    hrc.min3 = reader["MIN3_NOTURNO"].ToString() != "" ? double.Parse(reader["MIN3_NOTURNO"].ToString()) : 0;
                                //    hrc.min4 = reader["MIN4_NOTURNO"].ToString() != "" ? double.Parse(reader["MIN4_NOTURNO"].ToString()) : 0;
                                //    hrc.META = reader["META_NOTURNA"].ToString() != "" ? double.Parse(reader["META_NOTURNA"].ToString()) : 0;
                                //    hrc.META_MAXIMA_MOEDAS = reader["META_MAXIMA_NOTURNA"].ToString() != "" ? int.Parse(reader["META_MAXIMA_NOTURNA"].ToString()) : 0;
                                //}
                                //else if (reader["turno"].ToString() == "MADRUGADA")
                                //{

                                //    hrc.min1 = reader["MIN1_MADRUGADA"].ToString() != "" ? double.Parse(reader["MIN1_MADRUGADA"].ToString()) : 0;
                                //    hrc.min2 = reader["MIN2_MADRUGADA"].ToString() != "" ? double.Parse(reader["MIN2_MADRUGADA"].ToString()) : 0;
                                //    hrc.min3 = reader["MIN3_MADRUGADA"].ToString() != "" ? double.Parse(reader["MIN3_MADRUGADA"].ToString()) : 0;
                                //    hrc.min4 = reader["MIN4_MADRUGADA"].ToString() != "" ? double.Parse(reader["MIN4_MADRUGADA"].ToString()) : 0;
                                //    hrc.META = reader["META_MADRUGADA"].ToString() != "" ? double.Parse(reader["META_MADRUGADA"].ToString()) : 0;
                                //    hrc.META_MAXIMA_MOEDAS = reader["META_MAXIMA_MADRUGADA"].ToString() != "" ? int.Parse(reader["META_MAXIMA_MADRUGADA"].ToString()) : 0;
                                //}
                                //else if (reader["turno"].ToString() == "" || reader["turno"].ToString() == null)
                                //{

                                //    hrc.min1 = 0;
                                //    hrc.min2 = 0;
                                //    hrc.min3 = 0;
                                //    hrc.min4 = 0;
                                //    hrc.META = 0;
                                //    hrc.META_MAXIMA_MOEDAS = 0;
                                //}

                                hrc.MOEDA_GANHA = reader["MOEDA_GANHA"].ToString() != "" ? double.Parse(reader["MOEDA_GANHA"].ToString()) : 0;
                                hrc.QTD_MON = reader["QTD_MON"].ToString() != "" ? double.Parse(reader["QTD_MON"].ToString()) : 0;
                                hrc.QTD_MON_TOTAL = reader["QTD_MON_TOTAL"].ToString() != "" ? double.Parse(reader["QTD_MON_TOTAL"].ToString()) : 0;
                                hrc.QTD_META = reader["QTD_META"].ToString() != "" ? double.Parse(reader["QTD_META"].ToString()) : 0;
                                hrc.QTD = Double.Parse(reader["QTD"].ToString());
                                hrc.DATAPAGAMENTO = reader["CREATED_AT"].ToString();
                                hrc.TYPE = reader["TYPE"].ToString();

                                hrc.MOEDA_GANHA = hrc.MOEDA_GANHA > 0 && hrc.QTD_MON > 0 ? hrc.MOEDA_GANHA / hrc.QTD_MON : 0;

                                Listhrc.Add(hrc);
                            }
                            catch (Exception)
                            {

                            }

                        }
                    }
                }

                connection.Close();
            }
            return Listhrc;
        }

        public static HomeResultConsolidated DoCalculateFinal(HomeResultConsolidated consolidado)
        {
            try
            {
                List<groups> lgroup = returnTables.listGroups("");
                groups lgroup1 = lgroup.Find(l => l.id == 1);
                groups lgroup2 = lgroup.Find(l => l.id == 2);
                groups lgroup3 = lgroup.Find(l => l.id == 3);
                groups lgroup4 = lgroup.Find(l => l.id == 4);


                if (consolidado.IDINDICADOR == "2")
                {
                    var parou = true;
                }

                string contaAg = "";
                if (consolidado.IDINDICADOR == "10000013")
                {
                    //Calculo do Agente
                    contaAg = $"{consolidado.SUMDIASLOGADOSESCALADOS} / {consolidado.SUMDIASESCALADOS}";
                    //consolidado.META = consolidado.SUMDIASESCALADOS;
                }
                else if (consolidado.IDINDICADOR == "10000014")
                {
                    //Calculo do Agente
                    contaAg = $"{consolidado.SUMDIASLOGADOS} / {consolidado.SUMDIASESCALADOS}";
                    //consolidado.META = consolidado.SUMDIASESCALADOS;
                }
                else
                {
                    contaAg = consolidado.CONTA.Replace("#fator0", consolidado.FACTOR0.ToString()).Replace("#fator1", consolidado.FACTOR1.ToString());
                }



                //Realiza a conta de resultado
                DataTable dt = new DataTable();
                double resultado = 0;
                var result = dt.Compute(contaAg, "").ToString();
                resultado = double.Parse(result);
                if (resultado == double.PositiveInfinity)
                {
                    resultado = 0;
                }
                if (double.IsNaN(resultado))
                {
                    resultado = 0;
                }

                if (consolidado.CARGO != "AGENTE")
                {
                    if (consolidado.META == 0)
                    {
                        consolidado.META = 0;
                    }
                    //else if (consolidado.QTD > 0)
                    //{
                    //    consolidado.META = consolidado.META / consolidado.QTD;
                    //    consolidado.META = Math.Round(consolidado.META, 2, MidpointRounding.AwayFromZero);
                    //}

                    if (double.IsNaN(resultado))
                    {
                        consolidado.GRUPO = "Bronze";
                        consolidado.PERCENTUAL = 0;
                        consolidado.RESULTADO = 0;

                        //retorno[i] = consolidado;
                    }

                    if (resultado == 0 && (consolidado.IDINDICADOR == "191" || consolidado.IDINDICADOR == "371" || consolidado.IDINDICADOR == "193"))
                    {
                        consolidado.GRUPO = "Bronze";
                        consolidado.PERCENTUAL = 0;
                        consolidado.RESULTADO = 0;
                    }

                    //Regra do TMA. Quando o arredondamento tambem der 0, não monetizar e considerar bronze
                    double arredondResult = Math.Round(resultado, 0, MidpointRounding.AwayFromZero);
                    if (arredondResult == 0 && (consolidado.IDINDICADOR == "191" || consolidado.IDINDICADOR == "371" || consolidado.IDINDICADOR == "193"))
                    {
                        consolidado.GRUPO = "Bronze";
                        consolidado.PERCENTUAL = 0;
                        consolidado.RESULTADO = 0;

                        //retorno[i] = consolidado;

                    }

                    double resultadoD = resultado;

                    if (consolidado.TYPE == null)
                    {
                        resultadoD = resultadoD * 100;
                    }
                    else if (consolidado.TYPE == "PERCENT")
                    {
                        resultadoD = resultadoD * 100;
                    }
                    double atingimentoMeta = 0;
                    //Verifica se é melhor ou menor melhor
                    if (consolidado.BETTER == "BIGGER_BETTER")
                    {
                        if (consolidado.META == 0)
                        {
                            atingimentoMeta = 0;
                        }
                        else
                        {
                            atingimentoMeta = resultadoD / consolidado.META;
                        }
                    }
                    else
                    {
                        // No caso de menor melhor, quando o denominador é 0 não esta sendo possivel fazer a conta de divisão por 0.
                        // E como é menor melhor, logo 0 é um resultado de 100% de atingimento.

                        //double resultadoRetorno = Math.Round(resultadoD / 100, 0);

                        //if (consolidado.META < 1 && resultadoRetorno == 0 && consolidado.IDINDICADOR == "2")
                        //{
                        //    atingimentoMeta = 10;
                        //}
                        //else 
                        if (resultadoD == 0) // && factor.goal < 1
                        {
                            atingimentoMeta = 10;
                        }
                        else
                        {
                            atingimentoMeta = (consolidado.META / resultadoD);

                        }
                    }

                    atingimentoMeta = atingimentoMeta * 100;



                    //Verifica a qual grupo pertence

                    if (atingimentoMeta >= consolidado.min1)
                    {
                        //consolidado.GRUPO = "Diamante";
                        consolidado.GRUPO = lgroup1.name;
                        consolidado.IDGRUPO = lgroup1.id;
                        consolidado.IMAGEMGRUPO = lgroup1.image;
                    }
                    else if (atingimentoMeta >= consolidado.min2)
                    {
                        //consolidado.GRUPO = "Ouro";
                        consolidado.GRUPO = lgroup2.name;
                        consolidado.IDGRUPO = lgroup2.id;
                        consolidado.IMAGEMGRUPO = lgroup2.image;
                    }
                    else if (atingimentoMeta >= consolidado.min3)
                    {
                        //consolidado.GRUPO = "Prata";
                        consolidado.GRUPO = lgroup3.name;
                        consolidado.IDGRUPO = lgroup3.id;
                        consolidado.IMAGEMGRUPO = lgroup3.image;
                    }
                    else if (atingimentoMeta >= consolidado.min4)
                    {
                        //consolidado.GRUPO = "Bronze";
                        consolidado.GRUPO = lgroup4.name;
                        consolidado.IDGRUPO = lgroup4.id;
                        consolidado.IMAGEMGRUPO = lgroup4.image;
                    }
                    else
                    {
                        //consolidado.GRUPO = "Bronze";
                        consolidado.GRUPO = lgroup4.name;
                        consolidado.IDGRUPO = lgroup4.id;
                        consolidado.IMAGEMGRUPO = lgroup4.image;
                    }

                    if (consolidado.TYPE == "INTEGER" || consolidado.TYPE == "HOUR")
                    {
                        consolidado.PERCENTUAL = atingimentoMeta;
                        consolidado.RESULTADO = Math.Round(resultadoD, 0, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        consolidado.PERCENTUAL = atingimentoMeta;
                        consolidado.RESULTADO = resultadoD;
                    }

                }
            }
            catch (Exception)
            {

            }


            return consolidado;
        }

        public static List<HomeResultConsolidated> ReturnHomeResultConsolidatedAccess(string idCol, string dtinicial, string dtfinal, string hierarchies, string sectors, string subsectors, string periods, string homefloors, string sites, bool? bkt, string indicatorsAsString)
        {
            string filter = "";
            HomeResultConsolidated rmams = new HomeResultConsolidated();
            rmams.MATRICULA = idCol;

            string filterHis = "";

            //COLABORADOR
            if (hierarchies.ToString().Contains("1"))
            {
                filter = "AND CL.IDGDA_COLLABORATORS = @INPUTID";
            }
            //SUPERVISOR
            if (hierarchies.ToString().Contains("2"))
            {
                filter = "AND CL.MATRICULA_SUPERVISOR = @INPUTID";
            }
            //COORDENADOR
            if (hierarchies.ToString().Contains("3"))
            {
                filter = "AND CL.MATRICULA_COORDENADOR = @INPUTID";
            }
            //GERENTE_II
            if (hierarchies.ToString().Contains("4"))
            {
                filter = "AND CL.MATRICULA_GERENTE_II = @INPUTID";
            }
            //GERENTE_I
            if (hierarchies.ToString().Contains("5"))
            {
                filter = "AND CL.MATRICULA_GERENTE_I = @INPUTID";
            }
            //DIRETOR
            if (hierarchies.ToString().Contains("6"))
            {
                filter = "AND CL.MATRICULA_DIRETOR = @INPUTID";
            }
            //CEO
            if (hierarchies.ToString().Contains("7"))
            {
                filter = "AND CL.MATRICULA_CEO = @INPUTID";
            }
            //SETORES
            //if (sectors != "")
            //{
            //    filter = filter + $" AND CL.IDGDA_SECTOR IN ({sectors}) ";
            //}

            string filterTBL = "";
            //Periodos
            if (periods != "")
            {
                filterTBL = filterTBL + $" AND IDGDA_PERIOD IN ({periods}) ";
            }

            //Home ou Piso
            if (homefloors != "")
            {
                string hf = homefloors.Replace("1", "'SIM'").Replace("2", "'NÃO'");

                filterTBL = filterTBL + $" AND HOME_BASED IN ({hf}) ";
            }

            //Site
            if (sites != "")
            {
                filterTBL = filterTBL + $" AND S.IDGDA_SITE IN ({sites}) ";
            }

            //SETORES
            string columnName = "";
            if ((sectors != "" || subsectors != "")) // && bkt == true)
            {
                if (subsectors != "")
                {
                    columnName = "IDGDA_SUBSECTOR";
                    filter = filter + $" AND CL.IDGDA_SECTOR IN ({sectors}) AND CL.IDGDA_SUBSECTOR IN ({subsectors}) ";
                    filterHis = filterHis + $" AND SECTOR_ID IN ({subsectors}) ";
                }
                else if (sectors != "")
                {
                    columnName = "IDGDA_SECTOR";
                    filter = filter + $" AND CL.IDGDA_SECTOR IN ({sectors}) ";
                    filterHis = filterHis + $" AND SECTOR_ID IN ({sectors}) ";
                }
            }
            else
            {
                columnName = "IDGDA_SECTOR";
            }

            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtinicial);
            stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtfinal);
            stb.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}'; ", idCol);

            stb.AppendFormat("SELECT TBl.CREATED_AT, ");
            stb.AppendFormat("       SUM(QTD) AS QTD, ");
            stb.AppendFormat("       SUM(CASE ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 1 ");
            stb.AppendFormat("                    AND HIG1.MONETIZATION > 0 THEN QTD ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 2 ");
            stb.AppendFormat("                    AND HIG1.MONETIZATION_NIGHT > 0 THEN QTD ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 3 ");
            stb.AppendFormat("                    AND HIG1.MONETIZATION_LATENIGHT > 0 THEN QTD ");
            stb.AppendFormat("               ELSE 0 ");
            stb.AppendFormat("           END) AS QTD_MON, ");
            stb.AppendFormat("       SUM(CASE ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 1 ");
            stb.AppendFormat("                    AND HIG1.MONETIZATION > 0 THEN (HIG1.MONETIZATION * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 2 ");
            stb.AppendFormat("                    AND HIG1.MONETIZATION_NIGHT > 0 THEN (HIG1.MONETIZATION_NIGHT * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 3 ");
            stb.AppendFormat("                    AND HIG1.MONETIZATION_LATENIGHT > 0 THEN (HIG1.MONETIZATION_LATENIGHT * QTD) ");
            stb.AppendFormat("               ELSE 0 ");
            stb.AppendFormat("           END) AS QTD_MON_TOTAL, ");
            stb.AppendFormat("       MAX(TBL.IDGDA_SECTOR) AS IDGDA_SECTOR, ");
            stb.AppendFormat("       MAX(SEC.NAME) AS SETOR, ");
            stb.AppendFormat("       MAX(IT.IDGDA_INDICATOR) AS IDGDA_INDICATOR, ");
            stb.AppendFormat("       MAX(IT.NAME) AS 'INDICADOR', ");

            stb.AppendFormat(" 		   SUM(CASE ");
            stb.AppendFormat("              WHEN TBL.IDGDA_PERIOD = 1 ");
            stb.AppendFormat("                  AND HIS.GOAL IS NOT NULL THEN (QTD) ");
            stb.AppendFormat("              WHEN TBL.IDGDA_PERIOD = 2 ");
            stb.AppendFormat("                  AND HIS.GOAL_NIGHT > 0 THEN (QTD) ");
            stb.AppendFormat("              WHEN TBL.IDGDA_PERIOD = 3 ");
            stb.AppendFormat("                  AND HIS.GOAL_LATENIGHT > 0 THEN (QTD) ");
            stb.AppendFormat("              ELSE 0 ");
            stb.AppendFormat("          END) AS QTD_META, ");

            stb.AppendFormat("       SUM(CASE ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 1 THEN (HIS.GOAL * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 2 THEN (HIS.GOAL_NIGHT * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 3 THEN (HIS.GOAL_LATENIGHT * QTD) ");
            stb.AppendFormat("               ELSE 0 ");
            stb.AppendFormat("           END) AS META, ");
            stb.AppendFormat("       SUM(CASE ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 1 THEN (HIG1.METRIC_MIN * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 2 THEN (HIG1.METRIC_MIN_NIGHT * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 3 THEN (HIG1.METRIC_MIN_LATENIGHT * QTD) ");
            stb.AppendFormat("               ELSE 0 ");
            stb.AppendFormat("           END) AS MIN1, ");
            stb.AppendFormat("       SUM(CASE ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 1 THEN (HIG2.METRIC_MIN * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 2 THEN (HIG2.METRIC_MIN_NIGHT * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 3 THEN (HIG2.METRIC_MIN_LATENIGHT * QTD) ");
            stb.AppendFormat("               ELSE 0 ");
            stb.AppendFormat("           END) AS MIN2, ");
            stb.AppendFormat("       SUM(CASE ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 1 THEN (HIG3.METRIC_MIN * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 2 THEN (HIG3.METRIC_MIN_NIGHT * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 3 THEN (HIG3.METRIC_MIN_LATENIGHT * QTD) ");
            stb.AppendFormat("               ELSE 0 ");
            stb.AppendFormat("           END) AS MIN3, ");
            stb.AppendFormat("       SUM(CASE ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 1 THEN (HIG4.METRIC_MIN * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 2 THEN (HIG4.METRIC_MIN_NIGHT * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 3 THEN (HIG4.METRIC_MIN_LATENIGHT * QTD) ");
            stb.AppendFormat("               ELSE 0 ");
            stb.AppendFormat("           END) AS MIN4, ");
            stb.AppendFormat("       CASE ");
            stb.AppendFormat("           WHEN MAX(ME.EXPRESSION) IS NULL THEN '(#FATOR1/#FATOR0)' ");
            stb.AppendFormat("           ELSE MAX(ME.EXPRESSION) ");
            stb.AppendFormat("       END AS CONTA, ");
            stb.AppendFormat("       CASE ");
            stb.AppendFormat("           WHEN MAX(IT.CALCULATION_TYPE) IS NULL THEN 'BIGGER_BETTER' ");
            stb.AppendFormat("           ELSE MAX(IT.CALCULATION_TYPE) ");
            stb.AppendFormat("       END AS BETTER, ");
            stb.AppendFormat("       MAX(IT.TYPE) AS TYPE, ");
            stb.AppendFormat("       SUM(TBL.FACTOR0) AS FACTOR0, ");
            stb.AppendFormat("       SUM(TBL.FACTOR1) AS FACTOR1, ");
            stb.AppendFormat("       SUM(TBL.MOEDA_GANHA) AS MOEDA_GANHA, ");
            stb.AppendFormat("	   SUM(TBL.LOGADO) AS LOGADO, ");
            stb.AppendFormat("	   SUM(TBL.ESCALADO) AS ESCALADO, ");
            stb.AppendFormat("	   SUM(TBL.ESCALADOLOGADO) AS ESCALADOLOGADO ");
            stb.AppendFormat("FROM ");
            stb.AppendFormat("  (SELECT CONVERT(DATE, CL.CREATED_AT) AS CREATED_AT, ");
            stb.AppendFormat("          COUNT(0) AS QTD, ");
            stb.AppendFormat("          CL.IDGDA_PERIOD AS IDGDA_PERIOD, ");
            stb.AppendFormat("          CL.IDGDA_SECTOR AS IDGDA_SECTOR, ");
            stb.AppendFormat("          CL.IDGDA_SUBSECTOR AS IDGDA_SUBSECTOR, ");
            stb.AppendFormat("          CL.IDGDA_SECTOR_SUBSECTOR AS IDGDA_SECTOR_SUBSECTOR, ");
            stb.AppendFormat("          IT.IDGDA_INDICATOR AS INDICADORID, ");
            stb.AppendFormat("          CASE ");
            stb.AppendFormat("              WHEN @INPUTID = MAX(CL.IDGDA_COLLABORATORS) THEN 'AGENTE' ");
            stb.AppendFormat("              WHEN @INPUTID = MAX(CL.MATRICULA_SUPERVISOR) THEN 'SUPERVISOR' ");
            stb.AppendFormat("              WHEN @INPUTID = MAX(CL.MATRICULA_COORDENADOR) THEN 'COORDENADOR' ");
            stb.AppendFormat("              WHEN @INPUTID = MAX(CL.MATRICULA_GERENTE_II) THEN 'GERENTE_II' ");
            stb.AppendFormat("              WHEN @INPUTID = MAX(CL.MATRICULA_GERENTE_I) THEN 'GERENTE_I' ");
            stb.AppendFormat("              WHEN @INPUTID = MAX(CL.MATRICULA_DIRETOR) THEN 'DIRETOR' ");
            stb.AppendFormat("              WHEN @INPUTID = MAX(CL.MATRICULA_CEO) THEN 'CEO' ");
            stb.AppendFormat("              ELSE '' ");
            stb.AppendFormat("          END AS CARGO, ");
            stb.AppendFormat("          0 AS FACTOR0, ");
            stb.AppendFormat("          0 AS FACTOR1, ");
            stb.AppendFormat("          SUM(MZ.INPUT) AS MOEDA_GANHA, ");
            stb.AppendFormat("		    SUM(ESC.ESCALADO) AS ESCALADO, ");
            stb.AppendFormat("          SUM(LOG.LOGIN) AS LOGADO,  ");
            stb.AppendFormat("		  SUM(ESCLOG.ESCALADOLOGADO) AS ESCALADOLOGADO ");
            stb.AppendFormat("   FROM (SELECT CD.IDGDA_SECTOR, ");
            stb.AppendFormat("             CD.IDGDA_SUBSECTOR, ");

            stb.AppendFormat("   CASE WHEN CD.IDGDA_SUBSECTOR IS NOT NULL THEN CD.IDGDA_SUBSECTOR ");
            stb.AppendFormat("   ELSE CD.IDGDA_SECTOR END AS IDGDA_SECTOR_SUBSECTOR, ");

            stb.AppendFormat("             CD.CREATED_AT, ");
            stb.AppendFormat("             CD.IDGDA_COLLABORATORS, ");
            stb.AppendFormat("             CD.IDGDA_SECTOR_SUPERVISOR, ");
            stb.AppendFormat("             CD.ACTIVE, ");
            stb.AppendFormat("             CD.CARGO, ");
            stb.AppendFormat("             CD.HOME_BASED, ");
            stb.AppendFormat("             CD.SITE, ");
            stb.AppendFormat("             CD.IDGDA_PERIOD, ");
            stb.AppendFormat("             CD.MATRICULA_SUPERVISOR, ");
            stb.AppendFormat("             CD.NOME_SUPERVISOR, ");
            stb.AppendFormat("             CD.MATRICULA_COORDENADOR, ");
            stb.AppendFormat("             CD.NOME_COORDENADOR, ");
            stb.AppendFormat("             CD.MATRICULA_GERENTE_II, ");
            stb.AppendFormat("             CD.NOME_GERENTE_II, ");
            stb.AppendFormat("             CD.MATRICULA_GERENTE_I, ");
            stb.AppendFormat("             CD.NOME_GERENTE_I, ");
            stb.AppendFormat("             CD.MATRICULA_DIRETOR, ");
            stb.AppendFormat("             CD.NOME_DIRETOR, ");
            stb.AppendFormat("             CD.MATRICULA_CEO, ");
            stb.AppendFormat("             CD.NOME_CEO ");
            stb.AppendFormat("      FROM GDA_COLLABORATORS_DETAILS (NOLOCK) CD ");
            stb.AppendFormat("      LEFT JOIN GDA_SITE (NOLOCK) S ON CD.SITE = S.SITE ");
            stb.AppendFormat("      WHERE CD.CREATED_AT >= @DATAINICIAL ");
            stb.AppendFormat($"        AND CD.CREATED_AT <= @DATAFINAL {filterTBL} ) AS CL ");
            stb.AppendFormat($"   INNER JOIN GDA_INDICATOR (NOLOCK) AS IT ON IT.IDGDA_INDICATOR IN ({indicatorsAsString}) AND IT.DELETED_AT IS NULL AND (IT.STATUS IS NULL OR IT.STATUS = 1) ");//
            stb.AppendFormat("   LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS CB ON CB.IDGDA_COLLABORATORS = CL.IDGDA_COLLABORATORS ");
            stb.AppendFormat("   LEFT JOIN ");
            stb.AppendFormat("     (SELECT SUM(INPUT) - SUM(OUTPUT) AS INPUT, ");
            stb.AppendFormat("             gda_indicator_idgda_indicator, ");
            stb.AppendFormat("             result_date, ");
            stb.AppendFormat("             COLLABORATOR_ID ");
            stb.AppendFormat("      FROM GDA_CHECKING_ACCOUNT ");
            stb.AppendFormat("      WHERE RESULT_DATE >= @DATAINICIAL ");
            stb.AppendFormat("        AND RESULT_DATE <= @DATAFINAL ");
            //stb.AppendFormat("        AND COLLABORATOR_ID = @INPUTID ");
            stb.AppendFormat("      GROUP BY gda_indicator_idgda_indicator, ");
            stb.AppendFormat("               result_date, ");
            stb.AppendFormat("               COLLABORATOR_ID) AS MZ ON MZ.gda_indicator_idgda_indicator = IT.IDGDA_INDICATOR ");
            stb.AppendFormat("   AND MZ.result_date = CL.CREATED_AT ");
            stb.AppendFormat("   AND MZ.COLLABORATOR_ID = CL.IDGDA_COLLABORATORS ");
            stb.AppendFormat("LEFT JOIN  ");
            stb.AppendFormat("  (SELECT COUNT(0) AS 'ESCALADO',  ");
            stb.AppendFormat("          IDGDA_COLLABORATORS,  ");
            stb.AppendFormat("          CREATED_AT ");
            stb.AppendFormat("   FROM GDA_RESULT (NOLOCK)  ");
            stb.AppendFormat("   WHERE INDICADORID = -1  ");
            stb.AppendFormat("     AND CREATED_AT >= @DATAINICIAL  ");
            stb.AppendFormat("     AND CREATED_AT <= @DATAFINAL  ");
            stb.AppendFormat("     AND RESULT = 1  ");
            stb.AppendFormat("     AND DELETED_AT IS NULL ");
            stb.AppendFormat("   GROUP BY IDGDA_COLLABORATORS,  ");
            stb.AppendFormat("            CREATED_AT) AS ESC ON ESC.IDGDA_COLLABORATORS = CL.IDGDA_COLLABORATORS  ");
            stb.AppendFormat("AND ESC.CREATED_AT = CL.CREATED_AT  ");
            stb.AppendFormat("LEFT JOIN  ");
            stb.AppendFormat("  (SELECT COUNT(DISTINCT IDGDA_COLLABORATOR) AS 'LOGIN',  ");
            stb.AppendFormat("          IDGDA_COLLABORATOR,  ");
            stb.AppendFormat("          CONVERT(DATE, DATE_ACCESS) AS CREATED_AT  ");
            stb.AppendFormat("   FROM GDA_LOGIN_ACCESS (NOLOCK)  ");
            stb.AppendFormat("   WHERE CONVERT(DATE, DATE_ACCESS) >= @DATAINICIAL  ");
            stb.AppendFormat("     AND CONVERT(DATE, DATE_ACCESS) <= @DATAFINAL  ");
            stb.AppendFormat("   GROUP BY IDGDA_COLLABORATOR,  ");
            stb.AppendFormat("            CONVERT(DATE, DATE_ACCESS)) AS LOG ON LOG.IDGDA_COLLABORATOR = CL.IDGDA_COLLABORATORS  ");
            stb.AppendFormat("AND LOG.CREATED_AT = CL.CREATED_AT  ");
            stb.AppendFormat("LEFT JOIN  ");
            stb.AppendFormat("  (SELECT COUNT(0) AS 'ESCALADOLOGADO',  ");
            stb.AppendFormat("          IDGDA_COLLABORATORS,  ");
            stb.AppendFormat("          CREATED_AT  ");
            stb.AppendFormat("   FROM GDA_RESULT (NOLOCK) R ");
            stb.AppendFormat("   INNER JOIN GDA_LOGIN_ACCESS (NOLOCK) L ON CONVERT(DATE, DATE_ACCESS) = CREATED_AT AND L.IDGDA_COLLABORATOR = R.IDGDA_COLLABORATORS ");
            stb.AppendFormat("   WHERE INDICADORID = -1  ");
            stb.AppendFormat("     AND CREATED_AT >= @DATAINICIAL  ");
            stb.AppendFormat("     AND CREATED_AT <= @DATAFINAL  ");
            stb.AppendFormat("     AND RESULT = 1  ");
            stb.AppendFormat("     AND DELETED_AT IS NULL  ");
            stb.AppendFormat("   GROUP BY IDGDA_COLLABORATORS,  ");
            stb.AppendFormat("            CREATED_AT) AS ESCLOG ON ESCLOG.IDGDA_COLLABORATORS = CL.IDGDA_COLLABORATORS  ");
            stb.AppendFormat("			AND ESCLOG.CREATED_AT = CL.CREATED_AT  ");
            stb.AppendFormat(" ");
            stb.AppendFormat("   WHERE 1 = 1 ");
            stb.AppendFormat("     AND CL.CREATED_AT >= @DATAINICIAL ");
            stb.AppendFormat("     AND CL.CREATED_AT <= @DATAFINAL ");
            stb.AppendFormat($"     AND CL.{columnName} IS NOT NULL ");
            stb.AppendFormat("     AND CL.CARGO IS NOT NULL ");
            stb.AppendFormat($"     AND CL.HOME_BASED <> ''  ");
            //stb.AppendFormat("     AND CL.ACTIVE = 'true'  ");
            stb.AppendFormat("     AND CL.CARGO = 'AGENTE' ");
            stb.AppendFormat("  {0}  ", filter);
            stb.AppendFormat("   GROUP BY IT.IDGDA_INDICATOR, ");
            stb.AppendFormat($"            CL.IDGDA_SECTOR, CL.IDGDA_SUBSECTOR, CL.IDGDA_SECTOR_SUBSECTOR, ");
            stb.AppendFormat("            CONVERT(DATE, CL.CREATED_AT), ");
            stb.AppendFormat("            IDGDA_PERIOD) AS TBL ");
            stb.AppendFormat("INNER JOIN GDA_INDICATOR (NOLOCK) AS IT ON IT.IDGDA_INDICATOR = TBL.INDICADORID AND (IT.STATUS IS NULL OR IT.STATUS = 1) ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
            stb.AppendFormat("AND HIG1.INDICATOR_ID = TBL.INDICADORID ");
            stb.AppendFormat($"AND HIG1.SECTOR_ID = TBL.IDGDA_SECTOR_SUBSECTOR ");
            stb.AppendFormat("AND HIG1.GROUPID = 1 ");
            stb.AppendFormat("AND CONVERT(DATE,HIG1.STARTED_AT) <= TBL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIG1.ENDED_AT) >= TBL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG2 ON HIG2.DELETED_AT IS NULL ");
            stb.AppendFormat("AND HIG2.INDICATOR_ID = TBL.INDICADORID ");
            stb.AppendFormat($"AND HIG2.SECTOR_ID = TBL.IDGDA_SECTOR_SUBSECTOR ");
            stb.AppendFormat("AND HIG2.GROUPID = 2 ");
            stb.AppendFormat("AND CONVERT(DATE,HIG2.STARTED_AT) <= TBL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIG2.ENDED_AT) >= TBL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG3 ON HIG3.DELETED_AT IS NULL ");
            stb.AppendFormat("AND HIG3.INDICATOR_ID = TBL.INDICADORID ");
            stb.AppendFormat($"AND HIG3.SECTOR_ID = TBL.IDGDA_SECTOR_SUBSECTOR ");
            stb.AppendFormat("AND HIG3.GROUPID = 3 ");
            stb.AppendFormat("AND CONVERT(DATE,HIG3.STARTED_AT) <= TBL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIG3.ENDED_AT) >= TBL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG4 ON HIG4.DELETED_AT IS NULL ");
            stb.AppendFormat("AND HIG4.INDICATOR_ID = TBL.INDICADORID ");
            stb.AppendFormat($"AND HIG4.SECTOR_ID = TBL.IDGDA_SECTOR_SUBSECTOR ");
            stb.AppendFormat("AND HIG4.GROUPID = 4 ");
            stb.AppendFormat("AND CONVERT(DATE,HIG4.STARTED_AT) <= TBL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIG4.ENDED_AT) >= TBL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN ");
            stb.AppendFormat("  (SELECT (SUM(GOAL) / COUNT(0)) AS GOAL, ");
            stb.AppendFormat("          (SUM(GOAL_NIGHT) / COUNT(0)) AS GOAL_NIGHT, ");
            stb.AppendFormat("          (SUM(GOAL_LATENIGHT) / COUNT(0)) AS GOAL_LATENIGHT, ");
            stb.AppendFormat("          INDICATOR_ID, ");
            stb.AppendFormat("          MAX(STARTED_AT) AS STARTED_AT, ");
            stb.AppendFormat("          MAX(ENDED_AT) AS ENDED_AT ");
            stb.AppendFormat("   FROM GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) ");
            stb.AppendFormat("   WHERE CONVERT(DATE,STARTED_AT) <= @DATAINICIAL ");
            stb.AppendFormat("     AND CONVERT(DATE,ENDED_AT) >= @DATAFINAL ");
            stb.AppendFormat($"     AND DELETED_AT IS NULL {filterHis} ");
            stb.AppendFormat("   GROUP BY INDICATOR_ID) AS HIS ON HIS.INDICATOR_ID = TBL.INDICADORID ");
            stb.AppendFormat("AND CONVERT(DATE,HIS.STARTED_AT) <= TBL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIS.ENDED_AT) >= TBL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME ON HME.INDICATORID = TBL.INDICADORID ");
            stb.AppendFormat("AND HME.deleted_at IS NULL ");
            stb.AppendFormat("LEFT JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON ME.ID = HME.MATHEMATICALEXPRESSIONID ");
            stb.AppendFormat("AND ME.DELETED_AT IS NULL ");
            stb.AppendFormat($"LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = TBL.{columnName} ");
            stb.AppendFormat("GROUP BY TBL.INDICADORID, ");
            stb.AppendFormat("         CONVERT(DATE, TBL.CREATED_AT) ");

            List<HomeResultConsolidated> Listhrc = new List<HomeResultConsolidated>();

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                {
                    command.CommandTimeout = 0;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                HomeResultConsolidated hrc = new HomeResultConsolidated();
                                hrc.MATRICULA = idCol;
                                //hrc.CARGO = reader["CARGO"].ToString();
                                hrc.CODGIP = reader["IDGDA_SECTOR"].ToString();
                                //hrc.SETOR = reader["SETOR"].ToString();
                                hrc.IDINDICADOR = reader["IDGDA_INDICATOR"].ToString();
                                hrc.INDICADOR = reader["INDICADOR"].ToString();
                                //hrc.META = Double.Parse(reader["META"].ToString());
                                hrc.META = reader["META"].ToString() != "" ? double.Parse(reader["META"].ToString()) : 0;
                                hrc.min1 = reader["min1"].ToString() != "" ? double.Parse(reader["min1"].ToString()) : 0;
                                hrc.min2 = reader["min2"].ToString() != "" ? double.Parse(reader["min2"].ToString()) : 0;
                                hrc.min3 = reader["min3"].ToString() != "" ? double.Parse(reader["min3"].ToString()) : 0;
                                hrc.min4 = reader["min4"].ToString() != "" ? double.Parse(reader["min4"].ToString()) : 0;
                                hrc.CONTA = reader["CONTA"].ToString();
                                hrc.BETTER = reader["BETTER"].ToString();
                                hrc.FACTOR0 = reader["FACTOR0"].ToString() != "" ? double.Parse(reader["FACTOR0"].ToString()) : 0;
                                hrc.FACTOR1 = reader["FACTOR1"].ToString() != "" ? double.Parse(reader["FACTOR1"].ToString()) : 0;

                                hrc.SUMDIASLOGADOS = reader["LOGADO"].ToString() != "" ? Convert.ToInt32(reader["LOGADO"].ToString()) : 0;
                                hrc.SUMDIASESCALADOS = reader["ESCALADO"].ToString() != "" ? Convert.ToInt32(reader["ESCALADO"].ToString()) : 0;
                                hrc.SUMDIASLOGADOSESCALADOS = reader["ESCALADOLOGADO"].ToString() != "" ? Convert.ToInt32(reader["ESCALADOLOGADO"].ToString()) : 0;

                                hrc.MOEDA_GANHA = reader["MOEDA_GANHA"].ToString() != "" ? double.Parse(reader["MOEDA_GANHA"].ToString()) : 0;
                                hrc.QTD_MON = reader["QTD_MON"].ToString() != "" ? double.Parse(reader["QTD_MON"].ToString()) : 0;
                                hrc.QTD_MON_TOTAL = reader["QTD_MON_TOTAL"].ToString() != "" ? double.Parse(reader["QTD_MON_TOTAL"].ToString()) : 0;
                                hrc.QTD = Double.Parse(reader["QTD"].ToString());
                                hrc.DATAPAGAMENTO = reader["CREATED_AT"].ToString();
                                hrc.TYPE = reader["TYPE"].ToString();
                                hrc.MOEDA_GANHA = hrc.MOEDA_GANHA > 0 ? hrc.MOEDA_GANHA / hrc.QTD_MON : 0;
                                hrc.QTD_META = reader["QTD_META"].ToString() != "" ? double.Parse(reader["QTD_META"].ToString()) : 0;

                                Listhrc.Add(hrc);
                            }
                            catch (Exception)
                            {

                            }

                        }
                    }
                }

                connection.Close();
            }
            return Listhrc;
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {
            string dtInicial = inputModel.dataInicial.ToString("yyyy-MM-dd");
            string dtFinal = inputModel.dataFinal.ToString("yyyy-MM-dd");
            string codCollaborator = inputModel.codCollaborator.ToString();
            string hierarchiesAsString = inputModel.Hierarchies.ToString();
            string sectorsAsString = string.Join(",", inputModel.Sectors.Select(g => g.Id));
            //string sectorsGroupsAsString = string.Join(", ", inputModel.SectorsGroups.Select(g => $"'{g.Name}'"));
            string periodsAsString = string.Join(",", inputModel.periods.Select(g => g.Id));
            string homeFloorsAsString = string.Join(",", inputModel.homeFloors.Select(g => g.Id));
            string sitesAsString = string.Join(",", inputModel.sites.Select(g => g.Id));
            string subSectorsAsString = string.Join(",", inputModel.SubSectors.Select(g => g.Id));
            //string hierarchiesAsString = string.Join(",", inputModel.Hierarchies.Select(g => g.Id));
            string indicatorsAsString = string.Join(",", inputModel.Indicators.Where(i => i.Id.ToString() != "10000013").Select(g => g.Id));
            string indicatorsAccessAsString = string.Join(",", inputModel.Indicators.Where(i => i.Id.ToString() == "10000013").Select(g => g.Id));

            string sectorsIdsGroupsAsString = "";
            if (inputModel.sectorsIds != null)
            {
                sectorsIdsGroupsAsString = string.Join(",", inputModel.sectorsIds.Select(g => g.Id));
            }
            //Caso seja agrupado.
            //if (sectorsGroupsAsString != "")
            //{
            //    sectorsAsString = Funcoes.retornaIdsSectorsGroup(sectorsGroupsAsString);
            //}
            if (sectorsIdsGroupsAsString != "")
            {
                sectorsAsString = sectorsIdsGroupsAsString;
            }

            //Verifica perfil administrativo
            bool adm = Funcoes.retornaPermissao(codCollaborator.ToString());
            if (adm == true)
            {
                //Coloca o id do CEO, pois tera a mesma visão
                codCollaborator = "756399";
                hierarchiesAsString = "7";
            }


            if (hierarchiesAsString == "")
            {
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {

                    StringBuilder stb = new StringBuilder();
                    stb.Append("SELECT TOP 1 IDGDA_HIERARCHY FROM GDA_COLLABORATORS_DETAILS (NOLOCK) CD ");
                    stb.Append("INNER JOIN GDA_HIERARCHY (NOLOCK) H ON CD.CARGO = H.LEVELNAME ");
                    stb.Append("WHERE CONVERT(DATE, CD.CREATED_AT) >= CONVERT(DATE, DATEADD(DAY, -1, GETDATE())) ");
                    stb.Append($"AND CD.IDGDA_COLLABORATORS = '{codCollaborator}' ");

                    connection.Open();
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.CommandTimeout = 0;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                hierarchiesAsString = reader["IDGDA_HIERARCHY"] != DBNull.Value ? reader["IDGDA_HIERARCHY"].ToString() : "";
                            }
                        }
                    }

                    connection.Close();
                }
            }
            //if (hierarchiesAsString == "1")
            //{
            //    return BadRequest("Perfil não adequado para gerar resultado consolidado");
            //}

            //Setar Filtro de Consolidado somente para um mes
            DateTime dtTimeInicial = DateTime.ParseExact(dtInicial, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dtTimeFinal = DateTime.ParseExact(dtFinal, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            TimeSpan diff = dtTimeFinal.Subtract(dtTimeInicial);
            int diferencaEmDias = (int)diff.TotalDays;
            if (diferencaEmDias > 31)
            {
                return BadRequest("Selecionar uma data de no maximo 1 mês!");
            }

            bool? bkt = inputModel.basket != null ? inputModel.basket : false;


            List<groups> lgroup = returnTables.listGroups("");
            groups lgroup1 = lgroup.Find(l => l.id == 1);
            groups lgroup2 = lgroup.Find(l => l.id == 2);
            groups lgroup3 = lgroup.Find(l => l.id == 3);
            groups lgroup4 = lgroup.Find(l => l.id == 4);


            //Realiza a query que retorna todas as informações consolidada do colaborador filtrado.
            List<HomeResultConsolidated> rmams = new List<HomeResultConsolidated>();
            rmams = ReturnHomeResultConsolidated(codCollaborator, dtInicial, dtFinal, hierarchiesAsString, indicatorsAsString, sectorsAsString, subSectorsAsString, periodsAsString, homeFloorsAsString, sitesAsString, bkt);

            //Adicionando os indicadores de taxa de acesso
            if (indicatorsAccessAsString != "" || indicatorsAsString == "")
            {
                List<HomeResultConsolidated> listaIndicadorAcesso = new List<HomeResultConsolidated>();
                listaIndicadorAcesso = ReturnHomeResultConsolidatedAccess(codCollaborator, dtInicial, dtFinal, hierarchiesAsString, sectorsAsString, subSectorsAsString, periodsAsString, homeFloorsAsString, sitesAsString, bkt, "'10000013','10000014'");
                rmams = rmams.Concat(listaIndicadorAcesso).ToList();
            }

            //Retirar Tx de Acesso
            rmams = rmams.Where(rd => rd.IDINDICADOR != "-1").ToList();

            // ORDERAR POR INDICADOR ORDER DESCRESENTE DATA DE PAGAMENTO.
            rmams = rmams.OrderBy(r => r.IDINDICADOR).OrderByDescending(r => r.DATAPAGAMENTO).ToList();

            //AGRUPAR POR INDICADOR REALIZANDO O PONTERAMENTO DAS MOEDAS MAXIMAS COM MOEDAS GANHAS
            List<HomeResultConsolidated> retorno = new List<HomeResultConsolidated>();

            retorno = rmams.GroupBy(d => new { d.IDINDICADOR }).Select(item => new HomeResultConsolidated
            {
                MATRICULA = item.First().MATRICULA,
                CARGO = item.First().CARGO,
                //CODGIP = item.First().CODGIP,
                //SETOR = item.First().SETOR,
                IDINDICADOR = item.Key.IDINDICADOR,
                INDICADOR = item.First().INDICADOR,
                QTD = item.Sum(d => d.QTD),
                //META = Math.Round(item.Sum(d => d.META) / item.Count(), 2, MidpointRounding.AwayFromZero),
                META = Math.Round(item.Sum(d => d.META) / (item.Sum(d => d.QTD_META) > 0 ? item.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                FACTOR0 = item.Sum(d => d.FACTOR0),
                FACTOR1 = item.Sum(d => d.FACTOR1),
                //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                min1 = Math.Round(item.Sum(d => d.min1) / (item.Sum(d => d.QTD_META) > 0 ? item.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                min2 = Math.Round(item.Sum(d => d.min2) / (item.Sum(d => d.QTD_META) > 0 ? item.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                min3 = Math.Round(item.Sum(d => d.min3) / (item.Sum(d => d.QTD_META) > 0 ? item.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                min4 = Math.Round(item.Sum(d => d.min4) / (item.Sum(d => d.QTD_META) > 0 ? item.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                CONTA = item.First().CONTA,
                BETTER = item.First().BETTER,
                //META_MAXIMA_MOEDAS = Math.Round(item.Sum(d => d.META_MAXIMA_MOEDAS) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                META_MAXIMA_MOEDAS = item.Sum(d => d.QTD_MON != 0 ? Math.Round(d.QTD_MON_TOTAL / d.QTD_MON, 0, MidpointRounding.AwayFromZero) : 0),

                SUMDIASLOGADOS = item.Sum(d => d.SUMDIASLOGADOS),
                SUMDIASESCALADOS = item.Sum(d => d.SUMDIASESCALADOS),
                SUMDIASLOGADOSESCALADOS = item.Sum(d => d.SUMDIASLOGADOSESCALADOS),

                //MOEDA_GANHA =  Math.Round(item.Sum(d => d.MOEDA_GANHA) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                MOEDA_GANHA = Math.Round(item.Sum(d => d.MOEDA_GANHA), 0, MidpointRounding.AwayFromZero),
                TYPE = item.First().TYPE,
            }).ToList();

            for (int i = 0; i < retorno.Count; i++)
            {
                HomeResultConsolidated consolidado = retorno[i];
                retorno[i] = DoCalculateFinal(consolidado);
            }

            for (int i = 0; i < retorno.Count; i++)
            {
                HomeResultConsolidated consolidado = retorno[i];
                if (consolidado.META_MAXIMA_MOEDAS > 0)
                {
                    consolidado.MONETIZATION = true;
                }
                else
                    consolidado.MONETIZATION = false;
            }

            //if (inputModel.basket == true)
            //{
            HomeResultConsolidated cestaIndicadores = new HomeResultConsolidated
            {
                MATRICULA = "",
                CARGO = "",
                //CODGIP = item.First().CODGIP,
                //SETOR = item.First().SETOR,
                IDINDICADOR = "10000012",
                INDICADOR = "Cesta de Indicadores",
                QTD = 0,
                RESULTADO = retorno.Sum(d => d.MOEDA_GANHA),
                META = retorno.Sum(d => d.META_MAXIMA_MOEDAS),
                FACTOR0 = 0,
                FACTOR1 = 0,
                //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                min1 = 0,
                min2 = 0,
                min3 = 0,
                min4 = 0,
                CONTA = "",
                BETTER = "BIGGER_BETTER",
                //META_MAXIMA_MOEDAS = Math.Round(item.Sum(d => d.META_MAXIMA_MOEDAS) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                META_MAXIMA_MOEDAS = retorno.Sum(d => d.META_MAXIMA_MOEDAS),
                SUMDIASLOGADOS = 0,
                SUMDIASESCALADOS = 0,
                SUMDIASLOGADOSESCALADOS = 0,
                MONETIZATION = true,
                //MOEDA_GANHA =  Math.Round(item.Sum(d => d.MOEDA_GANHA) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                MOEDA_GANHA = retorno.Sum(d => d.MOEDA_GANHA),
                TYPE = "INTEGER",
            };

            Funcoes.cestaMetrica cm = Funcoes.getInfMetricBasket();

            if (cestaIndicadores.META_MAXIMA_MOEDAS == 0)
            {
                cestaIndicadores.PERCENTUAL = 100;
                cestaIndicadores.IDGRUPO = lgroup1.id;
                cestaIndicadores.GRUPO = lgroup1.name;
                cestaIndicadores.IMAGEMGRUPO = lgroup1.image;
            }
            else
            {
                double calculo = (cestaIndicadores.MOEDA_GANHA / cestaIndicadores.META_MAXIMA_MOEDAS) * 100;

                //Verifica a qual grupo pertence
                if (calculo >= cm.min1)
                {
                    cestaIndicadores.GRUPO = lgroup1.name;
                    cestaIndicadores.IDGRUPO = lgroup1.id;
                    cestaIndicadores.IMAGEMGRUPO = lgroup1.image;
                }
                else if (calculo >= cm.min2)
                {
                    cestaIndicadores.GRUPO = lgroup2.name;
                    cestaIndicadores.IDGRUPO = lgroup2.id;
                    cestaIndicadores.IMAGEMGRUPO = lgroup2.image;
                }
                else if (calculo >= cm.min3)
                {
                    cestaIndicadores.GRUPO = lgroup3.name;
                    cestaIndicadores.IDGRUPO = lgroup3.id;
                    cestaIndicadores.IMAGEMGRUPO = lgroup3.image;
                }
                else if (calculo >= cm.min4)
                {
                    cestaIndicadores.GRUPO = lgroup4.name;
                    cestaIndicadores.IDGRUPO = lgroup4.id;
                    cestaIndicadores.IMAGEMGRUPO = lgroup4.image;
                }
                else
                {
                    cestaIndicadores.GRUPO = lgroup4.name;
                    cestaIndicadores.IDGRUPO = lgroup4.id;
                    cestaIndicadores.IMAGEMGRUPO = lgroup4.image;
                }

                cestaIndicadores.PERCENTUAL = Math.Round(calculo, 2, MidpointRounding.AwayFromZero);
            }
            retorno.Insert(0, cestaIndicadores);
            //}

            //Realiza retirada dos resultados sem meta
            permit pm = new permit();
            pm = Funcoes.returnPermitByActionResource("Ver resultados de indicadores sem meta", "Home", codCollaborator);
            if (pm.active == false)
            {
                retorno = retorno.Where(d => d.META != 0).ToList();
            }


            var jsonData = retorno.Select(item => new returnResponseDay
            {
                MATRICULA = item.MATRICULA,
                CARGO = item.CARGO,
                CODGIP = item.CODGIP,
                SETOR = item.SETOR,
                IDINDICADOR = item.IDINDICADOR,
                INDICADOR = item.INDICADOR,
                TYPE = item.TYPE,
                META = item.META != 0 ? item.META : (double?)null,
                META_HORA = item.TYPE == "HOUR" ? item.META != 0 ? FormatTime(item.META.ToString()) : null : null,
                RESULTADO = item.RESULTADO,
                RESULTADO_HORA = item.TYPE == "HOUR" ? FormatTime(item.RESULTADO.ToString()) : "",
                PERCENTUAL = item.PERCENTUAL,
                META_MAXIMA_MOEDAS = item.META_MAXIMA_MOEDAS,
                MOEDA_GANHA = item.MOEDA_GANHA,
                IDGRUPO = item.IDGRUPO,
                GRUPO = item.GRUPO,
                IMAGEMGRUPO = item.IMAGEMGRUPO,
                MONETIZATION = item.MONETIZATION,
            }).ToList();


            //calcular percentual de cada grupo para ranking
            var rankingGeral = jsonData.Where(d => d.IDINDICADOR != "10000012").ToList();
            int contadorDima = rankingGeral.Count(item => item.IDGRUPO == 1 && item.MONETIZATION);
            int contadorOuro = rankingGeral.Count(item => item.IDGRUPO == 2 && item.MONETIZATION);
            int contadorPrata = rankingGeral.Count(item => item.IDGRUPO == 3 && item.MONETIZATION);
            int contadorBronze = rankingGeral.Count(item => item.IDGRUPO == 4 && item.MONETIZATION);

            int totalElementos = rankingGeral.Count(item => item.MONETIZATION);
            double frequenciaPercentualDima = (double)contadorDima / totalElementos * 100;
            double frequenciaPercentualOuro = (double)contadorOuro / totalElementos * 100;
            double frequenciaPercentualPrata = (double)contadorPrata / totalElementos * 100;
            double frequenciaPercentualBronze = (double)contadorBronze / totalElementos * 100;

            var allGroups = GetAllGroups();
            var responseData = new ReturnResponseConsolidade
            {
                RESULTS = jsonData,
                RANKING = allGroups.Select(group => new Ranking
                {
                    IDGROUP = group.id,
                    GROUPNAME = group.name,
                    DESCRIPTION = group.description,
                    ALIAS = group.alias,
                    IMAGEMGROUP = group.url,
                    COUNT = (group.id == 1) ? contadorDima : (group.id == 2) ? contadorOuro : (group.id == 3) ? contadorPrata : contadorBronze,
                    PERCENT = (group.id == 1) ? frequenciaPercentualDima : (group.id == 2) ? frequenciaPercentualOuro : (group.id == 3) ? frequenciaPercentualPrata : frequenciaPercentualBronze
                })
                .ToList()
            };

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(responseData);
        }



        public static string FormatTime(string totalSecondsD)
        {
            try
            {
                int totalSeconds = (int)Math.Round(Convert.ToDouble(totalSecondsD));

                // Calcula horas, minutos e segundos a partir dos segundos totais
                int hours = totalSeconds / 3600;
                int minutes = (totalSeconds % 3600) / 60;
                int seconds = totalSeconds % 60;

                // Retorna o formato "HH:mm:ss"
                return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
            }
            catch (Exception)
            {

            }
            return "";
        }

        public static double ConvertToDoubleOrZero(string input)
        {
            // Tenta converter o input para double
            if (Double.TryParse(input, out double result))
            {
                return result; // Retorna o valor convertido se for bem-sucedido
            }

            return 0; // Se falhar, retorna 0
        }

        public static List<GroupModel> GetAllGroups()
        {
            List<GroupModel> groups = new List<GroupModel>();

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT G.id, G.name, G.description, G.alias, G.uploadId, U.url FROM GDA_GROUPS (NOLOCK) AS G ");
            sb.AppendFormat("LEFT JOIN GDA_UPLOADS (NOLOCK) AS U ON U.id = G.uploadId");

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
                                GroupModel group = new GroupModel();
                                group.id = int.Parse(reader["id"].ToString());
                                group.name = reader["name"].ToString();
                                group.description = reader["description"].ToString();
                                group.alias = reader["alias"].ToString();
                                group.uploadId = int.Parse(reader["uploadId"].ToString());
                                group.url = reader["url"].ToString();
                                groups.Add(group);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return groups;
        }




    }
}