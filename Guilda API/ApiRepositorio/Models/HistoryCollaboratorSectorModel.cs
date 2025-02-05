using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRepositorio.Models
{
    [Table("GDA_HISTORY_COLLABORATOR_SECTOR")]
    public class HistoryCollaboratorSectorModel
    {
        [Key]
        public int IDGDA_HISTORY_COLLABORATOR_SECTOR { get; set; }
        public DateTime? CREATED_AT { get; set; }
        public int IDGDA_COLLABORATORS { get; set; }
        public int IDGDA_SECTOR { get; set; }
        public int TRANSACTIONID { get; set; }
    }
}