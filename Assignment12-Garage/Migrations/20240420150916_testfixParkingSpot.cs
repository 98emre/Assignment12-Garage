using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Assignment12_Garage.Migrations
{
    /// <inheritdoc />
    public partial class testfixParkingSpot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParkingSpot",
                table: "Vehicle",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Vehicle",
                columns: new[] { "Id", "ArrivalDate", "Brand", "Color", "NrOfWheels", "ParkingSpot", "RegNumber", "VehicleModel", "VehicleType" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Stenaline", "Blue", 0, "1", "ABC123", "JokeBoat", 2 },
                    { 2, new DateTime(2003, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Aston Martin", "Yellow", 4, "2", "BRUM", "BRUM", 0 },
                    { 3, new DateTime(2023, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Boeing", "White", 6, "3", "1800FLY", "OSAIsJustASuggestion", 1 },
                    { 4, new DateTime(2013, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Volvo", "Deep Blue", 6, "4", "VTF696", "Long Boy", 3 },
                    { 5, new DateTime(2001, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ford", "Purple", 4, "5", "424242", "Fiesta", 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Vehicle",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Vehicle",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Vehicle",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Vehicle",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Vehicle",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DropColumn(
                name: "ParkingSpot",
                table: "Vehicle");
        }
    }
}
