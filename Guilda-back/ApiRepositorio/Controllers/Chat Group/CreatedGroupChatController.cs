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
    public class CreatedGroupChatController : ApiController
    {// POST: api/Results
        public class InputModel
        {
            public int IDCREATORCOLLABORATOR { get; set; }
            public string TITLE { get; set; }
            public List<User> Users { get; set; }
            public string DESCRIPTION { get; set; }
        }
        public class User
        {
            public int Id { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {
            int IDGDA_CHAT_GROUP = 0;
            int IDCREATORCOLLABORATOR = inputModel.IDCREATORCOLLABORATOR;
            string TITLE = inputModel.TITLE;
            List<User> USERS = inputModel.Users;
            string DESCRIPTION = inputModel.DESCRIPTION;

            //Primeiro cria o GRUPO no banco
             StringBuilder sb = new StringBuilder();
            sb.AppendFormat($"INSERT INTO GDA_CHAT_GROUP(CREATED_BY, TITLE, CREATED_AT, DESCRIPTION) VALUES ({IDCREATORCOLLABORATOR}, '{TITLE}', GETDATE(), '{DESCRIPTION}') SELECT @@IDENTITY AS 'IDGDA_CHAT_GROUP' ");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        IDGDA_CHAT_GROUP = Convert.ToInt32(command.ExecuteScalar());
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    return BadRequest($"Erro ao criar o grupo do colaborador: {ex.Message}");
                }
            }
            //Apos grupo criado, inserimos os usuarios nesse grupo
            //int count = USERS.Count();
            StringBuilder sb2 = new StringBuilder();
            foreach (User collaborator in USERS)
            {
                sb2.AppendFormat($"INSERT INTO GDA_CHAT_GROUP_MEMBER(IDGDA_CHAT_GROUP , IDGDA_CHAT_COLLABORATORS, CREATED_AT) VALUES ({IDGDA_CHAT_GROUP}, {collaborator.Id}, GETDATE()) ");           
            }
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
                    return BadRequest($"Erro ao inserir os usuario no grupo do colaborador criado: {ex.Message}");
                }
            }
            return Ok("Retorno Ok");
        }
        // Método para serializar um DataTable em JSON
    }
}