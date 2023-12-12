using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Library.Migrations
{
    /// <inheritdoc />
    public partial class addForeignTypeForHelpers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "HelperInfo");

            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "HelperInfo",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HelperType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelperType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HelperInfo_TypeId",
                table: "HelperInfo",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_HelperInfo_HelperType_TypeId",
                table: "HelperInfo",
                column: "TypeId",
                principalTable: "HelperType",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HelperInfo_HelperType_TypeId",
                table: "HelperInfo");

            migrationBuilder.DropTable(
                name: "HelperType");

            migrationBuilder.DropIndex(
                name: "IX_HelperInfo_TypeId",
                table: "HelperInfo");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "HelperInfo");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "HelperInfo",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
