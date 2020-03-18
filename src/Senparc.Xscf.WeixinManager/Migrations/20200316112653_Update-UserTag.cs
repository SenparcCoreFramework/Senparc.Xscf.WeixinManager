using Microsoft.EntityFrameworkCore.Migrations;

namespace Senparc.Xscf.WeixinManager.Migrations
{
    public partial class UpdateUserTag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "WeixinManager_UserTag",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "WeixinManager_UserTag",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "MpAccountId",
                table: "WeixinManager_UserTag",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TagId",
                table: "WeixinManager_UserTag",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WeixinManager_UserTag_MpAccountId",
                table: "WeixinManager_UserTag",
                column: "MpAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK__UserTag__MpAccountId",
                table: "WeixinManager_UserTag",
                column: "MpAccountId",
                principalTable: "WeixinManager_MpAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__UserTag__MpAccountId",
                table: "WeixinManager_UserTag");

            migrationBuilder.DropIndex(
                name: "IX_WeixinManager_UserTag_MpAccountId",
                table: "WeixinManager_UserTag");

            migrationBuilder.DropColumn(
                name: "MpAccountId",
                table: "WeixinManager_UserTag");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "WeixinManager_UserTag");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "WeixinManager_UserTag",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "WeixinManager_UserTag",
                type: "int",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("SqlServer:Identity", "1, 1");
        }
    }
}
