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
    public class ResultsController : ApiController
    {
        // POST: api/Results
        [ResponseType(typeof(ResultsModel))]
        public IHttpActionResult PostResultsModel()
        {

            string campo1 = HttpContext.Current.Request.Form["name"];
            string campo2 = HttpContext.Current.Request.Form["alias"];

            string commandText = "SELECT TOP 1 * FROM GDA_COLLABORATORS"; // Substitua pela sua consulta SQL

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // Serializa o DataTable em JSON
                        string jsonData = JsonConvert.SerializeObject(dataTable);

                        // Retorna o JSON como resposta HTTP
                        connection.Close();
                        return Ok(jsonData);
                    }
                }
                connection.Close();
            }
        }
        // Método para serializar um DataTable em JSON
    }
}