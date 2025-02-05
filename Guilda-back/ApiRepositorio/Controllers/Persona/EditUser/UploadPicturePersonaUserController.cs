using System.Data.SqlClient;
using System.Web.Http;
using System.Web.Http.Description;
using ApiRepositorio.Models;
using System.Web;
using System;
using System.Data;
using ApiC.Class;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class UploadPicturePersonaUserController : ApiController
    {
        [HttpPost]
        [ResponseType(typeof(ResultsModel))]
        public IHttpActionResult PostResultsModel()
        {
            int collaboratorId = 0;
            int personauserId = 0;  
            var token = Request.Headers.Authorization?.Parameter;
            bool tokn = TokenService.TryDecodeToken(token);
            if (tokn == false)
            {
                return Unauthorized();
            }
             collaboratorId = TokenService.InfsUsers.collaboratorId;
            personauserId = TokenService.InfsUsers.personauserId;


            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            var picture = PictureClass.UploadFiles(files, personauserId);
            return Ok(picture);
        }
    }
}