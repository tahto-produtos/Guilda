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
using Iced.Intel;
using ApiC.Class.DowloadFile;
using Microsoft.AspNetCore.Mvc;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class DownloadController : ApiController
    {

        [HttpGet]
        public IHttpActionResult GetResultsModel(string rel, string month, string year)
        {
            string NameFile = $"Relatorio_{rel} {month}-{year}";

            List<GalleryResponseModel> files = BucketClass.ListGalleryFiles(NameFile);

            return Ok(files);
        }

    }
}