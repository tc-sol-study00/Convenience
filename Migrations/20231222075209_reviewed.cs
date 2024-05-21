using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Convenience.Migrations
{
    /// <inheritdoc />
    public partial class reviewed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_shiire_jisseki",
                table: "shiire_jisseki");

            migrationBuilder.AddPrimaryKey(
                name: "PK_shiire_jisseki",
                table: "shiire_jisseki",
                columns: new[] { "chumon_code", "shiire_date", "seq_by_shiiredate", "shiire_saki_code", "shiire_prd_code" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_shiire_jisseki",
                table: "shiire_jisseki");

            migrationBuilder.AddPrimaryKey(
                name: "PK_shiire_jisseki",
                table: "shiire_jisseki",
                columns: new[] { "chumon_code", "shiire_date", "seq_by_shiiredate" });
        }
    }
}
