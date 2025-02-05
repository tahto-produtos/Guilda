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
using static TokenService;
using static ApiRepositorio.Controllers.ResultConsolidatedController;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]


    public class SubSectorsBySectorController : ApiController
    {
        public class InputModel
        {
            public string sector { get; set; }
            public string dtInicial { get; set; }
            public string dtFinal { get; set; }
            public List<int> sectorIds { get; set; }

        }

        // POST: api/Results
        [HttpPost]
        public IHttpActionResult GetResultsModel([FromBody] InputModel inputModel)
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
            string sectorsIdsGroupsAsString = "";
            if (inputModel.sectorIds != null)
            {
                sectorsIdsGroupsAsString = string.Join(",", inputModel.sectorIds);
            }

            bool adm = Funcoes.retornaPermissao(COLLABORATORID.ToString());
            if (adm == true)
            {
                //Coloca o id do CEO, pois tera a mesma visão
                COLLABORATORID = 756399;
            }

            List<subSector> rmams = new List<subSector>();
            rmams = returnTables.listSubSectorsBySector(inputModel.sector, inputModel.dtInicial, inputModel.dtFinal, COLLABORATORID, sectorsIdsGroupsAsString);

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(rmams);
        }

        // Método para serializar um DataTable em JSON
    }
}