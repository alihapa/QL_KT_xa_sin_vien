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

    public virtual DbSet<ToaNha> ToaNhas { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=QL_Sinh_vien;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Giuong>(entity =>
        {
            entity.HasKey(e => e.MaGiuong).HasName("PK__Giuong__1D39A54C871BDD00");

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
                .HasConstraintName("FK__Giuong__maPhong__2739D489");

            entity.HasOne(d => d.OccupiedByNavigation).WithMany(p => p.Giuongs)
                .HasForeignKey(d => d.OccupiedBy)
                .HasConstraintName("FK__Giuong__occupied__282DF8C2");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon).HasName("PK__HoaDon__026B4D9A7EAADDCE");

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
                .HasConstraintName("FK__HoaDon__maHopDon__2FCF1A8A");

            entity.HasOne(d => d.MaSvNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaSv)
                .HasConstraintName("FK__HoaDon__maSV__30C33EC3");
        });

        modelBuilder.Entity<HopDong>(entity =>
        {
            entity.HasKey(e => e.MaHopDong).HasName("PK__HopDong__429F83D99317F2D4");

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
                .HasConstraintName("FK__HopDong__maGiuon__2CF2ADDF");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.HopDongs)
                .HasForeignKey(d => d.MaPhong)
                .HasConstraintName("FK__HopDong__maPhong__2BFE89A6");

            entity.HasOne(d => d.MaSvNavigation).WithMany(p => p.HopDongs)
                .HasForeignKey(d => d.MaSv)
                .HasConstraintName("FK__HopDong__maSV__2B0A656D");
        });

        modelBuilder.Entity<NhatKy>(entity =>
        {
            entity.HasKey(e => e.MaLog).HasName("PK__NhatKy__261ECAEA6CEC6600");

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
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("giaTriSau");
            entity.Property(e => e.GiaTriTruoc)
                .HasMaxLength(100)
                .IsUnicode(false)
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
                .HasConstraintName("FK__NhatKy__nguoiThu__1CBC4616");
        });

        modelBuilder.Entity<PhanAnh>(entity =>
        {
            entity.HasKey(e => e.MaPhanAnh).HasName("PK__PhanAnh__4028347105BFE5F0");

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
                .HasMaxLength(5000)
                .IsUnicode(false)
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
                .HasConstraintName("FK__PhanAnh__maPhong__3493CFA7");

            entity.HasOne(d => d.MaSvNavigation).WithMany(p => p.PhanAnhs)
                .HasForeignKey(d => d.MaSv)
                .HasConstraintName("FK__PhanAnh__maSV__339FAB6E");

            entity.HasOne(d => d.NguoiXuLyNavigation).WithMany(p => p.PhanAnhs)
                .HasForeignKey(d => d.NguoiXuLy)
                .HasConstraintName("FK__PhanAnh__nguoiXu__3587F3E0");
        });

        modelBuilder.Entity<Phong>(entity =>
        {
            entity.HasKey(e => e.MaPhong).HasName("PK__Phong__4CD55E109AAB6959");

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
                .HasConstraintName("FK__Phong__toaNha__2180FB33");
        });

        modelBuilder.Entity<SinhVien>(entity =>
        {
            entity.HasKey(e => e.MaSv).HasName("PK__SinhVien__7A227A6410C7D164");

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
                .HasConstraintName("FK__SinhVien__maTaiK__245D67DE");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTaiKhoan).HasName("PK__TaiKhoan__8FFF6A9D15F201F9");

            entity.ToTable("TaiKhoan");

            entity.HasIndex(e => e.TenDangNhap, "UQ__TaiKhoan__59267D4A0C6BCA01").IsUnique();

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
                .HasConstraintName("FK__TaiKhoan__vaiTro__17036CC0");
        });

        modelBuilder.Entity<ThongBao>(entity =>
        {
            entity.HasKey(e => e.MaThongBao).HasName("PK__ThongBao__657CA539BBDBBEAB");

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
                .HasConstraintName("FK__ThongBao__nguoiN__19DFD96B");
        });

        modelBuilder.Entity<ToaNha>(entity =>
        {
            entity.HasKey(e => e.MaToaNha).HasName("PK__ToaNha__35A87834E94250F9");

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
            entity.HasKey(e => e.MaVaiTro).HasName("PK__VaiTro__BFC88AB73053790E");

            entity.ToTable("VaiTro");

            entity.Property(e => e.MaVaiTro)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("maVaiTro");
            entity.Property(e => e.QuyenHan)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("quyenHan");
            entity.Property(e => e.TenVaiTro)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("tenVaiTro");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
