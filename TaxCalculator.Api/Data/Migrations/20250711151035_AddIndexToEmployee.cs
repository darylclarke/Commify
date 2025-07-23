using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaxCalculator.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Employees",
                type: "TEXT",
                nullable: false,
                computedColumnSql: "[FirstName] + ' ' + [LastName]",
                stored: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true,
                oldComputedColumnSql: "[FirstName] + ' ' + [LastName]",
                oldStored: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Employees",
                type: "TEXT",
                nullable: true,
                computedColumnSql: "[FirstName] + ' ' + [LastName]",
                stored: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldComputedColumnSql: "[FirstName] + ' ' + [LastName]",
                oldStored: true);
        }
    }
}
