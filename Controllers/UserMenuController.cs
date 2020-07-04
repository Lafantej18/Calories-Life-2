using Calories_Life_2.DAL;
using Calories_Life_2.Models;
using Microsoft.AspNet.Identity;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Calories_Life_2.ViewModels;

namespace Calories_Life_2.Controllers
{
    public class UserMenuController : Controller
    {
        private CaloriesLifeContext _context;

        public UserMenuController()
        {
            _context = new CaloriesLifeContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: UserMenu/IndexUserMenu/id
        public ActionResult IndexUserMenu(string id)
        {
            if (id == null)
                id = User.Identity.GetUserId();

            var counter = _context.CaloriesCounters.FirstOrDefault(d => d.UserId == id);

            if (counter == null)
                return RedirectToAction("Form", "CaloriesCounter");

            var macros = _context.UserMenus.FirstOrDefault(d => d.CaloriesCounter.UserId == id && d.Date == DateTime.Today);

            if (macros == null)
            {
                CreateUserMenu(ref macros, ref counter);
                _context.UserMenus.Add(macros);
                _context.SaveChanges();
            }

            return View(macros);
        }

        private void CreateUserMenu(ref UserMenu macros, ref CaloriesCounter counter)
        {
            macros = new UserMenu
            {
                Date = DateTime.Today,
                CaloriesCounterId = counter.CaloriesCounterId,
                CaloricDemandChangable = counter.CaloricDemand,
                FatChangable = counter.Fat,
                CarbohydratesChangable = counter.Carbohydrates,
                ProteinsChangable = counter.Proteins
            };
        }

        public ActionResult Autocomplete(string term)
        {
            return Json(RequestToNutritionix(term), JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<object> RequestToNutritionix(string term)
        {
            var client = new RestClient("https://trackapi.nutritionix.com/v2/search/instant");
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-app-id", "f178526d");
            request.AddHeader("x-app-key", "3dfc5f1283d40b183ccd20b624c7847d");
            request.AddHeader("x-remote-user-id", "0");
            request.AddParameter("query", term);
            IRestResponse response = client.Execute(request);

            JObject foodResearch = JObject.Parse(response.Content);

            //get JSON result objects into a list

            var methods = new FoodController();
            //BRANDED Products class
            // var branded = SerializingJson<JsonModels.Branded>(foodResearch["branded"].Children().ToList());

            //Merging two lists
            IList<JsonModels.Merged> merged = new List<JsonModels.Merged>();

            return merged.Select(a => new { label = a.FoodName });
        }

        // GET: UserMenu/Trainings/
        public ActionResult Trainings(ManageMessageId? message, int? id = null)
        {
            var methods = new FoodController();

            if (id == null)
            {
                string userId = User.Identity.GetUserId();
                methods.ReturnUserId(ref id, userId);
            }

            ViewBag.StatusMessage =
                message == ManageMessageId.SearchTermFail ? "You must enter data, name of exercise should be less than 300 characters," +
                " time is calculated in minutes"
                : message == ManageMessageId.NotFoundFail ? "There is no such exercise in database"
                : "";

            var viewmodel = new CaloriesTrainingsViewModel();

            if (methods.IsEntry(id, "exercises") == true)
            {
                var listOfExercises = _context.CaloriesTrainings.Where(f => f.UserMenuId == id).ToList();
                viewmodel.CaloriesTrainings = listOfExercises;

                if (listOfExercises[0].Day == DateTime.Today)
                {
                    for (int i = 0; i < listOfExercises.Count(); i++)
                        viewmodel.SumKcal += listOfExercises[i].ExerciseCalories;
                }
                return View(viewmodel);
            }

            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddExercise(CaloriesTrainingsViewModel viemodel)
        {
            if (ModelState.IsValid)
            {
                var id = User.Identity.GetUserId();
                var user = _context.CaloriesCounters.FirstOrDefault(d => d.UserId == id);
                string sex = "";

                if (user.GenderChoose == CaloriesCounter.Gender.Man)
                    sex = "male";
                else
                    sex = "female";

                var client = new RestClient("https://trackapi.nutritionix.com/v2/natural/exercise");
                var request = new RestRequest(Method.POST)
                {
                    RequestFormat = DataFormat.Json,
                };
                request.AddHeader("x-app-id", "f178526d");
                request.AddHeader("x-app-key", "3dfc5f1283d40b183ccd20b624c7847d");
                request.AddJsonBody(new
                {
                    query = viemodel.caloriesTrainingEnterWords.term + " " + viemodel.caloriesTrainingEnterWords.time.ToString() + " min",
                    gender = sex,
                    weight_kg = user.Weight,
                    height_cm = user.Height,
                    age = user.Age
                }); // uses JsonSerializer
                IRestResponse response = client.Execute(request);

                if (response.StatusCode.ToString() != "NotFound")
                {
                    JObject exercise = JObject.Parse(response.Content);

                    var methods = new FoodController();

                    // get JSON result objects into a list
                    var exerciseObject = methods.SerializingJson<JsonModels.Exercises>(exercise["exercises"].Children().ToList());

                    //walking is default response from nutitionix when databse can't find exercise
                    if ((exerciseObject[0].name != "walking" && viemodel.caloriesTrainingEnterWords.term != "walking") || viemodel.caloriesTrainingEnterWords.term == "walking")
                    {
                        AddNewExerciseModel(exerciseObject, viemodel.caloriesTrainingEnterWords.time);
                        return RedirectToAction("Trainings", "UserMenu", new { id = User.Identity.GetUserId() });
                    }
                    else
                        return RedirectToAction("Trainings", "UserMenu", new { id = User.Identity.GetUserId(), Message = ManageMessageId.NotFoundFail });

                }
                else
                    return RedirectToAction("Trainings", "UserMenu", new { id = User.Identity.GetUserId(), Message = ManageMessageId.NotFoundFail });
            }
            else
            {
                return RedirectToAction("Trainings", "UserMenu", new { id = User.Identity.GetUserId(), Message = ManageMessageId.SearchTermFail });
            }
        }

        private void AddNewExerciseModel(IList<JsonModels.Exercises> exerciseJson, short time)
        {
            var currentId = User.Identity.GetUserId();
            var userMenu = _context.UserMenus.FirstOrDefault(d => d.CaloriesCounter.UserId == currentId && d.Date == DateTime.Today);

            var training = new CaloriesTraining
            {
                UserMenuId = userMenu.UserMenuId,
                NameOfExercise = exerciseJson[0].name,
                ExerciseTime = time,
                ExerciseCalories = (short)exerciseJson[0].nf_calories,
                Day = DateTime.Today
            };
            UpdatePlusSumOfNutriens(training);

            _context.CaloriesTrainings.Add(training);
            _context.SaveChanges();
        }

        public ActionResult RemovingExercise(int exerciseId)
        {
            var exercise = _context.CaloriesTrainings.SingleOrDefault(item => item.CaloriesTrainingId == exerciseId);

            if (exercise == null)
                return HttpNotFound();

            UpdateMinusSumOfNutriens(exerciseId);
            _context.CaloriesTrainings.Remove(exercise);
            _context.SaveChanges();

            return RedirectToAction("Trainings", "UserMenu");
        }

        //Updating UserMenu
        private void UpdatePlusSumOfNutriens(CaloriesTraining training)
        {
            var id = User.Identity.GetUserId();
            UserMenu sumOfNutriens = _context.UserMenus.FirstOrDefault(s => s.Date == DateTime.Today && s.CaloriesCounter.UserId == id);

            sumOfNutriens.CaloricDemandChangable -= (short)training.ExerciseCalories;

            _context.SaveChanges();
        }
        private void UpdateMinusSumOfNutriens(int exerciseId)
        {
            var id = User.Identity.GetUserId();
            var foodItem = _context.CaloriesTrainings.FirstOrDefault(f => f.CaloriesTrainingId == exerciseId);
            UserMenu sumOfNutriens = _context.UserMenus.FirstOrDefault(s => s.Date == DateTime.Today && s.CaloriesCounter.UserId == id);

            sumOfNutriens.CaloricDemandChangable += (short)foodItem.ExerciseCalories;

            _context.SaveChanges();
        }

        public enum ManageMessageId
        {
            SearchTermFail,
            NotFoundFail
        }
    }
}
