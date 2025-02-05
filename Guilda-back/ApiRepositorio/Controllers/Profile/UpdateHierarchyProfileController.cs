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
using System.Drawing.Imaging;
using System.Windows.Media.Converters;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    //[Authorize]
    public class UpdateHierarchyProfileController : ApiController
    {
        public class InputModel
        {
            public int IDGDA_PROFILE { get; set; }
            public int BASICAPROFILEHIERARCHY { get; set; } 
            public bool CONFIRM {  get; set; }  
        }

        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] InputModel inputModel)
        {
            string PERFIL = "";
            string IDPERFIL = "";
            string MENSAGEM = "";
            int IDGDA_PROFILE = inputModel.IDGDA_PROFILE;
            int BASICAPROFILEHIERARCHY = inputModel.BASICAPROFILEHIERARCHY;
            bool CONFIRM = inputModel.CONFIRM;

            //SELECT PARA VERIFICAR SE JA TEMOS CRIADO UM PERFIL COM ESSA HIERARCHY
            StringBuilder stb = new StringBuilder();
            stb.Append("SELECT ID, NAME, BASIC_PROFILE_IDHIERARCHY FROM GDA_PROFILE_COLLABORATOR_ADMINISTRATION  NOLOCK  ");
            stb.Append("WHERE DELETED_AT IS NULL ");
            //stb.AppendFormat("AND ID = {0} ", IDGDA_PROFILE);
            stb.AppendFormat("AND BASIC_PROFILE_IDHIERARCHY ={0} ", BASICAPROFILEHIERARCHY);

            using (SqlConnection connection = new SqlConnection(Database.retornaConn()))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(stb.ToString(), connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            IDPERFIL = reader["ID"].ToString();
                            PERFIL = reader["NAME"].ToString();
                        }
                    }
                    //JA TEMOS UM PERFIL CRIADO NA BASE COM ESSA CONFIGURAÇÃO,
                    //CONFIRMAÇÃO PARA ALTERAR FOI ENVIADA COMO FALSE.
                    if (CONFIRM == false && PERFIL != "")
                    {
                        MENSAGEM = $"Ja existe um perfil com essa configuração:( {PERFIL} ) Você deseja alterar mesmo assim?";
                    }
                    //JA TEMOS UM PERFIL CRIADO NA BASE COM ESSA CONFIGURAÇÃO,
                    //CONFIRMAÇÃO PARA ALTERAR FOI ENVIADA COMO TRUE,
                    //UPDATE NO PERFIL QUE JA EXITES PRA 0 E ATUALIZA O NOVO PERFIL PARA CONFIGURAÇÃO NOVA.
                    else if (CONFIRM == true && PERFIL != "")
                    {
                        StringBuilder stb2 = new StringBuilder();
                        stb2.Append($"UPDATE GDA_PROFILE_COLLABORATOR_ADMINISTRATION SET BASIC_PROFILE_IDHIERARCHY = 0 WHERE ID ={IDPERFIL} ; ");
                        stb2.Append($" UPDATE GDA_PROFILE_COLLABORATOR_ADMINISTRATION SET BASIC_PROFILE_IDHIERARCHY = {BASICAPROFILEHIERARCHY} WHERE ID ={IDGDA_PROFILE}");
                        
                        using (SqlCommand command2 = new SqlCommand(stb2.ToString(), connection))
                        {
                            command2.ExecuteNonQuery();
                        }
                        MENSAGEM = $"Perfil atualizado com sucesso.";
                    }
                    //NÃO TEMOS UM PERFIL CRIADO NA BASE COM ESSA CONFIGURAÇÃO,
                    //UPDATE NO PERFIL E ATUALIZA O PERFIL PARA CONFIGURAÇÃO NOVA.
                    else if (CONFIRM == true)
                    {
                        StringBuilder stb3 = new StringBuilder();
                        stb3.Append($" UPDATE GDA_PROFILE_COLLABORATOR_ADMINISTRATION SET BASIC_PROFILE_IDHIERARCHY = {BASICAPROFILEHIERARCHY} WHERE ID ={IDGDA_PROFILE}");
                        using (SqlCommand command3 = new SqlCommand(stb3.ToString(), connection))
                        {
                            command3.ExecuteNonQuery();
                        }
                        MENSAGEM = $"Perfil atualizado com sucesso.";
                    }
                    else if (CONFIRM == false)
                    {
                        StringBuilder stb4 = new StringBuilder();
                        stb4.Append($" UPDATE GDA_PROFILE_COLLABORATOR_ADMINISTRATION SET BASIC_PROFILE_IDHIERARCHY = {BASICAPROFILEHIERARCHY} WHERE ID ={IDGDA_PROFILE}");
                        using (SqlCommand command4 = new SqlCommand(stb4.ToString(), connection))
                        {
                            command4.ExecuteNonQuery();
                        }
                        MENSAGEM = $"Perfil atualizado com sucesso.";
                    }
                    connection.Close();
                }
            }
            return Ok(MENSAGEM);
        }
    }
}