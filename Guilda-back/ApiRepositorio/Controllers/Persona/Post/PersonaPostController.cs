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
using System.Linq;
using Microsoft.Extensions.Hosting;
using static TokenService;

namespace ApiRepositorio.Controllers
{

    public class inputPost
    {
        public string Post { get; set; }
        public int codPostReference { get; set; }
        public string expiredAt { get; set; }
        public int highlight { get; set; }
        public int allowComment { get; set; }
        public Visibility visibility { get; set; }

    }

    public class Visibility
    {
        public List<int> sector { get; set; }
        public List<int> subSector { get; set; }
        public List<int> period { get; set; }
        public List<int> hierarchy { get; set; }
        public List<int> group { get; set; }
        public List<int> userId { get; set; }
        public List<int> client { get; set; }
        public List<int> homeOrFloor { get; set; }
    }



    public class returnQueryPosts
    {
        public int cod { get; set; }
        public int isRepost { get; set; }
        public string imageUser { get; set; }
        public string nameUser { get; set; }
        public string hierarchyUser { get; set; }
        public string post { get; set; }
        public int idPostReference { get; set; }
        public string postReference { get; set; }
        public string imageUserReference { get; set; }
        public string nameReference { get; set; }
        public string hierarchyuserReference { get; set; }
        public string datePost { get; set; }
        public string timeAgo { get; set; }
        public string linkFile { get; set; }
        public string linkFileReference { get; set; }
        public int comments { get; set; }
        public DateTime? expiredAt { get; set; }
        public int highlight { get; set; }
        public int sumReactions { get; set; }
        public reactionsPosts myReaction { get; set; }
        public List<reactionsPosts> reactions { get; set; }
        public List<filesPosts> files { get; set; }
        public List<filesPosts> filesReference { get; set; }
    }

    public class reactionsPosts
    {
        public int id { get; set; }
        public string name { get; set; }
        public string linkIcon { get; set; }
        public string linkIconSelected { get; set; }
        public int amount { get; set; }
    }

    public class filesPosts
    {
        public string url { get; set; }
    }

    public class userConfigPost
    {
        public List<int> sector { get; set; }
        public List<int> subsector { get; set; }
        public List<int> client { get; set; }
        public int period { get; set; }
        public int hierarchy { get; set; }
        public int group { get; set; }
        public int homeOrFloor { get; set; }
    }


    //[Authorize]
    public class PersonaPostController : ApiController
    {
        //Realiza um Post // Realizar Repost

        //[ResponseType(typeof(ResultsModel))]
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
            personauserId = inf.personauserId;

            string jsonFromFormData = System.Web.HttpContext.Current.Request.Form["json"];
            inputPost post = JsonConvert.DeserializeObject<inputPost>(jsonFromFormData);




            int codPostagem = 0;
            int isRepost = post.codPostReference > 0 ? 1 : 0;
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;

            int lengthFiles = 0;
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFile file = files[i];

                lengthFiles += file.ContentLength;
            }



            long maxFileSize = 500 * 1024 * 1024; // 500MB por exemplo

            // Verifica se o tamanho do ficheiro excede o limite
            if (lengthFiles > maxFileSize)
            {
                return BadRequest("O vídeo é muito grande. Por favor, faça upload de um vídeo menor que 500MB.");
            }


            if (isRepost == 1)
            {
                //Regra adicionada para sempre repostarmos do post original, evitando repostagem de outra postagem
                bool existReferente = true;
                while (existReferente == true)
                {
                    int codPostOriginal = Banco.returnCodOrig(post.codPostReference);
                    if (codPostOriginal != 0)
                    {
                        post.codPostReference = codPostOriginal;
                    }
                    else
                    {
                        existReferente = false;
                    }
                }

                if (Funcoes.VerificaPostDeletado(post.codPostReference) == false)
                {
                    return BadRequest("Este post foi deletado.");
                }
            }
            else
            {
                bool isEmpty = string.IsNullOrWhiteSpace(post.Post.ToString().Trim());
                if (isEmpty && files.Count == 0)
                {
                    return BadRequest("A postagem deve conter ao menos um texto ou anexo!");
                }
            }

            post.Post = Funcoes.FiltroBlackList(post.Post) == false ? post.Post : "Post Error: Palavra Na Blacklist";

            if (post.Post.Contains("Palavra Na Blacklist"))
            {
                return BadRequest("Post com palavras inapropriadas.");
            }

            //Inserir a postagem retornando o codigo
            codPostagem = Banco.insertPost(personauserId, collaboratorId, post.Post, post.codPostReference, isRepost, post.expiredAt, post.highlight, post.allowComment);

            //Inserir o publico alvo, visibilidade da postagem
            Banco.insertVisibility(codPostagem, post);


            List<GalleryResponseModel> pictures = PictureClass.UploadFilesToBlob(files);

            foreach (GalleryResponseModel item in pictures)
            {
                //Insiro na tabela de GDA_PERSONA_POSTS_FILES
                Banco.insertFiles(item.url, codPostagem);
            }

            return Ok("Postado com sucesso!");
        }

        [HttpDelete]
        public IHttpActionResult DeleteResultsModel(int idPost, bool isAdm)
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

            bool ok = Banco.deletetPost(idPost, personauserId, isAdm);

            if (ok == true || isAdm == true)
            {
                return Ok("Post deletado!");
            }
            else
            {
                return BadRequest("Não foi possivel deletar o post!");
            }

        }

        //Visualiza os Posts (Tahto ou Geral

    }

    public class Banco
    {
        #region Banco
        public static int returnCodOrig(int codPost)
        {
            int retorno = 0;
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();

                StringBuilder sbGetCod = new StringBuilder();
                sbGetCod.AppendFormat("SELECT IDGDA_PERSONA_POSTS_REFERENCE ");
                sbGetCod.AppendFormat("FROM GDA_PERSONA_POSTS (NOLOCK) ");
                sbGetCod.AppendFormat($"WHERE IDGDA_PERSONA_POSTS = {codPost} ");
                using (SqlCommand commandInsert = new SqlCommand(sbGetCod.ToString(), connection))
                {
                    using (SqlDataReader reader = commandInsert.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            retorno = Convert.ToInt32(reader["IDGDA_PERSONA_POSTS_REFERENCE"].ToString());
                        }
                    }
                }

                connection.Close();
            }


            return retorno;
        }

        public static int insertPost(int IDGDA_PERSONA_USER, int IDGDA_COLLABORATOR, string post, int codPost, int isRepost, string expiredAt, int highlight, int allowComment)
        {
            int codInsert = 0;
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {


                connection.Open();
                try
                {

                    StringBuilder sbInsert = new StringBuilder();
                    sbInsert.AppendFormat("INSERT INTO GDA_PERSONA_POSTS (IDGDA_PERSONA_USER, IDGDA_COLLABORATOR, POST, CREATED_AT, ISREPOST, IDGDA_PERSONA_POSTS_REFERENCE, EXPIRED_AT, HIGHLIGHT, ALLOW_COMMENT) ");
                    sbInsert.AppendFormat(" VALUES ( ");
                    sbInsert.AppendFormat(" '{0}', ", IDGDA_PERSONA_USER); //IDGDA_PERSONA_USER
                    sbInsert.AppendFormat(" '{0}', ", IDGDA_COLLABORATOR); // IDGDA_COLLABORATOR
                    sbInsert.AppendFormat(" '{0}', ", post); //POST
                    sbInsert.AppendFormat(" GETDATE(), "); //CREATED_AT
                    sbInsert.AppendFormat(" {0}, ", isRepost); //ISREPOST
                    sbInsert.AppendFormat(" {0}, ", codPost); //IDGDA_PERSONA_POSTS_REFERENCE
                    sbInsert.AppendFormat(" '{0}', ", expiredAt); //EXPIRED_AT
                    sbInsert.AppendFormat(" {0}, ", highlight); //HIGHLIGHT
                    sbInsert.AppendFormat(" {0} ", allowComment); //ALLOW_COMMENT
                    sbInsert.AppendFormat(" ) SELECT @@IDENTITY AS 'CODINSERT' ");


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


        public static void inserVisibilityItem(int codPost, int visibilityTipe, int idReferer)
        {
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {

                    StringBuilder sbInsert = new StringBuilder();
                    sbInsert.AppendFormat("INSERT INTO GDA_PERSONA_POSTS_VISIBILITY (IDGDA_PERSONA_POSTS, IDGDA_PERSONA_POSTS_VISIBILITY_TYPE, ID_REFERER) ");
                    sbInsert.AppendFormat("VALUES ( ");
                    sbInsert.AppendFormat("{0}, ", codPost); //IDGDA_PRESONA_POSTS
                    sbInsert.AppendFormat("{0}, ", visibilityTipe); //IDGDA_PERSONA_POSTS_VISIBILITY_TYPE
                    sbInsert.AppendFormat("{0} ", idReferer); //ID_REFERER
                    sbInsert.AppendFormat(") ");

                    using (SqlCommand commandInsert = new SqlCommand(sbInsert.ToString(), connection))
                    {
                        commandInsert.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {
                }
                connection.Close();
            }
        }

        public static void insertVisibility(int codPost, inputPost inputPost)
        {
            //Sector
            if (inputPost.visibility.sector.Count > 0)
            {
                foreach (int item in inputPost.visibility.sector)
                {
                    inserVisibilityItem(codPost, 1, item);
                }
            }
            //SubSector
            if (inputPost.visibility.subSector.Count > 0)
            {
                foreach (int item in inputPost.visibility.subSector)
                {
                    inserVisibilityItem(codPost, 2, item);
                }
            }
            //Period
            if (inputPost.visibility.period.Count > 0)
            {
                foreach (int item in inputPost.visibility.period)
                {
                    inserVisibilityItem(codPost, 3, item);
                }
            }
            //Hierarchy
            if (inputPost.visibility.hierarchy.Count > 0)
            {
                foreach (int item in inputPost.visibility.hierarchy)
                {
                    inserVisibilityItem(codPost, 4, item);
                }
            }
            //Group
            if (inputPost.visibility.group.Count > 0)
            {
                foreach (int item in inputPost.visibility.group)
                {
                    inserVisibilityItem(codPost, 5, item);
                }
            }
            //UserId
            if (inputPost.visibility.userId.Count > 0)
            {
                foreach (int item in inputPost.visibility.userId)
                {
                    inserVisibilityItem(codPost, 6, item);
                }
            }
            //Client
            if (inputPost.visibility.client.Count > 0)
            {
                foreach (int item in inputPost.visibility.client)
                {
                    inserVisibilityItem(codPost, 7, item);
                }
            }
            //HomeOrFloor
            if (inputPost.visibility.homeOrFloor.Count > 0)
            {
                foreach (int item in inputPost.visibility.homeOrFloor)
                {
                    inserVisibilityItem(codPost, 8, item);
                }
            }

        }

        public static bool deletetPost(int IDGDA_PERSONA_POSTS, int personauserId, bool isAdm)
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
                        int idPost = 0;
                        StringBuilder sbInsert = new StringBuilder();

                        sbInsert.Append("SELECT IDGDA_PERSONA_USER AS IDPOST  ");
                        sbInsert.Append("FROM GDA_PERSONA_POSTS (NOLOCK)  ");
                        sbInsert.Append($"WHERE IDGDA_PERSONA_POSTS = {IDGDA_PERSONA_POSTS}  ");

                        using (SqlCommand commandInsert = new SqlCommand(sbInsert.ToString(), connection))
                        {
                            using (SqlDataReader reader = commandInsert.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    idPost = reader["IDPOST"] != DBNull.Value ? Convert.ToInt32(reader["IDPOST"]) : 0;
                                }
                            }
                        }

                        if (idPost == personauserId)
                        {
                            canDelete = true;
                        }
                    }

                    if (canDelete == true)
                    {
                        StringBuilder sbInsert = new StringBuilder();
                        sbInsert.Append("UPDATE GDA_PERSONA_POSTS SET ");
                        sbInsert.Append($"DELETED_AT = GETDATE(), DELETED_BY = {personauserId} ");
                        sbInsert.Append($"WHERE IDGDA_PERSONA_POSTS = {IDGDA_PERSONA_POSTS} ");

                        using (SqlCommand commandInsert = new SqlCommand(sbInsert.ToString(), connection))
                        {
                            int rowsAffected = commandInsert.ExecuteNonQuery();
                        }

                        StringBuilder sbUpdate2 = new StringBuilder();
                        sbUpdate2.Append("UPDATE GDA_PERSONA_POSTS SET ");
                        sbUpdate2.Append($"DELETED_AT = GETDATE(), DELETED_BY = {personauserId} ");
                        sbUpdate2.Append($"WHERE DELETED_AT IS NULL AND IDGDA_PERSONA_POSTS_REFERENCE = {IDGDA_PERSONA_POSTS} ");

                        using (SqlCommand commandInsert = new SqlCommand(sbUpdate2.ToString(), connection))
                        {
                            int rowsAffected2 = commandInsert.ExecuteNonQuery();
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


        public static void insertFiles(string url, int codPost)
        {

            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("INSERT INTO GDA_PERSONA_POSTS_FILES (IDGDA_PERSONA_POSTS, LINK_FILE, CREATED_AT) ");
            stb.AppendFormat("VALUES ( ");
            stb.AppendFormat("'{0}', ", codPost);
            stb.AppendFormat("'{0}', ", url);
            stb.AppendFormat("GETDATE() ");
            stb.AppendFormat(") ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {

                }
                connection.Close();
            }
        }
        #endregion
    }
}