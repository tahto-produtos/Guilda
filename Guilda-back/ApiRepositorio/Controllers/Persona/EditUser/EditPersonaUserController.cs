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
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using System.Drawing;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Bibliography;
using System.Web.Helpers;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class EditPersonaUserController : ApiController
    {// POST: api/Results
        public class InputModel
        {
            public int IDPERSONAUSER { get; set; }
            public string NOME { get; set; }
            public string PEFIL { get; set; }
            public string BC { get; set; }
            public int IDADE { get; set; }
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
            public string SITE { get; set; }
            public int PERSONATAHTO { get; set; }
        }
        private void InsertHobbyForUser(int userId, int hobbyId)
        {
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    string query = $"INSERT INTO GDA_PERSONA_USER_HOBBY (IDGDA_PERSONA_USER, IDGDA_PERSONA_HOBBY, CREATED_AT) VALUES ({userId}, {hobbyId}, GETDATE())";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
        }
        private void RemoveHobbyForUser(int userId, int hobbyId)
        {
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    string query = $"UPDATE GDA_PERSONA_USER_HOBBY SET DELETED_AT = GETDATE() WHERE IDGDA_PERSONA_USER = {userId} AND IDGDA_PERSONA_HOBBY = {hobbyId}";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
        }
        [HttpPost]
        //public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
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


            string idLog = Logs.InsertActionLogs("Insert EditPersonaUser ", "GDA_PERSONA_USER", collaboratorId.ToString());
            string jsonFromFormData = System.Web.HttpContext.Current.Request.Form["json"];
            InputModel Json = JsonConvert.DeserializeObject<InputModel>(jsonFromFormData);
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            List<GalleryResponseModel> pictures = PictureClass.UploadFilesToBlob(files);

            //REALIZAR A VERIFICAÇÃO DE QUEM TA FAZENDO EDIÇÃO SENDO UM USUARIO COMUM OU CRUD.
            string EDIT_IDCOLLABORATOR = collaboratorId.ToString();
            int IDPERSONAUSER = Json.IDPERSONAUSER;
            string NOME = "";
            string BC = "";
            string EMAIL = "";
            string TELEFONE = "";
            string DATANASC = "";
            int UF = 0;
            string SITE = "";
            int CIDADE = 0;
            string QUEM_E = "";
            int PERSONATAHTO = 0;
            bool adm = Funcoes.retornaPermissao(EDIT_IDCOLLABORATOR.ToString());
            if (adm == true)
            {
                NOME = Json.NOME;
                BC = Json.BC;
                EMAIL = Json.EMAIL;
                TELEFONE = Json.TELEFONE;
                DATANASC = Json.DATA_NASCIMENTO;
                UF = Json.UF;
                SITE = Json.SITE;
                CIDADE = Json.CIDADE;
                QUEM_E = Json.WHO_IS;
                PERSONATAHTO = Json.PERSONATAHTO;
            }
            string NOME_SOCIAL = Funcoes.FiltroBlackList(Json.NOME_SOCIAL) == false ? Json.NOME_SOCIAL : "Nome Social Error: Palavra Na Blacklist";
            string MOTIVACOES = Funcoes.FiltroBlackList(Json.MOTIVACOES) == false ? Json.MOTIVACOES : "Motivação Error: Palavra Na Blacklist";
            string OBJETIVO = Funcoes.FiltroBlackList(Json.OBJETIVO) == false ? Json.OBJETIVO : "Objetivo Error: Palavra Na Blacklist";
            string HOBBY = string.Join(",", Json.HOBBIES.Select(g => g.IDGDA_HOBBIES));
            int IDADE = Json.IDADE;
            string FOTO = "";
            if (files.Count > 0)
            {
                FOTO = pictures.First().url;
            }



            //VERIFICAÇÃO DE PALAVRAS DA BLACKLIST ANTES DE EDITAR.
            if (NOME_SOCIAL.Length > 45)
            {
                return BadRequest("Nome social com mais de 45 caracteres!");
            }
            if (NOME_SOCIAL.Contains("Palavra Na Blacklist"))
            {
                return BadRequest("Nome Social com palavras inapropriadas.");
            }
            if (MOTIVACOES.Contains("Palavra Na Blacklist"))
            {
                return BadRequest("Motivações com palavras inapropriadas.");
            }
            if (OBJETIVO.Contains("Palavra Na Blacklist"))
            {
                return BadRequest("Objetivo com palavras inapropriadas.");
            }

            #region UPDATE GDA_PERSONA_USER
            //UPDATE NA GDA_PERSONA_USER
            //NAME = NOME
            //BC = BC
            //SOCIAL_NAME = NOME_SOCIAL
            //SHOW_AGE = IDADE
            //PICTURE = FOTO
            StringBuilder sbPersonaUser = new StringBuilder();
            sbPersonaUser.Append("UPDATE GDA_PERSONA_USER SET  ");
            if (adm == true)
            {
                sbPersonaUser.Append($"NAME  = '{NOME}',  ");
                sbPersonaUser.Append($"BC = '{BC}',  ");
                sbPersonaUser.Append($"TAHTO = '{PERSONATAHTO}', ");
            }
            sbPersonaUser.Append($"SOCIAL_NAME = '{NOME_SOCIAL}',  ");
            //if (FOTO !="")
            //{
            sbPersonaUser.Append($"PICTURE = '{FOTO}',  ");
            //}
            sbPersonaUser.Append($"SHOW_AGE = {IDADE}  ");

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

            #region UPDATE GDA_PERSONA_USER_DETAILS
            //GDA_PERSONA_USER_DETAILS
            //YOUR_MOTIVATIONS = MOTIVACOES
            //GOALS = OBJETIVO
            //PHONE_NUMBER = TELEFONE
            //BIRTH_DATE = DATANASC
            //UF = UF
            //CITY = CIDADE
            //WHO_IS = QUEM_E
            //EMAIL = EMAIL
            StringBuilder sbPersonaUserDetails = new StringBuilder();
            sbPersonaUserDetails.Append("UPDATE GDA_PERSONA_USER_DETAILS SET  ");
            if (adm == true)
            {
                sbPersonaUserDetails.Append($"PHONE_NUMBER ='{TELEFONE}',  ");
                sbPersonaUserDetails.Append($"BIRTH_DATE ='{DATANASC}',  ");
                sbPersonaUserDetails.Append($"IDGDA_STATE = '{UF}',  ");
                sbPersonaUserDetails.Append($"IDGDA_CITY ='{CIDADE}',  ");
                sbPersonaUserDetails.Append($"WHO_IS = '{QUEM_E}',  ");
                sbPersonaUserDetails.Append($"EMAIL = '{EMAIL}',  ");
                sbPersonaUserDetails.Append($"SITE = '{SITE}',  ");
            }
            sbPersonaUserDetails.Append($"YOUR_MOTIVATIONS = '{MOTIVACOES}',  ");
            sbPersonaUserDetails.Append($"GOALS = '{OBJETIVO}'  ");
            sbPersonaUserDetails.Append($"WHERE IDGDA_PERSONA_USER = {IDPERSONAUSER} ");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sbPersonaUserDetails.ToString(), connection))
                    {
                        command.ExecuteScalar();
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    return BadRequest($"Erro ao Atualizar GDA_PERSONA_USER_DETAILS: {ex.Message}");
                }
            }
            #endregion

            #region INSERT AND UPDATE GDA_PERSONA_USER_HOBBY
            //Verificar se o Hobby ja ta inserido no banco se precisa ser apagado ou adicionado.
            //GDA_PERSONA_USER_HOBBY
            //IDGDA_PERSONA_USER
            //IDGA_PERSONA_HOBBY
            //CREATED_AT

            // Consulta para obter os hobbies do usuário do banco de dados
            List<int> userHobbyIds = Json.HOBBIES.Select(h => h.IDGDA_HOBBIES).ToList();
            List<int> databaseHobbyIds = new List<int>();
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    string query = $"SELECT IDGDA_PERSONA_HOBBY FROM GDA_PERSONA_USER_HOBBY (NOLOCK) WHERE DELETED_AT IS NULL AND IDGDA_PERSONA_USER = {IDPERSONAUSER}";
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
                    return BadRequest($"Erro ao consultar Hobby Desse Usuario {IDPERSONAUSER}: {ex.Message}");
                }
            }

            // Insere hobbies ausentes na lista do usuário
            foreach (int hobbyId in userHobbyIds)
            {
                if (!databaseHobbyIds.Contains(hobbyId))
                {
                    InsertHobbyForUser(IDPERSONAUSER, hobbyId);
                }
            }

            // Remove hobbies não presentes na lista do usuário
            foreach (int hobbyId in databaseHobbyIds)
            {
                if (!userHobbyIds.Contains(hobbyId))
                {
                    RemoveHobbyForUser(IDPERSONAUSER, hobbyId);
                }
            }
            #endregion

            #region INSERT LOG PERSONA_USER
            Logs.InsertActionPersonaUser(NOME_SOCIAL, IDADE, FOTO, EDIT_IDCOLLABORATOR, MOTIVACOES, OBJETIVO, TELEFONE, DATANASC, UF, CIDADE, QUEM_E, EMAIL);
            #endregion
            return Ok("Atualizações Realizada com Sucesso.");

        }
        // Método para serializar um DataTable em JSON
    }
}