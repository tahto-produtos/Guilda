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
using ApiC.Class.DowloadFile;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{

    //[Authorize]
    public class BuyItemController : ApiController
    {// POST: api/Results
        public class LimitExceededException : Exception
        {
            public int Quantity { get; }
            public string Product { get; }
            public int Bought { get; }

            public LimitExceededException(int quantity, string product, int bought)
                : base($"Limit for purchasing is {quantity}")
            {
                Quantity = quantity;
                Product = product;
                Bought = bought;
            }
        }

        public class BallanceCart
        {
            public string nameColaborator { get; set; }
            public int stockProductId { get; set; }
            public int productId { get; set; }
            public int stockId { get; set; }
            public int supplierId { get; set; }
            public int saleLimit { get; set; }
            public string comercialName { get; set; }
            public int amountBuy { get; set; }
            public int amountStock { get; set; }
            public string autosale { get; set; }
            public int historyAmountOutput { get; set; }
            public int price { get; set; }

            public int status { get; set; }
        }

        public class inputOrder
        {
            public string observationOrder { get; set; }
            public string userGroup { get; set; }


        }

        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        [HttpPost]
        public async Task<IHttpActionResult> PostResultsModel([FromBody] inputOrder inputModel)
        {
            CancellationToken cancellationToken = new CancellationToken();
            if (!await _semaphore.WaitAsync(TimeSpan.FromMinutes(60), cancellationToken))
            {
                return BadRequest("Tente novamente");
            }

            try
            {
                int collaboratorId = 0;
                int personauserId = 0;
                var token = Request.Headers.Authorization?.Parameter;
                string produtos = "";
                string nomeOperador = "";
                string bcOperador = "";

                InfsUser inf = TokenService.TryDecodeToken(token);
                if (inf == null)
                {
                    return Unauthorized();
                }
                collaboratorId = inf.collaboratorId;
                personauserId = inf.personauserId;

                bcOperador = "BC" + collaboratorId;

                int ballance = BancoBuyItem.returnLastBalance(collaboratorId);

                int ballanceCart = BancoBuyItem.returnPricesCart(collaboratorId);

                if (ballanceCart == 0)
                {
                    return BadRequest("Nenhum produto no carrinho!");
                }
                if (ballanceCart > ballance)
                {
                    return BadRequest("Moedas insuficientes!");
                }
                else
                {
                    //Varrer item a item do carrinho
                    //  
                    List<BallanceCart> bcc = new List<BallanceCart>();
                    bcc = BancoBuyItem.returnBbc(collaboratorId);
                    nomeOperador = bcc.FirstOrDefault().nameColaborator;
                    foreach (BallanceCart item in bcc)
                    {
                        if (item.saleLimit != 0 && item.historyAmountOutput >= item.saleLimit)
                        {
                            var errorMessage = $"É possível comprar apenas {item.saleLimit} itens para o produto: {item.comercialName}" +
                                               $"\nVocê já comprou: {item.historyAmountOutput}";

                            return BadRequest(errorMessage);
                        }

                        if (item.amountBuy > item.amountStock)
                        {
                            return BadRequest($"{item.comercialName} - Existe apenas {item.amountStock} products neste estoque. ");
                        }

                        if (item.status == 2)
                        {
                            return BadRequest($"{item.comercialName} - Produto inativado. ");
                        }

                        if (produtos != "")
                        {
                            produtos = $"{produtos}, {item.comercialName}";
                        }
                        else
                        {
                            produtos = $"{item.comercialName}";
                        }
                    }


                    foreach (BallanceCart item in bcc)
                    {
                        //Reserva Itens
                        List<bankMktplace.ModelItem> its = new List<bankMktplace.ModelItem>();
                        its = bankMktplace.consultItensAvaliable(item.productId, item.amountBuy);

                        //Update Product Item
                        foreach (bankMktplace.ModelItem itn in its)
                        {
                            bankMktplace.updateItem(itn.itemId, "RESERVED");
                        }

                        //Reserva Vouchers
                        List<bankMktplace.ModelVoucher> vchers = new List<bankMktplace.ModelVoucher>();
                        vchers = bankMktplace.consultVoucherAvailable(item.productId, item.stockId, item.amountBuy);

                        foreach (bankMktplace.ModelVoucher vcher in vchers)
                        {
                            //Update Voucher
                            bankMktplace.updateVoucher(vcher.voucherId, "RESERVED");
                        }

                    }

                    //int? idGroup = BancoBuyItem.returnIdGroup(inputModel.userGroup);

                    //Criar order
                    int codOrder = BancoBuyItem.createOrder(collaboratorId, inputModel.observationOrder, ballanceCart, inputModel.userGroup);

                    //Criar orderProduct
                    foreach (BallanceCart item in bcc)
                    {
                        BancoBuyItem.createOrderProduct(item.productId, item.stockId, item.supplierId, item.amountBuy, item.price, codOrder);

                        //Cria linha no HistoryStockProduct
                        BancoBuyItem.createHistoryStockProduct(item.productId, item.stockId, item.supplierId, 0, item.amountBuy, 9, collaboratorId);

                        //Atualiza stock product
                        int amountEnv = item.amountStock - item.amountBuy;
                        BancoBuyItem.updateStockProduct(item.stockProductId, amountEnv);
                    }

                    //Criar linha checking Acount
                    BancoBuyItem.insertCheckingAccount(collaboratorId, ballance, ballanceCart, codOrder);

                    //Clear Cart
                    BancoBuyItem.clearCart(collaboratorId);

                    //Reserva Coins
                    BancoBuyItem.reserveCoins(collaboratorId, ballanceCart);

                    //Insere notificação
                    ScheduledNotification.insertNotificationMktPlace(12, "Novo Pedido MarketPlace", $"Você recebeu um novo pedido de {nomeOperador} - {bcOperador}. Produtos: {produtos}!", true, personauserId, false, collaboratorId);
                }

                return Ok("Ok");
            }
            catch (Exception)
            {
                return BadRequest("Não foi possivel realizar a compra!");

            }
            finally
            {
                _semaphore.Release(); // Libera o slot no semáforo
            }
        }

        public class BancoBuyItem
        {
            public static int returnIdGroup(string group)
            {
                int idGroup = 0;
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("SELECT ID FROM GDA_GROUPS WHERE  ");
                stb.AppendFormat($"ID = '{group}' OR NAME = '{group}' OR ALIAS = '{group}' OR DESCRIPTION = '{group}' ");


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
                                    idGroup = Convert.ToInt32(reader["ID"].ToString());

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }

                return idGroup;
            }

            public static List<BallanceCart> returnBbc(int collaboratorId)
            {
                List<BallanceCart> bcc = new List<BallanceCart>();
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("SELECT ");
                stb.AppendFormat("MAX(SC.STOCK_PRODUCT_ID) AS STOCK_PRODUCT_ID, ");
                stb.AppendFormat("MAX(SP.GDA_PRODUCT_IDGDA_PRODUCT) AS PRODUCTID, ");
                stb.AppendFormat("MAX(SP.GDA_STOCK_IDGDA_STOCK) AS STOCKID, ");
                stb.AppendFormat("MAX(SP.GDA_SUPPLIER_IDGDA_SUPPLIER) AS SUPPLIERID, ");
                stb.AppendFormat("AUTOSALE, ");
                stb.AppendFormat("MAX(SALE_LIMIT) AS SALE_LIMIT, ");
                stb.AppendFormat("MAX(COMERCIAL_NAME) AS COMERCIAL_NAME, ");
                stb.AppendFormat("MAX(SC.AMOUNT) AS AMOUNT_BUY, ");
                stb.AppendFormat("MAX(SP.AMOUNT) AS AMOUNT_STOCK, ");
                stb.AppendFormat("SUM(HSP.AMOUNT_OUTPUT) - SUM(HSP.AMOUNT_INPUT) AS HISTORY_COLLABORATOR_AMOUNT_INPUT, ");
                stb.AppendFormat("MAX(PRICE) AS PRICE, ");
                stb.AppendFormat("MAX(P.GDA_PRODUCT_STATUS_IDGDA_PRODUCT_STATUS) AS STT, ");
                stb.AppendFormat("MAX(C.NAME) AS NAME ");
                stb.AppendFormat("FROM GDA_SHOPPING_CART (NOLOCK) AS SC ");
                stb.AppendFormat("INNER JOIN GDA_STOCK_PRODUCT (NOLOCK) AS SP ON SC.STOCK_PRODUCT_ID = SP.IDGDA_STOCK_PRODUCT ");
                stb.AppendFormat("INNER JOIN GDA_PRODUCT (NOLOCK) AS P ON P.IDGDA_PRODUCT = SP.GDA_PRODUCT_IDGDA_PRODUCT ");
                stb.AppendFormat("LEFT JOIN GDA_HISTORY_STOCK_PRODUCT (NOLOCK) AS HSP ON HSP.GDA_PRODUCT_IDGDA_PRODUCT = SP.GDA_PRODUCT_IDGDA_PRODUCT ");
                stb.AppendFormat("													AND HSP.GDA_STOCK_IDGDA_STOCK = SP.GDA_STOCK_IDGDA_STOCK ");
                stb.AppendFormat("													AND HSP.REGISTERED_BY = SC.ORDER_BY ");
                stb.AppendFormat("													AND HSP.GDA_REASON_REMOVED_IDGDA_REASON_REMOVED = 9 ");
                stb.AppendFormat("LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON ORDER_BY = C.IDGDA_COLLABORATORS ");
                stb.AppendFormat($"WHERE ORDER_BY = {collaboratorId} ");
                stb.AppendFormat("GROUP BY SC.IDGDA_SHOPPINGCART, AUTOSALE ");

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
                                    BallanceCart bc = new BallanceCart();
                                    bc.nameColaborator = reader["NAME"].ToString();
                                    bc.stockProductId = Convert.ToInt32(reader["STOCK_PRODUCT_ID"].ToString());
                                    bc.productId = Convert.ToInt32(reader["PRODUCTID"].ToString());
                                    bc.stockId = Convert.ToInt32(reader["STOCKID"].ToString());
                                    bc.supplierId = reader["SUPPLIERID"] != DBNull.Value ? Convert.ToInt32(reader["SUPPLIERID"].ToString()) : 0;
                                    bc.saleLimit = reader["SALE_LIMIT"] != DBNull.Value ? Convert.ToInt32(reader["SALE_LIMIT"].ToString()) : 0;
                                    bc.comercialName = reader["COMERCIAL_NAME"].ToString();
                                    bc.amountBuy = Convert.ToInt32(reader["AMOUNT_BUY"].ToString());
                                    bc.amountStock = Convert.ToInt32(reader["AMOUNT_STOCK"].ToString());
                                    bc.autosale = reader["AUTOSALE"].ToString();
                                    bc.historyAmountOutput = reader["HISTORY_COLLABORATOR_AMOUNT_INPUT"] != DBNull.Value ? Convert.ToInt32(reader["HISTORY_COLLABORATOR_AMOUNT_INPUT"].ToString()) : 0;
                                    bc.price = Convert.ToInt32(reader["PRICE"].ToString());
                                    bc.status = Convert.ToInt32(reader["STT"].ToString());
                                    bcc.Add(bc);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }

                return bcc;
            }

            public static bool updateStockProduct(int stockProductId, int amount)
            {

                List<BallanceCart> bcc = new List<BallanceCart>();
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("UPDATE GDA_STOCK_PRODUCT ");
                stb.AppendFormat($"SET AMOUNT = '{amount}' ");
                stb.AppendFormat($"WHERE IDGDA_STOCK_PRODUCT = {stockProductId} ");

                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }

                return true;
            }

            public static bool clearCart(int collaboratorId)
            {

                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("DELETE FROM GDA_SHOPPING_CART ");
                stb.AppendFormat($"WHERE ORDER_BY = {collaboratorId} ");


                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }

                return true;
            }

            public static bool insertCheckingAccount(int collaboratorId, int balance, int balanceCart, int idOrder)
            {
                int balanceTotal = balance - balanceCart;
                List<BallanceCart> bcc = new List<BallanceCart>();
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("INSERT INTO GDA_CHECKING_ACCOUNT (BALANCE, COLLABORATOR_ID, INPUT, OUTPUT, GDA_ORDER_IDGDA_ORDER, RESULT_DATE, REASON) ");
                stb.AppendFormat($"VALUES ( ");
                stb.AppendFormat($"{balanceTotal}, "); //BALANCE
                stb.AppendFormat($"{collaboratorId}, "); //COLLABORATOR_ID
                stb.AppendFormat($"0, "); //INPUT
                stb.AppendFormat($"{balanceCart}, "); //OUTPUT
                stb.AppendFormat($"{idOrder}, "); //GDA_ORDER_IDGDA_ORDER
                stb.AppendFormat($"GETDATE(), "); //RESULT_DATE
                stb.AppendFormat($"NULL "); //REASON
                stb.AppendFormat($") ");


                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }

                return true;
            }

            public static bool createHistoryStockProduct(int idProduct, int idStock, int idSupplier, int input, int output, int reasonRemoved, int collaboratorId)
            {
                string idSupplierEnv = idSupplier == 0 ? "NULL" : idSupplier.ToString();

                List<BallanceCart> bcc = new List<BallanceCart>();
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("INSERT INTO GDA_HISTORY_STOCK_PRODUCT (GDA_PRODUCT_IDGDA_PRODUCT, GDA_STOCK_IDGDA_STOCK,  ");
                stb.AppendFormat("GDA_SUPPLIER_IDGDA_SUPPLIER, AMOUNT_INPUT, AMOUNT_OUTPUT, GDA_REASON_REMOVED_IDGDA_REASON_REMOVED, REGISTERED_BY, CREATED_AT) ");
                stb.AppendFormat("VALUES ( ");
                stb.AppendFormat($"'{idProduct}', "); //GDA_PRODUCT_IDGDA_PRODUCT
                stb.AppendFormat($"'{idStock}', "); //GDA_STOCK_IDGDA_STOCK
                stb.AppendFormat($"{idSupplierEnv}, "); //GDA_SUPPLIER_IDGDA_SUPPLIER
                stb.AppendFormat($"'{input}', "); //AMOUNT_INPUT
                stb.AppendFormat($"'{output}', "); //AMOUNT_OUTPUT
                stb.AppendFormat($"'{reasonRemoved}', "); //GDA_REASON_REMOVED_IDGDA_REASON_REMOVED
                stb.AppendFormat($"'{collaboratorId}', "); //REGISTERED_BY
                stb.AppendFormat($"GETDATE() "); //CREATED_AT
                stb.AppendFormat(") ");
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }

                return true;
            }

            public static int createOrder(int idCollaborator, string observationOrder, int totalPrice, string idGroup)
            {
                string idGroupEnv = idGroup == "" ? "NULL" : idGroup.ToString();

                int codOrder = 0;
                int qtdOrder = 0;
                StringBuilder stb2 = new StringBuilder();
                stb2.AppendFormat("SELECT COUNT(0) AS QTD FROM GDA_ORDER (NOLOCK) ");

                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        using (SqlCommand command = new SqlCommand(stb2.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    qtdOrder = 1 + Convert.ToInt32(reader["QTD"].ToString());
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }


                List<BallanceCart> bcc = new List<BallanceCart>();
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("INSERT INTO GDA_ORDER (ORDER_BY, GDA_ORDER_STATUS_IDGDA_ORDER_STATUS, CREATED_AT, COD_ORDER, ");
                stb.AppendFormat("OBSERVATION_ORDER, ORDER_AT, TOTAL_PRICE, LAST_UPDATED_AT, LAST_UPDATED_BY, REASON_PURCHASE, IDGROUP) ");
                stb.AppendFormat("VALUES (");
                stb.AppendFormat($"{idCollaborator}, "); //ORDER_BY
                stb.AppendFormat($"1, "); //GDA_ORDER_STATUS_IDGDA_ORDER_STATUS
                stb.AppendFormat($"GETDATE(), "); //CREATED_AT
                stb.AppendFormat($"{qtdOrder}, "); //COD_ORDER
                stb.AppendFormat($"'{observationOrder}', "); //OBSERVATION_ORDER
                stb.AppendFormat($"GETDATE(), "); //ORDER_AT
                stb.AppendFormat($"{totalPrice}, "); //TOTAL_PRICE
                stb.AppendFormat($"GETDATE(), "); //LAST_UPDATED_AT
                stb.AppendFormat($"{idCollaborator}, "); //LAST_UPDATED_BY
                stb.AppendFormat($"'Compra realizada na loja', "); //REASON_PURCHASE
                stb.AppendFormat($"{idGroupEnv} "); //IDGROUP
                stb.AppendFormat(") SELECT @@IDENTITY AS 'CODORDER' ");


                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    codOrder = Convert.ToInt32(reader["CODORDER"].ToString());
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }

                return codOrder;
            }

            public static bool createOrderProduct(int idProduct, int idStock, int idSupplier, int amount, int price, int codOrder)
            {
                string idSupplierEnv = idSupplier == 0 ? "NULL" : idSupplier.ToString();

                List<BallanceCart> bcc = new List<BallanceCart>();
                StringBuilder stb = new StringBuilder();
                stb.AppendFormat("INSERT INTO GDA_ORDER_PRODUCT (AMOUNT, PRICE, GDA_ORDER_IDGDA_ORDER, GDA_PRODUCT_IDGDA_PRODUCT,  ");
                stb.AppendFormat("CREATED_AT, GDA_STOCK_IDGDA_STOCK, GDA_SUPPLIER_IDGDA_SUPPLIER, ORDER_PRODUCT_STATUS) ");
                stb.AppendFormat("VALUES (");
                stb.AppendFormat($"{amount}, "); //AMOUNT
                stb.AppendFormat($"{price}, "); //PRICE
                stb.AppendFormat($"{codOrder}, "); //GDA_ORDER_IDGDA_ORDER
                stb.AppendFormat($"{idProduct}, "); //GDA_PRODUCT_IDGDA_PRODUCT
                stb.AppendFormat($"GETDATE(), "); //CREATED_AT
                stb.AppendFormat($"{idStock}, "); //GDA_STOCK_IDGDA_STOCK
                stb.AppendFormat($"{idSupplierEnv}, "); //GDA_SUPPLIER_IDGDA_SUPPLIER
                stb.AppendFormat($"'ORDERED' "); //ORDER_PRODUCT_STATUS
                stb.AppendFormat(") ");

                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }

                return true;
            }

            public static bool reserveCoins(int idCollaborator, int ballanceCart)
            {
                bool retorno = false;

                StringBuilder stb = new StringBuilder();
                stb.Append("SELECT ID, INPUT, INPUT_USED FROM GDA_CHECKING_ACCOUNT (NOLOCK) ");
                stb.Append("WHERE  ");
                stb.Append("INPUT > 0 AND ");
                stb.Append($"COLLABORATOR_ID = {idCollaborator}  ");
                stb.Append("AND ((INPUT - INPUT_USED) > 0 OR INPUT_USED IS NULL) ");
                stb.Append("ORDER BY  ");

                stb.Append("CASE ");
                stb.Append("    WHEN INPUT_USED > 0 THEN 1 ");
                stb.Append("    ELSE 0 ");
                stb.Append("END, ");
                stb.Append("CASE ");
                stb.Append("	WHEN DUE_AT IS NULL THEN 1 ");
                stb.Append("	ELSE 0 ");
                stb.Append("END, ");
                stb.Append("	DUE_AT ASC, ");
                stb.Append("	CONVERT(DATE, CREATED_AT) DESC, ");
                stb.Append("INPUT ");

                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read() && ballanceCart > 0)
                                {

                                    //ballanceCart 7

                                    //input 10
                                    //inputUsed 3
                                    //inputLeft 7
                                    int id = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0;
                                    int input = reader["INPUT"] != DBNull.Value ? Convert.ToInt32(reader["INPUT"]) : 0;
                                    int inputUsed = reader["INPUT_USED"] != DBNull.Value ? Convert.ToInt32(reader["INPUT_USED"]) : 0;
                                    int inputLeft = input - inputUsed;

                                    int coinsUsed = 0;

                                    if (inputLeft > ballanceCart)
                                    {
                                        coinsUsed = ballanceCart;
                                        ballanceCart = 0;
                                    }
                                    else if (inputLeft == ballanceCart)
                                    {
                                        coinsUsed = inputLeft;
                                        ballanceCart = 0;
                                    }
                                    else
                                    {
                                        ballanceCart = ballanceCart - inputLeft;
                                        coinsUsed = inputLeft;
                                    }

                                    coinsUsed = coinsUsed + inputUsed;

                                    StringBuilder stbUpdate = new StringBuilder();
                                    stbUpdate.Append($"UPDATE GDA_CHECKING_ACCOUNT SET ");
                                    stbUpdate.Append($"INPUT_USED = {coinsUsed} ");
                                    stbUpdate.Append($"WHERE ID = {id} ");

                                    using (SqlCommand command2 = new SqlCommand(stbUpdate.ToString(), connection))
                                    {
                                        command2.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }

                return retorno;
            }
            public static int returnPricesCart(int idCollaborator)
            {
                int retorno = 0;


                StringBuilder stb = new StringBuilder();
                stb.Append("SELECT SUM(PRICE * SC.AMOUNT) SOMA FROM GDA_SHOPPING_CART (NOLOCK) SC ");
                stb.Append("INNER JOIN GDA_STOCK_PRODUCT (NOLOCK) SP ON SP.IDGDA_STOCK_PRODUCT = SC.STOCK_PRODUCT_ID ");
                stb.Append("INNER JOIN GDA_PRODUCT (NOLOCK) P ON P.IDGDA_PRODUCT = SP.GDA_PRODUCT_IDGDA_PRODUCT ");
                stb.Append($"WHERE ORDER_BY = {idCollaborator} ");

                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    retorno = reader["SOMA"] != DBNull.Value ? Convert.ToInt32(reader["SOMA"]) : 0;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }

                return retorno;
            }
            public static int returnLastBalance(int idCollaborator)
            {
                int retorno = 0;


                StringBuilder stb = new StringBuilder();
                stb.Append("SELECT TOP 1 BALANCE FROM GDA_CHECKING_ACCOUNT (NOLOCK)  ");
                stb.Append($"WHERE COLLABORATOR_ID = {idCollaborator} ");
                stb.Append("ORDER BY CREATED_AT DESC ");


                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    retorno = reader["BALANCE"] != DBNull.Value ? Convert.ToInt32(reader["BALANCE"]) : 0;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }

                return retorno;
            }

        }

        // Método para serializar um DataTable em JSON
    }
}