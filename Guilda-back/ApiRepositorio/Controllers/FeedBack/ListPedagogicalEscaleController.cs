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
    public class ListPedagogicalEscaleController : ApiController
    {// POST: api/Results

        public class PedagogicalEscale
        {
            public int IDGDA_PEDAGOGICAL_SCALE { get; set; }
            public int IDGDA_PEDAGOGICAL_SCALE_TYPE { get; set; }
            public int IDGDA_PEDAGOGICAL_SCALE_GRAVITY { get; set; }
            public int IDPERSONA_CREATED_BY { get; set; }
            public int TIME_OFF { get; set; }
            public int PEDAGOGICAL_ORDER { get; set; }
            public string PEDAGOGICAL_SCALE { get; set; }
            public string CREATED_AT { get; set; }
            public int ACTIVE { get; set; }
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

            List<PedagogicalEscale> rmams = new List<PedagogicalEscale>();

            rmams = BancoPedagogicalEscale.listPedagogicalEscale();

            return Ok(rmams);

        }
        // Método para serializar um DataTable em JSON


        #region Banco

        public class BancoPedagogicalEscale
        {
            public static List<PedagogicalEscale> listPedagogicalEscale()
            {
                List<PedagogicalEscale> ListPedagogicalEscale = new List<PedagogicalEscale>();
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT * FROM GDA_PEDAGOGICAL_SCALE (NOLOCK) ");
                sb.Append("WHERE DELETED_AT IS NULL ");
                sb.Append("ORDER BY IDGDA_PEDAGOGICAL_SCALE_GRAVITY, PEDAGOGICAL_ORDER ASC ");
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
                                    PedagogicalEscale PedagogicalEscale = new PedagogicalEscale();

                                    PedagogicalEscale.IDGDA_PEDAGOGICAL_SCALE = reader["IDGDA_PEDAGOGICAL_SCALE"] != DBNull.Value ? int.Parse(reader["IDGDA_PEDAGOGICAL_SCALE"].ToString()) : 0;
                                    PedagogicalEscale.IDGDA_PEDAGOGICAL_SCALE_TYPE = reader["IDGDA_PEDAGOGICAL_SCALE_TYPE"] != DBNull.Value ? int.Parse(reader["IDGDA_PEDAGOGICAL_SCALE_TYPE"].ToString()) : 0;
                                    PedagogicalEscale.IDGDA_PEDAGOGICAL_SCALE_GRAVITY = reader["IDGDA_PEDAGOGICAL_SCALE_GRAVITY"] != DBNull.Value ? int.Parse(reader["IDGDA_PEDAGOGICAL_SCALE_GRAVITY"].ToString()) : 0;
                                    PedagogicalEscale.IDPERSONA_CREATED_BY = reader["IDPERSONA_CREATED_BY"] != DBNull.Value ? int.Parse(reader["IDPERSONA_CREATED_BY"].ToString()) : 0;
                                    PedagogicalEscale.TIME_OFF = reader["TIME_OFF"] != DBNull.Value ? int.Parse(reader["TIME_OFF"].ToString()) : 0;
                                    PedagogicalEscale.PEDAGOGICAL_SCALE = reader["PEDAGOGICAL_SCALE"] != DBNull.Value ? reader["PEDAGOGICAL_SCALE"].ToString() : "";
                                    PedagogicalEscale.CREATED_AT = reader["CREATED_AT"] != DBNull.Value ? reader["CREATED_AT"].ToString() : "";
                                    PedagogicalEscale.PEDAGOGICAL_ORDER = reader["PEDAGOGICAL_ORDER"] != DBNull.Value ? int.Parse(reader["PEDAGOGICAL_ORDER"].ToString()) : 0;
                                    PedagogicalEscale.ACTIVE = reader["ACTIVE"] != DBNull.Value ? int.Parse(reader["ACTIVE"].ToString()) : 0;
                                    ListPedagogicalEscale.Add(PedagogicalEscale);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }
                return ListPedagogicalEscale;
            }
            #endregion

        }


    }
}