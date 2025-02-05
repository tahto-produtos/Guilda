using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRepositorio.Models
{
    [Table("GDA_LOG_TRANSACTION")]
    public class TransactionLogTableModel
    {
        [Key]
        public int IDGDA_LOG { get; set; }
        //public int indicator_Id { get; set; }
        public string TRANSACTIONID { get; set; }
        public string TYPE { get; set; }
        public DateTime? CREATED_AT { get; set; }
        public string STATUS { get; set; }

    }
}