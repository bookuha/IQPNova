using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IQP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAlgoTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlgoTaskCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlgoTaskCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CodeLanguages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Slug = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Extension = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodeLanguages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlgoTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    AlgoCategoryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlgoTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlgoTasks_AlgoTaskCategories_AlgoCategoryId",
                        column: x => x.AlgoCategoryId,
                        principalTable: "AlgoTaskCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlgoTaskCodeSnippet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AlgoTaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguageId = table.Column<Guid>(type: "uuid", nullable: false),
                    SampleCode = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    TestsCode = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlgoTaskCodeSnippet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlgoTaskCodeSnippet_AlgoTasks_AlgoTaskId",
                        column: x => x.AlgoTaskId,
                        principalTable: "AlgoTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlgoTaskCodeSnippet_CodeLanguages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "CodeLanguages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsersPassedAlgoTasks",
                columns: table => new
                {
                    PassedAlgoTasksId = table.Column<Guid>(type: "uuid", nullable: false),
                    PassedById = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersPassedAlgoTasks", x => new { x.PassedAlgoTasksId, x.PassedById });
                    table.ForeignKey(
                        name: "FK_UsersPassedAlgoTasks_AlgoTasks_PassedAlgoTasksId",
                        column: x => x.PassedAlgoTasksId,
                        principalTable: "AlgoTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersPassedAlgoTasks_AspNetUsers_PassedById",
                        column: x => x.PassedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlgoTaskCodeSnippet_AlgoTaskId",
                table: "AlgoTaskCodeSnippet",
                column: "AlgoTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_AlgoTaskCodeSnippet_LanguageId",
                table: "AlgoTaskCodeSnippet",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_AlgoTasks_AlgoCategoryId",
                table: "AlgoTasks",
                column: "AlgoCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersPassedAlgoTasks_PassedById",
                table: "UsersPassedAlgoTasks",
                column: "PassedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlgoTaskCodeSnippet");

            migrationBuilder.DropTable(
                name: "UsersPassedAlgoTasks");

            migrationBuilder.DropTable(
                name: "CodeLanguages");

            migrationBuilder.DropTable(
                name: "AlgoTasks");

            migrationBuilder.DropTable(
                name: "AlgoTaskCategories");
        }
    }
}
