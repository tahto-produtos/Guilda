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
using System.Threading;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    public class returnListsClimates
    {
        public bool response { get; set; }
        public List<listReturnListsClimates> climates { get; set; }
    }

    public class listReturnListsClimates
    {
        public int idClimate { get; set; }
        public string climate { get; set; }
        public string image { get; set; }
    }


    //[Authorize]
    public class ListClimateController : ApiController
    {// POST: api/Results
        [HttpGet]
        public IHttpActionResult PostResultsModel()
        {

            //Thread.Sleep(3000);
            //Task.Delay(3000);
            int COLLABORATORID = 0;
            int PERSONAUSERID = 0;
            var token = Request.Headers.Authorization?.Parameter;
            //PERSONAUSERID = BankListClimate.returnPersonaCollaboratorId(COLLABORATORID);

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            COLLABORATORID = inf.collaboratorId;
            PERSONAUSERID = inf.personauserId;

            if (PERSONAUSERID == 0)
            {
                returnListsClimates rtn = new returnListsClimates();
                rtn.response = false;
                return Ok(rtn);
            }

            bool responseClimate = BankDoClimate.verifyInsertClimate(PERSONAUSERID);
            if (responseClimate == true)
            {
                returnListsClimates rtn = new returnListsClimates();
                rtn.response = false;
                return Ok(rtn);
            }


            returnListsClimates rmams = new returnListsClimates();
            rmams = BankListClimate.listClimate();
            rmams.response = true;

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(rmams);
        }
        // Método para serializar um DataTable em JSON
    }

    public class BankListClimate
    {

        public static int returnPersonaCollaboratorId(int collaboratorId)
        {
            int retorno = 0;

            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT CU.IDGDA_PERSONA_USER ");
            sb.Append($"FROM GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS CU ");
            sb.Append($"INNER JOIN GDA_PERSONA_USER (NOLOCK) AS PU ON CU.IDGDA_PERSONA_USER = PU.IDGDA_PERSONA_USER ");
            sb.Append($"WHERE IDGDA_COLLABORATORS = {collaboratorId} AND PU.IDGDA_PERSONA_USER_TYPE = 1 AND PU.DELETED_AT IS NULL ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                retorno = Convert.ToInt32(reader["IDGDA_PERSONA_USER"].ToString());
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

            return retorno;
        }


        

        public static returnListsClimates listClimate()
        {

            returnListsClimates retorno = new returnListsClimates();
            retorno.climates = new List<listReturnListsClimates>();

            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT IDGDA_CLIMATE, CLIMATE, IMAGE, POSITION ");
            sb.Append($"FROM GDA_CLIMATE (NOLOCK) ");
            sb.Append($"WHERE DELETED_AT IS NULL ");
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
                                listReturnListsClimates rtn = new listReturnListsClimates();

                                rtn.idClimate = reader["IDGDA_CLIMATE"] != DBNull.Value ? int.Parse(reader["IDGDA_CLIMATE"].ToString()) : 0;
                                rtn.climate = reader["CLIMATE"] != DBNull.Value ? reader["CLIMATE"].ToString() : "";
                                rtn.image = reader["IMAGE"] != DBNull.Value ? reader["IMAGE"].ToString() : "";

                                retorno.climates.Add(rtn);
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
            return retorno;
        }

    }


}