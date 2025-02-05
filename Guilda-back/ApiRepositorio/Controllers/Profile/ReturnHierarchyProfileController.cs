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
using System.ComponentModel.DataAnnotations;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ReturnHierarchyProfileController : ApiController
    {
        [HttpGet]
        public IHttpActionResult PostResultsModel(int IDGDA_PROFILE)
        {
            StringBuilder stb = new StringBuilder();
            //stb.Append("SELECT PCA.ID AS 'ID PERFIL', PF.ID AS 'ID HIERARQUIA', PF.profile AS 'HIERARQUIA' FROM GDA_PROFILE_COLLABORATOR_ADMINISTRATION PCA (NOLOCK)  ");
            stb.Append("SELECT PCA.ID AS 'ID PERFIL',PCA.BASIC_PROFILE_IDHIERARCHY AS 'ID HIERARQUIA',PF.PROFILE AS 'HIERARQUIA' FROM GDA_PROFILE_COLLABORATOR_ADMINISTRATION PCA (NOLOCK) ");
            stb.Append("INNER JOIN GDA_PROFILES PF (NOLOCK) ON PF.hierarchy_id = PCA.BASIC_PROFILE_IDHIERARCHY ");
            stb.AppendFormat("WHERE PCA.ID = '{0}'", IDGDA_PROFILE);

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
               
                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
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