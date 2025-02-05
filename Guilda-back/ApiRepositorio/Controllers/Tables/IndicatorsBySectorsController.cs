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
    public class IndicatorsBySectorsController : ApiController
    {
        public class InputModel
        {
            public List<Sector> Sectors { get; set; }

            public bool monetize { get; set; }

            public string dtInicial { get; set; }

            public string dtfinal { get; set; }
        }

        public class Sector
        {
            public int Id { get; set; }
        }

        // POST: api/Results
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {



            string sectorsAsString = string.Join(",", inputModel.Sectors.Select(g => g.Id));
            bool monetize = inputModel.monetize;
            string dtInicial = inputModel.dtInicial;
            string dtFinal = inputModel.dtfinal;


            //Realiza a query que retorna todas as informações dos colaboradores que tiveram moneitzação.
            List<indicator> rmams = new List<indicator>();
            rmams = returnTables.listIndicatorBySector(sectorsAsString, monetize, dtInicial, dtFinal);

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(rmams);
        }


        // Método para serializar um DataTable em JSON
    }
}