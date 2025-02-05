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
    public class DetailsOperationalCampaignController : ApiController
    {// POST: api/Results

        public class ReturnDetailsOperationalCampaign
        {
            public int  idCampaign {  get; set; }
            public string name { get; set; }
            public string image { get; set; }
            public int punctuation { get; set; }
            public int position { get; set; }
            public string dtInicio { get; set; }
            public string dtFim { get; set; }
            public string mission_Concluded { get; set; }
            public int mission_Punctuation { get; set; }
            public string mission_Status { get; set; }
            public int mission_Percent { get; set; }

            public int value_win { get; set; }
        }

        public class InputModelDetailsOperationalCampaign
        {
            public int IDGDA_OPERATIONAL_CAMPAIGN { get; set; }
            public bool? ISIMPORTANT { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModelDetailsOperationalCampaign inputModel)
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

            ReturnDetailsOperationalCampaign ReturnDetailsOperationalCampaign = new ReturnDetailsOperationalCampaign();

            ReturnDetailsOperationalCampaign = BankDetailsOperationalCampaign.returnDetailsOperationalCampaign(PERSONAUSERID, COLLABORATORID, inputModel);
            
            if (ReturnDetailsOperationalCampaign.idCampaign == 0)
            {
                inputModel.ISIMPORTANT = false;
                ReturnDetailsOperationalCampaign = BankDetailsOperationalCampaign.returnDetailsOperationalCampaign(PERSONAUSERID, COLLABORATORID, inputModel);
            }

            return Ok(ReturnDetailsOperationalCampaign);
        }
        // Método para serializar um DataTable em JSON

        public class BankDetailsOperationalCampaign
        {
            public static DetailsOperationalCampaignController.ReturnDetailsOperationalCampaign returnDetailsOperationalCampaign(int personaId, int collaboratorId, DetailsOperationalCampaignController.InputModelDetailsOperationalCampaign inputModel)
            {
                DetailsOperationalCampaignController.ReturnDetailsOperationalCampaign retorno = new DetailsOperationalCampaignController.ReturnDetailsOperationalCampaign();
                StringBuilder sb = new StringBuilder();
                inputModel.ISIMPORTANT = inputModel.ISIMPORTANT == null ? false : inputModel.ISIMPORTANT;

                string filter = "";
                if (inputModel.ISIMPORTANT == false)
                {
                    filter = $" AND OC.IDGDA_OPERATIONAL_CAMPAIGN = {inputModel.IDGDA_OPERATIONAL_CAMPAIGN} ";
                }
                else
                {
                    filter = $" AND OCUP.IMPORTANT = 1 ";
                }

                sb.Append("SELECT  ");
                sb.Append("MAX(OC.IDGDA_OPERATIONAL_CAMPAIGN) AS IDGDA_OPERATIONAL_CAMPAIGN, ");
                sb.Append("MAX(OC.NAME) AS NAME, ");
                sb.Append("MAX(OC.IMAGE) AS IMAGE, ");
                sb.Append("MAX(OC.STARTED_AT) AS STARTED_AT, ");
                sb.Append("MAX(OC.ENDED_AT) AS ENDED_AT, ");
                sb.Append("MAX(OCUP.POSITION) AS RANKING, ");
                sb.Append("CONVERT(VARCHAR, MAX(OCUP.VALUE)) + '/' + CONVERT(VARCHAR, SUM(OCP.REWARD_POINTS)) AS PONTUACAO,    ");
                sb.Append("MAX(IND.NAME) AS MISSAO, ");
                sb.Append("MAX(CASE WHEN OCP.IDGDA_OPERATIONAL_CAMPAIGN_PONTUATION = OCM.IDGDA_OPERATIONAL_CAMPAIGN_PONTUATION THEN OCP.REWARD_POINTS END) AS PONTUACAO_MISSAO, ");
                sb.Append("'COMPLETA' AS STATUS, ");
                sb.Append("SUM(OCUP.VALUE) AS VALOR ");
                sb.Append("FROM GDA_OPERATIONAL_CAMPAIGN (NOLOCK) OC ");
                sb.Append($"INNER JOIN GDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT (NOLOCK) AS OCUP ON OCUP.IDGDA_OPERATIONAL_CAMPAIGN = OC.IDGDA_OPERATIONAL_CAMPAIGN AND OCUP.IDGDA_PERSONA = {personaId} ");
                sb.Append("LEFT JOIN GDA_OPERATIONAL_CAMPAIGN_PONTUATION (NOLOCK) AS OCP ON OCP.IDGDA_OPERATIONAL_CAMPAIGN = OC.IDGDA_OPERATIONAL_CAMPAIGN ");
                sb.Append("LEFT JOIN ( SELECT  TOP 1 IDGDA_OPERATIONAL_CAMPAIGN_MISIONS, ");
                sb.Append("				   IDGDA_OPERATIONAL_CAMPAIGN_PONTUATION, ");
                sb.Append("				   IDGDA_PERSONA ");
                sb.Append("				   FROM GDA_OPERATIONAL_CAMPAIGN_MISIONS (NOLOCK) ");
                sb.Append("				   ORDER BY CREATED_AT DESC ");
                sb.Append($"				   ) AS OCM ON OCM.IDGDA_OPERATIONAL_CAMPAIGN_PONTUATION = OCP.IDGDA_OPERATIONAL_CAMPAIGN_PONTUATION AND OCM.IDGDA_PERSONA = {personaId} ");
                sb.Append($"LEFT JOIN GDA_INDICATOR (NOLOCK) AS IND ON IND.IDGDA_INDICATOR = OCP.IDGDA_INDICATOR AND  OCM.IDGDA_OPERATIONAL_CAMPAIGN_PONTUATION = OCP.IDGDA_OPERATIONAL_CAMPAIGN_PONTUATION AND OCM.IDGDA_PERSONA = {personaId} ");
                sb.Append($"WHERE 1 = 1 {filter} ");

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
                                    retorno.idCampaign = reader["IDGDA_OPERATIONAL_CAMPAIGN"] != DBNull.Value ? int.Parse(reader["IDGDA_OPERATIONAL_CAMPAIGN"].ToString()) : 0;
                                    retorno.name = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "";
                                    retorno.image = reader["IMAGE"] != DBNull.Value ? reader["IMAGE"].ToString() : "";
                                    retorno.punctuation = reader["PONTUACAO"] != DBNull.Value ? int.Parse(reader["PONTUACAO"].ToString()) : 0;
                                    retorno.position = reader["RANKING"] != DBNull.Value ? int.Parse(reader["RANKING"].ToString()) : 0;
                                    retorno.dtInicio = reader["STARTED_AT"] != DBNull.Value ? reader["STARTED_AT"].ToString() : "";
                                    retorno.dtFim = reader["ENDED_AT"] != DBNull.Value ? reader["ENDED_AT"].ToString() : "";
                                    retorno.mission_Concluded = reader["MISSAO"] != DBNull.Value ? reader["MISSAO"].ToString() : "";
                                    retorno.mission_Punctuation = reader["PONTUACAO_MISSAO"] != DBNull.Value ? int.Parse(reader["PONTUACAO_MISSAO"].ToString()) : 0;
                                    retorno.mission_Status = reader["STATUS"] != DBNull.Value ? reader["STATUS"].ToString() : "";

                                    retorno.mission_Percent = retorno.mission_Punctuation != 0 ? (retorno.position / retorno.mission_Punctuation) : 0;
                                }
                            }
                        }


                        StringBuilder sb2 = new StringBuilder();
                        sb2.Append($"SELECT TOP 1 BALANCE FROM GDA_CHECKING_ACCOUNT (NOLOCK) ");
                        sb2.Append($"WHERE COLLABORATOR_ID = {collaboratorId} ");
                        sb2.Append($"ORDER BY CREATED_AT DESC ");

                        using (SqlCommand command = new SqlCommand(sb2.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    retorno.value_win = reader["BALANCE"] != DBNull.Value ? int.Parse(reader["BALANCE"].ToString()) : 0;
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