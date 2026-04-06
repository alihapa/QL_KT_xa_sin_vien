using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QL_KT_xa_sin_vien.Migrations
{
    /// <inheritdoc />
    public partial class lan1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ToaNha",
                columns: table => new
                {
                    maToaNha = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    tenToaNha = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    diaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToaNha", x => x.maToaNha);
                });

            migrationBuilder.CreateTable(
                name: "VaiTro",
                columns: table => new
                {
                    maVaiTro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    tenVaiTro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    quyenHan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaiTro", x => x.maVaiTro);
                });

            migrationBuilder.CreateTable(
                name: "Phong",
                columns: table => new
                {
                    maPhong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    maToaNha = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    tang = table.Column<int>(type: "int", nullable: true),
                    loaiPhong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    sucChua = table.Column<int>(type: "int", nullable: true),
                    soLuongDangO = table.Column<int>(type: "int", nullable: true),
                    gioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    trangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phong", x => x.maPhong);
                    table.ForeignKey(
                        name: "FK_Phong_ToaNha",
                        column: x => x.maToaNha,
                        principalTable: "ToaNha",
                        principalColumn: "maToaNha");
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoan",
                columns: table => new
                {
                    maTaiKhoan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    tenDangNhap = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    matKhauMH = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SDT = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    vaiTro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    trangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoan", x => x.maTaiKhoan);
                    table.ForeignKey(
                        name: "FK_TaiKhoan_VaiTro",
                        column: x => x.vaiTro,
                        principalTable: "VaiTro",
                        principalColumn: "maVaiTro");
                });

            migrationBuilder.CreateTable(
                name: "NhatKy",
                columns: table => new
                {
                    maLog = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    nguoiThucHien = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    hanhDong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    doiTuong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    giaTriTruoc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    giaTriSau = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    thoiGian = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhatKy", x => x.maLog);
                    table.ForeignKey(
                        name: "FK_NhatKy_TaiKhoan",
                        column: x => x.nguoiThucHien,
                        principalTable: "TaiKhoan",
                        principalColumn: "maTaiKhoan");
                });

            migrationBuilder.CreateTable(
                name: "SinhVien",
                columns: table => new
                {
                    maSV = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    hoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    lop = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    khoa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    soCMND = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    maTaiKhoan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SinhVien", x => x.maSV);
                    table.ForeignKey(
                        name: "FK_SinhVien_TaiKhoan",
                        column: x => x.maTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "maTaiKhoan");
                });

            migrationBuilder.CreateTable(
                name: "ThongBao",
                columns: table => new
                {
                    maThongBao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    nguoiNhan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    loaiThongBao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    noiDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    trangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    thoiGianGui = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongBao", x => x.maThongBao);
                    table.ForeignKey(
                        name: "FK_ThongBao_TaiKhoan",
                        column: x => x.nguoiNhan,
                        principalTable: "TaiKhoan",
                        principalColumn: "maTaiKhoan");
                });

            migrationBuilder.CreateTable(
                name: "Giuong",
                columns: table => new
                {
                    maGiuong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    maPhong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    soGiuong = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    occupiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    trangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Giuong", x => x.maGiuong);
                    table.ForeignKey(
                        name: "FK_Giuong_Phong",
                        column: x => x.maPhong,
                        principalTable: "Phong",
                        principalColumn: "maPhong");
                    table.ForeignKey(
                        name: "FK_Giuong_SinhVien",
                        column: x => x.occupiedBy,
                        principalTable: "SinhVien",
                        principalColumn: "maSV");
                });

            migrationBuilder.CreateTable(
                name: "PhanAnh",
                columns: table => new
                {
                    maPhanAnh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    maSV = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    maPhong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    moTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    mucDoUuTien = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    trangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    nguoiXuLy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    thoiGianTao = table.Column<DateTime>(type: "datetime", nullable: true),
                    thoiGianCapNhat = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanAnh", x => x.maPhanAnh);
                    table.ForeignKey(
                        name: "FK_PhanAnh_Phong",
                        column: x => x.maPhong,
                        principalTable: "Phong",
                        principalColumn: "maPhong");
                    table.ForeignKey(
                        name: "FK_PhanAnh_SinhVien",
                        column: x => x.maSV,
                        principalTable: "SinhVien",
                        principalColumn: "maSV");
                    table.ForeignKey(
                        name: "FK_PhanAnh_TaiKhoan",
                        column: x => x.nguoiXuLy,
                        principalTable: "TaiKhoan",
                        principalColumn: "maTaiKhoan");
                });

            migrationBuilder.CreateTable(
                name: "HopDong",
                columns: table => new
                {
                    maHopDong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    maSV = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    maPhong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    maGiuong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ngayBatDau = table.Column<DateOnly>(type: "date", nullable: true),
                    ngayKetThuc = table.Column<DateOnly>(type: "date", nullable: true),
                    trangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    dieuKhoan = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HopDong", x => x.maHopDong);
                    table.ForeignKey(
                        name: "FK_HopDong_Giuong",
                        column: x => x.maGiuong,
                        principalTable: "Giuong",
                        principalColumn: "maGiuong");
                    table.ForeignKey(
                        name: "FK_HopDong_Phong",
                        column: x => x.maPhong,
                        principalTable: "Phong",
                        principalColumn: "maPhong");
                    table.ForeignKey(
                        name: "FK_HopDong_SinhVien",
                        column: x => x.maSV,
                        principalTable: "SinhVien",
                        principalColumn: "maSV");
                });

            migrationBuilder.CreateTable(
                name: "HoaDon",
                columns: table => new
                {
                    maHoaDon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    maHopDong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    maSV = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    soTien = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    trangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ngayXuat = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDon", x => x.maHoaDon);
                    table.ForeignKey(
                        name: "FK_HoaDon_HopDong",
                        column: x => x.maHopDong,
                        principalTable: "HopDong",
                        principalColumn: "maHopDong");
                    table.ForeignKey(
                        name: "FK_HoaDon_SinhVien",
                        column: x => x.maSV,
                        principalTable: "SinhVien",
                        principalColumn: "maSV");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Giuong_maPhong",
                table: "Giuong",
                column: "maPhong");

            migrationBuilder.CreateIndex(
                name: "IX_Giuong_occupiedBy",
                table: "Giuong",
                column: "occupiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_maHopDong",
                table: "HoaDon",
                column: "maHopDong");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_maSV",
                table: "HoaDon",
                column: "maSV");

            migrationBuilder.CreateIndex(
                name: "IX_HopDong_maGiuong",
                table: "HopDong",
                column: "maGiuong");

            migrationBuilder.CreateIndex(
                name: "IX_HopDong_maPhong",
                table: "HopDong",
                column: "maPhong");

            migrationBuilder.CreateIndex(
                name: "IX_HopDong_maSV",
                table: "HopDong",
                column: "maSV");

            migrationBuilder.CreateIndex(
                name: "IX_NhatKy_nguoiThucHien",
                table: "NhatKy",
                column: "nguoiThucHien");

            migrationBuilder.CreateIndex(
                name: "IX_PhanAnh_maPhong",
                table: "PhanAnh",
                column: "maPhong");

            migrationBuilder.CreateIndex(
                name: "IX_PhanAnh_maSV",
                table: "PhanAnh",
                column: "maSV");

            migrationBuilder.CreateIndex(
                name: "IX_PhanAnh_nguoiXuLy",
                table: "PhanAnh",
                column: "nguoiXuLy");

            migrationBuilder.CreateIndex(
                name: "IX_Phong_maToaNha",
                table: "Phong",
                column: "maToaNha");

            migrationBuilder.CreateIndex(
                name: "IX_SinhVien_maTaiKhoan",
                table: "SinhVien",
                column: "maTaiKhoan");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_vaiTro",
                table: "TaiKhoan",
                column: "vaiTro");

            migrationBuilder.CreateIndex(
                name: "UQ_TaiKhoan_tenDangNhap",
                table: "TaiKhoan",
                column: "tenDangNhap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThongBao_nguoiNhan",
                table: "ThongBao",
                column: "nguoiNhan");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HoaDon");

            migrationBuilder.DropTable(
                name: "NhatKy");

            migrationBuilder.DropTable(
                name: "PhanAnh");

            migrationBuilder.DropTable(
                name: "ThongBao");

            migrationBuilder.DropTable(
                name: "HopDong");

            migrationBuilder.DropTable(
                name: "Giuong");

            migrationBuilder.DropTable(
                name: "Phong");

            migrationBuilder.DropTable(
                name: "SinhVien");

            migrationBuilder.DropTable(
                name: "ToaNha");

            migrationBuilder.DropTable(
                name: "TaiKhoan");

            migrationBuilder.DropTable(
                name: "VaiTro");
        }
    }
}
