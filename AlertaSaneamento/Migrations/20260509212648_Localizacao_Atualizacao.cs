using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlertaSaneamento.Migrations
{
    /// <inheritdoc />
    public partial class Localizacao_Atualizacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Endereco",
                table: "Alertas",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Alertas",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Alertas",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Endereco",
                table: "Alertas");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Alertas");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Alertas");
        }
    }
}
