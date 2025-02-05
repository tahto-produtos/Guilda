using ApiRepositorio.Class;
using ApiRepositorio.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class AtributesController : ApiController
    {
        private RepositorioDBContext db = new RepositorioDBContext();
        // GET: api/Atributes
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Atributes/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Atributes
        [ResponseType(typeof(AtributeModel))]
        public IHttpActionResult PostAtributeModel(long t_id, IEnumerable<AtributeModel> atributeModel)
        {

            Log.insertLogTransaction(t_id.ToString(), "ATRIBUTES", "START", "");

            bool validation = false;
            //Valida Token
            var tkn = TokenValidate.validate(Request.Headers);
            if (tkn == false)
            {
                Log.insertLogTransaction(t_id.ToString(), "ATRIBUTES", "Invalid Token!", "");
                return BadRequest("Invalid Token!");
            }

            //Valida Model
            if (!ModelState.IsValid)
            {
                Log.insertLogTransaction(t_id.ToString(), "ATRIBUTES", "Invalid Model!", "");
                return BadRequest(ModelState);
            }

            //Valida Transaction
            int? t = TransactionValidade.validate(Request.Headers, t_id);
            if (t is null)
            {
                Log.insertLogTransaction(t_id.ToString(), "ATRIBUTES", "Invalid Transaction!", "");
                return BadRequest("Invalid Transaction!");
            }



            try
            {

                JArray jsonArray = new JArray(atributeModel.Select(obj =>
                {
                    JObject jsonObject = JObject.FromObject(obj);
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


                string formattedDate = "";
                try
                {
                    string dateString = (string)jsonArray[0]["date"];
                    DateTime date = DateTime.ParseExact(dateString, "MM/dd/yyyy HH:mm:ss", null);
                    formattedDate = date.ToString("yyyy-MM-dd");
                }
                catch (Exception)
                {

                    throw;
                }




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

                SqlConnection connection = new SqlConnection(Database.Conn);
                try
                {
                    connection.Open();

                    string commandText = $"CREATE TABLE #TEMPTABLE ({colunas})"; // Substitua as colunas com as adequadas do seu DataTable
                    SqlCommand createTableCommand = new SqlCommand(commandText, connection);
                    createTableCommand.ExecuteNonQuery();

                    int tentativas = 0;
                    int maxTentativas = 3;
                    // Inserir os dados do DataTable na tabela temporária
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {

                        tentativas = 0;
                        maxTentativas = 3;
                        while (tentativas < maxTentativas)
                        {
                            try
                            {
                                bulkCopy.DestinationTableName = "#TEMPTABLE";
                                bulkCopy.BulkCopyTimeout = 0;
                                bulkCopy.WriteToServer(dt);
                                break;
                            }
                            catch (Exception)
                            {
                                tentativas++;
                            }
                        }
                    }

                    //Inserir os clientes novos
                    try
                    {
                        StringBuilder stbN = new StringBuilder();
                        stbN.Append("INSERT INTO GDA_CLIENT (CLIENT, CREATED_AT) ");
                        stbN.Append("SELECT DISTINCT(VALUE), GETDATE() FROM #TEMPTABLE (NOLOCK) AS A ");
                        stbN.Append("LEFT JOIN GDA_CLIENT (NOLOCK) AS C ON C.CLIENT = A.VALUE ");
                        stbN.Append("WHERE A.NAME = 'NOME_CLIENTE' AND C.CLIENT IS NULL ");

                        SqlCommand createTableCommandCli = new SqlCommand(stbN.ToString(), connection);
                        createTableCommandCli.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {

                    }

                    //Inserir os SITES novos
                    try
                    {
                        StringBuilder stbN = new StringBuilder();
                        stbN.Append("INSERT INTO GDA_SITE (SITE, CREATED_AT) ");
                        stbN.Append("SELECT DISTINCT(VALUE), GETDATE() FROM #TEMPTABLE (NOLOCK) AS A ");
                        stbN.Append("LEFT JOIN GDA_SITE (NOLOCK) AS C ON C.SITE = A.VALUE ");
                        stbN.Append("WHERE A.NAME = 'SITE' AND C.SITE IS NULL ");

                        SqlCommand createTableCommandCli = new SqlCommand(stbN.ToString(), connection);
                        createTableCommandCli.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {

                    }

                    tentativas = 0;
                    maxTentativas = 3;
                    while (tentativas < maxTentativas)
                    {
                        try
                        {
                            SqlCommand createTableCommandIdentity = new SqlCommand(@"SET IDENTITY_INSERT [dbo].[GDA_SECTOR] ON", connection);
                            createTableCommandIdentity.ExecuteNonQuery();
                            break;
                        }
                        catch (Exception)
                        {
                            tentativas++;
                        }
                    }




                    //DELETED_AT ATTRIBUTES
                    //StringBuilder query1 = new StringBuilder();
                    //query1.AppendFormat("UPDATE GDA_ATRIBUTES ");
                    //query1.AppendFormat("SET DELETED_AT = GETDATE() ");
                    //query1.AppendFormat("FROM GDA_ATRIBUTES AS TARGET ");
                    //query1.AppendFormat("INNER JOIN (SELECT COLLABORATORIDENTIFICATION, NAME, VALUE FROM #TEMPTABLE (NOLOCK) ");
                    //query1.AppendFormat("   WHERE NAME <> 'SETOR') AS SOURCE ON (TARGET.NAME = SOURCE.NAME ");
                    //query1.AppendFormat("   AND TARGET.IDGDA_COLLABORATORS = SOURCE.COLLABORATORIDENTIFICATION ");
                    //query1.AppendFormat("   AND TARGET.VALUE <> SOURCE.VALUE AND TARGET.DELETED_AT IS NULL); ");
                    //string commandText1 = query1.ToString();
                    //SqlCommand createTableCommand1 = new SqlCommand(commandText1, connection);
                    //createTableCommand1.ExecuteNonQuery();

                    //INSERIR DELETADOS ATTRIBUTES
                    //StringBuilder query2 = new StringBuilder();
                    //query2.AppendFormat("INSERT INTO GDA_ATRIBUTES (IDGDA_COLLABORATORS, NAME, LEVEL, VALUE, CREATED_AT) ");
                    //query2.AppendFormat("SELECT TARGET.COLLABORATORIDENTIFICATION, TARGET.NAME, TARGET.LEVEL, TARGET.VALUE, TARGET.DATE ");
                    //query2.AppendFormat("FROM #TEMPTABLE AS TARGET ");
                    //query2.AppendFormat("INNER JOIN (SELECT IDGDA_COLLABORATORS, NAME, VALUE, MAX(DELETED_AT) AS DELETED_AT FROM GDA_ATRIBUTES (NOLOCK) ");
                    //query2.AppendFormat("   WHERE NAME <> 'SETOR' GROUP BY IDGDA_COLLABORATORS, NAME, VALUE) AS SOURCE ON (TARGET.NAME = SOURCE.NAME ");
                    //query2.AppendFormat("   AND TARGET.COLLABORATORIDENTIFICATION = SOURCE.IDGDA_COLLABORATORS ");
                    //query2.AppendFormat("   AND TARGET.VALUE <> SOURCE.VALUE AND SOURCE.DELETED_AT IS NOT NULL); ");
                    //string commandText2 = query2.ToString();
                    //SqlCommand createTableCommand2 = new SqlCommand(commandText2, connection);
                    //createTableCommand2.ExecuteNonQuery();




                    //StringBuilder queryInsertResult2 = new StringBuilder();
                    //queryInsertResult2.Append("MERGE INTO GDA_ATRIBUTES AS TARGET  ");
                    //queryInsertResult2.Append("USING #TEMPTABLE AS SOURCE  ");
                    //queryInsertResult2.Append("ON (TARGET.IDGDA_COLLABORATORS = SOURCE.COLLABORATORIDENTIFICATION AND TARGET.CREATED_AT = SOURCE.DATE AND TARGET.NAME = SOURCE.NAME)  ");
                    //queryInsertResult2.Append("WHEN NOT MATCHED BY TARGET THEN  ");
                    //queryInsertResult2.Append("  INSERT ( IDGDA_COLLABORATORS, NAME, LEVEL, VALUE, CREATED_AT, DELETED_AT )");
                    //queryInsertResult2.Append("  VALUES ( SOURCE.COLLABORATORIDENTIFICATION, SOURCE.NAME, SOURCE.LEVEL, SOURCE.VALUE, SOURCE.DATE, NULL )  ");
                    //queryInsertResult2.Append("WHEN MATCHED THEN  ");
                    //queryInsertResult2.Append("  UPDATE SET  ");
                    //queryInsertResult2.Append("  TARGET.LEVEL = SOURCE.LEVEL, ");
                    //queryInsertResult2.Append("  TARGET.VALUE = SOURCE.VALUE; ");

                    StringBuilder queryR = new StringBuilder();
                    queryR.Append("INSERT INTO GDA_REPROCESS_TABLE_PHOTO (DATA, REPROCESS) ");
                    queryR.AppendFormat("SELECT '{0}', 0 ", formattedDate);
                    queryR.Append("WHERE NOT EXISTS ( ");
                    queryR.Append("    SELECT 1 ");
                    queryR.Append("    FROM GDA_REPROCESS_TABLE_PHOTO (NOLOCK) ");
                    queryR.AppendFormat("    WHERE DATA = '{0}' AND REPROCESS = 0 ", formattedDate);
                    queryR.Append("); ");


                    tentativas = 0;
                    maxTentativas = 3;
                    while (tentativas < maxTentativas)
                    {
                        try
                        {
                            SqlCommand createTableCommandR = new SqlCommand(queryR.ToString(), connection);
                            createTableCommandR.CommandTimeout = 0;
                            createTableCommandR.ExecuteNonQuery();
                            break;
                        }
                        catch (Exception)
                        {
                            tentativas++;
                        }
                    }



                    //query3.AppendFormat("MERGE INTO GDA_REPROCESS_TABLE_PHOTO AS TARGET ");
                    //query3.AppendFormat("USING #TEMPTABLE AS SOURCE ");
                    //query3.AppendFormat("ON (TARGET.NAME = SOURCE.NAME AND SOURCE.COLLABORATORIDENTIFICATION = TARGET.IDGDA_COLLABORATORS ");
                    //query3.AppendFormat("AND TARGET.CREATED_AT = SOURCE.DATE ) ");
                    //query3.AppendFormat("   WHEN NOT MATCHED BY TARGET AND SOURCE.NAME <> 'SETOR' THEN ");
                    //query3.AppendFormat("INSERT (IDGDA_COLLABORATORS, NAME, LEVEL, VALUE, CREATED_AT) ");
                    //query3.AppendFormat("VALUES (COLLABORATORIDENTIFICATION, NAME, LEVEL, VALUE, [DATE]); ");





                    //INSERT SELECT GDA_ATTRIBUTES
                    StringBuilder query3 = new StringBuilder();
                    query3.AppendFormat("INSERT INTO GDA_ATRIBUTES (IDGDA_COLLABORATORS, NAME, LEVEL, VALUE, CREATED_AT, DELETED_AT, TRANSACTIONID) ");
                    query3.AppendFormat("SELECT COLLABORATORIDENTIFICATION, NAME, LEVEL, VALUE, DATE, NULL, TRANSACTIONID FROM #TEMPTABLE (NOLOCK) ");

                    tentativas = 0;
                    maxTentativas = 3;
                    while (tentativas < maxTentativas)
                    {
                        try
                        {
                            SqlCommand createTableCommand3 = new SqlCommand(query3.ToString(), connection);
                            createTableCommand3.CommandTimeout = 0;
                            createTableCommand3.ExecuteNonQuery();
                            break;
                        }
                        catch (Exception)
                        {
                            tentativas++;
                        }
                    }




                    //INSERIR NOVOS ATTRIBUTES
                    //StringBuilder query3 = new StringBuilder();
                    //query3.AppendFormat("MERGE INTO GDA_ATRIBUTES AS TARGET ");
                    //query3.AppendFormat("USING #TEMPTABLE AS SOURCE ");
                    //query3.AppendFormat("ON (TARGET.NAME = SOURCE.NAME AND SOURCE.COLLABORATORIDENTIFICATION = TARGET.IDGDA_COLLABORATORS ");
                    //query3.AppendFormat("AND TARGET.CREATED_AT = SOURCE.DATE ) ");
                    //query3.AppendFormat("   WHEN NOT MATCHED BY TARGET AND SOURCE.NAME <> 'SETOR' THEN ");
                    //query3.AppendFormat("INSERT (IDGDA_COLLABORATORS, NAME, LEVEL, VALUE, CREATED_AT) ");
                    //query3.AppendFormat("VALUES (COLLABORATORIDENTIFICATION, NAME, LEVEL, VALUE, [DATE]); ");
                    //string commandText3 = query3.ToString();
                    //SqlCommand createTableCommand3 = new SqlCommand(commandText3, connection);
                    //createTableCommand3.CommandTimeout = 0;
                    //createTableCommand3.ExecuteNonQuery();

                    //UPDATE SETOR
                    StringBuilder query4 = new StringBuilder();
                    query4.AppendFormat("UPDATE GDA_SECTOR ");
                    query4.AppendFormat("SET GDA_SECTOR.NAME = SOURCE.SETOR, GDA_SECTOR.SECTOR = 1, GDA_SECTOR.SUBSECTOR = 0, GDA_SECTOR.DELETED_AT = NULL ");
                    query4.AppendFormat("FROM GDA_SECTOR AS TARGET ");
                    query4.AppendFormat("INNER JOIN ( ");
                    query4.AppendFormat("   SELECT COLLABORATORIDENTIFICATION, MAX(CASE WHEN NAME = 'CD_GIP' THEN VALUE END) AS CD_GIP, ");
                    query4.AppendFormat("   MAX(CASE WHEN NAME = 'SETOR' THEN VALUE END) AS SETOR  ");
                    query4.AppendFormat("FROM ");
                    query4.AppendFormat("   #TEMPTABLE (NOLOCK) ");
                    query4.AppendFormat("GROUP BY ");
                    query4.AppendFormat("   COLLABORATORIDENTIFICATION ) AS SOURCE ON (SOURCE.CD_GIP = TARGET.IDGDA_SECTOR); ");
                    string commandText4 = query4.ToString();

                    tentativas = 0;
                    maxTentativas = 3;
                    while (tentativas < maxTentativas)
                    {
                        try
                        {
                            SqlCommand createTableCommand4 = new SqlCommand(commandText4, connection);
                            createTableCommand4.CommandTimeout = 0;
                            createTableCommand4.ExecuteNonQuery();
                            break;
                        }
                        catch (Exception)
                        {
                            tentativas++;
                        }
                    }



                    //INSERIR NOVOS SETORES
                    StringBuilder query5 = new StringBuilder();
                    query5.AppendFormat("INSERT INTO GDA_SECTOR (IDGDA_SECTOR, NAME, LEVEL, CREATED_AT, SECTOR, SUBSECTOR) ");
                    query5.AppendFormat("   SELECT TBL.CD_GIP, TBL.SETOR, MAX(TBL.LEVEL), GETDATE(), 1, 0 FROM ");
                    query5.AppendFormat("       (SELECT MAX(CASE WHEN NAME = 'CD_GIP' THEN VALUE END) AS CD_GIP, ");
                    query5.AppendFormat("       MAX(CASE WHEN NAME = 'SETOR' THEN VALUE END) AS SETOR, ");
                    query5.AppendFormat("       MAX(LEVEL) AS LEVEL ");
                    query5.AppendFormat("   FROM #TEMPTABLE (NOLOCK) ");
                    query5.AppendFormat("   GROUP BY COLLABORATORIDENTIFICATION) AS TBL ");
                    query5.AppendFormat("WHERE TBL.CD_GIP NOT IN (SELECT IDGDA_SECTOR FROM GDA_SECTOR (NOLOCK)) ");
                    query5.AppendFormat("GROUP BY TBL.SETOR, TBL.CD_GIP; ");
                    string commandText5 = query5.ToString();

                    tentativas = 0;
                    maxTentativas = 3;
                    while (tentativas < maxTentativas)
                    {
                        try
                        {
                            SqlCommand createTableCommand5 = new SqlCommand(commandText5, connection);
                            createTableCommand5.CommandTimeout = 0;
                            createTableCommand5.ExecuteNonQuery();
                            break;
                        }
                        catch (Exception)
                        {
                            tentativas++;
                        }
                    }



                    //DESCOMENTAR SUBSETOR
                    //UPDATE SUB_SETOR
                    StringBuilder query10 = new StringBuilder();
                    query10.AppendFormat("UPDATE GDA_SECTOR ");
                    query10.AppendFormat("SET GDA_SECTOR.NAME = SOURCE.SUB_SETOR, GDA_SECTOR.SUBSECTOR = 1,  GDA_SECTOR.SECTOR = 0, GDA_SECTOR.DELETED_AT = NULL ");
                    query10.AppendFormat("FROM GDA_SECTOR AS TARGET ");
                    query10.AppendFormat("INNER JOIN ( ");
                    query10.AppendFormat("   SELECT COLLABORATORIDENTIFICATION, MAX(CASE WHEN NAME = 'COD_SUBSETOR' THEN VALUE END) AS COD_SUBSETOR, ");
                    query10.AppendFormat("   MAX(CASE WHEN NAME = 'SUB_SETOR' THEN VALUE END) AS SUB_SETOR  ");
                    query10.AppendFormat("FROM ");
                    query10.AppendFormat("   #TEMPTABLE (NOLOCK) ");
                    query10.AppendFormat("GROUP BY ");
                    query10.AppendFormat("   COLLABORATORIDENTIFICATION ) AS SOURCE ON (SOURCE.COD_SUBSETOR = TARGET.IDGDA_SECTOR); ");
                    string commandText10 = query10.ToString();

                    tentativas = 0;
                    maxTentativas = 3;
                    while (tentativas < maxTentativas)
                    {
                        try
                        {
                            SqlCommand createTableCommand10 = new SqlCommand(commandText10, connection);
                            createTableCommand10.CommandTimeout = 0;
                            createTableCommand10.ExecuteNonQuery();
                            break;
                        }
                        catch (Exception)
                        {
                            tentativas++;
                        }
                    }



                    //DESCOMENTAR SUBSETOR
                    //INSERIR SUB_SETORES
                    StringBuilder query11 = new StringBuilder();
                    query11.AppendFormat("INSERT INTO GDA_SECTOR (IDGDA_SECTOR, NAME, LEVEL, CREATED_AT, SECTOR, SUBSECTOR) ");
                    query11.AppendFormat("   SELECT TBL.COD_SUBSETOR, TBL.SUB_SETOR, MAX(TBL.LEVEL), GETDATE(), 0, 1 FROM ");
                    query11.AppendFormat("       (SELECT MAX(CASE WHEN NAME = 'COD_SUBSETOR' THEN VALUE END) AS COD_SUBSETOR, ");
                    query11.AppendFormat("       MAX(CASE WHEN NAME = 'SUB_SETOR' THEN VALUE END) AS SUB_SETOR, ");
                    query11.AppendFormat("       MAX(LEVEL) AS LEVEL ");
                    query11.AppendFormat("   FROM #TEMPTABLE (NOLOCK) ");
                    query11.AppendFormat("   GROUP BY COLLABORATORIDENTIFICATION) AS TBL ");
                    query11.AppendFormat("WHERE TBL.COD_SUBSETOR NOT IN (SELECT IDGDA_SECTOR FROM GDA_SECTOR (NOLOCK)) ");
                    query11.AppendFormat("GROUP BY TBL.SUB_SETOR, TBL.COD_SUBSETOR; ");
                    string commandText11 = query11.ToString();

                    tentativas = 0;
                    maxTentativas = 3;
                    while (tentativas < maxTentativas)
                    {
                        try
                        {
                            SqlCommand createTableCommand11 = new SqlCommand(commandText11, connection);
                            createTableCommand11.CommandTimeout = 0;
                            createTableCommand11.ExecuteNonQuery();
                            break;
                        }
                        catch (Exception)
                        {
                            tentativas++;
                        }
                    }




                    //DELETED_AT HISTORICO COLLABORATOR x SETOR
                    //StringBuilder query6 = new StringBuilder();
                    //query6.AppendFormat("UPDATE GDA_HISTORY_COLLABORATOR_SECTOR ");
                    //query6.AppendFormat("SET DELETED_AT = GETDATE() ");
                    //query6.AppendFormat("FROM GDA_HISTORY_COLLABORATOR_SECTOR AS TARGET ");
                    //query6.AppendFormat("INNER JOIN ( ");
                    //query6.AppendFormat("   SELECT COLLABORATORIDENTIFICATION, ");
                    //query6.AppendFormat("       MAX(CASE WHEN NAME = 'CD_GIP' THEN VALUE END) AS CD_GIP, ");
                    //query6.AppendFormat("       MAX(LEVEL) AS LEVEL ");
                    //query6.AppendFormat("   FROM #TEMPTABLE (NOLOCK) ");
                    //query6.AppendFormat("   GROUP BY COLLABORATORIDENTIFICATION ");
                    //query6.AppendFormat(") AS SOURCE ON (TARGET.IDGDA_COLLABORATORS = SOURCE.COLLABORATORIDENTIFICATION ");
                    //query6.AppendFormat("AND TARGET.IDGDA_SECTOR <> SOURCE.CD_GIP ");
                    //query6.AppendFormat("AND DELETED_AT IS NULL); ");
                    //string commandText6 = query6.ToString();
                    //SqlCommand createTableCommand6 = new SqlCommand(commandText6, connection);
                    //createTableCommand6.ExecuteNonQuery();

                    //INSERIR DELETADOS HISTORICO COLLABORATOR x SETOR
                    //StringBuilder query7 = new StringBuilder();
                    //query7.AppendFormat("INSERT INTO GDA_HISTORY_COLLABORATOR_SECTOR (CREATED_AT, IDGDA_COLLABORATORS, IDGDA_SECTOR, TRANSACTIONID) ");
                    //query7.AppendFormat("SELECT SOURCE.DATE, SOURCE.COLLABORATORIDENTIFICATION, SOURCE.CD_GIP, {0} ", t.idgda_Transaction);
                    //query7.AppendFormat("FROM GDA_HISTORY_COLLABORATOR_SECTOR AS TARGET ");
                    //query7.AppendFormat("INNER JOIN ( ");
                    //query7.AppendFormat("   SELECT	COLLABORATORIDENTIFICATION, ");
                    //query7.AppendFormat("       MAX(CASE WHEN NAME = 'CD_GIP' THEN VALUE END) AS CD_GIP, ");
                    //query7.AppendFormat("       MAX(LEVEL) AS LEVEL, ");
                    //query7.AppendFormat("       MAX(DATE) AS DATE ");
                    //query7.AppendFormat("   FROM #TEMPTABLE (NOLOCK) ");
                    //query7.AppendFormat("   GROUP BY COLLABORATORIDENTIFICATION ");
                    //query7.AppendFormat(") AS SOURCE ON (TARGET.IDGDA_COLLABORATORS = SOURCE.COLLABORATORIDENTIFICATION ");
                    //query7.AppendFormat("AND TARGET.IDGDA_SECTOR <> SOURCE.CD_GIP ");
                    //query7.AppendFormat("AND DELETED_AT IS NOT NULL); ");
                    //string commandText7 = query7.ToString();
                    //SqlCommand createTableCommand7 = new SqlCommand(commandText7, connection);
                    //createTableCommand7.ExecuteNonQuery();

                    //INSERIR NOVOS HISTORICO COLLABORATOR x SETOR
                    StringBuilder query8 = new StringBuilder();
                    query8.AppendFormat("MERGE INTO GDA_HISTORY_COLLABORATOR_SECTOR AS TARGET ");
                    query8.AppendFormat("USING ( ");
                    query8.AppendFormat("   SELECT TPL.COLLABORATORIDENTIFICATION, ");
                    query8.AppendFormat("   MAX(CASE WHEN TPL.NAME = 'CD_GIP' THEN TPL.VALUE END) AS CD_GIP, ");
                    query8.AppendFormat("   MAX(LEVEL) AS LEVEL, MAX(DATE) AS DATEENV ");
                    query8.AppendFormat("   FROM #TEMPTABLE (NOLOCK) AS TPL ");
                    query8.AppendFormat("   INNER JOIN GDA_COLLABORATORS (NOLOCK) ON TPL.COLLABORATORIDENTIFICATION = GDA_COLLABORATORS.IDGDA_COLLABORATORS ");
                    query8.AppendFormat("   GROUP BY TPL.COLLABORATORIDENTIFICATION ");
                    query8.AppendFormat(") AS SOURCE ON (TARGET.IDGDA_COLLABORATORS = SOURCE.COLLABORATORIDENTIFICATION ");
                    query8.AppendFormat("AND TARGET.IDGDA_SECTOR = SOURCE.CD_GIP AND TARGET.CREATED_AT = SOURCE.DATEENV ) ");
                    query8.AppendFormat("WHEN NOT MATCHED BY TARGET THEN ");
                    query8.AppendFormat("INSERT (CREATED_AT, IDGDA_COLLABORATORS, IDGDA_SECTOR, TRANSACTIONID) ");
                    query8.AppendFormat("VALUES (SOURCE.DATEENV, SOURCE.COLLABORATORIDENTIFICATION, SOURCE.CD_GIP, {0} ); ", t);
                    string commandText8 = query8.ToString();

                    tentativas = 0;
                    maxTentativas = 3;
                    while (tentativas < maxTentativas)
                    {
                        try
                        {
                            SqlCommand createTableCommand8 = new SqlCommand(commandText8, connection);
                            createTableCommand8.CommandTimeout = 0;
                            createTableCommand8.ExecuteNonQuery();
                            break;
                        }
                        catch (Exception)
                        {
                            tentativas++;
                        }
                    }



                    tentativas = 0;
                    maxTentativas = 3;
                    while (tentativas < maxTentativas)
                    {
                        try
                        {
                            SqlCommand createTableCommandIdentityOFF = new SqlCommand(@"SET IDENTITY_INSERT [dbo].[GDA_SECTOR] OFF", connection);
                            createTableCommandIdentityOFF.ExecuteNonQuery();
                            break;
                        }
                        catch (Exception)
                        {
                            tentativas++;
                        }
                    }




                    //DESCOMENTAR SUBSETOR
                    //INSERIR NOVOS HISTORICO COLLABORATOR x SUB_SETOR
                    StringBuilder query13 = new StringBuilder();
                    query13.AppendFormat("MERGE INTO GDA_HISTORY_COLLABORATOR_SUBSECTOR AS TARGET ");
                    query13.AppendFormat("USING ( ");
                    query13.AppendFormat("   SELECT TPL.COLLABORATORIDENTIFICATION, ");
                    query13.AppendFormat("   MAX(CASE WHEN TPL.NAME = 'COD_SUBSETOR' THEN TPL.VALUE END) AS COD_SUBSETOR, ");
                    query13.AppendFormat("   MAX(LEVEL) AS LEVEL, MAX(DATE) AS DATEENV ");
                    query13.AppendFormat("   FROM #TEMPTABLE (NOLOCK) AS TPL ");
                    query13.AppendFormat("   INNER JOIN GDA_COLLABORATORS (NOLOCK) ON TPL.COLLABORATORIDENTIFICATION = GDA_COLLABORATORS.IDGDA_COLLABORATORS ");
                    query13.AppendFormat("   GROUP BY TPL.COLLABORATORIDENTIFICATION ");
                    query13.AppendFormat("   HAVING MAX(CASE WHEN TPL.NAME = 'COD_SUBSETOR' THEN TPL.VALUE END) <> 0 ");
                    query13.AppendFormat(") AS SOURCE ON (TARGET.IDGDA_COLLABORATORS = SOURCE.COLLABORATORIDENTIFICATION ");
                    query13.AppendFormat("AND TARGET.IDGDA_SECTOR = SOURCE.COD_SUBSETOR AND TARGET.CREATED_AT = SOURCE.DATEENV ) ");
                    query13.AppendFormat("WHEN NOT MATCHED BY TARGET THEN ");
                    query13.AppendFormat("INSERT (CREATED_AT, IDGDA_COLLABORATORS, IDGDA_SECTOR, TRANSACTIONID) ");
                    query13.AppendFormat("VALUES (SOURCE.DATEENV, SOURCE.COLLABORATORIDENTIFICATION, SOURCE.COD_SUBSETOR, {0} ); ", t);
                    string commandText13 = query13.ToString();

                    tentativas = 0;
                    maxTentativas = 3;
                    while (tentativas < maxTentativas)
                    {
                        try
                        {
                            SqlCommand createTableCommand13 = new SqlCommand(commandText13, connection);
                            createTableCommand13.CommandTimeout = 0;
                            createTableCommand13.ExecuteNonQuery();
                            break;
                        }
                        catch (Exception)
                        {
                            tentativas++;
                        }
                    }


                    //try
                    //{
                    //    SqlCommand createTableCommandIdentityOFF = new SqlCommand(@"SET IDENTITY_INSERT [dbo].[GDA_SECTOR] OFF", connection);
                    //    createTableCommandIdentityOFF.ExecuteNonQuery();
                    //}
                    //catch (Exception)
                    //{


                    //}

                    // Remove a tabela temporária
                    string dropTempTableQuery2 = $"DROP TABLE #TEMPTABLE";
                    using (SqlCommand dropTempTableCommand2 = new SqlCommand(dropTempTableQuery2, connection))
                    {

                        tentativas = 0;
                        maxTentativas = 3;
                        while (tentativas < maxTentativas)
                        {
                            try
                            {
                                dropTempTableCommand2.ExecuteNonQuery();
                                break;
                            }
                            catch (Exception)
                            {
                                tentativas++;
                            }
                        }


                    }

                    Log.insertLogTransaction(t_id.ToString(), "ATRIBUTES", "CONCLUDED", formattedDate);

                    validation = true;
                }
                catch (Exception ex)
                {
                    Log.insertLogTransaction(t_id.ToString(), "ATRIBUTES", "ERRO: " + ex.Message.ToString(), "");
                }
                connection.Close();

            }
            catch (Exception ex)
            {
                Log.insertLogTransaction(t_id.ToString(), "ATRIBUTES", "ERRO: " + ex.Message.ToString(), "");
            }

            if (validation == true)
            {
                return CreatedAtRoute("DefaultApi", new { id = atributeModel.First().collaboratorIdentification }, atributeModel);
            }
            else
            {
                return BadRequest("No information entered.");
            }
        }



        // POST: api/Atributes

        //[ResponseType(typeof(AtributeModel))]
        //public IHttpActionResult PostAtributeModel(long t_id, IEnumerable<AtributeModel> atributeModel)
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


        //    foreach (AtributeModel am in atributeModel)
        //    {

        //        string teste = am.collaboratorIdentification;

        //        //Valida se o colaborador ja existe
        //        //CollaboratorTableModel cl = db.CollaboratorTableModels.Where(p => p.collaboratorIdentification == am.collaboratorIdentification).FirstOrDefault();
        //        //if (cl is null)
        //        //{
        //        //    continue;
        //        //}

        //        //Caso seja um setor, salva na tabela de setores, caso seja outro atributo, salva na tabela de atributos.
        //        if (am.name == "SETOR")
        //        {
        //            //Valida se o setor ja existe
        //            if (db.SectorModels.Where(p => p.NAME == am.value).Count() == 0)
        //            {
        //                //Adiciona Setor
        //                SectorModel sm = new SectorModel();
        //                sm.NAME = am.name;
        //                sm.LEVEL = am.level;
        //                sm.NAME = am.value;
        //                sm.CREATED_AT = DateTime.Now;
        //                db.SectorModels.Add(sm);

        //                try
        //                {
        //                    db.SaveChanges();
        //                    validation = true;
        //                }
        //                catch
        //                {

        //                }

        //            }

        //            var st = db.SectorModels.Where(p => p.NAME == am.value).FirstOrDefault();

        //            HistoryCollaboratorSectorModel hsm = new HistoryCollaboratorSectorModel();

        //            var idEx = am.collaboratorIdentification.Replace("BC", "");

        //            hsm.IDGDA_COLLABORATORS = Convert.ToInt32(idEx);
        //            hsm.IDGDA_SECTOR = st.IDGDA_SECTOR;
        //            hsm.CREATED_AT = DateTime.Now;
        //            hsm.TRANSACTIONID = t.idgda_Transaction;

        //            db.HistoryCollaboratorSectorModels.Add(hsm);

        //            try
        //            {
        //                db.SaveChanges();
        //                validation = true;
        //            }
        //            catch { }

        //        }
        //        else
        //        {
        //            AtributeTableModel atm = new AtributeTableModel();

        //            var idEx = am.collaboratorIdentification.Replace("BC", "");

        //            atm.IDGDA_COLLABORATORS = Convert.ToInt32(idEx);
        //            atm.NAME = am.name;
        //            atm.LEVEL = am.level;
        //            atm.VALUE = am.value;
        //            atm.CREATED_AT = DateTime.Now;
        //            //GDA_ATRIBUTES
        //            db.AtributeTableModels.Add(atm);

        //            try
        //            {
        //                db.SaveChanges();
        //                validation = true;
        //            }
        //            catch { }
        //        }
        //    }

        //    if (validation == true)
        //    {
        //        return CreatedAtRoute("DefaultApi", new { id = atributeModel.First().collaboratorIdentification }, atributeModel);
        //    }
        //    else
        //    {
        //        return BadRequest("No information entered.");
        //    }
        //}
    }
}
