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
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class RemoveGroupChatController : ApiController
    {// POST: api/Results
        public class InputModel
        {
            public int IDGROUP { get; set; }
        }
        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {
            //REMOVER O GRUPO DE ACORDO COM ID SELECIONADO
            int IDGROUP = inputModel.IDGROUP;
             string SqlUpdateGroup = $"UPDATE GDA_CHAT_GROUP SET DELETED_AT = GETDATE() WHERE IDGDA_CHAT_GROUP = {IDGROUP}";
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(SqlUpdateGroup.ToString(), connection))
                    {
                     command.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"Erro ao deletar o grupo: {ex.Message}");
                }
                connection.Close();
            }
            return Ok("Configuração deletada com sucesso.");
        }

        // Método para serializar um DataTable em JSON
    }
}