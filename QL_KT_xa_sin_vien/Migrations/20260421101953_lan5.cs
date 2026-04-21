using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QL_KT_xa_sin_vien.Migrations
{
    /// <inheritdoc />
    public partial class lan5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ThoiHan",
                table: "TaiKhoan",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MediaPath",
                table: "PhanAnh",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MediaType",
                table: "PhanAnh",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DonGia",
                columns: table => new
                {
                    maDonGia = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    donGiaDien = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    donGiaNuoc = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    donGiaPhong = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    dienUsageDefault = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    nuocUsageDefault = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    ngayHieuLuc = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonGia", x => x.maDonGia);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DonGia");

            migrationBuilder.DropColumn(
                name: "ThoiHan",
                table: "TaiKhoan");

            migrationBuilder.DropColumn(
                name: "MediaPath",
                table: "PhanAnh");

            migrationBuilder.DropColumn(
                name: "MediaType",
                table: "PhanAnh");
        }
    }
}
