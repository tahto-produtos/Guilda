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
using static ApiRepositorio.Controllers.HierarchyEscalationController;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class HierarchyEscalationController : ApiController
    {

        public class returnHierarchyEscalation
        {
            public List<hierarchyEscalation> items { get; set; }

        }

        public class hierarchyEscalation
        {
            public int id { get; set; }
            public string levelName { get; set; }
            public int levelWeight { get; set; }
            public DateTime? createdAt { get; set; }
            public DateTime? deletedAt { get; set; }

        }

        // POST: api/Results
        [HttpGet]
        public IHttpActionResult GetResultsModel(int limit, int offset, string searchText)
        {


            //Realiza a query que retorna todas as informações dos colaboradores que tiveram moneitzação.
            returnHierarchyEscalation rmams = new returnHierarchyEscalation();
            rmams = bankHierarchyEscalation.listHierarchy(searchText);

            // Use o método Ok() para retornar o objeto serializado em JSON
            return Ok(rmams);
        }


        // Método para serializar um DataTable em JSON
    }

    public class bankHierarchyEscalation
    {
        public static returnHierarchyEscalation listHierarchy(string st)
        {
            returnHierarchyEscalation llh = new returnHierarchyEscalation();
            llh.items = new List<hierarchyEscalation>();

            string filtro = "";
            if (filtro != "")
            {
                filtro = $" AND LEVELNAME LIKE '%{st}%' ";
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat($" SELECT IDGDA_HIERARCHY, LEVELNAME, LEVELWEIGHT, CREATED_AT, DELETED_AT FROM GDA_HIERARCHY (NOLOCK) WHERE 1 = 1 AND IDGDA_HIERARCHY > 1 {filtro} ");


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
                                hierarchyEscalation hierarc = new hierarchyEscalation();
                                hierarc.id = int.Parse(reader["IDGDA_HIERARCHY"].ToString());
                                hierarc.levelName = reader["LEVELNAME"].ToString();
                                hierarc.levelWeight = int.Parse(reader["LEVELWEIGHT"].ToString());
                                hierarc.createdAt = Convert.ToDateTime(reader["CREATED_AT"].ToString());
                                hierarc.deletedAt = (DateTime?)null;
                                llh.items.Add(hierarc);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

            return llh;
        }
    }
}