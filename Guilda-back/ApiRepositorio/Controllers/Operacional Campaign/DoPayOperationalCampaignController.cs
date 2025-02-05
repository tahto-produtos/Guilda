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
    public class DoPayOperationalCampaignController : ApiController
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

        public class inputDoPayOperationalCampaign
        {
            public int idCampaign { get; set; }
        }


        [HttpPost]
        public IHttpActionResult GetResultsModel([FromBody] inputDoPayOperationalCampaign inputModel)
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

            PagamentoAutomaticoCampanha(inputModel.idCampaign);

            return Ok("");
        }
        // Método para serializar um DataTable em JSON

        public static void PagamentoAutomaticoCampanha(int idCampaign)
        {
            //LISTAR A CAMPANHA AUTOMATICA QUE JA FOI FINALIZADA POREM AINDA NÃO PAGA
            StringBuilder sb = new StringBuilder();

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    //PEGAR AS INFORMAÇÕES DE PAGAMENTO DA CAMPANHA
                    string NAMECAMPANHA = "";
                    int IDGDA_OPERATIONAL_CAMPAIGN = 0;
                    int IDGDA_PRODUCT = 0;
                    int RANKING = 0;
                    int MIN_PONTUATION = 0;
                    int VALUE_COINS = 0;
                    int QUANTITY_PRODUCT = 0;
                    sb.Clear();
                    sb.Append("SELECT IDGDA_OPERATIONAL_CAMPAIGN_AWARD, ");
                    sb.Append("	   OC.NAME, ");
                    sb.Append("	   OCA.IDGDA_OPERATIONAL_CAMPAIGN, ");
                    sb.Append("	   IDGDA_PRODUCT,  ");
                    sb.Append("	   RANKING,  ");
                    sb.Append("	   MIN_PONTUATION,  ");
                    sb.Append("	   VALUE_COINS, ");
                    sb.Append("	   QUANTITY_PRODUCT  ");
                    sb.Append("FROM GDA_OPERATIONAL_CAMPAIGN_AWARD (NOLOCK) OCA ");
                    sb.Append("LEFT JOIN GDA_OPERATIONAL_CAMPAIGN (NOLOCK) OC ON OC.IDGDA_OPERATIONAL_CAMPAIGN = OCA.IDGDA_OPERATIONAL_CAMPAIGN ");
                    sb.Append($"WHERE OCA.IDGDA_OPERATIONAL_CAMPAIGN = {idCampaign} ");

                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                NAMECAMPANHA = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "";
                                IDGDA_OPERATIONAL_CAMPAIGN = reader["IDGDA_OPERATIONAL_CAMPAIGN"] != DBNull.Value ? int.Parse(reader["IDGDA_OPERATIONAL_CAMPAIGN"].ToString()) : 0;
                                IDGDA_PRODUCT = reader["IDGDA_PRODUCT"] != DBNull.Value ? int.Parse(reader["IDGDA_PRODUCT"].ToString()) : 0;
                                RANKING = reader["RANKING"] != DBNull.Value ? int.Parse(reader["RANKING"].ToString()) : 0;
                                MIN_PONTUATION = reader["MIN_PONTUATION"] != DBNull.Value ? int.Parse(reader["MIN_PONTUATION"].ToString()) : 0;
                                VALUE_COINS = reader["VALUE_COINS"] != DBNull.Value ? int.Parse(reader["VALUE_COINS"].ToString()) : 0;
                                QUANTITY_PRODUCT = reader["QUANTITY_PRODUCT"] != DBNull.Value ? int.Parse(reader["QUANTITY_PRODUCT"].ToString()) : 0;
                            }
                        }
                    }
                    //VERIFICAR QUAL TIPO DE PAGAMENTO SERÁ FEITO

                    //PAGAMENTO POR MOEDAS
                    if (VALUE_COINS != 0 || IDGDA_PRODUCT == 0 || QUANTITY_PRODUCT == 0)
                    {
                        // PAGAR POR RANKING
                        if (RANKING != 0 || MIN_PONTUATION == 0)
                        {
                            Funcoes.PagamentoMoedas(idCampaign, NAMECAMPANHA, VALUE_COINS, RANKING, true);
                        }
                        // PAGAR POR PONTUACAO MINIMA
                        else
                        {
                            Funcoes.PagamentoMoedas(idCampaign, NAMECAMPANHA, VALUE_COINS, MIN_PONTUATION, false);
                        }
                    }

                    //PAGAMENTO POR PRODUTO
                    if (IDGDA_PRODUCT != 0 || QUANTITY_PRODUCT != 0 || VALUE_COINS != 0)
                    {
                        if (RANKING != 0 || MIN_PONTUATION == 0)
                        {
                            Funcoes.PagamentoProdutos(idCampaign, NAMECAMPANHA, IDGDA_PRODUCT, QUANTITY_PRODUCT, VALUE_COINS, RANKING, true);
                        }
                        else
                        {
                            Funcoes.PagamentoProdutos(idCampaign, NAMECAMPANHA, IDGDA_PRODUCT, QUANTITY_PRODUCT, VALUE_COINS, MIN_PONTUATION, false);
                        }
                    }

                    //Marcacao na campanha
                    Funcoes.marcacaoPagamentoCampanha(idCampaign);

                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
        }

    }


}
