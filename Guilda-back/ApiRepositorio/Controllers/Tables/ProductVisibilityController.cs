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
using DocumentFormat.OpenXml.Office2010.Excel;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ProductVisibilityController : ApiController
    {
        public class Group
        {
            public int id { get; set; }
        }
        public class Hierarchy
        {
            public int id { get; set; }
        }
        public class Stock
        {
            public int id { get; set; }
        }
        public class InputModel
        {
            public List<Group> Groups { get; set; }
            public List<Hierarchy> Hierarchies { get; set; }
            public List<Stock> Stock {  get; set; }
            public string ProductName { get; set; }
            //public string CollaboratorId { get; set; }
        }
        public class returnResponseDay
        {
            public string ProductName { get; set; }
            public int  id { get; set; }

        }

        // POST: api/Results
        [HttpPost]
        public IHttpActionResult GetResultsModel(InputModel inputModel)
        {
            string groupsAsString = string.Join(",", inputModel.Groups.Select(g => g.id));
            string stockAsString = string.Join(",", inputModel.Stock.Select(g => g.id));
            string hierarchiesAsString = string.Join(",", inputModel.Hierarchies.Select(g => g.id));
            string productNameAsString = inputModel.ProductName.ToString();
            //Realiza a query que retorna todas as informações dos colaboradores que tiveram moneitzação.
            var jsonData = returnTables.ListProductVisibility(groupsAsString, hierarchiesAsString, stockAsString, productNameAsString);
            var json = jsonData.Select(item => new returnResponseDay
            {
                ProductName = item.COMERCIAL_NAME,
                id = item.IDGDA_PRODUCT
            }).ToList();
            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(json);

        }

        // Método para serializar um DataTable em JSON
    }
}