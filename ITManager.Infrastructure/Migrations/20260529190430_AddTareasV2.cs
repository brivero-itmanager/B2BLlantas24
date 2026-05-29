using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTareasV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Tareas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "deduplication_key",
                table: "Tareas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "task_type",
                table: "Tareas",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "woocommerce_response",
                table: "Tareas",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Tareas");

            migrationBuilder.DropColumn(
                name: "deduplication_key",
                table: "Tareas");

            migrationBuilder.DropColumn(
                name: "task_type",
                table: "Tareas");

            migrationBuilder.DropColumn(
                name: "woocommerce_response",
                table: "Tareas");
        }
    }
}
