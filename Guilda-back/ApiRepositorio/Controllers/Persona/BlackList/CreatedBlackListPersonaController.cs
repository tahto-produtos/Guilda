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
    public class CreatedBlackListPersonaController : ApiController
    {// POST: api/Results
        public class InputModel
        {
            public int IDCREATORCOLLABORATOR { get; set; }
            public string WORD { get; set; }
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

            string idLog = Logs.InsertActionLogs("INSERT CreatedBlackListPersona ", "GDA_PERSONA_BLACKLIST", collaboratorId.ToString());
            int IDCREATORCOLLABORATOR = collaboratorId;
            string WORD = inputModel.WORD;
            bool ValidaBlackList = false;
            //Verifica se Essa Palavra Já está criada Na BlackList, caso não esteja insere no banco.
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat($"SELECT IDGDA_PERSONA_BLACKLIST, WORD FROM GDA_PERSONA_BLACKLIST (NOLOCK) WHERE WORD = '{WORD}' AND DELETED_AT IS NULL ");
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
                                ValidaBlackList = true;
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

            if (ValidaBlackList == true)
            {
                return BadRequest($"Palavra ja cadastrada na BlackList");
            }
            else 
            {
                StringBuilder sb2 = new StringBuilder();
                sb2.AppendFormat($"INSERT INTO GDA_PERSONA_BLACKLIST (WORD, CREATED_AT, CREATED_BY) VALUES ('{WORD}',GETDATE(), {IDCREATORCOLLABORATOR}) ");
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
                        return BadRequest($"Erro ao inserir a palavra na BlackList: {ex.Message}");
                    }
                }
                return Ok("Palavra cadastrada com sucesso na BlackList");
            }
            
        }
        // Método para serializar um DataTable em JSON
    }
}