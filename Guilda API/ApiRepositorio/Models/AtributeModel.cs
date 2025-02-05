using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApiRepositorio.Models
{
    public class AtributeModel
    {
        [Key]
        public string collaboratorIdentification { get; set; }
        public string name { get; set; }
        public int level { get; set; }
        public string value { get; set; }
        public DateTime? date { get; set; }
    }
}