using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiC.Class
{
    public class GalleryResponseModel
    {
        public int id { get; set; }

        public string originalName { get; set; }

        public string key { get; set; }

        public string type { get; set; }

        public string url { get; set; }

        public string created_at { get; set; }
    }

}