

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRepositorio.Models
{
    [Table("GDA_COLLABORATORS")]
    public class CollaboratorTableModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int idgda_Collaborators { get; set; }
        public long? transactionId { get; set; }
        public string name { get; set; }
        public string collaboratorIdentification { get; set; }
        public string matricula { get; set; }
        public string genre { get; set; }
        public string active { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? birthDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? admissionDate { get; set; }
        public string email { get; set; }
        public int? civilState { get; set; }
        public string street { get; set; }
        public int? number { get; set; }
        public string neighborhood { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string homeNumber { get; set; }
        public string phoneNumber { get; set; }
        public int? schooling { get; set; }
        public string contractorControlId { get; set; }
        public string dependantNumber { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? entryDate { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        //Processo Hierarquia base
        public int? new_agent { get; set; }
    }
}