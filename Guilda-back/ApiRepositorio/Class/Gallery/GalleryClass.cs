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

namespace ApiC.Class
{
    public class GalleryClass
    {
        private const string bucketName = "focare-public";
        private const string folderName = "public/";
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;

        public static List<GalleryResponseModel> ListGalleryFiles()
        {
            List<GalleryResponseModel> files = new List<GalleryResponseModel>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT * FROM GDA_UPLOADS WHERE IDGDA_UPLOADS_CATEGORY = 1 AND type LIKE '%image/%'");


            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    var id = reader["id"].ToString();
                                    var originalNameBd = reader["originalName"].ToString();
                                    byte[] isoBytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(originalNameBd);
                                    var originalName = Encoding.UTF8.GetString(isoBytes);
                                    var key = reader["key"].ToString();
                                    var typeBd = reader["type"].ToString();
                                    var url = reader["url"].ToString();
                                    var createdAt = reader["created_at"].ToString();

                                    GalleryResponseModel file = new GalleryResponseModel();
                                    file.id = Int32.Parse(id);
                                    file.originalName = originalName;
                                    file.key = key;
                                    file.type = typeBd;
                                    file.url = url;
                                    file.created_at = createdAt;
                                    files.Add(file);
                                }
                            }
                            else
                            {
                                throw new Exception("Nenhum arquivo encontrado");
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

        public static List<GalleryResponseModel> UploadFiles(HttpFileCollection files)
        {
            _ = new List<GalleryResponseModel>();
            List<GalleryResponseModel> filesUploads;
            try
            {
                bool validateFiles = ValidateFiles(files);
                if (validateFiles)
                {
                    filesUploads = UploadFilesToBlob(files);
                }
                else
                {
                    throw new Exception("Somente são aceitos os arquivos do tipo imagem");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return filesUploads;
        }

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

                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        StringBuilder stb = new StringBuilder();
                        stb.Append("INSERT INTO GDA_UPLOADS (originalName, [key], type, url, IDGDA_UPLOADS_CATEGORY, created_at) VALUES ( ");
                        stb.AppendFormat("'{0}', ", file.FileName);
                        stb.AppendFormat("'{0}', ", fileNameInBlob);
                        stb.AppendFormat("'{0}', ", file.ContentType);
                        stb.AppendFormat("'{0}', ", fileUrl);
                        stb.AppendFormat("'{0}', ", 1);
                        stb.AppendFormat("'{0}');", DateTime.Now.ToString("yyyy-MM-dd H:m:s"));
                        stb.Append("SELECT SCOPE_IDENTITY()");

                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            int newId = Int32.Parse(command.ExecuteScalar().ToString());
                            GalleryResponseModel gl = new GalleryResponseModel
                            {
                                id = newId,
                                originalName = file.FileName,
                                key = fileNameInBlob,
                                type = file.ContentType,
                                url = fileUrl,
                                created_at = DateTime.Now.ToString("yyyy-MM-dd H:m:s")
                            };
                            filesUploads.Add(gl);
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

        //private static List<GalleryResponseModel> UploadToAWSS3(HttpFileCollection files)
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
        //                stb.Append("INSERT INTO GDA_UPLOADS (originalName, [key], type, url, IDGDA_UPLOADS_CATEGORY, created_at) VALUES ( ");
        //                stb.AppendFormat("'{0}', ", file.FileName);
        //                stb.AppendFormat("'{0}', ", fileNameInS3);
        //                stb.AppendFormat("'{0}', ", file.ContentType);
        //                stb.AppendFormat("'{0}', ", fileUrl);
        //                stb.AppendFormat("'{0}', ", 1);
        //                stb.AppendFormat("'{0}');", DateTime.Now.ToString("yyyy-MM-dd H:m:s"));
        //                stb.Append("SELECT SCOPE_IDENTITY()");

        //                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
        //                {
        //                    int newId = Int32.Parse(command.ExecuteScalar().ToString());
        //                    GalleryResponseModel gl = new GalleryResponseModel
        //                    {
        //                        id = newId,
        //                        originalName = file.FileName,
        //                        key = fileNameInS3,
        //                        type = file.ContentType,
        //                        url = fileUrl,
        //                        created_at = DateTime.Now.ToString("yyyy-MM-dd H:m:s")
        //                    };
        //                    filesUploads.Add(gl);
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

        public static bool DeleteFilesToBlob(DeleteFileGalleryModel files)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=stthprdguildaxferbsouth;AccountKey=Eirvcvk+eQ+GaCOYmBRB9CrJPMmae8LsuFlsG/GUwJLsCcEPekoT7yDbDjiKHmOomlg07DRB+pc/+AStwEOfQA==;EndpointSuffix=core.windows.net");
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("guilda");

            string imagesIdsAsString = string.Join(",", files.images.Select(i => i.id));

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("DELETE GDA_UPLOADS WHERE id IN ({0}); ", imagesIdsAsString);

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {


                    StringBuilder selectFiles = new StringBuilder();
                    selectFiles.AppendFormat("SELECT * FROM GDA_UPLOADS WHERE id IN ({0}); ", imagesIdsAsString);
                    using (SqlCommand commandF = new SqlCommand(selectFiles.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandF.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    var key = reader["key"].ToString();
                                    var deleteObjectRequest = new DeleteObjectRequest
                                    {
                                        BucketName = bucketName,
                                        Key = folderName + key,
                                    };
                                    BlobClient blobClient = containerClient.GetBlobClient(key);
                                    blobClient.DeleteIfExists(DeleteSnapshotsOption.IncludeSnapshots);
                                }
                            }
                        }
                    }



                    StringBuilder sb2 = new StringBuilder();
                    sb2.AppendFormat("DELETE GDA_PRODUCT_IMAGES WHERE uploadId IN ({0}); ", imagesIdsAsString);
                    using (SqlCommand command2 = new SqlCommand(sb2.ToString(), connection))
                    {
                        command2.ExecuteNonQuery();
                    }
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch
                {
                    throw;
                }
            }


            return true;
        }

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

        public static List<ReponseVerifyImagesInProductsModel> VerifyFileInProducts(VerifyImagesInProductsModel files)
        {
            List<ReponseVerifyImagesInProductsModel> filesAssociate = new List<ReponseVerifyImagesInProductsModel>();
            string imagesIdsAsString = string.Join(",", files.images.Select(i => i.id));

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    StringBuilder selectFiles = new StringBuilder();
                    selectFiles.Append("SELECT * FROM GDA_UPLOADS AS U ");
                    selectFiles.Append("JOIN GDA_PRODUCT_IMAGES AS PU ON PU.uploadId = U.id ");
                    selectFiles.Append("JOIN GDA_PRODUCT AS P ON P.IDGDA_PRODUCT = PU.productId ");
                    selectFiles.AppendFormat("WHERE U.id IN ({0}) ", imagesIdsAsString);
                    selectFiles.Append("AND P.DELETED_AT IS NULL");
                    using (SqlCommand command = new SqlCommand(selectFiles.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    var id = reader["id"].ToString();
                                    var originalName = reader["originalName"].ToString();
                                    var key = reader["key"].ToString();
                                    var type = reader["type"].ToString();
                                    var url = reader["url"].ToString();
                                    var createdAt = reader["created_at"].ToString();

                                    var idProduct = reader["IDGDA_PRODUCT"].ToString();
                                    var code = reader["CODE"].ToString();
                                    var description = reader["DESCRIPTION"].ToString();
                                    var comercialName = reader["COMERCIAL_NAME"].ToString();


                                    var hasItem = filesAssociate.Find(i => i.id == int.Parse(id));
                                    if (hasItem == null)
                                    {
                                        ReponseVerifyImagesInProductsModel gl = new ReponseVerifyImagesInProductsModel
                                        {
                                            id = int.Parse(id),
                                            originalName = originalName,
                                            key = key,
                                            type = type,
                                            url = url,
                                            created_at = createdAt,
                                            products = new List<ProductImageModel>()
                                        };
                                        gl.products.Add(new ProductImageModel
                                        {
                                            id = int.Parse(idProduct),
                                            code = code,
                                            description = description,
                                            comercialName = comercialName
                                        });
                                        filesAssociate.Add(gl);
                                    }
                                    else
                                    {
                                        hasItem.products.Add(new ProductImageModel
                                        {
                                            id = int.Parse(idProduct),
                                            code = code,
                                            description = description,
                                            comercialName = comercialName
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return filesAssociate;
        }

        public static bool AssociateImageToProducts(AssociateImageToProductsModel file)
        {
            try
            {
                if (file.id == default(int))
                    throw new Exception("Id da imagem é obrigatório");
            }
            catch(Exception ex)
            {
                throw new Exception("Id da imagem é obrigatório");
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    StringBuilder imageAssociateSB = new StringBuilder();
                    imageAssociateSB.AppendFormat("SELECT * FROM GDA_UPLOADS WHERE id = {0} ", file.id);
                    using (SqlCommand command = new SqlCommand(imageAssociateSB.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                connection.Close();
                                throw new Exception("Imagem não existe");
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
            var productsAssociated = VerifyFileInProducts(new VerifyImagesInProductsModel
            {
                images = new List<GalleryModel>
                {
                    new GalleryModel { id = file.id }
                }
            });

            foreach (var product in file.products)
            {
                var hasItem = productsAssociated.SelectMany(objeto => objeto.products).FirstOrDefault(productL => productL.id == product.id);
                if (hasItem == null)
                {
                    //VERIFICAR SE REALMENTE ESTE PRODUTO EXITES
                    using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                    {
                        connection.Open();
                        StringBuilder productExits = new StringBuilder();
                        productExits.AppendFormat("SELECT * FROM GDA_PRODUCT WHERE IDGDA_PRODUCT = {0} ", product.id.ToString());
                        productExits.Append(" AND DELETED_AT IS NULL ");
                        using (SqlCommand command = new SqlCommand(productExits.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    //INSERE ASSOCIAÇÃO
                                    StringBuilder sb2 = new StringBuilder();
                                    sb2.AppendFormat("INSERT INTO GDA_PRODUCT_IMAGES (uploadId, productId) VALUES ({0}, ", file.id.ToString());
                                    sb2.AppendFormat(" {0}); ", product.id.ToString());
                                    using (SqlCommand command2 = new SqlCommand(sb2.ToString(), connection))
                                    {
                                        command2.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}