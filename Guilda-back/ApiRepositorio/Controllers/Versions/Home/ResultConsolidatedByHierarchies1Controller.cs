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
using System.Runtime.ConstrainedExecution;
using System.Web.UI.WebControls;
using static ApiRepositorio.Controllers.BasketGeneralUserController;
using System.Text.RegularExpressions;
using static ApiRepositorio.Controllers.ResultConsolidatedByHierarchiesController;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ResultConsolidatedByHierarchies1Controller : ApiController
    {
        public class Sector
        {
            public int Id { get; set; }
        }
        public class Indicator
        {
            public int Id { get; set; }
        }
        public class Hierarchy
        {
            public int Id { get; set; }
        }
        public class InputModel
        {
            public string codCollaborator { get; set; }
            public DateTime dataInicial { get; set; }
            public DateTime dataFinal { get; set; }
            public string Hierarchies { get; set; }
            public List<Sector> Sectors { get; set; }
            public List<Indicator> Indicators { get; set; }
            //public List<Hierarchy> Hierarchies { get; set; }
        }
        public class ConsolidatedByHierarchies
        {
            public string MATRICULA { get; set; }
            public string CARGO { get; set; }
            public string IDINDICADOR { get; set; }
            public string INDICADOR { get; set; }
            public double META_MAXIMA_MOEDAS { get; set; }
            public double MOEDA_GANHA { get; set; }
            public double FACTOR0 { get; set; }
            public double FACTOR1 { get; set; }
            public double META { get; set; }
            public string CONTA { get; set; }
            public string BETTER { get; set; }
            public double RESULTADO { get; set; }
            public double PERCENTUAL { get; set; }
            public double QTD { get; set; }
            public string DATAPAGAMENTO { get; set; }
            public double min1 { get; set; }
            public double min2 { get; set; }
            public double min3 { get; set; }
            public double min4 { get; set; }
            public string GRUPO { get; set; }
        }
        public class returnResponseDay
        {
            public string MATRICULA { get; set; }
            public string CARGO { get; set; }
            public string IDINDICADOR { get; set; }
            public string INDICADOR { get; set; }
            public double META { get; set; }
            public double RESULTADO { get; set; }
            public double PERCENTUAL { get; set; }
            public double META_MAXIMA_MOEDAS { get; set; }
            public double MOEDA_GANHA { get; set; }
            public string GRUPO { get; set; }
        }
        public List<ConsolidatedByHierarchies> ReturnHomeResultConsolidated(string idCol, string dtinicial, string dtfinal, string hierarchies, string indicators, string sectors)
        {
            string filter = "";
            ConsolidatedByHierarchies rmams = new ConsolidatedByHierarchies();
            rmams.MATRICULA = idCol;

            //COLABORADOR
            if (hierarchies.ToString().Contains("1"))
            {
                filter = "AND CL.IDGDA_COLLABORATORS = @INPUTID";
            }
            //SUPERVISOR
            if (hierarchies.ToString().Contains("2"))
            {
                filter = "AND CL.MATRICULA_SUPERVISOR = @INPUTID";
            }
            //COORDENADOR
            if (hierarchies.ToString().Contains("3"))
            {
                filter = "AND CL.MATRICULA_COORDENADOR = @INPUTID";
            }
            //GERENTE_II
            if (hierarchies.ToString().Contains("4"))
            {
                filter = "AND CL.MATRICULA_GERENTE_II = @INPUTID";
            }
            //GERENTE_I
            if (hierarchies.ToString().Contains("5"))
            {
                filter = "AND CL.MATRICULA_GERENTE_I = @INPUTID";
            }
            //DIRETOR
            if (hierarchies.ToString().Contains("6"))
            {
                filter = "AND CL.MATRICULA_DIRETOR = @INPUTID";
            }
            //CEO
            if (hierarchies.ToString().Contains("7"))
            {
                filter = "AND CL.MATRICULA_CEO = @INPUTID";
            }
            //SETORES
            if (sectors != "")
            {
                filter = filter + $" AND CL.IDGDA_SECTOR IN ({sectors}) ";
            }
            //INDICADORES
            if (indicators != "")
            {
                filter = filter + $" AND R.INDICADORID IN ({indicators}) ";
            }
            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtinicial);
            stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtfinal);
            stb.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}'; ", idCol);
            stb.Append("SELECT  ");
            stb.Append("       CONVERT(DATE, R.CREATED_AT) AS CREATED_AT, ");
            stb.Append("	   COUNT(0) AS QTD, ");
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
            stb.Append("       R.INDICADORID AS 'COD INDICADOR', ");
            stb.Append("	   MAX(IT.NAME) AS 'INDICADOR', ");
            stb.Append("	   SUM(ISNULL(HIS.GOAL,0)) AS META, ");
            stb.Append("	   MAX(ISNULL(HIG1.METRIC_MIN,0)) AS MIN1, ");
            stb.Append("       MAX(ISNULL(HIG2.METRIC_MIN,0)) AS MIN2, ");
            stb.Append("       MAX(ISNULL(HIG3.METRIC_MIN,0)) AS MIN3, ");
            stb.Append("       MAX(ISNULL(HIG4.METRIC_MIN,0)) AS MIN4, ");
            stb.Append("       CASE ");
            stb.Append("           WHEN MAX(ME.EXPRESSION) IS NULL THEN '(#FATOR1/#FATOR0)' ");
            stb.Append("           ELSE MAX(ME.EXPRESSION) ");
            stb.Append("       END AS CONTA, ");
            stb.Append("       CASE ");
            stb.Append("           WHEN MAX(IT.CALCULATION_TYPE) IS NULL THEN 'BIGGER_BETTER' ");
            stb.Append("           ELSE MAX(IT.CALCULATION_TYPE) ");
            stb.Append("       END AS BETTER, ");
            stb.Append("	   sUM(F1.FACTOR) AS FACTOR0, ");
            stb.Append("       SUM(F2.FACTOR) AS FACTOR1, ");
            stb.Append("       SUM(ISNULL(HIG1.MONETIZATION,0)) AS META_MAXIMA_MOEDA, ");
            stb.Append("       CASE ");
            stb.Append("           WHEN SUM(HIG1.MONETIZATION) IS NULL THEN 0 ");
            stb.Append("           WHEN SUM(MZ.INPUT) IS NULL THEN 0 ");
            stb.Append("           ELSE SUM(MZ.INPUT) ");
            stb.Append("       END AS MOEDA_GANHA ");
            stb.Append(" ");
            stb.Append("FROM GDA_RESULT (NOLOCK) AS R ");
            stb.Append("LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS CB ON CB.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            stb.Append("LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CL ON CL.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            stb.Append("AND CL.CREATED_AT = R.CREATED_AT ");
            stb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS IT ON IT.IDGDA_INDICATOR = R.INDICADORID ");
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
            stb.Append("AND HIG1.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND HIG1.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.Append("AND HIG1.GROUPID = 1 ");
            stb.Append("AND CONVERT(DATE,HIG1.STARTED_AT) <= R.CREATED_AT ");
            stb.Append("AND CONVERT(DATE,HIG1.ENDED_AT) >= R.CREATED_AT ");
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG2 ON HIG2.DELETED_AT IS NULL ");
            stb.Append("AND HIG2.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND HIG2.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.Append("AND HIG2.GROUPID = 2 ");
            stb.Append("AND CONVERT(DATE,HIG2.STARTED_AT) <= R.CREATED_AT ");
            stb.Append("AND CONVERT(DATE,HIG2.ENDED_AT) >= R.CREATED_AT ");
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG3 ON HIG3.DELETED_AT IS NULL ");
            stb.Append("AND HIG3.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND HIG3.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.Append("AND HIG3.GROUPID = 3 ");
            stb.Append("AND CONVERT(DATE,HIG3.STARTED_AT) <= R.CREATED_AT ");
            stb.Append("AND CONVERT(DATE,HIG3.ENDED_AT) >= R.CREATED_AT ");
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG4 ON HIG4.DELETED_AT IS NULL ");
            stb.Append("AND HIG4.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND HIG4.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.Append("AND HIG4.GROUPID = 4 ");
            stb.Append("AND CONVERT(DATE,HIG4.STARTED_AT) <= R.CREATED_AT ");
            stb.Append("AND CONVERT(DATE,HIG4.ENDED_AT) >= R.CREATED_AT ");
            stb.Append("LEFT JOIN GDA_FACTOR (NOLOCK) AS F1 ON F1.IDGDA_RESULT = R.IDGDA_RESULT ");
            stb.Append("AND F1.[INDEX] = 1 ");
            stb.Append("LEFT JOIN GDA_FACTOR (NOLOCK) AS F2 ON F2.IDGDA_RESULT = R.IDGDA_RESULT ");
            stb.Append("AND F2.[INDEX] = 2 ");
            stb.Append("LEFT JOIN GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME ON HME.INDICATORID = R.INDICADORID ");
            stb.Append("AND HME.deleted_at IS NULL ");
            stb.Append("LEFT JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON ME.ID = HME.MATHEMATICALEXPRESSIONID ");
            stb.Append("AND ME.DELETED_AT IS NULL ");
            stb.Append("LEFT JOIN ");
            stb.Append("  (SELECT SUM(INPUT) - SUM(OUTPUT) AS INPUT, ");
            stb.Append("          gda_indicator_idgda_indicator, ");
            stb.Append("          result_date, ");
            stb.Append("          COLLABORATOR_ID ");
            stb.Append("   FROM GDA_CHECKING_ACCOUNT ");
            stb.Append("   WHERE RESULT_DATE >= @DATAINICIAL ");
            stb.Append("     AND RESULT_DATE <= @DATAFINAL ");
            stb.Append("   GROUP BY gda_indicator_idgda_indicator, ");
            stb.Append("            result_date, ");
            stb.Append("            COLLABORATOR_ID) AS MZ ON MZ.gda_indicator_idgda_indicator = R.INDICADORID ");
            stb.Append("AND MZ.result_date = R.CREATED_AT ");
            stb.Append("AND MZ.COLLABORATOR_ID = R.IDGDA_COLLABORATORS ");
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) AS HIS ON HIS.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND HIS.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.Append("AND CONVERT(DATE,HIS.STARTED_AT) <= R.CREATED_AT ");
            stb.Append("AND CONVERT(DATE,HIS.ENDED_AT) >= R.CREATED_AT ");
            stb.Append("AND HIS.DELETED_AT IS NULL ");
            stb.Append(" ");
            stb.Append("WHERE 1 = 1 ");
            stb.Append("  AND R.CREATED_AT >= @DATAINICIAL ");
            stb.Append("  AND R.CREATED_AT <= @DATAFINAL ");
            stb.Append("  AND R.DELETED_AT IS NULL ");
            stb.Append("  AND CL.IDGDA_SECTOR IS NOT NULL ");
            stb.Append("  AND CL.CARGO IS NOT NULL ");
            stb.Append("  AND CL.HOME_BASED <> '' ");
            //stb.Append("  AND CL.active = 'true' ");
            stb.Append("AND R.FACTORS <> '0.000000;0.000000'  ");
            stb.Append("AND R.FACTORS <> '0.000000; 0.000000' ");
            stb.AppendFormat("  {0}  ", filter);
            stb.Append("GROUP BY R.INDICADORID, ");
            stb.Append("         CONVERT(DATE, R.CREATED_AT) ");

            List<ConsolidatedByHierarchies> Listhrc = new List<ConsolidatedByHierarchies>();
           
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ConsolidatedByHierarchies hrc = new ConsolidatedByHierarchies();
                            hrc.MATRICULA = idCol;
                            hrc.CARGO = reader["CARGO"].ToString();
                            hrc.IDINDICADOR = reader["COD INDICADOR"].ToString();
                            hrc.INDICADOR = reader["INDICADOR"].ToString();
                            hrc.META = Double.Parse(reader["META"].ToString());
                            hrc.min1 = reader["min1"].ToString() != "" ? double.Parse(reader["min1"].ToString()) : 0;
                            hrc.min2 = reader["min2"].ToString() != "" ? double.Parse(reader["min2"].ToString()) : 0;
                            hrc.min3 = reader["min3"].ToString() != "" ? double.Parse(reader["min3"].ToString()) : 0;
                            hrc.min4 = reader["min4"].ToString() != "" ? double.Parse(reader["min4"].ToString()) : 0;
                            hrc.CONTA = reader["CONTA"].ToString();
                            hrc.BETTER = reader["BETTER"].ToString();
                            hrc.FACTOR0 = Double.Parse(reader["FACTOR0"].ToString());
                            hrc.FACTOR1 = Double.Parse(reader["FACTOR1"].ToString());
                            hrc.META_MAXIMA_MOEDAS = Double.Parse(reader["META_MAXIMA_MOEDA"].ToString());
                            hrc.MOEDA_GANHA = Convert.ToDouble(reader["MOEDA_GANHA"].ToString());
                            hrc.QTD = Double.Parse(reader["QTD"].ToString());
                            hrc.DATAPAGAMENTO = reader["CREATED_AT"].ToString();
                            Listhrc.Add(hrc);
                        }
                    }
                }

                connection.Close();
            }
            return Listhrc;
        }
        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {
            string dtInicial = inputModel.dataInicial.ToString("yyyy-MM-dd");
            string dtFinal = inputModel.dataFinal.ToString("yyyy-MM-dd");
            string codCollaborator = inputModel.codCollaborator.ToString();
            string hierarchiesAsString = inputModel.Hierarchies.ToString();
            string sectorsAsString = string.Join(",", inputModel.Sectors.Select(g => g.Id));
            //string hierarchiesAsString = string.Join(",", inputModel.Hierarchies.Select(g => g.Id));
            string indicatorsAsString = string.Join(",", inputModel.Indicators.Select(g => g.Id));

            //Verifica perfil administrativo
            bool adm = Funcoes.retornaPermissao(codCollaborator.ToString());
            if (adm == true)
            {
                //Coloca o id do CEO, pois tera a mesma visão
                codCollaborator = "756399";
            }

            //Setar Filtro de Consolidado somente para um mes
            DateTime dtTimeInicial = DateTime.ParseExact(dtInicial, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dtTimeFinal = DateTime.ParseExact(dtFinal, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            TimeSpan diff = dtTimeFinal.Subtract(dtTimeInicial);
            int diferencaEmDias = (int)diff.TotalDays;
            if (diferencaEmDias > 31)
            {
                return BadRequest("Selecionar uma data de no maximo 1 mês!");
            }

            //Realiza a query que retorna todas as informações consolidada do colaborador filtrado.
            List<ConsolidatedByHierarchies> rmams = new List<ConsolidatedByHierarchies>();
            rmams = ReturnHomeResultConsolidated(codCollaborator, dtInicial, dtFinal, hierarchiesAsString, indicatorsAsString, sectorsAsString);

            // ORDERAR POR INDICADOR ORDER DESCRESENTE DATA DE PAGAMENTO.
            rmams = rmams.OrderBy(r => r.IDINDICADOR).OrderByDescending(r => r.DATAPAGAMENTO).ToList();

            //AGRUPAR POR INDICADOR REALIZANDO O PONTERAMENTO DAS MOEDAS MAXIMAS COM MOEDAS GANHAS
            List<ConsolidatedByHierarchies> retorno = new List<ConsolidatedByHierarchies>();
            retorno = rmams.GroupBy(d => new { d.IDINDICADOR }).Select(item => new ConsolidatedByHierarchies
            {
                MATRICULA = item.First().MATRICULA,
                CARGO = item.First().CARGO,
                IDINDICADOR = item.Key.IDINDICADOR,
                INDICADOR = item.First().INDICADOR,
                QTD = item.Count(d => d.QTD > 0),
                META = item.Sum(d => d.META) / item.Sum(d => d.QTD),
                FACTOR0 = item.Sum(d => d.FACTOR0),
                FACTOR1 = item.Sum(d => d.FACTOR1),
                //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                //min1 = grupo.Sum(item2 => item2.min1),
                //min2 = grupo.Sum(item2 => item2.min2),
                //min3 = grupo.Sum(item2 => item2.min3),
                //min4 = grupo.Sum(item2 => item2.min4),
                min1 = item.First().min1,
                min2 = item.First().min2,
                min3 = item.First().min3,
                min4 = item.First().min4,
                CONTA = item.First().CONTA,
                BETTER = item.First().BETTER,
                META_MAXIMA_MOEDAS = Math.Round(item.Sum(d => d.META_MAXIMA_MOEDAS) / item.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                MOEDA_GANHA = Math.Round(item.Sum(d => d.MOEDA_GANHA) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
            }).ToList();

            for (int i = 0; i < retorno.Count; i++)
            {
                ConsolidatedByHierarchies consolidado = retorno[i];
                try
                {
                    string contaAg = consolidado.CONTA.Replace("#fator0", consolidado.FACTOR0.ToString()).Replace("#fator1", consolidado.FACTOR1.ToString());
                    //Realiza a conta de resultado
                    DataTable dt = new DataTable();
                    double resultado = 0;   
                        var result = dt.Compute(contaAg, "").ToString();
                        resultado = double.Parse(result);
                        if (resultado == double.PositiveInfinity)
                        {
                            resultado = 0;
                        }
                        if (double.IsNaN(resultado))
                        {
                            resultado = 0;
                        }

                    if (consolidado.CARGO != "AGENTE")
                    {
                        if (consolidado.META == 0)
                        {
                            consolidado.META = 0;
                        }
                        else if (consolidado.QTD > 0)
                        {
                            consolidado.META = consolidado.META / consolidado.QTD;
                            consolidado.META = Math.Round(consolidado.META, 2, MidpointRounding.AwayFromZero);
                        }

                        if (resultado == 0 && (consolidado.IDINDICADOR == "191" || consolidado.IDINDICADOR == "371" || consolidado.IDINDICADOR == "193"))
                        {
                            consolidado.GRUPO = "Bronze";
                            consolidado.PERCENTUAL = 0;
                            consolidado.RESULTADO = 0;
                        }

                        //Regra do TMA. Quando o arredondamento tambem der 0, não monetizar e considerar bronze
                        double arredondResult = Math.Round(resultado, 0, MidpointRounding.AwayFromZero);
                        if (arredondResult == 0 && (consolidado.IDINDICADOR == "191" || consolidado.IDINDICADOR == "371" || consolidado.IDINDICADOR == "193"))
                        {
                            consolidado.GRUPO = "Bronze";
                            consolidado.PERCENTUAL = 0;
                            consolidado.RESULTADO = 0;
                        }
                        double resultadoD = resultado;
                        resultadoD = resultadoD * 100;
                        double atingimentoMeta = 0;
                        //Verifica se é melhor ou menor melhor
                        if (consolidado.BETTER == "BIGGER_BETTER")
                        {
                            if (consolidado.META == 0)
                            {
                                atingimentoMeta = 0;
                            }
                            else
                            {
                                atingimentoMeta = resultadoD / consolidado.META;
                            }
                        }
                        else
                        {
                            // No caso de menor melhor, quando o denominador é 0 não esta sendo possivel fazer a conta de divisão por 0.
                            // E como é menor melhor, logo 0 é um resultado de 100% de atingimento.
                            if (resultadoD == 0) // && factor.goal < 1
                            {
                                atingimentoMeta = 10;
                            }

                            else
                            {
                                atingimentoMeta = (consolidado.META / resultadoD);

                            }
                        }

                        atingimentoMeta = atingimentoMeta * 100;



                            //Verifica a qual grupo pertence

                            if (atingimentoMeta >= consolidado.min1)
                            {
                            consolidado.GRUPO = "Diamante";
                            }
                            else if (atingimentoMeta >= consolidado.min2)
                            {
                            consolidado.GRUPO = "Ouro";
                            }
                            else if (atingimentoMeta >= consolidado.min3)
                            {
                            consolidado.GRUPO = "Prata";
                            }
                            else if (atingimentoMeta >= consolidado.min4)
                            {
                            consolidado.GRUPO = "Bronze";
                            }
                            else
                            {
                            consolidado.GRUPO = "Bronze";
                            }
                        consolidado.PERCENTUAL = atingimentoMeta;

                        consolidado.RESULTADO = resultadoD;

                        retorno[i] = consolidado;
                    }
                }        
                    catch (Exception)
                    {

                    }
            }

                var jsonData = retorno.Select(item => new returnResponseDay
            {
               MATRICULA = item.MATRICULA,
               CARGO = item.CARGO,
               IDINDICADOR = item.IDINDICADOR,
               INDICADOR = item.INDICADOR,
               META = item.META,
               RESULTADO = item.RESULTADO,
               PERCENTUAL = item.PERCENTUAL,
               META_MAXIMA_MOEDAS = item.META_MAXIMA_MOEDAS,
               MOEDA_GANHA = item.MOEDA_GANHA,
               GRUPO = item.GRUPO,
    }).ToList();


            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(jsonData);
        }


    }
}