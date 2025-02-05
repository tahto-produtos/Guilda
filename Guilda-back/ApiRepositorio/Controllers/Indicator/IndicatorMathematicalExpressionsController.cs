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
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class IndicatorMathematicalExpressionsController : ApiController
    {


        [HttpGet]
        [ResponseType(typeof(ResultsModel))]
        public IHttpActionResult PostResultsModel()
        {
            string commandText = $"SELECT DISTINCT(EXPRESSION) as expression FROM GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME " +
                $"INNER JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON HME.MATHEMATICALEXPRESSIONID = ME.ID " +
                $"WHERE HME.DELETED_AT IS NULL AND ME.DELETED_AT IS NULL ";

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        connection.Close();
                        return Ok(dataTable);
                    }
                }
                connection.Close();
            }
        }
    }
}