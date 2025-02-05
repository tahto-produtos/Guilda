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

    public class InputModel
    {
        public string comment { get; set; }
        public int codPost { get; set; }

    }


    public class getCommentFollow
    {

        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string hierarchy { get; set; }
        public string comment { get; set; }
        public string timeAgo { get; set; }
        public bool canDeleteComment { get; set; }
    }


    public class returnGetCommentFollow
    {
        public int TOTALPAGES { get; set; }

        public List<getCommentFollow> listsComments { get; set; }

    }


    //[Authorize]
    public class PersonaCommentController : ApiController
    {
        //Realiza um Post // Realizar Repost
        [HttpPost]
        //[ResponseType(typeof(ResultsModel))]
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
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
            //string comment = inputModel.comment.ToString();
            string comment = Funcoes.FiltroBlackList(inputModel.comment) == false ? inputModel.comment : "Comment Error: Palavra Na Blacklist";
            int codPost = inputModel.codPost;

            bool canC = BancoComments.canComment(codPost);
            if (canC == false)
            {
                return BadRequest("Esse post não aceita comentarios!");
            }

            if (comment.Contains("Palavra Na Blacklist"))
            {
                return BadRequest("Comentario com palavras inapropriadas.");
            }
            BancoComments.insertComment(personauserId, collaboratorId, comment, codPost);

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok("Comentario Inserido!");
        }



        [HttpGet]
        //[ResponseType(typeof(ResultsModel))]
        public IHttpActionResult GetResultsModel(int codPost, bool isAdm, int limit, int page)
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

            returnGetCommentFollow returnGetCommentFollow = new returnGetCommentFollow();

            int totalInfo = BancoComments.quantidadeComentarios(codPost);
            //int totalpage = totalInfo / limit;
            int totalpage = (int)Math.Ceiling((double)totalInfo / limit);
            int offset = (page - 1) * limit;
            returnGetCommentFollow.TOTALPAGES = totalpage;

            returnGetCommentFollow.listsComments = new List<getCommentFollow>();
            returnGetCommentFollow.listsComments = BancoComments.getComments(codPost, isAdm, limit, offset, personauserId);


            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(returnGetCommentFollow);
        }

        [HttpDelete]
        public IHttpActionResult DeleteResultsModel(int idComment, bool isAdm)
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

            bool ok = BancoComments.deletetComment(idComment, personauserId, isAdm);

            if (ok == true)
            {
                return Ok("Comentario deletado!");
            }
            else
            {
                return BadRequest("Não foi possivel deletar o comentario!");
            }

        }





    }

    public class BancoComments
    {
        #region Banco

        public static bool canComment(int idPost)
        {
            bool retorno = true;

            StringBuilder stb = new StringBuilder();
            stb.Append("SELECT ALLOW_COMMENT FROM GDA_PERSONA_POSTS (NOLOCK) ");
            stb.Append($"WHERE IDGDA_PERSONA_POSTS = {idPost} ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand commandInsert = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                retorno = reader["ALLOW_COMMENT"] != DBNull.Value ? Convert.ToInt32(reader["ALLOW_COMMENT"]) == 1 ? true : false : true;
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

        public static int quantidadeComentarios(int codPost)
        {
            int total = 0;

            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("DECLARE @DATAATUAL DATE; SET @DATAATUAL = CONVERT(DATE, DATEADD(DAY, -1, GETDATE())); ");
            stb.AppendFormat("SELECT COUNT(DISTINCT PPC.IDGDA_PERSONA_POSTS_COMMENTS) ");
            stb.AppendFormat("FROM GDA_PERSONA_POSTS_COMMENTS (NOLOCK) PPC ");
            stb.AppendFormat("INNER JOIN GDA_PERSONA_USER (NOLOCK) PU ON PU.IDGDA_PERSONA_USER = PPC.IDGDA_PERSONA_USER ");
            stb.AppendFormat("LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CD ON CD.IDGDA_COLLABORATORS = PPC.IDGDA_COLLABORATOR AND CD.CREATED_AT >= @DATAATUAL ");
            stb.AppendFormat("WHERE IDGDA_PERSONA_POSTS = {0} AND PPC.DELETED_AT IS NULL ", codPost);

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

        public static List<getCommentFollow> getComments(int codPost, bool isAdm, int limit, int offset, int personaId)
        {

            List<getCommentFollow> listComments = new List<getCommentFollow>();

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    StringBuilder sbInsert = new StringBuilder();
                    sbInsert.AppendFormat("DECLARE @DATAATUAL DATE; SET @DATAATUAL = CONVERT(DATE, DATEADD(DAY, -1, GETDATE())); ");
                    sbInsert.AppendFormat("SELECT PPC.IDGDA_PERSONA_POSTS_COMMENTS, PU.NAME, PU.PICTURE, CD.CARGO, PPC.COMMENT, PPC.IDGDA_PERSONA_USER AS ID_USER_COMMENT, PP.IDGDA_PERSONA_USER AS ID_USER_POST, ");
                    sbInsert.AppendFormat("CASE  ");
                    sbInsert.AppendFormat("       WHEN DATEDIFF(MINUTE, PPC.CREATED_AT, GETDATE()) < 60 THEN CONCAT(DATEDIFF(MINUTE, PPC.CREATED_AT, GETDATE()), ' MINUTOS')  ");
                    sbInsert.AppendFormat("        WHEN DATEDIFF(HOUR, PPC.CREATED_AT, GETDATE()) < 24 THEN CONCAT(DATEDIFF(HOUR, PPC.CREATED_AT, GETDATE()), ' HORAS')  ");
                    sbInsert.AppendFormat("        ELSE CONCAT(DATEDIFF(DAY, PPC.CREATED_AT, GETDATE()), ' DIAS')  ");
                    sbInsert.AppendFormat("    END AS TIMEAGO ");
                    sbInsert.AppendFormat("FROM GDA_PERSONA_POSTS_COMMENTS (NOLOCK) PPC ");
                    sbInsert.AppendFormat("INNER JOIN GDA_PERSONA_USER (NOLOCK) PU ON PU.IDGDA_PERSONA_USER = PPC.IDGDA_PERSONA_USER ");
                    sbInsert.AppendFormat("LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CD ON CD.IDGDA_COLLABORATORS = PPC.IDGDA_COLLABORATOR AND CD.CREATED_AT >= @DATAATUAL ");
                    sbInsert.AppendFormat("LEFT JOIN GDA_PERSONA_POSTS (NOLOCK) AS PP ON PP.IDGDA_PERSONA_POSTS = PPC.IDGDA_PERSONA_POSTS ");
                    sbInsert.AppendFormat("WHERE PPC.IDGDA_PERSONA_POSTS = {0} AND PPC.DELETED_AT IS NULL ", codPost);
                    sbInsert.AppendFormat("GROUP BY PU.NAME, PU.PICTURE, CD.CARGO, PPC.CREATED_AT, PPC.IDGDA_PERSONA_POSTS_COMMENTS, PPC.COMMENT, PPC.IDGDA_PERSONA_USER, PP.IDGDA_PERSONA_USER ");
                    sbInsert.AppendFormat("ORDER BY PPC.CREATED_AT DESC ");
                    sbInsert.AppendFormat($" OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY ");
                    using (SqlCommand commandInsert = new SqlCommand(sbInsert.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                getCommentFollow getCommentFollow = new getCommentFollow();
                                getCommentFollow.id = reader["IDGDA_PERSONA_POSTS_COMMENTS"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_PERSONA_POSTS_COMMENTS"]) : 0;
                                getCommentFollow.name = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "";
                                getCommentFollow.url = reader["PICTURE"] != DBNull.Value ? reader["PICTURE"].ToString() : "";
                                getCommentFollow.hierarchy = reader["CARGO"] != DBNull.Value ? reader["CARGO"].ToString() : "";
                                getCommentFollow.comment = reader["COMMENT"] != DBNull.Value ? reader["COMMENT"].ToString() : "";
                                getCommentFollow.timeAgo = reader["TIMEAGO"] != DBNull.Value ? reader["TIMEAGO"].ToString() : "";

                                int idOwnerComment = reader["ID_USER_COMMENT"] != DBNull.Value ? Convert.ToInt32(reader["ID_USER_COMMENT"]) : 0;
                                int idOwnerPost = reader["ID_USER_POST"] != DBNull.Value ? Convert.ToInt32(reader["ID_USER_POST"]) : 0;

                                if (isAdm == true)
                                {
                                    getCommentFollow.canDeleteComment = true;
                                }
                                else
                                {
                                    getCommentFollow.canDeleteComment = idOwnerComment == personaId || idOwnerPost == personaId ? true : false;
                                }

                                listComments.Add(getCommentFollow);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                connection.Close();
            }

            return listComments;
        }

        public static int insertComment(int IDGDA_PERSONA_USER, int IDGDA_COLLABORATOR, string COMMENT, int IDGDA_PERSONA_POSTS)
        {
            int codInsert = 0;
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    StringBuilder sbInsert = new StringBuilder();
                    sbInsert.AppendFormat("INSERT INTO GDA_PERSONA_POSTS_COMMENTS (IDGDA_PERSONA_USER, IDGDA_PERSONA_POSTS, IDGDA_COLLABORATOR, COMMENT, CREATED_AT) ");
                    sbInsert.AppendFormat("VALUES ( ");
                    sbInsert.AppendFormat("'{0}', ", IDGDA_PERSONA_USER); //IDGDA_PERSONA_USER
                    sbInsert.AppendFormat("{0}, ", IDGDA_PERSONA_POSTS); //IDGDA_PERSONA_POSTS
                    sbInsert.AppendFormat("'{0}', ", IDGDA_COLLABORATOR); //IDGDA_COLLABORATOR
                    sbInsert.AppendFormat("N'{0}', ", COMMENT); //COMMENT
                    sbInsert.AppendFormat("GETDATE() "); //CREATED_AT
                    sbInsert.AppendFormat(") SELECT @@IDENTITY AS 'CODINSERT' ");

                    using (SqlCommand commandInsert = new SqlCommand(sbInsert.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                codInsert = Convert.ToInt32(reader["CODINSERT"].ToString());
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                connection.Close();
            }
            return codInsert;
        }


        public static bool deletetComment(int IDGDA_PERSONA_POSTS_COMMENTS, int personauserId, bool isAdm)
        {
            bool canDelete = false;

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {

                connection.Open();
                try
                {
               

                    if (isAdm == true)
                    {
                        canDelete = true;
                    }
                    else
                    {
                        int idComment = 0;
                        int idPost = 0;
                        StringBuilder sbInsert = new StringBuilder();
                        //sbInsert.AppendFormat("UPDATE GDA_PERSONA_POSTS_COMMENTS SET  ");
                        //sbInsert.AppendFormat($"DELETED_AT = GETDATE(), DELETED_BY = {personauserId} ");
                        //sbInsert.AppendFormat($"WHERE IDGDA_PERSONA_POSTS_COMMENTS = {IDGDA_PERSONA_POSTS_COMMENTS} AND IDGDA_PERSONA_USER = {personauserId} ");

                        sbInsert.Append("SELECT A.IDGDA_PERSONA_USER AS IDCOMENT, P.IDGDA_PERSONA_USER AS IDPOST  ");
                        sbInsert.Append("FROM GDA_PERSONA_POSTS_COMMENTS (NOLOCK) A  ");
                        sbInsert.Append("INNER JOIN GDA_PERSONA_POSTS (NOLOCK) P ON A.IDGDA_PERSONA_POSTS = P.IDGDA_PERSONA_POSTS  ");
                        sbInsert.Append($"WHERE A.IDGDA_PERSONA_POSTS_COMMENTS = {IDGDA_PERSONA_POSTS_COMMENTS}  ");

                        using (SqlCommand commandInsert = new SqlCommand(sbInsert.ToString(), connection))
                        {
                            using (SqlDataReader reader = commandInsert.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    idComment = reader["IDCOMENT"] != DBNull.Value ? Convert.ToInt32(reader["IDCOMENT"]) : 0;
                                    idPost = reader["IDPOST"] != DBNull.Value ? Convert.ToInt32(reader["IDPOST"]) : 0;
                                }
                            }
                        }

                        if (idComment == personauserId || idPost == personauserId)
                        {
                            canDelete = true;
                        }
                    }

                    

                    if (canDelete == true)
                    {
                        StringBuilder sbInsert = new StringBuilder();
                        sbInsert.Append("UPDATE GDA_PERSONA_POSTS_COMMENTS SET ");
                        sbInsert.Append($"DELETED_AT = GETDATE(), DELETED_BY = {personauserId} ");
                        sbInsert.Append($"WHERE IDGDA_PERSONA_POSTS_COMMENTS = {IDGDA_PERSONA_POSTS_COMMENTS} ");

                        using (SqlCommand commandInsert = new SqlCommand(sbInsert.ToString(), connection))
                        {
                            int rowsAffected = commandInsert.ExecuteNonQuery();

                        }
                    }

                   
                }
                catch (Exception)
                {
                }
                connection.Close();

                return canDelete;
            }

        }

        #endregion
    }

}