namespace Calories_Life_2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixingDatabase : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CaloriesTrainings", "NameOfExercise", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CaloriesTrainings", "NameOfExercise", c => c.String(nullable: false));
        }
    }
}
