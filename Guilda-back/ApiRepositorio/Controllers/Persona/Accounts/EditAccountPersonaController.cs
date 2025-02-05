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
using System.Drawing.Imaging;
using System.Net.NetworkInformation;
using System.Web.UI;
using System.Xml.Linq;
using CommandLine;
using DocumentFormat.OpenXml.Spreadsheet;
using static ApiRepositorio.Controllers.FinancialSummaryController;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class EditAccountPersonaController : ApiController
    {// POST: api/Results
        public class InputModel
        {
            public int IDPERSONAUSER { get; set; }
            public int TYPE { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel()
        {
            int collaboratorId = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            collaboratorId = inf.collaboratorId;


            string idLog = Logs.InsertActionLogs("Update EditAccountPersona ", "GDA_PERSONA_USER", collaboratorId.ToString());
            string jsonFromFormData = System.Web.HttpContext.Current.Request.Form["json"];
            InputModel Json = JsonConvert.DeserializeObject<InputModel>(jsonFromFormData);
            int IDPERSONAUSER = Json.IDPERSONAUSER;
            int TYPE = Json.TYPE;

            #region UPDATE GDA_PERSONA_USER
            StringBuilder sbPersonaUser = new StringBuilder();
            sbPersonaUser.Append("UPDATE GDA_PERSONA_USER SET  ");
            sbPersonaUser.Append($"IDGDA_PERSONA_USER_TYPE = {TYPE}  ");
            sbPersonaUser.Append($"WHERE IDGDA_PERSONA_USER = {IDPERSONAUSER} ");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sbPersonaUser.ToString(), connection))
                    {
                        command.ExecuteScalar();
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    return BadRequest($"Erro ao Atualizar GDA_PERSONA_USER: {ex.Message}");
                }
            }
            #endregion
            return Ok("Edição de conta realizada com sucesso.");

            
        }
        // Método para serializar um DataTable em JSON
    }
}