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
    public class ReportHomeResult1Controller : ApiController
    {

        public List<ModelsEx.homeRel> returnHomeResult(string dtInicial, string dtFinal, string sectors, string indicators, string hierarchies, string ordem)
        {
            // Preparar Filtros
            string filter = "";
            string orderBy = "";
            if (sectors != "")
            {
                filter = filter + $" AND S.IDGDA_SECTOR IN ({sectors}) ";
            }
            //if (groups != "")
            //{
            //    filter = filter + " ";
            //}
            if (indicators != "")
            {
                filter = filter + $" AND R.INDICADORID IN ({indicators}) ";
            }
            if (hierarchies != "")
            {
                filter = filter + $" AND HHR.IDGDA_HIERARCHY IN ({hierarchies}) ";
            }
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
            stb.Append("       MAX(CB.IDGDA_COLLABORATORS) AS IDCOLLABORATOR, ");
            stb.Append("       MAX(CB.NAME) AS NAME, ");
            stb.Append("       MAX(LEVELNAME) AS CARGO, ");
            stb.Append(" ");
            stb.Append("			MAX(R.RESULT) AS RESULTADOAPI, ");
            stb.Append("			MAX(TRAB.TRABALHADO) AS TRABALHADO, ");
            stb.Append("			MAX(ESC.ESCALADO) AS ESCALADO, ");
            stb.Append("			MAX(Bk.MONETIZATION_G1) AS META_MAXIMA, ");
            stb.Append("			MAX(CA.SOMA) AS MOEDA_GANHA, ");
            stb.Append(" ");
            stb.Append("       CONVERT(DATE, R.CREATED_AT) AS 'DATA', ");
            stb.Append("       MAX(R.INDICADORID) AS 'COD INDICADOR', ");
            stb.Append("       MAX(I.NAME) AS 'INDICADOR', ");
            stb.Append("       MAX(HIS.GOAL) AS META, ");
            stb.Append("       '' AS RESULTADO, ");
            stb.Append("       '' AS PORCENTUAL, ");
            stb.Append("       '' AS GRUPO, ");
            stb.Append("       GETDATE() AS 'Data de atualização', ");
            stb.Append("       MAX(S.IDGDA_SECTOR) AS COD_GIP, ");
            stb.Append("       MAX(SEC.NAME) AS SETOR, ");
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
            stb.Append("       MAX(R.FACTORS) AS FATOR, ");
            stb.Append("       HIS.GOAL, ");
            stb.Append("       I.WEIGHT AS WEIGHT, ");
            stb.Append("       HHR.LEVELWEIGHT AS HIERARCHYLEVEL, ");
            stb.Append("       HIG1.MONETIZATION AS COIN1, ");
            stb.Append("       HIG2.MONETIZATION AS COIN2, ");
            stb.Append("       HIG3.MONETIZATION AS COIN3, ");
            stb.Append("       HIG4.MONETIZATION AS COIN4, ");
            stb.Append("       S.IDGDA_SECTOR, ");
            stb.Append("       HIG1.METRIC_MIN AS MIN1, ");
            stb.Append("       HIG2.METRIC_MIN AS MIN2, ");
            stb.Append("       HIG3.METRIC_MIN AS MIN3, ");
            stb.Append("       HIG4.METRIC_MIN AS MIN4, ");
            stb.Append("       CASE ");
            stb.Append("           WHEN MAX(ME.EXPRESSION) IS NULL THEN '(#FATOR1/#FATOR0)' ");
            stb.Append("           ELSE MAX(ME.EXPRESSION) ");
            stb.Append("       END AS CONTA, ");
            stb.Append("       CASE ");
            stb.Append("           WHEN MAX(I.CALCULATION_TYPE) IS NULL THEN 'BIGGER_BETTER' ");
            stb.Append("           ELSE MAX(I.CALCULATION_TYPE) ");
            stb.Append("       END AS BETTER, ");
            stb.Append("       MAX(I.TYPE) AS TYPE ");
            stb.Append("FROM GDA_RESULT (NOLOCK) AS R ");
            stb.Append("INNER JOIN GDA_COLLABORATORS (NOLOCK) AS CB ON CB.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            stb.Append("INNER JOIN GDA_HISTORY_COLLABORATOR_SECTOR (NOLOCK) AS S ON S.CREATED_AT = R.CREATED_AT ");
            stb.Append("AND S.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            stb.Append(" ");
            stb.Append("LEFT JOIN (SELECT COUNT(0) AS 'ESCALADO', IDGDA_COLLABORATORS, CREATED_AT FROM GDA_RESULT (NOLOCK)   ");
            stb.Append("WHERE INDICADORID = -1   ");
            stb.Append("AND CREATED_AT >= @DATAINICIAL AND CREATED_AT <= @DATAFINAL ");
            stb.Append("AND RESULT = 1 AND DELETED_AT IS NULL  ");
            stb.Append("GROUP BY IDGDA_COLLABORATORS, CREATED_AT) AS ESC ON ESC.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS AND ESC.CREATED_AT = R.CREATED_AT ");
            stb.Append("LEFT JOIN (SELECT COUNT(0) AS 'TRABALHADO', IDGDA_COLLABORATORS, CREATED_AT FROM GDA_RESULT (NOLOCK)   ");
            stb.Append("WHERE INDICADORID = 2   ");
            stb.Append("AND CREATED_AT >= @DATAINICIAL AND CREATED_AT <= @DATAFINAL ");
            stb.Append("AND RESULT = 0 AND DELETED_AT IS NULL  ");
            stb.Append("GROUP BY IDGDA_COLLABORATORS, CREATED_AT) AS TRAB ON TRAB.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS AND TRAB.CREATED_AT = R.CREATED_AT ");
            stb.Append("LEFT JOIN GDA_BASKET_INDICATOR (NOLOCK) AS BK ON BK.DATE = R.CREATED_AT AND Bk.INDICATOR_ID = R.INDICADORID AND BK.SECTOR_ID = S.IDGDA_SECTOR ");
            stb.Append("LEFT JOIN  (SELECT collaborator_id,  ");
            stb.Append("	          GDA_INDICATOR_IDGDA_INDICATOR,  ");
            stb.Append("			  RESULT_DATE, ");
            stb.Append("	          SUM(INPUT) AS SOMA  ");
            stb.Append("	   FROM GDA_CHECKING_ACCOUNT (NOLOCK)  ");
            stb.Append("	   WHERE RESULT_DATE >= @DATAINICIAL  ");
            stb.Append("	     AND RESULT_DATE <= @DATAFINAL  ");
            stb.Append("	   GROUP BY collaborator_id,  ");
            stb.Append("	            GDA_INDICATOR_IDGDA_INDICATOR, RESULT_DATE) AS CA ON CA.GDA_INDICATOR_IDGDA_INDICATOR = R.INDICADORID  ");
            stb.Append("	AND CA.collaborator_id = R.IDGDA_COLLABORATORS  AND CA.RESULT_DATE = R.CREATED_AT ");
            stb.Append(" ");
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
            stb.Append("AND HIG1.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND HIG1.SECTOR_ID = S.IDGDA_SECTOR ");
            stb.Append("AND HIG1.GROUPID = 1 ");
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG2 ON HIG2.DELETED_AT IS NULL ");
            stb.Append("AND HIG2.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND HIG2.SECTOR_ID = S.IDGDA_SECTOR ");
            stb.Append("AND HIG2.GROUPID = 2 ");
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG3 ON HIG3.DELETED_AT IS NULL ");
            stb.Append("AND HIG3.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND HIG3.SECTOR_ID = S.IDGDA_SECTOR ");
            stb.Append("AND HIG3.GROUPID = 3 ");
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG4 ON HIG4.DELETED_AT IS NULL ");
            stb.Append("AND HIG4.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND HIG4.SECTOR_ID = S.IDGDA_SECTOR ");
            stb.Append("AND HIG4.GROUPID = 4 ");
            stb.Append("LEFT JOIN GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME ON HME.INDICATORID = R.INDICADORID ");
            stb.Append("AND HME.deleted_at IS NULL ");
            stb.Append("LEFT JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON ME.ID = HME.MATHEMATICALEXPRESSIONID ");
            stb.Append("AND ME.DELETED_AT IS NULL ");
            stb.Append("LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS HHR ON HHR.idgda_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            stb.Append("AND HHR.DATE = @DATAINICIAL ");
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) AS HIS ON HIS.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND HIS.SECTOR_ID = S.IDGDA_SECTOR ");
            stb.Append("AND HIS.DELETED_AT IS NULL ");
            stb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS I ON I.IDGDA_INDICATOR = R.INDICADORID ");
            stb.Append("LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = S.IDGDA_SECTOR ");
            stb.Append("AND R.CREATED_AT >= @DATAINICIAL ");
            stb.Append("AND R.CREATED_AT <= @DATAFINAL ");
            stb.Append("AND I.STATUS = 1 ");
            stb.Append("AND R.DELETED_AT IS NULL ");
            stb.Append("LEFT JOIN GDA_COLLABORATORS_LAST_DETAILS (NOLOCK) AS CL ON CL.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS ");
            stb.AppendFormat("WHERE 1 = 1 {0} ", filter);
            stb.Append("  AND R.CREATED_AT >= @DATAINICIAL ");
            stb.Append("  AND R.CREATED_AT <= @DATAFINAL ");
            stb.Append("GROUP BY R.IDGDA_COLLABORATORS, ");
            stb.Append("         R.CREATED_AT, ");
            stb.Append("         INDICADORID, ");
            stb.Append("         S.IDGDA_SECTOR, ");
            stb.Append("         HIG1.METRIC_MIN, ");
            stb.Append("         HIG2.METRIC_MIN, ");
            stb.Append("         HIG3.METRIC_MIN, ");
            stb.Append("         HIG4.METRIC_MIN, ");
            stb.Append("         HIS.GOAL, ");
            stb.Append("         I.WEIGHT, ");
            stb.Append("         HHR.LEVELWEIGHT, ");
            stb.Append("         HIG1.MONETIZATION, ");
            stb.Append("         HIG2.MONETIZATION, ");
            stb.Append("         HIG3.MONETIZATION, ");
            stb.Append("         HIG4.MONETIZATION  ");


            #region Antigo
            //stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtInicial);
            //stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFinal);
            //stb.Append(" ");
            //stb.Append("SELECT MONTH(@DATAINICIAL) AS MES, ");
            //stb.Append("       YEAR(@DATAINICIAL) AS ANO, ");
            //stb.Append("       MAX(CB.IDGDA_COLLABORATORS) AS IDCOLLABORATOR, ");
            //stb.Append("       MAX(CB.NAME) AS NAME, ");
            //stb.Append("       MAX(LEVELNAME) AS CARGO, ");
            //stb.Append("       CONVERT(DATE, R.CREATED_AT) AS 'DATA', ");
            //stb.Append("       MAX(R.INDICADORID) AS 'COD INDICADOR', ");
            //stb.Append("       MAX(I.NAME) AS 'INDICADOR', ");
            //stb.Append("       MAX(HIS.GOAL) AS META, ");
            //stb.Append("       '' AS RESULTADO, ");
            //stb.Append("       '' AS PORCENTUAL, ");
            //stb.Append("       '' AS GRUPO, ");
            //stb.Append("       GETDATE() AS 'Data de atualização', ");
            //stb.Append("       MAX(S.IDGDA_SECTOR) AS COD_GIP, ");
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
            //stb.Append("       MAX(CL.NOME_CEO) AS 'NOME CEO', ");
            //stb.Append("       MAX(R.FACTORS) AS FATOR, ");
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
            //stb.Append("       END AS BETTER, ");
            //stb.Append("       MAX(I.TYPE) AS TYPE ");
            //stb.Append("FROM GDA_RESULT (NOLOCK) AS R ");
            //stb.Append("INNER JOIN GDA_COLLABORATORS (NOLOCK) AS CB ON CB.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            //stb.Append("INNER JOIN GDA_HISTORY_COLLABORATOR_SECTOR (NOLOCK) AS S ON S.CREATED_AT = R.CREATED_AT ");
            //stb.Append("AND S.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
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
            //stb.Append("LEFT JOIN GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME ON HME.INDICATORID = R.INDICADORID  AND HME.deleted_at IS NULL ");
            //stb.Append("LEFT JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON ME.ID = HME.MATHEMATICALEXPRESSIONID AND ME.DELETED_AT IS NULL ");
            //stb.Append("LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS HHR ON HHR.idgda_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            //stb.Append("AND HHR.DATE = @DATAINICIAL ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) AS HIS ON HIS.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIS.SECTOR_ID = S.IDGDA_SECTOR AND HIS.DELETED_AT IS NULL ");
            //stb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS I ON I.IDGDA_INDICATOR = R.INDICADORID ");
            //stb.Append("LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = S.IDGDA_SECTOR ");
            //stb.Append("AND R.CREATED_AT >= @DATAINICIAL ");
            //stb.Append("AND R.CREATED_AT <= @DATAFINAL ");
            //stb.Append("AND I.STATUS = 1 ");
            //stb.Append("AND R.DELETED_AT IS NULL ");
            //stb.Append("LEFT JOIN GDA_COLLABORATORS_LAST_DETAILS (NOLOCK) AS CL ON CL.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS  ");
            //stb.AppendFormat("WHERE 1 = 1 {0} ", filter);
            //stb.Append("  AND R.CREATED_AT >= @DATAINICIAL ");
            //stb.Append("  AND R.CREATED_AT <= @DATAFINAL ");
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
            //stb.Append("         HIG4.MONETIZATION  ");
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
                            string factors = reader["FATOR"].ToString();
                            string factor0 = Funcoes.RemoverZerosAposPonto(factors.Split(";")[0]);
                            string factor1 = Funcoes.RemoverZerosAposPonto(factors.Split(";")[1]);

                            ModelsEx.homeRel rmam = new ModelsEx.homeRel();
                            rmam.indicatorType = reader["TYPE"].ToString();
                            rmam.mes = reader["MES"].ToString();
                            rmam.ano = reader["ANO"].ToString();
                            rmam.idcollaborator = reader["IDCOLLABORATOR"].ToString();
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




                            if (reader["GOAL"].ToString() != "")
                            {
                                rmam.goal = double.Parse(reader["GOAL"].ToString());
                            }
                            else
                            {
                                rmam.goal = 0;
                            }


                            rmam.weight = reader["WEIGHT"].ToString();
                            rmam.hierarchylevel = reader["HIERARCHYLEVEL"].ToString();

                            if (reader["COIN1"].ToString() != "")
                            {
                                rmam.coin1 = reader["COIN1"].ToString();
                            }
                            else
                            {
                                rmam.coin1 = "";
                            }

                            if (reader["COIN2"].ToString() != "")
                            {
                                rmam.coin2 = reader["COIN2"].ToString();
                            }
                            else
                            {
                                rmam.coin2 = "";
                            }

                            if (reader["COIN3"].ToString() != "")
                            {
                                rmam.coin3 = reader["COIN3"].ToString();
                            }
                            else
                            {
                                rmam.coin3 = "";
                            }

                            if (reader["COIN4"].ToString() != "")
                            {
                                rmam.coin4 = reader["COIN4"].ToString();
                            }
                            else
                            {
                                rmam.coin4 = "";
                            }


                            rmam.idgda_sector = reader["IDGDA_SECTOR"].ToString();

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

                            rmam.conta = reader["CONTA"].ToString();
                            rmam.better = reader["BETTER"].ToString();

                            rmams.Add(rmam);

                        }
                    }
                }
                connection.Close();
            }

            return rmams;
        }


        //public List<RelMonetizationAdmMonth> returnHomeResult(string dtInicial, string dtFinal, string sectors, string indicators, string hierarchies, string ordem)
        //{
        //    // Preparar Filtros
        //    string filter = "";
        //    string orderBy = "";
        //    if (sectors != "")
        //    {
        //        filter = filter + $" AND CA.IDGDA_SECTOR IN ({sectors}) ";
        //    }
        //    //if (groups != "")
        //    //{
        //    //    filter = filter + " ";
        //    //}
        //    if (indicators != "")
        //    {
        //        filter = filter + $" AND CA.GDA_INDICATOR_IDGDA_INDICATOR IN ({indicators}) ";
        //    }
        //    if (hierarchies != "")
        //    {
        //        filter = filter + $" AND HHR.IDGDA_HIERARCHY IN ({hierarchies}) ";
        //    }
        //    //if (ordem != "")
        //    //{
        //    //    if (ordem.ToUpper() == "MELHOR")
        //    //    {
        //    //        orderBy = " ORDER BY MAX(MZ.SOMA) DESC ";
        //    //    }
        //    //    else
        //    //    {
        //    //        orderBy = " ORDER BY MAX(MZ.SOMA) ASC ";
        //    //    }

        //    //}




        
        //    StringBuilder stb = new StringBuilder();
        //    stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '2023-09-01'; ", dtInicial);
        //    stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '2023-09-11'; ", dtFinal);
        //    stb.Append(" ");
        //    stb.Append("SELECT MONTH(@DATAINICIAL) AS MES, YEAR(@DATAINICIAL) AS ANO, CB.IDGDA_COLLABORATORS,  ");
        //    stb.Append("MAX(CB.NAME) AS NAME,  ");
        //    stb.Append("MAX(LEVELNAME) AS CARGO, ");
        //    stb.Append("CONVERT(DATE, CA.RESULT_DATE) AS 'DATA', ");
        //    //stb.Append("MAX(RS.QTD_TRABALHADO) AS 'DIAS TRABALHADOS', ");
        //    stb.Append("MAX(CA.GDA_INDICATOR_IDGDA_INDICATOR) AS 'COD INDICADOR', ");
        //    stb.Append("IT.NAME AS 'INDICADOR', ");
        //    stb.Append("MAX(HIS.GOAL) AS META, ");
        //    stb.Append("'' AS RESULTADO, ");
        //    stb.Append("'' AS PORCENTUAL, ");
        //    //stb.Append("MAX(MZ.SOMA) AS 'GANHO EM MOEDAS', ");
        //    //stb.Append("MAX(BK.GANHO_MAXIMO) AS 'META MAXIMA DE MOEDAS', ");
        //    stb.Append("'' AS GRUPO, ");
        //    stb.Append("GETDATE() AS 'Data de atualização', ");
        //    stb.Append("MAX(CA.IDGDA_SECTOR) AS COD_GIP, ");
        //    stb.Append("MAX(SEC.NAME) AS SETOR, ");
        //    stb.Append("  MAX(CASE WHEN A.NAME = 'HOME_BASED' THEN A.VALUE ELSE '' END) AS HOME_BASED, ");
        //    stb.Append("  MAX(CASE WHEN A.NAME = 'SITE' THEN A.VALUE ELSE '' END) AS SITE, ");
        //    stb.Append("  MAX(CASE WHEN A.NAME = 'PERIODO' THEN A.VALUE ELSE '' END) AS TURNO, ");
        //    stb.Append("  MAX(CASE WHEN HIERARCHY.LEVELWEIGHT = '2' THEN CAST(HIERARCHY.PARENTIDENTIFICATION AS VARCHAR(255)) ELSE '-' END) AS 'MATRICULA SUPERVISOR', ");
        //    stb.Append("  MAX(CASE WHEN HIERARCHY.LEVELWEIGHT = '2' THEN HIERARCHY.NAME ELSE '-' END) AS 'NOME SUPERVISOR', ");
        //    stb.Append("  MAX(CASE WHEN HIERARCHY.LEVELWEIGHT = '3' THEN CAST(HIERARCHY.PARENTIDENTIFICATION AS VARCHAR(255)) ELSE '-' END) AS 'MATRICULA COORDENADOR', ");
        //    stb.Append("  MAX(CASE WHEN HIERARCHY.LEVELWEIGHT = '3' THEN HIERARCHY.NAME ELSE '-' END) AS 'NOME COORDENADOR', ");
        //    stb.Append("  MAX(CASE WHEN HIERARCHY.LEVELWEIGHT = '4' THEN CAST(HIERARCHY.PARENTIDENTIFICATION AS VARCHAR(255)) ELSE '-' END) AS 'MATRICULA GERENTE II', ");
        //    stb.Append("  MAX(CASE WHEN HIERARCHY.LEVELWEIGHT = '4' THEN HIERARCHY.NAME ELSE '-' END) AS 'NOME GERENTE II', ");
        //    stb.Append("  MAX(CASE WHEN HIERARCHY.LEVELWEIGHT = '5' THEN CAST(HIERARCHY.PARENTIDENTIFICATION AS VARCHAR(255)) ELSE '-' END) AS 'MATRICULA GERENTE I', ");
        //    stb.Append("  MAX(CASE WHEN HIERARCHY.LEVELWEIGHT = '5' THEN HIERARCHY.NAME ELSE '-' END) AS 'NOME GERENTE I', ");
        //    stb.Append("  MAX(CASE WHEN HIERARCHY.LEVELWEIGHT = '6' THEN CAST(HIERARCHY.PARENTIDENTIFICATION AS VARCHAR(255)) ELSE '-' END) AS 'MATRICULA DIRETOR', ");
        //    stb.Append("  MAX(CASE WHEN HIERARCHY.LEVELWEIGHT = '6' THEN HIERARCHY.NAME ELSE '-' END) AS 'NOME DIRETOR', ");
        //    stb.Append("  MAX(CASE WHEN HIERARCHY.LEVELWEIGHT = '7' THEN HIERARCHY.PARENTIDENTIFICATION ELSE '-' END) AS 'MATRICULA CEO', ");
        //    stb.Append("  MAX(CASE WHEN HIERARCHY.LEVELWEIGHT = '7' THEN HIERARCHY.NAME ELSE '-' END) AS 'NOME CEO' ");
        //    stb.Append("FROM GDA_COLLABORATORS (NOLOCK) AS CB ");
        //    stb.Append("INNER JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS HHR ON HHR.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS AND CONVERT(DATE, HHR.CREATED_AT) > @DATAINICIAL ");
        //    //stb.Append("LEFT JOIN ( ");
        //    //stb.Append("SELECT IDGDA_COLLABORATORS, COUNT(0) AS QTD_TRABALHADO FROM (SELECT IDGDA_COLLABORATORS, CREATED_AT AS QTD_TRABALHADO FROM GDA_RESULT (NOLOCK) ");
        //    //stb.Append("WHERE CREATED_AT >= @DATAINICIAL AND CREATED_AT <= @DATAFINAL ");
        //    //stb.Append("GROUP BY IDGDA_COLLABORATORS, CREATED_AT) AS S  ");
        //    //stb.Append("GROUP BY S.IDGDA_COLLABORATORS ");
        //    //stb.Append(") AS RS ON RS.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS ");
        //    stb.Append("INNER JOIN GDA_CHECKING_ACCOUNT (NOLOCK) AS CA ON CA.COLLABORATOR_ID = CB.IDGDA_COLLABORATORS AND CA.RESULT_DATE >= @DATAINICIAL AND  ");
        //    stb.Append("CA.RESULT_DATE <= @DATAFINAL AND GDA_INDICATOR_IDGDA_INDICATOR IS NOT NULL AND CA.IDGDA_SECTOR IS NOT NULL ");
        //    stb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS IT ON IT.IDGDA_INDICATOR =  CA.GDA_INDICATOR_IDGDA_INDICATOR ");
        //    //stb.Append("INNER JOIN ( ");
        //    //stb.Append("SELECT collaborator_id, GDA_INDICATOR_IDGDA_INDICATOR, SUM(INPUT) AS SOMA FROM GDA_CHECKING_ACCOUNT (NOLOCK) ");
        //    //stb.Append("WHERE RESULT_DATE >= @DATAINICIAL AND RESULT_DATE <= @DATAFINAL ");
        //    //stb.Append("group by collaborator_id, GDA_INDICATOR_IDGDA_INDICATOR ");
        //    //stb.Append(") AS MZ ON MZ.GDA_INDICATOR_IDGDA_INDICATOR = CA.GDA_INDICATOR_IDGDA_INDICATOR AND MZ.collaborator_id = CA.COLLABORATOR_ID ");
        //    //stb.Append("LEFT JOIN ( ");
        //    //stb.Append("SELECT SUM(MONETIZATION_G1) AS GANHO_MAXIMO, SECTOR_ID FROM GDA_BASKET_INDICATOR (NOLOCK) ");
        //    //stb.Append("WHERE DATE >= @DATAINICIAL AND DATE <= @DATAFINAL ");
        //    //stb.Append("GROUP BY SECTOR_ID ");
        //    //stb.Append(") AS BK ON BK.SECTOR_ID = CA.IDGDA_SECTOR ");
        //    stb.Append("LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = CA.IDGDA_SECTOR ");
        //    stb.Append("LEFT JOIN GDA_ATRIBUTES (NOLOCK) AS A ON (A.NAME = 'HOME_BASED' OR A.NAME = 'SITE' OR A.NAME = 'PERIODO') AND A.CREATED_AT >= @DATAINICIAL AND  ");
        //    stb.Append("A.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS  ");
        //    stb.Append(" ");
        //    stb.Append("INNER JOIN ");
        //    stb.Append("  (SELECT GOAL, ");
        //    stb.Append("          INDICATOR_ID, ");
        //    stb.Append("          SECTOR_ID, ");
        //    stb.Append("          ROW_NUMBER() OVER (PARTITION BY INDICATOR_ID, ");
        //    stb.Append("                                          SECTOR_ID ");
        //    stb.Append("                             ORDER BY CREATED_AT DESC) AS RN ");
        //    stb.Append("   FROM GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) ");
        //    stb.Append("   WHERE DELETED_AT IS NULL ) AS HIS ON HIS.INDICATOR_ID = CA.GDA_INDICATOR_IDGDA_INDICATOR ");
        //    stb.Append("AND HIS.SECTOR_ID = CA.IDGDA_SECTOR ");
        //    stb.Append(" ");
        //    stb.Append(" ");
        //    stb.Append(" ");
        //    stb.Append("LEFT JOIN ( ");
        //    stb.Append("SELECT COD, IDGDA_COLLABORATORS, PARENTIDENTIFICATION, NAME, LEVELWEIGHT ");
        //    stb.Append("FROM ( ");
        //    stb.Append("    SELECT LV1.IDGDA_COLLABORATORS AS COD, LV1.IDGDA_COLLABORATORS, ISNULL(LV1.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION, C.NAME, CASE WHEN LV2.LEVELWEIGHT IS NULL  ");
        //    stb.Append("	AND LV1.PARENTIDENTIFICATION IS NOT NULL THEN '7' ELSE LV2.LEVELWEIGHT END AS LEVELWEIGHT ");
        //    stb.Append("    FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1 ");
        //    stb.Append("	LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS AND LV2.DATE = @DATAINICIAL ");
        //    stb.Append("    LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV1.PARENTIDENTIFICATION ");
        //    stb.Append("	WHERE LV1.DATE = @DATAINICIAL   ");
        //    stb.Append("    UNION ALL ");
        //    stb.Append("    SELECT LV1.IDGDA_COLLABORATORS AS COD, LV2.IDGDA_COLLABORATORS, ISNULL(LV2.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION, C.NAME, CASE WHEN LV3.LEVELWEIGHT IS NULL  ");
        //    stb.Append("	AND LV2.PARENTIDENTIFICATION IS NOT NULL THEN '7' ELSE LV3.LEVELWEIGHT END AS LEVELWEIGHT ");
        //    stb.Append("    FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1 ");
        //    stb.Append("    LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS AND LV2.DATE = @DATAINICIAL ");
        //    stb.Append("    LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS AND LV3.DATE = @DATAINICIAL ");
        //    stb.Append("	LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV2.PARENTIDENTIFICATION ");
        //    stb.Append("    WHERE LV1.DATE = @DATAINICIAL ");
        //    stb.Append("    UNION ALL ");
        //    stb.Append("    SELECT LV1.IDGDA_COLLABORATORS AS COD, LV3.IDGDA_COLLABORATORS, ISNULL(LV3.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION, C.NAME, CASE WHEN LV4.LEVELWEIGHT IS NULL  ");
        //    stb.Append("	AND LV3.PARENTIDENTIFICATION IS NOT NULL THEN '7' ELSE LV4.LEVELWEIGHT END AS LEVELWEIGHT ");
        //    stb.Append("    FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1 ");
        //    stb.Append("    LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS AND LV2.DATE = @DATAINICIAL ");
        //    stb.Append("    LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS AND LV3.DATE = @DATAINICIAL ");
        //    stb.Append("    LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV4 ON LV3.PARENTIDENTIFICATION = LV4.IDGDA_COLLABORATORS AND LV4.DATE = @DATAINICIAL ");
        //    stb.Append("	LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV3.PARENTIDENTIFICATION ");
        //    stb.Append("    WHERE LV1.DATE = @DATAINICIAL ");
        //    stb.Append(" ");
        //    stb.Append("	UNION ALL ");
        //    stb.Append("     ");
        //    stb.Append("    SELECT LV1.IDGDA_COLLABORATORS AS COD, LV4.IDGDA_COLLABORATORS, ISNULL(LV4.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION, C.NAME, CASE WHEN LV5.LEVELWEIGHT IS NULL  ");
        //    stb.Append("	AND LV4.PARENTIDENTIFICATION IS NOT NULL THEN '7' ELSE LV5.LEVELWEIGHT END AS LEVELWEIGHT ");
        //    stb.Append("    FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1 ");
        //    stb.Append("    LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS AND LV2.DATE = @DATAINICIAL ");
        //    stb.Append("    LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS AND LV3.DATE = @DATAINICIAL ");
        //    stb.Append("    LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV4 ON LV3.PARENTIDENTIFICATION = LV4.IDGDA_COLLABORATORS AND LV4.DATE = @DATAINICIAL ");
        //    stb.Append("	LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV5 ON LV4.PARENTIDENTIFICATION = LV5.IDGDA_COLLABORATORS AND LV5.DATE = @DATAINICIAL ");
        //    stb.Append("	LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV4.PARENTIDENTIFICATION ");
        //    stb.Append("    WHERE LV1.DATE = @DATAINICIAL ");
        //    stb.Append(" ");
        //    stb.Append("	UNION ALL ");
        //    stb.Append("     ");
        //    stb.Append("    SELECT LV1.IDGDA_COLLABORATORS AS COD, LV5.IDGDA_COLLABORATORS, ISNULL(LV5.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION, C.NAME, CASE WHEN LV6.LEVELWEIGHT IS NULL  ");
        //    stb.Append("	AND LV5.PARENTIDENTIFICATION IS NOT NULL THEN '7' ELSE LV6.LEVELWEIGHT END AS LEVELWEIGHT ");
        //    stb.Append("    FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1 ");
        //    stb.Append("    LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS AND LV2.DATE = @DATAINICIAL ");
        //    stb.Append("    LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS AND LV3.DATE = @DATAINICIAL ");
        //    stb.Append("    LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV4 ON LV3.PARENTIDENTIFICATION = LV4.IDGDA_COLLABORATORS AND LV4.DATE = @DATAINICIAL ");
        //    stb.Append("	LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV5 ON LV4.PARENTIDENTIFICATION = LV5.IDGDA_COLLABORATORS AND LV5.DATE = @DATAINICIAL ");
        //    stb.Append("	LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV6 ON LV5.PARENTIDENTIFICATION = LV6.IDGDA_COLLABORATORS AND LV6.DATE = @DATAINICIAL ");
        //    stb.Append("	LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV5.PARENTIDENTIFICATION ");
        //    stb.Append("    WHERE LV1.DATE = @DATAINICIAL ");
        //    stb.Append(" ");
        //    stb.Append("	UNION ALL ");
        //    stb.Append("     ");
        //    stb.Append("    SELECT LV1.IDGDA_COLLABORATORS AS COD, LV6.IDGDA_COLLABORATORS, ISNULL(LV6.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION, C.NAME, CASE WHEN LV7.LEVELWEIGHT IS NULL  ");
        //    stb.Append("	AND LV6.PARENTIDENTIFICATION IS NOT NULL THEN '7' ELSE LV7.LEVELWEIGHT END AS LEVELWEIGHT ");
        //    stb.Append("    FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1 ");
        //    stb.Append("    LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS AND LV2.DATE = @DATAINICIAL ");
        //    stb.Append("    LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS AND LV3.DATE = @DATAINICIAL ");
        //    stb.Append("    LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV4 ON LV3.PARENTIDENTIFICATION = LV4.IDGDA_COLLABORATORS AND LV4.DATE = @DATAINICIAL ");
        //    stb.Append("	LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV5 ON LV4.PARENTIDENTIFICATION = LV5.IDGDA_COLLABORATORS AND LV5.DATE = @DATAINICIAL ");
        //    stb.Append("	LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV6 ON LV5.PARENTIDENTIFICATION = LV6.IDGDA_COLLABORATORS AND LV6.DATE = @DATAINICIAL ");
        //    stb.Append("	LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV7 ON LV6.PARENTIDENTIFICATION = LV7.IDGDA_COLLABORATORS AND LV7.DATE = @DATAINICIAL ");
        //    stb.Append("	LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV8 ON LV7.PARENTIDENTIFICATION = LV8.IDGDA_COLLABORATORS AND LV8.DATE = @DATAINICIAL ");
        //    stb.Append("	LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV6.PARENTIDENTIFICATION ");
        //    stb.Append("    WHERE LV1.DATE = @DATAINICIAL ");
        //    stb.Append(" ");
        //    stb.Append(") AS CombinedData ");
        //    stb.Append(" ");
        //    stb.Append(") AS HIERARCHY ON HIERARCHY.COD = CB.IDGDA_COLLABORATORS  ");
        //    stb.AppendFormat(" WHERE 1 = 1 {0} ", filter);
        //    stb.Append("GROUP BY CB.IDGDA_COLLABORATORS, IT.NAME, CONVERT(DATE, CA.RESULT_DATE) ");
        //    //stb.AppendFormat(" {0} ", orderBy);

        //    List<RelMonetizationAdmMonth> rmams = new List<RelMonetizationAdmMonth>();

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();
        //        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
        //        {
        //            using (SqlDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    RelMonetizationAdmMonth rmam = new RelMonetizationAdmMonth();
        //                    rmam.datePay = reader["DATA"].ToString();
        //                    rmam.month = reader["MES"].ToString();
        //                    rmam.year = reader["ANO"].ToString();
        //                    rmam.idgda_collaborators = reader["IDGDA_COLLABORATORS"].ToString();
        //                    rmam.name = reader["name"].ToString();
        //                    rmam.cargo = reader["cargo"].ToString();
        //                    //rmam.dias_trabalhados = reader["dias trabalhados"].ToString();
        //                    rmam.codIndicador = reader["cod indicador"].ToString();
        //                    rmam.indicador = reader["indicador"].ToString();
        //                    rmam.meta = reader["meta"].ToString();
        //                    //rmam.resultado = double.Parse(reader["resultado"].ToString());
        //                    //rmam.porcentual = double.Parse(reader["porcentual"].ToString());
        //                    //rmam.ganho_em_moedas = reader["ganho em moedas"].ToString();
        //                    //rmam.meta_maxima_de_moedas = reader["meta maxima de moedas"].ToString();
        //                    //rmam.grupo = reader["grupo"].ToString();
        //                    //rmam.data_de_atualizacao = reader["data de atualização"].ToString();
        //                    rmam.cod_gip = reader["cod_gip"].ToString();
        //                    rmam.setor = reader["setor"].ToString();
        //                    rmam.home = reader["home_based"].ToString();
        //                    rmam.site = reader["site"].ToString();
        //                    rmam.turno = reader["turno"].ToString();
        //                    rmam.matricula_supervisor = reader["matricula supervisor"].ToString();
        //                    rmam.nome_supervisor = reader["nome supervisor"].ToString();
        //                    rmam.matricula_coordenador = reader["matricula coordenador"].ToString();
        //                    rmam.nome_coordenador = reader["nome coordenador"].ToString();
        //                    rmam.matricula_gerente_ii = reader["matricula gerente ii"].ToString();
        //                    rmam.nome_gerente_ii = reader["nome gerente ii"].ToString();
        //                    rmam.matricula_gerente_i = reader["matricula gerente i"].ToString();
        //                    rmam.nome_gerente_i = reader["nome gerente i"].ToString();
        //                    rmam.matricula_diretor = reader["matricula diretor"].ToString();
        //                    rmam.nome_diretor = reader["nome diretor"].ToString();
        //                    rmam.matricula_ceo = reader["matricula ceo"].ToString();
        //                    rmam.nome_ceo = reader["nome ceo"].ToString();

        //                    rmams.Add(rmam);

        //                }
        //            }
        //        }
        //    }

        //    return rmams;
        //}


        public class returnResponseHome
        {
            public string Data { get; set; }
            public string Ano { get; set; }
            public string Mes { get; set; }
            public string MatriculaDoColaborador { get; set; }
            public string CodigoGIP { get; set; }
            public string Setor { get; set; }
            public string CodigoIndicador { get; set; }
            public string NomeIndicador { get; set; }
            public string TipoIndicador { get; set; }
            public string Resultado { get; set; }
            public string Meta { get; set; }
            public double PercentualDeAtingimento { get; set; }
            public string Grupo { get; set; }
            public string Cargo { get; set; }
            public string NomeAgente { get; set; }
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
            public string StatusHome { get; set; }
            public string Uf { get; set; } //Site
            public string TurnoDoAgente { get; set; }

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
            public List<Sector> sectors { get; set; }
            public List<Group> groups { get; set; }
            public List<Indicator> indicators { get; set; }
            public List<Hierarchy> hierarchies { get; set; }
            public string Type { get; set; }
            public string order { get; set; }
            public DateTime dataInicial { get; set; }
            public DateTime dataFinal { get; set; }
            public string CollaboratorId { get; set; }
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
                        .GroupBy(item => new { item.cod_indicador, item.data, item.matricula_supervisor })
                        .Select(grupo => new ModelsEx.homeRel
                        {
                            mes = grupo.First().mes,
                            ano = grupo.First().ano,


                            indicatorType = grupo.First().indicatorType,
                            indicador = grupo.First().indicador,
                            meta = grupo.First().meta,
                            data_atualizacao = grupo.First().data_atualizacao,
                            cod_gip = grupo.First().cod_gip,
                            setor = grupo.First().setor,
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

                            cod_indicador = grupo.Key.cod_indicador,
                            data = grupo.Key.data,

                            fator0 = grupo.Sum(item => item.fator0),
                            fator1 = grupo.Sum(item => item.fator1),

                            diasTrabalhados = grupo.Max(item => item.diasTrabalhados),
                            diasEscalados = grupo.Max(item => item.diasEscalados),
                            moedasPossiveis = grupo.Max(item => item.moedasPossiveis),
                            moedasGanhas = grupo.Sum(item => item.moedasGanhas),
                            qtdPessoas = grupo.Count(item2 => item2.resultado > 0),
                            resultadoAPI = grupo.Sum(item => item.resultadoAPI),
                        })
                        .ToList();
                }
                else if (hierarchy == "COORDENADOR")
                {
                    retorno = original
                    .GroupBy(item => new { item.cod_indicador, item.data, item.matricula_coordenador })
                    .Select(grupo => new ModelsEx.homeRel
                    {
                        mes = grupo.First().mes,
                        ano = grupo.First().ano,


                        indicatorType = grupo.First().indicatorType,
                        indicador = grupo.First().indicador,
                        meta = grupo.First().meta,
                        data_atualizacao = grupo.First().data_atualizacao,
                        cod_gip = grupo.First().cod_gip,
                        setor = grupo.First().setor,
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
                        moedasPossiveis = grupo.Max(item => item.moedasPossiveis),
                        moedasGanhas = grupo.Sum(item => item.moedasGanhas),
                        qtdPessoas = grupo.Count(item2 => item2.resultado > 0),
                        resultadoAPI = grupo.Sum(item => item.resultadoAPI),
                    })
                    .ToList();
                }
                else if (hierarchy == "GERENTE II")
                {
                    retorno = original
                    .GroupBy(item => new { item.cod_indicador, item.data, item.matricula_gerente_ii })
                    .Select(grupo => new ModelsEx.homeRel
                    {
                        mes = grupo.First().mes,
                        ano = grupo.First().ano,


                        indicatorType = grupo.First().indicatorType,
                        indicador = grupo.First().indicador,
                        meta = grupo.First().meta,
                        data_atualizacao = grupo.First().data_atualizacao,
                        cod_gip = grupo.First().cod_gip,
                        setor = grupo.First().setor,
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

                        cod_indicador = grupo.Key.cod_indicador,
                        data = grupo.Key.data,

                        fator0 = grupo.Sum(item => item.fator0),
                        fator1 = grupo.Sum(item => item.fator1),

                        diasTrabalhados = grupo.Max(item => item.diasTrabalhados),
                        diasEscalados = grupo.Max(item => item.diasEscalados),
                        moedasPossiveis = grupo.Max(item => item.moedasPossiveis),
                        moedasGanhas = grupo.Sum(item => item.moedasGanhas),
                        qtdPessoas = grupo.Count(item2 => item2.resultado > 0),
                        resultadoAPI = grupo.Sum(item => item.resultadoAPI),
                    })
                    .ToList();
                }
                else if (hierarchy == "GERENTE I")
                {
                    retorno = original
                    .GroupBy(item => new { item.cod_indicador, item.data, item.matricula_gerente_i })
                    .Select(grupo => new ModelsEx.homeRel
                    {
                        mes = grupo.First().mes,
                        ano = grupo.First().ano,


                        indicatorType = grupo.First().indicatorType,
                        indicador = grupo.First().indicador,
                        meta = grupo.First().meta,
                        data_atualizacao = grupo.First().data_atualizacao,
                        cod_gip = grupo.First().cod_gip,
                        setor = grupo.First().setor,
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

                        cod_indicador = grupo.Key.cod_indicador,
                        data = grupo.Key.data,

                        fator0 = grupo.Sum(item => item.fator0),
                        fator1 = grupo.Sum(item => item.fator1),

                        diasTrabalhados = grupo.Max(item => item.diasTrabalhados),
                        diasEscalados = grupo.Max(item => item.diasEscalados),
                        moedasPossiveis = grupo.Max(item => item.moedasPossiveis),
                        moedasGanhas = grupo.Sum(item => item.moedasGanhas),
                        qtdPessoas = grupo.Count(item2 => item2.resultado > 0),
                        resultadoAPI = grupo.Sum(item => item.resultadoAPI),
                    })
                    .ToList();
                }
                else if (hierarchy == "DIRETOR")
                {
                    retorno = original
                    .GroupBy(item => new { item.cod_indicador, item.data, item.matricula_diretor })
                    .Select(grupo => new ModelsEx.homeRel
                    {
                        mes = grupo.First().mes,
                        ano = grupo.First().ano,


                        indicatorType = grupo.First().indicatorType,
                        indicador = grupo.First().indicador,
                        meta = grupo.First().meta,
                        data_atualizacao = grupo.First().data_atualizacao,
                        cod_gip = grupo.First().cod_gip,
                        setor = grupo.First().setor,
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

                        cod_indicador = grupo.Key.cod_indicador,
                        data = grupo.Key.data,

                        fator0 = grupo.Sum(item => item.fator0),
                        fator1 = grupo.Sum(item => item.fator1),

                        diasTrabalhados = grupo.Max(item => item.diasTrabalhados),
                        diasEscalados = grupo.Max(item => item.diasEscalados),
                        moedasPossiveis = grupo.Max(item => item.moedasPossiveis),
                        moedasGanhas = grupo.Sum(item => item.moedasGanhas),
                        qtdPessoas = grupo.Count(item2 => item2.resultado > 0),
                        resultadoAPI = grupo.Sum(item => item.resultadoAPI),
                    })
                    .ToList();
                }
                else if (hierarchy == "CEO")
                {
                    retorno = original
                    .GroupBy(item => new { item.cod_indicador, item.data, item.matricula_ceo })
                    .Select(grupo => new ModelsEx.homeRel
                    {
                        mes = grupo.First().mes,
                        ano = grupo.First().ano,


                        indicatorType = grupo.First().indicatorType,
                        indicador = grupo.First().indicador,
                        meta = grupo.First().meta,
                        data_atualizacao = grupo.First().data_atualizacao,
                        cod_gip = grupo.First().cod_gip,
                        setor = grupo.First().setor,
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

                        cod_indicador = grupo.Key.cod_indicador,
                        data = grupo.Key.data,

                        fator0 = grupo.Sum(item => item.fator0),
                        fator1 = grupo.Sum(item => item.fator1),

                        diasTrabalhados = grupo.Max(item => item.diasTrabalhados),
                        diasEscalados = grupo.Max(item => item.diasEscalados),
                        moedasPossiveis = grupo.Max(item => item.moedasPossiveis),
                        moedasGanhas = grupo.Sum(item => item.moedasGanhas),
                        qtdPessoas = grupo.Count(item2 => item2.resultado > 0),
                        resultadoAPI = grupo.Sum(item => item.resultadoAPI),
                    })
                    .ToList();
                }

            }
            catch (Exception ex)
            {

                throw;
            }

            return retorno;
        }

        // POST: api/Results
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {
            string dtInicial = inputModel.dataInicial.ToString("yyyy-MM-dd");
            string dtFinal = inputModel.dataFinal.ToString("yyyy-MM-dd");
            string groupsAsString = string.Join(",", inputModel.groups.Select(g => g.Id));
            string sectorsAsString = string.Join(",", inputModel.sectors.Select(g => g.Id));
            string hierarchiesAsString = string.Join(",", inputModel.hierarchies.Select(g => g.Id));
            string indicatorsAsString = string.Join(",", inputModel.indicators.Select(g => g.Id));
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

            if (sectorsAsString == "")
            {
                return BadRequest("Selecione ao menos 1 setor!");
            }

            Funcoes.cestaMetrica cm = Funcoes.getInfMetricBasket();

            //Pega informação monetização Hierarquia
            List<ModelsEx.monetizacaoHierarquia> lMone = new List<ModelsEx.monetizacaoHierarquia>();
            lMone = Funcoes.retornaListaMonetizacaoHierarquia(dtInicial, dtFinal);

            //Realiza a query que retorna todas as informações dos colaboradores que tiveram moneitzação.
            List<ModelsEx.homeRel> rmams = new List<ModelsEx.homeRel>();
            rmams = returnHomeResult(dtInicial, dtFinal, sectorsAsString, indicatorsAsString, hierarchiesAsString, inputModel.order);

            for (int i = 0; i < rmams.Count; i++)
            {

                if (rmams[i].idcollaborator == "753027")
                {
                    var parou = true;
                }

                ModelsEx.homeRel agente = rmams[i];

                agente = monetizationClass.doCalculationResultHome(agente, false);

                rmams[i] = agente;
            }





            //Supervisores
            //List<ModelsEx.homeRel> supervisores = returnListHierarchy(rmams, "SUPERVISOR");

            //List<ModelsEx.homeRel> coordenador = returnListHierarchy(rmams, "COORDENADOR");

            //List<ModelsEx.homeRel> gerenteii = returnListHierarchy(rmams, "GERENTE II");

            //List<ModelsEx.homeRel> gerentei = returnListHierarchy(rmams, "GERENTE I");

            //List<ModelsEx.homeRel> diretor = returnListHierarchy(rmams, "DIRETOR");

            //List<ModelsEx.homeRel> ceo = returnListHierarchy(rmams, "CEO");

            //Verifica perfil administrativo
            bool adm = Funcoes.retornaPermissao(CollaboratorId);
            List<string> listaColaboradores = new List<string>();
            int cargoAtual = 0;
            //Retorna os ids abaixo para filtrar apenas os abaixos
            if (adm == true)
            {
                cargoAtual = 8;
                listaColaboradores = Funcoes.retornaColaboradoresGeral(dtInicial.ToString(), CollaboratorId);
            }
            else
            {
                listaColaboradores = Funcoes.retornaColaboradoresAbaixo(dtInicial.ToString(), CollaboratorId);
                cargoAtual = Funcoes.retornaCargoAtual(dtInicial, CollaboratorId);
            }


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

            }


            hierarchies = supervisores.Concat(coordenador).Concat(gerenteii).Concat(gerentei).Concat(diretor).Concat(ceo).ToList();


            for (int i = 0; i < hierarchies.Count; i++)
            {
                ModelsEx.homeRel agente = hierarchies[i];

                //if (agente.name == "LUIS RICARDO FERREIRA" && agente.data == "04/09/2023 00:00:00" && agente.indicador == "Irc 24hrs | Taxa Repetida")
                //{
                //    bool parou = true;
                //}

                agente = monetizationClass.doCalculationResultHome(agente, false);

                hierarchies[i] = agente;


                if (agente.name == "TAYNNA BORGES SOUZA SILVA DE OLIVEIRA" && agente.data == "02/10/2023 00:00:00")
                {
                    var parou = true;
                }

                var monEnc2 = lMone.Find(item => item.id.ToString() == hierarchies[i].idcollaborator && item.date == DateTime.Parse(agente.data) && item.idIndicador == int.Parse(hierarchies[i].cod_indicador));

                double monAg = 0;
                if (monEnc2 != null)
                {

                    if (monEnc2.Monetizacao.ToString() != "")
                    {
                        monAg = double.Parse(monEnc2.Monetizacao.ToString());
                    }

                }

                hierarchies[i].moedasGanhas = monAg;

            }



            List<ModelsEx.homeRel> elementosFiltrados = rmams.Concat(hierarchies).ToList();


            //Calculo de cesta
            List<ModelsEx.homeRel> listaCesta = new List<ModelsEx.homeRel>();
            listaCesta = Funcoes.retornaCestaIndicadores(elementosFiltrados, cm, true, false, false);
            elementosFiltrados = elementosFiltrados.Concat(listaCesta).ToList();

            //Filtro de grupo, após os calculos
            if (groupsAsString != "")
            {
                //Pega infs Grupo
                List<groups> lGroups = returnTables.listGroups("");
                string groupsAsString3 = string.Join(",", lGroups
                .Where(g => groupsAsString.Contains(g.id.ToString()))
                .Select(g => g.alias));

                //Filtra só os grupos especificos
                elementosFiltrados = supervisores
                    .Where(item => groupsAsString3.Contains(item.grupo.ToUpper()))
                    .ToList();
            }

            if (inputModel.order != "")
            {
                if (inputModel.order.ToUpper() == "MELHOR")
                {
                    elementosFiltrados = elementosFiltrados.OrderByDescending(item => item.porcentual).ToList();
                    //orderBy = " ORDER BY MAX(MZ.SOMA) DESC ";
                }
                else
                {
                    elementosFiltrados = elementosFiltrados.OrderBy(item => item.porcentual).ToList();
                    //orderBy = " ORDER BY MAX(MZ.SOMA) ASC ";
                }
            }


            var jsonData = elementosFiltrados.Select(item => new returnResponseHome
            {
                Data = item.data,
                Mes = item.mes,
                Ano = item.ano,
                MatriculaDoColaborador = item.idcollaborator,
                CodigoGIP = item.cod_gip,
                Setor = item.setor,
                CodigoIndicador = item.cod_indicador,
                NomeIndicador = item.indicador,
                TipoIndicador = item.indicatorType,
                Resultado = item.resultado.ToString().Replace(".", ","),
                Meta = item.meta.Replace(".", ","),
                PercentualDeAtingimento = item.porcentual,
                Grupo = item.grupo,
                Cargo = item.cargo,
                NomeAgente = item.name,
                MatriculaSupervisor = item.matricula_supervisor,
                NomeSupervisor = item.nome_supervisor,
                MatriculaCoordenador = item.matricula_coordenador,
                NomeCoordenador = item.nome_coordenador,
                MatriculaGerente2 = item.matricula_gerente_ii,
                NomeGerente2 = item.nome_gerente_ii,
                MatriculaGerente1 = item.matricula_gerente_i,
                NomeGerente1 = item.nome_gerente_i,
                MatriculaDiretor = item.matricula_diretor,
                NomeDiretor = item.nome_diretor,
                MatriculaCEO = item.matricula_ceo,
                NomeCEO = item.nome_ceo,
                StatusHome = item.home_based,
                Uf = item.site, //Site
                TurnoDoAgente = item.turno
            }).ToList();

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(jsonData);
        }

        // Método para serializar um DataTable em JSON
    }
}