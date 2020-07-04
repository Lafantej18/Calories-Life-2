using Calories_Life_2.DAL;
using Calories_Life_2.Models;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using static Calories_Life_2.Models.CaloriesCounter;
using System;

namespace Calories_Life_2.Controllers
{
    public class CaloriesCounterController : Controller
    {
        private CaloriesLifeContext _context;

        public CaloriesCounterController()
        {
            _context = new CaloriesLifeContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        //Calculating calories
        public void CaloriesCalculator(ref CaloriesCounter caloriesCounter)
        {
            float cal = BMR(caloriesCounter);
            cal = BMRGender(caloriesCounter, cal);
            cal = CalTemp(cal);

            cal = CaloriesCalculatorActive(cal, caloriesCounter.Active);

            if (caloriesCounter.Choice != Choose.MaintainWeight)
                cal = CaloriesCalculatorChoose(cal, caloriesCounter.Choice);

            caloriesCounter.CaloricDemand = (short)cal;
            Macros(ref caloriesCounter);
        }

        //Calculating Macronutrients
        public void Macros(ref CaloriesCounter counter)
        {
            //Weight mantain
            if (counter.Choice == Choose.MaintainWeight)
            {
                counter.Proteins = ProteinsOrFat(2.1F, counter.Weight);
                counter.Fat = ProteinsOrFat(0.223F, counter.CaloricDemand, 9);
                counter.Carbohydrates = Carbos(counter.Proteins, counter.Fat, counter.CaloricDemand);
            }
            //Weight reduction
            else if (counter.Choice == Choose.Loose_05kg || counter.Choice == Choose.Loose_1kg)
            {
                counter.Proteins = ProteinsOrFat(2.2F, counter.Weight);
                counter.Fat = ProteinsOrFat(0.2F, counter.CaloricDemand, 9);
                counter.Carbohydrates = Carbos(counter.Proteins, counter.Fat, counter.CaloricDemand);
            }
            //Weight gain
            else if (counter.Choice == Choose.Gain_05kg || counter.Choice == Choose.Gain_1kg)
            {
                counter.Proteins = ProteinsOrFat(2.0F, counter.Weight);
                counter.Fat = ProteinsOrFat(0.25F, counter.CaloricDemand, 9);
                counter.Carbohydrates = Carbos(counter.Proteins, counter.Fat, counter.CaloricDemand);
            }
        }

        //Auxiliary functions
        public short ProteinsOrFat(float ratio, short kgOrKcal, byte divide = 1)
        {
            return (short)((ratio * kgOrKcal)/divide);
        }

        public short Carbos(short proteins, short fat, short kcal)
        {
            return (short)((kcal - 4 * proteins - 9 * fat) / 4);
        }

        public float BMR (CaloriesCounter caloriesCounter)
        {
            return (caloriesCounter.Weight * 9.99F) + (caloriesCounter.Height * 6.25F) - (4.92F * caloriesCounter.Age);
        }

        public float BMRGender(CaloriesCounter caloriesCounter, float cal)
        {
            if (caloriesCounter.GenderChoose == Gender.Man)
                return cal + 5;
            else
                return cal - 161;
        }

        public float CalTemp(float cal)
        {
            return cal + cal * 0.1F;
        }

        public float CaloriesCalculatorActive(float caloricDemand, Activity activity)
        {
            switch(activity)
            {
                case Activity.Active_not:
                    caloricDemand *= 1.2F;
                    break;
                case Activity.Active_light:
                    caloricDemand *= 1.4F;
                    break;
                case Activity.Active_average:
                    caloricDemand *= 1.6F;
                    break;
                case Activity.Active:
                    caloricDemand *= 1.75F;
                    break;
                case Activity.Active_big:
                    caloricDemand *= 1.9F;
                    break;
                case Activity.Active_huge:
                    caloricDemand *= 2.15F;
                    break;
            }

            return caloricDemand;
        }

        public float CaloriesCalculatorChoose(float caloricDemand, Choose choice)
        {
            switch (choice)
            {
                case Choose.Gain_1kg:
                    caloricDemand += 1000;
                    break;
                case Choose.Gain_05kg:
                    caloricDemand += 500;
                    break;                
                case Choose.Loose_05kg:
                    caloricDemand -= 500;
                    break;
                case Choose.Loose_1kg:
                    caloricDemand -= 1000;
                    break;
            }

            return caloricDemand;
        }

        // GET: CaloriesCounter/Form
        public ActionResult Form(string ChangeForm = "not")
        {
            if (ChangeForm == "yes")
            {
                string user = User.Identity.GetUserId();
                CaloriesCounter userData = _context.CaloriesCounters.FirstOrDefault(d => d.UserId == user);

                if (userData == null)
                    return View();

                var userMenu = _context.UserMenus.FirstOrDefault(d => d.CaloriesCounterId == userData.CaloriesCounterId && d.Date == DateTime.Today);

                if (userData == null || userMenu == null || user == null)
                    return View();

                foreach (FoodSaves food in _context.FoodSaveses.Where(d => d.UserMenuId == userMenu.UserMenuId))
                {
                    if (food != null)
                        _context.FoodSaveses.Remove(food);
                }

                foreach (CaloriesTraining training in _context.CaloriesTrainings.Where(d => d.UserMenuId == userMenu.UserMenuId))
                {
                    if (training != null)
                        _context.CaloriesTrainings.Remove(training);
                }

                foreach (UserMenu menus in _context.UserMenus.Where(d => d.CaloriesCounterId == userData.CaloriesCounterId))
                {
                    if (menus != null)
                        _context.UserMenus.Remove(menus);
                }
                _context.CaloriesCounters.Remove(userData);
                _context.SaveChanges();

                return View();
            }

            return View();
        }

        // POST: CaloriesCounter/Form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Form(CaloriesCounter CaloriesCounter)
        {
            if (!ModelState.IsValid)
                return View("Form", CaloriesCounter);
            else
            {
                var caloriesCounter = CaloriesCounter;
                CaloriesCalculator(ref caloriesCounter);

                var currentUserId = User.Identity.GetUserId();
                var caloriesCounterInfo = _context.CaloriesCounters.FirstOrDefault(d => d.UserId == currentUserId);

                if (caloriesCounterInfo == null)
                {
                    caloriesCounterInfo = _context.CaloriesCounters.Create();
                    caloriesCounterInfo.UserId = currentUserId;
                }
                caloriesCounter.UserId = caloriesCounterInfo.UserId;

                _context.CaloriesCounters.Add(caloriesCounter);
                _context.SaveChanges();

                return RedirectToAction("IndexUserMenu", "UserMenu", new { id = caloriesCounter.UserId });
            }
        }
    }
}