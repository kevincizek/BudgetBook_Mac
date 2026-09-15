using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetBook.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
            table: "Categories",
            columns: new[] { "Id", "Name", "Type", "IsActive" },
            values: new object[,]
            {
                { 1, "Salary", 0, true },
                { 2, "Rent", 1, true },
                { 3, "Groceries", 1, true },
                { 4, "Other Income", 0, true },
                { 5, "Test - Non Selectable Category", 0, false },
            });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "Categories", keyColumn: "Id", keyValues: new object[] { 1, 2, 3, 4, 5 });
        }
    }
}
