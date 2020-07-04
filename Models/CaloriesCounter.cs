using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Calories_Life_2.Models
{
    public class CaloriesCounter
    {    
        [ScaffoldColumn(false)]
        public short CaloriesCounterId { get; set; }

        [ScaffoldColumn(false)]
        [Key, ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        [ScaffoldColumn(false)]
        public string UserId { get; set; }

        [Required( AllowEmptyStrings = false, ErrorMessage = "You need to enter your weight")]
        [Range(1, 300 , ErrorMessage = "Max weight is 300 kg")]
        [Display(Name = "Enter your weight")]
        public short Weight { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "You need to enter your height")]
        [Range(1, 300, ErrorMessage = "Max height is 300 cm")]
        [Display(Name = "Enter your height")]
        public short Height { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "You need to enter your sex")]
        [Display(Name = "Choose your sex")]
        public Gender GenderChoose { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "You need to enter your age")]
        [Range(1, 110, ErrorMessage = "Age cannot be larger than 110")]
        [Display(Name = "Enter your age")]
        public byte Age { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "You need to pick your activity")]
        [Display(Name = "How would you describe your normal daily activities?")]
        public Activity Active { get; set; }

        [ScaffoldColumn(false)] 
        public short CaloricDemand { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "You need to choose something")]
        [Range(1, int.MaxValue, ErrorMessage = "Select a correct choice")]
        [Display(Name = "What is your goal?")]
        public Choose Choice { get; set; }

        public short Carbohydrates { get; set; }
        public short Proteins { get; set; }
        public short Fat { get; set; }

        public enum Gender
        {
            Man,
            Woman
        }

        public enum Choose
        {
            [Display(Name = "Loose 1 kilogram per week")]
            Loose_1kg = 1,
            [Display(Name = "Loose 0.5 kilograms per week")]
            Loose_05kg = 2,
            [Display(Name = "Mantain my current weight")]
            MaintainWeight = 3,
            [Display(Name = "Gain 0.5 kilograms per week")]
            Gain_05kg = 4,
            [Display(Name = "Gain 1 kilogram per week")]
            Gain_1kg = 5
        }

        public enum Activity
        {
            Active_not,      // 1.2
            Active_light,    // 1.4
            Active_average,  // 1.6
            Active,          // 1.75
            Active_big,      // 1.9
            Active_huge      // 2.15
        }

    }
}
