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
using OfficeOpenXml;
using Utilities;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ProfileAssociationController : ApiController
    {
        [HttpPost]
        [ResponseType(typeof(ResultsModel))]
        public IHttpActionResult PostResultsModel()
        {
            HttpPostedFile arquivo = HttpContext.Current.Request.Files["FILE"];
            
            if (arquivo != null && arquivo.ContentLength > 0)
            {
                using (ExcelPackage package = new ExcelPackage(arquivo.InputStream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    int rowCount = worksheet.Dimension.Rows;
                    for (int row = 2; row <= rowCount; row++)
                    {
                        string perfil = worksheet.Cells[row, 1].Value.ToString();
                        string idcolaborador = worksheet.Cells[row, 2].Value.ToString();
                        //string nomecolaborador = worksheet.Cells[row, 3].Value.ToString();

                        using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                        {
                            string commandText = $"UPDATE GDA_COLLABORATORS SET PROFILE_COLLABORATOR_ADMINISTRATIONID = (SELECT ID FROM GDA_PROFILE_COLLABORATOR_ADMINISTRATION WHERE NAME = '{perfil}') WHERE IDGDA_COLLABORATORS = '{idcolaborador.Replace("BC", "")}'";
                            connection.Open();
                            using (SqlCommand command = new SqlCommand(commandText, connection))
                            {
                                command.ExecuteNonQuery();
                            }
                            connection.Close();
                        }

                    }
                }
                return Ok("Associação massiva concluída com sucesso.");
            }
            else
            {
                return BadRequest("Arquivo não enviado.");
            }
        }
    }
}