using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.UI.WebControls;
using ApiRepositorio.Models;
using Microsoft.Diagnostics.Tracing.Analysis.JIT;

//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ResetPasswordController : ApiController
    {
        public class InputModel
        {
            public string LOGIN { get; set; }
            //public string PASSWORD { get; set; }
            //public int IDCOLLABORATOR { get; set; }
        }
        [HttpPost]
        [ResponseType(typeof(ResultsModel))]
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {
            int validaUser = 0;
            //string login = HttpContext.Current.Request.Form["LOGIN"];
            //string hashedPassword = BCrypt.Net.BCrypt.HashPassword(login, 8);

            //string updateUsers = $"UPDATE GDA_USERS SET SENHA = '{hashedPassword}' WHERE LOGIN = '{login}'";
            //string updateCollaborators = $"UPDATE GDA_COLLABORATORS SET FIRST_LOGIN = 1 WHERE MATRICULA = '{login}'";

            string MATRICULA = inputModel.LOGIN;
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(MATRICULA, 8);
            string idCollaborator = MATRICULA.Replace("BC", "");
            string idCollaboratorCadastrado = "";
            //string PASSWORD = BCrypt.Net.BCrypt.HashPassword(inputModel.PASSWORD, 8);
            //int IDCOLLABORATOR = inputModel.IDCOLLABORATOR;

            //Validar se ja temos inserido na base, caso nao contem realiza upate.

            //Validar se temos mais de um usuario cadastrado nas senhas
            string strQtdLogin = $"SELECT COUNT(0) AS QTD, MAX(COLLABORATOR_ID) AS COLLABORATOR_ID FROM GDA_USERS (NOLOCK) WHERE (COLLABORATOR_ID = '{idCollaborator}' OR LOGIN = '{MATRICULA}')";
            int qtd = 0;
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(strQtdLogin.ToString(), connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            if (reader.Read())
                            {
                                qtd = Convert.ToInt32(reader["QTD"].ToString());
                                idCollaboratorCadastrado = reader["COLLABORATOR_ID"].ToString();
                            }
                        }
                    }
                }
                connection.Close();
            }

            if (qtd > 1 || idCollaboratorCadastrado == "")
            {
                string deleteLogins = $"";
                if (idCollaboratorCadastrado != "")
                {
                    deleteLogins = $"DELETE FROM GDA_USERS WHERE COLLABORATOR_ID = '{idCollaboratorCadastrado}'";
                }
                else
                {
                    deleteLogins = $"DELETE FROM GDA_USERS WHERE LOGIN = '{MATRICULA}'";
                }
 
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(deleteLogins.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }

                string insertLogins = $"INSERT GDA_USERS (LOGIN, SENHA, COLLABORATOR_ID) VALUES ('{MATRICULA}', '{hashedPassword}', '{idCollaborator}') ";
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(insertLogins.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }


            //string Str = $"SELECT LOGIN  FROM GDA_USERS (NOLOCK) WHERE  Login = '{MATRICULA}'";
            string sqlCollaborator = $"SELECT ACTIVE FROM GDA_COLLABORATORS (NOLOCK) WHERE COLLABORATORIDENTIFICATION = '{MATRICULA}'";
            string Str = $"SELECT LOGIN, COLLABORATOR_ID  FROM GDA_USERS (NOLOCK) WHERE  Login = '{MATRICULA}'";
            string InsertUsers = $"INSERT INTO GDA_USERS (Login, Senha) VALUES ('{MATRICULA}','{hashedPassword}')";
            string updateUsers = $"UPDATE GDA_USERS SET SENHA = '{hashedPassword}' WHERE LOGIN = '{MATRICULA}'";
            string updateCollaborators = $"UPDATE GDA_COLLABORATORS SET FIRST_LOGIN = 1 WHERE MATRICULA = '{MATRICULA}'";


            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sqlCollaborator.ToString(), connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                if (reader["ACTIVE"].ToString() == "false")
                                {
                                    throw new Exception("Não é possível resetar a senha, o colaborador está inativo");
                                }
                            }
                        }
                    }
                }

                using (SqlCommand command = new SqlCommand(Str.ToString(), connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["COLLABORATOR_ID"].ToString() != "")
                            {
                                validaUser = +1;
                            }
                            //Caso tenha cadastro mas sem o preenchimento do collaborator_id
                            if (reader["LOGIN"].ToString() != "" && validaUser == 0)
                            {
                                validaUser = +1;

                                using (SqlCommand commandUp = new SqlCommand($"UPDATE GDA_USERS SET COLLABORATOR_ID = {MATRICULA.Replace("BC", "")}  WHERE LOGIN = '{MATRICULA}'", connection))
                                {
                                    commandUp.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }

                if (validaUser == 0)
                {
                    using (SqlCommand command = new SqlCommand(InsertUsers, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    using (SqlCommand command = new SqlCommand(updateCollaborators, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                else
                {   
                    using (SqlCommand command = new SqlCommand(updateUsers, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    using (SqlCommand command = new SqlCommand(updateCollaborators, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                connection.Close();
            }
            return Ok("Senhas resetadas com sucesso.");
        }
    }
}