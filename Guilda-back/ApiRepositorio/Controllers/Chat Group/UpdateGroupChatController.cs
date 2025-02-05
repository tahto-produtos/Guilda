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
using static ApiRepositorio.Controllers.CreatedGroupChatController;
using Utilities;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class UpdateGroupChatController : ApiController
    {
        public class InputModel
        {
            public int IDCREATORCOLLABORATOR { get; set; }
            public int IDGRUOP { get; set; }
            public string TITLE { get; set; }
            public string DESCRIPTION { get; set; }
            public List<User> Users { get; set; }
        }
        public class User
        {
            public int Id { get; set; }
        }
        // POST: api/Results
        [HttpPut]
        public IHttpActionResult PutResultsModel([FromBody] InputModel inputModel)
        {
            int validatorCollaborator = 0;
            int IDCREATORCOLLABORATOR = inputModel.IDCREATORCOLLABORATOR;
            int IDGRUOP = inputModel.IDGRUOP;
            string TITLE = inputModel.TITLE;
            List<User> USERS = inputModel.Users;
            string DESCRIPTION = inputModel.DESCRIPTION;

            //Update nas informações do grupo criado.
             StringBuilder sb = new StringBuilder();
            sb.AppendFormat($"UPDATE GDA_CHAT_GROUP SET TITLE = '{TITLE}', DESCRIPTION = '{DESCRIPTION}', UPDATED_BY = {IDCREATORCOLLABORATOR}, UPDATED_AT = GETDATE() WHERE IDGDA_CHAT_GROUP ={IDGRUOP}");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        command.ExecuteScalar();
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    return BadRequest($"Erro ao criar o grupo do colaborador: {ex.Message}");
                }
            }
            //Verificação se Usuario está no grupo, caso não esteja, insere no grupo.
            StringBuilder sb2 = new StringBuilder();
            foreach (User collaborator in USERS)
            {
                sb2.AppendFormat($"SELECT * FROM GDA_CHAT_GROUP_MEMBER (NOLOCK) WHERE IDGDA_CHAT_GROUP ={IDGRUOP} AND IDGDA_CHAT_COLLABORATORS = {collaborator.Id}");

                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(sb2.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    validatorCollaborator = +1;
                                }
                            }
                        }
                        string insert = "";
                        if (validatorCollaborator == 0)
                        {
                            insert = $"INSERT INTO GDA_CHAT_GROUP_MEMBER(IDGDA_CHAT_GROUP , IDGDA_CHAT_COLLABORATORS) VALUES ({IDGRUOP}, {collaborator.Id})";
                            using (SqlCommand command = new SqlCommand(insert, connection))
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"Erro ao inserir os usuario no grupo do colaborador criado: {ex.Message}");
                    }
                    sb2 = new StringBuilder();
                    validatorCollaborator = 0;
                    connection.Close();
                }
            }
            return Ok("Retorno Ok");
        }

        // Método para serializar um DataTable em JSON
    }
}