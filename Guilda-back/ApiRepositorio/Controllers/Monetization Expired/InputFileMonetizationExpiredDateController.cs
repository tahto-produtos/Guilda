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
using Microsoft.Ajax.Utilities;
using DocumentFormat.OpenXml.Office2010.Excel;
using static TokenService;


namespace ApiRepositorio.Controllers
{
    public class InputFileMonetizationExpiredDateController : ApiController
    {

        public class ResponseInputMassiveMonetizationExpired
        {
            public List<string> success { get; set; }

            public List<string> failed { get; set; }
        }

        public class MonetizationExpiredInputModel
        {
            public string SetorSite { get; set; }
            public string CodGipSite { get; set; }
            public int ExpireDays { get; set; }
            public int Status { get; set; }
        }

        [HttpPost]
        [ResponseType(typeof(ResponseInputMassiveMonetizationExpired))]
        public IHttpActionResult InputMassive()
        {

            int collaboratorId = 0;
            int personauserId = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            collaboratorId = inf.collaboratorId;
            personauserId = inf.personauserId;

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
                        return Ok(this.ImportConfigMonetization(worksheet, collaboratorId));
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

        private ResponseInputMassiveMonetizationExpired ImportConfigMonetization(ExcelWorksheet worksheet, int collaboratorId)
        {
            List<string> camSuccess = new List<string>();
            List<string> camFailed = new List<string>();
            int rowCount = worksheet.Dimension.Rows;
            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    ExcelRange cells = worksheet.Cells;

                    if (cells[row, 1].Value == null)
                    {
                        continue;
                    }
                    //VALIDA DADOS DA LINHA
                    var blE = this.ValidadeDataMC(cells, row);

                    if (blE)
                    {
                        //RECUPERA OS DADOS DA LINHA NAS VARIÁVEIS
                        string SetorSite = cells[row, 1].Value.ToString();
                        string CodGipSite = cells[row, 2].Value.ToString();
                        int ExpireDays = Convert.ToInt32(cells[row, 3].Value.ToString());
                        int status = Convert.ToInt32(cells[row, 4].Value.ToString());

                        int IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE = 0;
                        int ID_REFERER = 0;

                        var bl = new MonetizationExpiredInputModel();
                        bl.SetorSite = SetorSite;
                        bl.CodGipSite = CodGipSite;
                        bl.ExpireDays = ExpireDays;


                        if (SetorSite == "Setor")
                        {
                            IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE = 1;
                            ID_REFERER = Convert.ToInt32(CodGipSite);
                        }
                        else if (SetorSite == "Site")
                        {
                            IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE = 2;
                            ID_REFERER = Convert.ToInt32(this.verifyCodeSite(CodGipSite));
                            if (ID_REFERER == 0)
                            {
                                camFailed.Add("");
                                continue;
                            }
                        }
                        else
                        {
                            camFailed.Add("");
                            continue;
                        }



                        var success = this.verifyAndAddMC(SetorSite, CodGipSite, ExpireDays, IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE, ID_REFERER, status, collaboratorId);


                        if (success)
                        {
                            camSuccess.Add("");
                        }
                        else
                        {
                            camFailed.Add("");
                        }
                    }
                }
                catch (Exception ex)
                {                   
                    var exception = ex.Message;
                    camFailed.Add(ex.Message);
                }
            }

            var response = new ResponseInputMassiveMonetizationExpired();
            response.success = camSuccess;
            response.failed = camFailed;
            return response;
        }

        private bool ValidadeDataMC(ExcelRange cells, int rowNumber)
        {
            var setorSite = cells[rowNumber, 1];
            try
            {
                setorSite.Value.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Setor/Site não definda");
            }

            var CodGipSite = cells[rowNumber, 2];
            try
            {
                CodGipSite.Value.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("CodGip/Site não definda");
            }




            var days = cells[rowNumber, 3];
            int numberTest = 0;
            if (!int.TryParse(days.Value.ToString(), out numberTest))
            {
                throw new Exception("Dias a expirar não é um número válido");
            }

            if (cells[rowNumber, 1].Value.ToString() == "Setor" && !int.TryParse(cells[rowNumber, 2].Value.ToString(), out numberTest))
            {
                throw new Exception("CodGip não é um número válido");
            }

            var status = cells[rowNumber, 4];
            try
            {
                status.Value.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Status não defindo");
            }


            return true;
        }

        public int verifyCodeSite(string site)
        {
            int retorno = 0;
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {

                    StringBuilder stb = new StringBuilder();
                    stb.AppendFormat($"SELECT IDGDA_SITE FROM GDA_SITE (NOLOCK) ");
                    stb.AppendFormat($"WHERE SITE = '{site}' ");
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            retorno = Convert.ToInt32(reader["IDGDA_SITE"]);
                        }
                    }
                }
                catch (Exception)
                {
                    return 0;
                }
                connection.Close();

                return retorno;
            }
        }

        private bool verifyAndAddMC(string SetorSite, string CodGipSite, int ExpireDays, int IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE, int ID_REFERER, int status, int collaboratorId)
        {
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {

                    StringBuilder stb = new StringBuilder();
                    stb.AppendFormat($"UPDATE GDA_MONETIZATION_CONFIG SET DELETED_AT = GETDATE() ");
                    stb.AppendFormat($"WHERE DELETED_AT IS NULL ");
                    stb.AppendFormat($"AND IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE = {IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE} ");
                    stb.AppendFormat($"AND ID_REFERER = {ID_REFERER} ");
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    if (status == 1)
                    {
                        //InsertConfigMonetization.Append($"('{DAYS}', GETDATE(), '{COLLABORATORID}', '{STARTED_AT}', '0', '{PAST_DATE}', '{filterType}', '{referer}' )  ");
                        stb.Clear();
                        stb.AppendFormat($"INSERT INTO GDA_MONETIZATION_CONFIG (DAYS, CREATED_AT, CREATED_BY, DELETED_AT, REPROCESSED, STARTED_AT, PAST_DATE, IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE, ID_REFERER) VALUES (");
                        stb.AppendFormat($"'{ExpireDays}', "); //DAYS
                        stb.AppendFormat($"GETDATE(), "); //CREATED_AT
                        stb.AppendFormat($"{collaboratorId}, "); //CREATED_BY
                        stb.AppendFormat($"NULL, "); //DELETED_AT
                        stb.AppendFormat($"0, "); //REPROCESSED
                        stb.AppendFormat($"GETDATE(), "); //STARTED_AT
                        stb.AppendFormat($"0, "); //PAST_DATE
                        stb.AppendFormat($"{IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE}, "); //IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE
                        stb.AppendFormat($"{ID_REFERER} "); //ID_REFERER
                        stb.AppendFormat($") ");

                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception)
                {
                    return false;
                }
                connection.Close();

                return true;
            }
        }
    }
}