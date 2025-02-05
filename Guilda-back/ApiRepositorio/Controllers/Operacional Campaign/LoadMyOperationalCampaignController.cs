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
using static TokenService;

//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class LoadMyOperationalCampaignController : ApiController
    {// POST: api/Results

        public class ReturnMyOperationalCampaign
        {
            public int totalpages { get; set; }
            public List<LoadMyOperationalCampaign> MyOperationalCampaign { get; set; }
        }

        public class LoadMyOperationalCampaign
        {
            public int IDGDA_OPERATIONAL_CAMPAIGN { get; set; }
            public string NAME { get; set; }
            public string IMAGE { get; set; }
        }

        public class InputModelMyOperationalCampaign
        {
            public DateTime? STARTEDATFROM { get; set; }
            public DateTime? STARTEDATTO { get; set; }
            public DateTime? ENDEDATFROM { get; set; }
            public DateTime? ENDEDATTO { get; set; }
            public string NAME { get; set; }
            public int limit { get; set; }
            public int page { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModelMyOperationalCampaign inputModel)
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

            ReturnMyOperationalCampaign returnMyOperationalCampaign = new ReturnMyOperationalCampaign();

            returnMyOperationalCampaign = BankLoadMyOperationalCampaign.returnMyOperationalCampaign(PERSONAUSERID, inputModel);
            


            return Ok(returnMyOperationalCampaign);
        }
        // Método para serializar um DataTable em JSON
    }

    public class BankLoadMyOperationalCampaign
    {
        public static string filterMyOperationalCampaign(LoadMyOperationalCampaignController.InputModelMyOperationalCampaign inputModel)
        {
            string ret = "";

            try
            {
                if (inputModel.STARTEDATFROM != (DateTime?)null)
                {
                    ret = $"{ret} AND STARTED_AT >= '{inputModel.STARTEDATFROM}' AND STARTED_AT <= '{inputModel.STARTEDATTO}' ";
                }
                if (inputModel.ENDEDATFROM != (DateTime?)null)
                {
                    ret = $"{ret} AND ENDED_AT >= '{inputModel.ENDEDATFROM}' AND ENDED_AT <= '{inputModel.ENDEDATTO}' ";
                }        
                if (inputModel.NAME != "")
                {
                    ret = $"{ret} AND NAME '%{inputModel.NAME}%' ";

                }             
            }
            catch (Exception)
            {

            }

            return ret;
        }

        public static LoadMyOperationalCampaignController.ReturnMyOperationalCampaign returnMyOperationalCampaign(int personaId,LoadMyOperationalCampaignController.InputModelMyOperationalCampaign inputModel)
        {
            LoadMyOperationalCampaignController.ReturnMyOperationalCampaign retorno = new LoadMyOperationalCampaignController.ReturnMyOperationalCampaign();
            List<LoadMyOperationalCampaignController.LoadMyOperationalCampaign> listOperationalCampaign = new List<LoadMyOperationalCampaignController.LoadMyOperationalCampaign>();

            string filter = filterMyOperationalCampaign(inputModel);
            int totalInfo = Funcoes.QuantidadeMyOperationalCampaign(filter, personaId);
            int totalpage = (int)Math.Ceiling((double)totalInfo / inputModel.limit);
            int offset = (inputModel.page - 1) * inputModel.limit;
            retorno.totalpages = totalpage;


            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT OC.IDGDA_OPERATIONAL_CAMPAIGN, OC.NAME, OC.IMAGE ");
            sb.Append("FROM GDA_OPERATIONAL_CAMPAIGN (NOLOCK) OC ");
            sb.Append("INNER JOIN GDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT (NOLOCK) AS  OCUP ON OCUP.IDGDA_OPERATIONAL_CAMPAIGN = OC.IDGDA_OPERATIONAL_CAMPAIGN ");
            sb.Append("WHERE OC.DELETED_AT IS NULL ");
            sb.Append($"AND OCUP.IDGDA_PERSONA = {personaId} ");
            sb.Append($"{filter} ");
            sb.Append("ORDER BY OC.STARTED_AT DESC ");
            sb.Append($"OFFSET {offset} ROWS FETCH NEXT {inputModel.limit} ROWS ONLY ");

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
                                LoadMyOperationalCampaignController.LoadMyOperationalCampaign LoadMyOperationalCampaign = new LoadMyOperationalCampaignController.LoadMyOperationalCampaign();

                                LoadMyOperationalCampaign.IDGDA_OPERATIONAL_CAMPAIGN = reader["IDGDA_OPERATIONAL_CAMPAIGN"] != DBNull.Value ? int.Parse(reader["IDGDA_OPERATIONAL_CAMPAIGN"].ToString()) : 0;
                                LoadMyOperationalCampaign.NAME = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "";
                                LoadMyOperationalCampaign.IMAGE = reader["IMAGE"] != DBNull.Value ? reader["IMAGE"].ToString() : "";
                                listOperationalCampaign.Add(LoadMyOperationalCampaign);
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
            retorno.MyOperationalCampaign = listOperationalCampaign;
            return retorno;
        }

    }


}