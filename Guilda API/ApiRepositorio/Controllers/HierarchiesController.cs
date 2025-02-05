using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using ApiRepositorio.Class;
using ApiRepositorio.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class HierarchiesController : ApiController
    {
        private RepositorioDBContext db = new RepositorioDBContext();

        // GET: api/Hierarchies
        //public IQueryable<HierarchiesModel> GetHierarchiesModels()
        //{
        //    return db.HierarchiesModels;
        //}

        //// GET: api/Hierarchies/5
        //[ResponseType(typeof(HierarchiesModel))]
        //public IHttpActionResult GetHierarchiesModel(int id)
        //{
        //    HierarchiesModel hierarchiesModel = db.HierarchiesModels.Find(id);
        //    if (hierarchiesModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(hierarchiesModel);
        //}

        //// PUT: api/Hierarchies/5
        //[ResponseType(typeof(void))]
        //public IHttpActionResult PutHierarchiesModel(int id, HierarchiesModel hierarchiesModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != hierarchiesModel.IDGDA_HISTORY_HIERARCHY_RELATIONSHIP)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(hierarchiesModel).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!HierarchiesModelExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // POST: api/Hierarchies
        [ResponseType(typeof(HierarchiesModel))]
        public IHttpActionResult PostHierarchiesModel(long t_id, IEnumerable<HierarchiesModel> hierarchiesModel)
        {
            Log.insertLogTransaction(t_id.ToString(), "HIERARCHY", "START", "");

            bool validation = false;
            //Valida Token
            var tkn = TokenValidate.validate(Request.Headers);
            if (tkn == false)
            {
                Log.insertLogTransaction(t_id.ToString(), "HIERARCHY", "Invalid Token!", "");
                return BadRequest("Invalid Token!");
            }

            if (!ModelState.IsValid)
            {
                Log.insertLogTransaction(t_id.ToString(), "HIERARCHY", "Invalid Model!", "");
                return BadRequest(ModelState);
            }


            //Valida Transaction
            int? t = TransactionValidade.validate(Request.Headers, t_id);
            if (t is null)
            {
                Log.insertLogTransaction(t_id.ToString(), "HIERARCHY", "Invalid Transaction!", "");
                return BadRequest("Invalid Transaction!");
            }


            //Modelo antigo, mantive apenas para inserir hierarquias novas se precisar
            //foreach (HierarchiesModel hm in hierarchiesModel)
            //{

            //Valida se o colaborador ja existe
            //CollaboratorTableModel cl = db.CollaboratorTableModels.Where(p => p.collaboratorIdentification == hm.collaboratorIdentification).FirstOrDefault();
            //CollaboratorTableModel pi = db.CollaboratorTableModels.Where(p => p.collaboratorIdentification == hm.parentIdentification).FirstOrDefault();
            //CollaboratorTableModel cl = db.CollaboratorTableModels.Where(p => p.contractorControlId == hm.COLABORADOR_IDENTIFICATION).FirstOrDefault();
            //if (cl is null)
            //{
            //    continue;
            //}
            //if (pi is null)
            //{
            //    continue;
            //}

            //HierarchiesTableModel ht = db.HierarchiesTableModels.Where(p => p.LEVELNAME == hm.levelName).FirstOrDefault();
            //if (ht is null)
            //{
            //    HierarchiesTableModel htm = new HierarchiesTableModel();
            //    htm.LEVELNAME = hm.levelName;
            //    htm.LEVELWEIGHT = hm.levelWeight;
            //    htm.CREATED_AT = DateTime.Now;

            //    db.HierarchiesTableModels.Add(htm);

            //    try
            //    {
            //        db.SaveChanges();
            //        validation = true;
            //    }
            //    catch { }
            //}

            //ht = db.HierarchiesTableModels.Where(p => p.LEVELNAME == hm.levelName).FirstOrDefault();

            //HistoryHierarchiesModel hhm = new HistoryHierarchiesModel();

            //var idEx = hm.collaboratorIdentification.Replace("BC", "");
            //var idParentEx = hm.parentIdentification.Replace("BC", "");

            //hhm.IDGDA_HIERARCHY = ht.IDGDA_HIERARCHY;
            //hhm.IDGDA_COLLABORATORS = Convert.ToInt32(idEx);
            //hhm.CONTRACTORCONTROLID = hm.contractorControlId;
            //hhm.PARENTIDENTIFICATION = Convert.ToInt32(idParentEx);
            //hhm.DATE = hm.date;
            //hhm.LEVELNAME = hm.levelName;
            //hhm.LEVELWEIGHT = hm.levelWeight;
            //hhm.CREATED_AT = DateTime.Now;
            //hhm.TRANSACTIONID = t.idgda_Transaction;
            //db.HistoryHierarchiesModels.Add(hhm);
            //try
            //{
            //    db.SaveChanges();
            //    validation = true;
            //}
            //catch
            //{ }
            //}


            //Insert Ativos (Modelo novo bulk)
            try
            {

                JArray jsonArray = new JArray(hierarchiesModel.Select(obj =>
                {
                    JObject jsonObject = JObject.FromObject(obj);
                    if (jsonObject.ContainsKey("collaboratorIdentification"))
                    {
                        // Realiza a substituição do valor na coluna
                        var oldValue = jsonObject["collaboratorIdentification"].ToString();
                        var newValue = oldValue.Replace("BC", "");
                        jsonObject["collaboratorIdentification"] = newValue;
                    }

                    if (jsonObject.ContainsKey("parentIdentification"))
                    {
                        // Realiza a substituição do valor na coluna
                        var oldValue = jsonObject["parentIdentification"].ToString();
                        var newValue = oldValue.Replace("BC", "");
                        jsonObject["parentIdentification"] = newValue;
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

                //string connectionString = "Data Source=database-1.cpmxhjwr8mgp.us-east-1.rds.amazonaws.com;Initial Catalog=GUILDA_PROD;User ID=RDSadminsql2019;Password=H4ND4faG3AhjFe943NyPLRhMsEamXucKdDa;MultipleActiveResultSets=True;Connection Timeout=10";
                SqlConnection connection = new SqlConnection(Database.Conn);
                try
                {
                    connection.Open();

                    string commandText = $"CREATE TABLE #TEMPTABLE ({colunas});"; // Substitua as colunas com as adequadas do seu DataTable
                    SqlCommand createTableCommand = new SqlCommand(commandText, connection);
                    createTableCommand.ExecuteNonQuery();

                    int tentativas = 0;
                    int maxTentativas = 3;

                    // Inserir os dados do DataTable na tabela temporária
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        //tratataiva DeadLock

                        while (tentativas < maxTentativas)
                        {
                            try
                            {
                                bulkCopy.DestinationTableName = "#TEMPTABLE";
                                bulkCopy.WriteToServer(dt);
                                break;
                            }
                            catch (Exception)
                            {
                                tentativas++;
                            }
                        }
                    }

                    // Inserir os dados do DataTable na tabela temporária
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {

                        //tratataiva DeadLock
                        tentativas = 0;
                        maxTentativas = 3;
                        while (tentativas < maxTentativas)
                        {
                            try
                            {
                                bulkCopy.DestinationTableName = "GDA_LOG_HISTORY_HIERARCHY";
                                bulkCopy.WriteToServer(dt);
                                break;
                            }
                            catch (Exception)
                            {
                                tentativas++;
                            }
                        }
                    }




                    StringBuilder queryInsertResult2 = new StringBuilder();
                    queryInsertResult2.Append("MERGE INTO GDA_HISTORY_HIERARCHY_RELATIONSHIP AS TARGET  ");
                    queryInsertResult2.Append("USING #TEMPTABLE AS SOURCE  ");
                    queryInsertResult2.Append("INNER JOIN GDA_HIERARCHY (NOLOCK) H ON SOURCE.LEVELNAME = H.LEVELNAME ");
                    queryInsertResult2.Append("ON (TARGET.IDGDA_COLLABORATORS = SOURCE.COLLABORATORIDENTIFICATION AND TARGET.DATE = SOURCE.DATE)  ");
                    queryInsertResult2.Append("WHEN NOT MATCHED BY TARGET THEN  ");
                    queryInsertResult2.Append("  INSERT ( CONTRACTORCONTROLID, PARENTIDENTIFICATION, IDGDA_COLLABORATORS, IDGDA_HIERARCHY, CREATED_AT, TRANSACTIONID, LEVELWEIGHT, DATE, LEVELNAME )");
                    queryInsertResult2.Append("  VALUES ( SOURCE.CONTRACTORCONTROLID, SOURCE.PARENTIDENTIFICATION, SOURCE.COLLABORATORIDENTIFICATION, H.IDGDA_HIERARCHY, GETDATE(), SOURCE.TRANSACTIONID, SOURCE.LEVELWEIGHT, SOURCE.DATE, SOURCE.LEVELNAME )  ");
                    queryInsertResult2.Append("WHEN MATCHED THEN  ");
                    queryInsertResult2.Append("  UPDATE SET  ");
                    queryInsertResult2.Append("  TARGET.CONTRACTORCONTROLID = SOURCE.CONTRACTORCONTROLID, ");
                    queryInsertResult2.Append("  TARGET.PARENTIDENTIFICATION = SOURCE.PARENTIDENTIFICATION, ");
                    queryInsertResult2.Append("  TARGET.IDGDA_HIERARCHY = H.IDGDA_HIERARCHY, ");
                    queryInsertResult2.Append("  TARGET.CREATED_AT = GETDATE(), ");
                    queryInsertResult2.Append("  TARGET.TRANSACTIONID = SOURCE.TRANSACTIONID, ");
                    queryInsertResult2.Append("  TARGET.LEVELWEIGHT = SOURCE.LEVELWEIGHT, ");
                    queryInsertResult2.Append("  TARGET.LEVELNAME = SOURCE.LEVELNAME; ");

                    //StringBuilder queryInsertResult2 = new StringBuilder();
                    //queryInsertResult2.Append("INSERT INTO GDA_HISTORY_HIERARCHY_RELATIONSHIP (CONTRACTORCONTROLID, PARENTIDENTIFICATION, IDGDA_COLLABORATORS, IDGDA_HIERARCHY, CREATED_AT, TRANSACTIONID, LEVELWEIGHT, DATE, LEVELNAME) ");
                    //queryInsertResult2.Append("SELECT CONTRACTORCONTROLID, PARENTIDENTIFICATION, COLLABORATORIDENTIFICATION, IDGDA_HIERARCHY, GETDATE(), TRANSACTIONID, T.LEVELWEIGHT, DATE, T.LEVELNAME  ");
                    //queryInsertResult2.Append("FROM #TEMPTABLE (NOLOCK) T ");
                    //queryInsertResult2.Append("INNER JOIN GDA_HIERARCHY (NOLOCK) H ON T.LEVELNAME = H.LEVELNAME ");

                    //tratataiva DeadLock
                    tentativas = 0;
                    maxTentativas = 3;
                    while (tentativas < maxTentativas)
                    {
                        try
                        {
                            SqlCommand createTableCommand2 = new SqlCommand(queryInsertResult2.ToString(), connection);
                            createTableCommand2.CommandTimeout = 0;
                            createTableCommand2.ExecuteNonQuery();
                            break;
                        }
                        catch (Exception)
                        {
                            tentativas++;
                        }
                    }

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

                    Log.insertLogTransaction(t_id.ToString(), "HIERARCHY", "CONCLUDED", formattedDate);

                    validation = true;
                }
                catch (Exception ex)
                {
                    Log.insertLogTransaction(t_id.ToString(), "HIERARCHY", "ERRO: " + ex.Message.ToString(), "");
                }

                connection.Close();
                connection.Open();
                try
                {
                    //Processo Hierarquia base

                    StringBuilder queryProfile = new StringBuilder();
                    //queryProfile.Append("UPDATE A ");
                    //queryProfile.Append("SET PROFILE_COLLABORATOR_ADMINISTRATIONID = ( ");
                    //queryProfile.Append("    SELECT ID ");
                    //queryProfile.Append("    FROM GDA_PROFILE_COLLABORATOR_ADMINISTRATION AS PCA  ");
                    //queryProfile.Append("    INNER JOIN #TEMPTABLE AS H ON H.levelWeight = PCA.BASIC_PROFILE_IDHIERARCHY ");
                    //queryProfile.Append("    WHERE H.contractorControlId = A.IDGDA_COLLABORATORS ");
                    //queryProfile.Append("), NEW_AGENT = 0 FROM GDA_COLLABORATORS A WHERE A.NEW_AGENT = 1 ");
                    queryProfile.Append("UPDATE GDA_COLLABORATORS ");
                    queryProfile.Append("SET PROFILE_COLLABORATOR_ADMINISTRATIONID =  ");
                    queryProfile.Append("( ");
                    queryProfile.Append("    SELECT MAX(PCA.ID) ");
                    queryProfile.Append("    FROM GDA_COLLABORATORS_DETAILS AS CD ");
                    queryProfile.Append("    INNER JOIN GDA_HIERARCHY AS H ON H.LEVELNAME = CD.CARGO ");
                    queryProfile.Append("    INNER JOIN GDA_PROFILE_COLLABORATOR_ADMINISTRATION AS PCA ON PCA.BASIC_PROFILE_IDHIERARCHY = H.IDGDA_HIERARCHY ");
                    queryProfile.Append("    WHERE CD.IDGDA_COLLABORATORS = GDA_COLLABORATORS.IDGDA_COLLABORATORS ");
                    queryProfile.Append("    AND CONVERT(DATE, CD.CREATED_AT) >= CONVERT(DATE, DATEADD(DAY, -1 , GETDATE())) ");
                    queryProfile.Append(") ");
                    queryProfile.Append("WHERE PROFILE_COLLABORATOR_ADMINISTRATIONID IS NULL; ");

                    int tentativas = 0;
                    int maxTentativas = 3;
                    while (tentativas < maxTentativas)
                    {
                        try
                        {
                            using (SqlCommand createTableCommandProfile = new SqlCommand(queryProfile.ToString(), connection))
                            {
                                createTableCommandProfile.ExecuteNonQuery();
                                break;
                            }
                        }
                        catch (Exception)
                        {
                            tentativas++;
                        }
                    }
                }
                catch (Exception)
                {

                }

                connection.Close();

            }
            catch (Exception ex)
            {
                Log.insertLogTransaction(t_id.ToString(), "HIERARCHY", "ERRO: " + ex.Message.ToString(), "");
            }





            if (validation == true)
            {
                return CreatedAtRoute("DefaultApi", new { id = hierarchiesModel.First().collaboratorIdentification }, hierarchiesModel);
            }
            else
            {
                return BadRequest("No information entered.");
            }


        }

        //// DELETE: api/Hierarchies/5
        //[ResponseType(typeof(HierarchiesModel))]
        //public IHttpActionResult DeleteHierarchiesModel(int id)
        //{
        //    HierarchiesModel hierarchiesModel = db.HierarchiesModels.Find(id);
        //    if (hierarchiesModel == null)
        //    {
        //        return NotFound();
        //    }

        //    db.HierarchiesModels.Remove(hierarchiesModel);
        //    db.SaveChanges();

        //    return Ok(hierarchiesModel);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}