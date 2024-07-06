using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IQP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedTechTasksTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TechTaskSubmission");

            migrationBuilder.DropTable(
                name: "TechTask");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TechTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActiveUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Description = table.Column<string>(type: "character varying(750)", maxLength: 750, nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TechTask_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TechTask_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TechTaskSubmission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    TechTaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "character varying(900)", maxLength: 900, nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Review = table.Column<string>(type: "character varying(900)", maxLength: 900, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechTaskSubmission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TechTaskSubmission_TechTask_TechTaskId",
                        column: x => x.TechTaskId,
                        principalTable: "TechTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TechTaskSubmission_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TechTask_CategoryId",
                table: "TechTask",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TechTask_CreatorId",
                table: "TechTask",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_TechTaskSubmission_CreatorId",
                table: "TechTaskSubmission",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_TechTaskSubmission_TechTaskId",
                table: "TechTaskSubmission",
                column: "TechTaskId");
        }
    }
}
