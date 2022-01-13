using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TxnManualEntry2022.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountTxns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FloorNum = table.Column<int>(type: "INTEGER", nullable: false),
                    RoomNum = table.Column<string>(type: "TEXT", nullable: false),
                    TxnTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TxnAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    UserID = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTxns", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountTxns");
        }
    }
}
