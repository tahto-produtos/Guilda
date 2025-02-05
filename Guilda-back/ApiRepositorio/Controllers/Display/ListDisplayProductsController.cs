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
using System.Drawing.Imaging;
using ApiC.Class;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ListDisplayProductsController : ApiController
    {
        public class idsVitrines
        {
            public int id { get; set; }
            public string name { get; set; }
            public string filtro { get; set; }
            public string created_at { get; set; }
        }

        public List<string> verificaEstoques(int idcollaborator)
        {
            List<string> str = new List<string>();

            StringBuilder stb = new StringBuilder();
            stb.Append(" SELECT S1.IDGDA_STOCK AS SB1, S2.IDGDA_STOCK AS SB2 FROM GDA_COLLABORATORS(NOLOCK) CL ");
            stb.AppendFormat(" LEFT JOIN GDA_ATRIBUTES (NOLOCK) A1 ON A1.NAME = 'NOME_CLIENTE' AND A1.CREATED_AT > CONVERT(DATE, DATEADD(DAY, -3, GETDATE())) AND A1.IDGDA_COLLABORATORS = '{0}' ", idcollaborator);
            stb.AppendFormat(" LEFT JOIN GDA_ATRIBUTES (NOLOCK) A2 ON A2.NAME = 'SITE' AND A2.CREATED_AT > CONVERT(DATE, DATEADD(DAY, -3, GETDATE())) AND A2.IDGDA_COLLABORATORS = '{0}' ", idcollaborator);
            stb.Append(" LEFT JOIN GDA_STOCK (NOLOCK) AS S1 ON S1.DELETED_AT IS NULL AND S1.CITY = A2.VALUE AND S1.GDA_ATRIBUTES = A1.VALUE ");
            stb.Append(" LEFT JOIN GDA_STOCK (NOLOCK) AS S2 ON S2.DELETED_AT IS NULL AND S2.CITY = A2.VALUE ");
            stb.AppendFormat(" WHERE CL.IDGDA_COLLABORATORS = {0} ", idcollaborator);
            stb.Append(" GROUP BY S1.IDGDA_STOCK, S2.IDGDA_STOCK ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    using (SqlCommand sqlcommand = new SqlCommand(stb.ToString(), connection))
                    {
                        SqlDataReader reader = sqlcommand.ExecuteReader();
                        while (reader.Read())
                        {
                            if (reader["SB1"].ToString() != "")
                            {
                                str.Add(reader["SB1"].ToString());
                            }
                            if (reader["SB2"].ToString() != "")
                            {
                                str.Add(reader["SB2"].ToString());
                            }


                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
                connection.Close();

            }

            return str;
        }

        public List<idsVitrines> verificaIdsVitrinis(string type, string stocks, int hierarquia, int grupo, int idcollaborator, List<idsVitrines> vits)
        {


            string where = " WHERE DC.ACTIVATED = 1 ";
            if (type == "ADM")
            {
                //Quando for ADM não adiciona filtro
            }
            else if (type == "EstoqueGrupoHierarquia")
            {
                //where = where + $" AND S1.IDGDA_STOCK IN ({stocks}) AND DH.GDA_HIERARCHY_IDGDA_HIERARCHY = {hierarquia}  AND DG.GDA_GROUPS_ID = {grupo} ";
                where = where + $" AND GDA_STOCK_IDGDA_STOCK IN ({stocks}) AND DH.GDA_HIERARCHY_IDGDA_HIERARCHY = {hierarquia}  AND DG.GDA_GROUPS_ID = {grupo} ";
            }
            //if (type == "EstoqueGrupoHierarquia2")
            //{
            //    //where = where + $" AND (S1.IDGDA_STOCK IN ({stocks}) OR S2.IDGDA_STOCK IN ({stocks})) AND DH.GDA_HIERARCHY_IDGDA_HIERARCHY = {hierarquia}  AND DG.GDA_GROUPS_ID = {grupo} ";
            //    where = where + $" AND (S1.IDGDA_STOCK IN ({stocks}) OR S2.IDGDA_STOCK IN ({stocks})) AND DH.GDA_HIERARCHY_IDGDA_HIERARCHY = {hierarquia}  AND DG.GDA_GROUPS_ID = {grupo} ";
            //}
            else if (type == "EstoqueGrupo")
            {
                //where = where + $" AND S1.IDGDA_STOCK IN ({stocks}) AND DG.GDA_GROUPS_ID = {grupo} AND DH.GDA_HIERARCHY_IDGDA_HIERARCHY IS NULL ";
                where = where + $" AND GDA_STOCK_IDGDA_STOCK IN ({stocks}) AND DG.GDA_GROUPS_ID = {grupo} AND DH.GDA_HIERARCHY_IDGDA_HIERARCHY IS NULL ";
            }
            //else if (type == "EstoqueGrupo2")
            //{
            //    //where = where + $" AND (S1.IDGDA_STOCK IN ({stocks}) OR S2.IDGDA_STOCK IN ({stocks})) AND DG.GDA_GROUPS_ID = {grupo} AND DH.GDA_HIERARCHY_IDGDA_HIERARCHY IS NULL  ";
            //    where = where + $" AND (S1.IDGDA_STOCK IN ({stocks}) OR S2.IDGDA_STOCK IN ({stocks})) AND DG.GDA_GROUPS_ID = {grupo} AND DH.GDA_HIERARCHY_IDGDA_HIERARCHY IS NULL  ";
            //}
            else if (type == "EstoqueHieraquia")
            {
                //where = where + $" AND S1.IDGDA_STOCK IN ({stocks}) AND DH.GDA_HIERARCHY_IDGDA_HIERARCHY = {hierarquia} AND DG.GDA_GROUPS_ID IS NULL ";
                where = where + $" AND GDA_STOCK_IDGDA_STOCK IN ({stocks}) AND DH.GDA_HIERARCHY_IDGDA_HIERARCHY = {hierarquia} AND DG.GDA_GROUPS_ID IS NULL ";
            }
            //else if (type == "EstoqueHieraquia2")
            //{
            //    //where = where + $" AND (S1.IDGDA_STOCK IN ({stocks}) OR S2.IDGDA_STOCK IN ({stocks})) AND DH.GDA_HIERARCHY_IDGDA_HIERARCHY = {hierarquia} AND DG.GDA_GROUPS_ID IS NULL ";
            //    where = where + $" AND (S1.IDGDA_STOCK IN ({stocks}) OR S2.IDGDA_STOCK IN ({stocks})) AND DH.GDA_HIERARCHY_IDGDA_HIERARCHY = {hierarquia} AND DG.GDA_GROUPS_ID IS NULL ";
            //}
            else if (type == "GrupoHierarquia")
            {
                //where = where + $" AND DG.GDA_GROUPS_ID = {grupo} AND DH.GDA_HIERARCHY_IDGDA_HIERARCHY = {hierarquia} ";
                where = where + $" AND GDA_STOCK_IDGDA_STOCK IS NULL AND DG.GDA_GROUPS_ID = {grupo} AND DH.GDA_HIERARCHY_IDGDA_HIERARCHY = {hierarquia} ";
            }
            else if (type == "Estoque")
            {
                //where = where + $" AND S1.IDGDA_STOCK IN ({stocks}) AND DH.GDA_HIERARCHY_IDGDA_HIERARCHY IS NULL  AND DG.GDA_GROUPS_ID IS NULL ";
                where = where + $" AND GDA_STOCK_IDGDA_STOCK IN ({stocks}) AND DH.GDA_HIERARCHY_IDGDA_HIERARCHY IS NULL  AND DG.GDA_GROUPS_ID IS NULL ";
            }
            //else if (type == "Estoque2")
            //{
            //    //where = where + $" AND (S1.IDGDA_STOCK IN ({stocks}) OR S2.IDGDA_STOCK IN ({stocks})) AND DH.GDA_HIERARCHY_IDGDA_HIERARCHY IS NULL  AND DG.GDA_GROUPS_ID IS NULL ";
            //    where = where + $" AND (S1.IDGDA_STOCK IN ({stocks}) OR S2.IDGDA_STOCK IN ({stocks})) AND DH.GDA_HIERARCHY_IDGDA_HIERARCHY IS NULL  AND DG.GDA_GROUPS_ID IS NULL ";
            //}
            else if (type == "Grupo")
            {
                //where = where + $" AND DG.GDA_GROUPS_ID = {grupo} AND DH.GDA_HIERARCHY_IDGDA_HIERARCHY IS NULL ";
                where = where + $" AND GDA_STOCK_IDGDA_STOCK IS NULL AND DG.GDA_GROUPS_ID = {grupo} AND DH.GDA_HIERARCHY_IDGDA_HIERARCHY IS NULL ";
            }
            else if (type == "Hierarquia")
            {
                //where = where + $" AND DH.GDA_HIERARCHY_IDGDA_HIERARCHY = {hierarquia}  AND DG.GDA_GROUPS_ID IS NULL ";
                where = where + $" AND GDA_STOCK_IDGDA_STOCK IS NULL AND DH.GDA_HIERARCHY_IDGDA_HIERARCHY = {hierarquia}  AND DG.GDA_GROUPS_ID IS NULL ";
            }



            StringBuilder stb = new StringBuilder();
            stb.Append("SELECT DC.IDGDA_DISPLAY_CONFIG, DC.CREATED_AT, DC.NAME, DH.GDA_HIERARCHY_IDGDA_HIERARCHY, DG.GDA_GROUPS_ID AS CONFIG, DP.IDGDA_DISPLAY_PRIORITY AS PRIORITY, DS.GDA_STOCK_IDGDA_STOCK "); //, S1.IDGDA_STOCK, S2.IDGDA_STOCK
            stb.Append("FROM GDA_DISPLAY_CONFIG DC (NOLOCK) ");
            stb.Append("LEFT JOIN GDA_DISPLAY_GROUP DG (NOLOCK) ON DG.GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG = DC.IDGDA_DISPLAY_CONFIG ");
            stb.Append("LEFT JOIN GDA_DISPLAY_HIERARCHY DH (NOLOCK) ON DH.GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG = DC.IDGDA_DISPLAY_CONFIG ");
            stb.Append("LEFT JOIN GDA_DISPLAY_PRIORITY DP (NOLOCK) ON DC.GDA_DISPLAY_PRIORITY_IDGDA_DISPLAY_PRIORITY = DP.IDGDA_DISPLAY_PRIORITY ");
            stb.Append("LEFT JOIN GDA_DISPLAY_STOCK DS (NOLOCK) ON DC.IDGDA_DISPLAY_CONFIG = DS.GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG ");
            stb.AppendFormat("LEFT JOIN GDA_COLLABORATORS(NOLOCK) CL ON CL.IDGDA_COLLABORATORS = {0} ", idcollaborator);
            stb.AppendFormat("LEFT JOIN GDA_ATRIBUTES (NOLOCK) A1 ON A1.NAME = 'NOME_CLIENTE' AND A1.CREATED_AT = CONVERT(DATE, DATEADD(DAY, -1, GETDATE())) AND A1.IDGDA_COLLABORATORS = '{0}'  ", idcollaborator);
            stb.AppendFormat("LEFT JOIN GDA_ATRIBUTES (NOLOCK) A2 ON A2.NAME = 'SITE' AND A2.CREATED_AT = CONVERT(DATE, DATEADD(DAY, -1, GETDATE())) AND A2.IDGDA_COLLABORATORS = '{0}'  ", idcollaborator);
            //stb.Append("LEFT JOIN GDA_STOCK (NOLOCK) AS S1 ON S1.CITY = A2.VALUE AND S1.GDA_ATRIBUTES = A1.VALUE ");
            //stb.Append("LEFT JOIN GDA_STOCK (NOLOCK) AS S2 ON S2.CITY = A2.VALUE ");
            stb.AppendFormat(" {0} ", where);
            stb.Append("ORDER BY IDGDA_DISPLAY_CONFIG DESC ");


            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();


                    //Tenta com hierarquia e grupo

                    using (SqlCommand sqlcommand = new SqlCommand(stb.ToString(), connection))
                    {
                        SqlDataReader reader = sqlcommand.ExecuteReader();
                        while (reader.Read())
                        {
                            idsVitrines idVit = new idsVitrines();

                            idVit.id = Convert.ToInt32(reader["IDGDA_DISPLAY_CONFIG"]);
                            idVit.filtro = reader["PRIORITY"].ToString();
                            idVit.name = reader["NAME"].ToString();
                            idVit.created_at = reader["CREATED_AT"].ToString();
                            vits.Add(idVit);
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
                connection.Close();
            }
            return vits;
        }

        public class Output
        {
            public int IDGDA_STOCK_PRODUCT { get; set; }
            public int IDGDA_PRODUCT { get; set; }
            public string NOME_VITRINE { get; set; }
            public string COMERCIAL_NAME { get; set; }
            public string DESCRIPTION { get; set; }
            public double PRICE { get; set; }
            public string URL { get; set; }
            public int SALE_LIMIT { get; set; }
            public string CREATED_AT { get; set; }
        }

        [HttpGet]
        [ResponseType(typeof(ResultsModel))]
        public IHttpActionResult PostResultsModel(int hierarquia, int grupo, int collaboratorId)
        {

            string filtro = "";
            string query = "";

            List<string> lstr = verificaEstoques(collaboratorId);
            if (lstr.Count == 0)
            {
                return Ok("Nenhuma estoque vinculado!");
            }

            List<idsVitrines> vits = new List<idsVitrines>();


            string stocks = string.Join(",", lstr);
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();



                bool adm = Funcoes.retornaPermissao(collaboratorId.ToString());
                if (adm == true)
                {
                    vits = verificaIdsVitrinis("ADM", "", 0, 0, 0, vits);
                }
                else
                {
                    //Os finais 2, são por conta de achar estoque com cidade e cliente, ou só com cidade.. ele tenta sempre a maior combinação possivel
                    vits = verificaIdsVitrinis("EstoqueGrupoHierarquia", stocks, hierarquia, grupo, collaboratorId, vits);
                    //if (vit.id == 0)
                    //{
                    //vits = verificaIdsVitrinis("EstoqueGrupoHierarquia2", stocks, hierarquia, grupo, collaboratorId, vits);
                    //}
                    //if (vit.id == 0)
                    //{
                    vits = verificaIdsVitrinis("EstoqueGrupo", stocks, hierarquia, grupo, collaboratorId, vits);
                    //}
                    //if (vit.id == 0)
                    //{
                    //vits = verificaIdsVitrinis("EstoqueGrupo2", stocks, hierarquia, grupo, collaboratorId, vits);
                    //}
                    //if (vit.id == 0)
                    //{
                    vits = verificaIdsVitrinis("EstoqueHieraquia", stocks, hierarquia, grupo, collaboratorId, vits);
                    //}
                    // if (vit.id == 0)
                    //{
                    //vits = verificaIdsVitrinis("EstoqueHieraquia2", stocks, hierarquia, grupo, collaboratorId, vits);
                    //}
                    //if (vit.id == 0)
                    //{
                    vits = verificaIdsVitrinis("GrupoHierarquia", stocks, hierarquia, grupo, collaboratorId, vits);
                    //}
                    //if (vit.id == 0)
                    //{
                    vits = verificaIdsVitrinis("Estoque", stocks, hierarquia, grupo, collaboratorId, vits);
                    //}
                    //if (vit.id == 0)
                    //{
                    //vits = verificaIdsVitrinis("Estoque2", stocks, hierarquia, grupo, collaboratorId, vits);
                    //}
                    //if (vit.id == 0)
                    //{
                    vits = verificaIdsVitrinis("Grupo", stocks, hierarquia, grupo, collaboratorId, vits);
                    //}
                    //if (vit.id == 0)
                    //{
                    vits = verificaIdsVitrinis("Hierarquia", stocks, hierarquia, grupo, collaboratorId, vits);
                    //}
                }


                if (vits.Count == 0)
                {
                    connection.Close();
                    return Ok("Nenhuma configuração encontrada!");
                }

                vits = vits.GroupBy(item => new { item.id }).Select(sel => new idsVitrines
                {
                    filtro = sel.First().filtro,
                    id = sel.Key.id,
                    name = sel.First().name,
                    created_at = sel.First().created_at,
                }).OrderBy(item => item.created_at).ToList();

                List<Output> vitrinesConfigs = new List<Output>();

                foreach (var vit in vits)
                {
                    string queryprodutos = "";
                    filtro = vit.filtro;
                    if (adm == true)
                    {
                        queryprodutos = $@"SELECT P.IDGDA_PRODUCT, P.COMERCIAL_NAME, P.DESCRIPTION, DCI.POSITION FROM GDA_DISPLAY_CONFIG_ITEMS DCI
                                            INNER JOIN GDA_PRODUCT P ON P.IDGDA_PRODUCT = DCI.GDA_PRODUCT_IDGDA_PRODUCT
                                            WHERE DCI.GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG = {vit.id}";
                    }
                    else
                    {
                        queryprodutos = $@"SELECT P.IDGDA_PRODUCT AS IDGDA_PRODUCT, MAX(P.COMERCIAL_NAME), MAX(P.DESCRIPTION),MAX( P.POSITION) AS POSITION, MAX(P.TYPE), MIN(P.TESTE) FROM
                                                (
                                                SELECT P.IDGDA_PRODUCT, P.COMERCIAL_NAME, P.DESCRIPTION, DCI.POSITION, V.TYPE,
                                                CASE WHEN (CASE WHEN V.TYPE = 'GROUP' THEN MAX(G.ID) 
                                                WHEN V.TYPE = 'HIERARCHY' THEN MAX(H.IDGDA_HIERARCHY) 
                                                WHEN V.TYPE = 'SECTOR' THEN MAX(S.IDGDA_SECTOR) 
                                                WHEN V.TYPE = 'COLLABORATOR' THEN MAX(C.IDGDA_COLLABORATORS) ELSE -1
                                                END) IS NULL THEN -1 
                                                ELSE
                                                (CASE WHEN V.TYPE = 'GROUP' THEN MAX(G.ID) 
                                                WHEN V.TYPE = 'HIERARCHY' THEN MAX(H.IDGDA_HIERARCHY) 
                                                WHEN V.TYPE = 'SECTOR' THEN MAX(S.IDGDA_SECTOR) 
                                                WHEN V.TYPE = 'COLLABORATOR' THEN MAX(C.IDGDA_COLLABORATORS) ELSE -1
                                                END)
                                                END AS TESTE
                                                FROM GDA_DISPLAY_CONFIG_ITEMS (NOLOCK) DCI
                                                                                        INNER JOIN GDA_PRODUCT (NOLOCK) P ON P.IDGDA_PRODUCT = DCI.GDA_PRODUCT_IDGDA_PRODUCT
                                                										LEFT JOIN GDA_VISIBILITY (NOLOCK) V ON V.GDA_PRODUCT_IDGDA_PRODUCT_ID = P.IDGDA_PRODUCT
                                                										LEFT JOIN GDA_GROUPS (NOLOCK) G ON V.TYPE = 'GROUP' AND V.VALUE = G.NAME AND G.ID = {grupo}
                                                										LEFT JOIN GDA_HIERARCHY (NOLOCK) H ON V.TYPE = 'HIERARCHY' AND V.VALUE = H.LEVELNAME AND H.IDGDA_HIERARCHY = {hierarquia}
                                                										LEFT JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) CD ON CD.CREATED_AT = CONVERT(DATE, DATEADD(DAY, -1, GETDATE())) AND IDGDA_COLLABORATORS = {collaboratorId}
                                                										LEFT JOIN GDA_SECTOR (NOLOCK) S ON V.TYPE = 'SECTOR' AND V.VALUE = S.NAME
                                                										LEFT JOIN GDA_COLLABORATORS (NOLOCK) C ON V.TYPE = 'COLLABORATOR' AND V.VALUE = C.NAME AND C.IDGDA_COLLABORATORS = {collaboratorId}
                                                                                        WHERE DCI.GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG = {vit.id} 
                                                										GROUP BY  P.IDGDA_PRODUCT, P.COMERCIAL_NAME, P.DESCRIPTION, DCI.POSITION, V.TYPE
                                                ) AS P
                                                GROUP BY P.IDGDA_PRODUCT
                                                HAVING MAX(P.TYPE) IS NULL OR MIN(P.TESTE) > 0;";
                    }

                   

                    string produtos = "";
                    string posicoes = "";
                    using (SqlCommand sqlcommand = new SqlCommand(queryprodutos, connection))
                    {
                        SqlDataReader reader = sqlcommand.ExecuteReader();
                        while (reader.Read())
                        {
                            if (produtos == "")
                            {
                                produtos = reader["IDGDA_PRODUCT"].ToString();
                                posicoes = reader["POSITION"].ToString();
                            }
                            else
                            {
                                produtos += $",{reader["IDGDA_PRODUCT"]}";
                                posicoes += $",{reader["POSITION"]}";
                            }
                        }
                    }

                    if (filtro == "1" && produtos == "")
                    {
                        continue;
                    }

                    //filtros de ordenação
                    //1 - prioridade
                    //2 - dinamico
                    //3 - mais vendido
                    //4 - menos vendido
                    //5 - antigo no estoque
                    //6 - novo no estoque

                    if (filtro == "1")
                    {
                        query = $@"SELECT IDGDA_STOCK_PRODUCT, P.IDGDA_PRODUCT, P.COMERCIAL_NAME, P.DESCRIPTION, P.PRICE, U.URL, P.SALE_LIMIT FROM GDA_PRODUCT P (NOLOCK) 
                            INNER JOIN GDA_PRODUCT_IMAGES I (NOLOCK) ON I.productId = P.IDGDA_PRODUCT AND P.DELETED_AT IS NULL
                            INNER JOIN GDA_STOCK_PRODUCT SP (NOLOCK) ON SP.GDA_PRODUCT_IDGDA_PRODUCT = P.IDGDA_PRODUCT 
                            INNER JOIN GDA_UPLOADS U (NOLOCK) ON U.id = I.uploadId WHERE P.IDGDA_PRODUCT IN ({produtos}) AND P.DELETED_AT IS NULL AND COALESCE(P.GDA_PRODUCT_STATUS_IDGDA_PRODUCT_STATUS, 1) = 1 ORDER BY CASE IDGDA_PRODUCT ";

                        string[] positions = posicoes.Split(',');
                        string[] ids = produtos.Split(',');
                        for (int i = 0; i < ids.Length; i++)
                        {
                            query += $"WHEN {ids[i]} THEN {positions[i]} ";
                        }

                        query += "END";
                    }
                    else if (filtro == "2")
                    {
                        query = $@"SELECT TOP 10 IDGDA_STOCK_PRODUCT, P.IDGDA_PRODUCT, P.COMERCIAL_NAME, P.DESCRIPTION, P.PRICE, U.URL, P.SALE_LIMIT FROM GDA_PRODUCT P (NOLOCK) 
                            INNER JOIN GDA_PRODUCT_IMAGES I (NOLOCK) ON I.productId = P.IDGDA_PRODUCT
                            INNER JOIN GDA_UPLOADS U (NOLOCK) ON U.id = I.uploadId 
                            INNER JOIN GDA_STOCK_PRODUCT SP (NOLOCK) ON SP.GDA_PRODUCT_IDGDA_PRODUCT = P.IDGDA_PRODUCT 
                            WHERE SP.GDA_STOCK_IDGDA_STOCK IN ({stocks}) AND COALESCE(P.GDA_PRODUCT_STATUS_IDGDA_PRODUCT_STATUS, 1) = 1 AND P.DELETED_AT IS NULL ORDER BY  NEWID()";
                    }
                    else if (filtro == "3" || filtro == "4")
                    {
                        string ordenacao = (filtro == "3" ? "DESC" : "ASC");
                        query = $@"SELECT TOP 10 
                            MAX(IDGDA_STOCK_PRODUCT) AS IDGDA_STOCK_PRODUCT,
                            P.IDGDA_PRODUCT,
                            MAX(P.COMERCIAL_NAME) AS COMERCIAL_NAME,
	                        MAX(P.DESCRIPTION) AS DESCRIPTION,
	                        MAX(P.PRICE) AS PRICE,
	                        MAX(U.URL) AS URL,
                            COUNT(OP.IDGDA_ORDER_PRODUCT) AS VENDIDOS,
                            MAX(P.SALE_LIMIT) AS SALE_LIMIT 
                            FROM 
                                GDA_PRODUCT P (NOLOCK)
	                        INNER JOIN GDA_PRODUCT_IMAGES I (NOLOCK) ON I.productId = P.IDGDA_PRODUCT
	                        INNER JOIN GDA_UPLOADS U (NOLOCK) ON U.id = I.uploadId
                            INNER JOIN GDA_STOCK_PRODUCT SP (NOLOCK) ON SP.GDA_PRODUCT_IDGDA_PRODUCT = P.IDGDA_PRODUCT 
                            JOIN 
                                GDA_ORDER_PRODUCT OP (NOLOCK) ON P.IDGDA_PRODUCT = OP.GDA_PRODUCT_IDGDA_PRODUCT
                            WHERE
                            	P.DELETED_AT IS NULL AND COALESCE(P.GDA_PRODUCT_STATUS_IDGDA_PRODUCT_STATUS, 1) = 1 AND OP.CREATED_AT >= DATEADD(month, -3, GETDATE()) AND SP.GDA_STOCK_IDGDA_STOCK IN ({stocks}) 
                            GROUP BY 
                                P.IDGDA_PRODUCT
                            ORDER BY 
                                 VENDIDOS {ordenacao};";
                    }
                    else if (filtro == "5" || filtro == "6")
                    {
                        string ordenacao = (filtro == "5" ? "ASC" : "DESC");
                        query = $@"SELECT TOP 10 IDGDA_STOCK_PRODUCT, P.IDGDA_PRODUCT, P.COMERCIAL_NAME, P.DESCRIPTION, P.PRICE, U.URL, P.SALE_LIMIT FROM GDA_PRODUCT P (NOLOCK) 
                            INNER JOIN GDA_PRODUCT_IMAGES I (NOLOCK) ON I.productId = P.IDGDA_PRODUCT 
                            INNER JOIN GDA_UPLOADS U (NOLOCK) ON U.id = I.uploadId 
                            INNER JOIN GDA_STOCK_PRODUCT SP (NOLOCK) ON SP.GDA_PRODUCT_IDGDA_PRODUCT = P.IDGDA_PRODUCT 
                            WHERE P.DELETED_AT IS NULL AND COALESCE(P.GDA_PRODUCT_STATUS_IDGDA_PRODUCT_STATUS, 1) = 1 AND SP.GDA_STOCK_IDGDA_STOCK IN ({stocks}) ORDER BY P.CREATED_AT {ordenacao}";
                    }

                    if (filtro == "")
                    {
                        return BadRequest("Não foi possivel encontrar nenhuma vitrine disponivel!");
                    }

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            Output vitrine = new Output();
                            vitrine.COMERCIAL_NAME = reader["COMERCIAL_NAME"].ToString();
                            vitrine.IDGDA_STOCK_PRODUCT = Convert.ToInt32(reader["IDGDA_STOCK_PRODUCT"].ToString());
                            vitrine.IDGDA_PRODUCT = Convert.ToInt32(reader["IDGDA_PRODUCT"].ToString());
                            vitrine.DESCRIPTION = reader["DESCRIPTION"].ToString();
                            vitrine.PRICE = Convert.ToDouble(reader["PRICE"].ToString());
                            vitrine.URL = reader["URL"].ToString();
                            if (reader["SALE_LIMIT"] != DBNull.Value)
                            {
                                vitrine.SALE_LIMIT = Convert.ToInt32(reader["SALE_LIMIT"].ToString());
                            }
                            else
                            {
                                vitrine.SALE_LIMIT = 0;
                            }
                            vitrine.NOME_VITRINE = vit.name;
                            vitrinesConfigs.Add(vitrine);
                        }
                        //using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        //{

                        //    DataTable dataTable = new DataTable();
                        //    adapter.Fill(dataTable);
                        //    connection.Close();
                        //    return Ok(dataTable);
                        //}
                    }




                }
                connection.Close();

                if (vitrinesConfigs.Count() == 0)
                {
                    return Ok("Nenhuma configuração encontrada!");
                }

                vitrinesConfigs = vitrinesConfigs.GroupBy(item => new { item.NOME_VITRINE, item.IDGDA_PRODUCT }).Select(sel => new Output
                {

                    IDGDA_STOCK_PRODUCT = sel.First().IDGDA_STOCK_PRODUCT,
                    IDGDA_PRODUCT = sel.First().IDGDA_PRODUCT,
                    NOME_VITRINE = sel.First().NOME_VITRINE,
                    COMERCIAL_NAME = sel.First().COMERCIAL_NAME,
                    DESCRIPTION = sel.First().DESCRIPTION,
                    PRICE = sel.First().PRICE,
                    URL = sel.First().URL,
                    SALE_LIMIT = sel.First().SALE_LIMIT,
                    CREATED_AT = sel.First().CREATED_AT,
                }).ToList();


                var jsonData = vitrinesConfigs.Select(item => new Output
                {
                    COMERCIAL_NAME = item.COMERCIAL_NAME,
                    IDGDA_STOCK_PRODUCT = item.IDGDA_STOCK_PRODUCT,
                    IDGDA_PRODUCT = item.IDGDA_PRODUCT,
                    NOME_VITRINE = item.NOME_VITRINE,
                    DESCRIPTION = item.DESCRIPTION,
                    PRICE = item.PRICE,
                    URL = item.URL,
                    SALE_LIMIT = item.SALE_LIMIT,
                    CREATED_AT = item.CREATED_AT,
                }).ToList();
                return Ok(jsonData);

            }
        }
    }
}