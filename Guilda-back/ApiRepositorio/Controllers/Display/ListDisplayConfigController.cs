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
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class ListDisplayConfigController : ApiController
    {
        [HttpGet]
        [ResponseType(typeof(DisplayResponseModel))]
        public IHttpActionResult PostResultsModel(string idConfig)
        {
           
            string where = "";
            if (idConfig != null)
            {
                where = $" WHERE IDGDA_DISPLAY_CONFIG = {idConfig} ";
            }


            List<int> ids = new List<int>();
            List<DisplayResponseModel> displayModelList = new List<DisplayResponseModel>();

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    string queryconfig = $@"SELECT IDGDA_DISPLAY_CONFIG FROM GDA_DISPLAY_CONFIG (NOLOCK) {where} "; //WHERE ACTIVATED = 1
                    using (SqlCommand sqlcommand = new SqlCommand(queryconfig, connection))
                    {
                        SqlDataReader reader = sqlcommand.ExecuteReader();
                        while (reader.Read())
                        {
                            int id = Convert.ToInt32(reader["IDGDA_DISPLAY_CONFIG"]);
                            ids.Add(id);
                        }
                    }

                    foreach (int id in ids)
                    {
                        DisplayResponseModel displayModel = new DisplayResponseModel();
                        displayModel.ITENS = new List<ItemResponse>();
                        displayModel.GRUPO = new List<GrupoResponse>();
                        displayModel.HIERARQUIA = new List<HierarquiaResponse>();
                        displayModel.ESTOQUE = new List<EstoqueResponse>();

                        string queryconfigall = $@"SELECT * FROM GDA_DISPLAY_CONFIG (NOLOCK) WHERE IDGDA_DISPLAY_CONFIG = {id}";
                        using (SqlCommand sqlcommand = new SqlCommand(queryconfigall, connection))
                        {
                            SqlDataReader reader = sqlcommand.ExecuteReader();
                            while (reader.Read())
                            {
                                //1 - prioridade
                                //2 - dinamico
                                //3 - mais vendido
                                //4 - menos vendido
                                //5 - antigo no estoque
                                //6 - novo no estoque
                                displayModel.IDCONFIG = Convert.ToInt32(reader["IDGDA_DISPLAY_CONFIG"]);
                                displayModel.PRIORIDADE = Convert.ToInt32(reader["GDA_DISPLAY_PRIORITY_IDGDA_DISPLAY_PRIORITY"]);
                                if (displayModel.PRIORIDADE == 1)
                                {
                                    displayModel.NOMEPRIORIDADE = "Prioridade";
                                }
                                else if (displayModel.PRIORIDADE == 2)
                                {
                                    displayModel.NOMEPRIORIDADE = "Dinamico";
                                }
                                else if (displayModel.PRIORIDADE == 3)
                                {
                                    displayModel.NOMEPRIORIDADE = "Mais vendido";
                                }
                                else if (displayModel.PRIORIDADE == 4)
                                {
                                    displayModel.NOMEPRIORIDADE = "Menos vendido";
                                }
                                else if (displayModel.PRIORIDADE == 5)
                                {
                                    displayModel.NOMEPRIORIDADE = "Antigo no estoque";
                                }
                                else if (displayModel.PRIORIDADE == 6)
                                {
                                    displayModel.NOMEPRIORIDADE = "Novo no estoque";
                                }

                                displayModel.NOMECONFIG = reader["NAME"].ToString();
                                displayModel.STATUS = reader["ACTIVATED"].ToString();
                            }
                        }

                        string queryconfigitens = $@"SELECT D.GDA_PRODUCT_IDGDA_PRODUCT AS CODPRODUTO, P.COMERCIAL_NAME, P.DESCRIPTION, D.POSITION, D.GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG FROM GDA_DISPLAY_CONFIG_ITEMS D (NOLOCK)
                                                     INNER JOIN GDA_PRODUCT P (NOLOCK) ON P.IDGDA_PRODUCT = D.GDA_PRODUCT_IDGDA_PRODUCT 
                                                     WHERE D.GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG = {id}";
                        using (SqlCommand sqlcommand = new SqlCommand(queryconfigitens, connection))
                        {
                            SqlDataReader reader = sqlcommand.ExecuteReader();
                            while (reader.Read())
                            {
                                ItemResponse item = new ItemResponse();
                                item.CODPRODUTO = Convert.ToInt32(reader["CODPRODUTO"]);
                                item.NOMEPRODUTO = reader["COMERCIAL_NAME"].ToString();
                                item.POSICAO = Convert.ToInt32(reader["POSITION"]);
                                displayModel.ITENS.Add(item);
                            }
                        }

                        string queryhierarquia = $@"SELECT D.GDA_HIERARCHY_IDGDA_HIERARCHY AS CODHIERARQUIA, H.LEVELNAME, D.GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG FROM GDA_DISPLAY_HIERARCHY D (NOLOCK)
                                                    INNER JOIN GDA_HIERARCHY H (NOLOCK) ON H.IDGDA_HIERARCHY = D.GDA_HIERARCHY_IDGDA_HIERARCHY
                                                    WHERE D.GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG = {id}";
                        using (SqlCommand sqlcommand = new SqlCommand(queryhierarquia, connection))
                        {
                            SqlDataReader reader = sqlcommand.ExecuteReader();
                            while (reader.Read())
                            {
                                HierarquiaResponse hierarquia = new HierarquiaResponse();
                                hierarquia.CODHIERARQUIA = Convert.ToInt32(reader["CODHIERARQUIA"]);
                                hierarquia.NOMEHIERARQUIA = reader["LEVELNAME"].ToString();
                                displayModel.HIERARQUIA.Add(hierarquia);
                            }
                        }

                        string querygrupo = $@"SELECT D.GDA_GROUPS_ID AS CODGRUPO, G.ALIAS, D.GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG FROM GDA_DISPLAY_GROUP D (NOLOCK)
                                                    INNER JOIN GDA_GROUPS G (NOLOCK) ON G.id = D.GDA_GROUPS_ID 
                                                    WHERE D.GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG = {id}";
                        using (SqlCommand sqlcommand = new SqlCommand(querygrupo, connection))
                        {
                            SqlDataReader reader = sqlcommand.ExecuteReader();
                            while (reader.Read())
                            {
                                GrupoResponse grupo = new GrupoResponse();
                                grupo.CODGRUPO = Convert.ToInt32(reader["CODGRUPO"]);
                                grupo.NOMEGRUPO = reader["ALIAS"].ToString();
                                displayModel.GRUPO.Add(grupo);
                            }
                        }

                        string queryestoque = $@"SELECT D.GDA_STOCK_IDGDA_STOCK AS CODESTOQUE, S.DESCRIPTION, D.GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG FROM GDA_DISPLAY_STOCK D (NOLOCK)
                                                INNER JOIN GDA_STOCK S (NOLOCK) ON S.IDGDA_STOCK = D.GDA_STOCK_IDGDA_STOCK 
                                                WHERE GDA_DISPLAY_CONFIG_IDGDA_DISPLAY_CONFIG = {id}";
                        using (SqlCommand sqlcommand = new SqlCommand(queryestoque, connection))
                        {
                            SqlDataReader reader = sqlcommand.ExecuteReader();
                            while (reader.Read())
                            {
                                EstoqueResponse estoque = new EstoqueResponse();
                                estoque.CODESTOQUE = Convert.ToInt32(reader["CODESTOQUE"]);
                                estoque.NOMEESTOQUE = reader["DESCRIPTION"].ToString();
                                displayModel.ESTOQUE.Add(estoque);
                            }
                        }
                        displayModelList.Add(displayModel);
                    }
                }
                catch (Exception ex)
                {
                    connection.Close();
                    return BadRequest(ex.Message);
                }
                connection.Close();
                return Ok(displayModelList);
            }
        }
    }
}