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
using System.ComponentModel.DataAnnotations;
using ApiC.Class;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ListHierarchyByProfileController : ApiController
    {
        [HttpGet]
        public IHttpActionResult PostResultsModel(int IDGDA_HIERARCHY)
        {
            List<hierarchy> rmams = new List<hierarchy>();
            rmams = returnTables.listHierarchyProfile(IDGDA_HIERARCHY);

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(rmams);
        }     
    }
}