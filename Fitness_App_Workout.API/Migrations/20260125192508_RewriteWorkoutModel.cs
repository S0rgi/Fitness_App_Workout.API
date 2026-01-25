using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fitness_App_Workout.API.Migrations
{
    /// <inheritdoc />
    public partial class RewriteWorkoutModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reps",
                table: "WorkoutExercise");

            migrationBuilder.DropColumn(
                name: "Sets",
                table: "WorkoutExercise");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "WorkoutExercise");

            migrationBuilder.CreateTable(
                name: "WorkoutSet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkoutExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Reps = table.Column<int>(type: "integer", nullable: false),
                    Weight = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutSet_WorkoutExercise_WorkoutExerciseId",
                        column: x => x.WorkoutExerciseId,
                        principalTable: "WorkoutExercise",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSet_WorkoutExerciseId",
                table: "WorkoutSet",
                column: "WorkoutExerciseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkoutSet");

            migrationBuilder.AddColumn<int>(
                name: "Reps",
                table: "WorkoutExercise",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Sets",
                table: "WorkoutExercise",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "Weight",
                table: "WorkoutExercise",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
