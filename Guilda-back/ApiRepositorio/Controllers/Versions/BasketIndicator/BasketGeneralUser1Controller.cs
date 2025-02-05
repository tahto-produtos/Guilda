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
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class BasketGeneralUser1Controller : ApiController
    {
        public class ReturnBasketIndicatorUser
        {
            public string idCollaborator { get; set; }
            public int idGroup { get; set; }
            public string groupName { get; set; }
            public string groupAlias { get; set; }
            public string groupImage { get; set; }
            public double coinsEarned { get; set; }
            public double coinsPossible { get; set; }
            public double resultPercent { get; set; }

        }
        public class basketIndicatorResults
        {
            public string cargo { get; set; }
            public string cargoResult { get; set; }
            public int idcollaborator { get; set; }
            public string diasTrabalhados { get; set; }
            public string diasEscalados { get; set; }  
            public string dataPagamento { get; set; }
            public int codIndicator { get; set; }
            public double moedasGanhas { get; set; }
            public double moedasPossiveis { get; set; }
            public int qtdPessoas { get; set; }
        }
        //#region Input

        //public class PostInputModel
        //{
        //    public string groupId { get; set; }

        //    public string metricMin { get; set; }

        //}

        //#endregion

        //[HttpPost]
        //public IHttpActionResult PostResultsModel([FromBody] PostInputModel inputModel)
        //{
        //    string groupId = inputModel.groupId.ToString();
        //    string metricMin = inputModel.metricMin.ToString();

        //    //Realiza a query que retorna todas as informações dos colaboradores que tiveram moneitzação.
        //   bool ok = doConfigureBasket(groupId, metricMin);

        //    if (ok == true)
        //    {
        //        return Ok();
        //    }
        //    else
        //    {
        //        return BadRequest();
        //    }

        //}

        [HttpGet]
        public IHttpActionResult GetResultsModel(int codCollaborator, string dtinicial, string dtfinal)
        {
            string idCol = codCollaborator.ToString();

            //Verifica perfil administrativo
            bool adm = Funcoes.retornaPermissao(codCollaborator.ToString());
            if (adm == true)
            {
                //Coloca o id do CEO, pois tera a mesma visão
                idCol = "756399";
            }

            //Realiza a query que retorna todas as informações dos colaboradores que tiveram moneitzação.
            ReturnBasketIndicatorUser rmams = new ReturnBasketIndicatorUser();
            rmams = returnIndicatorUserMonetization(idCol, dtinicial, dtfinal);

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(rmams);
        }





        public ReturnBasketIndicatorUser returnIndicatorUserMonetization(string idCol, string dtinicial, string dtfinal)
        {

            ReturnBasketIndicatorUser rmams = new ReturnBasketIndicatorUser();
            rmams.idCollaborator = idCol;

            //Query ganhou

            //stb.Append("SELECT SUM(INPUT*I.WEIGHT) AS 'INPUTWEIGHT', SUM(INPUT) AS 'INPUT' FROM GDA_CHECKING_ACCOUNT (NOLOCK) AS GA ");
            //stb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS I ON GA.GDA_INDICATOR_IDGDA_INDICATOR = I.IDGDA_INDICATOR ");
            //stb.AppendFormat("WHERE COLLABORATOR_ID = {0} AND RESULT_DATE > DATEADD(DAY, -30, GETDATE())  ", idCol);
            double moedasGanhas = 0;

            DateTime dtInicio = DateTime.Now.AddDays(-30);
            string dtIni = dtInicio.ToString("yyyy-MM-dd");
            DateTime dtFinal = DateTime.Now;
            string dtFim = dtFinal.ToString("yyyy-MM-dd");
            if (dtinicial != null)
            {
                dtIni = dtinicial;
                dtFim = dtfinal;
            }

            //List<colaboradoresTrabalhados> lColaboradoresTrabalhados = returnTables.colaboradoresTrabalhados(dtIni, dtFim);

            //lColaboradoresTrabalhados = lColaboradoresTrabalhados.GroupBy(t => t.)
            //List<colaboradoresEscalados> lColaboradoresEscalados = returnTables.colaboradoresEscalados(dtIni, dtFim);



            //Query moedas ganhas
            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtIni);
            stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFim);
            stb.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}'; ", idCol);
            stb.Append("SELECT SUM(INPUT) AS INPUT ");
            stb.Append("   FROM GDA_CHECKING_ACCOUNT ");
            stb.Append("   WHERE RESULT_DATE >= @DATAINICIAL ");
            stb.Append("     AND RESULT_DATE <= @DATAFINAL ");
            stb.Append("	 AND COLLABORATOR_ID = @INPUTID ");
            stb.Append("     AND GDA_INDICATOR_IDGDA_INDICATOR IS NOT NULL ");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {

                    connection.Open();


                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                moedasGanhas = Convert.ToDouble(reader["INPUT"].ToString());
                            }
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }


            //Query para pegar as moedas possiveis
            stb.Clear();
            stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtIni);
            stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFim);
            stb.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}'; ", idCol);
            //stb.Append("SELECT COUNT(0)  AS QTD,  ");

            stb.Append("SELECT ");
            stb.Append("        MAX(R.IDGDA_COLLABORATORS) AS IDGDA_COLLABORATORS, MAX(TRAB.RESULT) AS TRAB, MAX(ESC.RESULT) AS ESC, ");
            stb.Append(" CONVERT(DATE, R.CREATED_AT) AS CREATED_AT, CASE ");
            stb.Append("		WHEN @INPUTID = MAX(CL.IDGDA_COLLABORATORS) THEN 'AGENTE' ");
            stb.Append("		WHEN @INPUTID = MAX(CL.MATRICULA_SUPERVISOR) THEN 'SUPERVISOR' ");
            stb.Append("		WHEN @INPUTID = MAX(CL.MATRICULA_COORDENADOR) THEN 'COORDENADOR' ");
            stb.Append("		WHEN @INPUTID = MAX(CL.MATRICULA_GERENTE_II) THEN 'GERENTE_II' ");
            stb.Append("		WHEN @INPUTID = MAX(CL.MATRICULA_GERENTE_I) THEN 'GERENTE_I' ");
            stb.Append("		WHEN @INPUTID = MAX(CL.MATRICULA_DIRETOR) THEN 'DIRETOR' ");
            stb.Append("		WHEN @INPUTID = MAX(CL.MATRICULA_CEO) THEN 'CEO' ");
            stb.Append("		ELSE '' END AS CARGO, ");
            stb.Append("       R.INDICADORID AS 'COD INDICADOR', ");
            //stb.Append("       SUM(HIG1.MONETIZATION) AS META_MAXIMA ");
            stb.Append("       MAX(HIG1.MONETIZATION) AS META_MAXIMA, ");
            //stb.Append("       SUM(HIG1.MONETIZATION) AS META_MAXIMA, ");
            //stb.Append("       CASE ");
            //stb.Append("           WHEN SUM(HIG1.MONETIZATION) IS NULL THEN 0 ");
            //stb.Append("           WHEN SUM(MZ.INPUT) IS NULL THEN 0 ");
            //stb.Append("           ELSE SUM(MZ.INPUT) ");
            //stb.Append("       END AS MOEDA_GANHA ");
            stb.Append(" MAX(CL.CARGO) AS CARGO_RESULT ");
            stb.Append("FROM GDA_RESULT (NOLOCK) AS R ");
            stb.Append("LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS CB ON CB.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            stb.Append("LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CL ON CL.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            stb.Append("AND CL.CREATED_AT = R.CREATED_AT ");
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
            stb.Append("AND HIG1.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND HIG1.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.Append("AND HIG1.GROUPID = 1 ");
            stb.Append("AND CONVERT(DATE,HIG1.STARTED_AT) <= R.CREATED_AT ");
            stb.Append("AND CONVERT(DATE,HIG1.ENDED_AT) >= R.CREATED_AT ");

            stb.Append("LEFT JOIN GDA_RESULT (NOLOCK) AS TRAB ON R.IDGDA_COLLABORATORS = TRAB.IDGDA_COLLABORATORS AND R.CREATED_AT = TRAB.CREATED_AT AND TRAB.INDICADORID = 2 ");
            stb.Append("LEFT JOIN GDA_RESULT (NOLOCK) AS ESC ON R.IDGDA_COLLABORATORS = ESC.IDGDA_COLLABORATORS AND R.CREATED_AT = ESC.CREATED_AT AND ESC.INDICADORID =  -1 ");
            //stb.Append("LEFT JOIN ");
            //stb.Append("  (SELECT SUM(INPUT) AS INPUT, ");
            //stb.Append("          gda_indicator_idgda_indicator, ");
            //stb.Append("          result_date, ");
            //stb.Append("          COLLABORATOR_ID ");
            //stb.Append("   FROM GDA_CHECKING_ACCOUNT ");
            //stb.Append("   WHERE RESULT_DATE >= @DATAINICIAL ");
            //stb.Append("     AND RESULT_DATE <= @DATAFINAL ");
            //stb.Append("   GROUP BY gda_indicator_idgda_indicator, ");
            //stb.Append("            result_date, ");
            //stb.Append("            COLLABORATOR_ID) AS MZ ON MZ.gda_indicator_idgda_indicator = R.INDICADORID ");
            //stb.Append("AND MZ.result_date = R.CREATED_AT ");
            //stb.Append("AND MZ.COLLABORATOR_ID = R.IDGDA_COLLABORATORS ");
            stb.Append("WHERE 1 = 1 ");
            stb.Append("  AND R.CREATED_AT >= @DATAINICIAL ");
            stb.Append("  AND R.CREATED_AT <= @DATAFINAL ");
            stb.Append("  AND R.DELETED_AT IS NULL ");
            //stb.Append("  AND CL.IDGDA_SECTOR IS NOT NULL ");
            //stb.Append("  AND CL.CARGO IS NOT NULL ");
            //stb.Append("  AND CL.HOME_BASED <> '' ");
            //stb.Append("  AND CL.active = 'true' ");
            stb.Append("  AND HIG1.MONETIZATION > 0 ");
            stb.Append("  AND ");
            stb.Append(" (CL.IDGDA_COLLABORATORS = @INPUTID OR   ");
            stb.Append("  CL.MATRICULA_SUPERVISOR = @INPUTID OR  ");
            stb.Append("  CL.MATRICULA_COORDENADOR = @INPUTID OR  ");
            stb.Append("  CL.MATRICULA_GERENTE_II = @INPUTID OR  ");
            stb.Append("  CL.MATRICULA_GERENTE_I = @INPUTID OR  ");
            stb.Append("  CL.MATRICULA_DIRETOR = @INPUTID OR  ");
            stb.Append("  CL.MATRICULA_CEO = @INPUTID) ");
            stb.Append(" AND R.FACTORS <> '0.000000;0.000000' ");
            stb.Append("GROUP BY R.INDICADORID, ");
            stb.Append("		CONVERT(DATE, R.CREATED_AT)  ");
            //adicionado
            stb.Append(", R.IDGDA_COLLABORATORS");


            //stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtIni);
            //stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFim);
            //stb.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}'; ", idCol);
            //stb.Append(" ");
            //stb.Append("SELECT MAX(CONVERT(DATE, R.CREATED_AT)) AS 'DATA DO PAGAMENTO', ");
            //stb.Append("       R.IDGDA_COLLABORATORS AS 'MATRICULA', ");
            //stb.Append("       MAX(CB.NAME) AS NAME, ");
            ////stb.Append("       MAX(CL.CARGO) AS CARGO, ");
            //stb.Append("CASE ");
            //stb.Append("    WHEN @INPUTID = MAX(CL.IDGDA_COLLABORATORS) THEN 'AGENTE' ");
            //stb.Append("    WHEN @INPUTID = MAX(CL.MATRICULA_SUPERVISOR) THEN 'SUPERVISOR' ");
            //stb.Append("    WHEN @INPUTID = MAX(CL.MATRICULA_COORDENADOR) THEN 'COORDENADOR' ");
            //stb.Append("    WHEN @INPUTID = MAX(CL.MATRICULA_GERENTE_II) THEN 'GERENTE_II' ");
            //stb.Append("    WHEN @INPUTID = MAX(CL.MATRICULA_GERENTE_I) THEN 'GERENTE_I' ");
            //stb.Append("    WHEN @INPUTID = MAX(CL.MATRICULA_DIRETOR) THEN 'DIRETOR' ");
            //stb.Append("    WHEN @INPUTID = MAX(CL.MATRICULA_CEO) THEN 'CEO' ");
            //stb.Append("    ELSE '' ");
            //stb.Append("END AS CARGO, ");
            //stb.Append("       MAX(CONVERT(DATE, R.CREATED_AT)) AS 'REFERENCIA PAGAMENTO', ");
            //stb.Append("       R.INDICADORID AS 'COD INDICADOR', ");
            //stb.Append("       MAX(HIG1.MONETIZATION) AS META_MAXIMA, ");
            //stb.Append("       CASE ");
            //stb.Append("           WHEN MAX(HIG1.MONETIZATION) IS NULL THEN 0 ");
            //stb.Append("           WHEN MAX(MZ.INPUT) IS NULL THEN 0 ");
            //stb.Append("           ELSE MAX(MZ.INPUT) ");
            //stb.Append("       END AS MOEDA_GANHA, ");
            //stb.Append("       MAX(CL.IDGDA_SECTOR) AS COD_GIP ");
            //stb.Append("FROM GDA_RESULT (NOLOCK) AS R ");
            //stb.Append("LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS CB ON CB.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            //stb.Append("LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CL ON CL.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            //stb.Append("AND CL.CREATED_AT = R.CREATED_AT ");
            //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
            //stb.Append("AND HIG1.INDICATOR_ID = R.INDICADORID ");
            //stb.Append("AND HIG1.SECTOR_ID = CL.IDGDA_SECTOR ");
            //stb.Append("AND HIG1.GROUPID = 1 ");
            //stb.Append("AND CONVERT(DATE,HIG1.STARTED_AT) <= R.CREATED_AT ");
            //stb.Append("AND CONVERT(DATE,HIG1.ENDED_AT) >= R.CREATED_AT ");
            //stb.Append("LEFT JOIN ");
            //stb.Append("  (SELECT SUM(INPUT) AS INPUT, ");
            //stb.Append("          gda_indicator_idgda_indicator, ");
            //stb.Append("          result_date, ");
            //stb.Append("          COLLABORATOR_ID ");
            //stb.Append("   FROM GDA_CHECKING_ACCOUNT ");
            //stb.Append("   WHERE RESULT_DATE >= @DATAINICIAL ");
            //stb.Append("     AND RESULT_DATE <= @DATAFINAL ");
            //stb.Append("   GROUP BY gda_indicator_idgda_indicator, ");
            //stb.Append("            result_date, ");
            //stb.Append("            COLLABORATOR_ID) AS MZ ON MZ.gda_indicator_idgda_indicator = R.INDICADORID ");
            //stb.Append("AND MZ.result_date = R.CREATED_AT ");
            //stb.Append("AND MZ.COLLABORATOR_ID = @INPUTID ");
            //stb.Append("WHERE 1 = 1 ");
            //stb.Append("  AND R.CREATED_AT >= @DATAINICIAL ");
            //stb.Append("  AND R.CREATED_AT <= @DATAFINAL ");
            //stb.Append("  AND R.DELETED_AT IS NULL ");
            //stb.Append("  AND CL.IDGDA_SECTOR IS NOT NULL ");
            //stb.Append("  AND CL.CARGO IS NOT NULL ");
            //stb.Append("  AND CL.HOME_BASED <> '' ");
            //stb.Append("  AND CL.active = 'true' ");
            //stb.Append("  AND HIG1.MONETIZATION > 0 ");
            //stb.Append("  AND ");
            //stb.Append(" (CL.IDGDA_COLLABORATORS = @INPUTID OR   ");
            //stb.Append("  CL.MATRICULA_SUPERVISOR = @INPUTID OR  ");
            //stb.Append("  CL.MATRICULA_COORDENADOR = @INPUTID OR  ");
            //stb.Append("  CL.MATRICULA_GERENTE_II = @INPUTID OR  ");
            //stb.Append("  CL.MATRICULA_GERENTE_I = @INPUTID OR  ");
            //stb.Append("  CL.MATRICULA_DIRETOR = @INPUTID OR  ");
            //stb.Append("  CL.MATRICULA_CEO = @INPUTID) ");
            //stb.Append("GROUP BY R.IDGDA_COLLABORATORS, ");
            //stb.Append("         R.INDICADORID, ");
            //stb.Append("         CONVERT(DATE, R.CREATED_AT)  ");


            List<basketIndicatorResults> bir = new List<basketIndicatorResults>();
            basketIndicatorResults birFinal = new basketIndicatorResults();

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();


                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                basketIndicatorResults bi = new basketIndicatorResults();

                                bi.cargo = reader["CARGO"].ToString();


                                if (reader["CARGO_RESULT"].ToString() == "")
                                {
                                    bi.cargoResult = "Não Informado";
                                }
                                else
                                {
                                    bi.cargoResult = reader["CARGO_RESULT"].ToString();
                                }

                                bi.codIndicator = Convert.ToInt32(reader["COD INDICADOR"].ToString());

                                bi.dataPagamento = reader["CREATED_AT"].ToString();

                                //bi.moedasGanhas = Convert.ToDouble(reader["MOEDA_GANHA"].ToString());

                                bi.moedasPossiveis = Convert.ToDouble(reader["META_MAXIMA"].ToString());

                                bi.idcollaborator = Convert.ToInt32(reader["IDGDA_COLLABORATORS"].ToString());
                                //bi.qtdPessoas = Convert.ToInt32(reader["QTD"].ToString());

                                //DateTime dataAg = DateTime.Parse(bi.dataPagamento);
                                //int idCola = int.Parse(bi.idcollaborator.ToString());

                                //int trabalhado = lColaboradoresTrabalhados.Where(t => t.data == dataAg && t.id == idCola).Select(t => t.trabalhado).DefaultIfEmpty(-1).Max();
                                //if (trabalhado != -1)
                                //{
                                //    bi.diasTrabalhados = trabalhado.ToString();
                                //}
                                //else
                                //{
                                //    bi.diasTrabalhados = "-";
                                //}
                                string trabalhado = reader["TRAB"].ToString();
                                if (trabalhado != "")
                                {
                                    bi.diasTrabalhados = trabalhado.ToString();
                                }
                                else
                                {
                                    bi.diasTrabalhados = "-";
                                }

                                //int escalado = lColaboradoresEscalados.Where(t => t.data == dataAg && t.id == idCola).Select(t => t.escalado).DefaultIfEmpty(-1).Max();
                                //if (escalado != -1)
                                //{
                                //    bi.diasEscalados = escalado.ToString();
                                //}
                                //else
                                //{
                                //    bi.diasEscalados = "-";
                                //}

                                string escalado = reader["ESC"].ToString();
                                if (escalado != "")
                                {
                                    bi.diasEscalados = escalado.ToString();
                                }
                                else
                                {
                                    bi.diasEscalados = "-";
                                }

                                bir.Add(bi);
                            }
                        }
                    }

                    bir = bir.FindAll(item => item.cargoResult == "AGENTE" || item.cargoResult == "Não Informado").ToList();

                    //Caso não retorne informação, retornar zerado para não dar erro pro front
                    if (bir.Count() == 0)
                    {
                        rmams.coinsEarned = 0;
                        rmams.coinsPossible = 0;
                        rmams.groupName = "";
                        rmams.idGroup = 0;
                        rmams.groupAlias = "";
                        rmams.groupImage = "";
                        return rmams;
                    }

                    //Agrupamento em data e indicador
                    bir = bir.GroupBy(item => new { item.dataPagamento, item.codIndicator }).Select(grupo => new basketIndicatorResults
                    {
                        cargo = grupo.First().cargo,
                        codIndicator = grupo.Key.codIndicator,
                        dataPagamento = grupo.Key.dataPagamento,
                        //moedasPossiveis = grupo.Sum(i => i.moedasPossiveis),
                        moedasPossiveis = grupo.Max(item => item.diasTrabalhados) == "1"
                        ? Math.Round(grupo.Sum(item => item.moedasPossiveis), 2, MidpointRounding.AwayFromZero)
                        : grupo.Max(item => item.diasEscalados) == "0" && grupo.Max(item => item.diasTrabalhados) == "1" ?
                        Math.Round(grupo.Sum(item => item.moedasPossiveis), 2, MidpointRounding.AwayFromZero) :
                        grupo.Max(item => item.diasEscalados) == "0" && grupo.Max(item => item.diasTrabalhados) == "0" ?
                        0 : Math.Round(grupo.Sum(item => item.moedasPossiveis), 2, MidpointRounding.AwayFromZero),

                        qtdPessoas = grupo.Count(),
                    }).ToList();


                    if (bir.First().cargo == "AGENTE")
                    {
                        birFinal = bir
                            .GroupBy(item => new { item.cargo })
                            .Select(grupo => new basketIndicatorResults
                            {
                                //moedasGanhas = grupo.Sum(item => item.moedasGanhas),
                                moedasPossiveis = grupo.Sum(item => item.moedasPossiveis),
                                qtdPessoas = grupo.Count(),
                            }).First();
                    }
                    else
                    {
                        List<basketIndicatorResults> listHierarquia = new List<basketIndicatorResults>();

                        List<basketIndicatorResults> teste = new List<basketIndicatorResults>();
                        listHierarquia = bir
                            .GroupBy(item => new { item.codIndicator, item.dataPagamento })
                            .Select(grupo => new basketIndicatorResults
                            {
                                dataPagamento = grupo.Key.dataPagamento,
                                codIndicator = grupo.Key.codIndicator,
                                // moedasGanhas = Math.Round(grupo.Sum(item => item.moedasGanhas) / grupo.Sum(item => item.qtdPessoas), 0, MidpointRounding.AwayFromZero),
                                //moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Count(), 2, MidpointRounding.AwayFromZero),
                                moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Sum(item => item.qtdPessoas), 2, MidpointRounding.AwayFromZero),
                                qtdPessoas = grupo.Count(),
                            }).ToList();

                        //listHierarquia = listHierarquia
                        //    .GroupBy(item => new {  item.dataPagamento })
                        //    .Select(grupo => new basketIndicatorResults
                        //    {
                        //        dataPagamento = grupo.Key.dataPagamento,
                        //        //codIndicator = grupo.Key.codIndicator,
                        //        moedasGanhas = grupo.Sum(item => item.moedasGanhas),
                        //        //moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Count(), 2, MidpointRounding.AwayFromZero),
                        //        moedasPossiveis = grupo.Sum(item => item.moedasPossiveis),
                        //        qtdPessoas = grupo.Count(),
                        //    }).ToList();

                        listHierarquia = listHierarquia
                            .GroupBy(item => new { item.codIndicator })
                            .Select(grupo => new basketIndicatorResults
                            {
                                codIndicator = grupo.Key.codIndicator,
                                //moedasGanhas = grupo.Sum(item => item.moedasGanhas),
                                //moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Count(), 2, MidpointRounding.AwayFromZero),
                                moedasPossiveis = grupo.Sum(item => item.moedasPossiveis),
                                qtdPessoas = grupo.Count(),
                            }).ToList();

                        birFinal = listHierarquia
                            .GroupBy(item => new { item.cargo })
                            .Select(grupo => new basketIndicatorResults
                            {
                                codIndicator = 0,
                                //moedasGanhas = Math.Round(grupo.Sum(item => item.moedasGanhas), 2, MidpointRounding.AwayFromZero),
                                moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis), 2, MidpointRounding.AwayFromZero),
                                //moedasPossiveis = grupo.Sum(item => item.moedasPossiveis),
                                qtdPessoas = grupo.Count(),
                            }).First();
                    }


                    //rmams.coinsEarned = birFinal.moedasGanhas;

                    rmams.coinsEarned = moedasGanhas;
                    rmams.coinsPossible = birFinal.moedasPossiveis;

                    #region Antigo
                    //Query Quantos ganhou
                    //using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    //{
                    //    using (SqlDataReader reader = command.ExecuteReader())
                    //    {
                    //        if (reader.Read())
                    //        {
                    //            if (reader["INPUTWEIGHT"].ToString() == "")
                    //            {
                    //                rmams.coinsEarned = 0;
                    //            }
                    //            else
                    //            {
                    //                rmams.coinsEarned = Convert.ToInt32(reader["INPUTWEIGHT"].ToString());
                    //            }                                
                    //        }
                    //    }
                    //}

                    //Query Quantos possivel


                    //int valorMaximoMoedas = 0;


                    //StringBuilder stb3 = new StringBuilder();
                    //stb3.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}'; ", idCol);
                    //stb3.Append("SELECT SUM(MONE) AS MAXIMO FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS DT ");
                    //stb3.Append("LEFT JOIN GDA_HISTORY_COLLABORATOR_SECTOR (NOLOCK) AS CS ");
                    //stb3.Append("ON CS.IDGDA_COLLABORATORS = DT.IDGDA_COLLABORATORS AND ");
                    //stb3.Append("CS.CREATED_AT = DT.CREATED_AT ");
                    //stb3.Append("LEFT JOIN GDA_RESULT (NOLOCK) AS R ON R.IDGDA_COLLABORATORS = DT.IDGDA_COLLABORATORS ");
                    //stb3.Append("AND R.CREATED_AT = DT.CREATED_AT AND R.DELETED_AT IS NULL ");
                    //stb3.Append("   LEFT JOIN ");
                    //stb3.Append("     (SELECT DATE, SECTOR_ID, INDICATOR_ID, ");
                    //stb3.Append("             SUM(MONETIZATION_G1*I.WEIGHT) AS MONE ");
                    //stb3.Append("      FROM GDA_BASKET_INDICATOR (NOLOCK) AS GA ");
                    //stb3.Append("	  INNER JOIN GDA_INDICATOR (NOLOCK) AS I ON GA.INDICATOR_ID = I.IDGDA_INDICATOR ");
                    //stb3.Append("      WHERE DATE >= CONVERT(DATE, DATEADD(DAY, -30, GETDATE())) ");
                    //stb3.Append("      GROUP BY DATE, SECTOR_ID, INDICATOR_ID) AS BI ");
                    //stb3.Append("	  ON BI.DATE = DT.CREATED_AT AND ");
                    //stb3.Append("	  BI.SECTOR_ID = CS.IDGDA_SECTOR AND ");
                    //stb3.Append("	  BI.INDICATOR_ID = R.INDICADORID ");
                    //stb3.Append("WHERE (DT.IDGDA_COLLABORATORS = @INPUTID OR  ");
                    //stb3.Append("	    DT.MATRICULA_SUPERVISOR = @INPUTID OR ");
                    //stb3.Append("		DT.MATRICULA_COORDENADOR = @INPUTID OR ");
                    //stb3.Append("		DT.MATRICULA_GERENTE_II = @INPUTID OR ");
                    //stb3.Append("		DT.MATRICULA_GERENTE_I = @INPUTID OR ");
                    //stb3.Append("		DT.MATRICULA_DIRETOR = @INPUTID OR ");
                    //stb3.Append("		DT.MATRICULA_CEO = @INPUTID) ");
                    //stb3.Append("		AND DT.CREATED_AT >= CONVERT(DATE, DATEADD(DAY, -30, GETDATE())) ");
                    ////stb3.Append("		GROUP BY DT.CREATED_AT, DT.IDGDA_COLLABORATORS");

                    //using (SqlCommand command = new SqlCommand(stb3.ToString(), connection))
                    //{
                    //    using (SqlDataReader reader = command.ExecuteReader())
                    //    {
                    //        while (reader.Read())
                    //        {
                    //            try
                    //            {
                    //                string maximo = reader["MAXIMO"].ToString();
                    //                if (maximo != "")
                    //                {
                    //                    valorMaximoMoedas = Convert.ToInt32(maximo);
                    //                }
                    //                else
                    //                {
                    //                    valorMaximoMoedas = 0;
                    //                }

                    //            }
                    //            catch (Exception ex)
                    //            {

                    //                throw;
                    //            }

                    //        }
                    //    }
                    //}


                    //DateTime today = DateTime.Now;
                    //DateTime thirtyDaysAgo = today.AddDays(-30);
                    // Realizar un bucle desde hace 30 días hasta hoy.
                    //for (DateTime date = thirtyDaysAgo; date <= today; date = date.AddDays(1))
                    //{

                    //    StringBuilder stb2 = new StringBuilder();
                    //    stb2.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}'; ", idCol);
                    //    stb2.AppendFormat("DECLARE @DATEENV DATE; SET @DATEENV = '{0}'; ", date.ToString("yyyy-MM-dd"));
                    //    stb2.Append(" ");
                    //    stb2.Append("WITH HIERARCHYCTE AS  ");
                    //    stb2.Append("  (SELECT IDGDA_HISTORY_HIERARCHY_RELATIONSHIP,  ");
                    //    stb2.Append("          CONTRACTORCONTROLID,  ");
                    //    stb2.Append("          PARENTIDENTIFICATION,  ");
                    //    stb2.Append("          IDGDA_COLLABORATORS,  ");
                    //    stb2.Append("          IDGDA_HIERARCHY,  ");
                    //    stb2.Append("          CREATED_AT,  ");
                    //    stb2.Append("          DELETED_AT,  ");
                    //    stb2.Append("          TRANSACTIONID,  ");
                    //    stb2.Append("          LEVELWEIGHT, DATE, LEVELNAME  ");
                    //    stb2.Append("   FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK)  ");
                    //    stb2.Append("   WHERE IDGDA_COLLABORATORS = @INPUTID  ");
                    //    stb2.Append("     AND [DATE] = @DATEENV  ");
                    //    stb2.Append("   UNION ALL SELECT H.IDGDA_HISTORY_HIERARCHY_RELATIONSHIP,  ");
                    //    stb2.Append("                    H.CONTRACTORCONTROLID,  ");
                    //    stb2.Append("                    H.PARENTIDENTIFICATION,  ");
                    //    stb2.Append("                    H.IDGDA_COLLABORATORS,  ");
                    //    stb2.Append("                    H.IDGDA_HIERARCHY,  ");
                    //    stb2.Append("                    H.CREATED_AT,  ");
                    //    stb2.Append("                    H.DELETED_AT,  ");
                    //    stb2.Append("                    H.TRANSACTIONID,  ");
                    //    stb2.Append("                    H.LEVELWEIGHT,  ");
                    //    stb2.Append("                    H.DATE,  ");
                    //    stb2.Append("                    H.LEVELNAME  ");
                    //    stb2.Append("   FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP H (NOLOCK)  ");
                    //    stb2.Append("   INNER JOIN HIERARCHYCTE CTE ON H.PARENTIDENTIFICATION = CTE.IDGDA_COLLABORATORS  ");
                    //    stb2.Append("   WHERE CTE.LEVELNAME <> 'AGENTE'  ");
                    //    stb2.Append("     AND CTE.[DATE] = @DATEENV )  ");
                    //    stb2.Append(" ");
                    //    stb2.Append("	 SELECT (SUM(PESSOAS)/SUM(QTD)) AS MAXIMO  FROM ");
                    //    stb2.Append("	 ( ");
                    //    stb2.Append("	SELECT GHC.IDGDA_SECTOR, COUNT(IDGDA_COLLABORATORS) AS QTD, MAX(TBL.MONE) AS M,    ");
                    //    stb2.Append("	 CASE WHEN  ");
                    //    stb2.Append("	  MAX(TBL.MONE) IS NULL THEN 0 ");
                    //    stb2.Append("	 ELSE ");
                    //    stb2.Append("	 (COUNT(IDGDA_COLLABORATORS)* MAX(TBL.MONE))  ");
                    //    stb2.Append("	 END AS PESSOAS  ");
                    //    stb2.Append("	 FROM GDA_HISTORY_COLLABORATOR_SECTOR (NOLOCK) AS GHC ");
                    //    stb2.Append("	 LEFT JOIN  ");
                    //    stb2.Append("	 ( ");
                    //    stb2.Append("	 	   SELECT SECTOR_ID, SUM(MONETIZATION_G1*I.WEIGHT) AS MONE FROM GDA_BASKET_INDICATOR (NOLOCK) AS GA ");
                    //    stb2.Append("		   INNER JOIN GDA_INDICATOR (NOLOCK) AS I ON GA.INDICATOR_ID = I.IDGDA_INDICATOR ");
                    //    stb2.Append("		   WHERE DATE = @DATEENV ");
                    //    stb2.Append("		   GROUP BY SECTOR_ID ");
                    //    stb2.Append("	 ) AS TBL ON TBL.SECTOR_ID = GHC.IDGDA_SECTOR ");
                    //    stb2.Append("	  ");
                    //    stb2.Append("	 WHERE IDGDA_COLLABORATORS IN ( ");
                    //    stb2.Append("	 SELECT DISTINCT(IDGDA_COLLABORATORS)  ");
                    //    stb2.Append("     FROM HIERARCHYCTE  ");
                    //    stb2.Append("     WHERE LEVELNAME = 'AGENTE'  ");
                    //    stb2.Append("       AND DATE = @DATEENV ");
                    //    stb2.Append("	   ) AND GHC.CREATED_AT = @DATEENV ");
                    //    stb2.Append("	   GROUP BY GHC.IDGDA_SECTOR ");
                    //    stb2.Append("	   ) AS TBL2 WHERE TBL2.PESSOAS > 0 ");


                    //    using (SqlCommand command = new SqlCommand(stb2.ToString(), connection))
                    //    {
                    //        using (SqlDataReader reader = command.ExecuteReader())
                    //        {
                    //            if (reader.Read())
                    //            {
                    //                try
                    //                {
                    //                    string maximo = reader["MAXIMO"].ToString();
                    //                    if (maximo != "")
                    //                    {
                    //                        valorMaximoMoedas = valorMaximoMoedas + Convert.ToInt32(maximo);
                    //                    }

                    //                }
                    //                catch (Exception ex)
                    //                {

                    //                    throw;
                    //                }

                    //            }
                    //        }
                    //    }
                    //}

                    //rmams.coinsPossible = valorMaximoMoedas;
                    #endregion

                    //Realiza conta
                    rmams.resultPercent = (rmams.coinsEarned / rmams.coinsPossible) * 100;

                    //Como ele não teve como ganhar nenhuma moeda, ele atingiu 100% da meta
                    if (rmams.coinsPossible == 0)
                    {
                        rmams.resultPercent = 100;
                    }

                    rmams.resultPercent = Math.Round(rmams.resultPercent, 2, MidpointRounding.AwayFromZero);

                    List<basket> lbasket = returnTables.listBasketConfiguration();
                    List<groups> lgroup = returnTables.listGroups("");
                    basket lbasket1 = lbasket.Find(l => l.group_id == 1);
                    basket lbasket2 = lbasket.Find(l => l.group_id == 2);
                    basket lbasket3 = lbasket.Find(l => l.group_id == 3);
                    basket lbasket4 = lbasket.Find(l => l.group_id == 4);
                    groups lgroup1 = lgroup.Find(l => l.id == 1);
                    groups lgroup2 = lgroup.Find(l => l.id == 2);
                    groups lgroup3 = lgroup.Find(l => l.id == 3);
                    groups lgroup4 = lgroup.Find(l => l.id == 4);

                    if (rmams.resultPercent >= lbasket1.metric_min)
                    {
                        rmams.groupName = lgroup1.name;
                        rmams.idGroup = lgroup1.id;
                        rmams.groupAlias = lgroup1.alias;
                        rmams.groupImage = lgroup1.image;
                    }
                    else if (rmams.resultPercent >= lbasket2.metric_min)
                    {
                        rmams.groupName = lgroup2.name;
                        rmams.idGroup = lgroup2.id;
                        rmams.groupAlias = lgroup2.alias;
                        rmams.groupImage = lgroup2.image;
                    }
                    else if (rmams.resultPercent >= lbasket3.metric_min)
                    {
                        rmams.groupName = lgroup3.name;
                        rmams.idGroup = lgroup3.id;
                        rmams.groupAlias = lgroup3.alias;
                        rmams.groupImage = lgroup3.image;
                    }
                    else if (rmams.resultPercent >= lbasket4.metric_min)
                    {
                        rmams.groupName = lgroup4.name;
                        rmams.idGroup = lgroup4.id;
                        rmams.groupAlias = lgroup4.alias;
                        rmams.groupImage = lgroup4.image;
                    }
                    else
                    {
                        rmams.groupName = lgroup4.name;
                        rmams.idGroup = lgroup4.id;
                        rmams.groupAlias = lgroup4.alias;
                        rmams.groupImage = lgroup4.image;
                    }

                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            return rmams;
        }




    }
}