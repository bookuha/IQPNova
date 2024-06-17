using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IQP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLikesToAlgoTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsersLikedAlgoTasks",
                columns: table => new
                {
                    LikedAlgoTasksId = table.Column<Guid>(type: "uuid", nullable: false),
                    LikedById = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersLikedAlgoTasks", x => new { x.LikedAlgoTasksId, x.LikedById });
                    table.ForeignKey(
                        name: "FK_UsersLikedAlgoTasks_AlgoTasks_LikedAlgoTasksId",
                        column: x => x.LikedAlgoTasksId,
                        principalTable: "AlgoTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersLikedAlgoTasks_Users_LikedById",
                        column: x => x.LikedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersLikedAlgoTasks_LikedById",
                table: "UsersLikedAlgoTasks",
                column: "LikedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsersLikedAlgoTasks");
        }
    }
}
