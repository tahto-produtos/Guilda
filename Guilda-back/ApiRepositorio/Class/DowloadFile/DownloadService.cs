using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiC.Class.DowloadFile
{
    public class DownloadService
    {
        public static int DownloadAndGenerateExcel()
        {
            // Lógica para obter dados da consulta SQL
            var data = DatabaseService.GetQueryResult();

            // Lógica para gerar o arquivo Excel
            byte[] excelData = ExcelGenerator.GenerateExcel(data);

            // Salve as informações na tabela do banco de dados
            int fileId = DatabaseService.SaveExcelFile(excelData);

            return fileId;
        }



    }
}