using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QL_KT_xa_sin_vien.Migrations
{
    /// <inheritdoc />
    public partial class lan7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DieuKhoan",
                columns: table => new
                {
                    maDieuKhoan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    filePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    originalFileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    uploadedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    uploadedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DieuKhoan", x => x.maDieuKhoan);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DieuKhoan");
        }
    }
}
