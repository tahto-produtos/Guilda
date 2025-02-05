using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiC.Class
{
    public class AssociateImageToProductsModel
    {
        public int id { get; set; }

        public List<ProductImageModel> products { get; set; } = new List<ProductImageModel>();
    }
}