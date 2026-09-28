using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameVault.Data.Migrations
{
    public partial class PocetnaBaza : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Igre",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Naziv = table.Column<string>(type: "TEXT", nullable: false),
                    Opis = table.Column<string>(type: "TEXT", nullable: true),
                    GodinaIzdanja = table.Column<int>(type: "INTEGER", nullable: true),
                    Developer = table.Column<string>(type: "TEXT", nullable: true),
                    Izdavac = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Ocena = table.Column<int>(type: "INTEGER", nullable: true),
                    BrojSati = table.Column<int>(type: "INTEGER", nullable: false),
                    Omiljena = table.Column<bool>(type: "INTEGER", nullable: false),
                    DatumDodavanja = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Igre", x => x.Id);
                    table.CheckConstraint("CK_Igre_BrojSati", "BrojSati >= 0");
                    table.CheckConstraint("CK_Igre_Naziv", "length(trim(Naziv)) > 0");
                    table.CheckConstraint("CK_Igre_Ocena", "Ocena IS NULL OR Ocena BETWEEN 1 AND 10");
                    table.CheckConstraint("CK_Igre_Status", "Status IN (0, 1, 2, 3)");
                });

            migrationBuilder.CreateTable(
                name: "Platforme",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Naziv = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Platforme", x => x.Id);
                    table.CheckConstraint("CK_Platforme_Naziv", "length(trim(Naziv)) > 0");
                });

            migrationBuilder.CreateTable(
                name: "Zanrovi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Naziv = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zanrovi", x => x.Id);
                    table.CheckConstraint("CK_Zanrovi_Naziv", "length(trim(Naziv)) > 0");
                });

            migrationBuilder.CreateTable(
                name: "IgraPlatforma",
                columns: table => new
                {
                    IgreId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlatformeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IgraPlatforma", x => new { x.IgreId, x.PlatformeId });
                    table.ForeignKey(
                        name: "FK_IgraPlatforma_Igre_IgreId",
                        column: x => x.IgreId,
                        principalTable: "Igre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IgraPlatforma_Platforme_PlatformeId",
                        column: x => x.PlatformeId,
                        principalTable: "Platforme",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IgraZanr",
                columns: table => new
                {
                    IgreId = table.Column<int>(type: "INTEGER", nullable: false),
                    ZanroviId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IgraZanr", x => new { x.IgreId, x.ZanroviId });
                    table.ForeignKey(
                        name: "FK_IgraZanr_Igre_IgreId",
                        column: x => x.IgreId,
                        principalTable: "Igre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IgraZanr_Zanrovi_ZanroviId",
                        column: x => x.ZanroviId,
                        principalTable: "Zanrovi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IgraPlatforma_PlatformeId",
                table: "IgraPlatforma",
                column: "PlatformeId");

            migrationBuilder.CreateIndex(
                name: "IX_IgraZanr_ZanroviId",
                table: "IgraZanr",
                column: "ZanroviId");

            migrationBuilder.CreateIndex(
                name: "IX_Platforme_Naziv",
                table: "Platforme",
                column: "Naziv",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Zanrovi_Naziv",
                table: "Zanrovi",
                column: "Naziv",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IgraPlatforma");

            migrationBuilder.DropTable(
                name: "IgraZanr");

            migrationBuilder.DropTable(
                name: "Platforme");

            migrationBuilder.DropTable(
                name: "Igre");

            migrationBuilder.DropTable(
                name: "Zanrovi");
        }
    }
}
