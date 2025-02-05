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
using static ApiRepositorio.Controllers.LoadLibraryActionEscalationController;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ListModelNotificationActionEscalationController : ApiController
    {// POST: api/Results

        public class ReturnistModelNotificationActionEscalation
        {
            public int IGDA_NOTIFICATION_MODEL_ESCALATION { get; set; }
            public string TYPE { get; set; }
            public string DESCRIPTION { get; set; }
        }

        [HttpGet]
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

            List<ReturnistModelNotificationActionEscalation> ModelNotification = new List<ReturnistModelNotificationActionEscalation>();

            //INSERÇÃO DO FEEDBACK AO USUARIO DESTINADO
            ModelNotification = BancoModelNotificationActionEscalation.ListModelNotificationActionEscalation();


            return Ok("Ação escalação criada com sucesso.");

        }
        // Método para serializar um DataTable em JSON


        #region Banco

        public class BancoModelNotificationActionEscalation
        {
            public static List<ReturnistModelNotificationActionEscalation> ListModelNotificationActionEscalation()
            {
                List<ReturnistModelNotificationActionEscalation> retorno = new List<ReturnistModelNotificationActionEscalation>();

                StringBuilder sb = new StringBuilder();

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
                                    ReturnistModelNotificationActionEscalation ModelNotification = new ReturnistModelNotificationActionEscalation();

                                    ModelNotification.IGDA_NOTIFICATION_MODEL_ESCALATION = reader["IGDA_NOTIFICATION_MODEL_ESCALATION"] != DBNull.Value ? int.Parse(reader["IGDA_NOTIFICATION_MODEL_ESCALATION"].ToString()) : 0;
                                    ModelNotification.TYPE = reader["TYPE"] != DBNull.Value ? reader["TYPE"].ToString() : "";
                                    ModelNotification.DESCRIPTION = reader["DESCRIPTION"] != DBNull.Value ? reader["DESCRIPTION"].ToString() : "";
                                    retorno.Add(ModelNotification);
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

            public static void InsertHistoryActionEscalation(int idAction, string actionRealize, int personaId)
            {
                #region INSERT GDA_ESCALATION_HISTORY_ACTION_REALIZE
                StringBuilder sb = new StringBuilder();
                sb.Append("INSERT INTO GDA_ESCALATION_HISTORY_ACTION_REALIZE  ");
                sb.Append("(IDGDA_ESCALATION_ACTION, ACTION_REALIZE, CREATED_AT, CREATED_BY) ");
                sb.Append("VALUES  ");
                sb.Append($"('{idAction}', '{actionRealize}', GETDATE(), '{personaId}' )  ");
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
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

            public static int InsertNotificationAction(int Hierarchy, string NomeIndicador, string NomeGerente = "", string nomeOperacao = "", string CentralControle = "", string Resultado = "")
            {
                int IdNotification = 0;
                string DescriptionNotification = "";
                if (Hierarchy == 1)
                {
                    DescriptionNotification = ValidaDescriptionNotification("Agente").Replace("#nomedoIndicador", NomeIndicador).Replace("#centraldecontrole", CentralControle).Replace("#resultado", Resultado);
                }
                else
                {
                    DescriptionNotification = ValidaDescriptionNotification("Hierarquia").Replace("#nomedoGerente", NomeGerente).Replace("#nomedaOperação", nomeOperacao).Replace("#nomedoIndicador", NomeIndicador);
                }
                StringBuilder sb = new StringBuilder();
                sb.Append("INSERT INTO GDA_NOTIFICATION  ");
                sb.Append("(IDGDA_NOTIFICATION_TYPE, TITLE, NOTIFICATION, CREATED_AT, CREATED_BY, ACTIVE) ");
                sb.Append("VALUES ");
                sb.Append($"(15,'Escalação', '{DescriptionNotification}', GETDATE(), 0, 1 ) ");
                sb.Append("SELECT  @@IDENTITY AS 'IDGDA_NOTIFICATION' ");
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
                                    IdNotification = Convert.ToInt32(reader["IDGDA_NOTIFICATION"].ToString());
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                return IdNotification;
            }

            public static string ValidaDescriptionNotification(string Type)
            {
                string retorno = "";

                StringBuilder sb = new StringBuilder();

                sb.Append($"SELECT DESCRIPTION  FROM GDA_NOTIFICATION_MODEL_ESCALATION (NOLOCK) WHERE TYPE = '{Type}'  ");
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
                                    retorno = reader["DESCRIPTION"].ToString();
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
        #endregion

    }



}