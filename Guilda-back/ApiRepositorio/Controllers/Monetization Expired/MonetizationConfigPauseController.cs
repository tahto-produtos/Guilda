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
    public class MonetizationConfigPauseController : ApiController
    {// POST: api/Results

        public class InputModelMonetizationConfigPause
        {
            public DateTime PAUSE_AT { get; set; }
            public int PAUSE { get; set; }
            public bool VALIDATED { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel(InputModelMonetizationConfigPause inputModel)
        {
            int COLLABORATORID = 0;
            int PERSONAID = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            COLLABORATORID = inf.collaboratorId;
            PERSONAID = inf.personauserId;

            int PAUSE = inputModel.PAUSE;
            int REPROCESSED = 0;
            bool VALIDATED = inputModel.VALIDATED;
            string PAUSE_AT = inputModel.PAUSE_AT.ToString("yyyy-MM-dd 00:00:00");
            if (inputModel.VALIDATED == false)
            {
                return BadRequest("Voce tem certeza que deseja seguir alteração de pausa?");
            }
            else
            {
                //VERIFICAR SE JA TEMOS UMA PAUSA CRIADA
                int PauseCreated = BancoMonetizationPause.ValidatedPause();
                BancoMonetizationPause.InsertPause(PauseCreated, COLLABORATORID, PAUSE, REPROCESSED, PAUSE_AT);
            }
            return Ok("Pausa criada com sucesso.");

        }
        // Método para serializar um DataTable em JSON

    }
    #region Banco

    public class BancoMonetizationPause
    {
        public static int ValidatedPause()
        {
            int retorno = 0;
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    StringBuilder sbSelectPause = new StringBuilder();
                    sbSelectPause.AppendFormat("SELECT  IDGDA_MONETIZATION_CONFIG_PAUSE FROM GDA_MONETIZATION_CONFIG_PAUSE (NOLOCK) ");
                    sbSelectPause.AppendFormat("WHERE DELETED_AT IS NULL ");

                    using (SqlCommand commandSelect = new SqlCommand(sbSelectPause.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandSelect.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                retorno = Convert.ToInt32(reader["IDGDA_MONETIZATION_CONFIG_PAUSE"].ToString());
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                connection.Close();
            }
            return retorno;
        }

        public static void InsertPause(int PauseCreated, int collaboratorID, int pause, int reprocessed, string pause_At)
        {
            if(PauseCreated != 0)
            {
                //REALIZAR UPDATE
                StringBuilder UpdatePause = new StringBuilder();
                UpdatePause.Append("UPDATE GDA_MONETIZATION_CONFIG_PAUSE SET  ");
                UpdatePause.Append("DELETED_AT = GETDATE()  ");
                UpdatePause.Append($"WHERE IDGDA_MONETIZATION_CONFIG_PAUSE = {PauseCreated}  ");
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(UpdatePause.ToString(), connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            #region INSERT GDA_MONETIZATION_CONFIG_PAUSE
            StringBuilder InsertPause = new StringBuilder();
            InsertPause.Append("INSERT INTO GDA_MONETIZATION_CONFIG_PAUSE  ");
            InsertPause.Append("(PAUSE_AT, PAUSE_BY, PAUSE, REPROCESSED)  ");
            InsertPause.Append("VALUES  ");
            InsertPause.Append($"('{pause_At}', '{collaboratorID}', '{pause}', '{reprocessed}')  ");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(InsertPause.ToString(), connection))
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