using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiRepositorio.Models
{
    public class DisplayRequestModel
    {
        public int PRIORIDADE { get; set; }
        public string NOMECONFIG { get; set; }
        public List<Item> ITENS { get; set; }
        public List<Hierarquia> HIERARQUIA { get; set; }
        public List<Grupo> GRUPO { get; set; }
        public List<Estoque> ESTOQUE { get; set; }
    }

    public class Item
    {
        public int CODPRODUTO { get; set; }
        public int POSICAO { get; set; }
    }

    public class Hierarquia
    {
        public int CODHIERARQUIA { get; set; }
    }

    public class Grupo
    {
        public int CODGRUPO { get; set; }
    }

    public class Estoque
    {
        public int CODESTOQUE { get; set; }
    }
}