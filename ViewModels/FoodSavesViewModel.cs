using Calories_Life_2.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Calories_Life_2.ViewModels
{
    public class FoodSavesViewModel
    {
        public List<FoodSaves> FoodSaveses { get; set; }

        public CaloriesFoodEnterWords caloriesFoodEnterWords { get; set; }
        public short SumKcal { get; set; }
        public short SumFat { get; set; }
        public short SumCarbs { get; set; }
        public short SumProteins { get; set; }
    }

    public class CaloriesFoodEnterWords
    {
        [Required]
        [StringLength(300)]
        public string SearchQuery { get; set; }

        [Required]
        public int Meal { get; set; }
    }

    public class Meal
    {
        public int Id { get; set; }
        public string Text { get; set; }

    }
}