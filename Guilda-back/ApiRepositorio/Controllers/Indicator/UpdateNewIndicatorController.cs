using System.Data.SqlClient;
using System.Web.Http;
using System.Web.Http.Description;
using ApiRepositorio.Models;
using System.Web;
using System;
using System.Data;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class UpdateNewIndicatorController : ApiController
    {
        [HttpPost]
        [ResponseType(typeof(ResultsModel))]
        public IHttpActionResult PostResultsModel()
        {
            
            string indicatorId = HttpContext.Current.Request.Form["INDICATORID"];
            string name = HttpContext.Current.Request.Form["NAME"];
            string description = HttpContext.Current.Request.Form["DESCRIPTION"];
            string sector = HttpContext.Current.Request.Form["SECTOR"];
            string metric = HttpContext.Current.Request.Form["METRIC"];
            string calctype = HttpContext.Current.Request.Form["CALCTYPE"];
            string weight = HttpContext.Current.Request.Form["WEIGHT"];

            //Faz o primeiro update (GDA_INDICATOR)
            string update1 = $"UPDATE GDA_INDICATOR SET NAME = '{name}', DESCRIPTION = '{description}', CALCULATION_TYPE = '{calctype}', WEIGHT = '{weight}', NEWAPI = 0 WHERE IDGDA_INDICATOR = {indicatorId}";

            try 
            {
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(update1, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch (Exception ex) 
            {
                return BadRequest($"Erro na atualização do indicador: {ex.Message}");
            }
            

            //Faz o segundo update (GDA_HISTORY_INDICATOR_SECTORS)
            string[] idlist = sector.Split(';');

            if (sector == "")
            {
                try
                {

                    using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                    {
                        connection.Open();

                        string deleteIdsNotInList = $"UPDATE GDA_HISTORY_INDICATOR_SECTORS SET DELETED_AT = GETDATE() WHERE INDICATOR_ID = {indicatorId} AND DELETED_AT IS NULL";
                        using (SqlCommand deleteCommand = new SqlCommand(deleteIdsNotInList, connection))
                        {
                            deleteCommand.ExecuteNonQuery();
                        }
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"Erro na inserção de setor: {ex.Message}");
                }
            }
            else
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                    {
                        connection.Open();

                        foreach (string id in idlist)
                        {
                            int idInt = int.Parse(id);

                            string checkIfIdExists = $"SELECT COUNT(*) FROM GDA_HISTORY_INDICATOR_SECTORS WHERE SECTOR_ID = {idInt} AND INDICATOR_ID = {indicatorId} AND DELETED_AT IS NULL";
                            using (SqlCommand command = new SqlCommand(checkIfIdExists, connection))
                            {
                                int count = (int)command.ExecuteScalar();
                                if (count == 0)
                                {
                                    string insertNew = $"INSERT INTO GDA_HISTORY_INDICATOR_SECTORS (INDICATOR_ID, SECTOR_ID, CREATED_AT, GOAL) VALUES ('{indicatorId}','{id}',GETDATE(), 0)";
                                    using (SqlCommand insertCommand = new SqlCommand(insertNew, connection))
                                    {
                                        insertCommand.ExecuteNonQuery();
                                    }
                                }
                            }
                        }

                        string idsToDelete = string.Join(",", idlist);
                        string deleteIdsNotInList = $"UPDATE GDA_HISTORY_INDICATOR_SECTORS SET DELETED_AT = GETDATE() WHERE SECTOR_ID NOT IN ({idsToDelete}) AND INDICATOR_ID = {indicatorId} AND DELETED_AT IS NULL";
                        using (SqlCommand deleteCommand = new SqlCommand(deleteIdsNotInList, connection))
                        {
                            deleteCommand.ExecuteNonQuery();
                        }
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"Erro na inserção de setor: {ex.Message}");
                }
            }


           
            

            //Faz o terceiro update (GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR)
            string test = metric.Replace("#fator0", "10").Replace("#fator1", "5");
            DataTable dt = new DataTable();
            double resultado = 0;
            try
            {
                var result = dt.Compute(test, "").ToString();
                resultado = double.Parse(result);
            }
            catch (Exception)
            {
                return BadRequest("Operação matemática inválida.");
            }

            string updatemetric = $"UPDATE GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR SET DELETED_AT = GETDATE() WHERE INDICATORID = {indicatorId} AND DELETED_AT IS NULL";
            string insertmetric = $"INSERT INTO GDA_MATHEMATICAL_EXPRESSIONS (EXPRESSION, CREATED_AT) VALUES ('{metric}', GETDATE()); SELECT SCOPE_IDENTITY();";

            try
            {
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(updatemetric, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    using (SqlCommand command = new SqlCommand(insertmetric, connection))
                    {
                        int insertedId = Convert.ToInt32(command.ExecuteScalar());
                        string insertmetrichistory = $"INSERT INTO GDA_HISTORY_MATHEMATICAL_EXPRESSIONS_INDICATOR (mathematicalExpressionId, indicatorId, created_at) VALUES ('{insertedId}','{indicatorId}',GETDATE())";
                        using (SqlCommand command2 = new SqlCommand(insertmetrichistory, connection))
                        {
                            command2.ExecuteNonQuery();
                        }
                    }
                    connection.Close();
                }
            }
            catch(Exception ex)
            {
                return BadRequest($"Erro na inserção da métrica: {ex.Message}");
            }
            return Ok("Indicador atualizado com sucesso.");
        }
    }
}