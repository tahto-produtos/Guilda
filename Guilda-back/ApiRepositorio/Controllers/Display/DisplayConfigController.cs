using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Http;
using System.Web.Http.Description;
using ApiRepositorio.Models;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class DisplayConfigController : ApiController
    {
        [HttpPost]
        [ResponseType(typeof(ResultsModel))]
        public IHttpActionResult PostResultsModel([FromBody] DisplayRequestModel model)
        {
           
            int prioridade = model.PRIORIDADE;
            string config = model.NOMECONFIG;
            List<Item> itens = model.ITENS;
            List<Grupo> grupos = model.GRUPO;
            List<Hierarquia> hierarquias = model.HIERARQUIA;
            List<Estoque> estoques = model.ESTOQUE;


            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand select = connection.CreateCommand())
                    {
                        foreach (Hierarquia hierarquia in hierarquias)
                        {
                            foreach (Grupo grupo in grupos)
                            {
                                foreach (Estoque estoque in estoques)
                                {
                                    string querybusca = $@"SELECT DC.IDGDA_DISPLAY_CONFIG, DC.NAME AS CONFIG, DP.NAME AS PRIORITY FROM GDA_DISPLAY_CONFIG DC (NOLOCK)
                                        INNER JOIN GDA_DISPLAY_GROUP DG (NOLOCK) ON DG.GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG = DC.IDGDA_DISPLAY_CONFIG
                                        INNER JOIN GDA_DISPLAY_HIERARCHY DH (NOLOCK) ON DH.GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG = DC.IDGDA_DISPLAY_CONFIG
                                        INNER JOIN GDA_DISPLAY_PRIORITY DP (NOLOCK) ON DC.GDA_DISPLAY_PRIORITY_IDGDA_DISPLAY_PRIORITY = DP.IDGDA_DISPLAY_PRIORITY
                                        INNER JOIN GDA_DISPLAY_STOCK ST (NOLOCK) ON DC.IDGDA_DISPLAY_CONFIG = ST.GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG
                                        WHERE DH.GDA_HIERARCHY_IDGDA_HIERARCHY = {hierarquia.CODHIERARQUIA} AND DG.GDA_GROUPS_ID = {grupo.CODGRUPO} AND ST.GDA_STOCK_IDGDA_STOCK = {estoque.CODESTOQUE} 
                                        AND DC.ACTIVATED = 1 
                                        ";
                                    using (SqlCommand sqlcommand = new SqlCommand(querybusca, connection))
                                    {
                                        SqlDataReader reader = sqlcommand.ExecuteReader();
                                        while (reader.Read())
                                        {
                                            return BadRequest($"Já existe uma configuração criada para a hierarquia e grupo selecionados.");
                                        }
                                    }
                                }
                            }
                        }
                    }

                    int id = 0;
                    string queryconfig = $@"INSERT INTO GDA_DISPLAY_CONFIG (NAME, GDA_DISPLAY_PRIORITY_IDGDA_DISPLAY_PRIORITY, ACTIVATED, CREATED_AT) VALUES ('{config}', {prioridade}, 1, GETDATE()); SELECT SCOPE_IDENTITY();";
                    using (SqlCommand sqlcommand = new SqlCommand(queryconfig, connection))
                    {
                        id = Convert.ToInt32(sqlcommand.ExecuteScalar());
                    }

                    foreach (Item item in itens)
                    {
                        string queryitens = $@"INSERT INTO GDA_DISPLAY_CONFIG_ITEMS (GDA_PRODUCT_IDGDA_PRODUCT, POSITION, GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG) VALUES ('{item.CODPRODUTO}', '{item.POSICAO}', {id})";
                        using (SqlCommand sqlcommand = new SqlCommand(queryitens, connection))
                        {
                            sqlcommand.ExecuteNonQuery();
                        }
                    }

                    foreach (Grupo grupo in grupos)
                    {
                        string querygrupo = $@"INSERT INTO GDA_DISPLAY_GROUP (GDA_GROUPS_ID, GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG) VALUES ('{grupo.CODGRUPO}', {id})";
                        using (SqlCommand sqlcommand = new SqlCommand(querygrupo, connection))
                        {
                            sqlcommand.ExecuteNonQuery();
                        }
                    }

                    foreach (Hierarquia hierarquia in hierarquias)
                    {
                        string queryhierarquia = $@"INSERT INTO GDA_DISPLAY_HIERARCHY (GDA_HIERARCHY_IDGDA_HIERARCHY, GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG) VALUES ('{hierarquia.CODHIERARQUIA}', {id})";
                        using (SqlCommand sqlcommand = new SqlCommand(queryhierarquia, connection))
                        {
                            sqlcommand.ExecuteNonQuery();
                        }
                    }

                    foreach (Estoque estoque in estoques)
                    {
                        string queryestoque = $@"INSERT INTO GDA_DISPLAY_STOCK (GDA_STOCK_IDGDA_STOCK, GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG) VALUES ('{estoque.CODESTOQUE}', {id})";
                        using (SqlCommand sqlcommand = new SqlCommand(queryestoque, connection))
                        {
                            sqlcommand.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    connection.Close();
                    return BadRequest(ex.Message);
                }
                connection.Close();
                return Ok("Configuração cadastrada com sucesso.");
            }
        }

        [HttpDelete]
        public IHttpActionResult DeleteResultsModel(int displayId)
        {
            try
            {
                 using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    string queryDeleteConfig = $@"DELETE FROM GDA_DISPLAY_CONFIG WHERE IDGDA_DISPLAY_CONFIG = {displayId}";
                    using (SqlCommand sqlcommand = new SqlCommand(queryDeleteConfig, connection))
                    {
                        sqlcommand.ExecuteNonQuery();
                    }

                    string queryDeleteConfigItems = $@"DELETE FROM GDA_DISPLAY_CONFIG_ITEMS WHERE GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG = {displayId}";
                    using (SqlCommand sqlcommand = new SqlCommand(queryDeleteConfigItems, connection))
                    {
                        sqlcommand.ExecuteNonQuery();
                    }

                    string queryDeleteGroup = $@"DELETE FROM GDA_DISPLAY_GROUP WHERE GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG = {displayId}";
                    using (SqlCommand sqlcommand = new SqlCommand(queryDeleteGroup, connection))
                    {
                        sqlcommand.ExecuteNonQuery();
                    }

                    string queryDeleteHierarchy = $@"DELETE FROM GDA_DISPLAY_HIERARCHY WHERE GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG = {displayId}";
                    using (SqlCommand sqlcommand = new SqlCommand(queryDeleteHierarchy, connection))
                    {
                        sqlcommand.ExecuteNonQuery();
                    }

                    string queryDeleteStock = $@"DELETE FROM GDA_DISPLAY_STOCK WHERE GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG = {displayId}";
                    using (SqlCommand sqlcommand = new SqlCommand(queryDeleteStock, connection))
                    {
                        sqlcommand.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                return Ok("Configuração deletada com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest("Ocorreu um erro ao deletar a configuração selecionada: " + ex.Message);
            }
        }

        [HttpPut]
        public IHttpActionResult UpdateResultsModel(int displayId, int activated, [FromBody] DisplayRequestModel model)
        {
            try
            {
                int prioridade = model.PRIORIDADE;
                string config = model.NOMECONFIG;
                List<Item> itens = model.ITENS;
                List<Grupo> grupos = model.GRUPO;
                List<Hierarquia> hierarquias = model.HIERARQUIA;
                List<Estoque> estoques = model.ESTOQUE;

                
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    //using (SqlCommand select = connection.CreateCommand())
                    //{
                    //    foreach (Hierarquia hierarquia in hierarquias)
                    //    {
                    //        foreach (Grupo grupo in grupos)
                    //        {
                    //            string querybusca = $@"SELECT DC.IDGDA_DISPLAY_CONFIG, DC.NAME AS CONFIG, DP.NAME AS PRIORITY FROM GDA_DISPLAY_CONFIG DC (NOLOCK)
                    //                    INNER JOIN GDA_DISPLAY_GROUP DG (NOLOCK) ON DG.GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG = DC.IDGDA_DISPLAY_CONFIG
                    //                    INNER JOIN GDA_DISPLAY_HIERARCHY DH (NOLOCK) ON DH.GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG = DC.IDGDA_DISPLAY_CONFIG
                    //                    INNER JOIN GDA_DISPLAY_PRIORITY DP (NOLOCK) ON DC.GDA_DISPLAY_PRIORITY_IDGDA_DISPLAY_PRIORITY = DP.IDGDA_DISPLAY_PRIORITY
                    //                    WHERE DH.GDA_HIERARCHY_IDGDA_HIERARCHY = {hierarquia.CODHIERARQUIA} AND DG.GDA_GROUPS_ID = {grupo.CODGRUPO} AND IDGDA_DISPLAY_CONFIG <> {displayId}";
                    //            using (SqlCommand sqlcommand = new SqlCommand(querybusca, connection))
                    //            {
                    //                SqlDataReader reader = sqlcommand.ExecuteReader();
                    //                while (reader.Read())
                    //                {
                    //                    return BadRequest($"Já existe uma configuração criada para a hierarquia e grupo selecionados.");
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

                    //ATUALIZA CONFIGURACAO
                    string queryUpdateConfig = $@"UPDATE GDA_DISPLAY_CONFIG SET NAME = '{model.NOMECONFIG}', GDA_DISPLAY_PRIORITY_IDGDA_DISPLAY_PRIORITY = {model.PRIORIDADE}, ACTIVATED = {activated} WHERE IDGDA_DISPLAY_CONFIG = {displayId}";
                    using (SqlCommand sqlcommand = new SqlCommand(queryUpdateConfig, connection))
                    {
                        sqlcommand.ExecuteNonQuery();
                    }

                    //DELETA E INSERE ITENS
                    string queryDeleteConfigItems = $@"DELETE FROM GDA_DISPLAY_CONFIG_ITEMS WHERE GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG = {displayId}";
                    using (SqlCommand sqlcommand = new SqlCommand(queryDeleteConfigItems, connection))
                    {
                        sqlcommand.ExecuteNonQuery();
                    }
                    foreach (Item item in itens)
                    {
                        string queryitens = $@"INSERT INTO GDA_DISPLAY_CONFIG_ITEMS (GDA_PRODUCT_IDGDA_PRODUCT, POSITION, GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG) VALUES ('{item.CODPRODUTO}', '{item.POSICAO}', {displayId})";
                        using (SqlCommand sqlcommand = new SqlCommand(queryitens, connection))
                        {
                            sqlcommand.ExecuteNonQuery();
                        }
                    }

                    //DELETA E INSERE GRUPOS
                    string queryDeleteGroup = $@"DELETE FROM GDA_DISPLAY_GROUP WHERE GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG = {displayId}";
                    using (SqlCommand sqlcommand = new SqlCommand(queryDeleteGroup, connection))
                    {
                        sqlcommand.ExecuteNonQuery();
                    }
                    foreach (Grupo grupo in grupos)
                    {
                        string querygrupo = $@"INSERT INTO GDA_DISPLAY_GROUP (GDA_GROUPS_ID, GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG) VALUES ('{grupo.CODGRUPO}', {displayId})";
                        using (SqlCommand sqlcommand = new SqlCommand(querygrupo, connection))
                        {
                            sqlcommand.ExecuteNonQuery();
                        }
                    }

                    //DELETA E INSERE HIERARQUIA
                    string queryDeleteHierarchy = $@"DELETE FROM GDA_DISPLAY_HIERARCHY WHERE GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG = {displayId}";
                    using (SqlCommand sqlcommand = new SqlCommand(queryDeleteHierarchy, connection))
                    {
                        sqlcommand.ExecuteNonQuery();
                    }
                    foreach (Hierarquia hierarquia in hierarquias)
                    {
                        string queryhierarquia = $@"INSERT INTO GDA_DISPLAY_HIERARCHY (GDA_HIERARCHY_IDGDA_HIERARCHY, GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG) VALUES ('{hierarquia.CODHIERARQUIA}', {displayId})";
                        using (SqlCommand sqlcommand = new SqlCommand(queryhierarquia, connection))
                        {
                            sqlcommand.ExecuteNonQuery();
                        }
                    }

                    //DELETA E INSERE ESTOQUE
                    string queryDeleteStock = $@"DELETE FROM GDA_DISPLAY_STOCK WHERE GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG = {displayId}";
                    using (SqlCommand sqlcommand = new SqlCommand(queryDeleteStock, connection))
                    {
                        sqlcommand.ExecuteNonQuery();
                    }
                    foreach (Estoque estoque in estoques)
                    {
                        string queryestoque = $@"INSERT INTO GDA_DISPLAY_STOCK (GDA_STOCK_IDGDA_STOCK, GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG) VALUES ('{estoque.CODESTOQUE}', {displayId})";
                        using (SqlCommand sqlcommand = new SqlCommand(queryestoque, connection))
                        {
                            sqlcommand.ExecuteNonQuery();
                        }
                    }
                    connection.Close();
                }
                return Ok("Configuração alterada com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest("Ocorreu um erro ao alterar a configuração selecionada: " + ex.Message);
            }
        }
    }
}