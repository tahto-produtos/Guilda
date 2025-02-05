using ApiRepositorio;
using ApiRepositorio.Controllers;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Office;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using static ApiRepositorio.Controllers.LoadMyNotificationController;
using static ApiRepositorio.Controllers.SendQuizController;
using static ApiRepositorio.Controllers.TesteController;

namespace ApiC.Class.DowloadFile
{
    public class ScheduledNotification
    {


        private static Timer timer;
        private static Timer timer2;
        private static Timer timer3;
        static readonly object timerLock = new object();
        public static List<int> VerificacaoNotificacao()
        {
            List<int> retorno = new List<int>();
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT IDGDA_NOTIFICATION FROM GDA_NOTIFICATION (NOLOCK) N  ");
                sb.Append("WHERE N.ACTIVE = 1  ");
                sb.Append("AND N.DELETED_AT IS NULL  ");
                sb.Append("AND N.STARTED_AT <= GETDATE()  ");
                sb.Append("AND (N.ENDED_AT IS NULL	OR GETDATE() <= N.ENDED_AT)  ");
                sb.Append("AND N.SENDED_AT IS NULL  ");

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
                                    retorno.Add(int.Parse(reader["IDGDA_NOTIFICATION"].ToString()));
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

            return retorno;
        }
        public static List<int> VerificacaoQuiz()
        {
            List<int> retorno = new List<int>();
            try
            {

                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT IDGDA_QUIZ FROM GDA_QUIZ(NOLOCK) Q ");
                sb.Append("WHERE Q.DELETED_AT IS NULL ");
                sb.Append("AND Q.SENDED_AT IS NULL ");
                sb.Append("AND Q.STARTED_AT <= GETDATE() ");
                sb.Append("AND (Q.ENDED_AT IS NULL OR GETDATE() <= Q.ENDED_AT)  ");

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
                                    retorno.Add(int.Parse(reader["IDGDA_QUIZ"].ToString()));
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
            return retorno;

        }
        public static List<int> VerificacaoExpiracao()
        {
            List<int> retorno = new List<int>();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT  ");
            sb.AppendFormat("CA.ID AS IDGDA, ");
            sb.AppendFormat("PCU.IDGDA_PERSONA_USER,  ");
            sb.AppendFormat("CA.DUE_AT ");
            sb.AppendFormat("FROM GDA_CHECKING_ACCOUNT (NOLOCK) AS CA ");
            sb.AppendFormat("INNER JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCU ON PCU.IDGDA_COLLABORATORS = CA.COLLABORATOR_ID ");
            sb.AppendFormat("INNER JOIN GDA_PERSONA_USER (NOLOCK) AS PU ON PCU.IDGDA_PERSONA_USER = PU.IDGDA_PERSONA_USER AND PU.IDGDA_PERSONA_USER_TYPE = 1 AND PU.DELETED_AT IS NULL ");
            sb.AppendFormat("WHERE CA.INPUT > 0 AND  ");
            sb.AppendFormat("(CA.INPUT - CA.INPUT_USED) > 0 AND  ");
            sb.AppendFormat("CA.DUE_AT IS NOT NULL ");

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
                                retorno.Add(int.Parse(reader["IDGDA_PERSONA_USER"].ToString()));
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
            stbNot.Append($"NULL, NULL, NULL, NULL, NULL) SELECT @@IDENTITY AS 'CODINSERT' "); //ENDED_AT, DELETED_AT, DELETED_BY, REFERER, SENDED_AT
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
        }

        public static List<int> idsPeronasMarketingPlace(int collaboratorId)
        {
            List<int> ids = new List<int>();
            StringBuilder stbNot = new StringBuilder();

            stbNot.Append($"SELECT C.IDGDA_COLLABORATORS, PU.IDGDA_PERSONA_USER FROM GDA_PROFILE_PERMIT (NOLOCK) AS PP ");
            stbNot.Append($"INNER JOIN GDA_PROFILE_COLLABORATOR_ADMINISTRATION (NOLOCK) AS PCA ON PP.PROFILE_ID = PCA.ID AND DELETED_AT IS NULL ");
            stbNot.Append($"INNER JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.PROFILE_COLLABORATOR_ADMINISTRATIONID = PP.PROFILE_ID ");
            stbNot.Append($"INNER JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCU ON PCU.IDGDA_COLLABORATORS = C.IDGDA_COLLABORATORS ");
            stbNot.Append($"INNER JOIN GDA_PERSONA_USER (NOLOCK) AS PU ON PCU.IDGDA_PERSONA_USER = PU.IDGDA_PERSONA_USER AND PU.IDGDA_PERSONA_USER_TYPE = 1 AND PU.DELETED_AT IS NULL ");
            stbNot.Append($"WHERE PERMISSION_ID = 30 ");
            stbNot.Append($"AND PP.ACTIVE = 1 ");
            stbNot.Append($"GROUP BY C.IDGDA_COLLABORATORS, PU.IDGDA_PERSONA_USER ");

            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(stbNot.ToString(), connection))
                    {

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ids.Add(Convert.ToInt32(reader["IDGDA_PERSONA_USER"].ToString()));
                            }
                        }
                    }
                }
                catch (Exception)
                {

                }
                connection.Close();
            }

            stbNot.Clear();
            stbNot.Append("SELECT TOP 1 CU.IDGDA_PERSONA_USER FROM GDA_PERSONA_COLLABORATOR_USER (NOLOCK) CU ");
            stbNot.Append("INNER JOIN GDA_PERSONA_USER (NOLOCK) PU ON CU.IDGDA_PERSONA_USER = PU.IDGDA_PERSONA_USER AND PU.IDGDA_PERSONA_USER_TYPE = 1 ");
            stbNot.Append("WHERE CU.IDGDA_COLLABORATORS = ");
            stbNot.Append("( ");
            stbNot.Append("SELECT TOP 1 ");
            stbNot.Append("CASE WHEN MATRICULA_SUPERVISOR <> 0 THEN MATRICULA_SUPERVISOR ");
            stbNot.Append("     WHEN MATRICULA_COORDENADOR <> 0 THEN MATRICULA_COORDENADOR ");
            stbNot.Append("	 WHEN MATRICULA_GERENTE_II <> 0 THEN MATRICULA_GERENTE_II ");
            stbNot.Append("	 WHEN MATRICULA_GERENTE_I <> 0 THEN MATRICULA_GERENTE_I ");
            stbNot.Append("	 WHEN MATRICULA_DIRETOR <> 0 THEN MATRICULA_DIRETOR ");
            stbNot.Append("	 WHEN MATRICULA_CEO <> 0 THEN MATRICULA_CEO ");
            stbNot.Append("ELSE 0 END AS GESTOR_IMEDIATO ");
            stbNot.Append("FROM GDA_COLLABORATORS_DETAILS (NOLOCK) ");
            stbNot.Append($"WHERE CREATED_AT >= CONVERT(DATE, DATEADD(DAY, -3, GETDATE())) AND IDGDA_COLLABORATORS = {collaboratorId} ");
            stbNot.Append("ORDER BY CREATED_AT DESC ");
            stbNot.Append(") ");


            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(stbNot.ToString(), connection))
                    {

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (Convert.ToInt32(reader["IDGDA_PERSONA_USER"].ToString()) != 0)
                                {
                                    List<int> idExist = ids.FindAll(i => i == Convert.ToInt32(reader["IDGDA_PERSONA_USER"].ToString())).ToList();

                                    if (idExist.Count == 0)
                                    {
                                        ids.Add(Convert.ToInt32(reader["IDGDA_PERSONA_USER"].ToString()));
                                    }
                                    
                                }                                
                            }
                        }
                    }
                }
                catch (Exception)
                {

                }
                connection.Close();
            }


            return ids;
        }


        public static void insertNotificationMktPlace(int IDGDA_NOTIFICATION_TYPE, string TITLE, string NOTIFICATION, bool adms, int idSend = 0, bool idSendEnv = false, int collaboratorId = 0)
        {
            List<int> ids = new List<int>();
            //Enviar para todos os adms que tem permissão de realizar o administrar pedidos
            if (adms)
            {
                ids = idsPeronasMarketingPlace(collaboratorId);
            }
            //Enviar atualização para quem realizou o pedido
            if (idSendEnv)
            {
                int idFind = ids.Find(i => i == idSend);


                if (idFind == 0)
                {
                    ids.Add(idSend);
                }
                
            }

            int codInsert = 0;

            if (adms == true && ids.Count > 0 || adms == false)
            {
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

                    //Agrupamento
                    List<infsNotification> infNot = BankDoClimate.getInfsNotification(codInsert);

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

                    if (codInsert != 0)
                    {
                        foreach (int item in ids)
                        {
                            StringBuilder stbFilter = new StringBuilder();
                            stbFilter.Append($"INSERT INTO GDA_NOTIFICATION_SEND_FILTER (IDGDA_NOTIFICATION, IDGDA_PERSONA_POSTS_VISIBILITY_TYPE, ID_REFERER) ");
                            stbFilter.Append($"VALUES (");
                            stbFilter.Append($"{codInsert},   ");
                            stbFilter.Append($"6,  ");
                            stbFilter.Append($"{item} ");
                            stbFilter.Append($") ");

                            using (SqlCommand command = new SqlCommand(stbFilter.ToString(), connection))
                            {
                                command.ExecuteNonQuery();
                            }

                            stbFilter.Clear();
                            stbFilter.Append($"INSERT INTO GDA_NOTIFICATION_USER (IDGDA_NOTIFICATION, IDGDA_PERSONA_USER, SENDED_AT, VIEWED_AT, DELETED_AT, REFERER) ");
                            stbFilter.Append($"VALUES (");
                            stbFilter.Append($"{codInsert}, "); //IDGDA_NOTIFICATION
                            stbFilter.Append($"{item}, "); //IDGDA_PERSONA_USER
                            stbFilter.Append($"GETDATE(), "); //SENDED_AT
                            stbFilter.Append($"NULL, "); //VIEWED_AT
                            stbFilter.Append($"NULL, "); //DELETED_AT
                            stbFilter.Append($"NULL "); //REFERER
                            stbFilter.Append($") SELECT @@IDENTITY AS 'CODINSERT' ");
                            int retornoId = 0;
                            using (SqlCommand command = new SqlCommand(stbFilter.ToString(), connection))
                            {
                                //command.ExecuteNonQuery();
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        retornoId = Convert.ToInt32(reader["CODINSERT"].ToString());
                                    }
                                }
                            }


                            ////Envia Notificação
                            messageReturned msgInput = new messageReturned();
                            msgInput.type = "Notification";
                            msgInput.data = new dataMessage();
                            msgInput.data.idUserReceive = item;
                            msgInput.data.idNotificationUser = retornoId;
                            msgInput.data.idNotification = Convert.ToInt32(codInsert);
                            msgInput.data.idUserSend = infNot2.First().idUserSend;
                            msgInput.data.urlUserSend = infNot2.First().urlUserSend;
                            msgInput.data.nameUserSend = infNot2.First().nameUserSend;
                            msgInput.data.message = infNot2.First().message;
                             msgInput.data.urlFilesMessage = infNot2.First().files;

                            Startup.messageQueue.Enqueue(msgInput);
                        }
                    }

                    connection.Close();
                }
            }

        }


        public static void ExecuteActionStage(object state)
        {
            //Next Stage
            CreatedAutomaticActionEscalationController.BancoCreatedAutomaticActionEscalation.VerificyEscalationAction();

            //Config Automatica
            processEscalationAutomatic();

            //Validação Campanha
            //automaticProcessOperationCampaign();


            try
            {
                DateTime ultimoDia = DateTime.Now;
                if (ultimoDia.Hour >= 01)
                {
                    ultimoDia = DateTime.Now;

                    ultimoDia = new DateTime(ultimoDia.Year, ultimoDia.Month, ultimoDia.Day, 04, 0, 0);
                }
                else
                {
                    ultimoDia = DateTime.Now.AddDays(-1);

                    ultimoDia = new DateTime(ultimoDia.Year, ultimoDia.Month, ultimoDia.Day, 04, 0, 0);
                }

                ultimoDia = ultimoDia.AddDays(1);
                DateTime now = DateTime.Now;
                int dueTime = (int)(ultimoDia - now).TotalMilliseconds;
                timer2.Change(dueTime, 24 * 60 * 60 * 1000);
            }
            catch (Exception ex)
            {


            }

        }

        public static void ExecuteTasksScheduledNearExpiredAndExpired(object state)
        {

            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                connection.Open();
                try
                {
                    StringBuilder stbExpirou = new StringBuilder();
                    stbExpirou.Append($"SELECT SUM(COIN_EXPIRED) AS SOMA, COLLABORATOR_ID, MAX(PU.IDGDA_PERSONA_USER) AS IDGDA_PERSONA_USER  ");
                    stbExpirou.Append($"FROM GDA_CHECKING_ACCOUNT (NOLOCK) CA ");

                    stbExpirou.Append($"LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCU ON PCU.IDGDA_COLLABORATORS = CA.COLLABORATOR_ID ");
                    stbExpirou.Append($"LEFT JOIN GDA_PERSONA_USER AS PU ON IDGDA_PERSONA_USER_TYPE = 1 AND PCU.IDGDA_PERSONA_USER = PU.IDGDA_PERSONA_USER ");

                    stbExpirou.Append($"WHERE CA.DUE_AT = CONVERT(DATE, DATEADD(DAY, -1, GETDATE())) ");
                    stbExpirou.Append($"GROUP BY CA.COLLABORATOR_ID ");

                    using (SqlCommand command = new SqlCommand(stbExpirou.ToString(), connection))
                    {

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int moedas = reader["SOMA"] != DBNull.Value ? Convert.ToInt32(reader["SOMA"].ToString()) : 0;
                                int idPersona = reader["IDGDA_PERSONA_USER"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_PERSONA_USER"].ToString()) : 0;
                                if (moedas == 0 || idPersona == 0)
                                {
                                    continue;
                                }

                                insertNotification(8, "EXPIRAÇÃO MOEDAS", $"Você teve {moedas} moedas expiradas", idPersona);
                            }
                        }
                    }

                    StringBuilder stbProximoExp = new StringBuilder();
                    stbProximoExp.Append($"SELECT SUM(INPUT - INPUT_USED) AS SOMA, COLLABORATOR_ID, MAX(PU.IDGDA_PERSONA_USER) AS IDGDA_PERSONA_USER  ");
                    stbProximoExp.Append($"FROM GDA_CHECKING_ACCOUNT (NOLOCK) AS CA ");

                    stbProximoExp.Append($"LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCU ON PCU.IDGDA_COLLABORATORS = CA.COLLABORATOR_ID ");
                    stbProximoExp.Append($"LEFT JOIN GDA_PERSONA_USER AS PU ON IDGDA_PERSONA_USER_TYPE = 1 AND PCU.IDGDA_PERSONA_USER = PU.IDGDA_PERSONA_USER ");

                    stbProximoExp.Append($"WHERE CA.DUE_AT = CONVERT(DATE,  GETDATE()) ");
                    stbProximoExp.Append($"AND (CA.INPUT - CA.INPUT_USED) > 0 ");
                    stbProximoExp.Append($"GROUP BY CA.COLLABORATOR_ID ");

                    using (SqlCommand command = new SqlCommand(stbProximoExp.ToString(), connection))
                    {

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int moedas = reader["SOMA"] != DBNull.Value ? Convert.ToInt32(reader["SOMA"].ToString()) : 0;
                                int idPersona = reader["IDGDA_PERSONA_USER"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_PERSONA_USER"].ToString()) : 0;
                                if (moedas == 0 || idPersona == 0)
                                {
                                    continue;
                                }

                                insertNotification(9, "MOEDAS PROXIMAS A EXPIRAR", $"Você tem {moedas} moedas que irão expirar hoje!", idPersona);
                            }
                        }
                    }

                }
                catch (Exception)
                {

                }

                connection.Close();

            }

            try
            {
                DateTime ultimoDia = DateTime.Now;
                if (ultimoDia.Hour >= 01)
                {
                    ultimoDia = DateTime.Now;

                    ultimoDia = new DateTime(ultimoDia.Year, ultimoDia.Month, ultimoDia.Day, 01, 0, 0);
                }
                else
                {
                    ultimoDia = DateTime.Now.AddDays(-1);

                    ultimoDia = new DateTime(ultimoDia.Year, ultimoDia.Month, ultimoDia.Day, 01, 0, 0);
                }

                ultimoDia = ultimoDia.AddDays(1);
                DateTime now = DateTime.Now;
                int dueTime = (int)(ultimoDia - now).TotalMilliseconds;
                timer2.Change(dueTime, 24 * 60 * 60 * 1000);
            }
            catch (Exception ex)
            {


            }
        }
        public static void ExecuteTaskScheduled(object state)
        {

            //Notification
            List<int> listNotification = VerificacaoNotificacao();

            foreach (int idNot in listNotification)
            {

                CreatedNotificationController.VisibilityCreateNotification userListConfigPost = new CreatedNotificationController.VisibilityCreateNotification();

                userListConfigPost = SendNotificationController.BancoNotification.returnFiltersNotification(idNot);

                SendNotificationController.BancoNotification.InsertNotification(idNot, userListConfigPost);
            }

            //Quiz
            List<int> listQuiz = VerificacaoQuiz();

            foreach (int idNot in listQuiz)
            {
                CreatedQuizController.VisibilityCreateQuiz userListConfigPost = new CreatedQuizController.VisibilityCreateQuiz();

                userListConfigPost = SendQuizController.BancoQuiz.returnFiltersQuiz(idNot);

                int personauser = SendQuizController.BancoQuiz.ReturnPersonaUserCreatedQuiz(idNot);

                SendQuizController.BancoQuiz.InsertQuiz(idNot, personauser, userListConfigPost);
            }

            //Expiração
            //List<int> listExpiracao = VerificacaoExpiracao();
            //if(listExpiracao.Count >0)
            //{
            //    int idNotification = CreatedNotificationController.BancoCreateNotification.InsertNotification(8, "Expiracao Moedas", "Voce tem moedas que será expirada.", 0, 1, DateTime.Now.ToString("yyyy-MM-dd 00:00:00"), DateTime.Now.ToString("yyyy-MM-dd 23:59:59"));
            //    List<int> listNotification2 = VerificacaoNotificacao();
            //    foreach (int idNot in listNotification2)
            //    {
            //        CreatedNotificationController.VisibilityCreateNotification userListConfigPost = new CreatedNotificationController.VisibilityCreateNotification();

            //        userListConfigPost = SendNotificationController.BancoNotification.returnFiltersNotification(idNot);

            //        SendNotificationController.BancoNotification.InsertNotification(idNot, userListConfigPost);
            //    }
            //}



            // Obtém a data e hora atual
            DateTime datainicio = DateTime.Now;

            // Calcula a próxima hora cheia
            //DateTime proximaHora = DateTime.Now.AddHours(1);
            DateTime proximaHora = DateTime.Now.AddMinutes(2);
            proximaHora = new DateTime(proximaHora.Year, proximaHora.Month, proximaHora.Day, proximaHora.Hour, proximaHora.Minute, 0);

            // Calcula o tempo até a próxima hora cheia
            int dueTime = (int)(proximaHora - DateTime.Now).TotalMilliseconds;
            //int period = 60 * 60 * 1000;
            int period = 120 * 1000;
            timer.Change(dueTime, period);
        }
        public static void Scheduled()
        {
            lock (timerLock)
            {
                // Configurar o timer para chamar o método ExecuteTask de uma Hora em um Hora
                try
                {
                    // Calcula o horário para a primeira execução (próxima hora cheia)
                    DateTime now = DateTime.Now;
                    //DateTime scheduledTime = new DateTime(now.Year, now.Month, now.Day, now.Hour + 1, 0, 0);

                    DateTime scheduledTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute + 2, 0);


                    // Se já passou da próxima hora cheia, avança para a próxima hora
                    if (now > scheduledTime)
                    {
                        //scheduledTime = scheduledTime.AddHours(1);
                        scheduledTime = scheduledTime.AddMinutes(2);
                    }
                    // Calcula o tempo até a próxima execução
                    int dueTime = (int)(scheduledTime - now).TotalMilliseconds;

                    //int period = 60 * 60 * 1000;

                    int period = 120000;

                    // Cria o timer
                    timer = new System.Threading.Timer(state => ExecuteTaskScheduled(null), null, dueTime, period);


                    DateTime scheduledTime2 = new DateTime(now.Year, now.Month, now.Day, 01, 00, 0);
                    if (now > scheduledTime2)
                    {
                        scheduledTime2 = scheduledTime2.AddDays(1);
                    }

                    int dueTime2 = (int)(scheduledTime2 - now).TotalMilliseconds;
                    timer2 = new System.Threading.Timer(ExecuteTasksScheduledNearExpiredAndExpired, null, dueTime2, 24 * 60 * 60 * 1000);


                    //ESCALATION
                    DateTime scheduledTime3 = new DateTime(now.Year, now.Month, now.Day, 04, 00, 0);
                    if (now > scheduledTime3)
                    {
                        scheduledTime3 = scheduledTime3.AddDays(1);
                    }

                    int dueTime3 = (int)(scheduledTime3 - now).TotalMilliseconds;
                    timer3 = new System.Threading.Timer(ExecuteActionStage, null, dueTime3, 24 * 60 * 60 * 1000);

                }
                catch (Exception ex)
                {

                }
                // Inicializa o timer com intervalo de 1 hora (em milissegundos)
            }
        }

        #region Escalation

        public class EscalationAutomatic
        {

            public int IDGDA_ESCALATION_AUTOMATIC_SECTORS { get; set; }
            public int IDGDA_INDICATOR { get; set; }
            public int IDGDA_SECTOR { get; set; }
            public int IDGDA_SUBSECTOR { get; set; }
            public int IDGDA_GROUP { get; set; }
            public int PERCENTAGE_DETOUR { get; set; }
            public double TOLERANCE_RANGE { get; set; }
            public int IDGDA_HIERARCHY { get; set; }
            public int NUMBER_STAGE { get; set; }
            public List<EscalationAutomaticListStages> LIST_STAGES { get; set; }

        }

        public class EscalationAutomaticListStages
        {
            public int IDGDA_HIERARCHY { get; set; }
            public int NUMBER_STAGE { get; set; }
        }

        public static void processEscalationAutomatic()
        {
            //Processo de pegar as configs automaticas ativas

            List<EscalationAutomatic> EscalationAutomatics = new List<EscalationAutomatic>();

            StringBuilder stb = new StringBuilder();
            stb.Append("SELECT GEA.IDGDA_ESCALATION_AUTOMATIC_SECTORS, ");
            stb.Append("       IDGDA_INDICATOR, ");
            stb.Append("       IDGDA_SECTOR, ");
            stb.Append("       IDGDA_SUBSECTOR, ");
            stb.Append("       IDGDA_GROUP, ");
            stb.Append("       PERCENTAGE_DETOUR, ");
            stb.Append("       TOLERANCE_RANGE, ");
            stb.Append("       IDGDA_HIERARCHY, ");
            stb.Append("       NUMBER_STAGE ");
            stb.Append("FROM GDA_ESCALATION_AUTOMATIC_SECTORS (NOLOCK) GEA ");
            stb.Append("LEFT JOIN GDA_ESCALATION_ACTION_STAGE (NOLOCK) GEAS ON GEAS.IDGDA_ESCALATION_AUTOMATIC_SECTORS = GEA.IDGDA_ESCALATION_AUTOMATIC_SECTORS ");
            stb.Append("WHERE GEA.DELETED_AT IS NULL ");


            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                {
                    command.CommandTimeout = 0;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            EscalationAutomatic EscalationAtm = new EscalationAutomatic();

                            EscalationAtm.IDGDA_ESCALATION_AUTOMATIC_SECTORS = reader["IDGDA_ESCALATION_AUTOMATIC_SECTORS"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_ESCALATION_AUTOMATIC_SECTORS"].ToString()) : 0;
                            EscalationAtm.IDGDA_INDICATOR = reader["IDGDA_INDICATOR"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_INDICATOR"].ToString()) : 0;
                            //EscalationAtm.IDGDA_ESCALATION_ACTION_STAGE = reader["IDGDA_ESCALATION_ACTION_STAGE"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_ESCALATION_ACTION_STAGE"].ToString()) : 0;
                            EscalationAtm.IDGDA_SECTOR = reader["IDGDA_SECTOR"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_SECTOR"].ToString()) : 0;
                            EscalationAtm.IDGDA_SUBSECTOR = reader["IDGDA_SUBSECTOR"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_SUBSECTOR"].ToString()) : 0;
                            EscalationAtm.IDGDA_GROUP = reader["IDGDA_GROUP"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_GROUP"].ToString()) : 0;
                            EscalationAtm.PERCENTAGE_DETOUR = reader["PERCENTAGE_DETOUR"] != DBNull.Value ? Convert.ToInt32(reader["PERCENTAGE_DETOUR"].ToString()) : 0;
                            EscalationAtm.TOLERANCE_RANGE = reader["TOLERANCE_RANGE"] != DBNull.Value ? Convert.ToDouble(reader["TOLERANCE_RANGE"].ToString()) : 0;
                            EscalationAtm.IDGDA_HIERARCHY = reader["IDGDA_HIERARCHY"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_HIERARCHY"].ToString()) : 0;
                            EscalationAtm.NUMBER_STAGE = reader["NUMBER_STAGE"] != DBNull.Value ? Convert.ToInt32(reader["NUMBER_STAGE"].ToString()) : 0;

                            EscalationAutomatics.Add(EscalationAtm);
                        }
                    }
                }
                connection.Close();
            }


            EscalationAutomatics = EscalationAutomatics.GroupBy(item => new { item.IDGDA_ESCALATION_AUTOMATIC_SECTORS })
                               .Select(grupo => new EscalationAutomatic
                               {
                                   IDGDA_ESCALATION_AUTOMATIC_SECTORS = grupo.Key.IDGDA_ESCALATION_AUTOMATIC_SECTORS,
                                   IDGDA_INDICATOR = grupo.First().IDGDA_INDICATOR,

                                   IDGDA_SECTOR = grupo.First().IDGDA_SECTOR,

                                   IDGDA_SUBSECTOR = grupo.First().IDGDA_SUBSECTOR,
                                   IDGDA_GROUP = grupo.First().IDGDA_GROUP,
                                   PERCENTAGE_DETOUR = grupo.First().PERCENTAGE_DETOUR,
                                   TOLERANCE_RANGE = grupo.First().TOLERANCE_RANGE,
                                   IDGDA_HIERARCHY = grupo.First().IDGDA_HIERARCHY,
                                   NUMBER_STAGE = grupo.First().NUMBER_STAGE,

                                   LIST_STAGES = grupo.Select(r => new EscalationAutomaticListStages
                                   {
                                       IDGDA_HIERARCHY = r.IDGDA_HIERARCHY,
                                       NUMBER_STAGE = r.NUMBER_STAGE
                                   }).ToList()
                               }).ToList();

            foreach (EscalationAutomatic eatm in EscalationAutomatics)
            {
                int idIndicator = eatm.IDGDA_INDICATOR;
                string codCollaborator = "";
                string dtInicial = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
                string dtFinal = DateTime.Now.ToString("yyyy-MM-dd");
                //string dtInicial = "2024-01-01";
                //string dtFinal = "2024-01-30";

                string hierarchiesAsString = "";
                string indicatorsAsString = eatm.IDGDA_INDICATOR.ToString();
                //string indicatorsAsString = "728";
                string sectorsAsString = eatm.IDGDA_SECTOR.ToString() == "0" ? "" : eatm.IDGDA_SECTOR.ToString();
                string subSectorsAsString = eatm.IDGDA_SUBSECTOR.ToString() == "0" ? "" : eatm.IDGDA_SUBSECTOR.ToString();
                bool? bkt = true;
                int porcentage_detour = eatm.PERCENTAGE_DETOUR;
                double tolerance_range = eatm.TOLERANCE_RANGE;

                List<ResultConsolidatedController.HomeResultConsolidated> rmams = new List<ResultConsolidatedController.HomeResultConsolidated>();
                rmams = ResultConsolidatedController.ReturnHomeResultConsolidated(codCollaborator, dtInicial, dtFinal, hierarchiesAsString, indicatorsAsString, sectorsAsString, subSectorsAsString, "", "", "", bkt);

                if (idIndicator == 10000013 || idIndicator == 10000014)
                {
                    string filterEnv = $"'{idIndicator}'";
                    List<ResultConsolidatedController.HomeResultConsolidated> listaIndicadorAcesso = new List<ResultConsolidatedController.HomeResultConsolidated>();
                    listaIndicadorAcesso = ResultConsolidatedController.ReturnHomeResultConsolidatedAccess(codCollaborator, dtInicial, dtFinal, hierarchiesAsString, sectorsAsString, subSectorsAsString, "", "", "", bkt, filterEnv);
                    rmams = rmams.Concat(listaIndicadorAcesso).ToList();
                }

                rmams = rmams.GroupBy(d => new { d.IDINDICADOR }).Select(item => new ResultConsolidatedController.HomeResultConsolidated
                {
                    MATRICULA = item.First().MATRICULA,
                    CARGO = item.First().CARGO,
                    //CODGIP = item.First().CODGIP,
                    //SETOR = item.First().SETOR,
                    IDINDICADOR = item.Key.IDINDICADOR,
                    INDICADOR = item.First().INDICADOR,
                    QTD = item.Sum(d => d.QTD),
                    META = Math.Round(item.Sum(d => d.META) / item.Count(), 2, MidpointRounding.AwayFromZero),
                    FACTOR0 = item.Sum(d => d.FACTOR0),
                    FACTOR1 = item.Sum(d => d.FACTOR1),
                    //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                    min1 = Math.Round(item.Sum(d => d.min1) / item.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                    min2 = Math.Round(item.Sum(d => d.min2) / item.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                    min3 = Math.Round(item.Sum(d => d.min3) / item.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                    min4 = Math.Round(item.Sum(d => d.min4) / item.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                    CONTA = item.First().CONTA,
                    BETTER = item.First().BETTER,
                    //META_MAXIMA_MOEDAS = Math.Round(item.Sum(d => d.META_MAXIMA_MOEDAS) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                    META_MAXIMA_MOEDAS = item.Sum(d => d.QTD_MON != 0 ? Math.Round(d.QTD_MON_TOTAL / d.QTD_MON, 0, MidpointRounding.AwayFromZero) : 0),

                    SUMDIASLOGADOS = item.Sum(d => d.SUMDIASLOGADOS),
                    SUMDIASESCALADOS = item.Sum(d => d.SUMDIASESCALADOS),
                    SUMDIASLOGADOSESCALADOS = item.Sum(d => d.SUMDIASLOGADOSESCALADOS),

                    //MOEDA_GANHA =  Math.Round(item.Sum(d => d.MOEDA_GANHA) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                    MOEDA_GANHA = Math.Round(item.Sum(d => d.MOEDA_GANHA), 0, MidpointRounding.AwayFromZero),
                    TYPE = item.First().TYPE,
                }).ToList();


                ResultConsolidatedController.HomeResultConsolidated consolidado = rmams[0];
                rmams[0] = ResultConsolidatedController.DoCalculateFinal(consolidado);

                double resultado = rmams[0].RESULTADO;
                double meta = rmams[0].META;
                double percentual = rmams[0].PERCENTUAL;
                string biggerBetter = rmams[0].BETTER;
                double faltaAtingir = 100 - percentual;

                if (porcentage_detour != 0)
                {
                    //Caso maior melhor
                    if (porcentage_detour <= faltaAtingir)
                    {
                        //Verifica se ja existe um escalation em vigor
                        if (verifyScalationCreated(eatm) == false)
                        {
                            //Realiza Cadastro Automatico Escalation
                            realizeAutomaticInsertEscalation(eatm);
                        }

                    }
                }
                if (tolerance_range != 0)
                {
                    if (tolerance_range >= percentual)
                    {
                        //Verifica se ja existe um escalation em vigor
                        if (verifyScalationCreated(eatm) == false)
                        {
                            //Realiza Cadastro Automatico Escalation
                            realizeAutomaticInsertEscalation(eatm);
                        }

                    }
                }
            }
        }

        public static bool verifyScalationCreated(EscalationAutomatic eatm)
        {
            bool retorno = false;

            string filterSubsetor = eatm.IDGDA_SUBSECTOR == 0 ? " AND IDGDA_SUBSECTOR IS NULL " : $" AND IDGDA_SUBSECTOR = {eatm.IDGDA_SUBSECTOR} ";


            StringBuilder stb = new StringBuilder();
            stb.Append("SELECT * FROM GDA_ESCALATION_ACTION (NOLOCK) ");
            stb.Append($"WHERE IDGDA_SECTOR = {eatm.IDGDA_SECTOR} ");
            stb.Append($" {filterSubsetor} ");
            stb.Append($"AND IDGDA_INDICATOR = {eatm.IDGDA_INDICATOR} ");
            stb.Append($"AND STARTED_AT <= GETDATE() ");
            stb.Append($"AND ENDED_AT >= GETDATE() ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                {
                    command.CommandTimeout = 0;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            retorno = true;
                        }
                    }
                }
                connection.Close();
            }

            return retorno;

        }

        public static void realizeAutomaticInsertEscalation(EscalationAutomatic eatm)
        {
            CreatedActionEscalationController.InputModelCreatedActionEscalation imc = new CreatedActionEscalationController.InputModelCreatedActionEscalation();
            imc.IDGDA_INDICATOR = eatm.IDGDA_INDICATOR;
            imc.IDGDA_PERSONA_RESPONSIBLE_CREATION = 0;
            imc.IDGDA_PERSONA_RESPONSIBLE_ACTION = 0;
            imc.IDGDA_SECTOR = eatm.IDGDA_SECTOR;
            imc.IDGDA_SUBSECTOR = eatm.IDGDA_SUBSECTOR;

            imc.NAME = $"Ação criada automatica para melhorar o indicador: {eatm.IDGDA_INDICATOR}.";
            imc.DESCRIPTION = $"Ação criada automatica para melhorar o indicador: {eatm.IDGDA_INDICATOR}.";
            imc.STARTED_AT = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
            imc.ENDED_AT = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd 00:00:00");

            int idActionEscalation = CreatedActionEscalationController.BancoCreatedActionEscalation.InsertCreatedActionEscalation(imc);

            foreach (EscalationAutomaticListStages item in eatm.LIST_STAGES)
            {
                CreatedActionEscalationController.BancoCreatedActionEscalation.InsertAutomaticStageActionEscalation(idActionEscalation, item.IDGDA_HIERARCHY, item.NUMBER_STAGE);
            }


            //CreatedActionEscalationController.BancoCreatedActionEscalation.InsertHistoryActionEscalation(idActionEscalation, imc.DESCRIPTION, 0);
        }

        public static void automaticProcessOperationCampaign()
        {

            List<OperationalCampaignItem> ocs = new List<OperationalCampaignItem>();

            //Pegar campanhas dentro do periodo
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();

                try
                {
                    //Criação da campanha
                    StringBuilder sb = new StringBuilder();
                    sb.Append($"SELECT OCU.IDGDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT, OC.IDGDA_OPERATIONAL_CAMPAIGN, OCU.IDGDA_PERSONA, PCU.IDGDA_COLLABORATORS,  ");
                    sb.Append($"OCE.IDGDA_INDICATOR AS ELIM_IDGDA_INDICATOR, OCE.[PERCENT] AS ELIM_PERCENT, OCE.INDICATOR_INCREASE AS ELIM_INDICATOR_INCREASE, ");
                    sb.Append($"OCP.IDGDA_INDICATOR, OCP.[PERCENT], OCP.INDICATOR_INCREASE, OCP.REWARD_POINTS ");
                    sb.Append($"FROM GDA_OPERATIONAL_CAMPAIGN (NOLOCK) OC  ");
                    sb.Append($"INNER JOIN GDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT (NOLOCK) OCU ON OC.IDGDA_OPERATIONAL_CAMPAIGN = OCU.IDGDA_OPERATIONAL_CAMPAIGN  ");
                    sb.Append($"INNER JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) PCU ON PCU.IDGDA_PERSONA_USER  = OCU.IDGDA_PERSONA ");
                    sb.Append($"INNER JOIN GDA_OPERATIONAL_CAMPAIGN_ELIMINATION (NOLOCK) OCE ON OCE.IDGDA_OPERATIONAL_CAMPAIGN = OC.IDGDA_OPERATIONAL_CAMPAIGN ");
                    sb.Append($"INNER JOIN GDA_OPERATIONAL_CAMPAIGN_PONTUATION (NOLOCK) OCP ON OCP.IDGDA_OPERATIONAL_CAMPAIGN = OC.IDGDA_OPERATIONAL_CAMPAIGN ");
                    sb.Append($"WHERE STARTED_AT <= CONVERT(DATE, GETDATE())  ");
                    sb.Append($"AND ENDED_AT >= CONVERT(DATE, GETDATE())  ");

                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                OperationalCampaignItem oc = new OperationalCampaignItem();
                                oc.idgda_operational_campaign_user_participant = reader["IDGDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT"]) : 0;
                                oc.idgda_operational_campaign = reader["IDGDA_OPERATIONAL_CAMPAIGN"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_OPERATIONAL_CAMPAIGN"]) : 0;
                                oc.idPersona = reader["IDGDA_PERSONA"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_PERSONA"]) : 0;
                                oc.idcollaborator = reader["IDGDA_COLLABORATORS"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_COLLABORATORS"]) : 0;
                                oc.elimIdIndicator = reader["ELIM_IDGDA_INDICATOR"] != DBNull.Value ? Convert.ToInt32(reader["ELIM_IDGDA_INDICATOR"]) : 0;
                                oc.elimPercent = reader["ELIM_PERCENT"] != DBNull.Value ? Convert.ToDouble(reader["ELIM_PERCENT"]) : 0;
                                oc.elimIndicatorIncrease = reader["ELIM_INDICATOR_INCREASE"] != DBNull.Value ? Convert.ToInt32(reader["ELIM_INDICATOR_INCREASE"]) : 0;
                                oc.idIndicator = reader["IDGDA_INDICATOR"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_INDICATOR"]) : 0;
                                oc.percent = reader["PERCENT"] != DBNull.Value ? Convert.ToDouble(reader["PERCENT"]) : 0;
                                oc.indicatorIncrease = reader["INDICATOR_INCREASE"] != DBNull.Value ? Convert.ToInt32(reader["INDICATOR_INCREASE"]) : 0;
                                oc.rewardPoints = reader["REWARD_POINTS"] != DBNull.Value ? Convert.ToInt32(reader["REWARD_POINTS"]) : 0;
                                ocs.Add(oc);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }

                connection.Close();

                List<OperationalCampaign> operationalCampaigns = ocs.GroupBy(item => item.idgda_operational_campaign).Select(group =>
                {
                    var operationalCampaign = new OperationalCampaign
                    {
                        idgda_operational_campaign = group.Key,
                        listUser = group.GroupBy(item => item.idgda_operational_campaign_user_participant)
                        .Select(grp => grp.First())
                        .Select(item => new OperationalCampaignUser
                        {
                            idgda_operational_campaign_user_participant = item.idgda_operational_campaign_user_participant,
                            idPersona = item.idPersona,
                            idcollaborator = item.idcollaborator
                        }).ToList(),
                        listElimination = group.GroupBy(item => item.elimIdIndicator)
                        .Select(grp => grp.First())
                        .Select(item => new OperationalCampaignElimination
                        {
                            idIndicator = item.elimIdIndicator,
                            percent = item.elimPercent,
                            indicatorIncrease = item.elimIndicatorIncrease
                        }).ToList(),
                        listIndicators = group.GroupBy(item => item.idIndicator)
                        .Select(grp => grp.First())
                        .Select(item => new OperationalCampaignIndicators
                        {
                            idIndicator = item.idIndicator,
                            percent = item.percent,
                            indicatorIncrease = item.indicatorIncrease,
                            rewardPoints = item.rewardPoints
                        }).ToList()
                    };
                    return operationalCampaign;
                }).ToList();

                foreach (OperationalCampaign ocEnv in operationalCampaigns)
                {
                    string listIndicatorsElim = string.Join(",", ocEnv.listElimination.Select(elim => elim.idIndicator));
                    string listIndicatorsPont = string.Join(",", ocEnv.listIndicators.Select(elim => elim.idIndicator));
                    string listIndicators = string.Join(",", listIndicatorsElim, listIndicatorsPont);
                    string listUser = $" AND CL.IDGDA_COLLABORATORS IN ({string.Join(",", ocEnv.listUser.Select(elim => elim.idcollaborator))}) ";

                    List<ResultConsolidatedController.HomeResultConsolidated> resultadoPrimeiroDiaCampanha = new List<ResultConsolidatedController.HomeResultConsolidated>();
                    resultadoPrimeiroDiaCampanha = ResultConsolidatedController.ReturnHomeResultConsolidated("", ocEnv.dtStart.ToString("yyyy-MM-dd"), ocEnv.dtStart.ToString("yyyy-MM-dd"), "", listIndicators, "", "", "", "", "", true, listUser);

                    List<ResultConsolidatedController.HomeResultConsolidated> resultadoDuranteCampanha = new List<ResultConsolidatedController.HomeResultConsolidated>();
                    resultadoDuranteCampanha = ResultConsolidatedController.ReturnHomeResultConsolidated("", ocEnv.dtStart.ToString("yyyy-MM-dd"), ocEnv.dtEnd.ToString("yyyy-MM-dd"), "", listIndicators, "", "", "", "", "", true, listUser);

                    foreach (OperationalCampaignUser user in ocEnv.listUser)
                    {
                        //Realizar inserções de pontuações e missões alcançadas
                        foreach (OperationalCampaignIndicators pont in ocEnv.listIndicators)
                        {
                            ResultConsolidatedController.HomeResultConsolidated primResult = new ResultConsolidatedController.HomeResultConsolidated();
                            ResultConsolidatedController.HomeResultConsolidated segundResult = new ResultConsolidatedController.HomeResultConsolidated();
                            primResult = resultadoPrimeiroDiaCampanha.Find(kkk => kkk.MATRICULA == user.idcollaborator.ToString() && kkk.IDINDICADOR == pont.idIndicator.ToString());
                            segundResult = resultadoDuranteCampanha.Find(kkk => kkk.MATRICULA == user.idcollaborator.ToString() && kkk.IDINDICADOR == pont.idIndicator.ToString());

                            ResultConsolidatedController.DoCalculateFinal(primResult);
                            ResultConsolidatedController.DoCalculateFinal(segundResult);

                            double percentAltered = 0;
                            percentAltered = segundResult.PERCENTUAL - primResult.PERCENTUAL;

                            if (pont.indicatorIncrease == 1 && percentAltered > 0)
                            {
                                if (percentAltered >= pont.percent)
                                {
                                    //Verifica se o usuario ja pontuou
                                    bool ValidPoints = Funcoes.ValidaPoints(ocEnv.idgda_operational_campaign, pont.idIndicator, user.idPersona);

                                    //Realizar Pontuacao
                                    if (ValidPoints == true)
                                    {
                                        Funcoes.InserirPoints(ocEnv.idgda_operational_campaign, pont.idIndicator, user.idPersona);
                                    }
                                }

                            }

                            else if (pont.indicatorIncrease == 0 && percentAltered < 0)
                            {
                                percentAltered = percentAltered * (-1);
                                if (percentAltered >= pont.percent)
                                {
                                    //Verifica se o usuario ja pontuou
                                    bool ValidPoints = Funcoes.ValidaPoints(ocEnv.idgda_operational_campaign, pont.idIndicator, user.idPersona);

                                    //Realizar Pontuacao
                                    if (ValidPoints == true)
                                    {
                                        Funcoes.InserirPoints(ocEnv.idgda_operational_campaign, pont.idIndicator, user.idPersona);
                                    }
                                }
                            }

                        }

                        //Verificar criterio de eliminação
                        foreach (OperationalCampaignElimination elim in ocEnv.listElimination)
                        {
                            ResultConsolidatedController.HomeResultConsolidated primResult = new ResultConsolidatedController.HomeResultConsolidated();
                            ResultConsolidatedController.HomeResultConsolidated segundResult = new ResultConsolidatedController.HomeResultConsolidated();
                            primResult = resultadoPrimeiroDiaCampanha.Find(kkk => kkk.MATRICULA == user.idcollaborator.ToString() && kkk.IDINDICADOR == elim.idIndicator.ToString());
                            segundResult = resultadoDuranteCampanha.Find(kkk => kkk.MATRICULA == user.idcollaborator.ToString() && kkk.IDINDICADOR == elim.idIndicator.ToString());

                            ResultConsolidatedController.DoCalculateFinal(primResult);
                            ResultConsolidatedController.DoCalculateFinal(segundResult);

                            double percentAltered = 0;
                            percentAltered = segundResult.PERCENTUAL - primResult.PERCENTUAL;

                            //Se aumentou
                            if (elim.indicatorIncrease == 1 && percentAltered > 0)
                            {
                                if (percentAltered >= elim.percent)
                                {
                                    //Realizar eliminação
                                    Funcoes.InserirEliminacao(ocEnv.idgda_operational_campaign, elim.idIndicator, user.idPersona);
                                }
                            }
                            else if (elim.indicatorIncrease == 0 && percentAltered < 0)
                            {
                                percentAltered = percentAltered * (-1);
                                if (percentAltered >= elim.percent)
                                {
                                    //Realizar eliminação
                                    Funcoes.InserirEliminacao(ocEnv.idgda_operational_campaign, elim.idIndicator, user.idPersona);
                                }
                            }
                        }
                    }

                    //Realizar Notificação
                    List<ResultConsolidatedController.HomeResultConsolidated> retornoPrimeiroDia = new List<ResultConsolidatedController.HomeResultConsolidated>();
                    retornoPrimeiroDia = resultadoPrimeiroDiaCampanha.GroupBy(d => new { d.IDINDICADOR }).Select(item => new ResultConsolidatedController.HomeResultConsolidated
                    {
                        MATRICULA = item.First().MATRICULA,
                        CARGO = item.First().CARGO,
                        //CODGIP = item.First().CODGIP,
                        //SETOR = item.First().SETOR,
                        IDINDICADOR = item.Key.IDINDICADOR,
                        INDICADOR = item.First().INDICADOR,
                        QTD = item.Sum(d => d.QTD),
                        META = Math.Round(item.Sum(d => d.META) / item.Count(), 2, MidpointRounding.AwayFromZero),
                        FACTOR0 = item.Sum(d => d.FACTOR0),
                        FACTOR1 = item.Sum(d => d.FACTOR1),
                        //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                        min1 = Math.Round(item.Sum(d => d.min1) / item.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                        min2 = Math.Round(item.Sum(d => d.min2) / item.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                        min3 = Math.Round(item.Sum(d => d.min3) / item.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                        min4 = Math.Round(item.Sum(d => d.min4) / item.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                        CONTA = item.First().CONTA,
                        BETTER = item.First().BETTER,
                        //META_MAXIMA_MOEDAS = Math.Round(item.Sum(d => d.META_MAXIMA_MOEDAS) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                        META_MAXIMA_MOEDAS = item.Sum(d => d.QTD_MON != 0 ? Math.Round(d.QTD_MON_TOTAL / d.QTD_MON, 0, MidpointRounding.AwayFromZero) : 0),

                        SUMDIASLOGADOS = item.Sum(d => d.SUMDIASLOGADOS),
                        SUMDIASESCALADOS = item.Sum(d => d.SUMDIASESCALADOS),
                        SUMDIASLOGADOSESCALADOS = item.Sum(d => d.SUMDIASLOGADOSESCALADOS),

                        //MOEDA_GANHA =  Math.Round(item.Sum(d => d.MOEDA_GANHA) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                        MOEDA_GANHA = Math.Round(item.Sum(d => d.MOEDA_GANHA), 0, MidpointRounding.AwayFromZero),
                        TYPE = item.First().TYPE,
                    }).ToList();

                    List<ResultConsolidatedController.HomeResultConsolidated> retornoDiasAtuais = new List<ResultConsolidatedController.HomeResultConsolidated>();
                    retornoDiasAtuais = resultadoDuranteCampanha.GroupBy(d => new { d.IDINDICADOR }).Select(item => new ResultConsolidatedController.HomeResultConsolidated
                    {
                        MATRICULA = item.First().MATRICULA,
                        CARGO = item.First().CARGO,
                        //CODGIP = item.First().CODGIP,
                        //SETOR = item.First().SETOR,
                        IDINDICADOR = item.Key.IDINDICADOR,
                        INDICADOR = item.First().INDICADOR,
                        QTD = item.Sum(d => d.QTD),
                        META = Math.Round(item.Sum(d => d.META) / item.Count(), 2, MidpointRounding.AwayFromZero),
                        FACTOR0 = item.Sum(d => d.FACTOR0),
                        FACTOR1 = item.Sum(d => d.FACTOR1),
                        //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                        min1 = Math.Round(item.Sum(d => d.min1) / item.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                        min2 = Math.Round(item.Sum(d => d.min2) / item.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                        min3 = Math.Round(item.Sum(d => d.min3) / item.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                        min4 = Math.Round(item.Sum(d => d.min4) / item.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                        CONTA = item.First().CONTA,
                        BETTER = item.First().BETTER,
                        //META_MAXIMA_MOEDAS = Math.Round(item.Sum(d => d.META_MAXIMA_MOEDAS) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                        META_MAXIMA_MOEDAS = item.Sum(d => d.QTD_MON != 0 ? Math.Round(d.QTD_MON_TOTAL / d.QTD_MON, 0, MidpointRounding.AwayFromZero) : 0),

                        SUMDIASLOGADOS = item.Sum(d => d.SUMDIASLOGADOS),
                        SUMDIASESCALADOS = item.Sum(d => d.SUMDIASESCALADOS),
                        SUMDIASLOGADOSESCALADOS = item.Sum(d => d.SUMDIASLOGADOSESCALADOS),

                        //MOEDA_GANHA =  Math.Round(item.Sum(d => d.MOEDA_GANHA) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                        MOEDA_GANHA = Math.Round(item.Sum(d => d.MOEDA_GANHA), 0, MidpointRounding.AwayFromZero),
                        TYPE = item.First().TYPE,
                    }).ToList();

                    foreach (OperationalCampaignIndicators pont in ocEnv.listIndicators)
                    {
                        ResultConsolidatedController.HomeResultConsolidated primResult = new ResultConsolidatedController.HomeResultConsolidated();
                        ResultConsolidatedController.HomeResultConsolidated segundResult = new ResultConsolidatedController.HomeResultConsolidated();
                        primResult = retornoPrimeiroDia.Find(kkk => kkk.IDINDICADOR == pont.idIndicator.ToString());
                        segundResult = resultadoDuranteCampanha.Find(kkk => kkk.IDINDICADOR == pont.idIndicator.ToString());

                        ResultConsolidatedController.DoCalculateFinal(primResult);
                        ResultConsolidatedController.DoCalculateFinal(segundResult);

                        double percentAltered = 0;
                        percentAltered = segundResult.PERCENTUAL - primResult.PERCENTUAL;

                        if (pont.indicatorIncrease == 1 && percentAltered > 0)
                        {
                            if (percentAltered >= pont.percent)
                            {
                                //Enviar Notificação com o aumento
                                Funcoes.EnvioNotCampanha(ocEnv.idgda_operational_campaign, pont.idIndicator, percentAltered, true);
                            }


                        }
                        else if (pont.indicatorIncrease == 0 && percentAltered < 0)
                        {
                            percentAltered = percentAltered * (-1);
                            if (percentAltered >= pont.percent)
                            {
                                //Enviar Notificação com a baixa
                                Funcoes.EnvioNotCampanha(ocEnv.idgda_operational_campaign, pont.idIndicator, percentAltered, false);
                            }
                        }

                    }
                    //Realizar rankiamento da campanha
                    Funcoes.RankearCampanha(ocEnv.idgda_operational_campaign);
                }

                //Pegar campanhas finalizadas com pagamento automaticos ainda não efetuados
                Funcoes.PagamentoAutomaticoCampanha();


            }

        }

        #endregion
    }
}