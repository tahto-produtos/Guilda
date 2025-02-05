using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiC.Class
{
    public class ModelsEx
    {
        public class monetizacaoHierarquia
        {
            public int id { get; set; }

            public DateTime date { get; set; }

            public int Monetizacao { get; set; }

            public int idIndicador { get; set; }

            public string sector { get; set; }
        }

        public class homeRel
        {
            public string datePay { get; set; }
            public string dateReferer { get; set; }
            public bool reincidencia { get; set; }
            public string mes { get; set; }
            public string ano { get; set; }
            public string idcollaborator { get; set; }
            public string indicatorType { get; set; }
            public string name { get; set; }
            public string cargo { get; set; }
            public string data { get; set; }

            public double metaSomada { get; set; }
            public double qtdPessoasTotal { get; set; }


            public string cod_indicador { get; set; }
            public string indicador { get; set; }
            public string meta { get; set; }
            public string data_atualizacao { get; set; }
            public string cod_gip { get; set; }

            public string cod_gip_supervisor { get; set; }

            public string cod_gip_reference { get; set; }
            public string setor_supervisor { get; set; }
            public string setor { get; set; }
            public string setor_reference { get; set; }

            public string home_based { get; set; }
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
            public string fatores { get; set; }
            public double fator0 { get; set; }
            public double fator1 { get; set; }
            public double goal { get; set; }
            public string weight { get; set; }
            public string hierarchylevel { get; set; }
            public string coin1 { get; set; }
            public string coin2 { get; set; }
            public string coin3 { get; set; }
            public string coin4 { get; set; }
            public string idgda_sector { get; set; }
            public double min1 { get; set; }
            public double min2 { get; set; }
            public double min3 { get; set; }
            public double min4 { get; set; }
            public string conta { get; set; }
            public string better { get; set; }

            public string grupo { get; set; }
            public string grupoAlias { get; set; }
            public int grupoNum { get; set; }

            public double porcentual { get; set; }
            public double resultado { get; set; }
            public string diasTrabalhados { get; set; }
            public string diasEscalados { get; set; }

            public int sumDiasEscalados { get; set; }
            public int sumDiasLogados { get; set; }
            public int sumDiasLogadosEscalados { get; set; }

            public double moedasPossiveis { get; set; }
            public int moedasPossiveisConsolidado { get; set; }
            public double moedasGanhas { get; set; }
            public double MoedasExpiradas { get; set; }

            public int qtdPessoas { get; set; }
            public double resultadoAPI { get; set; }

            public double contemplados_diamante { get; set; }
            public double contemplados_ouro { get; set; }
            public double contemplados_prata { get; set; }
            public double contemplados_bronze { get; set; }

            public double contemplados_diamante_Percent { get; set; }
            public double contemplados_ouro_Percent { get; set; }
            public double contemplados_prata_Percent { get; set; }
            public double contemplados_bronze_Percent { get; set; }
            public int vemMeta { get; set; }
            public double peso {get; set;}
            public double score { get; set; }
            public double Logado { get; set; }

        }

    }
}