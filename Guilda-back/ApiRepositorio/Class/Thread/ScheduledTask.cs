using ApiRepositorio.Controllers;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using static ApiRepositorio.Controllers.ReportMonthConsolidatedController;

namespace ApiC.Class.DowloadFile
{
    public static class ScheduledTask
    {
        public class objectsClass
        {
            public List<object> listObjects { get; set; }
        }

        static Timer timer;
        static Timer timerPreview1;
        static Timer timerPreview2;
        static Timer timerPreview3;
        static Timer timerPreview4;
        static Timer timerPreview5;




        static Timer timer2;
        static readonly object timerLock = new object();

        public static void insereLogProd(string process, string status)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Database.Conn))
                {
                    //GDA_ATRIBUTES 
                    connection.Open();
                    StringBuilder stb = new StringBuilder();
                    stb.Append("INSERT INTO GDA_LOG_THREAD (PROCESS, CREATED_AT, STATUS) ");
                    stb.AppendFormat("VALUES ('{0}', GETDATE(), '{1}') ", process, status);

                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch (Exception)
            {

            }
        }

        public static void insereLog(string process, string status)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Database.ConnHomolog))
                {
                    //GDA_ATRIBUTES 
                    connection.Open();
                    StringBuilder stb = new StringBuilder();
                    stb.Append("INSERT INTO GDA_LOG_THREAD (PROCESS, CREATED_AT, STATUS) ");
                    stb.AppendFormat("VALUES ('{0}', GETDATE(), '{1}') ", process, status);

                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch (Exception)
            {

            }
        }

        //Verifica se rodou o ultimo
        public static bool ultimaDataRelatorios()
        {
            bool rodouUltimoRelatorio = false;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT TOP 1 CREATED_AT FROM GDA_LOG_THREAD (NOLOCK) ");
            sb.Append("WHERE PROCESS = 'ReportsPreview' AND STATUS = 'Execute Concluded' ");
            sb.Append("ORDER BY CREATED_AT DESC  ");

            DateTime ultimaDataDoLog = new DateTime();

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ultimaDataDoLog = Convert.ToDateTime(reader["CREATED_AT"].ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            // 23h30
            DateTime ultimoDia = DateTime.Now;

            if (ultimoDia.Hour == 23)
            {
                ultimoDia = DateTime.Now;

                ultimoDia = new DateTime(ultimoDia.Year, ultimoDia.Month, ultimoDia.Day, 23, 0, 0);
            }
            else
            {
                ultimoDia = DateTime.Now.AddDays(-1);

                ultimoDia = new DateTime(ultimoDia.Year, ultimoDia.Month, ultimoDia.Day, 23, 0, 0);
            }

            // Verifica se o último log ocorreu após as 23h00 do último dia
            if (ultimaDataDoLog > ultimoDia)
            {
                rodouUltimoRelatorio = true;
            }
            else
            {
                rodouUltimoRelatorio = false;
            }

            //Caso não tenha rodado pela thread ter sido reiniciada, rodará agora se for um horario bom.
            if (rodouUltimoRelatorio == false && (DateTime.Now.Hour == 23 || DateTime.Now.Hour <= 2))
            {
                return true;
            }


            return false;
        }

        public static void StartReports()
        {
            try
            {
                insereLogProd("StartReports", "Execute Start");

                DateTime now = DateTime.Now;

                //DateTime scheduledTime1 = new DateTime(now.Year, now.Month, now.Day, 22, 30, 0);
                //if (now > scheduledTime1)
                //{
                //    scheduledTime1 = scheduledTime1.AddDays(1);
                //}
                //int dueTime1 = (int)(scheduledTime1 - now).TotalMilliseconds;
                //timerPreview1 = new Timer(reportsPreview1, null, dueTime1, 24 * 60 * 60 * 1000);


                //DateTime scheduledTime = new DateTime(now.Year, now.Month, now.Day, 22, 30, 0);
                //if (now > scheduledTime)
                //{
                //    scheduledTime = scheduledTime.AddDays(1);
                //}
                //int dueTime = (int)(scheduledTime - now).TotalMilliseconds;
                //timerPreview2 = new Timer(reportsPreview2, null, dueTime, 24 * 60 * 60 * 1000);
                //timer = new Timer(ExecuteTask, null, dueTime, 60 * 1000);


                DateTime scheduledTime2 = new DateTime(now.Year, now.Month, now.Day, 23, 00, 0);

                if (now > scheduledTime2)
                {
                    scheduledTime2 = scheduledTime2.AddDays(1);
                }

                int dueTime2 = (int)(scheduledTime2 - now).TotalMilliseconds;
                timerPreview3 = new Timer(reportsPreview3, null, dueTime2, 24 * 60 * 60 * 1000);
                //timer = new Timer(ExecuteTask, null, dueTime, 60 * 1000);

                // Clima

                DateTime now5 = DateTime.Now;
                DateTime scheduledTime4 = new DateTime(now5.Year, now5.Month, now5.Day, 23, 10, 0);
                //DateTime scheduledTime4 = new DateTime(now.Year, now.Month, now.Day, now.Hour,  now.Minute + 2, 0);

                if (now5 > scheduledTime4)
                {
                    //scheduledTime4 = scheduledTime4.AddMinutes(2);
                    scheduledTime4 = scheduledTime4.AddDays(1);
                }

                int period = 120000;

                int dueTime4 = (int)(scheduledTime4 - now5).TotalMilliseconds;
                timerPreview5 = new Timer(ExecuteTask3, null, dueTime4, 24 * 60 * 60 * 1000);
                //DateTime scheduledTime3 = new DateTime(now.Year, now.Month, now.Day, 23, 30, 0);

                //if (now > scheduledTime3)
                //{
                //    scheduledTime3 = scheduledTime3.AddDays(1);
                //}

                //int dueTime3 = (int)(scheduledTime3 - now).TotalMilliseconds;
                //timerPreview4 = new Timer(reportsPreview4, null, dueTime3, 24 * 60 * 60 * 1000);

                insereLogProd("StartReports", "Execute Concluded");
            }
            catch (Exception)
            {

            }
        }

        public static void Start()
        {
            lock (timerLock)
            {
                // Configurar o timer para chamar o método ExecuteTask todos os dias às 2 da manhã
                insereLog("Thread", "Execute Started");

                try
                {
                    //if (ultimaDataRelatorios() == true)
                    //{
                    //    ExecuteTask(null);
                    //}
                    //else
                    //{
                    DateTime nowAg = DateTime.Now;
                    DateTime scheduledTimeAg = new DateTime(nowAg.Year, nowAg.Month, nowAg.Day, 22, 00, 0);

                    if (nowAg > scheduledTimeAg)
                    {
                        scheduledTimeAg = scheduledTimeAg.AddDays(1);
                    }

                    int dueTimeAg = (int)(scheduledTimeAg - nowAg).TotalMilliseconds;
                    timer = new Timer(ExecuteTask, null, dueTimeAg, 24 * 60 * 60 * 1000);
                    //timer = new Timer(ExecuteTask, null, dueTime, 60 * 1000);
                    //}

                    //timer = new Timer(ExecuteTask, null, dueTime, 60 * 1000);

                    //DateTime now2 = DateTime.Now;
                    //DateTime scheduledTimeOutro = new DateTime(now2.Year, now2.Month, now2.Day, 01, 00, 0);

                    //if (now2 > scheduledTimeOutro)
                    //{
                    //    scheduledTimeOutro = scheduledTimeOutro.AddDays(1);
                    //}

                    //int dueTimeOutro = (int)(scheduledTimeOutro - now2).TotalMilliseconds;
                    //timer2 = new Timer(ExecuteTask2, null, dueTimeOutro, 24 * 60 * 60 * 1000);



                    //timerPreview5 = new Timer(ExecuteTask3, null, dueTime4, period);




                    //Inserir o fechamento de caixa do dia.
                    //DateTime Now = DateTime.Now;
                    //DateTime DateNowFinancialSummarry = DateTime.Now.AddDays(-1);
                    //if (DateNowFinancialSummarry < Now)
                    //{
                    //    InsertRelatorioFinanceiro();
                    //}

                    insereLog("Thread", "Execute Concluded");

                }
                catch (Exception ex)
                {
                    insereLog("Thread", $"Execute Error {ex.Message}");
                }
            }


            //timer = new Timer(ExecuteTask, null, dueTime, 60 * 1000);
        }

        private static void ExecuteTask3(object state)
        {
            BankDoClimate.InserClimateUserNotAnswerd();


            // Obtém a data e hora atual
            DateTime datainicio = DateTime.Now;

            // Calcula a próxima hora cheia
            //DateTime proximaHora = DateTime.Now.AddHours(1);
            DateTime proximoDia = DateTime.Now.AddDays(1);
            proximoDia = new DateTime(proximoDia.Year, proximoDia.Month, proximoDia.Day, proximoDia.Hour, proximoDia.Minute, 0);

            // Calcula o tempo até a próxima hora cheia
            int dueTime = (int)(proximoDia - DateTime.Now).TotalMilliseconds;
            int period = 24 * 60 * 60 * 1000;
            timer.Change(dueTime, period);
        }


        private static void ExecuteTask2(object state)
        {
            DateTime datainicio = DateTime.Now;

            InsertRelatorioFinanceiro(datainicio);

            DateTime proximodia = DateTime.Now.AddDays(1);

            proximodia = new DateTime(proximodia.Year, proximodia.Month, proximodia.Day, 01, 0, 0);

            int dueTime = (int)(proximodia - DateTime.Now).TotalMilliseconds;
            timer.Change(dueTime, 24 * 60 * 60 * 1000);
        }
        private static void ExecuteTask(object state)
        {


            //insereLog("ExecuteTask", "Execute");

            homologGlass();


            DateTime ultimoDia = DateTime.Now;
            if (ultimoDia.Hour >= 22)
            {
                ultimoDia = DateTime.Now;

                ultimoDia = new DateTime(ultimoDia.Year, ultimoDia.Month, ultimoDia.Day, 22, 0, 0);
            }
            else
            {
                ultimoDia = DateTime.Now.AddDays(-1);

                ultimoDia = new DateTime(ultimoDia.Year, ultimoDia.Month, ultimoDia.Day, 22, 0, 0);
            }


            //try
            //{
            //    List<basket> lbasket = listBasketConfiguration(true);
            //    List<groups> lgroup = listGroups("", true);

            //    List<usuariosBk> users = returnUsers(DateTime.Now.ToString("yyyy-MM-dd"), true);
            //    foreach (usuariosBk user in users)
            //    {
            //        ReturnBasketIndicatorUser biu = returnIndicatorUserMonetization(user.idCollaborator.ToString(), lbasket, lgroup, true);

            //        attUser(biu.idGroup, user.idTable, true);
            //    }
            //}
            //catch (Exception)
            //{

            //}



            ultimoDia.AddDays(1);
            DateTime now = DateTime.Now;
            int dueTime = (int)(ultimoDia - now).TotalMilliseconds;
            timer.Change(dueTime, 24 * 60 * 60 * 1000);

            insereLog("NextMilisecond", dueTime.ToString());
        }
        public class ReturnBasketIndicatorUser
        {
            public string idCollaborator { get; set; }
            public int idGroup { get; set; }
            public string groupName { get; set; }
            public string groupAlias { get; set; }
            public string groupImage { get; set; }
            public double coinsEarned { get; set; }
            public double coinsPossible { get; set; }
            public double resultPercent { get; set; }

        }
        public class usuariosBk
        {
            public int idCollaborator { get; set; }
            public int idTable { get; set; }
        }
        public class basketIndicatorResults
        {
            public string cargo { get; set; }
            public string cargoResult { get; set; }
            public int idcollaborator { get; set; }
            public string diasTrabalhados { get; set; }
            public string diasEscalados { get; set; }
            public string dataPagamento { get; set; }
            public int codIndicator { get; set; }
            public double moedasGanhas { get; set; }
            public double moedasPossiveis { get; set; }
            public int qtdPessoas { get; set; }
        }
        public static void attUser(int idGroup, int idTable, bool prod)
        {

            //QUERY MOEDAS GANHAS
            string dtUltima = DateTime.Now.ToString("yyyy-MM-dd");
            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("UPDATE GDA_COLLABORATORS_DETAILS ");
            stb.AppendFormat("SET IDGDA_GROUP = '{0}' ", idGroup);
            stb.AppendFormat("WHERE ID = '{0}' ", idTable);

            using (SqlConnection connection = new SqlConnection(Database.retornaConn(prod)))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {

                    //throw;
                }
                connection.Close();
            }

        }
        public static ReturnBasketIndicatorUser returnIndicatorUserMonetization(string idCol, List<basket> lbasket, List<groups> lgroup, bool prod)
        {
            ReturnBasketIndicatorUser rmams = new ReturnBasketIndicatorUser();
            rmams.idCollaborator = idCol;
            double moedasGanhas = 0;
            DateTime dtInicio = DateTime.Now.AddDays(-30);
            string dtIni = dtInicio.ToString("yyyy-MM-dd");
            DateTime dtFinal = DateTime.Now;
            string dtFim = dtFinal.ToString("yyyy-MM-dd");

            //QUERY MOEDAS GANHAS
            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtIni);
            stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFim);
            stb.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}'; ", idCol);
            stb.Append("SELECT ISNULL(SUM(INPUT) - SUM(OUTPUT),0) AS INPUT ");
            stb.Append("   FROM GDA_CHECKING_ACCOUNT (NOLOCK) ");
            stb.Append("   WHERE RESULT_DATE >= @DATAINICIAL ");
            stb.Append("     AND RESULT_DATE <= @DATAFINAL ");
            stb.Append("	 AND COLLABORATOR_ID = @INPUTID ");
            stb.Append("     AND GDA_INDICATOR_IDGDA_INDICATOR IS NOT NULL ");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn(prod)))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                moedasGanhas = Convert.ToDouble(reader["INPUT"].ToString());
                            }
                        }
                    }
                }
                catch (Exception)
                {

                    //throw;
                }
            }

            //QUERY PARA PEGAR AS MOEDAS POSSIVEIS
            stb.Clear();
            stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtIni);
            stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFim);
            stb.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}'; ", idCol);
            stb.Append(" ");
            stb.Append("SELECT MAX(R.IDGDA_COLLABORATORS) AS IDGDA_COLLABORATORS, ");
            stb.Append("       MAX(TRAB.RESULT) AS TRAB, ");
            stb.Append("       MAX(ESC.RESULT) AS ESC, ");
            stb.Append("       CONVERT(DATE, R.CREATED_AT) AS CREATED_AT, ");
            stb.Append("       CASE ");
            stb.Append("           WHEN @INPUTID = MAX(CL.IDGDA_COLLABORATORS) THEN 'AGENTE' ");
            stb.Append("           WHEN @INPUTID = MAX(CL.MATRICULA_SUPERVISOR) THEN 'SUPERVISOR' ");
            stb.Append("           WHEN @INPUTID = MAX(CL.MATRICULA_COORDENADOR) THEN 'COORDENADOR' ");
            stb.Append("           WHEN @INPUTID = MAX(CL.MATRICULA_GERENTE_II) THEN 'GERENTE_II' ");
            stb.Append("           WHEN @INPUTID = MAX(CL.MATRICULA_GERENTE_I) THEN 'GERENTE_I' ");
            stb.Append("           WHEN @INPUTID = MAX(CL.MATRICULA_DIRETOR) THEN 'DIRETOR' ");
            stb.Append("           WHEN @INPUTID = MAX(CL.MATRICULA_CEO) THEN 'CEO' ");
            stb.Append("           ELSE '' ");
            stb.Append("       END AS CARGO, ");
            stb.Append("	   MAX(CL.PERIODO) AS TURNO, ");
            stb.Append("       R.INDICADORID AS 'COD INDICADOR', ");
            stb.Append("       MAX(ISNULL(HIG1.MONETIZATION, 0)) AS META_MAXIMA, ");
            stb.Append("       MAX(ISNULL(HIG1.MONETIZATION_NIGHT, 0)) AS META_MAXIMA_NOTURNA, ");
            stb.Append("       MAX(ISNULL(HIG1.MONETIZATION_LATENIGHT, 0)) AS META_MAXIMA_MADRUGADA, ");
            stb.Append("       MAX(CL.CARGO) AS CARGO_RESULT ");
            stb.Append("FROM GDA_RESULT (NOLOCK) AS R ");
            stb.Append("LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS CB ON CB.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            stb.Append("LEFT JOIN ");
            stb.Append("  (SELECT CASE ");
            stb.Append("              WHEN IDGDA_SUBSECTOR IS NULL THEN IDGDA_SECTOR ");
            stb.Append("              ELSE IDGDA_SUBSECTOR ");
            stb.Append("          END AS IDGDA_SECTOR, ");
            stb.Append("          CREATED_AT, ");
            stb.Append("          IDGDA_COLLABORATORS, ");
            stb.Append("          ACTIVE, ");
            stb.Append("          CARGO, ");
            stb.Append("          PERIODO, ");
            stb.Append("          MATRICULA_SUPERVISOR, ");
            stb.Append("          MATRICULA_COORDENADOR, ");
            stb.Append("          MATRICULA_GERENTE_II, ");
            stb.Append("          MATRICULA_GERENTE_I, ");
            stb.Append("          MATRICULA_DIRETOR, ");
            stb.Append("          MATRICULA_CEO ");
            stb.Append("   FROM GDA_COLLABORATORS_DETAILS (NOLOCK) ");
            stb.Append("   WHERE CREATED_AT >= @DATAINICIAL ");
            stb.Append("     AND CREATED_AT <= @DATAFINAL ) AS CL ON CL.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS ");
            stb.Append("AND CL.CREATED_AT = R.CREATED_AT ");
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
            stb.Append("AND HIG1.INDICATOR_ID = R.INDICADORID ");
            stb.Append("AND HIG1.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.Append("AND HIG1.GROUPID = 1 ");
            stb.Append("AND CONVERT(DATE,HIG1.STARTED_AT) <= R.CREATED_AT ");
            stb.Append("AND CONVERT(DATE,HIG1.ENDED_AT) >= R.CREATED_AT ");
            stb.Append("LEFT JOIN GDA_RESULT (NOLOCK) AS TRAB ON R.IDGDA_COLLABORATORS = TRAB.IDGDA_COLLABORATORS ");
            stb.Append("AND R.CREATED_AT = TRAB.CREATED_AT ");
            stb.Append("AND TRAB.INDICADORID = 2 ");
            stb.Append("LEFT JOIN GDA_RESULT (NOLOCK) AS ESC ON R.IDGDA_COLLABORATORS = ESC.IDGDA_COLLABORATORS ");
            stb.Append("AND R.CREATED_AT = ESC.CREATED_AT ");
            stb.Append("AND ESC.INDICADORID = -1 ");
            stb.Append("WHERE 1 = 1 ");
            stb.Append("  AND R.CREATED_AT >= @DATAINICIAL ");
            stb.Append("  AND R.CREATED_AT <= @DATAFINAL ");
            stb.Append("  AND R.DELETED_AT IS NULL ");
            //stb.Append("  AND CL.active = 'true' ");
            stb.Append("  AND HIG1.MONETIZATION > 0 ");
            stb.Append("  AND (CL.IDGDA_COLLABORATORS = @INPUTID ");
            stb.Append("       OR CL.MATRICULA_SUPERVISOR = @INPUTID ");
            stb.Append("       OR CL.MATRICULA_COORDENADOR = @INPUTID ");
            stb.Append("       OR CL.MATRICULA_GERENTE_II = @INPUTID ");
            stb.Append("       OR CL.MATRICULA_GERENTE_I = @INPUTID ");
            stb.Append("       OR CL.MATRICULA_DIRETOR = @INPUTID ");
            stb.Append("       OR CL.MATRICULA_CEO = @INPUTID) ");
            stb.Append("  AND R.FACTORS <> '0.000000;0.000000' ");
            stb.Append("GROUP BY R.INDICADORID, ");
            stb.Append("         CONVERT(DATE, R.CREATED_AT) , ");
            stb.Append("         R.IDGDA_COLLABORATORS ");

            List<basketIndicatorResults> bir = new List<basketIndicatorResults>();
            basketIndicatorResults birFinal = new basketIndicatorResults();
            using (SqlConnection connection = new SqlConnection(Database.retornaConn(prod)))
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
                                basketIndicatorResults bi = new basketIndicatorResults();
                                bi.cargo = reader["CARGO"].ToString();
                                bi.cargoResult = reader["CARGO_RESULT"].ToString() == "" ? "Não Informado" : reader["CARGO_RESULT"].ToString();
                                bi.codIndicator = Convert.ToInt32(reader["COD INDICADOR"].ToString());
                                bi.dataPagamento = reader["CREATED_AT"].ToString();
                                if (reader["turno"].ToString() == "DIURNO")
                                {
                                    bi.moedasPossiveis = reader["META_MAXIMA"].ToString() != "" ? int.Parse(reader["META_MAXIMA"].ToString()) : 0;
                                }
                                else if (reader["turno"].ToString() == "NOTURNO")
                                {
                                    bi.moedasPossiveis = reader["META_MAXIMA_NOTURNA"].ToString() != "" ? int.Parse(reader["META_MAXIMA_NOTURNA"].ToString()) : 0;
                                }
                                else if (reader["turno"].ToString() == "MADRUGADA")
                                {
                                    bi.moedasPossiveis = reader["META_MAXIMA_MADRUGADA"].ToString() != "" ? int.Parse(reader["META_MAXIMA_MADRUGADA"].ToString()) : 0;
                                }
                                else if (reader["turno"].ToString() == "" || reader["turno"].ToString() == null)
                                {
                                    bi.moedasPossiveis = 0;
                                }
                                bi.idcollaborator = Convert.ToInt32(reader["IDGDA_COLLABORATORS"].ToString());
                                bi.diasTrabalhados = reader["TRAB"].ToString() != "" ? reader["TRAB"].ToString() : "-";
                                bi.diasEscalados = reader["ESC"].ToString() != "" ? reader["ESC"].ToString() : "-";
                                bir.Add(bi);
                            }
                        }
                    }
                    //RETIRANDO OS RESULTADOS DO SUPERVISOR.. ENTENDER COM A TAHTO COMO FICARA ESTA PARTE.
                    bir = bir.FindAll(item => item.cargoResult == "AGENTE" || item.cargoResult == "Não Informado").ToList();

                    //CASO NÃO RETORNE INFORMAÇÃO, RETORNAR ZERADO PARA NÃO DAR ERRO PRO FRONT
                    if (bir.Count() == 0)
                    {
                        rmams.coinsEarned = 0;
                        rmams.coinsPossible = 0;
                        rmams.groupName = "";
                        rmams.idGroup = 0;
                        rmams.groupAlias = "";
                        rmams.groupImage = "";
                        return rmams;
                    }

                    //AGRUPAMENTO EM DATA E INDICADOR
                    bir = bir.GroupBy(item => new { item.dataPagamento, item.codIndicator }).Select(grupo => new basketIndicatorResults
                    {
                        cargo = grupo.First().cargo,
                        codIndicator = grupo.Key.codIndicator,
                        dataPagamento = grupo.Key.dataPagamento,
                        moedasPossiveis = grupo.Max(item => item.diasTrabalhados) == "1"
                        ? Math.Round(grupo.Sum(item => item.moedasPossiveis), 2, MidpointRounding.AwayFromZero)
                        : grupo.Max(item => item.diasEscalados) == "0" && grupo.Max(item => item.diasTrabalhados) == "1" ?
                        Math.Round(grupo.Sum(item => item.moedasPossiveis), 2, MidpointRounding.AwayFromZero) :
                        grupo.Max(item => item.diasEscalados) == "0" && grupo.Max(item => item.diasTrabalhados) == "0" ?
                        0 : Math.Round(grupo.Sum(item => item.moedasPossiveis), 2, MidpointRounding.AwayFromZero),
                        qtdPessoas = grupo.Count(),
                    }).ToList();


                    if (bir.First().cargo == "AGENTE")
                    {
                        birFinal = bir
                            .GroupBy(item => new { item.cargo })
                            .Select(grupo => new basketIndicatorResults
                            {
                                moedasPossiveis = grupo.Sum(item => item.moedasPossiveis),
                                qtdPessoas = grupo.Count(),
                            }).First();
                    }
                    else
                    {
                        List<basketIndicatorResults> listHierarquia = new List<basketIndicatorResults>();
                        List<basketIndicatorResults> teste = new List<basketIndicatorResults>();
                        listHierarquia = bir
                            .GroupBy(item => new { item.codIndicator, item.dataPagamento })
                            .Select(grupo => new basketIndicatorResults
                            {
                                dataPagamento = grupo.Key.dataPagamento,
                                codIndicator = grupo.Key.codIndicator,
                                moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Sum(item => item.qtdPessoas), 2, MidpointRounding.AwayFromZero),
                                qtdPessoas = grupo.Count(),
                            }).ToList();

                        listHierarquia = listHierarquia
                            .GroupBy(item => new { item.codIndicator })
                            .Select(grupo => new basketIndicatorResults
                            {
                                codIndicator = grupo.Key.codIndicator,
                                moedasPossiveis = grupo.Sum(item => item.moedasPossiveis),
                                qtdPessoas = grupo.Count(),
                            }).ToList();

                        birFinal = listHierarquia
                            .GroupBy(item => new { item.cargo })
                            .Select(grupo => new basketIndicatorResults
                            {
                                codIndicator = 0,
                                moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis), 2, MidpointRounding.AwayFromZero),
                                qtdPessoas = grupo.Count(),
                            }).First();
                    }
                    rmams.coinsEarned = moedasGanhas;
                    rmams.coinsPossible = birFinal.moedasPossiveis;

                    //REALIZA CONTA
                    rmams.resultPercent = (rmams.coinsEarned / rmams.coinsPossible) * 100;

                    //COMO ELE NÃO TEVE COMO GANHAR NENHUMA MOEDA, ELE ATINGIU 100% DA META
                    if (rmams.coinsPossible == 0)
                    {
                        rmams.resultPercent = 100;
                    }
                    rmams.resultPercent = Math.Round(rmams.resultPercent, 2, MidpointRounding.AwayFromZero);

                    basket lbasket1 = lbasket.Find(l => l.group_id == 1);
                    basket lbasket2 = lbasket.Find(l => l.group_id == 2);
                    basket lbasket3 = lbasket.Find(l => l.group_id == 3);
                    basket lbasket4 = lbasket.Find(l => l.group_id == 4);
                    groups lgroup1 = lgroup.Find(l => l.id == 1);
                    groups lgroup2 = lgroup.Find(l => l.id == 2);
                    groups lgroup3 = lgroup.Find(l => l.id == 3);
                    groups lgroup4 = lgroup.Find(l => l.id == 4);
                    if (rmams.resultPercent >= lbasket1.metric_min)
                    {
                        rmams.groupName = lgroup1.name;
                        rmams.idGroup = lgroup1.id;
                        rmams.groupAlias = lgroup1.alias;
                        rmams.groupImage = lgroup1.image;
                    }
                    else if (rmams.resultPercent >= lbasket2.metric_min)
                    {
                        rmams.groupName = lgroup2.name;
                        rmams.idGroup = lgroup2.id;
                        rmams.groupAlias = lgroup2.alias;
                        rmams.groupImage = lgroup2.image;
                    }
                    else if (rmams.resultPercent >= lbasket3.metric_min)
                    {
                        rmams.groupName = lgroup3.name;
                        rmams.idGroup = lgroup3.id;
                        rmams.groupAlias = lgroup3.alias;
                        rmams.groupImage = lgroup3.image;
                    }
                    else if (rmams.resultPercent >= lbasket4.metric_min)
                    {
                        rmams.groupName = lgroup4.name;
                        rmams.idGroup = lgroup4.id;
                        rmams.groupAlias = lgroup4.alias;
                        rmams.groupImage = lgroup4.image;
                    }
                    else
                    {
                        rmams.groupName = lgroup4.name;
                        rmams.idGroup = lgroup4.id;
                        rmams.groupAlias = lgroup4.alias;
                        rmams.groupImage = lgroup4.image;
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            return rmams;
        }
        public static List<usuariosBk> returnUsers(string dtUltima, bool prod)
        {
            List<usuariosBk> users = new List<usuariosBk>();

            //QUERY MOEDAS GANHAS
            //string dtUltima = DateTime.Now.ToString("yyyy-MM-dd");
            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("SELECT IDGDA_COLLABORATORS, MAX(ID) AS ID ");
            stb.AppendFormat("FROM GDA_COLLABORATORS_DETAILS (NOLOCK) ");
            stb.AppendFormat("WHERE CREATED_AT >= '{0}' ", dtUltima);
            //stb.AppendFormat(" AND ACTIVE = 'true' ");
            stb.AppendFormat("GROUP BY IDGDA_COLLABORATORS ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn(prod)))
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
                                usuariosBk user = new usuariosBk();
                                user.idCollaborator = Convert.ToInt32(reader["IDGDA_COLLABORATORS"].ToString());
                                user.idTable = Convert.ToInt32(reader["ID"].ToString());
                                users.Add(user);
                            }
                        }
                    }
                }
                catch (Exception)
                {

                    //throw;
                }
                connection.Close();
            }
            return users;
        }

        public static List<basket> listBasketConfiguration(bool prod)
        {
            List<basket> lBasket = new List<basket>();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT METRIC_MIN, GROUPID FROM GDA_HISTORY_INDICATOR_GROUP (NOLOCK) ");
            sb.Append("WHERE INDICATOR_ID = 10000012 AND DELETED_AT IS NULL ");
            sb.Append("ORDER BY 1 DESC ");


            using (SqlConnection connection = new SqlConnection(Database.retornaConn(prod)))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                List<basket> lteste = lBasket.FindAll(l => l.group_id == int.Parse(reader["GROUPID"].ToString()));

                                if (lteste.Count() > 0)
                                {
                                    continue;
                                }
                                if (lBasket.Count() >= 4)
                                {
                                    break;
                                }

                                basket baskets = new basket();
                                baskets.group_id = int.Parse(reader["GROUPID"].ToString());
                                baskets.metric_min = int.Parse(reader["METRIC_MIN"].ToString());
                                lBasket.Add(baskets);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return lBasket;
        }

        public static List<groups> listGroups(string filter, bool prod)
        {
            List<groups> lGroups = new List<groups>();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT G.ID, NAME, ALIAS, URL FROM GDA_GROUPS (NOLOCK) AS G ");
            sb.Append("INNER JOIN GDA_UPLOADS (NOLOCK) AS U ON G.UPLOADID = U.ID ");


            using (SqlConnection connection = new SqlConnection(Database.retornaConn(prod)))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                groups group = new groups();
                                group.id = int.Parse(reader["ID"].ToString());
                                group.name = reader["NAME"].ToString();
                                group.alias = reader["ALIAS"].ToString();
                                group.image = reader["URL"].ToString();
                                lGroups.Add(group);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return lGroups;
        }

        public static void homologGlass()
        {
            // Lógica da tarefa agendada
            try
            {
                insereLog("HomologGlass", "Execute Started");
                using (SqlConnection connection = new SqlConnection(Database.ConnHomolog))
                {
                    //GDA_ATRIBUTES 
                    connection.Open();
                    StringBuilder stb = new StringBuilder();
                    stb.Append("INSERT INTO GDA_ATRIBUTES (IDGDA_COLLABORATORS, NAME, LEVEL, VALUE, CREATED_AT, DELETED_AT) ");
                    stb.Append("SELECT IDGDA_COLLABORATORS, NAME, LEVEL, VALUE, CREATED_AT, DELETED_AT ");
                    stb.Append("FROM OPENROWSET('SQLNCLI', 'Server=10.115.65.41;UID=TMP00001;PWD=JwhMLD7FNqZ9Eysn;', ");
                    stb.Append("               'SELECT IDGDA_COLLABORATORS, NAME, LEVEL, VALUE, CREATED_AT, DELETED_AT FROM GUILDA_PROD.dbo.gda_atributes (NOLOCK)  ");
                    stb.Append("			   WHERE CREATED_AT >= CONVERT(DATE, DATEADD(DAY, -1, GETDATE()))'); ");

                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.CommandTimeout = 180;
                        command.ExecuteNonQuery();
                    }

                    //GDA_HISTORY_HIERARCHY_RELATIONSHIP 
                    stb.Clear();
                    stb.Append("INSERT INTO GDA_HISTORY_HIERARCHY_RELATIONSHIP (CONTRACTORCONTROLID, PARENTIDENTIFICATION, IDGDA_COLLABORATORS, IDGDA_HIERARCHY, CREATED_AT, DELETED_AT, TRANSACTIONID, LEVELWEIGHT, DATE, LEVELNAME) ");
                    stb.Append("SELECT CONTRACTORCONTROLID, PARENTIDENTIFICATION, IDGDA_COLLABORATORS, IDGDA_HIERARCHY, CREATED_AT, DELETED_AT, TRANSACTIONID, LEVELWEIGHT, DATE, LEVELNAME ");
                    stb.Append("FROM OPENROWSET('SQLNCLI', 'Server=10.115.65.41;UID=TMP00001;PWD=JwhMLD7FNqZ9Eysn;', ");
                    stb.Append("               'SELECT CONTRACTORCONTROLID, PARENTIDENTIFICATION, IDGDA_COLLABORATORS, IDGDA_HIERARCHY, CREATED_AT, DELETED_AT, TRANSACTIONID, LEVELWEIGHT, DATE, LEVELNAME  ");
                    stb.Append("			   FROM GUILDA_PROD.dbo.GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) WHERE DATE >= CONVERT(DATE, DATEADD(DAY, -1, GETDATE()))'); ");

                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.CommandTimeout = 180;
                        command.ExecuteNonQuery();
                    }

                    //GDA_COLLABORATORS_DETAILS 
                    stb.Clear();
                    stb.Append("INSERT INTO GDA_COLLABORATORS_DETAILS (IDGDA_COLLABORATORS, CREATED_AT, IDGDA_SECTOR, HOME_BASED, SITE, PERIODO, MATRICULA_SUPERVISOR, NOME_SUPERVISOR, MATRICULA_COORDENADOR, NOME_COORDENADOR, MATRICULA_GERENTE_II, NOME_GERENTE_II, MATRICULA_GERENTE_I, NOME_GERENTE_I, MATRICULA_DIRETOR, NOME_DIRETOR, MATRICULA_CEO, NOME_CEO, CARGO, ACTIVE, IDGDA_SECTOR_SUPERVISOR, IDGDA_SUBSECTOR, IDGDA_CLIENT, IDGDA_GROUP, IDGDA_PERIOD) ");
                    stb.Append("SELECT IDGDA_COLLABORATORS, CREATED_AT, IDGDA_SECTOR, HOME_BASED, SITE, PERIODO, MATRICULA_SUPERVISOR, NOME_SUPERVISOR, MATRICULA_COORDENADOR, NOME_COORDENADOR, MATRICULA_GERENTE_II, NOME_GERENTE_II, MATRICULA_GERENTE_I, NOME_GERENTE_I, MATRICULA_DIRETOR, NOME_DIRETOR, MATRICULA_CEO, NOME_CEO, CARGO, ACTIVE, IDGDA_SECTOR_SUPERVISOR, IDGDA_SUBSECTOR, IDGDA_CLIENT, IDGDA_GROUP, IDGDA_PERIOD ");
                    stb.Append("FROM OPENROWSET('SQLNCLI', 'Server=10.115.65.41;UID=TMP00001;PWD=JwhMLD7FNqZ9Eysn;', ");
                    stb.Append("               'SELECT IDGDA_COLLABORATORS, CREATED_AT, IDGDA_SECTOR, HOME_BASED, SITE, PERIODO, MATRICULA_SUPERVISOR, NOME_SUPERVISOR, MATRICULA_COORDENADOR, NOME_COORDENADOR, MATRICULA_GERENTE_II, NOME_GERENTE_II, MATRICULA_GERENTE_I, NOME_GERENTE_I, MATRICULA_DIRETOR, NOME_DIRETOR, MATRICULA_CEO, NOME_CEO, CARGO, ACTIVE, IDGDA_SECTOR_SUPERVISOR, IDGDA_SUBSECTOR, IDGDA_CLIENT, IDGDA_GROUP, IDGDA_PERIOD FROM GUILDA_PROD.dbo.GDA_COLLABORATORS_DETAILS (NOLOCK) WHERE CREATED_AT >= CONVERT(DATE, DATEADD(DAY, -1, GETDATE()))'); ");

                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.CommandTimeout = 180;
                        command.ExecuteNonQuery();
                    }

                    //GDA_RESULT 
                    //stb.Clear();
                    //stb.Append("INSERT INTO GDA_RESULT (INDICADORID, TRANSACTIONID, RESULT, CREATED_AT, IDGDA_COLLABORATORS, FACTORS, DELETED_AT, FACTORSAG0, FACTORSAG1) ");
                    //stb.Append("SELECT INDICADORID, TRANSACTIONID, RESULT, CREATED_AT, IDGDA_COLLABORATORS, FACTORS, DELETED_AT, FACTORSAG0, FACTORSAG1 ");
                    //stb.Append("FROM OPENROWSET('SQLNCLI', 'Server=10.115.65.41;UID=TMP00001;PWD=JwhMLD7FNqZ9Eysn;',  ");
                    //stb.Append("               'SELECT INDICADORID, TRANSACTIONID, RESULT, CREATED_AT, IDGDA_COLLABORATORS, FACTORS, DELETED_AT, FACTORSAG0, FACTORSAG1 FROM GUILDA_PROD.dbo.GDA_RESULT (NOLOCK) WHERE DELETED_AT IS NULL AND CREATED_AT >= CONVERT(DATE, DATEADD(DAY, 0, GETDATE()))'); ");

                    //using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    //{
                    //    command.CommandTimeout = 180;
                    //    command.ExecuteNonQuery();
                    //}

                    //GDA_COLLABORATORS
                    //stb.Clear();
                    //stb.Append("   SET IDENTITY_INSERT GDA_COLLABORATORS ON; ");
                    //stb.Append("   INSERT INTO GDA_COLLABORATORS ([IDGDA_COLLABORATORS] ");
                    //stb.Append("      ,[NAME] ");
                    //stb.Append("      ,[COLLABORATORIDENTIFICATION] ");
                    //stb.Append("      ,[MATRICULA] ");
                    //stb.Append("      ,[GENRE] ");
                    //stb.Append("      ,[BIRTHDATE] ");
                    //stb.Append("      ,[ADMISSIONDATE] ");
                    //stb.Append("      ,[CIVILSTATE] ");
                    //stb.Append("      ,[ACTIVE] ");
                    //stb.Append("      ,[EMAIL] ");
                    //stb.Append("      ,[STREET] ");
                    //stb.Append("      ,[NUMBER] ");
                    //stb.Append("      ,[NEIGHBORHOOD] ");
                    //stb.Append("      ,[CITY] ");
                    //stb.Append("      ,[STATE] ");
                    //stb.Append("      ,[COUNTRY] ");
                    //stb.Append("      ,[HOMENUMBER] ");
                    //stb.Append("      ,[PHONENUMBER] ");
                    //stb.Append("      ,[SCHOOLING] ");
                    //stb.Append("      ,[CONTRACTORCONTROLID] ");
                    //stb.Append("      ,[DEPENDANTNUMBER] ");
                    //stb.Append("      ,[CREATED_AT] ");
                    //stb.Append("      ,[DELETED_AT] ");
                    //stb.Append("      ,[UPDATED_AT] ");
                    //stb.Append("      ,[TRANSACTIONID] ");
                    //stb.Append("      ,[ENTRYDATE] ");
                    //stb.Append("      ,[ENTRY_TIME] ");
                    //stb.Append("      ,[HOME_BASED] ");
                    //stb.Append("      ,[FIRST_LOGIN] ");
                    //stb.Append("      ,[PROFILE_COLLABORATOR_ADMINISTRATIONID] ");
                    //stb.Append("      ,[NEW_AGENT])  ");
                    //stb.Append("SELECT [IDGDA_COLLABORATORS] ");
                    //stb.Append("      ,[NAME] ");
                    //stb.Append("      ,[COLLABORATORIDENTIFICATION] ");
                    //stb.Append("      ,[MATRICULA] ");
                    //stb.Append("      ,[GENRE] ");
                    //stb.Append("      ,[BIRTHDATE] ");
                    //stb.Append("      ,[ADMISSIONDATE] ");
                    //stb.Append("      ,[CIVILSTATE] ");
                    //stb.Append("      ,[ACTIVE] ");
                    //stb.Append("      ,[EMAIL] ");
                    //stb.Append("      ,[STREET] ");
                    //stb.Append("      ,[NUMBER] ");
                    //stb.Append("      ,[NEIGHBORHOOD] ");
                    //stb.Append("      ,[CITY] ");
                    //stb.Append("      ,[STATE] ");
                    //stb.Append("      ,[COUNTRY] ");
                    //stb.Append("      ,[HOMENUMBER] ");
                    //stb.Append("      ,[PHONENUMBER] ");
                    //stb.Append("      ,[SCHOOLING] ");
                    //stb.Append("      ,[CONTRACTORCONTROLID] ");
                    //stb.Append("      ,[DEPENDANTNUMBER] ");
                    //stb.Append("      ,[CREATED_AT] ");
                    //stb.Append("      ,[DELETED_AT] ");
                    //stb.Append("      ,[UPDATED_AT] ");
                    //stb.Append("      ,[TRANSACTIONID] ");
                    //stb.Append("      ,[ENTRYDATE] ");
                    //stb.Append("      ,[ENTRY_TIME] ");
                    //stb.Append("      ,[HOME_BASED] ");
                    //stb.Append("      ,[FIRST_LOGIN] ");
                    //stb.Append("      ,[PROFILE_COLLABORATOR_ADMINISTRATIONID] ");
                    //stb.Append("      ,[NEW_AGENT] ");
                    //stb.Append("FROM OPENROWSET('SQLNCLI', 'Server=10.115.65.41;UID=TMP00001;PWD=JwhMLD7FNqZ9Eysn;',  ");
                    //stb.Append("               'SELECT [IDGDA_COLLABORATORS] ");
                    //stb.Append("      ,[NAME] ");
                    //stb.Append("      ,[COLLABORATORIDENTIFICATION] ");
                    //stb.Append("      ,[MATRICULA] ");
                    //stb.Append("      ,[GENRE] ");
                    //stb.Append("      ,[BIRTHDATE] ");
                    //stb.Append("      ,[ADMISSIONDATE] ");
                    //stb.Append("      ,[CIVILSTATE] ");
                    //stb.Append("      ,[ACTIVE] ");
                    //stb.Append("      ,[EMAIL] ");
                    //stb.Append("      ,[STREET] ");
                    //stb.Append("      ,[NUMBER] ");
                    //stb.Append("      ,[NEIGHBORHOOD] ");
                    //stb.Append("      ,[CITY] ");
                    //stb.Append("      ,[STATE] ");
                    //stb.Append("      ,[COUNTRY] ");
                    //stb.Append("      ,[HOMENUMBER] ");
                    //stb.Append("      ,[PHONENUMBER] ");
                    //stb.Append("      ,[SCHOOLING] ");
                    //stb.Append("      ,[CONTRACTORCONTROLID] ");
                    //stb.Append("      ,[DEPENDANTNUMBER] ");
                    //stb.Append("      ,[CREATED_AT] ");
                    //stb.Append("      ,[DELETED_AT] ");
                    //stb.Append("      ,[UPDATED_AT] ");
                    //stb.Append("      ,[TRANSACTIONID] ");
                    //stb.Append("      ,[ENTRYDATE] ");
                    //stb.Append("      ,[ENTRY_TIME] ");
                    //stb.Append("      ,[HOME_BASED] ");
                    //stb.Append("      ,[FIRST_LOGIN] ");
                    //stb.Append("      ,[PROFILE_COLLABORATOR_ADMINISTRATIONID] ");
                    //stb.Append("      ,[NEW_AGENT] FROM GUILDA_PROD.dbo.GDA_COLLABORATORS (NOLOCK)   ");
                    //stb.Append("			   ') AS IndicatorsFromDatabase1 ");
                    //stb.Append("where IDGDA_COLLABORATORS NOT IN (SELECT IDGDA_COLLABORATORS FROM GDA_COLLABORATORS (NOLOCK)) ");
                    //stb.Append("   SET IDENTITY_INSERT GDA_COLLABORATORS OFF; ");

                    //using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    //{
                    //    command.CommandTimeout = 180;
                    //    command.ExecuteNonQuery();
                    //}

                    //Quando descomentar, resolver questão da conexão do banco no inserir persona.
                    //criarPersonaNovosHomolog();

                    insereLog("HomologGlass", "Execute Concluded");
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                insereLog("HomologGlass", $"Execute Error: {ex.Message}");
            }
        }

        public static void criarPersonaNovosHomolog()
        {
            using (SqlConnection connection = new SqlConnection(Database.ConnHomolog))
            {
                connection.Open();
                StringBuilder stb = new StringBuilder();
                stb.Append("SELECT C.IDGDA_COLLABORATORS, C.NAME, C.COLLABORATORIDENTIFICATION, C.PHONENUMBER, C.BIRTHDATE, C.EMAIL, C.STATE, C.CITY FROM GDA_COLLABORATORS (NOLOCK) AS C ");
                stb.Append("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS CU ON CU.IDGDA_COLLABORATORS = C.IDGDA_COLLABORATORS ");
                stb.Append("WHERE CU.IDGDA_PERSONA_COLLABORATOR_USER IS NULL ");

                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string idgda_collaborators = reader["IDGDA_COLLABORATORS"] != DBNull.Value ? reader["IDGDA_COLLABORATORS"].ToString() : "";
                            string name = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "";
                            string identification = reader["COLLABORATORIDENTIFICATION"] != DBNull.Value ? reader["COLLABORATORIDENTIFICATION"].ToString() : "";
                            string phoneNumber = reader["PHONENUMBER"] != DBNull.Value ? reader["PHONENUMBER"].ToString() : "";
                            DateTime? birthDate = reader["BIRTHDATE"] != DBNull.Value ? Convert.ToDateTime(reader["BIRTHDATE"].ToString()) : (DateTime?)null;
                            string email = reader["EMAIL"] != DBNull.Value ? reader["EMAIL"].ToString() : "";
                            string state = reader["STATE"] != DBNull.Value ? reader["STATE"].ToString() : "";
                            string city = reader["CITY"] != DBNull.Value ? reader["CITY"].ToString() : "";

                            inserirPersona(idgda_collaborators, name, identification, phoneNumber, birthDate, email, state, city);
                        }
                    }
                }
                connection.Close();
            }
        }

        public static void inserirPersona(string idgda_collaborators, string name, string identification, string phoneNumber, DateTime? birthDate, string email, string state, string city)
        {
            SqlConnection connection = new SqlConnection(Database.Conn);
            try
            {

                connection.Open();

                //Verificar e inserir estado
                StringBuilder stbStateS = new StringBuilder();
                stbStateS.Append("SELECT IDGDA_STATE FROM GDA_STATE (NOLOCK) ");
                stbStateS.AppendFormat("WHERE STATE = '{0}' ", state);

                string codState = "";
                using (SqlCommand commandSelect = new SqlCommand(stbStateS.ToString(), connection))
                {
                    using (SqlDataReader reader = commandSelect.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            codState = reader["IDGDA_STATE"].ToString();
                        }
                    }
                }

                if (codState == "")
                {
                    StringBuilder stbStateInsert = new StringBuilder();
                    stbStateInsert.AppendFormat("INSERT INTO GDA_STATE (STATE, CREATED_AT)  ");
                    stbStateInsert.AppendFormat("VALUES ( ");
                    stbStateInsert.AppendFormat("'{0}', ", state);
                    stbStateInsert.AppendFormat("GETDATE() ");
                    stbStateInsert.AppendFormat(") SELECT @@IDENTITY AS 'CODSTATE' ");

                    using (SqlCommand commandInsert = new SqlCommand(stbStateInsert.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                codState = reader["CODSTATE"].ToString();
                            }
                        }
                    }
                }

                string codCity = "";
                StringBuilder stbSelectCity = new StringBuilder();
                stbSelectCity.Append("SELECT * FROM GDA_CITY (NOLOCK) ");
                stbSelectCity.Append("WHERE ");
                stbSelectCity.AppendFormat("IDGDA_STATE = {0} ", codState);
                stbSelectCity.AppendFormat("AND CITY = '{0}' ", city);

                using (SqlCommand commandInsert = new SqlCommand(stbSelectCity.ToString(), connection))
                {
                    using (SqlDataReader reader = commandInsert.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            codCity = reader["IDGDA_CITY"].ToString();
                        }
                    }
                }

                if (codCity == "")
                {
                    StringBuilder stbInsertCity = new StringBuilder();
                    stbInsertCity.Append("INSERT INTO GDA_CITY (IDGDA_STATE, CITY, CREATED_AT)  ");
                    stbInsertCity.Append("VALUES ( ");
                    stbInsertCity.AppendFormat("'{0}', ", codState);
                    stbInsertCity.AppendFormat("'{0}', ", city);
                    stbInsertCity.Append("GETDATE() ");
                    stbInsertCity.Append(") SELECT @@IDENTITY AS 'CODCITY'");

                    using (SqlCommand commandInsert = new SqlCommand(stbInsertCity.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                codCity = reader["CODCITY"].ToString();
                            }
                        }
                    }
                }

                //Inserir na rede social
                StringBuilder stbPersona = new StringBuilder();
                stbPersona.Append("INSERT INTO GDA_PERSONA_USER (IDGDA_PERSONA_USER_TYPE, IDGDA_PERSONA_USER_VISIBILITY, NAME, BC, SOCIAL_NAME, SHOW_AGE, PICTURE, CREATED_AT, DELETED_AT, CREATED_BY, DELETED_BY) ");
                stbPersona.Append("VALUES (1, "); //IDGDA_PERSONA_USER_TYPE = 1 = Pessoal
                stbPersona.Append("1, "); //IDGDA_PERSONA_USER_VISIBILITY = 1 = Publico
                stbPersona.AppendFormat("'{0}', ", name); //Name
                stbPersona.AppendFormat("'{0}', ", identification); //BC
                stbPersona.Append("'',  "); //SOCIAL_NAME
                stbPersona.Append("1, "); //SHOW_AGE
                stbPersona.Append("'', "); //PICTURE
                stbPersona.Append("GETDATE(), "); //CREATED_AT
                stbPersona.Append("NULL, "); //DELETED_AT
                stbPersona.Append("NULL, "); //CREATED_BY
                stbPersona.Append("NULL "); //DELETED_BY
                stbPersona.Append(") SELECT @@IDENTITY AS 'CODPERSONA' ");
                string codPersona = "";


                using (SqlCommand commandInsert = new SqlCommand(stbPersona.ToString(), connection))
                {
                    using (SqlDataReader reader = commandInsert.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            codPersona = reader["CODPERSONA"].ToString();
                        }
                    }
                }

                StringBuilder stbPersonaDetails = new StringBuilder();
                stbPersonaDetails.Append("INSERT INTO GDA_PERSONA_USER_DETAILS (IDGDA_PERSONA_USER, YOUR_MOTIVATIONS, GOALS, PHONE_NUMBER, BIRTH_DATE, IDGDA_STATE, IDGDA_CITY, WHO_IS, EMAIL) ");
                stbPersonaDetails.Append("VALUES ( ");
                stbPersonaDetails.AppendFormat("'{0}', ", codPersona); //IDGDA_PERSONA_USER
                stbPersonaDetails.Append("'',  "); //YOUR_MOTIVATIONS
                stbPersonaDetails.Append("'',  "); //GOALS
                stbPersonaDetails.AppendFormat("'{0}',  ", phoneNumber); // PHONE_NUMBER
                stbPersonaDetails.AppendFormat("'{0}',  ", birthDate?.ToString("yyyy-MM-dd")); //BIRTH_DATE
                stbPersonaDetails.AppendFormat("'{0}',  ", codState); //IDGDA_STATE
                stbPersonaDetails.AppendFormat("'{0}',  ", codCity); //IDGDA_CITY
                stbPersonaDetails.Append("'#Setor - #Cargo - #Moedas - #Grupo',  "); //WHO_IS
                stbPersonaDetails.AppendFormat("'{0}' ", email); //EMAIL
                stbPersonaDetails.Append(") ");
                SqlCommand createTableCommand = new SqlCommand(stbPersonaDetails.ToString(), connection);
                createTableCommand.ExecuteNonQuery();

                StringBuilder stbPersonaCollaborator = new StringBuilder();
                stbPersonaCollaborator.Append("INSERT INTO GDA_PERSONA_COLLABORATOR_USER (IDGDA_COLLABORATORS, IDGDA_PERSONA_USER) ");
                stbPersonaCollaborator.Append("VALUES ( ");
                stbPersonaCollaborator.AppendFormat("'{0}', ", Convert.ToInt32(idgda_collaborators)); //IDGDA_COLLABORATORS
                stbPersonaCollaborator.AppendFormat("'{0}' ", codPersona); //IDGDA_PERSONA_USER
                stbPersonaCollaborator.Append(") ");
                SqlCommand createTableCommand2 = new SqlCommand(stbPersonaCollaborator.ToString(), connection);
                createTableCommand2.ExecuteNonQuery();


            }
            catch (Exception)
            {

            }
            connection.Close();
        }




        public static List<List<T>> DividirLista<T>(this List<T> lista, int tamanho)
        {
            var numParticoes = (int)Math.Ceiling((double)lista.Count / tamanho);
            var particoes = new List<List<T>>(numParticoes);

            for (int i = 0; i < numParticoes; i++)
            {
                var particao = lista.Skip(i * tamanho).Take(tamanho).ToList();
                particoes.Add(particao);
            }

            return particoes;
        }
        public class returnResponseDaySchedule
        {
            public string DataPagamento { get; set; }
            public string Matricula { get; set; }

            public string NomeColaborador { get; set; }

            public string Cargo { get; set; }

            public string DataReferencia { get; set; }
            public string IDIndicador { get; set; }
            public string Indicador { get; set; }
            public string TipoIndicador { get; set; }


            public string Meta { get; set; }
            public string Resultado { get; set; }
            public string Percentual { get; set; }
            public double GanhoEmMoedas { get; set; }
            public string MetaMaximaMoedas { get; set; }
            public string MoedasExpiradas { get; set; }
            public string Grupo { get; set; }
            public DateTime DataAtualizacao { get; set; }

            public string MatriculaSupervisor { get; set; }
            public string NomeSupervisor { get; set; }
            public string MatriculaCoordenador { get; set; }
            public string NomeCoordenador { get; set; }
            public string MatriculaGerente2 { get; set; }
            public string NomeGerente2 { get; set; }
            public string MatriculaGerente1 { get; set; }
            public string NomeGerente1 { get; set; }
            public string MatriculaDiretor { get; set; }
            public string NomeDiretor { get; set; }
            public string MatriculaCEO { get; set; }
            public string NomeCEO { get; set; }
            public string CodigoGIP { get; set; }
            public string Setor { get; set; }
            public string CodigoGIPSubsetor { get; set; }
            public string Subsetor { get; set; }
            public string Home { get; set; }
            public string Turno { get; set; }
            public string Site { get; set; }
            public double Score { get; set; }
        }


        class retornoSaldoAcumulado
        {
            public string Mes { get; set; }


        }

        public static List<object> trataDadosRelatorio(List<object> dataB1, string relatorio)
        {
            List<object> dataRet = new List<object>();
            if (relatorio == "Monetização Diario")
            {
                List<ReportMonthDayController.returnResponseDay> dataB2 = dataB1.Cast<ReportMonthDayController.returnResponseDay>().ToList();
                var jsonData = dataB2.Select(item => new returnResponseDaySchedule
                {
                    DataPagamento = item.DataPagamento,
                    Matricula = item.Matricula,
                    NomeColaborador = item.NomeColaborador,
                    Cargo = item.Cargo,
                    DataReferencia = item.DataReferencia,
                    IDIndicador = item.IDIndicador,
                    Indicador = item.Indicador,
                    TipoIndicador = item.TipoIndicador,
                    Meta = item.Meta,
                    Resultado = item.Resultado,
                    Percentual = item.Percentual,
                    GanhoEmMoedas = item.GanhoEmMoedas,
                    MetaMaximaMoedas = item.MetaMaximaMoedas,
                    MoedasExpiradas = Convert.ToString(item.MoedasExpiradas),
                    Grupo = item.Grupo,
                    DataAtualizacao = item.DataAtualizacao, // Você pode precisar ajustar o formato da data
                    MatriculaSupervisor = item.MatriculaSupervisor,
                    NomeSupervisor = item.NomeSupervisor,
                    MatriculaCoordenador = item.MatriculaCoordenador,
                    NomeCoordenador = item.NomeCoordenador,
                    MatriculaGerente2 = item.MatriculaGerente2,
                    NomeGerente2 = item.NomeGerente2,
                    MatriculaGerente1 = item.MatriculaGerente1,
                    NomeGerente1 = item.NomeGerente1,
                    MatriculaDiretor = item.MatriculaDiretor,
                    NomeDiretor = item.NomeDiretor,
                    MatriculaCEO = item.MatriculaCEO,
                    NomeCEO = item.NomeCEO,
                    CodigoGIP = item.CodigoGIP,
                    Setor = item.Setor,
                    CodigoGIPSubsetor = item.CodigoGIPSubsetor,
                    Subsetor = item.Subsetor,
                    Home = item.Home,
                    Turno = item.Turno,
                    Site = item.Site,
                    Score = item.Score,
                }).ToList();

                dataRet = jsonData.Cast<object>().ToList();
            }

            return dataRet;
        }

        public static Dictionary<string, string> trataCabecalhosRelatorio(string relatorio)
        {
            //List<returnResponseConsolidated> rsc = new List<returnResponseConsolidated>();
            Dictionary<string, string> columnNames2 = new Dictionary<string, string>();

            if (relatorio == "Saldo Acumulado")
            {
                Dictionary<string, string> columnNames = new Dictionary<string, string>()
                {
                    { "Mes", "Mes" },
                    { "Ano", "Ano" },
                    { "Matricula", "Matricula" },
                    { "NomeColaborador", "Nome do colaborador" },
                    { "Cargo", "Cargo" },
                    { "EntradaSaida", "Entrada / Saida" },
                    { "Entrada", "Entrada" },
                    { "Saida", "Saida" },
                    { "Expirada", "Expirados" },
                    { "MatriculaSupervisor", "Matrícula Supervisor" },
                    { "NomeSupervisor", "Nome Supervisor" },
                    { "MatriculaCoordenador", "Matrícula Coordenador" },
                    { "NomeCoordenador", "Nome Coordenador" },
                    { "MatriculaGerente2", "Matrícula Gerente 2" },
                    { "NomeGerente2", "Nome Gerente 2" },
                    { "MatriculaGerente1", "Matrícula Gerente 1" },
                    { "NomeGerente1", "Nome Gerente 1" },
                    { "MatriculaDiretor", "Matrícula Diretor" },
                    { "NomeDiretor", "Nome Diretor" },
                    { "MatriculaCEO", "Matrícula CEO" },
                    { "NomeCEO", "Nome CEO" },
                    { "CodigoGIP", "Código GIP" },
                    { "Setor", "Setor" },
                    { "CodigoGIPSubsetor", "Código GIP Subsetor" },
                    { "Subsetor", "Subsetor" },
                    { "Home", "Home" },
                    { "Turno", "Turno" },
                    { "Site", "Site" },
                };
                columnNames2 = columnNames;
            }
            else if (relatorio == "Monetização Diario")
            {
                Dictionary<string, string> columnNames = new Dictionary<string, string>()
                {
                    { "DataPagamento",  "Data do pagamento" },
                    { "Matricula",  "Matricula" },
                    { "NomeColaborador",  "Nome do colaborador" },
                    { "Cargo",  "Cargo" },
                    { "DataReferencia",  "Referência pagamento" },
                    { "IDIndicador",  "Cod. Indicador" },
                    { "Indicador",  "Indicador" },
                    { "TipoIndicador",  "Tipo Indicador" },
                    { "Meta",  "Meta" },
                    { "Resultado",  "Resultado" },
                    { "Percentual", "Percentual" },
                    { "GanhoEmMoedas", "Ganho em Moedas" },
                    { "MetaMaximaMoedas", "Meta máxima de moedas" },
                    { "MoedasExpiradas", "Moedas Expiradas" },
                    { "Grupo", "Grupo" },
                    { "DataAtualizacao", "Data Atualização" },
                    { "MatriculaSupervisor", "Matrícula Supervisor" },
                    { "NomeSupervisor", "Nome Supervisor" },
                    { "MatriculaCoordenador", "Matrícula Coordenador" },
                    { "NomeCoordenador", "Nome Coordenador" },
                    { "MatriculaGerente2", "Matrícula Gerente 2" },
                    { "NomeGerente2", "Nome Gerente 2" },
                    { "MatriculaGerente1", "Matrícula Gerente 1" },
                    { "NomeGerente1", "Nome Gerente 1" },
                    { "MatriculaDiretor", "Matrícula Diretor" },
                    { "NomeDiretor", "Nome Diretor" },
                    { "MatriculaCEO", "Matrícula CEO" },
                    { "NomeCEO", "Nome CEO" },
                    { "CodigoGIP", "Código GIP" },
                    { "Setor", "Setor" },
                    { "CodigoGIPSubsetor", "Codigo GIP Subsetor" },
                    { "Subsetor", "Subsetor" },
                    { "Home", "Home" },
                    { "Turno", "Turno" },
                    { "Site", "Site" },
                    { "Score", "Score" },
                };
                columnNames2 = columnNames;
            }
            else if (relatorio == "Monetizacao_Consolidado")
            {
                Dictionary<string, string> columnNames = new Dictionary<string, string>()
                {
                    { "Mes",  "Mes" },
                    { "Ano",  "Ano" },
                    { "Matricula",  "Matricula" },
                    { "NomeColaborador",  "Nome do colaborador" },
                    { "Cargo",  "Cargo" },
                    { "DiasTrabalhados",  "Dias trabalhados" },
                    { "Indicador",  "Indicador" },
                    { "NomeIndicador",  "Nome do indicador" },
                    { "TipoIndicador",  "Tipo Indicador" },
                    { "Meta",  "Meta" },
                    { "Resultado", "Resultado" },
                    { "Percentual", "Percentual" },
                    { "GanhoEmMoedas", "Ganho em Moedas" },
                    { "MetaMaximaMoedas", "Meta máxima de moedas" },
                    { "MoedasExpiradas", "Moedas Expiradas" },
                    { "Grupo", "Grupo" },
                    { "DataAtualizacao", "Data de atualização" },
                    { "MatriculaSupervisor", "Matrícula Supervisor" },
                    { "NomeSupervisor", "Nome Supervisor" },
                    { "MatriculaCoordenador", "Matrícula Coordenador" },
                    { "NomeCoordenador", "Nome Coordenador" },
                    { "MatriculaGerente2", "Matrícula Gerente 2" },
                    { "NomeGerente2", "Nome Gerente 2" },
                    { "MatriculaGerente1", "Matrícula Gerente 1" },
                    { "NomeGerente1", "Nome Gerente 1" },
                    { "MatriculaDiretor", "Matrícula Diretor" },
                    { "NomeDiretor", "Nome Diretor" },
                    { "MatriculaCEO", "Matrícula CEO" },
                    { "NomeCEO", "Nome CEO" },
                    { "CodigoGIP", "Código GIP" },
                    { "Setor", "Setor" },
                    { "CodigoGIPSubsetor", "Código GIP SubSetor" },
                    { "Subsetor", "SubSetor" },
                    { "Home", "Home" },
                    { "Turno", "Turno" },
                    { "Site", "Site" },
                    { "Score", "Score" },

                };
                columnNames2 = columnNames;
            }
            else if (relatorio == "Resultado")
            {
                Dictionary<string, string> columnNames = new Dictionary<string, string>()
                {
                    { "Data",  "Data" },
                    { "Ano",  "Ano" },
                    { "Mes",  "Mes" },
                    { "MatriculaDoColaborador", "Matricula Do Colaborador" },
                    { "CodigoGIP",  "Codigo GIP" },
                    { "Setor",  "Setor" },
                    { "CodigoGIPSubsetor", "Código GIP SubSetor" },
                    { "Subsetor", "SubSetor" },
                    { "CodigoIndicador",  "Codigo Indicador" },
                    { "NomeIndicador",  "Nome Indicador" },
                    { "TipoIndicador",  "Tipo Indicador" },
                    { "Resultado",  "Resultado" },
                    { "Meta",  "Meta" },
                    { "PercentualDeAtingimento", "Percentual De Atingimento" },
                    { "Grupo", "Grupo" },
                    { "Cargo", "Cargo" },
                    { "NomeAgente", "Nome Agente" },
                    { "MatriculaSupervisor", "Matricula Supervisor" },
                    { "NomeSupervisor", "Nome Supervisor" },
                    { "MatriculaCoordenador", "Matricula Coordenador" },
                    { "NomeCoordenador", "Nome Coordenador" },
                    { "MatriculaGerente2", "Matricula Gerente 2" },
                    { "NomeGerente2", "Nome Gerente 2" },
                    { "MatriculaGerente1", "MatriculaGerente1" },
                    { "NomeGerente1", "Nome Gerente 1" },
                    { "MatriculaDiretor", "Matricula Diretor" },
                    { "NomeDiretor", "Nome Diretor" },
                    { "MatriculaCEO", "Matricula CEO" },
                    { "NomeCEO", "Nome CEO" },
                    { "StatusHome", "Status Home" },
                    { "Uf", "Uf" },
                    { "TurnoDoAgente", "Turno Do Agente" },
                    { "Score", "Score" },
                };
                columnNames2 = columnNames;
            }
            else if (relatorio == "Consolidado_setor")
            {
                Dictionary<string, string> columnNames = new Dictionary<string, string>()
                {
                    { "Mes",  "Mês" },
                    { "Ano",  "Ano" },
                    { "DataReferencia",  "Data de Referencia" },
                    { "Codigo_Gip",  "Código GIP" },
                    { "Setor",  "SETOR" },
                    { "CodigoGIPSubsetor",  "Código GIP SubSetor" },
                    { "Subsetor",  "SubSetor" },
                    { "Indicador",  "Indicador" },
                    { "NomeDoIndicador",  "Nome do indicador" },
                    { "TipoIndicador",  "Tipo Indicador" },
                    { "Meta",  "Meta" },
                    { "Resultado",  "Resultado" },
                    { "Percentual", "Percentual" },
                    { "GanhoEmMoedas", "Ganho em Moedas" },
                    { "MetaMaximaDeMoedas", "Meta máxima de moedas" },
                    { "Grupo", "Grupo" },
                    { "DataAtualizacao", "Data de Atualização" },
                    { "Site", "Site" },
                    { "MatriculaGerente1", "Matricula do Gerente 1" },
                    { "NomeGerente1", "Nome do Gerente 1" },
                    { "MatriculaGerente2", "Matricula do Gerente 2" },
                    { "NomeGerente2", "Nome do Gerente 2" },
                    { "MatriculaDiretor", "Matricula do Diretor" },
                    { "NomeDiretor", "Nome do Diretor" },
                    { "ContempladosDiamante", "Contemplados Diamante" },
                    { "ContempladosOuro", "Contemplados Ouro" },
                    { "ContempladosPrata", "Contemplados Prata" },
                    { "ContempladosBronze", "Contemplados Bronze" },
                    { "Score", "Score" },
                };
                columnNames2 = columnNames;
            }

            return columnNames2;
        }

        public static void InsertRelatorioFinanceiro(DateTime datainicio)
        {
            try
            {
                //Inserir o Relatorio Financeiro
                insereLog("Fechamento Caixa", "Execute Started");
                DatabaseService.InsertQueryResult("Fechamento Caixa", "", "");
                insereLog("Fechamento Caixa", "Execute Concluded");
            }

            catch (Exception ex)
            {
                insereLog("ReportsPreview", $"Execute Error: {ex.Message}");
            }


        }

        //public static void reportsPreview()
        //{
        //    string local = "";
        //    insereLog("ReportsPreview", "Execute Started");
        //    try
        //    {
        //        //data atual
        //        DateTime today = DateTime.Now;

        //        //primeiro dia do mes atual
        //        DateTime firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

        //        //primeiro dia do mes passado
        //        DateTime firstDayOfLastMonth = firstDayOfMonth.AddMonths(-1);

        //        //último dia do mês passado
        //        DateTime lastDayOfLastMonth = firstDayOfMonth.AddDays(-1);

        //        //
        //        int tamanhoMaximoPorLista = 500000; // 1.048.000

        //        string dtFinal = today.ToString("yyyy-MM-dd");
        //        string dtInicial = firstDayOfMonth.ToString("yyyy-MM-dd");

        //        string dtiFinalPassado = lastDayOfLastMonth.ToString("yyyy-MM-dd");
        //        string dtInicialPassado = firstDayOfLastMonth.ToString("yyyy-MM-dd");
        //        string month = DateTime.Now.ToString("MM");
        //        string year = DateTime.Now.Year.ToString();
        //        string yearPast = firstDayOfLastMonth.ToString("yyyy");
        //        string monthPast = firstDayOfLastMonth.ToString("MM");
        //        string NameFile = "";
        //        Dictionary<string, string> columnNames = new Dictionary<string, string>();

        //        #region Saldo Acumulado
        //        //Saldo Acumulado
        //        insereLog("Saldo_Acumulado", "Execute Started");
        //        NameFile = $"Relatorio_Saldo_Acumulado {month}-{year}";
        //        BucketClass.DeleteFiles(NameFile);
        //        List<object> dataA = DatabaseService.GetQueryResult("Saldo Acumulado", dtInicial, dtFinal);

        //        var listasDivididasA = DividirLista(dataA, tamanhoMaximoPorLista);
        //        List<objectsClass> dataQuebA = listasDivididasA.Select(lista => new objectsClass { listObjects = lista }).ToList();

        //        columnNames = trataCabecalhosRelatorio("Saldo Acumulado");
        //        if (dataQuebA.Count > 1)
        //        {
        //            int part = 0;
        //            foreach (ScheduledTask.objectsClass item in dataQuebA)
        //            {
        //                ExcelGenerator.DownloadAndGenerateExcel(item, NameFile + "Part_" + part + ".xlsx", columnNames);
        //                part += 1;
        //            }
        //        }
        //        else
        //        {
        //            ExcelGenerator.DownloadAndGenerateExcel(dataQuebA.First(), NameFile + ".xlsx", columnNames);
        //        }
        //        insereLog("Saldo_Acumulado", "Execute Concluded");

        //        //Saldo Acumulado do Mes Passado
        //        insereLog("Saldo_Acumulado", "Execute Started");
        //        NameFile = $"Relatorio_Saldo_Acumulado {monthPast}-{yearPast}";
        //        BucketClass.DeleteFiles(NameFile);
        //        List<object> dataA1 = DatabaseService.GetQueryResult("Saldo Acumulado", dtInicialPassado, dtiFinalPassado);

        //        var listasDivididasA1 = DividirLista(dataA1, tamanhoMaximoPorLista);
        //        List<objectsClass> dataQuebA1 = listasDivididasA1.Select(lista => new objectsClass { listObjects = lista }).ToList();

        //        columnNames = trataCabecalhosRelatorio("Saldo Acumulado");
        //        if (dataQuebA1.Count > 1)
        //        {
        //            int part = 0;
        //            foreach (ScheduledTask.objectsClass item in dataQuebA1)
        //            {
        //                ExcelGenerator.DownloadAndGenerateExcel(item, NameFile + "Part_" + part + ".xlsx", columnNames);
        //                part += 1;
        //            }
        //        }
        //        else
        //        {
        //            ExcelGenerator.DownloadAndGenerateExcel(dataQuebA1.First(), NameFile + ".xlsx", columnNames);
        //        }
        //        insereLog("Saldo_Acumulado", "Execute Concluded");

        //        #endregion

        //        #region Monetização Diario

        //        //Monitização Diario
        //        insereLog("Monetizacao_Diario", "Execute Started");
        //        NameFile = $"Relatorio_Monetizacao_Diario {month}-{year}";
        //        BucketClass.DeleteFiles(NameFile);
        //        List<object> dataB = DatabaseService.GetQueryResult("Monetização Diario", dtInicial, dtFinal);
        //        List<object> dataBt = trataDadosRelatorio(dataB, "Monetização Diario");

        //        var listasDivididasBt = DividirLista(dataBt, tamanhoMaximoPorLista);
        //        List<objectsClass> dataQuebBT = listasDivididasBt.Select(lista => new objectsClass { listObjects = lista }).ToList();

        //        columnNames = trataCabecalhosRelatorio("Monetização Diario");
        //        if (dataQuebBT.Count > 1)
        //        {
        //            int part = 0;
        //            foreach (ScheduledTask.objectsClass item in dataQuebBT)
        //            {
        //                ExcelGenerator.DownloadAndGenerateExcel(item, NameFile + "Part_" + part + ".xlsx", columnNames);
        //                part += 1;
        //            }
        //        }
        //        else
        //        {
        //            ExcelGenerator.DownloadAndGenerateExcel(dataQuebBT.First(), NameFile + ".xlsx", columnNames);
        //        }
        //        insereLog("Monetizacao_Diario", "Execute Concluded");

        //        //Monitização Diario Mes Passado
        //        insereLog("Monetizacao_Diario", "Execute Started");
        //        NameFile = $"Relatorio_Monetizacao_Diario {monthPast}-{yearPast}";
        //        BucketClass.DeleteFiles(NameFile);
        //        List<object> dataB1 = DatabaseService.GetQueryResult("Monetização Diario", dtInicialPassado, dtiFinalPassado);
        //        List<object> dataBt1 = trataDadosRelatorio(dataB1, "Monetização Diario");

        //        var listasDivididasBt1 = DividirLista(dataBt1, tamanhoMaximoPorLista);
        //        List<objectsClass> dataQuebBT1 = listasDivididasBt1.Select(lista => new objectsClass { listObjects = lista }).ToList();

        //        columnNames = trataCabecalhosRelatorio("Monetização Diario");
        //        if (dataQuebBT1.Count > 1)
        //        {
        //            int part = 0;
        //            foreach (ScheduledTask.objectsClass item in dataQuebBT1)
        //            {
        //                ExcelGenerator.DownloadAndGenerateExcel(item, NameFile + "Part_" + part + ".xlsx", columnNames);
        //                part += 1;
        //            }
        //        }
        //        else
        //        {
        //            ExcelGenerator.DownloadAndGenerateExcel(dataQuebBT1.First(), NameFile + ".xlsx", columnNames);
        //        }
        //        insereLog("Monetizacao_Diario", "Execute Concluded");
        //        #endregion

        //        #region Monetização Mensal
        //        //Monetização Mensal
        //        insereLog("Monetizacao_Consolidado", "Execute Started");
        //        NameFile = $"Relatorio_Monetizacao_Consolidado {month}-{year}";
        //        BucketClass.DeleteFiles(NameFile);
        //        List<object> dataC = DatabaseService.GetQueryResult("Monetizacao_Consolidado", dtInicial, dtFinal);

        //        var listasDivididasC = DividirLista(dataC, tamanhoMaximoPorLista);
        //        List<objectsClass> dataQuebC = listasDivididasC.Select(lista => new objectsClass { listObjects = lista }).ToList();

        //        columnNames = trataCabecalhosRelatorio("Monetizacao_Consolidado");
        //        if (dataQuebC.Count > 1)
        //        {
        //            int part = 0;
        //            foreach (ScheduledTask.objectsClass item in dataQuebC)
        //            {
        //                ExcelGenerator.DownloadAndGenerateExcel(item, NameFile + "Part_" + part + ".xlsx", columnNames);
        //                part += 1;
        //            }
        //        }
        //        else
        //        {
        //            ExcelGenerator.DownloadAndGenerateExcel(dataQuebC.First(), NameFile + ".xlsx", columnNames);
        //        }
        //        insereLog("Monetizacao_Consolidado", "Execute Concluded");

        //        //Monetização Mensal Mes Passado
        //        insereLog("Monetizacao_Consolidado", "Execute Started");
        //        NameFile = $"Relatorio_Monetizacao_Consolidado {monthPast}-{yearPast}";
        //        local = "1";
        //        BucketClass.DeleteFiles(NameFile);
        //        List<object> dataC1 = DatabaseService.GetQueryResult("Monetizacao_Consolidado", dtInicialPassado, dtiFinalPassado);
        //        local = "2";
        //        var listasDivididasC1 = DividirLista(dataC1, tamanhoMaximoPorLista);
        //        local = "3";
        //        List<objectsClass> dataQuebC1 = listasDivididasC1.Select(lista => new objectsClass { listObjects = lista }).ToList();
        //        local = "4";
        //        columnNames = trataCabecalhosRelatorio("Monetizacao_Consolidado");
        //        local = "5";
        //        if (dataQuebC1.Count > 1)
        //        {
        //            int part = 0;
        //            foreach (ScheduledTask.objectsClass item in dataQuebC1)
        //            {
        //                ExcelGenerator.DownloadAndGenerateExcel(item, NameFile + "Part_" + part + ".xlsx", columnNames);
        //                part += 1;
        //            }
        //        }
        //        else
        //        {
        //            ExcelGenerator.DownloadAndGenerateExcel(dataQuebC1.First(), NameFile + ".xlsx", columnNames);
        //        }
        //        insereLog("Monetizacao_Consolidado", "Execute Concluded");

        //        #endregion

        //        #region Monetização Resultado
        //        //Monetização Resultado
        //        insereLog("Resultado", "Execute Started");
        //        NameFile = $"Relatorio_Resultado {month}-{year}";
        //        BucketClass.DeleteFiles(NameFile);
        //        List<object> dataD = DatabaseService.GetQueryResult("Resultado", dtInicial, dtFinal);

        //        var listasDivididasD = DividirLista(dataD, tamanhoMaximoPorLista);
        //        List<objectsClass> dataQuebD = listasDivididasD.Select(lista => new objectsClass { listObjects = lista }).ToList();

        //        columnNames = trataCabecalhosRelatorio("Resultado");
        //        if (dataQuebD.Count > 1)
        //        {
        //            int part = 0;
        //            foreach (ScheduledTask.objectsClass item in dataQuebD)
        //            {
        //                ExcelGenerator.DownloadAndGenerateExcel(item, NameFile + "Part_" + part + ".xlsx", columnNames);
        //                part += 1;
        //            }
        //        }
        //        else
        //        {
        //            ExcelGenerator.DownloadAndGenerateExcel(dataQuebD.First(), NameFile + ".xlsx", columnNames);
        //        }
        //        insereLog("Resultado", "Execute Concluded");

        //        //Monitização Resultado mes Passado
        //        insereLog("Resultado", "Execute Started");
        //        NameFile = $"Relatorio_Resultado {monthPast}-{yearPast}";
        //        BucketClass.DeleteFiles(NameFile);
        //        List<object> dataD1 = DatabaseService.GetQueryResult("Resultado", dtInicialPassado, dtiFinalPassado);

        //        var listasDivididasD1 = DividirLista(dataD1, tamanhoMaximoPorLista);
        //        List<objectsClass> dataQuebD1 = listasDivididasD1.Select(lista => new objectsClass { listObjects = lista }).ToList();

        //        columnNames = trataCabecalhosRelatorio("Resultado");
        //        if (dataQuebD1.Count > 1)
        //        {
        //            int part = 0;
        //            foreach (ScheduledTask.objectsClass item in dataQuebD1)
        //            {
        //                ExcelGenerator.DownloadAndGenerateExcel(item, NameFile + "Part_" + part + ".xlsx", columnNames);
        //                part += 1;
        //            }
        //        }
        //        else
        //        {
        //            ExcelGenerator.DownloadAndGenerateExcel(dataQuebD1.First(), NameFile + ".xlsx", columnNames);
        //        }
        //        insereLog("Resultado", "Execute Concluded");





        //        #endregion

        //        #region Monetização Consolidado Setor
        //        //Monetização Consolidado Setor
        //        insereLog("Consolidado_setor", "Execute Started");
        //        NameFile = $"Relatorio_Consolidado_setor {month}-{year}";
        //        BucketClass.DeleteFiles(NameFile);
        //        List<object> dataE = DatabaseService.GetQueryResult("Consolidado_setor", dtInicial, dtFinal);

        //        var listasDivididasE = DividirLista(dataE, tamanhoMaximoPorLista);
        //        List<objectsClass> dataQuebE = listasDivididasE.Select(lista => new objectsClass { listObjects = lista }).ToList();

        //        columnNames = trataCabecalhosRelatorio("Consolidado_setor");
        //        if (dataQuebE.Count > 1)
        //        {
        //            int part = 0;
        //            foreach (ScheduledTask.objectsClass item in dataQuebE)
        //            {
        //                ExcelGenerator.DownloadAndGenerateExcel(item, NameFile + "Part_" + part + ".xlsx", columnNames);
        //                part += 1;
        //            }
        //        }
        //        else
        //        {
        //            ExcelGenerator.DownloadAndGenerateExcel(dataQuebE.First(), NameFile + ".xlsx", columnNames);
        //        }
        //        insereLog("Consolidado_setor", "Execute Concluded");

        //        //Monetização Consolidado Setor Mes Passado
        //        insereLog("Consolidado_setor", "Execute Started");
        //        NameFile = $"Relatorio_Consolidado_setor {monthPast}-{yearPast}";
        //        BucketClass.DeleteFiles(NameFile);
        //        List<object> dataE1 = DatabaseService.GetQueryResult("Consolidado_setor", dtInicialPassado, dtiFinalPassado);

        //        var listasDivididasE1 = DividirLista(dataE1, tamanhoMaximoPorLista);
        //        List<objectsClass> dataQuebE1 = listasDivididasE1.Select(lista => new objectsClass { listObjects = lista }).ToList();

        //        columnNames = trataCabecalhosRelatorio("Consolidado_setor");
        //        if (dataQuebE1.Count > 1)
        //        {
        //            int part = 0;
        //            foreach (ScheduledTask.objectsClass item in dataQuebE1)
        //            {
        //                ExcelGenerator.DownloadAndGenerateExcel(item, NameFile + "Part_" + part + ".xlsx", columnNames);
        //                part += 1;
        //            }
        //        }
        //        else
        //        {
        //            ExcelGenerator.DownloadAndGenerateExcel(dataQuebE1.First(), NameFile + ".xlsx", columnNames);
        //        }
        //        #endregion

        //        insereLog("Consolidado_setor", "Execute Concluded");

        //        insereLog("ReportsPreview", "Execute Concluded");
        //    }
        //    catch (Exception ex)
        //    {
        //        insereLog("ReportsPreview", $"Execute Error: {ex.Message} Local: {local}");
        //    }

        //}

        //public static void reportsPreview2(object state)
        //{
        //    string local = "";
        //    insereLog("ReportsPreview2", "Execute Started");
        //    try
        //    {
        //        //data atual
        //        DateTime today = DateTime.Now;

        //        //primeiro dia do mes atual
        //        DateTime firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

        //        //primeiro dia do mes passado
        //        DateTime firstDayOfLastMonth = firstDayOfMonth.AddMonths(-1);

        //        //último dia do mês passado
        //        DateTime lastDayOfLastMonth = firstDayOfMonth.AddDays(-1);

        //        //
        //        int tamanhoMaximoPorLista = 1000000; // 1.048.000

        //        string dtFinal = today.ToString("yyyy-MM-dd");
        //        string dtInicial = firstDayOfMonth.ToString("yyyy-MM-dd");

        //        string dtiFinalPassado = lastDayOfLastMonth.ToString("yyyy-MM-dd");
        //        string dtInicialPassado = firstDayOfLastMonth.ToString("yyyy-MM-dd");
        //        string month = DateTime.Now.ToString("MM");
        //        string year = DateTime.Now.Year.ToString();
        //        string yearPast = firstDayOfLastMonth.ToString("yyyy");
        //        string monthPast = firstDayOfLastMonth.ToString("MM");
        //        string NameFile = "";
        //        Dictionary<string, string> columnNames = new Dictionary<string, string>();

        //        #region Monetização Mensal
        //        //Monetização Mensal
        //        insereLog("Monetizacao_Consolidado", "Execute Started");
        //        NameFile = $"Relatorio_Monetizacao_Consolidado {month}-{year}";
        //        BucketClass.DeleteFiles(NameFile);
        //        List<object> dataC = DatabaseService.GetQueryResult("Monetizacao_Consolidado", dtInicial, dtFinal);

        //        var listasDivididasC = DividirLista(dataC, tamanhoMaximoPorLista);
        //        List<objectsClass> dataQuebC = listasDivididasC.Select(lista => new objectsClass { listObjects = lista }).ToList();

        //        columnNames = trataCabecalhosRelatorio("Monetizacao_Consolidado");
        //        if (dataQuebC.Count > 1)
        //        {
        //            int part = 0;
        //            foreach (ScheduledTask.objectsClass item in dataQuebC)
        //            {
        //                ExcelGenerator.DownloadAndGenerateExcel(item, NameFile + "Part_" + part + ".xlsx", columnNames);
        //                part += 1;
        //            }
        //        }
        //        else
        //        {
        //            ExcelGenerator.DownloadAndGenerateExcel(dataQuebC.First(), NameFile + ".xlsx", columnNames);
        //        }
        //        insereLog("Monetizacao_Consolidado", "Execute Concluded");

        //        //Monetização Mensal Mes Passado
        //        insereLog("Monetizacao_Consolidado", "Execute Started");
        //        NameFile = $"Relatorio_Monetizacao_Consolidado {monthPast}-{yearPast}";
        //        local = "1";
        //        BucketClass.DeleteFiles(NameFile);
        //        List<object> dataC1 = DatabaseService.GetQueryResult("Monetizacao_Consolidado", dtInicialPassado, dtiFinalPassado);
        //        local = "2";
        //        var listasDivididasC1 = DividirLista(dataC1, tamanhoMaximoPorLista);
        //        local = "3";
        //        List<objectsClass> dataQuebC1 = listasDivididasC1.Select(lista => new objectsClass { listObjects = lista }).ToList();
        //        local = "4";
        //        columnNames = trataCabecalhosRelatorio("Monetizacao_Consolidado");
        //        local = "5";
        //        if (dataQuebC1.Count > 1)
        //        {
        //            int part = 0;
        //            foreach (ScheduledTask.objectsClass item in dataQuebC1)
        //            {
        //                ExcelGenerator.DownloadAndGenerateExcel(item, NameFile + "Part_" + part + ".xlsx", columnNames);
        //                part += 1;
        //            }
        //        }
        //        else
        //        {
        //            ExcelGenerator.DownloadAndGenerateExcel(dataQuebC1.First(), NameFile + ".xlsx", columnNames);
        //        }
        //        insereLog("Monetizacao_Consolidado", "Execute Concluded");

        //        #endregion

        //        insereLog("ReportsPreview2", "Execute Concluded");
        //    }
        //    catch (Exception ex)
        //    {
        //        insereLog("ReportsPreview2", $"Execute Error: {ex.Message} Local: {local}");
        //    }


        //    try
        //    {
        //        DateTime ultimoDia = DateTime.Now;
        //        if (ultimoDia.Hour >= 22)
        //        {
        //            ultimoDia = DateTime.Now;

        //            ultimoDia = new DateTime(ultimoDia.Year, ultimoDia.Month, ultimoDia.Day, 22, 30, 0);
        //        }
        //        else
        //        {
        //            ultimoDia = DateTime.Now.AddDays(-1);

        //            ultimoDia = new DateTime(ultimoDia.Year, ultimoDia.Month, ultimoDia.Day, 22, 30, 0);
        //        }

        //        ultimoDia = ultimoDia.AddDays(1);
        //        DateTime now = DateTime.Now;
        //        int dueTime = (int)(ultimoDia - now).TotalMilliseconds;
        //        timerPreview2.Change(dueTime, 24 * 60 * 60 * 1000);
        //    }
        //    catch (Exception)
        //    {


        //    }

        //}


        public static void reportsPreview1(object state)
        {
            string local = "";
            insereLogProd("ReportsPreview", "Execute Started");
            try
            {
                //data atual
                DateTime today = DateTime.Now;

                //primeiro dia do mes atual
                DateTime firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

                //primeiro dia do mes passado
                DateTime firstDayOfLastMonth = firstDayOfMonth.AddMonths(-1);

                //último dia do mês passado
                DateTime lastDayOfLastMonth = firstDayOfMonth.AddDays(-1);

                //
                int tamanhoMaximoPorLista = 1000000; // 1.048.000

                string dtFinal = today.ToString("yyyy-MM-dd");
                string dtInicial = firstDayOfMonth.ToString("yyyy-MM-dd");

                string dtiFinalPassado = lastDayOfLastMonth.ToString("yyyy-MM-dd");
                string dtInicialPassado = firstDayOfLastMonth.ToString("yyyy-MM-dd");
                string month = DateTime.Now.ToString("MM");
                string year = DateTime.Now.Year.ToString();
                string yearPast = firstDayOfLastMonth.ToString("yyyy");
                string monthPast = firstDayOfLastMonth.ToString("MM");
                string NameFile = "";
                Dictionary<string, string> columnNames = new Dictionary<string, string>();

                #region Saldo Acumulado
                //Saldo Acumulado
                insereLogProd("Saldo_Acumulado", "Execute Started");
                NameFile = $"Relatorio_Saldo_Acumulado {month}-{year}";
                BucketClass.DeleteFiles(NameFile);
                List<object> dataA = DatabaseService.GetQueryResult("Saldo Acumulado", dtInicial, dtFinal);

                objectsClass dataClass = new objectsClass();
                dataClass.listObjects = dataA;

                columnNames = trataCabecalhosRelatorio("Saldo Acumulado");
                TxtGenerator.DownloadAndGenerateTXT(dataClass, NameFile + ".txt", columnNames);

                insereLogProd("Saldo_Acumulado", "Execute Concluded");

                //Saldo Acumulado do Mes Passado
                insereLogProd("Saldo_Acumulado", "Execute Started");
                NameFile = $"Relatorio_Saldo_Acumulado {monthPast}-{yearPast}";
                BucketClass.DeleteFiles(NameFile);
                List<object> dataA1 = DatabaseService.GetQueryResult("Saldo Acumulado", dtInicialPassado, dtiFinalPassado);

                objectsClass dataClass2 = new objectsClass();
                dataClass2.listObjects = dataA1;

                columnNames = trataCabecalhosRelatorio("Saldo Acumulado");
                TxtGenerator.DownloadAndGenerateTXT(dataClass2, NameFile + ".txt", columnNames);

                insereLogProd("Saldo_Acumulado", "Execute Concluded");

                #endregion



                insereLogProd("ReportsPreview", "Execute Concluded");
            }
            catch (Exception ex)
            {
                insereLogProd("ReportsPreview", $"Execute Error: {ex.Message} Local: {local}");
            }

            try
            {
                DateTime ultimoDia = DateTime.Now;
                if (ultimoDia.Hour >= 22)
                {
                    ultimoDia = DateTime.Now;

                    ultimoDia = new DateTime(ultimoDia.Year, ultimoDia.Month, ultimoDia.Day, 22, 30, 0);
                }
                else
                {
                    ultimoDia = DateTime.Now.AddDays(-1);

                    ultimoDia = new DateTime(ultimoDia.Year, ultimoDia.Month, ultimoDia.Day, 22, 30, 0);
                }

                ultimoDia = ultimoDia.AddDays(1);
                DateTime now = DateTime.Now;
                int dueTime = (int)(ultimoDia - now).TotalMilliseconds;
                timerPreview1.Change(dueTime, 24 * 60 * 60 * 1000);
            }
            catch (Exception)
            {


            }
        }

        public static void reportsPreview2(object state)
        {
            string local = "";
            insereLogProd("ReportsPreview2", "Execute Started");
            try
            {
                //data atual
                DateTime today = DateTime.Now;

                //primeiro dia do mes atual
                DateTime firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

                //primeiro dia do mes passado
                DateTime firstDayOfLastMonth = firstDayOfMonth.AddMonths(-1);

                //último dia do mês passado
                DateTime lastDayOfLastMonth = firstDayOfMonth.AddDays(-1);

                //
                int tamanhoMaximoPorLista = 1000000; // 1.048.000

                string dtFinal = today.ToString("yyyy-MM-dd");
                string dtInicial = firstDayOfMonth.ToString("yyyy-MM-dd");

                string dtiFinalPassado = lastDayOfLastMonth.ToString("yyyy-MM-dd");
                string dtInicialPassado = firstDayOfLastMonth.ToString("yyyy-MM-dd");
                string month = DateTime.Now.ToString("MM");
                string year = DateTime.Now.Year.ToString();
                string yearPast = firstDayOfLastMonth.ToString("yyyy");
                string monthPast = firstDayOfLastMonth.ToString("MM");
                string NameFile = "";
                Dictionary<string, string> columnNames = new Dictionary<string, string>();

                #region Monetização Mensal
                //Monetização Mensal
                insereLogProd("Monetizacao_Consolidado", "Execute Started");
                NameFile = $"Relatorio_Monetizacao_Consolidado {month}-{year}";
                BucketClass.DeleteFiles(NameFile);
                List<object> dataC = DatabaseService.GetQueryResult("Monetizacao_Consolidado", dtInicial, dtFinal);


                objectsClass dataQuebC = new objectsClass();
                dataQuebC.listObjects = dataC;

                columnNames = trataCabecalhosRelatorio("Monetizacao_Consolidado");
                TxtGenerator.DownloadAndGenerateTXT(dataQuebC, NameFile + ".txt", columnNames);

                insereLogProd("Monetizacao_Consolidado", "Execute Concluded");

                //Monetização Mensal Mes Passado
                insereLogProd("Monetizacao_Consolidado", "Execute Started");
                NameFile = $"Relatorio_Monetizacao_Consolidado {monthPast}-{yearPast}";

                BucketClass.DeleteFiles(NameFile);
                List<object> dataC1 = DatabaseService.GetQueryResult("Monetizacao_Consolidado", dtInicialPassado, dtiFinalPassado);

                objectsClass dataQuebC1 = new objectsClass();
                dataQuebC1.listObjects = dataC1;

                columnNames = trataCabecalhosRelatorio("Monetizacao_Consolidado");
                TxtGenerator.DownloadAndGenerateTXT(dataQuebC1, NameFile + ".txt", columnNames);

                insereLogProd("Monetizacao_Consolidado", "Execute Concluded");

                #endregion

                insereLogProd("ReportsPreview2", "Execute Concluded");
            }
            catch (Exception ex)
            {
                insereLogProd("ReportsPreview2", $"Execute Error: {ex.Message} Local: {local}");
            }


            try
            {
                DateTime ultimoDia = DateTime.Now;
                if (ultimoDia.Hour >= 22)
                {
                    ultimoDia = DateTime.Now;

                    ultimoDia = new DateTime(ultimoDia.Year, ultimoDia.Month, ultimoDia.Day, 22, 30, 0);
                }
                else
                {
                    ultimoDia = DateTime.Now.AddDays(-1);

                    ultimoDia = new DateTime(ultimoDia.Year, ultimoDia.Month, ultimoDia.Day, 22, 30, 0);
                }

                ultimoDia = ultimoDia.AddDays(1);
                DateTime now = DateTime.Now;
                int dueTime = (int)(ultimoDia - now).TotalMilliseconds;
                timerPreview2.Change(dueTime, 24 * 60 * 60 * 1000);
            }
            catch (Exception)
            {


            }

        }

        public static void reportsPreview3(object state)
        {
            string local = "";
            insereLogProd("ReportsPreview3", "Execute Started");
            try
            {
                //data atual
                DateTime today = DateTime.Now;

                //primeiro dia do mes atual
                DateTime firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

                //primeiro dia do mes passado
                DateTime firstDayOfLastMonth = firstDayOfMonth.AddMonths(-1);

                //último dia do mês passado
                DateTime lastDayOfLastMonth = firstDayOfMonth.AddDays(-1);

                //
                int tamanhoMaximoPorLista = 500000; // 1.048.000

                string dtFinal = today.ToString("yyyy-MM-dd");
                string dtInicial = firstDayOfMonth.ToString("yyyy-MM-dd");

                string dtiFinalPassado = lastDayOfLastMonth.ToString("yyyy-MM-dd");
                string dtInicialPassado = firstDayOfLastMonth.ToString("yyyy-MM-dd");
                string month = DateTime.Now.ToString("MM");
                string year = DateTime.Now.Year.ToString();
                string yearPast = firstDayOfLastMonth.ToString("yyyy");
                string monthPast = firstDayOfLastMonth.ToString("MM");
                string NameFile = "";
                Dictionary<string, string> columnNames = new Dictionary<string, string>();

                //insereLogProd("WeekMonth", "Execute Started");

                //performanceSemanalMensal.processoSemanalMensal();

                //insereLogProd("WeekMonth", "Execute Concluded");

                #region Monetização Diario

                //Monitização Diario
                insereLogProd("Monetizacao_Diario", "Execute Started");
                NameFile = $"Relatorio_Monetizacao_Diario {month}-{year}";
                BucketClass.DeleteFiles(NameFile);
                List<object> dataB = DatabaseService.GetQueryResult("Monetização Diario", dtInicial, dtFinal);
                List<object> dataBt = trataDadosRelatorio(dataB, "Monetização Diario");

                objectsClass dataClass3 = new objectsClass();
                dataClass3.listObjects = dataBt;

                columnNames = trataCabecalhosRelatorio("Monetização Diario");
                TxtGenerator.DownloadAndGenerateTXT(dataClass3, NameFile + ".txt", columnNames);
                //AttTablePerformance(dataB, firstDayOfMonth.ToString("MM"), firstDayOfMonth.ToString("yyyy"), dtInicial, dtFinal);


                insereLogProd("Monetizacao_Diario", "Execute Concluded");

                //Monitização Diario Mes Passado
                insereLogProd("Monetizacao_Diario", "Execute Started");
                NameFile = $"Relatorio_Monetizacao_Diario {monthPast}-{yearPast}";
                BucketClass.DeleteFiles(NameFile);
                List<object> dataB1 = DatabaseService.GetQueryResult("Monetização Diario", dtInicialPassado, dtiFinalPassado);
                List<object> dataBt1 = trataDadosRelatorio(dataB1, "Monetização Diario");

                objectsClass dataClass4 = new objectsClass();
                dataClass4.listObjects = dataBt1;

                columnNames = trataCabecalhosRelatorio("Monetização Diario");
                TxtGenerator.DownloadAndGenerateTXT(dataClass4, NameFile + ".txt", columnNames);
                //AttTablePerformance(dataB, firstDayOfLastMonth.ToString("MM"), firstDayOfLastMonth.ToString("yyyy"), dtInicialPassado, dtiFinalPassado);

                insereLogProd("Monetizacao_Diario", "Execute Concluded");
                #endregion


                #region Monetização Resultado
                //Monetização Resultado
                insereLogProd("Resultado", "Execute Started");
                NameFile = $"Relatorio_Resultado {month}-{year}";
                BucketClass.DeleteFiles(NameFile);
                List<object> dataD = DatabaseService.GetQueryResult("Resultado", dtInicial, dtFinal);



                objectsClass dataQuebD = new objectsClass();
                dataQuebD.listObjects = dataD;

                columnNames = trataCabecalhosRelatorio("Resultado");

                TxtGenerator.DownloadAndGenerateTXT(dataQuebD, NameFile + ".txt", columnNames);

                insereLogProd("Resultado", "Execute Concluded");

                //Monitização Resultado mes Passado
                insereLogProd("Resultado", "Execute Started");
                NameFile = $"Relatorio_Resultado {monthPast}-{yearPast}";
                BucketClass.DeleteFiles(NameFile);
                List<object> dataD1 = DatabaseService.GetQueryResult("Resultado", dtInicialPassado, dtiFinalPassado);


                objectsClass dataQuebD1 = new objectsClass();
                dataQuebD1.listObjects = dataD1;

                columnNames = trataCabecalhosRelatorio("Resultado");

                TxtGenerator.DownloadAndGenerateTXT(dataQuebD1, NameFile + ".txt", columnNames);

                insereLogProd("Resultado", "Execute Concluded");

                #endregion


                insereLogProd("ReportsPreview3", "Execute Concluded");


            }
            catch (Exception ex)
            {
                insereLogProd("ReportsPreview3", $"Execute Error: {ex.Message} Local: {local}");
            }

            try
            {
                DateTime ultimoDia = DateTime.Now;
                if (ultimoDia.Hour >= 22)
                {
                    ultimoDia = DateTime.Now;

                    ultimoDia = new DateTime(ultimoDia.Year, ultimoDia.Month, ultimoDia.Day, 23, 0, 0);
                }
                else
                {
                    ultimoDia = DateTime.Now.AddDays(-1);

                    ultimoDia = new DateTime(ultimoDia.Year, ultimoDia.Month, ultimoDia.Day, 23, 0, 0);
                }

                ultimoDia = ultimoDia.AddDays(1);
                DateTime now = DateTime.Now;
                int dueTime = (int)(ultimoDia - now).TotalMilliseconds;
                timerPreview3.Change(dueTime, 24 * 60 * 60 * 1000);
            }
            catch (Exception)
            {


            }
        }

        public static void AttTablePerformance(List<object> lt, string mes, string ano, string dtInicial, string dtFinal)
        {
            using (SqlConnection connection = new SqlConnection(Database.ConnPerformance))
            {
                //GDA_ATRIBUTES 


                StringBuilder stbDel = new StringBuilder();
                stbDel.AppendFormat("DELETE TOP (10000) FROM GDA_PERFORMANCE WHERE ");
                stbDel.AppendFormat($"DATA >= '{dtInicial}' ");
                stbDel.AppendFormat($"AND DATA <= '{dtFinal}' ");

                int rowsAffected;

                using (SqlCommand command = new SqlCommand(stbDel.ToString(), connection))
                {
                    connection.Open();

                    do
                    {
                        rowsAffected = command.ExecuteNonQuery();

                    } while (rowsAffected > 0);

                    connection.Close();
                }

                foreach (ReportMonthDayController.returnResponseDay item in lt)
                {
                    try
                    {


                        connection.Open();
                        StringBuilder stb = new StringBuilder();
                        stb.AppendFormat("INSERT INTO GDA_PERFORMANCE ");
                        stb.AppendFormat("(");
                        stb.AppendFormat("DATA,");
                        stb.AppendFormat("IDENTIFICACAO,");
                        stb.AppendFormat("CHAVE_EXTERNA,");
                        stb.AppendFormat("NOME_USUARIO, ");
                        stb.AppendFormat("NOME_NIVEL_HIERARQUIA, ");
                        stb.AppendFormat("IDENTIFICACAO_SUPERVISOR,");//     
                        stb.AppendFormat("CHAVE_EXTERNA_SUPERVISOR,");
                        stb.AppendFormat("NOME_SUPERVISOR, ");
                        stb.AppendFormat("IDENTIFICACAO_COORDENADOR,");
                        stb.AppendFormat("CHAVE_EXTERNA_COORDENADOR,");
                        stb.AppendFormat("NOME_COORDENADOR, ");
                        stb.AppendFormat("IDENTIFICACAO_GERENTE_II,");
                        stb.AppendFormat("CHAVE_EXTERNA_GERENTE_II,");
                        stb.AppendFormat("NOME_GERENTE_II, ");
                        stb.AppendFormat("IDENTIFICACAO_GERENTE_I,");
                        stb.AppendFormat("CHAVE_EXTERNA_GERENTE_I,");
                        stb.AppendFormat("NOME_GERENTE_I, ");
                        stb.AppendFormat("IDENTIFICACAO_DIRETOR,");
                        stb.AppendFormat("CHAVE_EXTERNA_DIRETOR,");
                        stb.AppendFormat("NOME_DIRETOR, ");
                        stb.AppendFormat("IDENTIFICACAO_CEO,");
                        stb.AppendFormat("CHAVE_EXTERNA_CEO,");
                        stb.AppendFormat("NOME_CEO, ");
                        stb.AppendFormat("CD_GIP,");
                        stb.AppendFormat("SETOR, ");
                        stb.AppendFormat("ID_INDICADOR,");
                        stb.AppendFormat("NOME_INDICADOR, ");
                        stb.AppendFormat("RESULTADO,");
                        stb.AppendFormat("FATOR, ");
                        stb.AppendFormat("FATOR_1,");
                        stb.AppendFormat("FATOR_2,");
                        stb.AppendFormat("RESULTADO_CALCULADO,");
                        stb.AppendFormat("PERCENTUAL_ATINGIMENTO,");
                        stb.AppendFormat("META,");
                        stb.AppendFormat("GANHO,");
                        stb.AppendFormat("MAX_GANHO,");
                        stb.AppendFormat("ID_GRUPO,");
                        stb.AppendFormat("NOME_GRUPO, ");
                        stb.AppendFormat("NOME_NIVEL, ");
                        stb.AppendFormat("DATA_ATUALIZACAO,");
                        stb.AppendFormat("HOME_BASED,");
                        stb.AppendFormat("LOCAL, ");
                        stb.AppendFormat("NOVATO, ");
                        stb.AppendFormat("SITE");
                        stb.AppendFormat(")");
                        stb.AppendFormat("VALUES");
                        stb.AppendFormat("(");
                        stb.AppendFormat($"'{Convert.ToDateTime(item.DataReferencia).ToString("yyyy-MM-dd")}', "); //DATA
                        stb.AppendFormat($"'BC{item.Matricula}', "); //IDENTIFICACAO
                        stb.AppendFormat($"'{item.Matricula}', "); //CHAVE_EXTERNA
                        stb.AppendFormat($"'{item.NomeColaborador}', "); //NOME_USUARIO
                        stb.AppendFormat($"'{item.Cargo}', "); //NOME_NIVEL_HIERARQUIA
                        stb.AppendFormat($"'BC{item.MatriculaSupervisor}', "); //IDENTIFICACAO_SUPERVISOR
                        stb.AppendFormat($"'{item.MatriculaSupervisor}', "); //CHAVE_EXTERNA_SUPERVISOR
                        stb.AppendFormat($"'{item.NomeSupervisor}', "); //NOME_SUPERVISOR
                        stb.AppendFormat($"'BC{item.MatriculaCoordenador}', "); //IDENTIFICACAO_COORDENADOR
                        stb.AppendFormat($"'{item.MatriculaCoordenador}', "); //CHAVE_EXTERNA_COORDENADOR
                        stb.AppendFormat($"'{item.NomeCoordenador}', "); //NOME_COORDENADOR
                        stb.AppendFormat($"'BC{item.MatriculaGerente2}', "); //IDENTIFICACAO_GERENTE_II
                        stb.AppendFormat($"'{item.MatriculaGerente2}', "); //CHAVE_EXTERNA_GERENTE_II
                        stb.AppendFormat($"'{item.NomeGerente2}', "); //NOME_GERENTE_II
                        stb.AppendFormat($"'BC{item.MatriculaGerente1}', "); //IDENTIFICACAO_GERENTE_I
                        stb.AppendFormat($"'{item.MatriculaGerente1}', "); //CHAVE_EXTERNA_GERENTE_I
                        stb.AppendFormat($"'{item.NomeGerente1}', "); //NOME_GERENTE_I
                        stb.AppendFormat($"'BC{item.MatriculaDiretor}', "); //IDENTIFICACAO_DIRETOR
                        stb.AppendFormat($"'{item.MatriculaDiretor}', "); //CHAVE_EXTERNA_DIRETOR
                        stb.AppendFormat($"'{item.NomeDiretor}', "); //NOME_DIRETOR
                        stb.AppendFormat($"'BC{item.MatriculaCEO}', "); //IDENTIFICACAO_CEO
                        stb.AppendFormat($"'{item.MatriculaCEO}', "); //CHAVE_EXTERNA_CEO
                        stb.AppendFormat($"'{item.NomeCEO}', "); //NOME_CEO
                        stb.AppendFormat($"'{item.CodigoGIP}', "); //CD_GIP
                        stb.AppendFormat($"'{item.Setor}', "); //SETOR
                        stb.AppendFormat($"'{item.IDIndicador}', "); //ID_INDICADOR
                        stb.AppendFormat($"'{item.Indicador}', "); //NOME_INDICADOR
                        stb.AppendFormat($"'{item.Resultado.Replace(",", ".")}', "); //RESULTADO
                        stb.AppendFormat($"'{item.fatores}', "); //FATOR
                        stb.AppendFormat($"'{item.fator0}', "); //FATOR_1
                        stb.AppendFormat($"'{item.fator1}', "); //FATOR_2
                        stb.AppendFormat($"'{item.Resultado.Replace(",", ".")}', "); //RESULTADO_CALCULADO
                        stb.AppendFormat($"'{item.Percentual.Replace(",", ".")}', "); //PERCENTUAL_ATINGIMENTO
                        stb.AppendFormat($"'{item.Meta.Replace(",", ".")}', "); //META
                        stb.AppendFormat($"'{item.GanhoEmMoedas}', "); //GANHO
                        stb.AppendFormat($"'{item.MetaMaximaMoedas}', "); //MAX_GANHO
                        stb.AppendFormat($"'{item.idGrupo}', "); //ID_GRUPO
                        stb.AppendFormat($"'{item.aliasGrupo}', "); //NOME_GRUPO
                        stb.AppendFormat($"'{item.Grupo}', "); //NOME_NIVEL
                        stb.AppendFormat($"GETDATE(), "); //DATA_ATUALIZACAO
                        stb.AppendFormat($"'{item.Home}', "); //HOME_BASED
                        stb.AppendFormat($"'', "); //LOCAL
                        stb.AppendFormat($"'', "); //NOVATO
                        stb.AppendFormat($"'{item.Site}' "); //SITE
                        stb.AppendFormat(")");

                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        insereLogProd("ReportsPreview3", $"Execute Error AttTablePerformance: {ex.Message} ");
                    }
                    connection.Close();
                }


            }
        }

        public static void reportsPreview4(object state)
        {
            string local = "";
            insereLogProd("ReportsPreview4", "Execute Started");
            try
            {
                //data atual
                DateTime today = DateTime.Now;

                //primeiro dia do mes atual
                DateTime firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

                //primeiro dia do mes passado
                DateTime firstDayOfLastMonth = firstDayOfMonth.AddMonths(-1);

                //último dia do mês passado
                DateTime lastDayOfLastMonth = firstDayOfMonth.AddDays(-1);

                //
                int tamanhoMaximoPorLista = 500000; // 1.048.000

                string dtFinal = today.ToString("yyyy-MM-dd");
                string dtInicial = firstDayOfMonth.ToString("yyyy-MM-dd");

                string dtiFinalPassado = lastDayOfLastMonth.ToString("yyyy-MM-dd");
                string dtInicialPassado = firstDayOfLastMonth.ToString("yyyy-MM-dd");
                string month = DateTime.Now.ToString("MM");
                string year = DateTime.Now.Year.ToString();
                string yearPast = firstDayOfLastMonth.ToString("yyyy");
                string monthPast = firstDayOfLastMonth.ToString("MM");
                string NameFile = "";
                Dictionary<string, string> columnNames = new Dictionary<string, string>();

                #region Monetização Consolidado Setor
                //Monetização Consolidado Setor
                insereLogProd("Consolidado_setor", "Execute Started");
                NameFile = $"Relatorio_Consolidado_setor {month}-{year}";
                BucketClass.DeleteFiles(NameFile);
                List<object> dataE = DatabaseService.GetQueryResult("Consolidado_setor", dtInicial, dtFinal);

                objectsClass dataQuebE = new objectsClass();
                dataQuebE.listObjects = dataE;

                columnNames = trataCabecalhosRelatorio("Consolidado_setor");

                TxtGenerator.DownloadAndGenerateTXT(dataQuebE, NameFile + ".txt", columnNames);

                insereLogProd("Consolidado_setor", "Execute Concluded");

                //Monetização Consolidado Setor Mes Passado
                insereLogProd("Consolidado_setor", "Execute Started");
                NameFile = $"Relatorio_Consolidado_setor {monthPast}-{yearPast}";
                BucketClass.DeleteFiles(NameFile);
                List<object> dataE1 = DatabaseService.GetQueryResult("Consolidado_setor", dtInicialPassado, dtiFinalPassado);

                objectsClass dataQuebE1 = new objectsClass();
                dataQuebE1.listObjects = dataE1;

                columnNames = trataCabecalhosRelatorio("Consolidado_setor");

                TxtGenerator.DownloadAndGenerateTXT(dataQuebE1, NameFile + ".txt", columnNames);

                insereLogProd("Consolidado_setor", "Execute Concluded");
                #endregion

                insereLogProd("ReportsPreview4", "Execute Concluded");
            }
            catch (Exception ex)
            {
                insereLogProd("ReportsPreview4", $"Execute Error: {ex.Message} Local: {local}");
            }

            try
            {
                DateTime ultimoDia = DateTime.Now;
                if (ultimoDia.Hour >= 22)
                {
                    ultimoDia = DateTime.Now;

                    ultimoDia = new DateTime(ultimoDia.Year, ultimoDia.Month, ultimoDia.Day, 23, 30, 0);
                }
                else
                {
                    ultimoDia = DateTime.Now.AddDays(-1);

                    ultimoDia = new DateTime(ultimoDia.Year, ultimoDia.Month, ultimoDia.Day, 23, 30, 0);
                }

                ultimoDia = ultimoDia.AddDays(1);
                DateTime now = DateTime.Now;
                int dueTime = (int)(ultimoDia - now).TotalMilliseconds;
                timerPreview4.Change(dueTime, 24 * 60 * 60 * 1000);
            }
            catch (Exception)
            {


            }

        }

        public static void Stop()
        {
            insereLog("HomologGlass", "Stop");
            // Parar o timer
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }





    }
}