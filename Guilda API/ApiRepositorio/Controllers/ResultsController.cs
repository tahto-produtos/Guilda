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
using System.Threading;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ResultsController : ApiController
    {

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

        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(20); // Limita a 5 chamadas simultâneas
        [ResponseType(typeof(ResultsModel))]
        public async Task<IHttpActionResult> PostResultsModel(long t_id, IEnumerable<Result> results, CancellationToken cancellationToken)
        {
            if (!await _semaphore.WaitAsync(TimeSpan.FromMinutes(60), cancellationToken))
            {
                // Aguardou 30 segundos, mas não conseguiu um slot no semáforo.
                Log.insertLogTransaction(t_id.ToString(), "RESULT", "Semaphore timeout - request dropped", "");
                return BadRequest("The server is busy. Please try again later.");
            }


            try
            {

                bool validation = false;

                Log.insertLogTransaction(t_id.ToString(), "RESULT", "START", "");

                //Valida Token
                var tkn = TokenValidate.validate(Request.Headers);
                if (tkn == false)
                {
                    Log.insertLogTransaction(t_id.ToString(), "RESULT", "Invalid Token!", "");
                    return BadRequest("Invalid Token!");
                }

                //Valida Model
                if (!ModelState.IsValid)
                {
                    Log.insertLogTransaction(t_id.ToString(), "RESULT", "Invalid Model!", "");
                    return BadRequest(ModelState);
                }

                //Valida Transaction
                int? t = TransactionValidade.validate(Request.Headers, t_id);
                if (t is null)
                {
                    Log.insertLogTransaction(t_id.ToString(), "RESULT", "Invalid Transaction!", "");
                    return BadRequest("Invalid Transaction!");
                }



                try
                {

                    JArray jsonArray = new JArray(results.Select(obj =>
                    {
                        JObject jsonObject = JObject.FromObject(obj);
                        JArray factorsArray = new JArray(obj.factors.Select(f => f.ToString()));
                        jsonObject["factors"] = string.Join(";", factorsArray);
                        if (jsonObject.ContainsKey("collaboratorIdentification"))
                        {
                            // Realiza a substituição do valor na coluna
                            var oldValue = jsonObject["collaboratorIdentification"].ToString();
                            var newValue = oldValue.Replace("BC", "");
                            jsonObject["collaboratorIdentification"] = newValue;
                        }


                        jsonObject["transactionId"] = t;
                        return jsonObject;
                    }));

                    string jsonString = jsonArray.ToString();

                    DataTable dt = (DataTable)JsonConvert.DeserializeObject(jsonString, (typeof(DataTable)));

                    string colunas = "";
                    foreach (DataColumn colunaAtual in dt.Columns)
                    {
                        var tipoColuna = colunaAtual.DataType;
                        string strTipoColunaSQL = "";

                        switch (tipoColuna.Name)
                        {
                            case "Int32":
                                {
                                    strTipoColunaSQL = "INT";
                                    break;
                                }

                            case "String":
                                {
                                    strTipoColunaSQL = "VARCHAR(MAX)";
                                    break;
                                }

                            case "DateTime":
                                {
                                    strTipoColunaSQL = "DATETIME";
                                    break;
                                }

                            case "Double":
                                {
                                    strTipoColunaSQL = "FLOAT";
                                    break;
                                }

                            default:
                                {
                                    strTipoColunaSQL = "VARCHAR(MAX)";
                                    break;
                                }
                        }

                        colunas = colunas == "" ? $" [{colunaAtual.ColumnName}] {strTipoColunaSQL} " : $"{colunas}, [{colunaAtual.ColumnName}] {strTipoColunaSQL}";
                    }
                    int linhasInseridas = 0;
                    //string connectionString = "Data Source=database-1.cpmxhjwr8mgp.us-east-1.rds.amazonaws.com;Initial Catalog=GUILDA_PROD;User ID=RDSadminsql2019;Password=H4ND4faG3AhjFe943NyPLRhMsEamXucKdDa;MultipleActiveResultSets=True;Connection Timeout=10";
                    using (SqlConnection connection = new SqlConnection(Database.Conn))
                    {
                        connection.Open();
                        try
                        {
                           

                            //string createTempTableQuery = $"CREATE TABLE #INSERTEDIDS (ID INT);";
                            //using (SqlCommand createTempTableCommand = new SqlCommand(createTempTableQuery, connection))
                            //{
                            //    createTempTableCommand.ExecuteNonQuery();
                            //}

                            //string commandText = $"CREATE TABLE #TEMPTABLE ({colunas});"; // Substitua as colunas com as adequadas do seu DataTable
                            //SqlCommand createTableCommand = new SqlCommand(commandText, connection);
                            //createTableCommand.ExecuteNonQuery();



                            // Inserir os dados do tabela de historico
                            //using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                            //{
                            //    bulkCopy.DestinationTableName = "GDA_HISTORY_RESULT";
                            //    bulkCopy.BulkCopyTimeout = 0;

                            //    // Manipular o evento SqlRowsCopied
                            //    bulkCopy.SqlRowsCopied += (sender, e) =>
                            //    {
                            //        linhasInseridas += 1;
                            //    };

                            //    // Define o número de linhas a serem notificadas
                            //    bulkCopy.NotifyAfter = 1; // Notifica a cada 1000 linhas copiadas, por exemplo

                            //    await bulkCopy.WriteToServerAsync(dt);
                            //}

                            // Inserir os dados do tabela de historico, apenas a atual para performance
                            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                            {
                                bulkCopy.DestinationTableName = "GDA_HISTORY_LAST_RESULT";
                                bulkCopy.BulkCopyTimeout = 0;

                                // Manipular o evento SqlRowsCopied
                                bulkCopy.SqlRowsCopied += (sender, e) =>
                                {
                                    linhasInseridas += 1;
                                };

                                bulkCopy.NotifyAfter = 1;

                                await bulkCopy.WriteToServerAsync(dt);
                            }

                            Log.insertLogTransaction(t_id.ToString(), "RESULT", "CONCLUDED", "", linhasInseridas);

                            validation = true;
                        }
                        catch (Exception ex)
                        {
                            Log.insertLogTransaction(t_id.ToString(), "RESULT", "ERRO: " + ex.Message.ToString(), "");
                        }
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    Log.insertLogTransaction(t_id.ToString(), "RESULT", "ERRO: " + ex.Message.ToString(), "");
                }


                if (validation == true)
                {
                    return CreatedAtRoute("DefaultApi", new { id = results.First().collaboratorIdentification }, results);
                }
                else
                {
                    return BadRequest("No information entered.");
                }
            }
            catch (Exception ex)
            {
                Log.insertLogTransaction(t_id.ToString(), "RESULT", "ERRO: " + ex.ToString(), "");
                return BadRequest(ex.ToString());
            }
            finally
            {
                _semaphore.Release(); // Libera o slot no semáforo
            }
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

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        //db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        //private bool ResultsModelExists(int id)
        //{
        //    return db.ResultsModels.Count(e => e.IDGDA_RESULT == id) > 0;
        //}
    }
}