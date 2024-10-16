using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Convenience.Migrations
{
    /// <inheritdoc />
    public partial class zeiritsu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "shohi_zeiritsu_gaisyoku",
                table: "shohin_master",
                newName: "shohi_zeiritsu_eatin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "shohi_zeiritsu_eatin",
                table: "shohin_master",
                newName: "shohi_zeiritsu_gaisyoku");
        }
    }
}
