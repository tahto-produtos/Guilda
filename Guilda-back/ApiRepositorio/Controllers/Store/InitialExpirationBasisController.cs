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
using OfficeOpenXml;
using Utilities;
using ApiC.Class;
using static ApiRepositorio.Controllers.FinancialSummaryController;
using static ApiRepositorio.Controllers.ListCatalogController;
using static ApiRepositorio.Controllers.ScoreInputController;
using DocumentFormat.OpenXml.Spreadsheet;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class InitialExpirationBasisController : ApiController
    {
        public class InputModel
        {
            public string Expiration_Home { get; set; }
            public string Expiration_Present { get; set; }
            public string CollabortorID { get; set; }
        }

        [HttpPost]
        [ResponseType(typeof(ResultsModel))]
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {
            string ExpirationHome = inputModel.Expiration_Home.ToString();
            string ExpirationPresent = inputModel.Expiration_Present.ToString();
            string CollaboratorID = inputModel.CollabortorID.ToString();
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                if (ExpirationHome != "" && ExpirationPresent != "")
                {
                try
                {
                    connection.Open();
                    StringBuilder stb = new StringBuilder();
                    //VERIFICA SE OS VALORES MANDADO NAO SÃO NULOS, CASO SEJA NÃO VAI ALTERAR O PERIODO QUE JA ESTÁ SETADO.
                    if (inputModel.Expiration_Home != "")
                    {
                        stb.Append($"UPDATE GDA_MKT_CONFIG SET DELETED_AT = GETDATE(), ALTERED_BY ='{CollaboratorID}'  WHERE IDGDA_MKT_CONFIG = (SELECT TOP 1 IDGDA_MKT_CONFIG FROM GDA_MKT_CONFIG WHERE TYPE LIKE '%HOME%' AND DELETED_AT IS NULL ORDER BY CREATED_AT DESC) ; ");
                        stb.Append($"INSERT INTO GDA_MKT_CONFIG (TYPE, VALUE,CREATED_AT,ALTERED_BY) VALUES ('VENCIMENTO_LIBERADO_HOME','{ExpirationHome}',GETDATE(),'{CollaboratorID}'); ");
                    }
                    if (inputModel.Expiration_Present != "")
                    {
                        stb.Append($"UPDATE GDA_MKT_CONFIG SET DELETED_AT = GETDATE(), ALTERED_BY ='{CollaboratorID}'  WHERE IDGDA_MKT_CONFIG = (SELECT TOP 1 IDGDA_MKT_CONFIG FROM GDA_MKT_CONFIG WHERE TYPE LIKE '%PRESENCIAL%' AND DELETED_AT IS NULL ORDER BY CREATED_AT DESC) ;");
                        stb.Append($"INSERT INTO GDA_MKT_CONFIG (TYPE, VALUE,CREATED_AT,ALTERED_BY) VALUES ('VENCIMENTO_LIBERADO_PRESENCIAL','{ExpirationPresent}',GETDATE(),'{CollaboratorID}') ");
                    }
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {
                    return BadRequest("Erro Ao Configurar Expiração de retirada dos produtos.");
                }
                connection.Close();
                return Ok("Atualização feita com sucesso.");
                }
                else
                {
                    return BadRequest("Não ha parametros para serem atualizados.");
                }
                // Use o método Ok() para retornar o objeto serializado em JSON
            }
        }
    }
}