using ApiRepositorio.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using static ApiRepositorio.Controllers.ReportMonthAdmController;

namespace ApiC.Class
{
    public class monetizationClass
    {
        public static RelMonetizationAdmMonth doCalculationResultDay(factors factor, RelMonetizationAdmMonth rmam)
        {
            try
            {
                factor.count = factor.count.Replace("#fator0", factor.factor0.ToString()).Replace("#fator1", factor.factor1.ToString());
                //Realiza a conta de resultado
                DataTable dt = new DataTable();
                double resultado = 0;
                try
                {
                    var result = dt.Compute(factor.count, "").ToString();
                    resultado = double.Parse(result);
                    if (resultado == double.PositiveInfinity)
                    {
                        resultado = 0;
                    }
                    if (double.IsNaN(resultado))
                    {
                        resultado = 0;
                    }
                }
                catch (Exception)
                {

                }

                double resultadoD = resultado;

                if (rmam.indicadorTipo == null)
                {
                    resultadoD = resultadoD * 100;
                }
                else if (rmam.indicadorTipo == "PERCENT")
                {
                    resultadoD = resultadoD * 100;
                }


                double atingimentoMeta = 0;
                //Verifica se é melhor ou menor melhor
                if (factor.better == "BIGGER_BETTER")
                {
                    //if (double.IsNaN(resultado))
                    //{
                    //    resultadoD = 0;
                    //}

                    if (factor.goal == 0)
                    {
                        atingimentoMeta = 0;
                    }
                    else
                    {
                        atingimentoMeta = resultadoD / factor.goal;
                    }
                }
                else
                {
                    // No caso de menor melhor, quando o denominador é 0 não esta sendo possivel fazer a conta de divisão por 0.
                    // E como é menor melhor, logo 0 é um resultado de 100% de atingimento.
                    if (resultadoD == 0) //&& factor.goal < 1
                    {
                        atingimentoMeta = 1;
                    }
                    //else if (resultadoD == 0)
                    //{

                    //    atingimentoMeta = (factor.goal / 0.01);
                    //}
                    else
                    {
                        //if (factor.cod_indicador == "2")
                        //{
                        //    atingimentoMeta = 100 - resultadoD;
                        //    //atingimentoMeta = (factor.goal / resultadoD);
                        //    atingimentoMeta = atingimentoMeta / 100;
                        //}
                        //else
                        //{
                        atingimentoMeta = (factor.goal / resultadoD);
                        //}
                    }
                }

                atingimentoMeta = atingimentoMeta * 100;



                //Verifica a qual grupo pertence
                if (atingimentoMeta >= factor.min1)
                {
                    rmam.grupo = "Diamante";
                }
                else if (atingimentoMeta >= factor.min2)
                {
                    rmam.grupo = "Ouro";
                }
                else if (atingimentoMeta >= factor.min3)
                {
                    rmam.grupo = "Prata";
                }
                else if (atingimentoMeta >= factor.min4)
                {
                    rmam.grupo = "Bronze";
                }
                else
                {
                    rmam.grupo = "Bronze";
                }
                rmam.porcentual = atingimentoMeta;
                rmam.resultado = resultadoD;
                rmam.moedasGanhas = double.Parse(factor.coins);
            }
            catch (Exception ex)
            {
                rmam.grupo = "Bronze";

            }

            return rmam;
        }

        public static RelMonetizationAdmMonth doCalculationResultDay(RelMonetizationAdmMonth rmam)
        {
            try
            {
                rmam.conta = rmam.conta.Replace("#fator0", rmam.fator0.ToString()).Replace("#fator1", rmam.fator1.ToString());
                //Realiza a conta de resultado
                DataTable dt = new DataTable();
                double resultado = 0;
                try
                {
                    var result = dt.Compute(rmam.conta, "").ToString();
                    resultado = double.Parse(result);
                    if (resultado == double.PositiveInfinity)
                    {
                        resultado = 0;
                    }
                    if (double.IsNaN(resultado))
                    {
                        resultado = 0;
                    }
                }
                catch (Exception)
                {

                }



                double resultadoD = resultado;
                if (rmam.indicadorTipo == null)
                {
                    resultadoD = resultadoD * 100;
                }
                else if (rmam.indicadorTipo == "PERCENT")
                {
                    resultadoD = resultadoD * 100;
                }

                double atingimentoMeta = 0;
                //Verifica se é melhor ou menor melhor
                if (rmam.better == "BIGGER_BETTER")
                {
                    //if (double.IsNaN(resultado))
                    //{
                    //    resultadoD = 0;
                    //}

                    if (Convert.ToDouble(rmam.meta) == 0)
                    {
                        atingimentoMeta = 0;
                    }
                    else
                    {
                        atingimentoMeta = resultadoD / Convert.ToDouble(rmam.meta);
                    }
                }
                else
                {
                    // No caso de menor melhor, quando o denominador é 0 não esta sendo possivel fazer a conta de divisão por 0.
                    // E como é menor melhor, logo 0 é um resultado de 100% de atingimento.

                    if (resultadoD == 0) //&& (Convert.ToDouble(rmam.meta) < 1)
                    {
                        atingimentoMeta = 1;
                    }
                    //else if (resultadoD == 0)
                    //{

                    //    atingimentoMeta = (Convert.ToDouble(rmam.meta) / 0.01);
                    //}
                    else
                    {
                        //if (factor.cod_indicador == "2")
                        //{
                        //    atingimentoMeta = 100 - resultadoD;
                        //    //atingimentoMeta = (factor.goal / resultadoD);
                        //    atingimentoMeta = atingimentoMeta / 100;
                        //}
                        //else
                        //{
                        atingimentoMeta = (Convert.ToDouble(rmam.meta) / resultadoD);
                        //}
                    }
                }

                atingimentoMeta = atingimentoMeta * 100;



                //Verifica a qual grupo pertence
                if (atingimentoMeta >= rmam.min1)
                {
                    rmam.grupo = "Diamante";
                }
                else if (atingimentoMeta >= rmam.min2)
                {
                    rmam.grupo = "Ouro";
                }
                else if (atingimentoMeta >= rmam.min3)
                {
                    rmam.grupo = "Prata";
                }
                else if (atingimentoMeta >= rmam.min4)
                {
                    rmam.grupo = "Bronze";
                }
                else
                {
                    rmam.grupo = "Bronze";
                }
                rmam.porcentual = atingimentoMeta;
                rmam.resultado = resultadoD;

                //Caso o tipo do indicador seja inteiro, não tera casa decimal.

                if (rmam.indicadorTipo == "INTEGER" || rmam.indicadorTipo == "HOUR")
                {
                    rmam.resultado = Math.Round(resultadoD, 0, MidpointRounding.AwayFromZero);
                }
                else
                {
                    rmam.resultado = resultadoD;
                }


            }
            catch (Exception ex)
            {
                rmam.grupo = "Bronze";

            }

            return rmam;
        }

        public static RelMonetizationAdmMonth doCalculationResult(factors factor, RelMonetizationAdmMonth rmam)
        {
            try
            {
                factor.count = factor.count.Replace("#fator0", factor.factor0.ToString()).Replace("#fator1", factor.factor1.ToString());
                //Realiza a conta de resultado
                DataTable dt = new DataTable();
                double resultado = 0;
                try
                {
                    var result = dt.Compute(factor.count, "").ToString();
                    resultado = double.Parse(result);
                    if (resultado == double.PositiveInfinity)
                    {
                        resultado = 0;
                    }
                    if (double.IsNaN(resultado))
                    {
                        resultado = 0;
                    }
                }
                catch (Exception)
                {

                }



                double resultadoD = resultado;
                if (rmam.indicadorTipo == null)
                {
                    resultadoD = resultadoD * 100;
                }
                else if (rmam.indicadorTipo == "PERCENT")
                {
                    resultadoD = resultadoD * 100;
                }

                double atingimentoMeta = 0;
                //Verifica se é melhor ou menor melhor
                if (factor.better == "BIGGER_BETTER")
                {
                    //if (double.IsNaN(resultado))
                    //{
                    //    resultadoD = 0;
                    //}

                    if (factor.goal == 0)
                    {
                        atingimentoMeta = 0;
                    }
                    else
                    {
                        atingimentoMeta = resultadoD / factor.goal;
                    }
                }
                else
                {
                    // No caso de menor melhor, quando o denominador é 0 não esta sendo possivel fazer a conta de divisão por 0.
                    // E como é menor melhor, logo 0 é um resultado de 100% de atingimento.
                    if (resultadoD == 0) // && factor.goal < 1
                    {
                        atingimentoMeta = 1;
                    }
                    //else if (resultadoD == 0)
                    //{

                    //    atingimentoMeta = (factor.goal / 0.01);
                    //}
                    else
                    {
                        //if (factor.cod_indicador == "2")
                        //{
                        //    atingimentoMeta = 100 - resultadoD;
                        //    //atingimentoMeta = (factor.goal / resultadoD);
                        //    atingimentoMeta = atingimentoMeta / 100;
                        //}
                        //else
                        //{
                        atingimentoMeta = (factor.goal / resultadoD);
                        //}
                    }
                }

                atingimentoMeta = atingimentoMeta * 100;



                //Verifica a qual grupo pertence
                if (atingimentoMeta >= factor.min1)
                {
                    rmam.grupo = "Diamante";
                }
                else if (atingimentoMeta >= factor.min2)
                {
                    rmam.grupo = "Ouro";
                }
                else if (atingimentoMeta >= factor.min3)
                {
                    rmam.grupo = "Prata";
                }
                else if (atingimentoMeta >= factor.min4)
                {
                    rmam.grupo = "Bronze";
                }
                else
                {
                    rmam.grupo = "Bronze";
                }
                rmam.porcentual = atingimentoMeta;
                rmam.resultado = resultadoD;
                rmam.moedasGanhas = double.Parse(factor.coins);
            }
            catch (Exception ex)
            {

                rmam.grupo = "Bronze";
            }

            return rmam;
        }


        public static ModelsEx.homeRel doCalculationResultHome(ModelsEx.homeRel factor, bool ConsolidadoSetor)
        {
            try
            {
                double resultado = 0;

                string contaAg = "";
                if (factor.cod_indicador == "10000013")
                {
                    //Calculo do Agente
                    contaAg = $"{factor.sumDiasLogadosEscalados} / {factor.sumDiasEscalados}";

                }
                else if(factor.cod_indicador == "10000014")
                {
                    // Calculo do Agente
                    contaAg = $"{factor.sumDiasLogados} / {factor.sumDiasEscalados}";
                }
                else
                {
                    contaAg = factor.conta.Replace("#fator0", factor.fator0.ToString()).Replace("#fator1", factor.fator1.ToString());
                    //Realiza a conta de resultado
                }

                DataTable dt = new DataTable();

                try
                {
                    var result = dt.Compute(contaAg, "").ToString();
                    resultado = double.Parse(result);
                    if (resultado == double.PositiveInfinity)
                    {
                        resultado = 0;
                    }
                    if (double.IsNaN(resultado))
                    {
                        resultado = 0;
                    }
                }
                catch (Exception)
                {

                }
                //Para hierarquia, a meta será o consolidado
                if (factor.cargo != "AGENTE")
                {
                    if (factor.cargo == "CEO")
                    {
                        var teste = true;
                    }


                    if (factor.metaSomada == 0)
                    {
                        factor.goal = 0;
                        factor.meta = "0";
                    }
                    else if (factor.qtdPessoasTotal > 0)
                    {
                        factor.goal = factor.metaSomada / factor.qtdPessoasTotal;
                        factor.goal = Math.Round(factor.goal, 2, MidpointRounding.AwayFromZero);
                        factor.meta = factor.goal.ToString();
                    }

                    if (factor.moedasGanhas == 0)
                    {
                        factor.moedasGanhas = 0;
                    }
                    else
                    {
                        if (ConsolidadoSetor == true)
                        {
                            factor.moedasGanhas = factor.moedasGanhas;

                            //factor.score = factor.score;
                        }
                        else
                        {
                            factor.moedasGanhas = factor.moedasGanhas / factor.qtdPessoasTotal;
                            factor.moedasGanhas = Math.Round(factor.moedasGanhas, 0, MidpointRounding.AwayFromZero);


                        }

                    }


                    //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                    //if (factor.min1 == 0)
                    //{
                    //    factor.min1 = 0;
                    //}
                    //else
                    //{
                    //    factor.min1 = factor.min1 / factor.qtdPessoasTotal;
                    //    factor.min1 = Math.Round(factor.min1, 2, MidpointRounding.AwayFromZero);
                    //}

                    //if (factor.min2 == 0)
                    //{
                    //    factor.min2 = 0;
                    //}
                    //else
                    //{
                    //    factor.min2 = factor.min2 / factor.qtdPessoasTotal;
                    //    factor.min2 = Math.Round(factor.min2, 2, MidpointRounding.AwayFromZero);
                    //}

                    //if (factor.min3 == 0)
                    //{
                    //    factor.min3 = 0;
                    //}
                    //else
                    //{
                    //    factor.min3 = factor.min3 / factor.qtdPessoasTotal;
                    //    factor.min3 = Math.Round(factor.min3, 2, MidpointRounding.AwayFromZero);
                    //}

                    //if (factor.min4 == 0)
                    //{
                    //    factor.min4 = 0;
                    //}
                    //else
                    //{
                    //    factor.min4 = factor.min4 / factor.qtdPessoasTotal;
                    //    factor.min4 = Math.Round(factor.min4, 2, MidpointRounding.AwayFromZero);
                    //}

                }


                if (double.IsNaN(resultado))
                {
                    factor.grupo = "Bronze";
                    factor.grupoAlias = "G4";
                    factor.porcentual = 0;
                    factor.resultado = 0;
                    factor.grupoNum = 4;
                    return factor;
                }


                //Regra do TMA. Quando o resultado for 0, não monetizar e considerar bronze
                if (resultado == 0 && (factor.cod_indicador == "191" || factor.cod_indicador == "371" || factor.cod_indicador == "193"))
                {
                    factor.grupo = "Bronze";
                    factor.grupoAlias = "G4";
                    factor.porcentual = 0;
                    factor.resultado = 0;
                    factor.grupoNum = 4;
                    if (factor.vemMeta == 0)
                    {
                        factor.meta = "-";
                    }

                    return factor;
                }
                //Regra do TMA. Quando o arredondamento tambem der 0, não monetizar e considerar bronze
                double arredondResult = Math.Round(resultado, 0, MidpointRounding.AwayFromZero);
                if (arredondResult == 0 && (factor.cod_indicador == "191" || factor.cod_indicador == "371" || factor.cod_indicador == "193"))
                {
                    factor.grupo = "Bronze";
                    factor.grupoAlias = "G4";
                    factor.porcentual = 0;
                    factor.resultado = 0;
                    factor.grupoNum = 4;
                    if (factor.vemMeta == 0)
                    {
                        factor.meta = "-";
                    }

                    return factor;
                }



                double resultadoD = resultado;

                if (factor.indicatorType == null)
                {
                    resultadoD = resultadoD * 100;
                }
                else if (factor.indicatorType == "PERCENT")
                {
                    resultadoD = resultadoD * 100;
                }

                //CASOS QUE NÃO VEM META NO PERIODO SELECIONADO.
                if (factor.vemMeta == 0)
                {
                    factor.grupo = "-";
                    factor.meta = "-";
                    //return factor;
                }
                else
                {


                    double atingimentoMeta = 0;
                    //Verifica se é melhor ou menor melhor
                    if (factor.better == "BIGGER_BETTER")
                    {
                        if (factor.goal == 0)
                        {
                            atingimentoMeta = 0;
                        }
                        else
                        {
                            atingimentoMeta = resultadoD / factor.goal;
                        }
                    }
                    else
                    {
                        // No caso de menor melhor, quando o denominador é 0 não esta sendo possivel fazer a conta de divisão por 0.
                        // E como é menor melhor, logo 0 é um resultado de 100% de atingimento.
                        if (resultadoD == 0) // && factor.goal < 1
                        {
                            atingimentoMeta = 10;
                        }
                        //else if ( resultadoD == 0 )
                        //{

                        //    atingimentoMeta = (factor.goal / 0.01);
                        //}
                        else
                        {
                            //if (factor.cod_indicador == "2")
                            //{
                            //    atingimentoMeta = 100 - resultadoD;
                            //    //atingimentoMeta = (factor.goal / resultadoD);
                            //    atingimentoMeta = atingimentoMeta / 100;
                            //}
                            //else
                            //{
                            atingimentoMeta = (factor.goal / resultadoD);
                            //}
                        }
                    }

                    atingimentoMeta = atingimentoMeta * 100;


                    //SCORE

                    factor.score = Math.Round(atingimentoMeta * factor.peso, 2, MidpointRounding.AwayFromZero);


                    //Verifica a qual grupo pertence

                    if (atingimentoMeta >= factor.min1)
                    {
                        factor.grupoNum = 1;
                        factor.grupoAlias = "G1";
                        factor.grupo = "Diamante";
                    }
                    else if (atingimentoMeta >= factor.min2)
                    {
                        factor.grupoNum = 2;
                        factor.grupoAlias = "G2";
                        factor.grupo = "Ouro";
                    }
                    else if (atingimentoMeta >= factor.min3)
                    {
                        factor.grupoNum = 3;
                        factor.grupoAlias = "G3";
                        factor.grupo = "Prata";
                    }
                    else if (atingimentoMeta >= factor.min4)
                    {
                        factor.grupoNum = 4;
                        factor.grupoAlias = "G4";
                        factor.grupo = "Bronze";
                    }
                    else
                    {
                        factor.grupoNum = 4;
                        factor.grupoAlias = "G4";
                        factor.grupo = "Bronze";
                    }
                    factor.porcentual = atingimentoMeta;

                }
                //Caso o tipo do indicador seja inteiro, não tera casa decimal.

                if (factor.indicatorType == "INTEGER" || factor.indicatorType == "HOUR")
                {
                    factor.resultado = Math.Round(resultadoD, 0, MidpointRounding.AwayFromZero);
                }
                else
                {
                    factor.resultado = resultadoD;
                }

                //factor.score = (factor.porcentual / factor.score) * 100;
            }
            catch (Exception ex)
            {
                factor.grupo = "Bronze";
                factor.grupoAlias = "G4";
                factor.grupoNum = 4;
            }

            return factor;
        }
    }

    public class factors
    {
        public double factor0 { get; set; }
        public double factor1 { get; set; }
        public double goal { get; set; }
        public double min1 { get; set; }
        public double min2 { get; set; }
        public double min3 { get; set; }
        public double min4 { get; set; }
        public string count { get; set; }
        public string better { get; set; }
        public string coins { get; set; }

    }
}