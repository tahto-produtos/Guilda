using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using ApiRepositorio.Models;
using BCrypt.Net;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls;

namespace ApiC.Class
{
    public class ProductClass
    {
        public static bool ImportProducts(ExcelWorksheet worksheet)
        {
            try
            {
                int rowCount = worksheet.Dimension.Rows;
                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        ExcelRange cells = worksheet.Cells;

                        //VALIDA DADOS DA LINHA
                        var validate = ValidateRow(cells, row);

                        if (validate)
                        {
                            InsertProduct(cells, row);
                            //RECUPERA OS DADOS DA LINHA NAS VARIÁVEIS
                            //var bc = cells[row, 1].Value.ToString();
                            //var operation = cells[row, 5].Value.ToString();
                            //var coins = cells[row, 6].Value.ToString();
                            //var reason = cells[row, 7].Value.ToString();
                            //var observation = cells[row, 8].Value.ToString();

                            //var success = this.AddCredit(bc, operation, coins, reason, observation);

                            //int coinsInt = Int32.Parse(coins);
                            //var ca = new CheckingAccountModel();
                            //ca.Collaborator_Id = bc;
                            //ca.Input = operation == "Crédito" ? coinsInt : 0;
                            //ca.Output = operation == "Débito" ? coinsInt : 0;
                            //ca.Observation = observation;
                            //ca.Reason = reason;

                            //if (success)
                            //{
                            //    camSuccess.Add(ca);
                            //}
                            //else
                            //{
                            //    camFailed.Add(ca);
                            //}
                        }
                    }
                    catch (Exception ex)
                    {
                        var exception = ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return true;
        }

        private static bool ValidateRow(ExcelRange cells, int rowNumber)
        {
            bool validate = true;
            try
            {
                var type = cells[rowNumber, 1];
                type.Value.ToString();
                if(type.Value.ToString() == "")
                {
                    validate = false;
                }
                else if (type.Value.ToString() != "FISICO" && type.Value.ToString() != "VIRTUAL")
                {
                    validate = false;
                }

                if (type.Value.ToString() == "VIRTUAL")
                {
                    var vouchers = cells[rowNumber, 2];
                    vouchers.Value.ToString();
                    if (vouchers.Value.ToString() == "") 
                    {
                        validate = false;
                    }
                }

                var code = cells[rowNumber, 3];
                code.Value.ToString();
                if (code.Value.ToString() == "") 
                { 
                    validate = false; 
                }

                var name = cells[rowNumber, 4];
                name.Value.ToString();
                if (name.Value.ToString() == "") 
                { 
                    validate = false; 
                }

                var description = cells[rowNumber, 5];
                description.Value.ToString();
                if (description.Value.ToString() == "") 
                { 
                    validate = false; 
                }

                int numberTest = 0;

                var quantity = cells[rowNumber, 6];
                if (!int.TryParse(quantity.Value.ToString(), out numberTest))
                {
                    validate = false;
                }

                var price = cells[rowNumber, 7];
                if (!int.TryParse(price.Value.ToString(), out numberTest))
                {
                    validate = false;
                }

                var stock = cells[rowNumber, 8];
                stock.Value.ToString();
                if (stock.Value.ToString() == "") 
                { 
                    validate = false; 
                }

                var site = cells[rowNumber, 9];
                site.Value.ToString();
                if (site.Value.ToString() == "") 
                { 
                    validate = false; 
                }

                var initDateExibition = cells[rowNumber, 14];
                initDateExibition.Value.ToString();
                if (initDateExibition.Value.ToString() == "") 
                { 
                    validate = false; 
                }

                var endDateExibition = cells[rowNumber, 15];
                endDateExibition.Value.ToString();
                if (endDateExibition.Value.ToString() == "") 
                { 
                    validate = false; 
                }

                var limitForCollaborator = cells[rowNumber, 16];
                limitForCollaborator.Value.ToString();
                if (limitForCollaborator.Value.ToString() == "") 
                { 
                    validate = false; 
                }

                var status = cells[rowNumber, 17];
                status.Value.ToString();
                if (status.Value.ToString() == "") 
                { 
                    validate = false; 
                }

                //var validityDate = cells[rowNumber, 18];
                //validityDate.Value.ToString();
            }
            catch (Exception ex)
            {
                validate = false;
            }

            return validate;
        }

        private static bool InsertProduct(ExcelRange cells, int rowNumber)
        {
            var stockName = cells[rowNumber, 8];
            var stockNameP = stockName.Value.ToString();
            var city = cells[rowNumber, 9];
            var cityP = city.Value.ToString();
            var type = cells[rowNumber, 1];
            var typeP = type.Value.ToString();

            StockModel stock = FindStockOrCreateByDescription(stockNameP, cityP, typeP);

            //using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            //{
            //    connection.Open();
            //    try
            //    {
            //        StringBuilder stb = new StringBuilder();
            //        stb.Append("INSERT INTO GDA_UPLOADS (originalName, [key], type, url, created_at) VALUES ( ");
            //        stb.AppendFormat("'{0}', ", file.FileName);
            //        stb.AppendFormat("'{0}', ", fileNameInS3);
            //        stb.AppendFormat("'{0}', ", file.ContentType);
            //        stb.AppendFormat("'{0}', ", fileUrl);
            //        stb.AppendFormat("'{0}');", DateTime.Now.ToString("yyyy-MM-dd H:m:s"));
            //        stb.Append("SELECT SCOPE_IDENTITY()");

            //        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
            //        {
            //            int newId = Int32.Parse(command.ExecuteScalar().ToString());
            //            GalleryResponseModel gl = new GalleryResponseModel
            //            {
            //                id = newId,
            //                originalName = file.FileName,
            //                key = fileNameInS3,
            //                type = file.ContentType,
            //                url = fileUrl,
            //                created_at = DateTime.Now.ToString("yyyy-MM-dd H:m:s")
            //            };
            //            filesUploads.Add(gl);
            //        }
            //    }
            //    catch (Exception)
            //    {
            //        throw;
            //    }
            //}
            return true;
        }

        private static StockModel FindStockOrCreateByDescription(string stockName, string city, string type)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT * FROM GDA_STOCK WHERE DESCRIPTION = '{0}' AND DELETED_AT IS NULL; ", stockName);

            StockModel stcModel = new StockModel();

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
                                    var id = reader["IDGDA_STOCK"].ToString();
                                    var description = reader["DESCRIPTION"].ToString();
                                    var createdAt = reader["CREATED_AT"].ToString();
                                    var deletedAt = reader["DELETED_AT"].ToString();
                                    var cityDb = reader["CITY"].ToString();
                                    var typeDb = reader["type"].ToString();

                                    stcModel.idGdaStock = Int32.Parse(id);
                                    stcModel.description = description;
                                    stcModel.createdAt = DateTimeOffset.Parse(createdAt);
                                    stcModel.city = cityDb;
                                    stcModel.type = typeDb;
                                }
                            }
                            else
                            {
                                StringBuilder stb = new StringBuilder();
                                DateTimeOffset dateInsert = DateTime.Now;
                                stb.Append("INSERT INTO GDA_STOCK (DESCRIPTION, CITY, type, CREATED_AT) VALUES ( ");
                                stb.AppendFormat("'{0}', ", stockName);
                                stb.AppendFormat("'{0}', ", city);
                                stb.AppendFormat("'{0}', ", type);
                                stb.AppendFormat("'{0}');", dateInsert.ToString("yyyy-MM-dd H:m:s"));
                                stb.Append("SELECT SCOPE_IDENTITY()");

                                using (SqlCommand commandInsert = new SqlCommand(stb.ToString(), connection))
                                {
                                    int Id = Int32.Parse(commandInsert.ExecuteScalar().ToString());
                                    stcModel.idGdaStock = Id;
                                    stcModel.description = stockName;
                                    stcModel.createdAt = dateInsert;
                                    stcModel.city = city;
                                    stcModel.type = type;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    throw;
                }
            }
            return stcModel;
        }

        private static StockModel FindProductOrCreate(string code, string description, int quantity, int price, string type, int saleLimit, DateTimeOffset dateValidit, string comercialName, DateTimeOffset expirationDate, DateTimeOffset publicationDate)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT * FROM GDA_PRODUCT WHERE CODE = '{0}' AND DELETED_AT IS NULL; ", code);

            StockModel stcModel = new StockModel();

            //using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            //{
            //    connection.Open();
            //    try
            //    {
            //        using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
            //        {
            //            using (SqlDataReader reader = command.ExecuteReader())
            //            {
            //                if (reader.HasRows)
            //                {
            //                    while (reader.Read())
            //                    {
            //                        var id = reader["IDGDA_STOCK"].ToString();
            //                        var description = reader["DESCRIPTION"].ToString();
            //                        var createdAt = reader["CREATED_AT"].ToString();
            //                        var deletedAt = reader["DELETED_AT"].ToString();
            //                        var cityDb = reader["CITY"].ToString();
            //                        var typeDb = reader["type"].ToString();

            //                        stcModel.idGdaStock = Int32.Parse(id);
            //                        stcModel.description = description;
            //                        stcModel.createdAt = DateTimeOffset.Parse(createdAt);
            //                        stcModel.city = cityDb;
            //                        stcModel.type = typeDb;
            //                    }
            //                }
            //                else
            //                {
            //                    StringBuilder stb = new StringBuilder();
            //                    DateTimeOffset dateInsert = DateTime.Now;
            //                    stb.Append("INSERT INTO GDA_STOCK (DESCRIPTION, CITY, type, CREATED_AT) VALUES ( ");
            //                    stb.AppendFormat("'{0}', ", stockName);
            //                    stb.AppendFormat("'{0}', ", city);
            //                    stb.AppendFormat("'{0}', ", type);
            //                    stb.AppendFormat("'{0}');", dateInsert.ToString("yyyy-MM-dd H:m:s"));
            //                    stb.Append("SELECT SCOPE_IDENTITY()");

            //                    using (SqlCommand commandInsert = new SqlCommand(stb.ToString(), connection))
            //                    {
            //                        int Id = Int32.Parse(commandInsert.ExecuteScalar().ToString());
            //                        stcModel.idGdaStock = Id;
            //                        stcModel.description = stockName;
            //                        stcModel.createdAt = dateInsert;
            //                        stcModel.city = city;
            //                        stcModel.type = type;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    catch
            //    {
            //        throw;
            //    }
            //}
            return stcModel;
        }
    }
}