using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

public class Sqlutil {
    public static void ExecutaSemRetorno(string query) {
        try {
            using (var con = new SqlConnection(Database.Conn)) {
                con.Open();
                new SqlCommand(query, con).ExecuteNonQuery();
                con.Close();
            }
        }
        catch (Exception) {

         
        }     
    }

    public static int ExecutaRetornaId(string query) {
        using (var con = new SqlConnection(Database.Conn)) {
            con.Open();
            var result = new SqlCommand(query, con).ExecuteReader();
            return result.Read() ? Convert.ToInt32(result[0]) : 0;
        }
    }

    public static string ExecutaRetornaString(string query) {
        using (var con = new SqlConnection(Database.Conn)) {
            con.Open();
            var result = new SqlCommand(query, con).ExecuteReader();
            return result.Read() ? result[0].ToString() : "";
        }
    }

    public static DataTable ExecutarRetornaDataTable(string query) {
        try {
            var dataLocal = new DataTable();

            using (var con = new SqlConnection(Database.Conn)) {
                con.Open();

                using (var c = new SqlDataAdapter(query, con)) {
                    c.Fill(dataLocal);
                    return dataLocal;
                }

            }
        }
        catch (Exception EX) {
        }
        return null;
    }
}

