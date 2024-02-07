using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IQP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTableNamesSlightly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Commentaries_AspNetUsers_CreatedById",
                table: "Commentaries");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_AspNetUsers_CreatorId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_TechTask_AspNetUsers_CreatorId",
                table: "TechTask");

            migrationBuilder.DropForeignKey(
                name: "FK_TechTaskSubmission_AspNetUsers_CreatorId",
                table: "TechTaskSubmission");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersPassedAlgoTasks_AspNetUsers_PassedById",
                table: "UsersPassedAlgoTasks");

            migrationBuilder.DropTable(
                name: "CommentaryUser");

            migrationBuilder.DropTable(
                name: "QuestionUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "Users");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UsersLikedCommentaries",
                columns: table => new
                {
                    LikedById = table.Column<Guid>(type: "uuid", nullable: false),
                    LikedCommentariesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersLikedCommentaries", x => new { x.LikedById, x.LikedCommentariesId });
                    table.ForeignKey(
                        name: "FK_UsersLikedCommentaries_Commentaries_LikedCommentariesId",
                        column: x => x.LikedCommentariesId,
                        principalTable: "Commentaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersLikedCommentaries_Users_LikedById",
                        column: x => x.LikedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsersLikedQuestions",
                columns: table => new
                {
                    LikedById = table.Column<Guid>(type: "uuid", nullable: false),
                    LikedQuestionsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersLikedQuestions", x => new { x.LikedById, x.LikedQuestionsId });
                    table.ForeignKey(
                        name: "FK_UsersLikedQuestions_Questions_LikedQuestionsId",
                        column: x => x.LikedQuestionsId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersLikedQuestions_Users_LikedById",
                        column: x => x.LikedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersLikedCommentaries_LikedCommentariesId",
                table: "UsersLikedCommentaries",
                column: "LikedCommentariesId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersLikedQuestions_LikedQuestionsId",
                table: "UsersLikedQuestions",
                column: "LikedQuestionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_Users_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_Users_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_Users_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_Users_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Commentaries_Users_CreatedById",
                table: "Commentaries",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Users_CreatorId",
                table: "Questions",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TechTask_Users_CreatorId",
                table: "TechTask",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TechTaskSubmission_Users_CreatorId",
                table: "TechTaskSubmission",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersPassedAlgoTasks_Users_PassedById",
                table: "UsersPassedAlgoTasks",
                column: "PassedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_Users_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_Users_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_Users_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_Users_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Commentaries_Users_CreatedById",
                table: "Commentaries");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Users_CreatorId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_TechTask_Users_CreatorId",
                table: "TechTask");

            migrationBuilder.DropForeignKey(
                name: "FK_TechTaskSubmission_Users_CreatorId",
                table: "TechTaskSubmission");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersPassedAlgoTasks_Users_PassedById",
                table: "UsersPassedAlgoTasks");

            migrationBuilder.DropTable(
                name: "UsersLikedCommentaries");

            migrationBuilder.DropTable(
                name: "UsersLikedQuestions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "AspNetUsers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CommentaryUser",
                columns: table => new
                {
                    LikedById = table.Column<Guid>(type: "uuid", nullable: false),
                    LikedCommentariesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentaryUser", x => new { x.LikedById, x.LikedCommentariesId });
                    table.ForeignKey(
                        name: "FK_CommentaryUser_AspNetUsers_LikedById",
                        column: x => x.LikedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentaryUser_Commentaries_LikedCommentariesId",
                        column: x => x.LikedCommentariesId,
                        principalTable: "Commentaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionUser",
                columns: table => new
                {
                    LikedById = table.Column<Guid>(type: "uuid", nullable: false),
                    LikedQuestionsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionUser", x => new { x.LikedById, x.LikedQuestionsId });
                    table.ForeignKey(
                        name: "FK_QuestionUser_AspNetUsers_LikedById",
                        column: x => x.LikedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionUser_Questions_LikedQuestionsId",
                        column: x => x.LikedQuestionsId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentaryUser_LikedCommentariesId",
                table: "CommentaryUser",
                column: "LikedCommentariesId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionUser_LikedQuestionsId",
                table: "QuestionUser",
                column: "LikedQuestionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Commentaries_AspNetUsers_CreatedById",
                table: "Commentaries",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_AspNetUsers_CreatorId",
                table: "Questions",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TechTask_AspNetUsers_CreatorId",
                table: "TechTask",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TechTaskSubmission_AspNetUsers_CreatorId",
                table: "TechTaskSubmission",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersPassedAlgoTasks_AspNetUsers_PassedById",
                table: "UsersPassedAlgoTasks",
                column: "PassedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
