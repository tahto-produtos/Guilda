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
using static ApiRepositorio.Controllers.ResultConsolidatedController;
using System.Drawing;
using DocumentFormat.OpenXml.Bibliography;
using System.Runtime.ConstrainedExecution;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class IntegracaoAPIResultController : ApiController
    {// POST: api/Results
        public class ResultAPI
        {
            public string DATA { get; set; }
            public string TYPE { get; set; }
            public string STATUS { get; set; }
            public int QTD { get; set; }
        }
        public class returnResponseDay
        {
            public string DATA { get; set; }
            public string TYPE { get; set; }
            public string STATUS { get; set; }
            public int QTD { get; set; }
        }

        public class Type
        {
            public string DATA { get; set; }
            public string TYPE { get; set; }
            public int QTD { get; set; }
            public string STATUS { get; set; }
        }

        public List<Type> ReturnInfoType(string type, string dtinicial, string dtfinal)
        {
            List<Type> ListType = new List<Type>();
            StringBuilder sb = new StringBuilder();
            if (type == "ATRIBUTES")
            {
                //sb.Append($"DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{dtinicial}'  ");
                //sb.Append($"DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{dtfinal}'  ");
                //sb.Append("SELECT ISNULL(MAX(SOMA), 0) AS QTD, MAX(LT.CREATED_AT) AS DATA, MAX(T.TRANSACTIONID) AS TRANSACTIONID,  ");
                //sb.Append("MAX(CASE WHEN LT.STATUS = 'START' THEN 1  ");
                //sb.Append("			WHEN LT.STATUS = 'CONCLUDED' THEN 2  ");
                //sb.Append("			ELSE 3 END) AS STATUS,  ");
                //sb.Append("			'ATRIBUTOS' AS TYPE  ");
                //sb.Append("FROM GDA_LOG_TRANSACTION (NOLOCK) AS LT  ");
                //sb.Append("INNER JOIN GDA_TRANSACTION (NOLOCK) AS T ON LT.TRANSACTIONID = T.TRANSACTIONID  ");
                //sb.Append("LEFT JOIN  ");
                //sb.Append("(  ");
                //sb.Append("SELECT COUNT(0) AS SOMA, TRANSACTIONID FROM GDA_ATRIBUTES (NOLOCK)  ");
                //sb.Append("WHERE CREATED_AT >= @DATAINICIAL  ");
                //sb.Append("AND CREATED_AT <= @DATAFINAL  ");
                //sb.Append("GROUP BY TRANSACTIONID  ");
                //sb.Append(") AS R ON R.TRANSACTIONID = T.IDGDA_TRANSACTION   ");
                //sb.Append("WHERE CONVERT(DATE, LT.CREATED_AT) >= @DATAINICIAL  ");
                //sb.Append("AND CONVERT(DATE, LT.CREATED_AT) <= @DATAFINAL AND TYPE = 'ATRIBUTES'  ");
                //sb.Append("GROUP BY T.IDGDA_TRANSACTION  ");
                sb.Append($"DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{dtinicial}'  ");
                sb.Append($"DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{dtfinal}'  ");
                sb.Append("SELECT COUNT(0) AS QTD, 'ATRIBUTOS' AS TYPE, CREATED_AT AS DATA, CASE WHEN COUNT(0) > 0 THEN 2 ELSE 3 END AS STATUS FROM GDA_ATRIBUTES (NOLOCK) ");
                sb.Append("WHERE CONVERT(DATE,CREATED_AT) >= CONVERT(DATE,@DATAINICIAL)  ");
                sb.Append("AND CONVERT(DATE,CREATED_AT) <= CONVERT(DATE,@DATAFINAL)  ");
                sb.Append("GROUP BY CREATED_AT  ");

            }
            else if (type == "HIERARCHY")
            {
                //sb.Append($"DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{dtinicial}'  ");
                //sb.Append($"DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{dtfinal}'  ");
                //sb.Append("SELECT ISNULL(MAX(SOMA), 0) AS QTD, MAX(LT.CREATED_AT) AS DATA, MAX(T.TRANSACTIONID) AS TRANSACTIONID,  ");
                //sb.Append("MAX(CASE WHEN LT.STATUS = 'START' THEN 1  ");
                //sb.Append("			WHEN LT.STATUS = 'CONCLUDED' THEN 2  ");
                //sb.Append("			ELSE 3 END) AS STATUS,  ");
                //sb.Append("			'HIERARQUIA' AS TYPE  ");
                //sb.Append("FROM GDA_LOG_TRANSACTION (NOLOCK) AS LT  ");
                //sb.Append("INNER JOIN GDA_TRANSACTION (NOLOCK) AS T ON LT.TRANSACTIONID = T.TRANSACTIONID  ");
                //sb.Append("LEFT JOIN  ");
                //sb.Append("(  ");
                //sb.Append("SELECT COUNT(0) AS SOMA, TRANSACTIONID FROM GDA_LOG_HISTORY_HIERARCHY  (NOLOCK)  ");
                //sb.Append(" WHERE DATE >= @DATAINICIAL  ");
                //sb.Append("  AND DATE <= @DATAFINAL  ");
                //sb.Append("GROUP BY TRANSACTIONID  ");
                //sb.Append(") AS R ON R.TRANSACTIONID = T.IDGDA_TRANSACTION   ");
                //sb.Append("WHERE CONVERT(DATE, LT.CREATED_AT) >= @DATAINICIAL  ");
                //sb.Append("AND CONVERT(DATE, LT.CREATED_AT) <= @DATAFINAL AND TYPE = 'HIERARCHY'  ");
                //sb.Append("GROUP BY T.IDGDA_TRANSACTION  ");
                sb.Append($"DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{dtinicial}'  ");
                sb.Append($"DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{dtfinal}'  ");
                sb.Append("SELECT COUNT(0) AS QTD, 'HIERARQUIA' AS TYPE, DATE AS DATA,  ");
                sb.Append("CASE WHEN COUNT(0) > 0 THEN 2 ELSE 3 END AS STATUS FROM GDA_LOG_HISTORY_HIERARCHY  (NOLOCK)   ");
                sb.Append(" WHERE CONVERT(DATE,DATE) >= CONVERT(DATE,@DATAINICIAL)   ");
                sb.Append("  AND CONVERT(DATE,DATE) <= CONVERT(DATE,@DATAFINAL)   ");
                sb.Append("GROUP BY DATE   ");

            }
            else if (type == "COLLABORATOR")
            {
                //sb.Append($"DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{dtinicial}'  ");
                //sb.Append($"DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{dtfinal}'  ");
                //sb.Append("SELECT ISNULL(MAX(SOMA), 0) AS QTD, MAX(LT.CREATED_AT) AS DATA, MAX(T.TRANSACTIONID) AS TRANSACTIONID,  ");
                //sb.Append("MAX(CASE WHEN LT.STATUS = 'START' THEN 1  ");
                //sb.Append("			WHEN LT.STATUS = 'CONCLUDED' THEN 2  ");
                //sb.Append("			ELSE 3 END) AS STATUS,  ");
                //sb.Append("			'COLABORADOR' AS TYPE  ");
                //sb.Append("FROM GDA_LOG_TRANSACTION (NOLOCK) AS LT  ");
                //sb.Append("INNER JOIN GDA_TRANSACTION (NOLOCK) AS T ON LT.TRANSACTIONID = T.TRANSACTIONID  ");
                //sb.Append("LEFT JOIN  ");
                //sb.Append("(  ");
                //sb.Append("SELECT COUNT(0) AS SOMA, TRANSACTIONID FROM GDA_HISTORY_COLLABORATORS   (NOLOCK)  ");
                //sb.Append(" WHERE ENTRYDATE >= @DATAINICIAL  ");
                //sb.Append(" AND ENTRYDATE <= @DATAFINAL  ");
                //sb.Append("GROUP BY TRANSACTIONID  ");
                //sb.Append(") AS R ON R.TRANSACTIONID = T.IDGDA_TRANSACTION   ");
                //sb.Append("WHERE CONVERT(DATE, LT.CREATED_AT) >= @DATAINICIAL  ");
                //sb.Append("AND CONVERT(DATE, LT.CREATED_AT) <= @DATAFINAL AND TYPE = 'COLLABORATOR'  ");
                //sb.Append("GROUP BY T.IDGDA_TRANSACTION  ");

                sb.Append($"DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{dtinicial}'  ");
                sb.Append($"DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{dtfinal}'  ");
                sb.Append("SELECT COUNT(0) AS QTD, 'COLABORADOR' AS TYPE, ENTRYDATE AS DATA, ");
                sb.Append("CASE WHEN COUNT(0) > 0 THEN 2 ELSE 3 END AS STATUS ");
                sb.Append(" FROM GDA_HISTORY_COLLABORATORS   (NOLOCK)   ");
                sb.Append(" WHERE CONVERT(DATE, ENTRYDATE) >= CONVERT(DATE,@DATAINICIAL)   ");
                sb.Append(" AND CONVERT(DATE, ENTRYDATE) <= CONVERT(DATE,@DATAFINAL)   ");
                sb.Append("GROUP BY ENTRYDATE   ");

            }
            else if (type == "INDICATOR")
            {
                //sb.Append($"DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{dtinicial}'  ");
                //sb.Append($"DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{dtfinal}'  ");
                //sb.Append("SELECT ISNULL(MAX(SOMA), 0) AS QTD, MAX(LT.CREATED_AT) AS DATA, MAX(T.TRANSACTIONID) AS TRANSACTIONID,  ");
                //sb.Append("MAX(CASE WHEN LT.STATUS = 'START' THEN 1  ");
                //sb.Append("			WHEN LT.STATUS = 'CONCLUDED' THEN 2  ");
                //sb.Append("			ELSE 3 END) AS STATUS,  ");
                //sb.Append("			'INDICADOR' AS TYPE  ");
                //sb.Append("FROM GDA_LOG_TRANSACTION (NOLOCK) AS LT  ");
                //sb.Append("INNER JOIN GDA_TRANSACTION (NOLOCK) AS T ON LT.TRANSACTIONID = T.TRANSACTIONID  ");
                //sb.Append("LEFT JOIN  ");
                //sb.Append("(  ");
                //sb.Append("SELECT COUNT(0) AS SOMA, TRANSACTIONID FROM GDA_INDICATOR    (NOLOCK)  ");
                //sb.Append("WHERE CREATED_AT >= @DATAINICIAL  ");
                //sb.Append("AND CREATED_AT <= @DATAFINAL  ");
                //sb.Append("GROUP BY TRANSACTIONID  ");
                //sb.Append(") AS R ON R.TRANSACTIONID = T.IDGDA_TRANSACTION   ");
                //sb.Append("WHERE CONVERT(DATE, LT.CREATED_AT) >= @DATAINICIAL  ");
                //sb.Append("AND CONVERT(DATE, LT.CREATED_AT) <= @DATAFINAL AND TYPE = 'INDICATOR'  ");
                //sb.Append("GROUP BY T.IDGDA_TRANSACTION  ");

                sb.Append($"DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{dtinicial}'  ");
                sb.Append($"DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{dtfinal}'  ");
                sb.Append("SELECT COUNT(0) AS QTD, CREATED_AT AS DATA,'INDICADOR' AS TYPE, ");
                sb.Append("CASE WHEN COUNT(0) > 0 THEN 2 ELSE 3 END AS STATUS ");
                sb.Append(" FROM GDA_INDICATOR    (NOLOCK)   ");
                sb.Append("WHERE CONVERT(DATE, CREATED_AT) >= CONVERT(DATE,@DATAINICIAL)   ");
                sb.Append("AND CONVERT(DATE, CREATED_AT) <= CONVERT(DATE, @DATAFINAL)   ");
                sb.Append("GROUP BY CREATED_AT  ");


            }
            else if (type == "RESULT")
            {
                //sb.Append($"DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{dtinicial}'  ");
                //sb.Append($"DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{dtfinal}'  ");
                //sb.Append("SELECT MAX(LT.CREATED_AT) AS DATA,  ");
                //sb.Append("	   'RESULTADO' AS TYPE,  ");
                //sb.Append("	    ISNULL(SUM(AMOUNT),0) AS QTD ,  ");
                //sb.Append("	   MAX(CASE  ");
                //sb.Append("            WHEN LT.STATUS = 'START' THEN 1  ");
                //sb.Append("            WHEN LT.STATUS = 'CONCLUDED' THEN 2  ");
                //sb.Append("            ELSE 3  ");
                //sb.Append("        END) AS STATUS,  ");
                //sb.Append("	   MAX(T.TRANSACTIONID) AS TRANSACTIONID  ");
                //sb.Append("FROM GDA_LOG_TRANSACTION (NOLOCK) AS LT  ");
                //sb.Append("INNER JOIN GDA_TRANSACTION (NOLOCK) AS T ON LT.TRANSACTIONID = T.TRANSACTIONID  ");
                //sb.Append("WHERE TYPE = 'RESULT'  ");
                //sb.Append("AND CONVERT(DATE, LT.CREATED_AT) >= @DATAINICIAL  ");
                //sb.Append("AND CONVERT(DATE, LT.CREATED_AT) <= @DATAFINAL  ");
                //sb.Append("GROUP BY LT.TRANSACTIONID  ");

                sb.Append($"DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{dtinicial}'  ");
                sb.Append($"DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{dtfinal}'  ");
                sb.Append("SELECT COUNT(0) AS QTD, CONVERT(DATE, INSERTED_AT) AS DATA, 'RESULTADO' AS TYPE, ");
                sb.Append("CASE WHEN COUNT(0) > 0 THEN 2 ELSE 3 END AS STATUS  ");
                sb.Append(" FROM GDA_RESULT (NOLOCK) ");
                sb.Append("WHERE CONVERT(DATE, INSERTED_AT) >= CONVERT(DATE, @DATAINICIAL) AND CONVERT(DATE, INSERTED_AT) <= CONVERT(DATE, @DATAFINAL) ");
                sb.Append("GROUP BY CONVERT(DATE, INSERTED_AT) ");

                //sb.Append($"DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{dtinicial}'  ");
                //sb.Append($"DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{dtfinal}'  ");
                //sb.Append("SELECT ISNULL(MAX(SOMA), 0) AS QTD, MAX(T.CREATED_AT) AS DATA, MAX(T.TRANSACTIONID) AS TRANSACTIONID,  ");
                //sb.Append("MAX(CASE WHEN LT.STATUS = 'START' THEN 1  ");
                //sb.Append("			WHEN LT.STATUS = 'CONCLUDED' THEN 2  ");
                //sb.Append("			ELSE 3 END) AS STATUS, ");
                //sb.Append("			'RESULT' AS TYPE  ");
                //sb.Append("FROM GDA_LOG_TRANSACTION (NOLOCK) AS LT  ");
                //sb.Append("INNER JOIN GDA_TRANSACTION (NOLOCK) AS T ON LT.TRANSACTIONID = T.TRANSACTIONID  ");
                //sb.Append("LEFT JOIN  ");
                //sb.Append("(  ");
                ////sb.Append("SELECT COUNT(0) AS SOMA, TRANSACTIONID FROM GDA_HISTORY_RESULT  (NOLOCK)  ");
                //sb.Append("SELECT COUNT(0) AS SOMA, DATE FROM GDA_HISTORY_RESULT  (NOLOCK)  ");
                //sb.Append("WHERE DATE >= @DATAINICIAL  ");
                //sb.Append("AND DATE <= @DATAFINAL  ");
                ////sb.Append("GROUP BY TRANSACTIONID  ");
                //sb.Append("GROUP BY DATE  ");
                //sb.Append(") AS R ON R.TRANSACTIONID = T.IDGDA_TRANSACTION   ");
                //sb.Append("WHERE CONVERT(DATE, LT.CREATED_AT) >= @DATAINICIAL  ");
                //sb.Append("AND CONVERT(DATE, LT.CREATED_AT) <= @DATAFINAL AND TYPE = 'RESULT'  ");
                //sb.Append("GROUP BY T.IDGDA_TRANSACTION  ");              
            }
            else if (type == "MONETIZATION")
            {
                //sb.Append($"DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{dtinicial}'  ");
                //sb.Append($"DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{dtfinal}'  ");
                //sb.Append("SELECT ISNULL(MAX(SOMA), 0) AS QTD, MAX(LT.CREATED_AT) AS DATA, MAX(T.TRANSACTIONID) AS TRANSACTIONID,  ");
                //sb.Append("MAX(CASE WHEN LT.STATUS = 'START' THEN 1  ");
                //sb.Append("			WHEN LT.STATUS = 'CONCLUDED' THEN 2  ");
                //sb.Append("			ELSE 3 END) AS STATUS,  ");
                //sb.Append("			'MONETIZACAO' AS TYPE  ");
                //sb.Append("FROM GDA_LOG_TRANSACTION (NOLOCK) AS LT  ");
                //sb.Append("INNER JOIN GDA_TRANSACTION (NOLOCK) AS T ON LT.TRANSACTIONID = T.TRANSACTIONID  ");
                //sb.Append("LEFT JOIN  ");
                //sb.Append("(  ");
                //sb.Append("SELECT COUNT(0) AS SOMA, TRANSACTIONID FROM GDA_CONSOLIDATE_CHECKING_ACCOUNT     (NOLOCK)  ");
                //sb.Append(" WHERE CREATED_AT >= @DATAINICIAL  ");
                //sb.Append(" AND CREATED_AT <= @DATAFINAL  ");
                //sb.Append("GROUP BY TRANSACTIONID  ");
                //sb.Append(") AS R ON R.TRANSACTIONID = T.IDGDA_TRANSACTION   ");
                //sb.Append("WHERE CONVERT(DATE, LT.CREATED_AT) >= @DATAINICIAL  ");
                //sb.Append("AND CONVERT(DATE, LT.CREATED_AT) <= @DATAFINAL AND TYPE = 'MONETIZATION'  ");
                //sb.Append("GROUP BY T.IDGDA_TRANSACTION  ");

                sb.Append($"DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{dtinicial}'  ");
                sb.Append($"DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{dtfinal}'  ");
                sb.Append("SELECT COUNT(0) AS QTD, CONVERT(DATE, RESULT_DATE) AS DATA, 'MONETIZACAO' AS TYPE, ");
                sb.Append("CASE WHEN COUNT(0) > 0 THEN 2 ELSE 3 END AS STATUS   ");
                sb.Append(" FROM GDA_CHECKING_ACCOUNT     (NOLOCK)   ");
                sb.Append("  WHERE CONVERT(DATE, RESULT_DATE) >= @DATAINICIAL   ");
                sb.Append("  AND CONVERT(DATE, RESULT_DATE) <= @DATAFINAL   ");
                sb.Append("  AND GDA_INDICATOR_IDGDA_INDICATOR IS NOT NULL ");
                sb.Append(" GROUP BY CONVERT(DATE, RESULT_DATE) ");

            }
            else if (type == "RESULT_CONSOLIDATED")
            {
                //sb.Append($"DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{dtinicial}'  ");
                //sb.Append($"DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{dtfinal}'  ");
                //sb.Append("SELECT ISNULL(MAX(SOMA), 0) AS QTD, MAX(LT.CREATED_AT) AS DATA, MAX(T.TRANSACTIONID) AS TRANSACTIONID,  ");
                //sb.Append("MAX(CASE WHEN LT.STATUS = 'START' THEN 1  ");
                //sb.Append("			WHEN LT.STATUS = 'CONCLUDED' THEN 2  ");
                //sb.Append("			ELSE 3 END) AS STATUS,  ");
                //sb.Append("			'RESULTADO_CONSOLIDADO' AS TYPE  ");
                //sb.Append("FROM GDA_LOG_TRANSACTION (NOLOCK) AS LT  ");
                //sb.Append("INNER JOIN GDA_TRANSACTION (NOLOCK) AS T ON LT.TRANSACTIONID = T.TRANSACTIONID  ");
                //sb.Append("LEFT JOIN  ");
                //sb.Append("(  ");
                //sb.Append("SELECT COUNT(0) AS SOMA, TRANSACTIONID FROM GDA_RESULT (NOLOCK)  ");
                //sb.Append(" WHERE CREATED_AT >=  @DATAINICIAL  ");
                //sb.Append("	AND CREATED_AT <= @DATAFINAL  ");
                //sb.Append("GROUP BY TRANSACTIONID  ");
                //sb.Append(") AS R ON R.TRANSACTIONID = T.IDGDA_TRANSACTION   ");
                //sb.Append("WHERE CONVERT(DATE, LT.CREATED_AT) >= @DATAINICIAL  ");
                //sb.Append("AND CONVERT(DATE, LT.CREATED_AT) <= @DATAFINAL AND TYPE = 'RESULT_CONSOLIDATED'  ");
                //sb.Append("GROUP BY T.IDGDA_TRANSACTION  ");

                //sb.Append($"DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{dtinicial}' ");
                //sb.Append($"DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{dtfinal}' ");
                //sb.Append("SELECT COUNT(0) AS QTD, CONVERT(DATE,INSERTED_AT) AS DATA, 'RESULTADO_CONSOLIDADO' AS TYPE,  ");
                //sb.Append("CASE WHEN COUNT(0) > 0 THEN 2 ELSE 3 END AS STATUS ");
                //sb.Append(" FROM GDA_RESULT (NOLOCK) ");
                //sb.Append(" WHERE CONVERT(DATE,INSERTED_AT) >=  CONVERT(DATE,@DATAINICIAL) ");
                //sb.Append("	AND CONVERT(DATE,INSERTED_AT) <= CONVERT(DATE,@DATAFINAL) ");
                //sb.Append("GROUP BY CONVERT(DATE,INSERTED_AT) ");


                sb.Append($"DECLARE @DATAINICIAL DATE; ");
                sb.Append($"SET @DATAINICIAL = '{dtinicial}'; ");
                sb.Append($" ");
                sb.Append($"DECLARE @DATAFINAL DATE; ");
                sb.Append($"SET @DATAFINAL = '{dtfinal}'; ");
                sb.Append($" ");
                sb.Append($"WITH DateRange AS ( ");
                sb.Append($"    SELECT @DATAINICIAL AS DateValue ");
                sb.Append($"    UNION ALL ");
                sb.Append($"    SELECT DATEADD(DAY, 1, DateValue) ");
                sb.Append($"    FROM DateRange ");
                sb.Append($"    WHERE DATEADD(DAY, 1, DateValue) <= @DATAFINAL ");
                sb.Append($") ");
                sb.Append($"SELECT  ");
                sb.Append($"    dr.DateValue AS DATA,  ");
                sb.Append($"    'RESULTADO_CONSOLIDADO' AS TYPE,   ");
                sb.Append($"    COUNT(0) AS QTD,  ");
                sb.Append($"    CASE WHEN COUNT(0) > 0 THEN 2 ELSE 3 END AS STATUS ");
                sb.Append($"FROM  ");
                sb.Append($"    DateRange dr ");
                sb.Append($"LEFT JOIN  ");
                sb.Append($"    GDA_RESULT gr (NOLOCK) ");
                sb.Append($"    ON CONVERT(DATE, COALESCE(gr.CREATED_AT, gr.INSERTED_AT)) = dr.DateValue ");
                sb.Append($"	OR (CREATED_AT IS NULL AND CONVERT(DATE, INSERTED_AT) = dr.DateValue) ");
                sb.Append($" ");
                sb.Append($"GROUP BY  ");
                sb.Append($"    dr.DateValue ");
                sb.Append($"ORDER BY  ");
                sb.Append($"    dr.DateValue ");
                sb.Append($"OPTION (MAXRECURSION 0); ");


            }
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                {
                    command.CommandTimeout = 0;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["DATA"].ToString() == "")
                            {
                                continue;
                            }
                            Type ResultType = new Type();
                            ResultType.DATA = reader["DATA"].ToString();
                            ResultType.TYPE = reader["TYPE"].ToString();
                            ResultType.QTD = Convert.ToInt32(reader["QTD"].ToString());
                            ResultType.STATUS = reader["STATUS"].ToString() == "2" ? "COMPLETO" : reader["STATUS"].ToString() == "3" && Convert.ToInt32(reader["QTD"].ToString()) == 0 ? "ERRO" : "INCOMPLETO";
                            ListType.Add(ResultType);
                        }
                    }
                }
                connection.Close();
            }



            return ListType;
        }

        public List<Type> verificaDatasVazias(string dtinicial, string dtfinal, List<Type> tp, string type)
        {
            List<Type> tprs = new List<Type>();

            DateTime startDate = DateTime.ParseExact(dtinicial, "yyyy-MM-dd", null);
            DateTime endDate = DateTime.ParseExact(dtfinal, "yyyy-MM-dd", null);
            string typeConv = "";

            if (type == "ATRIBUTES")
            {
                typeConv = "ATRIBUTOS";
            }
            else if (type == "HIERARCHY")
            {
                typeConv = "HIERARQUIA";
            }
            else if (type == "COLLABORATOR")
            {
                typeConv = "COLABORADOR";
            }
            else if (type == "INDICATOR")
            {
                typeConv = "INDICADOR";
            }
            else if (type == "RESULT")
            {
                typeConv = "RESULTADO";
            }
            else if (type == "MONETIZATION")
            {
                typeConv = "MONETIZACAO";
            }
            else if (type == "RESULT_CONSOLIDATED")
            {
                typeConv = "RESULTADO_CONSOLIDADO";
            }


            // Loop usando for
            for (DateTime currentDate = startDate; currentDate <= endDate; currentDate = currentDate.AddDays(1))
            {

                string dateAg = currentDate.ToString();

                if (tp.FindAll(i => i.DATA == dateAg).Count == 0)
                {
                    Type tpr = new Type();
                    tpr.DATA = currentDate.ToString("dd/MM/yyyy") + " 00:00:00";
                    tpr.TYPE = typeConv;
                    tpr.QTD = 0;
                    tpr.STATUS = "INCOMPLETO";
                    tprs.Add(tpr);
                }

            }

            tprs = tprs.Concat(tp).ToList();

            return tprs;
        }

        public List<Type> ReturnResultAPI(string dtinicial, string dtfinal)
        {
            var ATRIBUTES = ReturnInfoType("ATRIBUTES", dtinicial, dtfinal);
            ATRIBUTES = verificaDatasVazias(dtinicial, dtfinal, ATRIBUTES, "ATRIBUTES");

            var HIERARCHY = ReturnInfoType("HIERARCHY", dtinicial, dtfinal);
            HIERARCHY = verificaDatasVazias(dtinicial, dtfinal, HIERARCHY, "HIERARCHY");

            var COLLABORATOR = ReturnInfoType("COLLABORATOR", dtinicial, dtfinal);
            //COLLABORATOR = verificaDatasVazias(dtinicial, dtfinal, COLLABORATOR, "COLLABORATOR");

            var INDICATOR = ReturnInfoType("INDICATOR", dtinicial, dtfinal);
            //INDICATOR = verificaDatasVazias(dtinicial, dtfinal, INDICATOR, "INDICATOR");

            var RESULT = ReturnInfoType("RESULT", dtinicial, dtfinal);
            //RESULT = verificaDatasVazias(dtinicial, dtfinal, RESULT, "RESULT");

            //var MONETIZATION = ReturnInfoType("MONETIZATION", dtinicial, dtfinal);
            //MONETIZATION = verificaDatasVazias(dtinicial, dtfinal, MONETIZATION, "MONETIZATION");

            var RESULT_CONSOLIDATED = ReturnInfoType("RESULT_CONSOLIDATED", dtinicial, dtfinal);
            RESULT_CONSOLIDATED = verificaDatasVazias(dtinicial, dtfinal, RESULT_CONSOLIDATED, "RESULT_CONSOLIDATED");

            List<Type> ListIntegracaoAPI = new List<Type>();
            ListIntegracaoAPI = ListIntegracaoAPI.Concat(ATRIBUTES).Concat(HIERARCHY).Concat(COLLABORATOR).Concat(INDICATOR).Concat(RESULT).Concat(RESULT_CONSOLIDATED).ToList();


            ListIntegracaoAPI = ListIntegracaoAPI.OrderBy(i => i.TYPE).OrderBy(i => i.DATA).ToList();

            #region Comentado
            //StringBuilder stb = new StringBuilder();
            //stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0} 00:00:00'; ", dtinicial);
            //stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0} 23:59:59'; ", dtfinal);
            //stb.Append("SELECT  ");
            //stb.Append("    TYPE AS ROTA,  ");
            //stb.Append("    DATEADD(MINUTE, (DATEDIFF(MINUTE, 0, TT.CREATED_AT) / 60) * 60, 0) AS DATA,  ");
            //stb.Append("	CASE  ");
            //stb.Append("           WHEN TT.STATUS = 'CONCLUDED' THEN 'COMPLETO'  ");
            //stb.Append("           WHEN TT.STATUS <> 'START'  ");
            //stb.Append("                AND TT.STATUS <> 'CONCLUDED' THEN CONCAT('NÃO COMPLETO, ', TT.STATUS)  ");
            //stb.Append("       END AS 'STATUS',  ");
            //stb.Append("    COUNT(*) AS QTD_IMPORTACAO  ");
            //stb.Append("FROM  ");
            //stb.Append("    GDA_LOG_TRANSACTION TT (nolock)  ");
            //stb.Append("WHERE  ");
            //stb.Append("    STATUS <> 'START'  ");
            //stb.Append("    AND TT.CREATED_AT >= '2024-01-01 00:00:00'  ");
            //stb.Append("    AND TT.CREATED_AT <= '2024-01-30 23:59:59'  ");
            //stb.Append("GROUP BY  ");
            //stb.Append("    TYPE,  ");
            //stb.Append("	STATUS,  ");
            //stb.Append("    DATEADD(MINUTE, (DATEDIFF(MINUTE, 0, TT.CREATED_AT) / 60) * 60, 0)  ");
            //stb.Append("ORDER BY  ");
            //stb.Append("    DATA;  ");

            #region Query Antiga
            //stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtinicial);
            //stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtfinal);
            //stb.AppendFormat("SELECT  ");
            //stb.AppendFormat("CONVERT(DATE, TT.CREATED_AT) AS DATA, ");
            //stb.AppendFormat("MAX(TT.TYPE) AS TYPE, ");
            //stb.AppendFormat("CASE ");
            //stb.AppendFormat("	WHEN TT.STATUS = 'CONCLUDED' THEN 'COMPLETO' ");
            //stb.AppendFormat("	WHEN TT.STATUS <> 'START' AND TT.STATUS <> 'CONCLUDED' THEN CONCAT('NÃO COMPLETO, ', TT.STATUS) ");
            //stb.AppendFormat("	END AS 'STATUS', ");
            //stb.AppendFormat("CASE  ");
            //stb.AppendFormat("	WHEN TYPE = 'ATRIBUTES' AND TT.STATUS = 'CONCLUDED' THEN (SELECT COUNT(0) ");
            //stb.AppendFormat("								  FROM GDA_ATRIBUTES (NOLOCK) ");
            //stb.AppendFormat("								  WHERE CONVERT(DATE,CREATED_AT) >= CONVERT(DATE, TT.CREATED_AT) AND CONVERT(DATE,CREATED_AT) <= CONVERT(DATE, TT.CREATED_AT)) ");
            //stb.AppendFormat("	WHEN TYPE = 'HIERARCHY' AND TT.STATUS = 'CONCLUDED'  THEN (SELECT COUNT(0) ");
            //stb.AppendFormat("								  FROM  GDA_LOG_HISTORY_HIERARCHY (NOLOCK) ");
            //stb.AppendFormat("								  WHERE CONVERT(DATE,DATE) >= CONVERT(DATE, TT.CREATED_AT) AND CONVERT(DATE,DATE) <= CONVERT(DATE, TT.CREATED_AT)) ");
            //stb.AppendFormat("	WHEN TYPE = 'COLLABORATOR' AND TT.STATUS = 'CONCLUDED'  THEN (SELECT COUNT(0) ");
            //stb.AppendFormat("								  FROM GDA_HISTORY_COLLABORATORS (NOLOCK) ");
            //stb.AppendFormat("								  WHERE CONVERT(DATE,ENTRYDATE) >= CONVERT(DATE, TT.CREATED_AT) AND CONVERT(DATE,ENTRYDATE) <= CONVERT(DATE, TT.CREATED_AT) AND ACTIVE = 'TRUE') ");
            //stb.AppendFormat("	WHEN TYPE = 'INDICATOR' AND TT.STATUS = 'CONCLUDED'  THEN (SELECT COUNT (0) ");
            //stb.AppendFormat("							      FROM GDA_INDICATOR (NOLOCK) ");
            //stb.AppendFormat("								  WHERE CONVERT(DATE,CREATED_AT) >= CONVERT(DATE, TT.CREATED_AT) AND CONVERT(DATE,CREATED_AT) <= CONVERT(DATE, TT.CREATED_AT) AND DELETED_AT IS NULL) ");
            //stb.AppendFormat("	WHEN TYPE = 'RESULT' AND TT.STATUS = 'CONCLUDED' THEN (SELECT COUNT (0) ");
            //stb.AppendFormat("							      FROM GDA_RESULT (NOLOCK) ");
            //stb.AppendFormat("								  WHERE CONVERT(DATE,CREATED_AT) >= CONVERT(DATE, TT.CREATED_AT) AND CONVERT(DATE,CREATED_AT) <= CONVERT(DATE, TT.CREATED_AT)) ");
            //stb.AppendFormat("	WHEN TYPE = 'RESULT_CONSOLIDATED' AND TT.STATUS = 'CONCLUDED' THEN (SELECT COUNT (0) AS QTD_RESULT_CONSOLIDATED ");
            //stb.AppendFormat("							      FROM GDA_HISTORY_RESULT (NOLOCK) ");
            //stb.AppendFormat("								  WHERE CONVERT(DATE,DATE) >= CONVERT(DATE, TT.CREATED_AT) AND CONVERT(DATE,DATE) <= CONVERT(DATE, TT.CREATED_AT)) ");
            //stb.AppendFormat("	WHEN TYPE = 'MONETIZATION' AND TT.STATUS = 'CONCLUDED' THEN (SELECT COUNT (0) AS QTD_MONETIZATION ");
            //stb.AppendFormat("								  FROM GDA_CONSOLIDATE_CHECKING_ACCOUNT (NOLOCK) ");
            //stb.AppendFormat("								 WHERE CONVERT(DATE,CREATED_AT) >= CONVERT(DATE, TT.CREATED_AT) AND CONVERT(DATE,CREATED_AT) <= CONVERT(DATE, TT.CREATED_AT)) ");
            //stb.AppendFormat("ELSE '0' END AS QTD_IMPORTACAO ");
            //stb.AppendFormat("from GDA_LOG_TRANSACTION  TT (nolock) ");
            //stb.AppendFormat("where 1=1 ");
            //stb.AppendFormat("AND STATUS <> 'START' ");
            //stb.AppendFormat("AND CONVERT(DATE,TT.CREATED_AT) >= @DATAINICIAL ");
            //stb.AppendFormat("AND CONVERT(DATE,TT.CREATED_AT) <= @DATAFINAL ");
            //stb.AppendFormat("group by ");
            //stb.AppendFormat("type, ");
            //stb.AppendFormat("CONVERT(DATE, TT.CREATED_AT), ");
            //stb.AppendFormat("STATUS ");
            //stb.AppendFormat(" ");
            //stb.AppendFormat("order by max(TT.CREATED_AT)ASC ");
            #endregion
            //List<ResultAPI> ListApi = new List<ResultAPI>();

            //using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            //{
            //    connection.Open();
            //    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
            //    {
            //        using (SqlDataReader reader = command.ExecuteReader())
            //        {
            //            while (reader.Read())
            //            {
            //                ResultAPI API = new ResultAPI();
            //                API.DATA = reader["DATA"].ToString();
            //                API.TYPE = reader["ROTA"].ToString();
            //                API.STATUS = reader["STATUS"].ToString();
            //                API.QTD_IMPORTACAO = Convert.ToInt32(reader["QTD_IMPORTACAO"].ToString());
            //                ListApi.Add(API);
            //            }
            //        }
            //    }

            //    connection.Close();
            //}
            #endregion

            return ListIntegracaoAPI;
        }
        [HttpGet]
        public IHttpActionResult GetResultsModel(string dtInicial, string dtFinal)
        {
            //Setar Filtro de Resultados da API somente para um mes
            DateTime dtTimeInicial = DateTime.ParseExact(dtInicial, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dtTimeFinal = DateTime.ParseExact(dtFinal, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            TimeSpan diff = dtTimeFinal.Subtract(dtTimeInicial);
            int diferencaEmDias = (int)diff.TotalDays;
            if (diferencaEmDias > 31)
            {
                return BadRequest("Selecionar uma data de no maximo 1 mês!");
            }

            //Realiza a query que retorna todas as informações de resultados Importados da API.
            List<Type> rmams = new List<Type>();
            rmams = ReturnResultAPI(dtInicial, dtFinal);
            List<Type> retorno = new List<Type>();

            //A condição item.Any(i => i.STATUS == "COMPLETO") verifica se há algum item dentro do grupo com STATUS igual a "COMPLETO".
            //Se pelo menos um item tiver "COMPLETO", o STATUS será definido como "COMPLETO";
            //caso contrário, será definido como "NÃO COMPLETO". 
            retorno = rmams.GroupBy(d => new { d.TYPE, d.DATA }).Select(item => new Type
            {
                DATA = item.First().DATA,
                TYPE = item.First().TYPE,
                STATUS = item.First().STATUS,
                QTD = item.Sum(d => d.QTD),
            }).ToList();

            var jsonData = retorno.Select(item => new returnResponseDay
            {
                DATA = item.DATA,
                TYPE = item.TYPE,
                STATUS = item.STATUS,
                QTD = item.QTD,
            }).ToList();

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(jsonData);
        }
        // Método para serializar um DataTable em JSON
    }
}