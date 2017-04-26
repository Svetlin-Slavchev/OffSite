using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OffSite.Web.Data.Migrations
{
    public partial class first : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.AddColumn<double>(
                name: "PaidDaysOff",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 20.0);

            migrationBuilder.AddColumn<string>(
                name: "WatchrersId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OffSiteStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OffSiteStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OffSiteRequests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Approved = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    IsHalfADayRequest = table.Column<bool>(nullable: false),
                    Reason = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    StatusFkId = table.Column<int>(nullable: false),
                    UserFkId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OffSiteRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OffSiteRequests_OffSiteStatuses_StatusFkId",
                        column: x => x.StatusFkId,
                        principalTable: "OffSiteStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OffSiteRequests_AspNetUsers_UserFkId",
                        column: x => x.UserFkId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationMessages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Approved = table.Column<bool>(nullable: false),
                    ApprovedDone = table.Column<bool>(nullable: false),
                    ApproverUserFkId = table.Column<string>(nullable: true),
                    OffSiteRequestFkId = table.Column<int>(nullable: false),
                    SelectedUserFkId = table.Column<string>(nullable: true),
                    Viewed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationMessages_AspNetUsers_ApproverUserFkId",
                        column: x => x.ApproverUserFkId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificationMessages_OffSiteRequests_OffSiteRequestFkId",
                        column: x => x.OffSiteRequestFkId,
                        principalTable: "OffSiteRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationMessages_AspNetUsers_SelectedUserFkId",
                        column: x => x.SelectedUserFkId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationMessages_ApproverUserFkId",
                table: "NotificationMessages",
                column: "ApproverUserFkId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationMessages_OffSiteRequestFkId",
                table: "NotificationMessages",
                column: "OffSiteRequestFkId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationMessages_SelectedUserFkId",
                table: "NotificationMessages",
                column: "SelectedUserFkId");

            migrationBuilder.CreateIndex(
                name: "IX_OffSiteRequests_StatusFkId",
                table: "OffSiteRequests",
                column: "StatusFkId");

            migrationBuilder.CreateIndex(
                name: "IX_OffSiteRequests_UserFkId",
                table: "OffSiteRequests",
                column: "UserFkId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationMessages");

            migrationBuilder.DropTable(
                name: "OffSiteRequests");

            migrationBuilder.DropTable(
                name: "OffSiteStatuses");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "PaidDaysOff",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "WatchrersId",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName");
        }
    }
}
