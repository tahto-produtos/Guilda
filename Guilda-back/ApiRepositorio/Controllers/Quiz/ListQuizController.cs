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
    public class ListQuizController : ApiController
    {// POST: api/Results
        [HttpGet]
        public IHttpActionResult PostResultsModel(int quiz, int idQuizUser)
        {
            int COLLABORATORID = 0;
            int PERSONAUSERID = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            COLLABORATORID = inf.collaboratorId;
            PERSONAUSERID = inf.personauserId;

            List<Quiz> rmams = new List<Quiz>();
            rmams = returnTables.listQuiz(quiz, idQuizUser, PERSONAUSERID, COLLABORATORID);

            //Quando ja estiver 100% respondido, não retornar a proxima pergunta
            if (rmams.Count > 0)
            {
                if (rmams.First().QTD_QUESTION == rmams.First().QTD_ANSWER)
                {
                    rmams.Clear();
                }
            }


            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(rmams);
        }
        // Método para serializar um DataTable em JSON
    }
}