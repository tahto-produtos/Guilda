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
using static ApiRepositorio.Controllers.CreatedAutomaticActionEscalationController;
using static ApiRepositorio.Controllers.ScoreInputController;
using System.Threading;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class CreatedAutomaticActionEscalationController : ApiController
    {// POST: api/Results

        public class InputModelCreatedAutomaticActionEscalation
        {
            public int IDGDA_INDICATOR { get; set; }
            public int IDGDA_SECTOR { get; set; }
            public int IDGDA_SUBSECTOR { get; set; }
            public int GROUP { get; set; }
            public int PERCENTAGE_DETOUR { get; set; }
            public double TOLERANCE_RANGE { get; set; }
            public List<ModelCreatedStageActionEscalation> LIST_STAGES { get; set; }
        }

        public class ModelCreatedStageActionEscalation
        {
            public List<int> IDGDA_HIERARCHY { get; set; }
            public int NUMBER_STAGE { get; set; }
        }

        public class VerificaActionEscalation
        {
            public int IDGDA_ESCALATION_ACTION { get; set; }
            public int IDGDA_INDICATOR { get; set; }
            public int IDGDA_SECTOR { get; set; }
            public int IDGDA_SUBSECTOR { get; set; }
            public int IDGDA_ESCALATION_ACTION_STAGE { get; set; }
            public int? LAST_STAGE { get; set; }
            public int? ID_NEXT_STAGE { get; set; }
            public int? NEXT_STAGE { get; set; }
            public string DATE_START { get; set; }
            public string DATE_END { get; set; }
            public int ID_HIERARCHY { get; set; }

        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModelCreatedAutomaticActionEscalation inputModel)
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
            ////INSERÇÃO DO FEEDBACK AO USUARIO DESTINADO
            //int idEscalationAutomaticVerify = BancoCreatedAutomaticActionEscalation.InsertEscalationAutomaticVerify(inputModel);

            int cod = BancoCreatedAutomaticActionEscalation.InsertEscalationAutomaticSector(inputModel);

            //INSERÇÃO DOS STAGES DENTRO DO ESCALATION
            foreach (ModelCreatedStageActionEscalation item in inputModel.LIST_STAGES)
            {
                BancoCreatedAutomaticActionEscalation.InsertCreatedStageActionEscalation(cod, item, personaid);
            }

            return Ok(cod);

        }
        // Método para serializar um DataTable em JSON


        #region Banco

        public class BancoCreatedAutomaticActionEscalation
        {
            public static void InsertCreatedStageActionEscalation(int idEscalation, ModelCreatedStageActionEscalation inputModel, int personaid)
            {
                foreach (int idHierarchy in inputModel.IDGDA_HIERARCHY)
                {
                    string nomeTable = "";

                    nomeTable = "IDGDA_ESCALATION_AUTOMATIC_SECTORS";

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

            //public static int InsertEscalationAutomaticVerify(InputModelCreatedAutomaticActionEscalation inputModel)
            //{
            //    int retorno = 0;

            //    #region INSERT GDA_ESCALATION_AUTOMATIC_VERIFY
            //    StringBuilder sb = new StringBuilder();
            //    sb.Append("INSERT INTO GDA_ESCALATION_AUTOMATIC_VERIFY  ");
            //    sb.Append("(IDGDA_INDICATOR, IDGDA_ESCALATION_ACTION_STAGE)  ");
            //    sb.Append("VALUES  ");
            //    sb.Append($"('{inputModel.IDGDA_INDICATOR}', '{inputModel.IDGDA_ESCALATION_ACTION_STAGE}')  ");
            //    sb.Append("SELECT  @@IDENTITY AS 'IDGDA_ESCALATION_AUTOMATIC_VERIFY' ");
            //    using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            //    {
            //        try
            //        {
            //            connection.Open();
            //            using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
            //            {
            //                using (SqlDataReader reader = command.ExecuteReader())
            //                {
            //                    if (reader.Read())
            //                    {
            //                        retorno = Convert.ToInt32(reader["IDGDA_ESCALATION_AUTOMATIC_VERIFY"].ToString());
            //                    }
            //                }
            //            }
            //        }
            //        catch (Exception ex)
            //        {

            //        }
            //    }
            //    #endregion
            //    return retorno;

            //}

            public static int InsertEscalationAutomaticSector(InputModelCreatedAutomaticActionEscalation inputModel)
            {
                int retorno = 0;

                #region INSERT GDA_ESCALATION_AUTOMATIC_SECTOR
                StringBuilder sb = new StringBuilder();
                sb.Append("INSERT INTO GDA_ESCALATION_AUTOMATIC_SECTORS  ");
                sb.Append("(IDGDA_SECTOR, IDGDA_SUBSECTOR, IDGDA_INDICATOR, IDGDA_GROUP, PERCENTAGE_DETOUR, TOLERANCE_RANGE, CREATED_AT)  ");
                sb.Append("VALUES  ");
                sb.Append($"('{inputModel.IDGDA_SECTOR}', '{inputModel.IDGDA_SUBSECTOR}', '{inputModel.IDGDA_INDICATOR}', '{inputModel.GROUP}', '{inputModel.PERCENTAGE_DETOUR}', '{inputModel.TOLERANCE_RANGE.ToString().Replace(",",".")}', GETDATE())  ");
                sb.Append(" SELECT  @@IDENTITY AS 'COD' ");

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
                                    retorno = Convert.ToInt32(reader["COD"].ToString());
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

            public static void VerificyEscalationAction()
            {
                //Verifica se temos Action Vigente
                List<VerificaActionEscalation> listActionEscalation = new List<VerificaActionEscalation>();
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("SELECT GEA.IDGDA_ESCALATION_ACTION, IDGDA_INDICATOR, IDGDA_SECTOR, IDGDA_SUBSECTOR, EHS.NUMBER_STAGE AS LAST_STAGE, EAS2.NUMBER_STAGE AS NEXT_STAGE, EAS2.IDGDA_ESCALATION_ACTION_STAGE AS ID_NEXT_STAGE, GEA.STARTED_AT, GEA.ENDED_AT, EAS2.IDGDA_HIERARCHY  ");
                        sb.Append("FROM GDA_ESCALATION_ACTION (NOLOCK) GEA ");
                        sb.Append("LEFT JOIN  ");
                        sb.Append("(SELECT NUMBER_STAGE, SS1.IDGDA_ESCALATION_ACTION AS IDGDA_ESCALATION_ACTION  ");
                        sb.Append("FROM  GDA_ESCALATION_HISTORY_STAGE (NOLOCK)  AS SS1 ");
                        sb.Append("INNER JOIN GDA_ESCALATION_ACTION_STAGE (NOLOCK) AS SS2 ON DELETED_AT IS NULL AND SS2.IDGDA_ESCALATION_ACTION_STAGE = SS1.IDGDA_ESCALATION_ACTION_STAGE  ");
                        sb.Append(") AS EHS ON EHS.IDGDA_ESCALATION_ACTION = GEA.IDGDA_ESCALATION_ACTION ");
                        sb.Append("LEFT JOIN ");
                        sb.Append("(SELECT NUMBER_STAGE, IDGDA_ESCALATION_ACTION, IDGDA_ESCALATION_ACTION_STAGE, IDGDA_HIERARCHY ");
                        sb.Append("FROM  GDA_ESCALATION_ACTION_STAGE (NOLOCK) WHERE DELETED_AT IS NULL ");
                        sb.Append(") AS EAS2 ON EAS2.IDGDA_ESCALATION_ACTION = GEA.IDGDA_ESCALATION_ACTION AND  EAS2.NUMBER_STAGE = CASE WHEN EHS.NUMBER_STAGE IS NULL THEN 1 ELSE EHS.NUMBER_STAGE + 1 END ");
                        sb.Append("WHERE STARTED_AT <= GETDATE() AND ENDED_AT >= GETDATE() AND GEA.DELETED_AT IS NULL ");

                        using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    VerificaActionEscalation VerificaActionEscalation = new VerificaActionEscalation();

                                    VerificaActionEscalation.IDGDA_ESCALATION_ACTION = reader["IDGDA_ESCALATION_ACTION"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_ESCALATION_ACTION"]) : 0;
                                    VerificaActionEscalation.IDGDA_INDICATOR = reader["IDGDA_INDICATOR"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_INDICATOR"]) : 0;
                                    VerificaActionEscalation.IDGDA_SECTOR = reader["IDGDA_SECTOR"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_SECTOR"]) : 0;
                                    VerificaActionEscalation.IDGDA_SUBSECTOR = reader["IDGDA_SUBSECTOR"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_SUBSECTOR"]) : 0;
                                    // VerificaActionEscalation.IDGDA_ESCALATION_ACTION_STAGE = reader["IDGDA_ESCALATION_ACTION_STAGE"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_ESCALATION_ACTION_STAGE"]) : 0;
                                    VerificaActionEscalation.LAST_STAGE = reader["LAST_STAGE"] != DBNull.Value ? Convert.ToInt32(reader["LAST_STAGE"]) : 0;
                                    VerificaActionEscalation.ID_NEXT_STAGE = reader["ID_NEXT_STAGE"] != DBNull.Value ? Convert.ToInt32(reader["ID_NEXT_STAGE"]) : 0;
                                    VerificaActionEscalation.NEXT_STAGE = reader["NEXT_STAGE"] != DBNull.Value ? Convert.ToInt32(reader["NEXT_STAGE"]) : 0;
                                    VerificaActionEscalation.DATE_START = reader["STARTED_AT"] != DBNull.Value ? reader["STARTED_AT"].ToString() : "";
                                    VerificaActionEscalation.DATE_END = reader["ENDED_AT"] != DBNull.Value ? reader["ENDED_AT"].ToString() : "";
                                    VerificaActionEscalation.ID_HIERARCHY = reader["IDGDA_HIERARCHY"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_HIERARCHY"]) : 0;

                                    if (VerificaActionEscalation.NEXT_STAGE == 0)
                                    {
                                        continue;
                                    }

                                    listActionEscalation.Add(VerificaActionEscalation);
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }
                //


                foreach (VerificaActionEscalation item in listActionEscalation)
                {

                    int idIndicator = item.IDGDA_INDICATOR;
                    string codCollaborator = "";
                    string dtInicial = Convert.ToDateTime(item.DATE_START).ToString("yyyy-MM-dd");
                    string dtFinal = Convert.ToDateTime(item.DATE_END).ToString("yyyy-MM-dd");
                    //string dtInicial = "2024-01-01";
                    //string dtFinal = "2024-01-30";

                    string hierarchiesAsString = "";
                    string indicatorsAsString = item.IDGDA_INDICATOR.ToString();
                    //string indicatorsAsString = "728";
                    string sectorsAsString = item.IDGDA_SECTOR.ToString() == "0" ? "" : item.IDGDA_SECTOR.ToString();
                    string subSectorsAsString = item.IDGDA_SUBSECTOR.ToString() == "0" ? "" : item.IDGDA_SUBSECTOR.ToString();
                    bool? bkt = true;

                    List<ResultConsolidatedController.HomeResultConsolidated> rmams = new List<ResultConsolidatedController.HomeResultConsolidated>();
                    rmams = ResultConsolidatedController.ReturnHomeResultConsolidated(codCollaborator, dtInicial, dtFinal, hierarchiesAsString, indicatorsAsString, sectorsAsString, subSectorsAsString, "", "", "", bkt);

                    if (rmams.Count == 0)
                    {
                        continue;
                    }

                    if (idIndicator == 10000013 || idIndicator == 10000014)
                    {
                        string filterEnv = $"'{idIndicator}'";
                        List<ResultConsolidatedController.HomeResultConsolidated> listaIndicadorAcesso = new List<ResultConsolidatedController.HomeResultConsolidated>();
                        listaIndicadorAcesso = ResultConsolidatedController.ReturnHomeResultConsolidatedAccess(codCollaborator, dtInicial, dtFinal, hierarchiesAsString, sectorsAsString, subSectorsAsString, "", "", "", bkt, filterEnv);
                        rmams = rmams.Concat(listaIndicadorAcesso).ToList();
                    }

                    rmams = rmams.GroupBy(d => new { d.IDINDICADOR }).Select(item2 => new ResultConsolidatedController.HomeResultConsolidated
                    {
                        MATRICULA = item2.First().MATRICULA,
                        CARGO = item2.First().CARGO,
                        //CODGIP = item2.First().CODGIP,
                        //SETOR = item2.First().SETOR,
                        IDINDICADOR = item2.Key.IDINDICADOR,
                        INDICADOR = item2.First().INDICADOR,
                        QTD = item2.Sum(d => d.QTD),
                        META = Math.Round(item2.Sum(d => d.META) / item2.Count(), 2, MidpointRounding.AwayFromZero),
                        FACTOR0 = item2.Sum(d => d.FACTOR0),
                        FACTOR1 = item2.Sum(d => d.FACTOR1),
                        //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                        min1 = Math.Round(item2.Sum(d => d.min1) / item2.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                        min2 = Math.Round(item2.Sum(d => d.min2) / item2.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                        min3 = Math.Round(item2.Sum(d => d.min3) / item2.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                        min4 = Math.Round(item2.Sum(d => d.min4) / item2.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                        CONTA = item2.First().CONTA,
                        BETTER = item2.First().BETTER,
                        //META_MAXIMA_MOEDAS = Math.Round(item.Sum(d => d.META_MAXIMA_MOEDAS) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                        META_MAXIMA_MOEDAS = item2.Sum(d => d.QTD_MON != 0 ? Math.Round(d.QTD_MON_TOTAL / d.QTD_MON, 0, MidpointRounding.AwayFromZero) : 0),

                        SUMDIASLOGADOS = item2.Sum(d => d.SUMDIASLOGADOS),
                        SUMDIASESCALADOS = item2.Sum(d => d.SUMDIASESCALADOS),
                        SUMDIASLOGADOSESCALADOS = item2.Sum(d => d.SUMDIASLOGADOSESCALADOS),

                        //MOEDA_GANHA =  Math.Round(item.Sum(d => d.MOEDA_GANHA) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                        MOEDA_GANHA = Math.Round(item2.Sum(d => d.MOEDA_GANHA), 0, MidpointRounding.AwayFromZero),
                        TYPE = item2.First().TYPE,
                    }).ToList();


                    ResultConsolidatedController.HomeResultConsolidated consolidado = rmams[0];
                    rmams[0] = ResultConsolidatedController.DoCalculateFinal(consolidado);

                    double resultado = rmams[0].RESULTADO;
                    double meta = rmams[0].META;
                    double percentual = rmams[0].PERCENTUAL;
                    string biggerBetter = rmams[0].BETTER;

                    if (percentual < meta)
                    {
                        if (item.ID_NEXT_STAGE != null)
                        {
                            //Insere historico proximo stage
                            CreatedStageActionEscalationController.BancoCreatedStageActionEscalation.InsertHistoryActionEscalationStage(item.IDGDA_ESCALATION_ACTION, item.ID_NEXT_STAGE, 0);

                            //Pegar modelo na tabela nova de agente e hierarquia para utilizarmos na description da notificação.

                            //Verifica colaboradores que irão receber a notificação com base na hierarquia. -> ID_HIERARCHY
                            List<PersonaNotification> listPersona = new List<PersonaNotification>();
                            listPersona = Funcoes.ReturnColaboradoresAction(item.ID_HIERARCHY, item.IDGDA_SECTOR, item.IDGDA_SUBSECTOR);

                            //Envia notificação

                            foreach (PersonaNotification personauser in listPersona)
                            {
                                string Indicador = Funcoes.VerificaIndicador(item.IDGDA_INDICATOR);
                                string NomeGerente = Funcoes.VerificaNomeAgente(personauser.idUserReceived);
                                string NomeOperacao = Funcoes.VerificaSetor(item.IDGDA_SECTOR != 0 ? item.IDGDA_SECTOR : item.IDGDA_SUBSECTOR);

                                int IdNotification = CreatedActionEscalationController.BancoCreatedActionEscalation.InsertNotificationAction(item.IDGDA_SECTOR, item.ID_HIERARCHY, Indicador, NomeGerente, NomeOperacao);

                                //Agrupamento
                                List<infsNotification> infNot = SendNotificationController.BancoNotification.getInfsNotification(IdNotification);

                                List<infsNotification> infNot2 = infNot
                                .GroupBy(d => new { d.codNotification })
                              .Select(group => new infsNotification
                              {
                                  codNotification = group.Key.codNotification,
                                  idUserSend = group.First().idUserSend,
                                  urlUserSend = group.First().urlUserSend,
                                  nameUserSend = group.First().nameUserSend,
                                  message = group.First().message,
                                  file = "",
                                  files = group.Select(dd => dd.file).Distinct().Select(link => new urlFiles { url = link }).ToList() // files = group.Select(item => new filesListPosts { url = item.linkFile }).ToList()
                              })
                              .ToList();

                                //Inserir No Banco
                                int sendId = SendNotificationController.BancoNotification.InsertNotificationForUser(IdNotification, personauser.idUserReceived);

                                //Envia Notificação
                                messageReturned msgInput = new messageReturned();
                                msgInput.type = "Notification";
                                msgInput.data = new dataMessage();
                                msgInput.data.idUserReceive = personauser.idUserReceived;
                                msgInput.data.idNotificationUser = sendId;
                                msgInput.data.idNotification = Convert.ToInt32(IdNotification);
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
            }
        }
        #endregion

    }



}