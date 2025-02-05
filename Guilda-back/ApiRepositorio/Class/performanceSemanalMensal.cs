

using ApiRepositorio.Controllers;
using ApiRepositorio.Models;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using static ApiC.Class.performanceSemanalMensal;

namespace ApiC.Class
{
    public class performanceSemanalMensal
    {
        public class returnResponseSemanalMensal
        {
            public string Mes { get; set; }
            public string Ano { get; set; }
            public string Matricula { get; set; }
            public string NomeColaborador { get; set; }
            public string Cargo { get; set; }

            public string Indicador { get; set; }
            public string NomeIndicador { get; set; }
            public string TipoIndicador { get; set; }
            public double Meta { get; set; }
            public double Resultado { get; set; }
            public double Percentual { get; set; }
            public double GanhoEmMoedas { get; set; }
            public double MetaMaximaMoedas { get; set; }
            public string Grupo { get; set; }
            public string DataAtualizacao { get; set; }
            public string MatriculaSupervisor { get; set; }
            public string NomeSupervisor { get; set; }
            public string MatriculaCoordenador { get; set; }
            public string NomeCoordenador { get; set; }
            public string MatriculaGerente2 { get; set; }
            public string NomeGerente2 { get; set; }
            public string MatriculaGerente1 { get; set; }
            public string NomeGerente1 { get; set; }
            public string MatriculaDiretor { get; set; }
            public string NomeDiretor { get; set; }
            public string MatriculaCEO { get; set; }
            public string NomeCEO { get; set; }
            public string CodigoGIP { get; set; }
            public string Setor { get; set; }
            public double Fator0 { get; set; }
            public double Fator1 { get; set; }

            public int Idgda_Home { get; set; }
            public int idgda_Period { get; set; }
            public string Novato { get; set; }
            public int Idgda_Site { get; set; }
            //public string CodigoGIPSubsetor { get; set; }
            //public string Subsetor { get; set; }

        }

        public class semanalMensalConsolidado
        {
            public string MATRICULA { get; set; }
            public string NOME { get; set; }
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
            public double QTD_MON { get; set; }
            public double QTD_MON_TOTAL { get; set; }
            public double QTD_META { get; set; }
            public string DATAPAGAMENTO { get; set; }
            public double min1 { get; set; }
            public double min2 { get; set; }
            public double min3 { get; set; }
            public double min4 { get; set; }
            public string GRUPO { get; set; }
            public int IDGRUPO { get; set; }
            public string NOME_NIVEL { get; set; }
            public string IMAGEMGRUPO { get; set; }
            public string CODGIP { get; set; }
            public string SETOR { get; set; }
            public string CODGIPSUBSETOR { get; set; }
            public string SUBSETOR { get; set; }
            public string TYPE { get; set; }
            public bool MONETIZATION { get; set; }
            public int SUMDIASLOGADOS { get; set; }
            public int SUMDIASESCALADOS { get; set; }
            public int SUMDIASLOGADOSESCALADOS { get; set; }
            public string MATRICULASUPERVISOR { get; set; }
            public string NOMESUPERVISOR { get; set; }
            public string MATRICULACOORDENADOR { get; set; }
            public string NOMECOORDENADOR { get; set; }
            public string MATRICULAGERENTE2 { get; set; }
            public string NOMEGERENTE2 { get; set; }
            public string MATRICULAGERENTE1 { get; set; }
            public string NOMEGERENTE1 { get; set; }
            public string MATRICULADIRETOR { get; set; }
            public string NOMEDIRETOR { get; set; }
            public string MATRICULACEO { get; set; }
            public string NOMECEO { get; set; }
            public int IDGDA_HOME { get; set; }
            public int IDGDA_PERIODO { get; set; }
            public string NOVATO { get; set; }
            public int IDGDA_SITE { get; set; }

        }

        public static List<semanalMensalConsolidado> returnListHierarchy(List<semanalMensalConsolidado> original, string hierarchy, bool agruparResultado)
        {
            List<semanalMensalConsolidado> retorno = new List<semanalMensalConsolidado>();
            try
            {
                if (agruparResultado == true)
                {
                    if (hierarchy == "SUPERVISOR")
                    {
                        retorno = original
                            .GroupBy(item => new { item.IDINDICADOR, item.MATRICULASUPERVISOR, item.IDGDA_PERIODO, item.IDGDA_HOME, item.IDGDA_SITE }).Where(d => d.Key.MATRICULASUPERVISOR != "0")
                            .Select(grupo => new semanalMensalConsolidado
                            {
                                IDGDA_PERIODO = grupo.Key.IDGDA_PERIODO,
                                IDGDA_HOME = grupo.Key.IDGDA_HOME,
                                IDGDA_SITE = grupo.Key.IDGDA_SITE,

                                MATRICULA = grupo.First().MATRICULASUPERVISOR,
                                NOME = grupo.First().NOMESUPERVISOR,
                                CARGO = "SUPERVISOR",
                                //CODGIP = item.First().CODGIP,
                                //SETOR = item.First().SETOR,
                                IDINDICADOR = grupo.Key.IDINDICADOR,
                                INDICADOR = grupo.First().INDICADOR,
                                QTD = grupo.Sum(d => d.QTD),
                                //META = Math.Round(item.Sum(d => d.META) / item.Count(), 2, MidpointRounding.AwayFromZero),
                                META = Math.Round(grupo.Sum(d => d.META) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                FACTOR0 = grupo.Sum(d => d.FACTOR0),
                                FACTOR1 = grupo.Sum(d => d.FACTOR1),
                                //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                                min1 = Math.Round(grupo.Sum(d => d.min1) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min2 = Math.Round(grupo.Sum(d => d.min2) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min3 = Math.Round(grupo.Sum(d => d.min3) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min4 = Math.Round(grupo.Sum(d => d.min4) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                CONTA = grupo.First().CONTA,
                                BETTER = grupo.First().BETTER,
                                //META_MAXIMA_MOEDAS = Math.Round(item.Sum(d => d.META_MAXIMA_MOEDAS) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                                META_MAXIMA_MOEDAS = grupo.Sum(d => d.QTD_MON != 0 ? Math.Round(d.QTD_MON_TOTAL / d.QTD_MON, 0, MidpointRounding.AwayFromZero) : 0),

                                NOMESUPERVISOR = "-",
                                MATRICULASUPERVISOR = "-",
                                MATRICULACOORDENADOR = grupo.First().MATRICULACOORDENADOR,
                                NOMECOORDENADOR = grupo.First().NOMECOORDENADOR,
                                MATRICULAGERENTE2 = grupo.First().MATRICULAGERENTE2,
                                NOMEGERENTE2 = grupo.First().NOMEGERENTE2,
                                MATRICULAGERENTE1 = grupo.First().MATRICULAGERENTE1,
                                NOMEGERENTE1 = grupo.First().NOMEGERENTE1,
                                MATRICULADIRETOR = grupo.First().MATRICULADIRETOR,
                                NOMEDIRETOR = grupo.First().NOMEDIRETOR,
                                MATRICULACEO = grupo.First().MATRICULACEO,
                                NOMECEO = grupo.First().NOMECEO,

                                SUMDIASLOGADOS = grupo.Sum(d => d.SUMDIASLOGADOS),
                                SUMDIASESCALADOS = grupo.Sum(d => d.SUMDIASESCALADOS),
                                SUMDIASLOGADOSESCALADOS = grupo.Sum(d => d.SUMDIASLOGADOSESCALADOS),

                                //MOEDA_GANHA =  Math.Round(item.Sum(d => d.MOEDA_GANHA) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                                MOEDA_GANHA = Math.Round(grupo.Sum(d => d.MOEDA_GANHA), 0, MidpointRounding.AwayFromZero),
                                TYPE = grupo.First().TYPE,
                            }).ToList();

                    }
                    else if (hierarchy == "COORDENADOR")
                    {
                        retorno = original
                        .GroupBy(item => new { item.IDINDICADOR, item.MATRICULACOORDENADOR, item.IDGDA_PERIODO, item.IDGDA_HOME, item.IDGDA_SITE }).Where(d => d.Key.MATRICULACOORDENADOR != "0")
                            .Select(grupo => new semanalMensalConsolidado
                            {
                                IDGDA_PERIODO = grupo.Key.IDGDA_PERIODO,
                                IDGDA_HOME = grupo.Key.IDGDA_HOME,
                                IDGDA_SITE = grupo.Key.IDGDA_SITE,

                                MATRICULA = grupo.First().MATRICULACOORDENADOR,
                                NOME = grupo.First().NOMECOORDENADOR,
                                CARGO = "COORDENADOR",
                                //CODGIP = item.First().CODGIP,
                                //SETOR = item.First().SETOR,
                                IDINDICADOR = grupo.Key.IDINDICADOR,
                                INDICADOR = grupo.First().INDICADOR,
                                QTD = grupo.Sum(d => d.QTD),
                                //META = Math.Round(item.Sum(d => d.META) / item.Count(), 2, MidpointRounding.AwayFromZero),
                                META = Math.Round(grupo.Sum(d => d.META) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                FACTOR0 = grupo.Sum(d => d.FACTOR0),
                                FACTOR1 = grupo.Sum(d => d.FACTOR1),
                                //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                                min1 = Math.Round(grupo.Sum(d => d.min1) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min2 = Math.Round(grupo.Sum(d => d.min2) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min3 = Math.Round(grupo.Sum(d => d.min3) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min4 = Math.Round(grupo.Sum(d => d.min4) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                CONTA = grupo.First().CONTA,
                                BETTER = grupo.First().BETTER,
                                //META_MAXIMA_MOEDAS = Math.Round(item.Sum(d => d.META_MAXIMA_MOEDAS) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                                META_MAXIMA_MOEDAS = grupo.Sum(d => d.QTD_MON != 0 ? Math.Round(d.QTD_MON_TOTAL / d.QTD_MON, 0, MidpointRounding.AwayFromZero) : 0),

                                NOMESUPERVISOR = "-",
                                MATRICULASUPERVISOR = "-",
                                MATRICULACOORDENADOR = "-",
                                NOMECOORDENADOR = "-",
                                MATRICULAGERENTE2 = grupo.First().MATRICULAGERENTE2,
                                NOMEGERENTE2 = grupo.First().NOMEGERENTE2,
                                MATRICULAGERENTE1 = grupo.First().MATRICULAGERENTE1,
                                NOMEGERENTE1 = grupo.First().NOMEGERENTE1,
                                MATRICULADIRETOR = grupo.First().MATRICULADIRETOR,
                                NOMEDIRETOR = grupo.First().NOMEDIRETOR,
                                MATRICULACEO = grupo.First().MATRICULACEO,
                                NOMECEO = grupo.First().NOMECEO,

                                SUMDIASLOGADOS = grupo.Sum(d => d.SUMDIASLOGADOS),
                                SUMDIASESCALADOS = grupo.Sum(d => d.SUMDIASESCALADOS),
                                SUMDIASLOGADOSESCALADOS = grupo.Sum(d => d.SUMDIASLOGADOSESCALADOS),

                                //MOEDA_GANHA =  Math.Round(item.Sum(d => d.MOEDA_GANHA) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                                MOEDA_GANHA = Math.Round(grupo.Sum(d => d.MOEDA_GANHA), 0, MidpointRounding.AwayFromZero),
                                TYPE = grupo.First().TYPE,
                            }).ToList();
                    }
                    else if (hierarchy == "GERENTE II")
                    {
                        retorno = original
                        .GroupBy(item => new { item.IDINDICADOR, item.MATRICULAGERENTE2, item.IDGDA_PERIODO, item.IDGDA_HOME, item.IDGDA_SITE }).Where(d => d.Key.MATRICULAGERENTE2 != "0")
                            .Select(grupo => new semanalMensalConsolidado
                            {
                                IDGDA_PERIODO = grupo.Key.IDGDA_PERIODO,
                                IDGDA_HOME = grupo.Key.IDGDA_HOME,
                                IDGDA_SITE = grupo.Key.IDGDA_SITE,

                                MATRICULA = grupo.First().MATRICULAGERENTE2,
                                NOME = grupo.First().NOMEGERENTE2,
                                CARGO = "GERENTE II",
                                //CODGIP = item.First().CODGIP,
                                //SETOR = item.First().SETOR,
                                IDINDICADOR = grupo.Key.IDINDICADOR,
                                INDICADOR = grupo.First().INDICADOR,
                                QTD = grupo.Sum(d => d.QTD),
                                //META = Math.Round(item.Sum(d => d.META) / item.Count(), 2, MidpointRounding.AwayFromZero),
                                META = Math.Round(grupo.Sum(d => d.META) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                FACTOR0 = grupo.Sum(d => d.FACTOR0),
                                FACTOR1 = grupo.Sum(d => d.FACTOR1),
                                //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                                min1 = Math.Round(grupo.Sum(d => d.min1) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min2 = Math.Round(grupo.Sum(d => d.min2) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min3 = Math.Round(grupo.Sum(d => d.min3) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min4 = Math.Round(grupo.Sum(d => d.min4) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                CONTA = grupo.First().CONTA,
                                BETTER = grupo.First().BETTER,
                                //META_MAXIMA_MOEDAS = Math.Round(item.Sum(d => d.META_MAXIMA_MOEDAS) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                                META_MAXIMA_MOEDAS = grupo.Sum(d => d.QTD_MON != 0 ? Math.Round(d.QTD_MON_TOTAL / d.QTD_MON, 0, MidpointRounding.AwayFromZero) : 0),

                                NOMESUPERVISOR = "-",
                                MATRICULASUPERVISOR = "-",
                                MATRICULACOORDENADOR = "-",
                                NOMECOORDENADOR = "-",
                                MATRICULAGERENTE2 = "-",
                                NOMEGERENTE2 = "-",
                                MATRICULAGERENTE1 = grupo.First().MATRICULAGERENTE1,
                                NOMEGERENTE1 = grupo.First().NOMEGERENTE1,
                                MATRICULADIRETOR = grupo.First().MATRICULADIRETOR,
                                NOMEDIRETOR = grupo.First().NOMEDIRETOR,
                                MATRICULACEO = grupo.First().MATRICULACEO,
                                NOMECEO = grupo.First().NOMECEO,

                                SUMDIASLOGADOS = grupo.Sum(d => d.SUMDIASLOGADOS),
                                SUMDIASESCALADOS = grupo.Sum(d => d.SUMDIASESCALADOS),
                                SUMDIASLOGADOSESCALADOS = grupo.Sum(d => d.SUMDIASLOGADOSESCALADOS),

                                //MOEDA_GANHA =  Math.Round(item.Sum(d => d.MOEDA_GANHA) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                                MOEDA_GANHA = Math.Round(grupo.Sum(d => d.MOEDA_GANHA), 0, MidpointRounding.AwayFromZero),
                                TYPE = grupo.First().TYPE,
                            }).ToList();
                    }
                    else if (hierarchy == "GERENTE I")
                    {
                        retorno = original
                        .GroupBy(item => new { item.IDINDICADOR, item.MATRICULAGERENTE1, item.IDGDA_PERIODO, item.IDGDA_HOME, item.IDGDA_SITE }).Where(d => d.Key.MATRICULAGERENTE1 != "0")
                            .Select(grupo => new semanalMensalConsolidado
                            {
                                IDGDA_PERIODO = grupo.Key.IDGDA_PERIODO,
                                IDGDA_HOME = grupo.Key.IDGDA_HOME,
                                IDGDA_SITE = grupo.Key.IDGDA_SITE,

                                MATRICULA = grupo.First().MATRICULAGERENTE1,
                                NOME = grupo.First().NOMEGERENTE1,
                                CARGO = "GERENTE I",
                                //CODGIP = item.First().CODGIP,
                                //SETOR = item.First().SETOR,
                                IDINDICADOR = grupo.Key.IDINDICADOR,
                                INDICADOR = grupo.First().INDICADOR,
                                QTD = grupo.Sum(d => d.QTD),
                                //META = Math.Round(item.Sum(d => d.META) / item.Count(), 2, MidpointRounding.AwayFromZero),
                                META = Math.Round(grupo.Sum(d => d.META) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                FACTOR0 = grupo.Sum(d => d.FACTOR0),
                                FACTOR1 = grupo.Sum(d => d.FACTOR1),
                                //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                                min1 = Math.Round(grupo.Sum(d => d.min1) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min2 = Math.Round(grupo.Sum(d => d.min2) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min3 = Math.Round(grupo.Sum(d => d.min3) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min4 = Math.Round(grupo.Sum(d => d.min4) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                CONTA = grupo.First().CONTA,
                                BETTER = grupo.First().BETTER,
                                //META_MAXIMA_MOEDAS = Math.Round(item.Sum(d => d.META_MAXIMA_MOEDAS) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                                META_MAXIMA_MOEDAS = grupo.Sum(d => d.QTD_MON != 0 ? Math.Round(d.QTD_MON_TOTAL / d.QTD_MON, 0, MidpointRounding.AwayFromZero) : 0),

                                NOMESUPERVISOR = "-",
                                MATRICULASUPERVISOR = "-",
                                MATRICULACOORDENADOR = "-",
                                NOMECOORDENADOR = "-",
                                MATRICULAGERENTE2 = "-",
                                NOMEGERENTE2 = "-",
                                MATRICULAGERENTE1 = "-",
                                NOMEGERENTE1 = "-",
                                MATRICULADIRETOR = grupo.First().MATRICULADIRETOR,
                                NOMEDIRETOR = grupo.First().NOMEDIRETOR,
                                MATRICULACEO = grupo.First().MATRICULACEO,
                                NOMECEO = grupo.First().NOMECEO,

                                SUMDIASLOGADOS = grupo.Sum(d => d.SUMDIASLOGADOS),
                                SUMDIASESCALADOS = grupo.Sum(d => d.SUMDIASESCALADOS),
                                SUMDIASLOGADOSESCALADOS = grupo.Sum(d => d.SUMDIASLOGADOSESCALADOS),

                                //MOEDA_GANHA =  Math.Round(item.Sum(d => d.MOEDA_GANHA) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                                MOEDA_GANHA = Math.Round(grupo.Sum(d => d.MOEDA_GANHA), 0, MidpointRounding.AwayFromZero),
                                TYPE = grupo.First().TYPE,
                            }).ToList();
                    }
                    else if (hierarchy == "DIRETOR")
                    {
                        retorno = original
                        .GroupBy(item => new { item.IDINDICADOR, item.MATRICULADIRETOR, item.IDGDA_PERIODO, item.IDGDA_HOME, item.IDGDA_SITE }).Where(d => d.Key.MATRICULADIRETOR != "0")
                            .Select(grupo => new semanalMensalConsolidado
                            {
                                IDGDA_PERIODO = grupo.Key.IDGDA_PERIODO,
                                IDGDA_HOME = grupo.Key.IDGDA_HOME,
                                IDGDA_SITE = grupo.Key.IDGDA_SITE,

                                MATRICULA = grupo.First().MATRICULADIRETOR,
                                NOME = grupo.First().NOMEDIRETOR,
                                CARGO = "DIRETOR",
                                //CODGIP = item.First().CODGIP,
                                //SETOR = item.First().SETOR,
                                IDINDICADOR = grupo.Key.IDINDICADOR,
                                INDICADOR = grupo.First().INDICADOR,
                                QTD = grupo.Sum(d => d.QTD),
                                //META = Math.Round(item.Sum(d => d.META) / item.Count(), 2, MidpointRounding.AwayFromZero),
                                META = Math.Round(grupo.Sum(d => d.META) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                FACTOR0 = grupo.Sum(d => d.FACTOR0),
                                FACTOR1 = grupo.Sum(d => d.FACTOR1),
                                //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                                min1 = Math.Round(grupo.Sum(d => d.min1) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min2 = Math.Round(grupo.Sum(d => d.min2) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min3 = Math.Round(grupo.Sum(d => d.min3) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min4 = Math.Round(grupo.Sum(d => d.min4) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                CONTA = grupo.First().CONTA,
                                BETTER = grupo.First().BETTER,
                                //META_MAXIMA_MOEDAS = Math.Round(item.Sum(d => d.META_MAXIMA_MOEDAS) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                                META_MAXIMA_MOEDAS = grupo.Sum(d => d.QTD_MON != 0 ? Math.Round(d.QTD_MON_TOTAL / d.QTD_MON, 0, MidpointRounding.AwayFromZero) : 0),

                                NOMESUPERVISOR = "-",
                                MATRICULASUPERVISOR = "-",
                                MATRICULACOORDENADOR = "-",
                                NOMECOORDENADOR = "-",
                                MATRICULAGERENTE2 = "-",
                                NOMEGERENTE2 = "-",
                                MATRICULAGERENTE1 = "-",
                                NOMEGERENTE1 = "-",
                                MATRICULADIRETOR = "-",
                                NOMEDIRETOR = "-",
                                MATRICULACEO = grupo.First().MATRICULACEO,
                                NOMECEO = grupo.First().NOMECEO,

                                SUMDIASLOGADOS = grupo.Sum(d => d.SUMDIASLOGADOS),
                                SUMDIASESCALADOS = grupo.Sum(d => d.SUMDIASESCALADOS),
                                SUMDIASLOGADOSESCALADOS = grupo.Sum(d => d.SUMDIASLOGADOSESCALADOS),

                                //MOEDA_GANHA =  Math.Round(item.Sum(d => d.MOEDA_GANHA) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                                MOEDA_GANHA = Math.Round(grupo.Sum(d => d.MOEDA_GANHA), 0, MidpointRounding.AwayFromZero),
                                TYPE = grupo.First().TYPE,
                            }).ToList();
                    }
                    else if (hierarchy == "CEO")
                    {
                        retorno = original
                        .GroupBy(item => new { item.IDINDICADOR, item.MATRICULACEO, item.IDGDA_PERIODO, item.IDGDA_HOME, item.IDGDA_SITE }).Where(d => d.Key.MATRICULACEO != "0")
                            .Select(grupo => new semanalMensalConsolidado
                            {
                                IDGDA_PERIODO = grupo.Key.IDGDA_PERIODO,
                                IDGDA_HOME = grupo.Key.IDGDA_HOME,
                                IDGDA_SITE = grupo.Key.IDGDA_SITE,

                                MATRICULA = grupo.First().MATRICULACEO,
                                NOME = grupo.First().NOMECEO,
                                CARGO = "CEO",
                                //CODGIP = item.First().CODGIP,
                                //SETOR = item.First().SETOR,
                                IDINDICADOR = grupo.Key.IDINDICADOR,
                                INDICADOR = grupo.First().INDICADOR,
                                QTD = grupo.Sum(d => d.QTD),
                                //META = Math.Round(item.Sum(d => d.META) / item.Count(), 2, MidpointRounding.AwayFromZero),
                                META = Math.Round(grupo.Sum(d => d.META) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                FACTOR0 = grupo.Sum(d => d.FACTOR0),
                                FACTOR1 = grupo.Sum(d => d.FACTOR1),
                                //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                                min1 = Math.Round(grupo.Sum(d => d.min1) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min2 = Math.Round(grupo.Sum(d => d.min2) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min3 = Math.Round(grupo.Sum(d => d.min3) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min4 = Math.Round(grupo.Sum(d => d.min4) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                CONTA = grupo.First().CONTA,
                                BETTER = grupo.First().BETTER,
                                //META_MAXIMA_MOEDAS = Math.Round(item.Sum(d => d.META_MAXIMA_MOEDAS) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                                META_MAXIMA_MOEDAS = grupo.Sum(d => d.QTD_MON != 0 ? Math.Round(d.QTD_MON_TOTAL / d.QTD_MON, 0, MidpointRounding.AwayFromZero) : 0),

                                NOMESUPERVISOR = "-",
                                MATRICULASUPERVISOR = "-",
                                MATRICULACOORDENADOR = "-",
                                NOMECOORDENADOR = "-",
                                MATRICULAGERENTE2 = "-",
                                NOMEGERENTE2 = "-",
                                MATRICULAGERENTE1 = "-",
                                NOMEGERENTE1 = "-",
                                MATRICULADIRETOR = "-",
                                NOMEDIRETOR = "-",
                                MATRICULACEO = "-",
                                NOMECEO = "-",

                                SUMDIASLOGADOS = grupo.Sum(d => d.SUMDIASLOGADOS),
                                SUMDIASESCALADOS = grupo.Sum(d => d.SUMDIASESCALADOS),
                                SUMDIASLOGADOSESCALADOS = grupo.Sum(d => d.SUMDIASLOGADOSESCALADOS),

                                //MOEDA_GANHA =  Math.Round(item.Sum(d => d.MOEDA_GANHA) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                                MOEDA_GANHA = Math.Round(grupo.Sum(d => d.MOEDA_GANHA), 0, MidpointRounding.AwayFromZero),
                                TYPE = grupo.First().TYPE,
                            }).ToList();
                    }
                }
                else
                {
                    if (hierarchy == "SUPERVISOR")
                    {
                        retorno = original
                            .GroupBy(item => new { item.IDINDICADOR, item.MATRICULASUPERVISOR }).Where(d => d.Key.MATRICULASUPERVISOR != "0")
                            .Select(grupo => new semanalMensalConsolidado
                            {
                                IDGDA_PERIODO = grupo.First().IDGDA_PERIODO,
                                IDGDA_HOME = grupo.First().IDGDA_HOME,
                                IDGDA_SITE = grupo.First().IDGDA_SITE,

                                MATRICULA = grupo.First().MATRICULASUPERVISOR,
                                NOME = grupo.First().NOMESUPERVISOR,
                                CARGO = "SUPERVISOR",
                                //CODGIP = item.First().CODGIP,
                                //SETOR = item.First().SETOR,
                                IDINDICADOR = grupo.Key.IDINDICADOR,
                                INDICADOR = grupo.First().INDICADOR,
                                QTD = grupo.Sum(d => d.QTD),
                                //META = Math.Round(item.Sum(d => d.META) / item.Count(), 2, MidpointRounding.AwayFromZero),
                                META = Math.Round(grupo.Sum(d => d.META) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                FACTOR0 = grupo.Sum(d => d.FACTOR0),
                                FACTOR1 = grupo.Sum(d => d.FACTOR1),
                                //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                                min1 = Math.Round(grupo.Sum(d => d.min1) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min2 = Math.Round(grupo.Sum(d => d.min2) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min3 = Math.Round(grupo.Sum(d => d.min3) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min4 = Math.Round(grupo.Sum(d => d.min4) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                CONTA = grupo.First().CONTA,
                                BETTER = grupo.First().BETTER,
                                //META_MAXIMA_MOEDAS = Math.Round(item.Sum(d => d.META_MAXIMA_MOEDAS) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                                META_MAXIMA_MOEDAS = grupo.Sum(d => d.QTD_MON != 0 ? Math.Round(d.QTD_MON_TOTAL / d.QTD_MON, 0, MidpointRounding.AwayFromZero) : 0),

                                NOMESUPERVISOR = "-",
                                MATRICULASUPERVISOR = "-",
                                MATRICULACOORDENADOR = grupo.First().MATRICULACOORDENADOR,
                                NOMECOORDENADOR = grupo.First().NOMECOORDENADOR,
                                MATRICULAGERENTE2 = grupo.First().MATRICULAGERENTE2,
                                NOMEGERENTE2 = grupo.First().NOMEGERENTE2,
                                MATRICULAGERENTE1 = grupo.First().MATRICULAGERENTE1,
                                NOMEGERENTE1 = grupo.First().NOMEGERENTE1,
                                MATRICULADIRETOR = grupo.First().MATRICULADIRETOR,
                                NOMEDIRETOR = grupo.First().NOMEDIRETOR,
                                MATRICULACEO = grupo.First().MATRICULACEO,
                                NOMECEO = grupo.First().NOMECEO,

                                SUMDIASLOGADOS = grupo.Sum(d => d.SUMDIASLOGADOS),
                                SUMDIASESCALADOS = grupo.Sum(d => d.SUMDIASESCALADOS),
                                SUMDIASLOGADOSESCALADOS = grupo.Sum(d => d.SUMDIASLOGADOSESCALADOS),

                                //MOEDA_GANHA =  Math.Round(item.Sum(d => d.MOEDA_GANHA) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                                MOEDA_GANHA = Math.Round(grupo.Sum(d => d.MOEDA_GANHA), 0, MidpointRounding.AwayFromZero),
                                TYPE = grupo.First().TYPE,
                            }).ToList();

                    }
                    else if (hierarchy == "COORDENADOR")
                    {
                        retorno = original
                            .GroupBy(item => new { item.IDINDICADOR, item.MATRICULACOORDENADOR }).Where(d => d.Key.MATRICULACOORDENADOR != "0")
                            .Select(grupo => new semanalMensalConsolidado
                            {
                                IDGDA_PERIODO = grupo.First().IDGDA_PERIODO,
                                IDGDA_HOME = grupo.First().IDGDA_HOME,
                                IDGDA_SITE = grupo.First().IDGDA_SITE,

                                MATRICULA = grupo.First().MATRICULACOORDENADOR,
                                NOME = grupo.First().NOMECOORDENADOR,
                                CARGO = "COORDENADOR",
                                //CODGIP = item.First().CODGIP,
                                //SETOR = item.First().SETOR,
                                IDINDICADOR = grupo.Key.IDINDICADOR,
                                INDICADOR = grupo.First().INDICADOR,
                                QTD = grupo.Sum(d => d.QTD),
                                //META = Math.Round(item.Sum(d => d.META) / item.Count(), 2, MidpointRounding.AwayFromZero),
                                META = Math.Round(grupo.Sum(d => d.META) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                FACTOR0 = grupo.Sum(d => d.FACTOR0),
                                FACTOR1 = grupo.Sum(d => d.FACTOR1),
                                //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                                min1 = Math.Round(grupo.Sum(d => d.min1) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min2 = Math.Round(grupo.Sum(d => d.min2) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min3 = Math.Round(grupo.Sum(d => d.min3) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min4 = Math.Round(grupo.Sum(d => d.min4) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                CONTA = grupo.First().CONTA,
                                BETTER = grupo.First().BETTER,
                                //META_MAXIMA_MOEDAS = Math.Round(item.Sum(d => d.META_MAXIMA_MOEDAS) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                                META_MAXIMA_MOEDAS = grupo.Sum(d => d.QTD_MON != 0 ? Math.Round(d.QTD_MON_TOTAL / d.QTD_MON, 0, MidpointRounding.AwayFromZero) : 0),

                                NOMESUPERVISOR = "-",
                                MATRICULASUPERVISOR = "-",
                                MATRICULACOORDENADOR = "-",
                                NOMECOORDENADOR = "-",
                                MATRICULAGERENTE2 = grupo.First().MATRICULAGERENTE2,
                                NOMEGERENTE2 = grupo.First().NOMEGERENTE2,
                                MATRICULAGERENTE1 = grupo.First().MATRICULAGERENTE1,
                                NOMEGERENTE1 = grupo.First().NOMEGERENTE1,
                                MATRICULADIRETOR = grupo.First().MATRICULADIRETOR,
                                NOMEDIRETOR = grupo.First().NOMEDIRETOR,
                                MATRICULACEO = grupo.First().MATRICULACEO,
                                NOMECEO = grupo.First().NOMECEO,

                                SUMDIASLOGADOS = grupo.Sum(d => d.SUMDIASLOGADOS),
                                SUMDIASESCALADOS = grupo.Sum(d => d.SUMDIASESCALADOS),
                                SUMDIASLOGADOSESCALADOS = grupo.Sum(d => d.SUMDIASLOGADOSESCALADOS),

                                //MOEDA_GANHA =  Math.Round(item.Sum(d => d.MOEDA_GANHA) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                                MOEDA_GANHA = Math.Round(grupo.Sum(d => d.MOEDA_GANHA), 0, MidpointRounding.AwayFromZero),
                                TYPE = grupo.First().TYPE,
                            })
                        .ToList();
                    }
                    else if (hierarchy == "GERENTE II")
                    {
                        retorno = original
                            .GroupBy(item => new { item.IDINDICADOR, item.MATRICULAGERENTE2 }).Where(d => d.Key.MATRICULAGERENTE2 != "0")
                            .Select(grupo => new semanalMensalConsolidado
                            {
                                IDGDA_PERIODO = grupo.First().IDGDA_PERIODO,
                                IDGDA_HOME = grupo.First().IDGDA_HOME,
                                IDGDA_SITE = grupo.First().IDGDA_SITE,

                                MATRICULA = grupo.First().MATRICULAGERENTE2,
                                NOME = grupo.First().NOMEGERENTE2,
                                CARGO = "GERENTE II",
                                //CODGIP = item.First().CODGIP,
                                //SETOR = item.First().SETOR,
                                IDINDICADOR = grupo.Key.IDINDICADOR,
                                INDICADOR = grupo.First().INDICADOR,
                                QTD = grupo.Sum(d => d.QTD),
                                //META = Math.Round(item.Sum(d => d.META) / item.Count(), 2, MidpointRounding.AwayFromZero),
                                META = Math.Round(grupo.Sum(d => d.META) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                FACTOR0 = grupo.Sum(d => d.FACTOR0),
                                FACTOR1 = grupo.Sum(d => d.FACTOR1),
                                //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                                min1 = Math.Round(grupo.Sum(d => d.min1) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min2 = Math.Round(grupo.Sum(d => d.min2) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min3 = Math.Round(grupo.Sum(d => d.min3) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min4 = Math.Round(grupo.Sum(d => d.min4) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                CONTA = grupo.First().CONTA,
                                BETTER = grupo.First().BETTER,
                                //META_MAXIMA_MOEDAS = Math.Round(item.Sum(d => d.META_MAXIMA_MOEDAS) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                                META_MAXIMA_MOEDAS = grupo.Sum(d => d.QTD_MON != 0 ? Math.Round(d.QTD_MON_TOTAL / d.QTD_MON, 0, MidpointRounding.AwayFromZero) : 0),

                                NOMESUPERVISOR = "-",
                                MATRICULASUPERVISOR = "-",
                                MATRICULACOORDENADOR = "-",
                                NOMECOORDENADOR = "-",
                                MATRICULAGERENTE2 = "-",
                                NOMEGERENTE2 = "-",
                                MATRICULAGERENTE1 = grupo.First().MATRICULAGERENTE1,
                                NOMEGERENTE1 = grupo.First().NOMEGERENTE1,
                                MATRICULADIRETOR = grupo.First().MATRICULADIRETOR,
                                NOMEDIRETOR = grupo.First().NOMEDIRETOR,
                                MATRICULACEO = grupo.First().MATRICULACEO,
                                NOMECEO = grupo.First().NOMECEO,

                                SUMDIASLOGADOS = grupo.Sum(d => d.SUMDIASLOGADOS),
                                SUMDIASESCALADOS = grupo.Sum(d => d.SUMDIASESCALADOS),
                                SUMDIASLOGADOSESCALADOS = grupo.Sum(d => d.SUMDIASLOGADOSESCALADOS),

                                //MOEDA_GANHA =  Math.Round(item.Sum(d => d.MOEDA_GANHA) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                                MOEDA_GANHA = Math.Round(grupo.Sum(d => d.MOEDA_GANHA), 0, MidpointRounding.AwayFromZero),
                                TYPE = grupo.First().TYPE,
                            })
                        .ToList();
                    }
                    else if (hierarchy == "GERENTE I")
                    {
                        retorno = original
                            .GroupBy(item => new { item.IDINDICADOR, item.MATRICULAGERENTE1 }).Where(d => d.Key.MATRICULAGERENTE1 != "0")
                            .Select(grupo => new semanalMensalConsolidado
                            {
                                IDGDA_PERIODO = grupo.First().IDGDA_PERIODO,
                                IDGDA_HOME = grupo.First().IDGDA_HOME,
                                IDGDA_SITE = grupo.First().IDGDA_SITE,

                                MATRICULA = grupo.First().MATRICULAGERENTE1,
                                NOME = grupo.First().NOMEGERENTE1,
                                CARGO = "GERENTE I",
                                //CODGIP = item.First().CODGIP,
                                //SETOR = item.First().SETOR,
                                IDINDICADOR = grupo.Key.IDINDICADOR,
                                INDICADOR = grupo.First().INDICADOR,
                                QTD = grupo.Sum(d => d.QTD),
                                //META = Math.Round(item.Sum(d => d.META) / item.Count(), 2, MidpointRounding.AwayFromZero),
                                META = Math.Round(grupo.Sum(d => d.META) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                FACTOR0 = grupo.Sum(d => d.FACTOR0),
                                FACTOR1 = grupo.Sum(d => d.FACTOR1),
                                //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                                min1 = Math.Round(grupo.Sum(d => d.min1) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min2 = Math.Round(grupo.Sum(d => d.min2) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min3 = Math.Round(grupo.Sum(d => d.min3) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min4 = Math.Round(grupo.Sum(d => d.min4) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                CONTA = grupo.First().CONTA,
                                BETTER = grupo.First().BETTER,
                                //META_MAXIMA_MOEDAS = Math.Round(item.Sum(d => d.META_MAXIMA_MOEDAS) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                                META_MAXIMA_MOEDAS = grupo.Sum(d => d.QTD_MON != 0 ? Math.Round(d.QTD_MON_TOTAL / d.QTD_MON, 0, MidpointRounding.AwayFromZero) : 0),

                                NOMESUPERVISOR = "-",
                                MATRICULASUPERVISOR = "-",
                                MATRICULACOORDENADOR = "-",
                                NOMECOORDENADOR = "-",
                                MATRICULAGERENTE2 = "-",
                                NOMEGERENTE2 = "-",
                                MATRICULAGERENTE1 = "-",
                                NOMEGERENTE1 = "-",
                                MATRICULADIRETOR = grupo.First().MATRICULADIRETOR,
                                NOMEDIRETOR = grupo.First().NOMEDIRETOR,
                                MATRICULACEO = grupo.First().MATRICULACEO,
                                NOMECEO = grupo.First().NOMECEO,

                                SUMDIASLOGADOS = grupo.Sum(d => d.SUMDIASLOGADOS),
                                SUMDIASESCALADOS = grupo.Sum(d => d.SUMDIASESCALADOS),
                                SUMDIASLOGADOSESCALADOS = grupo.Sum(d => d.SUMDIASLOGADOSESCALADOS),

                                //MOEDA_GANHA =  Math.Round(item.Sum(d => d.MOEDA_GANHA) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                                MOEDA_GANHA = Math.Round(grupo.Sum(d => d.MOEDA_GANHA), 0, MidpointRounding.AwayFromZero),
                                TYPE = grupo.First().TYPE,
                            })
                        .ToList();
                    }
                    else if (hierarchy == "DIRETOR")
                    {
                        retorno = original
                            .GroupBy(item => new { item.IDINDICADOR, item.MATRICULADIRETOR }).Where(d => d.Key.MATRICULADIRETOR != "0")
                            .Select(grupo => new semanalMensalConsolidado
                            {
                                IDGDA_PERIODO = grupo.First().IDGDA_PERIODO,
                                IDGDA_HOME = grupo.First().IDGDA_HOME,
                                IDGDA_SITE = grupo.First().IDGDA_SITE,

                                MATRICULA = grupo.First().MATRICULADIRETOR,
                                NOME = grupo.First().NOMEDIRETOR,
                                CARGO = "DIRETOR",
                                //CODGIP = item.First().CODGIP,
                                //SETOR = item.First().SETOR,
                                IDINDICADOR = grupo.Key.IDINDICADOR,
                                INDICADOR = grupo.First().INDICADOR,
                                QTD = grupo.Sum(d => d.QTD),
                                //META = Math.Round(item.Sum(d => d.META) / item.Count(), 2, MidpointRounding.AwayFromZero),
                                META = Math.Round(grupo.Sum(d => d.META) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                FACTOR0 = grupo.Sum(d => d.FACTOR0),
                                FACTOR1 = grupo.Sum(d => d.FACTOR1),
                                //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                                min1 = Math.Round(grupo.Sum(d => d.min1) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min2 = Math.Round(grupo.Sum(d => d.min2) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min3 = Math.Round(grupo.Sum(d => d.min3) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min4 = Math.Round(grupo.Sum(d => d.min4) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                CONTA = grupo.First().CONTA,
                                BETTER = grupo.First().BETTER,
                                //META_MAXIMA_MOEDAS = Math.Round(item.Sum(d => d.META_MAXIMA_MOEDAS) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                                META_MAXIMA_MOEDAS = grupo.Sum(d => d.QTD_MON != 0 ? Math.Round(d.QTD_MON_TOTAL / d.QTD_MON, 0, MidpointRounding.AwayFromZero) : 0),

                                NOMESUPERVISOR = "-",
                                MATRICULASUPERVISOR = "-",
                                MATRICULACOORDENADOR = "-",
                                NOMECOORDENADOR = "-",
                                MATRICULAGERENTE2 = "-",
                                NOMEGERENTE2 = "-",
                                MATRICULAGERENTE1 = "-",
                                NOMEGERENTE1 = "-",
                                MATRICULADIRETOR = "-",
                                NOMEDIRETOR = "-",
                                MATRICULACEO = grupo.First().MATRICULACEO,
                                NOMECEO = grupo.First().NOMECEO,

                                SUMDIASLOGADOS = grupo.Sum(d => d.SUMDIASLOGADOS),
                                SUMDIASESCALADOS = grupo.Sum(d => d.SUMDIASESCALADOS),
                                SUMDIASLOGADOSESCALADOS = grupo.Sum(d => d.SUMDIASLOGADOSESCALADOS),

                                //MOEDA_GANHA =  Math.Round(item.Sum(d => d.MOEDA_GANHA) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                                MOEDA_GANHA = Math.Round(grupo.Sum(d => d.MOEDA_GANHA), 0, MidpointRounding.AwayFromZero),
                                TYPE = grupo.First().TYPE,
                            })
                        .ToList();
                    }
                    else if (hierarchy == "CEO")
                    {
                        retorno = original
                            .GroupBy(item => new { item.IDINDICADOR, item.MATRICULACEO }).Where(d => d.Key.MATRICULACEO != "0")
                            .Select(grupo => new semanalMensalConsolidado
                            {
                                IDGDA_PERIODO = grupo.First().IDGDA_PERIODO,
                                IDGDA_HOME = grupo.First().IDGDA_HOME,
                                IDGDA_SITE = grupo.First().IDGDA_SITE,

                                MATRICULA = grupo.First().MATRICULASUPERVISOR,
                                NOME = grupo.First().NOMESUPERVISOR,
                                CARGO = "CEO",
                                //CODGIP = item.First().CODGIP,
                                //SETOR = item.First().SETOR,
                                IDINDICADOR = grupo.Key.IDINDICADOR,
                                INDICADOR = grupo.First().INDICADOR,
                                QTD = grupo.Sum(d => d.QTD),
                                //META = Math.Round(item.Sum(d => d.META) / item.Count(), 2, MidpointRounding.AwayFromZero),
                                META = Math.Round(grupo.Sum(d => d.META) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                FACTOR0 = grupo.Sum(d => d.FACTOR0),
                                FACTOR1 = grupo.Sum(d => d.FACTOR1),
                                //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                                min1 = Math.Round(grupo.Sum(d => d.min1) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min2 = Math.Round(grupo.Sum(d => d.min2) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min3 = Math.Round(grupo.Sum(d => d.min3) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                min4 = Math.Round(grupo.Sum(d => d.min4) / (grupo.Sum(d => d.QTD_META) > 0 ? grupo.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                                CONTA = grupo.First().CONTA,
                                BETTER = grupo.First().BETTER,
                                //META_MAXIMA_MOEDAS = Math.Round(item.Sum(d => d.META_MAXIMA_MOEDAS) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                                META_MAXIMA_MOEDAS = grupo.Sum(d => d.QTD_MON != 0 ? Math.Round(d.QTD_MON_TOTAL / d.QTD_MON, 0, MidpointRounding.AwayFromZero) : 0),

                                NOMESUPERVISOR = "-",
                                MATRICULASUPERVISOR = "-",
                                MATRICULACOORDENADOR = "-",
                                NOMECOORDENADOR = "-",
                                MATRICULAGERENTE2 = "-",
                                NOMEGERENTE2 = "-",
                                MATRICULAGERENTE1 = "-",
                                NOMEGERENTE1 = "-",
                                MATRICULADIRETOR = "-",
                                NOMEDIRETOR = "-",
                                MATRICULACEO = "-",
                                NOMECEO = "-",

                                SUMDIASLOGADOS = grupo.Sum(d => d.SUMDIASLOGADOS),
                                SUMDIASESCALADOS = grupo.Sum(d => d.SUMDIASESCALADOS),
                                SUMDIASLOGADOSESCALADOS = grupo.Sum(d => d.SUMDIASLOGADOSESCALADOS),

                                //MOEDA_GANHA =  Math.Round(item.Sum(d => d.MOEDA_GANHA) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                                MOEDA_GANHA = Math.Round(grupo.Sum(d => d.MOEDA_GANHA), 0, MidpointRounding.AwayFromZero),
                                TYPE = grupo.First().TYPE,
                            })
                        .ToList();
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
            return retorno;
        }


        public static void processoSemanalMensal()
        {
            //int anoAnterior4 = DateTime.Now.AddMonths(-4).Year;
            //int mesAnterior4 = DateTime.Now.AddMonths(-4).Month;

            ////Mes anterior
            ////1ª Semana
            //ConsolidatedResultSemanalDiario(1, mesAnterior4, anoAnterior4);
            ////2ª Semana
            //ConsolidatedResultSemanalDiario(2, mesAnterior4, anoAnterior4);
            ////3ª Semana
            //ConsolidatedResultSemanalDiario(3, mesAnterior4, anoAnterior4);
            ////4ª Semana
            //ConsolidatedResultSemanalDiario(4, mesAnterior4, anoAnterior4);
            ////Mensal
            //ConsolidatedResultSemanalDiario(0, mesAnterior4, anoAnterior4);

            //int anoAnterior3 = DateTime.Now.AddMonths(-3).Year;
            //int mesAnterior3 = DateTime.Now.AddMonths(-3).Month;

            ////Mes anterior
            ////1ª Semana
            //ConsolidatedResultSemanalDiario(1, mesAnterior3, anoAnterior3);
            ////2ª Semana
            //ConsolidatedResultSemanalDiario(2, mesAnterior3, anoAnterior3);
            ////3ª Semana
            //ConsolidatedResultSemanalDiario(3, mesAnterior3, anoAnterior3);
            ////4ª Semana
            //ConsolidatedResultSemanalDiario(4, mesAnterior3, anoAnterior3);
            ////Mensal
            //ConsolidatedResultSemanalDiario(0, mesAnterior3, anoAnterior3);

            //int anoAnterior2 = DateTime.Now.AddMonths(-2).Year;
            //int mesAnterior2 = DateTime.Now.AddMonths(-2).Month;

            ////Mes anterior
            ////1ª Semana
            //ConsolidatedResultSemanalDiario(1, mesAnterior2, anoAnterior2);
            ////2ª Semana
            //ConsolidatedResultSemanalDiario(2, mesAnterior2, anoAnterior2);
            ////3ª Semana
            //ConsolidatedResultSemanalDiario(3, mesAnterior2, anoAnterior2);
            ////4ª Semana
            //ConsolidatedResultSemanalDiario(4, mesAnterior2, anoAnterior2);
            ////Mensal
            //ConsolidatedResultSemanalDiario(0, mesAnterior2, anoAnterior2);


            int anoAnterior = DateTime.Now.AddMonths(-1).Year;
            int mesAnterior = DateTime.Now.AddMonths(-1).Month;

            //Mes anterior
            //1ª Semana
            ConsolidatedResultSemanalDiario(1, mesAnterior, anoAnterior);
            //2ª Semana
            ConsolidatedResultSemanalDiario(2, mesAnterior, anoAnterior);
            //3ª Semana
            ConsolidatedResultSemanalDiario(3, mesAnterior, anoAnterior);
            //4ª Semana
            ConsolidatedResultSemanalDiario(4, mesAnterior, anoAnterior);
            //Mensal
            ConsolidatedResultSemanalDiario(0, mesAnterior, anoAnterior);

            int anoAtual = DateTime.Now.Year;
            int mesAtual = DateTime.Now.Month;
            //Mes Atual
            //1ª Semana
            ConsolidatedResultSemanalDiario(1, mesAtual, anoAtual);
            //2ª Semana
            ConsolidatedResultSemanalDiario(2, mesAtual, anoAtual);
            //3ª Semana
            ConsolidatedResultSemanalDiario(3, mesAtual, anoAtual);
            //4ª Semana
            ConsolidatedResultSemanalDiario(4, mesAtual, anoAtual);
            //Mensal
            ConsolidatedResultSemanalDiario(0, mesAtual, anoAtual);
        }

        public static List<semanalMensalConsolidado> agrupamentoTratativa(List<semanalMensalConsolidado> rmams, bool agruparResultado)
        {
            List<groups> lgroup = returnTables.listGroups("");
            groups lgroup1 = lgroup.Find(l => l.id == 1);
            groups lgroup2 = lgroup.Find(l => l.id == 2);
            groups lgroup3 = lgroup.Find(l => l.id == 3);
            groups lgroup4 = lgroup.Find(l => l.id == 4);

            List<semanalMensalConsolidado> supervisores = new List<semanalMensalConsolidado>();
            List<semanalMensalConsolidado> coordenador = new List<semanalMensalConsolidado>();
            List<semanalMensalConsolidado> gerenteii = new List<semanalMensalConsolidado>();
            List<semanalMensalConsolidado> gerentei = new List<semanalMensalConsolidado>();
            List<semanalMensalConsolidado> diretor = new List<semanalMensalConsolidado>();
            List<semanalMensalConsolidado> ceo = new List<semanalMensalConsolidado>();
            List<semanalMensalConsolidado> hierarchies = new List<semanalMensalConsolidado>();

            supervisores = returnListHierarchy(rmams, "SUPERVISOR", agruparResultado);

            coordenador = returnListHierarchy(rmams, "COORDENADOR", agruparResultado);
            gerenteii = returnListHierarchy(rmams, "GERENTE II", agruparResultado);
            gerentei = returnListHierarchy(rmams, "GERENTE I", agruparResultado);
            diretor = returnListHierarchy(rmams, "DIRETOR", agruparResultado);
            ceo = returnListHierarchy(rmams, "CEO", agruparResultado);

            rmams = rmams.Concat(supervisores).Concat(coordenador).Concat(gerenteii).Concat(gerentei).Concat(diretor).Concat(ceo).ToList();

            //AGRUPAR POR INDICADOR REALIZANDO O PONTERAMENTO DAS MOEDAS MAXIMAS COM MOEDAS GANHAS
            List<semanalMensalConsolidado> retorno = new List<semanalMensalConsolidado>();

            if (agruparResultado == true)
            {
                retorno = rmams.GroupBy(d => new { d.IDINDICADOR, d.MATRICULA, d.IDGDA_HOME, d.IDGDA_PERIODO, d.IDGDA_SITE }).Select(item => new semanalMensalConsolidado
                {
                    IDGDA_HOME = item.Key.IDGDA_HOME,
                    IDGDA_PERIODO = item.Key.IDGDA_PERIODO,
                    IDGDA_SITE = item.Key.IDGDA_SITE,

                    NOME = item.First().NOME,
                    CODGIP = item.First().CODGIP,
                    SETOR = item.First().SETOR,

                    MATRICULA = item.First().MATRICULA,
                    CARGO = item.First().CARGO,
                    //CODGIP = item.First().CODGIP,
                    //SETOR = item.First().SETOR,
                    IDINDICADOR = item.Key.IDINDICADOR,
                    INDICADOR = item.First().INDICADOR,
                    QTD = item.Sum(d => d.QTD),
                    //META = Math.Round(item.Sum(d => d.META) / item.Count(), 2, MidpointRounding.AwayFromZero),
                    META = Math.Round(item.Sum(d => d.META) / (item.Sum(d => d.QTD_META) > 0 ? item.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                    FACTOR0 = item.Sum(d => d.FACTOR0),
                    FACTOR1 = item.Sum(d => d.FACTOR1),
                    //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                    min1 = Math.Round(item.Sum(d => d.min1) / (item.Sum(d => d.QTD_META) > 0 ? item.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                    min2 = Math.Round(item.Sum(d => d.min2) / (item.Sum(d => d.QTD_META) > 0 ? item.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                    min3 = Math.Round(item.Sum(d => d.min3) / (item.Sum(d => d.QTD_META) > 0 ? item.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                    min4 = Math.Round(item.Sum(d => d.min4) / (item.Sum(d => d.QTD_META) > 0 ? item.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                    CONTA = item.First().CONTA,
                    BETTER = item.First().BETTER,
                    //META_MAXIMA_MOEDAS = Math.Round(item.Sum(d => d.META_MAXIMA_MOEDAS) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                    META_MAXIMA_MOEDAS = item.Sum(d => d.QTD_MON != 0 ? Math.Round(d.QTD_MON_TOTAL / d.QTD_MON, 0, MidpointRounding.AwayFromZero) : 0),

                    MATRICULASUPERVISOR = item.First().MATRICULASUPERVISOR,
                    NOMESUPERVISOR = item.First().NOMESUPERVISOR,
                    MATRICULACOORDENADOR = item.First().MATRICULACOORDENADOR,
                    NOMECOORDENADOR = item.First().NOMECOORDENADOR,
                    MATRICULAGERENTE2 = item.First().MATRICULAGERENTE2,
                    NOMEGERENTE2 = item.First().NOMEGERENTE2,
                    MATRICULAGERENTE1 = item.First().MATRICULAGERENTE1,
                    NOMEGERENTE1 = item.First().NOMEGERENTE1,
                    MATRICULADIRETOR = item.First().MATRICULADIRETOR,
                    NOMEDIRETOR = item.First().NOMEDIRETOR,
                    MATRICULACEO = item.First().MATRICULACEO,
                    NOMECEO = item.First().NOMECEO,

                    SUMDIASLOGADOS = item.Sum(d => d.SUMDIASLOGADOS),
                    SUMDIASESCALADOS = item.Sum(d => d.SUMDIASESCALADOS),
                    SUMDIASLOGADOSESCALADOS = item.Sum(d => d.SUMDIASLOGADOSESCALADOS),

                    //MOEDA_GANHA =  Math.Round(item.Sum(d => d.MOEDA_GANHA) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                    MOEDA_GANHA = Math.Round(item.Sum(d => d.MOEDA_GANHA), 0, MidpointRounding.AwayFromZero),
                    TYPE = item.First().TYPE,
                }).ToList();
            }
            else
            {
                retorno = rmams.GroupBy(d => new { d.IDINDICADOR, d.MATRICULA }).Select(item => new semanalMensalConsolidado
                {
                    IDGDA_HOME = item.First().IDGDA_HOME,
                    IDGDA_PERIODO = item.First().IDGDA_PERIODO,
                    IDGDA_SITE = item.First().IDGDA_SITE,

                    NOME = item.First().NOME,
                    CODGIP = item.First().CODGIP,
                    SETOR = item.First().SETOR,

                    MATRICULA = item.First().MATRICULA,
                    CARGO = item.First().CARGO,
                    //CODGIP = item.First().CODGIP,
                    //SETOR = item.First().SETOR,
                    IDINDICADOR = item.Key.IDINDICADOR,
                    INDICADOR = item.First().INDICADOR,
                    QTD = item.Sum(d => d.QTD),
                    //META = Math.Round(item.Sum(d => d.META) / item.Count(), 2, MidpointRounding.AwayFromZero),
                    META = Math.Round(item.Sum(d => d.META) / (item.Sum(d => d.QTD_META) > 0 ? item.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                    FACTOR0 = item.Sum(d => d.FACTOR0),
                    FACTOR1 = item.Sum(d => d.FACTOR1),
                    //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                    min1 = Math.Round(item.Sum(d => d.min1) / (item.Sum(d => d.QTD_META) > 0 ? item.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                    min2 = Math.Round(item.Sum(d => d.min2) / (item.Sum(d => d.QTD_META) > 0 ? item.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                    min3 = Math.Round(item.Sum(d => d.min3) / (item.Sum(d => d.QTD_META) > 0 ? item.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                    min4 = Math.Round(item.Sum(d => d.min4) / (item.Sum(d => d.QTD_META) > 0 ? item.Sum(d => d.QTD_META) : 1), 2, MidpointRounding.AwayFromZero),
                    CONTA = item.First().CONTA,
                    BETTER = item.First().BETTER,
                    //META_MAXIMA_MOEDAS = Math.Round(item.Sum(d => d.META_MAXIMA_MOEDAS) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                    META_MAXIMA_MOEDAS = item.Sum(d => d.QTD_MON != 0 ? Math.Round(d.QTD_MON_TOTAL / d.QTD_MON, 0, MidpointRounding.AwayFromZero) : 0),

                    MATRICULASUPERVISOR = item.First().MATRICULASUPERVISOR,
                    NOMESUPERVISOR = item.First().NOMESUPERVISOR,
                    MATRICULACOORDENADOR = item.First().MATRICULACOORDENADOR,
                    NOMECOORDENADOR = item.First().NOMECOORDENADOR,
                    MATRICULAGERENTE2 = item.First().MATRICULAGERENTE2,
                    NOMEGERENTE2 = item.First().NOMEGERENTE2,
                    MATRICULAGERENTE1 = item.First().MATRICULAGERENTE1,
                    NOMEGERENTE1 = item.First().NOMEGERENTE1,
                    MATRICULADIRETOR = item.First().MATRICULADIRETOR,
                    NOMEDIRETOR = item.First().NOMEDIRETOR,
                    MATRICULACEO = item.First().MATRICULACEO,
                    NOMECEO = item.First().NOMECEO,

                    SUMDIASLOGADOS = item.Sum(d => d.SUMDIASLOGADOS),
                    SUMDIASESCALADOS = item.Sum(d => d.SUMDIASESCALADOS),
                    SUMDIASLOGADOSESCALADOS = item.Sum(d => d.SUMDIASLOGADOSESCALADOS),

                    //MOEDA_GANHA =  Math.Round(item.Sum(d => d.MOEDA_GANHA) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                    MOEDA_GANHA = Math.Round(item.Sum(d => d.MOEDA_GANHA), 0, MidpointRounding.AwayFromZero),
                    TYPE = item.First().TYPE,
                }).ToList();
            }


            for (int i = 0; i < retorno.Count; i++)
            {
                if (retorno[i].IDINDICADOR == "3")
                {
                    bool parou = true;
                }

                semanalMensalConsolidado consolidado = retorno[i];
                retorno[i] = DoCalculateFinal(consolidado);
            }
            //if (inputModel.basket == true)
            //{


            List<semanalMensalConsolidado> cii = new List<semanalMensalConsolidado>();
            if (agruparResultado == true)
            {
                cii = retorno.GroupBy(d => new { d.MATRICULA, d.IDGDA_HOME, d.IDGDA_PERIODO, d.IDGDA_SITE }).Select(item => new semanalMensalConsolidado
                {
                    MATRICULA = item.Key.MATRICULA,
                    CARGO = item.First().CARGO,
                    //CODGIP = item.First().CODGIP,
                    //SETOR = item.First().SETOR,

                    IDGDA_HOME = item.Key.IDGDA_HOME,
                    IDGDA_PERIODO = item.Key.IDGDA_PERIODO,
                    IDGDA_SITE = item.Key.IDGDA_SITE,

                    NOME = item.First().NOME,
                    CODGIP = item.First().CODGIP,
                    SETOR = item.First().SETOR,
                    IDINDICADOR = "10000012",
                    INDICADOR = "Cesta de Indicadores",
                    QTD = 0,
                    RESULTADO = item.Sum(d => d.MOEDA_GANHA),
                    META = item.Sum(d => d.META_MAXIMA_MOEDAS),

                    FACTOR0 = 0,
                    FACTOR1 = 0,
                    //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                    min1 = 0,
                    min2 = 0,
                    min3 = 0,
                    min4 = 0,
                    MATRICULASUPERVISOR = item.First().MATRICULASUPERVISOR,
                    NOMESUPERVISOR = item.First().NOMESUPERVISOR,
                    MATRICULACOORDENADOR = item.First().MATRICULACOORDENADOR,
                    NOMECOORDENADOR = item.First().NOMECOORDENADOR,
                    MATRICULAGERENTE2 = item.First().MATRICULAGERENTE2,
                    NOMEGERENTE2 = item.First().NOMEGERENTE2,
                    MATRICULAGERENTE1 = item.First().MATRICULAGERENTE1,
                    NOMEGERENTE1 = item.First().NOMEGERENTE1,
                    MATRICULADIRETOR = item.First().MATRICULADIRETOR,
                    NOMEDIRETOR = item.First().NOMEDIRETOR,
                    MATRICULACEO = item.First().MATRICULACEO,
                    NOMECEO = item.First().NOMECEO,
                    CONTA = "",
                    BETTER = "BIGGER_BETTER",
                    //META_MAXIMA_MOEDAS = Math.Round(item.Sum(d => d.META_MAXIMA_MOEDAS) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                    META_MAXIMA_MOEDAS = item.Sum(d => d.META_MAXIMA_MOEDAS),
                    SUMDIASLOGADOS = 0,
                    SUMDIASESCALADOS = 0,
                    SUMDIASLOGADOSESCALADOS = 0,
                    MONETIZATION = true,
                    //MOEDA_GANHA =  Math.Round(item.Sum(d => d.MOEDA_GANHA) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                    MOEDA_GANHA = item.Sum(d => d.MOEDA_GANHA),
                    TYPE = "INTEGER",
                }).ToList();
            }
            else
            {
                cii = retorno.GroupBy(d => new { d.MATRICULA }).Select(item => new semanalMensalConsolidado
                {
                    MATRICULA = item.Key.MATRICULA,
                    CARGO = item.First().CARGO,
                    //CODGIP = item.First().CODGIP,
                    //SETOR = item.First().SETOR,

                    IDGDA_HOME = item.First().IDGDA_HOME,
                    IDGDA_PERIODO = item.First().IDGDA_PERIODO,
                    IDGDA_SITE = item.First().IDGDA_SITE,

                    NOME = item.First().NOME,
                    CODGIP = item.First().CODGIP,
                    SETOR = item.First().SETOR,
                    IDINDICADOR = "10000012",
                    INDICADOR = "Cesta de Indicadores",
                    QTD = 0,
                    RESULTADO = item.Sum(d => d.MOEDA_GANHA),
                    META = item.Sum(d => d.META_MAXIMA_MOEDAS),

                    FACTOR0 = 0,
                    FACTOR1 = 0,
                    //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                    min1 = 0,
                    min2 = 0,
                    min3 = 0,
                    min4 = 0,
                    MATRICULASUPERVISOR = item.First().MATRICULASUPERVISOR,
                    NOMESUPERVISOR = item.First().NOMESUPERVISOR,
                    MATRICULACOORDENADOR = item.First().MATRICULACOORDENADOR,
                    NOMECOORDENADOR = item.First().NOMECOORDENADOR,
                    MATRICULAGERENTE2 = item.First().MATRICULAGERENTE2,
                    NOMEGERENTE2 = item.First().NOMEGERENTE2,
                    MATRICULAGERENTE1 = item.First().MATRICULAGERENTE1,
                    NOMEGERENTE1 = item.First().NOMEGERENTE1,
                    MATRICULADIRETOR = item.First().MATRICULADIRETOR,
                    NOMEDIRETOR = item.First().NOMEDIRETOR,
                    MATRICULACEO = item.First().MATRICULACEO,
                    NOMECEO = item.First().NOMECEO,
                    CONTA = "",
                    BETTER = "BIGGER_BETTER",
                    //META_MAXIMA_MOEDAS = Math.Round(item.Sum(d => d.META_MAXIMA_MOEDAS) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                    META_MAXIMA_MOEDAS = item.Sum(d => d.META_MAXIMA_MOEDAS),
                    SUMDIASLOGADOS = 0,
                    SUMDIASESCALADOS = 0,
                    SUMDIASLOGADOSESCALADOS = 0,
                    MONETIZATION = true,
                    //MOEDA_GANHA =  Math.Round(item.Sum(d => d.MOEDA_GANHA) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                    MOEDA_GANHA = item.Sum(d => d.MOEDA_GANHA),
                    TYPE = "INTEGER",
                }).ToList();
            }









            List<semanalMensalConsolidado> ci = new List<semanalMensalConsolidado>();
            //foreach (semanalMensalConsolidado item in retorno)
            //{
            //    semanalMensalConsolidado cestaIndicadores = new semanalMensalConsolidado
            //    {
            //        MATRICULA = item.MATRICULA,
            //        CARGO = item.CARGO,
            //        //CODGIP = item.First().CODGIP,
            //        //SETOR = item.First().SETOR,
            //        NOME = item.NOME,
            //        CODGIP = item.CODGIP,
            //        SETOR = item.SETOR,
            //        IDINDICADOR = "10000012",
            //        INDICADOR = "Cesta de Indicadores",
            //        QTD = 0,
            //        RESULTADO = item.MOEDA_GANHA,
            //        META = item.META_MAXIMA_MOEDAS,
            //        FACTOR0 = 0,
            //        FACTOR1 = 0,
            //        //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
            //        min1 = 0,
            //        min2 = 0,
            //        min3 = 0,
            //        min4 = 0,
            //        MATRICULASUPERVISOR = item.MATRICULASUPERVISOR,
            //        NOMESUPERVISOR = item.NOMESUPERVISOR,
            //        MATRICULACOORDENADOR = item.MATRICULACOORDENADOR,
            //        NOMECOORDENADOR = item.NOMECOORDENADOR,
            //        MATRICULAGERENTE2 = item.MATRICULAGERENTE2,
            //        NOMEGERENTE2 = item.NOMEGERENTE2,
            //        MATRICULAGERENTE1 = item.MATRICULAGERENTE1,
            //        NOMEGERENTE1 = item.NOMEGERENTE1,
            //        MATRICULADIRETOR = item.MATRICULADIRETOR,
            //        NOMEDIRETOR = item.NOMEDIRETOR,
            //        MATRICULACEO = item.MATRICULACEO,
            //        NOMECEO = item.NOMECEO,
            //        CONTA = "",
            //        BETTER = "BIGGER_BETTER",
            //        //META_MAXIMA_MOEDAS = Math.Round(item.Sum(d => d.META_MAXIMA_MOEDAS) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
            //        META_MAXIMA_MOEDAS = item.META_MAXIMA_MOEDAS,
            //        SUMDIASLOGADOS = 0,
            //        SUMDIASESCALADOS = 0,
            //        SUMDIASLOGADOSESCALADOS = 0,
            //        MONETIZATION = true,
            //        //MOEDA_GANHA =  Math.Round(item.Sum(d => d.MOEDA_GANHA) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
            //        MOEDA_GANHA = item.MOEDA_GANHA,
            //        TYPE = "INTEGER",
            //    };




            foreach (semanalMensalConsolidado cestaIndicadores in cii)
            {


                Funcoes.cestaMetrica cm = Funcoes.getInfMetricBasket();

                if (cestaIndicadores.META_MAXIMA_MOEDAS == 0)
                {
                    cestaIndicadores.PERCENTUAL = 100;
                    cestaIndicadores.IDGRUPO = lgroup1.id;
                    cestaIndicadores.GRUPO = lgroup1.name;
                    cestaIndicadores.IMAGEMGRUPO = lgroup1.image;
                    cestaIndicadores.NOME_NIVEL = lgroup1.alias;
                }
                else
                {
                    double calculo = (cestaIndicadores.MOEDA_GANHA / cestaIndicadores.META_MAXIMA_MOEDAS) * 100;

                    //Verifica a qual grupo pertence
                    if (calculo >= cm.min1)
                    {
                        cestaIndicadores.GRUPO = lgroup1.name;
                        cestaIndicadores.IDGRUPO = lgroup1.id;
                        cestaIndicadores.IMAGEMGRUPO = lgroup1.image;
                        cestaIndicadores.NOME_NIVEL = lgroup1.alias;
                    }
                    else if (calculo >= cm.min2)
                    {
                        cestaIndicadores.GRUPO = lgroup2.name;
                        cestaIndicadores.IDGRUPO = lgroup2.id;
                        cestaIndicadores.IMAGEMGRUPO = lgroup2.image;
                        cestaIndicadores.NOME_NIVEL = lgroup2.alias;
                    }
                    else if (calculo >= cm.min3)
                    {
                        cestaIndicadores.GRUPO = lgroup3.name;
                        cestaIndicadores.IDGRUPO = lgroup3.id;
                        cestaIndicadores.IMAGEMGRUPO = lgroup3.image;
                        cestaIndicadores.NOME_NIVEL = lgroup3.alias;
                    }
                    else if (calculo >= cm.min4)
                    {
                        cestaIndicadores.GRUPO = lgroup4.name;
                        cestaIndicadores.IDGRUPO = lgroup4.id;
                        cestaIndicadores.IMAGEMGRUPO = lgroup4.image;
                        cestaIndicadores.NOME_NIVEL = lgroup4.alias;
                    }
                    else
                    {
                        cestaIndicadores.GRUPO = lgroup4.name;
                        cestaIndicadores.IDGRUPO = lgroup4.id;
                        cestaIndicadores.IMAGEMGRUPO = lgroup4.image;
                        cestaIndicadores.NOME_NIVEL = lgroup4.alias;
                    }

                    cestaIndicadores.PERCENTUAL = Math.Round(calculo, 2, MidpointRounding.AwayFromZero);
                }
                ci.Insert(0, cestaIndicadores);
            }

            //List<semanalMensalConsolidado> ASAS = ci.FindAll(it => it.MATRICULA == "695573").ToList();

            retorno = retorno.Concat(ci).ToList();
            return retorno;
        }

        public static List<returnResponseSemanalMensal> ConsolidatedResultSemanalDiario(int semana, int mes, int ano)
        {
            int dia = 1;
            string dtInicial = "";
            string dtFinal = "";
            if (semana == 1)
            {
                dia = 1;
                DateTime dtI = new DateTime(ano, mes, 1);
                dtInicial = dtI.ToString("yyyy-MM-dd");
                DateTime dtF = new DateTime(ano, mes, 8);
                dtFinal = dtF.ToString("yyyy-MM-dd");
            }
            else if (semana == 2)
            {
                dia = 9;
                DateTime dtI = new DateTime(ano, mes, 9);
                dtInicial = dtI.ToString("yyyy-MM-dd");
                DateTime dtF = new DateTime(ano, mes, 15);
                dtFinal = dtF.ToString("yyyy-MM-dd");
            }
            else if (semana == 3)
            {
                dia = 16;
                DateTime dtI = new DateTime(ano, mes, 16);
                dtInicial = dtI.ToString("yyyy-MM-dd");
                DateTime dtF = new DateTime(ano, mes, 22);
                dtFinal = dtF.ToString("yyyy-MM-dd");
            }
            else if (semana == 4)
            {
                dia = 23;
                int ultimoDiaMes = DateTime.DaysInMonth(ano, mes);
                DateTime dtI = new DateTime(ano, mes, 23);
                dtInicial = dtI.ToString("yyyy-MM-dd");
                DateTime dtF = new DateTime(ano, mes, ultimoDiaMes);
                dtFinal = dtF.ToString("yyyy-MM-dd");
            }
            else if (semana == 0)
            {
                int ultimoDiaMes = DateTime.DaysInMonth(ano, mes);
                //int ultimoDiaMes = 01;
                DateTime dtI = new DateTime(ano, mes, 01);
                dtInicial = dtI.ToString("yyyy-MM-dd");
                DateTime dtF = new DateTime(ano, mes, ultimoDiaMes);
                dtFinal = dtF.ToString("yyyy-MM-dd");
            }
            else
            {
                return null;
            }

            //Realiza a query que retorna todas as informações consolidada do colaborador filtrado.
            List<semanalMensalConsolidado> rmams = new List<semanalMensalConsolidado>();
            rmams = ReturnHomeResultConsolidated(dtInicial, dtFinal);

            rmams = rmams.Where(rd => rd.CARGO == "AGENTE").ToList();

            List<semanalMensalConsolidado> listaIndicadorAcesso = new List<semanalMensalConsolidado>();
            listaIndicadorAcesso = ReturnHomeResultConsolidatedAccess(dtInicial, dtFinal, "'10000013','10000014'");

            rmams = rmams.Concat(listaIndicadorAcesso).ToList();


            //Retirar Tx de Acesso
            rmams = rmams.Where(rd => rd.IDINDICADOR != "-1").ToList();


            // ORDERAR POR INDICADOR ORDER DESCRESENTE DATA DE PAGAMENTO.
            rmams = rmams.OrderBy(r => r.IDINDICADOR).OrderByDescending(r => r.DATAPAGAMENTO).ToList();



            List<semanalMensalConsolidado> tst = rmams.Where(rd => rd.MATRICULA == "600321").ToList();



            List<semanalMensalConsolidado> retorno = new List<semanalMensalConsolidado>();

            if (semana == 0)
            {
                inserirInfBanco(rmams, 0, 0, 0, 0, true, true);
            }
            
            retorno = agrupamentoTratativa(rmams, true);

            inserirInfBanco(retorno, semana, dia, mes, ano, true);

            retorno.Clear();
            retorno = agrupamentoTratativa(rmams, false);

            inserirInfBanco(retorno, semana, dia, mes, ano, false);

            return null;
        }

        public static DataTable ToDataTable<T>(IEnumerable<T> items)
        {
            var dataTable = new DataTable(typeof(T).Name);

            // Obter propriedades públicas da classe
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Criação das colunas no DataTable
            foreach (var property in properties)
            {
                dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }

            // Adicionar linhas no DataTable
            foreach (var item in items)
            {
                var values = new object[properties.Length];
                for (var i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        public static void inserirInfBanco(List<semanalMensalConsolidado> retorno, int semana, int dia, int mes, int ano, bool agruparResultado, bool diario = false)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    if (diario == true)
                    {
                        int ultimoDiaMes = DateTime.DaysInMonth(ano, mes);
                        //int ultimoDiaMes = 01;
                        DateTime dtI = new DateTime(ano, mes, 01);
                        string dtInicial = dtI.ToString("yyyy-MM-dd");
                        DateTime dtF = new DateTime(ano, mes, ultimoDiaMes);
                        string dtFinal = dtF.ToString("yyyy-MM-dd");


                        StringBuilder stbDel = new StringBuilder();
                        stbDel.AppendFormat("DELETE TOP (10000) FROM GDA_PERFORMANCE WHERE ");
                        stbDel.AppendFormat($"DATA >= '{dtInicial}' ");
                        stbDel.AppendFormat($"AND DATA <= '{dtFinal}' ");

                        int rowsAffected;

                        using (SqlCommand command = new SqlCommand(stbDel.ToString(), connection))
                        {
                            connection.Open();

                            do
                            {
                                rowsAffected = command.ExecuteNonQuery();

                            } while (rowsAffected > 0);

                            connection.Close();
                        }

                        DataTable dataTable = ToDataTable(retorno);

                        string colunas = "";
                        foreach (DataColumn colunaAtual in dataTable.Columns)
                        {
                            var tipoColuna = colunaAtual.DataType;
                            string strTipoColunaSQL = "";

                            switch (tipoColuna.Name)
                            {
                                case "Int32":
                                    {
                                        strTipoColunaSQL = "INT";
                                        break;
                                    }

                                case "String":
                                    {
                                        strTipoColunaSQL = "VARCHAR(MAX)";
                                        break;
                                    }

                                case "DateTime":
                                    {
                                        strTipoColunaSQL = "DATETIME";
                                        break;
                                    }

                                case "Double":
                                    {
                                        strTipoColunaSQL = "FLOAT";
                                        break;
                                    }

                                default:
                                    {
                                        strTipoColunaSQL = "VARCHAR(MAX)";
                                        break;
                                    }
                            }

                            colunas = colunas == "" ? $" [{colunaAtual.ColumnName}] {strTipoColunaSQL} " : $"{colunas}, [{colunaAtual.ColumnName}] {strTipoColunaSQL}";
                        }

                        string commandText = $"CREATE TABLE #TEMPTABLE ({colunas});"; // Substitua as colunas com as adequadas do seu DataTable
                        SqlCommand createTableCommand = new SqlCommand(commandText, connection);
                        createTableCommand.ExecuteNonQuery();

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName = "#TEMPTABLE";
                            bulkCopy.BulkCopyTimeout = 0;

                            // Define o número de linhas a serem notificadas
                            bulkCopy.NotifyAfter = 1; // Notifica a cada 1000 linhas copiadas, por exemplo

                            bulkCopy.WriteToServer(dataTable);
                        }

                        foreach (semanalMensalConsolidado item in retorno)
                        {
                            try
                            {


                                connection.Open();
                                StringBuilder stb = new StringBuilder();
                                stb.AppendFormat("INSERT INTO GDA_PERFORMANCE ");
                                stb.AppendFormat("(");
                                stb.AppendFormat("DATA,");
                                stb.AppendFormat("IDENTIFICACAO,");
                                stb.AppendFormat("CHAVE_EXTERNA,");
                                stb.AppendFormat("NOME_USUARIO, ");
                                stb.AppendFormat("NOME_NIVEL_HIERARQUIA, ");
                                stb.AppendFormat("IDENTIFICACAO_SUPERVISOR,");//     
                                stb.AppendFormat("CHAVE_EXTERNA_SUPERVISOR,");
                                stb.AppendFormat("NOME_SUPERVISOR, ");
                                stb.AppendFormat("IDENTIFICACAO_COORDENADOR,");
                                stb.AppendFormat("CHAVE_EXTERNA_COORDENADOR,");
                                stb.AppendFormat("NOME_COORDENADOR, ");
                                stb.AppendFormat("IDENTIFICACAO_GERENTE_II,");
                                stb.AppendFormat("CHAVE_EXTERNA_GERENTE_II,");
                                stb.AppendFormat("NOME_GERENTE_II, ");
                                stb.AppendFormat("IDENTIFICACAO_GERENTE_I,");
                                stb.AppendFormat("CHAVE_EXTERNA_GERENTE_I,");
                                stb.AppendFormat("NOME_GERENTE_I, ");
                                stb.AppendFormat("IDENTIFICACAO_DIRETOR,");
                                stb.AppendFormat("CHAVE_EXTERNA_DIRETOR,");
                                stb.AppendFormat("NOME_DIRETOR, ");
                                stb.AppendFormat("IDENTIFICACAO_CEO,");
                                stb.AppendFormat("CHAVE_EXTERNA_CEO,");
                                stb.AppendFormat("NOME_CEO, ");
                                stb.AppendFormat("CD_GIP,");
                                stb.AppendFormat("SETOR, ");
                                stb.AppendFormat("ID_INDICADOR,");
                                stb.AppendFormat("NOME_INDICADOR, ");
                                stb.AppendFormat("RESULTADO,");
                                stb.AppendFormat("FATOR, ");
                                stb.AppendFormat("FATOR_1,");
                                stb.AppendFormat("FATOR_2,");
                                stb.AppendFormat("RESULTADO_CALCULADO,");
                                stb.AppendFormat("PERCENTUAL_ATINGIMENTO,");
                                stb.AppendFormat("META,");
                                stb.AppendFormat("GANHO,");
                                stb.AppendFormat("MAX_GANHO,");
                                stb.AppendFormat("ID_GRUPO,");
                                stb.AppendFormat("NOME_GRUPO, ");
                                stb.AppendFormat("NOME_NIVEL, ");
                                stb.AppendFormat("DATA_ATUALIZACAO,");
                                stb.AppendFormat("HOME_BASED,");
                                stb.AppendFormat("LOCAL, ");
                                stb.AppendFormat("NOVATO, ");
                                stb.AppendFormat("SITE");
                                stb.AppendFormat(")");
                                stb.AppendFormat(") SELECT ");
                                stb.AppendFormat("DATA_PAGAMENTO,"); //DATA, 
                                stb.AppendFormat("CONCAT('BC', MATRICULA),"); //IDENTIFICACAO, 
                                stb.AppendFormat("MATRICULA,"); //CHAVE_EXTERNA, 
                                stb.AppendFormat("NOME,"); //NOME_USUARIO, 
                                stb.AppendFormat("CARGO,"); //NOME_NIVEL_HIERARQUIA, 
                                stb.AppendFormat("CONCAT('BC', MATRICULASUPERVISOR),"); //IDENTIFICACAO_SUPERVISOR, 
                                stb.AppendFormat("MATRICULASUPERVISOR,"); //CHAVE_EXTERNA_SUPERVISOR, 
                                stb.AppendFormat("NOMESUPERVISOR,"); //NOME_SUPERVISOR,
                                stb.AppendFormat("CONCAT('BC', MATRICULACOORDENADOR),"); //IDENTIFICACAO_COORDENADOR, 
                                stb.AppendFormat("MATRICULACOORDENADOR,"); //CHAVE_EXTERNA_COORDENADOR, 
                                stb.AppendFormat("NOMECOORDENADOR,"); //NOME_COORDENADOR,
                                stb.AppendFormat("CONCAT('BC', MATRICULAGERENTE2),"); //IDENTIFICACAO_GERENTE_II, 
                                stb.AppendFormat("MATRICULAGERENTE2,"); //CHAVE_EXTERNA_GERENTE_II, 
                                stb.AppendFormat("NOMEGERENTE2,"); //NOME_GERENTE_II,
                                stb.AppendFormat("CONCAT('BC', MATRICULAGERENTE1),"); //IDENTIFICACAO_GERENTE_I, 
                                stb.AppendFormat("MATRICULAGERENTE1,"); //CHAVE_EXTERNA_GERENTE_I, 
                                stb.AppendFormat("NOMEGERENTE1,"); //NOME_GERENTE_I,
                                stb.AppendFormat("CONCAT('BC', MATRICULADIRETOR),"); //IDENTIFICACAO_DIRETOR, 
                                stb.AppendFormat("MATRICULADIRETOR,"); //CHAVE_EXTERNA_DIRETOR, 
                                stb.AppendFormat("NOMEDIRETOR,"); //NOME_DIRETOR,
                                stb.AppendFormat("CONCAT('BC', MATRICULACEO),"); //IDENTIFICACAO_CEO, 
                                stb.AppendFormat("MATRICULACEO,"); //CHAVE_EXTERNA_CEO, 
                                stb.AppendFormat("NOMECEO,"); //NOME_CEO,
                                stb.AppendFormat("CODGIP,"); //CD_GIP, 
                                stb.AppendFormat("SETOR,"); //SETOR, 
                                stb.AppendFormat("IDINDICADOR,"); //ID_INDICADOR, 
                                stb.AppendFormat("INDICADOR,"); //NOME_INDICADOR, 
                                stb.AppendFormat("RESULTADO,"); //RESULTADO,
                                stb.AppendFormat("CONCAT(FACTOR0,';', FACTOR1),"); //FATOR, 
                                stb.AppendFormat("FACTOR0,"); //FATOR_1, 
                                stb.AppendFormat("FACTOR1,"); //FATOR_2, 
                                stb.AppendFormat("RESULTADO,"); //RESULTADO_CALCULADO, 
                                stb.AppendFormat("PERCENTUAL,"); //PERCENTUAL_ATINGIMENTO, 
                                stb.AppendFormat("META,"); //META, 
                                stb.AppendFormat("MOEDA_GANHA,"); //GANHO, 
                                stb.AppendFormat("META_MAXIMA_MOEDAS,"); //MAX_GANHO, 
                                stb.AppendFormat("IDGRUPO,"); //ID_GRUPO, 
                                stb.AppendFormat("GRUPO,"); //NOME_GRUPO, 
                                stb.AppendFormat("NOME_NIVEL,"); //NOME_NIVEL, 
                                stb.AppendFormat("GETDATE(),"); //DATA_ATUALIZACAO,
                                stb.AppendFormat("IDGDA_HOME,"); //HOME_BASED, 
                                stb.AppendFormat("'',"); //LOCAL, 
                                stb.AppendFormat("NOVATO,"); //NOVATO, 
                                stb.AppendFormat("IDGDA_SITE,"); //SITE, 
                                stb.AppendFormat("FROM #TEMPTABLE ");




                                stb.AppendFormat("VALUES");
                                stb.AppendFormat("(");
                                //stb.AppendFormat($"'{Convert.ToDateTime(item.DATAPAGAMENTO).ToString("yyyy-MM-dd")}', "); //DATA
                                //stb.AppendFormat($"'BC{item.Matricula}', "); //IDENTIFICACAO
                                //stb.AppendFormat($"'{item.Matricula}', "); //CHAVE_EXTERNA
                                //stb.AppendFormat($"'{item.NomeColaborador}', "); //NOME_USUARIO
                                //stb.AppendFormat($"'{item.Cargo}', "); //NOME_NIVEL_HIERARQUIA
                                //stb.AppendFormat($"'BC{item.MatriculaSupervisor}', "); //IDENTIFICACAO_SUPERVISOR
                                //stb.AppendFormat($"'{item.MatriculaSupervisor}', "); //CHAVE_EXTERNA_SUPERVISOR
                                //stb.AppendFormat($"'{item.NomeSupervisor}', "); //NOME_SUPERVISOR
                                //stb.AppendFormat($"'BC{item.MatriculaCoordenador}', "); //IDENTIFICACAO_COORDENADOR
                                //stb.AppendFormat($"'{item.MatriculaCoordenador}', "); //CHAVE_EXTERNA_COORDENADOR
                                //stb.AppendFormat($"'{item.NomeCoordenador}', "); //NOME_COORDENADOR
                                //stb.AppendFormat($"'BC{item.MatriculaGerente2}', "); //IDENTIFICACAO_GERENTE_II
                                //stb.AppendFormat($"'{item.MatriculaGerente2}', "); //CHAVE_EXTERNA_GERENTE_II
                                //stb.AppendFormat($"'{item.NomeGerente2}', "); //NOME_GERENTE_II
                                //stb.AppendFormat($"'BC{item.MatriculaGerente1}', "); //IDENTIFICACAO_GERENTE_I
                                //stb.AppendFormat($"'{item.MatriculaGerente1}', "); //CHAVE_EXTERNA_GERENTE_I
                                //stb.AppendFormat($"'{item.NomeGerente1}', "); //NOME_GERENTE_I
                                //stb.AppendFormat($"'BC{item.MatriculaDiretor}', "); //IDENTIFICACAO_DIRETOR
                                //stb.AppendFormat($"'{item.MatriculaDiretor}', "); //CHAVE_EXTERNA_DIRETOR
                                //stb.AppendFormat($"'{item.NomeDiretor}', "); //NOME_DIRETOR
                                //stb.AppendFormat($"'BC{item.MatriculaCEO}', "); //IDENTIFICACAO_CEO
                                //stb.AppendFormat($"'{item.MatriculaCEO}', "); //CHAVE_EXTERNA_CEO
                                //stb.AppendFormat($"'{item.NomeCEO}', "); //NOME_CEO
                                //stb.AppendFormat($"'{item.CodigoGIP}', "); //CD_GIP
                                //stb.AppendFormat($"'{item.Setor}', "); //SETOR
                                //stb.AppendFormat($"'{item.IDIndicador}', "); //ID_INDICADOR
                                //stb.AppendFormat($"'{item.Indicador}', "); //NOME_INDICADOR
                                //stb.AppendFormat($"'{item.Resultado.Replace(",", ".")}', "); //RESULTADO
                                //stb.AppendFormat($"'{item.fatores}', "); //FATOR
                                //stb.AppendFormat($"'{item.fator0}', "); //FATOR_1
                                //stb.AppendFormat($"'{item.fator1}', "); //FATOR_2
                                //stb.AppendFormat($"'{item.Resultado.Replace(",", ".")}', "); //RESULTADO_CALCULADO
                                //stb.AppendFormat($"'{item.Percentual.Replace(",", ".")}', "); //PERCENTUAL_ATINGIMENTO
                                //stb.AppendFormat($"'{item.Meta.Replace(",", ".")}', "); //META
                                //stb.AppendFormat($"'{item.GanhoEmMoedas}', "); //GANHO
                                //stb.AppendFormat($"'{item.MetaMaximaMoedas}', "); //MAX_GANHO
                                //stb.AppendFormat($"'{item.idGrupo}', "); //ID_GRUPO
                                //stb.AppendFormat($"'{item.aliasGrupo}', "); //NOME_GRUPO
                                //stb.AppendFormat($"'{item.Grupo}', "); //NOME_NIVEL
                                //stb.AppendFormat($"GETDATE(), "); //DATA_ATUALIZACAO
                                //stb.AppendFormat($"'{item.Home}', "); //HOME_BASED
                                //stb.AppendFormat($"'', "); //LOCAL
                                //stb.AppendFormat($"'', "); //NOVATO
                                //stb.AppendFormat($"'{item.Site}' "); //SITE
                                stb.AppendFormat(")");

                                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                                {
                                    command.ExecuteNonQuery();
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        return;
                    }

                    if (semana != 0 && agruparResultado == true)
                    {
                        StringBuilder stb = new StringBuilder();
                        stb.Append("DELETE FROM GDA_PERFORMANCE_SEMANAL_RESULTADO ");
                        stb.AppendFormat("WHERE SEMANA = {0} ", semana);
                        stb.AppendFormat("AND MES = {0} ", mes);
                        stb.AppendFormat("AND ANO = {0} ", ano);

                        connection.Open();
                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            command.CommandTimeout = 0;
                            command.ExecuteNonQuery();
                        }

                        DataTable dataTable = ToDataTable(retorno);

                        string colunas = "";
                        foreach (DataColumn colunaAtual in dataTable.Columns)
                        {
                            var tipoColuna = colunaAtual.DataType;
                            string strTipoColunaSQL = "";

                            switch (tipoColuna.Name)
                            {
                                case "Int32":
                                    {
                                        strTipoColunaSQL = "INT";
                                        break;
                                    }

                                case "String":
                                    {
                                        strTipoColunaSQL = "VARCHAR(MAX)";
                                        break;
                                    }

                                case "DateTime":
                                    {
                                        strTipoColunaSQL = "DATETIME";
                                        break;
                                    }

                                case "Double":
                                    {
                                        strTipoColunaSQL = "FLOAT";
                                        break;
                                    }

                                default:
                                    {
                                        strTipoColunaSQL = "VARCHAR(MAX)";
                                        break;
                                    }
                            }

                            colunas = colunas == "" ? $" [{colunaAtual.ColumnName}] {strTipoColunaSQL} " : $"{colunas}, [{colunaAtual.ColumnName}] {strTipoColunaSQL}";
                        }

                        string commandText = $"CREATE TABLE #TEMPTABLE ({colunas});"; // Substitua as colunas com as adequadas do seu DataTable
                        SqlCommand createTableCommand = new SqlCommand(commandText, connection);
                        createTableCommand.ExecuteNonQuery();

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName = "#TEMPTABLE";
                            bulkCopy.BulkCopyTimeout = 0;

                            // Define o número de linhas a serem notificadas
                            bulkCopy.NotifyAfter = 1; // Notifica a cada 1000 linhas copiadas, por exemplo

                            bulkCopy.WriteToServer(dataTable);
                        }


                        DateTime st = new DateTime(ano, mes, dia);
                        string stt = st.ToString("yyyy-MM-dd");
                        stb.Clear();
                        stb.AppendFormat("INSERT INTO GDA_PERFORMANCE_SEMANAL_RESULTADO (");
                        stb.AppendFormat("DATA, ");
                        stb.AppendFormat("SEMANA, ");
                        stb.AppendFormat("MES, ");
                        stb.AppendFormat("ANO, ");
                        stb.AppendFormat("IDENTIFICACAO, ");
                        stb.AppendFormat("CHAVE_EXTERNA, ");
                        stb.AppendFormat("NOME_USUARIO, ");
                        stb.AppendFormat("NOME_NIVEL_HIERARQUIA, ");
                        stb.AppendFormat("IDENTIFICACAO_SUPERVISOR, ");
                        stb.AppendFormat("CHAVE_EXTERNA_SUPERVISOR, ");
                        stb.AppendFormat("NOME_SUPERVISOR,");
                        stb.AppendFormat("IDENTIFICACAO_COORDENADOR, ");
                        stb.AppendFormat("CHAVE_EXTERNA_COORDENADOR, ");
                        stb.AppendFormat("NOME_COORDENADOR,");
                        stb.AppendFormat("IDENTIFICACAO_GERENTE_II, ");
                        stb.AppendFormat("CHAVE_EXTERNA_GERENTE_II, ");
                        stb.AppendFormat("NOME_GERENTE_II,");
                        stb.AppendFormat("IDENTIFICACAO_GERENTE_I, ");
                        stb.AppendFormat("CHAVE_EXTERNA_GERENTE_I, ");
                        stb.AppendFormat("NOME_GERENTE_I,");
                        stb.AppendFormat("IDENTIFICACAO_DIRETOR, ");
                        stb.AppendFormat("CHAVE_EXTERNA_DIRETOR, ");
                        stb.AppendFormat("NOME_DIRETOR,");
                        stb.AppendFormat("IDENTIFICACAO_CEO, ");
                        stb.AppendFormat("CHAVE_EXTERNA_CEO, ");
                        stb.AppendFormat("NOME_CEO,");
                        stb.AppendFormat("CD_GIP, ");
                        stb.AppendFormat("SETOR, ");
                        stb.AppendFormat("ID_INDICADOR, ");
                        stb.AppendFormat("NOME_INDICADOR, ");
                        stb.AppendFormat("RESULTADO,");
                        stb.AppendFormat("FATOR, ");
                        stb.AppendFormat("FATOR_1, ");
                        stb.AppendFormat("FATOR_2, ");
                        stb.AppendFormat("RESULTADO_CALCULADO, ");
                        stb.AppendFormat("PERCENTUAL_ATINGIMENTO, ");
                        stb.AppendFormat("META, ");
                        stb.AppendFormat("GANHO, ");
                        stb.AppendFormat("MAX_GANHO, ");
                        stb.AppendFormat("ID_GRUPO, ");
                        stb.AppendFormat("NOME_GRUPO, ");
                        stb.AppendFormat("NOME_NIVEL, ");
                        stb.AppendFormat("DATA_ATUALIZACAO,");
                        stb.AppendFormat("NOVATO, ");
                        stb.AppendFormat("SITE, ");
                        stb.AppendFormat("HOME_BASED, ");
                        stb.AppendFormat("PERIODO ");
                        stb.AppendFormat(") SELECT ");
                        stb.AppendFormat("'{0}' AS DATA,", stt); //DATA, 
                        stb.AppendFormat("'{0}' AS SEMANA,", semana); //SEMANA, 
                        stb.AppendFormat("'{0}' AS MES,", mes); //MES, 
                        stb.AppendFormat("'{0}' AS ANO,", ano); //ANO, 
                        stb.AppendFormat("CONCAT('BC', MATRICULA),"); //IDENTIFICACAO, 
                        stb.AppendFormat("MATRICULA,"); //CHAVE_EXTERNA, 
                        stb.AppendFormat("NOME,"); //NOME_USUARIO, 
                        stb.AppendFormat("CARGO,"); //NOME_NIVEL_HIERARQUIA, 
                        stb.AppendFormat("CONCAT('BC', MATRICULASUPERVISOR),"); //IDENTIFICACAO_SUPERVISOR, 
                        stb.AppendFormat("MATRICULASUPERVISOR,"); //CHAVE_EXTERNA_SUPERVISOR, 
                        stb.AppendFormat("NOMESUPERVISOR,"); //NOME_SUPERVISOR,
                        stb.AppendFormat("CONCAT('BC', MATRICULACOORDENADOR),"); //IDENTIFICACAO_COORDENADOR, 
                        stb.AppendFormat("MATRICULACOORDENADOR,"); //CHAVE_EXTERNA_COORDENADOR, 
                        stb.AppendFormat("NOMECOORDENADOR,"); //NOME_COORDENADOR,
                        stb.AppendFormat("CONCAT('BC', MATRICULAGERENTE2),"); //IDENTIFICACAO_GERENTE_II, 
                        stb.AppendFormat("MATRICULAGERENTE2,"); //CHAVE_EXTERNA_GERENTE_II, 
                        stb.AppendFormat("NOMEGERENTE2,"); //NOME_GERENTE_II,
                        stb.AppendFormat("CONCAT('BC', MATRICULAGERENTE1),"); //IDENTIFICACAO_GERENTE_I, 
                        stb.AppendFormat("MATRICULAGERENTE1,"); //CHAVE_EXTERNA_GERENTE_I, 
                        stb.AppendFormat("NOMEGERENTE1,"); //NOME_GERENTE_I,
                        stb.AppendFormat("CONCAT('BC', MATRICULADIRETOR),"); //IDENTIFICACAO_DIRETOR, 
                        stb.AppendFormat("MATRICULADIRETOR,"); //CHAVE_EXTERNA_DIRETOR, 
                        stb.AppendFormat("NOMEDIRETOR,"); //NOME_DIRETOR,
                        stb.AppendFormat("CONCAT('BC', MATRICULACEO),"); //IDENTIFICACAO_CEO, 
                        stb.AppendFormat("MATRICULACEO,"); //CHAVE_EXTERNA_CEO, 
                        stb.AppendFormat("NOMECEO,"); //NOME_CEO,
                        stb.AppendFormat("CODGIP,"); //CD_GIP, 
                        stb.AppendFormat("SETOR,"); //SETOR, 
                        stb.AppendFormat("IDINDICADOR,"); //ID_INDICADOR, 
                        stb.AppendFormat("INDICADOR,"); //NOME_INDICADOR, 
                        stb.AppendFormat("RESULTADO,"); //RESULTADO,
                        stb.AppendFormat("CONCAT(FACTOR0,';', FACTOR1),"); //FATOR, 
                        stb.AppendFormat("FACTOR0,"); //FATOR_1, 
                        stb.AppendFormat("FACTOR1,"); //FATOR_2, 
                        stb.AppendFormat("RESULTADO,"); //RESULTADO_CALCULADO, 
                        stb.AppendFormat("PERCENTUAL,"); //PERCENTUAL_ATINGIMENTO, 
                        stb.AppendFormat("META,"); //META, 
                        stb.AppendFormat("MOEDA_GANHA,"); //GANHO, 
                        stb.AppendFormat("META_MAXIMA_MOEDAS,"); //MAX_GANHO, 
                        stb.AppendFormat("IDGRUPO,"); //ID_GRUPO, 
                        stb.AppendFormat("GRUPO,"); //NOME_GRUPO, 
                        stb.AppendFormat("NOME_NIVEL,"); //NOME_NIVEL, 
                        stb.AppendFormat("GETDATE(),"); //DATA_ATUALIZACAO,
                        stb.AppendFormat("NOVATO,"); //NOVATO, 
                        stb.AppendFormat("IDGDA_SITE,"); //SITE, 
                        stb.AppendFormat("IDGDA_HOME,"); //HOME_BASED, 
                        stb.AppendFormat("IDGDA_PERIODO "); //PERIODO 
                        stb.AppendFormat("FROM #TEMPTABLE ");

                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            command.CommandTimeout = 0;
                            command.ExecuteNonQuery();
                        }
                        connection.Close();
                    }
                    if (semana != 0 && agruparResultado == false)
                    {
                        StringBuilder stb = new StringBuilder();
                        stb.Append("DELETE FROM GDA_PERFORMANCE_SEMANAL ");
                        stb.AppendFormat("WHERE SEMANA = {0} ", semana);
                        stb.AppendFormat("AND MES = {0} ", mes);
                        stb.AppendFormat("AND ANO = {0} ", ano);

                        connection.Open();
                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            command.CommandTimeout = 0;
                            command.ExecuteNonQuery();
                        }

                        DataTable dataTable = ToDataTable(retorno);

                        string colunas = "";
                        foreach (DataColumn colunaAtual in dataTable.Columns)
                        {
                            var tipoColuna = colunaAtual.DataType;
                            string strTipoColunaSQL = "";

                            switch (tipoColuna.Name)
                            {
                                case "Int32":
                                    {
                                        strTipoColunaSQL = "INT";
                                        break;
                                    }

                                case "String":
                                    {
                                        strTipoColunaSQL = "VARCHAR(MAX)";
                                        break;
                                    }

                                case "DateTime":
                                    {
                                        strTipoColunaSQL = "DATETIME";
                                        break;
                                    }

                                case "Double":
                                    {
                                        strTipoColunaSQL = "FLOAT";
                                        break;
                                    }

                                default:
                                    {
                                        strTipoColunaSQL = "VARCHAR(MAX)";
                                        break;
                                    }
                            }

                            colunas = colunas == "" ? $" [{colunaAtual.ColumnName}] {strTipoColunaSQL} " : $"{colunas}, [{colunaAtual.ColumnName}] {strTipoColunaSQL}";
                        }

                        string commandText = $"CREATE TABLE #TEMPTABLE ({colunas});"; // Substitua as colunas com as adequadas do seu DataTable
                        SqlCommand createTableCommand = new SqlCommand(commandText, connection);
                        createTableCommand.ExecuteNonQuery();

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName = "#TEMPTABLE";
                            bulkCopy.BulkCopyTimeout = 0;

                            // Define o número de linhas a serem notificadas
                            bulkCopy.NotifyAfter = 1; // Notifica a cada 1000 linhas copiadas, por exemplo

                            bulkCopy.WriteToServer(dataTable);
                        }


                        DateTime st = new DateTime(ano, mes, dia);
                        string stt = st.ToString("yyyy-MM-dd");

                        stb.Clear();
                        stb.AppendFormat("INSERT INTO GDA_PERFORMANCE_SEMANAL (");
                        stb.AppendFormat("DATA, ");
                        stb.AppendFormat("SEMANA, ");
                        stb.AppendFormat("MES, ");
                        stb.AppendFormat("ANO, ");
                        stb.AppendFormat("IDENTIFICACAO, ");
                        stb.AppendFormat("CHAVE_EXTERNA, ");
                        stb.AppendFormat("NOME_USUARIO, ");
                        stb.AppendFormat("NOME_NIVEL_HIERARQUIA, ");
                        stb.AppendFormat("IDENTIFICACAO_SUPERVISOR, ");
                        stb.AppendFormat("CHAVE_EXTERNA_SUPERVISOR, ");
                        stb.AppendFormat("NOME_SUPERVISOR,");
                        stb.AppendFormat("IDENTIFICACAO_COORDENADOR, ");
                        stb.AppendFormat("CHAVE_EXTERNA_COORDENADOR, ");
                        stb.AppendFormat("NOME_COORDENADOR,");
                        stb.AppendFormat("IDENTIFICACAO_GERENTE_II, ");
                        stb.AppendFormat("CHAVE_EXTERNA_GERENTE_II, ");
                        stb.AppendFormat("NOME_GERENTE_II,");
                        stb.AppendFormat("IDENTIFICACAO_GERENTE_I, ");
                        stb.AppendFormat("CHAVE_EXTERNA_GERENTE_I, ");
                        stb.AppendFormat("NOME_GERENTE_I,");
                        stb.AppendFormat("IDENTIFICACAO_DIRETOR, ");
                        stb.AppendFormat("CHAVE_EXTERNA_DIRETOR, ");
                        stb.AppendFormat("NOME_DIRETOR,");
                        stb.AppendFormat("IDENTIFICACAO_CEO, ");
                        stb.AppendFormat("CHAVE_EXTERNA_CEO, ");
                        stb.AppendFormat("NOME_CEO,");
                        stb.AppendFormat("CD_GIP, ");
                        stb.AppendFormat("SETOR, ");
                        stb.AppendFormat("ID_INDICADOR, ");
                        stb.AppendFormat("NOME_INDICADOR, ");
                        stb.AppendFormat("RESULTADO,");
                        stb.AppendFormat("FATOR, ");
                        stb.AppendFormat("FATOR_1, ");
                        stb.AppendFormat("FATOR_2, ");
                        stb.AppendFormat("RESULTADO_CALCULADO, ");
                        stb.AppendFormat("PERCENTUAL_ATINGIMENTO, ");
                        stb.AppendFormat("META, ");
                        stb.AppendFormat("GANHO, ");
                        stb.AppendFormat("MAX_GANHO, ");
                        stb.AppendFormat("ID_GRUPO, ");
                        stb.AppendFormat("NOME_GRUPO, ");
                        stb.AppendFormat("NOME_NIVEL, ");
                        stb.AppendFormat("DATA_ATUALIZACAO,");
                        stb.AppendFormat("NOVATO, ");
                        stb.AppendFormat("SITE, ");
                        stb.AppendFormat("HOME_BASED, ");
                        stb.AppendFormat("PERIODO ");
                        stb.AppendFormat(") SELECT ");
                        stb.AppendFormat("'{0}' AS DATA,", stt); //DATA, 
                        stb.AppendFormat("'{0}' AS SEMANA,", semana); //SEMANA, 
                        stb.AppendFormat("'{0}' AS MES,", mes); //MES, 
                        stb.AppendFormat("'{0}' AS ANO,", ano); //ANO, 
                        stb.AppendFormat("CONCAT('BC', MATRICULA),"); //IDENTIFICACAO, 
                        stb.AppendFormat("MATRICULA,"); //CHAVE_EXTERNA, 
                        stb.AppendFormat("NOME,"); //NOME_USUARIO, 
                        stb.AppendFormat("CARGO,"); //NOME_NIVEL_HIERARQUIA, 
                        stb.AppendFormat("CONCAT('BC', MATRICULASUPERVISOR),"); //IDENTIFICACAO_SUPERVISOR, 
                        stb.AppendFormat("MATRICULASUPERVISOR,"); //CHAVE_EXTERNA_SUPERVISOR, 
                        stb.AppendFormat("NOMESUPERVISOR,"); //NOME_SUPERVISOR,
                        stb.AppendFormat("CONCAT('BC', MATRICULACOORDENADOR),"); //IDENTIFICACAO_COORDENADOR, 
                        stb.AppendFormat("MATRICULACOORDENADOR,"); //CHAVE_EXTERNA_COORDENADOR, 
                        stb.AppendFormat("NOMECOORDENADOR,"); //NOME_COORDENADOR,
                        stb.AppendFormat("CONCAT('BC', MATRICULAGERENTE2),"); //IDENTIFICACAO_GERENTE_II, 
                        stb.AppendFormat("MATRICULAGERENTE2,"); //CHAVE_EXTERNA_GERENTE_II, 
                        stb.AppendFormat("NOMEGERENTE2,"); //NOME_GERENTE_II,
                        stb.AppendFormat("CONCAT('BC', MATRICULAGERENTE1),"); //IDENTIFICACAO_GERENTE_I, 
                        stb.AppendFormat("MATRICULAGERENTE1,"); //CHAVE_EXTERNA_GERENTE_I, 
                        stb.AppendFormat("NOMEGERENTE1,"); //NOME_GERENTE_I,
                        stb.AppendFormat("CONCAT('BC', MATRICULADIRETOR),"); //IDENTIFICACAO_DIRETOR, 
                        stb.AppendFormat("MATRICULADIRETOR,"); //CHAVE_EXTERNA_DIRETOR, 
                        stb.AppendFormat("NOMEDIRETOR,"); //NOME_DIRETOR,
                        stb.AppendFormat("CONCAT('BC', MATRICULACEO),"); //IDENTIFICACAO_CEO, 
                        stb.AppendFormat("MATRICULACEO,"); //CHAVE_EXTERNA_CEO, 
                        stb.AppendFormat("NOMECEO,"); //NOME_CEO,
                        stb.AppendFormat("CODGIP,"); //CD_GIP, 
                        stb.AppendFormat("SETOR,"); //SETOR, 
                        stb.AppendFormat("IDINDICADOR,"); //ID_INDICADOR, 
                        stb.AppendFormat("INDICADOR,"); //NOME_INDICADOR, 
                        stb.AppendFormat("RESULTADO,"); //RESULTADO,
                        stb.AppendFormat("CONCAT(FACTOR0,';', FACTOR1),"); //FATOR, 
                        stb.AppendFormat("FACTOR0,"); //FATOR_1, 
                        stb.AppendFormat("FACTOR1,"); //FATOR_2, 
                        stb.AppendFormat("RESULTADO,"); //RESULTADO_CALCULADO, 
                        stb.AppendFormat("PERCENTUAL,"); //PERCENTUAL_ATINGIMENTO, 
                        stb.AppendFormat("META,"); //META, 
                        stb.AppendFormat("MOEDA_GANHA,"); //GANHO, 
                        stb.AppendFormat("META_MAXIMA_MOEDAS,"); //MAX_GANHO, 
                        stb.AppendFormat("IDGRUPO,"); //ID_GRUPO, 
                        stb.AppendFormat("GRUPO,"); //NOME_GRUPO, 
                        stb.AppendFormat("NOME_NIVEL,"); //NOME_NIVEL, 
                        stb.AppendFormat("GETDATE(),"); //DATA_ATUALIZACAO,
                        stb.AppendFormat("NOVATO,"); //NOVATO, 
                        stb.AppendFormat("IDGDA_SITE,"); //SITE, 
                        stb.AppendFormat("IDGDA_HOME,"); //HOME_BASED, 
                        stb.AppendFormat("IDGDA_PERIODO "); //PERIODO 
                        stb.AppendFormat("FROM #TEMPTABLE ");

                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            command.CommandTimeout = 0;
                            command.ExecuteNonQuery();
                        }
                        connection.Close();
                    }
                    if (semana == 0 && agruparResultado == true)
                    {
                        StringBuilder stb = new StringBuilder();
                        stb.Append("DELETE FROM GDA_PERFORMANCE_MENSAL_RESULTADO ");
                        stb.AppendFormat("WHERE MES = {0} ", mes);
                        stb.AppendFormat("AND ANO = {0} ", ano);

                        connection.Open();
                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            command.CommandTimeout = 0;
                            command.ExecuteNonQuery();
                        }

                        DataTable dataTable = ToDataTable(retorno);

                        string colunas = "";
                        foreach (DataColumn colunaAtual in dataTable.Columns)
                        {
                            var tipoColuna = colunaAtual.DataType;
                            string strTipoColunaSQL = "";

                            switch (tipoColuna.Name)
                            {
                                case "Int32":
                                    {
                                        strTipoColunaSQL = "INT";
                                        break;
                                    }

                                case "String":
                                    {
                                        strTipoColunaSQL = "VARCHAR(MAX)";
                                        break;
                                    }

                                case "DateTime":
                                    {
                                        strTipoColunaSQL = "DATETIME";
                                        break;
                                    }

                                case "Double":
                                    {
                                        strTipoColunaSQL = "FLOAT";
                                        break;
                                    }

                                default:
                                    {
                                        strTipoColunaSQL = "VARCHAR(MAX)";
                                        break;
                                    }
                            }

                            colunas = colunas == "" ? $" [{colunaAtual.ColumnName}] {strTipoColunaSQL} " : $"{colunas}, [{colunaAtual.ColumnName}] {strTipoColunaSQL}";
                        }

                        string commandText = $"CREATE TABLE #TEMPTABLE ({colunas});"; // Substitua as colunas com as adequadas do seu DataTable
                        SqlCommand createTableCommand = new SqlCommand(commandText, connection);
                        createTableCommand.ExecuteNonQuery();

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName = "#TEMPTABLE";
                            bulkCopy.BulkCopyTimeout = 0;

                            // Define o número de linhas a serem notificadas
                            bulkCopy.NotifyAfter = 1; // Notifica a cada 1000 linhas copiadas, por exemplo

                            bulkCopy.WriteToServer(dataTable);
                        }


                        DateTime st = new DateTime(ano, mes, dia);
                        string stt = st.ToString("yyyy-MM-dd");

                        stb.Clear();
                        stb.AppendFormat("INSERT INTO GDA_PERFORMANCE_MENSAL_RESULTADO (");
                        stb.AppendFormat("DATA, ");
                        stb.AppendFormat("MES, ");
                        stb.AppendFormat("ANO, ");
                        stb.AppendFormat("IDENTIFICACAO, ");
                        stb.AppendFormat("CHAVE_EXTERNA, ");
                        stb.AppendFormat("NOME_USUARIO, ");
                        stb.AppendFormat("NOME_NIVEL_HIERARQUIA, ");
                        stb.AppendFormat("IDENTIFICACAO_SUPERVISOR, ");
                        stb.AppendFormat("CHAVE_EXTERNA_SUPERVISOR, ");
                        stb.AppendFormat("NOME_SUPERVISOR,");
                        stb.AppendFormat("IDENTIFICACAO_COORDENADOR, ");
                        stb.AppendFormat("CHAVE_EXTERNA_COORDENADOR, ");
                        stb.AppendFormat("NOME_COORDENADOR,");
                        stb.AppendFormat("IDENTIFICACAO_GERENTE_II, ");
                        stb.AppendFormat("CHAVE_EXTERNA_GERENTE_II, ");
                        stb.AppendFormat("NOME_GERENTE_II,");
                        stb.AppendFormat("IDENTIFICACAO_GERENTE_I, ");
                        stb.AppendFormat("CHAVE_EXTERNA_GERENTE_I, ");
                        stb.AppendFormat("NOME_GERENTE_I,");
                        stb.AppendFormat("IDENTIFICACAO_DIRETOR, ");
                        stb.AppendFormat("CHAVE_EXTERNA_DIRETOR, ");
                        stb.AppendFormat("NOME_DIRETOR,");
                        stb.AppendFormat("IDENTIFICACAO_CEO, ");
                        stb.AppendFormat("CHAVE_EXTERNA_CEO, ");
                        stb.AppendFormat("NOME_CEO,");
                        stb.AppendFormat("CD_GIP, ");
                        stb.AppendFormat("SETOR, ");
                        stb.AppendFormat("ID_INDICADOR, ");
                        stb.AppendFormat("NOME_INDICADOR, ");
                        stb.AppendFormat("RESULTADO,");
                        stb.AppendFormat("FATOR, ");
                        stb.AppendFormat("FATOR_1, ");
                        stb.AppendFormat("FATOR_2, ");
                        stb.AppendFormat("RESULTADO_CALCULADO, ");
                        stb.AppendFormat("PERCENTUAL_ATINGIMENTO, ");
                        stb.AppendFormat("META, ");
                        stb.AppendFormat("GANHO, ");
                        stb.AppendFormat("MAX_GANHO, ");
                        stb.AppendFormat("ID_GRUPO, ");
                        stb.AppendFormat("NOME_GRUPO, ");
                        stb.AppendFormat("NOME_NIVEL, ");
                        stb.AppendFormat("DATA_ATUALIZACAO,");
                        stb.AppendFormat("NOVATO, ");
                        stb.AppendFormat("SITE, ");
                        stb.AppendFormat("HOME_BASED, ");
                        stb.AppendFormat("PERIODO ");
                        stb.AppendFormat(") SELECT ");
                        stb.AppendFormat("'{0}' AS DATA,", stt); //DATA,  
                        stb.AppendFormat("'{0}' AS MES,", mes); //MES, 
                        stb.AppendFormat("'{0}' AS ANO,", ano); //ANO, 
                        stb.AppendFormat("CONCAT('BC', MATRICULA),"); //IDENTIFICACAO, 
                        stb.AppendFormat("MATRICULA,"); //CHAVE_EXTERNA, 
                        stb.AppendFormat("NOME,"); //NOME_USUARIO, 
                        stb.AppendFormat("CARGO,"); //NOME_NIVEL_HIERARQUIA, 
                        stb.AppendFormat("CONCAT('BC', MATRICULASUPERVISOR),"); //IDENTIFICACAO_SUPERVISOR, 
                        stb.AppendFormat("MATRICULASUPERVISOR,"); //CHAVE_EXTERNA_SUPERVISOR, 
                        stb.AppendFormat("NOMESUPERVISOR,"); //NOME_SUPERVISOR,
                        stb.AppendFormat("CONCAT('BC', MATRICULACOORDENADOR),"); //IDENTIFICACAO_COORDENADOR, 
                        stb.AppendFormat("MATRICULACOORDENADOR,"); //CHAVE_EXTERNA_COORDENADOR, 
                        stb.AppendFormat("NOMECOORDENADOR,"); //NOME_COORDENADOR,
                        stb.AppendFormat("CONCAT('BC', MATRICULAGERENTE2),"); //IDENTIFICACAO_GERENTE_II, 
                        stb.AppendFormat("MATRICULAGERENTE2,"); //CHAVE_EXTERNA_GERENTE_II, 
                        stb.AppendFormat("NOMEGERENTE2,"); //NOME_GERENTE_II,
                        stb.AppendFormat("CONCAT('BC', MATRICULAGERENTE1),"); //IDENTIFICACAO_GERENTE_I, 
                        stb.AppendFormat("MATRICULAGERENTE1,"); //CHAVE_EXTERNA_GERENTE_I, 
                        stb.AppendFormat("NOMEGERENTE1,"); //NOME_GERENTE_I,
                        stb.AppendFormat("CONCAT('BC', MATRICULADIRETOR),"); //IDENTIFICACAO_DIRETOR, 
                        stb.AppendFormat("MATRICULADIRETOR,"); //CHAVE_EXTERNA_DIRETOR, 
                        stb.AppendFormat("NOMEDIRETOR,"); //NOME_DIRETOR,
                        stb.AppendFormat("CONCAT('BC', MATRICULACEO),"); //IDENTIFICACAO_CEO, 
                        stb.AppendFormat("MATRICULACEO,"); //CHAVE_EXTERNA_CEO, 
                        stb.AppendFormat("NOMECEO,"); //NOME_CEO,
                        stb.AppendFormat("CODGIP,"); //CD_GIP, 
                        stb.AppendFormat("SETOR,"); //SETOR, 
                        stb.AppendFormat("IDINDICADOR,"); //ID_INDICADOR, 
                        stb.AppendFormat("INDICADOR,"); //NOME_INDICADOR, 
                        stb.AppendFormat("RESULTADO,"); //RESULTADO,
                        stb.AppendFormat("CONCAT(FACTOR0,';', FACTOR1),"); //FATOR, 
                        stb.AppendFormat("FACTOR0,"); //FATOR_1, 
                        stb.AppendFormat("FACTOR1,"); //FATOR_2, 
                        stb.AppendFormat("RESULTADO,"); //RESULTADO_CALCULADO, 
                        stb.AppendFormat("PERCENTUAL,"); //PERCENTUAL_ATINGIMENTO, 
                        stb.AppendFormat("META,"); //META, 
                        stb.AppendFormat("MOEDA_GANHA,"); //GANHO, 
                        stb.AppendFormat("META_MAXIMA_MOEDAS,"); //MAX_GANHO, 
                        stb.AppendFormat("IDGRUPO,"); //ID_GRUPO, 
                        stb.AppendFormat("GRUPO,"); //NOME_GRUPO, 
                        stb.AppendFormat("NOME_NIVEL,"); //NOME_NIVEL, 
                        stb.AppendFormat("GETDATE(),"); //DATA_ATUALIZACAO,
                        stb.AppendFormat("NOVATO,"); //NOVATO, 
                        stb.AppendFormat("IDGDA_SITE,"); //SITE, 
                        stb.AppendFormat("IDGDA_HOME,"); //HOME_BASED, 
                        stb.AppendFormat("IDGDA_PERIODO "); //PERIODO 
                        stb.AppendFormat("FROM #TEMPTABLE ");

                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            command.CommandTimeout = 0;
                            command.ExecuteNonQuery();
                        }
                        connection.Close();
                    }
                    if (semana == 0 && agruparResultado == false)
                    {
                        StringBuilder stb = new StringBuilder();
                        stb.Append("DELETE FROM GDA_PERFORMANCE_MENSAL ");
                        stb.AppendFormat("WHERE MES = {0} ", mes);
                        stb.AppendFormat("AND ANO = {0} ", ano);

                        connection.Open();
                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            command.CommandTimeout = 0;
                            command.ExecuteNonQuery();
                        }

                        DataTable dataTable = ToDataTable(retorno);

                        string colunas = "";
                        foreach (DataColumn colunaAtual in dataTable.Columns)
                        {
                            var tipoColuna = colunaAtual.DataType;
                            string strTipoColunaSQL = "";

                            switch (tipoColuna.Name)
                            {
                                case "Int32":
                                    {
                                        strTipoColunaSQL = "INT";
                                        break;
                                    }

                                case "String":
                                    {
                                        strTipoColunaSQL = "VARCHAR(MAX)";
                                        break;
                                    }

                                case "DateTime":
                                    {
                                        strTipoColunaSQL = "DATETIME";
                                        break;
                                    }

                                case "Double":
                                    {
                                        strTipoColunaSQL = "FLOAT";
                                        break;
                                    }

                                default:
                                    {
                                        strTipoColunaSQL = "VARCHAR(MAX)";
                                        break;
                                    }
                            }

                            colunas = colunas == "" ? $" [{colunaAtual.ColumnName}] {strTipoColunaSQL} " : $"{colunas}, [{colunaAtual.ColumnName}] {strTipoColunaSQL}";
                        }

                        string commandText = $"CREATE TABLE #TEMPTABLE ({colunas});"; // Substitua as colunas com as adequadas do seu DataTable
                        SqlCommand createTableCommand = new SqlCommand(commandText, connection);
                        createTableCommand.ExecuteNonQuery();

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName = "#TEMPTABLE";
                            bulkCopy.BulkCopyTimeout = 0;

                            // Define o número de linhas a serem notificadas
                            bulkCopy.NotifyAfter = 1; // Notifica a cada 1000 linhas copiadas, por exemplo

                            bulkCopy.WriteToServer(dataTable);
                        }


                        DateTime st = new DateTime(ano, mes, dia);
                        string stt = st.ToString("yyyy-MM-dd");
                        stb.Clear();
                        stb.AppendFormat("INSERT INTO GDA_PERFORMANCE_MENSAL (");
                        stb.AppendFormat("DATA, ");
                        stb.AppendFormat("MES, ");
                        stb.AppendFormat("ANO, ");
                        stb.AppendFormat("IDENTIFICACAO, ");
                        stb.AppendFormat("CHAVE_EXTERNA, ");
                        stb.AppendFormat("NOME_USUARIO, ");
                        stb.AppendFormat("NOME_NIVEL_HIERARQUIA, ");
                        stb.AppendFormat("IDENTIFICACAO_SUPERVISOR, ");
                        stb.AppendFormat("CHAVE_EXTERNA_SUPERVISOR, ");
                        stb.AppendFormat("NOME_SUPERVISOR,");
                        stb.AppendFormat("IDENTIFICACAO_COORDENADOR, ");
                        stb.AppendFormat("CHAVE_EXTERNA_COORDENADOR, ");
                        stb.AppendFormat("NOME_COORDENADOR,");
                        stb.AppendFormat("IDENTIFICACAO_GERENTE_II, ");
                        stb.AppendFormat("CHAVE_EXTERNA_GERENTE_II, ");
                        stb.AppendFormat("NOME_GERENTE_II,");
                        stb.AppendFormat("IDENTIFICACAO_GERENTE_I, ");
                        stb.AppendFormat("CHAVE_EXTERNA_GERENTE_I, ");
                        stb.AppendFormat("NOME_GERENTE_I,");
                        stb.AppendFormat("IDENTIFICACAO_DIRETOR, ");
                        stb.AppendFormat("CHAVE_EXTERNA_DIRETOR, ");
                        stb.AppendFormat("NOME_DIRETOR,");
                        stb.AppendFormat("IDENTIFICACAO_CEO, ");
                        stb.AppendFormat("CHAVE_EXTERNA_CEO, ");
                        stb.AppendFormat("NOME_CEO,");
                        stb.AppendFormat("CD_GIP, ");
                        stb.AppendFormat("SETOR, ");
                        stb.AppendFormat("ID_INDICADOR, ");
                        stb.AppendFormat("NOME_INDICADOR, ");
                        stb.AppendFormat("RESULTADO,");
                        stb.AppendFormat("FATOR, ");
                        stb.AppendFormat("FATOR_1, ");
                        stb.AppendFormat("FATOR_2, ");
                        stb.AppendFormat("RESULTADO_CALCULADO, ");
                        stb.AppendFormat("PERCENTUAL_ATINGIMENTO, ");
                        stb.AppendFormat("META, ");
                        stb.AppendFormat("GANHO, ");
                        stb.AppendFormat("MAX_GANHO, ");
                        stb.AppendFormat("ID_GRUPO, ");
                        stb.AppendFormat("NOME_GRUPO, ");
                        stb.AppendFormat("NOME_NIVEL, ");
                        stb.AppendFormat("DATA_ATUALIZACAO,");
                        stb.AppendFormat("NOVATO, ");
                        stb.AppendFormat("SITE, ");
                        stb.AppendFormat("HOME_BASED, ");
                        stb.AppendFormat("PERIODO ");
                        stb.AppendFormat(") SELECT ");
                        stb.AppendFormat("'{0}' AS DATA,", stt); //DATA, 
                        stb.AppendFormat("'{0}' AS MES,", mes); //MES, 
                        stb.AppendFormat("'{0}' AS ANO,", ano); //ANO, 
                        stb.AppendFormat("CONCAT('BC', MATRICULA),"); //IDENTIFICACAO, 
                        stb.AppendFormat("MATRICULA,"); //CHAVE_EXTERNA, 
                        stb.AppendFormat("NOME,"); //NOME_USUARIO, 
                        stb.AppendFormat("CARGO,"); //NOME_NIVEL_HIERARQUIA, 
                        stb.AppendFormat("CONCAT('BC', MATRICULASUPERVISOR),"); //IDENTIFICACAO_SUPERVISOR, 
                        stb.AppendFormat("MATRICULASUPERVISOR,"); //CHAVE_EXTERNA_SUPERVISOR, 
                        stb.AppendFormat("NOMESUPERVISOR,"); //NOME_SUPERVISOR,
                        stb.AppendFormat("CONCAT('BC', MATRICULACOORDENADOR),"); //IDENTIFICACAO_COORDENADOR, 
                        stb.AppendFormat("MATRICULACOORDENADOR,"); //CHAVE_EXTERNA_COORDENADOR, 
                        stb.AppendFormat("NOMECOORDENADOR,"); //NOME_COORDENADOR,
                        stb.AppendFormat("CONCAT('BC', MATRICULAGERENTE2),"); //IDENTIFICACAO_GERENTE_II, 
                        stb.AppendFormat("MATRICULAGERENTE2,"); //CHAVE_EXTERNA_GERENTE_II, 
                        stb.AppendFormat("NOMEGERENTE2,"); //NOME_GERENTE_II,
                        stb.AppendFormat("CONCAT('BC', MATRICULAGERENTE1),"); //IDENTIFICACAO_GERENTE_I, 
                        stb.AppendFormat("MATRICULAGERENTE1,"); //CHAVE_EXTERNA_GERENTE_I, 
                        stb.AppendFormat("NOMEGERENTE1,"); //NOME_GERENTE_I,
                        stb.AppendFormat("CONCAT('BC', MATRICULADIRETOR),"); //IDENTIFICACAO_DIRETOR, 
                        stb.AppendFormat("MATRICULADIRETOR,"); //CHAVE_EXTERNA_DIRETOR, 
                        stb.AppendFormat("NOMEDIRETOR,"); //NOME_DIRETOR,
                        stb.AppendFormat("CONCAT('BC', MATRICULACEO),"); //IDENTIFICACAO_CEO, 
                        stb.AppendFormat("MATRICULACEO,"); //CHAVE_EXTERNA_CEO, 
                        stb.AppendFormat("NOMECEO,"); //NOME_CEO,
                        stb.AppendFormat("CODGIP,"); //CD_GIP, 
                        stb.AppendFormat("SETOR,"); //SETOR, 
                        stb.AppendFormat("IDINDICADOR,"); //ID_INDICADOR, 
                        stb.AppendFormat("INDICADOR,"); //NOME_INDICADOR, 
                        stb.AppendFormat("RESULTADO,"); //RESULTADO,
                        stb.AppendFormat("CONCAT(FACTOR0,';', FACTOR1),"); //FATOR, 
                        stb.AppendFormat("FACTOR0,"); //FATOR_1, 
                        stb.AppendFormat("FACTOR1,"); //FATOR_2, 
                        stb.AppendFormat("RESULTADO,"); //RESULTADO_CALCULADO, 
                        stb.AppendFormat("PERCENTUAL,"); //PERCENTUAL_ATINGIMENTO, 
                        stb.AppendFormat("META,"); //META, 
                        stb.AppendFormat("MOEDA_GANHA,"); //GANHO, 
                        stb.AppendFormat("META_MAXIMA_MOEDAS,"); //MAX_GANHO, 
                        stb.AppendFormat("IDGRUPO,"); //ID_GRUPO, 
                        stb.AppendFormat("GRUPO,"); //NOME_GRUPO, 
                        stb.AppendFormat("NOME_NIVEL,"); //NOME_NIVEL, 
                        stb.AppendFormat("GETDATE(),"); //DATA_ATUALIZACAO,
                        stb.AppendFormat("NOVATO,"); //NOVATO, 
                        stb.AppendFormat("IDGDA_SITE,"); //SITE, 
                        stb.AppendFormat("IDGDA_HOME,"); //HOME_BASED, 
                        stb.AppendFormat("IDGDA_PERIODO "); //PERIODO 
                        stb.AppendFormat("FROM #TEMPTABLE ");

                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            command.CommandTimeout = 0;
                            command.ExecuteNonQuery();
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static List<semanalMensalConsolidado> ReturnHomeResultConsolidated(string dtinicial, string dtfinal)
        {
            string filter = "";
            semanalMensalConsolidado rmams = new semanalMensalConsolidado();


            //Periodos

            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtinicial);
            stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtfinal);
            //stb.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}'; ", idCol);

            stb.AppendFormat("SELECT MAX(TBL.NAME) AS NAME, MAX(TBL.CARGO) AS CARGO, TBL.IDGDA_COLLABORATORS, TBl.CREATED_AT, SUM(QTD) AS QTD, ");
            stb.AppendFormat("SUM(CASE ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 1 ");
            stb.AppendFormat("                    AND HIG1.MONETIZATION > 0 THEN QTD ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 2 ");
            stb.AppendFormat("                    AND HIG1.MONETIZATION_NIGHT > 0 THEN QTD ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 3 ");
            stb.AppendFormat("                    AND HIG1.MONETIZATION_LATENIGHT > 0 THEN QTD ");
            stb.AppendFormat("               ELSE 0 ");
            stb.AppendFormat("           END) AS QTD_MON, ");
            stb.AppendFormat("       SUM(CASE ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 1 ");
            stb.AppendFormat("                    AND HIG1.MONETIZATION > 0 THEN (HIG1.MONETIZATION * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 2 ");
            stb.AppendFormat("                    AND HIG1.MONETIZATION_NIGHT > 0 THEN (HIG1.MONETIZATION_NIGHT * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 3 ");
            stb.AppendFormat("                    AND HIG1.MONETIZATION_LATENIGHT > 0 THEN (HIG1.MONETIZATION_LATENIGHT * QTD) ");
            stb.AppendFormat("               ELSE 0 ");
            stb.AppendFormat("           END) AS QTD_MON_TOTAL, ");

            stb.AppendFormat(" 		   SUM(CASE ");
            stb.AppendFormat("              WHEN TBL.IDGDA_PERIOD = 1 ");
            stb.AppendFormat("                  AND HIS.GOAL IS NOT NULL THEN (QTD) ");
            stb.AppendFormat("              WHEN TBL.IDGDA_PERIOD = 2 ");
            stb.AppendFormat("                  AND HIS.GOAL_NIGHT > 0 THEN (QTD) ");
            stb.AppendFormat("              WHEN TBL.IDGDA_PERIOD = 3 ");
            stb.AppendFormat("                  AND HIS.GOAL_LATENIGHT > 0 THEN (QTD) ");
            stb.AppendFormat("              ELSE 0 ");
            stb.AppendFormat("          END) AS QTD_META, ");

            stb.AppendFormat("	   MAX(TBL.IDGDA_SECTOR) AS IDGDA_SECTOR, ");
            stb.AppendFormat("	   MAX(TBL.IDGDA_SUBSECTOR) AS IDGDA_SUBSECTOR, ");
            stb.AppendFormat("	   MAX(SEC.NAME) AS SETOR, ");
            stb.AppendFormat("	   MAX(SUBSEC.NAME) AS SUBSETOR, ");
            stb.AppendFormat("       MAX(IT.IDGDA_INDICATOR) AS IDGDA_INDICATOR, ");
            stb.AppendFormat("	   MAX(IT.NAME) AS 'INDICADOR', ");
            stb.AppendFormat("       SUM(CASE ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 1 THEN (HIS.GOAL * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 2 THEN (HIS.GOAL_NIGHT * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 3 THEN (HIS.GOAL_LATENIGHT * QTD) ");
            stb.AppendFormat("               ELSE 0 ");
            stb.AppendFormat("           END) AS META, ");
            stb.AppendFormat("       SUM(CASE ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 1 THEN (HIG1.METRIC_MIN * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 2 THEN (HIG1.METRIC_MIN_NIGHT * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 3 THEN (HIG1.METRIC_MIN_LATENIGHT * QTD) ");
            stb.AppendFormat("               ELSE 0 ");
            stb.AppendFormat("           END) AS MIN1, ");
            stb.AppendFormat("       SUM(CASE ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 1 THEN (HIG2.METRIC_MIN * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 2 THEN (HIG2.METRIC_MIN_NIGHT * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 3 THEN (HIG2.METRIC_MIN_LATENIGHT * QTD) ");
            stb.AppendFormat("               ELSE 0 ");
            stb.AppendFormat("           END) AS MIN2, ");
            stb.AppendFormat("       SUM(CASE ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 1 THEN (HIG3.METRIC_MIN * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 2 THEN (HIG3.METRIC_MIN_NIGHT * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 3 THEN (HIG3.METRIC_MIN_LATENIGHT * QTD) ");
            stb.AppendFormat("               ELSE 0 ");
            stb.AppendFormat("           END) AS MIN3, ");
            stb.AppendFormat("       SUM(CASE ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 1 THEN (HIG4.METRIC_MIN * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 2 THEN (HIG4.METRIC_MIN_NIGHT * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 3 THEN (HIG4.METRIC_MIN_LATENIGHT * QTD) ");
            stb.AppendFormat("               ELSE 0 ");
            stb.AppendFormat("           END) AS MIN4, ");
            stb.AppendFormat("		    CASE ");
            stb.AppendFormat("           WHEN MAX(ME.EXPRESSION) IS NULL THEN '(#FATOR1/#FATOR0)' ");
            stb.AppendFormat("           ELSE MAX(ME.EXPRESSION) ");
            stb.AppendFormat("       END AS CONTA, ");
            stb.AppendFormat("       CASE ");
            stb.AppendFormat("           WHEN MAX(IT.CALCULATION_TYPE) IS NULL THEN 'BIGGER_BETTER' ");
            stb.AppendFormat("           ELSE MAX(IT.CALCULATION_TYPE) ");
            stb.AppendFormat("       END AS BETTER, ");
            stb.AppendFormat("       MAX(IT.TYPE) AS TYPE, ");
            stb.AppendFormat("	   SUM(TBL.FACTOR0) AS FACTOR0, ");
            stb.AppendFormat("	   SUM(TBL.FACTOR1) AS FACTOR1, ");

            stb.AppendFormat("       MAX(TBL.IDGDA_HOMEBASED) AS HOME_BASED, ");
            stb.AppendFormat("       MAX(TBL.IDGDA_SITE) AS SITE, ");
            stb.AppendFormat("       MAX(TBL.IDGDA_PERIOD) AS TURNO, ");
            stb.AppendFormat("       MAX(TBL.MATRICULA_SUPERVISOR) AS 'MATRICULA SUPERVISOR', ");
            stb.AppendFormat("       MAX(TBL.NOME_SUPERVISOR) AS 'NOME SUPERVISOR', ");
            stb.AppendFormat("       MAX(TBL.MATRICULA_COORDENADOR) AS 'MATRICULA COORDENADOR', ");
            stb.AppendFormat("       MAX(TBL.NOME_COORDENADOR) AS 'NOME COORDENADOR', ");
            stb.AppendFormat("       MAX(TBL.MATRICULA_GERENTE_II) AS 'MATRICULA GERENTE II', ");
            stb.AppendFormat("       MAX(TBL.NOME_GERENTE_II) AS 'NOME GERENTE II', ");
            stb.AppendFormat("       MAX(TBL.MATRICULA_GERENTE_I) AS 'MATRICULA GERENTE I', ");
            stb.AppendFormat("       MAX(TBL.NOME_GERENTE_I) AS 'NOME GERENTE I', ");
            stb.AppendFormat("       MAX(TBL.MATRICULA_DIRETOR) AS 'MATRICULA DIRETOR', ");
            stb.AppendFormat("       MAX(TBL.NOME_DIRETOR) AS 'NOME DIRETOR', ");
            stb.AppendFormat("       MAX(TBL.MATRICULA_CEO) AS 'MATRICULA CEO', ");
            stb.AppendFormat("       MAX(TBL.NOME_CEO) AS 'NOME CEO', ");

            //stb.AppendFormat("	   SUM(TBL.MOEDA_GANHA) AS MOEDA_GANHA ");

            stb.AppendFormat(" SUM(CASE ");
            stb.AppendFormat("         WHEN TBL.IDGDA_PERIOD = 1 ");
            stb.AppendFormat("              AND HIG1.MONETIZATION > 0 THEN(TBL.MOEDA_GANHA) ");
            stb.AppendFormat("         WHEN TBL.IDGDA_PERIOD = 2 ");
            stb.AppendFormat("              AND HIG1.MONETIZATION_NIGHT > 0 THEN(TBL.MOEDA_GANHA) ");
            stb.AppendFormat("         WHEN TBL.IDGDA_PERIOD = 3 ");
            stb.AppendFormat("              AND HIG1.MONETIZATION_LATENIGHT > 0 THEN(TBL.MOEDA_GANHA) ");
            stb.AppendFormat("         ELSE 0 ");
            stb.AppendFormat("     END) AS MOEDA_GANHA ");

            stb.AppendFormat("FROM  ");
            stb.AppendFormat("( ");
            stb.AppendFormat("SELECT  ");
            stb.AppendFormat("    MAX(CB.NAME) AS NAME, CL.IDGDA_COLLABORATORS, ");
            stb.AppendFormat("     CONVERT(DATE, R.CREATED_AT) AS CREATED_AT, ");
            stb.AppendFormat("     COUNT(0) AS QTD, ");
            stb.AppendFormat("     MAX(CL.IDGDA_HOMEBASED) AS IDGDA_HOMEBASED, ");
            stb.AppendFormat("     MAX(CL.IDGDA_SITE) AS IDGDA_SITE, ");

            stb.AppendFormat("     MAX(CL.MATRICULA_SUPERVISOR) AS MATRICULA_SUPERVISOR, ");
            stb.AppendFormat("     MAX(CL.NOME_SUPERVISOR) AS NOME_SUPERVISOR, ");
            stb.AppendFormat("     MAX(CL.MATRICULA_COORDENADOR) AS MATRICULA_COORDENADOR, ");
            stb.AppendFormat("     MAX(CL.NOME_COORDENADOR) AS NOME_COORDENADOR, ");
            stb.AppendFormat("     MAX(CL.MATRICULA_GERENTE_II) AS MATRICULA_GERENTE_II, ");
            stb.AppendFormat("     MAX(CL.NOME_GERENTE_II) AS NOME_GERENTE_II, ");
            stb.AppendFormat("     MAX(CL.MATRICULA_GERENTE_I) AS MATRICULA_GERENTE_I, ");
            stb.AppendFormat("     MAX(CL.NOME_GERENTE_I) AS NOME_GERENTE_I, ");
            stb.AppendFormat("     MAX(CL.MATRICULA_DIRETOR) AS MATRICULA_DIRETOR, ");
            stb.AppendFormat("     MAX(CL.NOME_DIRETOR) AS NOME_DIRETOR, ");
            stb.AppendFormat("     MAX(CL.MATRICULA_CEO) AS MATRICULA_CEO, ");
            stb.AppendFormat("     MAX(CL.NOME_CEO) AS NOME_CEO, ");

            stb.AppendFormat("	   CL.IDGDA_PERIOD AS IDGDA_PERIOD, ");
            stb.AppendFormat("     CL.IDGDA_SECTOR AS IDGDA_SECTOR, ");
            stb.AppendFormat("     CL.IDGDA_SUBSECTOR AS IDGDA_SUBSECTOR, ");
            stb.AppendFormat("     CL.IDGDA_SECTOR_SUBSECTOR AS IDGDA_SECTOR_SUBSECTOR, ");
            stb.AppendFormat("     R.INDICADORID AS INDICADORID, ");

            stb.AppendFormat("     MAX(CL.CARGO) AS CARGO, ");
            stb.AppendFormat("	   SUM(FACTORSAG0) AS FACTOR0, ");
            stb.AppendFormat("	   SUM(FACTORSAG1) AS FACTOR1, ");
            stb.AppendFormat("	   SUM(MZ.INPUT) AS MOEDA_GANHA ");
            stb.AppendFormat("FROM GDA_RESULT (NOLOCK) AS R ");
            stb.AppendFormat("LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS CB ON CB.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");

            stb.AppendFormat("LEFT JOIN ");
            stb.AppendFormat("  (SELECT ");
            stb.AppendFormat("   CD.IDGDA_COLLABORATORS, ");
            stb.AppendFormat("   CD.IDGDA_SECTOR, ");
            stb.AppendFormat("   CD.IDGDA_SUBSECTOR, ");

            stb.AppendFormat("   CASE WHEN CD.IDGDA_SUBSECTOR IS NOT NULL THEN CD.IDGDA_SUBSECTOR ");
            stb.AppendFormat("   ELSE CD.IDGDA_SECTOR END AS IDGDA_SECTOR_SUBSECTOR, ");

            stb.AppendFormat("   CD.CREATED_AT, ");
            stb.AppendFormat("	 CD.CARGO, ");
            stb.AppendFormat("	 CD.IDGDA_PERIOD, ");
            stb.AppendFormat("   HB.IDGDA_HOMEBASED, ");
            stb.AppendFormat("   S.IDGDA_SITE, ");
            stb.AppendFormat("   CD.MATRICULA_SUPERVISOR, ");
            stb.AppendFormat("   CD.NOME_SUPERVISOR, ");
            stb.AppendFormat("   CD.MATRICULA_COORDENADOR, ");
            stb.AppendFormat("   CD.NOME_COORDENADOR, ");
            stb.AppendFormat("   CD.MATRICULA_GERENTE_II, ");
            stb.AppendFormat("   CD.NOME_GERENTE_II, ");
            stb.AppendFormat("   CD.MATRICULA_GERENTE_I, ");
            stb.AppendFormat("   CD.NOME_GERENTE_I, ");
            stb.AppendFormat("   CD.MATRICULA_DIRETOR, ");
            stb.AppendFormat("   CD.NOME_DIRETOR, ");
            stb.AppendFormat("   CD.MATRICULA_CEO, ");
            stb.AppendFormat("   CD.NOME_CEO ");
            stb.AppendFormat("   FROM GDA_COLLABORATORS_DETAILS (NOLOCK) CD ");
            stb.AppendFormat("   LEFT JOIN GDA_SITE (NOLOCK) S ON CD.SITE = S.SITE ");
            stb.AppendFormat("   LEFT JOIN GDA_HOMEBASED (NOLOCK) HB ON HB.HOMEBASED = CD.HOME_BASED ");
            stb.AppendFormat("   WHERE CD.CREATED_AT >= @DATAINICIAL ");
            stb.AppendFormat("     AND CD.CREATED_AT <= @DATAFINAL ");
            //stb.AppendFormat("	AND active = 'true' ");
            stb.AppendFormat($"	AND CD.HOME_BASED <> '' ");
            stb.AppendFormat($"	AND CD.CARGO IS NOT NULL ) AS CL ON CL.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS ");
            stb.AppendFormat("AND CL.CREATED_AT = R.CREATED_AT ");

            stb.AppendFormat(" ");
            stb.AppendFormat("LEFT JOIN ");
            stb.AppendFormat("  (SELECT SUM(INPUT) - SUM(OUTPUT) AS INPUT, ");
            stb.AppendFormat("          gda_indicator_idgda_indicator, ");
            stb.AppendFormat("          result_date, ");
            stb.AppendFormat("          COLLABORATOR_ID ");
            stb.AppendFormat("   FROM GDA_CHECKING_ACCOUNT ");
            stb.AppendFormat("   WHERE RESULT_DATE >= @DATAINICIAL ");
            stb.AppendFormat("     AND RESULT_DATE <= @DATAFINAL ");

            stb.AppendFormat("   GROUP BY gda_indicator_idgda_indicator, ");
            stb.AppendFormat("            result_date, ");
            stb.AppendFormat("            COLLABORATOR_ID) AS MZ ON MZ.gda_indicator_idgda_indicator = R.INDICADORID ");
            stb.AppendFormat("AND MZ.result_date = R.CREATED_AT ");
            stb.AppendFormat("AND MZ.COLLABORATOR_ID = R.IDGDA_COLLABORATORS ");
            stb.AppendFormat(" ");
            stb.AppendFormat("WHERE 1 = 1 ");
            stb.AppendFormat("  AND R.CREATED_AT >= @DATAINICIAL ");
            stb.AppendFormat("  AND R.CREATED_AT <= @DATAFINAL ");
            stb.AppendFormat("  AND R.DELETED_AT IS NULL ");
            stb.AppendFormat($"  AND (CL.IDGDA_SECTOR IS NOT NULL OR CL.IDGDA_SUBSECTOR IS NOT NULL) ");

            stb.AppendFormat("  AND R.FACTORS <> '0.000000;0.000000' ");
            stb.AppendFormat("  AND R.FACTORS <> '0.000000; 0.000000' ");
            stb.AppendFormat("  {0}  ", filter);
            stb.AppendFormat($"GROUP BY R.INDICADORID, CL.IDGDA_SECTOR, CL.IDGDA_SUBSECTOR, CL.IDGDA_SECTOR_SUBSECTOR, ");
            stb.AppendFormat("         CONVERT(DATE, R.CREATED_AT), IDGDA_PERIOD, CL.IDGDA_COLLABORATORS ");
            stb.AppendFormat(") AS TBL ");
            stb.AppendFormat("INNER JOIN GDA_INDICATOR (NOLOCK) AS IT ON IT.IDGDA_INDICATOR = TBL.INDICADORID ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
            stb.AppendFormat("AND HIG1.INDICATOR_ID = TBL.INDICADORID ");
            stb.AppendFormat($"AND HIG1.SECTOR_ID = TBL.IDGDA_SECTOR_SUBSECTOR ");
            stb.AppendFormat("AND HIG1.GROUPID = 1 ");
            stb.AppendFormat("AND CONVERT(DATE,HIG1.STARTED_AT) <= TBL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIG1.ENDED_AT) >= TBL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG2 ON HIG2.DELETED_AT IS NULL ");
            stb.AppendFormat("AND HIG2.INDICATOR_ID = TBL.INDICADORID ");
            stb.AppendFormat($"AND HIG2.SECTOR_ID = TBL.IDGDA_SECTOR_SUBSECTOR ");
            stb.AppendFormat("AND HIG2.GROUPID = 2 ");
            stb.AppendFormat("AND CONVERT(DATE,HIG2.STARTED_AT) <= TBL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIG2.ENDED_AT) >= TBL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG3 ON HIG3.DELETED_AT IS NULL ");
            stb.AppendFormat("AND HIG3.INDICATOR_ID = TBL.INDICADORID ");
            stb.AppendFormat($"AND HIG3.SECTOR_ID = TBL.IDGDA_SECTOR_SUBSECTOR ");
            stb.AppendFormat("AND HIG3.GROUPID = 3 ");
            stb.AppendFormat("AND CONVERT(DATE,HIG3.STARTED_AT) <= TBL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIG3.ENDED_AT) >= TBL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG4 ON HIG4.DELETED_AT IS NULL ");
            stb.AppendFormat("AND HIG4.INDICATOR_ID = TBL.INDICADORID ");
            stb.AppendFormat($"AND HIG4.SECTOR_ID = TBL.IDGDA_SECTOR_SUBSECTOR ");
            stb.AppendFormat("AND HIG4.GROUPID = 4 ");
            stb.AppendFormat("AND CONVERT(DATE,HIG4.STARTED_AT) <= TBL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIG4.ENDED_AT) >= TBL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN ");
            stb.AppendFormat("  (SELECT (SUM(GOAL) / COUNT(0)) AS GOAL, ");
            stb.AppendFormat("          (SUM(GOAL_NIGHT) / COUNT(0)) AS GOAL_NIGHT, ");
            stb.AppendFormat("          (SUM(GOAL_LATENIGHT) / COUNT(0)) AS GOAL_LATENIGHT, ");
            stb.AppendFormat("          INDICATOR_ID, SECTOR_ID, ");
            stb.AppendFormat("          CONVERT(DATE,STARTED_AT) AS STARTED_AT, ");
            stb.AppendFormat("          CONVERT(DATE,ENDED_AT) AS ENDED_AT ");
            stb.AppendFormat("   FROM GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) ");

            stb.AppendFormat("   WHERE ((CONVERT(DATE,STARTED_AT) <= @DATAINICIAL AND CONVERT(DATE,ENDED_AT) >= @DATAINICIAL) ");
            stb.AppendFormat("     OR  (CONVERT(DATE,STARTED_AT) <= @DATAFINAL AND CONVERT(DATE,ENDED_AT) >= @DATAFINAL)) ");
            stb.AppendFormat($"     AND DELETED_AT IS NULL ");

            stb.AppendFormat("   GROUP BY SECTOR_ID, INDICATOR_ID, CONVERT(DATE,STARTED_AT), CONVERT(DATE,ENDED_AT)) AS HIS ON HIS.SECTOR_ID = TBL.IDGDA_SECTOR_SUBSECTOR AND HIS.INDICATOR_ID = TBL.INDICADORID ");
            stb.AppendFormat("AND CONVERT(DATE,HIS.STARTED_AT) <= TBL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIS.ENDED_AT) >= TBL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME ON HME.INDICATORID = TBL.INDICADORID ");
            stb.AppendFormat("AND HME.deleted_at IS NULL ");
            stb.AppendFormat("LEFT JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON ME.ID = HME.MATHEMATICALEXPRESSIONID ");
            stb.AppendFormat("AND ME.DELETED_AT IS NULL ");
            stb.AppendFormat($"LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = TBL.IDGDA_SECTOR ");
            stb.AppendFormat($"LEFT JOIN GDA_SECTOR (NOLOCK) AS SUBSEC ON SEC.IDGDA_SECTOR = TBL.IDGDA_SUBSECTOR WHERE TBL.IDGDA_COLLABORATORS = '794907'"); 
            stb.AppendFormat("GROUP BY TBL.IDGDA_COLLABORATORS, TBL.INDICADORID, TBl.CREATED_AT ");

            List<semanalMensalConsolidado> Listhrc = new List<semanalMensalConsolidado>();

            using (SqlConnection connection = new SqlConnection(Database.retornaConn(true)))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                {
                    command.CommandTimeout = 0;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                semanalMensalConsolidado hrc = new semanalMensalConsolidado();
                                hrc.MATRICULA = reader["IDGDA_COLLABORATORS"].ToString();
                                hrc.NOME = reader["NAME"].ToString();
                                hrc.CARGO = reader["CARGO"].ToString();
                                hrc.CODGIP = reader["IDGDA_SECTOR"].ToString();
                                hrc.SETOR = reader["SETOR"].ToString();
                                hrc.CODGIPSUBSETOR = reader["IDGDA_SUBSECTOR"].ToString();
                                hrc.SUBSETOR = reader["SUBSETOR"].ToString();
                                hrc.IDINDICADOR = reader["IDGDA_INDICATOR"].ToString();
                                hrc.INDICADOR = reader["INDICADOR"].ToString();

                                //Regra Nova Descomentar
                                if (reader["META"] != DBNull.Value && Double.Parse(reader["QTD_META"].ToString()) > 0)
                                {

                                    hrc.META = Double.Parse(reader["META"].ToString());
                                }
                                else
                                {
                                    hrc.META = 0;
                                }

                                hrc.min1 = reader["min1"] != DBNull.Value && Double.Parse(reader["QTD_META"].ToString()) > 0 ? double.Parse(reader["min1"].ToString()) : 0;
                                hrc.min2 = reader["min2"] != DBNull.Value && Double.Parse(reader["QTD_META"].ToString()) > 0 ? double.Parse(reader["min2"].ToString()) : 0;
                                hrc.min3 = reader["min3"] != DBNull.Value && Double.Parse(reader["QTD_META"].ToString()) > 0 ? double.Parse(reader["min3"].ToString()) : 0;
                                hrc.min4 = reader["min4"] != DBNull.Value && Double.Parse(reader["QTD_META"].ToString()) > 0 ? double.Parse(reader["min4"].ToString()) : 0;
                                hrc.CONTA = reader["CONTA"].ToString();
                                hrc.BETTER = reader["BETTER"].ToString();
                                hrc.FACTOR0 = reader["FACTOR0"].ToString() != "" ? double.Parse(reader["FACTOR0"].ToString()) : 0;
                                hrc.FACTOR1 = reader["FACTOR1"].ToString() != "" ? double.Parse(reader["FACTOR1"].ToString()) : 0;

                                hrc.MOEDA_GANHA = reader["MOEDA_GANHA"].ToString() != "" ? double.Parse(reader["MOEDA_GANHA"].ToString()) : 0;
                                hrc.QTD_MON = reader["QTD_MON"].ToString() != "" ? double.Parse(reader["QTD_MON"].ToString()) : 0;
                                hrc.QTD_MON_TOTAL = reader["QTD_MON_TOTAL"].ToString() != "" ? double.Parse(reader["QTD_MON_TOTAL"].ToString()) : 0;
                                hrc.QTD_META = reader["QTD_META"].ToString() != "" ? double.Parse(reader["QTD_META"].ToString()) : 0;
                                hrc.QTD = Double.Parse(reader["QTD"].ToString());
                                hrc.DATAPAGAMENTO = reader["CREATED_AT"].ToString();
                                hrc.TYPE = reader["TYPE"].ToString();

                                hrc.MATRICULASUPERVISOR = reader["MATRICULA SUPERVISOR"].ToString() != "" ? reader["MATRICULA SUPERVISOR"].ToString() : "-";
                                hrc.NOMESUPERVISOR = reader["NOME SUPERVISOR"].ToString() != "" ? reader["NOME SUPERVISOR"].ToString() : "-";
                                hrc.MATRICULACOORDENADOR = reader["MATRICULA COORDENADOR"].ToString() != "" ? reader["MATRICULA COORDENADOR"].ToString() : "-";
                                hrc.NOMECOORDENADOR = reader["NOME COORDENADOR"].ToString() != "" ? reader["NOME COORDENADOR"].ToString() : "-";
                                hrc.MATRICULAGERENTE2 = reader["MATRICULA GERENTE II"].ToString() != "" ? reader["MATRICULA GERENTE II"].ToString() : "-";
                                hrc.NOMEGERENTE2 = reader["NOME GERENTE II"].ToString() != "" ? reader["NOME GERENTE II"].ToString() : "-";
                                hrc.MATRICULAGERENTE1 = reader["MATRICULA GERENTE I"].ToString() != "" ? reader["MATRICULA GERENTE I"].ToString() : "-";
                                hrc.NOMEGERENTE1 = reader["NOME GERENTE I"].ToString() != "" ? reader["NOME GERENTE I"].ToString() : "-";
                                hrc.MATRICULADIRETOR = reader["MATRICULA DIRETOR"].ToString() != "" ? reader["MATRICULA DIRETOR"].ToString() : "-";
                                hrc.NOMEDIRETOR = reader["NOME DIRETOR"].ToString() != "" ? reader["NOME DIRETOR"].ToString() : "-";
                                hrc.MATRICULACEO = reader["MATRICULA CEO"].ToString() != "" ? reader["MATRICULA CEO"].ToString() : "-";
                                hrc.NOMECEO = reader["NOME CEO"].ToString() != "" ? reader["NOME CEO"].ToString() : "-";

                                hrc.IDGDA_HOME = reader["HOME_BASED"].ToString() != "" ? int.Parse(reader["HOME_BASED"].ToString()) : 0;
                                hrc.IDGDA_PERIODO = reader["TURNO"].ToString() != "" ? int.Parse(reader["TURNO"].ToString()) : 0;
                                hrc.NOVATO = "";
                                hrc.IDGDA_SITE = reader["SITE"].ToString() != "" ? int.Parse(reader["SITE"].ToString()) : 0;

                                hrc.MOEDA_GANHA = hrc.MOEDA_GANHA > 0 && hrc.QTD_MON > 0 ? hrc.MOEDA_GANHA / hrc.QTD_MON : 0;

                                Listhrc.Add(hrc);
                            }
                            catch (Exception ex)
                            {

                            }

                        }
                    }
                }

                connection.Close();
            }
            return Listhrc;
        }

        public static List<semanalMensalConsolidado> ReturnHomeResultConsolidatedAccess(string dtinicial, string dtfinal, string indicatorsAsString)
        {

            semanalMensalConsolidado rmams = new semanalMensalConsolidado();

            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtinicial);
            stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtfinal);

            stb.AppendFormat("SELECT MAX(TBL.NAME) AS NAME, TBL.IDGDA_COLLABORATORS, TBl.CREATED_AT, ");
            stb.AppendFormat("       SUM(QTD) AS QTD, ");
            stb.AppendFormat("       MAX(TBL.CARGO) AS CARGO, ");
            stb.AppendFormat("       SUM(CASE ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 1 ");
            stb.AppendFormat("                    AND HIG1.MONETIZATION > 0 THEN QTD ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 2 ");
            stb.AppendFormat("                    AND HIG1.MONETIZATION_NIGHT > 0 THEN QTD ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 3 ");
            stb.AppendFormat("                    AND HIG1.MONETIZATION_LATENIGHT > 0 THEN QTD ");
            stb.AppendFormat("               ELSE 0 ");
            stb.AppendFormat("           END) AS QTD_MON, ");
            stb.AppendFormat("       SUM(CASE ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 1 ");
            stb.AppendFormat("                    AND HIG1.MONETIZATION > 0 THEN (HIG1.MONETIZATION * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 2 ");
            stb.AppendFormat("                    AND HIG1.MONETIZATION_NIGHT > 0 THEN (HIG1.MONETIZATION_NIGHT * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 3 ");
            stb.AppendFormat("                    AND HIG1.MONETIZATION_LATENIGHT > 0 THEN (HIG1.MONETIZATION_LATENIGHT * QTD) ");
            stb.AppendFormat("               ELSE 0 ");
            stb.AppendFormat("           END) AS QTD_MON_TOTAL, ");
            stb.AppendFormat("       MAX(TBL.IDGDA_SECTOR) AS IDGDA_SECTOR, ");
            stb.AppendFormat("       MAX(SEC.NAME) AS SETOR, ");
            stb.AppendFormat("       MAX(TBL.IDGDA_SUBSECTOR) AS IDGDA_SUBSECTOR, ");
            stb.AppendFormat("       MAX(SECSUB.NAME) AS SUBSETOR, ");
            stb.AppendFormat("       MAX(IT.IDGDA_INDICATOR) AS IDGDA_INDICATOR, ");
            stb.AppendFormat("       MAX(IT.NAME) AS 'INDICADOR', ");

            stb.AppendFormat(" 		   SUM(CASE ");
            stb.AppendFormat("              WHEN TBL.IDGDA_PERIOD = 1 ");
            stb.AppendFormat("                  AND HIS.GOAL IS NOT NULL THEN (QTD) ");
            stb.AppendFormat("              WHEN TBL.IDGDA_PERIOD = 2 ");
            stb.AppendFormat("                  AND HIS.GOAL_NIGHT > 0 THEN (QTD) ");
            stb.AppendFormat("              WHEN TBL.IDGDA_PERIOD = 3 ");
            stb.AppendFormat("                  AND HIS.GOAL_LATENIGHT > 0 THEN (QTD) ");
            stb.AppendFormat("              ELSE 0 ");
            stb.AppendFormat("          END) AS QTD_META, ");

            stb.AppendFormat("       MAX(TBL.IDGDA_HOMEBASED) AS HOME_BASED, ");
            stb.AppendFormat("       MAX(TBL.IDGDA_SITE) AS SITE, ");
            stb.AppendFormat("       MAX(TBL.IDGDA_PERIOD) AS TURNO, ");
            stb.AppendFormat("       MAX(TBL.MATRICULA_SUPERVISOR) AS 'MATRICULA SUPERVISOR', ");
            stb.AppendFormat("       MAX(TBL.NOME_SUPERVISOR) AS 'NOME SUPERVISOR', ");
            stb.AppendFormat("       MAX(TBL.MATRICULA_COORDENADOR) AS 'MATRICULA COORDENADOR', ");
            stb.AppendFormat("       MAX(TBL.NOME_COORDENADOR) AS 'NOME COORDENADOR', ");
            stb.AppendFormat("       MAX(TBL.MATRICULA_GERENTE_II) AS 'MATRICULA GERENTE II', ");
            stb.AppendFormat("       MAX(TBL.NOME_GERENTE_II) AS 'NOME GERENTE II', ");
            stb.AppendFormat("       MAX(TBL.MATRICULA_GERENTE_I) AS 'MATRICULA GERENTE I', ");
            stb.AppendFormat("       MAX(TBL.NOME_GERENTE_I) AS 'NOME GERENTE I', ");
            stb.AppendFormat("       MAX(TBL.MATRICULA_DIRETOR) AS 'MATRICULA DIRETOR', ");
            stb.AppendFormat("       MAX(TBL.NOME_DIRETOR) AS 'NOME DIRETOR', ");
            stb.AppendFormat("       MAX(TBL.MATRICULA_CEO) AS 'MATRICULA CEO', ");
            stb.AppendFormat("       MAX(TBL.NOME_CEO) AS 'NOME CEO', ");

            stb.AppendFormat("       SUM(CASE ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 1 THEN (HIS.GOAL * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 2 THEN (HIS.GOAL_NIGHT * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 3 THEN (HIS.GOAL_LATENIGHT * QTD) ");
            stb.AppendFormat("               ELSE 0 ");
            stb.AppendFormat("           END) AS META, ");
            stb.AppendFormat("       SUM(CASE ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 1 THEN (HIG1.METRIC_MIN * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 2 THEN (HIG1.METRIC_MIN_NIGHT * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 3 THEN (HIG1.METRIC_MIN_LATENIGHT * QTD) ");
            stb.AppendFormat("               ELSE 0 ");
            stb.AppendFormat("           END) AS MIN1, ");
            stb.AppendFormat("       SUM(CASE ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 1 THEN (HIG2.METRIC_MIN * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 2 THEN (HIG2.METRIC_MIN_NIGHT * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 3 THEN (HIG2.METRIC_MIN_LATENIGHT * QTD) ");
            stb.AppendFormat("               ELSE 0 ");
            stb.AppendFormat("           END) AS MIN2, ");
            stb.AppendFormat("       SUM(CASE ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 1 THEN (HIG3.METRIC_MIN * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 2 THEN (HIG3.METRIC_MIN_NIGHT * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 3 THEN (HIG3.METRIC_MIN_LATENIGHT * QTD) ");
            stb.AppendFormat("               ELSE 0 ");
            stb.AppendFormat("           END) AS MIN3, ");
            stb.AppendFormat("       SUM(CASE ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 1 THEN (HIG4.METRIC_MIN * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 2 THEN (HIG4.METRIC_MIN_NIGHT * QTD) ");
            stb.AppendFormat("               WHEN TBL.IDGDA_PERIOD = 3 THEN (HIG4.METRIC_MIN_LATENIGHT * QTD) ");
            stb.AppendFormat("               ELSE 0 ");
            stb.AppendFormat("           END) AS MIN4, ");
            stb.AppendFormat("       CASE ");
            stb.AppendFormat("           WHEN MAX(ME.EXPRESSION) IS NULL THEN '(#FATOR1/#FATOR0)' ");
            stb.AppendFormat("           ELSE MAX(ME.EXPRESSION) ");
            stb.AppendFormat("       END AS CONTA, ");
            stb.AppendFormat("       CASE ");
            stb.AppendFormat("           WHEN MAX(IT.CALCULATION_TYPE) IS NULL THEN 'BIGGER_BETTER' ");
            stb.AppendFormat("           ELSE MAX(IT.CALCULATION_TYPE) ");
            stb.AppendFormat("       END AS BETTER, ");
            stb.AppendFormat("       MAX(IT.TYPE) AS TYPE, ");
            stb.AppendFormat("       SUM(TBL.FACTOR0) AS FACTOR0, ");
            stb.AppendFormat("       SUM(TBL.FACTOR1) AS FACTOR1, ");
            stb.AppendFormat("       SUM(TBL.MOEDA_GANHA) AS MOEDA_GANHA, ");
            stb.AppendFormat("	   SUM(TBL.LOGADO) AS LOGADO, ");
            stb.AppendFormat("	   SUM(TBL.ESCALADO) AS ESCALADO, ");
            stb.AppendFormat("	   SUM(TBL.ESCALADOLOGADO) AS ESCALADOLOGADO ");
            stb.AppendFormat("FROM ");
            stb.AppendFormat("  (SELECT MAX(CB.NAME) AS NAME, CL.IDGDA_COLLABORATORS, CONVERT(DATE, CL.CREATED_AT) AS CREATED_AT, ");
            stb.AppendFormat("          COUNT(0) AS QTD, ");
            stb.AppendFormat("          CL.IDGDA_PERIOD AS IDGDA_PERIOD, ");
            stb.AppendFormat("          CL.IDGDA_SECTOR AS IDGDA_SECTOR, ");
            stb.AppendFormat("          CL.IDGDA_SUBSECTOR AS IDGDA_SUBSECTOR, ");
            stb.AppendFormat("          CL.IDGDA_SECTOR_SUBSECTOR AS IDGDA_SECTOR_SUBSECTOR, ");
            stb.AppendFormat("          IT.IDGDA_INDICATOR AS INDICADORID, ");
            stb.AppendFormat("          MAX(CL.CARGO) AS CARGO, ");

            stb.AppendFormat("     MAX(CL.IDGDA_HOMEBASED) AS IDGDA_HOMEBASED, ");
            stb.AppendFormat("     MAX(CL.IDGDA_SITE) AS IDGDA_SITE, ");

            stb.AppendFormat("     MAX(CL.MATRICULA_SUPERVISOR) AS MATRICULA_SUPERVISOR, ");
            stb.AppendFormat("     MAX(CL.NOME_SUPERVISOR) AS NOME_SUPERVISOR, ");
            stb.AppendFormat("     MAX(CL.MATRICULA_COORDENADOR) AS MATRICULA_COORDENADOR, ");
            stb.AppendFormat("     MAX(CL.NOME_COORDENADOR) AS NOME_COORDENADOR, ");
            stb.AppendFormat("     MAX(CL.MATRICULA_GERENTE_II) AS MATRICULA_GERENTE_II, ");
            stb.AppendFormat("     MAX(CL.NOME_GERENTE_II) AS NOME_GERENTE_II, ");
            stb.AppendFormat("     MAX(CL.MATRICULA_GERENTE_I) AS MATRICULA_GERENTE_I, ");
            stb.AppendFormat("     MAX(CL.NOME_GERENTE_I) AS NOME_GERENTE_I, ");
            stb.AppendFormat("     MAX(CL.MATRICULA_DIRETOR) AS MATRICULA_DIRETOR, ");
            stb.AppendFormat("     MAX(CL.NOME_DIRETOR) AS NOME_DIRETOR, ");
            stb.AppendFormat("     MAX(CL.MATRICULA_CEO) AS MATRICULA_CEO, ");
            stb.AppendFormat("     MAX(CL.NOME_CEO) AS NOME_CEO, ");


            stb.AppendFormat("          0 AS FACTOR0, ");
            stb.AppendFormat("          0 AS FACTOR1, ");
            stb.AppendFormat("          SUM(MZ.INPUT) AS MOEDA_GANHA, ");
            stb.AppendFormat("		    SUM(ESC.ESCALADO) AS ESCALADO, ");
            stb.AppendFormat("          SUM(LOG.LOGIN) AS LOGADO,  ");
            stb.AppendFormat("		  SUM(ESCLOG.ESCALADOLOGADO) AS ESCALADOLOGADO ");
            stb.AppendFormat("   FROM (SELECT CD.IDGDA_SECTOR, ");
            stb.AppendFormat("             CD.IDGDA_SUBSECTOR, ");
            stb.AppendFormat("             CD.CREATED_AT, ");
            stb.AppendFormat("             CD.IDGDA_COLLABORATORS, ");
            stb.AppendFormat("             CD.IDGDA_SECTOR_SUPERVISOR, ");
            stb.AppendFormat("             CASE WHEN CD.IDGDA_SUBSECTOR IS NOT NULL THEN CD.IDGDA_SUBSECTOR ");
            stb.AppendFormat("             ELSE CD.IDGDA_SECTOR END AS IDGDA_SECTOR_SUBSECTOR, ");
            stb.AppendFormat("             CD.ACTIVE, ");
            stb.AppendFormat("             CD.CARGO, ");
            stb.AppendFormat("             HB.IDGDA_HOMEBASED, ");
            stb.AppendFormat("             S.IDGDA_SITE, ");
            stb.AppendFormat("             CD.IDGDA_PERIOD, ");
            stb.AppendFormat("             CD.MATRICULA_SUPERVISOR, ");
            stb.AppendFormat("             CD.NOME_SUPERVISOR, ");
            stb.AppendFormat("             CD.MATRICULA_COORDENADOR, ");
            stb.AppendFormat("             CD.NOME_COORDENADOR, ");
            stb.AppendFormat("             CD.MATRICULA_GERENTE_II, ");
            stb.AppendFormat("             CD.NOME_GERENTE_II, ");
            stb.AppendFormat("             CD.MATRICULA_GERENTE_I, ");
            stb.AppendFormat("             CD.NOME_GERENTE_I, ");
            stb.AppendFormat("             CD.MATRICULA_DIRETOR, ");
            stb.AppendFormat("             CD.NOME_DIRETOR, ");
            stb.AppendFormat("             CD.MATRICULA_CEO, ");
            stb.AppendFormat("             CD.NOME_CEO ");
            stb.AppendFormat("      FROM GDA_COLLABORATORS_DETAILS (NOLOCK) CD ");
            stb.AppendFormat("      LEFT JOIN GDA_SITE (NOLOCK) S ON CD.SITE = S.SITE ");
            stb.AppendFormat("      LEFT JOIN GDA_HOMEBASED (NOLOCK) HB ON HB.HOMEBASED = CD.HOME_BASED ");
            stb.AppendFormat("      WHERE CD.CREATED_AT >= @DATAINICIAL ");
            stb.AppendFormat($"        AND CD.CREATED_AT <= @DATAFINAL ) AS CL ");
            stb.AppendFormat($"   INNER JOIN GDA_INDICATOR (NOLOCK) AS IT ON IT.IDGDA_INDICATOR IN ({indicatorsAsString}) ");//
            stb.AppendFormat("   LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS CB ON CB.IDGDA_COLLABORATORS = CL.IDGDA_COLLABORATORS ");
            stb.AppendFormat("   LEFT JOIN ");
            stb.AppendFormat("     (SELECT SUM(INPUT) - SUM(OUTPUT) AS INPUT, ");
            stb.AppendFormat("             gda_indicator_idgda_indicator, ");
            stb.AppendFormat("             result_date, ");
            stb.AppendFormat("             COLLABORATOR_ID ");
            stb.AppendFormat("      FROM GDA_CHECKING_ACCOUNT ");
            stb.AppendFormat("      WHERE RESULT_DATE >= @DATAINICIAL ");
            stb.AppendFormat("        AND RESULT_DATE <= @DATAFINAL ");
            //stb.AppendFormat("        AND COLLABORATOR_ID = @INPUTID ");
            stb.AppendFormat("      GROUP BY gda_indicator_idgda_indicator, ");
            stb.AppendFormat("               result_date, ");
            stb.AppendFormat("               COLLABORATOR_ID) AS MZ ON MZ.gda_indicator_idgda_indicator = IT.IDGDA_INDICATOR ");
            stb.AppendFormat("   AND MZ.result_date = CL.CREATED_AT ");
            stb.AppendFormat("   AND MZ.COLLABORATOR_ID = CL.IDGDA_COLLABORATORS ");
            stb.AppendFormat("LEFT JOIN  ");
            stb.AppendFormat("  (SELECT COUNT(0) AS 'ESCALADO',  ");
            stb.AppendFormat("          IDGDA_COLLABORATORS,  ");
            stb.AppendFormat("          CREATED_AT ");
            stb.AppendFormat("   FROM GDA_RESULT (NOLOCK)  ");
            stb.AppendFormat("   WHERE INDICADORID = -1  ");
            stb.AppendFormat("     AND CREATED_AT >= @DATAINICIAL  ");
            stb.AppendFormat("     AND CREATED_AT <= @DATAFINAL  ");
            stb.AppendFormat("     AND RESULT = 1  ");
            stb.AppendFormat("     AND DELETED_AT IS NULL ");
            stb.AppendFormat("   GROUP BY IDGDA_COLLABORATORS,  ");
            stb.AppendFormat("            CREATED_AT) AS ESC ON ESC.IDGDA_COLLABORATORS = CL.IDGDA_COLLABORATORS  ");
            stb.AppendFormat("AND ESC.CREATED_AT = CL.CREATED_AT  ");
            stb.AppendFormat("LEFT JOIN  ");
            stb.AppendFormat("  (SELECT COUNT(DISTINCT IDGDA_COLLABORATOR) AS 'LOGIN',  ");
            stb.AppendFormat("          IDGDA_COLLABORATOR,  ");
            stb.AppendFormat("          CONVERT(DATE, DATE_ACCESS) AS CREATED_AT  ");
            stb.AppendFormat("   FROM GDA_LOGIN_ACCESS (NOLOCK)  ");
            stb.AppendFormat("   WHERE CONVERT(DATE, DATE_ACCESS) >= @DATAINICIAL  ");
            stb.AppendFormat("     AND CONVERT(DATE, DATE_ACCESS) <= @DATAFINAL  ");
            stb.AppendFormat("   GROUP BY IDGDA_COLLABORATOR,  ");
            stb.AppendFormat("            CONVERT(DATE, DATE_ACCESS)) AS LOG ON LOG.IDGDA_COLLABORATOR = CL.IDGDA_COLLABORATORS  ");
            stb.AppendFormat("AND LOG.CREATED_AT = CL.CREATED_AT  ");
            stb.AppendFormat("LEFT JOIN  ");
            stb.AppendFormat("  (SELECT COUNT(0) AS 'ESCALADOLOGADO',  ");
            stb.AppendFormat("          IDGDA_COLLABORATORS,  ");
            stb.AppendFormat("          CREATED_AT  ");
            stb.AppendFormat("   FROM GDA_RESULT (NOLOCK) R ");
            stb.AppendFormat("   INNER JOIN GDA_LOGIN_ACCESS (NOLOCK) L ON CONVERT(DATE, DATE_ACCESS) = CREATED_AT AND L.IDGDA_COLLABORATOR = R.IDGDA_COLLABORATORS ");
            stb.AppendFormat("   WHERE INDICADORID = -1  ");
            stb.AppendFormat("     AND CREATED_AT >= @DATAINICIAL  ");
            stb.AppendFormat("     AND CREATED_AT <= @DATAFINAL  ");
            stb.AppendFormat("     AND RESULT = 1  ");
            stb.AppendFormat("     AND DELETED_AT IS NULL  ");
            stb.AppendFormat("   GROUP BY IDGDA_COLLABORATORS,  ");
            stb.AppendFormat("            CREATED_AT) AS ESCLOG ON ESCLOG.IDGDA_COLLABORATORS = CL.IDGDA_COLLABORATORS  ");
            stb.AppendFormat("			AND ESCLOG.CREATED_AT = CL.CREATED_AT  ");
            stb.AppendFormat(" ");
            stb.AppendFormat("   WHERE 1 = 1 ");
            stb.AppendFormat("     AND CL.CREATED_AT >= @DATAINICIAL ");
            stb.AppendFormat("     AND CL.CREATED_AT <= @DATAFINAL ");
            stb.AppendFormat($"     AND (CL.IDGDA_SECTOR IS NOT NULL OR CL.IDGDA_SUBSECTOR IS NOT NULL) ");
            stb.AppendFormat("     AND CL.CARGO IS NOT NULL ");
            stb.AppendFormat($"     AND CL.IDGDA_HOMEBASED <> ''  ");
            //stb.AppendFormat("     AND CL.ACTIVE = 'true'  ");
            stb.AppendFormat("     AND CL.CARGO = 'AGENTE' ");

            stb.AppendFormat("   GROUP BY IT.IDGDA_INDICATOR, ");
            stb.AppendFormat($"            CL.IDGDA_SECTOR, CL.IDGDA_SUBSECTOR, CL.IDGDA_SECTOR_SUBSECTOR, ");
            stb.AppendFormat("            CONVERT(DATE, CL.CREATED_AT), ");
            stb.AppendFormat("            IDGDA_PERIOD, CL.IDGDA_COLLABORATORS) AS TBL ");
            stb.AppendFormat("INNER JOIN GDA_INDICATOR (NOLOCK) AS IT ON IT.IDGDA_INDICATOR = TBL.INDICADORID ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
            stb.AppendFormat("AND HIG1.INDICATOR_ID = TBL.INDICADORID ");
            stb.AppendFormat($"AND HIG1.SECTOR_ID = TBL.IDGDA_SECTOR_SUBSECTOR ");
            stb.AppendFormat("AND HIG1.GROUPID = 1 ");
            stb.AppendFormat("AND CONVERT(DATE,HIG1.STARTED_AT) <= TBL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIG1.ENDED_AT) >= TBL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG2 ON HIG2.DELETED_AT IS NULL ");
            stb.AppendFormat("AND HIG2.INDICATOR_ID = TBL.INDICADORID ");
            stb.AppendFormat($"AND HIG2.SECTOR_ID = TBL.IDGDA_SECTOR_SUBSECTOR ");
            stb.AppendFormat("AND HIG2.GROUPID = 2 ");
            stb.AppendFormat("AND CONVERT(DATE,HIG2.STARTED_AT) <= TBL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIG2.ENDED_AT) >= TBL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG3 ON HIG3.DELETED_AT IS NULL ");
            stb.AppendFormat("AND HIG3.INDICATOR_ID = TBL.INDICADORID ");
            stb.AppendFormat($"AND HIG3.SECTOR_ID = TBL.IDGDA_SECTOR_SUBSECTOR ");
            stb.AppendFormat("AND HIG3.GROUPID = 3 ");
            stb.AppendFormat("AND CONVERT(DATE,HIG3.STARTED_AT) <= TBL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIG3.ENDED_AT) >= TBL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG4 ON HIG4.DELETED_AT IS NULL ");
            stb.AppendFormat("AND HIG4.INDICATOR_ID = TBL.INDICADORID ");
            stb.AppendFormat($"AND HIG4.SECTOR_ID = TBL.IDGDA_SECTOR_SUBSECTOR ");
            stb.AppendFormat("AND HIG4.GROUPID = 4 ");
            stb.AppendFormat("AND CONVERT(DATE,HIG4.STARTED_AT) <= TBL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIG4.ENDED_AT) >= TBL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN ");
            stb.AppendFormat("  (SELECT (SUM(GOAL) / COUNT(0)) AS GOAL, ");
            stb.AppendFormat("          (SUM(GOAL_NIGHT) / COUNT(0)) AS GOAL_NIGHT, ");
            stb.AppendFormat("          (SUM(GOAL_LATENIGHT) / COUNT(0)) AS GOAL_LATENIGHT, ");
            stb.AppendFormat("          INDICATOR_ID, ");
            stb.AppendFormat("          MAX(STARTED_AT) AS STARTED_AT, ");
            stb.AppendFormat("          MAX(ENDED_AT) AS ENDED_AT ");
            stb.AppendFormat("   FROM GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) ");
            stb.AppendFormat("   WHERE CONVERT(DATE,STARTED_AT) <= @DATAINICIAL ");
            stb.AppendFormat("     AND CONVERT(DATE,ENDED_AT) >= @DATAFINAL ");
            stb.AppendFormat($"     AND DELETED_AT IS NULL ");
            stb.AppendFormat("   GROUP BY INDICATOR_ID) AS HIS ON HIS.INDICATOR_ID = TBL.INDICADORID ");
            stb.AppendFormat("AND CONVERT(DATE,HIS.STARTED_AT) <= TBL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIS.ENDED_AT) >= TBL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME ON HME.INDICATORID = TBL.INDICADORID ");
            stb.AppendFormat("AND HME.deleted_at IS NULL ");
            stb.AppendFormat("LEFT JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON ME.ID = HME.MATHEMATICALEXPRESSIONID ");
            stb.AppendFormat("AND ME.DELETED_AT IS NULL ");
            stb.AppendFormat($"LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = TBL.IDGDA_SECTOR ");
            stb.AppendFormat($"LEFT JOIN GDA_SECTOR (NOLOCK) AS SECSUB ON SECSUB.IDGDA_SECTOR = TBL.IDGDA_SUBSECTOR WHERE TBL.IDGDA_COLLABORATORS = '794907'"); //
            stb.AppendFormat("GROUP BY TBL.IDGDA_COLLABORATORS, TBL.CREATED_AT, TBL.INDICADORID, TBL.IDGDA_SECTOR_SUBSECTOR ");

            List<semanalMensalConsolidado> Listhrc = new List<semanalMensalConsolidado>();

            using (SqlConnection connection = new SqlConnection(Database.retornaConn(true)))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                {
                    command.CommandTimeout = 0;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                semanalMensalConsolidado hrc = new semanalMensalConsolidado();
                                hrc.MATRICULA = reader["IDGDA_COLLABORATORS"].ToString();
                                hrc.NOME = reader["NAME"].ToString();
                                hrc.CARGO = reader["CARGO"].ToString();
                                hrc.SETOR = reader["SETOR"].ToString();
                                hrc.CODGIP = reader["IDGDA_SECTOR"].ToString();
                                hrc.SUBSETOR = reader["SUBSETOR"].ToString();
                                hrc.CODGIPSUBSETOR = reader["IDGDA_SUBSECTOR"].ToString();
                                //hrc.SETOR = reader["SETOR"].ToString();
                                hrc.IDINDICADOR = reader["IDGDA_INDICATOR"].ToString();
                                hrc.INDICADOR = reader["INDICADOR"].ToString();
                                //hrc.META = Double.Parse(reader["META"].ToString());
                                hrc.META = reader["META"].ToString() != "" ? double.Parse(reader["META"].ToString()) : 0;
                                hrc.min1 = reader["min1"].ToString() != "" ? double.Parse(reader["min1"].ToString()) : 0;
                                hrc.min2 = reader["min2"].ToString() != "" ? double.Parse(reader["min2"].ToString()) : 0;
                                hrc.min3 = reader["min3"].ToString() != "" ? double.Parse(reader["min3"].ToString()) : 0;
                                hrc.min4 = reader["min4"].ToString() != "" ? double.Parse(reader["min4"].ToString()) : 0;
                                hrc.CONTA = reader["CONTA"].ToString();
                                hrc.BETTER = reader["BETTER"].ToString();
                                hrc.FACTOR0 = reader["FACTOR0"].ToString() != "" ? double.Parse(reader["FACTOR0"].ToString()) : 0;
                                hrc.FACTOR1 = reader["FACTOR1"].ToString() != "" ? double.Parse(reader["FACTOR1"].ToString()) : 0;

                                hrc.SUMDIASLOGADOS = reader["LOGADO"].ToString() != "" ? Convert.ToInt32(reader["LOGADO"].ToString()) : 0;
                                hrc.SUMDIASESCALADOS = reader["ESCALADO"].ToString() != "" ? Convert.ToInt32(reader["ESCALADO"].ToString()) : 0;
                                hrc.SUMDIASLOGADOSESCALADOS = reader["ESCALADOLOGADO"].ToString() != "" ? Convert.ToInt32(reader["ESCALADOLOGADO"].ToString()) : 0;

                                hrc.MOEDA_GANHA = reader["MOEDA_GANHA"].ToString() != "" ? double.Parse(reader["MOEDA_GANHA"].ToString()) : 0;
                                hrc.QTD_MON = reader["QTD_MON"].ToString() != "" ? double.Parse(reader["QTD_MON"].ToString()) : 0;
                                hrc.QTD_MON_TOTAL = reader["QTD_MON_TOTAL"].ToString() != "" ? double.Parse(reader["QTD_MON_TOTAL"].ToString()) : 0;
                                hrc.QTD = Double.Parse(reader["QTD"].ToString());
                                hrc.DATAPAGAMENTO = reader["CREATED_AT"].ToString();
                                hrc.TYPE = reader["TYPE"].ToString();
                                hrc.MOEDA_GANHA = hrc.MOEDA_GANHA > 0 ? hrc.MOEDA_GANHA / hrc.QTD_MON : 0;
                                hrc.QTD_META = reader["QTD_META"].ToString() != "" ? double.Parse(reader["QTD_META"].ToString()) : 0;

                                hrc.MATRICULASUPERVISOR = reader["MATRICULA SUPERVISOR"].ToString() != "" ? reader["MATRICULA SUPERVISOR"].ToString() : "-";
                                hrc.NOMESUPERVISOR = reader["NOME SUPERVISOR"].ToString() != "" ? reader["NOME SUPERVISOR"].ToString() : "-";
                                hrc.MATRICULACOORDENADOR = reader["MATRICULA COORDENADOR"].ToString() != "" ? reader["MATRICULA COORDENADOR"].ToString() : "-";
                                hrc.NOMECOORDENADOR = reader["NOME COORDENADOR"].ToString() != "" ? reader["NOME COORDENADOR"].ToString() : "-";
                                hrc.MATRICULAGERENTE2 = reader["MATRICULA GERENTE II"].ToString() != "" ? reader["MATRICULA GERENTE II"].ToString() : "-";
                                hrc.NOMEGERENTE2 = reader["NOME GERENTE II"].ToString() != "" ? reader["NOME GERENTE II"].ToString() : "-";
                                hrc.MATRICULAGERENTE1 = reader["MATRICULA GERENTE I"].ToString() != "" ? reader["MATRICULA GERENTE I"].ToString() : "-";
                                hrc.NOMEGERENTE1 = reader["NOME GERENTE I"].ToString() != "" ? reader["NOME GERENTE I"].ToString() : "-";
                                hrc.MATRICULADIRETOR = reader["MATRICULA DIRETOR"].ToString() != "" ? reader["MATRICULA DIRETOR"].ToString() : "-";
                                hrc.NOMEDIRETOR = reader["NOME DIRETOR"].ToString() != "" ? reader["NOME DIRETOR"].ToString() : "-";
                                hrc.MATRICULACEO = reader["MATRICULA CEO"].ToString() != "" ? reader["MATRICULA CEO"].ToString() : "-";
                                hrc.NOMECEO = reader["NOME CEO"].ToString() != "" ? reader["NOME CEO"].ToString() : "-";

                                hrc.IDGDA_HOME = reader["HOME_BASED"].ToString() != "" ? int.Parse(reader["HOME_BASED"].ToString()) : 0;
                                hrc.IDGDA_PERIODO = reader["TURNO"].ToString() != "" ? int.Parse(reader["TURNO"].ToString()) : 0;
                                hrc.NOVATO = "";
                                hrc.IDGDA_SITE = reader["SITE"].ToString() != "" ? int.Parse(reader["SITE"].ToString()) : 0;

                                Listhrc.Add(hrc);
                            }
                            catch (Exception)
                            {

                            }

                        }
                    }
                }

                connection.Close();
            }
            return Listhrc;
        }


        public static semanalMensalConsolidado DoCalculateFinal(semanalMensalConsolidado consolidado)
        {
            try
            {
                List<groups> lgroup = returnTables.listGroups("");
                groups lgroup1 = lgroup.Find(l => l.id == 1);
                groups lgroup2 = lgroup.Find(l => l.id == 2);
                groups lgroup3 = lgroup.Find(l => l.id == 3);
                groups lgroup4 = lgroup.Find(l => l.id == 4);

                string contaAg = "";
                if (consolidado.IDINDICADOR == "10000013")
                {
                    //Calculo do Agente
                    contaAg = $"{consolidado.SUMDIASLOGADOSESCALADOS} / {consolidado.SUMDIASESCALADOS}";
                    //consolidado.META = consolidado.SUMDIASESCALADOS;
                }
                else if (consolidado.IDINDICADOR == "10000014")
                {
                    //Calculo do Agente
                    contaAg = $"{consolidado.SUMDIASLOGADOS} / {consolidado.SUMDIASESCALADOS}";
                    //consolidado.META = consolidado.SUMDIASESCALADOS;
                }
                else
                {
                    contaAg = consolidado.CONTA.Replace("#fator0", consolidado.FACTOR0.ToString()).Replace("#fator1", consolidado.FACTOR1.ToString());
                }



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

                //if (consolidado.CARGO != "AGENTE")
                //{
                if (consolidado.META == 0)
                {
                    consolidado.META = 0;
                }
                //else if (consolidado.QTD > 0)
                //{
                //    consolidado.META = consolidado.META / consolidado.QTD;
                //    consolidado.META = Math.Round(consolidado.META, 2, MidpointRounding.AwayFromZero);
                //}

                if (double.IsNaN(resultado))
                {
                    consolidado.GRUPO = "Bronze";
                    consolidado.PERCENTUAL = 0;
                    consolidado.RESULTADO = 0;


                    //retorno[i] = consolidado;
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

                    //retorno[i] = consolidado;

                }

                double resultadoD = resultado;

                if (consolidado.TYPE == null)
                {
                    resultadoD = resultadoD * 100;
                }
                else if (consolidado.TYPE == "PERCENT")
                {
                    resultadoD = resultadoD * 100;
                }
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
                    //double resultadoRetorno = Math.Round(resultadoD / 100, 0);

                    //if (consolidado.META < 1 && resultadoRetorno == 0 && consolidado.IDINDICADOR == "2")
                    //{
                    //    atingimentoMeta = 10;
                    //}
                    //else 
                    if(resultadoD == 0) // && factor.goal < 1
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
                    //consolidado.GRUPO = "Diamante";
                    consolidado.GRUPO = lgroup1.name;
                    consolidado.IDGRUPO = lgroup1.id;
                    consolidado.IMAGEMGRUPO = lgroup1.image;
                    consolidado.NOME_NIVEL = lgroup1.alias;
                }
                else if (atingimentoMeta >= consolidado.min2)
                {
                    //consolidado.GRUPO = "Ouro";
                    consolidado.GRUPO = lgroup2.name;
                    consolidado.IDGRUPO = lgroup2.id;
                    consolidado.IMAGEMGRUPO = lgroup2.image;
                    consolidado.NOME_NIVEL = lgroup2.alias;
                }
                else if (atingimentoMeta >= consolidado.min3)
                {
                    //consolidado.GRUPO = "Prata";
                    consolidado.GRUPO = lgroup3.name;
                    consolidado.IDGRUPO = lgroup3.id;
                    consolidado.IMAGEMGRUPO = lgroup3.image;
                    consolidado.NOME_NIVEL = lgroup3.alias;
                }
                else if (atingimentoMeta >= consolidado.min4)
                {
                    //consolidado.GRUPO = "Bronze";
                    consolidado.GRUPO = lgroup4.name;
                    consolidado.IDGRUPO = lgroup4.id;
                    consolidado.IMAGEMGRUPO = lgroup4.image;
                    consolidado.NOME_NIVEL = lgroup4.alias;
                }
                else
                {
                    //consolidado.GRUPO = "Bronze";
                    consolidado.GRUPO = lgroup4.name;
                    consolidado.IDGRUPO = lgroup4.id;
                    consolidado.IMAGEMGRUPO = lgroup4.image;
                    consolidado.NOME_NIVEL = lgroup4.alias;
                }

                if (consolidado.TYPE == "INTEGER" || consolidado.TYPE == "HOUR")
                {
                    consolidado.PERCENTUAL = atingimentoMeta;
                    consolidado.RESULTADO = Math.Round(resultadoD, 0, MidpointRounding.AwayFromZero);
                }
                else
                {
                    consolidado.PERCENTUAL = atingimentoMeta;
                    consolidado.RESULTADO = resultadoD;
                }

                //}
            }
            catch (Exception)
            {

            }


            return consolidado;
        }

    }
}