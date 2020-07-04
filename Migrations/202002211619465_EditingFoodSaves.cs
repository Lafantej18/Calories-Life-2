namespace Calories_Life_2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditingFoodSaves : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FoodSaves", "Kcal", c => c.Short(nullable: false));
            AlterColumn("dbo.FoodSaves", "Fat", c => c.Short(nullable: false));
            AlterColumn("dbo.FoodSaves", "Carbs", c => c.Short(nullable: false));
            AlterColumn("dbo.FoodSaves", "Proteins", c => c.Short(nullable: false));
            AlterColumn("dbo.FoodSaves", "KcalDefault", c => c.Short(nullable: false));
            AlterColumn("dbo.FoodSaves", "FatDefault", c => c.Short(nullable: false));
            AlterColumn("dbo.FoodSaves", "CarbsDefault", c => c.Short(nullable: false));
            AlterColumn("dbo.FoodSaves", "ProteinsDefault", c => c.Short(nullable: false));
            AlterColumn("dbo.FoodSaves", "ServingWeightGrams", c => c.Short(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.FoodSaves", "ServingWeightGrams", c => c.Short());
            AlterColumn("dbo.FoodSaves", "ProteinsDefault", c => c.Short());
            AlterColumn("dbo.FoodSaves", "CarbsDefault", c => c.Short());
            AlterColumn("dbo.FoodSaves", "FatDefault", c => c.Short());
            AlterColumn("dbo.FoodSaves", "KcalDefault", c => c.Short());
            AlterColumn("dbo.FoodSaves", "Proteins", c => c.Short());
            AlterColumn("dbo.FoodSaves", "Carbs", c => c.Short());
            AlterColumn("dbo.FoodSaves", "Fat", c => c.Short());
            AlterColumn("dbo.FoodSaves", "Kcal", c => c.Short());
        }
    }
}
