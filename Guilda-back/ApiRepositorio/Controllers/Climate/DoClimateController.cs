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
using static ApiRepositorio.Controllers.SendQuizController;
using System.Linq;
using DocumentFormat.OpenXml.Spreadsheet;
using static TokenService;

namespace ApiRepositorio.Controllers
{

    public class InputModelDoClimate
    {
        public int idClimate { get; set; }
        public int idClimateReason { get; set; }
    }

    public class Climate
    {
        public int idUserReceived { get; set; }
    }

    public class ReturnModelDoClimate
    {
        public string link { get; set; }
    }

    //[Authorize]
    public class DoClimateController : ApiController
    {
        //Realiza um Post // Realizar Repost
        [HttpPost]
        //[ResponseType(typeof(ResultsModel))]
        public IHttpActionResult PostResultsModel([FromBody] InputModelDoClimate inputModel)
        {
            int collaboratorId = 0;
            int personauserId = 0;
            var token = Request.Headers.Authorization?.Parameter;
            //personauserId = BankListClimate.returnPersonaCollaboratorId(collaboratorId);

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            collaboratorId = inf.collaboratorId;
            personauserId = inf.personauserId;

            bool vf = BankDoClimate.verifyInsertClimate(personauserId);

            if (vf == false)
            {
                int codClimate = BankDoClimate.insertClimate(personauserId, inputModel);
                bool validatedCliemate = BankDoClimate.validatedClimate(codClimate);
                BankDoClimate.insertNotificationClimate(validatedCliemate, codClimate, personauserId);
            }

            return Ok("Inserido com sucesso!");
        }
    }

    public class BankDoClimate
    {
        public static bool verifyInsertClimate(int personaUserId)
        {
            bool existe = false;

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    StringBuilder sbInsert = new StringBuilder();
                    sbInsert.Append($"SELECT * FROM GDA_CLIMATE_USER (NOLOCK) ");
                    sbInsert.Append($"WHERE IDGDA_PERSONA = {personaUserId} AND  ");
                    sbInsert.Append($"CONVERT(DATE, CREATED_AT) = '{DateTime.Now.ToString("yyyy-MM-dd")}' ");

                    using (SqlCommand commandInsert = new SqlCommand(sbInsert.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                existe = true;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                connection.Close();
            }

            return existe;
        }

        public static int insertClimate(int personaUserId, InputModelDoClimate input)
        {
            int codInsert = 0;
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {

                string idClimateReason = input.idClimateReason == 0 ? "NULL" : input.idClimateReason.ToString();

                connection.Open();
                try
                {
                    StringBuilder sbInsert = new StringBuilder();
                    sbInsert.Append("INSERT INTO GDA_CLIMATE_USER (IDGDA_PERSONA, IDGDA_CLIMATE, IDGDA_CLIMATE_REASON, IDGDA_CLIMATE_APPLY_TYPE, CREATED_AT) ");
                    sbInsert.Append("VALUES ( ");
                    sbInsert.Append($"{personaUserId},  "); //IDGDA_PERSONA
                    sbInsert.Append($"{input.idClimate},  "); //IDGDA_CLIMATE
                    sbInsert.Append($"{idClimateReason},  "); //IDGDA_CLIMATE_REASON
                    sbInsert.Append("NULL,  "); //IDGDA_CLIMATE_APPLY_TYPE
                    sbInsert.Append("GETDATE() "); //CREATED_AT
                    sbInsert.Append(") ");
                    sbInsert.Append("SELECT @@IDENTITY AS 'CODINSERT' ");

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
        public static bool validatedClimate(int codClimate)
        {
            bool validated = false;
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    StringBuilder sbInsert = new StringBuilder();
                    sbInsert.Append("SELECT CAN_APPLY FROM GDA_CLIMATE (NOLOCK) ");
                    sbInsert.Append($"WHERE IDGDA_CLIMATE = ( SELECT IDGDA_CLIMATE FROM GDA_CLIMATE_USER (NOLOCK) WHERE IDGDA_CLIMATE_USER = {codClimate} ) ");
                    using (SqlCommand commandInsert = new SqlCommand(sbInsert.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                validated = reader["CAN_APPLY"] != DBNull.Value ? true : false;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                connection.Close();
            }
            return validated;
        }
        public static void insertNotificationClimate(bool validatedCliemate, int codClimate, int personauserId)
        {
            if (validatedCliemate == true)
            {
                List<Climate> listPersona = new List<Climate>();
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        StringBuilder stb = new StringBuilder();
                        stb.AppendFormat("SELECT  ");
                        stb.AppendFormat("PU.IDGDA_PERSONA_USER AS PERSONA_ANSWER, ");
                        stb.AppendFormat("PUNEXT.IDGDA_PERSONA_USER AS PERSONA_NEXT_HIERARCHY, ");
                        stb.AppendFormat("PCU.IDGDA_COLLABORATORS AS IDGDA_COLLABORATORS_ANSWER, ");
                        stb.AppendFormat("PCUNEXT.IDGDA_COLLABORATORS AS IDGDA_COLLABORATORS_NEXT_HIERARCHY  ");
                        stb.AppendFormat("FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CD ");
                        stb.AppendFormat("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCU ON PCU.IDGDA_COLLABORATORS = CD.IDGDA_COLLABORATORS  ");
                        stb.AppendFormat("LEFT JOIN GDA_PERSONA_USER (NOLOCK) AS PU ON PU.IDGDA_PERSONA_USER = PCU.IDGDA_PERSONA_USER AND PU.IDGDA_PERSONA_USER_TYPE = 1 AND PU.DELETED_AT IS NULL  ");
                        stb.AppendFormat("LEFT JOIN GDA_CLIMATE_USER (NOLOCK) AS GCU ON GCU.IDGDA_PERSONA = PU.IDGDA_PERSONA_USER AND CONVERT(DATE, GCU.CREATED_AT) >= CONVERT(DATE, DATEADD(DAY, -1, GETDATE()))  ");
                        stb.AppendFormat("LEFT JOIN GDA_CLIMATE (NOLOCK) AS C ON C.IDGDA_CLIMATE = GCU.IDGDA_CLIMATE  ");
                        stb.AppendFormat("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCUNEXT ON PCUNEXT.IDGDA_COLLABORATORS = (CASE WHEN CD.MATRICULA_SUPERVISOR <> 0 THEN CD.MATRICULA_SUPERVISOR  ");
                        stb.AppendFormat("																								WHEN CD.MATRICULA_COORDENADOR <> 0 THEN CD.MATRICULA_COORDENADOR  ");
                        stb.AppendFormat("																								WHEN CD.MATRICULA_GERENTE_II <> 0 THEN CD.MATRICULA_GERENTE_II ");
                        stb.AppendFormat("																								WHEN CD.MATRICULA_GERENTE_I <> 0 THEN CD.MATRICULA_GERENTE_I ");
                        stb.AppendFormat("																								WHEN CD.MATRICULA_DIRETOR <> 0 THEN CD.MATRICULA_DIRETOR ");
                        stb.AppendFormat("																								WHEN CD.MATRICULA_CEO <> 0 THEN CD.MATRICULA_CEO ");
                        stb.AppendFormat("																								END ");
                        stb.AppendFormat("																							  ) ");
                        stb.AppendFormat("LEFT JOIN GDA_PERSONA_USER (NOLOCK) AS PUNEXT ON PUNEXT.IDGDA_PERSONA_USER = PCUNEXT.IDGDA_PERSONA_USER AND PUNEXT.IDGDA_PERSONA_USER_TYPE = 1  AND PUNEXT.DELETED_AT IS NULL   ");
                        stb.AppendFormat("WHERE  ");
                        stb.AppendFormat("CD.CREATED_AT >= CONVERT(DATE, DATEADD(DAY, -1, GETDATE()))  ");
                        stb.AppendFormat("AND C.CAN_APPLY = 1  ");
                        stb.AppendFormat("AND GCU.IDGDA_CLIMATE_USER IS NOT NULL  ");
                        stb.AppendFormat("AND CD.CARGO IS NOT NULL  ");
                        stb.AppendFormat($"AND GCU.IDGDA_CLIMATE_USER = {codClimate} AND PUNEXT.IDGDA_PERSONA_USER IS NOT NULL ");
                        stb.AppendFormat("GROUP BY PU.IDGDA_PERSONA_USER, PUNEXT.IDGDA_PERSONA_USER, PCU.IDGDA_COLLABORATORS, PCUNEXT.IDGDA_COLLABORATORS ");

                        using (SqlCommand commandSelect = new SqlCommand(stb.ToString(), connection))
                        {
                            using (SqlDataReader reader = commandSelect.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    Climate userid = new Climate();
                                    userid.idUserReceived = Convert.ToInt32(reader["PERSONA_NEXT_HIERARCHY"]);
                                    listPersona.Add(userid);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }

                //Criar Notificação Quiz 
                int idnotification = InsertNotificationClimate(codClimate, personauserId);

                //Agrupamento
                List<infsNotification> infNot = getInfsNotification(idnotification);


                List<infsNotification> infNot2 = infNot
                .GroupBy(item => new { item.codNotification })
              .Select(group => new infsNotification
              {
                  codNotification = group.Key.codNotification,
                  idUserSend = group.First().idUserSend,
                  urlUserSend = group.First().urlUserSend,
                  nameUserSend = group.First().nameUserSend,
                  message = group.First().message,
                  file = "",
                  files = group.Select(item => item.file).Distinct().Select(link => new urlFiles { url = link }).ToList() // files = group.Select(item => new filesListPosts { url = item.linkFile }).ToList()
              })
              .ToList();

                //Inserir a notificação do Usuario e depois enviar via Socket
                if (listPersona.Count > 0)
                {
                    foreach (var personaQuiz in listPersona)
                    {
                        //Inserir No Banco
                        int sendId = InsertNotificationForUser(idnotification, personaQuiz.idUserReceived);

                        ////Envia Notificação
                        messageReturned msgInput = new messageReturned();
                        msgInput.type = "Notification";
                        msgInput.data = new dataMessage();
                        msgInput.data.idUserReceive = personaQuiz.idUserReceived;
                        msgInput.data.idNotificationUser = sendId;
                        msgInput.data.idNotification = Convert.ToInt32(idnotification);
                        msgInput.data.idUserSend = infNot2.First().idUserSend;
                        msgInput.data.urlUserSend = infNot2.First().urlUserSend;
                        msgInput.data.nameUserSend = infNot2.First().nameUserSend;
                        msgInput.data.message = infNot2.First().message;
                        msgInput.data.urlFilesMessage = infNot2.First().files;

                        Startup.messageQueue.Enqueue(msgInput);
                    }
                }

            }
        }
        public static int InsertNotificationClimate(int idClimate, int createdBY)
        {
            List<string> listQuiz = new List<string>();

            listQuiz = SelectInfoClimate(idClimate);

            int IDNOTIFICATION = 0;

            StringBuilder InsertNotification = new StringBuilder();
            InsertNotification.Append("INSERT INTO GDA_NOTIFICATION  ");
            InsertNotification.Append("(IDGDA_NOTIFICATION_TYPE, TITLE, NOTIFICATION, CREATED_AT, CREATED_BY, ACTIVE, STARTED_AT, ENDED_AT, REFERER, SENDED_AT)  ");
            InsertNotification.Append("VALUES  ");
            InsertNotification.Append($"('14', 'Clima', '{listQuiz[0]} - {listQuiz[1]} - {listQuiz[2]}', GETDATE(), '{createdBY}', 1, GETDATE(), NULL, '{idClimate}', GETDATE())  ");
            InsertNotification.Append("SELECT  @@IDENTITY AS 'IDGDA_NOTIFICATION' ");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(InsertNotification.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                IDNOTIFICATION = Convert.ToInt32(reader["IDGDA_NOTIFICATION"].ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return IDNOTIFICATION;
        }
        public static List<infsNotification> getInfsNotification(int idNotification)
        {
            List<infsNotification> ifs = new List<infsNotification>();

            try
            {
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        StringBuilder stb = new StringBuilder();
                        stb.AppendFormat("SELECT N.IDGDA_NOTIFICATION AS COD, MAX(P.IDGDA_PERSONA_USER) AS CODBY, MAX(P.NAME) AS NAME,   ");
                        stb.AppendFormat("MAX(P.PICTURE) AS PICTURE, MAX(CD.CARGO) AS CARGO, MAX(N.NOTIFICATION) AS NOTIFICATION, LINK_FILE,   ");
                        stb.AppendFormat("MAX(NT.TYPE) AS TYPE  ");
                        stb.AppendFormat("FROM  GDA_NOTIFICATION (NOLOCK) N ");
                        stb.AppendFormat("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER CU ON CU.IDGDA_PERSONA_USER = N.CREATED_BY   ");
                        stb.AppendFormat("LEFT JOIN GDA_PERSONA_USER (NOLOCK) P ON P.IDGDA_PERSONA_USER = CU.IDGDA_PERSONA_USER AND P.IDGDA_PERSONA_USER_TYPE = 1   ");
                        stb.AppendFormat("LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) CD ON CD.CREATED_AT >= CONVERT(DATE, DATEADD(DAY, -1, GETDATE())) AND CD.IDGDA_COLLABORATORS = CU.IDGDA_COLLABORATORS ");
                        stb.AppendFormat("LEFT JOIN GDA_NOTIFICATION_FILES (NOLOCK) NF ON NF.IDGDA_NOTIFICATION = N.IDGDA_NOTIFICATION  ");
                        stb.AppendFormat("INNER JOIN GDA_NOTIFICATION_TYPE (NOLOCK) NT ON NT.IDGDA_NOTIFICATION_TYPE = N.IDGDA_NOTIFICATION_TYPE  ");
                        stb.AppendFormat("WHERE  ");
                        stb.AppendFormat("N.IDGDA_NOTIFICATION = {0} AND ", idNotification);
                        stb.AppendFormat("N.DELETED_AT IS NULL  ");
                        stb.AppendFormat("GROUP BY N.IDGDA_NOTIFICATION, LINK_FILE  ");

                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    infsNotification ifn = new infsNotification();

                                    ifn.codNotification = Convert.ToInt32(reader["COD"].ToString());
                                    ifn.idUserSend = Convert.ToInt32(reader["CODBY"].ToString());
                                    ifn.urlUserSend = reader["PICTURE"].ToString();
                                    ifn.nameUserSend = reader["NAME"].ToString();
                                    ifn.message = reader["NOTIFICATION"].ToString();
                                    ifn.file = reader["LINK_FILE"].ToString();
                                    ifs.Add(ifn);

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }
            }
            catch (Exception)
            {


            }
            return ifs;
        }
        static List<string> SelectInfoClimate(int idClimate)
        {
            List<string> listClimate = new List<string>();

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    StringBuilder stb = new StringBuilder();
                    stb.Append("SELECT PU.NAME, PU.BC, C.CLIMATE FROM GDA_CLIMATE_USER (NOLOCK) CU ");
                    stb.Append("INNER JOIN GDA_CLIMATE (NOLOCK) AS C ON C.IDGDA_CLIMATE = CU.IDGDA_CLIMATE ");
                    stb.Append("INNER JOIN GDA_PERSONA_USER (NOLOCK) AS PU ON PU.IDGDA_PERSONA_USER  = CU.IDGDA_PERSONA ");
                    stb.Append($"WHERE IDGDA_CLIMATE_USER = {idClimate}");

                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                listClimate.Add(reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "");
                                listClimate.Add(reader["BC"] != DBNull.Value ? reader["BC"].ToString() : "");
                                listClimate.Add(reader["CLIMATE"] != DBNull.Value ? reader["CLIMATE"].ToString() : "");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            return listClimate;
        }
        public static int InsertNotificationForUser(int NOTIFICATION_ID, int userId)
        {
            int retornoId = 0;
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    string query = $"INSERT INTO GDA_NOTIFICATION_USER (IDGDA_NOTIFICATION,IDGDA_PERSONA_USER, SENDED_AT) VALUES ('{NOTIFICATION_ID}','{userId}',GETDATE()) SELECT @@IDENTITY AS 'CODINSERT' ";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                retornoId = Convert.ToInt32(reader["CODINSERT"].ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            return retornoId;
        }

        public static void InserClimateUserNotAnswerd()
        {

            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("SELECT PU.IDGDA_PERSONA_USER AS PERSONA_NO_ANSWER,  PU.NAME, PU.BC, PUNEXT.IDGDA_PERSONA_USER AS PERSONA_NOTIFICATION FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CD ");
            stb.AppendFormat("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCU ON PCU.IDGDA_COLLABORATORS = CD.IDGDA_COLLABORATORS  ");
            stb.AppendFormat("LEFT JOIN GDA_PERSONA_USER (NOLOCK) AS PU ON PU.IDGDA_PERSONA_USER = PCU.IDGDA_PERSONA_USER AND PU.IDGDA_PERSONA_USER_TYPE = 1   ");
            stb.AppendFormat("LEFT JOIN GDA_CLIMATE_USER (NOLOCK) AS GCU ON GCU.IDGDA_PERSONA = PU.IDGDA_PERSONA_USER AND CONVERT(DATE, GCU.CREATED_AT) >= CONVERT(DATE, DATEADD(DAY, -1, GETDATE())) ");
            stb.AppendFormat("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCUNEXT ON PCUNEXT.IDGDA_COLLABORATORS = (CASE WHEN CD.MATRICULA_SUPERVISOR <> 0 THEN CD.MATRICULA_SUPERVISOR  ");
            stb.AppendFormat("																								WHEN CD.MATRICULA_COORDENADOR <> 0 THEN CD.MATRICULA_COORDENADOR  ");
            stb.AppendFormat("																								WHEN CD.MATRICULA_GERENTE_II <> 0 THEN CD.MATRICULA_GERENTE_II ");
            stb.AppendFormat("																								WHEN CD.MATRICULA_GERENTE_I <> 0 THEN CD.MATRICULA_GERENTE_I ");
            stb.AppendFormat("																								WHEN CD.MATRICULA_DIRETOR <> 0 THEN CD.MATRICULA_DIRETOR ");
            stb.AppendFormat("																								WHEN CD.MATRICULA_CEO <> 0 THEN CD.MATRICULA_CEO ");
            stb.AppendFormat("																								END ");
            stb.AppendFormat("																							  ) ");
            stb.AppendFormat("LEFT JOIN GDA_PERSONA_USER (NOLOCK) AS PUNEXT ON PUNEXT.IDGDA_PERSONA_USER = PCUNEXT.IDGDA_PERSONA_USER AND PUNEXT.IDGDA_PERSONA_USER_TYPE = 1   ");
            stb.AppendFormat("WHERE CD.CREATED_AT >= CONVERT(DATE, DATEADD(DAY, -1, GETDATE())) AND GCU.IDGDA_CLIMATE_USER IS NULL AND CD.CARGO IS NOT NULL AND PU.IDGDA_PERSONA_USER IS NOT NULL AND PUNEXT.IDGDA_PERSONA_USER IS NOT NULL ");
            stb.AppendFormat("GROUP BY PU.IDGDA_PERSONA_USER, PU.NAME, PU.BC, PUNEXT.IDGDA_PERSONA_USER ");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn(true)))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                try
                                {
                                    int personaUserNoAnswer = Convert.ToInt32(reader["PERSONA_NO_ANSWER"].ToString());
                                    string name = reader["NAME"].ToString();
                                    string bc = reader["BC"].ToString();
                                    int idPersonaUserRecived = Convert.ToInt32(reader["PERSONA_NOTIFICATION"].ToString());

                                    insertNotification(14, "Clima não Respondido", $"{name} - {bc} - Não respondeu a pergunta Diaria", idPersonaUserRecived);
                                }
                                catch (Exception)
                                {

                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Trate a exceção aqui
                }
                connection.Close();
            }
        }

        public static void insertNotification(int IDGDA_NOTIFICATION_TYPE, string TITLE, string NOTIFICATION, int idCollaborator, int idSend = 0)
        {
            int codInsert = 0;

            StringBuilder stbNot = new StringBuilder();
            stbNot.Append($"INSERT INTO GDA_NOTIFICATION ");
            stbNot.Append($"(IDGDA_NOTIFICATION_TYPE, TITLE, NOTIFICATION, CREATED_AT, CREATED_BY, ACTIVE, STARTED_AT, ENDED_AT, DELETED_AT, DELETED_BY, REFERER, SENDED_AT) ");
            stbNot.Append($"VALUES (  ");
            stbNot.Append($"{IDGDA_NOTIFICATION_TYPE}, ");//IDGDA_NOTIFICATION_TYPE
            stbNot.Append($"'{TITLE}', "); //TITLE
            stbNot.Append($"'{NOTIFICATION}', "); //NOTIFICATION
            stbNot.Append($"GETDATE(),  "); //CREATED_AT
            stbNot.Append($"{idSend},  "); //CREATED_BY
            stbNot.Append($"1,  "); //ACTIVE
            stbNot.Append($"GETDATE(),  "); //STARTED_AT
            stbNot.Append($"NULL, NULL, NULL, NULL, GETDATE()) SELECT @@IDENTITY AS 'CODINSERT' "); //ENDED_AT, DELETED_AT, DELETED_BY, REFERER, SENDED_AT
            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(stbNot.ToString(), connection))
                {

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            codInsert = Convert.ToInt32(reader["CODINSERT"].ToString());
                        }
                    }
                }

                if (codInsert != 0)
                {
                    StringBuilder stbFilter = new StringBuilder();
                    stbFilter.Append($"INSERT INTO GDA_NOTIFICATION_SEND_FILTER (IDGDA_NOTIFICATION, IDGDA_PERSONA_POSTS_VISIBILITY_TYPE, ID_REFERER) ");
                    stbFilter.Append($"VALUES (");
                    stbFilter.Append($"{codInsert},   ");
                    stbFilter.Append($"6,  ");
                    stbFilter.Append($"{idCollaborator} ");
                    stbFilter.Append($") ");

                    using (SqlCommand command = new SqlCommand(stbFilter.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                connection.Close();
            }

            CreatedNotificationController.VisibilityCreateNotification userListConfigPost = new CreatedNotificationController.VisibilityCreateNotification();

            userListConfigPost = SendNotificationController.BancoNotification.returnFiltersNotification(codInsert);

            SendNotificationController.BancoNotification.InsertNotification(codInsert, userListConfigPost);
        }


    }
}