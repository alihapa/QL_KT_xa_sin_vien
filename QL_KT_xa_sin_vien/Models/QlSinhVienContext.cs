using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QL_KT_xa_sin_vien.Models;

public partial class QlSinhVienContext : DbContext
{
    public QlSinhVienContext()
    {
    }

    public QlSinhVienContext(DbContextOptions<QlSinhVienContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Giuong> Giuongs { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<HopDong> HopDongs { get; set; }

    public virtual DbSet<NhatKy> NhatKies { get; set; }

    public virtual DbSet<PhanAnh> PhanAnhs { get; set; }

    public virtual DbSet<Phong> Phongs { get; set; }

    public virtual DbSet<SinhVien> SinhViens { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<ThongBao> ThongBaos { get; set; }

    public virtual DbSet<ToaNha> ToaNhas { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=QL_Sinh_vien;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Giuong>(entity =>
        {
            entity.HasKey(e => e.MaGiuong).HasName("PK__Giuong__1D39A54C4533E400");

            entity.ToTable("Giuong");

            entity.Property(e => e.MaGiuong)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("maGiuong");
            entity.Property(e => e.MaPhong)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("maPhong");
            entity.Property(e => e.OccupiedBy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("occupiedBy");
            entity.Property(e => e.SoGiuong)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("soGiuong");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("trangThai");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.Giuongs)
                .HasForeignKey(d => d.MaPhong)
                .HasConstraintName("FK__Giuong__maPhong__02FC7413");

            entity.HasOne(d => d.OccupiedByNavigation).WithMany(p => p.Giuongs)
                .HasForeignKey(d => d.OccupiedBy)
                .HasConstraintName("FK__Giuong__occupied__03F0984C");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon).HasName("PK__HoaDon__026B4D9A77ADB03D");

            entity.ToTable("HoaDon");

            entity.Property(e => e.MaHoaDon)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("maHoaDon");
            entity.Property(e => e.MaHopDong)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("maHopDong");
            entity.Property(e => e.MaSv)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("maSV");
            entity.Property(e => e.NgayXuat)
                .HasColumnType("datetime")
                .HasColumnName("ngayXuat");
            entity.Property(e => e.SoTien)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("soTien");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("trangThai");

            entity.HasOne(d => d.MaHopDongNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaHopDong)
                .HasConstraintName("FK__HoaDon__maHopDon__0B91BA14");

            entity.HasOne(d => d.MaSvNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaSv)
                .HasConstraintName("FK__HoaDon__maSV__0C85DE4D");
        });

        modelBuilder.Entity<HopDong>(entity =>
        {
            entity.HasKey(e => e.MaHopDong).HasName("PK__HopDong__429F83D9989123B4");

            entity.ToTable("HopDong");

            entity.Property(e => e.MaHopDong)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("maHopDong");
            entity.Property(e => e.DieuKhoan)
                .HasColumnType("text")
                .HasColumnName("dieuKhoan");
            entity.Property(e => e.MaGiuong)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("maGiuong");
            entity.Property(e => e.MaPhong)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("maPhong");
            entity.Property(e => e.MaSv)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("maSV");
            entity.Property(e => e.NgayBatDau).HasColumnName("ngayBatDau");
            entity.Property(e => e.NgayKetThuc).HasColumnName("ngayKetThuc");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("trangThai");

            entity.HasOne(d => d.MaGiuongNavigation).WithMany(p => p.HopDongs)
                .HasForeignKey(d => d.MaGiuong)
                .HasConstraintName("FK__HopDong__maGiuon__08B54D69");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.HopDongs)
                .HasForeignKey(d => d.MaPhong)
                .HasConstraintName("FK__HopDong__maPhong__07C12930");

            entity.HasOne(d => d.MaSvNavigation).WithMany(p => p.HopDongs)
                .HasForeignKey(d => d.MaSv)
                .HasConstraintName("FK__HopDong__maSV__06CD04F7");
        });

        modelBuilder.Entity<NhatKy>(entity =>
        {
            entity.HasKey(e => e.MaLog).HasName("PK__NhatKy__261ECAEABC269A97");

            entity.ToTable("NhatKy");

            entity.Property(e => e.MaLog)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("maLog");
            entity.Property(e => e.DoiTuong)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("doiTuong");
            entity.Property(e => e.GiaTriSau)
                .HasColumnType("text")
                .HasColumnName("giaTriSau");
            entity.Property(e => e.GiaTriTruoc)
                .HasColumnType("text")
                .HasColumnName("giaTriTruoc");
            entity.Property(e => e.HanhDong)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("hanhDong");
            entity.Property(e => e.NguoiThucHien)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nguoiThucHien");
            entity.Property(e => e.ThoiGian)
                .HasColumnType("datetime")
                .HasColumnName("thoiGian");

            entity.HasOne(d => d.NguoiThucHienNavigation).WithMany(p => p.NhatKies)
                .HasForeignKey(d => d.NguoiThucHien)
                .HasConstraintName("FK__NhatKy__nguoiThu__787EE5A0");
        });

        modelBuilder.Entity<PhanAnh>(entity =>
        {
            entity.HasKey(e => e.MaPhanAnh).HasName("PK__PhanAnh__40283471A4CB3DAB");

            entity.ToTable("PhanAnh");

            entity.Property(e => e.MaPhanAnh)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("maPhanAnh");
            entity.Property(e => e.MaPhong)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("maPhong");
            entity.Property(e => e.MaSv)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("maSV");
            entity.Property(e => e.MoTa)
                .HasColumnType("text")
                .HasColumnName("moTa");
            entity.Property(e => e.MucDoUuTien)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("mucDoUuTien");
            entity.Property(e => e.NguoiXuLy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nguoiXuLy");
            entity.Property(e => e.ThoiGianCapNhat)
                .HasColumnType("datetime")
                .HasColumnName("thoiGianCapNhat");
            entity.Property(e => e.ThoiGianTao)
                .HasColumnType("datetime")
                .HasColumnName("thoiGianTao");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("trangThai");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.PhanAnhs)
                .HasForeignKey(d => d.MaPhong)
                .HasConstraintName("FK__PhanAnh__maPhong__10566F31");

            entity.HasOne(d => d.MaSvNavigation).WithMany(p => p.PhanAnhs)
                .HasForeignKey(d => d.MaSv)
                .HasConstraintName("FK__PhanAnh__maSV__0F624AF8");

            entity.HasOne(d => d.NguoiXuLyNavigation).WithMany(p => p.PhanAnhs)
                .HasForeignKey(d => d.NguoiXuLy)
                .HasConstraintName("FK__PhanAnh__nguoiXu__114A936A");
        });

        modelBuilder.Entity<Phong>(entity =>
        {
            entity.HasKey(e => e.MaPhong).HasName("PK__Phong__4CD55E1078D78C40");

            entity.ToTable("Phong");

            entity.Property(e => e.MaPhong)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("maPhong");
            entity.Property(e => e.GioiTinh)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("gioiTinh");
            entity.Property(e => e.LoaiPhong)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("loaiPhong");
            entity.Property(e => e.SoLuongDangO).HasColumnName("soLuongDangO");
            entity.Property(e => e.SucChua).HasColumnName("sucChua");
            entity.Property(e => e.Tang).HasColumnName("tang");
            entity.Property(e => e.ToaNha)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("toaNha");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("trangThai");

            entity.HasOne(d => d.ToaNhaNavigation).WithMany(p => p.Phongs)
                .HasForeignKey(d => d.ToaNha)
                .HasConstraintName("FK__Phong__toaNha__7D439ABD");
        });

        modelBuilder.Entity<SinhVien>(entity =>
        {
            entity.HasKey(e => e.MaSv).HasName("PK__SinhVien__7A227A6433BC42FC");

            entity.ToTable("SinhVien");

            entity.Property(e => e.MaSv)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("maSV");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.HoTen)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("hoTen");
            entity.Property(e => e.Khoa)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("khoa");
            entity.Property(e => e.Lop)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("lop");
            entity.Property(e => e.MaTaiKhoan)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("maTaiKhoan");
            entity.Property(e => e.SoCmnd)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("soCMND");

            entity.HasOne(d => d.MaTaiKhoanNavigation).WithMany(p => p.SinhViens)
                .HasForeignKey(d => d.MaTaiKhoan)
                .HasConstraintName("FK__SinhVien__maTaiK__00200768");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTaiKhoan).HasName("PK__TaiKhoan__8FFF6A9DDD632658");

            entity.ToTable("TaiKhoan");

            entity.HasIndex(e => e.TenDangNhap, "UQ__TaiKhoan__59267D4A85ADB739").IsUnique();

            entity.Property(e => e.MaTaiKhoan)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("maTaiKhoan");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.MatKhauMh)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("matKhauMH");
            entity.Property(e => e.Sdt)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SDT");
            entity.Property(e => e.TenDangNhap)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("tenDangNhap");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("trangThai");
            entity.Property(e => e.VaiTro)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("vaiTro");

            entity.HasOne(d => d.VaiTroNavigation).WithMany(p => p.TaiKhoans)
                .HasForeignKey(d => d.VaiTro)
                .HasConstraintName("FK__TaiKhoan__vaiTro__72C60C4A");
        });

        modelBuilder.Entity<ThongBao>(entity =>
        {
            entity.HasKey(e => e.MaThongBao).HasName("PK__ThongBao__657CA539883D571E");

            entity.ToTable("ThongBao");

            entity.Property(e => e.MaThongBao)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("maThongBao");
            entity.Property(e => e.LoaiThongBao)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("loaiThongBao");
            entity.Property(e => e.NguoiNhan)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nguoiNhan");
            entity.Property(e => e.NoiDung)
                .HasColumnType("text")
                .HasColumnName("noiDung");
            entity.Property(e => e.ThoiGianGui)
                .HasColumnType("datetime")
                .HasColumnName("thoiGianGui");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("trangThai");

            entity.HasOne(d => d.NguoiNhanNavigation).WithMany(p => p.ThongBaos)
                .HasForeignKey(d => d.NguoiNhan)
                .HasConstraintName("FK__ThongBao__nguoiN__75A278F5");
        });

        modelBuilder.Entity<ToaNha>(entity =>
        {
            entity.HasKey(e => e.MaToaNha).HasName("PK__ToaNha__35A878347505E367");

            entity.ToTable("ToaNha");

            entity.Property(e => e.MaToaNha)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("maToaNha");
            entity.Property(e => e.DiaChi)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("diaChi");
            entity.Property(e => e.TenToaNha)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("tenToaNha");
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.MaVaiTro).HasName("PK__VaiTro__BFC88AB7DD30F5CB");

            entity.ToTable("VaiTro");

            entity.Property(e => e.MaVaiTro)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("maVaiTro");
            entity.Property(e => e.TenVaiTro)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("tenVaiTro");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
