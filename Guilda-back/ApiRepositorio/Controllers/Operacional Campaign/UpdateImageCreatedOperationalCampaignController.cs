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
using System.Net.NetworkInformation;
using System.Web.UI;
using System.Xml.Linq;
using CommandLine;
using DocumentFormat.OpenXml.Spreadsheet;
using static ApiRepositorio.Controllers.FinancialSummaryController;
using static ApiRepositorio.Controllers.SearchAccountsController;
using static ApiRepositorio.Controllers.SendNotificationController;
using DocumentFormat.OpenXml.Wordprocessing;
using static ApiRepositorio.Controllers.CreatedNotificationController;
using Antlr.Runtime.Misc;
using static ApiRepositorio.Controllers.LoadMyNotificationController;
using OfficeOpenXml.ConditionalFormatting;
using static ClosedXML.Excel.XLPredefinedFormat;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class UpdateImageCreatedOperationalCampaignController : ApiController
    {// POST: api/Results

        public class inputImageCreateImageOperaiontalCampaign
        {
            public int ID_OPERATIONAL_CAMPAIGN { get; set; }

        }

        [HttpPost]
        public IHttpActionResult PostResultsModel()
        {
            int collaboratorId = 0;
            int personaid = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            collaboratorId = inf.collaboratorId;
            personaid = inf.personauserId;

            int ID_OPERATIONAL_CAMPAIGN = Convert.ToInt32(System.Web.HttpContext.Current.Request.Form["ID_OPERATIONAL_CAMPAIGN"]);

            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            List<GalleryResponseModel> pictures = PictureClass.UploadFilesToBlob(files);

            foreach (GalleryResponseModel item in pictures)
            {
                //Insiro na tabela de GDA_PERSONA_POSTS_FILES
                BancoImageCreatedOperationalCampaign.insertFiles(item.url, ID_OPERATIONAL_CAMPAIGN);
            }

            return Ok("Imagem alterada com sucesso.");
        }
        // Método para serializar um DataTable em JSON


        #region Banco

        public class BancoImageCreatedOperationalCampaign
        {
            public static void insertFiles(string url, int codCampaign)
            {

                StringBuilder stb = new StringBuilder();
                stb.Append("UPDATE GDA_OPERATIONAL_CAMPAIGN ");
                stb.Append($"SET IMAGE = '{url}' ");
                stb.Append($"WHERE IDGDA_OPERATIONAL_CAMPAIGN = {codCampaign} ");


                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception)
                    {

                    }
                    connection.Close();
                }
            }
        }
        #endregion

    }



}