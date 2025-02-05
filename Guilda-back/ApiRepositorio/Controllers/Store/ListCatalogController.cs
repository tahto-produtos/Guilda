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
using static ApiRepositorio.Controllers.FinancialSummaryController;
using static ApiRepositorio.Controllers.ListCatalogController;
using static ApiRepositorio.Controllers.ScoreInputController;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ListCatalogController : ApiController
    {
        public class returnCatalogProduct
        {
            public string CODIGO_PRODUTO { get; set; }
            public string PRODUTO { get; set; }
            public string DESCRICAO { get; set; }
            public string CODIGO_REF { get; set; }
            public string ATIVO { get; set; }
            public string INATIVO { get; set; }
            public string ESTOQUE { get; set; }
            public string IMAGEM { get; set; }
        }
        public static List<string> verificaEstoques()
        {
            List<string> str = new List<string>();
            StringBuilder stb = new StringBuilder();
            stb.Append("SELECT IDGDA_STOCK, DESCRIPTION FROM GDA_STOCK NOLOCK WHERE DELETED_AT IS NULL");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand sqlcommand = new SqlCommand(stb.ToString(), connection))
                    {
                        SqlDataReader reader = sqlcommand.ExecuteReader();
                        while (reader.Read())
                        {
                            str.Add(reader["IDGDA_STOCK"].ToString());
                            // str.Add(reader["DESCRIPTION"].ToString());
                        }
                    }
                }
                catch (Exception)
                {
                }
                connection.Close();
            }
            return str;
        }
        public static List<returnCatalogProduct> ReturnCatalogProduct(string StockID)
        {
            List<returnCatalogProduct> catalogProducts = new List<returnCatalogProduct>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT   ");
            sb.Append("GP.IDGDA_PRODUCT,  ");
            sb.Append("GP.COMERCIAL_NAME AS PRODUTO,  ");
            sb.Append("GP.DESCRIPTION AS DESCRICAO,  ");
            sb.Append("GP.CODE AS CODIGO_REFERENCIA,  ");
            sb.Append("CASE WHEN GP.GDA_PRODUCT_STATUS_IDGDA_PRODUCT_STATUS = 1 THEN ISNULL(SUM(HP.AMOUNT_INPUT),0) - ISNULL(SUM(HP.AMOUNT_OUTPUT),0) ELSE 0 END AS QTD_ATIVO,  ");
            sb.Append("CASE WHEN GP.GDA_PRODUCT_STATUS_IDGDA_PRODUCT_STATUS = 2 THEN ISNULL(SUM(HP.AMOUNT_INPUT),0) - ISNULL(SUM(HP.AMOUNT_OUTPUT),0) ELSE 0 END AS QTD_INATIVO,  ");
            sb.Append("ST.DESCRIPTION AS ESTOQUE, ");
            sb.Append("UP.URL AS IMAGEM ");
            sb.Append("FROM GDA_HISTORY_STOCK_PRODUCT (NOLOCK) HP   ");
            sb.Append("INNER JOIN GDA_PRODUCT GP (NOLOCK) ON GP.IDGDA_PRODUCT  = HP.GDA_PRODUCT_IDGDA_PRODUCT AND GP.DELETED_AT IS NULL  ");
            sb.Append("INNER JOIN GDA_PRODUCT_STATUS S (NOLOCK) ON S.IDGDA_PRODUCT_STATUS = GP.GDA_PRODUCT_STATUS_IDGDA_PRODUCT_STATUS   ");
            sb.Append("INNER JOIN GDA_STOCK ST (NOLOCK) ON ST.IDGDA_STOCK = HP.GDA_STOCK_IDGDA_STOCK AND ST.DELETED_AT IS NULL  ");
            sb.Append("LEFT JOIN GDA_PRODUCT_IMAGES IMG (NOLOCK) ON  IMG.PRODUCTID = GP.IDGDA_PRODUCT ");
            sb.Append("LEFT JOIN GDA_UPLOADS UP (NOLOCK) ON UP.ID = IMG.UPLOADID ");
            sb.Append($"WHERE HP.GDA_STOCK_IDGDA_STOCK = {StockID}  ");
            sb.Append("GROUP BY GP.IDGDA_PRODUCT,  ");
            sb.Append("		 GP.CODE,   ");
            sb.Append("		 GP.COMERCIAL_NAME,    ");
            sb.Append("		 GP.DESCRIPTION,  ");
            sb.Append("		 GP.GDA_PRODUCT_STATUS_IDGDA_PRODUCT_STATUS,  ");
            sb.Append("		 HP.GDA_STOCK_IDGDA_STOCK,  ");
            sb.Append("      ST.DESCRIPTION, ");
            sb.Append("      UP.URL");

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
                                returnCatalogProduct catalog = new returnCatalogProduct();
                                catalog.CODIGO_PRODUTO = reader["IDGDA_PRODUCT"].ToString();
                                catalog.PRODUTO = reader["PRODUTO"].ToString();
                                catalog.DESCRICAO = reader["DESCRICAO"].ToString();
                                catalog.CODIGO_REF = reader["CODIGO_REFERENCIA"].ToString();
                                catalog.ATIVO = reader["QTD_ATIVO"].ToString();
                                catalog.INATIVO = reader["QTD_INATIVO"].ToString();
                                catalog.ESTOQUE = reader["ESTOQUE"].ToString();
                                catalog.IMAGEM = reader["IMAGEM"].ToString();
                                catalogProducts.Add(catalog);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            return catalogProducts;

        }
        public static List<returnCatalogProduct> CatalogProduct()
        {
            List<string> lstr = verificaEstoques();
            List<returnCatalogProduct> Catalog = new List<returnCatalogProduct>();
            if (lstr.Count != 0)
            {
                for (int i = 0; i < lstr.Count; i++)
                {
                    List<returnCatalogProduct> CatalogoPorEstoque = new List<returnCatalogProduct>();
                    CatalogoPorEstoque = ReturnCatalogProduct(lstr[i].ToString());
                    Catalog.AddRange(CatalogoPorEstoque);
                }
            }
            return Catalog;
            //return Ok("Nenhuma estoque vinculado!");
        }

        [HttpGet]
        [ResponseType(typeof(ResultsModel))]
        public IHttpActionResult PostResultsModel()
        {
            //Realiza a query que retorna todos resultados fator dos colaboradores.
            List<returnCatalogProduct> rmams = new List<returnCatalogProduct>();
            rmams = CatalogProduct();

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(rmams);
        }
    }
}