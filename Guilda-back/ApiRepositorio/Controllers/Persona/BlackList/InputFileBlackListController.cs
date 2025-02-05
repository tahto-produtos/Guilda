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
using static TokenService;


namespace ApiRepositorio.Controllers
{
    public class InputFileBlackListController : ApiController
    {

        public class ResponseInputMassiveBlackList
        {
            public List<BlackListInputModel> success { get; set; }

            public List<BlackListInputModel> failed { get; set; }
        }

        public class BlackListInputModel
        {
            public string word { get; set; }
        }

        [HttpPost]
        [ResponseType(typeof(ResponseInputMassiveBlackList))]
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
                        return Ok(this.ImportBlacklist(worksheet, collaboratorId));
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

        private ResponseInputMassiveBlackList ImportBlacklist(ExcelWorksheet worksheet, int collaboratorId)
        {
            List<BlackListInputModel> camSuccess = new List<BlackListInputModel>();
            List<BlackListInputModel> camFailed = new List<BlackListInputModel>();
            int rowCount = worksheet.Dimension.Rows;
            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    ExcelRange cells = worksheet.Cells;

                    //VALIDA DADOS DA LINHA
                    var blE = this.ValidadeDataBL(cells, row);

                    if (blE)
                    {
                        //RECUPERA OS DADOS DA LINHA NAS VARIÁVEIS
                        var word = cells[row, 1].Value.ToString();

                        var success = this.verifyAndAddBL(word, collaboratorId);

                        var bl = new BlackListInputModel();
                        bl.word = word;

                        if (success)
                        {
                            camSuccess.Add(bl);
                        }
                        else
                        {
                            camFailed.Add(bl);
                        }
                    }
                }
                catch (Exception ex)
                {
                    var exception = ex.Message;
                }
            }

            var response = new ResponseInputMassiveBlackList();
            response.success = camSuccess;
            response.failed = camFailed;
            return response;
        }

        private bool ValidadeDataBL(ExcelRange cells, int rowNumber)
        {
            var word = cells[rowNumber, 1];
            try
            {
                word.Value.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Palavra não definda");
            }

            return true;
        }

        private bool verifyAndAddBL(string word, int collaboratorId)
        {
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    bool existWord = false;
                    StringBuilder stb = new StringBuilder();
                    stb.AppendFormat($"SELECT IDGDA_PERSONA_BLACKLIST FROM GDA_PERSONA_BLACKLIST (NOLOCK) ");
                    stb.AppendFormat($"WHERE DELETED_AT IS NULL AND WORD = '{word}' ");
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                existWord = true;
                            }
                        }
                    }

                    if (existWord == false)
                    {
                        stb.Clear();
                        stb.AppendFormat($"INSERT INTO GDA_PERSONA_BLACKLIST (WORD, CREATED_AT, DELETED_AT, CREATED_BY, DELETED_BY) VALUES ( ");
                        stb.AppendFormat($"'{word}', "); //WORD
                        stb.AppendFormat($"GETDATE(), "); //CREATED_AT
                        stb.AppendFormat($"NULL, "); //DELETED_AT
                        stb.AppendFormat($"{collaboratorId}, "); //CREATED_BY
                        stb.AppendFormat($"NULL "); //DELETED_BY
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