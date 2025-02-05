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
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class listImageController : ApiController
    {// POST: api/Results
        [HttpGet]
        public async Task<HttpResponseMessage> GetImage(string fileName)
        {
            // Conexão com o Azure Blob Storage
            BlobServiceClient blobServiceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=stthprdguildaxferbsouth;AccountKey=Eirvcvk+eQ+GaCOYmBRB9CrJPMmae8LsuFlsG/GUwJLsCcEPekoT7yDbDjiKHmOomlg07DRB+pc/+AStwEOfQA==;EndpointSuffix=core.windows.net");
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("guilda");
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            if (await blobClient.ExistsAsync())
            {
                var blobProperties = await blobClient.GetPropertiesAsync();
                var totalLength = blobProperties.Value.ContentLength;
                var contentType = blobProperties.Value.ContentType;

                HttpRequestHeaders headers = Request.Headers;
                HttpResponseMessage response;

                // Suporte para Range Request (vídeos, por exemplo)
                if (headers.Range != null && headers.Range.Ranges.Count > 0)
                {
                    // Obtém a posição de início e fim do Range
                    var range = headers.Range.Ranges.First();
                    long start = range.From ?? 0;
                    long end = range.To ?? totalLength - 1;

                    // Define o tamanho do conteúdo a ser enviado
                    long contentLength = end - start + 1;

                    // Configura a resposta HTTP para suporte a Range Requests
                    response = new HttpResponseMessage(HttpStatusCode.PartialContent);
                    response.Content = new StreamContent(await blobClient.OpenReadAsync(new Azure.Storage.Blobs.Models.BlobOpenReadOptions(false) { Position = start }));
                    response.Content.Headers.ContentLength = contentLength;
                    response.Content.Headers.ContentRange = new System.Net.Http.Headers.ContentRangeHeaderValue(start, end, totalLength);
                }
                else
                {
                    // Se não houver Range Request, retorna o arquivo completo
                    var blobDownloadInfo = await blobClient.DownloadAsync();
                    response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StreamContent(blobDownloadInfo.Value.Content)
                    };
                    response.Content.Headers.ContentLength = totalLength;
                }

                // Define o tipo de conteúdo (MIME type) para vídeos/imagens
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

                // Adiciona cache control para evitar problemas com carregamento de mídia
                response.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
                {
                    Public = true,
                    MaxAge = TimeSpan.FromMinutes(10)
                };

                HttpContext.Current.Response.BufferOutput = false;

                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Blob {fileName} not found.");
            }
        }
        //public async Task<HttpResponseMessage> GetImage(string fileName)
        //{

        //    BlobServiceClient blobServiceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=stthprdguildaxferbsouth;AccountKey=Eirvcvk+eQ+GaCOYmBRB9CrJPMmae8LsuFlsG/GUwJLsCcEPekoT7yDbDjiKHmOomlg07DRB+pc/+AStwEOfQA==;EndpointSuffix=core.windows.net");
        //    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("guilda");
        //    BlobClient blobClient = containerClient.GetBlobClient(fileName);

        //    if (await blobClient.ExistsAsync())
        //    {
        //        var blobDownloadInfo = await blobClient.DownloadAsync();
        //        var content = blobDownloadInfo.Value.Content;
        //        var contentType = blobDownloadInfo.Value.ContentType;

        //        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
        //        response.Content = new StreamContent(content);
        //        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
        //        response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
        //        {
        //            FileName = fileName
        //        };

        //        return response;
        //    }
        //    else
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Blob {fileName} not found.");
        //    }
        //}

    }

}