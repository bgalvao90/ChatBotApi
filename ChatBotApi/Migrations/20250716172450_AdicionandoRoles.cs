using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatBotApi.Migrations
{
    /// <inheritdoc />
    public partial class AdicionandoRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Usuario",
                table: "Atendentes");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Usuarios",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "Atendimentos",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Atendimentos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observacao",
                table: "Atendimentos",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Titulo",
                table: "Atendimentos",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "UserModelId",
                table: "Atendentes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserModelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clientes_Usuarios_UserModelId",
                        column: x => x.UserModelId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Atendimentos_ClienteId",
                table: "Atendimentos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Atendentes_UserModelId",
                table: "Atendentes",
                column: "UserModelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_UserModelId",
                table: "Clientes",
                column: "UserModelId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Atendentes_Usuarios_UserModelId",
                table: "Atendentes",
                column: "UserModelId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Atendimentos_Clientes_ClienteId",
                table: "Atendimentos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Atendentes_Usuarios_UserModelId",
                table: "Atendentes");

            migrationBuilder.DropForeignKey(
                name: "FK_Atendimentos_Clientes_ClienteId",
                table: "Atendimentos");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Atendimentos_ClienteId",
                table: "Atendimentos");

            migrationBuilder.DropIndex(
                name: "IX_Atendentes_UserModelId",
                table: "Atendentes");

            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Atendimentos");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Atendimentos");

            migrationBuilder.DropColumn(
                name: "Observacao",
                table: "Atendimentos");

            migrationBuilder.DropColumn(
                name: "Titulo",
                table: "Atendimentos");

            migrationBuilder.DropColumn(
                name: "UserModelId",
                table: "Atendentes");

            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "Usuarios",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Usuario",
                table: "Atendentes",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
