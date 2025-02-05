using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ApiRepositorio.Class;
using ApiRepositorio.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Formatting;
using System.Web;
using ApiC.Class;
using ApiC.Class.DowloadFile;
using static ApiC.Class.bankMktplace;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class TesteController : ApiController
    {
        public static void corrigirHistoricoVoucher()
        {
            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("	WITH AggregatedData AS ( ");
            stb.AppendFormat("    SELECT  ");
            stb.AppendFormat("		SP.GDA_PRODUCT_IDGDA_PRODUCT, ");
            stb.AppendFormat("		SP.GDA_STOCK_IDGDA_STOCK, ");
            stb.AppendFormat("        SP.REGISTERED_BY, ");
            stb.AppendFormat("        SUM(SP.amount_output) AS TotalAmountOutput, ");
            stb.AppendFormat("        MAX(OPP.QTD) AS MaxQTD ");
            stb.AppendFormat("    FROM GDA_HISTORY_STOCK_PRODUCT (nolock) AS SP ");
            stb.AppendFormat("	inner join GDA_PRODUCT (NOLOCK) AS P ON P.IDGDA_PRODUCT = sp.GDA_PRODUCT_IDGDA_PRODUCT and p.type = 'DIGITAL' ");
            stb.AppendFormat("    LEFT JOIN ( ");
            stb.AppendFormat("        SELECT  ");
            stb.AppendFormat("            sum(amount) AS QTD,  ");
            stb.AppendFormat("            OP.GDA_PRODUCT_IDGDA_PRODUCT,  ");
            stb.AppendFormat("            OP.GDA_STOCK_IDGDA_STOCK,  ");
            stb.AppendFormat("            O.ORDER_BY  ");
            stb.AppendFormat("        FROM GDA_ORDER_PRODUCT AS OP ");
            stb.AppendFormat("        INNER JOIN GDA_ORDER AS O  ");
            stb.AppendFormat("            ON OP.GDA_ORDER_IDGDA_ORDER = O.IDGDA_ORDER ");
            stb.AppendFormat("        GROUP BY  ");
            stb.AppendFormat("            OP.GDA_PRODUCT_IDGDA_PRODUCT,  ");
            stb.AppendFormat("            OP.GDA_STOCK_IDGDA_STOCK,  ");
            stb.AppendFormat("            O.ORDER_BY ");
            stb.AppendFormat("    ) AS OPP  ");
            stb.AppendFormat("    ON OPP.GDA_PRODUCT_IDGDA_PRODUCT = SP.GDA_PRODUCT_IDGDA_PRODUCT  ");
            stb.AppendFormat("    AND OPP.GDA_STOCK_IDGDA_STOCK = SP.GDA_STOCK_IDGDA_STOCK  ");
            stb.AppendFormat("    AND OPP.ORDER_BY = SP.REGISTERED_BY ");
            stb.AppendFormat("    WHERE  ");
            stb.AppendFormat("        SP.GDA_REASON_REMOVED_IDGDA_REASON_REMOVED = 9  ");
            stb.AppendFormat("    GROUP BY  ");
            stb.AppendFormat("        SP.GDA_PRODUCT_IDGDA_PRODUCT, SP.GDA_STOCK_IDGDA_STOCK, SP.REGISTERED_BY ");
            stb.AppendFormat(") ");
            stb.AppendFormat("SELECT  ");
            stb.AppendFormat("	GDA_PRODUCT_IDGDA_PRODUCT, ");
            stb.AppendFormat("	GDA_STOCK_IDGDA_STOCK, ");
            stb.AppendFormat("    REGISTERED_BY, ");
            stb.AppendFormat("    TotalAmountOutput, ");
            stb.AppendFormat("    MaxQTD ");
            stb.AppendFormat("FROM AggregatedData ");
            stb.AppendFormat("WHERE  ");
            stb.AppendFormat("    MaxQTD IS NULL OR TotalAmountOutput > MaxQTD; ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int productId = Convert.ToInt32(reader["GDA_PRODUCT_IDGDA_PRODUCT"].ToString());
                                int stockId = Convert.ToInt32(reader["GDA_STOCK_IDGDA_STOCK"].ToString());
                                int orderById = Convert.ToInt32(reader["REGISTERED_BY"].ToString());
                                int TotalHistorico = Convert.ToInt32(reader["TotalAmountOutput"].ToString());
                                int QtdBuy = reader["MaxQTD"] != DBNull.Value ? Convert.ToInt32(reader["MaxQTD"].ToString()) : 0;
                             
                                int qtdAltered = 0;
                                while (TotalHistorico > QtdBuy)
                                {
                                    int qtdHist = historico(stockId, orderById, productId);
                                    qtdAltered += qtdHist;
                                    UpdateVoucherDelete(qtdHist, stockId, productId);

                                    QtdBuy += qtdHist;
                                }

                                UpdateStockProduct(qtdAltered, stockId, productId);

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

        public static int historico(int idStock, int idRegistered, int idProduct)
        {
           int amountEnv = 0;

            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("SELECT IDGDA_HISTORY_STOCK_PRODUCT, AMOUNT_OUTPUT  FROM GDA_HISTORY_STOCK_PRODUCT ");
            stb.AppendFormat($"WHERE GDA_STOCK_IDGDA_STOCK = {idStock} AND REGISTERED_BY = {idRegistered} AND GDA_PRODUCT_IDGDA_PRODUCT = {idProduct} AND AMOUNT_OUTPUT > 0 ");
            stb.AppendFormat("ORDER BY CREATED_AT DESC ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int idHistorico = Convert.ToInt32(reader["IDGDA_HISTORY_STOCK_PRODUCT"].ToString());
                                amountEnv = Convert.ToInt32(reader["AMOUNT_OUTPUT"].ToString());

                                deleteHistorico(idHistorico);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            return amountEnv;
        }

        public static void UpdateVoucherDelete(int qtdVoucher, int idStock, int idProduct)
        {


            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("UPDATE GDA_VOUCHERS SET STATUS = 'AVAILABLE' ");
            stb.AppendFormat($"where IDGDA_VOUCHERS in (select top {qtdVoucher} IDGDA_VOUCHERS from GDA_VOUCHERS where productId = {idProduct} and stockId = {idStock} and status = 'RESERVED'  ");
            stb.AppendFormat("order by voucherValidity asc)");


            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
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

        public static void deleteHistorico(int idHistorico)
        {


            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("DELETE FROM GDA_HISTORY_STOCK_PRODUCT ");
            stb.AppendFormat($"WHERE IDGDA_HISTORY_STOCK_PRODUCT = {idHistorico} ");


            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
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

        public static void UpdateStockProduct(int qtd, int idStock, int idProduct)
        {


            StringBuilder stb = new StringBuilder();
            stb.AppendFormat($"UPDATE GDA_STOCK_PRODUCT SET AMOUNT = AMOUNT + {qtd} ");
            stb.AppendFormat($"where GDA_STOCK_IDGDA_STOCK = {idStock} and GDA_PRODUCT_IDGDA_PRODUCT = {idProduct} ");



            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
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

        public class inputPost
        {
            public string report { get; set; }
            public Boolean monthCurrent { get; set; }


        }

        public static void resetSenha(string MATRICULA)
        {
            int validaUser = 0;
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(MATRICULA, 8);
            //string PASSWORD = BCrypt.Net.BCrypt.HashPassword(inputModel.PASSWORD, 8);
            //int IDCOLLABORATOR = inputModel.IDCOLLABORATOR;

            //Validar se ja temos inserido na base, caso nao contem realiza upate.

            //string Str = $"SELECT LOGIN  FROM GDA_USERS (NOLOCK) WHERE  Login = '{MATRICULA}'";
            string sqlCollaborator = $"SELECT ACTIVE FROM GDA_COLLABORATORS (NOLOCK) WHERE COLLABORATORIDENTIFICATION = '{MATRICULA}'";
            string Str = $"SELECT LOGIN, COLLABORATOR_ID  FROM GDA_USERS (NOLOCK) WHERE  Login = '{MATRICULA}'";
            string InsertUsers = $"INSERT INTO GDA_USERS (Login, Senha) VALUES ('{MATRICULA}','{hashedPassword}')";
            string updateUsers = $"UPDATE GDA_USERS SET SENHA = '{hashedPassword}' WHERE LOGIN = '{MATRICULA}'";
            string updateCollaborators = $"UPDATE GDA_COLLABORATORS SET FIRST_LOGIN = 1 WHERE MATRICULA = '{MATRICULA}'";


            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sqlCollaborator.ToString(), connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                if (reader["ACTIVE"].ToString() == "false")
                                {
                                    //throw new Exception("Não é possível resetar a senha, o colaborador está inativo");
                                }
                            }
                        }
                    }
                }

                using (SqlCommand command = new SqlCommand(Str.ToString(), connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["COLLABORATOR_ID"].ToString() != "")
                            {
                                validaUser = +1;
                            }
                        }
                    }
                }

                if (validaUser == 0)
                {
                    using (SqlCommand command = new SqlCommand(InsertUsers, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    using (SqlCommand command = new SqlCommand(updateCollaborators, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (SqlCommand command = new SqlCommand(updateUsers, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    using (SqlCommand command = new SqlCommand(updateCollaborators, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                connection.Close();
            }
        }

        //Função para corrigir diferença entre pedidos no historico de stoque, e pedidos no gda_order_product
        public static void OrganizaHistoricoPedidos()
        {
            StringBuilder stb = new StringBuilder();
            stb.AppendFormat("SELECT  P.REGISTERED_BY, P.GDA_STOCK_IDGDA_STOCK, P.GDA_PRODUCT_IDGDA_PRODUCT, MAX(AMOUNT) AS PEDIDOS, SUM(AMOUNT_OUTPUT) AS RETIRADOS ");
            stb.AppendFormat("FROM GDA_HISTORY_STOCK_PRODUCT AS P ");
            stb.AppendFormat("INNER JOIN GDA_PRODUCT (NOLOCK) AS PP ON PP.IDGDA_PRODUCT = P.GDA_PRODUCT_IDGDA_PRODUCT and PP.type = 'PHYSICAL' ");
            stb.AppendFormat("LEFT JOIN ( ");
            stb.AppendFormat("    SELECT SUM(AMOUNT) AS AMOUNT, GDA_STOCK_IDGDA_STOCK, GDA_PRODUCT_IDGDA_PRODUCT, O.ORDER_BY ");
            stb.AppendFormat("    FROM GDA_ORDER_PRODUCT AS OP ");
            stb.AppendFormat("	INNER JOIN GDA_ORDER AS O ON O.IDGDA_ORDER = OP.GDA_ORDER_IDGDA_ORDER ");
            stb.AppendFormat("	GROUP BY O.ORDER_BY, GDA_STOCK_IDGDA_STOCK, GDA_PRODUCT_IDGDA_PRODUCT ");
            stb.AppendFormat(") AS OP  ");
            stb.AppendFormat("    ON OP.GDA_STOCK_IDGDA_STOCK = P.GDA_STOCK_IDGDA_STOCK ");
            stb.AppendFormat("    AND OP.GDA_PRODUCT_IDGDA_PRODUCT = P.GDA_PRODUCT_IDGDA_PRODUCT ");
            stb.AppendFormat("	AND OP.ORDER_BY = P.REGISTERED_BY ");
            stb.AppendFormat("	WHERE P.GDA_REASON_REMOVED_IDGDA_REASON_REMOVED = 9  ");
            stb.AppendFormat("	group by P.GDA_STOCK_IDGDA_STOCK, P.GDA_PRODUCT_IDGDA_PRODUCT, P.REGISTERED_BY ");
            stb.AppendFormat("	HAVING ");
            stb.AppendFormat("MAX(AMOUNT) <> SUM(AMOUNT_OUTPUT) OR MAX(AMOUNT) IS NULL ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int registeredBy = Convert.ToInt32(reader["REGISTERED_BY"].ToString());
                                int productId = Convert.ToInt32(reader["GDA_PRODUCT_IDGDA_PRODUCT"].ToString());
                                int stockId = Convert.ToInt32(reader["GDA_STOCK_IDGDA_STOCK"].ToString());
                                int pedidos = reader["PEDIDOS"] != DBNull.Value ? Convert.ToInt32(reader["PEDIDOS"].ToString()) : 0;
                                int retirados = Convert.ToInt32(reader["RETIRADOS"].ToString());


                                int qtdAltered = 0;
                                while (retirados > pedidos)
                                {
                                    int qtdHist = historico(stockId, registeredBy, productId);
                                    qtdAltered += qtdHist;
                                    pedidos += qtdHist;
                                }

                                UpdateStockProduct(qtdAltered, stockId, productId);
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

        // POST: api/Results
        [HttpGet]
        public IHttpActionResult GetResultsModel(int idpersona)
        {
            //OrganizaHistoricoPedidos();
            //ScheduledNotification.ExecuteTaskScheduled(null);
            //corrigirHistoricoVoucher();
            //criarPersonaNovos();

            //Processo colocado na API de dados
            //performanceSemanalMensal.processoSemanalMensal();

            //ScheduledTask.reportsPreview3(null);
            //InsereVoucherErro();
            //CorrigirVoucherStock();

            //Insere notificação

            //ScheduledTask.homologGlass();
            //ScheduledNotification.insertNotificationMktPlace(12, "Novo Pedido MarketPlace", $"Você recebeu um novo pedido de NomeOperador - 11111111. Produtos: XPTO!", true, 11483, false);

            messageReturned msgInput = new messageReturned();
            msgInput.type = "Notification";
            msgInput.data = new dataMessage();
            msgInput.data.idUserReceive = idpersona;
            msgInput.data.idNotificationUser = 12;
            msgInput.data.idNotification = 1;
            msgInput.data.idUserSend = 1;
            msgInput.data.urlUserSend = "";
            msgInput.data.nameUserSend = "Nome Teste";
            msgInput.data.message = "Teste de mensagem";
            msgInput.data.urlFilesMessage = null;
            Startup.messageQueue.Enqueue(msgInput);

            return Ok("Teste");
        }

        public static void InsereVoucherErro()
        {
            StringBuilder stb = new StringBuilder();
            stb.Append("SELECT GDA_PRODUCT_IDGDA_PRODUCT, GDA_STOCK_IDGDA_STOCK, AMOUNT, ORDER_BY, IDGDA_ORDER FROM GDA_ORDER (NOLOCK) O ");
            stb.Append("INNER JOIN GDA_ORDER_PRODUCT (NOLOCK) OP ON O.IDGDA_ORDER = OP.GDA_ORDER_IDGDA_ORDER ");
            stb.Append("LEFT JOIN GDA_COLLABORATOR_VOUCHER (NOLOCK) CV ON CV.COLLABORATORID = O.ORDER_BY ");
            stb.Append("LEFT JOIN GDA_PRODUCT (NOLOCK) P ON P.IDGDA_PRODUCT = OP.GDA_PRODUCT_IDGDA_PRODUCT ");
            stb.Append("WHERE O.GDA_ORDER_STATUS_IDGDA_ORDER_STATUS = 2 AND CV.IDGDA_COLLABORATOR_VOUCHER IS NULL AND P.TYPE = 'DIGITAL' ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int productId = Convert.ToInt32(reader["GDA_PRODUCT_IDGDA_PRODUCT"].ToString());
                                int stockId = Convert.ToInt32(reader["GDA_STOCK_IDGDA_STOCK"].ToString());
                                int amount = Convert.ToInt32(reader["AMOUNT"].ToString());
                                int orderById = Convert.ToInt32(reader["ORDER_BY"].ToString());
                                int orderId = Convert.ToInt32(reader["IDGDA_ORDER"].ToString());

                                List<bankMktplace.ModelVoucher> vchers = new List<bankMktplace.ModelVoucher>();
                                vchers = bankMktplace.consultVoucher(productId, stockId, amount);

                                foreach (bankMktplace.ModelVoucher vcher in vchers)
                                {
                                    //Update Voucher
                                    bankMktplace.updateVoucher(vcher.voucherId, "REDEEMED");

                                    //Insert collaborator
                                    bankMktplace.insertVoucherCollaborator(vcher.voucherId, orderById, orderId);
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



        public static void criarPersonaNovos()
        {
            using (SqlConnection connection = new SqlConnection(Database.Conn))
            {
                connection.Open();
                StringBuilder stb = new StringBuilder();
                stb.Append("SELECT C.IDGDA_COLLABORATORS, C.NAME, C.COLLABORATORIDENTIFICATION, C.PHONENUMBER, C.BIRTHDATE, C.EMAIL, C.STATE, C.CITY FROM GDA_COLLABORATORS (NOLOCK) AS C ");
                stb.Append("LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS CU ON CU.IDGDA_COLLABORATORS = C.IDGDA_COLLABORATORS ");
                stb.Append("WHERE CU.IDGDA_PERSONA_COLLABORATOR_USER IS NULL ");

                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string idgda_collaborators = reader["IDGDA_COLLABORATORS"] != DBNull.Value ? reader["IDGDA_COLLABORATORS"].ToString() : "";
                            string name = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "";
                            string identification = reader["COLLABORATORIDENTIFICATION"] != DBNull.Value ? reader["COLLABORATORIDENTIFICATION"].ToString() : "";
                            string phoneNumber = reader["PHONENUMBER"] != DBNull.Value ? reader["PHONENUMBER"].ToString() : "";
                            DateTime? birthDate = reader["BIRTHDATE"] != DBNull.Value ? Convert.ToDateTime(reader["BIRTHDATE"].ToString()) : (DateTime?)null;
                            string email = reader["EMAIL"] != DBNull.Value ? reader["EMAIL"].ToString() : "";
                            string state = reader["STATE"] != DBNull.Value ? reader["STATE"].ToString() : "";
                            string city = reader["CITY"] != DBNull.Value ? reader["CITY"].ToString() : "";

                            inserirPersona(idgda_collaborators, name, identification, phoneNumber, birthDate, email, state, city);
                        }
                    }
                }
                connection.Close();
            }
        }

        public static void inserirPersona(string idgda_collaborators, string name, string identification, string phoneNumber, DateTime? birthDate, string email, string state, string city)
        {
            SqlConnection connection = new SqlConnection(Database.Conn);
            try
            {

                connection.Open();

                //Verificar e inserir estado
                StringBuilder stbStateS = new StringBuilder();
                stbStateS.Append("SELECT IDGDA_STATE FROM GDA_STATE (NOLOCK) ");
                stbStateS.AppendFormat("WHERE STATE = '{0}' ", state);

                string codState = "";
                using (SqlCommand commandSelect = new SqlCommand(stbStateS.ToString(), connection))
                {
                    using (SqlDataReader reader = commandSelect.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            codState = reader["IDGDA_STATE"].ToString();
                        }
                    }
                }

                if (codState == "")
                {
                    StringBuilder stbStateInsert = new StringBuilder();
                    stbStateInsert.AppendFormat("INSERT INTO GDA_STATE (STATE, CREATED_AT)  ");
                    stbStateInsert.AppendFormat("VALUES ( ");
                    stbStateInsert.AppendFormat("'{0}', ", state);
                    stbStateInsert.AppendFormat("GETDATE() ");
                    stbStateInsert.AppendFormat(") SELECT @@IDENTITY AS 'CODSTATE' ");

                    using (SqlCommand commandInsert = new SqlCommand(stbStateInsert.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                codState = reader["CODSTATE"].ToString();
                            }
                        }
                    }
                }

                string codCity = "";
                StringBuilder stbSelectCity = new StringBuilder();
                stbSelectCity.Append("SELECT * FROM GDA_CITY (NOLOCK) ");
                stbSelectCity.Append("WHERE ");
                stbSelectCity.AppendFormat("IDGDA_STATE = {0} ", codState);
                stbSelectCity.AppendFormat("AND CITY = '{0}' ", city);

                using (SqlCommand commandInsert = new SqlCommand(stbSelectCity.ToString(), connection))
                {
                    using (SqlDataReader reader = commandInsert.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            codCity = reader["IDGDA_CITY"].ToString();
                        }
                    }
                }

                if (codCity == "")
                {
                    StringBuilder stbInsertCity = new StringBuilder();
                    stbInsertCity.Append("INSERT INTO GDA_CITY (IDGDA_STATE, CITY, CREATED_AT)  ");
                    stbInsertCity.Append("VALUES ( ");
                    stbInsertCity.AppendFormat("'{0}', ", codState);
                    stbInsertCity.AppendFormat("'{0}', ", city);
                    stbInsertCity.Append("GETDATE() ");
                    stbInsertCity.Append(") SELECT @@IDENTITY AS 'CODCITY'");

                    using (SqlCommand commandInsert = new SqlCommand(stbInsertCity.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                codCity = reader["CODCITY"].ToString();
                            }
                        }
                    }
                }

                //Inserir na rede social
                StringBuilder stbPersona = new StringBuilder();
                stbPersona.Append("INSERT INTO GDA_PERSONA_USER (IDGDA_PERSONA_USER_TYPE, IDGDA_PERSONA_USER_VISIBILITY, NAME, BC, SOCIAL_NAME, SHOW_AGE, PICTURE, CREATED_AT, DELETED_AT, CREATED_BY, DELETED_BY) ");
                stbPersona.Append("VALUES (1, "); //IDGDA_PERSONA_USER_TYPE = 1 = Pessoal
                stbPersona.Append("1, "); //IDGDA_PERSONA_USER_VISIBILITY = 1 = Publico
                stbPersona.AppendFormat("'{0}', ", name); //Name
                stbPersona.AppendFormat("'{0}', ", identification); //BC
                stbPersona.Append("'',  "); //SOCIAL_NAME
                stbPersona.Append("1, "); //SHOW_AGE
                stbPersona.Append("'', "); //PICTURE
                stbPersona.Append("GETDATE(), "); //CREATED_AT
                stbPersona.Append("NULL, "); //DELETED_AT
                stbPersona.Append("NULL, "); //CREATED_BY
                stbPersona.Append("NULL "); //DELETED_BY
                stbPersona.Append(") SELECT @@IDENTITY AS 'CODPERSONA' ");
                string codPersona = "";


                using (SqlCommand commandInsert = new SqlCommand(stbPersona.ToString(), connection))
                {
                    using (SqlDataReader reader = commandInsert.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            codPersona = reader["CODPERSONA"].ToString();
                        }
                    }
                }

                StringBuilder stbPersonaDetails = new StringBuilder();
                stbPersonaDetails.Append("INSERT INTO GDA_PERSONA_USER_DETAILS (IDGDA_PERSONA_USER, YOUR_MOTIVATIONS, GOALS, PHONE_NUMBER, BIRTH_DATE, IDGDA_STATE, IDGDA_CITY, WHO_IS, EMAIL) ");
                stbPersonaDetails.Append("VALUES ( ");
                stbPersonaDetails.AppendFormat("'{0}', ", codPersona); //IDGDA_PERSONA_USER
                stbPersonaDetails.Append("'',  "); //YOUR_MOTIVATIONS
                stbPersonaDetails.Append("'',  "); //GOALS
                stbPersonaDetails.AppendFormat("'{0}',  ", phoneNumber); // PHONE_NUMBER
                stbPersonaDetails.AppendFormat("'{0}',  ", birthDate?.ToString("yyyy-MM-dd")); //BIRTH_DATE
                stbPersonaDetails.AppendFormat("'{0}',  ", codState); //IDGDA_STATE
                stbPersonaDetails.AppendFormat("'{0}',  ", codCity); //IDGDA_CITY
                stbPersonaDetails.Append("'#Setor - #Cargo - #Moedas - #Grupo',  "); //WHO_IS
                stbPersonaDetails.AppendFormat("'{0}' ", email); //EMAIL
                stbPersonaDetails.Append(") ");
                SqlCommand createTableCommand = new SqlCommand(stbPersonaDetails.ToString(), connection);
                createTableCommand.ExecuteNonQuery();

                StringBuilder stbPersonaCollaborator = new StringBuilder();
                stbPersonaCollaborator.Append("INSERT INTO GDA_PERSONA_COLLABORATOR_USER (IDGDA_COLLABORATORS, IDGDA_PERSONA_USER) ");
                stbPersonaCollaborator.Append("VALUES ( ");
                stbPersonaCollaborator.AppendFormat("'{0}', ", Convert.ToInt32(idgda_collaborators)); //IDGDA_COLLABORATORS
                stbPersonaCollaborator.AppendFormat("'{0}' ", codPersona); //IDGDA_PERSONA_USER
                stbPersonaCollaborator.Append(") ");
                SqlCommand createTableCommand2 = new SqlCommand(stbPersonaCollaborator.ToString(), connection);
                createTableCommand2.ExecuteNonQuery();


            }
            catch (Exception)
            {

            }
            connection.Close();
        }


        public class vouchersErro
        {
            public int idVoucher { get; set; }
            public int idStockOrigim { get; set; }
            public int idStockNew { get; set; }
            public int idProduct { get; set; }
        }
        public static void CorrigirVoucherStock()
        {

            List<vouchersErro> ves = new List<vouchersErro>();
            StringBuilder stb = new StringBuilder();
            stb.Append("SELECT IDGDA_VOUCHERS, STOCKID, OP.GDA_STOCK_IDGDA_STOCK, V.PRODUCTID FROM GDA_VOUCHERS AS V ");
            stb.Append("INNER JOIN GDA_COLLABORATOR_VOUCHER AS CV ON CV.VOUCHERID = V.IDGDA_VOUCHERS ");
            stb.Append("INNER JOIN GDA_ORDER AS O ON O.IDGDA_ORDER = CV.GDA_IDGDA_ORDER ");
            stb.Append("INNER JOIN GDA_ORDER_PRODUCT AS OP ON OP.GDA_ORDER_IDGDA_ORDER = CV.GDA_IDGDA_ORDER ");
            stb.Append($"WHERE V.PRODUCTID = OP.GDA_PRODUCT_IDGDA_PRODUCT AND STOCKID <> OP.GDA_STOCK_IDGDA_STOCK ");
            //stb.Append($"WHERE V.PRODUCTID = {productId} ");
            //stb.Append($"AND STOCKID = {stockId} ");
            //stb.Append($"AND OP.GDA_PRODUCT_IDGDA_PRODUCT = {productId} ");
            //stb.Append($"AND OP.GDA_STOCK_IDGDA_STOCK <> {stockId} ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                vouchersErro ve = new vouchersErro();

                                ve.idVoucher = Convert.ToInt32(reader["IDGDA_VOUCHERS"].ToString());
                                ve.idStockOrigim = Convert.ToInt32(reader["STOCKID"].ToString());
                                ve.idStockNew = Convert.ToInt32(reader["GDA_STOCK_IDGDA_STOCK"].ToString());
                                ve.idProduct = Convert.ToInt32(reader["PRODUCTID"].ToString());
                                ves.Add(ve);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            foreach (vouchersErro ve in ves)
            {
                List<ModelVoucher> VoucherOutroStock = new List<ModelVoucher>();
                VoucherOutroStock = bankMktplace.consultVoucher(ve.idProduct, ve.idStockNew, 1);

                if (VoucherOutroStock.Count > 0)
                {
                    StringBuilder up1 = new StringBuilder();
                    up1.AppendFormat("UPDATE GDA_VOUCHERS SET ");
                    up1.AppendFormat($"STOCKID = {ve.idStockOrigim}, STATUS = 'RESERVED' ");
                    up1.AppendFormat($"WHERE IDGDA_VOUCHERS = {VoucherOutroStock.First().voucherId} ");

                    using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                    {
                        connection.Open();
                        try
                        {
                            using (SqlCommand command = new SqlCommand(up1.ToString(), connection))
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                        connection.Close();
                    }

                    StringBuilder up2 = new StringBuilder();
                    up2.AppendFormat("UPDATE GDA_VOUCHERS SET ");
                    up2.AppendFormat($"STOCKID = {ve.idStockNew} ");
                    up2.AppendFormat($"WHERE IDGDA_VOUCHERS = {ve.idVoucher} ");
                    using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                    {
                        connection.Open();
                        try
                        {
                            using (SqlCommand command = new SqlCommand(up2.ToString(), connection))
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
        public class OperationalCampaignItem
        {
            public int idgda_operational_campaign { get; set; }
            public int idgda_operational_campaign_user_participant { get; set; }
            public int idPersona { get; set; }
            public int idcollaborator { get; set; }
            public int elimIdIndicator { get; set; }
            public double elimPercent { get; set; }
            public int elimIndicatorIncrease { get; set; }
            public int idIndicator { get; set; }
            public double percent { get; set; }
            public int indicatorIncrease { get; set; }
            public int rewardPoints { get; set; }
        }

        public class OperationalCampaign
        {
            public int idgda_operational_campaign { get; set; }
            public DateTime dtStart { get; set; }
            public DateTime dtEnd { get; set; }
            public List<OperationalCampaignUser> listUser { get; set; }
            public List<OperationalCampaignElimination> listElimination { get; set; }
            public List<OperationalCampaignIndicators> listIndicators { get; set; }

        }

        public class OperationalCampaignUser
        {
            public int idgda_operational_campaign_user_participant { get; set; }
            public int idPersona { get; set; }

            public int idcollaborator { get; set; }
        }

        public class OperationalCampaignElimination
        {
            public int idIndicator { get; set; }
            public double percent { get; set; }
            public int indicatorIncrease { get; set; }
        }

        public class OperationalCampaignIndicators
        {
            public int idIndicator { get; set; }
            public double percent { get; set; }
            public int indicatorIncrease { get; set; }
            public int rewardPoints { get; set; }

        }

        public static void automaticProcessOperationCampaign()
        {

            List<OperationalCampaignItem> ocs = new List<OperationalCampaignItem>();

            //Pegar campanhas dentro do periodo
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();

                try
                {
                    //Criação da campanha
                    StringBuilder sb = new StringBuilder();
                    sb.Append($"SELECT OCU.IDGDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT, OC.IDGDA_OPERATIONAL_CAMPAIGN, OCU.IDGDA_PERSONA, PCU.IDGDA_COLLABORATORS,  ");
                    sb.Append($"OCE.IDGDA_INDICATOR AS ELIM_IDGDA_INDICATOR, OCE.[PERCENT] AS ELIM_PERCENT, OCE.INDICATOR_INCREASE AS ELIM_INDICATOR_INCREASE, ");
                    sb.Append($"OCP.IDGDA_INDICATOR, OCP.[PERCENT], OCP.INDICATOR_INCREASE, OCP.REWARD_POINTS ");
                    sb.Append($"FROM GDA_OPERATIONAL_CAMPAIGN (NOLOCK) OC  ");
                    sb.Append($"INNER JOIN GDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT (NOLOCK) OCU ON OC.IDGDA_OPERATIONAL_CAMPAIGN = OCU.IDGDA_OPERATIONAL_CAMPAIGN  ");
                    sb.Append($"INNER JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) PCU ON PCU.IDGDA_PERSONA_USER  = OCU.IDGDA_PERSONA ");
                    sb.Append($"INNER JOIN GDA_OPERATIONAL_CAMPAIGN_ELIMINATION (NOLOCK) OCE ON OCE.IDGDA_OPERATIONAL_CAMPAIGN = OC.IDGDA_OPERATIONAL_CAMPAIGN ");
                    sb.Append($"INNER JOIN GDA_OPERATIONAL_CAMPAIGN_PONTUATION (NOLOCK) OCP ON OCP.IDGDA_OPERATIONAL_CAMPAIGN = OC.IDGDA_OPERATIONAL_CAMPAIGN ");
                    sb.Append($"WHERE STARTED_AT <= CONVERT(DATE, GETDATE())  ");
                    sb.Append($"AND ENDED_AT >= CONVERT(DATE, GETDATE())  ");

                    using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                OperationalCampaignItem oc = new OperationalCampaignItem();
                                oc.idgda_operational_campaign_user_participant = reader["IDGDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_OPERATIONAL_CAMPAIGN_USER_PARTICIPANT"]) : 0;
                                oc.idgda_operational_campaign = reader["IDGDA_OPERATIONAL_CAMPAIGN"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_OPERATIONAL_CAMPAIGN"]) : 0;
                                oc.idPersona = reader["IDGDA_PERSONA"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_PERSONA"]) : 0;
                                oc.idcollaborator = reader["IDGDA_COLLABORATORS"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_COLLABORATORS"]) : 0;
                                oc.elimIdIndicator = reader["ELIM_IDGDA_INDICATOR"] != DBNull.Value ? Convert.ToInt32(reader["ELIM_IDGDA_INDICATOR"]) : 0;
                                oc.elimPercent = reader["ELIM_PERCENT"] != DBNull.Value ? Convert.ToDouble(reader["ELIM_PERCENT"]) : 0;
                                oc.elimIndicatorIncrease = reader["ELIM_INDICATOR_INCREASE"] != DBNull.Value ? Convert.ToInt32(reader["ELIM_INDICATOR_INCREASE"]) : 0;
                                oc.idIndicator = reader["IDGDA_INDICATOR"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_INDICATOR"]) : 0;
                                oc.percent = reader["PERCENT"] != DBNull.Value ? Convert.ToDouble(reader["PERCENT"]) : 0;
                                oc.indicatorIncrease = reader["INDICATOR_INCREASE"] != DBNull.Value ? Convert.ToInt32(reader["INDICATOR_INCREASE"]) : 0;
                                oc.rewardPoints = reader["REWARD_POINTS"] != DBNull.Value ? Convert.ToInt32(reader["REWARD_POINTS"]) : 0;
                                ocs.Add(oc);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }

                connection.Close();

                List<OperationalCampaign> operationalCampaigns = ocs.GroupBy(item => item.idgda_operational_campaign).Select(group =>
                {
                    var operationalCampaign = new OperationalCampaign
                    {
                        idgda_operational_campaign = group.Key,
                        listUser = group.GroupBy(item => item.idgda_operational_campaign_user_participant)
                        .Select(grp => grp.First())
                        .Select(item => new OperationalCampaignUser
                        {
                            idgda_operational_campaign_user_participant = item.idgda_operational_campaign_user_participant,
                            idPersona = item.idPersona,
                            idcollaborator = item.idcollaborator
                        }).ToList(),
                        listElimination = group.GroupBy(item => item.elimIdIndicator)
                        .Select(grp => grp.First())
                        .Select(item => new OperationalCampaignElimination
                        {
                            idIndicator = item.elimIdIndicator,
                            percent = item.elimPercent,
                            indicatorIncrease = item.elimIndicatorIncrease
                        }).ToList(),
                        listIndicators = group.GroupBy(item => item.idIndicator)
                        .Select(grp => grp.First())
                        .Select(item => new OperationalCampaignIndicators
                        {
                            idIndicator = item.idIndicator,
                            percent = item.percent,
                            indicatorIncrease = item.indicatorIncrease,
                            rewardPoints = item.rewardPoints
                        }).ToList()
                    };
                    return operationalCampaign;
                }).ToList();

                foreach (OperationalCampaign ocEnv in operationalCampaigns)
                {
                    string listIndicatorsElim = string.Join(",", ocEnv.listElimination.Select(elim => elim.idIndicator));
                    string listIndicatorsPont = string.Join(",", ocEnv.listIndicators.Select(elim => elim.idIndicator));
                    string listIndicators = string.Join(",", listIndicatorsElim, listIndicatorsPont);
                    string listUser = $" AND CL.IDGDA_COLLABORATORS IN ({string.Join(",", ocEnv.listUser.Select(elim => elim.idcollaborator))}) ";

                    List<ResultConsolidatedController.HomeResultConsolidated> resultadoPrimeiroDiaCampanha = new List<ResultConsolidatedController.HomeResultConsolidated>();
                    resultadoPrimeiroDiaCampanha = ResultConsolidatedController.ReturnHomeResultConsolidated("", ocEnv.dtStart.ToString("yyyy-MM-dd"), ocEnv.dtStart.ToString("yyyy-MM-dd"), "", listIndicators, "", "", "", "", "", true, listUser);

                    List<ResultConsolidatedController.HomeResultConsolidated> resultadoDuranteCampanha = new List<ResultConsolidatedController.HomeResultConsolidated>();
                    resultadoDuranteCampanha = ResultConsolidatedController.ReturnHomeResultConsolidated("", ocEnv.dtStart.ToString("yyyy-MM-dd"), ocEnv.dtEnd.ToString("yyyy-MM-dd"), "", listIndicators, "", "", "", "", "", true, listUser);

                    foreach (OperationalCampaignUser user in ocEnv.listUser)
                    {
                        //Realizar inserções de pontuações e missões alcançadas
                        foreach (OperationalCampaignIndicators pont in ocEnv.listIndicators)
                        {
                            ResultConsolidatedController.HomeResultConsolidated primResult = new ResultConsolidatedController.HomeResultConsolidated();
                            ResultConsolidatedController.HomeResultConsolidated segundResult = new ResultConsolidatedController.HomeResultConsolidated();
                            primResult = resultadoPrimeiroDiaCampanha.Find(kkk => kkk.MATRICULA == user.idcollaborator.ToString() && kkk.IDINDICADOR == pont.idIndicator.ToString());
                            segundResult = resultadoDuranteCampanha.Find(kkk => kkk.MATRICULA == user.idcollaborator.ToString() && kkk.IDINDICADOR == pont.idIndicator.ToString());

                            ResultConsolidatedController.DoCalculateFinal(primResult);
                            ResultConsolidatedController.DoCalculateFinal(segundResult);

                            double percentAltered = 0;
                            percentAltered = segundResult.PERCENTUAL - primResult.PERCENTUAL;

                            if (pont.indicatorIncrease == 1 && percentAltered > 0)
                            {
                                if (percentAltered >= pont.percent)
                                {
                                    //Verifica se o usuario ja pontuou
                                    bool ValidPoints = Funcoes.ValidaPoints(ocEnv.idgda_operational_campaign, pont.idIndicator, user.idPersona);

                                    //Realizar Pontuacao
                                    if (ValidPoints == true)
                                    {
                                        Funcoes.InserirPoints(ocEnv.idgda_operational_campaign, pont.idIndicator, user.idPersona);
                                    }
                                }

                            }

                            else if (pont.indicatorIncrease == 0 && percentAltered < 0)
                            {
                                percentAltered = percentAltered * (-1);
                                if (percentAltered >= pont.percent)
                                {
                                    //Verifica se o usuario ja pontuou
                                    bool ValidPoints = Funcoes.ValidaPoints(ocEnv.idgda_operational_campaign, pont.idIndicator, user.idPersona);

                                    //Realizar Pontuacao
                                    if (ValidPoints == true)
                                    {
                                        Funcoes.InserirPoints(ocEnv.idgda_operational_campaign, pont.idIndicator, user.idPersona);
                                    }
                                }
                            }

                        }

                        //Verificar criterio de eliminação
                        foreach (OperationalCampaignElimination elim in ocEnv.listElimination)
                        {
                            ResultConsolidatedController.HomeResultConsolidated primResult = new ResultConsolidatedController.HomeResultConsolidated();
                            ResultConsolidatedController.HomeResultConsolidated segundResult = new ResultConsolidatedController.HomeResultConsolidated();
                            primResult = resultadoPrimeiroDiaCampanha.Find(kkk => kkk.MATRICULA == user.idcollaborator.ToString() && kkk.IDINDICADOR == elim.idIndicator.ToString());
                            segundResult = resultadoDuranteCampanha.Find(kkk => kkk.MATRICULA == user.idcollaborator.ToString() && kkk.IDINDICADOR == elim.idIndicator.ToString());

                            ResultConsolidatedController.DoCalculateFinal(primResult);
                            ResultConsolidatedController.DoCalculateFinal(segundResult);

                            double percentAltered = 0;
                            percentAltered = segundResult.PERCENTUAL - primResult.PERCENTUAL;

                            //Se aumentou
                            if (elim.indicatorIncrease == 1 && percentAltered > 0)
                            {
                                if (percentAltered >= elim.percent)
                                {
                                    //Realizar eliminação
                                    Funcoes.InserirEliminacao(ocEnv.idgda_operational_campaign, elim.idIndicator, user.idPersona);
                                }
                            }
                            else if (elim.indicatorIncrease == 0 && percentAltered < 0)
                            {
                                percentAltered = percentAltered * (-1);
                                if (percentAltered >= elim.percent)
                                {
                                    //Realizar eliminação
                                    Funcoes.InserirEliminacao(ocEnv.idgda_operational_campaign, elim.idIndicator, user.idPersona);
                                }
                            }
                        }
                    }

                    //Realizar Notificação
                    List<ResultConsolidatedController.HomeResultConsolidated> retornoPrimeiroDia = new List<ResultConsolidatedController.HomeResultConsolidated>();
                    retornoPrimeiroDia = resultadoPrimeiroDiaCampanha.GroupBy(d => new { d.IDINDICADOR }).Select(item => new ResultConsolidatedController.HomeResultConsolidated
                    {
                        MATRICULA = item.First().MATRICULA,
                        CARGO = item.First().CARGO,
                        //CODGIP = item.First().CODGIP,
                        //SETOR = item.First().SETOR,
                        IDINDICADOR = item.Key.IDINDICADOR,
                        INDICADOR = item.First().INDICADOR,
                        QTD = item.Sum(d => d.QTD),
                        META = Math.Round(item.Sum(d => d.META) / item.Count(), 2, MidpointRounding.AwayFromZero),
                        FACTOR0 = item.Sum(d => d.FACTOR0),
                        FACTOR1 = item.Sum(d => d.FACTOR1),
                        //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                        min1 = Math.Round(item.Sum(d => d.min1) / item.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                        min2 = Math.Round(item.Sum(d => d.min2) / item.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                        min3 = Math.Round(item.Sum(d => d.min3) / item.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                        min4 = Math.Round(item.Sum(d => d.min4) / item.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                        CONTA = item.First().CONTA,
                        BETTER = item.First().BETTER,
                        //META_MAXIMA_MOEDAS = Math.Round(item.Sum(d => d.META_MAXIMA_MOEDAS) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                        META_MAXIMA_MOEDAS = item.Sum(d => d.QTD_MON != 0 ? Math.Round(d.QTD_MON_TOTAL / d.QTD_MON, 0, MidpointRounding.AwayFromZero) : 0),

                        SUMDIASLOGADOS = item.Sum(d => d.SUMDIASLOGADOS),
                        SUMDIASESCALADOS = item.Sum(d => d.SUMDIASESCALADOS),
                        SUMDIASLOGADOSESCALADOS = item.Sum(d => d.SUMDIASLOGADOSESCALADOS),

                        //MOEDA_GANHA =  Math.Round(item.Sum(d => d.MOEDA_GANHA) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                        MOEDA_GANHA = Math.Round(item.Sum(d => d.MOEDA_GANHA), 0, MidpointRounding.AwayFromZero),
                        TYPE = item.First().TYPE,
                    }).ToList();

                    List<ResultConsolidatedController.HomeResultConsolidated> retornoDiasAtuais = new List<ResultConsolidatedController.HomeResultConsolidated>();
                    retornoDiasAtuais = resultadoDuranteCampanha.GroupBy(d => new { d.IDINDICADOR }).Select(item => new ResultConsolidatedController.HomeResultConsolidated
                    {
                        MATRICULA = item.First().MATRICULA,
                        CARGO = item.First().CARGO,
                        //CODGIP = item.First().CODGIP,
                        //SETOR = item.First().SETOR,
                        IDINDICADOR = item.Key.IDINDICADOR,
                        INDICADOR = item.First().INDICADOR,
                        QTD = item.Sum(d => d.QTD),
                        META = Math.Round(item.Sum(d => d.META) / item.Count(), 2, MidpointRounding.AwayFromZero),
                        FACTOR0 = item.Sum(d => d.FACTOR0),
                        FACTOR1 = item.Sum(d => d.FACTOR1),
                        //DESCOMENTAR PARA PONDERAMENTO DAS METRICAS
                        min1 = Math.Round(item.Sum(d => d.min1) / item.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                        min2 = Math.Round(item.Sum(d => d.min2) / item.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                        min3 = Math.Round(item.Sum(d => d.min3) / item.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                        min4 = Math.Round(item.Sum(d => d.min4) / item.Sum(d => d.QTD), 2, MidpointRounding.AwayFromZero),
                        CONTA = item.First().CONTA,
                        BETTER = item.First().BETTER,
                        //META_MAXIMA_MOEDAS = Math.Round(item.Sum(d => d.META_MAXIMA_MOEDAS) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                        META_MAXIMA_MOEDAS = item.Sum(d => d.QTD_MON != 0 ? Math.Round(d.QTD_MON_TOTAL / d.QTD_MON, 0, MidpointRounding.AwayFromZero) : 0),

                        SUMDIASLOGADOS = item.Sum(d => d.SUMDIASLOGADOS),
                        SUMDIASESCALADOS = item.Sum(d => d.SUMDIASESCALADOS),
                        SUMDIASLOGADOSESCALADOS = item.Sum(d => d.SUMDIASLOGADOSESCALADOS),

                        //MOEDA_GANHA =  Math.Round(item.Sum(d => d.MOEDA_GANHA) / item.Sum(d => d.QTD), 0, MidpointRounding.AwayFromZero),
                        MOEDA_GANHA = Math.Round(item.Sum(d => d.MOEDA_GANHA), 0, MidpointRounding.AwayFromZero),
                        TYPE = item.First().TYPE,
                    }).ToList();

                    foreach (OperationalCampaignIndicators pont in ocEnv.listIndicators)
                    {
                        ResultConsolidatedController.HomeResultConsolidated primResult = new ResultConsolidatedController.HomeResultConsolidated();
                        ResultConsolidatedController.HomeResultConsolidated segundResult = new ResultConsolidatedController.HomeResultConsolidated();
                        primResult = retornoPrimeiroDia.Find(kkk => kkk.IDINDICADOR == pont.idIndicator.ToString());
                        segundResult = resultadoDuranteCampanha.Find(kkk => kkk.IDINDICADOR == pont.idIndicator.ToString());

                        ResultConsolidatedController.DoCalculateFinal(primResult);
                        ResultConsolidatedController.DoCalculateFinal(segundResult);

                        double percentAltered = 0;
                        percentAltered = segundResult.PERCENTUAL - primResult.PERCENTUAL;

                        if (pont.indicatorIncrease == 1 && percentAltered > 0)
                        {
                            if (percentAltered >= pont.percent)
                            {
                                //Enviar Notificação com o aumento
                                Funcoes.EnvioNotCampanha(ocEnv.idgda_operational_campaign, pont.idIndicator, percentAltered, true);
                            }


                        }
                        else if (pont.indicatorIncrease == 0 && percentAltered < 0)
                        {
                            percentAltered = percentAltered * (-1);
                            if (percentAltered >= pont.percent)
                            {
                                //Enviar Notificação com a baixa
                                Funcoes.EnvioNotCampanha(ocEnv.idgda_operational_campaign, pont.idIndicator, percentAltered, false);
                            }
                        }

                    }
                    //Realizar rankiamento da campanha
                    Funcoes.RankearCampanha(ocEnv.idgda_operational_campaign);
                }

                //Pegar campanhas finalizadas com pagamento automaticos ainda não efetuados
                Funcoes.PagamentoAutomaticoCampanha();


            }

        }



        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] inputPost infs)
        {
            try
            {
                //ScheduledTask.reportsPreview1(null);
                //ScheduledTask.reportsPreview2(null);
                //ScheduledTask.reportsPreview3(null);
                //ScheduledTask.reportsPreview4(null);

                return Ok("");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: {ex.Message}");
            }



            //try
            //{

            //    int collaboratorId = 0;
            //    int personauserId = 0;
            //    var token = Request.Headers.Authorization?.Parameter;
            //    bool tokn = TokenService.TryDecodeToken(token);
            //    if (tokn == false)
            //    {
            //        return Unauthorized();
            //    }
            //    collaboratorId = TokenService.InfsUsers.collaboratorId;
            //    personauserId = TokenService.InfsUsers.personauserId;

            //    DateTime today = DateTime.Now;

            //    //primeiro dia do mes atual
            //    DateTime firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

            //    //primeiro dia do mes passado
            //    DateTime firstDayOfLastMonth = firstDayOfMonth.AddMonths(-1);

            //    //último dia do mês passado
            //    DateTime lastDayOfLastMonth = firstDayOfMonth.AddDays(-1);

            //    int tamanhoMaximoPorLista = 1000000; // 1048000

            //    string dtFinal = today.ToString("yyyy-MM-dd");
            //    string dtInicial = firstDayOfMonth.ToString("yyyy-MM-dd");

            //    string dtiFinalPassado = lastDayOfLastMonth.ToString("yyyy-MM-dd");
            //    string dtInicialPassado = firstDayOfLastMonth.ToString("yyyy-MM-dd");
            //    string month = DateTime.Now.ToString("MM");
            //    string year = DateTime.Now.Year.ToString();
            //    string yearPast = firstDayOfLastMonth.ToString("yyyy");
            //    string monthPast = firstDayOfLastMonth.ToString("MM");
            //    string NameFile = "";
            //    Dictionary<string, string> columnNames = new Dictionary<string, string>();

            //    List<ScheduledTask.objectsClass> dataQuebTeste = new List<ScheduledTask.objectsClass>();
            //    if (infs.report == "reportsPreview1")
            //    {
            //        ScheduledTask.reportsPreview1(null);
            //    }
            //    else if (infs.report == "reportsPreview2")
            //    {
            //        ScheduledTask.reportsPreview2(null);
            //    }
            //    else if (infs.report == "reportsPreview3")
            //    {
            //        ScheduledTask.reportsPreview3(null);
            //    }
            //    else if (infs.report == "reportsPreview4")
            //    {
            //        ScheduledTask.reportsPreview4(null);
            //    }
            //    else if (infs.report == "Teste")
            //    {
            //        //Teste
            //        NameFile = $"Teste {monthPast}-{yearPast}";
            //        BucketClass.DeleteFiles(NameFile);
            //        List<ModelsEx.homeRel> hr = new List<ModelsEx.homeRel>();
            //        for (int i = 0; i < 2000000; i++)
            //        {
            //            ModelsEx.homeRel objeto = new ModelsEx.homeRel
            //            {
            //                // Atribuindo valores fictícios aos campos do objeto
            //                cargo = "aa",
            //                cod_gip = "1" + i,
            //                setor = "ss"
            //            };

            //            hr.Add(objeto);
            //        }

            //        List<object> dataDTeste = hr.Cast<object>().ToList();

            //        var listasDivididasDTeste = ScheduledTask.DividirLista(dataDTeste, tamanhoMaximoPorLista);
            //        dataQuebTeste = listasDivididasDTeste.Select(lista => new ScheduledTask.objectsClass { listObjects = lista }).ToList();

            //        columnNames = ScheduledTask.trataCabecalhosRelatorio("Resultado");




            //    }
            //    else if (infs.report == "Saldo_Acumulado")
            //    {
            //        #region Saldo Acumulado

            //        if (infs.monthCurrent == true)
            //        {
            //            NameFile = $"Relatorio_Saldo_Acumulado {month}-{year}";
            //            BucketClass.DeleteFiles(NameFile);
            //            List<object> dataA = DatabaseService.GetQueryResult("Saldo Acumulado", dtInicial, dtFinal);

            //            var listasDivididas = ScheduledTask.DividirLista(dataA, tamanhoMaximoPorLista);
            //            dataQuebTeste = listasDivididas.Select(lista => new ScheduledTask.objectsClass { listObjects = lista }).ToList();

            //            columnNames = ScheduledTask.trataCabecalhosRelatorio("Saldo Acumulado");

            //        }
            //        else
            //        {
            //            NameFile = $"Relatorio_Saldo_Acumulado {monthPast}-{yearPast}";
            //            BucketClass.DeleteFiles(NameFile);
            //            List<object> dataA1 = DatabaseService.GetQueryResult("Saldo Acumulado", dtInicialPassado, dtiFinalPassado);

            //            var listasDivididas = ScheduledTask.DividirLista(dataA1, tamanhoMaximoPorLista);
            //            dataQuebTeste = listasDivididas.Select(lista => new ScheduledTask.objectsClass { listObjects = lista }).ToList();

            //            columnNames = ScheduledTask.trataCabecalhosRelatorio("Saldo Acumulado");

            //        }

            //        #endregion
            //    }
            //    else if (infs.report == "Monetizacao_Diario")
            //    {
            //        #region Monetização Diario

            //        if (infs.monthCurrent == true)
            //        {
            //            NameFile = $"Relatorio_Monetizacao_Diario {month}-{year}";
            //            BucketClass.DeleteFiles(NameFile);
            //            List<object> dataB = DatabaseService.GetQueryResult("Monetização Diario", dtInicial, dtFinal);
            //            List<object> dataBt = ScheduledTask.trataDadosRelatorio(dataB, "Monetização Diario");

            //            var listasDivididas = ScheduledTask.DividirLista(dataBt, tamanhoMaximoPorLista);
            //            dataQuebTeste = listasDivididas.Select(lista => new ScheduledTask.objectsClass { listObjects = lista }).ToList();

            //            columnNames = ScheduledTask.trataCabecalhosRelatorio("Monetização Diario");

            //        }
            //        else
            //        {
            //            NameFile = $"Relatorio_Monetizacao_Diario {monthPast}-{yearPast}";
            //            BucketClass.DeleteFiles(NameFile);
            //            List<object> dataB1 = DatabaseService.GetQueryResult("Monetização Diario", dtInicialPassado, dtiFinalPassado);
            //            List<object> dataBt1 = ScheduledTask.trataDadosRelatorio(dataB1, "Monetização Diario");

            //            var listasDivididas = ScheduledTask.DividirLista(dataBt1, tamanhoMaximoPorLista);
            //            dataQuebTeste = listasDivididas.Select(lista => new ScheduledTask.objectsClass { listObjects = lista }).ToList();

            //            columnNames = ScheduledTask.trataCabecalhosRelatorio("Monetização Diario");

            //        }

            //        #endregion
            //    }
            //    else if (infs.report == "Monetizacao_Consolidado")
            //    {
            //        #region Monetização Mensal

            //        if (infs.monthCurrent == true)
            //        {
            //            NameFile = $"Relatorio_Monetizacao_Consolidado {month}-{year}";
            //            BucketClass.DeleteFiles(NameFile);
            //            List<object> dataC = DatabaseService.GetQueryResult("Monetizacao_Consolidado", dtInicial, dtFinal);

            //            var listasDivididas = ScheduledTask.DividirLista(dataC, tamanhoMaximoPorLista);
            //            dataQuebTeste = listasDivididas.Select(lista => new ScheduledTask.objectsClass { listObjects = lista }).ToList();

            //            columnNames = ScheduledTask.trataCabecalhosRelatorio("Monetizacao_Consolidado");

            //        }
            //        else
            //        {
            //            NameFile = $"Relatorio_Monetizacao_Consolidado {monthPast}-{yearPast}";
            //            BucketClass.DeleteFiles(NameFile);
            //            List<object> dataC1 = DatabaseService.GetQueryResult("Monetizacao_Consolidado", dtInicialPassado, dtiFinalPassado);

            //            var listasDivididas = ScheduledTask.DividirLista(dataC1, tamanhoMaximoPorLista);
            //            dataQuebTeste = listasDivididas.Select(lista => new ScheduledTask.objectsClass { listObjects = lista }).ToList();

            //            columnNames = ScheduledTask.trataCabecalhosRelatorio("Monetizacao_Consolidado");

            //        }

            //        #endregion
            //    }
            //    else if (infs.report == "Resultado")
            //    {
            //        #region Monetização Resultado

            //        if (infs.monthCurrent == true)
            //        {
            //            NameFile = $"Relatorio_Resultado {month}-{year}";
            //            BucketClass.DeleteFiles(NameFile);
            //            List<object> dataD = DatabaseService.GetQueryResult("Resultado", dtInicial, dtFinal);

            //            var listasDivididas = ScheduledTask.DividirLista(dataD, tamanhoMaximoPorLista);
            //            dataQuebTeste = listasDivididas.Select(lista => new ScheduledTask.objectsClass { listObjects = lista }).ToList();

            //            columnNames = ScheduledTask.trataCabecalhosRelatorio("Resultado");

            //        }
            //        else
            //        {
            //            NameFile = $"Relatorio_Resultado {monthPast}-{yearPast}";
            //            BucketClass.DeleteFiles(NameFile);
            //            List<object> dataD1 = DatabaseService.GetQueryResult("Resultado", dtInicialPassado, dtiFinalPassado);

            //            var listasDivididas = ScheduledTask.DividirLista(dataD1, tamanhoMaximoPorLista);
            //            dataQuebTeste = listasDivididas.Select(lista => new ScheduledTask.objectsClass { listObjects = lista }).ToList();

            //            columnNames = ScheduledTask.trataCabecalhosRelatorio("Resultado");

            //        }

            //        #endregion
            //    }
            //    else if (infs.report == "Consolidado_setor")
            //    {
            //        #region Monetização Consolidado Setor

            //        if (infs.monthCurrent == true)
            //        {
            //            NameFile = $"Relatorio_Consolidado_setor {month}-{year}";
            //            BucketClass.DeleteFiles(NameFile);
            //            List<object> dataE = DatabaseService.GetQueryResult("Consolidado_setor", dtInicial, dtFinal);

            //            var listasDivididas = ScheduledTask.DividirLista(dataE, tamanhoMaximoPorLista);
            //            dataQuebTeste = listasDivididas.Select(lista => new ScheduledTask.objectsClass { listObjects = lista }).ToList();

            //            columnNames = ScheduledTask.trataCabecalhosRelatorio("Consolidado_setor");

            //        }
            //        else
            //        {
            //            NameFile = $"Relatorio_Consolidado_setor {monthPast}-{yearPast}";
            //            BucketClass.DeleteFiles(NameFile);
            //            List<object> dataE1 = DatabaseService.GetQueryResult("Consolidado_setor", dtInicialPassado, dtiFinalPassado);

            //            var listasDivididas = ScheduledTask.DividirLista(dataE1, tamanhoMaximoPorLista);
            //            dataQuebTeste = listasDivididas.Select(lista => new ScheduledTask.objectsClass { listObjects = lista }).ToList();

            //            columnNames = ScheduledTask.trataCabecalhosRelatorio("Consolidado_setor");

            //        }

            //        #endregion
            //    }

            //    if (dataQuebTeste.Count > 1)
            //    {
            //        int part = 0;
            //        foreach (ScheduledTask.objectsClass item in dataQuebTeste)
            //        {
            //            ExcelGenerator.DownloadAndGenerateExcel(item, NameFile + "Part_" + part + ".xlsx", columnNames);
            //            part += 1;
            //        }
            //    }
            //    else
            //    {
            //        ExcelGenerator.DownloadAndGenerateExcel(dataQuebTeste.First(), NameFile + ".xlsx", columnNames);
            //    }

            //    return Ok("");
            //}
            //catch (Exception ex)
            //{
            //    return Ok(ex.Message.ToString());
            //}

        }


        // Método para serializar um DataTable em JSON
    }
}