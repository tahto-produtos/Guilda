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
    public class ListProfileController : ApiController
    {
        [HttpGet]
        [ResponseType(typeof(ResultsModel))]
        public IHttpActionResult PostResultsModel()
        {
            string commandText = $"SELECT ISNULL(P.name, 'SEM SETOR') AS PROFILENAME, COUNT(*) AS QUANTITY FROM GDA_COLLABORATORS C (NOLOCK) LEFT JOIN GDA_PROFILE_COLLABORATOR_ADMINISTRATION P (NOLOCK) ON C.PROFILE_COLLABORATOR_ADMINISTRATIONID = P.id WHERE C.DELETED_AT IS NULL GROUP BY ISNULL(P.name, 'SEM SETOR')";

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