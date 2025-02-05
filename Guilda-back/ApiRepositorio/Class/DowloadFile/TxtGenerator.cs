using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace ApiC.Class.DowloadFile
{
    public class TxtGenerator
    {
        public static int DownloadAndGenerateTXT(ScheduledTask.objectsClass data, string nameFile, Dictionary<string, string> columnNames)
        {
            // Lógica para gerar o arquivo Excel

            byte[] excelData = GenerateTXT(data, columnNames);

            //Salvar as informações no Bucket
            string[] types = { ".txt" };
            BucketClass.UploadToAzureBlob(excelData, nameFile);

            return 0;
        }


        public static string FormatPercentualDeAtingimento(string percentualDeAtingimento)
        {
            try
            {
                percentualDeAtingimento = percentualDeAtingimento.Replace(",", ".");
                if (!string.IsNullOrEmpty(percentualDeAtingimento) && decimal.TryParse(percentualDeAtingimento, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
                {
                    // Formata o número com duas casas decimais e substitui o ponto por vírgula
                    string valorFormatado = valor.ToString("F2", CultureInfo.InvariantCulture).Replace(".", ",");
                    return valorFormatado + "%";
                }
                else
                {
                    // Se o valor for "-", retorna sem o símbolo de porcentagem
                    if (percentualDeAtingimento == "-")
                    {
                        return percentualDeAtingimento;
                    }
                    // Caso contrário, retorna o valor original com o símbolo de porcentagem
                    else
                    {
                        return percentualDeAtingimento + "%";
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }
            
        }

        public static byte[] GenerateTXT(ScheduledTask.objectsClass data, Dictionary<string, string> columnNames)
        {
            var dados = data.listObjects;
            var sb = new StringBuilder();



            // Adiciona o cabeçalho
            sb.AppendLine(string.Join(";", columnNames.Values));

            // Adiciona os dados
            foreach (var obj in dados)
            {
                var values = new List<string>();
                foreach (var columnName in columnNames.Keys)
                {
                    var property = obj.GetType().GetProperty(columnName);
                    if (property != null)
                    {
                        var value = property.GetValue(obj)?.ToString() ?? string.Empty;

                        if (columnName == "PercentualDeAtingimento")
                        {
                            value = FormatPercentualDeAtingimento(value);
                        }

                        values.Add(value);
                    }
                    else
                    {
                        values.Add(string.Empty);
                    }
                }
                sb.AppendLine(string.Join(";", values));
            }


            // Converte o StringBuilder para um array de bytes
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

    }
}