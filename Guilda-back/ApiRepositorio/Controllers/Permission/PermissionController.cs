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
    public class PermissionController : ApiController
    {


        // POST: api/Results
        [HttpGet]
        public IHttpActionResult GetResultsModel(string codCollaborator, string codProfile)
        {

            string codCola = "";
            string codPro = "";
            if (codCollaborator != null)
            {
                codCola = codCollaborator;
            }
            if (codProfile != null)
            {
                codPro = codProfile;
            }


            List<permit> rmams = new List<permit>();
            rmams = Funcoes.retornaPermissaoColaboradorPerfil(codCola, codPro);

            return Ok(rmams);
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {
            
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                //Select para descobrir o id do setor

                connection.Open();
                foreach (permission item in inputModel.permit)
                {
                    int active = 0;
                    if (item.active == true)
                    {
                        active = 1;
                    }

                    StringBuilder stb = new StringBuilder();
                    stb.Append("MERGE INTO GDA_PROFILE_PERMIT AS TARGET ");
                    stb.AppendFormat("USING (VALUES ({0}, {1})) AS SOURCE (PERMISSION_ID, PROFILE_ID) ", item.permissionId, inputModel.profileId);
                    stb.Append("ON TARGET.PERMISSION_ID = SOURCE.PERMISSION_ID AND TARGET.PROFILE_ID = SOURCE.PROFILE_ID ");
                    stb.Append("WHEN MATCHED THEN ");
                    stb.AppendFormat("    UPDATE SET TARGET.ACTIVE = {0} ", active);
                    stb.Append("WHEN NOT MATCHED THEN ");
                    stb.AppendFormat("    INSERT (PERMISSION_ID, PROFILE_ID, CREATED_AT, ACTIVE) VALUES (SOURCE.PERMISSION_ID, SOURCE.PROFILE_ID, GETDATE(), {0}); ", active);

                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();

            }
            return Ok();
        }


        public class InputModel
        {
            public List<permission> permit { get; set; }
            public int profileId { get; set; }
            

        }

        public class permission
        {
            public int permissionId { get; set; }
            public Boolean active { get; set; }
        }

        // Método para serializar um DataTable em JSON
    }
}