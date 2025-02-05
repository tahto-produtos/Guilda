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
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ReportConsolidatedSector1Controller : ApiController
    {

        public List<ModelsEx.homeRel> returnMonetizationAdmMonth(string dtInicial, string dtFinal, string sectors, string indicators, string ordem, bool consolidado)
        {
            // Preparar Filtros
            string filter = "";
            string orderBy = "";
            if (sectors != "")
            {
                filter = filter + $" AND CL.IDGDA_SECTOR IN ({sectors}) ";
            }
            //if (groups != "")
            //{
            //    filter = filter + " ";
            //}
            if (indicators != "")
            {
                filter = filter + $" AND R.INDICADORID IN ({indicators}) ";
            }

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
            stb.Append("SELECT MONTH(@DATAINICIAL) AS MES, ");
            stb.Append("       YEAR(@DATAINICIAL) AS ANO, ");
            stb.Append("       MAX(R.CREATED_AT) AS DATA, ");
            stb.Append("       R.IDGDA_COLLABORATORS AS IDGDA_COLLABORATORS, ");
            stb.Append("       MAX(CB.NAME) AS NAME, ");
            stb.Append("       MAX(CL.CARGO) AS CARGO, ");
            stb.Append("       MAX(IT.TYPE) AS TIPO, ");
            stb.Append("       MAX(R.RESULT) AS RESULTADOAPI, ");
            stb.Append("       MAX(TRAB.TRABALHADO) AS TRABALHADO, ");
            stb.Append("       MAX(ESC.ESCALADO) AS ESCALADO, ");
            stb.Append("       CASE ");
            stb.Append("           WHEN MAX(HIG1.MONETIZATION) IS NULL THEN 0 ");
            stb.Append("           WHEN MAX(MZ.SOMA) IS NULL THEN 0 ");
            stb.Append("           ELSE MAX(MZ.SOMA) ");
            stb.Append("       END AS MOEDA_GANHA, ");
            //stb.Append("       MAX(MZ.SOMA) AS MOEDA_GANHA, ");
            stb.Append("       MAX(HIG1.MONETIZATION) AS META_MAXIMA, ");
            stb.Append("       R.INDICADORID AS 'COD INDICADOR', ");
            stb.Append("       MAX(IT.NAME) AS 'INDICADOR', ");
            stb.Append("       MAX(HIS.GOAL) AS META, ");
            stb.Append("       MAX(SC.WEIGHT_SCORE) AS SCORE, ");
            stb.Append("       MAX(HIG1.METRIC_MIN) AS MIN1, ");
            stb.Append("       MAX(HIG2.METRIC_MIN) AS MIN2, ");
            stb.Append("       MAX(HIG3.METRIC_MIN) AS MIN3, ");
            stb.Append("       MAX(HIG4.METRIC_MIN) AS MIN4, ");
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
            stb.Append("       MAX(CL.SITE) AS SITE, ");
            stb.Append("       MAX(CL.MATRICULA_GERENTE_II) AS 'MATRICULA GERENTE II', ");
            stb.Append("       MAX(CL.NOME_GERENTE_II) AS 'NOME GERENTE II', ");
            stb.Append("       MAX(CL.MATRICULA_GERENTE_I) AS 'MATRICULA GERENTE I', ");
            stb.Append("       MAX(CL.NOME_GERENTE_I) AS 'NOME GERENTE I', ");
            stb.Append("       MAX(CL.MATRICULA_DIRETOR) AS 'MATRICULA DIRETOR', ");
            stb.Append("       MAX(CL.NOME_DIRETOR) AS 'NOME DIRETOR' ");
            stb.Append("FROM GDA_RESULT (NOLOCK) AS R ");
            stb.Append("INNER JOIN GDA_COLLABORATORS (NOLOCK) AS CB ON CB.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            //stb.Append("LEFT JOIN ");
            //stb.Append("  (SELECT CASE ");
            //stb.Append("              WHEN CL.IDGDA_SECTOR IS NOT NULL THEN CL.IDGDA_SECTOR ");
            //stb.Append("              ELSE CL2.IDGDA_SECTOR ");
            //stb.Append("          END AS IDGDA_SECTOR, ");
            //stb.Append("          CASE ");
            //stb.Append("              WHEN CL.HOME_BASED != '' THEN CL.HOME_BASED ");
            //stb.Append("              ELSE CL2.HOME_BASED ");
            //stb.Append("          END AS HOME_BASED, ");
            //stb.Append("          CASE ");
            //stb.Append("              WHEN CL.CARGO IS NOT NULL THEN CL.CARGO ");
            //stb.Append("              ELSE CL2.CARGO ");
            //stb.Append("          END AS CARGO, ");
            //stb.Append("          CASE ");
            //stb.Append("              WHEN CL.ACTIVE IS NOT NULL THEN CL.ACTIVE ");
            //stb.Append("              ELSE CL2.ACTIVE ");
            //stb.Append("          END AS ACTIVE, ");
            //stb.Append("          CASE ");
            //stb.Append("              WHEN CL.SITE != '' THEN CL.SITE ");
            //stb.Append("              ELSE CL2.SITE ");
            //stb.Append("          END AS SITE, ");
            //stb.Append("          CASE ");
            //stb.Append("              WHEN CL.PERIODO != '' THEN CL.PERIODO ");
            //stb.Append("              ELSE CL2.PERIODO ");
            //stb.Append("          END AS PERIODO, ");
            //stb.Append("          CASE ");
            //stb.Append("              WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_SUPERVISOR ");
            //stb.Append("              ELSE CL2.MATRICULA_SUPERVISOR ");
            //stb.Append("          END AS MATRICULA_SUPERVISOR, ");
            //stb.Append("          CASE ");
            //stb.Append("              WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_SUPERVISOR ");
            //stb.Append("              ELSE CL2.NOME_SUPERVISOR ");
            //stb.Append("          END AS NOME_SUPERVISOR, ");
            //stb.Append("          CASE ");
            //stb.Append("              WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_COORDENADOR ");
            //stb.Append("              ELSE CL2.MATRICULA_COORDENADOR ");
            //stb.Append("          END AS MATRICULA_COORDENADOR, ");
            //stb.Append("          CASE ");
            //stb.Append("              WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_COORDENADOR ");
            //stb.Append("              ELSE CL2.NOME_COORDENADOR ");
            //stb.Append("          END AS NOME_COORDENADOR, ");
            //stb.Append("          CASE ");
            //stb.Append("              WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_GERENTE_II ");
            //stb.Append("              ELSE CL2.MATRICULA_GERENTE_II ");
            //stb.Append("          END AS MATRICULA_GERENTE_II, ");
            //stb.Append("          CASE ");
            //stb.Append("              WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_GERENTE_II ");
            //stb.Append("              ELSE CL2.NOME_GERENTE_II ");
            //stb.Append("          END AS NOME_GERENTE_II, ");
            //stb.Append("          CASE ");
            //stb.Append("              WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_GERENTE_I ");
            //stb.Append("              ELSE CL2.MATRICULA_GERENTE_I ");
            //stb.Append("          END AS MATRICULA_GERENTE_I, ");
            //stb.Append("          CASE ");
            //stb.Append("              WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_GERENTE_I ");
            //stb.Append("              ELSE CL2.NOME_GERENTE_I ");
            //stb.Append("          END AS NOME_GERENTE_I, ");
            //stb.Append("          CASE ");
            //stb.Append("              WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_DIRETOR ");
            //stb.Append("              ELSE CL2.MATRICULA_DIRETOR ");
            //stb.Append("          END AS MATRICULA_DIRETOR, ");
            //stb.Append("          CASE ");
            //stb.Append("              WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_DIRETOR ");
            //stb.Append("              ELSE CL2.NOME_DIRETOR ");
            //stb.Append("          END AS NOME_DIRETOR, ");
            //stb.Append("          CASE ");
            //stb.Append("              WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_CEO ");
            //stb.Append("              ELSE CL2.MATRICULA_CEO ");
            //stb.Append("          END AS MATRICULA_CEO, ");
            //stb.Append("          CASE ");
            //stb.Append("              WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_CEO ");
            //stb.Append("              ELSE CL2.NOME_CEO ");
            //stb.Append("          END AS NOME_CEO, ");
            //stb.Append("          C.IDGDA_COLLABORATORS, ");
            //stb.Append("          CL.CREATED_AT ");
            //stb.Append("   FROM GDA_COLLABORATORS (NOLOCK) AS C ");
            //stb.Append("   LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CL ON CL.IDGDA_COLLABORATORS = C.IDGDA_COLLABORATORS ");
            //stb.Append("   AND CL.CREATED_AT >= @DATAINICIAL ");
            //stb.Append("   AND CL.CREATED_AT <= @DATAFINAL ");
            //stb.Append("   LEFT JOIN GDA_COLLABORATORS_LAST_DETAILS (NOLOCK) AS CL2 ON CL2.IDGDA_COLLABORATORS = C.IDGDA_COLLABORATORS) AS CL  ");
            //stb.Append("   ON CL.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS AND CL.CREATED_AT = R.CREATED_AT ");
            stb.Append("");
            stb.Append(" LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CL ON CL.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS AND CL.CREATED_AT = R.CREATED_AT ");
            stb.Append("");
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
            stb.Append("  (SELECT SUM(INPUT) AS SOMA, ");
            //stb.Append("          idgda_sector, ");
            stb.Append("          gda_indicator_idgda_indicator, ");
            stb.Append("          result_date, ");
            stb.Append("          COLLABORATOR_ID ");
            stb.Append("   FROM GDA_CHECKING_ACCOUNT ");
            stb.Append("   WHERE RESULT_DATE >= @DATAINICIAL ");
            stb.Append("     AND RESULT_DATE <= @DATAFINAL ");
            //stb.Append("   GROUP BY idgda_sector, ");
            stb.Append("   GROUP BY  ");
            stb.Append("            gda_indicator_idgda_indicator, ");
            stb.Append("            result_date, ");
            stb.Append("            COLLABORATOR_ID) AS MZ ON ");
            stb.Append(" MZ.gda_indicator_idgda_indicator = R.INDICADORID ");
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
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) AS HIS ON HIS.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND HIS.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.Append("AND CONVERT(DATE,HIS.STARTED_AT) <= R.CREATED_AT ");
            stb.Append("AND CONVERT(DATE,HIS.ENDED_AT) >= R.CREATED_AT ");
            stb.Append("AND HIS.DELETED_AT IS NULL ");
            //FK
            stb.Append("LEFT JOIN GDA_HISTORY_SCORE_INDICATOR_SECTOR (NOLOCK) AS SC ON ");
            stb.Append("SC.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND SC.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.Append("AND CONVERT(DATE,SC.STARTED_AT) <= R.CREATED_AT ");
            stb.Append("AND CONVERT(DATE,SC.ENDED_AT) >= R.CREATED_AT ");


            stb.AppendFormat(" WHERE 1 = 1 AND R.CREATED_AT >= @DATAINICIAL AND R.CREATED_AT <= @DATAFINAL {0} ", filter);
            stb.Append("AND R.DELETED_AT IS NULL ");
            //stb.Append("AND CL.IDGDA_SECTOR IS NOT NULL ");
            //stb.Append("AND CL.CARGO IS NOT NULL ");
            //stb.Append("AND CL.HOME_BASED <> '' ");
            //stb.Append("AND CL.active = 'true' ");

            stb.Append("GROUP BY R.IDGDA_COLLABORATORS, ");
            stb.Append("         R.INDICADORID, ");
            stb.Append("         R.CREATED_AT ");

            #region Antigo
            //stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtInicial);
            //stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFinal);
            //stb.Append(" ");
            //stb.Append("SELECT MONTH(@DATAINICIAL) AS MES, ");
            //stb.Append("       YEAR(@DATAINICIAL) AS ANO, ");
            //stb.Append("       MAX(R.CREATED_AT) AS DATA, ");
            //stb.Append("       R.IDGDA_COLLABORATORS AS IDGDA_COLLABORATORS, ");
            //stb.Append("       MAX(CB.NAME) AS NAME, ");
            //stb.Append("       MAX(LEVELNAME) AS CARGO, ");
            //stb.Append("       MAX(IT.TYPE) AS TIPO, ");
            //stb.Append("        ");
            //stb.Append("	   MAX(R.RESULT) AS RESULTADOAPI, ");
            //stb.Append("       MAX(TRAB.TRABALHADO) AS TRABALHADO, ");
            //stb.Append("       MAX(ESC.ESCALADO) AS ESCALADO, ");
            //stb.Append("	   MAX(MZ.SOMA) AS MOEDA_GANHA, ");
            //stb.Append("       MAX(M.MONETIZATION_G1) AS META_MAXIMA, ");
            //stb.Append(" ");
            //stb.Append("       R.INDICADORID AS 'COD INDICADOR', ");
            //stb.Append("       MAX(IT.NAME) AS 'INDICADOR', ");
            //stb.Append("       MAX(HIS.GOAL) AS META, ");
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
            //stb.Append(" ");
            //stb.Append("       '' AS GRUPO, ");
            //stb.Append("       GETDATE() AS 'Data de atualização', ");
            //stb.Append("       MAX(HCS.IDGDA_SECTOR) AS COD_GIP, ");
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
            //stb.Append("INNER JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS HHR ON HHR.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            //stb.Append("AND CONVERT(DATE, HHR.DATE) = @DATAINICIAL ");
            //stb.Append("INNER JOIN GDA_HISTORY_COLLABORATOR_SECTOR (NOLOCK) AS HCS ON HCS.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            //stb.Append("AND CONVERT(DATE, HCS.CREATED_AT) = @DATAINICIAL ");
            //stb.Append("LEFT JOIN GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME ON HME.INDICATORID = R.INDICADORID ");
            //stb.Append("AND HME.deleted_at IS NULL ");
            //stb.Append("LEFT JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON ME.ID = HME.MATHEMATICALEXPRESSIONID ");
            //stb.Append("AND ME.DELETED_AT IS NULL ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
            //stb.Append("AND HIG1.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIG1.SECTOR_ID = HCS.IDGDA_SECTOR ");
            //stb.Append("AND HIG1.GROUPID = 1 ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG2 ON HIG2.DELETED_AT IS NULL ");
            //stb.Append("AND HIG2.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIG2.SECTOR_ID = HCS.IDGDA_SECTOR ");
            //stb.Append("AND HIG2.GROUPID = 2 ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG3 ON HIG3.DELETED_AT IS NULL ");
            //stb.Append("AND HIG3.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIG3.SECTOR_ID = HCS.IDGDA_SECTOR ");
            //stb.Append("AND HIG3.GROUPID = 3 ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG4 ON HIG4.DELETED_AT IS NULL ");
            //stb.Append("AND HIG4.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIG4.SECTOR_ID = HCS.IDGDA_SECTOR ");
            //stb.Append("AND HIG4.GROUPID = 4 ");
            //stb.Append("LEFT JOIN GDA_CHECKING_ACCOUNT (NOLOCK) AS CA ON CA.COLLABORATOR_ID = R.IDGDA_COLLABORATORS ");
            //stb.Append("AND CA.RESULT_DATE >= @DATAINICIAL ");
            //stb.Append("AND CA.RESULT_DATE <= @DATAFINAL ");
            //stb.Append("AND CA.GDA_INDICATOR_IDGDA_INDICATOR = R.INDICADORID ");
            //stb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS IT ON IT.IDGDA_INDICATOR = R.INDICADORID ");
            //stb.Append("LEFT JOIN ");
            //stb.Append("  (SELECT SUM(INPUT) AS SOMA, ");
            //stb.Append("          idgda_sector, ");
            //stb.Append("          gda_indicator_idgda_indicator, ");
            //stb.Append("          result_date, ");
            //stb.Append("          COLLABORATOR_ID ");
            //stb.Append("   FROM GDA_CHECKING_ACCOUNT ");
            //stb.Append("   WHERE RESULT_DATE >= @DATAINICIAL ");
            //stb.Append("     AND RESULT_DATE <= @DATAFINAL ");
            //stb.Append("   GROUP BY idgda_sector, ");
            //stb.Append("            gda_indicator_idgda_indicator, ");
            //stb.Append("            result_date, ");
            //stb.Append("            COLLABORATOR_ID) AS MZ ON MZ.idgda_sector = HCS.IDGDA_SECTOR ");
            //stb.Append("AND MZ.gda_indicator_idgda_indicator = R.INDICADORID ");
            //stb.Append("AND MZ.result_date = R.CREATED_AT ");
            //stb.Append("AND MZ.COLLABORATOR_ID = R.IDGDA_COLLABORATORS ");
            //stb.Append(" ");
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
            //stb.Append(" ");
            //stb.Append("LEFT JOIN GDA_BASKET_INDICATOR (NOLOCK) AS M ON M.DATE = R.CREATED_AT ");
            //stb.Append("AND M.SECTOR_ID = HCS.IDGDA_SECTOR ");
            //stb.Append("AND M.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = HCS.IDGDA_SECTOR ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) AS HIS ON HIS.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIS.SECTOR_ID = SEC.IDGDA_SECTOR ");
            //stb.Append("AND HIS.DELETED_AT IS NULL ");
            //stb.Append("LEFT JOIN GDA_COLLABORATORS_LAST_DETAILS (NOLOCK) AS CL ON CL.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS ");
            //stb.AppendFormat(" WHERE 1 = 1 AND R.CREATED_AT >= @DATAINICIAL AND R.CREATED_AT <= @DATAFINAL {0} ", filter);
            //stb.Append("GROUP BY R.IDGDA_COLLABORATORS, ");
            //stb.Append("         R.INDICADORID, ");
            //stb.Append("         R.CREATED_AT ");
            #endregion

            List<ModelsEx.homeRel> rmams = new List<ModelsEx.homeRel>();
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
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

                            if (factors == "0.000000;0.000000")
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
           
                            //rmam.diasTrabalhados = reader["dias trabalhados"].ToString();
                            rmam.cod_indicador = reader["cod indicador"].ToString();
                            rmam.indicador = reader["indicador"].ToString();
                            rmam.indicatorType = reader["TIPO"].ToString();
                            rmam.meta = reader["meta"].ToString();
                            //rmam.resultado = double.Parse(reader["resultado"].ToString());
                            //rmam.porcentual = double.Parse(reader["porcentual"].ToString());
                            //if (reader["ganho em moedas"].ToString() != "")
                            //{
                            //    rmam.moedasGanhas = double.Parse(reader["ganho em moedas"].ToString());
                            //}

                            //rmam.moedasPossiveis = int.Parse(reader["meta maxima de moedas"].ToString());
                            //rmam.grupo = reader["grupo"].ToString();
                            rmam.data_atualizacao = reader["data de atualização"].ToString();

                            //rmam.cargo = reader["cargo"].ToString();
                            //rmam.cod_gip = reader["cod_gip"].ToString();
                            //rmam.setor = reader["setor"].ToString();
                            if (reader["cargo"].ToString() == "")
                            {
                                rmam.cargo = "Não Informado";
                            }
                            else
                            {
                                rmam.cargo = reader["cargo"].ToString();
                            }

                            if (reader["cod_gip"].ToString() == "")
                            {
                                rmam.cod_gip = "Não Informado";
                            }
                            else
                            {
                                rmam.cod_gip = reader["cod_gip"].ToString();
                            }

                            if (reader["setor"].ToString() == "")
                            {
                                rmam.setor = "Não Informado";
                            }
                            else
                            {
                                rmam.setor = reader["setor"].ToString();
                            }

                            //if (reader["home_based"].ToString() == "")
                            //{
                            //    rmam.home_based = "Não Informado";
                            //}
                            //else
                            //{
                            //    rmam.home_based = reader["home_based"].ToString();
                            //}




                            //rmam.home = reader["home_based"].ToString();
                            rmam.site = reader["site"].ToString();
                            //rmam.turno = reader["turno"].ToString();
                            //rmam.matricula_supervisor = reader["matricula supervisor"].ToString();
                            //rmam.nome_supervisor = reader["nome supervisor"].ToString();
                            //rmam.matricula_coordenador = reader["matricula coordenador"].ToString();
                            //rmam.nome_coordenador = reader["nome coordenador"].ToString();
                            rmam.matricula_gerente_ii = reader["matricula gerente ii"].ToString();
                            rmam.nome_gerente_ii = reader["nome gerente ii"].ToString();
                            rmam.matricula_gerente_i = reader["matricula gerente i"].ToString();
                            rmam.nome_gerente_i = reader["nome gerente i"].ToString();
                            rmam.matricula_diretor = reader["matricula diretor"].ToString();
                            rmam.nome_diretor = reader["nome diretor"].ToString();
                            //rmam.matricula_ceo = reader["matricula ceo"].ToString();
                            //rmam.nome_ceo = reader["nome ceo"].ToString();

                            //if (reader["FATOR1"].ToString() != "")
                            //{
                            //    rmam.fator0 = double.Parse(reader["FATOR1"].ToString());
                            //}
                            //if (reader["FATOR2"].ToString() != "")
                            //{
                            //    rmam.fator1 = double.Parse(reader["FATOR2"].ToString());
                            //}


                            if (reader["min1"].ToString() != "")
                            {
                                rmam.min1 = double.Parse(reader["min1"].ToString());
                            }
                            if (reader["min2"].ToString() != "")
                            {
                                rmam.min2 = double.Parse(reader["min2"].ToString());
                            }
                            if (reader["min3"].ToString() != "")
                            {
                                rmam.min3 = double.Parse(reader["min3"].ToString());
                            }
                            if (reader["min4"].ToString() != "")
                            {
                                rmam.min4 = double.Parse(reader["min4"].ToString());
                            }

                            rmam.conta = reader["CONTA"].ToString();
                            rmam.better = reader["BETTER"].ToString();

                            //string diasTrabalhados = reader["DIAS TRABALHADOS"].ToString();
                            //string metaMaxima = reader["META MAXIMA DE MOEDAS"].ToString();

                            //if (diasTrabalhados != "" && metaMaxima != "")
                            //{
                            //    rmam.meta_maxima_de_moedas_double = Convert.ToDouble(diasTrabalhados) * Convert.ToDouble(metaMaxima);
                            //}

                            if (reader["meta"].ToString() != "")
                            {
                                rmam.goal = double.Parse(reader["meta"].ToString());
                            }
                            else
                            {
                                //Não veio informação
                                rmam.goal = 0;
                            }

                            if (reader["RESULTADOAPI"].ToString() != "")
                            {
                                if (double.Parse(reader["RESULTADOAPI"].ToString()) != 100)
                                {
                                    rmam.resultadoAPI = double.Parse(reader["RESULTADOAPI"].ToString());
                                }
                                else
                                {
                                    rmam.resultadoAPI = 0;
                                }
                            }
                            else
                            {
                                //Não veio informação
                                rmam.resultadoAPI = 0;
                            }

                            if (reader["TRABALHADO"].ToString() != "")
                            {
                                rmam.diasTrabalhados = reader["TRABALHADO"].ToString();
                            }
                            else
                            {
                                //Não veio informação
                                rmam.diasTrabalhados = "-";
                            }


                            if (reader["ESCALADO"].ToString() != "")
                            {
                                rmam.diasEscalados = reader["ESCALADO"].ToString();
                            }
                            else
                            {
                                //Não veio informação
                                rmam.diasEscalados = "-";
                            }

                            if (reader["META_MAXIMA"].ToString() != "")
                            {
                                rmam.moedasPossiveis = int.Parse(reader["META_MAXIMA"].ToString());
                            }
                            else
                            {
                                //Não veio informação
                                rmam.moedasPossiveis = 0;
                            }

                            if (reader["MOEDA_GANHA"].ToString() != "")
                            {
                                rmam.moedasGanhas = double.Parse(reader["MOEDA_GANHA"].ToString());
                            }
                            else
                            {
                                //Não veio informação
                                rmam.moedasGanhas = 0;
                            }

                            if (reader["meta"].ToString() == "" || reader["min1"].ToString() == "")
                            {
                                rmam.vemMeta = 0;
                                rmam.moedasPossiveis = 0;
                            }
                            else
                            {
                                rmam.vemMeta = 1;
                            }

                            if (reader["SCORE"].ToString() != "")
                            {
                                rmam.peso = double.Parse(reader["SCORE"].ToString());
                            }
                            else
                            {
                                rmam.peso = 0;
                            }

                            rmams.Add(rmam);
                        }
                    }
                }
                connection.Close();
            }

            return rmams;
        }


        public class returnResponseConsolidatedSetor
        {
            public string Mes { get; set; }
            public string Ano { get; set; }
            public string DataReferencia { get; set; }
            public string Codigo_Gip { get; set; }
            public string Setor { get; set; }
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

        public factors returnCoins(string idCollaborator, string dataInicial, string idIndicator, string idSector, factors factor, Boolean calculoDiaDia, string dataFinal)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}';  ", idCollaborator);
            sb.AppendFormat("DECLARE @DATEENV DATE; SET @DATEENV = '{0}';  ", dataInicial);
            sb.AppendFormat("DECLARE @DATEFIM DATE; SET @DATEFIM = '{0}';  ", dataFinal);
            sb.AppendFormat("DECLARE @INDICADORID VARCHAR(MAX); SET @INDICADORID = '{0}';  ", idIndicator);
            sb.AppendFormat("DECLARE @IDGDA_SECTOR VARCHAR(MAX); SET @IDGDA_SECTOR = '{0}'; ", idSector);
            sb.Append("	 SELECT SUM(INPUT) AS INPUT  FROM GDA_CHECKING_ACCOUNT (NOLOCK)  ");
            sb.Append("  where idgda_sector = @IDGDA_SECTOR AND gda_indicator_idgda_indicator = @INDICADORID  ");
            sb.Append("  AND result_date >= @DateEnv AND result_date <= @DATEFIM  ");
            sb.Append("  AND COLLABORATOR_ID = @INPUTID  ");


            
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                factor.coins = reader["INPUT"].ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return factor;
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

        //public static RelMonetizationAdmMonth retornaMetaMaximaMoedas(string dataInicial, string dataFinal, string codSetor, string codIndicator, string userInside, RelMonetizationAdmMonth rmam)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}';  ", dataInicial);
        //    sb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}';  ", dataFinal);
        //    sb.AppendFormat("DECLARE @SECTOR_ID int; SET @SECTOR_ID = {0};  ", codSetor);
        //    sb.AppendFormat("DECLARE @INDICATOR_ID int; SET @INDICATOR_ID = {0};  ", codIndicator);
        //    sb.Append("SELECT SUM(CountResult * MaxQTD) AS TotalSum ");
        //    sb.Append("FROM ( ");
        //    sb.Append("    SELECT COUNT(0) AS CountResult, MAX(m.QTD) AS MaxQTD ");
        //    sb.Append("    FROM GDA_RESULT R (NOLOCK) ");
        //    sb.Append("    INNER JOIN ( ");
        //    sb.Append("        SELECT SUM(MONETIZATION_G1 * WEIGHT) AS QTD, DATE ");
        //    sb.Append("        FROM GDA_BASKET_INDICATOR ");
        //    sb.Append("        WHERE SECTOR_ID = @SECTOR_ID AND INDICATOR_ID = @INDICATOR_ID AND DATE >= @DATAINICIAL AND DATE <= @DATAFINAL ");
        //    sb.Append("        GROUP BY DATE ");
        //    sb.Append("    ) AS M ON M.DATE = R.CREATED_AT ");
        //    sb.Append("    WHERE INDICADORID = -1 ");
        //    sb.AppendFormat("    AND IDGDA_COLLABORATORS IN ({0}) ", userInside);
        //    sb.Append("    AND R.CREATED_AT >= @DATAINICIAL AND R.CREATED_AT <= @DATAFINAL ");
        //    sb.Append("    AND FACTORS = '1.000000;0.000000' AND DELETED_AT IS NULL ");
        //    sb.Append("    GROUP BY R.CREATED_AT ");
        //    sb.Append(") AS Subquery; ");



        
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();
        //        try
        //        {
        //            using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
        //            {
        //                using (SqlDataReader reader = command.ExecuteReader())
        //                {
        //                    if (reader.Read())
        //                    {
        //                        rmam.moedasPossiveis = int.Parse(reader["TotalSum"].ToString());
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //        connection.Close();
        //    }

        //    return rmam;
        //}

        // POST: api/Results
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {

            string dtInicial = inputModel.DataInicial.ToString("yyyy-MM-dd");
            string dtFinal = inputModel.DataFinal.ToString("yyyy-MM-dd");
            string sectorsAsString = string.Join(",", inputModel.Sectors.Select(g => g.Id));
            //string indicatorsAsString = string.Join(",", inputModel.Indicators.Select(g => g.Id));
            string indicatorsAsString = string.Join(",", inputModel.Indicators.Where(i => i.Id.ToString() != "10000012").Select(g => g.Id));
            string indicatorsCestaAsString = string.Join(",", inputModel.Indicators.Where(i => i.Id.ToString() == "10000012").Select(g => g.Id));
            bool consolidated = inputModel.Consolidado;
            DateTime dtTimeInicial = DateTime.ParseExact(dtInicial, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dtTimeFinal = DateTime.ParseExact(dtFinal, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            //string dtInicial = HttpContext.Current.Request.Form["DataInicio"];
            //string dtFinal = HttpContext.Current.Request.Form["DataFim"];
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

            Funcoes.cestaMetrica cm = Funcoes.getInfMetricBasket();

            //Pega informação monetização Hierarquia
            //List<ModelsEx.monetizacaoHierarquia> lMone = new List<ModelsEx.monetizacaoHierarquia>();
            //lMone = Funcoes.retornaListaMonetizacaoHierarquia(dtInicial, dtFinal);

            //Realiza a query que retorna todas as informações dos colaboradores.
            List<ModelsEx.homeRel> rmams = new List<ModelsEx.homeRel>();
            rmams = returnMonetizationAdmMonth(dtInicial, dtFinal, sectorsAsString, indicatorsAsString, inputModel.Order, consolidated);

            //Retirando os resultados do supervisor.. Entender com a Tahto como ficara esta parte.
            rmams = rmams.FindAll(item => item.cargo == "AGENTE" || item.cargo == "Não Informado").ToList();


            //Retirando os resultados que não vieram informação de hierarquia
            //rmams = rmams.FindAll(item => item.matricula_supervisor != "0" ||
            //                              item.matricula_coordenador != "0" ||
            //                              item.matricula_gerente_i != "0" ||
            //                              item.matricula_gerente_ii != "0" ||
            //                              item.matricula_diretor != "0" ||
            //                              item.matricula_ceo != "0" 
            //                              ).ToList();

            for (int i = 0; i < rmams.Count; i++)
            {

                ModelsEx.homeRel agente = rmams[i];

                if (agente.idcollaborator == "709846")
                {
                    var parou = true;
                }

                factors factor = new factors();
                factors factor2 = new factors();

                agente = monetizationClass.doCalculationResultHome(agente, false);

                rmams[i] = agente;
            }

            List<ModelsEx.homeRel> listaCestaParaCalculoAgrupados = new List<ModelsEx.homeRel>();


            List<ModelsEx.homeRel> listaCesta = new List<ModelsEx.homeRel>();
            //Agrupar por setor, realizando a conta de contemplados 
            List<ModelsEx.homeRel> agrupadoSetor;

            if (consolidated == true)
            {
                
                if (indicatorsCestaAsString != "" || indicatorsAsString == "")
                {
                    listaCestaParaCalculoAgrupados = Funcoes.retornaCestaIndicadores(rmams, cm, true, true, false);
                }
                //listaCestaParaCalculoAgrupados = Funcoes.retornaCestaIndicadores(rmams, cm, true, true, false);

            agrupadoSetor = rmams
                    .GroupBy(item => new { item.cod_gip, item.cod_indicador })
                    .Select(grupo => new ModelsEx.homeRel
                    {
                        mes = grupo.First().mes,
                        ano = grupo.First().ano,
                        dateReferer = "",
                        cod_gip = grupo.Key.cod_gip,
                        setor = grupo.First().setor,
                        cod_indicador = grupo.Key.cod_indicador,
                        indicador = grupo.First().indicador,
                        meta = grupo.First().meta,
                        goal = grupo.First().goal,
                        //moedasPossiveis = grupo.Sum(item => item.moedasPossiveis),

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
                        //score = grupo.Max(item => item.score),
                        peso = grupo.Max(Item => Item.peso),
                        //moedasGanhas = grupo.Sum(item => item.moedasGanhas),
                        //Para calcular a meta maxima de moedas
                        //userInside = string.Join(",", grupo.Select(g => g.idcollaborator)),

                        conta = grupo.First().conta,
                        fator0 = grupo.Sum(item => item.fator0),
                        fator1 = grupo.Sum(item => item.fator1),
                        indicatorType = grupo.First().indicatorType,
                        better = grupo.First().better,
                        min1 = grupo.First().min1,
                        min2 = grupo.First().min2,
                        min3 = grupo.First().min3,
                        min4 = grupo.First().min4,
                       //peso = grupo.Max(item =>item.score)


                        diasTrabalhados = grupo.Max(item2 => item2.diasTrabalhados),
                        diasEscalados = grupo.Max(item2 => item2.diasEscalados),
                        moedasPossiveis = grupo.Sum(item2 => item2.moedasPossiveis),
                        //moedasGanhas = grupo.Sum(item2 => item2.moedasGanhas),
                        moedasGanhas = grupo.Sum(item2 => Double.IsNaN(item2.moedasGanhas) || Double.IsInfinity(item2.moedasGanhas) ? 0 : item2.moedasGanhas),
                        qtdPessoas = grupo.Count(item2 => item2.resultado > 0),
                        resultadoAPI = grupo.Sum(item2 => item2.resultadoAPI),
                        vemMeta = grupo.Max(item => item.vemMeta),
                        metaSomada = grupo.Sum(item2 => item2.goal),
                        qtdPessoasTotal = grupo.Count(),
                    })
                    .ToList();

                //Calculo de cesta separado para calculo de contemplados
                listaCestaParaCalculoAgrupados = listaCestaParaCalculoAgrupados
                   .GroupBy(item => new { item.cod_gip, item.cod_indicador })
                   .Select(grupo => new ModelsEx.homeRel
                   {
                       dateReferer = grupo.First().dateReferer,
                       cod_gip = grupo.Key.cod_gip,
                       setor = grupo.First().setor,
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
                //listaCesta = Funcoes.retornaCestaIndicadores(agrupadoSetor, cm, true, false, true);
            }
            else
            {
                //listaCestaParaCalculoAgrupados = Funcoes.retornaCestaIndicadores(rmams, cm, true, false, false);
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
                        cod_indicador = grupo.Key.cod_indicador,
                        indicador = grupo.First().indicador,
                        meta = grupo.First().meta,
                        goal = grupo.First().goal,
                        //moedasPossiveis = grupo.Sum(item => item.moedasPossiveis),

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
                        //moedasGanhas = grupo.Sum(item => item.moedasGanhas),
                        //Para calcular a meta maxima de moedas
                        //userInside = string.Join(",", grupo.Select(g => g.idcollaborator)),

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
                        //moedasGanhas = grupo.Sum(item2 => item2.moedasGanhas),
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
                //listaCesta = Funcoes.retornaCestaIndicadores(agrupadoSetor, cm, true, false, true);
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

                //agente = retornaMetaMaximaMoedas(dtInicial, dtFinal, agente.cod_gip, agente.codIndicador, agente.userInside, agente);

                //Calculo Porcentagem Contemplados
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

            //Filtro Cesta
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
            //agrupadoSetor = agrupadoSetor.Concat(listaCesta).ToList();

            //Retirar zerados
            agrupadoSetor = agrupadoSetor.FindAll(item => item.grupoNum != 0).ToList();

            if (inputModel.Order != "")
            {
                List<(string grupo, int order)> grupoOrderList;
                if (inputModel.Order.ToUpper() == "MELHOR")
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
                //if (inputModel.Order.ToUpper() == "MELHOR")
                //{
                //    if (consolidated == true)
                //    {
                //        agrupadoSetor = agrupadoSetor.OrderByDescending(item => item.cod_indicador).OrderByDescending(item => item.porcentual).OrderBy(item => item.grupoNum).ToList();
                //    }
                //    else
                //    {
                //        agrupadoSetor = agrupadoSetor.OrderByDescending(item => item.cod_indicador).OrderByDescending(item => item.dateReferer).OrderByDescending(item => item.porcentual).OrderBy(item => item.grupoNum).ToList();
                //    }
                //}
                //else
                //{
                //    if (consolidated == true)
                //    {
                //        agrupadoSetor = agrupadoSetor.OrderByDescending(item => item.cod_indicador).OrderBy(item => item.porcentual).OrderByDescending(item => item.grupoNum).ToList();
                //    }
                //    else
                //    {
                //        agrupadoSetor = agrupadoSetor.OrderByDescending(item => item.dateReferer).OrderByDescending(item => item.cod_indicador).OrderBy(item => item.porcentual).OrderByDescending(item => item.grupoNum).ToList();
                //    }
                //}


            }

            var jsonData = agrupadoSetor.Select(item => new returnResponseConsolidatedSetor
            {
                Mes = item.mes,
                Ano = item.ano,
                DataReferencia = item.dateReferer,
                Codigo_Gip = item.cod_gip,
                Setor = item.setor,
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
            return Ok(jsonData);
        }

        // Método para serializar um DataTable em JSON

    }
}