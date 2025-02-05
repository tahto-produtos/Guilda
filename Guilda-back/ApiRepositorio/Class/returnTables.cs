using ApiRepositorio.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using static ApiRepositorio.Controllers.IntegracaoAPIResultController;
using System.Configuration;
using OfficeOpenXml.Style;
using System.Security.Cryptography;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static ApiRepositorio.Controllers.ScoreInputController;
using OfficeOpenXml.Filter;
using DocumentFormat.OpenXml.Bibliography;
using ThirdParty.Json.LitJson;
using DocumentFormat.OpenXml.ExtendedProperties;
using System.Globalization;
using System.Runtime.Remoting;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Office.Word;
using ApiRepositorio.Controllers;

namespace ApiC.Class
{
    public class colaboradoresTrabalhados
    {
        public int id { get; set; }
        public int trabalhado { get; set; }
        public DateTime data { get; set; }
    }
    public class colaboradoresEscalados
    {
        public int id { get; set; }
        public int escalado { get; set; }
        public DateTime data { get; set; }
    }
    public class score
    {
        public int indicatorId { get; set; }
        public string name { get; set; }
        public string Score { get; set; }
    }
    public class groups
    {
        public int id { get; set; }
        public string name { get; set; }
        public string alias { get; set; }
        public string image { get; set; }

    }
    public class period
    {
        public int id { get; set; }
        public string name { get; set; }

    }
    public class homeFloor
    {
        public int id { get; set; }
        public string name { get; set; }

    }
    public class group
    {
        public int id { get; set; }
        public string name { get; set; }

    }
    public class client
    {
        public int id { get; set; }
        public string name { get; set; }

    }
    public class hierarchy
    {
        public int id { get; set; }
        public string name { get; set; }

    }
    public class indicator
    {
        public int id { get; set; }
        public string name { get; set; }

    }
    public class permit
    {
        public int id { get; set; }
        public string action { get; set; }
        public string resource { get; set; }
        public Boolean active { get; set; }
    }
    public class sector
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool isSector { get; set; }

    }

    public class sectorReturn
    {
        public int id { get; set; }
        public string name { get; set; }
        public int level { get; set; }
        public int sector { get; set; }
        public int subSector { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? deletedAt { get; set; }

    }

    public class subSector
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool isSector { get; set; }

    }
    public class basket
    {
        public int metric_min { get; set; }
        public int group_id { get; set; }

    }
    public class stock
    {
        public int IDGDA_STOCK { get; set; }
        public string DESCRIPTION { get; set; }
        public string CITY { get; set; }
        public string GDA_ATRIBUTES { get; set; }
        public string CAMPAIGN { get; set; }
    }
    public class product
    {
        public int IDGDA_PRODUCT { get; set; }
        public string DESCRIPTION { get; set; }
        public string CODE { get; set; }
        public string REGISTERED_BY { get; set; }
        public string PRICE { get; set; }
        public string TYPE { get; set; }
        public string COMERCIAL_NAME { get; set; }
        public string VALIDITY_DATE { get; set; }
    }
    public class city
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class state
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class site
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class status
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class personaDetails
    {
        public int ID { get; set; }
        public string NAME { get; set; }
        public string AGE { get; set; }
        public string CITY { get; set; }
        public string WHOARE { get; set; }
        public string YOURMOTIVATIONS { get; set; }
        public string GOALS { get; set; }
        public string NEEDS { get; set; }
        public string DIFFICULTIES { get; set; }
        public string STATUS { get; set; }

    }
    public class collaborators
    {
        public int ID { get; set; }
        public string NAME { get; set; }
    }
    public class historychat
    {
        public string MESSAGE { get; set; }
        public string CREATED_AT { get; set; }
        public int IDGDA_CHAT_COLLABORATORS { get; set; }
    }
    public class metricBySector
    {
        public string SETOR { get; set; }
        public int CODGIP { get; set; }
        public int CODGIPSUB { get; set; }
        public string SUBSETOR { get; set; }
        public string INDICADOR { get; set; }
        public int CODINDICADOR { get; set; }
        public string TIPO { get; set; }
        public string TURNO { get; set; }
        public string META { get; set; }
        public string GRUPO { get; set; }
        public string META_MINIMA { get; set; }
        public string MOEDAS { get; set; }
        public string DATAINICIAL { get; set; }
        public string DATAFINAL { get; set; }

        public int STATUS { get; set; }
    }
    public class resultCollaborators
    {
        public int INDICADORID { get; set; }
        public string RESULTADO { get; set; }
        public string INDICADOR { get; set; }
        public string DATA { get; set; }
        public int MATRICULA { get; set; }
        public string FATOR { get; set; }
    }
    public class ScoreBySector
    {
        public string SETOR { get; set; }
        public int CODGIP { get; set; }
        public string INDICADOR { get; set; }
        public int CODINDICADOR { get; set; }
        public string SCORE { get; set; }
    }
    public class BlackList
    {
        public int IDGDA_BLACKLIST { get; set; }
        public string WORD { get; set; }
    }
    public class Hobby
    {
        public int IDGDA_PERSONA_USER_HOBBY { get; set; }
        public string HOBBY { get; set; }
    }
    public class Hobbies
    {
        public int IDGDA_HOBBIES { get; set; }
        public string HOBBY { get; set; }
    }
    public class PersonaUser
    {
        public string NOME { get; set; }
        public string BC { get; set; }
        public int IDADE { get; set; }
        public int FLAGTAHTO { get; set; }
        public int IDADE_CALCULADA { get; set; }
        public string NOME_SOCIAL { get; set; }
        public string FOTO { get; set; }
        public string QUEM_E { get; set; }
        public string MOTIVACOES { get; set; }
        public string OBJETIVO { get; set; }
        public List<Hobbies> HOBBIES { get; set; }
        public string EMAIL { get; set; }
        public string TELEFONE { get; set; }
        public string DATA_NASCIMENTO { get; set; }
        public string UF { get; set; }
        public string ID_UF { get; set; }
        public string CIDADE { get; set; }
        public string ID_CIDADE { get; set; }
        public string ID_SITE { get; set; }
        public string SITE { get; set; }

    }
    public class Accounts
    {
        public int IDGDA_PERSONA_USER { get; set; }
        public string NOME { get; set; }
        public string FOTO { get; set; }
        public string TIPO { get; set; }
        public bool FOLLOWED_BY_ME { get; set; }
    }
    public class Account
    {
        public int PERSONAID { get; set; }
        public int TOTALPAGES { get; set; }
        public List<Accounts> ACCOUNTS { get; set; }
    }
    public class Reaction
    {
        public int ID_POST_LIKE_REACTION { get; set; }
        public string NAME { get; set; }
        public string LINK_ICON { get; set; }
        public string LINK_ICON_SELECTED { get; set; }
    }
    public class Question
    {
        public int IDGDA_QUIZ_QUESTION { get; set; }
        public int IDGDA_QUIZ { get; set; }
        public int IDGDA_TYPE { get; set; }
        public int IDTYPE { get; set; }
        public string TYPE { get; set; }
        public string QUESTION { get; set; }
        public string URL_QUESTION { get; set; }
        public int TIME_ANSWER { get; set; }
        public string ORIENTATION { get; set; }
        public List<Answer> ANSWER { get; set; }
    }
    public class TypeQuestion
    {
        public int IDGDA_QUIZ_QUESTION_TYPE { get; set; }
        public string TYPE { get; set; }
    }
    public class Answer
    {
        public int IDGDA_QUIZ_QUESTION { get; set; }
        public int IDGDA_QUIZ_ANSWER { get; set; }
        public string QUESTION { get; set; }
        public int RIGHT_ANSWER { get; set; }
        public string URL { get; set; }
    }

    public class QuizVerify
    {
        public int IDGDA_QUIZ { get; set; }

        public int? IDGDA_NOTIFICATION { get; set; }
    }

    public class Quiz
    {
        public int IDGDA_QUIZ { get; set; }
        public string TITLE { get; set; }
        public int QTD_QUESTION { get; set; }
        public int QTD_ANSWER { get; set; }
        public double PERCENT { get; set; }
        public List<Question> QUESTION { get; set; }
    }
    public class MyQuiz
    {
        public int IDGDA_QUIZ_USER { get; set; }
        public int IDGDA_QUIZ { get; set; }
        public string TITLE { get; set; }
        public DateTime? STARTED_AT { get; set; }
        public DateTime? ENDED_AT { get; set; }
        public string CONCLUED { get; set; }
        public int REQUIRED { get; set; }

        public string STATUS { get; set; }

    }
    public class DayLogged
    {
        public string DAY { get; set; }
        public string ACRONYM { get; set; }
        public int LOGIN { get; set; }
        public string DATE { get; set; }
    }
    public class ListDayLoggeds
    {
        public string DATA { get; set; }
        public string TEMPO_LOGIN { get; set; }
    }
    public class MonetizationConfigPause
    {
        public int SELECT { get; set; }
        public string PAUSE_AT { get; set; }
        public int PAUSE { get; set; }
        public int REPROCESSED { get; set; }
    }
    public class MonetizationConfigDay
    {
        public int IDGDA_MONETIZATION_CONFIG { get; set; }
        public int DAYS { get; set; }
        public string CREATED_AT { get; set; }
        public int REPROCESSED { get; set; }
        public string STARTED_AT { get; set; }
        public string TYPE { get; set; }
        public int REFERER { get; set; }
    }
    public class MonetizationConfigType
    {
        public int IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE { get; set; }
        public string TYPE { get; set; }
    }

    public class ConfigNotificationExpire
    {
        public int IDGDA_CONFIG_NOTIFICATION { get; set; }
        public int TYPE_NOTIFICATION { get; set; }
        public int DAYS { get; set; }
    }
    public class returnTables
    {
        public static List<colaboradoresEscalados> colaboradoresEscalados(string dataInicial, string dataFinal)
        {
            List<colaboradoresEscalados> lColaboradoresEscalados = new List<colaboradoresEscalados>();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT COUNT(0) AS 'ESCALADO', ");
            sb.Append("       IDGDA_COLLABORATORS, ");
            sb.Append("       CREATED_AT ");
            sb.Append("FROM GDA_RESULT (NOLOCK) ");
            sb.Append("WHERE INDICADORID = -1 ");
            sb.AppendFormat("  AND CREATED_AT >= '{0}' ", dataInicial);
            sb.AppendFormat("  AND CREATED_AT <= '{0}' ", dataFinal);
            sb.Append("  AND RESULT = 1 ");
            sb.Append("  AND DELETED_AT IS NULL ");
            sb.Append("GROUP BY IDGDA_COLLABORATORS, ");
            sb.Append("         CREATED_AT ");


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
                                colaboradoresEscalados colaboradoresTrab = new colaboradoresEscalados();
                                colaboradoresTrab.id = int.Parse(reader["IDGDA_COLLABORATORS"].ToString());
                                colaboradoresTrab.escalado = int.Parse(reader["ESCALADO"].ToString());
                                colaboradoresTrab.data = DateTime.Parse(reader["CREATED_AT"].ToString());

                                lColaboradoresEscalados.Add(colaboradoresTrab);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return lColaboradoresEscalados;
        }
        public static List<colaboradoresTrabalhados> colaboradoresTrabalhados(string dataInicial, string dataFinal)
        {
            List<colaboradoresTrabalhados> lColaboradoresTrabalhados = new List<colaboradoresTrabalhados>();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT COUNT(0) AS 'TRABALHADO', ");
            sb.Append("       IDGDA_COLLABORATORS, ");
            sb.Append("       CREATED_AT ");
            sb.Append("FROM GDA_RESULT (NOLOCK) ");
            sb.Append("WHERE INDICADORID = 2 ");
            sb.AppendFormat("  AND CREATED_AT >= '{0}' ", dataInicial);
            sb.AppendFormat("  AND CREATED_AT <= '{0}' ", dataFinal);
            sb.Append("  AND RESULT = 0 ");
            sb.Append("  AND DELETED_AT IS NULL ");
            sb.Append(" GROUP BY IDGDA_COLLABORATORS, ");
            sb.Append("         CREATED_AT ");

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
                                colaboradoresTrabalhados colaboradoresTrab = new colaboradoresTrabalhados();
                                colaboradoresTrab.id = int.Parse(reader["IDGDA_COLLABORATORS"].ToString());
                                colaboradoresTrab.trabalhado = int.Parse(reader["TRABALHADO"].ToString());
                                colaboradoresTrab.data = DateTime.Parse(reader["CREATED_AT"].ToString());

                                lColaboradoresTrabalhados.Add(colaboradoresTrab);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return lColaboradoresTrabalhados;
        }
        public static List<groups> listGroups(string filter)
        {
            List<groups> lGroups = new List<groups>();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT G.ID, NAME, ALIAS, URL FROM GDA_GROUPS (NOLOCK) AS G ");
            sb.Append("INNER JOIN GDA_UPLOADS (NOLOCK) AS U ON G.UPLOADID = U.ID ");


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
                                groups group = new groups();
                                group.id = int.Parse(reader["ID"].ToString());
                                group.name = reader["NAME"].ToString();
                                group.alias = reader["ALIAS"].ToString();
                                group.image = reader["URL"].ToString();
                                lGroups.Add(group);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return lGroups;
        }
        public static List<indicator> listIndicatorBySector(string sector, bool monetize, string dtInicial, string dtFinal)
        {
            string filter = "";
            List<indicator> lInd = new List<indicator>();
            if (sector != "")
            {
                filter = $" AND S.SECTOR_ID IN ({sector}) ";
            }
            if (monetize == true)
            {
                filter = $" {filter} AND S.MONETIZATION > 0 ";
                //filter = $" {filter} AND CONVERT(DATE, S.STARTED_AT) <= CONVERT(DATE, GETDATE()) ";
                //filter = $" {filter} AND CONVERT(DATE,S.ENDED_AT) >= CONVERT(DATE, GETDATE()) ";
                if (dtInicial != null && dtInicial != "")
                {
                    filter = $" {filter} AND ((CONVERT(DATE, S.STARTED_AT) <= @DATAFINAL AND CONVERT(DATE, S.ENDED_AT) >= @DATAINICIAL) ";
                    filter = $" {filter} OR (@DATAINICIAL <= CONVERT(DATE, S.ENDED_AT) AND CONVERT(DATE, S.ENDED_AT) >= CONVERT(DATE, S.STARTED_AT))) ";
                }
            }

            StringBuilder sb = new StringBuilder();
            //sb.Append("SELECT I.NAME, S.INDICATOR_ID FROM GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) AS S ");
            //sb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS I ON I.IDGDA_INDICATOR = S.INDICATOR_ID ");
            //sb.AppendFormat("WHERE S.DELETED_AT IS NULL AND I.DELETED_AT IS NULL {0} ", filter);
            //sb.Append("GROUP BY I.NAME, S.INDICATOR_ID ");
            sb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtInicial);
            sb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFinal);
            sb.Append("SELECT I.NAME, ");
            sb.Append("       S.INDICATOR_ID ");
            sb.Append("FROM GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS S ");
            sb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS I ON I.IDGDA_INDICATOR = S.INDICATOR_ID ");
            sb.Append("WHERE (S.DELETED_AT IS NULL ");
            sb.Append("       AND I.DELETED_AT IS NULL ");
            sb.AppendFormat(" {0} ", filter);
            sb.Append("       ) ");
            sb.Append("  OR S.INDICATOR_ID = 10000012 OR S.INDICATOR_ID = 10000013 OR S.INDICATOR_ID = 10000014 ");
            sb.Append("GROUP BY I.NAME, ");
            sb.Append("         S.INDICATOR_ID ");


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
                                indicator sec = new indicator();
                                sec.id = int.Parse(reader["INDICATOR_ID"].ToString());
                                sec.name = reader["NAME"].ToString();
                                lInd.Add(sec);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return lInd;
        }

        public static List<subSector> listSubSectorsBySector(string sector, string dtInicial, string dtFinal, int collaboratorId, string sectorsIds)
        {
            List<subSector> lsec = new List<subSector>();
            StringBuilder sb = new StringBuilder();


            if (sectorsIds != "")
            {
                sector = sectorsIds;
            }

            //sb.Append("SELECT IDGDA_SUBSECTOR, SEC.NAME FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CD ");
            //sb.Append("INNER JOIN GDA_SECTOR (NOLOCK) AS SEC ON CD.IDGDA_SUBSECTOR = SEC.IDGDA_SECTOR ");
            //sb.Append($"WHERE CD.CREATED_AT >= '{dtInicial}' AND CD.CREATED_AT <= '{dtFinal}' ");
            //sb.Append("AND CD.IDGDA_SUBSECTOR IS NOT NULL ");
            //sb.Append($"AND CD.IDGDA_SECTOR = {sector} ");
            //sb.Append("GROUP BY CD.IDGDA_SUBSECTOR, SEC.NAME ");

            sb.Append("SELECT IDGDA_SUBSECTOR, SEC.NAME FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CD ");
            sb.Append("INNER JOIN GDA_SECTOR (NOLOCK) AS SEC ON CD.IDGDA_SUBSECTOR = SEC.IDGDA_SECTOR ");
            sb.Append($"INNER JOIN GDA_RESULT (NOLOCK) AS R ON R.IDGDA_COLLABORATORS = CD.IDGDA_COLLABORATORS AND R.CREATED_AT = CD.CREATED_AT ");
            sb.Append($"WHERE CD.CREATED_AT >= '{dtInicial}' AND CD.CREATED_AT <= '{dtFinal}' ");
            sb.Append("AND CD.IDGDA_SUBSECTOR IS NOT NULL ");
            sb.Append($"AND CD.IDGDA_SECTOR IN ({sector}) ");
            sb.Append($"AND (CD.IDGDA_COLLABORATORS = {collaboratorId} ");
            sb.Append($"OR CD.MATRICULA_SUPERVISOR = {collaboratorId} ");
            sb.Append($"OR CD.MATRICULA_COORDENADOR = {collaboratorId} ");
            sb.Append($"OR CD.MATRICULA_GERENTE_II = {collaboratorId} ");
            sb.Append($"OR CD.MATRICULA_GERENTE_I = {collaboratorId} ");
            sb.Append($"OR CD.MATRICULA_DIRETOR = {collaboratorId} ");
            sb.Append($"OR CD.MATRICULA_CEO = {collaboratorId}) ");
            sb.Append("GROUP BY CD.IDGDA_SUBSECTOR, SEC.NAME ");

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
                                subSector sec = new subSector();
                                sec.id = int.Parse(reader["IDGDA_SUBSECTOR"].ToString());
                                sec.name = reader["NAME"].ToString();
                                sec.isSector = false;
                                lsec.Add(sec);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return lsec;
        }

        public static List<sector> listSectorHierarchy(bool filtro, string sector, string codCola, string dtInicial, string dtFinal)
        {

            string filter = "";
            if (sector != null)
            {
                // filter = $" AND (NAME LIKE '%{sector}%' OR CONVERT(VARCHAR, SEC.IDGDA_SECTOR) LIKE '%{sector}%') ";
                //filter = $" AND (CONVERT(VARCHAR, SEC.IDGDA_SECTOR) LIKE '%{sector}%' OR NAME LIKE '%{sector}%') ";
                filter = $"AND (CONVERT(VARCHAR, GS.IDGDA_SECTOR) LIKE '%{sector}%' OR NAME LIKE '%{sector}%')";
            }

            List<sector> lsec = new List<sector>();
            StringBuilder sb = new StringBuilder();
            if (filtro == false)
            {
                sb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}'; ", dtInicial);
                sb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFinal);
                sb.AppendFormat("DECLARE @ID INT; SET @ID = {0}; ", codCola);
                sb.AppendFormat("SELECT DISTINCT GCD.IDGDA_SECTOR,  GS.NAME FROM GDA_COLLABORATORS_DETAILS AS GCD ");
                //sb.AppendFormat("SELECT DISTINCT(GCD.IDGDA_SECTOR), GS.NAME FROM GDA_COLLABORATORS_DETAILS AS GCD ");
                sb.AppendFormat("LEFT JOIN GDA_SECTOR AS GS ON GS.IDGDA_SECTOR = GCD.IDGDA_SECTOR ");
                sb.AppendFormat("WHERE  ");
                sb.AppendFormat("(GCD.IDGDA_COLLABORATORS = @ID OR ");
                sb.AppendFormat("GCD.MATRICULA_SUPERVISOR = @ID OR ");
                sb.AppendFormat("GCD.MATRICULA_COORDENADOR = @ID OR ");
                sb.AppendFormat("GCD.MATRICULA_GERENTE_II = @ID OR ");
                sb.AppendFormat("GCD.MATRICULA_GERENTE_I = @ID OR ");
                sb.AppendFormat("GCD.MATRICULA_DIRETOR = @ID OR ");
                sb.AppendFormat("GCD.MATRICULA_CEO = @ID)  ");
                sb.AppendFormat("AND GCD.CREATED_AT BETWEEN @DATAINICIAL  ");
                sb.AppendFormat("AND @DATAFINAL  ");
                sb.AppendFormat("AND GCD.IDGDA_SECTOR IS NOT NULL  ");
                sb.AppendFormat("AND GS.SECTOR = 1  ");
                sb.AppendFormat("AND GS.DELETED_AT IS NULL ");
                sb.AppendFormat("AND CARGO = 'AGENTE' {0}", filter);




                //sb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}';",dtInicial);
                //sb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}'; ", dtFinal);
                //sb.AppendFormat("DECLARE @ID INT; SET @ID = {0};", codCola);
                //sb.AppendFormat("SELECT DISTINCT(GCD.IDGDA_SECTOR), GS.NAME FROM GDA_COLLABORATORS_DETAILS AS GCD ");
                //sb.AppendFormat("LEFT JOIN GDA_SECTOR AS GS ON GS.IDGDA_SECTOR = GCD.IDGDA_SECTOR ");
                //sb.AppendFormat("WHERE (GCD.IDGDA_COLLABORATORS = @ID OR ");
                //sb.AppendFormat(" GCD.MATRICULA_COORDENADOR = @ID OR ");
                //sb.AppendFormat(" GCD.MATRICULA_GERENTE_II = @ID OR ");
                //sb.AppendFormat(" GCD.MATRICULA_GERENTE_I = @ID OR ");
                //sb.AppendFormat(" GCD.MATRICULA_DIRETOR = @ID) ");
                //sb.AppendFormat(" AND GCD.CREATED_AT BETWEEN @DATAINICIAL ");
                //sb.AppendFormat("AND @DATAFINAL ");
                //sb.AppendFormat("AND GCD.IDGDA_SECTOR IS NOT NULL ");
                //sb.AppendFormat("AND GS.SECTOR = 1 ");
                //sb.AppendFormat("AND CARGO = 'AGENTE' ");




                //sb.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}';", codCola);
                //sb.Append("DECLARE @DATEENV DATE; SET @DATEENV = CONVERT(DATE, DATEADD(DAY, -1, GETDATE())); ");
                //sb.Append(" ");
                //sb.Append("SELECT SEC.IDGDA_SECTOR, MAX(NAME) AS NAME ");
                //sb.Append("FROM GDA_COLLABORATORS_DETAILS (NOLOCK) CLD ");
                //sb.Append("INNER JOIN GDA_SECTOR (NOLOCK) SEC ON SEC.IDGDA_SECTOR = CLD.IDGDA_SECTOR AND SEC.SECTOR = 1 ");
                //sb.Append("WHERE (IDGDA_COLLABORATORS = @INPUTID OR  ");
                //sb.Append("	    MATRICULA_SUPERVISOR = @INPUTID OR ");
                //sb.Append("		MATRICULA_COORDENADOR = @INPUTID OR ");
                //sb.Append("		MATRICULA_GERENTE_II = @INPUTID OR ");
                //sb.Append("		MATRICULA_GERENTE_I = @INPUTID OR ");
                //sb.Append("		MATRICULA_DIRETOR = @INPUTID OR ");
                //sb.Append("		MATRICULA_CEO = @INPUTID)  ");
                //sb.Append("		AND CLD.CREATED_AT = @DATEENV AND CLD.CARGO = 'AGENTE' ");
                //sb.AppendFormat(" {0} ", filter);
                //sb.Append("		GROUP BY SEC.IDGDA_SECTOR ");

                //sb.Append("DECLARE @DATEENV DATE; SET @DATEENV = DATEADD(DAY, -1, GETDATE()) ");
                //sb.AppendFormat("DECLARE @INPUTID INT; SET @INPUTID = '{0}'; ", codCola);
                //sb.Append("            WITH HIERARCHYCTE AS  ");
                //sb.Append("              (SELECT IDGDA_HISTORY_HIERARCHY_RELATIONSHIP,  ");
                //sb.Append("                      CONTRACTORCONTROLID,  ");
                //sb.Append("                      PARENTIDENTIFICATION,  ");
                //sb.Append("                      IDGDA_COLLABORATORS,  ");
                //sb.Append("                      IDGDA_HIERARCHY,  ");
                //sb.Append("                      CREATED_AT,  ");
                //sb.Append("                      DELETED_AT,  ");
                //sb.Append("                      TRANSACTIONID,  ");
                //sb.Append("                      LEVELWEIGHT, DATE, LEVELNAME  ");
                //sb.Append("               FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP (NOLOCK)  ");
                //sb.Append("               WHERE IDGDA_COLLABORATORS = @INPUTID  ");
                //sb.Append("                 AND [DATE] = @DATEENV  ");
                //sb.Append("               UNION ALL SELECT H.IDGDA_HISTORY_HIERARCHY_RELATIONSHIP,  ");
                //sb.Append("                                H.CONTRACTORCONTROLID,  ");
                //sb.Append("                                H.PARENTIDENTIFICATION,  ");
                //sb.Append("                                H.IDGDA_COLLABORATORS,  ");
                //sb.Append("                                H.IDGDA_HIERARCHY,  ");
                //sb.Append("                                H.CREATED_AT,  ");
                //sb.Append("                                H.DELETED_AT,  ");
                //sb.Append("                                H.TRANSACTIONID,  ");
                //sb.Append("                                H.LEVELWEIGHT,  ");
                //sb.Append("                                H.DATE,  ");
                //sb.Append("                                H.LEVELNAME  ");
                //sb.Append("               FROM GDA_HISTORY_HIERARCHY_RELATIONSHIP H (NOLOCK)  ");
                //sb.Append("               INNER JOIN HIERARCHYCTE CTE ON H.PARENTIDENTIFICATION = CTE.IDGDA_COLLABORATORS  ");
                //sb.Append("               WHERE CTE.LEVELNAME <> 'AGENTE'  ");
                //sb.Append("                 AND CTE.[DATE] = @DATEENV )  ");
                //sb.Append(" ");
                //sb.Append(" ");
                //sb.Append("				 SELECT * FROM GDA_SECTOR WHERE IDGDA_SECTOR IN (SELECT DISTINCT IDGDA_SECTOR FROM GDA_HISTORY_COLLABORATOR_SECTOR ");
                //sb.Append("		WHERE IDGDA_COLLABORATORS IN (SELECT DISTINCT(IDGDA_COLLABORATORS)  ");
                //sb.Append("      FROM HIERARCHYCTE  ");
                //sb.Append("      WHERE LEVELNAME = 'AGENTE'  ");
                //sb.Append("        AND DATE = @DATEENV) AND CREATED_AT = @DATEENV) AND SECTOR = 1 AND DELETED_AT IS NULL ");
                //sb.AppendFormat(" {0} ", filter);
            }
            else
            {
                //sb.Append("SELECT * FROM GDA_SECTOR (NOLOCK) ");
                //sb.Append("WHERE SECTOR = 1 AND DELETED_AT IS NULL ");
                //sb.AppendFormat(" {0} ", filter);
                sb.Append("SELECT GS.IDGDA_SECTOR, ");
                sb.Append("MAX(NAME) AS NAME  	    ");
                sb.Append("FROM GDA_SECTOR  GS (NOLOCK) ");
                sb.Append("WHERE DELETED_AT IS NULL ");
                //sb.Append("WHERE SECTOR = 1 ");
                //sb.Append("AND DELETED_AT IS NULL  ");
                sb.AppendFormat(" {0} ", filter);
                sb.Append("GROUP BY GS.IDGDA_SECTOR   ");
            }


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
                                sector sec = new sector();
                                sec.id = int.Parse(reader["IDGDA_SECTOR"].ToString());
                                sec.name = reader["NAME"].ToString();
                                lsec.Add(sec);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return lsec;
        }
        public static bool VariavelContemApenasNumeros(object variavel)
        {
            string texto = variavel.ToString();
            foreach (char c in texto)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }

        public static List<sectorReturn> listSectorsGroupDate(string startDate, string endDate, int collaboratorId)
        {
            string filter = "";
            StringBuilder sb = new StringBuilder();
            List<sectorReturn> lsec = new List<sectorReturn>();

            sb.Append($"SELECT S.NAME AS NAME, MAX(S.LEVEL) AS LEVEL, MAX(S.CREATED_AT) AS CREATED_AT FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CD ");
            sb.Append($"INNER JOIN GDA_SECTOR (NOLOCK) AS S ON S.IDGDA_SECTOR = CD.IDGDA_SECTOR ");
            sb.Append($"INNER JOIN GDA_RESULT (NOLOCK) AS R ON R.IDGDA_COLLABORATORS = CD.IDGDA_COLLABORATORS AND R.CREATED_AT = CD.CREATED_AT ");
            sb.Append($"WHERE CD.CREATED_AT >= '{startDate}' AND CD.CREATED_AT <= '{endDate}' ");
            sb.Append($"AND S.DELETED_AT IS NULL AND ");
            sb.Append($"(CD.IDGDA_COLLABORATORS = {collaboratorId} ");
            sb.Append($"OR CD.MATRICULA_SUPERVISOR = {collaboratorId} ");
            sb.Append($"OR CD.MATRICULA_COORDENADOR = {collaboratorId} ");
            sb.Append($"OR CD.MATRICULA_GERENTE_II = {collaboratorId} ");
            sb.Append($"OR CD.MATRICULA_GERENTE_I = {collaboratorId} ");
            sb.Append($"OR CD.MATRICULA_DIRETOR = {collaboratorId} ");
            sb.Append($"OR CD.MATRICULA_CEO = {collaboratorId}) ");
            sb.Append($"AND S.IDGDA_SECTOR IS NOT NULL ");
            sb.Append($"GROUP BY S.NAME ");

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
                                sectorReturn sec = new sectorReturn();
                                sec.id = 0;
                                sec.name = reader["NAME"].ToString();
                                sec.level = reader["LEVEL"] != DBNull.Value ? Convert.ToInt32(reader["LEVEL"]) : 0;
                                sec.sector = 1;
                                sec.subSector = 0;
                                sec.createdAt = reader["CREATED_AT"] != DBNull.Value ? Convert.ToDateTime(reader["CREATED_AT"]) : (DateTime?)null;
                                sec.deletedAt = (DateTime?)null;
                                lsec.Add(sec);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return lsec;
        }

        public static List<sectorReturn> listSectorsDate(string startDate, string endDate, int collaboratorId)
        {
            string filter = "";
            StringBuilder sb = new StringBuilder();
            List<sectorReturn> lsec = new List<sectorReturn>();

            sb.Append($"SELECT CD.IDGDA_SECTOR, MAX(S.NAME) AS NAME, MAX(S.LEVEL) AS LEVEL, MAX(S.CREATED_AT) AS CREATED_AT FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CD ");
            sb.Append($"INNER JOIN GDA_SECTOR (NOLOCK) AS S ON S.IDGDA_SECTOR = CD.IDGDA_SECTOR ");
            //sb.Append($"INNER JOIN GDA_RESULT (NOLOCK) AS R ON R.IDGDA_COLLABORATORS = CD.IDGDA_COLLABORATORS AND R.CREATED_AT = CD.CREATED_AT ");
            sb.Append($"WHERE CD.CREATED_AT >= '{startDate}' AND CD.CREATED_AT <= '{endDate}' ");
            sb.Append($"AND S.DELETED_AT IS NULL AND ");
            sb.Append($"(CD.IDGDA_COLLABORATORS = {collaboratorId} ");
            sb.Append($"OR CD.MATRICULA_SUPERVISOR = {collaboratorId} ");
            sb.Append($"OR CD.MATRICULA_COORDENADOR = {collaboratorId} ");
            sb.Append($"OR CD.MATRICULA_GERENTE_II = {collaboratorId} ");
            sb.Append($"OR CD.MATRICULA_GERENTE_I = {collaboratorId} ");
            sb.Append($"OR CD.MATRICULA_DIRETOR = {collaboratorId} ");
            sb.Append($"OR CD.MATRICULA_CEO = {collaboratorId}) ");
            sb.Append($"AND S.IDGDA_SECTOR IS NOT NULL ");
            sb.Append($"GROUP BY CD.IDGDA_SECTOR ");

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
                                sectorReturn sec = new sectorReturn();
                                sec.id = int.Parse(reader["IDGDA_SECTOR"].ToString());
                                sec.name = reader["NAME"].ToString();
                                sec.level = reader["LEVEL"] != DBNull.Value ? Convert.ToInt32(reader["LEVEL"]) : 0;
                                sec.sector = 1;
                                sec.subSector = 0;
                                sec.createdAt = reader["CREATED_AT"] != DBNull.Value ? Convert.ToDateTime(reader["CREATED_AT"]) : (DateTime?)null;
                                sec.deletedAt = (DateTime?)null;
                                lsec.Add(sec);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return lsec;
        }
        public static List<sector> listSectors(string sector, int isSubsector)
        {
            string filter = "";
            StringBuilder sb = new StringBuilder();
            List<sector> lsec = new List<sector>();




            if (string.IsNullOrEmpty(sector) == false)
            {

                if (VariavelContemApenasNumeros(sector) == true)
                {

                    if (isSubsector == 0)
                    {
                        filter = $"AND (GS.NAME LIKE '%{sector}%' OR GCD.IDGDA_SECTOR = {sector})";
                    }
                    else
                    {
                        filter = $"AND (GS.NAME LIKE '%{sector}%' OR GCD.IDGDA_SUBSECTOR = {sector})";
                    }

                }
                else
                {
                    filter = $"AND (GS.NAME LIKE '%{sector}%')";
                }

            }

            if (isSubsector == 0)
            {

                sb.Append("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = DATEADD(DAY, -30, GETDATE()); ");
                sb.Append("SELECT DISTINCT GCD.IDGDA_SECTOR,  GS.NAME FROM GDA_COLLABORATORS_DETAILS AS GCD ");
                sb.Append("LEFT JOIN GDA_SECTOR AS GS ON GS.IDGDA_SECTOR = GCD.IDGDA_SECTOR ");
                sb.Append("WHERE  ");
                sb.Append(" GCD.CREATED_AT >= @DATAINICIAL ");
                sb.Append("AND GCD.IDGDA_SECTOR IS NOT NULL  ");
                sb.Append("AND GS.SECTOR = 1  ");
                sb.Append("AND GS.DELETED_AT IS NULL ");
                sb.Append("AND CARGO = 'AGENTE' ");
                sb.Append($" {filter} ");
            }
            else
            {
                sb.Append("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = DATEADD(DAY, -30, GETDATE()); ");
                sb.Append("SELECT DISTINCT GCD.IDGDA_SUBSECTOR AS IDGDA_SECTOR,  GS.NAME FROM GDA_COLLABORATORS_DETAILS AS GCD  ");
                sb.Append("LEFT JOIN GDA_SECTOR AS GS ON GS.IDGDA_SECTOR = GCD.IDGDA_SUBSECTOR  ");
                sb.Append("WHERE   ");
                sb.Append(" GCD.CREATED_AT >= @DATAINICIAL ");
                sb.Append("AND GCD.IDGDA_SUBSECTOR IS NOT NULL   ");
                sb.Append("AND CARGO = 'AGENTE'  ");
                sb.Append($" {filter} ");

            }

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
                                sector sec = new sector();
                                sec.id = int.Parse(reader["IDGDA_SECTOR"].ToString());
                                sec.name = reader["NAME"].ToString();
                                lsec.Add(sec);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return lsec;
        }
        public static List<client> listClient()
        {
            List<client> clients = new List<client>();

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" SELECT * FROM GDA_CLIENT (NOLOCK) ");

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
                                client client = new client();
                                client.id = int.Parse(reader["IDGDA_CLIENT"].ToString());
                                client.name = reader["CLIENT"].ToString();
                                clients.Add(client);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return clients;
        }
        public static List<group> listGroup()
        {
            List<group> groups = new List<group>();

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" SELECT * FROM GDA_GROUPS (NOLOCK) ");

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
                                group group = new group();
                                group.id = int.Parse(reader["ID"].ToString());
                                group.name = reader["ALIAS"].ToString();
                                groups.Add(group);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return groups;
        }
        public static List<homeFloor> listHomeFloor()
        {
            List<homeFloor> homeFloors = new List<homeFloor>();

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" SELECT * FROM GDA_HOMEFLOOR (NOLOCK) ");

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
                                homeFloor homefloor = new homeFloor();
                                homefloor.id = int.Parse(reader["IDGDA_HOMEFLOOR"].ToString());

                                string homeFloorName = "";
                                if (reader["HOMEFLOOR"].ToString() == "HOME")
                                {
                                    homeFloorName = "HOME";
                                }
                                else if (reader["HOMEFLOOR"].ToString() == "FLOOR")
                                {
                                    homeFloorName = "WORK";
                                }
                                else
                                {
                                    homeFloorName = reader["HOMEFLOOR"].ToString();
                                }
                                homefloor.name = homeFloorName;
                                homeFloors.Add(homefloor);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return homeFloors;
        }
        public static List<period> listPeriod()
        {
            List<period> lPeriod = new List<period>();

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" SELECT * FROM GDA_PERIOD (NOLOCK) ");

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
                                period period = new period();
                                period.id = int.Parse(reader["IDGDA_PERIOD"].ToString());
                                period.name = reader["PERIOD"].ToString();
                                lPeriod.Add(period);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return lPeriod;
        }

        public static List<period> listVeteranoNovato()
        {
            List<period> lVeteranoNovato = new List<period>();

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" SELECT * FROM GDA_VETERANO_NOVATO (NOLOCK) ");

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
                                period veteranoNovato = new period();
                                veteranoNovato.id = int.Parse(reader["IDGDA_VETERANO_NOVATO"].ToString());
                                veteranoNovato.name = reader["VETERANO_NOVATO"].ToString();
                                lVeteranoNovato.Add(veteranoNovato);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return lVeteranoNovato;
        }
        public static List<hierarchy> listHierarchy(string filtro)
        {
            List<hierarchy> lHierarchy = new List<hierarchy>();
            string filtroEnv = "";
            if (filtro != "")
            {
                filtroEnv = $" AND IDGDA_HIERARCHY < {filtro} ";
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" SELECT IDGDA_HIERARCHY, LEVELNAME FROM GDA_HIERARCHY (NOLOCK) WHERE 1 = 1 {0} ", filtroEnv);


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
                                hierarchy hierarc = new hierarchy();
                                hierarc.id = int.Parse(reader["IDGDA_HIERARCHY"].ToString());
                                hierarc.name = reader["LEVELNAME"].ToString();
                                lHierarchy.Add(hierarc);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return lHierarchy;
        }
        public static List<basket> listBasketConfiguration()
        {
            List<basket> lBasket = new List<basket>();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT METRIC_MIN, GROUPID FROM GDA_HISTORY_INDICATOR_GROUP (NOLOCK) ");
            sb.Append("WHERE INDICATOR_ID = 10000012 AND DELETED_AT IS NULL ");
            sb.Append("ORDER BY 1 DESC ");


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
                                List<basket> lteste = lBasket.FindAll(l => l.group_id == int.Parse(reader["GROUPID"].ToString()));

                                if (lteste.Count() > 0)
                                {
                                    continue;
                                }
                                if (lBasket.Count() >= 4)
                                {
                                    break;
                                }

                                basket baskets = new basket();
                                baskets.group_id = int.Parse(reader["GROUPID"].ToString());
                                baskets.metric_min = int.Parse(reader["METRIC_MIN"].ToString());
                                lBasket.Add(baskets);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return lBasket;
        }
        public static List<stock> listStock()
        {
            List<stock> lstock = new List<stock>();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT * FROM GDA_STOCK (NOLOCK) WHERE DELETED_AT IS NULL ");


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
                                stock stk = new stock();
                                stk.IDGDA_STOCK = int.Parse(reader["IDGDA_STOCK"].ToString());
                                stk.DESCRIPTION = reader["DESCRIPTION"].ToString();
                                stk.CITY = reader["CITY"].ToString();
                                stk.GDA_ATRIBUTES = reader["GDA_ATRIBUTES"].ToString();
                                stk.CAMPAIGN = reader["CAMPAIGN"].ToString();
                                lstock.Add(stk);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return lstock;

        }

        public static List<product> listProductGeneral(string product)
        {
            string filter = IsInteger(product) == true ? $" AND IDGDA_PRODUCT = '{product}' " : $" AND COMERCIAL_NAME LIKE '%{product}%' ";

            List<product> lproduct = new List<product>();

            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT * FROM GDA_PRODUCT (NOLOCK) WHERE DELETED_AT IS NULL {filter} ");


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
                                product pdt = new product();
                                pdt.IDGDA_PRODUCT = int.Parse(reader["IDGDA_PRODUCT"].ToString());
                                pdt.DESCRIPTION = reader["DESCRIPTION"].ToString();
                                pdt.CODE = reader["CODE"].ToString();
                                pdt.REGISTERED_BY = reader["REGISTERED_BY"].ToString();
                                pdt.PRICE = reader["PRICE"].ToString();
                                pdt.TYPE = reader["TYPE"].ToString();
                                pdt.COMERCIAL_NAME = reader["COMERCIAL_NAME"].ToString();
                                pdt.VALIDITY_DATE = reader["VALIDITY_DATE"].ToString();
                                lproduct.Add(pdt);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return lproduct;

        }

        public static List<product> listProduct()
        {
            List<product> lproduct = new List<product>();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT * FROM GDA_PRODUCT (NOLOCK) WHERE DELETED_AT IS NULL ");


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
                                product pdt = new product();
                                pdt.IDGDA_PRODUCT = int.Parse(reader["IDGDA_PRODUCT"].ToString());
                                pdt.DESCRIPTION = reader["DESCRIPTION"].ToString();
                                pdt.CODE = reader["CODE"].ToString();
                                pdt.REGISTERED_BY = reader["REGISTERED_BY"].ToString();
                                pdt.PRICE = reader["PRICE"].ToString();
                                pdt.TYPE = reader["TYPE"].ToString();
                                pdt.COMERCIAL_NAME = reader["COMERCIAL_NAME"].ToString();
                                pdt.VALIDITY_DATE = reader["VALIDITY_DATE"].ToString();
                                lproduct.Add(pdt);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return lproduct;

        }
        public static List<city> listCity(string state, string city)
        {
            List<city> lCity = new List<city>();
            string filtroCity = "";

            if (state != "")
            {
                filtroCity = $" AND IDGDA_STATE = '{state}' ";
            }
            if (city != "")
            {
                filtroCity = $" {filtroCity} AND CITY LIKE '%{city}%' ";
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" SELECT IDGDA_CITY, CITY FROM GDA_CITY NOLOCK WHERE 1 = 1 {0} ", filtroCity);


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
                                city City = new city();
                                City.id = int.Parse(reader["IDGDA_CITY"].ToString());
                                City.name = reader["CITY"].ToString();
                                lCity.Add(City);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return lCity;
        }
        public static List<state> listState()
        {
            List<state> lState = new List<state>();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" SELECT IDGDA_STATE, STATE FROM GDA_STATE NOLOCK ");
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
                                state State = new state();
                                State.id = int.Parse(reader["IDGDA_STATE"].ToString());
                                State.name = reader["STATE"].ToString();
                                lState.Add(State);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            return lState;
        }
        public static List<site> listSite()
        {
            List<site> lSite = new List<site>();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" SELECT IDGDA_SITE, SITE FROM GDA_SITE NOLOCK ");
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
                                site Site = new site();
                                Site.id = int.Parse(reader["IDGDA_SITE"].ToString());
                                Site.name = reader["SITE"].ToString();
                                lSite.Add(Site);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            return lSite;
        }
        public static List<status> listStatus()
        {
            List<status> lstatus = new List<status>();

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT ID, STATUS FROM GDA_PERSONA_STATUS NOLOCK ");


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
                                status status = new status();
                                status.id = int.Parse(reader["ID"].ToString());
                                status.name = reader["STATUS"].ToString();
                                lstatus.Add(status);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return lstatus;
        }

        public static bool IsInteger(string value)
        {
            // Tenta converter a string para um inteiro
            return int.TryParse(value, out _);
        }

        public static List<personaDetails> PersonaDetails(int filtro)
        {
            List<personaDetails> PersonaDetails = new List<personaDetails>();

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT ");
            sb.AppendFormat("DT.IDGDA_COLLABORATORS, ");
            sb.AppendFormat("COLAB.NAME, ");
            sb.AppendFormat("DT.AGE, ");
            sb.AppendFormat("CT.CITY, ");
            sb.AppendFormat("DT.WHOARE, ");
            sb.AppendFormat("DT.YOURMOTIVATIONS, ");
            sb.AppendFormat("DT.GOALS, ");
            sb.AppendFormat("DT.NEEDS, ");
            sb.AppendFormat("DT.DIFFICULTIES, ");
            sb.AppendFormat("CASE WHEN ISNULL(ST.STATUS,'') = '' THEN 'OFFLINE' ELSE ST.STATUS END AS 'STATUS' ");
            sb.AppendFormat("FROM GDA_PERSONA_DETAILS DT ");
            sb.AppendFormat("INNER JOIN GDA_PERSONA_CITY CT ON CT.ID = DT.IDCITY ");
            sb.AppendFormat("LEFT JOIN GDA_PERSONA_STATUS ST ON ST.ID = DT.IDSTATUS ");
            sb.AppendFormat("LEFT JOIN GDA_COLLABORATORS COLAB ON COLAB.IDGDA_COLLABORATORS = DT.IDGDA_COLLABORATORS ");
            sb.AppendFormat("WHERE DT.IDGDA_COLLABORATORS = {0}", filtro);

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
                                personaDetails Details = new personaDetails();
                                Details.ID = int.Parse(reader["IDGDA_COLLABORATORS"].ToString());
                                Details.NAME = reader["NAME"].ToString();
                                Details.AGE = reader["AGE"].ToString();
                                Details.CITY = reader["CITY"].ToString();
                                Details.WHOARE = reader["WHOARE"].ToString();
                                Details.YOURMOTIVATIONS = reader["YOURMOTIVATIONS"].ToString();
                                Details.GOALS = reader["GOALS"].ToString();
                                Details.NEEDS = reader["NEEDS"].ToString();
                                Details.DIFFICULTIES = reader["DIFFICULTIES"].ToString();
                                Details.STATUS = reader["STATUS"].ToString();
                                PersonaDetails.Add(Details);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            return PersonaDetails;
        }
        public static List<collaborators> listCollaborators(string filtro)
        {
            List<collaborators> collaborators = new List<collaborators>();
            string filtroEnv = "";
            if (filtro != "")
            {
                filtroEnv = $" AND (CONVERT(VARCHAR, CLA.IDGDA_COLLABORATORS) LIKE '%{filtro}%' OR NAME LIKE '%{filtro}%') ";
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT CLA.IDGDA_COLLABORATORS, MAX(NAME) AS NAME   ");
            sb.AppendFormat("FROM GDA_COLLABORATORS CLA (NOLOCK) ");
            sb.AppendFormat("WHERE DELETED_AT IS NULL {0} ", filtroEnv);
            sb.AppendFormat("GROUP BY CLA.IDGDA_COLLABORATORS ");


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
                                collaborators Collaborator = new collaborators();
                                Collaborator.ID = int.Parse(reader["IDGDA_COLLABORATORS"].ToString());
                                Collaborator.NAME = reader["NAME"].ToString();
                                collaborators.Add(Collaborator);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return collaborators;
        }
        public static List<historychat> listHistoryChat(int codgroup)
        {
            List<historychat> HistoryChat = new List<historychat>();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT TOP 10 ");
            sb.AppendFormat("    CASE  ");
            sb.AppendFormat("        WHEN DELETED_AT_ALL IS NOT NULL THEN 'Mensagem apagada'  ");
            sb.AppendFormat("        ELSE MESSAGE  ");
            sb.AppendFormat("    END AS MESSAGE, ");
            sb.AppendFormat("    CREATED_AT, ");
            sb.AppendFormat("    IDGDA_CHAT_COLLABORATORS, ");
            sb.AppendFormat("    DELETED_AT_ME, ");
            sb.AppendFormat("    DELETED_AT_ALL ");
            sb.AppendFormat("FROM GDA_CHAT_GROUP_MESSAGE (NOLOCK) ");
            sb.AppendFormat("WHERE  ");
            sb.AppendFormat("    IDGDA_CHAT_GROUP ={0}", codgroup);
            sb.AppendFormat("    AND (IDGDA_CHAT_COLLABORATORS <> IDGDA_CHAT_COLLABORATORS OR DELETED_AT_ME IS NULL) ");
            sb.AppendFormat("ORDER BY 1 DESC ");

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
                                historychat Chat = new historychat();
                                Chat.MESSAGE = reader["MESSAGE"].ToString();
                                Chat.CREATED_AT = reader["CREATED_AT"].ToString();
                                Chat.IDGDA_CHAT_COLLABORATORS = int.Parse(reader["IDGDA_CHAT_COLLABORATORS"].ToString());
                                HistoryChat.Add(Chat);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            return HistoryChat;
        }
        public static List<metricBySector> listMetricBySector(string dtInicial, string dtFinal)
        {
            List<metricBySector> metrica = new List<metricBySector>();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("DECLARE @DATAINI DATE; SET @DATAINI = '{0}'; ", dtInicial);
            sb.AppendFormat("DECLARE @DATAFIM DATE; SET @DATAFIM = '{0}'; ", dtFinal);
            sb.Append(" ");
            sb.Append("SELECT ISNULL(CASE  ");
            sb.Append("            WHEN SEC.IDGDA_SECTOR IS NULL AND SUB.IDGDA_SECTOR IS NOT NULL THEN SECSUB.IDGDA_SECTOR  ");
            sb.Append("           ELSE SEC.IDGDA_SECTOR  ");
            sb.Append("       END,0) AS 'GIP',  ");
            sb.Append("       CASE  ");
            sb.Append("            WHEN SEC.IDGDA_SECTOR IS NULL AND SUB.IDGDA_SECTOR IS NOT NULL THEN MAX(SECSUB.NAME)  ");
            sb.Append("           ELSE MAX(SEC.NAME)  ");
            sb.Append("       END AS SETOR,  ");
            sb.Append("        ISNULL(CASE  ");
            sb.Append("           WHEN SUB.IDGDA_SECTOR IS NULL THEN '-'  ");
            sb.Append("           ELSE SUB.IDGDA_SECTOR  ");
            sb.Append("       END,0) AS 'GIPSUB',  ");
            sb.Append("       CASE  ");
            sb.Append("           WHEN MAX(SUB.NAME) IS NULL THEN '-'  ");
            sb.Append("           ELSE MAX(SUB.NAME)  ");
            sb.Append("       END AS SUBSETOR,  ");
            sb.Append("       MAX(IND.NAME) AS INDICADOR,  ");
            sb.Append("       IND.IDGDA_INDICATOR AS 'COD IND',  ");
            sb.Append("       MAX(IND.TYPE) AS 'TIPO', ");
            sb.Append("       MAX(S.GOAL) AS GOAL,  ");
            sb.Append("       MAX(S.GOAL_NIGHT) AS GOAL_NIGHT,  ");
            sb.Append("       MAX(S.GOAL_LATENIGHT) AS GOAL_LATENIGHT,  ");
            sb.Append("       MAX(GRO.ALIAS) AS GRUPO,  ");
            sb.Append("       MAX(G.METRIC_MIN) AS METRIC_MIN,  ");
            sb.Append("       MAX(G.METRIC_MIN_NIGHT) AS METRIC_MIN_NIGHT,  ");
            sb.Append("       MAX(G.METRIC_MIN_LATENIGHT) AS METRIC_MIN_LATENIGHT,  ");
            sb.Append("       MAX(G.MONETIZATION) AS MONETIZATION,  ");
            sb.Append("       MAX(G.MONETIZATION_NIGHT) AS MONETIZATION_NIGHT,  ");
            sb.Append("       MAX(G.MONETIZATION_LATENIGHT) AS MONETIZATION_LATENIGHT,  ");
            sb.Append("CONVERT(DATE, G.STARTED_AT) AS 'INICIO', ");
            sb.Append("CONVERT(DATE, G.ENDED_AT) AS 'FIM', ");
            sb.Append("CASE WHEN G.DELETED_AT IS NULL THEN 1 ELSE 0 END AS STATUS ");
            sb.Append("FROM GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS G  ");
            sb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) AS S ON S.INDICATOR_ID = G.INDICATOR_ID  ");
            sb.Append("AND S.SECTOR_ID = G.SECTOR_ID  ");
            sb.Append("AND CONVERT(DATE, S.STARTED_AT) >= @DATAINI  ");
            sb.Append("AND CONVERT(DATE, S.ENDED_AT) <= @DATAFIM  ");
            sb.Append("LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = G.SECTOR_ID  ");
            sb.Append("AND SEC.SECTOR = 1  ");
            sb.Append("LEFT JOIN GDA_SECTOR (NOLOCK) AS SUB ON SUB.IDGDA_SECTOR = G.SECTOR_ID  ");
            sb.Append("AND SUB.SUBSECTOR = 1  ");
            sb.Append("LEFT JOIN GDA_SECTOR (NOLOCK) AS SECSUB ON SECSUB.IDGDA_SECTOR = G.SECTOR_ID_PARENT  ");
            sb.Append("LEFT JOIN GDA_INDICATOR (NOLOCK) AS IND ON IND.IDGDA_INDICATOR = G.INDICATOR_ID  ");
            sb.Append("LEFT JOIN GDA_GROUPS (NOLOCK) AS GRO ON GRO.ID = G.GROUPID  ");
            sb.Append("WHERE CONVERT(DATE, G.STARTED_AT) >= @DATAINI  ");
            sb.Append("  AND CONVERT(DATE,G.ENDED_AT) <= @DATAFIM  ");
            sb.Append("GROUP BY SEC.IDGDA_SECTOR, SECSUB.IDGDA_SECTOR, SUB.IDGDA_SECTOR, IND.IDGDA_INDICATOR,  G.GROUPID, CONVERT(DATE, G.STARTED_AT), CONVERT(DATE,G.ENDED_AT), G.DELETED_AT ");
            sb.Append("ORDER BY CONVERT(DATE, G.STARTED_AT),  ");
            sb.Append("         SEC.IDGDA_SECTOR,  ");
            sb.Append("         IND.IDGDA_INDICATOR,  ");
            sb.Append("         G.GROUPID  ");
            #region QueryAntiga
            //sb.Append("SEC.IDGDA_SECTOR AS 'GIP', ");
            //sb.Append("SEC.NAME AS SETOR, ");
            //sb.Append("SUB.IDGDA_SECTOR AS 'GIPSUB', ");
            //sb.Append("SUB.NAME AS SUBSETOR, ");
            //sb.Append("	   IND.NAME AS INDICADOR, ");
            //sb.Append("       IND.IDGDA_INDICATOR AS 'COD IND', ");
            ////sb.Append("	   CASE WHEN GOAL <>  0 THEN 'DIURNO' ");
            ////sb.Append("			WHEN GOAL_NIGHT <> 0 THEN 'NOTURNO' ");
            ////sb.Append("			WHEN GOAL_LATENIGHT <> 0 THEN 'MADRUGADA' ");
            ////sb.Append("		END AS 'TURNO', ");
            //sb.Append("GOAL, ");
            //sb.Append("GOAL_NIGHT, ");
            //sb.Append("GOAL_LATENIGHT, ");
            //sb.Append("	   GRO.ALIAS AS GRUPO, ");
            //sb.Append("G.METRIC_MIN, ");
            //sb.Append("G.METRIC_MIN_NIGHT, ");
            //sb.Append("G.METRIC_MIN_LATENIGHT, ");
            //sb.Append("G.MONETIZATION, ");
            //sb.Append("G.MONETIZATION_NIGHT, ");
            //sb.Append("G.MONETIZATION_LATENIGHT, ");
            ////sb.Append("	   ISNULL(CASE WHEN G.METRIC_MIN <>  0 THEN G.METRIC_MIN ");
            ////sb.Append("			WHEN G.METRIC_MIN_NIGHT <> 0 THEN G.METRIC_MIN_NIGHT ");
            ////sb.Append("			WHEN G.METRIC_MIN_LATENIGHT <> 0 THEN G.METRIC_MIN_LATENIGHT ");
            ////sb.Append("		END,0) AS 'META MINIMA', ");
            ////sb.Append("		ISNULL(CASE WHEN G.MONETIZATION <>  0 THEN G.MONETIZATION ");
            ////sb.Append("			WHEN G.MONETIZATION_NIGHT <> 0 THEN G.MONETIZATION_NIGHT ");
            ////sb.Append("			WHEN G.MONETIZATION_LATENIGHT <> 0 THEN G.MONETIZATION_LATENIGHT ");
            ////sb.Append("		END,0) AS 'MOEDAS', ");
            //sb.Append("	   CONVERT(DATE, G.STARTED_AT) AS 'INICIO', ");
            //sb.Append("       CONVERT(DATE,G.ENDED_AT) AS 'FIM' ");
            //sb.Append("FROM GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS G ");
            //sb.Append("LEFT JOIN GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) AS S ON S.INDICATOR_ID = G.INDICATOR_ID ");
            //sb.Append("AND S.SECTOR_ID = G.SECTOR_ID ");
            //sb.Append("AND S.DELETED_AT IS NULL ");
            //sb.Append("AND CONVERT(DATE, G.STARTED_AT) = CONVERT(DATE, S.STARTED_AT) ");
            //sb.Append("AND CONVERT(DATE, G.ENDED_AT) = CONVERT(DATE, S.ENDED_AT) ");
            //sb.Append("LEFT JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = G.SECTOR_ID AND SEC.SECTOR = 1 ");
            //sb.Append("LEFT JOIN GDA_SECTOR (NOLOCK) AS SUB ON SUB.IDGDA_SECTOR = G.SECTOR_ID AND SUB.SUBSECTOR = 1 ");
            //sb.Append("LEFT JOIN GDA_INDICATOR (NOLOCK) AS IND ON IND.IDGDA_INDICATOR = G.INDICATOR_ID ");
            //sb.Append("LEFT JOIN GDA_GROUPS (NOLOCK) AS GRO ON GRO.ID = G.GROUPID ");
            //sb.Append("WHERE  ");
            //sb.Append("G.DELETED_AT IS NULL ");
            //sb.Append("AND CONVERT(DATE, G.STARTED_AT) <= @DATAINI ");
            //sb.Append("AND CONVERT(DATE,G.ENDED_AT) >= @DATAFIM ");
            //sb.Append("ORDER BY CONVERT(DATE, G.STARTED_AT), ");
            //sb.Append("         SEC.IDGDA_SECTOR, ");
            //sb.Append("         IND.IDGDA_INDICATOR, ");
            //sb.Append("         G.GROUPID ");
            //sb.Append("SELECT SEC.NAME AS SETOR, ");
            //sb.Append("       SEC.IDGDA_SECTOR AS 'GIP', ");
            //sb.Append("       IND.NAME AS INDICADOR, ");
            //sb.Append("       IND.IDGDA_INDICATOR AS 'COD IND', ");
            //sb.Append("       ISNULL(S.GOAL,0) AS 'META', ");
            //sb.Append("       GRO.ALIAS AS GRUPO, ");
            //sb.Append("ISNULL(CASE WHEN G.METRIC_MIN_NIGHT IS NULL AND G.METRIC_MIN_LATENIGHT IS NULL THEN G.METRIC_MIN ");
            //sb.Append("WHEN G.METRIC_MIN IS NULL AND G.METRIC_MIN_LATENIGHT IS NULL THEN G.METRIC_MIN_NIGHT ");
            //sb.Append("WHEN G.METRIC_MIN IS NULL AND G.METRIC_MIN_NIGHT IS NULL THEN G.METRIC_MIN_LATENIGHT ");
            //sb.Append("END,0) AS 'META MINIMA', ");
            //sb.Append("ISNULL(CASE WHEN G.MONETIZATION_NIGHT IS NULL AND G.METRIC_MIN_LATENIGHT IS NULL THEN G.MONETIZATION ");
            //sb.Append("WHEN G.MONETIZATION IS NULL AND G.MONETIZATION_LATENIGHT IS NULL THEN G.MONETIZATION_NIGHT ");
            //sb.Append("WHEN G.MONETIZATION IS NULL AND G.MONETIZATION_NIGHT IS NULL THEN G.MONETIZATION_LATENIGHT ");
            //sb.Append("END,0) AS 'MOEDAS', ");
            //sb.Append("       CONVERT(DATE, G.STARTED_AT) AS 'INICIO', ");
            //sb.Append("       CONVERT(DATE,G.ENDED_AT) AS 'FIM' ");
            //sb.Append("FROM GDA_HISTORY_INDICATOR_GROUP (NOLOCK) AS G ");
            //sb.Append("INNER JOIN GDA_HISTORY_INDICATOR_SECTORS (NOLOCK) AS S ON S.INDICATOR_ID = G.INDICATOR_ID ");
            //sb.Append("AND S.SECTOR_ID = G.SECTOR_ID ");
            //sb.Append("AND CONVERT(DATE, G.STARTED_AT) = CONVERT(DATE, S.STARTED_AT) ");
            //sb.Append("AND CONVERT(DATE, G.ENDED_AT) = CONVERT(DATE, S.ENDED_AT) ");
            //sb.Append("INNER JOIN GDA_GROUPS (NOLOCK) AS GRO ON GRO.ID = G.GROUPID ");
            //sb.Append("INNER JOIN GDA_SECTOR (NOLOCK) AS SEC ON SEC.IDGDA_SECTOR = G.SECTOR_ID ");
            //sb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) AS IND ON IND.IDGDA_INDICATOR = G.INDICATOR_ID ");
            //sb.Append("WHERE G.DELETED_AT IS NULL ");
            //sb.Append("  AND S.DELETED_AT IS NULL ");
            //sb.Append("  AND CONVERT(DATE, G.STARTED_AT) >= @DATAINI ");
            //sb.Append("  AND CONVERT(DATE,G.ENDED_AT) <= @DATAFIM ");
            //sb.Append("ORDER BY CONVERT(DATE, G.STARTED_AT), ");
            //sb.Append("         SEC.IDGDA_SECTOR, ");
            //sb.Append("         IND.IDGDA_INDICATOR, ");
            //sb.Append("         G.groupId ");
            #endregion
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
                                if (reader["GIPSUB"].ToString() == "0" && reader["GIP"].ToString() == "0")
                                {
                                    continue;
                                }


                                string tipo = "";
                                if (reader["TIPO"].ToString() == "INTEGER")
                                {
                                    tipo = "INTEIRO";
                                }
                                else if (reader["TIPO"].ToString() == "PERCENT")
                                {
                                    tipo = "PORCENTAGEM";
                                }
                                else if (reader["TIPO"].ToString() == "HOUR")
                                {
                                    tipo = "HORA";
                                }
                                else
                                {
                                    tipo = reader["TIPO"].ToString();
                                }

                                if (reader["GOAL"].ToString() != "" && reader["METRIC_MIN"].ToString() != "" && reader["MONETIZATION"].ToString() != "")
                                {
                                    metricBySector metricManha = new metricBySector();
                                    metricManha.SETOR = reader["SETOR"].ToString() == null ? "-" : reader["SETOR"].ToString();
                                    metricManha.CODGIP = int.Parse(reader["GIP"].ToString());
                                    metricManha.SUBSETOR = reader["SUBSETOR"].ToString();
                                    metricManha.CODGIPSUB = int.Parse(reader["GIPSUB"].ToString());
                                    metricManha.INDICADOR = reader["INDICADOR"].ToString();
                                    metricManha.CODINDICADOR = int.Parse(reader["COD IND"].ToString());
                                    metricManha.TIPO = tipo;
                                    metricManha.TURNO = "DIURNO";
                                    metricManha.META = reader["GOAL"].ToString() == "0" ? "0" : metricManha.TIPO == "HOUR" ? ResultConsolidatedController.FormatTime(reader["GOAL"].ToString()) : reader["GOAL"].ToString();
                                    metricManha.GRUPO = reader["GRUPO"].ToString();
                                    metricManha.META_MINIMA = reader["METRIC_MIN"].ToString() == "0" ? "0" : reader["METRIC_MIN"].ToString();
                                    metricManha.MOEDAS = reader["MONETIZATION"].ToString() == "0" ? "0" : reader["MONETIZATION"].ToString();
                                    metricManha.DATAINICIAL = reader["INICIO"].ToString();
                                    metricManha.DATAFINAL = reader["FIM"].ToString();
                                    metricManha.STATUS = Convert.ToInt32(reader["STATUS"].ToString());
                                    metrica.Add(metricManha);
                                }


                                if (reader["GOAL_NIGHT"].ToString() != "" && reader["METRIC_MIN_NIGHT"].ToString() != "" && reader["MONETIZATION_NIGHT"].ToString() != "")
                                {
                                    metricBySector metricNoite = new metricBySector();
                                    metricNoite.SETOR = reader["SETOR"].ToString() == null ? "-" : reader["SETOR"].ToString();
                                    metricNoite.CODGIP = int.Parse(reader["GIP"].ToString());
                                    metricNoite.SUBSETOR = reader["SUBSETOR"].ToString();
                                    metricNoite.CODGIPSUB = int.Parse(reader["GIPSUB"].ToString());
                                    metricNoite.INDICADOR = reader["INDICADOR"].ToString();
                                    metricNoite.CODINDICADOR = int.Parse(reader["COD IND"].ToString());
                                    metricNoite.TIPO = tipo;
                                    metricNoite.TURNO = "NOTURNO";
                                    metricNoite.META = reader["GOAL_NIGHT"].ToString() == "0" ? "0" : metricNoite.TIPO == "HOUR" ? ResultConsolidatedController.FormatTime(reader["GOAL_NIGHT"].ToString()) : reader["GOAL_NIGHT"].ToString();
                                    metricNoite.GRUPO = reader["GRUPO"].ToString();
                                    metricNoite.META_MINIMA = reader["METRIC_MIN_NIGHT"].ToString() == "0" ? "0" : reader["METRIC_MIN_NIGHT"].ToString();
                                    metricNoite.MOEDAS = reader["MONETIZATION_NIGHT"].ToString() == "0" ? "0" : reader["MONETIZATION_NIGHT"].ToString();
                                    metricNoite.DATAINICIAL = reader["INICIO"].ToString();
                                    metricNoite.DATAFINAL = reader["FIM"].ToString();
                                    metricNoite.STATUS = Convert.ToInt32(reader["STATUS"].ToString());
                                    metrica.Add(metricNoite);
                                }
                                if (reader["GOAL_LATENIGHT"].ToString() != "" && reader["METRIC_MIN_LATENIGHT"].ToString() != "" && reader["MONETIZATION_LATENIGHT"].ToString() != "")
                                {
                                    if (int.Parse(reader["GIP"].ToString()) == 136 && int.Parse(reader["COD IND"].ToString()) == 2)
                                    {
                                        var parou = "";
                                    }

                                    metricBySector metricMadrugada = new metricBySector();
                                    metricMadrugada.SETOR = reader["SETOR"].ToString() == null ? "-" : reader["SETOR"].ToString();
                                    metricMadrugada.CODGIP = int.Parse(reader["GIP"].ToString());
                                    metricMadrugada.SUBSETOR = reader["SUBSETOR"].ToString();
                                    metricMadrugada.CODGIPSUB = int.Parse(reader["GIPSUB"].ToString());
                                    metricMadrugada.INDICADOR = reader["INDICADOR"].ToString();
                                    metricMadrugada.CODINDICADOR = int.Parse(reader["COD IND"].ToString());
                                    metricMadrugada.TIPO = tipo;
                                    metricMadrugada.TURNO = "MADRUGADA";
                                    metricMadrugada.META = reader["GOAL_LATENIGHT"].ToString() == "0" ? "0" : metricMadrugada.TIPO == "HOUR" ? ResultConsolidatedController.FormatTime(reader["GOAL_LATENIGHT"].ToString()) : reader["GOAL_LATENIGHT"].ToString();
                                    metricMadrugada.GRUPO = reader["GRUPO"].ToString();
                                    metricMadrugada.META_MINIMA = reader["METRIC_MIN_LATENIGHT"].ToString() == "0" ? "0" : reader["METRIC_MIN_LATENIGHT"].ToString();
                                    metricMadrugada.MOEDAS = reader["MONETIZATION_LATENIGHT"].ToString() == "0" ? "0" : reader["MONETIZATION_LATENIGHT"].ToString();
                                    metricMadrugada.DATAINICIAL = reader["INICIO"].ToString();
                                    metricMadrugada.DATAFINAL = reader["FIM"].ToString();
                                    metricMadrugada.STATUS = Convert.ToInt32(reader["STATUS"].ToString());
                                    metrica.Add(metricMadrugada);
                                }



                                //metricBySector metric = new metricBySector();
                                //metric.SETOR = reader["SETOR"].ToString();
                                //metric.CODGIP = int.Parse(reader["GIP"].ToString());                          
                                //metric.INDICADOR = reader["INDICADOR"].ToString();
                                //metric.CODINDICADOR = int.Parse(reader["COD IND"].ToString());
                                //if (metric.CODGIP == 136 && metric.CODINDICADOR == 729)
                                //{
                                //    var parou = "";
                                //}
                                //string GOAL = reader["GOAL"].ToString();
                                //string GOAL_NIGHT = reader["GOAL_NIGHT"].ToString();
                                //string GOAL_LATENIGHT = reader["GOAL_LATENIGHT"].ToString();
                                //if (GOAL == "0" && GOAL_NIGHT == "" && GOAL_LATENIGHT == "" || GOAL != "" && GOAL != "0")
                                //{
                                //    metric.TURNO = "DIURNO";
                                //}
                                //else if(GOAL_NIGHT == "0" && GOAL == "" && GOAL_LATENIGHT == "" || GOAL_NIGHT != "" && GOAL_NIGHT !="0")
                                //{
                                //    metric.TURNO = "NOTURNO";
                                //}
                                //else if (GOAL_LATENIGHT =="0" && GOAL == "" && GOAL_NIGHT == "" || GOAL_LATENIGHT != "" && GOAL_LATENIGHT != "0")
                                //{
                                //    metric.TURNO = "MADRUGADA";
                                //}
                                ////metric.TURNO = reader["TURNO"].ToString();
                                //metric.META = reader["META"].ToString() == "0" ? "0" : reader["META"].ToString();
                                //metric.GRUPO = reader["GRUPO"].ToString();
                                //metric.META_MINIMA = reader["META MINIMA"].ToString() == "0" ? "0" : reader["META MINIMA"].ToString();
                                //metric.MOEDAS = reader["MOEDAS"].ToString() == "0" ? "0" : reader["MOEDAS"].ToString();
                                //metric.DATAINICIAL = reader["INICIO"].ToString();
                                //metric.DATAFINAL = reader["FIM"].ToString();
                                //metrica.Add(metric);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            return metrica;
        }
        public static List<resultCollaborators> listResultCollaborators(string dtInicial, string dtFinal)
        {
            List<resultCollaborators> result = new List<resultCollaborators>();
            //string filtroEnv = "";
            //if (filtro != "")
            //{
            //    filtroEnv = $" AND (CONVERT(VARCHAR, CT.IDGDA_COLLABORATORS) LIKE '%{filtro}%' OR CT.NAME LIKE '%{filtro}%') ";
            //}
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("DECLARE @DATAINICIAL DATE; SET @DATAINICIAL = '{0}';", dtInicial);
            sb.AppendFormat("DECLARE @DATAFINAL DATE; SET @DATAFINAL = '{0}';", dtFinal);
            sb.Append("SELECT  ");
            sb.Append("R.IDGDA_COLLABORATORS AS MATRICULA, ");
            sb.Append("R.INDICADORID AS 'COD INDICADOR', ");
            sb.Append("IND.NAME AS INDICADOR,");
            sb.Append("R.RESULT AS RESULTADO, ");
            sb.Append("R.FACTORS AS FATOR, ");
            sb.Append("R.CREATED_AT AS DATA  ");
            sb.Append("FROM GDA_RESULT R (NOLOCK) ");
            sb.Append("INNER JOIN GDA_COLLABORATORS CT (NOLOCK) ON CT.IDGDA_COLLABORATORS = R.IDGDA_COLLABORATORS ");
            sb.Append("INNER JOIN GDA_INDICATOR IND (NOLOCK) ON IND.IDGDA_INDICATOR = R.INDICADORID ");
            sb.Append("WHERE 1=1 ");
            sb.Append("AND CONVERT(DATE,R.CREATED_AT) <= @DATAINICIAL ");
            sb.Append("AND CONVERT(DATE,R.CREATED_AT) >= @DATAINICIAL ");

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
                                resultCollaborators resultado = new resultCollaborators();
                                resultado.MATRICULA = int.Parse(reader["MATRICULA"].ToString());
                                resultado.INDICADORID = int.Parse(reader["COD INDICADOR"].ToString());
                                resultado.INDICADOR = reader["INDICADOR"].ToString();
                                resultado.RESULTADO = reader["RESULTADO"].ToString();
                                resultado.FATOR = reader["FATOR"].ToString();
                                resultado.DATA = reader["DATA"].ToString();
                                result.Add(resultado);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            return result;
        }
        public static List<hierarchy> listHierarchyProfile(int IDGDA_HIERARCHY)
        {
            List<hierarchy> retorno = new List<hierarchy>();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT * FROM GDA_HIERARCHY NOLOCK WHERE DELETED_AT IS NULL");
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
                                hierarchy hierarchyProfile = new hierarchy();

                                hierarchyProfile.id = int.Parse(reader["IDGDA_HIERARCHY"].ToString());
                                hierarchyProfile.name = reader["LEVELNAME"].ToString();
                                retorno.Add(hierarchyProfile);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            if (IDGDA_HIERARCHY != 0)
            {
                // Encontrar o índice do objeto com o ID_PROFILE na lista
                int index = retorno.FindIndex(item => item.id == IDGDA_HIERARCHY);

                if (index != -1)
                {
                    // Mover o objeto com ID_PROFILE para o início da lista
                    hierarchy hierarchyProfile = retorno[index];
                    retorno.RemoveAt(index);
                    retorno.Insert(0, hierarchyProfile);
                }

            }
            return retorno;
        }
        public static List<score> ListIndicatorByScore(int SECTOR_ID, string DTATUAL)
        {
            List<score> retorno = new List<score>();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT INDICATOR_ID, IT.NAME, WEIGHT_SCORE, STARTED_AT, ENDED_AT FROM GDA_HISTORY_SCORE_INDICATOR_SECTOR SC (NOLOCK) ");
            sb.AppendLine("INNER JOIN GDA_INDICATOR IT (NOLOCK) ON IT.IDGDA_INDICATOR = SC.INDICATOR_ID ");
            sb.AppendFormat($"WHERE SECTOR_ID = {SECTOR_ID}  AND CONVERT(DATE, STARTED_AT) <= '{DTATUAL}' AND CONVERT(DATE, ENDED_AT) >= '{DTATUAL}' ");
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
                                score Score = new score();

                                Score.indicatorId = int.Parse(reader["INDICATOR_ID"].ToString());
                                Score.name = reader["NAME"].ToString();
                                Score.Score = reader["WEIGHT_SCORE"].ToString();
                                retorno.Add(Score);
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
        public static List<ScoreBySector> ListScoreBySector(string DTATUAL)
        {
            List<ScoreBySector> retorno = new List<ScoreBySector>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  ");
            sb.Append("		HSC.SECTOR_ID AS CODGIP, ");
            sb.Append("		SC.NAME AS SETOR, ");
            sb.Append("		HSC.INDICATOR_ID , ");
            sb.Append("		IT.NAME AS INDICADOR, ");
            sb.Append("		CAST(HSC.WEIGHT_SCORE * 100 AS VARCHAR) + '%'AS SCORE, ");
            sb.Append("		HSC.STARTED_AT, ");
            sb.Append("		HSC.ENDED_AT ");
            sb.Append("		FROM GDA_HISTORY_SCORE_INDICATOR_SECTOR (NOLOCK) HSC ");
            sb.Append("INNER JOIN GDA_SECTOR (NOLOCK) SC ON  SC.IDGDA_SECTOR = HSC.SECTOR_ID ");
            sb.Append("INNER JOIN GDA_INDICATOR (NOLOCK) IT ON  IT.IDGDA_INDICATOR = HSC.INDICATOR_ID ");
            sb.Append("WHERE 1=1 ");
            sb.Append($"AND CONVERT(DATE,HSC.STARTED_AT) <= '{DTATUAL}' ");
            sb.Append($"AND CONVERT(DATE,HSC.ENDED_AT) >= '{DTATUAL}' ");

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
                                ScoreBySector scoreBySector = new ScoreBySector();

                                scoreBySector.CODGIP = int.Parse(reader["CODGIP"].ToString());
                                scoreBySector.SETOR = reader["SETOR"].ToString();
                                scoreBySector.CODINDICADOR = int.Parse(reader["INDICATOR_ID"].ToString());
                                scoreBySector.INDICADOR = reader["INDICADOR"].ToString();
                                scoreBySector.SCORE = reader["SCORE"].ToString();
                                retorno.Add(scoreBySector);
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
        public static CheckingAccountModel LastLineCheckingAccount(string collaboratorId)
        {
            CheckingAccountModel checkingAccount = new CheckingAccountModel();

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT TOP(1) input, output, balance, created_at, observation, reason   ");
            sb.AppendFormat("FROM GDA_CHECKING_ACCOUNT (NOLOCK) ");
            sb.AppendFormat("WHERE collaborator_id = {0} ", collaboratorId);
            sb.AppendFormat("ORDER BY created_at DESC, Id DESC ");

            //string connectionString = ConfigurationManager.ConnectionStrings["RepositorioDBContext"].ConnectionString;

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
                                CheckingAccountModel checkingAccount1 = new CheckingAccountModel();
                                checkingAccount1.Input = reader["input"] != DBNull.Value ? int.Parse(reader["input"].ToString()) : 0;
                                checkingAccount1.Output = reader["output"] != DBNull.Value ? int.Parse(reader["output"].ToString()) : 0;
                                checkingAccount1.Balance = reader["balance"] != null ? int.Parse(reader["balance"].ToString()) : 0;
                                checkingAccount1.Observation = reader["observation"] != DBNull.Value ? reader["observation"].ToString() : "";
                                checkingAccount1.Reason = reader["reason"] != DBNull.Value ? reader["reason"].ToString() : "";
                                checkingAccount1.Created_At = reader["created_at"].ToString();
                                checkingAccount = checkingAccount1;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return checkingAccount;
        }
        public static List<product> ListProductVisibility(string group, string hierarchy, string stock, string ProductName)
        {

            string filterName = "";
            string FilterGroup = "";
            string FilterHierarchy = "";

            //if (!string.IsNullOrEmpty(group) && !string.IsNullOrEmpty(hierarchy))
            //{
            //    filter += $" AND VI.VALUE IN ('{group.Replace(",", "','")}','{hierarchy.Replace(",", "','")}') AND ST.GDA_STOCK_IDGDA_STOCK IN  ('{stock.Replace(",", "','")}') ";
            //}
            //else
            //{
            if (!string.IsNullOrEmpty(group))
            {
                //filter += $" AND VI.VALUE IN ('{group.Replace(",", "','")}') ";
                FilterGroup = $"AND G.ID IN ('{group.Replace(",", "','")}')";
            }

            if (!string.IsNullOrEmpty(hierarchy))
            {
                //filter += $" AND VI.VALUE IN ('{hierarchy.Replace(",", "','")}') ";
                FilterHierarchy = $" AND H.IDGDA_HIERARCHY IN ('{hierarchy.Replace(",", "','")}') ";
            }
            //}
            if (ProductName != "")
            {
                filterName = $"AND P.COMERCIAL_NAME LIKE '%{ProductName}%'";
            }
            List<product> retorno = new List<product>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT MAX(P.IDGDA_PRODUCT) AS IDGDA_PRODUCT, MAX(P.COMERCIAL_NAME) AS COMERCIAL_NAME, MAX(P.TYPE) AS TYPE, MIN(P.TESTE) AS TESTE FROM  ");
            sb.Append("                             (  ");
            sb.Append("                             SELECT P.IDGDA_PRODUCT, P.COMERCIAL_NAME, P.DESCRIPTION, V.TYPE,  ");
            sb.Append("                             CASE WHEN (CASE WHEN V.TYPE = 'GROUP' THEN MAX(G.ID)   ");
            sb.Append("                             WHEN V.TYPE = 'HIERARCHY' THEN MAX(H.IDGDA_HIERARCHY)   ");
            sb.Append("                             WHEN V.TYPE = 'SECTOR' THEN MAX(S.IDGDA_SECTOR)   ");
            sb.Append("                             WHEN V.TYPE IS NULL THEN 1 ");
            sb.Append("							 ELSE -1  ");
            sb.Append("                             END) IS NULL THEN -1   ");
            sb.Append("                             ELSE  ");
            sb.Append("                             (CASE WHEN V.TYPE = 'GROUP' THEN MAX(G.ID)   ");
            sb.Append("                             WHEN V.TYPE = 'HIERARCHY' THEN MAX(H.IDGDA_HIERARCHY)   ");
            sb.Append("                             WHEN V.TYPE = 'SECTOR' THEN MAX(S.IDGDA_SECTOR)   ");
            sb.Append("                             WHEN V.TYPE IS NULL THEN 1 ");
            sb.Append("							 ELSE -1  ");
            sb.Append("                             END)  ");
            sb.Append("                             END AS TESTE  ");
            sb.Append("                             FROM GDA_PRODUCT (NOLOCK) P  ");
            sb.Append("                                               										LEFT JOIN GDA_VISIBILITY (NOLOCK) V ON V.GDA_PRODUCT_IDGDA_PRODUCT_ID = P.IDGDA_PRODUCT  ");
            sb.Append($"                                               										LEFT JOIN GDA_GROUPS (NOLOCK) G ON V.TYPE = 'GROUP' AND V.VALUE = G.NAME  {FilterGroup}  ");
            sb.Append($"                                               										LEFT JOIN GDA_HIERARCHY (NOLOCK) H ON V.TYPE = 'HIERARCHY' AND V.VALUE = H.LEVELNAME  {FilterHierarchy}  ");
            sb.Append("                                               										LEFT JOIN GDA_SECTOR (NOLOCK) S ON V.TYPE = 'SECTOR' AND V.VALUE = S.NAME  ");
            sb.Append("																					LEFT JOIN GDA_STOCK_PRODUCT  ST (NOLOCK) ON  ST.GDA_PRODUCT_IDGDA_PRODUCT = P.IDGDA_PRODUCT  AND AMOUNT <> 0  ");
            sb.Append($"                                                                     WHERE ST.GDA_STOCK_IDGDA_STOCK = {stock} AND P.DELETED_AT IS NULL  ");
            sb.Append("                                               										GROUP BY  P.IDGDA_PRODUCT, P.COMERCIAL_NAME, P.DESCRIPTION,  V.TYPE  ");
            sb.Append("                             ) AS P  ");
            sb.Append("  ");
            sb.Append("							 WHERE 1=1  ");
            sb.Append($"                         {filterName}");
            sb.Append("                             GROUP BY  p.COMERCIAL_NAME  ");
            sb.Append("                             HAVING  MIN(P.TESTE) > 0  ");


            //sb.Append("  ");
            //sb.Append("SELECT DISTINCT  GP.IDGDA_PRODUCT, GP.COMERCIAL_NAME FROM GDA_PRODUCT  GP (NOLOCK)   ");
            //sb.Append("INNER JOIN GDA_VISIBILITY VI (NOLOCK) ON VI.GDA_PRODUCT_IDGDA_PRODUCT_ID = GP.IDGDA_PRODUCT  ");
            //sb.Append("INNER JOIN GDA_STOCK_PRODUCT  ST (NOLOCK) ON ST.GDA_PRODUCT_IDGDA_PRODUCT = GP.IDGDA_PRODUCT AND AMOUNT <> 0  ");
            //sb.Append("WHERE GP.DELETED_AT IS NULL  ");
            //sb.AppendFormat(" {0} ", filter);
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
                                product ProductVisibility = new product();

                                ProductVisibility.IDGDA_PRODUCT = int.Parse(reader["IDGDA_PRODUCT"].ToString());
                                ProductVisibility.COMERCIAL_NAME = reader["COMERCIAL_NAME"].ToString();
                                retorno.Add(ProductVisibility);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            var jsonData = retorno.Select(item => new product
            {
                COMERCIAL_NAME = item.COMERCIAL_NAME,
                IDGDA_PRODUCT = item.IDGDA_PRODUCT,
            }).ToList();
            return jsonData;
        }
        public static List<BlackList> listBlackList(string word)
        {
            string filterName = "";
            if (word != null)
            {
                filterName = $"AND WORD LIKE '%{word}%' ";
            }
            List<BlackList> retorno = new List<BlackList>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT IDGDA_PERSONA_BLACKLIST, WORD FROM GDA_PERSONA_BLACKLIST (NOLOCK) WHERE DELETED_AT IS NULL ");
            sb.Append($"{filterName} ");

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
                                BlackList blacklist = new BlackList();
                                blacklist.IDGDA_BLACKLIST = int.Parse(reader["IDGDA_PERSONA_BLACKLIST"].ToString());
                                blacklist.WORD = reader["WORD"].ToString();
                                retorno.Add(blacklist);
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
        public static List<Hobby> listHobby(string hobby)
        {
            string filterName = "";
            if (hobby != "")
            {
                filterName = $"AND HOBBY LIKE '%{hobby}%' ";
            }
            List<Hobby> retorno = new List<Hobby>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT IDGDA_PERSONA_HOBBY, HOBBY FROM GDA_PERSONA_HOBBY WHERE DELETED_AT IS NULL  ");
            sb.Append($"{filterName} ");

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
                                Hobby Hobby = new Hobby();
                                Hobby.IDGDA_PERSONA_USER_HOBBY = int.Parse(reader["IDGDA_PERSONA_HOBBY"].ToString());
                                Hobby.HOBBY = reader["HOBBY"].ToString();
                                retorno.Add(Hobby);
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
        public static List<PersonaUser> PersonaUser(int idcollaborator, string personaUser)
        {
            List<PersonaUser> retorno = new List<PersonaUser>();
            StringBuilder sb = new StringBuilder();
            string dtAg = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

            sb.Append("SELECT   ");
            sb.Append("	   MAX(PU.IDGDA_PERSONA_USER) AS ID_PERSONA_USER,  ");
            sb.Append("	   MAX(PU.NAME) AS NOME ,   ");
            sb.Append("    MAX(PU.TAHTO) AS FLAGTAHTO, ");
            sb.Append("	   MAX(PU.BC) AS BC,   ");
            sb.Append("	   MAX(SHOW_AGE) AS IDADE,   ");
            sb.Append("	   MAX(SOCIAL_NAME) AS NOME_SOCIAL,   ");
            sb.Append("	   MAX(PICTURE) AS FOTO,   ");
            sb.Append("    MAX(PUD.WHO_IS)AS QUEM_E, ");
            sb.Append("	   MAX(PUD.YOUR_MOTIVATIONS) AS MOTIVACOES,   ");
            sb.Append("	   MAX(PUD.GOALS) AS OBJETIVO,  ");
            sb.Append("    PH.IDGDA_PERSONA_HOBBY,  ");
            sb.Append("	   PH.HOBBY,  ");
            sb.Append("	   MAX(PUD.EMAIL) AS EMAIL ,   ");
            sb.Append("	   MAX(PUD.PHONE_NUMBER) AS TELEFONE,   ");
            sb.Append("	   MAX(PUD.BIRTH_DATE) AS DATA_NASCIMENTO,   ");
            sb.Append("	   MAX(ST.IDGDA_STATE) AS ID_UF,   ");
            sb.Append("	   MAX(ST.STATE) AS UF,   ");
            sb.Append("	   MAX(CT.IDGDA_CITY) AS ID_CIDADE,   ");
            sb.Append("	   MAX(CT.CITY) AS CIDADE,   ");
            sb.Append("	   MAX(C.IDGDA_SITE) AS ID_SITE,   ");
            sb.Append("	   MAX(C.SITE) AS SITE,   ");
            sb.Append($"   MAX(CD.CARGO) AS CARGO, ");
            sb.Append($"   (SELECT TOP 1 BALANCE FROM GDA_CHECKING_ACCOUNT (NOLOCK) ");
            sb.Append($"        WHERE COLLABORATOR_ID = {idcollaborator} ");
            sb.Append($"        ORDER BY CREATED_AT DESC) AS SALDO, ");
            sb.Append($"   MAX(GR.NAME) AS GRUPO, ");
            sb.Append($"   MAX(SEC.NAME) AS SECTOR ");
            sb.Append("FROM GDA_PERSONA_USER  PU (NOLOCK)   ");
            sb.Append("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER CU (NOLOCK) ON CU.IDGDA_PERSONA_USER  = PU.IDGDA_PERSONA_USER   ");
            sb.Append("LEFT JOIN GDA_PERSONA_USER_DETAILS PUD (NOLOCK) ON PUD.IDGDA_PERSONA_USER = PU.IDGDA_PERSONA_USER   ");
            sb.Append("LEFT JOIN GDA_SITE C (NOLOCK) ON C.IDGDA_SITE  = PUD.SITE  ");
            sb.Append("LEFT JOIN GDA_PERSONA_USER_HOBBY   PUH (NOLOCK) ON PUH.IDGDA_PERSONA_USER = PU.IDGDA_PERSONA_USER  AND PUH.DELETED_AT IS NULL ");
            sb.Append("LEFT JOIN GDA_PERSONA_HOBBY	       PH  (NOLOCK) ON PH.IDGDA_PERSONA_HOBBY = PUH.IDGDA_PERSONA_HOBBY AND PH.DELETED_AT IS NULL  ");
            sb.Append("LEFT JOIN GDA_CITY CT (NOLOCK) ON CT.IDGDA_CITY = PUD.IDGDA_CITY ");
            sb.Append("LEFT JOIN GDA_STATE ST (NOLOCK) ON ST.IDGDA_STATE = PUD.IDGDA_STATE ");
            sb.Append($"LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) CD ON CD.IDGDA_COLLABORATORS = CU.IDGDA_COLLABORATORS AND CD.CREATED_AT >= '{dtAg}' ");
            sb.Append($"LEFT JOIN GDA_GROUPS (NOLOCK) GR ON GR.ID = CD.IDGDA_GROUP ");
            sb.Append($"LEFT JOIN GDA_SECTOR (NOLOCK) SEC ON SEC.IDGDA_SECTOR = CD.IDGDA_SECTOR ");
            sb.Append($"WHERE  PU.IDGDA_PERSONA_USER = {personaUser} ");
            sb.Append("GROUP BY  PH.IDGDA_PERSONA_HOBBY, PH.HOBBY  ");

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
                                string hobby = reader["HOBBY"].ToString();

                                string teste = reader["IDGDA_PERSONA_HOBBY"].ToString();

                                int idhobby = reader["IDGDA_PERSONA_HOBBY"].ToString() == "" ? 0 : int.Parse(reader["IDGDA_PERSONA_HOBBY"].ToString());

                                // Verifica se já existe um usuário com o mesmo nome na lista
                                PersonaUser existingUser = retorno.FirstOrDefault(u => u.NOME == reader["NOME"].ToString());

                                // Se o usuário ainda não existe na lista, cria um novo objeto PersonaUser e adiciona à lista
                                if (existingUser == null)
                                {
                                    PersonaUser newUser = new PersonaUser
                                    {
                                        NOME = reader["NOME"] != DBNull.Value ? reader["NOME"].ToString() : "",
                                        FLAGTAHTO = reader["FLAGTAHTO"] != DBNull.Value ? int.Parse(reader["FLAGTAHTO"].ToString()) : 0,
                                        BC = reader["BC"] != DBNull.Value ? reader["BC"].ToString() : "",
                                        IDADE = reader["IDADE"] != DBNull.Value ? int.Parse(reader["IDADE"].ToString()) : 0,
                                        NOME_SOCIAL = reader["NOME_SOCIAL"] != DBNull.Value ? reader["NOME_SOCIAL"].ToString() == "" ? reader["NOME"].ToString() : reader["NOME_SOCIAL"].ToString() : "",
                                        FOTO = reader["FOTO"] != DBNull.Value ? reader["FOTO"].ToString() : "",
                                        QUEM_E = reader["QUEM_E"] != DBNull.Value ? reader["QUEM_E"].ToString() : "",
                                        MOTIVACOES = reader["MOTIVACOES"] != DBNull.Value ? reader["MOTIVACOES"].ToString() : "",
                                        OBJETIVO = reader["OBJETIVO"] != DBNull.Value ? reader["OBJETIVO"].ToString() : "",
                                        EMAIL = reader["EMAIL"] != DBNull.Value ? reader["EMAIL"].ToString() : "",
                                        TELEFONE = reader["TELEFONE"] != DBNull.Value ? reader["TELEFONE"].ToString() : "",
                                        DATA_NASCIMENTO = reader["DATA_NASCIMENTO"] != DBNull.Value ? reader["DATA_NASCIMENTO"].ToString() : "",
                                        ID_UF = reader["ID_UF"] != DBNull.Value ? reader["ID_UF"].ToString() : "",
                                        UF = reader["UF"] != DBNull.Value ? reader["UF"].ToString() : "",
                                        ID_CIDADE = reader["ID_CIDADE"] != DBNull.Value ? reader["ID_CIDADE"].ToString() : "",
                                        CIDADE = reader["CIDADE"] != DBNull.Value ? reader["CIDADE"].ToString() : "",
                                        ID_SITE = reader["ID_SITE"] != DBNull.Value ? reader["ID_SITE"].ToString() : "",
                                        SITE = reader["SITE"] != DBNull.Value ? reader["SITE"].ToString() : "",
                                        IDADE_CALCULADA = reader["DATA_NASCIMENTO"] != DBNull.Value ? CalcularDiferencaEmAnos(Convert.ToDateTime(reader["DATA_NASCIMENTO"]), DateTime.Today) : 0,
                                        HOBBIES = new List<Hobbies>()
                                        // Instancia uma nova lista de hobbies
                                    };

                                    if (reader["DATA_NASCIMENTO"] != DBNull.Value)
                                    {
                                        TimeSpan diferenca = DateTime.Today.Subtract(Convert.ToDateTime(reader["DATA_NASCIMENTO"].ToString()));
                                        int anos = (int)(diferenca.Days / 365.25);
                                    }


                                    if (hobby != "")
                                    {
                                        newUser.HOBBIES.Add(new Hobbies { HOBBY = hobby, IDGDA_HOBBIES = idhobby }); // Adiciona o hobby à lista de hobbies do novo usuário
                                    }

                                    retorno.Add(newUser);   // Adiciona o novo usuário à lista de usuários
                                }
                                else
                                {
                                    existingUser.HOBBIES.Add(new Hobbies { HOBBY = hobby, IDGDA_HOBBIES = idhobby }); // Adiciona o hobby à lista de hobbies do usuário existente
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
            return retorno;
        }
        public static int CalcularDiferencaEmAnos(DateTime dataInicio, DateTime dataFim)
        {
            int anos = 0;
            try
            {
                // Calcular diferença entre as datas
                TimeSpan diferenca = dataFim.Subtract(dataInicio);

                // Calcular a diferença em anos
                anos = (int)(diferenca.Days / 365.25);
            }
            catch (Exception)
            {

            }


            return anos;
        }
        public static string whoIsHashtags(string whoIs, string grupo, string setor, string cargo, string moedas)
        {

            string retorno = "";
            try
            {
                grupo = grupo == null ? "" : grupo;
                setor = setor == null ? "" : setor;
                cargo = cargo == null ? "" : cargo;
                moedas = moedas == null ? "" : moedas;
                retorno = whoIs.Replace("#Grupo", grupo).Replace("#Setor", setor).Replace("#Cargo", cargo).Replace("#Moedas", moedas);
            }
            catch (Exception)
            {

            }

            return retorno;
        }

        public static Account listMyAccounts(string collaboratorId, int myPersonaId, int limit, int page)
        {
            Account retorno = new Account();
            List<Accounts> listAccount = new List<Accounts>();

            int totalInfo = Funcoes.QuantidadeMyAccounts(collaboratorId);
            int totalpage = (int)Math.Ceiling((double)totalInfo / limit);
            int offset = (page - 1) * limit;
            string dtAg = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

            retorno.TOTALPAGES = totalpage;
            retorno.PERSONAID = myPersonaId;

            StringBuilder sb = new StringBuilder();
            sb.Append($"DECLARE @searchAccount NVARCHAR(100) = '{collaboratorId}'   ");
            sb.Append("SELECT   ");
            sb.Append("PCU.IDGDA_PERSONA_USER AS IDGDA_PERSONA_USER,   ");
            sb.Append("MAX(PU.NAME) AS NOME,  ");
            sb.Append("MAX(PU.PICTURE) AS FOTO, ");
            sb.Append("PUT.TYPE AS TIPO,  ");
            sb.Append("COUNT(DISTINCT PF3.IDGDA_PERSONA_FOLLOWERS) AS FOLLOWEDBYME ");
            sb.Append("FROM GDA_PERSONA_COLLABORATOR_USER  PCU (NOLOCK)  ");
            sb.Append("INNER JOIN GDA_PERSONA_USER PU (NOLOCK) ON PU.IDGDA_PERSONA_USER = PCU.IDGDA_PERSONA_USER AND PU.DELETED_AT IS NULL  ");
            sb.Append("INNER JOIN GDA_PERSONA_USER_TYPE PUT (NOLOCK) ON PUT.IDGDA_PERSONA_USER_TYPE = PU.IDGDA_PERSONA_USER_TYPE  ");
            sb.Append("INNER JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) CD ON CD.IDGDA_COLLABORATORS = PCU.IDGDA_COLLABORATORS   ");
            sb.Append($"LEFT JOIN GDA_PERSONA_FOLLOWERS (NOLOCK) AS PF3 ON PF3.DELETED_AT IS NULL AND PF3.IDGDA_PERSONA_USER = {myPersonaId} AND PF3.IDGDA_PERSONA_USER_FOLLOWED = PU.IDGDA_PERSONA_USER  ");
            sb.Append($"WHERE 1=1 ");
            sb.Append($" AND CD.CREATED_AT >= '{dtAg}' ");
            sb.Append($" AND CD.IDGDA_COLLABORATORS = TRY_CAST(@searchAccount AS INT) ");
            sb.Append("GROUP BY PCU.IDGDA_PERSONA_USER, PUT.TYPE ");
            sb.Append($"ORDER BY PCU.IDGDA_PERSONA_USER  ");
            sb.Append($"OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY");

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
                                Accounts Acc = new Accounts();

                                Acc.IDGDA_PERSONA_USER = int.Parse(reader["IDGDA_PERSONA_USER"].ToString());
                                Acc.NOME = reader["NOME"].ToString();
                                Acc.FOTO = reader["FOTO"].ToString();
                                Acc.TIPO = reader["TIPO"].ToString();

                                Acc.FOLLOWED_BY_ME = reader["FOLLOWEDBYME"] != DBNull.Value ? Convert.ToInt32(reader["FOLLOWEDBYME"].ToString()) > 0 ? true : false : false;

                                listAccount.Add(Acc);
                            }

                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            retorno.ACCOUNTS = listAccount;
            return retorno;

        }

        public static Account listAccounts(string accountPersona, int myPersonaId, int limit, int page)
        {
            Account retorno = new Account();
            List<Accounts> listAccount = new List<Accounts>();

            int totalInfo = Funcoes.QuantidadeAccounts(accountPersona);
            int totalpage = (int)Math.Ceiling((double)totalInfo / limit);
            int offset = (page - 1) * limit;
            string dtAg = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            string filter = "";
            if (accountPersona != null)
            {
                filter = "AND (PU.NAME LIKE '%' + @searchAccount + '%' OR PU.IDGDA_PERSONA_USER = TRY_CAST(@searchAccount AS INT) OR PCU.IDGDA_COLLABORATORS = TRY_CAST(@searchAccount AS INT)  ) ";
            }

            retorno.TOTALPAGES = totalpage;
            retorno.PERSONAID = myPersonaId;

            StringBuilder sb = new StringBuilder();
            sb.Append($"DECLARE @searchAccount NVARCHAR(100) = '{accountPersona}'   ");
            sb.Append("SELECT   ");
            sb.Append("PCU.IDGDA_PERSONA_USER AS IDGDA_PERSONA_USER,   ");
            sb.Append("MAX(PU.NAME) AS NOME,  ");
            sb.Append("MAX(PU.PICTURE) AS FOTO, ");
            sb.Append("PUT.TYPE AS TIPO,  ");
            sb.Append("COUNT(DISTINCT PF3.IDGDA_PERSONA_FOLLOWERS) AS FOLLOWEDBYME ");
            sb.Append("FROM GDA_PERSONA_COLLABORATOR_USER  PCU (NOLOCK)  ");
            sb.Append("INNER JOIN GDA_PERSONA_USER PU (NOLOCK) ON PU.IDGDA_PERSONA_USER = PCU.IDGDA_PERSONA_USER AND PU.DELETED_AT IS NULL  ");
            sb.Append("INNER JOIN GDA_PERSONA_USER_TYPE PUT (NOLOCK) ON PUT.IDGDA_PERSONA_USER_TYPE = PU.IDGDA_PERSONA_USER_TYPE  ");
            //sb.Append("INNER JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) CD ON CD.IDGDA_COLLABORATORS = PCU.IDGDA_COLLABORATORS   ");
            sb.Append($"LEFT JOIN GDA_PERSONA_FOLLOWERS (NOLOCK) AS PF3 ON PF3.DELETED_AT IS NULL AND PF3.IDGDA_PERSONA_USER = {myPersonaId} AND PF3.IDGDA_PERSONA_USER_FOLLOWED = PU.IDGDA_PERSONA_USER  ");
            sb.Append($"WHERE 1=1 ");
            //sb.Append($" AND CD.CREATED_AT >= '{dtAg}' ");
            sb.Append($"{filter} ");
            sb.Append("GROUP BY PCU.IDGDA_PERSONA_USER, PUT.TYPE ");
            sb.Append($"ORDER BY PCU.IDGDA_PERSONA_USER  ");
            sb.Append($"OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY");

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
                                Accounts Acc = new Accounts();

                                Acc.IDGDA_PERSONA_USER = int.Parse(reader["IDGDA_PERSONA_USER"].ToString());
                                Acc.NOME = reader["NOME"].ToString();
                                Acc.FOTO = reader["FOTO"].ToString();
                                Acc.TIPO = reader["TIPO"].ToString();

                                Acc.FOLLOWED_BY_ME = reader["FOLLOWEDBYME"] != DBNull.Value ? Convert.ToInt32(reader["FOLLOWEDBYME"].ToString()) > 0 ? true : false : false;

                                listAccount.Add(Acc);
                            }

                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            retorno.ACCOUNTS = listAccount;
            return retorno;

        }
        public static List<Reaction> listReactions(string word)
        {
            string filterName = "";
            if (word != "")
            {
                filterName = $"AND NAME LIKE '%{word}%' ";
            }
            List<Reaction> retorno = new List<Reaction>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT IDGDA_PERSONA_POSTS_LIKES_REACTION, NAME, LINK_ICON, LINK_ICON_SELECTED  ");
            sb.Append("FROM GDA_PERSONA_POSTS_LIKES_REACTION (NOLOCK) ");
            sb.Append("WHERE DELETED_AT IS NULL ");
            sb.Append($"{filterName} ");

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
                                Reaction reaction = new Reaction();
                                reaction.ID_POST_LIKE_REACTION = int.Parse(reader["IDGDA_PERSONA_POSTS_LIKES_REACTION"].ToString());
                                reaction.NAME = reader["NAME"].ToString();
                                reaction.LINK_ICON = reader["LINK_ICON"].ToString();
                                reaction.LINK_ICON_SELECTED = reader["LINK_ICON_SELECTED"].ToString();
                                retorno.Add(reaction);
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
        public static List<Question> listQuestions(int quiz, string word)
        {
            string filterName = "";
            if (word != "")
            {
                filterName += $"AND QUESTION LIKE '%{word}%' ";
            }
            if (quiz != 0)
            {
                filterName += $"AND IDGDA_QUIZ = '{quiz}' ";
            }
            List<Question> retorno = new List<Question>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT IDGDA_QUIZ_QUESTION, IDGDA_QUIZ, QQ.IDGDA_QUIZ_QUESTION_TYPE, QQT.TYPE, QUESTION, TIME_ANSWER ");
            sb.Append("FROM GDA_QUIZ_QUESTION (NOLOCK) QQ ");
            sb.Append("INNER JOIN GDA_QUIZ_QUESTION_TYPE (NOLOCK) AS QQT ON QQT.IDGDA_QUIZ_QUESTION_TYPE = QQ.IDGDA_QUIZ_QUESTION_TYPE ");
            sb.Append("WHERE 1=1 ");
            sb.Append("AND QQ.DELETED_AT IS NULL ");
            sb.Append($"{filterName} ");
            sb.Append("ORDER BY IDGDA_QUIZ_QUESTION ASC ");

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
                                Question question = new Question();
                                question.IDGDA_QUIZ_QUESTION = int.Parse(reader["IDGDA_QUIZ_QUESTION"].ToString());
                                question.IDGDA_QUIZ = int.Parse(reader["IDGDA_QUIZ"].ToString());
                                question.IDGDA_TYPE = int.Parse(reader["IDGDA_QUIZ_QUESTION_TYPE"].ToString());
                                question.TYPE = reader["TYPE"].ToString();
                                question.QUESTION = reader["QUESTION"].ToString();
                                question.TIME_ANSWER = int.Parse(reader["TIME_ANSWER"].ToString());
                                retorno.Add(question);
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

        public static List<TypeQuestion> listTypeQuestion()
        {
            List<TypeQuestion> retorno = new List<TypeQuestion>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT * FROM GDA_QUIZ_QUESTION_TYPE (NOLOCK) ");

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
                                TypeQuestion TypeQuestion = new TypeQuestion();
                                TypeQuestion.IDGDA_QUIZ_QUESTION_TYPE = int.Parse(reader["IDGDA_QUIZ_QUESTION_TYPE"].ToString());
                                TypeQuestion.TYPE = reader["TYPE"].ToString();
                                retorno.Add(TypeQuestion);
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
        public static List<Answer> listAnswer(int question, string word)
        {
            string filterquestion = "";
            if (word != "")
            {
                filterquestion += $"AND QA.QUESTION LIKE '%{word}%' ";
            }
            if (question != 0)
            {
                filterquestion += $"AND QA.IDGDA_QUIZ_QUESTION = '{question}' ";
            }
            List<Answer> retorno = new List<Answer>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT QA.IDGDA_QUIZ_ANSWER, QUESTION, RIGHT_ANSWER, QAF.URL  ");
            sb.Append("FROM GDA_QUIZ_ANSWER (NOLOCK) QA ");
            sb.Append("LEFT JOIN GDA_QUIZ_ANSWER_FILES  (NOLOCK) AS QAF ON QAF.IDGDA_QUIZ_ANSWER = QA.IDGDA_QUIZ_ANSWER ");
            sb.Append("WHERE 1=1 ");
            sb.Append("AND DELETED_AT IS NULL ");
            sb.Append($"{filterquestion} ");
            sb.Append("ORDER BY QA.IDGDA_QUIZ_ANSWER ASC ");

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
                                Answer answer = new Answer();
                                answer.IDGDA_QUIZ_QUESTION = question;
                                answer.IDGDA_QUIZ_ANSWER = int.Parse(reader["IDGDA_QUIZ_ANSWER"].ToString());
                                answer.QUESTION = reader["QUESTION"].ToString();
                                answer.RIGHT_ANSWER = int.Parse(reader["RIGHT_ANSWER"].ToString());
                                answer.URL = reader["URL"].ToString();
                                retorno.Add(answer);
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


        public static List<QuizVerify> listQuizNotify(int personaId)
        {
            List<QuizVerify> retorno = new List<QuizVerify>();

            StringBuilder sb = new StringBuilder();
            sb.Append($"DECLARE @INPUTID INT; SET @INPUTID = {personaId}; ");
            sb.Append($"SELECT Q.IDGDA_QUIZ,  ");
            sb.Append($"	MAX(NN.IDGDA_NOTIFICATION) AS IDGDA_NOTIFICATION ");
            sb.Append($"FROM GDA_QUIZ (NOLOCK) AS Q ");
            sb.Append($"INNER JOIN GDA_QUIZ_USER (NOLOCK) AS QU ON QU.IDGDA_QUIZ = Q.IDGDA_QUIZ ");
            sb.Append($"INNER JOIN GDA_QUIZ_QUESTION (NOLOCK) AS QQ ON QQ.IDGDA_QUIZ = Q.IDGDA_QUIZ ");
            sb.Append($"AND QQ.DELETED_AT IS NULL ");
            sb.Append($"LEFT JOIN GDA_QUIZ_USER_ANSWER (NOLOCK) AS UA ON UA.IDGDA_QUIZ_QUESTION = QQ.IDGDA_QUIZ_QUESTION AND UA.IDGDA_QUIZ_USER = QU.IDGDA_QUIZ_USER ");
            sb.Append($"LEFT JOIN gda_notification (NOLOCK) AS NN ON NN.REFERER = Q.IDGDA_QUIZ ");
            sb.Append($"WHERE QU.IDGDA_PERSONA_USER = @INPUTID ");
            sb.Append($"AND Q.SENDED_AT IS NOT NULL AND Q.ENDED_AT > GETDATE() AND Q.DELETED_AT IS NULL ");
            sb.Append($"GROUP BY Q.IDGDA_QUIZ ");
            sb.Append($"HAVING  ");
            sb.Append($"    COUNT(*) > SUM(CASE WHEN UA.IDGDA_QUIZ_USER_ANSWER IS NULL THEN 0 ELSE 1 END); ");


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
                                QuizVerify qv = new QuizVerify();

                                qv.IDGDA_QUIZ = int.Parse(reader["IDGDA_QUIZ"].ToString());
                                qv.IDGDA_NOTIFICATION = reader["IDGDA_NOTIFICATION"] != DBNull.Value ? int.Parse(reader["IDGDA_NOTIFICATION"].ToString()) : (int?)null;

                                retorno.Add(qv);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Trate a exceção aqui
                }
                connection.Close();
            }
            return retorno;
        }

        public static List<Quiz> listQuiz(int quiz, int idQuizUser, int personaId, int collaboratorId)
        {
            int quizId = 0;
            int qtdQuestion = 0;
            int qtdAnswer = 0;
            int qtdNoAnswer = 0;
            int questionId = 0;
            int typerid = 0;
            double Percent = 0;
            double quizmonetizado = 0;
            double jaMonetizado = 0;
            double atgmoentizacao = 0;
            List<Quiz> retorno = new List<Quiz>();

            StringBuilder sb = new StringBuilder();
            sb.Append($"DECLARE @INPUTID INT; SET @INPUTID = {personaId}; ");
            sb.Append($"DECLARE @QUIZID INT; SET @QUIZID = {quiz}; ");
            sb.Append(" ");
            sb.Append("SELECT Q.IDGDA_QUIZ,  ");
            sb.Append("MAX(Q.TITLE) AS TITULO, ");
            sb.Append("COUNT(0) AS QTD_PERGUNTAS,  ");
            sb.Append("SUM(CASE WHEN UA.NO_ANSWER IS NULL THEN 0 ELSE UA.NO_ANSWER END) AS QTD_NAORESPONDIDAS,  ");
            sb.Append("SUM(CASE WHEN UA.IDGDA_QUIZ_USER_ANSWER IS NULL THEN 0 ELSE 1 END) AS QTD_RESPOSTAS, ");
            sb.Append("(CAST(SUM(CASE WHEN UA.IDGDA_QUIZ_USER_ANSWER IS NULL THEN 0 ELSE 1 END) AS FLOAT) / COUNT(0)) * 100 AS PERCENTUAL, ");

            sb.Append("(SELECT TOP 1 QQ.IDGDA_QUIZ_QUESTION FROM GDA_QUIZ_USER (NOLOCK) AS QU ");
            sb.Append("LEFT JOIN GDA_QUIZ_QUESTION (NOLOCK) AS QQ ON QU.IDGDA_QUIZ = QQ.IDGDA_QUIZ AND QQ.DELETED_AT IS NULL ");
            sb.Append("LEFT JOIN GDA_QUIZ_USER_ANSWER (NOLOCK) AS QUA ON QUA.IDGDA_QUIZ_USER = QU.IDGDA_QUIZ_USER AND QUA.IDGDA_QUIZ_QUESTION = QQ.IDGDA_QUIZ_QUESTION ");
            sb.Append($"WHERE QU.IDGDA_QUIZ = {quiz} AND QU.IDGDA_PERSONA_USER = @INPUTID AND QUA.IDGDA_QUIZ_USER_ANSWER IS NULL ");
            sb.Append("ORDER BY QQ.IDGDA_QUIZ_QUESTION) AS NEXT_QUESTION, ");

            //sb.Append("(SELECT TOP 1 Q.IDGDA_QUIZ_QUESTION  FROM GDA_QUIZ_QUESTION (NOLOCK) AS Q ");
            //sb.Append("LEFT JOIN GDA_QUIZ_USER_ANSWER AS QU ON QU.IDGDA_QUIZ_QUESTION = Q.IDGDA_QUIZ_QUESTION ");
            //sb.Append($"WHERE IDGDA_QUIZ = {quiz} AND QU.IDGDA_QUIZ_USER_ANSWER IS NULL ");
            //sb.Append("ORDER BY Q.IDGDA_QUIZ_QUESTION) AS NEXT_QUESTION, ");
            sb.Append("MAX(QQ.IDGDA_QUIZ_QUESTION_TYPE) AS TYPE, ");
            sb.Append("ISNULL(MAX(Q.MONETIZATION), 0)  AS MONETIZADO, ");
            sb.Append("ISNULL(MAX(Q.PERCENT_MONETIZATION), 0) AS PERCENTUAL_MONETIZACAO, ");
            sb.Append("MAX(MONETIZED) AS MONETIZED ");
            sb.Append("FROM GDA_QUIZ (NOLOCK) AS Q ");
            sb.Append("INNER JOIN GDA_QUIZ_USER (NOLOCK) AS QU ON QU.IDGDA_QUIZ = Q.IDGDA_QUIZ AND IDGDA_PERSONA_USER = @INPUTID ");
            sb.Append("INNER JOIN GDA_QUIZ_QUESTION (NOLOCK) AS QQ ON QQ.IDGDA_QUIZ = Q.IDGDA_QUIZ AND QQ.DELETED_AT IS NULL ");
            sb.Append("LEFT JOIN GDA_QUIZ_USER_ANSWER (NOLOCK) AS UA ON UA.IDGDA_QUIZ_QUESTION = QQ.IDGDA_QUIZ_QUESTION AND UA.IDGDA_QUIZ_USER = QU.IDGDA_QUIZ_USER ");
            sb.Append("WHERE Q.IDGDA_QUIZ = @QUIZID AND Q.DELETED_AT IS NULL AND Q.STARTED_AT <= GETDATE() AND Q.ENDED_AT >= GETDATE() ");
            sb.Append("GROUP BY Q.IDGDA_QUIZ ");

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
                                quizId = int.Parse(reader["IDGDA_QUIZ"].ToString());
                                qtdQuestion = int.Parse(reader["QTD_PERGUNTAS"].ToString());
                                qtdAnswer = int.Parse(reader["QTD_RESPOSTAS"].ToString());
                                qtdNoAnswer = int.Parse(reader["QTD_NAORESPONDIDAS"].ToString());
                                questionId = reader["NEXT_QUESTION"] != DBNull.Value ? int.Parse(reader["NEXT_QUESTION"].ToString()) : 0;
                                Percent = double.Parse(reader["PERCENTUAL"].ToString());
                                quizmonetizado = double.Parse(reader["MONETIZADO"].ToString());
                                atgmoentizacao = double.Parse(reader["PERCENTUAL_MONETIZACAO"].ToString());

                                jaMonetizado = reader["MONETIZED"] != DBNull.Value ? int.Parse(reader["MONETIZED"].ToString()) : 0;
                                // Verifica se o quiz já foi adicionado à lista
                                Quiz quizObj = retorno.FirstOrDefault(q => q.IDGDA_QUIZ == quizId);
                                if (quizObj == null)
                                {
                                    quizObj = new Quiz
                                    {
                                        IDGDA_QUIZ = quizId,
                                        TITLE = reader["TITULO"].ToString(),
                                        QTD_QUESTION = qtdQuestion,
                                        QTD_ANSWER = qtdAnswer,
                                        PERCENT = Percent,
                                        QUESTION = new List<Question>()
                                    };
                                    retorno.Add(quizObj);
                                }
                            }
                        }
                    }

                    if (quizmonetizado > 0 && jaMonetizado == 0 && Percent >= atgmoentizacao && qtdNoAnswer == 0)
                    {
                        StringBuilder InsertMonetization = new StringBuilder();
                        InsertMonetization.Append("INSERT GDA_CHECKING_ACCOUNT (INPUT, BALANCE, COLLABORATOR_ID, CREATED_AT, GDA_INDICATOR_IDGDA_INDICATOR, OBSERVATION, REASON, WEIGHT, RESULT_DATE) ");
                        InsertMonetization.Append("VALUES ");
                        InsertMonetization.Append($"({quizmonetizado},ISNULL((SELECT TOP 1 BALANCE + {quizmonetizado} FROM GDA_CHECKING_ACCOUNT (NOLOCK) WHERE COLLABORATOR_ID = {collaboratorId} ORDER BY CREATED_AT DESC), {quizmonetizado}), {collaboratorId}, GETDATE(), NULL, 'QUIZ', '{retorno[0].TITLE.ToString()}', 1, GETDATE()) ");
                        using (SqlCommand command = new SqlCommand(InsertMonetization.ToString(), connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        StringBuilder UpdateMonetization = new StringBuilder();
                        UpdateMonetization.Append($"UPDATE GDA_QUIZ_USER SET  MONETIZED = 1 WHERE IDGDA_QUIZ_USER = {idQuizUser} ");
                        using (SqlCommand command = new SqlCommand(UpdateMonetization.ToString(), connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }

                    StringBuilder sb2 = new StringBuilder();
                    sb2.Append("SELECT   ");
                    sb2.Append("QQ.IDGDA_QUIZ_QUESTION AS IDGDA_QUIZ_QUESTION, ");
                    sb2.Append("QQ.QUESTION AS PERGUNTA, ");
                    sb2.Append("QQF.URL AS URL_QUESTION, ");
                    sb2.Append("QQ.TIME_ANSWER AS TIME_ANSWER, ");
                    sb2.Append("QA.IDGDA_QUIZ_ANSWER AS IDGDA_QUIZ_ANSWER, ");
                    sb2.Append("QQ.IDGDA_QUIZ_QUESTION_TYPE AS IDTYPE, ");
                    sb2.Append("QQT.TYPE AS TYPE, ");
                    sb2.Append("QA.QUESTION AS RESPOSTA, ");
                    sb2.Append("QA.RIGHT_ANSWER AS RIGHT_ANSWER, ");
                    sb2.Append("QAF.URL AS URL, Q.ORIENTATION ");
                    sb2.Append("FROM GDA_QUIZ_QUESTION (NOLOCK) AS QQ  ");
                    sb2.Append("LEFT JOIN GDA_QUIZ(NOLOCK) AS Q ON Q.IDGDA_QUIZ = QQ.IDGDA_QUIZ ");
                    sb2.Append("LEFT JOIN GDA_QUIZ_ANSWER (NOLOCK) AS QA ON QQ.IDGDA_QUIZ_QUESTION = QA.IDGDA_QUIZ_QUESTION ");
                    sb2.Append("LEFT JOIN GDA_QUIZ_ANSWER_FILES (NOLOCK) AS QAF ON QAF.IDGDA_QUIZ_ANSWER = QA.IDGDA_QUIZ_ANSWER ");
                    sb2.Append("LEFT JOIN GDA_QUIZ_QUESTION_FILES (NOLOCK) AS QQF ON QQF.IDGDA_QUIZ_QUESTION = QQ.IDGDA_QUIZ_QUESTION ");
                    sb2.Append("LEFT JOIN GDA_QUIZ_QUESTION_TYPE (NOLOCK) AS QQT ON QQT.IDGDA_QUIZ_QUESTION_TYPE = QQ.IDGDA_QUIZ_QUESTION_TYPE ");
                    sb2.Append($"WHERE QQ.IDGDA_QUIZ_QUESTION = {questionId} ");
                    using (SqlCommand command = new SqlCommand(sb2.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Verifica se a pergunta já foi adicionada ao quiz
                                Question questionObj = retorno.First().QUESTION.FirstOrDefault(q => q.IDGDA_QUIZ_QUESTION == questionId);
                                if (questionObj == null)
                                {
                                    questionObj = new Question
                                    {
                                        IDGDA_QUIZ_QUESTION = questionId,
                                        QUESTION = reader["PERGUNTA"].ToString(),
                                        URL_QUESTION = reader["URL_QUESTION"].ToString(),
                                        IDTYPE = int.Parse(reader["IDTYPE"].ToString()),
                                        TYPE = reader["TYPE"].ToString(),
                                        TIME_ANSWER = int.Parse(reader["TIME_ANSWER"].ToString()),
                                        ORIENTATION = reader["ORIENTATION"] != null ? reader["ORIENTATION"].ToString() : "",
                                        ANSWER = new List<Answer>()
                                    };
                                    retorno[0].QUESTION.Add(questionObj);
                                }

                                if (int.Parse(reader["IDTYPE"].ToString()) != 4)
                                {

                                    if (int.Parse(reader["IDGDA_QUIZ_ANSWER"].ToString()) != 0)
                                    {
                                        // Adiciona a resposta à pergunta
                                        Answer answer = new Answer
                                        {
                                            IDGDA_QUIZ_ANSWER = int.Parse(reader["IDGDA_QUIZ_ANSWER"].ToString()) != null ? int.Parse(reader["IDGDA_QUIZ_ANSWER"].ToString()) : 0,
                                            QUESTION = reader["RESPOSTA"].ToString(),
                                            URL = reader["URL"].ToString(),
                                            RIGHT_ANSWER = int.Parse(reader["RIGHT_ANSWER"].ToString()) != null ? int.Parse(reader["RIGHT_ANSWER"].ToString()) : 0,

                                        };
                                        retorno[0].QUESTION[0].ANSWER.Add(answer);
                                    }
                                }


                            }
                        }

                        //caso não tenha mais perguntas.. Marcar como answered
                        if (retorno[0].QUESTION.Count() == 0)
                        {
                            StringBuilder UpdateMonetization = new StringBuilder();
                            UpdateMonetization.Append($"UPDATE GDA_QUIZ_USER SET ANSWERED = 1 WHERE IDGDA_QUIZ_USER = {idQuizUser} ");
                            using (SqlCommand command2 = new SqlCommand(UpdateMonetization.ToString(), connection))
                            {
                                command2.ExecuteNonQuery();
                            }
                            retorno.Clear();
                        }


                    }
                }
                catch (Exception ex)
                {
                    // Trate a exceção aqui
                }
                connection.Close();
            }
            return retorno;
        }
        public static List<MyQuiz> listMyQuizs(int personaId)
        {
            List<MyQuiz> retorno = new List<MyQuiz>();

            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT   ");
            sb.Append("QU.IDGDA_QUIZ_USER AS IDGDA_QUIZ_USER,  ");
            sb.Append("QU.IDGDA_QUIZ AS IDGDA_QUIZ,  ");
            sb.Append("Q.TITLE AS TITLE,  ");
            sb.Append("Q.STARTED_AT AS STARTED_AT,  ");
            sb.Append("Q.ENDED_AT AS ENDED_AT,  ");
            sb.Append("Q.REQUIRED AS REQUIRED, ");
            sb.Append("COUNT(QQ.IDGDA_QUIZ_QUESTION) AS PERGUNTAS,  ");
            sb.Append("COUNT(QUA.IDGDA_QUIZ_QUESTION) AS RESPOSTAS  ");
            sb.Append("FROM GDA_QUIZ_USER (NOLOCK) QU  ");
            sb.Append("INNER JOIN GDA_QUIZ (NOLOCK) AS Q ON Q.IDGDA_QUIZ = QU.IDGDA_QUIZ  ");
            sb.Append("INNER JOIN GDA_QUIZ_QUESTION (NOLOCK) AS QQ ON QQ.IDGDA_QUIZ = Q.IDGDA_QUIZ AND QQ.DELETED_AT IS NULL ");
            sb.Append("LEFT JOIN GDA_QUIZ_USER_ANSWER (NOLOCK) AS QUA ON QUA.IDGDA_QUIZ_QUESTION = QQ.IDGDA_QUIZ_QUESTION AND QUA.IDGDA_QUIZ_USER = QU.IDGDA_QUIZ_USER  ");
            sb.Append("WHERE Q.DELETED_AT IS NULL ");
            sb.Append($"AND IDGDA_PERSONA_USER = {personaId} ");
            sb.Append("GROUP BY QU.IDGDA_QUIZ_USER, QU.IDGDA_QUIZ, Q.REQUIRED, Q.TITLE,Q.STARTED_AT,Q.ENDED_AT   ");
            sb.Append($"ORDER BY Q.STARTED_AT DESC ");

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
                                MyQuiz myQuiz = new MyQuiz();
                                if (reader["IDGDA_QUIZ"].ToString() != "")
                                {
                                    myQuiz.IDGDA_QUIZ_USER = int.Parse(reader["IDGDA_QUIZ_USER"].ToString());
                                    myQuiz.IDGDA_QUIZ = int.Parse(reader["IDGDA_QUIZ"].ToString());
                                    myQuiz.TITLE = reader["TITLE"].ToString();
                                    //myQuiz.STARTED_AT = reader["STARTED_AT"].ToString();

                                    myQuiz.STARTED_AT = reader["STARTED_AT"] != DBNull.Value ? Convert.ToDateTime(reader["STARTED_AT"]) : (DateTime?)null;
                                    myQuiz.ENDED_AT = reader["ENDED_AT"] != DBNull.Value ? Convert.ToDateTime(reader["ENDED_AT"]) : (DateTime?)null;


                                    //myQuiz.ENDED_AT = reader["ENDED_AT"].ToString();
                                    myQuiz.REQUIRED = int.Parse(reader["REQUIRED"].ToString());
                                    double perguntas = int.Parse(reader["PERGUNTAS"].ToString());
                                    double repostas = int.Parse(reader["RESPOSTAS"].ToString());
                                    double porcentagem = repostas == 0 || perguntas == 0 ? porcentagem = 0 : (repostas / perguntas) * 100;

                                    string dataFinalizacaoString = reader["ENDED_AT"].ToString();
                                    DateTime dataFinalizacao;
                                    if (DateTime.TryParse(dataFinalizacaoString, out dataFinalizacao))
                                    {

                                        DateTime dtNow = DateTime.Now;
                                        DateTime endDate = Convert.ToDateTime(dataFinalizacaoString);

                                        if (endDate < dtNow)
                                        {
                                            myQuiz.STATUS = "Concluido";
                                        }
                                        else if (porcentagem == 100)
                                        {
                                            myQuiz.STATUS = "Concluido";
                                        }
                                        else if (porcentagem == 0)
                                        {
                                            myQuiz.STATUS = "Não Iniciado";
                                        }
                                        else
                                        {
                                            myQuiz.STATUS = "Pendente";
                                        }
                                        myQuiz.CONCLUED = $"% {porcentagem.ToString("F2")}";
                                    }
                                    retorno.Add(myQuiz);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Trate a exceção aqui
                }
                connection.Close();
            }
            return retorno;
        }
        public static List<DayLogged> listDayLogged(int collaboratorId)
        {
            List<DayLogged> retorno = new List<DayLogged>();

            // Obter a data atual
            DateTime dataAtual = DateTime.Today;

            //Obter todas as datas da semana atual.
            List<DateTime> weekDates = Funcoes.GetWeekDates(dataAtual);

            // Loop para obter as datas e verificar se houve acesso.
            for (int i = 0; i <= weekDates.Count - 1; i++)
            {
                string iData = weekDates[i].Date.ToString("yyyy-MM-dd");
                string Dia = weekDates[i].DayOfWeek.ToString();
                string diaDaSemana = Funcoes.RetornoDiaSemana(Dia);
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT IDGDA_LOGIN_ACCESS FROM GDA_LOGIN_ACCESS NOLOCK   ");
                sb.Append($"WHERE IDGDA_COLLABORATOR = {collaboratorId}  ");
                sb.Append($"AND CONVERT(DATE,DATE_ACCESS) = ('{iData}')   ");

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
                                    DayLogged AcessLogin = new DayLogged();
                                    AcessLogin.DAY = diaDaSemana;
                                    AcessLogin.ACRONYM = diaDaSemana.Substring(0, 1);
                                    AcessLogin.LOGIN = 1;
                                    AcessLogin.DATE = iData;
                                    retorno.Add(AcessLogin);
                                }
                                else
                                {
                                    DayLogged AcessLogin = new DayLogged();
                                    AcessLogin.DAY = diaDaSemana;
                                    AcessLogin.ACRONYM = diaDaSemana.Substring(0, 1);
                                    AcessLogin.LOGIN = 0;
                                    AcessLogin.DATE = iData;
                                    retorno.Add(AcessLogin);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Trate a exceção aqui
                    }
                    connection.Close();
                }
            }
            // Reordenar a lista de retorno de acordo com a ordem dos dias da semana
            //retorno = retorno.OrderBy(d => Array.IndexOf(weekDays, d.DAY)).ToList();


            return retorno;
        }

        public static List<ListDayLoggeds> listDayLoggedsFilter(int personaID, string dtInicial, string dtFinal)
        {
            List<ListDayLoggeds> retorno = new List<ListDayLoggeds>();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT DATE_ACCESS, TEMPOLOGIN FROM GDA_LOGIN_ACCESS (NOLOCK) ");
            sb.Append($"WHERE IDGDA_COLLABORATOR = {personaID} ");
            sb.Append($"AND DATE_ACCESS >= '{dtInicial}' ");
            sb.Append($"AND DATE_ACCESS <= '{dtFinal}' ");
            sb.Append("ORDER BY DATE_ACCESS DESC ");

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
                                ListDayLoggeds ListAcessLogin = new ListDayLoggeds();
                                ListAcessLogin.DATA = reader["DATE_ACCESS"].ToString();
                                ListAcessLogin.TEMPO_LOGIN = reader["TEMPOLOGIN"].ToString();
                                retorno.Add(ListAcessLogin);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Trate a exceção aqui
                }
                connection.Close();
            }
            return retorno;
        }
        public static List<MonetizationConfigDay> MonetizationConfigDay(int referer, int filtertype)
        {
            string filter = "";
            if (referer != 0)
            {
                filter += $"AND ID_REFERER = {referer}   ";
            }
            if (filtertype != 0)
            {
                filter += $"AND MG.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE = '{filtertype}'  ";
            }

            List<MonetizationConfigDay> retorno = new List<MonetizationConfigDay>();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT  ");
            sb.AppendFormat("IDGDA_MONETIZATION_CONFIG, ");
            sb.AppendFormat("DAYS, ");
            sb.AppendFormat("CREATED_AT, ");
            sb.AppendFormat("REPROCESSED, ");
            sb.AppendFormat("STARTED_AT, ");
            sb.AppendFormat("MEFT.TYPE, ");
            sb.AppendFormat("ID_REFERER  ");
            sb.AppendFormat("FROM GDA_MONETIZATION_CONFIG (NOLOCK) MG ");
            sb.AppendFormat("INNER JOIN GDA_MONETIZATION_EXPIRED_FILTER_TYPE (NOLOCK) AS MEFT ON MEFT.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE = MG.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE ");
            sb.AppendFormat("WHERE DELETED_AT IS NULL ");
            sb.AppendFormat($"{filter}  ");

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
                                MonetizationConfigDay ConfigDay = new MonetizationConfigDay();

                                ConfigDay.IDGDA_MONETIZATION_CONFIG = Convert.ToInt32(reader["IDGDA_MONETIZATION_CONFIG"].ToString());
                                ConfigDay.DAYS = Convert.ToInt32(reader["DAYS"].ToString());
                                ConfigDay.CREATED_AT = reader["CREATED_AT"].ToString();
                                ConfigDay.REPROCESSED = Convert.ToInt32(reader["REPROCESSED"].ToString());
                                ConfigDay.STARTED_AT = reader["STARTED_AT"].ToString();
                                ConfigDay.TYPE = reader["TYPE"].ToString();
                                ConfigDay.REFERER = Convert.ToInt32(reader["ID_REFERER"].ToString());
                                retorno.Add(ConfigDay);
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
        public static List<MonetizationConfigPause> MonetizationConfigPause()
        {
            List<MonetizationConfigPause> retorno = new List<MonetizationConfigPause>();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT      ");
            sb.AppendFormat("PAUSE_AT, ");
            sb.AppendFormat("PAUSE, ");
            sb.AppendFormat("REPROCESSED ");
            sb.AppendFormat("FROM GDA_MONETIZATION_CONFIG_PAUSE (NOLOCK) MCP ");
            sb.AppendFormat("WHERE DELETED_AT IS NULL ");

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
                                MonetizationConfigPause ConfigPause = new MonetizationConfigPause();
                                ConfigPause.PAUSE_AT = reader["PAUSE_AT"].ToString();
                                ConfigPause.PAUSE = Convert.ToInt32(reader["PAUSE"].ToString());
                                ConfigPause.REPROCESSED = Convert.ToInt32(reader["REPROCESSED"].ToString());
                                retorno.Add(ConfigPause);
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

        public static List<MonetizationConfigType> MonetizationConfigType()
        {
            List<MonetizationConfigType> retorno = new List<MonetizationConfigType>();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT * FROM GDA_MONETIZATION_EXPIRED_FILTER_TYPE (NOLOCK)  ");

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
                                MonetizationConfigType ConfigType = new MonetizationConfigType();
                                ConfigType.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE = Convert.ToInt32(reader["IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE"].ToString());
                                ConfigType.TYPE = reader["TYPE"].ToString();
                                retorno.Add(ConfigType);
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

        public static List<ConfigNotificationExpire> ConfigNotificationExpire()
        {
            List<ConfigNotificationExpire> retorno = new List<ConfigNotificationExpire>();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT * FROM GDA_CONFIG_NOTIFICATION (NOLOCK)  WHERE DELETED_AT IS NULL ");

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
                                ConfigNotificationExpire ConfigNotificationExpire = new ConfigNotificationExpire();
                                ConfigNotificationExpire.IDGDA_CONFIG_NOTIFICATION = Convert.ToInt32(reader["IDGDA_CONFIG_NOTIFICATION"].ToString());
                                ConfigNotificationExpire.TYPE_NOTIFICATION = Convert.ToInt32(reader["TYPE_NOTIFICATION"].ToString());
                                ConfigNotificationExpire.DAYS = Convert.ToInt32(reader["DAYS"].ToString());
                                retorno.Add(ConfigNotificationExpire);
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
    }
}