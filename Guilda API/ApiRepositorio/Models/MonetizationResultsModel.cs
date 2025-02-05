using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiRepositorio.Models
{
    public class monetizationHierarchyInformation
    {
        public int idCollaborator { get; set; }
        public int idSite { get; set; }
        public int idSector { get; set; }
        public int idSubSector { get; set; }
        public int duePause { get; set; }
        public int daySite { get; set; }
        public int daySetor { get; set; }
        public int idgdaMonetizationConfigSite { get; set; }
        public int idgdaMonetizationConfigSetor { get; set; }
        public DateTime? dueDateCalculed { get; set; }
    }

    public class MonetizationResultsModel
    {

        public string idCollaborator { get; set; }
        public string idIndicator { get; set; }
        public string typeIndicator { get; set; }
        public string idResult { get; set; }
        public string idSector { get; set; }
        public int idCheckingAccount { get; set; }
        public string indicatorWeight { get; set; }
        public string hierarchyLevel { get; set; }
        public double meta { get; set; }
        public string fatores { get; set; }
        public double fator0 { get; set; }
        public double fator1 { get; set; }
        public string conta { get; set; }
        public string melhor { get; set; }
        public double G1 { get; set; }
        public double G2 { get; set; }
        public double G3 { get; set; }
        public double G4 { get; set; }
        public double C1 { get; set; }
        public double C2 { get; set; }
        public double C3 { get; set; }
        public double C4 { get; set; }
        public double saldo { get; set; }
        public string parentId { get; set; }
        public string levelName { get; set; }
        public string level { get; set; }

        public int transactionId { get; set; }
        public string matriculaSupervisor { get; set; }
        public string nomeSupervisor { get; set; }
        public string matriculaCoordenador { get; set; }
        public string nomeCoordenador { get; set; }
        public string matriculaGerenteii { get; set; }
        public string nomeGerenteii { get; set; }
        public string matriculaGerentei { get; set; }
        public string nomeGerentei { get; set; }
        public string matriculaDiretor { get; set; }
        public string nomeDiretor { get; set; }
        public string matriculaCeo { get; set; }
        public string nomeCeo { get; set; }
        public double coins { get; set; }
        public double quantidade { get; set; }
        public int sumDiasLogados { get; set; }
        public int sumDiasEscalados { get; set; }
        public int sumDiasLogadosEEscalados { get; set; }

        public int duePause { get; set; }
        public int daySite { get; set; }
        public int daySetor { get; set; }
        public int idgdaMonetizationConfigSite { get; set; }
        public int idgdaMonetizationConfigSetor { get; set; }
        public DateTime? dueDateCalculed { get; set; }

        public int hig1Id { get; set; }
        public int hig2Id { get; set; }
        public int hig3Id { get; set; }
        public int hig4Id { get; set; }
        public int hisId { get; set; }


    }
}