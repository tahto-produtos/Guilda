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
using System.ComponentModel.DataAnnotations;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class UpdatePorfileController : ApiController
    {
        public class InputModel
        {
            public int ID { get; set; }
            public string NAME { get; set; }
            public string AGE { get; set; }
            public string IDCITY { get; set; }
            public string WHOARE { get; set; }
            public string YOURMOTIVATIONS { get; set; }
            public string GOALS { get; set; }
            public string NEEDS { get; set; }
            public string DIFFICULTIES { get; set; }
            public string IDSTATUS { get; set; }
            public int CODCOLLABORATOR { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {
            int validatorCollaborator = 0;

           
            string NAME = inputModel.NAME;
            string AGE = inputModel.AGE;
            string IDCITY = inputModel.IDCITY;
            string WHOARE = inputModel.WHOARE;
            string YOURMOTIVATIONS = inputModel.YOURMOTIVATIONS;
            string GOALS = inputModel.GOALS;
            string NEEDS = inputModel.NEEDS;
            string DIFFICULTIES = inputModel.DIFFICULTIES;
            string IDSTATUS = inputModel.IDSTATUS;
            int CODCOLLABORATOR = inputModel.CODCOLLABORATOR;
            //Faz o primeiro select para verificar se ja existe uma inserção na base
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT IDGDA_COLLABORATORS FROM GDA_PERSONA_DETAILS NOLOCK WHERE IDGDA_COLLABORATORS= {0}", CODCOLLABORATOR);

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
                                validatorCollaborator = +1;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                connection.Close();
            }
            //Caso colaborador ja exista, faça update, se não faça um insert
            string insert = "";
            string update = "";
            if (validatorCollaborator == 0)
            {
                insert = $"--INSERT INTO GDA_PERSONA_DETAILS (NAME,AGE,IDCITY,WHOARE,YOURMOTIVATIONS,GOALS,NEEDS,DIFFICULTIES,IDSTATUS,IDGDA_COLLABORATORS) VALUES ('{NAME}', {AGE}, {IDCITY}, '{WHOARE}', '{YOURMOTIVATIONS}', '{GOALS}', '{NEEDS}', '{DIFFICULTIES}', {IDSTATUS}, {CODCOLLABORATOR})";
            }
            else
            {
                update = $"UPDATE GDA_PERSONA_DETAILS SET NAME ='{NAME}', AGE='{AGE}', IDCITY ={IDCITY}, WHOARE= '{WHOARE}', YOURMOTIVATIONS='{YOURMOTIVATIONS}', GOALS='{GOALS}', NEEDS='{NEEDS}', DIFFICULTIES='{DIFFICULTIES}', IDSTATUS='{IDSTATUS}' WHERE IDGDA_COLLABORATORS = {CODCOLLABORATOR}";
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand((insert != "") ? insert : update, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na atualização do colaborador: {ex.Message}");
            }
            return Ok("Configuração atualizada com sucesso.");
        }
    }
}