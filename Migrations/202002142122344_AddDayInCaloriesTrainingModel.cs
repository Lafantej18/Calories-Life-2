namespace Calories_Life_2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDayInCaloriesTrainingModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CaloriesTrainings", "Day", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CaloriesTrainings", "Day");
        }
    }
}
