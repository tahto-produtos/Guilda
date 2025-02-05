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
using System.Drawing.Imaging;
using Microsoft.Diagnostics.Runtime;
using ApiC.Class;
using OfficeOpenXml;
using System.Net.NetworkInformation;
using Iced.Intel;
using System.Collections;
using static ApiRepositorio.Controllers.ResultConsolidatedController;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]

    public class ListScoreBySectorController : ApiController
    {
        [HttpGet]
        public IHttpActionResult PostResultsModel()
        {
            string DataAtual = DateTime.Now.ToString("yyyy-MM-dd");
            List<ScoreBySector> rmams = new List<ScoreBySector>();
            rmams = returnTables.ListScoreBySector(DataAtual);

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(rmams);
        }

    }
}