using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRepositorio.Models
{
    public class HierarchiesIdModel
    {
        public string parentId { get; set; }
        public string levelName { get; set; }
        public string level { get; set; }

    }
}