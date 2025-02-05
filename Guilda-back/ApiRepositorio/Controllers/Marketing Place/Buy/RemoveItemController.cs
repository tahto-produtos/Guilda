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
using ApiC.Class.DowloadFile;
using static ApiRepositorio.Controllers.ReleasedItemController;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class RemoveItemController : ApiController
    {// POST: api/Results

        public class InputModel
        {
            public int codOrder { get; set; }
            public int codProduct { get; set; }
        }


        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
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

            int pause = BancoRemoveItem.getDuePause();

            List<releasedItemModel> bcc = new List<releasedItemModel>();
            bcc = BancoReleasedOrder.returnBbcOrder(inputModel.codOrder, inputModel.codProduct.ToString());
            bcOperador = "BC" + bcc.FirstOrDefault().idCollaborator;
            nomeOperador = bcc.FirstOrDefault().nameColaborator;

            foreach (releasedItemModel item in bcc)
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

            ScheduledNotification.insertNotificationMktPlace(12, "Pedido Cancelado", $"O pedido do {nomeOperador} - {bcOperador} foi cancelado. Produtos: {produtos}!", true, personauserId, true, collaboratorId);

            if (pause == 0)
            {
                BancoRemoveItem.calculateValidated(inputModel.codOrder);


                return Ok("Ok");
            }
            else
            {
                return Ok("Configuração de expiração esta pausada!");
            }


        }

        public class BancoRemoveItem
        {

            public static int getDuePause()
            {
                int pause = 0;

                using (SqlConnection connection = new SqlConnection(Database.Conn))
                {
                    connection.Open();
                    try
                    {
                        StringBuilder stb = new StringBuilder();
                        stb.Append("SELECT PAUSE, REPROCESSED FROM GDA_MONETIZATION_CONFIG_PAUSE (NOLOCK) ");
                        stb.Append("WHERE DELETED_AT IS NULL AND REPROCESSED = 0 ");
                        stb.Append("ORDER BY 1 DESC ");

                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    pause = Convert.ToInt32(reader["PAUSE"].ToString());
                                }
                            }
                        }

                    }
                    catch (Exception)
                    {

                    }


                    connection.Close();

                }
                return pause;
            }

            public static bool calculateValidated(int idOrder)
            {
                bool retorno = false;

                //

                StringBuilder stb = new StringBuilder();
                stb.Append($"DECLARE @DATAINICIAL DATETIME; SET @DATAINICIAL = CONVERT(DATE, DATEADD(DAY, -1, GETDATE())); ");
                stb.Append($"UPDATE CK SET ");
                stb.Append($"DUE_AT = CONVERT(DATE, DATEADD(DAY, CKD.DIAS, GETDATE())), ");
                stb.Append($"IDGDA_MONETIZATION_CONFIG =  CKD.CONFIG ");
                stb.Append($"FROM GDA_CHECKING_ACCOUNT CK ");
                stb.Append($"LEFT JOIN  ");
                stb.Append($"(  ");
                stb.Append($"    SELECT  ");
                stb.Append($"        IDGDA_COLLABORATORS,  ");
                stb.Append($"        ACTIVE,  ");
                stb.Append($"        MATRICULA_SUPERVISOR,  ");
                stb.Append($"        MATRICULA_COORDENADOR,  ");
                stb.Append($"        NOME_SUPERVISOR,  ");
                stb.Append($"        NOME_COORDENADOR,  ");
                stb.Append($"        MATRICULA_GERENTE_II,  ");
                stb.Append($"        NOME_GERENTE_II, ");
                stb.Append($"        MATRICULA_GERENTE_I,  ");
                stb.Append($"        NOME_GERENTE_I,  ");
                stb.Append($"        MATRICULA_DIRETOR,  ");
                stb.Append($"        NOME_DIRETOR,  ");
                stb.Append($"        MATRICULA_CEO,  ");
                stb.Append($"        NOME_CEO,  ");
                stb.Append($"        PERIODO,  ");
                stb.Append($"        Cargo, ");
                stb.Append($"        CASE WHEN IDGDA_SUBSECTOR IS NOT NULL THEN IDGDA_SUBSECTOR ELSE IDGDA_SECTOR END AS IDGDA_SECTOR,  ");
                stb.Append($"        COALESCE(MG2.IDGDA_MONETIZATION_CONFIG, MG1.IDGDA_MONETIZATION_CONFIG) AS CONFIG, ");
                stb.Append($"        COALESCE(MG2.DAYS, MG1.DAYS) AS DIAS ");
                stb.Append($"    FROM GDA_COLLABORATORS_DETAILS (NOLOCK) CL  ");
                stb.Append($"    LEFT JOIN GDA_SITE (NOLOCK) AS SS ON SS.SITE = CL.SITE  ");
                stb.Append($"    LEFT JOIN GDA_MONETIZATION_CONFIG (NOLOCK) AS MG1 ON MG1.DELETED_AT IS NULL  ");
                stb.Append($"	 AND MG1.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE = 2 AND MG1.ID_REFERER = SS.IDGDA_SITE  ");
                stb.Append($"    LEFT JOIN GDA_MONETIZATION_CONFIG (NOLOCK) AS MG2 ON MG2.DELETED_AT IS NULL  ");
                stb.Append($"	 AND MG2.IDGDA_MONETIZATION_EXPIRED_FILTER_TYPE = 1 AND MG2.ID_REFERER = CL.IDGDA_SECTOR  ");
                stb.Append($"    WHERE CL.CREATED_AT = @DATAINICIAL  ");
                stb.Append($") AS CKD ON CKD.IDGDA_COLLABORATORS = CK.COLLABORATOR_ID  ");
                stb.Append($"WHERE ");
                stb.Append($"    GDA_ORDER_IDGDA_ORDER = {idOrder} ");
                stb.Append($"    AND INPUT > 0; ");

                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    try
                    {
                        using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    connection.Close();
                }

                return retorno;
            }

            // Método para serializar um DataTable em JSON

        }
    }
}