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
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class VerifyQuizController : ApiController
    {// POST: api/Results
        [HttpGet]
        public IHttpActionResult PostResultsModel()
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

            List<QuizVerify> ids = returnTables.listQuizNotify(PERSONAUSERID);

            foreach (QuizVerify item2 in ids)
            {
                int? idnotification = 0;
                if (item2.IDGDA_NOTIFICATION == null)
                {
                    idnotification = SendQuizController.BancoQuiz.InsertNotificationQuiz(item2.IDGDA_QUIZ, PERSONAUSERID);
                }
                else
                {
                    idnotification = item2.IDGDA_NOTIFICATION;
                }

                int sendId = SendQuizController.BancoQuiz.InsertNotificationForUser(Convert.ToInt32(idnotification), PERSONAUSERID);

                List<SendQuizController.infsNotification> infNot = SendQuizController.BancoQuiz.getInfsNotification(Convert.ToInt32(idnotification));

                List<SendQuizController.infsNotification> infNot2 = infNot
                    .GroupBy(item => new { item.codNotification })
                    .Select(group => new SendQuizController.infsNotification
                    {
                        codNotification = group.Key.codNotification,
                        idUserSend = group.First().idUserSend,
                        urlUserSend = group.First().urlUserSend,
                        nameUserSend = group.First().nameUserSend,
                        message = group.First().message,
                        file = "",
                        files = group.Select(item => item.file).Distinct().Select(link => new urlFiles { url = link }).ToList() // files = group.Select(item => new filesListPosts { url = item.linkFile }).ToList()
                    }).ToList();

                messageReturned msgInput = new messageReturned();
                msgInput.type = "Notification";
                msgInput.data = new dataMessage();
                msgInput.data.idUserReceive = PERSONAUSERID;
                msgInput.data.idNotificationUser = sendId;
                msgInput.data.idNotification = Convert.ToInt32(idnotification);
                msgInput.data.idUserSend = infNot2.First().idUserSend;
                msgInput.data.urlUserSend = infNot2.First().urlUserSend;
                msgInput.data.nameUserSend = infNot2.First().nameUserSend;
                msgInput.data.message = infNot2.First().message;
                msgInput.data.urlFilesMessage = infNot2.First().files;

                Startup.messageQueue.Enqueue(msgInput);
            }

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok("Enviado com sucesso!");
        }
        // Método para serializar um DataTable em JSON
    }

}