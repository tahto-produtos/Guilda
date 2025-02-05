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
using OfficeOpenXml;
using Utilities;
using ApiC.Class;
using System.Drawing.Imaging;
using Microsoft.Extensions.Primitives;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class StockInputQuantityController : ApiController
    {
        public bool verificaProdutoEstoque(int idProduto, int idStock)
        {
            bool retorno = false;


            StringBuilder stb = new StringBuilder();
            stb.Append("SELECT * FROM GDA_STOCK_PRODUCT P (NOLOCK) ");
            stb.AppendFormat("WHERE GDA_PRODUCT_IDGDA_PRODUCT = {0} AND GDA_STOCK_IDGDA_STOCK = {1} ", idProduto, idStock);


            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.CommandTimeout = 60;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                retorno = true;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    connection.Close();
                    throw;
                }
                connection.Close();
            }

            return retorno;
        }

        [HttpPost]
        [ResponseType(typeof(ResultsModel))]
        public IHttpActionResult PostResultsModel()
        {
            HttpPostedFile arquivo = HttpContext.Current.Request.Files["FILE"];
            string matricula = HttpContext.Current.Request.Form["MATRICULA"];
            
            if (matricula == "")
            {
                matricula = "0";
            }

            int qtdNaoEncontrado = 0;
            int qtdTotal = 0;
            List<stock> lStock = returnTables.listStock();
            List<product> lProduct = returnTables.listProduct();

           string idLog= Logs.InsertActionLogs("IMPORT_QUANTITY_PRODUCT", "GDA_LOG_ACTIONS_INPUT_QUANTITY_PRODUCTS", matricula);

            if (arquivo != null && arquivo.ContentLength > 0)
            {
                using (ExcelPackage package = new ExcelPackage(arquivo.InputStream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    int rowCount = worksheet.Dimension.Rows;
                    for (int row = 2; row <= rowCount; row++)
                    {
                        qtdTotal += 1;
                        DateTime Data = Convert.ToDateTime(worksheet.Cells[row, 1].Value.ToString());
                        string Fornecedor = worksheet.Cells[row, 2].Value.ToString();
                        string ValorDoProduto = worksheet.Cells[row, 3].Value.ToString().Replace(",",".");
                        string IdDoProduto = worksheet.Cells[row, 4].Value.ToString();
                        string Estoque = worksheet.Cells[row, 5].Value.ToString();
                        string Quantidade = worksheet.Cells[row, 6].Value.ToString();
                        //string nomecolaborador = worksheet.Cells[row, 3].Value.ToString();


                        var list1 = lStock.FindAll(l => l.DESCRIPTION == Estoque);
                        if (list1.Count == 0)
                        {
                            qtdNaoEncontrado += 1;
                            continue;
                        }
                        int idStock = list1.First().IDGDA_STOCK;

                        var list2 = lProduct.FindAll(l => l.CODE.ToString() == IdDoProduto);
                        if (list2.Count == 0)
                        {
                            qtdNaoEncontrado += 1;
                            continue;
                        }
                        int idProduct = list2.First().IDGDA_PRODUCT;

                        string NameProduct = list2.First().COMERCIAL_NAME;
                        string ValidateProduct = list2.First().VALIDITY_DATE;

                        if (idStock > 0 && idProduct > 0)
                        {

                            bool existeProdutoNoEstoque = verificaProdutoEstoque(idProduct, idStock);

                            if (existeProdutoNoEstoque == false)
                            {
                                qtdNaoEncontrado += 1;
                                continue;
                            }

                            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                            {
                                try
                                {
                                    
                                    //Insere historico
                                    connection.Open();
                                    StringBuilder stb = new StringBuilder();
                                    stb.Append("INSERT INTO GDA_HISTORY_STOCK_PRODUCT (AMOUNT_INPUT, AMOUNT_OUTPUT, EXPIRATION_DATE, REGISTERED_BY, GDA_PRODUCT_IDGDA_PRODUCT, GDA_STOCK_IDGDA_STOCK, GDA_REASON_REMOVED_IDGDA_REASON_REMOVED, CREATED_AT, DELETED_AT, GDA_SUPPLIER_IDGDA_SUPPLIER, FORNECEDOR, VALORPRODUTO, IDGDA_STOCK_ORIGIN, REASON_COMMENT) VALUES ( ");
                                    stb.AppendFormat("'{0}',", Quantidade);
                                    stb.AppendFormat("0, ");
                                    stb.AppendFormat("NULL, ");
                                    stb.AppendFormat("'', ");
                                    stb.AppendFormat("'{0}', ", idProduct);
                                    stb.AppendFormat("'{0}', ", idStock);
                                    stb.AppendFormat("7, ");
                                    stb.AppendFormat("'{0}', ", Data.ToString("yyyy-MM-dd"));
                                    stb.AppendFormat("NULL, ");
                                    stb.AppendFormat("NULL, ");
                                    stb.AppendFormat("'{0}', ", Fornecedor);
                                    stb.AppendFormat("{0}, ", ValorDoProduto);
                                    stb.AppendFormat("NULL, ");
                                    stb.AppendFormat("NULL) ");
                                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                                    {
                                        command.ExecuteNonQuery();
                                    }
                                    //Insere no stock
                                    StringBuilder stb2 = new StringBuilder();
                                    stb2.Append("UPDATE GDA_STOCK_PRODUCT SET ");
                                    stb2.AppendFormat("AMOUNT = AMOUNT + {0} ", Quantidade);
                                    stb2.AppendFormat("WHERE GDA_STOCK_IDGDA_STOCK = {0} ", idStock);
                                    stb2.AppendFormat("AND GDA_PRODUCT_IDGDA_PRODUCT = {0} ", idProduct);
                                    using (SqlCommand command = new SqlCommand(stb2.ToString(), connection))
                                    {
                                        command.ExecuteNonQuery();
                                    }


                                    for(int Indice = 0; Indice < Convert.ToInt32(Quantidade); Indice++)
                                    {
                                        StringBuilder stb3 = new StringBuilder();
                                        //Insere no Product_item
                                        stb3.Append("INSERT INTO GDA_PRODUCT_ITEM ");
                                        stb3.Append("(PRODUTO, CREATED_AT, VALIDATED_AT, STATUS, GDA_PRODUCT_IDGDA_PRODUCT, STOCKID) ");
                                        stb3.Append("VALUES ");
                                        stb3.Append("( ");
                                        stb3.AppendFormat("'{0}', ", NameProduct);
                                        stb3.AppendFormat("	GETDATE(), ");
                                        stb3.AppendFormat("'{0}', ", ValidateProduct);
                                        stb3.AppendFormat("	NULL, ");
                                        stb3.AppendFormat("	{0}, ", idProduct);
                                        stb3.AppendFormat("	{0} ", idStock);
                                        stb3.Append(") ");
                                        using (SqlCommand command = new SqlCommand(stb3.ToString(), connection))
                                        {
                                            command.ExecuteNonQuery();
                                        }
                                    }
                                    

                                    //Insere Log 
                                   Logs.InsertActionInputQuantiyProductsLogs(idLog, Quantidade, idProduct, ValorDoProduto, idStock, Fornecedor, Data.ToString("yyyy-MM-dd"));
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                                connection.Close();
                            }
                        }
                        else
                        {
                            qtdNaoEncontrado += 1;
                        }        
                    }


                }
            

                if (qtdNaoEncontrado > 0 && qtdTotal > qtdNaoEncontrado)
                {
                    return Ok($"importação massiva concluída com sucesso. Não foram encontrados {qtdNaoEncontrado} produtos e estoques para importação.");

                }
                else if (qtdNaoEncontrado == 0)
                {
                    return Ok("importação massiva concluída com sucesso.");
                }
                else
                {
                    return BadRequest("Produto e estoque não encontrados.");
                }
                
            }
            else
            {
                return BadRequest("Arquivo não enviado.");
            }          
        }
    }
}