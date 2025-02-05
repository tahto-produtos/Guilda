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
using static ApiRepositorio.Controllers.SendNotificationController;
using DocumentFormat.OpenXml.Wordprocessing;
using static ApiRepositorio.Controllers.CreatedNotificationController;
using Antlr.Runtime.Misc;
using static ApiRepositorio.Controllers.LoadMyNotificationController;
using OfficeOpenXml.ConditionalFormatting;
using static TokenService;

//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ProductMaxValueController : ApiController
    {// POST: api/Results

        public class ReturnModelMaxValue
        {
            public int valueProduct { get; set; }

        }

        [HttpGet]
        public IHttpActionResult GetResultsModel()
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

            //INSERÇÃO DO FEEDBACK
            ReturnModelMaxValue rmd = new ReturnModelMaxValue();
            rmd.valueProduct = BancoMaxProduct.getMaxValueProduct();

            return Ok(rmd);

        }
        // Método para serializar um DataTable em JSON


        #region Banco

        public class BancoMaxProduct
        {

            public static int getMaxValueProduct()
            {
                int price = 0;

                StringBuilder sb = new StringBuilder();
                sb.Append($"SELECT TOP 1 PRICE FROM GDA_PRODUCT (NOLOCK) ");
                sb.Append($"WHERE DELETED_AT IS NULL  ");
                sb.Append($"ORDER BY PRICE DESC ");

                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    price = Convert.ToInt32(reader["PRICE"]);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }

                return price;
            }
        }
        #endregion

    }



}