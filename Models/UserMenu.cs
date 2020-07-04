using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Calories_Life_2.Models
{
    public class UserMenu
    {
        public short UserMenuId { get; set; }
        public short CarbohydratesChangable { get; set; }
        public short ProteinsChangable { get; set; }
        public short FatChangable { get; set; }
        public short CaloricDemandChangable { get; set; }
        public DateTime Date { get; set; }

        //Navigation Properties
        [Key, ForeignKey("CaloriesCounterId")]
        public CaloriesCounter CaloriesCounter { get; set; }
        public short CaloriesCounterId { get; set; }
    }
}