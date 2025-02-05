using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRepositorio.Models
{
    [Table("GDA_TOKEN")]
    public class TokenModel
    {
        [Key]
        public int IDGDA_TOKEN { get; set; }
        public string TOKEN { get; set; }
        public int ACTIVE { get; set; }
    }
  
}