using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ResultsConsolidatedTempVersao1Controller : ApiController
    {
        //private string connectionString = "Data Source=database-1.cpmxhjwr8mgp.us-east-1.rds.amazonaws.com;Initial Catalog=GUILDA_PROD;User ID=RDSadminsql2019;Password=H4ND4faG3AhjFe943NyPLRhMsEamXucKdDa;MultipleActiveResultSets=True;Connection Timeout=10";
        private RepositorioDBContext db = new RepositorioDBContext();


        // POST: api/Results
        [ResponseType(typeof(ResultsModel))]
        public IHttpActionResult PostResultsModel(long t_id, IEnumerable<Result> results)
        {

            List<string> datas = new List<string>();

            datas.Clear();
            //datas.Add("2023-10-01");
            //datas.Add("2023-10-02");
            //datas.Add("2023-10-03");
            //datas.Add("2023-10-04");
            //datas.Add("2023-10-05");
            //datas.Add("2023-10-06");
            //datas.Add("2023-10-07");
            //datas.Add("2023-10-08");
            //datas.Add("2023-10-09");
            //datas.Add("2023-10-10");
            //datas.Add("2023-10-11");
            //datas.Add("2023-10-12");
            //datas.Add("2023-10-13");
            //datas.Add("2023-10-14");
            //datas.Add("2023-10-15");
            //datas.Add("2023-10-16");
            //datas.Add("2023-10-17");
            //datas.Add("2023-10-18");
            //datas.Add("2023-10-19");
            //datas.Add("2023-10-20");
            //datas.Add("2023-10-21");
            //datas.Add("2023-10-22");
            //datas.Add("2023-10-23");
            //datas.Add("2023-10-24");
            //datas.Add("2023-10-25");
            //datas.Add("2023-10-26");
            //datas.Add("2023-10-27");
            //datas.Add("2023-10-28");
            //datas.Add("2023-10-29");
            //datas.Add("2023-10-30");
            //datas.Add("2023-10-31");
            //datas.Add("2023-11-01");
            //datas.Add("2023-11-02");
            //datas.Add("2023-11-03");
            //datas.Add("2023-11-04");
            //datas.Add("2023-11-05");
            //datas.Add("2023-11-06");
            //datas.Add("2023-11-07");
            //foreach (string dt in datas)
            //{
            //    insereTabelasAuxiliares(dt);
            //}

           

            monetization(0);

            return StatusCode(HttpStatusCode.Created);
        }

        public void insereTabelasAuxiliares(string dt)
        {
            //string connectionString = "Data Source=database-1.cpmxhjwr8mgp.us-east-1.rds.amazonaws.com;Initial Catalog=GUILDA_PROD;User ID=RDSadminsql2019;Password=H4ND4faG3AhjFe943NyPLRhMsEamXucKdDa;MultipleActiveResultSets=True;Connection Timeout=10";
            SqlConnection connection = new SqlConnection(Database.Conn);
            try
            {
                connection.Open();
                StringBuilder stb1 = new StringBuilder();
                stb1.Append("CREATE TABLE [dbo].[#TEMPAG]( ");
                stb1.Append("	[IDGDA_COLLABORATORS] [int] NULL,");
                stb1.Append("   [CARGO] [nvarchar](50) NULL,");
                stb1.Append("   [CREATED_AT] [datetime2](7) NULL,");
                stb1.Append("	[IDGDA_SECTOR] [int] NULL,");
                stb1.Append("	[HOME_BASED] [nvarchar](100) NULL,");
                stb1.Append("	[SITE] [nvarchar](100) NULL,");
                stb1.Append("	[PERIODO] [nvarchar](100) NULL,");
                stb1.Append("	[MATRICULA_SUPERVISOR] [int] NULL,");
                stb1.Append("	[NOME_SUPERVISOR] [nvarchar](200) NULL,");
                stb1.Append("	[MATRICULA_COORDENADOR] [int] NULL,");
                stb1.Append("	[NOME_COORDENADOR] [nvarchar](200) NULL,");
                stb1.Append("	[MATRICULA_GERENTE_II] [int] NULL,");
                stb1.Append("	[NOME_GERENTE_II] [nvarchar](200) NULL,");
                stb1.Append("	[MATRICULA_GERENTE_I] [int] NULL,");
                stb1.Append("	[NOME_GERENTE_I] [nvarchar](200) NULL,");
                stb1.Append("	[MATRICULA_DIRETOR] [int] NULL,");
                stb1.Append("	[NOME_DIRETOR] [nvarchar](200) NULL,");
                stb1.Append("	[MATRICULA_CEO] [int] NULL,");
                stb1.Append("	[NOME_CEO] [nvarchar](200) NULL,");
                stb1.Append("	[ACTIVE] [nvarchar](10) NULL)");

                using (SqlCommand createTempTableCommand = new SqlCommand(stb1.ToString(), connection))
                {
                    createTempTableCommand.ExecuteNonQuery();
                }



                StringBuilder temp = new StringBuilder();
                temp.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}';", dt);
                temp.Append("INSERT INTO #TEMPAG (IDGDA_COLLABORATORS, CARGO, CREATED_AT, IDGDA_SECTOR, HOME_BASED, SITE, PERIODO, MATRICULA_SUPERVISOR, NOME_SUPERVISOR, MATRICULA_COORDENADOR,  ");
                temp.Append("NOME_COORDENADOR, MATRICULA_GERENTE_II, NOME_GERENTE_II, MATRICULA_GERENTE_I, NOME_GERENTE_I, MATRICULA_DIRETOR, NOME_DIRETOR, MATRICULA_CEO, NOME_CEO, ACTIVE)  ");
                temp.Append("SELECT CB.IDGDA_COLLABORATORS, MAX(CLS.LEVELNAME), @DATAINICIAL, MAX(S.IDGDA_SECTOR) AS IDGDA_SECTOR,  ");
                temp.Append("	   MAX(CASE  ");
                temp.Append("               WHEN A.NAME = 'HOME_BASED' THEN A.VALUE  ");
                temp.Append("               ELSE ''  ");
                temp.Append("           END) AS HOME_BASED,  ");
                temp.Append("       MAX(CASE  ");
                temp.Append("               WHEN A.NAME = 'SITE' THEN A.VALUE  ");
                temp.Append("               ELSE ''  ");
                temp.Append("           END) AS SITE,  ");
                temp.Append("       MAX(CASE  ");
                temp.Append("               WHEN A.NAME = 'PERIODO' THEN A.VALUE  ");
                temp.Append("               ELSE ''  ");
                temp.Append("           END) AS TURNO,  ");
                temp.Append("       MAX(CASE  ");
                temp.Append("               WHEN HIERARCHY.LEVELWEIGHT = '2' THEN CAST(HIERARCHY.PARENTIDENTIFICATION AS VARCHAR(255))  ");
                temp.Append("               ELSE '-'  ");
                temp.Append("           END) AS 'MATRICULA SUPERVISOR',  ");
                temp.Append("       MAX(CASE  ");
                temp.Append("               WHEN HIERARCHY.LEVELWEIGHT = '2' THEN HIERARCHY.NAME  ");
                temp.Append("               ELSE '-'  ");
                temp.Append("           END) AS 'NOME SUPERVISOR',  ");
                temp.Append("       MAX(CASE  ");
                temp.Append("               WHEN HIERARCHY.LEVELWEIGHT = '3' THEN CAST(HIERARCHY.PARENTIDENTIFICATION AS VARCHAR(255))  ");
                temp.Append("               ELSE '-'  ");
                temp.Append("           END) AS 'MATRICULA COORDENADOR',  ");
                temp.Append("       MAX(CASE  ");
                temp.Append("               WHEN HIERARCHY.LEVELWEIGHT = '3' THEN HIERARCHY.NAME  ");
                temp.Append("               ELSE '-'  ");
                temp.Append("           END) AS 'NOME COORDENADOR',  ");
                temp.Append("       MAX(CASE  ");
                temp.Append("               WHEN HIERARCHY.LEVELWEIGHT = '4' THEN CAST(HIERARCHY.PARENTIDENTIFICATION AS VARCHAR(255))  ");
                temp.Append("               ELSE '-'  ");
                temp.Append("           END) AS 'MATRICULA GERENTE II',  ");
                temp.Append("       MAX(CASE  ");
                temp.Append("               WHEN HIERARCHY.LEVELWEIGHT = '4' THEN HIERARCHY.NAME  ");
                temp.Append("               ELSE '-'  ");
                temp.Append("           END) AS 'NOME GERENTE II',  ");
                temp.Append("       MAX(CASE  ");
                temp.Append("               WHEN HIERARCHY.LEVELWEIGHT = '5' THEN CAST(HIERARCHY.PARENTIDENTIFICATION AS VARCHAR(255))  ");
                temp.Append("               ELSE '-'  ");
                temp.Append("           END) AS 'MATRICULA GERENTE I',  ");
                temp.Append("       MAX(CASE  ");
                temp.Append("               WHEN HIERARCHY.LEVELWEIGHT = '5' THEN HIERARCHY.NAME  ");
                temp.Append("               ELSE '-'  ");
                temp.Append("           END) AS 'NOME GERENTE I',  ");
                temp.Append("       MAX(CASE  ");
                temp.Append("               WHEN HIERARCHY.LEVELWEIGHT = '6' THEN CAST(HIERARCHY.PARENTIDENTIFICATION AS VARCHAR(255))  ");
                temp.Append("               ELSE '-'  ");
                temp.Append("           END) AS 'MATRICULA DIRETOR',  ");
                temp.Append("       MAX(CASE  ");
                temp.Append("               WHEN HIERARCHY.LEVELWEIGHT = '6' THEN HIERARCHY.NAME  ");
                temp.Append("               ELSE '-'  ");
                temp.Append("           END) AS 'NOME DIRETOR',  ");
                temp.Append("       MAX(CASE  ");
                temp.Append("               WHEN HIERARCHY.LEVELWEIGHT = '7' THEN HIERARCHY.PARENTIDENTIFICATION  ");
                temp.Append("               ELSE '-'  ");
                temp.Append("           END) AS 'MATRICULA CEO',  ");
                temp.Append("       MAX(CASE  ");
                temp.Append("               WHEN HIERARCHY.LEVELWEIGHT = '7' THEN HIERARCHY.NAME  ");
                temp.Append("               ELSE '-'  ");
                temp.Append("           END) AS 'NOME CEO',  ");
                temp.Append("           MAX(CAT.ACTIVE) AS ACTIVE ");
                temp.Append(" FROM GDA_COLLABORATORS (NOLOCK) AS CB  ");
                temp.Append("LEFT JOIN GDA_HISTORY_COLLABORATOR_ACTIVE (NOLOCK) CAT ON CAT.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS AND CAT.ENTRYDATE = @DATAINICIAL ");
                temp.Append("LEFT JOIN GDA_HISTORY_COLLABORATOR_SECTOR (NOLOCK) S ON S.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS AND S.CREATED_AT = @DATAINICIAL  ");
                temp.Append("LEFT JOIN GDA_ATRIBUTES (NOLOCK) AS A ON (A.NAME = 'HOME_BASED'  ");
                temp.Append("                                          OR A.NAME = 'SITE'  ");
                temp.Append("                                          OR A.NAME = 'PERIODO')  ");
                temp.Append("AND A.CREATED_AT = @DATAINICIAL  ");
                temp.Append("AND A.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS  ");
                temp.Append("LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS CLS ON CLS.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS ");
                temp.Append("AND CLS.DATE = @DATAINICIAL ");
                temp.Append("LEFT JOIN  ");
                temp.Append("  (SELECT COD,  ");
                temp.Append("          IDGDA_COLLABORATORS,  ");
                temp.Append("          PARENTIDENTIFICATION,  ");
                temp.Append("          NAME,  ");
                temp.Append("          LEVELWEIGHT  ");
                temp.Append("   FROM  ");
                temp.Append("     (SELECT LV1.IDGDA_COLLABORATORS AS COD,  ");
                temp.Append("             LV1.IDGDA_COLLABORATORS,  ");
                temp.Append("             ISNULL(LV1.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION,  ");
                temp.Append("             C.NAME,  ");
                temp.Append("             CASE  ");
                temp.Append("                 WHEN LV2.LEVELWEIGHT IS NULL  ");
                temp.Append("                      AND LV1.PARENTIDENTIFICATION IS NOT NULL THEN '2'  ");
                temp.Append("                 ELSE LV2.LEVELWEIGHT  ");
                temp.Append("             END AS LEVELWEIGHT  ");
                temp.Append("      FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1  ");
                temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS  ");
                temp.Append("      AND LV2.DATE = @DATAINICIAL  ");
                temp.Append("      LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV1.PARENTIDENTIFICATION  ");
                temp.Append("      WHERE LV1.DATE = @DATAINICIAL  ");
                temp.Append("      UNION ALL SELECT LV1.IDGDA_COLLABORATORS AS COD,  ");
                temp.Append("                       LV2.IDGDA_COLLABORATORS,  ");
                temp.Append("                       ISNULL(LV2.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION,  ");
                temp.Append("                       C.NAME,  ");
                temp.Append("                       CASE  ");
                temp.Append("                           WHEN LV3.LEVELWEIGHT IS NULL  ");
                temp.Append("                                AND LV2.PARENTIDENTIFICATION IS NOT NULL THEN '3'  ");
                temp.Append("                           ELSE LV3.LEVELWEIGHT  ");
                temp.Append("                       END AS LEVELWEIGHT  ");
                temp.Append("      FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1  ");
                temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS  ");
                temp.Append("      AND LV2.DATE = @DATAINICIAL  ");
                temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS  ");
                temp.Append("      AND LV3.DATE = @DATAINICIAL  ");
                temp.Append("      LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV2.PARENTIDENTIFICATION  ");
                temp.Append("      WHERE LV1.DATE = @DATAINICIAL  ");
                temp.Append("      UNION ALL SELECT LV1.IDGDA_COLLABORATORS AS COD,  ");
                temp.Append("                       LV3.IDGDA_COLLABORATORS,  ");
                temp.Append("                       ISNULL(LV3.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION,  ");
                temp.Append("                       C.NAME,  ");
                temp.Append("                       CASE  ");
                temp.Append("                           WHEN LV4.LEVELWEIGHT IS NULL  ");
                temp.Append("                                AND LV3.PARENTIDENTIFICATION IS NOT NULL THEN '4'  ");
                temp.Append("                           ELSE LV4.LEVELWEIGHT  ");
                temp.Append("                       END AS LEVELWEIGHT  ");
                temp.Append("      FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1  ");
                temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS  ");
                temp.Append("      AND LV2.DATE = @DATAINICIAL  ");
                temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS  ");
                temp.Append("      AND LV3.DATE = @DATAINICIAL  ");
                temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV4 ON LV3.PARENTIDENTIFICATION = LV4.IDGDA_COLLABORATORS  ");
                temp.Append("      AND LV4.DATE = @DATAINICIAL  ");
                temp.Append("      LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV3.PARENTIDENTIFICATION  ");
                temp.Append("      WHERE LV1.DATE = @DATAINICIAL  ");
                temp.Append("      UNION ALL SELECT LV1.IDGDA_COLLABORATORS AS COD,  ");
                temp.Append("                       LV4.IDGDA_COLLABORATORS,  ");
                temp.Append("                       ISNULL(LV4.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION,  ");
                temp.Append("                       C.NAME,  ");
                temp.Append("                       CASE  ");
                temp.Append("                           WHEN LV5.LEVELWEIGHT IS NULL  ");
                temp.Append("                                AND LV4.PARENTIDENTIFICATION IS NOT NULL THEN '5'  ");
                temp.Append("                           ELSE LV5.LEVELWEIGHT  ");
                temp.Append("                       END AS LEVELWEIGHT  ");
                temp.Append("      FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1  ");
                temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS  ");
                temp.Append("      AND LV2.DATE = @DATAINICIAL  ");
                temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS  ");
                temp.Append("      AND LV3.DATE = @DATAINICIAL  ");
                temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV4 ON LV3.PARENTIDENTIFICATION = LV4.IDGDA_COLLABORATORS  ");
                temp.Append("      AND LV4.DATE = @DATAINICIAL  ");
                temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV5 ON LV4.PARENTIDENTIFICATION = LV5.IDGDA_COLLABORATORS  ");
                temp.Append("      AND LV5.DATE = @DATAINICIAL  ");
                temp.Append("      LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV4.PARENTIDENTIFICATION  ");
                temp.Append("      WHERE LV1.DATE = @DATAINICIAL  ");
                temp.Append("      UNION ALL SELECT LV1.IDGDA_COLLABORATORS AS COD,  ");
                temp.Append("                       LV5.IDGDA_COLLABORATORS,  ");
                temp.Append("                       ISNULL(LV5.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION,  ");
                temp.Append("                       C.NAME,  ");
                temp.Append("                       CASE  ");
                temp.Append("                           WHEN LV6.LEVELWEIGHT IS NULL  ");
                temp.Append("                                AND LV5.PARENTIDENTIFICATION IS NOT NULL THEN '6'  ");
                temp.Append("                           ELSE LV6.LEVELWEIGHT  ");
                temp.Append("                       END AS LEVELWEIGHT  ");
                temp.Append("      FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1  ");
                temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS  ");
                temp.Append("      AND LV2.DATE = @DATAINICIAL  ");
                temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS  ");
                temp.Append("      AND LV3.DATE = @DATAINICIAL  ");
                temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV4 ON LV3.PARENTIDENTIFICATION = LV4.IDGDA_COLLABORATORS  ");
                temp.Append("      AND LV4.DATE = @DATAINICIAL  ");
                temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV5 ON LV4.PARENTIDENTIFICATION = LV5.IDGDA_COLLABORATORS  ");
                temp.Append("      AND LV5.DATE = @DATAINICIAL  ");
                temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV6 ON LV5.PARENTIDENTIFICATION = LV6.IDGDA_COLLABORATORS  ");
                temp.Append("      AND LV6.DATE = @DATAINICIAL  ");
                temp.Append("      LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV5.PARENTIDENTIFICATION  ");
                temp.Append("      WHERE LV1.DATE = @DATAINICIAL  ");
                temp.Append("      UNION ALL SELECT LV1.IDGDA_COLLABORATORS AS COD,  ");
                temp.Append("                       LV6.IDGDA_COLLABORATORS,  ");
                temp.Append("                       ISNULL(LV6.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION,  ");
                temp.Append("                       C.NAME,  ");
                temp.Append("                       CASE  ");
                temp.Append("                           WHEN LV7.LEVELWEIGHT IS NULL  ");
                temp.Append("                                AND LV6.PARENTIDENTIFICATION IS NOT NULL THEN '7'  ");
                temp.Append("                           ELSE LV7.LEVELWEIGHT  ");
                temp.Append("                       END AS LEVELWEIGHT  ");
                temp.Append("      FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1  ");
                temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS  ");
                temp.Append("      AND LV2.DATE = @DATAINICIAL  ");
                temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS  ");
                temp.Append("      AND LV3.DATE = @DATAINICIAL  ");
                temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV4 ON LV3.PARENTIDENTIFICATION = LV4.IDGDA_COLLABORATORS  ");
                temp.Append("      AND LV4.DATE = @DATAINICIAL  ");
                temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV5 ON LV4.PARENTIDENTIFICATION = LV5.IDGDA_COLLABORATORS  ");
                temp.Append("      AND LV5.DATE = @DATAINICIAL  ");
                temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV6 ON LV5.PARENTIDENTIFICATION = LV6.IDGDA_COLLABORATORS  ");
                temp.Append("      AND LV6.DATE = @DATAINICIAL  ");
                temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV7 ON LV6.PARENTIDENTIFICATION = LV7.IDGDA_COLLABORATORS  ");
                temp.Append("      AND LV7.DATE = @DATAINICIAL  ");
                temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV8 ON LV7.PARENTIDENTIFICATION = LV8.IDGDA_COLLABORATORS  ");
                temp.Append("      AND LV8.DATE = @DATAINICIAL  ");
                temp.Append("      LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV6.PARENTIDENTIFICATION  ");
                temp.Append("      WHERE LV1.DATE = @DATAINICIAL ) AS COMBINEDDATA) AS HIERARCHY ON HIERARCHY.COD = CB.IDGDA_COLLABORATORS  ");
                temp.Append("	  WHERE 1=1  ");
                temp.Append("	  GROUP BY CB.IDGDA_COLLABORATORS; ");

                SqlCommand createTableCommandteste = new SqlCommand(temp.ToString(), connection);
                createTableCommandteste.CommandTimeout = 0;
                createTableCommandteste.ExecuteNonQuery();

                //Verifica se tem informações para serem atualizadas.. caso esteja rodando em um horario que não tenha ainda hieraquia ou atributos
                StringBuilder stb3 = new StringBuilder();
                stb3.Append("SELECT COUNT(0) AS QTD FROM #TEMPAG ");
                stb3.Append("WHERE IDGDA_SECTOR IS NOT NULL AND MATRICULA_SUPERVISOR <> 0 ");
                int qtd = 0;
                using (SqlCommand command = new SqlCommand(stb3.ToString(), connection))
                {
                    command.CommandTimeout = 60;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            qtd = Convert.ToInt32(reader["QTD"].ToString());
                        }
                    }
                }

                if (qtd > 0)
                {
                    //Atualizar deleted_at dos resultados que tiveram modificação [DESCOMENTAR QUANDO EXISTIR COLUNA]
                    StringBuilder queryInsertResult1 = new StringBuilder();
                    queryInsertResult1.Append("MERGE INTO GDA_COLLABORATORS_LAST_DETAILS AS TARGET  ");
                    queryInsertResult1.Append("USING #TEMPAG AS SOURCE  ");
                    queryInsertResult1.Append("ON (TARGET.IDGDA_COLLABORATORS = SOURCE.IDGDA_COLLABORATORS)  ");
                    queryInsertResult1.Append("WHEN NOT MATCHED BY TARGET THEN  ");
                    queryInsertResult1.Append("  INSERT (IDGDA_COLLABORATORS, CARGO, IDGDA_SECTOR, HOME_BASED, SITE, PERIODO, MATRICULA_SUPERVISOR, NOME_SUPERVISOR, MATRICULA_COORDENADOR,  ");
                    queryInsertResult1.Append("		  NOME_COORDENADOR, MATRICULA_GERENTE_II, NOME_GERENTE_II, MATRICULA_GERENTE_I, NOME_GERENTE_I, MATRICULA_DIRETOR, NOME_DIRETOR, MATRICULA_CEO, NOME_CEO, ACTIVE)  ");
                    queryInsertResult1.Append("  VALUES (SOURCE.IDGDA_COLLABORATORS, SOURCE.CARGO, SOURCE.IDGDA_SECTOR, SOURCE.HOME_BASED, SOURCE.SITE, SOURCE.PERIODO, SOURCE.MATRICULA_SUPERVISOR, SOURCE.NOME_SUPERVISOR, SOURCE.MATRICULA_COORDENADOR,  ");
                    queryInsertResult1.Append("		  SOURCE.NOME_COORDENADOR, SOURCE.MATRICULA_GERENTE_II, SOURCE.NOME_GERENTE_II, SOURCE.MATRICULA_GERENTE_I, SOURCE.NOME_GERENTE_I, SOURCE.MATRICULA_DIRETOR, SOURCE.NOME_DIRETOR, SOURCE.MATRICULA_CEO, SOURCE.NOME_CEO, SOURCE.ACTIVE);  ");
                    //queryInsertResult1.Append("WHEN MATCHED THEN  ");
                    //queryInsertResult1.Append("  UPDATE SET  ");
                    //queryInsertResult1.Append("  TARGET.IDGDA_SECTOR = SOURCE.IDGDA_SECTOR, ");
                    //queryInsertResult1.Append("  TARGET.HOME_BASED = SOURCE.HOME_BASED, ");
                    //queryInsertResult1.Append("  TARGET.SITE = SOURCE.SITE, ");
                    //queryInsertResult1.Append("  TARGET.PERIODO = SOURCE.PERIODO, ");
                    //queryInsertResult1.Append("  TARGET.MATRICULA_SUPERVISOR = SOURCE.MATRICULA_SUPERVISOR, ");
                    //queryInsertResult1.Append("  TARGET.NOME_SUPERVISOR = SOURCE.NOME_SUPERVISOR, ");
                    //queryInsertResult1.Append("  TARGET.MATRICULA_COORDENADOR = SOURCE.MATRICULA_COORDENADOR, ");
                    //queryInsertResult1.Append("  TARGET.NOME_COORDENADOR = SOURCE.NOME_COORDENADOR, ");
                    //queryInsertResult1.Append("  TARGET.MATRICULA_GERENTE_II = SOURCE.MATRICULA_GERENTE_II, ");
                    //queryInsertResult1.Append("  TARGET.NOME_GERENTE_II = SOURCE.NOME_GERENTE_II, ");
                    //queryInsertResult1.Append("  TARGET.MATRICULA_GERENTE_I = SOURCE.MATRICULA_GERENTE_I, ");
                    //queryInsertResult1.Append("  TARGET.NOME_GERENTE_I = SOURCE.NOME_GERENTE_I, ");
                    //queryInsertResult1.Append("  TARGET.MATRICULA_DIRETOR = SOURCE.MATRICULA_DIRETOR, ");
                    //queryInsertResult1.Append("  TARGET.NOME_DIRETOR = SOURCE.NOME_DIRETOR, ");
                    //queryInsertResult1.Append("  TARGET.MATRICULA_CEO = SOURCE.MATRICULA_CEO, ");
                    //queryInsertResult1.Append("  TARGET.NOME_CEO = SOURCE.NOME_CEO; ");

                    SqlCommand createTableCommand1 = new SqlCommand(queryInsertResult1.ToString(), connection);
                    createTableCommand1.CommandTimeout = 0;
                    createTableCommand1.ExecuteNonQuery();

                    StringBuilder queryUp = new StringBuilder();
                    queryUp.Append("UPDATE D1 SET D1.CARGO = D2.CARGO ");
                    queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                    queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                    queryUp.Append("WHERE D2.CARGO IS NOT NULL ");
                    SqlCommand createnews1 = new SqlCommand(queryUp.ToString(), connection);
                    createnews1.CommandTimeout = 0;
                    createnews1.ExecuteNonQuery();

                    queryUp.Clear();
                    queryUp.Append("UPDATE D1 SET D1.IDGDA_SECTOR = D2.IDGDA_SECTOR ");
                    queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                    queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                    queryUp.Append("WHERE D2.IDGDA_SECTOR IS NOT NULL ");
                    SqlCommand createnew1 = new SqlCommand(queryUp.ToString(), connection);
                    createnew1.CommandTimeout = 0;
                    createnew1.ExecuteNonQuery();

                    queryUp.Clear();
                    queryUp.Append("UPDATE D1 SET D1.HOME_BASED = D2.HOME_BASED ");
                    queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                    queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                    queryUp.Append("WHERE D2.HOME_BASED <> '' ");
                    SqlCommand createnew2 = new SqlCommand(queryUp.ToString(), connection);
                    createnew2.CommandTimeout = 0;
                    createnew2.ExecuteNonQuery();

                    queryUp.Clear();
                    queryUp.Append("UPDATE D1 SET D1.SITE = D2.SITE ");
                    queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                    queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                    queryUp.Append("WHERE D2.SITE <> '' ");
                    SqlCommand createnew3 = new SqlCommand(queryUp.ToString(), connection);
                    createnew3.CommandTimeout = 0;
                    createnew3.ExecuteNonQuery();

                    queryUp.Clear();
                    queryUp.Append("UPDATE D1 SET D1.PERIODO = D2.PERIODO ");
                    queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                    queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                    queryUp.Append("WHERE D2.PERIODO <> '' ");
                    SqlCommand createnew4 = new SqlCommand(queryUp.ToString(), connection);
                    createnew4.CommandTimeout = 0;
                    createnew4.ExecuteNonQuery();


                    queryUp.Clear();
                    queryUp.Append("UPDATE D1 SET D1.MATRICULA_SUPERVISOR = D2.MATRICULA_SUPERVISOR ");
                    queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                    queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                    queryUp.Append("WHERE D2.MATRICULA_SUPERVISOR <> '0' ");
                    SqlCommand createUp1 = new SqlCommand(queryUp.ToString(), connection);
                    createUp1.CommandTimeout = 0;
                    createUp1.ExecuteNonQuery();

                    queryUp.Clear();
                    queryUp.Append("UPDATE D1 SET D1.NOME_SUPERVISOR = D2.NOME_SUPERVISOR ");
                    queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                    queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                    queryUp.Append("WHERE D2.NOME_SUPERVISOR <> '-' ");
                    SqlCommand createUp2 = new SqlCommand(queryUp.ToString(), connection);
                    createUp2.CommandTimeout = 0;
                    createUp2.ExecuteNonQuery();

                    queryUp.Clear();
                    queryUp.Append("UPDATE D1 SET D1.MATRICULA_COORDENADOR = D2.MATRICULA_COORDENADOR ");
                    queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                    queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                    queryUp.Append("WHERE D2.MATRICULA_COORDENADOR <> '0' ");
                    SqlCommand createUp3 = new SqlCommand(queryUp.ToString(), connection);
                    createUp3.CommandTimeout = 0;
                    createUp3.ExecuteNonQuery();

                    queryUp.Clear();
                    queryUp.Append("UPDATE D1 SET D1.NOME_COORDENADOR = D2.NOME_COORDENADOR ");
                    queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                    queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                    queryUp.Append("WHERE D2.NOME_COORDENADOR <> '-' ");
                    SqlCommand createUp4 = new SqlCommand(queryUp.ToString(), connection);
                    createUp4.CommandTimeout = 0;
                    createUp4.ExecuteNonQuery();

                    queryUp.Clear();
                    queryUp.Append("UPDATE D1 SET D1.MATRICULA_GERENTE_II = D2.MATRICULA_GERENTE_II ");
                    queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                    queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                    queryUp.Append("WHERE D2.MATRICULA_GERENTE_II <> '0' ");
                    SqlCommand createUp5 = new SqlCommand(queryUp.ToString(), connection);
                    createUp5.CommandTimeout = 0;
                    createUp5.ExecuteNonQuery();

                    queryUp.Clear();
                    queryUp.Append("UPDATE D1 SET D1.NOME_GERENTE_II = D2.NOME_GERENTE_II ");
                    queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                    queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                    queryUp.Append("WHERE D2.NOME_GERENTE_II <> '-' ");
                    SqlCommand createUp6 = new SqlCommand(queryUp.ToString(), connection);
                    createUp6.CommandTimeout = 0;
                    createUp6.ExecuteNonQuery();

                    queryUp.Clear();
                    queryUp.Append("UPDATE D1 SET D1.MATRICULA_GERENTE_I = D2.MATRICULA_GERENTE_I ");
                    queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                    queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                    queryUp.Append("WHERE D2.MATRICULA_GERENTE_I <> '0' ");
                    SqlCommand createUp7 = new SqlCommand(queryUp.ToString(), connection);
                    createUp7.CommandTimeout = 0;
                    createUp7.ExecuteNonQuery();

                    queryUp.Clear();
                    queryUp.Append("UPDATE D1 SET D1.NOME_GERENTE_I = D2.NOME_GERENTE_I ");
                    queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                    queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                    queryUp.Append("WHERE D2.NOME_GERENTE_I <> '-' ");
                    SqlCommand createUp8 = new SqlCommand(queryUp.ToString(), connection);
                    createUp8.CommandTimeout = 0;
                    createUp8.ExecuteNonQuery();

                    queryUp.Clear();
                    queryUp.Append("UPDATE D1 SET D1.MATRICULA_DIRETOR = D2.MATRICULA_DIRETOR ");
                    queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                    queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                    queryUp.Append("WHERE D2.MATRICULA_DIRETOR <> '-' ");
                    SqlCommand createUp9 = new SqlCommand(queryUp.ToString(), connection);
                    createUp9.CommandTimeout = 0;
                    createUp9.ExecuteNonQuery();

                    queryUp.Clear();
                    queryUp.Append("UPDATE D1 SET D1.NOME_DIRETOR = D2.NOME_DIRETOR ");
                    queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                    queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                    queryUp.Append("WHERE D2.NOME_DIRETOR <> '-' ");
                    SqlCommand createUp10 = new SqlCommand(queryUp.ToString(), connection);
                    createUp10.CommandTimeout = 0;
                    createUp10.ExecuteNonQuery();

                    queryUp.Clear();
                    queryUp.Append("UPDATE D1 SET D1.MATRICULA_CEO = D2.MATRICULA_CEO ");
                    queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                    queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                    queryUp.Append("WHERE D2.MATRICULA_CEO <> '-' ");
                    SqlCommand createUp11 = new SqlCommand(queryUp.ToString(), connection);
                    createUp11.CommandTimeout = 0;
                    createUp11.ExecuteNonQuery();

                    queryUp.Clear();
                    queryUp.Append("UPDATE D1 SET D1.NOME_CEO = D2.NOME_CEO ");
                    queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                    queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                    queryUp.Append("WHERE D2.NOME_CEO <> '-' ");
                    SqlCommand createUp12 = new SqlCommand(queryUp.ToString(), connection);
                    createUp12.CommandTimeout = 0;
                    createUp12.ExecuteNonQuery();

                    queryUp.Clear();
                    queryUp.Append("UPDATE D1 SET D1.ACTIVE = D2.ACTIVE ");
                    queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                    queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                    queryUp.Append("WHERE D2.ACTIVE IS NOT NULL ");
                    SqlCommand createUp13 = new SqlCommand(queryUp.ToString(), connection);
                    createUp13.CommandTimeout = 0;
                    createUp13.ExecuteNonQuery();


                    //A
                    StringBuilder queryInsertResult2 = new StringBuilder();
                    queryInsertResult2.Append("MERGE INTO GDA_COLLABORATORS_DETAILS AS TARGET  ");
                    queryInsertResult2.Append("USING #TEMPAG AS SOURCE  ");
                    queryInsertResult2.Append("ON (TARGET.IDGDA_COLLABORATORS = SOURCE.IDGDA_COLLABORATORS AND TARGET.CREATED_AT = SOURCE.CREATED_AT)  ");
                    queryInsertResult2.Append("WHEN NOT MATCHED BY TARGET THEN  ");
                    queryInsertResult2.Append("  INSERT (IDGDA_COLLABORATORS, CARGO, CREATED_AT, IDGDA_SECTOR, HOME_BASED, SITE, PERIODO, MATRICULA_SUPERVISOR, NOME_SUPERVISOR, MATRICULA_COORDENADOR,  ");
                    queryInsertResult2.Append("		  NOME_COORDENADOR, MATRICULA_GERENTE_II, NOME_GERENTE_II, MATRICULA_GERENTE_I, NOME_GERENTE_I, MATRICULA_DIRETOR, NOME_DIRETOR, MATRICULA_CEO, NOME_CEO, ACTIVE)  ");
                    queryInsertResult2.Append("  VALUES (SOURCE.IDGDA_COLLABORATORS, SOURCE.CARGO, SOURCE.CREATED_AT, SOURCE.IDGDA_SECTOR, SOURCE.HOME_BASED, SOURCE.SITE, SOURCE.PERIODO, SOURCE.MATRICULA_SUPERVISOR, SOURCE.NOME_SUPERVISOR, SOURCE.MATRICULA_COORDENADOR,  ");
                    queryInsertResult2.Append("		  SOURCE.NOME_COORDENADOR, SOURCE.MATRICULA_GERENTE_II, SOURCE.NOME_GERENTE_II, SOURCE.MATRICULA_GERENTE_I, SOURCE.NOME_GERENTE_I, SOURCE.MATRICULA_DIRETOR, SOURCE.NOME_DIRETOR, SOURCE.MATRICULA_CEO, SOURCE.NOME_CEO, SOURCE.ACTIVE)  ");
                    queryInsertResult2.Append("WHEN MATCHED THEN  ");
                    queryInsertResult2.Append("  UPDATE SET  ");
                    queryInsertResult2.Append("  TARGET.CARGO = SOURCE.CARGO, ");
                    queryInsertResult2.Append("  TARGET.IDGDA_SECTOR = SOURCE.IDGDA_SECTOR, ");
                    queryInsertResult2.Append("  TARGET.HOME_BASED = SOURCE.HOME_BASED, ");
                    queryInsertResult2.Append("  TARGET.SITE = SOURCE.SITE, ");
                    queryInsertResult2.Append("  TARGET.PERIODO = SOURCE.PERIODO, ");
                    queryInsertResult2.Append("  TARGET.MATRICULA_SUPERVISOR = SOURCE.MATRICULA_SUPERVISOR, ");
                    queryInsertResult2.Append("  TARGET.NOME_SUPERVISOR = SOURCE.NOME_SUPERVISOR, ");
                    queryInsertResult2.Append("  TARGET.MATRICULA_COORDENADOR = SOURCE.MATRICULA_COORDENADOR, ");
                    queryInsertResult2.Append("  TARGET.NOME_COORDENADOR = SOURCE.NOME_COORDENADOR, ");
                    queryInsertResult2.Append("  TARGET.MATRICULA_GERENTE_II = SOURCE.MATRICULA_GERENTE_II, ");
                    queryInsertResult2.Append("  TARGET.NOME_GERENTE_II = SOURCE.NOME_GERENTE_II, ");
                    queryInsertResult2.Append("  TARGET.MATRICULA_GERENTE_I = SOURCE.MATRICULA_GERENTE_I, ");
                    queryInsertResult2.Append("  TARGET.NOME_GERENTE_I = SOURCE.NOME_GERENTE_I, ");
                    queryInsertResult2.Append("  TARGET.MATRICULA_DIRETOR = SOURCE.MATRICULA_DIRETOR, ");
                    queryInsertResult2.Append("  TARGET.NOME_DIRETOR = SOURCE.NOME_DIRETOR, ");
                    queryInsertResult2.Append("  TARGET.MATRICULA_CEO = SOURCE.MATRICULA_CEO, ");
                    queryInsertResult2.Append("  TARGET.NOME_CEO = SOURCE.NOME_CEO, ");
                    queryInsertResult2.Append("  TARGET.ACTIVE = SOURCE.ACTIVE; ");

                    SqlCommand createTableCommand2 = new SqlCommand(queryInsertResult2.ToString(), connection);
                    createTableCommand2.CommandTimeout = 0;
                    createTableCommand2.ExecuteNonQuery();
                }



                string dropTempTableQuery2 = $"DROP TABLE #TEMPAG";
                using (SqlCommand dropTempTableCommand2 = new SqlCommand(dropTempTableQuery2, connection))
                {
                    dropTempTableCommand2.ExecuteNonQuery();
                }

                //Log.insertLogTransaction(db, t_id.ToString(), "RESULT_CONSOLIDATED", "CONCLUDED", "");

            }
            catch (Exception ex)
            {
                //Log.insertLogTransaction(db, t_id.ToString(), "RESULT_CONSOLIDATED", "ERRO: " + ex.Message.ToString(), "");
            }
            connection.Close();
        }


        public bool monetization(int transactionID)
        {
            // Pegar todas as datas diferentes que existem nessa transaction
            List<string> datas = getDateTransaction(transactionID);

            datas.Clear();
            datas.Add("2023-10-01");
            datas.Add("2023-10-02");
            datas.Add("2023-10-03");
            datas.Add("2023-10-04");
            datas.Add("2023-10-05");
            datas.Add("2023-10-06");
            datas.Add("2023-10-07");
            datas.Add("2023-10-08");
            datas.Add("2023-10-09");
            datas.Add("2023-10-10");
            datas.Add("2023-10-11");
            datas.Add("2023-10-12");
            datas.Add("2023-10-13");
            datas.Add("2023-10-14");
            datas.Add("2023-10-15");
            datas.Add("2023-10-16");
            datas.Add("2023-10-17");
            datas.Add("2023-10-18");
            datas.Add("2023-10-19");
            datas.Add("2023-10-20");
            datas.Add("2023-10-21");
            datas.Add("2023-10-22");
            datas.Add("2023-10-23");
            datas.Add("2023-10-24");
            datas.Add("2023-10-25");
            datas.Add("2023-10-26");
            datas.Add("2023-10-27");
            datas.Add("2023-10-28");
            datas.Add("2023-10-29");
            datas.Add("2023-10-30");
            datas.Add("2023-10-31");
            datas.Add("2023-11-01");
            datas.Add("2023-11-02");
            datas.Add("2023-11-03");
            datas.Add("2023-11-04");
            datas.Add("2023-11-05");
            datas.Add("2023-11-06");
            //Varrer as datas retornadas
            foreach (string dt in datas)
            {

                List<MonetizationResultsModel> mrs = new List<MonetizationResultsModel>();

                using (SqlConnection connection = new SqlConnection(Database.Conn))
                {
                    connection.Open();

                    //Insere Log
                    StringBuilder stbLog = new StringBuilder();
                    stbLog.Append("INSERT INTO GDA_LOG_MONETIZATION_REPROCESSING (DATE_REFERENCE, UPDATED_AT) VALUES ");
                    stbLog.AppendFormat("('{0}', GETDATE())", dt);
                    SqlCommand insertLogMone = new SqlCommand(stbLog.ToString(), connection);
                    insertLogMone.ExecuteNonQuery();


                    try
                    {
                        StringBuilder stb = new StringBuilder();
                        stb.AppendFormat("DECLARE @DATAINICIAL AS DATE; SET @DATAINICIAL = '{0}'; ", dt);
                        stb.Append(" ");
                        stb.Append("SELECT R.*, ");
                        stb.Append("       HIS.GOAL, ");
                        stb.Append("       I.WEIGHT AS WEIGHT, ");
                        stb.Append("       HHR.LEVELWEIGHT AS HIERARCHYLEVEL, ");
                        stb.Append("       HIG1.MONETIZATION AS COIN1, ");
                        stb.Append("       HIG2.MONETIZATION AS COIN2, ");
                        stb.Append("       HIG3.MONETIZATION AS COIN3, ");
                        stb.Append("       HIG4.MONETIZATION AS COIN4, ");
                        stb.Append("       CL.IDGDA_SECTOR, ");
                        stb.Append("       MAX(I.TYPE) AS TYPE, ");
                        stb.Append("       HIG1.METRIC_MIN AS MIN1, ");
                        stb.Append("       HIG2.METRIC_MIN AS MIN2, ");
                        stb.Append("       HIG3.METRIC_MIN AS MIN3, ");
                        stb.Append("       HIG4.METRIC_MIN AS MIN4, ");
                        stb.Append("       CASE ");
                        stb.Append("           WHEN MAX(TBL.EXPRESSION) IS NULL THEN '(#FATOR1/#FATOR0)' ");
                        stb.Append("           ELSE MAX(TBL.EXPRESSION) ");
                        stb.Append("       END AS CONTA, ");
                        stb.Append("       CASE ");
                        stb.Append("           WHEN MAX(I.CALCULATION_TYPE) IS NULL THEN 'BIGGER_BETTER' ");
                        stb.Append("           ELSE MAX(I.CALCULATION_TYPE) ");
                        stb.Append("       END AS BETTER, ");
                        stb.Append("       COALESCE( ");
                        stb.Append("                  (SELECT TOP 1 BALANCE ");
                        stb.Append("                   FROM GDA_CHECKING_ACCOUNT (NOLOCK) ");
                        stb.Append("                   WHERE COLLABORATOR_ID = R.IDGDA_COLLABORATORS ");
                        stb.Append("                   ORDER BY CREATED_AT DESC), 0) AS SALDO, ");
                        stb.Append("       COALESCE( ");
                        stb.Append("                  (SELECT SUM(INPUT) ");
                        stb.Append("                   FROM GDA_CHECKING_ACCOUNT (NOLOCK) AS A ");
                        stb.Append("                   WHERE A.COLLABORATOR_ID = R.IDGDA_COLLABORATORS ");
                        stb.Append("                     AND A.RESULT_DATE = R.CREATED_AT ");
                        stb.Append("                     AND GDA_INDICATOR_IDGDA_INDICATOR = R.INDICADORID ");
                        stb.Append("                   GROUP BY A.COLLABORATOR_ID, A.RESULT_DATE, A.GDA_INDICATOR_IDGDA_INDICATOR), 0) AS COINS, ");
                        stb.Append("       R.TRANSACTIONID, ");
                        stb.Append("       MAX(CL.MATRICULA_SUPERVISOR) AS 'MATRICULA SUPERVISOR', ");
                        stb.Append("       MAX(CL.NOME_SUPERVISOR) AS 'NOME SUPERVISOR', ");
                        stb.Append("       MAX(CL.MATRICULA_COORDENADOR) AS 'MATRICULA COORDENADOR', ");
                        stb.Append("       MAX(CL.NOME_COORDENADOR) AS 'NOME COORDENADOR', ");
                        stb.Append("       MAX(CL.MATRICULA_GERENTE_II) AS 'MATRICULA GERENTE II', ");
                        stb.Append("       MAX(CL.NOME_GERENTE_II) AS 'NOME GERENTE II', ");
                        stb.Append("       MAX(CL.MATRICULA_GERENTE_I) AS 'MATRICULA GERENTE I', ");
                        stb.Append("       MAX(CL.NOME_GERENTE_I) AS 'NOME GERENTE I', ");
                        stb.Append("       MAX(CL.MATRICULA_DIRETOR) AS 'MATRICULA DIRETOR', ");
                        stb.Append("       MAX(CL.NOME_DIRETOR) AS 'NOME DIRETOR', ");
                        stb.Append("       MAX(CL.MATRICULA_CEO) AS 'MATRICULA CEO', ");
                        stb.Append("       MAX(CL.NOME_CEO) AS 'NOME CEO' ");
                        stb.Append("FROM GDA_RESULT (NOLOCK) AS R ");
                        stb.Append("LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CL ON CL.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
                        stb.Append("AND CL.CREATED_AT = R.CREATED_AT ");
                        stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
                        stb.Append("AND HIG1.INDICATOR_ID = R.INDICADORID ");
                        stb.Append("AND HIG1.SECTOR_ID = CL.IDGDA_SECTOR ");
                        stb.Append("AND HIG1.GROUPID = 1 ");
                        stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG2 ON HIG2.DELETED_AT IS NULL ");
                        stb.Append("AND HIG2.INDICATOR_ID = R.INDICADORID ");
                        stb.Append("AND HIG2.SECTOR_ID = CL.IDGDA_SECTOR ");
                        stb.Append("AND HIG2.GROUPID = 2 ");
                        stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG3 ON HIG3.DELETED_AT IS NULL ");
                        stb.Append("AND HIG3.INDICATOR_ID = R.INDICADORID ");
                        stb.Append("AND HIG3.SECTOR_ID = CL.IDGDA_SECTOR ");
                        stb.Append("AND HIG3.GROUPID = 3 ");
                        stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG4 ON HIG4.DELETED_AT IS NULL ");
                        stb.Append("AND HIG4.INDICATOR_ID = R.INDICADORID ");
                        stb.Append("AND HIG4.SECTOR_ID = CL.IDGDA_SECTOR ");
                        stb.Append("AND HIG4.GROUPID = 4 ");
                        stb.Append("LEFT JOIN ");
                        stb.Append("  (SELECT HME.INDICATORID, ");
                        stb.Append("          ME.EXPRESSION ");
                        stb.Append("   FROM GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME ");
                        stb.Append("   INNER JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON ME.ID = HME.MATHEMATICALEXPRESSIONID ");
                        stb.Append("   WHERE HME.DELETED_AT IS NULL) AS TBL ON TBL.INDICATORID = R.INDICADORID ");
                        stb.Append("LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS HHR ON HHR.idgda_COLLABORATORS = R.IDGDA_COLLABORATORS ");
                        stb.Append("AND HHR.DATE = @DATAINICIAL ");
                        stb.Append("INNER JOIN ");
                        stb.Append("  (SELECT GOAL, ");
                        stb.Append("          INDICATOR_ID, ");
                        stb.Append("          SECTOR_ID, ");
                        stb.Append("          ROW_NUMBER() OVER (PARTITION BY INDICATOR_ID, ");
                        stb.Append("                                          SECTOR_ID ");
                        stb.Append("                             ORDER BY CREATED_AT DESC) AS RN ");
                        stb.Append("   FROM GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) ");
                        stb.Append("   WHERE DELETED_AT IS NULL ) AS HIS ON HIS.INDICATOR_ID = R.INDICADORID ");
                        stb.Append("AND HIS.SECTOR_ID = CL.IDGDA_SECTOR ");
                        stb.Append("AND HIS.RN = 1 ");
                        stb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS I ON I.IDGDA_INDICATOR = R.INDICADORID ");
                        stb.Append("WHERE R.CREATED_AT = @DATAINICIAL ");
                        stb.Append("  AND HIG1.MONETIZATION > 0 ");
                        stb.Append("  AND R.DELETED_AT IS NULL AND CL.ACTIVE = 'true' ");
                        stb.Append("GROUP BY IDGDA_RESULT, ");
                        stb.Append("         INDICADORID, ");
                        stb.Append("         R.TRANSACTIONID, ");
                        stb.Append("         RESULT, ");
                        stb.Append("         R.CREATED_AT, ");
                        stb.Append("         R.IDGDA_COLLABORATORS, ");
                        stb.Append("         FACTORS, ");
                        stb.Append("         R.DELETED_AT, ");
                        stb.Append("         CL.IDGDA_SECTOR, ");
                        stb.Append("         HIG1.METRIC_MIN, ");
                        stb.Append("         HIG2.METRIC_MIN, ");
                        stb.Append("         HIG3.METRIC_MIN, ");
                        stb.Append("         HIG4.METRIC_MIN, ");
                        stb.Append("         HIS.GOAL, ");
                        stb.Append("         I.WEIGHT, ");
                        stb.Append("         HHR.LEVELWEIGHT, ");
                        stb.Append("         HIG1.MONETIZATION, ");
                        stb.Append("         HIG2.MONETIZATION, ");
                        stb.Append("         HIG3.MONETIZATION, ");
                        stb.Append("         HIG4.MONETIZATION ");
                        stb.Append("UNION ALL ");
                        stb.Append("SELECT R.*, ");
                        stb.Append("       HIS.GOAL, ");
                        stb.Append("       I.WEIGHT AS WEIGHT, ");
                        stb.Append("       HHR.LEVELWEIGHT AS HIERARCHYLEVEL, ");
                        stb.Append("       HIG1.MONETIZATION AS COIN1, ");
                        stb.Append("       HIG2.MONETIZATION AS COIN2, ");
                        stb.Append("       HIG3.MONETIZATION AS COIN3, ");
                        stb.Append("       HIG4.MONETIZATION AS COIN4, ");
                        stb.Append("       CL2.IDGDA_SECTOR, ");
                        stb.Append("       MAX(I.TYPE) AS TYPE, ");
                        stb.Append("       HIG1.METRIC_MIN AS MIN1, ");
                        stb.Append("       HIG2.METRIC_MIN AS MIN2, ");
                        stb.Append("       HIG3.METRIC_MIN AS MIN3, ");
                        stb.Append("       HIG4.METRIC_MIN AS MIN4, ");
                        stb.Append("       CASE ");
                        stb.Append("           WHEN MAX(TBL.EXPRESSION) IS NULL THEN '(#FATOR1/#FATOR0)' ");
                        stb.Append("           ELSE MAX(TBL.EXPRESSION) ");
                        stb.Append("       END AS CONTA, ");
                        stb.Append("       CASE ");
                        stb.Append("           WHEN MAX(I.CALCULATION_TYPE) IS NULL THEN 'BIGGER_BETTER' ");
                        stb.Append("           ELSE MAX(I.CALCULATION_TYPE) ");
                        stb.Append("       END AS BETTER, ");
                        stb.Append("       COALESCE( ");
                        stb.Append("                  (SELECT TOP 1 BALANCE ");
                        stb.Append("                   FROM GDA_CHECKING_ACCOUNT (NOLOCK) ");
                        stb.Append("                   WHERE COLLABORATOR_ID = R.IDGDA_COLLABORATORS ");
                        stb.Append("                   ORDER BY CREATED_AT DESC), 0) AS SALDO, ");
                        stb.Append("       COALESCE( ");
                        stb.Append("                  (SELECT SUM(INPUT) ");
                        stb.Append("                   FROM GDA_CHECKING_ACCOUNT (NOLOCK) AS A ");
                        stb.Append("                   WHERE A.COLLABORATOR_ID = R.IDGDA_COLLABORATORS ");
                        stb.Append("                     AND A.RESULT_DATE = R.CREATED_AT ");
                        stb.Append("                     AND GDA_INDICATOR_IDGDA_INDICATOR = R.INDICADORID ");
                        stb.Append("                   GROUP BY A.COLLABORATOR_ID, A.RESULT_DATE, A.GDA_INDICATOR_IDGDA_INDICATOR), 0) AS COINS, ");
                        stb.Append("       R.TRANSACTIONID, ");
                        stb.Append("       MAX(CL2.MATRICULA_SUPERVISOR) AS 'MATRICULA SUPERVISOR', ");
                        stb.Append("       MAX(CL2.NOME_SUPERVISOR) AS 'NOME SUPERVISOR', ");
                        stb.Append("       MAX(CL2.MATRICULA_COORDENADOR) AS 'MATRICULA COORDENADOR', ");
                        stb.Append("       MAX(CL2.NOME_COORDENADOR) AS 'NOME COORDENADOR', ");
                        stb.Append("       MAX(CL2.MATRICULA_GERENTE_II) AS 'MATRICULA GERENTE II', ");
                        stb.Append("       MAX(CL2.NOME_GERENTE_II) AS 'NOME GERENTE II', ");
                        stb.Append("       MAX(CL2.MATRICULA_GERENTE_I) AS 'MATRICULA GERENTE I', ");
                        stb.Append("       MAX(CL2.NOME_GERENTE_I) AS 'NOME GERENTE I', ");
                        stb.Append("       MAX(CL2.MATRICULA_DIRETOR) AS 'MATRICULA DIRETOR', ");
                        stb.Append("       MAX(CL2.NOME_DIRETOR) AS 'NOME DIRETOR', ");
                        stb.Append("       MAX(CL2.MATRICULA_CEO) AS 'MATRICULA CEO', ");
                        stb.Append("       MAX(CL2.NOME_CEO) AS 'NOME CEO' ");
                        stb.Append("FROM GDA_RESULT (NOLOCK) AS R ");
                        stb.Append("LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CL ON CL.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
                        stb.Append("AND CL.CREATED_AT = R.CREATED_AT ");
                        stb.Append("LEFT JOIN GDA_COLLABORATORS_LAST_DETAILS (NOLOCK) AS CL2 ON CL2.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
                        stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
                        stb.Append("AND HIG1.INDICATOR_ID = R.INDICADORID ");
                        stb.Append("AND HIG1.SECTOR_ID = CL2.IDGDA_SECTOR ");
                        stb.Append("AND HIG1.GROUPID = 1 ");
                        stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG2 ON HIG2.DELETED_AT IS NULL ");
                        stb.Append("AND HIG2.INDICATOR_ID = R.INDICADORID ");
                        stb.Append("AND HIG2.SECTOR_ID = CL2.IDGDA_SECTOR ");
                        stb.Append("AND HIG2.GROUPID = 2 ");
                        stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG3 ON HIG3.DELETED_AT IS NULL ");
                        stb.Append("AND HIG3.INDICATOR_ID = R.INDICADORID ");
                        stb.Append("AND HIG3.SECTOR_ID = CL2.IDGDA_SECTOR ");
                        stb.Append("AND HIG3.GROUPID = 3 ");
                        stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG4 ON HIG4.DELETED_AT IS NULL ");
                        stb.Append("AND HIG4.INDICATOR_ID = R.INDICADORID ");
                        stb.Append("AND HIG4.SECTOR_ID = CL2.IDGDA_SECTOR ");
                        stb.Append("AND HIG4.GROUPID = 4 ");
                        stb.Append("LEFT JOIN ");
                        stb.Append("  (SELECT HME.INDICATORID, ");
                        stb.Append("          ME.EXPRESSION ");
                        stb.Append("   FROM GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME ");
                        stb.Append("   INNER JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON ME.ID = HME.MATHEMATICALEXPRESSIONID ");
                        stb.Append("   WHERE HME.DELETED_AT IS NULL) AS TBL ON TBL.INDICATORID = R.INDICADORID ");
                        stb.Append("LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS HHR ON HHR.idgda_COLLABORATORS = R.IDGDA_COLLABORATORS ");
                        stb.Append("AND HHR.DATE = @DATAINICIAL ");
                        stb.Append("INNER JOIN ");
                        stb.Append("  (SELECT GOAL, ");
                        stb.Append("          INDICATOR_ID, ");
                        stb.Append("          SECTOR_ID, ");
                        stb.Append("          ROW_NUMBER() OVER (PARTITION BY INDICATOR_ID, ");
                        stb.Append("                                          SECTOR_ID ");
                        stb.Append("                             ORDER BY CREATED_AT DESC) AS RN ");
                        stb.Append("   FROM GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) ");
                        stb.Append("   WHERE DELETED_AT IS NULL ) AS HIS ON HIS.INDICATOR_ID = R.INDICADORID ");
                        stb.Append("AND HIS.SECTOR_ID = CL2.IDGDA_SECTOR ");
                        stb.Append("AND HIS.RN = 1 ");
                        stb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS I ON I.IDGDA_INDICATOR = R.INDICADORID ");
                        stb.Append("WHERE R.CREATED_AT = @DATAINICIAL ");
                        stb.Append("  AND HIG1.MONETIZATION > 0 ");
                        stb.Append("  AND R.DELETED_AT IS NULL ");
                        stb.Append("  AND (CL.IDGDA_SECTOR IS NULL OR CL.ACTIVE IS NULL) AND CL2.ACTIVE = 'true' ");
                        stb.Append("GROUP BY IDGDA_RESULT, ");
                        stb.Append("         INDICADORID, ");
                        stb.Append("         R.TRANSACTIONID, ");
                        stb.Append("         RESULT, ");
                        stb.Append("         R.CREATED_AT, ");
                        stb.Append("         R.IDGDA_COLLABORATORS, ");
                        stb.Append("         FACTORS, ");
                        stb.Append("         R.DELETED_AT, ");
                        stb.Append("         CL2.IDGDA_SECTOR, ");
                        stb.Append("         HIG1.METRIC_MIN, ");
                        stb.Append("         HIG2.METRIC_MIN, ");
                        stb.Append("         HIG3.METRIC_MIN, ");
                        stb.Append("         HIG4.METRIC_MIN, ");
                        stb.Append("         HIS.GOAL, ");
                        stb.Append("         I.WEIGHT, ");
                        stb.Append("         HHR.LEVELWEIGHT, ");
                        stb.Append("         HIG1.MONETIZATION, ");
                        stb.Append("         HIG2.MONETIZATION, ");
                        stb.Append("         HIG3.MONETIZATION, ");
                        stb.Append("         HIG4.MONETIZATION ");
                        
                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            command.CommandTimeout = 120;
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    try
                                    {
                                        MonetizationResultsModel mr = new MonetizationResultsModel();

                                        mr.idCollaborator = reader["IDGDA_COLLABORATORS"].ToString();
                                        mr.idIndicator = reader["INDICADORID"].ToString();
                                        mr.idResult = reader["IDGDA_RESULT"].ToString();
                                        mr.idSector = reader["IDGDA_SECTOR"].ToString();
                                        mr.idCheckingAccount = 0;
                                        mr.indicatorWeight = reader["WEIGHT"].ToString();
                                        mr.hierarchyLevel = reader["HIERARCHYLEVEL"].ToString();
                                        mr.meta = double.Parse(reader["GOAL"].ToString());
                                        mr.fatores = reader["FACTORS"].ToString();
                                        mr.fator0 = Convert.ToDouble(reader["FACTORS"].ToString().Split(";")[0]);
                                        mr.fator1 = Convert.ToDouble(reader["FACTORS"].ToString().Split(";")[1]);
                                        mr.conta = reader["CONTA"].ToString();
                                        mr.melhor = reader["BETTER"].ToString();
                                        mr.G1 = double.Parse(reader["MIN1"].ToString());
                                        mr.G2 = double.Parse(reader["MIN2"].ToString());
                                        mr.G3 = double.Parse(reader["MIN3"].ToString());
                                        mr.G4 = double.Parse(reader["MIN4"].ToString());
                                        mr.C1 = double.Parse(reader["COIN1"].ToString());
                                        mr.C2 = double.Parse(reader["COIN2"].ToString());
                                        mr.C3 = double.Parse(reader["COIN3"].ToString());
                                        mr.C4 = double.Parse(reader["COIN4"].ToString());
                                        mr.saldo = double.Parse(reader["SALDO"].ToString());
                                        mr.typeIndicator = reader["TYPE"].ToString();

                                        mr.transactionId = Convert.ToInt32(reader["TRANSACTIONID"].ToString());
                                        mr.matriculaSupervisor = reader["MATRICULA SUPERVISOR"].ToString();
                                        mr.nomeSupervisor = reader["NOME SUPERVISOR"].ToString();
                                        mr.matriculaCoordenador = reader["MATRICULA COORDENADOR"].ToString();
                                        mr.nomeCoordenador = reader["NOME COORDENADOR"].ToString();
                                        mr.matriculaGerenteii = reader["MATRICULA GERENTE II"].ToString();
                                        mr.nomeGerenteii = reader["NOME GERENTE II"].ToString();
                                        mr.matriculaGerentei = reader["MATRICULA GERENTE I"].ToString();
                                        mr.nomeGerentei = reader["NOME GERENTE I"].ToString();
                                        mr.matriculaDiretor = reader["MATRICULA DIRETOR"].ToString();
                                        mr.nomeDiretor = reader["NOME DIRETOR"].ToString();
                                        mr.matriculaCeo = reader["MATRICULA CEO"].ToString();
                                        mr.nomeCeo = reader["NOME CEO"].ToString();
                                        mr.coins = Convert.ToInt32(reader["COINS"].ToString());

                                        mrs.Add(mr);
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                }
                            }
                        }

                        var count = 0;

                        //List<MonetizationResultsModel> mrsAgentesMonetizados = mrs.Where(whr => whr.transactionId == transactionID).ToList();
                        List<MonetizationResultsModel> mrsAgentesMonetizados = mrs;

                        //Monetização Agentes - Monetiza todos os agentes primeiro.. Depois monetiza a hierarquia
                        foreach (MonetizationResultsModel mr in mrsAgentesMonetizados)
                        {
                            //if (mr.idCollaborator != "789671" || mr.idIndicator != "2")
                            //{
                            //    continue;
                            //}

                            try
                            {



                                count += 1;

                                double coins = doMonetizationAgent(transactionID, dt, mr);

                                //if (coins > 0)
                                //{
                                //    var parou = true;
                                //}

                                var foundItem = mrs.Find(l => l.idResult == mr.idResult);
                                if (foundItem != null)
                                {
                                    var index = mrs.IndexOf(foundItem);
                                    mrs[index].coins += coins;
                                }
                            }
                            catch (Exception ex)
                            {

  
                            } 
                        }

                        List<MonetizationResultsModel> superAtual = getListHierarchy(mrsAgentesMonetizados, "SUPERVISOR");
                        List<MonetizationResultsModel> mrsGroup = getListHierarchy(mrs, "SUPERVISOR");
                        List<MonetizationResultsModel> filteredMrsGroup = mrsGroup
                        .Where(m => superAtual.Any(s => m.matriculaSupervisor.Contains(s.matriculaSupervisor) && m.idIndicator.Contains(s.idIndicator))).Where(n => n.coins > 0).Where(t => t.matriculaSupervisor != "-").Where(t => t.matriculaSupervisor != "0")
                        .ToList();

                        foreach (MonetizationResultsModel mr in filteredMrsGroup)
                        {
                            try
                            {
                                doMonetizationHierarchyNew(dt, mr.matriculaSupervisor, mr);
                            }
                            catch (Exception)
                            {

                            }
                          
                        }

                        superAtual = getListHierarchy(mrsAgentesMonetizados, "COORDENADOR");
                        mrsGroup = getListHierarchy(mrs, "COORDENADOR");
                        filteredMrsGroup = mrsGroup
                        .Where(m => superAtual.Any(s => m.matriculaCoordenador.Contains(s.matriculaCoordenador) && m.idIndicator.Contains(s.idIndicator))).Where(n => n.coins > 0).Where(t => t.matriculaCoordenador != "-").Where(t => t.matriculaCoordenador != "0")
                        .ToList();
                        foreach (MonetizationResultsModel mr in filteredMrsGroup)
                        {
                            try
                            {
                                doMonetizationHierarchyNew(dt, mr.matriculaCoordenador, mr);
                            }
                            catch (Exception)
                            {
                            }
                            
                        }

                        superAtual = getListHierarchy(mrsAgentesMonetizados, "GERENTE II");
                        mrsGroup = getListHierarchy(mrs, "GERENTE II");
                        filteredMrsGroup = mrsGroup
                        .Where(m => superAtual.Any(s => m.matriculaGerenteii.Contains(s.matriculaGerenteii) && m.idIndicator.Contains(s.idIndicator))).Where(n => n.coins > 0).Where(t => t.matriculaGerenteii != "-").Where(t => t.matriculaGerenteii != "0")
                        .ToList();
                        foreach (MonetizationResultsModel mr in filteredMrsGroup)
                        {
                            //if (mr.matriculaGerenteii != "600111" || mr.idIndicator != "727")
                            //{
                            //    continue;
                            //}
                            try
                            {
                                doMonetizationHierarchyNew(dt, mr.matriculaGerenteii, mr);
                            }
                            catch (Exception)
                            {
                            }
                            
                        }

                        superAtual = getListHierarchy(mrsAgentesMonetizados, "GERENTE I");
                        mrsGroup = getListHierarchy(mrs, "GERENTE I");
                        filteredMrsGroup = mrsGroup
                       .Where(m => superAtual.Any(s => m.matriculaGerentei.Contains(s.matriculaGerentei) && m.idIndicator.Contains(s.idIndicator))).Where(n => n.coins > 0).Where(t => t.matriculaGerentei != "-").Where(t => t.matriculaGerentei != "0")
                       .ToList();
                        foreach (MonetizationResultsModel mr in filteredMrsGroup)
                        {
                            try
                            {
                                doMonetizationHierarchyNew(dt, mr.matriculaGerentei, mr);
                            }
                            catch (Exception)
                            {
                            }
                            
                        }

                        superAtual = getListHierarchy(mrsAgentesMonetizados, "DIRETOR");
                        mrsGroup = getListHierarchy(mrs, "DIRETOR");
                        filteredMrsGroup = mrsGroup
                        .Where(m => superAtual.Any(s => m.matriculaDiretor.Contains(s.matriculaDiretor) && m.idIndicator.Contains(s.idIndicator))).Where(n => n.coins > 0).Where(t => t.matriculaDiretor != "-").Where(t => t.matriculaDiretor != "0")
                        .ToList();
                        foreach (MonetizationResultsModel mr in filteredMrsGroup)
                        {
                            try
                            {
                                doMonetizationHierarchyNew(dt, mr.matriculaDiretor, mr);
                            }
                            catch (Exception)
                            {
                            }
                        }

                        superAtual = getListHierarchy(mrsAgentesMonetizados, "CEO");
                        mrsGroup = getListHierarchy(mrs, "CEO");
                        filteredMrsGroup = mrsGroup
                        .Where(m => superAtual.Any(s => m.matriculaCeo.Contains(s.matriculaCeo) && m.idIndicator.Contains(s.idIndicator))).Where(n => n.coins > 0).Where(t => t.matriculaCeo != "-").Where(t => t.matriculaCeo != "0")
                        .ToList();
                        foreach (MonetizationResultsModel mr in filteredMrsGroup)
                        {
                            try
                            {
                                doMonetizationHierarchyNew(dt, mr.matriculaCeo, mr);
                            }
                            catch (Exception)
                            {
                            }
                        }


                        //doBasketMonetization(dt);

                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                    connection.Close();
                }

            }




            //




            return true;
        }

        public List<string> getDateTransaction(int transactionID)
        {


            List<string> datas = new List<string>();

            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                connection.Open();
                try
                {
                    StringBuilder stb = new StringBuilder();
                    stb.Append("SELECT DISTINCT(CONVERT(varchar, CREATED_AT, 23)) AS CREATED_AT FROM GDA_RESULT (NOLOCK) ");
                    stb.AppendFormat("WHERE TRANSACTIONID = {0}; ", transactionID);

                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string resultado = reader["CREATED_AT"].ToString(); // Substitua "Nome" pelo nome da coluna que você deseja recuperar.
                                datas.Add(resultado);
                            }
                        }
                    }
                }
                catch (Exception)
                {

                }
                connection.Close();
            }

            return datas;
        }

        public double doMonetizationAgent(int transactionID, string dateM, MonetizationResultsModel mr)
        {
            double retorno = 0;
            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                connection.Open();
                try
                {
                    mr.conta = mr.conta.Replace("#fator0", mr.fator0.ToString()).Replace("#fator1", mr.fator1.ToString());
                    //Realiza a conta de resultado

                    if (mr.fator0.ToString() == "0" && mr.fator1.ToString() == "0")
                    {
                        return 0;
                    }

                    DataTable dt = new DataTable();
                    double resultado = 0;
                    try
                    {
                        var result = dt.Compute(mr.conta, "").ToString();
                        resultado = double.Parse(result);
                        if (resultado == double.PositiveInfinity)
                        {
                            resultado = 0;
                        }
                        if (double.IsNaN(resultado))
                        {
                            resultado = 0;
                        }
                    }
                    catch (Exception)
                    {

                    }


                    double resultadoD = resultado;

                    if (mr.typeIndicator == null)
                    {
                        resultadoD = resultadoD * 100;
                    }
                    else if (mr.typeIndicator == "PERCENT")
                    {
                        resultadoD = resultadoD * 100;
                    }

                    double monetizacaoDia = 0;

                    double atingimentoMeta = 0;
                    //Verifica se é melhor ou menor melhor
                    if (mr.melhor == "BIGGER_BETTER")
                    {
                        if (mr.meta == 0)
                        {
                            atingimentoMeta = 0;
                        }
                        else
                        {
                            atingimentoMeta = resultadoD / mr.meta;
                        }

                    }
                    else
                    {
                        // No caso de menor melhor, quando o denominador é 0 não esta sendo possivel fazer a conta de divisão por 0.
                        // E como é menor melhor, logo 0 é um resultado de 100% de atingimento.
                        if (resultadoD == 0)
                        {
                            atingimentoMeta = 10;
                        }
                        else
                        {
                            if (mr.idIndicator == "2")
                            {
                                atingimentoMeta = 100 - resultadoD;
                                //atingimentoMeta = (factor.goal / resultadoD);
                                atingimentoMeta = atingimentoMeta / 100;
                            }
                            else
                            {
                                atingimentoMeta = (mr.meta / resultadoD);
                            }
                        }


                    }

                    atingimentoMeta = atingimentoMeta * 100;

                    double moedas = 0;
                    //Verifica a qual grupo pertence
                    if (atingimentoMeta >= mr.G1)
                    {
                        moedas = mr.C1;
                    }
                    else if (atingimentoMeta >= mr.G2)
                    {
                        moedas = mr.C2;
                    }
                    else if (atingimentoMeta >= mr.G3)
                    {
                        moedas = mr.C3;
                    }
                    else if (atingimentoMeta >= mr.G4)
                    {
                        moedas = mr.C4;
                    }

                    double inputMoedas = 0;
                    double outptMoedas = 0;

                    if (moedas > 0)
                    {
                        inputMoedas = moedas;
                        mr.saldo = mr.saldo + moedas;
                    }
                    else if (moedas < 0)
                    {
                        outptMoedas = moedas;
                        mr.saldo = mr.saldo - moedas;
                    }

                    //Verifica se ja foi monetizado
                    StringBuilder stb = new StringBuilder();

                    stb.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}'; ", mr.idCollaborator);
                    stb.AppendFormat("DECLARE @DATEENV DATE; SET @DATEENV = '{0}'; ", dateM);
                    stb.AppendFormat("DECLARE @INDICADORID VARCHAR(MAX); SET @INDICADORID = '{0}'; ", mr.idIndicator);
                    stb.Append("SELECT CASE WHEN SUM(INPUT) IS NULL THEN 0 ELSE SUM(INPUT) END AS SOMA ");
                    stb.Append("FROM GDA_CHECKING_ACCOUNT (NOLOCK) CA ");
                    //stb.Append("INNER JOIN GDA_HISTORY_COLLABORATOR_SECTOR (NOLOCK) AS S ON CONVERT(DATE, S.CREATED_AT, 120) = CONVERT(DATE, CA.RESULT_DATE, 120)  ");
                    //stb.Append("AND S.IDGDA_COLLABORATORS = CA.COLLABORATOR_ID ");
                    stb.Append("WHERE CA.COLLABORATOR_ID = @INPUTID ");
                    stb.Append("AND CA.RESULT_DATE = @DATEENV ");
                    stb.Append("AND GDA_INDICATOR_IDGDA_INDICATOR = @INDICADORID ");

                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.CommandTimeout = 0;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                monetizacaoDia = double.Parse(reader["SOMA"].ToString());
                            }
                        }
                    }

                    //Monetizar apenas o que ainda não foi monetizado. E apenas a diferença para cima
                    if (inputMoedas > monetizacaoDia)
                    {
                        //Verifica ultimo saldo
                        //
                        StringBuilder stbBalance = new StringBuilder();
                        double balanceEnc = 0;
                        stbBalance.Append("SELECT TOP 1 BALANCE FROM GDA_CHECKING_ACCOUNT (NOLOCK) ");
                        stbBalance.AppendFormat("WHERE COLLABORATOR_ID = '{0}' ", mr.idCollaborator);
                        stbBalance.Append("ORDER BY CREATED_AT DESC ");

                        using (SqlCommand command = new SqlCommand(stbBalance.ToString(), connection))
                        {
                            command.CommandTimeout = 0;
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    balanceEnc = double.Parse(reader["BALANCE"].ToString());
                                }
                            }
                        }

                        balanceEnc = balanceEnc + inputMoedas;

                        double conta = inputMoedas - monetizacaoDia;
                        inputMoedas = conta;

                        retorno = inputMoedas;

                        //Inserir moedas                    
                        StringBuilder stbCheckingAccount = new StringBuilder();
                        stbCheckingAccount.Append("INSERT INTO GDA_CHECKING_ACCOUNT (INPUT, OUTPUT, BALANCE, COLLABORATOR_ID, CREATED_AT, ");
                        stbCheckingAccount.Append(" GDA_INDICATOR_IDGDA_INDICATOR, GDA_ORDER_IDGDA_ORDER, CREATEDBYCOLLABORATORID, OBSERVATION, ");
                        stbCheckingAccount.Append("REASON, IDGDA_RESULT, RESULT_DATE, WEIGHT, IDGDA_SECTOR) OUTPUT INSERTED.ID VALUES( ");
                        stbCheckingAccount.AppendFormat("'{0}',", inputMoedas); //INPUT
                        stbCheckingAccount.AppendFormat("'{0}',", outptMoedas); //OUTPUT
                        stbCheckingAccount.AppendFormat("'{0}',", balanceEnc); //BALANCE
                        stbCheckingAccount.AppendFormat("'{0}',", mr.idCollaborator); //COLLABORATOR_ID
                        stbCheckingAccount.AppendFormat("{0},", "GETDATE()"); //CREATED_AT
                        stbCheckingAccount.AppendFormat("'{0}',", mr.idIndicator); //GDA_INDICATOR_IDGDA_INDICATOR
                        stbCheckingAccount.AppendFormat("{0},", "NULL"); //GDA_ORDER_IDGDA_ORDER
                        stbCheckingAccount.AppendFormat("{0},", "NULL"); //CREATEDBYCOLLABORATORID
                        stbCheckingAccount.AppendFormat("{0},", "NULL"); //OBSERVATION
                        stbCheckingAccount.AppendFormat("{0},", "NULL"); //REASON
                        stbCheckingAccount.AppendFormat("'{0}',", mr.idResult); //IRGDA_RESULT
                        stbCheckingAccount.AppendFormat("'{0}',", dateM); //RESULT_DATE
                        stbCheckingAccount.AppendFormat("'{0}',", mr.indicatorWeight); //WEIGHT
                        stbCheckingAccount.AppendFormat("'{0}'", mr.idSector); //IDGDA_SECTOR
                        stbCheckingAccount.Append("); ");
                        using (SqlCommand cmd = new SqlCommand(stbCheckingAccount.ToString(), connection))
                        {
                            mr.idCheckingAccount = (int)cmd.ExecuteScalar();
                        }


                        if (mr.hierarchyLevel == "")
                        {
                            mr.hierarchyLevel = "1";
                        }

                        //Inserir moedas historico
                        StringBuilder stbConsolChecking = new StringBuilder();
                        stbConsolChecking.Append("INSERT INTO GDA_CONSOLIDATE_CHECKING_ACCOUNT (MONETIZATION, ");
                        stbConsolChecking.Append("GDA_CHECKING_ACCOUNT_IDGDA_CHECKING_ACCOUNT, GDA_HIERARCHY_IDGDA_HIERARCHY,  ");
                        stbConsolChecking.Append("GDA_INDICATOR_IDGDA_INDICATOR, GDA_SECTOR_IDGDA_SECTOR, CREATED_AT, DELETED_AT, ");
                        stbConsolChecking.Append("IDGDA_RESULT, RESULT_DATE, WEIGHT) VALUES (");
                        stbConsolChecking.AppendFormat("'{0}', ", moedas.ToString()); //MONETIZATION
                        stbConsolChecking.AppendFormat("'{0}', ", mr.idCheckingAccount); //GDA_CHECKING_ACCOUNT_IDGDA_CHECKING_ACCOUNT
                        stbConsolChecking.AppendFormat("'{0}', ", mr.hierarchyLevel); //GDA_HIERARCHY_IDGDA_HIERARCHY
                        stbConsolChecking.AppendFormat("'{0}', ", mr.idIndicator); //GDA_INDICATOR_IDGDA_INDICATOR
                        stbConsolChecking.AppendFormat("'{0}', ", mr.idSector); //GDA_SECTOR_IDGDA_SECTOR
                        stbConsolChecking.AppendFormat("{0}, ", "GETDATE()"); //CREATED_AT
                        stbConsolChecking.AppendFormat("{0}, ", "NULL"); //DELETED_AT
                        stbConsolChecking.AppendFormat("'{0}', ", mr.idResult); //IDGDA_RESULT
                        stbConsolChecking.AppendFormat("'{0}', ", dateM); //RESULT_DATE
                        stbConsolChecking.AppendFormat("'{0}' ", mr.indicatorWeight); //WEIGHT
                        stbConsolChecking.Append(");");
                        SqlCommand insertConsolCheckingCommand = new SqlCommand(stbConsolChecking.ToString(), connection);
                        insertConsolCheckingCommand.ExecuteNonQuery();
                    }

                }
                catch (Exception ex)
                {
                    return 0;
                }
                connection.Close();
            }
            return retorno;
        }

        public List<MonetizationResultsModel> getListHierarchy(List<MonetizationResultsModel> mrs, string Hierarchy)
        {
            List<MonetizationResultsModel> mrsReturn = new List<MonetizationResultsModel>();

            mrs = mrs.Where(x => x.fatores != "0.000000;0.000000").ToList();

            if (Hierarchy == "SUPERVISOR")
            {
                mrsReturn = mrs.GroupBy(item => new { item.idIndicator, item.matriculaSupervisor }).Select(grupo => new MonetizationResultsModel
                {
                    idCollaborator = grupo.First().idCollaborator,
                    idIndicator = grupo.Key.idIndicator,
                    idResult = grupo.First().idResult,
                    idSector = grupo.First().idSector,
                    idCheckingAccount = 0,
                    indicatorWeight = grupo.First().indicatorWeight,
                    hierarchyLevel = "2",
                    meta = grupo.First().meta,
                    fatores = grupo.First().fatores,
                    fator0 = 0,
                    fator1 = 0,
                    conta = grupo.First().idCollaborator,
                    melhor = grupo.First().idCollaborator,
                    G1 = grupo.First().G1,
                    G2 = grupo.First().G2,
                    G3 = grupo.First().G3,
                    G4 = grupo.First().G4,
                    C1 = grupo.Max(it => it.C1),
                    C2 = grupo.Max(it => it.C2),
                    C3 = grupo.Max(it => it.C3),
                    C4 = grupo.Max(it => it.C4),
                    saldo = grupo.First().saldo,
                    typeIndicator = grupo.First().typeIndicator,
                    parentId = grupo.First().parentId,
                    levelName = grupo.First().levelName,
                    level = grupo.First().level,
                    transactionId = grupo.First().transactionId,
                    coins = grupo.Sum(it => it.coins),
                    quantidade = grupo.Count(),
                    matriculaSupervisor = grupo.Key.matriculaSupervisor,
                }).ToList();
            }
            else if (Hierarchy == "COORDENADOR")
            {
                mrsReturn = mrs.GroupBy(item => new { item.idIndicator, item.matriculaCoordenador }).Select(grupo => new MonetizationResultsModel
                {
                    idCollaborator = grupo.First().idCollaborator,
                    idIndicator = grupo.Key.idIndicator,
                    idResult = grupo.First().idResult,
                    idSector = grupo.First().idSector,
                    idCheckingAccount = 0,
                    indicatorWeight = grupo.First().indicatorWeight,
                    hierarchyLevel = "3",
                    meta = grupo.First().meta,
                    fatores = grupo.First().fatores,
                    fator0 = grupo.Sum(it => it.fator0),
                    fator1 = grupo.Sum(it => it.fator0),
                    conta = grupo.First().idCollaborator,
                    melhor = grupo.First().idCollaborator,
                    G1 = grupo.First().G1,
                    G2 = grupo.First().G2,
                    G3 = grupo.First().G3,
                    G4 = grupo.First().G4,
                    C1 = grupo.Max(it => it.C1),
                    C2 = grupo.Max(it => it.C2),
                    C3 = grupo.Max(it => it.C3),
                    C4 = grupo.Max(it => it.C4),
                    saldo = grupo.First().saldo,
                    typeIndicator = grupo.First().typeIndicator,
                    parentId = grupo.First().parentId,
                    levelName = grupo.First().levelName,
                    level = grupo.First().level,
                    transactionId = grupo.First().transactionId,
                    coins = grupo.Sum(it => it.coins),
                    quantidade = grupo.Count(),
                    matriculaCoordenador = grupo.Key.matriculaCoordenador,
                }).ToList();
            }
            else if (Hierarchy == "GERENTE II")
            {
                mrsReturn = mrs.GroupBy(item => new { item.idIndicator, item.matriculaGerenteii }).Select(grupo => new MonetizationResultsModel
                {
                    idCollaborator = grupo.First().idCollaborator,
                    idIndicator = grupo.Key.idIndicator,
                    idResult = grupo.First().idResult,
                    idSector = grupo.First().idSector,
                    idCheckingAccount = 0,
                    indicatorWeight = grupo.First().indicatorWeight,
                    hierarchyLevel = "4",
                    meta = grupo.First().meta,
                    fatores = grupo.First().fatores,
                    fator0 = grupo.Sum(it => it.fator0),
                    fator1 = grupo.Sum(it => it.fator0),
                    conta = grupo.First().idCollaborator,
                    melhor = grupo.First().idCollaborator,
                    G1 = grupo.First().G1,
                    G2 = grupo.First().G2,
                    G3 = grupo.First().G3,
                    G4 = grupo.First().G4,
                    C1 = grupo.Max(it => it.C1),
                    C2 = grupo.Max(it => it.C2),
                    C3 = grupo.Max(it => it.C3),
                    C4 = grupo.Max(it => it.C4),
                    saldo = grupo.First().saldo,
                    typeIndicator = grupo.First().typeIndicator,
                    parentId = grupo.First().parentId,
                    levelName = grupo.First().levelName,
                    level = grupo.First().level,
                    transactionId = grupo.First().transactionId,
                    coins = grupo.Sum(it => it.coins),
                    quantidade = grupo.Count(),
                    matriculaGerenteii = grupo.Key.matriculaGerenteii,
                }).ToList();
            }
            else if (Hierarchy == "GERENTE I")
            {
                mrsReturn = mrs.GroupBy(item => new { item.idIndicator, item.matriculaGerentei }).Select(grupo => new MonetizationResultsModel
                {
                    idCollaborator = grupo.First().idCollaborator,
                    idIndicator = grupo.Key.idIndicator,
                    idResult = grupo.First().idResult,
                    idSector = grupo.First().idSector,
                    idCheckingAccount = 0,
                    indicatorWeight = grupo.First().indicatorWeight,
                    hierarchyLevel = "5",
                    meta = grupo.First().meta,
                    fatores = grupo.First().fatores,
                    fator0 = grupo.Sum(it => it.fator0),
                    fator1 = grupo.Sum(it => it.fator0),
                    conta = grupo.First().idCollaborator,
                    melhor = grupo.First().idCollaborator,
                    G1 = grupo.First().G1,
                    G2 = grupo.First().G2,
                    G3 = grupo.First().G3,
                    G4 = grupo.First().G4,
                    C1 = grupo.Max(it => it.C1),
                    C2 = grupo.Max(it => it.C2),
                    C3 = grupo.Max(it => it.C3),
                    C4 = grupo.Max(it => it.C4),
                    saldo = grupo.First().saldo,
                    typeIndicator = grupo.First().typeIndicator,
                    parentId = grupo.First().parentId,
                    levelName = grupo.First().levelName,
                    level = grupo.First().level,
                    transactionId = grupo.First().transactionId,
                    coins = grupo.Sum(it => it.coins),
                    quantidade = grupo.Count(),
                    matriculaGerentei = grupo.Key.matriculaGerentei,
                }).ToList();
            }
            else if (Hierarchy == "DIRETOR")
            {
                mrsReturn = mrs.GroupBy(item => new { item.idIndicator, item.matriculaDiretor }).Select(grupo => new MonetizationResultsModel
                {
                    idCollaborator = grupo.First().idCollaborator,
                    idIndicator = grupo.Key.idIndicator,
                    idResult = grupo.First().idResult,
                    idSector = grupo.First().idSector,
                    idCheckingAccount = 0,
                    indicatorWeight = grupo.First().indicatorWeight,
                    hierarchyLevel = "6",
                    meta = grupo.First().meta,
                    fatores = grupo.First().fatores,
                    fator0 = grupo.Sum(it => it.fator0),
                    fator1 = grupo.Sum(it => it.fator0),
                    conta = grupo.First().idCollaborator,
                    melhor = grupo.First().idCollaborator,
                    G1 = grupo.First().G1,
                    G2 = grupo.First().G2,
                    G3 = grupo.First().G3,
                    G4 = grupo.First().G4,
                    C1 = grupo.Max(it => it.C1),
                    C2 = grupo.Max(it => it.C2),
                    C3 = grupo.Max(it => it.C3),
                    C4 = grupo.Max(it => it.C4),
                    saldo = grupo.First().saldo,
                    typeIndicator = grupo.First().typeIndicator,
                    parentId = grupo.First().parentId,
                    levelName = grupo.First().levelName,
                    level = grupo.First().level,
                    transactionId = grupo.First().transactionId,
                    coins = grupo.Sum(it => it.coins),
                    quantidade = grupo.Count(),
                    matriculaDiretor = grupo.Key.matriculaDiretor,
                }).ToList();
            }
            else if (Hierarchy == "CEO")
            {
                mrsReturn = mrs.GroupBy(item => new { item.idIndicator, item.matriculaCeo }).Select(grupo => new MonetizationResultsModel
                {
                    idCollaborator = grupo.First().idCollaborator,
                    idIndicator = grupo.Key.idIndicator,
                    idResult = grupo.First().idResult,
                    idSector = grupo.First().idSector,
                    idCheckingAccount = 0,
                    indicatorWeight = grupo.First().indicatorWeight,
                    hierarchyLevel = "7",
                    meta = grupo.First().meta,
                    fatores = grupo.First().fatores,
                    fator0 = grupo.Sum(it => it.fator0),
                    fator1 = grupo.Sum(it => it.fator0),
                    conta = grupo.First().idCollaborator,
                    melhor = grupo.First().idCollaborator,
                    G1 = grupo.First().G1,
                    G2 = grupo.First().G2,
                    G3 = grupo.First().G3,
                    G4 = grupo.First().G4,
                    C1 = grupo.Max(it => it.C1),
                    C2 = grupo.Max(it => it.C2),
                    C3 = grupo.Max(it => it.C3),
                    C4 = grupo.Max(it => it.C4),
                    saldo = grupo.First().saldo,
                    typeIndicator = grupo.First().typeIndicator,
                    parentId = grupo.First().parentId,
                    levelName = grupo.First().levelName,
                    level = grupo.First().level,
                    transactionId = grupo.First().transactionId,
                    coins = grupo.Sum(it => it.coins),
                    quantidade = grupo.Count(),
                    matriculaCeo = grupo.Key.matriculaCeo,
                }).ToList();
            }


            return mrsReturn;
        }

        public void doMonetizationHierarchyNew(string dateM, string idReferer, MonetizationResultsModel mr)
        {
            //Pega Hierarquia
            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                connection.Open();
                try
                {

                    List<MonetizationHierarchy> mons = getMonetizationHierarchyNew(dateM, idReferer, mr.idIndicator);

                    //Fazer media x Comparação
                    if (mons.Count == 0)
                    {
                        return;
                    }
                    double quantidadeHierarquia = mr.quantidade;
                    var somaHierarquia = mr.coins;
                    var quantidadeReferencia = mons[0].Quantidade;
                    double somaReferencia = mons[0].Soma;
                    double valorMonetizar = 0;


                    var ultimoSaldo = 0;
                    try
                    {
                        ultimoSaldo = mons[1].Soma;
                    }
                    catch (Exception)
                    {

                        throw;
                    }


                    double mediaMonetizacao = 0;
                    if (quantidadeHierarquia > 0)
                    {
                        mediaMonetizacao = Math.Round(somaHierarquia / quantidadeHierarquia, MidpointRounding.AwayFromZero);
                    }

                    var mediaReferencia = somaReferencia;

                    if (mediaMonetizacao > mr.C1)
                    {
                        connection.Close();
                        return;
                    }

                    if (mediaMonetizacao > mediaReferencia)
                    {
                        valorMonetizar = mediaMonetizacao - mediaReferencia;

                        valorMonetizar = Math.Round(valorMonetizar, 0, MidpointRounding.AwayFromZero);

                        var saldoFinal = valorMonetizar + ultimoSaldo;

                        //Inserir moedas                    
                        StringBuilder stbCheckingAccount = new StringBuilder();
                        stbCheckingAccount.Append("INSERT INTO GDA_CHECKING_ACCOUNT (INPUT, OUTPUT, BALANCE, COLLABORATOR_ID, CREATED_AT, ");
                        stbCheckingAccount.Append(" GDA_INDICATOR_IDGDA_INDICATOR, GDA_ORDER_IDGDA_ORDER, CREATEDBYCOLLABORATORID, OBSERVATION, ");
                        stbCheckingAccount.Append("REASON, IDGDA_RESULT, RESULT_DATE, WEIGHT, IDGDA_SECTOR) OUTPUT INSERTED.ID VALUES( ");
                        stbCheckingAccount.AppendFormat("'{0}',", valorMonetizar); //INPUT
                        stbCheckingAccount.AppendFormat("'{0}',", 0); //OUTPUT
                        stbCheckingAccount.AppendFormat("'{0}',", saldoFinal); //BALANCE
                        stbCheckingAccount.AppendFormat("'{0}',", idReferer); //COLLABORATOR_ID
                        stbCheckingAccount.AppendFormat("{0},", "GETDATE()"); //CREATED_AT
                        stbCheckingAccount.AppendFormat("'{0}',", mr.idIndicator); //GDA_INDICATOR_IDGDA_INDICATOR
                        stbCheckingAccount.AppendFormat("{0},", "NULL"); //GDA_ORDER_IDGDA_ORDER
                        stbCheckingAccount.AppendFormat("{0},", "NULL"); //CREATEDBYCOLLABORATORID
                        stbCheckingAccount.AppendFormat("{0},", "NULL"); //OBSERVATION
                        stbCheckingAccount.AppendFormat("{0},", "NULL"); //REASON
                        stbCheckingAccount.AppendFormat("'{0}',", mr.idResult); //IRGDA_RESULT
                        stbCheckingAccount.AppendFormat("'{0}',", dateM); //RESULT_DATE
                        stbCheckingAccount.AppendFormat("'{0}',", mr.indicatorWeight); //WEIGHT
                        stbCheckingAccount.AppendFormat("'{0}'", mr.idSector); //IDGDA_SECTOR
                        stbCheckingAccount.Append("); ");
                        using (SqlCommand cmd = new SqlCommand(stbCheckingAccount.ToString(), connection))
                        {
                            mr.idCheckingAccount = (int)cmd.ExecuteScalar();
                        }

                        //Inserir moedas historico
                        StringBuilder stbConsolChecking = new StringBuilder();
                        stbConsolChecking.Append("INSERT INTO GDA_CONSOLIDATE_CHECKING_ACCOUNT (MONETIZATION, ");
                        stbConsolChecking.Append("GDA_CHECKING_ACCOUNT_IDGDA_CHECKING_ACCOUNT, GDA_HIERARCHY_IDGDA_HIERARCHY,  ");
                        stbConsolChecking.Append("GDA_INDICATOR_IDGDA_INDICATOR, GDA_SECTOR_IDGDA_SECTOR, CREATED_AT, DELETED_AT, ");
                        stbConsolChecking.Append("IDGDA_RESULT, RESULT_DATE, WEIGHT) VALUES (");
                        stbConsolChecking.AppendFormat("'{0}', ", valorMonetizar.ToString()); //MONETIZATION
                        stbConsolChecking.AppendFormat("'{0}', ", mr.idCheckingAccount); //GDA_CHECKING_ACCOUNT_IDGDA_CHECKING_ACCOUNT
                        stbConsolChecking.AppendFormat("'{0}', ", mr.hierarchyLevel); //GDA_HIERARCHY_IDGDA_HIERARCHY
                        stbConsolChecking.AppendFormat("'{0}', ", mr.idIndicator); //GDA_INDICATOR_IDGDA_INDICATOR
                        stbConsolChecking.AppendFormat("'{0}', ", mr.idSector); //GDA_SECTOR_IDGDA_SECTOR
                        stbConsolChecking.AppendFormat("{0}, ", "GETDATE()"); //CREATED_AT
                        stbConsolChecking.AppendFormat("{0}, ", "NULL"); //DELETED_AT
                        stbConsolChecking.AppendFormat("'{0}', ", mr.idResult); //IDGDA_RESULT
                        stbConsolChecking.AppendFormat("'{0}', ", dateM); //RESULT_DATE
                        stbConsolChecking.AppendFormat("'{0}' ", mr.indicatorWeight); //WEIGHT
                        stbConsolChecking.Append(");");
                        SqlCommand insertConsolCheckingCommand = new SqlCommand(stbConsolChecking.ToString(), connection);
                        insertConsolCheckingCommand.ExecuteNonQuery();


                    }

                    //}
                }



                catch (Exception ex)
                {

                    throw;
                }
                connection.Close();
            }
        }

        public List<MonetizationHierarchy> getMonetizationHierarchyNew(string dateM, string idEnv, string idIndicador)
        {
            List<MonetizationHierarchy> mhs = new List<MonetizationHierarchy>();

            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                try
                {
                    connection.Open();
                    StringBuilder stb = new StringBuilder();

                    stb.AppendFormat("DECLARE @INPUTID INT;SET @INPUTID = '{0}'; ", idEnv);
                    stb.AppendFormat("DECLARE @DATEENV DATE; SET @DATEENV = '{0}'; ", dateM);
                    stb.AppendFormat("DECLARE @INDICADORID VARCHAR(MAX); SET @INDICADORID = '{0}'; ", idIndicador);
                    //stb.AppendFormat("DECLARE @SECTORID VARCHAR(MAX); SET @SECTORID = '{0}'; ", idSector);

                    stb.Append("SELECT COUNT(DISTINCT COLLABORATOR_ID) AS QUANTIDADE, ");
                    stb.Append("       CASE ");
                    stb.Append("           WHEN SUM(INPUT) IS NULL THEN 0 ");
                    stb.Append("           ELSE SUM(INPUT) ");
                    stb.Append("       END AS SOMA ");
                    stb.Append("FROM GDA_CHECKING_ACCOUNT (NOLOCK) CA ");
                    stb.Append("WHERE CA.COLLABORATOR_ID = @INPUTID ");
                    stb.Append("  AND CA.RESULT_DATE = @DATEENV ");
                    stb.Append("  AND GDA_INDICATOR_IDGDA_INDICATOR = @INDICADORID ");
                    //stb.Append("  AND IDGDA_SECTOR = @SECTORID ");
                    stb.Append("UNION ALL ");
                    stb.Append("SELECT COALESCE(SUM(QUANTIDADE), 0) AS QUANTIDADE, ");
                    stb.Append("       COALESCE(SUM(SOMA), 0) AS SOMA ");
                    stb.Append("FROM ");
                    stb.Append("  (SELECT TOP 1 '1' AS QUANTIDADE, ");
                    stb.Append("              BALANCE AS SOMA ");
                    stb.Append("   FROM GDA_CHECKING_ACCOUNT (NOLOCK) CA ");
                    stb.Append("   WHERE CA.COLLABORATOR_ID = @INPUTID ");
                    stb.Append("   ORDER BY CREATED_AT DESC ");
                    stb.Append("   UNION ALL SELECT 0 AS QUANTIDADE, ");
                    stb.Append("                    0 AS SOMA) AS A ");


                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                MonetizationHierarchy mh = new MonetizationHierarchy();
                                mh.Quantidade = int.Parse(reader["QUANTIDADE"].ToString());
                                mh.Soma = int.Parse(reader["SOMA"].ToString());

                                mhs.Add(mh);

                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                connection.Close();
            }

            return mhs;
        }

    }





}