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
using DocumentFormat.OpenXml.ExtendedProperties;
using static ApiRepositorio.Controllers.IntegracaoAPIResultController;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class DeletedAccountPersonaController : ApiController
    {// POST: api/Results
        public class InputModel
        {
            public int IDPERSONAUSER { get; set; }
            public bool VALIDADETED { get; set; }   
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {
            int collaboratorId = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            collaboratorId = inf.collaboratorId;


            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT IDGDA_PERSONA_USER_TYPE ");
            sb.AppendFormat("FROM GDA_PERSONA_USER (NOLOCK) ");
            sb.AppendFormat("WHERE IDGDA_PERSONA_USER = {0} ", inputModel.IDPERSONAUSER);
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                               int typePersona =  reader["IDGDA_PERSONA_USER_TYPE"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_PERSONA_USER_TYPE"]) : 0;
                                if (typePersona != 2)
                                {
                                    return BadRequest($"Não é possivel remover uma conta pessoal!");
                                }
                            }
                        }
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    return BadRequest($"Erro ao deletar GDA_PERSONA_USER: {ex.Message}");
                }
            }


            string idLog = Logs.InsertActionLogs("Deleted DeletedAccountPersona ", "GDA_PERSONA_USER", collaboratorId.ToString());
            //string jsonFromFormData = System.Web.HttpContext.Current.Request.Form["json"];
            //InputModel Json = JsonConvert.DeserializeObject<InputModel>(jsonFromFormData);
            //int IDPERSONAUSER = Json.IDPERSONAUSER;
            //bool ValidacaoDeleted = Json.VALIDADETED;
            int IDPERSONAUSER = inputModel.IDPERSONAUSER;
            bool ValidacaoDeleted = inputModel.VALIDADETED;

            if (ValidacaoDeleted == false)
            {
                return Ok("Você tem certeza que deseja Excluir essa conta?");
            }
            else
            {
                #region DELETED GDA_PERSONA_USER
                StringBuilder sbPersonaUser = new StringBuilder();
                sbPersonaUser.Append("UPDATE GDA_PERSONA_USER SET  ");
                sbPersonaUser.Append($"DELETED_BY = {IDPERSONAUSER},  ");
                sbPersonaUser.Append($"DELETED_AT = GETDATE()  ");
                sbPersonaUser.Append($"WHERE IDGDA_PERSONA_USER = {IDPERSONAUSER} ");

                StringBuilder sbDeletedFollow = new StringBuilder();
                sbDeletedFollow.Append($"UPDATE GDA_PERSONA_FOLLOWERS SET DELETED_AT = GETDATE() WHERE IDGDA_PERSONA_USER_FOLLOWED = {IDPERSONAUSER}; ");
                sbDeletedFollow.Append($"UPDATE GDA_PERSONA_FOLLOWERS SET DELETED_AT = GETDATE() WHERE IDGDA_PERSONA_USER = {IDPERSONAUSER}  ");
                sbDeletedFollow.Append("");
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(sbPersonaUser.ToString(), connection))
                        {
                            command.ExecuteScalar();
                        }

                        using (SqlCommand command = new SqlCommand(sbDeletedFollow.ToString(), connection))
                        {
                            command.ExecuteScalar();
                        }
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"Erro ao deletar GDA_PERSONA_USER: {ex.Message}");
                    }
                }
                #endregion
                return Ok("Conta deletada com sucesso.");
            }
            
            
        }
        // Método para serializar um DataTable em JSON
    }
}