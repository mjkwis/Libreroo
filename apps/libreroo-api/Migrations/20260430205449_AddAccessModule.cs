using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Libreroo.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAccessModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "access_users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Subject = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    DisplayName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    MemberId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_access_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "access_role_assignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    AssignedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_access_role_assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_access_role_assignments_access_users_AccessUserId",
                        column: x => x.AccessUserId,
                        principalTable: "access_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_access_role_assignments_AccessUserId_Role",
                table: "access_role_assignments",
                columns: new[] { "AccessUserId", "Role" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_access_users_MemberId",
                table: "access_users",
                column: "MemberId",
                unique: true,
                filter: "\"MemberId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_access_users_Subject",
                table: "access_users",
                column: "Subject",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "access_role_assignments");

            migrationBuilder.DropTable(
                name: "access_users");
        }
    }
}
