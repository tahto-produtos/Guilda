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
using System.Drawing;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{



    public class PersonaShowUser
    {
        public string NOME { get; set; }
        public string BC { get; set; }
        public int IDADE { get; set; }
        public int IDADE_CALCULADA { get; set; }
        public string NOME_SOCIAL { get; set; }
        public string FOTO { get; set; }
        public string TIPO_CONTA {  get; set; }
        public string QUEM_E { get; set; }
        public string MOTIVACOES { get; set; }
        public string OBJETIVO { get; set; }
        public List<Hobbies> HOBBIES { get; set; }
        public string EMAIL { get; set; }
        public string TELEFONE { get; set; }
        public string DATA_NASCIMENTO { get; set; }
        public string UF { get; set; }
        public string CIDADE { get; set; }
        public string SITE { get; set; }
        public int ISPUBLIC { get; set; }
        public int FOLLOWERS { get; set; }
        public int FOLLOWING { get; set; }
        public string CARGO { get; set; }
        public bool FOLLOWED_BY_ME { get; set; }
        public string SETOR { get; set; }
        public string ISADM { get; set; }
        public string PERSONAVISION { get; set; }

    }
    //[Authorize]
    public class PersonaShowUserController : ApiController
    {// POST: api/Results
        [HttpGet]
        public IHttpActionResult PostResultsModel(int specificUserId)
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
            personauserId = inf.personauserId;

            bool adm = Funcoes.retornaPermissao(collaboratorId.ToString(), false, false);

            List<PersonaShowUser> rmams = new List<PersonaShowUser>();

            int idBuscado = 0;
            if (specificUserId != 0)
            {
                idBuscado = specificUserId;
            }
            else
            {
                idBuscado = personauserId;
            }

            rmams = BancoShowUser.PersonaShowUser(idBuscado, personauserId, adm);

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(rmams);
        }
        // Método para serializar um DataTable em JSON
    }

    public class BancoShowUser
    {
        public static List<PersonaShowUser> PersonaShowUser(int idBuscado, int idPersona, bool adm = false)
        {
            List<PersonaShowUser> retorno = new List<PersonaShowUser>();
            StringBuilder sb = new StringBuilder();
            string dtAg = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

            sb.Append("SELECT   ");
            sb.Append("	   PU.IDGDA_PERSONA_USER AS ID_PERSONA_USER,  ");
            sb.Append("	   MAX(PU.NAME) AS NOME ,   ");
            sb.Append("	   MAX(CU.IDGDA_COLLABORATORS) AS BC,   ");
            sb.Append("	   MAX(SHOW_AGE) AS IDADE,   ");
            sb.Append("	   MAX(SOCIAL_NAME) AS NOME_SOCIAL,   ");
            sb.Append("	   MAX(PICTURE) AS FOTO,   ");
            sb.Append("    MAX(PU.IDGDA_PERSONA_USER_TYPE) AS TIPOCONTA, ");
            sb.Append("    MAX(PUD.WHO_IS)AS QUEM_E, ");
            sb.Append("	   MAX(PUD.YOUR_MOTIVATIONS) AS MOTIVACOES,   ");
            sb.Append("	   MAX(PUD.GOALS) AS OBJETIVO,  ");
            sb.Append("    PH.IDGDA_PERSONA_HOBBY,  ");
            sb.Append("	   PH.HOBBY,  ");
            sb.Append("	   MAX(PUD.EMAIL) AS EMAIL ,   ");
            sb.Append("	   MAX(PUD.PHONE_NUMBER) AS TELEFONE,   ");
            sb.Append("	   MAX(PUD.BIRTH_DATE) AS DATA_NASCIMENTO,   ");
            sb.Append("	   MAX(ST.STATE) AS UF,   ");
            sb.Append("	   MAX(CT.CITY) AS CIDADE,   ");
            sb.Append("	   MAX(C.SITE) AS SITE,   ");
            sb.Append("	   MAX(PU.IDGDA_PERSONA_USER_VISIBILITY) AS PUBLICS, ");
            sb.Append("	   MAX(C.CARGO) AS CARGO, ");
            sb.Append("	   COUNT(DISTINCT PF1.IDGDA_PERSONA_FOLLOWERS) AS FOLLOWING, ");
            sb.Append("	   COUNT(DISTINCT PF2.IDGDA_PERSONA_FOLLOWERS) AS FOLLOWERS, ");
            sb.Append("    COUNT(DISTINCT PF3.IDGDA_PERSONA_FOLLOWERS) AS FOLLOWEDBYME, ");
            sb.Append($"   (SELECT TOP 1 BALANCE FROM GDA_CHECKING_ACCOUNT (NOLOCK) ");
            sb.Append($"        WHERE COLLABORATOR_ID = {idBuscado} ");
            sb.Append($"        ORDER BY CREATED_AT DESC) AS SALDO, ");
            sb.Append($"   MAX(GR.NAME) AS GRUPO, ");
            sb.Append($"   MAX(SEC.NAME) AS SECTOR, ");
            sb.Append($"   MAX(PCA.NAME) AS PROFILE, ");
            sb.Append($"   MAX(PCA.NO_HIERARCHY) AS NO_HIERARCHY, ");
            sb.Append($"   MAX(CB.PERSONAL_VISION) AS PERSONAL_VISION ");
            sb.Append("FROM GDA_PERSONA_USER  PU (NOLOCK)   ");
            sb.Append("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER CU (NOLOCK) ON CU.IDGDA_PERSONA_USER  = PU.IDGDA_PERSONA_USER   ");
            sb.Append($"LEFT JOIN GDA_COLLABORATORS_DETAILS C (NOLOCK) ON C.IDGDA_COLLABORATORS  = CU.IDGDA_COLLABORATORS AND C.CREATED_AT >= '{dtAg}' ");
            sb.Append("LEFT JOIN GDA_PERSONA_USER_DETAILS PUD (NOLOCK) ON PUD.IDGDA_PERSONA_USER = PU.IDGDA_PERSONA_USER   ");
            sb.Append("LEFT JOIN GDA_PERSONA_USER_HOBBY   PUH (NOLOCK) ON PUH.IDGDA_PERSONA_USER = PU.IDGDA_PERSONA_USER  AND PUH.DELETED_AT IS NULL ");
            sb.Append("LEFT JOIN GDA_PERSONA_HOBBY	       PH  (NOLOCK) ON PH.IDGDA_PERSONA_HOBBY = PUH.IDGDA_PERSONA_HOBBY  AND PH.DELETED_AT IS NULL  ");
            sb.Append("LEFT JOIN GDA_CITY CT (NOLOCK) ON CT.IDGDA_CITY = PUD.IDGDA_CITY ");
            sb.Append("LEFT JOIN GDA_STATE ST (NOLOCK) ON ST.IDGDA_STATE = PUD.IDGDA_STATE ");
            sb.Append("LEFT JOIN GDA_PERSONA_FOLLOWERS (NOLOCK) AS PF1 ON PF1.DELETED_AT IS NULL AND PF1.IDGDA_PERSONA_USER = PU.IDGDA_PERSONA_USER ");
            sb.Append("LEFT JOIN GDA_PERSONA_FOLLOWERS (NOLOCK) AS PF2 ON PF2.DELETED_AT IS NULL AND PF2.IDGDA_PERSONA_USER_FOLLOWED = PU.IDGDA_PERSONA_USER ");
            sb.Append($"LEFT JOIN GDA_PERSONA_FOLLOWERS (NOLOCK) AS PF3 ON PF3.DELETED_AT IS NULL AND PF3.IDGDA_PERSONA_USER = {idPersona} AND PF3.IDGDA_PERSONA_USER_FOLLOWED = PU.IDGDA_PERSONA_USER  ");
            sb.Append($"LEFT JOIN GDA_GROUPS (NOLOCK) GR ON GR.ID = C.IDGDA_GROUP ");
            sb.Append($"LEFT JOIN GDA_SECTOR (NOLOCK) SEC ON SEC.IDGDA_SECTOR = C.IDGDA_SECTOR ");
            sb.Append($"LEFT JOIN GDA_COLLABORATORS (NOLOCK) CB ON CB.IDGDA_COLLABORATORS = CU.IDGDA_COLLABORATORS ");
            sb.Append($"LEFT JOIN GDA_PROFILE_COLLABORATOR_ADMINISTRATION (NOLOCK) PCA ON PCA.ID = CB.PROFILE_COLLABORATOR_ADMINISTRATIONID ");
            sb.Append($"WHERE  PU.IDGDA_PERSONA_USER = {idBuscado} ");
            sb.Append("GROUP BY PU.IDGDA_PERSONA_USER, PH.IDGDA_PERSONA_HOBBY, PH.HOBBY  ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string hobby = reader["HOBBY"].ToString();

                                string teste = reader["IDGDA_PERSONA_HOBBY"].ToString();

                                int idhobby = reader["IDGDA_PERSONA_HOBBY"].ToString() == "" ? 0 : int.Parse(reader["IDGDA_PERSONA_HOBBY"].ToString());

                                

                                // Verifica se já existe um usuário com o mesmo nome na lista
                                PersonaShowUser existingUser = retorno.FirstOrDefault(u => u.NOME == reader["NOME"].ToString());

                                // Se o usuário ainda não existe na lista, cria um novo objeto PersonaUser e adiciona à lista
                                if (existingUser == null)
                                {
                                    int NO_HIERARCHY = reader["NO_HIERARCHY"] != DBNull.Value ? Convert.ToInt32(reader["NO_HIERARCHY"]) : 0;
                                    string CARGO = reader["CARGO"] != DBNull.Value ? reader["CARGO"].ToString() : "";
                                    string PERFIL = reader["PROFILE"] != DBNull.Value ? reader["PROFILE"].ToString() : "";
                                    string CARGOF = NO_HIERARCHY == 0 ? CARGO : PERFIL;



                                    PersonaShowUser newUser = new PersonaShowUser
                                    {
                                        NOME = reader["NOME"] != DBNull.Value ? reader["NOME"].ToString() : "",
                                        BC = reader["BC"] != DBNull.Value ? reader["BC"].ToString() : "",
                                        IDADE = reader["IDADE"] != DBNull.Value ? int.Parse(reader["IDADE"].ToString()) : 0,
                                        NOME_SOCIAL = reader["NOME_SOCIAL"] != DBNull.Value ? reader["NOME_SOCIAL"].ToString() : "",
                                        FOTO = reader["FOTO"] != DBNull.Value ? reader["FOTO"].ToString() : "",
                                        TIPO_CONTA = reader["TIPOCONTA"] != DBNull.Value ? reader["TIPOCONTA"].ToString() : "0",
                                        QUEM_E = returnTables.whoIsHashtags(reader["QUEM_E"].ToString(), reader["GRUPO"].ToString(), reader["SECTOR"].ToString(), reader["CARGO"].ToString(), reader["SALDO"].ToString()),
                                        MOTIVACOES = reader["MOTIVACOES"] != DBNull.Value ? reader["MOTIVACOES"].ToString() : "",
                                        OBJETIVO = reader["OBJETIVO"] != DBNull.Value ? reader["OBJETIVO"].ToString() : "",
                                        EMAIL = reader["EMAIL"] != DBNull.Value ? reader["EMAIL"].ToString() : "",
                                        TELEFONE = adm == true ? reader["TELEFONE"] != DBNull.Value ? reader["TELEFONE"].ToString() : "" : "",
                                        DATA_NASCIMENTO = reader["DATA_NASCIMENTO"] != DBNull.Value ? reader["DATA_NASCIMENTO"].ToString() : "",
                                        UF = reader["UF"] != DBNull.Value ? reader["UF"].ToString() : "",
                                        CIDADE = reader["CIDADE"] != DBNull.Value ? reader["CIDADE"].ToString() : "",
                                        SITE = reader["SITE"] != DBNull.Value ? reader["SITE"].ToString() : "",
                                        HOBBIES = new List<Hobbies>(),  // Instancia uma nova lista de hobbies
                                        ISPUBLIC = reader["PUBLICS"] != DBNull.Value ? Convert.ToInt32(reader["PUBLICS"]) == 2 ? 0 : 1 : 0,
                                        CARGO = CARGOF,
                                        FOLLOWERS = reader["FOLLOWERS"] != DBNull.Value ? Convert.ToInt32(reader["FOLLOWERS"].ToString()) : 0,
                                        FOLLOWING = reader["FOLLOWING"] != DBNull.Value ? Convert.ToInt32(reader["FOLLOWING"].ToString()) : 0,
                                        IDADE_CALCULADA = reader["DATA_NASCIMENTO"] != DBNull.Value ? returnTables.CalcularDiferencaEmAnos(Convert.ToDateTime(reader["DATA_NASCIMENTO"]), DateTime.Today) : 0,
                                        FOLLOWED_BY_ME = reader["FOLLOWEDBYME"] != DBNull.Value ? Convert.ToInt32(reader["FOLLOWEDBYME"].ToString()) > 0 ? true : false : false,
                                        SETOR = reader["SECTOR"] != DBNull.Value ? reader["SECTOR"].ToString() : "",
                                        PERSONAVISION = reader["PERSONAL_VISION"] != DBNull.Value ? reader["PERSONAL_VISION"].ToString() : "",
                                        ISADM = adm == true ? "1" : "",
                                    };

                                    //Regra criada para quando o usuario for seguido por mim, se tornar sempre publico
                                    newUser.ISPUBLIC = newUser.FOLLOWED_BY_ME == true ? 1 : newUser.ISPUBLIC;
                                    newUser.QUEM_E = newUser.ISPUBLIC == 1 ? newUser.QUEM_E : "";
                                    newUser.CARGO = newUser.TIPO_CONTA == "2" ? "Comercial" : newUser.CARGO;
                                    newUser.HOBBIES.Add(new Hobbies { HOBBY = hobby, IDGDA_HOBBIES = idhobby }); // Adiciona o hobby à lista de hobbies do novo usuário
                                    retorno.Add(newUser);   // Adiciona o novo usuário à lista de usuários
                                }
                                else
                                {
                                    existingUser.HOBBIES.Add(new Hobbies { HOBBY = hobby, IDGDA_HOBBIES = idhobby }); // Adiciona o hobby à lista de hobbies do usuário existente
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            return retorno;
        }
    }
}