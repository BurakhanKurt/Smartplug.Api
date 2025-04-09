using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smartplug.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduleEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Schedules");

            migrationBuilder.AddColumn<bool>(
                name: "DesiredStatus",
                table: "Schedules",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTimeOfDay",
                table: "Schedules",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Executed",
                table: "Schedules",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HangfireJobId",
                table: "Schedules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HangfireJobIdOff",
                table: "Schedules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HangfireJobIdOn",
                table: "Schedules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecurringDay",
                table: "Schedules",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledTime",
                table: "Schedules",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTimeOfDay",
                table: "Schedules",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Schedules",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DesiredStatus",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "EndTimeOfDay",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "Executed",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "HangfireJobId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "HangfireJobIdOff",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "HangfireJobIdOn",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "RecurringDay",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "ScheduledTime",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "StartTimeOfDay",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Schedules");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "Schedules",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "Schedules",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
