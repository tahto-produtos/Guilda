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
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class SearchAccountsController : ApiController
    {// POST: api/Results
        public class returnSearchAccount
        {
            public int totalpages { get; set; }
            public List<SearchAccounts> account { get; set; }
        }
        public class SearchAccounts
        {
            public int id { get; set; }
            public string name { get; set; }
            public string url { get; set; }
            public string hierarchy { get; set; }
        }

        [HttpGet]
        public IHttpActionResult PostResultsModel(string Collaborator, int limit, int page)
        {
            int collaboratorId = 0;
            int personauserId = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }


            //collaboratorId = TokenService.InfsUsers.collaboratorId;
            List<returnSearchAccount> rmams = new List<returnSearchAccount>();
            rmams = BancoSearchAccounts.returnSearchAccount(Collaborator,limit,page);

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(rmams);
        }

        public class BancoSearchAccounts
        {
            #region Consulta CollaboratorID

            public static List<returnSearchAccount> returnSearchAccount(string Collaborator, int limit, int page)
            {
                List<returnSearchAccount> retorno = new List<returnSearchAccount>();

                //Listo todas as contas com filtro de busca ou sem.
                        returnSearchAccount existingTotalPages = new returnSearchAccount();
                        int totalInfo = Funcoes.QuantidadeAccounts(Collaborator);
                int totalpage = (int)Math.Ceiling((double)totalInfo / limit);
                int offset = (page - 1) * limit;
                        string dtAg = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                        StringBuilder stb = new StringBuilder();
                        string FilterCollaboratorID = "";
                        if (Collaborator != "")
                        {
                            FilterCollaboratorID = "AND (U.NAME LIKE '%' + @searchAccount + '%' OR U.IDGDA_PERSONA_USER = TRY_CAST(@searchAccount AS INT) OR CD.IDGDA_COLLABORATORS = TRY_CAST(@searchAccount AS INT) ) ";
                        }
                        stb.Append($"DECLARE @searchAccount NVARCHAR(100) = '{Collaborator}' ");

                        stb.Append("SELECT  U.IDGDA_PERSONA_USER,   ");
                        stb.Append("        U.NAME,   ");
                        stb.Append("        U.PICTURE,   ");
                        stb.Append("        CD.CARGO   ");
                        stb.Append("FROM    GDA_COLLABORATORS_DETAILS (NOLOCK) CD   ");
                        stb.Append("INNER JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) PCU ON PCU.IDGDA_COLLABORATORS = CD.IDGDA_COLLABORATORS   ");
                        stb.Append("INNER JOIN GDA_PERSONA_USER (NOLOCK) U ON U.IDGDA_PERSONA_USER = PCU.IDGDA_PERSONA_USER   ");
                        stb.Append("WHERE   1 = 1   ");
                        stb.Append($" AND CD.CREATED_AT >= '{dtAg}' " );
                        stb.Append($" {FilterCollaboratorID} ");
                        //stb.Append("AND     CD.ACTIVE = 'true'  ");
                        stb.Append("GROUP BY U.IDGDA_PERSONA_USER, U.NAME, U.PICTURE, CD.CARGO  ");
                        stb.Append("ORDER BY U.NAME  ");
                        stb.Append($"OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY  ");

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
                                    if (existingTotalPages.account == null)
                                    {
                                        existingTotalPages.totalpages = totalpage;
                                        existingTotalPages.account = new List<SearchAccounts>();
                                        existingTotalPages.account.Add(new SearchAccounts
                                        {
                                            id = reader["IDGDA_PERSONA_USER"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_PERSONA_USER"]) : 0,
                                            name = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "",
                                            url = reader["PICTURE"] != DBNull.Value ? reader["PICTURE"].ToString() : "",
                                            hierarchy = reader["CARGO"] != DBNull.Value ? reader["CARGO"].ToString() : "",
                                        });
                                    }
                                    else
                                    {
                                        existingTotalPages.account.Add(new SearchAccounts
                                        {
                                            id = reader["IDGDA_PERSONA_USER"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_PERSONA_USER"]) : 0,
                                            name = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "",
                                            url = reader["PICTURE"] != DBNull.Value ? reader["PICTURE"].ToString() : "",
                                            hierarchy = reader["CARGO"] != DBNull.Value ? reader["CARGO"].ToString() : "",
                                        });
                                    }

                                }

                                if (existingTotalPages.account == null)
                                {
                                    existingTotalPages.account = new List<SearchAccounts>(); 
                                }
                                    
                                retorno.Add(existingTotalPages);
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

        // Método para serializar um DataTable em JSON
    }
}