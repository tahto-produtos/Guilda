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
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class CreatedAutomaticActionEscalationController : ApiController
    {// POST: api/Results

        public class InputModelCreatedAutomaticActionEscalation
        {
            public int IDGDA_INDICATOR { get; set; }
            public int IDGDA_ESCALATION_ACTION_STAGE { get; set; }
            public List<int> SECTOR {  get; set; }
            public int PERIOD { get; set; }
            public int GROUP { get; set; }    
            public int PERCENTAGE_DETOUR { get; set; }
            public int TOLERANCE_RANGE { get; set; }
        }

        public class VerificaActionEscalation
        {
            public int IDGDA_ESCALATION_ACTION { get; set; }
            public int IDGDA_INDICATOR { get; set; }
            public int IDGDA_SECTOR { get; set; }
            public string STARTED_AT { get; set; }
            public string ENDED_AT { get; set; }

        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModelCreatedAutomaticActionEscalation inputModel)
        {
            int collaboratorId = 0;
            int personaid = 0;
            var token = Request.Headers.Authorization?.Parameter;
            bool tokn = TokenService.TryDecodeToken(token);
            if (tokn == false)
            {
                return Unauthorized();
            }
            //CRIAR FEEDBACK
            collaboratorId = TokenService.InfsUsers.collaboratorId;
            personaid = TokenService.InfsUsers.personauserId;

            //INSERÇÃO DO FEEDBACK AO USUARIO DESTINADO
            int idEscalationAutomaticVerify = BancoCreatedAutomaticActionEscalation.InsertEscalationAutomaticVerify(inputModel);

            foreach (int sector in inputModel.SECTOR) 
            {
                BancoCreatedAutomaticActionEscalation.InsertEscalationAutomaticSector(idEscalationAutomaticVerify, sector, inputModel);

            }
            return Ok("Configuração Automatica criada com sucesso.");
            
        }
        // Método para serializar um DataTable em JSON


        #region Banco

        public class BancoCreatedAutomaticActionEscalation
        { 
            public static int InsertEscalationAutomaticVerify(InputModelCreatedAutomaticActionEscalation inputModel)
            {
                int retorno = 0;

                #region INSERT GDA_ESCALATION_AUTOMATIC_VERIFY
                StringBuilder sb = new StringBuilder();
                sb.Append("INSERT INTO GDA_ESCALATION_AUTOMATIC_VERIFY  ");
                sb.Append("(IDGDA_INDICATOR, IDGDA_ESCALATION_ACTION_STAGE)  ");
                sb.Append("VALUES  ");
                sb.Append($"('{inputModel.IDGDA_INDICATOR}', '{inputModel.IDGDA_ESCALATION_ACTION_STAGE}')  ");
                sb.Append("SELECT  @@IDENTITY AS 'IDGDA_ESCALATION_AUTOMATIC_VERIFY' ");
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
                                    retorno = Convert.ToInt32(reader["IDGDA_ESCALATION_AUTOMATIC_VERIFY"].ToString());
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

            public static void InsertEscalationAutomaticSector(int idEscalationAutomaticVerify, int sector,InputModelCreatedAutomaticActionEscalation inputModel)
            {
                #region INSERT GDA_ESCALATION_AUTOMATIC_SECTOR
                StringBuilder sb = new StringBuilder();
                sb.Append("INSERT INTO GDA_ESCALATION_AUTOMATIC_SECTORS  ");
                sb.Append("(IDGDA_ESCALATION_AUTOMATIC_VERIFY, IDGDA_SECTOR, IDGDA_PERIOD, IDGDA_GROUP, PERCENTAGE_DETOUR, TOLERANCE_RANGE)  ");
                sb.Append("VALUES  ");
                sb.Append($"('{idEscalationAutomaticVerify}', '{sector}', '{inputModel.PERIOD}', '{inputModel.GROUP}', '{inputModel.PERCENTAGE_DETOUR}', '{inputModel.TOLERANCE_RANGE}')  ");
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

            public static void VerificyEscalationAction()
            {
                string dtInicial = System.DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00");
                string dtFinal = System.DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 23:59:59");

                //Verifica se temos Action Vigente
                List<VerificaActionEscalation> listActionEscalation = new List<VerificaActionEscalation>();
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("SELECT  ");
                        sb.Append("IDGDA_ESCALATION_ACTION, IDGDA_INDICATOR, IDGDA_SECTOR, STARTED_AT, ENDED_AT ");
                        sb.Append("FROM GDA_ESCALATION_ACTION (NOLOCK) ");
                        sb.Append("WHERE 1=1 ");
                        sb.Append("AND CONVERT(DATE,GETDATE()) >=  CONVERT(DATE,STARTED_AT)  ");
                        sb.Append("AND CONVERT(DATE,GETDATE()) <= CONVERT(DATE,ENDED_AT) ");

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
                                    VerificaActionEscalation.STARTED_AT = reader["STARTED_AT"] != DBNull.Value ? reader["STARTED_AT"].ToString() : "";
                                    VerificaActionEscalation.ENDED_AT = reader["ENDED_AT"] != DBNull.Value ? reader["ENDED_AT"].ToString() : "";
                                    
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
                    List<ModelsEx.homeRel> rmams = new List<ModelsEx.homeRel>();
                    rmams = ReportHomeResultController.returnHomeResult(dtInicial, dtFinal, item.IDGDA_SECTOR.ToString(), item.IDGDA_INDICATOR.ToString(), "", "");
                }
         

            }
        }
        #endregion

    }



}