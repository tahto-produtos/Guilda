using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiRepositorio.Models
{
    public class DisplayResponseModel
    {
        public int IDCONFIG { get; set; }
        public int PRIORIDADE { get; set; }
        public string NOMEPRIORIDADE { get; set; }
        public string NOMECONFIG { get; set; }

        public string STATUS { get; set; }
        public List<ItemResponse> ITENS { get; set; }
        public List<HierarquiaResponse> HIERARQUIA { get; set; }
        public List<GrupoResponse> GRUPO { get; set; }
        public List<EstoqueResponse> ESTOQUE { get; set; }
    }

    public class ItemResponse
    {
        public int CODPRODUTO { get; set; }
        public string NOMEPRODUTO { get; set; }
        public int POSICAO { get; set; }
    }

    public class HierarquiaResponse
    {
        public int CODHIERARQUIA { get; set; }
        public string NOMEHIERARQUIA { get; set; }
    }

    public class GrupoResponse
    {
        public int CODGRUPO { get; set; }
        public string NOMEGRUPO { get; set; }
    }

    public class EstoqueResponse
    {
        public int CODESTOQUE { get; set; }
        public string NOMEESTOQUE { get; set; }
    }
}