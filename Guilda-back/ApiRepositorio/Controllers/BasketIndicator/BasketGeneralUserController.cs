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
    public class BasketGeneralUserController : ApiController
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
        [HttpGet]
        public IHttpActionResult GetResultsModel(int codCollaborator, string dtinicial, string dtfinal)
        {
            string idCol = codCollaborator.ToString();

            //VERIFICA PERFIL ADMINISTRATIVO
            bool adm = Funcoes.retornaPermissao(codCollaborator.ToString());
            if (adm == true)
            {
                //COLOCA O ID DO CEO, POIS TERA A MESMA VISÃO
                idCol = "756399";
            }

            //setor = setor != null ? setor : "";

            //REALIZA A QUERY QUE RETORNA TODAS AS INFORMAÇÕES DOS COLABORADORES QUE TIVERAM MONEITZAÇÃO.
            ReturnBasketIndicatorUser rmams = new ReturnBasketIndicatorUser();
            rmams = returnIndicatorUserMonetization(idCol, dtinicial, dtfinal);

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(rmams);
        }



        public ReturnBasketIndicatorUser returnIndicatorUserMonetization(string idCol, string dtinicial, string dtfinal)
        {
            ReturnBasketIndicatorUser rmams = new ReturnBasketIndicatorUser();
            rmams.idCollaborator = idCol;
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

            //SETORES
            string filter = "";
            //if (sector != "")
            //{
            //    filter = filter + $" AND IDGDA_SECTOR = {sector} ";
            //}

            //QUERY MOEDAS GANHAS
            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtIni);
            stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFim);
            stb.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}'; ", idCol);
            stb.Append("SELECT ISNULL(SUM(INPUT) - SUM(OUTPUT),0) AS INPUT ");
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

                    //throw;
                }
            }

            //QUERY PARA PEGAR AS MOEDAS POSSIVEIS
            stb.Clear();
            stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtIni);
            stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFim);
            stb.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}'; ", idCol);
            stb.Append(" ");
            stb.Append("SELECT MAX(R.IDGDA_COLLABORATORS) AS IDGDA_COLLABORATORS, ");
            stb.Append("       MAX(TRAB.RESULT) AS TRAB, ");
            stb.Append("       MAX(ESC.RESULT) AS ESC, ");
            stb.Append("       CONVERT(DATE, R.CREATED_AT) AS CREATED_AT, ");
            stb.Append("       CASE ");
            stb.Append("           WHEN @INPUTID = MAX(CL.IDGDA_COLLABORATORS) THEN 'AGENTE' ");
            stb.Append("           WHEN @INPUTID = MAX(CL.MATRICULA_SUPERVISOR) THEN 'SUPERVISOR' ");
            stb.Append("           WHEN @INPUTID = MAX(CL.MATRICULA_COORDENADOR) THEN 'COORDENADOR' ");
            stb.Append("           WHEN @INPUTID = MAX(CL.MATRICULA_GERENTE_II) THEN 'GERENTE_II' ");
            stb.Append("           WHEN @INPUTID = MAX(CL.MATRICULA_GERENTE_I) THEN 'GERENTE_I' ");
            stb.Append("           WHEN @INPUTID = MAX(CL.MATRICULA_DIRETOR) THEN 'DIRETOR' ");
            stb.Append("           WHEN @INPUTID = MAX(CL.MATRICULA_CEO) THEN 'CEO' ");
            stb.Append("           ELSE '' ");
            stb.Append("       END AS CARGO, ");
            stb.Append("	   MAX(CL.PERIODO) AS TURNO, ");
            stb.Append("       R.INDICADORID AS 'COD INDICADOR', ");
            stb.Append("       MAX(ISNULL(HIG1.MONETIZATION, 0)) AS META_MAXIMA, ");
            stb.Append("       MAX(ISNULL(HIG1.MONETIZATION_NIGHT, 0)) AS META_MAXIMA_NOTURNA, ");
            stb.Append("       MAX(ISNULL(HIG1.MONETIZATION_LATENIGHT, 0)) AS META_MAXIMA_MADRUGADA, ");
            stb.Append("       MAX(CL.CARGO) AS CARGO_RESULT ");
            stb.Append("FROM GDA_RESULT (NOLOCK) AS R ");
            stb.Append("LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS CB ON CB.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            stb.Append("LEFT JOIN ");
            stb.Append("  (SELECT CASE ");
            stb.Append("              WHEN IDGDA_SUBSECTOR IS NULL THEN IDGDA_SECTOR ");
            stb.Append("              ELSE IDGDA_SUBSECTOR ");
            stb.Append("          END AS IDGDA_SECTOR, ");
            stb.Append("          CREATED_AT, ");
            stb.Append("          IDGDA_COLLABORATORS, ");
            stb.Append("          ACTIVE, ");
            stb.Append("          CARGO, ");
            stb.Append("          PERIODO, ");
            stb.Append("          MATRICULA_SUPERVISOR, ");
            stb.Append("          MATRICULA_COORDENADOR, ");
            stb.Append("          MATRICULA_GERENTE_II, ");
            stb.Append("          MATRICULA_GERENTE_I, ");
            stb.Append("          MATRICULA_DIRETOR, ");
            stb.Append("          MATRICULA_CEO ");
            stb.Append("   FROM GDA_COLLABORATORS_DETAILS (NOLOCK) ");
            stb.Append("   WHERE CREATED_AT >= @DATAINICIAL ");
            stb.Append($"     AND CREATED_AT <= @DATAFINAL {filter} ) AS CL ON CL.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS ");
            stb.Append("AND CL.CREATED_AT = R.CREATED_AT ");
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
            stb.Append("AND HIG1.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND HIG1.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.Append("AND HIG1.GROUPID = 1 ");
            stb.Append("AND CONVERT(DATE,HIG1.STARTED_AT) <= R.CREATED_AT ");
            stb.Append("AND CONVERT(DATE,HIG1.ENDED_AT) >= R.CREATED_AT ");
            stb.Append("LEFT JOIN GDA_RESULT (NOLOCK) AS TRAB ON R.IDGDA_COLLABORATORS = TRAB.IDGDA_COLLABORATORS ");
            stb.Append("AND R.CREATED_AT = TRAB.CREATED_AT ");
            stb.Append("AND TRAB.INDICADORID = 2 ");
            stb.Append("LEFT JOIN GDA_RESULT (NOLOCK) AS ESC ON R.IDGDA_COLLABORATORS = ESC.IDGDA_COLLABORATORS ");
            stb.Append("AND R.CREATED_AT = ESC.CREATED_AT ");
            stb.Append("AND ESC.INDICADORID = -1 ");
            stb.Append("WHERE 1 = 1 ");
            stb.Append("  AND R.CREATED_AT >= @DATAINICIAL ");
            stb.Append("  AND R.CREATED_AT <= @DATAFINAL ");
            stb.Append("  AND R.DELETED_AT IS NULL ");
            //stb.Append("  AND CL.active = 'true' ");
            stb.Append("  AND HIG1.MONETIZATION > 0 ");
            stb.Append("  AND (CL.IDGDA_COLLABORATORS = @INPUTID ");
            stb.Append("       OR CL.MATRICULA_SUPERVISOR = @INPUTID ");
            stb.Append("       OR CL.MATRICULA_COORDENADOR = @INPUTID ");
            stb.Append("       OR CL.MATRICULA_GERENTE_II = @INPUTID ");
            stb.Append("       OR CL.MATRICULA_GERENTE_I = @INPUTID ");
            stb.Append("       OR CL.MATRICULA_DIRETOR = @INPUTID ");
            stb.Append("       OR CL.MATRICULA_CEO = @INPUTID) ");
            stb.Append($"  AND R.FACTORS <> '0.000000;0.000000' ");

            stb.Append("GROUP BY R.INDICADORID, ");
            stb.Append("         CONVERT(DATE, R.CREATED_AT) , ");
            stb.Append("         R.IDGDA_COLLABORATORS ");

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
                                bi.cargoResult = reader["CARGO_RESULT"].ToString() == "" ? "Não Informado" : reader["CARGO_RESULT"].ToString();
                                bi.codIndicator = Convert.ToInt32(reader["COD INDICADOR"].ToString());
                                bi.dataPagamento = reader["CREATED_AT"].ToString();
                                if (reader["turno"].ToString() == "DIURNO")
                                {
                                    bi.moedasPossiveis = reader["META_MAXIMA"].ToString() != "" ? int.Parse(reader["META_MAXIMA"].ToString()) : 0;
                                }
                                else if (reader["turno"].ToString() == "NOTURNO")
                                {
                                    bi.moedasPossiveis = reader["META_MAXIMA_NOTURNA"].ToString() != "" ? int.Parse(reader["META_MAXIMA_NOTURNA"].ToString()) : 0;
                                }
                                else if (reader["turno"].ToString() == "MADRUGADA")
                                {
                                    bi.moedasPossiveis = reader["META_MAXIMA_MADRUGADA"].ToString() != "" ? int.Parse(reader["META_MAXIMA_MADRUGADA"].ToString()) : 0;
                                }
                                else if (reader["turno"].ToString() == "" || reader["turno"].ToString() == null)
                                {
                                    bi.moedasPossiveis = 0;
                                }
                                bi.idcollaborator = Convert.ToInt32(reader["IDGDA_COLLABORATORS"].ToString());
                                bi.diasTrabalhados = reader["TRAB"].ToString() != "" ? reader["TRAB"].ToString() : "-";
                                bi.diasEscalados = reader["ESC"].ToString() != "" ? reader["ESC"].ToString() : "-";
                                bir.Add(bi);
                            }
                        }
                    }
                    //RETIRANDO OS RESULTADOS DO SUPERVISOR.. ENTENDER COM A TAHTO COMO FICARA ESTA PARTE.
                    bir = bir.FindAll(item => item.cargoResult == "AGENTE" || item.cargoResult == "Não Informado").ToList();

                    //CASO NÃO RETORNE INFORMAÇÃO, RETORNAR ZERADO PARA NÃO DAR ERRO PRO FRONT
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

                    //AGRUPAMENTO EM DATA E INDICADOR
                    bir = bir.GroupBy(item => new { item.dataPagamento, item.codIndicator }).Select(grupo => new basketIndicatorResults
                    {
                        cargo = grupo.First().cargo,
                        codIndicator = grupo.Key.codIndicator,
                        dataPagamento = grupo.Key.dataPagamento,
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
                                moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Sum(item => item.qtdPessoas), 2, MidpointRounding.AwayFromZero),
                                qtdPessoas = grupo.Count(),
                            }).ToList();

                        listHierarquia = listHierarquia
                            .GroupBy(item => new { item.codIndicator })
                            .Select(grupo => new basketIndicatorResults
                            {
                                codIndicator = grupo.Key.codIndicator,
                                moedasPossiveis = grupo.Sum(item => item.moedasPossiveis),
                                qtdPessoas = grupo.Count(),
                            }).ToList();

                        birFinal = listHierarquia
                            .GroupBy(item => new { item.cargo })
                            .Select(grupo => new basketIndicatorResults
                            {
                                codIndicator = 0,
                                moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis), 2, MidpointRounding.AwayFromZero),
                                qtdPessoas = grupo.Count(),
                            }).First();
                    }
                    rmams.coinsEarned = moedasGanhas;
                    rmams.coinsPossible = birFinal.moedasPossiveis;

                    //REALIZA CONTA
                    rmams.resultPercent = (rmams.coinsEarned / rmams.coinsPossible) * 100;

                    //COMO ELE NÃO TEVE COMO GANHAR NENHUMA MOEDA, ELE ATINGIU 100% DA META
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