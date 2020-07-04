namespace Calories_Life_2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingCaloriesTrainingModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CaloriesTrainings", "NameOfExercise", c => c.String(nullable: false));
            AddColumn("dbo.CaloriesTrainings", "ExerciseTime", c => c.Short(nullable: false));
            AddColumn("dbo.CaloriesTrainings", "ExerciseCalories", c => c.Short(nullable: false));
            DropColumn("dbo.CaloriesTrainings", "NumbersOfTrainings");
            DropColumn("dbo.CaloriesTrainings", "Intense");
            DropColumn("dbo.CaloriesTrainings", "TrainingTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CaloriesTrainings", "TrainingTime", c => c.Short(nullable: false));
            AddColumn("dbo.CaloriesTrainings", "Intense", c => c.Short(nullable: false));
            AddColumn("dbo.CaloriesTrainings", "NumbersOfTrainings", c => c.Short(nullable: false));
            DropColumn("dbo.CaloriesTrainings", "ExerciseCalories");
            DropColumn("dbo.CaloriesTrainings", "ExerciseTime");
            DropColumn("dbo.CaloriesTrainings", "NameOfExercise");
        }
    }
}
