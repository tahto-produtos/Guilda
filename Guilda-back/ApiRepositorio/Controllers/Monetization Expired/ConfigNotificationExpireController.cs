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
using ClosedXML.Excel;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ConfigNotificationExpireController : ApiController
    {// POST: api/Results
        public class InputModelConfigNotificationExpire
        {
            public int DAYS { get; set; }
            public bool VALIDATED { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel(InputModelConfigNotificationExpire inputModel)
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

            int DAYS = inputModel.DAYS;
            bool VALIDATED = inputModel.VALIDATED;
            if (inputModel.VALIDATED == false)
            {
                return BadRequest("Voce tem certeza que deseja seguir alteração de notificação expirada?");
            }
            else
            {
                //VERIFICAR SE JA TEMOS UMA PAUSA CRIADA
                int CONFIGNOTIFICATIONEXPIRE = BancoConfigNotificationExpire.ValidatedConfigNotificationExpire();
                BancoConfigNotificationExpire.InsertConfigNotificationExpire(CONFIGNOTIFICATIONEXPIRE, PERSONAID, DAYS);
            }
            return Ok("Configuração de Notificação expirada criada com sucesso.");

        }
        // Método para serializar um DataTable em JSON
    }

    #region Banco

    public class BancoConfigNotificationExpire 
    { 
        public static int ValidatedConfigNotificationExpire()
        {
            int retorno = 0;
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    StringBuilder sbSelectPause = new StringBuilder();
                    sbSelectPause.AppendFormat("SELECT  IDGDA_CONFIG_NOTIFICATION FROM GDA_CONFIG_NOTIFICATION (NOLOCK) ");
                    sbSelectPause.AppendFormat("WHERE DELETED_AT IS NULL ");

                    using (SqlCommand commandSelect = new SqlCommand(sbSelectPause.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandSelect.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                retorno = Convert.ToInt32(reader["IDGDA_CONFIG_NOTIFICATION"].ToString());
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

        public static void InsertConfigNotificationExpire(int CONFIGNOTIFICATIONEXPIRE, int PERSONAID, int DAYS)
        {
            if (CONFIGNOTIFICATIONEXPIRE != 0)
            {
                //REALIZAR UPDATE
                StringBuilder UpdatePause = new StringBuilder();
                UpdatePause.Append("UPDATE GDA_CONFIG_NOTIFICATION SET  ");
                UpdatePause.Append($"DELETED_AT = GETDATE(), DELETED_BY = {PERSONAID}  ");
                UpdatePause.Append($"WHERE IDGDA_CONFIG_NOTIFICATION = {CONFIGNOTIFICATIONEXPIRE}  ");
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

            #region INSERT IDGDA_CONFIG_NOTIFICATION
            StringBuilder InsertPause = new StringBuilder();
            InsertPause.Append("INSERT INTO GDA_CONFIG_NOTIFICATION  ");
            InsertPause.Append("(TYPE_NOTIFICATION, DAYS, CREATED_AT, CREATED_BY)  ");
            InsertPause.Append("VALUES  ");
            InsertPause.Append($"('9', '{DAYS}', GETDATE(), '{PERSONAID}')  ");
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