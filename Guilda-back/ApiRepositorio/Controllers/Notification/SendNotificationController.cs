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
using DocumentFormat.OpenXml.Office2019.Presentation;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class SendNotificationController : ApiController
    {// POST: api/Results

        public class infsNotification
        {
            public int codNotification { get; set; }
            public int idUserSend { get; set; }
            public string urlUserSend { get; set; }
            public string nameUserSend { get; set; }
            public string message { get; set; }
            public string file { get; set; }
            public List<urlFiles> files { get; set; }
        }
        public class PersonaNotification
        {
            public int idUserReceived { get; set; }
        }

        public class InputModel
        {
            public int NOTIFICATION_ID { get; set; }

        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {
            int collaboratorId = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            collaboratorId = inf.collaboratorId;


            CreatedNotificationController.VisibilityCreateNotification userListConfigPost = new CreatedNotificationController.VisibilityCreateNotification();

            userListConfigPost = BancoNotification.returnFiltersNotification(inputModel.NOTIFICATION_ID);

            BancoNotification.InsertNotification(inputModel.NOTIFICATION_ID, userListConfigPost);

            return Ok("Notificação enviado com sucesso.");

        }
        // Método para serializar um DataTable em JSON

        public class BancoNotification
        {
            #region Banco

            public static CreatedNotificationController.VisibilityCreateNotification returnFiltersNotification(int idNotification)
            {
                CreatedNotificationController.VisibilityCreateNotification userListConfigPost = new CreatedNotificationController.VisibilityCreateNotification();

                try
                {
                    StringBuilder stb = new StringBuilder();
                    stb.Append("SELECT ");
                    stb.Append("(SELECT  ");
                    stb.Append("STRING_AGG(ID_REFERER, ';') AS SECTORS ");
                    stb.Append("FROM GDA_NOTIFICATION_SEND_FILTER (NOLOCK) ");
                    stb.Append($"WHERE IDGDA_NOTIFICATION = {idNotification} AND IDGDA_PERSONA_POSTS_VISIBILITY_TYPE = 1) AS SECTORS, ");
                    stb.Append("(SELECT  ");
                    stb.Append("STRING_AGG(ID_REFERER, ';') AS SUBSECTORS ");
                    stb.Append("FROM GDA_NOTIFICATION_SEND_FILTER (NOLOCK) ");
                    stb.Append($"WHERE IDGDA_NOTIFICATION =  {idNotification} AND IDGDA_PERSONA_POSTS_VISIBILITY_TYPE = 2) AS SUBSECTORS, ");
                    stb.Append("(SELECT  ");
                    stb.Append("STRING_AGG(ID_REFERER, ';') AS PERIODS ");
                    stb.Append("FROM GDA_NOTIFICATION_SEND_FILTER (NOLOCK) ");
                    stb.Append($"WHERE IDGDA_NOTIFICATION =  {idNotification} AND IDGDA_PERSONA_POSTS_VISIBILITY_TYPE = 3) AS PERIODS, ");
                    stb.Append("(SELECT  ");
                    stb.Append("STRING_AGG(ID_REFERER, ';') AS HIERARCHYS ");
                    stb.Append("FROM GDA_NOTIFICATION_SEND_FILTER (NOLOCK) ");
                    stb.Append($"WHERE IDGDA_NOTIFICATION =  {idNotification} AND IDGDA_PERSONA_POSTS_VISIBILITY_TYPE = 4) AS HIERARCHYS, ");
                    stb.Append("(SELECT  ");
                    stb.Append("STRING_AGG(ID_REFERER, ';') AS GROUPS ");
                    stb.Append("FROM GDA_NOTIFICATION_SEND_FILTER (NOLOCK) ");
                    stb.Append($"WHERE IDGDA_NOTIFICATION =  {idNotification} AND IDGDA_PERSONA_POSTS_VISIBILITY_TYPE = 5) AS GROUPS, ");
                    stb.Append("(SELECT  ");
                    stb.Append("STRING_AGG(ID_REFERER, ';') AS USERIDS ");
                    stb.Append("FROM GDA_NOTIFICATION_SEND_FILTER (NOLOCK) ");
                    stb.Append($"WHERE IDGDA_NOTIFICATION =  {idNotification} AND IDGDA_PERSONA_POSTS_VISIBILITY_TYPE = 6) AS USERIDS, ");
                    stb.Append("(SELECT  ");
                    stb.Append("STRING_AGG(ID_REFERER, ';') AS CLIENTS ");
                    stb.Append("FROM GDA_NOTIFICATION_SEND_FILTER (NOLOCK) ");
                    stb.Append($"WHERE IDGDA_NOTIFICATION =  {idNotification} AND IDGDA_PERSONA_POSTS_VISIBILITY_TYPE = 7) AS CLIENTS, ");
                    stb.Append("(SELECT  ");
                    stb.Append("STRING_AGG(ID_REFERER, ';') AS HOMEORFLOORS ");
                    stb.Append("FROM GDA_NOTIFICATION_SEND_FILTER (NOLOCK) ");
                    stb.Append($"WHERE IDGDA_NOTIFICATION =  {idNotification} AND IDGDA_PERSONA_POSTS_VISIBILITY_TYPE = 8) AS HOMEORFLOORS ");

                    using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                    {
                        connection.Open();
                        using (SqlCommand commandSelect = new SqlCommand(stb.ToString(), connection))
                        {
                            using (SqlDataReader reader = commandSelect.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    userListConfigPost.sector = reader["SECTORS"] != DBNull.Value ? reader["SECTORS"].ToString().Split(";").Select(int.Parse).ToList() : null; 
                                    userListConfigPost.subSector = reader["SUBSECTORS"] != DBNull.Value ? reader["SUBSECTORS"].ToString().Split(";").Select(int.Parse).ToList() : null;
                                    userListConfigPost.period = reader["PERIODS"] != DBNull.Value ? reader["PERIODS"].ToString().Split(";").Select(int.Parse).ToList() : null; 
                                    userListConfigPost.hierarchy = reader["HIERARCHYS"] != DBNull.Value ? reader["HIERARCHYS"].ToString().Split(";").Select(int.Parse).ToList() : null; 
                                    userListConfigPost.group = reader["GROUPS"] != DBNull.Value ? reader["GROUPS"].ToString().Split(";").Select(int.Parse).ToList() : null; 
                                    userListConfigPost.userId = reader["USERIDS"] != DBNull.Value ? reader["USERIDS"].ToString().Split(";").Select(int.Parse).ToList() : null; 
                                    userListConfigPost.client = reader["CLIENTS"] != DBNull.Value ? reader["CLIENTS"].ToString().Split(";").Select(int.Parse).ToList() : null; 
                                    userListConfigPost.homeOrFloor = reader["HOMEORFLOORS"] != DBNull.Value ? reader["HOMEORFLOORS"].ToString().Split(";").Select(int.Parse).ToList() : null; 

                                }
                            }
                        }
                        connection.Close();
                    }
                }
                catch (Exception)
                {

                }

                return userListConfigPost;
            }


            public static void InsertNotification(int NOTIFICATION_ID, CreatedNotificationController.VisibilityCreateNotification filterNotification)
            {
                List<PersonaNotification> listPersona = new List<PersonaNotification>();
                string filter = "";
                string sectors = filterNotification.sector == null ? "" : string.Join(",", filterNotification.sector);
                string subSectors = filterNotification.subSector == null ? "" : string.Join(",", filterNotification.subSector);
                string periods = filterNotification.period == null ? "" : string.Join(",", filterNotification.period);
                string hierarchys = filterNotification.hierarchy == null ? "" : string.Join(",", filterNotification.hierarchy);
                string groups = filterNotification.group == null ? "" : string.Join(",", filterNotification.group);
                string userIds = filterNotification.userId == null ? "" : string.Join(",", filterNotification.userId);
                string clients = filterNotification.client == null ? "" : string.Join(",", filterNotification.client);
                string homeOrFloors = filterNotification.homeOrFloor == null ? "" : string.Join(",", filterNotification.homeOrFloor);
                homeOrFloors = homeOrFloors.Replace("1", "'SIM'").Replace("2", "'NÃO'");

                filter += sectors != "" ? $" AND IDGDA_SECTOR IN ({sectors}) " : "";
                filter += subSectors != "" ? $" AND IDGDA_SUBSECTOR IN ({subSectors}) " : "";
                filter += periods != "" ? $" AND P.IDGDA_PERIOD IN ({periods}) " : "";
                filter += hierarchys != "" ? $" AND H.IDGDA_HIERARCHY IN ({hierarchys}) " : "";
                //filter += groups != "" ? $" AND IDGDA_SECTOR IN ({sectors}) " : "";
                filter += userIds != "" ? $" AND PU.IDGDA_PERSONA_USER IN ({userIds}) " : "";
                filter += clients != "" ? $" AND C.IDGDA_CLIENT IN ({clients}) " : "";
                filter += homeOrFloors != "" ? $" AND CD.HOME_BASED IN ({homeOrFloors}) " : "";

                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        StringBuilder stb = new StringBuilder();
                        string filterSector = "";
                        if (sectors != "")
                        {
                            filterSector = " AND (IDGDA_SECTOR IN ('" + sectors + "') OR IDGDA_SUBSECTOR IN ('" + sectors + "')) ";
                        }
                        stb.AppendFormat("SELECT DISTINCT PU.IDGDA_PERSONA_USER FROM GDA_COLLABORATORS_DETAILS (NOLOCK) CD ");
                        stb.AppendFormat("LEFT JOIN GDA_PERIOD (NOLOCK) P ON P.PERIOD = CD.PERIODO ");
                        stb.AppendFormat("LEFT JOIN GDA_CLIENT (NOLOCK) C ON C.IDGDA_CLIENT = CD.IDGDA_CLIENT ");
                        stb.AppendFormat("LEFT JOIN GDA_HIERARCHY (NOLOCK) H ON H.LEVELNAME = CD.CARGO ");
                        stb.AppendFormat("INNER JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) PCU ON PCU.IDGDA_COLLABORATORS = CD.IDGDA_COLLABORATORS ");
                        stb.AppendFormat("INNER JOIN GDA_PERSONA_USER (NOLOCK) PU ON PU.IDGDA_PERSONA_USER = PCU.IDGDA_PERSONA_USER AND PU.IDGDA_PERSONA_USER_TYPE = 1 AND PU.DELETED_AT IS NULL ");
                        stb.AppendFormat("LEFT JOIN GDA_LOGIN_ACCESS (NOLOCK) LA ON LA.IDGDA_COLLABORATOR = PCU.IDGDA_COLLABORATORS ");
                        stb.AppendFormat("WHERE 1=1 ");
                        //stb.AppendFormat("AND CD.ACTIVE = 'true' ");
                        stb.AppendFormat("{0}  ", filter);
                        stb.AppendFormat("GROUP BY PU.IDGDA_PERSONA_USER  ");


                        using (SqlCommand commandSelect = new SqlCommand(stb.ToString(), connection))
                        {
                            using (SqlDataReader reader = commandSelect.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    PersonaNotification userid = new PersonaNotification();
                                    userid.idUserReceived = Convert.ToInt32(reader["IDGDA_PERSONA_USER"]);

                                    listPersona.Add(userid);
                                }
                            }
                        }

                        stb.Clear();
                        stb.Append("UPDATE GDA_NOTIFICATION SET ");
                        stb.Append("SENDED_AT = GETDATE() ");
                        stb.Append($"WHERE IDGDA_NOTIFICATION = {NOTIFICATION_ID} ");
                        using (SqlCommand commandSelect = new SqlCommand(stb.ToString(), connection))
                        {
                            commandSelect.ExecuteNonQuery();
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }

                //Agrupamento
                List<infsNotification> infNot = getInfsNotification(NOTIFICATION_ID);

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

                if (listPersona.Count > 0 && infNot.Count > 0)
                {
                    foreach (var personaNotification in listPersona)
                    {

                        //Inserir No Banco
                        int sendId = InsertNotificationForUser(NOTIFICATION_ID, personaNotification.idUserReceived);

                        //Envia Notificação
                        messageReturned msgInput = new messageReturned();
                        msgInput.type = "Notification";
                        msgInput.data = new dataMessage();
                        msgInput.data.idUserReceive = personaNotification.idUserReceived;
                        msgInput.data.idNotificationUser = sendId;
                        msgInput.data.idNotification = Convert.ToInt32(NOTIFICATION_ID);
                        msgInput.data.idUserSend =  infNot2.First().idUserSend;
                        msgInput.data.urlUserSend = infNot2.First().urlUserSend;
                        msgInput.data.nameUserSend = infNot2.First().nameUserSend;
                        msgInput.data.message = infNot2.First().message;
                        msgInput.data.urlFilesMessage = infNot2.First().files;

                        Startup.messageQueue.Enqueue(msgInput);
                    }
                }
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
                            stb.AppendFormat("LEFT JOIN GDA_PERSONA_USER (NOLOCK) P ON P.IDGDA_PERSONA_USER = CU.IDGDA_PERSONA_USER AND P.IDGDA_PERSONA_USER_TYPE = 1 AND P.DELETED_AT IS NULL  ");
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
                                        ifn.idUserSend = reader["CODBY"] != DBNull.Value ? Convert.ToInt32(reader["CODBY"].ToString()) : 0;
                                        ifn.urlUserSend = reader["PICTURE"] != DBNull.Value ? reader["PICTURE"].ToString() : "";
                                        ifn.nameUserSend = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "";
                                        ifn.message = reader["NOTIFICATION"] != DBNull.Value ? reader["NOTIFICATION"].ToString() : "";
                                        ifn.file = reader["LINK_FILE"] != DBNull.Value ? reader["LINK_FILE"].ToString() : "";
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

            #endregion
        }
    }
}