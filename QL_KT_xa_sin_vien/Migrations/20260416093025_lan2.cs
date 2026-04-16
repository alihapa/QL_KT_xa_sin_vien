using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QL_KT_xa_sin_vien.Migrations
{
    /// <inheritdoc />
    public partial class lan2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NguoiGui",
                table: "ThongBao",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NguoiGui",
                table: "ThongBao");
        }
    }
}
