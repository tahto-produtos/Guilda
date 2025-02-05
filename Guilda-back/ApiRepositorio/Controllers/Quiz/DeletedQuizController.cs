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
using static ApiRepositorio.Controllers.SendNotificationController;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class DeletedQuizController : ApiController
    {// POST: api/Results
        public class InputModel
        {
            public List<int> IDGDA_QUIZ { get; set; }
            public bool VALIDADETED { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
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

            bool ValidacaoDeleted = inputModel.VALIDADETED;

            if (ValidacaoDeleted == false)
            {
                return Ok("Você tem certeza que deseja Excluir esse Quiz?");
            }
            else
            {

                foreach (int item in inputModel.IDGDA_QUIZ)
                {              
                    #region DELETED GDA_QUIZ
                    StringBuilder sbNotification = new StringBuilder();
                sbNotification.Append("UPDATE GDA_QUIZ SET  ");
                sbNotification.Append($"DELETED_BY = {PERSONAUSERID},  ");
                sbNotification.Append($"DELETED_AT = GETDATE()  ");
                sbNotification.Append($"WHERE IDGDA_QUIZ = {item} ");
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(sbNotification.ToString(), connection))
                        {
                            command.ExecuteScalar();
                        }
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"Erro ao deletar quiz GDA_QUIZ: {ex.Message}");
                    }
                }
                }
                #endregion
                return Ok("Quiz deletado com sucesso.");
            }


        }
    }
}