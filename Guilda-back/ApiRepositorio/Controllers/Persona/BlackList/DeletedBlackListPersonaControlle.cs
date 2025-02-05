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
    public class DeletedBlackListPersonaController : ApiController
    {// POST: api/Results
        public class InputModel
        {
            public int IDGDA_BLACKLIST { get; set; }
            public int IDCOLLABORATOR { get; set; }
            public bool Validated { get; set; } 
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


            string idLog = Logs.InsertActionLogs("DELETED DeletedBlackListPersona ", "GDA_PERSONA_BLACKLIST", collaboratorId.ToString());

            int IDGDA_BLACKLIST = inputModel.IDGDA_BLACKLIST;
            int IDCOLLABORATOR = collaboratorId;
            bool Validated = inputModel.Validated;

            //VALIDACAO SE DESEJA REALMENTE EXCLUIR A PALAVRA DA BLACKLIST
            if (Validated == false)
            {
                return BadRequest($"Deseja Seguir com a exclusão dessa palavra da BlackList?");
            }
            else
            {
                //VERIFICAR SE JA FOI EXCLUIDA DA BLACKLIST A PALAVRA.
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat($"SELECT IDGDA_PERSONA_BLACKLIST FROM GDA_PERSONA_BLACKLIST (NOLOCK) WHERE IDGDA_PERSONA_BLACKLIST ={IDGDA_BLACKLIST} AND DELETED_AT IS NOT NULL ");
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
                                    return BadRequest($"Palavra ja foi deletada da Blacklist");
                                }
                            }
                        }
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"Erro ao validar Blacklist: {ex.Message}");
                    }
                }

                StringBuilder sb2 = new StringBuilder();
                sb2.Append($"UPDATE GDA_PERSONA_BLACKLIST SET DELETED_AT = GETDATE(), DELETED_BY = {IDCOLLABORATOR} WHERE IDGDA_PERSONA_BLACKLIST = {IDGDA_BLACKLIST}");
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
                        return BadRequest($"Erro ao deletar a palavra na BlackList: {ex.Message}");
                    }
                }
                return Ok("Palavra excluida com sucesso na BlackList");
            }
            
            
        }
        // Método para serializar um DataTable em JSON
    }
}