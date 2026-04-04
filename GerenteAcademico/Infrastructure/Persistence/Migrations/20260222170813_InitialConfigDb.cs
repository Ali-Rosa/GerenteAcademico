using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GerenteAcademico.Migrations
{
    /// <inheritdoc />
    public partial class InitialConfigDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Academias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    LogoUrl = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    CadenaConexionPrincipal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    EmailContacto = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Pais = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Ciudad = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IdFiscal = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UrlSitioWeb = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    EsDemo = table.Column<bool>(type: "bit", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCreacion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    UsuarioModificacion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Academias", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Academias_Codigo",
                table: "Academias",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Academias_IdFiscal",
                table: "Academias",
                column: "IdFiscal",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Academias");
        }
    }
}
