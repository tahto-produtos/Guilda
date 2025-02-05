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
    public class ResultsConsolidatedControllerOld  : ApiController
    {
        //private string connectionString = "Data Source=database-1.cpmxhjwr8mgp.us-east-1.rds.amazonaws.com;Initial Catalog=GUILDA_PROD;User ID=RDSadminsql2019;Password=H4ND4faG3AhjFe943NyPLRhMsEamXucKdDa;MultipleActiveResultSets=True;Connection Timeout=10";
        private RepositorioDBContext db = new RepositorioDBContext();

        //// POST: api/Results
        //[ResponseType(typeof(ResultsModel))]
        //public IHttpActionResult PostResultsModel(long t_id, IEnumerable<Result> results)
        //{
        //    bool validation = false;

        //    //Valida Token
        //    var tkn = TokenValidate.validate(Request.Headers);
        //    if (tkn == false)
        //    {
        //        return BadRequest("Invalid Token!");
        //    }

        //    //Valida Model
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    //Valida Transaction
        //    var t = TransactionValidade.validate(Request.Headers, t_id);
        //    if (t is null)
        //    {
        //        return BadRequest("Invalid Transaction!");
        //    }


        //    foreach (Result re in results)
        //    {
        //        //Valida se o colaborador ja existe
        //        //CollaboratorTableModel cl = db.CollaboratorTableModels.Where(p => p.collaboratorIdentification == re.collaboratorIdentification).FirstOrDefault();
        //        //if (cl is null)
        //        //{
        //        //    continue;
        //        //}

        //        ResultsModel rm = new ResultsModel();

        //        var idEx = re.collaboratorIdentification.Replace("BC", "");

        //        rm.IDGDA_COLLABORATORS = Convert.ToInt32(idEx);
        //        rm.INDICADORID = re.indicadorId;
        //        rm.TRANSACTIONID = t.idgda_Transaction;

        //        rm.RESULT = re.resultado;
        //        rm.CREATED_AT = re.date;

        //        //var factors = "";
        //        //foreach (string fm in re.factors)
        //        //{
        //        //    if (factors == "")
        //        //    {
        //        //        factors = fm;
        //        //    }
        //        //    else
        //        //    {
        //        //        factors = factors + ";" + fm;
        //        //    }

        //        //}

        //        //rm.factors = factors;

        //        try
        //        {

        //            db.ResultsModels.Add(rm);
        //            db.SaveChanges();
        //            validation = true;
        //            var index = 1;

        //            foreach (string fm in re.factors)
        //            {

        //                FactorsModel facModel = new FactorsModel();
        //                facModel.IDGDA_RESULT = rm.IDGDA_RESULT;
        //                facModel.INDEX = index;
        //                facModel.FACTOR = fm;
        //                try
        //                {
        //                    db.FactorsModels.Add(facModel);
        //                    db.SaveChanges();
        //                }
        //                catch
        //                { }
        //                validation = true;
        //                index += 1;
        //            }
        //        }
        //        catch
        //        { }
        //    }

        //    //try
        //    //{
        //    //    db.SaveChanges();
        //    //    validation = true;
        //    //}
        //    //catch
        //    //{
        //    //}

        //    //var tknNovo = getTokenApi();

        //    //enviaResultado(t_id.ToString(), tknNovo);

        //    if (validation == true)
        //    {

        //        return CreatedAtRoute("DefaultApi", new { id = results.First().collaboratorIdentification }, results);
        //    }
        //    else
        //    {
        //        return BadRequest("No information entered.");
        //    }
        //}






        //[ResponseType(typeof(ResultsModel))]
        //public IHttpActionResult PostResultsModel(long t_id, IEnumerable<Result> results)
        //{

        //    try
        //    {
        //        // Criar uma lista de strings no formato CSV para a Tabela1
        //        var tabela1CsvData = new List<string>();
        //        tabela1CsvData.Add("collaboratorIdentification,indicadorId,resultado,date");

        //        // Criar uma lista de strings no formato CSV para a Tabela2
        //        var tabela2CsvData = new List<string>();
        //        tabela2CsvData.Add("factor,idgda_result,index");

        //        // Gerar os dados para as tabelas 1 e 2
        //        foreach (var inputData in results)
        //        {
        //            // Adicionar linha na tabela1
        //            var tabela1CsvLine = $"{inputData.collaboratorIdentification},{inputData.indicadorId},{inputData.resultado},{inputData.date:yyyy-MM-dd}";
        //            tabela1CsvData.Add(tabela1CsvLine);

        //            var index = 0;
        //            // Adicionar linhas na tabela2 com base nos fatores
        //            foreach (var factor in inputData.factors)
        //            {
        //                index += 1;
        //                var tabela2CsvLine = $"{factor},{inputData.idgda_result},{index}";
        //                tabela2CsvData.Add(tabela2CsvLine);
        //            }
        //        }

        //        // Converter as listas de strings em strings CSV
        //        var tabela1CsvContent = string.Join(Environment.NewLine, tabela1CsvData);
        //        var tabela2CsvContent = string.Join(Environment.NewLine, tabela2CsvData);

        //        // Salvar os dados em arquivos CSV temporários
        //        var tabela1TempFilePath = Path.GetTempFileName() + ".csv";
        //        File.WriteAllText(tabela1TempFilePath, tabela1CsvContent);

        //        var tabela2TempFilePath = Path.GetTempFileName() + ".csv";
        //        File.WriteAllText(tabela2TempFilePath, tabela2CsvContent);

        //        // Realizar o BULK INSERT nas tabelas utilizando os arquivos CSV temporários
        //        using (var connection = new SqlConnection(""))
        //        {
        //            connection.Open();

        //            // Criar tabela temporária para Tabela1
        //            var createTabela1TempTableSql = @"
        //            CREATE TABLE #Tabela1Temp (
        //                collaboratorIdentification VARCHAR(100),
        //                indicadorId INT,
        //                resultado FLOAT,
        //                [date] DATE
        //            )";

        //            using (var command = new SqlCommand(createTabela1TempTableSql, connection))
        //            {
        //                command.ExecuteNonQuery();
        //            }

        //            // Carregar dados do arquivo CSV para tabela temporária Tabela1
        //            var bulkCopy1 = new SqlBulkCopy(connection);
        //            bulkCopy1.DestinationTableName = "#Tabela1Temp";
        //            using (var reader = new StreamReader(tabela1TempFilePath))
        //            using (var csv = new CsvHelper.CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
        //            {
        //                bulkCopy1.WriteToServer((IDataReader)csv);
        //            }

        //            // Criar tabela temporária para Tabela2
        //            var createTabela2TempTableSql = @"
        //            CREATE TABLE #Tabela2Temp (
        //                factor FLOAT,
        //                index INT,
        //                idgda_result INT
        //            )";

        //            using (var command = new SqlCommand(createTabela2TempTableSql, connection))
        //            {
        //                command.ExecuteNonQuery();
        //            }

        //            // Carregar dados do arquivo CSV para tabela temporária Tabela2
        //            var bulkCopy2 = new SqlBulkCopy(connection);
        //            bulkCopy2.DestinationTableName = "#Tabela2Temp";
        //            using (var reader = new StreamReader(tabela2TempFilePath))
        //            using (var csv = new CsvHelper.CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
        //            {
        //                bulkCopy2.WriteToServer((IDataReader)csv);
        //            }

        //            // Realizar a inserção final nas tabelas

        //            var insercaoFinalSql = @"
        //            INSERT INTO Tabela1 (collaboratorIdentification, indicadorId, resultado, date)
        //            SELECT collaboratorIdentification, indicadorId, resultado, date FROM #Tabela1Temp;

        //            INSERT INTO Tabela2 (factor, tabela1Id)
        //            SELECT factor, tabela1Id FROM #Tabela2Temp;

        //            DROP TABLE #Tabela1Temp;
        //            DROP TABLE #Tabela2Temp;";

        //            using (var command = new SqlCommand(insercaoFinalSql, connection))
        //            {
        //                command.ExecuteNonQuery();
        //            }

        //            connection.Close();
        //        }

        //        // Remover os arquivos CSV temporários após o BULK INSERT
        //        File.Delete(tabela1TempFilePath);
        //        File.Delete(tabela2TempFilePath);

        //        return Ok("Dados inseridos com sucesso.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Ocorreu um erro ao inserir os dados: {ex.Message}");
        //    }

        //}

        // POST: api/Results
        [ResponseType(typeof(ResultsModel))]
        public IHttpActionResult PostResultsModel(long t_id, IEnumerable<Result> results)
        {
            bool validation = false;
            Log.insertLogTransaction(t_id.ToString(), "RESULT_CONSOLIDATED", "START", "");

            //Valida Token
            var tkn = TokenValidate.validate(Request.Headers);
            if (tkn == false)
            {
                Log.insertLogTransaction(t_id.ToString(), "RESULT_CONSOLIDATED", "Invalid Token!", "");
                return BadRequest("Invalid Token!");
            }

            //Valida Model
            if (!ModelState.IsValid)
            {
                Log.insertLogTransaction(t_id.ToString(), "RESULT_CONSOLIDATED", "Invalid Model!", "");
                return BadRequest(ModelState);
            }

            //Valida Transaction
            var t = TransactionValidade.validateOnlyNumber(Request.Headers, t_id);
            if (t is null)
            {
                Log.insertLogTransaction(t_id.ToString(), "RESULT_CONSOLIDATED", "Invalid Transaction!", "");
                return BadRequest("Invalid Transaction!");
            }

            try
            {

                //string connectionString = "Data Source=database-1.cpmxhjwr8mgp.us-east-1.rds.amazonaws.com;Initial Catalog=GUILDA_PROD;User ID=RDSadminsql2019;Password=H4ND4faG3AhjFe943NyPLRhMsEamXucKdDa;MultipleActiveResultSets=True;Connection Timeout=10";
                SqlConnection connection = new SqlConnection(Database.Conn);
                try
                {
                    connection.Open();

                    string createTempTableQuery = $"CREATE TABLE #INSERTEDIDS (ID INT);";
                    using (SqlCommand createTempTableCommand = new SqlCommand(createTempTableQuery, connection))
                    {
                        createTempTableCommand.ExecuteNonQuery();
                    }

                    StringBuilder temp = new StringBuilder();
                    temp.Append("SELECT * ");
                    temp.Append("INTO #TEMPTABLE ");
                    temp.Append("FROM GDA_HISTORY_LAST_RESULT ");
                    temp.AppendFormat("WHERE TRANSACTIONID = {0} ", t.idgda_Transaction);
                    SqlCommand createTableCommandteste = new SqlCommand(temp.ToString(), connection);
                    createTableCommandteste.CommandTimeout = 0;
                    createTableCommandteste.ExecuteNonQuery();

                    //Atualizar deleted_at dos resultados que tiveram modificação [DESCOMENTAR QUANDO EXISTIR COLUNA]
                    StringBuilder queryInsertResult1 = new StringBuilder();
                    queryInsertResult1.Append("UPDATE GDA_RESULT ");
                    queryInsertResult1.Append("SET DELETED_AT = GETDATE() ");
                    queryInsertResult1.Append("FROM GDA_RESULT AS TARGET ");
                    queryInsertResult1.Append("INNER JOIN (SELECT * FROM #TEMPTABLE (NOLOCK) ");
                    queryInsertResult1.Append("			GROUP BY COLLABORATORIDENTIFICATION, ");
                    queryInsertResult1.Append("			INDICADORID, ");
                    queryInsertResult1.Append("			RESULTADO, ");
                    queryInsertResult1.Append("			DATE, ");
                    queryInsertResult1.Append("			FACTORS, ");
                    queryInsertResult1.Append("			IDGDA_RESULT, ");
                    queryInsertResult1.Append("			TRANSACTIONID ");
                    queryInsertResult1.Append(") AS SOURCE ON (TARGET.INDICADORID = SOURCE.INDICADORID ");
                    queryInsertResult1.Append("				AND TARGET.CREATED_AT = SOURCE.DATE ");
                    queryInsertResult1.Append("				AND TARGET.IDGDA_COLLABORATORS = SOURCE.COLLABORATORIDENTIFICATION ");
                    queryInsertResult1.Append("				AND TARGET.FACTORS <> SOURCE.FACTORS AND DELETED_AT IS NULL); ");
                    SqlCommand createTableCommand1 = new SqlCommand(queryInsertResult1.ToString(), connection);
                    createTableCommand1.CommandTimeout = 0;
                    createTableCommand1.ExecuteNonQuery();

                    //Insert Bulk Result
                    StringBuilder queryInsertResult = new StringBuilder();
                    queryInsertResult.Append("MERGE INTO GDA_RESULT AS TARGET ");
                    queryInsertResult.Append("USING #TEMPTABLE AS SOURCE ");
                    queryInsertResult.Append("ON (TARGET.INDICADORID = SOURCE.INDICADORID AND TARGET.CREATED_AT = SOURCE.DATE AND TARGET.IDGDA_COLLABORATORS = SOURCE.COLLABORATORIDENTIFICATION AND TARGET.DELETED_AT IS NULL) ");
                    queryInsertResult.Append("WHEN NOT MATCHED BY TARGET THEN ");
                    queryInsertResult.Append("  INSERT (INDICADORID, TRANSACTIONID, RESULT, CREATED_AT, IDGDA_COLLABORATORS, FACTORS) ");
                    queryInsertResult.Append("  VALUES (INDICADORID, TRANSACTIONID, RESULTADO,[DATE], COLLABORATORIDENTIFICATION, FACTORS) ");
                    queryInsertResult.Append("  OUTPUT INSERTED.IDGDA_RESULT INTO #INSERTEDIDS; ");
                    SqlCommand createTableCommand3 = new SqlCommand(queryInsertResult.ToString(), connection);
                    createTableCommand3.CommandTimeout = 0;
                    createTableCommand3.ExecuteNonQuery();

                    //Insert Into Factor
                    StringBuilder queryInsertFactor = new StringBuilder();
                    queryInsertFactor.Append("INSERT INTO GDA_FACTOR ([INDEX], FACTOR, IDGDA_RESULT) ");
                    queryInsertFactor.Append("SELECT ROW_NUMBER() OVER (PARTITION BY IDGDA_RESULT ORDER BY (SELECT NULL)) AS [INDEX], VALUE, IDGDA_RESULT ");
                    queryInsertFactor.Append("FROM GDA_RESULT (NOLOCK) ");
                    queryInsertFactor.Append("CROSS APPLY STRING_SPLIT(FACTORS, ';') ");
                    queryInsertFactor.AppendFormat("WHERE GDA_RESULT.TRANSACTIONID = {0} AND GDA_RESULT.IDGDA_RESULT IN (SELECT ID FROM #INSERTEDIDS); ", t.idgda_Transaction);
                    string commandText3 = queryInsertFactor.ToString();
                    SqlCommand createTableCommand4 = new SqlCommand(commandText3, connection);
                    createTableCommand4.CommandTimeout = 0;
                    createTableCommand4.ExecuteNonQuery();

                    // Remove a tabela temporária
                    string dropTempTableQuery = $"DROP TABLE #INSERTEDIDS";
                    using (SqlCommand dropTempTableCommand = new SqlCommand(dropTempTableQuery, connection))
                    {
                        dropTempTableCommand.ExecuteNonQuery();
                    }

                    string dropTempTableQuery2 = $"DROP TABLE #TEMPTABLE";
                    using (SqlCommand dropTempTableCommand2 = new SqlCommand(dropTempTableQuery2, connection))
                    {
                        dropTempTableCommand2.ExecuteNonQuery();
                    }

                    Log.insertLogTransaction(t_id.ToString(), "RESULT_CONSOLIDATED", "CONCLUDED", "");
                    validation = true;
                }
                catch (Exception ex)
                {
                    Log.insertLogTransaction(t_id.ToString(), "RESULT_CONSOLIDATED", "ERRO: " + ex.Message.ToString(), "");
                }
                connection.Close();

            }
            catch (Exception ex)
            {
                Log.insertLogTransaction(t_id.ToString(), "RESULT_CONSOLIDATED", "ERRO: " + ex.Message.ToString(), "");
            }

            try
            {
                //List<string> lt = new List<string>();
                //lt.Add("2023-10-01");
                //lt.Add("2023-10-02");
                //lt.Add("2023-10-03");
                //lt.Add("2023-10-04");
                //lt.Add("2023-10-05");
                //lt.Add("2023-10-06");
                //lt.Add("2023-10-07");
                //lt.Add("2023-10-08");
                //lt.Add("2023-10-09");
                //lt.Add("2023-10-10");
                //lt.Add("2023-10-11");
                //lt.Add("2023-10-12");
                //lt.Add("2023-10-13");
                //lt.Add("2023-10-14");
                //lt.Add("2023-10-15");
                //lt.Add("2023-10-16");
                //lt.Add("2023-10-17");
                //lt.Add("2023-10-18");
                //lt.Add("2023-10-19");
                //lt.Add("2023-10-20");
                //lt.Add("2023-10-21");
                //lt.Add("2023-10-22");
                //lt.Add("2023-10-23");
                //lt.Add("2023-10-24");
                //lt.Add("2023-10-25");
                //lt.Add("2023-10-26");

                //foreach (string item in lt)
                //{
                //    insereTabelasAuxiliares(item);
                //}

                try
                {
                    //string connectionString = "Data Source=database-1.cpmxhjwr8mgp.us-east-1.rds.amazonaws.com;Initial Catalog=GUILDA_PROD;User ID=RDSadminsql2019;Password=H4ND4faG3AhjFe943NyPLRhMsEamXucKdDa;MultipleActiveResultSets=True;Connection Timeout=10";
                    SqlConnection connection = new SqlConnection(Database.Conn);

                    List<string> datas = new List<string>();

                    StringBuilder stb = new StringBuilder();
                    stb.Append("SELECT DATA FROM GDA_REPROCESS_TABLE_PHOTO (NOLOCK) ");
                    stb.Append("WHERE REPROCESS = 0 ");
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.CommandTimeout = 120;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                try
                                {
                                    string dt = reader["DATA"].ToString();
                                    DateTime dts = Convert.ToDateTime(dt);
                                    string dtf = dts.ToString("yyyy-MM-dd");

                                    datas.Add(dtf);
                                }
                                catch (Exception ex)
                                {

                                }

                            }
                        }
                    }

                    connection.Close();

                    foreach (string dt in datas)
                    {
                        insereTabelasAuxiliares(dt);

                        try
                        {
                            connection.Open();
                            StringBuilder queryR = new StringBuilder();
                            queryR.AppendFormat("UPDATE GDA_REPROCESS_TABLE_PHOTO SET REPROCESS = 1 WHERE DATA = '{0}' ", dt);
                            SqlCommand createTableCommandR = new SqlCommand(queryR.ToString(), connection);
                            createTableCommandR.CommandTimeout = 0;
                            createTableCommandR.ExecuteNonQuery();
                        }
                        catch (Exception)
                        {
                        }
                        connection.Close();
                    }
                }
                catch (Exception)
                {

                }

                insereTabelasAuxiliares(DateTime.Now.ToString("yyyy-MM-dd"));
            }
            catch (Exception)
            {

            }

            try
            {


                Log.insertLogTransaction(t_id.ToString(), "MONETIZATION", "START", "");

                //List<int> listas = new List<int>();
                //listas.Add(490);
                //listas.Add(494);
                //listas.Add(495);
                //listas.Add(496);
                //listas.Add(500);
                //listas.Add(501);
                //listas.Add(502);
                //listas.Add(506);
                //listas.Add(507);
                //listas.Add(508);
                //listas.Add(512);
                //listas.Add(513);
                //listas.Add(514);
                //listas.Add(518);
                //listas.Add(519);
                //listas.Add(520);
                //listas.Add(523);
                //listas.Add(524);
                //listas.Add(527);
                //listas.Add(531);
                //listas.Add(532);
                //listas.Add(533);
                //listas.Add(537);
                //listas.Add(538);
                //listas.Add(539);
                //listas.Add(543);
                //listas.Add(544);
                //listas.Add(548);
                //listas.Add(550);
                //listas.Add(551);
                //listas.Add(556);
                //listas.Add(557);
                //listas.Add(558);
                //listas.Add(562);
                //listas.Add(563);
                //listas.Add(564);
                //listas.Add(568);
                //listas.Add(569);

                //foreach (int item in listas)
                //{
                //    monetization(item).ToString();
                //    Log.insertLogTransaction(item.ToString(), "MONETIZATION BKP", "CONCLUDED", "");
                //}

                monetization(t.idgda_Transaction).ToString();
                Log.insertLogTransaction(t_id.ToString(), "MONETIZATION", "CONCLUDED", "");





            }
            catch (Exception ex)
            {
                Log.insertLogTransaction(t_id.ToString(), "MONETIZATION", "ERRO: " + ex.Message.ToString(), "");
            }


            try
            {
                for (int i = 1; i < 10; i++)
                {
                    DateTime dtAgora = DateTime.Now.AddDays(-i);

                    doBasketMonetization(dtAgora.ToString("yyyy-MM-dd"));
                }
            }
            catch (Exception)
            {

            }
            try
            {
                //Remove tudo do last result para performance
                //string connectionString = "Data Source=database-1.cpmxhjwr8mgp.us-east-1.rds.amazonaws.com;Initial Catalog=GUILDA_PROD;User ID=RDSadminsql2019;Password=H4ND4faG3AhjFe943NyPLRhMsEamXucKdDa;MultipleActiveResultSets=True;Connection Timeout=10";
                SqlConnection connection = new SqlConnection(Database.Conn);

                connection.Open();
                StringBuilder queryR = new StringBuilder();
                queryR.Append("TRUNCATE TABLE GDA_HISTORY_LAST_RESULT ");
                SqlCommand createTableCommandR = new SqlCommand(queryR.ToString(), connection);
                createTableCommandR.CommandTimeout = 0;
                createTableCommandR.ExecuteNonQuery();

                connection.Close();
            }
            catch (Exception)
            {

            }


            if (validation == true)
            {
                return StatusCode(HttpStatusCode.Created);
            }
            else
            {
                return BadRequest("No information entered.");
            }
        }

        //public bool monetization(int transactionID)
        //{
        //    // Pegar todas as datas diferentes que existem nessa transaction
        //    List<string> datas = getDateTransaction(transactionID);

        //    //Varrer as datas retornadas
        //    foreach (string dt in datas)
        //    {

        //        List<MonetizationResultsModel> mrs = new List<MonetizationResultsModel>();

        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            connection.Open();
        //            try
        //            {
        //                StringBuilder stb = new StringBuilder();
        //                stb.Append("SELECT R.*,HIS.GOAL, I.WEIGHT AS WEIGHT, HHR.LEVELWEIGHT AS HIERARCHYLEVEL, HIG1.MONETIZATION AS COIN1, HIG2.MONETIZATION AS COIN2, HIG3.MONETIZATION AS COIN3,  ");
        //                stb.Append("HIG4.MONETIZATION AS COIN4, S.IDGDA_SECTOR, MAX(I.TYPE) AS TYPE, HIG1.METRIC_MIN AS MIN1, HIG2.METRIC_MIN AS MIN2, HIG3.METRIC_MIN AS MIN3, HIG4.METRIC_MIN AS MIN4, ");
        //                stb.Append("CASE WHEN MAX(TBL.EXPRESSION) IS NULL THEN '(#FATOR1/#FATOR0)' ELSE MAX(TBL.EXPRESSION) END AS CONTA, ");
        //                stb.Append("CASE WHEN MAX(I.CALCULATION_TYPE) IS NULL THEN 'BIGGER_BETTER' ELSE MAX(I.CALCULATION_TYPE) END AS BETTER, ");
        //                stb.Append("COALESCE((SELECT TOP 1 BALANCE FROM GDA_CHECKING_ACCOUNT (NOLOCK) WHERE COLLABORATOR_ID = R.IDGDA_COLLABORATORS ORDER BY CREATED_AT DESC), 0) AS SALDO ");
        //                stb.Append("FROM GDA_RESULT (NOLOCK) AS R ");
        //                stb.Append("INNER JOIN GDA_HISTORY_COLLABORATOR_SECTOR (NOLOCK) AS S ON CONVERT(DATE, S.CREATED_AT, 120) = CONVERT(DATE, R.CREATED_AT, 120) AND S.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
        //                stb.Append("LEFT JOIN ( ");
        //                stb.Append("    SELECT SECTOR_ID, INDICATOR_ID ");
        //                stb.Append("    FROM GDA_HISTORY_INDICATOR_GROUP ");
        //                stb.Append("    WHERE MONETIZATION > 0 AND DELETED_AT IS NULL ");
        //                stb.Append(") TS ON TS.SECTOR_ID = S.IDGDA_SECTOR AND TS.INDICATOR_ID = R.INDICADORID ");
        //                stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL AND HIG1.INDICATOR_ID = R.INDICADORID AND HIG1.SECTOR_ID = S.IDGDA_SECTOR AND HIG1.GROUPID = 1 ");
        //                stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG2 ON HIG2.DELETED_AT IS NULL AND HIG2.INDICATOR_ID = R.INDICADORID AND HIG2.SECTOR_ID = S.IDGDA_SECTOR AND HIG2.GROUPID = 2 ");
        //                stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG3 ON HIG3.DELETED_AT IS NULL AND HIG3.INDICATOR_ID = R.INDICADORID AND HIG3.SECTOR_ID = S.IDGDA_SECTOR AND HIG3.GROUPID = 3 ");
        //                stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG4 ON HIG4.DELETED_AT IS NULL AND HIG4.INDICATOR_ID = R.INDICADORID AND HIG4.SECTOR_ID = S.IDGDA_SECTOR AND HIG4.GROUPID = 4 ");
        //                stb.Append("LEFT JOIN (SELECT HME.INDICATORID, ME.EXPRESSION FROM GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME ");
        //                stb.Append("INNER JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON ME.ID = HME.MATHEMATICALEXPRESSIONID ");
        //                stb.Append("WHERE HME.DELETED_AT IS NULL) AS TBL ON TBL.INDICATORID = R.INDICADORID ");
        //                stb.AppendFormat("LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS HHR ON HHR.idgda_COLLABORATORS = R.IDGDA_COLLABORATORS AND HHR.DATE = '{0}' ", dt);
        //                stb.Append("INNER JOIN ( ");
        //                stb.Append("SELECT GOAL, INDICATOR_ID, SECTOR_ID, ROW_NUMBER() OVER (PARTITION BY INDICATOR_ID, SECTOR_ID ORDER BY CREATED_AT DESC) AS RN ");
        //                stb.Append("FROM GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) ");
        //                stb.Append("WHERE DELETED_AT IS NULL ");
        //                stb.Append(") AS HIS ON HIS.INDICATOR_ID = R.INDICADORID AND HIS.SECTOR_ID = S.IDGDA_SECTOR AND HIS.RN = 1 ");
        //                stb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS I ON I.IDGDA_INDICATOR = R.INDICADORID ");
        //                stb.AppendFormat("WHERE R.TRANSACTIONID = {0} AND CONVERT(DATE, R.CREATED_AT, 120) = '{1}' AND  TS.SECTOR_ID IS NOT NULL ", transactionID, dt);
        //                stb.Append("GROUP BY IDGDA_RESULT, INDICADORID, R.TRANSACTIONID, RESULT, R.CREATED_AT, R.IDGDA_COLLABORATORS, FACTORS, R.DELETED_AT, S.IDGDA_SECTOR, HIG1.METRIC_MIN,  ");
        //                stb.Append("HIG2.METRIC_MIN, HIG3.METRIC_MIN, HIG4.METRIC_MIN,HIS.GOAL, I.WEIGHT, HHR.LEVELWEIGHT, HIG1.MONETIZATION, HIG2.MONETIZATION, HIG3.MONETIZATION, HIG4.MONETIZATION ");




        //                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
        //                {
        //                    command.CommandTimeout = 0;
        //                    using (SqlDataReader reader = command.ExecuteReader())
        //                    {
        //                        while (reader.Read())
        //                        {
        //                            MonetizationResultsModel mr = new MonetizationResultsModel();

        //                            mr.idCollaborator = reader["IDGDA_COLLABORATORS"].ToString();
        //                            mr.idIndicator = reader["INDICADORID"].ToString();
        //                            mr.idResult = reader["IDGDA_RESULT"].ToString();
        //                            mr.idSector = reader["IDGDA_SECTOR"].ToString();
        //                            mr.idCheckingAccount = 0;
        //                            mr.indicatorWeight = reader["WEIGHT"].ToString();
        //                            mr.hierarchyLevel = reader["HIERARCHYLEVEL"].ToString();
        //                            mr.meta = double.Parse(reader["GOAL"].ToString());
        //                            mr.fatores = reader["FACTORS"].ToString();
        //                            mr.fator0 = reader["FACTORS"].ToString().Split(";")[0];
        //                            mr.fator1 = reader["FACTORS"].ToString().Split(";")[1];
        //                            mr.conta = reader["CONTA"].ToString();
        //                            mr.melhor = reader["BETTER"].ToString();
        //                            mr.G1 = double.Parse(reader["MIN1"].ToString());
        //                            mr.G2 = double.Parse(reader["MIN2"].ToString());
        //                            mr.G3 = double.Parse(reader["MIN3"].ToString());
        //                            mr.G4 = double.Parse(reader["MIN4"].ToString());
        //                            mr.C1 = double.Parse(reader["COIN1"].ToString());
        //                            mr.C2 = double.Parse(reader["COIN2"].ToString());
        //                            mr.C3 = double.Parse(reader["COIN3"].ToString());
        //                            mr.C4 = double.Parse(reader["COIN4"].ToString());
        //                            mr.saldo = double.Parse(reader["SALDO"].ToString());
        //                            mr.typeIndicator = reader["TYPE"].ToString();




        //                            mrs.Add(mr);
        //                        }
        //                    }
        //                }

        //                //Monetização Agentes - Monetiza todos os agentes primeiro.. Depois monetiza a hierarquia
        //                foreach (MonetizationResultsModel mr in mrs)
        //                {
        //                    doMonetizationAgent(transactionID, dt, mr);
        //                }

        //                //Monetização Hierarquia
        //                foreach (MonetizationResultsModel mr in mrs)
        //                {
        //                    if (mr.idIndicator == "371")
        //                    {
        //                        var parou = true;
        //                    }

        //                    doMonetizationHierarchy(transactionID, dt, mr);
        //                }


        //                doBasketMonetization(dt);
        //            }
        //            catch (Exception)
        //            {
        //                throw;
        //            }
        //            connection.Close();
        //        }

        //    }




        //    //




        //    return true;
        //}

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
                stb1.Append("	[IDGDA_SECTOR_SUPERVISOR] [int] NULL,");
                stb1.Append("	[IDGDA_SUBSECTOR] [int] NULL,");
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
                temp.Append("INSERT INTO #TEMPAG (IDGDA_COLLABORATORS, CARGO, CREATED_AT, IDGDA_SECTOR, IDGDA_SECTOR_SUPERVISOR, IDGDA_SUBSECTOR, HOME_BASED, SITE, PERIODO, MATRICULA_SUPERVISOR, NOME_SUPERVISOR, MATRICULA_COORDENADOR,  ");
                temp.Append("NOME_COORDENADOR, MATRICULA_GERENTE_II, NOME_GERENTE_II, MATRICULA_GERENTE_I, NOME_GERENTE_I, MATRICULA_DIRETOR, NOME_DIRETOR, MATRICULA_CEO, NOME_CEO, ACTIVE)  ");
                temp.Append("SELECT CB.IDGDA_COLLABORATORS, MAX(CLS.LEVELNAME), @DATAINICIAL, MAX(S.IDGDA_SECTOR) AS IDGDA_SECTOR, MAX(SSUPER.IDGDA_SECTOR) AS IDGDA_SECTOR_SUPER, MAX(SUBS.IDGDA_SECTOR) AS IDGDA_SUBSECTOR, ");
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
                temp.Append("LEFT JOIN GDA_HISTORY_COLLABORATOR_SUBSECTOR (NOLOCK) SUBS ON SUBS.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS AND SUBS.CREATED_AT = @DATAINICIAL  ");
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
                temp.Append("                      AND LV1.PARENTIDENTIFICATION IS NOT NULL THEN '-'  ");
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
                temp.Append("                                AND LV2.PARENTIDENTIFICATION IS NOT NULL THEN '-'  ");
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
                temp.Append("                                AND LV3.PARENTIDENTIFICATION IS NOT NULL THEN '-'  ");
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
                temp.Append("                                AND LV4.PARENTIDENTIFICATION IS NOT NULL THEN '-'  ");
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
                temp.Append("                                AND LV5.PARENTIDENTIFICATION IS NOT NULL THEN '-'  ");
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
                temp.Append("                                AND LV6.PARENTIDENTIFICATION IS NOT NULL THEN '-'  ");
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

                temp.Append("LEFT JOIN GDA_HISTORY_COLLABORATOR_SECTOR (NOLOCK) SSUPER ON HIERARCHY.LEVELWEIGHT = '2' AND SSUPER.IDGDA_COLLABORATORS = HIERARCHY.PARENTIDENTIFICATION ");
                temp.Append("AND SSUPER.CREATED_AT = @DATAINICIAL ");

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
                    //Verifica se não é data de reprocessamento
                    if (dt == DateTime.Now.ToString("yyyy-MM-dd"))
                    {
                        //Atualizar deleted_at dos resultados que tiveram modificação [DESCOMENTAR QUANDO EXISTIR COLUNA]
                        StringBuilder queryInsertResult1 = new StringBuilder();
                        queryInsertResult1.Append("MERGE INTO GDA_COLLABORATORS_LAST_DETAILS AS TARGET  ");
                        queryInsertResult1.Append("USING #TEMPAG AS SOURCE  ");
                        queryInsertResult1.Append("ON (TARGET.IDGDA_COLLABORATORS = SOURCE.IDGDA_COLLABORATORS)  ");
                        queryInsertResult1.Append("WHEN NOT MATCHED BY TARGET THEN  ");
                        queryInsertResult1.Append("  INSERT (IDGDA_COLLABORATORS, CARGO, IDGDA_SECTOR, IDGDA_SECTOR_SUPERVISOR, IDGDA_SUBSECTOR, HOME_BASED, SITE, PERIODO, MATRICULA_SUPERVISOR, NOME_SUPERVISOR, MATRICULA_COORDENADOR,  ");
                        queryInsertResult1.Append("		  NOME_COORDENADOR, MATRICULA_GERENTE_II, NOME_GERENTE_II, MATRICULA_GERENTE_I, NOME_GERENTE_I, MATRICULA_DIRETOR, NOME_DIRETOR, MATRICULA_CEO, NOME_CEO, ACTIVE)  ");
                        queryInsertResult1.Append("  VALUES (SOURCE.IDGDA_COLLABORATORS, SOURCE.CARGO, SOURCE.IDGDA_SECTOR, SOURCE.IDGDA_SECTOR_SUPERVISOR, SOURCE.IDGDA_SUBSECTOR, SOURCE.HOME_BASED, SOURCE.SITE, SOURCE.PERIODO, SOURCE.MATRICULA_SUPERVISOR, SOURCE.NOME_SUPERVISOR, SOURCE.MATRICULA_COORDENADOR,  ");
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
                        queryUp.Append("UPDATE D1 SET D1.IDGDA_SECTOR_SUPERVISOR = D2.IDGDA_SECTOR_SUPERVISOR ");
                        queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                        queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                        queryUp.Append("WHERE D2.IDGDA_SECTOR_SUPERVISOR IS NOT NULL ");
                        SqlCommand createnewS = new SqlCommand(queryUp.ToString(), connection);
                        createnewS.CommandTimeout = 0;
                        createnewS.ExecuteNonQuery();

                        queryUp.Clear();
                        queryUp.Append("UPDATE D1 SET D1.IDGDA_SUBSECTOR = D2.IDGDA_SUBSECTOR ");
                        queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                        queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                        queryUp.Append("WHERE D2.IDGDA_SUBSECTOR IS NOT NULL ");
                        SqlCommand createnewSUB = new SqlCommand(queryUp.ToString(), connection);
                        createnewSUB.CommandTimeout = 0;
                        createnewSUB.ExecuteNonQuery();

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
                        queryUp.Append("WHERE D2.MATRICULA_SUPERVISOR <> '0' ");
                        SqlCommand createUp2 = new SqlCommand(queryUp.ToString(), connection);
                        createUp2.CommandTimeout = 0;
                        createUp2.ExecuteNonQuery();

                        queryUp.Clear();
                        queryUp.Append("UPDATE D1 SET D1.MATRICULA_COORDENADOR = D2.MATRICULA_COORDENADOR ");
                        queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                        queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                        queryUp.Append("WHERE D2.MATRICULA_SUPERVISOR <> '0' ");
                        SqlCommand createUp3 = new SqlCommand(queryUp.ToString(), connection);
                        createUp3.CommandTimeout = 0;
                        createUp3.ExecuteNonQuery();

                        queryUp.Clear();
                        queryUp.Append("UPDATE D1 SET D1.NOME_COORDENADOR = D2.NOME_COORDENADOR ");
                        queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                        queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                        queryUp.Append("WHERE D2.MATRICULA_SUPERVISOR <> '0' ");
                        SqlCommand createUp4 = new SqlCommand(queryUp.ToString(), connection);
                        createUp4.CommandTimeout = 0;
                        createUp4.ExecuteNonQuery();

                        queryUp.Clear();
                        queryUp.Append("UPDATE D1 SET D1.MATRICULA_GERENTE_II = D2.MATRICULA_GERENTE_II ");
                        queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                        queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                        queryUp.Append("WHERE D2.MATRICULA_SUPERVISOR <> '0' ");
                        SqlCommand createUp5 = new SqlCommand(queryUp.ToString(), connection);
                        createUp5.CommandTimeout = 0;
                        createUp5.ExecuteNonQuery();

                        queryUp.Clear();
                        queryUp.Append("UPDATE D1 SET D1.NOME_GERENTE_II = D2.NOME_GERENTE_II ");
                        queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                        queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                        queryUp.Append("WHERE D2.MATRICULA_SUPERVISOR <> '0' ");
                        SqlCommand createUp6 = new SqlCommand(queryUp.ToString(), connection);
                        createUp6.CommandTimeout = 0;
                        createUp6.ExecuteNonQuery();

                        queryUp.Clear();
                        queryUp.Append("UPDATE D1 SET D1.MATRICULA_GERENTE_I = D2.MATRICULA_GERENTE_I ");
                        queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                        queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                        queryUp.Append("WHERE D2.MATRICULA_SUPERVISOR <> '0' ");
                        SqlCommand createUp7 = new SqlCommand(queryUp.ToString(), connection);
                        createUp7.CommandTimeout = 0;
                        createUp7.ExecuteNonQuery();

                        queryUp.Clear();
                        queryUp.Append("UPDATE D1 SET D1.NOME_GERENTE_I = D2.NOME_GERENTE_I ");
                        queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                        queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                        queryUp.Append("WHERE D2.MATRICULA_SUPERVISOR <> '0' ");
                        SqlCommand createUp8 = new SqlCommand(queryUp.ToString(), connection);
                        createUp8.CommandTimeout = 0;
                        createUp8.ExecuteNonQuery();

                        queryUp.Clear();
                        queryUp.Append("UPDATE D1 SET D1.MATRICULA_DIRETOR = D2.MATRICULA_DIRETOR ");
                        queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                        queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                        queryUp.Append("WHERE D2.MATRICULA_SUPERVISOR <> '0' ");
                        SqlCommand createUp9 = new SqlCommand(queryUp.ToString(), connection);
                        createUp9.CommandTimeout = 0;
                        createUp9.ExecuteNonQuery();

                        queryUp.Clear();
                        queryUp.Append("UPDATE D1 SET D1.NOME_DIRETOR = D2.NOME_DIRETOR ");
                        queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                        queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                        queryUp.Append("WHERE D2.MATRICULA_SUPERVISOR <> '0' ");
                        SqlCommand createUp10 = new SqlCommand(queryUp.ToString(), connection);
                        createUp10.CommandTimeout = 0;
                        createUp10.ExecuteNonQuery();

                        queryUp.Clear();
                        queryUp.Append("UPDATE D1 SET D1.MATRICULA_CEO = D2.MATRICULA_CEO ");
                        queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                        queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                        queryUp.Append("WHERE D2.MATRICULA_SUPERVISOR <> '0' ");
                        SqlCommand createUp11 = new SqlCommand(queryUp.ToString(), connection);
                        createUp11.CommandTimeout = 0;
                        createUp11.ExecuteNonQuery();

                        queryUp.Clear();
                        queryUp.Append("UPDATE D1 SET D1.NOME_CEO = D2.NOME_CEO ");
                        queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                        queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                        queryUp.Append("WHERE D2.MATRICULA_SUPERVISOR <> '0' ");
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
                    }





                    //A
                    StringBuilder queryInsertResult2 = new StringBuilder();
                    queryInsertResult2.Append("MERGE INTO GDA_COLLABORATORS_DETAILS AS TARGET  ");
                    queryInsertResult2.Append("USING #TEMPAG AS SOURCE  ");
                    queryInsertResult2.Append("ON (TARGET.IDGDA_COLLABORATORS = SOURCE.IDGDA_COLLABORATORS AND TARGET.CREATED_AT = SOURCE.CREATED_AT)  ");
                    queryInsertResult2.Append("WHEN NOT MATCHED BY TARGET THEN  ");
                    queryInsertResult2.Append("  INSERT (IDGDA_COLLABORATORS, CARGO, CREATED_AT, IDGDA_SECTOR, IDGDA_SECTOR_SUPERVISOR, IDGDA_SUBSECTOR, HOME_BASED, SITE, PERIODO, MATRICULA_SUPERVISOR, NOME_SUPERVISOR, MATRICULA_COORDENADOR,  ");
                    queryInsertResult2.Append("		  NOME_COORDENADOR, MATRICULA_GERENTE_II, NOME_GERENTE_II, MATRICULA_GERENTE_I, NOME_GERENTE_I, MATRICULA_DIRETOR, NOME_DIRETOR, MATRICULA_CEO, NOME_CEO, ACTIVE)  ");
                    queryInsertResult2.Append("  VALUES (SOURCE.IDGDA_COLLABORATORS, SOURCE.CARGO, SOURCE.CREATED_AT, SOURCE.IDGDA_SECTOR, SOURCE.IDGDA_SECTOR_SUPERVISOR, SOURCE.IDGDA_SUBSECTOR, SOURCE.HOME_BASED, SOURCE.SITE, SOURCE.PERIODO, SOURCE.MATRICULA_SUPERVISOR, SOURCE.NOME_SUPERVISOR, SOURCE.MATRICULA_COORDENADOR,  ");
                    queryInsertResult2.Append("		  SOURCE.NOME_COORDENADOR, SOURCE.MATRICULA_GERENTE_II, SOURCE.NOME_GERENTE_II, SOURCE.MATRICULA_GERENTE_I, SOURCE.NOME_GERENTE_I, SOURCE.MATRICULA_DIRETOR, SOURCE.NOME_DIRETOR, SOURCE.MATRICULA_CEO, SOURCE.NOME_CEO, SOURCE.ACTIVE)  ");
                    queryInsertResult2.Append("WHEN MATCHED THEN  ");
                    queryInsertResult2.Append("  UPDATE SET  ");
                    queryInsertResult2.Append("  TARGET.CARGO = SOURCE.CARGO, ");
                    queryInsertResult2.Append("  TARGET.IDGDA_SECTOR = SOURCE.IDGDA_SECTOR, ");
                    queryInsertResult2.Append("  TARGET.IDGDA_SECTOR_SUPERVISOR = SOURCE.IDGDA_SECTOR_SUPERVISOR, ");
                    queryInsertResult2.Append("  TARGET.IDGDA_SUBSECTOR = SOURCE.IDGDA_SUBSECTOR, ");
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
            //List<string> datas = getDateTransaction(transactionID);
            int diferencaDias = 0;

            List<string> datas = getDateTransaction(transactionID);

            DateTime dataAtual = DateTime.Now;

            try
            {
                List<DateTime> datas2 = datas.Select(s => DateTime.ParseExact(s, "yyyy-MM-dd", null)).ToList();

                DateTime dataMaisAntiga = datas2.Min();

                TimeSpan dd = dataAtual.Subtract(dataMaisAntiga);

                diferencaDias = dd.Days;

            }
            catch (Exception)
            {

            }

            if (diferencaDias == 0)
            {
                diferencaDias = 31;
            }

            for (int i = diferencaDias; i > 0; i--)
            {
                // Subtrai o número de dias da iteração da data atual
                DateTime data = dataAtual.AddDays(-i);

                // Formata a data no formato "yyyy-MM-dd" e adiciona à lista
                string dataFormatada = data.ToString("yyyy-MM-dd");
                datas.Add(dataFormatada);
            }

            //Varrer as datas retornadas
            foreach (string dt in datas)
            {

                List<MonetizationResultsModel> mrs = new List<MonetizationResultsModel>();

                using (SqlConnection connection = new SqlConnection(Database.Conn))
                {
                    connection.Open();
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
                        stb.Append("           WHEN MAX(ME.EXPRESSION) IS NULL THEN '(#FATOR1/#FATOR0)' ");
                        stb.Append("           ELSE MAX(ME.EXPRESSION) ");
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
                        //stb.Append("                  (SELECT SUM(INPUT) ");
                        //Deflator
                        stb.Append("                  (SELECT (SUM(INPUT) - SUM(OUTPUT)) AS INPUT ");
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
                        //stb.Append("LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CL ON CL.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
                        //stb.Append("AND CL.CREATED_AT = R.CREATED_AT ");

                        stb.Append("LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CL ON CL.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
                        stb.Append("AND CL.CREATED_AT = R.CREATED_AT ");

                        //stb.Append("LEFT JOIN ( ");
                        //stb.Append("    SELECT  ");
                        //stb.Append("        CASE  ");
                        //stb.Append("            WHEN CL.IDGDA_SECTOR IS NOT NULL THEN CL.IDGDA_SECTOR  ");
                        //stb.Append("            ELSE CL2.IDGDA_SECTOR  ");
                        //stb.Append("        END AS IDGDA_SECTOR,  ");
                        //stb.Append("		 CASE  ");
                        //stb.Append("            WHEN CL.HOME_BASED != '' THEN CL.HOME_BASED  ");
                        //stb.Append("            ELSE CL2.HOME_BASED  ");
                        //stb.Append("        END AS HOME_BASED,  ");
                        //stb.Append("		CASE  ");
                        //stb.Append("            WHEN CL.CARGO IS NOT NULL THEN CL.CARGO  ");
                        //stb.Append("            ELSE CL2.CARGO  ");
                        //stb.Append("        END AS CARGO,  ");
                        //stb.Append("		CASE  ");
                        //stb.Append("            WHEN CL.ACTIVE IS NOT NULL THEN CL.ACTIVE  ");
                        //stb.Append("            ELSE CL2.ACTIVE  ");
                        //stb.Append("        END AS ACTIVE,  ");
                        //stb.Append("		CASE  ");
                        //stb.Append("            WHEN CL.SITE != '' THEN CL.SITE  ");
                        //stb.Append("            ELSE CL2.SITE  ");
                        //stb.Append("        END AS SITE,  ");
                        //stb.Append("		CASE  ");
                        //stb.Append("            WHEN CL.PERIODO != '' THEN CL.PERIODO  ");
                        //stb.Append("            ELSE CL2.PERIODO  ");
                        //stb.Append("        END AS PERIODO,  ");
                        //stb.Append("		CASE  ");
                        //stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_SUPERVISOR  ");
                        //stb.Append("            ELSE CL2.MATRICULA_SUPERVISOR  ");
                        //stb.Append("        END AS MATRICULA_SUPERVISOR,  ");
                        //stb.Append("		CASE  ");
                        //stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_SUPERVISOR  ");
                        //stb.Append("            ELSE CL2.NOME_SUPERVISOR  ");
                        //stb.Append("        END AS NOME_SUPERVISOR,  ");
                        //stb.Append("				CASE  ");
                        //stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_COORDENADOR  ");
                        //stb.Append("            ELSE CL2.MATRICULA_COORDENADOR  ");
                        //stb.Append("        END AS MATRICULA_COORDENADOR,  ");
                        //stb.Append("				CASE  ");
                        //stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_COORDENADOR  ");
                        //stb.Append("            ELSE CL2.NOME_COORDENADOR  ");
                        //stb.Append("        END AS NOME_COORDENADOR,  ");
                        //stb.Append("				CASE  ");
                        //stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_GERENTE_II  ");
                        //stb.Append("            ELSE CL2.MATRICULA_GERENTE_II  ");
                        //stb.Append("        END AS MATRICULA_GERENTE_II,  ");
                        //stb.Append("				CASE  ");
                        //stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_GERENTE_II  ");
                        //stb.Append("            ELSE CL2.NOME_GERENTE_II  ");
                        //stb.Append("        END AS NOME_GERENTE_II,  ");
                        //stb.Append("				CASE  ");
                        //stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_GERENTE_I  ");
                        //stb.Append("            ELSE CL2.MATRICULA_GERENTE_I  ");
                        //stb.Append("        END AS MATRICULA_GERENTE_I,  ");
                        //stb.Append("				CASE  ");
                        //stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_GERENTE_I  ");
                        //stb.Append("            ELSE CL2.NOME_GERENTE_I  ");
                        //stb.Append("        END AS NOME_GERENTE_I,  ");
                        //stb.Append("				CASE  ");
                        //stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_DIRETOR  ");
                        //stb.Append("            ELSE CL2.MATRICULA_DIRETOR  ");
                        //stb.Append("        END AS MATRICULA_DIRETOR,  ");
                        //stb.Append("				CASE  ");
                        //stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_DIRETOR  ");
                        //stb.Append("            ELSE CL2.NOME_DIRETOR  ");
                        //stb.Append("        END AS NOME_DIRETOR,  ");
                        //stb.Append("				CASE  ");
                        //stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.MATRICULA_CEO  ");
                        //stb.Append("            ELSE CL2.MATRICULA_CEO  ");
                        //stb.Append("        END AS MATRICULA_CEO,  ");
                        //stb.Append("				CASE  ");
                        //stb.Append("            WHEN CL.MATRICULA_SUPERVISOR != 0 THEN CL.NOME_CEO  ");
                        //stb.Append("            ELSE CL2.NOME_CEO  ");
                        //stb.Append("        END AS NOME_CEO,  ");
                        //stb.Append(" ");
                        //stb.Append("        C.IDGDA_COLLABORATORS, ");
                        //stb.Append("        CL.CREATED_AT ");
                        //stb.Append("    FROM GDA_COLLABORATORS (NOLOCK) AS C ");
                        //stb.Append("    LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CL  ");
                        //stb.Append("        ON CL.IDGDA_COLLABORATORS = C.IDGDA_COLLABORATORS AND CL.CREATED_AT = @DATAINICIAL ");
                        //stb.Append("    LEFT JOIN GDA_COLLABORATORS_LAST_DETAILS (NOLOCK) AS CL2  ");
                        //stb.Append("        ON CL2.IDGDA_COLLABORATORS = C.IDGDA_COLLABORATORS ");
                        //stb.Append(") AS CL  ");
                        //stb.Append("ON CL.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS AND CL.CREATED_AT = R.CREATED_AT ");


                        stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
                        stb.Append("AND HIG1.INDICATOR_ID = R.INDICADORID ");
                        stb.Append("AND HIG1.SECTOR_ID = CL.IDGDA_SECTOR ");
                        stb.Append("AND HIG1.GROUPID = 1 ");
                        stb.Append("AND CONVERT(DATE, HIG1.STARTED_AT) <= R.CREATED_AT AND CONVERT(DATE, HIG1.ENDED_AT) >= R.CREATED_AT ");
                        stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG2 ON HIG2.DELETED_AT IS NULL ");
                        stb.Append("AND HIG2.INDICATOR_ID = R.INDICADORID ");
                        stb.Append("AND HIG2.SECTOR_ID = CL.IDGDA_SECTOR ");
                        stb.Append("AND HIG2.GROUPID = 2 ");
                        stb.Append("AND CONVERT(DATE, HIG2.STARTED_AT) <= R.CREATED_AT AND CONVERT(DATE, HIG2.ENDED_AT) >= R.CREATED_AT ");
                        stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG3 ON HIG3.DELETED_AT IS NULL ");
                        stb.Append("AND HIG3.INDICATOR_ID = R.INDICADORID ");
                        stb.Append("AND HIG3.SECTOR_ID = CL.IDGDA_SECTOR ");
                        stb.Append("AND HIG3.GROUPID = 3 ");
                        stb.Append("AND CONVERT(DATE, HIG3.STARTED_AT) <= R.CREATED_AT AND CONVERT(DATE, HIG3.ENDED_AT) >= R.CREATED_AT ");
                        stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG4 ON HIG4.DELETED_AT IS NULL ");
                        stb.Append("AND HIG4.INDICATOR_ID = R.INDICADORID ");
                        stb.Append("AND HIG4.SECTOR_ID = CL.IDGDA_SECTOR ");
                        stb.Append("AND HIG4.GROUPID = 4 ");
                        stb.Append("AND CONVERT(DATE, HIG4.STARTED_AT) <= R.CREATED_AT AND CONVERT(DATE, HIG4.ENDED_AT) >= R.CREATED_AT ");


                        stb.Append("LEFT JOIN GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME ON HME.INDICATORID = R.INDICADORID AND HME.DELETED_AT IS NULL ");
                        stb.Append("LEFT JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON ME.ID = HME.MATHEMATICALEXPRESSIONID AND ME.DELETED_AT IS NULL  ");


                        stb.Append("LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS HHR ON HHR.idgda_COLLABORATORS = R.IDGDA_COLLABORATORS ");
                        stb.Append("AND HHR.DATE = @DATAINICIAL ");

                        stb.Append("INNER JOIN GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) AS HIS ");
                        stb.Append(" ON HIS.INDICATOR_ID = R.INDICADORID  ");
                        stb.Append("AND HIS.SECTOR_ID = CL.IDGDA_SECTOR ");
                        stb.Append("AND HIS.DELETED_AT IS NULL ");
                        stb.Append("AND CONVERT(DATE, HIS.STARTED_AT) <= R.CREATED_AT AND CONVERT(DATE, HIS.ENDED_AT) >= R.CREATED_AT ");

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
                        //stb.Append("UNION ALL ");
                        //stb.Append("SELECT R.*, ");
                        //stb.Append("       HIS.GOAL, ");
                        //stb.Append("       I.WEIGHT AS WEIGHT, ");
                        //stb.Append("       HHR.LEVELWEIGHT AS HIERARCHYLEVEL, ");
                        //stb.Append("       HIG1.MONETIZATION AS COIN1, ");
                        //stb.Append("       HIG2.MONETIZATION AS COIN2, ");
                        //stb.Append("       HIG3.MONETIZATION AS COIN3, ");
                        //stb.Append("       HIG4.MONETIZATION AS COIN4, ");
                        //stb.Append("       CL2.IDGDA_SECTOR, ");
                        //stb.Append("       MAX(I.TYPE) AS TYPE, ");
                        //stb.Append("       HIG1.METRIC_MIN AS MIN1, ");
                        //stb.Append("       HIG2.METRIC_MIN AS MIN2, ");
                        //stb.Append("       HIG3.METRIC_MIN AS MIN3, ");
                        //stb.Append("       HIG4.METRIC_MIN AS MIN4, ");
                        //stb.Append("       CASE ");
                        //stb.Append("           WHEN MAX(ME.EXPRESSION) IS NULL THEN '(#FATOR1/#FATOR0)' ");
                        //stb.Append("           ELSE MAX(ME.EXPRESSION) ");
                        //stb.Append("       END AS CONTA, ");
                        //stb.Append("       CASE ");
                        //stb.Append("           WHEN MAX(I.CALCULATION_TYPE) IS NULL THEN 'BIGGER_BETTER' ");
                        //stb.Append("           ELSE MAX(I.CALCULATION_TYPE) ");
                        //stb.Append("       END AS BETTER, ");
                        //stb.Append("       COALESCE( ");
                        //stb.Append("                  (SELECT TOP 1 BALANCE ");
                        //stb.Append("                   FROM GDA_CHECKING_ACCOUNT (NOLOCK) ");
                        //stb.Append("                   WHERE COLLABORATOR_ID = R.IDGDA_COLLABORATORS ");
                        //stb.Append("                   ORDER BY CREATED_AT DESC), 0) AS SALDO, ");
                        //stb.Append("       COALESCE( ");
                        //stb.Append("                  (SELECT SUM(INPUT) ");
                        //stb.Append("                   FROM GDA_CHECKING_ACCOUNT (NOLOCK) AS A ");
                        //stb.Append("                   WHERE A.COLLABORATOR_ID = R.IDGDA_COLLABORATORS ");
                        //stb.Append("                     AND A.RESULT_DATE = R.CREATED_AT ");
                        //stb.Append("                     AND GDA_INDICATOR_IDGDA_INDICATOR = R.INDICADORID ");
                        //stb.Append("                   GROUP BY A.COLLABORATOR_ID, A.RESULT_DATE, A.GDA_INDICATOR_IDGDA_INDICATOR), 0) AS COINS, ");
                        //stb.Append("       R.TRANSACTIONID, ");
                        //stb.Append("       MAX(CL2.MATRICULA_SUPERVISOR) AS 'MATRICULA SUPERVISOR', ");
                        //stb.Append("       MAX(CL2.NOME_SUPERVISOR) AS 'NOME SUPERVISOR', ");
                        //stb.Append("       MAX(CL2.MATRICULA_COORDENADOR) AS 'MATRICULA COORDENADOR', ");
                        //stb.Append("       MAX(CL2.NOME_COORDENADOR) AS 'NOME COORDENADOR', ");
                        //stb.Append("       MAX(CL2.MATRICULA_GERENTE_II) AS 'MATRICULA GERENTE II', ");
                        //stb.Append("       MAX(CL2.NOME_GERENTE_II) AS 'NOME GERENTE II', ");
                        //stb.Append("       MAX(CL2.MATRICULA_GERENTE_I) AS 'MATRICULA GERENTE I', ");
                        //stb.Append("       MAX(CL2.NOME_GERENTE_I) AS 'NOME GERENTE I', ");
                        //stb.Append("       MAX(CL2.MATRICULA_DIRETOR) AS 'MATRICULA DIRETOR', ");
                        //stb.Append("       MAX(CL2.NOME_DIRETOR) AS 'NOME DIRETOR', ");
                        //stb.Append("       MAX(CL2.MATRICULA_CEO) AS 'MATRICULA CEO', ");
                        //stb.Append("       MAX(CL2.NOME_CEO) AS 'NOME CEO' ");
                        //stb.Append("FROM GDA_RESULT (NOLOCK) AS R ");
                        //stb.Append("LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CL ON CL.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
                        //stb.Append("AND CL.CREATED_AT = R.CREATED_AT ");
                        //stb.Append("LEFT JOIN GDA_COLLABORATORS_LAST_DETAILS (NOLOCK) AS CL2 ON CL2.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
                        //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
                        //stb.Append("AND HIG1.INDICATOR_ID = R.INDICADORID ");
                        //stb.Append("AND HIG1.SECTOR_ID = CL2.IDGDA_SECTOR ");
                        //stb.Append("AND HIG1.GROUPID = 1 ");
                        //stb.Append("AND CONVERT(DATE, HIG1.STARTED_AT) <= R.CREATED_AT AND CONVERT(DATE, HIG1.ENDED_AT) >= R.CREATED_AT ");
                        //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG2 ON HIG2.DELETED_AT IS NULL ");
                        //stb.Append("AND HIG2.INDICATOR_ID = R.INDICADORID ");
                        //stb.Append("AND HIG2.SECTOR_ID = CL2.IDGDA_SECTOR ");
                        //stb.Append("AND HIG2.GROUPID = 2 ");
                        //stb.Append("AND CONVERT(DATE, HIG2.STARTED_AT) <= R.CREATED_AT AND CONVERT(DATE, HIG2.ENDED_AT) >= R.CREATED_AT ");
                        //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG3 ON HIG3.DELETED_AT IS NULL ");
                        //stb.Append("AND HIG3.INDICATOR_ID = R.INDICADORID ");
                        //stb.Append("AND HIG3.SECTOR_ID = CL2.IDGDA_SECTOR ");
                        //stb.Append("AND HIG3.GROUPID = 3 ");
                        //stb.Append("AND CONVERT(DATE, HIG3.STARTED_AT) <= R.CREATED_AT AND CONVERT(DATE, HIG3.ENDED_AT) >= R.CREATED_AT ");
                        //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG4 ON HIG4.DELETED_AT IS NULL ");
                        //stb.Append("AND HIG4.INDICATOR_ID = R.INDICADORID ");
                        //stb.Append("AND HIG4.SECTOR_ID = CL2.IDGDA_SECTOR ");
                        //stb.Append("AND HIG4.GROUPID = 4 ");
                        //stb.Append("AND CONVERT(DATE, HIG4.STARTED_AT) <= R.CREATED_AT AND CONVERT(DATE, HIG4.ENDED_AT) >= R.CREATED_AT ");

                        //stb.Append("LEFT JOIN GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME ON HME.INDICATORID = R.INDICADORID AND HME.DELETED_AT IS NULL ");
                        //stb.Append("LEFT JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON ME.ID = HME.MATHEMATICALEXPRESSIONID AND ME.DELETED_AT IS NULL  ");

                        //stb.Append("LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS HHR ON HHR.idgda_COLLABORATORS = R.IDGDA_COLLABORATORS ");
                        //stb.Append("AND HHR.DATE = @DATAINICIAL ");
                        //stb.Append("INNER JOIN GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) AS HIS ");
                        //stb.Append(" ON HIS.INDICATOR_ID = R.INDICADORID  ");
                        //stb.Append("AND HIS.SECTOR_ID = CL2.IDGDA_SECTOR ");
                        //stb.Append("AND HIS.DELETED_AT IS NULL ");
                        //stb.Append("AND CONVERT(DATE, HIS.STARTED_AT) <= R.CREATED_AT AND CONVERT(DATE, HIS.ENDED_AT) >= R.CREATED_AT ");

                        //stb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS I ON I.IDGDA_INDICATOR = R.INDICADORID ");
                        //stb.Append("WHERE R.CREATED_AT = @DATAINICIAL ");
                        //stb.Append("  AND HIG1.MONETIZATION > 0 ");
                        //stb.Append("  AND R.DELETED_AT IS NULL ");
                        //stb.Append("  AND (CL.IDGDA_SECTOR IS NULL OR CL.ACTIVE IS NULL) AND CL2.ACTIVE = 'true' ");
                        //stb.Append("GROUP BY IDGDA_RESULT, ");
                        //stb.Append("         INDICADORID, ");
                        //stb.Append("         R.TRANSACTIONID, ");
                        //stb.Append("         RESULT, ");
                        //stb.Append("         R.CREATED_AT, ");
                        //stb.Append("         R.IDGDA_COLLABORATORS, ");
                        //stb.Append("         FACTORS, ");
                        //stb.Append("         R.DELETED_AT, ");
                        //stb.Append("         CL2.IDGDA_SECTOR, ");
                        //stb.Append("         HIG1.METRIC_MIN, ");
                        //stb.Append("         HIG2.METRIC_MIN, ");
                        //stb.Append("         HIG3.METRIC_MIN, ");
                        //stb.Append("         HIG4.METRIC_MIN, ");
                        //stb.Append("         HIS.GOAL, ");
                        //stb.Append("         I.WEIGHT, ");
                        //stb.Append("         HHR.LEVELWEIGHT, ");
                        //stb.Append("         HIG1.MONETIZATION, ");
                        //stb.Append("         HIG2.MONETIZATION, ");
                        //stb.Append("         HIG3.MONETIZATION, ");
                        //stb.Append("         HIG4.MONETIZATION ");

                        #region Antigo
                        //stb.AppendFormat("DECLARE @DATAINICIAL AS DATE; SET @DATAINICIAL = '{0}'; ", dt);
                        //stb.Append("SELECT R.*,HIS.GOAL, I.WEIGHT AS WEIGHT, HHR.LEVELWEIGHT AS HIERARCHYLEVEL, HIG1.MONETIZATION AS COIN1, HIG2.MONETIZATION AS COIN2, HIG3.MONETIZATION AS COIN3,  ");
                        //stb.Append("HIG4.MONETIZATION AS COIN4, S.IDGDA_SECTOR, MAX(I.TYPE) AS TYPE, HIG1.METRIC_MIN AS MIN1, HIG2.METRIC_MIN AS MIN2, HIG3.METRIC_MIN AS MIN3, HIG4.METRIC_MIN AS MIN4, ");
                        //stb.Append("CASE WHEN MAX(TBL.EXPRESSION) IS NULL THEN '(#FATOR1/#FATOR0)' ELSE MAX(TBL.EXPRESSION) END AS CONTA, ");
                        //stb.Append("CASE WHEN MAX(I.CALCULATION_TYPE) IS NULL THEN 'BIGGER_BETTER' ELSE MAX(I.CALCULATION_TYPE) END AS BETTER, ");
                        //stb.Append("COALESCE((SELECT TOP 1 BALANCE FROM GDA_CHECKING_ACCOUNT (NOLOCK) WHERE COLLABORATOR_ID = R.IDGDA_COLLABORATORS ORDER BY CREATED_AT DESC), 0) AS SALDO, ");
                        //stb.Append("COALESCE((SELECT SUM(INPUT) FROM GDA_CHECKING_ACCOUNT (NOLOCK) AS A WHERE A.COLLABORATOR_ID = R.IDGDA_COLLABORATORS	AND A.RESULT_DATE = R.CREATED_AT AND GDA_INDICATOR_IDGDA_INDICATOR = R.INDICADORID GROUP BY A.COLLABORATOR_ID, A.RESULT_DATE, A.GDA_INDICATOR_IDGDA_INDICATOR), 0) AS COINS, ");
                        //stb.Append("R.TRANSACTIONID, ");
                        //stb.Append("MAX(CASE ");
                        //stb.Append("        WHEN HIERARCHY.LEVELWEIGHT = '2' THEN CAST(HIERARCHY.PARENTIDENTIFICATION AS VARCHAR(255)) ");
                        //stb.Append("        ELSE '-' ");
                        //stb.Append("    END) AS 'MATRICULA SUPERVISOR', ");
                        //stb.Append("MAX(CASE ");
                        //stb.Append("        WHEN HIERARCHY.LEVELWEIGHT = '2' THEN HIERARCHY.NAME ");
                        //stb.Append("        ELSE '-' ");
                        //stb.Append("    END) AS 'NOME SUPERVISOR', ");
                        //stb.Append("MAX(CASE ");
                        //stb.Append("        WHEN HIERARCHY.LEVELWEIGHT = '3' THEN CAST(HIERARCHY.PARENTIDENTIFICATION AS VARCHAR(255)) ");
                        //stb.Append("        ELSE '-' ");
                        //stb.Append("    END) AS 'MATRICULA COORDENADOR', ");
                        //stb.Append("MAX(CASE ");
                        //stb.Append("        WHEN HIERARCHY.LEVELWEIGHT = '3' THEN HIERARCHY.NAME ");
                        //stb.Append("        ELSE '-' ");
                        //stb.Append("    END) AS 'NOME COORDENADOR', ");
                        //stb.Append("MAX(CASE ");
                        //stb.Append("        WHEN HIERARCHY.LEVELWEIGHT = '4' THEN CAST(HIERARCHY.PARENTIDENTIFICATION AS VARCHAR(255)) ");
                        //stb.Append("        ELSE '-' ");
                        //stb.Append("    END) AS 'MATRICULA GERENTE II', ");
                        //stb.Append("MAX(CASE ");
                        //stb.Append("        WHEN HIERARCHY.LEVELWEIGHT = '4' THEN HIERARCHY.NAME ");
                        //stb.Append("        ELSE '-' ");
                        //stb.Append("    END) AS 'NOME GERENTE II', ");
                        //stb.Append("MAX(CASE ");
                        //stb.Append("        WHEN HIERARCHY.LEVELWEIGHT = '5' THEN CAST(HIERARCHY.PARENTIDENTIFICATION AS VARCHAR(255)) ");
                        //stb.Append("        ELSE '-' ");
                        //stb.Append("    END) AS 'MATRICULA GERENTE I', ");
                        //stb.Append("MAX(CASE ");
                        //stb.Append("        WHEN HIERARCHY.LEVELWEIGHT = '5' THEN HIERARCHY.NAME ");
                        //stb.Append("        ELSE '-' ");
                        //stb.Append("    END) AS 'NOME GERENTE I', ");
                        //stb.Append("MAX(CASE ");
                        //stb.Append("        WHEN HIERARCHY.LEVELWEIGHT = '6' THEN CAST(HIERARCHY.PARENTIDENTIFICATION AS VARCHAR(255)) ");
                        //stb.Append("        ELSE '-' ");
                        //stb.Append("    END) AS 'MATRICULA DIRETOR', ");
                        //stb.Append("MAX(CASE ");
                        //stb.Append("        WHEN HIERARCHY.LEVELWEIGHT = '6' THEN HIERARCHY.NAME ");
                        //stb.Append("        ELSE '-' ");
                        //stb.Append("    END) AS 'NOME DIRETOR', ");
                        //stb.Append("MAX(CASE ");
                        //stb.Append("        WHEN HIERARCHY.LEVELWEIGHT = '7' THEN HIERARCHY.PARENTIDENTIFICATION ");
                        //stb.Append("        ELSE '-' ");
                        //stb.Append("    END) AS 'MATRICULA CEO', ");
                        //stb.Append("MAX(CASE ");
                        //stb.Append("        WHEN HIERARCHY.LEVELWEIGHT = '7' THEN HIERARCHY.NAME ");
                        //stb.Append("        ELSE '-' ");
                        //stb.Append("    END) AS 'NOME CEO' ");



                        //stb.Append("FROM GDA_RESULT (NOLOCK) AS R ");
                        //stb.Append("INNER JOIN GDA_HISTORY_COLLABORATOR_SECTOR (NOLOCK) AS S ON S.CREATED_AT = R.CREATED_AT AND S.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
                        ////stb.Append("LEFT JOIN ( ");
                        ////stb.Append("    SELECT SECTOR_ID, INDICATOR_ID ");
                        ////stb.Append("    FROM GDA_HISTORY_INDICATOR_GROUP ");
                        ////stb.Append("    WHERE MONETIZATION > 0 AND DELETED_AT IS NULL ");
                        ////stb.Append(") TS ON TS.SECTOR_ID = S.IDGDA_SECTOR AND TS.INDICATOR_ID = R.INDICADORID ");
                        //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL AND HIG1.INDICATOR_ID = R.INDICADORID AND HIG1.SECTOR_ID = S.IDGDA_SECTOR AND HIG1.GROUPID = 1 ");
                        //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG2 ON HIG2.DELETED_AT IS NULL AND HIG2.INDICATOR_ID = R.INDICADORID AND HIG2.SECTOR_ID = S.IDGDA_SECTOR AND HIG2.GROUPID = 2 ");
                        //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG3 ON HIG3.DELETED_AT IS NULL AND HIG3.INDICATOR_ID = R.INDICADORID AND HIG3.SECTOR_ID = S.IDGDA_SECTOR AND HIG3.GROUPID = 3 ");
                        //stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG4 ON HIG4.DELETED_AT IS NULL AND HIG4.INDICATOR_ID = R.INDICADORID AND HIG4.SECTOR_ID = S.IDGDA_SECTOR AND HIG4.GROUPID = 4 ");
                        //stb.Append("LEFT JOIN (SELECT HME.INDICATORID, ME.EXPRESSION FROM GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME ");
                        //stb.Append("INNER JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON ME.ID = HME.MATHEMATICALEXPRESSIONID ");
                        //stb.Append("WHERE HME.DELETED_AT IS NULL) AS TBL ON TBL.INDICATORID = R.INDICADORID ");
                        //stb.AppendFormat("LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS HHR ON HHR.idgda_COLLABORATORS = R.IDGDA_COLLABORATORS AND HHR.DATE = '{0}' ", dt);
                        //stb.Append("INNER JOIN ( ");
                        //stb.Append("SELECT GOAL, INDICATOR_ID, SECTOR_ID, ROW_NUMBER() OVER (PARTITION BY INDICATOR_ID, SECTOR_ID ORDER BY CREATED_AT DESC) AS RN ");
                        //stb.Append("FROM GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) ");
                        //stb.Append("WHERE DELETED_AT IS NULL ");
                        //stb.Append(") AS HIS ON HIS.INDICATOR_ID = R.INDICADORID AND HIS.SECTOR_ID = S.IDGDA_SECTOR AND HIS.RN = 1 ");
                        //stb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS I ON I.IDGDA_INDICATOR = R.INDICADORID ");
                        //stb.Append("LEFT JOIN ");
                        //stb.Append("  (SELECT COD, ");
                        //stb.Append("          IDGDA_COLLABORATORS, ");
                        //stb.Append("          PARENTIDENTIFICATION, ");
                        //stb.Append("          NAME, ");
                        //stb.Append("          LEVELWEIGHT ");
                        //stb.Append("   FROM ");
                        //stb.Append("     (SELECT LV1.IDGDA_COLLABORATORS AS COD, ");
                        //stb.Append("             LV1.IDGDA_COLLABORATORS, ");
                        //stb.Append("             ISNULL(LV1.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION, ");
                        //stb.Append("             C.NAME, ");
                        //stb.Append("             CASE ");
                        //stb.Append("                 WHEN LV2.LEVELWEIGHT IS NULL ");
                        //stb.Append("                      AND LV1.PARENTIDENTIFICATION IS NOT NULL THEN '7' ");
                        //stb.Append("                 ELSE LV2.LEVELWEIGHT ");
                        //stb.Append("             END AS LEVELWEIGHT ");
                        //stb.Append("      FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1 ");
                        //stb.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS ");
                        //stb.Append("      AND LV2.DATE = @DATAINICIAL ");
                        //stb.Append("      LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV1.PARENTIDENTIFICATION ");
                        //stb.Append("      WHERE LV1.DATE = @DATAINICIAL ");
                        //stb.Append("      UNION ALL SELECT LV1.IDGDA_COLLABORATORS AS COD, ");
                        //stb.Append("                       LV2.IDGDA_COLLABORATORS, ");
                        //stb.Append("                       ISNULL(LV2.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION, ");
                        //stb.Append("                       C.NAME, ");
                        //stb.Append("                       CASE ");
                        //stb.Append("                           WHEN LV3.LEVELWEIGHT IS NULL ");
                        //stb.Append("                                AND LV2.PARENTIDENTIFICATION IS NOT NULL THEN '7' ");
                        //stb.Append("                           ELSE LV3.LEVELWEIGHT ");
                        //stb.Append("                       END AS LEVELWEIGHT ");
                        //stb.Append("      FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1 ");
                        //stb.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS ");
                        //stb.Append("      AND LV2.DATE = @DATAINICIAL ");
                        //stb.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS ");
                        //stb.Append("      AND LV3.DATE = @DATAINICIAL ");
                        //stb.Append("      LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV2.PARENTIDENTIFICATION ");
                        //stb.Append("      WHERE LV1.DATE = @DATAINICIAL ");
                        //stb.Append("      UNION ALL SELECT LV1.IDGDA_COLLABORATORS AS COD, ");
                        //stb.Append("                       LV3.IDGDA_COLLABORATORS, ");
                        //stb.Append("                       ISNULL(LV3.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION, ");
                        //stb.Append("                       C.NAME, ");
                        //stb.Append("                       CASE ");
                        //stb.Append("                           WHEN LV4.LEVELWEIGHT IS NULL ");
                        //stb.Append("                                AND LV3.PARENTIDENTIFICATION IS NOT NULL THEN '7' ");
                        //stb.Append("                           ELSE LV4.LEVELWEIGHT ");
                        //stb.Append("                       END AS LEVELWEIGHT ");
                        //stb.Append("      FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1 ");
                        //stb.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS ");
                        //stb.Append("      AND LV2.DATE = @DATAINICIAL ");
                        //stb.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS ");
                        //stb.Append("      AND LV3.DATE = @DATAINICIAL ");
                        //stb.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV4 ON LV3.PARENTIDENTIFICATION = LV4.IDGDA_COLLABORATORS ");
                        //stb.Append("      AND LV4.DATE = @DATAINICIAL ");
                        //stb.Append("      LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV3.PARENTIDENTIFICATION ");
                        //stb.Append("      WHERE LV1.DATE = @DATAINICIAL ");
                        //stb.Append("      UNION ALL SELECT LV1.IDGDA_COLLABORATORS AS COD, ");
                        //stb.Append("                       LV4.IDGDA_COLLABORATORS, ");
                        //stb.Append("                       ISNULL(LV4.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION, ");
                        //stb.Append("                       C.NAME, ");
                        //stb.Append("                       CASE ");
                        //stb.Append("                           WHEN LV5.LEVELWEIGHT IS NULL ");
                        //stb.Append("                                AND LV4.PARENTIDENTIFICATION IS NOT NULL THEN '7' ");
                        //stb.Append("                           ELSE LV5.LEVELWEIGHT ");
                        //stb.Append("                       END AS LEVELWEIGHT ");
                        //stb.Append("      FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1 ");
                        //stb.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS ");
                        //stb.Append("      AND LV2.DATE = @DATAINICIAL ");
                        //stb.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS ");
                        //stb.Append("      AND LV3.DATE = @DATAINICIAL ");
                        //stb.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV4 ON LV3.PARENTIDENTIFICATION = LV4.IDGDA_COLLABORATORS ");
                        //stb.Append("      AND LV4.DATE = @DATAINICIAL ");
                        //stb.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV5 ON LV4.PARENTIDENTIFICATION = LV5.IDGDA_COLLABORATORS ");
                        //stb.Append("      AND LV5.DATE = @DATAINICIAL ");
                        //stb.Append("      LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV4.PARENTIDENTIFICATION ");
                        //stb.Append("      WHERE LV1.DATE = @DATAINICIAL ");
                        //stb.Append("      UNION ALL SELECT LV1.IDGDA_COLLABORATORS AS COD, ");
                        //stb.Append("                       LV5.IDGDA_COLLABORATORS, ");
                        //stb.Append("                       ISNULL(LV5.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION, ");
                        //stb.Append("                       C.NAME, ");
                        //stb.Append("                       CASE ");
                        //stb.Append("                           WHEN LV6.LEVELWEIGHT IS NULL ");
                        //stb.Append("                                AND LV5.PARENTIDENTIFICATION IS NOT NULL THEN '7' ");
                        //stb.Append("                           ELSE LV6.LEVELWEIGHT ");
                        //stb.Append("                       END AS LEVELWEIGHT ");
                        //stb.Append("      FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1 ");
                        //stb.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS ");
                        //stb.Append("      AND LV2.DATE = @DATAINICIAL ");
                        //stb.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS ");
                        //stb.Append("      AND LV3.DATE = @DATAINICIAL ");
                        //stb.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV4 ON LV3.PARENTIDENTIFICATION = LV4.IDGDA_COLLABORATORS ");
                        //stb.Append("      AND LV4.DATE = @DATAINICIAL ");
                        //stb.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV5 ON LV4.PARENTIDENTIFICATION = LV5.IDGDA_COLLABORATORS ");
                        //stb.Append("      AND LV5.DATE = @DATAINICIAL ");
                        //stb.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV6 ON LV5.PARENTIDENTIFICATION = LV6.IDGDA_COLLABORATORS ");
                        //stb.Append("      AND LV6.DATE = @DATAINICIAL ");
                        //stb.Append("      LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV5.PARENTIDENTIFICATION ");
                        //stb.Append("      WHERE LV1.DATE = @DATAINICIAL ");
                        //stb.Append("      UNION ALL SELECT LV1.IDGDA_COLLABORATORS AS COD, ");
                        //stb.Append("                       LV6.IDGDA_COLLABORATORS, ");
                        //stb.Append("                       ISNULL(LV6.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION, ");
                        //stb.Append("                       C.NAME, ");
                        //stb.Append("                       CASE ");
                        //stb.Append("                           WHEN LV7.LEVELWEIGHT IS NULL ");
                        //stb.Append("                                AND LV6.PARENTIDENTIFICATION IS NOT NULL THEN '7' ");
                        //stb.Append("                           ELSE LV7.LEVELWEIGHT ");
                        //stb.Append("                       END AS LEVELWEIGHT ");
                        //stb.Append("      FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1 ");
                        //stb.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS ");
                        //stb.Append("      AND LV2.DATE = @DATAINICIAL ");
                        //stb.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS ");
                        //stb.Append("      AND LV3.DATE = @DATAINICIAL ");
                        //stb.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV4 ON LV3.PARENTIDENTIFICATION = LV4.IDGDA_COLLABORATORS ");
                        //stb.Append("      AND LV4.DATE = @DATAINICIAL ");
                        //stb.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV5 ON LV4.PARENTIDENTIFICATION = LV5.IDGDA_COLLABORATORS ");
                        //stb.Append("      AND LV5.DATE = @DATAINICIAL ");
                        //stb.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV6 ON LV5.PARENTIDENTIFICATION = LV6.IDGDA_COLLABORATORS ");
                        //stb.Append("      AND LV6.DATE = @DATAINICIAL ");
                        //stb.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV7 ON LV6.PARENTIDENTIFICATION = LV7.IDGDA_COLLABORATORS ");
                        //stb.Append("      AND LV7.DATE = @DATAINICIAL ");
                        //stb.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV8 ON LV7.PARENTIDENTIFICATION = LV8.IDGDA_COLLABORATORS ");
                        //stb.Append("      AND LV8.DATE = @DATAINICIAL ");
                        //stb.Append("      LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV6.PARENTIDENTIFICATION ");
                        //stb.Append("      WHERE LV1.DATE = @DATAINICIAL ) AS CombinedData) AS HIERARCHY ON HIERARCHY.COD = R.IDGDA_COLLABORATORS ");
                        //stb.AppendFormat("WHERE R.CREATED_AT = @DATAINICIAL AND HIG1.MONETIZATION > 0 AND R.DELETED_AT IS NULL  ", transactionID); //TS.SECTOR_ID IS NOT NULL
                        //stb.Append("GROUP BY IDGDA_RESULT, INDICADORID, R.TRANSACTIONID, RESULT, R.CREATED_AT, R.IDGDA_COLLABORATORS, FACTORS, R.DELETED_AT, S.IDGDA_SECTOR, HIG1.METRIC_MIN,  ");
                        //stb.Append("HIG2.METRIC_MIN, HIG3.METRIC_MIN, HIG4.METRIC_MIN,HIS.GOAL, I.WEIGHT, HHR.LEVELWEIGHT, HIG1.MONETIZATION, HIG2.MONETIZATION, HIG3.MONETIZATION, HIG4.MONETIZATION ");

                        #endregion

                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            command.CommandTimeout = 60;
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
                            catch (Exception)
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
                                doMonetizationHierarchyNew(transactionID, dt, mr.matriculaSupervisor, mr);
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
                                doMonetizationHierarchyNew(transactionID, dt, mr.matriculaCoordenador, mr);
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
                            try
                            {
                                doMonetizationHierarchyNew(transactionID, dt, mr.matriculaGerenteii, mr);
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
                                doMonetizationHierarchyNew(transactionID, dt, mr.matriculaGerentei, mr);
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
                                doMonetizationHierarchyNew(transactionID, dt, mr.matriculaDiretor, mr);
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
                                doMonetizationHierarchyNew(transactionID, dt, mr.matriculaCeo, mr);
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


        public List<string> getDateTransaction(int transactionID)
        {


            List<string> datas = new List<string>();

            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                connection.Open();
                try
                {
                    StringBuilder stb = new StringBuilder();
                    stb.Append("SELECT DISTINCT(CONVERT(varchar, DATE, 23)) AS CREATED_AT FROM GDA_HISTORY_LAST_RESULT (NOLOCK) ");
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

        public void doMonetizationHierarchyNew(int transactionID, string dateM, string idReferer, MonetizationResultsModel mr)
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
                        stbConsolChecking.Append("IDGDA_RESULT, RESULT_DATE, WEIGHT, TRANSACTIONID) VALUES (");
                        stbConsolChecking.AppendFormat("'{0}', ", valorMonetizar.ToString()); //MONETIZATION
                        stbConsolChecking.AppendFormat("'{0}', ", mr.idCheckingAccount); //GDA_CHECKING_ACCOUNT_IDGDA_CHECKING_ACCOUNT
                        stbConsolChecking.AppendFormat("'{0}', ", mr.hierarchyLevel); //GDA_HIERARCHY_IDGDA_HIERARCHY
                        stbConsolChecking.AppendFormat("'{0}', ", mr.idIndicator); //GDA_INDICATOR_IDGDA_INDICATOR
                        stbConsolChecking.AppendFormat("'{0}', ", mr.idSector); //GDA_SECTOR_IDGDA_SECTOR
                        stbConsolChecking.AppendFormat("{0}, ", "GETDATE()"); //CREATED_AT
                        stbConsolChecking.AppendFormat("{0}, ", "NULL"); //DELETED_AT
                        stbConsolChecking.AppendFormat("'{0}', ", mr.idResult); //IDGDA_RESULT
                        stbConsolChecking.AppendFormat("'{0}', ", dateM); //RESULT_DATE
                        stbConsolChecking.AppendFormat("'{0}', ", mr.indicatorWeight); //WEIGHT
                        stbConsolChecking.AppendFormat("'{0}' ", transactionID); //transactionID
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


        public void doMonetizationHierarchy(int transactionID, string dateM, MonetizationResultsModel mr)
        {
            //Pega Hierarquia
            List<HierarchiesIdModel> him = getHierarchyById(dateM, mr.idCollaborator);
            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                connection.Open();
                try
                {


                    //Varre as Hierarquias
                    foreach (var item in him)
                    {
                        //Pega os Setores
                        //List<string> sets = getSectorsByCollaborators(dateM, item.parentId);

                        //Varre os setores
                        //foreach (var set in sets)
                        //{

                        //Select para pegar as monetizações
                        List<MonetizationHierarchy> mons = getMonetizationHierarchy(dateM, item.parentId, mr.idIndicator, mr.idSector);

                        //Fazer media x Comparação
                        if (mons.Count == 0)
                        {
                            continue;
                        }
                        double quantidadeHierarquia = mons[0].Quantidade;
                        var somaHierarquia = mons[0].Soma;
                        var quantidadeReferencia = mons[1].Quantidade;
                        double somaReferencia = mons[1].Soma;
                        double valorMonetizar = 0;


                        var ultimoSaldo = 0;
                        try
                        {
                            ultimoSaldo = mons[2].Soma;
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
                            stbCheckingAccount.AppendFormat("'{0}',", item.parentId); //COLLABORATOR_ID
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
                            stbConsolChecking.AppendFormat("'{0}', ", item.level); //GDA_HIERARCHY_IDGDA_HIERARCHY
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


                }
                catch (Exception ex)
                {

                    throw;
                }
                connection.Close();
            }
        }

        public void doBasketMonetization(string dateM)
        {
            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                connection.Open();
                try
                {

                    StringBuilder stbBacket = new StringBuilder();
                    stbBacket.Append("INSERT INTO GDA_BASKET_INDICATOR (SECTOR_ID, INDICATOR_ID, DATE, WEIGHT, CREATED_AT, MONETIZATION_G1) ");
                    stbBacket.AppendFormat("SELECT HIG.SECTOR_ID, HIG.INDICATOR_ID, '{0}', GI.WEIGHT, GETDATE(), MONETIZATION FROM GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG ", dateM);
                    stbBacket.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS GI ON GI.IDGDA_INDICATOR = HIG.INDICATOR_ID ");
                    stbBacket.AppendFormat("LEFT JOIN GDA_BASKET_INDICATOR (NOLOCK) AS BK ON BK.SECTOR_ID = HIG.SECTOR_ID AND BK.INDICATOR_ID = HIG.INDICATOR_ID AND BK.DATE = '{0}' ", dateM);
                    stbBacket.Append("WHERE HIG.DELETED_AT IS NULL ");
                    stbBacket.Append("AND GROUPID = 1 AND MONETIZATION > 0 AND HIG.INDICATOR_ID <> 10000012 ");
                    stbBacket.Append("AND BK.SECTOR_ID IS NULL AND BK.INDICATOR_ID IS NULL AND GI.DELETED_AT IS NULL ");
                    stbBacket.Append("GROUP BY HIG.SECTOR_ID, HIG.INDICATOR_ID, GI.WEIGHT, MONETIZATION ");

                    SqlCommand insertBasketCommand = new SqlCommand(stbBacket.ToString(), connection);
                    insertBasketCommand.ExecuteNonQuery();

                }
                catch (Exception)
                {

                    throw;
                }
                connection.Close();
            }
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
                    DataTable dt = new DataTable();
                    double resultado = 0;

                    if (mr.fator0.ToString() == "0" && mr.fator1.ToString() == "0")
                    {
                        return 0;
                    }
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

                    //Regra do TMA. Quando o resultado for 0, não monetizar e considerar bronze
                    if (resultado == 0 && (mr.idIndicator == "191" || mr.idIndicator == "371"))
                    {
                        return 0;
                    }
                    //Regra do TMA. Quando o arredondamento tambem der 0, não monetizar e considerar bronze
                    double arredondResult = Math.Round(resultado, 0, MidpointRounding.AwayFromZero);
                    if (arredondResult == 0 && (mr.idIndicator == "191" || mr.idIndicator == "371"))
                    {
                        return 0;
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
                    double monRetiradaDia = 0;

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
                    //string setorMonetizado = "";
                    stb.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}'; ", mr.idCollaborator);
                    stb.AppendFormat("DECLARE @DATEENV DATE; SET @DATEENV = '{0}'; ", dateM);
                    stb.AppendFormat("DECLARE @INDICADORID VARCHAR(MAX); SET @INDICADORID = '{0}'; ", mr.idIndicator);
                    //stb.Append("SELECT CASE WHEN SUM(INPUT) IS NULL THEN 0 ELSE SUM(INPUT) END AS SOMA, MAX(IDGDA_SECTOR) AS IDGDA_SECTOR ");
                    //Deflator
                    stb.Append("SELECT CASE WHEN SUM(INPUT) IS NULL THEN 0 ELSE SUM(INPUT) END AS SOMA, CASE WHEN SUM(OUTPUT) IS NULL THEN 0 ELSE SUM(OUTPUT) END AS SOMA_OUT, MAX(IDGDA_SECTOR) AS IDGDA_SECTOR ");
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
                                //Deflator
                                monRetiradaDia = double.Parse(reader["SOMA_OUT"].ToString());
                                //setorMonetizado = reader["IDGDA_SECTOR"].ToString();
                            }
                        }
                    }

                    //if (setorMonetizado != "" && setorMonetizado != mr.idSector)
                    //{
                    //    return monetizacaoDia;
                    //}

                    //Deflator
                    monRetiradaDia = monRetiradaDia * (-1);

                    //Monetizar apenas o que ainda não foi monetizado. E apenas a diferença para cima
                    //Deflator
                    if (inputMoedas > monetizacaoDia || outptMoedas < monRetiradaDia)
                    //if (inputMoedas > monetizacaoDia)
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

                        //Deflator
                        if (inputMoedas > 0)
                        {
                            balanceEnc = balanceEnc + inputMoedas;

                            double conta = inputMoedas - monetizacaoDia;
                            inputMoedas = conta;

                            retorno = inputMoedas;
                        }
                        else
                        {
                            balanceEnc = balanceEnc + outptMoedas;

                            double conta = outptMoedas - monRetiradaDia;
                            outptMoedas = conta * (-1);

                            retorno = conta;
                        }


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
                        stbConsolChecking.Append("IDGDA_RESULT, RESULT_DATE, WEIGHT, TRANSACTIONID) VALUES (");
                        stbConsolChecking.AppendFormat("'{0}', ", moedas.ToString()); //MONETIZATION
                        stbConsolChecking.AppendFormat("'{0}', ", mr.idCheckingAccount); //GDA_CHECKING_ACCOUNT_IDGDA_CHECKING_ACCOUNT
                        stbConsolChecking.AppendFormat("'{0}', ", mr.hierarchyLevel); //GDA_HIERARCHY_IDGDA_HIERARCHY
                        stbConsolChecking.AppendFormat("'{0}', ", mr.idIndicator); //GDA_INDICATOR_IDGDA_INDICATOR
                        stbConsolChecking.AppendFormat("'{0}', ", mr.idSector); //GDA_SECTOR_IDGDA_SECTOR
                        stbConsolChecking.AppendFormat("{0}, ", "GETDATE()"); //CREATED_AT
                        stbConsolChecking.AppendFormat("{0}, ", "NULL"); //DELETED_AT
                        stbConsolChecking.AppendFormat("'{0}', ", mr.idResult); //IDGDA_RESULT
                        stbConsolChecking.AppendFormat("'{0}', ", dateM); //RESULT_DATE
                        stbConsolChecking.AppendFormat("'{0}', ", mr.indicatorWeight); //WEIGHT
                        stbConsolChecking.AppendFormat("'{0}' ", transactionID); //transactionID
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

        public List<HierarchiesIdModel> getHierarchyById(string dateM, string idEnv)
        {
            List<HierarchiesIdModel> lhid = new List<HierarchiesIdModel>();


            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                try
                {
                    connection.Open();
                    StringBuilder stb = new StringBuilder();

                    stb.AppendFormat("DECLARE @InputID INT; SET @InputID = {0}; ", idEnv);
                    stb.AppendFormat("DECLARE @DateEnv DATE; SET @DateEnv = '{0}'; ", dateM);

                    stb.Append("WITH HIERARCHYCTE AS ( ");
                    stb.Append("    SELECT  ");
                    stb.Append("        IDGDA_HISTORY_HIERARCHY_RELATIONSHIP, ");
                    stb.Append("        CONTRACTORCONTROLID, ");
                    stb.Append("        PARENTIDENTIFICATION, ");
                    stb.Append("        IDGDA_COLLABORATORS, ");
                    stb.Append("        IDGDA_HIERARCHY, ");
                    stb.Append("        CREATED_AT, ");
                    stb.Append("        DELETED_AT, ");
                    stb.Append("        TRANSACTIONID, ");
                    stb.Append("        LEVELWEIGHT, ");
                    stb.Append("        DATE, ");
                    stb.Append("        LEVELNAME ");
                    stb.Append("    FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP ");
                    stb.Append("    WHERE IDGDA_COLLABORATORS = @INPUTID ");
                    stb.Append("    AND CONVERT(DATE, [DATE]) = @DATEENV ");
                    stb.Append(" ");
                    stb.Append("    UNION ALL ");
                    stb.Append(" ");
                    stb.Append("    SELECT  ");
                    stb.Append("        H.IDGDA_HISTORY_HIERARCHY_RELATIONSHIP, ");
                    stb.Append("        H.CONTRACTORCONTROLID, ");
                    stb.Append("        H.PARENTIDENTIFICATION, ");
                    stb.Append("        H.IDGDA_COLLABORATORS, ");
                    stb.Append("        H.IDGDA_HIERARCHY, ");
                    stb.Append("        H.CREATED_AT, ");
                    stb.Append("        H.DELETED_AT, ");
                    stb.Append("        H.TRANSACTIONID, ");
                    stb.Append("        H.LEVELWEIGHT, ");
                    stb.Append("        H.DATE, ");
                    stb.Append("        H.LEVELNAME ");
                    stb.Append("    FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP H ");
                    stb.Append("    INNER JOIN HIERARCHYCTE CTE ON H.IDGDA_COLLABORATORS = CTE.PARENTIDENTIFICATION ");
                    stb.Append("    WHERE CONVERT(DATE, CTE.[DATE]) = @DATEENV ");
                    stb.Append(") ");
                    stb.Append(" ");
                    stb.Append(", RECURSIVECTE AS ( ");
                    stb.Append("    SELECT ");
                    stb.Append("        CTE.IDGDA_COLLABORATORS AS PARENTID, ");
                    stb.Append("        CTE.LEVELNAME, ");
                    stb.Append("        [LEVEL] = 1 ");
                    stb.Append("    FROM HIERARCHYCTE CTE ");
                    stb.Append(" ");
                    stb.Append("    UNION ALL ");
                    stb.Append(" ");
                    stb.Append("    SELECT ");
                    stb.Append("        H.IDGDA_COLLABORATORS, ");
                    stb.Append("        H.LEVELNAME, ");
                    stb.Append("        [LEVEL] = RC.[LEVEL] + 1 ");
                    stb.Append("    FROM RECURSIVECTE RC ");
                    stb.Append("    JOIN HIERARCHYCTE H ON RC.PARENTID = H.PARENTIDENTIFICATION ");
                    stb.Append("    WHERE CONVERT(DATE, H.[DATE]) = @DATEENV ");
                    stb.Append(") ");
                    stb.Append(" ");
                    stb.Append("SELECT DISTINCT PARENTID, LEVELNAME, MAX([LEVEL]) AS LEVEL ");
                    stb.Append("FROM RECURSIVECTE ");
                    stb.Append("WHERE PARENTID != @INPUTID ");
                    stb.Append("GROUP BY PARENTID, LEVELNAME ");

                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                HierarchiesIdModel hid = new HierarchiesIdModel();

                                hid.parentId = reader["PARENTID"].ToString();
                                hid.levelName = reader["LEVELNAME"].ToString();
                                hid.level = reader["LEVEL"].ToString();

                                lhid.Add(hid);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                connection.Close();
            }

            return lhid;
        }

        //

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

                    stb.Append("SELECT COUNT(DISTINCT COLLABORATOR_ID) AS QUANTIDADE, ");
                    stb.Append("       CASE ");
                    stb.Append("           WHEN SUM(INPUT) IS NULL THEN 0 ");
                    stb.Append("           ELSE SUM(INPUT) ");
                    stb.Append("       END AS SOMA ");
                    stb.Append("FROM GDA_CHECKING_ACCOUNT (NOLOCK) CA ");
                    stb.Append("WHERE CA.COLLABORATOR_ID = @INPUTID ");
                    stb.Append("  AND CA.RESULT_DATE = @DATEENV ");
                    stb.Append("  AND GDA_INDICATOR_IDGDA_INDICATOR = @INDICADORID ");
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


        public List<MonetizationHierarchy> getMonetizationHierarchy(string dateM, string idEnv, string idIndicador, string idSetor)
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
                    stb.Append(" ");
                    stb.Append("WITH HIERARCHYCTE AS ");
                    stb.Append("  (SELECT IDGDA_HISTORY_HIERARCHY_RELATIONSHIP, ");
                    stb.Append("          CONTRACTORCONTROLID, ");
                    stb.Append("          PARENTIDENTIFICATION, ");
                    stb.Append("          IDGDA_COLLABORATORS, ");
                    stb.Append("          IDGDA_HIERARCHY, ");
                    stb.Append("          CREATED_AT, ");
                    stb.Append("          DELETED_AT, ");
                    stb.Append("          TRANSACTIONID, ");
                    stb.Append("          LEVELWEIGHT, DATE, LEVELNAME ");
                    stb.Append("   FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP ");
                    stb.Append("   WHERE IDGDA_COLLABORATORS = @INPUTID ");
                    stb.Append("     AND [DATE] = @DATEENV ");
                    stb.Append("   UNION ALL SELECT H.IDGDA_HISTORY_HIERARCHY_RELATIONSHIP, ");
                    stb.Append("                    H.CONTRACTORCONTROLID, ");
                    stb.Append("                    H.PARENTIDENTIFICATION, ");
                    stb.Append("                    H.IDGDA_COLLABORATORS, ");
                    stb.Append("                    H.IDGDA_HIERARCHY, ");
                    stb.Append("                    H.CREATED_AT, ");
                    stb.Append("                    H.DELETED_AT, ");
                    stb.Append("                    H.TRANSACTIONID, ");
                    stb.Append("                    H.LEVELWEIGHT, ");
                    stb.Append("                    H.DATE, ");
                    stb.Append("                    H.LEVELNAME ");
                    stb.Append("   FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP H ");
                    stb.Append("   INNER JOIN HIERARCHYCTE CTE ON H.PARENTIDENTIFICATION = CTE.IDGDA_COLLABORATORS ");
                    stb.Append("   WHERE CTE.LEVELNAME <> 'AGENTE' ");
                    stb.Append("     AND CTE.[DATE] = @DATEENV ) ");
                    stb.Append("SELECT COUNT(DISTINCT R.IDGDA_COLLABORATORS) AS QUANTIDADE, ");
                    stb.Append(" ");
                    stb.Append("(SELECT SUM(INPUT) AS INPUT  FROM GDA_CHECKING_ACCOUNT (NOLOCK) ");
                    stb.Append("where gda_indicator_idgda_indicator = @INDICADORID ");
                    stb.Append("AND result_date = @DATEENV ");
                    stb.Append("AND COLLABORATOR_ID IN ");
                    stb.Append("(SELECT DISTINCT(IDGDA_COLLABORATORS) ");
                    stb.Append("     FROM HIERARCHYCTE ");
                    stb.Append("     WHERE LEVELNAME = 'AGENTE' ");
                    stb.Append("       AND CONVERT(DATE, DATE) = @DATEENV) ");
                    stb.Append(" ");
                    stb.Append(") AS SOMA ");
                    stb.Append("FROM GDA_RESULT (NOLOCK) AS R ");
                    stb.Append("LEFT JOIN GDA_CHECKING_ACCOUNT (NOLOCK) AS CA ON R.IDGDA_COLLABORATORS = CA.COLLABORATOR_ID ");
                    stb.Append("AND CA.RESULT_DATE = @DATEENV ");
                    stb.Append("AND CA.GDA_INDICATOR_IDGDA_INDICATOR = @INDICADORID ");
                    stb.Append(" ");
                    stb.Append(" ");
                    stb.Append("INNER JOIN GDA_HISTORY_COLLABORATOR_SECTOR (NOLOCK) AS HCS ON HCS.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
                    stb.Append("AND CONVERT(DATE, HCS.CREATED_AT) > @DATEENV ");
                    stb.Append("INNER JOIN ");
                    stb.Append("  (SELECT INDICATOR_ID, ");
                    stb.Append("          SECTOR_ID ");
                    stb.Append("   FROM GDA_HISTORY_INDICATOR_GROUP (NOLOCK) ");
                    stb.Append("   WHERE MONETIZATION > 0 ");
                    stb.Append("     AND DELETED_AT IS NULL ");
                    stb.Append("   GROUP BY INDICATOR_ID, ");
                    stb.Append("            SECTOR_ID) AS INDA ON INDA.SECTOR_ID = HCS.IDGDA_SECTOR ");
                    stb.Append("AND INDA.INDICATOR_ID = R.INDICADORID ");
                    stb.Append(" ");
                    //stb.Append("INNER JOIN ");
                    //stb.Append("  (SELECT GOAL, ");
                    //stb.Append("          INDICATOR_ID, ");
                    //stb.Append("          SECTOR_ID, ");
                    //stb.Append("          ROW_NUMBER() OVER (PARTITION BY INDICATOR_ID, ");
                    //stb.Append("                                          SECTOR_ID ");
                    //stb.Append("                             ORDER BY CREATED_AT DESC) AS RN ");
                    //stb.Append("   FROM GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) ");
                    //stb.Append("   WHERE DELETED_AT IS NULL ) AS HIS ON HIS.INDICATOR_ID = R.INDICADORID ");
                    //stb.Append("AND HIS.SECTOR_ID = HCS.IDGDA_SECTOR ");
                    stb.Append(" ");
                    stb.Append("WHERE R.IDGDA_COLLABORATORS IN ");
                    stb.Append("    (SELECT DISTINCT(IDGDA_COLLABORATORS) ");
                    stb.Append("     FROM HIERARCHYCTE ");
                    stb.Append("     WHERE LEVELNAME = 'AGENTE' ");
                    stb.Append("       AND CONVERT(DATE, DATE) = @DATEENV ) ");
                    stb.Append("  AND R.CREATED_AT = @DATEENV ");
                    stb.Append("  AND R.INDICADORID = @INDICADORID ");
                    stb.Append("UNION ALL ");
                    stb.Append("SELECT COUNT(DISTINCT COLLABORATOR_ID) AS QUANTIDADE, ");
                    stb.Append("       CASE ");
                    stb.Append("           WHEN SUM(INPUT) IS NULL THEN 0 ");
                    stb.Append("           ELSE SUM(INPUT) ");
                    stb.Append("       END AS SOMA ");
                    stb.Append("FROM GDA_CHECKING_ACCOUNT (NOLOCK) CA ");
                    stb.Append("WHERE CA.COLLABORATOR_ID = @INPUTID ");
                    stb.Append("  AND CA.RESULT_DATE = @DATEENV ");
                    stb.Append("  AND GDA_INDICATOR_IDGDA_INDICATOR = @INDICADORID ");
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



                    //stb.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}'; ", idEnv);
                    //stb.AppendFormat("DECLARE @DATEENV DATE; SET @DATEENV = '{0}'; ", dateM);
                    //stb.AppendFormat("DECLARE @INDICADORID VARCHAR(MAX); SET @INDICADORID = '{0}'; ", idIndicador);
                    //stb.Append(" ");
                    //stb.Append("WITH HIERARCHYCTE AS ");
                    //stb.Append("  (SELECT IDGDA_HISTORY_HIERARCHY_RELATIONSHIP, ");
                    //stb.Append("          CONTRACTORCONTROLID, ");
                    //stb.Append("          PARENTIDENTIFICATION, ");
                    //stb.Append("          IDGDA_COLLABORATORS, ");
                    //stb.Append("          IDGDA_HIERARCHY, ");
                    //stb.Append("          CREATED_AT, ");
                    //stb.Append("          DELETED_AT, ");
                    //stb.Append("          TRANSACTIONID, ");
                    //stb.Append("          LEVELWEIGHT, DATE, LEVELNAME ");
                    //stb.Append("   FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP ");
                    //stb.Append("   WHERE IDGDA_COLLABORATORS = @INPUTID ");
                    //stb.Append("     AND CONVERT(DATE, [DATE]) = @DATEENV ");
                    //stb.Append("   UNION ALL SELECT H.IDGDA_HISTORY_HIERARCHY_RELATIONSHIP, ");
                    //stb.Append("                    H.CONTRACTORCONTROLID, ");
                    //stb.Append("                    H.PARENTIDENTIFICATION, ");
                    //stb.Append("                    H.IDGDA_COLLABORATORS, ");
                    //stb.Append("                    H.IDGDA_HIERARCHY, ");
                    //stb.Append("                    H.CREATED_AT, ");
                    //stb.Append("                    H.DELETED_AT, ");
                    //stb.Append("                    H.TRANSACTIONID, ");
                    //stb.Append("                    H.LEVELWEIGHT, ");
                    //stb.Append("                    H.DATE, ");
                    //stb.Append("                    H.LEVELNAME ");
                    //stb.Append("   FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP H ");
                    //stb.Append("   INNER JOIN HIERARCHYCTE CTE ON H.PARENTIDENTIFICATION = CTE.IDGDA_COLLABORATORS ");
                    //stb.Append("   WHERE CTE.LEVELNAME <> 'AGENTE' ");
                    //stb.Append("     AND CONVERT(DATE, CTE.[DATE]) = @DATEENV ) ");
                    //stb.Append("SELECT COUNT(DISTINCT R.IDGDA_COLLABORATORS) AS QUANTIDADE, ");
                    //stb.Append("       CASE ");
                    //stb.Append("           WHEN SUM(INPUT) IS NULL THEN 0 ");
                    //stb.Append("           ELSE SUM(INPUT) ");
                    //stb.Append("       END AS SOMA ");
                    //stb.Append("FROM GDA_RESULT (NOLOCK) AS R ");
                    //stb.Append("LEFT JOIN GDA_CHECKING_ACCOUNT (NOLOCK) AS CA ON R.IDGDA_COLLABORATORS = CA.COLLABORATOR_ID ");
                    //stb.Append("AND CA.RESULT_DATE = @DATEENV ");
                    //stb.Append("AND CA.GDA_INDICATOR_IDGDA_INDICATOR = @INDICADORID ");
                    //stb.Append("WHERE IDGDA_COLLABORATORS IN ");
                    //stb.Append("    (SELECT DISTINCT(IDGDA_COLLABORATORS) ");
                    //stb.Append("     FROM HIERARCHYCTE ");
                    //stb.Append("     WHERE LEVELNAME = 'AGENTE' ");
                    //stb.Append("       AND CONVERT(DATE, DATE) = @DATEENV ) ");
                    //stb.Append("  AND R.CREATED_AT = @DATEENV ");
                    //stb.Append("  AND R.INDICADORID = @INDICADORID ");
                    //stb.Append("UNION ALL ");
                    //stb.Append("SELECT COUNT(DISTINCT COLLABORATOR_ID) AS QUANTIDADE, ");
                    //stb.Append("       CASE ");
                    //stb.Append("           WHEN SUM(INPUT) IS NULL THEN 0 ");
                    //stb.Append("           ELSE SUM(INPUT) ");
                    //stb.Append("       END AS SOMA ");
                    //stb.Append("FROM GDA_CHECKING_ACCOUNT (NOLOCK) CA ");
                    //stb.Append("WHERE CA.COLLABORATOR_ID = @INPUTID ");
                    //stb.Append("  AND CA.RESULT_DATE = @DATEENV ");
                    //stb.Append("  AND GDA_INDICATOR_IDGDA_INDICATOR = @INDICADORID ");
                    //stb.Append("UNION ALL ");
                    //stb.Append("SELECT COALESCE(SUM(QUANTIDADE), 0) AS QUANTIDADE, ");
                    //stb.Append("       COALESCE(SUM(SOMA), 0) AS SOMA ");
                    //stb.Append("FROM ");
                    //stb.Append("  (SELECT TOP 1 '1' AS QUANTIDADE, ");
                    //stb.Append("              BALANCE AS SOMA ");
                    //stb.Append("   FROM GDA_CHECKING_ACCOUNT (NOLOCK) CA ");
                    //stb.Append("   WHERE CA.COLLABORATOR_ID = @INPUTID ");
                    //stb.Append("   ORDER BY CREATED_AT DESC ");
                    //stb.Append("   UNION ALL SELECT 0 AS QUANTIDADE, ");
                    //stb.Append("                    0 AS SOMA) AS A ");

                    //stb.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}'; ", idEnv);
                    //stb.AppendFormat("DECLARE @DATEENV DATE; SET @DATEENV = '{0}'; ", dateM);
                    //stb.AppendFormat("DECLARE @INDICADORID VARCHAR(MAX); SET @INDICADORID = '{0}'; ", idIndicador);
                    //stb.AppendFormat("DECLARE @IDGDA_SECTOR VARCHAR(MAX); SET @IDGDA_SECTOR = '{0}'; ", idSetor);
                    //stb.Append(" ");
                    //stb.Append("WITH HIERARCHYCTE AS ( ");
                    //stb.Append("    SELECT  ");
                    //stb.Append("        IDGDA_HISTORY_HIERARCHY_RELATIONSHIP, ");
                    //stb.Append("        CONTRACTORCONTROLID, ");
                    //stb.Append("        PARENTIDENTIFICATION, ");
                    //stb.Append("        IDGDA_COLLABORATORS, ");
                    //stb.Append("        IDGDA_HIERARCHY, ");
                    //stb.Append("        CREATED_AT, ");
                    //stb.Append("        DELETED_AT, ");
                    //stb.Append("        TRANSACTIONID, ");
                    //stb.Append("        LEVELWEIGHT, ");
                    //stb.Append("        DATE, ");
                    //stb.Append("        LEVELNAME ");
                    //stb.Append("    FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP ");
                    //stb.Append("    WHERE IDGDA_COLLABORATORS = @INPUTID  ");
                    //stb.Append("	AND CONVERT(DATE, [DATE]) = @DATEENV ");
                    //stb.Append(" ");
                    //stb.Append("    UNION ALL ");
                    //stb.Append(" ");
                    //stb.Append("    SELECT  ");
                    //stb.Append("        H.IDGDA_HISTORY_HIERARCHY_RELATIONSHIP, ");
                    //stb.Append("        H.CONTRACTORCONTROLID, ");
                    //stb.Append("        H.PARENTIDENTIFICATION, ");
                    //stb.Append("        H.IDGDA_COLLABORATORS, ");
                    //stb.Append("        H.IDGDA_HIERARCHY, ");
                    //stb.Append("        H.CREATED_AT, ");
                    //stb.Append("        H.DELETED_AT, ");
                    //stb.Append("        H.TRANSACTIONID, ");
                    //stb.Append("        H.LEVELWEIGHT, ");
                    //stb.Append("        H.DATE, ");
                    //stb.Append("        H.LEVELNAME ");
                    //stb.Append("    FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP H ");
                    //stb.Append("    INNER JOIN HIERARCHYCTE CTE ON H.PARENTIDENTIFICATION = CTE.IDGDA_COLLABORATORS ");
                    //stb.Append("    WHERE CTE.LEVELNAME <> 'AGENTE' ");
                    //stb.Append("	AND CONVERT(DATE, CTE.[DATE]) = @DATEENV ");
                    //stb.Append(") ");
                    //stb.Append(" ");
                    //stb.Append("SELECT COUNT(DISTINCT COLLABORATOR_ID) AS QUANTIDADE, CASE WHEN SUM(INPUT) IS NULL THEN 0 ELSE SUM(INPUT) END AS SOMA  ");
                    //stb.Append("FROM GDA_CHECKING_ACCOUNT (NOLOCK) CA ");
                    //stb.Append("INNER JOIN GDA_HISTORY_COLLABORATOR_SECTOR (NOLOCK) AS S ON CONVERT(DATE, S.CREATED_AT, 120) = CONVERT(DATE, CA.RESULT_DATE, 120)  ");
                    //stb.Append("AND S.IDGDA_COLLABORATORS = CA.COLLABORATOR_ID ");
                    //stb.Append("WHERE CA.COLLABORATOR_ID IN ( ");
                    //stb.Append("	SELECT DISTINCT(IDGDA_COLLABORATORS) ");
                    //stb.Append("	FROM HIERARCHYCTE ");
                    //stb.Append("	WHERE LEVELNAME = 'AGENTE' ");
                    //stb.Append("	AND CONVERT(DATE, DATE) = @DATEENV ");
                    //stb.Append(") ");
                    //stb.Append("AND CA.RESULT_DATE = @DATEENV ");
                    //stb.Append("AND GDA_INDICATOR_IDGDA_INDICATOR = @INDICADORID ");
                    //stb.Append("AND S.IDGDA_SECTOR = @IDGDA_SECTOR ");
                    //stb.Append("UNION ALL ");
                    //stb.Append("SELECT COUNT(DISTINCT COLLABORATOR_ID) AS QUANTIDADE, CASE WHEN SUM(INPUT) IS NULL THEN 0 ELSE SUM(INPUT) END AS SOMA ");
                    //stb.Append("FROM GDA_CHECKING_ACCOUNT (NOLOCK) CA ");
                    //stb.Append("WHERE CA.COLLABORATOR_ID = @INPUTID ");
                    //stb.Append("AND CA.RESULT_DATE = @DATEENV ");
                    //stb.Append("AND GDA_INDICATOR_IDGDA_INDICATOR = @INDICADORID ");
                    //stb.Append("AND CA.IDGDA_SECTOR = @IDGDA_SECTOR ");
                    //stb.Append("UNION ALL ");
                    //stb.Append("SELECT COALESCE(SUM(QUANTIDADE), 0) AS QUANTIDADE, COALESCE(SUM(SOMA), 0) AS SOMA ");
                    //stb.Append("FROM ( ");
                    //stb.Append("    SELECT TOP 1 '1' AS QUANTIDADE, BALANCE AS SOMA  ");
                    //stb.Append("    FROM GDA_CHECKING_ACCOUNT (NOLOCK) CA ");
                    //stb.Append("    WHERE CA.COLLABORATOR_ID = @INPUTID ");
                    //stb.Append("    ORDER BY CREATED_AT DESC ");
                    //stb.Append("    UNION ALL ");
                    //stb.Append("    SELECT 0 AS QUANTIDADE, 0 AS SOMA ");
                    //stb.Append(") AS A ");

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


        public List<string> getSectorsByCollaborators(string dateM, string idEnv)
        {
            List<string> set = new List<string>();

            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                try
                {
                    connection.Open();
                    StringBuilder stb = new StringBuilder();

                    stb.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}'; ", idEnv);
                    stb.AppendFormat("DECLARE @DATEENV DATE; SET @DATEENV = '{0}'; ", dateM);

                    stb.Append("WITH HIERARCHYCTE AS ( ");
                    stb.Append("    SELECT  ");
                    stb.Append("        IDGDA_HISTORY_HIERARCHY_RELATIONSHIP, ");
                    stb.Append("        CONTRACTORCONTROLID, ");
                    stb.Append("        PARENTIDENTIFICATION, ");
                    stb.Append("        IDGDA_COLLABORATORS, ");
                    stb.Append("        IDGDA_HIERARCHY, ");
                    stb.Append("        CREATED_AT, ");
                    stb.Append("        DELETED_AT, ");
                    stb.Append("        TRANSACTIONID, ");
                    stb.Append("        LEVELWEIGHT, ");
                    stb.Append("        DATE, ");
                    stb.Append("        LEVELNAME ");
                    stb.Append("    FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP ");
                    stb.Append("    WHERE PARENTIDENTIFICATION = @INPUTID  ");
                    stb.Append("	AND CONVERT(DATE, [DATE]) = @DATEENV ");
                    stb.Append(" ");
                    stb.Append("    UNION ALL ");
                    stb.Append(" ");
                    stb.Append("    SELECT  ");
                    stb.Append("        H.IDGDA_HISTORY_HIERARCHY_RELATIONSHIP, ");
                    stb.Append("        H.CONTRACTORCONTROLID, ");
                    stb.Append("        H.PARENTIDENTIFICATION, ");
                    stb.Append("        H.IDGDA_COLLABORATORS, ");
                    stb.Append("        H.IDGDA_HIERARCHY, ");
                    stb.Append("        H.CREATED_AT, ");
                    stb.Append("        H.DELETED_AT, ");
                    stb.Append("        H.TRANSACTIONID, ");
                    stb.Append("        H.LEVELWEIGHT, ");
                    stb.Append("        H.DATE, ");
                    stb.Append("        H.LEVELNAME ");
                    stb.Append("    FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP H ");
                    stb.Append("    INNER JOIN HIERARCHYCTE CTE ON H.PARENTIDENTIFICATION = CTE.IDGDA_COLLABORATORS ");
                    stb.Append("    WHERE CTE.LEVELNAME <> 'AGENTE' ");
                    stb.Append("	AND CONVERT(DATE, CTE.[DATE]) = @DATEENV ");
                    stb.Append(") ");
                    stb.Append(" ");
                    stb.Append("SELECT DISTINCT IDGDA_SECTOR FROM GDA_HISTORY_COLLABORATOR_SECTOR (NOLOCK) AS S ");
                    stb.Append("WHERE S.IDGDA_COLLABORATORS IN ( ");
                    stb.Append("	SELECT DISTINCT(IDGDA_COLLABORATORS) ");
                    stb.Append("	FROM HIERARCHYCTE ");
                    stb.Append("	WHERE LEVELNAME = 'AGENTE' ");
                    stb.Append("	AND CONVERT(DATE, DATE) = @DATEENV ");
                    stb.Append(") ");
                    stb.Append("AND S.CREATED_AT = @DATEENV ");


                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                set.Add(reader["IDGDA_SECTOR"].ToString());
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                connection.Close();
            }

            return set;
        }

        //// POST: api/Results
        //[ResponseType(typeof(ResultsModel))]
        //public IHttpActionResult PostResultsModel(long t_id, IEnumerable<Result> results)
        //{
        //    bool validation = false;

        //    //Valida Token
        //    var tkn = TokenValidate.validate(Request.Headers);
        //    if (tkn == false)
        //    {
        //        return BadRequest("Invalid Token!");
        //    }

        //    //Valida Model
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    //Valida Transaction
        //    var t = TransactionValidade.validate(Request.Headers, t_id);
        //    if (t is null)
        //    {
        //        return BadRequest("Invalid Transaction!");
        //    }


        //    foreach (Result re in results)
        //    {
        //        //Valida se o colaborador ja existe
        //        //CollaboratorTableModel cl = db.CollaboratorTableModels.Where(p => p.collaboratorIdentification == re.collaboratorIdentification).FirstOrDefault();
        //        //if (cl is null)
        //        //{
        //        //    continue;
        //        //}

        //        ResultsModel rm = new ResultsModel();

        //        var idEx = re.collaboratorIdentification.Replace("BC", "");

        //        rm.IDGDA_COLLABORATORS = Convert.ToInt32(idEx);
        //        rm.INDICADORID = re.indicadorId;
        //        rm.TRANSACTIONID = t.idgda_Transaction;

        //        rm.RESULT = re.resultado;
        //        rm.CREATED_AT = re.date;

        //        var factors = "";
        //        foreach (string fm in re.factors)
        //        {
        //            if (factors == "")
        //            {
        //                factors = fm;
        //            }
        //            else
        //            {
        //                factors = factors + ";" + fm;
        //            }

        //        }

        //        rm.factors = factors;

        //        //try
        //        //{

        //            db.ResultsModels.Add(rm);
        //        //    db.SaveChanges();
        //        //    validation = true;
        //        //    var index = 1;

        //        //    foreach (string fm in re.factors)
        //        //    {

        //        //        FactorsModel facModel = new FactorsModel();
        //        //        facModel.IDGDA_RESULT = rm.IDGDA_RESULT;
        //        //        facModel.INDEX = index;
        //        //        facModel.FACTOR = fm;
        //        //        try
        //        //        {
        //        //            db.FactorsModels.Add(facModel);
        //        //            db.SaveChanges();
        //        //        }
        //        //        catch
        //        //        { }
        //        //        validation = true;
        //        //        index += 1;
        //        //    }
        //        //}
        //        //catch
        //        //{ }
        //    }

        //    try
        //    {
        //        db.SaveChanges();
        //        validation = true;
        //    }
        //    catch
        //    {
        //    }

        //    //var tknNovo = getTokenApi();

        //    //enviaResultado(t_id.ToString(), tknNovo);

        //    if (validation == true)
        //    {

        //        return CreatedAtRoute("DefaultApi", new { id = results.First().collaboratorIdentification }, results);
        //    }
        //    else
        //    {
        //        return BadRequest("No information entered.");
        //    }
        //}

        //public static void enviaResultado(string t_id, string tokenEnviado)
        //{
        //    try
        //    {
        //        // Criar o objeto que representa o JSON
        //        var data = new { transaction = t_id.ToString() };

        //        // Serializar o objeto para JSON
        //        string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);

        //        // Criar o conteúdo da requisição com o JSON
        //        var content = new StringContent(json, Encoding.UTF8, "application/json");

        //        // Criar uma instância do HttpClient
        //        using (var client = new HttpClient())
        //        {
        //            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenEnviado);
        //            // Fazer a chamada HTTP POST
        //            HttpResponseMessage response = client.PostAsync("http://44.196.213.55:3001/Results", content).Result;
        //        }
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        public string getTokenApi()
        {
            try
            {
                // Criar o objeto que representa o JSON
                var data = new { username = "admin", password = "sBf4zoYl" };

                // Serializar o objeto para JSON
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);

                // Criar o conteúdo da requisição com o JSON
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Criar uma instância do HttpClient
                using (var client = new HttpClient())
                {
                    // Fazer a chamada HTTP POST
                    HttpResponseMessage response = client.PostAsync("http://44.196.213.55:3001/authentication", content).Result;


                    string resp = response.Content.ReadAsStringAsync().Result;

                    var jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(resp);

                    string elementValue = jsonObject.token;

                    return elementValue;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        // DELETE: api/Results/5
        [ResponseType(typeof(ResultsModel))]
        public IHttpActionResult DeleteResultsModel(int id)
        {
            ResultsModel resultsModel = db.ResultsModels.Find(id);
            if (resultsModel == null)
            {
                return NotFound();
            }
            try
            {
                db.ResultsModels.Remove(resultsModel);
                db.SaveChanges();
            }
            catch
            {

            }


            return Ok(resultsModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ResultsModelExists(int id)
        {
            return db.ResultsModels.Count(e => e.IDGDA_RESULT == id) > 0;
        }
    }
}