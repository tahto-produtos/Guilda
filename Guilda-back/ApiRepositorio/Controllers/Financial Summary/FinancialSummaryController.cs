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
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Threading;
using ThirdParty.Json.LitJson;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Bibliography;
using System.Runtime.ConstrainedExecution;
using CommandLine;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class FinancialSummaryController : ApiController
    {
        public class InputModel
        {
            public DateTime DataInicial { get; set; }
            public DateTime DataFinal { get; set; }
        }
        public class returnResponseFinancialSummary
        {
            public string DATA_FECHAMENTO { get; set; }
            public string CODIGO_PRODUTO { get; set; }
            public string PRODUTO { get; set; }
            public string ESTOQUE { get; set; }
            public string SAIDA_ATUAL { get; set; }
            public string QTD_ESTOQUE { get; set; }
            public string MOVIMENTACAO_ESTOQUE { get; set; }
            public string ENTRADA { get; set; }
            public string LIBERADO_ENTREGA { get; set; }
            public string PEDIDO { get; set; }
            public string CANCELADO { get; set; }
            public string EXPIRADO { get; set; }
            public string PRATELEIRA { get; set; }
            public string FECHAMENTO_ESTOQUE { get; set; }  
            public string VALIDADOR {  get; set; }

        }
        public static void InsertFinancialSummary()
        {
            using (SqlConnection connection = new SqlConnection(Database.retornaConn(true)))
            {
                try
                {
                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    //INSERE OS VALORES DE FECHAMENTO DO CAIXA NA TABELA.
                    sb.Append("INSERT INTO GDA_FINANCIAL_CLOSE (CREATED_AT, GDA_PRODUCT_IDGDA_PRODUCT, GDA_STOCK_IDGDA_STOCK, EXIT_QUANTITY, STOCK_QUANTITY, STOCK_MOVEMENT, ENTRY_QUANTITY, CLEARED_DELIVERY, REQUEST_QUANTITY, CANCELED_QUANTITY, EXPIRED_QUANTITY)  ");
                    sb.Append("SELECT   ");
                    sb.Append("	   CONVERT(DATE,DATEADD(day, DATEDIFF(day, 0, GETDATE()), -1)) AS DATA_FECHAMENTO,  ");
                    sb.Append("	   GP.IDGDA_PRODUCT AS IDPRODUCT,  ");
                    sb.Append("	   ST.IDGDA_STOCK AS IDSTOCK,  ");
                    sb.Append("	   ISNULL(SA.SAIDA,0)AS SAIDA,  ");
                    sb.Append("	   ISNULL(SUM(HSP.AMOUNT_INPUT)-SUM(HSP.AMOUNT_OUTPUT),0) AS QT_EM_ESTOQUE,  ");
                    sb.Append("	   ISNULL(SA.MOVIMENTACAO_ESTOQUE,0) AS MOVIMENTACAO_ESTOQUE,  ");
                    sb.Append("	   ISNULL(SA.ENTRADA,0) AS ENTRADA,  ");
                    sb.Append("	   ISNULL(OP.LIBERADO_ENTREGA,0) AS LIBERADO_ENTREGA,  ");
                    sb.Append("	   ISNULL(SA.PEDIDO,0) AS PEDIDO,  ");
                    sb.Append("	   ISNULL(SB.CANCELADO,0) AS CANCELADO,  ");
                    sb.Append("	   ISNULL(SA.EXPIRADO,0) AS EXPIRADO  ");
                    sb.Append("FROM GDA_HISTORY_STOCK_PRODUCT (NOLOCK) HSP    ");
                    sb.Append("INNER JOIN GDA_PRODUCT GP (NOLOCK) ON GP.IDGDA_PRODUCT  = HSP.GDA_PRODUCT_IDGDA_PRODUCT AND GP.DELETED_AT IS NULL  ");
                    sb.Append("LEFT JOIN GDA_STOCK ST (NOLOCK) ON ST.IDGDA_STOCK = HSP.GDA_STOCK_IDGDA_STOCK  ");
                    sb.Append("LEFT JOIN (SELECT  ");
                    sb.Append("				GDA_PRODUCT_IDGDA_PRODUCT,  ");
                    sb.Append("				GDA_STOCK_IDGDA_STOCK,  ");
                    sb.Append("				ISNULL(SUM(CASE WHEN ORDER_PRODUCT_STATUS = 'DELIVERED' THEN 1 ELSE 0 END),0) AS LIBERADO_ENTREGA   ");
                    sb.Append("			FROM GDA_ORDER_PRODUCT (NOLOCK)  ");
                    sb.Append("			WHERE DATEADD(day, DATEDIFF(day, 0, CREATED_AT), 0) = DATEADD(day, DATEDIFF(day, 0, GETDATE()), -1)  ");
                    sb.Append("			 GROUP BY GDA_PRODUCT_IDGDA_PRODUCT,  ");
                    sb.Append("					  GDA_STOCK_IDGDA_STOCK   ");
                    sb.Append("		    )AS OP ON OP.GDA_PRODUCT_IDGDA_PRODUCT = HSP.GDA_PRODUCT_IDGDA_PRODUCT AND OP.GDA_STOCK_IDGDA_STOCK = HSP.GDA_STOCK_IDGDA_STOCK  ");
                    sb.Append("LEFT JOIN (SELECT   ");
                    sb.Append("				GDA_PRODUCT_IDGDA_PRODUCT,    ");
                    sb.Append("             GDA_STOCK_IDGDA_STOCK, ");
                    sb.Append("				SUM(AMOUNT_OUTPUT) AS SAIDA,  ");
                    sb.Append("				ISNULL(SUM(CASE WHEN GDA_REASON_REMOVED_IDGDA_REASON_REMOVED = 6 THEN 1 ELSE 0 END),0) AS MOVIMENTACAO_ESTOQUE,  ");
                    sb.Append("				ISNULL(SUM(CASE WHEN GDA_REASON_REMOVED_IDGDA_REASON_REMOVED = 4 THEN 1 ELSE 0 END),0) AS ENTRADA,  ");
                    sb.Append("				ISNULL(SUM(CASE WHEN GDA_REASON_REMOVED_IDGDA_REASON_REMOVED = 9 THEN 1 ELSE 0 END),0) AS PEDIDO,  ");
                    sb.Append("				ISNULL(SUM(CASE WHEN GDA_REASON_REMOVED_IDGDA_REASON_REMOVED = 2 THEN 1 ELSE 0 END),0) AS EXPIRADO  ");
                    sb.Append("			FROM GDA_HISTORY_STOCK_PRODUCT WITH (NOLOCK)  ");
                    sb.Append("			WHERE DATEADD(day, DATEDIFF(day, 0, CREATED_AT), 0) = DATEADD(day, DATEDIFF(day, 0, GETDATE()), -1)  ");
                    sb.Append("			 GROUP BY GDA_PRODUCT_IDGDA_PRODUCT, GDA_STOCK_IDGDA_STOCK  ");
                    sb.Append("		  ) AS SA ON SA.GDA_PRODUCT_IDGDA_PRODUCT = HSP.GDA_PRODUCT_IDGDA_PRODUCT AND  SA.GDA_STOCK_IDGDA_STOCK = HSP.GDA_STOCK_IDGDA_STOCK  ");
                    sb.Append("LEFT JOIN (SELECT  ");
                    sb.Append("				GDA_PRODUCT_IDGDA_PRODUCT,  ");
                    sb.Append("             GDA_STOCK_IDGDA_STOCK, ");
                    sb.Append("				ISNULL(SUM(AMOUNT_INPUT),0) AS CANCELADO  ");
                    sb.Append("		   FROM GDA_HISTORY_STOCK_PRODUCT WITH (NOLOCK)  ");
                    sb.Append("		   WHERE GDA_REASON_REMOVED_IDGDA_REASON_REMOVED = 10  ");
                    sb.Append("		   AND DATEADD(day, DATEDIFF(day, 0, CREATED_AT), 0) = DATEADD(day, DATEDIFF(day, 0, GETDATE()), -1)  ");
                    sb.Append("		   GROUP BY GDA_PRODUCT_IDGDA_PRODUCT,GDA_STOCK_IDGDA_STOCK  ");
                    sb.Append("		   ) AS SB ON SB.GDA_PRODUCT_IDGDA_PRODUCT = HSP.GDA_PRODUCT_IDGDA_PRODUCT AND  SB.GDA_STOCK_IDGDA_STOCK = HSP.GDA_STOCK_IDGDA_STOCK  ");
                    sb.Append("WHERE 1=1    ");
                    sb.Append("AND DATEADD(day, DATEDIFF(day, 0, HSP.CREATED_AT), 0) <= DATEADD(day, DATEDIFF(day, 0, GETDATE()), -1)  ");
                    sb.Append("GROUP BY  ");
                    sb.Append("		ST.IDGDA_STOCK,  ");
                    sb.Append("		GP.IDGDA_PRODUCT,  ");
                    sb.Append("		ST.DESCRIPTION,  ");
                    sb.Append("		SA.MOVIMENTACAO_ESTOQUE,  ");
                    sb.Append("		SA.ENTRADA,  ");
                    sb.Append("		SA.PEDIDO,  ");
                    sb.Append("		SB.CANCELADO,  ");
                    sb.Append("		SA.EXPIRADO,  ");
                    sb.Append("		SA.SAIDA,  ");
                    sb.Append("		OP.LIBERADO_ENTREGA  ");
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {
                 
                }
                connection.Close();
            }
        }
        public static List<returnResponseFinancialSummary> ReturnFinancialSummary(string dtInicial, string dtFinal, int collaboratorId, bool Thread = false)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtInicial);
            sb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFinal);
            sb.Append("SELECT  ");
            sb.Append("	  CONVERT(DATE,FC.CREATED_AT) AS DATA_FECHAMENTO,  ");
            sb.Append("	  PT.CODE AS CODIGO_PRODUTO,  ");
            sb.Append("	  PT.COMERCIAL_NAME AS PRODUTO,  ");
            sb.Append("	  ST.DESCRIPTION AS ESTOQUE,  ");
            sb.Append("	  EXIT_QUANTITY AS SAIDA_ATUAL,  ");
            sb.Append("	  STOCK_QUANTITY AS QTD_ESTOQUE,  ");
            sb.Append("	  STOCK_MOVEMENT AS MOVIMENTACAO_ESTOQUE,  ");
            sb.Append("	  ENTRY_QUANTITY AS ENTRADA,  ");
            sb.Append("	  CLEARED_DELIVERY AS LIBERADO_ENTREGA,  ");
            sb.Append("	  REQUEST_QUANTITY AS PEDIDO,  ");
            sb.Append("	  CANCELED_QUANTITY AS CANCELADO,  ");
            sb.Append("	  EXPIRED_QUANTITY AS EXPIRADO,  ");
            sb.Append("   STOCK_QUANTITY AS FECHAMENTO_ESTOQUE, ");
            sb.Append($"   (SELECT NAME FROM GDA_COLLABORATORS (NOLOCK) WHERE IDGDA_COLLABORATORS = {collaboratorId}) AS VALIDADOR ");
            sb.Append("FROM GDA_FINANCIAL_CLOSE FC (NOLOCK)  ");
            sb.Append("INNER JOIN GDA_PRODUCT PT (NOLOCK) ON PT.IDGDA_PRODUCT = FC.GDA_PRODUCT_IDGDA_PRODUCT  ");
            sb.Append("INNER JOIN GDA_STOCK ST (NOLOCK) ON ST.IDGDA_STOCK = FC.GDA_STOCK_IDGDA_STOCK  ");
            sb.Append("WHERE  ");
            sb.Append("1=1  ");
            sb.Append("AND FC.CREATED_AT >= @DATAINICIAL  ");
            sb.Append("AND FC.CREATED_AT <= @DATAFINAL  ");

            List<returnResponseFinancialSummary> rmams = new List<returnResponseFinancialSummary>();
            using (SqlConnection connection = new SqlConnection(Database.retornaConn(Thread)))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                {
                    command.CommandTimeout = 300;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            returnResponseFinancialSummary rmam = new returnResponseFinancialSummary();

                            rmam.DATA_FECHAMENTO = reader["DATA_FECHAMENTO"].ToString();
                            rmam.CODIGO_PRODUTO = reader["CODIGO_PRODUTO"].ToString();
                            rmam.PRODUTO = reader["PRODUTO"].ToString();
                            rmam.ESTOQUE = reader["ESTOQUE"].ToString();
                            rmam.SAIDA_ATUAL = reader["SAIDA_ATUAL"].ToString();
                            rmam.QTD_ESTOQUE = reader["QTD_ESTOQUE"].ToString();
                            rmam.MOVIMENTACAO_ESTOQUE = reader["MOVIMENTACAO_ESTOQUE"].ToString();
                            rmam.ENTRADA = reader["ENTRADA"].ToString();
                            rmam.LIBERADO_ENTREGA = reader["LIBERADO_ENTREGA"].ToString();
                            rmam.PEDIDO = reader["PEDIDO"].ToString();
                            rmam.CANCELADO = reader["CANCELADO"].ToString();
                            rmam.EXPIRADO = reader["EXPIRADO"].ToString();
                            rmam.FECHAMENTO_ESTOQUE = reader["FECHAMENTO_ESTOQUE"].ToString();
                            rmam.PRATELEIRA = reader["FECHAMENTO_ESTOQUE"].ToString();
                            rmam.VALIDADOR = reader["VALIDADOR"].ToString();
                            rmams.Add(rmam);
                        }
                    }
                }
                connection.Close();
            }
            return rmams;
        }
        public static List<returnResponseFinancialSummary> RelFinancialSummary(string dtInicial, string dtFinal, int collaboratorId,bool Thread = false)
        {
            //REALIZA A QUERY QUE RETORNA TODAS AS INFORMAÇÕES DE ENTRADA E SAIDA DO ESTOQUE.
            List<returnResponseFinancialSummary> rmams = new List<returnResponseFinancialSummary>();
            rmams = ReturnFinancialSummary(dtInicial, dtFinal, collaboratorId);


            List<returnResponseFinancialSummary> AgrupamentoPorData = new List<returnResponseFinancialSummary>();
            AgrupamentoPorData = rmams.GroupBy(item => new { item.DATA_FECHAMENTO}).Select(grupo => new returnResponseFinancialSummary
            {
                PRODUTO = "SubTotal",
                ESTOQUE = "SubTotal",
                SAIDA_ATUAL = grupo.Sum(item => Convert.ToInt32(item.SAIDA_ATUAL)).ToString(),
                QTD_ESTOQUE = grupo.Sum(item => Convert.ToInt32(item.QTD_ESTOQUE)).ToString(),
                MOVIMENTACAO_ESTOQUE = grupo.Sum(item => Convert.ToInt32(item.MOVIMENTACAO_ESTOQUE)).ToString(),
                ENTRADA = grupo.Sum(item => Convert.ToInt32(item.ENTRADA)).ToString(),
                LIBERADO_ENTREGA = grupo.Sum(item => Convert.ToInt32(item.LIBERADO_ENTREGA)).ToString(),
                PEDIDO = grupo.Sum(item => Convert.ToInt32(item.PEDIDO)).ToString(),
                CANCELADO = grupo.Sum(item => Convert.ToInt32(item.CANCELADO)).ToString(),
                EXPIRADO = grupo.Sum(item => Convert.ToInt32(item.EXPIRADO)).ToString(),

            }).ToList();

            List<returnResponseFinancialSummary> AgrupamentoTotal = new List<returnResponseFinancialSummary>();
            AgrupamentoTotal = AgrupamentoPorData.Select(grupo => new returnResponseFinancialSummary
            {
                PRODUTO = "Total",
                ESTOQUE = "Total",
                SAIDA_ATUAL = rmams.Sum(item => Convert.ToInt32(item.SAIDA_ATUAL)).ToString(),
                QTD_ESTOQUE = rmams.Sum(item => Convert.ToInt32(item.QTD_ESTOQUE)).ToString(),
                MOVIMENTACAO_ESTOQUE = rmams.Sum(item => Convert.ToInt32(item.MOVIMENTACAO_ESTOQUE)).ToString(),
                ENTRADA = rmams.Sum(item => Convert.ToInt32(item.ENTRADA)).ToString(),
                LIBERADO_ENTREGA = rmams.Sum(item => Convert.ToInt32(item.LIBERADO_ENTREGA)).ToString(),
                PEDIDO = rmams.Sum(item => Convert.ToInt32(item.PEDIDO)).ToString(),
                CANCELADO = rmams.Sum(item => Convert.ToInt32(item.CANCELADO)).ToString(),
                EXPIRADO = rmams.Sum(item => Convert.ToInt32(item.EXPIRADO)).ToString(),
            }).ToList();

            rmams = rmams.Concat(AgrupamentoPorData).Concat(AgrupamentoTotal).ToList();

            return rmams;
        }

        // POST: api/Results
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {
            int collaboratorId = 0;
            int personaid = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            collaboratorId = inf.collaboratorId;
            personaid = inf.personauserId;

            string dtInicial = inputModel.DataInicial.ToString("yyyy-MM-dd");
            string dtFinal = inputModel.DataFinal.ToString("yyyy-MM-dd");      
            DateTime dtTimeInicial = DateTime.ParseExact(dtInicial, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dtTimeFinal = DateTime.ParseExact(dtFinal, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            TimeSpan diff = dtTimeFinal.Subtract(dtTimeInicial);
            int diferencaEmDias = (int)diff.TotalDays;
            if (diferencaEmDias > 31)
            {
                return BadRequest("Selecionar uma data de no maximo 1 mês!");
            }
            var jsonData = RelFinancialSummary(dtInicial, dtFinal, collaboratorId);
            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(jsonData);
        }

        // Método para serializar um DataTable em JSON

    }
}