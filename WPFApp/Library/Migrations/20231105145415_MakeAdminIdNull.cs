using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Migrations
{
    /// <inheritdoc />
    public partial class MakeAdminIdNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Admin_AdminId",
                table: "Logs");

            migrationBuilder.AlterColumn<int>(
                name: "AdminId",
                table: "Logs",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Admin_AdminId",
                table: "Logs",
                column: "AdminId",
                principalTable: "Admin",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Admin_AdminId",
                table: "Logs");

            migrationBuilder.AlterColumn<int>(
                name: "AdminId",
                table: "Logs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Admin_AdminId",
                table: "Logs",
                column: "AdminId",
                principalTable: "Admin",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
