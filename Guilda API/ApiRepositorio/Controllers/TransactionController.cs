using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ApiRepositorio.Class;
using ApiRepositorio.Models;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class TransactionController : ApiController
    {
        private RepositorioDBContext db = new RepositorioDBContext();

        // GET: api/Transaction
        public IQueryable<TransactionModel> GetTransactionModels()
        {
            return db.TransactionModels;
        }

        // GET: api/Transaction/5
        //[ResponseType(typeof(TransactionModel))]
        //public IHttpActionResult GetTransactionModel(int id)
        //{
        //    TransactionModel transactionModel = db.TransactionModels.Find(id);
        //    if (transactionModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(transactionModel);
        //}

        // PUT: api/Transaction/5
        //[ResponseType(typeof(void))]
        //public IHttpActionResult PutTransactionModel(int id, TransactionModel transactionModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != transactionModel.Id)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(transactionModel).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!TransactionModelExists(id))
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

        // POST: api/Transaction
        [ResponseType(typeof(TransactionModel))]
        public IHttpActionResult PostTransactionModel(TransactionModel transactionModel)
        {

            //Valida Token
            var tkn = TokenValidate.validate(Request.Headers);
            if (tkn == false)
            {
                return BadRequest("Invalid Token!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            transactionModel = new TransactionModel();


            transactionModel.transactionId = Convert.ToInt64(String.Format("{0:d16}", (DateTime.Now.Ticks / 10) % 10000000000000000)).ToString();
            transactionModel.created_at = DateTime.Now;
            transactionModel.complete = false;

            db.TransactionModels.Add(transactionModel);
            try
            {
                db.SaveChanges();
            }
            catch
            {

            }

            return Ok(transactionModel);
            //return CreatedAtRoute("DefaultApi", new { id = transactionModel.transactionId }, transactionModel);
        }

        //// DELETE: api/Transaction/5
        //[ResponseType(typeof(TransactionModel))]
        //public IHttpActionResult DeleteTransactionModel(int id)
        //{
        //    TransactionModel transactionModel = db.TransactionModels.Find(id);
        //    if (transactionModel == null)
        //    {
        //        return NotFound();
        //    }

        //    db.TransactionModels.Remove(transactionModel);
        //    db.SaveChanges();

        //    return Ok(transactionModel);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TransactionModelExists(int id)
        {
            return db.TransactionModels.Count(e => e.idgda_Transaction == id) > 0;
        }
    }
}