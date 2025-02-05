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
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using System.Drawing;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Bibliography;
using System.Web.Helpers;
using System.Web.ModelBinding;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class AddPersonaUserAccountController : ApiController
    {// POST: api/Results
        public class InputModel
        {
            public int IDPERSONAUSER { get; set; }  

            public List<int> ids { get; set; }
        }

        [HttpPost]
        //public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
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


            string retornoAdd = "";
            retornoAdd =  BancoAddAccountsPersona.InsertAccountPersonaUser(inputModel, collaboratorId);
            return Ok(retornoAdd);

        }
        // Método para serializar um DataTable em JSON


        public class BancoAddAccountsPersona
        {
            public static string InsertAccountPersonaUser (InputModel inputModel, int COLLABORATORID)
            {
                string retorno = "";
                int IDPERSONACOMERCIAL = inputModel.IDPERSONAUSER;
                int CREATORIDPERSONACOMERCIAL = 0;
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT IDGDA_COLLABORATORS  ");
                sb.Append("FROM GDA_PERSONA_COLLABORATOR_USER (NOLOCK) ");
                sb.Append($"WHERE IDGDA_PERSONA_USER ={inputModel.IDPERSONAUSER} ");
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    CREATORIDPERSONACOMERCIAL = Convert.ToInt32(reader["IDGDA_COLLABORATORS"].ToString());
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                    if(CREATORIDPERSONACOMERCIAL == COLLABORATORID)
                    {
                        foreach (int item in inputModel.ids)
                        {
                            StringBuilder sbInsert = new StringBuilder();
                            sbInsert.Append("INSERT INTO GDA_PERSONA_COLLABORATOR_USER ");
                            sbInsert.Append("(IDGDA_COLLABORATORS, IDGDA_PERSONA_USER) ");
                            sbInsert.Append("VALUES ");
                            sbInsert.Append($"((SELECT TOP 1 IDGDA_COLLABORATORS FROM GDA_PERSONA_COLLABORATOR_USER  WHERE IDGDA_PERSONA_USER ={item}), {IDPERSONACOMERCIAL}) ");

                            try
                            {
                                using (SqlCommand command = new SqlCommand(sbInsert.ToString(), connection))
                                {
                                    command.ExecuteNonQuery();
                                }
                            }
                            catch (Exception ex)
                            {
                            }

                        }
                        retorno = "Associação de usuarios feito com sucesso";

                    }
                    else

                    {
                        retorno = "Usuario não tem permissão para associar usuarios a essa conta";
                    }
                    connection.Close();
                }

                return retorno;

            }
        }
    }
}