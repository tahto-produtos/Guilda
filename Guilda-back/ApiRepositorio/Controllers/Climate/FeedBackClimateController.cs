using System.Data.SqlClient;
using System.Web.Http;
using System.Web.Http.Description;
using ApiRepositorio.Models;
using System.Web;
using System;
using System.Data;
using ApiC.Class;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using ApiC.Class.DowloadFile;
using static ApiRepositorio.Controllers.SendQuizController;
using System.Linq;
using static TokenService;

namespace ApiRepositorio.Controllers
{

    public class InputModelFeedBackClimate
    {
        public int idClimateUser { get; set; }
        public int idClimateApplyType { get; set; }
    }
    //[Authorize]
    public class FeedBackClimateController : ApiController
    {
        //Realiza um Post // Realizar Repost
        [HttpPost]
        //[ResponseType(typeof(ResultsModel))]
        public IHttpActionResult PostResultsModel([FromBody] InputModelFeedBackClimate inputModel)
        {
            int collaboratorId = 0;
            int personauserId = 0;
            var token = Request.Headers.Authorization?.Parameter;

            InfsUser inf = TokenService.TryDecodeToken(token);
            if (inf == null)
            {
                return Unauthorized();
            }
            collaboratorId = inf.collaboratorId;
            personauserId = inf.personauserId;

            BankDoClimateFeedBack.UpdateClimate(personauserId, inputModel.idClimateUser, inputModel.idClimateApplyType);

            return Ok("FeedBack aplicado com sucesso!");
        }
    }

    public class BankDoClimateFeedBack
    {
        public static void UpdateClimate(int createdBY, int idClimate,int idClimateApplyType)
        {
            StringBuilder update = new StringBuilder();
            update.Append($"UPDATE GDA_CLIMATE_USER SET IDGDA_CLIMATE_APPLY_TYPE = {idClimateApplyType}, FEEDBACK_BY = {createdBY} WHERE IDGDA_CLIMATE_USER = {idClimate}  ");

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(update.ToString(), connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }

        }
    }
}