using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangAdmin.Models;

namespace QuanLyNhaHangAdmin.Data
{
    public class QuanLyNhaHangContext : DbContext
    {
        public QuanLyNhaHangContext(DbContextOptions<QuanLyNhaHangContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<BanAn> BanAns { get; set; }
        public DbSet<LoaiMonAn> LoaiMonAns { get; set; }
        public DbSet<MonAn> MonAns { get; set; }
        public DbSet<DatBan> DatBans { get; set; }
        public DbSet<DatMon> DatMons { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }
        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // KhachHang
            modelBuilder.Entity<KhachHang>()
                .HasKey(k => k.MaKH);

            // NhanVien
            modelBuilder.Entity<NhanVien>()
                .HasKey(n => n.MaNV);

            // BanAn
            modelBuilder.Entity<BanAn>()
                .HasKey(b => b.MaBan);

            // LoaiMonAn
            modelBuilder.Entity<LoaiMonAn>()
                .HasKey(l => l.MaLoai);

            // MonAn
            modelBuilder.Entity<MonAn>()
                .HasKey(m => m.MaMon);
            modelBuilder.Entity<MonAn>()
                .HasOne(m => m.LoaiMonAn)
                .WithMany(l => l.MonAns)
                .HasForeignKey(m => m.MaLoai);

            // DatBan
            modelBuilder.Entity<DatBan>()
                .HasKey(d => d.MaDatBan);
            modelBuilder.Entity<DatBan>()
                .HasOne(d => d.KhachHang)
                .WithMany(k => k.DatBans)
                .HasForeignKey(d => d.MaKH)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DatBan>()
                .HasOne(d => d.BanAn)
                .WithMany(b => b.DatBans)
                .HasForeignKey(d => d.MaBan)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DatBan>()
                .HasOne(d => d.NhanVien)
                .WithMany(n => n.DatBans)
                .HasForeignKey(d => d.MaNV)
                .OnDelete(DeleteBehavior.SetNull);

            // DatMon
            modelBuilder.Entity<DatMon>()
                .HasKey(dm => dm.MaDatMon);
            modelBuilder.Entity<DatMon>()
                .HasOne(dm => dm.DatBan)
                .WithMany(db => db.DatMons)
                .HasForeignKey(dm => dm.MaDatBan)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<DatMon>()
                .HasOne(dm => dm.MonAn)
                .WithMany(m => m.DatMons)
                .HasForeignKey(dm => dm.MaMon)
                .OnDelete(DeleteBehavior.Restrict);

            // HoaDon
            modelBuilder.Entity<HoaDon>()
                .HasKey(hd => hd.MaHD);
            modelBuilder.Entity<HoaDon>()
                .HasOne(hd => hd.KhachHang)
                .WithMany(k => k.HoaDons)
                .HasForeignKey(hd => hd.MaKH)
                .OnDelete(DeleteBehavior.SetNull);

            // ChiTietHoaDon
            modelBuilder.Entity<ChiTietHoaDon>()
                .HasKey(ct => new { ct.MaHD, ct.MaMon });
            modelBuilder.Entity<ChiTietHoaDon>()
                .HasOne(ct => ct.HoaDon)
                .WithMany(hd => hd.ChiTietHoaDons)
                .HasForeignKey(ct => ct.MaHD)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ChiTietHoaDon>()
                .HasOne(ct => ct.MonAn)
                .WithMany(m => m.ChiTietHoaDons)
                .HasForeignKey(ct => ct.MaMon)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

