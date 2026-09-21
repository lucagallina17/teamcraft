using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamCraft.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_Teams_TeamId",
                table: "TeamMembers");


            migrationBuilder.CreateTable(
                name: "TeamAggregate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Score = table.Column<float>(type: "real", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamAggregate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamAggregate_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamReviewAggregate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamReviewAggregate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamReviewAggregate_TeamAggregate_TeamId",
                        column: x => x.TeamId,
                        principalTable: "TeamAggregate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamAggregate_ProjectId",
                table: "TeamAggregate",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamReviewAggregate_TeamId",
                table: "TeamReviewAggregate",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_TeamAggregate_TeamId",
                table: "TeamMembers",
                column: "TeamId",
                principalTable: "TeamAggregate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_TeamAggregate_TeamId",
                table: "TeamMembers");

            migrationBuilder.DropTable(
                name: "TeamReviewAggregate");

            migrationBuilder.DropTable(
                name: "TeamAggregate");

            migrationBuilder.DropIndex(
                name: "IX_TeamMembers_TeamId1",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "TeamId1",
                table: "TeamMembers");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_Teams_TeamId",
                table: "TeamMembers",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
