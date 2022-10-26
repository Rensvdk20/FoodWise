using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Canteens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Location = table.Column<int>(type: "int", nullable: false),
                    City = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Canteens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    containsAlcohol = table.Column<bool>(type: "bit", nullable: false),
                    Picture = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    Birthday = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StudentNumber = table.Column<int>(type: "int", maxLength: 12, nullable: false),
                    StudyCity = table.Column<int>(type: "int", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CanteenEmployees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeNumber = table.Column<int>(type: "int", nullable: false),
                    CanteenId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanteenEmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanteenEmployees_Canteens_CanteenId",
                        column: x => x.CanteenId,
                        principalTable: "Canteens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CanteenId = table.Column<int>(type: "int", nullable: false),
                    PickupTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AvailableTill = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EighteenPlus = table.Column<bool>(type: "bit", nullable: false),
                    Price = table.Column<decimal>(type: "smallmoney", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    ReservedByStudentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Packages_Canteens_CanteenId",
                        column: x => x.CanteenId,
                        principalTable: "Canteens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Packages_Students_ReservedByStudentId",
                        column: x => x.ReservedByStudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Package_Product",
                columns: table => new
                {
                    PackageId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Package_Product", x => new { x.PackageId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_Package_Product_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Package_Product_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Canteens",
                columns: new[] { "Id", "City", "Location" },
                values: new object[,]
                {
                    { 1, 0, 1 },
                    { 2, 0, 0 }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Name", "Picture", "containsAlcohol" },
                values: new object[,]
                {
                    { 1, "Appel", null, false },
                    { 2, "Peer", null, false },
                    { 3, "Broodje Frikandel", null, false },
                    { 4, "Kaiserbroodje", null, false },
                    { 5, "Hertog Jan", null, true }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "Birthday", "Email", "FirstName", "LastName", "PhoneNumber", "StudentNumber", "StudyCity" },
                values: new object[] { 1, new DateTime(2001, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mark@gmail.com", "Mark", "De Groot", "+31612345678", 2184500, 0 });

            migrationBuilder.InsertData(
                table: "CanteenEmployees",
                columns: new[] { "Id", "CanteenId", "Email", "EmployeeNumber", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Helma@avanscanteen.nl", 555, "Helma" },
                    { 2, 2, "Erika@avanscanteen.nl", 678, "Erika" }
                });

            migrationBuilder.InsertData(
                table: "Packages",
                columns: new[] { "Id", "AvailableTill", "CanteenId", "Category", "Description", "EighteenPlus", "Name", "PickupTime", "Price", "ReservedByStudentId" },
                values: new object[,]
                {
                    { 1, new DateTime(2022, 11, 16, 17, 15, 0, 0, DateTimeKind.Unspecified), 1, 1, "Lekker gezond pakket met fruit aan te raden voor elke student", false, "Gezond pakket", new DateTime(2022, 11, 16, 13, 15, 0, 0, DateTimeKind.Unspecified), 4.50m, null },
                    { 2, new DateTime(2022, 11, 18, 16, 15, 0, 0, DateTimeKind.Unspecified), 2, 1, "Heerlijke broodjes als lunch of als tussendoortje", true, "Broodjes pakket", new DateTime(2022, 11, 18, 14, 15, 0, 0, DateTimeKind.Unspecified), 6.50m, null },
                    { 3, new DateTime(2022, 11, 16, 17, 15, 0, 0, DateTimeKind.Unspecified), 1, 2, "Verschillende soorten soep in een compact pakket", false, "Soep pakket", new DateTime(2022, 11, 16, 15, 15, 0, 0, DateTimeKind.Unspecified), 7.50m, 1 }
                });

            migrationBuilder.InsertData(
                table: "Package_Product",
                columns: new[] { "PackageId", "ProductId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 2, 3 },
                    { 2, 4 },
                    { 2, 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CanteenEmployees_CanteenId",
                table: "CanteenEmployees",
                column: "CanteenId");

            migrationBuilder.CreateIndex(
                name: "IX_Package_Product_ProductId",
                table: "Package_Product",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_CanteenId",
                table: "Packages",
                column: "CanteenId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_ReservedByStudentId",
                table: "Packages",
                column: "ReservedByStudentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CanteenEmployees");

            migrationBuilder.DropTable(
                name: "Package_Product");

            migrationBuilder.DropTable(
                name: "Packages");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Canteens");

            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}
