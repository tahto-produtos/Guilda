using ApiRepositorio.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;

namespace ApiRepositorio.Class
{
    public class Log
    {
        public static string GetRequestBody()
        {
            var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
            bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
            var bodyText = bodyStream.ReadToEnd();
            return bodyText;
        }

        public static bool insertLog(string request, RepositorioDBContext db, int returnStatus, string route)
        {
            var requestStr = GetRequestBody();

            LogRequestModel lg = new LogRequestModel();
            lg.REQUEST = request.ToString();
            lg.ROUTE = route;
            lg.RETURN = returnStatus;

            db.LogRequestModels.Add(lg);
            try
            {
                db.SaveChanges();
            }
            catch { }

            return true;
        }

        public static bool insertLogTransaction(string TRANSACTIONID, string TYPE, string STATUS, string DATEFILE, int COUNTLINES = 0)
        {
            //string connectionString = "Data Source=database-1.cpmxhjwr8mgp.us-east-1.rds.amazonaws.com;Initial Catalog=GUILDA_PROD;User ID=RDSadminsql2019;Password=H4ND4faG3AhjFe943NyPLRhMsEamXucKdDa;MultipleActiveResultSets=True;Connection Timeout=10";
            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                try
                {
                    connection.Open();

                    if (DATEFILE == "")
                    {
                        DATEFILE = "NULL";
                    }
                    else
                    {
                        DATEFILE = "'" + DATEFILE + "'";
                    }

                    //Inserir logs
                    StringBuilder queryInsertResult2 = new StringBuilder();
                    queryInsertResult2.Append("INSERT INTO GDA_LOG_TRANSACTION (TRANSACTIONID, TYPE, CREATED_AT, STATUS, DATE_FILE, AMOUNT) VALUES ( ");
                    queryInsertResult2.AppendFormat("'{0}', ", TRANSACTIONID); //TRANSACTIONID
                    queryInsertResult2.AppendFormat("'{0}', ", TYPE); //TYPE
                    queryInsertResult2.AppendFormat("GETDATE(), "); //CREATED_AT
                    queryInsertResult2.AppendFormat("'{0}', ", STATUS); //STATUS
                    queryInsertResult2.AppendFormat("{0}, ", DATEFILE); //DATEFILE
                    queryInsertResult2.AppendFormat("{0} ", COUNTLINES); //DATEFILE
                    queryInsertResult2.AppendFormat(") ");

                    SqlCommand createTableCommand2 = new SqlCommand(queryInsertResult2.ToString(), connection);
                    createTableCommand2.ExecuteNonQuery();

                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            return true;
        }


    }
}