using System.Data.SqlClient;
using System.Web.Http;
using System.Web.Http.Description;
using ApiRepositorio.Models;
using System.Web;
using System;
using System.Data;
using ApiC.Class;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using static TokenService;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class PersonaListReactionController : ApiController
    {
        [HttpGet]
        public IHttpActionResult PostResultsModel(string word)
        {
            int collaboratorId = 0;
            int personauserId = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            collaboratorId = inf.collaboratorId;
            personauserId = inf.personauserId;

            List<Reaction> rmams = new List<Reaction>();
            rmams = returnTables.listReactions(word);
            return Ok(rmams);
            // Use o método Ok() para retornar o objeto serializado em JSON
        }

    }
}