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
using System.Drawing.Imaging;
using System.Net.NetworkInformation;
using System.Web.UI;
using System.Xml.Linq;
using CommandLine;
using DocumentFormat.OpenXml.Spreadsheet;
using static ApiRepositorio.Controllers.FinancialSummaryController;
using static ApiRepositorio.Controllers.SearchAccountsController;
using System.Runtime.ConstrainedExecution;
using ApiC.Class.DowloadFile;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class OrderProductConfirmController : ApiController
    {// POST: api/Results

        public class InputModelOrderProductConfirm
        {
            public int orderId { get; set; }
            public int productId { get; set; }
            public string receivedBy { get; set; }
            public string observationText { get; set; }
        }



        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModelOrderProductConfirm inputModel)
        {
            int collaboratorId = 0;
            int personauserId = 0;
            var token = Request.Headers.Authorization?.Parameter;

            string produtos = "";
            string nomeOperador = "";
            string bcOperador = "";

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            collaboratorId = inf.collaboratorId;
            personauserId = inf.personauserId;

            bcOperador = "BC" + collaboratorId;

            List<bankMktplace.ModelOrderProduct> Orders = new List<bankMktplace.ModelOrderProduct>();
            Orders = bankMktplace.consultOrder(inputModel.orderId);

            List<bankMktplace.ModelOrderProduct> lProds = new List<bankMktplace.ModelOrderProduct>();
            lProds = bankMktplace.consultOrderProduct(inputModel.orderId, inputModel.productId);

            nomeOperador = lProds.FirstOrDefault().nameCollaborator;

            foreach (bankMktplace.ModelOrderProduct item in lProds)
            {
                if (produtos != "")
                {
                    produtos = $"{produtos}, {item.comercialName}";
                }
                else
                {
                    produtos = $"{item.comercialName}";
                }
            }

            //lProds = lProds.FindAll(l => l.type == "RELEASED");

            //if (lProds.Count > 0)
            //{
            //    return BadRequest("Order not found!");
            //}
            int falha = 0;
            int inserido = 0;
            foreach (bankMktplace.ModelOrderProduct item in lProds)
            {
                if (item.type == "DIGITAL")
                {
                    List<bankMktplace.ModelVoucher> vchers = new List<bankMktplace.ModelVoucher>();
                    vchers = bankMktplace.consultVoucher(item.productId, item.stockId, item.amount);

                    foreach (bankMktplace.ModelVoucher vcher in vchers)
                    {
                        //Update Voucher
                        bankMktplace.updateVoucher(vcher.voucherId, "REDEEMED");

                        //Insert collaborator
                        bankMktplace.insertVoucherCollaborator(vcher.voucherId, item.orderById, inputModel.orderId);

                        inserido = 1;
                    }
                }
                else
                {
                    //Select Product Item
                    List<bankMktplace.ModelItem> its = new List<bankMktplace.ModelItem>();
                    its = bankMktplace.consultItens(item.productId, item.amount);

                    //Update Product Item
                    foreach (bankMktplace.ModelItem itn in its)
                    {
                        bankMktplace.updateItem(itn.itemId, "REDEEMED");
                    }

                    inserido = 1;
                }

                //Update GdaOrderProduct
                if (inserido == 1)
                {
                    bankMktplace.updateOrderProduct(inputModel.orderId, item.productId, inputModel.receivedBy, collaboratorId, inputModel.observationText);
                }
                else
                {
                    falha = 1;
                }
                
            }

            if (falha == 1)
            {
                return BadRequest("Não foi possivel concluir!");
            }

            //Update GdaOrder
            int qtdProducts = Orders.Count();

            int qtdProductsLibered = Orders.FindAll(t => t.status != "RELEASED" && t.status != "ORDERED").ToList().Count();
            qtdProductsLibered += 1;

            if (qtdProducts == qtdProductsLibered)
            {
                bankMktplace.updateOrder(inputModel.orderId, collaboratorId);
            }

            ScheduledNotification.insertNotificationMktPlace(12, "Pedido Concluido", $"O(s) produto(s): {produtos} foi concluido. Pedido realizado por: {nomeOperador} - {bcOperador}.!", true, personauserId, true, collaboratorId);

            return Ok("Ok");
        }

    }
}