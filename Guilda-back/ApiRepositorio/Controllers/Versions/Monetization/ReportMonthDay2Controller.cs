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
    public class ReportMonthDay2Controller : ApiController
    {

        public List<ModelsEx.homeRel> returnMonetizationDayMonth(string dtInicial, string dtFinal, string sectors, string indicators, string collaborators, string hierarchies, string ordem)
        {
            // Preparar Filtros
            string filter = "";
            string orderBy = "";
            if (sectors != "")
            {
                filter = filter + $" AND CL.IDGDA_SECTOR IN ({sectors}) ";
            }

            //Comentado por conta do filtro de cesta
            if (indicators != "")
            {
                filter = filter + $" AND R.INDICADORID IN ({indicators}) ";
            }

            //if (hierarchies != "")
            //{
            //    filter = filter + $" AND CL.IDGDA_HIERARCHY IN ({hierarchies}) ";
            //}

            if (collaborators != "")
            {
                //filter = filter + $" AND R.IDGDA_COLLABORATORS IN ({collaborators}) ";

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

            StringBuilder stb = new StringBuilder();

            stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtInicial);
            stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFinal);
            stb.Append(" ");
            stb.Append("SELECT MAX(CONVERT(DATE, CA.CREATED_AT)) AS 'DATA DO PAGAMENTO', ");
            stb.Append("       R.IDGDA_COLLABORATORS AS 'MATRICULA', ");
            stb.Append("       MAX(CB.NAME) AS NAME, ");
            stb.Append("       MAX(CL.CARGO) AS CARGO, ");
            stb.Append("       MAX(IT.TYPE) AS TIPO, ");
            stb.Append("       MAX(CONVERT(DATE, R.CREATED_AT)) AS 'REFERENCIA PAGAMENTO', ");
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
            stb.Append("       MAX(IT.TYPE) AS TYPE, ");
            stb.Append("       '' AS RESULTADO, ");
            stb.Append("       '' AS PORCENTUAL, ");
            stb.Append("       MAX(IT.TYPE) AS TYPE, ");
            stb.Append("       MAX(R.RESULT) AS RESULTADOAPI, ");
            stb.Append("       MAX(TRAB.TRABALHADO) AS TRABALHADO, ");
            stb.Append("       MAX(ESC.ESCALADO) AS ESCALADO, ");
            stb.Append("       MAX(HIG1.MONETIZATION) AS META_MAXIMA, ");
            stb.Append("       CASE ");
            stb.Append("           WHEN MAX(HIG1.MONETIZATION) IS NULL THEN 0 ");
            stb.Append("           WHEN MAX(MZ.INPUT) IS NULL THEN 0 ");
            stb.Append("           ELSE MAX(MZ.INPUT) ");
            stb.Append("       END AS MOEDA_GANHA, ");
            stb.Append("       '' AS GRUPO, ");
            stb.Append("       GETDATE() AS 'Data de atualização', ");
            stb.Append("       MAX(CL.IDGDA_SECTOR) AS COD_GIP, ");
            stb.Append("       MAX(CL.IDGDA_SECTOR_SUPERVISOR) AS COD_GIP_SUPERVISOR, ");
            stb.Append("       MAX(SEC.NAME) AS SETOR, ");
            stb.Append("       MAX(SECSUP.NAME) AS SETOR_SUPERVISOR, ");
            stb.Append("       MAX(CL.HOME_BASED) AS HOME_BASED, ");
            stb.Append("       MAX(CL.SITE) AS SITE, ");
            stb.Append("       MAX(CL.PERIODO) AS TURNO, ");
            stb.Append("       MAX(CL.MATRICULA_SUPERVISOR) AS 'MATRICULA SUPERVISOR', ");
            stb.Append("       MAX(CL.NOME_SUPERVISOR) AS 'NOME SUPERVISOR', ");
            stb.Append("       MAX(CL.MATRICULA_COORDENADOR) AS 'MATRICULA COORDENADOR', ");
            stb.Append("       MAX(CL.NOME_COORDENADOR) AS 'NOME COORDENADOR', ");
            stb.Append("       MAX(CL.MATRICULA_GERENTE_II) AS 'MATRICULA GERENTE II', ");
            stb.Append("       MAX(CL.NOME_GERENTE_II) AS 'NOME GERENTE II', ");
            stb.Append("       MAX(CL.MATRICULA_GERENTE_I) AS 'MATRICULA GERENTE I', ");
            stb.Append("       MAX(CL.NOME_GERENTE_I) AS 'NOME GERENTE I', ");
            stb.Append("       MAX(CL.MATRICULA_DIRETOR) AS 'MATRICULA DIRETOR', ");
            stb.Append("       MAX(CL.NOME_DIRETOR) AS 'NOME DIRETOR', ");
            stb.Append("       MAX(CL.MATRICULA_CEO) AS 'MATRICULA CEO', ");
            stb.Append("       MAX(CL.NOME_CEO) AS 'NOME CEO' ");
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



            stb.Append("LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CL ON CL.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS ");
            stb.Append("AND CL.CREATED_AT = R.CREATED_AT ");
            //stb.Append("LEFT JOIN ( ");
            //stb.Append("    SELECT  ");
            //stb.Append("        CASE  ");
            //stb.Append("            WHEN CL.IDGDA_SECTOR IS NOT NULL THEN CL.IDGDA_SECTOR  ");
            //stb.Append("            ELSE CL2.IDGDA_SECTOR  ");
            //stb.Append("        END AS IDGDA_SECTOR,  ");
            //stb.Append("		 CASE  ");
            //stb.Append("            WHEN CL.HOME_BASED != '' THEN CL.HOME_BASED  ");
            //stb.Append("            ELSE CL2.HOME_BASED  ");
            //stb.Append("        END AS HOME_BASED,  ");
            //stb.Append("		CASE  ");
            //stb.Append("            WHEN CL.CARGO IS NOT NULL THEN CL.CARGO  ");
            //stb.Append("            ELSE CL2.CARGO  ");
            //stb.Append("        END AS CARGO,  ");
            //stb.Append("		CASE  ");
            //stb.Append("            WHEN CL.ACTIVE IS NOT NULL THEN CL.ACTIVE  ");
            //stb.Append("            ELSE CL2.ACTIVE  ");
            //stb.Append("        END AS ACTIVE,  ");
            //stb.Append("		CASE  ");
            //stb.Append("            WHEN CL.SITE != '' THEN CL.SITE  ");
            //stb.Append("            ELSE CL2.SITE  ");
            //stb.Append("        END AS SITE,  ");
            //stb.Append("		CASE  ");
            //stb.Append("            WHEN CL.PERIODO != '' THEN CL.PERIODO  ");
            //stb.Append("            ELSE CL2.PERIODO  ");
            //stb.Append("        END AS PERIODO,  ");
            //stb.Append("		CASE  ");
            //stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_SUPERVISOR  ");
            //stb.Append("            ELSE CL2.MATRICULA_SUPERVISOR  ");
            //stb.Append("        END AS MATRICULA_SUPERVISOR,  ");
            //stb.Append("		CASE  ");
            //stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_SUPERVISOR  ");
            //stb.Append("            ELSE CL2.NOME_SUPERVISOR  ");
            //stb.Append("        END AS NOME_SUPERVISOR,  ");
            //stb.Append("				CASE  ");
            //stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_COORDENADOR  ");
            //stb.Append("            ELSE CL2.MATRICULA_COORDENADOR  ");
            //stb.Append("        END AS MATRICULA_COORDENADOR,  ");
            //stb.Append("				CASE  ");
            //stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_COORDENADOR  ");
            //stb.Append("            ELSE CL2.NOME_COORDENADOR  ");
            //stb.Append("        END AS NOME_COORDENADOR,  ");
            //stb.Append("				CASE  ");
            //stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_GERENTE_II  ");
            //stb.Append("            ELSE CL2.MATRICULA_GERENTE_II  ");
            //stb.Append("        END AS MATRICULA_GERENTE_II,  ");
            //stb.Append("				CASE  ");
            //stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_GERENTE_II  ");
            //stb.Append("            ELSE CL2.NOME_GERENTE_II  ");
            //stb.Append("        END AS NOME_GERENTE_II,  ");
            //stb.Append("				CASE  ");
            //stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_GERENTE_I  ");
            //stb.Append("            ELSE CL2.MATRICULA_GERENTE_I  ");
            //stb.Append("        END AS MATRICULA_GERENTE_I,  ");
            //stb.Append("				CASE  ");
            //stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_GERENTE_I  ");
            //stb.Append("            ELSE CL2.NOME_GERENTE_I  ");
            //stb.Append("        END AS NOME_GERENTE_I,  ");
            //stb.Append("				CASE  ");
            //stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_DIRETOR  ");
            //stb.Append("            ELSE CL2.MATRICULA_DIRETOR  ");
            //stb.Append("        END AS MATRICULA_DIRETOR,  ");
            //stb.Append("				CASE  ");
            //stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_DIRETOR  ");
            //stb.Append("            ELSE CL2.NOME_DIRETOR  ");
            //stb.Append("        END AS NOME_DIRETOR,  ");
            //stb.Append("				CASE  ");
            //stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_CEO  ");
            //stb.Append("            ELSE CL2.MATRICULA_CEO  ");
            //stb.Append("        END AS MATRICULA_CEO,  ");
            //stb.Append("				CASE  ");
            //stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_CEO  ");
            //stb.Append("            ELSE CL2.NOME_CEO  ");
            //stb.Append("        END AS NOME_CEO,  ");
            //stb.Append(" ");
            //stb.Append("        C.IDGDA_COLLABORATORS, ");
            //stb.Append("        CL.CREATED_AT ");
            //stb.Append("    FROM GDA_COLLABORATORS (NOLOCK) AS C ");
            //stb.Append("    LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CL  ");
            //stb.Append("        ON CL.IDGDA_COLLABORATORS = C.IDGDA_COLLABORATORS AND CL.CREATED_AT >= @DATAINICIAL AND CL.CREATED_AT <= @DATAFINAL ");
            //stb.Append("    LEFT JOIN GDA_COLLABORATORS_LAST_DETAILS (NOLOCK) AS CL2  ");
            //stb.Append("        ON CL2.IDGDA_COLLABORATORS = C.IDGDA_COLLABORATORS ");
            //stb.Append(") AS CL  ");
            //stb.Append("ON CL.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS AND CL.CREATED_AT = R.CREATED_AT ");

            stb.Append("LEFT JOIN GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME ON HME.INDICATORID = R.INDICADORID ");
            stb.Append("AND HME.deleted_at IS NULL ");
            stb.Append("LEFT JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON ME.ID = HME.MATHEMATICALEXPRESSIONID ");
            stb.Append("AND ME.DELETED_AT IS NULL ");
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
            stb.Append("AND HIG1.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND HIG1.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.Append("AND HIG1.GROUPID = 1 ");
            stb.Append("AND CONVERT(DATE,HIG1.STARTED_AT) <= R.CREATED_AT AND CONVERT(DATE,HIG1.ENDED_AT) >= R.CREATED_AT ");
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG2 ON HIG2.DELETED_AT IS NULL ");
            stb.Append("AND HIG2.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND HIG2.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.Append("AND HIG2.GROUPID = 2 ");
            stb.Append("AND CONVERT(DATE,HIG2.STARTED_AT) <= R.CREATED_AT AND CONVERT(DATE,HIG2.ENDED_AT) >= R.CREATED_AT ");
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG3 ON HIG3.DELETED_AT IS NULL ");
            stb.Append("AND HIG3.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND HIG3.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.Append("AND HIG3.GROUPID = 3 ");
            stb.Append("AND CONVERT(DATE,HIG3.STARTED_AT) <= R.CREATED_AT AND CONVERT(DATE,HIG3.ENDED_AT) >= R.CREATED_AT ");
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG4 ON HIG4.DELETED_AT IS NULL ");
            stb.Append("AND HIG4.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND HIG4.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.Append("AND HIG4.GROUPID = 4 ");
            stb.Append("AND CONVERT(DATE,HIG4.STARTED_AT) <= R.CREATED_AT AND CONVERT(DATE,HIG4.ENDED_AT) >= R.CREATED_AT ");
            stb.Append("LEFT JOIN GDA_CHECKING_ACCOUNT (NOLOCK) AS CA ON CA.COLLABORATOR_ID = R.IDGDA_COLLABORATORS ");
            stb.Append("AND CA.RESULT_DATE = R.CREATED_AT ");
            stb.Append("AND CA.GDA_INDICATOR_IDGDA_INDICATOR = R.INDICADORID ");
            stb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS IT ON IT.IDGDA_INDICATOR = R.INDICADORID ");
            stb.Append("LEFT JOIN ");
            stb.Append("  (SELECT SUM(INPUT) AS INPUT, ");
            //stb.Append("          idgda_sector, ");
            stb.Append("          gda_indicator_idgda_indicator, ");
            stb.Append("          result_date, ");
            stb.Append("          COLLABORATOR_ID ");
            stb.Append("   FROM GDA_CHECKING_ACCOUNT ");
            stb.Append("   WHERE CREATED_AT >= @DATAINICIAL ");
            stb.Append("     AND CREATED_AT <= @DATAFINAL ");
            //stb.Append("   GROUP BY idgda_sector, ");
            stb.Append("   GROUP BY  ");
            stb.Append("            gda_indicator_idgda_indicator, ");
            stb.Append("            result_date, ");
            stb.Append("            COLLABORATOR_ID) AS MZ ON  ");
            stb.Append(" MZ.gda_indicator_idgda_indicator = R.INDICADORID ");
            stb.Append("AND MZ.result_date = R.CREATED_AT ");
            stb.Append("AND MZ.COLLABORATOR_ID = R.IDGDA_COLLABORATORS ");
            //stb.Append("INNER JOIN GDA_BASKET_INDICATOR AS BK ON BK.SECTOR_ID = CL.IDGDA_SECTOR ");
            //stb.Append("AND BK.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND BK.DATE = R.CREATED_AT ");
            stb.Append("LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = CL.IDGDA_SECTOR ");
            stb.Append("LEFT JOIN GDA_SECTOR (NOLOCK) AS SECSUP ON SECSUP.IDGDA_SECTOR = CL.IDGDA_SECTOR_SUPERVISOR ");
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) AS HIS ON HIS.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND HIS.SECTOR_ID = CL.IDGDA_SECTOR AND CONVERT(DATE,HIS.STARTED_AT) <= R.CREATED_AT AND CONVERT(DATE,HIS.ENDED_AT) >= R.CREATED_AT ");
            stb.Append("AND HIS.DELETED_AT IS NULL ");

            //FK
            stb.Append("LEFT JOIN GDA_HISTORY_SCORE_INDICATOR_SECTOR (NOLOCK) AS SC ON ");
            stb.Append("SC.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND SC.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.Append("AND CONVERT(DATE,SC.STARTED_AT) <= R.CREATED_AT ");
            stb.Append("AND CONVERT(DATE,SC.ENDED_AT) >= R.CREATED_AT ");

            stb.Append("WHERE 1 = 1 ");
            stb.Append("  AND R.CREATED_AT >= @DATAINICIAL ");
            stb.Append("  AND R.CREATED_AT <= @DATAFINAL ");
            stb.Append("  AND R.DELETED_AT IS NULL ");
            //stb.Append("  AND CL.IDGDA_SECTOR IS NOT NULL ");
            //stb.Append("  AND CL.CARGO IS NOT NULL ");
            //stb.Append("  AND CL.HOME_BASED <> '' ");
            //stb.Append("  AND CL.active = 'true' ");
            stb.Append("  AND HIG1.MONETIZATION > 0 ");
            stb.Append("  AND R.FACTORS <> '0.000000;0.000000'");
            stb.Append("  AND R.FACTORS <> '0.000000; 0.000000'");
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
            //stb.Append("	   MAX(CL.CARGO) AS CARGO, ");
            //stb.Append("       MAX(IT.TYPE) AS TIPO, ");
            //stb.Append("       MAX(CONVERT(DATE, R.CREATED_AT)) AS 'REFERENCIA PAGAMENTO', ");
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
            //stb.Append("       MAX(IT.TYPE) AS TYPE, ");
            //stb.Append("       '' AS RESULTADO, ");
            //stb.Append("       '' AS PORCENTUAL, ");
            //stb.Append("       MAX(IT.TYPE) AS TYPE, ");
            //stb.Append("       MAX(R.RESULT) AS RESULTADOAPI, ");
            //stb.Append("       MAX(TRAB.TRABALHADO) AS TRABALHADO, ");
            //stb.Append("       MAX(ESC.ESCALADO) AS ESCALADO, ");
            //stb.Append("       MAX(BK.MONETIZATION_G1) AS META_MAXIMA, ");
            //stb.Append("       CASE ");
            //stb.Append("           WHEN MAX(MZ.INPUT) IS NULL THEN 0 ");
            //stb.Append("           ELSE MAX(MZ.INPUT) ");
            //stb.Append("       END AS MOEDA_GANHA, ");
            //stb.Append("       '' AS GRUPO, ");
            //stb.Append("       GETDATE() AS 'Data de atualização', ");
            //stb.Append("       MAX(CL.IDGDA_SECTOR) AS COD_GIP, ");
            //stb.Append("       MAX(SEC.NAME) AS SETOR, ");
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
            //stb.Append("LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CL ON CL.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS AND CL.CREATED_AT = R.CREATED_AT ");
            //stb.Append(" ");
            //stb.Append("LEFT JOIN GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME ON HME.INDICATORID = R.INDICADORID ");
            //stb.Append("AND HME.deleted_at IS NULL ");
            //stb.Append("LEFT JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON ME.ID = HME.MATHEMATICALEXPRESSIONID ");
            //stb.Append("AND ME.DELETED_AT IS NULL ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
            //stb.Append("AND HIG1.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIG1.SECTOR_ID = CL.IDGDA_SECTOR ");
            //stb.Append("AND HIG1.GROUPID = 1 ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG2 ON HIG2.DELETED_AT IS NULL ");
            //stb.Append("AND HIG2.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIG2.SECTOR_ID = CL.IDGDA_SECTOR ");
            //stb.Append("AND HIG2.GROUPID = 2 ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG3 ON HIG3.DELETED_AT IS NULL ");
            //stb.Append("AND HIG3.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIG3.SECTOR_ID = CL.IDGDA_SECTOR ");
            //stb.Append("AND HIG3.GROUPID = 3 ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG4 ON HIG4.DELETED_AT IS NULL ");
            //stb.Append("AND HIG4.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIG4.SECTOR_ID = CL.IDGDA_SECTOR ");
            //stb.Append("AND HIG4.GROUPID = 4 ");
            //stb.Append("LEFT JOIN GDA_CHECKING_ACCOUNT (NOLOCK) AS CA ON CA.COLLABORATOR_ID = R.IDGDA_COLLABORATORS ");
            //stb.Append("AND CA.RESULT_DATE = R.CREATED_AT ");
            //stb.Append("AND CA.GDA_INDICATOR_IDGDA_INDICATOR = R.INDICADORID ");
            //stb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS IT ON IT.IDGDA_INDICATOR = R.INDICADORID ");
            //stb.Append("LEFT JOIN ");
            //stb.Append("  (SELECT SUM(INPUT) AS INPUT, ");
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
            //stb.Append("            COLLABORATOR_ID) AS MZ ON MZ.idgda_sector = CL.IDGDA_SECTOR ");
            //stb.Append("AND MZ.gda_indicator_idgda_indicator = R.INDICADORID ");
            //stb.Append("AND MZ.result_date = R.CREATED_AT ");
            //stb.Append("AND MZ.COLLABORATOR_ID = R.IDGDA_COLLABORATORS ");
            //stb.Append("INNER JOIN GDA_BASKET_INDICATOR AS BK ON BK.SECTOR_ID = CL.IDGDA_SECTOR ");
            //stb.Append("AND BK.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND BK.DATE = R.CREATED_AT ");
            //stb.Append("LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = CL.IDGDA_SECTOR ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) AS HIS ON HIS.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIS.SECTOR_ID = CL.IDGDA_SECTOR ");
            //stb.Append("AND HIS.DELETED_AT IS NULL ");
            //stb.Append(" ");
            //stb.Append("WHERE 1 = 1 ");
            //stb.Append("  AND R.CREATED_AT >= @DATAINICIAL ");
            //stb.Append("  AND R.CREATED_AT <= @DATAFINAL ");
            //stb.Append("  AND R.DELETED_AT IS NULL ");
            //stb.Append("  AND CL.IDGDA_SECTOR IS NOT NULL ");
            //stb.Append("  AND CL.CARGO IS NOT NULL ");
            //stb.Append("  AND CL.HOME_BASED <> '' ");
            //stb.Append("  AND CL.active = 'true' ");
            //stb.AppendFormat(" {0} ", filter);
            //stb.Append(" ");
            //stb.Append("GROUP BY R.IDGDA_COLLABORATORS, ");
            //stb.Append("         R.INDICADORID, ");
            //stb.Append("         CONVERT(DATE, R.CREATED_AT) ");
            //stb.Append("		 UNION ALL ");
            //stb.Append("		 SELECT MAX(CONVERT(DATE, CA.CREATED_AT)) AS 'DATA DO PAGAMENTO', ");
            //stb.Append("       R.IDGDA_COLLABORATORS AS 'MATRICULA', ");
            //stb.Append("       MAX(CB.NAME) AS NAME, ");
            //stb.Append("	   MAX(CL2.CARGO) AS CARGO, ");
            //stb.Append("       MAX(IT.TYPE) AS TIPO, ");
            //stb.Append("       MAX(CONVERT(DATE, R.CREATED_AT)) AS 'REFERENCIA PAGAMENTO', ");
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
            //stb.Append("       MAX(IT.TYPE) AS TYPE, ");
            //stb.Append("       '' AS RESULTADO, ");
            //stb.Append("       '' AS PORCENTUAL, ");
            //stb.Append("       MAX(IT.TYPE) AS TYPE, ");
            //stb.Append("       MAX(R.RESULT) AS RESULTADOAPI, ");
            //stb.Append("       MAX(TRAB.TRABALHADO) AS TRABALHADO, ");
            //stb.Append("       MAX(ESC.ESCALADO) AS ESCALADO, ");
            //stb.Append("       MAX(BK.MONETIZATION_G1) AS META_MAXIMA, ");
            //stb.Append("       CASE ");
            //stb.Append("           WHEN MAX(MZ.INPUT) IS NULL THEN 0 ");
            //stb.Append("           ELSE MAX(MZ.INPUT) ");
            //stb.Append("       END AS MOEDA_GANHA, ");
            //stb.Append("       '' AS GRUPO, ");
            //stb.Append("       GETDATE() AS 'Data de atualização', ");
            //stb.Append("       MAX(CL2.IDGDA_SECTOR) AS COD_GIP, ");
            //stb.Append("       MAX(SEC.NAME) AS SETOR, ");
            //stb.Append("       MAX(CL2.HOME_BASED) AS HOME_BASED, ");
            //stb.Append("       MAX(CL2.SITE) AS SITE, ");
            //stb.Append("       MAX(CL2.PERIODO) AS TURNO, ");
            //stb.Append("       MAX(CL2.MATRICULA_SUPERVISOR) AS 'MATRICULA SUPERVISOR', ");
            //stb.Append("       MAX(CL2.NOME_SUPERVISOR) AS 'NOME SUPERVISOR', ");
            //stb.Append("       MAX(CL2.MATRICULA_COORDENADOR) AS 'MATRICULA COORDENADOR', ");
            //stb.Append("       MAX(CL2.NOME_COORDENADOR) AS 'NOME COORDENADOR', ");
            //stb.Append("       MAX(CL2.MATRICULA_GERENTE_II) AS 'MATRICULA GERENTE II', ");
            //stb.Append("       MAX(CL2.NOME_GERENTE_II) AS 'NOME GERENTE II', ");
            //stb.Append("       MAX(CL2.MATRICULA_GERENTE_I) AS 'MATRICULA GERENTE I', ");
            //stb.Append("       MAX(CL2.NOME_GERENTE_I) AS 'NOME GERENTE I', ");
            //stb.Append("       MAX(CL2.MATRICULA_DIRETOR) AS 'MATRICULA DIRETOR', ");
            //stb.Append("       MAX(CL2.NOME_DIRETOR) AS 'NOME DIRETOR', ");
            //stb.Append("       MAX(CL2.MATRICULA_CEO) AS 'MATRICULA CEO', ");
            //stb.Append("       MAX(CL2.NOME_CEO) AS 'NOME CEO' ");
            //stb.Append("FROM GDA_RESULT (NOLOCK) AS R ");
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
            //stb.Append("LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS CB ON CB.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            //stb.Append("LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CL ON CL.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS AND CL.CREATED_AT = R.CREATED_AT ");
            //stb.Append("LEFT JOIN GDA_COLLABORATORS_LAST_DETAILS (NOLOCK) AS CL2 ON CL2.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS ");
            //stb.Append(" ");
            //stb.Append(" ");
            //stb.Append("LEFT JOIN GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME ON HME.INDICATORID = R.INDICADORID ");
            //stb.Append("AND HME.deleted_at IS NULL ");
            //stb.Append("LEFT JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON ME.ID = HME.MATHEMATICALEXPRESSIONID ");
            //stb.Append("AND ME.DELETED_AT IS NULL ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
            //stb.Append("AND HIG1.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIG1.SECTOR_ID = CL2.IDGDA_SECTOR ");
            //stb.Append("AND HIG1.GROUPID = 1 ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG2 ON HIG2.DELETED_AT IS NULL ");
            //stb.Append("AND HIG2.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIG2.SECTOR_ID = CL2.IDGDA_SECTOR ");
            //stb.Append("AND HIG2.GROUPID = 2 ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG3 ON HIG3.DELETED_AT IS NULL ");
            //stb.Append("AND HIG3.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIG3.SECTOR_ID = CL2.IDGDA_SECTOR ");
            //stb.Append("AND HIG3.GROUPID = 3 ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG4 ON HIG4.DELETED_AT IS NULL ");
            //stb.Append("AND HIG4.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIG4.SECTOR_ID = CL2.IDGDA_SECTOR ");
            //stb.Append("AND HIG4.GROUPID = 4 ");
            //stb.Append("LEFT JOIN GDA_CHECKING_ACCOUNT (NOLOCK) AS CA ON CA.COLLABORATOR_ID = R.IDGDA_COLLABORATORS ");
            //stb.Append("AND CA.RESULT_DATE = R.CREATED_AT ");
            //stb.Append("AND CA.GDA_INDICATOR_IDGDA_INDICATOR = R.INDICADORID ");
            //stb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS IT ON IT.IDGDA_INDICATOR = R.INDICADORID ");
            //stb.Append("LEFT JOIN ");
            //stb.Append("  (SELECT SUM(INPUT) AS INPUT, ");
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
            //stb.Append("            COLLABORATOR_ID) AS MZ ON MZ.idgda_sector = CL2.IDGDA_SECTOR ");
            //stb.Append("AND MZ.gda_indicator_idgda_indicator = R.INDICADORID ");
            //stb.Append("AND MZ.result_date = R.CREATED_AT ");
            //stb.Append("AND MZ.COLLABORATOR_ID = R.IDGDA_COLLABORATORS ");
            //stb.Append("INNER JOIN GDA_BASKET_INDICATOR AS BK ON BK.SECTOR_ID = CL2.IDGDA_SECTOR ");
            //stb.Append("AND BK.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND BK.DATE = R.CREATED_AT ");
            //stb.Append("LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = CL2.IDGDA_SECTOR ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) AS HIS ON HIS.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIS.SECTOR_ID = CL2.IDGDA_SECTOR ");
            //stb.Append("AND HIS.DELETED_AT IS NULL ");
            //stb.Append(" ");
            //stb.Append("WHERE 1 = 1 ");
            //stb.Append("  AND R.CREATED_AT >= @DATAINICIAL ");
            //stb.Append("  AND R.CREATED_AT <= @DATAFINAL ");
            //stb.AppendFormat(" {0} ", filter2);
            //stb.Append("  AND CL.IDGDA_SECTOR IS NULL ");
            //stb.Append(" AND (CL.IDGDA_SECTOR IS NULL OR CL.CARGO IS NULL OR CL.HOME_BASED = '') ");
            //stb.Append("  AND R.DELETED_AT IS NULL ");
            //stb.Append("  AND CL2.ACTIVE = 'true' ");
            //stb.Append(" ");
            //stb.Append("GROUP BY R.IDGDA_COLLABORATORS, ");
            //stb.Append("         R.INDICADORID, ");
            //stb.Append("         CONVERT(DATE, R.CREATED_AT) ");
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
                            string factors = reader["FATOR"].ToString();

                            if (factors == "0.000000;0.000000" || factors == "0.000000; 0.000000")
                            {
                                continue;
                            }

                            string factor0 = Funcoes.RemoverZerosAposPonto(factors.Split(";")[0]);
                            string factor1 = Funcoes.RemoverZerosAposPonto(factors.Split(";")[1]);

                            ModelsEx.homeRel rmam = new ModelsEx.homeRel();
                            //rmam.month = reader["MES"].ToString();
                            //rmam.year = reader["ANO"].ToString();
                            rmam.datePay = reader["DATA DO PAGAMENTO"].ToString();
                            rmam.dateReferer = reader["REFERENCIA PAGAMENTO"].ToString();

                            rmam.data = reader["REFERENCIA PAGAMENTO"].ToString();

                            rmam.idcollaborator = reader["MATRICULA"].ToString();
                            rmam.name = reader["name"].ToString();
         
                            //rmam.dias_trabalhados = reader["dias trabalhados"].ToString();
                            rmam.cod_indicador = reader["cod indicador"].ToString();
                            rmam.indicador = reader["indicador"].ToString();
                            rmam.meta = reader["meta"].ToString();
                            rmam.indicatorType = reader["TIPO"].ToString();
                            //rmam.resultado = double.Parse(reader["resultado"].ToString());
                            //rmam.porcentual = double.Parse(reader["porcentual"].ToString());
                            //rmam.ganho_em_moedas = double.Parse(reader["ganho em moedas"].ToString());
                            //rmam.meta_maxima_de_moedas = reader["meta maxima de moedas"].ToString();
                            //rmam.grupo = reader["grupo"].ToString();
                            rmam.data_atualizacao = reader["data de atualização"].ToString();
               

                            rmam.cod_gip_supervisor = reader["COD_GIP_SUPERVISOR"].ToString();

    
                            rmam.setor_supervisor = reader["setor_supervisor"].ToString();



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

                            if (reader["home_based"].ToString() == "")
                            {
                                rmam.home_based = "Não Informado";
                            }
                            else
                            {
                                rmam.home_based = reader["home_based"].ToString();
                            }

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

                            rmam.min1 = reader["min1"].ToString() != "" ? double.Parse(reader["min1"].ToString()) : 0;
                            rmam.min2 = reader["min2"].ToString() != "" ? double.Parse(reader["min2"].ToString()) : 0;
                            rmam.min3 = reader["min3"].ToString() != "" ? double.Parse(reader["min3"].ToString()) : 0;
                            rmam.min4 = reader["min4"].ToString() != "" ? double.Parse(reader["min4"].ToString()) : 0;
                            //rmam.min1 = double.Parse(reader["min1"].ToString());
                            //rmam.min2 = double.Parse(reader["min2"].ToString());
                            //rmam.min3 = double.Parse(reader["min3"].ToString());
                            //rmam.min4 = double.Parse(reader["min4"].ToString());
                            rmam.conta = reader["CONTA"].ToString();
                            //rmam.better = reader["TYPE"].ToString();
                            rmam.better = reader["BETTER"].ToString();
                            rmam.goal = reader["meta"].ToString() != "" ? double.Parse(reader["meta"].ToString()) : 0;


                            //if (reader["meta"].ToString() != "")
                            //{
                            //    rmam.goal = double.Parse(reader["meta"].ToString());
                            //}
                            //else
                            //{
                            //    //Não veio informação
                            //    rmam.goal = 0;
                            //}


                            //rmam.resultadoAPI = 1;



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
            public string Home { get; set; }
            public string Turno { get; set; }
            public string Site { get; set; }
            public double Score { get; set; }
        }




        //public factors returnFactorsById(string idCollaborator, string dataInicial, string idIndicator, string idSector, factors factor, Boolean calculoDiaDia, string dataFinal)
        //{

        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}';  ", idCollaborator);
        //    sb.AppendFormat("DECLARE @DATEENV DATE; SET @DATEENV = '{0}';  ", dataInicial);
        //    sb.AppendFormat("DECLARE @DATEFIM DATE; SET @DATEFIM = '{0}';  ", dataFinal);
        //    sb.AppendFormat("DECLARE @INDICADORID VARCHAR(MAX); SET @INDICADORID = '{0}';  ", idIndicator);
        //    sb.AppendFormat("DECLARE @IDGDA_SECTOR VARCHAR(MAX); SET @IDGDA_SECTOR = '{0}'; ", idSector);
        //    sb.Append("WITH HIERARCHYCTE AS ");
        //    sb.Append("  (SELECT IDGDA_HISTORY_HIERARCHY_RELATIONSHIP, ");
        //    sb.Append("          CONTRACTORCONTROLID, ");
        //    sb.Append("          PARENTIDENTIFICATION, ");
        //    sb.Append("          IDGDA_COLLABORATORS, ");
        //    sb.Append("          IDGDA_HIERARCHY, ");
        //    sb.Append("          CREATED_AT, ");
        //    sb.Append("          DELETED_AT, ");
        //    sb.Append("          TRANSACTIONID, ");
        //    sb.Append("          LEVELWEIGHT, DATE, LEVELNAME ");
        //    sb.Append("   FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) ");
        //    sb.Append("   WHERE IDGDA_COLLABORATORS = @INPUTID ");
        //    sb.Append("     AND  [DATE] = @DATEENV ");
        //    sb.Append("   UNION ALL SELECT H.IDGDA_HISTORY_HIERARCHY_RELATIONSHIP, ");
        //    sb.Append("                    H.CONTRACTORCONTROLID, ");
        //    sb.Append("                    H.PARENTIDENTIFICATION, ");
        //    sb.Append("                    H.IDGDA_COLLABORATORS, ");
        //    sb.Append("                    H.IDGDA_HIERARCHY, ");
        //    sb.Append("                    H.CREATED_AT, ");
        //    sb.Append("                    H.DELETED_AT, ");
        //    sb.Append("                    H.TRANSACTIONID, ");
        //    sb.Append("                    H.LEVELWEIGHT, ");
        //    sb.Append("                    H.DATE, ");
        //    sb.Append("                    H.LEVELNAME ");
        //    sb.Append("   FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP H (NOLOCK) ");
        //    sb.Append("   INNER JOIN HIERARCHYCTE CTE ON H.PARENTIDENTIFICATION = CTE.IDGDA_COLLABORATORS ");
        //    sb.Append("   WHERE CTE.LEVELNAME <> 'AGENTE' ");
        //    sb.Append("     AND CTE.[DATE] = @DATEENV ) ");
        //    sb.Append("	 SELECT SUM(F1.FACTOR) AS FATOR0, ");
        //    sb.Append("       SUM(F2.FACTOR) AS FATOR1, HIS.GOAL, ");
        //    sb.Append("       I.WEIGHT AS WEIGHT, ");
        //    sb.Append("       HHR.LEVELWEIGHT AS HIERARCHYLEVEL, ");
        //    sb.Append("       HIG1.MONETIZATION AS COIN1, ");
        //    sb.Append("       HIG2.MONETIZATION AS COIN2, ");
        //    sb.Append("       HIG3.MONETIZATION AS COIN3, ");
        //    sb.Append("       HIG4.MONETIZATION AS COIN4, ");
        //    sb.Append("       S.IDGDA_SECTOR, ");
        //    sb.Append("       HIG1.METRIC_MIN AS MIN1, ");
        //    sb.Append("       HIG2.METRIC_MIN AS MIN2, ");
        //    sb.Append("       HIG3.METRIC_MIN AS MIN3, ");
        //    sb.Append("       HIG4.METRIC_MIN AS MIN4, ");
        //    sb.Append("       CASE ");
        //    sb.Append("           WHEN MAX(TBL.EXPRESSION) IS NULL THEN '(#FATOR1/#FATOR0)' ");
        //    sb.Append("           ELSE MAX(TBL.EXPRESSION) ");
        //    sb.Append("       END AS CONTA, ");
        //    sb.Append("       CASE ");
        //    sb.Append("           WHEN MAX(I.CALCULATION_TYPE) IS NULL THEN 'BIGGER_BETTER' ");
        //    sb.Append("           ELSE MAX(I.CALCULATION_TYPE) ");
        //    sb.Append("       END AS BETTER ");
        //    sb.Append("FROM GDA_RESULT (NOLOCK) AS R ");
        //    sb.Append("INNER JOIN GDA_HISTORY_COLLABORATOR_SECTOR (NOLOCK) AS S ON  S.CREATED_AT = R.CREATED_AT ");
        //    sb.Append("AND S.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
        //    sb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
        //    sb.Append("AND HIG1.INDICATOR_ID = R.INDICADORID ");
        //    sb.Append("AND HIG1.SECTOR_ID = S.IDGDA_SECTOR ");
        //    sb.Append("AND HIG1.GROUPID = 1 ");
        //    sb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG2 ON HIG2.DELETED_AT IS NULL ");
        //    sb.Append("AND HIG2.INDICATOR_ID = R.INDICADORID ");
        //    sb.Append("AND HIG2.SECTOR_ID = S.IDGDA_SECTOR ");
        //    sb.Append("AND HIG2.GROUPID = 2 ");
        //    sb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG3 ON HIG3.DELETED_AT IS NULL ");
        //    sb.Append("AND HIG3.INDICATOR_ID = R.INDICADORID ");
        //    sb.Append("AND HIG3.SECTOR_ID = S.IDGDA_SECTOR ");
        //    sb.Append("AND HIG3.GROUPID = 3 ");
        //    sb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG4 ON HIG4.DELETED_AT IS NULL ");
        //    sb.Append("AND HIG4.INDICATOR_ID = R.INDICADORID ");
        //    sb.Append("AND HIG4.SECTOR_ID = S.IDGDA_SECTOR ");
        //    sb.Append("AND HIG4.GROUPID = 4 ");
        //    sb.Append("LEFT JOIN ");
        //    sb.Append("  (SELECT HME.INDICATORID, ");
        //    sb.Append("          ME.EXPRESSION ");
        //    sb.Append("   FROM GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME ");
        //    sb.Append("   INNER JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON ME.ID = HME.MATHEMATICALEXPRESSIONID ");
        //    sb.Append("   WHERE HME.DELETED_AT IS NULL) AS TBL ON TBL.INDICATORID = R.INDICADORID ");
        //    sb.Append("LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS HHR ON HHR.idgda_COLLABORATORS = R.IDGDA_COLLABORATORS ");
        //    sb.Append("AND HHR.DATE = @DATEENV ");
        //    sb.Append("INNER JOIN ");
        //    sb.Append("  (SELECT GOAL, ");
        //    sb.Append("          INDICATOR_ID, ");
        //    sb.Append("          SECTOR_ID, ");
        //    sb.Append("          ROW_NUMBER() OVER (PARTITION BY INDICATOR_ID, ");
        //    sb.Append("                                          SECTOR_ID ");
        //    sb.Append("                             ORDER BY CREATED_AT DESC) AS RN ");
        //    sb.Append("   FROM GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) ");
        //    sb.Append("   WHERE DELETED_AT IS NULL ) AS HIS ON HIS.INDICATOR_ID = R.INDICADORID ");
        //    sb.Append("AND HIS.SECTOR_ID = S.IDGDA_SECTOR ");
        //    sb.Append("AND HIS.RN = 1 ");
        //    sb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS I ON I.IDGDA_INDICATOR = R.INDICADORID ");
        //    sb.Append("INNER JOIN GDA_FACTOR (NOLOCK) AS F1 ON F1.IDGDA_RESULT = R.IDGDA_RESULT AND F1.[INDEX] = 1 ");
        //    sb.Append("INNER JOIN GDA_FACTOR (NOLOCK) AS F2 ON F2.IDGDA_RESULT = R.IDGDA_RESULT AND F2.[INDEX] = 2 ");
        //    sb.Append("WHERE R.IDGDA_COLLABORATORS IN ");
        //    sb.Append("    (SELECT distinct(IDGDA_COLLABORATORS) ");
        //    sb.Append("     FROM HierarchyCTE ");
        //    sb.Append("     WHERE LEVELNAME = 'AGENTE' ");
        //    sb.Append("       AND DATE = @DateEnv ) ");
        //    sb.Append("  AND S.IDGDA_SECTOR = @IDGDA_SECTOR ");

        //    if (calculoDiaDia == true)
        //    {
        //        sb.Append("  AND R.CREATED_AT = @DateEnv ");
        //    }
        //    else
        //    {
        //        sb.Append("  AND R.CREATED_AT >= @DateEnv AND R.CREATED_AT <= @DATEFIM ");
        //    }


        //    sb.Append("  AND I.STATUS = 1 ");
        //    sb.Append("  AND R.DELETED_AT IS NULL ");
        //    sb.Append("  AND R.INDICADORID = @INDICADORID ");
        //    sb.Append("GROUP BY INDICADORID, ");
        //    sb.Append("         R.TRANSACTIONID, ");
        //    sb.Append("         S.IDGDA_SECTOR, ");
        //    sb.Append("         HIG1.METRIC_MIN, ");
        //    sb.Append("         HIG2.METRIC_MIN, ");
        //    sb.Append("         HIG3.METRIC_MIN, ");
        //    sb.Append("         HIG4.METRIC_MIN, ");
        //    sb.Append("         HIS.GOAL, ");
        //    sb.Append("         I.WEIGHT, ");
        //    sb.Append("         HHR.LEVELWEIGHT, ");
        //    sb.Append("         HIG1.MONETIZATION, ");
        //    sb.Append("         HIG2.MONETIZATION, ");
        //    sb.Append("         HIG3.MONETIZATION, ");
        //    sb.Append("         HIG4.MONETIZATION ");


        
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();
        //        try
        //        {
        //            using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
        //            {
        //                command.CommandTimeout = 60;
        //                using (SqlDataReader reader = command.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        var a = factor.factor0;
        //                        var b = reader["fator0"];

        //                        factor.factor0 = factor.factor0 + double.Parse(reader["fator0"].ToString());
        //                        factor.factor1 = factor.factor1 + double.Parse(reader["fator1"].ToString());

        //                        factor.goal = double.Parse(reader["goal"].ToString());
        //                        factor.min1 = double.Parse(reader["min1"].ToString());
        //                        factor.min2 = double.Parse(reader["min2"].ToString());
        //                        factor.min3 = double.Parse(reader["min3"].ToString());
        //                        factor.min4 = double.Parse(reader["min4"].ToString());
        //                        factor.count = reader["conta"].ToString();
        //                        factor.better = reader["better"].ToString();
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //        connection.Close();
        //    }

        //    return factor;
        //}


        //public factors returnFactorsByIdHierarchy(string idCollaborator, string dataInicial, string idIndicator, string idSector, factors factor, Boolean calculoDiaDia, string dataFinal)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}';  ", idCollaborator);
        //    sb.AppendFormat("DECLARE @DATEENV DATE; SET @DATEENV = '{0}';  ", dataInicial);
        //    sb.AppendFormat("DECLARE @DATEFIM DATE; SET @DATEFIM = '{0}';  ", dataFinal);
        //    sb.AppendFormat("DECLARE @INDICADORID VARCHAR(MAX); SET @INDICADORID = '{0}';  ", idIndicator);
        //    sb.AppendFormat("DECLARE @IDGDA_SECTOR VARCHAR(MAX); SET @IDGDA_SECTOR = '{0}'; ", idSector);
        //    sb.Append("");
        //    sb.Append("WITH HIERARCHYCTE AS ");
        //    sb.Append("  (SELECT IDGDA_HISTORY_HIERARCHY_RELATIONSHIP, ");
        //    sb.Append("          CONTRACTORCONTROLID, ");
        //    sb.Append("          PARENTIDENTIFICATION, ");
        //    sb.Append("          IDGDA_COLLABORATORS, ");
        //    sb.Append("          IDGDA_HIERARCHY, ");
        //    sb.Append("          CREATED_AT, ");
        //    sb.Append("          DELETED_AT, ");
        //    sb.Append("          TRANSACTIONID, ");
        //    sb.Append("          LEVELWEIGHT, DATE, LEVELNAME ");
        //    sb.Append("   FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) ");
        //    sb.Append("   WHERE IDGDA_COLLABORATORS = @INPUTID ");
        //    sb.Append("     AND [DATE] = @DATEENV ");
        //    sb.Append("   UNION ALL SELECT H.IDGDA_HISTORY_HIERARCHY_RELATIONSHIP, ");
        //    sb.Append("                    H.CONTRACTORCONTROLID, ");
        //    sb.Append("                    H.PARENTIDENTIFICATION, ");
        //    sb.Append("                    H.IDGDA_COLLABORATORS, ");
        //    sb.Append("                    H.IDGDA_HIERARCHY, ");
        //    sb.Append("                    H.CREATED_AT, ");
        //    sb.Append("                    H.DELETED_AT, ");
        //    sb.Append("                    H.TRANSACTIONID, ");
        //    sb.Append("                    H.LEVELWEIGHT, ");
        //    sb.Append("                    H.DATE, ");
        //    sb.Append("                    H.LEVELNAME ");
        //    sb.Append("   FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP H (NOLOCK) ");
        //    sb.Append("   INNER JOIN HIERARCHYCTE CTE ON H.PARENTIDENTIFICATION = CTE.IDGDA_COLLABORATORS ");
        //    sb.Append("   WHERE CTE.LEVELNAME <> 'AGENTE' ");
        //    sb.Append("     AND CTE.[DATE] = @DATEENV ) ");
        //    sb.Append("SELECT SUM(F1.FACTOR) AS FATOR0, ");
        //    sb.Append("       SUM(F2.FACTOR) AS FATOR1, ");
        //    sb.Append("	    (SELECT SUM(INPUT) AS INPUT  FROM GDA_CHECKING_ACCOUNT (NOLOCK)  ");
        //    sb.Append("           	   where idgda_sector = @IDGDA_SECTOR AND gda_indicator_idgda_indicator = @INDICADORID  ");
        //    sb.Append("            	   AND result_date = @DATEENV AND COLLABORATOR_ID = @INPUTID) AS 'GANHO EM MOEDAS',  ");
        //    sb.Append("       HIS.GOAL, ");
        //    sb.Append("       I.WEIGHT AS WEIGHT, ");
        //    sb.Append("       HHR.LEVELWEIGHT AS HIERARCHYLEVEL, ");
        //    sb.Append("       HIG1.MONETIZATION AS COIN1, ");
        //    sb.Append("       HIG2.MONETIZATION AS COIN2, ");
        //    sb.Append("       HIG3.MONETIZATION AS COIN3, ");
        //    sb.Append("       HIG4.MONETIZATION AS COIN4, ");
        //    sb.Append("       S.IDGDA_SECTOR, ");
        //    sb.Append("       HIG1.METRIC_MIN AS MIN1, ");
        //    sb.Append("       HIG2.METRIC_MIN AS MIN2, ");
        //    sb.Append("       HIG3.METRIC_MIN AS MIN3, ");
        //    sb.Append("       HIG4.METRIC_MIN AS MIN4, ");
        //    sb.Append("       CASE ");
        //    sb.Append("           WHEN MAX(TBL.EXPRESSION) IS NULL THEN '(#FATOR1/#FATOR0)' ");
        //    sb.Append("           ELSE MAX(TBL.EXPRESSION) ");
        //    sb.Append("       END AS CONTA, ");
        //    sb.Append("       CASE ");
        //    sb.Append("           WHEN MAX(I.CALCULATION_TYPE) IS NULL THEN 'BIGGER_BETTER' ");
        //    sb.Append("           ELSE MAX(I.CALCULATION_TYPE) ");
        //    sb.Append("       END AS BETTER ");
        //    sb.Append("FROM GDA_RESULT (NOLOCK) AS R ");
        //    sb.Append("INNER JOIN GDA_HISTORY_COLLABORATOR_SECTOR (NOLOCK) AS S ON S.CREATED_AT = R.CREATED_AT ");
        //    sb.Append("AND S.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
        //    sb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
        //    sb.Append("AND HIG1.INDICATOR_ID = R.INDICADORID ");
        //    sb.Append("AND HIG1.SECTOR_ID = S.IDGDA_SECTOR ");
        //    sb.Append("AND HIG1.GROUPID = 1 ");
        //    sb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG2 ON HIG2.DELETED_AT IS NULL ");
        //    sb.Append("AND HIG2.INDICATOR_ID = R.INDICADORID ");
        //    sb.Append("AND HIG2.SECTOR_ID = S.IDGDA_SECTOR ");
        //    sb.Append("AND HIG2.GROUPID = 2 ");
        //    sb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG3 ON HIG3.DELETED_AT IS NULL ");
        //    sb.Append("AND HIG3.INDICATOR_ID = R.INDICADORID ");
        //    sb.Append("AND HIG3.SECTOR_ID = S.IDGDA_SECTOR ");
        //    sb.Append("AND HIG3.GROUPID = 3 ");
        //    sb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG4 ON HIG4.DELETED_AT IS NULL ");
        //    sb.Append("AND HIG4.INDICATOR_ID = R.INDICADORID ");
        //    sb.Append("AND HIG4.SECTOR_ID = S.IDGDA_SECTOR ");
        //    sb.Append("AND HIG4.GROUPID = 4 ");
        //    sb.Append("LEFT JOIN ");
        //    sb.Append("  (SELECT HME.INDICATORID, ");
        //    sb.Append("          ME.EXPRESSION ");
        //    sb.Append("   FROM GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME ");
        //    sb.Append("   INNER JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON ME.ID = HME.MATHEMATICALEXPRESSIONID ");
        //    sb.Append("   WHERE HME.DELETED_AT IS NULL) AS TBL ON TBL.INDICATORID = R.INDICADORID ");
        //    sb.Append("LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS HHR ON HHR.idgda_COLLABORATORS = R.IDGDA_COLLABORATORS ");
        //    sb.Append("AND HHR.DATE = @DATEENV ");
        //    sb.Append("INNER JOIN ");
        //    sb.Append("  (SELECT GOAL, ");
        //    sb.Append("          INDICATOR_ID, ");
        //    sb.Append("          SECTOR_ID, ");
        //    sb.Append("          ROW_NUMBER() OVER (PARTITION BY INDICATOR_ID, ");
        //    sb.Append("                                          SECTOR_ID ");
        //    sb.Append("                             ORDER BY CREATED_AT DESC) AS RN ");
        //    sb.Append("   FROM GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) ");
        //    sb.Append("   WHERE DELETED_AT IS NULL ) AS HIS ON HIS.INDICATOR_ID = R.INDICADORID ");
        //    sb.Append("AND HIS.SECTOR_ID = S.IDGDA_SECTOR ");
        //    sb.Append("AND HIS.RN = 1 ");
        //    sb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS I ON I.IDGDA_INDICATOR = R.INDICADORID ");
        //    sb.Append("INNER JOIN GDA_FACTOR (NOLOCK) AS F1 ON F1.IDGDA_RESULT = R.IDGDA_RESULT ");
        //    sb.Append("AND F1.[INDEX] = 1 ");
        //    sb.Append("INNER JOIN GDA_FACTOR (NOLOCK) AS F2 ON F2.IDGDA_RESULT = R.IDGDA_RESULT ");
        //    sb.Append("AND F2.[INDEX] = 2 ");
        //    sb.Append("WHERE R.IDGDA_COLLABORATORS IN ");
        //    sb.Append("    (SELECT distinct(IDGDA_COLLABORATORS) ");
        //    sb.Append("     FROM HierarchyCTE ");
        //    sb.Append("     WHERE LEVELNAME = 'AGENTE' ");
        //    sb.Append("       AND DATE = @DateEnv ) ");
        //    sb.Append("  AND S.IDGDA_SECTOR = @IDGDA_SECTOR ");

        //    if (calculoDiaDia == true)
        //    {
        //        sb.Append("  AND R.CREATED_AT = @DateEnv  ");
        //    }
        //    else
        //    {
        //        sb.Append("  AND R.CREATED_AT >= @DateEnv AND R.CREATED_AT <= @DATEFIM  ");
        //    }

        //    sb.Append("  AND I.STATUS = 1 ");
        //    sb.Append("  AND R.DELETED_AT IS NULL ");
        //    sb.Append("  AND R.INDICADORID = @INDICADORID ");
        //    sb.Append("GROUP BY INDICADORID, ");
        //    sb.Append("         R.TRANSACTIONID, ");
        //    sb.Append("         S.IDGDA_SECTOR, ");
        //    sb.Append("         HIG1.METRIC_MIN, ");
        //    sb.Append("         HIG2.METRIC_MIN, ");
        //    sb.Append("         HIG3.METRIC_MIN, ");
        //    sb.Append("         HIG4.METRIC_MIN, ");
        //    sb.Append("         HIS.GOAL, ");
        //    sb.Append("         I.WEIGHT, ");
        //    sb.Append("         HHR.LEVELWEIGHT, ");
        //    sb.Append("         HIG1.MONETIZATION, ");
        //    sb.Append("         HIG2.MONETIZATION, ");
        //    sb.Append("         HIG3.MONETIZATION, ");
        //    sb.Append("         HIG4.MONETIZATION  ");

        //    #region Comentario
        //    //StringBuilder sb = new StringBuilder();
        //    //sb.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}';  ", idCollaborator);
        //    //sb.AppendFormat("DECLARE @DATEENV DATE; SET @DATEENV = '{0}';  ", dataInicial);
        //    //sb.AppendFormat("DECLARE @DATEFIM DATE; SET @DATEFIM = '{0}';  ", dataFinal);
        //    //sb.AppendFormat("DECLARE @INDICADORID VARCHAR(MAX); SET @INDICADORID = '{0}';  ", idIndicator);
        //    //sb.AppendFormat("DECLARE @IDGDA_SECTOR VARCHAR(MAX); SET @IDGDA_SECTOR = '{0}'; ", idSector);
        //    //sb.Append("WITH HIERARCHYCTE AS ");
        //    //sb.Append("  (SELECT IDGDA_HISTORY_HIERARCHY_RELATIONSHIP, ");
        //    //sb.Append("          CONTRACTORCONTROLID, ");
        //    //sb.Append("          PARENTIDENTIFICATION, ");
        //    //sb.Append("          IDGDA_COLLABORATORS, ");
        //    //sb.Append("          IDGDA_HIERARCHY, ");
        //    //sb.Append("          CREATED_AT, ");
        //    //sb.Append("          DELETED_AT, ");
        //    //sb.Append("          TRANSACTIONID, ");
        //    //sb.Append("          LEVELWEIGHT, DATE, LEVELNAME ");
        //    //sb.Append("   FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) ");
        //    //sb.Append("   WHERE IDGDA_COLLABORATORS = @INPUTID ");
        //    //sb.Append("     AND  [DATE] = @DATEENV ");
        //    //sb.Append("   UNION ALL SELECT H.IDGDA_HISTORY_HIERARCHY_RELATIONSHIP, ");
        //    //sb.Append("                    H.CONTRACTORCONTROLID, ");
        //    //sb.Append("                    H.PARENTIDENTIFICATION, ");
        //    //sb.Append("                    H.IDGDA_COLLABORATORS, ");
        //    //sb.Append("                    H.IDGDA_HIERARCHY, ");
        //    //sb.Append("                    H.CREATED_AT, ");
        //    //sb.Append("                    H.DELETED_AT, ");
        //    //sb.Append("                    H.TRANSACTIONID, ");
        //    //sb.Append("                    H.LEVELWEIGHT, ");
        //    //sb.Append("                    H.DATE, ");
        //    //sb.Append("                    H.LEVELNAME ");
        //    //sb.Append("   FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP H (NOLOCK) ");
        //    //sb.Append("   INNER JOIN HIERARCHYCTE CTE ON H.PARENTIDENTIFICATION = CTE.IDGDA_COLLABORATORS ");
        //    //sb.Append("   WHERE CTE.LEVELNAME <> 'AGENTE' ");
        //    //sb.Append("     AND CTE.[DATE] = @DATEENV ) ");
        //    //sb.Append("	 SELECT SUM(F1.FACTOR) AS FATOR0, ");
        //    //sb.Append("       SUM(F2.FACTOR) AS FATOR1, HIS.GOAL, ");
        //    //sb.Append("       I.WEIGHT AS WEIGHT, ");
        //    //sb.Append("       HHR.LEVELWEIGHT AS HIERARCHYLEVEL, ");
        //    //sb.Append("       HIG1.MONETIZATION AS COIN1, ");
        //    //sb.Append("       HIG2.MONETIZATION AS COIN2, ");
        //    //sb.Append("       HIG3.MONETIZATION AS COIN3, ");
        //    //sb.Append("       HIG4.MONETIZATION AS COIN4, ");
        //    //sb.Append("       S.IDGDA_SECTOR, ");
        //    //sb.Append("       HIG1.METRIC_MIN AS MIN1, ");
        //    //sb.Append("       HIG2.METRIC_MIN AS MIN2, ");
        //    //sb.Append("       HIG3.METRIC_MIN AS MIN3, ");
        //    //sb.Append("       HIG4.METRIC_MIN AS MIN4, ");
        //    //sb.Append("       CASE ");
        //    //sb.Append("           WHEN MAX(TBL.EXPRESSION) IS NULL THEN '(#FATOR1/#FATOR0)' ");
        //    //sb.Append("           ELSE MAX(TBL.EXPRESSION) ");
        //    //sb.Append("       END AS CONTA, ");
        //    //sb.Append("       CASE ");
        //    //sb.Append("           WHEN MAX(I.CALCULATION_TYPE) IS NULL THEN 'BIGGER_BETTER' ");
        //    //sb.Append("           ELSE MAX(I.CALCULATION_TYPE) ");
        //    //sb.Append("       END AS BETTER ");
        //    //sb.Append("FROM GDA_RESULT (NOLOCK) AS R ");
        //    //sb.Append("INNER JOIN GDA_HISTORY_COLLABORATOR_SECTOR (NOLOCK) AS S ON  S.CREATED_AT = R.CREATED_AT ");
        //    //sb.Append("AND S.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
        //    //sb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
        //    //sb.Append("AND HIG1.INDICATOR_ID = R.INDICADORID ");
        //    //sb.Append("AND HIG1.SECTOR_ID = S.IDGDA_SECTOR ");
        //    //sb.Append("AND HIG1.GROUPID = 1 ");
        //    //sb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG2 ON HIG2.DELETED_AT IS NULL ");
        //    //sb.Append("AND HIG2.INDICATOR_ID = R.INDICADORID ");
        //    //sb.Append("AND HIG2.SECTOR_ID = S.IDGDA_SECTOR ");
        //    //sb.Append("AND HIG2.GROUPID = 2 ");
        //    //sb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG3 ON HIG3.DELETED_AT IS NULL ");
        //    //sb.Append("AND HIG3.INDICATOR_ID = R.INDICADORID ");
        //    //sb.Append("AND HIG3.SECTOR_ID = S.IDGDA_SECTOR ");
        //    //sb.Append("AND HIG3.GROUPID = 3 ");
        //    //sb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG4 ON HIG4.DELETED_AT IS NULL ");
        //    //sb.Append("AND HIG4.INDICATOR_ID = R.INDICADORID ");
        //    //sb.Append("AND HIG4.SECTOR_ID = S.IDGDA_SECTOR ");
        //    //sb.Append("AND HIG4.GROUPID = 4 ");
        //    //sb.Append("LEFT JOIN ");
        //    //sb.Append("  (SELECT HME.INDICATORID, ");
        //    //sb.Append("          ME.EXPRESSION ");
        //    //sb.Append("   FROM GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME ");
        //    //sb.Append("   INNER JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON ME.ID = HME.MATHEMATICALEXPRESSIONID ");
        //    //sb.Append("   WHERE HME.DELETED_AT IS NULL) AS TBL ON TBL.INDICATORID = R.INDICADORID ");
        //    //sb.Append("LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS HHR ON HHR.idgda_COLLABORATORS = R.IDGDA_COLLABORATORS ");
        //    //sb.Append("AND HHR.DATE = @DATEENV ");
        //    //sb.Append("INNER JOIN ");
        //    //sb.Append("  (SELECT GOAL, ");
        //    //sb.Append("          INDICATOR_ID, ");
        //    //sb.Append("          SECTOR_ID, ");
        //    //sb.Append("          ROW_NUMBER() OVER (PARTITION BY INDICATOR_ID, ");
        //    //sb.Append("                                          SECTOR_ID ");
        //    //sb.Append("                             ORDER BY CREATED_AT DESC) AS RN ");
        //    //sb.Append("   FROM GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) ");
        //    //sb.Append("   WHERE DELETED_AT IS NULL ) AS HIS ON HIS.INDICATOR_ID = R.INDICADORID ");
        //    //sb.Append("AND HIS.SECTOR_ID = S.IDGDA_SECTOR ");
        //    //sb.Append("AND HIS.RN = 1 ");
        //    //sb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS I ON I.IDGDA_INDICATOR = R.INDICADORID ");
        //    //sb.Append("INNER JOIN GDA_FACTOR (NOLOCK) AS F1 ON F1.IDGDA_RESULT = R.IDGDA_RESULT AND F1.[INDEX] = 1 ");
        //    //sb.Append("INNER JOIN GDA_FACTOR (NOLOCK) AS F2 ON F2.IDGDA_RESULT = R.IDGDA_RESULT AND F2.[INDEX] = 2 ");
        //    //sb.Append("WHERE R.IDGDA_COLLABORATORS IN ");
        //    //sb.Append("    (SELECT distinct(IDGDA_COLLABORATORS) ");
        //    //sb.Append("     FROM HierarchyCTE ");
        //    //sb.Append("     WHERE LEVELNAME = 'AGENTE' ");
        //    //sb.Append("       AND DATE = @DateEnv ) ");
        //    //sb.Append("  AND S.IDGDA_SECTOR = @IDGDA_SECTOR ");

        //    //if (calculoDiaDia == true)
        //    //{
        //    //    sb.Append("  AND R.CREATED_AT = @DateEnv ");
        //    //}
        //    //else 
        //    //{
        //    //    sb.Append("  AND R.CREATED_AT >= @DateEnv AND R.CREATED_AT <= @DATEFIM ");
        //    //}


        //    //sb.Append("  AND I.STATUS = 1 ");
        //    //sb.Append("  AND R.DELETED_AT IS NULL ");
        //    //sb.Append("  AND R.INDICADORID = @INDICADORID ");
        //    //sb.Append("GROUP BY INDICADORID, ");
        //    //sb.Append("         R.TRANSACTIONID, ");
        //    //sb.Append("         S.IDGDA_SECTOR, ");
        //    //sb.Append("         HIG1.METRIC_MIN, ");
        //    //sb.Append("         HIG2.METRIC_MIN, ");
        //    //sb.Append("         HIG3.METRIC_MIN, ");
        //    //sb.Append("         HIG4.METRIC_MIN, ");
        //    //sb.Append("         HIS.GOAL, ");
        //    //sb.Append("         I.WEIGHT, ");
        //    //sb.Append("         HHR.LEVELWEIGHT, ");
        //    //sb.Append("         HIG1.MONETIZATION, ");
        //    //sb.Append("         HIG2.MONETIZATION, ");
        //    //sb.Append("         HIG3.MONETIZATION, ");
        //    //sb.Append("         HIG4.MONETIZATION ");
        //    #endregion

        
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();
        //        try
        //        {
        //            using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
        //            {
        //                using (SqlDataReader reader = command.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        var a = factor.factor0;
        //                        var b = reader["fator0"];

        //                        factor.factor0 = factor.factor0 + double.Parse(reader["fator0"].ToString());
        //                        factor.factor1 = factor.factor1 + double.Parse(reader["fator1"].ToString());

        //                        factor.goal = double.Parse(reader["goal"].ToString());
        //                        factor.min1 = double.Parse(reader["min1"].ToString());
        //                        factor.min2 = double.Parse(reader["min2"].ToString());
        //                        factor.min3 = double.Parse(reader["min3"].ToString());
        //                        factor.min4 = double.Parse(reader["min4"].ToString());
        //                        factor.count = reader["conta"].ToString();
        //                        factor.better = reader["better"].ToString();
        //                        factor.coins = reader["GANHO EM MOEDAS"].ToString();
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //        connection.Close();
        //    }

        //    return factor;
        //}

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

        public List<ModelsEx.homeRel> returnListHierarchy(List<ModelsEx.homeRel> original, string hierarchy)
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
                            // moedasPossiveis = grupo.First().moedasPossiveis,
                            data_atualizacao = grupo.First().data_atualizacao, // Você pode precisar ajustar o formato da data
                            idcollaborator = grupo.Key.matricula_supervisor,
                            name = grupo.Max(s => s.nome_supervisor),
                            cargo = "SUPERVISOR",
                            home_based = grupo.First().home_based,
                            setor = grupo.First().setor_supervisor,
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
                            moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Count(), 2, MidpointRounding.AwayFromZero),
                            //moedasPossiveis = grupo.Max(item => item.moedasPossiveis),
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
                        // moedasPossiveis = grupo.First().moedasPossiveis,
                        data_atualizacao = grupo.First().data_atualizacao, // Você pode precisar ajustar o formato da data
                        idcollaborator = grupo.Key.matricula_coordenador,
                        name = grupo.First().nome_coordenador,
                        cargo = "COORDENADOR",
                        home_based = grupo.First().home_based,
                        setor = grupo.First().setor,
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
                        //moedasPossiveis = grupo.Max(item => item.moedasPossiveis),
                        moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Count(), 2, MidpointRounding.AwayFromZero),
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
                        //moedasPossiveis = grupo.First().moedasPossiveis,
                        data_atualizacao = grupo.First().data_atualizacao, // Você pode precisar ajustar o formato da data
                        idcollaborator = grupo.Key.matricula_gerente_ii,
                        name = grupo.First().nome_gerente_ii,
                        cargo = "GERENTE II",
                        home_based = grupo.First().home_based,
                        setor = grupo.First().setor,
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
                        //moedasPossiveis = grupo.Max(item => item.moedasPossiveis),
                        moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Count(), 2, MidpointRounding.AwayFromZero),
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
                        // moedasPossiveis = grupo.First().moedasPossiveis,
                        data_atualizacao = grupo.First().data_atualizacao, // Você pode precisar ajustar o formato da data
                        idcollaborator = grupo.Key.matricula_gerente_i,
                        name = grupo.First().nome_gerente_i,
                        cargo = "GERENTE I",
                        home_based = grupo.First().home_based,
                        setor = grupo.First().setor,
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
                        //moedasPossiveis = grupo.Max(item => item.moedasPossiveis),
                        moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Count(), 2, MidpointRounding.AwayFromZero),
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
                        //moedasPossiveis = grupo.First().moedasPossiveis,
                        data_atualizacao = grupo.First().data_atualizacao, // Você pode precisar ajustar o formato da data
                        idcollaborator = grupo.Key.matricula_diretor,
                        name = grupo.First().nome_diretor,
                        cargo = "DIRETOR",
                        home_based = grupo.First().home_based,
                        setor = grupo.First().setor,
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
                        //moedasPossiveis = grupo.Max(item => item.moedasPossiveis),
                        moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Count(), 2, MidpointRounding.AwayFromZero),
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

                    //retorno = original
                    //.GroupBy(item => new { item.cod_indicador, item.dateReferer, item.matricula_ceo, item.moedasPossiveis }).Where(d => d.Key.matricula_ceo != "0")
                    //.Select(grupo => new ModelsEx.homeRel
                    //{
                    //    datePay = grupo.First().datePay,
                    //    dateReferer = grupo.First().dateReferer,
                    //    data = grupo.First().data,
                    //    cod_indicador = grupo.First().cod_indicador,
                    //    indicador = grupo.First().indicador,
                    //    cod_gip = grupo.First().cod_gip,
                    //    meta = grupo.First().meta,
                    //    goal = grupo.First().goal,
                    //    indicatorType = grupo.First().indicatorType,
                    //    //moedasPossiveis = grupo.First().moedasPossiveis,
                    //    data_atualizacao = grupo.First().data_atualizacao, // Você pode precisar ajustar o formato da data
                    //    idcollaborator = grupo.Key.matricula_ceo,
                    //    name = grupo.First().nome_ceo,
                    //    cargo = "CEO",
                    //    home_based = grupo.First().home_based,
                    //    setor = grupo.First().setor,
                    //    turno = grupo.First().turno,
                    //    site = grupo.First().site,
                    //    nome_supervisor = "-",
                    //    matricula_supervisor = "-",
                    //    matricula_coordenador = "-",
                    //    nome_coordenador = "-",
                    //    matricula_gerente_ii = "-",
                    //    nome_gerente_ii = "-",
                    //    matricula_gerente_i = "-",
                    //    nome_gerente_i = "-",
                    //    matricula_diretor = "-",
                    //    nome_diretor = "-",
                    //    matricula_ceo = "-",
                    //    nome_ceo = "-",
                    //    fator0 = grupo.Sum(item => item.fator0),
                    //    fator1 = grupo.Sum(item => item.fator1),
                    //    min1 = grupo.First().min1,
                    //    min2 = grupo.First().min2,
                    //    min3 = grupo.First().min3,
                    //    min4 = grupo.First().min4,
                    //    conta = grupo.First().conta,
                    //    better = grupo.First().better,

                    //    diasTrabalhados = grupo.Max(item => item.diasTrabalhados),
                    //    diasEscalados = grupo.Max(item => item.diasEscalados),
                    //    moedasPossiveis = grupo.Max(item => item.moedasPossiveis),
                    //    //moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Count(), 2, MidpointRounding.AwayFromZero),
                    //    moedasGanhas = grupo.Sum(item => item.moedasGanhas),
                    //    qtdPessoas = grupo.Count(item2 => item2.resultado > 0),
                    //    metaSomada = grupo.Sum(item2 => item2.goal),
                    //    qtdPessoasTotal = grupo.Count(),
                    //    resultadoAPI = grupo.Sum(item => item.resultadoAPI),
                    //})
                    //.ToList();

                    //List<string> codIndicatorsDiferentes = retorno.Select(item => item.cod_indicador).Distinct().ToList();
                    //string resultado = string.Join(", ", codIndicatorsDiferentes);

                    //List<ModelsEx.homeRel> retornoteste = new List<ModelsEx.homeRel>();
                    //retornoteste = retorno.FindAll(item => item.cod_indicador == "401").ToList();

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
                        //moedasPossiveis = grupo.First().moedasPossiveis,
                        data_atualizacao = grupo.First().data_atualizacao, // Você pode precisar ajustar o formato da data
                        idcollaborator = grupo.Key.matricula_ceo,
                        name = grupo.First().nome_ceo,
                        cargo = "CEO",
                        home_based = grupo.First().home_based,
                        setor = grupo.First().setor,
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
                        //moedasPossiveis = grupo.Max(item => item.moedasPossiveis),
                        moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Count(), 2, MidpointRounding.AwayFromZero),
                        //moedasPossiveis = Math.Round(grupo.Select(item => item.moedasPossiveis).Distinct().Sum() / grupo.Select(item => item.moedasPossiveis).Distinct().Count(), 2, MidpointRounding.AwayFromZero),
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
            //string CollaboratorId = inputModel.CollaboratorId.ToString();
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
            List<ModelsEx.monetizacaoHierarquia> lMone = new List<ModelsEx.monetizacaoHierarquia>();
            lMone = Funcoes.retornaListaMonetizacaoHierarquia(dtInicial, dtFinal);

            //Realiza a query que retorna todas as informações dos colaboradores que tiveram moneitzação.
            List<ModelsEx.homeRel> rmams = new List<ModelsEx.homeRel>();
            rmams = returnMonetizationDayMonth(dtInicial, dtFinal, sectorsAsString, indicatorsAsString, colaboratorsAsString, hierarchiesAsString, inputModel.Order);



            //Retirando os resultados do supervisor.. Entender com a Tahto como ficara esta parte.
            rmams = rmams.FindAll(item => item.cargo == "AGENTE" || item.cargo == "Não Informado").ToList();

            if (inputModel.Order != "")
            {
                if (inputModel.Order.ToUpper() == "MELHOR")
                {
                    rmams = rmams.OrderBy(r => r.cod_indicador).OrderByDescending(r => r.moedasGanhas).ToList();
                }
                else
                {
                    rmams = rmams.OrderBy(r => r.cod_indicador).OrderBy(r => r.moedasGanhas).ToList();
                }

            }


            ////Verifica perfil administrativo
            //bool adm = Funcoes.retornaPermissao(CollaboratorId);
            //List<string> listaColaboradores = new List<string>();
            //int cargoAtual = 0;
            ////Retorna os ids abaixo para filtrar apenas os abaixos
            //if (adm == true)
            //{
            //    cargoAtual = 8;
            //    listaColaboradores = Funcoes.retornaColaboradoresGeral(dtInicial.ToString(), CollaboratorId);
            //}
            //else
            //{
            //    listaColaboradores = Funcoes.retornaColaboradoresAbaixo(dtInicial.ToString(), CollaboratorId);
            //    cargoAtual = Funcoes.retornaCargoAtual(dtInicial, CollaboratorId);
            //}

            List<ModelsEx.homeRel> supervisores = new List<ModelsEx.homeRel>();
            List<ModelsEx.homeRel> coordenador = new List<ModelsEx.homeRel>();
            List<ModelsEx.homeRel> gerenteii = new List<ModelsEx.homeRel>();
            List<ModelsEx.homeRel> gerentei = new List<ModelsEx.homeRel>();
            List<ModelsEx.homeRel> diretor = new List<ModelsEx.homeRel>();
            List<ModelsEx.homeRel> ceo = new List<ModelsEx.homeRel>();
            List<ModelsEx.homeRel> hierarchies = new List<ModelsEx.homeRel>();

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
                //remover os agentes em caso de não filtrar agentes
                if (hierarchiesAsString.ToString().Contains("1") == false)
                {
                    rmams = new List<ModelsEx.homeRel>();
                }
            }
            else
            {
                //if (cargoAtual >= 2)
                //{
                //    supervisores = returnListHierarchy(rmams, "SUPERVISOR");
                //}
                //if (cargoAtual >= 3)
                //{
                //    coordenador = returnListHierarchy(rmams, "COORDENADOR");
                //}
                //if (cargoAtual >= 4)
                //{
                //    gerenteii = returnListHierarchy(rmams, "GERENTE II");
                //}
                //if (cargoAtual >= 5)
                //{
                //    gerentei = returnListHierarchy(rmams, "GERENTE I");
                //}
                //if (cargoAtual >= 6)
                //{
                //    diretor = returnListHierarchy(rmams, "DIRETOR");
                //}
                //if (cargoAtual >= 7)
                //{
                //    ceo = returnListHierarchy(rmams, "CEO");
                //}

                supervisores = returnListHierarchy(rmams, "SUPERVISOR");
                //List<ModelsEx.homeRel> ASAS = new List<ModelsEx.homeRel>();
                //ASAS = supervisores.FindAll(item => item.idcollaborator == "766162 " && item.cod_indicador == "2").ToList();

                List<ModelsEx.homeRel> ASAS = supervisores.FindAll(it => it.idcollaborator == "789587").ToList();

                coordenador = returnListHierarchy(rmams, "COORDENADOR");
                gerenteii = returnListHierarchy(rmams, "GERENTE II");
                gerentei = returnListHierarchy(rmams, "GERENTE I");
                diretor = returnListHierarchy(rmams, "DIRETOR");
                ceo = returnListHierarchy(rmams, "CEO");

                //List<ModelsEx.homeRel> testeNovo = new List<ModelsEx.homeRel>();
                //testeNovo = supervisores.FindAll(item => item.idcollaborator == "671714" && item.cod_indicador == "665").ToList();
                //testeNovo = rmams.FindAll(item => item.idcollaborator == "671714" && item.cod_indicador == "665").ToList();
                //testeNovo = gerentei.FindAll(item => item.idcollaborator == "671714" && item.cod_indicador == "665").ToList();
                //testeNovo = gerenteii.FindAll(item => item.idcollaborator == "671714" && item.cod_indicador == "665").ToList();
                //testeNovo = diretor.FindAll(item => item.idcollaborator == "671714" && item.cod_indicador == "665").ToList();
                //testeNovo = ceo.FindAll(item => item.idcollaborator == "671714" && item.cod_indicador == "665").ToList();



            }

            if (colaboratorsAsString != "")
            {//765743
                List<string> colaboratorIds = colaboratorsAsString.Split(',').ToList();

                supervisores = supervisores.FindAll(t => colaboratorIds.Any(id => t.idcollaborator.ToString().Contains(id))).ToList();
                coordenador = coordenador.FindAll(t => colaboratorIds.Any(id => t.idcollaborator.ToString().Contains(id))).ToList();
                gerenteii = gerenteii.FindAll(t => colaboratorIds.Any(id => t.idcollaborator.ToString().Contains(id))).ToList();
                gerentei = gerentei.FindAll(t => colaboratorIds.Any(id => t.idcollaborator.ToString().Contains(id))).ToList();
                diretor = diretor.FindAll(t => colaboratorIds.Any(id => t.idcollaborator.ToString().Contains(id))).ToList();
                ceo = ceo.FindAll(t => colaboratorIds.Any(id => t.idcollaborator.ToString().Contains(id))).ToList();

            }

            rmams = rmams.Concat(supervisores).Concat(coordenador).Concat(gerenteii).Concat(gerentei).Concat(diretor).Concat(ceo).ToList();

            List<ModelsEx.homeRel> listaAgentes = rmams
                  .Where(item => item.cargo.ToUpper() == "AGENTE")
                  .ToList();

            List<ModelsEx.homeRel> listaHieraquias = rmams
                   .Where(item => item.cargo.ToUpper() != "AGENTE")
                   .ToList();

            //

            //Parallel.ForEach(listaAgentes, agente => {
            //    factors factor = new factors();
            //    factors factor2 = new factors();

            //    factor2 = returnFactorsById(agente.idgda_collaborators, dtInicial, agente.codIndicador, agente.cod_gip, factor, false, dtFinal);
            //    //Realiza a conta
            //    agente = monetizationClass.doCalculationResult(factor2, agente);

            //});

            //<ModelsEx.homeRel> testeNovo = new List<ModelsEx.homeRel>();
            //testeNovo = rmams.FindAll(item => item.idcollaborator == "671714" && item.cod_indicador == "665").ToList();

            for (int i = 0; i < listaAgentes.Count; i++)
            {

                //if (listaAgentes[i].cod_indicador == "4")
                //{
                //    var parou = true;
                //}


                ModelsEx.homeRel agente = listaAgentes[i];

                if (agente.idcollaborator == "671714" && agente.cod_indicador == "665" && listaHieraquias[i].dateReferer == "2023-11-10")
                {
                    var parou = true;
                }

                //factors factor = new factors();
                //factors factor2 = new factors();
                //Faz a varredura em todos os intervalos de data, pois ele pode pertencer a uma hierarquia e a uma configuração diferente dependendo da data
                //for (DateTime dataAtual = dtTimeInicial; dataAtual <= dtTimeFinal; dataAtual = dataAtual.AddDays(1))
                //{
                //    factor = returnFactorsById(rmam.idgda_collaborators, dataAtual.ToString("yyyy-MM-dd"), rmam.codIndicador, rmam.cod_gip, factor, true, dtFinal);
                //}

                //factor2 = returnFactorsById(agente.idgda_collaborators, dtInicial, agente.codIndicador, agente.cod_gip, factor, false, dtFinal);
                //Realiza a conta
                //agente = monetizationClass.doCalculationResultDay(agente);

                agente = monetizationClass.doCalculationResultHome(agente, false);


                listaAgentes[i] = agente;
            }


            for (int i = 0; i < listaHieraquias.Count; i++)
            {

                //if (listaHieraquias[i].idcollaborator == "765743" && listaHieraquias[i].cod_indicador == "73")
                //{
                //    var parou = true;
                //}

                if (listaHieraquias[i].dateReferer == "15/12/2023 00:00:00")
                {
                    var parou = true;
                }


                ModelsEx.homeRel hierarquia = listaHieraquias[i];

                factors factor = new factors();
                factors factor2 = new factors();
                //Faz a varredura em todos os intervalos de data, pois ele pode pertencer a uma hierarquia e a uma configuração diferente dependendo da data
                //for (DateTime dataAtual = dtTimeInicial; dataAtual <= dtTimeFinal; dataAtual = dataAtual.AddDays(1))
                //{
                //    factor = returnFactorsByIdHierarchy(hierarquia.idgda_collaborators, dataAtual.ToString("yyyy-MM-dd"), hierarquia.codIndicador, hierarquia.cod_gip, factor, true, dtFinal);
                //}

                //factor2 = returnFactorsById(hierarquia.idgda_collaborators, dtInicial, hierarquia.codIndicador, hierarquia.cod_gip, factor, false, dtFinal);
                //Realiza a conta
                hierarquia = monetizationClass.doCalculationResultHome(hierarquia, false);
                //if (hierarquia.cargo != "SUPERVISOR")
                //{
                //    hierarquia.meta = "-";
                //    hierarquia.setor = "-";
                //    hierarquia.cod_gip = "-";
                //}
                int idAg = Convert.ToInt32(hierarquia.idcollaborator);
                int indicadorAg = int.Parse(hierarquia.cod_indicador);
                DateTime dateAg = DateTime.Parse(hierarquia.dateReferer);
                var monEnc2 = lMone.Find(item => item.id == idAg && item.date == dateAg && item.idIndicador == indicadorAg);
                //var monEnc2 = lMone.Find(item => item.id.ToString() == hierarquia.idcollaborator && item.date == DateTime.Parse(hierarquia.dateReferer) && item.idIndicador == int.Parse(hierarquia.cod_indicador));

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



            //For each nos resultados de monetização
            //for (int i = 0; i < rmams.Count; i++)
            //{
            //    RelMonetizationAdmMonth rmam = rmams[i];

            //    factors factor = new factors();
            //    factors factor2 = new factors();
            //    //Faz a varredura em todos os intervalos de data, pois ele pode pertencer a uma hierarquia e a uma configuração diferente dependendo da data
            //    //for (DateTime dataAtual = dtTimeInicial; dataAtual <= dtTimeFinal; dataAtual = dataAtual.AddDays(1))
            //    //{
            //    //    factor = returnFactorsById(rmam.idgda_collaborators, dataAtual.ToString("yyyy-MM-dd"), rmam.codIndicador, rmam.cod_gip, factor, true, dtFinal);
            //    //}

            //    factor2 = returnFactorsById(rmam.idgda_collaborators, dtInicial, rmam.codIndicador, rmam.cod_gip, factor, false, dtFinal);


            //    //Realiza a conta
            //    rmam = monetizationClass.doCalculationResult(factor2, rmam);

            //    rmams[i] = rmam;
            //}

            rmams = listaAgentes.Concat(listaHieraquias).ToList();

            //Filtro Cesta
            List<ModelsEx.homeRel> listaCesta = new List<ModelsEx.homeRel>();
            if (indicatorsCestaAsString != "" || indicatorsAsString == "")
            {
                listaCesta = Funcoes.retornaCestaIndicadores(rmams, cm, true, false, false);
            }
            if (indicatorsAsString != "")
            {
                List<string> valoresLista = indicatorsAsString.Split(',').Select(valor => valor.Trim()).ToList();
                rmams = rmams.FindAll(item => valoresLista.Contains(item.cod_indicador)).ToList();

                //rmams = rmams.FindAll(item => item.cod_indicador.Contains(indicatorsAsString)).ToList();
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
            //Filtro de grupo, após os calculos
            if (groupsAsString != "")
            {
                //Pega infs Grupo
                List<groups> lGroups = returnTables.listGroups("");
                string groupsAsString3 = string.Join(",", lGroups
                .Where(g => groupsAsString.Contains(g.id.ToString()))
                .Select(g => g.alias));

                //Filtra só os grupos especificos
                elementosFiltrados = rmams
                    .Where(item => groupsAsString3.Contains(item.grupo.ToUpper()))
                    .ToList();
            }

            if (inputModel.Order != "")
            {
                //if (inputModel.Order.ToUpper() == "MELHOR")
                //{
                //    elementosFiltrados = elementosFiltrados.OrderByDescending(item => item.porcentual).ToList();
                //    //orderBy = " ORDER BY MAX(MZ.SOMA) DESC ";
                //}
                //else
                //{
                //    elementosFiltrados = elementosFiltrados.OrderBy(item => item.porcentual).ToList();
                //    //orderBy = " ORDER BY MAX(MZ.SOMA) ASC ";
                //}

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

            try
            {

            }
            catch (Exception)
            {

            }


            var tst2 = "0";
            if (double.TryParse(tst2, out double valorNumerico))
            {
                string valorFormatado = valorNumerico.ToString("F2");
            }


            var jsonData = elementosFiltrados.Select(item => new returnResponseDay
            {
                DataPagamento = item.datePay,
                DataReferencia = item.dateReferer,
                Matricula = item.idcollaborator,
                NomeColaborador = item.name,
                Cargo = item.cargo,
                //DiasTrabalhados = item.dias_trabalhados,
                IDIndicador = item.cod_indicador,
                Indicador = item.indicador,
                TipoIndicador = item.indicatorType,
                Meta = item.meta.ToString().Replace(".", ","),
                Resultado = item.resultado.ToString().Replace(".", ","),
                //Resultado = double.TryParse(item.resultado.ToString(), out valorNumerico) ? valorNumerico.ToString("F2") : item.resultado.ToString(),
                //Percentual =  item.porcentual,
                //Percentual = ((item.cargo == "AGENTE" || item.cargo == "SUPERVISOR") && item.vemMeta == 1) || item.cod_indicador == "10000012" ? item.porcentual.ToString() : "-",
                Percentual = (item.vemMeta == 1) || item.cod_indicador == "10000012" ? item.porcentual.ToString() : "-",
                MetaMaximaMoedas = item.moedasPossiveis.ToString(),
                GanhoEmMoedas = item.moedasGanhas,
                //Grupo = item.grupo,
                //Grupo = ((item.cargo == "AGENTE" || item.cargo == "SUPERVISOR") && item.vemMeta == 1) || item.cod_indicador == "10000012" ? item.grupo.ToString() : "-",
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

                //CodigoGIP = item.cod_gip,
                //Setor = item.setor,
                CodigoGIP = (item.cargo == "AGENTE" || item.cargo == "SUPERVISOR") ? item.cod_gip : "-",
                Setor = (item.cargo == "AGENTE" || item.cargo == "SUPERVISOR") ? item.setor : "-",
                Home = item.home_based,
                Turno = item.turno,
                Site = item.site,
                Score = item.score
            }).ToList();

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(jsonData);
        }

        // Método para serializar um DataTable em JSON

    }
}