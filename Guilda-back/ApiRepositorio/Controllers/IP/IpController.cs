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
    public class IpController : ApiController
    {
     

        // POST: api/Results
        [HttpGet]
        public IHttpActionResult GetResultsModel()
        {
            string enderecoIp = Database.ObterEnderecoIpDaMaquina();
            Database.Conn = Database.retornaConn();
            string retorno = "";
            if (Database.retornaConn().ToString() != "" && Database.retornaConn().ToString().Contains("database-1.cpmxhjwr8mgp"))
            {
                retorno = enderecoIp + " - produção";
            }
            else if (Database.retornaConn().ToString() != "" && Database.retornaConn().ToString().Contains("10.0.141.36"))
            {
                retorno = enderecoIp + " - homologação";
            }
            else
            {
                retorno = "Não encontrado";
            }

            return Ok(retorno);
        }

        
        // Método para serializar um DataTable em JSON
    }
}