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
using System.Transactions;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Description;
using System.Xml.Linq;
using ApiRepositorio.Class;
using ApiRepositorio.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ApiRepositorio.Controllers
{
    public class CollaboratorsController : ApiController
    {
        private RepositorioDBContext db = new RepositorioDBContext();

        public static void inserirPersonaTeste()
        {
            try
            {
                SqlConnection connection = new SqlConnection(Database.Conn);
                connection.Open();

                StringBuilder stbBuscaColla = new StringBuilder();
                stbBuscaColla.Append("SELECT C.IDGDA_COLLABORATORS, MAX(C.NAME) AS NAME, MAX(C.COLLABORATORIDENTIFICATION) AS IDENTIFICATION,  ");
                stbBuscaColla.Append("MAX(C.PHONENUMBER) AS PHONENUMBER, MAX(C.BIRTHDATE) AS BIRTHDATE, MAX(S.IDGDA_STATE) AS IDGDA_STATE,  ");
                stbBuscaColla.Append("MAX(CC.IDGDA_CITY) AS IDGDA_CITY, MAX(C.EMAIL) AS EMAIL  ");
                stbBuscaColla.Append("FROM GDA_COLLABORATORS (NOLOCK) AS C ");
                stbBuscaColla.Append("INNER JOIN GDA_STATE (NOLOCK) S ON C.STATE = S.STATE ");
                stbBuscaColla.Append("INNER JOIN GDA_CITY (NOLOCK) CC ON  CC.CITY = C.CITY ");
                stbBuscaColla.Append("WHERE IDGDA_COLLABORATORS NOT IN ( ");
                stbBuscaColla.Append("SELECT IDGDA_COLLABORATORS FROM GDA_PERSONA_COLLABORATOR_USER ");
                stbBuscaColla.Append(") ");
                stbBuscaColla.Append("GROUP BY C.IDGDA_COLLABORATORS ");
                using (SqlCommand commandInsert2 = new SqlCommand(stbBuscaColla.ToString(), connection))
                {
                    using (SqlDataReader reader2 = commandInsert2.ExecuteReader())
                    {
                        while (reader2.Read())
                        {
                            string name = reader2["NAME"].ToString();
                            string idgda_collaborators = reader2["IDGDA_COLLABORATORS"].ToString();
                            string identification = reader2["IDENTIFICATION"].ToString();
                            string phoneNumber = reader2["PHONENUMBER"].ToString();
                            DateTime birthDate = Convert.ToDateTime(reader2["BIRTHDATE"].ToString());
                            string codState = reader2["IDGDA_STATE"].ToString();
                            string codCity = reader2["IDGDA_CITY"].ToString();
                            string email = reader2["EMAIL"].ToString();

                            //Inserir na rede social
                            StringBuilder stbPersona = new StringBuilder();
                            stbPersona.Append("INSERT INTO GDA_PERSONA_USER (IDGDA_PERSONA_USER_TYPE, IDGDA_PERSONA_USER_VISIBILITY, NAME, BC, SOCIAL_NAME, SHOW_AGE, PICTURE, CREATED_AT, DELETED_AT, CREATED_BY, DELETED_BY) ");
                            stbPersona.Append("VALUES (1, "); //IDGDA_PERSONA_USER_TYPE = 1 = Pessoal
                            stbPersona.Append("1, "); //IDGDA_PERSONA_USER_VISIBILITY = 1 = Publico
                            stbPersona.AppendFormat("'{0}', ", name); //Name
                            stbPersona.AppendFormat("'{0}', ", identification); //BC
                            stbPersona.Append("'',  "); //SOCIAL_NAME
                            stbPersona.Append("1, "); //SHOW_AGE
                            stbPersona.Append("'', "); //PICTURE
                            stbPersona.Append("GETDATE(), "); //CREATED_AT
                            stbPersona.Append("NULL, "); //DELETED_AT
                            stbPersona.Append("NULL, "); //CREATED_BY
                            stbPersona.Append("NULL "); //DELETED_BY
                            stbPersona.Append(") SELECT @@IDENTITY AS 'CODPERSONA' ");
                            string codPersona = "";


                            using (SqlCommand commandInsert = new SqlCommand(stbPersona.ToString(), connection))
                            {
                                using (SqlDataReader reader = commandInsert.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        codPersona = reader["CODPERSONA"].ToString();
                                    }
                                }
                            }

                            StringBuilder stbPersonaDetails = new StringBuilder();
                            stbPersonaDetails.Append("INSERT INTO GDA_PERSONA_USER_DETAILS (IDGDA_PERSONA_USER, YOUR_MOTIVATIONS, GOALS, PHONE_NUMBER, BIRTH_DATE, IDGDA_STATE, IDGDA_CITY, WHO_IS, EMAIL) ");
                            stbPersonaDetails.Append("VALUES ( ");
                            stbPersonaDetails.AppendFormat("'{0}', ", codPersona); //IDGDA_PERSONA_USER
                            stbPersonaDetails.Append("'',  "); //YOUR_MOTIVATIONS
                            stbPersonaDetails.Append("'',  "); //GOALS
                            stbPersonaDetails.AppendFormat("'{0}',  ", phoneNumber); // PHONE_NUMBER
                            stbPersonaDetails.AppendFormat("'{0}',  ", birthDate.ToString("yyyy-MM-dd")); //BIRTH_DATE
                            stbPersonaDetails.AppendFormat("'{0}',  ", codState); //IDGDA_STATE
                            stbPersonaDetails.AppendFormat("'{0}',  ", codCity); //IDGDA_CITY
                            stbPersonaDetails.Append("'#Setor - #Cargo - #Moedas - #Grupo',  "); //WHO_IS
                            stbPersonaDetails.AppendFormat("'{0}' ", email); //EMAIL
                            stbPersonaDetails.Append(") ");
                            SqlCommand createTableCommand = new SqlCommand(stbPersonaDetails.ToString(), connection);
                            createTableCommand.ExecuteNonQuery();

                            StringBuilder stbPersonaCollaborator = new StringBuilder();
                            stbPersonaCollaborator.Append("INSERT INTO GDA_PERSONA_COLLABORATOR_USER (IDGDA_COLLABORATORS, IDGDA_PERSONA_USER) ");
                            stbPersonaCollaborator.Append("VALUES ( ");
                            stbPersonaCollaborator.AppendFormat("'{0}', ", Convert.ToInt32(idgda_collaborators)); //IDGDA_COLLABORATORS
                            stbPersonaCollaborator.AppendFormat("'{0}' ", codPersona); //IDGDA_PERSONA_USER
                            stbPersonaCollaborator.Append(") ");
                            SqlCommand createTableCommand2 = new SqlCommand(stbPersonaCollaborator.ToString(), connection);
                            createTableCommand2.ExecuteNonQuery();
                        }
                    }
                }

            }
            catch (Exception)
            {

            }
        }


        public void inserirPersona(string idgda_collaborators, string name, string identification, string phoneNumber, DateTime birthDate, string email, string state, string city)
        {
            try
            {
                SqlConnection connection = new SqlConnection(Database.Conn);
                connection.Open();

                //Verificar e inserir estado
                StringBuilder stbStateS = new StringBuilder();
                stbStateS.Append("SELECT IDGDA_STATE FROM GDA_STATE (NOLOCK) ");
                stbStateS.AppendFormat("WHERE STATE = '{0}' ", state);

                string codState = "";
                using (SqlCommand commandSelect = new SqlCommand(stbStateS.ToString(), connection))
                {
                    using (SqlDataReader reader = commandSelect.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            codState = reader["IDGDA_STATE"].ToString();
                        }
                    }
                }

                if (codState == "")
                {
                    StringBuilder stbStateInsert = new StringBuilder();
                    stbStateInsert.AppendFormat("INSERT INTO GDA_STATE (STATE, CREATED_AT)  ");
                    stbStateInsert.AppendFormat("VALUES ( ");
                    stbStateInsert.AppendFormat("'{0}', ", state);
                    stbStateInsert.AppendFormat("GETDATE() ");
                    stbStateInsert.AppendFormat(") SELECT @@IDENTITY AS 'CODSTATE' ");

                    using (SqlCommand commandInsert = new SqlCommand(stbStateInsert.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                codState = reader["CODSTATE"].ToString();
                            }
                        }
                    }
                }

                string codCity = "";
                StringBuilder stbSelectCity = new StringBuilder();
                stbSelectCity.Append("SELECT * FROM GDA_CITY (NOLOCK) ");
                stbSelectCity.Append("WHERE ");
                stbSelectCity.AppendFormat("IDGDA_STATE = {0} ", codState);
                stbSelectCity.AppendFormat("AND CITY = '{0}' ", city);

                using (SqlCommand commandInsert = new SqlCommand(stbSelectCity.ToString(), connection))
                {
                    using (SqlDataReader reader = commandInsert.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            codCity = reader["IDGDA_CITY"].ToString();
                        }
                    }
                }

                if (codCity == "")
                {
                    StringBuilder stbInsertCity = new StringBuilder();
                    stbInsertCity.Append("INSERT INTO GDA_CITY (IDGDA_STATE, CITY, CREATED_AT)  ");
                    stbInsertCity.Append("VALUES ( ");
                    stbInsertCity.AppendFormat("'{0}', ", codState);
                    stbInsertCity.AppendFormat("'{0}', ", city);
                    stbInsertCity.Append("GETDATE() ");
                    stbInsertCity.Append(") SELECT @@IDENTITY AS 'CODCITY'");

                    using (SqlCommand commandInsert = new SqlCommand(stbInsertCity.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                codCity = reader["CODCITY"].ToString();
                            }
                        }
                    }
                }

                //Inserir na rede social
                StringBuilder stbPersona = new StringBuilder();
                stbPersona.Append("INSERT INTO GDA_PERSONA_USER (IDGDA_PERSONA_USER_TYPE, IDGDA_PERSONA_USER_VISIBILITY, NAME, BC, SOCIAL_NAME, SHOW_AGE, PICTURE, CREATED_AT, DELETED_AT, CREATED_BY, DELETED_BY) ");
                stbPersona.Append("VALUES (1, "); //IDGDA_PERSONA_USER_TYPE = 1 = Pessoal
                stbPersona.Append("1, "); //IDGDA_PERSONA_USER_VISIBILITY = 1 = Publico
                stbPersona.AppendFormat("'{0}', ", name); //Name
                stbPersona.AppendFormat("'{0}', ", identification); //BC
                stbPersona.Append("'',  "); //SOCIAL_NAME
                stbPersona.Append("1, "); //SHOW_AGE
                stbPersona.Append("'', "); //PICTURE
                stbPersona.Append("GETDATE(), "); //CREATED_AT
                stbPersona.Append("NULL, "); //DELETED_AT
                stbPersona.Append("NULL, "); //CREATED_BY
                stbPersona.Append("NULL "); //DELETED_BY
                stbPersona.Append(") SELECT @@IDENTITY AS 'CODPERSONA' ");
                string codPersona = "";


                using (SqlCommand commandInsert = new SqlCommand(stbPersona.ToString(), connection))
                {
                    using (SqlDataReader reader = commandInsert.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            codPersona = reader["CODPERSONA"].ToString();
                        }
                    }
                }

                StringBuilder stbPersonaDetails = new StringBuilder();
                stbPersonaDetails.Append("INSERT INTO GDA_PERSONA_USER_DETAILS (IDGDA_PERSONA_USER, YOUR_MOTIVATIONS, GOALS, PHONE_NUMBER, BIRTH_DATE, IDGDA_STATE, IDGDA_CITY, WHO_IS, EMAIL) ");
                stbPersonaDetails.Append("VALUES ( ");
                stbPersonaDetails.AppendFormat("'{0}', ", codPersona); //IDGDA_PERSONA_USER
                stbPersonaDetails.Append("'',  "); //YOUR_MOTIVATIONS
                stbPersonaDetails.Append("'',  "); //GOALS
                stbPersonaDetails.AppendFormat("'{0}',  ", phoneNumber); // PHONE_NUMBER
                stbPersonaDetails.AppendFormat("'{0}',  ", birthDate.ToString("yyyy-MM-dd")); //BIRTH_DATE
                stbPersonaDetails.AppendFormat("'{0}',  ", codState); //IDGDA_STATE
                stbPersonaDetails.AppendFormat("'{0}',  ", codCity); //IDGDA_CITY
                stbPersonaDetails.Append("'#Setor - #Cargo - #Moedas - #Grupo',  "); //WHO_IS
                stbPersonaDetails.AppendFormat("'{0}' ", email); //EMAIL
                stbPersonaDetails.Append(") ");
                SqlCommand createTableCommand = new SqlCommand(stbPersonaDetails.ToString(), connection);
                createTableCommand.ExecuteNonQuery();

                StringBuilder stbPersonaCollaborator = new StringBuilder();
                stbPersonaCollaborator.Append("INSERT INTO GDA_PERSONA_COLLABORATOR_USER (IDGDA_COLLABORATORS, IDGDA_PERSONA_USER) ");
                stbPersonaCollaborator.Append("VALUES ( ");
                stbPersonaCollaborator.AppendFormat("'{0}', ", Convert.ToInt32(idgda_collaborators)); //IDGDA_COLLABORATORS
                stbPersonaCollaborator.AppendFormat("'{0}' ", codPersona); //IDGDA_PERSONA_USER
                stbPersonaCollaborator.Append(") ");
                SqlCommand createTableCommand2 = new SqlCommand(stbPersonaCollaborator.ToString(), connection);
                createTableCommand2.ExecuteNonQuery();

               
            }
            catch (Exception)
            {

            }
        }


        [ResponseType(typeof(CollaboratorTableModel))]
        public IHttpActionResult PostCollaboratorModel(long t_id, IEnumerable<CollaboratorModel> collaboratorModel)
        {
            bool validation = false;
            Log.insertLogTransaction(t_id.ToString(), "COLLABORATOR", "START", "");

            //Valida Token
            var tkn = TokenValidate.validate(Request.Headers);
            if (tkn == false)
            {
                Log.insertLogTransaction(t_id.ToString(), "COLLABORATOR", "Invalid Token!", "");
                return BadRequest("Invalid Token!");
            }

            //Valida Model
            if (!ModelState.IsValid)
            {
                Log.insertLogTransaction(t_id.ToString(), "COLLABORATOR", "Invalid Model!", "");
                return BadRequest(ModelState);
            }


            //Valida Transaction
            int? t = TransactionValidade.validate(Request.Headers, t_id);
            if (t is null)
            {
                Log.insertLogTransaction(t_id.ToString(), "COLLABORATOR", "Invalid Transaction!", "");
                return BadRequest("Invalid Transaction!");
            }

            //Insert de collaboradores
            try
            {
                db.Database.Connection.Open();
                try
                {
                    db.Database.ExecuteSqlCommand(@"SET IDENTITY_INSERT [dbo].[GDA_COLLABORATORS] ON");
                }
                catch (Exception)
                {

                }

                int count = 0;

                foreach (CollaboratorModel cm in collaboratorModel)
                {
                    count += 1;

                    var idEx = cm.matricula.Replace("BC", "");

                    //Valida se o colaborador ja existe
                    var qtd = db.CollaboratorTableModels.Where(p => p.idgda_Collaborators.ToString() == idEx).Count();

                    if (qtd > 0)
                    {
                       

                        //UPDATE
                        CollaboratorTableModel ctm = db.CollaboratorTableModels.Where(p => p.idgda_Collaborators.ToString() == idEx).FirstOrDefault();


                        try
                        {

                            ctm.transactionId = t;
                            ctm.name = cm.name;
                            ctm.collaboratorIdentification = cm.Identification;
                            ctm.matricula = cm.matricula;
                            ctm.genre = cm.genre;
                            ctm.active = cm.active;
                            ctm.birthDate = cm.birthDate;
                            ctm.admissionDate = cm.admissionDate;
                            ctm.email = cm.email;
                            ctm.civilState = cm.civilState;
                            ctm.street = cm.street;
                            ctm.number = cm.number;
                            ctm.neighborhood = cm.neighborhood;
                            ctm.city = cm.city;
                            ctm.state = cm.state;
                            ctm.country = cm.country;
                            ctm.homeNumber = cm.homeNumber;
                            ctm.phoneNumber = cm.phoneNumber;
                            ctm.schooling = cm.schooling;
                            ctm.contractorControlId = cm.contractorControlId;
                            ctm.dependantNumber = cm.dependantsNumber.ToString();
                            ctm.entryDate = cm.entryDate;
                            ctm.created_at = DateTime.Now;
                            //Processo Hierarquia base
                            ctm.new_agent = 0;

                            db.SaveChanges();
                            validation = true;
                        }
                        catch
                        {

                        }

                    }
                    else
                    {
                        //INSERT
                        CollaboratorTableModel ctm = new CollaboratorTableModel();

                        try
                        {
                            ctm.idgda_Collaborators = Convert.ToInt32(idEx);
                            ctm.transactionId = t;
                            ctm.name = cm.name;
                            ctm.collaboratorIdentification = cm.Identification;
                            ctm.matricula = cm.matricula;
                            ctm.genre = cm.genre;
                            ctm.active = cm.active;
                            ctm.birthDate = cm.birthDate;
                            ctm.admissionDate = cm.admissionDate;
                            ctm.email = cm.email;
                            ctm.civilState = cm.civilState;
                            ctm.street = cm.street;
                            ctm.number = cm.number;
                            ctm.neighborhood = cm.neighborhood;
                            ctm.city = cm.city;
                            ctm.state = cm.state;
                            ctm.country = cm.country;
                            ctm.homeNumber = cm.homeNumber;
                            ctm.phoneNumber = cm.phoneNumber;
                            ctm.schooling = cm.schooling;
                            ctm.contractorControlId = cm.contractorControlId;
                            ctm.dependantNumber = cm.dependantsNumber.ToString();
                            ctm.entryDate = cm.entryDate;
                            ctm.created_at = DateTime.Now;
                            //Processo Hierarquia base
                            ctm.new_agent = 1;

                            db.CollaboratorTableModels.Add(ctm);
                            db.SaveChanges();

                            //Inserir persona cidade e estado
                            inserirPersona(idEx, cm.name, cm.Identification, cm.phoneNumber, cm.birthDate, cm.email, cm.state, cm.city);


                            validation = true;
                        }
                        catch (Exception)
                        {

                        }

                    }

                }
            }
            finally
            {
                db.Database.ExecuteSqlCommand(@"SET IDENTITY_INSERT [dbo].[GDA_COLLABORATORS] OFF");
                db.Database.Connection.Close();
            }



            //Insert Ativos (Modelo novo bulk)
            try
            {

                JArray jsonArray = new JArray(collaboratorModel.Select(obj =>
                {
                    JObject jsonObject = JObject.FromObject(obj);
                    if (jsonObject.ContainsKey("Identification"))
                    {
                        // Realiza a substituição do valor na coluna
                        var oldValue = jsonObject["Identification"].ToString();
                        var newValue = oldValue.Replace("BC", "");
                        jsonObject["Identification"] = newValue;
                    }


                    jsonObject["transactionId"] = t;
                    return jsonObject;
                }));


                string formattedDate = "";
                try
                {
                    string dateString = (string)jsonArray[0]["entryDate"];
                    DateTime date = DateTime.ParseExact(dateString, "MM/dd/yyyy HH:mm:ss", null);
                    formattedDate = date.ToString("yyyy-MM-dd");
                }
                catch (Exception)
                {


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

                    // Inserir os dados do DataTable na tabela temporária
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = "#TEMPTABLE";
                        bulkCopy.WriteToServer(dt);
                    }

                    // Inserir os dados do DataTable na tabela de LOG
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = "GDA_HISTORY_COLLABORATORS";
                        bulkCopy.WriteToServer(dt);
                    }

                    //Inserir colunas que tiveram informações alterada [DESCOMENTAR QUANDO EXISTIR COLUNA]
                    StringBuilder queryInsertResult2 = new StringBuilder();
                    queryInsertResult2.Append("MERGE INTO GDA_HISTORY_COLLABORATOR_ACTIVE AS TARGET  ");
                    queryInsertResult2.Append("USING #TEMPTABLE AS SOURCE  ");
                    queryInsertResult2.Append("ON (TARGET.IDGDA_COLLABORATORS = SOURCE.IDENTIFICATION AND TARGET.ENTRYDATE = SOURCE.ENTRYDATE)  ");
                    queryInsertResult2.Append("WHEN NOT MATCHED BY TARGET THEN  ");
                    queryInsertResult2.Append("INSERT (IDGDA_COLLABORATORS, ENTRYDATE, ACTIVE, TRANSACTIONID)  ");
                    queryInsertResult2.Append("VALUES (SOURCE.IDENTIFICATION, SOURCE.ENTRYDATE, SOURCE.ACTIVE, SOURCE.TRANSACTIONID);  ");
                    SqlCommand createTableCommand2 = new SqlCommand(queryInsertResult2.ToString(), connection);
                    createTableCommand2.CommandTimeout = 0;
                    createTableCommand2.ExecuteNonQuery();

                    string dropTempTableQuery2 = $"DROP TABLE #TEMPTABLE";
                    using (SqlCommand dropTempTableCommand2 = new SqlCommand(dropTempTableQuery2, connection))
                    {
                        dropTempTableCommand2.ExecuteNonQuery();
                    }

                    Log.insertLogTransaction(t_id.ToString(), "COLLABORATOR", "CONCLUDED", formattedDate);
                    validation = true;
                }
                catch (Exception ex)
                {
                    Log.insertLogTransaction(t_id.ToString(), "COLLABORATOR", "ERRO: " + ex.Message.ToString(), "");
                }
                connection.Close();

            }
            catch (Exception ex)
            {
                Log.insertLogTransaction(t_id.ToString(), "COLLABORATOR", "ERRO: " + ex.Message.ToString(), "");
            }


            if (validation == true)
            {
                return CreatedAtRoute("DefaultApi", new { id = collaboratorModel.First().idgda_Collaborators }, collaboratorModel);
            }
            else
            {
                return BadRequest("No information entered.");
            }


            //return CreatedAtRoute("DefaultApi", new { id = collaboratorModel.idgda_Collaborators }, collaboratorModel);
        }

        // DELETE: api/Collaborators/5
        //[ResponseType(typeof(CollaboratorModel))]
        //public IHttpActionResult DeleteCollaboratorModel(int id)
        //{
        //    CollaboratorModel collaboratorModel = db.CollaboratorModels.Find(id);
        //    if (collaboratorModel == null)
        //    {
        //        return NotFound();
        //    }

        //    db.CollaboratorModels.Remove(collaboratorModel);
        //    db.SaveChanges();

        //    return Ok(collaboratorModel);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CollaboratorModelExists(int id)
        {
            return db.CollaboratorTableModels.Count(e => e.idgda_Collaborators == id) > 0;
        }
    }
}