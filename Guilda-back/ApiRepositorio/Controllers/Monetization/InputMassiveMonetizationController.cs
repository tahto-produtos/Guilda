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
using static ApiRepositorio.Controllers.InputMassiveMetricController;
using System.Linq;


namespace ApiRepositorio.Controllers
{
    public class InputMassiveMonetizationController : ApiController
    {
        [HttpPost]
        [ResponseType(typeof(ResponseInputMassiveInputModel))]
        public IHttpActionResult InputMassive()
        {
            HttpPostedFile arquivo = HttpContext.Current.Request.Files["FILE"];

            int qtdNaoEncontrado = 0;
            int qtdTotal = 0;

            if (arquivo != null && arquivo.ContentLength > 0)
            {
                using (ExcelPackage package = new ExcelPackage(arquivo.InputStream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    try
                    {
                        return Ok(this.ImportSingleCredit(worksheet));
                    }
                    catch (Exception ex)
                    {

                    }

                }
            }
            else
            {
                return BadRequest("Arquivo não enviado.");
            }

            return Ok($"Importação concluída com sucesso!");
        }

        private ProcessResult ImportSingleCredit(ExcelWorksheet worksheet)
        {
            List<FailedRow> faileds = new List<FailedRow>();
            var createdIndicatorSectors = new List<string>();
            var updatedIndicatorSectors = new List<string>();


            int rowCount = worksheet.Dimension.Rows;

            var usedRows = worksheet.Cells
.Where(cell => !string.IsNullOrWhiteSpace(cell.Text))
.Select(cell => cell.Start.Row)
.Distinct();

            for (int row = 2; row <= usedRows.Count(); row++)
            {


                try
                {
                    ExcelRange cells = worksheet.Cells;

                    //VALIDA DADOS DA LINHA

                    FailedRow failed = this.ValidadeDataCredit(cells, row);

                    if (failed.FailedReason != null)
                    {
                        faileds.Add(failed);
                    }
                }
                catch (Exception ex)
                {
                    var exception = ex.Message;
                }
            }

            if (faileds.Count > 0)
            {
                ProcessResult pr = new ProcessResult();
                pr.totalUpdated = updatedIndicatorSectors.Count;
                pr.totalCreated = createdIndicatorSectors.Count;
                pr.creates = createdIndicatorSectors;
                pr.updates = updatedIndicatorSectors;
                pr.failed = faileds.Select(f => f.FailedReason).ToList();

                return pr;
            }


            for (int row = 2; row <= usedRows.Count(); row++)
            {
                try
                {
                    ExcelRange cells = worksheet.Cells;

                    //RECUPERA OS DADOS DA LINHA NAS VARIÁVEIS
                    var bc = cells[row, 1].Value.ToString();
                    var operation = cells[row, 5].Value.ToString();
                    var coins = cells[row, 6].Value.ToString();
                    var reason = cells[row, 7].Value.ToString();

                    var observation = cells[row, 8].Value != null ? cells[row, 8].Value.ToString() : "";

                    var success = this.AddCredit(bc, operation, coins, reason, observation);

                    int coinsInt = Int32.Parse(coins);
                    var ca = new CheckingAccountModel();
                    ca.Collaborator_Id = bc;
                    ca.Input = operation.ToUpper() == "CRÉDITO" ? coinsInt : 0;
                    ca.Output = operation.ToUpper() == "DÉBITO" ? coinsInt : 0;
                    ca.Observation = observation;
                    ca.Reason = reason;

                    if (success)
                    {
                        createdIndicatorSectors.Add(ca.Collaborator_Id);
                    }
                }
                catch (Exception ex)
                {
                    var exception = ex.Message;
                }
            }

            ProcessResult pr2 = new ProcessResult();
            pr2.totalUpdated = updatedIndicatorSectors.Count;
            pr2.totalCreated = createdIndicatorSectors.Count;
            pr2.creates = createdIndicatorSectors;
            pr2.updates = updatedIndicatorSectors;
            pr2.failed = faileds.Select(f => f.FailedReason).ToList();

            return pr2;

        }

        private FailedRow  ValidadeDataCredit(ExcelRange cells, int rowNumber)
        {

            FailedRow faileds = new FailedRow();
            var bc = cells[rowNumber, 1];
            try
            {
                bc.Value.ToString();
            }
            catch (Exception ex)
            {
                faileds = (new FailedRow
                {
                    FailedReason = $"BC do colaborador não definda. Linha: {rowNumber}.",
                    Row = null
                });

                return faileds;
            }

            var operation = cells[rowNumber, 5];
            try
            {
                operation.Value.ToString();
            }
            catch (Exception ex)
            {
                faileds = (new FailedRow
                {
                    FailedReason = $"Tipo da operação não definada. Linha: {rowNumber}.",
                    Row = null
                });

                return faileds;
            }

            var coins = cells[rowNumber, 6];
            try
            {
                coins.Value.ToString();
            }
            catch (Exception ex)
            {
                faileds = (new FailedRow
                {
                    FailedReason = $"Quantidade de moedas não definida. Linha: {rowNumber}.",
                    Row = null
                });

                return faileds;
            }

            int numberTest = 0;
            if (!int.TryParse(coins.Value.ToString(), out numberTest))
            {
                faileds = (new FailedRow
                {
                    FailedReason = $"Moedas não é um número válido. Linha: {rowNumber}.",
                    Row = null
                });

                return faileds;

            }

            return faileds;
        }

        private bool AddCredit(string bc, string operation, string coins, string reason, string observation)
        {
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    CheckingAccountModel lCam = returnTables.LastLineCheckingAccount(bc.Replace("BC", ""));

                    StringBuilder stb = new StringBuilder();
                    int coinsInt = Int32.Parse(coins);
                    if (operation.ToUpper() == "CRÉDITO")
                    {
                        stb.Append("INSERT INTO GDA_CHECKING_ACCOUNT (input, output, balance, collaborator_id, reason, observation, created_at, result_date) VALUES ( ");
                        stb.AppendFormat("'{0}', ", coinsInt);
                        stb.AppendFormat("'{0}', ", 0);
                        stb.AppendFormat("{0}, ", (lCam != null) ? (lCam.Balance + coinsInt) : coinsInt);
                        stb.AppendFormat("'{0}', ", bc.Replace("BC", ""));
                        stb.AppendFormat("'{0}', ", reason);
                        stb.AppendFormat("'{0}',", observation);
                        stb.AppendFormat("'{0}',", DateTime.Now.AddHours(3).ToString("yyyy-MM-dd H:m:s"));
                        stb.AppendFormat("'{0}')", DateTime.Now.AddHours(3).ToString("yyyy-MM-dd"));
                    }
                    else if (operation.ToUpper() == "DÉBITO")
                    {
                        stb.Append("INSERT INTO GDA_CHECKING_ACCOUNT (input, output, balance, collaborator_id, reason, observation, created_at, result_date) VALUES ( ");
                        stb.AppendFormat("'{0}', ", 0);
                        stb.AppendFormat("'{0}', ", coinsInt);
                        stb.AppendFormat("{0}, ", (lCam != null) ? (lCam.Balance - coinsInt) : coinsInt);
                        stb.AppendFormat("'{0}', ", bc.Replace("BC", ""));
                        stb.AppendFormat("'{0}', ", reason);
                        stb.AppendFormat("'{0}',", observation);
                        stb.AppendFormat("'{0}',", DateTime.Now.AddHours(3).ToString("yyyy-MM-dd H:m:s"));
                        stb.AppendFormat("'{0}')", DateTime.Now.AddHours(3).ToString("yyyy-MM-dd"));
                    }

                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                    return true;
                }
                catch (Exception)
                {
                    throw;
                }
                connection.Close();
            }
        }
    }
}