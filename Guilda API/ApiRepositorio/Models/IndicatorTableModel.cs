using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRepositorio.Models
{
    [Table("GDA_INDICATOR")]
    public class IndicatorTableModel
    {
        [Key]
        public int idgda_Indicator { get; set; }
        //public int indicator_Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }

        public int? TRANSACTIONID { get; set; }
        public DateTime? CREATED_AT { get; set; }
        public DateTime? deleted_at { get; set; }

    }
}