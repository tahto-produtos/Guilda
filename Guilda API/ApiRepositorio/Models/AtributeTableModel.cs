using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRepositorio.Models
{
    [Table("GDA_ATRIBUTES")]
    public class AtributeTableModel
    {
        [Key]
        public int IDGDA_ATRIBUTES { get; set; }
        public int IDGDA_COLLABORATORS { get; set; }
        public string NAME { get; set; }
        public int LEVEL { get; set; }
        public string VALUE { get; set; }
        public DateTime? CREATED_AT { get; set; }
    }
}