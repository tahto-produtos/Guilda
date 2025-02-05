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
using static ApiC.Class.bankMktplace;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class RemoveShoppingCartController : ApiController
    {// POST: api/Results

        public class InputModel
        {
            public List<InputProductsModel> products { get; set; }
        }

        public class InputProductsModel
        {
            public int stockProductId { get; set; }
            public int amount { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {
            int collaboratorId = 0;
            int personauserId = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            collaboratorId = inf.collaboratorId;
            personauserId = inf.personauserId;

            // Obter os stockProductId e criar uma lista de strings
            List<string> stockProductIds = inputModel.products
                .Select(p => p.stockProductId.ToString())
                .ToList();

            // Unir a lista numa string, separada por vírgulas
            string result = string.Join(",", stockProductIds);

            bool retn = BancoRemoveShoppingCart.removeItem(collaboratorId, result);

            if (retn == true)
            {
                return Ok("Ok");
            }
            else
            {
                return BadRequest("Não foi possivel remover!");
            }
        }

        public class BancoRemoveShoppingCart
        {
            public static bool removeItem(int idCollaborator, string products)
            {

                if (products == "")
                {
                    products = "''";
                }

                bool retorno = false;
                StringBuilder stb = new StringBuilder();
                stb.Append("SELECT TOP 1 IDGDA_SHOPPINGCART  ");
                stb.Append("FROM GDA_SHOPPING_CART (NOLOCK)  ");
                stb.Append("WHERE  ");
                stb.Append($"ORDER_BY = {idCollaborator} AND  ");
                stb.Append($"STOCK_PRODUCT_ID NOT IN ({products}) ");

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
                                    int idShoppingCart = Convert.ToInt32(reader["IDGDA_SHOPPINGCART"].ToString());
                                    StringBuilder stb2 = new StringBuilder();
                                    stb2.Append($"DELETE FROM GDA_SHOPPING_CART WHERE IDGDA_SHOPPINGCART = {idShoppingCart} ");
                                    using (SqlCommand command2 = new SqlCommand(stb2.ToString(), connection))
                                    {                                       
                                        command2.ExecuteNonQuery();
                                        retorno = true;
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
        }
    }
}