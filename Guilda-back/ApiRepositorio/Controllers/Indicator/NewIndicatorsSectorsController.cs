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
    public class NewIndicatorsSectorsController : ApiController
    {
        [HttpGet]
        [ResponseType(typeof(ResultsModel))]
        public IHttpActionResult PostResultsModel(string indicatorId)
        {
            string commandText = $"SELECT IDGDA_INDICATOR AS INDICATORID, "+
                                "I.NAME, "+
                                "I.DESCRIPTION, "+
                                "I.CREATED_AT, "+
                                "I.WEIGHT, "+
                                "I.CALCULATION_TYPE AS CALCTYPE, "+
                                "M.expression AS METRIC, "+
                                "I.STATUS "+
                                "FROM GDA_INDICATOR I "+
                                "INNER JOIN GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR H ON H.indicatorId = I.IDGDA_INDICATOR "+
                                "INNER JOIN GDA_MATHEMATICAL_EXPRESSIONS M ON M.id = H.mathematicalExpressionId "+
                                "WHERE NEWAPI = 1";

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