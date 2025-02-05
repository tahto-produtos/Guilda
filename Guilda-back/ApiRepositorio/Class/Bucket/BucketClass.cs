using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using ApiRepositorio.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BCrypt.Net;
using HeyRed.Mime;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Windows.Media.Animation;
using Utilities;

namespace ApiC.Class
{
    public class BucketClass
    {
        private const string bucketName = "focare-public";
        private const string folderName = "public/";
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;


        //public static List<GalleryResponseModel> UploadToAWSS3(byte[] fileBytes, string fileName)
        //{
        //    List<GalleryResponseModel> filesUploads = new List<GalleryResponseModel>();
        //    s3Client = new AmazonS3Client("AKIAQ5UGFJSENBSKKGM3", "juBFm1d52BrmuRqwSRCwyzU1Y9gItY/NdOS/NM2V", bucketRegion);

        //    //string fileNameInS3 = Guid.NewGuid() + Path.GetExtension(fileName);
        //    string fileNameInS3 = fileName;


        //    var uploadRequest = new TransferUtilityUploadRequest
        //    {
        //        InputStream = new MemoryStream(fileBytes),
        //        Key = folderName + fileNameInS3,
        //        BucketName = bucketName
        //        // CannedACL = S3CannedACL.PublicRead
        //    };

        //    using (var transferUtility = new TransferUtility(s3Client))
        //    {
        //        transferUtility.Upload(uploadRequest);
        //    }

        //    string fileUrl = $"https://{bucketName}.s3.amazonaws.com/{folderName}{fileNameInS3}";

        //    // Obter o tipo MIME com base na extensão do arquivo

        //    string contentType = MimeTypesMap.GetMimeType(Path.GetExtension(fileName));

        //    using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
        //    {
        //        connection.Open();
        //        try
        //        {
        //            StringBuilder stb = new StringBuilder();
        //            stb.Append("INSERT INTO GDA_UPLOADS (originalName, [key], type, url, idgda_uploads_category, created_at) VALUES ( ");
        //            stb.AppendFormat("'{0}', ", fileName);
        //            stb.AppendFormat("'{0}', ", fileNameInS3);
        //            stb.AppendFormat("'{0}', ", contentType);
        //            stb.AppendFormat("'{0}', ", fileUrl);
        //            stb.AppendFormat("2,"); //Tipo 2 é tipo relatorio na tabela de uploads
        //            stb.AppendFormat("'{0}');", DateTime.Now.ToString("yyyy-MM-dd H:m:s"));
        //            stb.Append("SELECT SCOPE_IDENTITY()");

        //            using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
        //            {
        //                int newId = Convert.ToInt32(command.ExecuteScalar());
        //                GalleryResponseModel gl = new GalleryResponseModel
        //                {
        //                    id = newId,
        //                    originalName = fileName,
        //                    key = fileNameInS3,
        //                    type = contentType,
        //                    url = fileUrl,
        //                    created_at = DateTime.Now.ToString("yyyy-MM-dd H:m:s")
        //                };
        //                filesUploads.Add(gl);
        //            }
        //        }
        //        catch (Exception)
        //        {
        //        }
        //    }

        //    return filesUploads;
        //}

        public static List<GalleryResponseModel> UploadToAzureBlob(byte[] fileBytes, string fileName)
        {
            List<GalleryResponseModel> filesUploads = new List<GalleryResponseModel>();

            // Define o nome do arquivo no Blob
            string fileNameInBlob = fileName;
            string blobConnectionString = "DefaultEndpointsProtocol=https;AccountName=stthprdguildaxferbsouth;AccountKey=Eirvcvk+eQ+GaCOYmBRB9CrJPMmae8LsuFlsG/GUwJLsCcEPekoT7yDbDjiKHmOomlg07DRB+pc/+AStwEOfQA==;EndpointSuffix=core.windows.net";
            string containerName = "guilda"; // Substitua pelo nome do seu container

            // Cria um cliente do BlobService
            BlobServiceClient blobServiceClient = new BlobServiceClient(blobConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Cria um novo cliente de Blob para o arquivo
            BlobClient blobClient = containerClient.GetBlobClient(fileNameInBlob);

            using (var stream = new MemoryStream(fileBytes))
            {
                blobClient.Upload(stream, new BlobHttpHeaders { ContentType = MimeTypesMap.GetMimeType(Path.GetExtension(fileName)) });
            }

            string fileUrl = blobClient.Uri.ToString();

            // Obter o tipo MIME com base na extensão do arquivo
            string contentType = MimeTypesMap.GetMimeType(Path.GetExtension(fileName));

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    StringBuilder stb = new StringBuilder();
                    stb.Append("INSERT INTO GDA_UPLOADS (originalName, [key], type, url, idgda_uploads_category, created_at) VALUES ( ");
                    stb.AppendFormat("'{0}', ", fileName);
                    stb.AppendFormat("'{0}', ", fileNameInBlob);
                    stb.AppendFormat("'{0}', ", contentType);
                    stb.AppendFormat("'{0}', ", fileUrl);
                    stb.AppendFormat("2,"); //Tipo 2 é tipo relatorio na tabela de uploads
                    stb.AppendFormat("'{0}');", DateTime.Now.ToString("yyyy-MM-dd H:m:s"));
                    stb.Append("SELECT SCOPE_IDENTITY()");

                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        int newId = Convert.ToInt32(command.ExecuteScalar());
                        GalleryResponseModel gl = new GalleryResponseModel
                        {
                            id = newId,
                            originalName = fileName,
                            key = fileNameInBlob,
                            type = contentType,
                            url = fileUrl,
                            created_at = DateTime.Now.ToString("yyyy-MM-dd H:m:s")
                        };
                        filesUploads.Add(gl);
                    }
                }
                catch (Exception)
                {
                }
            }

            return filesUploads;
        }



        public static List<GalleryResponseModel> ListGalleryFiles(string nomeArquivo)
        {
            List<GalleryResponseModel> files = new List<GalleryResponseModel>();

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" SELECT * FROM GDA_UPLOADS (NOLOCK) WHERE ORIGINALNAME LIKE '%{0}%' ", nomeArquivo);

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                GalleryResponseModel file = new GalleryResponseModel();

                                var id = reader["id"].ToString();
                                var originalName = reader["originalName"].ToString();
                                var key = reader["key"].ToString();
                                var typeBd = reader["type"].ToString();
                                var url = reader["url"].ToString();
                                var createdAt = reader["created_at"].ToString();

                                file.id = Int32.Parse(id);
                                file.originalName = originalName;
                                file.key = key;
                                file.type = typeBd;
                                file.url = url;
                                file.created_at = createdAt;

                                files.Add(file);
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return files;
        }
        private static readonly string _blobConnectionString = "DefaultEndpointsProtocol=https;AccountName=stthprdguildaxferbsouth;AccountKey=Eirvcvk+eQ+GaCOYmBRB9CrJPMmae8LsuFlsG/GUwJLsCcEPekoT7yDbDjiKHmOomlg07DRB+pc/+AStwEOfQA==;EndpointSuffix=core.windows.net";
        private static readonly string _containerName = "guilda";

        public static async Task<(Stream Content, string ContentType, string FileName)> GetFileAsync(string fileName)
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

        public static bool DeleteFiles(string nomeArquivo)
        {


            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {


                    StringBuilder selectFiles = new StringBuilder();
                    selectFiles.AppendFormat("SELECT * FROM GDA_UPLOADS (NOLOCK) WHERE ORIGINALNAME LIKE '%{0}%'; ", nomeArquivo);
                    using (SqlCommand commandF = new SqlCommand(selectFiles.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandF.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    //var key = reader["key"].ToString();
                                    //var deleteObjectRequest = new DeleteObjectRequest
                                    //{
                                    //    BucketName = bucketName,
                                    //    Key = folderName + key,
                                    //};
                                    //s3Client.DeleteObject(deleteObjectRequest);

                                    string blobConnectionString = "DefaultEndpointsProtocol=https;AccountName=stthprdguildaxferbsouth;AccountKey=Eirvcvk+eQ+GaCOYmBRB9CrJPMmae8LsuFlsG/GUwJLsCcEPekoT7yDbDjiKHmOomlg07DRB+pc/+AStwEOfQA==;EndpointSuffix=core.windows.net";
                                    string containerName = "guilda"; // Substitua pelo nome do seu container

                                    BlobServiceClient blobServiceClient = new BlobServiceClient(blobConnectionString);
                                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                                    // Obtém uma referência para o blob
                                    BlobClient blobClient = containerClient.GetBlobClient(reader["key"].ToString());

                                    // Exclui o blob
                                    blobClient.DeleteIfExists(DeleteSnapshotsOption.IncludeSnapshots);
                                }
                            }
                        }

                        StringBuilder sb = new StringBuilder();
                        sb.AppendFormat("DELETE GDA_UPLOADS WHERE ORIGINALNAME LIKE '%{0}%'; ", nomeArquivo);

                        using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }


                    //StringBuilder sb2 = new StringBuilder();
                    //sb2.AppendFormat("DELETE GDA_PRODUCT_IMAGES WHERE uploadId IN ({0}); ", imagesIdsAsString);
                    //using (SqlCommand command2 = new SqlCommand(sb2.ToString(), connection))
                    //{
                    //    command2.ExecuteNonQuery();
                    //}
                    //using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    //{
                    //    command.ExecuteNonQuery();
                    //}
                }
                catch (Exception ex)
                {

                }
            }


            return true;
        }

    }
}