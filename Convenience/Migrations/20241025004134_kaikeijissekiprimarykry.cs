using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
#pragma warning disable CS8981

namespace Convenience.Migrations
{
    /// <inheritdoc />
    public partial class kaikeijissekiprimarykry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_kaikei_jisseki",
                table: "kaikei_jisseki");

            migrationBuilder.AddPrimaryKey(
                name: "PK_kaikei_jisseki",
                table: "kaikei_jisseki",
                columns: new[] { "uriage_datetimeid", "shohin_code" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_kaikei_jisseki",
                table: "kaikei_jisseki");

            migrationBuilder.AddPrimaryKey(
                name: "PK_kaikei_jisseki",
                table: "kaikei_jisseki",
                columns: new[] { "uriage_datetimeid", "shohin_code", "uriage_datetime" });
        }
    }
}
