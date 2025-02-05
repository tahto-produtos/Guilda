using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web.Security;

public class Database
{

    public static string Conn = "";

    public static string ConnPerformance = @"Data Source=10.115.65.41;Initial Catalog=GUILDA_PROD;Integrated Security=True;MultipleActiveResultSets=True;Connection Timeout=120";

    public static string dtbe = "2025-01-16";

    //Criado para o espelhamento
    public static string ConnHomolog = @"Data Source=10.115.193.41;Initial Catalog=GUILDA_HOMOLOG;Integrated Security=True;MultipleActiveResultSets=True;Connection Timeout=10";
    //public static string ConnHomolog = @"Data Source=10.115.193.41;Initial Catalog=GUILDA_HOMOLOG;User ID=TMP00001;Password=JwhMLD7FNqZ9Eysn;MultipleActiveResultSets=True;Connection Timeout=120";

    public static string retornaConn(Boolean threadEnv = false)
    {
        string enderecoIp = ObterEnderecoIpDaMaquina();
        string connectionString = ConstruirConnectionStringComBaseNoIp(enderecoIp, threadEnv);

        return connectionString;
    }

    public static string ObterEnderecoIpDaMaquina()
    {
        string hostName = Dns.GetHostName();
        IPAddress[] enderecos = Dns.GetHostAddresses(hostName);

        foreach (IPAddress endereco in enderecos)
        {
            if (endereco.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return endereco.ToString();
            }
        }

        throw new Exception("Endereço IP não encontrado");
    }

    static string ConstruirConnectionStringComBaseNoIp(string enderecoIp, bool threadEnv)
    {
        // Lógica para construir a string de conexão com base no endereço IP
        // Substitua isso de acordo com sua necessidade e tipo de banco de dados

        //if (threadEnv == true || enderecoIp == "26.190.33.61")
        //if (threadEnv == true)
        //{
        //    return @"Data Source=database-1.cpmxhjwr8mgp.us-east-1.rds.amazonaws.com;Initial Catalog=he;User ID=RDSadminsql2019;Password=H4ND4faG3AhjFe943NyPLRhMsEamXucKdDa;MultipleActiveResultSets=True;Connection Timeout=60";
        //}
        //else if (enderecoIp == "10.0.10.30" || enderecoIp == "172.27.236.113")
        //{
        //    return @"Data Source=database-1.cpmxhjwr8mgp.us-east-1.rds.amazonaws.com;Initial Catalog=GUILDA_PROD;User ID=RDSadminsql2019;Password=H4ND4faG3AhjFe943NyPLRhMsEamXucKdDa;MultipleActiveResultSets=True;Connection Timeout=10";
        //}
        //else
        //{
        //    return @"Data Source=10.0.132.254;Initial Catalog=GUILDA_HOMOLOG;User ID=RDSadminsql;Password=ZEfXSHpB5DOOVn7851;MultipleActiveResultSets=True;Connection Timeout=10";
        //}

        //Servidor Novo

        //    if (threadEnv == true)
        //    {
        return @"Data Source=10.115.65.41;Initial Catalog=GUILDA_PROD;User ID=TMP00001;Password=JwhMLD7FNqZ9Eysn;MultipleActiveResultSets=True;Connection Timeout=60";
        //return @"Data Source=10.115.65.41;Initial Catalog=GUILDA_PROD;Integrated Security=True;MultipleActiveResultSets=True;Connection Timeout=100";
        //    }
        //    else if (enderecoIp == "10.115.65.4")
        //    {
        //        //return @"Data Source=10.115.65.41;Initial Catalog=GUILDA_PROD;User ID=TMP00001;Password=JwhMLD7FNqZ9Eysn;MultipleActiveResultSets=True;Connection Timeout=10";
        //        return @"Data Source=10.115.65.41;Initial Catalog=GUILDA_PROD;Integrated Security=True;MultipleActiveResultSets=True;Connection Timeout=10";
        //    }
        //    else if (enderecoIp == "10.115.193.4")
        //    {
        //        //return @"Data Source=10.115.193.41;Initial Catalog=GUILDA_HOMOLOG;User ID=TMP00001;Password=JwhMLD7FNqZ9Eysn;MultipleActiveResultSets=True;Connection Timeout=10";
        //        return @"Data Source=10.115.193.41;Initial Catalog=GUILDA_HOMOLOG;Integrated Security=True;MultipleActiveResultSets=True;Connection Timeout=10";
        //    }
        //    else
        //    {
        //        //return @"Data Source=10.115.193.41;Initial Catalog=GUILDA_HOMOLOG;User ID=TMP00001;Password=JwhMLD7FNqZ9Eysn;MultipleActiveResultSets=True;Connection Timeout=10";
        //        //return @"Data Source=10.115.65.41;Initial Catalog=GUILDA_PROD;User ID=TMP00001;Password=JwhMLD7FNqZ9Eysn;MultipleActiveResultSets=True;Connection Timeout=10";
        //        return @"Data Source=10.115.65.41;Initial Catalog=GUILDA_PROD;Integrated Security=True;MultipleActiveResultSets=True;Connection Timeout=10";
        //    }
        }

    }

