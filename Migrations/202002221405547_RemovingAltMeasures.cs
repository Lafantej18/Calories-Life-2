namespace Calories_Life_2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovingAltMeasures : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FoodSaves", "Is100Grams", c => c.Boolean(nullable: false));
            DropColumn("dbo.FoodSaves", "IsBranded");
            DropColumn("dbo.FoodSaves", "ServingWeight");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FoodSaves", "ServingWeight", c => c.Short(nullable: false));
            AddColumn("dbo.FoodSaves", "IsBranded", c => c.Boolean(nullable: false));
            DropColumn("dbo.FoodSaves", "Is100Grams");
        }
    }
}
