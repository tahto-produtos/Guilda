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

    public class returnSugestFollow
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string hierarchy { get; set; }
    }

    public class inputSugestFollow
    {
        public List<int> sectors { get; set; }

    }

    //[Authorize]
    public class PersonaSugestFollowController : ApiController
    {

        [HttpPost]
        [ResponseType(typeof(returnSugestFollow))]
        public IHttpActionResult GetResultsModel([FromBody] inputSugestFollow inputSugestFollow)
        {
            List<returnSugestFollow> users = new List<returnSugestFollow>();

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

            string sectors = inputSugestFollow.sectors == null ? "" : string.Join(",", inputSugestFollow.sectors);

            users = BancoSugestFollowers.returnFollowers(personauserId, sectors);

            return Ok(users);
        }

    }


    public class BancoSugestFollowers
    {
        #region Banco

        public static List<returnSugestFollow> returnFollowers(int personauserId, string sectors)
        {

            List<returnSugestFollow> returnSugestFollows = new List<returnSugestFollow>();

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
                        filterSector = " AND (IDGDA_SECTOR IN (" + sectors + ") OR IDGDA_SUBSECTOR IN (" + sectors + ")) ";
                    }


                    stb.AppendFormat("SELECT TOP 10 U.IDGDA_PERSONA_USER, U.NAME, U.PICTURE, CD.CARGO FROM GDA_COLLABORATORS_DETAILS (NOLOCK) CD ");
                    stb.AppendFormat("INNER JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) PCU ON PCU.IDGDA_COLLABORATORS = CD.IDGDA_COLLABORATORS ");
                    stb.AppendFormat("INNER JOIN GDA_PERSONA_USER (NOLOCK) U ON U.IDGDA_PERSONA_USER = PCU.IDGDA_PERSONA_USER ");
                    stb.AppendFormat("WHERE 1 = 1 ");
                    stb.AppendFormat(" {0} ", filterSector);
                    stb.AppendFormat(" AND CD.CREATED_AT >= '{0}' ", dtAg);
                    stb.AppendFormat("AND NOT EXISTS ( ");
                    stb.AppendFormat("    SELECT 1  ");
                    stb.AppendFormat("    FROM GDA_PERSONA_FOLLOWERS PF WITH(NOLOCK) ");
                    stb.AppendFormat("    WHERE PF.IDGDA_PERSONA_USER_FOLLOWED = U.IDGDA_PERSONA_USER ");
                    stb.AppendFormat("    AND PF.IDGDA_PERSONA_USER = {0} ", personauserId);
                    stb.AppendFormat("    AND PF.DELETED_AT IS NULL ");
                    stb.AppendFormat(")  ");
                    //stb.AppendFormat("AND CD.ACTIVE = 'true' ");
                    stb.AppendFormat("GROUP BY U.IDGDA_PERSONA_USER, U.NAME, U.PICTURE, CD.CARGO ");


                    using (SqlCommand commandInsert = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                returnSugestFollow returnSugestFollow = new returnSugestFollow();
                                returnSugestFollow.id = reader["IDGDA_PERSONA_USER"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_PERSONA_USER"]) : 0;
                                returnSugestFollow.name = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "";
                                returnSugestFollow.url = reader["PICTURE"] != DBNull.Value ? reader["PICTURE"].ToString() : "";
                                returnSugestFollow.hierarchy = reader["CARGO"] != DBNull.Value ? reader["CARGO"].ToString() : "";

                                returnSugestFollows.Add(returnSugestFollow);

                            }
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }


            return returnSugestFollows;
        }

        #endregion
    }

}