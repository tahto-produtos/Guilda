using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiRepositorio.Models
{
    public class RelMonetizationAdmMonth
    {
        public double min1 { get; set; }
        public double min2 { get; set; }
        public double min3 { get; set; }
        public double min4 { get; set; }
        public string conta { get; set; }
        public string better { get; set; }

        public double fator0 { get; set; }
        public double fator1 { get; set; }

        public string datePay { get; set; }
        public string dateReferer { get; set; }
        public string month { get; set; }
        public string year { get; set; }
        public string idgda_collaborators { get; set; }
        public string name { get; set; }
        public string cargo { get; set; }

        public string entradaSaida { get; set; }
        public string entrada { get; set; }
        public string saida { get; set; }
        public string expirada { get; set; }


        public string codIndicador { get; set; }
        public string indicador { get; set; }
        public string indicadorTipo { get; set; }
        public string meta { get; set; }
        public double resultado { get; set; }
        public double porcentual { get; set; }
        public string diasTrabalhados { get; set; }
        public string diasEscalados { get; set; }
        public double moedasGanhas { get; set; }
        public int moedasPossiveis { get; set; }
        public int qtdPessoas { get; set; }
        public double resultadoAPI { get; set; }
        public double meta_maxima_de_moedas_double { get; set; }
        public string grupo { get; set; }
        public string data_de_atualizacao { get; set; }
        public string cod_gip { get; set; }
        public string setor { get; set; }
        public string cod_gip_reference { get; set; }
        public string setor_reference { get; set; }
        public string cod_gip_supervisor { get; set; }
        public string setor_supervisor { get; set; }
        public string home { get; set; }
        public string site { get; set; }
        public string turno { get; set; }
        public string matricula_supervisor { get; set; }
        public string nome_supervisor { get; set; }
        public string matricula_coordenador { get; set; }
        public string nome_coordenador { get; set; }
        public string matricula_gerente_ii { get; set; }
        public string nome_gerente_ii { get; set; }
        public string matricula_gerente_i { get; set; }
        public string nome_gerente_i { get; set; }
        public string matricula_diretor { get; set; }
        public string nome_diretor { get; set; }
        public string matricula_ceo { get; set; }
        public string nome_ceo { get; set; }
        public double contemplados_diamante { get; set; }
        public double contemplados_ouro { get; set; }
        public double contemplados_prata { get; set; }
        public double contemplados_bronze { get; set; }

        public double contemplados_diamante_Percent { get; set; }
        public double contemplados_ouro_Percent { get; set; }
        public double contemplados_prata_Percent { get; set; }
        public double contemplados_bronze_Percent { get; set; }

        public string userInside { get; set; }
    }

}