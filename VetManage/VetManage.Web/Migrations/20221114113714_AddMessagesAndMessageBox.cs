using Microsoft.EntityFrameworkCore.Migrations;

namespace VetManage.Web.Migrations
{
    public partial class AddMessagesAndMessageBox : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageMessageBox_MessageBoxes_RecipientsId",
                table: "MessageMessageBox");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageMessageBox_Messages_OutboxId",
                table: "MessageMessageBox");

            migrationBuilder.RenameColumn(
                name: "RecipientsId",
                table: "MessageMessageBox",
                newName: "MessageBoxId");

            migrationBuilder.RenameColumn(
                name: "OutboxId",
                table: "MessageMessageBox",
                newName: "MessageId");

            migrationBuilder.RenameIndex(
                name: "IX_MessageMessageBox_RecipientsId",
                table: "MessageMessageBox",
                newName: "IX_MessageMessageBox_MessageBoxId");

            migrationBuilder.AddColumn<string>(
                name: "SenderUsername",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "MessageMessageBox",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageMessageBox_MessageBoxes_MessageBoxId",
                table: "MessageMessageBox",
                column: "MessageBoxId",
                principalTable: "MessageBoxes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageMessageBox_Messages_MessageId",
                table: "MessageMessageBox",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageMessageBox_MessageBoxes_MessageBoxId",
                table: "MessageMessageBox");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageMessageBox_Messages_MessageId",
                table: "MessageMessageBox");

            migrationBuilder.DropColumn(
                name: "SenderUsername",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "MessageMessageBox");

            migrationBuilder.RenameColumn(
                name: "MessageBoxId",
                table: "MessageMessageBox",
                newName: "RecipientsId");

            migrationBuilder.RenameColumn(
                name: "MessageId",
                table: "MessageMessageBox",
                newName: "OutboxId");

            migrationBuilder.RenameIndex(
                name: "IX_MessageMessageBox_MessageBoxId",
                table: "MessageMessageBox",
                newName: "IX_MessageMessageBox_RecipientsId");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageMessageBox_MessageBoxes_RecipientsId",
                table: "MessageMessageBox",
                column: "RecipientsId",
                principalTable: "MessageBoxes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageMessageBox_Messages_OutboxId",
                table: "MessageMessageBox",
                column: "OutboxId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
