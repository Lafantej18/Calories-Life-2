using Calories_Life_2.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Calories_Life_2.ViewModels
{
    public class CaloriesTrainingsViewModel
    {
        public List<CaloriesTraining> CaloriesTrainings { get; set; }

        public CaloriesTrainingEnterWords caloriesTrainingEnterWords { get; set; }

        public short SumKcal { get; set; }
    }

    public class CaloriesTrainingEnterWords
    {
        [Required]
        [StringLength(300)]
        public string term { get; set; }

        [Required]
        [Range(1, 1000)]
        public short time { get; set; }
    }
}