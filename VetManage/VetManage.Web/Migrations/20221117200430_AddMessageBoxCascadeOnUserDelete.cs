using Microsoft.EntityFrameworkCore.Migrations;

namespace VetManage.Web.Migrations
{
    public partial class AddMessageBoxCascadeOnUserDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageBoxes_AspNetUsers_UserId",
                table: "MessageBoxes");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageBoxes_AspNetUsers_UserId",
                table: "MessageBoxes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageBoxes_AspNetUsers_UserId",
                table: "MessageBoxes");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageBoxes_AspNetUsers_UserId",
                table: "MessageBoxes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
