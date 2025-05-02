using Microsoft.EntityFrameworkCore.Migrations;

namespace InternTask.Migrations
{
    public partial class AddParametersJsonbColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParametersJson",
                table: "ConditionEvaluations",
                type: "jsonb",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParametersJson",
                table: "ConditionEvaluations");
        }
    }
}