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
using System.Drawing;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class SectorsGroupsDateController : ApiController
    {
     

        // POST: api/Results
        [HttpGet]
        public IHttpActionResult GetResultsModel(string startDate, string endDate)
        {

            int collaboratorId = 0;
            int personauserId = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            collaboratorId = inf.collaboratorId;
            personauserId = inf.personauserId;

            bool adm = Funcoes.retornaPermissao(collaboratorId.ToString());
            if (adm == true)
            {
                //Coloca o id do CEO, pois tera a mesma visão
                collaboratorId = 756399;
            }

            //Realiza a query que retorna todas as informações dos colaboradores que tiveram moneitzação.
            List<sectorReturn> rmams = new List<sectorReturn>();
            rmams = returnTables.listSectorsGroupDate(startDate, endDate, collaboratorId);

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(rmams);
        }

        // Método para serializar um DataTable em JSON
    }
}