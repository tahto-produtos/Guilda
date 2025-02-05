using System;
using System.Web;
using ApiC.Class;
using System.Text;
using System.Web.Http;
using System.Configuration;
using System.Data.SqlClient;
using ApiRepositorio.Models;
using System.Collections.Generic;
using System.Web.Http.Description;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using ConfigurationManager = System.Configuration.ConfigurationManager;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;
using System.Linq;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.Diagnostics.Runtime.Utilities;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Text.RegularExpressions;
using static ApiRepositorio.Controllers.InputMassiveMetricController;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Reflection;
using Microsoft.CodeAnalysis.Operations;
using System.Globalization;
using static TokenService;
using OfficeOpenXml.Drawing.Slicer.Style;


namespace ApiRepositorio.Controllers
{
    public class InputMassiveMetricController : ApiController
    {

        public static List<RawRow> Rows = new List<RawRow>();

        // Classes auxiliares
        public class FailedRow
        {
            public string FailedReason { get; set; }
            public RawRow Row { get; set; }
        }

        public class RawRow
        {
            public string NomeIndicador { get; set; }
            public string CodIndicador { get; set; }
            public string ValorMeta { get; set; }
            public string CodGipSetor { get; set; }
            public string Setor { get; set; }
            public string CodGipSubsetor { get; set; }
            public string Subsetor { get; set; }
            public string Turno { get; set; }
            public string Status { get; set; }
            public string Moeda1 { get; set; }
            public string Moeda2 { get; set; }
            public string Moeda3 { get; set; }
            public string Moeda4 { get; set; }
            public string Peso { get; set; }
            public string Grupo1 { get; set; }
            public string Grupo2 { get; set; }
            public string Grupo3 { get; set; }
            public string Grupo4 { get; set; }
            public string DataInicio { get; set; }
            public string DataFinal { get; set; }

        }

        public class IndicatorData
        {
            public string Cod { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
        }

        public class SectorData
        {
            public string Cod { get; set; }
            public string Name { get; set; }
        }


        public class ProcessResult
        {
            public int totalUpdated { get; set; }
            public int totalCreated { get; set; }
            public List<string> creates { get; set; }
            public List<string> updates { get; set; }
            public List<string> failed { get; set; }
        }

        [HttpPost]
        public IHttpActionResult InputMassiveMetric()
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


            var createdIndicatorSectors = new List<string>();
            var updatedIndicatorSectors = new List<string>();
            List<FailedRow> faileds = new List<FailedRow>();
            Rows.Clear();

            HttpPostedFile arquivo = HttpContext.Current.Request.Files["FILE"];
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                if (arquivo != null && arquivo.ContentLength > 0)
                {
                    using (ExcelPackage package = new ExcelPackage(arquivo.InputStream))
                    {

                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                        // Filtra as linhas que têm algum valor não vazio

                        try
                        {
                            //Consulta os Indicadores
                            List<IndicatorData> listIndicatorData = new List<IndicatorData>();
                            listIndicatorData = FunctionInputMetric.GetIndicatorData();

                            //Lista de setores
                            List<SectorData> listSectorData = new List<SectorData>();
                            listSectorData = FunctionInputMetric.GetSectorData();

                            //Verifica colunas
                            faileds = FunctionInputMetric.ProcessSheet(worksheet, listIndicatorData, listSectorData);

                            if (faileds.Count > 0)
                            {
                                ProcessResult pr = new ProcessResult();
                                pr.totalUpdated = updatedIndicatorSectors.Count;
                                pr.totalCreated = createdIndicatorSectors.Count;
                                pr.creates = createdIndicatorSectors;
                                pr.updates = updatedIndicatorSectors;
                                pr.failed = faileds.Select(f => f.FailedReason).ToList();

                                return Ok(pr);
                            }

                            Logs.InsertActionLogs("IMPORT_METRICS", "GDA_LOG_ACTIONS_IMPORT_METRICS", COLLABORATORID.ToString());
                            //Remover
                            foreach (RawRow r in Rows.FindAll(s => s.Status == "0").ToList())
                            {
                                string codGipReal = "";
                                if (r.CodGipSubsetor != "")
                                {
                                    codGipReal = r.CodGipSubsetor;
                                }
                                else
                                {
                                    codGipReal = r.CodGipSetor;
                                }

                                if (FunctionInputMetric.RemoveMetric(codGipReal, r) == true)
                                {
                                    updatedIndicatorSectors.Add(r.NomeIndicador);
                                }
                            }

                            //Inserir
                            foreach (RawRow r in Rows.FindAll(s => s.Status == "1").ToList())
                            {

                                if (FunctionInputMetric.AddMetric(r, COLLABORATORID) == true)
                                {
                                    createdIndicatorSectors.Add(r.NomeIndicador);
                                }
                            }

                            //Realiza o Input
                            //ImportSingleMetric(worksheet);
                        }
                        catch (Exception ex)
                        {
                            return BadRequest(ex.Message);
                        }

                    }

                }
                else
                {
                    return BadRequest("Arquivo não enviado.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



            ProcessResult pr2 = new ProcessResult();
            pr2.totalUpdated = updatedIndicatorSectors.Count;
            pr2.totalCreated = createdIndicatorSectors.Count;
            pr2.creates = createdIndicatorSectors;
            pr2.updates = updatedIndicatorSectors;
            pr2.failed = faileds.Select(f => f.FailedReason).ToList();

            return Ok(pr2);
        }

        public class FunctionInputMetric
        {
            public static List<FailedRow> ProcessSheet(ExcelWorksheet worksheet, List<IndicatorData> listIndicatorData, List<SectorData> listSectorData)
            {
                var failed = new List<FailedRow>();

                var usedRows = worksheet.Cells
                .Where(cell => !string.IsNullOrWhiteSpace(cell.Text))
                .Select(cell => cell.Start.Row)
                .Distinct();

                var rows = usedRows.Count();
                //var rows = worksheet.Dimension.Rows;
                var header = new List<string>();

                if (rows == 0)
                {
                    throw new Exception("Planilha de importação não pode ser vazia");
                }

                // Captura cabeçalhos
                for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                {
                    header.Add(worksheet.Cells[1, col].Text);
                }

                var sheetModel = 2;
                int indexEnv = 1;

                for (int row = 2; row <= rows; row++)
                {
                    try
                    {


                        indexEnv++;
                        var rawRow = new RawRow
                        {
                            // Captura os valores das células da linha atual
                            NomeIndicador = worksheet.Cells[row, 1].Text,
                            CodIndicador = worksheet.Cells[row, 2].Text,
                            ValorMeta = worksheet.Cells[row, 3].Text,
                            CodGipSetor = worksheet.Cells[row, 4].Text,
                            Setor = worksheet.Cells[row, 5].Text,
                            CodGipSubsetor = worksheet.Cells[row, 6].Text,
                            Subsetor = worksheet.Cells[row, 7].Text,
                            Turno = worksheet.Cells[row, 8].Text,
                            Status = worksheet.Cells[row, 9].Text,
                            Moeda1 = worksheet.Cells[row, 10].Text,
                            Moeda2 = worksheet.Cells[row, 11].Text,
                            Moeda3 = worksheet.Cells[row, 12].Text,
                            Moeda4 = worksheet.Cells[row, 13].Text,
                            Peso = worksheet.Cells[row, 14].Text,
                            Grupo1 = worksheet.Cells[row, 15].Text,
                            Grupo2 = worksheet.Cells[row, 16].Text,
                            Grupo3 = worksheet.Cells[row, 17].Text,
                            Grupo4 = worksheet.Cells[row, 18].Text,
                            DataInicio = worksheet.Cells[row, 19].Text,
                            DataFinal = worksheet.Cells[row, 20].Text,
                            // Continue capturando as demais colunas da planilha aqui
                        };

                        var dataInicioStr = worksheet.Cells[row, 19].Style.Numberformat.Format;
                        if (dataInicioStr.ToString() != "@")
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"A coluna data de inicio tem que ter a formatação texto. Linha {indexEnv}",
                                Row = rawRow
                            });
                            continue;
                        }

                        var dataFinalStr = worksheet.Cells[row, 20].Style.Numberformat.Format;
                        if (dataFinalStr.ToString() != "@")
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"A coluna data de final tem que ter a formatação texto. Linha {indexEnv}",
                                Row = rawRow
                            });
                            continue;
                        }

                        var dtIniVerif = DateTime.ParseExact(rawRow.DataInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        var dtFinVerif = DateTime.ParseExact(rawRow.DataFinal, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        if (dtIniVerif > dtFinVerif)
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"A coluna data de inicio tem que ser menor que a data final. Linha {indexEnv}",
                                Row = rawRow
                            });
                            continue;
                        }

                        ExcelRange teste = worksheet.Cells[row, 19];
                        rawRow.Grupo1 = rawRow.Grupo1.ToString().Split("%")[0];
                        rawRow.Grupo2 = rawRow.Grupo2.ToString().Split("%")[0];
                        rawRow.Grupo3 = rawRow.Grupo3.ToString().Split("%")[0];
                        rawRow.Grupo4 = rawRow.Grupo4.ToString().Split("%")[0];
                        rawRow.ValorMeta = rawRow.ValorMeta.ToString().Replace(",", ".");

                        string nome = "";
                        string codGipReal = "";
                        if (rawRow.CodGipSubsetor != "")
                        {
                            nome = "Subsetor";
                            codGipReal = rawRow.CodGipSubsetor;
                        }
                        else
                        {
                            nome = "Setor";
                            codGipReal = rawRow.CodGipSetor;
                        }


                        // Query para obter o indicador
                        IndicatorData indicatorData = listIndicatorData.Find(u => u.Cod == rawRow.CodIndicador);

                        if (indicatorData == null)
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"Indicador não encontrado: Linha {indexEnv}",
                                Row = rawRow
                            });
                            continue;
                        }

                        SectorData sectorData = listSectorData.Find(u => u.Cod == codGipReal);

                        if (sectorData == null)
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"Setor não encontrado: Linha {indexEnv}",
                                Row = rawRow
                            });
                            continue;
                        }

                        if (rawRow.DataInicio == "")
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"Não foi informado a data de inicio: Linha {indexEnv}",
                                Row = rawRow
                            });
                            continue;
                        }

                        if (rawRow.DataFinal == "")
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"Não foi informado a data final: Linha {indexEnv}",
                                Row = rawRow
                            });
                            continue;
                        }

                        int history = existHistory(codGipReal, rawRow.CodIndicador, rawRow.DataInicio, rawRow.DataFinal, rawRow.Turno);

                        if (history == 1 && rawRow.Status == "1")
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"Meta já configurada no periodo informado: Linha {indexEnv}",
                                Row = rawRow
                            });
                            continue;
                        }
                        if (history == 0 && rawRow.Status == "0")
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"Meta não encontrada para desativação: Linha {indexEnv}",
                                Row = rawRow
                            });
                            continue;
                        }

                        int existFailed = 0;

                        //verifica se ja existe uma linha configurada para este setor e indicador que batam a data de inicio e fim
                        List<RawRow> RowsFind = new List<RawRow>();

                        if (rawRow.CodGipSubsetor != "")
                        {

                            RowsFind = Rows.FindAll(r => r.CodIndicador == rawRow.CodIndicador && r.CodGipSubsetor == codGipReal && r.Status == "1" && r.Turno == rawRow.Turno); 
                        }
                        else
                        {

                            RowsFind = Rows.FindAll(r => r.CodIndicador == rawRow.CodIndicador && r.CodGipSetor == codGipReal && r.CodGipSubsetor == "" && r.Status == "1" && r.Turno == rawRow.Turno); 
                        }
                        foreach (RawRow item in RowsFind)
                        {


                            DateTime rrDataInicio = DateTime.Now;
                            DateTime rrDataFinal = DateTime.Now;
                            DateTime itDataInicio = DateTime.Now;
                            DateTime itDataFinal = DateTime.Now;
                            itDataInicio = DateTime.ParseExact(item.DataInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            itDataFinal = DateTime.ParseExact(item.DataFinal, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            rrDataInicio = DateTime.ParseExact(rawRow.DataInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            rrDataFinal = DateTime.ParseExact(rawRow.DataFinal, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                            bool semIntersecao = itDataFinal < rrDataInicio || rrDataFinal < itDataInicio;

                            if (semIntersecao == false)
                            {

                                failed.Add(new FailedRow
                                {
                                    FailedReason = $"Duas metas para este indicador e {nome} neste mesmo arquivo com conflito de datas: Linha {indexEnv}",
                                    Row = rawRow
                                });
                                existFailed = 1;
                                break;
                            }
                            if (rrDataFinal >= itDataInicio && rrDataFinal <= itDataFinal)
                            {
                                failed.Add(new FailedRow
                                {
                                    FailedReason = $"Duas metas para este indicador e {nome} neste mesmo arquivo com conflito de datas: Linha {indexEnv}",
                                    Row = rawRow
                                });
                                existFailed = 1;
                                break;
                            }
                        }

                        if (existFailed == 1)
                        {
                            continue;
                        }

                        List<RawRow> RowsFind2 = new List<RawRow>();
                        if (rawRow.CodGipSubsetor != "")
                        {
                            RowsFind2 = Rows.FindAll(r => r.CodIndicador == rawRow.CodIndicador && r.CodGipSubsetor == codGipReal && r.Status == "1" && r.Turno == rawRow.Turno);
                        }
                        else
                        {
                            RowsFind2 = Rows.FindAll(r => r.CodIndicador == rawRow.CodIndicador && r.CodGipSetor == codGipReal && r.CodGipSubsetor == "" && r.Status == "1" && r.Turno == rawRow.Turno);
                        }
                        foreach (RawRow item in RowsFind2)
                        {
                            //DateTime rrDataInicio = DateTime.ParseExact(rawRow.DataInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            //DateTime rrDataFinal = DateTime.ParseExact(rawRow.DataFinal, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            //DateTime itDataInicio = DateTime.ParseExact(item.DataInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            //DateTime itDataFinal = DateTime.ParseExact(item.DataFinal, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                            DateTime rrDataInicio = DateTime.Now;
                            DateTime rrDataFinal = DateTime.Now;
                            DateTime itDataInicio = DateTime.Now;
                            DateTime itDataFinal = DateTime.Now;

                            itDataInicio = DateTime.ParseExact(item.DataInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            itDataFinal = DateTime.ParseExact(item.DataFinal, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            rrDataInicio = DateTime.ParseExact(rawRow.DataInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            rrDataFinal = DateTime.ParseExact(rawRow.DataFinal, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                            bool semIntersecao = itDataFinal < rrDataInicio || rrDataFinal < itDataInicio;

                            if (semIntersecao == false)
                            {
                                failed.Add(new FailedRow
                                {
                                    FailedReason = $"Configuração de datas diferente entre periodos para o mesmo {nome} e indicador: Linha {indexEnv}",
                                    Row = rawRow
                                });
                                existFailed = 1;
                                break;
                            }
                        }

                        if (existFailed == 1)
                        {
                            continue;
                        }


                        string typeIndicator = indicatorData.Type;

                        if (indicatorData.Name.ToLower() != rawRow.NomeIndicador.ToLower())
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"Cod do indicador não bate com o nome cadastrado: Linha {indexEnv}",
                                Row = rawRow
                            });
                            continue;
                        }

                        if (rawRow == null)
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"Erro na linha {indexEnv}: row está indefinido.",
                                Row = rawRow
                            });
                            continue;
                        }

                        if (string.IsNullOrEmpty(rawRow.CodIndicador?.ToString()))
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"Erro na linha {indexEnv}: Falha no indicador {rawRow.NomeIndicador}: coluna COD INDICADOR necessita ser um número de indicador correto",
                                Row = rawRow
                            });
                            continue;
                        }

                        string[] periods = { "diurno", "noturno", "madrugada" };
                        string vlMetaOrigim = string.Empty;
                        string vlMeta = string.Empty;

                        if (rawRow.ValorMeta != null)
                        {
                            vlMetaOrigim = rawRow.ValorMeta.ToString();

                            if (typeIndicator == "HOUR")
                            {
                                vlMeta = ExtractTime(vlMetaOrigim);
                                if (vlMeta == "O formato da string de tempo não é válido")
                                {
                                    failed.Add(new FailedRow
                                    {
                                        FailedReason = $"Erro na linha {indexEnv}: O formato do valor de meta para o tipo hora, não é valido.",
                                        Row = rawRow
                                    });
                                    continue;
                                }
                                vlMeta = TimeToSeconds(vlMeta).ToString();
                            }
                            else
                            {
                                vlMeta = vlMetaOrigim.Replace(".%", "");
                            }
                        }
                        rawRow.ValorMeta = vlMeta;





                        // Validação do tipo de indicador
                        if (typeIndicator == "PERCENT" && !vlMetaOrigim.Contains("%"))
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"Erro na linha {indexEnv}: Falha na coluna VALOR DA META {rawRow.NomeIndicador}, indicador percentual precisa do símbolo %",
                                Row = rawRow
                            });
                            continue;
                        }
                        else if (typeIndicator == "HOUR" && !vlMetaOrigim.Contains(":"))
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"Erro na linha {indexEnv}: Falha na coluna VALOR DA META, indicador hora precisa ser no formato hh:mm:ss",
                                Row = rawRow
                            });
                            continue;
                        }
                        else if (typeIndicator != null && typeIndicator != "PERCENT" && vlMetaOrigim.Contains("%"))
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"Erro na linha {indexEnv}: Falha na coluna VALOR DA META, indicador inteiro não pode ter o símbolo %",
                                Row = rawRow
                            });
                            continue;
                        }

                        // Validação do turno
                        if (string.IsNullOrEmpty(rawRow.Turno?.ToString()))
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"Erro na linha {indexEnv}: coluna TURNO, necessita ser um TURNO correto",
                                Row = rawRow
                            });
                            continue;
                        }

                        if (!periods.Contains(rawRow.Turno?.ToString().ToLower()))
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"Erro na linha {indexEnv}: coluna TURNO, necessita ser um TURNO correto",
                                Row = rawRow
                            });
                            continue;
                        }

                        // Validação do setor ou subsetor
                        if (string.IsNullOrEmpty(rawRow.CodGipSubsetor?.ToString()) && string.IsNullOrEmpty(rawRow.CodGipSetor?.ToString()))
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"Erro na linha {indexEnv}: coluna SUBSETOR ou CODIGO GIP, necessita ser um número de setor correto",
                                Row = rawRow
                            });
                            continue;
                        }

                        // Validação do valor da meta
                        if (!decimal.TryParse(vlMeta, out decimal parsedVlMeta))
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"Erro na linha {indexEnv}: Falha na coluna VALOR DA META, não pode ser letras ou vazio",
                                Row = rawRow
                            });
                            continue;
                        }

                        // Validação do status
                        if (!new[] { "0", "1" }.Contains(rawRow.Status?.ToString()))
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"Erro na linha {indexEnv}: coluna STATUS, deve ser 0 ou 1",
                                Row = rawRow
                            });
                            continue;
                        }

                        // Validação das moedas

                        if (!Int32.TryParse(rawRow.Moeda1?.ToString(), out _))
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"Erro na linha {indexEnv}: coluna MOEDA GRUPO 1, deve ser um numero inteiro sem virgulas, não pode ser letras ou vazio",
                                Row = rawRow
                            });
                            continue;
                        }
                        if (!Int32.TryParse(rawRow.Moeda2?.ToString(), out _))
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"Erro na linha {indexEnv}: coluna MOEDA GRUPO 2, deve ser um numero inteiro sem virgulas, não pode ser letras ou vazio",
                                Row = rawRow
                            });
                            continue;
                        }
                        if (!Int32.TryParse(rawRow.Moeda3?.ToString(), out _))
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"Erro na linha {indexEnv}: coluna MOEDA GRUPO 3, deve ser um numero inteiro sem virgulas, não pode ser letras ou vazio",
                                Row = rawRow
                            });
                            continue;
                        }
                        if (!Int32.TryParse(rawRow.Moeda4?.ToString(), out _))
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"Erro na linha {indexEnv}: coluna MOEDA GRUPO 4, deve ser um numero inteiro sem virgulas, não pode ser letras ou vazio",
                                Row = rawRow
                            });
                            continue;
                        }

                        // Validação do peso
                        if (!decimal.TryParse(rawRow.Peso?.ToString(), out _))
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"Erro na linha {indexEnv}: coluna PESO, não pode ser letras ou vazio",
                                Row = rawRow
                            });
                            continue;
                        }

                        // Validação das colunas G1, G2, G3, G4
                        if (string.IsNullOrEmpty(rawRow.Grupo1?.ToString()))
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"Erro na linha {indexEnv}: coluna G1, não pode ser letras ou vazio",
                                Row = rawRow
                            });
                            continue;
                        }
                        if (string.IsNullOrEmpty(rawRow.Grupo2?.ToString()))
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"Erro na linha {indexEnv}: coluna G2, não pode ser letras ou vazio",
                                Row = rawRow
                            });
                            continue;
                        }
                        if (string.IsNullOrEmpty(rawRow.Grupo3?.ToString()))
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"Erro na linha {indexEnv}: coluna G3, não pode ser letras ou vazio",
                                Row = rawRow
                            });
                            continue;
                        }
                        if (string.IsNullOrEmpty(rawRow.Grupo4?.ToString()))
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"Erro na linha {indexEnv}: coluna G4, não pode ser letras ou vazio",
                                Row = rawRow
                            });
                            continue;
                        }

                        // Validação do histórico existente
                        if (rawRow.Status?.ToString() == "0" && history == 0)
                        {
                            failed.Add(new FailedRow
                            {
                                FailedReason = $"Erro na linha {indexEnv}: Falha ao inativar uma métrica que não existe.",
                                Row = rawRow
                            });
                            continue;
                        }

                        Rows.Add(rawRow);
                    }
                    catch (Exception ex)
                    {
                        failed.Add(new FailedRow
                        {
                            FailedReason = $"Erro na linha {indexEnv}: {ex.Message.ToString()}",
                            Row = new RawRow { }
                        });
                    }
                    string aaaa = "";
                }

                return failed;
            }

            public static int TimeToSeconds(string timeString)
            {
                var timeParts = timeString.Split(':').Select(int.Parse).ToArray();
                int hours = timeParts[0];
                int minutes = timeParts[1];
                int seconds = timeParts.Length > 2 ? timeParts[2] : 0; // Verifica se existem segundos

                return (hours * 3600) + (minutes * 60) + seconds;
            }

            public static string ExtractTime(string timeString)
            {
                // Verifica se o tempo já está no formato HH:mm:ss
                if (Regex.IsMatch(timeString, @"^\d{2}:\d{2}:\d{2}$"))
                {
                    return timeString; // Retorna diretamente se já estiver no formato correto
                }

                // Caso esteja em outro formato (ex.: convertido para data), extrai a parte do tempo
                var timeParts = timeString.Split(' ');

                if (timeParts.Length >= 5)
                {
                    return timeParts[4]; // Assume que a hora está na 5ª posição
                }

                return "O formato da string de tempo não é válido";
            }

            public static List<SectorData> GetSectorData()
            {
                List<SectorData> listSectorData = new List<SectorData>();
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        StringBuilder stb = new StringBuilder();
                        stb.AppendFormat("SELECT IDGDA_SECTOR, NAME FROM GDA_SECTOR (NOLOCK) ");
                        stb.AppendFormat("WHERE DELETED_AT IS NULL ");

                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    listSectorData.Add(new SectorData
                                    {
                                        Cod = reader["IDGDA_SECTOR"].ToString(),
                                        Name = reader["NAME"].ToString(),
                                    });
                                }
                            }



                        }
                        connection.Close();
                    }
                    catch (Exception)
                    {
                        connection.Close();
                    }

                    return listSectorData;
                }

            }

            public static List<IndicatorData> GetIndicatorData()
            {
                List<IndicatorData> listIndicatorData = new List<IndicatorData>();
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        StringBuilder stb = new StringBuilder();
                        stb.AppendFormat("SELECT IDGDA_INDICATOR, NAME, TYPE ");
                        stb.AppendFormat("FROM GDA_INDICATOR (NOLOCK) ");
                        stb.AppendFormat("WHERE DELETED_AT IS NULL ");


                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    listIndicatorData.Add(new IndicatorData
                                    {
                                        Cod = reader["IDGDA_INDICATOR"].ToString(),
                                        Name = reader["NAME"].ToString(),
                                        Type = reader["TYPE"].ToString(),
                                    });
                                }
                            }



                        }
                        connection.Close();
                    }
                    catch (Exception)
                    {
                        connection.Close();
                    }

                    return listIndicatorData;
                }

            }

            public static int existHistory(string sectorId, string indicatorId, string dataInicio, string dataFinal, string turno)
            {
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    //string dtInicio = Convert.ToDateTime(dataInicio).ToString("yyyy-MM-dd");
                    //string dtFim = Convert.ToDateTime(dataFinal).ToString("yyyy-MM-dd");
                    string dtInicio = "";
                    string dtFim = "";


                    dtInicio = DateTime.ParseExact(dataInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    dtFim = DateTime.ParseExact(dataFinal, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

                    int retorno = 0;
                    connection.Open();
                    try
                    {
                        string filtro = "";
                        //if (turno.ToUpper() == "NOTURNO")
                        //{
                        //    filtro = " AND GOAL_NIGHT IS NOT NULL ";
                        //}
                        //else if (turno.ToUpper() == "MADRUGADA")
                        //{
                        //    filtro = " AND GOAL_LATENIGHT IS NOT NULL ";
                        //}
                        //else
                        //{
                        //    filtro = " AND GOAL IS NOT NULL ";
                        //}


                        StringBuilder stb = new StringBuilder();
                        stb.AppendFormat("SELECT * FROM GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) ");
                        stb.AppendFormat("WHERE DELETED_AT IS NULL ");
                        stb.AppendFormat($"AND SECTOR_ID = '{sectorId}' ");
                        stb.AppendFormat($"AND INDICATOR_ID = '{indicatorId}' ");
                        stb.AppendFormat($"AND '{dtInicio}' >= STARTED_AT AND '{dtInicio}' <= ENDED_AT ");
                        stb.AppendFormat($"AND '{dtFim}' >= STARTED_AT AND '{dtFim}' <= ENDED_AT {filtro} ");

                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    var dtInicioR = Convert.ToDateTime(reader["STARTED_AT"]).ToString("yyyy-MM-dd");
                                    var dtFinalR = Convert.ToDateTime(reader["ENDED_AT"]).ToString("yyyy-MM-dd");
                                    var goal = reader["GOAL"].ToString();
                                    var goalNight = reader["GOAL_NIGHT"].ToString();
                                    var goalLateNight = reader["GOAL_LATENIGHT"].ToString();

                                    if (dtInicioR != dtInicio || dtFinalR != dtFim)
                                    {
                                        retorno = 1;
                                    }

                                    if (turno.ToUpper() == "NOTURNO")
                                    {
                                        if (goalNight != "")
                                        {
                                            retorno = 1;
                                        }
                                    }
                                    else if (turno.ToUpper() == "MADRUGADA")
                                    {
                                        if (goalLateNight != "")
                                        {
                                            retorno = 1;
                                        }
                                    }
                                    else
                                    {
                                        if (goal != "")
                                        {
                                            retorno = 1;
                                        }
                                    }
                                }
                            }
                        }


                        stb.Clear();
                        stb.AppendFormat("SELECT * FROM GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) ");
                        stb.AppendFormat("WHERE DELETED_AT IS NULL ");
                        stb.AppendFormat($"AND SECTOR_ID = '{sectorId}' ");
                        stb.AppendFormat($"AND INDICATOR_ID = '{indicatorId}' ");
                        stb.AppendFormat($"AND STARTED_AT >= '{dtInicio}' ");
                        stb.AppendFormat($"AND ENDED_AT <= '{dtFim}' ");

                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    var dtInicioR = Convert.ToDateTime(reader["STARTED_AT"]).ToString("yyyy-MM-dd");
                                    var dtFinalR = Convert.ToDateTime(reader["ENDED_AT"]).ToString("yyyy-MM-dd");
                                    var goal = reader["GOAL"].ToString();
                                    var goalNight = reader["GOAL_NIGHT"].ToString();
                                    var goalLateNight = reader["GOAL_LATENIGHT"].ToString();

                                    if (dtInicioR != dtInicio || dtFinalR != dtFim)
                                    {
                                        retorno = 1;
                                    }

                                    if (turno.ToUpper() == "NOTURNO")
                                    {
                                        if (goalNight != "")
                                        {
                                            retorno = 1;
                                        }
                                    }
                                    else if (turno.ToUpper() == "MADRUGADA")
                                    {
                                        if (goalLateNight != "")
                                        {
                                            retorno = 1;
                                        }
                                    }
                                    else
                                    {
                                        if (goal != "")
                                        {
                                            retorno = 1;
                                        }
                                    }
                                }
                            }
                        }

                        connection.Close();
                    }
                    catch (Exception)
                    {
                        connection.Close();
                    }

                    return retorno;
                }
            }

            public static bool AddMetric(RawRow row, int idColaborator)
            {

                string codGipReal = "";
                string codGipReferencia = "NULL";
                if (row.CodGipSubsetor != "")
                {
                    codGipReal = row.CodGipSubsetor;
                    codGipReferencia = row.CodGipSetor;
                }
                else
                {
                    codGipReal = row.CodGipSetor;
                }

                row.ValorMeta = row.ValorMeta.Replace(",", ".");
                row.Grupo1 = row.Grupo1.Replace(",", ".");
                row.Grupo2 = row.Grupo2.Replace(",", ".");
                row.Grupo3 = row.Grupo3.Replace(",", ".");
                row.Grupo4 = row.Grupo4.Replace(",", ".");

                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        string dtInicio = "";
                        string dtFim = "";

                        dtInicio = DateTime.ParseExact(row.DataInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        dtFim = DateTime.ParseExact(row.DataFinal, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

                        int exist = 0;
                        StringBuilder stb = new StringBuilder();
                        stb.AppendFormat("SELECT * FROM GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) ");
                        stb.AppendFormat("WHERE DELETED_AT IS NULL ");
                        stb.AppendFormat($"AND SECTOR_ID = '{codGipReal}' ");
                        stb.AppendFormat($"AND INDICATOR_ID = '{row.CodIndicador}' ");
                        stb.AppendFormat($"AND '{dtInicio}' = STARTED_AT ");
                        stb.AppendFormat($"AND '{dtFim}' = ENDED_AT ");

                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    exist = 1;
                                }
                            }
                        }
                        //Caso exista, só atualizar o noturno madrugada ou periodo que ainda não existia
                        if (exist == 1)
                        {
                            string filterGroup1 = "";
                            string filterGroup2 = "";
                            string filterGroup3 = "";
                            string filterGroup4 = "";
                            string filterSector = "";
                            if (row.Turno.ToUpper() == "MADRUGADA")
                            {
                                filterGroup1 = $" METRIC_MIN_LATENIGHT = '{row.Grupo1}', MONETIZATION_LATENIGHT = '{row.Moeda1}' ";
                                filterGroup2 = $" METRIC_MIN_LATENIGHT = '{row.Grupo2}', MONETIZATION_LATENIGHT = '{row.Moeda2}' ";
                                filterGroup3 = $" METRIC_MIN_LATENIGHT = '{row.Grupo3}', MONETIZATION_LATENIGHT = '{row.Moeda3}' ";
                                filterGroup4 = $" METRIC_MIN_LATENIGHT = '{row.Grupo4}', MONETIZATION_LATENIGHT = '{row.Moeda4}' ";
                                filterSector = $" GOAL_LATENIGHT = '{row.ValorMeta}' ";
                            }
                            else if (row.Turno.ToUpper() == "NOTURNO")
                            {
                                filterGroup1 = $" METRIC_MIN_NIGHT = '{row.Grupo1}', MONETIZATION_NIGHT = '{row.Moeda1}' ";
                                filterGroup2 = $" METRIC_MIN_NIGHT = '{row.Grupo2}', MONETIZATION_NIGHT = '{row.Moeda2}' ";
                                filterGroup3 = $" METRIC_MIN_NIGHT = '{row.Grupo3}', MONETIZATION_NIGHT = '{row.Moeda3}' ";
                                filterGroup4 = $" METRIC_MIN_NIGHT = '{row.Grupo4}', MONETIZATION_NIGHT = '{row.Moeda4}' ";
                                filterSector = $" GOAL_NIGHT = '{row.ValorMeta}' ";
                            }
                            else
                            {
                                filterGroup1 = $" METRIC_MIN = '{row.Grupo1}', MONETIZATION = '{row.Moeda1}' ";
                                filterGroup2 = $" METRIC_MIN = '{row.Grupo2}', MONETIZATION = '{row.Moeda2}' ";
                                filterGroup3 = $" METRIC_MIN = '{row.Grupo3}', MONETIZATION = '{row.Moeda3}' ";
                                filterGroup4 = $" METRIC_MIN = '{row.Grupo4}', MONETIZATION = '{row.Moeda4}' ";
                                filterSector = $" GOAL = '{row.ValorMeta}' ";
                            }


                            stb.Clear();
                            stb.AppendFormat("UPDATE GDA_HISTORY_INDICATOR_GROUP ");
                            stb.AppendFormat($"SET {filterGroup1} ");
                            stb.AppendFormat("WHERE DELETED_AT IS NULL ");
                            stb.AppendFormat($"AND SECTOR_ID = '{codGipReal}' ");
                            stb.AppendFormat($"AND INDICATOR_ID = '{row.CodIndicador}' ");
                            stb.AppendFormat($"AND '{dtInicio}' = STARTED_AT ");
                            stb.AppendFormat($"AND '{dtFim}' = ENDED_AT AND GROUPID = 1 ");

                            using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                            {
                                command.ExecuteNonQuery();
                            }

                            stb.Clear();
                            stb.AppendFormat("UPDATE GDA_HISTORY_INDICATOR_GROUP ");
                            stb.AppendFormat($"SET {filterGroup2} ");
                            stb.AppendFormat("WHERE DELETED_AT IS NULL ");
                            stb.AppendFormat($"AND SECTOR_ID = '{codGipReal}' ");
                            stb.AppendFormat($"AND INDICATOR_ID = '{row.CodIndicador}' ");
                            stb.AppendFormat($"AND '{dtInicio}' = STARTED_AT ");
                            stb.AppendFormat($"AND '{dtFim}' = ENDED_AT AND GROUPID = 2 ");

                            using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                            {
                                command.ExecuteNonQuery();
                            }

                            stb.Clear();
                            stb.AppendFormat("UPDATE GDA_HISTORY_INDICATOR_GROUP ");
                            stb.AppendFormat($"SET {filterGroup3} ");
                            stb.AppendFormat("WHERE DELETED_AT IS NULL ");
                            stb.AppendFormat($"AND SECTOR_ID = '{codGipReal}' ");
                            stb.AppendFormat($"AND INDICATOR_ID = '{row.CodIndicador}' ");
                            stb.AppendFormat($"AND '{dtInicio}' = STARTED_AT ");
                            stb.AppendFormat($"AND '{dtFim}' = ENDED_AT AND GROUPID = 3 ");

                            using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                            {
                                command.ExecuteNonQuery();
                            }

                            stb.Clear();
                            stb.AppendFormat("UPDATE GDA_HISTORY_INDICATOR_GROUP ");
                            stb.AppendFormat($"SET {filterGroup4} ");
                            stb.AppendFormat("WHERE DELETED_AT IS NULL ");
                            stb.AppendFormat($"AND SECTOR_ID = '{codGipReal}' ");
                            stb.AppendFormat($"AND INDICATOR_ID = '{row.CodIndicador}' ");
                            stb.AppendFormat($"AND '{dtInicio}' = STARTED_AT ");
                            stb.AppendFormat($"AND '{dtFim}' = ENDED_AT AND GROUPID = 4 ");

                            using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                            {
                                command.ExecuteNonQuery();
                            }

                            stb.Clear();
                            stb.AppendFormat("UPDATE GDA_HISTORY_INDICATOR_SECTORS ");
                            stb.AppendFormat($"SET {filterSector} ");
                            stb.AppendFormat("WHERE DELETED_AT IS NULL ");
                            stb.AppendFormat($"AND SECTOR_ID = '{codGipReal}' ");
                            stb.AppendFormat($"AND INDICATOR_ID = '{row.CodIndicador}' ");
                            stb.AppendFormat($"AND '{dtInicio}' = STARTED_AT ");
                            stb.AppendFormat($"AND '{dtFim}' = ENDED_AT ");

                            using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                            {
                                command.ExecuteNonQuery();
                            }

                        }
                        else
                        {
                            string metric_min = "NULL";
                            string metric_min2 = "NULL";
                            string metric_min3 = "NULL";
                            string metric_min4 = "NULL";
                            string metric_min_night = "NULL";
                            string metric_min_night2 = "NULL";
                            string metric_min_night3 = "NULL";
                            string metric_min_night4 = "NULL";
                            string metric_min_latenight = "NULL";
                            string metric_min_latenight2 = "NULL";
                            string metric_min_latenight3 = "NULL";
                            string metric_min_latenight4 = "NULL";
                            string monetization = "NULL";
                            string monetization2 = "NULL";
                            string monetization3 = "NULL";
                            string monetization4 = "NULL";
                            string monetization_night = "NULL";
                            string monetization_night2 = "NULL";
                            string monetization_night3 = "NULL";
                            string monetization_night4 = "NULL";
                            string monetization_latenight = "NULL";
                            string monetization_latenight2 = "NULL";
                            string monetization_latenight3 = "NULL";
                            string monetization_latenight4 = "NULL";
                            string goal = "NULL";
                            string goal_night = "NULL";
                            string goal_latenight = "NULL";

                            if (row.Turno.ToUpper() == "MADRUGADA")
                            {
                                metric_min_latenight = $"'{row.Grupo1}'";
                                metric_min_latenight2 = $"'{row.Grupo2}'";
                                metric_min_latenight3 = $"'{row.Grupo3}'";
                                metric_min_latenight4 = $"'{row.Grupo4}'";
                                monetization_latenight = $"'{row.Moeda1}'";
                                monetization_latenight2 = $"'{row.Moeda2}'";
                                monetization_latenight3 = $"'{row.Moeda3}'";
                                monetization_latenight4 = $"'{row.Moeda4}'";
                                goal_latenight = row.ValorMeta;
                            }
                            else if (row.Turno.ToUpper() == "NOTURNO")
                            {
                                metric_min_night = $"'{row.Grupo1}'";
                                metric_min_night2 = $"'{row.Grupo2}'";
                                metric_min_night3 = $"'{row.Grupo3}'";
                                metric_min_night4 = $"'{row.Grupo4}'";
                                monetization_night = $"'{row.Moeda1}'";
                                monetization_night2 = $"'{row.Moeda2}'";
                                monetization_night3 = $"'{row.Moeda3}'";
                                monetization_night4 = $"'{row.Moeda4}'";
                                goal_night = row.ValorMeta;
                            }
                            else
                            {
                                metric_min = $"'{row.Grupo1}'";
                                metric_min2 = $"'{row.Grupo2}'";
                                metric_min3 = $"'{row.Grupo3}'";
                                metric_min4 = $"'{row.Grupo4}'";
                                monetization = $"'{row.Moeda1}'";
                                monetization2 = $"'{row.Moeda2}'";
                                monetization3 = $"'{row.Moeda3}'";
                                monetization4 = $"'{row.Moeda4}'";
                                goal = row.ValorMeta;
                            }

                            //Caso não exista, inserir o primeiro caso
                            stb.Clear();
                            stb.AppendFormat("INSERT INTO GDA_HISTORY_INDICATOR_GROUP (INDICATOR_ID, SECTOR_ID, METRIC_MIN, MONETIZATION, GROUPID, CREATED_AT, DELETED_AT, ENDED_AT, STARTED_AT, METRIC_MIN_NIGHT, METRIC_MIN_LATENIGHT, MONETIZATION_NIGHT, MONETIZATION_LATENIGHT, SECTOR_ID_PARENT) ");
                            stb.AppendFormat("VALUES (");
                            stb.AppendFormat($"{row.CodIndicador}, "); //INDICATOR_ID
                            stb.AppendFormat($"{codGipReal}, "); //SECTOR_ID
                            stb.AppendFormat($"{metric_min}, "); //METRIC_MIN
                            stb.AppendFormat($"{monetization}, "); //MONETIZATION
                            stb.AppendFormat($"1, "); //GROUPID
                            stb.AppendFormat($"GETDATE(), "); //CREATED_AT
                            stb.AppendFormat($"NULL, "); //DELETED_AT
                            stb.AppendFormat($"'{dtFim}', "); //ENDED_AT
                            stb.AppendFormat($"'{dtInicio}', "); //STARTED_AT
                            stb.AppendFormat($"{metric_min_night}, "); //METRIC_MIN_NIGHT
                            stb.AppendFormat($"{metric_min_latenight}, "); //METRIC_MIN_LATENIGHT
                            stb.AppendFormat($"{monetization_night}, "); //MONETIZATION_NIGHT
                            stb.AppendFormat($"{monetization_latenight}, "); //MONETIZATION_LATENIGHT
                            stb.AppendFormat($"{codGipReferencia} "); //SECTOR_ID_PARENT
                            stb.AppendFormat(") ");

                            using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                            {
                                command.ExecuteNonQuery();
                            }

                            stb.Clear();
                            stb.AppendFormat("INSERT INTO GDA_HISTORY_INDICATOR_GROUP (INDICATOR_ID, SECTOR_ID, METRIC_MIN, MONETIZATION, GROUPID, CREATED_AT, DELETED_AT, ENDED_AT, STARTED_AT, METRIC_MIN_NIGHT, METRIC_MIN_LATENIGHT, MONETIZATION_NIGHT, MONETIZATION_LATENIGHT, SECTOR_ID_PARENT) ");
                            stb.AppendFormat("VALUES (");
                            stb.AppendFormat($"{row.CodIndicador}, "); //INDICATOR_ID
                            stb.AppendFormat($"{codGipReal}, "); //SECTOR_ID
                            stb.AppendFormat($"{metric_min2}, "); //METRIC_MIN
                            stb.AppendFormat($"{monetization2}, "); //MONETIZATION
                            stb.AppendFormat($"2, "); //GROUPID
                            stb.AppendFormat($"GETDATE(), "); //CREATED_AT
                            stb.AppendFormat($"NULL, "); //DELETED_AT
                            stb.AppendFormat($"'{dtFim}', "); //ENDED_AT
                            stb.AppendFormat($"'{dtInicio}', "); //STARTED_AT
                            stb.AppendFormat($"{metric_min_night2}, "); //METRIC_MIN_NIGHT
                            stb.AppendFormat($"{metric_min_latenight2}, "); //METRIC_MIN_LATENIGHT
                            stb.AppendFormat($"{monetization_night2}, "); //MONETIZATION_NIGHT
                            stb.AppendFormat($"{monetization_latenight2}, "); //MONETIZATION_LATENIGHT
                            stb.AppendFormat($"{codGipReferencia} "); //SECTOR_ID_PARENT
                            stb.AppendFormat(") ");

                            using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                            {
                                command.ExecuteNonQuery();
                            }

                            stb.Clear();
                            stb.AppendFormat("INSERT INTO GDA_HISTORY_INDICATOR_GROUP (INDICATOR_ID, SECTOR_ID, METRIC_MIN, MONETIZATION, GROUPID, CREATED_AT, DELETED_AT, ENDED_AT, STARTED_AT, METRIC_MIN_NIGHT, METRIC_MIN_LATENIGHT, MONETIZATION_NIGHT, MONETIZATION_LATENIGHT, SECTOR_ID_PARENT) ");
                            stb.AppendFormat("VALUES (");
                            stb.AppendFormat($"{row.CodIndicador}, "); //INDICATOR_ID
                            stb.AppendFormat($"{codGipReal}, "); //SECTOR_ID
                            stb.AppendFormat($"{metric_min3}, "); //METRIC_MIN
                            stb.AppendFormat($"{monetization3}, "); //MONETIZATION
                            stb.AppendFormat($"3, "); //GROUPID
                            stb.AppendFormat($"GETDATE(), "); //CREATED_AT
                            stb.AppendFormat($"NULL, "); //DELETED_AT
                            stb.AppendFormat($"'{dtFim}', "); //ENDED_AT
                            stb.AppendFormat($"'{dtInicio}', "); //STARTED_AT
                            stb.AppendFormat($"{metric_min_night3}, "); //METRIC_MIN_NIGHT
                            stb.AppendFormat($"{metric_min_latenight3}, "); //METRIC_MIN_LATENIGHT
                            stb.AppendFormat($"{monetization_night3}, "); //MONETIZATION_NIGHT
                            stb.AppendFormat($"{monetization_latenight3}, "); //MONETIZATION_LATENIGHT
                            stb.AppendFormat($"{codGipReferencia} "); //SECTOR_ID_PARENT
                            stb.AppendFormat(") ");

                            using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                            {
                                command.ExecuteNonQuery();
                            }

                            stb.Clear();
                            stb.AppendFormat("INSERT INTO GDA_HISTORY_INDICATOR_GROUP (INDICATOR_ID, SECTOR_ID, METRIC_MIN, MONETIZATION, GROUPID, CREATED_AT, DELETED_AT, ENDED_AT, STARTED_AT, METRIC_MIN_NIGHT, METRIC_MIN_LATENIGHT, MONETIZATION_NIGHT, MONETIZATION_LATENIGHT, SECTOR_ID_PARENT) ");
                            stb.AppendFormat("VALUES (");
                            stb.AppendFormat($"{row.CodIndicador}, "); //INDICATOR_ID
                            stb.AppendFormat($"{codGipReal}, "); //SECTOR_ID
                            stb.AppendFormat($"{metric_min4}, "); //METRIC_MIN
                            stb.AppendFormat($"{monetization4}, "); //MONETIZATION
                            stb.AppendFormat($"4, "); //GROUPID
                            stb.AppendFormat($"GETDATE(), "); //CREATED_AT
                            stb.AppendFormat($"NULL, "); //DELETED_AT
                            stb.AppendFormat($"'{dtFim}', "); //ENDED_AT
                            stb.AppendFormat($"'{dtInicio}', "); //STARTED_AT
                            stb.AppendFormat($"{metric_min_night4}, "); //METRIC_MIN_NIGHT
                            stb.AppendFormat($"{metric_min_latenight4}, "); //METRIC_MIN_LATENIGHT
                            stb.AppendFormat($"{monetization_night4}, "); //MONETIZATION_NIGHT
                            stb.AppendFormat($"{monetization_latenight4}, "); //MONETIZATION_LATENIGHT
                            stb.AppendFormat($"{codGipReferencia} "); //SECTOR_ID_PARENT
                            stb.AppendFormat(") ");

                            using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                            {
                                command.ExecuteNonQuery();
                            }


                            stb.Clear();
                            stb.AppendFormat("INSERT INTO GDA_HISTORY_INDICATOR_SECTORS (INDICATOR_ID, SECTOR_ID, CREATED_AT, DELETED_AT, GOAL, ACTIVE, ALTERED_BY, ENDED_AT, STARTED_AT, WEIGHT_BASKET, GOAL_NIGHT, GOAL_LATENIGHT, SECTOR_ID_PARENT, WEIGHT, WEIGHT_NIGHT, WEIGHT_LATENIGHT) ");
                            stb.AppendFormat("VALUES ( ");
                            stb.AppendFormat($"{row.CodIndicador}, "); //INDICATOR_ID
                            stb.AppendFormat($"{codGipReal}, "); //SECTOR_ID
                            stb.AppendFormat($"GETDATE(), "); //CREATED_AT
                            stb.AppendFormat($"NULL, "); //DELETED_AT
                            stb.AppendFormat($"{goal}, "); //GOAL
                            stb.AppendFormat($"1, "); //ACTIVE
                            stb.AppendFormat($"{idColaborator}, "); //ALTERED_BY
                            stb.AppendFormat($"'{dtFim}', "); //ENDED_AT
                            stb.AppendFormat($"'{dtInicio}', "); //STARTED_AT
                            stb.AppendFormat($"NULL, "); //WEIGHT_BASKET
                            stb.AppendFormat($"{goal_night}, "); //GOAL_NIGHT
                            stb.AppendFormat($"{goal_latenight}, "); //GOAL_LATENIGHT
                            stb.AppendFormat($"{codGipReferencia}, "); //SECTOR_ID_PARENT
                            stb.AppendFormat($"NULL, "); //WEIGHT
                            stb.AppendFormat($"NULL, "); //WEIGHT_NIGHT
                            stb.AppendFormat($"NULL "); //WEIGHT_LATENIGHT
                            stb.AppendFormat(")");

                            using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                            {
                                command.ExecuteNonQuery();
                            }
                        }

                        connection.Close();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        connection.Close();
                        return false;
                    }

                }
            }

            public static bool RemoveMetric(string sectorId, RawRow row)
            {


                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {

                        row.ValorMeta = row.ValorMeta.Replace(",", ".");
                        row.Grupo1 = row.Grupo1.Replace(",", ".");
                        row.Grupo2 = row.Grupo2.Replace(",", ".");
                        row.Grupo3 = row.Grupo3.Replace(",", ".");
                        row.Grupo4 = row.Grupo4.Replace(",", ".");

                        //string dtInicio = Convert.ToDateTime(row.DataInicio).ToString("yyyy-MM-dd");
                        //string dtFim = Convert.ToDateTime(row.DataFinal).ToString("yyyy-MM-dd");
                        string dtInicio = "";
                        string dtFim = "";

                        dtInicio = DateTime.ParseExact(row.DataInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        dtFim = DateTime.ParseExact(row.DataFinal, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

                        StringBuilder stb = new StringBuilder();
                        stb.AppendFormat("UPDATE GDA_HISTORY_INDICATOR_GROUP  ");
                        stb.AppendFormat("SET DELETED_AT = GETDATE() ");
                        stb.AppendFormat("WHERE DELETED_AT IS NULL ");
                        stb.AppendFormat($"AND SECTOR_ID = '{sectorId}' ");
                        stb.AppendFormat($"AND INDICATOR_ID = '{row.CodIndicador}' ");
                        stb.AppendFormat($"AND CONVERT(DATE, STARTED_AT) = '{dtInicio}' ");
                        stb.AppendFormat($"AND CONVERT(DATE, ENDED_AT) = '{dtFim}' ");

                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        stb.Clear();
                        stb.AppendFormat("UPDATE GDA_HISTORY_INDICATOR_SECTORS  ");
                        stb.AppendFormat("SET DELETED_AT = GETDATE() ");
                        stb.AppendFormat("WHERE DELETED_AT IS NULL ");
                        stb.AppendFormat($"AND SECTOR_ID = '{sectorId}' ");
                        stb.AppendFormat($"AND INDICATOR_ID = '{row.CodIndicador}' ");
                        stb.AppendFormat($"AND CONVERT(DATE, STARTED_AT) = '{dtInicio}' ");
                        stb.AppendFormat($"AND CONVERT(DATE, ENDED_AT) = '{dtFim}' ");

                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        connection.Close();
                        return true;
                    }
                    catch (Exception)
                    {
                        connection.Close();
                        return false;
                    }

                }
            }
        }


    }
}