

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRepositorio.Models
{
    public class CollaboratorModel
    {
        public int idgda_Collaborators { get; set; }
        public long transactionId { get; set; }
        public string name { get; set; }
        public string Identification { get; set; }
        public string matricula { get; set; }
        public string genre { get; set; }
        public string active { get; set; }
        [Column(TypeName = "Date")]
        public DateTime birthDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime admissionDate { get; set; }
        public string email { get; set; }
        public int civilState { get; set; }
        public string street { get; set; }
        public int number { get; set; }
        public string neighborhood { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string homeNumber { get; set; }
        public string phoneNumber { get; set; }
        public int schooling { get; set; }
        public string contractorControlId { get; set; }
        public int dependantsNumber { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? entryDate { get; set; }
    }
}