using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiC.Class
{
    public class StockModel
    {
        public int idGdaStock { get; set; }

        public string description { get; set; }

        public DateTimeOffset createdAt { get; set; }

        public DateTimeOffset? deletedAt { get; set; }

        public string city { get; set; }

        public string type { get; set; }
    }
}