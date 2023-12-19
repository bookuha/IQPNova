using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IQP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentaries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Commentary_AspNetUsers_CreatedById",
                table: "Commentary");

            migrationBuilder.DropForeignKey(
                name: "FK_Commentary_Commentary_ReplyToId",
                table: "Commentary");

            migrationBuilder.DropForeignKey(
                name: "FK_Commentary_Questions_QuestionId",
                table: "Commentary");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentaryUser_Commentary_LikedCommentariesId",
                table: "CommentaryUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Commentary",
                table: "Commentary");

            migrationBuilder.RenameTable(
                name: "Commentary",
                newName: "Commentaries");

            migrationBuilder.RenameIndex(
                name: "IX_Commentary_ReplyToId",
                table: "Commentaries",
                newName: "IX_Commentaries_ReplyToId");

            migrationBuilder.RenameIndex(
                name: "IX_Commentary_QuestionId",
                table: "Commentaries",
                newName: "IX_Commentaries_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_Commentary_CreatedById",
                table: "Commentaries",
                newName: "IX_Commentaries_CreatedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Commentaries",
                table: "Commentaries",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Commentaries_AspNetUsers_CreatedById",
                table: "Commentaries",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Commentaries_Commentaries_ReplyToId",
                table: "Commentaries",
                column: "ReplyToId",
                principalTable: "Commentaries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Commentaries_Questions_QuestionId",
                table: "Commentaries",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentaryUser_Commentaries_LikedCommentariesId",
                table: "CommentaryUser",
                column: "LikedCommentariesId",
                principalTable: "Commentaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Commentaries_AspNetUsers_CreatedById",
                table: "Commentaries");

            migrationBuilder.DropForeignKey(
                name: "FK_Commentaries_Commentaries_ReplyToId",
                table: "Commentaries");

            migrationBuilder.DropForeignKey(
                name: "FK_Commentaries_Questions_QuestionId",
                table: "Commentaries");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentaryUser_Commentaries_LikedCommentariesId",
                table: "CommentaryUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Commentaries",
                table: "Commentaries");

            migrationBuilder.RenameTable(
                name: "Commentaries",
                newName: "Commentary");

            migrationBuilder.RenameIndex(
                name: "IX_Commentaries_ReplyToId",
                table: "Commentary",
                newName: "IX_Commentary_ReplyToId");

            migrationBuilder.RenameIndex(
                name: "IX_Commentaries_QuestionId",
                table: "Commentary",
                newName: "IX_Commentary_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_Commentaries_CreatedById",
                table: "Commentary",
                newName: "IX_Commentary_CreatedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Commentary",
                table: "Commentary",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Commentary_AspNetUsers_CreatedById",
                table: "Commentary",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Commentary_Commentary_ReplyToId",
                table: "Commentary",
                column: "ReplyToId",
                principalTable: "Commentary",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Commentary_Questions_QuestionId",
                table: "Commentary",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentaryUser_Commentary_LikedCommentariesId",
                table: "CommentaryUser",
                column: "LikedCommentariesId",
                principalTable: "Commentary",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
