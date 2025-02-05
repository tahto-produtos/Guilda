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
using DocumentFormat.OpenXml.Drawing;
using static TokenService;

//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class DetailsInformationOperationalCampaignController : ApiController
    {// POST: api/Results

        public class ReturnDetailsOperationalCampaignInformation
        {
            public int idCampaign { get; set; }
            public string name { get; set; }
            public string image { get; set; }
            public string pontuation { get; set; }
            public string max_pontuation { get; set; }
            public int position { get; set; }
            public DateTime? dtInicio { get; set; }
            public DateTime? dtFim { get; set; }
            public int showButtonPay { get; set; }
            public int pay { get; set; }
            public List<OperationalCampaignMissions> missions { get; set; }
            public List<OperationalCampaignRanking> rankings { get; set; }

        }

        public class OperationalCampaignMissions
        {
            public string mission_type { get; set; }
            public string mission_indicator { get; set; }
            public string mission_text { get; set; }
        }

        public class OperationalCampaignRanking
        {
            public string position { get; set; }
            public string name { get; set; }
            public string status { get; set; }
            public string pontuation { get; set; }
            public string award { get; set; } //Premio
        }


        public class InputModelDetailsOperationalCampaignInformation
        {
            public int IDGDA_OPERATIONAL_CAMPAIGN { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModelDetailsOperationalCampaignInformation inputModel)
        {
            int COLLABORATORID = 0;
            int PERSONAUSERID = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            COLLABORATORID = inf.collaboratorId;
            PERSONAUSERID = inf.personauserId;

            ReturnDetailsOperationalCampaignInformation ReturnDetailsOperationalCampaign = new ReturnDetailsOperationalCampaignInformation();

            ReturnDetailsOperationalCampaign = BankDetailsOperationalCampaignInformation.returnDetailsOperationalCampaign(PERSONAUSERID, inputModel);

            return Ok(ReturnDetailsOperationalCampaign);
        }
        // Método para serializar um DataTable em JSON

        public class BankDetailsOperationalCampaignInformation
        {
            public static ReturnDetailsOperationalCampaignInformation returnDetailsOperationalCampaign(int personaId, InputModelDetailsOperationalCampaignInformation inputModel)
            {
                ReturnDetailsOperationalCampaignInformation retorno = new ReturnDetailsOperationalCampaignInformation();
                retorno.missions = new List<OperationalCampaignMissions>();
                retorno.rankings = new List<OperationalCampaignRanking>();

                StringBuilder sb = new StringBuilder();

                sb.Append("SELECT  ");
                sb.Append("MAX(OC.IDGDA_OPERATIONAL_CAMPAIGN) AS IDGDA_OPERATIONAL_CAMPAIGN, ");
                sb.Append("MAX(OC.NAME) AS NAME, ");
                sb.Append("MAX(OC.IMAGE) AS IMAGE, ");
                sb.Append("MAX(OC.STARTED_AT) AS STARTED_AT, ");
                sb.Append("MAX(OC.ENDED_AT) AS ENDED_AT, ");
                sb.Append("MAX(OCUP.POSITION) AS RANKING, ");
                sb.Append("MAX(OC.PAID) AS PAY, ");
                sb.Append("CONVERT(VARCHAR, MAX(OCUP.VALUE)) + '/' + CONVERT(VARCHAR, SUM(OCP.REWARD_POINTS)) AS PONTUACAO    ");
                sb.Append("FROM GDA_OPERATIONAL_CAMPAIGN (NOLOCK) OC ");
                sb.Append($"LEFT JOIN GDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT (NOLOCK) AS OCUP ON OCUP.IDGDA_OPERATIONAL_CAMPAIGN = OC.IDGDA_OPERATIONAL_CAMPAIGN AND OCUP.IDGDA_PERSONA = {personaId} ");
                sb.Append("LEFT JOIN GDA_OPERATIONAL_CAMPAIGN_PONTUATION (NOLOCK) AS OCP ON OCP.IDGDA_OPERATIONAL_CAMPAIGN = OC.IDGDA_OPERATIONAL_CAMPAIGN ");
                sb.Append($"WHERE OC.IDGDA_OPERATIONAL_CAMPAIGN = {inputModel.IDGDA_OPERATIONAL_CAMPAIGN} ");

                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        //Detalhes
                        using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    retorno.idCampaign = reader["IDGDA_OPERATIONAL_CAMPAIGN"] != DBNull.Value ? int.Parse(reader["IDGDA_OPERATIONAL_CAMPAIGN"].ToString()) : 0;
                                    retorno.name = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "";
                                    retorno.image = reader["IMAGE"] != DBNull.Value ? reader["IMAGE"].ToString() : "";
                                    retorno.pontuation = reader["PONTUACAO"] != DBNull.Value ? reader["PONTUACAO"].ToString() : "";
                                    retorno.position = reader["RANKING"] != DBNull.Value ? int.Parse(reader["RANKING"].ToString()) : 0;

                                    retorno.dtInicio = reader["STARTED_AT"] != DBNull.Value ? Convert.ToDateTime(reader["STARTED_AT"].ToString()) : (DateTime?)null;
                                    retorno.dtFim = reader["ENDED_AT"] != DBNull.Value ? Convert.ToDateTime(reader["ENDED_AT"].ToString()) : (DateTime?)null;
                                    retorno.pay = reader["PAY"] != DBNull.Value ? Convert.ToInt32(reader["PAY"].ToString()) : 0;

                                    if (DateTime.Now > retorno.dtFim && retorno.pay == 0)
                                    {
                                        retorno.showButtonPay = 1;
                                    }
                                    else
                                    {
                                        retorno.showButtonPay = 0;
                                    }                                    
                                }
                            }
                        }

                        //Missões
                        sb.Clear();
                        sb.Append("SELECT INDICATOR_INCREASE, [PERCENT], REWARD_POINTS, I.NAME, ");
                        sb.Append($"(SELECT SUM(REWARD_POINTS) FROM GDA_OPERATIONAL_CAMPAIGN_PONTUATION (NOLOCK) WHERE IDGDA_OPERATIONAL_CAMPAIGN = {inputModel.IDGDA_OPERATIONAL_CAMPAIGN}) AS MAX_PONTUATION ");
                        sb.Append("FROM GDA_OPERATIONAL_CAMPAIGN (NOLOCK) OC ");
                        sb.Append("INNER JOIN GDA_OPERATIONAL_CAMPAIGN_PONTUATION (NOLOCK) OCP ON OCP.IDGDA_OPERATIONAL_CAMPAIGN = OC.IDGDA_OPERATIONAL_CAMPAIGN ");
                        sb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) I ON I.IDGDA_INDICATOR = OCP.IDGDA_INDICATOR ");
                        sb.Append($"WHERE OC.IDGDA_OPERATIONAL_CAMPAIGN = {inputModel.IDGDA_OPERATIONAL_CAMPAIGN} ");

                        using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    OperationalCampaignMissions OCM = new OperationalCampaignMissions();

                                    retorno.max_pontuation = reader["MAX_PONTUATION"].ToString();
                                    OCM.mission_type = Convert.ToInt32(reader["INDICATOR_INCREASE"]) == 0 ? "Abaixe o indicador de" : "Aumente o indicador de";
                                    OCM.mission_indicator = reader["NAME"].ToString();
                                    OCM.mission_text = $"Em {reader["PERCENT"]} até o final da campanha";

                                    retorno.missions.Add(OCM);
                                }
                            }
                        }

                        //Ranking
                        sb.Clear();
                        sb.Append("SELECT  ");
                        sb.Append("TBL.IDGDA_OPERATIONAL_CAMPAIGN, TBL.IDGDA_PERSONA_USER, MAX(POSITION) AS POSITION, MAX(NAME) AS NAME, MAX(STATUS) AS STATUS, MAX(PONTUATION) AS PONTUATION,  ");
                        sb.Append("SUM(OCA.VALUE_COINS) AS VALUE_COINS ");
                        sb.Append("FROM ");
                        sb.Append("( ");
                        sb.Append("SELECT CUP.IDGDA_OPERATIONAL_CAMPAIGN, GPU.IDGDA_PERSONA_USER, MAX(POSITION) AS POSITION, MAX(GPU.NAME) AS NAME, 'Participante' AS STATUS,  ");
                        sb.Append("SUM(OCP.REWARD_POINTS) AS PONTUATION ");
                        sb.Append("FROM GDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT (NOLOCK) CUP ");
                        sb.Append("INNER JOIN GDA_PERSONA_USER (NOLOCK) GPU ON GPU.IDGDA_PERSONA_USER = CUP.IDGDA_PERSONA ");
                        sb.Append("INNER JOIN GDA_OPERATIONAL_CAMPAIGN_PONTUATION (NOLOCK) OCP ON OCP.IDGDA_OPERATIONAL_CAMPAIGN = CUP.IDGDA_OPERATIONAL_CAMPAIGN ");
                        sb.Append("INNER JOIN GDA_OPERATIONAL_CAMPAIGN_MISIONS (NOLOCK) OCM ON OCM.IDGDA_OPERATIONAL_CAMPAIGN_PONTUATION = OCP.IDGDA_OPERATIONAL_CAMPAIGN_PONTUATION ");
                        sb.Append($"WHERE CUP.IDGDA_OPERATIONAL_CAMPAIGN = {inputModel.IDGDA_OPERATIONAL_CAMPAIGN} ");
                        sb.Append("AND POSITION > 0 ");
                        sb.Append("GROUP BY CUP.IDGDA_OPERATIONAL_CAMPAIGN, GPU.IDGDA_PERSONA_USER ");
                        sb.Append(") AS TBL ");
                        sb.Append("INNER JOIN GDA_OPERATIONAL_CAMPAIGN_AWARD (NOLOCK) OCA ON OCA.IDGDA_OPERATIONAL_CAMPAIGN = TBL.IDGDA_OPERATIONAL_CAMPAIGN  ");
                        sb.Append("and (OCA.MIN_PONTUATION <> 0 and TBL.PONTUATION > OCA.MIN_PONTUATION) OR (OCA.RANKING <> 0 and OCA.RANKING = TBL.POSITION) ");
                        sb.Append("GROUP BY TBL.IDGDA_OPERATIONAL_CAMPAIGN, TBL.IDGDA_PERSONA_USER ");
                        sb.Append("ORDER BY POSITION ");

                        using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    OperationalCampaignRanking OCR = new OperationalCampaignRanking();

                                    OCR.position = reader["POSITION"].ToString();
                                    OCR.name = reader["NAME"].ToString();
                                    OCR.status = reader["STATUS"].ToString();
                                    OCR.pontuation = reader["PONTUATION"].ToString();
                                    OCR.award = reader["VALUE_COINS"].ToString();

                                    retorno.rankings.Add(OCR);
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        // Trate a exceção aqui
                    }
                    connection.Close();
                }
                return retorno;
            }



        }
    }




}