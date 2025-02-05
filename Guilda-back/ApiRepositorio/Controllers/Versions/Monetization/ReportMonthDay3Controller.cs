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
using Microsoft.CodeAnalysis.CSharp.Syntax;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ReportMonthDay3Controller : ApiController
    {
        public static List<ModelsEx.homeRel> returnMonetizationDayMonth(string dtInicial, string dtFinal, string sectors, string indicators, string collaborators, string hierarchies, string ordem, bool Thread = false)
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
            // FILTRO POR COLABORATORES
            if (collaborators != "")
            {
                StringBuilder stb2 = new StringBuilder();
                stb2.AppendFormat(" AND (CL.IDGDA_COLLABORATORS IN ({0}) OR  ", collaborators);
                stb2.AppendFormat("	    CL.MATRICULA_SUPERVISOR IN ({0}) OR ", collaborators);
                stb2.AppendFormat("		CL.MATRICULA_COORDENADOR IN ({0}) OR ", collaborators);
                stb2.AppendFormat("		CL.MATRICULA_GERENTE_II IN ({0}) OR ", collaborators);
                stb2.AppendFormat("		CL.MATRICULA_GERENTE_I IN ({0}) OR ", collaborators);
                stb2.AppendFormat("		CL.MATRICULA_DIRETOR IN ({0}) OR ", collaborators);
                stb2.AppendFormat("		CL.MATRICULA_CEO IN ({0})) ", collaborators);
                filter = filter + $" {stb2.ToString()} ";
            }

            //CONSULTA NO BANCO DO RELATORIO DE MONETIZAÇÃO DIARIO.
            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtInicial);
            stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFinal);
            stb.Append("SELECT MAX(CONVERT(DATE, CA.CREATED_AT)) AS 'DATA DO PAGAMENTO', ");
            stb.Append("       R.IDGDA_COLLABORATORS AS 'MATRICULA', ");
            stb.Append("       MAX(CB.NAME) AS NAME, ");
            stb.Append("       MAX(CL.CARGO) AS CARGO, ");
            stb.Append("       MAX(IT.TYPE) AS TIPO, ");
            stb.Append("       MAX(CONVERT(DATE, R.CREATED_AT)) AS 'REFERENCIA PAGAMENTO', ");
            stb.Append("       R.INDICADORID AS 'COD INDICADOR', ");
            stb.Append("       MAX(IT.NAME) AS 'INDICADOR',  ");
            stb.Append("	   MAX(HIS.GOAL) AS META, ");
            stb.Append("	   MAX(HIS.GOAL_NIGHT) AS META_NOTURNA, ");
            stb.Append("	   MAX(HIS.GOAL_LATENIGHT) AS META_MADRUGADA,  ");
            stb.Append("	   MAX(SC.WEIGHT_SCORE) AS SCORE, ");
            stb.Append("	   MAX(HIG1.METRIC_MIN) AS MIN1, ");
            stb.Append("	   MAX(HIG2.METRIC_MIN) AS MIN2, ");
            stb.Append("	   MAX(HIG3.METRIC_MIN) AS MIN3, ");
            stb.Append("	   MAX(HIG4.METRIC_MIN) AS MIN4, ");
            stb.Append("	   MAX(HIG1.METRIC_MIN_NIGHT) AS MIN1_NOTURNO, ");
            stb.Append("	   MAX(HIG2.METRIC_MIN_NIGHT) AS MIN2_NOTURNO, ");
            stb.Append("	   MAX(HIG3.METRIC_MIN_NIGHT) AS MIN3_NOTURNO, ");
            stb.Append("	   MAX(HIG4.METRIC_MIN_NIGHT) AS MIN4_NOTURNO, ");
            stb.Append("	   MAX(HIG1.METRIC_MIN_LATENIGHT) AS MIN1_MADRUGADA, ");
            stb.Append("	   MAX(HIG2.METRIC_MIN_LATENIGHT) AS MIN2_MADRUGADA, ");
            stb.Append("	   MAX(HIG3.METRIC_MIN_LATENIGHT) AS MIN3_MADRUGADA, ");
            stb.Append("	   MAX(HIG4.METRIC_MIN_LATENIGHT) AS MIN4_MADRUGADA, ");
            stb.Append("	   MAX(R.FACTORS) AS FATOR, ");
            stb.Append("	   CASE ");
            stb.Append("	       WHEN MAX(ME.EXPRESSION) IS NULL THEN '(#FATOR1/#FATOR0)' ");
            stb.Append("	       ELSE MAX(ME.EXPRESSION) ");
            stb.Append("	   END AS CONTA, ");
            stb.Append("	   CASE ");
            stb.Append("	       WHEN MAX(IT.CALCULATION_TYPE) IS NULL THEN 'BIGGER_BETTER' ");
            stb.Append("	       ELSE MAX(IT.CALCULATION_TYPE) ");
            stb.Append("	   END AS BETTER, ");
            stb.Append("	   MAX(IT.TYPE) AS TYPE, ");
            stb.Append("	   '' AS RESULTADO, ");
            stb.Append("	   '' AS PORCENTUAL, ");
            stb.Append("	   MAX(IT.TYPE) AS TYPE, ");
            stb.Append("	   MAX(R.RESULT) AS RESULTADOAPI, ");
            stb.Append("	   MAX(TRAB.TRABALHADO) AS TRABALHADO, ");
            stb.Append("	   MAX(ESC.ESCALADO) AS ESCALADO, ");
            stb.Append("       MAX(ISNULL(HIG1.MONETIZATION,0)) AS META_MAXIMA, ");
            stb.Append("       MAX(ISNULL(HIG1.MONETIZATION_NIGHT,0)) AS META_MAXIMA_NOTURNA, ");
            stb.Append("       MAX(ISNULL(HIG1.MONETIZATION_LATENIGHT,0)) AS META_MAXIMA_MADRUGADA, ");
            stb.Append("	   CASE  ");
            stb.Append("	       WHEN MAX(MZ.INPUT) IS NULL THEN 0 ");
            stb.Append("	       ELSE MAX(MZ.INPUT) ");
            stb.Append("	   END AS MOEDA_GANHA, ");
            stb.Append("	   '' AS GRUPO, ");
            stb.Append("	   GETDATE() AS 'Data de atualização',  ");
            stb.Append("	   MAX(CL.IDGDA_SECTOR) AS COD_GIP, ");
            stb.Append("	   MAX(SEC.NAME) AS SETOR, ");
            stb.Append("       MAX(CL.IDGDA_SECTOR_REFERENCE) AS COD_GIP_REFERENCE, ");
            stb.Append("       MAX(SECREFERENCE.NAME) AS SETOR_REFERENCE, ");

            stb.Append("	   MAX(CL.IDGDA_SECTOR_SUPERVISOR) AS COD_GIP_SUPERVISOR,  ");
            stb.Append("	   MAX(SECSUP.NAME) AS SETOR_SUPERVISOR, ");
            stb.Append("	   MAX(CL.HOME_BASED) AS HOME_BASED, ");
            stb.Append("	   MAX(CL.SITE) AS SITE, ");
            stb.Append("	   MAX(CL.PERIODO) AS TURNO, ");
            stb.Append("	   MAX(CL.MATRICULA_SUPERVISOR) AS 'MATRICULA SUPERVISOR', ");
            stb.Append("	   MAX(CL.NOME_SUPERVISOR) AS 'NOME SUPERVISOR', ");
            stb.Append("	   MAX(CL.MATRICULA_COORDENADOR) AS 'MATRICULA COORDENADOR', ");
            stb.Append("	   MAX(CL.NOME_COORDENADOR) AS 'NOME COORDENADOR', ");
            stb.Append("	   MAX(CL.MATRICULA_GERENTE_II) AS 'MATRICULA GERENTE II', ");
            stb.Append("	   MAX(CL.NOME_GERENTE_II) AS 'NOME GERENTE II', ");
            stb.Append("	   MAX(CL.MATRICULA_GERENTE_I) AS 'MATRICULA GERENTE I', ");
            stb.Append("	   MAX(CL.NOME_GERENTE_I) AS 'NOME GERENTE I', ");
            stb.Append("	   MAX(CL.MATRICULA_DIRETOR) AS 'MATRICULA DIRETOR', ");
            stb.Append("	   MAX(CL.NOME_DIRETOR) AS 'NOME DIRETOR', ");
            stb.Append("	   MAX(CL.MATRICULA_CEO) AS 'MATRICULA CEO', ");
            stb.Append("	   MAX(CL.NOME_CEO) AS 'NOME CEO' ");
            stb.Append("FROM GDA_RESULT (NOLOCK) AS R ");
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
            stb.Append("LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS CB ON CB.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
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
            stb.Append("AND CA.RESULT_DATE = R.CREATED_AT ");
            stb.Append("AND CA.GDA_INDICATOR_IDGDA_INDICATOR = R.INDICADORID ");
            stb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS IT ON IT.IDGDA_INDICATOR = R.INDICADORID ");
            stb.Append("LEFT JOIN ");
            stb.Append("  (SELECT SUM(INPUT) - SUM(OUTPUT) AS INPUT, ");
            stb.Append("          gda_indicator_idgda_indicator, ");
            stb.Append("          result_date, ");
            stb.Append("          COLLABORATOR_ID ");
            stb.Append("   FROM GDA_CHECKING_ACCOUNT ");
            stb.Append("   WHERE CONVERT(DATE,RESULT_DATE) >= @DATAINICIAL ");
            stb.Append("     AND CONVERT(DATE,RESULT_DATE) <= @DATAFINAL ");
            stb.Append("   GROUP BY gda_indicator_idgda_indicator, ");
            stb.Append("            result_date, ");
            stb.Append("            COLLABORATOR_ID) AS MZ ON MZ.gda_indicator_idgda_indicator = R.INDICADORID ");
            stb.Append("AND MZ.result_date = R.CREATED_AT ");
            stb.Append("AND MZ.COLLABORATOR_ID = R.IDGDA_COLLABORATORS ");
            stb.Append("LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = CL.IDGDA_SECTOR ");
            stb.Append("LEFT JOIN GDA_SECTOR (NOLOCK) AS SECSUP ON SECSUP.IDGDA_SECTOR = CL.IDGDA_SECTOR_SUPERVISOR ");
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
            stb.Append("WHERE 1 = 1 ");
            stb.Append("  AND R.CREATED_AT >= @DATAINICIAL ");
            stb.Append("  AND R.CREATED_AT <= @DATAFINAL ");
            stb.Append("  AND R.DELETED_AT IS NULL ");
            //stb.Append("  AND CL.active = 'true' ");
            stb.Append("  AND ((CL.periodo = 'DIURNO' ");
            stb.Append("        AND HIG1.MONETIZATION > 0) ");
            stb.Append("       OR (CL.periodo = 'NOTURNO' ");
            stb.Append("           AND HIG1.MONETIZATION_NIGHT > 0) ");
            stb.Append("       OR (CL.periodo = 'MADRUGADA' ");
            stb.Append("           AND HIG1.MONETIZATION_LATENIGHT > 0)) ");
            stb.Append("  AND R.FACTORS <> '0.000000;0.000000' ");
            stb.Append("  AND R.FACTORS <> '0.000000; 0.000000' ");
            stb.AppendFormat(" {0} ", filter);
            stb.Append(" ");
            stb.Append("GROUP BY R.IDGDA_COLLABORATORS, ");
            stb.Append("         R.INDICADORID, ");
            stb.Append("         CONVERT(DATE, R.CREATED_AT) ");

            #region Antigo
            //stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtInicial);
            //stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFinal);
            //stb.Append(" ");
            //stb.Append("SELECT MAX(CONVERT(DATE, CA.CREATED_AT)) AS 'DATA DO PAGAMENTO', ");
            //stb.Append("       R.IDGDA_COLLABORATORS AS 'MATRICULA', ");
            //stb.Append("       MAX(CB.NAME) AS NAME, ");
            //stb.Append("       MAX(CL.CARGO) AS CARGO, ");
            //stb.Append("       MAX(IT.TYPE) AS TIPO, ");
            //stb.Append("       MAX(CONVERT(DATE, R.CREATED_AT)) AS 'REFERENCIA PAGAMENTO', ");
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
            //stb.Append("       MAX(IT.TYPE) AS TYPE, ");
            //stb.Append("       '' AS RESULTADO, ");
            //stb.Append("       '' AS PORCENTUAL, ");
            //stb.Append("       MAX(IT.TYPE) AS TYPE, ");
            //stb.Append("       MAX(R.RESULT) AS RESULTADOAPI, ");
            //stb.Append("       MAX(TRAB.TRABALHADO) AS TRABALHADO, ");
            //stb.Append("       MAX(ESC.ESCALADO) AS ESCALADO, ");
            //stb.Append("       MAX(HIG1.MONETIZATION) AS META_MAXIMA, ");
            //stb.Append("       CASE ");
            //stb.Append("           WHEN MAX(HIG1.MONETIZATION) IS NULL THEN 0 ");
            //stb.Append("           WHEN MAX(MZ.INPUT) IS NULL THEN 0 ");
            //stb.Append("           ELSE MAX(MZ.INPUT) ");
            //stb.Append("       END AS MOEDA_GANHA, ");
            //stb.Append("       '' AS GRUPO, ");
            //stb.Append("       GETDATE() AS 'Data de atualização', ");
            //stb.Append("       MAX(CL.IDGDA_SECTOR) AS COD_GIP, ");
            //stb.Append("       MAX(CL.IDGDA_SECTOR_SUPERVISOR) AS COD_GIP_SUPERVISOR, ");
            //stb.Append("       MAX(SEC.NAME) AS SETOR, ");
            //stb.Append("       MAX(SECSUP.NAME) AS SETOR_SUPERVISOR, ");
            //stb.Append("       MAX(CL.HOME_BASED) AS HOME_BASED, ");
            //stb.Append("       MAX(CL.SITE) AS SITE, ");
            //stb.Append("       MAX(CL.PERIODO) AS TURNO, ");
            //stb.Append("       MAX(CL.MATRICULA_SUPERVISOR) AS 'MATRICULA SUPERVISOR', ");
            //stb.Append("       MAX(CL.NOME_SUPERVISOR) AS 'NOME SUPERVISOR', ");
            //stb.Append("       MAX(CL.MATRICULA_COORDENADOR) AS 'MATRICULA COORDENADOR', ");
            //stb.Append("       MAX(CL.NOME_COORDENADOR) AS 'NOME COORDENADOR', ");
            //stb.Append("       MAX(CL.MATRICULA_GERENTE_II) AS 'MATRICULA GERENTE II', ");
            //stb.Append("       MAX(CL.NOME_GERENTE_II) AS 'NOME GERENTE II', ");
            //stb.Append("       MAX(CL.MATRICULA_GERENTE_I) AS 'MATRICULA GERENTE I', ");
            //stb.Append("       MAX(CL.NOME_GERENTE_I) AS 'NOME GERENTE I', ");
            //stb.Append("       MAX(CL.MATRICULA_DIRETOR) AS 'MATRICULA DIRETOR', ");
            //stb.Append("       MAX(CL.NOME_DIRETOR) AS 'NOME DIRETOR', ");
            //stb.Append("       MAX(CL.MATRICULA_CEO) AS 'MATRICULA CEO', ");
            //stb.Append("       MAX(CL.NOME_CEO) AS 'NOME CEO' ");
            //stb.Append("FROM GDA_RESULT (NOLOCK) AS R ");
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
            //stb.Append("LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS CB ON CB.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");



            //stb.Append("LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CL ON CL.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS ");
            //stb.Append("AND CL.CREATED_AT = R.CREATED_AT ");
            ////stb.Append("LEFT JOIN ( ");
            ////stb.Append("    SELECT  ");
            ////stb.Append("        CASE  ");
            ////stb.Append("            WHEN CL.IDGDA_SECTOR IS NOT NULL THEN CL.IDGDA_SECTOR  ");
            ////stb.Append("            ELSE CL2.IDGDA_SECTOR  ");
            ////stb.Append("        END AS IDGDA_SECTOR,  ");
            ////stb.Append("		 CASE  ");
            ////stb.Append("            WHEN CL.HOME_BASED != '' THEN CL.HOME_BASED  ");
            ////stb.Append("            ELSE CL2.HOME_BASED  ");
            ////stb.Append("        END AS HOME_BASED,  ");
            ////stb.Append("		CASE  ");
            ////stb.Append("            WHEN CL.CARGO IS NOT NULL THEN CL.CARGO  ");
            ////stb.Append("            ELSE CL2.CARGO  ");
            ////stb.Append("        END AS CARGO,  ");
            ////stb.Append("		CASE  ");
            ////stb.Append("            WHEN CL.ACTIVE IS NOT NULL THEN CL.ACTIVE  ");
            ////stb.Append("            ELSE CL2.ACTIVE  ");
            ////stb.Append("        END AS ACTIVE,  ");
            ////stb.Append("		CASE  ");
            ////stb.Append("            WHEN CL.SITE != '' THEN CL.SITE  ");
            ////stb.Append("            ELSE CL2.SITE  ");
            ////stb.Append("        END AS SITE,  ");
            ////stb.Append("		CASE  ");
            ////stb.Append("            WHEN CL.PERIODO != '' THEN CL.PERIODO  ");
            ////stb.Append("            ELSE CL2.PERIODO  ");
            ////stb.Append("        END AS PERIODO,  ");
            ////stb.Append("		CASE  ");
            ////stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_SUPERVISOR  ");
            ////stb.Append("            ELSE CL2.MATRICULA_SUPERVISOR  ");
            ////stb.Append("        END AS MATRICULA_SUPERVISOR,  ");
            ////stb.Append("		CASE  ");
            ////stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_SUPERVISOR  ");
            ////stb.Append("            ELSE CL2.NOME_SUPERVISOR  ");
            ////stb.Append("        END AS NOME_SUPERVISOR,  ");
            ////stb.Append("				CASE  ");
            ////stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_COORDENADOR  ");
            ////stb.Append("            ELSE CL2.MATRICULA_COORDENADOR  ");
            ////stb.Append("        END AS MATRICULA_COORDENADOR,  ");
            ////stb.Append("				CASE  ");
            ////stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_COORDENADOR  ");
            ////stb.Append("            ELSE CL2.NOME_COORDENADOR  ");
            ////stb.Append("        END AS NOME_COORDENADOR,  ");
            ////stb.Append("				CASE  ");
            ////stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_GERENTE_II  ");
            ////stb.Append("            ELSE CL2.MATRICULA_GERENTE_II  ");
            ////stb.Append("        END AS MATRICULA_GERENTE_II,  ");
            ////stb.Append("				CASE  ");
            ////stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_GERENTE_II  ");
            ////stb.Append("            ELSE CL2.NOME_GERENTE_II  ");
            ////stb.Append("        END AS NOME_GERENTE_II,  ");
            ////stb.Append("				CASE  ");
            ////stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_GERENTE_I  ");
            ////stb.Append("            ELSE CL2.MATRICULA_GERENTE_I  ");
            ////stb.Append("        END AS MATRICULA_GERENTE_I,  ");
            ////stb.Append("				CASE  ");
            ////stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_GERENTE_I  ");
            ////stb.Append("            ELSE CL2.NOME_GERENTE_I  ");
            ////stb.Append("        END AS NOME_GERENTE_I,  ");
            ////stb.Append("				CASE  ");
            ////stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_DIRETOR  ");
            ////stb.Append("            ELSE CL2.MATRICULA_DIRETOR  ");
            ////stb.Append("        END AS MATRICULA_DIRETOR,  ");
            ////stb.Append("				CASE  ");
            ////stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_DIRETOR  ");
            ////stb.Append("            ELSE CL2.NOME_DIRETOR  ");
            ////stb.Append("        END AS NOME_DIRETOR,  ");
            ////stb.Append("				CASE  ");
            ////stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_CEO  ");
            ////stb.Append("            ELSE CL2.MATRICULA_CEO  ");
            ////stb.Append("        END AS MATRICULA_CEO,  ");
            ////stb.Append("				CASE  ");
            ////stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_CEO  ");
            ////stb.Append("            ELSE CL2.NOME_CEO  ");
            ////stb.Append("        END AS NOME_CEO,  ");
            ////stb.Append(" ");
            ////stb.Append("        C.IDGDA_COLLABORATORS, ");
            ////stb.Append("        CL.CREATED_AT ");
            ////stb.Append("    FROM GDA_COLLABORATORS (NOLOCK) AS C ");
            ////stb.Append("    LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CL  ");
            ////stb.Append("        ON CL.IDGDA_COLLABORATORS = C.IDGDA_COLLABORATORS AND CL.CREATED_AT >= @DATAINICIAL AND CL.CREATED_AT <= @DATAFINAL ");
            ////stb.Append("    LEFT JOIN GDA_COLLABORATORS_LAST_DETAILS (NOLOCK) AS CL2  ");
            ////stb.Append("        ON CL2.IDGDA_COLLABORATORS = C.IDGDA_COLLABORATORS ");
            ////stb.Append(") AS CL  ");
            ////stb.Append("ON CL.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS AND CL.CREATED_AT = R.CREATED_AT ");

            //stb.Append("LEFT JOIN GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME ON HME.INDICATORID = R.INDICADORID ");
            //stb.Append("AND HME.deleted_at IS NULL ");
            //stb.Append("LEFT JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON ME.ID = HME.MATHEMATICALEXPRESSIONID ");
            //stb.Append("AND ME.DELETED_AT IS NULL ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
            //stb.Append("AND HIG1.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIG1.SECTOR_ID = CL.IDGDA_SECTOR ");
            //stb.Append("AND HIG1.GROUPID = 1 ");
            //stb.Append("AND CONVERT(DATE,HIG1.STARTED_AT) <= R.CREATED_AT AND CONVERT(DATE,HIG1.ENDED_AT) >= R.CREATED_AT ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG2 ON HIG2.DELETED_AT IS NULL ");
            //stb.Append("AND HIG2.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIG2.SECTOR_ID = CL.IDGDA_SECTOR ");
            //stb.Append("AND HIG2.GROUPID = 2 ");
            //stb.Append("AND CONVERT(DATE,HIG2.STARTED_AT) <= R.CREATED_AT AND CONVERT(DATE,HIG2.ENDED_AT) >= R.CREATED_AT ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG3 ON HIG3.DELETED_AT IS NULL ");
            //stb.Append("AND HIG3.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIG3.SECTOR_ID = CL.IDGDA_SECTOR ");
            //stb.Append("AND HIG3.GROUPID = 3 ");
            //stb.Append("AND CONVERT(DATE,HIG3.STARTED_AT) <= R.CREATED_AT AND CONVERT(DATE,HIG3.ENDED_AT) >= R.CREATED_AT ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG4 ON HIG4.DELETED_AT IS NULL ");
            //stb.Append("AND HIG4.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIG4.SECTOR_ID = CL.IDGDA_SECTOR ");
            //stb.Append("AND HIG4.GROUPID = 4 ");
            //stb.Append("AND CONVERT(DATE,HIG4.STARTED_AT) <= R.CREATED_AT AND CONVERT(DATE,HIG4.ENDED_AT) >= R.CREATED_AT ");
            //stb.Append("LEFT JOIN GDA_CHECKING_ACCOUNT (NOLOCK) AS CA ON CA.COLLABORATOR_ID = R.IDGDA_COLLABORATORS ");
            //stb.Append("AND CA.RESULT_DATE = R.CREATED_AT ");
            //stb.Append("AND CA.GDA_INDICATOR_IDGDA_INDICATOR = R.INDICADORID ");
            //stb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS IT ON IT.IDGDA_INDICATOR = R.INDICADORID ");
            //stb.Append("LEFT JOIN ");
            //stb.Append("  (SELECT SUM(INPUT) AS INPUT, ");
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
            //stb.Append("            COLLABORATOR_ID) AS MZ ON  ");
            //stb.Append(" MZ.gda_indicator_idgda_indicator = R.INDICADORID ");
            //stb.Append("AND MZ.result_date = R.CREATED_AT ");
            //stb.Append("AND MZ.COLLABORATOR_ID = R.IDGDA_COLLABORATORS ");
            ////stb.Append("INNER JOIN GDA_BASKET_INDICATOR AS BK ON BK.SECTOR_ID = CL.IDGDA_SECTOR ");
            ////stb.Append("AND BK.INDICATOR_ID = R.INDICADORID ");
            ////stb.Append("AND BK.DATE = R.CREATED_AT ");
            //stb.Append("LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = CL.IDGDA_SECTOR ");
            //stb.Append("LEFT JOIN GDA_SECTOR (NOLOCK) AS SECSUP ON SECSUP.IDGDA_SECTOR = CL.IDGDA_SECTOR_SUPERVISOR ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) AS HIS ON HIS.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIS.SECTOR_ID = CL.IDGDA_SECTOR AND CONVERT(DATE,HIS.STARTED_AT) <= R.CREATED_AT AND CONVERT(DATE,HIS.ENDED_AT) >= R.CREATED_AT ");
            //stb.Append("AND HIS.DELETED_AT IS NULL ");

            ////FK
            //stb.Append("LEFT JOIN GDA_HISTORY_SCORE_INDICATOR_SECTOR (NOLOCK) AS SC ON ");
            //stb.Append("SC.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND SC.SECTOR_ID = CL.IDGDA_SECTOR ");
            //stb.Append("AND CONVERT(DATE,SC.STARTED_AT) <= R.CREATED_AT ");
            //stb.Append("AND CONVERT(DATE,SC.ENDED_AT) >= R.CREATED_AT ");

            //stb.Append("WHERE 1 = 1 ");
            //stb.Append("  AND R.CREATED_AT >= @DATAINICIAL ");
            //stb.Append("  AND R.CREATED_AT <= @DATAFINAL ");
            //stb.Append("  AND R.DELETED_AT IS NULL ");
            ////stb.Append("  AND CL.IDGDA_SECTOR IS NOT NULL ");
            ////stb.Append("  AND CL.CARGO IS NOT NULL ");
            ////stb.Append("  AND CL.HOME_BASED <> '' ");
            //stb.Append("  AND CL.active = 'true' ");
            //stb.Append("  AND HIG1.MONETIZATION > 0 ");
            //stb.Append("  AND R.FACTORS <> '0.000000;0.000000'");
            //stb.Append("  AND R.FACTORS <> '0.000000; 0.000000'");
            //stb.AppendFormat(" {0} ", filter);
            //stb.Append(" ");
            //stb.Append("GROUP BY R.IDGDA_COLLABORATORS, ");
            //stb.Append("         R.INDICADORID, ");
            //stb.Append("         CONVERT(DATE, R.CREATED_AT) ");
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
                            string factors = reader["FATOR"].ToString();
                            if (factors == "0.000000;0.000000" || factors == "0.000000; 0.000000")
                            {
                                continue;
                            }
                            string factor0 = Funcoes.RemoverZerosAposPonto(factors.Split(";")[0]);
                            string factor1 = Funcoes.RemoverZerosAposPonto(factors.Split(";")[1]);
                            ModelsEx.homeRel rmam = new ModelsEx.homeRel();
                            rmam.datePay = reader["DATA DO PAGAMENTO"].ToString();
                            rmam.dateReferer = reader["REFERENCIA PAGAMENTO"].ToString();
                            rmam.data = reader["REFERENCIA PAGAMENTO"].ToString();
                            rmam.idcollaborator = reader["MATRICULA"].ToString();
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
                            rmam.cod_gip_supervisor = reader["COD_GIP_SUPERVISOR"].ToString();
                            rmam.setor_supervisor = reader["setor_supervisor"].ToString();
                            rmam.cargo = reader["cargo"].ToString() == "" ? "Não Informado" : reader["cargo"].ToString();
                            rmam.cod_gip = reader["cod_gip"].ToString() == "" ? "Não Informado" : reader["cod_gip"].ToString();
                            rmam.setor = reader["setor"].ToString() == "" ? "Não Informado" : reader["setor"].ToString();


                            rmam.cod_gip_reference = reader["COD_GIP_REFERENCE"].ToString() == "" ? "Não Informado" : reader["COD_GIP_REFERENCE"].ToString();
                            rmam.setor_reference = reader["SETOR_REFERENCE"].ToString() == "" ? "Não Informado" : reader["SETOR_REFERENCE"].ToString();

                            string teste = reader["SETOR_REFERENCE"].ToString();

                            rmam.home_based = reader["home_based"].ToString() == "" ? "Não Informado" : reader["home_based"].ToString();
                            rmam.site = reader["site"].ToString();
                            rmam.turno = reader["turno"].ToString();
                            rmam.matricula_supervisor = reader["matricula supervisor"].ToString();
                            rmam.nome_supervisor = reader["nome supervisor"].ToString();
                            rmam.matricula_coordenador = reader["matricula coordenador"].ToString();
                            rmam.nome_coordenador = reader["nome coordenador"].ToString();
                            rmam.matricula_gerente_ii = reader["matricula gerente ii"].ToString();
                            rmam.nome_gerente_ii = reader["nome gerente ii"].ToString();
                            rmam.matricula_gerente_i = reader["matricula gerente i"].ToString();
                            rmam.nome_gerente_i = reader["nome gerente i"].ToString();
                            rmam.matricula_diretor = reader["matricula diretor"].ToString();
                            rmam.nome_diretor = reader["nome diretor"].ToString();
                            rmam.matricula_ceo = reader["matricula ceo"].ToString();
                            rmam.nome_ceo = reader["nome ceo"].ToString();
                            rmam.fator0 = double.Parse(factor0);
                            rmam.fator1 = double.Parse(factor1);
                            rmam.conta = reader["CONTA"].ToString();
                            rmam.better = reader["BETTER"].ToString();
                            rmam.resultadoAPI = reader["RESULTADOAPI"].ToString() != "" ? double.Parse(reader["RESULTADOAPI"].ToString()) != 100 ? rmam.resultadoAPI = double.Parse(reader["RESULTADOAPI"].ToString()) : 0 : 0;
                            rmam.diasTrabalhados = reader["TRABALHADO"].ToString() != "" ? reader["TRABALHADO"].ToString() : "-";
                            rmam.diasEscalados = reader["ESCALADO"].ToString() != "" ? reader["ESCALADO"].ToString() : "-";
                            rmam.moedasGanhas = reader["MOEDA_GANHA"].ToString() != "" ? double.Parse(reader["MOEDA_GANHA"].ToString()) : 0;
                            rmam.vemMeta = rmam.meta.ToString() == "" && rmam.meta.ToString() == null || rmam.min1.ToString() == "" ? 0 : 1;
                            rmam.peso = reader["SCORE"].ToString() != "" ? double.Parse(reader["SCORE"].ToString()) : 0;
                            rmams.Add(rmam);
                        }
                    }
                }
                connection.Close();
            }
            return rmams;
        }
        public class returnResponseDay
        {
            public string DataPagamento { get; set; }
            public string DataReferencia { get; set; }
            //public string Ano { get; set; }
            public string Matricula { get; set; }
            public string NomeColaborador { get; set; }
            public string Cargo { get; set; }
            //public string DiasTrabalhados { get; set; }
            public string IDIndicador { get; set; }
            public string Indicador { get; set; }
            public string TipoIndicador { get; set; }
            public string Meta { get; set; }
            public string Resultado { get; set; }
            public string Percentual { get; set; }
            public double GanhoEmMoedas { get; set; }
            public string MetaMaximaMoedas { get; set; }
            public string Grupo { get; set; }
            public DateTime DataAtualizacao { get; set; }
            public string MatriculaSupervisor { get; set; }
            public string NomeSupervisor { get; set; }
            public string MatriculaCoordenador { get; set; }
            public string NomeCoordenador { get; set; }
            public string MatriculaGerente2 { get; set; }
            public string NomeGerente2 { get; set; }
            public string MatriculaGerente1 { get; set; }
            public string NomeGerente1 { get; set; }
            public string MatriculaDiretor { get; set; }
            public string NomeDiretor { get; set; }
            public string MatriculaCEO { get; set; }
            public string NomeCEO { get; set; }
            public string CodigoGIP { get; set; }
            public string Setor { get; set; }
            public string CodigoGIPSubsetor { get; set; }
            public string Subsetor { get; set; }
            public string Home { get; set; }
            public string Turno { get; set; }
            public string Site { get; set; }
            public double Score { get; set; }
        }

        #region Input
        public class InputModel
        {
            public List<Sector> Sectors { get; set; }
            public List<Group> Groups { get; set; }
            public List<Indicator> Indicators { get; set; }
            public List<Hierarchy> Hierarchies { get; set; }
            public List<Indicator> Collaborators { get; set; }
            public string Type { get; set; }
            public string Order { get; set; }
            public DateTime DataInicial { get; set; }
            public DateTime DataFinal { get; set; }
            //public string CollaboratorId { get; set; }
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

        public static List<ModelsEx.homeRel> returnListHierarchy(List<ModelsEx.homeRel> original, string hierarchy)
        {
            List<ModelsEx.homeRel> retorno = new List<ModelsEx.homeRel>();
            try
            {
                if (hierarchy == "SUPERVISOR")
                {
                    retorno = original
                        .GroupBy(item => new { item.cod_indicador, item.dateReferer, item.matricula_supervisor }).Where(d => d.Key.matricula_supervisor != "0")
                        .Select(grupo => new ModelsEx.homeRel
                        {
                            datePay = grupo.First().datePay,
                            dateReferer = grupo.First().dateReferer,
                            data = grupo.First().data,
                            cod_indicador = grupo.First().cod_indicador,
                            indicador = grupo.First().indicador,
                            cod_gip = grupo.First().cod_gip_supervisor,
                            indicatorType = grupo.First().indicatorType,
                            meta = grupo.First().meta,
                            goal = grupo.First().goal,
                            data_atualizacao = grupo.First().data_atualizacao, // Você pode precisar ajustar o formato da data
                            idcollaborator = grupo.Key.matricula_supervisor,
                            name = grupo.Max(s => s.nome_supervisor),
                            cargo = "SUPERVISOR",
                            home_based = grupo.First().home_based,
                            setor = grupo.First().setor_supervisor,
                            setor_reference = grupo.First().setor_reference,
                            cod_gip_reference = grupo.First().cod_gip_reference,
                            turno = grupo.First().turno,
                            site = grupo.First().site,
                            nome_supervisor = "-",
                            matricula_supervisor = "-",
                            matricula_coordenador = grupo.First().matricula_coordenador,
                            nome_coordenador = grupo.First().nome_coordenador,
                            matricula_gerente_ii = grupo.First().matricula_gerente_ii,
                            nome_gerente_ii = grupo.First().nome_gerente_ii,
                            matricula_gerente_i = grupo.First().matricula_gerente_i,
                            nome_gerente_i = grupo.First().nome_gerente_i,
                            matricula_diretor = grupo.First().matricula_diretor,
                            nome_diretor = grupo.First().nome_diretor,
                            matricula_ceo = grupo.First().matricula_ceo,
                            nome_ceo = grupo.First().nome_ceo,
                            fator0 = grupo.Sum(item => item.fator0),
                            fator1 = grupo.Sum(item => item.fator1),
                            //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                            //min1 = grupo.Sum(item2 => item2.min1),
                            //min2 = grupo.Sum(item2 => item2.min2),
                            //min3 = grupo.Sum(item2 => item2.min3),
                            //min4 = grupo.Sum(item2 => item2.min4),
                            min1 = grupo.First().min1,
                            min2 = grupo.First().min2,
                            min3 = grupo.First().min3,
                            min4 = grupo.First().min4,
                            conta = grupo.First().conta,
                            better = grupo.First().better,
                            peso = grupo.Max(item => item.peso),
                            diasTrabalhados = grupo.Max(item => item.diasTrabalhados),
                            diasEscalados = grupo.Max(item => item.diasEscalados),
                            moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Count(), 0, MidpointRounding.AwayFromZero),
                            moedasGanhas = grupo.Sum(item => item.moedasGanhas),
                            qtdPessoas = grupo.Count(item2 => item2.resultado > 0),
                            metaSomada = grupo.Sum(item2 => item2.goal),
                            qtdPessoasTotal = grupo.Count(),
                            resultadoAPI = grupo.Sum(item => item.resultadoAPI),
                            vemMeta = grupo.Max(item => item.vemMeta),
                        }).ToList();
                }
                else if (hierarchy == "COORDENADOR")
                {
                    retorno = original
                    .GroupBy(item => new { item.cod_indicador, item.dateReferer, item.matricula_coordenador }).Where(d => d.Key.matricula_coordenador != "0")
                    .Select(grupo => new ModelsEx.homeRel
                    {
                        datePay = grupo.First().datePay,
                        dateReferer = grupo.First().dateReferer,
                        data = grupo.First().data,
                        cod_indicador = grupo.First().cod_indicador,
                        indicador = grupo.First().indicador,
                        cod_gip = grupo.First().cod_gip,
                        meta = grupo.First().meta,
                        goal = grupo.First().goal,
                        indicatorType = grupo.First().indicatorType,
                        data_atualizacao = grupo.First().data_atualizacao, // Você pode precisar ajustar o formato da data
                        idcollaborator = grupo.Key.matricula_coordenador,
                        name = grupo.First().nome_coordenador,
                        cargo = "COORDENADOR",
                        home_based = grupo.First().home_based,
                        setor = grupo.First().setor,
                        setor_reference = grupo.First().setor_reference,
                        cod_gip_reference = grupo.First().cod_gip_reference,
                        turno = grupo.First().turno,
                        site = grupo.First().site,
                        nome_supervisor = "-",
                        matricula_supervisor = "-",
                        matricula_coordenador = "-",
                        nome_coordenador = "-",
                        matricula_gerente_ii = grupo.First().matricula_gerente_ii,
                        nome_gerente_ii = grupo.First().nome_gerente_ii,
                        matricula_gerente_i = grupo.First().matricula_gerente_i,
                        nome_gerente_i = grupo.First().nome_gerente_i,
                        matricula_diretor = grupo.First().matricula_diretor,
                        nome_diretor = grupo.First().nome_diretor,
                        matricula_ceo = grupo.First().matricula_ceo,
                        nome_ceo = grupo.First().nome_ceo,
                        fator0 = grupo.Sum(item => item.fator0),
                        fator1 = grupo.Sum(item => item.fator1),
                        //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                        //min1 = grupo.Sum(item2 => item2.min1),
                        //min2 = grupo.Sum(item2 => item2.min2),
                        //min3 = grupo.Sum(item2 => item2.min3),
                        //min4 = grupo.Sum(item2 => item2.min4),
                        min1 = grupo.First().min1,
                        min2 = grupo.First().min2,
                        min3 = grupo.First().min3,
                        min4 = grupo.First().min4,
                        conta = grupo.First().conta,
                        better = grupo.First().better,
                        diasTrabalhados = grupo.Max(item => item.diasTrabalhados),
                        diasEscalados = grupo.Max(item => item.diasEscalados),
                        peso = grupo.Max(item => item.peso),
                        moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Count(), 0, MidpointRounding.AwayFromZero),
                        moedasGanhas = grupo.Sum(item => item.moedasGanhas),
                        qtdPessoas = grupo.Count(item2 => item2.resultado > 0),
                        metaSomada = grupo.Sum(item2 => item2.goal),
                        qtdPessoasTotal = grupo.Count(),
                        resultadoAPI = grupo.Sum(item => item.resultadoAPI),
                        vemMeta = grupo.Max(item => item.vemMeta),
                    })
                    .ToList();
                }
                else if (hierarchy == "GERENTE II")
                {
                    retorno = original
                    .GroupBy(item => new { item.cod_indicador, item.dateReferer, item.matricula_gerente_ii }).Where(d => d.Key.matricula_gerente_ii != "0")
                    .Select(grupo => new ModelsEx.homeRel
                    {
                        datePay = grupo.First().datePay,
                        dateReferer = grupo.First().dateReferer,
                        data = grupo.First().data,
                        cod_indicador = grupo.First().cod_indicador,
                        indicador = grupo.First().indicador,
                        cod_gip = grupo.First().cod_gip,
                        meta = grupo.First().meta,
                        goal = grupo.First().goal,
                        indicatorType = grupo.First().indicatorType,
                        data_atualizacao = grupo.First().data_atualizacao, // Você pode precisar ajustar o formato da data
                        idcollaborator = grupo.Key.matricula_gerente_ii,
                        name = grupo.First().nome_gerente_ii,
                        cargo = "GERENTE II",
                        home_based = grupo.First().home_based,
                        setor = grupo.First().setor,
                        setor_reference = grupo.First().setor_reference,
                        cod_gip_reference = grupo.First().cod_gip_reference,
                        turno = grupo.First().turno,
                        site = grupo.First().site,
                        nome_supervisor = "-",
                        matricula_supervisor = "-",
                        matricula_coordenador = "-",
                        nome_coordenador = "-",
                        matricula_gerente_ii = "-",
                        nome_gerente_ii = "-",
                        matricula_gerente_i = grupo.First().matricula_gerente_i,
                        nome_gerente_i = grupo.First().nome_gerente_i,
                        matricula_diretor = grupo.First().matricula_diretor,
                        nome_diretor = grupo.First().nome_diretor,
                        matricula_ceo = grupo.First().matricula_ceo,
                        nome_ceo = grupo.First().nome_ceo,
                        fator0 = grupo.Sum(item => item.fator0),
                        fator1 = grupo.Sum(item => item.fator1),
                        //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                        //min1 = grupo.Sum(item2 => item2.min1),
                        //min2 = grupo.Sum(item2 => item2.min2),
                        //min3 = grupo.Sum(item2 => item2.min3),
                        //min4 = grupo.Sum(item2 => item2.min4),
                        min1 = grupo.First().min1,
                        min2 = grupo.First().min2,
                        min3 = grupo.First().min3,
                        min4 = grupo.First().min4,
                        conta = grupo.First().conta,
                        better = grupo.First().better,
                        peso = grupo.Max(item => item.peso),
                        diasTrabalhados = grupo.Max(item => item.diasTrabalhados),
                        diasEscalados = grupo.Max(item => item.diasEscalados),
                        moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Count(), 0, MidpointRounding.AwayFromZero),
                        moedasGanhas = grupo.Sum(item => item.moedasGanhas),
                        qtdPessoas = grupo.Count(item2 => item2.resultado > 0),
                        metaSomada = grupo.Sum(item2 => item2.goal),
                        qtdPessoasTotal = grupo.Count(),
                        resultadoAPI = grupo.Sum(item => item.resultadoAPI),
                        vemMeta = grupo.Max(item => item.vemMeta),
                    })
                    .ToList();
                }
                else if (hierarchy == "GERENTE I")
                {
                    retorno = original
                    .GroupBy(item => new { item.cod_indicador, item.dateReferer, item.matricula_gerente_i }).Where(d => d.Key.matricula_gerente_i != "0")
                    .Select(grupo => new ModelsEx.homeRel
                    {
                        datePay = grupo.First().datePay,
                        dateReferer = grupo.First().dateReferer,
                        data = grupo.First().data,
                        cod_indicador = grupo.First().cod_indicador,
                        indicador = grupo.First().indicador,
                        cod_gip = grupo.First().cod_gip,
                        meta = grupo.First().meta,
                        goal = grupo.First().goal,
                        indicatorType = grupo.First().indicatorType,
                        data_atualizacao = grupo.First().data_atualizacao, // Você pode precisar ajustar o formato da data
                        idcollaborator = grupo.Key.matricula_gerente_i,
                        name = grupo.First().nome_gerente_i,
                        cargo = "GERENTE I",
                        home_based = grupo.First().home_based,
                        setor = grupo.First().setor,
                        setor_reference = grupo.First().setor_reference,
                        cod_gip_reference = grupo.First().cod_gip_reference,
                        turno = grupo.First().turno,
                        site = grupo.First().site,
                        nome_supervisor = "-",
                        matricula_supervisor = "-",
                        matricula_coordenador = "-",
                        nome_coordenador = "-",
                        matricula_gerente_ii = "-",
                        nome_gerente_ii = "-",
                        matricula_gerente_i = "-",
                        nome_gerente_i = "-",
                        matricula_diretor = grupo.First().matricula_diretor,
                        nome_diretor = grupo.First().nome_diretor,
                        matricula_ceo = grupo.First().matricula_ceo,
                        nome_ceo = grupo.First().nome_ceo,
                        fator0 = grupo.Sum(item => item.fator0),
                        fator1 = grupo.Sum(item => item.fator1),
                        //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                        //min1 = grupo.Sum(item2 => item2.min1),
                        //min2 = grupo.Sum(item2 => item2.min2),
                        //min3 = grupo.Sum(item2 => item2.min3),
                        //min4 = grupo.Sum(item2 => item2.min4),
                        min1 = grupo.First().min1,
                        min2 = grupo.First().min2,
                        min3 = grupo.First().min3,
                        min4 = grupo.First().min4,
                        conta = grupo.First().conta,
                        better = grupo.First().better,
                        peso = grupo.Max(item => item.peso),
                        diasTrabalhados = grupo.Max(item => item.diasTrabalhados),
                        diasEscalados = grupo.Max(item => item.diasEscalados),
                        moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Count(), 0, MidpointRounding.AwayFromZero),
                        moedasGanhas = grupo.Sum(item => item.moedasGanhas),
                        qtdPessoas = grupo.Count(item2 => item2.resultado > 0),
                        metaSomada = grupo.Sum(item2 => item2.goal),
                        qtdPessoasTotal = grupo.Count(),
                        resultadoAPI = grupo.Sum(item => item.resultadoAPI),
                        vemMeta = grupo.Max(item => item.vemMeta),
                    })
                    .ToList();
                }
                else if (hierarchy == "DIRETOR")
                {
                    retorno = original
                    .GroupBy(item => new { item.cod_indicador, item.dateReferer, item.matricula_diretor }).Where(d => d.Key.matricula_diretor != "0")
                    .Select(grupo => new ModelsEx.homeRel
                    {
                        datePay = grupo.First().datePay,
                        dateReferer = grupo.First().dateReferer,
                        data = grupo.First().data,
                        cod_indicador = grupo.First().cod_indicador,
                        indicador = grupo.First().indicador,
                        cod_gip = grupo.First().cod_gip,
                        meta = grupo.First().meta,
                        goal = grupo.First().goal,
                        indicatorType = grupo.First().indicatorType,
                        data_atualizacao = grupo.First().data_atualizacao, // Você pode precisar ajustar o formato da data
                        idcollaborator = grupo.Key.matricula_diretor,
                        name = grupo.First().nome_diretor,
                        cargo = "DIRETOR",
                        home_based = grupo.First().home_based,
                        setor = grupo.First().setor,
                        setor_reference = grupo.First().setor_reference,
                        cod_gip_reference = grupo.First().cod_gip_reference,
                        turno = grupo.First().turno,
                        site = grupo.First().site,
                        nome_supervisor = "-",
                        matricula_supervisor = "-",
                        matricula_coordenador = "-",
                        nome_coordenador = "-",
                        matricula_gerente_ii = "-",
                        nome_gerente_ii = "-",
                        matricula_gerente_i = "-",
                        nome_gerente_i = "-",
                        matricula_diretor = "-",
                        nome_diretor = "-",
                        matricula_ceo = grupo.First().matricula_ceo,
                        nome_ceo = grupo.First().nome_ceo,
                        fator0 = grupo.Sum(item => item.fator0),
                        fator1 = grupo.Sum(item => item.fator1),
                        //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                        //min1 = grupo.Sum(item2 => item2.min1),
                        //min2 = grupo.Sum(item2 => item2.min2),
                        //min3 = grupo.Sum(item2 => item2.min3),
                        //min4 = grupo.Sum(item2 => item2.min4),
                        min1 = grupo.First().min1,
                        min2 = grupo.First().min2,
                        min3 = grupo.First().min3,
                        min4 = grupo.First().min4,
                        conta = grupo.First().conta,
                        better = grupo.First().better,
                        peso = grupo.Max(item => item.peso),
                        diasTrabalhados = grupo.Max(item => item.diasTrabalhados),
                        diasEscalados = grupo.Max(item => item.diasEscalados),
                        moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Count(), 0, MidpointRounding.AwayFromZero),
                        moedasGanhas = grupo.Sum(item => item.moedasGanhas),
                        qtdPessoas = grupo.Count(item2 => item2.resultado > 0),
                        metaSomada = grupo.Sum(item2 => item2.goal),
                        qtdPessoasTotal = grupo.Count(),
                        resultadoAPI = grupo.Sum(item => item.resultadoAPI),
                        vemMeta = grupo.Max(item => item.vemMeta),
                    })
                    .ToList();
                }
                else if (hierarchy == "CEO")
                {
                    retorno = original
                    .GroupBy(item => new { item.cod_indicador, item.dateReferer, item.matricula_ceo }).Where(d => d.Key.matricula_ceo != "0")
                    .Select(grupo => new ModelsEx.homeRel
                    {
                        datePay = grupo.First().datePay,
                        dateReferer = grupo.First().dateReferer,
                        data = grupo.First().data,
                        cod_indicador = grupo.First().cod_indicador,
                        indicador = grupo.First().indicador,
                        cod_gip = grupo.First().cod_gip,
                        meta = grupo.First().meta,
                        goal = grupo.First().goal,
                        indicatorType = grupo.First().indicatorType,
                        data_atualizacao = grupo.First().data_atualizacao, // Você pode precisar ajustar o formato da data
                        idcollaborator = grupo.Key.matricula_ceo,
                        name = grupo.First().nome_ceo,
                        cargo = "CEO",
                        home_based = grupo.First().home_based,
                        setor = grupo.First().setor,
                        setor_reference = grupo.First().setor_reference,
                        cod_gip_reference = grupo.First().cod_gip_reference,
                        turno = grupo.First().turno,
                        site = grupo.First().site,
                        nome_supervisor = "-",
                        matricula_supervisor = "-",
                        matricula_coordenador = "-",
                        nome_coordenador = "-",
                        matricula_gerente_ii = "-",
                        nome_gerente_ii = "-",
                        matricula_gerente_i = "-",
                        nome_gerente_i = "-",
                        matricula_diretor = "-",
                        nome_diretor = "-",
                        matricula_ceo = "-",
                        nome_ceo = "-",
                        fator0 = grupo.Sum(item => item.fator0),
                        fator1 = grupo.Sum(item => item.fator1),
                        //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                        //min1 = grupo.Sum(item2 => item2.min1),
                        //min2 = grupo.Sum(item2 => item2.min2),
                        //min3 = grupo.Sum(item2 => item2.min3),
                        //min4 = grupo.Sum(item2 => item2.min4),
                        min1 = grupo.First().min1,
                        min2 = grupo.First().min2,
                        min3 = grupo.First().min3,
                        min4 = grupo.First().min4,
                        conta = grupo.First().conta,
                        better = grupo.First().better,
                        peso = grupo.Max(item => item.peso),
                        diasTrabalhados = grupo.Max(item => item.diasTrabalhados),
                        diasEscalados = grupo.Max(item => item.diasEscalados),
                        moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Count(), 0, MidpointRounding.AwayFromZero),
                        moedasGanhas = grupo.Sum(item => item.moedasGanhas),
                        qtdPessoas = grupo.Count(item2 => item2.resultado > 0),
                        metaSomada = grupo.Sum(item2 => item2.goal),
                        qtdPessoasTotal = grupo.Count(),
                        resultadoAPI = grupo.Sum(item => item.resultadoAPI),
                        vemMeta = grupo.Max(item => item.vemMeta),
                    })
                    .ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
            return retorno;
        }
        // POST: api/Results

        public static List<returnResponseDay> relMonDiario(string dtInicial, string dtFinal, string groupsAsString, string sectorsAsString, string hierarchiesAsString, string indicatorsAsString, string indicatorsCestaAsString, string colaboratorsAsString, string orderFil, bool Thread = false)
        {


            Funcoes.cestaMetrica cm = Funcoes.getInfMetricBasket(Thread);

            //PEGA INFORMAÇÃO MONETIZAÇÃO HIERARQUIA
            List<ModelsEx.monetizacaoHierarquia> lMone = new List<ModelsEx.monetizacaoHierarquia>();
            lMone = Funcoes.retornaListaMonetizacaoHierarquia(dtInicial, dtFinal, Thread);

            //REALIZA A QUERY QUE RETORNA TODAS AS INFORMAÇÕES DOS COLABORADORES QUE TIVERAM MONEITZAÇÃO.
            List<ModelsEx.homeRel> rmams = new List<ModelsEx.homeRel>();
            rmams = returnMonetizationDayMonth(dtInicial, dtFinal, sectorsAsString, indicatorsAsString, colaboratorsAsString, hierarchiesAsString, orderFil, Thread);

            //RETIRANDO OS RESULTADOS DO SUPERVISOR.. ENTENDER COM A TAHTO COMO FICARA ESTA PARTE.
            rmams = rmams.FindAll(item => item.cargo == "AGENTE" || item.cargo == "Não Informado").ToList();

            //ORDENAÇÃO POR INDICADOR E MOEDAS GANHAS.
            if (orderFil != "")
            {
                if (orderFil.ToUpper() == "MELHOR")
                {
                    rmams = rmams.OrderBy(r => r.cod_indicador).OrderByDescending(r => r.moedasGanhas).ToList();
                }
                else
                {
                    rmams = rmams.OrderBy(r => r.cod_indicador).OrderBy(r => r.moedasGanhas).ToList();
                }
            }
            List<ModelsEx.homeRel> supervisores = new List<ModelsEx.homeRel>();
            List<ModelsEx.homeRel> coordenador = new List<ModelsEx.homeRel>();
            List<ModelsEx.homeRel> gerenteii = new List<ModelsEx.homeRel>();
            List<ModelsEx.homeRel> gerentei = new List<ModelsEx.homeRel>();
            List<ModelsEx.homeRel> diretor = new List<ModelsEx.homeRel>();
            List<ModelsEx.homeRel> ceo = new List<ModelsEx.homeRel>();
            List<ModelsEx.homeRel> hierarchies = new List<ModelsEx.homeRel>();

            //POPULA LISTA DE ACORDO COM SUA HIERARQUIA
            if (hierarchiesAsString.ToString() != "")
            {
                if (hierarchiesAsString.ToString().Contains("2"))
                {
                    supervisores = returnListHierarchy(rmams, "SUPERVISOR");
                }
                if (hierarchiesAsString.ToString().Contains("3"))
                {
                    coordenador = returnListHierarchy(rmams, "COORDENADOR");
                }
                if (hierarchiesAsString.ToString().Contains("4"))
                {
                    gerenteii = returnListHierarchy(rmams, "GERENTE II");
                }
                if (hierarchiesAsString.ToString().Contains("5"))
                {
                    gerentei = returnListHierarchy(rmams, "GERENTE I");
                }
                if (hierarchiesAsString.ToString().Contains("6"))
                {
                    diretor = returnListHierarchy(rmams, "DIRETOR");
                }
                if (hierarchiesAsString.ToString().Contains("7"))
                {
                    ceo = returnListHierarchy(rmams, "CEO");
                }
                //REMOVER OS AGENTES EM CASO DE NÃO FILTRAR AGENTES
                if (hierarchiesAsString.ToString().Contains("1") == false)
                {
                    rmams = new List<ModelsEx.homeRel>();
                }
            }
            else
            {
                supervisores = returnListHierarchy(rmams, "SUPERVISOR");
                //List<ModelsEx.homeRel> ASAS = new List<ModelsEx.homeRel>();
                //ASAS = supervisores.FindAll(item => item.idcollaborator == "766162 " && item.cod_indicador == "2").ToList();
                coordenador = returnListHierarchy(rmams, "COORDENADOR");
                gerenteii = returnListHierarchy(rmams, "GERENTE II");
                gerentei = returnListHierarchy(rmams, "GERENTE I");
                diretor = returnListHierarchy(rmams, "DIRETOR");
                ceo = returnListHierarchy(rmams, "CEO");
            }

            if (colaboratorsAsString != "")
            {
                List<string> colaboratorIds = colaboratorsAsString.Split(',').ToList();
                supervisores = supervisores.FindAll(t => colaboratorIds.Any(id => t.idcollaborator.ToString().Contains(id))).ToList();
                coordenador = coordenador.FindAll(t => colaboratorIds.Any(id => t.idcollaborator.ToString().Contains(id))).ToList();
                gerenteii = gerenteii.FindAll(t => colaboratorIds.Any(id => t.idcollaborator.ToString().Contains(id))).ToList();
                gerentei = gerentei.FindAll(t => colaboratorIds.Any(id => t.idcollaborator.ToString().Contains(id))).ToList();
                diretor = diretor.FindAll(t => colaboratorIds.Any(id => t.idcollaborator.ToString().Contains(id))).ToList();
                ceo = ceo.FindAll(t => colaboratorIds.Any(id => t.idcollaborator.ToString().Contains(id))).ToList();
            }

            rmams = rmams.Concat(supervisores).Concat(coordenador).Concat(gerenteii).Concat(gerentei).Concat(diretor).Concat(ceo).ToList();

            //FILTRO DE HIERARQUIA AGENTE
            List<ModelsEx.homeRel> listaAgentes = rmams
                  .Where(item => item.cargo.ToUpper() == "AGENTE")
                  .ToList();

            //FILTRO DE HIERARQUIA ACIMA DE AGENTE
            List<ModelsEx.homeRel> listaHieraquias = rmams
                   .Where(item => item.cargo.ToUpper() != "AGENTE")
                   .ToList();

            //<ModelsEx.homeRel> testeNovo = new List<ModelsEx.homeRel>();
            //testeNovo = rmams.FindAll(item => item.idcollaborator == "671714" && item.cod_indicador == "665").ToList();

            //CALCULO DE RESULTADO DOS AGENTES
            for (int i = 0; i < listaAgentes.Count; i++)
            {
                ModelsEx.homeRel agente = listaAgentes[i];
                if (agente.idcollaborator == "671714" && agente.cod_indicador == "665" && listaHieraquias[i].dateReferer == "2023-11-10")
                {
                    var parou = true;
                }
                agente = monetizationClass.doCalculationResultHome(agente, false);
                listaAgentes[i] = agente;
            }

            //CALCULO DE RESULTADO DAS HIERARQUIA ACIMA DE AGENTE
            for (int i = 0; i < listaHieraquias.Count; i++)
            {
                ModelsEx.homeRel hierarquia = listaHieraquias[i];
                if (hierarquia.idcollaborator == "756399")
                {
                    var parou = true;
                }
                factors factor = new factors();
                factors factor2 = new factors();
                hierarquia = monetizationClass.doCalculationResultHome(hierarquia, false);
                int idAg = Convert.ToInt32(hierarquia.idcollaborator);
                int indicadorAg = int.Parse(hierarquia.cod_indicador);
                DateTime dateAg = DateTime.Parse(hierarquia.dateReferer);
                var monEnc2 = lMone.Find(item => item.id == idAg && item.date == dateAg && item.idIndicador == indicadorAg);
                double monAg = 0;
                if (monEnc2 != null)
                {
                    if (monEnc2.Monetizacao.ToString() != "")
                    {
                        monAg = double.Parse(monEnc2.Monetizacao.ToString());
                    }
                }
                hierarquia.moedasGanhas = monAg;
                listaHieraquias[i] = hierarquia;
            }

            rmams = listaAgentes.Concat(listaHieraquias).ToList();

            //FILTRO DA CESTA DE INDICADORES.
            List<ModelsEx.homeRel> listaCesta = new List<ModelsEx.homeRel>();
            if (indicatorsCestaAsString != "" || indicatorsAsString == "")
            {
                listaCesta = Funcoes.retornaCestaIndicadores(rmams, cm, true, false, false);
            }
            if (indicatorsAsString != "")
            {
                List<string> valoresLista = indicatorsAsString.Split(',').Select(valor => valor.Trim()).ToList();
                rmams = rmams.FindAll(item => valoresLista.Contains(item.cod_indicador)).ToList();
            }

            if (indicatorsCestaAsString != "" && indicatorsAsString == "")
            {
                rmams.Clear();
                rmams = rmams.Concat(listaCesta).ToList();
            }
            else if (indicatorsCestaAsString != "")
            {
                rmams = rmams.Concat(listaCesta).ToList();
            }
            else if (indicatorsAsString == "")
            {
                rmams = rmams.Concat(listaCesta).ToList();
            }

            //List<ModelsEx.homeRel> listaCesta = new List<ModelsEx.homeRel>();
            //listaCesta = Funcoes.retornaCestaIndicadores(rmams, cm, true, false, false);
            //rmams = rmams.Concat(listaCesta).ToList();

            List<ModelsEx.homeRel> elementosFiltrados = rmams;

            //FILTRO DE GRUPO, APÓS OS CALCULOS
            if (groupsAsString != "")
            {
                //PEGA INFS GRUPO
                List<groups> lGroups = returnTables.listGroups("");
                string groupsAsString3 = string.Join(",", lGroups
                .Where(g => groupsAsString.Contains(g.id.ToString()))
                .Select(g => g.alias));

                //FILTRA SÓ OS GRUPOS ESPECIFICOS
                elementosFiltrados = rmams
                    .Where(item => groupsAsString3.Contains(item.grupo.ToUpper()))
                    .ToList();
            }

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
                elementosFiltrados = elementosFiltrados
                    .OrderBy(item =>
                    {
                        string grupo = item.grupo.ToUpper();
                        // Ordene por grupo
                        var order = grupoOrderList.FirstOrDefault(x => x.grupo == grupo);
                        return order.order;
                    })
                    .ToList();
            }
            var jsonData = elementosFiltrados.Select(item => new returnResponseDay
            {
                DataPagamento = item.datePay,
                DataReferencia = item.dateReferer,
                Matricula = item.idcollaborator,
                NomeColaborador = item.name,
                Cargo = item.cargo,
                IDIndicador = item.cod_indicador,
                Indicador = item.indicador,
                TipoIndicador = item.indicatorType,
                Meta = item.meta.ToString().Replace(".", ","),
                Resultado = item.resultado.ToString().Replace(".", ","),
                Percentual = (item.vemMeta == 1) || item.cod_indicador == "10000012" ? item.porcentual.ToString() : "-",
                MetaMaximaMoedas = item.moedasPossiveis.ToString(),
                GanhoEmMoedas = item.moedasGanhas,
                Grupo = (item.vemMeta == 1) || item.cod_indicador == "10000012" ? item.grupo.ToString() : "-",
                DataAtualizacao = DateTime.Parse(item.data_atualizacao), // Você pode precisar ajustar o formato da data
                MatriculaSupervisor = item.cargo == "Não Informado" ? "Não Informado" : item.matricula_supervisor,
                NomeSupervisor = item.cargo == "Não Informado" ? "Não Informado" : item.nome_supervisor,
                MatriculaCoordenador = item.cargo == "Não Informado" ? "Não Informado" : item.matricula_coordenador,
                NomeCoordenador = item.cargo == "Não Informado" ? "Não Informado" : item.nome_coordenador,
                MatriculaGerente2 = item.cargo == "Não Informado" ? "Não Informado" : item.matricula_gerente_ii,
                NomeGerente2 = item.cargo == "Não Informado" ? "Não Informado" : item.nome_gerente_ii,
                MatriculaGerente1 = item.cargo == "Não Informado" ? "Não Informado" : item.matricula_gerente_i,
                NomeGerente1 = item.cargo == "Não Informado" ? "Não Informado" : item.nome_gerente_i,
                MatriculaDiretor = item.cargo == "Não Informado" ? "Não Informado" : item.matricula_diretor,
                NomeDiretor = item.cargo == "Não Informado" ? "Não Informado" : item.nome_diretor,
                MatriculaCEO = item.cargo == "Não Informado" ? "Não Informado" : item.matricula_ceo,
                NomeCEO = item.cargo == "Não Informado" ? "Não Informado" : item.nome_ceo,
                //CodigoGIP = (item.cargo == "AGENTE" || item.cargo == "SUPERVISOR") ? item.cod_gip : "-",
                //Setor = (item.cargo == "AGENTE" || item.cargo == "SUPERVISOR") ? item.setor : "-",
                CodigoGIP = (item.cargo == "AGENTE" || item.cargo == "SUPERVISOR") ? item.cod_gip_reference : "-",
                Setor = (item.cargo == "AGENTE" || item.cargo == "SUPERVISOR") ? item.setor_reference : "-",
                CodigoGIPSubsetor = (item.cargo != "AGENTE" && item.cargo != "SUPERVISOR") ? "-" : item.cod_gip == item.cod_gip_reference ? "-" : item.cod_gip,
                Subsetor = (item.cargo != "AGENTE" && item.cargo != "SUPERVISOR") ? "-" : item.cod_gip == item.cod_gip_reference ? "-" : item.setor,
                Home = item.home_based,
                Turno = item.turno,
                Site = item.site,
                Score = item.score
            }).ToList();

            return jsonData;
        }

        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {

            string dtInicial = inputModel.DataInicial.ToString("yyyy-MM-dd");
            string dtFinal = inputModel.DataFinal.ToString("yyyy-MM-dd");
            string groupsAsString = string.Join(",", inputModel.Groups.Select(g => g.Id));
            string sectorsAsString = string.Join(",", inputModel.Sectors.Select(g => g.Id));
            string hierarchiesAsString = string.Join(",", inputModel.Hierarchies.Select(g => g.Id));
            string indicatorsAsString = string.Join(",", inputModel.Indicators.Where(i => i.Id.ToString() != "10000012").Select(g => g.Id));
            string indicatorsCestaAsString = string.Join(",", inputModel.Indicators.Where(i => i.Id.ToString() == "10000012").Select(g => g.Id));
            string colaboratorsAsString = string.Join(",", inputModel.Collaborators.Select(g => g.Id));
            string order = inputModel.Order.ToString();
            DateTime dtTimeInicial = DateTime.ParseExact(dtInicial, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dtTimeFinal = DateTime.ParseExact(dtFinal, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            TimeSpan diff = dtTimeFinal.Subtract(dtTimeInicial);
            int diferencaEmDias = (int)diff.TotalDays;
            if (diferencaEmDias > 31)
            {
                return BadRequest("Selecionar uma data de no maximo 1 mês!");
            }

            var jsonData = relMonDiario(dtInicial, dtFinal, groupsAsString, sectorsAsString, hierarchiesAsString, indicatorsAsString, indicatorsCestaAsString, colaboratorsAsString, order);

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(jsonData);
        }

        // Método para serializar um DataTable em JSON

    }
}