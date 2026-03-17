using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fitness_App_Workout.API.Migrations
{
    /// <inheritdoc />
    public partial class OwnWeight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOwnWeight",
                table: "WorkoutSet",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOwnWeight",
                table: "WorkoutSet");
        }
    }
}
