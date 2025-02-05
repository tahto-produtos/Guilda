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
    public class SignedFeedBackController : ApiController
    {// POST: api/Results

        public class InputModelSignedFeedBack
        {
            public int IDGDA_FEEDBACK_USER { get; set; }
        }



        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModelSignedFeedBack inputModel)
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

            //INSERÇÃO DO FEEDBACK
            BancoSignedFeedBack.UpdateFeedBack(personaid, inputModel.IDGDA_FEEDBACK_USER);
            
            return Ok("FeedBack assinado com sucesso.");
            
        }
        // Método para serializar um DataTable em JSON


        #region Banco

        public class BancoSignedFeedBack
        {
           public static void UpdateFeedBack(int PersonaUser, int IdFeedbackUser)
            {
                //Validar quem está flegando se é colaborator que recebeu o feedback ou collaborador que aplicou o feedback
                bool validadedReceiver = ValidatedPersonaUser(PersonaUser, IdFeedbackUser);
                bool validadedSended = ValidatedPersonaUserSended(PersonaUser, IdFeedbackUser);

                #region INSERT GDAFEEDBACK_USER
                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE GDA_FEEDBACK_USER SET  ");
                sb.Append("PROTOCOL = PROTOCOL ");
                if (validadedReceiver == true)
                {
                    sb.Append($", RESPONDED_AT = GETDATE() ");
                    sb.Append($", SIGNED_RECEIVED = 1 ");
                }
                if (validadedSended == true)
                {
                    sb.Append($", SIGNED_CREATOR = 1 ");
                }
                sb.Append($"WHERE IDGDA_FEEDBACK_USER = {IdFeedbackUser}  ");
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }
                #endregion

            }
   
           public static bool ValidatedPersonaUser(int personaUser, int IdFeedbackUser)
            {
                bool retorno = false;
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT IDPERSONA_RECEIVED_BY FROM GDA_FEEDBACK_USER  ");
                sb.Append($"WHERE IDGDA_FEEDBACK_USER  = {IdFeedbackUser} ");
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    int IdPersonaReceived = int.Parse(reader["IDPERSONA_RECEIVED_BY"].ToString());

                                    if (IdPersonaReceived == personaUser)
                                    {
                                        retorno = true;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }

                return retorno;
            }

            public static bool ValidatedPersonaUserSended(int personaUser, int IdFeedbackUser)
            {
                bool retorno = false;
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT IDPERSONA_SENDED_BY FROM GDA_FEEDBACK_USER  ");
                sb.Append($"WHERE IDGDA_FEEDBACK_USER  = {IdFeedbackUser} ");
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    int IdPersonaReceived = int.Parse(reader["IDPERSONA_SENDED_BY"].ToString());

                                    if (IdPersonaReceived == personaUser)
                                    {
                                        retorno = true;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }

                return retorno;
            }
        }     
        #endregion

    }



}