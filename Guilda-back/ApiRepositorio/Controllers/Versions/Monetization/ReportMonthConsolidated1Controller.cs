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
    public class ReportMonthConsolidated1Controller : ApiController
    {
        public List<RelMonetizationAdmMonth> returnMonetizationConsolidated(string dtInicial, string dtFinal, string sectors, string collaborators, string hierarchies, string ordem)
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
            //if (indicators != "")
            //{
            //    filter = filter + $" AND CA.GDA_INDICATOR_IDGDA_INDICATOR IN ({indicators}) ";
            //}
            //if (hierarchies != "")
            //{
            //    filter = filter + $" AND CL.IDGDA_HIERARCHY IN ({hierarchies}) ";
            //}
            if (collaborators != "")
            {
                filter = filter + $" AND CB.IDGDA_COLLABORATORS IN ({collaborators}) ";
            }
            //if (ordem != "")
            //{
            //    if (ordem.ToUpper() == "MELHOR")
            //    {
            //        //ARRUMAR ORDER BY
            //       orderBy = " ORDER BY SUM(INPUT) DESC ";
            //    }
            //    else
            //    {
            //        orderBy = " ORDER BY SUM(INPUT) ASC ";
            //    }

            //}


           
            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}';  ", dtInicial);
            stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFinal);
            stb.Append("SELECT MONTH(@DATAINICIAL) AS MES, ");
            stb.Append("       YEAR(@DATAINICIAL) AS ANO, ");
            stb.Append("       CB.IDGDA_COLLABORATORS, ");
            stb.Append("       MAX(CB.NAME) AS NAME, ");
            stb.Append("       MAX(CL.CARGO) AS CARGO, ");
            stb.Append("       CONVERT(NVARCHAR(MAX), SUM(INPUT)) + '/' + CONVERT(NVARCHAR(MAX), SUM(OUTPUT)) AS 'Entrada/Saida', ");
            stb.Append("       SUM(INPUT) AS Entrada, ");
            stb.Append("       SUM(OUTPUT) AS Saida, ");
            stb.Append("       MAX(CL.IDGDA_SECTOR) AS COD_GIP, ");
            //stb.Append("       MAX(CL.IDGDA_SECTOR_SUPERVISOR) AS COD_GIP_SUPERVISOR, ");
            //stb.Append("       MAX(SECSUP.NAME) AS SETOR_SUPERVISOR,  ");
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
            stb.Append("       MAX(CL.NOME_CEO) AS 'NOME CEO' ");
            stb.Append("FROM GDA_COLLABORATORS (NOLOCK) AS CB ");
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
            //stb.Append("   ON CL.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS ");
            //stb.Append("AND CL.CREATED_AT = @DATAINICIAL ");
            stb.Append("INNER JOIN GDA_CHECKING_ACCOUNT (NOLOCK) AS CA ON CA.COLLABORATOR_ID = CB.IDGDA_COLLABORATORS ");
            //stb.Append("AND CA.RESULT_DATE >= @DATAINICIAL ");
            //stb.Append("AND CA.RESULT_DATE <= @DATAFINAL ");
            stb.Append("AND CONVERT(DATE,CA.CREATED_AT) >= @DATAINICIAL ");
            stb.Append("AND CONVERT(DATE,CA.CREATED_AT) <= @DATAFINAL ");
            stb.Append(" LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CL ON CL.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS AND CL.CREATED_AT = CONVERT(DATE,CA.RESULT_DATE) ");
            //stb.Append("AND GDA_INDICATOR_IDGDA_INDICATOR IS NOT NULL ");
            //stb.Append("AND CA.IDGDA_SECTOR IS NOT NULL ");
            stb.Append("LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = CL.IDGDA_SECTOR ");
            stb.Append("LEFT JOIN GDA_SECTOR (NOLOCK) AS SECSUP ON SECSUP.IDGDA_SECTOR = CL.IDGDA_SECTOR_SUPERVISOR ");
            stb.AppendFormat("WHERE 1 = 1 {0} ", filter);
            stb.Append("AND CL.IDGDA_SECTOR IS NOT NULL ");
            stb.Append("AND CL.CARGO IS NOT NULL ");
            stb.Append("AND CL.HOME_BASED <> '' ");
            //stb.Append("AND CL.active = 'true' ");
            stb.Append("GROUP BY CB.IDGDA_COLLABORATORS  ");
            
            #region Antigo
            //stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}';  ", dtInicial);
            //stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFinal);
            //stb.Append(" ");
            //stb.Append("SELECT MONTH(@DATAINICIAL) AS MES, ");
            //stb.Append("       YEAR(@DATAINICIAL) AS ANO, ");
            //stb.Append("       CB.IDGDA_COLLABORATORS, ");
            //stb.Append("       MAX(CB.NAME) AS NAME, ");
            //stb.Append("       MAX(LEVELNAME) AS CARGO, ");
            //stb.Append("       CONVERT(NVARCHAR(MAX), SUM(INPUT)) + '/' + CONVERT(NVARCHAR(MAX), SUM(OUTPUT)) AS 'Entrada/Saida', ");
            //stb.Append("       SUM(INPUT) AS Entrada, ");
            //stb.Append("       SUM(OUTPUT) AS Saida, ");
            //stb.Append("       MAX(CA.IDGDA_SECTOR) AS COD_GIP, ");
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
            //stb.Append("FROM GDA_COLLABORATORS (NOLOCK) AS CB ");
            //stb.Append("INNER JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS HHR ON HHR.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS ");
            //stb.Append("AND CONVERT(DATE, HHR.DATE) = CONVERT(DATE, DATEADD(DAY, -1, GETDATE())) ");
            //stb.Append("INNER JOIN GDA_CHECKING_ACCOUNT (NOLOCK) AS CA ON CA.COLLABORATOR_ID = CB.IDGDA_COLLABORATORS ");
            //stb.Append("AND CA.RESULT_DATE >= @DATAINICIAL ");
            //stb.Append("AND CA.RESULT_DATE <= @DATAFINAL ");
            //stb.Append("AND GDA_INDICATOR_IDGDA_INDICATOR IS NOT NULL ");
            //stb.Append("AND CA.IDGDA_SECTOR IS NOT NULL ");
            //stb.Append("LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = CA.IDGDA_SECTOR ");
            //stb.Append("LEFT JOIN GDA_COLLABORATORS_LAST_DETAILS (NOLOCK) AS CL ON CL.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS ");
            //stb.AppendFormat("WHERE 1 = 1 {0} ", filter);
            //stb.Append("GROUP BY CB.IDGDA_COLLABORATORS ");
            #endregion

            List<RelMonetizationAdmMonth> rmams = new List<RelMonetizationAdmMonth>();

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
                            RelMonetizationAdmMonth rmam = new RelMonetizationAdmMonth();
                            rmam.month = reader["MES"].ToString();
                            rmam.year = reader["ANO"].ToString();

                            rmam.idgda_collaborators = reader["IDGDA_COLLABORATORS"].ToString();
                            rmam.name = reader["name"].ToString();
                            rmam.cargo = reader["cargo"].ToString();
                            
                            rmam.entradaSaida = reader["entrada/saida"].ToString();
                            rmam.entrada = reader["entrada"].ToString();
                            rmam.saida = reader["saida"].ToString();
                            //rmam.resultado = double.Parse(reader["resultado"].ToString());
                            //rmam.porcentual = double.Parse(reader["porcentual"].ToString());

                            rmam.cod_gip = reader["cod_gip"].ToString();
                            rmam.setor = reader["setor"].ToString();
                            rmam.home = reader["home_based"].ToString();
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
                            //rmam.cod_gip_supervisor = reader["COD_GIP_SUPERVISOR"].ToString();
                            //rmam.setor_supervisor = reader["setor_supervisor"].ToString();

                            rmams.Add(rmam);

                        }
                    }
                }
                connection.Close();
            }

            return rmams;
        }


        public class returnResponseConsolidated
        {
            public string Mes { get; set; }
            public string Ano { get; set; }
            public string Matricula { get; set; }
            public string NomeColaborador { get; set; }
            public string Cargo { get; set; }

            public string EntradaSaida { get; set; }
            public string Entrada { get; set; }

            public string Saida { get; set; }
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
        //    sb.Append("   FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP ");
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
        //    sb.Append("   FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP H ");
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
            public List<Indicator> Collaborators { get; set; }
            public List<Hierarchy> Hierarchies { get; set; }
            public string Type { get; set; }
            public string Order { get; set; }
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
            //string groupsAsString = string.Join(",", inputModel.Groups.Select(g => g.Id));
            string sectorsAsString = string.Join(",", inputModel.Sectors.Select(g => g.Id));
            string hierarchiesAsString = string.Join(",", inputModel.Hierarchies.Select(g => g.Id));
            string colaboratorsAsString = string.Join(",", inputModel.Collaborators.Select(g => g.Id));
            //string indicatorsAsString = string.Join(",", inputModel.Indicators.Select(g => g.Id));
            DateTime dtTimeInicial = DateTime.ParseExact(dtInicial, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dtTimeFinal = DateTime.ParseExact(dtFinal, "yyyy-MM-dd", CultureInfo.InvariantCulture);


            TimeSpan diff = dtTimeFinal.Subtract(dtTimeInicial);
            int diferencaEmDias = (int)diff.TotalDays;
            if (diferencaEmDias > 31)
            {
                return BadRequest("Selecionar uma data de no maximo 1 mês!");
            }

            //string dtInicial = HttpContext.Current.Request.Form["DataInicio"];
            //string dtFinal = HttpContext.Current.Request.Form["DataFim"];

            //Realiza a query que retorna todas as informações dos colaboradores que tiveram moneitzação.
            List<RelMonetizationAdmMonth> rmams = new List<RelMonetizationAdmMonth>();
            rmams = returnMonetizationConsolidated(dtInicial, dtFinal, sectorsAsString, colaboratorsAsString, hierarchiesAsString, inputModel.Order);

            if (inputModel.Order != "")
            {
                if (inputModel.Order.ToUpper() == "MELHOR")
                {
                    rmams = rmams.OrderByDescending(r => r.entrada).ToList();
                }
                else
                {
                    rmams = rmams.OrderBy(r => r.entrada).ToList();
                }
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

            //Filtro de grupo, após os calculos
            //if (groupsAsString != "")
            //{
            //    //Pega infs Grupo
            //    List<groups> lGroups = returnTables.listGroups("");
            //    string groupsAsString3 = string.Join(",", lGroups
            //    .Where(g => groupsAsString.Contains(g.id.ToString()))
            //    .Select(g => g.alias));

            //    //Filtra só os grupos especificos
            //    List<RelMonetizationAdmMonth> elementosFiltrados = rmams
            //        .Where(item => groupsAsString3.Contains(item.grupo.ToUpper()))
            //        .ToList();
            //}


            var jsonData = rmams.Select(item => new returnResponseConsolidated
            {
                Mes = item.month,
                Ano = item.year,
                Matricula = item.idgda_collaborators,
                NomeColaborador = item.name,
                Cargo = item.cargo,
                EntradaSaida = item.entradaSaida,
                Entrada = item.entrada,
                Saida = item.saida,
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
                //CodigoGIP = item.cod_gip,
                //CodigoGIP = item.cargo == "SUPERVISOR" ? item.cod_gip_supervisor : item.cargo == "AGENTE" ? item.cod_gip : "-",
                //CodigoGIP = item.cargo == "SUPERVISOR" || item.cod_gip_supervisor == "" && item.cod_gip_supervisor == null ? item.cod_gip :
                //            item.cargo == "SUPERVISOR" || item.cod_gip_supervisor != null && item.cod_gip_supervisor != "" ? item.cod_gip_supervisor :
                //            item.cargo == "AGENTE" ? item.cod_gip : "-",
                //Setor = item.cargo == "SUPERVISOR" ? item.setor_supervisor : item.cargo == "AGENTE" ? item.setor : "-",
                //Setor = item.cargo == "SUPERVISOR" || item.setor_supervisor == ""  && item.setor_supervisor == null ? item.setor :
                //        item.cargo == "SUPERVISOR" || item.setor_supervisor != null && item.setor_supervisor != "" ? item.setor_supervisor:
                //        item.cargo == "AGENTE" ? item.setor : "-",
                //Setor = item.setor,
                CodigoGIP = (item.cargo == "AGENTE" || item.cargo == "SUPERVISOR") ? item.cod_gip : "-",
                Setor = (item.cargo == "AGENTE" || item.cargo == "SUPERVISOR") ? item.setor : "-",
                Home = item.home,
                Turno = item.turno,
                Site = item.site
            }).ToList();

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(jsonData);
        }

        // Método para serializar um DataTable em JSON

    }
}