using ApiRepositorio.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace ApiRepositorio.Class
{
    public class TokenValidate
    {

        public static bool validate(HttpRequestHeaders header)
        {
            string token = "";
            if (header.Contains("Token"))
            {
                token = header.GetValues("Token").First();
            }
            else
            {
                return false;
            }

            StringBuilder temp = new StringBuilder();
            temp.AppendFormat($"SELECT TOKEN FROM GDA_TOKEN (NOLOCK) WHERE ACTIVE = 1 AND TOKEN = '{token}' ");

            int qtd = 0;
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
                            qtd = 1;
                        }
                    }
                }
                connection.Close();
            }

            if (qtd == 0)
            {
                return false;
            }

            return true;
        }




    }
}