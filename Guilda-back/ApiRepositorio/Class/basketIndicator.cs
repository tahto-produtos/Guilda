using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace ApiC.Class
{
    public class basketIndicator
    {
        public class PostInputModel
        {
            public string groupId { get; set; }

            public string metricMin { get; set; }

        }

        public static List<PostInputModel> rtnBktIndicator()
        {
            
            StringBuilder stb = new StringBuilder();
            stb.Append("WITH CTE AS ( ");
            stb.Append("    SELECT *, ");
            stb.Append("           ROW_NUMBER() OVER (PARTITION BY GROUPID ORDER BY CREATED_AT DESC) AS RN ");
            stb.Append("    FROM GDA_HISTORY_INDICATOR_GROUP (NOLOCK) ");
            stb.Append("    WHERE INDICATOR_ID = 10000012 ");
            stb.Append(") ");
            stb.Append("SELECT METRIC_MIN, GROUPID ");
            stb.Append("FROM CTE ");
            stb.Append("WHERE RN = 1; ");

            List<PostInputModel> rmams = new List<PostInputModel>();

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PostInputModel rmam = new PostInputModel();
                                rmam.metricMin = reader["METRIC_MIN"].ToString();
                                rmam.groupId = reader["GROUPID"].ToString();

                                rmams.Add(rmam);
                            }
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
                connection.Close();
            }
            return rmams;
        }

    }
}