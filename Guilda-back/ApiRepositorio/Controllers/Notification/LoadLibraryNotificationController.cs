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
using DocumentFormat.OpenXml.Math;
using static ApiRepositorio.Controllers.LoadLibraryNotificationController;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class LoadLibraryNotificationController : ApiController
    {// POST: api/Results
        public class ReturnLibraryNotification
        {
            public int totalpages { get; set; }
            public List<LibraryNotification> LibraryNotification { get; set; }
           
        }
        public class LibraryNotification
        {
            public int codNotification { get; set; }
            public DateTime? createdAt { get; set; }
            public DateTime? startedAt { get; set; }
            public DateTime? sendedAt { get; set; }
            public DateTime? endedAt { get; set; }
            public string title { get; set; }
            public string notification { get; set; }
            public int idCreator { get; set; }
            public string nameCreator { get; set; }
            public int active { get; set; }
            public int edit { get; set; }
            public string url { get; set; }
        }

        public class InputLibraryNotification
        {
            public DateTime? createdAtFrom { get; set; }
            public DateTime? createdAtTo { get; set; }
            public DateTime? startedAtFrom { get; set; }
            public DateTime? startedAtTo { get; set; }
            public DateTime? endedAtFrom { get; set; }
            public DateTime? endedAtTo { get; set; }
            public string title { get; set; }
            public int idCreator { get; set; }
            public string nameCreator { get; set; }
            public string word { get; set; }
            public Boolean scheduling { get; set; }
            public int limit { get; set; }
            public int page { get; set; }
            public string type { get; set; }
        }


        [HttpPost]
        public IHttpActionResult GetResultsModel([FromBody] InputLibraryNotification inputModel)
        {
            int personaId = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            personaId = inf.personauserId;

            ReturnLibraryNotification returnNotification = new ReturnLibraryNotification();

            returnNotification = BancoLibraryNotification.returnLibraryNotification(personaId, inputModel);

            return Ok(returnNotification);
        }
        // Método para serializar um DataTable em JSON


    }

    public class BancoLibraryNotification
    {

        public static string filterLibraryNotification(LoadLibraryNotificationController.InputLibraryNotification inputModel)
        {
            string ret = "";

            try
            {

                if (inputModel.createdAtFrom != (DateTime?)null)
                {
                    ret = $"{ret} AND CONVERT(DATE, N.CREATED_AT) >= '{inputModel.createdAtFrom}' AND CONVERT(DATE, N.CREATED_AT) <= '{inputModel.createdAtTo}' ";
                }
                if (inputModel.startedAtFrom != (DateTime?)null)
                {
                    ret = $"{ret} AND CONVERT(DATE, N.STARTED_AT) >= '{inputModel.startedAtFrom}' AND CONVERT(DATE, N.STARTED_AT) <= '{inputModel.startedAtTo}' ";
                }
                if (inputModel.endedAtFrom != (DateTime?)null)
                {
                    ret = $"{ret} AND CONVERT(DATE, N.ENDED_AT) >= '{inputModel.endedAtFrom}' AND CONVERT(DATE, N.ENDED_AT) <= '{inputModel.endedAtTo}' ";
                }
                if (inputModel.title != "")
                {
                    ret = $"{ret} AND N.TITLE LIKE '%{inputModel.title}%' ";
                }
                if (inputModel.idCreator != 0)
                {
                    ret = $"{ret} AND N.CREATED_BY = {inputModel.idCreator} ";
                }
                if (inputModel.nameCreator != "")
                {
                    ret = $"{ret} AND P.NAME LIKE '%{inputModel.nameCreator}%' ";
                }
                if (inputModel.word != "")
                {
                    ret = $"{ret} AND N.NOTIFICATION LIKE '%{inputModel.word}%' ";
                }
                if (inputModel.scheduling != false)
                {
                    ret = $"{ret} AND N.STARTED_AT > GETDATE() -1  AND SENDED_AT IS NULL ";
                }
                //if (inputModel.type != "")
                //{
                //    ret = $"{ret} AND N.STARTED_AT > GETDATE() ";
                //}
                if (inputModel.type != "")
                {
                    ret = $" {ret} AND (NT.TYPE LIKE '%' + @searchNotification + '%' ) ";
                }

            }
            catch (Exception)
            {

            }

            return ret;
        }

        public static LoadLibraryNotificationController.ReturnLibraryNotification returnLibraryNotification(int personauserId, LoadLibraryNotificationController.InputLibraryNotification inputModel)
        {
            ReturnLibraryNotification retorno = new ReturnLibraryNotification();
            List<LibraryNotification> listLibraryNotificationt = new List<LibraryNotification>();

            //List<LoadLibraryNotificationController.ReturnLibraryNotification> rLibraryNotification = new List<LoadLibraryNotificationController.ReturnLibraryNotification>();
            string filter = filterLibraryNotification(inputModel);

            //Listo todas as postagens e repostagens do banco
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {


                    //787450
                    //13180
                    int totalInfo = Funcoes.QuantidadeNotifications(inputModel.word, filter);
                    int totalpage = (int)Math.Ceiling((double)totalInfo / inputModel.limit);
                    int offset = (inputModel.page - 1) * inputModel.limit;

                    retorno.totalpages = totalpage;

                    

                    string dtAg = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                    StringBuilder stb = new StringBuilder();
                    stb.Append($"DECLARE @searchNotification NVARCHAR(100) = '{inputModel.type}' ");
                    stb.AppendFormat("SELECT N.IDGDA_NOTIFICATION, MAX(N.CREATED_AT) AS CREATED_AT, MAX(N.STARTED_AT) AS STARTED_AT,  MAX(N.SENDED_AT) AS SENDED_AT, ");
                    stb.AppendFormat("MAX(N.ENDED_AT) AS ENDED_AT, MAX(N.TITLE) AS TITLE, MAX(N.NOTIFICATION) AS NOTIFICATION, MAX(N.CREATED_BY) AS ID, MAX(P.NAME) AS NAME, MAX(N.ACTIVE) AS ACTIVE,  ");
                    stb.AppendFormat("MAX(NF.LINK_FILE) AS URL, ");
                    stb.AppendFormat("MAX(CASE WHEN SENDED_AT IS NULL THEN 1 ELSE 0 END) AS EDITAR ");
                    stb.AppendFormat("FROM GDA_NOTIFICATION (NOLOCK) N ");
                    stb.AppendFormat("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER CU ON CU.IDGDA_PERSONA_USER = N.CREATED_BY  ");
                    stb.AppendFormat("LEFT JOIN GDA_PERSONA_USER (NOLOCK) P ON P.IDGDA_PERSONA_USER = CU.IDGDA_PERSONA_USER AND P.IDGDA_PERSONA_USER_TYPE = 1  ");
                    stb.AppendFormat("LEFT JOIN GDA_NOTIFICATION_FILES (NOLOCK) NF ON NF.IDGDA_NOTIFICATION = N.IDGDA_NOTIFICATION ");
                    stb.AppendFormat("LEFT JOIN GDA_NOTIFICATION_TYPE (NOLOCK) NT ON NT.IDGDA_NOTIFICATION_TYPE = N.IDGDA_NOTIFICATION_TYPE ");
                    stb.AppendFormat("WHERE N.DELETED_AT IS NULL ");
                    stb.AppendFormat("AND N.IDGDA_NOTIFICATION_TYPE in (5,6,3,10,11,12,13) ");
                    stb.AppendFormat(" {0} ", filter);
                    stb.AppendFormat("GROUP BY N.IDGDA_NOTIFICATION, N.CREATED_AT ");
                    stb.AppendFormat("ORDER BY N.CREATED_AT DESC ");
                    stb.Append($"OFFSET {offset} ROWS FETCH NEXT {inputModel.limit} ROWS ONLY  ");

                    using (SqlCommand commandInsert = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                LoadLibraryNotificationController.LibraryNotification libraryNot = new LoadLibraryNotificationController.LibraryNotification();

                                libraryNot.codNotification = reader["IDGDA_NOTIFICATION"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_NOTIFICATION"]) : 0;
                                libraryNot.createdAt = reader["CREATED_AT"] != DBNull.Value ? Convert.ToDateTime(reader["CREATED_AT"]) : (DateTime?)null;
                                libraryNot.startedAt = reader["STARTED_AT"] != DBNull.Value ? Convert.ToDateTime(reader["STARTED_AT"]) : (DateTime?)null;
                                libraryNot.endedAt = reader["ENDED_AT"] != DBNull.Value ? Convert.ToDateTime(reader["ENDED_AT"]) : (DateTime?)null;
                                libraryNot.sendedAt = reader["SENDED_AT"] != DBNull.Value ? Convert.ToDateTime(reader["SENDED_AT"]) : (DateTime?)null;
                                libraryNot.title = reader["TITLE"] != DBNull.Value ? reader["TITLE"].ToString() : "";
                                libraryNot.notification = reader["NOTIFICATION"] != DBNull.Value ? reader["NOTIFICATION"].ToString() : "";
                                libraryNot.idCreator = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0;
                                libraryNot.nameCreator = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "";
                                libraryNot.active = reader["ACTIVE"] != DBNull.Value ? Convert.ToInt32(reader["ACTIVE"]) : 0;
                                libraryNot.edit = reader["EDITAR"] != DBNull.Value ? Convert.ToInt32(reader["EDITAR"]) : 0;
                                libraryNot.url = reader["URL"] != DBNull.Value ? reader["URL"].ToString() : "";

                                listLibraryNotificationt.Add(libraryNot);

                                //retorno.LibraryNotification.Add(libraryNot);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            retorno.LibraryNotification = listLibraryNotificationt;
            return retorno;
        }
    }
}