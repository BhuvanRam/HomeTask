using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RetailBank.Migrations
{
    /// <inheritdoc />
    public partial class RetailBank1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    PaymentTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CheckOutId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AmountDeducted = table.Column<double>(type: "float", nullable: false),
                    CardNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.PaymentTransactionId);
                });

            migrationBuilder.CreateTable(
                name: "ShopperDetails",
                columns: table => new
                {
                    CardNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExpiryDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Balance = table.Column<double>(type: "float", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CVV = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopperDetails", x => x.CardNumber);
                });

            migrationBuilder.InsertData(
                table: "ShopperDetails",
                columns: new[] { "CardNumber", "Balance", "CVV", "Currency", "ExpiryDate" },
                values: new object[,]
                {
                    { "371449635398431", 200.0, "456", "EUR", "10/2024" },
                    { "378282246310005", 200.0, "123", "USD", "10/2023" },
                    { "378734493671000", 200.0, "789", "INR", "10/2025" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "ShopperDetails");
        }
    }
}
