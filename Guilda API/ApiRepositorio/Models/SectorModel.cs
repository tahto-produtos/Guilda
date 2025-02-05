using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRepositorio.Models
{
    [Table("GDA_SECTOR")]
    public class SectorModel
    {
        [Key]
        public int IDGDA_SECTOR { get; set; }
        public string NAME { get; set; }
        public int LEVEL { get; set; }
        //public string VALUE { get; set; }
        public DateTime? CREATED_AT { get; set; }
        public DateTime? DELETED_AT { get; set; }
    }
}