using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chartify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateChartCompositeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChartEntries_Charts_ChartId",
                table: "ChartEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Charts",
                table: "Charts");

            migrationBuilder.DropIndex(
                name: "IX_ChartEntries_ChartId",
                table: "ChartEntries");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Charts");

            migrationBuilder.DropColumn(
                name: "ChartId",
                table: "ChartEntries");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Charts",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "daily",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Region",
                table: "Charts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "TrackSpotifyId",
                table: "ChartEntries",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "TrackName",
                table: "ChartEntries",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ArtistNames",
                table: "ChartEntries",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateOnly>(
                name: "ChartDate",
                table: "ChartEntries",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(2026, 1, 29));

            migrationBuilder.AddColumn<string>(
                name: "ChartRegion",
                table: "ChartEntries",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Charts",
                table: "Charts",
                columns: new[] { "Date", "Region" });

            migrationBuilder.CreateIndex(
                name: "IX_Charts_Date",
                table: "Charts",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Charts_Region",
                table: "Charts",
                column: "Region");

            migrationBuilder.CreateIndex(
                name: "IX_ChartEntries_ChartDate_ChartRegion",
                table: "ChartEntries",
                columns: new[] { "ChartDate", "ChartRegion" });

            migrationBuilder.CreateIndex(
                name: "IX_ChartEntries_TrackSpotifyId",
                table: "ChartEntries",
                column: "TrackSpotifyId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChartEntries_Charts_ChartDate_ChartRegion",
                table: "ChartEntries",
                columns: new[] { "ChartDate", "ChartRegion" },
                principalTable: "Charts",
                principalColumns: new[] { "Date", "Region" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChartEntries_Charts_ChartDate_ChartRegion",
                table: "ChartEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Charts",
                table: "Charts");

            migrationBuilder.DropIndex(
                name: "IX_Charts_Date",
                table: "Charts");

            migrationBuilder.DropIndex(
                name: "IX_Charts_Region",
                table: "Charts");

            migrationBuilder.DropIndex(
                name: "IX_ChartEntries_ChartDate_ChartRegion",
                table: "ChartEntries");

            migrationBuilder.DropIndex(
                name: "IX_ChartEntries_TrackSpotifyId",
                table: "ChartEntries");

            migrationBuilder.DropColumn(
                name: "ChartDate",
                table: "ChartEntries");

            migrationBuilder.DropColumn(
                name: "ChartRegion",
                table: "ChartEntries");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Charts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldDefaultValue: "daily");

            migrationBuilder.AlterColumn<string>(
                name: "Region",
                table: "Charts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Charts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "TrackSpotifyId",
                table: "ChartEntries",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "TrackName",
                table: "ChartEntries",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "ArtistNames",
                table: "ChartEntries",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<Guid>(
                name: "ChartId",
                table: "ChartEntries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Charts",
                table: "Charts",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ChartEntries_ChartId",
                table: "ChartEntries",
                column: "ChartId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChartEntries_Charts_ChartId",
                table: "ChartEntries",
                column: "ChartId",
                principalTable: "Charts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
