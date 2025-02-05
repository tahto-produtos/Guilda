using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace ApiC.Class.DowloadFile
{
    public class ScheduledTask
    {
        private static Timer timer;

        public static void Start()
        {
            // Configurar o timer para chamar o método ExecuteTask todos os dias às 2 da manhã
            DateTime now = DateTime.Now;
            DateTime scheduledTime = new DateTime(now.Year, now.Month, now.Day, 01, 00, 0);

            if (now > scheduledTime)
            {
                scheduledTime = scheduledTime.AddDays(1);
            }

            int dueTime = (int)(scheduledTime - now).TotalMilliseconds;

            timer = new Timer(ExecuteTask, null, dueTime, 24 * 60 * 60 * 1000);
        }

        private static void ExecuteTask(object state)
        {
            // Lógica da tarefa agendada
            try
            {
                //Regra de nomes dos relatorios a serem removidos e criados


                //Remover o atual
                //BucketClass.DeleteFiles("teste.xlsx");

                // Lógica para salvar o atual
                //List<ModelsEx.homeRel> data = DatabaseService.GetQueryResult();
                //ExcelGenerator.DownloadAndGenerateExcel(data, "teste.xlsx");
            }
            catch (Exception)
            {

            }
         }

        public static void Stop()
        {
            // Parar o timer
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }
}