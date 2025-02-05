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
    public class BasketIndicatorSectorController : ApiController
    {

        public class ReturnBasketIndicatorSector
        {
            public string codIndicator { get; set; }
            public string nameIndicator { get; set; }
            public string weightIndicator { get; set; }

        }

        public bool doWeightIndicatorSectorBasket(string sector, string indicator, string weight)
        {
            
            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("UPDATE GDA_HISTORY_INDICATOR_SECTORS SET WEIGHT_BASKET = '{0}' ", weight);
            stb.Append("WHERE ID = (SELECT MAX(HIS.ID) FROM GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG ");
            stb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS GI ON GI.IDGDA_INDICATOR = HIG.INDICATOR_ID ");
            stb.Append("INNER JOIN  ");
            stb.Append("  (SELECT ID, GOAL,  ");
            stb.Append("		  WEIGHT_BASKET, ");
            stb.Append("          INDICATOR_ID,  ");
            stb.Append("          SECTOR_ID,  ");
            stb.Append("          ROW_NUMBER() OVER (PARTITION BY INDICATOR_ID,  ");
            stb.Append("                                          SECTOR_ID  ");
            stb.Append("                             ORDER BY CREATED_AT DESC) AS RN  ");
            stb.Append("   FROM GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) WHERE DELETED_AT IS NULL ) AS HIS ON HIS.INDICATOR_ID = HIG.INDICATOR_ID  ");
            stb.Append("																				AND HIS.SECTOR_ID = HIG.SECTOR_ID AND HIS.RN = 1 ");
            stb.Append("WHERE MONETIZATION > 0 AND HIG.DELETED_AT IS NULL ");
            stb.AppendFormat("AND HIG.SECTOR_ID = {0} AND HIG.INDICATOR_ID <> 10000012 AND HIG.INDICATOR_ID = {1} ", sector, indicator);
            stb.Append("GROUP BY HIG.INDICATOR_ID) ");

            List<ReturnBasketIndicatorSector> rmams = new List<ReturnBasketIndicatorSector>();

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

        public List<ReturnBasketIndicatorSector> returnIndicatorSectorMonetization(string sector)
        {


            
            StringBuilder stb = new StringBuilder();
            stb.Append("SELECT HIG.INDICATOR_ID, MAX(NAME) AS NAME, MAX(WEIGHT_BASKET) AS 'WEIGHT_BASKET' FROM GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG ");
            stb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS GI ON GI.IDGDA_INDICATOR = HIG.INDICATOR_ID ");
            stb.Append("INNER JOIN  ");
            stb.Append("  (SELECT GOAL,  ");
            stb.Append("		  WEIGHT_BASKET, ");
            stb.Append("          INDICATOR_ID,  ");
            stb.Append("          SECTOR_ID,  ");
            stb.Append("          ROW_NUMBER() OVER (PARTITION BY INDICATOR_ID,  ");
            stb.Append("                                          SECTOR_ID  ");
            stb.Append("                             ORDER BY CREATED_AT DESC) AS RN  ");
            stb.Append("   FROM GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) WHERE DELETED_AT IS NULL ) AS HIS ON HIS.INDICATOR_ID = HIG.INDICATOR_ID  ");
            stb.Append("																				AND HIS.SECTOR_ID = HIG.SECTOR_ID AND HIS.RN = 1 ");
            stb.Append("WHERE MONETIZATION > 0 AND HIG.DELETED_AT IS NULL ");
            stb.AppendFormat("AND HIG.SECTOR_ID = {0} AND HIG.INDICATOR_ID <> 10000012 ", sector);
            stb.Append("GROUP BY HIG.INDICATOR_ID ");

            List<ReturnBasketIndicatorSector> rmams = new List<ReturnBasketIndicatorSector>();

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ReturnBasketIndicatorSector rmam = new ReturnBasketIndicatorSector();
                                rmam.codIndicator = reader["INDICATOR_ID"].ToString();
                                rmam.nameIndicator = reader["NAME"].ToString();
                                rmam.weightIndicator = reader["WEIGHT_BASKET"].ToString();

                                rmams.Add(rmam);
                            }
                        }
                    }
                    connection.Close();
                }
                catch (Exception)
                {

                    throw;
                }
                connection.Close();
            }
            return rmams;
        }

        #region Input
        public class GetInputModel
        {
            public string codSector { get; set; }
        }



        public class PostInputModel
        {
            public string codSector { get; set; }

            public string codIndicator { get; set; }

            public string weightIndicator { get; set; }
        }
       
        #endregion

        // POST: api/Results
        [HttpGet]
        public IHttpActionResult GetResultsModel(int codSetor)
        {

            string sector = codSetor.ToString();
           


            //Realiza a query que retorna todas as informações dos colaboradores que tiveram moneitzação.
            List<ReturnBasketIndicatorSector> rmams = new List<ReturnBasketIndicatorSector>();
            rmams = returnIndicatorSectorMonetization(sector);

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(rmams);
        }


        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] PostInputModel inputModel)
        {
            string sector = inputModel.codSector.ToString();
            string indicator = inputModel.codIndicator.ToString();
            string weight = inputModel.weightIndicator.ToString();

            //Realiza a query que retorna todas as informações dos colaboradores que tiveram moneitzação.
           bool ok = doWeightIndicatorSectorBasket(sector, indicator, weight);

            if (ok == true)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }

        }

    }
}