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
    public class CreateProfileController : ApiController
    {
        [HttpPost]
        [ResponseType(typeof(ResultsModel))]
        public IHttpActionResult PostResultsModel()
        {
            string campo = HttpContext.Current.Request.Form["NAME"];
           
            string commandText = $"INSERT INTO GDA_PROFILE_COLLABORATOR_ADMINISTRATION (name, created_at) VALUES ('{campo}', GETDATE())";

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.ExecuteNonQuery();

                    SqlCommand command2 = new SqlCommand("SELECT * FROM GDA_PROFILE_COLLABORATOR_ADMINISTRATION", connection);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command2))
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