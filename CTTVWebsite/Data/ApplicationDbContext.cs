
using CTTVWebsite.Models;
using Microsoft.EntityFrameworkCore;

namespace Website.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Wiptracking> R_WIP_TRACKING_T { get; set; }
        public DbSet<MOno> mOnos { get; set; }
        public DbSet<Mosum> mosums { get; set; }
        public DbSet<PalletTrackng> palletTrackngs { get; set; }
        public DbSet<SnDetail> R_SN_DETAIL_T { get; set; }
        public DbSet<Wipkeypart> R_WIP_KEYPART_T { get; set; }
        public DbSet<HistorySn> History_serial { get; set; }
        public DbSet<ApplyTable> R_APPLY_TABLE_T1 { get; set; }
        public DbSet<QAHold> QA_HOLD { get; set; }
        public DbSet<InOut> inOuts { get; set; }
        public DbSet<InOutTotal> inOutTotals { get; set; }
        public DbSet<InOutOBI> inOutObi { get; set;}
        public DbSet<PONO> Pono { get; set; }
        public DbSet<Holdstt> holdstts { get; set; }
        public DbSet<ModelNotShip> Modelnotship { get; set; }
        public DbSet<WiptrackingWipkeypart> WiptrackingWipkeypart { get; set; }
        public DbSet<InstockSearch> Instocksearch { get; set; }
        public DbSet<StockInven> stockInvens { get; set; }
        public DbSet<OldKP> OldKP { get; set; }
        public DbSet<OBI>  OBIs { get; set; }
        public DbSet<OBI_QA_detail> OBI_QA_detail { get; set; }
        public DbSet<OBI_EMP> OBI_EMPs { get; set; }
        public DbSet<OBI_EMP_T> OBI_EMPTs { get; set; }
        public DbSet<KpTvSn> KpTvSn { get; set;}
        public DbSet<FacSum> FacSums { get; set; }
        public DbSet<Bincode> bincodes { get; set; }
        public DbSet<Class> plan { get; set; }
        public DbSet<OnlineOver> r_wip_online_t { get; set; }
        public DbSet<KpManage> C_KEYPARTS_MANAGE_T { get; set; }
        public DbSet<ProOutput> ProOutputs { get; set; }
        public DbSet<HoldModel> holdModels { get; set; }
        public DbSet<InoutPro>inoutPros { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Wiptracking>()
                .Property(p => p.SERIAL_NUMBER).IsRequired();

            modelBuilder.Entity<SnDetail>(
            eb =>
            {
                eb.HasNoKey();
            });
            modelBuilder.Entity<Wipkeypart>(
        eb =>
        {
            eb.HasNoKey();
        });
        }

    }

}
