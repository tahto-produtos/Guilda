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
using System.Net.NetworkInformation;
using System.Web.UI;
using System.Xml.Linq;
using CommandLine;
using DocumentFormat.OpenXml.Spreadsheet;
using static ApiRepositorio.Controllers.FinancialSummaryController;
using static ApiRepositorio.Controllers.SearchAccountsController;
using Azure.Storage.Blobs.Specialized;
using static ApiRepositorio.Controllers.BuyItemController;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using DocumentFormat.OpenXml.Math;
using System.Drawing;
using System.Threading;
using static ApiRepositorio.Controllers.ProductVisibilityController;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{

    //[Authorize]
    public class ExportManageInventoryController : ApiController
    {// POST: api/Results
        public class inputExportManage
        {
            public string startDate { get; set; }
            public string endDate { get; set; }
            public int? productId { get; set; }
            public bool bestSeller { get; set; }
            public bool lessSold { get; set; }
            public string type { get; set; }

        }

        public class returnExportManage
        {
            public string startDate { get; set; }
            public string endDate { get; set; }
            public string product { get; set; }
            public StockExportManage stock { get; set; }
            public string city { get; set; }
            public int inicial_amount { get; set; }
            public int pending { get; set; }
            public int sold { get; set; }
            public int canceled { get; set; }
            public int expired { get; set; }
            public int quantity { get; set; }
            public int amount_input { get; set; }
            public int amount_output { get; set; }
            public int released { get; set; }
            public int added { get; set; }
            public int removed { get; set; }
            public string reason { get; set; }
            public string code { get; set; }
            public string registeredBy { get; set; }
            public decimal price { get; set; }
            public string type { get; set; }
            public string updatedAt { get; set; }
            public string publicationDate { get; set; }
            public string expirationDateVoucher { get; set; }
            public string expirationDate { get; set; }
            public string status { get; set; }
            public string createdAt { get; set; }
            public string visibility { get; set; }
            public int final_balance { get; set; }
        }

        public class StockExportManage
        {
            public int id { get; set; }
            public string description { get; set; }
            public DateTime? createdAt { get; set; }
            public DateTime? deletedAt { get; set; }
            public string city { get; set; }
            public string client { get; set; }
            public string campaign { get; set; }
            public string type { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] inputExportManage inputModel)
        {
            //int collaboratorId = 0;
            //int personauserId = 0;
            //var token = Request.Headers.Authorization?.Parameter;
            //bool tokn = TokenService.TryDecodeToken(token);
            //if (tokn == false)
            //{
            //    return Unauthorized();
            //}

            //collaboratorId = TokenService.InfsUsers.collaboratorId;

            List<returnExportManage> rems = new List<returnExportManage>();

            rems = BankExportManage.returnListStockProduct(inputModel);

            return Ok(rems);
        }

        public class BankExportManage
        {
            public static List<returnExportManage> returnListStockProduct(ExportManageInventoryController.inputExportManage inputModel)
            {
                List<returnExportManage> rems = new List<returnExportManage>();


                int retorno = 0;
                string filter = "";
                if (inputModel.productId != null)
                {
                    filter = $"{filter} AND HSP.GDA_PRODUCT_IDGDA_PRODUCT = {inputModel.productId} ";
                }
                if (inputModel.type != null)
                {
                    filter = $"{filter} AND S.TYPE = '{inputModel.type}' ";
                }

                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("WITH OrderData AS ( ");
                stb.AppendFormat("    SELECT  ");
                stb.AppendFormat("		GDA_PRODUCT_IDGDA_PRODUCT, ");
                stb.AppendFormat("        GDA_STOCK_IDGDA_STOCK,  ");
                stb.AppendFormat("        SUM(AMOUNT) AS SUM_AMOUNT,  ");
                stb.AppendFormat("        ORDER_PRODUCT_STATUS ");
                stb.AppendFormat("    FROM GDA_ORDER (NOLOCK) AS o ");
                stb.AppendFormat("    INNER JOIN GDA_ORDER_PRODUCT (NOLOCK) AS op  ");
                stb.AppendFormat("        ON o.IDGDA_ORDER = op.GDA_ORDER_IDGDA_ORDER ");
                stb.AppendFormat("    GROUP BY GDA_STOCK_IDGDA_STOCK, ORDER_PRODUCT_STATUS, GDA_PRODUCT_IDGDA_PRODUCT ");
                stb.AppendFormat(") ");
                stb.AppendFormat("SELECT  ");
                stb.AppendFormat("    hsp.GDA_PRODUCT_IDGDA_PRODUCT,  ");
                stb.AppendFormat("	MAX(p.COMERCIAL_NAME) AS PRODUTO, ");
                stb.AppendFormat("	MAX(p.CODE) AS CODIGO_DO_PRODUTO, ");
                stb.AppendFormat("	MAX(p.CREATED_AT) AS DATA_DE_CADASTRO, ");

                stb.AppendFormat("	MAX(S.CREATED_AT) AS DATA_CRIACAO_ESTOQUE, ");
                stb.AppendFormat("	MAX(S.DELETED_AT) AS DATA_DELETE_ESTOQUE, ");
                stb.AppendFormat("	MAX(S.CITY) AS CIDADE_ESTOQUE, ");
                stb.AppendFormat("	MAX(S.GDA_ATRIBUTES) AS CLIENTE_ESTOQUE, ");
                stb.AppendFormat("	MAX(S.CAMPAIGN) AS CAMPANHA_ESTOQUE, ");
                stb.AppendFormat("	MAX(S.TYPE) AS TIPO_ESTOQUE, ");


                stb.AppendFormat("    HSP.GDA_STOCK_IDGDA_STOCK, ");
                stb.AppendFormat("    SUM(CASE WHEN hsp.GDA_REASON_REMOVED_IDGDA_REASON_REMOVED = 4 THEN AMOUNT_INPUT ELSE 0 END) AS SALDO_INICIAL, ");
                stb.AppendFormat("    SUM(AMOUNT_OUTPUT) AS SAIDA, ");
                stb.AppendFormat("    SUM(CASE WHEN hsp.GDA_REASON_REMOVED_IDGDA_REASON_REMOVED <> 4 THEN AMOUNT_INPUT ELSE 0 END) AS ENTRADA, ");
                stb.AppendFormat("	SUM(CASE WHEN hsp.GDA_REASON_REMOVED_IDGDA_REASON_REMOVED = 6 THEN AMOUNT_OUTPUT ELSE 0 END) AS TRANSFERIDO, ");
                stb.AppendFormat("	MAX(DELIVERED.SUM_AMOUNT) AS FOI_RETIRADO, ");
                stb.AppendFormat("	MAX(ORDERED.SUM_AMOUNT) AS PEDIDO, ");
                stb.AppendFormat("    SUM(AMOUNT_INPUT) - SUM(AMOUNT_OUTPUT) AS SALDO_FINAL, ");
                stb.AppendFormat("	MAX(CANCELED.SUM_AMOUNT) AS CANCELADO, ");
                stb.AppendFormat("	MAX(EXPIRED.SUM_AMOUNT) AS EXPIRADO, ");

                stb.AppendFormat("  MAX(RELEASED.SUM_AMOUNT) AS LIBERADO, ");

                stb.AppendFormat("	MAX(P.PRICE) AS VALOR_EM_MOEDAS, ");
                stb.AppendFormat("	'' AS VALOR_RM_RS, ");
                stb.AppendFormat("	'' AS VALOR_EM_RS_ESTOQUE, ");
                stb.AppendFormat("	MAX(S.DESCRIPTION) AS NOME_ESTOQUE, ");
                stb.AppendFormat("	SUM(AMOUNT_INPUT) - SUM(AMOUNT_OUTPUT) AS EM_ESTOQUE, ");
                stb.AppendFormat("	MAX(P.VALIDITY_DATE) AS VENCIMENTO, ");
                stb.AppendFormat("	MAX(S.TYPE) AS TIPO_DE_ESTOQUE, ");
                stb.AppendFormat("	MAX(C.NAME) AS QUEM_CADASTROU, ");
                stb.AppendFormat("	MAX(P.UPDATED_AT) AS DATA_ATUALIZACAO, ");
                stb.AppendFormat("	MAX(CASE WHEN P.GDA_PRODUCT_STATUS_IDGDA_PRODUCT_STATUS = 1 THEN 'ATIVO' ELSE 'INATIVO' END) AS STATUS ");
                stb.AppendFormat(" ");
                stb.AppendFormat("FROM GDA_HISTORY_STOCK_PRODUCT (NOLOCK) AS hsp ");
                stb.AppendFormat("INNER JOIN GDA_PRODUCT (NOLOCK) AS p  ");
                stb.AppendFormat("    ON p.IDGDA_PRODUCT = hsp.GDA_PRODUCT_IDGDA_PRODUCT ");
                stb.AppendFormat("INNER JOIN GDA_COLLABORATORS (NOLOCK) AS C ");
                stb.AppendFormat("	ON P.REGISTERED_BY = C.IDGDA_COLLABORATORS ");
                stb.AppendFormat("INNER JOIN GDA_STOCK (NOLOCK) AS S ");
                stb.AppendFormat("	ON S.IDGDA_STOCK = HSP.GDA_STOCK_IDGDA_STOCK ");
                stb.AppendFormat("LEFT JOIN ORDERDATA AS DELIVERED  ");
                stb.AppendFormat("    ON hsp.GDA_STOCK_IDGDA_STOCK = DELIVERED.GDA_STOCK_IDGDA_STOCK  ");
                stb.AppendFormat("	AND DELIVERED.ORDER_PRODUCT_STATUS = 'DELIVERED' ");
                stb.AppendFormat("	AND DELIVERED.GDA_PRODUCT_IDGDA_PRODUCT = HSP.GDA_PRODUCT_IDGDA_PRODUCT ");
                stb.AppendFormat("LEFT JOIN ORDERDATA AS ORDERED  ");
                stb.AppendFormat("    ON hsp.GDA_STOCK_IDGDA_STOCK = ORDERED.GDA_STOCK_IDGDA_STOCK  ");
                stb.AppendFormat("	AND ORDERED.ORDER_PRODUCT_STATUS = 'ORDERED' ");
                stb.AppendFormat("	AND ORDERED.GDA_PRODUCT_IDGDA_PRODUCT = HSP.GDA_PRODUCT_IDGDA_PRODUCT ");
                stb.AppendFormat("LEFT JOIN ORDERDATA AS CANCELED  ");
                stb.AppendFormat("    ON hsp.GDA_STOCK_IDGDA_STOCK = CANCELED.GDA_STOCK_IDGDA_STOCK  ");
                stb.AppendFormat("	AND CANCELED.ORDER_PRODUCT_STATUS = 'CANCELED' ");
                stb.AppendFormat("	AND CANCELED.GDA_PRODUCT_IDGDA_PRODUCT = HSP.GDA_PRODUCT_IDGDA_PRODUCT ");
                stb.AppendFormat("LEFT JOIN ORDERDATA AS EXPIRED  ");
                stb.AppendFormat("    ON hsp.GDA_STOCK_IDGDA_STOCK = EXPIRED.GDA_STOCK_IDGDA_STOCK  ");
                stb.AppendFormat("	AND EXPIRED.ORDER_PRODUCT_STATUS = 'EXPIRED' ");
                stb.AppendFormat("	AND EXPIRED.GDA_PRODUCT_IDGDA_PRODUCT = HSP.GDA_PRODUCT_IDGDA_PRODUCT ");

                stb.AppendFormat("LEFT JOIN ORDERDATA AS RELEASED  ");
                stb.AppendFormat("  ON hsp.GDA_STOCK_IDGDA_STOCK = RELEASED.GDA_STOCK_IDGDA_STOCK  ");
                stb.AppendFormat("  AND RELEASED.ORDER_PRODUCT_STATUS = 'RELEASED' ");
                stb.AppendFormat("  AND RELEASED.GDA_PRODUCT_IDGDA_PRODUCT = HSP.GDA_PRODUCT_IDGDA_PRODUCT ");

                stb.AppendFormat($"WHERE 1 = 1 {filter} ");
                stb.AppendFormat("GROUP BY  ");
                stb.AppendFormat("    hsp.GDA_PRODUCT_IDGDA_PRODUCT,  ");
                stb.AppendFormat("    hsp.GDA_STOCK_IDGDA_STOCK ");


                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    returnExportManage rem = new returnExportManage();
                                    rem.stock = new StockExportManage();

                                    rem.startDate = inputModel.startDate;
                                    rem.endDate = inputModel.endDate;
                                    rem.product = reader["PRODUTO"].ToString();
                                    rem.stock.id = Convert.ToInt32(reader["GDA_STOCK_IDGDA_STOCK"].ToString());
                                    rem.stock.description = reader["NOME_ESTOQUE"].ToString();
                                    rem.stock.createdAt = Convert.ToDateTime(reader["DATA_CRIACAO_ESTOQUE"].ToString());
                                    rem.stock.deletedAt = reader["DATA_DELETE_ESTOQUE"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_DELETE_ESTOQUE"].ToString()) : (DateTime?)null;
                                    rem.stock.city = reader["CIDADE_ESTOQUE"].ToString();
                                    rem.stock.client = reader["CLIENTE_ESTOQUE"].ToString();
                                    rem.stock.campaign = reader["CAMPANHA_ESTOQUE"].ToString();
                                    rem.stock.type = reader["TIPO_ESTOQUE"].ToString();
                                    rem.city = reader["CIDADE_ESTOQUE"].ToString();
                                    rem.inicial_amount = reader["SALDO_INICIAL"] != DBNull.Value ? Convert.ToInt32(reader["SALDO_INICIAL"].ToString()) : 0;
                                    rem.pending = reader["PEDIDO"] != DBNull.Value ? Convert.ToInt32(reader["PEDIDO"].ToString()) : 0;
                                    rem.sold = reader["FOI_RETIRADO"] != DBNull.Value ? Convert.ToInt32(reader["FOI_RETIRADO"].ToString()) : 0;
                                    rem.canceled = reader["CANCELADO"] != DBNull.Value ? Convert.ToInt32(reader["CANCELADO"].ToString()) : 0;
                                    rem.expired = reader["EXPIRADO"] != DBNull.Value ? Convert.ToInt32(reader["EXPIRADO"].ToString()) : 0;
                                    rem.quantity = 0;
                                    rem.amount_input = 0;
                                    rem.amount_output = 0;
                                    rem.released = reader["LIBERADO"] != DBNull.Value ? Convert.ToInt32(reader["LIBERADO"].ToString()) : 0;
                                    rem.added = reader["ENTRADA"] != DBNull.Value ? Convert.ToInt32(reader["ENTRADA"].ToString()) : 0;
                                    rem.removed = reader["SAIDA"] != DBNull.Value ? Convert.ToInt32(reader["SAIDA"].ToString()) : 0;
                                    rem.reason = "";
                                    rem.code = reader["CODIGO_DO_PRODUTO"].ToString();
                                    rem.registeredBy = reader["QUEM_CADASTROU"].ToString();
                                    rem.price = reader["VALOR_EM_MOEDAS"] != DBNull.Value ? Convert.ToInt32(reader["VALOR_EM_MOEDAS"].ToString()) : 0;
                                    rem.type = reader["TIPO_DE_ESTOQUE"].ToString();
                                    rem.updatedAt = reader["DATA_ATUALIZACAO"].ToString();
                                    rem.publicationDate = "";
                                    rem.expirationDateVoucher = reader["DATA_ATUALIZACAO"].ToString();
                                    rem.expirationDate = reader["VENCIMENTO"].ToString();
                                    rem.status = reader["STATUS"].ToString();
                                    rem.createdAt = reader["DATA_DE_CADASTRO"].ToString();
                                    rem.visibility = "";
                                    rem.final_balance = reader["SALDO_FINAL"] != DBNull.Value ? Convert.ToInt32(reader["SALDO_FINAL"].ToString()) : 0;

                                    rems.Add(rem);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }

                return rems;
            }

        }

        // Método para serializar um DataTable em JSON
    }
}