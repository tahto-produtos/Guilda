using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRepositorio.Models
{
    [Table("GDA_HIERARCHY")]
    public class HierarchiesTableModel
    {
        [Key]
        public int IDGDA_HIERARCHY { get; set; }
        public string LEVELNAME { get; set; }
        public int LEVELWEIGHT { get; set; }
        public DateTime? CREATED_AT { get; set; }
        public DateTime? DELETED_AT { get; set; }
    }
}