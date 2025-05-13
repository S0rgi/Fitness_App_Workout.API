using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fitness_App_Workout.API.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSomeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Workouts",
                newName: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Workouts",
                newName: "Name");
        }
    }
}
