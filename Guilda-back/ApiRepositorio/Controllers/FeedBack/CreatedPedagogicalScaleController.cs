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
using DocumentFormat.OpenXml.VariantTypes;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class CreatedPedagogicalScaleController : ApiController
    {// POST: api/Results

        public class InputModelCreatedPedagogicalScale
        {
            public string NAME_INFRACTION { get; set; }
            public int CODE { get; set; }
            public int IDGDA_PEDAGOGICAL_SCALE_TYPE { get; set; }
            public int IDGDA_PEDAGOGICAL_SCALE_GRAVITY { get; set; }
            public int TIME_OFF { get; set; }
            public int PEDAGOGICAL_ORDER { get; set; }
        }


        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModelCreatedPedagogicalScale inputModel)
        {
            int collaboratorId = 0;
            int personaid = 0;
            int idPedagogicalScale = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            collaboratorId = inf.collaboratorId;
            personaid = inf.personauserId;

            BancoCreatedPedagogicalScale.InsertPedagogicalScale(inputModel, personaid);

            return Ok("Escala pedagogica criada com sucesso.");

        }
        // Método para serializar um DataTable em JSON

        [HttpPut]
        public IHttpActionResult PutResultsModel(int idPedagogicalScale, int active)
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

            BancoCreatedPedagogicalScale.UpdatePedagogicalScale(idPedagogicalScale, active, personaid);

            return Ok("Escala pedagogica atualizada com sucesso.");

        }

        [HttpDelete]
        public IHttpActionResult DeleteResultsModel(int idPedagogicalScale)
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

            BancoCreatedPedagogicalScale.DeletePedagogicalScale(idPedagogicalScale, personaid);

            return Ok("Escala pedagogica removida com sucesso.");

        }

        #region Banco


        public class BancoCreatedPedagogicalScale
        {
            public static void InsertPedagogicalScale(InputModelCreatedPedagogicalScale inputModel, int PERSONAID)
            {
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();

                    StringBuilder InserPedagogicalScale = new StringBuilder();
                    InserPedagogicalScale.Append("INSERT INTO GDA_PEDAGOGICAL_SCALE  ");
                    InserPedagogicalScale.Append("(IDGDA_PEDAGOGICAL_SCALE_TYPE, IDGDA_PEDAGOGICAL_SCALE_GRAVITY, PEDAGOGICAL_SCALE, CREATED_AT, IDPERSONA_CREATED_BY, TIME_OFF, PEDAGOGICAL_ORDER, CODE)  ");
                    InserPedagogicalScale.Append("VALUES  ");
                    InserPedagogicalScale.Append($"('{inputModel.IDGDA_PEDAGOGICAL_SCALE_TYPE}', '{inputModel.IDGDA_PEDAGOGICAL_SCALE_GRAVITY}', '{inputModel.NAME_INFRACTION}', GETDATE(), {PERSONAID}, {inputModel.TIME_OFF}, '{inputModel.PEDAGOGICAL_ORDER}', '{inputModel.CODE}')  ");

                    try
                    {
                        //connection.Open();
                        using (SqlCommand command2 = new SqlCommand(InserPedagogicalScale.ToString(), connection))
                        {
                            command2.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }
            }

            public static void UpdatePedagogicalScale(int id, int active, int personaid)
            {
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();

                    StringBuilder Update = new StringBuilder();
                    Update.Append("UPDATE GDA_PEDAGOGICAL_SCALE SET   ");
                    Update.Append($"ACTIVE = {active}, ");
                    Update.Append($"ACTIVE_BY = {personaid}  ");
                    Update.Append($"WHERE IDGDA_PEDAGOGICAL_SCALE = {id} ");
                    try
                    {

                        using (SqlCommand command = new SqlCommand(Update.ToString(), connection))
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

            public static void DeletePedagogicalScale(int id, int personaid)
            {
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();

                    StringBuilder Update = new StringBuilder();
                    Update.Append("UPDATE GDA_PEDAGOGICAL_SCALE SET   ");
                    Update.Append($"DELETED_AT = GETDATE(), ");
                    Update.Append($"IDPERSONA_DELETED_BY = {personaid}  ");
                    Update.Append($"WHERE IDGDA_PEDAGOGICAL_SCALE = {id} ");
                    try
                    {

                        using (SqlCommand command = new SqlCommand(Update.ToString(), connection))
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
        }
        #endregion

    }



}