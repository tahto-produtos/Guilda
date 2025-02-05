using System;
using System.Web;
using ApiC.Class;
using System.Web.Http;
using System.Collections.Generic;
using System.Web.Http.Description;


namespace ApiRepositorio.Controllers
{
    public class AssociateImageToProductsController : ApiController
    {
        [HttpPost]
        //[ResponseType(typeof(List<ReponseVerifyImagesInProductsModel>))]
        public IHttpActionResult AssociateImageToProducts([FromBody] AssociateImageToProductsModel data)
        {
            try
            {
                var response = GalleryClass.AssociateImageToProducts(data);

                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}