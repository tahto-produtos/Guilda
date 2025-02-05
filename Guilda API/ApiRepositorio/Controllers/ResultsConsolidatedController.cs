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
    public class ResultsConsolidatedController : ApiController
    {
        //private string connectionString = "Data Source=database-1.cpmxhjwr8mgp.us-east-1.rds.amazonaws.com;Initial Catalog=GUILDA_PROD;User ID=RDSadminsql2019;Password=H4ND4faG3AhjFe943NyPLRhMsEamXucKdDa;MultipleActiveResultSets=True;Connection Timeout=10";


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
                    queryInsertResult.Append("  INSERT (INDICADORID, TRANSACTIONID, RESULT, CREATED_AT, IDGDA_COLLABORATORS, FACTORS, INSERTED_AT, FACTORSAG0, FACTORSAG1) ");
                    queryInsertResult.Append("  VALUES (INDICADORID, TRANSACTIONID, RESULTADO,[DATE], COLLABORATORIDENTIFICATION, FACTORS, GETDATE(), LEFT(FACTORS, CHARINDEX(';', FACTORS) - 1), RIGHT(FACTORS, LEN(FACTORS) - CHARINDEX(';', FACTORS))) ");
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

            List<string> datas = new List<string>();
            try
            {
                Log.insertLogTransaction(t_id.ToString(), "TABLE GLASS", "START", "");

                try
                {
                    //string connectionString = "Data Source=database-1.cpmxhjwr8mgp.us-east-1.rds.amazonaws.com;Initial Catalog=GUILDA_PROD;User ID=RDSadminsql2019;Password=H4ND4faG3AhjFe943NyPLRhMsEamXucKdDa;MultipleActiveResultSets=True;Connection Timeout=10";
                    SqlConnection connection = new SqlConnection(Database.Conn);


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

                Log.insertLogTransaction(t_id.ToString(), "TABLE GLASS", "CONCLUDED", "");
            }
            catch (Exception ex)
            {
                Log.insertLogTransaction(t_id.ToString(), "TABLE GLASS", "ERRO: " + ex.Message.ToString(), "");
            }

            try
            {
                Log.insertLogTransaction(t_id.ToString(), "MONETIZATION", "START", "");
                Task.Run(() => monetization(t.idgda_Transaction, t_id).ToString());
            }
            catch (Exception ex)
            {
                Log.insertLogTransaction(t_id.ToString(), "MONETIZATION", "ERRO: " + ex.Message.ToString(), "");
            }


            try
            {
                Log.insertLogTransaction(t_id.ToString(), "EXPIRATION COIN", "START", "");
                dueMonetizationExpiration();
                Log.insertLogTransaction(t_id.ToString(), "EXPIRATION COIN", "CONCLUDED", "");
            }
            catch (Exception ex)
            {
                Log.insertLogTransaction(t_id.ToString(), "MONETIZATION", "ERRO: " + ex.Message.ToString(), "");
            }


            //try
            //{
            //    for (int i = 1; i < 10; i++)
            //    {
            //        DateTime dtAgora = DateTime.Now.AddDays(-i);

            //        doBasketMonetization(dtAgora.ToString("yyyy-MM-dd"));
            //    }
            //}
            //catch (Exception)
            //{

            //}
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

            //Reformulado.
            //try
            //{
            //    List<ResultsConsolidatedController.basket> lbasket = ResultsConsolidatedController.listBasketConfiguration();
            //    List<ResultsConsolidatedController.groups> lgroup = ResultsConsolidatedController.listGroups("");

            //    List<ResultsConsolidatedController.usuariosBk> users = ResultsConsolidatedController.returnUsers(DateTime.Now.ToString("yyyy-MM-dd"));
            //    foreach (ResultsConsolidatedController.usuariosBk user in users)
            //    {
            //        ResultsConsolidatedController.ReturnBasketIndicatorUser biu = ResultsConsolidatedController.returnIndicatorUserMonetization(user.idCollaborator.ToString(), lbasket, lgroup);

            //        ResultsConsolidatedController.attUser(biu.idGroup, user.idTable);
            //    }
            //}
            //catch (Exception)
            //{

            //}
            Log.insertLogTransaction(t_id.ToString(), "RESULT_CONSOLIDATED", "RETURN", "");
            return StatusCode(HttpStatusCode.Created);
        }



        public class usuariosBk
        {
            public int idCollaborator { get; set; }
            public int idTable { get; set; }
        }

        public class usuariosBkNovo
        {
            public int idCollaborator { get; set; }
            public int idTable { get; set; }
            public double conta { get; set; }
            public double ganho { get; set; }
            public double max_ganho { get; set; }
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
        public class groups
        {
            public int id { get; set; }
            public string name { get; set; }
            public string alias { get; set; }
            public string image { get; set; }

        }
        public class basket
        {
            public int metric_min { get; set; }
            public int group_id { get; set; }

        }

        public static void attUser(int idGroup, int idTable)
        {

            //QUERY MOEDAS GANHAS
            string dtUltima = DateTime.Now.ToString("yyyy-MM-dd");
            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("UPDATE GDA_COLLABORATORS_DETAILS ");
            stb.AppendFormat("SET IDGDA_GROUP = '{0}' ", idGroup);
            stb.AppendFormat("WHERE ID = '{0}' ", idTable);

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
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

        //public static List<usuariosBkNovo> returnUsers(string dtUltima)
        //{
        //    List<usuariosBkNovo> users = new List<usuariosBkNovo>();

        //    string dtAg = Convert.ToDateTime(dtUltima).AddDays(-30).Month.ToString();
                

        //    //QUERY MOEDAS GANHAS
        //    //string dtUltima = DateTime.Now.ToString("yyyy-MM-dd");
        //    StringBuilder stb = new StringBuilder();
        //    //stb.AppendFormat("SELECT IDGDA_COLLABORATORS, MAX(ID) AS ID ");
        //    //stb.AppendFormat("FROM GDA_COLLABORATORS_DETAILS (NOLOCK) ");
        //    //stb.AppendFormat("WHERE CREATED_AT >= '{0}' AND ACTIVE = 'true' ", dtUltima);
        //    //stb.AppendFormat("GROUP BY IDGDA_COLLABORATORS ");
        //    stb.AppendFormat("SELECT IDGDA_COLLABORATORS, MAX(ID) AS ID, SUM(GANHO) AS GANHO, SUM(MAX_GANHO) AS MAX_GANHO, CASE WHEN SUM(MAX_GANHO) = 0 THEN 0 ELSE SUM(GANHO) / SUM(MAX_GANHO) END AS CONTA ");
        //    stb.AppendFormat("FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CD ");
        //    stb.AppendFormat($"INNER JOIN GDA_PERFORMANCE_MENSAL (NOLOCK) AS P ON CD.IDGDA_COLLABORATORS = P.CHAVE_EXTERNA AND P.mes >= {dtAg} ");
        //    stb.AppendFormat("WHERE CREATED_AT = '{0}' ", dtUltima);
        //    stb.AppendFormat("GROUP BY IDGDA_COLLABORATORS ");

        //    using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
        //    {
        //        try
        //        {
        //            connection.Open();
        //            using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
        //            {
        //                using (SqlDataReader reader = command.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        usuariosBkNovo user = new usuariosBkNovo();
        //                        user.idCollaborator = Convert.ToInt32(reader["IDGDA_COLLABORATORS"].ToString());
        //                        user.idTable = Convert.ToInt32(reader["ID"].ToString());
        //                        user.conta = Convert.ToDouble(reader["CONTA"].ToString());
        //                        user.ganho = Convert.ToDouble(reader["GANHO"].ToString());
        //                        user.max_ganho = Convert.ToDouble(reader["MAX_GANHO"].ToString());
        //                        users.Add(user);
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception)
        //        {

        //            //throw;
        //        }
        //        connection.Close();
        //    }
        //    return users;
        //}

        public static List<usuariosBk> returnUsers(string dtUltima)
        {
            List<usuariosBk> users = new List<usuariosBk>();

            string dtAg = Convert.ToDateTime(dtUltima).AddDays(-30).Month.ToString();


            //QUERY MOEDAS GANHAS
            //string dtUltima = DateTime.Now.ToString("yyyy-MM-dd");
            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("SELECT IDGDA_COLLABORATORS, MAX(ID) AS ID ");
            stb.AppendFormat("FROM GDA_COLLABORATORS_DETAILS (NOLOCK) ");
            stb.AppendFormat("WHERE CREATED_AT = '{0}' ", dtUltima);
            stb.AppendFormat("GROUP BY IDGDA_COLLABORATORS ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
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

        //public static ReturnBasketIndicatorUser returnIndicatorUserMonetization(string idCol, double conta, double ganho, double max_ganho, List<basket> lbasket, List<groups> lgroup)
        //{
        //    ReturnBasketIndicatorUser rmams = new ReturnBasketIndicatorUser();
        //    try
        //    {

        //        //REALIZA CONTA
        //        double resultPercent = conta * 100;

        //        //COMO ELE NÃO TEVE COMO GANHAR NENHUMA MOEDA, ELE ATINGIU 100% DA META
        //        if (max_ganho == 0)
        //        {
        //            resultPercent = 100;
        //        }
        //        resultPercent = Math.Round(resultPercent, 2, MidpointRounding.AwayFromZero);
        //        rmams.resultPercent = resultPercent;
        //        basket lbasket1 = lbasket.Find(l => l.group_id == 1);
        //        basket lbasket2 = lbasket.Find(l => l.group_id == 2);
        //        basket lbasket3 = lbasket.Find(l => l.group_id == 3);
        //        basket lbasket4 = lbasket.Find(l => l.group_id == 4);
        //        groups lgroup1 = lgroup.Find(l => l.id == 1);
        //        groups lgroup2 = lgroup.Find(l => l.id == 2);
        //        groups lgroup3 = lgroup.Find(l => l.id == 3);
        //        groups lgroup4 = lgroup.Find(l => l.id == 4);
        //        if (resultPercent >= lbasket1.metric_min)
        //        {
        //            rmams.groupName = lgroup1.name;
        //            rmams.idGroup = lgroup1.id;
        //            rmams.groupAlias = lgroup1.alias;
        //            rmams.groupImage = lgroup1.image;
        //        }
        //        else if (resultPercent >= lbasket2.metric_min)
        //        {
        //            rmams.groupName = lgroup2.name;
        //            rmams.idGroup = lgroup2.id;
        //            rmams.groupAlias = lgroup2.alias;
        //            rmams.groupImage = lgroup2.image;
        //        }
        //        else if (resultPercent >= lbasket3.metric_min)
        //        {
        //            rmams.groupName = lgroup3.name;
        //            rmams.idGroup = lgroup3.id;
        //            rmams.groupAlias = lgroup3.alias;
        //            rmams.groupImage = lgroup3.image;
        //        }
        //        else if (resultPercent >= lbasket4.metric_min)
        //        {
        //            rmams.groupName = lgroup4.name;
        //            rmams.idGroup = lgroup4.id;
        //            rmams.groupAlias = lgroup4.alias;
        //            rmams.groupImage = lgroup4.image;
        //        }
        //        else
        //        {
        //            rmams.groupName = lgroup4.name;
        //            rmams.idGroup = lgroup4.id;
        //            rmams.groupAlias = lgroup4.alias;
        //            rmams.groupImage = lgroup4.image;
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    return rmams;
        //}


        public static ReturnBasketIndicatorUser returnIndicatorUserMonetization(string idCol, List<basket> lbasket, List<groups> lgroup)
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
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
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
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
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
                        stb.Clear();
                        stb.Append($"DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{dtIni}'; ");
                        stb.Append($"DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{dtFim}';  ");
                        stb.Append($"DECLARE @INPUTID INT; SET @INPUTID = '{idCol}'; ");
                        stb.Append(" ");
                        stb.Append("SELECT  ");
                        stb.Append("       SUM(QTD) AS QTD,  ");
                        stb.Append("       SUM(CASE  ");
                        stb.Append("               WHEN TBL.IDGDA_PERIOD = 1  ");
                        stb.Append("                    AND HIG1.MONETIZATION > 0 THEN QTD  ");
                        stb.Append("               WHEN TBL.IDGDA_PERIOD = 2  ");
                        stb.Append("                    AND HIG1.MONETIZATION_NIGHT > 0 THEN QTD  ");
                        stb.Append("               WHEN TBL.IDGDA_PERIOD = 3  ");
                        stb.Append("                    AND HIG1.MONETIZATION_LATENIGHT > 0 THEN QTD  ");
                        stb.Append("               ELSE 0  ");
                        stb.Append("           END) AS QTD_MON,  ");
                        stb.Append("       SUM(CASE  ");
                        stb.Append("               WHEN TBL.IDGDA_PERIOD = 1  ");
                        stb.Append("                    AND HIG1.MONETIZATION > 0 THEN (HIG1.MONETIZATION * QTD)  ");
                        stb.Append("               WHEN TBL.IDGDA_PERIOD = 2  ");
                        stb.Append("                    AND HIG1.MONETIZATION_NIGHT > 0 THEN (HIG1.MONETIZATION_NIGHT * QTD)  ");
                        stb.Append("               WHEN TBL.IDGDA_PERIOD = 3  ");
                        stb.Append("                    AND HIG1.MONETIZATION_LATENIGHT > 0 THEN (HIG1.MONETIZATION_LATENIGHT * QTD)  ");
                        stb.Append("               ELSE 0  ");
                        stb.Append("           END) AS QTD_MON_TOTAL ");
                        stb.Append("FROM  ");
                        stb.Append("  (SELECT CONVERT(DATE, CL.CREATED_AT) AS CREATED_AT,  ");
                        stb.Append("          COUNT(0) AS QTD,  ");
                        stb.Append("          CL.IDGDA_PERIOD AS IDGDA_PERIOD,  ");
                        stb.Append("          CL.IDGDA_SECTOR AS IDGDA_SECTOR,  ");
                        stb.Append("          CL.IDGDA_SUBSECTOR AS IDGDA_SUBSECTOR,  ");
                        stb.Append("          CL.IDGDA_SECTOR_SUBSECTOR AS IDGDA_SECTOR_SUBSECTOR,  ");
                        stb.Append("          IT.IDGDA_INDICATOR AS INDICADORID,  ");
                        stb.Append("          CASE  ");
                        stb.Append("              WHEN @INPUTID = MAX(CL.IDGDA_COLLABORATORS) THEN 'AGENTE'  ");
                        stb.Append("              WHEN @INPUTID = MAX(CL.MATRICULA_SUPERVISOR) THEN 'SUPERVISOR'  ");
                        stb.Append("              WHEN @INPUTID = MAX(CL.MATRICULA_COORDENADOR) THEN 'COORDENADOR'  ");
                        stb.Append("              WHEN @INPUTID = MAX(CL.MATRICULA_GERENTE_II) THEN 'GERENTE_II'  ");
                        stb.Append("              WHEN @INPUTID = MAX(CL.MATRICULA_GERENTE_I) THEN 'GERENTE_I'  ");
                        stb.Append("              WHEN @INPUTID = MAX(CL.MATRICULA_DIRETOR) THEN 'DIRETOR'  ");
                        stb.Append("              WHEN @INPUTID = MAX(CL.MATRICULA_CEO) THEN 'CEO'  ");
                        stb.Append("              ELSE ''  ");
                        stb.Append("          END AS CARGO,  ");
                        stb.Append("          0 AS FACTOR0,  ");
                        stb.Append("          0 AS FACTOR1 ");
                        stb.Append("   FROM (SELECT CD.IDGDA_SECTOR,  ");
                        stb.Append("             CD.IDGDA_SUBSECTOR,  ");
                        stb.Append(" ");
                        stb.Append("   CASE WHEN CD.IDGDA_SUBSECTOR IS NOT NULL THEN CD.IDGDA_SUBSECTOR  ");
                        stb.Append("   ELSE CD.IDGDA_SECTOR END AS IDGDA_SECTOR_SUBSECTOR,  ");
                        stb.Append(" ");
                        stb.Append("             CD.CREATED_AT,  ");
                        stb.Append("             CD.IDGDA_COLLABORATORS,  ");
                        stb.Append("             CD.IDGDA_SECTOR_SUPERVISOR,  ");
                        stb.Append("             CD.ACTIVE,  ");
                        stb.Append("             CD.CARGO,  ");
                        stb.Append("             CD.HOME_BASED,  ");
                        stb.Append("             CD.SITE,  ");
                        stb.Append("             CD.IDGDA_PERIOD,  ");
                        stb.Append("             CD.MATRICULA_SUPERVISOR,  ");
                        stb.Append("             CD.NOME_SUPERVISOR,  ");
                        stb.Append("             CD.MATRICULA_COORDENADOR,  ");
                        stb.Append("             CD.NOME_COORDENADOR,  ");
                        stb.Append("             CD.MATRICULA_GERENTE_II,  ");
                        stb.Append("             CD.NOME_GERENTE_II,  ");
                        stb.Append("             CD.MATRICULA_GERENTE_I,  ");
                        stb.Append("             CD.NOME_GERENTE_I,  ");
                        stb.Append("             CD.MATRICULA_DIRETOR,  ");
                        stb.Append("             CD.NOME_DIRETOR,  ");
                        stb.Append("             CD.MATRICULA_CEO,  ");
                        stb.Append("             CD.NOME_CEO  ");
                        stb.Append("      FROM GDA_COLLABORATORS_DETAILS (NOLOCK) CD  ");
                        stb.Append("      LEFT JOIN GDA_SITE (NOLOCK) S ON CD.SITE = S.SITE  ");
                        stb.Append("      WHERE CD.CREATED_AT >= @DATAINICIAL  ");
                        stb.Append("        AND CD.CREATED_AT <= @DATAFINAL  ) AS CL  ");
                        stb.Append("   INNER JOIN GDA_INDICATOR (NOLOCK) AS IT ON IT.IDGDA_INDICATOR IN ('10000013') AND IT.DELETED_AT IS NULL AND (IT.STATUS IS NULL OR IT.STATUS = 1)  ");
                        stb.Append("   WHERE 1 = 1  ");
                        stb.Append("     AND CL.CREATED_AT >= @DATAINICIAL  ");
                        stb.Append("     AND CL.CREATED_AT <= @DATAFINAL  ");
                        stb.Append("     AND CL.IDGDA_SECTOR IS NOT NULL  ");
                        stb.Append("     AND CL.CARGO IS NOT NULL  ");
                        stb.Append("     AND CL.HOME_BASED <> ''   ");
                        stb.Append("     AND CL.ACTIVE = 'true'   ");
                        stb.Append("	 and CL.IDGDA_COLLABORATORS = @INPUTID ");
                        stb.Append("   GROUP BY IT.IDGDA_INDICATOR,  ");
                        stb.Append("            CL.IDGDA_SECTOR, CL.IDGDA_SUBSECTOR, CL.IDGDA_SECTOR_SUBSECTOR,  ");
                        stb.Append("            CONVERT(DATE, CL.CREATED_AT),  ");
                        stb.Append("            IDGDA_PERIOD) AS TBL  ");
                        stb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS IT ON IT.IDGDA_INDICATOR = TBL.INDICADORID AND (IT.STATUS IS NULL OR IT.STATUS = 1)  ");
                        stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL  ");
                        stb.Append("AND HIG1.INDICATOR_ID = TBL.INDICADORID  ");
                        stb.Append("AND HIG1.SECTOR_ID = TBL.IDGDA_SECTOR_SUBSECTOR  ");
                        stb.Append("AND HIG1.GROUPID = 1  ");
                        stb.Append("AND CONVERT(DATE,HIG1.STARTED_AT) <= TBL.CREATED_AT  ");
                        stb.Append("AND CONVERT(DATE,HIG1.ENDED_AT) >= TBL.CREATED_AT  ");
                        stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG2 ON HIG2.DELETED_AT IS NULL  ");
                        stb.Append("AND HIG2.INDICATOR_ID = TBL.INDICADORID  ");
                        stb.Append("AND HIG2.SECTOR_ID = TBL.IDGDA_SECTOR_SUBSECTOR  ");
                        stb.Append("AND HIG2.GROUPID = 2  ");
                        stb.Append("AND CONVERT(DATE,HIG2.STARTED_AT) <= TBL.CREATED_AT  ");
                        stb.Append("AND CONVERT(DATE,HIG2.ENDED_AT) >= TBL.CREATED_AT  ");
                        stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG3 ON HIG3.DELETED_AT IS NULL  ");
                        stb.Append("AND HIG3.INDICATOR_ID = TBL.INDICADORID  ");
                        stb.Append("AND HIG3.SECTOR_ID = TBL.IDGDA_SECTOR_SUBSECTOR  ");
                        stb.Append("AND HIG3.GROUPID = 3  ");
                        stb.Append("AND CONVERT(DATE,HIG3.STARTED_AT) <= TBL.CREATED_AT  ");
                        stb.Append("AND CONVERT(DATE,HIG3.ENDED_AT) >= TBL.CREATED_AT  ");
                        stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG4 ON HIG4.DELETED_AT IS NULL  ");
                        stb.Append("AND HIG4.INDICATOR_ID = TBL.INDICADORID  ");
                        stb.Append("AND HIG4.SECTOR_ID = TBL.IDGDA_SECTOR_SUBSECTOR  ");
                        stb.Append("AND HIG4.GROUPID = 4  ");
                        stb.Append("AND CONVERT(DATE,HIG4.STARTED_AT) <= TBL.CREATED_AT  ");
                        stb.Append("AND CONVERT(DATE,HIG4.ENDED_AT) >= TBL.CREATED_AT  ");
                        stb.Append("GROUP BY TBL.INDICADORID ");

                        int qtdTotal = 0;
                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    qtdTotal = Convert.ToInt32(reader["QTD_MON_TOTAL"].ToString());
                                }
                            }
                        }
                        if (qtdTotal > 0)
                        {
                            groups lgroup12 = lgroup.Find(l => l.id == 4);
                            rmams.coinsEarned = 0;
                            rmams.coinsPossible = 0;
                            rmams.groupName = lgroup12.name;
                            rmams.idGroup = lgroup12.id;
                            rmams.groupAlias = lgroup12.alias;
                            rmams.groupImage = lgroup12.image;
                            return rmams;
                        }
                        else
                        {
                            groups lgroup12 = lgroup.Find(l => l.id == 1);
                            rmams.coinsEarned = 0;
                            rmams.coinsPossible = 0;
                            rmams.groupName = lgroup12.name;
                            rmams.idGroup = lgroup12.id;
                            rmams.groupAlias = lgroup12.alias;
                            rmams.groupImage = lgroup12.image;
                            return rmams;
                        }
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


        public static List<basket> listBasketConfiguration()
        {
            List<basket> lBasket = new List<basket>();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT METRIC_MIN, GROUPID FROM GDA_HISTORY_INDICATOR_GROUP (NOLOCK) ");
            sb.Append("WHERE INDICATOR_ID = 10000012 AND DELETED_AT IS NULL ");
            sb.Append("ORDER BY 1 DESC ");


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

        public static List<groups> listGroups(string filter)
        {
            List<groups> lGroups = new List<groups>();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT G.ID, NAME, ALIAS, URL FROM GDA_GROUPS (NOLOCK) AS G ");
            sb.Append("INNER JOIN GDA_UPLOADS (NOLOCK) AS U ON G.UPLOADID = U.ID ");


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
                stb1.Append("	[IDGDA_CLIENT] [nvarchar](100) NULL,");
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
                stb1.Append("	[ACTIVE] [nvarchar](10) NULL,");
                stb1.Append("	[IDGDA_PERIOD] [INT] NULL)");

                using (SqlCommand createTempTableCommand = new SqlCommand(stb1.ToString(), connection))
                {
                    createTempTableCommand.ExecuteNonQuery();
                }



                StringBuilder temp = new StringBuilder();
                temp.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}';", dt);
                temp.Append("INSERT INTO #TEMPAG (IDGDA_COLLABORATORS, CARGO, CREATED_AT, IDGDA_SECTOR, IDGDA_SECTOR_SUPERVISOR, IDGDA_SUBSECTOR, HOME_BASED, SITE, IDGDA_CLIENT, PERIODO, MATRICULA_SUPERVISOR, NOME_SUPERVISOR, MATRICULA_COORDENADOR,  ");
                temp.Append("NOME_COORDENADOR, MATRICULA_GERENTE_II, NOME_GERENTE_II, MATRICULA_GERENTE_I, NOME_GERENTE_I, MATRICULA_DIRETOR, NOME_DIRETOR, MATRICULA_CEO, NOME_CEO, ACTIVE, IDGDA_PERIOD)  ");
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
                temp.Append("               WHEN A.NAME = 'NOME_CLIENTE' THEN CLI.IDGDA_CLIENT  ");
                temp.Append("               ELSE ''  ");
                temp.Append("           END) AS IDGDA_CLIENT,  ");
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
                temp.Append("           MAX(CAT.ACTIVE) AS ACTIVE, ");
                temp.Append("           MAX(PERI.IDGDA_PERIOD) AS IDGDA_PERIOD ");
                temp.Append(" FROM GDA_COLLABORATORS (NOLOCK) AS CB  ");
                temp.Append("LEFT JOIN GDA_HISTORY_COLLABORATOR_ACTIVE (NOLOCK) CAT ON CAT.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS AND CAT.ENTRYDATE = @DATAINICIAL ");
                temp.Append("LEFT JOIN GDA_HISTORY_COLLABORATOR_SECTOR (NOLOCK) S ON S.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS AND S.CREATED_AT = @DATAINICIAL  ");
                temp.Append("LEFT JOIN GDA_HISTORY_COLLABORATOR_SUBSECTOR (NOLOCK) SUBS ON SUBS.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS AND SUBS.CREATED_AT = @DATAINICIAL  ");
                temp.Append("LEFT JOIN GDA_ATRIBUTES (NOLOCK) AS A ON (A.NAME = 'HOME_BASED'  ");
                temp.Append("                                          OR A.NAME = 'SITE'  ");
                temp.Append("                                          OR A.NAME = 'NOME_CLIENTE' ");
                temp.Append("                                          OR A.NAME = 'PERIODO')  ");
                temp.Append("AND A.CREATED_AT = @DATAINICIAL  ");
                temp.Append("AND A.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS  ");
                temp.Append("LEFT JOIN GDA_CLIENT (NOLOCK) AS CLI ON A.VALUE = CLI.CLIENT ");
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
                temp.Append("LEFT JOIN GDA_PERIOD (NOLOCK) PERI ON PERI.PERIOD = (CASE WHEN A.NAME = 'PERIODO' THEN A.VALUE ELSE '' END) ");
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
                        queryInsertResult1.Append("  INSERT (IDGDA_COLLABORATORS, CARGO, IDGDA_SECTOR, IDGDA_SECTOR_SUPERVISOR, IDGDA_SUBSECTOR, HOME_BASED, SITE, IDGDA_CLIENT, PERIODO, MATRICULA_SUPERVISOR, NOME_SUPERVISOR, MATRICULA_COORDENADOR,  ");
                        queryInsertResult1.Append("		  NOME_COORDENADOR, MATRICULA_GERENTE_II, NOME_GERENTE_II, MATRICULA_GERENTE_I, NOME_GERENTE_I, MATRICULA_DIRETOR, NOME_DIRETOR, MATRICULA_CEO, NOME_CEO, ACTIVE)  ");
                        queryInsertResult1.Append("  VALUES (SOURCE.IDGDA_COLLABORATORS, SOURCE.CARGO, SOURCE.IDGDA_SECTOR, SOURCE.IDGDA_SECTOR_SUPERVISOR, SOURCE.IDGDA_SUBSECTOR, SOURCE.HOME_BASED, SOURCE.SITE, SOURCE.IDGDA_CLIENT, SOURCE.PERIODO, SOURCE.MATRICULA_SUPERVISOR, SOURCE.NOME_SUPERVISOR, SOURCE.MATRICULA_COORDENADOR,  ");
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
                        queryUp.Append("UPDATE D1 SET D1.IDGDA_CLIENT = D2.IDGDA_CLIENT ");
                        queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
                        queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
                        queryUp.Append("WHERE D2.IDGDA_CLIENT <> '0' ");
                        SqlCommand createnew122 = new SqlCommand(queryUp.ToString(), connection);
                        createnew122.CommandTimeout = 0;
                        createnew122.ExecuteNonQuery();

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


                    //InserirPersona
                    //if (dt == DateTime.Now.ToString("yyyy-MM-dd"))
                    //{
                    //    StringBuilder strPersona = new StringBuilder();
                    //    strPersona.Append("UPDATE GDA_PERSONA_USER_DETAILS  ");
                    //    strPersona.Append("SET  ");
                    //    strPersona.Append("    SITE = S.IDGDA_SITE ");
                    //    strPersona.Append("FROM  ");
                    //    strPersona.Append("    GDA_PERSONA_USER_DETAILS UD ");
                    //    strPersona.Append("INNER JOIN  ");
                    //    strPersona.Append("    GDA_PERSONA_COLLABORATOR_USER CU ON UD.IDGDA_PERSONA_USER = CU.IDGDA_PERSONA_USER ");
                    //    strPersona.Append("INNER JOIN  ");
                    //    strPersona.Append("    #TEMPAG C ON C.IDGDA_COLLABORATORS = CU.IDGDA_COLLABORATORS ");
                    //    strPersona.Append("LEFT JOIN  ");
                    //    strPersona.Append("    GDA_SITE S ON C.SITE = C.SITE ");

                    //    SqlCommand createPersona = new SqlCommand(strPersona.ToString(), connection);
                    //    createPersona.CommandTimeout = 0;
                    //    createPersona.ExecuteNonQuery();
                    //}



                    //A
                    StringBuilder queryInsertResult2 = new StringBuilder();
                    queryInsertResult2.Append("MERGE INTO GDA_COLLABORATORS_DETAILS AS TARGET  ");
                    queryInsertResult2.Append("USING #TEMPAG AS SOURCE  ");
                    queryInsertResult2.Append("ON (TARGET.IDGDA_COLLABORATORS = SOURCE.IDGDA_COLLABORATORS AND TARGET.CREATED_AT = SOURCE.CREATED_AT)  ");
                    queryInsertResult2.Append("WHEN NOT MATCHED BY TARGET THEN  ");
                    queryInsertResult2.Append("  INSERT (IDGDA_COLLABORATORS, CARGO, CREATED_AT, IDGDA_SECTOR, IDGDA_SECTOR_SUPERVISOR, IDGDA_SUBSECTOR, HOME_BASED, SITE, IDGDA_CLIENT, PERIODO, MATRICULA_SUPERVISOR, NOME_SUPERVISOR, MATRICULA_COORDENADOR,  ");
                    queryInsertResult2.Append("		  NOME_COORDENADOR, MATRICULA_GERENTE_II, NOME_GERENTE_II, MATRICULA_GERENTE_I, NOME_GERENTE_I, MATRICULA_DIRETOR, NOME_DIRETOR, MATRICULA_CEO, NOME_CEO, ACTIVE, IDGDA_PERIOD)  ");
                    queryInsertResult2.Append("  VALUES (SOURCE.IDGDA_COLLABORATORS, SOURCE.CARGO, SOURCE.CREATED_AT, SOURCE.IDGDA_SECTOR, SOURCE.IDGDA_SECTOR_SUPERVISOR, SOURCE.IDGDA_SUBSECTOR, SOURCE.HOME_BASED, SOURCE.SITE, SOURCE.IDGDA_CLIENT, SOURCE.PERIODO, SOURCE.MATRICULA_SUPERVISOR, SOURCE.NOME_SUPERVISOR, SOURCE.MATRICULA_COORDENADOR,  ");
                    queryInsertResult2.Append("		  SOURCE.NOME_COORDENADOR, SOURCE.MATRICULA_GERENTE_II, SOURCE.NOME_GERENTE_II, SOURCE.MATRICULA_GERENTE_I, SOURCE.NOME_GERENTE_I, SOURCE.MATRICULA_DIRETOR, SOURCE.NOME_DIRETOR, SOURCE.MATRICULA_CEO, SOURCE.NOME_CEO, SOURCE.ACTIVE, SOURCE.IDGDA_PERIOD)  ");
                    queryInsertResult2.Append("WHEN MATCHED THEN  ");
                    queryInsertResult2.Append("  UPDATE SET  ");
                    queryInsertResult2.Append("  TARGET.CARGO = SOURCE.CARGO, ");
                    queryInsertResult2.Append("  TARGET.IDGDA_SECTOR = SOURCE.IDGDA_SECTOR, ");
                    queryInsertResult2.Append("  TARGET.IDGDA_SECTOR_SUPERVISOR = SOURCE.IDGDA_SECTOR_SUPERVISOR, ");
                    queryInsertResult2.Append("  TARGET.IDGDA_SUBSECTOR = SOURCE.IDGDA_SUBSECTOR, ");
                    queryInsertResult2.Append("  TARGET.HOME_BASED = SOURCE.HOME_BASED, ");
                    queryInsertResult2.Append("  TARGET.SITE = SOURCE.SITE, ");
                    queryInsertResult2.Append("  TARGET.IDGDA_CLIENT = SOURCE.IDGDA_CLIENT, ");
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
                    queryInsertResult2.Append("  TARGET.ACTIVE = SOURCE.ACTIVE, ");
                    queryInsertResult2.Append("  TARGET.IDGDA_PERIOD = SOURCE.IDGDA_PERIOD; ");

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

        //public void insereTabelasAuxiliares(string dt)
        //{
        //    //string connectionString = "Data Source=database-1.cpmxhjwr8mgp.us-east-1.rds.amazonaws.com;Initial Catalog=GUILDA_PROD;User ID=RDSadminsql2019;Password=H4ND4faG3AhjFe943NyPLRhMsEamXucKdDa;MultipleActiveResultSets=True;Connection Timeout=10";
        //    SqlConnection connection = new SqlConnection(Database.Conn);
        //    try
        //    {
        //        connection.Open();
        //        StringBuilder stb1 = new StringBuilder();
        //        stb1.Append("CREATE TABLE [dbo].[#TEMPAG]( ");
        //        stb1.Append("	[IDGDA_COLLABORATORS] [int] NULL,");
        //        stb1.Append("   [CARGO] [nvarchar](50) NULL,");
        //        stb1.Append("   [CREATED_AT] [datetime2](7) NULL,");
        //        stb1.Append("	[IDGDA_SECTOR] [int] NULL,");
        //        stb1.Append("	[IDGDA_SECTOR_SUPERVISOR] [int] NULL,");
        //        stb1.Append("	[IDGDA_SUBSECTOR] [int] NULL,");
        //        stb1.Append("	[HOME_BASED] [nvarchar](100) NULL,");
        //        stb1.Append("	[SITE] [nvarchar](100) NULL,");
        //        stb1.Append("	[PERIODO] [nvarchar](100) NULL,");
        //        stb1.Append("	[MATRICULA_SUPERVISOR] [int] NULL,");
        //        stb1.Append("	[NOME_SUPERVISOR] [nvarchar](200) NULL,");
        //        stb1.Append("	[MATRICULA_COORDENADOR] [int] NULL,");
        //        stb1.Append("	[NOME_COORDENADOR] [nvarchar](200) NULL,");
        //        stb1.Append("	[MATRICULA_GERENTE_II] [int] NULL,");
        //        stb1.Append("	[NOME_GERENTE_II] [nvarchar](200) NULL,");
        //        stb1.Append("	[MATRICULA_GERENTE_I] [int] NULL,");
        //        stb1.Append("	[NOME_GERENTE_I] [nvarchar](200) NULL,");
        //        stb1.Append("	[MATRICULA_DIRETOR] [int] NULL,");
        //        stb1.Append("	[NOME_DIRETOR] [nvarchar](200) NULL,");
        //        stb1.Append("	[MATRICULA_CEO] [int] NULL,");
        //        stb1.Append("	[NOME_CEO] [nvarchar](200) NULL,");
        //        stb1.Append("	[ACTIVE] [nvarchar](10) NULL)");

        //        using (SqlCommand createTempTableCommand = new SqlCommand(stb1.ToString(), connection))
        //        {
        //            createTempTableCommand.ExecuteNonQuery();
        //        }



        //        StringBuilder temp = new StringBuilder();
        //        temp.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}';", dt);
        //        temp.Append("INSERT INTO #TEMPAG (IDGDA_COLLABORATORS, CARGO, CREATED_AT, IDGDA_SECTOR, IDGDA_SECTOR_SUPERVISOR, IDGDA_SUBSECTOR, HOME_BASED, SITE, PERIODO, MATRICULA_SUPERVISOR, NOME_SUPERVISOR, MATRICULA_COORDENADOR,  ");
        //        temp.Append("NOME_COORDENADOR, MATRICULA_GERENTE_II, NOME_GERENTE_II, MATRICULA_GERENTE_I, NOME_GERENTE_I, MATRICULA_DIRETOR, NOME_DIRETOR, MATRICULA_CEO, NOME_CEO, ACTIVE)  ");
        //        temp.Append("SELECT CB.IDGDA_COLLABORATORS, MAX(CLS.LEVELNAME), @DATAINICIAL, MAX(S.IDGDA_SECTOR) AS IDGDA_SECTOR, MAX(SSUPER.IDGDA_SECTOR) AS IDGDA_SECTOR_SUPER, MAX(SUBS.IDGDA_SECTOR) AS IDGDA_SUBSECTOR, ");
        //        temp.Append("	   MAX(CASE  ");
        //        temp.Append("               WHEN A.NAME = 'HOME_BASED' THEN A.VALUE  ");
        //        temp.Append("               ELSE ''  ");
        //        temp.Append("           END) AS HOME_BASED,  ");
        //        temp.Append("       MAX(CASE  ");
        //        temp.Append("               WHEN A.NAME = 'SITE' THEN A.VALUE  ");
        //        temp.Append("               ELSE ''  ");
        //        temp.Append("           END) AS SITE,  ");
        //        temp.Append("       MAX(CASE  ");
        //        temp.Append("               WHEN A.NAME = 'PERIODO' THEN A.VALUE  ");
        //        temp.Append("               ELSE ''  ");
        //        temp.Append("           END) AS TURNO,  ");
        //        temp.Append("       MAX(CASE  ");
        //        temp.Append("               WHEN HIERARCHY.LEVELWEIGHT = '2' THEN CAST(HIERARCHY.PARENTIDENTIFICATION AS VARCHAR(255))  ");
        //        temp.Append("               ELSE '-'  ");
        //        temp.Append("           END) AS 'MATRICULA SUPERVISOR',  ");
        //        temp.Append("       MAX(CASE  ");
        //        temp.Append("               WHEN HIERARCHY.LEVELWEIGHT = '2' THEN HIERARCHY.NAME  ");
        //        temp.Append("               ELSE '-'  ");
        //        temp.Append("           END) AS 'NOME SUPERVISOR',  ");
        //        temp.Append("       MAX(CASE  ");
        //        temp.Append("               WHEN HIERARCHY.LEVELWEIGHT = '3' THEN CAST(HIERARCHY.PARENTIDENTIFICATION AS VARCHAR(255))  ");
        //        temp.Append("               ELSE '-'  ");
        //        temp.Append("           END) AS 'MATRICULA COORDENADOR',  ");
        //        temp.Append("       MAX(CASE  ");
        //        temp.Append("               WHEN HIERARCHY.LEVELWEIGHT = '3' THEN HIERARCHY.NAME  ");
        //        temp.Append("               ELSE '-'  ");
        //        temp.Append("           END) AS 'NOME COORDENADOR',  ");
        //        temp.Append("       MAX(CASE  ");
        //        temp.Append("               WHEN HIERARCHY.LEVELWEIGHT = '4' THEN CAST(HIERARCHY.PARENTIDENTIFICATION AS VARCHAR(255))  ");
        //        temp.Append("               ELSE '-'  ");
        //        temp.Append("           END) AS 'MATRICULA GERENTE II',  ");
        //        temp.Append("       MAX(CASE  ");
        //        temp.Append("               WHEN HIERARCHY.LEVELWEIGHT = '4' THEN HIERARCHY.NAME  ");
        //        temp.Append("               ELSE '-'  ");
        //        temp.Append("           END) AS 'NOME GERENTE II',  ");
        //        temp.Append("       MAX(CASE  ");
        //        temp.Append("               WHEN HIERARCHY.LEVELWEIGHT = '5' THEN CAST(HIERARCHY.PARENTIDENTIFICATION AS VARCHAR(255))  ");
        //        temp.Append("               ELSE '-'  ");
        //        temp.Append("           END) AS 'MATRICULA GERENTE I',  ");
        //        temp.Append("       MAX(CASE  ");
        //        temp.Append("               WHEN HIERARCHY.LEVELWEIGHT = '5' THEN HIERARCHY.NAME  ");
        //        temp.Append("               ELSE '-'  ");
        //        temp.Append("           END) AS 'NOME GERENTE I',  ");
        //        temp.Append("       MAX(CASE  ");
        //        temp.Append("               WHEN HIERARCHY.LEVELWEIGHT = '6' THEN CAST(HIERARCHY.PARENTIDENTIFICATION AS VARCHAR(255))  ");
        //        temp.Append("               ELSE '-'  ");
        //        temp.Append("           END) AS 'MATRICULA DIRETOR',  ");
        //        temp.Append("       MAX(CASE  ");
        //        temp.Append("               WHEN HIERARCHY.LEVELWEIGHT = '6' THEN HIERARCHY.NAME  ");
        //        temp.Append("               ELSE '-'  ");
        //        temp.Append("           END) AS 'NOME DIRETOR',  ");
        //        temp.Append("       MAX(CASE  ");
        //        temp.Append("               WHEN HIERARCHY.LEVELWEIGHT = '7' THEN HIERARCHY.PARENTIDENTIFICATION  ");
        //        temp.Append("               ELSE '-'  ");
        //        temp.Append("           END) AS 'MATRICULA CEO',  ");
        //        temp.Append("       MAX(CASE  ");
        //        temp.Append("               WHEN HIERARCHY.LEVELWEIGHT = '7' THEN HIERARCHY.NAME  ");
        //        temp.Append("               ELSE '-'  ");
        //        temp.Append("           END) AS 'NOME CEO',  ");
        //        temp.Append("           MAX(CAT.ACTIVE) AS ACTIVE ");
        //        temp.Append(" FROM GDA_COLLABORATORS (NOLOCK) AS CB  ");
        //        temp.Append("LEFT JOIN GDA_HISTORY_COLLABORATOR_ACTIVE (NOLOCK) CAT ON CAT.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS AND CAT.ENTRYDATE = @DATAINICIAL ");
        //        temp.Append("LEFT JOIN GDA_HISTORY_COLLABORATOR_SECTOR (NOLOCK) S ON S.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS AND S.CREATED_AT = @DATAINICIAL  ");
        //        temp.Append("LEFT JOIN GDA_HISTORY_COLLABORATOR_SUBSECTOR (NOLOCK) SUBS ON SUBS.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS AND SUBS.CREATED_AT = @DATAINICIAL  ");
        //        temp.Append("LEFT JOIN GDA_ATRIBUTES (NOLOCK) AS A ON (A.NAME = 'HOME_BASED'  ");
        //        temp.Append("                                          OR A.NAME = 'SITE'  ");
        //        temp.Append("                                          OR A.NAME = 'PERIODO')  ");
        //        temp.Append("AND A.CREATED_AT = @DATAINICIAL  ");
        //        temp.Append("AND A.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS  ");
        //        temp.Append("LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS CLS ON CLS.IDGDA_COLLABORATORS = CB.IDGDA_COLLABORATORS ");
        //        temp.Append("AND CLS.DATE = @DATAINICIAL ");
        //        temp.Append("LEFT JOIN  ");
        //        temp.Append("  (SELECT COD,  ");
        //        temp.Append("          IDGDA_COLLABORATORS,  ");
        //        temp.Append("          PARENTIDENTIFICATION,  ");
        //        temp.Append("          NAME,  ");
        //        temp.Append("          LEVELWEIGHT  ");
        //        temp.Append("   FROM  ");
        //        temp.Append("     (SELECT LV1.IDGDA_COLLABORATORS AS COD,  ");
        //        temp.Append("             LV1.IDGDA_COLLABORATORS,  ");
        //        temp.Append("             ISNULL(LV1.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION,  ");
        //        temp.Append("             C.NAME,  ");
        //        temp.Append("             CASE  ");
        //        temp.Append("                 WHEN LV2.LEVELWEIGHT IS NULL  ");
        //        temp.Append("                      AND LV1.PARENTIDENTIFICATION IS NOT NULL THEN '-'  ");
        //        temp.Append("                 ELSE LV2.LEVELWEIGHT  ");
        //        temp.Append("             END AS LEVELWEIGHT  ");
        //        temp.Append("      FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1  ");
        //        temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS  ");
        //        temp.Append("      AND LV2.DATE = @DATAINICIAL  ");
        //        temp.Append("      LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV1.PARENTIDENTIFICATION  ");
        //        temp.Append("      WHERE LV1.DATE = @DATAINICIAL  ");
        //        temp.Append("      UNION ALL SELECT LV1.IDGDA_COLLABORATORS AS COD,  ");
        //        temp.Append("                       LV2.IDGDA_COLLABORATORS,  ");
        //        temp.Append("                       ISNULL(LV2.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION,  ");
        //        temp.Append("                       C.NAME,  ");
        //        temp.Append("                       CASE  ");
        //        temp.Append("                           WHEN LV3.LEVELWEIGHT IS NULL  ");
        //        temp.Append("                                AND LV2.PARENTIDENTIFICATION IS NOT NULL THEN '-'  ");
        //        temp.Append("                           ELSE LV3.LEVELWEIGHT  ");
        //        temp.Append("                       END AS LEVELWEIGHT  ");
        //        temp.Append("      FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1  ");
        //        temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS  ");
        //        temp.Append("      AND LV2.DATE = @DATAINICIAL  ");
        //        temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS  ");
        //        temp.Append("      AND LV3.DATE = @DATAINICIAL  ");
        //        temp.Append("      LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV2.PARENTIDENTIFICATION  ");
        //        temp.Append("      WHERE LV1.DATE = @DATAINICIAL  ");
        //        temp.Append("      UNION ALL SELECT LV1.IDGDA_COLLABORATORS AS COD,  ");
        //        temp.Append("                       LV3.IDGDA_COLLABORATORS,  ");
        //        temp.Append("                       ISNULL(LV3.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION,  ");
        //        temp.Append("                       C.NAME,  ");
        //        temp.Append("                       CASE  ");
        //        temp.Append("                           WHEN LV4.LEVELWEIGHT IS NULL  ");
        //        temp.Append("                                AND LV3.PARENTIDENTIFICATION IS NOT NULL THEN '-'  ");
        //        temp.Append("                           ELSE LV4.LEVELWEIGHT  ");
        //        temp.Append("                       END AS LEVELWEIGHT  ");
        //        temp.Append("      FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1  ");
        //        temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS  ");
        //        temp.Append("      AND LV2.DATE = @DATAINICIAL  ");
        //        temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS  ");
        //        temp.Append("      AND LV3.DATE = @DATAINICIAL  ");
        //        temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV4 ON LV3.PARENTIDENTIFICATION = LV4.IDGDA_COLLABORATORS  ");
        //        temp.Append("      AND LV4.DATE = @DATAINICIAL  ");
        //        temp.Append("      LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV3.PARENTIDENTIFICATION  ");
        //        temp.Append("      WHERE LV1.DATE = @DATAINICIAL  ");
        //        temp.Append("      UNION ALL SELECT LV1.IDGDA_COLLABORATORS AS COD,  ");
        //        temp.Append("                       LV4.IDGDA_COLLABORATORS,  ");
        //        temp.Append("                       ISNULL(LV4.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION,  ");
        //        temp.Append("                       C.NAME,  ");
        //        temp.Append("                       CASE  ");
        //        temp.Append("                           WHEN LV5.LEVELWEIGHT IS NULL  ");
        //        temp.Append("                                AND LV4.PARENTIDENTIFICATION IS NOT NULL THEN '-'  ");
        //        temp.Append("                           ELSE LV5.LEVELWEIGHT  ");
        //        temp.Append("                       END AS LEVELWEIGHT  ");
        //        temp.Append("      FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1  ");
        //        temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS  ");
        //        temp.Append("      AND LV2.DATE = @DATAINICIAL  ");
        //        temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS  ");
        //        temp.Append("      AND LV3.DATE = @DATAINICIAL  ");
        //        temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV4 ON LV3.PARENTIDENTIFICATION = LV4.IDGDA_COLLABORATORS  ");
        //        temp.Append("      AND LV4.DATE = @DATAINICIAL  ");
        //        temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV5 ON LV4.PARENTIDENTIFICATION = LV5.IDGDA_COLLABORATORS  ");
        //        temp.Append("      AND LV5.DATE = @DATAINICIAL  ");
        //        temp.Append("      LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV4.PARENTIDENTIFICATION  ");
        //        temp.Append("      WHERE LV1.DATE = @DATAINICIAL  ");
        //        temp.Append("      UNION ALL SELECT LV1.IDGDA_COLLABORATORS AS COD,  ");
        //        temp.Append("                       LV5.IDGDA_COLLABORATORS,  ");
        //        temp.Append("                       ISNULL(LV5.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION,  ");
        //        temp.Append("                       C.NAME,  ");
        //        temp.Append("                       CASE  ");
        //        temp.Append("                           WHEN LV6.LEVELWEIGHT IS NULL  ");
        //        temp.Append("                                AND LV5.PARENTIDENTIFICATION IS NOT NULL THEN '-'  ");
        //        temp.Append("                           ELSE LV6.LEVELWEIGHT  ");
        //        temp.Append("                       END AS LEVELWEIGHT  ");
        //        temp.Append("      FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1  ");
        //        temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS  ");
        //        temp.Append("      AND LV2.DATE = @DATAINICIAL  ");
        //        temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS  ");
        //        temp.Append("      AND LV3.DATE = @DATAINICIAL  ");
        //        temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV4 ON LV3.PARENTIDENTIFICATION = LV4.IDGDA_COLLABORATORS  ");
        //        temp.Append("      AND LV4.DATE = @DATAINICIAL  ");
        //        temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV5 ON LV4.PARENTIDENTIFICATION = LV5.IDGDA_COLLABORATORS  ");
        //        temp.Append("      AND LV5.DATE = @DATAINICIAL  ");
        //        temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV6 ON LV5.PARENTIDENTIFICATION = LV6.IDGDA_COLLABORATORS  ");
        //        temp.Append("      AND LV6.DATE = @DATAINICIAL  ");
        //        temp.Append("      LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV5.PARENTIDENTIFICATION  ");
        //        temp.Append("      WHERE LV1.DATE = @DATAINICIAL  ");
        //        temp.Append("      UNION ALL SELECT LV1.IDGDA_COLLABORATORS AS COD,  ");
        //        temp.Append("                       LV6.IDGDA_COLLABORATORS,  ");
        //        temp.Append("                       ISNULL(LV6.PARENTIDENTIFICATION, '') AS PARENTIDENTIFICATION,  ");
        //        temp.Append("                       C.NAME,  ");
        //        temp.Append("                       CASE  ");
        //        temp.Append("                           WHEN LV7.LEVELWEIGHT IS NULL  ");
        //        temp.Append("                                AND LV6.PARENTIDENTIFICATION IS NOT NULL THEN '-'  ");
        //        temp.Append("                           ELSE LV7.LEVELWEIGHT  ");
        //        temp.Append("                       END AS LEVELWEIGHT  ");
        //        temp.Append("      FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV1  ");
        //        temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV2 ON LV1.PARENTIDENTIFICATION = LV2.IDGDA_COLLABORATORS  ");
        //        temp.Append("      AND LV2.DATE = @DATAINICIAL  ");
        //        temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV3 ON LV2.PARENTIDENTIFICATION = LV3.IDGDA_COLLABORATORS  ");
        //        temp.Append("      AND LV3.DATE = @DATAINICIAL  ");
        //        temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV4 ON LV3.PARENTIDENTIFICATION = LV4.IDGDA_COLLABORATORS  ");
        //        temp.Append("      AND LV4.DATE = @DATAINICIAL  ");
        //        temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV5 ON LV4.PARENTIDENTIFICATION = LV5.IDGDA_COLLABORATORS  ");
        //        temp.Append("      AND LV5.DATE = @DATAINICIAL  ");
        //        temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV6 ON LV5.PARENTIDENTIFICATION = LV6.IDGDA_COLLABORATORS  ");
        //        temp.Append("      AND LV6.DATE = @DATAINICIAL  ");
        //        temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV7 ON LV6.PARENTIDENTIFICATION = LV7.IDGDA_COLLABORATORS  ");
        //        temp.Append("      AND LV7.DATE = @DATAINICIAL  ");
        //        temp.Append("      LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS LV8 ON LV7.PARENTIDENTIFICATION = LV8.IDGDA_COLLABORATORS  ");
        //        temp.Append("      AND LV8.DATE = @DATAINICIAL  ");
        //        temp.Append("      LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = LV6.PARENTIDENTIFICATION  ");
        //        temp.Append("      WHERE LV1.DATE = @DATAINICIAL ) AS COMBINEDDATA) AS HIERARCHY ON HIERARCHY.COD = CB.IDGDA_COLLABORATORS  ");

        //        temp.Append("LEFT JOIN GDA_HISTORY_COLLABORATOR_SECTOR (NOLOCK) SSUPER ON HIERARCHY.LEVELWEIGHT = '2' AND SSUPER.IDGDA_COLLABORATORS = HIERARCHY.PARENTIDENTIFICATION ");
        //        temp.Append("AND SSUPER.CREATED_AT = @DATAINICIAL ");

        //        temp.Append("	  WHERE 1=1  ");
        //        temp.Append("	  GROUP BY CB.IDGDA_COLLABORATORS; ");

        //        SqlCommand createTableCommandteste = new SqlCommand(temp.ToString(), connection);
        //        createTableCommandteste.CommandTimeout = 0;
        //        createTableCommandteste.ExecuteNonQuery();

        //        //Verifica se tem informações para serem atualizadas.. caso esteja rodando em um horario que não tenha ainda hieraquia ou atributos
        //        StringBuilder stb3 = new StringBuilder();
        //        stb3.Append("SELECT COUNT(0) AS QTD FROM #TEMPAG ");
        //        stb3.Append("WHERE IDGDA_SECTOR IS NOT NULL AND MATRICULA_SUPERVISOR <> 0 ");
        //        int qtd = 0;
        //        using (SqlCommand command = new SqlCommand(stb3.ToString(), connection))
        //        {
        //            command.CommandTimeout = 60;
        //            using (SqlDataReader reader = command.ExecuteReader())
        //            {
        //                if (reader.Read())
        //                {
        //                    qtd = Convert.ToInt32(reader["QTD"].ToString());
        //                }
        //            }
        //        }

        //        if (qtd > 0)
        //        {
        //            //Verifica se não é data de reprocessamento
        //            if (dt == DateTime.Now.ToString("yyyy-MM-dd"))
        //            {
        //                //Atualizar deleted_at dos resultados que tiveram modificação [DESCOMENTAR QUANDO EXISTIR COLUNA]
        //                StringBuilder queryInsertResult1 = new StringBuilder();
        //                queryInsertResult1.Append("MERGE INTO GDA_COLLABORATORS_LAST_DETAILS AS TARGET  ");
        //                queryInsertResult1.Append("USING #TEMPAG AS SOURCE  ");
        //                queryInsertResult1.Append("ON (TARGET.IDGDA_COLLABORATORS = SOURCE.IDGDA_COLLABORATORS)  ");
        //                queryInsertResult1.Append("WHEN NOT MATCHED BY TARGET THEN  ");
        //                queryInsertResult1.Append("  INSERT (IDGDA_COLLABORATORS, CARGO, IDGDA_SECTOR, IDGDA_SECTOR_SUPERVISOR, IDGDA_SUBSECTOR, HOME_BASED, SITE, PERIODO, MATRICULA_SUPERVISOR, NOME_SUPERVISOR, MATRICULA_COORDENADOR,  ");
        //                queryInsertResult1.Append("		  NOME_COORDENADOR, MATRICULA_GERENTE_II, NOME_GERENTE_II, MATRICULA_GERENTE_I, NOME_GERENTE_I, MATRICULA_DIRETOR, NOME_DIRETOR, MATRICULA_CEO, NOME_CEO, ACTIVE)  ");
        //                queryInsertResult1.Append("  VALUES (SOURCE.IDGDA_COLLABORATORS, SOURCE.CARGO, SOURCE.IDGDA_SECTOR, SOURCE.IDGDA_SECTOR_SUPERVISOR, SOURCE.IDGDA_SUBSECTOR, SOURCE.HOME_BASED, SOURCE.SITE, SOURCE.PERIODO, SOURCE.MATRICULA_SUPERVISOR, SOURCE.NOME_SUPERVISOR, SOURCE.MATRICULA_COORDENADOR,  ");
        //                queryInsertResult1.Append("		  SOURCE.NOME_COORDENADOR, SOURCE.MATRICULA_GERENTE_II, SOURCE.NOME_GERENTE_II, SOURCE.MATRICULA_GERENTE_I, SOURCE.NOME_GERENTE_I, SOURCE.MATRICULA_DIRETOR, SOURCE.NOME_DIRETOR, SOURCE.MATRICULA_CEO, SOURCE.NOME_CEO, SOURCE.ACTIVE);  ");
        //                //queryInsertResult1.Append("WHEN MATCHED THEN  ");
        //                //queryInsertResult1.Append("  UPDATE SET  ");
        //                //queryInsertResult1.Append("  TARGET.IDGDA_SECTOR = SOURCE.IDGDA_SECTOR, ");
        //                //queryInsertResult1.Append("  TARGET.HOME_BASED = SOURCE.HOME_BASED, ");
        //                //queryInsertResult1.Append("  TARGET.SITE = SOURCE.SITE, ");
        //                //queryInsertResult1.Append("  TARGET.PERIODO = SOURCE.PERIODO, ");
        //                //queryInsertResult1.Append("  TARGET.MATRICULA_SUPERVISOR = SOURCE.MATRICULA_SUPERVISOR, ");
        //                //queryInsertResult1.Append("  TARGET.NOME_SUPERVISOR = SOURCE.NOME_SUPERVISOR, ");
        //                //queryInsertResult1.Append("  TARGET.MATRICULA_COORDENADOR = SOURCE.MATRICULA_COORDENADOR, ");
        //                //queryInsertResult1.Append("  TARGET.NOME_COORDENADOR = SOURCE.NOME_COORDENADOR, ");
        //                //queryInsertResult1.Append("  TARGET.MATRICULA_GERENTE_II = SOURCE.MATRICULA_GERENTE_II, ");
        //                //queryInsertResult1.Append("  TARGET.NOME_GERENTE_II = SOURCE.NOME_GERENTE_II, ");
        //                //queryInsertResult1.Append("  TARGET.MATRICULA_GERENTE_I = SOURCE.MATRICULA_GERENTE_I, ");
        //                //queryInsertResult1.Append("  TARGET.NOME_GERENTE_I = SOURCE.NOME_GERENTE_I, ");
        //                //queryInsertResult1.Append("  TARGET.MATRICULA_DIRETOR = SOURCE.MATRICULA_DIRETOR, ");
        //                //queryInsertResult1.Append("  TARGET.NOME_DIRETOR = SOURCE.NOME_DIRETOR, ");
        //                //queryInsertResult1.Append("  TARGET.MATRICULA_CEO = SOURCE.MATRICULA_CEO, ");
        //                //queryInsertResult1.Append("  TARGET.NOME_CEO = SOURCE.NOME_CEO; ");

        //                SqlCommand createTableCommand1 = new SqlCommand(queryInsertResult1.ToString(), connection);
        //                createTableCommand1.CommandTimeout = 0;
        //                createTableCommand1.ExecuteNonQuery();

        //                StringBuilder queryUp = new StringBuilder();
        //                queryUp.Append("UPDATE D1 SET D1.CARGO = D2.CARGO ");
        //                queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
        //                queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
        //                queryUp.Append("WHERE D2.CARGO IS NOT NULL ");
        //                SqlCommand createnews1 = new SqlCommand(queryUp.ToString(), connection);
        //                createnews1.CommandTimeout = 0;
        //                createnews1.ExecuteNonQuery();

        //                queryUp.Clear();
        //                queryUp.Append("UPDATE D1 SET D1.IDGDA_SECTOR = D2.IDGDA_SECTOR ");
        //                queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
        //                queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
        //                queryUp.Append("WHERE D2.IDGDA_SECTOR IS NOT NULL ");
        //                SqlCommand createnew1 = new SqlCommand(queryUp.ToString(), connection);
        //                createnew1.CommandTimeout = 0;
        //                createnew1.ExecuteNonQuery();

        //                queryUp.Clear();
        //                queryUp.Append("UPDATE D1 SET D1.IDGDA_SECTOR_SUPERVISOR = D2.IDGDA_SECTOR_SUPERVISOR ");
        //                queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
        //                queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
        //                queryUp.Append("WHERE D2.IDGDA_SECTOR_SUPERVISOR IS NOT NULL ");
        //                SqlCommand createnewS = new SqlCommand(queryUp.ToString(), connection);
        //                createnewS.CommandTimeout = 0;
        //                createnewS.ExecuteNonQuery();

        //                queryUp.Clear();
        //                queryUp.Append("UPDATE D1 SET D1.IDGDA_SUBSECTOR = D2.IDGDA_SUBSECTOR ");
        //                queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
        //                queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
        //                queryUp.Append("WHERE D2.IDGDA_SUBSECTOR IS NOT NULL ");
        //                SqlCommand createnewSUB = new SqlCommand(queryUp.ToString(), connection);
        //                createnewSUB.CommandTimeout = 0;
        //                createnewSUB.ExecuteNonQuery();

        //                queryUp.Clear();
        //                queryUp.Append("UPDATE D1 SET D1.HOME_BASED = D2.HOME_BASED ");
        //                queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
        //                queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
        //                queryUp.Append("WHERE D2.HOME_BASED <> '' ");
        //                SqlCommand createnew2 = new SqlCommand(queryUp.ToString(), connection);
        //                createnew2.CommandTimeout = 0;
        //                createnew2.ExecuteNonQuery();

        //                queryUp.Clear();
        //                queryUp.Append("UPDATE D1 SET D1.SITE = D2.SITE ");
        //                queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
        //                queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
        //                queryUp.Append("WHERE D2.SITE <> '' ");
        //                SqlCommand createnew3 = new SqlCommand(queryUp.ToString(), connection);
        //                createnew3.CommandTimeout = 0;
        //                createnew3.ExecuteNonQuery();

        //                queryUp.Clear();
        //                queryUp.Append("UPDATE D1 SET D1.PERIODO = D2.PERIODO ");
        //                queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
        //                queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
        //                queryUp.Append("WHERE D2.PERIODO <> '' ");
        //                SqlCommand createnew4 = new SqlCommand(queryUp.ToString(), connection);
        //                createnew4.CommandTimeout = 0;
        //                createnew4.ExecuteNonQuery();


        //                queryUp.Clear();
        //                queryUp.Append("UPDATE D1 SET D1.MATRICULA_SUPERVISOR = D2.MATRICULA_SUPERVISOR ");
        //                queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
        //                queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
        //                queryUp.Append("WHERE D2.MATRICULA_SUPERVISOR <> '0' ");
        //                SqlCommand createUp1 = new SqlCommand(queryUp.ToString(), connection);
        //                createUp1.CommandTimeout = 0;
        //                createUp1.ExecuteNonQuery();

        //                queryUp.Clear();
        //                queryUp.Append("UPDATE D1 SET D1.NOME_SUPERVISOR = D2.NOME_SUPERVISOR ");
        //                queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
        //                queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
        //                queryUp.Append("WHERE D2.MATRICULA_SUPERVISOR <> '0' ");
        //                SqlCommand createUp2 = new SqlCommand(queryUp.ToString(), connection);
        //                createUp2.CommandTimeout = 0;
        //                createUp2.ExecuteNonQuery();

        //                queryUp.Clear();
        //                queryUp.Append("UPDATE D1 SET D1.MATRICULA_COORDENADOR = D2.MATRICULA_COORDENADOR ");
        //                queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
        //                queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
        //                queryUp.Append("WHERE D2.MATRICULA_SUPERVISOR <> '0' ");
        //                SqlCommand createUp3 = new SqlCommand(queryUp.ToString(), connection);
        //                createUp3.CommandTimeout = 0;
        //                createUp3.ExecuteNonQuery();

        //                queryUp.Clear();
        //                queryUp.Append("UPDATE D1 SET D1.NOME_COORDENADOR = D2.NOME_COORDENADOR ");
        //                queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
        //                queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
        //                queryUp.Append("WHERE D2.MATRICULA_SUPERVISOR <> '0' ");
        //                SqlCommand createUp4 = new SqlCommand(queryUp.ToString(), connection);
        //                createUp4.CommandTimeout = 0;
        //                createUp4.ExecuteNonQuery();

        //                queryUp.Clear();
        //                queryUp.Append("UPDATE D1 SET D1.MATRICULA_GERENTE_II = D2.MATRICULA_GERENTE_II ");
        //                queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
        //                queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
        //                queryUp.Append("WHERE D2.MATRICULA_SUPERVISOR <> '0' ");
        //                SqlCommand createUp5 = new SqlCommand(queryUp.ToString(), connection);
        //                createUp5.CommandTimeout = 0;
        //                createUp5.ExecuteNonQuery();

        //                queryUp.Clear();
        //                queryUp.Append("UPDATE D1 SET D1.NOME_GERENTE_II = D2.NOME_GERENTE_II ");
        //                queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
        //                queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
        //                queryUp.Append("WHERE D2.MATRICULA_SUPERVISOR <> '0' ");
        //                SqlCommand createUp6 = new SqlCommand(queryUp.ToString(), connection);
        //                createUp6.CommandTimeout = 0;
        //                createUp6.ExecuteNonQuery();

        //                queryUp.Clear();
        //                queryUp.Append("UPDATE D1 SET D1.MATRICULA_GERENTE_I = D2.MATRICULA_GERENTE_I ");
        //                queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
        //                queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
        //                queryUp.Append("WHERE D2.MATRICULA_SUPERVISOR <> '0' ");
        //                SqlCommand createUp7 = new SqlCommand(queryUp.ToString(), connection);
        //                createUp7.CommandTimeout = 0;
        //                createUp7.ExecuteNonQuery();

        //                queryUp.Clear();
        //                queryUp.Append("UPDATE D1 SET D1.NOME_GERENTE_I = D2.NOME_GERENTE_I ");
        //                queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
        //                queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
        //                queryUp.Append("WHERE D2.MATRICULA_SUPERVISOR <> '0' ");
        //                SqlCommand createUp8 = new SqlCommand(queryUp.ToString(), connection);
        //                createUp8.CommandTimeout = 0;
        //                createUp8.ExecuteNonQuery();

        //                queryUp.Clear();
        //                queryUp.Append("UPDATE D1 SET D1.MATRICULA_DIRETOR = D2.MATRICULA_DIRETOR ");
        //                queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
        //                queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
        //                queryUp.Append("WHERE D2.MATRICULA_SUPERVISOR <> '0' ");
        //                SqlCommand createUp9 = new SqlCommand(queryUp.ToString(), connection);
        //                createUp9.CommandTimeout = 0;
        //                createUp9.ExecuteNonQuery();

        //                queryUp.Clear();
        //                queryUp.Append("UPDATE D1 SET D1.NOME_DIRETOR = D2.NOME_DIRETOR ");
        //                queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
        //                queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
        //                queryUp.Append("WHERE D2.MATRICULA_SUPERVISOR <> '0' ");
        //                SqlCommand createUp10 = new SqlCommand(queryUp.ToString(), connection);
        //                createUp10.CommandTimeout = 0;
        //                createUp10.ExecuteNonQuery();

        //                queryUp.Clear();
        //                queryUp.Append("UPDATE D1 SET D1.MATRICULA_CEO = D2.MATRICULA_CEO ");
        //                queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
        //                queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
        //                queryUp.Append("WHERE D2.MATRICULA_SUPERVISOR <> '0' ");
        //                SqlCommand createUp11 = new SqlCommand(queryUp.ToString(), connection);
        //                createUp11.CommandTimeout = 0;
        //                createUp11.ExecuteNonQuery();

        //                queryUp.Clear();
        //                queryUp.Append("UPDATE D1 SET D1.NOME_CEO = D2.NOME_CEO ");
        //                queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
        //                queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
        //                queryUp.Append("WHERE D2.MATRICULA_SUPERVISOR <> '0' ");
        //                SqlCommand createUp12 = new SqlCommand(queryUp.ToString(), connection);
        //                createUp12.CommandTimeout = 0;
        //                createUp12.ExecuteNonQuery();

        //                queryUp.Clear();
        //                queryUp.Append("UPDATE D1 SET D1.ACTIVE = D2.ACTIVE ");
        //                queryUp.Append("FROM GDA_COLLABORATORS_LAST_DETAILS AS D1 ");
        //                queryUp.Append("INNER JOIN #TEMPAG AS D2 ON D2.IDGDA_COLLABORATORS = D1.IDGDA_COLLABORATORS ");
        //                queryUp.Append("WHERE D2.ACTIVE IS NOT NULL ");
        //                SqlCommand createUp13 = new SqlCommand(queryUp.ToString(), connection);
        //                createUp13.CommandTimeout = 0;
        //                createUp13.ExecuteNonQuery();
        //            }





        //            //A
        //            StringBuilder queryInsertResult2 = new StringBuilder();
        //            queryInsertResult2.Append("MERGE INTO GDA_COLLABORATORS_DETAILS AS TARGET  ");
        //            queryInsertResult2.Append("USING #TEMPAG AS SOURCE  ");
        //            queryInsertResult2.Append("ON (TARGET.IDGDA_COLLABORATORS = SOURCE.IDGDA_COLLABORATORS AND TARGET.CREATED_AT = SOURCE.CREATED_AT)  ");
        //            queryInsertResult2.Append("WHEN NOT MATCHED BY TARGET THEN  ");
        //            queryInsertResult2.Append("  INSERT (IDGDA_COLLABORATORS, CARGO, CREATED_AT, IDGDA_SECTOR, IDGDA_SECTOR_SUPERVISOR, IDGDA_SUBSECTOR, HOME_BASED, SITE, PERIODO, MATRICULA_SUPERVISOR, NOME_SUPERVISOR, MATRICULA_COORDENADOR,  ");
        //            queryInsertResult2.Append("		  NOME_COORDENADOR, MATRICULA_GERENTE_II, NOME_GERENTE_II, MATRICULA_GERENTE_I, NOME_GERENTE_I, MATRICULA_DIRETOR, NOME_DIRETOR, MATRICULA_CEO, NOME_CEO, ACTIVE)  ");
        //            queryInsertResult2.Append("  VALUES (SOURCE.IDGDA_COLLABORATORS, SOURCE.CARGO, SOURCE.CREATED_AT, SOURCE.IDGDA_SECTOR, SOURCE.IDGDA_SECTOR_SUPERVISOR, SOURCE.IDGDA_SUBSECTOR, SOURCE.HOME_BASED, SOURCE.SITE, SOURCE.PERIODO, SOURCE.MATRICULA_SUPERVISOR, SOURCE.NOME_SUPERVISOR, SOURCE.MATRICULA_COORDENADOR,  ");
        //            queryInsertResult2.Append("		  SOURCE.NOME_COORDENADOR, SOURCE.MATRICULA_GERENTE_II, SOURCE.NOME_GERENTE_II, SOURCE.MATRICULA_GERENTE_I, SOURCE.NOME_GERENTE_I, SOURCE.MATRICULA_DIRETOR, SOURCE.NOME_DIRETOR, SOURCE.MATRICULA_CEO, SOURCE.NOME_CEO, SOURCE.ACTIVE)  ");
        //            queryInsertResult2.Append("WHEN MATCHED THEN  ");
        //            queryInsertResult2.Append("  UPDATE SET  ");
        //            queryInsertResult2.Append("  TARGET.CARGO = SOURCE.CARGO, ");
        //            queryInsertResult2.Append("  TARGET.IDGDA_SECTOR = SOURCE.IDGDA_SECTOR, ");
        //            queryInsertResult2.Append("  TARGET.IDGDA_SECTOR_SUPERVISOR = SOURCE.IDGDA_SECTOR_SUPERVISOR, ");
        //            queryInsertResult2.Append("  TARGET.IDGDA_SUBSECTOR = SOURCE.IDGDA_SUBSECTOR, ");
        //            queryInsertResult2.Append("  TARGET.HOME_BASED = SOURCE.HOME_BASED, ");
        //            queryInsertResult2.Append("  TARGET.SITE = SOURCE.SITE, ");
        //            queryInsertResult2.Append("  TARGET.PERIODO = SOURCE.PERIODO, ");
        //            queryInsertResult2.Append("  TARGET.MATRICULA_SUPERVISOR = SOURCE.MATRICULA_SUPERVISOR, ");
        //            queryInsertResult2.Append("  TARGET.NOME_SUPERVISOR = SOURCE.NOME_SUPERVISOR, ");
        //            queryInsertResult2.Append("  TARGET.MATRICULA_COORDENADOR = SOURCE.MATRICULA_COORDENADOR, ");
        //            queryInsertResult2.Append("  TARGET.NOME_COORDENADOR = SOURCE.NOME_COORDENADOR, ");
        //            queryInsertResult2.Append("  TARGET.MATRICULA_GERENTE_II = SOURCE.MATRICULA_GERENTE_II, ");
        //            queryInsertResult2.Append("  TARGET.NOME_GERENTE_II = SOURCE.NOME_GERENTE_II, ");
        //            queryInsertResult2.Append("  TARGET.MATRICULA_GERENTE_I = SOURCE.MATRICULA_GERENTE_I, ");
        //            queryInsertResult2.Append("  TARGET.NOME_GERENTE_I = SOURCE.NOME_GERENTE_I, ");
        //            queryInsertResult2.Append("  TARGET.MATRICULA_DIRETOR = SOURCE.MATRICULA_DIRETOR, ");
        //            queryInsertResult2.Append("  TARGET.NOME_DIRETOR = SOURCE.NOME_DIRETOR, ");
        //            queryInsertResult2.Append("  TARGET.MATRICULA_CEO = SOURCE.MATRICULA_CEO, ");
        //            queryInsertResult2.Append("  TARGET.NOME_CEO = SOURCE.NOME_CEO, ");
        //            queryInsertResult2.Append("  TARGET.ACTIVE = SOURCE.ACTIVE; ");

        //            SqlCommand createTableCommand2 = new SqlCommand(queryInsertResult2.ToString(), connection);
        //            createTableCommand2.CommandTimeout = 0;
        //            createTableCommand2.ExecuteNonQuery();
        //        }



        //        string dropTempTableQuery2 = $"DROP TABLE #TEMPAG";
        //        using (SqlCommand dropTempTableCommand2 = new SqlCommand(dropTempTableQuery2, connection))
        //        {
        //            dropTempTableCommand2.ExecuteNonQuery();
        //        }

        //        //Log.insertLogTransaction(db, t_id.ToString(), "RESULT_CONSOLIDATED", "CONCLUDED", "");

        //    }
        //    catch (Exception ex)
        //    {
        //        //Log.insertLogTransaction(db, t_id.ToString(), "RESULT_CONSOLIDATED", "ERRO: " + ex.Message.ToString(), "");
        //    }
        //    connection.Close();
        //}

        public static List<MonetizationResultsModel> monetizaAccess(string dt)
        {
            List<MonetizationResultsModel> mrs = new List<MonetizationResultsModel>();

            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("DECLARE @DATAINICIAL AS DATE; SET @DATAINICIAL = '{0}'; ", dt);
            stb.Append("SELECT '' AS IDGDA_RESULT, I.IDGDA_INDICATOR AS INDICADORID, '' AS TRANSACTIONID, '' AS RESULT, CL.CREATED_AT, CL.IDGDA_COLLABORATORS, '' AS FACTORS, '' AS DELETED_AT, ");
            stb.Append("       MAX(CL.ACTIVE) AS ACTIVE,  ");
            stb.Append("       MAX(HIS.GOAL) AS GOAL,  ");
            stb.Append("       MAX(HIS.GOAL_NIGHT) AS GOAL_NIGHT,  ");
            stb.Append("       MAX(HIS.GOAL_LATENIGHT) AS GOAL_LATENIGHT,  ");
            stb.Append("       I.WEIGHT AS WEIGHT,  ");
            stb.Append("       HHR.LEVELWEIGHT AS HIERARCHYLEVEL,  ");
            stb.Append("       MAX(CL.PERIODO) AS PERIODO,  ");
            stb.Append("       MAX(HIG1.MONETIZATION) AS COIN1,  ");
            stb.Append("       MAX(HIG1.MONETIZATION_NIGHT) AS COIN1_NIGHT,  ");
            stb.Append("       MAX(HIG1.MONETIZATION_LATENIGHT) AS COIN1_LATENIGHT,  ");
            stb.Append("       MAX(HIG2.MONETIZATION) AS COIN2,  ");
            stb.Append("       MAX(HIG2.MONETIZATION_NIGHT) AS COIN2_NIGHT,  ");
            stb.Append("       MAX(HIG2.MONETIZATION_LATENIGHT) AS COIN2_LATENIGHT,  ");
            stb.Append("       MAX(HIG3.MONETIZATION) AS COIN3,  ");
            stb.Append("       MAX(HIG3.MONETIZATION_NIGHT) AS COIN3_NIGHT,  ");
            stb.Append("       MAX(HIG3.MONETIZATION_LATENIGHT) AS COIN3_LATENIGHT,  ");
            stb.Append("       MAX(HIG4.MONETIZATION) AS COIN4,  ");
            stb.Append("       MAX(HIG4.MONETIZATION_NIGHT) AS COIN4_NIGHT,  ");
            stb.Append("       MAX(HIG4.MONETIZATION_LATENIGHT) AS COIN4_LATENIGHT,  ");
            stb.Append("       CL.IDGDA_SECTOR,  ");
            stb.Append("       MAX(I.TYPE) AS TYPE,  ");
            stb.Append("       MAX(HIG1.METRIC_MIN) AS MIN1,   ");
            stb.Append("       MAX(HIG1.METRIC_MIN_NIGHT) AS MIN1_NIGHT,  ");
            stb.Append("       MAX(HIG1.METRIC_MIN_LATENIGHT) AS MIN1_LATENIGHT,  ");
            stb.Append("       MAX(HIG2.METRIC_MIN) AS MIN2,   ");
            stb.Append("       MAX(HIG2.METRIC_MIN_NIGHT) AS MIN2_NIGHT,   ");
            stb.Append("       MAX(HIG2.METRIC_MIN_LATENIGHT) AS MIN2_LATENIGHT,  ");
            stb.Append("       MAX(HIG3.METRIC_MIN) AS MIN3,   ");
            stb.Append("       MAX(HIG3.METRIC_MIN_NIGHT) AS MIN3_NIGHT,   ");
            stb.Append("       MAX(HIG3.METRIC_MIN_LATENIGHT) AS MIN3_LATENIGHT,  ");
            stb.Append("       MAX(HIG4.METRIC_MIN) AS MIN4,   ");
            stb.Append("       MAX(HIG4.METRIC_MIN_NIGHT) AS MIN4_NIGHT,   ");
            stb.Append("       MAX(HIG4.METRIC_MIN_LATENIGHT) AS MIN4_LATENIGHT,  ");
            stb.Append("       CASE  ");
            stb.Append("           WHEN MAX(ME.EXPRESSION) IS NULL THEN '(#FATOR1/#FATOR0)'  ");
            stb.Append("           ELSE MAX(ME.EXPRESSION)  ");
            stb.Append("       END AS CONTA,  ");
            stb.Append("       CASE  ");
            stb.Append("           WHEN MAX(I.CALCULATION_TYPE) IS NULL THEN 'BIGGER_BETTER'  ");
            stb.Append("           ELSE MAX(I.CALCULATION_TYPE)  ");
            stb.Append("       END AS BETTER,  ");
            stb.Append("       COALESCE(  ");
            stb.Append("                  (SELECT TOP 1 BALANCE  ");
            stb.Append("                   FROM GDA_CHECKING_ACCOUNT (NOLOCK)  ");
            stb.Append("                   WHERE COLLABORATOR_ID = CL.IDGDA_COLLABORATORS  ");
            stb.Append("                   ORDER BY CREATED_AT DESC), 0) AS SALDO,  ");
            stb.Append("       COALESCE(  ");
            stb.Append("                  (SELECT (SUM(INPUT) - SUM(OUTPUT)) AS INPUT  ");
            stb.Append("                   FROM GDA_CHECKING_ACCOUNT (NOLOCK) AS A  ");
            stb.Append("                   WHERE A.COLLABORATOR_ID = CL.IDGDA_COLLABORATORS  ");
            stb.Append("                     AND A.RESULT_DATE = CL.CREATED_AT  ");
            stb.Append("                     AND GDA_INDICATOR_IDGDA_INDICATOR = I.IDGDA_INDICATOR  ");
            stb.Append("                   GROUP BY A.COLLABORATOR_ID, A.RESULT_DATE, A.GDA_INDICATOR_IDGDA_INDICATOR), 0) AS COINS,  ");
            stb.Append("       '' AS TRANSACTIONID,  ");
            stb.Append("       MAX(CL.MATRICULA_SUPERVISOR) AS 'MATRICULA SUPERVISOR',  ");
            stb.Append("       MAX(CL.NOME_SUPERVISOR) AS 'NOME SUPERVISOR',  ");
            stb.Append("       MAX(CL.MATRICULA_COORDENADOR) AS 'MATRICULA COORDENADOR',  ");
            stb.Append("       MAX(CL.NOME_COORDENADOR) AS 'NOME COORDENADOR',  ");
            stb.Append("       MAX(CL.MATRICULA_GERENTE_II) AS 'MATRICULA GERENTE II',  ");
            stb.Append("       MAX(CL.NOME_GERENTE_II) AS 'NOME GERENTE II',  ");
            stb.Append("       MAX(CL.MATRICULA_GERENTE_I) AS 'MATRICULA GERENTE I',  ");
            stb.Append("       MAX(CL.NOME_GERENTE_I) AS 'NOME GERENTE I',  ");
            stb.Append("       MAX(CL.MATRICULA_DIRETOR) AS 'MATRICULA DIRETOR',  ");
            stb.Append("       MAX(CL.NOME_DIRETOR) AS 'NOME DIRETOR',  ");
            stb.Append("       MAX(CL.MATRICULA_CEO) AS 'MATRICULA CEO',  ");
            stb.Append("       MAX(CL.NOME_CEO) AS 'NOME CEO', ");
            stb.Append("	   MAX(ESC.ESCALADO) AS ESCALADO,  ");
            stb.Append("       MAX(LOG.LOGIN) AS LOGADO,   ");
            stb.Append("	   MAX(ESCLOG.ESCALADOLOGADO) AS ESCALADOLOGADO  ");

            //Expiração moedas
            stb.Append(", MAX(MGP.PAUSE) AS 'PAUSE',  ");
            stb.Append("MAX(MG1.IDGDA_MONETIZATION_CONFIG) AS 'IDGDA_MONETIZATION_CONFIG1', ");
            stb.Append("MAX(MG2.IDGDA_MONETIZATION_CONFIG) AS 'IDGDA_MONETIZATION_CONFIG2', ");
            stb.Append("MAX(MG1.DAYS) AS 'DIAS SITE', ");
            stb.Append("MAX(MG2.DAYS) AS 'DIAS SETOR' ");

            stb.Append("FROM  ");
            stb.Append("(  ");
            stb.Append("SELECT IDGDA_COLLABORATORS, ACTIVE, MATRICULA_SUPERVISOR, MATRICULA_COORDENADOR, NOME_SUPERVISOR,  ");
            stb.Append("NOME_COORDENADOR, MATRICULA_GERENTE_II, NOME_GERENTE_II,MATRICULA_GERENTE_I, NOME_GERENTE_I,  ");
            stb.Append("MATRICULA_DIRETOR, NOME_DIRETOR, MATRICULA_CEO, NOME_CEO, PERIODO, CREATED_AT, ");
            stb.Append("CASE WHEN IDGDA_SUBSECTOR IS NOT NULL THEN IDGDA_SUBSECTOR  ");
            stb.Append("ELSE IDGDA_SECTOR END AS IDGDA_SECTOR, SITE  ");
            stb.Append("FROM GDA_COLLABORATORS_DETAILS (NOLOCK) CL  ");
            stb.Append("WHERE CL.CREATED_AT = @DATAINICIAL  ");
            stb.Append(") AS CL  ");
            stb.Append(" ");

            //Expiração moedas
            stb.Append("LEFT JOIN GDA_MONETIZATION_CONFIG_PAUSE (NOLOCK) MGP ON MGP.DELETED_AT IS NULL ");
            stb.Append("LEFT JOIN GDA_SITE (NOLOCK) AS SS ON SS.SITE = CL.SITE ");
            stb.Append("LEFT JOIN GDA_MONETIZATION_CONFIG (NOLOCK) AS MG1 ON MG1.DELETED_AT IS NULL AND MG1.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE = 2 AND MG1.ID_REFERER = SS.IDGDA_SITE ");
            stb.Append("LEFT JOIN GDA_MONETIZATION_CONFIG (NOLOCK) AS MG2 ON MG2.DELETED_AT IS NULL AND MG2.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE = 1 AND MG2.ID_REFERER = CL.IDGDA_SECTOR ");


            stb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS I ON I.IDGDA_INDICATOR IN ('10000013') ");
            stb.Append(" ");
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL  ");
            stb.Append("AND HIG1.INDICATOR_ID = I.IDGDA_INDICATOR ");
            stb.Append("AND HIG1.SECTOR_ID = CL.IDGDA_SECTOR  ");
            stb.Append("AND HIG1.GROUPID = 1  ");
            stb.Append("AND CONVERT(DATE, HIG1.STARTED_AT) <= CL.CREATED_AT AND CONVERT(DATE, HIG1.ENDED_AT) >= CL.CREATED_AT  ");
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG2 ON HIG2.DELETED_AT IS NULL  ");
            stb.Append("AND HIG2.INDICATOR_ID = I.IDGDA_INDICATOR ");
            stb.Append("AND HIG2.SECTOR_ID = CL.IDGDA_SECTOR  ");
            stb.Append("AND HIG2.GROUPID = 2  ");
            stb.Append("AND CONVERT(DATE, HIG2.STARTED_AT) <= CL.CREATED_AT AND CONVERT(DATE, HIG2.ENDED_AT) >= CL.CREATED_AT  ");
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG3 ON HIG3.DELETED_AT IS NULL  ");
            stb.Append("AND HIG3.INDICATOR_ID = I.IDGDA_INDICATOR ");
            stb.Append("AND HIG3.SECTOR_ID = CL.IDGDA_SECTOR  ");
            stb.Append("AND HIG3.GROUPID = 3  ");
            stb.Append("AND CONVERT(DATE, HIG3.STARTED_AT) <= CL.CREATED_AT AND CONVERT(DATE, HIG3.ENDED_AT) >= CL.CREATED_AT  ");
            stb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG4 ON HIG4.DELETED_AT IS NULL  ");
            stb.Append("AND HIG4.INDICATOR_ID = I.IDGDA_INDICATOR ");
            stb.Append("AND HIG4.SECTOR_ID = CL.IDGDA_SECTOR  ");
            stb.Append("AND HIG4.GROUPID = 4  ");
            stb.Append("AND CONVERT(DATE, HIG4.STARTED_AT) <= CL.CREATED_AT AND CONVERT(DATE, HIG4.ENDED_AT) >= CL.CREATED_AT  ");
            stb.Append("LEFT JOIN GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME ON HME.INDICATORID = I.IDGDA_INDICATOR AND HME.DELETED_AT IS NULL  ");
            stb.Append("LEFT JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON ME.ID = HME.MATHEMATICALEXPRESSIONID AND ME.DELETED_AT IS NULL   ");
            stb.Append("LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS HHR ON HHR.idgda_COLLABORATORS = CL.IDGDA_COLLABORATORS  ");
            stb.Append("AND HHR.DATE = @DATAINICIAL  ");
            stb.Append("INNER JOIN GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) AS HIS  ");
            stb.Append(" ON HIS.INDICATOR_ID = I.IDGDA_INDICATOR ");
            stb.Append("AND HIS.SECTOR_ID = CL.IDGDA_SECTOR  ");
            stb.Append("AND HIS.DELETED_AT IS NULL  ");
            stb.Append("AND CONVERT(DATE, HIS.STARTED_AT) <= CL.CREATED_AT AND CONVERT(DATE, HIS.ENDED_AT) >= CL.CREATED_AT  ");
            stb.Append(" ");
            stb.Append("LEFT JOIN   ");
            stb.Append("  (SELECT COUNT(0) AS 'ESCALADO',   ");
            stb.Append("          IDGDA_COLLABORATORS,   ");
            stb.Append("          CREATED_AT  ");
            stb.Append("   FROM GDA_RESULT (NOLOCK)   ");
            stb.Append("   WHERE INDICADORID = -1   ");
            stb.Append("     AND CREATED_AT = @DATAINICIAL    ");
            stb.Append("     AND RESULT = 1   ");
            stb.Append("     AND DELETED_AT IS NULL  ");
            stb.Append("   GROUP BY IDGDA_COLLABORATORS,   ");
            stb.Append("            CREATED_AT) AS ESC ON ESC.IDGDA_COLLABORATORS = CL.IDGDA_COLLABORATORS   ");
            stb.Append("AND ESC.CREATED_AT = CL.CREATED_AT   ");
            stb.Append("LEFT JOIN   ");
            stb.Append("  (SELECT COUNT(DISTINCT IDGDA_COLLABORATOR) AS 'LOGIN',   ");
            stb.Append("          IDGDA_COLLABORATOR,   ");
            stb.Append("          CONVERT(DATE, DATE_ACCESS) AS CREATED_AT   ");
            stb.Append("   FROM GDA_LOGIN_ACCESS (NOLOCK)   ");
            stb.Append("   WHERE CONVERT(DATE, DATE_ACCESS) = @DATAINICIAL    ");
            stb.Append("   GROUP BY IDGDA_COLLABORATOR,   ");
            stb.Append("            CONVERT(DATE, DATE_ACCESS)) AS LOG ON LOG.IDGDA_COLLABORATOR = CL.IDGDA_COLLABORATORS   ");
            stb.Append("AND LOG.CREATED_AT = CL.CREATED_AT   ");
            stb.Append("LEFT JOIN   ");
            stb.Append("  (SELECT COUNT(0) AS 'ESCALADOLOGADO',   ");
            stb.Append("          IDGDA_COLLABORATORS,   ");
            stb.Append("          CREATED_AT   ");
            stb.Append("   FROM GDA_RESULT (NOLOCK) R  ");
            stb.Append("   INNER JOIN GDA_LOGIN_ACCESS (NOLOCK) L ON CONVERT(DATE, DATE_ACCESS) = CREATED_AT AND L.IDGDA_COLLABORATOR = R.IDGDA_COLLABORATORS  ");
            stb.Append("   WHERE INDICADORID = -1   ");
            stb.Append("     AND CREATED_AT = @DATAINICIAL   ");
            stb.Append("     AND RESULT = 1   ");
            stb.Append("     AND DELETED_AT IS NULL   ");
            stb.Append("   GROUP BY IDGDA_COLLABORATORS,   ");
            stb.Append("            CREATED_AT) AS ESCLOG ON ESCLOG.IDGDA_COLLABORATORS = CL.IDGDA_COLLABORATORS   ");
            stb.Append("			AND ESCLOG.CREATED_AT = CL.CREATED_AT   ");
            stb.Append("WHERE CL.CREATED_AT = @DATAINICIAL  ");
            stb.Append("  AND HIG1.MONETIZATION > 0  ");
            stb.Append("  AND CL.active = 'true' ");
            stb.Append("GROUP BY CL.IDGDA_COLLABORATORS,  ");
            stb.Append("         I.IDGDA_INDICATOR,  ");
            stb.Append("         CL.CREATED_AT,  ");
            stb.Append("         CL.IDGDA_SECTOR,  ");
            stb.Append("         I.WEIGHT,  ");
            stb.Append("         HHR.LEVELWEIGHT  ");


            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                connection.Open();



                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                {
                    command.CommandTimeout = 1800;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                MonetizationResultsModel mr = new MonetizationResultsModel();

                                if (reader["ACTIVE"].ToString() != "true")
                                {
                                    continue;
                                }

                                mr.idCollaborator = reader["IDGDA_COLLABORATORS"].ToString();
                                mr.idIndicator = reader["INDICADORID"].ToString();
                                mr.idResult = reader["IDGDA_RESULT"].ToString();
                                mr.idSector = reader["IDGDA_SECTOR"].ToString();
                                mr.idCheckingAccount = 0;
                                mr.indicatorWeight = reader["WEIGHT"].ToString();
                                mr.hierarchyLevel = reader["HIERARCHYLEVEL"].ToString();

                                mr.fatores = reader["FACTORS"].ToString();
                                mr.fator0 = 0;
                                mr.fator1 = 0;
                                mr.conta = reader["CONTA"].ToString();
                                mr.melhor = reader["BETTER"].ToString();

                                string periodo = reader["PERIODO"].ToString();
                                if (periodo == "DIURNO")
                                {
                                    if (reader["GOAL"] == DBNull.Value)
                                    {
                                        continue;
                                    }
                                    mr.meta = double.Parse(reader["GOAL"].ToString());
                                    mr.G1 = double.Parse(reader["MIN1"].ToString());
                                    mr.G2 = double.Parse(reader["MIN2"].ToString());
                                    mr.G3 = double.Parse(reader["MIN3"].ToString());
                                    mr.G4 = double.Parse(reader["MIN4"].ToString());
                                    mr.C1 = double.Parse(reader["COIN1"].ToString());
                                    mr.C2 = double.Parse(reader["COIN2"].ToString());
                                    mr.C3 = double.Parse(reader["COIN3"].ToString());
                                    mr.C4 = double.Parse(reader["COIN4"].ToString());
                                }
                                else if (periodo == "NOTURNO")
                                {
                                    if (reader["GOAL_NIGHT"] == DBNull.Value)
                                    {
                                        continue;
                                    }
                                    mr.meta = double.Parse(reader["GOAL_NIGHT"].ToString());
                                    mr.G1 = double.Parse(reader["MIN1_NIGHT"].ToString());
                                    mr.G2 = double.Parse(reader["MIN2_NIGHT"].ToString());
                                    mr.G3 = double.Parse(reader["MIN3_NIGHT"].ToString());
                                    mr.G4 = double.Parse(reader["MIN4_NIGHT"].ToString());
                                    mr.C1 = double.Parse(reader["COIN1_NIGHT"].ToString());
                                    mr.C2 = double.Parse(reader["COIN2_NIGHT"].ToString());
                                    mr.C3 = double.Parse(reader["COIN3_NIGHT"].ToString());
                                    mr.C4 = double.Parse(reader["COIN4_NIGHT"].ToString());
                                }
                                else if (periodo == "MADRUGADA")
                                {
                                    if (reader["GOAL_LATENIGHT"] == DBNull.Value)
                                    {
                                        continue;
                                    }
                                    mr.meta = double.Parse(reader["GOAL_LATENIGHT"].ToString());
                                    mr.G1 = double.Parse(reader["MIN1_LATENIGHT"].ToString());
                                    mr.G2 = double.Parse(reader["MIN2_LATENIGHT"].ToString());
                                    mr.G3 = double.Parse(reader["MIN3_LATENIGHT"].ToString());
                                    mr.G4 = double.Parse(reader["MIN4_LATENIGHT"].ToString());
                                    mr.C1 = double.Parse(reader["COIN1_LATENIGHT"].ToString());
                                    mr.C2 = double.Parse(reader["COIN2_LATENIGHT"].ToString());
                                    mr.C3 = double.Parse(reader["COIN3_LATENIGHT"].ToString());
                                    mr.C4 = double.Parse(reader["COIN4_LATENIGHT"].ToString());
                                }
                                else
                                {
                                    continue;
                                }


                                mr.saldo = double.Parse(reader["SALDO"].ToString());
                                mr.typeIndicator = reader["TYPE"].ToString();

                                mr.transactionId = 0;
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

                                mr.sumDiasLogados = reader["LOGADO"].ToString() != "" ? Convert.ToInt32(reader["LOGADO"].ToString()) : 0;
                                mr.sumDiasEscalados = reader["ESCALADO"].ToString() != "" ? Convert.ToInt32(reader["ESCALADO"].ToString()) : 0;
                                mr.sumDiasLogadosEEscalados = reader["ESCALADOLOGADO"].ToString() != "" ? Convert.ToInt32(reader["ESCALADOLOGADO"].ToString()) : 0;

                                // Expiração de moedas
                                mr.duePause = reader["PAUSE"] != DBNull.Value ? Convert.ToInt32(reader["PAUSE"].ToString()) : 0;
                                mr.daySite = reader["DIAS SITE"] != DBNull.Value ? Convert.ToInt32(reader["DIAS SITE"].ToString()) : 0;
                                mr.daySetor = reader["DIAS SETOR"] != DBNull.Value ? Convert.ToInt32(reader["DIAS SETOR"].ToString()) : 0;
                                mr.idgdaMonetizationConfigSite = reader["IDGDA_MONETIZATION_CONFIG2"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_MONETIZATION_CONFIG2"].ToString()) : 0;
                                mr.idgdaMonetizationConfigSetor = reader["IDGDA_MONETIZATION_CONFIG1"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_MONETIZATION_CONFIG1"].ToString()) : 0;

                                mr.hig1Id = reader["HIG1ID"] != DBNull.Value ? Convert.ToInt32(reader["HIG1ID"].ToString()) : 0;
                                mr.hig2Id = reader["HIG2ID"] != DBNull.Value ? Convert.ToInt32(reader["HIG2ID"].ToString()) : 0;
                                mr.hig3Id = reader["HIG3ID"] != DBNull.Value ? Convert.ToInt32(reader["HIG3ID"].ToString()) : 0;
                                mr.hig4Id = reader["HIG4ID"] != DBNull.Value ? Convert.ToInt32(reader["HIG4ID"].ToString()) : 0;
                                mr.hisId = reader["HISID"] != DBNull.Value ? Convert.ToInt32(reader["HISID"].ToString()) : 0;

                                mrs.Add(mr);
                            }
                            catch (Exception ex)
                            {

                            }

                        }
                    }
                }
            }
            return mrs;
        }

        public static List<monetizationHierarchyInformation> returnHierarchyInformation(string dt)
        {
            List<monetizationHierarchyInformation> hierarInformation = new List<monetizationHierarchyInformation>();

            try
            {
                using (SqlConnection connection = new SqlConnection(Database.Conn))
                {
                    StringBuilder stb = new StringBuilder();
                    stb.Append($"DECLARE @DATAINICIAL AS DATE; SET @DATAINICIAL = '{dt}'; ");
                    stb.Append("SELECT IDGDA_COLLABORATORS, IDGDA_SITE, IDGDA_SECTOR, IDGDA_SUBSECTOR, ");
                    stb.Append("MGP.PAUSE AS 'PAUSE',  ");
                    stb.Append("MG1.IDGDA_MONETIZATION_CONFIG AS 'IDGDA_MONETIZATION_CONFIG1', ");
                    stb.Append("MG2.IDGDA_MONETIZATION_CONFIG AS 'IDGDA_MONETIZATION_CONFIG2', ");
                    stb.Append("MG1.DAYS AS 'DIAS SITE', ");
                    stb.Append("MG2.DAYS AS 'DIAS SETOR' ");
                    stb.Append("FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CL ");
                    stb.Append("LEFT JOIN GDA_MONETIZATION_CONFIG_PAUSE (NOLOCK) MGP ON MGP.DELETED_AT IS NULL  ");
                    stb.Append("LEFT JOIN GDA_SITE (NOLOCK) AS SS ON SS.SITE = CL.SITE  ");
                    stb.Append("LEFT JOIN GDA_MONETIZATION_CONFIG (NOLOCK) AS MG1 ON MG1.DELETED_AT IS NULL AND MG1.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE = 2 AND MG1.ID_REFERER = SS.IDGDA_SITE  ");
                    stb.Append("LEFT JOIN GDA_MONETIZATION_CONFIG (NOLOCK) AS MG2 ON MG2.DELETED_AT IS NULL AND MG2.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE = 1 AND MG2.ID_REFERER = CL.IDGDA_SECTOR  ");
                    stb.Append("WHERE CARGO <> 'AGENTE' ");
                    stb.Append("AND CL.CREATED_AT = @DATAINICIAL ");

                    connection.Open();
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.CommandTimeout = 1800;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                monetizationHierarchyInformation hierarInf = new monetizationHierarchyInformation();
                                hierarInf.idCollaborator = reader["IDGDA_COLLABORATORS"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_COLLABORATORS"].ToString()) : 0;
                                hierarInf.idSite = reader["IDGDA_SITE"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_SITE"].ToString()) : 0;
                                hierarInf.idSector = reader["IDGDA_SECTOR"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_SECTOR"].ToString()) : 0;
                                hierarInf.idSubSector = reader["IDGDA_SUBSECTOR"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_SUBSECTOR"].ToString()) : 0;
                                hierarInf.duePause = reader["PAUSE"] != DBNull.Value ? Convert.ToInt32(reader["PAUSE"].ToString()) : 0;
                                hierarInf.daySite = reader["DIAS SITE"] != DBNull.Value ? Convert.ToInt32(reader["DIAS SITE"].ToString()) : 0;
                                hierarInf.daySetor = reader["DIAS SETOR"] != DBNull.Value ? Convert.ToInt32(reader["DIAS SETOR"].ToString()) : 0;
                                hierarInf.idgdaMonetizationConfigSite = reader["IDGDA_MONETIZATION_CONFIG1"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_MONETIZATION_CONFIG1"].ToString()) : 0;
                                hierarInf.idgdaMonetizationConfigSetor = reader["IDGDA_MONETIZATION_CONFIG2"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_MONETIZATION_CONFIG2"].ToString()) : 0;

                                hierarInformation.Add(hierarInf);
                            }
                        }
                    }
                    connection.Close();
                }

            }
            catch (Exception ex)
            {

            }

            return hierarInformation;
        }

        public static List<MonetizationResultsModel> monetizationTxIndicatorAccess(string dt)
        {
            List<MonetizationResultsModel> mrs = new List<MonetizationResultsModel>();

            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                connection.Open();
                try
                {
                    StringBuilder stb = new StringBuilder();
                    stb.AppendFormat($"DECLARE @DATAINICIAL AS DATE; SET @DATAINICIAL = '{dt}'; ");
                    stb.AppendFormat(" ");
                    stb.AppendFormat("SELECT ");
                    //stb.AppendFormat("     R.IDGDA_RESULT, ");
                    stb.AppendFormat("       I.IDGDA_INDICATOR AS INDICADORID, ");
                    //stb.AppendFormat("     R.TRANSACTIONID, ");
                    //stb.AppendFormat("     R.RESULT, ");
                    stb.AppendFormat("       @DATAINICIAL AS CREATED_AT, ");
                    stb.AppendFormat("       CL.IDGDA_COLLABORATORS, ");
                    //stb.AppendFormat("     R.FACTORS, ");
                    stb.AppendFormat("       I.DELETED_AT, ");
                    stb.AppendFormat("       MAX(CL.ACTIVE) AS ACTIVE, ");
                    stb.AppendFormat("       MAX(HIS.GOAL) AS GOAL, ");
                    stb.AppendFormat("       MAX(HIS.GOAL_NIGHT) AS GOAL_NIGHT, ");
                    stb.AppendFormat("       MAX(HIS.GOAL_LATENIGHT) AS GOAL_LATENIGHT, ");
                    stb.AppendFormat("       I.WEIGHT AS WEIGHT, ");
                    stb.AppendFormat("       HHR.LEVELWEIGHT AS HIERARCHYLEVEL, ");
                    stb.AppendFormat("       MAX(CL.PERIODO) AS PERIODO, ");
                    stb.AppendFormat("       MAX(HIG1.MONETIZATION) AS COIN1, ");
                    stb.AppendFormat("       MAX(HIG1.MONETIZATION_NIGHT) AS COIN1_NIGHT, ");
                    stb.AppendFormat("       MAX(HIG1.MONETIZATION_LATENIGHT) AS COIN1_LATENIGHT, ");
                    stb.AppendFormat("       MAX(HIG2.MONETIZATION) AS COIN2, ");
                    stb.AppendFormat("       MAX(HIG2.MONETIZATION_NIGHT) AS COIN2_NIGHT, ");
                    stb.AppendFormat("       MAX(HIG2.MONETIZATION_LATENIGHT) AS COIN2_LATENIGHT, ");
                    stb.AppendFormat("       MAX(HIG3.MONETIZATION) AS COIN3, ");
                    stb.AppendFormat("       MAX(HIG3.MONETIZATION_NIGHT) AS COIN3_NIGHT, ");
                    stb.AppendFormat("       MAX(HIG3.MONETIZATION_LATENIGHT) AS COIN3_LATENIGHT, ");
                    stb.AppendFormat("       MAX(HIG4.MONETIZATION) AS COIN4, ");
                    stb.AppendFormat("       MAX(HIG4.MONETIZATION_NIGHT) AS COIN4_NIGHT, ");
                    stb.AppendFormat("       MAX(HIG4.MONETIZATION_LATENIGHT) AS COIN4_LATENIGHT, ");
                    stb.AppendFormat("       CL.IDGDA_SECTOR, ");
                    stb.AppendFormat("       MAX(I.TYPE) AS TYPE, ");
                    stb.AppendFormat("       MAX(HIG1.METRIC_MIN) AS MIN1, ");
                    stb.AppendFormat("       MAX(HIG1.METRIC_MIN_NIGHT) AS MIN1_NIGHT, ");
                    stb.AppendFormat("       MAX(HIG1.METRIC_MIN_LATENIGHT) AS MIN1_LATENIGHT, ");
                    stb.AppendFormat("       MAX(HIG2.METRIC_MIN) AS MIN2, ");
                    stb.AppendFormat("       MAX(HIG2.METRIC_MIN_NIGHT) AS MIN2_NIGHT, ");
                    stb.AppendFormat("       MAX(HIG2.METRIC_MIN_LATENIGHT) AS MIN2_LATENIGHT, ");
                    stb.AppendFormat("       MAX(HIG3.METRIC_MIN) AS MIN3, ");
                    stb.AppendFormat("       MAX(HIG3.METRIC_MIN_NIGHT) AS MIN3_NIGHT, ");
                    stb.AppendFormat("       MAX(HIG3.METRIC_MIN_LATENIGHT) AS MIN3_LATENIGHT, ");
                    stb.AppendFormat("       MAX(HIG4.METRIC_MIN) AS MIN4, ");
                    stb.AppendFormat("       MAX(HIG4.METRIC_MIN_NIGHT) AS MIN4_NIGHT, ");
                    stb.AppendFormat("       MAX(HIG4.METRIC_MIN_LATENIGHT) AS MIN4_LATENIGHT, ");
                    stb.AppendFormat("       CASE ");
                    stb.AppendFormat("           WHEN MAX(ME.EXPRESSION) IS NULL THEN '(#FATOR1/#FATOR0)' ");
                    stb.AppendFormat("           ELSE MAX(ME.EXPRESSION) ");
                    stb.AppendFormat("       END AS CONTA, ");
                    stb.AppendFormat("       CASE ");
                    stb.AppendFormat("           WHEN MAX(I.CALCULATION_TYPE) IS NULL THEN 'BIGGER_BETTER' ");
                    stb.AppendFormat("           ELSE MAX(I.CALCULATION_TYPE) ");
                    stb.AppendFormat("       END AS BETTER, ");
                    stb.AppendFormat("       COALESCE( ");
                    stb.AppendFormat("                  (SELECT TOP 1 BALANCE ");
                    stb.AppendFormat("                   FROM GDA_CHECKING_ACCOUNT (NOLOCK) ");
                    stb.AppendFormat("                   WHERE COLLABORATOR_ID = CL.IDGDA_COLLABORATORS ");
                    stb.AppendFormat("                   ORDER BY CREATED_AT DESC), 0) AS SALDO, ");
                    stb.AppendFormat("       COALESCE( ");
                    stb.AppendFormat("                  (SELECT (SUM(INPUT) - SUM(OUTPUT)) AS INPUT ");
                    stb.AppendFormat("                   FROM GDA_CHECKING_ACCOUNT (NOLOCK) AS A ");
                    stb.AppendFormat("                   WHERE A.COLLABORATOR_ID = CL.IDGDA_COLLABORATORS ");
                    stb.AppendFormat("                     AND A.RESULT_DATE = @DATAINICIAL ");
                    stb.AppendFormat("                     AND GDA_INDICATOR_IDGDA_INDICATOR = I.IDGDA_INDICATOR ");
                    stb.AppendFormat("                   GROUP BY A.COLLABORATOR_ID, A.RESULT_DATE, A.GDA_INDICATOR_IDGDA_INDICATOR), 0) AS COINS, ");
                    //stb.AppendFormat("     R.TRANSACTIONID, ");
                    stb.AppendFormat("       MAX(CL.MATRICULA_SUPERVISOR) AS 'MATRICULA SUPERVISOR', ");
                    stb.AppendFormat("       MAX(CL.NOME_SUPERVISOR) AS 'NOME SUPERVISOR', ");
                    stb.AppendFormat("       MAX(CL.MATRICULA_COORDENADOR) AS 'MATRICULA COORDENADOR', ");
                    stb.AppendFormat("       MAX(CL.NOME_COORDENADOR) AS 'NOME COORDENADOR', ");
                    stb.AppendFormat("       MAX(CL.MATRICULA_GERENTE_II) AS 'MATRICULA GERENTE II', ");
                    stb.AppendFormat("       MAX(CL.NOME_GERENTE_II) AS 'NOME GERENTE II', ");
                    stb.AppendFormat("       MAX(CL.MATRICULA_GERENTE_I) AS 'MATRICULA GERENTE I', ");
                    stb.AppendFormat("       MAX(CL.NOME_GERENTE_I) AS 'NOME GERENTE I', ");
                    stb.AppendFormat("       MAX(CL.MATRICULA_DIRETOR) AS 'MATRICULA DIRETOR', ");
                    stb.AppendFormat("       MAX(CL.NOME_DIRETOR) AS 'NOME DIRETOR', ");
                    stb.AppendFormat("       MAX(CL.MATRICULA_CEO) AS 'MATRICULA CEO', ");
                    stb.AppendFormat("       MAX(CL.NOME_CEO) AS 'NOME CEO', ");
                    stb.AppendFormat("	     MAX(ESC.ESCALADO) AS ESCALADO,  ");
                    stb.AppendFormat("       ISNULL(MAX(AC.QUANTIDADE_LOGIN),0) AS LOGADO,  ");
                    stb.AppendFormat("       '' AS ESCALADOLOGADO , ");
                    stb.AppendFormat("       MAX(MGP.PAUSE) AS 'PAUSE', ");
                    stb.AppendFormat("       MAX(MG1.IDGDA_MONETIZATION_CONFIG) AS 'IDGDA_MONETIZATION_CONFIG1', ");
                    stb.AppendFormat("       MAX(MG2.IDGDA_MONETIZATION_CONFIG) AS 'IDGDA_MONETIZATION_CONFIG2', ");
                    stb.AppendFormat("       MAX(MG1.DAYS) AS 'DIAS SITE', ");
                    stb.AppendFormat("       MAX(MG2.DAYS) AS 'DIAS SETOR', ");
                    stb.AppendFormat("       MAX(HIG1.ID) AS HIG1ID, ");
                    stb.AppendFormat("       MAX(HIG2.ID) AS HIG2ID, ");
                    stb.AppendFormat("       MAX(HIG3.ID) AS HIG3ID, ");
                    stb.AppendFormat("       MAX(HIG4.ID) AS HIG4ID, ");
                    stb.AppendFormat("       MAX(HIS.ID) AS HISID ");
                    stb.AppendFormat("FROM (SELECT IDGDA_COLLABORATORS, ");
                    stb.AppendFormat("          ACTIVE, ");
                    stb.AppendFormat("          MATRICULA_SUPERVISOR, ");
                    stb.AppendFormat("          MATRICULA_COORDENADOR, ");
                    stb.AppendFormat("          NOME_SUPERVISOR, ");
                    stb.AppendFormat("          NOME_COORDENADOR, ");
                    stb.AppendFormat("          MATRICULA_GERENTE_II, ");
                    stb.AppendFormat("          NOME_GERENTE_II, ");
                    stb.AppendFormat("          MATRICULA_GERENTE_I, ");
                    stb.AppendFormat("          NOME_GERENTE_I, ");
                    stb.AppendFormat("          MATRICULA_DIRETOR, ");
                    stb.AppendFormat("          NOME_DIRETOR, ");
                    stb.AppendFormat("          MATRICULA_CEO, ");
                    stb.AppendFormat("          NOME_CEO, ");
                    stb.AppendFormat("		  CARGO, ");
                    stb.AppendFormat("          PERIODO, ");
                    stb.AppendFormat("          CASE ");
                    stb.AppendFormat("              WHEN IDGDA_SUBSECTOR IS NOT NULL THEN IDGDA_SUBSECTOR ");
                    stb.AppendFormat("              ELSE IDGDA_SECTOR ");
                    stb.AppendFormat("          END AS IDGDA_SECTOR, ");
                    stb.AppendFormat("          SITE ");
                    stb.AppendFormat("   FROM GDA_COLLABORATORS_DETAILS (NOLOCK) CL ");
                    stb.AppendFormat("   WHERE CL.CREATED_AT = @DATAINICIAL) AS CL  ");
                    stb.AppendFormat("   INNER JOIN GDA_INDICATOR (NOLOCK) AS I ON I.IDGDA_INDICATOR IN ('10000013','10000014')  ");
                    stb.AppendFormat("LEFT JOIN GDA_MONETIZATION_CONFIG_PAUSE (NOLOCK) MGP ON MGP.DELETED_AT IS NULL ");
                    stb.AppendFormat("LEFT JOIN GDA_SITE (NOLOCK) AS SS ON SS.SITE = CL.SITE ");
                    stb.AppendFormat("LEFT JOIN GDA_MONETIZATION_CONFIG (NOLOCK) AS MG1 ON MG1.DELETED_AT IS NULL ");
                    stb.AppendFormat("AND MG1.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE = 2 ");
                    stb.AppendFormat("AND MG1.ID_REFERER = SS.IDGDA_SITE ");
                    stb.AppendFormat("LEFT JOIN GDA_MONETIZATION_CONFIG (NOLOCK) AS MG2 ON MG2.DELETED_AT IS NULL ");
                    stb.AppendFormat("AND MG2.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE = 1 ");
                    stb.AppendFormat("AND MG2.ID_REFERER = CL.IDGDA_SECTOR ");
                    stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
                    stb.AppendFormat("AND HIG1.INDICATOR_ID = I.IDGDA_INDICATOR ");
                    stb.AppendFormat("AND HIG1.SECTOR_ID = CL.IDGDA_SECTOR ");
                    stb.AppendFormat("AND HIG1.GROUPID = 1 ");
                    stb.AppendFormat("AND CONVERT(DATE, HIG1.STARTED_AT) <= @DATAINICIAL ");
                    stb.AppendFormat("AND CONVERT(DATE, HIG1.ENDED_AT) >= @DATAINICIAL ");
                    stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG2 ON HIG2.DELETED_AT IS NULL ");
                    stb.AppendFormat("AND HIG2.INDICATOR_ID = i.IDGDA_INDICATOR ");
                    stb.AppendFormat("AND HIG2.SECTOR_ID = CL.IDGDA_SECTOR ");
                    stb.AppendFormat("AND HIG2.GROUPID = 2 ");
                    stb.AppendFormat("AND CONVERT(DATE, HIG2.STARTED_AT) <= @DATAINICIAL ");
                    stb.AppendFormat("AND CONVERT(DATE, HIG2.ENDED_AT) >= @DATAINICIAL ");
                    stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG3 ON HIG3.DELETED_AT IS NULL ");
                    stb.AppendFormat("AND HIG3.INDICATOR_ID = i.IDGDA_INDICATOR ");
                    stb.AppendFormat("AND HIG3.SECTOR_ID = CL.IDGDA_SECTOR ");
                    stb.AppendFormat("AND HIG3.GROUPID = 3 ");
                    stb.AppendFormat("AND CONVERT(DATE, HIG3.STARTED_AT) <= @DATAINICIAL ");
                    stb.AppendFormat("AND CONVERT(DATE, HIG3.ENDED_AT) >= @DATAINICIAL ");
                    stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG4 ON HIG4.DELETED_AT IS NULL ");
                    stb.AppendFormat("AND HIG4.INDICATOR_ID = i.IDGDA_INDICATOR ");
                    stb.AppendFormat("AND HIG4.SECTOR_ID = CL.IDGDA_SECTOR ");
                    stb.AppendFormat("AND HIG4.GROUPID = 4 ");
                    stb.AppendFormat("AND CONVERT(DATE, HIG4.STARTED_AT) <= @DATAINICIAL ");
                    stb.AppendFormat("AND CONVERT(DATE, HIG4.ENDED_AT) >= @DATAINICIAL ");
                    stb.AppendFormat("LEFT JOIN GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (NOLOCK) AS HME ON HME.INDICATORID = i.IDGDA_INDICATOR ");
                    stb.AppendFormat("AND HME.DELETED_AT IS NULL ");
                    stb.AppendFormat("LEFT JOIN GDA_MATHEMATICAL_EXPRESSIONS (NOLOCK) AS ME ON ME.ID = HME.MATHEMATICALEXPRESSIONID ");
                    stb.AppendFormat("AND ME.DELETED_AT IS NULL ");
                    stb.AppendFormat("LEFT JOIN GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) AS HHR ON HHR.idgda_COLLABORATORS = CL.IDGDA_COLLABORATORS ");
                    stb.AppendFormat("AND HHR.DATE = @DATAINICIAL ");
                    stb.AppendFormat("INNER JOIN GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) AS HIS ON HIS.INDICATOR_ID = i.IDGDA_INDICATOR ");
                    stb.AppendFormat("AND HIS.SECTOR_ID = CL.IDGDA_SECTOR ");
                    stb.AppendFormat("AND HIS.DELETED_AT IS NULL ");
                    stb.AppendFormat("AND CONVERT(DATE, HIS.STARTED_AT) <= @DATAINICIAL ");
                    stb.AppendFormat("AND CONVERT(DATE, HIS.ENDED_AT) >= @DATAINICIAL ");
                    stb.AppendFormat("LEFT JOIN  ");
                    stb.AppendFormat("   (SELECT COUNT(0) AS 'QUANTIDADE_LOGIN',  ");
                    stb.AppendFormat("   IDGDA_COLLABORATOR   ");
                    stb.AppendFormat("   FROM GDA_LOGIN_ACCESS (NOLOCK)  ");
                    stb.AppendFormat("   WHERE CONVERT(DATE, DATE_ACCESS) = @DATAINICIAL  ");
                    stb.AppendFormat("   GROUP BY IDGDA_COLLABORATOR  ");
                    stb.AppendFormat("   ) AS AC ON AC.IDGDA_COLLABORATOR = CL.IDGDA_COLLABORATORS  ");
                    stb.AppendFormat("LEFT JOIN  ");
                    stb.AppendFormat("  (SELECT COUNT(0) AS 'ESCALADO',  ");
                    stb.AppendFormat("          IDGDA_COLLABORATORS,  ");
                    stb.AppendFormat("          CREATED_AT  ");
                    stb.AppendFormat("   FROM GDA_RESULT (NOLOCK)  ");
                    stb.AppendFormat("   WHERE INDICADORID = -1  ");
                    stb.AppendFormat("     AND CREATED_AT = @DATAINICIAL  ");
                    stb.AppendFormat("     AND RESULT = 1  ");
                    stb.AppendFormat("     AND DELETED_AT IS NULL  ");
                    stb.AppendFormat("   GROUP BY IDGDA_COLLABORATORS,  ");
                    stb.AppendFormat("            CREATED_AT) AS ESC ON ESC.IDGDA_COLLABORATORS = CL.IDGDA_COLLABORATORS  ");
                    stb.AppendFormat("			AND ESC.CREATED_AT = @DATAINICIAL ");
                    stb.AppendFormat("WHERE  ");
                    stb.AppendFormat("(HIG1.MONETIZATION > 0 OR HIG1.MONETIZATION_NIGHT > 0 OR HIG1.MONETIZATION_LATENIGHT > 0) AND  ");
                    stb.AppendFormat("  I.DELETED_AT IS NULL ");
                    stb.AppendFormat("  AND CL.CARGO = 'AGENTE' ");
                    stb.AppendFormat("GROUP BY I.IDGDA_INDICATOR, ");
                    stb.AppendFormat("         CL.IDGDA_COLLABORATORS, ");
                    stb.AppendFormat("         I.DELETED_AT, ");
                    stb.AppendFormat("         CL.IDGDA_SECTOR, ");
                    stb.AppendFormat("         I.WEIGHT, ");
                    stb.AppendFormat("         HHR.LEVELWEIGHT ");


                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.CommandTimeout = 1800;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                try
                                {
                                    MonetizationResultsModel mr = new MonetizationResultsModel();

                                    //if (reader["ACTIVE"].ToString() != "true")
                                    //{
                                    //    continue;
                                    //}

                                    mr.idCollaborator = reader["IDGDA_COLLABORATORS"].ToString();
                                    mr.idIndicator = reader["INDICADORID"].ToString();
                                    mr.idResult = "";
                                    mr.idSector = reader["IDGDA_SECTOR"].ToString();
                                    mr.idCheckingAccount = 0;
                                    mr.indicatorWeight = reader["WEIGHT"].ToString();
                                    mr.hierarchyLevel = reader["HIERARCHYLEVEL"].ToString();

                                    mr.sumDiasEscalados = reader["ESCALADO"] != DBNull.Value ? Convert.ToInt32(reader["ESCALADO"]) : 0;
                                    mr.sumDiasLogados = reader["LOGADO"] != DBNull.Value ? Convert.ToInt32(reader["LOGADO"]) : 0;
                                    mr.sumDiasLogadosEEscalados = 0;

                                    mr.fatores = "";
                                    mr.fator0 = 0;
                                    mr.fator1 = 0;

                                    mr.conta = reader["CONTA"].ToString();
                                    mr.melhor = reader["BETTER"].ToString();

                                    string periodo = reader["PERIODO"].ToString();
                                    if (periodo == "DIURNO")
                                    {
                                        if (reader["GOAL"] == DBNull.Value)
                                        {
                                            continue;
                                        }
                                        mr.meta = double.Parse(reader["GOAL"].ToString());
                                        mr.G1 = double.Parse(reader["MIN1"].ToString());
                                        mr.G2 = double.Parse(reader["MIN2"].ToString());
                                        mr.G3 = double.Parse(reader["MIN3"].ToString());
                                        mr.G4 = double.Parse(reader["MIN4"].ToString());
                                        mr.C1 = double.Parse(reader["COIN1"].ToString());
                                        mr.C2 = double.Parse(reader["COIN2"].ToString());
                                        mr.C3 = double.Parse(reader["COIN3"].ToString());
                                        mr.C4 = double.Parse(reader["COIN4"].ToString());
                                    }
                                    else if (periodo == "NOTURNO")
                                    {
                                        if (reader["GOAL_NIGHT"] == DBNull.Value)
                                        {
                                            continue;
                                        }
                                        mr.meta = double.Parse(reader["GOAL_NIGHT"].ToString());
                                        mr.G1 = double.Parse(reader["MIN1_NIGHT"].ToString());
                                        mr.G2 = double.Parse(reader["MIN2_NIGHT"].ToString());
                                        mr.G3 = double.Parse(reader["MIN3_NIGHT"].ToString());
                                        mr.G4 = double.Parse(reader["MIN4_NIGHT"].ToString());
                                        mr.C1 = double.Parse(reader["COIN1_NIGHT"].ToString());
                                        mr.C2 = double.Parse(reader["COIN2_NIGHT"].ToString());
                                        mr.C3 = double.Parse(reader["COIN3_NIGHT"].ToString());
                                        mr.C4 = double.Parse(reader["COIN4_NIGHT"].ToString());
                                    }
                                    else if (periodo == "MADRUGADA")
                                    {
                                        if (reader["GOAL_LATENIGHT"] == DBNull.Value)
                                        {
                                            continue;
                                        }
                                        mr.meta = double.Parse(reader["GOAL_LATENIGHT"].ToString());
                                        mr.G1 = double.Parse(reader["MIN1_LATENIGHT"].ToString());
                                        mr.G2 = double.Parse(reader["MIN2_LATENIGHT"].ToString());
                                        mr.G3 = double.Parse(reader["MIN3_LATENIGHT"].ToString());
                                        mr.G4 = double.Parse(reader["MIN4_LATENIGHT"].ToString());
                                        mr.C1 = double.Parse(reader["COIN1_LATENIGHT"].ToString());
                                        mr.C2 = double.Parse(reader["COIN2_LATENIGHT"].ToString());
                                        mr.C3 = double.Parse(reader["COIN3_LATENIGHT"].ToString());
                                        mr.C4 = double.Parse(reader["COIN4_LATENIGHT"].ToString());
                                    }
                                    else
                                    {
                                        continue;
                                    }


                                    mr.saldo = double.Parse(reader["SALDO"].ToString());
                                    mr.typeIndicator = reader["TYPE"].ToString();

                                    mr.transactionId = 0;
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


                                    //Expiração de moedas
                                    mr.duePause = reader["PAUSE"] != DBNull.Value ? Convert.ToInt32(reader["PAUSE"].ToString()) : 0;
                                    mr.daySite = reader["DIAS SITE"] != DBNull.Value ? Convert.ToInt32(reader["DIAS SITE"].ToString()) : 0;
                                    mr.daySetor = reader["DIAS SETOR"] != DBNull.Value ? Convert.ToInt32(reader["DIAS SETOR"].ToString()) : 0;
                                    mr.idgdaMonetizationConfigSite = reader["IDGDA_MONETIZATION_CONFIG1"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_MONETIZATION_CONFIG1"].ToString()) : 0;
                                    mr.idgdaMonetizationConfigSetor = reader["IDGDA_MONETIZATION_CONFIG2"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_MONETIZATION_CONFIG2"].ToString()) : 0;

                                    //HIGS
                                    mr.hig1Id = reader["HIG1ID"] != DBNull.Value ? Convert.ToInt32(reader["HIG1ID"].ToString()) : 0;
                                    mr.hig2Id = reader["HIG2ID"] != DBNull.Value ? Convert.ToInt32(reader["HIG2ID"].ToString()) : 0;
                                    mr.hig3Id = reader["HIG3ID"] != DBNull.Value ? Convert.ToInt32(reader["HIG3ID"].ToString()) : 0;
                                    mr.hig4Id = reader["HIG4ID"] != DBNull.Value ? Convert.ToInt32(reader["HIG4ID"].ToString()) : 0;
                                    mr.hisId = reader["HISID"] != DBNull.Value ? Convert.ToInt32(reader["HISID"].ToString()) : 0;

                                    mrs.Add(mr);
                                }
                                catch (Exception ex)
                                {

                                }

                            }
                        }
                    }
                }
                catch (Exception)
                {

                }

                connection.Close();
            }
            return mrs;
        }
        public static bool thisMonetization(int transactionID, List<string> datas, long t_id)
        {


            try
            {
                //Varrer as datas retornadas
                foreach (string dt in datas)
                {
                    Log.insertLogTransaction(t_id.ToString(), "DATE MONETIZATION", $"START - {dt}", "");
                    // Expiração de moedas
                    List<monetizationHierarchyInformation> hierarInformation = new List<monetizationHierarchyInformation>();
                    hierarInformation = returnHierarchyInformation(dt);

                    List<MonetizationResultsModel> mrs = new List<MonetizationResultsModel>();

                    using (SqlConnection connection = new SqlConnection(Database.Conn))
                    {
                        connection.Open();
                        try
                        {
                            StringBuilder stb = new StringBuilder();
                            stb.AppendFormat("DECLARE @DATAINICIAL AS DATE; SET @DATAINICIAL = '{0}'; ", dt);
                            stb.Append(" ");
                            stb.Append("SELECT R.IDGDA_RESULT, R.INDICADORID, R.TRANSACTIONID, R.RESULT, R.CREATED_AT, R.IDGDA_COLLABORATORS, R.FACTORS, R.DELETED_AT, ");
                            stb.Append("       MAX(CL.ACTIVE) AS ACTIVE, ");
                            stb.Append("       MAX(HIS.GOAL) AS GOAL, ");
                            stb.Append("       MAX(HIS.GOAL_NIGHT) AS GOAL_NIGHT, ");
                            stb.Append("       MAX(HIS.GOAL_LATENIGHT) AS GOAL_LATENIGHT, ");
                            stb.Append("       I.WEIGHT AS WEIGHT, ");
                            stb.Append("       HHR.LEVELWEIGHT AS HIERARCHYLEVEL, ");
                            stb.Append("       MAX(CL.PERIODO) AS PERIODO, ");
                            stb.Append("       MAX(HIG1.MONETIZATION) AS COIN1, ");
                            stb.Append("       MAX(HIG1.MONETIZATION_NIGHT) AS COIN1_NIGHT, ");
                            stb.Append("       MAX(HIG1.MONETIZATION_LATENIGHT) AS COIN1_LATENIGHT, ");
                            stb.Append("       MAX(HIG2.MONETIZATION) AS COIN2, ");
                            stb.Append("       MAX(HIG2.MONETIZATION_NIGHT) AS COIN2_NIGHT, ");
                            stb.Append("       MAX(HIG2.MONETIZATION_LATENIGHT) AS COIN2_LATENIGHT, ");
                            stb.Append("       MAX(HIG3.MONETIZATION) AS COIN3, ");
                            stb.Append("       MAX(HIG3.MONETIZATION_NIGHT) AS COIN3_NIGHT, ");
                            stb.Append("       MAX(HIG3.MONETIZATION_LATENIGHT) AS COIN3_LATENIGHT, ");
                            stb.Append("       MAX(HIG4.MONETIZATION) AS COIN4, ");
                            stb.Append("       MAX(HIG4.MONETIZATION_NIGHT) AS COIN4_NIGHT, ");
                            stb.Append("       MAX(HIG4.MONETIZATION_LATENIGHT) AS COIN4_LATENIGHT, ");
                            stb.Append("       CL.IDGDA_SECTOR, ");
                            stb.Append("       MAX(I.TYPE) AS TYPE, ");
                            stb.Append("       MAX(HIG1.METRIC_MIN) AS MIN1,  ");
                            stb.Append("       MAX(HIG1.METRIC_MIN_NIGHT) AS MIN1_NIGHT, ");
                            stb.Append("       MAX(HIG1.METRIC_MIN_LATENIGHT) AS MIN1_LATENIGHT, ");
                            stb.Append("       MAX(HIG2.METRIC_MIN) AS MIN2,  ");
                            stb.Append("       MAX(HIG2.METRIC_MIN_NIGHT) AS MIN2_NIGHT,  ");
                            stb.Append("       MAX(HIG2.METRIC_MIN_LATENIGHT) AS MIN2_LATENIGHT, ");
                            stb.Append("       MAX(HIG3.METRIC_MIN) AS MIN3,  ");
                            stb.Append("       MAX(HIG3.METRIC_MIN_NIGHT) AS MIN3_NIGHT,  ");
                            stb.Append("       MAX(HIG3.METRIC_MIN_LATENIGHT) AS MIN3_LATENIGHT, ");
                            stb.Append("       MAX(HIG4.METRIC_MIN) AS MIN4,  ");
                            stb.Append("       MAX(HIG4.METRIC_MIN_NIGHT) AS MIN4_NIGHT,  ");
                            stb.Append("       MAX(HIG4.METRIC_MIN_LATENIGHT) AS MIN4_LATENIGHT, ");
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
                            stb.Append("       MAX(CL.NOME_CEO) AS 'NOME CEO', ");
                            stb.Append("	   '' AS ESCALADO,  ");
                            stb.Append("       '' AS LOGADO,   ");
                            stb.Append("	   '' AS ESCALADOLOGADO  ");

                            //Expiração moedas
                            stb.Append(", MAX(MGP.PAUSE) AS 'PAUSE',  ");
                            stb.Append("MAX(MG1.IDGDA_MONETIZATION_CONFIG) AS 'IDGDA_MONETIZATION_CONFIG1', ");
                            stb.Append("MAX(MG2.IDGDA_MONETIZATION_CONFIG) AS 'IDGDA_MONETIZATION_CONFIG2', ");
                            stb.Append("MAX(MG1.DAYS) AS 'DIAS SITE', ");
                            stb.Append("MAX(MG2.DAYS) AS 'DIAS SETOR' ");


                            stb.Append(", MAX(HIG1.ID) AS HIG1ID, ");
                            stb.Append("MAX(HIG2.ID) AS HIG2ID, ");
                            stb.Append("MAX(HIG3.ID) AS HIG3ID, ");
                            stb.Append("MAX(HIG4.ID) AS HIG4ID, ");
                            stb.Append("MAX(HIS.ID) AS HISID ");

                            stb.Append("FROM GDA_RESULT (NOLOCK) AS R ");
                            stb.Append("LEFT JOIN ");
                            stb.Append("( ");
                            stb.Append("SELECT IDGDA_COLLABORATORS, ACTIVE, MATRICULA_SUPERVISOR, MATRICULA_COORDENADOR, NOME_SUPERVISOR, ");
                            stb.Append("NOME_COORDENADOR, MATRICULA_GERENTE_II, NOME_GERENTE_II,MATRICULA_GERENTE_I, NOME_GERENTE_I, ");
                            stb.Append("MATRICULA_DIRETOR, NOME_DIRETOR, MATRICULA_CEO, NOME_CEO, PERIODO, ");
                            stb.Append("CASE WHEN IDGDA_SUBSECTOR IS NOT NULL THEN IDGDA_SUBSECTOR ");
                            stb.Append("ELSE IDGDA_SECTOR END AS IDGDA_SECTOR, SITE ");
                            stb.Append("FROM GDA_COLLABORATORS_DETAILS (NOLOCK) CL ");
                            stb.Append("WHERE CL.CREATED_AT = @DATAINICIAL ");
                            stb.Append(") AS CL ON CL.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");

                            //Expiração moedas
                            stb.Append("LEFT JOIN GDA_MONETIZATION_CONFIG_PAUSE (NOLOCK) MGP ON MGP.DELETED_AT IS NULL ");
                            stb.Append("LEFT JOIN GDA_SITE (NOLOCK) AS SS ON SS.SITE = CL.SITE ");
                            stb.Append("LEFT JOIN GDA_MONETIZATION_CONFIG (NOLOCK) AS MG1 ON MG1.DELETED_AT IS NULL AND MG1.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE = 2 AND MG1.ID_REFERER = SS.IDGDA_SITE ");
                            stb.Append("LEFT JOIN GDA_MONETIZATION_CONFIG (NOLOCK) AS MG2 ON MG2.DELETED_AT IS NULL AND MG2.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE = 1 AND MG2.ID_REFERER = CL.IDGDA_SECTOR ");

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
                            //stb.Append("  AND HIG1.MONETIZATION > 0 ");
                            stb.Append("  AND (HIG1.MONETIZATION > 0 OR HIG1.MONETIZATION_NIGHT > 0 OR HIG1.MONETIZATION_LATENIGHT > 0) ");
                            stb.Append("  AND R.DELETED_AT IS NULL ");
                            stb.Append("GROUP BY IDGDA_RESULT, ");
                            stb.Append("         INDICADORID, ");
                            stb.Append("         R.TRANSACTIONID, ");
                            stb.Append("         RESULT, ");
                            stb.Append("         R.CREATED_AT, ");
                            stb.Append("         R.IDGDA_COLLABORATORS, ");
                            stb.Append("         FACTORS, ");
                            stb.Append("         R.DELETED_AT, ");
                            stb.Append("         CL.IDGDA_SECTOR, ");
                            stb.Append("         I.WEIGHT, ");
                            stb.Append("         HHR.LEVELWEIGHT ");

                            using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                            {
                                command.CommandTimeout = 1800;
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        try
                                        {
                                            MonetizationResultsModel mr = new MonetizationResultsModel();

                                            //if (reader["ACTIVE"].ToString() != "true")
                                            //{
                                            //    continue;
                                            //}

                                            mr.idCollaborator = reader["IDGDA_COLLABORATORS"].ToString();
                                            mr.idIndicator = reader["INDICADORID"].ToString();
                                            mr.idResult = reader["IDGDA_RESULT"].ToString();
                                            mr.idSector = reader["IDGDA_SECTOR"].ToString();
                                            mr.idCheckingAccount = 0;
                                            mr.indicatorWeight = reader["WEIGHT"].ToString();
                                            mr.hierarchyLevel = reader["HIERARCHYLEVEL"].ToString();

                                            mr.fatores = reader["FACTORS"].ToString();
                                            mr.fator0 = Convert.ToDouble(reader["FACTORS"].ToString().Split(";")[0]);
                                            mr.fator1 = Convert.ToDouble(reader["FACTORS"].ToString().Split(";")[1]);
                                            mr.conta = reader["CONTA"].ToString();
                                            mr.melhor = reader["BETTER"].ToString();

                                            string periodo = reader["PERIODO"].ToString();
                                            if (periodo == "DIURNO")
                                            {
                                                if (reader["GOAL"] == DBNull.Value)
                                                {
                                                    continue;
                                                }
                                                mr.meta = double.Parse(reader["GOAL"].ToString());
                                                mr.G1 = double.Parse(reader["MIN1"].ToString());
                                                mr.G2 = double.Parse(reader["MIN2"].ToString());
                                                mr.G3 = double.Parse(reader["MIN3"].ToString());
                                                mr.G4 = double.Parse(reader["MIN4"].ToString());
                                                mr.C1 = double.Parse(reader["COIN1"].ToString());
                                                mr.C2 = double.Parse(reader["COIN2"].ToString());
                                                mr.C3 = double.Parse(reader["COIN3"].ToString());
                                                mr.C4 = double.Parse(reader["COIN4"].ToString());
                                            }
                                            else if (periodo == "NOTURNO")
                                            {
                                                if (reader["GOAL_NIGHT"] == DBNull.Value)
                                                {
                                                    continue;
                                                }
                                                mr.meta = double.Parse(reader["GOAL_NIGHT"].ToString());
                                                mr.G1 = double.Parse(reader["MIN1_NIGHT"].ToString());
                                                mr.G2 = double.Parse(reader["MIN2_NIGHT"].ToString());
                                                mr.G3 = double.Parse(reader["MIN3_NIGHT"].ToString());
                                                mr.G4 = double.Parse(reader["MIN4_NIGHT"].ToString());
                                                mr.C1 = double.Parse(reader["COIN1_NIGHT"].ToString());
                                                mr.C2 = double.Parse(reader["COIN2_NIGHT"].ToString());
                                                mr.C3 = double.Parse(reader["COIN3_NIGHT"].ToString());
                                                mr.C4 = double.Parse(reader["COIN4_NIGHT"].ToString());
                                            }
                                            else if (periodo == "MADRUGADA")
                                            {
                                                if (reader["GOAL_LATENIGHT"] == DBNull.Value)
                                                {
                                                    continue;
                                                }
                                                mr.meta = double.Parse(reader["GOAL_LATENIGHT"].ToString());
                                                mr.G1 = double.Parse(reader["MIN1_LATENIGHT"].ToString());
                                                mr.G2 = double.Parse(reader["MIN2_LATENIGHT"].ToString());
                                                mr.G3 = double.Parse(reader["MIN3_LATENIGHT"].ToString());
                                                mr.G4 = double.Parse(reader["MIN4_LATENIGHT"].ToString());
                                                mr.C1 = double.Parse(reader["COIN1_LATENIGHT"].ToString());
                                                mr.C2 = double.Parse(reader["COIN2_LATENIGHT"].ToString());
                                                mr.C3 = double.Parse(reader["COIN3_LATENIGHT"].ToString());
                                                mr.C4 = double.Parse(reader["COIN4_LATENIGHT"].ToString());
                                            }
                                            else
                                            {
                                                continue;
                                            }


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

                                            mr.sumDiasLogados = 0;
                                            mr.sumDiasEscalados = 0;
                                            mr.sumDiasLogadosEEscalados = 0;

                                            //Expiração de moedas
                                            mr.duePause = reader["PAUSE"] != DBNull.Value ? Convert.ToInt32(reader["PAUSE"].ToString()) : 0;
                                            mr.daySite = reader["DIAS SITE"] != DBNull.Value ? Convert.ToInt32(reader["DIAS SITE"].ToString()) : 0;
                                            mr.daySetor = reader["DIAS SETOR"] != DBNull.Value ? Convert.ToInt32(reader["DIAS SETOR"].ToString()) : 0;
                                            mr.idgdaMonetizationConfigSite = reader["IDGDA_MONETIZATION_CONFIG1"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_MONETIZATION_CONFIG1"].ToString()) : 0;
                                            mr.idgdaMonetizationConfigSetor = reader["IDGDA_MONETIZATION_CONFIG2"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_MONETIZATION_CONFIG2"].ToString()) : 0;

                                            //HIGS
                                            mr.hig1Id = reader["HIG1ID"] != DBNull.Value ? Convert.ToInt32(reader["HIG1ID"].ToString()) : 0;
                                            mr.hig2Id = reader["HIG2ID"] != DBNull.Value ? Convert.ToInt32(reader["HIG2ID"].ToString()) : 0;
                                            mr.hig3Id = reader["HIG3ID"] != DBNull.Value ? Convert.ToInt32(reader["HIG3ID"].ToString()) : 0;
                                            mr.hig4Id = reader["HIG4ID"] != DBNull.Value ? Convert.ToInt32(reader["HIG4ID"].ToString()) : 0;
                                            mr.hisId = reader["HISID"] != DBNull.Value ? Convert.ToInt32(reader["HISID"].ToString()) : 0;

                                            mrs.Add(mr);
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                    }
                                }
                            }

                            //Adicionando os indicadores de taxa de acesso
                            //List<MonetizationResultsModel> mrsAccess = new List<MonetizationResultsModel>();
                            //mrsAccess = monetizaAccess(dt);
                            //mrs = mrs.Concat(mrsAccess).ToList();

                            List<MonetizationResultsModel> mrsAccess = monetizationTxIndicatorAccess(dt);
                            mrs = mrs.Concat(mrsAccess).ToList();

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
                                    doMonetizationHierarchyNew(transactionID, dt, mr.matriculaSupervisor, mr, hierarInformation);
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
                                    doMonetizationHierarchyNew(transactionID, dt, mr.matriculaCoordenador, mr, hierarInformation);
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
                                    doMonetizationHierarchyNew(transactionID, dt, mr.matriculaGerenteii, mr, hierarInformation);
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
                                    doMonetizationHierarchyNew(transactionID, dt, mr.matriculaGerentei, mr, hierarInformation);
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
                                    doMonetizationHierarchyNew(transactionID, dt, mr.matriculaDiretor, mr, hierarInformation);
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
                                    doMonetizationHierarchyNew(transactionID, dt, mr.matriculaCeo, mr, hierarInformation);
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
                    Log.insertLogTransaction(t_id.ToString(), "DATE MONETIZATION", $"CONCLUDED - {dt}", "");
                }
            }
            catch (Exception)
            {

            }
            return true;
        }

        public static bool dueMonetizationExpiration()
        {
            bool retorno = false;

            try
            {
                int pause = 0;
                int pauseReprocesso = 0;

                //Processo de expiração realizar de madrugada 1 vez.
                expireCoins();

                //Pega informação do pause
                getDuePause(out pause, out pauseReprocesso);

                if (pauseReprocesso == 0)
                {
                    if (pause == 1)
                    {
                        // Congelar
                        dueFreezing();
                    }
                    else
                    {
                        // Descongelar
                        dueDefrosting();
                    }
                }
                else
                {
                    if (pause == 1)
                    {
                        // Não faz nada
                    }
                    else
                    {
                        // Reprocessa Todos as listagens de configs novos do passado
                        dueReprocess();
                    }
                }


            }
            catch (Exception)
            {

            }

            return retorno;
        }

        public static bool monetization(int transactionID, long t_id)
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

                DateTime dataLimite = DateTime.ParseExact("2024-08-01", "yyyy-MM-dd", null);

                if (dataMaisAntiga < dataLimite)
                {
                    dataMaisAntiga = dataLimite;
                }

                TimeSpan dd = dataAtual.Subtract(dataMaisAntiga);

                diferencaDias = dd.Days;

            }
            catch (Exception)
            {

            }

            //if (diferencaDias == 0)
            //{
            //    diferencaDias = 31;
            //}

            datas.Clear();

            if (diferencaDias != 0)
            {
                for (int i = diferencaDias; i > 0; i--)
                {
                    // Subtrai o número de dias da iteração da data atual
                    DateTime data = dataAtual.AddDays(-i);

                    // Formata a data no formato "yyyy-MM-dd" e adiciona à lista
                    string dataFormatada = data.ToString("yyyy-MM-dd");
                    datas.Add(dataFormatada);
                }

                thisMonetization(transactionID, datas, t_id);
            }

            Log.insertLogTransaction(t_id.ToString(), "MONETIZATION", "CONCLUDED", "");
            return true;
        }




        public static List<MonetizationResultsModel> getListHierarchy(List<MonetizationResultsModel> mrs, string Hierarchy)
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
                    //Adicionado para Indicador de Acesso
                    sumDiasLogados = grupo.Sum(item => item.sumDiasLogados),
                    sumDiasEscalados = grupo.Sum(item => item.sumDiasEscalados),
                    sumDiasLogadosEEscalados = grupo.Count(item => item.sumDiasEscalados == 1 && item.sumDiasLogados == 1),

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
                    //Adicionado para Indicador de Acesso
                    sumDiasLogados = grupo.Sum(item => item.sumDiasLogados),
                    sumDiasEscalados = grupo.Sum(item => item.sumDiasEscalados),
                    sumDiasLogadosEEscalados = grupo.Count(item => item.sumDiasEscalados == 1 && item.sumDiasLogados == 1),
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
                    //Adicionado para Indicador de Acesso
                    sumDiasLogados = grupo.Sum(item => item.sumDiasLogados),
                    sumDiasEscalados = grupo.Sum(item => item.sumDiasEscalados),
                    sumDiasLogadosEEscalados = grupo.Count(item => item.sumDiasEscalados == 1 && item.sumDiasLogados == 1),
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
                    //Adicionado para Indicador de Acesso
                    sumDiasLogados = grupo.Sum(item => item.sumDiasLogados),
                    sumDiasEscalados = grupo.Sum(item => item.sumDiasEscalados),
                    sumDiasLogadosEEscalados = grupo.Count(item => item.sumDiasEscalados == 1 && item.sumDiasLogados == 1),
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
                    //Adicionado para Indicador de Acesso
                    sumDiasLogados = grupo.Sum(item => item.sumDiasLogados),
                    sumDiasEscalados = grupo.Sum(item => item.sumDiasEscalados),
                    sumDiasLogadosEEscalados = grupo.Count(item => item.sumDiasEscalados == 1 && item.sumDiasLogados == 1),
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
                    //Adicionado para Indicador de Acesso
                    sumDiasLogados = grupo.Sum(item => item.sumDiasLogados),
                    sumDiasEscalados = grupo.Sum(item => item.sumDiasEscalados),
                    sumDiasLogadosEEscalados = grupo.Count(item => item.sumDiasEscalados == 1 && item.sumDiasLogados == 1),
                }).ToList();
            }


            return mrsReturn;
        }

        public static void dueReprocess()
        {
            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                connection.Open();
                try
                {
                    StringBuilder stb = new StringBuilder();
                    stb.Append("DECLARE @DATAINICIAL DATETIME; SET @DATAINICIAL = CONVERT(DATE, DATEADD(DAY, -1, GETDATE())); ");
                    stb.Append("UPDATE CK ");
                    //stb.Append("SET DUE_AT = CONVERT(DATE, DATEADD(DAY, CKD.DIAS, GETDATE())), ");
                    stb.Append("SET DUE_AT = CONVERT(DATE, DATEADD(DAY, CKD.DIAS, CK.CREATED_AT)), ");
                    stb.Append("IDGDA_MONETIZATION_CONFIG = CKD.CONFIG ");
                    stb.Append("FROM GDA_CHECKING_ACCOUNT CK ");
                    stb.Append("LEFT JOIN  ");
                    stb.Append("(  ");
                    stb.Append("    SELECT  ");
                    stb.Append("        IDGDA_COLLABORATORS,  ");
                    stb.Append("        ACTIVE,  ");
                    stb.Append("        MATRICULA_SUPERVISOR,  ");
                    stb.Append("        MATRICULA_COORDENADOR,  ");
                    stb.Append("        NOME_SUPERVISOR,  ");
                    stb.Append("        NOME_COORDENADOR,  ");
                    stb.Append("        MATRICULA_GERENTE_II,  ");
                    stb.Append("        NOME_GERENTE_II, ");
                    stb.Append("        MATRICULA_GERENTE_I,  ");
                    stb.Append("        NOME_GERENTE_I,  ");
                    stb.Append("        MATRICULA_DIRETOR,  ");
                    stb.Append("        NOME_DIRETOR,  ");
                    stb.Append("        MATRICULA_CEO,  ");
                    stb.Append("        NOME_CEO,  ");
                    stb.Append("        PERIODO,  ");
                    stb.Append("        Cargo, ");
                    stb.Append("        CASE WHEN IDGDA_SUBSECTOR IS NOT NULL THEN IDGDA_SUBSECTOR ELSE IDGDA_SECTOR END AS IDGDA_SECTOR,  ");
                    stb.Append("        COALESCE(MG2_NEW.IDGDA_MONETIZATION_CONFIG, MG2.IDGDA_MONETIZATION_CONFIG,  ");
                    stb.Append("		 MG1_NEW.IDGDA_MONETIZATION_CONFIG, MG1.IDGDA_MONETIZATION_CONFIG) AS CONFIG, ");
                    stb.Append("        COALESCE(MG2_NEW.STARTED_AT, MG2.STARTED_AT, MG1_NEW.STARTED_AT, MG1.STARTED_AT) AS INICIO, ");
                    stb.Append("        COALESCE(MG2_NEW.DAYS, MG2.DAYS, MG1_NEW.DAYS, MG1.DAYS) AS DIAS ");
                    stb.Append(" ");
                    stb.Append("    FROM GDA_COLLABORATORS_DETAILS (NOLOCK) CL  ");
                    stb.Append("    LEFT JOIN GDA_SITE (NOLOCK) AS SS ON SS.SITE = CL.SITE  ");
                    stb.Append("	LEFT JOIN GDA_MONETIZATION_CONFIG (NOLOCK) AS MG1 ON MG1.DELETED_AT IS NULL  ");
                    stb.Append("	 AND MG1.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE = 2 AND MG1.ID_REFERER = SS.IDGDA_SITE  ");
                    stb.Append("    LEFT JOIN GDA_MONETIZATION_CONFIG (NOLOCK) AS MG2 ON MG2.DELETED_AT IS NULL  ");
                    stb.Append("	 AND MG2.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE = 1 AND MG2.ID_REFERER = CL.IDGDA_SECTOR  ");
                    stb.Append("    LEFT JOIN GDA_MONETIZATION_CONFIG (NOLOCK) AS MG1_NEW ON MG1_NEW.DELETED_AT IS NULL  ");
                    stb.Append("	 AND MG1_NEW.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE = 2 AND MG1_NEW.ID_REFERER = SS.IDGDA_SITE  ");
                    stb.Append("	 AND MG1_NEW.PAST_DATE = 1 AND MG1_NEW.REPROCESSED = 0 ");
                    stb.Append("    LEFT JOIN GDA_MONETIZATION_CONFIG (NOLOCK) AS MG2_NEW ON MG2_NEW.DELETED_AT IS NULL  ");
                    stb.Append("	 AND MG2_NEW.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE = 1 AND MG2_NEW.ID_REFERER = CL.IDGDA_SECTOR  ");
                    stb.Append("	 AND MG2_NEW.PAST_DATE = 1 AND MG2_NEW.REPROCESSED = 0 ");
                    stb.Append("    WHERE CL.CREATED_AT = @DATAINICIAL  ");
                    stb.Append("	 AND COALESCE(MG2_NEW.STARTED_AT, MG2.STARTED_AT, MG1_NEW.STARTED_AT, MG1.STARTED_AT) IS NOT NULL ");
                    stb.Append(") AS CKD ON CKD.IDGDA_COLLABORATORS = CK.COLLABORATOR_ID  ");
                    stb.Append("WHERE CK.INPUT_USED < CK.INPUT ");
                    stb.Append("    AND CK.INPUT > 0 ");
                    stb.Append("    AND (CKD.DIAS IS NOT NULL) ");
                    stb.Append("	AND CK.created_at >= CKD.INICIO; ");

                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                    }



                    StringBuilder stb2 = new StringBuilder();
                    stb2.Append("UPDATE GDA_MONETIZATION_CONFIG SET ");
                    stb2.Append("REPROCESSED = 1 ");
                    stb2.Append("WHERE REPROCESSED = 0 ");
                    //stb2.Append("AND PAST_DATE = 1 ");
                    stb2.Append("AND DELETED_AT IS NULL ");

                    using (SqlCommand command = new SqlCommand(stb2.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {

                }
                connection.Close();
            }
        }


        public static void dueDefrosting()
        {
            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                connection.Open();
                try
                {
                    StringBuilder stb = new StringBuilder();
                    stb.Append("DECLARE @DATAINICIAL DATETIME; SET @DATAINICIAL = CONVERT(DATE, DATEADD(DAY, -1, GETDATE())); ");
                    stb.Append("UPDATE CK SET ");
                    stb.Append("DUE_AT = CONVERT(DATE, DATEADD(DAY, CKD.DIAS, GETDATE())), ");
                    stb.Append("IDGDA_MONETIZATION_CONFIG =  CKD.CONFIG ");
                    stb.Append("FROM GDA_CHECKING_ACCOUNT CK ");
                    stb.Append("LEFT JOIN  ");
                    stb.Append("(  ");
                    stb.Append("    SELECT  ");
                    stb.Append("        IDGDA_COLLABORATORS,  ");
                    stb.Append("        ACTIVE,  ");
                    stb.Append("        MATRICULA_SUPERVISOR,  ");
                    stb.Append("        MATRICULA_COORDENADOR,  ");
                    stb.Append("        NOME_SUPERVISOR,  ");
                    stb.Append("        NOME_COORDENADOR,  ");
                    stb.Append("        MATRICULA_GERENTE_II,  ");
                    stb.Append("        NOME_GERENTE_II, ");
                    stb.Append("        MATRICULA_GERENTE_I,  ");
                    stb.Append("        NOME_GERENTE_I,  ");
                    stb.Append("        MATRICULA_DIRETOR,  ");
                    stb.Append("        NOME_DIRETOR,  ");
                    stb.Append("        MATRICULA_CEO,  ");
                    stb.Append("        NOME_CEO,  ");
                    stb.Append("        PERIODO,  ");
                    stb.Append("        Cargo, ");
                    stb.Append("        CASE WHEN IDGDA_SUBSECTOR IS NOT NULL THEN IDGDA_SUBSECTOR ELSE IDGDA_SECTOR END AS IDGDA_SECTOR,  ");
                    stb.Append("        COALESCE(MG2.IDGDA_MONETIZATION_CONFIG, MG1.IDGDA_MONETIZATION_CONFIG) AS CONFIG, ");
                    stb.Append("        COALESCE(MG2.DAYS, MG1.DAYS) AS DIAS ");
                    stb.Append("    FROM GDA_COLLABORATORS_DETAILS (NOLOCK) CL  ");
                    stb.Append("    LEFT JOIN GDA_SITE (NOLOCK) AS SS ON SS.SITE = CL.SITE  ");
                    stb.Append("    LEFT JOIN GDA_MONETIZATION_CONFIG (NOLOCK) AS MG1 ON MG1.DELETED_AT IS NULL  ");
                    stb.Append("	 AND MG1.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE = 2 AND MG1.ID_REFERER = SS.IDGDA_SITE  ");
                    stb.Append("    LEFT JOIN GDA_MONETIZATION_CONFIG (NOLOCK) AS MG2 ON MG2.DELETED_AT IS NULL  ");
                    stb.Append("	 AND MG2.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE = 1 AND MG2.ID_REFERER = CL.IDGDA_SECTOR  ");
                    stb.Append("    WHERE CL.CREATED_AT = @DATAINICIAL  ");
                    stb.Append(") AS CKD ON CKD.IDGDA_COLLABORATORS = CK.COLLABORATOR_ID  ");
                    stb.Append("WHERE ");
                    stb.Append("    CK.DUE_AT IS NULL  ");
                    stb.Append("    AND CK.INPUT_USED < CK.INPUT ");
                    stb.Append("    AND CK.INPUT > 0 ");
                    stb.Append("    AND CKD.DIAS IS NOT NULL; ");

                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {

                }
                connection.Close();
            }
        }

        public static void expireCoins()
        {
            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                connection.Open();
                try
                {
                    StringBuilder stb = new StringBuilder();
                    stb.Append("SELECT CA.ID, ");
                    stb.Append("       CA.INPUT, ");
                    stb.Append("       CA.COLLABORATOR_ID, ");
                    stb.Append("       CA.INPUT_USED, ");
                    stb.Append("       CA.COIN_EXPIRED ");

                    stb.Append("FROM GDA_CHECKING_ACCOUNT (NOLOCK) AS CA ");
                    stb.Append("INNER JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCU ON PCU.IDGDA_COLLABORATORS = CA.COLLABORATOR_ID ");
                    stb.Append("INNER JOIN GDA_PERSONA_USER (NOLOCK) AS PU ON PCU.IDGDA_PERSONA_USER = PU.IDGDA_PERSONA_USER ");
                    stb.Append("AND PU.IDGDA_PERSONA_USER_TYPE = 1 ");
                    stb.Append("AND PU.DELETED_AT IS NULL ");
                    stb.Append("WHERE CA.INPUT > 0 ");
                    stb.Append("  AND (CA.INPUT - CA.INPUT_USED) > 0 ");
                    stb.Append("  AND CA.DUE_AT IS NOT NULL ");
                    stb.Append("  AND CONVERT(DATE, CA.DUE_AT) < CONVERT(DATE, GETDATE()) ");

                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                int ID = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"].ToString()) : 0;
                                int INPUT = reader["INPUT"] != DBNull.Value ? Convert.ToInt32(reader["INPUT"].ToString()) : 0;
                                int COLLABORATOR_ID = reader["COLLABORATOR_ID"] != DBNull.Value ? Convert.ToInt32(reader["COLLABORATOR_ID"].ToString()) : 0;
                                int INPUT_USED = reader["INPUT_USED"] != DBNull.Value ? Convert.ToInt32(reader["INPUT_USED"].ToString()) : 0;
                                int COIN_EXPIRED = reader["COIN_EXPIRED"] != DBNull.Value ? Convert.ToInt32(reader["COIN_EXPIRED"].ToString()) : 0;
                                int LASTBALANCE = 0;
                                int IDGDA_SECTOR = 0;


                                StringBuilder stbBalance = new StringBuilder();
                                double balanceEnc = 0;
                                stbBalance.Append("SELECT TOP 1 BALANCE, IDGDA_SECTOR FROM GDA_CHECKING_ACCOUNT (NOLOCK) ");
                                stbBalance.AppendFormat("WHERE COLLABORATOR_ID = '{0}' ", COLLABORATOR_ID);
                                stbBalance.Append("ORDER BY CREATED_AT DESC ");

                                using (SqlCommand command4 = new SqlCommand(stbBalance.ToString(), connection))
                                {
                                    command4.CommandTimeout = 0;
                                    using (SqlDataReader reader2 = command4.ExecuteReader())
                                    {
                                        if (reader2.Read())
                                        {
                                            LASTBALANCE = reader2["BALANCE"] != DBNull.Value ? Convert.ToInt32(reader2["BALANCE"].ToString()) : 0;
                                            IDGDA_SECTOR = reader2["IDGDA_SECTOR"] != DBNull.Value ? Convert.ToInt32(reader2["IDGDA_SECTOR"].ToString()) : 0;
                                        }
                                    }
                                }

                                int coinExpired = INPUT - INPUT_USED;
                                int balanceFinal = LASTBALANCE - coinExpired;

                                //5
                                //3



                                //Realiza a expiração
                                StringBuilder stb2 = new StringBuilder();
                                stb2.Append("UPDATE GDA_CHECKING_ACCOUNT SET ");
                                stb2.Append($"INPUT_USED = {INPUT}, ");
                                stb2.Append($"COIN_EXPIRED = {coinExpired} ");
                                stb2.Append($"WHERE ID = {ID} ");
                                using (SqlCommand command2 = new SqlCommand(stb2.ToString(), connection))
                                {
                                    command2.ExecuteNonQuery();
                                }

                                //Regra adicionada, pois hoje quando ele perde moedas de um indicador ele não retira das moedas que podem ser gastas.. Evitar que fique negativo.
                                //Ira expirar apenas oq tiver de saudo.
                                if (balanceFinal < 0)
                                {
                                    coinExpired = LASTBALANCE;
                                    balanceFinal = 0;
                                }

                                //Insere a retiradas de moedas                    
                                StringBuilder stbCheckingAccount = new StringBuilder();
                                stbCheckingAccount.Append("INSERT INTO GDA_CHECKING_ACCOUNT (INPUT, OUTPUT, BALANCE, COLLABORATOR_ID, CREATED_AT, ");
                                stbCheckingAccount.Append(" GDA_INDICATOR_IDGDA_INDICATOR, GDA_ORDER_IDGDA_ORDER, CREATEDBYCOLLABORATORID, OBSERVATION, ");
                                stbCheckingAccount.Append("REASON, IDGDA_RESULT, RESULT_DATE, WEIGHT, IDGDA_SECTOR ");
                                stbCheckingAccount.Append(", IDGDA_MONETIZATION_CONFIG, DUE_AT, INPUT_USED, ID_EXPIRED, COIN_EXPIRED ");

                                stbCheckingAccount.Append(") OUTPUT INSERTED.ID VALUES( ");
                                stbCheckingAccount.AppendFormat("'{0}',", 0); //INPUT
                                stbCheckingAccount.AppendFormat("'{0}',", coinExpired); //OUTPUT
                                stbCheckingAccount.AppendFormat("'{0}',", balanceFinal); //BALANCE
                                stbCheckingAccount.AppendFormat("'{0}',", COLLABORATOR_ID); //COLLABORATOR_ID
                                stbCheckingAccount.AppendFormat("{0},", "GETDATE()"); //CREATED_AT
                                stbCheckingAccount.AppendFormat("{0},", "NULL"); //GDA_INDICATOR_IDGDA_INDICATOR
                                stbCheckingAccount.AppendFormat("{0},", "NULL"); //GDA_ORDER_IDGDA_ORDER
                                stbCheckingAccount.AppendFormat("{0},", "NULL"); //CREATEDBYCOLLABORATORID
                                stbCheckingAccount.AppendFormat("'{0}',", "Moedas Expiradas"); //OBSERVATION
                                stbCheckingAccount.AppendFormat("'{0}',", "Expiradas"); //REASON
                                stbCheckingAccount.AppendFormat("{0},", "NULL"); //IRGDA_RESULT
                                stbCheckingAccount.AppendFormat("{0},", "NULL"); //RESULT_DATE
                                stbCheckingAccount.AppendFormat("'{0}',", "1"); //WEIGHT
                                stbCheckingAccount.AppendFormat("'{0}'", IDGDA_SECTOR); //IDGDA_SECTOR

                                stbCheckingAccount.AppendFormat(",{0}", "NULL"); //IDGDA_MONETIZATION_CONFIG
                                stbCheckingAccount.AppendFormat(",{0}", "NULL"); //DUE_AT
                                stbCheckingAccount.AppendFormat(",0 "); //INPUT_USED
                                stbCheckingAccount.AppendFormat(",{0} ", ID); //ID_EXPIRED
                                stbCheckingAccount.AppendFormat(",0 "); //COIN_EXPIRED

                                stbCheckingAccount.Append("); ");

                                using (SqlCommand command3 = new SqlCommand(stbCheckingAccount.ToString(), connection))
                                {
                                    command3.ExecuteNonQuery();
                                }

                            }
                        }
                    }

                }
                catch (Exception)
                {

                }


                connection.Close();

            }
        }

        public static void getDuePause(out int pause, out int reprocessed)
        {
            pause = 0;
            reprocessed = 1;

            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                connection.Open();
                try
                {
                    StringBuilder stb = new StringBuilder();
                    stb.Append("SELECT PAUSE, REPROCESSED FROM GDA_MONETIZATION_CONFIG_PAUSE (NOLOCK) ");
                    stb.Append("WHERE DELETED_AT IS NULL AND REPROCESSED = 0 ");
                    stb.Append("ORDER BY 1 DESC ");

                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                pause = Convert.ToInt32(reader["PAUSE"].ToString());
                                reprocessed = Convert.ToInt32(reader["REPROCESSED"].ToString());
                            }
                        }
                    }

                    StringBuilder stb2 = new StringBuilder();
                    stb2.Append("UPDATE GDA_MONETIZATION_CONFIG_PAUSE ");
                    stb2.Append("SET REPROCESSED = 1 ");
                    stb2.Append("WHERE REPROCESSED = 0 AND DELETED_AT IS NULL ");
                    using (SqlCommand command = new SqlCommand(stb2.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {

                }


                connection.Close();

            }
        }




        public static void dueFreezing()
        {

            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                connection.Open();
                try
                {
                    StringBuilder stb = new StringBuilder();
                    stb.Append("UPDATE GDA_CHECKING_ACCOUNT  ");
                    stb.Append("SET DUE_AT = NULL  ");
                    stb.Append("WHERE DUE_AT > GETDATE() ");
                    stb.Append("AND INPUT_USED < INPUT ");

                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.ExecuteNonQuery();

                        //using (SqlDataReader reader = command.ExecuteReader())
                        //{
                        //    while (reader.Read())
                        //    {

                        //    }
                        //}
                    }
                }
                catch (Exception)
                {

                }
                connection.Close();
            }

        }


        public static List<string> getDateTransaction(int transactionID)
        {


            List<string> datas = new List<string>();

            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                connection.Open();
                try
                {
                    StringBuilder stb = new StringBuilder();
                    stb.Append("SELECT DISTINCT(CONVERT(varchar, CREATED_AT, 23)) AS CREATED_AT FROM GDA_RESULT (NOLOCK) ");
                    stb.AppendFormat("WHERE  TRANSACTIONID = {0}; ", transactionID);

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

        public static void doMonetizationHierarchyNew(int transactionID, string dateM, string idReferer, MonetizationResultsModel mr, List<monetizationHierarchyInformation> hierarInformation)
        {
            //Pega Hierarquia
            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                connection.Open();
                try
                {

                    List<MonetizationHierarchy> mons = getMonetizationHierarchyNew(dateM, idReferer, mr.idIndicator);
                    monetizationHierarchyInformation infHierarchyExpired = new monetizationHierarchyInformation();

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

                        //Expiração Moedas
                        infHierarchyExpired = hierarInformation.Find(i => i.idCollaborator == Convert.ToInt32(idReferer));

                        //Expiração Moedas
                        int? idgdaMonetizationConfig = null;
                        if (infHierarchyExpired.duePause == 0)
                        {
                            //if (infHierarchyExpired.daySetor > 0)
                            //{
                            //    DateTime dueDateNew = DateTime.Now.AddDays(infHierarchyExpired.daySetor).Date;
                            //    infHierarchyExpired.dueDateCalculed = dueDateNew;
                            //    idgdaMonetizationConfig = infHierarchyExpired.idgdaMonetizationConfigSetor;
                            //}
                            //else 
                            if (infHierarchyExpired.daySite > 0)
                            {
                                DateTime dueDateNew = DateTime.Now.AddDays(infHierarchyExpired.daySite).Date;
                                infHierarchyExpired.dueDateCalculed = dueDateNew;
                                idgdaMonetizationConfig = infHierarchyExpired.idgdaMonetizationConfigSite;
                            }
                            else
                            {
                                infHierarchyExpired.dueDateCalculed = null;
                            }
                        }

                        //Inserir moedas                    
                        StringBuilder stbCheckingAccount = new StringBuilder();
                        stbCheckingAccount.Append("INSERT INTO GDA_CHECKING_ACCOUNT (INPUT, OUTPUT, BALANCE, COLLABORATOR_ID, CREATED_AT, ");
                        stbCheckingAccount.Append(" GDA_INDICATOR_IDGDA_INDICATOR, GDA_ORDER_IDGDA_ORDER, CREATEDBYCOLLABORATORID, OBSERVATION, ");
                        stbCheckingAccount.Append("REASON, IDGDA_RESULT, RESULT_DATE, WEIGHT, IDGDA_SECTOR ");

                        //Expiração Moedas
                        stbCheckingAccount.Append(", IDGDA_MONETIZATION_CONFIG, DUE_AT, INPUT_USED, ID_EXPIRED, COIN_EXPIRED ");

                        stbCheckingAccount.Append(") OUTPUT INSERTED.ID VALUES( ");
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

                        //Expiração Moedas
                        string dueDateString = infHierarchyExpired.dueDateCalculed is null ? "NULL" : $"'{infHierarchyExpired.dueDateCalculed?.ToString("yyyy-MM-dd")}'";
                        string idgdaMoneString = idgdaMonetizationConfig is null ? "NULL" : $"'{idgdaMonetizationConfig.ToString()}'";
                        stbCheckingAccount.AppendFormat(",{0}", idgdaMoneString); //IDGDA_MONETIZATION_CONFIG
                        stbCheckingAccount.AppendFormat(",{0}", dueDateString); //DUE_AT
                        stbCheckingAccount.AppendFormat(",0 "); //INPUT_USED
                        stbCheckingAccount.AppendFormat(",NULL"); //ID_EXPIRED
                        stbCheckingAccount.AppendFormat(",0 "); //COIN_EXPIRED

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

        public static double doMonetizationAgent(int transactionID, string dateM, MonetizationResultsModel mr)
        {
            double retorno = 0;
            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                connection.Open();
                try
                {



                    mr.conta = mr.conta.Replace("#fator0", mr.fator0.ToString()).Replace("#fator1", mr.fator1.ToString());

                    //if (mr.idIndicator == "10000013")
                    //{
                    //    //Calculo do Agente
                    //    mr.conta = $"{mr.sumDiasLogadosEEscalados} / {mr.sumDiasEscalados}";
                    //}
                    //else
                    //{
                    //    mr.conta = mr.conta.Replace("#fator0", mr.fator0.ToString()).Replace("#fator1", mr.fator1.ToString());
                    //}

                    //Realiza a conta de resultado
                    DataTable dt = new DataTable();
                    double resultado = 0;

                    if (mr.idIndicator != "10000013" && mr.idIndicator != "10000014")
                    {
                        if (mr.fator0.ToString() == "0" && mr.fator1.ToString() == "0")
                        {
                            return 0;
                        }
                    }

                    try
                    {
                        string result = "";
                        if (mr.idIndicator == "10000013")
                        {
                            if (mr.sumDiasLogados == 1 && mr.sumDiasEscalados == 1)
                            {
                                result = "1";
                            }
                            else
                            {
                                result = "0";
                            }


                        }
                        else if (mr.idIndicator == "10000014")
                        {
                            if (mr.sumDiasLogados == 1)
                            {
                                result = "1";
                            }
                            else
                            {
                                result = "0";
                            }
                        }
                        else
                        {
                            result = dt.Compute(mr.conta, "").ToString();
                        }

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

                    int higId = 0;

                    atingimentoMeta = atingimentoMeta * 100;

                    double moedas = 0;
                    //Verifica a qual grupo pertence
                    if (atingimentoMeta >= mr.G1)
                    {
                        higId = mr.hig1Id;
                        moedas = mr.C1;
                    }
                    else if (atingimentoMeta >= mr.G2)
                    {
                        higId = mr.hig2Id;
                        moedas = mr.C2;
                    }
                    else if (atingimentoMeta >= mr.G3)
                    {
                        higId = mr.hig3Id;
                        moedas = mr.C3;
                    }
                    else if (atingimentoMeta >= mr.G4)
                    {
                        higId = mr.hig4Id;
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

                    //if (monRetiradaDia > 0)
                    //{
                    //    bool parou = true;
                    //}

                    //Deflator Devolvendo moedas
                    if (monRetiradaDia > 0 && inputMoedas > 0)
                    {
                        inputMoedas += monRetiradaDia;
                    }
                    //Deflator Retirando moedas
                    if (monetizacaoDia > 0 && outptMoedas < 0)
                    {
                        outptMoedas -= monetizacaoDia;
                    }

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

                            //balanceEnc = balanceEnc + inputMoedas;
                            double conta = inputMoedas - monetizacaoDia;
                            inputMoedas = conta;

                            balanceEnc = balanceEnc + inputMoedas;

                            retorno = inputMoedas;
                        }
                        else
                        {
                            //balanceEnc = balanceEnc + outptMoedas;

                            double conta = outptMoedas - monRetiradaDia;

                            balanceEnc = balanceEnc + conta;
                            outptMoedas = conta * (-1);

                            //Retire as moedas que podem ser utilizadas.
                            reserveCoins(Convert.ToInt32(mr.idCollaborator), Convert.ToInt32(outptMoedas));

                            retorno = conta;
                        }


                        //Expiração Moedas
                        int? idgdaMonetizationConfig = null;
                        if (mr.duePause == 0)
                        {
                            if (mr.daySetor > 0)
                            {
                                DateTime dueDateNew = DateTime.Now.AddDays(mr.daySetor).Date;
                                mr.dueDateCalculed = dueDateNew;
                                idgdaMonetizationConfig = mr.idgdaMonetizationConfigSetor;
                            }
                            else if (mr.daySite > 0)
                            {
                                DateTime dueDateNew = DateTime.Now.AddDays(mr.daySite).Date;
                                mr.dueDateCalculed = dueDateNew;
                                idgdaMonetizationConfig = mr.idgdaMonetizationConfigSite;
                            }
                            else
                            {
                                mr.dueDateCalculed = null;
                            }
                        }

                        string resultadoStr = mr.idResult;
                        if (resultadoStr == "")
                        {
                            resultadoStr = "NULL";
                        }
                        else
                        {
                            resultadoStr = $"'{resultadoStr}'";
                        }

                        //Inserir moedas                    
                        StringBuilder stbCheckingAccount = new StringBuilder();
                        stbCheckingAccount.Append("INSERT INTO GDA_CHECKING_ACCOUNT (INPUT, OUTPUT, BALANCE, COLLABORATOR_ID, CREATED_AT, ");
                        stbCheckingAccount.Append(" GDA_INDICATOR_IDGDA_INDICATOR, GDA_ORDER_IDGDA_ORDER, CREATEDBYCOLLABORATORID, OBSERVATION, ");
                        stbCheckingAccount.Append("REASON, IDGDA_RESULT, RESULT_DATE, WEIGHT, IDGDA_SECTOR ");
                        //Expiração Moedas
                        stbCheckingAccount.Append(", IDGDA_MONETIZATION_CONFIG, DUE_AT, INPUT_USED, ID_EXPIRED, COIN_EXPIRED ");

                        stbCheckingAccount.Append(", IDGDA_HISTORY_INDICATOR_GROUP, IDGDA_HISTORY_INDICATOR_SECTORS ");

                        stbCheckingAccount.Append(") OUTPUT INSERTED.ID VALUES( ");
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
                        stbCheckingAccount.AppendFormat("{0},", resultadoStr); //IRGDA_RESULT
                        stbCheckingAccount.AppendFormat("'{0}',", dateM); //RESULT_DATE
                        stbCheckingAccount.AppendFormat("'{0}',", mr.indicatorWeight); //WEIGHT
                        stbCheckingAccount.AppendFormat("'{0}'", mr.idSector); //IDGDA_SECTOR

                        //Expiração Moedas
                        string dueDateString = mr.dueDateCalculed is null ? "NULL" : $"'{mr.dueDateCalculed?.ToString("yyyy-MM-dd")}'";
                        string idgdaMoneString = idgdaMonetizationConfig is null ? "NULL" : $"'{idgdaMonetizationConfig.ToString()}'";
                        stbCheckingAccount.AppendFormat(",{0}", idgdaMoneString); //IDGDA_MONETIZATION_CONFIG
                        stbCheckingAccount.AppendFormat(",{0}", dueDateString); //DUE_AT
                        stbCheckingAccount.AppendFormat(",0 "); //INPUT_USED
                        stbCheckingAccount.AppendFormat(",NULL"); //ID_EXPIRED
                        stbCheckingAccount.AppendFormat(",0 "); //COIN_EXPIRED

                        stbCheckingAccount.AppendFormat(",{0} ", higId); //IDGDA_HISTORY_INDICATOR_GROUP
                        stbCheckingAccount.AppendFormat(",{0} ", mr.hisId); //IDGDA_HISTORY_INDICATOR_SECTORS

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
                        stbConsolChecking.AppendFormat("{0}, ", resultadoStr); //IDGDA_RESULT
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


        public static bool reserveCoins(int idCollaborator, int ballanceCart)
        {
            bool retorno = false;

            StringBuilder stb = new StringBuilder();
            stb.Append("SELECT ID, INPUT, INPUT_USED FROM GDA_CHECKING_ACCOUNT (NOLOCK) ");
            stb.Append("WHERE  ");
            stb.Append("INPUT > 0 AND ");
            stb.Append($"COLLABORATOR_ID = {idCollaborator}  ");
            stb.Append("AND ((INPUT - INPUT_USED) > 0 OR INPUT_USED IS NULL) ");
            stb.Append("ORDER BY  ");

            stb.Append("CASE ");
            stb.Append("    WHEN INPUT_USED > 0 THEN 1 ");
            stb.Append("    ELSE 0 ");
            stb.Append("END, ");
            stb.Append("CASE ");
            stb.Append("	WHEN DUE_AT IS NULL THEN 1 ");
            stb.Append("	ELSE 0 ");
            stb.Append("END, ");
            stb.Append("	DUE_AT ASC, ");
            stb.Append("	CONVERT(DATE, CREATED_AT) DESC, ");
            stb.Append("INPUT ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read() && ballanceCart > 0)
                            {

                                //ballanceCart 7

                                //input 10
                                //inputUsed 3
                                //inputLeft 7
                                int id = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0;
                                int input = reader["INPUT"] != DBNull.Value ? Convert.ToInt32(reader["INPUT"]) : 0;
                                int inputUsed = reader["INPUT_USED"] != DBNull.Value ? Convert.ToInt32(reader["INPUT_USED"]) : 0;
                                int inputLeft = input - inputUsed;

                                int coinsUsed = 0;

                                if (inputLeft > ballanceCart)
                                {
                                    coinsUsed = ballanceCart;
                                    ballanceCart = 0;
                                }
                                else if (inputLeft == ballanceCart)
                                {
                                    coinsUsed = inputLeft;
                                    ballanceCart = 0;
                                }
                                else
                                {
                                    ballanceCart = ballanceCart - inputLeft;
                                    coinsUsed = inputLeft;
                                }

                                coinsUsed = coinsUsed + inputUsed;

                                StringBuilder stbUpdate = new StringBuilder();
                                stbUpdate.Append($"UPDATE GDA_CHECKING_ACCOUNT SET ");
                                stbUpdate.Append($"INPUT_USED = {coinsUsed} ");
                                stbUpdate.Append($"WHERE ID = {id} ");

                                using (SqlCommand command2 = new SqlCommand(stbUpdate.ToString(), connection))
                                {
                                    command2.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

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

        public static List<MonetizationHierarchy> getMonetizationHierarchyNew(string dateM, string idEnv, string idIndicador)
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
        //[ResponseType(typeof(ResultsModel))]
        //public IHttpActionResult DeleteResultsModel(int id)
        //{
        //    ResultsModel resultsModel = db.ResultsModels.Find(id);
        //    if (resultsModel == null)
        //    {
        //        return NotFound();
        //    }
        //    try
        //    {
        //        db.ResultsModels.Remove(resultsModel);
        //        db.SaveChanges();
        //    }
        //    catch
        //    {

        //    }


        //    return Ok(resultsModel);
        //}


    }
}