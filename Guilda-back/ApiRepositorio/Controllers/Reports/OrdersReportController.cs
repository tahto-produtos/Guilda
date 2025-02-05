using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ApiRepositorio.Class;
using ApiRepositorio.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Formatting;
using System.Web;
using Microsoft.Extensions.Primitives;
using ApiC.Class;
using System.Runtime.ConstrainedExecution;
using static ApiRepositorio.Controllers.BasketGeneralUserController;
using System.Drawing;
using OfficeOpenXml.Style;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class OrdersReportController : ApiController
    {
        public class OrdersReport
        {
            public int CODIGO { get; set; }
            public int CRIADO_POR { get; set; }
            public string STATUS { get; set; }
            public string CRIADO_EM { get; set; }
            public string COLABORADOR { get; set; }
            public string CARGO { get; set; }
            public int MATRICULA_SUPERVISOR { get; set; }
            public string NOME_SUPERVISOR { get; set; }
            public int MATRICULA_COORDENADOR { get; set; }
            public string NOME_COORDENADOR { get; set; }
            public int MATRICULA_GERENTE_II { get; set; }
            public string NOME_GERENTE_II { get; set; }
            public int MATRICULA_GERENTE_I { get; set; }
            public string NOME_GERENTE_I { get; set; }
            public int MATRICULA_DIRETOR { get; set; }
            public string NOME_DIRETOR { get; set; }
            public int MATRICULA_CEO { get; set; }
            public string NOME_CEO { get; set; }
            public string GRUPO { get; set; }
            public string UF { get; set; }
            public int COD_GIP { get; set; }
            public string SETOR { get; set; }
            public string HOME { get; set; }
            public string TIPO_DE_PRODUTO { get; set; }
            public string NOME_DO_PRODUTO { get; set; }
            public int QUANTIDADE { get; set; }
            public double VALOR_EM_MOEDAS { get; set; }
            public double VALOR_EM_MOEDAS_TOTAL { get; set; }
            public string ULTIMA_ATUALIZACAO { get; set; }
            public string QUEM_ATUALIZOU { get; set; }
            public string ENTREGUE_POR { get; set; }
            public string BC_COLABORADOR { get; set; }
            public string LIBERADO_POR { get; set; }
            public string OBSERVACAO_LIBERACAO { get; set; }
            public string QUEM_VAI_RETIRAR { get; set; }
            public string QUEM_RETIROU { get; set; }
            public string OBSERVACAO_DE_ENTREGA { get; set; }
            public string CANCELADO_POR { get; set; }
            public string OBSERVACAO_CANCELAMENTO { get; set; }
            //public string ESTOQUE { get; set; }
            public int ESTOQUE { get; set; }
        }
        public class returnResponseDay
        {
            public int CODIGO { get; set; }
            public int CRIADO_POR { get; set; }
            public string STATUS { get; set; }
            public string CRIADO_EM { get; set; }
            public string COLABORADOR { get; set; }
            public string CARGO { get; set; }
            public int MATRICULA_SUPERVISOR { get; set; }
            public string NOME_SUPERVISOR { get; set; }
            public int MATRICULA_COORDENADOR { get; set; }
            public string NOME_COORDENADOR { get; set; }
            public int MATRICULA_GERENTE_II { get; set; }
            public string NOME_GERENTE_II { get; set; }
            public int MATRICULA_GERENTE_I { get; set; }
            public string NOME_GERENTE_I { get; set; }
            public int MATRICULA_DIRETOR { get; set; }
            public string NOME_DIRETOR { get; set; }
            public int MATRICULA_CEO { get; set; }
            public string NOME_CEO { get; set; }
            public string GRUPO { get; set; }
            public string UF { get; set; }
            public int COD_GIP { get; set; }
            public string SETOR { get; set; }
            public string HOME { get; set; }
            public string TIPO_DE_PRODUTO { get; set; }
            public string NOME_DO_PRODUTO { get; set; }
            public int QUANTIDADE { get; set; }
            public double VALOR_EM_MOEDAS { get; set; }
            public double VALOR_EM_MOEDAS_TOTAL { get; set; }
            public string ULTIMA_ATUALIZACAO { get; set; }
            public string QUEM_ATUALIZOU { get; set; }
            public string ENTREGUE_POR { get; set; }
            public string BC_COLABORADOR { get; set; }
            public string LIBERADO_POR { get; set; }
            public string OBSERVACAO_LIBERACAO { get; set; }
            public string QUEM_VAI_RETIRAR { get; set; }
            public string QUEM_RETIROU { get; set; }
            public string OBSERVACAO_DE_ENTREGA { get; set; }
            public string CANCELADO_POR { get; set; }
            public string OBSERVACAO_CANCELAMENTO { get; set; }
            //public string ESTOQUE { get; set; }
            public int ESTOQUE { get; set; }
        }

            [HttpPost]
        [ResponseType(typeof(ResultsModel))]
        public IHttpActionResult PostResultsModel()
        {
            string datainicio = HttpContext.Current.Request.Form["DATAINICIO"];
            string datafim = HttpContext.Current.Request.Form["DATAFIM"];
            string tipo = HttpContext.Current.Request.Form["TIPO"];
            string setor = HttpContext.Current.Request.Form["SETOR"];
            string uf = HttpContext.Current.Request.Form["UF"];
            string quantidade = HttpContext.Current.Request.Form["QUANTIDADE"];
            string status = HttpContext.Current.Request.Form["STATUS"];
            string ordervendas = HttpContext.Current.Request.Form["ORDEM_VENDAS"];
            string produto = HttpContext.Current.Request.Form["NOME_DO_PRODUTO"];
            string where = "WHERE ";
            string order = "ORDER BY ";

            if (!string.IsNullOrEmpty(ordervendas))
            {
                if (ordervendas == "1")
                {
                    order += $"VALOR_EM_MOEDAS_TOTAL DESC ";
                }
                else
                {
                    order += $"VALOR_EM_MOEDAS_TOTAL ASC";
                }
            }
            else
            {
                order += "  CODIGO";
            }

            if (!string.IsNullOrEmpty(datainicio.Trim()) && !string.IsNullOrEmpty(datafim.Trim()))
            {
                where += $"(CONVERT(DATE, [CRIADO_EM]) >= @DATAINICIAL AND CONVERT(DATE, [CRIADO_EM]) <= @DATAFINAL) ";
            }

            if (!string.IsNullOrEmpty(tipo))
            {
                where += (where == "WHERE ") ? $"([TIPO_DE_PRODUTO] = '{tipo}') " : $"AND ([TIPO_DE_PRODUTO] = '{tipo}') ";
            }

            if (!string.IsNullOrEmpty(setor))
            {
                where += (where == "WHERE ") ? $"([COD_GIP] = '{setor}') " : $"AND ([COD_GIP] = '{setor}') ";
                //where += (where == "WHERE ") ? $"([SETOR] = '{setor}') " : $"AND ([SETOR] = '{setor}') ";
            }

            if (!string.IsNullOrEmpty(uf))
            {
                where += (where == "WHERE ") ? $"([UF] = '{uf}') " : $"AND ([UF] = '{uf}') ";
            }

            if (!string.IsNullOrEmpty(status))
            {
                where += (where == "WHERE ") ? $"([STATUS] = '{status}') " : $"AND ([STATUS] = '{status}') ";
            }

            if (!string.IsNullOrEmpty(quantidade))
            {
                where += (where == "WHERE ") ? $"([QUANTIDADE] = {quantidade}) " : $"AND ([QUANTIDADE] = {quantidade}) ";
            }

            if (!string.IsNullOrEmpty(produto))
            {
                where += (where == "WHERE ") ? $"([NOME_DO_PRODUTO] LIKE '%{produto}%') " : $"AND ([NOME_DO_PRODUTO] LIKE '%{produto}%') ";
            }
            _ = (where == "WHERE ") ? where = "" : where;


            #region QUERYANTIGA

            //StringBuilder stb = new StringBuilder();
            //stb.Append("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = FORMAT(DATEADD(DAY, -5, GETDATE()), 'yyyy-MM-dd') ; ");
            //stb.Append(" ");
            //stb.Append("SELECT * FROM(");
            //stb.Append("SELECT O.COD_ORDER AS CODIGO, ");
            //stb.Append("          MAX(O.ORDER_BY) AS 'CRIADO_POR', ");
            //stb.Append("          MAX(S.STATUS) AS STATUS, ");
            //stb.Append("          MAX(O.CREATED_AT) AS 'CRIADO_EM', ");
            //stb.Append("          MAX(O.ORDER_BY) AS 'COLABORADOR', ");
            //stb.Append("          MAX(LEVELNAME) AS 'CARGO', ");
            //stb.Append("          MAX(CASE ");
            //stb.Append("                  WHEN HIERARCHY.LEVELWEIGHT = '2' THEN HIERARCHY.NAME ");
            //stb.Append("                  ELSE '-' ");
            //stb.Append("              END) AS 'NOME_SUPERVISOR', ");
            //stb.Append("          MAX(CASE ");
            //stb.Append("                  WHEN HIERARCHY.LEVELWEIGHT = '3' THEN HIERARCHY.NAME ");
            //stb.Append("                  ELSE '-' ");
            //stb.Append("              END) AS 'NOME_COORDENADOR', ");
            //stb.Append("          MAX(CASE ");
            //stb.Append("                  WHEN HIERARCHY.LEVELWEIGHT = '4' THEN HIERARCHY.NAME ");
            //stb.Append("                  ELSE '-' ");
            //stb.Append("              END) AS 'NOME_GERENTE_II', ");
            //stb.Append("          MAX(CASE ");
            //stb.Append("                  WHEN HIERARCHY.LEVELWEIGHT = '5' THEN HIERARCHY.NAME ");
            //stb.Append("                  ELSE '-' ");
            //stb.Append("              END) AS 'NOME_GERENTE_I', ");
            //stb.Append("          MAX(CASE ");
            //stb.Append("                  WHEN HIERARCHY.LEVELWEIGHT = '6' THEN HIERARCHY.NAME ");
            //stb.Append("                  ELSE '-' ");
            //stb.Append("              END) AS 'NOME_DIRETOR', ");
            //stb.Append("          MAX(CASE ");
            //stb.Append("                  WHEN HIERARCHY.LEVELWEIGHT = '7' THEN HIERARCHY.NAME ");
            //stb.Append("                  ELSE '-' ");
            //stb.Append("              END) AS 'NOME_CEO', ");
            //stb.Append("          '' AS GRUPO, ");
            //stb.Append("          MAX(CASE ");
            //stb.Append("                  WHEN A.NAME = 'SITE' THEN A.VALUE ");
            //stb.Append("                  ELSE '' ");
            //stb.Append("              END) AS UF, ");
            //stb.Append("          MAX(HCS.IDGDA_SECTOR) AS 'COD_GIP', ");
            //stb.Append("          MAX(SEC.NAME) AS 'SETOR', ");
            //stb.Append("          MAX(CASE ");
            //stb.Append("                  WHEN A.NAME = 'HOME_BASED' THEN A.VALUE ");
            //stb.Append("                  ELSE '' ");
            //stb.Append("              END) AS HOME, ");
            //stb.Append("          P.TYPE AS 'TIPO_DE_PRODUTO', ");
            //stb.Append("          P.COMERCIAL_NAME AS 'NOME_DO_PRODUTO', ");
            //stb.Append("          OP.AMOUNT AS 'QUANTIDADE', ");
            //stb.Append("          P.PRICE AS 'VALOR_EM_MOEDAS', ");
            //stb.Append("          OP.AMOUNT * P.PRICE AS 'VALOR_EM_MOEDAS_TOTAL', ");
            //stb.Append("          MAX(O.LAST_UPDATED_AT) AS 'ULTIMA_ATUALIZACAO', ");
            //stb.Append("          MAX(COL.NAME) AS 'QUEM_ATUALIZOU', ");
            //stb.Append("          MAX(COL2.NAME) AS 'ENTREGUE_POR', ");
            //stb.Append("          MAX(COL3.COLLABORATORIDENTIFICATION) AS 'BC_COLABORADOR', ");
            //stb.Append("          MAX(O.OBSERVATION_ORDER) AS 'QUEM_VAI_RETIRAR', ");
            //stb.Append("          MAX(O.OBSERVATION_DELIVERED) AS 'QUEM_RETIROU', ");
            //stb.Append("          P.QUANTITY AS 'ESTOQUE' ");
            //stb.Append("   FROM GDA_ORDER O ");
            //stb.Append("   INNER JOIN GDA_ORDER_STATUS S (NOLOCK) ON S.IDGDA_ORDER_STATUS = O.GDA_ORDER_STATUS_IDGDA_ORDER_STATUS ");
            //stb.Append("   LEFT JOIN GDA_ATRIBUTES (NOLOCK) AS A ON (A.NAME = 'HOME_BASED' ");
            //stb.Append("                                             OR A.NAME = 'SITE') ");
            //stb.Append("   AND A.CREATED_AT >= @DATAINICIAL ");
            //stb.Append("   AND A.IDGDA_COLLABORATORS = O.ORDER_BY ");
            //stb.Append("   INNER JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS HHR ON HHR.IDGDA_COLLABORATORS = O.ORDER_BY ");
            //stb.Append("   AND CONVERT(DATE, HHR.CREATED_AT) > @DATAINICIAL ");
            //stb.Append("   LEFT JOIN ");
            //stb.Append("     (SELECT COD, ");
            //stb.Append("             IDGDA_COLLABORATORS, ");
            //stb.Append("             PARENTIDENTIFICATION, ");
            //stb.Append("             NAME, ");
            //stb.Append("             LEVELWEIGHT ");
            //stb.Append("      FROM ");
            //stb.Append("        (SELECT LV1.IDGDA_COLLABORATORS AS COD, ");
            //stb.Append("                LV1.IDGDA_COLLABORATORS, ");
            //stb.Append("                ISNULL(LV1.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION, ");
            //stb.Append("                C.NAME, ");
            //stb.Append("                CASE ");
            //stb.Append("                    WHEN LV2.LEVELWEIGHT IS NULL ");
            //stb.Append("                         AND LV1.PARENTIDENTIFICATION IS NOT NULL THEN '7' ");
            //stb.Append("                    ELSE LV2.LEVELWEIGHT ");
            //stb.Append("                END AS LEVELWEIGHT ");
            //stb.Append("         FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1 ");
            //stb.Append("         LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS ");
            //stb.Append("         AND LV2.DATE = @DATAINICIAL ");
            //stb.Append("         LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV1.PARENTIDENTIFICATION ");
            //stb.Append("         WHERE LV1.DATE = @DATAINICIAL ");
            //stb.Append("         UNION ALL SELECT LV1.IDGDA_COLLABORATORS AS COD, ");
            //stb.Append("                          LV2.IDGDA_COLLABORATORS, ");
            //stb.Append("                          ISNULL(LV2.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION, ");
            //stb.Append("                          C.NAME, ");
            //stb.Append("                          CASE ");
            //stb.Append("                              WHEN LV3.LEVELWEIGHT IS NULL ");
            //stb.Append("                                   AND LV2.PARENTIDENTIFICATION IS NOT NULL THEN '7' ");
            //stb.Append("                              ELSE LV3.LEVELWEIGHT ");
            //stb.Append("                          END AS LEVELWEIGHT ");
            //stb.Append("         FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1 ");
            //stb.Append("         LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS ");
            //stb.Append("         AND LV2.DATE = @DATAINICIAL ");
            //stb.Append("         LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS ");
            //stb.Append("         AND LV3.DATE = @DATAINICIAL ");
            //stb.Append("         LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV2.PARENTIDENTIFICATION ");
            //stb.Append("         WHERE LV1.DATE = @DATAINICIAL ");
            //stb.Append("         UNION ALL SELECT LV1.IDGDA_COLLABORATORS AS COD, ");
            //stb.Append("                          LV3.IDGDA_COLLABORATORS, ");
            //stb.Append("                          ISNULL(LV3.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION, ");
            //stb.Append("                          C.NAME, ");
            //stb.Append("                          CASE ");
            //stb.Append("                              WHEN LV4.LEVELWEIGHT IS NULL ");
            //stb.Append("                                   AND LV3.PARENTIDENTIFICATION IS NOT NULL THEN '7' ");
            //stb.Append("                              ELSE LV4.LEVELWEIGHT ");
            //stb.Append("                          END AS LEVELWEIGHT ");
            //stb.Append("         FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1 ");
            //stb.Append("         LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS ");
            //stb.Append("         AND LV2.DATE = @DATAINICIAL ");
            //stb.Append("         LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS ");
            //stb.Append("         AND LV3.DATE = @DATAINICIAL ");
            //stb.Append("         LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV4 ON LV3.PARENTIDENTIFICATION = LV4.IDGDA_COLLABORATORS ");
            //stb.Append("         AND LV4.DATE = @DATAINICIAL ");
            //stb.Append("         LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV3.PARENTIDENTIFICATION ");
            //stb.Append("         WHERE LV1.DATE = @DATAINICIAL ");
            //stb.Append("         UNION ALL SELECT LV1.IDGDA_COLLABORATORS AS COD, ");
            //stb.Append("                          LV4.IDGDA_COLLABORATORS, ");
            //stb.Append("                          ISNULL(LV4.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION, ");
            //stb.Append("                          C.NAME, ");
            //stb.Append("                          CASE ");
            //stb.Append("                              WHEN LV5.LEVELWEIGHT IS NULL ");
            //stb.Append("                                   AND LV4.PARENTIDENTIFICATION IS NOT NULL THEN '7' ");
            //stb.Append("                              ELSE LV5.LEVELWEIGHT ");
            //stb.Append("                          END AS LEVELWEIGHT ");
            //stb.Append("         FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1 ");
            //stb.Append("         LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS ");
            //stb.Append("         AND LV2.DATE = @DATAINICIAL ");
            //stb.Append("         LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS ");
            //stb.Append("         AND LV3.DATE = @DATAINICIAL ");
            //stb.Append("         LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV4 ON LV3.PARENTIDENTIFICATION = LV4.IDGDA_COLLABORATORS ");
            //stb.Append("         AND LV4.DATE = @DATAINICIAL ");
            //stb.Append("         LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV5 ON LV4.PARENTIDENTIFICATION = LV5.IDGDA_COLLABORATORS ");
            //stb.Append("         AND LV5.DATE = @DATAINICIAL ");
            //stb.Append("         LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV4.PARENTIDENTIFICATION ");
            //stb.Append("         WHERE LV1.DATE = @DATAINICIAL ");
            //stb.Append("         UNION ALL SELECT LV1.IDGDA_COLLABORATORS AS COD, ");
            //stb.Append("                          LV5.IDGDA_COLLABORATORS, ");
            //stb.Append("                          ISNULL(LV5.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION, ");
            //stb.Append("                          C.NAME, ");
            //stb.Append("                          CASE ");
            //stb.Append("                              WHEN LV6.LEVELWEIGHT IS NULL ");
            //stb.Append("                                   AND LV5.PARENTIDENTIFICATION IS NOT NULL THEN '7' ");
            //stb.Append("                              ELSE LV6.LEVELWEIGHT ");
            //stb.Append("                          END AS LEVELWEIGHT ");
            //stb.Append("         FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1 ");
            //stb.Append("         LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS ");
            //stb.Append("         AND LV2.DATE = @DATAINICIAL ");
            //stb.Append("         LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS ");
            //stb.Append("         AND LV3.DATE = @DATAINICIAL ");
            //stb.Append("         LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV4 ON LV3.PARENTIDENTIFICATION = LV4.IDGDA_COLLABORATORS ");
            //stb.Append("         AND LV4.DATE = @DATAINICIAL ");
            //stb.Append("         LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV5 ON LV4.PARENTIDENTIFICATION = LV5.IDGDA_COLLABORATORS ");
            //stb.Append("         AND LV5.DATE = @DATAINICIAL ");
            //stb.Append("         LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV6 ON LV5.PARENTIDENTIFICATION = LV6.IDGDA_COLLABORATORS ");
            //stb.Append("         AND LV6.DATE = @DATAINICIAL ");
            //stb.Append("         LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV5.PARENTIDENTIFICATION ");
            //stb.Append("         WHERE LV1.DATE = @DATAINICIAL ");
            //stb.Append("         UNION ALL SELECT LV1.IDGDA_COLLABORATORS AS COD, ");
            //stb.Append("                          LV6.IDGDA_COLLABORATORS, ");
            //stb.Append("                          ISNULL(LV6.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION, ");
            //stb.Append("                          C.NAME, ");
            //stb.Append("                          CASE ");
            //stb.Append("                              WHEN LV7.LEVELWEIGHT IS NULL ");
            //stb.Append("                                   AND LV6.PARENTIDENTIFICATION IS NOT NULL THEN '7' ");
            //stb.Append("                              ELSE LV7.LEVELWEIGHT ");
            //stb.Append("                          END AS LEVELWEIGHT ");
            //stb.Append("         FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1 ");
            //stb.Append("         LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS ");
            //stb.Append("         AND LV2.DATE = @DATAINICIAL ");
            //stb.Append("         LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS ");
            //stb.Append("         AND LV3.DATE = @DATAINICIAL ");
            //stb.Append("         LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV4 ON LV3.PARENTIDENTIFICATION = LV4.IDGDA_COLLABORATORS ");
            //stb.Append("         AND LV4.DATE = @DATAINICIAL ");
            //stb.Append("         LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV5 ON LV4.PARENTIDENTIFICATION = LV5.IDGDA_COLLABORATORS ");
            //stb.Append("         AND LV5.DATE = @DATAINICIAL ");
            //stb.Append("         LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV6 ON LV5.PARENTIDENTIFICATION = LV6.IDGDA_COLLABORATORS ");
            //stb.Append("         AND LV6.DATE = @DATAINICIAL ");
            //stb.Append("         LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV7 ON LV6.PARENTIDENTIFICATION = LV7.IDGDA_COLLABORATORS ");
            //stb.Append("         AND LV7.DATE = @DATAINICIAL ");
            //stb.Append("         LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV8 ON LV7.PARENTIDENTIFICATION = LV8.IDGDA_COLLABORATORS ");
            //stb.Append("         AND LV8.DATE = @DATAINICIAL ");
            //stb.Append("         LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV6.PARENTIDENTIFICATION ");
            //stb.Append("         WHERE LV1.DATE = @DATAINICIAL ) AS CombinedData) AS HIERARCHY ON HIERARCHY.COD = O.ORDER_BY ");
            //stb.Append("   INNER JOIN GDA_HISTORY_COLLABORATOR_SECTOR HCS (NOLOCK) ON HCS.IDGDA_COLLABORATORS = O.ORDER_BY ");
            //stb.Append("   AND HCS.CREATED_AT > @DATAINICIAL ");
            //stb.Append("   INNER JOIN GDA_SECTOR SEC (NOLOCK) ON SEC.IDGDA_SECTOR = HCS.IDGDA_SECTOR ");
            //stb.Append("   INNER JOIN GDA_ORDER_PRODUCT OP (NOLOCK) ON OP.GDA_ORDER_IDGDA_ORDER = O.IDGDA_ORDER ");
            //stb.Append("   INNER JOIN GDA_PRODUCT P (NOLOCK) ON P.IDGDA_PRODUCT = OP.GDA_PRODUCT_IDGDA_PRODUCT ");
            //stb.Append("   INNER JOIN GDA_COLLABORATORS COL (NOLOCK) ON COL.IDGDA_COLLABORATORS = O.LAST_UPDATED_BY ");
            //stb.Append("   INNER JOIN GDA_COLLABORATORS COL2 (NOLOCK) ON COL2.IDGDA_COLLABORATORS = O.DELIVERED_BY ");
            //stb.Append("   INNER JOIN GDA_COLLABORATORS COL3 (NOLOCK) ON COL3.IDGDA_COLLABORATORS = O.ORDER_BY ");
            //stb.Append("   INNER JOIN GDA_COLLABORATORS COL4 (NOLOCK) ON COL4.IDGDA_COLLABORATORS = O.RELEASED_BY ");
            //stb.Append("GROUP BY O.COD_ORDER, P.TYPE, P.COMERCIAL_NAME, OP.AMOUNT, P.PRICE, P.QUANTITY) ");
            //stb.Append(" AS QUERY ");
            //stb.AppendFormat(" {0} ", where);


            //ANTIGA

            //string commandText = $@"DECLARE @DATAINICIAL DATE;
            //                        SET @DATAINICIAL = FORMAT(DATEADD(DAY, -5, GETDATE()), 'yyyy-MM-dd');
            //                        SELECT * FROM(
            //                        SELECT O.COD_ORDER AS CODIGO,
            //                        MAX(O.ORDER_BY) AS 'CRIADO_POR',
            //                        MAX(S.STATUS) AS STATUS,
            //                        MAX(O.CREATED_AT) AS 'CRIADO_EM',
            //                        MAX(O.ORDER_BY) AS 'COLABORADOR',
            //                        MAX(LEVELNAME) AS 'CARGO',
            //                        MAX(CASE WHEN HIERARCHY.LEVELWEIGHT = '2' THEN HIERARCHY.NAME ELSE '-' END) AS 'NOME_SUPERVISOR',
            //                        MAX(CASE WHEN HIERARCHY.LEVELWEIGHT = '3' THEN HIERARCHY.NAME ELSE '-' END) AS 'NOME_COORDENADOR',
            //                        MAX(CASE WHEN HIERARCHY.LEVELWEIGHT = '4' THEN HIERARCHY.NAME ELSE '-' END) AS 'NOME_GERENTE_II',
            //                        MAX(CASE WHEN HIERARCHY.LEVELWEIGHT = '5' THEN HIERARCHY.NAME ELSE '-' END) AS 'NOME_GERENTE_I',
            //                        MAX(CASE WHEN HIERARCHY.LEVELWEIGHT = '6' THEN HIERARCHY.NAME ELSE '-' END) AS 'NOME_DIRETOR',
            //                        MAX(CASE WHEN HIERARCHY.LEVELWEIGHT = '7' THEN HIERARCHY.NAME ELSE '-' END) AS 'NOME_CEO',
            //                        '' AS GRUPO,
            //                        MAX(CASE WHEN A.NAME = 'SITE' THEN A.VALUE ELSE '' END) AS UF,
            //                        MAX(HCS.IDGDA_SECTOR) AS 'COD_GIP',
            //                        MAX(SEC.NAME) AS 'SETOR',
            //                        MAX(CASE WHEN A.NAME = 'HOME_BASED' THEN A.VALUE ELSE '' END) AS HOME,
            //                        MAX(P.TYPE) AS 'TIPO_DE_PRODUTO',
            //                        MAX(P.COMERCIAL_NAME) AS 'NOME_DO_PRODUTO',
            //                        MAX(OP.AMOUNT) AS 'QUANTIDADE',
            //                        MAX(P.PRICE) AS 'VALOR_EM_MOEDAS',
            //                        (MAX(P.PRICE) * MAX(OP.AMOUNT)) AS 'VALOR_EM_MOEDAS_TOTAL',
            //                        MAX(O.LAST_UPDATED_AT) AS 'ULTIMA_ATUALIZACAO',
            //                        MAX(COL.NAME) AS 'QUEM_ATUALIZOU',
            //                        MAX(COL2.NAME) AS 'ENTREGUE_POR',
            //                        MAX(COL3.COLLABORATORIDENTIFICATION) AS 'BC_COLABORADOR',
            //                        MAX(O.OBSERVATION_ORDER) AS 'QUEM_VAI_RETIRAR',
            //                        MAX(O.OBSERVATION_DELIVERED) AS 'QUEM_RETIROU',
            //                        MAX(SP.AMOUNT) AS 'ESTOQUE'
            //                        FROM GDA_ORDER O
            //                        INNER JOIN GDA_ORDER_STATUS S (NOLOCK) ON S.IDGDA_ORDER_STATUS = O.GDA_ORDER_STATUS_IDGDA_ORDER_STATUS
            //                        LEFT JOIN GDA_ATRIBUTES (NOLOCK) AS A ON (A.NAME = 'HOME_BASED' OR A.NAME = 'SITE') AND A.CREATED_AT >= @DATAINICIAL AND A.IDGDA_COLLABORATORS = O.ORDER_BY
            //                        INNER JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS HHR ON HHR.IDGDA_COLLABORATORS = O.ORDER_BY AND CONVERT(DATE, HHR.DATE) > @DATAINICIAL
            //                        LEFT JOIN
            //                          (SELECT COD,
            //                                  IDGDA_COLLABORATORS,
            //                                  PARENTIDENTIFICATION,
            //                                  NAME,
            //                                  LEVELWEIGHT
            //                           FROM
            //                             (SELECT LV1.IDGDA_COLLABORATORS AS COD,
            //                                     LV1.IDGDA_COLLABORATORS,
            //                                     ISNULL(LV1.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION,
            //                                     C.NAME,
            //                                     CASE
            //                                         WHEN LV2.LEVELWEIGHT IS NULL
            //                                              AND LV1.PARENTIDENTIFICATION IS NOT NULL THEN '7'
            //                                         ELSE LV2.LEVELWEIGHT
            //                                     END AS LEVELWEIGHT
            //                              FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1
            //                              LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS
            //                              AND LV2.DATE = @DATAINICIAL
            //                              LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV1.PARENTIDENTIFICATION
            //                              WHERE LV1.DATE = @DATAINICIAL
            //                              UNION ALL SELECT LV1.IDGDA_COLLABORATORS AS COD,
            //                                               LV2.IDGDA_COLLABORATORS,
            //                                               ISNULL(LV2.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION,
            //                                               C.NAME,
            //                                               CASE
            //                                                   WHEN LV3.LEVELWEIGHT IS NULL
            //                                                        AND LV2.PARENTIDENTIFICATION IS NOT NULL THEN '7'
            //                                                   ELSE LV3.LEVELWEIGHT
            //                                               END AS LEVELWEIGHT
            //                              FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1
            //                              LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS
            //                              AND LV2.DATE = @DATAINICIAL
            //                              LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS
            //                              AND LV3.DATE = @DATAINICIAL
            //                              LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV2.PARENTIDENTIFICATION
            //                              WHERE LV1.DATE = @DATAINICIAL
            //                              UNION ALL SELECT LV1.IDGDA_COLLABORATORS AS COD,
            //                                               LV3.IDGDA_COLLABORATORS,
            //                                               ISNULL(LV3.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION,
            //                                               C.NAME,
            //                                               CASE
            //                                                   WHEN LV4.LEVELWEIGHT IS NULL
            //                                                        AND LV3.PARENTIDENTIFICATION IS NOT NULL THEN '7'
            //                                                   ELSE LV4.LEVELWEIGHT
            //                                               END AS LEVELWEIGHT
            //                              FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1
            //                              LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS
            //                              AND LV2.DATE = @DATAINICIAL
            //                              LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS
            //                              AND LV3.DATE = @DATAINICIAL
            //                              LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV4 ON LV3.PARENTIDENTIFICATION = LV4.IDGDA_COLLABORATORS
            //                              AND LV4.DATE = @DATAINICIAL
            //                              LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV3.PARENTIDENTIFICATION
            //                              WHERE LV1.DATE = @DATAINICIAL
            //                              UNION ALL SELECT LV1.IDGDA_COLLABORATORS AS COD,
            //                                               LV4.IDGDA_COLLABORATORS,
            //                                               ISNULL(LV4.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION,
            //                                               C.NAME,
            //                                               CASE
            //                                                   WHEN LV5.LEVELWEIGHT IS NULL
            //                                                        AND LV4.PARENTIDENTIFICATION IS NOT NULL THEN '7'
            //                                                   ELSE LV5.LEVELWEIGHT
            //                                               END AS LEVELWEIGHT
            //                              FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1
            //                              LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS
            //                              AND LV2.DATE = @DATAINICIAL
            //                              LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS
            //                              AND LV3.DATE = @DATAINICIAL
            //                              LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV4 ON LV3.PARENTIDENTIFICATION = LV4.IDGDA_COLLABORATORS
            //                              AND LV4.DATE = @DATAINICIAL
            //                              LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV5 ON LV4.PARENTIDENTIFICATION = LV5.IDGDA_COLLABORATORS
            //                              AND LV5.DATE = @DATAINICIAL
            //                              LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV4.PARENTIDENTIFICATION
            //                              WHERE LV1.DATE = @DATAINICIAL
            //                              UNION ALL SELECT LV1.IDGDA_COLLABORATORS AS COD,
            //                                               LV5.IDGDA_COLLABORATORS,
            //                                               ISNULL(LV5.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION,
            //                                               C.NAME,
            //                                               CASE
            //                                                   WHEN LV6.LEVELWEIGHT IS NULL
            //                                                        AND LV5.PARENTIDENTIFICATION IS NOT NULL THEN '7'
            //                                                   ELSE LV6.LEVELWEIGHT
            //                                               END AS LEVELWEIGHT
            //                              FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1
            //                              LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS
            //                              AND LV2.DATE = @DATAINICIAL
            //                              LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS
            //                              AND LV3.DATE = @DATAINICIAL
            //                              LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV4 ON LV3.PARENTIDENTIFICATION = LV4.IDGDA_COLLABORATORS
            //                              AND LV4.DATE = @DATAINICIAL
            //                              LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV5 ON LV4.PARENTIDENTIFICATION = LV5.IDGDA_COLLABORATORS
            //                              AND LV5.DATE = @DATAINICIAL
            //                              LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV6 ON LV5.PARENTIDENTIFICATION = LV6.IDGDA_COLLABORATORS
            //                              AND LV6.DATE = @DATAINICIAL
            //                              LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV5.PARENTIDENTIFICATION
            //                              WHERE LV1.DATE = @DATAINICIAL
            //                              UNION ALL SELECT LV1.IDGDA_COLLABORATORS AS COD,
            //                                               LV6.IDGDA_COLLABORATORS,
            //                                               ISNULL(LV6.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION,
            //                                               C.NAME,
            //                                               CASE
            //                                                   WHEN LV7.LEVELWEIGHT IS NULL
            //                                                        AND LV6.PARENTIDENTIFICATION IS NOT NULL THEN '7'
            //                                                   ELSE LV7.LEVELWEIGHT
            //                                               END AS LEVELWEIGHT
            //                              FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1
            //                              LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS
            //                              AND LV2.DATE = @DATAINICIAL
            //                              LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS
            //                              AND LV3.DATE = @DATAINICIAL
            //                              LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV4 ON LV3.PARENTIDENTIFICATION = LV4.IDGDA_COLLABORATORS
            //                              AND LV4.DATE = @DATAINICIAL
            //                              LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV5 ON LV4.PARENTIDENTIFICATION = LV5.IDGDA_COLLABORATORS
            //                              AND LV5.DATE = @DATAINICIAL
            //                              LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV6 ON LV5.PARENTIDENTIFICATION = LV6.IDGDA_COLLABORATORS
            //                              AND LV6.DATE = @DATAINICIAL
            //                              LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV7 ON LV6.PARENTIDENTIFICATION = LV7.IDGDA_COLLABORATORS
            //                              AND LV7.DATE = @DATAINICIAL
            //                              LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV8 ON LV7.PARENTIDENTIFICATION = LV8.IDGDA_COLLABORATORS
            //                              AND LV8.DATE = @DATAINICIAL
            //                              LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV6.PARENTIDENTIFICATION
            //                              WHERE LV1.DATE = @DATAINICIAL ) AS CombinedData) AS HIERARCHY ON HIERARCHY.COD = O.ORDER_BY
            //                        INNER JOIN GDA_HISTORY_COLLABORATOR_SECTOR HCS (NOLOCK) ON HCS.IDGDA_COLLABORATORS = O.ORDER_BY AND HCS.CREATED_AT > @DATAINICIAL
            //                        INNER JOIN GDA_SECTOR SEC (NOLOCK) ON SEC.IDGDA_SECTOR = HCS.IDGDA_SECTOR
            //                        INNER JOIN GDA_ORDER_PRODUCT OP (NOLOCK) ON OP.GDA_ORDER_IDGDA_ORDER = O.IDGDA_ORDER
            //                        INNER JOIN GDA_PRODUCT P (NOLOCK) ON P.IDGDA_PRODUCT = OP.GDA_PRODUCT_IDGDA_PRODUCT
            //                        INNER JOIN GDA_STOCK_PRODUCT SP (NOLOCK) ON SP.GDA_PRODUCT_IDGDA_PRODUCT = OP.GDA_PRODUCT_IDGDA_PRODUCT
            //                        INNER JOIN GDA_COLLABORATORS COL (NOLOCK) ON COL.IDGDA_COLLABORATORS = O.LAST_UPDATED_BY AND COL.CREATED_AT > @DataInicial
            //                        LEFT JOIN GDA_COLLABORATORS COL2 (NOLOCK) ON COL2.IDGDA_COLLABORATORS = O.DELIVERED_BY AND COL2.CREATED_AT > @DataInicial
            //                        INNER JOIN GDA_COLLABORATORS COL3 (NOLOCK) ON COL3.IDGDA_COLLABORATORS = O.ORDER_BY AND COL3.CREATED_AT > @DataInicial
            //                        GROUP BY O.COD_ORDER, P.COMERCIAL_NAME) AS QUERY {where}  {order}";
            #endregion

            StringBuilder stb = new StringBuilder();
            stb.Append($"DECLARE @DATAINICIAL DATE;SET @DATAINICIAL = '{datainicio}'; ");
            stb.Append(" ");
            stb.Append($"DECLARE @DATAFINAL DATE;SET @DATAFINAL = '{datafim}'; ");
            stb.Append(" ");
            stb.Append("SELECT * ");
            stb.Append("FROM ");
            stb.Append("  (SELECT O.IDGDA_ORDER AS CODIGO, ");
            stb.Append("          MAX(O.ORDER_BY) AS 'CRIADO_POR', ");
            stb.Append("          MAX(CASE WHEN OP.ORDER_PRODUCT_STATUS IS NULL THEN S.STATUS ELSE OP.ORDER_PRODUCT_STATUS END) AS STATUS,  ");
            //stb.Append("          MAX(S.STATUS) AS STATUS, ");
            stb.Append("          MAX(O.CREATED_AT) AS 'CRIADO_EM', ");
            //stb.Append("          MAX(O.ORDER_BY) AS 'COLABORADOR', ");
            stb.Append("          MAX(COL3.NAME) AS 'COLABORADOR',");
            stb.Append("		  MAX(CL.CARGO) AS CARGO, ");
            stb.Append("          ISNULL(MAX(CL.MATRICULA_SUPERVISOR),0) AS 'MATRICULA SUPERVISOR', ");
            stb.Append("          MAX(CL.NOME_SUPERVISOR) AS 'NOME SUPERVISOR', ");
            stb.Append("          ISNULL(MAX(CL.MATRICULA_COORDENADOR),0) AS 'MATRICULA COORDENADOR', ");
            stb.Append("          MAX(CL.NOME_COORDENADOR) AS 'NOME COORDENADOR', ");
            stb.Append("          ISNULL(MAX(CL.MATRICULA_GERENTE_II),0) AS 'MATRICULA GERENTE II', ");
            stb.Append("          MAX(CL.NOME_GERENTE_II) AS 'NOME GERENTE II', ");
            stb.Append("          ISNULL(MAX(CL.MATRICULA_GERENTE_I),0) AS 'MATRICULA GERENTE I', ");
            stb.Append("          MAX(CL.NOME_GERENTE_I) AS 'NOME GERENTE I', ");
            stb.Append("          ISNULL(MAX(CL.MATRICULA_DIRETOR),0) AS 'MATRICULA DIRETOR', ");
            stb.Append("          MAX(CL.NOME_DIRETOR) AS 'NOME DIRETOR', ");
            stb.Append("          ISNULL(MAX(CL.MATRICULA_CEO),0) AS 'MATRICULA CEO', ");
            stb.Append("          MAX(CL.NOME_CEO) AS 'NOME CEO', ");
            stb.Append("          ISNULL(MAX(O.IDGROUP),0) AS GRUPO, ");
            stb.Append("		  MAX(CL.SITE) AS UF, ");
            stb.Append("          ISNULL(MAX(CL.IDGDA_SECTOR),0) AS 'COD_GIP', ");
            stb.Append("          MAX(SEC.NAME) AS 'SETOR', ");
            stb.Append("		  MAX(CL.HOME_BASED) AS HOME, ");
            stb.Append("          MAX(P.TYPE) AS 'TIPO_DE_PRODUTO', ");
            stb.Append("          MAX(P.COMERCIAL_NAME) AS 'NOME_DO_PRODUTO', ");
            stb.Append("          MAX(OP.AMOUNT) AS 'QUANTIDADE', ");
            stb.Append("          MAX(P.PRICE) AS 'VALOR_EM_MOEDAS', ");
            stb.Append("          (MAX(P.PRICE) * MAX(OP.AMOUNT)) AS 'VALOR_EM_MOEDAS_TOTAL', ");
            stb.Append("          MAX(O.LAST_UPDATED_AT) AS 'ULTIMA_ATUALIZACAO', ");
            stb.Append("          MAX(COL4.NAME) AS 'QUEM_ATUALIZOU', ");
            stb.Append("          MAX(CASE WHEN O.OBSERVATION_CANCELED IS NOT NULL OR OP.OBSERVATION_CANCELED IS NOT NULL THEN '' ELSE COL8.NAME END) AS 'ENTREGUE_POR', ");
            stb.Append("          MAX(COL3.COLLABORATORIDENTIFICATION) AS 'BC_COLABORADOR', ");
            //stb.Append("          MAX(COL.NAME)AS 'LIBERADO_POR', ");
            //stb.Append("          MAX( CASE WHEN COL6.NAME IS NULL THEN COL.NAME ELSE COL6.NAME END) AS 'LIBERADO_POR', ");
            //stb.Append("          MAX(O.OBSERVATION_RELEASED) AS 'OBSERVACAO_LIBERACAO',  ");
            //stb.Append("          MAX( CASE WHEN OP.OBSERVATION_RELEASED IS NULL THEN O.OBSERVATION_RELEASED ELSE OP.OBSERVATION_RELEASED END) AS 'OBSERVACAO_LIBERACAO', ");
            stb.Append("          MAX(COL6.NAME) AS 'LIBERADO_POR', ");
            stb.Append("          MAX(OP.OBSERVATION_RELEASED) AS 'OBSERVACAO_LIBERACAO', ");
            stb.Append("          MAX(CASE WHEN O.OBSERVATION_CANCELED IS NOT NULL OR OP.OBSERVATION_CANCELED IS NOT NULL THEN '' ELSE O.OBSERVATION_ORDER END) AS 'QUEM_VAI_RETIRAR', ");
            //stb.Append("          MAX(O.OBSERVATION_DELIVERED) AS 'QUEM_RETIROU', ");
            stb.Append("          MAX(OP.OBSERVATION_DELIVERED)AS 'QUEM_RETIROU', ");
            stb.Append("          MAX(CASE WHEN O.OBSERVATION_CANCELED IS NOT NULL OR OP.OBSERVATION_CANCELED IS NOT NULL THEN  '' ELSE OP.DELIVERY_NOTE END) AS 'OBSERVACAO_DE_ENTREGA', ");
            //stb.Append("          MAX(COL5.NAME) AS 'CANCELADO_POR', ");
            //stb.Append("          MAX(CASE WHEN COL7.NAME IS NULL THEN COL5.NAME ELSE COL7.NAME END) AS 'CANCELADO_POR', ");
            //stb.Append("          MAX(O.OBSERVATION_CANCELED) AS 'OBSERVACAO_CANCELAMENTO', ");
            //stb.Append("          MAX(CASE WHEN O.OBSERVATION_CANCELED IS NULL THEN OP.OBSERVATION_CANCELED ELSE O.OBSERVATION_CANCELED END) AS 'OBSERVACAO_CANCELAMENTO', ");
            stb.Append("          MAX(COL7.NAME) AS 'CANCELADO_POR', ");
            stb.Append("          MAX(OP.OBSERVATION_CANCELED) AS 'OBSERVACAO_CANCELAMENTO', ");
            stb.Append("          SUM(ISNULL(SK.AMOUNT,0)) AS 'ESTOQUE' ");
            stb.Append("   FROM GDA_ORDER O (NOLOCK) ");
            stb.Append("   INNER JOIN GDA_ORDER_STATUS S (NOLOCK) ON S.IDGDA_ORDER_STATUS = O.GDA_ORDER_STATUS_IDGDA_ORDER_STATUS ");
            stb.Append("   LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CL ON CL.IDGDA_COLLABORATORS = O.ORDER_BY AND ");
            //stb.Append("   AND CONVERT(DATE, CL.CREATED_AT) = CONVERT(DATE, O.ORDER_AT) ");
            stb.Append("CASE ");
            stb.Append("    WHEN CONVERT(DATE, O.ORDER_AT) = CONVERT(DATE, GETDATE()) ");
            stb.Append("    THEN DATEADD(DAY, -1, CONVERT(DATE, O.ORDER_AT)) ");
            stb.Append("    ELSE CONVERT(DATE, O.ORDER_AT) ");
            stb.Append("END = CONVERT(DATE, CL.CREATED_AT) ");


            stb.Append("   LEFT JOIN GDA_SECTOR SEC (NOLOCK) ON SEC.IDGDA_SECTOR = CL.IDGDA_SECTOR ");
            stb.Append("   INNER JOIN GDA_ORDER_PRODUCT OP (NOLOCK) ON OP.GDA_ORDER_IDGDA_ORDER = O.IDGDA_ORDER ");
            stb.Append("   INNER JOIN GDA_PRODUCT P (NOLOCK) ON P.IDGDA_PRODUCT = OP.GDA_PRODUCT_IDGDA_PRODUCT ");
            //stb.Append("   INNER JOIN GDA_STOCK_PRODUCT SP (NOLOCK) ON SP.GDA_PRODUCT_IDGDA_PRODUCT = OP.GDA_PRODUCT_IDGDA_PRODUCT ");

            stb.Append("LEFT JOIN (SELECT CITY, IDGDA_STOCK, GDA_PRODUCT_IDGDA_PRODUCT, MAX(AMOUNT) AS AMOUNT ");
            stb.Append("	FROM GDA_STOCK (NOLOCK) AS SK ");
            stb.Append("		LEFT JOIN GDA_STOCK_PRODUCT SP (NOLOCK) ON SP.GDA_STOCK_IDGDA_STOCK = SK.IDGDA_STOCK ");
            stb.Append("	WHERE SK.DELETED_AT IS NULL ");
            stb.Append("	GROUP BY CITY, IDGDA_STOCK, GDA_PRODUCT_IDGDA_PRODUCT ");
            stb.Append(") AS SK ON SK.CITY = CL.SITE AND SK.GDA_PRODUCT_IDGDA_PRODUCT = OP.GDA_PRODUCT_IDGDA_PRODUCT ");


            stb.Append("   LEFT JOIN GDA_COLLABORATORS COL (NOLOCK) ON COL.IDGDA_COLLABORATORS = O.RELEASED_BY ");
            //stb.Append("   AND COL.CREATED_AT > @DATAINICIAL ");
            stb.Append("   LEFT JOIN GDA_COLLABORATORS COL2 (NOLOCK) ON COL2.IDGDA_COLLABORATORS = O.DELIVERED_BY ");
            //stb.Append("   AND COL2.CREATED_AT > @DATAINICIAL ");
            stb.Append("   LEFT JOIN GDA_COLLABORATORS COL3 (NOLOCK) ON COL3.IDGDA_COLLABORATORS = O.ORDER_BY ");
            stb.Append("   LEFT JOIN GDA_COLLABORATORS COL4 (NOLOCK) ON COL4.IDGDA_COLLABORATORS = O.LAST_UPDATED_BY ");
            stb.Append("   LEFT JOIN GDA_COLLABORATORS COL5 (NOLOCK) ON COL5.IDGDA_COLLABORATORS = O.CANCELED_BY  ");
            stb.Append("   LEFT JOIN GDA_COLLABORATORS COL6 (NOLOCK) ON COL6.IDGDA_COLLABORATORS = OP.RELEASED_BY");
            stb.Append("   LEFT JOIN GDA_COLLABORATORS COL7 (NOLOCK) ON COL7.IDGDA_COLLABORATORS = OP.CANCELED_BY");
            stb.Append("   LEFT JOIN GDA_COLLABORATORS COL8 (NOLOCK) ON COL8.IDGDA_COLLABORATORS = OP.DELIVERED_BY ");
            //stb.Append("   AND COL3.CREATED_AT > @DATAINICIAL ");
            stb.Append("   GROUP BY O.IDGDA_ORDER, ");
            stb.Append($"            P.COMERCIAL_NAME) AS QUERY {where}  {order} ");

            List<OrdersReport> OrderReport = new List<OrdersReport>();

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                {

                    //using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    //{
                    //    DataTable dataTable = new DataTable();
                    //    adapter.Fill(dataTable);
                    //    connection.Close();
                    //    return Ok(dataTable);
                    //}


                    command.CommandTimeout = 300;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            OrdersReport orders = new OrdersReport();
                            orders.CODIGO = int.Parse(reader["CODIGO"].ToString());
                            orders.CRIADO_POR = int.Parse(reader["CRIADO_POR"].ToString());
                            orders.STATUS = reader["STATUS"].ToString();
                            orders.CRIADO_EM = reader["CRIADO_EM"].ToString();
                            orders.COLABORADOR = reader["COLABORADOR"].ToString();
                            orders.CARGO = reader["CARGO"].ToString();
                            orders.MATRICULA_SUPERVISOR = int.Parse(reader["MATRICULA SUPERVISOR"].ToString());
                            orders.NOME_SUPERVISOR = reader["NOME SUPERVISOR"].ToString();
                            orders.MATRICULA_COORDENADOR = int.Parse(reader["MATRICULA COORDENADOR"].ToString());
                            orders.NOME_COORDENADOR = reader["NOME COORDENADOR"].ToString();
                            orders.MATRICULA_GERENTE_II = int.Parse(reader["MATRICULA GERENTE II"].ToString());
                            orders.NOME_GERENTE_II = reader["NOME GERENTE II"].ToString();
                            orders.MATRICULA_GERENTE_I = int.Parse(reader["MATRICULA GERENTE I"].ToString());
                            orders.NOME_GERENTE_I = reader["NOME GERENTE I"].ToString();
                            orders.MATRICULA_DIRETOR = int.Parse(reader["MATRICULA DIRETOR"].ToString());
                            orders.NOME_DIRETOR = reader["NOME DIRETOR"].ToString();
                            orders.MATRICULA_CEO = int.Parse(reader["MATRICULA CEO"].ToString());
                            orders.NOME_CEO = reader["NOME CEO"].ToString();                           
                            if (int.Parse(reader["GRUPO"].ToString()) == 1)
                            {
                                orders.GRUPO = "DIAMANTE";
                            }
                            else if (int.Parse(reader["GRUPO"].ToString()) == 2)
                            {
                                orders.GRUPO = "OURO";
                            }
                            else if (int.Parse(reader["GRUPO"].ToString()) == 3)
                            {
                                orders.GRUPO = "PRATA";
                            }
                            else if (int.Parse(reader["GRUPO"].ToString()) == 4)
                            {
                                orders.GRUPO = "BRONZE";
                            }
                            else if (int.Parse(reader["GRUPO"].ToString()) == 0)
                            {
                                orders.GRUPO = "-";
                            }
                            orders.UF = reader["UF"].ToString();
                            orders.COD_GIP = int.Parse(reader["COD_GIP"].ToString());
                            orders.SETOR = reader["SETOR"].ToString();
                            orders.HOME = reader["HOME"].ToString();
                            orders.TIPO_DE_PRODUTO = reader["TIPO_DE_PRODUTO"].ToString();
                            orders.NOME_DO_PRODUTO = reader["NOME_DO_PRODUTO"].ToString();
                            orders.QUANTIDADE = int.Parse(reader["QUANTIDADE"].ToString());
                            orders.VALOR_EM_MOEDAS = double.Parse(reader["VALOR_EM_MOEDAS"].ToString());
                            orders.VALOR_EM_MOEDAS_TOTAL = double.Parse(reader["VALOR_EM_MOEDAS_TOTAL"].ToString());
                            orders.ULTIMA_ATUALIZACAO = reader["ULTIMA_ATUALIZACAO"].ToString();
                            orders.QUEM_ATUALIZOU = reader["QUEM_ATUALIZOU"].ToString();
                            orders.ENTREGUE_POR = reader["ENTREGUE_POR"].ToString();
                            orders.BC_COLABORADOR = reader["BC_COLABORADOR"].ToString();
                            orders.LIBERADO_POR = reader["LIBERADO_POR"].ToString();
                            orders.OBSERVACAO_LIBERACAO = reader["OBSERVACAO_LIBERACAO"].ToString();
                            orders.QUEM_VAI_RETIRAR = reader["QUEM_VAI_RETIRAR"].ToString();
                            orders.QUEM_RETIROU = reader["QUEM_RETIROU"].ToString();
                            orders.OBSERVACAO_DE_ENTREGA = reader["OBSERVACAO_DE_ENTREGA"].ToString();
                            orders.CANCELADO_POR = reader["CANCELADO_POR"].ToString();
                            orders.OBSERVACAO_CANCELAMENTO = reader["OBSERVACAO_CANCELAMENTO"].ToString();
                            orders.ESTOQUE = int.Parse(reader["ESTOQUE"].ToString());               
                            //orders.ESTOQUE = reader["ESTOQUE"].ToString();
                            OrderReport.Add(orders);
                        }
                    }

                    //for (int i = 0; i < OrderReport.Count; i++)
                    //{
                    //    OrdersReport Ordem = OrderReport[i];

                    //    ReturnBasketIndicatorUser rmams = new ReturnBasketIndicatorUser();
                    //    rmams = returnIndicatorUserMonetization(Ordem.CRIADO_POR, Ordem.CRIADO_EM.Trim(), Ordem.CRIADO_EM.Trim());

                    

                    //    OrderReport[i] = Ordem;
                    //}
                    connection.Close();
                }
            }
            var jsonData = OrderReport.Select(item => new returnResponseDay
            {
                CODIGO = item.CODIGO,
                CRIADO_POR = item.CRIADO_POR,
                STATUS = item.STATUS,
                CRIADO_EM = item.CRIADO_EM,
                COLABORADOR = item.COLABORADOR,
                CARGO = item.CARGO,
                MATRICULA_SUPERVISOR = item.MATRICULA_SUPERVISOR,
                NOME_SUPERVISOR = item.NOME_SUPERVISOR,
                MATRICULA_COORDENADOR = item.MATRICULA_COORDENADOR,
                NOME_COORDENADOR = item.NOME_COORDENADOR,
                MATRICULA_GERENTE_II = item.MATRICULA_GERENTE_II,
                NOME_GERENTE_II = item.NOME_GERENTE_II,
                MATRICULA_GERENTE_I = item.MATRICULA_GERENTE_I,
                NOME_GERENTE_I = item.NOME_GERENTE_I,
                MATRICULA_DIRETOR = item.MATRICULA_DIRETOR,
                NOME_DIRETOR = item.NOME_DIRETOR,
                MATRICULA_CEO = item.MATRICULA_CEO,
                NOME_CEO = item.NOME_CEO,
                GRUPO = item.GRUPO,
                UF = item.UF,
                COD_GIP = item.COD_GIP,
                SETOR = item.SETOR,
                HOME = item.HOME,
                TIPO_DE_PRODUTO = item.TIPO_DE_PRODUTO,
                NOME_DO_PRODUTO = item.NOME_DO_PRODUTO,
                QUANTIDADE = item.QUANTIDADE,
                VALOR_EM_MOEDAS = item.VALOR_EM_MOEDAS,
                VALOR_EM_MOEDAS_TOTAL = item.VALOR_EM_MOEDAS_TOTAL,
                ULTIMA_ATUALIZACAO = item.ULTIMA_ATUALIZACAO,
                QUEM_ATUALIZOU = item.QUEM_ATUALIZOU,
                ENTREGUE_POR = item.ENTREGUE_POR,
                BC_COLABORADOR = item.BC_COLABORADOR,
                LIBERADO_POR = item.LIBERADO_POR,
                OBSERVACAO_LIBERACAO = item.OBSERVACAO_LIBERACAO,
                CANCELADO_POR = item.CANCELADO_POR,
                OBSERVACAO_CANCELAMENTO = item.OBSERVACAO_CANCELAMENTO,
                QUEM_VAI_RETIRAR = item.QUEM_VAI_RETIRAR,
                QUEM_RETIROU = item.QUEM_RETIROU,
                OBSERVACAO_DE_ENTREGA = item.OBSERVACAO_DE_ENTREGA,
                ESTOQUE = item.ESTOQUE,
            }).ToList();

            //Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(jsonData);
        }

        //public ReturnBasketIndicatorUser returnIndicatorUserMonetization(int idCol, string dtinicial, string dtfinal)
        //{
        //    ReturnBasketIndicatorUser rmams = new ReturnBasketIndicatorUser();
        //    rmams.idCollaborator = idCol.ToString();

        //    double moedasGanhas = 0;

        //    DateTime dtInicio = DateTime.Now.AddDays(-30);
        //    string dtIni = dtInicio.ToString("yyyy-MM-dd");
        //    DateTime dtFinal = DateTime.Now;
        //    string dtFim = dtFinal.ToString("yyyy-MM-dd");
        //    if (dtinicial != null )
        //    {
        //        var formato = "dd/MM/yyyy HH:mm:ss";
        //        DateTime dataInicio;
        //        var dtIniConvertida = DateTime.TryParseExact(dtinicial, formato, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dataInicio);

        //        DateTime dataFim;
        //        var dtFimConvertida = DateTime.TryParseExact(dtfinal, formato, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dataFim);


        //        //DateTime dataFormatada = DateTime.ParseExact(dtinicial, "dd/MM/yyyy HH:mm:ss", null);
        //        //dataFormatada = dataFormatada.Date;
        //        //string dataAmericana = dataFormatada.ToString("yyyy-MM-dd");

        //        //DateTime dataFormatada2 = DateTime.ParseExact(dtfinal, "dd/MM/yyyy HH:mm:ss", null);
        //        //dataFormatada2 = dataFormatada2.Date;
        //        // string dataAmericana2 = dataFormatada2.ToString("yyyy-MM-dd");
        //        dtIni = dataInicio.ToString("yyyy-MM-dd");
        //        dtFim = dataFim.ToString("yyyy-MM-dd");

        //        //dtIni = dtIniConvertida.ToString().Trim();
        //        //dtFim = dtFimConvertida.ToString().Trim();

        //    }

        //    //Query moedas ganhas
        //    StringBuilder stb = new StringBuilder();
        //    stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtIni);
        //    stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFim);
        //    stb.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}'; ", idCol);
        //    stb.Append("SELECT ISNULL(SUM(INPUT),0) AS INPUT ");
        //    stb.Append("   FROM GDA_CHECKING_ACCOUNT ");
        //    stb.Append("   WHERE CONVERT(DATE,RESULT_DATE) >= @DATAINICIAL ");
        //    stb.Append("     AND CONVERT(DATE,RESULT_DATE) <= @DATAFINAL ");
        //    stb.Append("	 AND COLLABORATOR_ID = @INPUTID ");
        //    stb.Append("     AND GDA_INDICATOR_IDGDA_INDICATOR IS NOT NULL ");
        //    using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
        //    {
        //        try
        //        {

        //            connection.Open();


        //            using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
        //            {
        //                using (SqlDataReader reader = command.ExecuteReader())
        //                {
        //                    command.CommandTimeout = 300;
        //                    if (reader.Read())
        //                    {
        //                        moedasGanhas = Convert.ToDouble(reader["INPUT"].ToString());
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception)
        //        {

        //            throw;
        //        }
        //    }


        //    //Query para pegar as moedas possiveis
        //    stb.Clear();
        //    stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtIni);
        //    stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFim);
        //    stb.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}'; ", idCol);
        //    //stb.Append("SELECT COUNT(0)  AS QTD,  ");
        //    stb.Append("SELECT ");
        //    stb.Append(" CONVERT(DATE, R.CREATED_AT) AS CREATED_AT, CASE ");
        //    stb.Append("		WHEN @INPUTID = MAX(CL.IDGDA_COLLABORATORS) THEN 'AGENTE' ");
        //    stb.Append("		WHEN @INPUTID = MAX(CL.MATRICULA_SUPERVISOR) THEN 'SUPERVISOR' ");
        //    stb.Append("		WHEN @INPUTID = MAX(CL.MATRICULA_COORDENADOR) THEN 'COORDENADOR' ");
        //    stb.Append("		WHEN @INPUTID = MAX(CL.MATRICULA_GERENTE_II) THEN 'GERENTE_II' ");
        //    stb.Append("		WHEN @INPUTID = MAX(CL.MATRICULA_GERENTE_I) THEN 'GERENTE_I' ");
        //    stb.Append("		WHEN @INPUTID = MAX(CL.MATRICULA_DIRETOR) THEN 'DIRETOR' ");
        //    stb.Append("		WHEN @INPUTID = MAX(CL.MATRICULA_CEO) THEN 'CEO' ");
        //    stb.Append("		ELSE '' END AS CARGO, ");
        //    stb.Append("       R.INDICADORID AS 'COD INDICADOR', ");
        //    //stb.Append("       SUM(HIG1.MONETIZATION) AS META_MAXIMA ");
        //    stb.Append("       MAX(HIG1.MONETIZATION) AS META_MAXIMA, ");
        //    //stb.Append("       SUM(HIG1.MONETIZATION) AS META_MAXIMA, ");
        //    //stb.Append("       CASE ");
        //    //stb.Append("           WHEN SUM(HIG1.MONETIZATION) IS NULL THEN 0 ");
        //    //stb.Append("           WHEN SUM(MZ.INPUT) IS NULL THEN 0 ");
        //    //stb.Append("           ELSE SUM(MZ.INPUT) ");
        //    //stb.Append("       END AS MOEDA_GANHA ");
        //    stb.Append(" MAX(CL.CARGO) AS CARGO_RESULT ");
        //    stb.Append("FROM GDA_RESULT (NOLOCK) AS R ");
        //    stb.Append("LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS CB ON CB.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
        //    stb.Append("LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CL ON CL.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
        //    stb.Append("AND CL.CREATED_AT = R.CREATED_AT ");
        //    stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
        //    stb.Append("AND HIG1.INDICATOR_ID = R.INDICADORID ");
        //    stb.Append("AND HIG1.SECTOR_ID = CL.IDGDA_SECTOR ");
        //    stb.Append("AND HIG1.GROUPID = 1 ");
        //    stb.Append("AND CONVERT(DATE,HIG1.STARTED_AT) <= R.CREATED_AT ");
        //    stb.Append("AND CONVERT(DATE,HIG1.ENDED_AT) >= R.CREATED_AT ");
        //    //stb.Append("LEFT JOIN ");
        //    //stb.Append("  (SELECT SUM(INPUT) AS INPUT, ");
        //    //stb.Append("          gda_indicator_idgda_indicator, ");
        //    //stb.Append("          result_date, ");
        //    //stb.Append("          COLLABORATOR_ID ");
        //    //stb.Append("   FROM GDA_CHECKING_ACCOUNT ");
        //    //stb.Append("   WHERE RESULT_DATE >= @DATAINICIAL ");
        //    //stb.Append("     AND RESULT_DATE <= @DATAFINAL ");
        //    //stb.Append("   GROUP BY gda_indicator_idgda_indicator, ");
        //    //stb.Append("            result_date, ");
        //    //stb.Append("            COLLABORATOR_ID) AS MZ ON MZ.gda_indicator_idgda_indicator = R.INDICADORID ");
        //    //stb.Append("AND MZ.result_date = R.CREATED_AT ");
        //    //stb.Append("AND MZ.COLLABORATOR_ID = R.IDGDA_COLLABORATORS ");
        //    stb.Append("WHERE 1 = 1 ");
        //    stb.Append("  AND CONVERT(DATE,R.CREATED_AT) >= @DATAINICIAL ");
        //    stb.Append("  AND CONVERT(DATE,R.CREATED_AT) <= @DATAFINAL ");
        //    stb.Append("  AND R.DELETED_AT IS NULL ");
        //    //stb.Append("  AND CL.IDGDA_SECTOR IS NOT NULL ");
        //    //stb.Append("  AND CL.CARGO IS NOT NULL ");
        //    //stb.Append("  AND CL.HOME_BASED <> '' ");
        //    stb.Append("  AND CL.active = 'true' ");
        //    stb.Append("  AND HIG1.MONETIZATION > 0 ");
        //    stb.Append("  AND ");
        //    stb.Append(" (CL.IDGDA_COLLABORATORS = @INPUTID OR   ");
        //    stb.Append("  CL.MATRICULA_SUPERVISOR = @INPUTID OR  ");
        //    stb.Append("  CL.MATRICULA_COORDENADOR = @INPUTID OR  ");
        //    stb.Append("  CL.MATRICULA_GERENTE_II = @INPUTID OR  ");
        //    stb.Append("  CL.MATRICULA_GERENTE_I = @INPUTID OR  ");
        //    stb.Append("  CL.MATRICULA_DIRETOR = @INPUTID OR  ");
        //    stb.Append("  CL.MATRICULA_CEO = @INPUTID) ");
        //    stb.Append(" AND R.FACTORS <> '0.000000;0.000000' ");
        //    stb.Append("GROUP BY R.INDICADORID, ");
        //    stb.Append("		CONVERT(DATE, R.CREATED_AT)  ");
        //    //adicionado
        //    stb.Append(", R.IDGDA_COLLABORATORS");

        //    List<basketIndicatorResults> bir = new List<basketIndicatorResults>();
        //    basketIndicatorResults birFinal = new basketIndicatorResults();

        //    using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
        //    {
        //        try
        //        {
        //            connection.Open();


        //            using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
        //            {
        //                command.CommandTimeout = 300;
        //                using (SqlDataReader reader = command.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        basketIndicatorResults bi = new basketIndicatorResults();

        //                        bi.cargo = reader["CARGO"].ToString();


        //                        if (reader["CARGO_RESULT"].ToString() == "")
        //                        {
        //                            bi.cargoResult = "Não Informado";
        //                        }
        //                        else
        //                        {
        //                            bi.cargoResult = reader["CARGO_RESULT"].ToString();
        //                        }

        //                        bi.codIndicator = Convert.ToInt32(reader["COD INDICADOR"].ToString());

        //                        bi.dataPagamento = reader["CREATED_AT"].ToString();

        //                        //bi.moedasGanhas = Convert.ToDouble(reader["MOEDA_GANHA"].ToString());

        //                        bi.moedasPossiveis = Convert.ToDouble(reader["META_MAXIMA"].ToString());

        //                        //bi.qtdPessoas = Convert.ToInt32(reader["QTD"].ToString());

        //                        bir.Add(bi);
        //                    }
        //                }
        //            }

        //            bir = bir.FindAll(item => item.cargoResult == "AGENTE" || item.cargoResult == "Não Informado").ToList();

        //            //Caso não retorne informação, retornar zerado para não dar erro pro front
        //            if (bir.Count() == 0)
        //            {
        //                rmams.coinsEarned = 0;
        //                rmams.coinsPossible = 0;
        //                rmams.groupName = "";
        //                rmams.idGroup = 0;
        //                rmams.groupAlias = "";
        //                rmams.groupImage = "";
        //                return rmams;
        //            }

        //            //Agrupamento em data e indicador
        //            bir = bir.GroupBy(item => new { item.dataPagamento, item.codIndicator }).Select(grupo => new basketIndicatorResults
        //            {
        //                cargo = grupo.First().cargo,
        //                codIndicator = grupo.Key.codIndicator,
        //                dataPagamento = grupo.Key.dataPagamento,
        //                moedasPossiveis = grupo.Sum(i => i.moedasPossiveis),
        //                qtdPessoas = grupo.Count(),
        //            }).ToList();


        //            if (bir.First().cargo == "AGENTE")
        //            {
        //                birFinal = bir
        //                    .GroupBy(item => new { item.cargo })
        //                    .Select(grupo => new basketIndicatorResults
        //                    {
        //                        //moedasGanhas = grupo.Sum(item => item.moedasGanhas),
        //                        moedasPossiveis = grupo.Sum(item => item.moedasPossiveis),
        //                        qtdPessoas = grupo.Count(),
        //                    }).First();
        //            }
        //            else
        //            {
        //                List<basketIndicatorResults> listHierarquia = new List<basketIndicatorResults>();
        //                listHierarquia = bir
        //                    .GroupBy(item => new { item.codIndicator, item.dataPagamento })
        //                    .Select(grupo => new basketIndicatorResults
        //                    {
        //                        dataPagamento = grupo.Key.dataPagamento,
        //                        codIndicator = grupo.Key.codIndicator,
        //                        // moedasGanhas = Math.Round(grupo.Sum(item => item.moedasGanhas) / grupo.Sum(item => item.qtdPessoas), 0, MidpointRounding.AwayFromZero),
        //                        //moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Count(), 2, MidpointRounding.AwayFromZero),
        //                        moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Sum(item => item.qtdPessoas), 2, MidpointRounding.AwayFromZero),
        //                        qtdPessoas = grupo.Count(),
        //                    }).ToList();


        //                //listHierarquia = listHierarquia
        //                //    .GroupBy(item => new {  item.dataPagamento })
        //                //    .Select(grupo => new basketIndicatorResults
        //                //    {
        //                //        dataPagamento = grupo.Key.dataPagamento,
        //                //        //codIndicator = grupo.Key.codIndicator,
        //                //        moedasGanhas = grupo.Sum(item => item.moedasGanhas),
        //                //        //moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Count(), 2, MidpointRounding.AwayFromZero),
        //                //        moedasPossiveis = grupo.Sum(item => item.moedasPossiveis),
        //                //        qtdPessoas = grupo.Count(),
        //                //    }).ToList();

        //                listHierarquia = listHierarquia
        //                    .GroupBy(item => new { item.codIndicator })
        //                    .Select(grupo => new basketIndicatorResults
        //                    {
        //                        codIndicator = grupo.Key.codIndicator,
        //                        //moedasGanhas = grupo.Sum(item => item.moedasGanhas),
        //                        //moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis) / grupo.Count(), 2, MidpointRounding.AwayFromZero),
        //                        moedasPossiveis = grupo.Sum(item => item.moedasPossiveis),
        //                        qtdPessoas = grupo.Count(),
        //                    }).ToList();

        //                birFinal = listHierarquia
        //                    .GroupBy(item => new { item.cargo })
        //                    .Select(grupo => new basketIndicatorResults
        //                    {
        //                        codIndicator = 0,
        //                        //moedasGanhas = Math.Round(grupo.Sum(item => item.moedasGanhas), 2, MidpointRounding.AwayFromZero),
        //                        moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis), 2, MidpointRounding.AwayFromZero),
        //                        qtdPessoas = grupo.Count(),
        //                    }).First();
        //            }


        //            //rmams.coinsEarned = birFinal.moedasGanhas;

        //            rmams.coinsEarned = moedasGanhas;
        //            rmams.coinsPossible = birFinal.moedasPossiveis;

        //            #region Antigo
        //            //Query Quantos ganhou
        //            //using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
        //            //{
        //            //    using (SqlDataReader reader = command.ExecuteReader())
        //            //    {
        //            //        if (reader.Read())
        //            //        {
        //            //            if (reader["INPUTWEIGHT"].ToString() == "")
        //            //            {
        //            //                rmams.coinsEarned = 0;
        //            //            }
        //            //            else
        //            //            {
        //            //                rmams.coinsEarned = Convert.ToInt32(reader["INPUTWEIGHT"].ToString());
        //            //            }                                
        //            //        }
        //            //    }
        //            //}

        //            //Query Quantos possivel


        //            //int valorMaximoMoedas = 0;


        //            //StringBuilder stb3 = new StringBuilder();
        //            //stb3.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}'; ", idCol);
        //            //stb3.Append("SELECT SUM(MONE) AS MAXIMO FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS DT ");
        //            //stb3.Append("LEFT JOIN GDA_HISTORY_COLLABORATOR_SECTOR (NOLOCK) AS CS ");
        //            //stb3.Append("ON CS.IDGDA_COLLABORATORS = DT.IDGDA_COLLABORATORS AND ");
        //            //stb3.Append("CS.CREATED_AT = DT.CREATED_AT ");
        //            //stb3.Append("LEFT JOIN GDA_RESULT (NOLOCK) AS R ON R.IDGDA_COLLABORATORS = DT.IDGDA_COLLABORATORS ");
        //            //stb3.Append("AND R.CREATED_AT = DT.CREATED_AT AND R.DELETED_AT IS NULL ");
        //            //stb3.Append("   LEFT JOIN ");
        //            //stb3.Append("     (SELECT DATE, SECTOR_ID, INDICATOR_ID, ");
        //            //stb3.Append("             SUM(MONETIZATION_G1*I.WEIGHT) AS MONE ");
        //            //stb3.Append("      FROM GDA_BASKET_INDICATOR (NOLOCK) AS GA ");
        //            //stb3.Append("	  INNER JOIN GDA_INDICATOR (NOLOCK) AS I ON GA.INDICATOR_ID = I.IDGDA_INDICATOR ");
        //            //stb3.Append("      WHERE DATE >= CONVERT(DATE, DATEADD(DAY, -30, GETDATE())) ");
        //            //stb3.Append("      GROUP BY DATE, SECTOR_ID, INDICATOR_ID) AS BI ");
        //            //stb3.Append("	  ON BI.DATE = DT.CREATED_AT AND ");
        //            //stb3.Append("	  BI.SECTOR_ID = CS.IDGDA_SECTOR AND ");
        //            //stb3.Append("	  BI.INDICATOR_ID = R.INDICADORID ");
        //            //stb3.Append("WHERE (DT.IDGDA_COLLABORATORS = @INPUTID OR  ");
        //            //stb3.Append("	    DT.MATRICULA_SUPERVISOR = @INPUTID OR ");
        //            //stb3.Append("		DT.MATRICULA_COORDENADOR = @INPUTID OR ");
        //            //stb3.Append("		DT.MATRICULA_GERENTE_II = @INPUTID OR ");
        //            //stb3.Append("		DT.MATRICULA_GERENTE_I = @INPUTID OR ");
        //            //stb3.Append("		DT.MATRICULA_DIRETOR = @INPUTID OR ");
        //            //stb3.Append("		DT.MATRICULA_CEO = @INPUTID) ");
        //            //stb3.Append("		AND DT.CREATED_AT >= CONVERT(DATE, DATEADD(DAY, -30, GETDATE())) ");
        //            ////stb3.Append("		GROUP BY DT.CREATED_AT, DT.IDGDA_COLLABORATORS");

        //            //using (SqlCommand command = new SqlCommand(stb3.ToString(), connection))
        //            //{
        //            //    using (SqlDataReader reader = command.ExecuteReader())
        //            //    {
        //            //        while (reader.Read())
        //            //        {
        //            //            try
        //            //            {
        //            //                string maximo = reader["MAXIMO"].ToString();
        //            //                if (maximo != "")
        //            //                {
        //            //                    valorMaximoMoedas = Convert.ToInt32(maximo);
        //            //                }
        //            //                else
        //            //                {
        //            //                    valorMaximoMoedas = 0;
        //            //                }

        //            //            }
        //            //            catch (Exception ex)
        //            //            {

        //            //                throw;
        //            //            }

        //            //        }
        //            //    }
        //            //}


        //            //DateTime today = DateTime.Now;
        //            //DateTime thirtyDaysAgo = today.AddDays(-30);
        //            // Realizar un bucle desde hace 30 días hasta hoy.
        //            //for (DateTime date = thirtyDaysAgo; date <= today; date = date.AddDays(1))
        //            //{

        //            //    StringBuilder stb2 = new StringBuilder();
        //            //    stb2.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}'; ", idCol);
        //            //    stb2.AppendFormat("DECLARE @DATEENV DATE; SET @DATEENV = '{0}'; ", date.ToString("yyyy-MM-dd"));
        //            //    stb2.Append(" ");
        //            //    stb2.Append("WITH HIERARCHYCTE AS  ");
        //            //    stb2.Append("  (SELECT IDGDA_HISTORY_HIERARCHY_RELATIONSHIP,  ");
        //            //    stb2.Append("          CONTRACTORCONTROLID,  ");
        //            //    stb2.Append("          PARENTIDENTIFICATION,  ");
        //            //    stb2.Append("          IDGDA_COLLABORATORS,  ");
        //            //    stb2.Append("          IDGDA_HIERARCHY,  ");
        //            //    stb2.Append("          CREATED_AT,  ");
        //            //    stb2.Append("          DELETED_AT,  ");
        //            //    stb2.Append("          TRANSACTIONID,  ");
        //            //    stb2.Append("          LEVELWEIGHT, DATE, LEVELNAME  ");
        //            //    stb2.Append("   FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK)  ");
        //            //    stb2.Append("   WHERE IDGDA_COLLABORATORS = @INPUTID  ");
        //            //    stb2.Append("     AND [DATE] = @DATEENV  ");
        //            //    stb2.Append("   UNION ALL SELECT H.IDGDA_HISTORY_HIERARCHY_RELATIONSHIP,  ");
        //            //    stb2.Append("                    H.CONTRACTORCONTROLID,  ");
        //            //    stb2.Append("                    H.PARENTIDENTIFICATION,  ");
        //            //    stb2.Append("                    H.IDGDA_COLLABORATORS,  ");
        //            //    stb2.Append("                    H.IDGDA_HIERARCHY,  ");
        //            //    stb2.Append("                    H.CREATED_AT,  ");
        //            //    stb2.Append("                    H.DELETED_AT,  ");
        //            //    stb2.Append("                    H.TRANSACTIONID,  ");
        //            //    stb2.Append("                    H.LEVELWEIGHT,  ");
        //            //    stb2.Append("                    H.DATE,  ");
        //            //    stb2.Append("                    H.LEVELNAME  ");
        //            //    stb2.Append("   FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP H (NOLOCK)  ");
        //            //    stb2.Append("   INNER JOIN HIERARCHYCTE CTE ON H.PARENTIDENTIFICATION = CTE.IDGDA_COLLABORATORS  ");
        //            //    stb2.Append("   WHERE CTE.LEVELNAME <> 'AGENTE'  ");
        //            //    stb2.Append("     AND CTE.[DATE] = @DATEENV )  ");
        //            //    stb2.Append(" ");
        //            //    stb2.Append("	 SELECT (SUM(PESSOAS)/SUM(QTD)) AS MAXIMO  FROM ");
        //            //    stb2.Append("	 ( ");
        //            //    stb2.Append("	SELECT GHC.IDGDA_SECTOR, COUNT(IDGDA_COLLABORATORS) AS QTD, MAX(TBL.MONE) AS M,    ");
        //            //    stb2.Append("	 CASE WHEN  ");
        //            //    stb2.Append("	  MAX(TBL.MONE) IS NULL THEN 0 ");
        //            //    stb2.Append("	 ELSE ");
        //            //    stb2.Append("	 (COUNT(IDGDA_COLLABORATORS)* MAX(TBL.MONE))  ");
        //            //    stb2.Append("	 END AS PESSOAS  ");
        //            //    stb2.Append("	 FROM GDA_HISTORY_COLLABORATOR_SECTOR (NOLOCK) AS GHC ");
        //            //    stb2.Append("	 LEFT JOIN  ");
        //            //    stb2.Append("	 ( ");
        //            //    stb2.Append("	 	   SELECT SECTOR_ID, SUM(MONETIZATION_G1*I.WEIGHT) AS MONE FROM GDA_BASKET_INDICATOR (NOLOCK) AS GA ");
        //            //    stb2.Append("		   INNER JOIN GDA_INDICATOR (NOLOCK) AS I ON GA.INDICATOR_ID = I.IDGDA_INDICATOR ");
        //            //    stb2.Append("		   WHERE DATE = @DATEENV ");
        //            //    stb2.Append("		   GROUP BY SECTOR_ID ");
        //            //    stb2.Append("	 ) AS TBL ON TBL.SECTOR_ID = GHC.IDGDA_SECTOR ");
        //            //    stb2.Append("	  ");
        //            //    stb2.Append("	 WHERE IDGDA_COLLABORATORS IN ( ");
        //            //    stb2.Append("	 SELECT DISTINCT(IDGDA_COLLABORATORS)  ");
        //            //    stb2.Append("     FROM HIERARCHYCTE  ");
        //            //    stb2.Append("     WHERE LEVELNAME = 'AGENTE'  ");
        //            //    stb2.Append("       AND DATE = @DATEENV ");
        //            //    stb2.Append("	   ) AND GHC.CREATED_AT = @DATEENV ");
        //            //    stb2.Append("	   GROUP BY GHC.IDGDA_SECTOR ");
        //            //    stb2.Append("	   ) AS TBL2 WHERE TBL2.PESSOAS > 0 ");


        //            //    using (SqlCommand command = new SqlCommand(stb2.ToString(), connection))
        //            //    {
        //            //        using (SqlDataReader reader = command.ExecuteReader())
        //            //        {
        //            //            if (reader.Read())
        //            //            {
        //            //                try
        //            //                {
        //            //                    string maximo = reader["MAXIMO"].ToString();
        //            //                    if (maximo != "")
        //            //                    {
        //            //                        valorMaximoMoedas = valorMaximoMoedas + Convert.ToInt32(maximo);
        //            //                    }

        //            //                }
        //            //                catch (Exception ex)
        //            //                {

        //            //                    throw;
        //            //                }

        //            //            }
        //            //        }
        //            //    }
        //            //}

        //            //rmams.coinsPossible = valorMaximoMoedas;
        //            #endregion

        //            //Realiza conta
        //            rmams.resultPercent = (rmams.coinsEarned / rmams.coinsPossible) * 100;

        //            //Como ele não teve como ganhar nenhuma moeda, ele atingiu 100% da meta
        //            if (rmams.coinsPossible == 0)
        //            {
        //                rmams.resultPercent = 100;
        //            }

        //            rmams.resultPercent = Math.Round(rmams.resultPercent, 2, MidpointRounding.AwayFromZero);

        //            List<basket> lbasket = returnTables.listBasketConfiguration();
        //            List<groups> lgroup = returnTables.listGroups("");
        //            basket lbasket1 = lbasket.Find(l => l.group_id == 1);
        //            basket lbasket2 = lbasket.Find(l => l.group_id == 2);
        //            basket lbasket3 = lbasket.Find(l => l.group_id == 3);
        //            basket lbasket4 = lbasket.Find(l => l.group_id == 4);
        //            groups lgroup1 = lgroup.Find(l => l.id == 1);
        //            groups lgroup2 = lgroup.Find(l => l.id == 2);
        //            groups lgroup3 = lgroup.Find(l => l.id == 3);
        //            groups lgroup4 = lgroup.Find(l => l.id == 4);

        //            if (rmams.resultPercent >= lbasket1.metric_min)
        //            {
        //                rmams.groupName = lgroup1.name;
        //                rmams.idGroup = lgroup1.id;
        //                rmams.groupAlias = lgroup1.alias;
        //                rmams.groupImage = lgroup1.image;
        //            }
        //            else if (rmams.resultPercent >= lbasket2.metric_min)
        //            {
        //                rmams.groupName = lgroup2.name;
        //                rmams.idGroup = lgroup2.id;
        //                rmams.groupAlias = lgroup2.alias;
        //                rmams.groupImage = lgroup2.image;
        //            }
        //            else if (rmams.resultPercent >= lbasket3.metric_min)
        //            {
        //                rmams.groupName = lgroup3.name;
        //                rmams.idGroup = lgroup3.id;
        //                rmams.groupAlias = lgroup3.alias;
        //                rmams.groupImage = lgroup3.image;
        //            }
        //            else if (rmams.resultPercent >= lbasket4.metric_min)
        //            {
        //                rmams.groupName = lgroup4.name;
        //                rmams.idGroup = lgroup4.id;
        //                rmams.groupAlias = lgroup4.alias;
        //                rmams.groupImage = lgroup4.image;
        //            }
        //            else
        //            {
        //                rmams.groupName = lgroup4.name;
        //                rmams.idGroup = lgroup4.id;
        //                rmams.groupAlias = lgroup4.alias;
        //                rmams.groupImage = lgroup4.image;
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            throw;
        //        }
        //        connection.Close();
        //    }
        //    return rmams;
        //}


    }
}