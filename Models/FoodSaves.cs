using System;
using System.ComponentModel.DataAnnotations;

namespace Calories_Life_2.Models
{

    public class FoodSaves
    {
        public int FoodSavesId { get; set; }
        public bool Is100Grams { get; set; }

        public string Name { get; set; }
        public short Kcal { get; set; }
        public short Fat { get; set; }
        public short Carbs { get; set; }
        public short Proteins { get; set; }

        public short KcalDefault { get; set; }
        public short FatDefault { get; set; }
        public short CarbsDefault { get; set; }
        public short ProteinsDefault { get; set; }

        public double ServingSize { get; set; }
        public string ServingUnit { get; set; }
        public short ServingWeightGrams { get; set; }

        public DateTime Day { get; set; }
        public byte Meal { get; set; }



        //Navigation Properties
        [Required]
        public virtual UserMenu UserMenu { get; set; }
        public short UserMenuId { get; set; }
    }
}