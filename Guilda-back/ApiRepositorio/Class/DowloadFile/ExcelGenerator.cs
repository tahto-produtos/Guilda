using ClosedXML.Excel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;

namespace ApiC.Class.DowloadFile
{
    public class ExcelGenerator
    {
        public static int DownloadAndGenerateExcel(ScheduledTask.objectsClass data, string nameFile, Dictionary<string, string> columnNames)
        {
            // Lógica para gerar o arquivo Excel
           
            byte[] excelData = GenerateExcel(data, columnNames);

            //Salvar as informações no Bucket
            string[] types = { ".xlsx" };
            BucketClass.UploadToAzureBlob(excelData, nameFile);

            return 0;
        }

        //public static byte[] GenerateExcel(List<object> data)
        //{


        //    using (var workbook = new XLWorkbook())
        //    {
        //        var worksheet = workbook.Worksheets.Add("Sheet1");


        //        // Adicione os dados à planilha
        //        worksheet.Cell(1, 1).InsertData(data);

        //        // Salve o arquivo Excel em um MemoryStream
        //        using (var stream = new MemoryStream())
        //        {
        //            workbook.SaveAs(stream);
        //            return stream.ToArray();
        //        }
        //    }
        //}

        public static byte[] GenerateExcel(ScheduledTask.objectsClass data, Dictionary<string, string> columnNames)
        {

            using (var workbook = new XLWorkbook())
            {

                var dados = data.listObjects;

                var worksheet = workbook.Worksheets.Add("Sheet1");
            
                // Adiciona o cabeçalho à planilha
                var linhaCabecalho = worksheet.Row(1);
                linhaCabecalho.Style.Font.Bold = true;
                int indiceColuna = 1;
                foreach (var nomeColuna in columnNames.Values)
                {
                    worksheet.Cell(1, indiceColuna).Value = nomeColuna;
                    indiceColuna++;
                }

                // Inicia a partir da segunda linha para os dados
                int linhaAtual = 2;

                // Adiciona os dados à planilha a partir da segunda linha
                worksheet.Cell(2, 1).InsertData(dados);

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }

        }



    }
}