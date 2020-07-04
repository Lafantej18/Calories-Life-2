using System.Data.Entity;
using Calories_Life_2.Models;
using Microsoft.AspNet.Identity.EntityFramework;


namespace Calories_Life_2.DAL
{
    public class CaloriesLifeContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<CaloriesCounter> CaloriesCounters { get; set; }
        public DbSet<UserMenu> UserMenus { get; set; }
        public DbSet<FoodSaves> FoodSaveses { get; set; }
        public DbSet<CaloriesTraining> CaloriesTrainings { get; set; }

        public CaloriesLifeContext() : base("CaloriesLifeContext", throwIfV1Schema: false)
        {
        }

        //static CaloriesLifeContext() 
        //{
        //   // Database.SetInitializer(new CaloriesLifeInitializer());
        //}

        public static CaloriesLifeContext Create()
        {    
            return new CaloriesLifeContext();
        }
    }

}