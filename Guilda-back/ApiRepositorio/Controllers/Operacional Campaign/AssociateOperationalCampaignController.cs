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
    public class AssociateOperationalCampaignController : ApiController
    {// POST: api/Results
        public class InputModelAssociateOperationalCampaign
        {
            public int IDGDA_OPERATIONAL_CAMPAIGN { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModelAssociateOperationalCampaign inputModel)
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

            BankAssociateOperationalCampaign.insertAssociateOperationalCampaign(PERSONAUSERID, inputModel);
            
            return Ok("Associacao de campanha com sucesso");
        }
        // Método para serializar um DataTable em JSON

        public class BankAssociateOperationalCampaign
        {
            public static void insertAssociateOperationalCampaign(int personaId, AssociateOperationalCampaignController.InputModelAssociateOperationalCampaign inputModel)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("INSERT GDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT (IDGDA_OPERATIONAL_CAMPAIGN, IDGDA_PERSONA)  ");
                sb.Append("VALUES ");
                sb.Append($"({inputModel.IDGDA_OPERATIONAL_CAMPAIGN}, {personaId}) ");


                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Trate a exceção aqui
                    }
                    connection.Close();
                }
            }
        }
    }




}