using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocUploader.Server.Migrations
{
    /// <inheritdoc />
    public partial class DocumentCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentCategories",
                columns: table => new
                {
                    DocumentCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CategoryName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentCategories", x => x.DocumentCategoryId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ClientDocumentCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    DocumentCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientDocumentCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientDocumentCategories_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientDocumentCategories_DocumentCategories_DocumentCategory~",
                        column: x => x.DocumentCategoryId,
                        principalTable: "DocumentCategories",
                        principalColumn: "DocumentCategoryId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "KRMN8-SHDA3-BGMNP-MP88",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "59556a30-ebb2-45a4-aaec-83e11c45d0cd", "AQAAAAIAAYagAAAAELcelnE2Y2ciAfH0CiIc3VUuFJMMOuftQRLEWrluKpKNrZes3h3oFzIUlv1JoANJTw==", "5b10def5-303c-481f-abe9-ada041a043fb" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "SHDRE-COM2T-FF99Z-GM55",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7d934d64-20a6-4b17-be0d-04ce2faf182d", "AQAAAAIAAYagAAAAEGDgiphYiUlIwqbQ5/R3FpIY5bj4/ZGzhefh+/GZ9vh6jzE+IlN92PTLz/XdBZZ3HA==", "ae915261-f211-4cb2-ab64-d87b1763e556" });

            migrationBuilder.CreateIndex(
                name: "IX_ClientDocumentCategories_ClientId",
                table: "ClientDocumentCategories",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientDocumentCategories_DocumentCategoryId",
                table: "ClientDocumentCategories",
                column: "DocumentCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientDocumentCategories");

            migrationBuilder.DropTable(
                name: "DocumentCategories");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "KRMN8-SHDA3-BGMNP-MP88",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "26c0b4af-2254-431a-8cc4-eec50faaf5e0", "AQAAAAIAAYagAAAAEJmCUaL2Uwa7K4Aa0b4u+iHgfIqOmp1BOiMMhRSrmAsr7HnyDYnGIKXkSeDZoLrn9A==", "1716f997-2f56-4cfa-bf6c-eb56d5c7a597" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "SHDRE-COM2T-FF99Z-GM55",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b59c5e9e-7e42-4158-ad9d-d3110a2ce240", "AQAAAAIAAYagAAAAENukjePolyxVehGzFW2T9/z61ZNV9hZQ5nTXF/avONABuAYZCymUdm7zP7IPXMbrZA==", "e6bbb6fb-7699-4e90-9dd5-03945097ef6c" });
        }
    }
}
