using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QL_KT_xa_sin_vien.Models;

public partial class QLSinhVienContext : DbContext
{
    public QLSinhVienContext()
    {
    }

    public QLSinhVienContext(DbContextOptions<QLSinhVienContext> options)
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

    public virtual DbSet<DonGia> DonGias { get; set; }

    public virtual DbSet<ToaNha> ToaNhas { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }
    public virtual DbSet<DieuKhoan> DieuKhoans { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=QL_Sinh_vien;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Giuong>(entity =>
        {
            entity.HasKey(e => e.MaGiuong);

            entity.ToTable("Giuong");

            entity.Property(e => e.MaGiuong)
                .HasMaxLength(50)
                .HasColumnName("maGiuong");
            entity.Property(e => e.MaPhong)
                .HasMaxLength(50)
                .HasColumnName("maPhong");
            entity.Property(e => e.OccupiedBy)
                .HasMaxLength(50)
                .HasColumnName("occupiedBy");
            entity.Property(e => e.SoGiuong)
                .HasMaxLength(20)
                .HasColumnName("soGiuong");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasColumnName("trangThai");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.Giuongs)
                .HasForeignKey(d => d.MaPhong)
                .HasConstraintName("FK_Giuong_Phong");

            entity.HasOne(d => d.OccupiedByNavigation).WithMany(p => p.Giuongs)
                .HasForeignKey(d => d.OccupiedBy)
                .HasConstraintName("FK_Giuong_SinhVien");
        });

        modelBuilder.Entity<DieuKhoan>(entity =>
        {
            entity.HasKey(e => e.MaDieuKhoan);
            entity.ToTable("DieuKhoan");
            entity.Property(e => e.MaDieuKhoan).HasMaxLength(50).HasColumnName("maDieuKhoan");
            entity.Property(e => e.FilePath).HasMaxLength(500).HasColumnName("filePath");
            entity.Property(e => e.OriginalFileName).HasMaxLength(500).HasColumnName("originalFileName");
            entity.Property(e => e.UploadedBy).HasMaxLength(50).HasColumnName("uploadedBy");
            entity.Property(e => e.UploadedAt).HasColumnType("datetime").HasColumnName("uploadedAt");
        });

        modelBuilder.Entity<DonGia>(entity =>
        {
            entity.HasKey(e => e.MaDonGia);
            entity.ToTable("DonGia");
            entity.Property(e => e.MaDonGia).HasMaxLength(50).HasColumnName("maDonGia");
            entity.Property(e => e.DonGiaDien).HasColumnType("decimal(12,2)").HasColumnName("donGiaDien");
            entity.Property(e => e.DonGiaNuoc).HasColumnType("decimal(12,2)").HasColumnName("donGiaNuoc");
            entity.Property(e => e.DonGiaPhong).HasColumnType("decimal(12,2)").HasColumnName("donGiaPhong");
            entity.Property(e => e.DienUsageDefault).HasColumnType("decimal(12,2)").HasColumnName("dienUsageDefault");
            entity.Property(e => e.NuocUsageDefault).HasColumnType("decimal(12,2)").HasColumnName("nuocUsageDefault");
            entity.Property(e => e.NgayHieuLuc).HasColumnType("datetime").HasColumnName("ngayHieuLuc");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon);

            entity.ToTable("HoaDon");

            entity.Property(e => e.MaHoaDon)
                .HasMaxLength(50)
                .HasColumnName("maHoaDon");
            entity.Property(e => e.MaHopDong)
                .HasMaxLength(50)
                .HasColumnName("maHopDong");
            entity.Property(e => e.MaSv)
                .HasMaxLength(50)
                .HasColumnName("maSV");
            entity.Property(e => e.NgayXuat)
                .HasColumnType("datetime")
                .HasColumnName("ngayXuat");
            entity.Property(e => e.SoTien)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("soTien");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasColumnName("trangThai");

            entity.HasOne(d => d.MaHopDongNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaHopDong)
                .HasConstraintName("FK_HoaDon_HopDong");

            entity.HasOne(d => d.MaSvNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaSv)
                .HasConstraintName("FK_HoaDon_SinhVien");
        });

        modelBuilder.Entity<HopDong>(entity =>
        {
            entity.HasKey(e => e.MaHopDong);

            entity.ToTable("HopDong");

            entity.Property(e => e.MaHopDong)
                .HasMaxLength(50)
                .HasColumnName("maHopDong");
            entity.Property(e => e.DieuKhoan).HasColumnName("dieuKhoan");
            entity.Property(e => e.MaGiuong)
                .HasMaxLength(50)
                .HasColumnName("maGiuong");
            entity.Property(e => e.MaPhong)
                .HasMaxLength(50)
                .HasColumnName("maPhong");
            entity.Property(e => e.MaSv)
                .HasMaxLength(50)
                .HasColumnName("maSV");
            entity.Property(e => e.NgayBatDau).HasColumnName("ngayBatDau");
            entity.Property(e => e.NgayKetThuc).HasColumnName("ngayKetThuc");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasColumnName("trangThai");

            entity.HasOne(d => d.MaGiuongNavigation).WithMany(p => p.HopDongs)
                .HasForeignKey(d => d.MaGiuong)
                .HasConstraintName("FK_HopDong_Giuong");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.HopDongs)
                .HasForeignKey(d => d.MaPhong)
                .HasConstraintName("FK_HopDong_Phong");

            entity.HasOne(d => d.MaSvNavigation).WithMany(p => p.HopDongs)
                .HasForeignKey(d => d.MaSv)
                .HasConstraintName("FK_HopDong_SinhVien");
        });

        modelBuilder.Entity<NhatKy>(entity =>
        {
            entity.HasKey(e => e.MaLog);

            entity.ToTable("NhatKy");

            entity.Property(e => e.MaLog)
                .HasMaxLength(50)
                .HasColumnName("maLog");
            entity.Property(e => e.DoiTuong)
                .HasMaxLength(100)
                .HasColumnName("doiTuong");
            entity.Property(e => e.GiaTriSau)
                .HasMaxLength(100)
                .HasColumnName("giaTriSau");
            entity.Property(e => e.GiaTriTruoc)
                .HasMaxLength(100)
                .HasColumnName("giaTriTruoc");
            entity.Property(e => e.HanhDong)
                .HasMaxLength(100)
                .HasColumnName("hanhDong");
            entity.Property(e => e.NguoiThucHien)
                .HasMaxLength(50)
                .HasColumnName("nguoiThucHien");
            entity.Property(e => e.ThoiGian)
                .HasColumnType("datetime")
                .HasColumnName("thoiGian");

            entity.HasOne(d => d.NguoiThucHienNavigation).WithMany(p => p.NhatKies)
                .HasForeignKey(d => d.NguoiThucHien)
                .HasConstraintName("FK_NhatKy_TaiKhoan");
        });

        modelBuilder.Entity<PhanAnh>(entity =>
        {
            entity.HasKey(e => e.MaPhanAnh);

            entity.ToTable("PhanAnh");

            entity.Property(e => e.MaPhanAnh)
                .HasMaxLength(50)
                .HasColumnName("maPhanAnh");
            entity.Property(e => e.MaPhong)
                .HasMaxLength(50)
                .HasColumnName("maPhong");
            entity.Property(e => e.MaSv)
                .HasMaxLength(50)
                .HasColumnName("maSV");
            entity.Property(e => e.MoTa).HasColumnName("moTa");
            entity.Property(e => e.MucDoUuTien)
                .HasMaxLength(50)
                .HasColumnName("mucDoUuTien");
            entity.Property(e => e.NguoiXuLy)
                .HasMaxLength(50)
                .HasColumnName("nguoiXuLy");
            entity.Property(e => e.ThoiGianCapNhat)
                .HasColumnType("datetime")
                .HasColumnName("thoiGianCapNhat");
            entity.Property(e => e.ThoiGianTao)
                .HasColumnType("datetime")
                .HasColumnName("thoiGianTao");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasColumnName("trangThai");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.PhanAnhs)
                .HasForeignKey(d => d.MaPhong)
                .HasConstraintName("FK_PhanAnh_Phong");

            entity.HasOne(d => d.MaSvNavigation).WithMany(p => p.PhanAnhs)
                .HasForeignKey(d => d.MaSv)
                .HasConstraintName("FK_PhanAnh_SinhVien");

            entity.HasOne(d => d.NguoiXuLyNavigation).WithMany(p => p.PhanAnhs)
                .HasForeignKey(d => d.NguoiXuLy)
                .HasConstraintName("FK_PhanAnh_TaiKhoan");
        });

        modelBuilder.Entity<Phong>(entity =>
        {
            entity.HasKey(e => e.MaPhong);

            entity.ToTable("Phong");

            entity.Property(e => e.MaPhong)
                .HasMaxLength(50)
                .HasColumnName("maPhong");
            entity.Property(e => e.GioiTinh)
                .HasMaxLength(10)
                .HasColumnName("gioiTinh");
            entity.Property(e => e.LoaiPhong)
                .HasMaxLength(50)
                .HasColumnName("loaiPhong");
            entity.Property(e => e.MaToaNha)
                .HasMaxLength(50)
                .HasColumnName("maToaNha");
            entity.Property(e => e.SoLuongDangO).HasColumnName("soLuongDangO");
            entity.Property(e => e.SucChua).HasColumnName("sucChua");
            entity.Property(e => e.Tang).HasColumnName("tang");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasColumnName("trangThai");

            entity.HasOne(d => d.MaToaNhaNavigation).WithMany(p => p.Phongs)
                .HasForeignKey(d => d.MaToaNha)
                .HasConstraintName("FK_Phong_ToaNha");
        });

        modelBuilder.Entity<SinhVien>(entity =>
        {
            entity.HasKey(e => e.MaSv);

            entity.ToTable("SinhVien");

            entity.Property(e => e.MaSv)
                .HasMaxLength(50)
                .HasColumnName("maSV");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.HoTen)
                .HasMaxLength(100)
                .HasColumnName("hoTen");
            entity.Property(e => e.Khoa)
                .HasMaxLength(50)
                .HasColumnName("khoa");
            entity.Property(e => e.Lop)
                .HasMaxLength(50)
                .HasColumnName("lop");
            entity.Property(e => e.MaTaiKhoan)
                .HasMaxLength(50)
                .HasColumnName("maTaiKhoan");
            entity.Property(e => e.SoCmnd)
                .HasMaxLength(20)
                .HasColumnName("soCMND");

            entity.HasOne(d => d.MaTaiKhoanNavigation).WithMany(p => p.SinhViens)
                .HasForeignKey(d => d.MaTaiKhoan)
                .HasConstraintName("FK_SinhVien_TaiKhoan");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTaiKhoan);

            entity.ToTable("TaiKhoan");

            entity.HasIndex(e => e.TenDangNhap, "UQ_TaiKhoan_tenDangNhap").IsUnique();

            entity.Property(e => e.MaTaiKhoan)
                .HasMaxLength(50)
                .HasColumnName("maTaiKhoan");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.MatKhauMh)
                .HasMaxLength(255)
                .HasColumnName("matKhauMH");
            entity.Property(e => e.Sdt)
                .HasMaxLength(20)
                .HasColumnName("SDT");
            entity.Property(e => e.TenDangNhap)
                .HasMaxLength(100)
                .HasColumnName("tenDangNhap");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasColumnName("trangThai");
            entity.Property(e => e.VaiTro)
                .HasMaxLength(50)
                .HasColumnName("vaiTro");

            entity.HasOne(d => d.VaiTroNavigation).WithMany(p => p.TaiKhoans)
                .HasForeignKey(d => d.VaiTro)
                .HasConstraintName("FK_TaiKhoan_VaiTro");
        });

        modelBuilder.Entity<ThongBao>(entity =>
        {
            entity.HasKey(e => e.MaThongBao);

            entity.ToTable("ThongBao");

            entity.Property(e => e.MaThongBao)
                .HasMaxLength(50)
                .HasColumnName("maThongBao");
            entity.Property(e => e.LoaiThongBao)
                .HasMaxLength(50)
                .HasColumnName("loaiThongBao");
            entity.Property(e => e.NguoiNhan)
                .HasMaxLength(50)
                .HasColumnName("nguoiNhan");
            entity.Property(e => e.NoiDung).HasColumnName("noiDung");
            entity.Property(e => e.ThoiGianGui)
                .HasColumnType("datetime")
                .HasColumnName("thoiGianGui");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasColumnName("trangThai");

            entity.HasOne(d => d.NguoiNhanNavigation).WithMany(p => p.ThongBaos)
                .HasForeignKey(d => d.NguoiNhan)
                .HasConstraintName("FK_ThongBao_TaiKhoan");
        });

        modelBuilder.Entity<ToaNha>(entity =>
        {
            entity.HasKey(e => e.MaToaNha);

            entity.ToTable("ToaNha");

            entity.Property(e => e.MaToaNha)
                .HasMaxLength(50)
                .HasColumnName("maToaNha");
            entity.Property(e => e.DiaChi)
                .HasMaxLength(255)
                .HasColumnName("diaChi");
            entity.Property(e => e.TenToaNha)
                .HasMaxLength(100)
                .HasColumnName("tenToaNha");
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.MaVaiTro);

            entity.ToTable("VaiTro");

            entity.Property(e => e.MaVaiTro)
                .HasMaxLength(50)
                .HasColumnName("maVaiTro");
            entity.Property(e => e.QuyenHan)
                .HasMaxLength(50)
                .HasColumnName("quyenHan");
            entity.Property(e => e.TenVaiTro)
                .HasMaxLength(100)
                .HasColumnName("tenVaiTro");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
