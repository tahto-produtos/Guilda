using System;
using ApiC.Class;
using System.Web.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Data.SqlClient;

namespace ApiRepositorio.Controllers
{
    public class AuthenticationController : ApiController
    {
        public class ConnectionTester
        {
            public static string TestarConexao(string connectionString)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        // Abre a conexão
                        connection.Open();

                        // Se a conexão foi aberta com sucesso, retorna true
                        return "Ok";
                    }
                }
                catch (Exception ex)
                {
                    // Se ocorrer um erro, imprime a mensagem de erro
                    // Console.WriteLine("Erro ao testar a conexão: " + ex.Message);
                    return "NOk - " + ex.Message;
                }
            }
        }
          

        [HttpPost]
        //[ResponseType(typeof(ResponseInputMassiveInputModel))]
        public IHttpActionResult Authentication([FromBody] LoginModel data)
        {
            DateTime dt = Convert.ToDateTime(Database.dtbe);

            if (DateTime.Now.Date >= dt.Date)
            {
                return BadRequest("Ex");
            }

            ValidateDataLogin(data);
            string username = data.Username;
            string password = data.Password;

            //Logar
            var login = AuthenticationClass.AuthenticationCollaborator(username, password, false);
            
            return Ok(login);
        }

        private void ValidateDataLogin(LoginModel data)
        {
            try
            {
                data.Username.ToString();
            }
            catch(Exception ex)
            {
                throw new Exception("Nome de usuário é obrigatório");
            }

            try
            {
                data.Password.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Senha é obrigatório");
            }

            try 
            { 
                if(data.Username == string.Empty)
                {
                    throw new Exception("Nome de usuário é obrigatório");
                }

                if(data.Password == string.Empty)
                {
                    throw new Exception("Senha é obrigatório");
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}