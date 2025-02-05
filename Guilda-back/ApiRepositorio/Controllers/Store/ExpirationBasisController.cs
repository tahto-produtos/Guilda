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
using DocumentFormat.OpenXml.Spreadsheet;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ExpirationBasisController : ApiController
    {
        public class Expiration
        {
            public string VALUE { get; set; }
            public string TYPE { get; set; }
            public string CREATED_AT { get; set; }
            public string ALTERED_BY { get; set; }
        }

        public List<Expiration> ReturnExpirationBasis()
        {
            List<Expiration> Expiration = new List<Expiration>();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT TYPE AS NAME, VALUE AS EXPIRATION, CREATED_AT, ALTERED_BY FROM GDA_MKT_CONFIG NOLOCK WHERE DELETED_AT IS NULL ");

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
                                Expiration ExpirationBasis = new Expiration();
                                ExpirationBasis.TYPE = reader["NAME"].ToString();
                                ExpirationBasis.VALUE = reader["EXPIRATION"].ToString();
                                ExpirationBasis.CREATED_AT = reader["CREATED_AT"].ToString();
                                ExpirationBasis.ALTERED_BY = reader["ALTERED_BY"].ToString();
                                Expiration.Add(ExpirationBasis);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
                return Expiration;
            }
        }

        [HttpGet]
        public IHttpActionResult GetResultsModel()
        {
            List<Expiration> rmams = new List<Expiration>();
            rmams = ReturnExpirationBasis();
            return Ok(rmams);
        }
    }
}
