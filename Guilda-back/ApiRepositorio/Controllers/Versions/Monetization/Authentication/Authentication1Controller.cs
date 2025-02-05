using System;
using ApiC.Class;
using System.Web.Http;
using System.IdentityModel.Tokens.Jwt;

namespace ApiRepositorio.Controllers
{
    public class Authentication1Controller : ApiController
    {
  

        [HttpPost]
        //[ResponseType(typeof(ResponseInputMassiveInputModel))]
        public IHttpActionResult Authentication([FromBody] LoginModel data)
        {
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