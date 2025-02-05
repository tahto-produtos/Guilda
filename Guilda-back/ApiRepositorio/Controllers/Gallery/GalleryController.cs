using System;
using System.Web;
using ApiC.Class;
using System.Web.Http;
using System.Collections.Generic;
using System.Web.Http.Description;


namespace ApiRepositorio.Controllers
{
    public class GalleryController : ApiController
    {
        [HttpGet]
        [ResponseType(typeof(List<GalleryResponseModel>))]
        public IHttpActionResult Gallery()
        {

            var galleryFiles = GalleryClass.ListGalleryFiles();
            return Ok(galleryFiles);
        }

        [HttpPost]
        [ResponseType(typeof(List<GalleryResponseModel>))]
        public IHttpActionResult GalleryUpload()
        {
            try
            {
                HttpFileCollection filesResquest = System.Web.HttpContext.Current.Request.Files;
                var galleryFiles = GalleryClass.UploadFiles(filesResquest);
               
                return Ok(galleryFiles);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpDelete]
        [ResponseType(typeof(List<GalleryModel>))]
        public IHttpActionResult GalleryDelete([FromBody] DeleteFileGalleryModel files)
        {
            try
            {
                var response = GalleryClass.DeleteFilesToBlob(files);

                return Ok();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}