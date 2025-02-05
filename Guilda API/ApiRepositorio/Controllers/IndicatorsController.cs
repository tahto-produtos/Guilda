using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using ApiRepositorio.Class;
using ApiRepositorio.Models;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class IndicatorsController : ApiController
    {
        private RepositorioDBContext db = new RepositorioDBContext();

        // GET: api/Indicators
        //public IQueryable<IndicatorModel> GetIndicatorModels()
        //{
        //    return db.IndicatorModels;
        //}

        //// GET: api/Indicators/5
        //[ResponseType(typeof(IndicatorModel))]
        //public IHttpActionResult GetIndicatorModel(int id)
        //{
        //    IndicatorModel indicatorModel = db.IndicatorModels.Find(id);
        //    if (indicatorModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(indicatorModel);
        //}

        //// PUT: api/Indicators/5
        //[ResponseType(typeof(void))]
        //public IHttpActionResult PutIndicatorModel(int id, IndicatorModel indicatorModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != indicatorModel.idgda_Indicator)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(indicatorModel).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!IndicatorModelExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // POST: api/Indicators
        [ResponseType(typeof(IndicatorModel))]
        public IHttpActionResult PostIndicatorModel(long t_id, IEnumerable<IndicatorModel> indicatorModel)
        {
            bool validation = false;

            Log.insertLogTransaction(t_id.ToString(), "INDICATOR", "START", "");
            //Valida Token
            var tkn = TokenValidate.validate(Request.Headers);
            if (tkn == false)
            {
                Log.insertLogTransaction(t_id.ToString(), "INDICATOR", "Invalid Token!", "");
                return BadRequest("Invalid Token!");
            }

            if (!ModelState.IsValid)
            {
                Log.insertLogTransaction(t_id.ToString(), "INDICATOR", "Invalid Model!", "");
                return BadRequest(ModelState);
            }

            //Valida Transaction
            int? t = TransactionValidade.validate(Request.Headers, t_id);
            if (t is null)
            {
                Log.insertLogTransaction(t_id.ToString(), "INDICATOR", "Invalid Transaction!", "");
                return BadRequest("Invalid Transaction!");
            }


            try
            {
                db.Database.Connection.Open();
                db.Database.ExecuteSqlCommand(@"SET IDENTITY_INSERT [dbo].[GDA_INDICATOR] ON", "");
                foreach (IndicatorModel im in indicatorModel)
                {
                    //Valida se o indicador ja existe
                    var qtd = db.IndicatorTableModels.Where(p => p.idgda_Indicator == im.indicatorId).Count();
                    if (qtd > 0)
                    {
                        try
                        {
                            string query = $"UPDATE GDA_INDICATOR SET NAME = '{im.name}', DESCRIPTION = '{im.description}', TRANSACTIONID = {t} " +
                                       $"WHERE IDGDA_INDICATOR = {im.indicatorId} ";

                            SqlConnection connection = new SqlConnection(Database.Conn);
                            connection.Open();
                            SqlCommand createTableCommand2 = new SqlCommand(query.ToString(), connection);
                            createTableCommand2.ExecuteNonQuery();
                            connection.Close();

                            //db.SaveChanges();
                            validation = true;
                        }
                        catch (Exception ex)
                        {
                            Log.insertLogTransaction(t_id.ToString(), "INDICATOR", "ERRO: " + ex.Message.ToString(), "");
                        }

                    }
                    else
                    {
                        try
                        {
                            string query = "SET IDENTITY_INSERT [dbo].[GDA_INDICATOR] ON; INSERT INTO GDA_INDICATOR (IDGDA_INDICATOR, NAME, DESCRIPTION, NEWAPI, TRANSACTIONID) " +
                                           $"VALUES ({im.indicatorId}, '{im.name}', '{im.description}', 1, {t})";

                            SqlConnection connection = new SqlConnection(Database.Conn);
                            connection.Open();
                            SqlCommand createTableCommand2 = new SqlCommand(query.ToString(), connection);
                            createTableCommand2.ExecuteNonQuery();
                            connection.Close();

                            validation = true;
                        }
                        catch (Exception ex)
                        {
                            Log.insertLogTransaction(t_id.ToString(), "INDICATOR", "ERRO: " + ex.Message.ToString(), "");
                        }
                    }
                }
            }
            finally
            {
                db.Database.ExecuteSqlCommand(@"SET IDENTITY_INSERT [dbo].[GDA_INDICATOR] OFF");
                db.Database.Connection.Close();
            }

            Log.insertLogTransaction(t_id.ToString(), "INDICATOR", "CONCLUDED", "");

            if (validation == true)
            {
                return CreatedAtRoute("DefaultApi", new { id = indicatorModel.FirstOrDefault().indicatorId }, indicatorModel);
            }
            else
            {
                return BadRequest("No information entered.");
            }
        }

        // DELETE: api/Indicators/5
        //[ResponseType(typeof(IndicatorModel))]
        //public IHttpActionResult DeleteIndicatorModel(int id)
        //{
        //    IndicatorModel indicatorModel = db.IndicatorModels.Find(id);
        //    if (indicatorModel == null)
        //    {
        //        return NotFound();
        //    }

        //    db.IndicatorModels.Remove(indicatorModel);
        //    db.SaveChanges();

        //    return Ok(indicatorModel);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool IndicatorModelExists(int id)
        {
            return db.IndicatorTableModels.Count(e => e.idgda_Indicator == id) > 0;
        }
    }
}