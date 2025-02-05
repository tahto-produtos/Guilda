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
using DocumentFormat.OpenXml.Drawing.Charts;
using static ApiRepositorio.Controllers.BasketIndicatorSectorController;
using static TokenService;
//using BenchmarkDotNet.Exporters.Csv;

namespace ApiRepositorio.Controllers
{
    public class inputListGroupHierarchyClimate
    {
        public DateTime STARTEDATFROM { get; set; }
        public DateTime STARTEDATTO { get; set; }
        public List<int> PERSONASID { get; set; }
        public List<int> SECTORSID { get; set; }
        public List<int> GROUP { get; set; }
        public List<int> CATEGORY { get; set; }
        public List<int>  HIERARCHY { get; set; }
        public string PERIOD { get; set; }
        public string COLLABORATOR { get; set; }    
    }

    public class reasonsClimateGroup
    {
        public int id { get; set; }
        public string name { get; set; }
        public int count { get; set; }
        public double percent { get; set; }
        public string data { get; set; }
    }

    public class climateGroup
    {
        public int id { get; set; }
        public string name { get; set; }
        public int count { get; set; }
        public double percent { get; set; }
         public string data { get; set; }
    }

    public class returnListGroupByHierarchyColaboratorRtn
    {
        public List<returnListGroupByHierarchyColaborator> response { get; set; }
    }

    public class returnListGroupByHierarchyColaborator
    {
        public int id { get; set; }
        public string name { get; set; }
        public int idSupervisor { get; set; }
        public int idCoordenador { get; set; }
        public int idGerenteII { get; set; }
        public int idGerenteI { get; set; }
        public int idDiretor { get; set; }
        public int idCeo { get; set; }
        public List<climateGroup> climates { get; set; }
        public List<reasonsClimateGroup> reasons { get; set; }
        public List<returnListGroupByHierarchyColaborator> returnListGroupByHierarchyColaboratorChild { get; set; }
    }

    public class returnListsGroupHierarchyClimate
    {
        public int idCollaborator { get; set; }
        public int idUserClimate { get; set; }
        public string data { get; set; }
        public string name { get; set; }
        public string BC { get; set; }
        public int idClimate { get; set; }
        public int idReason { get; set; }
        public int idHierarchy { get; set; }
        public string climate { get; set; }
        public string reason { get; set; }
        public string applyType { get; set; }
        public int idSupervisor { get; set; }
        public string nomeSupervisor { get; set; }
        public int idCoordenador { get; set; }
        public string nomeCoordenador { get; set; }
        public int idGerenteII { get; set; }
        public string nomeGerenteII { get; set; }
        public int idGerenteI { get; set; }
        public string nomeGerenteI { get; set; }
        public int idDiretor { get; set; }
        public string nomeDiretor { get; set; }
        public int idCeo { get; set; }
        public string nomeCeo { get; set; }
    }

    //[Authorize]
    public class ListGroupHierarchyClimateController : ApiController
    {// POST: api/Results
        [HttpPost]
        public IHttpActionResult PostResultsModel([FromBody] inputListGroupHierarchyClimate inputModel)
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

            string dtInicial = inputModel.STARTEDATFROM.ToString("yyyy-MM-dd");
            string dtFinal = inputModel.STARTEDATTO.ToString("yyyy-MM-dd");

            string personasAsString = string.Join(",", inputModel.PERSONASID);
            string sectorsAsString = string.Join(",", inputModel.SECTORSID);

            string groupAsString = string.Join(",", inputModel.GROUP);
            string categoryAsString = string.Join(",", inputModel.CATEGORY);
            string hierarchyAsString = string.Join(",", inputModel.HIERARCHY);
            string collaboratorAsString = string.Join(",", inputModel.COLLABORATOR);
            
            //GroupingPeriod period = GroupingPeriod.Day;
            //if (inputModel.PERIOD == "mensal")
            //{
            //    period = GroupingPeriod.Month;
            //}
            //else if (inputModel.PERIOD == "diario")
            //{
            //    period = GroupingPeriod.Day;
            //}
            //else if (inputModel.PERIOD == "quinzenal")
            //{
            //    period = GroupingPeriod.Biweekly;
            //}
            //else if (inputModel.PERIOD == "semanal")
            //{
            //    period = GroupingPeriod.Week;
            //}

            string filtro = "";
            string filtroSetor = "";

            filtro = personasAsString != "" ? $"{filtro} AND PU.IDGDA_PERSONA_USER IN ({personasAsString}) " : filtro;
            filtro = groupAsString != "" ? $"{filtro} AND CD.GRUPO IN ({groupAsString}) " : filtro;
            filtro = categoryAsString != "" ? $"{filtro} AND C.IDGDA_CLIMATE  IN ({categoryAsString}) " : filtro;
            filtro = hierarchyAsString != "" ? $"{filtro} AND H.IDGDA_HIERARCHY IN ({hierarchyAsString}) " : filtro;
            filtro = hierarchyAsString != "" ? $"{filtro} AND H.IDGDA_HIERARCHY IN ({hierarchyAsString}) " : filtro;
            filtro = collaboratorAsString != "" ? $"{filtro} AND (PU.NAME LIKE '%'+@searchAccount+'%' OR PU.IDGDA_PERSONA_USER = TRY_CAST(@searchAccount AS INT) OR CD.IDGDA_COLLABORATORS = TRY_CAST(@searchAccount AS INT) ) " : filtro;
            filtroSetor = sectorsAsString != "" ? $"{filtroSetor} AND CDI2.IDGDA_SECTOR IN ({sectorsAsString}) " : filtroSetor;

            //filtro = $"{filtro} AND C.CLIMATE IS NOT NULL ";

            List<returnListsGroupHierarchyClimate> rtn = new List<returnListsGroupHierarchyClimate>();
            rtn = BankListGroupHierarchyClimate.returnListGroupHierarchy(COLLABORATORID, dtInicial, dtFinal, filtroSetor, filtro, collaboratorAsString, "");

            //Agrupar por ceo
            List<returnListGroupByHierarchyColaborator> agente = new List<returnListGroupByHierarchyColaborator>();
            List<returnListGroupByHierarchyColaborator> supervisor = new List<returnListGroupByHierarchyColaborator>();
            List<returnListGroupByHierarchyColaborator> coordenador = new List<returnListGroupByHierarchyColaborator>();
            List<returnListGroupByHierarchyColaborator> gerenteII = new List<returnListGroupByHierarchyColaborator>();
            List<returnListGroupByHierarchyColaborator> gerenteI = new List<returnListGroupByHierarchyColaborator>();
            List<returnListGroupByHierarchyColaborator> diretor = new List<returnListGroupByHierarchyColaborator>();
            List<returnListGroupByHierarchyColaborator> ceo = new List<returnListGroupByHierarchyColaborator>();

            agente = returnListHierarchyClimate(rtn, "AGENTE");
            supervisor = returnListHierarchyClimate(rtn, "SUPERVISOR");
            coordenador = returnListHierarchyClimate(rtn, "COORDENADOR");
            gerenteII = returnListHierarchyClimate(rtn, "GERENTE II");
            gerenteI = returnListHierarchyClimate(rtn, "GERENTE I");
            diretor = returnListHierarchyClimate(rtn, "DIRETOR");
            ceo = returnListHierarchyClimate(rtn, "CEO");

            #region Carrega Hierarquia

            SetChildItems<returnListGroupByHierarchyColaborator>(supervisor, agente, "SUPERVISOR", out supervisor);
            SetChildItems<returnListGroupByHierarchyColaborator>(coordenador, agente, "COORDENADOR", out coordenador);
            SetChildItems<returnListGroupByHierarchyColaborator>(gerenteII, agente, "GERENTE II", out gerenteII);
            SetChildItems<returnListGroupByHierarchyColaborator>(gerenteI, agente, "GERENTE I", out gerenteI);
            SetChildItems<returnListGroupByHierarchyColaborator>(diretor, agente, "DIRETOR", out diretor);
            SetChildItems<returnListGroupByHierarchyColaborator>(ceo, agente, "CEO", out ceo);

            SetChildItems<returnListGroupByHierarchyColaborator>(supervisor, supervisor, "SUPERVISOR", out supervisor);
            SetChildItems<returnListGroupByHierarchyColaborator>(coordenador, supervisor, "COORDENADOR", out coordenador);
            SetChildItems<returnListGroupByHierarchyColaborator>(gerenteII, supervisor, "GERENTE II", out gerenteII);
            SetChildItems<returnListGroupByHierarchyColaborator>(gerenteI, supervisor, "GERENTE I", out gerenteI);
            SetChildItems<returnListGroupByHierarchyColaborator>(diretor, supervisor, "DIRETOR", out diretor);
            SetChildItems<returnListGroupByHierarchyColaborator>(ceo, supervisor, "CEO", out ceo);

            SetChildItems<returnListGroupByHierarchyColaborator>(supervisor, coordenador, "SUPERVISOR", out supervisor);
            SetChildItems<returnListGroupByHierarchyColaborator>(coordenador, coordenador, "COORDENADOR", out coordenador);
            SetChildItems<returnListGroupByHierarchyColaborator>(gerenteII, coordenador, "GERENTE II", out gerenteII);
            SetChildItems<returnListGroupByHierarchyColaborator>(gerenteI, coordenador, "GERENTE I", out gerenteI);
            SetChildItems<returnListGroupByHierarchyColaborator>(diretor, coordenador, "DIRETOR", out diretor);
            SetChildItems<returnListGroupByHierarchyColaborator>(ceo, coordenador, "CEO", out ceo);

            SetChildItems<returnListGroupByHierarchyColaborator>(supervisor, gerenteII, "SUPERVISOR", out supervisor);
            SetChildItems<returnListGroupByHierarchyColaborator>(coordenador, gerenteII, "COORDENADOR", out coordenador);
            SetChildItems<returnListGroupByHierarchyColaborator>(gerenteII, gerenteII, "GERENTE II", out gerenteII);
            SetChildItems<returnListGroupByHierarchyColaborator>(gerenteI, gerenteII, "GERENTE I", out gerenteI);
            SetChildItems<returnListGroupByHierarchyColaborator>(diretor, gerenteII, "DIRETOR", out diretor);
            SetChildItems<returnListGroupByHierarchyColaborator>(ceo, gerenteII, "CEO", out ceo);

            SetChildItems<returnListGroupByHierarchyColaborator>(supervisor, gerenteI, "SUPERVISOR", out supervisor);
            SetChildItems<returnListGroupByHierarchyColaborator>(coordenador, gerenteI, "COORDENADOR", out coordenador);
            SetChildItems<returnListGroupByHierarchyColaborator>(gerenteII, gerenteI, "GERENTE II", out gerenteII);
            SetChildItems<returnListGroupByHierarchyColaborator>(gerenteI, gerenteI, "GERENTE I", out gerenteI);
            SetChildItems<returnListGroupByHierarchyColaborator>(diretor, gerenteI, "DIRETOR", out diretor);
            SetChildItems<returnListGroupByHierarchyColaborator>(ceo, gerenteI, "CEO", out ceo);

            SetChildItems<returnListGroupByHierarchyColaborator>(supervisor, diretor, "SUPERVISOR", out supervisor);
            SetChildItems<returnListGroupByHierarchyColaborator>(coordenador, diretor, "COORDENADOR", out coordenador);
            SetChildItems<returnListGroupByHierarchyColaborator>(gerenteII, diretor, "GERENTE II", out gerenteII);
            SetChildItems<returnListGroupByHierarchyColaborator>(gerenteI, diretor, "GERENTE I", out gerenteI);
            SetChildItems<returnListGroupByHierarchyColaborator>(diretor, diretor, "DIRETOR", out diretor);
            SetChildItems<returnListGroupByHierarchyColaborator>(ceo, diretor, "CEO", out ceo);

            SetChildItems<returnListGroupByHierarchyColaborator>(supervisor, ceo, "SUPERVISOR", out supervisor);
            SetChildItems<returnListGroupByHierarchyColaborator>(coordenador, ceo, "COORDENADOR", out coordenador);
            SetChildItems<returnListGroupByHierarchyColaborator>(gerenteII, ceo, "GERENTE II", out gerenteII);
            SetChildItems<returnListGroupByHierarchyColaborator>(gerenteI, ceo, "GERENTE I", out gerenteI);
            SetChildItems<returnListGroupByHierarchyColaborator>(diretor, ceo, "DIRETOR", out diretor);
            SetChildItems<returnListGroupByHierarchyColaborator>(ceo, ceo, "CEO", out ceo);

            #endregion

            #region Comentado
            //supervisor.ForEach(item =>
            //{

            //    item.returnListGroupByHierarchyColaboratorChild.AddRange(agente.Where(d => d != null && d.idSupervisor == item.id).ToList());
            //});

            //coordenador.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = agente.Where(d => d.idCoordenador == item.id && d.idSupervisor == 0).ToList();
            //});

            //gerenteII.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = agente.Where(d => d.idGerenteII == item.id && d.idSupervisor == 0 && d.idCoordenador == 0).ToList();
            //});

            //gerenteI.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = agente.Where(d => d.idGerenteI == item.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0).ToList();
            //});

            //diretor.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = agente.Where(d => d.idDiretor == item.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0).ToList();
            //});

            //ceo.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = agente.Where(d => d.idCeo == item.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0 && d.idDiretor == 0).ToList();
            //});

            //supervisor.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = supervisor.Where(d => d.idSupervisor == item.id).ToList();
            //});

            //coordenador.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = supervisor.Where(d => d.idCoordenador == item.id && d.idSupervisor == 0).ToList();
            //});

            //gerenteII.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = supervisor.Where(d => d.idGerenteII == item.id && d.idSupervisor == 0 && d.idCoordenador == 0).ToList();
            //});

            //gerenteI.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = supervisor.Where(d => d.idGerenteI == item.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0).ToList();
            //});

            //diretor.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = supervisor.Where(d => d.idDiretor == item.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0).ToList();
            //});

            //ceo.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = supervisor.Where(d => d.idCeo == item.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0 && d.idDiretor == 0).ToList();
            //});

            //supervisor.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = coordenador.Where(d => d.idSupervisor == item.id).ToList();
            //});

            //coordenador.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = coordenador.Where(d => d.idCoordenador == item.id && d.idSupervisor == 0).ToList();
            //});

            //gerenteII.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = coordenador.Where(d => d.idGerenteII == item.id && d.idSupervisor == 0 && d.idCoordenador == 0).ToList();
            //});

            //gerenteI.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = coordenador.Where(d => d.idGerenteI == item.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0).ToList();
            //});

            //diretor.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = coordenador.Where(d => d.idDiretor == item.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0).ToList();
            //});

            //ceo.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = coordenador.Where(d => d.idCeo == item.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0 && d.idDiretor == 0).ToList();
            //});

            //supervisor.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = gerenteII.Where(d => d.idSupervisor == item.id).ToList();
            //});

            //coordenador.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = gerenteII.Where(d => d.idCoordenador == item.id && d.idSupervisor == 0).ToList();
            //});

            //gerenteII.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = gerenteII.Where(d => d.idGerenteII == item.id && d.idSupervisor == 0 && d.idCoordenador == 0).ToList();
            //});

            //gerenteI.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = gerenteII.Where(d => d.idGerenteI == item.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0).ToList();
            //});

            //diretor.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = gerenteII.Where(d => d.idDiretor == item.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0).ToList();
            //});

            //ceo.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = gerenteII.Where(d => d.idCeo == item.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0 && d.idDiretor == 0).ToList();
            //});

            //supervisor.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = gerenteI.Where(d => d.idSupervisor == item.id).ToList();
            //});

            //coordenador.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = gerenteI.Where(d => d.idCoordenador == item.id && d.idSupervisor == 0).ToList();
            //});

            //gerenteII.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = gerenteI.Where(d => d.idGerenteII == item.id && d.idSupervisor == 0 && d.idCoordenador == 0).ToList();
            //});

            //gerenteI.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = gerenteI.Where(d => d.idGerenteI == item.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0).ToList();
            //});

            //diretor.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = gerenteI.Where(d => d.idDiretor == item.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0).ToList();
            //});

            //ceo.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = gerenteI.Where(d => d.idCeo == item.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0 && d.idDiretor == 0).ToList();
            //});

            //supervisor.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = diretor.Where(d => d.idSupervisor == item.id).ToList();
            //});

            //coordenador.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = diretor.Where(d => d.idCoordenador == item.id && d.idSupervisor == 0).ToList();
            //});

            //gerenteII.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = diretor.Where(d => d.idGerenteII == item.id && d.idSupervisor == 0 && d.idCoordenador == 0).ToList();
            //});

            //gerenteI.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = diretor.Where(d => d.idGerenteI == item.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0).ToList();
            //});

            //diretor.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = diretor.Where(d => d.idDiretor == item.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0).ToList();
            //});

            //ceo.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = diretor.Where(d => d.idCeo == item.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0 && d.idDiretor == 0).ToList();
            //});

            //supervisor.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = ceo.Where(d => d.idSupervisor == item.id).ToList();
            //});

            //coordenador.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = ceo.Where(d => d.idCoordenador == item.id && d.idSupervisor == 0).ToList();
            //});

            //gerenteII.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = ceo.Where(d => d.idGerenteII == item.id && d.idSupervisor == 0 && d.idCoordenador == 0).ToList();
            //});

            //gerenteI.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = ceo.Where(d => d.idGerenteI == item.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0).ToList();
            //});

            //diretor.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = ceo.Where(d => d.idDiretor == item.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0).ToList();
            //});

            //ceo.ForEach(item =>
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = ceo.Where(d => d.idCeo == item.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0 && d.idDiretor == 0).ToList();
            //});

            //SetChildItems<returnListGroupByHierarchyColaborator>(supervisor, d => d.idSupervisor == d.id, agente, out supervisor);
            //SetChildItems<returnListGroupByHierarchyColaborator>(coordenador, d => d.idCoordenador == d.id && d.idSupervisor == 0, agente, out coordenador);
            //SetChildItems<returnListGroupByHierarchyColaborator>(gerenteII, d => d.idGerenteII == d.id && d.idSupervisor == 0 && d.idCoordenador == 0, agente, out gerenteII);
            //SetChildItems<returnListGroupByHierarchyColaborator>(gerenteI, d => d.idGerenteI == d.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0, agente, out gerenteI);
            //SetChildItems<returnListGroupByHierarchyColaborator>(diretor, d => d.idDiretor == d.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0, agente, out diretor);
            //SetChildItems<returnListGroupByHierarchyColaborator>(ceo, d => d.idCeo == d.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0 && d.idDiretor == 0, agente, out ceo);

            //SetChildItems<returnListGroupByHierarchyColaborator>(supervisor, d => d.idSupervisor == d.id, supervisor, out supervisor);
            //SetChildItems<returnListGroupByHierarchyColaborator>(coordenador, d => d.idCoordenador == d.id && d.idSupervisor == 0, supervisor, out coordenador);
            //SetChildItems<returnListGroupByHierarchyColaborator>(gerenteII, d => d.idGerenteII == d.id && d.idSupervisor == 0 && d.idCoordenador == 0, supervisor, out gerenteII);
            //SetChildItems<returnListGroupByHierarchyColaborator>(gerenteI, d => d.idGerenteI == d.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0, supervisor, out gerenteI);
            //SetChildItems<returnListGroupByHierarchyColaborator>(diretor, d => d.idDiretor == d.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0, supervisor, out diretor);
            //SetChildItems<returnListGroupByHierarchyColaborator>(ceo, d => d.idCeo == d.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0 && d.idDiretor == 0, supervisor, out ceo);

            //SetChildItems<returnListGroupByHierarchyColaborator>(supervisor, d => d.idSupervisor == d.id, coordenador, out supervisor);
            //SetChildItems<returnListGroupByHierarchyColaborator>(coordenador, d => d.idCoordenador == d.id && d.idSupervisor == 0, coordenador, out coordenador);
            //SetChildItems<returnListGroupByHierarchyColaborator>(gerenteII, d => d.idGerenteII == d.id && d.idSupervisor == 0 && d.idCoordenador == 0, coordenador, out gerenteII);
            //SetChildItems<returnListGroupByHierarchyColaborator>(gerenteI, d => d.idGerenteI == d.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0, coordenador, out gerenteI);
            //SetChildItems<returnListGroupByHierarchyColaborator>(diretor, d => d.idDiretor == d.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0, coordenador, out diretor);
            //SetChildItems<returnListGroupByHierarchyColaborator>(ceo, d => d.idCeo == d.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0 && d.idDiretor == 0, coordenador, out ceo);

            //SetChildItems<returnListGroupByHierarchyColaborator>(supervisor, d => d.idSupervisor == d.id, gerenteI, out supervisor);
            //SetChildItems<returnListGroupByHierarchyColaborator>(coordenador, d => d.idCoordenador == d.id && d.idSupervisor == 0, gerenteI, out coordenador);
            //SetChildItems<returnListGroupByHierarchyColaborator>(gerenteII, d => d.idGerenteII == d.id && d.idSupervisor == 0 && d.idCoordenador == 0, gerenteI, out gerenteII);
            //SetChildItems<returnListGroupByHierarchyColaborator>(gerenteI, d => d.idGerenteI == d.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0, gerenteI, out gerenteI);
            //SetChildItems<returnListGroupByHierarchyColaborator>(diretor, d => d.idDiretor == d.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0, gerenteI, out diretor);
            //SetChildItems<returnListGroupByHierarchyColaborator>(ceo, d => d.idCeo == d.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0 && d.idDiretor == 0, gerenteI, out ceo);

            //SetChildItems<returnListGroupByHierarchyColaborator>(supervisor, d => d.idSupervisor == d.id, gerenteII, out supervisor);
            //SetChildItems<returnListGroupByHierarchyColaborator>(coordenador, d => d.idCoordenador == d.id && d.idSupervisor == 0, gerenteII, out coordenador);
            //SetChildItems<returnListGroupByHierarchyColaborator>(gerenteII, d => d.idGerenteII == d.id && d.idSupervisor == 0 && d.idCoordenador == 0, gerenteII, out gerenteII);
            //SetChildItems<returnListGroupByHierarchyColaborator>(gerenteI, d => d.idGerenteI == d.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0, gerenteII, out gerenteI);
            //SetChildItems<returnListGroupByHierarchyColaborator>(diretor, d => d.idDiretor == d.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0, gerenteII, out diretor);
            //SetChildItems<returnListGroupByHierarchyColaborator>(ceo, d => d.idCeo == d.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0 && d.idDiretor == 0, gerenteII, out ceo);

            //SetChildItems<returnListGroupByHierarchyColaborator>(supervisor, d => d.idSupervisor == d.id, diretor, out supervisor);
            //SetChildItems<returnListGroupByHierarchyColaborator>(coordenador, d => d.idCoordenador == d.id && d.idSupervisor == 0, diretor, out coordenador);
            //SetChildItems<returnListGroupByHierarchyColaborator>(gerenteII, d => d.idGerenteII == d.id && d.idSupervisor == 0 && d.idCoordenador == 0, diretor, out gerenteII);
            //SetChildItems<returnListGroupByHierarchyColaborator>(gerenteI, d => d.idGerenteI == d.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0, diretor, out gerenteI);
            //SetChildItems<returnListGroupByHierarchyColaborator>(diretor, d => d.idDiretor == d.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0, diretor, out diretor);
            //SetChildItems<returnListGroupByHierarchyColaborator>(ceo, d => d.idCeo == d.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0 && d.idDiretor == 0, diretor, out ceo);

            //SetChildItems<returnListGroupByHierarchyColaborator>(supervisor, d => d.idSupervisor == d.id, ceo, out supervisor);
            //SetChildItems<returnListGroupByHierarchyColaborator>(coordenador, d => d.idCoordenador == d.id && d.idSupervisor == 0, ceo, out coordenador);
            //SetChildItems<returnListGroupByHierarchyColaborator>(gerenteII, d => d.idGerenteII == d.id && d.idSupervisor == 0 && d.idCoordenador == 0, ceo, out gerenteII);
            //SetChildItems<returnListGroupByHierarchyColaborator>(gerenteI, d => d.idGerenteI == d.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0, ceo, out gerenteI);
            //SetChildItems<returnListGroupByHierarchyColaborator>(diretor, d => d.idDiretor == d.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0, ceo, out diretor);
            //SetChildItems<returnListGroupByHierarchyColaborator>(ceo, d => d.idCeo == d.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0 && d.idDiretor == 0, ceo, out ceo);


            //foreach (returnListGroupByHierarchyColaborator item in supervisor)
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = agente.Where(d => d.idSupervisor == item.id).ToList(); ;
            //}
            //foreach (returnListGroupByHierarchyColaborator item in coordenador)
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = supervisor.Where(d => d.idCoordenador == item.id).ToList(); ;
            //}
            //foreach (returnListGroupByHierarchyColaborator item in gerenteII)
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = coordenador.Where(d => d.idGerenteII == item.id).ToList(); ;
            //}
            //foreach (returnListGroupByHierarchyColaborator item in gerenteI)
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = gerenteII.Where(d => d.idGerenteI == item.id).ToList(); ;
            //}
            //foreach (returnListGroupByHierarchyColaborator item in diretor)
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = gerenteI.Where(d => d.idDiretor == item.id).ToList(); ;
            //}
            //foreach (returnListGroupByHierarchyColaborator item in ceo)
            //{
            //    item.returnListGroupByHierarchyColaboratorChild = diretor.Where(d => d.idCeo == item.id).ToList(); ;
            //}
            #endregion

            returnListGroupByHierarchyColaboratorRtn retornoFinal = new returnListGroupByHierarchyColaboratorRtn();
            retornoFinal.response = new List<returnListGroupByHierarchyColaborator>();
            foreach (returnListGroupByHierarchyColaborator item in ceo)
            {

                retornoFinal.response.Add(item);
            }
            return Ok(retornoFinal);
        }
        // Método para serializar um DataTable em JSON




        public static void SetChildItems<T>(List<returnListGroupByHierarchyColaborator> itemList, List<returnListGroupByHierarchyColaborator> sourceList, string type, out List<returnListGroupByHierarchyColaborator> result)
        {

            if (type == "SUPERVISOR")
            {
                foreach (returnListGroupByHierarchyColaborator item in itemList)
                {
                    if (item.returnListGroupByHierarchyColaboratorChild == null)
                    {
                        item.returnListGroupByHierarchyColaboratorChild = new List<returnListGroupByHierarchyColaborator>();
                    }

                    List<returnListGroupByHierarchyColaborator> rtn = sourceList.Where(d => d.idSupervisor == item.id).ToList();
                    if (rtn.Count > 0)
                    {
                        item.returnListGroupByHierarchyColaboratorChild.AddRange(rtn);
                    }
                }
            }
            else if (type == "COORDENADOR")
            {
                foreach (returnListGroupByHierarchyColaborator item in itemList)
                {
                    if (item.returnListGroupByHierarchyColaboratorChild == null)
                    {
                        item.returnListGroupByHierarchyColaboratorChild = new List<returnListGroupByHierarchyColaborator>();
                    }

                    List<returnListGroupByHierarchyColaborator> rtn = sourceList.Where(d => d.idCoordenador == item.id && d.idSupervisor == 0).ToList();
                    if (rtn.Count > 0)
                    {
                        item.returnListGroupByHierarchyColaboratorChild.AddRange(rtn);
                    }
                }
            }
            else if (type == "GERENTE II")
            {
                foreach (returnListGroupByHierarchyColaborator item in itemList)
                {
                    if (item.returnListGroupByHierarchyColaboratorChild == null)
                    {
                        item.returnListGroupByHierarchyColaboratorChild = new List<returnListGroupByHierarchyColaborator>();
                    }

                    List<returnListGroupByHierarchyColaborator> rtn = sourceList.Where(d => d.idGerenteII == item.id && d.idSupervisor == 0 && d.idCoordenador == 0).ToList();
                    if (rtn.Count > 0)
                    {
                        item.returnListGroupByHierarchyColaboratorChild.AddRange(rtn);
                    }
                }
            }
            else if (type == "GERENTE I")
            {
                foreach (returnListGroupByHierarchyColaborator item in itemList)
                {
                    if (item.returnListGroupByHierarchyColaboratorChild == null)
                    {
                        item.returnListGroupByHierarchyColaboratorChild = new List<returnListGroupByHierarchyColaborator>();
                    }

                    List<returnListGroupByHierarchyColaborator> rtn = sourceList.Where(d => d.idGerenteI == item.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0).ToList();
                    if (rtn.Count > 0)
                    {
                        item.returnListGroupByHierarchyColaboratorChild.AddRange(rtn);
                    }
                }
            }
            else if (type == "DIRETOR")
            {
                foreach (returnListGroupByHierarchyColaborator item in itemList)
                {
                    if (item.returnListGroupByHierarchyColaboratorChild == null)
                    {
                        item.returnListGroupByHierarchyColaboratorChild = new List<returnListGroupByHierarchyColaborator>();
                    }

                    List<returnListGroupByHierarchyColaborator> rtn = sourceList.Where(d => d.idDiretor == item.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0).ToList();
                    if (rtn.Count > 0)
                    {
                        item.returnListGroupByHierarchyColaboratorChild.AddRange(rtn);
                    }
                }
            }
            else if (type == "CEO")
            {
                foreach (returnListGroupByHierarchyColaborator item in itemList)
                {
                    if (item.returnListGroupByHierarchyColaboratorChild == null)
                    {
                        item.returnListGroupByHierarchyColaboratorChild = new List<returnListGroupByHierarchyColaborator>();
                    }

                    List<returnListGroupByHierarchyColaborator> rtn = sourceList.Where(d => d.idCeo == item.id && d.idSupervisor == 0 && d.idCoordenador == 0 && d.idGerenteII == 0 && d.idGerenteI == 0 && d.idDiretor == 0).ToList();
                    if (rtn.Count > 0)
                    {
                        item.returnListGroupByHierarchyColaboratorChild.AddRange(rtn);
                    }
                }
            }


            result = itemList;

        }
        public static List<climateGroup> GetAllClimateGroups()
        {
            List<climateGroup> lcg = new List<climateGroup>();

            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT IDGDA_CLIMATE, CLIMATE FROM GDA_CLIMATE (NOLOCK) ");

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

                                climateGroup lc = new climateGroup();
                                lc.id = reader["IDGDA_CLIMATE"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_CLIMATE"]) : 0;
                                lc.name = reader["CLIMATE"] != DBNull.Value ? reader["CLIMATE"].ToString() : "";
                                lc.count = 0;
                                lc.percent = 0;
                                lcg.Add(lc);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Trate a exceção aqui
                }
                connection.Close();
            }

            return lcg;
        }
        public static List<reasonsClimateGroup> GetAllReasonsClimateGroups()
        {

            List<reasonsClimateGroup> lcg = new List<reasonsClimateGroup>();

            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT IDGDA_CLIMATE_REASON, REASON FROM GDA_CLIMATE_REASON (NOLOCK) ");

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
                                reasonsClimateGroup lc = new reasonsClimateGroup();
                                lc.id = reader["IDGDA_CLIMATE_REASON"] != DBNull.Value ? Convert.ToInt32(reader["IDGDA_CLIMATE_REASON"]) : 0;
                                lc.name = reader["REASON"] != DBNull.Value ? reader["REASON"].ToString() : "";
                                lc.count = 0;
                                lc.percent = 0;
                                lcg.Add(lc);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Trate a exceção aqui
                }
                connection.Close();
            }

            return lcg;
        }
        public static List<returnListGroupByHierarchyColaborator> returnListHierarchyClimate(List<returnListsGroupHierarchyClimate> original, string hierarchy)
        {
            List<returnListGroupByHierarchyColaborator> retorno = new List<returnListGroupByHierarchyColaborator>();
            try
            {

                List<climateGroup> allClimateGroups = GetAllClimateGroups();

                List<reasonsClimateGroup> allReasonsClimateGroups = GetAllReasonsClimateGroups();

        
                if (hierarchy == "AGENTE")
                {
                    retorno = original
                        .GroupBy(item => new { item.idCollaborator, item.idHierarchy }).Where(d => d.Key.idHierarchy == 1)
                       .Select(group => new returnListGroupByHierarchyColaborator
                       {
                           id = group.Key.idCollaborator,
                           idSupervisor = group.First().idSupervisor,
                           idCoordenador = group.First().idCoordenador,
                           idGerenteII = group.First().idGerenteII,
                           idGerenteI = group.First().idGerenteI,
                           idDiretor = group.First().idDiretor,
                           idCeo = group.First().idCeo,
                           name = group.First().name,
                           climates = allClimateGroups
                           .Select(climate => new climateGroup
                           {
                               id = climate.id,
                               name = climate.name,
                               count = group.Count(item => item.idClimate == climate.id),
                               percent = group.Count(item => item.idClimate != 0) > 0 ? Math.Round(((Convert.ToDouble(group.Count(item => item.idClimate == climate.id)) / Convert.ToDouble(group.Count(item => item.idClimate != 0)) * 100)), 2, MidpointRounding.AwayFromZero) : 0
                               //percent = group.Count(item => item.idClimate != 0) > 0 ? (group.Count(item => item.idClimate == climate.id) / group.Count(item => item.idClimate != 0)) * 100 : 0
                           })
                           .ToList(),
                           reasons = allReasonsClimateGroups
                           .Select(reason => new reasonsClimateGroup
                           {
                               id = reason.id,
                               name = reason.name,
                               count = group.Count(item => item.idReason == reason.id),
                               percent = group.Count(item => item.idReason != 0) > 0 ? Math.Round(((Convert.ToDouble(group.Count(item => item.idReason == reason.id)) / Convert.ToDouble(group.Count(item => item.idReason != 0)) * 100)), 2, MidpointRounding.AwayFromZero) : 0
                           })
                            .ToList()

                       }).ToList();
                }
                else if (hierarchy == "SUPERVISOR")
                {
                    retorno = original
                        .GroupBy(item => new { item.idSupervisor }).Where(d => d.Key.idSupervisor != 0)
                       .Select(group => new returnListGroupByHierarchyColaborator
                       {
                           id = group.Key.idSupervisor,
                           idSupervisor = 0,
                           idCoordenador = group.First().idCoordenador,
                           idGerenteII = group.First().idGerenteII,
                           idGerenteI = group.First().idGerenteI,
                           idDiretor = group.First().idDiretor,
                           idCeo = group.First().idCeo,
                           name = group.First().nomeSupervisor,
                           climates = allClimateGroups
                           .Select(climate => new climateGroup
                           {
                               id = climate.id,
                               name = climate.name,
                               count = group.Count(item => item.idClimate == climate.id),
                               percent = group.Count(item => item.idClimate != 0) > 0 ? Math.Round(((Convert.ToDouble(group.Count(item => item.idClimate == climate.id)) / Convert.ToDouble(group.Count(item => item.idClimate != 0)) * 100)), 2, MidpointRounding.AwayFromZero) : 0
                               //percent = group.Count(item => item.idClimate != 0) > 0 ? (group.Count(item => item.idClimate == climate.id) / group.Count(item => item.idClimate != 0)) * 100 : 0
                           })
                           .ToList(),
                           reasons = allReasonsClimateGroups
                           .Select(reason => new reasonsClimateGroup
                           {
                               id = reason.id,
                               name = reason.name,
                               count = group.Count(item => item.idReason == reason.id),
                               percent = group.Count(item => item.idReason != 0) > 0 ? Math.Round(((Convert.ToDouble(group.Count(item => item.idReason == reason.id)) / Convert.ToDouble(group.Count(item => item.idReason != 0)) * 100)), 2, MidpointRounding.AwayFromZero) : 0
                           })
                            .ToList()

                       }).ToList();
                }
                else if (hierarchy == "COORDENADOR")
                {
                    retorno = original
                    .GroupBy(item => new { item.idCoordenador }).Where(d => d.Key.idCoordenador != 0)
                    .Select(group => new returnListGroupByHierarchyColaborator
                    {
                        id = group.Key.idCoordenador,
                        idSupervisor = 0,
                        idCoordenador = 0,
                        idGerenteII = group.First().idGerenteII,
                        idGerenteI = group.First().idGerenteI,
                        idDiretor = group.First().idDiretor,
                        idCeo = group.First().idCeo,
                        name = group.First().nomeCoordenador,
                        climates = allClimateGroups
                           .Select(climate => new climateGroup
                           {
                               id = climate.id,
                               name = climate.name,
                               count = group.Count(item => item.idClimate == climate.id),
                               percent = group.Count(item => item.idClimate != 0) > 0 ? Math.Round(((Convert.ToDouble(group.Count(item => item.idClimate == climate.id)) / Convert.ToDouble(group.Count(item => item.idClimate != 0)) * 100)), 2, MidpointRounding.AwayFromZero) : 0
                               //percent = group.Count(item => item.idClimate != 0) > 0 ? (group.Count(item => item.idClimate == climate.id) / group.Count(item => item.idClimate != 0)) * 100 : 0
                           })
                           .ToList(),
                        reasons = allReasonsClimateGroups
                           .Select(reason => new reasonsClimateGroup
                           {
                               id = reason.id,
                               name = reason.name,
                               count = group.Count(item => item.idReason == reason.id),
                               percent = group.Count(item => item.idReason != 0) > 0 ? Math.Round(((Convert.ToDouble(group.Count(item => item.idReason == reason.id)) / Convert.ToDouble(group.Count(item => item.idReason != 0)) * 100)), 2, MidpointRounding.AwayFromZero) : 0
                           })
                            .ToList()

                    }).ToList();
                }
                else if (hierarchy == "GERENTE II")
                {
                    retorno = original
                    .GroupBy(item => new { item.idGerenteII }).Where(d => d.Key.idGerenteII != 0)
                   .Select(group => new returnListGroupByHierarchyColaborator
                   {
                       id = group.Key.idGerenteII,
                       idSupervisor = 0,
                       idCoordenador = 0,
                       idGerenteII = 0,
                       idGerenteI = group.First().idGerenteI,
                       idDiretor = group.First().idDiretor,
                       idCeo = group.First().idCeo,
                       name = group.First().nomeGerenteII,
                       climates = allClimateGroups
                           .Select(climate => new climateGroup
                           {
                               id = climate.id,
                               name = climate.name,
                               count = group.Count(item => item.idClimate == climate.id),
                               percent = group.Count(item => item.idClimate != 0) > 0 ? Math.Round(((Convert.ToDouble(group.Count(item => item.idClimate == climate.id)) / Convert.ToDouble(group.Count(item => item.idClimate != 0)) * 100)), 2, MidpointRounding.AwayFromZero) : 0
                               //percent = group.Count(item => item.idClimate != 0) > 0 ? (group.Count(item => item.idClimate == climate.id) / group.Count(item => item.idClimate != 0)) * 100 : 0
                           })
                           .ToList(),
                       reasons = allReasonsClimateGroups
                           .Select(reason => new reasonsClimateGroup
                           {
                               id = reason.id,
                               name = reason.name,
                               count = group.Count(item => item.idReason == reason.id),
                               percent = group.Count(item => item.idReason != 0) > 0 ? Math.Round(((Convert.ToDouble(group.Count(item => item.idReason == reason.id)) / Convert.ToDouble(group.Count(item => item.idReason != 0)) * 100)), 2, MidpointRounding.AwayFromZero) : 0
                           })
                            .ToList()

                   }).ToList();
                }
                else if (hierarchy == "GERENTE I")
                {
                    retorno = original
                    .GroupBy(item => new { item.idGerenteI }).Where(d => d.Key.idGerenteI != 0)
                   .Select(group => new returnListGroupByHierarchyColaborator
                   {
                       id = group.Key.idGerenteI,
                       idSupervisor = 0,
                       idCoordenador = 0,
                       idGerenteII = 0,
                       idGerenteI = 0,
                       idDiretor = group.First().idDiretor,
                       idCeo = group.First().idCeo,
                       name = group.First().nomeGerenteI,
                       climates = allClimateGroups
                           .Select(climate => new climateGroup
                           {
                               id = climate.id,
                               name = climate.name,
                               count = group.Count(item => item.idClimate == climate.id),
                               percent = group.Count(item => item.idClimate != 0) > 0 ? Math.Round(((Convert.ToDouble(group.Count(item => item.idClimate == climate.id)) / Convert.ToDouble(group.Count(item => item.idClimate != 0)) * 100)), 2, MidpointRounding.AwayFromZero) : 0
                               //percent = group.Count(item => item.idClimate != 0) > 0 ? (group.Count(item => item.idClimate == climate.id) / group.Count(item => item.idClimate != 0)) * 100 : 0
                           })
                           .ToList(),
                       reasons = allReasonsClimateGroups
                           .Select(reason => new reasonsClimateGroup
                           {
                               id = reason.id,
                               name = reason.name,
                               count = group.Count(item => item.idReason == reason.id),
                               percent = group.Count(item => item.idReason != 0) > 0 ? Math.Round(((Convert.ToDouble(group.Count(item => item.idReason == reason.id)) / Convert.ToDouble(group.Count(item => item.idReason != 0)) * 100)), 2, MidpointRounding.AwayFromZero) : 0
                           })
                            .ToList()

                   }).ToList();
                }
                else if (hierarchy == "DIRETOR")
                {
                    retorno = original
                    .GroupBy(item => new { item.idDiretor }).Where(d => d.Key.idDiretor != 0)
                    .Select(group => new returnListGroupByHierarchyColaborator
                    {
                        id = group.Key.idDiretor,
                        idSupervisor = 0,
                        idCoordenador = 0,
                        idGerenteII = 0,
                        idGerenteI = 0,
                        idDiretor = 0,
                        idCeo = group.First().idCeo,
                        name = group.First().nomeDiretor,
                        climates = allClimateGroups
                           .Select(climate => new climateGroup
                           {
                               id = climate.id,
                               name = climate.name,
                               count = group.Count(item => item.idClimate == climate.id),
                               percent = group.Count(item => item.idClimate != 0) > 0 ? Math.Round(((Convert.ToDouble(group.Count(item => item.idClimate == climate.id)) / Convert.ToDouble(group.Count(item => item.idClimate != 0)) * 100)), 2, MidpointRounding.AwayFromZero) : 0
                               //percent = group.Count(item => item.idClimate != 0) > 0 ? (group.Count(item => item.idClimate == climate.id) / group.Count(item => item.idClimate != 0)) * 100 : 0
                           })
                           .ToList(),
                        reasons = allReasonsClimateGroups
                           .Select(reason => new reasonsClimateGroup
                           {
                               id = reason.id,
                               name = reason.name,
                               count = group.Count(item => item.idReason == reason.id),
                               percent = group.Count(item => item.idReason != 0) > 0 ? Math.Round(((Convert.ToDouble(group.Count(item => item.idReason == reason.id)) / Convert.ToDouble(group.Count(item => item.idReason != 0)) * 100)), 2, MidpointRounding.AwayFromZero) : 0
                           })
                            .ToList()

                    }).ToList();
                }
                else if (hierarchy == "CEO")
                {
                    retorno = original
                    .GroupBy(item => new { item.idCeo }).Where(d => d.Key.idCeo != 0)
                    .Select(group => new returnListGroupByHierarchyColaborator
                    {
                        id = group.Key.idCeo,
                        idSupervisor = 0,
                        idCoordenador = 0,
                        idGerenteII = 0,
                        idGerenteI = 0,
                        idDiretor = 0,
                        idCeo = 0,
                        name = group.First().nomeCeo,
                        climates = allClimateGroups
                           .Select(climate => new climateGroup
                           {
                               id = climate.id,
                               name = climate.name,
                               count = group.Count(item => item.idClimate == climate.id),
                               percent = group.Count(item => item.idClimate != 0) > 0 ? Math.Round(((Convert.ToDouble(group.Count(item => item.idClimate == climate.id)) / Convert.ToDouble(group.Count(item => item.idClimate != 0)) * 100)), 2, MidpointRounding.AwayFromZero) : 0
                               //percent = group.Count(item => item.idClimate != 0) > 0 ? (group.Count(item => item.idClimate == climate.id) / group.Count(item => item.idClimate != 0)) * 100 : 0
                           })
                           .ToList(),
                        reasons = allReasonsClimateGroups
                           .Select(reason => new reasonsClimateGroup
                           {
                               id = reason.id,
                               name = reason.name,
                               count = group.Count(item => item.idReason == reason.id),
                               percent = group.Count(item => item.idReason != 0) > 0 ? Math.Round(((Convert.ToDouble(group.Count(item => item.idReason == reason.id)) / Convert.ToDouble(group.Count(item => item.idReason != 0)) * 100)), 2, MidpointRounding.AwayFromZero) : 0
                           })
                            .ToList()
                    }).ToList();
                }
            }
            catch (Exception ex)
            {

            }
            return retorno;
        }

        private static string GetPeriod(string date, GroupingPeriod period)
        {
            DateTime dateTime = DateTime.Parse(date);

            switch (period)
            {
                case GroupingPeriod.Day:
                    return dateTime.ToString("yyyy-MM-dd");
                case GroupingPeriod.Week:
                    return $"{dateTime.Year}-W{GetIso8601WeekOfYear(dateTime)}";
                case GroupingPeriod.Biweekly:
                    return $"{dateTime.Year}-BW{GetIso8601BiweekOfYear(dateTime)}";
                case GroupingPeriod.Month:
                    return dateTime.ToString("yyyy-MM");
                default:
                    throw new ArgumentException("Invalid grouping period");
            }
        }

        private static int GetIso8601WeekOfYear(DateTime date)
        {
            return System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        private static int GetIso8601BiweekOfYear(DateTime date)
        {
            return (GetIso8601WeekOfYear(date) + 1) / 2;
        }
    }




    public class BankListGroupHierarchyClimate
    {
        public static List<returnListsGroupHierarchyClimate> returnListGroupHierarchy(int idCollaborator, string dtInicial, string dtFinal, string filtroSetor, string filtro, string filtroCollaborator, string filterRowSet)
        {

            List<returnListsGroupHierarchyClimate> resposts = new List<returnListsGroupHierarchyClimate>();

            StringBuilder sb = new StringBuilder();
            sb.Append($"DECLARE @ID INT; SET @ID = '{idCollaborator}'; ");
            sb.Append($"DECLARE @searchAccount NVARCHAR(100) = '{filtroCollaborator}'; ");
            sb.Append($"DECLARE @DATE_INI DATE; SET @DATE_INI = '{dtInicial}'; ");
            sb.Append($"DECLARE @DATE_FIM DATE; SET @DATE_FIM = '{dtFinal}'; ");
            sb.Append($"SELECT  ");
            sb.Append($"CD.IDGDA_COLLABORATORS, GCU.IDGDA_CLIMATE_USER, CONVERT(DATE, CD.CREATED_AT) AS DATA, ");
            sb.Append($"MAX(PU.NAME) AS NAME, ");
            sb.Append($"MAX(PU.BC) AS BC,  ");
            sb.Append($"CD.IDGDA_COLLABORATORS, ");
            sb.Append($"PU.IDGDA_PERSONA_USER,  ");
            sb.Append($"MAX(C.IDGDA_CLIMATE) AS IDGDA_CLIMATE,  ");
            sb.Append($"MAX(CR.IDGDA_CLIMATE_REASON) AS IDGDA_CLIMATE_REASON,  ");
            sb.Append($"CASE WHEN MAX(C.CLIMATE) IS NULL THEN 'SEM RESPOSTA' ELSE MAX(C.CLIMATE) END AS CLIMATE, ");
            sb.Append($"CASE WHEN MAX(CR.REASON) IS NULL THEN '' ELSE MAX(CR.REASON) END AS REASON, ");
            sb.Append($"MAX(AT.TYPE) AS TYPE, ");
            sb.Append($"MAX(CD.NOME_SUPERVISOR) AS NOME_SUPERVISOR, ");
            sb.Append($"MAX(CD.MATRICULA_SUPERVISOR) AS MATRICULA_SUPERVISOR, ");
            sb.Append($"MAX(CD.NOME_COORDENADOR) AS NOME_COORDENADOR, ");
            sb.Append($"MAX(CD.MATRICULA_COORDENADOR) AS MATRICULA_COORDENADOR, ");
            sb.Append($"MAX(CD.NOME_GERENTE_II) AS NOME_GERENTE_II, ");
            sb.Append($"MAX(CD.MATRICULA_GERENTE_II) AS MATRICULA_GERENTE_II, ");
            sb.Append($"MAX(CD.NOME_GERENTE_I) AS NOME_GERENTE_I, ");
            sb.Append($"MAX(CD.MATRICULA_GERENTE_I) AS MATRICULA_GERENTE_I, ");
            sb.Append($"MAX(CD.NOME_DIRETOR) AS NOME_DIRETOR, ");
            sb.Append($"MAX(CD.MATRICULA_DIRETOR) AS MATRICULA_DIRETOR, ");
            sb.Append($"MAX(CD.NOME_CEO) AS NOME_CEO, ");
            sb.Append($"MAX(CD.MATRICULA_CEO) AS MATRICULA_CEO, ");
            sb.Append($"MAX(CD.GRUPO) AS GRUPO, ");
            sb.Append($"MAX(H.IDGDA_HIERARCHY) AS IDHIERARCHY ");
            sb.Append($"FROM  ");
            sb.Append($"(  ");
            sb.Append($"SELECT  ");
            sb.Append($"CDI2.IDGDA_COLLABORATORS, CONVERT(DATE, CDI2.CREATED_AT) AS CREATED_AT,  ");
            sb.Append($"MAX(CDI2.MATRICULA_SUPERVISOR) AS MATRICULA_SUPERVISOR, MAX(CDI2.NOME_SUPERVISOR) AS NOME_SUPERVISOR,  ");
            sb.Append($"MAX(CDI2.MATRICULA_COORDENADOR) AS MATRICULA_COORDENADOR,  MAX(CDI2.NOME_COORDENADOR) AS NOME_COORDENADOR,  ");
            sb.Append($"MAX(CDI2.MATRICULA_GERENTE_II) AS MATRICULA_GERENTE_II,  MAX(CDI2.NOME_GERENTE_II) AS NOME_GERENTE_II, ");
            sb.Append($"MAX(CDI2.MATRICULA_GERENTE_I) AS MATRICULA_GERENTE_I, MAX(CDI2.NOME_GERENTE_I) AS NOME_GERENTE_I,  ");
            sb.Append($"MAX(CDI2.MATRICULA_DIRETOR) AS MATRICULA_DIRETOR, MAX(CDI2.NOME_DIRETOR) AS NOME_DIRETOR,  ");
            sb.Append($"MAX(CDI2.MATRICULA_CEO) AS MATRICULA_CEO, MAX(CDI2.NOME_CEO) AS NOME_CEO, MAX(CDI2.IDGDA_GROUP) AS GRUPO, MAX(CDI2.CARGO) AS CARGO ");
            sb.Append($"FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CDI2 ");
            sb.Append($"WHERE CDI2.CREATED_AT >= @DATE_INI AND CDI2.CREATED_AT <= @DATE_FIM   ");
            sb.Append($"{filtroSetor} ");
            sb.Append($"AND CDI2.CARGO = 'AGENTE' ");
            sb.Append($"GROUP BY CDI2.IDGDA_COLLABORATORS, CONVERT(DATE, CDI2.CREATED_AT) ");
            sb.Append($"UNION ALL ");
            sb.Append($"SELECT  ");
            sb.Append($"CDI.IDGDA_COLLABORATORS, CONVERT(DATE, CDI.CREATED_AT) AS CREATED_AT,  ");
            sb.Append($"MAX(CDI.MATRICULA_SUPERVISOR) AS MATRICULA_SUPERVISOR, MAX(CDI.NOME_SUPERVISOR) AS NOME_SUPERVISOR,  ");
            sb.Append($"MAX(CDI.MATRICULA_COORDENADOR) AS MATRICULA_COORDENADOR,  MAX(CDI.NOME_COORDENADOR) AS NOME_COORDENADOR,  ");
            sb.Append($"MAX(CDI.MATRICULA_GERENTE_II) AS MATRICULA_GERENTE_II,  MAX(CDI.NOME_GERENTE_II) AS NOME_GERENTE_II, ");
            sb.Append($"MAX(CDI.MATRICULA_GERENTE_I) AS MATRICULA_GERENTE_I, MAX(CDI.NOME_GERENTE_I) AS NOME_GERENTE_I,  ");
            sb.Append($"MAX(CDI.MATRICULA_DIRETOR) AS MATRICULA_DIRETOR, MAX(CDI.NOME_DIRETOR) AS NOME_DIRETOR,  ");
            sb.Append($"MAX(CDI.MATRICULA_CEO) AS MATRICULA_CEO, MAX(CDI.NOME_CEO) AS NOME_CEO, MAX(CDI.IDGDA_GROUP) AS GRUPO, MAX(CDI.CARGO) AS CARGO ");
            sb.Append($"FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CDI ");
            sb.Append($"INNER JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CDI2 ON  CONVERT(DATE, CDI.CREATED_AT) = CONVERT(DATE, CDI2.CREATED_AT) AND  ");
            sb.Append($"(CDI2.MATRICULA_SUPERVISOR = CDI.IDGDA_COLLABORATORS OR  ");
            sb.Append($"		CDI2.MATRICULA_COORDENADOR = CDI.IDGDA_COLLABORATORS OR  ");
            sb.Append($"		CDI2.MATRICULA_GERENTE_II = CDI.IDGDA_COLLABORATORS OR  ");
            sb.Append($"		CDI2.MATRICULA_GERENTE_I = CDI.IDGDA_COLLABORATORS OR  ");
            sb.Append($"		CDI2.MATRICULA_DIRETOR = CDI.IDGDA_COLLABORATORS OR  ");
            sb.Append($"		CDI2.MATRICULA_CEO = CDI.IDGDA_COLLABORATORS) AND CDI2.CREATED_AT >= @DATE_INI AND CDI2.CREATED_AT <= @DATE_FIM ");
            sb.Append($"WHERE CDI.CREATED_AT >= @DATE_INI AND CDI.CREATED_AT <= @DATE_FIM  ");
            sb.Append($"AND CDI.CARGO != 'AGENTE' AND CDI.CARGO IS NOT NULL {filtroSetor} ");
            sb.Append($"GROUP BY CDI.IDGDA_COLLABORATORS, CONVERT(DATE, CDI.CREATED_AT) ");
            sb.Append($"UNION ALL ");
            sb.Append($"SELECT  ");
            sb.Append($"CDI2.IDGDA_COLLABORATORS, CONVERT(DATE, GETDATE()) AS CREATED_AT,  ");
            sb.Append($"MAX(CDI2.MATRICULA_SUPERVISOR) AS MATRICULA_SUPERVISOR, MAX(CDI2.NOME_SUPERVISOR) AS NOME_SUPERVISOR,  ");
            sb.Append($"MAX(CDI2.MATRICULA_COORDENADOR) AS MATRICULA_COORDENADOR,  MAX(CDI2.NOME_COORDENADOR) AS NOME_COORDENADOR,  ");
            sb.Append($"MAX(CDI2.MATRICULA_GERENTE_II) AS MATRICULA_GERENTE_II,  MAX(CDI2.NOME_GERENTE_II) AS NOME_GERENTE_II, ");
            sb.Append($"MAX(CDI2.MATRICULA_GERENTE_I) AS MATRICULA_GERENTE_I, MAX(CDI2.NOME_GERENTE_I) AS NOME_GERENTE_I,  ");
            sb.Append($"MAX(CDI2.MATRICULA_DIRETOR) AS MATRICULA_DIRETOR, MAX(CDI2.NOME_DIRETOR) AS NOME_DIRETOR,  ");
            sb.Append($"MAX(CDI2.MATRICULA_CEO) AS MATRICULA_CEO, MAX(CDI2.NOME_CEO) AS NOME_CEO, MAX(CDI2.IDGDA_GROUP) AS GRUPO, MAX(CDI2.CARGO) AS CARGO ");
            sb.Append($"FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CDI2 ");
            sb.Append($"WHERE CDI2.CREATED_AT >= CONVERT(DATE, DATEADD(DAY, -1, GETDATE()))  ");
            sb.Append($"{filtroSetor} ");
            sb.Append($"AND CDI2.CARGO = 'AGENTE' ");
            sb.Append($"GROUP BY CDI2.IDGDA_COLLABORATORS ");
            sb.Append($"UNION ALL ");
            sb.Append($"SELECT  ");
            sb.Append($"CDI.IDGDA_COLLABORATORS, CONVERT(DATE, GETDATE()) AS CREATED_AT,  ");
            sb.Append($"MAX(CDI.MATRICULA_SUPERVISOR) AS MATRICULA_SUPERVISOR, MAX(CDI.NOME_SUPERVISOR) AS NOME_SUPERVISOR,  ");
            sb.Append($"MAX(CDI.MATRICULA_COORDENADOR) AS MATRICULA_COORDENADOR,  MAX(CDI.NOME_COORDENADOR) AS NOME_COORDENADOR,  ");
            sb.Append($"MAX(CDI.MATRICULA_GERENTE_II) AS MATRICULA_GERENTE_II,  MAX(CDI.NOME_GERENTE_II) AS NOME_GERENTE_II, ");
            sb.Append($"MAX(CDI.MATRICULA_GERENTE_I) AS MATRICULA_GERENTE_I, MAX(CDI.NOME_GERENTE_I) AS NOME_GERENTE_I,  ");
            sb.Append($"MAX(CDI.MATRICULA_DIRETOR) AS MATRICULA_DIRETOR, MAX(CDI.NOME_DIRETOR) AS NOME_DIRETOR,  ");
            sb.Append($"MAX(CDI.MATRICULA_CEO) AS MATRICULA_CEO, MAX(CDI.NOME_CEO) AS NOME_CEO, MAX(CDI.IDGDA_GROUP) AS GRUPO, MAX(CDI.CARGO) AS CARGO ");
            sb.Append($"FROM GDA_COLLABORATORS_DETAILS (NOLOCK) AS CDI ");
            sb.Append($"INNER JOIN GDA_COLLABORATORS_DETAILS (NOLOCK) AS CDI2 ON  CONVERT(DATE, CDI.CREATED_AT) = CONVERT(DATE, CDI2.CREATED_AT) AND  ");
            sb.Append($"(CDI2.MATRICULA_SUPERVISOR = CDI.IDGDA_COLLABORATORS OR  ");
            sb.Append($"		CDI2.MATRICULA_COORDENADOR = CDI.IDGDA_COLLABORATORS OR  ");
            sb.Append($"		CDI2.MATRICULA_GERENTE_II = CDI.IDGDA_COLLABORATORS OR  ");
            sb.Append($"		CDI2.MATRICULA_GERENTE_I = CDI.IDGDA_COLLABORATORS OR  ");
            sb.Append($"		CDI2.MATRICULA_DIRETOR = CDI.IDGDA_COLLABORATORS OR  ");
            sb.Append($"		CDI2.MATRICULA_CEO = CDI.IDGDA_COLLABORATORS) AND CDI2.CREATED_AT >= CONVERT(DATE, DATEADD(DAY, -1, GETDATE())) ");
            sb.Append($"WHERE CDI.CREATED_AT >= CONVERT(DATE, DATEADD(DAY, -1, GETDATE()))  ");
            sb.Append($"AND CDI.CARGO != 'AGENTE' AND CDI.CARGO IS NOT NULL {filtroSetor} ");
            sb.Append($"GROUP BY CDI.IDGDA_COLLABORATORS ");
            sb.Append($") AS CD ");
            sb.Append($"LEFT JOIN GDA_PERSONA_COLLABORATOR_USER (NOLOCK) AS PCU ON PCU.IDGDA_COLLABORATORS = CD.IDGDA_COLLABORATORS ");
            sb.Append($"LEFT JOIN GDA_PERSONA_USER (NOLOCK) AS PU ON PU.IDGDA_PERSONA_USER = PCU.IDGDA_PERSONA_USER AND PU.IDGDA_PERSONA_USER_TYPE = 1  ");
            sb.Append($"LEFT JOIN GDA_CLIMATE_USER (NOLOCK) AS GCU ON GCU.IDGDA_PERSONA = PU.IDGDA_PERSONA_USER AND CONVERT(DATE, GCU.CREATED_AT) = CONVERT(DATE, CD.CREATED_AT) ");
            sb.Append($"LEFT JOIN GDA_CLIMATE (NOLOCK) AS C ON C.IDGDA_CLIMATE = GCU.IDGDA_CLIMATE  ");
            sb.Append($"LEFT JOIN GDA_CLIMATE_REASON (NOLOCK) AS CR ON CR.IDGDA_CLIMATE_REASON = GCU.IDGDA_CLIMATE_REASON ");
            sb.Append($"LEFT JOIN GDA_CLIMATE_APPLY_TYPE (NOLOCK) AS AT ON AT.IDGDA_CLIMATE_APPLY_TYPE = GCU.IDGDA_CLIMATE_APPLY_TYPE ");
            sb.Append($"LEFT JOIN GDA_HIERARCHY (NOLOCK) AS H ON H.LEVELNAME = CD.CARGO ");
            sb.Append($"WHERE (CD.IDGDA_COLLABORATORS = @ID OR  ");
            sb.Append($"		CD.MATRICULA_SUPERVISOR = @ID OR  ");
            sb.Append($"		CD.MATRICULA_COORDENADOR = @ID OR  ");
            sb.Append($"		CD.MATRICULA_GERENTE_II = @ID OR  ");
            sb.Append($"		CD.MATRICULA_GERENTE_I = @ID OR  ");
            sb.Append($"		CD.MATRICULA_DIRETOR = @ID OR  ");
            sb.Append($"		CD.MATRICULA_CEO = @ID) ");
            sb.Append($"AND BC IS NOT NULL ");
            sb.Append($"AND CONVERT(DATE, CD.CREATED_AT) >= @DATE_INI AND CONVERT(DATE, CD.CREATED_AT) <= @DATE_FIM {filtro} ");
            sb.Append($"GROUP BY CD.IDGDA_COLLABORATORS, GCU.IDGDA_CLIMATE_USER, CONVERT(DATE, CD.CREATED_AT), CD.IDGDA_COLLABORATORS, PU.IDGDA_PERSONA_USER ");
            sb.Append($"ORDER BY CONVERT(DATE, CD.CREATED_AT), CD.IDGDA_COLLABORATORS ");
            sb.Append($"{filterRowSet} ");


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
                                returnListsGroupHierarchyClimate rtn = new returnListsGroupHierarchyClimate();
                                rtn.idCollaborator = reader["IDGDA_COLLABORATORS"] != DBNull.Value ? int.Parse(reader["IDGDA_COLLABORATORS"].ToString()) : 0;
                                rtn.idUserClimate = reader["IDGDA_CLIMATE_USER"] != DBNull.Value ? int.Parse(reader["IDGDA_CLIMATE_USER"].ToString()) : 0;
                                rtn.data = reader["DATA"] != DBNull.Value ? reader["DATA"].ToString() : "";
                                rtn.name = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : "";
                                rtn.BC = reader["BC"] != DBNull.Value ? reader["BC"].ToString() : "";
                                rtn.climate = reader["CLIMATE"] != DBNull.Value ? reader["CLIMATE"].ToString() : "";
                                rtn.reason = reader["REASON"] != DBNull.Value ? reader["REASON"].ToString() : "";

                                rtn.idClimate = reader["IDGDA_CLIMATE"] != DBNull.Value ? int.Parse(reader["IDGDA_CLIMATE"].ToString()) : 0;
                                rtn.idReason = reader["IDGDA_CLIMATE_REASON"] != DBNull.Value ? int.Parse(reader["IDGDA_CLIMATE_REASON"].ToString()) : 0;
                                rtn.idHierarchy = reader["IDHIERARCHY"] != DBNull.Value ? int.Parse(reader["IDHIERARCHY"].ToString()) : 0;

                                rtn.applyType = reader["TYPE"] != DBNull.Value ? reader["TYPE"].ToString() : "";
                                rtn.idSupervisor = reader["MATRICULA_SUPERVISOR"] != DBNull.Value ? int.Parse(reader["MATRICULA_SUPERVISOR"].ToString()) : 0; 
                                rtn.nomeSupervisor = reader["NOME_SUPERVISOR"] != DBNull.Value ? reader["NOME_SUPERVISOR"].ToString() : "";
                                rtn.idCoordenador = reader["MATRICULA_COORDENADOR"] != DBNull.Value ? int.Parse(reader["MATRICULA_COORDENADOR"].ToString()) : 0; 
                                rtn.nomeCoordenador = reader["NOME_COORDENADOR"] != DBNull.Value ? reader["NOME_COORDENADOR"].ToString() : "";
                                rtn.idGerenteII = reader["MATRICULA_GERENTE_II"] != DBNull.Value ? int.Parse(reader["MATRICULA_GERENTE_II"].ToString()) : 0; 
                                rtn.nomeGerenteII = reader["NOME_GERENTE_II"] != DBNull.Value ? reader["NOME_GERENTE_II"].ToString() : "";
                                rtn.idGerenteI = reader["MATRICULA_GERENTE_I"] != DBNull.Value ? int.Parse(reader["MATRICULA_GERENTE_I"].ToString()) : 0; 
                                rtn.nomeGerenteI = reader["NOME_GERENTE_I"] != DBNull.Value ? reader["NOME_GERENTE_I"].ToString() : "";
                                rtn.idDiretor = reader["MATRICULA_DIRETOR"] != DBNull.Value ? int.Parse(reader["MATRICULA_DIRETOR"].ToString()) : 0; 
                                rtn.nomeDiretor = reader["NOME_DIRETOR"] != DBNull.Value ? reader["NOME_DIRETOR"].ToString() : "";
                                rtn.idCeo = reader["MATRICULA_CEO"] != DBNull.Value ? int.Parse(reader["MATRICULA_CEO"].ToString()) : 0; 
                                rtn.nomeCeo = reader["NOME_CEO"] != DBNull.Value ? reader["NOME_CEO"].ToString() : "";

                                resposts.Add(rtn);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Trate a exceção aqui
                }
                connection.Close();
            }


            return resposts;
        }

    }

}