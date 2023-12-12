using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysAdminApp.Migrations
{
    /// <inheritdoc />
    public partial class SetNullAsTypeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HelperInfo_HelperType_TypeId",
                table: "HelperInfo");

            migrationBuilder.AlterColumn<int>(
                name: "TypeId",
                table: "HelperInfo",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

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

            migrationBuilder.AlterColumn<int>(
                name: "TypeId",
                table: "HelperInfo",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HelperInfo_HelperType_TypeId",
                table: "HelperInfo",
                column: "TypeId",
                principalTable: "HelperType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
