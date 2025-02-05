using System;
using System.Web;
using ApiC.Class;
using System.Web.Http;
using System.Collections.Generic;
using System.Web.Http.Description;


namespace ApiRepositorio.Controllers
{
    public class VerifyFileInProductsController : ApiController
    {
        [HttpPost]
        [ResponseType(typeof(List<ReponseVerifyImagesInProductsModel>))]
        public IHttpActionResult VerifyFileInProducts([FromBody] VerifyImagesInProductsModel data)
        {
            try
            {
                var response = GalleryClass.VerifyFileInProducts(data);

                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}