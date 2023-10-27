using btl_tkweb.Models;
using Microsoft.EntityFrameworkCore;

namespace btl_tkweb.Data
{
    public class SchoolContext:DbContext
    {
        public SchoolContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<Lop> Lop { get; set; }
        public virtual DbSet<HocSinh> HocSinh { get; set;}
        public virtual DbSet<MonHoc> MonHoc { get; set;}
        public virtual DbSet<DiemSo> DiemSo { get; set; }
        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<GiaoVien> GiaoVien { get; set; }
        public virtual DbSet<ChiTietGiangDay> ChiTietGiangDay { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lop>().ToTable(nameof(Lop));
            modelBuilder.Entity<HocSinh>().ToTable(nameof(HocSinh));
            modelBuilder.Entity<MonHoc>().ToTable(nameof(MonHoc));
            modelBuilder.Entity<DiemSo>().ToTable(nameof(DiemSo));
            modelBuilder.Entity<Account>().ToTable(nameof(Account));
            modelBuilder.Entity<GiaoVien>().ToTable(nameof(GiaoVien));
            modelBuilder.Entity<ChiTietGiangDay>().ToTable(nameof(ChiTietGiangDay));


        }
    }
}