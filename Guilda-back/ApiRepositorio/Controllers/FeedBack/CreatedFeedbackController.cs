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
using static ApiRepositorio.Controllers.SearchAccountsController;
using static ApiRepositorio.Controllers.SendNotificationController;
using DocumentFormat.OpenXml.Wordprocessing;
using static ApiRepositorio.Controllers.CreatedNotificationController;
using Antlr.Runtime.Misc;
using static ApiRepositorio.Controllers.LoadMyNotificationController;
using OfficeOpenXml.ConditionalFormatting;
using static ClosedXML.Excel.XLPredefinedFormat;
using static ApiRepositorio.Controllers.SendQuizController;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class CreatedFeedbackController : ApiController
    {// POST: api/Results

        public class InputModelCreatedFeedback
        {
            public int IDGDA_PEDAGOGICAL_SCALE_GRAVITY { get; set; }
            public int IDGDA_PEDAGOGICAL_SCALE { get; set; }
            public int IDPERSONA_RECEIVED_BY { get; set; }
            public string REASON { get; set; }
            public string DETAILS { get; set; }
        }



        [HttpPost]
        public IHttpActionResult PostResultsModel()
        {
            int collaboratorId = 0;
            int personaid = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            collaboratorId = inf.collaboratorId;
            personaid = inf.personauserId;

            string jsonFromFormData = System.Web.HttpContext.Current.Request.Form["json"];
            InputModelCreatedFeedback Json = JsonConvert.DeserializeObject<InputModelCreatedFeedback>(jsonFromFormData);
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            List<GalleryResponseModel> pictures = PictureClass.UploadFilesToBlob(files);
            int IDGDA_FEEDBACK = 0;
            int IDGDA_PEDAGOGICAL_SCALE = Json.IDGDA_PEDAGOGICAL_SCALE;
            int IDGDA_PEDAGOGICAL_SCALE_GRAVITY = Json.IDGDA_PEDAGOGICAL_SCALE_GRAVITY;
            int IDPERSONA_RECEIVED_BY = Json.IDPERSONA_RECEIVED_BY;
            string REASON = Json.REASON;
            string DETAILS = Json.DETAILS;
            string FOTO = "";

            //Verifica se o usuario ja logou na ferramenda
            bool logFerramenta = BancoCreatedFeedBack.VerifyLoginUser(personaid);
            if (logFerramenta == false)
            {
                return Ok("Não foi possivel enviar o feedback, pois o colaborador não logou nenhuma vez.");
            }


            //INSERÇÃO DO FEEDBACK
            IDGDA_FEEDBACK = BancoCreatedFeedBack.InsertFeedBack(personaid, IDGDA_PEDAGOGICAL_SCALE_GRAVITY, REASON, DETAILS);

            //CASO CONTER ARQUIVOS, INSERÇÃO DOS ARQUIVOS DO FEEDBACK

            foreach (GalleryResponseModel item in pictures)
            {
                //Insiro na tabela de GDA_PERSONA_POSTS_FILES
                BancoCreatedFeedBack.InsertFeedBackFiles(item.url, IDGDA_FEEDBACK);
            }

            //INSERÇÃO DO FEEDBACK AO USUARIO DESTINADO
            int idUserFeedBack = BancoCreatedFeedBack.InsertFeedBackUser(IDGDA_FEEDBACK, IDGDA_PEDAGOGICAL_SCALE, personaid, IDPERSONA_RECEIVED_BY);

            int idnotification = BancoCreatedFeedBack.InsertNotificationFeedBack(REASON, DETAILS, idUserFeedBack, personaid);

            //Inserir No Banco
            int sendId = BancoQuiz.InsertNotificationForUser(idnotification, IDPERSONA_RECEIVED_BY);

            List<urlFiles> uff = new List<urlFiles>();
            foreach (string item in files)
            {
                urlFiles uf = new urlFiles();
                uf.url = item;
                uff.Add(uf);
            }

            //Agrupamento
            List<SendQuizController.infsNotification> infNot = BancoQuiz.getInfsNotification(idnotification);


            List<SendQuizController.infsNotification> infNot2 = infNot
            .GroupBy(item => new { item.codNotification })
            .Select(group => new SendQuizController.infsNotification
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

            ////Envia Notificação
            messageReturned msgInput = new messageReturned();
            msgInput.type = "Notification";
            msgInput.data = new dataMessage();
            msgInput.data.idUserReceive = IDPERSONA_RECEIVED_BY;
            msgInput.data.idNotificationUser = sendId;
            msgInput.data.idNotification = Convert.ToInt32(idnotification);
            msgInput.data.idUserSend = infNot2.First().idUserSend;
            msgInput.data.urlUserSend = infNot2.First().urlUserSend;
            msgInput.data.nameUserSend = infNot2.First().nameUserSend;
            msgInput.data.message = infNot2.First().message;
            msgInput.data.urlFilesMessage = infNot2.First().files;

            Startup.messageQueue.Enqueue(msgInput);

            return Ok("Escala pedagogica aplicada com sucesso.");

        }
        // Método para serializar um DataTable em JSON


        #region Banco

        public class BancoCreatedFeedBack
        {
            public static int InsertNotificationFeedBack(string title, string notification, int idUserFeedBack, int createdBY)
            {

                int IDNOTIFICATION = 0;

                StringBuilder InsertNotification = new StringBuilder();
                InsertNotification.Append("INSERT INTO GDA_NOTIFICATION  ");
                InsertNotification.Append("(IDGDA_NOTIFICATION_TYPE, TITLE, NOTIFICATION, CREATED_AT, CREATED_BY, ACTIVE, STARTED_AT, ENDED_AT, REFERER, SENDED_AT)  ");
                InsertNotification.Append("VALUES  ");
                InsertNotification.Append($"('17', '{title}', '{notification}', GETDATE(), '{createdBY}', 1, GETDATE(), NULL, '{idUserFeedBack}', GETDATE())  ");
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

            public static bool VerifyLoginUser(int PersonaCreatedUser)
            {
                bool retorno = false;
                #region INSERT GDAFEEDBACK 
                StringBuilder InsertFeedBack = new StringBuilder();
                InsertFeedBack.Append($"SELECT TOP 1 IDGDA_LOGIN_ACCESS FROM GDA_LOGIN_ACCESS (NOLOCK) LA ");
                InsertFeedBack.Append($"INNER JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) PCU ON PCU.IDGDA_COLLABORATORS = LA.IDGDA_COLLABORATOR ");
                InsertFeedBack.Append($"INNER JOIN GDA_PERSONA_USER (NOLOCK) PU ON PU.IDGDA_PERSONA_USER = PCU.IDGDA_PERSONA_USER AND PU.IDGDA_PERSONA_USER_TYPE = 1 ");
                InsertFeedBack.Append($"WHERE PCU.IDGDA_PERSONA_USER = {PersonaCreatedUser} ");

                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(InsertFeedBack.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    retorno = true;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                #endregion
                return retorno;
            }


            public static int InsertFeedBack(int PersonaCreatedUser, int idPedagogicalScaleGravity, string Reason, string Details)
            {
                int retorno = 0;
                #region INSERT GDAFEEDBACK 
                StringBuilder InsertFeedBack = new StringBuilder();
                InsertFeedBack.Append("INSERT INTO GDA_FEEDBACK  ");
                InsertFeedBack.Append("(IDGDA_PEDAGOGICAL_SCALE_GRAVITY, CREATED_AT, CREATED_BY, REASON, DETAILS)  ");
                InsertFeedBack.Append("VALUES  ");
                InsertFeedBack.Append($"('{idPedagogicalScaleGravity}', GETDATE(), '{PersonaCreatedUser}', '{Reason}', '{Details}')  ");
                InsertFeedBack.Append("SELECT  @@IDENTITY AS 'IDGDA_FEEDBACK' ");
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(InsertFeedBack.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    retorno = Convert.ToInt32(reader["IDGDA_FEEDBACK"].ToString());
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                #endregion
                return retorno;

            }

            public static void InsertFeedBackFiles(string foto, int idgda_feedback)
            {
                #region INSERT NOTIFICATION FILES
                StringBuilder InsertFeedBackFiles = new StringBuilder();
                InsertFeedBackFiles.Append("INSERT INTO GDA_FEEDBACK_FILES  ");
                InsertFeedBackFiles.Append("(IDGDA_FEEDBACK, FILES) ");
                InsertFeedBackFiles.Append("VALUES  ");
                InsertFeedBackFiles.Append($"('{idgda_feedback}', '{foto}' )  ");
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(InsertFeedBackFiles.ToString(), connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }

                #endregion
            }

            public static int InsertFeedBackUser(int idFeedBack, int idPedagogicalScale, int PersonaCreatedUser, int PersonaReceivedUser)
            {
                int retorno = 0;
                string DataSuspensao = "";
                string Protocol = ProtocolGenerator.GenerateProtocolNumber();
                int days = ValidatedDaysSuspend(idPedagogicalScale);

                if (days != 0)
                {
                    // Criar um TimeSpan com o número de dias
                    TimeSpan timeToAdd = TimeSpan.FromDays(days);
                    DataSuspensao = $"'{System.DateTime.Now.Add(timeToAdd).ToString("yyyy-MM-dd 00:00:00")}' ";
                }
                else
                {
                    DataSuspensao = "NULL";
                }


                #region INSERT GDA_FEEDBACK_USER 
                StringBuilder InsertFeedBackUser = new StringBuilder();
                InsertFeedBackUser.Append("INSERT INTO GDA_FEEDBACK_USER  ");
                InsertFeedBackUser.Append("(IDGDA_FEEDBACK, IDGDA_PEDAGOGICAL_SCALE, PROTOCOL, IDPERSONA_SENDED_BY, IDPERSONA_RECEIVED_BY, CREATED_AT, SUSPENDED_UNTIL, SIGNED_CREATOR) ");
                InsertFeedBackUser.Append("VALUES  ");
                InsertFeedBackUser.Append($"('{idFeedBack}', '{idPedagogicalScale}',{Protocol}, {PersonaCreatedUser}, {PersonaReceivedUser}, GETDATE(), {DataSuspensao}, 1)  ");
                InsertFeedBackUser.Append("SELECT  @@IDENTITY AS 'IDGDA_FEEDBACK_USER' ");
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(InsertFeedBackUser.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    retorno = Convert.ToInt32(reader["IDGDA_FEEDBACK_USER"].ToString());
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }
                #endregion

                return retorno;
            }

            public static int ValidatedDaysSuspend(int idPedagogicalScale)
            {
                int retorno = 0;
                StringBuilder sb = new StringBuilder();
                sb.Append($"SELECT TIME_OFF FROM GDA_PEDAGOGICAL_SCALE WHERE IDGDA_PEDAGOGICAL_SCALE = {idPedagogicalScale}");
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    retorno = Convert.ToInt32(reader["TIME_OFF"].ToString());
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                return retorno;
            }


        }
        public class ProtocolGenerator
        {
            private static readonly Random random = new Random();

            public static string GenerateProtocolNumber()
            {
                // Gera um número aleatório com 6 dígitos
                int randomNumber = random.Next(100000, 1000000); // 100000 a 999999
                return randomNumber.ToString();
            }
        }
        #endregion

    }



}