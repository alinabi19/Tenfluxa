using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tenfluxa.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkerMetrics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastAssignedAt",
                table: "Workers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TotalJobsCompleted",
                table: "Workers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastAssignedAt",
                table: "Workers");

            migrationBuilder.DropColumn(
                name: "TotalJobsCompleted",
                table: "Workers");
        }
    }
}
