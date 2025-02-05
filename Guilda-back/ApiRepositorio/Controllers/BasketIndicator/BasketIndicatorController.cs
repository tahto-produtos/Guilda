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
    public class BasketIndicatorController : ApiController
    {
        public bool doConfigureBasket(string groupId, string metricMin)
        {
            
            StringBuilder stb = new StringBuilder();
            stb.Append("WITH CTE AS ( ");
            stb.Append("    SELECT *, ");
            stb.Append("           ROW_NUMBER() OVER (PARTITION BY GROUPID ORDER BY CREATED_AT DESC) AS RN ");
            stb.Append("    FROM GDA_HISTORY_INDICATOR_GROUP (NOLOCK) ");
            stb.Append("    WHERE INDICATOR_ID = 10000012 ");
            stb.Append(") ");
            stb.AppendFormat("UPDATE CTE SET METRIC_MIN = {0} WHERE RN = 1 AND GROUPID = {1}; ", metricMin, groupId);
           
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {
                    connection.Close();
                    return false;
                }
                connection.Close();
            }
            return true;
        }

         #region Input

        public class PostInputModel
        {
            public string groupId { get; set; }

            public string metricMin { get; set; }

        }
       
        #endregion



        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] PostInputModel inputModel)
        {
            string groupId = inputModel.groupId.ToString();
            string metricMin = inputModel.metricMin.ToString();

           bool ok = doConfigureBasket(groupId, metricMin);

            if (ok == true)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }

        }

        [HttpGet]
        public IHttpActionResult GetResultsModel()
        {
            //Realiza a query que retorna todas as informações dos colaboradores que tiveram moneitzação.

            List<basketIndicator.PostInputModel> rmams = new List<basketIndicator.PostInputModel>();
             rmams = basketIndicator.rtnBktIndicator();

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(rmams);
            //return Ok();
        }







    }
}