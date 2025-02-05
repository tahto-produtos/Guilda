using System.Data.SqlClient;
using System.Web.Http;
using System.Web.Http.Description;
using ApiRepositorio.Models;
using System.Web;
using System;
using System.Data;
using ApiC.Class;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using static TokenService;

namespace ApiRepositorio.Controllers
{

    public class userConfigReturn
    {
        public List<int> sector { get; set; }
        public List<int> subsector { get; set; }
        public int period { get; set; }
        public int hierarchy { get; set; }
        public int group { get; set; }
        public List<int> client { get; set; }
        public int homeOrFloor { get; set; }
        public int site { get; set; }
    }


    //[Authorize]
    public class PersonaInfUserController : ApiController
    {


        //Visualiza os Posts (Tahto ou Geral

        [HttpGet]
        [ResponseType(typeof(returnQueryPosts))]
        public IHttpActionResult GetResultsModel()
        {
            List<returnQueryPosts> posts = new List<returnQueryPosts>();

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

            userConfigReturn userConfig = new userConfigReturn();

            userConfig = BancoPersonaInfUser.returnInfsUser(userConfig, collaboratorId);

            return Ok(userConfig);
        }


    }

    public class BancoPersonaInfUser
    {
        #region Banco

        public static userConfigReturn returnInfsUser(userConfigReturn user, int collaboratorId)
        {


            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    user.sector = new List<int>();
                    user.subsector = new List<int>();
                    user.client = new List<int>();


                    string dtAg = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                    StringBuilder stb = new StringBuilder();
                    stb.Append("SELECT IDGDA_SECTOR, IDGDA_SUBSECTOR, P.IDGDA_PERIOD, H.IDGDA_HIERARCHY AS HIERARCHY, IDGDA_CLIENT, ");
                    stb.Append(" HF.IDGDA_HOMEFLOOR, GS.IDGDA_SITE FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CD ");
                    stb.Append("LEFT JOIN GDA_HIERARCHY (NOLOCK) AS H ON H.LEVELNAME = CARGO ");
                    stb.Append("LEFT JOIN GDA_PERIOD (NOLOCK) AS P ON P.PERIOD = PERIODO ");
                    stb.Append("LEFT JOIN GDA_HOMEFLOOR (NOLOCK) AS HF ON HF.HOMEFLOOR = CD.HOME_BASED ");
                    stb.Append("LEFT JOIN GDA_SITE (NOLOCK) AS GS ON GS.SITE = CD.SITE ");
                    stb.AppendFormat("WHERE CD.CREATED_AT >= '{0}' AND IDGDA_COLLABORATORS = {1} ", dtAg, collaboratorId);

                    using (SqlCommand commandInsert = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            if (reader.Read())
                            {

                                user.period = reader["IDGDA_PERIOD"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_PERIOD"]) : 0;
                                user.hierarchy = reader["HIERARCHY"] != DBNull.Value ? Convert.ToInt32(reader["HIERARCHY"]) : 0;
                                user.homeOrFloor = reader["IDGDA_HOMEFLOOR"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_HOMEFLOOR"]) : 0;
                                user.site = reader["IDGDA_SITE"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_SITE"]) : 0;
                            }
                        }
                    }


                    stb.Clear();
                    stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtAg);
                    //stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtAg);
                    stb.AppendFormat("DECLARE @ID INT; SET @ID = {0}; ", collaboratorId);
                    stb.Append("SELECT DISTINCT(GCD.IDGDA_SECTOR) AS IDGDA_SECTOR FROM GDA_COLLABORATORS_DETAILS AS GCD ");
                    stb.Append("LEFT JOIN GDA_SECTOR AS GS ON GS.IDGDA_SECTOR = GCD.IDGDA_SECTOR ");
                    stb.Append("WHERE (GCD.IDGDA_COLLABORATORS = @ID OR GCD.MATRICULA_COORDENADOR = @ID OR GCD.MATRICULA_GERENTE_II = @ID OR GCD.MATRICULA_GERENTE_I = @ID OR GCD.MATRICULA_DIRETOR = @ID) ");
                    stb.Append("AND GCD.CREATED_AT >= @DATAINICIAL AND GCD.IDGDA_SECTOR IS NOT NULL AND GS.SECTOR = 1 AND CARGO = 'AGENTE' ");

                    using (SqlCommand commandInsert = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                user.sector.Add(reader["IDGDA_SECTOR"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_SECTOR"]) : 0);
                            }
                        }
                    }

                    stb.Clear();
                    stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtAg);
                    stb.AppendFormat("DECLARE @ID INT; SET @ID = {0}; ", collaboratorId);
                    stb.Append("SELECT DISTINCT(GCD.IDGDA_SUBSECTOR), GS.NAME FROM GDA_COLLABORATORS_DETAILS AS GCD ");
                    stb.Append("LEFT JOIN GDA_SECTOR AS GS ON GS.IDGDA_SECTOR = GCD.IDGDA_SECTOR ");
                    stb.Append("WHERE (GCD.IDGDA_COLLABORATORS = @ID OR GCD.MATRICULA_COORDENADOR = @ID OR GCD.MATRICULA_GERENTE_II = @ID OR GCD.MATRICULA_GERENTE_I = @ID OR GCD.MATRICULA_DIRETOR = @ID) ");
                    stb.Append("AND GCD.CREATED_AT >= @DATAINICIAL AND GCD.IDGDA_SUBSECTOR IS NOT NULL AND GS.SUBSECTOR = 1 AND CARGO = 'AGENTE' ");


                    using (SqlCommand commandInsert = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                user.subsector.Add(reader["IDGDA_SUBSECTOR"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_SUBSECTOR"]) : 0);
                            }
                        }
                    }

                    stb.Clear();
                    stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtAg);
                    stb.AppendFormat("DECLARE @ID INT; SET @ID = {0}; ", collaboratorId);
                    stb.AppendFormat("SELECT DISTINCT(GCD.IDGDA_CLIENT) FROM GDA_COLLABORATORS_DETAILS AS GCD ");
                    stb.AppendFormat("WHERE (GCD.IDGDA_COLLABORATORS = @ID OR GCD.MATRICULA_COORDENADOR = @ID OR GCD.MATRICULA_GERENTE_II = @ID OR GCD.MATRICULA_GERENTE_I = @ID OR GCD.MATRICULA_DIRETOR = @ID) ");
                    stb.AppendFormat("AND GCD.CREATED_AT >= @DATAINICIAL AND GCD.IDGDA_CLIENT IS NOT NULL AND CARGO = 'AGENTE' ");

                    using (SqlCommand commandInsert = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                user.client.Add(reader["IDGDA_CLIENT"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_CLIENT"]) : 0);
                            }
                        }
                    }

                }
                catch (Exception)
                {

                }
                connection.Close();
            }

            return user;
        }
    
        #endregion
    }
}