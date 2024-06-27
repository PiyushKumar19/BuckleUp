using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuckleUp.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedtheforeignkeytoIdentifierofTenantsinUserstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Tenants_TenantId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_TenantId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "TenantIdentifier",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantIdentifier",
                table: "Users",
                column: "TenantIdentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Tenants_TenantIdentifier",
                table: "Users",
                column: "TenantIdentifier",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Tenants_TenantIdentifier",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_TenantIdentifier",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TenantIdentifier",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Users",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId",
                table: "Users",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Tenants_TenantId",
                table: "Users",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");
        }
    }
}
