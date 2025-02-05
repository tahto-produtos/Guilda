using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRepositorio.Models
{
    [Table("GDA_HISTORY_HIERARCHY_RELATIONSHIP")]
    public class HistoryHierarchiesModel
    {
        [Key]
        public int IDGDA_HISTORY_HIERARCHY_RELATIONSHIP { get; set; }
        public int IDGDA_COLLABORATORS { get; set; }
        public int IDGDA_HIERARCHY { get; set; }
        public int PARENTIDENTIFICATION { get; set; }
        public string CONTRACTORCONTROLID { get; set; }
        public string LEVELNAME { get; set; }
        public int LEVELWEIGHT { get; set; }
        public DateTime? DATE { get; set; }
        public DateTime? CREATED_AT { get; set; }
        public int TRANSACTIONID { get; set; }
    }
}