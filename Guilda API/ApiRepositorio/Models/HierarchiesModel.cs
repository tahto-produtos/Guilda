using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRepositorio.Models
{
    public class HierarchiesModel
    {
        [Key]
        public string collaboratorIdentification { get; set; }
        public string levelName { get; set; }
        public int levelWeight { get; set; }
        public string parentIdentification { get; set; }
        public string contractorControlId { get; set; }
        public DateTime? date { get; set; }

    }
}