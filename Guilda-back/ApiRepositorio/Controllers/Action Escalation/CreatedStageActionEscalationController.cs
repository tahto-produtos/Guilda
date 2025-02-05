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
    public class CreatedStageActionEscalationController : ApiController
    {// POST: api/Results

        public class InputModelCreatedStageActionEscalation
        {
            public int IDGDA_ESCALATION_ACTION { get; set; }
            public int AUTOMATIC { get; set; }
            public List<int> IDGDA_HIERARCHY { get; set; }
            public int NUMBER_STAGE { get; set; }
        }

        [HttpDelete]
        public IHttpActionResult DeleteResultsModel(int IDGDA_ESCALATION_ACTION_STAGE, int AUTOMATIC)
        {
            int collaboratorId = 0;
            int personaid = 0;
            int idActionEscalationStage = 0;
            string retorno = "";
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            collaboratorId = inf.collaboratorId;
            personaid = inf.personauserId;

            BancoCreatedStageActionEscalation.RemoveActionEscalationStage(IDGDA_ESCALATION_ACTION_STAGE, AUTOMATIC, personaid);
            retorno = "Stage deletado com sucesso.";

            return Ok(retorno);
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModelCreatedStageActionEscalation inputModel)
        {
            int collaboratorId = 0;
            int personaid = 0;
            int idActionEscalationStage = 0;
            string retorno = "";
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            collaboratorId = inf.collaboratorId;
            personaid = inf.personauserId;

            BancoCreatedStageActionEscalation.InsertCreatedStageActionEscalation(inputModel, personaid);
            retorno = "Stage criado com sucesso.";

            return Ok(retorno);

        }
        // Método para serializar um DataTable em JSON


        #region Banco

        public class BancoCreatedStageActionEscalation
        {
            public static void InsertCreatedStageActionEscalation(InputModelCreatedStageActionEscalation inputModel, int personaid)
            {
                foreach (int idHierarchy in inputModel.IDGDA_HIERARCHY)
                {
                    string nomeTable = "";
                    if (inputModel.AUTOMATIC == 1)
                    {
                        nomeTable = "IDGDA_ESCALATION_AUTOMATIC_SECTORS";
                    }
                    else
                    {
                        nomeTable = "IDGDA_ESCALATION_ACTION";
                    }


                    int idStage = 0;
                    #region INSERT GDA_ESCALATION_ACTION_STAGE
                    StringBuilder sb = new StringBuilder();
                    sb.Append("INSERT INTO GDA_ESCALATION_ACTION_STAGE  ");
                    sb.Append($"({nomeTable}, IDGDA_HIERARCHY, NUMBER_STAGE)  ");
                    sb.Append("VALUES  ");
                    sb.Append($"('{inputModel.IDGDA_ESCALATION_ACTION}', '{idHierarchy}', '{inputModel.NUMBER_STAGE}')  ");
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
            public static void InsertHistoryActionEscalationStage(int idAction, int? idActionStage, int personaId)
            {

                //string nomeTable = "";
                //if (inputModel.AUTOMATIC == 1)
                //{
                //    nomeTable = "IDGDA_ESCALATION_AUTOMATIC_SECTORS";
                //}
                //else
                //{
                string nomeTable = "IDGDA_ESCALATION_ACTION";
                //}

                #region INSERT GDA_ESCALATION_HISTORY_ACTION_REALIZE
                StringBuilder sb = new StringBuilder();
                sb.Append("INSERT INTO GDA_ESCALATION_HISTORY_STAGE  ");
                sb.Append($"({nomeTable}, IDGDA_ESCALATION_ACTION_STAGE, CREATED_AT, CREATED_BY) ");
                sb.Append("VALUES  ");
                sb.Append($"('{idAction}', '{idActionStage}', GETDATE(), {personaId})  ");
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

            public static void RemoveActionEscalationStage(int IDGDA_ESCALATION_ACTION_STAGE, int AUTOMATIC, int personaId)
            {
                //string nomeTable = "";
                //if (AUTOMATIC == 1)
                //{
                //    nomeTable = "IDGDA_ESCALATION_AUTOMATIC_SECTORS";
                //}
                //else
                //{
                //    nomeTable = "IDGDA_ESCALATION_ACTION";
                //}

                #region REMOVE GDA_ESCALATION_ACTION_STAGE
                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE GDA_ESCALATION_ACTION_STAGE SET  ");
                sb.Append("DELETED_AT = GETDATE(), ");
                sb.Append($"DELETED_BY = {personaId} ");
                sb.Append($"WHERE IDGDA_ESCALATION_ACTION_STAGE = {IDGDA_ESCALATION_ACTION_STAGE} ");
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

        }
        #endregion

    }



}