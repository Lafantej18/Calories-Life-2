namespace Calories_Life_2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedNutritionsDefaultToFoodSaves : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FoodSaves", "KcalDefault", c => c.Short(nullable: false));
            AddColumn("dbo.FoodSaves", "FatDefault", c => c.Short(nullable: false));
            AddColumn("dbo.FoodSaves", "CarbsDefault", c => c.Short(nullable: false));
            AddColumn("dbo.FoodSaves", "ProteinsDefault", c => c.Short(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FoodSaves", "ProteinsDefault");
            DropColumn("dbo.FoodSaves", "CarbsDefault");
            DropColumn("dbo.FoodSaves", "FatDefault");
            DropColumn("dbo.FoodSaves", "KcalDefault");
        }
    }
}
