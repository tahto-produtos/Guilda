using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRepositorio.Models
{
    [Table("GDA_TRANSACTION")]
    public class TransactionModel
    {
        [Key]
        public int idgda_Transaction { get; set; }
        public string transactionId { get; set; }
        public DateTime? created_at { get; set; }
        public Boolean? complete { get; set; }

    }
}