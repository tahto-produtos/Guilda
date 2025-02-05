using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiC.Class
{
    public class ResponseInputMassiveInputModel
    {
        public List<CheckingAccountModel> success { get; set; }

        public List<CheckingAccountModel> failed { get; set; }
    }
}