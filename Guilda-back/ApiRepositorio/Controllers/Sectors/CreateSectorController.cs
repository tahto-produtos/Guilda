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
using OfficeOpenXml;
using Utilities;
using ApiC.Class;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class CreateSectorController : ApiController
    {

        public class InputModel
        {
            public int COD_GIP { get; set; }
            public string name { get; set; }
            public int level { get; set; }
            public List<int> indicatorsIds { get; set; }
        }

        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {
            int COLLABORATORID = 0;
            int PERSONAUSERID = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            COLLABORATORID = inf.collaboratorId;
            PERSONAUSERID = inf.personauserId;

            //Verifica se o setor ja existe
            string result = bankCreateSector.verifySector(inputModel.COD_GIP);
            if (result == "EXISTE")
            {
                return BadRequest("Este cod de setor ja existe!");
            }

            //Verifica se selecionou mais de um setor
            if (inputModel.indicatorsIds.Count == 0)
            {
                return BadRequest("Selecione ao menos um indicador!");
            }

            if (result == "REMOVIDO")
            {
                bankCreateSector.updateSector(inputModel.COD_GIP, inputModel.name);
            }
            else
            {
                bankCreateSector.insertSector(inputModel);
            }

            return Ok();
        }
    }

    public class bankCreateSector
    {
        public static string verifySector(int idSector)
        {
            string retorno = "";
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {

                    StringBuilder sbInsert = new StringBuilder();
                    sbInsert.AppendFormat("SELECT DELETED_AT FROM GDA_SECTOR (NOLOCK) ");
                    sbInsert.AppendFormat($"WHERE IDGDA_SECTOR = {idSector} ");

                    using (SqlCommand commandInsert = new SqlCommand(sbInsert.ToString(), connection))
                    {
                        using (SqlDataReader reader = commandInsert.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader["DELETED_AT"] != DBNull.Value)
                                {
                                    retorno = "REMOVIDO";
                                }
                                else
                                {
                                    retorno = "EXISTE";
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                connection.Close();
            }

            if (retorno == "")
            {
                retorno = "NÃO EXISTE";
            }
            return retorno;

        }

        public static void updateSector(int idSector, string name)
        {
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {

                    StringBuilder sbInsert = new StringBuilder();
                    sbInsert.AppendFormat("UPDATE GDA_SECTOR ");
                    sbInsert.AppendFormat($"SET DELETED_AT = NULL, NAME = '{name}' WHERE IDGDA_SECTOR = {idSector} ");

                    using (SqlCommand commandInsert = new SqlCommand(sbInsert.ToString(), connection))
                    {
                        commandInsert.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {
                }
                connection.Close();
            }
        }

        public static void insertSector(CreateSectorController.InputModel inputModel)
        {
            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();
                try
                {
                    StringBuilder sbInsert = new StringBuilder();
                    sbInsert.AppendFormat("INSERT INTO GDA_SECTOR (IDGDA_SECTOR, NAME, LEVEL, CREATED_AT, DELETED_AT, SECTOR, SUBSECTOR, REMOVED_AT) VALUES ( ");
                    sbInsert.AppendFormat($"{inputModel.COD_GIP}, "); //IDGDA_SECTOR
                    sbInsert.AppendFormat($"'{inputModel.name}', "); //NAME
                    sbInsert.AppendFormat($"{inputModel.level}, "); //LEVEL
                    sbInsert.AppendFormat($"GETDATE(), "); //CREATED_AT
                    sbInsert.AppendFormat($"NULL, "); //DELETED_AT
                    sbInsert.AppendFormat($"1, "); //SECTOR
                    sbInsert.AppendFormat($"0, "); //SUBSECTOR
                    sbInsert.AppendFormat($"NULL "); //REMOVED_AT
                    sbInsert.AppendFormat($"); ");
                    using (SqlCommand commandInsert = new SqlCommand(sbInsert.ToString(), connection))
                    {
                        commandInsert.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {
                }
                connection.Close();
            }
        }

    }
}