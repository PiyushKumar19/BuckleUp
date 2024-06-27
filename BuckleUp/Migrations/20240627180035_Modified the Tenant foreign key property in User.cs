using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuckleUp.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedtheTenantforeignkeypropertyinUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Tenants_TenantIdentifier",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "TenantIdentifier",
                table: "Users",
                newName: "TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_TenantIdentifier",
                table: "Users",
                newName: "IX_Users_TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Tenants_TenantId",
                table: "Users",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Tenants_TenantId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "Users",
                newName: "TenantIdentifier");

            migrationBuilder.RenameIndex(
                name: "IX_Users_TenantId",
                table: "Users",
                newName: "IX_Users_TenantIdentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Tenants_TenantIdentifier",
                table: "Users",
                column: "TenantIdentifier",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
