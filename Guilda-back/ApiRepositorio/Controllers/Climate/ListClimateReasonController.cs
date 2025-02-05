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
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    public class returnListsClimateReason
    {
        public bool response { get; set; }
        public List<listReturnListsClimateReason> reasons { get; set; }
    }

    public class listReturnListsClimateReason
    {
        public int idClimate { get; set; }
        public int idClimateReason { get; set; }
        public string reason { get; set; }
        public string image { get; set; }
    }

    //[Authorize]
    public class ListClimateReasonController : ApiController
    {// POST: api/Results
        [HttpGet]
        public IHttpActionResult PostResultsModel(int idClimate)
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

            returnListsClimateReason rmams = new returnListsClimateReason();
            rmams = BankListClimateReason.listClimateReason(idClimate);

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(rmams);
        }
        // Método para serializar um DataTable em JSON
    }

    public class BankListClimateReason
    {



        public static returnListsClimateReason listClimateReason(int idClimate)
        {

            returnListsClimateReason retorno = new returnListsClimateReason();
            retorno.reasons = new List<listReturnListsClimateReason>();
            retorno.response = false;

            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT IDGDA_CLIMATE_REASON, IDGDA_CLIMATE, REASON, IMAGE, POSITION  ");
            sb.Append($"FROM GDA_CLIMATE_REASON (NOLOCK) ");
            sb.Append($"WHERE DELETED_AT IS NULL AND IDGDA_CLIMATE = {idClimate} ");
            sb.Append($"ORDER BY POSITION ");

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
                                listReturnListsClimateReason rtn = new listReturnListsClimateReason();

                                rtn.idClimate = reader["IDGDA_CLIMATE"] != DBNull.Value ? int.Parse(reader["IDGDA_CLIMATE"].ToString()) : 0;
                                rtn.idClimateReason = reader["IDGDA_CLIMATE_REASON"] != DBNull.Value ? int.Parse(reader["IDGDA_CLIMATE_REASON"].ToString()) : 0;
                                rtn.reason = reader["REASON"] != DBNull.Value ? reader["REASON"].ToString() : "";
                                rtn.image = reader["IMAGE"] != DBNull.Value ? reader["IMAGE"].ToString() : "";

                                retorno.reasons.Add(rtn);
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

            if (retorno.reasons.Count > 0)
            {
                retorno.response = true;
            }

            return retorno;
        }

    }


}