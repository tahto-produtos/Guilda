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
using System.Drawing.Imaging;
using Microsoft.Diagnostics.Runtime;
using ApiC.Class;
using OfficeOpenXml;
using System.Net.NetworkInformation;
using Iced.Intel;
using System.Collections;
using static ApiRepositorio.Controllers.ResultConsolidatedController;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ScoreInputController : ApiController
    {
        public class InputModel
        {
            public int Sectorid { get; set; }
            public List<Indicators> Indicators { get; set; }
            public int Matricula { get; set; }
        }
        public class Indicators
        {
            public int Id { get; set; }
            public int Score { get; set; }
            public int Sectorid { get; set; }
        }
        public string VerificaVingenciaIndicador(int IndicadorID, int SectorID, string dataatual)
        {
            string retorno = "";
            StringBuilder stb = new StringBuilder();
            stb.Append("SELECT * FROM GDA_HISTORY_SCORE_INDICATOR_SECTOR (NOLOCK) ");
            stb.Append($"WHERE INDICATOR_ID = '{IndicadorID}' AND SECTOR_ID = '{SectorID}'  AND CONVERT(DATE,STARTED_AT) <= '{dataatual}' AND CONVERT(DATE,ENDED_AT) >= '{dataatual}' ");


            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.CommandTimeout = 60;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Temos vingencia no periodo do mes do indicador nesse setor
                                retorno = reader["ID"].ToString();
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    connection.Close();
                    throw;
                }
                connection.Close();
            }

            return retorno;
        }
        [HttpPost]
        // POST: api/Results
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {


            int ControleInput = 0;
            string DataAtual = DateTime.Now.ToString("yyyy-MM-dd");
            int matricula = inputModel.Matricula;
            int SectorID = inputModel.Sectorid;
            List<Indicators> Indicators = new List<Indicators>();
            Indicators = inputModel.Indicators;

            List<Indicators> retorno = new List<Indicators>();

            string idLog = Logs.InsertActionLogs("Insert Score Input", "GDA_HISTORY_SCORE_INDICATOR_SECTOR", matricula.ToString());


            //A conta para input massivo e salvamento tem que dar sempre 100%, caso não bater 100% não irá inserir.
            retorno = Indicators.GroupBy(d => new { d.Sectorid }).Select(item => new Indicators
            { 
            Score = item.Sum(d => d.Score), 
            }).ToList();

            int totalscore = retorno.Max(d => d.Score);

            if (totalscore != 100)
            {
                return BadRequest("Indicadores inseridos não batem 100%");
            }

            if (Indicators.Count > 0)

                foreach (Indicators indicador in Indicators)
                {
                    int IndicadorID = indicador.Id;
                    string score = indicador.Score.ToString();

                    //VERIFICAR SE JA TEMOS UMA VINGENCIA NO MES.
                    string VerificaDataVigencia = VerificaVingenciaIndicador(IndicadorID, SectorID, DataAtual);
                    bool QuebraDiaSeguinte = false;
                    if (VerificaDataVigencia != "")
                    {
                        //VERIFICAR SE JA TEM UMA QUEBRA PARA DIA SEGUINTE, CASO TIVER, ATUALIZAR ESSA QUEBRA.
                        using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                        {
                            try
                            {
                                connection.Open();
                                StringBuilder stb = new StringBuilder();
                                stb.Append("SELECT * FROM GDA_HISTORY_SCORE_INDICATOR_SECTOR (NOLOCK) ");
                                stb.AppendFormat($"WHERE INDICATOR_ID = '{IndicadorID}' AND SECTOR_ID = {SectorID} AND CONVERT(DATE,STARTED_AT) > '{DataAtual}'");
                                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                                {
                                    using (SqlDataReader reader = command.ExecuteReader())
                                    {
                                        if (reader.Read())
                                        {
                                            VerificaDataVigencia = reader["ID"].ToString();
                                            QuebraDiaSeguinte = true;
                                        }
                                    }
                                }

                                if (QuebraDiaSeguinte == true)
                                {
                                    //UPDATE NO SCORE QUE ESTÁ EM PARA QUEBRAR NO DIA SEGUINTE.
                                    double scoreValue = double.Parse(score);
                                    double weightScore = scoreValue / 100;
                                    StringBuilder stb2 = new StringBuilder();
                                    stb2.Append("UPDATE GDA_HISTORY_SCORE_INDICATOR_SECTOR SET ");
                                    stb2.AppendFormat("WEIGHT_SCORE = '{0}' ", weightScore.ToString().Replace(",", "."));
                                    stb2.AppendFormat("WHERE ID = {0}  ", VerificaDataVigencia);

                                    using (SqlCommand command = new SqlCommand(stb2.ToString(), connection))
                                    {
                                        command.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                        try
                                        {
                                            //UPDATE SCORE QUE ESTÁ EM VINGENCIA.
                                            StringBuilder stb3 = new StringBuilder();
                                        stb3.Append("UPDATE GDA_HISTORY_SCORE_INDICATOR_SECTOR SET ");
                                        stb3.AppendFormat("ENDED_AT = '{0}' ", DataAtual);
                                        stb3.AppendFormat("WHERE ID = {0}  ", VerificaDataVigencia);

                                            using (SqlCommand command = new SqlCommand(stb3.ToString(), connection))
                                            {
                                                command.ExecuteNonQuery();
                                            }

                                            string UltimoDiaMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)).ToString("yyyy-MM-dd");

                                            //INSERIR SCORE NOVO COM SUA VIGENCIA ATUALIZADA.
                                            string DataSeguinteAtual = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");

                                            double scoreValue = double.Parse(score);
                                            // Multiplicar por 100
                                            double weightScore = scoreValue / 100;
                                            StringBuilder sb4 = new StringBuilder();
                                        sb4.Append("INSERT GDA_HISTORY_SCORE_INDICATOR_SECTOR (INDICATOR_ID, SECTOR_ID, CREATED_AT, WEIGHT_SCORE, ALTERED_BY, ENDED_AT, STARTED_AT) VALUES ( ");
                                        sb4.AppendFormat(" '{0}', ", IndicadorID);
                                        sb4.AppendFormat(" '{0}', ", SectorID);
                                        sb4.AppendFormat(" GETDATE(), ");
                                        sb4.AppendFormat(" '{0}', ", weightScore.ToString().Replace(",", "."));
                                        sb4.AppendFormat(" '{0}', ", matricula);
                                        sb4.AppendFormat(" '{0}',", UltimoDiaMes);
                                        sb4.AppendFormat(" '{0}') ", DataSeguinteAtual);

                                            using (SqlCommand command = new SqlCommand(sb4.ToString(), connection))
                                            {
                                                command.ExecuteNonQuery();
                                            }
                                        }
                                        catch (Exception)
                                        {
                                            throw;
                                        }
                                        connection.Close();
                                    }

                            }
                            catch (Exception)
                            {
                                throw;
                            }
                            connection.Close();

                        }
                    }
                    else 
                    {
                        using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                        {
                            try
                            {
                                //COMO NÃO TEMOS VINGENCIA NO MÊS, SERÁ INSERIDO A PRIMERA VIGENCIA DO INDICADOR NO SETOR ESPECIFICO,
                                ControleInput = 1;
                                string UltimoDiaMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)).ToString("yyyy-MM-dd");
                                string PrimeiroDiaMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy-MM-dd");
                                double scoreValue = double.Parse(score);
                                // Multiplicar por 100
                                double weightScore = scoreValue / 100;
                                connection.Open();
                                StringBuilder stb5 = new StringBuilder();
                                stb5.Append("INSERT GDA_HISTORY_SCORE_INDICATOR_SECTOR (INDICATOR_ID, SECTOR_ID, CREATED_AT, WEIGHT_SCORE, ALTERED_BY, ENDED_AT, STARTED_AT) VALUES ( ");
                                stb5.AppendFormat("'{0}', ", IndicadorID);
                                stb5.AppendFormat("'{0}', ", SectorID);
                                stb5.AppendFormat("GETDATE(), ");
                                stb5.AppendFormat("'{0}', ", weightScore.ToString().Replace(",","."));
                                stb5.AppendFormat("'{0}', ", matricula);
                                stb5.AppendFormat("'{0}', ", UltimoDiaMes);
                                stb5.AppendFormat("'{0}') ", PrimeiroDiaMes);

                                using (SqlCommand command = new SqlCommand(stb5.ToString(), connection))
                                {
                                    command.ExecuteNonQuery();
                                }
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                            connection.Close();
                        }
                    }
                }
            else
            {
                        return BadRequest("Indicadores não enviado corretamente, tente novamente.");
            }

            if(ControleInput == 1)
            {
                return Ok("Indicadores inseridos com sucesso.");

            }
            else
            {
                return Ok("Indicadores inseridos e atualizados com sucesso. Vigencia será aplicada no dia seguinte.");
            }





            //HttpPostedFile arquivo = HttpContext.Current.Request.Files["FILE"];
            //string matricula = HttpContext.Current.Request.Form["MATRICULA"];
            //string idLog = Logs.InsertActionLogs("Insert Input score", "CRIAR TABELA DE LOG DE SCORE", matricula);

            //if (arquivo != null && arquivo.ContentLength > 0)
            //{
            //    using (ExcelPackage package = new ExcelPackage(arquivo.InputStream))
            //    {
            //        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

            //        int rowCount = worksheet.Dimension.Rows;
            //        for (int row = 2; row <= rowCount; row++)
            //        {
            //            qtdTotal += 1;
            //            //string NomeProduto = worksheet.Cells[row,1].Value.ToString();
            //            string IndicadorID = worksheet.Cells[row,2].Value.ToString();
            //            string Score = worksheet.Cells[row, 3].Value.ToString();
            //            //string SectorID = worksheet.Cells[row,4].Value.ToString();

            //            //VERIFICAR SE JA TEMOS UMA VINGENCIA NO MES.
            //            string VerificaDataVigencia = VerificaVingenciaIndicador(IndicadorID, SectorID, DataAtual);   
            //            bool QuebraDiaSeguinte = false;
            //            if (VerificaDataVigencia != "" )
            //            {
            //                //VERIFICAR SE JA TEM UMA QUEBRA PARA DIA SEGUINTE, CASO TIVER, ATUALIZAR ESSA QUEBRA.
            //                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            //                {
            //                    try
            //                    {
            //                        connection.Open();
            //                        StringBuilder stb = new StringBuilder();
            //                        stb.Append("SELECT * FROM GDA_HISTORY_SCORE_INDICATOR_SECTOR (NOLOCK) ");
            //                        stb.AppendFormat($"WHERE INDICATOR_ID = '{IndicadorID}' AND SECTOR_ID = {SectorID} AND CONVERT(DATE,STARTED_AT) >= '{DataAtual}'");
            //                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
            //                        {
            //                            using (SqlDataReader reader = command.ExecuteReader())
            //                            {
            //                                if (reader.Read())
            //                                {
            //                                   // Temo vingencia no periodo do mes do indicador nesse setor
            //                                    VerificaDataVigencia = reader["ID"].ToString();
            //                                    QuebraDiaSeguinte = true;
            //                                }
            //                            }
            //                        }
            //                    }
            //                    catch (Exception)
            //                    {
            //                        throw;
            //                    }
            //                    connection.Close();

            //                }

            //                    using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            //                    {
            //                    try
            //                    {
            //                        //UPDATE SCORE QUE ESTÁ EM VINGENCIA.
            //                        connection.Open();
            //                        StringBuilder stb = new StringBuilder();
            //                        stb.Append("UPDATE GDA_HISTORY_SCORE_INDICATOR_SECTOR SET ");
            //                        stb.AppendFormat("ENDED_AT = '{0}' ", DataAtual);
            //                        stb.AppendFormat("WHERE ID = {0}  ",VerificaDataVigencia);

            //                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
            //                        {
            //                            command.ExecuteNonQuery();
            //                        }

            //                        string UltimoDiaMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)).ToString("yyyy-MM-dd");

            //                        //INSERIR SCORE NOVO COM SUA VIGENCIA ATUALIZADA.
            //                        string DataSeguinteAtual = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");

            //                        double scoreValue = double.Parse(Score);
            //                        // Multiplicar por 100
            //                        double weightScore = scoreValue / 100;
            //                        StringBuilder stb2 = new StringBuilder();
            //                        stb2.Append("INSERT GDA_HISTORY_SCORE_INDICATOR_SECTOR (INDICATOR_ID, SECTOR_ID, CREATED_AT, WEIGHT_SCORE, ALTERED_BY, ENDED_AT, STARTED_AT) VALUES ( ");
            //                        stb2.AppendFormat(" '{0}', ", IndicadorID);
            //                        stb2.AppendFormat(" '{0}', ", SectorID);
            //                        stb2.AppendFormat(" GETDATE(), ");
            //                        stb2.AppendFormat(" '{0}', ", weightScore);
            //                        stb2.AppendFormat(" '{0}', ", matricula);
            //                        stb2.AppendFormat(" '{0}',", UltimoDiaMes);
            //                        stb2.AppendFormat(" '{0}') ", DataSeguinteAtual);

            //                        using (SqlCommand command = new SqlCommand(stb2.ToString(), connection))
            //                        {
            //                            command.ExecuteNonQuery();
            //                        }
            //                    }
            //                    catch (Exception)
            //                    {
            //                        throw;
            //                    }
            //                    connection.Close();
            //                }
            //            }
            //            else
            //            {
            //                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            //                {
            //                    try
            //                    {
            //                        string UltimoDiaMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)).ToString("yyyy-MM-dd");

            //                        //INSERT SCORE
            //                        connection.Open();
            //                        StringBuilder stb = new StringBuilder();
            //                        stb.Append("INSERT GDA_HISTORY_SCORE_INDICATOR_SECTOR (INDICATOR_ID, SECTOR_ID, CREATED_AT, WEIGHT_SCORE, ALTERED_BY, ENDED_AT, STARTED_AT) VALUES ( ");
            //                        stb.AppendFormat("INDICATOR_ID = '{0}', ", IndicadorID);
            //                        stb.AppendFormat("SECTOR_ID = '{0}', ", SectorID);
            //                        stb.AppendFormat("CREATED_AT = GETDATE(), ");
            //                        stb.AppendFormat("WEIGHT_SCORE = '{0}', ", Score);
            //                        stb.AppendFormat("ALTERED_BY = '{0}', ", matricula);
            //                        stb.AppendFormat("ENDED_AT = '{0}',", UltimoDiaMes);
            //                        stb.AppendFormat("STARTED_AT = '{0}') ", DataAtual);

            //                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
            //                        {
            //                            command.ExecuteNonQuery();
            //                        }
            //                    }
            //                    catch (Exception)
            //                    {
            //                        throw;
            //                    }
            //                    connection.Close();
            //                }
            //            }
            //        }

            //    }
            //    if (qtdNaoEncontrado > 0 && qtdTotal > qtdNaoEncontrado)
            //    {
            //        return Ok($"importação score concluída com sucesso. Não foram encontrados {qtdNaoEncontrado} score para importação.");

            //    }
            //    else if (qtdNaoEncontrado == 0)
            //    {
            //        return Ok("importação massiva de score concluída com sucesso.");
            //    }
            //    else
            //    {
            //        return BadRequest("indicador de score não encontrados.");
            //    }

            //}
            //else
            //{
            //    return BadRequest("Arquivo não enviado.");
            //}
        }
        // Método para serializar um DataTable em JSON
    }
}