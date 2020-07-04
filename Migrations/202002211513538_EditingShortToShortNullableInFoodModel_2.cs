namespace Calories_Life_2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditingShortToShortNullableInFoodModel_2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FoodSaves", "ServingWeightGrams", c => c.Short());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.FoodSaves", "ServingWeightGrams", c => c.Short(nullable: false));
        }
    }
}
