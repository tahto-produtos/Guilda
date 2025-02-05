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
using ApiC.Class.DowloadFile;
using static TokenService;

namespace ApiRepositorio.Controllers
{

    public class InputModelReaction
    {
        public int idReaction { get; set; }

        public bool flag { get; set; }

        public bool isComment { get; set; }

        public int codPostComment { get; set; }

    }

    public class ReturnModelReaction
    {
        public string link { get; set; }
    }

    //[Authorize]
    public class PersonaReactionController : ApiController
    {
        //Realiza um Post // Realizar Repost
        [HttpPost]
        //[ResponseType(typeof(ResultsModel))]
        public IHttpActionResult PostResultsModel([FromBody] InputModelReaction inputModel)
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

            int codPost = 0;
            int codRepost = 0;
            int codComment = 0;
            if (inputModel.isComment == true)
            {
                codComment = inputModel.codPostComment;
            }
            else
            {
                codPost = inputModel.codPostComment;
            }

            int codOwner = insertReaction(personauserId, collaboratorId, codPost, codComment, inputModel.idReaction, inputModel.flag);

            if (inputModel.flag == true)
            {
                ScheduledNotification.insertNotification(4, "Curtida", $"Você recebeu uma curtida!", codOwner, personauserId);
            }


            ReturnModelReaction ReturnModelReaction = new ReturnModelReaction();
            ReturnModelReaction = returnReaction(inputModel.idReaction, inputModel.flag);
            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(ReturnModelReaction);
        }

        //Visualiza os Posts (Tahto ou Geral



        #region Banco
        public ReturnModelReaction returnReaction(int idReaction, bool flag)
        {
            ReturnModelReaction reaction = new ReturnModelReaction();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT LINK_ICON, LINK_ICON_SELECTED ");
            sb.Append("FROM GDA_PERSONA_POSTS_LIKES_REACTION (NOLOCK) ");
            sb.Append($"WHERE IDGDA_PERSONA_POSTS_LIKES_REACTION = {idReaction} ");

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
                                if (flag == true)
                                {
                                    reaction.link = reader["LINK_ICON_SELECTED"].ToString();
                                }
                                else
                                {
                                    reaction.link = reader["LINK_ICON"].ToString();
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


            return reaction;
        }

        public int insertReaction(int IDGDA_PERSONA_USER, int IDGDA_COLLABORATOR, int IDGDA_PERSONA_POSTS, int IDGDA_PERSONA_POSTS_COMMENTS, int IDGDA_PERSONA_POSTS_LIKES_REACTION, bool FLAG)
        {
            int idOwner = 0;
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                string codPost = IDGDA_PERSONA_POSTS == 0 ? "NULL" : IDGDA_PERSONA_POSTS.ToString();

                string codComment = IDGDA_PERSONA_POSTS_COMMENTS == 0 ? "NULL" : IDGDA_PERSONA_POSTS_COMMENTS.ToString();

                string nomeColuna = "";
                int idUtilizado = 0;
                if (IDGDA_PERSONA_POSTS != 0)
                {
                    nomeColuna = "IDGDA_PERSONA_POSTS";
                    idUtilizado = IDGDA_PERSONA_POSTS;
                }
                else
                {
                    nomeColuna = "IDGDA_PERSONA_POSTS_COMMENTS";
                    idUtilizado = IDGDA_PERSONA_POSTS_COMMENTS;
                }

         



                connection.Open();
                try
                {
                    StringBuilder sbSelectOwnner = new StringBuilder();
                    if (IDGDA_PERSONA_POSTS != 0)
                    {

                        sbSelectOwnner.AppendFormat("SELECT IDGDA_PERSONA_USER FROM GDA_PERSONA_POSTS (NOLOCK) ");
                        sbSelectOwnner.AppendFormat($"WHERE IDGDA_PERSONA_POSTS = {IDGDA_PERSONA_POSTS} ");


                    }
                    else
                    {
                        sbSelectOwnner.AppendFormat("SELECT IDGDA_PERSONA_USER FROM GDA_PERSONA_POSTS_COMMENTS (NOLOCK) ");
                        sbSelectOwnner.AppendFormat($"WHERE IDGDA_PERSONA_POSTS_COMMENTS = {IDGDA_PERSONA_POSTS_COMMENTS} ");
                    }

                    using (SqlCommand commandInsert = new SqlCommand(sbSelectOwnner.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                    idOwner = Convert.ToInt32(reader["IDGDA_PERSONA_USER"].ToString());
                            }
                        }
                    }


                    //Caso seja para inserir uma curtida
                    if (FLAG == true)
                    {
                        int codReaction = 0;
                        //Verifica se esta trocando de reação
                        StringBuilder sbSelect = new StringBuilder();
                        sbSelect.AppendFormat("SELECT * FROM GDA_PERSONA_POSTS_LIKES (NOLOCK) ");
                        sbSelect.AppendFormat("WHERE IDGDA_PERSONA_USER = '{0}' ", IDGDA_PERSONA_USER);
                        sbSelect.AppendFormat("AND {0} = {1} ", nomeColuna, idUtilizado);
                        using (SqlCommand commandInsert = new SqlCommand(sbSelect.ToString(), connection))
                        {
                            using (SqlDataReader reader = commandInsert.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    codReaction = Convert.ToInt32(reader["IDGDA_PERSONA_POSTS_LIKES"].ToString());
                                }
                            }
                        }

                        //Remove a outra reação
                        if (codReaction != 0)
                        {
                            removeReaction(nomeColuna, idUtilizado, connection);
                        }

                        StringBuilder sbInsert = new StringBuilder();
                        sbInsert.AppendFormat("INSERT INTO GDA_PERSONA_POSTS_LIKES (IDGDA_PERSONA_USER, IDGDA_PERSONA_POSTS, IDGDA_PERSONA_POSTS_LIKES_REACTION, IDGDA_PERSONA_POSTS_COMMENTS, IDGDA_COLLABORATORS, CREATED_AT) ");
                        sbInsert.AppendFormat("VALUES ( ");
                        sbInsert.AppendFormat("'{0}', ", IDGDA_PERSONA_USER); //IDGDA_PERSONA_USER
                        sbInsert.AppendFormat("{0}, ", codPost); //IDGDA_PERSONA_POSTS
                        sbInsert.AppendFormat("'{0}', ", IDGDA_PERSONA_POSTS_LIKES_REACTION); //IDGDA_PERSONA_POSTS_LIKES_REACTION
                        sbInsert.AppendFormat("{0}, ", codComment); //IDGDA_PERSONA_POSTS_COMMENTS
                        sbInsert.AppendFormat("'{0}', ", IDGDA_COLLABORATOR); //IDGDA_COLLABORATORS
                        sbInsert.AppendFormat("GETDATE() "); //CREATED_AT
                        sbInsert.AppendFormat(") SELECT @@IDENTITY AS 'CODINSERT' ");

                        using (SqlCommand commandInsert = new SqlCommand(sbInsert.ToString(), connection))
                        {
                            using (SqlDataReader reader = commandInsert.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                  int  codInsert = Convert.ToInt32(reader["CODINSERT"].ToString());
                                }
                            }
                        }
                    }
                    else
                    {

                        //Caso seja para retirar uma curtida
                        removeReaction(nomeColuna, idUtilizado, connection);
                    }
                }
                catch (Exception)
                {
                }
                connection.Close();
            }
            return idOwner;
        }

        public void removeReaction(string nomeColuna, int idUtilizado, SqlConnection connection)
        {
            try
            {
                StringBuilder sbUpdate = new StringBuilder();
                sbUpdate.Append("UPDATE GDA_PERSONA_POSTS_LIKES SET DELETED_AT = GETDATE() WHERE ");
                sbUpdate.AppendFormat("{0} = {1} ", nomeColuna, idUtilizado);

                using (SqlCommand command = new SqlCommand(sbUpdate.ToString(), connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {

            }
        }

        #endregion


    }
}