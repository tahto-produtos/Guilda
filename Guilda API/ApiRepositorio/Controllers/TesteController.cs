using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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
using static ApiRepositorio.Controllers.ResultsConsolidatedController;
using System.Reflection;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class TesteController : ApiController
    {
        //private string connectionString = "Data Source=database-1.cpmxhjwr8mgp.us-east-1.rds.amazonaws.com;Initial Catalog=GUILDA_PROD;User ID=RDSadminsql2019;Password=H4ND4faG3AhjFe943NyPLRhMsEamXucKdDa;MultipleActiveResultSets=True;Connection Timeout=10";
        private RepositorioDBContext db = new RepositorioDBContext();


        // POST: api/Results
        [ResponseType(typeof(ResultsModel))]
        public IHttpActionResult PostResultsModel()
        {

            return StatusCode(HttpStatusCode.Created);
        }







    }

}