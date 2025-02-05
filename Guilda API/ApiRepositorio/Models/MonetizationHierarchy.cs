using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRepositorio.Models
{
    public class MonetizationHierarchy
    {
        public int Quantidade { get; set; }
        public int Soma { get; set; }

    }
}