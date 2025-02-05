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
    public class CreatedOperationalCampaignController : ApiController
    {// POST: api/Results

        public class InputModelCreatedOperationalCampaign
        {
            public string NAME_CAMPAIGN { get; set; }
            public string DESCRIPTION { get; set; }
            public string PICTURE { get; set; }
            public string STARTED_AT { get; set; }
            public string ENDED_AT { get; set; }
            public CreatedOperationalCampaignVisibility VISIBILITY { get; set; }
            //public List<int> OFFICES { get; set; }
            public List<CreatedOperationalCampaignPontuation> PONTUATION { get; set; }
            public CreatedOperationalCampaignRanking RANKING { get; set; }
            public List<CreatedOperationalCampaignElimination> ELIMINATION { get; set; }

        }
        public class CreatedOperationalCampaignVisibility
        {
            public List<int> SECTOR { get; set; }
            public List<int> SUBSECTOR { get; set; }
            public List<int> HIERARCHY { get; set; }
            public List<int> HOMEORFLOOR { get; set; }
            public List<int> GROUP { get; set; }
            public List<int> VETERANONOVADO { get; set; }

        }


        public class CreatedOperationalCampaignPontuation
        {
            public int ID_INDICATOR { get; set; }
            public int INDICATOR_INCREASE { get; set; } //
            public double PERCENT { get; set; }
            public int REWARD_POINTS { get; set; }
        }

        public class CreatedOperationalCampaignRanking
        {
            public int ID_RANKING_AWARD_PUBLIC { get; set; } // Sendo 1 = Primeiras colocações || 2 = Itens especificos
            public int ID_RANKING_AWARD_TYPE { get; set; } // Sendo 1 = Moedas Virtuais || 2 = Pontuação minima
            public int ID_RANKING_PAY_OPTION { get; set; } // Sendi 1 = Automatico || 2 = Manual
            public List<CreatedOperationalCampaignRankingItens> RANKING_ITENS { get; set; }

        }

        public class CreatedOperationalCampaignRankingItens
        {
            public int ID_PRODUCT { get; set; }
            public int POSITION { get; set; }
            public int MIN_PONTUATION { get; set; }
            public int VALUE_COINS { get; set; }
            public int QUANTITY_PRODUCT { get; set; }
        }

        public class CreatedOperationalCampaignElimination
        {
            public int ID_INDICATOR { get; set; }
            public int INDICATOR_INCREASE { get; set; } //Sendo 1 = Aumente.. 0 = Baixe
            public double PERCENT { get; set; }
        }

        public class ReturnOperationalCampaign
        {
            public int ID_OPERATIONAL_CAMPAIGN { get; set; }
        }


        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModelCreatedOperationalCampaign inputModel)
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
            int idOper = BancoCreatedOperationalCampaign.InsertCreatedOperationalCampaign(personaid, inputModel);

            //Insere Visibilidade
            BancoCreatedOperationalCampaign.insertVisibility(idOper, inputModel);

            ReturnOperationalCampaign rtn = new ReturnOperationalCampaign();
            rtn.ID_OPERATIONAL_CAMPAIGN = idOper;

            if (idOper != 0)
            {
                return Ok(rtn);
            }
            else
            {
                return BadRequest("Não foi possivel realizar a criação da campanha operacional.");
            }
        }
        // Método para serializar um DataTable em JSON


        #region Banco

        public class BancoCreatedOperationalCampaign
        {
            public static void inserVisibilityItem(int codCampaign, int visibilityTipe, int idReferer)
            {
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {

                        StringBuilder sbInsert = new StringBuilder();
                        sbInsert.AppendFormat("INSERT INTO GDA_OPERATIONAL_CAMPAIGN_VISIBILITY (IDGDA_OPERATIONAL_CAMPAIGN, IDGDA_PERSONA_POSTS_VISIBILITY_TYPE, ID_REFERER) ");
                        sbInsert.AppendFormat("VALUES ( ");
                        sbInsert.AppendFormat("{0}, ", codCampaign); //IDGDA_OPERATIONAL_CAMPAIGN
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

            public static void insertVisibility(int codCampanha, InputModelCreatedOperationalCampaign inputCampaign)
            {
                //Sector
                if (inputCampaign.VISIBILITY.SECTOR.Count > 0)
                {
                    foreach (int item in inputCampaign.VISIBILITY.SECTOR)
                    {
                        inserVisibilityItem(codCampanha, 1, item);
                    }
                }
                //SubSector
                if (inputCampaign.VISIBILITY.SUBSECTOR.Count > 0)
                {
                    foreach (int item in inputCampaign.VISIBILITY.SUBSECTOR)
                    {
                        inserVisibilityItem(codCampanha, 2, item);
                    }
                }
               
                //Hierarchy
                if (inputCampaign.VISIBILITY.HIERARCHY.Count > 0)
                {
                    foreach (int item in inputCampaign.VISIBILITY.HIERARCHY)
                    {
                        inserVisibilityItem(codCampanha, 4, item);
                    }
                }
                //Group
                if (inputCampaign.VISIBILITY.GROUP.Count > 0)
                {
                    foreach (int item in inputCampaign.VISIBILITY.GROUP)
                    {
                        inserVisibilityItem(codCampanha, 5, item);
                    }
                }
                
              
                //HomeOrFloor
                if (inputCampaign.VISIBILITY.HOMEORFLOOR.Count > 0)
                {
                    foreach (int item in inputCampaign.VISIBILITY.HOMEORFLOOR)
                    {
                        inserVisibilityItem(codCampanha, 8, item);
                    }
                }

                //VeteranoNovato
                if (inputCampaign.VISIBILITY.HOMEORFLOOR.Count > 0)
                {
                    foreach (int item in inputCampaign.VISIBILITY.VETERANONOVADO)
                    {
                        inserVisibilityItem(codCampanha, 10, item);
                    }
                }

            }




            public static int InsertCreatedOperationalCampaign(int personaId, InputModelCreatedOperationalCampaign inputModel)
            {
                //Insere a campanha
                int idgda_operational_campaign = 0;

                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();

                    string picture = inputModel.PICTURE != null ? inputModel.PICTURE : "NULL";
                    try
                    {
                        //Criação da campanha
                        StringBuilder sb = new StringBuilder();
                        sb.Append($"INSERT INTO GDA_OPERATIONAL_CAMPAIGN (NAME, DESCRIPTION, IMAGE, CREATED_AT, CREATED_BY, ENDED_AT, STARTED_AT) VALUES ( ");
                        sb.Append($" '{inputModel.NAME_CAMPAIGN}', "); //NAME
                        sb.Append($" '{inputModel.DESCRIPTION}', "); //DESCRIPTION
                        sb.Append($" {picture}, "); //IMAGE
                        sb.Append($" GETDATE(), "); //CREATED_AT
                        sb.Append($" {personaId}, "); //CREATED_AT
                        sb.Append($" '{inputModel.ENDED_AT}',"); //ENDED_AT
                        sb.Append($" '{inputModel.STARTED_AT}' "); //STARTED_AT
                        sb.Append($") SELECT @@IDENTITY AS 'IDGDA_OPERATIONAL_CAMPAIGN' ");
                        using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    idgda_operational_campaign = int.Parse(reader["IDGDA_OPERATIONAL_CAMPAIGN"].ToString());
                                }
                            }
                        }

                        //Criação Pontuação
                        foreach (CreatedOperationalCampaignPontuation item in inputModel.PONTUATION)
                        {
                            sb.Clear();
                            sb.Append($"INSERT INTO GDA_OPERATIONAL_CAMPAIGN_PONTUATION (IDGDA_OPERATIONAL_CAMPAIGN, IDGDA_INDICATOR, INDICATOR_INCREASE, [PERCENT], REWARD_POINTS) VALUES ( ");
                            sb.Append($"{idgda_operational_campaign}, "); //IDGDA_OPERATIONAL_CAMPAIGN
                            sb.Append($"{item.ID_INDICATOR}, "); //IDGDA_INDICATOR
                            sb.Append($"{item.INDICATOR_INCREASE}, "); //INDICATOR_INCREASE - //Sendo 1 = Aumente.. 0 = Baixe
                            sb.Append($"'{item.PERCENT.ToString().Replace(",",".")}', "); //PERCENT
                            sb.Append($"{item.REWARD_POINTS} "); //REWARD_POINTS
                            sb.Append($") ");

                            using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                            {
                                command.ExecuteNonQuery();
                            }
                        }

                        //Criação Ranking
                        foreach (CreatedOperationalCampaignRankingItens item in inputModel.RANKING.RANKING_ITENS)
                        {
                            sb.Clear();
                            sb.Append($"INSERT INTO GDA_OPERATIONAL_CAMPAIGN_AWARD (IDGDA_OPERATIONAL_CAMPAIGN, IDGDA_OPERATIONAL_CAMPAIGN_AWARD_PUBLIC, IDGDA_OPERATIONAL_CAMPAIGN_AWARD_TYPE, IDGDA_PRODUCT, ");
                            sb.Append($"IDGDA_OPERATIONAL_CAMPAIGN_AWARD_OPTION_PAY, RANKING, MIN_PONTUATION, VALUE_COINS, QUANTITY_PRODUCT) VALUES (  ");
                            sb.Append($"{idgda_operational_campaign}, "); //IDGDA_OPERATIONAL_CAMPAIGN
                            sb.Append($"{inputModel.RANKING.ID_RANKING_AWARD_PUBLIC}, "); //IDGDA_OPERATIONAL_CAMPAIGN_AWARD_PUBLIC - // Sendo 1 = Primeiras colocações || 2 = Pontuação minima
                            sb.Append($"{inputModel.RANKING.ID_RANKING_AWARD_TYPE}, "); //IDGDA_OPERATIONAL_CAMPAIGN_AWARD_TYPE - // Sendo 1 = Moedas Virtuais || 2 = Itens especificos
                            sb.Append($"{item.ID_PRODUCT}, "); //IDGDA_PRODUCT
                            sb.Append($"{inputModel.RANKING.ID_RANKING_PAY_OPTION}, "); //IDGDA_OPERATIONAL_CAMPAIGN_AWARD_OPTION_PAY // Sendi 1 = Automatico || 2 = Manual
                            sb.Append($"{item.POSITION}, "); //RANKING
                            sb.Append($"{item.MIN_PONTUATION}, "); //MIN_PONTUATION
                            sb.Append($"{item.VALUE_COINS}, "); //VALUE_COINS
                            sb.Append($"{item.QUANTITY_PRODUCT} "); //QUANTITY_PRODUCT
                            sb.Append($") ");

                            using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                            {
                                command.ExecuteNonQuery();
                            }
                        }

                        //Criação Elimination
                        foreach (CreatedOperationalCampaignElimination item in inputModel.ELIMINATION)
                        {
                            sb.Clear();
                            sb.Append($"INSERT INTO GDA_OPERATIONAL_CAMPAIGN_ELIMINATION (IDGDA_OPERATIONAL_CAMPAIGN, IDGDA_INDICATOR, [PERCENT], INDICATOR_INCREASE) VALUES ( ");
                            sb.Append($"{idgda_operational_campaign}, "); //IDGDA_OPERATIONAL_CAMPAIGN
                            sb.Append($"{item.ID_INDICATOR}, "); //IDGDA_INDICATOR
                            sb.Append($"{item.PERCENT.ToString().Replace(",", ".")}, "); //[PERCENT]
                            sb.Append($"{item.INDICATOR_INCREASE} "); //INDICATOR_INCREASE
                            sb.Append($") ");

                            using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                            {
                                command.ExecuteNonQuery();
                            }
                        }

                    }
                    catch (Exception ex)
                    {

                    }

                    connection.Close();
                }

                return idgda_operational_campaign;
            }
        }
        #endregion

    }



}