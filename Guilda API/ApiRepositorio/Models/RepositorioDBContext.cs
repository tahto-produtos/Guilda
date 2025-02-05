using System.Data.Entity;

namespace ApiRepositorio.Models
{
    public class RepositorioDBContext : DbContext
    {
        public RepositorioDBContext() : base(global::Database.Conn)
        { }
        public virtual DbSet<TokenModel> TokenModels { get; set; }
        public virtual DbSet<UserModel> UserModels { get; set; }
        public virtual DbSet<TransactionModel> TransactionModels { get; set; }
        public virtual DbSet<CollaboratorTableModel> CollaboratorTableModels { get; set; }
        public virtual DbSet<IndicatorTableModel> IndicatorTableModels { get; set; }
        public virtual DbSet<SectorModel> SectorModels { get; set; }
        public virtual DbSet<HistoryCollaboratorSectorModel> HistoryCollaboratorSectorModels { get; set; }
        public virtual DbSet<HistoryHierarchiesModel> HistoryHierarchiesModels { get; set; }
        public virtual DbSet<HierarchiesTableModel> HierarchiesTableModels { get; set; }
        public virtual DbSet<ResultsModel> ResultsModels { get; set; }
        public virtual DbSet<FactorsModel> FactorsModels { get; set; }
        public virtual DbSet<AtributeTableModel> AtributeTableModels { get; set; }
        public virtual DbSet<LogRequestModel> LogRequestModels { get; set; }
        public virtual DbSet<TransactionLogTableModel> TransactionLogTableModels { get; set; }
        public System.Data.Entity.DbSet<ApiRepositorio.Models.HierarchiesModel> HierarchiesModels { get; set; }
    }
}