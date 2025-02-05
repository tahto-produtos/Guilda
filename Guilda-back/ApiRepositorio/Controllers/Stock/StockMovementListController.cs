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
    public class StockMovementListController : ApiController
    {

        public class InputModel
        {
            public List<Products> Products { get; set; }
            public List<Stocks> StocksFinals { get; set; }
            public List<Stocks> StocksOrigins { get; set; }
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
            public string Produto { get; set; }
            public string EstoqueDeOrigem { get; set; }
            public string EstoqueFinal { get; set; }
            public string Quantidade { get; set; }
            public string Motivo { get; set; }

        }

        public class returnResponse
        {
            public string data { get; set; }
            public string Produto { get; set; }
            public string EstoqueDeOrigem { get; set; }
            public string EstoqueFinal { get; set; }
            public string Quantidade { get; set; }
            public string Motivo { get; set; }
        }

        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {
            
            string dtInicial = inputModel.DataInicial.ToString("yyyy-MM-dd");
            string dtFinal = inputModel.DataFinal.ToString("yyyy-MM-dd");
            string stocksFinalsAsString = string.Join(",", inputModel.StocksFinals.Select(g => g.Id));
            string stocksOriginsAsString = string.Join(",", inputModel.StocksOrigins.Select(g => g.Id));
            string productsAsString = string.Join(",", inputModel.Products.Select(g => g.Id));

            string where = "";

            if (stocksFinalsAsString != "")
            {
                //where = $" AND HSI.IDGDA_STOCK IN ({stocksFinalsAsString}) ";

                where = $" AND HSP.GDA_STOCK_IDGDA_STOCK IN ({stocksFinalsAsString}) ";
            }
            if (stocksOriginsAsString != "")
            {
                //where = $" AND HSI.IDGDA_STOCK_ORIGIN IN ({stocksOriginsAsString}) ";

                where = $" AND HSP.IDGDA_STOCK_ORIGIN IN ({stocksOriginsAsString}) ";
            }
            if (productsAsString != "")
            {
                //where = $" AND HSI.IDGDA_PRODUCT IN ({productsAsString}) ";

                where = $" AND HSP.GDA_PRODUCT_IDGDA_PRODUCT IN ({productsAsString}) ";
            }

            DateTime dtTimeInicial = DateTime.ParseExact(dtInicial, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dtTimeFinal = DateTime.ParseExact(dtFinal, "yyyy-MM-dd", CultureInfo.InvariantCulture);


            //Realiza a query que retorna todas as informações dos colaboradores que tiveram moneitzação.
            List<HistoryInput> his = new List<HistoryInput>();

            StringBuilder sb = new StringBuilder();


            sb.Append("	   SELECT HSP.CREATED_AT AS DATA,  ");
            sb.Append("       P.COMERCIAL_NAME AS PRODUTO,  ");
            sb.Append("       CASE WHEN IDGDA_STOCK_ORIGIN = NULL THEN ORIGIN.DESCRIPTION ELSE FINAL.DESCRIPTION END  AS 'ESTOQUE DE ORIGEM',  ");
            sb.Append("       FINAL.DESCRIPTION AS 'ESTOQUE FINAL',  ");
            sb.Append("       AMOUNT_OUTPUT AS QUANTIDADE,  ");
            sb.Append("       RR.REASON AS MOTIVO  ");
            sb.Append("FROM GDA_HISTORY_STOCK_PRODUCT (NOLOCK) AS HSP  ");
            sb.Append("LEFT JOIN GDA_PRODUCT (NOLOCK) AS P ON P.IDGDA_PRODUCT = HSP.GDA_PRODUCT_IDGDA_PRODUCT  ");
            sb.Append("LEFT JOIN GDA_STOCK (NOLOCK) AS ORIGIN ON ORIGIN.IDGDA_STOCK = HSP.IDGDA_STOCK_ORIGIN  ");
            sb.Append("LEFT JOIN GDA_STOCK (NOLOCK) AS FINAL ON FINAL.IDGDA_STOCK = HSP.GDA_STOCK_IDGDA_STOCK  ");
            sb.Append("LEFT JOIN GDA_REASON_REMOVED (NOLOCK) AS RR ON RR.IDGDA_REASON_REMOVED = HSP.GDA_REASON_REMOVED_IDGDA_REASON_REMOVED  ");
            sb.AppendFormat("WHERE CONVERT(DATE, HSP.CREATED_AT) >= '{0}'  ", dtInicial);
            sb.AppendFormat("  AND CONVERT(DATE, HSP.CREATED_AT) <= '{0}'  ", dtFinal);
            sb.Append("  AND HSP.GDA_REASON_REMOVED_IDGDA_REASON_REMOVED = 6  ");
            sb.AppendFormat(" {0} ", where);

            //sb.Append("SELECT HSI.CREATED_AT AS DATA, P.COMERCIAL_NAME AS PRODUTO, ORIGIN.DESCRIPTION AS 'ESTOQUE DE ORIGEM',  ");
            //sb.Append("FINAL.DESCRIPTION AS 'ESTOQUE FINAL', QUANTIDADE, REASON AS MOTIVO FROM GDA_HISTORY_STOCK_INPUT (NOLOCK) AS HSI ");
            //sb.Append("INNER JOIN GDA_PRODUCT (NOLOCK) AS P ON P.IDGDA_PRODUCT = HSI.IDGDA_PRODUCT ");
            //sb.Append("INNER JOIN GDA_STOCK (NOLOCK) AS ORIGIN ON ORIGIN.IDGDA_STOCK = HSI.IDGDA_STOCK_ORIGIN ");
            //sb.Append("INNER JOIN GDA_STOCK (NOLOCK) AS FINAL ON FINAL.IDGDA_STOCK = HSI.IDGDA_STOCK ");
            //sb.AppendFormat("WHERE CONVERT(DATE, HSI.CREATED_AT) >= '{0}' AND CONVERT(DATE, HSI.CREATED_AT) <= '{1}' AND HSI.TYPE = 'TRANSFERIDO' ", dtInicial, dtFinal);
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
                                hi.Produto = reader["PRODUTO"].ToString();
                                hi.EstoqueDeOrigem = reader["ESTOQUE DE ORIGEM"].ToString();
                                hi.EstoqueFinal = reader["ESTOQUE FINAL"].ToString();
                                hi.Quantidade = reader["QUANTIDADE"].ToString();
                                if (reader["MOTIVO"].ToString() == "TRANSFERRED")
                                {
                                    hi.Motivo = "TRANSFERIDO";
                                }
                                //hi.Motivo = reader["MOTIVO"].ToString();

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
                Produto = item.Produto,
                EstoqueDeOrigem = item.EstoqueDeOrigem,
                EstoqueFinal = item.EstoqueFinal,
                Quantidade = item.Quantidade,
                Motivo = item.Motivo,
    }).ToList();

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(jsonData);
        }







    }
}