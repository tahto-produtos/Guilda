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
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class CreatedNotificationController : ApiController
    {// POST: api/Results

        public class InputModelCreateNotification
        {
            public int IDGDA_NOTIFICATION_TYPE { get; set; }
            public string TITLE { get; set; }
            public string NOTIFICATION { get; set; }
            public string CREATED_AT { get; set; }
            public int CREATED_BY { get; set; }
            public int ACTIVE { get; set; }
            public DateTime? STARTED_AT { get; set; }
            public DateTime? ENDED_AT { get; set; }
            public VisibilityCreateNotification visibility { get; set; }

        }

        public class VisibilityCreateNotification
        {
            public List<int> sector { get; set; }
            public List<int> subSector { get; set; }
            public List<int> period { get; set; }
            public List<int> hierarchy { get; set; }
            public List<int> group { get; set; }
            public List<int> userId { get; set; }
            public List<int> client { get; set; }
            public List<int> site { get; set; }
            public List<int> homeOrFloor { get; set; }
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


            string idLog = Logs.InsertActionLogs("Insert Notification ", "GDA_NOTIFICATION", personaid.ToString());
            string jsonFromFormData = System.Web.HttpContext.Current.Request.Form["json"];
            InputModelCreateNotification Json = JsonConvert.DeserializeObject<InputModelCreateNotification>(jsonFromFormData);
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            List<GalleryResponseModel> pictures = PictureClass.UploadFilesToBlob(files);
            int IDNOTIFICATION = 0;
            int IDGDA_NOTIFICATION_TYPE = Json.IDGDA_NOTIFICATION_TYPE;
            string TITLE = Json.TITLE;
            string NOTIFICATION = Json.NOTIFICATION;
            int ACTIVE = Json.ACTIVE;
            string STARTED_AT = Json.STARTED_AT != null ? Json.STARTED_AT?.ToString("yyyy-MM-dd HH:mm:ss") : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string ENDED_AT = Json.ENDED_AT != null ? Json.ENDED_AT?.ToString("yyyy-MM-dd HH:mm:ss") : null;
            string FOTO = "";

            IDNOTIFICATION = BancoCreateNotification.InsertNotification(IDGDA_NOTIFICATION_TYPE,TITLE,NOTIFICATION,personaid,ACTIVE,STARTED_AT,ENDED_AT);

            BancoCreateNotification.insertVisibility(IDNOTIFICATION, Json);

            if (files.Count > 0)
            {
                FOTO = pictures.First().url;
                #region INSERT NOTIFICATION FILES
                StringBuilder InsertNotificationFiles = new StringBuilder();
                InsertNotificationFiles.Append("INSERT INTO GDA_NOTIFICATION_FILES  ");
                InsertNotificationFiles.Append("(IDGDA_NOTIFICATION, LINK_FILE, CREATED_AT)  ");
                InsertNotificationFiles.Append("VALUES  ");
                InsertNotificationFiles.Append($"('{IDNOTIFICATION}', '{FOTO}', GETDATE() )  ");
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(InsertNotificationFiles.ToString(), connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"Erro ao inserir GDA_NOTIFICATION_FILES: {ex.Message}");
                    }
                }

                #endregion

            }
            return Ok("Notificação criada com sucesso.");
            
        }
        // Método para serializar um DataTable em JSON


        #region Banco

        public class BancoCreateNotification
        {
            public static void insertVisibility(int codNotification, CreatedNotificationController.InputModelCreateNotification inputPost)
            {
                //Sector
                if (inputPost.visibility.sector.Count > 0)
                {
                    foreach (int item in inputPost.visibility.sector)
                    {
                        inserVisibilityItem(codNotification, 1, item);
                    }
                }
                //SubSector
                if (inputPost.visibility.subSector.Count > 0)
                {
                    foreach (int item in inputPost.visibility.subSector)
                    {
                        inserVisibilityItem(codNotification, 2, item);
                    }
                }
                //Period
                if (inputPost.visibility.period.Count > 0)
                {
                    foreach (int item in inputPost.visibility.period)
                    {
                        inserVisibilityItem(codNotification, 3, item);
                    }
                }
                //Hierarchy
                if (inputPost.visibility.hierarchy.Count > 0)
                {
                    foreach (int item in inputPost.visibility.hierarchy)
                    {
                        inserVisibilityItem(codNotification, 4, item);
                    }
                }
                //Group
                if (inputPost.visibility.group.Count > 0)
                {
                    foreach (int item in inputPost.visibility.group)
                    {
                        inserVisibilityItem(codNotification, 5, item);
                    }
                }
                //UserId
                if (inputPost.visibility.userId.Count > 0)
                {
                    foreach (int item in inputPost.visibility.userId)
                    {
                        inserVisibilityItem(codNotification, 6, item);
                    }
                }
                //Client
                if (inputPost.visibility.client.Count > 0)
                {
                    foreach (int item in inputPost.visibility.client)
                    {
                        inserVisibilityItem(codNotification, 7, item);
                    }
                }
                //HomeOrFloor
                if (inputPost.visibility.homeOrFloor.Count > 0)
                {
                    foreach (int item in inputPost.visibility.homeOrFloor)
                    {
                        inserVisibilityItem(codNotification, 8, item);
                    }
                }
                //Site
                if (inputPost.visibility.site.Count > 0)
                {
                    foreach (int item in inputPost.visibility.site)
                    {
                        inserVisibilityItem(codNotification, 9, item);
                    }
                }

            }

            public static void inserVisibilityItem(int codNotification, int visibilityTipe, int idReferer)
            {
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {

                        StringBuilder sbInsert = new StringBuilder();
                        sbInsert.AppendFormat("INSERT INTO GDA_NOTIFICATION_SEND_FILTER (IDGDA_NOTIFICATION, IDGDA_PERSONA_POSTS_VISIBILITY_TYPE, ID_REFERER) ");
                        sbInsert.AppendFormat("VALUES ( ");
                        sbInsert.AppendFormat("{0}, ", codNotification); //IDGDA_NOTIFICATION
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

            public static int InsertNotification(int IDGDA_NOTIFICATION_TYPE, string TITLE, string NOTIFICATION, int PERSONAID, int ACTIVE, string STARTED_AT, string ENDED_AT)
            {
                string ENDEDATSTR = "";
                if (ENDED_AT == null)
                {
                    ENDEDATSTR = $"NULL";
                }
                else
                {
                    ENDEDATSTR = $"'{ENDED_AT}'";
                }

                int IDNOTIFICATION = 0;
                #region INSERT GDA_NOTIFICATION
                StringBuilder InsertNotification = new StringBuilder();
                InsertNotification.Append("INSERT INTO GDA_NOTIFICATION  ");
                InsertNotification.Append("(IDGDA_NOTIFICATION_TYPE, TITLE, NOTIFICATION, CREATED_AT, CREATED_BY, ACTIVE, STARTED_AT, ENDED_AT)  ");
                InsertNotification.Append("VALUES  ");
                InsertNotification.Append($"('{IDGDA_NOTIFICATION_TYPE}', '{TITLE}', '{NOTIFICATION}', GETDATE(), {PERSONAID}, {ACTIVE}, '{STARTED_AT}',{ENDEDATSTR})  ");
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
                #endregion
                return IDNOTIFICATION;
            }

        }


        #endregion

    }



}