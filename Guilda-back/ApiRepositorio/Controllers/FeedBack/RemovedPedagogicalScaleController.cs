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
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class RemovedPedagogicalScaleController : ApiController
    {// POST: api/Results

        public class InputModelCreatedPedagogicalScale
        {
            public int IDGDA_PEDAGOGICAL_SCALE { get; set; }
        }



        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModelCreatedPedagogicalScale inputModel)
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

            BancoCreatedPedagogicalScale.RemovePedagogicalScale(inputModel, personaid);

            return Ok("Escala pedagogica removida com sucesso.");

        }
        // Método para serializar um DataTable em JSON


        #region Banco

        public class BancoCreatedPedagogicalScale
        {
            public static void RemovePedagogicalScale(InputModelCreatedPedagogicalScale inputModel, int PERSONAID)
            {
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    #region UPDATE GDA_PEDAGOGICAL_SCALE
                    StringBuilder Update = new StringBuilder();
                    Update.Append("UPDATE GDA_PEDAGOGICAL_SCALE SET   ");
                    Update.Append("DELETED_AT = GETDATE(), ");
                    Update.Append($"IDPERSONA_DELETED_BY = {PERSONAID}  ");
                    Update.Append($"WHERE IDGDA_PEDAGOGICAL_SCALE = {inputModel.IDGDA_PEDAGOGICAL_SCALE} ");
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(Update.ToString(), connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    #endregion

                }
            }
            #endregion

        }


    }
}