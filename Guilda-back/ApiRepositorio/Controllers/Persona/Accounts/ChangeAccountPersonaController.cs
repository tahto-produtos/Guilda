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
using System.Drawing.Imaging;
using System.Net.NetworkInformation;
using System.Web.UI;
using System.Xml.Linq;
using CommandLine;
using DocumentFormat.OpenXml.Spreadsheet;
using static ApiRepositorio.Controllers.FinancialSummaryController;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ChangeAccountPersonaController : ApiController
    {// POST: api/Results


        public class returnChangeAccount
        {
            public string token { get; set; }

        }

        [HttpGet]
        public IHttpActionResult PostResultsModel(int idPersona)
        {
            int userId = 0;
            int codLog = 0;
            int collaboratorId = 0;

            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            userId = inf.idUser;
            collaboratorId = inf.collaboratorId;
            codLog = inf.codLog;


            returnChangeAccount rtn = new returnChangeAccount();
            rtn.token = TokenService.GenerateToken(userId, collaboratorId, codLog, idPersona);
         
            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(rtn);
        }
        // Método para serializar um DataTable em JSON
    }
}