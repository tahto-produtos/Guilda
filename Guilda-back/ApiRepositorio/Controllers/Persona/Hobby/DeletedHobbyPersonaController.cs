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
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class DeletedHobbyPersonaController : ApiController
    {// POST: api/Results
        public class InputModel
        {
            public int IDGDA_PERSONA_HOBBY { get; set; }
            public int CREATED_BY { get; set; }
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


            string idLog = Logs.InsertActionLogs("Deleted DeletedHobbyPersona ", "GDA_PERSONA_HOBBY", collaboratorId.ToString());

            int IDGDA_PERSONA_HOBBY = inputModel.IDGDA_PERSONA_HOBBY;
            int IDCOLLABORATOR = collaboratorId;
                //VERIFICAR SE JA FOI EXCLUIDA DA BLACKLIST A PALAVRA.
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat($"SELECT IDGDA_PERSONA_HOBBY FROM GDA_PERSONA_HOBBY (NOLOCK) WHERE IDGDA_PERSONA_HOBBY ={IDGDA_PERSONA_HOBBY} AND DELETED_AT IS NOT NULL ");
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    return BadRequest($"Hobby ja foi deletado.");
                                }
                            }
                        }
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"Erro ao Deletar Hobby: {ex.Message}");
                    }
                }

                StringBuilder sb2 = new StringBuilder();
                sb2.Append($"UPDATE GDA_PERSONA_HOBBY SET  DELETED_AT =GETDATE(), DELETED_BY = {IDCOLLABORATOR} WHERE IDGDA_PERSONA_HOBBY = {IDGDA_PERSONA_HOBBY} ");
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(sb2.ToString(), connection))
                        {
                            command.ExecuteScalar();
                        }
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"Erro ao deletar o Hobby: {ex.Message}");
                    }
                }
                return Ok("Hobby excluido com sucesso");
            
            
            
        }
        // Método para serializar um DataTable em JSON
    }
}