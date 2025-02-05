using Azure.Storage.Blobs;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace ApiC.Controllers.Download
{
    public class DownloadFileController : ApiController
    {
        private readonly FileService _fileService;

        public DownloadFileController()
        {
            _fileService = new FileService();
        }

        [HttpGet]
        public async Task<IHttpActionResult> DownloadFile(string fileName)
        {
            var (content, contentType, fileDownloadName) = await _fileService.GetFileAsync(fileName);

            if (content != null)
            {
                return new FileResult(content, contentType, fileDownloadName);
            }

            return NotFound();
        }
    }

    public class FileService
    {
        private readonly string _blobConnectionString = "DefaultEndpointsProtocol=https;AccountName=stthprdguildaxferbsouth;AccountKey=Eirvcvk+eQ+GaCOYmBRB9CrJPMmae8LsuFlsG/GUwJLsCcEPekoT7yDbDjiKHmOomlg07DRB+pc/+AStwEOfQA==;EndpointSuffix=core.windows.net";
        private readonly string _containerName = "guilda";

        public async Task<(Stream Content, string ContentType, string FileName)> GetFileAsync(string fileName)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_blobConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            if (await blobClient.ExistsAsync())
            {
                var blobDownloadInfo = await blobClient.DownloadAsync();
                var content = blobDownloadInfo.Value.Content;
                var contentType = blobDownloadInfo.Value.ContentType;

                return (content, contentType, fileName);
            }
            else
            {
                return (null, null, null);
            }
        }
    }

    public class FileResult : IHttpActionResult
    {
        private readonly Stream _content;
        private readonly string _contentType;
        private readonly string _fileName;

        public FileResult(Stream content, string contentType, string fileName)
        {
            _content = content;
            _contentType = contentType;
            _fileName = fileName;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(_content)
            };

            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(_contentType);
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = _fileName
            };

            return await Task.FromResult(response);
        }
    }
}
