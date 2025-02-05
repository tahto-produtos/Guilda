using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRepositorio.Models
{
    [Table("GDA_RESULT")]
    public class ResultsModel
    {
        [Key]
        public int IDGDA_RESULT { get; set; }
        public int INDICADORID { get; set; }
        public int TRANSACTIONID { get; set; }
        public float RESULT { get; set; }
        public DateTime? CREATED_AT { get; set; }
        public int IDGDA_COLLABORATORS { get; set; }

        public string factors { get; set; }

    }

    

}