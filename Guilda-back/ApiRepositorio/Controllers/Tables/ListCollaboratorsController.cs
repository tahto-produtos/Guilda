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
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ListCollaboratorController : ApiController
    {
        // POST: api/Results
        [HttpGet]
        public IHttpActionResult GetResultsModel(string collaborator)
        {
            //Realiza a query que retorna todas as informações dos colaboradores.
            List<collaborators> rmams = new List<collaborators>();
            rmams = returnTables.listCollaborators(collaborator);

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(rmams);
        }

        // Método para serializar um DataTable em JSON
    }
}