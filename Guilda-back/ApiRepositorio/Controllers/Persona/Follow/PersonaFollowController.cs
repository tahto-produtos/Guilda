using System.Data.SqlClient;
using System.Web.Http;
using System.Web.Http.Description;
using ApiRepositorio.Models;
using System.Web;
using System;
using System.Data;
using ApiC.Class;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using static TokenService;

namespace ApiRepositorio.Controllers
{

    public class InputModelPersonaFollow
    {
        public int idFollowed { get; set; }
        public bool follow { get; set; }
    }

    public class returnPersonaFollow
    {
        public int TOTALPAGES { get; set; }
        public List<listsFollows> listsFollows { get; set; }

    }

    public class listsFollows
    {
        public string idPersona { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string hierarchy { get; set; }
    }

    //[Authorize]
    public class PersonaFollowController : ApiController
    {
        //Realiza um Post // Realizar Repost
        [HttpPost]
        //[ResponseType(typeof(ResultsModel))]
        public IHttpActionResult PostResultsModel([FromBody] InputModelPersonaFollow inputModel)
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

            //IDGDA_PERSONA_POSTS
            //COMMENT

            if (personauserId == inputModel.idFollowed)
            {
                return BadRequest("Não é possivel seguir o proprio perfil!");
            }

            bool retorno = BancoFollowers.insertFollowUnfollow(personauserId, inputModel.idFollowed, inputModel.follow);

            // Use o método Ok() para retornar o objeto serializado em JSON
            if (retorno == true)
            {
                return Ok("Follow ou Unfollow Concluido!");
            }
            else
            {
                return Ok("Nenhuma alteração realizada!");
            }
        }


        [HttpGet]
        [ResponseType(typeof(returnPersonaFollow))]
        public IHttpActionResult GetResultsModel(bool follow, int idPersona, string filterName, int limit, int page)
        {
            returnPersonaFollow users = new returnPersonaFollow();

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

            users = BancoFollowers.returnFollowers(idPersona, follow, filterName, limit, page);

            

            //Visibilidade e ordenação


            return Ok(users);
        }





    }


    public class BancoFollowers
    {
        #region Banco

        public static bool insertFollowUnfollow(int personauserId, int idFollowed, bool follow)
        {
            bool followed = false;
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    int codFollow = 0;
                    StringBuilder sbSelect = new StringBuilder();
                    sbSelect.AppendFormat("SELECT IDGDA_PERSONA_FOLLOWERS FROM GDA_PERSONA_FOLLOWERS (NOLOCK) WHERE  ");
                    sbSelect.AppendFormat("IDGDA_PERSONA_USER = {0} AND  ", personauserId);
                    sbSelect.AppendFormat("IDGDA_PERSONA_USER_FOLLOWED = {0} AND  ", idFollowed);
                    sbSelect.AppendFormat("DELETED_AT IS NULL ");

                    using (SqlCommand commandInsert = new SqlCommand(sbSelect.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                codFollow = Convert.ToInt32(reader["IDGDA_PERSONA_FOLLOWERS"].ToString());
                            }
                        }
                    }

                    //Seguir
                    if (follow == true)
                    {
                        if (codFollow > 0)
                        {
                            followed = false;
                            //Ja existe
                        }
                        else
                        {
                            StringBuilder sbInsert = new StringBuilder();
                            sbInsert.AppendFormat("INSERT INTO GDA_PERSONA_FOLLOWERS (IDGDA_PERSONA_USER, IDGDA_PERSONA_USER_FOLLOWED, CREATED_AT, DELETED_AT) VALUES ( ");
                            sbInsert.AppendFormat("{0},  ", personauserId);
                            sbInsert.AppendFormat("{0},  ", idFollowed);
                            sbInsert.AppendFormat("GETDATE(),  ");
                            sbInsert.AppendFormat("NULL ");
                            sbInsert.AppendFormat(") ");


                            using (SqlCommand commandInsert = new SqlCommand(sbInsert.ToString(), connection))
                            {
                                commandInsert.ExecuteNonQuery();
                            }
                            followed = true;
                        }

                    }
                    //Não seguir
                    else
                    {
                        if (codFollow > 0)
                        {
                            StringBuilder sbInsert = new StringBuilder();
                            sbInsert.AppendFormat("UPDATE GDA_PERSONA_FOLLOWERS SET ");
                            sbInsert.AppendFormat("DELETED_AT = GETDATE() ");
                            sbInsert.AppendFormat("WHERE ");
                            sbInsert.AppendFormat("IDGDA_PERSONA_FOLLOWERS = {0} ", codFollow);

                            using (SqlCommand commandInsert = new SqlCommand(sbInsert.ToString(), connection))
                            {
                                commandInsert.ExecuteNonQuery();
                            }
                            followed = true;
                        }
                        else
                        {
                            followed = false;
                            //Não existe
                        }
                    }


                }
                catch (Exception)
                {
                }
                connection.Close();
            }
            return followed;
        }

        public static returnPersonaFollow returnFollowers(int personauserId, bool follow, string filterName, int limit, int page)
        {

            returnPersonaFollow returnPersonaFollows = new returnPersonaFollow();
            returnPersonaFollows.listsFollows = new List<listsFollows>();

            //Listo todas as postagens e repostagens do banco
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    //787450
                    //13180
                    string filter = "";
                    if (string.IsNullOrEmpty(filterName) == false)
                    {
                        filter = $" PU.NAME LIKE '%{filterName}%' AND ";
                    }

                    string dtAg = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                    StringBuilder stb = new StringBuilder();

                    if (follow == true)
                    {
                        int totalInfo = quantidadeSeguindo(personauserId, filter);
                        int totalpage = (int)Math.Ceiling((double)totalInfo / limit);
                        int offset = (page - 1) * limit;
                        returnPersonaFollows.TOTALPAGES = totalpage;
                        stb.AppendFormat("SELECT PU.IDGDA_PERSONA_USER, PU.NAME, PU.PICTURE AS URL, CD.CARGO FROM GDA_PERSONA_FOLLOWERS (NOLOCK) PF ");
                        stb.AppendFormat("INNER JOIN GDA_PERSONA_USER (NOLOCK) PU ON PF.IDGDA_PERSONA_USER_FOLLOWED = PU.IDGDA_PERSONA_USER ");
                        stb.AppendFormat("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER CU ON CU.IDGDA_PERSONA_USER = PU.IDGDA_PERSONA_USER AND PU.IDGDA_PERSONA_USER_TYPE = 1 ");
                        stb.AppendFormat("LEFT JOIN GDA_COLLABORATORS_DETAILS CD ON CD.CREATED_AT >= '{0}' AND CD.IDGDA_COLLABORATORS = CU.IDGDA_COLLABORATORS ", dtAg);
                        stb.AppendFormat("WHERE PF.IDGDA_PERSONA_USER = {0} AND ", personauserId);
                        stb.Append($"{filter} ");
                        stb.AppendFormat("PF.DELETED_AT IS NULL ");
                        stb.AppendFormat("GROUP BY PU.IDGDA_PERSONA_USER, PU.NAME, PU.PICTURE, CD.CARGO ");
                        stb.Append("ORDER BY PU.IDGDA_PERSONA_USER ");
                        stb.Append($"OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY");
                    }
                    else
                    {
                        int totalInfo = quantidadeSeguidores(personauserId, filter);
                        int totalpage = (int)Math.Ceiling((double)totalInfo / limit);
                        int offset = (page - 1) * limit;
                        returnPersonaFollows.TOTALPAGES = totalpage;
                        stb.AppendFormat("SELECT PU.IDGDA_PERSONA_USER, PU.NAME, PU.PICTURE AS URL, CD.CARGO FROM GDA_PERSONA_FOLLOWERS (NOLOCK) PF ");
                        stb.AppendFormat("INNER JOIN GDA_PERSONA_USER (NOLOCK) PU ON PF.IDGDA_PERSONA_USER = PU.IDGDA_PERSONA_USER ");
                        stb.AppendFormat("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER CU ON CU.IDGDA_PERSONA_USER = PU.IDGDA_PERSONA_USER AND PU.IDGDA_PERSONA_USER_TYPE = 1 ");
                        stb.AppendFormat("LEFT JOIN GDA_COLLABORATORS_DETAILS CD ON CD.CREATED_AT >= '{0}' AND CD.IDGDA_COLLABORATORS = CU.IDGDA_COLLABORATORS ", dtAg);
                        stb.AppendFormat("WHERE PF.IDGDA_PERSONA_USER_FOLLOWED = {0} AND  ", personauserId);
                        stb.Append($"{filter} ");
                        stb.AppendFormat("PF.DELETED_AT IS NULL ");
                        stb.AppendFormat("GROUP BY PU.IDGDA_PERSONA_USER, PU.NAME, PU.PICTURE, CD.CARGO ");
                        stb.Append("ORDER BY PU.IDGDA_PERSONA_USER ");
                        stb.Append($"OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY");
                    }


                    using (SqlCommand commandInsert = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listsFollows listsFs = new listsFollows();
                                listsFs.idPersona = reader["IDGDA_PERSONA_USER"] != DBNull.Value ? reader["IDGDA_PERSONA_USER"].ToString() : "";
                                listsFs.name = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "";
                                listsFs.url = reader["URL"] != DBNull.Value ? reader["URL"].ToString() : "";
                                listsFs.hierarchy = reader["CARGO"] != DBNull.Value ? reader["CARGO"].ToString() : "";

                                returnPersonaFollows.listsFollows.Add(listsFs);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }


            return returnPersonaFollows;
        }

        public static int quantidadeSeguindo(int personauserId, string filter)
        {
            StringBuilder stb = new StringBuilder();
            int total = 0;
            string dtAg = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

            stb.Append($"SELECT COUNT(0) FROM GDA_PERSONA_FOLLOWERS (NOLOCK) PF ");
            stb.Append($"INNER JOIN GDA_PERSONA_USER (NOLOCK) PU ON PF.IDGDA_PERSONA_USER_FOLLOWED = PU.IDGDA_PERSONA_USER ");
            stb.Append($"LEFT JOIN GDA_PERSONA_COLLABORATOR_USER CU ON CU.IDGDA_PERSONA_USER = PU.IDGDA_PERSONA_USER AND PU.IDGDA_PERSONA_USER_TYPE = 1 ");
            stb.Append($"LEFT JOIN GDA_COLLABORATORS_DETAILS CD ON CD.CREATED_AT >= '{dtAg}' AND CD.IDGDA_COLLABORATORS = CU.IDGDA_COLLABORATORS ");
            stb.Append($"WHERE PF.IDGDA_PERSONA_USER = {personauserId} AND ");
            stb.Append($"{filter} ");
            stb.Append($"PF.DELETED_AT IS NULL ");
            //stb.Append($"GROUP BY PU.IDGDA_PERSONA_USER, PU.NAME, PU.PICTURE, CD.CARGO ");


            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        // Executando o comando e armazenando o resultado na variável 'total'
                        total = (int)command.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            return total;
        }

        public static int quantidadeSeguidores(int personauserId, string filter)
        {
            StringBuilder stb = new StringBuilder();

            int total = 0;
            string dtAg = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

            stb.Append($"SELECT COUNT(0) FROM GDA_PERSONA_FOLLOWERS (NOLOCK) PF ");
            stb.Append($"INNER JOIN GDA_PERSONA_USER (NOLOCK) PU ON PF.IDGDA_PERSONA_USER = PU.IDGDA_PERSONA_USER ");
            stb.Append($"LEFT JOIN GDA_PERSONA_COLLABORATOR_USER CU ON CU.IDGDA_PERSONA_USER = PU.IDGDA_PERSONA_USER AND PU.IDGDA_PERSONA_USER_TYPE = 1 ");
            stb.Append($"LEFT JOIN GDA_COLLABORATORS_DETAILS CD ON CD.CREATED_AT >= '{dtAg}' AND CD.IDGDA_COLLABORATORS = CU.IDGDA_COLLABORATORS ");
            stb.Append($"WHERE PF.IDGDA_PERSONA_USER_FOLLOWED = {personauserId} AND  ");
            stb.Append($"{filter} ");
            stb.Append($"PF.DELETED_AT IS NULL ");
            //stb.Append($"GROUP BY PU.IDGDA_PERSONA_USER, PU.NAME, PU.PICTURE, CD.CARGO ");


            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        // Executando o comando e armazenando o resultado na variável 'total'
                        total = (int)command.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            return total;
        }


        #endregion
    }

}