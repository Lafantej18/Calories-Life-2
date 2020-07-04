namespace Calories_Life_2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChanchingMealTypeFromStringToInt : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FoodSaves", "Meal", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.FoodSaves", "Meal", c => c.String());
        }
    }
}
