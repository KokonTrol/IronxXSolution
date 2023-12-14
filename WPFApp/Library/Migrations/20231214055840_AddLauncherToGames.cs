using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Migrations
{
    /// <inheritdoc />
    public partial class AddLauncherToGames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LauncherId",
                table: "Games",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_LauncherId",
                table: "Games",
                column: "LauncherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_LaunchersInfos_LauncherId",
                table: "Games",
                column: "LauncherId",
                principalTable: "LaunchersInfos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_LaunchersInfos_LauncherId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_LauncherId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "LauncherId",
                table: "Games");
        }
    }
}
