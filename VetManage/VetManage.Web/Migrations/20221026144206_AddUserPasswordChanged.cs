using Microsoft.EntityFrameworkCore.Migrations;

namespace VetManage.Web.Migrations
{
    public partial class AddUserPasswordChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PasswordChanged",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordChanged",
                table: "AspNetUsers");
        }
    }
}
