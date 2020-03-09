using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Senparc.Xscf.WeixinManager.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeixinManager_MpAccount",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Flag = table.Column<bool>(nullable: false),
                    AddTime = table.Column<DateTime>(nullable: false),
                    LastUpdateTime = table.Column<DateTime>(nullable: false),
                    AdminRemark = table.Column<string>(maxLength: 300, nullable: true),
                    Remark = table.Column<string>(maxLength: 300, nullable: true),
                    Logo = table.Column<string>(maxLength: 200, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    AppId = table.Column<string>(maxLength: 100, nullable: false),
                    AppSecret = table.Column<string>(maxLength: 100, nullable: false),
                    Token = table.Column<string>(maxLength: 500, nullable: false),
                    EncodingAESKey = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeixinManager_MpAccount", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeixinManager_MpAccount");
        }
    }
}
