using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication3.Migrations
{
    public partial class addforeign : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IP",
                table: "LoginAttempts",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "IPLockouts",
                columns: table => new
                {
                    IP = table.Column<string>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IPLockouts", x => x.IP);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempts_IP",
                table: "LoginAttempts",
                column: "IP");

            migrationBuilder.AddForeignKey(
                name: "FK_LoginAttempts_IPLockouts_IP",
                table: "LoginAttempts",
                column: "IP",
                principalTable: "IPLockouts",
                principalColumn: "IP",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoginAttempts_IPLockouts_IP",
                table: "LoginAttempts");

            migrationBuilder.DropTable(
                name: "IPLockouts");

            migrationBuilder.DropIndex(
                name: "IX_LoginAttempts_IP",
                table: "LoginAttempts");

            migrationBuilder.AlterColumn<string>(
                name: "IP",
                table: "LoginAttempts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
