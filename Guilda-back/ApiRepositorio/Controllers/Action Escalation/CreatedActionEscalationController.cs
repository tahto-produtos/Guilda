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
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class CreatedActionEscalationController : ApiController
    {// POST: api/Results

        public class InputModelCreatedActionEscalation
        {
            public int IDGDA_INDICATOR { get; set; }
            public int IDGDA_PERSONA_RESPONSIBLE_CREATION { get; set; }
            public int IDGDA_PERSONA_RESPONSIBLE_ACTION { get; set; }
            public int IDGDA_SECTOR { get; set; }
            public int IDGDA_SUBSECTOR { get; set; }
            public string ACTION_REALIZED { get; set; }
            public string NAME { get; set; }
            public string DESCRIPTION { get; set; }
            public string STARTED_AT { get; set; }
            public string ENDED_AT { get; set; }

            public List<ModelCreatedStageActionEscalation> LIST_STAGES { get; set; }
        }

        public class ModelCreatedStageActionEscalation
        {
            public List<int> IDGDA_HIERARCHY { get; set; }
            public int NUMBER_STAGE { get; set; }
        }


        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModelCreatedActionEscalation inputModel)
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

            //INSERÇÃO DO FEEDBACK AO USUARIO DESTINADO
            int idActionEscalation = BancoCreatedActionEscalation.InsertCreatedActionEscalation(inputModel);

            BancoCreatedActionEscalation.InsertHistoryActionEscalation(idActionEscalation, inputModel.ACTION_REALIZED, personaid);

            //INSERÇÃO DOS STAGES DENTRO DO ESCALATION
            foreach (ModelCreatedStageActionEscalation item in inputModel.LIST_STAGES)
            {
                BancoCreatedActionEscalation.InsertCreatedStageActionEscalation(idActionEscalation, item, personaid);
            }

            return Ok(idActionEscalation);

        }
        // Método para serializar um DataTable em JSON


        [HttpDelete]
        public IHttpActionResult DeleteResultsModel(int idAction, int automatic)
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

            BancoCreatedActionEscalation.DeleteActionEscalation(idAction, automatic);

            return Ok("Removido com sucesso!");

        }

        #region Banco


        public class BancoCreatedActionEscalation
        {
            public static void InsertCreatedStageActionEscalation(int idEscalation, ModelCreatedStageActionEscalation inputModel, int personaid)
            {
                foreach (int idHierarchy in inputModel.IDGDA_HIERARCHY)
                {
                    string nomeTable = "";

                    nomeTable = "IDGDA_ESCALATION_ACTION";



                    int idStage = 0;
                    #region INSERT GDA_ESCALATION_ACTION_STAGE
                    StringBuilder sb = new StringBuilder();
                    sb.Append("INSERT INTO GDA_ESCALATION_ACTION_STAGE  ");
                    sb.Append($"({nomeTable}, IDGDA_HIERARCHY, NUMBER_STAGE)  ");
                    sb.Append("VALUES  ");
                    sb.Append($"('{idEscalation}', '{idHierarchy}', '{inputModel.NUMBER_STAGE}')  ");
                    sb.Append("SELECT  @@IDENTITY AS 'IDGDA_ESCALATION_ACTION_STAGE' ");
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
                                        idStage = Convert.ToInt32(reader["IDGDA_ESCALATION_ACTION_STAGE"].ToString());
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }


                }
                #endregion
            }

            public static void DeleteActionEscalation(int idAction, int automatic)
            {
                string nomeTabela = "";
                string nomeColuna = "";
                if (automatic == 0)
                {
                    nomeTabela = "GDA_ESCALATION_ACTION";
                    nomeColuna = "IDGDA_ESCALATION_ACTION";
                }
                else
                {
                    nomeTabela = "GDA_ESCALATION_AUTOMATIC_SECTORS";
                    nomeColuna = "IDGDA_ESCALATION_AUTOMATIC_SECTORS";
                }


                StringBuilder sb = new StringBuilder();
                sb.Append($"UPDATE {nomeTabela} SET DELETED_AT = GETDATE() WHERE {nomeColuna} = {idAction} ");

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
                    connection.Close();
                }
            }
            public static int InsertCreatedActionEscalation(InputModelCreatedActionEscalation inputModel)
            {
                int retorno = 0;
                string STARTED_AT = "";
                string ENDED_AT = "";

                if (inputModel.STARTED_AT != "")
                {
                    STARTED_AT = $"'{Convert.ToDateTime(inputModel.STARTED_AT).ToString("yyyy-MM-dd")}'";
                }
                else
                {
                    STARTED_AT = $"NULL";
                }

                if (inputModel.ENDED_AT != "")
                {
                    ENDED_AT = $"'{Convert.ToDateTime(inputModel.ENDED_AT).ToString("yyyy-MM-dd")}'";
                }
                else
                {
                    ENDED_AT = $"NULL";
                }


                #region INSERT GDA_ESCALATION_ACTION 
                StringBuilder sb = new StringBuilder();
                sb.Append("INSERT INTO GDA_ESCALATION_ACTION  ");
                sb.Append("(IDGDA_INDICATOR, IDGDA_PERSONA_RESPONSIBLE, IDGDA_PERSONA_ACTION, IDGDA_SECTOR, NAME, DESCRIPTION, STARTED_AT, ENDED_AT)  ");
                sb.Append("VALUES  ");
                sb.Append($"('{inputModel.IDGDA_INDICATOR}', '{inputModel.IDGDA_PERSONA_RESPONSIBLE_CREATION}', '{inputModel.IDGDA_PERSONA_RESPONSIBLE_ACTION}', '{inputModel.IDGDA_SECTOR}', '{inputModel.NAME}', '{inputModel.DESCRIPTION}', {STARTED_AT}, {ENDED_AT})  ");
                sb.Append("SELECT  @@IDENTITY AS 'IDGDA_ESCALATION_ACTION' ");
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
                                    retorno = Convert.ToInt32(reader["IDGDA_ESCALATION_ACTION"].ToString());
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

            public static void InsertAutomaticStageActionEscalation(int idAction, int idgda_hierarchy, int NUMBER_STAGE)
            {
                #region INSERT GDA_ESCALATION_HISTORY_ACTION_REALIZE
                StringBuilder sb = new StringBuilder();
                sb.Append("INSERT INTO GDA_ESCALATION_ACTION_STAGE  ");
                sb.Append("(IDGDA_ESCALATION_ACTION, IDGDA_HIERARCHY, NUMBER_STAGE) ");
                sb.Append("VALUES  ");
                sb.Append($"('{idAction}', '{idgda_hierarchy}', '{NUMBER_STAGE}' )  ");
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
                    connection.Close();
                }

                #endregion
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
                    connection.Close();
                }

                #endregion
            }

            public static int InsertNotificationAction(int Sector, int Hierarchy, string NomeIndicador, string NomeGerente = "", string nomeOperacao = "")
            {
                int IdNotification = 0;
                string DescriptionNotification = "";
                if (Hierarchy == 1)
                {
                    DescriptionNotification = ValidaDescriptionNotification("Agente").Replace("#nomedoIndicador", NomeIndicador);
                }
                else
                {
                    DescriptionNotification = ValidaDescriptionNotification("Hierarquia").Replace("#nomedoGerente", NomeGerente).Replace("#nomedaOperação", nomeOperacao).Replace("#nomedoIndicador", NomeIndicador);
                }
                StringBuilder sb = new StringBuilder();
                sb.Append("INSERT INTO GDA_NOTIFICATION  ");
                sb.Append("(IDGDA_NOTIFICATION_TYPE, TITLE, NOTIFICATION, CREATED_AT, CREATED_BY, ACTIVE ) ");
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

                        sb.Clear();
                        sb.Append("UPDATE GDA_NOTIFICATION SET ");
                        sb.Append("SENDED_AT = GETDATE() ");
                        sb.Append($"WHERE IDGDA_NOTIFICATION = {IdNotification} ");
                        using (SqlCommand commandSelect = new SqlCommand(sb.ToString(), connection))
                        {
                            commandSelect.ExecuteNonQuery();
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
                    connection.Close();
                }

                return retorno;

            }

        }
        #endregion

    }



}