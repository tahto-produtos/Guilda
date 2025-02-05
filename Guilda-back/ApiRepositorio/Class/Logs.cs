using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using ApiC.Class;
using System.Windows.Media;
using Amazon.Runtime.Internal;
using DocumentFormat.OpenXml.Spreadsheet;

public class Logs {
    public static string InsertActionLogs(string Resquest, string tabelas, string matricula)
    {
        matricula = Convert.ToInt32(matricula).ToString();

        string stbLog = $"INSERT INTO GDA_LOG_ACTIONS (PROCESS, ALTERED_BY, ALTERED_AT, [TABLE]) VALUES ('{Resquest}', '{matricula}', GETDATE(), '{tabelas}') SELECT @@IDENTITY AS 'CODLOG'";
        string codlog = "";
        using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
        {
            connection.Open();
            try
            {
                using (SqlCommand command = new SqlCommand(stbLog.ToString(), connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            codlog = reader["CODLOG"].ToString();
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            connection.Close();
        }

        return codlog;

    }
    public static string InsertActionInputQuantiyProductsLogs(string idLog, string Quantidade, int idProduct, string ValorDoProduto, int idStock, string Fornecedor, string data)
    {

        string stbLog = $"INSERT INTO GDA_LOG_ACTIONS_INPUT_QUANTITY_PRODUCTS (IDGDA_LOG_ACTIONS, DATA_DA_EXIBICAO, FORNECEDOR, VALOR, IDPRODUTO, QUANTIDADE, ESTOQUE)values ({idLog}, '{data}', '{Fornecedor}', '{ValorDoProduto}',{idProduct}, {Quantidade}, {idStock})";
        using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
        {
            connection.Open();
            try
            {
                using (SqlCommand command = new SqlCommand(stbLog.ToString(), connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {

            }
            connection.Close();
        }
        return "";

    }
    public static string InsertActionPersonaUser(string social_name, int show_age, string picture, string altered_by, string your_motivation, string goals, string phone_number, string birth_date, int uf, int city, string who_is, string email)
    {

        string stbLog = $"INSERT INTO GDA_LOG_ACTIONS_PERSONA_USER " +
            $"(SOCIAL_NAME, SHOW_AGE, PICTURE, ALTERED_AT, ALTERED_BY, YOUR_MOTIVATIONS, GOALS, PHONE_NUMBER, BIRTH_DATE, UF, CITY, WHO_IS, EMAIL)" +
            $"VALUES" +
            $"('{social_name}', {show_age}, '{picture}', GETDATE(), '{altered_by}', '{your_motivation}', '{goals}', '{phone_number}', '{birth_date}', '{uf}', '{city}', '{who_is}', '{email}')";
        string codlog = "";
        using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
        {
            connection.Open();
            try
            {
                using (SqlCommand command = new SqlCommand(stbLog.ToString(), connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            codlog = reader["CODLOG"].ToString();
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            connection.Close();
        }

        return codlog;
    }
}

