    using ApiRepositorio.Controllers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static ApiRepositorio.Controllers.ReportMonthConsolidatedController;

namespace ApiC.Class.DowloadFile
{
    public class DatabaseService
    {
        public static List<object> GetQueryResult(string relatorio, string dtInicial, string dtFinal)
        {

            if (relatorio == "Saldo Acumulado")
            {
                return ReportMonthConsolidatedController.RelSaldoAcumulado(dtInicial, dtFinal, "", "", "", "", true).Cast<object>().ToList();
            }
            else if (relatorio == "Monetização Diario")
            {
                return ReportMonthDayController.relMonDiario(dtInicial, dtFinal, "", "", "", "", "", "", "", "","", true).Cast<object>().ToList();
            }
            else if (relatorio == "Monetizacao_Consolidado")
            {
                return ReportMonthAdmController.relMonMensal(dtInicial, dtFinal, "", "", "", "", "", "", "", "", true).Cast<object>().ToList();
            }
            else if (relatorio == "Resultado")
            {
                return ReportHomeResultController.relHomeResult(dtInicial, dtFinal, "", "", "", "", "", "", "", "", true).Cast<object>().ToList();
            }
            else if (relatorio == "Consolidado_setor")
            {
                return ReportConsolidatedSectorController.relConsolidatedSector(dtInicial, dtFinal, "", "", "", false, "", "",true).Cast<object>().ToList();
            }
            //else if (relatorio == "Resultado_Semanal")
            //{
            //    return ResultConsolidatedController.relConsolidatedSector(dtInicial, dtFinal, "", "", "", false, "", "", true).Cast<object>().ToList();
            //}
            //else if (relatorio == "Resultado_Mensal")
            //{
            //    return ResultConsolidatedController.relConsolidatedSector(dtInicial, dtFinal, "", "", "", false, "", "", true).Cast<object>().ToList();
            //}
            else
            {
                List<ModelsEx.homeRel> jsonData = new List<ModelsEx.homeRel>();
                return jsonData.Cast<object>().ToList();
            }





            // Lógica para executar a consulta SQL e retornar os dados
            //    using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            //{
            //    connection.Open();

            //    string sqlQuery = "SELECT * FROM gda_groups (nolock)";
            //    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
            //    {
            //        using (SqlDataReader reader = command.ExecuteReader())
            //        {
            //            List<ModelsEx.homeRel> result = new List<ModelsEx.homeRel>();

            //            while (reader.Read())
            //            {
            //                ModelsEx.homeRel rs = new ModelsEx.homeRel();
            //                rs.name = reader["NAME"].ToString();
            //                result.Add(rs);
            //            }

            //            return result;
            //        }
            //    }
            //}
        }

        public static void InsertQueryResult(string relatorio, string dtInicial, string dtFinal)
        {
            if (relatorio == "Fechamento Caixa")
            {
                FinancialSummaryController.InsertFinancialSummary();
            }
        }
    }
}