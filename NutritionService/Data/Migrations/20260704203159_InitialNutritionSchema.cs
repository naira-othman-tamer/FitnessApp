using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutritionService.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialNutritionSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MealPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TargetCalorieRangeMin = table.Column<double>(type: "float(10)", precision: 10, scale: 2, nullable: false),
                    TargetCalorieRangeMax = table.Column<double>(type: "float(10)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Meals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Calories = table.Column<double>(type: "float(10)", precision: 10, scale: 2, nullable: false),
                    ProteinGrams = table.Column<double>(type: "float(10)", precision: 10, scale: 2, nullable: false),
                    CarbohydrateGrams = table.Column<double>(type: "float(10)", precision: 10, scale: 2, nullable: false),
                    FatGrams = table.Column<double>(type: "float(10)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MealAllergens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MealId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealAllergens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealAllergens_Meals_MealId",
                        column: x => x.MealId,
                        principalTable: "Meals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MealIngredients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MealId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Quantity = table.Column<double>(type: "float(10)", precision: 10, scale: 2, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealIngredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealIngredients_Meals_MealId",
                        column: x => x.MealId,
                        principalTable: "Meals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MealInstructions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MealId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StepNumber = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealInstructions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealInstructions_Meals_MealId",
                        column: x => x.MealId,
                        principalTable: "Meals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MealPlanItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MealPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MealId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeek = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    MealTime = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealPlanItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealPlanItems_MealPlans_MealPlanId",
                        column: x => x.MealPlanId,
                        principalTable: "MealPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MealPlanItems_Meals_MealId",
                        column: x => x.MealId,
                        principalTable: "Meals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MealTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MealId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealTags_Meals_MealId",
                        column: x => x.MealId,
                        principalTable: "Meals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MealVariations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MealId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealVariations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealVariations_Meals_MealId",
                        column: x => x.MealId,
                        principalTable: "Meals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MealAllergens_MealId_Name",
                table: "MealAllergens",
                columns: new[] { "MealId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MealIngredients_MealId",
                table: "MealIngredients",
                column: "MealId");

            migrationBuilder.CreateIndex(
                name: "IX_MealInstructions_MealId_StepNumber",
                table: "MealInstructions",
                columns: new[] { "MealId", "StepNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanItems_MealId",
                table: "MealPlanItems",
                column: "MealId");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanItems_MealPlanId_DayOfWeek_MealTime_SortOrder",
                table: "MealPlanItems",
                columns: new[] { "MealPlanId", "DayOfWeek", "MealTime", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MealPlans_TargetCalorieRangeMin_TargetCalorieRangeMax",
                table: "MealPlans",
                columns: new[] { "TargetCalorieRangeMin", "TargetCalorieRangeMax" });

            migrationBuilder.CreateIndex(
                name: "IX_Meals_ProteinGrams",
                table: "Meals",
                column: "ProteinGrams");

            migrationBuilder.CreateIndex(
                name: "IX_Meals_Type_Calories",
                table: "Meals",
                columns: new[] { "Type", "Calories" });

            migrationBuilder.CreateIndex(
                name: "IX_MealTags_MealId_Name",
                table: "MealTags",
                columns: new[] { "MealId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MealVariations_MealId",
                table: "MealVariations",
                column: "MealId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MealAllergens");

            migrationBuilder.DropTable(
                name: "MealIngredients");

            migrationBuilder.DropTable(
                name: "MealInstructions");

            migrationBuilder.DropTable(
                name: "MealPlanItems");

            migrationBuilder.DropTable(
                name: "MealTags");

            migrationBuilder.DropTable(
                name: "MealVariations");

            migrationBuilder.DropTable(
                name: "MealPlans");

            migrationBuilder.DropTable(
                name: "Meals");
        }
    }
}
