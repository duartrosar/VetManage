using Microsoft.EntityFrameworkCore.Migrations;

namespace VetManage.Web.Migrations
{
    public partial class AddPetIdAndVetIdToAppointment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Pets_PetId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Vets_VetId",
                table: "Appointments");

            migrationBuilder.AlterColumn<int>(
                name: "VetId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PetId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Pets_PetId",
                table: "Appointments",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Vets_VetId",
                table: "Appointments",
                column: "VetId",
                principalTable: "Vets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Pets_PetId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Vets_VetId",
                table: "Appointments");

            migrationBuilder.AlterColumn<int>(
                name: "VetId",
                table: "Appointments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PetId",
                table: "Appointments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Pets_PetId",
                table: "Appointments",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Vets_VetId",
                table: "Appointments",
                column: "VetId",
                principalTable: "Vets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
