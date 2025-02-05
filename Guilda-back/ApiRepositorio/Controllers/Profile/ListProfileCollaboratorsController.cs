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
    //[Authorize]
    public class ListProfileCollaboratorsController : ApiController
    {
        [HttpGet]
        [ResponseType(typeof(ResultsModel))]
        public IHttpActionResult PostResultsModel(string profileId)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT C.NAME, COLLABORATORIDENTIFICATION AS BC, MAX(S.NAME) AS SECTOR FROM GDA_COLLABORATORS C (NOLOCK) ");
            sb.Append("INNER JOIN GDA_HISTORY_COLLABORATOR_SECTOR H (NOLOCK) ON C.IDGDA_COLLABORATORS = H.IDGDA_COLLABORATORS AND H.DELETED_AT IS NULL ");
            sb.Append("INNER JOIN GDA_PROFILE_COLLABORATOR_ADMINISTRATION P ON C.PROFILE_COLLABORATOR_ADMINISTRATIONID = P.id ");
            sb.AppendFormat("INNER JOIN GDA_SECTOR S ON H.IDGDA_SECTOR = S.IDGDA_SECTOR WHERE P.ID = {0} ", profileId);
            sb.Append("GROUP BY C.NAME, COLLABORATORIDENTIFICATION ");
            sb.Append("ORDER BY C.NAME ASC ");


            //string commandText = $"SELECT DISTINCT C.NAME, COLLABORATORIDENTIFICATION AS BC, S.NAME AS SECTOR FROM GDA_COLLABORATORS C (NOLOCK) INNER JOIN GDA_HISTORY_COLLABORATOR_SECTOR H (NOLOCK) ON C.IDGDA_COLLABORATORS = H.IDGDA_COLLABORATORS AND H.DELETED_AT IS NULL INNER JOIN GDA_PROFILE_COLLABORATOR_ADMINISTRATION P ON C.PROFILE_COLLABORATOR_ADMINISTRATIONID = P.id INNER JOIN GDA_SECTOR S ON H.IDGDA_SECTOR = S.IDGDA_SECTOR WHERE P.ID = {profileId} ORDER BY C.NAME ASC";

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
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