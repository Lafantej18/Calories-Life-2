namespace Calories_Life_2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CaloriesCounters",
                c => new
                    {
                        CaloriesCounterId = c.Short(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        Weight = c.Short(nullable: false),
                        Height = c.Short(nullable: false),
                        GenderChoose = c.Int(nullable: false),
                        Age = c.Byte(nullable: false),
                        Active = c.Int(nullable: false),
                        CaloricDemand = c.Short(nullable: false),
                        Choice = c.Int(nullable: false),
                        Carbohydrates = c.Short(nullable: false),
                        Proteins = c.Short(nullable: false),
                        Fat = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.CaloriesCounterId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.CaloriesTrainings",
                c => new
                    {
                        CaloriesTrainingId = c.Int(nullable: false, identity: true),
                        NumbersOfTrainings = c.Short(nullable: false),
                        Intense = c.Short(nullable: false),
                        TrainingTime = c.Short(nullable: false),
                        UserMenuId = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.CaloriesTrainingId)
                .ForeignKey("dbo.UserMenus", t => t.UserMenuId, cascadeDelete: true)
                .Index(t => t.UserMenuId);
            
            CreateTable(
                "dbo.UserMenus",
                c => new
                    {
                        UserMenuId = c.Short(nullable: false, identity: true),
                        CarbohydratesChangable = c.Short(nullable: false),
                        ProteinsChangable = c.Short(nullable: false),
                        FatChangable = c.Short(nullable: false),
                        CaloricDemandChangable = c.Short(nullable: false),
                        Date = c.DateTime(nullable: false),
                        CaloriesCounterId = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.UserMenuId)
                .ForeignKey("dbo.CaloriesCounters", t => t.CaloriesCounterId, cascadeDelete: true)
                .Index(t => t.CaloriesCounterId);
            
            CreateTable(
                "dbo.FoodSaves",
                c => new
                    {
                        FoodSavesId = c.Int(nullable: false, identity: true),
                        IsBranded = c.Boolean(nullable: false),
                        Name = c.String(),
                        Kcal = c.Short(nullable: false),
                        Fat = c.Short(nullable: false),
                        Carbs = c.Short(nullable: false),
                        Proteins = c.Short(nullable: false),
                        ServingSize = c.Double(nullable: false),
                        ServingUnit = c.String(),
                        ServingWeightGrams = c.Short(nullable: false),
                        ServingWeight = c.Short(nullable: false),
                        Day = c.DateTime(nullable: false),
                        Meal = c.String(),
                        UserMenuId = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.FoodSavesId)
                .ForeignKey("dbo.UserMenus", t => t.UserMenuId, cascadeDelete: true)
                .Index(t => t.UserMenuId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.CaloriesTrainings", "UserMenuId", "dbo.UserMenus");
            DropForeignKey("dbo.FoodSaves", "UserMenuId", "dbo.UserMenus");
            DropForeignKey("dbo.UserMenus", "CaloriesCounterId", "dbo.CaloriesCounters");
            DropForeignKey("dbo.CaloriesCounters", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.FoodSaves", new[] { "UserMenuId" });
            DropIndex("dbo.UserMenus", new[] { "CaloriesCounterId" });
            DropIndex("dbo.CaloriesTrainings", new[] { "UserMenuId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.CaloriesCounters", new[] { "UserId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.FoodSaves");
            DropTable("dbo.UserMenus");
            DropTable("dbo.CaloriesTrainings");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.CaloriesCounters");
        }
    }
}
