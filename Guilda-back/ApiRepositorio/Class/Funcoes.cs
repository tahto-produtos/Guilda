using ApiRepositorio.Controllers;
using ApiRepositorio;
using ApiRepositorio.Models;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using static ApiRepositorio.Controllers.SendNotificationController;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Xml.Schema;

namespace ApiC.Class
{
    public class Funcoes
    {
        public static string VerificaIndicador(int idIndicator)
        {
            string retorno = "";
            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT NAME FROM GDA_INDICATOR NOLOCK WHERE IDGDA_INDICATOR = {idIndicator}  ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                retorno = reader["NAME"].ToString();
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    connection.Close();
                    throw;
                }
                connection.Close();
            }

            return retorno;

        }

        public static string VerificaNomeAgente(int idPersonaUser)
        {
            string retorno = "";
            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT NAME FROM GDA_PERSONA_USER NOLOCK WHERE IDGDA_PERSONA_USER = {idPersonaUser} ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                retorno = reader["NAME"].ToString();
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    connection.Close();
                    throw;
                }
                connection.Close();
            }

            return retorno;

        }

        public static string VerificaSetor(int idSector)
        {
            string retorno = "";
            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT NAME FROM GDA_SECTOR NOLOCK WHERE IDGDA_SECTOR = {idSector}  ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                retorno = reader["NAME"].ToString();
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    connection.Close();
                    throw;
                }
                connection.Close();
            }

            return retorno;

        }

        public static List<PersonaNotification> ReturnColaboradoresAction(int HierarchyID, int SectorId, int subSector)
        {
            List<PersonaNotification> listPersona = new List<PersonaNotification>();
            string filter = " ";
            if (SectorId != 0)
            {
                filter = filter + $"AND IDGDA_SECTOR = {SectorId}  ";
            }
            else if (subSector != 0)
            {
                filter = filter + $"AND IDGDA_SUBSECTOR = {subSector}  ";
            }

            List<int> col = new List<int>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT DISTINCT PU.IDGDA_PERSONA_USER FROM GDA_COLLABORATORS_DETAILS (NOLOCK) CD  ");
            sb.Append("LEFT JOIN GDA_PERIOD (NOLOCK) P ON P.PERIOD = CD.PERIODO  ");
            sb.Append("LEFT JOIN GDA_HOMEFLOOR (NOLOCK) HF ON HF.HOMEFLOOR = CD.HOME_BASED ");
            sb.Append("LEFT JOIN GDA_CLIENT (NOLOCK) C ON C.IDGDA_CLIENT = CD.IDGDA_CLIENT ");
            sb.Append("LEFT JOIN GDA_HIERARCHY (NOLOCK) H ON H.LEVELNAME = CD.CARGO  ");
            sb.Append("INNER JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) PCU ON PCU.IDGDA_COLLABORATORS = CD.IDGDA_COLLABORATORS ");
            sb.Append("INNER JOIN GDA_PERSONA_USER (NOLOCK) PU ON PU.IDGDA_PERSONA_USER = PCU.IDGDA_PERSONA_USER AND PU.IDGDA_PERSONA_USER_TYPE = 1 AND PU.DELETED_AT IS NULL  ");
            sb.Append("WHERE 1=1 ");
            sb.Append("AND CONVERT(DATE,CD.CREATED_AT) = CONVERT(DATE,GETDATE()-1) ");
            sb.Append($"AND IDGDA_HIERARCHY = {HierarchyID} ");
            sb.Append($"{filter} ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PersonaNotification userid = new PersonaNotification();
                                userid.idUserReceived = Convert.ToInt32(reader["IDGDA_PERSONA_USER"]);
                                listPersona.Add(userid);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    connection.Close();
                    throw;
                }
                connection.Close();
            }

            return listPersona;
        }

        public static int retornaCargoAtual(string dtInicio, string CollaboratorId, bool Thread = false)
        {
            int retorno = 0;
            CollaboratorId = CollaboratorId == "" ? "756399" : CollaboratorId;
            StringBuilder stb = new StringBuilder();
            stb.Append("SELECT TOP 1 IDGDA_HIERARCHY, LEVELNAME FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK)");
            stb.AppendFormat("WHERE DATE > '{0}' AND IDGDA_COLLABORATORS = {1}", dtInicio, CollaboratorId);
            stb.Append("ORDER BY DATE DESC");


            using (SqlConnection connection = new SqlConnection(Database.retornaConn(Thread)))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.CommandTimeout = 60;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ModelsEx.homeRel rmam = new ModelsEx.homeRel();

                                retorno = int.Parse(reader["IDGDA_HIERARCHY"].ToString());
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    connection.Close();
                    throw;
                }
                connection.Close();
            }

            return retorno;
        }
        public static List<string> retornaColaboradoresGeral(string data, string id, bool Thread = false)
        {
            List<string> str = new List<string>();


            StringBuilder stb = new StringBuilder();
            stb.Append("SELECT DISTINCT(IDGDA_COLLABORATORS) AS COLLA FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK) ");
            stb.Append("WHERE DATE > CONVERT(DATE, DATEADD(DAY, -2, GETDATE())) AND LEVELNAME = 'AGENTE' ");



            using (SqlConnection connection = new SqlConnection(Database.retornaConn(Thread)))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.CommandTimeout = 60;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                str.Add(reader["COLLA"].ToString());
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    connection.Close();
                    throw;
                }
                connection.Close();
            }

            return str;
        }
        public static List<string> retornaColaboradoresAbaixo(string data, string id, bool Thread = false)
        {
            List<string> str = new List<string>();

            int retorno = 0;
            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}'; ", id);
            stb.Append(" ");
            stb.Append("SELECT DISTINCT(CLD.IDGDA_COLLABORATORS) ");
            stb.Append("FROM GDA_COLLABORATORS_LAST_DETAILS (NOLOCK) CLD ");
            stb.Append("WHERE (IDGDA_COLLABORATORS = @INPUTID OR  ");
            stb.Append("	    MATRICULA_SUPERVISOR = @INPUTID OR ");
            stb.Append("		MATRICULA_COORDENADOR = @INPUTID OR ");
            stb.Append("		MATRICULA_GERENTE_II = @INPUTID OR ");
            stb.Append("		MATRICULA_GERENTE_I = @INPUTID OR ");
            stb.Append("		MATRICULA_DIRETOR = @INPUTID OR ");
            stb.Append("		MATRICULA_CEO = @INPUTID) ");
            stb.Append("ORDER BY CLD.IDGDA_COLLABORATORS ");

            //stb.AppendFormat("DECLARE @DATEENV DATE; SET @DATEENV = '{0}'; ", data);
            //stb.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}'; ", id);
            //stb.Append(" ");
            //stb.Append("WITH HIERARCHYCTE AS  ");
            //stb.Append("  (SELECT IDGDA_HISTORY_HIERARCHY_RELATIONSHIP,  ");
            //stb.Append("          CONTRACTORCONTROLID,  ");
            //stb.Append("          PARENTIDENTIFICATION,  ");
            //stb.Append("          IDGDA_COLLABORATORS,  ");
            //stb.Append("          IDGDA_HIERARCHY,  ");
            //stb.Append("          CREATED_AT,  ");
            //stb.Append("          DELETED_AT,  ");
            //stb.Append("          TRANSACTIONID,  ");
            //stb.Append("          LEVELWEIGHT, DATE, LEVELNAME  ");
            //stb.Append("   FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP  ");
            //stb.Append("   WHERE IDGDA_COLLABORATORS = @INPUTID  ");
            //stb.Append("     AND CONVERT(DATE, [DATE]) = @DATEENV  ");
            //stb.Append("   UNION ALL SELECT H.IDGDA_HISTORY_HIERARCHY_RELATIONSHIP,  ");
            //stb.Append("                    H.CONTRACTORCONTROLID,  ");
            //stb.Append("                    H.PARENTIDENTIFICATION,  ");
            //stb.Append("                    H.IDGDA_COLLABORATORS,  ");
            //stb.Append("                    H.IDGDA_HIERARCHY,  ");
            //stb.Append("                    H.CREATED_AT,  ");
            //stb.Append("                    H.DELETED_AT,  ");
            //stb.Append("                    H.TRANSACTIONID,  ");
            //stb.Append("                    H.LEVELWEIGHT,  ");
            //stb.Append("                    H.DATE,  ");
            //stb.Append("                    H.LEVELNAME  ");
            //stb.Append("   FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP H  ");
            //stb.Append("   INNER JOIN HIERARCHYCTE CTE ON H.PARENTIDENTIFICATION = CTE.IDGDA_COLLABORATORS  ");
            //stb.Append("   WHERE CTE.LEVELNAME <> 'AGENTE'  ");
            //stb.Append("     AND CONVERT(DATE, CTE.[DATE]) = @DATEENV ) ");
            //stb.Append(" ");
            //stb.Append("	 SELECT DISTINCT(IDGDA_COLLABORATORS), LEVELNAME ");
            //stb.Append("     FROM HIERARCHYCTE  ");
            //stb.Append("     WHERE CONVERT(DATE, DATE) = @DATEENV ");


            using (SqlConnection connection = new SqlConnection(Database.retornaConn(Thread)))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.CommandTimeout = 60;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                str.Add(reader["IDGDA_COLLABORATORS"].ToString());
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    connection.Close();
                    throw;
                }
                connection.Close();
            }

            return str;
        }

        public static string RemoverZerosAposPonto(string numero)
        {
            string[] partes = numero.Split('.');
            if (partes.Length > 1)
            {
                string parteDecimal = partes[1].TrimEnd('0');
                if (parteDecimal.Length == 0)
                    return partes[0];
                else
                    return partes[0] + "." + parteDecimal;
            }
            else
            {
                return numero;
            }
        }

        public static List<DateTime> GetWeekDates(DateTime currentDate)
        {
            List<DateTime> weekDates = new List<DateTime>();
            CultureInfo culture = new CultureInfo("pt-BR");

            // Obtém o primeiro dia da semana (segunda-feira)
            DayOfWeek firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;
            DateTime startDate = currentDate.AddDays(-(currentDate.DayOfWeek - firstDayOfWeek));

            // Adiciona as datas da semana à lista
            for (int i = 0; i < 7; i++)
            {
                weekDates.Add(startDate.AddDays(i));
            }

            return weekDates;
        }

        public static string RetornoDiaSemana(string day)
        {
            if (day == "Monday")
            {
                day = "Segunda-Feira";
            }
            else if (day == "Tuesday")
            {
                day = "Terça-Feira";
            }
            else if (day == "Wednesday")
            {
                day = "Quarta-Feira";
            }
            else if (day == "Thursday")
            {
                day = "Quinta-Feira";
            }
            else if (day == "Friday")
            {
                day = "Sexta-Feira";
            }
            else if (day == "Saturday")
            {
                day = "Sabado";
            }
            else if (day == "Sunday")
            {
                day = "Domingo";
            }

            return day;

        }

        public static permit returnPermitByActionResource(string action, string resource, string idCollaborator)
        {
            permit rmams = new permit();

            StringBuilder stb = new StringBuilder();
            stb.Append($"SELECT  ");
            stb.Append($"	P.ID,  ");
            stb.Append($"	ACTION,  ");
            stb.Append($"	RESOURCE,  ");
            stb.Append($"	CASE WHEN PP.ACTIVE IS NULL THEN 'FALSE' WHEN PP.ACTIVE = 1 THEN 'TRUE' ELSE 'FALSE' END AS ACTIVE  ");
            stb.Append($"FROM GDA_PERMIT (NOLOCK) AS P  ");
            stb.Append($"INNER JOIN GDA_COLLABORATORS (NOLOCK) AS C ON IDGDA_COLLABORATORS = {idCollaborator} ");
            stb.Append($"LEFT JOIN GDA_PROFILE_PERMIT (NOLOCK) AS PP ON PP.PROFILE_ID = C.PROFILE_COLLABORATOR_ADMINISTRATIONID AND P.ID = PP.PERMISSION_ID  ");
            stb.Append($"WHERE P.DELETED_AT IS NULL AND P.ACTION = '{action}' AND P.RESOURCE = '{resource}' ");



            //stb.AppendFormat(" {0} ", orderBy);

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                {
                    command.CommandTimeout = 60;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            rmams.id = Convert.ToInt32(reader["ID"].ToString());
                            rmams.action = reader["ACTION"].ToString();
                            rmams.resource = reader["RESOURCE"].ToString();
                            rmams.active = Convert.ToBoolean(reader["ACTIVE"].ToString());

                        }
                    }
                }
                connection.Close();
            }
            return rmams;
        }


        public static List<permit> retornaPermissaoColaboradorPerfil(string collaboratorID, string perfilID)
        {
            List<permit> rmams = new List<permit>();

            StringBuilder stb = new StringBuilder();
            if (collaboratorID != "")
            {
                stb.Append("SELECT ");
                stb.Append("	P.ID, ");
                stb.Append("	ACTION, ");
                stb.Append("	RESOURCE, ");
                stb.Append("	CASE WHEN PP.ACTIVE IS NULL THEN 'FALSE' WHEN PP.ACTIVE = 1 THEN 'TRUE' ELSE 'FALSE' END AS ACTIVE ");
                stb.Append("FROM GDA_PERMIT (NOLOCK) AS P ");
                stb.AppendFormat("INNER JOIN GDA_COLLABORATORS (NOLOCK) AS C ON IDGDA_COLLABORATORS = {0} ", collaboratorID);
                stb.Append("LEFT JOIN GDA_PROFILE_PERMIT (NOLOCK) AS PP ON PP.PROFILE_ID = C.PROFILE_COLLABORATOR_ADMINISTRATIONID AND P.ID = PP.PERMISSION_ID ");
                stb.Append("WHERE P.DELETED_AT IS NULL ");

            }
            else if (perfilID != "")
            {
                stb.Append("SELECT  ");
                stb.Append("	P.ID,  ");
                stb.Append("	ACTION,  ");
                stb.Append("	RESOURCE,  ");
                stb.Append("	CASE WHEN PP.ACTIVE IS NULL THEN 'FALSE' WHEN PP.ACTIVE = 1 THEN 'TRUE' ELSE 'FALSE' END AS ACTIVE ");
                stb.Append("FROM GDA_PERMIT (NOLOCK) AS P ");
                stb.AppendFormat("LEFT JOIN GDA_PROFILE_PERMIT (NOLOCK) AS PP ON P.ID = PP.PERMISSION_ID AND PP.PROFILE_ID = {0} ", perfilID);
                stb.Append("WHERE P.DELETED_AT IS NULL ");
            }

            //stb.AppendFormat(" {0} ", orderBy);

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                {
                    command.CommandTimeout = 60;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            permit rmam = new permit();
                            rmam.id = Convert.ToInt32(reader["ID"].ToString());
                            rmam.action = reader["ACTION"].ToString();
                            rmam.resource = reader["RESOURCE"].ToString();
                            rmam.active = Convert.ToBoolean(reader["ACTIVE"].ToString());

                            rmams.Add(rmam);
                        }
                    }
                }
                connection.Close();
            }
            return rmams;
        }

        public static string retornaIdsSectorsGroup(string sectorsGroup, bool Thread = false)
        {
            List<int> idsSector = new List<int>();

            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("SELECT * FROM GDA_SECTOR (NOLOCK) ");
            stb.AppendFormat("WHERE DELETED_AT IS NULL  ");
            stb.AppendFormat($"AND NAME IN ({sectorsGroup}) ");

            //stb.AppendFormat(" {0} ", orderBy);

            using (SqlConnection connection = new SqlConnection(Database.retornaConn(Thread)))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                {
                    command.CommandTimeout = 60;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            idsSector.Add(Convert.ToInt32(reader["IDGDA_SECTOR"].ToString()));
                        }
                    }
                }
                connection.Close();
            }
            return string.Join(",", idsSector);

        }

        public static bool retornaPermissao(string collaboratorID, bool Thread = false, bool checkFlag = true)
        {
            bool retorno = false;

            StringBuilder stb = new StringBuilder();
            stb.Append("SELECT * FROM GDA_COLLABORATORS (NOLOCK) AS C ");
            stb.Append("INNER JOIN GDA_PROFILE_COLLABORATOR_ADMINISTRATION (NOLOCK) AS A ON C.PROFILE_COLLABORATOR_ADMINISTRATIONID = A.ID ");
            stb.Append("INNER JOIN GDA_PROFILE_PERMIT (NOLOCK) AS P ON P.profile_id = A.ID AND P.PERMISSION_ID = 2 AND P.ACTIVE = 1 ");
            stb.AppendFormat("WHERE IDGDA_COLLABORATORS = '{0}' ", collaboratorID);

            //stb.AppendFormat(" {0} ", orderBy);

            using (SqlConnection connection = new SqlConnection(Database.retornaConn(Thread)))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                {
                    command.CommandTimeout = 60;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            
                            if (checkFlag == true)
                            {
                                var personaVision = reader["PERSONAL_VISION"] != DBNull.Value ? reader["PERSONAL_VISION"].ToString() : "0";


                                if (personaVision == "1")
                                {
                                    retorno = false;
                                }
                                else
                                {
                                    retorno = true;
                                }
                            }
                            else
                            {
                                retorno = true;
                            }                          
                        }
                    }
                }
                connection.Close();
            }
            return retorno;

        }

        public static List<ModelsEx.monetizacaoHierarquia> retornaListaMonetizacaoHierarquia(string dtInicial, string dtFinal, bool Thread = false)
        {
            List<ModelsEx.monetizacaoHierarquia> lMone = new List<ModelsEx.monetizacaoHierarquia>();

            try
            {

                StringBuilder stb = new StringBuilder();
                stb.Append("SELECT RESULT_DATE, COLLABORATOR_ID, GDA_INDICATOR_IDGDA_INDICATOR, SUM(INPUT) AS MOENTIZA, IDGDA_SECTOR FROM GDA_CHECKING_ACCOUNT (NOLOCK) ");
                stb.Append("WHERE GDA_INDICATOR_IDGDA_INDICATOR IS NOT NULL ");
                stb.AppendFormat("AND CONVERT(DATE, RESULT_DATE) >= '{0}' AND CONVERT(DATE,RESULT_DATE) <= '{1}' ", dtInicial, dtFinal);
                stb.Append("GROUP BY RESULT_DATE, COLLABORATOR_ID, GDA_INDICATOR_IDGDA_INDICATOR, IDGDA_SECTOR ");


                //stb.AppendFormat(" {0} ", orderBy);

                using (SqlConnection connection = new SqlConnection(Database.retornaConn(Thread)))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.CommandTimeout = 60;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ModelsEx.monetizacaoHierarquia mh = new ModelsEx.monetizacaoHierarquia();
                                mh.id = int.Parse(reader["COLLABORATOR_ID"].ToString());
                                mh.date = DateTime.Parse(reader["RESULT_DATE"].ToString());
                                mh.Monetizacao = int.Parse(reader["MOENTIZA"].ToString());
                                mh.idIndicador = int.Parse(reader["GDA_INDICATOR_IDGDA_INDICATOR"].ToString());
                                mh.sector = reader["IDGDA_SECTOR"].ToString();
                                lMone.Add(mh);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception)
            {

                throw;
            }


            return lMone;
        }

        public static List<ModelsEx.homeRel> RetornaIndicadorAcesso(List<ModelsEx.homeRel> rmams, bool agrupar)
        {
            List<ModelsEx.homeRel> IndicadorAcesso = new List<ModelsEx.homeRel>();
            List<ModelsEx.homeRel> NovoIndicadorAcesso = new List<ModelsEx.homeRel>();
            if (agrupar == true)
            {
                IndicadorAcesso = rmams.GroupBy(item => new { item.data, item.idcollaborator })
                   .Select(grupo => new ModelsEx.homeRel
                   {
                       mes = grupo.First().mes,
                       ano = grupo.First().ano,
                       datePay = grupo.First().datePay,
                       dateReferer = grupo.First().dateReferer,
                       indicatorType = "PERCENT",
                       indicador = "Indicador de Acesso",
                       cod_indicador = "-2",
                       meta = "-",
                       data_atualizacao = grupo.First().data_atualizacao,
                       cod_gip = grupo.First().cod_gip,
                       setor = grupo.First().setor,
                       home_based = grupo.First().home_based,
                       site = grupo.First().site,
                       turno = grupo.First().turno,
                       goal = grupo.First().goal,
                       weight = grupo.First().weight,
                       hierarchylevel = grupo.First().hierarchylevel,
                       coin1 = grupo.First().coin1,
                       coin2 = grupo.First().coin2,
                       coin3 = grupo.First().coin3,
                       coin4 = grupo.First().coin4,
                       idgda_sector = grupo.First().idgda_sector,
                       min1 = 0,
                       min2 = 0,
                       min3 = 0,
                       min4 = 0,
                       conta = grupo.First().conta,
                       better = grupo.First().better,
                       idcollaborator = grupo.First().idcollaborator,
                       name = grupo.First().name,
                       cargo = grupo.First().cargo,
                       nome_supervisor = grupo.First().nome_supervisor,
                       matricula_supervisor = grupo.First().matricula_supervisor,
                       matricula_coordenador = grupo.First().matricula_coordenador,
                       nome_coordenador = grupo.First().nome_coordenador,
                       matricula_gerente_ii = grupo.First().matricula_gerente_ii,
                       nome_gerente_ii = grupo.First().nome_gerente_ii,
                       matricula_gerente_i = grupo.First().matricula_gerente_i,
                       nome_gerente_i = grupo.First().nome_gerente_i,
                       matricula_diretor = grupo.First().matricula_diretor,
                       nome_diretor = grupo.First().nome_diretor,
                       matricula_ceo = grupo.First().matricula_ceo,
                       nome_ceo = grupo.First().nome_ceo,
                       data = grupo.Key.data,
                       fator0 = grupo.Sum(item => item.fator0),
                       fator1 = grupo.Sum(item => item.fator1),
                       diasTrabalhados = grupo.Max(item => item.diasTrabalhados),
                       diasEscalados = grupo.Max(item => item.diasEscalados),
                       Logado = grupo.Max(item => item.Logado),
                       moedasPossiveis = 0,
                       moedasGanhas = 0,
                       qtdPessoas = grupo.Count(item2 => item2.resultado > 0),
                       resultadoAPI = grupo.Sum(item => item.resultado),
                   })
                   .ToList();
            }
            foreach (ModelsEx.homeRel cc in IndicadorAcesso)
            {
                ModelsEx.homeRel hr = doCalculationIndicatorLogin(cc);

                NovoIndicadorAcesso.Add(hr);
            }
            return NovoIndicadorAcesso;
        }
        public static List<ModelsEx.homeRel> retornaCestaIndicadores(List<ModelsEx.homeRel> rmams, cestaMetrica cm, bool agrupar, bool consolidarColaborador, bool consolidarSetor)
        {
            List<ModelsEx.homeRel> cesta = new List<ModelsEx.homeRel>();
            List<ModelsEx.homeRel> NovaCesta = new List<ModelsEx.homeRel>();


            List<ModelsEx.homeRel> teste = rmams.FindAll(s => s.cargo == "CEO").ToList();

            if (agrupar == true)
            {
                cesta = rmams.GroupBy(item => new { item.data, item.idcollaborator })
                   .Select(grupo => new ModelsEx.homeRel
                   {
                       mes = grupo.First().mes,
                       ano = grupo.First().ano,

                       datePay = grupo.First().datePay,
                       dateReferer = grupo.First().dateReferer,

                       indicatorType = "INTEGER",
                       indicador = "Cesta de Indicadores",
                       cod_indicador = "10000012",
                       meta = grupo.First().meta,
                       data_atualizacao = grupo.First().data_atualizacao,
                       cod_gip = grupo.First().cod_gip,
                       setor = grupo.First().setor,
                       setor_reference = grupo.First().setor_reference,
                       cod_gip_reference = grupo.First().cod_gip_reference,
                       home_based = grupo.First().home_based,
                       site = grupo.First().site,
                       turno = grupo.First().turno,

                       goal = grupo.First().goal,
                       weight = grupo.First().weight,
                       hierarchylevel = grupo.First().hierarchylevel,
                       coin1 = grupo.First().coin1,
                       coin2 = grupo.First().coin2,
                       coin3 = grupo.First().coin3,
                       coin4 = grupo.First().coin4,
                       idgda_sector = grupo.First().idgda_sector,
                       min1 = cm.min1,
                       min2 = cm.min2,
                       min3 = cm.min3,
                       min4 = cm.min4,
                       conta = grupo.First().conta,
                       better = grupo.First().better,

                       idcollaborator = grupo.First().idcollaborator,
                       name = grupo.First().name,
                       cargo = grupo.First().cargo,
                       nome_supervisor = grupo.First().nome_supervisor,
                       matricula_supervisor = grupo.First().matricula_supervisor,
                       matricula_coordenador = grupo.First().matricula_coordenador,
                       nome_coordenador = grupo.First().nome_coordenador,
                       matricula_gerente_ii = grupo.First().matricula_gerente_ii,
                       nome_gerente_ii = grupo.First().nome_gerente_ii,
                       matricula_gerente_i = grupo.First().matricula_gerente_i,
                       nome_gerente_i = grupo.First().nome_gerente_i,
                       matricula_diretor = grupo.First().matricula_diretor,
                       nome_diretor = grupo.First().nome_diretor,
                       matricula_ceo = grupo.First().matricula_ceo,
                       nome_ceo = grupo.First().nome_ceo,

                       data = grupo.Key.data,

                       fator0 = grupo.Sum(item => item.fator0),
                       fator1 = grupo.Sum(item => item.fator1),

                       diasTrabalhados = grupo.Max(item => item.diasTrabalhados),
                       diasEscalados = grupo.Max(item => item.diasEscalados),
                       //moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis), 2, MidpointRounding.AwayFromZero),


                       moedasPossiveis = grupo.Max(item => item.diasTrabalhados) == "1"
                        ? Math.Round(grupo.Sum(item => item.moedasPossiveis), 2, MidpointRounding.AwayFromZero)
                        : grupo.Max(item => item.diasEscalados) == "0" && grupo.Max(item => item.diasTrabalhados) == "1" ?
                        Math.Round(grupo.Sum(item => item.moedasPossiveis), 2, MidpointRounding.AwayFromZero) :
                        grupo.Max(item => item.diasEscalados) == "0" && grupo.Max(item => item.diasTrabalhados) == "0" ?
                        0 :
                        grupo.Sum(item => item.moedasGanhas) > 0 ?
                        Math.Round(grupo.Sum(item => item.moedasPossiveis), 2, MidpointRounding.AwayFromZero) :
                        (grupo.Max(item => item.diasEscalados) == "0" || grupo.Max(item => item.diasTrabalhados) == "0") && grupo.Sum(item => item.resultado) > 0 ?
                        Math.Round(grupo.Sum(item => item.moedasPossiveis), 2, MidpointRounding.AwayFromZero) :
                        grupo.Max(item => item.diasEscalados) == "1" && grupo.Max(item => item.diasTrabalhados) == "-" ?
                        Math.Round(grupo.Sum(item => item.moedasPossiveis), 2, MidpointRounding.AwayFromZero) :
                        0,



                       //moedasPossiveis = grupo.First().moedasPossiveisConsolidado != 0 ? grupo.Sum(item => item.moedasPossiveisConsolidado) : grupo.Sum(item => item.moedasPossiveis),
                       moedasGanhas = grupo.Sum(item => item.moedasGanhas),
                       MoedasExpiradas = grupo.Sum(item => item.MoedasExpiradas),
                       qtdPessoas = grupo.Count(item2 => item2.resultado > 0),
                       resultadoAPI = grupo.Sum(item => item.resultado),
                   })
                   .ToList();

                //1 -> 10
                //2 -> 5
                //3 -> 7
                teste = cesta.FindAll(s => s.idcollaborator == "756399").ToList();

                if (consolidarColaborador == true)
                {
                    cesta = cesta.GroupBy(item => new { item.idcollaborator })
                             .Select(grupo => new ModelsEx.homeRel
                             {
                                 mes = grupo.First().mes,
                                 ano = grupo.First().ano,

                                 datePay = grupo.First().datePay,
                                 dateReferer = grupo.First().dateReferer,

                                 indicatorType = "INTEGER",
                                 indicador = "Cesta de Indicadores",
                                 cod_indicador = "10000012",
                                 meta = grupo.First().meta,
                                 data_atualizacao = grupo.First().data_atualizacao,
                                 cod_gip = grupo.First().cod_gip,
                                 setor = grupo.First().setor,
                                 setor_reference = grupo.First().setor_reference,
                                 cod_gip_reference = grupo.First().cod_gip_reference,
                                 home_based = grupo.First().home_based,
                                 site = grupo.First().site,
                                 turno = grupo.First().turno,


                                 goal = grupo.First().goal,
                                 weight = grupo.First().weight,
                                 hierarchylevel = grupo.First().hierarchylevel,
                                 coin1 = grupo.First().coin1,
                                 coin2 = grupo.First().coin2,
                                 coin3 = grupo.First().coin3,
                                 coin4 = grupo.First().coin4,
                                 idgda_sector = grupo.First().idgda_sector,
                                 min1 = cm.min1,
                                 min2 = cm.min2,
                                 min3 = cm.min3,
                                 min4 = cm.min4,
                                 conta = grupo.First().conta,
                                 better = grupo.First().better,

                                 idcollaborator = grupo.First().idcollaborator,
                                 name = grupo.First().name,
                                 cargo = grupo.First().cargo,
                                 nome_supervisor = grupo.First().nome_supervisor,
                                 matricula_supervisor = grupo.First().matricula_supervisor,
                                 matricula_coordenador = grupo.First().matricula_coordenador,
                                 nome_coordenador = grupo.First().nome_coordenador,
                                 matricula_gerente_ii = grupo.First().matricula_gerente_ii,
                                 nome_gerente_ii = grupo.First().nome_gerente_ii,
                                 matricula_gerente_i = grupo.First().matricula_gerente_i,
                                 nome_gerente_i = grupo.First().nome_gerente_i,
                                 matricula_diretor = grupo.First().matricula_diretor,
                                 nome_diretor = grupo.First().nome_diretor,
                                 matricula_ceo = grupo.First().matricula_ceo,
                                 nome_ceo = grupo.First().nome_ceo,


                                 data = grupo.First().data,

                                 fator0 = grupo.Sum(item => item.fator0),
                                 fator1 = grupo.Sum(item => item.fator1),

                                 diasTrabalhados = grupo.Max(item => item.diasTrabalhados),
                                 diasEscalados = grupo.Max(item => item.diasEscalados),
                                 //moedasPossiveis = grupo.First().moedasPossiveisConsolidado != 0 ? grupo.Sum(item => item.moedasPossiveisConsolidado) : grupo.Sum(item => item.moedasPossiveis),
                                 moedasPossiveis = grupo.Sum(item => item.moedasPossiveis),
                                 //moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis), 2, MidpointRounding.AwayFromZero),
                                 //moedasGanhas = grupo.First().cargo == "AGENTE" ? grupo.Sum(item => item.moedasGanhas) : grupo.Max(item => item.moedasGanhas),
                                 moedasGanhas = grupo.Sum(item => item.moedasGanhas),
                                 MoedasExpiradas = grupo.Sum(item => item.MoedasExpiradas),
                                 qtdPessoas = grupo.Count(item2 => item2.resultado > 0),
                                 resultadoAPI = grupo.Sum(item => item.resultadoAPI),
                             })
                             .ToList();
                }
                else if (consolidarSetor == true)
                {
                    cesta = rmams.GroupBy(item => new { item.data, item.cod_gip }).Select(grupo => new ModelsEx.homeRel
                    {
                        mes = grupo.First().mes,
                        ano = grupo.First().ano,

                        datePay = grupo.First().datePay,
                        dateReferer = grupo.First().dateReferer,

                        indicatorType = "INTEGER",
                        indicador = "Cesta de Indicadores",
                        cod_indicador = "10000012",
                        meta = grupo.First().meta,
                        data_atualizacao = grupo.First().data_atualizacao,
                        cod_gip = grupo.First().cod_gip,
                        setor = grupo.First().setor,
                        setor_reference = grupo.First().setor_reference,
                        cod_gip_reference = grupo.First().cod_gip_reference,
                        home_based = grupo.First().home_based,
                        site = grupo.First().site,
                        turno = grupo.First().turno,


                        goal = grupo.First().goal,
                        weight = grupo.First().weight,
                        hierarchylevel = grupo.First().hierarchylevel,
                        coin1 = grupo.First().coin1,
                        coin2 = grupo.First().coin2,
                        coin3 = grupo.First().coin3,
                        coin4 = grupo.First().coin4,
                        idgda_sector = grupo.First().idgda_sector,
                        min1 = cm.min1,
                        min2 = cm.min2,
                        min3 = cm.min3,
                        min4 = cm.min4,
                        conta = grupo.First().conta,
                        better = grupo.First().better,

                        idcollaborator = grupo.First().idcollaborator,
                        name = grupo.First().name,
                        cargo = grupo.First().cargo,
                        nome_supervisor = grupo.First().nome_supervisor,
                        matricula_supervisor = grupo.First().matricula_supervisor,
                        matricula_coordenador = grupo.First().matricula_coordenador,
                        nome_coordenador = grupo.First().nome_coordenador,
                        matricula_gerente_ii = grupo.First().matricula_gerente_ii,
                        nome_gerente_ii = grupo.First().nome_gerente_ii,
                        matricula_gerente_i = grupo.First().matricula_gerente_i,
                        nome_gerente_i = grupo.First().nome_gerente_i,
                        matricula_diretor = grupo.First().matricula_diretor,
                        nome_diretor = grupo.First().nome_diretor,
                        matricula_ceo = grupo.First().matricula_ceo,
                        nome_ceo = grupo.First().nome_ceo,


                        data = grupo.First().data,

                        fator0 = grupo.Sum(item => item.fator0),
                        fator1 = grupo.Sum(item => item.fator1),

                        diasTrabalhados = grupo.Max(item => item.diasTrabalhados),
                        diasEscalados = grupo.Max(item => item.diasEscalados),
                        moedasPossiveis = grupo.Sum(item => item.moedasPossiveis),
                        //moedasPossiveis = Math.Round(grupo.Sum(item => item.moedasPossiveis), 2, MidpointRounding.AwayFromZero),
                        //moedasGanhas = grupo.First().cargo == "AGENTE" ? grupo.Sum(item => item.moedasGanhas) : grupo.Max(item => item.moedasGanhas),
                        moedasGanhas = grupo.Sum(item => item.moedasGanhas),
                        MoedasExpiradas = grupo.Sum(item => item.MoedasExpiradas),
                        qtdPessoas = grupo.Count(item2 => item2.resultado > 0),
                        resultadoAPI = grupo.Sum(item => item.resultadoAPI),
                    }).ToList();
                }
            }
            else
            {
                cesta = rmams.Select(grupo => new ModelsEx.homeRel
                {
                    mes = grupo.mes,
                    ano = grupo.ano,

                    datePay = grupo.datePay,
                    dateReferer = grupo.dateReferer,

                    indicatorType = "INTEGER",
                    indicador = "Cesta de Indicadores",
                    cod_indicador = "10000012",
                    meta = grupo.meta,
                    data_atualizacao = grupo.data_atualizacao,
                    cod_gip = grupo.cod_gip,
                    setor = grupo.setor,
                    home_based = grupo.home_based,
                    site = grupo.site,
                    turno = grupo.turno,


                    goal = grupo.goal,
                    weight = grupo.weight,
                    hierarchylevel = grupo.hierarchylevel,
                    coin1 = grupo.coin1,
                    coin2 = grupo.coin2,
                    coin3 = grupo.coin3,
                    coin4 = grupo.coin4,
                    idgda_sector = grupo.idgda_sector,
                    min1 = cm.min1,
                    min2 = cm.min2,
                    min3 = cm.min3,
                    min4 = cm.min4,
                    conta = grupo.conta,
                    better = grupo.better,

                    idcollaborator = grupo.idcollaborator,
                    name = grupo.name,
                    cargo = grupo.cargo,
                    nome_supervisor = grupo.nome_supervisor,
                    matricula_supervisor = grupo.matricula_supervisor,
                    matricula_coordenador = grupo.matricula_coordenador,
                    nome_coordenador = grupo.nome_coordenador,
                    matricula_gerente_ii = grupo.matricula_gerente_ii,
                    nome_gerente_ii = grupo.nome_gerente_ii,
                    matricula_gerente_i = grupo.matricula_gerente_i,
                    nome_gerente_i = grupo.nome_gerente_i,
                    matricula_diretor = grupo.matricula_diretor,
                    nome_diretor = grupo.nome_diretor,
                    matricula_ceo = grupo.matricula_ceo,
                    nome_ceo = grupo.nome_ceo,


                    data = grupo.data,

                    fator0 = grupo.fator0,
                    fator1 = grupo.fator1,

                    diasTrabalhados = grupo.diasTrabalhados,
                    diasEscalados = grupo.diasEscalados,
                    moedasPossiveis = grupo.moedasPossiveis,
                    moedasGanhas = grupo.moedasGanhas,
                    MoedasExpiradas = grupo.MoedasExpiradas,
                    qtdPessoas = grupo.qtdPessoas,
                    resultadoAPI = grupo.resultadoAPI,
                }).ToList();
            }

            teste = cesta.FindAll(s => s.idcollaborator == "789587").ToList();


            foreach (ModelsEx.homeRel cc in cesta)
            {

                if (cc.idcollaborator == "789587")
                {
                    var parou = true;
                }

                //if (cc.name == "TAYNNA BORGES SOUZA SILVA DE OLIVEIRA" && cc.data == "02/10/2023 00:00:00")
                //{
                //    var parou = true;
                //}

                ModelsEx.homeRel hr = doCalculationBasket(cc, cm);

                if (agrupar == false)
                {
                    hr.indicatorType = "INTEGER";
                    hr.indicador = "Cesta de Indicadores";
                    hr.cod_indicador = "10000012";
                }

                hr.meta = cc.moedasPossiveis.ToString();
                //hr.metaSomada = cc.moedasPossiveis;
                hr.goal = cc.moedasPossiveis;
                hr.better = "BIGGER_BETTER";


                hr.resultado = cc.moedasGanhas;

                NovaCesta.Add(hr);
            }

            ModelsEx.homeRel tesasdasd = NovaCesta.Find(l => l.name == "AMANDA GABRIELE FONTES DE ANDRADE");

            return NovaCesta;
        }

        public static int retornaIndicadorAcessoQtd(string dtInicial, string dtFinal, string sectors, string collaborators, string hierarchies, string ordem, bool Thread = false)
        {
            int qtd = 0;
            //PREPARAR OS FILTROS
            string filter = "";
            string orderBy = "";

            //FILTRO POR SETOR.
            if (sectors != "")
            {
                //filter = filter + $" AND CL.IDGDA_SECTOR IN ({sectors}) ";
                filter = filter + $" AND CL.IDGDA_SECTOR_REFERENCE IN ({sectors}) ";
            }

            //CONSULTA NO BANCO DO RELATORIO DE MONETIZAÇÃO DIARIO.
            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtInicial);
            stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFinal);
            stb.AppendFormat("SELECT COUNT(0) AS QTD ");
            stb.AppendFormat("FROM  ");


            stb.Append("  (SELECT CASE WHEN IDGDA_SUBSECTOR IS NOT NULL THEN IDGDA_SUBSECTOR ELSE IDGDA_SECTOR END AS IDGDA_SECTOR, ");
            stb.Append("          IDGDA_SUBSECTOR, ");
            stb.Append("          IDGDA_SECTOR AS IDGDA_SECTOR_REFERENCE, ");

            //stb.AppendFormat("  (SELECT IDGDA_SECTOR, ");
            //stb.AppendFormat("          IDGDA_SUBSECTOR, ");
            stb.AppendFormat("          CREATED_AT, ");
            stb.AppendFormat("          IDGDA_COLLABORATORS, ");
            stb.AppendFormat("          IDGDA_SECTOR_SUPERVISOR, ");
            stb.AppendFormat("          ACTIVE, ");
            stb.AppendFormat("          CARGO, ");
            stb.AppendFormat("          HOME_BASED, ");
            stb.AppendFormat("          SITE, ");
            stb.AppendFormat("          PERIODO, ");
            stb.AppendFormat("          MATRICULA_SUPERVISOR, ");
            stb.AppendFormat("          NOME_SUPERVISOR, ");
            stb.AppendFormat("          MATRICULA_COORDENADOR, ");
            stb.AppendFormat("          NOME_COORDENADOR, ");
            stb.AppendFormat("          MATRICULA_GERENTE_II, ");
            stb.AppendFormat("          NOME_GERENTE_II, ");
            stb.AppendFormat("          MATRICULA_GERENTE_I, ");
            stb.AppendFormat("          NOME_GERENTE_I, ");
            stb.AppendFormat("          MATRICULA_DIRETOR, ");
            stb.AppendFormat("          NOME_DIRETOR, ");
            stb.AppendFormat("          MATRICULA_CEO, ");
            stb.AppendFormat("          NOME_CEO ");
            stb.AppendFormat("   FROM GDA_COLLABORATORS_DETAILS (NOLOCK) ");
            stb.AppendFormat("   WHERE CREATED_AT >= @DATAINICIAL ");
            stb.AppendFormat("     AND CREATED_AT <= @DATAFINAL ) AS CL  ");
            stb.AppendFormat("INNER JOIN GDA_INDICATOR (NOLOCK) AS IT ON IT.IDGDA_INDICATOR IN ('10000013','10000014') ");

            stb.AppendFormat("LEFT JOIN ");
            stb.AppendFormat("  (SELECT COUNT(0) AS 'ESCALADO', ");
            stb.AppendFormat("          IDGDA_COLLABORATORS, ");
            stb.AppendFormat("          CREATED_AT ");
            stb.AppendFormat("   FROM GDA_RESULT (NOLOCK) ");
            stb.AppendFormat("   WHERE INDICADORID = -1 ");
            stb.AppendFormat("     AND CREATED_AT >= @DATAINICIAL ");
            stb.AppendFormat("     AND CREATED_AT <= @DATAFINAL ");
            stb.AppendFormat("     AND RESULT = 1 ");
            stb.AppendFormat("     AND DELETED_AT IS NULL ");
            stb.AppendFormat("   GROUP BY IDGDA_COLLABORATORS, ");
            stb.AppendFormat("            CREATED_AT) AS ESC ON ESC.IDGDA_COLLABORATORS = CL.IDGDA_COLLABORATORS ");
            stb.AppendFormat("AND ESC.CREATED_AT = CL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN ");
            stb.AppendFormat("  (SELECT COUNT(DISTINCT IDGDA_COLLABORATOR) AS 'LOGIN', ");
            stb.AppendFormat("          IDGDA_COLLABORATOR, ");
            stb.AppendFormat("          CONVERT(DATE, DATE_ACCESS) AS CREATED_AT ");
            stb.AppendFormat("   FROM GDA_LOGIN_ACCESS (NOLOCK) ");
            stb.AppendFormat("   WHERE CONVERT(DATE, DATE_ACCESS) >= @DATAINICIAL ");
            stb.AppendFormat("     AND CONVERT(DATE, DATE_ACCESS) <= @DATAFINAL ");
            stb.AppendFormat("   GROUP BY IDGDA_COLLABORATOR, ");
            stb.AppendFormat("            CONVERT(DATE, DATE_ACCESS)) AS LOG ON LOG.IDGDA_COLLABORATOR = CL.IDGDA_COLLABORATORS ");
            stb.AppendFormat("AND LOG.CREATED_AT = CL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN ");
            stb.AppendFormat("  (SELECT COUNT(0) AS 'TRABALHADO', ");
            stb.AppendFormat("          IDGDA_COLLABORATORS, ");
            stb.AppendFormat("          CREATED_AT ");
            stb.AppendFormat("   FROM GDA_RESULT (NOLOCK) ");
            stb.AppendFormat("   WHERE INDICADORID = 2 ");
            stb.AppendFormat("     AND CREATED_AT >= @DATAINICIAL ");
            stb.AppendFormat("     AND CREATED_AT <= @DATAFINAL ");
            stb.AppendFormat("     AND RESULT = 0 ");
            stb.AppendFormat("     AND DELETED_AT IS NULL ");
            stb.AppendFormat("   GROUP BY IDGDA_COLLABORATORS, ");
            stb.AppendFormat("            CREATED_AT) AS TRAB ON TRAB.IDGDA_COLLABORATORS = CL.IDGDA_COLLABORATORS ");
            stb.AppendFormat("AND TRAB.CREATED_AT = CL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS CB ON CB.IDGDA_COLLABORATORS = CL.IDGDA_COLLABORATORS ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
            stb.AppendFormat("AND HIG1.INDICATOR_ID = IT.IDGDA_INDICATOR ");
            stb.AppendFormat("AND HIG1.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.AppendFormat("AND HIG1.GROUPID = 1 ");
            stb.AppendFormat("AND CONVERT(DATE,HIG1.STARTED_AT) <= CL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIG1.ENDED_AT) >= CL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG2 ON HIG2.DELETED_AT IS NULL ");
            stb.AppendFormat("AND HIG2.INDICATOR_ID = IT.IDGDA_INDICATOR ");
            stb.AppendFormat("AND HIG2.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.AppendFormat("AND HIG2.GROUPID = 2 ");
            stb.AppendFormat("AND CONVERT(DATE,HIG2.STARTED_AT) <= CL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIG2.ENDED_AT) >= CL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG3 ON HIG3.DELETED_AT IS NULL ");
            stb.AppendFormat("AND HIG3.INDICATOR_ID = IT.IDGDA_INDICATOR ");
            stb.AppendFormat("AND HIG3.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.AppendFormat("AND HIG3.GROUPID = 3 ");
            stb.AppendFormat("AND CONVERT(DATE,HIG3.STARTED_AT) <= CL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIG3.ENDED_AT) >= CL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG4 ON HIG4.DELETED_AT IS NULL ");
            stb.AppendFormat("AND HIG4.INDICATOR_ID = IT.IDGDA_INDICATOR ");
            stb.AppendFormat("AND HIG4.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.AppendFormat("AND HIG4.GROUPID = 4 ");
            stb.AppendFormat("AND CONVERT(DATE,HIG4.STARTED_AT) <= CL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIG4.ENDED_AT) >= CL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN GDA_CHECKING_ACCOUNT (NOLOCK) AS CA ON CA.COLLABORATOR_ID = CL.IDGDA_COLLABORATORS ");
            stb.AppendFormat("AND CA.RESULT_DATE = CL.CREATED_AT ");
            stb.AppendFormat("AND CA.GDA_INDICATOR_IDGDA_INDICATOR = IT.IDGDA_INDICATOR ");

            stb.AppendFormat("LEFT JOIN ");
            stb.AppendFormat("  (SELECT SUM(INPUT) - SUM(OUTPUT) AS INPUT, ");
            stb.AppendFormat("          gda_indicator_idgda_indicator, ");
            stb.AppendFormat("          result_date, ");
            stb.AppendFormat("          COLLABORATOR_ID ");
            stb.AppendFormat("   FROM GDA_CHECKING_ACCOUNT ");
            stb.AppendFormat("   WHERE CONVERT(DATE,RESULT_DATE) >= @DATAINICIAL ");
            stb.AppendFormat("     AND CONVERT(DATE,RESULT_DATE) <= @DATAFINAL ");
            stb.AppendFormat("   GROUP BY GDA_INDICATOR_IDGDA_INDICATOR, ");
            stb.AppendFormat("            RESULT_DATE, ");
            stb.AppendFormat("            COLLABORATOR_ID) AS MZ ON MZ.GDA_INDICATOR_IDGDA_INDICATOR IN ('10000013','10000014') ");
            stb.AppendFormat("AND MZ.RESULT_DATE = CL.CREATED_AT ");
            stb.AppendFormat("AND MZ.COLLABORATOR_ID = CL.IDGDA_COLLABORATORS ");
            stb.AppendFormat("LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = CL.IDGDA_SECTOR ");
            stb.AppendFormat("LEFT JOIN GDA_SECTOR (NOLOCK) AS SECSUP ON SECSUP.IDGDA_SECTOR = CL.IDGDA_SECTOR_SUPERVISOR ");
            stb.AppendFormat("LEFT JOIN GDA_SECTOR (NOLOCK) AS SECREFERENCE ON SECREFERENCE.IDGDA_SECTOR = CL.IDGDA_SECTOR_REFERENCE ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) AS HIS ON HIS.INDICATOR_ID = IT.IDGDA_INDICATOR ");
            stb.AppendFormat("AND HIS.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.AppendFormat("AND CONVERT(DATE,HIS.STARTED_AT) <= CL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIS.ENDED_AT) >= CL.CREATED_AT ");
            stb.AppendFormat("AND HIS.DELETED_AT IS NULL ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_SCORE_INDICATOR_SECTOR (NOLOCK) AS SC ON SC.INDICATOR_ID = IT.IDGDA_INDICATOR ");
            stb.AppendFormat("AND SC.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.AppendFormat("AND CONVERT(DATE,SC.STARTED_AT) <= CL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,SC.ENDED_AT) >= CL.CREATED_AT ");
            stb.AppendFormat("WHERE 1 = 1 ");
            stb.AppendFormat("  AND CL.CREATED_AT >= @DATAINICIAL ");
            stb.AppendFormat("  AND CL.CREATED_AT <= @DATAFINAL ");
            //stb.AppendFormat("  AND CL.active = 'true' ");
            stb.AppendFormat($"  AND CL.CARGO = 'AGENTE' {filter}");
            //stb.AppendFormat("  AND HIG1.SECTOR_ID IS NOT NULL ");


            List<ModelsEx.homeRel> rmams = new List<ModelsEx.homeRel>();
            using (SqlConnection connection = new SqlConnection(Database.retornaConn(Thread)))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                {
                    command.CommandTimeout = 0;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            qtd = Convert.ToInt32(reader["QTD"].ToString());
                        }
                    }
                }
                connection.Close();
            }
            return qtd;

        }


        public static List<ModelsEx.homeRel> retornaIndicadorAcesso(string dtInicial, string dtFinal, string sectors, string collaborators, string hierarchies, string ordem, bool Thread = false)
        {
            //PREPARAR OS FILTROS
            string filter = "";
            string orderBy = "";

            //FILTRO POR SETOR.
            if (sectors != "")
            {
                //filter = filter + $" AND CL.IDGDA_SECTOR IN ({sectors}) ";
                filter = filter + $" AND CL.IDGDA_SECTOR_REFERENCE IN ({sectors}) ";
            }

            ////FILTRO POR INDICADOR.
            //if (indicators != "")
            //{
            //    filter = filter + $" AND R.INDICADORID IN ({indicators}) ";
            //}
            // FILTRO POR COLABORATORES
            //if (collaborators != "")
            //{
            //    StringBuilder stb2 = new StringBuilder();
            //    stb2.AppendFormat(" AND (CL.IDGDA_COLLABORATORS IN ({0}) OR  ", collaborators);
            //    stb2.AppendFormat("	    CL.MATRICULA_SUPERVISOR IN ({0}) OR ", collaborators);
            //    stb2.AppendFormat("		CL.MATRICULA_COORDENADOR IN ({0}) OR ", collaborators);
            //    stb2.AppendFormat("		CL.MATRICULA_GERENTE_II IN ({0}) OR ", collaborators);
            //    stb2.AppendFormat("		CL.MATRICULA_GERENTE_I IN ({0}) OR ", collaborators);
            //    stb2.AppendFormat("		CL.MATRICULA_DIRETOR IN ({0}) OR ", collaborators);
            //    stb2.AppendFormat("		CL.MATRICULA_CEO IN ({0})) ", collaborators);
            //    filter = filter + $" {stb2.ToString()} ";
            //}

            //CONSULTA NO BANCO DO RELATORIO DE MONETIZAÇÃO DIARIO.
            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtInicial);
            stb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFinal);
            stb.AppendFormat("SELECT MONTH(@DATAINICIAL) AS MES, ");
            stb.AppendFormat("       YEAR(@DATAINICIAL) AS ANO, ");
            stb.AppendFormat(" MAX(CONVERT(DATE, CA.CREATED_AT)) AS 'DATA DO PAGAMENTO', ");
            stb.AppendFormat("       CL.IDGDA_COLLABORATORS AS 'MATRICULA', ");
            stb.AppendFormat("       MAX(CB.NAME) AS NAME, ");
            stb.AppendFormat("       MAX(CL.CARGO) AS CARGO, ");
            stb.AppendFormat("       IT.TYPE AS TIPO, ");
            stb.AppendFormat("       MAX(CONVERT(DATE, CL.CREATED_AT)) AS 'REFERENCIA PAGAMENTO', ");
            stb.AppendFormat("       IT.IDGDA_INDICATOR AS 'COD INDICADOR', ");
            stb.AppendFormat("       IT.NAME AS 'INDICADOR', ");
            stb.AppendFormat("       MAX(HIS.GOAL) AS META, ");
            stb.AppendFormat("       MAX(HIS.GOAL_NIGHT) AS META_NOTURNA, ");
            stb.AppendFormat("       MAX(HIS.GOAL_LATENIGHT) AS META_MADRUGADA, ");
            stb.AppendFormat("       MAX(SC.WEIGHT_SCORE) AS SCORE, ");
            stb.AppendFormat("       MAX(HIG1.METRIC_MIN) AS MIN1, ");
            stb.AppendFormat("       MAX(HIG2.METRIC_MIN) AS MIN2, ");
            stb.AppendFormat("       MAX(HIG3.METRIC_MIN) AS MIN3, ");
            stb.AppendFormat("       MAX(HIG4.METRIC_MIN) AS MIN4, ");
            stb.AppendFormat("       MAX(HIG1.METRIC_MIN_NIGHT) AS MIN1_NOTURNO, ");
            stb.AppendFormat("       MAX(HIG2.METRIC_MIN_NIGHT) AS MIN2_NOTURNO, ");
            stb.AppendFormat("       MAX(HIG3.METRIC_MIN_NIGHT) AS MIN3_NOTURNO, ");
            stb.AppendFormat("       MAX(HIG4.METRIC_MIN_NIGHT) AS MIN4_NOTURNO, ");
            stb.AppendFormat("       MAX(HIG1.METRIC_MIN_LATENIGHT) AS MIN1_MADRUGADA, ");
            stb.AppendFormat("       MAX(HIG2.METRIC_MIN_LATENIGHT) AS MIN2_MADRUGADA, ");
            stb.AppendFormat("       MAX(HIG3.METRIC_MIN_LATENIGHT) AS MIN3_MADRUGADA, ");
            stb.AppendFormat("       MAX(HIG4.METRIC_MIN_LATENIGHT) AS MIN4_MADRUGADA, ");
            stb.AppendFormat("       '' AS FATOR, ");
            stb.AppendFormat("      '(#FATOR1/#FATOR0)' AS CONTA, ");
            stb.AppendFormat("       'BIGGER_BETTER' AS BETTER, ");
            stb.AppendFormat("       MAX(IT.TYPE) AS TYPE, ");
            stb.AppendFormat("       '' AS RESULTADO, ");
            stb.AppendFormat("       '' AS PORCENTUAL, ");
            stb.AppendFormat("       MAX(IT.TYPE) AS TYPE, ");
            stb.AppendFormat("       '' AS RESULTADOAPI, ");
            stb.AppendFormat("       MAX(TRAB.TRABALHADO) AS TRABALHADO, ");
            stb.AppendFormat("       MAX(ESC.ESCALADO) AS ESCALADO, ");
            stb.AppendFormat("       MAX(LOG.LOGIN) AS LOGADO, ");
            stb.AppendFormat("       MAX(ISNULL(HIG1.MONETIZATION, 0)) AS META_MAXIMA, ");
            stb.AppendFormat("       MAX(ISNULL(HIG1.MONETIZATION_NIGHT, 0)) AS META_MAXIMA_NOTURNA, ");
            stb.AppendFormat("       MAX(ISNULL(HIG1.MONETIZATION_LATENIGHT, 0)) AS META_MAXIMA_MADRUGADA, ");
            stb.AppendFormat("       CASE ");
            stb.AppendFormat("           WHEN MAX(MZ.INPUT) IS NULL THEN 0 ");
            stb.AppendFormat("           ELSE MAX(MZ.INPUT) ");
            stb.AppendFormat("       END AS MOEDA_GANHA, ");
            stb.AppendFormat("       '' AS GRUPO, ");
            stb.AppendFormat("       GETDATE() AS 'Data de atualização', ");
            stb.AppendFormat("       MAX(CL.IDGDA_SECTOR) AS COD_GIP, ");
            stb.AppendFormat("       MAX(SEC.NAME) AS SETOR, ");
            stb.AppendFormat("       MAX(CL.IDGDA_SECTOR_REFERENCE) AS COD_GIP_REFERENCE, ");
            stb.AppendFormat("       MAX(SECREFERENCE.NAME) AS SETOR_REFERENCE, ");
            stb.AppendFormat("       MAX(CL.IDGDA_SECTOR_SUPERVISOR) AS COD_GIP_SUPERVISOR, ");
            stb.AppendFormat("       MAX(SECSUP.NAME) AS SETOR_SUPERVISOR, ");
            stb.AppendFormat("       MAX(CL.HOME_BASED) AS HOME_BASED, ");
            stb.AppendFormat("       MAX(CL.SITE) AS SITE, ");
            stb.AppendFormat("       MAX(CL.PERIODO) AS TURNO, ");
            stb.AppendFormat("       MAX(CL.MATRICULA_SUPERVISOR) AS 'MATRICULA SUPERVISOR', ");
            stb.AppendFormat("       MAX(CL.NOME_SUPERVISOR) AS 'NOME SUPERVISOR', ");
            stb.AppendFormat("       MAX(CL.MATRICULA_COORDENADOR) AS 'MATRICULA COORDENADOR', ");
            stb.AppendFormat("       MAX(CL.NOME_COORDENADOR) AS 'NOME COORDENADOR', ");
            stb.AppendFormat("       MAX(CL.MATRICULA_GERENTE_II) AS 'MATRICULA GERENTE II', ");
            stb.AppendFormat("       MAX(CL.NOME_GERENTE_II) AS 'NOME GERENTE II', ");
            stb.AppendFormat("       MAX(CL.MATRICULA_GERENTE_I) AS 'MATRICULA GERENTE I', ");
            stb.AppendFormat("       MAX(CL.NOME_GERENTE_I) AS 'NOME GERENTE I', ");
            stb.AppendFormat("       MAX(CL.MATRICULA_DIRETOR) AS 'MATRICULA DIRETOR', ");
            stb.AppendFormat("       MAX(CL.NOME_DIRETOR) AS 'NOME DIRETOR', ");
            stb.AppendFormat("       MAX(CL.MATRICULA_CEO) AS 'MATRICULA CEO', ");
            stb.AppendFormat("       MAX(CL.NOME_CEO) AS 'NOME CEO' ");
            stb.AppendFormat("FROM  ");


            stb.Append("  (SELECT CASE WHEN IDGDA_SUBSECTOR IS NOT NULL THEN IDGDA_SUBSECTOR ELSE IDGDA_SECTOR END AS IDGDA_SECTOR, ");
            stb.Append("          IDGDA_SUBSECTOR, ");
            stb.Append("          IDGDA_SECTOR AS IDGDA_SECTOR_REFERENCE, ");

            //stb.AppendFormat("  (SELECT IDGDA_SECTOR, ");
            //stb.AppendFormat("          IDGDA_SUBSECTOR, ");
            stb.AppendFormat("          CREATED_AT, ");
            stb.AppendFormat("          IDGDA_COLLABORATORS, ");
            stb.AppendFormat("          IDGDA_SECTOR_SUPERVISOR, ");
            stb.AppendFormat("          ACTIVE, ");
            stb.AppendFormat("          CARGO, ");
            stb.AppendFormat("          HOME_BASED, ");
            stb.AppendFormat("          SITE, ");
            stb.AppendFormat("          PERIODO, ");
            stb.AppendFormat("          MATRICULA_SUPERVISOR, ");
            stb.AppendFormat("          NOME_SUPERVISOR, ");
            stb.AppendFormat("          MATRICULA_COORDENADOR, ");
            stb.AppendFormat("          NOME_COORDENADOR, ");
            stb.AppendFormat("          MATRICULA_GERENTE_II, ");
            stb.AppendFormat("          NOME_GERENTE_II, ");
            stb.AppendFormat("          MATRICULA_GERENTE_I, ");
            stb.AppendFormat("          NOME_GERENTE_I, ");
            stb.AppendFormat("          MATRICULA_DIRETOR, ");
            stb.AppendFormat("          NOME_DIRETOR, ");
            stb.AppendFormat("          MATRICULA_CEO, ");
            stb.AppendFormat("          NOME_CEO ");
            stb.AppendFormat("   FROM GDA_COLLABORATORS_DETAILS (NOLOCK) ");
            stb.AppendFormat("   WHERE CREATED_AT >= @DATAINICIAL ");
            stb.AppendFormat("     AND CREATED_AT <= @DATAFINAL ) AS CL  ");
            stb.AppendFormat("INNER JOIN GDA_INDICATOR (NOLOCK) AS IT ON IT.IDGDA_INDICATOR IN ('10000013','10000014') ");

            stb.AppendFormat("LEFT JOIN ");
            stb.AppendFormat("  (SELECT COUNT(0) AS 'ESCALADO', ");
            stb.AppendFormat("          IDGDA_COLLABORATORS, ");
            stb.AppendFormat("          CREATED_AT ");
            stb.AppendFormat("   FROM GDA_RESULT (NOLOCK) ");
            stb.AppendFormat("   WHERE INDICADORID = -1 ");
            stb.AppendFormat("     AND CREATED_AT >= @DATAINICIAL ");
            stb.AppendFormat("     AND CREATED_AT <= @DATAFINAL ");
            stb.AppendFormat("     AND RESULT = 1 ");
            stb.AppendFormat("     AND DELETED_AT IS NULL ");
            stb.AppendFormat("   GROUP BY IDGDA_COLLABORATORS, ");
            stb.AppendFormat("            CREATED_AT) AS ESC ON ESC.IDGDA_COLLABORATORS = CL.IDGDA_COLLABORATORS ");
            stb.AppendFormat("AND ESC.CREATED_AT = CL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN ");
            stb.AppendFormat("  (SELECT COUNT(DISTINCT IDGDA_COLLABORATOR) AS 'LOGIN', ");
            stb.AppendFormat("          IDGDA_COLLABORATOR, ");
            stb.AppendFormat("          CONVERT(DATE, DATE_ACCESS) AS CREATED_AT ");
            stb.AppendFormat("   FROM GDA_LOGIN_ACCESS (NOLOCK) ");
            stb.AppendFormat("   WHERE CONVERT(DATE, DATE_ACCESS) >= @DATAINICIAL ");
            stb.AppendFormat("     AND CONVERT(DATE, DATE_ACCESS) <= @DATAFINAL ");
            stb.AppendFormat("   GROUP BY IDGDA_COLLABORATOR, ");
            stb.AppendFormat("            CONVERT(DATE, DATE_ACCESS)) AS LOG ON LOG.IDGDA_COLLABORATOR = CL.IDGDA_COLLABORATORS ");
            stb.AppendFormat("AND LOG.CREATED_AT = CL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN ");
            stb.AppendFormat("  (SELECT COUNT(0) AS 'TRABALHADO', ");
            stb.AppendFormat("          IDGDA_COLLABORATORS, ");
            stb.AppendFormat("          CREATED_AT ");
            stb.AppendFormat("   FROM GDA_RESULT (NOLOCK) ");
            stb.AppendFormat("   WHERE INDICADORID = 2 ");
            stb.AppendFormat("     AND CREATED_AT >= @DATAINICIAL ");
            stb.AppendFormat("     AND CREATED_AT <= @DATAFINAL ");
            stb.AppendFormat("     AND RESULT = 0 ");
            stb.AppendFormat("     AND DELETED_AT IS NULL ");
            stb.AppendFormat("   GROUP BY IDGDA_COLLABORATORS, ");
            stb.AppendFormat("            CREATED_AT) AS TRAB ON TRAB.IDGDA_COLLABORATORS = CL.IDGDA_COLLABORATORS ");
            stb.AppendFormat("AND TRAB.CREATED_AT = CL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS CB ON CB.IDGDA_COLLABORATORS = CL.IDGDA_COLLABORATORS ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG1 ON HIG1.DELETED_AT IS NULL ");
            stb.AppendFormat("AND HIG1.INDICATOR_ID = IT.IDGDA_INDICATOR ");
            stb.AppendFormat("AND HIG1.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.AppendFormat("AND HIG1.GROUPID = 1 ");
            stb.AppendFormat("AND CONVERT(DATE,HIG1.STARTED_AT) <= CL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIG1.ENDED_AT) >= CL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG2 ON HIG2.DELETED_AT IS NULL ");
            stb.AppendFormat("AND HIG2.INDICATOR_ID = IT.IDGDA_INDICATOR ");
            stb.AppendFormat("AND HIG2.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.AppendFormat("AND HIG2.GROUPID = 2 ");
            stb.AppendFormat("AND CONVERT(DATE,HIG2.STARTED_AT) <= CL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIG2.ENDED_AT) >= CL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG3 ON HIG3.DELETED_AT IS NULL ");
            stb.AppendFormat("AND HIG3.INDICATOR_ID = IT.IDGDA_INDICATOR ");
            stb.AppendFormat("AND HIG3.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.AppendFormat("AND HIG3.GROUPID = 3 ");
            stb.AppendFormat("AND CONVERT(DATE,HIG3.STARTED_AT) <= CL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIG3.ENDED_AT) >= CL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS HIG4 ON HIG4.DELETED_AT IS NULL ");
            stb.AppendFormat("AND HIG4.INDICATOR_ID = IT.IDGDA_INDICATOR ");
            stb.AppendFormat("AND HIG4.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.AppendFormat("AND HIG4.GROUPID = 4 ");
            stb.AppendFormat("AND CONVERT(DATE,HIG4.STARTED_AT) <= CL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIG4.ENDED_AT) >= CL.CREATED_AT ");
            stb.AppendFormat("LEFT JOIN GDA_CHECKING_ACCOUNT (NOLOCK) AS CA ON CA.COLLABORATOR_ID = CL.IDGDA_COLLABORATORS ");
            stb.AppendFormat("AND CA.RESULT_DATE = CL.CREATED_AT ");
            stb.AppendFormat("AND CA.GDA_INDICATOR_IDGDA_INDICATOR = IT.IDGDA_INDICATOR ");

            stb.AppendFormat("LEFT JOIN ");
            stb.AppendFormat("  (SELECT SUM(INPUT) - SUM(OUTPUT) AS INPUT, ");
            stb.AppendFormat("          gda_indicator_idgda_indicator, ");
            stb.AppendFormat("          result_date, ");
            stb.AppendFormat("          COLLABORATOR_ID ");
            stb.AppendFormat("   FROM GDA_CHECKING_ACCOUNT ");
            stb.AppendFormat("   WHERE CONVERT(DATE,RESULT_DATE) >= @DATAINICIAL ");
            stb.AppendFormat("     AND CONVERT(DATE,RESULT_DATE) <= @DATAFINAL ");
            stb.AppendFormat("   GROUP BY GDA_INDICATOR_IDGDA_INDICATOR, ");
            stb.AppendFormat("            RESULT_DATE, ");
            stb.AppendFormat("            COLLABORATOR_ID) AS MZ ON MZ.GDA_INDICATOR_IDGDA_INDICATOR IN ('10000013','10000014') ");
            stb.AppendFormat("AND MZ.RESULT_DATE = CL.CREATED_AT ");
            stb.AppendFormat("AND MZ.COLLABORATOR_ID = CL.IDGDA_COLLABORATORS ");
            stb.AppendFormat("LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = CL.IDGDA_SECTOR ");
            stb.AppendFormat("LEFT JOIN GDA_SECTOR (NOLOCK) AS SECSUP ON SECSUP.IDGDA_SECTOR = CL.IDGDA_SECTOR_SUPERVISOR ");
            stb.AppendFormat("LEFT JOIN GDA_SECTOR (NOLOCK) AS SECREFERENCE ON SECREFERENCE.IDGDA_SECTOR = CL.IDGDA_SECTOR_REFERENCE ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) AS HIS ON HIS.INDICATOR_ID = IT.IDGDA_INDICATOR ");
            stb.AppendFormat("AND HIS.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.AppendFormat("AND CONVERT(DATE,HIS.STARTED_AT) <= CL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,HIS.ENDED_AT) >= CL.CREATED_AT ");
            stb.AppendFormat("AND HIS.DELETED_AT IS NULL ");
            stb.AppendFormat("LEFT JOIN GDA_HISTORY_SCORE_INDICATOR_SECTOR (NOLOCK) AS SC ON SC.INDICATOR_ID = IT.IDGDA_INDICATOR ");
            stb.AppendFormat("AND SC.SECTOR_ID = CL.IDGDA_SECTOR ");
            stb.AppendFormat("AND CONVERT(DATE,SC.STARTED_AT) <= CL.CREATED_AT ");
            stb.AppendFormat("AND CONVERT(DATE,SC.ENDED_AT) >= CL.CREATED_AT ");
            stb.AppendFormat("WHERE 1 = 1 ");
            stb.AppendFormat("  AND CL.CREATED_AT >= @DATAINICIAL ");
            stb.AppendFormat("  AND CL.CREATED_AT <= @DATAFINAL ");
            //stb.AppendFormat("  AND CL.active = 'true' ");
            stb.AppendFormat($"  AND CL.CARGO = 'AGENTE' {filter}");
            //stb.AppendFormat("  AND HIG1.SECTOR_ID IS NOT NULL ");
            stb.AppendFormat("GROUP BY CL.IDGDA_COLLABORATORS, ");
            stb.AppendFormat("IT.TYPE,");
            stb.AppendFormat("IT.IDGDA_INDICATOR,");
            stb.AppendFormat("IT.NAME,");
            stb.AppendFormat("         CONVERT(DATE, CL.CREATED_AT) ");


            List<ModelsEx.homeRel> rmams = new List<ModelsEx.homeRel>();
            using (SqlConnection connection = new SqlConnection(Database.retornaConn(Thread)))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                {
                    command.CommandTimeout = 0;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            string factor0 = "";
                            string factor1 = "";
                            ModelsEx.homeRel rmam = new ModelsEx.homeRel();
                            rmam.mes = reader["MES"].ToString();
                            rmam.ano = reader["ANO"].ToString();
                            rmam.datePay = reader["DATA DO PAGAMENTO"].ToString();
                            rmam.dateReferer = reader["REFERENCIA PAGAMENTO"].ToString();
                            rmam.data = reader["REFERENCIA PAGAMENTO"].ToString();
                            rmam.idcollaborator = reader["MATRICULA"].ToString();
                            rmam.name = reader["name"].ToString();
                            rmam.cod_indicador = reader["cod indicador"].ToString();
                            rmam.indicador = reader["indicador"].ToString();
                            rmam.indicatorType = reader["TIPO"].ToString();
                            if (reader["turno"].ToString() == "DIURNO")
                            {
                                if (reader["META"].ToString() == "" || reader["META"].ToString() == null)
                                {
                                    rmam.meta = "";
                                    rmam.min1 = 0;
                                    rmam.min2 = 0;
                                    rmam.min3 = 0;
                                    rmam.min4 = 0;
                                    rmam.goal = 0;
                                    rmam.moedasPossiveis = 0;
                                }
                                else
                                {
                                    rmam.meta = reader["META"].ToString();
                                    rmam.min1 = reader["MIN1"].ToString() != "" ? double.Parse(reader["MIN1"].ToString()) : 0;
                                    rmam.min2 = reader["MIN2"].ToString() != "" ? double.Parse(reader["MIN2"].ToString()) : 0;
                                    rmam.min3 = reader["MIN3"].ToString() != "" ? double.Parse(reader["MIN3"].ToString()) : 0;
                                    rmam.min4 = reader["MIN4"].ToString() != "" ? double.Parse(reader["MIN4"].ToString()) : 0;
                                    rmam.goal = reader["META"].ToString() != "" ? double.Parse(reader["META"].ToString()) : 0;
                                    rmam.moedasPossiveis = reader["META_MAXIMA"].ToString() != "" ? int.Parse(reader["META_MAXIMA"].ToString()) : 0;
                                }

                            }
                            else if (reader["turno"].ToString() == "NOTURNO")
                            {
                                if (reader["META_NOTURNA"].ToString() == "" || reader["META_NOTURNA"].ToString() == null)
                                {
                                    rmam.meta = "";
                                    rmam.min1 = 0;
                                    rmam.min2 = 0;
                                    rmam.min3 = 0;
                                    rmam.min4 = 0;
                                    rmam.goal = 0;
                                    rmam.moedasPossiveis = 0;
                                }
                                else
                                {
                                    rmam.meta = reader["META_NOTURNA"].ToString();
                                    rmam.min1 = reader["MIN1_NOTURNO"].ToString() != "" ? double.Parse(reader["MIN1_NOTURNO"].ToString()) : 0;
                                    rmam.min2 = reader["MIN2_NOTURNO"].ToString() != "" ? double.Parse(reader["MIN2_NOTURNO"].ToString()) : 0;
                                    rmam.min3 = reader["MIN3_NOTURNO"].ToString() != "" ? double.Parse(reader["MIN3_NOTURNO"].ToString()) : 0;
                                    rmam.min4 = reader["MIN4_NOTURNO"].ToString() != "" ? double.Parse(reader["MIN4_NOTURNO"].ToString()) : 0;
                                    rmam.goal = reader["META_NOTURNA"].ToString() != "" ? double.Parse(reader["META_NOTURNA"].ToString()) : 0;
                                    rmam.moedasPossiveis = reader["META_MAXIMA_NOTURNA"].ToString() != "" ? int.Parse(reader["META_MAXIMA_NOTURNA"].ToString()) : 0;
                                }                                   
                            }
                            else if (reader["turno"].ToString() == "MADRUGADA")
                            {
                                if (reader["META_MADRUGADA"].ToString() == "" || reader["META_MADRUGADA"].ToString() == null)
                                {
                                    rmam.meta = "";
                                    rmam.min1 = 0;
                                    rmam.min2 = 0;
                                    rmam.min3 = 0;
                                    rmam.min4 = 0;
                                    rmam.goal = 0;
                                    rmam.moedasPossiveis = 0;
                                }
                                else
                                {
                                    rmam.meta = reader["META_MADRUGADA"].ToString();
                                    rmam.min1 = reader["MIN1_MADRUGADA"].ToString() != "" ? double.Parse(reader["MIN1_MADRUGADA"].ToString()) : 0;
                                    rmam.min2 = reader["MIN2_MADRUGADA"].ToString() != "" ? double.Parse(reader["MIN2_MADRUGADA"].ToString()) : 0;
                                    rmam.min3 = reader["MIN3_MADRUGADA"].ToString() != "" ? double.Parse(reader["MIN3_MADRUGADA"].ToString()) : 0;
                                    rmam.min4 = reader["MIN4_MADRUGADA"].ToString() != "" ? double.Parse(reader["MIN4_MADRUGADA"].ToString()) : 0;
                                    rmam.goal = reader["META_MADRUGADA"].ToString() != "" ? double.Parse(reader["META_MADRUGADA"].ToString()) : 0;
                                    rmam.moedasPossiveis = reader["META_MAXIMA_MADRUGADA"].ToString() != "" ? int.Parse(reader["META_MAXIMA_MADRUGADA"].ToString()) : 0;
                                }                                    
                            }
                            else if (reader["turno"].ToString() == "" || reader["turno"].ToString() == null)
                            {
                                rmam.meta = "";
                                rmam.min1 = 0;
                                rmam.min2 = 0;
                                rmam.min3 = 0;
                                rmam.min4 = 0;
                                rmam.goal = 0;
                                rmam.moedasPossiveis = 0;
                            }
                            rmam.data_atualizacao = reader["data de atualização"].ToString();
                            rmam.cod_gip_supervisor = reader["COD_GIP_SUPERVISOR"].ToString();
                            rmam.setor_supervisor = reader["setor_supervisor"].ToString();
                            rmam.cargo = reader["cargo"].ToString() == "" ? "Não Informado" : reader["cargo"].ToString();

                            rmam.cod_gip = reader["cod_gip"].ToString() == "" ? "Não Informado" : reader["cod_gip"].ToString();
                            rmam.setor = reader["setor"].ToString() == "" ? "Não Informado" : reader["setor"].ToString();
                            rmam.cod_gip_reference = reader["COD_GIP_REFERENCE"].ToString() == "" ? "Não Informado" : reader["COD_GIP_REFERENCE"].ToString();
                            rmam.setor_reference = reader["SETOR_REFERENCE"].ToString() == "" ? "Não Informado" : reader["SETOR_REFERENCE"].ToString();
                            //rmam.cod_gip = reader["cod_gip"].ToString() == "" ? "Não Informado" : reader["cod_gip"].ToString();
                            //rmam.setor = reader["setor"].ToString() == "" ? "Não Informado" : reader["setor"].ToString();
                            //rmam.cod_gip_reference = reader["COD_GIP_REFERENCE"].ToString() == "" ? "Não Informado" : reader["COD_GIP_REFERENCE"].ToString();
                            //rmam.setor_reference = reader["SETOR_REFERENCE"].ToString() == "" ? "Não Informado" : reader["SETOR_REFERENCE"].ToString();
                            string teste = reader["SETOR_REFERENCE"].ToString();

                            rmam.home_based = reader["home_based"].ToString() == "" ? "Não Informado" : reader["home_based"].ToString();
                            rmam.site = reader["site"].ToString();
                            rmam.turno = reader["turno"].ToString();
                            rmam.matricula_supervisor = reader["matricula supervisor"].ToString();
                            rmam.nome_supervisor = reader["nome supervisor"].ToString();
                            rmam.matricula_coordenador = reader["matricula coordenador"].ToString();
                            rmam.nome_coordenador = reader["nome coordenador"].ToString();
                            rmam.matricula_gerente_ii = reader["matricula gerente ii"].ToString();
                            rmam.nome_gerente_ii = reader["nome gerente ii"].ToString();
                            rmam.matricula_gerente_i = reader["matricula gerente i"].ToString();
                            rmam.nome_gerente_i = reader["nome gerente i"].ToString();
                            rmam.matricula_diretor = reader["matricula diretor"].ToString();
                            rmam.nome_diretor = reader["nome diretor"].ToString();
                            rmam.matricula_ceo = reader["matricula ceo"].ToString();
                            rmam.nome_ceo = reader["nome ceo"].ToString();
                            rmam.fator0 = 0;
                            rmam.fator1 = 0;
                            rmam.conta = reader["CONTA"].ToString();
                            rmam.better = reader["BETTER"].ToString();
                            rmam.resultadoAPI = reader["RESULTADOAPI"].ToString() != "" ? double.Parse(reader["RESULTADOAPI"].ToString()) != 100 ? rmam.resultadoAPI = double.Parse(reader["RESULTADOAPI"].ToString()) : 0 : 0;
                            rmam.diasTrabalhados = reader["TRABALHADO"].ToString() != "" ? reader["TRABALHADO"].ToString() : "-";
                            rmam.diasEscalados = reader["ESCALADO"].ToString() != "" ? reader["ESCALADO"].ToString() : "-";

                            //Adicionado para Indicador de Acesso
                            rmam.sumDiasLogados = reader["LOGADO"].ToString() != "" ? Convert.ToInt32(reader["LOGADO"].ToString()) : 0;
                            rmam.sumDiasEscalados = reader["ESCALADO"].ToString() != "" ? Convert.ToInt32(reader["ESCALADO"].ToString()) : 0;
                            rmam.sumDiasLogadosEscalados =
                                reader["ESCALADO"].ToString() != "" && reader["LOGADO"].ToString() != "" ?
                                (Convert.ToInt32(reader["ESCALADO"].ToString()) == 1 && Convert.ToInt32(reader["LOGADO"].ToString()) == 1 ? 1 : 0)
                                : 0;

                            rmam.moedasGanhas = reader["MOEDA_GANHA"].ToString() != "" ? double.Parse(reader["MOEDA_GANHA"].ToString()) : 0;
                            rmam.vemMeta = rmam.meta.ToString() == "" && rmam.meta.ToString() == null || rmam.min1.ToString() == "" ? 0 : 1;
                            rmam.peso = reader["SCORE"].ToString() != "" ? double.Parse(reader["SCORE"].ToString()) : 0;
                            rmams.Add(rmam);



                        }
                    }
                }
                connection.Close();
            }
            return rmams;

        }

        public static ModelsEx.homeRel doCalculationIndicatorLogin(ModelsEx.homeRel cc)
        {
            double calculo = 0.0;

            if (cc.diasEscalados == "1")
            {
                cc.vemMeta = 1;
                if (cc.cargo == "AGENTE")
                {
                    cc.meta = "1";
                    calculo = (cc.Logado / 1) * 100;
                    cc.grupo = "-";
                    cc.resultado = cc.Logado;
                }
                else
                {
                    cc.meta = cc.qtdPessoas.ToString();
                    calculo = (cc.Logado / cc.qtdPessoas) * 100;
                    cc.grupo = "-";
                    cc.resultado = cc.Logado;
                }
                cc.porcentual = Math.Round(calculo, 2, MidpointRounding.AwayFromZero);
            }
            return cc;
        }

        public static ModelsEx.homeRel doCalculationBasket(ModelsEx.homeRel cc, cestaMetrica cm)
        {
            try
            {

                double moedasMaximas = 0;
                //if (cc.vemMeta == 0)
                //{
                //    moedasMaximas = 0;
                //}
                if (cc.diasEscalados == "1")
                {
                    moedasMaximas = Math.Round(cc.moedasPossiveis, 2, MidpointRounding.AwayFromZero);
                    //moedasMaximas = Convert.ToInt32(cc.moedasPossiveis);
                }
                else if (cc.diasEscalados == "0" && cc.diasTrabalhados == "1")
                {
                    moedasMaximas = Math.Round(cc.moedasPossiveis, 2, MidpointRounding.AwayFromZero);
                    //moedasMaximas = Convert.ToInt32(cc.moedasPossiveis);
                }
                else if (cc.diasEscalados == "0" && cc.diasTrabalhados == "0")
                {
                    moedasMaximas = 0;
                }
                else if (cc.moedasGanhas > 0)
                {
                    moedasMaximas = Math.Round(cc.moedasPossiveis, 2, MidpointRounding.AwayFromZero);
                    //moedasMaximas = Convert.ToInt32(cc.moedasPossiveis);
                }
                else if (cc.diasEscalados == "-" || cc.diasTrabalhados == "-")
                {
                    //Caso não venha informação de escalado e dias trabalhados, mas vir resultado.. considerar
                    if (cc.resultadoAPI > 0)
                    {
                        moedasMaximas = Math.Round(cc.moedasPossiveis, 2, MidpointRounding.AwayFromZero);
                        //moedasMaximas = Convert.ToInt32(cc.moedasPossiveis);
                    }
                    else
                    {
                        cc.moedasPossiveis = 0;
                        moedasMaximas = 0;
                    }
                }

                cc.moedasPossiveis = moedasMaximas;

                if (moedasMaximas == 0)
                {
                    cc.porcentual = 100;
                    cc.grupo = "Diamante";
                    cc.grupoNum = 1;
                }
                else
                {
                    //double moedasGanhas = cc.moedasGanhas;
                    //if (cc.cargo != "AGENTE")
                    //{

                    //}

                    //if (moedasMaximas == 0)
                    //{
                    //    var asdasd = true;
                    //}

                    double calculo = (cc.moedasGanhas / moedasMaximas) * 100;


                    //Verifica a qual grupo pertence
                    if (calculo >= cm.min1)
                    {
                        cc.grupo = "Diamante";
                        cc.grupoNum = 1;
                    }
                    else if (calculo >= cm.min2)
                    {
                        cc.grupo = "Ouro";
                        cc.grupoNum = 2;
                    }
                    else if (calculo >= cm.min3)
                    {
                        cc.grupo = "Prata";
                        cc.grupoNum = 3;
                    }
                    else if (calculo >= cm.min4)
                    {
                        cc.grupo = "Bronze";
                        cc.grupoNum = 4;
                    }
                    else
                    {
                        cc.grupo = "Bronze";
                        cc.grupoNum = 4;
                    }

                    cc.porcentual = Math.Round(calculo, 2, MidpointRounding.AwayFromZero);
                    //cc.moedasPossiveis = Math.Round(cc.moedasPossiveis, 0, MidpointRounding.AwayFromZero);
                }

            }
            catch (Exception ex)
            {

                //throw;
            }


            return cc;
        }

        public static cestaMetrica getInfMetricBasket(bool Thread = false)
        {
            cestaMetrica cm = new cestaMetrica();

            try
            {

                StringBuilder stb = new StringBuilder();
                stb.Append("SELECT METRIC_MIN, GROUPID FROM GDA_HISTORY_INDICATOR_GROUP (NOLOCK) WHERE INDICATOR_ID = 10000012 AND DELETED_AT IS NULL ");

                //stb.AppendFormat(" {0} ", orderBy);

                using (SqlConnection connection = new SqlConnection(Database.retornaConn(Thread)))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.CommandTimeout = 60;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader["GROUPID"].ToString() == "1")
                                {
                                    cm.min1 = double.Parse(reader["METRIC_MIN"].ToString());
                                }
                                else if (reader["GROUPID"].ToString() == "2")
                                {
                                    cm.min2 = double.Parse(reader["METRIC_MIN"].ToString());
                                }
                                else if (reader["GROUPID"].ToString() == "3")
                                {
                                    cm.min3 = double.Parse(reader["METRIC_MIN"].ToString());
                                }
                                else if (reader["GROUPID"].ToString() == "4")
                                {
                                    cm.min4 = double.Parse(reader["METRIC_MIN"].ToString());
                                }

                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception)
            {

                throw;
            }


            return cm;
        }

        public static List<indicatorAccessMetrica> getInfMetricAccess(string dtInicial, string dtFinal, bool Thread = false)
        {
            List<indicatorAccessMetrica> listAccess = new List<indicatorAccessMetrica>();

            try
            {

                StringBuilder stb = new StringBuilder();
                stb.Append($"DECLARE @DATAINICIO DATETIME = '{dtInicial}'; ");
                stb.Append($"DECLARE @DATAFINAL DATETIME = '{dtFinal}'; ");
                stb.Append("SELECT G.STARTED_AT, G.ENDED_AT, GOAL, GOAL_NIGHT, GOAL_LATENIGHT,  ");
                stb.Append("METRIC_MIN, METRIC_MIN_NIGHT, METRIC_MIN_LATENIGHT, MONETIZATION,  ");
                stb.Append("MONETIZATION_NIGHT, MONETIZATION_LATENIGHT, G.SECTOR_ID, G.GROUPID  ");
                stb.Append("FROM GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS G ");
                stb.Append("INNER JOIN GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) AS I ON I.STARTED_AT = G.STARTED_AT AND I.DELETED_AT IS NULL ");
                stb.Append("AND I.ENDED_AT = G.ENDED_AT AND I.SECTOR_ID = G.SECTOR_ID AND I.INDICATOR_ID = G.INDICATOR_ID ");
                stb.Append("WHERE G.INDICATOR_ID = 10000013 ");
                stb.Append("AND G.DELETED_AT IS NULL  ");
                stb.Append("AND (( ");
                stb.Append("@DATAINICIO >= CONVERT(DATE,G.STARTED_AT) AND ");
                stb.Append("@DATAINICIO <= CONVERT(DATE,G.ENDED_AT) ");
                stb.Append(") ");
                stb.Append("OR ( ");
                stb.Append("@DATAFINAL >= CONVERT(DATE,G.STARTED_AT) AND ");
                stb.Append("@DATAFINAL <= CONVERT(DATE,G.ENDED_AT) ");
                stb.Append(") ");
                stb.Append("OR ");
                stb.Append("@DATAINICIO <= CONVERT(DATE,G.STARTED_AT) AND ");
                stb.Append("@DATAFINAL >= CONVERT(DATE,G.ENDED_AT) ");
                stb.Append(") ");

                //stb.AppendFormat(" {0} ", orderBy);

                using (SqlConnection connection = new SqlConnection(Database.retornaConn(Thread)))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.CommandTimeout = 60;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                indicatorAccessMetrica access = new indicatorAccessMetrica();

                                access.dateStart = Convert.ToDateTime(reader["STARTED_AT"].ToString());
                                access.dateEnd = Convert.ToDateTime(reader["ENDED_AT"].ToString());
                                access.goal = Convert.ToDouble(reader["GOAL"].ToString());
                                access.goalNight = Convert.ToDouble(reader["GOAL_NIGHT"].ToString());
                                access.goalLateNight = Convert.ToDouble(reader["GOAL_LATENIGHT"].ToString());
                                access.min = Convert.ToDouble(reader["METRIC_MIN"].ToString());
                                access.minNight = Convert.ToDouble(reader["METRIC_MIN_NIGHT"].ToString());
                                access.minLateNight = Convert.ToDouble(reader["METRIC_MIN_LATENIGHT"].ToString());
                                access.coin = Convert.ToDouble(reader["MONETIZATION"].ToString());
                                access.coinNight = Convert.ToDouble(reader["MONETIZATION_NIGHT"].ToString());
                                access.coinLateNight = Convert.ToDouble(reader["MONETIZATION_LATENIGHT"].ToString());
                                access.idSector = Convert.ToInt32(reader["SECTOR_ID"].ToString());
                                access.group = Convert.ToDouble(reader["GROUPID"].ToString());

                                listAccess.Add(access);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception)
            {

                throw;
            }


            return listAccess;
        }

        public class cestaMetrica
        {
            public double min1 { get; set; }
            public double min2 { get; set; }
            public double min3 { get; set; }
            public double min4 { get; set; }
        }

        public class indicatorAccessMetrica
        {
            public DateTime dateStart { get; set; }
            public DateTime dateEnd { get; set; }
            public double goal { get; set; }
            public double goalNight { get; set; }
            public double goalLateNight { get; set; }
            public double group { get; set; }
            public double min { get; set; }
            public double minNight { get; set; }
            public double minLateNight { get; set; }
            public double coin { get; set; }
            public double coinNight { get; set; }
            public double coinLateNight { get; set; }
            public int idSector { get; set; }
        }

        public static bool VerificaPostDeletado(int idPostReference)
        {
            bool retorno = false;
            // Query SQL para verificar se alguma palavra da tabela está contida no texto
            StringBuilder str = new StringBuilder();
            str.Append($"SELECT * FROM GDA_PERSONA_POSTS (NOLOCK)  ");
            str.Append($"WHERE IDGDA_PERSONA_POSTS = {idPostReference} ");
            str.Append($"AND DELETED_AT IS NULL  ");

            int count = 0;
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(str.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                retorno = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return retorno;
        }
        public static bool FiltroBlackList(string texto)
        {
            // Remover pontuações e converter o texto para minúsculas para comparação consistente
            string textoMinusculo = texto.ToLower();

            // Defina a consulta SQL para buscar todas as palavras da lista negra
            string query = "SELECT WORD FROM GDA_PERSONA_BLACKLIST (NOLOCK) WHERE DELETED_AT IS NULL";

            List<string> palavrasBlackList = new List<string>();

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Executar a consulta e obter as palavras da lista negra
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Adiciona a palavra em minúsculas à lista de palavras da blacklist
                                palavrasBlackList.Add(reader["WORD"].ToString().ToLower());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Lidar com exceções conforme necessário
                }
                finally
                {
                    connection.Close();
                }
            }

            // Verificar se alguma palavra da lista negra está contida no texto
            foreach (string palavra in palavrasBlackList)
            {
                if (textoMinusculo.ToString().ToUpper().IndexOf(palavra.ToString().ToUpper(), StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }


                //if (textoMinusculo.ToString().ToUpper() == palavra.ToString().ToUpper())
                //{
                //    return true;
                //}
            }

            return false;
        }
        //public static bool FiltroBlackList(string texto)
        //{
        //    // Dividindo o texto em palavras
        //    string[] palavras = texto.Split(new char[] { ' ', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

        //    // Query SQL para verificar se alguma palavra da tabela está contida no texto
        //    string query = $"SELECT COUNT(*) FROM GDA_PERSONA_BLACKLIST (NOLOCK) WHERE WORD IN ({string.Join(",", palavras.Select((_, index) => "@Palavra" + index))}) AND DELETED_AT IS NULL";

        //    int count = 0;
        //    using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
        //    {
        //        connection.Open();
        //        try
        //        {
        //            using (SqlCommand command = new SqlCommand(query.ToString(), connection))
        //            {
        //                for (int i = 0; i < palavras.Length; i++)
        //                {
        //                    command.Parameters.AddWithValue("@Palavra" + i, palavras[i]);
        //                }

        //                // Executando o comando e armazenando o resultado na variável 'count'
        //                count = (int)command.ExecuteScalar();
        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //        connection.Close();
        //    }

        //    if (count > 0)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }

        //}

        public static int QuantidadeMyAccounts(string accountPersona)
        {
            StringBuilder stb = new StringBuilder();
            string filter = "";
            int total = 0;
            string dtAg = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            if (accountPersona != "")
            {
                filter = " ";
            }
            stb.Append($"DECLARE @searchAccount NVARCHAR(100) = '{accountPersona}' ");
            stb.Append("SELECT COUNT(DISTINCT PCU.IDGDA_PERSONA_COLLABORATOR_USER) AS IDGDA_PERSONA_USER  ");
            stb.Append("FROM GDA_PERSONA_COLLABORATOR_USER  PCU (NOLOCK) ");
            stb.Append("INNER JOIN GDA_PERSONA_USER PU (NOLOCK) ON PU.IDGDA_PERSONA_USER = PCU.IDGDA_PERSONA_USER AND PU.DELETED_AT IS NULL  ");
            stb.Append("INNER JOIN GDA_PERSONA_USER_TYPE PUT (NOLOCK) ON PUT.IDGDA_PERSONA_USER_TYPE = PU.IDGDA_PERSONA_USER_TYPE ");
            stb.Append("INNER JOIN GDA_COLLABORATORS_DETAILS CD (NOLOCK) ON CD.IDGDA_COLLABORATORS = PCU.IDGDA_COLLABORATORS ");
            stb.Append("WHERE 1 = 1 ");
            stb.Append($"AND CD.CREATED_AT >= '{dtAg}' ");
            stb.Append($"AND CD.IDGDA_COLLABORATORS = TRY_CAST(@searchAccount AS INT) ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        // Executando o comando e armazenando o resultado na variável 'total'
                        total = (int)command.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            return total;
        }

        public static int QuantidadeAccounts(string accountPersona)
        {
            StringBuilder stb = new StringBuilder();
            string filter = "";
            int total = 0;
            string dtAg = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            if (accountPersona != "")
            {
                filter = "AND (PU.NAME LIKE '%' + @searchAccount + '%' OR PU.IDGDA_PERSONA_USER = TRY_CAST(@searchAccount AS INT) OR PCU.IDGDA_COLLABORATORS = TRY_CAST(@searchAccount AS INT) ) ";
            }
            stb.Append($"DECLARE @searchAccount NVARCHAR(100) = '{accountPersona}' ");
            stb.Append("SELECT COUNT(DISTINCT PCU.IDGDA_PERSONA_COLLABORATOR_USER) AS IDGDA_PERSONA_USER  ");
            stb.Append("FROM GDA_PERSONA_COLLABORATOR_USER  PCU (NOLOCK) ");
            stb.Append("INNER JOIN GDA_PERSONA_USER PU (NOLOCK) ON PU.IDGDA_PERSONA_USER = PCU.IDGDA_PERSONA_USER AND PU.DELETED_AT IS NULL  ");
            stb.Append("INNER JOIN GDA_PERSONA_USER_TYPE PUT (NOLOCK) ON PUT.IDGDA_PERSONA_USER_TYPE = PU.IDGDA_PERSONA_USER_TYPE ");
            //stb.Append("INNER JOIN GDA_COLLABORATORS_DETAILS CD (NOLOCK) ON CD.IDGDA_COLLABORATORS = PCU.IDGDA_COLLABORATORS ");
            stb.Append("WHERE   1 = 1 ");
            //stb.Append($"AND     CD.CREATED_AT >= '{dtAg}'  ");
            stb.Append($" {filter} ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        // Executando o comando e armazenando o resultado na variável 'total'
                        total = (int)command.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            return total;
        }

        public static int QuantidadeNotifications(string notification, string filters)
        {
            StringBuilder stb = new StringBuilder();
            string filter = "";
            int total = 0;
            string dtAg = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            if (notification != "")
            {
                filter = "AND (N.TITLE LIKE '%' + @searchNotification + '%' OR P.IDGDA_PERSONA_USER = TRY_CAST(@searchNotification AS INT) OR CU.IDGDA_COLLABORATORS = TRY_CAST(@searchNotification AS INT) ) ";
            }
            stb.Append($"DECLARE @searchNotification NVARCHAR(100) = '{notification}' ");

            stb.Append("SELECT COUNT(DISTINCT N.IDGDA_NOTIFICATION) AS IDGDA_NOTIFICATION  ");
            stb.AppendFormat("FROM GDA_NOTIFICATION (NOLOCK) N ");
            stb.AppendFormat("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER CU ON CU.IDGDA_PERSONA_USER = N.CREATED_BY  ");
            stb.AppendFormat("LEFT JOIN GDA_PERSONA_USER (NOLOCK) P ON P.IDGDA_PERSONA_USER = CU.IDGDA_PERSONA_USER AND P.IDGDA_PERSONA_USER_TYPE = 1  ");
            stb.AppendFormat("LEFT JOIN GDA_NOTIFICATION_FILES (NOLOCK) NF ON NF.IDGDA_NOTIFICATION = N.IDGDA_NOTIFICATION ");
            stb.AppendFormat("LEFT JOIN GDA_NOTIFICATION_TYPE (NOLOCK) NT ON NT.IDGDA_NOTIFICATION_TYPE = N.IDGDA_NOTIFICATION_TYPE ");
            stb.AppendFormat("WHERE N.DELETED_AT IS NULL ");
            stb.AppendFormat("AND N.IDGDA_NOTIFICATION_TYPE in (5,6,3,10,11,12,13) ");
            stb.AppendFormat(" {0} ", filter);

            //stb.Append("SELECT COUNT(DISTINCT N.IDGDA_NOTIFICATION) AS IDGDA_NOTIFICATION  ");
            //stb.Append("FROM GDA_NOTIFICATION (NOLOCK) N ");
            //stb.AppendFormat("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER CU ON CU.IDGDA_PERSONA_USER = N.CREATED_BY  ");
            //stb.AppendFormat("LEFT JOIN GDA_PERSONA_USER (NOLOCK) P ON P.IDGDA_PERSONA_USER = CU.IDGDA_PERSONA_USER AND P.IDGDA_PERSONA_USER_TYPE = 1  ");
            //stb.AppendFormat("LEFT JOIN GDA_NOTIFICATION_FILES (NOLOCK) NF ON NF.IDGDA_NOTIFICATION = N.IDGDA_NOTIFICATION ");
            //stb.AppendFormat("LEFT JOIN GDA_NOTIFICATION_TYPE (NOLOCK) NT ON NT.IDGDA_NOTIFICATION_TYPE = N.IDGDA_NOTIFICATION_TYPE ");
            //stb.AppendFormat("WHERE N.DELETED_AT IS NULL ");
            //stb.AppendFormat("AND N.IDGDA_NOTIFICATION_TYPE in (5,6,3,10,11,12,13) ");
            //stb.AppendFormat(" {0} ", filters);
            //stb.Append("AND N.DELETED_AT IS NULL ");
            //stb.Append($" {filter} ");
            //stb.AppendFormat("GROUP BY N.IDGDA_NOTIFICATION, N.CREATED_AT ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        // Executando o comando e armazenando o resultado na variável 'total'
                        total = (int)command.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            return total;
        }

        public static int QuantidadeQuiz(string filter)
        {
            StringBuilder stb = new StringBuilder();
            int total = 0;


            stb.Append("SELECT COUNT(0) ");
            stb.Append("FROM GDA_QUIZ (NOLOCK) Q ");
            stb.Append("LEFT JOIN GDA_PERSONA_USER (NOLOCK) PUC ON PUC.IDGDA_PERSONA_USER = Q.CREATED_BY ");
            stb.Append("LEFT JOIN GDA_PERSONA_USER (NOLOCK) PUD ON PUD.IDGDA_PERSONA_USER = Q.IDGDA_COLLABORATOR_DEMANDANT ");
            stb.Append("LEFT JOIN GDA_PERSONA_USER (NOLOCK) PUR ON PUR.IDGDA_PERSONA_USER = Q.IDGDA_COLLABORATOR_RESPONSIBLE ");
            stb.Append("LEFT JOIN GDA_QUIZ_SEND_FILTER (NOLOCK) QS ON QS.IDGDA_QUIZ = Q.IDGDA_QUIZ ");
            stb.Append("LEFT JOIN GDA_QUIZ_USER (NOLOCK) QUA ON Q.IDGDA_QUIZ = QUA.IDGDA_QUIZ AND QUA.ANSWERED = 1 ");
            stb.Append("WHERE Q.DELETED_AT IS NULL ");
            stb.Append($"{filter} ");
            //stb.Append("GROUP BY Q.IDGDA_QUIZ, Q.CREATED_AT ");
            //stb.Append("ORDER BY Q.CREATED_AT DESC ");


            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        // Executando o comando e armazenando o resultado na variável 'total'
                        total = (int)command.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            return total;
        }
        public static int QuantidadeFeedBack(string filter, int personaId)
        {
            StringBuilder sb = new StringBuilder();
            int total = 0;

            sb.Append($"DECLARE @ID INT; SET @ID = '{personaId}'; ");
            sb.Append($"SELECT COUNT(DS.CC) FROM (SELECT COUNT(0) AS CC ");
            sb.Append($"FROM GDA_FEEDBACK (NOLOCK) F ");
            sb.Append($"LEFT JOIN GDA_FEEDBACK_USER (NOLOCK) AS FU ON FU.IDGDA_FEEDBACK = F.IDGDA_FEEDBACK ");
            sb.Append($"LEFT JOIN GDA_PERSONA_USER (NOLOCK) AS PU ON PU.IDGDA_PERSONA_USER = FU.IDPERSONA_RECEIVED_BY ");
            sb.Append($"LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCU2 ON PCU2.IDGDA_PERSONA_USER = FU.IDPERSONA_RECEIVED_BY ");
            sb.Append($"LEFT JOIN ");
            sb.Append($"  (SELECT CDI2.IDGDA_COLLABORATORS, ");
            sb.Append($"          CDI2.MATRICULA_SUPERVISOR, ");
            sb.Append($"          CDI2.MATRICULA_COORDENADOR, ");
            sb.Append($"          CDI2.MATRICULA_GERENTE_II, ");
            sb.Append($"          CDI2.MATRICULA_GERENTE_I, ");
            sb.Append($"          CDI2.MATRICULA_DIRETOR, ");
            sb.Append($"          CDI2.MATRICULA_CEO, ");
            sb.Append($"          CDI2.CARGO, ");
            sb.Append($"          CDI2.SITE, CDI2.IDGDA_GROUP, ");
            sb.Append($"          CASE ");
            sb.Append($"              WHEN CDI2.IDGDA_SECTOR IS NULL THEN CDI2.IDGDA_SUBSECTOR ");
            sb.Append($"              ELSE CDI2.IDGDA_SECTOR ");
            sb.Append($"          END AS IDGDA_SECTOR ");
            sb.Append($"   FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CDI2 ");
            sb.Append($"   WHERE CONVERT(DATE, CDI2.CREATED_AT) >= CONVERT(DATE, DATEADD(DAY, -5, GETDATE())) )AS CDI2 ON CDI2.IDGDA_COLLABORATORS = PCU2.IDGDA_COLLABORATORS ");
            sb.Append($"LEFT JOIN GDA_HIERARCHY (NOLOCK) AS H ON H.LEVELNAME = CDI2.CARGO ");
            sb.Append($"LEFT JOIN GDA_SITE (NOLOCK) AS S ON S.SITE = CDI2.SITE ");
            sb.Append($"LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = CDI2.IDGDA_SECTOR ");
            sb.Append($"WHERE IDGDA_FEEDBACK_USER IS NOT NULL ");
            sb.Append($"AND (CDI2.IDGDA_COLLABORATORS = @ID ");
            sb.Append($"OR MATRICULA_SUPERVISOR = @ID ");
            sb.Append($"OR MATRICULA_COORDENADOR = @ID ");
            sb.Append($"OR MATRICULA_GERENTE_II = @ID ");
            sb.Append($"OR MATRICULA_GERENTE_I = @ID ");
            sb.Append($"OR MATRICULA_CEO = @ID) ");

            sb.Append($" {filter} GROUP BY FU.IDGDA_FEEDBACK_USER, F.CREATED_AT) AS DS ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        // Executando o comando e armazenando o resultado na variável 'total'
                        total = (int)command.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            return total;
        }
        public static int QuantidadeMyFeedBack(int personaID)
        {
            StringBuilder sb = new StringBuilder();
            int total = 0;

            sb.Append("SELECT ");
            sb.Append("COUNT(0) AS QTD ");
            sb.Append("FROM GDA_FEEDBACK_USER (NOLOCK) FU ");
            sb.Append("INNER JOIN GDA_PERSONA_USER (NOLOCK) AS PU ON PU.IDGDA_PERSONA_USER = FU.IDPERSONA_RECEIVED_BY ");
            sb.Append("INNER JOIN GDA_FEEDBACK (NOLOCK) AS F ON F.IDGDA_FEEDBACK = FU.IDGDA_FEEDBACK ");
            sb.Append("WHERE 1=1 ");
            sb.Append($"AND IDPERSONA_RECEIVED_BY = {personaID}  ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        // Executando o comando e armazenando o resultado na variável 'total'
                        total = (int)command.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            return total;
        }
        public static int QuantidadeActionEscalation(string filter, LoadLibraryActionEscalationController.InputLibraryActionEscalation inputModel)
        {
            StringBuilder sb = new StringBuilder();
            int total = 0;

            if (inputModel.AUTOMATIC == 1)
            {
                sb.Append("SELECT   ");
                sb.Append("  COUNT(IDGDA_ESCALATION_AUTOMATIC_SECTORS)  ");
                sb.Append("FROM GDA_ESCALATION_AUTOMATIC_SECTORS (NOLOCK) EA  ");
                sb.Append("LEFT JOIN GDA_INDICATOR (NOLOCK) AS IND ON IND.IDGDA_INDICATOR = EA.IDGDA_INDICATOR  ");
                sb.Append("WHERE EA.DELETED_AT IS NULL ");
                sb.Append($"{filter} ");
            }
            else
            {
                sb.Append("SELECT  ");
                sb.Append(" COUNT(IDGDA_ESCALATION_ACTION) ");
                sb.Append("FROM GDA_ESCALATION_ACTION (NOLOCK) EA ");
                sb.Append("LEFT JOIN GDA_INDICATOR (NOLOCK) AS IND ON IND.IDGDA_INDICATOR = EA.IDGDA_INDICATOR ");
                sb.Append("WHERE EA.DELETED_AT IS NULL ");
                sb.Append($"{filter} ");
            }



            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        // Executando o comando e armazenando o resultado na variável 'total'
                        total = (int)command.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            return total;
        }

        public static int QuantidadeOperationalCampaign(string filter)
        {
            StringBuilder sb = new StringBuilder();
            int total = 0;

            sb.Append("SELECT COUNT(0) FROM ");
            sb.Append("GDA_OPERATIONAL_CAMPAIGN (NOLOCK) ");
            sb.Append("WHERE DELETED_AT IS NULL ");
            sb.Append($"{filter} ");
            //stb.Append("GROUP BY Q.IDGDA_QUIZ, Q.CREATED_AT ");
            //stb.Append("ORDER BY Q.CREATED_AT DESC ");


            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        // Executando o comando e armazenando o resultado na variável 'total'
                        total = (int)command.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            return total;
        }
        public static int QuantidadeMyOperationalCampaign(string filter, int personaId)
        {
            StringBuilder sb = new StringBuilder();
            int total = 0;

            sb.Append("SELECT COUNT(0) ");
            sb.Append("FROM GDA_OPERATIONAL_CAMPAIGN (NOLOCK) OC ");
            sb.Append("INNER JOIN GDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT (NOLOCK) AS  OCUP ON OCUP.IDGDA_OPERATIONAL_CAMPAIGN = OC.IDGDA_OPERATIONAL_CAMPAIGN ");
            sb.Append("WHERE OC.DELETED_AT IS NULL ");
            sb.Append($"AND OCUP.IDGDA_PERSONA = {personaId} ");
            sb.Append($"{filter} ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        // Executando o comando e armazenando o resultado na variável 'total'
                        total = (int)command.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            return total;
        }

        public static int QuantidadeOperationalCampaignAvailable(string filter, int personaId)
        {
            StringBuilder sb = new StringBuilder();
            int total = 0;

            sb.Append("SELECT COUNT(0) ");
            sb.Append("FROM GDA_OPERATIONAL_CAMPAIGN OC ");
            sb.Append($"LEFT JOIN GDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT OCUP ON OC.IDGDA_OPERATIONAL_CAMPAIGN = OCUP.IDGDA_OPERATIONAL_CAMPAIGN AND OCUP.IDGDA_PERSONA = {personaId} ");
            sb.Append("WHERE OC.DELETED_AT IS NULL ");
            sb.Append($"AND OCUP.IDGDA_OPERATIONAL_CAMPAIGN IS NULL; ");
            sb.Append($"{filter} ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        // Executando o comando e armazenando o resultado na variável 'total'
                        total = (int)command.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            return total;
        }

        public static bool ValidaPoints(int idCampanha, int idIndicador, int personaId)
        {
            bool retorno = false;
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT REWARD_POINTS FROM GDA_OPERATIONAL_CAMPAIGN_MISIONS (NOLOCK) OCM ");
            sb.Append("INNER JOIN GDA_OPERATIONAL_CAMPAIGN_PONTUATION (NOLOCK) AS OCP ON OCP.IDGDA_OPERATIONAL_CAMPAIGN_PONTUATION = OCM.IDGDA_OPERATIONAL_CAMPAIGN_PONTUATION ");
            sb.Append("WHERE 1=1 ");
            sb.Append($"AND OCP.IDGDA_OPERATIONAL_CAMPAIGN = {idCampanha} ");
            sb.Append($"AND OCP.IDGDA_INDICATOR = {idIndicador} ");
            sb.Append($"AND OCM.IDGDA_PERSONA = {personaId} ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                retorno = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            return retorno;

        }

        public static void InserirPoints(int idCampanha, int idIndicador, int personaId)
        {
            StringBuilder sb = new StringBuilder();
            //Inserir a Pontuacao do colaborador
            sb.Append("INSERT INTO GDA_OPERATIONAL_CAMPAIGN_MISIONS (IDGDA_PERSONA, IDGDA_OPERATIONAL_CAMPAIGN_PONTUATION, CREATED_AT) ");
            sb.Append("VALUES ");
            sb.Append($"('{personaId}', SELECT IDGDA_OPERATIONAL_CAMPAIGN_PONTUATION FROM GDA_OPERATIONAL_CAMPAIGN_PONTUATION (NOLOCK) WHERE IDGDA_OPERATIONAL_CAMPAIGN = {idCampanha} AND IDGDA_INDICATOR = {idIndicador}, GETDATE()) ");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            //Pegar a quantidade de Pontos
            int RewardPoints = 0;
            sb.Clear();
            sb.Append("SELECT REWARD_POINTS FROM GDA_OPERATIONAL_CAMPAIGN_MISIONS (NOLOCK) OCM ");
            sb.Append("INNER JOIN GDA_OPERATIONAL_CAMPAIGN_PONTUATION (NOLOCK) AS OCP ON OCP.IDGDA_OPERATIONAL_CAMPAIGN_PONTUATION = OCM.IDGDA_OPERATIONAL_CAMPAIGN_PONTUATION ");
            sb.Append("WHERE 1=1 ");
            sb.Append($"AND OCP.IDGDA_OPERATIONAL_CAMPAIGN = {idCampanha} ");
            sb.Append($"AND OCP.IDGDA_INDICATOR = {idIndicador} ");
            sb.Append($"AND OCM.IDGDA_PERSONA = {personaId} ");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                RewardPoints = int.Parse(reader["REWARD_POINTS"].ToString());
                            }
                        }

                    }

                    // Atualizar Pontos do colaborador
                    sb.Clear();
                    sb.Append($"UPDATE GDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT SET VALUE = VALUE + {RewardPoints} WHERE IDGDA_OPERATIONAL_CAMPAIGN = {idCampanha} AND IDGDA_PERSONA = {personaId} ");
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            command.ExecuteNonQuery();
                        }

                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }


        }
        public static void InserirEliminacao(int idCampanha, int idIndicador, int personaId)
        {
            StringBuilder sb = new StringBuilder();
            int total = 0;

            //Pegar Id do porque está sendo eliminado
            int idOperationElimination = 0;
            sb.Append("SELECT IDGDA_OPERATIONAL_CAMPAIGN_ELIMINATION  FROM GDA_OPERATIONAL_CAMPAIGN_ELIMINATION (NOLOCK) ");
            sb.Append($"WHERE IDGDA_OPERATIONAL_CAMPAIGN = {idCampanha} ");
            sb.Append($"AND IDGDA_INDICATOR = {idIndicador} ");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                idOperationElimination = int.Parse(reader["IDGDA_OPERATIONAL_CAMPAIGN_ELIMINATION"].ToString());
                            }
                        }

                    }

                    // Atualizar Eliminação do colaborador
                    sb.Clear();
                    sb.Append($"UPDATE GDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT SET IDGDA_OPERATIONAL_CAMPAIGN_ELIMINATION = {idOperationElimination}, ELIMINATED_AT = GETDATE() ");
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            command.ExecuteNonQuery();
                        }

                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }


        }
        public static void EnvioNotCampanha(int idCampanha, int idIndicador, double percentual, bool TipoNot)
        {

            int personaid = 0;
            int IdNotification = 0;
            string campanha = "";
            string Indicador = Funcoes.VerificaIndicador(idIndicador);

            //Pegar o IdPeronsa responsavel pela Campanha
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT CREATED_BY, NAME  FROM GDA_OPERATIONAL_CAMPAIGN (NOLOCK) ");
            sb.Append($"WHERE IDGDA_OPERATIONAL_CAMPAIGN  ={idCampanha} ");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                personaid = int.Parse(reader["CREATED_BY"].ToString());
                                campanha = reader["NAME"].ToString();
                            }
                        }

                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            string DescriptionNotification = TipoNot == true ? $"O indicador {Indicador} teve um aumento de {percentual}% em relação ao início da campanha de nome: {campanha}." : $"O indicador {Indicador} teve uma baixa de {percentual}% em relação ao início da campanha de nome: {campanha}.";

            //Criar notifica~ção
            sb.Clear();
            sb.Append("INSERT INTO GDA_NOTIFICATION  ");
            sb.Append("(IDGDA_NOTIFICATION_TYPE, TITLE, NOTIFICATION, CREATED_AT, CREATED_BY, ACTIVE ) ");
            sb.Append("VALUES ");
            sb.Append($"(3,'Campanha', '{DescriptionNotification}', GETDATE(), 0, 1 ) ");
            sb.Append("SELECT  @@IDENTITY AS 'IDGDA_NOTIFICATION' ");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                IdNotification = Convert.ToInt32(reader["IDGDA_NOTIFICATION"].ToString());
                            }
                        }
                    }

                    sb.Clear();
                    sb.Append("UPDATE GDA_NOTIFICATION SET ");
                    sb.Append("SENDED_AT = GETDATE() ");
                    sb.Append($"WHERE IDGDA_NOTIFICATION = {IdNotification} ");
                    using (SqlCommand commandSelect = new SqlCommand(sb.ToString(), connection))
                    {
                        commandSelect.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {

                }


            }

            //Agrupamento
            List<infsNotification> infNot = SendNotificationController.BancoNotification.getInfsNotification(IdNotification);

            List<infsNotification> infNot2 = infNot
            .GroupBy(d => new { d.codNotification })
          .Select(group => new infsNotification
          {
              codNotification = group.Key.codNotification,
              idUserSend = group.First().idUserSend,
              urlUserSend = group.First().urlUserSend,
              nameUserSend = group.First().nameUserSend,
              message = group.First().message,
              file = "",
              files = group.Select(dd => dd.file).Distinct().Select(link => new urlFiles { url = link }).ToList() // files = group.Select(item => new filesListPosts { url = item.linkFile }).ToList()
          })
          .ToList();

            //Inserir No Banco
            int sendId = SendNotificationController.BancoNotification.InsertNotificationForUser(IdNotification, personaid);

            //Envia Notificação
            messageReturned msgInput = new messageReturned();
            msgInput.type = "Notification";
            msgInput.data = new dataMessage();
            msgInput.data.idUserReceive = personaid;
            msgInput.data.idNotificationUser = sendId;
            msgInput.data.idNotification = Convert.ToInt32(IdNotification);
            msgInput.data.idUserSend = infNot2.First().idUserSend;
            msgInput.data.urlUserSend = infNot2.First().urlUserSend;
            msgInput.data.nameUserSend = infNot2.First().nameUserSend;
            msgInput.data.message = infNot2.First().message;
            msgInput.data.urlFilesMessage = infNot2.First().files;
            Startup.messageQueue.Enqueue(msgInput);
        }
        public static void RankearCampanha(int idCampanha)
        {
            List<int> listRankUser = new List<int>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT IDGDA_PERSONA  ");
            sb.Append("FROM GDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT NOLOCK  ");
            sb.Append($"WHERE IDGDA_OPERATIONAL_CAMPAIGN = {idCampanha} ");
            sb.Append($"ORDER BY VALUE DESC ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listRankUser.Add(int.Parse(reader["IDGDA_PERSONA"].ToString()));
                            }
                        }
                    }

                    for (int i = 0; i < listRankUser.Count; i++)
                    {
                        int idPersona = listRankUser[i];
                        int newPosition = i + 1;

                        // Atualiza a posição na tabela GDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT
                        string updateQuery = $"UPDATE GDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT SET POSITION = {newPosition} " +
                                             $"WHERE IDGDA_OPERATIONAL_CAMPAIGN = {idCampanha} AND IDGDA_PERSONA = {idPersona}";

                        using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                        {
                            updateCommand.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
        }
        public static void PagamentoAutomaticoCampanha()
        {
            //LISTAR A CAMPANHA AUTOMATICA QUE JA FOI FINALIZADA POREM AINDA NÃO PAGA
            List<int> listCampanha = new List<int>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT DISTINCT(OC.IDGDA_OPERATIONAL_CAMPAIGN) FROM GDA_OPERATIONAL_CAMPAIGN (NOLOCK) OC  ");
            sb.Append("LEFT JOIN GDA_OPERATIONAL_CAMPAIGN_AWARD (NOLOCK) AS OCA ON OCA.IDGDA_OPERATIONAL_CAMPAIGN = OC.IDGDA_OPERATIONAL_CAMPAIGN  ");
            sb.Append($"WHERE OCA.IDGDA_OPERATIONAL_CAMPAIGN_AWARD_OPTION_PAY = 1  AND  OC.PAID IS NULL ");
            //sb.Append($"AND OC.ENDED_AT < GETDATE() ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listCampanha.Add(int.Parse(reader["IDGDA_OPERATIONAL_CAMPAIGN"].ToString()));
                            }
                        }
                    }

                    for (int i = 0; i < listCampanha.Count; i++)
                    {
                        //PEGAR AS INFORMAÇÕES DE PAGAMENTO DA CAMPANHA
                        string NAMECAMPANHA = "";
                        int IDGDA_OPERATIONAL_CAMPAIGN = 0;
                        int IDGDA_PRODUCT = 0;
                        int RANKING = 0;
                        int MIN_PONTUATION = 0;
                        int VALUE_COINS = 0;
                        int QUANTITY_PRODUCT = 0;
                        sb.Clear();
                        sb.Append("SELECT IDGDA_OPERATIONAL_CAMPAIGN_AWARD, ");
                        sb.Append("	   OC.NAME, ");
                        sb.Append("	   OCA.IDGDA_OPERATIONAL_CAMPAIGN, ");
                        sb.Append("	   IDGDA_PRODUCT,  ");
                        sb.Append("	   RANKING,  ");
                        sb.Append("	   MIN_PONTUATION,  ");
                        sb.Append("	   VALUE_COINS, ");
                        sb.Append("	   QUANTITY_PRODUCT  ");
                        sb.Append("FROM GDA_OPERATIONAL_CAMPAIGN_AWARD (NOLOCK) OCA ");
                        sb.Append("LEFT JOIN GDA_OPERATIONAL_CAMPAIGN (NOLOCK) OC ON OC.IDGDA_OPERATIONAL_CAMPAIGN = OCA.IDGDA_OPERATIONAL_CAMPAIGN ");
                        sb.Append($"WHERE OCA.IDGDA_OPERATIONAL_CAMPAIGN = {i} ");

                        using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    NAMECAMPANHA = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "";
                                    IDGDA_OPERATIONAL_CAMPAIGN = reader["IDGDA_OPERATIONAL_CAMPAIGN"] != DBNull.Value ? int.Parse(reader["IDGDA_OPERATIONAL_CAMPAIGN"].ToString()) : 0;
                                    IDGDA_PRODUCT = reader["IDGDA_PRODUCT"] != DBNull.Value ? int.Parse(reader["IDGDA_PRODUCT"].ToString()) : 0;
                                    RANKING = reader["RANKING"] != DBNull.Value ? int.Parse(reader["RANKING"].ToString()) : 0;
                                    MIN_PONTUATION = reader["MIN_PONTUATION"] != DBNull.Value ? int.Parse(reader["MIN_PONTUATION"].ToString()) : 0;
                                    VALUE_COINS = reader["VALUE_COINS"] != DBNull.Value ? int.Parse(reader["VALUE_COINS"].ToString()) : 0;
                                    QUANTITY_PRODUCT = reader["QUANTITY_PRODUCT"] != DBNull.Value ? int.Parse(reader["QUANTITY_PRODUCT"].ToString()) : 0;
                                }
                            }
                        }
                        //VERIFICAR QUAL TIPO DE PAGAMENTO SERÁ FEITO

                        //PAGAMENTO POR MOEDAS
                        if (VALUE_COINS != 0 || IDGDA_PRODUCT == 0 || QUANTITY_PRODUCT == 0)
                        {
                            // PAGAR POR RANKING
                            if (RANKING != 0 || MIN_PONTUATION == 0)
                            {
                                Funcoes.PagamentoMoedas(i, NAMECAMPANHA, VALUE_COINS, RANKING, true);
                            }
                            // PAGAR POR PONTUACAO MINIMA
                            else
                            {
                                Funcoes.PagamentoMoedas(i, NAMECAMPANHA, VALUE_COINS, MIN_PONTUATION, false);
                            }
                        }

                        //PAGAMENTO POR PRODUTO
                        if (IDGDA_PRODUCT != 0 || QUANTITY_PRODUCT != 0 || VALUE_COINS != 0)
                        {
                            if (RANKING != 0 || MIN_PONTUATION == 0)
                            {
                                Funcoes.PagamentoProdutos(i, NAMECAMPANHA, IDGDA_PRODUCT, QUANTITY_PRODUCT, VALUE_COINS, RANKING, true);
                            }
                            else
                            {
                                Funcoes.PagamentoProdutos(i, NAMECAMPANHA, IDGDA_PRODUCT, QUANTITY_PRODUCT, VALUE_COINS, MIN_PONTUATION, false);
                            }
                        }

                        //Marcacao na campanha
                        Funcoes.marcacaoPagamentoCampanha(i);

                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
        }


        public static void PagamentoMoedas(int idCampanha, string nomeCampanha, int MoedasGanhas, int Filter, bool ValidadorTipoPagamento)
        {
            StringBuilder sb = new StringBuilder();
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    // PAGAR POR RANKING
                    if (ValidadorTipoPagamento == true)
                    {
                        sb.Append("SELECT PCU.IDGDA_COLLABORATORS, OCUP.IDGDA_PERSONA FROM GDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT (NOLOCK) OCUP ");
                        sb.Append("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCU ON PCU.IDGDA_PERSONA_USER = OCUP.IDGDA_PERSONA ");
                        sb.Append($"WHERE OCUP.IDGDA_OPERATIONAL_CAMPAIGN = {idCampanha} AND POSITION = {Filter} WIN = 0 ");
                    }
                    // PAGAR POR PONTUACAO MINIMA
                    else
                    {
                        sb.Append("SELECT PCU.IDGDA_COLLABORATORS, OCUP.IDGDA_PERSONA FROM GDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT (NOLOCK) OCUP ");
                        sb.Append("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCU ON PCU.IDGDA_PERSONA_USER = OCUP.IDGDA_PERSONA ");
                        sb.Append($"WHERE OCUP.IDGDA_OPERATIONAL_CAMPAIGN = {idCampanha} AND  VALUE >= {Filter} AND WIN = 0  ");
                    }
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                int personaid = reader["IDGDA_PERSONA"] != DBNull.Value ? int.Parse(reader["IDGDA_PERSONA"].ToString()) : 0;
                                int collaboratoid = reader["IDGDA_COLLABORATORS"] != DBNull.Value ? int.Parse(reader["IDGDA_COLLABORATORS"].ToString()) : 0;

                                //INSERE MOEDAS GANHAS AO COLABORADOR
                                Funcoes.InsertCheckingAccount(collaboratoid, MoedasGanhas, nomeCampanha);

                                StringBuilder Update = new StringBuilder();
                                Update.Append("UPDATE GDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT SET WIN = 1 ");
                                Update.Append($"WHERE IDGDA_PERSONA = {personaid} AND IDGDA_OPERATIONAL_CAMPAIGN = {idCampanha} ");
                                using (SqlCommand command2 = new SqlCommand(Update.ToString(), connection))
                                {
                                    command2.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
        }
        public static void InsertCheckingAccount(int collaboratorId, int moedasGanhas, string nomeCampanha)
        {
            StringBuilder sb = new StringBuilder();
            //Inserir moedas ao colaborador
            sb.Append("INSERT INTO GDA_CHECKING_ACCOUNT  ");
            sb.Append("(input,balance, collaborator_id, reason, created_at, result_date) ");
            sb.Append("VALUES ");
            sb.Append("( ");
            sb.Append($"{moedasGanhas}, ");
            sb.Append("(SELECT TOP(1) balance ");
            sb.Append("FROM GDA_CHECKING_ACCOUNT (NOLOCK)  ");
            sb.Append($"WHERE collaborator_id = {collaboratorId} ");
            sb.Append("ORDER BY created_at DESC, Id DESC), ");
            sb.Append($"{collaboratorId}, ");
            sb.Append($"'Pagamento Campanha: {nomeCampanha}', ");
            sb.Append("GETDATE(), ");
            sb.Append("GETDATE()) ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
        }

        public static void marcacaoPagamentoCampanha(int idCampaign)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"UPDATE GDA_OPERATIONAL_CAMPAIGN SET PAID = 1 WHERE IDGDA_OPERATIONAL_CAMPAIGN = {idCampaign} ");
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                {
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public static void PagamentoProdutos(int idCampanha, string nomeCampanha, int idgdaProduto, int qtdProduto, int valorproduto, int Filter, bool ValidadorTipoPagamento)
        {
            StringBuilder sb = new StringBuilder();
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    // PAGAR POR RANKING
                    if (ValidadorTipoPagamento == true)
                    {
                        sb.Append("SELECT PCU.IDGDA_COLLABORATORS, OCUP.IDGDA_PERSONA FROM GDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT (NOLOCK) OCUP ");
                        sb.Append("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCU ON PCU.IDGDA_PERSONA_USER = OCUP.IDGDA_PERSONA ");
                        sb.Append($"WHERE OCUP.IDGDA_OPERATIONAL_CAMPAIGN = {idCampanha} AND POSITION = {Filter} WIN = 0 ");
                    }
                    // PAGAR POR PONTUACAO MINIMA
                    else
                    {
                        sb.Append("SELECT PCU.IDGDA_COLLABORATORS, OCUP.IDGDA_PERSONA FROM GDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT (NOLOCK) OCUP ");
                        sb.Append("EFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCU ON PCU.IDGDA_PERSONA_USER = OCUP.IDGDA_PERSONA ");
                        sb.Append($"HERE OCUP.IDGDA_OPERATIONAL_CAMPAIGN = {idCampanha} AND  VALUE >= {Filter} WIN = 0  ");
                    }
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                int personaid = reader["IDGDA_PERSONA"] != DBNull.Value ? int.Parse(reader["IDGDA_PERSONA"].ToString()) : 0;
                                int collaboratoid = reader["IDGDA_COLLABORATORS"] != DBNull.Value ? int.Parse(reader["IDGDA_COLLABORATORS"].ToString()) : 0;

                                //INSERE PRODUTO GANHO AO COLABORADOR
                                Funcoes.InsertOrderProdutc(collaboratoid, idgdaProduto, qtdProduto, valorproduto, nomeCampanha);

                                StringBuilder Update = new StringBuilder();
                                Update.Append("UPDATE GDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT SET WIN = 1 ");
                                Update.Append($"WHERE IDGDA_PERSONA = {personaid} AND IDGDA_OPERATIONAL_CAMPAIGN = {idCampanha} ");
                                using (SqlCommand command2 = new SqlCommand(Update.ToString(), connection))
                                {
                                    command2.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
        }

        public static void InsertOrderProdutc(int collaboratorId, int idgdaProduto, int qtdProduto, int valorProduto, string nomeCampanha)
        {
            int idOrder = 0;
            StringBuilder sb = new StringBuilder();
            //Inserir o pedido
            sb.Append("INSERT INTO GDA_ORDER  ");
            sb.Append("(ORDER_BY, GDA_ORDER_STATUS_IDGDA_ORDER_STATUS, CREATED_AT, REASON_PURCHASE) ");
            sb.Append("VALUES ");
            sb.Append($"{collaboratorId}, 1, GETDATE(), 'Item ganho em campanha: {nomeCampanha}') ");
            sb.Append("SELECT  @@IDENTITY AS 'IDGDA_ORDER' ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        idOrder = command.ExecuteNonQuery();
                    }

                    sb.Clear();
                    sb.Append("INSERT INTO GDA_ORDER_PRODUCT ");
                    sb.Append("(AMOUNT, PRICE, GDA_ORDER_IDGDA_ORDER, GDA_PRODUCT_IDGDA_PRODUCT, CREATED_AT, GDA_STOCK_IDGDA_STOCK,ORDER_PRODUCT_STATUS) ");
                    sb.Append("VALUES ");
                    sb.Append($"({qtdProduto}, {valorProduto}, {idOrder}, {idgdaProduto}, GETDATE(), 0, 1) ");
                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
        }

    }
}
