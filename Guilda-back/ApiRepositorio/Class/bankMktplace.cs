using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace ApiC.Class
{
    public class bankMktplace
    {
        public class ModelOrderProduct
        {
            public string comercialName { get; set; }
            public string nameCollaborator { get; set; }
            public int orderId { get; set; }
            public int orderById { get; set; }
            public int productId { get; set; }
            public int stockId { get; set; }
            public string status { get; set; }
            public string type { get; set; }
            public int amount { get; set; }
        }

        public class ModelVoucher
        {
            public int voucherId { get; set; }
            public string status { get; set; }
        }

        public class ModelItem
        {
            public int itemId { get; set; }
            public string status { get; set; }
        }
        public static List<ModelOrderProduct> consultOrder(int idOrder)
        {
            List<ModelOrderProduct> retorno = new List<ModelOrderProduct>();

            StringBuilder stb = new StringBuilder();
            stb.Append("SELECT OP.GDA_ORDER_IDGDA_ORDER AS IDGDA_ORDER,P.COMERCIAL_NAME,c.NAME, O.ORDER_BY, OP.GDA_PRODUCT_IDGDA_PRODUCT AS IDGDA_PRODUCT, OP.GDA_STOCK_IDGDA_STOCK AS IDGDA_STOCK, OP.ORDER_PRODUCT_STATUS, OP.AMOUNT, P.TYPE ");
            stb.Append("FROM GDA_ORDER (NOLOCK) AS O ");
            stb.Append("INNER JOIN GDA_ORDER_PRODUCT (NOLOCK) AS OP ON O.IDGDA_ORDER = OP.GDA_ORDER_IDGDA_ORDER ");
            stb.Append("INNER JOIN GDA_PRODUCT (NOLOCK) AS P ON P.IDGDA_PRODUCT = OP.GDA_PRODUCT_IDGDA_PRODUCT ");
            stb.Append("LEFT JOIN GDA_COLLABORATORS (NOLOCK) AS C ON C.IDGDA_COLLABORATORS = O.ORDER_BY ");
            stb.Append($"WHERE IDGDA_ORDER = {idOrder} AND GDA_ORDER_STATUS_IDGDA_ORDER_STATUS = 4 ");


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
                                ModelOrderProduct ret = new ModelOrderProduct();
                                ret.comercialName = reader["COMERCIAL_NAME"].ToString();
                                ret.nameCollaborator = reader["NAME"].ToString();
                                ret.orderId = Convert.ToInt32(reader["IDGDA_ORDER"]);
                                ret.orderById = Convert.ToInt32(reader["ORDER_BY"]);
                                ret.productId = Convert.ToInt32(reader["IDGDA_PRODUCT"]);
                                ret.stockId = Convert.ToInt32(reader["IDGDA_STOCK"]);
                                ret.status = reader["ORDER_PRODUCT_STATUS"].ToString();
                                ret.type = reader["TYPE"].ToString();
                                ret.amount = Convert.ToInt32(reader["AMOUNT"]);

                                retorno.Add(ret);
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

        public static List<ModelOrderProduct> consultOrderProduct(int idOrder, int idProduct)
        {
            List<ModelOrderProduct> retorno = new List<ModelOrderProduct>();

            StringBuilder stb = new StringBuilder();
            stb.Append("SELECT OP.GDA_ORDER_IDGDA_ORDER AS IDGDA_ORDER, O.ORDER_BY, OP.GDA_PRODUCT_IDGDA_PRODUCT AS IDGDA_PRODUCT, OP.GDA_STOCK_IDGDA_STOCK AS IDGDA_STOCK, OP.ORDER_PRODUCT_STATUS, OP.AMOUNT, P.TYPE ");
            stb.Append("FROM GDA_ORDER (NOLOCK) AS O ");
            stb.Append("INNER JOIN GDA_ORDER_PRODUCT (NOLOCK) AS OP ON O.IDGDA_ORDER = OP.GDA_ORDER_IDGDA_ORDER ");
            stb.Append("INNER JOIN GDA_PRODUCT (NOLOCK) AS P ON P.IDGDA_PRODUCT = OP.GDA_PRODUCT_IDGDA_PRODUCT ");
            stb.Append($"WHERE IDGDA_ORDER = {idOrder} AND GDA_PRODUCT_IDGDA_PRODUCT = {idProduct} AND GDA_ORDER_STATUS_IDGDA_ORDER_STATUS = 4 ");


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
                                ModelOrderProduct ret = new ModelOrderProduct();
                                ret.orderId = Convert.ToInt32(reader["IDGDA_ORDER"]);
                                ret.orderById = Convert.ToInt32(reader["ORDER_BY"]);
                                ret.productId = Convert.ToInt32(reader["IDGDA_PRODUCT"]);
                                ret.stockId = Convert.ToInt32(reader["IDGDA_STOCK"]);
                                ret.status = reader["ORDER_PRODUCT_STATUS"].ToString();
                                ret.type = reader["TYPE"].ToString();
                                ret.amount = Convert.ToInt32(reader["AMOUNT"]);

                                retorno.Add(ret);
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

        public static List<ModelItem> consultItensAvaliable(int idProduct, int quantity)
        {
            List<ModelItem> retorno = new List<ModelItem>();

            int qtdInt = quantity;

            StringBuilder stb = new StringBuilder();
            stb.Append("SELECT IDGDA_PRODUCT_ITEM, STATUS FROM GDA_PRODUCT_ITEM (NOLOCK) ");
            stb.Append($"WHERE GDA_PRODUCT_IDGDA_PRODUCT = {idProduct} AND STATUS = 'AVALIABLE' ORDER BY IDGDA_PRODUCT_ITEM ASC ");


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
                                if (qtdInt == 0)
                                {
                                    break;
                                }

                                ModelItem ret = new ModelItem();
                                ret.itemId = Convert.ToInt32(reader["IDGDA_PRODUCT_ITEM"]);
                                ret.status = reader["STATUS"].ToString();

                                retorno.Add(ret);
                                qtdInt--;

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

        public static List<ModelItem> consultItens(int idProduct, int quantity)
        {
            List<ModelItem> retorno = new List<ModelItem>();

            int qtdInt = quantity;
            StringBuilder stb = new StringBuilder();
            stb.Append("SELECT IDGDA_PRODUCT_ITEM, STATUS FROM GDA_PRODUCT_ITEM (NOLOCK) ");
            stb.Append($"WHERE GDA_PRODUCT_IDGDA_PRODUCT = {idProduct} AND STATUS = 'RESERVED' ");

            StringBuilder stb2 = new StringBuilder();
            stb2.Append("SELECT IDGDA_PRODUCT_ITEM, STATUS FROM GDA_PRODUCT_ITEM (NOLOCK) ");
            stb2.Append($"WHERE GDA_PRODUCT_IDGDA_PRODUCT = {idProduct} AND STATUS = 'AVALIABLE' ");


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
                                if (qtdInt == 0)
                                {
                                    break;
                                }

                                ModelItem ret = new ModelItem();
                                ret.itemId = Convert.ToInt32(reader["IDGDA_PRODUCT_ITEM"]);
                                ret.status = reader["STATUS"].ToString();

                                retorno.Add(ret);
                                qtdInt--;

                            }
                        }
                    }
                    if (qtdInt > 0)
                    {
                        using (SqlCommand command = new SqlCommand(stb2.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (qtdInt == 0)
                                    {
                                        break;
                                    }

                                    ModelItem ret = new ModelItem();
                                    ret.itemId = Convert.ToInt32(reader["IDGDA_PRODUCT_ITEM"]);
                                    ret.status = reader["STATUS"].ToString();

                                    retorno.Add(ret);
                                    qtdInt--;

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

        public static List<ModelVoucher> consultVoucherAvailable(int idProduct, int idStock, int quantity)
        {
            List<ModelVoucher> retorno = new List<ModelVoucher>();

            int qtdInt = quantity;

            StringBuilder stb = new StringBuilder();
            stb.Append("SELECT IDGDA_VOUCHERS, STATUS FROM GDA_VOUCHERS (NOLOCK) ");
            stb.Append($"WHERE PRODUCTID = {idProduct} AND STOCKID = {idStock} AND STATUS = 'AVAILABLE' ORDER BY IDGDA_VOUCHERS ASC ");


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

                                if (qtdInt == 0)
                                {
                                    break;
                                }
                                ModelVoucher ret = new ModelVoucher();
                                ret.voucherId = Convert.ToInt32(reader["IDGDA_VOUCHERS"]);
                                ret.status = reader["STATUS"].ToString();

                                retorno.Add(ret);
                                qtdInt--;
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

        public static List<ModelVoucher> consultVoucher(int idProduct, int idStock, int quantity)
        {
            List<ModelVoucher> retorno = new List<ModelVoucher>();

            int qtdInt = quantity;
            StringBuilder stb = new StringBuilder();
            stb.Append("SELECT IDGDA_VOUCHERS, STATUS FROM GDA_VOUCHERS (NOLOCK) ");
            stb.Append($"WHERE PRODUCTID = {idProduct} AND STOCKID = {idStock} AND STATUS = 'RESERVED' ");

            StringBuilder stb2 = new StringBuilder();
            stb2.Append("SELECT IDGDA_VOUCHERS, STATUS FROM GDA_VOUCHERS (NOLOCK) ");
            stb2.Append($"WHERE PRODUCTID = {idProduct} AND STOCKID = {idStock} AND STATUS = 'AVAILABLE' ");


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

                                if (qtdInt == 0)
                                {
                                    break;
                                }
                                ModelVoucher ret = new ModelVoucher();
                                ret.voucherId = Convert.ToInt32(reader["IDGDA_VOUCHERS"]);
                                ret.status = reader["STATUS"].ToString();

                                retorno.Add(ret);
                                qtdInt--;
                            }
                        }
                    }
                    if (qtdInt > 0)
                    {
                        using (SqlCommand command = new SqlCommand(stb2.ToString(), connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (qtdInt == 0)
                                    {
                                        break;
                                    }
                                    ModelVoucher ret = new ModelVoucher();
                                    ret.voucherId = Convert.ToInt32(reader["IDGDA_VOUCHERS"]);
                                    ret.status = reader["STATUS"].ToString();

                                    retorno.Add(ret);
                                    qtdInt--;

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

        public static bool updateVoucher(int idVoucher, string status)
        {
            bool retorno = false;

            StringBuilder stb = new StringBuilder();
            stb.Append("UPDATE GDA_VOUCHERS ");
            stb.Append($"SET STATUS = '{status}' ");
            stb.Append($"WHERE IDGDA_VOUCHERS = {idVoucher} ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {

                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                        retorno = true;
                    }

                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return retorno;
        }

        public static bool insertVoucherCollaborator(int idVoucher, int orderById, int idOrder)
        {
            bool retorno = false;


            StringBuilder stb = new StringBuilder();
            stb.Append("INSERT INTO GDA_COLLABORATOR_VOUCHER (COLLABORATORID, VOUCHERID, CREATED_AT, GDA_IDGDA_ORDER) ");
            stb.Append("VALUES ( ");
            stb.Append($"{orderById}, "); //COLLABORATORID
            stb.Append($"{idVoucher}, "); //VOUCHERID
            stb.Append("GETDATE(), "); //CREATED_AT
            stb.Append($"{idOrder} "); //GDA_IDGDA_ORDER
            stb.Append(") ");


            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {

                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                        retorno = true;
                    }

                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return retorno;
        }

        public static bool updateItem(int idItem, string status)
        {
            bool retorno = false;

            StringBuilder stb = new StringBuilder();
            stb.Append("UPDATE GDA_PRODUCT_ITEM ");
            stb.Append($"SET STATUS = '{status}' ");
            stb.Append($"WHERE IDGDA_PRODUCT_ITEM = {idItem} ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {

                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                        retorno = true;
                    }

                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return retorno;
        }

        public static bool updateOrderProduct(int orderId, int productId, string observationDelivered, int deliveredBy, string deliveredNote)
        {
            bool retorno = false;

            StringBuilder stb = new StringBuilder();
            stb.Append("UPDATE GDA_ORDER_PRODUCT SET ");
            stb.Append("ORDER_PRODUCT_STATUS = 'DELIVERED', ");
            stb.Append($"OBSERVATION_DELIVERED = '{observationDelivered}', ");
            stb.Append("DELIVERY_AT = GETDATE(), ");
            stb.Append($"DELIVERED_BY = '{deliveredBy}', ");
            stb.Append($"DELIVERY_NOTE = '{deliveredNote}' ");
            stb.Append("WHERE ");
            stb.Append($"GDA_ORDER_IDGDA_ORDER = {orderId} AND ");
            stb.Append($"GDA_PRODUCT_IDGDA_PRODUCT = {productId} AND ");
            stb.Append("ORDER_PRODUCT_STATUS NOT IN ('CANCELED', 'DELIVERED', 'EXPIRED') ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {

                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                        retorno = true;
                    }

                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return retorno;
        }

        public static bool updateOrder(int orderId, int idCollaborator)
        {
            bool retorno = false;

            StringBuilder stb = new StringBuilder();
            stb.Append("UPDATE GDA_ORDER SET ");
            stb.Append($"GDA_ORDER_STATUS_IDGDA_ORDER_STATUS =  2,");
            stb.Append($"DELIVERED_BY =  {idCollaborator},");
            stb.Append($"DELIVERED_AT = GETDATE(),");
            stb.Append($"LAST_UPDATED_AT =  GETDATE(),");
            stb.Append($"LAST_UPDATED_BY =  {idCollaborator}");
            stb.Append("WHERE ");
            stb.Append($"IDGDA_ORDER = {orderId} ");


            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {

                    using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                        retorno = true;
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