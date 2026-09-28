using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameVault.Data.Migrations
{
    public partial class DodateBeleske : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Beleske",
                table: "Igre",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Beleske",
                table: "Igre");
        }
    }
}
