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

    public class returnListGroupClimate
    {
        public int idClimate { get; set; }
        public string climate { get; set; }
        public string image { get; set; }
        public int count { get; set; }
        public double percent { get; set; }
    }

    //[Authorize]
    public class ListGroupClimateController : ApiController
    {// POST: api/Results
        [HttpGet]
        public IHttpActionResult PostResultsModel(DateTime STARTEDATFROM, DateTime STARTEDATTO)
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

            bool adm = Funcoes.retornaPermissao(COLLABORATORID.ToString());
            if (adm == true)
            {
                //COLOCA O ID DO CEO, POIS TERA A MESMA VISÃO
                COLLABORATORID = 756399;
            }

            List<returnListGroupClimate> rmams = new List<returnListGroupClimate>();
            rmams = BankListGroupClimate.listReportHierarchyClimate(COLLABORATORID, STARTEDATFROM, STARTEDATTO);

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(rmams);
        }
        // Método para serializar um DataTable em JSON
    }

    public class BankListGroupClimate
    {
        public static List<returnListGroupClimate> listReportHierarchyClimate(int idCollaborator, DateTime STARTEDATFROM, DateTime STARTEDATTO)
        {

            List<returnListGroupClimate> retorno = new List<returnListGroupClimate>();


            string dtInicial = STARTEDATFROM.ToString("yyyy-MM-dd");
            string dtFinal = STARTEDATTO.ToString("yyyy-MM-dd");

            StringBuilder sb = new StringBuilder();
            sb.Append($"DECLARE @ID INT; SET @ID = '{idCollaborator}'; ");
            sb.Append($"DECLARE @DATE_INI DATE; SET @DATE_INI = '{dtInicial}'; ");
            sb.Append($"DECLARE @DATE_FIM DATE; SET @DATE_FIM = '{dtFinal}'; ");
            sb.Append($"SELECT GC.IDGDA_CLIMATE, GC.CLIMATE, GC.IMAGE, ISNULL(GCU_COUNT, 0) AS COUNT ");
            sb.Append($"FROM GDA_CLIMATE GC ");
            sb.Append($"LEFT JOIN ( ");
            sb.Append($"    SELECT IDGDA_CLIMATE, COUNT(*) AS GCU_COUNT ");
            sb.Append($"    FROM GDA_CLIMATE_USER (NOLOCK) GDU ");
            sb.Append($"	INNER JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCU ON GDU.IDGDA_PERSONA = PCU.IDGDA_PERSONA_USER  ");
            sb.Append($"	INNER JOIN GDA_PERSONA_USER (NOLOCK) AS PU ON PU.IDGDA_PERSONA_USER = PCU.IDGDA_PERSONA_USER AND PU.IDGDA_PERSONA_USER_TYPE = 1  ");
            sb.Append($"	INNER JOIN ( ");
            sb.Append($"		SELECT IDGDA_COLLABORATORS, CONVERT(DATE, CREATED_AT) AS CREATED_AT FROM GDA_COLLABORATORS_DETAILS (NOLOCK) ");
            sb.Append($"		WHERE CREATED_AT >= @DATE_INI AND CREATED_AT <= @DATE_FIM AND (IDGDA_COLLABORATORS = @ID OR   ");
            sb.Append($"		MATRICULA_SUPERVISOR = @ID OR   ");
            sb.Append($"		MATRICULA_COORDENADOR = @ID OR   ");
            sb.Append($"		MATRICULA_GERENTE_II = @ID OR   ");
            sb.Append($"		MATRICULA_GERENTE_I = @ID OR   ");
            sb.Append($"		MATRICULA_DIRETOR = @ID OR   ");
            sb.Append($"		MATRICULA_CEO = @ID) GROUP BY CONVERT(DATE, CREATED_AT), IDGDA_COLLABORATORS) ");
            sb.Append($"	 CD ON CD.IDGDA_COLLABORATORS = PCU.IDGDA_COLLABORATORS   ");
            sb.Append($"		  ");
            sb.Append($"    WHERE CONVERT(DATE, GDU.CREATED_AT) >= @DATE_INI AND CONVERT(DATE, GDU.CREATED_AT) <= @DATE_FIM AND CONVERT(DATE, CD.CREATED_AT) = CONVERT(DATE, GDU.CREATED_AT) ");
            sb.Append($"    GROUP BY IDGDA_CLIMATE ");
            sb.Append($") GCU ON GCU.IDGDA_CLIMATE = GC.IDGDA_CLIMATE ");
            sb.Append($"ORDER BY GC.POSITION; ");


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
                                returnListGroupClimate rtn = new returnListGroupClimate();

                                rtn.idClimate = reader["IDGDA_CLIMATE"] != DBNull.Value ? int.Parse(reader["IDGDA_CLIMATE"].ToString()) : 0;
                                rtn.climate = reader["CLIMATE"] != DBNull.Value ? reader["CLIMATE"].ToString() : "";
                                rtn.image = reader["IMAGE"] != DBNull.Value ? reader["IMAGE"].ToString() : "";
                                rtn.count = reader["COUNT"] != DBNull.Value ? int.Parse(reader["COUNT"].ToString()) : 0;

                                retorno.Add(rtn);
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

            int totalQuantity = retorno.Sum(item => item.count);

            List<returnListGroupClimate> returnLists = retorno
                         .Select(group => new returnListGroupClimate
                         {
                             idClimate = group.idClimate,
                             climate = group.climate,
                             image = group.image,
                             count = group.count,
                             percent = totalQuantity != 0 ? Math.Round(((double)group.count / totalQuantity) * 100, 1, MidpointRounding.AwayFromZero) : 0
                         }).ToList();

            return returnLists;
        }

    }

}