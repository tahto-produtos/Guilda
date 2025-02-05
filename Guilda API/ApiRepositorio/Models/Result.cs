using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiRepositorio.Models
{
    public class Result
    {
        [Key]
        public string collaboratorIdentification { get; set; }
        public int indicadorId { get; set; }
        public float resultado { get; set; }
        public DateTime? date { get; set; }
        public IEnumerable<string> factors { get; set; }
        public int idgda_result { get; set; }
    }

}