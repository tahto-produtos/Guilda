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
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class StockInputQuantityListController : ApiController
    {

        public class InputModel
        {
            public List<Products> Products { get; set; }
            public List<Stocks> Stocks { get; set; }
            public DateTime DataInicial { get; set; }
            public DateTime DataFinal { get; set; }
        }

        public class Products
        {
            public int Id { get; set; }
        }

        public class Stocks
        {
            public int Id { get; set; }
        }

        public class HistoryInput
        {
            public string data { get; set; }
            public string fornecedor { get; set; }
            public decimal valorDoProduto { get; set; }
            public int idProduto { get; set; }
            public string CodProduto { get; set; }
            public string nomeProduto { get; set; }
            public string descricaoProduto { get; set; }
            public int idEstoque { get; set; }
            public string nomeEstoque { get; set; }
            public int quantidadeEntrada { get; set; }
            public int quantidadeAtual { get; set; }


        }

        public class returnResponse
        {
            public string data { get; set; }
            public string fornecedor { get; set; }
            public decimal valorDoProduto { get; set; }
            public string CodProduto { get; set; }
            public string nomeProduto { get; set; }
            public string nomeEstoque { get; set; }
            public int quantidadeEntrada { get; set; }
        }

        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {
            
            string dtInicial = inputModel.DataInicial.ToString("yyyy-MM-dd");
            string dtFinal = inputModel.DataFinal.ToString("yyyy-MM-dd");
            string stocksAsString = string.Join(",", inputModel.Stocks.Select(g => g.Id));
            string productsAsString = string.Join(",", inputModel.Products.Select(g => g.Id));

            string where = "";

            if (stocksAsString != "")
            {
                where = $" AND SI.IDGDA_STOCK IN ({stocksAsString}) ";
            }
            if (productsAsString != "")
            {
                where = $" AND SI.IDGDA_PRODUCT IN ({productsAsString}) ";
            }

            DateTime dtTimeInicial = DateTime.ParseExact(dtInicial, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dtTimeFinal = DateTime.ParseExact(dtFinal, "yyyy-MM-dd", CultureInfo.InvariantCulture);


            //Realiza a query que retorna todas as informações dos colaboradores que tiveram moneitzação.
            List<HistoryInput> his = new List<HistoryInput>();

            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT  ");
            sb.Append("HSP.CREATED_AT AS DATA, ");
            sb.Append("HSP.FORNECEDOR, ");
            sb.Append("HSP.VALORPRODUTO, ");
            sb.Append("HSP.GDA_PRODUCT_IDGDA_PRODUCT AS IDGDA_PRODUCT, ");
            sb.Append("HSP.GDA_STOCK_IDGDA_STOCK AS IDGDA_STOCK, ");
            sb.Append("HSP.AMOUNT_INPUT AS 'QUANTIDADE ENTRADA', ");
            sb.Append("SP.AMOUNT AS 'QUANTIDADE ATUAL', ");
            sb.Append("P.COMERCIAL_NAME, ");
            sb.Append("P.DESCRIPTION, ");
            sb.Append("P.CODE, ");
            sb.Append("S.DESCRIPTION AS 'ESTOQUE' ");
            sb.Append("FROM  ");
            sb.Append("GDA_HISTORY_STOCK_PRODUCT (NOLOCK) HSP ");
            sb.Append("INNER JOIN GDA_STOCK (NOLOCK) AS S ON HSP.GDA_STOCK_IDGDA_STOCK = S.IDGDA_STOCK ");
            sb.Append("AND S.DELETED_AT IS NULL ");
            sb.Append("INNER JOIN GDA_PRODUCT (NOLOCK) AS P ON HSP.GDA_PRODUCT_IDGDA_PRODUCT = P.IDGDA_PRODUCT ");
            sb.Append("AND P.DELETED_AT IS NULL ");
            sb.Append("INNER JOIN GDA_STOCK_PRODUCT(NOLOCK) AS SP ON HSP.GDA_PRODUCT_IDGDA_PRODUCT = SP.GDA_PRODUCT_IDGDA_PRODUCT ");
            sb.Append("AND HSP.GDA_STOCK_IDGDA_STOCK = SP.GDA_STOCK_IDGDA_STOCK ");
            sb.Append("AND SP.DELETED_AT IS NULL ");
            sb.Append($"WHERE HSP.CREATED_AT >= '{dtInicial}' ");
            sb.Append($"AND HSP.CREATED_AT <= '{dtFinal}' ");
            sb.Append("AND GDA_REASON_REMOVED_IDGDA_REASON_REMOVED = 7 ");
            sb.Append($" {where} " );


            //sb.Append("SELECT SI.CREATED_AT AS DATA, SI.FORNECEDOR, SI.VALORPRODUTO, SI.IDGDA_PRODUCT, SI.IDGDA_STOCK, SI.QUANTIDADE AS 'QUANTIDADE ENTRADA', ");
            //sb.Append("SP.AMOUNT AS 'QUANTIDADE ATUAL', P.COMERCIAL_NAME, P.DESCRIPTION, P.CODE, S.DESCRIPTION AS 'ESTOQUE' ");
            //sb.Append("FROM GDA_HISTORY_STOCK_INPUT (NOLOCK) AS SI ");
            //sb.Append("INNER JOIN GDA_STOCK (NOLOCK) AS S ON SI.IDGDA_STOCK = S.IDGDA_STOCK AND S.DELETED_AT IS NULL ");
            //sb.Append("INNER JOIN GDA_PRODUCT (NOLOCK) AS P ON SI.IDGDA_PRODUCT = P.IDGDA_PRODUCT AND P.DELETED_AT IS NULL ");
            //sb.Append("INNER JOIN GDA_STOCK_PRODUCT(NOLOCK) AS SP ON SI.IDGDA_PRODUCT = SP.GDA_PRODUCT_IDGDA_PRODUCT AND SI.IDGDA_STOCK = SP.GDA_STOCK_IDGDA_STOCK AND SP.DELETED_AT IS NULL ");
            //sb.AppendFormat("WHERE SI.CREATED_AT >= '{0}' AND SI.CREATED_AT <= '{1}' AND SI.TYPE = 'INPUT' ", dtInicial, dtFinal);
            //sb.AppendFormat(" {0} ", where);

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                HistoryInput hi = new HistoryInput();
                                hi.data = Convert.ToDateTime(reader["DATA"].ToString()).ToString("dd/MM/yyyy");
                                hi.fornecedor = reader["FORNECEDOR"].ToString();
                                hi.valorDoProduto = Convert.ToDecimal(reader["VALORPRODUTO"].ToString());
                                hi.idProduto = Convert.ToInt32(reader["IDGDA_PRODUCT"].ToString());
                                hi.CodProduto = reader["CODE"].ToString();
                                hi.nomeProduto = reader["COMERCIAL_NAME"].ToString();
                                hi.descricaoProduto = reader["DESCRIPTION"].ToString();
                                hi.idEstoque = Convert.ToInt32(reader["IDGDA_STOCK"].ToString());
                                hi.nomeEstoque = reader["ESTOQUE"].ToString();
                                hi.quantidadeEntrada = Convert.ToInt32(reader["QUANTIDADE ENTRADA"].ToString());
                                hi.quantidadeAtual = Convert.ToInt32(reader["QUANTIDADE ATUAL"].ToString());

                                his.Add(hi);
                            }
                        }
                    }

                }
                catch (Exception)
                {

                    throw;
                }
                connection.Close();
            }

            var jsonData = his.Select(item => new returnResponse
            {
                data = item.data,
                fornecedor = item.fornecedor,
                valorDoProduto = item.valorDoProduto,
                CodProduto = item.CodProduto,
                nomeProduto = item.nomeProduto,
                nomeEstoque = item.nomeEstoque,
                quantidadeEntrada = item.quantidadeEntrada
            }).ToList();

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(jsonData);
        }







    }
}