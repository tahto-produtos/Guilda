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
    public class CreatedAccountPersonaController : ApiController
    {// POST: api/Results
        public class InputModel
        {
            public string NOME { get; set; }
            public string PEFIL { get; set; }
            public string TYPE { get; set; }
            public string NOME_SOCIAL { get; set; }
            public string FOTO { get; set; }
            public string WHO_IS { get; set; }
            public string MOTIVACOES { get; set; }
            public string OBJETIVO { get; set; }
            public List<Hobbies> HOBBIES { get; set; }
            public string EMAIL { get; set; }
            public string TELEFONE { get; set; }
            public string DATA_NASCIMENTO { get; set; }
            public int UF { get; set; }
            public int CIDADE { get; set; }
            public int VISIBILITY { get; set; }
            public int SITE { get; set; }
            public int PERSONATAHTO { get; set; }
        }
        private void InsertHobbyForUser(int userId, int hobbyId)
        {
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                string query = $"INSERT INTO GDA_PERSONA_USER_HOBBY (IDGDA_PERSONA_USER, IDGDA_PERSONA_HOBBY, CREATED_AT) VALUES ({userId}, {hobbyId}, GETDATE())";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        private void RemoveHobbyForUser(int userId, int hobbyId)
        {
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                string query = $"UPDATE GDA_PERSONA_USER_HOBBY SET DELETED_AT = GETDATE() WHERE IDGDA_PERSONA_USER = {userId} AND IDGDA_PERSONA_HOBBY = {hobbyId}";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel()
        {
            int collaboratorId = 0;
            int personauserId = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            collaboratorId = inf.collaboratorId;


            string idLog = Logs.InsertActionLogs("Created AccountPersonaUser ", "GDA_PERSONA_USER", collaboratorId.ToString());
            string jsonFromFormData = System.Web.HttpContext.Current.Request.Form["json"];
            InputModel Json = JsonConvert.DeserializeObject<InputModel>(jsonFromFormData);
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            List<GalleryResponseModel> pictures = PictureClass.UploadFilesToBlob(files);
            string NAME = Json.NOME;
            string EMAIL = Json.EMAIL;
            int TYPE = 2;
            string PHONE = Json.TELEFONE;
            int STATE = Json.UF;
            int VISIBILITY = Json.VISIBILITY;
            int SITE = Json.SITE;
            int CITY = Json.CIDADE;
            string WHO_IS = Json.WHO_IS;
            int PERSONATAHTO = Json.PERSONATAHTO;

            if (NAME == "")
            {
                return BadRequest("Preencha um nome!");
            }


            string SOCIAL_NAME = Funcoes.FiltroBlackList(Json.NOME_SOCIAL) == false ? Json.NOME_SOCIAL : "Nome Social Error: Palavra Na Blacklist";
            string MOTIVATIONS = Funcoes.FiltroBlackList(Json.MOTIVACOES) == false ? Json.MOTIVACOES : "Motivação Error: Palavra Na Blacklist";
            string OBJETIVES = Funcoes.FiltroBlackList(Json.OBJETIVO) == false ? Json.OBJETIVO : "Observação Error: Palavra Na Blacklist";
            string HOBBY = string.Join(",", Json.HOBBIES.Select(g => g.IDGDA_HOBBIES));
            string PICTURE = "";
            if (files.Count > 0)
            {
                PICTURE = pictures.First().url;
            }

            //VERIFICAÇÃO DE PALAVRAS DA BLACKLIST ANTES DE EDITAR.
            if (SOCIAL_NAME.Contains("Palavra Na Blacklist"))
            {
                return BadRequest("Nome Social com palavras inapropriadas.");
            }
            if (MOTIVATIONS.Contains("Palavra Na Blacklist"))
            {
                return BadRequest("Motivações com palavras inapropriadas.");
            }
            if (OBJETIVES.Contains("Palavra Na Blacklist"))
            {
                return BadRequest("Observação com palavras inapropriadas.");
            }

            //Verifica se esta faltando alguma rota no sistema.
            StringBuilder stbAg = new StringBuilder();
            string dtAg = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

            stbAg.AppendFormat("SELECT COUNT(0) AS QTD ");
            stbAg.AppendFormat("FROM GDA_COLLABORATORS_DETAILS (NOLOCK) ");
            stbAg.AppendFormat($"WHERE CREATED_AT >= '{dtAg}' ");
            int qtdRecebida = 0;
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(stbAg.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                qtdRecebida = int.Parse(reader["QTD"].ToString());
                            }
                        }
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    return BadRequest($"Erro ao inserir conta nova GDA_PERSONA_USER: {ex.Message}");
                }
                connection.Close();
            }

            if (qtdRecebida == 0)
            {
                return BadRequest("Não é possivel criar a conta devido ao não recebimento de dados atuais!");
            }



            #region INSERT GDA_PERSONA_USER
            StringBuilder sbPersonaUser = new StringBuilder();
            sbPersonaUser.Append("INSERT INTO GDA_PERSONA_USER  ");
            sbPersonaUser.Append("(IDGDA_PERSONA_USER_TYPE, IDGDA_PERSONA_USER_VISIBILITY, NAME, SOCIAL_NAME, PICTURE, CREATED_BY, CREATED_AT, TAHTO) ");
            sbPersonaUser.Append("VALUES");
            sbPersonaUser.Append($"('{TYPE}','{VISIBILITY}','{NAME}','{SOCIAL_NAME}','{PICTURE}', '{collaboratorId}', GETDATE(), {PERSONATAHTO} ) ");
            sbPersonaUser.Append("SELECT  @@IDENTITY AS 'IDGDA_PERSONA_USER' ");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sbPersonaUser.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                personauserId = int.Parse(reader["IDGDA_PERSONA_USER"].ToString());
                            }
                        }
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    return BadRequest($"Erro ao inserir conta nova GDA_PERSONA_USER: {ex.Message}");
                }
                connection.Close();
            }
            #endregion

            #region INSERT GDA_PERSONA_USER_DETAILS
            StringBuilder sbPersonaUser02 = new StringBuilder();
            sbPersonaUser02.Append("INSERT INTO GDA_PERSONA_USER_DETAILS  ");
            sbPersonaUser02.Append("(IDGDA_PERSONA_USER, YOUR_MOTIVATIONS, PHONE_NUMBER, GOALS, WHO_IS, EMAIL, IDGDA_STATE, IDGDA_CITY, SITE)  ");
            sbPersonaUser02.Append("VALUES  ");
            sbPersonaUser02.Append($"('{personauserId}', '{MOTIVATIONS}', '{PHONE}', '{OBJETIVES}', '{WHO_IS}', '{EMAIL}', '{STATE}', '{CITY}', '{SITE}')  ");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sbPersonaUser02.ToString(), connection))
                    {
                        command.ExecuteScalar();
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    return BadRequest($"Erro ao inserir  GDA_PERSONA_USER_DETAILS: {ex.Message}");
                }
            }
            #endregion

            #region INSERT GDA_PERSONA_COLLABORATOR_USER
            StringBuilder sbPersonaUser03 = new StringBuilder();
            sbPersonaUser03.Append($"INSERT INTO GDA_PERSONA_COLLABORATOR_USER (IDGDA_COLLABORATORS,IDGDA_PERSONA_USER) VALUES ('{collaboratorId}', '{personauserId}')");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sbPersonaUser03.ToString(), connection))
                    {
                        command.ExecuteScalar();
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    return BadRequest($"Erro ao inserir  GDA_PERSONA_COLLABORATOR_USER: {ex.Message}");
                }
            }
            #endregion

            #region INSERT AND UPDATE GDA_PERSONA_USER_HOBBY
            //Verificar se o Hobby ja ta inserido no banco se precisa ser apagado ou adicionado.

            // Consulta para obter os hobbies do usuário do banco de dados
            List<int> userHobbyIds = Json.HOBBIES.Select(h => h.IDGDA_HOBBIES).ToList();
            List<int> databaseHobbyIds = new List<int>();
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    string query = $"SELECT IDGDA_PERSONA_HOBBY FROM GDA_PERSONA_USER_HOBBY (NOLOCK) WHERE DELETED_AT IS NULL AND IDGDA_PERSONA_USER = {personauserId}";
                    using (SqlCommand command = new SqlCommand(query.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                databaseHobbyIds.Add(reader.GetInt32(0));
                            }
                        }
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    return BadRequest($"Erro ao consultar Hobby Desse Usuario {personauserId}: {ex.Message}");
                }
            }

            // Insere hobbies ausentes na lista do usuário
            foreach (int hobbyId in userHobbyIds)
            {
                if (!databaseHobbyIds.Contains(hobbyId))
                {
                    InsertHobbyForUser(personauserId, hobbyId);
                }
            }

            // Remove hobbies não presentes na lista do usuário
            foreach (int hobbyId in databaseHobbyIds)
            {
                if (!userHobbyIds.Contains(hobbyId))
                {
                    RemoveHobbyForUser(personauserId, hobbyId);
                }
            }
            #endregion

            #region INSERT LOG PERSONA_USER
            Logs.InsertActionPersonaUser(SOCIAL_NAME, 0, PICTURE, collaboratorId.ToString(), MOTIVATIONS, OBJETIVES, "", "", STATE, CITY, WHO_IS, EMAIL);
            #endregion
            return Ok("Conta criada com Sucesso.");


        }
        // Método para serializar um DataTable em JSON
    }
}