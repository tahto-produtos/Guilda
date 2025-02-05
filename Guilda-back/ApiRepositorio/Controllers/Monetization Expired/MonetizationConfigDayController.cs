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
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using static ApiRepositorio.Controllers.MonetizationConfigDayController;
using Microsoft.CodeAnalysis.Operations;
using DocumentFormat.OpenXml.Drawing;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class MonetizationConfigDayController : ApiController
    {// POST: api/Results
        public class InputModelMonetizationConfigDay
        {
            public int DAYS { get; set; }
            public DateTime STARTED_AT { get; set; }
            public VisibilityConfigMonetizationDay visibility { get; set; }
        }
        public class VisibilityConfigMonetizationDay
        {
            public List<int> site { get; set; }
            public List<int> setor { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel(InputModelMonetizationConfigDay inputModel)
        {
            int COLLABORATORID = 0;
            int personaid = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            COLLABORATORID = inf.collaboratorId;
            personaid = inf.personauserId;

            int PAST_DATE = 0;
            int DAYS = inputModel.DAYS;
            string STARTED_AT = inputModel.STARTED_AT.ToString("yyyy-MM-dd 00:00:00");

            string dataAtual = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");

            if (DateTime.Parse(dataAtual) > DateTime.Parse(STARTED_AT))
            {
                PAST_DATE = 1;
            }
            BancoCreateConfigMonetization.insertVisibility(inputModel, PAST_DATE, COLLABORATORID);

            return Ok("Configuração da monetização criada com sucesso.");
            
        }
        // Método para serializar um DataTable em JSON

    }
    #region Banco
    public class BancoCreateConfigMonetization
    {
        public static void insertVisibility(MonetizationConfigDayController.InputModelMonetizationConfigDay inputPost, int PAST_DATE, int COLLABORATORID)
        {
            //Sector
            if (inputPost.visibility.setor.Count > 0)
            {
                foreach (int item in inputPost.visibility.setor)
                {
                    bool ValidatedConfig = BancoCreateConfigMonetization.SelectConfigMonetization(item);

                    BancoCreateConfigMonetization.inserVisibilityItem(ValidatedConfig, inputPost.DAYS, inputPost.STARTED_AT.ToString("yyy-MM-dd 00:00:00"), PAST_DATE, COLLABORATORID, 1, item);
                }
            }

            //Site
            if (inputPost.visibility.site.Count > 0)
            {
                foreach (int item in inputPost.visibility.site)
                {
                    bool ValidatedConfig = BancoCreateConfigMonetization.SelectConfigMonetization(item);

                    BancoCreateConfigMonetization.inserVisibilityItem(ValidatedConfig, inputPost.DAYS, inputPost.STARTED_AT.ToString("yyy-MM-dd 00:00:00"), PAST_DATE, COLLABORATORID, 2, item);
                }
            }

        }

        public static bool SelectConfigMonetization(int referer)
        {
            bool retorno = false;
            #region SELECT GDA_MONETIZATION_CONFIG
            StringBuilder SelectConfigMonetization = new StringBuilder();
            SelectConfigMonetization.Append("SELECT * FROM GDA_MONETIZATION_CONFIG (NOLOCK)  ");
            SelectConfigMonetization.Append("WHERE DELETED_AT IS NULL  ");
            SelectConfigMonetization.Append($"AND ID_REFERER = {referer} ");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand commandSelect = new SqlCommand(SelectConfigMonetization.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandSelect.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                retorno = true;
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

        public static void inserVisibilityItem(bool ValidatedConfigint, int DAYS, string STARTED_AT, int PAST_DATE, int COLLABORATORID, int filterType, int referer)
        {
            if(ValidatedConfigint == true)
            {
                //REALIZAR UPDATE
                StringBuilder UpdatePause = new StringBuilder();
                UpdatePause.Append("UPDATE GDA_MONETIZATION_CONFIG SET  ");
                UpdatePause.Append("DELETED_AT = GETDATE()  ");
                UpdatePause.Append($"WHERE ID_REFERER = {referer}  ");
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



            #region INSERT GDA_MONETIZATION_CONFIG
            StringBuilder InsertConfigMonetization = new StringBuilder();
            InsertConfigMonetization.Append("INSERT INTO GDA_MONETIZATION_CONFIG  ");
            InsertConfigMonetization.Append("(DAYS, CREATED_AT, CREATED_BY, STARTED_AT, REPROCESSED, PAST_DATE, IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE, ID_REFERER)  ");
            InsertConfigMonetization.Append("VALUES  ");
            InsertConfigMonetization.Append($"('{DAYS}', GETDATE(), '{COLLABORATORID}', '{STARTED_AT}', '0', '{PAST_DATE}', '{filterType}', '{referer}' )  ");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(InsertConfigMonetization.ToString(), connection))
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