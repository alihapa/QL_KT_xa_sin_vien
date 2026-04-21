using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QL_KT_xa_sin_vien.Migrations
{
    /// <inheritdoc />
    public partial class lan4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Agree",
                table: "HopDong",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DieuKhoanPdf",
                table: "HopDong",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Agree",
                table: "HopDong");

            migrationBuilder.DropColumn(
                name: "DieuKhoanPdf",
                table: "HopDong");
        }
    }
}
