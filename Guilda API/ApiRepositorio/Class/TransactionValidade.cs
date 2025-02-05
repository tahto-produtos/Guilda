using ApiRepositorio.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace ApiRepositorio.Class
{
    public class TransactionValidade
    {
        public static RepositorioDBContext db = new RepositorioDBContext();


        public static int? validate(HttpRequestHeaders header, long t_id)
        {


            StringBuilder temp = new StringBuilder();
            temp.AppendFormat($"SELECT CREATED_AT, IDGDA_TRANSACTION FROM GDA_TRANSACTION (NOLOCK) WHERE TRANSACTIONID = '{t_id}' ");

            DateTime? createdAt = null;
            int tid = 0;
            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(temp.ToString(), connection))
                {
                    command.CommandTimeout = 60;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            createdAt = Convert.ToDateTime(reader["CREATED_AT"].ToString());
                            tid = Convert.ToInt32(reader["IDGDA_TRANSACTION"].ToString());
                        }
                    }
                }
                connection.Close();
            }

            if (createdAt == null)
            {
                return null;
            }

            //TransactionModel t = db.TransactionModels.Where(p => p.transactionId == t_id.ToString()).FirstOrDefault();
            //if (t is null)
            //{
            //    return null;
            //}

            int d3 = (int)(DateTime.Now - createdAt).Value.TotalMinutes;

            if (d3 >= 1440)
            {
                return null;
            }

            return tid;
        }

        public static TransactionModel validateOnlyNumber(HttpRequestHeaders header, long t_id)
        {
            TransactionModel t = db.TransactionModels.Where(p => p.transactionId == t_id.ToString()).FirstOrDefault();
            if (t is null)
            {
                return null;
            }

            //int d3 = (int)(DateTime.Now - t.created_at).Value.TotalMinutes;

            //if (d3 >= 900)
            //{
            //    return null;
            //}

            return t;
        }

    }
}