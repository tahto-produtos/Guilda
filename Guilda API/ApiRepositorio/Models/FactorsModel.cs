using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ApiRepositorio.Models
{
    [Table("GDA_FACTOR")]
    public class FactorsModel
    {
        [Key]
        public int IDGDA_FACTOR { get; set; }
        public int INDEX { get; set; }
        public string FACTOR { get; set; }

        public int IDGDA_RESULT { get; set; }
        
    }
}