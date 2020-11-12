using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Skate3Server.Data.Migrations
{
    public partial class v1Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccountId = table.Column<long>(nullable: false),
                    ProfileId = table.Column<long>(nullable: false),
                    Username = table.Column<string>(nullable: false),
                    ExternalId = table.Column<ulong>(nullable: false),
                    ExternalIdType = table.Column<int>(nullable: false),
                    ExternalBlob = table.Column<byte[]>(nullable: true),
                    LastLogin = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
