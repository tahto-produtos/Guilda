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
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{

    //Query que retorna todos os indicadores por setor e suas monetizações
//SELECT G.SECTOR_ID AS COD_SETOR, S.NAME AS SETOR, G.INDICATOR_ID AS COD_INDICADOR, I.NAME AS INDICADOR, G.MONETIZATION, GROUPID FROM GDA_HISTORY_INDICATOR_GROUP (NOLOCK) G
//LEFT JOIN GDA_INDICATOR (NOLOCK) I ON G.INDICATOR_ID = I.IDGDA_INDICATOR
//LEFT JOIN GDA_SECTOR (NOLOCK) S ON G.SECTOR_ID = S.IDGDA_SECTOR
//WHERE G.DELETED_AT IS NULL and i.deleted_at is null AND I.NAME IS NOT NULL AND S.NAME IS NOT NULL
//GROUP BY G.INDICATOR_ID, I.NAME, G.SECTOR_ID, S.NAME, GROUPID, G.MONETIZATION
//order by G.SECTOR_ID, G.INDICATOR_ID, GROUPID


    //[Authorize]
    public class ListNewIndicatorsController : ApiController
    {
        [HttpGet]
        [ResponseType(typeof(ResultsModel))]
        public IHttpActionResult PostResultsModel()
        {
            //string commandText = $"SELECT IDGDA_INDICATOR AS INDICATORID, NAME, DESCRIPTION, CREATED_AT, WEIGHT, CALCULATION_TYPE AS CALCTYPE, CALCULATION_TYPE AS METRIC, STATUS FROM GDA_INDICATOR WHERE NEWAPI = 1";

            StringBuilder stb = new StringBuilder();
            stb.Append("SELECT IDGDA_INDICATOR AS INDICATORID, NAME, DESCRIPTION, IND.CREATED_AT, WEIGHT, CALCULATION_TYPE AS CALCTYPE,  ");
            stb.Append("MAT.EXPRESSION AS METRIC, STATUS FROM GDA_INDICATOR (NOLOCK) AS IND ");
            stb.Append("INNER JOIN GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HIST ON IND.IDGDA_INDICATOR = HIST.INDICATORID  ");
            stb.Append("AND HIST.DELETED_AT IS NULL ");
            stb.Append("INNER JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS MAT ON MAT.ID = HIST.MATHEMATICALEXPRESSIONID ");
            stb.Append("WHERE NEWAPI = 1 ");

            string commandText = stb.ToString();

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        connection.Close();
                        return Ok(dataTable);
                    }
                }
                connection.Close();
            }
        }
    }
}