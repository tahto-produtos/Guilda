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
using Iced.Intel;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class RankingOperacao1Controller : ApiController
    {

        public List<ModelsEx.homeRel> returnHomeRanking(string dtInicial, string dtFinal, string sectors, string indicators, string hierarchies, string ordem, string collaborators)
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

            if (collaborators != "")
            {
                //filter = filter + $" AND CL.IDGDA_COLLABORATORS IN ({collaborators}) ";
                filter = filter + collaborators;
            }

            //if (hierarchies != "")
            //{
            //    filter = filter + $" AND HHR.IDGDA_HIERARCHY IN ({hierarchies}) ";
            //}
            //if (ordem != "")
            //{
            //    if (ordem.ToUpper() == "MELHOR")
            //    {
            //        orderBy = " ORDER BY MAX(MZ.SOMA) DESC ";
            //    }
            //    else
            //    {
            //        orderBy = " ORDER BY MAX(MZ.SOMA) ASC ";
            //    }

            //}

           
            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtInicial);
            stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFinal);
            stb.Append(" ");
            stb.Append("SELECT MONTH(@DATAINICIAL) AS MES, ");
            stb.Append("       YEAR(@DATAINICIAL) AS ANO, ");
            stb.Append("       R.IDGDA_COLLABORATORS AS IDCOLLABORATOR, ");
            stb.Append("       MAX(CB.NAME) AS NAME, ");
            stb.Append("       MAX(CL.CARGO) AS CARGO, ");
            stb.Append("       MAX(R.RESULT) AS RESULTADOAPI, ");
            stb.Append("       MAX(TRAB.TRABALHADO) AS TRABALHADO, ");
            stb.Append("       MAX(ESC.ESCALADO) AS ESCALADO, ");
            stb.Append("       MAX(HIG1.MONETIZATION) AS META_MAXIMA, ");
            stb.Append("	   CASE ");
            stb.Append("           WHEN MAX(HIG1.MONETIZATION) IS NULL THEN 0 ");
            stb.Append("           WHEN MAX(MZ.SOMA) IS NULL THEN 0 ");
            stb.Append("           ELSE MAX(MZ.SOMA) ");
            stb.Append("       END AS MOEDA_GANHA, ");
            stb.Append("	   MAX(CONVERT(DATE, R.CREATED_AT)) AS 'DATA', ");
            stb.Append("	   MAX(R.FACTORS) AS 'FATOR', ");
            stb.Append("       R.INDICADORID AS 'COD INDICADOR', ");
            stb.Append("       MAX(I.NAME) AS 'INDICADOR', ");
            stb.Append("       CASE ");
            stb.Append("          WHEN MAX(I.TYPE) IS NULL THEN '' ");
            stb.Append("           ELSE MAX(I.TYPE) ");
            stb.Append("       END AS 'TYPE', ");
            stb.Append("       MAX(HIS.GOAL) AS META, ");
            stb.Append("       MAX(SC.WEIGHT_SCORE) AS SCORE, ");
            stb.Append("       '' AS RESULTADO, ");
            stb.Append("       '' AS PORCENTUAL, ");
            stb.Append("       '' AS GRUPO, ");
            stb.Append("       GETDATE() AS 'Data de atualização', ");
            stb.Append("       MAX(SEC.IDGDA_SECTOR) AS COD_GIP, ");
            stb.Append("       MAX(SEC.NAME) AS SETOR, ");
            stb.Append("       MAX(CL.IDGDA_SECTOR_SUPERVISOR) AS COD_GIP_SUPERVISOR, ");
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
            stb.Append("       MAX(CL.NOME_CEO) AS 'NOME CEO', ");
            //stb.Append("       SUM(F1.FACTOR) AS FATOR0, ");
            //stb.Append("       SUM(F2.FACTOR) AS FATOR1, ");
            stb.Append("	   MAX(HIS.GOAL) AS GOAL, ");
            stb.Append("	   MAX(I.WEIGHT) AS 'WEIGHT', ");
            stb.Append("       MAX(HIG1.MONETIZATION) AS COIN1, ");
            stb.Append("       MAX(HIG2.MONETIZATION) AS COIN2, ");
            stb.Append("       MAX(HIG3.MONETIZATION) AS COIN3, ");
            stb.Append("       MAX(HIG4.MONETIZATION) AS COIN4, ");
            stb.Append("	   MAX(SEC.IDGDA_SECTOR) AS IDGDA_SECTOR, ");
            stb.Append("       MAX(HIG1.METRIC_MIN) AS MIN1, ");
            stb.Append("       MAX(HIG2.METRIC_MIN) AS MIN2, ");
            stb.Append("       MAX(HIG3.METRIC_MIN) AS MIN3, ");
            stb.Append("       MAX(HIG4.METRIC_MIN) AS MIN4, ");
            stb.Append("       CASE ");
            stb.Append("           WHEN MAX(ME.EXPRESSION) IS NULL THEN '(#FATOR1/#FATOR0)' ");
            stb.Append("           ELSE MAX(ME.EXPRESSION) ");
            stb.Append("       END AS CONTA, ");
            stb.Append("       CASE ");
            stb.Append("           WHEN MAX(I.CALCULATION_TYPE) IS NULL THEN 'BIGGER_BETTER' ");
            stb.Append("           ELSE MAX(I.CALCULATION_TYPE) ");
            stb.Append("       END AS BETTER ");
            stb.Append("FROM GDA_RESULT (NOLOCK) AS R ");
            stb.Append("LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS CB ON CB.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
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
            //stb.Append("   LEFT JOIN GDA_COLLABORATORS_LAST_DETAILS (NOLOCK) AS CL2 ON CL2.IDGDA_COLLABORATORS = C.IDGDA_COLLABORATORS) AS CL ON CL.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            //stb.Append("AND CL.CREATED_AT = R.CREATED_AT ");

            stb.Append(" ");
            stb.Append("LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CL ON CL.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS AND CL.CREATED_AT = R.CREATED_AT ");
            stb.Append(" ");
            stb.Append("LEFT JOIN ");
            stb.Append("  (SELECT SUM(INPUT) AS SOMA, ");
            //stb.Append("          IDGDA_SECTOR, ");
            stb.Append("          GDA_INDICATOR_IDGDA_INDICATOR, ");
            stb.Append("          RESULT_DATE, ");
            stb.Append("          COLLABORATOR_ID ");
            stb.Append("   FROM GDA_CHECKING_ACCOUNT ");
            stb.Append("   WHERE RESULT_DATE >= @DATAINICIAL ");
            stb.Append("     AND RESULT_DATE <= @DATAFINAL ");
            //stb.Append("   GROUP BY IDGDA_SECTOR, ");
            stb.Append("   GROUP BY  ");
            stb.Append("            GDA_INDICATOR_IDGDA_INDICATOR, ");
            stb.Append("            RESULT_DATE, ");
            stb.Append("            COLLABORATOR_ID) AS MZ ON MZ.GDA_INDICATOR_IDGDA_INDICATOR = R.INDICADORID ");
            stb.Append("AND MZ.RESULT_DATE = R.CREATED_AT ");
            stb.Append("AND MZ.COLLABORATOR_ID = R.IDGDA_COLLABORATORS ");
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
            stb.Append("LEFT JOIN GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME ON HME.INDICATORID = R.INDICADORID ");
            stb.Append("AND HME.deleted_at IS NULL ");
            stb.Append("LEFT JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON ME.ID = HME.MATHEMATICALEXPRESSIONID ");
            stb.Append("AND ME.DELETED_AT IS NULL ");
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) AS HIS ON HIS.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND HIS.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.Append("AND CONVERT(DATE,HIS.STARTED_AT) <= R.CREATED_AT ");
            stb.Append("AND CONVERT(DATE,HIS.ENDED_AT) >= R.CREATED_AT ");
            stb.Append("AND HIS.DELETED_AT IS NULL ");
            stb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS I ON I.IDGDA_INDICATOR = R.INDICADORID ");
            stb.Append("LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = CL.IDGDA_SECTOR ");
            stb.Append("LEFT JOIN GDA_SECTOR (NOLOCK) AS SECSUP ON SECSUP.IDGDA_SECTOR = CL.IDGDA_SECTOR_SUPERVISOR ");
            //FK
            stb.Append("LEFT JOIN GDA_HISTORY_SCORE_INDICATOR_SECTOR (NOLOCK) AS SC ON ");
            stb.Append("SC.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND SC.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.Append("AND CONVERT(DATE,SC.STARTED_AT) <= R.CREATED_AT ");
            stb.Append("AND CONVERT(DATE,SC.ENDED_AT) >= R.CREATED_AT ");

            //stb.Append("INNER JOIN GDA_FACTOR (NOLOCK) AS F1 ON F1.IDGDA_RESULT = R.IDGDA_RESULT ");
            //stb.Append("AND F1.[INDEX] = 1 ");
            //stb.Append("INNER JOIN GDA_FACTOR (NOLOCK) AS F2 ON F2.IDGDA_RESULT = R.IDGDA_RESULT ");
            //stb.Append("AND F2.[INDEX] = 2 ");

            stb.AppendFormat(" WHERE 1 = 1 {0} ", filter);
            stb.Append("  AND R.DELETED_AT IS NULL ");
            stb.Append("  AND CL.IDGDA_SECTOR IS NOT NULL ");
            stb.Append("  AND CL.HOME_BASED <> '' ");
            stb.Append("  AND CL.CARGO IS NOT NULL ");
            //stb.Append("  AND CL.ACTIVE = 'true' ");
            stb.Append("  AND R.CREATED_AT >= @DATAINICIAL ");
            stb.Append("  AND R.CREATED_AT <= @DATAFINAL ");
            stb.Append("  AND I.STATUS = 1 ");
            stb.Append("GROUP BY R.IDGDA_COLLABORATORS, ");
            stb.Append("         CONVERT(DATE, R.CREATED_AT), ");
            stb.Append("         INDICADORID ");
            #region Antigo
            //stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtInicial);
            //stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFinal);
            //stb.Append(" ");
            //stb.Append("SELECT MONTH(@DATAINICIAL) AS MES, ");
            //stb.Append("       YEAR(@DATAINICIAL) AS ANO, ");
            //stb.Append("       MAX(CB.IDGDA_COLLABORATORS) AS IDCOLLABORATOR, ");
            //stb.Append("       MAX(CB.NAME) AS NAME, ");
            //stb.Append("       MAX(LEVELNAME) AS CARGO, ");
            //stb.Append(" ");
            //stb.Append("			MAX(R.RESULT) AS RESULTADOAPI, ");
            //stb.Append("			MAX(TRAB.TRABALHADO) AS TRABALHADO, ");
            //stb.Append("			MAX(ESC.ESCALADO) AS ESCALADO, ");
            //stb.Append("			MAX(Bk.MONETIZATION_G1) AS META_MAXIMA, ");
            //stb.Append("			MAX(CA.SOMA) AS MOEDA_GANHA, ");
            //stb.Append(" ");
            //stb.Append("       CONVERT(DATE, R.CREATED_AT) AS 'DATA', ");
            //stb.Append("       MAX(R.INDICADORID) AS 'COD INDICADOR', ");
            //stb.Append("       MAX(I.NAME) AS 'INDICADOR', ");
            //stb.Append("       CASE ");
            //stb.Append("           WHEN MAX(I.TYPE) IS NULL THEN '' ");
            //stb.Append("           ELSE MAX(I.TYPE) ");
            //stb.Append("       END AS 'TYPE', ");
            //stb.Append("       MAX(HIS.GOAL) AS META, ");
            //stb.Append("       '' AS RESULTADO, ");
            //stb.Append("       '' AS PORCENTUAL, ");
            //stb.Append("       '' AS GRUPO, ");
            //stb.Append("       GETDATE() AS 'Data de atualização', ");
            //stb.Append("       MAX(S.IDGDA_SECTOR) AS COD_GIP, ");
            //stb.Append("       MAX(SEC.NAME) AS SETOR, ");
            //stb.Append("       MAX(CL.HOME_BASED) AS HOME_BASED,  ");
            //stb.Append("       MAX(CL.SITE) AS SITE,  ");
            //stb.Append("       MAX(CL.PERIODO) AS TURNO,  ");
            //stb.Append("       MAX(CL.MATRICULA_SUPERVISOR) AS 'MATRICULA SUPERVISOR',  ");
            //stb.Append("       MAX(CL.NOME_SUPERVISOR) AS 'NOME SUPERVISOR',  ");
            //stb.Append("       MAX(CL.MATRICULA_COORDENADOR) AS 'MATRICULA COORDENADOR',  ");
            //stb.Append("       MAX(CL.NOME_COORDENADOR) AS 'NOME COORDENADOR',  ");
            //stb.Append("       MAX(CL.MATRICULA_GERENTE_II) AS 'MATRICULA GERENTE II',  ");
            //stb.Append("       MAX(CL.NOME_GERENTE_II) AS 'NOME GERENTE II',  ");
            //stb.Append("       MAX(CL.MATRICULA_GERENTE_I) AS 'MATRICULA GERENTE I',  ");
            //stb.Append("       MAX(CL.NOME_GERENTE_I) AS 'NOME GERENTE I',  ");
            //stb.Append("       MAX(CL.MATRICULA_DIRETOR) AS 'MATRICULA DIRETOR',  ");
            //stb.Append("       MAX(CL.NOME_DIRETOR) AS 'NOME DIRETOR',  ");
            //stb.Append("       MAX(CL.MATRICULA_CEO) AS 'MATRICULA CEO',  ");
            //stb.Append("       MAX(CL.NOME_CEO) AS 'NOME CEO', ");
            //stb.Append("       SUM(F1.FACTOR) AS FATOR0, ");
            //stb.Append("       SUM(F2.FACTOR) AS FATOR1, ");
            //stb.Append("       HIS.GOAL, ");
            //stb.Append("       I.WEIGHT AS WEIGHT, ");
            //stb.Append("       HHR.LEVELWEIGHT AS HIERARCHYLEVEL, ");
            //stb.Append("       HIG1.MONETIZATION AS COIN1, ");
            //stb.Append("       HIG2.MONETIZATION AS COIN2, ");
            //stb.Append("       HIG3.MONETIZATION AS COIN3, ");
            //stb.Append("       HIG4.MONETIZATION AS COIN4, ");
            //stb.Append("       S.IDGDA_SECTOR, ");
            //stb.Append("       HIG1.METRIC_MIN AS MIN1, ");
            //stb.Append("       HIG2.METRIC_MIN AS MIN2, ");
            //stb.Append("       HIG3.METRIC_MIN AS MIN3, ");
            //stb.Append("       HIG4.METRIC_MIN AS MIN4, ");
            //stb.Append("       CASE ");
            //stb.Append("           WHEN MAX(ME.EXPRESSION) IS NULL THEN '(#FATOR1/#FATOR0)' ");
            //stb.Append("           ELSE MAX(ME.EXPRESSION) ");
            //stb.Append("       END AS CONTA, ");
            //stb.Append("       CASE ");
            //stb.Append("           WHEN MAX(I.CALCULATION_TYPE) IS NULL THEN 'BIGGER_BETTER' ");
            //stb.Append("           ELSE MAX(I.CALCULATION_TYPE) ");
            //stb.Append("       END AS BETTER ");
            //stb.Append("FROM GDA_RESULT (NOLOCK) AS R ");
            //stb.Append("INNER JOIN GDA_COLLABORATORS (NOLOCK) AS CB ON CB.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            //stb.Append("INNER JOIN GDA_HISTORY_COLLABORATOR_SECTOR (NOLOCK) AS S ON S.CREATED_AT = R.CREATED_AT ");
            //stb.Append("AND S.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            //stb.Append(" ");
            //stb.Append("LEFT JOIN (SELECT COUNT(0) AS 'ESCALADO', IDGDA_COLLABORATORS, CREATED_AT FROM GDA_RESULT (NOLOCK)   ");
            //stb.Append("WHERE INDICADORID = -1   ");
            //stb.Append("AND CREATED_AT >= @DATAINICIAL AND CREATED_AT <= @DATAFINAL ");
            //stb.Append("AND RESULT = 1 AND DELETED_AT IS NULL  ");
            //stb.Append("GROUP BY IDGDA_COLLABORATORS, CREATED_AT) AS ESC ON ESC.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS AND ESC.CREATED_AT = R.CREATED_AT ");
            //stb.Append("LEFT JOIN (SELECT COUNT(0) AS 'TRABALHADO', IDGDA_COLLABORATORS, CREATED_AT FROM GDA_RESULT (NOLOCK)   ");
            //stb.Append("WHERE INDICADORID = 2   ");
            //stb.Append("AND CREATED_AT >= @DATAINICIAL AND CREATED_AT <= @DATAFINAL ");
            //stb.Append("AND RESULT = 0 AND DELETED_AT IS NULL  ");
            //stb.Append("GROUP BY IDGDA_COLLABORATORS, CREATED_AT) AS TRAB ON TRAB.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS AND TRAB.CREATED_AT = R.CREATED_AT ");
            //stb.Append("LEFT JOIN GDA_BASKET_INDICATOR (NOLOCK) AS BK ON BK.DATE = R.CREATED_AT AND Bk.INDICATOR_ID = R.INDICADORID AND BK.SECTOR_ID = S.IDGDA_SECTOR ");
            //stb.Append("LEFT JOIN  (SELECT collaborator_id,  ");
            //stb.Append("	          GDA_INDICATOR_IDGDA_INDICATOR,  ");
            //stb.Append("			  RESULT_DATE, ");
            //stb.Append("	          SUM(INPUT) AS SOMA  ");
            //stb.Append("	   FROM GDA_CHECKING_ACCOUNT (NOLOCK)  ");
            //stb.Append("	   WHERE RESULT_DATE >= @DATAINICIAL  ");
            //stb.Append("	     AND RESULT_DATE <= @DATAFINAL  ");
            //stb.Append("	   GROUP BY collaborator_id,  ");
            //stb.Append("	            GDA_INDICATOR_IDGDA_INDICATOR, RESULT_DATE) AS CA ON CA.GDA_INDICATOR_IDGDA_INDICATOR = R.INDICADORID  ");
            //stb.Append("	AND CA.collaborator_id = R.IDGDA_COLLABORATORS  AND CA.RESULT_DATE = R.CREATED_AT ");
            //stb.Append(" ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
            //stb.Append("AND HIG1.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIG1.SECTOR_ID = S.IDGDA_SECTOR ");
            //stb.Append("AND HIG1.GROUPID = 1 ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG2 ON HIG2.DELETED_AT IS NULL ");
            //stb.Append("AND HIG2.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIG2.SECTOR_ID = S.IDGDA_SECTOR ");
            //stb.Append("AND HIG2.GROUPID = 2 ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG3 ON HIG3.DELETED_AT IS NULL ");
            //stb.Append("AND HIG3.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIG3.SECTOR_ID = S.IDGDA_SECTOR ");
            //stb.Append("AND HIG3.GROUPID = 3 ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG4 ON HIG4.DELETED_AT IS NULL ");
            //stb.Append("AND HIG4.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIG4.SECTOR_ID = S.IDGDA_SECTOR ");
            //stb.Append("AND HIG4.GROUPID = 4 ");
            //stb.Append("LEFT JOIN GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME ON HME.INDICATORID = R.INDICADORID  AND HME.deleted_at IS NULL  ");
            //stb.Append("LEFT JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON ME.ID = HME.MATHEMATICALEXPRESSIONID AND ME.DELETED_AT IS NULL  ");
            //stb.Append("LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS HHR ON HHR.idgda_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            //stb.Append("AND HHR.DATE = @DATAINICIAL ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) AS HIS ON HIS.INDICATOR_ID = R.INDICADORID  ");
            //stb.Append("AND HIS.SECTOR_ID = S.IDGDA_SECTOR AND HIS.DELETED_AT IS NULL  ");
            //stb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS I ON I.IDGDA_INDICATOR = R.INDICADORID ");
            //stb.Append("LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = S.IDGDA_SECTOR ");
            //stb.Append("INNER JOIN GDA_FACTOR (NOLOCK) AS F1 ON F1.IDGDA_RESULT = R.IDGDA_RESULT ");
            //stb.Append("AND F1.[INDEX] = 1 ");
            //stb.Append("INNER JOIN GDA_FACTOR (NOLOCK) AS F2 ON F2.IDGDA_RESULT = R.IDGDA_RESULT ");
            //stb.Append("AND F2.[INDEX] = 2 ");
            //stb.Append("AND R.CREATED_AT >= @DATAINICIAL ");
            //stb.Append("AND R.CREATED_AT <= @DATAFINAL ");
            //stb.Append("AND I.STATUS = 1 ");
            //stb.Append("AND R.DELETED_AT IS NULL ");
            //stb.Append("LEFT JOIN GDA_COLLABORATORS_LAST_DETAILS (nolock) CL ON CL.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS ");
            //stb.AppendFormat(" WHERE 1 = 1 {0} ", filter);
            //stb.Append("GROUP BY R.IDGDA_COLLABORATORS, ");
            //stb.Append("         R.CREATED_AT, ");
            //stb.Append("         INDICADORID, ");
            //stb.Append("         S.IDGDA_SECTOR, ");
            //stb.Append("         HIG1.METRIC_MIN, ");
            //stb.Append("         HIG2.METRIC_MIN, ");
            //stb.Append("         HIG3.METRIC_MIN, ");
            //stb.Append("         HIG4.METRIC_MIN, ");
            //stb.Append("         HIS.GOAL, ");
            //stb.Append("         I.WEIGHT, ");
            //stb.Append("         HHR.LEVELWEIGHT, ");
            //stb.Append("         HIG1.MONETIZATION, ");
            //stb.Append("         HIG2.MONETIZATION, ");
            //stb.Append("         HIG3.MONETIZATION, ");
            //stb.Append("         HIG4.MONETIZATION ");
            //stb.Append("ORDER BY IDCOLLABORATOR, ");
            //stb.Append("         DATA ");
            #endregion

            List<ModelsEx.homeRel> rmams = new List<ModelsEx.homeRel>();

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                {
                    command.CommandTimeout = 60;
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

                            rmam.mes = reader["MES"].ToString();
                            rmam.ano = reader["ANO"].ToString();
                            rmam.idcollaborator = reader["IDCOLLABORATOR"].ToString();


                            rmam.indicatorType = reader["TYPE"].ToString();
                            rmam.name = reader["NAME"].ToString();
                            rmam.cargo = reader["CARGO"].ToString();
                            rmam.data = reader["DATA"].ToString();
                            rmam.cod_indicador = reader["COD INDICADOR"].ToString();
                            rmam.indicador = reader["INDICADOR"].ToString();
                            rmam.meta = reader["META"].ToString();
                            rmam.data_atualizacao = reader["Data de atualização"].ToString();
                            rmam.cod_gip = reader["COD_GIP"].ToString();
                            rmam.setor = reader["SETOR"].ToString();
                            rmam.home_based = reader["HOME_BASED"].ToString();
                            rmam.site = reader["SITE"].ToString();
                            rmam.turno = reader["TURNO"].ToString();
                            rmam.matricula_supervisor = reader["MATRICULA SUPERVISOR"].ToString();
                            rmam.nome_supervisor = reader["NOME SUPERVISOR"].ToString();
                            rmam.matricula_coordenador = reader["MATRICULA COORDENADOR"].ToString();
                            rmam.nome_coordenador = reader["NOME COORDENADOR"].ToString();
                            rmam.matricula_gerente_ii = reader["MATRICULA GERENTE II"].ToString();
                            rmam.nome_gerente_ii = reader["NOME GERENTE II"].ToString();
                            rmam.matricula_gerente_i = reader["MATRICULA GERENTE I"].ToString();
                            rmam.nome_gerente_i = reader["NOME GERENTE I"].ToString();
                            rmam.matricula_diretor = reader["MATRICULA DIRETOR"].ToString();
                            rmam.nome_diretor = reader["NOME DIRETOR"].ToString();
                            rmam.matricula_ceo = reader["MATRICULA CEO"].ToString();
                            rmam.nome_ceo = reader["NOME CEO"].ToString();
                            rmam.fator0 = double.Parse(factor0);
                            rmam.fator1 = double.Parse(factor1);

                            if (reader["GOAL"].ToString() != "")
                            {
                                rmam.goal = double.Parse(reader["GOAL"].ToString());
                            }
                            else
                            {
                                rmam.goal = 0;
                            }
                            //rmam.goal = double.Parse(reader["GOAL"].ToString());
                            rmam.weight = reader["WEIGHT"].ToString();
                            //rmam.hierarchylevel = reader["HIERARCHYLEVEL"].ToString();
                            rmam.idgda_sector = reader["IDGDA_SECTOR"].ToString();
                            rmam.conta = reader["CONTA"].ToString();
                            rmam.better = reader["BETTER"].ToString();
                            if (reader["MIN1"].ToString() != "")
                            {
                                rmam.min1 = double.Parse(reader["MIN1"].ToString());
                            }
                            else
                            {
                                rmam.min1 = 0;
                            }

                            if (reader["MIN2"].ToString() != "")
                            {
                                rmam.min2 = double.Parse(reader["MIN2"].ToString());
                            }
                            else
                            {
                                rmam.min2 = 0;
                            }

                            if (reader["MIN3"].ToString() != "")
                            {
                                rmam.min3 = double.Parse(reader["MIN3"].ToString());
                            }
                            else
                            {
                                rmam.min3 = 0;
                            }

                            if (reader["MIN4"].ToString() != "")
                            {
                                rmam.min4 = double.Parse(reader["MIN4"].ToString());
                            }
                            else
                            {
                                rmam.min4 = 0;
                            }
                            //rmam.coin1 = reader["COIN1"].ToString();
                            //rmam.coin2 = reader["COIN2"].ToString();
                            //rmam.coin3 = reader["COIN3"].ToString();
                            //rmam.coin4 = reader["COIN4"].ToString();
                            if (reader["COIN1"].ToString() != "")
                            {
                                rmam.coin1 = reader["COIN1"].ToString();
                            }
                            else
                            {
                                rmam.coin1 = "0";
                            }

                            if (reader["COIN2"].ToString() != "")
                            {
                                rmam.coin2 = reader["COIN2"].ToString();
                            }
                            else
                            {
                                rmam.coin2 = "0";
                            }

                            if (reader["COIN3"].ToString() != "")
                            {
                                rmam.coin3 = reader["COIN3"].ToString();
                            }
                            else
                            {
                                rmam.coin3 = "0";
                            }

                            if (reader["COIN4"].ToString() != "")
                            {
                                rmam.coin4 = reader["COIN4"].ToString();
                            }
                            else
                            {
                                rmam.coin4 = "0";
                            }

                            if (reader["RESULTADOAPI"].ToString() != "")
                            {
                                if (double.Parse(reader["RESULTADOAPI"].ToString()) != 100 && double.Parse(reader["RESULTADOAPI"].ToString()) != 1)
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

                            rmam.cod_gip_supervisor = reader["COD_GIP_SUPERVISOR"].ToString();
                            rmam.setor_supervisor = reader["SETOR_SUPERVISOR"].ToString();

                            if (reader["SCORE"].ToString() != "")
                            {
                                rmam.score = double.Parse(reader["SCORE"].ToString());
                            }
                            else
                            {
                                rmam.score = 0;
                            }

                            rmams.Add(rmam);

                        }
                    }
                }
                connection.Close();
            }

            return rmams;
        }

        public class returnResponseRanking
        {
            public string Data { get; set; }
            public string NomeAgente { get; set; }
            public string MatriculaDoColaborador { get; set; }
            public string CodigoGIP { get; set; }
            public string Setor { get; set; }
            public string CodigoIndicador { get; set; }
            public string NomeIndicador { get; set; }
            public string TipoIndicador { get; set; }
            public double Resultado { get; set; }
            public string Meta { get; set; }
            public double PercentualDeAtingimento { get; set; }
            public string Grupo { get; set; }
            public string Cargo { get; set; }
            public string MatriculaSupervisor { get; set; }
            public string NomeSupervisor { get; set; }
            public bool Reincidencia { get; set; }
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

        #region Input
        public class InputModel
        {
            public List<Sector> Sectors { get; set; }
            public List<Group> Groups { get; set; }
            public List<Indicator> Indicators { get; set; }
            public List<Hierarchy> Hierarchies { get; set; }
            public string Type { get; set; }
            public string Order { get; set; }
            public DateTime DataInicial { get; set; }
            public DateTime DataFinal { get; set; }
            public int CollaboratorId { get; set; }
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
                        .GroupBy(item => new { item.cod_indicador, item.data, item.matricula_supervisor }).Where(d => d.Key.matricula_supervisor != "0")
                        .Select(grupo => new ModelsEx.homeRel
                        {
                            mes = grupo.First().mes,
                            ano = grupo.First().ano,
                            indicador = grupo.First().indicador,
                            meta = grupo.First().meta,
                            data_atualizacao = grupo.First().data_atualizacao,
                            cod_gip = grupo.First().cod_gip_supervisor,
                            setor = grupo.First().setor_supervisor,
                            indicatorType = grupo.First().indicatorType,
                            home_based = grupo.First().home_based,
                            site = grupo.First().site,
                            turno = grupo.First().turno,


                            goal = grupo.First().goal,
                            weight = grupo.First().weight,
                            hierarchylevel = grupo.First().hierarchylevel,
                            coin1 = grupo.First().coin1,
                            coin2 = grupo.First().coin2,
                            coin3 = grupo.First().coin3,
                            coin4 = grupo.First().coin4,
                            idgda_sector = grupo.First().idgda_sector,
                            min1 = grupo.First().min1,
                            min2 = grupo.First().min2,
                            min3 = grupo.First().min3,
                            min4 = grupo.First().min4,
                            conta = grupo.First().conta,
                            better = grupo.First().better,

                            idcollaborator = grupo.Key.matricula_supervisor,
                            name = grupo.First().nome_supervisor,
                            cargo = "SUPERVISOR",
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
                            score = grupo.Max(item => item.score),
                            cod_indicador = grupo.Key.cod_indicador,
                            data = grupo.Key.data,

                            fator0 = grupo.Sum(item => item.fator0),
                            fator1 = grupo.Sum(item => item.fator1),

                            diasTrabalhados = grupo.Max(item => item.diasTrabalhados),
                            diasEscalados = grupo.Max(item => item.diasEscalados),
                            moedasPossiveis = grupo.Count(item => Convert.ToInt32(item.coin1) > 0) > 0 ? Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Count(item => Convert.ToInt32(item.coin1) > 0), 2, MidpointRounding.AwayFromZero) : 0,
                            moedasGanhas = grupo.Sum(item => item.moedasGanhas),
                            qtdPessoas = grupo.Count(item2 => item2.resultado > 0),
                            metaSomada = grupo.Sum(item2 => item2.goal),
                            qtdPessoasTotal = grupo.Count(),
                            resultadoAPI = grupo.Sum(item => item.resultadoAPI),
                            vemMeta = grupo.Max(item => item.vemMeta),
                        })
                        .ToList();
                }
                else if (hierarchy == "COORDENADOR")
                {
                    retorno = original
                    .GroupBy(item => new { item.cod_indicador, item.data, item.matricula_coordenador }).Where(d => d.Key.matricula_coordenador != "0")
                    .Select(grupo => new ModelsEx.homeRel
                    {
                        mes = grupo.First().mes,
                        ano = grupo.First().ano,



                        indicador = grupo.First().indicador,
                        meta = grupo.First().meta,
                        data_atualizacao = grupo.First().data_atualizacao,
                        cod_gip = grupo.First().cod_gip,
                        setor = grupo.First().setor,
                        home_based = grupo.First().home_based,
                        site = grupo.First().site,
                        turno = grupo.First().turno,
                        indicatorType = grupo.First().indicatorType,
                        score = grupo.Max(item => item.score),
                        goal = grupo.First().goal,
                        weight = grupo.First().weight,
                        hierarchylevel = grupo.First().hierarchylevel,
                        coin1 = grupo.First().coin1,
                        coin2 = grupo.First().coin2,
                        coin3 = grupo.First().coin3,
                        coin4 = grupo.First().coin4,
                        idgda_sector = grupo.First().idgda_sector,
                        min1 = grupo.First().min1,
                        min2 = grupo.First().min2,
                        min3 = grupo.First().min3,
                        min4 = grupo.First().min4,
                        conta = grupo.First().conta,
                        better = grupo.First().better,

                        idcollaborator = grupo.Key.matricula_coordenador,
                        name = grupo.First().nome_coordenador,
                        cargo = "COORDENADOR",
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

                        cod_indicador = grupo.Key.cod_indicador,
                        data = grupo.Key.data,

                        fator0 = grupo.Sum(item => item.fator0),
                        fator1 = grupo.Sum(item => item.fator1),

                        diasTrabalhados = grupo.Max(item => item.diasTrabalhados),
                        diasEscalados = grupo.Max(item => item.diasEscalados),
                        moedasPossiveis = grupo.Count(item => Convert.ToInt32(item.coin1) > 0) > 0 ? Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Count(item => Convert.ToInt32(item.coin1) > 0), 2, MidpointRounding.AwayFromZero) : 0,
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
                    .GroupBy(item => new { item.cod_indicador, item.data, item.matricula_gerente_ii }).Where(d => d.Key.matricula_gerente_ii != "0")
                    .Select(grupo => new ModelsEx.homeRel
                    {
                        mes = grupo.First().mes,
                        ano = grupo.First().ano,



                        indicador = grupo.First().indicador,
                        meta = grupo.First().meta,
                        data_atualizacao = grupo.First().data_atualizacao,
                        cod_gip = grupo.First().cod_gip,
                        setor = grupo.First().setor,
                        home_based = grupo.First().home_based,
                        site = grupo.First().site,
                        turno = grupo.First().turno,
                        indicatorType = grupo.First().indicatorType,

                        goal = grupo.First().goal,
                        weight = grupo.First().weight,
                        hierarchylevel = grupo.First().hierarchylevel,
                        coin1 = grupo.First().coin1,
                        coin2 = grupo.First().coin2,
                        coin3 = grupo.First().coin3,
                        coin4 = grupo.First().coin4,
                        idgda_sector = grupo.First().idgda_sector,
                        min1 = grupo.First().min1,
                        min2 = grupo.First().min2,
                        min3 = grupo.First().min3,
                        min4 = grupo.First().min4,
                        conta = grupo.First().conta,
                        better = grupo.First().better,

                        idcollaborator = grupo.Key.matricula_gerente_ii,
                        name = grupo.First().nome_gerente_ii,
                        cargo = "GERENTE II",
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
                        score = grupo.Max(item => item.score),
                        cod_indicador = grupo.Key.cod_indicador,
                        data = grupo.Key.data,

                        fator0 = grupo.Sum(item => item.fator0),
                        fator1 = grupo.Sum(item => item.fator1),

                        diasTrabalhados = grupo.Max(item => item.diasTrabalhados),
                        diasEscalados = grupo.Max(item => item.diasEscalados),
                        moedasPossiveis = grupo.Count(item => Convert.ToInt32(item.coin1) > 0) > 0 ? Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Count(item => Convert.ToInt32(item.coin1) > 0), 2, MidpointRounding.AwayFromZero) : 0,
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
                    .GroupBy(item => new { item.cod_indicador, item.data, item.matricula_gerente_i }).Where(d => d.Key.matricula_gerente_i != "0")
                    .Select(grupo => new ModelsEx.homeRel
                    {
                        mes = grupo.First().mes,
                        ano = grupo.First().ano,



                        indicador = grupo.First().indicador,
                        meta = grupo.First().meta,
                        data_atualizacao = grupo.First().data_atualizacao,
                        cod_gip = grupo.First().cod_gip,
                        setor = grupo.First().setor,
                        home_based = grupo.First().home_based,
                        site = grupo.First().site,
                        turno = grupo.First().turno,
                        indicatorType = grupo.First().indicatorType,

                        goal = grupo.First().goal,
                        weight = grupo.First().weight,
                        hierarchylevel = grupo.First().hierarchylevel,
                        coin1 = grupo.First().coin1,
                        coin2 = grupo.First().coin2,
                        coin3 = grupo.First().coin3,
                        coin4 = grupo.First().coin4,
                        idgda_sector = grupo.First().idgda_sector,
                        min1 = grupo.First().min1,
                        min2 = grupo.First().min2,
                        min3 = grupo.First().min3,
                        min4 = grupo.First().min4,
                        conta = grupo.First().conta,
                        better = grupo.First().better,

                        idcollaborator = grupo.Key.matricula_gerente_i,
                        name = grupo.First().nome_gerente_i,
                        cargo = "GERENTE I",
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
                        score = grupo.Max(item => item.score),
                        cod_indicador = grupo.Key.cod_indicador,
                        data = grupo.Key.data,

                        fator0 = grupo.Sum(item => item.fator0),
                        fator1 = grupo.Sum(item => item.fator1),

                        diasTrabalhados = grupo.Max(item => item.diasTrabalhados),
                        diasEscalados = grupo.Max(item => item.diasEscalados),
                        moedasPossiveis = grupo.Count(item => Convert.ToInt32(item.coin1) > 0) > 0 ? Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Count(item => Convert.ToInt32(item.coin1) > 0), 2, MidpointRounding.AwayFromZero) : 0,
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
                    .GroupBy(item => new { item.cod_indicador, item.data, item.matricula_diretor }).Where(d => d.Key.matricula_diretor != "0")
                    .Select(grupo => new ModelsEx.homeRel
                    {
                        mes = grupo.First().mes,
                        ano = grupo.First().ano,



                        indicador = grupo.First().indicador,
                        meta = grupo.First().meta,
                        data_atualizacao = grupo.First().data_atualizacao,
                        cod_gip = grupo.First().cod_gip,
                        setor = grupo.First().setor,
                        home_based = grupo.First().home_based,
                        site = grupo.First().site,
                        turno = grupo.First().turno,
                        indicatorType = grupo.First().indicatorType,

                        goal = grupo.First().goal,
                        weight = grupo.First().weight,
                        hierarchylevel = grupo.First().hierarchylevel,
                        coin1 = grupo.First().coin1,
                        coin2 = grupo.First().coin2,
                        coin3 = grupo.First().coin3,
                        coin4 = grupo.First().coin4,
                        idgda_sector = grupo.First().idgda_sector,
                        min1 = grupo.First().min1,
                        min2 = grupo.First().min2,
                        min3 = grupo.First().min3,
                        min4 = grupo.First().min4,
                        conta = grupo.First().conta,
                        better = grupo.First().better,

                        idcollaborator = grupo.Key.matricula_diretor,
                        name = grupo.First().nome_diretor,
                        cargo = "DIRETOR",
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
                        score = grupo.Max(item => item.score),
                        cod_indicador = grupo.Key.cod_indicador,
                        data = grupo.Key.data,

                        fator0 = grupo.Sum(item => item.fator0),
                        fator1 = grupo.Sum(item => item.fator1),

                        diasTrabalhados = grupo.Max(item => item.diasTrabalhados),
                        diasEscalados = grupo.Max(item => item.diasEscalados),
                        moedasPossiveis = grupo.Count(item => Convert.ToInt32(item.coin1) > 0) > 0 ? Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Count(item => Convert.ToInt32(item.coin1) > 0), 2, MidpointRounding.AwayFromZero) : 0,
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
                    .GroupBy(item => new { item.cod_indicador, item.data, item.matricula_ceo }).Where(d => d.Key.matricula_ceo != "0")
                    .Select(grupo => new ModelsEx.homeRel
                    {
                        mes = grupo.First().mes,
                        ano = grupo.First().ano,



                        indicador = grupo.First().indicador,
                        meta = grupo.First().meta,
                        data_atualizacao = grupo.First().data_atualizacao,
                        cod_gip = grupo.First().cod_gip,
                        setor = grupo.First().setor,
                        home_based = grupo.First().home_based,
                        site = grupo.First().site,
                        turno = grupo.First().turno,
                        indicatorType = grupo.First().indicatorType,

                        goal = grupo.First().goal,
                        weight = grupo.First().weight,
                        hierarchylevel = grupo.First().hierarchylevel,
                        coin1 = grupo.First().coin1,
                        coin2 = grupo.First().coin2,
                        coin3 = grupo.First().coin3,
                        coin4 = grupo.First().coin4,
                        idgda_sector = grupo.First().idgda_sector,
                        min1 = grupo.First().min1,
                        min2 = grupo.First().min2,
                        min3 = grupo.First().min3,
                        min4 = grupo.First().min4,
                        conta = grupo.First().conta,
                        better = grupo.First().better,

                        idcollaborator = grupo.Key.matricula_ceo,
                        name = grupo.First().nome_ceo,
                        cargo = "CEO",
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
                        score = grupo.Max(item => item.score),
                        cod_indicador = grupo.Key.cod_indicador,
                        data = grupo.Key.data,

                        fator0 = grupo.Sum(item => item.fator0),
                        fator1 = grupo.Sum(item => item.fator1),

                        diasTrabalhados = grupo.Max(item => item.diasTrabalhados),
                        diasEscalados = grupo.Max(item => item.diasEscalados),
                        moedasPossiveis = grupo.Count(item => Convert.ToInt32(item.coin1) > 0) > 0 ? Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Count(item => Convert.ToInt32(item.coin1) > 0), 2, MidpointRounding.AwayFromZero) : 0,
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
            //string indicatorsAsString = string.Join(",", inputModel.Indicators.Select(g => g.Id));
            string indicatorsAsString = string.Join(",", inputModel.Indicators.Where(i => i.Id.ToString() != "10000012").Select(g => g.Id));
            string indicatorsCestaAsString = string.Join(",", inputModel.Indicators.Where(i => i.Id.ToString() == "10000012").Select(g => g.Id));
            string CollaboratorId = inputModel.CollaboratorId.ToString();
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
            Funcoes.cestaMetrica cm = Funcoes.getInfMetricBasket();

            //Verifica perfil administrativo
            bool adm = Funcoes.retornaPermissao(CollaboratorId);
            //List<string> listaColaboradores = new List<string>();
            string filtro = "";
            int cargoAtual = 0;
            //Retorna os ids abaixo para filtrar apenas os abaixos
            if (adm == true)
            {
                cargoAtual = 8;
                //listaColaboradores = Funcoes.retornaColaboradoresGeral(dtInicial.ToString(), CollaboratorId);
                filtro = "";
            }
            else
            {
                //listaColaboradores = Funcoes.retornaColaboradoresAbaixo(dtInicial.ToString(), CollaboratorId);
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat(" AND (CL.IDGDA_COLLABORATORS IN ({0}) OR ", CollaboratorId);
                stb.AppendFormat("	    CL.MATRICULA_SUPERVISOR IN ({0}) OR ", CollaboratorId);
                stb.AppendFormat("		CL.MATRICULA_COORDENADOR IN ({0}) OR ", CollaboratorId);
                stb.AppendFormat("		CL.MATRICULA_GERENTE_II IN ({0}) OR ", CollaboratorId);
                stb.AppendFormat("		CL.MATRICULA_GERENTE_I IN ({0}) OR ", CollaboratorId);
                stb.AppendFormat("		CL.MATRICULA_DIRETOR IN ({0}) OR ", CollaboratorId);
                stb.AppendFormat("		CL.MATRICULA_CEO IN ({0})) ", CollaboratorId);

                filtro = stb.ToString();

                cargoAtual = Funcoes.retornaCargoAtual(dtInicial, CollaboratorId);
            }

            //string colaboratorsAsString = string.Join(",", listaColaboradores);

            //Realiza a query que retorna todas as informações dos colaboradores que tiveram monetização.
            List<ModelsEx.homeRel> rmams = new List<ModelsEx.homeRel>();
            rmams = returnHomeRanking(dtInicial, dtFinal, sectorsAsString, indicatorsAsString, hierarchiesAsString, inputModel.Order, filtro);

            //Retirando os resultados do supervisor.. Entender com a Tahto como ficara esta parte.
            rmams = rmams.FindAll(item => item.cargo == "AGENTE").ToList();

            //Pega Hierarquia Atual
            List<ModelsEx.homeRel> supervisores = new List<ModelsEx.homeRel>();
            List<ModelsEx.homeRel> coordenador = new List<ModelsEx.homeRel>();
            List<ModelsEx.homeRel> gerenteii = new List<ModelsEx.homeRel>();
            List<ModelsEx.homeRel> gerentei = new List<ModelsEx.homeRel>();
            List<ModelsEx.homeRel> diretor = new List<ModelsEx.homeRel>();
            List<ModelsEx.homeRel> ceo = new List<ModelsEx.homeRel>();
            List<ModelsEx.homeRel> hierarchies = new List<ModelsEx.homeRel>();
            List<hierarchy> hierar = returnTables.listHierarchy("");

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
                hierarchies = rmams.Concat(supervisores).Concat(coordenador).Concat(gerenteii).Concat(gerentei).Concat(diretor).Concat(ceo).ToList();

            }
            else
            {
                if (cargoAtual >= 2)
                {
                    supervisores = returnListHierarchy(rmams, "SUPERVISOR");
                }
                if (cargoAtual >= 3)
                {
                    coordenador = returnListHierarchy(rmams, "COORDENADOR");
                }
                if (cargoAtual >= 4)
                {
                    gerenteii = returnListHierarchy(rmams, "GERENTE II");
                }
                if (cargoAtual >= 5)
                {
                    gerentei = returnListHierarchy(rmams, "GERENTE I");
                }
                if (cargoAtual >= 6)
                {
                    diretor = returnListHierarchy(rmams, "DIRETOR");
                }
                if (cargoAtual >= 7)
                {
                    ceo = returnListHierarchy(rmams, "CEO");
                }
                //Lista Hierarquia
                hierarchies = rmams.Concat(supervisores).Concat(coordenador).Concat(gerenteii).Concat(gerentei).Concat(diretor).Concat(ceo).ToList();

            }


            hierarchies = hierarchies.OrderBy(d => d.idcollaborator).ThenBy(d => d.cod_indicador).ThenBy(d => d.data).ToList();


            //Pega informação monetização Hierarquia
            //List<ModelsEx.monetizacaoHierarquia> lMone = new List<ModelsEx.monetizacaoHierarquia>();
            //lMone = Funcoes.retornaListaMonetizacaoHierarquia(dtInicial, dtFinal);


            //calculo separado para reincidencia
            for (int i = 0; i < hierarchies.Count; i++)
            {

                if (hierarchies[i].idcollaborator == "652560" && hierarchies[i].cod_indicador == "732")
                {
                    var parou = true;
                }

                ModelsEx.homeRel agente = hierarchies[i];

                agente = monetizationClass.doCalculationResultHome(agente, false);

                if (agente.moedasGanhas == 58)
                {
                    var parou = true;
                }

                hierarchies[i] = agente;
                //.GroupBy(item => new { item.cod_indicador, item.data, item.matricula_supervisor }).Where(d => d.Key.matricula_supervisor != "-")

                //if (agente.cargo != "AGENTE")
                //{
                //    if (agente.name == "BERNARDO AUGUSTO DE BRITO")
                //    {
                //        var parou = true;
                //    }

                //    var monEnc2 = lMone.GroupBy(item => new { item.id }).Where(group => group.Any(items => items.id.ToString() == hierarchies[i].idcollaborator) && group.Any(items => items.date >= dtTimeInicial && items.date <= dtTimeFinal) && group.Any(items => items.idIndicador == int.Parse(hierarchies[i].cod_indicador))).Select(grupo => new ModelsEx.monetizacaoHierarquia
                //    {
                //        id = grupo.First().id,
                //        date = grupo.First().date,
                //        Monetizacao = grupo.Sum(item => item.Monetizacao),
                //    });

                //    double monAg = 0;
                //    if (monEnc2 != null)
                //    {
                //        if (monEnc2.Count() > 0)
                //        {
                //            ModelsEx.monetizacaoHierarquia monEnc = monEnc2.First();
                //            if (monEnc != null)
                //            {
                //                if (monEnc.Monetizacao.ToString() != "")
                //                {
                //                    monAg = double.Parse(monEnc.Monetizacao.ToString());
                //                }
                //            }
                //        }
                //    }

                //    hierarchies[i].moedasGanhas = monAg;
                //}



            }




            //for (int i = 2; i < hierarchies.Count; i++)
            //{
            //    if ((hierarchies[i - 2].grupo == "Prata" || hierarchies[i - 2].grupo == "Bronze") &&
            //        (hierarchies[i - 1].grupo == "Prata" || hierarchies[i - 1].grupo == "Bronze") &&
            //        (hierarchies[i].grupo == "Prata" || hierarchies[i].grupo == "Bronze") &&
            //        hierarchies[i].idcollaborator == hierarchies[i - 2].idcollaborator)
            //    {
            //        hierarchies[i].reincidencia = true;
            //        hierarchies[i - 2].reincidencia = true;
            //        hierarchies[i - 1].reincidencia = true;
            //    }
            //}




            //Calculo de cesta
            //List<ModelsEx.homeRel> listaCesta = new List<ModelsEx.homeRel>();
            //listaCesta = Funcoes.retornaCestaIndicadores(hierarchies, cm, true, true, false);

            //Filtro Cesta
            List<ModelsEx.homeRel> listaCesta = new List<ModelsEx.homeRel>();
            if (indicatorsCestaAsString != "" || indicatorsAsString == "")
            {
                listaCesta = Funcoes.retornaCestaIndicadores(hierarchies, cm, true, true, false);
            }

            //Agrupa resultado por agente e indicador
            List<ModelsEx.homeRel> resultList = hierarchies.GroupBy(d => new { d.idcollaborator, d.indicador }).Select(item => new ModelsEx.homeRel
            {

                mes = item.First().data,
                idcollaborator = item.First().idcollaborator,
                name = item.First().name,
                cargo = item.First().cargo,
                data = item.First().data,
                cod_indicador = item.First().cod_indicador,
                indicador = item.First().indicador,
                meta = item.First().meta,
                data_atualizacao = item.First().data_atualizacao,
                cod_gip = item.First().cod_gip,
                setor = item.First().setor,
                home_based = item.First().home_based,
                site = item.First().site,
                turno = item.First().turno,
                matricula_supervisor = item.First().matricula_supervisor,
                nome_supervisor = item.First().nome_supervisor,
                matricula_coordenador = item.First().matricula_coordenador,
                nome_coordenador = item.First().nome_coordenador,
                matricula_gerente_ii = item.First().matricula_gerente_ii,
                nome_gerente_ii = item.First().nome_gerente_ii,
                matricula_gerente_i = item.First().matricula_gerente_i,
                nome_gerente_i = item.First().nome_gerente_i,
                matricula_diretor = item.First().matricula_diretor,
                nome_diretor = item.First().nome_diretor,
                matricula_ceo = item.First().matricula_ceo,
                nome_ceo = item.First().nome_ceo,
                fator0 = item.Sum(d => d.fator0),
                fator1 = item.Sum(d => d.fator1),
                goal = item.First().goal,
                weight = item.First().weight,
                hierarchylevel = item.First().hierarchylevel,
                idgda_sector = item.First().idgda_sector,
                conta = item.First().conta,
                better = item.First().better,
                grupo = string.Join(", ", item.Select(d => d.grupo)),
                min1 = item.First().min1,
                min2 = item.First().min2,
                min3 = item.First().min3,
                min4 = item.First().min4,
                coin1 = item.First().coin1,
                coin2 = item.First().coin2,
                coin3 = item.First().coin3,
                coin4 = item.First().coin4,
                indicatorType = item.First().indicatorType,
                score = item.First().score,

                diasTrabalhados = item.Max(item2 => item2.diasTrabalhados),
                diasEscalados = item.Max(item2 => item2.diasEscalados),
                moedasPossiveis = item.Max(item2 => item2.moedasPossiveis),
                moedasGanhas = item.Sum(item2 => item2.moedasGanhas),
                qtdPessoas = item.Count(item2 => item2.resultado > 0),
                resultadoAPI = item.Sum(item2 => item2.resultadoAPI),
                vemMeta = item.Max(item2 => item2.vemMeta),
                metaSomada = item.Sum(item2 => item2.metaSomada),
                qtdPessoasTotal = item.Sum(item2 => item2.qtdPessoasTotal),
            }).ToList();


            for (int i = 0; i < resultList.Count(); i++)
            {
                int j = resultList[i].grupo.Split(",").Length;
                if (hierarchies[i].idcollaborator == "749988" && hierarchies[i].cod_indicador == "2")
                {
                    var parou = true;
                }

                if (j > 1)
                {
                    string inf1 = resultList[i].grupo.Split(",")[j - 1].Trim();
                    string inf2 = resultList[i].grupo.Split(",")[j - 2].Trim();
                    //string inf3 = resultList[i].grupo.Split(",")[j - 3].Trim();
                    if ((inf1 == "Prata" || inf1 == "Bronze") &&
                        (inf2 == "Prata" || inf2 == "Bronze")
                        //&& (inf3 == "Prata" || inf3 == "Bronze")
                        )
                    {
                        resultList[i].reincidencia = true;
                    }
                }
            }

            //Calculo agrupado
            for (int i = 0; i < resultList.Count; i++)
            {

                if (resultList[i].idcollaborator == "749988" && resultList[i].cod_indicador == "2")
                {
                    var parou = true;
                }

                ModelsEx.homeRel agente = resultList[i];

                agente = monetizationClass.doCalculationResultHome(agente, false);

                resultList[i] = agente;
            }


            if (indicatorsAsString != "")
            {
                List<string> valoresLista = indicatorsAsString.Split(',').Select(valor => valor.Trim()).ToList();
                resultList = resultList.FindAll(item => valoresLista.Contains(item.cod_indicador)).ToList();
            }

            if (indicatorsCestaAsString != "" && indicatorsAsString == "")
            {
                resultList.Clear();
                resultList = resultList.Concat(listaCesta).ToList();
            }
            else if (indicatorsCestaAsString != "")
            {
                resultList = resultList.Concat(listaCesta).ToList();
            }
            else if (indicatorsAsString == "")
            {
                resultList = resultList.Concat(listaCesta).ToList();
            }

            //resultList = resultList.Concat(listaCesta).ToList();
            //Filtro de grupo, após os calculos
            if (groupsAsString != "")
            {
                //Pega infs Grupo
                List<groups> lGroups = returnTables.listGroups("");
                string groupsAsString3 = string.Join(",", lGroups
                .Where(g => groupsAsString.Contains(g.id.ToString()))
                .Select(g => g.alias));

                //Filtra só os grupos especificos
                resultList = resultList
                    .Where(item => groupsAsString3.Contains(item.grupo.ToUpper()))
                    .ToList();
            }

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
                resultList = resultList
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
                //    resultList = resultList.OrderBy(d => d.cod_indicador).ToList();

                //    List<ModelsEx.homeRel> resultListLessBetter = resultList.FindAll(l => l.better == "LESS_BETTER").OrderBy(d => Convert.ToDouble(d.cod_indicador)).ThenBy(item => Convert.ToDouble(item.resultado)).ThenBy(item => Convert.ToDouble(item.porcentual)).ToList();
                //    List<ModelsEx.homeRel> resultListBiggerBetter = resultList.FindAll(l => l.better == "BIGGER_BETTER").OrderBy(d => Convert.ToDouble(d.cod_indicador)).ThenByDescending(item => Convert.ToDouble(item.resultado)).ThenByDescending(item => Convert.ToDouble(item.porcentual)).ToList();

                //    //resultList = resultList.OrderBy(d => d.cod_indicador).ThenByDescending(item => item.resultado).ThenByDescending(item => item.porcentual).ToList();
                //    resultList = resultListLessBetter.Concat(resultListBiggerBetter).ToList();

                //    //orderBy = " ORDER BY MAX(MZ.SOMA) DESC ";
                //}
                //else
                //{
                //    resultList = resultList.OrderBy(d => d.cod_indicador).ToList();

                //    List<ModelsEx.homeRel> resultListLessBetter = resultList.FindAll(l => l.better == "LESS_BETTER").OrderBy(d => Convert.ToDouble(d.cod_indicador)).ThenByDescending(item => Convert.ToDouble(item.resultado)).ThenByDescending(item => Convert.ToDouble(item.porcentual)).ToList();
                //    List<ModelsEx.homeRel> resultListBiggerBetter = resultList.FindAll(l => l.better == "BIGGER_BETTER").OrderBy(d => Convert.ToDouble(d.cod_indicador)).ThenBy(item => Convert.ToDouble(item.resultado)).ThenBy(item => Convert.ToDouble(item.porcentual)).ToList();

                //    resultList = resultListLessBetter.Concat(resultListBiggerBetter).ToList();

                //    //resultList = resultList.OrderBy(d => d.cod_indicador).ThenByDescending(item => item.resultado).ThenBy(item => item.porcentual).ToList();
                //    //orderBy = " ORDER BY MAX(MZ.SOMA) ASC ";
                //}
            }

            var teste = resultList.Select(Item => Item.idcollaborator == "749988");

            var jsonData = resultList.Select(item => new returnResponseRanking
            {
                Data = item.data,
                MatriculaDoColaborador = item.idcollaborator,
                NomeAgente = item.name,
                //CodigoGIP = item.cod_gip,
                //Setor = item.setor,
                CodigoGIP = (item.cargo == "AGENTE" || item.cargo == "SUPERVISOR") ? item.cod_gip : "-",
                Setor = (item.cargo == "AGENTE" || item.cargo == "SUPERVISOR") ? item.setor : "-",
                CodigoIndicador = item.cod_indicador,
                NomeIndicador = item.indicador,
                TipoIndicador = item.indicatorType,
                Resultado = item.resultado,
                Meta = item.meta,
                PercentualDeAtingimento = item.porcentual,
                Score = item.score,
                Grupo = item.grupo,
                Cargo = item.cargo,
                Reincidencia = item.reincidencia,
                NomeSupervisor = item.nome_supervisor,
                MatriculaSupervisor = item.matricula_supervisor
            }).ToList();

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(jsonData);
        }

        // Método para serializar um DataTable em JSON
    }
}