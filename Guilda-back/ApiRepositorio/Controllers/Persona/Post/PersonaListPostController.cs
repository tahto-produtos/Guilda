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
using static TokenService;

namespace ApiRepositorio.Controllers
{



    public class ReactionListTable
    {
        public int id { get; set; }
        public string name { get; set; }
        public string linkIcon { get; set; }
        public string linkIconSelected { get; set; }
    }

    public class inputListPostGet
    {
        public bool feed { get; set; }
        public bool feedTahto { get; set; }
        public bool isAdm { get; set; }
        public userListConfigPost userConfigPost { get; set; }
        public int specificUserId { get; set; }

        public int limit { get; set; }

        public int page { get; set; }

    }

    public class returnListsPosts
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
        public int allowComment { get; set; }
        public DateTime? expiredAt { get; set; }
        public int highlight { get; set; }
        public int sumReactions { get; set; }
        public reactionsListPosts myReaction { get; set; }
        public List<reactionsListPosts> reactions { get; set; }
        public List<filesListPosts> files { get; set; }
        public List<filesListPosts> filesReference { get; set; }
        public int EditPost { get; set; }
        public string visibility { get; set; }
        public bool canDeletePost { get; set; }
    }

    public class returnListQueryPosts
    {
        public int TOTALPAGES { get; set; }
        public List<returnListsPosts> returnListsPosts { get; set; }
    }

    public class reactionsListPosts
    {
        public int id { get; set; }
        public string name { get; set; }
        public string linkIcon { get; set; }
        public string linkIconSelected { get; set; }
        public int amount { get; set; }
    }

    public class filesListPosts
    {
        public string url { get; set; }
    }

    public class userListConfigPost
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
    public class PersonaListPostController : ApiController
    {

        //Visualiza os Posts (Tahto ou Geral

        [HttpPost]
        [ResponseType(typeof(returnListQueryPosts))]
        public IHttpActionResult GetResultsModel([FromBody] inputListPostGet inputModel)
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

            returnListQueryPosts returnListQueryPosts = new returnListQueryPosts();

            int totalInfo = BancoListPost.quantidadePosts(inputModel.userConfigPost, inputModel.feedTahto, collaboratorId, personauserId, inputModel.specificUserId);
            int totalpage = totalInfo / inputModel.limit;
            int offset = (inputModel.page - 1) * inputModel.limit;
            returnListQueryPosts.TOTALPAGES = totalpage;

            returnListQueryPosts.returnListsPosts = new List<returnListsPosts>();
            returnListQueryPosts.returnListsPosts = BancoListPost.returnPosts(inputModel.userConfigPost, inputModel.feedTahto, inputModel.isAdm, collaboratorId, personauserId, inputModel.specificUserId, offset, inputModel.limit);

            //ordenação
            return Ok(returnListQueryPosts);
        }

    }

    public class BancoListPost
    {
        #region Banco

        public static string montaFiltroVisibilidade(userListConfigPost user, int userId)
        {
            string retn = "";
            try
            {
                string sectors = user.sector == null ? "0" : string.Join(",", user.sector);
                string subsectors = user.subsector == null ? "0" : string.Join(",", user.subsector);
                string clients = user.client == null ? "0" : string.Join(",", user.client);

                sectors = sectors == "" ? "0" : sectors;
                subsectors = subsectors == "" ? "0" : subsectors;
                clients = clients == "" ? "0" : clients;

                StringBuilder stb = new StringBuilder();
                stb.Append("AND (SELECT COUNT(DISTINCT IDGDA_PERSONA_POSTS_Visibility_TYPE) FROM GDA_PERSONA_POSTS_Visibility (NOLOCK) ");
                stb.Append("WHERE IDGDA_PERSONA_POSTS = PP.IDGDA_PERSONA_POSTS ");
                stb.Append("AND ( ");
                stb.AppendFormat("    (IDGDA_PERSONA_POSTS_Visibility_TYPE = 1 AND ID_REFERER IN ({0})) ", sectors); // SETORES
                stb.AppendFormat("    OR (IDGDA_PERSONA_POSTS_Visibility_TYPE = 2 AND ID_REFERER IN ({0})) ", subsectors); // SUBSETORES
                stb.AppendFormat("    OR (IDGDA_PERSONA_POSTS_Visibility_TYPE = 3 AND ID_REFERER IN ({0})) ", user.period); //PERIOD
                stb.AppendFormat("    OR (IDGDA_PERSONA_POSTS_Visibility_TYPE = 4 AND ID_REFERER IN ({0})) ", user.hierarchy); //HIERARCHY
                stb.AppendFormat("    OR (IDGDA_PERSONA_POSTS_Visibility_TYPE = 5 AND ID_REFERER IN ({0})) ", user.group); //GROUP
                stb.AppendFormat("    OR (IDGDA_PERSONA_POSTS_Visibility_TYPE = 6 AND ID_REFERER IN ({0})) ", userId); //USERID
                stb.AppendFormat("    OR (IDGDA_PERSONA_POSTS_Visibility_TYPE = 7 AND ID_REFERER IN ({0})) ", clients); //CLIENT
                stb.AppendFormat("    OR (IDGDA_PERSONA_POSTS_Visibility_TYPE = 8 AND ID_REFERER IN ({0})) ", user.homeOrFloor); //HOMEORFLOOR
                stb.Append(")) >= ");
                stb.Append("(SELECT COUNT(DISTINCT IDGDA_PERSONA_POSTS_Visibility_TYPE) FROM GDA_PERSONA_POSTS_Visibility WITH (NOLOCK) ");
                stb.Append("WHERE IDGDA_PERSONA_POSTS = PP.IDGDA_PERSONA_POSTS) ");

                retn = stb.ToString();

            }
            catch (Exception)
            {

            }

            return retn;
        }

        public static int quantidadePosts(userListConfigPost user, bool feedTahto, int collaboratorId, int userId, int specificUserId)
        {
            int total = 0;
            string filtroFeedTahto = "";
            string filtroVisibilidade = "";
            string filtroSomenteSeguidos = "";

            string filtroUsuarioEspecifico = "";


            string orderBy = "";
            //Casos em que é enviado um usuario, ou o meu proprio usuario
            if (specificUserId != 0)
            {
                filtroUsuarioEspecifico = " AND PP.IDGDA_PERSONA_USER = " + specificUserId + " ";

                orderBy = " ORDER BY CREATED_AT DESC ";
            }

            if (feedTahto == true)
            {
                filtroFeedTahto = " AND PU.TAHTO = 1 ";
            }
            else
            {
                if (specificUserId != userId)
                {
                    filtroVisibilidade = montaFiltroVisibilidade(user, userId);

                    StringBuilder stbF = new StringBuilder();
                    stbF.Append(" AND (PP.IDGDA_PERSONA_USER IN ( ");
                    stbF.Append(" SELECT IDGDA_PERSONA_USER_FOLLOWED FROM GDA_PERSONA_FOLLOWERS (NOLOCK) WHERE IDGDA_PERSONA_USER = @IDGDA_USER AND DELETED_AT IS NULL ");
                    stbF.Append(" ) OR PU.IDGDA_PERSONA_USER_VISIBILITY = 1 ) ");

                    filtroSomenteSeguidos = stbF.ToString();

                    orderBy = " ORDER BY HIGHLIGHT DESC, CREATED_AT DESC ";
                }
            }

            List<returnListsPosts> listPosts = new List<returnListsPosts>();

            List<ReactionListTable> ReactionListTable = returnReationTable();



            //Listo todas as postagens e repostagens do banco
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    //787450
                    //13180
                    StringBuilder stb = new StringBuilder();
                    stb.AppendFormat("DECLARE @IDGDA_COLLABORATORS INT; SET @IDGDA_COLLABORATORS = {0}; ", collaboratorId);
                    stb.AppendFormat("DECLARE @IDGDA_USER INT; SET @IDGDA_USER = {0}; ", userId);
                    stb.Append("DECLARE @DATAATUAL DATE; SET @DATAATUAL = CONVERT(DATE, DATEADD(DAY, -1, GETDATE())) ");
                    stb.AppendFormat("SELECT COUNT(0) ");
                    stb.AppendFormat("FROM GDA_PERSONA_POSTS (NOLOCK) AS PP ");
                    stb.AppendFormat("LEFT JOIN GDA_PERSONA_USER (NOLOCK) AS PU ON PU.IDGDA_PERSONA_USER = PP.IDGDA_PERSONA_USER  ");
                    stb.Append("WHERE 1 = 1 ");
                    stb.AppendFormat(" {0} ", filtroVisibilidade);
                    stb.AppendFormat(" {0} ", filtroSomenteSeguidos);
                    stb.AppendFormat(" {0} ", filtroFeedTahto);
                    stb.AppendFormat(" {0} ", filtroUsuarioEspecifico);



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






            //Convert no json de resposta para o front
            // Convertendo a lista de returnListQueryPosts para returnPosts


            //Realizar conversão.

            return total;
        }



        public static List<returnListsPosts> returnPosts(userListConfigPost user, bool feedTahto, bool isAdm, int collaboratorId, int userId, int specificUserId, int offset, int limit)
        {
            int total = 0;
            string filtroFeedTahto = "";
            string filtroVisibilidade = "";
            string filtroSomenteSeguidos = "";

            string filtroUsuarioEspecifico = "";


            string orderBy = "";
            //Casos em que é enviado um usuario, ou o meu proprio usuario
            if (specificUserId != 0)
            {
                filtroUsuarioEspecifico = " AND PP.IDGDA_PERSONA_USER = " + specificUserId + " ";

                orderBy = " ORDER BY CREATED_AT DESC ";
            }

            if (feedTahto == true)
            {
                filtroFeedTahto = " AND PU.TAHTO = 1 ";

                filtroVisibilidade = montaFiltroVisibilidade(user, userId);
            }
            else
            {
                if (specificUserId != userId)
                {
                    filtroVisibilidade = montaFiltroVisibilidade(user, userId);

                    StringBuilder stbF = new StringBuilder();
                    stbF.Append(" AND (PP.IDGDA_PERSONA_USER IN ( ");
                    stbF.Append(" SELECT IDGDA_PERSONA_USER_FOLLOWED FROM GDA_PERSONA_FOLLOWERS (NOLOCK) WHERE IDGDA_PERSONA_USER = @IDGDA_USER AND DELETED_AT IS NULL ");
                    stbF.Append(" ) OR PU.IDGDA_PERSONA_USER_VISIBILITY = 1 ) ");

                    filtroSomenteSeguidos = stbF.ToString();

                    orderBy = " ORDER BY HIGHLIGHT DESC, CREATED_AT DESC ";

                }
            }


            List<returnListsPosts> listPosts = new List<returnListsPosts>();

            List<ReactionListTable> ReactionListTable = returnReationTable();

            //Listo todas as postagens e repostagens do banco
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    //787450
                    //13180
                    StringBuilder stb = new StringBuilder();
                    stb.AppendFormat("DECLARE @IDGDA_COLLABORATORS INT; SET @IDGDA_COLLABORATORS = {0}; ", collaboratorId);
                    stb.AppendFormat("DECLARE @IDGDA_USER INT; SET @IDGDA_USER = {0}; ", userId);
                    stb.Append("DECLARE @DATAATUAL DATE; SET @DATAATUAL = CONVERT(DATE, DATEADD(DAY, -1, GETDATE())) ");
                    stb.Append("SELECT PP.IDGDA_PERSONA_POSTS,  ");
                    stb.Append("PP.IDGDA_PERSONA_USER, ");
                    stb.Append("PP.ISREPOST, ");
                    stb.Append("PU.PICTURE AS IMAGEUSER,  ");
                    stb.Append("PU.NAME AS NAMEUSER, ");
                    stb.Append("CASE WHEN PU.IDGDA_PERSONA_USER_TYPE = 1 THEN CD.CARGO ");
                    stb.Append("ELSE 'COMERCIAL' END AS HIERARCHYUSER, ");
                    stb.Append("PP.POST, ");
                    stb.Append("PP.IDGDA_PERSONA_POSTS_REFERENCE AS 'ID_POST_REFERENCE', ");
                    stb.Append("PPR.POST AS 'POST_REFERENCE', ");
                    stb.Append("PUR.PICTURE AS 'IMAGEUSER_REFERENCE', ");
                    stb.Append("PUR.NAME AS 'NAME_REFERENCE', ");
                    stb.Append("CASE WHEN PUR.IDGDA_PERSONA_USER_TYPE = 1 THEN CD.CARGO ");
                    stb.Append("ELSE 'COMERCIAL' END AS HIERARCHYUSER_REFERENCE, ");
                    stb.Append("PP.CREATED_AT AS DATEPOST, ");
                    stb.Append("CASE ");
                    stb.Append("       WHEN DATEDIFF(MINUTE, PP.CREATED_AT, GETDATE()) < 60 THEN CONCAT(DATEDIFF(MINUTE, PP.CREATED_AT, GETDATE()), ' MINUTOS') ");
                    stb.Append("        WHEN DATEDIFF(HOUR, PP.CREATED_AT, GETDATE()) < 24 THEN CONCAT(DATEDIFF(HOUR, PP.CREATED_AT, GETDATE()), ' HORAS') ");
                    stb.Append("        ELSE CONCAT(DATEDIFF(DAY, PP.CREATED_AT, GETDATE()), ' DIAS') ");
                    stb.Append("    END AS TIMEAGO, ");
                    stb.Append("STRING_AGG(PPF.LINK_FILE, ';') AS LINK_FILE, ");
                    stb.Append("(SELECT COUNT(*) FROM GDA_PERSONA_POSTS_LIKES (NOLOCK) PPC_SUB WHERE PPC_SUB.IDGDA_PERSONA_POSTS_LIKES_REACTION = 1  ");
                    stb.Append("  AND PPC_SUB.IDGDA_PERSONA_POSTS = PP.IDGDA_PERSONA_POSTS AND PPC_SUB.DELETED_AT IS NULL) AS LIKES, ");
                    stb.Append("(SELECT COUNT(*) FROM GDA_PERSONA_POSTS_LIKES (NOLOCK) PPC_SUB WHERE PPC_SUB.IDGDA_PERSONA_POSTS_LIKES_REACTION = 2  ");
                    stb.Append("  AND PPC_SUB.IDGDA_PERSONA_POSTS = PP.IDGDA_PERSONA_POSTS AND PPC_SUB.DELETED_AT IS NULL) AS LOVE, ");
                    stb.Append("(SELECT COUNT(*) FROM GDA_PERSONA_POSTS_COMMENTS AS PPC_SUB WHERE PPC_SUB.IDGDA_PERSONA_POSTS = PP.IDGDA_PERSONA_POSTS) AS COMMENTS, ");
                    stb.Append("MAX(MYPPL.IDGDA_PERSONA_POSTS_LIKES_REACTION) AS MYREACTION, ");
                    stb.Append("STRING_AGG(PPFR.LINK_FILE, ';') AS LINK_FILE_REFERENCE, ");
                    stb.Append("MAX(PP.EXPIRED_AT) AS EXPIRED_AT, ");
                    stb.Append("MAX(PP.HIGHLIGHT) AS HIGHLIGHT, ");
                    stb.Append("MAX(PP.ALLOW_COMMENT) AS ALLOW_COMMENT, ");
                    stb.Append("STRING_AGG(TBLN.NAME, ' - ') AS VISIBILIDADE ");
                    stb.Append("FROM  GDA_PERSONA_POSTS (NOLOCK) AS PP ");
                    stb.Append("LEFT JOIN GDA_PERSONA_POSTS (NOLOCK) PPR ON PPR.IDGDA_PERSONA_POSTS = PP.IDGDA_PERSONA_POSTS_REFERENCE ");
                    stb.Append("LEFT JOIN GDA_PERSONA_POSTS_LIKES (NOLOCK) AS PPL ON PPL.IDGDA_PERSONA_POSTS = PP.IDGDA_PERSONA_POSTS AND PPL.DELETED_AT IS NULL ");
                    stb.Append("LEFT JOIN GDA_PERSONA_POSTS_LIKES (NOLOCK) AS MYPPL ON MYPPL.IDGDA_PERSONA_POSTS = PP.IDGDA_PERSONA_POSTS AND MYPPL.IDGDA_PERSONA_USER = @IDGDA_USER AND MYPPL.DELETED_AT IS NULL ");
                    stb.Append("LEFT JOIN GDA_PERSONA_POSTS_FILES (NOLOCK) AS PPF ON PPF.IDGDA_PERSONA_POSTS = PP.IDGDA_PERSONA_POSTS ");
                    stb.Append("LEFT JOIN GDA_PERSONA_POSTS_FILES (NOLOCK) AS PPFR ON PPFR.IDGDA_PERSONA_POSTS = PP.IDGDA_PERSONA_POSTS_REFERENCE ");
                    stb.Append("LEFT JOIN GDA_PERSONA_POSTS_COMMENTS (NOLOCK) AS PPC ON PPC.IDGDA_PERSONA_POSTS = PP.IDGDA_PERSONA_POSTS ");
                    stb.Append("LEFT JOIN GDA_PERSONA_USER (NOLOCK) AS PU ON PU.IDGDA_PERSONA_USER = PP.IDGDA_PERSONA_USER ");
                    stb.Append("LEFT JOIN GDA_PERSONA_USER (NOLOCK) AS PUR ON PUR.IDGDA_PERSONA_USER = PPR.IDGDA_PERSONA_USER ");
                    stb.Append("LEFT JOIN (SELECT CARGO, IDGDA_COLLABORATORS FROM GDA_COLLABORATORS_DETAILS (NOLOCK) WHERE CREATED_AT >= @DATAATUAL GROUP BY IDGDA_COLLABORATORS, CARGO) AS CD ON CD.IDGDA_COLLABORATORS = PP.IDGDA_COLLABORATOR ");
                    //stb.Append("LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CDR ON CDR.IDGDA_COLLABORATORS = PPR.IDGDA_COLLABORATOR AND CD.CREATED_AT >= @DATAATUAL ");

                    stb.Append("LEFT JOIN ( ");
                    stb.Append("SELECT IDGDA_PERSONA_POSTS, CASE WHEN PV.IDGDA_PERSONA_POSTS_VISIBILITY_TYPE = 1 THEN 'SETOR: ' + SEC.NAME ");
                    stb.Append("            WHEN PV.IDGDA_PERSONA_POSTS_VISIBILITY_TYPE = 2 THEN 'SUBSETOR: ' + SEC.NAME ");
                    stb.Append("			WHEN PV.IDGDA_PERSONA_POSTS_VISIBILITY_TYPE = 3 THEN 'PERIODO: ' + PD.PERIOD ");
                    stb.Append("			WHEN PV.IDGDA_PERSONA_POSTS_VISIBILITY_TYPE = 4 THEN 'HIERARQUIA: ' + HC.LEVELNAME ");
                    stb.Append("			WHEN PV.IDGDA_PERSONA_POSTS_VISIBILITY_TYPE = 5 THEN 'GRUPO: ' + GR.ALIAS ");
                    stb.Append("			WHEN PV.IDGDA_PERSONA_POSTS_VISIBILITY_TYPE = 6 THEN 'NOME: ' + CB.NAME ");
                    stb.Append("			WHEN PV.IDGDA_PERSONA_POSTS_VISIBILITY_TYPE = 7 THEN 'CLIENTE: ' + CL.CLIENT ");
                    stb.Append("			WHEN PV.IDGDA_PERSONA_POSTS_VISIBILITY_TYPE = 8 THEN  ");
                    stb.Append("				CASE WHEN HF.IDGDA_HOMEFLOOR = 1 THEN '' + 'HOME' ");
                    stb.Append("					WHEN HF.IDGDA_HOMEFLOOR = 2 THEN '' + 'FLOOR' ");
                    stb.Append("					ELSE '' + HF.HOMEFLOOR ");
                    stb.Append("				END  ");
                    stb.Append("				  ");
                    stb.Append("		END NAME ");
                    stb.Append("     FROM GDA_PERSONA_POSTS_VISIBILITY (NOLOCK) AS PV ");
                    stb.Append("	 INNER JOIN GDA_PERSONA_POSTS_VISIBILITY_TYPE (NOLOCK) AS PVT ON PV.IDGDA_PERSONA_POSTS_VISIBILITY_TYPE = PVT.IDGDA_PERSONA_POSTS_VISIBILITY_TYPE ");
                    stb.Append("	 LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = PV.ID_REFERER AND (PV.IDGDA_PERSONA_POSTS_VISIBILITY_TYPE = 1  ");
                    stb.Append("																					OR PV.IDGDA_PERSONA_POSTS_VISIBILITY_TYPE = 2) ");
                    stb.Append("	 LEFT JOIN GDA_PERIOD (NOLOCK) AS PD ON PD.IDGDA_PERIOD = PV.ID_REFERER AND PV.IDGDA_PERSONA_POSTS_VISIBILITY_TYPE = 3 ");
                    stb.Append("	 LEFT JOIN GDA_HIERARCHY (NOLOCK) AS HC ON HC.IDGDA_HIERARCHY = PV.ID_REFERER AND PV.IDGDA_PERSONA_POSTS_VISIBILITY_TYPE = 4 ");
                    stb.Append("	 LEFT JOIN GDA_GROUPS (NOLOCK) AS GR ON GR.ID = PV.ID_REFERER AND PV.IDGDA_PERSONA_POSTS_VISIBILITY_TYPE = 5 ");
                    stb.Append("	 LEFT JOIN GDA_PERSONA_USER (NOLOCK) AS CB ON CB.IDGDA_PERSONA_USER = PV.ID_REFERER AND PV.IDGDA_PERSONA_POSTS_VISIBILITY_TYPE = 6 ");
                    stb.Append("	 LEFT JOIN GDA_CLIENT (NOLOCK) AS CL ON CL.IDGDA_CLIENT = PV.ID_REFERER AND PV.IDGDA_PERSONA_POSTS_VISIBILITY_TYPE = 7 ");
                    stb.Append("	 LEFT JOIN GDA_HOMEFLOOR (NOLOCK) AS HF ON HF.IDGDA_HOMEFLOOR = PV.ID_REFERER AND PV.IDGDA_PERSONA_POSTS_VISIBILITY_TYPE = 8 ");
                    stb.Append(") AS TBLN ON TBLN.IDGDA_PERSONA_POSTS = PP.IDGDA_PERSONA_POSTS ");


                    stb.Append("WHERE 1 = 1 AND PP.DELETED_AT IS NULL ");
                    stb.AppendFormat(" {0} ", filtroVisibilidade);
                    stb.AppendFormat(" {0} ", filtroSomenteSeguidos);
                    stb.AppendFormat(" {0} ", filtroFeedTahto);
                    stb.AppendFormat(" {0} ", filtroUsuarioEspecifico);
                    stb.Append("GROUP BY PP.IDGDA_PERSONA_POSTS,PU.NAME, PU.PICTURE, PP.POST, PP.CREATED_AT, PU.IDGDA_PERSONA_USER_TYPE, CD.CARGO, PP.IDGDA_PERSONA_USER,  ");
                    stb.Append("  PP.ISREPOST, PP.IDGDA_PERSONA_POSTS_REFERENCE, PPR.POST, PUR.PICTURE, PUR.NAME, PUR.IDGDA_PERSONA_USER_TYPE, PP.HIGHLIGHT, PP.EXPIRED_AT ");
                    stb.Append("ORDER BY  ");
                    stb.Append("    CASE WHEN PP.HIGHLIGHT = 1 AND (CONVERT(DATE, PP.EXPIRED_AT) >= CONVERT(DATE, GETDATE()) OR PP.EXPIRED_AT IS NULL) THEN 1 ELSE 0 END DESC,  ");




                    stb.Append("    CASE WHEN PP.CREATED_AT >= DATEADD(DAY, -1, GETDATE()) THEN -1 ELSE 0 END,  ");
                    stb.Append("    CASE WHEN PP.CREATED_AT >= DATEADD(DAY, -1, GETDATE()) THEN   ");
                    stb.Append("            (SELECT COUNT(*)  ");
                    stb.Append("            FROM GDA_PERSONA_POSTS_LIKES (NOLOCK) PPC_SUB  ");
                    stb.Append("            WHERE PPC_SUB.IDGDA_PERSONA_POSTS_LIKES_REACTION IN (1, 2)  ");
                    stb.Append("            AND PPC_SUB.IDGDA_PERSONA_POSTS = PP.IDGDA_PERSONA_POSTS AND PPC_SUB.DELETED_AT IS NULL)   ");
                    stb.Append("            +   ");
                    stb.Append("            (SELECT COUNT(*)  ");
                    stb.Append("            FROM GDA_PERSONA_POSTS_COMMENTS AS PPC_SUB  ");
                    stb.Append("            WHERE PPC_SUB.IDGDA_PERSONA_POSTS = PP.IDGDA_PERSONA_POSTS) ELSE NULL END DESC,  ");
                    stb.Append("    PP.CREATED_AT DESC  ");

                    stb.Append($" OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY ");

                    using (SqlCommand commandInsert = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            while (reader.Read())
                            {


                                returnListsPosts returnListsPosts = new returnListsPosts();
                                returnListsPosts.reactions = new List<reactionsListPosts>();
                                returnListsPosts.files = new List<filesListPosts>();
                                returnListsPosts.myReaction = new reactionsListPosts();

                                int codMyReaction = reader["MYREACTION"] != DBNull.Value ? Convert.ToInt32(reader["MYREACTION"]) : 0;
                                if (codMyReaction > 0)
                                {
                                    ReactionListTable rrt3 = ReactionListTable.Find(item => item.id == codMyReaction);
                                    returnListsPosts.myReaction.id = rrt3.id;
                                    returnListsPosts.myReaction.name = rrt3.name;
                                    returnListsPosts.myReaction.linkIcon = rrt3.linkIcon;
                                    returnListsPosts.myReaction.linkIconSelected = rrt3.linkIcon;
                                    returnListsPosts.myReaction.amount = 1;
                                }

                                int editUser = reader["IDGDA_PERSONA_USER"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_PERSONA_USER"].ToString()) == userId ? 1 : 0 : 0;
                                returnListsPosts.EditPost = editUser;


                                if (isAdm == true)
                                {
                                    returnListsPosts.canDeletePost = true;
                                }
                                else
                                {
                                    returnListsPosts.canDeletePost = reader["IDGDA_PERSONA_USER"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_PERSONA_USER"].ToString()) == userId ? true : false : false;
                                }

                                reactionsListPosts reactionsListPosts1 = new reactionsListPosts();
                                ReactionListTable rrt = ReactionListTable.Find(item => item.name == "Like");
                                reactionsListPosts1.id = rrt.id;
                                reactionsListPosts1.name = rrt.name;
                                reactionsListPosts1.linkIcon = rrt.linkIcon;
                                reactionsListPosts1.linkIconSelected = rrt.linkIcon;
                                reactionsListPosts1.amount = reader["LIKES"] != DBNull.Value ? Convert.ToInt32(reader["LIKES"]) : 0;
                                returnListsPosts.reactions.Add(reactionsListPosts1);

                                reactionsListPosts reactionsListPosts2 = new reactionsListPosts();
                                ReactionListTable rrt2 = ReactionListTable.Find(item => item.name == "Love");
                                reactionsListPosts2.id = rrt2.id;
                                reactionsListPosts2.name = rrt2.name;
                                reactionsListPosts2.linkIcon = rrt2.linkIcon;
                                reactionsListPosts2.linkIconSelected = rrt2.linkIcon;
                                reactionsListPosts2.amount = reader["LOVE"] != DBNull.Value ? Convert.ToInt32(reader["LOVE"]) : 0;
                                returnListsPosts.reactions.Add(reactionsListPosts2);

                                returnListsPosts.cod = reader["IDGDA_PERSONA_POSTS"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_PERSONA_POSTS"]) : 0;
                                returnListsPosts.isRepost = reader["ISREPOST"] != DBNull.Value ? Convert.ToInt32(reader["ISREPOST"]) : 0;
                                returnListsPosts.imageUser = reader["IMAGEUSER"] != DBNull.Value ? reader["IMAGEUSER"].ToString() : "";
                                returnListsPosts.nameUser = reader["NAMEUSER"] != DBNull.Value ? reader["NAMEUSER"].ToString() : "";
                                returnListsPosts.hierarchyUser = reader["HIERARCHYUSER"] != DBNull.Value ? reader["HIERARCHYUSER"].ToString() : "";
                                returnListsPosts.post = reader["POST"] != DBNull.Value ? reader["POST"].ToString() : "";
                                returnListsPosts.idPostReference = reader["ID_POST_REFERENCE"] != DBNull.Value ? Convert.ToInt32(reader["ID_POST_REFERENCE"]) : 0;
                                returnListsPosts.postReference = reader["POST_REFERENCE"] != DBNull.Value ? reader["POST_REFERENCE"].ToString() : "";
                                returnListsPosts.imageUserReference = reader["IMAGEUSER_REFERENCE"] != DBNull.Value ? reader["IMAGEUSER_REFERENCE"].ToString() : "";
                                returnListsPosts.nameReference = reader["NAME_REFERENCE"] != DBNull.Value ? reader["NAME_REFERENCE"].ToString() : "";
                                returnListsPosts.hierarchyuserReference = reader["HIERARCHYUSER_REFERENCE"] != DBNull.Value ? reader["HIERARCHYUSER_REFERENCE"].ToString() : "";
                                returnListsPosts.datePost = reader["DATEPOST"] != DBNull.Value ? Convert.ToDateTime(reader["DATEPOST"]).ToString("dd/MM/yyyy HH:mm:ss") : "";
                                returnListsPosts.timeAgo = reader["TIMEAGO"] != DBNull.Value ? reader["TIMEAGO"].ToString() : "";
                                returnListsPosts.comments = reader["COMMENTS"] != DBNull.Value ? Convert.ToInt32(reader["COMMENTS"]) : 0;
                                returnListsPosts.linkFile = reader["LINK_FILE"] != DBNull.Value ? reader["LINK_FILE"].ToString() : "";
                                returnListsPosts.linkFileReference = reader["LINK_FILE_REFERENCE"] != DBNull.Value ? reader["LINK_FILE_REFERENCE"].ToString() : "";
                                returnListsPosts.expiredAt = reader["EXPIRED_AT"] != DBNull.Value ? Convert.ToDateTime(reader["EXPIRED_AT"]) : (DateTime?)null;
                                returnListsPosts.highlight = reader["HIGHLIGHT"] != DBNull.Value ? Convert.ToInt32(reader["HIGHLIGHT"]) : 0;
                                returnListsPosts.allowComment = reader["ALLOW_COMMENT"] != DBNull.Value ? Convert.ToInt32(reader["ALLOW_COMMENT"]) : 0;
                                returnListsPosts.visibility = reader["VISIBILIDADE"] != DBNull.Value ? reader["VISIBILIDADE"].ToString() : "";
                                
                                if (Database.Conn.ToString().Contains("GUILDA_HOMOLOG;"))
                                {
                                    returnListsPosts.linkFile = returnListsPosts.linkFile.Replace("https://10.115.65.4/api/csharp/api/listImage?fileName=", "https://localhost:44359/api/listImage?fileName=");
                                    returnListsPosts.linkFile = returnListsPosts.linkFile.Replace("https://guilda.tahto.net.br/api/csharp/api/listImage?fileName=", "https://localhost:44359/api/listImage?fileName=");
                                }

                                returnListsPosts.linkFileReference = reader["LINK_FILE_REFERENCE"] != DBNull.Value ? reader["LINK_FILE_REFERENCE"].ToString() : "";

                                listPosts.Add(returnListsPosts);

                            }
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            List<returnListsPosts> returnLists = listPosts
             .Select(group => new returnListsPosts
             {
                 cod = group.cod,
                 isRepost = group.isRepost,
                 imageUser = group.imageUser,
                 nameUser = group.nameUser,
                 hierarchyUser = group.hierarchyUser,
                 post = group.post,
                 idPostReference = group.idPostReference,
                 postReference = group.postReference,
                 imageUserReference = group.imageUserReference,
                 nameReference = group.nameReference,
                 hierarchyuserReference = group.hierarchyuserReference,
                 datePost = group.datePost,
                 timeAgo = group.timeAgo,
                 comments = group.comments,
                 linkFile = "",
                 linkFileReference = "",
                 expiredAt = group.expiredAt,
                 highlight = group.highlight,
                 myReaction = group.myReaction,
                 reactions = group.reactions, // Inicializa a lista de reações
                 allowComment = group.allowComment,
                 sumReactions = group.reactions.Sum(objeto => objeto.amount) + group.comments,
                 filesReference = group.linkFileReference.Split(';').Distinct().Select(url => new filesListPosts { url = url }).ToList(),
                 files = group.linkFile.Split(';').Distinct().Select(url => new filesListPosts { url = url }).ToList(),
                 visibility = group.visibility,
                 canDeletePost = group.canDeletePost,
                 //filesReference = group.Select(item => item.linkFileReference).Distinct().Select(link => new filesListPosts { url = link }).ToList(),
                 //files = group.Select(item => item.linkFile).Distinct().Select(link => new filesListPosts { url = link }).ToList() // files = group.Select(item => new filesListPosts { url = item.linkFile }).ToList()
             }).ToList();


            return returnLists;
        }

        public static List<ReactionListTable> returnReationTable()
        {
            List<ReactionListTable> ReactionListTable = new List<ReactionListTable>();
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {


                    string dtAg = DateTime.Now.ToString("yyyy-MM-dd");
                    StringBuilder stb = new StringBuilder();
                    stb.Append("SELECT IDGDA_PERSONA_POSTS_LIKES_REACTION, NAME, LINK_ICON, LINK_ICON_SELECTED ");
                    stb.Append("FROM GDA_PERSONA_POSTS_LIKES_REACTION (NOLOCK) ");
                    stb.Append("WHERE DELETED_AT IS NULL ");

                    using (SqlCommand commandInsert = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ReactionListTable rt = new ReactionListTable();
                                rt.name = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "";
                                rt.id = reader["IDGDA_PERSONA_POSTS_LIKES_REACTION"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_PERSONA_POSTS_LIKES_REACTION"]) : 0;
                                rt.linkIcon = reader["LINK_ICON"] != DBNull.Value ? reader["LINK_ICON"].ToString() : "";
                                rt.linkIconSelected = reader["LINK_ICON_SELECTED"] != DBNull.Value ? reader["LINK_ICON_SELECTED"].ToString() : "";

                                ReactionListTable.Add(rt);
                            }
                        }
                    }

                }
                catch (Exception)
                {

                }
                connection.Close();
            }

            return ReactionListTable;
        }

        #endregion
    }
}