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
using System.Web.UI.WebControls;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ReportConsolidatedSector2Controller : ApiController
    {
        public static List<ModelsEx.homeRel> returnMonetizationAdmMonth(string dtInicial, string dtFinal, string sectors, string indicators, string ordem, bool consolidado, bool Thread = false)
        {
            //PREPARAR OS FILTROS
            string filter = "";
            string orderBy = "";

            //FILTRO POR SETOR.
            if (sectors != "")
            {
                filter = filter + $" AND CL.IDGDA_SECTOR IN ({sectors}) ";
            }

            //FILTRO POR INDICADOR.
            if (indicators != "")
            {
                filter = filter + $" AND R.INDICADORID IN ({indicators}) ";
            }

            //FILTRO PO GANHO DE MOEDAS E INDICADOR.
            if (ordem != "")
            {
                if (ordem.ToUpper() == "MELHOR")
                {
                    orderBy = " ORDER BY R.INDICADORID, MAX(MZ.SOMA) DESC ";
                }
                else
                {
                    orderBy = " ORDER BY R.INDICADORID, MAX(MZ.SOMA) ASC ";
                }
            }
            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtInicial);
            stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFinal);
            stb.Append(" ");
            stb.Append("SELECT MONTH(@DATAINICIAL) AS MES, ");
            stb.Append("       YEAR(@DATAINICIAL) AS ANO, ");
            stb.Append("       MAX(R.CREATED_AT) AS DATA, ");
            stb.Append("       R.IDGDA_COLLABORATORS AS IDGDA_COLLABORATORS, ");
            stb.Append("       MAX(CB.NAME) AS NAME, ");
            stb.Append("       MAX(CL.CARGO) AS CARGO, ");
            stb.Append("	   MAX(CL.PERIODO) AS TURNO, ");
            stb.Append("       MAX(IT.TYPE) AS TIPO, ");
            stb.Append("       MAX(R.RESULT) AS RESULTADOAPI, ");
            stb.Append("       MAX(TRAB.TRABALHADO) AS TRABALHADO, ");
            stb.Append("       MAX(ESC.ESCALADO) AS ESCALADO, ");
            stb.Append("       CASE ");
            stb.Append("           WHEN MAX(MZ.SOMA) IS NULL THEN 0 ");
            stb.Append("           ELSE MAX(MZ.SOMA) ");
            stb.Append("       END AS MOEDA_GANHA, ");
            stb.Append("       MAX(HIS.GOAL) AS META, ");
            stb.Append("       MAX(HIS.GOAL_NIGHT) AS META_NOTURNA, ");
            stb.Append("       MAX(HIS.GOAL_LATENIGHT) AS META_MADRUGADA, ");
            stb.Append("       MAX(ISNULL(HIG1.MONETIZATION, 0)) AS META_MAXIMA, ");
            stb.Append("       MAX(ISNULL(HIG1.MONETIZATION_NIGHT, 0)) AS META_MAXIMA_NOTURNA, ");
            stb.Append("       MAX(ISNULL(HIG1.MONETIZATION_LATENIGHT, 0)) AS META_MAXIMA_MADRUGADA, ");
            stb.Append("       R.INDICADORID AS 'COD INDICADOR', ");
            stb.Append("       MAX(IT.NAME) AS 'INDICADOR', ");
            stb.Append("       MAX(HIS.GOAL) AS META, ");
            stb.Append("       MAX(SC.WEIGHT_SCORE) AS SCORE, ");
            stb.Append("       MAX(HIG1.METRIC_MIN) AS MIN1, ");
            stb.Append("       MAX(HIG2.METRIC_MIN) AS MIN2, ");
            stb.Append("       MAX(HIG3.METRIC_MIN) AS MIN3, ");
            stb.Append("       MAX(HIG4.METRIC_MIN) AS MIN4, ");
            stb.Append("       MAX(HIG1.METRIC_MIN_NIGHT) AS MIN1_NOTURNO, ");
            stb.Append("       MAX(HIG2.METRIC_MIN_NIGHT) AS MIN2_NOTURNO, ");
            stb.Append("       MAX(HIG3.METRIC_MIN_NIGHT) AS MIN3_NOTURNO, ");
            stb.Append("       MAX(HIG4.METRIC_MIN_NIGHT) AS MIN4_NOTURNO, ");
            stb.Append("       MAX(HIG1.METRIC_MIN_LATENIGHT) AS MIN1_MADRUGADA, ");
            stb.Append("       MAX(HIG2.METRIC_MIN_LATENIGHT) AS MIN2_MADRUGADA, ");
            stb.Append("       MAX(HIG3.METRIC_MIN_LATENIGHT) AS MIN3_MADRUGADA, ");
            stb.Append("       MAX(HIG4.METRIC_MIN_LATENIGHT) AS MIN4_MADRUGADA, ");
            stb.Append("       MAX(R.FACTORS) AS FATOR, ");
            stb.Append("       CASE ");
            stb.Append("           WHEN MAX(ME.EXPRESSION) IS NULL THEN '(#FATOR1/#FATOR0)' ");
            stb.Append("           ELSE MAX(ME.EXPRESSION) ");
            stb.Append("       END AS CONTA, ");
            stb.Append("       CASE ");
            stb.Append("           WHEN MAX(IT.CALCULATION_TYPE) IS NULL THEN 'BIGGER_BETTER' ");
            stb.Append("           ELSE MAX(IT.CALCULATION_TYPE) ");
            stb.Append("       END AS BETTER, ");
            stb.Append("       '' AS RESULTADO, ");
            stb.Append("       '' AS PORCENTUAL, ");
            stb.Append("       '' AS GRUPO, ");
            stb.Append("       GETDATE() AS 'Data de atualização', ");
            stb.Append("       MAX(CL.IDGDA_SECTOR) AS COD_GIP, ");
            stb.Append("       MAX(SEC.NAME) AS SETOR, ");
            stb.Append("       MAX(CL.IDGDA_SECTOR_REFERENCE) AS COD_GIP_REFERENCE, ");
            stb.Append("       MAX(SECREFERENCE.NAME) AS SETOR_REFERENCE, ");
            stb.Append("       MAX(CL.SITE) AS SITE, ");
            stb.Append("       MAX(CL.MATRICULA_GERENTE_II) AS 'MATRICULA GERENTE II', ");
            stb.Append("       MAX(CL.NOME_GERENTE_II) AS 'NOME GERENTE II', ");
            stb.Append("       MAX(CL.MATRICULA_GERENTE_I) AS 'MATRICULA GERENTE I', ");
            stb.Append("       MAX(CL.NOME_GERENTE_I) AS 'NOME GERENTE I', ");
            stb.Append("       MAX(CL.MATRICULA_DIRETOR) AS 'MATRICULA DIRETOR', ");
            stb.Append("       MAX(CL.NOME_DIRETOR) AS 'NOME DIRETOR', ");
            stb.Append("       ISNULL(MAX(AC.QUANTIDADE_LOGIN),0) AS LOGIN ");
            stb.Append("FROM GDA_RESULT (NOLOCK) AS R ");
            stb.Append("INNER JOIN GDA_COLLABORATORS (NOLOCK) AS CB ON CB.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            stb.Append("LEFT JOIN ");
            stb.Append("   (SELECT COUNT(0) AS 'QUANTIDADE_LOGIN', ");
            stb.Append("   IDGDA_COLLABORATOR  ");
            stb.Append("   FROM GDA_LOGIN_ACCESS (NOLOCK) ");
            stb.Append("   WHERE DATE_ACCESS >= @DATAINICIAL ");
            stb.Append("     AND DATE_ACCESS <= @DATAFINAL ");
            stb.Append("   GROUP BY IDGDA_COLLABORATOR ");
            stb.Append("   ) AS AC ON AC.IDGDA_COLLABORATOR = R.IDGDA_COLLABORATORS ");
            stb.Append("LEFT JOIN ");
            stb.Append("  (SELECT CASE ");
            stb.Append("              WHEN IDGDA_SUBSECTOR IS NULL THEN IDGDA_SECTOR ");
            stb.Append("              ELSE IDGDA_SUBSECTOR ");
            stb.Append("          END AS IDGDA_SECTOR, ");
            stb.Append("          IDGDA_SECTOR AS IDGDA_SECTOR_REFERENCE, ");
            stb.Append("          CREATED_AT, ");
            stb.Append("          IDGDA_COLLABORATORS, ");
            stb.Append("          IDGDA_SECTOR_SUPERVISOR, ");
            stb.Append("          ACTIVE, ");
            stb.Append("          CARGO, ");
            stb.Append("          HOME_BASED, ");
            stb.Append("          SITE, ");
            stb.Append("          PERIODO, ");
            stb.Append("          MATRICULA_SUPERVISOR, ");
            stb.Append("          NOME_SUPERVISOR, ");
            stb.Append("          MATRICULA_COORDENADOR, ");
            stb.Append("          NOME_COORDENADOR, ");
            stb.Append("          MATRICULA_GERENTE_II, ");
            stb.Append("          NOME_GERENTE_II, ");
            stb.Append("          MATRICULA_GERENTE_I, ");
            stb.Append("          NOME_GERENTE_I, ");
            stb.Append("          MATRICULA_DIRETOR, ");
            stb.Append("          NOME_DIRETOR, ");
            stb.Append("          MATRICULA_CEO, ");
            stb.Append("          NOME_CEO ");
            stb.Append("   FROM GDA_COLLABORATORS_DETAILS (NOLOCK) ");
            stb.Append("   WHERE CREATED_AT >= @DATAINICIAL ");
            stb.Append("     AND CREATED_AT <= @DATAFINAL ) AS CL ON CL.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS ");
            stb.Append("AND CL.CREATED_AT = R.CREATED_AT ");
            stb.Append("LEFT JOIN GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME ON HME.INDICATORID = R.INDICADORID ");
            stb.Append("AND HME.deleted_at IS NULL ");
            stb.Append("LEFT JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON ME.ID = HME.MATHEMATICALEXPRESSIONID ");
            stb.Append("AND ME.DELETED_AT IS NULL ");
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
            stb.Append("AND HIG1.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND HIG1.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.Append("AND HIG1.GROUPID = 1 ");
            stb.Append("AND CONVERT(DATE,HIG1.STARTED_AT) <= R.CREATED_AT ");
            stb.Append("AND CONVERT(DATE,HIG1.ENDED_AT) >= R.CREATED_AT ");
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG2 ON HIG2.DELETED_AT IS NULL ");
            stb.Append("AND HIG2.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND HIG2.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.Append("AND HIG2.GROUPID = 2 ");
            stb.Append("AND CONVERT(DATE,HIG2.STARTED_AT) <= R.CREATED_AT ");
            stb.Append("AND CONVERT(DATE,HIG2.ENDED_AT) >= R.CREATED_AT ");
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG3 ON HIG3.DELETED_AT IS NULL ");
            stb.Append("AND HIG3.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND HIG3.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.Append("AND HIG3.GROUPID = 3 ");
            stb.Append("AND CONVERT(DATE,HIG3.STARTED_AT) <= R.CREATED_AT ");
            stb.Append("AND CONVERT(DATE,HIG3.ENDED_AT) >= R.CREATED_AT ");
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG4 ON HIG4.DELETED_AT IS NULL ");
            stb.Append("AND HIG4.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND HIG4.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.Append("AND HIG4.GROUPID = 4 ");
            stb.Append("AND CONVERT(DATE,HIG4.STARTED_AT) <= R.CREATED_AT ");
            stb.Append("AND CONVERT(DATE,HIG4.ENDED_AT) >= R.CREATED_AT ");
            stb.Append("LEFT JOIN GDA_CHECKING_ACCOUNT (NOLOCK) AS CA ON CA.COLLABORATOR_ID = R.IDGDA_COLLABORATORS ");
            stb.Append("AND CA.RESULT_DATE >= @DATAINICIAL ");
            stb.Append("AND CA.RESULT_DATE <= @DATAFINAL ");
            stb.Append("AND CA.GDA_INDICATOR_IDGDA_INDICATOR = R.INDICADORID ");
            stb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS IT ON IT.IDGDA_INDICATOR = R.INDICADORID ");
            stb.Append("LEFT JOIN ");
            stb.Append("  (SELECT SUM(INPUT) - SUM(OUTPUT) AS SOMA, ");
            stb.Append("          gda_indicator_idgda_indicator, ");
            stb.Append("          result_date, ");
            stb.Append("          COLLABORATOR_ID ");
            stb.Append("   FROM GDA_CHECKING_ACCOUNT ");
            stb.Append("   WHERE RESULT_DATE >= @DATAINICIAL ");
            stb.Append("     AND RESULT_DATE <= @DATAFINAL ");
            stb.Append("   GROUP BY gda_indicator_idgda_indicator, ");
            stb.Append("            result_date, ");
            stb.Append("            COLLABORATOR_ID) AS MZ ON MZ.gda_indicator_idgda_indicator = R.INDICADORID ");
            stb.Append("AND MZ.result_date = R.CREATED_AT ");
            stb.Append("AND MZ.COLLABORATOR_ID = R.IDGDA_COLLABORATORS ");
            stb.Append("LEFT JOIN ");
            stb.Append("  (SELECT COUNT(0) AS 'ESCALADO', ");
            stb.Append("          IDGDA_COLLABORATORS, ");
            stb.Append("          CREATED_AT ");
            stb.Append("   FROM GDA_RESULT (NOLOCK) ");
            stb.Append("   WHERE INDICADORID = -1 ");
            stb.Append("     AND CREATED_AT >= @DATAINICIAL ");
            stb.Append("     AND CREATED_AT <= @DATAFINAL ");
            stb.Append("     AND RESULT = 1 ");
            stb.Append("     AND DELETED_AT IS NULL ");
            stb.Append("   GROUP BY IDGDA_COLLABORATORS, ");
            stb.Append("            CREATED_AT) AS ESC ON ESC.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            stb.Append("AND ESC.CREATED_AT = R.CREATED_AT ");
            stb.Append("LEFT JOIN ");
            stb.Append("  (SELECT COUNT(0) AS 'TRABALHADO', ");
            stb.Append("          IDGDA_COLLABORATORS, ");
            stb.Append("          CREATED_AT ");
            stb.Append("   FROM GDA_RESULT (NOLOCK) ");
            stb.Append("   WHERE INDICADORID = 2 ");
            stb.Append("     AND CREATED_AT >= @DATAINICIAL ");
            stb.Append("     AND CREATED_AT <= @DATAFINAL ");
            stb.Append("     AND RESULT = 0 ");
            stb.Append("     AND DELETED_AT IS NULL ");
            stb.Append("   GROUP BY IDGDA_COLLABORATORS, ");
            stb.Append("            CREATED_AT) AS TRAB ON TRAB.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            stb.Append("AND TRAB.CREATED_AT = R.CREATED_AT ");
            stb.Append("LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = CL.IDGDA_SECTOR ");
            stb.Append("LEFT JOIN GDA_SECTOR (NOLOCK) AS SECREFERENCE ON SECREFERENCE.IDGDA_SECTOR = CL.IDGDA_SECTOR_REFERENCE ");
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) AS HIS ON HIS.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND HIS.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.Append("AND CONVERT(DATE,HIS.STARTED_AT) <= R.CREATED_AT ");
            stb.Append("AND CONVERT(DATE,HIS.ENDED_AT) >= R.CREATED_AT ");
            stb.Append("AND HIS.DELETED_AT IS NULL ");
            stb.Append("LEFT JOIN GDA_HISTORY_SCORE_INDICATOR_SECTOR (NOLOCK) AS SC ON SC.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND SC.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.Append("AND CONVERT(DATE,SC.STARTED_AT) <= R.CREATED_AT ");
            stb.Append("AND CONVERT(DATE,SC.ENDED_AT) >= R.CREATED_AT ");
            stb.AppendFormat(" WHERE 1 = 1 {0} ", filter);
            stb.Append("  AND R.CREATED_AT >= @DATAINICIAL ");
            stb.Append("  AND R.CREATED_AT <= @DATAFINAL ");
            stb.Append("  AND R.DELETED_AT IS NULL ");
            //stb.Append("  AND CL.active = 'true' ");
            stb.Append("  AND R.FACTORS <> '0.000000;0.000000' ");
            stb.Append("  AND R.FACTORS <> '0.000000; 0.000000' ");
            stb.Append("GROUP BY R.IDGDA_COLLABORATORS, ");
            stb.Append("         R.INDICADORID, ");
            stb.Append("         R.CREATED_AT ");
            #region Antigo
            //stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtInicial);
            //stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFinal);
            //stb.Append("SELECT MONTH(@DATAINICIAL) AS MES, ");
            //stb.Append("       YEAR(@DATAINICIAL) AS ANO, ");
            //stb.Append("       MAX(R.CREATED_AT) AS DATA, ");
            //stb.Append("       R.IDGDA_COLLABORATORS AS IDGDA_COLLABORATORS, ");
            //stb.Append("       MAX(CB.NAME) AS NAME, ");
            //stb.Append("       MAX(CL.CARGO) AS CARGO, ");
            //stb.Append("       MAX(IT.TYPE) AS TIPO, ");
            //stb.Append("       MAX(R.RESULT) AS RESULTADOAPI, ");
            //stb.Append("       MAX(TRAB.TRABALHADO) AS TRABALHADO, ");
            //stb.Append("       MAX(ESC.ESCALADO) AS ESCALADO, ");
            //stb.Append("       CASE ");
            //stb.Append("           WHEN MAX(HIG1.MONETIZATION) IS NULL THEN 0 ");
            //stb.Append("           WHEN MAX(MZ.SOMA) IS NULL THEN 0 ");
            //stb.Append("           ELSE MAX(MZ.SOMA) ");
            //stb.Append("       END AS MOEDA_GANHA, ");
            ////stb.Append("       MAX(MZ.SOMA) AS MOEDA_GANHA, ");
            //stb.Append("       MAX(HIG1.MONETIZATION) AS META_MAXIMA, ");
            //stb.Append("       R.INDICADORID AS 'COD INDICADOR', ");
            //stb.Append("       MAX(IT.NAME) AS 'INDICADOR', ");
            //stb.Append("       MAX(HIS.GOAL) AS META, ");
            //stb.Append("       MAX(SC.WEIGHT_SCORE) AS SCORE, ");
            //stb.Append("       MAX(HIG1.METRIC_MIN) AS MIN1, ");
            //stb.Append("       MAX(HIG2.METRIC_MIN) AS MIN2, ");
            //stb.Append("       MAX(HIG3.METRIC_MIN) AS MIN3, ");
            //stb.Append("       MAX(HIG4.METRIC_MIN) AS MIN4, ");
            //stb.Append("       MAX(R.FACTORS) AS FATOR, ");
            //stb.Append("       CASE ");
            //stb.Append("           WHEN MAX(ME.EXPRESSION) IS NULL THEN '(#FATOR1/#FATOR0)' ");
            //stb.Append("           ELSE MAX(ME.EXPRESSION) ");
            //stb.Append("       END AS CONTA, ");
            //stb.Append("       CASE ");
            //stb.Append("           WHEN MAX(IT.CALCULATION_TYPE) IS NULL THEN 'BIGGER_BETTER' ");
            //stb.Append("           ELSE MAX(IT.CALCULATION_TYPE) ");
            //stb.Append("       END AS BETTER, ");
            //stb.Append("       '' AS RESULTADO, ");
            //stb.Append("       '' AS PORCENTUAL, ");
            //stb.Append("       '' AS GRUPO, ");
            //stb.Append("       GETDATE() AS 'Data de atualização', ");
            //stb.Append("       MAX(CL.IDGDA_SECTOR) AS COD_GIP, ");
            //stb.Append("       MAX(SEC.NAME) AS SETOR, ");
            //stb.Append("       MAX(CL.SITE) AS SITE, ");
            //stb.Append("       MAX(CL.MATRICULA_GERENTE_II) AS 'MATRICULA GERENTE II', ");
            //stb.Append("       MAX(CL.NOME_GERENTE_II) AS 'NOME GERENTE II', ");
            //stb.Append("       MAX(CL.MATRICULA_GERENTE_I) AS 'MATRICULA GERENTE I', ");
            //stb.Append("       MAX(CL.NOME_GERENTE_I) AS 'NOME GERENTE I', ");
            //stb.Append("       MAX(CL.MATRICULA_DIRETOR) AS 'MATRICULA DIRETOR', ");
            //stb.Append("       MAX(CL.NOME_DIRETOR) AS 'NOME DIRETOR' ");
            //stb.Append("FROM GDA_RESULT (NOLOCK) AS R ");
            //stb.Append("INNER JOIN GDA_COLLABORATORS (NOLOCK) AS CB ON CB.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            ////stb.Append("LEFT JOIN ");
            ////stb.Append("  (SELECT CASE ");
            ////stb.Append("              WHEN CL.IDGDA_SECTOR IS NOT NULL THEN CL.IDGDA_SECTOR ");
            ////stb.Append("              ELSE CL2.IDGDA_SECTOR ");
            ////stb.Append("          END AS IDGDA_SECTOR, ");
            ////stb.Append("          CASE ");
            ////stb.Append("              WHEN CL.HOME_BASED != '' THEN CL.HOME_BASED ");
            ////stb.Append("              ELSE CL2.HOME_BASED ");
            ////stb.Append("          END AS HOME_BASED, ");
            ////stb.Append("          CASE ");
            ////stb.Append("              WHEN CL.CARGO IS NOT NULL THEN CL.CARGO ");
            ////stb.Append("              ELSE CL2.CARGO ");
            ////stb.Append("          END AS CARGO, ");
            ////stb.Append("          CASE ");
            ////stb.Append("              WHEN CL.ACTIVE IS NOT NULL THEN CL.ACTIVE ");
            ////stb.Append("              ELSE CL2.ACTIVE ");
            ////stb.Append("          END AS ACTIVE, ");
            ////stb.Append("          CASE ");
            ////stb.Append("              WHEN CL.SITE != '' THEN CL.SITE ");
            ////stb.Append("              ELSE CL2.SITE ");
            ////stb.Append("          END AS SITE, ");
            ////stb.Append("          CASE ");
            ////stb.Append("              WHEN CL.PERIODO != '' THEN CL.PERIODO ");
            ////stb.Append("              ELSE CL2.PERIODO ");
            ////stb.Append("          END AS PERIODO, ");
            ////stb.Append("          CASE ");
            ////stb.Append("              WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_SUPERVISOR ");
            ////stb.Append("              ELSE CL2.MATRICULA_SUPERVISOR ");
            ////stb.Append("          END AS MATRICULA_SUPERVISOR, ");
            ////stb.Append("          CASE ");
            ////stb.Append("              WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_SUPERVISOR ");
            ////stb.Append("              ELSE CL2.NOME_SUPERVISOR ");
            ////stb.Append("          END AS NOME_SUPERVISOR, ");
            ////stb.Append("          CASE ");
            ////stb.Append("              WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_COORDENADOR ");
            ////stb.Append("              ELSE CL2.MATRICULA_COORDENADOR ");
            ////stb.Append("          END AS MATRICULA_COORDENADOR, ");
            ////stb.Append("          CASE ");
            ////stb.Append("              WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_COORDENADOR ");
            ////stb.Append("              ELSE CL2.NOME_COORDENADOR ");
            ////stb.Append("          END AS NOME_COORDENADOR, ");
            ////stb.Append("          CASE ");
            ////stb.Append("              WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_GERENTE_II ");
            ////stb.Append("              ELSE CL2.MATRICULA_GERENTE_II ");
            ////stb.Append("          END AS MATRICULA_GERENTE_II, ");
            ////stb.Append("          CASE ");
            ////stb.Append("              WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_GERENTE_II ");
            ////stb.Append("              ELSE CL2.NOME_GERENTE_II ");
            ////stb.Append("          END AS NOME_GERENTE_II, ");
            ////stb.Append("          CASE ");
            ////stb.Append("              WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_GERENTE_I ");
            ////stb.Append("              ELSE CL2.MATRICULA_GERENTE_I ");
            ////stb.Append("          END AS MATRICULA_GERENTE_I, ");
            ////stb.Append("          CASE ");
            ////stb.Append("              WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_GERENTE_I ");
            ////stb.Append("              ELSE CL2.NOME_GERENTE_I ");
            ////stb.Append("          END AS NOME_GERENTE_I, ");
            ////stb.Append("          CASE ");
            ////stb.Append("              WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_DIRETOR ");
            ////stb.Append("              ELSE CL2.MATRICULA_DIRETOR ");
            ////stb.Append("          END AS MATRICULA_DIRETOR, ");
            ////stb.Append("          CASE ");
            ////stb.Append("              WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_DIRETOR ");
            ////stb.Append("              ELSE CL2.NOME_DIRETOR ");
            ////stb.Append("          END AS NOME_DIRETOR, ");
            ////stb.Append("          CASE ");
            ////stb.Append("              WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_CEO ");
            ////stb.Append("              ELSE CL2.MATRICULA_CEO ");
            ////stb.Append("          END AS MATRICULA_CEO, ");
            ////stb.Append("          CASE ");
            ////stb.Append("              WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_CEO ");
            ////stb.Append("              ELSE CL2.NOME_CEO ");
            ////stb.Append("          END AS NOME_CEO, ");
            ////stb.Append("          C.IDGDA_COLLABORATORS, ");
            ////stb.Append("          CL.CREATED_AT ");
            ////stb.Append("   FROM GDA_COLLABORATORS (NOLOCK) AS C ");
            ////stb.Append("   LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CL ON CL.IDGDA_COLLABORATORS = C.IDGDA_COLLABORATORS ");
            ////stb.Append("   AND CL.CREATED_AT >= @DATAINICIAL ");
            ////stb.Append("   AND CL.CREATED_AT <= @DATAFINAL ");
            ////stb.Append("   LEFT JOIN GDA_COLLABORATORS_LAST_DETAILS (NOLOCK) AS CL2 ON CL2.IDGDA_COLLABORATORS = C.IDGDA_COLLABORATORS) AS CL  ");
            ////stb.Append("   ON CL.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS AND CL.CREATED_AT = R.CREATED_AT ");
            //stb.Append("");
            //stb.Append(" LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CL ON CL.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS AND CL.CREATED_AT = R.CREATED_AT ");
            //stb.Append("");
            //stb.Append("LEFT JOIN GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME ON HME.INDICATORID = R.INDICADORID ");
            //stb.Append("AND HME.deleted_at IS NULL ");
            //stb.Append("LEFT JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON ME.ID = HME.MATHEMATICALEXPRESSIONID "); 
            //stb.Append("AND ME.DELETED_AT IS NULL ");
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
            //stb.Append("LEFT JOIN GDA_CHECKING_ACCOUNT (NOLOCK) AS CA ON CA.COLLABORATOR_ID = R.IDGDA_COLLABORATORS ");
            //stb.Append("AND CA.RESULT_DATE >= @DATAINICIAL ");
            //stb.Append("AND CA.RESULT_DATE <= @DATAFINAL ");
            //stb.Append("AND CA.GDA_INDICATOR_IDGDA_INDICATOR = R.INDICADORID ");
            //stb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS IT ON IT.IDGDA_INDICATOR = R.INDICADORID ");
            //stb.Append("LEFT JOIN ");
            //stb.Append("  (SELECT SUM(INPUT) AS SOMA, ");
            ////stb.Append("          idgda_sector, ");
            //stb.Append("          gda_indicator_idgda_indicator, ");
            //stb.Append("          result_date, ");
            //stb.Append("          COLLABORATOR_ID ");
            //stb.Append("   FROM GDA_CHECKING_ACCOUNT ");
            //stb.Append("   WHERE RESULT_DATE >= @DATAINICIAL ");
            //stb.Append("     AND RESULT_DATE <= @DATAFINAL ");
            ////stb.Append("   GROUP BY idgda_sector, ");
            //stb.Append("   GROUP BY  ");
            //stb.Append("            gda_indicator_idgda_indicator, ");
            //stb.Append("            result_date, ");
            //stb.Append("            COLLABORATOR_ID) AS MZ ON ");
            //stb.Append(" MZ.gda_indicator_idgda_indicator = R.INDICADORID ");
            //stb.Append("AND MZ.result_date = R.CREATED_AT ");
            //stb.Append("AND MZ.COLLABORATOR_ID = R.IDGDA_COLLABORATORS ");
            //stb.Append("LEFT JOIN ");
            //stb.Append("  (SELECT COUNT(0) AS 'ESCALADO', ");
            //stb.Append("          IDGDA_COLLABORATORS, ");
            //stb.Append("          CREATED_AT ");
            //stb.Append("   FROM GDA_RESULT (NOLOCK) ");
            //stb.Append("   WHERE INDICADORID = -1 ");
            //stb.Append("     AND CREATED_AT >= @DATAINICIAL ");
            //stb.Append("     AND CREATED_AT <= @DATAFINAL ");
            //stb.Append("     AND RESULT = 1 ");
            //stb.Append("     AND DELETED_AT IS NULL ");
            //stb.Append("   GROUP BY IDGDA_COLLABORATORS, ");
            //stb.Append("            CREATED_AT) AS ESC ON ESC.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            //stb.Append("AND ESC.CREATED_AT = R.CREATED_AT ");
            //stb.Append("LEFT JOIN ");
            //stb.Append("  (SELECT COUNT(0) AS 'TRABALHADO', ");
            //stb.Append("          IDGDA_COLLABORATORS, ");
            //stb.Append("          CREATED_AT ");
            //stb.Append("   FROM GDA_RESULT (NOLOCK) ");
            //stb.Append("   WHERE INDICADORID = 2 ");
            //stb.Append("     AND CREATED_AT >= @DATAINICIAL ");
            //stb.Append("     AND CREATED_AT <= @DATAFINAL ");
            //stb.Append("     AND RESULT = 0 ");
            //stb.Append("     AND DELETED_AT IS NULL ");
            //stb.Append("   GROUP BY IDGDA_COLLABORATORS, ");
            //stb.Append("            CREATED_AT) AS TRAB ON TRAB.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            //stb.Append("AND TRAB.CREATED_AT = R.CREATED_AT ");
            //stb.Append("LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = CL.IDGDA_SECTOR ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) AS HIS ON HIS.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIS.SECTOR_ID = CL.IDGDA_SECTOR ");
            //stb.Append("AND CONVERT(DATE,HIS.STARTED_AT) <= R.CREATED_AT ");
            //stb.Append("AND CONVERT(DATE,HIS.ENDED_AT) >= R.CREATED_AT ");
            //stb.Append("AND HIS.DELETED_AT IS NULL ");
            ////FK
            //stb.Append("LEFT JOIN GDA_HISTORY_SCORE_INDICATOR_SECTOR (NOLOCK) AS SC ON ");
            //stb.Append("SC.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND SC.SECTOR_ID = CL.IDGDA_SECTOR ");
            //stb.Append("AND CONVERT(DATE,SC.STARTED_AT) <= R.CREATED_AT ");
            //stb.Append("AND CONVERT(DATE,SC.ENDED_AT) >= R.CREATED_AT ");


            //stb.AppendFormat(" WHERE 1 = 1 AND R.CREATED_AT >= @DATAINICIAL AND R.CREATED_AT <= @DATAFINAL {0} ", filter);
            //stb.Append("AND R.DELETED_AT IS NULL ");
            ////stb.Append("AND CL.IDGDA_SECTOR IS NOT NULL ");
            ////stb.Append("AND CL.CARGO IS NOT NULL ");
            ////stb.Append("AND CL.HOME_BASED <> '' ");
            //stb.Append("AND CL.active = 'true' ");

            //stb.Append("GROUP BY R.IDGDA_COLLABORATORS, ");
            //stb.Append("         R.INDICADORID, ");
            //stb.Append("         R.CREATED_AT ");
            #endregion

            List<ModelsEx.homeRel> rmams = new List<ModelsEx.homeRel>();
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
                            ModelsEx.homeRel rmam = new ModelsEx.homeRel();
                            string factors = reader["FATOR"].ToString();
                            if (factors == "0.000000;0.000000" || factors == "0.000000; 0.000000")
                            {
                                continue;
                            }
                            string factor0 = Funcoes.RemoverZerosAposPonto(factors.Split(";")[0]);
                            string factor1 = Funcoes.RemoverZerosAposPonto(factors.Split(";")[1]);
                            rmam.fator0 = double.Parse(factor0);
                            rmam.fator1 = double.Parse(factor1);
                            rmam.mes = reader["MES"].ToString();
                            rmam.ano = reader["ANO"].ToString();
                            rmam.dateReferer = reader["DATA"].ToString();
                            rmam.data = reader["DATA"].ToString();
                            rmam.idcollaborator = reader["IDGDA_COLLABORATORS"].ToString();
                            rmam.name = reader["name"].ToString();
                            rmam.cod_indicador = reader["cod indicador"].ToString();
                            rmam.indicador = reader["indicador"].ToString();
                            rmam.indicatorType = reader["TIPO"].ToString();
                            if (reader["turno"].ToString() == "DIURNO")
                            {
                                rmam.meta = reader["META"].ToString();
                                rmam.min1 = reader["MIN1"].ToString() != "" ? double.Parse(reader["MIN1"].ToString()) : 0;
                                rmam.min2 = reader["MIN2"].ToString() != "" ? double.Parse(reader["MIN2"].ToString()) : 0;
                                rmam.min3 = reader["MIN3"].ToString() != "" ? double.Parse(reader["MIN3"].ToString()) : 0;
                                rmam.min4 = reader["MIN4"].ToString() != "" ? double.Parse(reader["MIN4"].ToString()) : 0;
                                rmam.goal = reader["META"].ToString() != "" ? double.Parse(reader["META"].ToString()) : 0;
                                rmam.moedasPossiveis = reader["META_MAXIMA"].ToString() != "" ? int.Parse(reader["META_MAXIMA"].ToString()) : 0;
                            }
                            else if (reader["turno"].ToString() == "NOTURNO")
                            {
                                rmam.meta = reader["META_NOTURNA"].ToString();
                                rmam.min1 = reader["MIN1_NOTURNO"].ToString() != "" ? double.Parse(reader["MIN1_NOTURNO"].ToString()) : 0;
                                rmam.min2 = reader["MIN2_NOTURNO"].ToString() != "" ? double.Parse(reader["MIN2_NOTURNO"].ToString()) : 0;
                                rmam.min3 = reader["MIN3_NOTURNO"].ToString() != "" ? double.Parse(reader["MIN3_NOTURNO"].ToString()) : 0;
                                rmam.min4 = reader["MIN4_NOTURNO"].ToString() != "" ? double.Parse(reader["MIN4_NOTURNO"].ToString()) : 0;
                                rmam.goal = reader["META_NOTURNA"].ToString() != "" ? double.Parse(reader["META_NOTURNA"].ToString()) : 0;
                                rmam.moedasPossiveis = reader["META_MAXIMA_NOTURNA"].ToString() != "" ? int.Parse(reader["META_MAXIMA_NOTURNA"].ToString()) : 0;
                            }
                            else if (reader["turno"].ToString() == "MADRUGADA")
                            {
                                rmam.meta = reader["META_MADRUGADA"].ToString();
                                rmam.min1 = reader["MIN1_MADRUGADA"].ToString() != "" ? double.Parse(reader["MIN1_MADRUGADA"].ToString()) : 0;
                                rmam.min2 = reader["MIN2_MADRUGADA"].ToString() != "" ? double.Parse(reader["MIN2_MADRUGADA"].ToString()) : 0;
                                rmam.min3 = reader["MIN3_MADRUGADA"].ToString() != "" ? double.Parse(reader["MIN3_MADRUGADA"].ToString()) : 0;
                                rmam.min4 = reader["MIN4_MADRUGADA"].ToString() != "" ? double.Parse(reader["MIN4_MADRUGADA"].ToString()) : 0;
                                rmam.goal = reader["META_MADRUGADA"].ToString() != "" ? double.Parse(reader["META_MADRUGADA"].ToString()) : 0;
                                rmam.moedasPossiveis = reader["META_MAXIMA_MADRUGADA"].ToString() != "" ? int.Parse(reader["META_MAXIMA_MADRUGADA"].ToString()) : 0;
                            }
                            else if (reader["turno"].ToString() == "" || reader["turno"].ToString() == null)
                            {
                                rmam.meta = "";
                                rmam.min1 = 0;
                                rmam.min2 = 0;
                                rmam.min3 = 0;
                                rmam.min4 = 0;
                                rmam.goal = 0;
                                rmam.moedasPossiveis = 0;
                            }
                            rmam.data_atualizacao = reader["data de atualização"].ToString();
                            rmam.cargo = reader["cargo"].ToString() == "" ? "Não Informado" : reader["cargo"].ToString();
                            rmam.cod_gip = reader["cod_gip"].ToString() == "" ? "Não Informado" : reader["cod_gip"].ToString();
                            rmam.setor = reader["setor"].ToString() == "" ? "Não Informado" : reader["setor"].ToString();

                            rmam.cod_gip_reference = reader["COD_GIP_REFERENCE"].ToString() == "" ? "Não Informado" : reader["COD_GIP_REFERENCE"].ToString();
                            rmam.setor_reference = reader["SETOR_REFERENCE"].ToString() == "" ? "Não Informado" : reader["SETOR_REFERENCE"].ToString();
                            rmam.site = reader["site"].ToString();
                            rmam.matricula_gerente_ii = reader["matricula gerente ii"].ToString();
                            rmam.nome_gerente_ii = reader["nome gerente ii"].ToString();
                            rmam.matricula_gerente_i = reader["matricula gerente i"].ToString();
                            rmam.nome_gerente_i = reader["nome gerente i"].ToString();
                            rmam.matricula_diretor = reader["matricula diretor"].ToString();
                            rmam.nome_diretor = reader["nome diretor"].ToString();
                            rmam.conta = reader["CONTA"].ToString();
                            rmam.better = reader["BETTER"].ToString();
                            rmam.resultadoAPI = reader["RESULTADOAPI"].ToString() != "" ? double.Parse(reader["RESULTADOAPI"].ToString()) != 100 ? rmam.resultadoAPI = double.Parse(reader["RESULTADOAPI"].ToString()) : 0 : 0;
                            rmam.diasTrabalhados = reader["TRABALHADO"].ToString() != "" ? reader["TRABALHADO"].ToString() : "-";
                            rmam.diasEscalados = reader["ESCALADO"].ToString() != "" ? reader["ESCALADO"].ToString() : "-";
                            rmam.moedasGanhas = reader["MOEDA_GANHA"].ToString() != "" ? double.Parse(reader["MOEDA_GANHA"].ToString()) : 0;
                            rmam.vemMeta = rmam.meta.ToString() == "" || rmam.min1.ToString() == "" ? 0 : 1;
                            rmam.peso = reader["SCORE"].ToString() != "" ? double.Parse(reader["SCORE"].ToString()) : 0;
                            rmam.Logado = double.Parse(reader["LOGIN"].ToString());
                            rmams.Add(rmam);
                        }
                    }
                }
                connection.Close();
            }
            return rmams;
        }
        public static List<returnResponseConsolidatedSetor> relConsolidatedSector(string dtInicial, string dtFinal, string sectorsAsString, string indicatorsAsString, string indicatorsCestaAsString, bool consolidated, string orderFil, bool Thread = false)
        {
            Funcoes.cestaMetrica cm = Funcoes.getInfMetricBasket(Thread);

            //REALIZA A QUERY QUE RETORNA TODAS AS INFORMAÇÕES DOS COLABORADORES.
            List<ModelsEx.homeRel> rmams = new List<ModelsEx.homeRel>();
            rmams = returnMonetizationAdmMonth(dtInicial, dtFinal, sectorsAsString, indicatorsAsString, orderFil, consolidated, Thread);

            //RETIRANDO OS RESULTADOS DO SUPERVISOR.. ENTENDER COM A TAHTO COMO FICARA ESTA PARTE.
            rmams = rmams.FindAll(item => item.cargo == "AGENTE" || item.cargo == "Não Informado").ToList();

            //CALCULO DE RESULTADO DOS AGENTES
            for (int i = 0; i < rmams.Count; i++)
            {
                ModelsEx.homeRel agente = rmams[i];
                if (agente.idcollaborator == "709846")
                {
                    var parou = true;
                }
                factors factor = new factors();
                factors factor2 = new factors();
                agente = monetizationClass.doCalculationResultHome(agente, true);
                rmams[i] = agente;
            }

            List<ModelsEx.homeRel> listaCestaParaCalculoAgrupados = new List<ModelsEx.homeRel>();
            List<ModelsEx.homeRel> listaCesta = new List<ModelsEx.homeRel>();
            //AGRUPAR POR SETOR, REALIZANDO A CONTA DE CONTEMPLADOS 
            List<ModelsEx.homeRel> agrupadoSetor;
            if (consolidated == true)
            {
                if (indicatorsCestaAsString != "" || indicatorsAsString == "")
                {
                    listaCestaParaCalculoAgrupados = Funcoes.retornaCestaIndicadores(rmams, cm, true, true, false);
                }
                agrupadoSetor = rmams
                        .GroupBy(item => new { item.cod_gip, item.cod_indicador })
                        .Select(grupo => new ModelsEx.homeRel
                        {
                            mes = grupo.First().mes,
                            ano = grupo.First().ano,
                            dateReferer = "",
                            cod_gip = grupo.Key.cod_gip,
                            setor = grupo.First().setor,
                            setor_reference = grupo.First().setor_reference,
                            cod_gip_reference = grupo.First().cod_gip_reference,
                            cod_indicador = grupo.Key.cod_indicador,
                            indicador = grupo.First().indicador,
                            meta = grupo.First().meta,
                            goal = grupo.First().goal,
                            data_atualizacao = grupo.First().data_atualizacao,
                            site = grupo.First().site,
                            matricula_gerente_ii = grupo.First().matricula_gerente_ii,
                            nome_gerente_ii = grupo.First().nome_gerente_ii,
                            matricula_gerente_i = grupo.First().matricula_gerente_i,
                            nome_gerente_i = grupo.First().nome_gerente_i,
                            matricula_diretor = grupo.First().matricula_diretor,
                            nome_diretor = grupo.First().nome_diretor,
                            contemplados_bronze = grupo.Count(item => item.grupo == "Bronze"),
                            contemplados_prata = grupo.Count(item => item.grupo == "Prata"),
                            contemplados_ouro = grupo.Count(item => item.grupo == "Ouro"),
                            contemplados_diamante = grupo.Count(item => item.grupo == "Diamante"),
                            peso = grupo.Max(Item => Item.peso),
                            conta = grupo.First().conta,
                            fator0 = grupo.Sum(item => item.fator0),
                            fator1 = grupo.Sum(item => item.fator1),
                            indicatorType = grupo.First().indicatorType,
                            better = grupo.First().better,
                            min1 = grupo.First().min1,
                            min2 = grupo.First().min2,
                            min3 = grupo.First().min3,
                            min4 = grupo.First().min4,
                            diasTrabalhados = grupo.Max(item2 => item2.diasTrabalhados),
                            diasEscalados = grupo.Max(item2 => item2.diasEscalados),
                            moedasPossiveis = grupo.Sum(item2 => item2.moedasPossiveis),
                            moedasGanhas = grupo.Sum(item2 => Double.IsNaN(item2.moedasGanhas) || Double.IsInfinity(item2.moedasGanhas) ? 0 : item2.moedasGanhas),
                            qtdPessoas = grupo.Count(item2 => item2.resultado > 0),
                            resultadoAPI = grupo.Sum(item2 => item2.resultadoAPI),
                            vemMeta = grupo.Max(item => item.vemMeta),
                            metaSomada = grupo.Sum(item2 => item2.goal),
                            qtdPessoasTotal = grupo.Count(),
                        })
                        .ToList();

                //CALCULO DE CESTA SEPARADO PARA CALCULO DE CONTEMPLADOS
                listaCestaParaCalculoAgrupados = listaCestaParaCalculoAgrupados
                   .GroupBy(item => new { item.cod_gip, item.cod_indicador })
                   .Select(grupo => new ModelsEx.homeRel
                   {
                       dateReferer = grupo.First().dateReferer,
                       cod_gip = grupo.Key.cod_gip,
                       setor = grupo.First().setor,
                       setor_reference = grupo.First().setor_reference,
                       cod_gip_reference = grupo.First().cod_gip_reference,
                       cod_indicador = grupo.Key.cod_indicador,
                       indicador = grupo.First().indicador,
                       contemplados_bronze = grupo.Count(item => item.grupo == "Bronze"),
                       contemplados_prata = grupo.Count(item => item.grupo == "Prata"),
                       contemplados_ouro = grupo.Count(item => item.grupo == "Ouro"),
                       contemplados_diamante = grupo.Count(item => item.grupo == "Diamante"),
                   })
                   .ToList();

                if (indicatorsCestaAsString != "" || indicatorsAsString == "")
                {
                    listaCesta = Funcoes.retornaCestaIndicadores(agrupadoSetor, cm, true, false, true);
                }
            }
            else
            {
                if (indicatorsCestaAsString != "" || indicatorsAsString == "")
                {
                    listaCestaParaCalculoAgrupados = Funcoes.retornaCestaIndicadores(rmams, cm, true, false, false);
                }
                agrupadoSetor = rmams
                    .GroupBy(item => new { item.cod_gip, item.cod_indicador, item.dateReferer })
                    .Select(grupo => new ModelsEx.homeRel
                    {
                        mes = grupo.First().mes,
                        ano = grupo.First().ano,
                        dateReferer = grupo.First().dateReferer,
                        data = grupo.First().data,
                        cod_gip = grupo.Key.cod_gip,
                        setor = grupo.First().setor,
                        setor_reference = grupo.First().setor_reference,
                        cod_gip_reference = grupo.First().cod_gip_reference,
                        cod_indicador = grupo.Key.cod_indicador,
                        indicador = grupo.First().indicador,
                        meta = grupo.First().meta,
                        goal = grupo.First().goal,
                        data_atualizacao = grupo.First().data_atualizacao,
                        site = grupo.First().site,
                        matricula_gerente_ii = grupo.First().matricula_gerente_ii,
                        nome_gerente_ii = grupo.First().nome_gerente_ii,
                        matricula_gerente_i = grupo.First().matricula_gerente_i,
                        nome_gerente_i = grupo.First().nome_gerente_i,
                        matricula_diretor = grupo.First().matricula_diretor,
                        nome_diretor = grupo.First().nome_diretor,
                        contemplados_bronze = grupo.Count(item => item.grupo == "Bronze"),
                        contemplados_prata = grupo.Count(item => item.grupo == "Prata"),
                        contemplados_ouro = grupo.Count(item => item.grupo == "Ouro"),
                        contemplados_diamante = grupo.Count(item => item.grupo == "Diamante"),
                        peso = grupo.Max(Item => Item.peso),
                        conta = grupo.First().conta,
                        fator0 = grupo.Sum(item => item.fator0),
                        fator1 = grupo.Sum(item => item.fator1),
                        indicatorType = grupo.First().indicatorType,
                        better = grupo.First().better,
                        min1 = grupo.First().min1,
                        min2 = grupo.First().min2,
                        min3 = grupo.First().min3,
                        min4 = grupo.First().min4,
                        diasTrabalhados = grupo.Max(item2 => item2.diasTrabalhados),
                        diasEscalados = grupo.Max(item2 => item2.diasEscalados),
                        moedasPossiveis = grupo.Sum(item2 => item2.moedasPossiveis),
                        moedasGanhas = grupo.Sum(item2 => Double.IsNaN(item2.moedasGanhas) || Double.IsInfinity(item2.moedasGanhas) ? 0 : item2.moedasGanhas),
                        qtdPessoas = grupo.Count(item2 => item2.resultado > 0),
                        resultadoAPI = grupo.Sum(item2 => item2.resultadoAPI),
                        vemMeta = grupo.Max(item => item.vemMeta),
                        metaSomada = grupo.Sum(item2 => item2.goal),
                        qtdPessoasTotal = grupo.Count(),
                    })
                    .ToList();

                listaCestaParaCalculoAgrupados = listaCestaParaCalculoAgrupados
                    .GroupBy(item => new { item.cod_gip, item.cod_indicador, item.dateReferer })
                    .Select(grupo => new ModelsEx.homeRel
                    {
                        dateReferer = grupo.First().dateReferer,
                        cod_gip = grupo.Key.cod_gip,
                        setor = grupo.First().setor,
                        setor_reference = grupo.First().setor_reference,
                        cod_gip_reference = grupo.First().cod_gip_reference,
                        cod_indicador = grupo.Key.cod_indicador,
                        indicador = grupo.First().indicador,
                        contemplados_bronze = grupo.Count(item => item.grupo == "Bronze"),
                        contemplados_prata = grupo.Count(item => item.grupo == "Prata"),
                        contemplados_ouro = grupo.Count(item => item.grupo == "Ouro"),
                        contemplados_diamante = grupo.Count(item => item.grupo == "Diamante"),
                    })
                    .ToList();
                if (indicatorsCestaAsString != "" || indicatorsAsString == "")
                {
                    listaCesta = Funcoes.retornaCestaIndicadores(agrupadoSetor, cm, true, false, true);
                }
            }

            for (int i = 0; i < listaCesta.Count; i++)
            {
                ModelsEx.homeRel cesta = listaCesta[i];
                ModelsEx.homeRel cs = new ModelsEx.homeRel();
                if (consolidated == true)
                {
                    cs = listaCestaParaCalculoAgrupados.Find(j => j.cod_gip == cesta.cod_gip);
                }
                else
                {
                    cs = listaCestaParaCalculoAgrupados.Find(j => j.cod_gip == cesta.cod_gip && j.dateReferer == cesta.dateReferer);
                }
                if (cs != null)
                {
                    double valorTotal = cs.contemplados_bronze + cs.contemplados_diamante + cs.contemplados_ouro + cs.contemplados_prata;
                    if (valorTotal > 0)
                    {
                        cesta.contemplados_diamante_Percent = Math.Round((cs.contemplados_diamante / valorTotal) * 100, 2);
                        cesta.contemplados_ouro_Percent = Math.Round((cs.contemplados_ouro / valorTotal) * 100, 2);
                        cesta.contemplados_prata_Percent = Math.Round((cs.contemplados_prata / valorTotal) * 100, 2);
                        cesta.contemplados_bronze_Percent = Math.Round((cs.contemplados_bronze / valorTotal) * 100, 2);
                    }
                    listaCesta[i] = cesta;
                }
            }

            for (int i = 0; i < agrupadoSetor.Count; i++)
            {
                ModelsEx.homeRel agente = agrupadoSetor[i];
                if (agente.cod_indicador == "3")
                {
                    var parou = true;
                }
                factors factor = new factors();
                factors factor2 = new factors();
                agente = monetizationClass.doCalculationResultHome(agente, true);

                //CALCULO PORCENTAGEM CONTEMPLADOS
                double valorTotal = agente.contemplados_bronze + agente.contemplados_diamante + agente.contemplados_ouro + agente.contemplados_prata;
                if (valorTotal > 0)
                {
                    agente.contemplados_diamante_Percent = Math.Round((agente.contemplados_diamante / valorTotal) * 100, 2);
                    agente.contemplados_ouro_Percent = Math.Round((agente.contemplados_ouro / valorTotal) * 100, 2);
                    agente.contemplados_prata_Percent = Math.Round((agente.contemplados_prata / valorTotal) * 100, 2);
                    agente.contemplados_bronze_Percent = Math.Round((agente.contemplados_bronze / valorTotal) * 100, 2);
                }
                agrupadoSetor[i] = agente;
            }

            //FILTRO CESTA DE INDICADORES
            if (indicatorsAsString != "")
            {
                List<string> valoresLista = indicatorsAsString.Split(',').Select(valor => valor.Trim()).ToList();
                agrupadoSetor = agrupadoSetor.FindAll(item => valoresLista.Contains(item.cod_indicador)).ToList();
            }
            if (indicatorsCestaAsString != "" && indicatorsAsString == "")
            {
                agrupadoSetor.Clear();
                agrupadoSetor = agrupadoSetor.Concat(listaCesta).ToList();
            }
            else if (indicatorsCestaAsString != "")
            {
                agrupadoSetor = agrupadoSetor.Concat(listaCesta).ToList();
            }
            else if (indicatorsAsString == "")
            {
                agrupadoSetor = agrupadoSetor.Concat(listaCesta).ToList();
            }

            //RETIRAR ZERADOS
            agrupadoSetor = agrupadoSetor.FindAll(item => item.grupoNum != 0).ToList();

            //ORDENAÇÃO DA LISTA POR GRUPO
            if (orderFil != "")
            {
                List<(string grupo, int order)> grupoOrderList;
                if (orderFil.ToUpper() == "MELHOR")
                {
                    grupoOrderList = new List<(string grupo, int order)>
                    {
                        ("DIAMANTE", 1),
                        ("OURO", 2),
                        ("PRATA", 3),
                        ("BRONZE", 4),
                        ("-", 5)
                    };
                }
                else
                {
                    grupoOrderList = new List<(string grupo, int order)>
                    {
                        ("BRONZE", 1),
                        ("PRATA", 2),
                        ("OURO", 3),
                        ("DIAMANTE", 4),
                        ("-", 5)
                    };
                }
                agrupadoSetor = agrupadoSetor
                    .OrderBy(item =>
                    {
                        string grupo = item.grupo.ToUpper();
                        // Ordene por grupo
                        var order = grupoOrderList.FirstOrDefault(x => x.grupo == grupo);
                        return order.order;
                    })
                    .ToList();
            }

            var jsonData = agrupadoSetor.Select(item => new returnResponseConsolidatedSetor
            {
                Mes = item.mes,
                Ano = item.ano,
                DataReferencia = item.dateReferer,
                //Codigo_Gip = item.cod_gip,
                //Setor = item.setor,
                Codigo_Gip = item.cod_gip_reference,
                Setor = item.setor_reference,
                CodigoGIPSubsetor = item.cod_gip == item.cod_gip_reference ? "-" : item.cod_gip,
                Subsetor =  item.cod_gip == item.cod_gip_reference ? "-" : item.setor,
                Indicador = item.cod_indicador,
                NomeDoIndicador = item.indicador,
                TipoIndicador = item.indicatorType,
                Meta = item.meta.ToString().Replace(".", ","),
                Resultado = Math.Round(item.resultado, 2),
                Percentual = Math.Round(item.porcentual, 2),
                GanhoEmMoedas = item.moedasGanhas,
                MetaMaximaDeMoedas = item.moedasPossiveis,
                Grupo = item.grupo,
                DataAtualizacao = DateTime.Parse(item.data_atualizacao),
                Site = item.site,
                MatriculaGerente1 = item.cargo == "Não Informado" ? "Não Informado" : item.matricula_gerente_i,
                NomeGerente1 = item.cargo == "Não Informado" ? "Não Informado" : item.nome_gerente_i,
                MatriculaGerente2 = item.cargo == "Não Informado" ? "Não Informado" : item.matricula_gerente_ii,
                NomeGerente2 = item.cargo == "Não Informado" ? "Não Informado" : item.nome_gerente_ii,
                MatriculaDiretor = item.cargo == "Não Informado" ? "Não Informado" : item.matricula_diretor,
                NomeDiretor = item.cargo == "Não Informado" ? "Não Informado" : item.nome_diretor,
                ContempladosDiamante = item.contemplados_diamante_Percent,
                ContempladosOuro = item.contemplados_ouro_Percent,
                ContempladosPrata = item.contemplados_prata_Percent,
                ContempladosBronze = item.contemplados_bronze_Percent,
                Score = item.score,
            }).ToList();

            // Use o método Ok() para retornar o objeto serializado em JSON
            return jsonData;
        }
        public class returnResponseConsolidatedSetor
        {
            public string Mes { get; set; }
            public string Ano { get; set; }
            public string DataReferencia { get; set; }
            public string Codigo_Gip { get; set; }
            public string Setor { get; set; }
            public string CodigoGIPSubsetor { get; set; }
            public string Subsetor { get; set; }
            public string Indicador { get; set; }
            public string NomeDoIndicador { get; set; }
            public string TipoIndicador { get; set; }
            public string Meta { get; set; }
            public double Resultado { get; set; }
            public double Percentual { get; set; }
            public double GanhoEmMoedas { get; set; }
            public double MetaMaximaDeMoedas { get; set; }
            public string Grupo { get; set; }
            public DateTime DataAtualizacao { get; set; }
            public string Site { get; set; }
            public string MatriculaGerente1 { get; set; }
            public string NomeGerente1 { get; set; }
            public string MatriculaGerente2 { get; set; }
            public string NomeGerente2 { get; set; }
            public string MatriculaDiretor { get; set; }
            public string NomeDiretor { get; set; }
            public double ContempladosDiamante { get; set; }
            public double ContempladosOuro { get; set; }
            public double ContempladosPrata { get; set; }
            public double ContempladosBronze { get; set; }
            public double Score { get; set; }

        }
        #region Input
        public class InputModel
        {
            public List<Sector> Sectors { get; set; }
            public List<Indicator> Indicators { get; set; }
            public string Type { get; set; }
            public string Order { get; set; }

            public bool Consolidado { get; set; }
            public DateTime DataInicial { get; set; }
            public DateTime DataFinal { get; set; }
        }

        public class Sector
        {
            public int Id { get; set; }
        }

        public class Group
        {
            public int Id { get; set; }
        }

        public class Indicator
        {
            public int Id { get; set; }
        }

        public class Hierarchy
        {
            public int Id { get; set; }
        }
        #endregion
        // POST: api/Results
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {

            string dtInicial = inputModel.DataInicial.ToString("yyyy-MM-dd");
            string dtFinal = inputModel.DataFinal.ToString("yyyy-MM-dd");
            string sectorsAsString = string.Join(",", inputModel.Sectors.Select(g => g.Id));
            string indicatorsAsString = string.Join(",", inputModel.Indicators.Where(i => i.Id.ToString() != "10000012").Select(g => g.Id));
            string indicatorsCestaAsString = string.Join(",", inputModel.Indicators.Where(i => i.Id.ToString() == "10000012").Select(g => g.Id));
            bool consolidated = inputModel.Consolidado;
            string order = inputModel.Order.ToString();
            DateTime dtTimeInicial = DateTime.ParseExact(dtInicial, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dtTimeFinal = DateTime.ParseExact(dtFinal, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            TimeSpan diff = dtTimeFinal.Subtract(dtTimeInicial);
            int diferencaEmDias = (int)diff.TotalDays;
            if (diferencaEmDias > 31)
            {
                return BadRequest("Selecionar uma data de no maximo 1 mês!");
            }
            //if (sectorsAsString == "")
            //{
            //    return BadRequest("Selecione ao menos 1 setor!");
            //}
            var jsonData = relConsolidatedSector(dtInicial, dtFinal, sectorsAsString, indicatorsAsString, indicatorsCestaAsString, consolidated, order);
            return Ok(jsonData);
            
        }

        // Método para serializar um DataTable em JSON

    }
}