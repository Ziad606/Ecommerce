using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemovedSomeOfRequiredUserInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_BuyerId",
                table: "Reviews");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Users_DateOfBirth",
                table: "Users");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Users_Gender",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DefaultBillingAddress",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DefaultShippingAddress",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StripePaymentIntentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentMethodDetails = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_BuyerId",
                table: "Reviews",
                column: "BuyerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_BuyerId",
                table: "Reviews");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.AddColumn<string>(
                name: "DefaultBillingAddress",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DefaultShippingAddress",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Users_DateOfBirth",
                table: "Users",
                sql: "[DateOfBirth] <= GETDATE()");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Users_Gender",
                table: "Users",
                sql: "[Gender] IN ('Male', 'Female', 'Other', 'PreferNotToSay')");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_BuyerId",
                table: "Reviews",
                column: "BuyerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
