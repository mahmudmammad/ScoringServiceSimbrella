using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InternTask.Migrations
{
    /// <inheritdoc />
    public partial class SeedConditionConfigs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE \"ConditionConfigs\" ALTER COLUMN \"ParametersJson\" TYPE jsonb USING \"ParametersJson\"::jsonb;"
            );
            migrationBuilder.InsertData(
                table: "ConditionConfigs",
                columns: new[] { "Id", "Enabled", "ParametersJson", "Priority", "Required", "Type" },
                values: new object[,]
                {
                    { 1, true, "{\"MinimumSalary\":\"5000\",\"SalaryMultiplier\":\"1.2\"}", 1, true, "SalaryThreshold" },
                    { 2, true, "{\"MaxLoanCount\":\"5\"}", 2, true, "LoanCountLimit" },
                    { 3, true, "{\"MinimumBalance\":\"1000\",\"BalanceMultiplier\":\"2.0\"}", 3, false, "AccountBalance" },
                    { 4, false, "{\"FailIfHasDefault\":\"true\"}", 4, true, "DefaultHistory" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConditionConfigs",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ConditionConfigs",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ConditionConfigs",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ConditionConfigs",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.AlterColumn<string>(
                name: "ParametersJson",
                table: "ConditionConfigs",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "jsonb");
        }
    }
}
