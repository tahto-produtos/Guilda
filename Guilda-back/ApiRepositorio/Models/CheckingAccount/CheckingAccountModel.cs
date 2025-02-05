using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiC.Class
{
    public class CheckingAccountModel
    {
        public int Input { get; set; }

        public int Output { get; set; }

        public int Balance { get; set; }

        public string Created_At { get; set; }

        public string Observation { get; set; }

        public string Reason { get; set; }

        public string Collaborator_Id { get; set; }
    }
}