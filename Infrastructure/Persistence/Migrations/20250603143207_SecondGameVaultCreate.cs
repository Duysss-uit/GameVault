using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SecondGameVaultCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserGameStatuses_GameId",
                table: "UserGameStatuses",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGameStatuses_Games_GameId",
                table: "UserGameStatuses",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGameStatuses_Games_GameId",
                table: "UserGameStatuses");

            migrationBuilder.DropIndex(
                name: "IX_UserGameStatuses_GameId",
                table: "UserGameStatuses");
        }
    }
}
