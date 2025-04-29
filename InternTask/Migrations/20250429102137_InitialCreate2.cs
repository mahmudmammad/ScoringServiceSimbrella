using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternTask.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConditionId",
                table: "ConditionEvaluations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConditionId",
                table: "ConditionEvaluations",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
