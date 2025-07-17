using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatBotApi.Migrations
{
    /// <inheritdoc />
    public partial class AjustandoCampoAtendenteId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Atendimentos_Clientes_ClienteId",
                table: "Atendimentos");

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "MensagemHistorico",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ClienteId",
                table: "Atendimentos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Atendimentos_Clientes_ClienteId",
                table: "Atendimentos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Atendimentos_Clientes_ClienteId",
                table: "Atendimentos");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "MensagemHistorico");

            migrationBuilder.AlterColumn<int>(
                name: "ClienteId",
                table: "Atendimentos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Atendimentos_Clientes_ClienteId",
                table: "Atendimentos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");
        }
    }
}
