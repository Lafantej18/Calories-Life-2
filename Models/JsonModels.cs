
namespace Calories_Life_2.Models
{
    public class JsonModels
    {
        public class Branded
        {
            public string food_name { get; set; }
            public string brand_name { get; set; }
            public string nix_item_id { get; set; }
        }

        public class Common
        {
            public string food_name { get; set; }
        }

        public class Food
        {
            public string food_name { get; set; }
            public object brand_name { get; set; }
            public double? nf_calories { get; set; }
            public double? nf_total_fat { get; set; }
            public double? nf_total_carbohydrate { get; set; }
            public double? nf_protein { get; set; }
            public object nix_item_id { get; set; }
            public double serving_qty { get; set; }
            public string serving_unit { get; set; }
            public double? serving_weight_grams { get; set; }
            public double serving_weight { get; set; }
        }

        public class Exercises
        {
            public string name { get; set; }
            public double? nf_calories { get; set; }
        }

        public class Merged
        {
            public string FoodName { get; set; }
            public string BrandedId { get; set; }
        }

    }
}