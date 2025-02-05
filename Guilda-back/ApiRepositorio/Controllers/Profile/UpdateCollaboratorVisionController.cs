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
using ApiC.Class;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class UpdateCollaboratorVisionController : ApiController
    {
        public class InputModel
        {
            public bool isChecked { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
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

            string upChecked = inputModel.isChecked == true ? "1" : "0";

            //Faz o primeiro select para verificar se ja existe uma inserção na base
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat($"UPDATE GDA_COLLABORATORS SET PERSONAL_VISION = '{upChecked}' WHERE IDGDA_COLLABORATORS = '{COLLABORATORID}'");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();

            }
            return Ok("Configuração atualizada com sucesso.");
        }
    }
}