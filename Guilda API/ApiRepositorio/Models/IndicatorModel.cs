using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRepositorio.Models
{
  
    public class IndicatorModel
    {   
        //public int idgda_Indicator { get; set; }
        public int indicatorId { get; set; }
        public string name { get; set; }
        public string description { get; set; }

    }
}