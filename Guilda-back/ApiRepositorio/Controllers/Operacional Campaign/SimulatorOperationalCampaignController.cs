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
using DocumentFormat.OpenXml.Office2019.Presentation;
using System.Threading;
using static ApiRepositorio.Controllers.ReportOperationalCampaignController;
using static ApiRepositorio.Controllers.TesteController;
using static ApiRepositorio.Controllers.IntegracaoAPIResultController;
using static ApiRepositorio.Controllers.SimulatorOperationalCampaignController;
using static TokenService;

//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class SimulatorOperationalCampaignController : ApiController
    {// POST: api/Results

        public class listSimulatorOperationalCampaign
        {
            public int HC { get; set; }
            public string INDICE { get; set; }
            public string PAGO { get; set; }
            public string FAIXA { get; set; }
            public int COINS_MES { get; set; }
            public int COINS_MENSAL { get; set; }
            public string EVOLUCAO { get; set; }
            public string FULL_POTENCIAL_COINS { get; set; }
            public string FULL_POTENCIAL_TOTAL { get; set; }
            public string TOTAL_60 { get; set; }
            public string TOTAL_CAMPANHA { get; set; }
        }
        public class Sector
        {
            public int Id { get; set; }
        }

        public class inputSimulatorOperationalCampaign
        {
            public DateTime? DTINICIO { get; set; }
            public DateTime? DTFIM { get; set; }
            public List<Sector> SECTORS { get; set; }
        }
    

    [HttpPost]
        public IHttpActionResult GetResultsModel([FromBody] inputSimulatorOperationalCampaign inputModel)
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


            if (inputModel.DTINICIO.ToString() == "" && inputModel.DTFIM.ToString() == "")
            {
                return BadRequest("Selecione uma data!");
            }          

            string dtInicial = Convert.ToDateTime(inputModel.DTINICIO).ToString("yyyy-MM-dd");
            string dtFinal = Convert.ToDateTime(inputModel.DTFIM).ToString("yyyy-MM-dd");


                DateTime dtTimeInicial = DateTime.ParseExact(dtInicial, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime dtTimeFinal = DateTime.ParseExact(dtFinal, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                TimeSpan diff = dtTimeFinal.Subtract(dtTimeInicial);
                int diferencaEmDias = (int)diff.TotalDays;
                if (diferencaEmDias > 31)
                {
                    return BadRequest("Selecionar uma data de no maximo 1 mês!");
                }
        
            
            //var jsonData = BancoSimulatorOperationalCampaign.relSimulatorOperationalCampaign(inputModel);

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok("");
        }
        // Método para serializar um DataTable em JSON


    }

    public class BancoSimulatorOperationalCampaign
    {

        //public static List<SimulatorOperationalCampaignController.listSimulatorOperationalCampaign> relSimulatorOperationalCampaign(inputSimulatorOperationalCampaign inputModel)
        //{
        //    string dtInicial = Convert.ToDateTime(inputModel.DTINICIO).ToString("yyyy-MM-dd");
        //    string dtFinal = Convert.ToDateTime(inputModel.DTFIM).ToString("yyyy-MM-dd");
        //    string sectorsAsString = string.Join(",", inputModel.SECTORS.Select(g => g.Id));
        //    List<ReportMonthAdmController.returnResponseADM> rmams = new List<ReportMonthAdmController.returnResponseADM>();
        //    rmams = ReportMonthAdmController.relMonMensal(dtInicial, dtFinal,"",sectorsAsString,"","","","","","");

        //    //List<listSimulatorOperationalCampaign> rmams = new List<listSimulatorOperationalCampaign>();
        //    //rmams = returnSimulatorOperationalCampaign(inputModel);

        //    //var jsonData = rmams.Select(item => new ReportActionEscalationController.returnResponseDay
        //    //{

        //    //}).ToList();
        //    return rmams;

        //}

        //public static List<listSimulatorOperationalCampaign> returnSimulatorOperationalCampaign(inputSimulatorOperationalCampaign inputModel)
        //{

        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("SELECT  ");

        //    List<listOperationalCampaign> lEscs = new List<listOperationalCampaign>();
        //    using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
        //    {
        //        connection.Open();

        //        using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
        //        {
        //            command.CommandTimeout = 300;
        //            using (SqlDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    listOperationalCampaign lEsc = new listOperationalCampaign();
        //                    lEsc.NOME_CAMPANHA = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "";
        //                    lEsc.CRIADO_POR = reader["CRIADO_POR"] != DBNull.Value ? reader["CRIADO_POR"].ToString() : "";
        //                    lEsc.DATA_INICIO = reader["STARTEDAT"] != DBNull.Value ? Convert.ToDateTime(reader["STARTEDAT"]).ToString("yyyy-MM-dd") : "";
        //                    lEsc.DATA_FIM = reader["ENDEDAT"] != DBNull.Value ? Convert.ToDateTime(reader["ENDEDAT"]).ToString("yyyy-MM-dd") : "";
        //                    lEsc.IDGDA_COLLABORATOR = reader["IDGDA_COLLABORATORS"] != DBNull.Value ? reader["IDGDA_COLLABORATORS"].ToString() : "";
        //                    lEsc.NOME_COLABORADOR = reader["NOME_COLABORADOR"] != DBNull.Value ? reader["NOME_COLABORADOR"].ToString() : "";
        //                    lEsc.RANKING_COLABORADOR = reader["RANKING"] != DBNull.Value ? reader["RANKING"].ToString() : "";
        //                    lEsc.STATUS_CAMPANHA = reader["STATUS_CAMPANHA"] != DBNull.Value ? reader["STATUS_CAMPANHA"].ToString() : "";
        //                    lEsc.IDGDA_INDICATOR = reader["IDGDA_INDICATOR"] != DBNull.Value ? int.Parse(reader["IDGDA_INDICATOR"].ToString()) : 0;
        //                    lEsc.NOME_INDICADOR = reader["INDICADOR"] != DBNull.Value ? reader["INDICADOR"].ToString() : "";                                                                               
        //                    lEscs.Add(lEsc);
        //                }
        //            }
        //        }
        //        connection.Close();           
        //    }
        //    List <listOperationalCampaign> retorno = new List<listOperationalCampaign>();

        //    for (int i = 0; i < lEscs.Count; i++)
        //    {
        //        listOperationalCampaign user = lEscs[i];

        //        List<ResultConsolidatedController.HomeResultConsolidated> resultadoPrimeiroDiaCampanha = new List<ResultConsolidatedController.HomeResultConsolidated>();
        //        resultadoPrimeiroDiaCampanha = ResultConsolidatedController.ReturnHomeResultConsolidated("", Convert.ToDateTime(user.DATA_INICIO).ToString("yyyy-MM-dd"), Convert.ToDateTime(user.DATA_INICIO).ToString("yyyy-MM-dd"), "", user.IDGDA_INDICATOR.ToString(), "", "", true, user.IDGDA_COLLABORATOR);

        //        List<ResultConsolidatedController.HomeResultConsolidated> resultadoDuranteCampanha = new List<ResultConsolidatedController.HomeResultConsolidated>();
        //        resultadoDuranteCampanha = ResultConsolidatedController.ReturnHomeResultConsolidated("", Convert.ToDateTime(user.DATA_INICIO).ToString("yyyy-MM-dd"), Convert.ToDateTime(user.DATA_FIM).ToString("yyyy-MM-dd"), "", user.IDGDA_INDICATOR.ToString(), "", "", true, user.IDGDA_COLLABORATOR);

        //        ResultConsolidatedController.HomeResultConsolidated primResult = new ResultConsolidatedController.HomeResultConsolidated();
        //        ResultConsolidatedController.HomeResultConsolidated segundResult = new ResultConsolidatedController.HomeResultConsolidated();
        //        primResult = resultadoPrimeiroDiaCampanha.Find(kkk => kkk.MATRICULA == user.IDGDA_COLLABORATOR.ToString() && kkk.IDINDICADOR == user.IDGDA_INDICATOR.ToString());
        //        segundResult = resultadoDuranteCampanha.Find(kkk => kkk.MATRICULA == user.IDGDA_COLLABORATOR.ToString() && kkk.IDINDICADOR == user.IDGDA_INDICATOR.ToString());

        //        ResultConsolidatedController.DoCalculateFinal(primResult);
        //        ResultConsolidatedController.DoCalculateFinal(segundResult);

        //        user.RESULTADO_INICIAL = primResult.RESULTADO.ToString();
        //        user.RESULTADO_ATUAL = segundResult.RESULTADO.ToString();
        //        double percentAltered = 0;
        //        percentAltered = segundResult.PERCENTUAL - primResult.PERCENTUAL;
        //        user.PORCENTAGEM_EVOLUCAO = percentAltered.ToString();
        //        lEscs[i] = user;
        //}

        //    return retorno;
        //}

    }
}
