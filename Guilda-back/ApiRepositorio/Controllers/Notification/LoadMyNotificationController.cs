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
using static ApiRepositorio.Controllers.ListMonetizationExpiredController;
using DocumentFormat.OpenXml.Math;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class LoadMyNotificationController : ApiController
    {// POST: api/Results
        public class ReturnNotification
        {
            public int TOTALPAGES { get; set; }

            public List<Notifications> returnListNotifications { get; set; }
        }

        public class Notifications
        {
            public int codNotification { get; set; }
            public DateTime? dateNotification { get; set; }
            public string nameUser { get; set; }
            public string pictureUser { get; set; }
            public string hierarchyUser { get; set; }
            public string texto { get; set; }
            public string link_file { get; set; }
            public string categoria { get; set; }
            public DateTime? viewedAt { get; set; }
            public List<ReturnNotificationFiles> files { get; set; }
        }

        public class ReturnNotificationFiles
        {
            public string url { get; set; }
        }

        public class InputLoadNotification
        {
            public int limit { get; set; }
            public int page { get; set; }
            public int cod { get; set; }
            public string name { get; set; }
        }


        [HttpPost]
        public IHttpActionResult GetResultsModel([FromBody] InputLoadNotification inputModel)
        {
            int personaId = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            personaId = inf.personauserId;


            ReturnNotification returnNotification = BancoLoadNotification.returnNotification(personaId, inputModel);

            return Ok(returnNotification);
        }
        // Método para serializar um DataTable em JSON


    }

    public class BancoLoadNotification
    {
        public static LoadMyNotificationController.ReturnNotification returnNotification(int personauserId, LoadMyNotificationController.InputLoadNotification inputModel)
        {
            LoadMyNotificationController.ReturnNotification returnNotification = new LoadMyNotificationController.ReturnNotification();
            returnNotification.returnListNotifications = new List<LoadMyNotificationController.Notifications>();
            string filtro = "";

            if (inputModel.name != "")
            {
                if (inputModel.cod == 0)
                {
                    if (inputModel.name == "Lido")
                    {
                        filtro = $" AND NU.VIEWED_AT IS NOT NULL ";
                    }
                    else
                    {
                        filtro = $" AND NU.VIEWED_AT IS NULL ";
                    }
                }
                else
                {
                    filtro = $" AND NT.IDGDA_NOTIFICATION_TYPE = {inputModel.cod} ";
                }
            }
            

            int totalInfo = quantidadeNotificacao(personauserId, filtro);
            int totalpage =  (int)Math.Ceiling((double)totalInfo / inputModel.limit);

            int offset = (inputModel.page - 1) * inputModel.limit;
            returnNotification.TOTALPAGES = totalpage;


            //Listo todas as postagens e repostagens do banco
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    //787450
                    //13180

                    string dtAg = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                    StringBuilder stb = new StringBuilder();
                    stb.AppendFormat("SELECT IDGDA_NOTIFICATION_USER AS COD, MAX(P.NAME) AS NAME, MAX(NU.SENDED_AT) AS DATE_NOTIFICATION, MAX(NU.VIEWED_AT) AS VIEWED_AT,  ");
                    stb.AppendFormat("MAX(P.PICTURE) AS PICTURE, MAX(CD.CARGO) AS CARGO, MAX(N.NOTIFICATION) AS NOTIFICATION, LINK_FILE,  ");
                    stb.AppendFormat("MAX(NT.TYPE) AS TYPE ");
                    stb.AppendFormat("FROM GDA_NOTIFICATION_USER (NOLOCK) NU ");
                    stb.AppendFormat("INNER JOIN GDA_NOTIFICATION (NOLOCK) N ON NU.IDGDA_NOTIFICATION = N.IDGDA_NOTIFICATION ");
                    stb.AppendFormat("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER CU ON CU.IDGDA_PERSONA_USER = N.CREATED_BY  ");
                    stb.AppendFormat("LEFT JOIN GDA_PERSONA_USER (NOLOCK) P ON P.IDGDA_PERSONA_USER = CU.IDGDA_PERSONA_USER AND P.IDGDA_PERSONA_USER_TYPE = 1  ");
                    stb.AppendFormat("LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) CD ON CD.CREATED_AT >= '{0}' AND CD.IDGDA_COLLABORATORS = CU.IDGDA_COLLABORATORS ", dtAg);
                    stb.AppendFormat("LEFT JOIN GDA_NOTIFICATION_FILES (NOLOCK) NF ON NF.IDGDA_NOTIFICATION = N.IDGDA_NOTIFICATION ");
                    stb.AppendFormat("INNER JOIN GDA_NOTIFICATION_TYPE (NOLOCK) NT ON NT.IDGDA_NOTIFICATION_TYPE = N.IDGDA_NOTIFICATION_TYPE ");
                    stb.AppendFormat("WHERE NU.DELETED_AT IS NULL AND ");
                    stb.AppendFormat(" (N.ENDED_AT IS NULL OR GETDATE() <= N.ENDED_AT) AND ");
                    stb.AppendFormat("NU.IDGDA_PERSONA_USER = {0} ", personauserId);
                    stb.AppendFormat("AND N.DELETED_AT IS NULL ", personauserId);
                    stb.AppendFormat("AND NU.DELETED_AT IS NULL ", personauserId);
                    stb.AppendFormat(" {0} ", filtro);
                    stb.AppendFormat("GROUP BY IDGDA_NOTIFICATION_USER, LINK_FILE, NU.SENDED_AT ");
                    stb.Append($"ORDER BY NU.SENDED_AT DESC  ");
                    stb.Append($"OFFSET {offset} ROWS FETCH NEXT {inputModel.limit} ROWS ONLY");

                    using (SqlCommand commandInsert = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                LoadMyNotificationController.Notifications notification = new LoadMyNotificationController.Notifications();

                                notification.codNotification = reader["COD"] != DBNull.Value ? Convert.ToInt32(reader["COD"]) : 0;
                                notification.nameUser = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "";
                                notification.dateNotification = reader["DATE_NOTIFICATION"] != DBNull.Value ? Convert.ToDateTime(reader["DATE_NOTIFICATION"].ToString()) : (DateTime?)null;
                                notification.viewedAt = reader["VIEWED_AT"] != DBNull.Value ? Convert.ToDateTime(reader["VIEWED_AT"].ToString()) : (DateTime?)null;
                                notification.pictureUser = reader["PICTURE"] != DBNull.Value ? reader["PICTURE"].ToString() : "";
                                notification.hierarchyUser = reader["CARGO"] != DBNull.Value ? reader["CARGO"].ToString() : "";
                                notification.texto = reader["NOTIFICATION"] != DBNull.Value ? reader["NOTIFICATION"].ToString() : "";
                                notification.link_file = reader["LINK_FILE"] != DBNull.Value ? reader["LINK_FILE"].ToString() : "";
                                notification.categoria = reader["TYPE"] != DBNull.Value ? reader["TYPE"].ToString() : "";

                                returnNotification.returnListNotifications.Add(notification);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            if (returnNotification.returnListNotifications.Count > 0 || returnNotification.returnListNotifications != null)
            {
                returnNotification.returnListNotifications = returnNotification.returnListNotifications
             .GroupBy(item => new { item.codNotification })
             .Select(group => new LoadMyNotificationController.Notifications
             {
                 codNotification = group.Key.codNotification,
                 nameUser = group.First().nameUser,
                 dateNotification = group.First().dateNotification,
                 pictureUser = group.First().pictureUser,
                 hierarchyUser = group.First().hierarchyUser,
                 texto = group.First().texto,
                 link_file = "",
                 categoria = group.First().categoria,
                 viewedAt = group.First().viewedAt,
                 files = group.Any(item => !string.IsNullOrEmpty(item.link_file)) ?
               group.Select(item => item.link_file)
                    .Distinct()
                    .Select(link => new LoadMyNotificationController.ReturnNotificationFiles { url = link })
                    .ToList() :
               new List<LoadMyNotificationController.ReturnNotificationFiles>() // files = group.Select(item => item.link_file).Distinct().Select(link => new LoadMyNotificationController.ReturnNotificationFiles { url = link }).ToList() // files = group.Select(item => new filesListPosts { url = item.linkFile }).ToList()
              }).OrderByDescending(ordem => ordem.dateNotification)
             .ToList();
            }

              

            return returnNotification;
        }

        public static int quantidadeNotificacao(int personaID, string filtro)
        {
            int total = 0;

            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                connection.Open();
                try
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Append("SELECT COUNT(0) AS CONTA ");
                    sb.Append("FROM GDA_NOTIFICATION_USER (NOLOCK) NU ");
                    sb.Append("INNER JOIN GDA_NOTIFICATION (NOLOCK) N ON NU.IDGDA_NOTIFICATION = N.IDGDA_NOTIFICATION ");
                    sb.Append("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER CU ON CU.IDGDA_PERSONA_USER = N.CREATED_BY  ");
                    sb.Append("LEFT JOIN GDA_PERSONA_USER (NOLOCK) P ON P.IDGDA_PERSONA_USER = CU.IDGDA_PERSONA_USER AND P.IDGDA_PERSONA_USER_TYPE = 1  ");
                    sb.Append("LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) CD ON CD.CREATED_AT >= GETDATE()-1 AND CD.IDGDA_COLLABORATORS = CU.IDGDA_COLLABORATORS ");
                    sb.Append("LEFT JOIN GDA_NOTIFICATION_FILES (NOLOCK) NF ON NF.IDGDA_NOTIFICATION = N.IDGDA_NOTIFICATION ");
                    sb.Append("INNER JOIN GDA_NOTIFICATION_TYPE (NOLOCK) NT ON NT.IDGDA_NOTIFICATION_TYPE = N.IDGDA_NOTIFICATION_TYPE ");

                    sb.Append("WHERE NU.DELETED_AT IS NULL AND ");
                    sb.Append(" (N.ENDED_AT IS NULL OR GETDATE() <= N.ENDED_AT) ");
                    sb.Append($"AND NU.IDGDA_PERSONA_USER = {personaID} ");
                    sb.Append("AND N.DELETED_AT IS NULL   ");
                    sb.Append("AND NU.DELETED_AT IS NULL ");
                    sb.Append($" {filtro} ");


  


            
                   
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                total = reader["CONTA"] != DBNull.Value ? Convert.ToInt32(reader["CONTA"].ToString()) : 0;
                            }
                        }
                    }

                }
                catch (Exception)
                {

                }


                connection.Close();

            }
            return total;
        }

    }
}