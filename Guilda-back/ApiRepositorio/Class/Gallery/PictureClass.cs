using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using ApiRepositorio.Models;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Windows.Media.Animation;
using Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace ApiC.Class
{
    public class PictureClass
    {
        private const string bucketName = "focare-public";
        private const string folderName = "public/";
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
        private static IAmazonS3 s3Client;
        //public static List<GalleryResponseModel> UploadFiles(HttpFileCollection files, int personauser)
        //{
        //    _ = new List<GalleryResponseModel>();
        //    List<GalleryResponseModel> filesUploads;
        //    try
        //    {
        //        bool validateFiles = ValidateFiles(files);
        //        if (validateFiles)
        //        {
        //            filesUploads = UploadFilesToBlob(files, personauser);
        //        }
        //        else
        //        {
        //            throw new Exception("Somente são aceitos os arquivos do tipo imagem");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //    return filesUploads;
        //}

        private static bool ValidateFiles(HttpFileCollection files)
        {
            string[] typesFiles = { "image/png", "image/jpg", "image/jpeg" };
            bool validate = true;
            foreach (string fileName in files)
            {
                HttpPostedFile file = files[fileName];

                //file.ContentType
                if (Array.IndexOf(typesFiles, file.ContentType) < 0)
                {
                    validate = false;
                }
            }
            return validate;
        }

        //private static List<GalleryResponseModel> UploadToAWSS3(HttpFileCollection files, int personauser)
        //{
        //    List<GalleryResponseModel> filesUploads = new List<GalleryResponseModel>();
        //    s3Client = new AmazonS3Client("AKIAQ5UGFJSENBSKKGM3", "juBFm1d52BrmuRqwSRCwyzU1Y9gItY/NdOS/NM2V", bucketRegion);

        //    foreach (string fileName in files)
        //    {
        //        HttpPostedFile file = files[fileName];

        //        string fileNameInS3 = Guid.NewGuid() + Path.GetExtension(file.FileName);

        //        var uploadRequest = new TransferUtilityUploadRequest
        //        {
        //            InputStream = file.InputStream,
        //            Key = folderName + fileNameInS3,
        //            BucketName = bucketName
        //            //CannedACL = S3CannedACL.PublicRead
        //        };

        //        using (var transferUtility = new TransferUtility(s3Client))
        //        {
        //            transferUtility.Upload(uploadRequest);
        //        }

        //        string fileUrl = $"https://{bucketName}.s3.amazonaws.com/{folderName}{fileNameInS3}";

        //        using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
        //        {
        //            connection.Open();
        //            try
        //            {
        //                StringBuilder stb = new StringBuilder();
        //                stb.Append($"UPDATE GDA_PERSONA_USER SET PICTURE ='{fileUrl}' ");
        //                stb.Append($"WHERE IDGDA_PERSONA_USER = {personauser} ");
        //                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
        //                {
        //                    int newId = Int32.Parse(command.ExecuteScalar().ToString());
        //                }
        //            }
        //            catch (Exception)
        //            {
        //                throw;
        //            }
        //        }

        //        var teste = fileUrl;
        //    }
        //    return filesUploads;
        //}

        public static List<GalleryResponseModel> UploadFilesToBlob(HttpFileCollection files, int personauser)
        {
            List<GalleryResponseModel> filesUploads = new List<GalleryResponseModel>();

            BlobServiceClient blobServiceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=stthprdguildaxferbsouth;AccountKey=Eirvcvk+eQ+GaCOYmBRB9CrJPMmae8LsuFlsG/GUwJLsCcEPekoT7yDbDjiKHmOomlg07DRB+pc/+AStwEOfQA==;EndpointSuffix=core.windows.net");
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("guilda");

            foreach (string fileName in files)
            {
                HttpPostedFile file = files[fileName];

                string fileNameInBlob = Guid.NewGuid() + Path.GetExtension(file.FileName);
                BlobClient blobClient = containerClient.GetBlobClient(fileNameInBlob);

                using (var stream = file.InputStream)
                {
                    blobClient.Upload(stream, new BlobHttpHeaders { ContentType = file.ContentType });
                }

                string fileUrl = "https://guilda.tahto.net.br/api/csharp/api/listImage?fileName=" + fileNameInBlob;

                //string fileUrl = blobClient.Uri.ToString();

                GalleryResponseModel GRM = new GalleryResponseModel
                {
                    url = fileUrl
                };


                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        StringBuilder stb = new StringBuilder();
                        stb.Append($"UPDATE GDA_PERSONA_USER SET PICTURE ='{fileUrl}' ");
                        stb.Append($"WHERE IDGDA_PERSONA_USER = {personauser} ");
                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            int newId = Int32.Parse(command.ExecuteScalar().ToString());
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                filesUploads.Add(GRM);
            }

            return filesUploads;
        }

        //public static List<GalleryResponseModel> UploadFilesAWSS3(HttpFileCollection files)
        //{
        //    List<GalleryResponseModel> filesUploads = new List<GalleryResponseModel>();
        //    s3Client = new AmazonS3Client("AKIAQ5UGFJSENBSKKGM3", "juBFm1d52BrmuRqwSRCwyzU1Y9gItY/NdOS/NM2V", bucketRegion);

        //    foreach (string fileName in files)
        //    {
        //        HttpPostedFile file = files[fileName];

        //        string fileNameInS3 = Guid.NewGuid() + Path.GetExtension(file.FileName);

        //        var uploadRequest = new TransferUtilityUploadRequest
        //        {
        //            InputStream = file.InputStream,
        //            Key = folderName + fileNameInS3,
        //            BucketName = bucketName
        //            //CannedACL = S3CannedACL.PublicRead
        //        };

        //        using (var transferUtility = new TransferUtility(s3Client))
        //        {
        //            transferUtility.Upload(uploadRequest);
        //        }

        //        string fileUrl = $"https://{bucketName}.s3.amazonaws.com/{folderName}{fileNameInS3}";

        //        GalleryResponseModel GRM = new GalleryResponseModel();
        //        GRM.url = fileUrl;

        //        filesUploads.Add(GRM);

        //    }
        //    return filesUploads;
        //}


        //public static void transferAwsToAzure()
        //{
        //    List<GalleryResponseModel> filesUploads = new List<GalleryResponseModel>();
        //    s3Client = new AmazonS3Client("AKIAQ5UGFJSENBSKKGM3", "juBFm1d52BrmuRqwSRCwyzU1Y9gItY/NdOS/NM2V", bucketRegion);

        //    BlobServiceClient blobServiceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=stthprdguildaxferbsouth;AccountKey=Eirvcvk+eQ+GaCOYmBRB9CrJPMmae8LsuFlsG/GUwJLsCcEPekoT7yDbDjiKHmOomlg07DRB+pc/+AStwEOfQA==;EndpointSuffix=core.windows.net");
        //    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("guilda");



        //    ListObjectsRequest request = new ListObjectsRequest
        //    {
        //        BucketName = "focare-public"
        //    };

        //    ListObjectsResponse response;
        //    do
        //    {
        //        response = s3Client.ListObjects(request);

        //        foreach (S3Object entry in response.S3Objects)
        //        {

        //            // Aqui você pode iniciar o processo de migração para a Azure
        //            string awsKey = entry.Key;
        //            string nomeArquivo = awsKey.Replace("public/", "");
        //            string nomeTemp = awsKey.Replace("public/", "").Split('.')[0];
        //            if (nomeTemp == "")
        //            {
        //                continue;
        //            }
        //            string awsBucketName = "focare-public";
        //            //using (var responseStream = s3Client.GetObjectStream(awsBucketName, awsKey, null))
        //            //{

        //            //    BlobClient blobClient = containerClient.GetBlobClient(nomeArquivo); // Usa o mesmo nome de chave do AWS S3


        //            //    blobClient.Upload(responseStream, new BlobHttpHeaders { ContentType = entry.con });
        //            //}

        //            using (GetObjectResponse getObjectResponse = s3Client.GetObject(awsBucketName, awsKey))
        //            {
        //                // Obtém o ContentType do objeto S3 a partir do cabeçalho "Content-Type"
        //                string contentType = getObjectResponse.Headers.ContentType;

        //                // Faz o download do arquivo para um stream
        //                using (Stream responseStream = getObjectResponse.ResponseStream)
        //                {
        //                    BlobClient blobClient = containerClient.GetBlobClient(nomeArquivo);
        //                    BlobHttpHeaders headers = new BlobHttpHeaders { ContentType = contentType };

        //                    // Faz o upload do arquivo para o Azure Blob Storage
        //                    blobClient.Upload(responseStream, headers);

        //                }
        //            }

        //        }

        //        //request.ContinuationToken = response.NextContinuationToken;
        //    } while (response.IsTruncated);



        //}



        public static List<GalleryResponseModel> UploadFilesToBlob(HttpFileCollection files)
        {
            List<GalleryResponseModel> filesUploads = new List<GalleryResponseModel>();

            BlobServiceClient blobServiceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=stthprdguildaxferbsouth;AccountKey=Eirvcvk+eQ+GaCOYmBRB9CrJPMmae8LsuFlsG/GUwJLsCcEPekoT7yDbDjiKHmOomlg07DRB+pc/+AStwEOfQA==;EndpointSuffix=core.windows.net");
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("guilda");

            foreach (string fileName in files)
            {
                HttpPostedFile file = files[fileName];

                string fileNameInBlob = Guid.NewGuid() + Path.GetExtension(file.FileName);
                BlobClient blobClient = containerClient.GetBlobClient(fileNameInBlob);

                using (var stream = file.InputStream)
                {
                    blobClient.Upload(stream, new BlobHttpHeaders { ContentType = file.ContentType });
                }

                string fileUrl = "https://guilda.tahto.net.br/api/csharp/api/listImage?fileName=" + fileNameInBlob;

                //string fileUrl = blobClient.Uri.ToString();

                GalleryResponseModel GRM = new GalleryResponseModel
                {
                    url = fileUrl
                };

                filesUploads.Add(GRM);
            }

            return filesUploads;
        }

        //public static void GetImageAsync(string fileName)
        //{
        //    fileName = SanitizeFileName(fileName);
        //    BlobServiceClient blobServiceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=stthprdguildaxferbsouth;AccountKey=Eirvcvk+eQ+GaCOYmBRB9CrJPMmae8LsuFlsG/GUwJLsCcEPekoT7yDbDjiKHmOomlg07DRB+pc/+AStwEOfQA==;EndpointSuffix=core.windows.net");
        //    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("guilda");
        //    BlobClient blobClient = containerClient.GetBlobClient(fileName);

        //    if (blobClient.Exists())
        //    {
        //        var blobDownloadInfo = blobClient.Download();

        //        //var asd = File(blobDownloadInfo.Value.Content, blobDownloadInfo.Value.ContentType);

        //    }
        //    else
        //    {
        //        // Lógica para quando o arquivo não for encontrado
        //        // return NotFound();
        //    }
        //}



        //public static bool DeleteFiles(DeleteFileGalleryModel files)
        //{
        //    s3Client = new AmazonS3Client("AKIAQ5UGFJSENBSKKGM3", "juBFm1d52BrmuRqwSRCwyzU1Y9gItY/NdOS/NM2V", bucketRegion);
        //    string imagesIdsAsString = string.Join(",", files.images.Select(i => i.id));

        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendFormat("DELETE GDA_UPLOADS WHERE id IN ({0}); ", imagesIdsAsString);

        //    using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
        //    {
        //        connection.Open();
        //        try
        //        {


        //            StringBuilder selectFiles = new StringBuilder();
        //            selectFiles.AppendFormat("SELECT * FROM GDA_UPLOADS WHERE id IN ({0}); ", imagesIdsAsString);
        //            using (SqlCommand commandF = new SqlCommand(selectFiles.ToString(), connection))
        //            {
        //                using (SqlDataReader reader = commandF.ExecuteReader())
        //                {
        //                    if (reader.HasRows)
        //                    {
        //                        while (reader.Read())
        //                        {
        //                            var key = reader["key"].ToString();
        //                            var deleteObjectRequest = new DeleteObjectRequest
        //                            {
        //                                BucketName = bucketName,
        //                                Key = folderName + key,
        //                            };
        //                            s3Client.DeleteObject(deleteObjectRequest);
        //                        }
        //                    }
        //                }
        //            }



        //            StringBuilder sb2 = new StringBuilder();
        //            sb2.AppendFormat("DELETE GDA_PRODUCT_IMAGES WHERE uploadId IN ({0}); ", imagesIdsAsString);
        //            using (SqlCommand command2 = new SqlCommand(sb2.ToString(), connection))
        //            {
        //                command2.ExecuteNonQuery();
        //            }
        //            using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
        //            {
        //                command.ExecuteNonQuery();
        //            }
        //        }
        //        catch
        //        {
        //            throw;
        //        }
        //    }


        //    return true;
        //}
    }
}