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
using static ApiRepositorio.Controllers.ResultConsolidatedController;
using System.Drawing;
using DocumentFormat.OpenXml.Bibliography;
using System.Runtime.ConstrainedExecution;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ListPainelController : ApiController
    {// POST: api/Results

        public class itensPainel
        {
            public int ID_COLLABORATOR { get; set; }
            public string NAME { get; set; }
            public string HIERARQUIA { get; set; }
            public string ATIVO { get; set; }
            public string REPROCESSAMENTO { get; set; }
            public int MOEDAS_GANHAS { get; set; }
            public int FATOR0 { get; set; }
            public int FATOR1 { get; set; }
            public string DATA_RECEBIMENTO_RESULTADO { get; set; }
            public int META_UTILIZADA_MOEDA { get; set; }
            public int METRICA_MINIMA { get; set; }
            public int METRICA_GRUPO { get; set; }
            public string ALTEROU_METRICA { get; set; }
        }

        [HttpGet]
        public IHttpActionResult GetResultsModel(string dtSend, int idIndicator, int idCollaborator)
        {
            //Setar Filtro de Resultados da API somente para um mes

            int collaboratorId = 0;
            int personauserId = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            collaboratorId = inf.collaboratorId;
            personauserId = inf.personauserId;

            DateTime dtTimeFinal = DateTime.ParseExact(dtSend, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            List<itensPainel> rmams = new List<itensPainel>();
            rmams = BancoPainelListagem.returnListPainel(idCollaborator, dtTimeFinal.ToString("yyyy-MM-dd"), idIndicator);

            // rmams.persona = personauserId;
            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(rmams);







        }
        // Método para serializar um DataTable em JSON
    }

    public class BancoPainelListagem
    {

        public static List<ListPainelController.itensPainel> returnListPainel(int idCollaborator, string dtTimeFinal, int idIndicator)
        {
            List<ListPainelController.itensPainel> LPCS = new List<ListPainelController.itensPainel>();

            StringBuilder stb = new StringBuilder();
            stb.Append($"DECLARE @IDCOLLABORATOR INT; SET @IDCOLLABORATOR = {idCollaborator}; ");
            stb.Append($"DECLARE @DATA DATETIME; SET @DATA = '{dtTimeFinal}'; ");
            stb.Append($"DECLARE @INDICATOR INT; SET @INDICATOR = {idIndicator}; ");
            stb.Append($" ");
            stb.Append($"            SELECT C.IDGDA_COLLABORATORS,  ");
            stb.Append($"MAX(C.NAME) AS NAME, ");
            stb.Append($" CASE ");
            stb.Append($"        WHEN MAX(CD.ID) IS NULL THEN 'NÃO' ");
            stb.Append($"        ELSE 'SIM' ");
            stb.Append($" END AS HIERARQUIA, ");
            stb.Append($" CASE ");
            stb.Append($"        WHEN MAX(CD.ID) IS NULL THEN 'NÃO' ");
            stb.Append($"        ELSE ");
            stb.Append($"           CASE ");
            stb.Append($"                WHEN MAX(CD.ACTIVE) = 'TRUE' THEN 'SIM' ");
            stb.Append($"                ELSE 'NÃO' ");
            stb.Append($"            END ");
            stb.Append($" END AS ATIVO, ");
            stb.Append($" CASE ");
            stb.Append($"        WHEN MAX(R.DELETED_AT) IS NOT NULL THEN 'SIM' ");
            stb.Append($" ");
            stb.Append($"        ELSE 'NÃO' ");
            stb.Append($" END AS REPROCESSAMENTO, ");
            stb.Append($" SUM(INPUT) - SUM(OUTPUT) AS MOEDAS_GANHAS, ");
            stb.Append($" MAX(R.FACTORSAG0) AS FATOR0, ");
            stb.Append($" MAX(R.FACTORSAG0) AS FATOR1, ");
            stb.Append($" MAX(R.INSERTED_AT) AS DATA_RECEBIMENTO_RESULTADO, ");
            stb.Append($" CASE WHEN MAX(CD.IDGDA_PERIOD) = 1 THEN MAX(HIS.GOAL) ");
            stb.Append($"      WHEN MAX(CD.IDGDA_PERIOD) = 2 THEN MAX(HIS.GOAL_NIGHT) ");
            stb.Append($"      WHEN MAX(CD.IDGDA_PERIOD) = 3 THEN MAX(HIS.GOAL_LATENIGHT) ");
            stb.Append($"END AS META_UTILIZADA_MOEDA, ");
            stb.Append($" CASE WHEN MAX(CD.IDGDA_PERIOD) = 1 THEN MAX(HIG.METRIC_MIN) ");
            stb.Append($"      WHEN MAX(CD.IDGDA_PERIOD) = 2 THEN MAX(HIG.METRIC_MIN_NIGHT) ");
            stb.Append($"      WHEN MAX(CD.IDGDA_PERIOD) = 3 THEN MAX(HIG.METRIC_MIN_LATENIGHT) ");
            stb.Append($"END AS METRICA_MINIMA, ");
            stb.Append($" MAX(HIG.GROUPID) METRICA_GRUPO, ");
            stb.Append($" CASE ");
            stb.Append($"        WHEN MAX(HIS2.ID) IS NOT NULL THEN 'SIM' ");
            stb.Append($"        ELSE 'NÃO' ");
            stb.Append($" END AS ALTEROU_METRICA ");
            stb.Append($"FROM GDA_COLLABORATORS(NOLOCK) C ");
            stb.Append($"LEFT JOIN GDA_COLLABORATORS_DETAILS(NOLOCK) AS CD ON CD.IDGDA_COLLABORATORS = C.IDGDA_COLLABORATORS AND CONVERT(DATE, CD.CREATED_AT) = @DATA ");

            stb.Append($"LEFT JOIN  ");
            stb.Append($"( ");
            stb.Append($"SELECT PCU.IDGDA_PERSONA_USER, PCU.IDGDA_COLLABORATORS FROM GDA_PERSONA_COLLABORATOR_USER (NOLOCK) PCU ");
            stb.Append($"INNER JOIN GDA_PERSONA_USER (NOLOCK) AS GPU ON GPU.IDGDA_PERSONA_USER = PCU.IDGDA_PERSONA_USER  AND IDGDA_PERSONA_USER_TYPE = 1 ");
            stb.Append($") PPP ON PPP.IDGDA_COLLABORATORS = C.IDGDA_COLLABORATORS ");

            stb.Append($"LEFT JOIN GDA_RESULT(NOLOCK) AS R ON R.IDGDA_COLLABORATORS = C.IDGDA_COLLABORATORS AND CONVERT(DATE, R.CREATED_AT) = @DATA AND R.INDICADORID = @INDICATOR ");
            stb.Append($"LEFT JOIN GDA_CHECKING_ACCOUNT(NOLOCK) AS CA ON CA.COLLABORATOR_ID = C.IDGDA_COLLABORATORS AND CONVERT(DATE, CA.RESULT_DATE) = @DATA  ");
            stb.Append($"AND CA.GDA_INDICATOR_IDGDA_INDICATOR = @INDICATOR ");
            stb.Append($"LEFT JOIN GDA_HISTORY_INDICATOR_SECTORS(NOLOCK) AS HIS ON CA.IDGDA_HISTORY_INDICATOR_SECTORS = HIS.ID ");
            stb.Append($"LEFT JOIN GDA_HISTORY_INDICATOR_GROUP(NOLOCK) AS HIG ON CA.IDGDA_HISTORY_INDICATOR_GROUP = HIG.ID ");
            stb.Append($"LEFT JOIN GDA_HISTORY_INDICATOR_SECTORS(NOLOCK) AS HIS2 ON CA.IDGDA_HISTORY_INDICATOR_SECTORS = HIS.ID AND HIS.DELETED_AT IS NOT NULL ");
            stb.Append($"WHERE PPP.IDGDA_PERSONA_USER = @IDCOLLABORATOR ");
            stb.Append($"GROUP BY C.IDGDA_COLLABORATORS, R.IDGDA_RESULT ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ListPainelController.itensPainel LPC = new ListPainelController.itensPainel();

                                //public int ID_COLLABORATOR { get; set; }
                                //public int NAME { get; set; }
                                //public int HIERARQUIA { get; set; }
                                //public int ATIVO { get; set; }
                                //public int REPROCESSAMENTO { get; set; }
                                //public int MOEDAS_GANHAS { get; set; }
                                //public int FATOR0 { get; set; }
                                //public int FATOR1 { get; set; }
                                //public int DATA_RECEBIMENTO_RESULTADO { get; set; }
                                //public int META_UTILIZADA_MOEDA { get; set; }
                                //public int METRICA_MINIMA { get; set; }
                                //public int METRICA_GRUPO { get; set; }
                                //public int ALTEROU_METRICA { get; set; }

                                LPC.ID_COLLABORATOR = reader["IDGDA_COLLABORATORS"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_COLLABORATORS"].ToString()) : 0;
                                LPC.NAME = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "";
                                LPC.HIERARQUIA = reader["HIERARQUIA"] != DBNull.Value ? reader["HIERARQUIA"].ToString() : "";
                                LPC.ATIVO = reader["ATIVO"] != DBNull.Value ? reader["ATIVO"].ToString() : "";
                                LPC.REPROCESSAMENTO = reader["REPROCESSAMENTO"] != DBNull.Value ? reader["REPROCESSAMENTO"].ToString() : "";
                                LPC.MOEDAS_GANHAS = reader["MOEDAS_GANHAS"] != DBNull.Value ? Convert.ToInt32(reader["MOEDAS_GANHAS"].ToString()) : 0;
                                LPC.FATOR0 = reader["FATOR0"] != DBNull.Value ? Convert.ToInt32(reader["FATOR0"].ToString()) : 0;
                                LPC.FATOR1 = reader["FATOR1"] != DBNull.Value ? Convert.ToInt32(reader["FATOR1"].ToString()) : 0;
                                LPC.DATA_RECEBIMENTO_RESULTADO = reader["DATA_RECEBIMENTO_RESULTADO"] != DBNull.Value ? reader["DATA_RECEBIMENTO_RESULTADO"].ToString() : "";
                                LPC.META_UTILIZADA_MOEDA = reader["META_UTILIZADA_MOEDA"] != DBNull.Value ? Convert.ToInt32(reader["META_UTILIZADA_MOEDA"].ToString()) : 0;
                                LPC.METRICA_MINIMA = reader["METRICA_MINIMA"] != DBNull.Value ? Convert.ToInt32(reader["METRICA_MINIMA"].ToString()) : 0;
                                LPC.METRICA_GRUPO = reader["METRICA_GRUPO"] != DBNull.Value ? Convert.ToInt32(reader["METRICA_GRUPO"].ToString()) : 0;
                                LPC.ALTEROU_METRICA = reader["ALTEROU_METRICA"] != DBNull.Value ? reader["ALTEROU_METRICA"].ToString() : "";

                                LPCS.Add(LPC);
                            }

                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return LPCS;

        }


    }

}