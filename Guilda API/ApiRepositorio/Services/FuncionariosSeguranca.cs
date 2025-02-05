using ApiRepositorio.Models;
using System;
using System.Linq;
namespace ApiRepositorio.Services
{
    public class FuncionariosSeguranca
    {
        public static bool Login(string login, string senha)
        {
            using (RepositorioDBContext entities = new RepositorioDBContext())
            {
               return entities.UserModels.Any(user =>
               user.Login.Equals(login, StringComparison.OrdinalIgnoreCase)
               && user.Senha == senha);
            }
        }
    }
}