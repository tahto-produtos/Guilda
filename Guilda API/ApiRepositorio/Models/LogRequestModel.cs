using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRepositorio.Models
{
    [Table("GDA_LOGREQUEST")]
    public class LogRequestModel
    {
        [Key]
        public int IDGDA_LOGREQUEST { get; set; }
        public string REQUEST { get; set; }
        public string ROUTE { get; set; }
        public int RETURN { get; set; }
    }
  
}