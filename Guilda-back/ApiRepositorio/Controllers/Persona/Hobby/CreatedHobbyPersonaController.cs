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
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class CreatedHobbyPersonaController : ApiController
    {// POST: api/Results
        public class InputModel
        {
            public int CREATED_BY { get; set; }
            public string HOBBY { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {
            int IDCOLLABORATOR = inputModel.CREATED_BY;
            string HOBBY = inputModel.HOBBY;
            bool ValidaHobby = false;
            bool ValidaBlackList = Funcoes.FiltroBlackList(HOBBY);


            //Verifica se Temos um Hobby com o mesmo nome ja criado.
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat($"SELECT IDGDA_PERSONA_HOBBY, HOBBY FROM GDA_PERSONA_HOBBY (NOLOCK) WHERE HOBBY LIKE '%{HOBBY}%' AND DELETED_AT IS NULL ");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ValidaHobby = true;
                            }
                        }
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    return BadRequest($"Erro ao validar Hobby: {ex.Message}");
                }
            }

            if (ValidaHobby == true)
            {
                return BadRequest($"Hobby ja cadastrado em nossa lista.");
            }
            else 
            {
                StringBuilder sb2 = new StringBuilder();
                sb2.AppendFormat($"INSERT INTO GDA_PERSONA_HOBBY (CREATED_BY,HOBBY,CREATED_AT) VALUES ({IDCOLLABORATOR},'{HOBBY}',GETDATE()) ");
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(sb2.ToString(), connection))
                        {
                            command.ExecuteScalar();
                        }
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"Erro ao inserir o Hobby: {ex.Message}");
                    }
                }
                return Ok("Hobby cadastrado com sucesso.");
            }
            
        }
        // Método para serializar um DataTable em JSON
    }
}