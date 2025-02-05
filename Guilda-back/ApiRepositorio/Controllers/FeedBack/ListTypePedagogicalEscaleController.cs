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
using static ApiRepositorio.Controllers.LoadMyNotificationController;
using static ApiRepositorio.Controllers.ListFeedBackController;
using static ApiRepositorio.Controllers.ListFeedBackNotReceivedController;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ListTypePedagogicalEscaleController : ApiController
    {// POST: api/Results


        public class TypePedagogicalEscale
        {
            public int IDGDA_PEDAGOGICAL_SCALE_TYPE { get; set; }
            public string PEDAGOGICAL_SCALE_TYPE { get; set; }
        }



        [HttpGet]
        public IHttpActionResult PostResultsModel()
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

            List<TypePedagogicalEscale> rmams = new List<TypePedagogicalEscale>();

            rmams = BancoTypePedagogicalEscale.listTypePedagogicalEscale();

            return Ok(rmams);

        }
        // Método para serializar um DataTable em JSON


        #region Banco

        public class BancoTypePedagogicalEscale
        {
            public static List<TypePedagogicalEscale> listTypePedagogicalEscale()
            {
                List<TypePedagogicalEscale> ListTypePedagogicalEscale = new List<TypePedagogicalEscale>();
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT * FROM GDA_PEDAGOGICAL_SCALE_TYPE (NOLOCK) ");
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
                                    TypePedagogicalEscale TypePedagogicalEscale = new TypePedagogicalEscale();

                                    TypePedagogicalEscale.IDGDA_PEDAGOGICAL_SCALE_TYPE = reader["IDGDA_PEDAGOGICAL_SCALE_TYPE"] != DBNull.Value ? int.Parse(reader["IDGDA_PEDAGOGICAL_SCALE_TYPE"].ToString()) : 0;
                                    TypePedagogicalEscale.PEDAGOGICAL_SCALE_TYPE = reader["PEDAGOGICAL_SCALE_TYPE"] != DBNull.Value ? reader["PEDAGOGICAL_SCALE_TYPE"].ToString() : "";
                                    ListTypePedagogicalEscale.Add(TypePedagogicalEscale);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }
                return ListTypePedagogicalEscale;
            }
            #endregion

        }


    }
}