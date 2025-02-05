using System;
using System.Configuration;
using System.Net;
using System.Text.RegularExpressions;

public class Database
{

    public static string Conn = @"Data Source=10.115.65.41;Initial Catalog=GUILDA_PROD;Integrated Security=True;MultipleActiveResultSets=True;Connection Timeout=180";
    //public static string Conn = @"Data Source=10.115.65.41;Initial Catalog=GUILDA_PROD;User ID=TMP00001;Password=JwhMLD7FNqZ9Eysn;MultipleActiveResultSets=True;Connection Timeout=60";
    public static string retornaConn()
    {
        string enderecoIp = ObterEnderecoIpDaMaquina();
        string connectionString = ConstruirConnectionStringComBaseNoIp(enderecoIp);

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

    static string ConstruirConnectionStringComBaseNoIp(string enderecoIp)
    {

        //Servidores anteriores
        //if (enderecoIp == "10.0.10.30" || enderecoIp == "172.27.236.66")
        //{
        //    return @"Data Source=database-1.cpmxhjwr8mgp.us-east-1.rds.amazonaws.com;Initial Catalog=GUILDA_PROD;User ID=RDSadminsql2019;Password=H4ND4faG3AhjFe943NyPLRhMsEamXucKdDa;MultipleActiveResultSets=True;Connection Timeout=10";
        //}
        //else
        //{
        //    //return @"Data Source=database-1.cpmxhjwr8mgp.us-east-1.rds.amazonaws.com;Initial Catalog=GUILDA_PROD;User ID=RDSadminsql2019;Password=H4ND4faG3AhjFe943NyPLRhMsEamXucKdDa;MultipleActiveResultSets=True;Connection Timeout=10";
        //    return @"Data Source=10.0.132.254;Initial Catalog=GUILDA_HOMOLOG;User ID=RDSadminsql;Password=ZEfXSHpB5DOOVn7851;MultipleActiveResultSets=True;Connection Timeout=10";
        //}

        //Servidores Novos
        //if (enderecoIp == "10.115.65.4")
        //{ 

        //return @"Data Source=10.115.65.41;Initial Catalog=GUILDA_PROD;Integrated Security=True;MultipleActiveResultSets=True;Connection Timeout=180";
        return @"Data Source=10.115.65.41;Initial Catalog=GUILDA_PROD;User ID=TMP00001;Password=JwhMLD7FNqZ9Eysn;MultipleActiveResultSets=True;Connection Timeout=60";
        //}
        //else
        //{
        //    return @"Data Source=10.115.193.41;Initial Catalog=GUILDA_HOMOLOG;Integrated Security=True;MultipleActiveResultSets=True;Connection Timeout=10";
        //}
    }


}

