using System;
using System.ComponentModel.DataAnnotations;

namespace Calories_Life_2.Models
{
    public class CaloriesTraining
    {

        public int CaloriesTrainingId { get; set; }

        public string NameOfExercise { get; set; }

        public short ExerciseTime { get; set; }

        public short ExerciseCalories { get; set; }

        public DateTime Day { get; set; }

        //Navigation Properties
        [Required]
        public virtual UserMenu UserMenu { get; set; }
        
        public short UserMenuId { get; set; }

    }
}