using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ApiC.Controllers.Download
{
    public class DownloadFileSASController : ApiController
    {


        [HttpGet]
        public IHttpActionResult GetSasUrl(string fileName)
        {
            try
            {

                string blobConnectionString = "DefaultEndpointsProtocol=https;AccountName=stthprdguildaxferbsouth;AccountKey=Eirvcvk+eQ+GaCOYmBRB9CrJPMmae8LsuFlsG/GUwJLsCcEPekoT7yDbDjiKHmOomlg07DRB+pc/+AStwEOfQA==;EndpointSuffix=core.windows.net";
                string containerName = "guilda";

                BlobServiceClient blobServiceClient = new BlobServiceClient(blobConnectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                BlobClient blobClient = containerClient.GetBlobClient(fileName);

                if (blobClient.Exists())
                {
                    var sasUri = GenerateSasUri(blobClient);
                    return Ok(sasUri.ToString());
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private Uri GenerateSasUri(BlobClient blobClient)
        {
            BlobSasBuilder sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = blobClient.BlobContainerName,
                BlobName = blobClient.Name,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            // Adicione o cabeçalho Content-Disposition para forçar o download
            sasBuilder.ContentDisposition = $"attachment; filename={blobClient.Name}";

            Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
            return sasUri;
        }
    }


}