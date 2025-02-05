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
using DocumentFormat.OpenXml.Office2019.Presentation;
using static ApiRepositorio.Controllers.LoadLibraryNotificationController;
using static ApiRepositorio.Controllers.LoadLibraryQuizController;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class LoadLibraryQuizController : ApiController
    {// POST: api/Results
        public class ReturnLibraryQuiz
        {
            public int totalpages { get; set; }
            public List<LoadLibraryQuiz> LoadLibraryQuiz { get; set; }
        }
        public class LoadLibraryQuiz
        {
            public int codQuiz { get; set; }
            public DateTime? createdAt { get; set; }
            public DateTime? startedAt { get; set; }
            public DateTime? endedAt { get; set; }
            public DateTime? sendedAt { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public int required { get; set; }
            public int idCreator { get; set; }
            public string nameCreator { get; set; }
            public int idResponsible { get; set; }
            public string nameResponsible { get; set; }
            public int idDemandant { get; set; }
            public string nameDemandant { get; set; }
            public string status { get; set; }
            public string Monetization { get; set; }
            public string PercentMonetization { get; set; }
        }
        public class InputLibraryQuiz
        {
            public DateTime? CREATEDATFROM { get; set; }
            public DateTime? CREATEDATTO { get; set; }
            public DateTime? STARTEDATFROM { get; set; }
            public DateTime? STARTEDATTO { get; set; }
            public DateTime? ENDEDATFROM { get; set; }
            public DateTime? ENDEDATTO { get; set; }
            public List<int> SITES { get; set; }
            public string STATUS { get; set; }
            public string WORD { get; set; }
            public string TITLE { get; set; }
            public int IDCREATOR { get; set; }
            public string DESCRIPTION { get; set; }
            public int REQUIRED { get; set; }
            public int? FLAGRELEVANCE { get; set; }
            public int LIMIT { get; set; }
            public int PAGE { get; set; }
        }


        [HttpPost]
        public IHttpActionResult GetResultsModel([FromBody] InputLibraryQuiz inputModel)
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

            ReturnLibraryQuiz returnQuizz = new ReturnLibraryQuiz();

            returnQuizz = BancoLibraryQuiz.returnLibraryQuiz(PERSONAUSERID, inputModel);



            return Ok(returnQuizz);
        }
        // Método para serializar um DataTable em JSON


    }

    public class BancoLibraryQuiz
    {

        public static string filterLibraryQuiz(LoadLibraryQuizController.InputLibraryQuiz inputModel)
        {
            string ret = "";

            try
            {

                if (inputModel.CREATEDATFROM != (DateTime?)null)
                {
                    ret = $"{ret} AND Q.CREATED_AT >= '{inputModel.CREATEDATFROM}' AND Q.CREATED_AT <= '{inputModel.CREATEDATTO}' ";
                }
                if (inputModel.STARTEDATFROM != (DateTime?)null)
                {
                    ret = $"{ret} AND Q.STARTED_AT >= '{inputModel.STARTEDATFROM}' AND Q.STARTED_AT <= '{inputModel.STARTEDATTO}' ";
                }
                if (inputModel.ENDEDATFROM != (DateTime?)null)
                {
                    ret = $"{ret} AND Q.ENDED_AT >= '{inputModel.ENDEDATFROM}' AND Q.ENDED_AT <= '{inputModel.ENDEDATTO}' ";
                }
                if (inputModel.SITES.Count > 0 )
                {
                    string sits = string.Join(", ", inputModel.SITES);
                    ret = $"{ret} AND QS.IDGDA_PERSONA_POSTS_VISIBILITY_TYPE = 9 AND QS.ID_REFERER IN ({sits}) ";
                }
                if (inputModel.STATUS != "")
                {
                    if (inputModel.STATUS == "Pendente de edição")
                    {

                        ret = $"{ret} AND Q.SENDED_AT IS NULL AND Q.ENDED_AT > GETDATE() AND (SELECT COUNT(0) FROM GDA_QUIZ_QUESTION (NOLOCK) Q2 WHERE Q2.IDGDA_QUIZ = Q.IDGDA_QUIZ AND Q2.DELETED_AT IS NULL) = 0 ";

                    }
                    else if (inputModel.STATUS == "Em andamento")
                    {
                        ret = $"{ret} AND Q.SENDED_AT IS NOT NULL AND Q.ENDED_AT > GETDATE() ";
                    }
                    else if (inputModel.STATUS == "Finalizado")
                    {
                        ret = $"{ret} AND Q.ENDED_AT < GETDATE() ";
                    }

                }
                if (inputModel.WORD != "")
                {
                    ret = $"{ret} AND (SELECT COUNT(0) FROM GDA_QUIZ_QUESTION (NOLOCK) Q2 WHERE Q2.IDGDA_QUIZ = Q.IDGDA_QUIZ AND Q2.DELETED_AT IS NULL AND Q2.QUESTION LIKE '%{inputModel.WORD}%') > 0 ";

                }
                if (inputModel.TITLE != "")
                {
                    ret = $"{ret} AND Q.TITLE LIKE '%{inputModel.TITLE}%' ";
                }
                if (inputModel.DESCRIPTION != "")
                {
                    ret = $"{ret} AND Q.DESCRIPTION LIKE '%{inputModel.DESCRIPTION}%' ";
                }
                if (inputModel.IDCREATOR != 0)
                {
                    ret = $"{ret} AND Q.CREATED_BY = {inputModel.IDCREATOR} ";
                }
                if (inputModel.REQUIRED != 0)
                {
                    ret = $"{ret} AND Q.REQUIRED = {inputModel.REQUIRED} ";
                }

            }
            catch (Exception)
            {

            }

            return ret;
        }

        public static LoadLibraryQuizController.ReturnLibraryQuiz returnLibraryQuiz(int personauserId, LoadLibraryQuizController.InputLibraryQuiz inputModel)
        {
            ReturnLibraryQuiz retorno = new ReturnLibraryQuiz();
            List<LoadLibraryQuiz> listLibraryQuiz = new List<LoadLibraryQuiz>();

            //List<LoadLibraryQuizController.ReturnLibraryQuiz> rLibraryQuiz = new List<LoadLibraryQuizController.ReturnLibraryQuiz>();

            string filter = filterLibraryQuiz(inputModel);

            string orderBy = "";
            if (inputModel.FLAGRELEVANCE != null)
            {
                if (inputModel.FLAGRELEVANCE == 1)
                {
                    orderBy = $" Q.REQUIRED, SUM(QUA.IDGDA_QUIZ_USER),  Q.CREATED_AT DESC ";
                }
                else
                {
                    orderBy = $" Q.CREATED_AT DESC  ";
                }
            }
            else
            {
                orderBy = $" Q.CREATED_AT DESC  ";
            }

            //Listo todas quiz do banco
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    int totalInfo = Funcoes.QuantidadeQuiz(filter);
                    int totalpage = (int)Math.Ceiling((double)totalInfo / inputModel.LIMIT);
                    int offset = (inputModel.PAGE - 1) * inputModel.LIMIT;

                    retorno.totalpages = totalpage;


                    StringBuilder stb = new StringBuilder();
                    stb.Append("SELECT Q.IDGDA_QUIZ, ");
                    stb.Append("       MAX(Q.TITLE) AS TITLE, ");
                    stb.Append("       MAX(Q.DESCRIPTION) AS DESCRIPTION, ");
                    stb.Append("       MAX(Q.REQUIRED) AS REQUIRED, ");
                    stb.Append("       MAX(Q.CREATED_AT) AS CREATED_AT, ");
                    stb.Append("       MAX(Q.CREATED_BY) AS CREATED_BY, ");
                    stb.Append("       MAX(Q.STARTED_AT) AS STARTED_AT, ");
                    stb.Append("       MAX(Q.ENDED_AT) AS ENDED_AT, ");
                    stb.Append("       MAX(Q.SENDED_AT) AS SENDED_AT, ");
                    stb.Append("	   MAX(Q.IDGDA_COLLABORATOR_DEMANDANT) AS IDGDA_COLLABORATOR_DEMANDANT, ");
                    stb.Append("	   MAX(Q.IDGDA_COLLABORATOR_RESPONSIBLE) AS IDGDA_COLLABORATOR_RESPONSIBLE, ");
                    stb.Append("	   MAX(PUC.NAME) AS NAME_CREATOR, ");
                    stb.Append("	   MAX(PUD.NAME) AS NAME_DEMANDANT, ");
                    stb.Append("	   MAX(PUR.NAME) AS NAME_RESPONSIBLE, ");
                    stb.Append("       MAX(Q.MONETIZATION) AS MONETIZATION, ");
                    stb.Append("       MAX(Q.PERCENT_MONETIZATION) AS PERCENT_MONETIZATION, ");
                    stb.Append("       SUM(QUA.IDGDA_QUIZ_USER) AS AMOUNT_ANSWER ");
                    stb.Append("FROM GDA_QUIZ (NOLOCK) Q ");
                    stb.Append("LEFT JOIN GDA_PERSONA_USER (NOLOCK) PUC ON PUC.IDGDA_PERSONA_USER = Q.CREATED_BY ");
                    stb.Append("LEFT JOIN GDA_PERSONA_USER (NOLOCK) PUD ON PUD.IDGDA_PERSONA_USER = Q.IDGDA_COLLABORATOR_DEMANDANT ");
                    stb.Append("LEFT JOIN GDA_PERSONA_USER (NOLOCK) PUR ON PUR.IDGDA_PERSONA_USER = Q.IDGDA_COLLABORATOR_RESPONSIBLE ");
                    stb.Append("LEFT JOIN GDA_QUIZ_SEND_FILTER (NOLOCK) QS ON QS.IDGDA_QUIZ = Q.IDGDA_QUIZ ");
                    stb.Append("LEFT JOIN GDA_QUIZ_USER (NOLOCK) QUA ON Q.IDGDA_QUIZ = QUA.IDGDA_QUIZ AND QUA.ANSWERED = 1 ");
                    stb.Append("WHERE Q.DELETED_AT IS NULL ");
                    stb.Append($"{filter} ");
                    stb.Append("GROUP BY Q.IDGDA_QUIZ, Q.CREATED_AT, Q.REQUIRED ");
                    stb.Append($"ORDER BY {orderBy} ");
                    stb.Append($"OFFSET {offset} ROWS FETCH NEXT {inputModel.LIMIT} ROWS ONLY ");

                    using (SqlCommand commandInsert = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                LoadLibraryQuizController.LoadLibraryQuiz libraryQuizz = new LoadLibraryQuizController.LoadLibraryQuiz();

                                libraryQuizz.codQuiz = reader["IDGDA_QUIZ"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_QUIZ"]) : 0;
                                libraryQuizz.createdAt = reader["CREATED_AT"] != DBNull.Value ? Convert.ToDateTime(reader["CREATED_AT"]) : (DateTime?)null;
                                libraryQuizz.startedAt = reader["STARTED_AT"] != DBNull.Value ? Convert.ToDateTime(reader["STARTED_AT"]) : (DateTime?)null;
                                libraryQuizz.endedAt = reader["ENDED_AT"] != DBNull.Value ? Convert.ToDateTime(reader["ENDED_AT"]) : (DateTime?)null;
                                libraryQuizz.sendedAt = reader["SENDED_AT"] != DBNull.Value ? Convert.ToDateTime(reader["SENDED_AT"]) : (DateTime?)null;


                                libraryQuizz.title = reader["TITLE"] != DBNull.Value ? reader["TITLE"].ToString() : "";
                                libraryQuizz.description = reader["DESCRIPTION"] != DBNull.Value ? reader["DESCRIPTION"].ToString() : "";
                                libraryQuizz.idCreator = reader["CREATED_BY"] != DBNull.Value ? Convert.ToInt32(reader["CREATED_BY"]) : 0;
                                libraryQuizz.nameCreator = reader["NAME_CREATOR"] != DBNull.Value ? reader["NAME_CREATOR"].ToString() : "";

                                libraryQuizz.idDemandant = reader["IDGDA_COLLABORATOR_DEMANDANT"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_COLLABORATOR_DEMANDANT"]) : 0;
                                libraryQuizz.nameDemandant = reader["NAME_DEMANDANT"] != DBNull.Value ? reader["NAME_DEMANDANT"].ToString() : "";

                                libraryQuizz.idResponsible = reader["IDGDA_COLLABORATOR_RESPONSIBLE"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_COLLABORATOR_RESPONSIBLE"]) : 0;
                                libraryQuizz.nameResponsible = reader["NAME_RESPONSIBLE"] != DBNull.Value ? reader["NAME_RESPONSIBLE"].ToString() : "";

                                libraryQuizz.required = reader["REQUIRED"] != DBNull.Value ? Convert.ToInt32(reader["REQUIRED"]) : 0;
  
                                if (libraryQuizz.sendedAt == null && VerificaQtdPerguntasQuiz(libraryQuizz.codQuiz) == false && libraryQuizz.endedAt > Convert.ToDateTime(DateTime.Now))
                                {
                                    libraryQuizz.status = "Pendente de edição";
                                }
                                else if (libraryQuizz.sendedAt != null && libraryQuizz.endedAt > Convert.ToDateTime(DateTime.Now))
                                {
                                    libraryQuizz.status = "Em andamento";
                                }
                                else
                                {
                                    libraryQuizz.status = "Finalizado";
                                }

                                libraryQuizz.Monetization = reader["MONETIZATION"] != DBNull.Value ? reader["MONETIZATION"].ToString() : "";
                                libraryQuizz.PercentMonetization = reader["PERCENT_MONETIZATION"] != DBNull.Value ? reader["PERCENT_MONETIZATION"].ToString() : "";

                                listLibraryQuiz.Add(libraryQuizz);
                                //rLibraryQuiz.Add(libraryQuizz);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            retorno.LoadLibraryQuiz = listLibraryQuiz;
            return retorno;
            //return rLibraryQuiz;
        }

        public static bool VerificaQtdPerguntasQuiz(int idQuiz)
        {
            bool retorno = false;
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
               
                connection.Open();
                StringBuilder stb = new StringBuilder();
                stb.Append("SELECT COUNT(0) as QTD FROM GDA_QUIZ_QUESTION NOLOCK ");
                stb.Append($"WHERE IDGDA_QUIZ = {idQuiz} ");

                using (SqlCommand commandInsert = new SqlCommand(stb.ToString(), connection))
                {
                    using (SqlDataReader reader = commandInsert.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int qtd = Convert.ToInt32(reader["QTD"].ToString());
                            if (qtd > 0) 
                            {
                            retorno =  true;    
                            }
                        }
                    }
                }
            }
        return retorno;
        }

    }
}
