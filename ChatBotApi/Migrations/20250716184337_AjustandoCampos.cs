using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatBotApi.Migrations
{
    /// <inheritdoc />
    public partial class AjustandoCampos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IdUsuarioExterno",
                table: "Atendimentos",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Canal",
                table: "Atendimentos",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Atendimentos",
                keyColumn: "IdUsuarioExterno",
                keyValue: null,
                column: "IdUsuarioExterno",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "IdUsuarioExterno",
                table: "Atendimentos",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Atendimentos",
                keyColumn: "Canal",
                keyValue: null,
                column: "Canal",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Canal",
                table: "Atendimentos",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
