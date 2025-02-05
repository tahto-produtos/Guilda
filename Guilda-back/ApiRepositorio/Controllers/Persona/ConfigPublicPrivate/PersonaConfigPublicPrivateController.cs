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
using static TokenService;

namespace ApiRepositorio.Controllers
{

    public class inputConfigPublicPrivate
    {
        public int idPublicPrivate { get; set; }
        public List<int> sector { get; set; }
        public List<int> subSector { get; set; }
        public List<int> period { get; set; }
        public List<int> hierarchy { get; set; }
        public List<int> group { get; set; }
        public List<int> userId { get; set; }
        public List<int> client { get; set; }
        public List<int> homeOrFloor { get; set; }
        public List<int> site { get; set; }

    }

    //[Authorize]
    public class PersonaConfigPublicPrivateController : ApiController
    {

        [HttpPost]
        [ResponseType(typeof(returnSugestFollow))]
        public IHttpActionResult GetResultsModel([FromBody] inputConfigPublicPrivate inputSugestFollow)
        {
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

            bool rlz = BancoConfigPublicPrivate.insertPublicPrivate(personauserId, inputSugestFollow);

            return Ok("Configurado com sucesso!");
        }

    }


    public class BancoConfigPublicPrivate
    {
        #region Banco

        public static bool insertPublicPrivate(int personauserId, inputConfigPublicPrivate inputConfigPublicPrivate)
        {
            bool concluded = false;

            string filter = "";
            string sectors = inputConfigPublicPrivate.sector == null ? "" : string.Join(",", inputConfigPublicPrivate.sector);
            string subSectors = inputConfigPublicPrivate.subSector == null ? "" : string.Join(",", inputConfigPublicPrivate.subSector);
            string periods = inputConfigPublicPrivate.period == null ? "" : string.Join(",", inputConfigPublicPrivate.period);
            string hierarchys = inputConfigPublicPrivate.hierarchy == null ? "" : string.Join(",", inputConfigPublicPrivate.hierarchy);
            string groups = inputConfigPublicPrivate.group == null ? "" : string.Join(",", inputConfigPublicPrivate.group);
            string userIds = inputConfigPublicPrivate.userId == null ? "" : string.Join(",", inputConfigPublicPrivate.userId);
            string clients = inputConfigPublicPrivate.client == null ? "" : string.Join(",", inputConfigPublicPrivate.client);
            string homeOrFloors = inputConfigPublicPrivate.homeOrFloor == null ? "" : string.Join(",", inputConfigPublicPrivate.homeOrFloor);
            string sites = inputConfigPublicPrivate.site == null ? "" : string.Join(",", inputConfigPublicPrivate.site);
            filter += sectors != "" ? $" AND IDGDA_SECTOR IN ({sectors}) " : "";
            filter += subSectors != "" ? $" AND IDGDA_SUBSECTOR IN ({subSectors}) " : "";
            filter += periods != "" ? $" AND P.IDGDA_PERIOD IN ({periods}) " : "";
            filter += hierarchys != "" ? $" AND H.IDGDA_HIERARCHY IN ({hierarchys}) " : "";
            //filter += groups != "" ? $" AND IDGDA_SECTOR IN ({sectors}) " : "";
            filter += userIds != "" ? $" AND PU.IDGDA_PERSONA_USER IN ({userIds}) " : "";
            filter += clients != "" ? $" AND C.IDGDA_CLIENT IN ({clients}) " : "";
            filter += homeOrFloors != "" ? $" AND HF.IDGDA_HOMEFLOOR IN ({homeOrFloors}) " : "";
            filter += sites != "" ? $" AND GS.IDGDA_SITE IN ({sites}) " : "";
            //Listo todas as postagens e repostagens do banco
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    //787450
                    //13180

                    string dtAg = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                    StringBuilder stb = new StringBuilder();
                    string filterSector = "";
                    if (sectors != "")
                    {
                        filterSector = " AND (IDGDA_SECTOR IN ('" + sectors + "') OR IDGDA_SUBSECTOR IN ('" + sectors + "')) ";
                    }

                    stb.AppendFormat("DECLARE @DATEAG DATETIME; SET @DATEAG = DATEADD(DAY, -2, GETDATE()); ");
                    stb.AppendFormat("UPDATE GDA_PERSONA_USER SET IDGDA_PERSONA_USER_VISIBILITY = {0} ", inputConfigPublicPrivate.idPublicPrivate);
                    stb.AppendFormat("WHERE IDGDA_PERSONA_USER IN ( ");
                    stb.AppendFormat("SELECT PU.IDGDA_PERSONA_USER FROM GDA_COLLABORATORS_DETAILS (NOLOCK) CD ");
                    stb.AppendFormat("LEFT JOIN GDA_PERIOD (NOLOCK) P ON P.PERIOD = CD.PERIODO ");
                    stb.AppendFormat("LEFT JOIN GDA_HOMEFLOOR (NOLOCK) HF ON HF.HOMEFLOOR = CD.HOME_BASED ");
                    stb.AppendFormat("LEFT JOIN GDA_CLIENT (NOLOCK) C ON C.IDGDA_CLIENT = CD.IDGDA_CLIENT ");
                    stb.AppendFormat("LEFT JOIN GDA_HIERARCHY (NOLOCK) H ON H.LEVELNAME = CD.CARGO ");
                    stb.AppendFormat("LEFT JOIN GDA_SITE (NOLOCK) GS ON GS.SITE = CD.SITE ");
                    stb.AppendFormat("INNER JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) PCU ON PCU.IDGDA_COLLABORATORS = CD.IDGDA_COLLABORATORS ");
                    stb.AppendFormat("INNER JOIN GDA_PERSONA_USER (NOLOCK) PU ON PU.IDGDA_PERSONA_USER = PCU.IDGDA_PERSONA_USER AND PU.IDGDA_PERSONA_USER_TYPE = 1 ");
                    stb.AppendFormat("WHERE CD.CREATED_AT >= @DATEAG ");
                    //stb.AppendFormat("AND CD.ACTIVE = 'true' ");
                    stb.AppendFormat("{0}  ", filter);
                    stb.AppendFormat("GROUP BY PU.IDGDA_PERSONA_USER  ");
                    stb.AppendFormat(") ");
                    

                    using (SqlCommand commandInsert = new SqlCommand(stb.ToString(), connection))
                    {
                        commandInsert.ExecuteNonQuery();
                        concluded = true;
                    }

                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }


            return concluded;
        }

        #endregion
    }

}