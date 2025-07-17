using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatBotApi.Migrations
{
    /// <inheritdoc />
    public partial class AjustandoRelcionamentoCLientID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Atendimentos_Clientes_ClienteId",
                table: "Atendimentos");

            migrationBuilder.AddForeignKey(
                name: "FK_Atendimentos_Clientes_ClienteId",
                table: "Atendimentos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Atendimentos_Clientes_ClienteId",
                table: "Atendimentos");

            migrationBuilder.AddForeignKey(
                name: "FK_Atendimentos_Clientes_ClienteId",
                table: "Atendimentos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
