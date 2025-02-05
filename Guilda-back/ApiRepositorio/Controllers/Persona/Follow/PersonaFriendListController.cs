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

    public class returnFriendList
    { 
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string hierarchy { get; set; }
    }


    //[Authorize]
    public class PersonaFriendListController : ApiController
    {

        [HttpGet]
        [ResponseType(typeof(returnSugestFollow))]
        public IHttpActionResult GetResultsModel(string friend, bool tierList, int personaId)
        {
            List<returnFriendList> users = new List<returnFriendList>();

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

            //string sectors = inputSugestFollow.sectors == null ? "" : string.Join(",", inputSugestFollow.sectors);
            users = BancoPersonaFriendList.returnFriendLists(personauserId, tierList, friend, personaId);

            return Ok(users);
        }

    }

    public class BancoPersonaFriendList
    {
        #region Banco
        public static List<returnFriendList> returnFriendLists(int personauser, bool tierList, string friend, int personaIdEnv)
        {
            int idBuscado = 0;
            idBuscado = personaIdEnv != 0 ? personaIdEnv : personauser;

            List<returnFriendList> retorno = new List<returnFriendList>();

            //Listo todas as postagens e repostagens do banco
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    string Tier = "";
                    string dtAg = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                    StringBuilder stb = new StringBuilder();
                    string filter = "";                   
                    if (tierList == true)
                    {
                        Tier = "TOP 5 ";
                    }
                    if (friend != "") 
                    {
                        filter = "AND (U.NAME LIKE '%' + @searchAccount + '%' OR U.IDGDA_PERSONA_USER = TRY_CAST(@searchAccount AS INT) OR CD.IDGDA_COLLABORATORS = TRY_CAST(@searchAccount AS INT) ) ";
                    }
                    stb.Append($"DECLARE @searchAccount NVARCHAR(100) = '{friend}' ");
                    stb.Append($"SELECT {Tier} U.IDGDA_PERSONA_USER, U.NAME, U.PICTURE, CD.CARGO FROM GDA_COLLABORATORS_DETAILS (NOLOCK) CD   ");
                    stb.Append("INNER JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) PCU ON PCU.IDGDA_COLLABORATORS = CD.IDGDA_COLLABORATORS  ");
                    stb.Append("INNER JOIN GDA_PERSONA_USER (NOLOCK) U ON U.IDGDA_PERSONA_USER = PCU.IDGDA_PERSONA_USER   ");
                    stb.Append("WHERE 1 = 1   ");
                    stb.Append($" AND CD.CREATED_AT >= '{dtAg}'  ");
                    stb.Append("AND  EXISTS (   ");
                    stb.Append("    SELECT 1    ");
                    stb.Append("    FROM GDA_PERSONA_FOLLOWERS PF WITH(NOLOCK)   ");
                    stb.Append("    WHERE PF.IDGDA_PERSONA_USER_FOLLOWED = U.IDGDA_PERSONA_USER   ");
                    stb.Append($"    AND PF.IDGDA_PERSONA_USER = {idBuscado}  ");
                    stb.Append("    AND PF.DELETED_AT IS NULL   ");
                    stb.Append(")   ");
                    //stb.Append(" AND CD.ACTIVE = 'true' ");
                    stb.Append($" {filter} ");
                    stb.Append("GROUP BY U.IDGDA_PERSONA_USER, U.NAME, U.PICTURE, CD.CARGO   ");


                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                returnFriendList returnFriendList = new returnFriendList();
                                returnFriendList.id = reader["IDGDA_PERSONA_USER"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_PERSONA_USER"]) : 0;
                                returnFriendList.name = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "";
                                returnFriendList.url = reader["PICTURE"] != DBNull.Value ? reader["PICTURE"].ToString() : "";
                                returnFriendList.hierarchy = reader["CARGO"] != DBNull.Value ? reader["CARGO"].ToString() : "";

                                retorno.Add(returnFriendList);

                            }
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            return retorno;
        }

        #endregion
    }

}