using Calories_Life_2.DAL;
using Calories_Life_2.Models;
using Calories_Life_2.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Calories_Life_2.Controllers
{
    public class FoodController : Controller
    {

        private CaloriesLifeContext _context;

        public FoodController()
        {
            _context = new CaloriesLifeContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: /Food/id
        public ActionResult IndexFood(int? id, ManageMessageId? message)
        {   
            if (id == null)
            {
                string userId = User.Identity.GetUserId();
                ReturnUserId(ref id, userId);
            }

            ViewBag.StatusMessage =
                message == ManageMessageId.SearchQueryFail ? "You must enter your food, name should be less than 300 characters"
                : message == ManageMessageId.ChangeSizeOfFoodFail ? "You must put a number to edit size field"
                : "";

            var viewmodel = new FoodSavesViewModel();

            var meals = new List<Meal>()
            {
                new Meal(){Id = 1, Text = "Breakfast"},
                new Meal(){Id = 2, Text = "Dinner"},
                new Meal(){Id = 3, Text = "Supper"},
                new Meal(){Id = 4, Text = "Snacks"},
            };
            ViewBag.list = meals;

            if (IsEntry(id, "food") == true)
            {
                var ListOfFoods = _context.FoodSaveses.Where(f => f.UserMenuId == id).ToList();
                viewmodel.FoodSaveses = _context.FoodSaveses.Where(f => f.UserMenuId == id).ToList(); ;

                if (ListOfFoods[0].Day == DateTime.Today)
                {                    
                    for(int i = 0; i < ListOfFoods.Count(); i++)
                    {
                        viewmodel.SumKcal += (short)ListOfFoods[i].Kcal;
                        viewmodel.SumFat += (short)ListOfFoods[i].Fat;
                        viewmodel.SumCarbs += (short)ListOfFoods[i].Carbs;
                        viewmodel.SumProteins += (short)ListOfFoods[i].Proteins;
                    }             
                }
                return View(viewmodel);
            }

            return View(viewmodel);
        }

        public bool IsEntry(int? id, string type)
        {
            if (type == "food")
                return _context.FoodSaveses.Any(u => u.UserMenuId == id && u.UserMenu.Date == DateTime.Today);
            else
                return _context.CaloriesTrainings.Any(u => u.UserMenuId == id && u.UserMenu.Date == DateTime.Today);
        }

        public ActionResult ReturnUserId(ref int? id, string userId)
        {
            var userMenuId = _context.UserMenus.FirstOrDefault(d => d.CaloriesCounter.UserId == userId && d.Date == DateTime.Today);
            if (userMenuId == null)
            {
                return RedirectToAction("IndexUserMenu", "UserMenu", new { id = userId});
            }
            id = userMenuId.UserMenuId;
            return new EmptyResult();
        }

        // GET: AutocompleteToFood
        public IList<Type> SerializingJson<Type>(IList<JToken> listOfJTokenObjects)
        {
            // serialize JSON results into .NET objects
            IList<Type> returnList = new List<Type>();
            foreach (JToken result in listOfJTokenObjects)
            {
                // JToken.ToObject is a helper method that uses JsonSerializer internally
                Type searchResult = result.ToObject<Type>();
                returnList.Add(searchResult);
            }

            return returnList;
        }

        private IEnumerable<object> RequestToNutritionix(string term)
        {
            var client = new RestClient("https://trackapi.nutritionix.com/v2/search/instant");
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-app-id", "f178526d");
            request.AddHeader("x-app-key", "3dfc5f1283d40b183ccd20b624c7847d");
            request.AddParameter("query", term);
            IRestResponse response = client.Execute(request);

            JObject foodResearch = JObject.Parse(response.Content);

            //get JSON result objects into a list

            //BRANDED Products class
            var branded = SerializingJson<JsonModels.Branded>(foodResearch["branded"].Children().ToList());
            //COMMON Products class
            var common = SerializingJson<JsonModels.Common>(foodResearch["common"].Children().ToList());

            //Merging two lists
            IList<JsonModels.Merged> merged = new List<JsonModels.Merged>();

            if (common != null)
            {
                for (int i = 0; (i < common.Count()) && (i < 6); i++)
                {
                    merged.Add(new JsonModels.Merged
                    {
                        FoodName = common[i].food_name
                    });
                }
            }

            if (branded != null)
            {
                for (int i = 0; (i < branded.Count()) && (i < 6); i++)
                {
                    merged.Add(new JsonModels.Merged
                    {
                        FoodName = branded[i].food_name + " by " + branded[i].brand_name
                    });
                }
            }

            return merged.Select(a => new { label = a.FoodName });
        }

        public ActionResult FoodAutocomplete(string term)
        {
            return Json(RequestToNutritionix(term), JsonRequestBehavior.AllowGet);
        }

        // AddingFood
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFood(FoodSavesViewModel data)
        {
            if (ModelState.IsValid)
            {
                string id = "0";

                if (data.caloriesFoodEnterWords.SearchQuery.Contains("by"))
                    id = GetId(data.caloriesFoodEnterWords.SearchQuery);

                if (id != "0")
                {
                    SearchItemBrandedFood(id, data.caloriesFoodEnterWords.Meal);
                }
                else
                {
                    SearchNutrientsCommonFood(data.caloriesFoodEnterWords.SearchQuery, data.caloriesFoodEnterWords.Meal);
                }

                return RedirectToAction("IndexFood", "Food", new { id = User.Identity.GetUserId() });
            }
            else
            {
                return RedirectToAction("IndexFood", new { id = User.Identity.GetUserId(), Message = ManageMessageId.SearchQueryFail });
            }

        }

        private string GetId(string searchQuery)
        {
            var client = new RestClient("https://trackapi.nutritionix.com/v2/search/instant");
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-app-id", "f178526d");
            request.AddHeader("x-app-key", "3dfc5f1283d40b183ccd20b624c7847d");
            request.AddHeader("x-remote-user-id", "0");
            request.AddParameter("query", searchQuery);
            IRestResponse response = client.Execute(request);

            JObject foodResearch = JObject.Parse(response.Content);
            //BRANDED Products class
            var branded = SerializingJson<JsonModels.Branded>(foodResearch["branded"].Children().ToList());

            JsonModels.Merged brandFood = new JsonModels.Merged();
            brandFood.BrandedId = branded[0].nix_item_id;

            return brandFood.BrandedId;
        }

        private void SearchItemBrandedFood(string id, int meal)
        {
            var client = new RestClient("https://trackapi.nutritionix.com/v2/search/item");
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-app-id", "f178526d");
            request.AddHeader("x-app-key", "3dfc5f1283d40b183ccd20b624c7847d");
            request.AddHeader("x-remote-user-id", "0");
            request.AddParameter("nix_item_id", id);
            IRestResponse response = client.Execute(request);

            if (response.StatusCode.ToString() != "NotFound")
            {
                JObject foodBranded = JObject.Parse(response.Content);

                // get JSON result objects into a list
                var foodObject = SerializingJson<JsonModels.Food>(foodBranded["foods"].Children().ToList());

                if (foodObject[0].nf_protein != null && foodObject[0].nf_total_carbohydrate != null && foodObject[0].nf_total_fat != null)
                    AddNewFoodModel(foodObject, meal);
            }
        }

        private void SearchNutrientsCommonFood(string searchQuery, int meal)
        {
            var client = new RestClient("https://trackapi.nutritionix.com/v2/natural/nutrients");
            var request = new RestRequest(Method.POST)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddJsonBody(new { query = searchQuery }); // uses JsonSerializer
            request.AddHeader("x-app-id", "f178526d");
            request.AddHeader("x-app-key", "3dfc5f1283d40b183ccd20b624c7847d");
            request.AddHeader("x-remote-user-id", "0");
            IRestResponse response = client.Execute(request);

            if (response.StatusCode.ToString() != "NotFound")
            {
                JObject foodCommon = JObject.Parse(response.Content);

                // get JSON result objects into a list
                var foodObject = SerializingJson<JsonModels.Food>(foodCommon["foods"].Children().ToList());

                if (foodObject[0].nf_protein != null && foodObject[0].nf_total_carbohydrate != null && foodObject[0].nf_total_fat!= null)
                    AddNewFoodModel(foodObject, meal);
            }
        }

        private void ChangingServingToGrams(ref IList<JsonModels.Food> food)
        {
            food[0].serving_qty = 1;
            food[0].serving_unit = "100 grams";

            food[0].nf_calories /= food[0].serving_weight_grams;
            food[0].nf_protein /= food[0].serving_weight_grams;
            food[0].nf_total_carbohydrate /= food[0].serving_weight_grams;
            food[0].nf_total_fat /= food[0].serving_weight_grams;

            food[0].nf_calories *= 100;
            food[0].nf_protein *= 100;
            food[0].nf_total_carbohydrate *= 100;
            food[0].nf_total_fat *= 100;

            food[0].serving_weight_grams = 100;
        }

        private void AddNewFoodModel(IList<JsonModels.Food> listOfFoods, int meal)
        {
            var currentId = User.Identity.GetUserId();
            var userMenu = _context.UserMenus.FirstOrDefault(d => d.CaloriesCounter.UserId == currentId && d.Date == DateTime.Today);

            if (listOfFoods[0].serving_qty > 1 || listOfFoods[0].serving_qty < 1)
                ChangingServingToGrams(ref listOfFoods);

            if (listOfFoods[0].serving_weight_grams == null)
            {
                listOfFoods[0].serving_qty = 1;
                listOfFoods[0].serving_qty = 1;
                listOfFoods[0].serving_unit = "100 grams";
                listOfFoods[0].serving_weight_grams = 100;
            }
            FoodSaves food = new FoodSaves
            {
                UserMenuId = userMenu.UserMenuId,
                Name = listOfFoods[0].food_name,
                Carbs = (short)listOfFoods[0].nf_total_carbohydrate,
                Fat = (short)listOfFoods[0].nf_total_fat,
                Kcal = (short)listOfFoods[0].nf_calories,
                Proteins = (short)listOfFoods[0].nf_protein,

                CarbsDefault = (short)listOfFoods[0].nf_total_carbohydrate,
                FatDefault = (short)listOfFoods[0].nf_total_fat,
                KcalDefault = (short)listOfFoods[0].nf_calories,
                ProteinsDefault = (short)listOfFoods[0].nf_protein,

                ServingUnit = listOfFoods[0].serving_unit,
                ServingSize = listOfFoods[0].serving_qty,
                ServingWeightGrams = (short)listOfFoods[0].serving_weight_grams,

                Day = DateTime.Today,
                Meal = (byte)meal
            };

            if (listOfFoods[0].brand_name != null)
            {
                food.Name = listOfFoods[0].food_name + " by " + listOfFoods[0].brand_name;
            }

            if (food.ServingSize == 1 & food.ServingUnit == "100 grams")
                food.Is100Grams = true;
            else
                food.Is100Grams = false;

            food.Name = char.ToUpper(food.Name[0]) + food.Name.Substring(1);
            UpdatePlusSumOfNutriens(food);

            _context.FoodSaveses.Add(food);
            _context.SaveChanges();
        }

        //Removing Food records from Database
        public ActionResult RemovingFoodItem(int foodItemId)
        {
            var foodItem = _context.FoodSaveses.SingleOrDefault(item => item.FoodSavesId == foodItemId);

            if (foodItem == null)
                return HttpNotFound();

            UpdateMinusSumOfNutriens(foodItemId);
            _context.FoodSaveses.Remove(foodItem);
            _context.SaveChanges();
            
            return RedirectToAction("IndexFood", "Food");
        }

        //Changing Data Base Food Records
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeServingSize(string foodItemId, string param)
        {
            if(foodItemId == null || param == null || param == "")
                return RedirectToAction("IndexFood", "Food", new { id = User.Identity.GetUserId(), Message = ManageMessageId.ChangeSizeOfFoodFail });

            var sizing = double.Parse(param);
            var id = int.Parse(foodItemId);

            var food = _context.FoodSaveses.SingleOrDefault(item => item.FoodSavesId == id);
            if (food.ServingSize == sizing)
                return RedirectToAction("IndexFood", "Food", new { id = User.Identity.GetUserId() });

            UpdateMinusSumOfNutriens(food.FoodSavesId);
            food.Carbs = (short)(food.CarbsDefault * sizing);
            food.Proteins = (short)(food.ProteinsDefault * sizing);
            food.Fat = (short)(food.FatDefault * sizing);
            food.Kcal = (short)(food.KcalDefault * sizing);

            food.ServingSize = sizing;

            _context.SaveChanges();
            UpdatePlusSumOfNutriens(food);

            return RedirectToAction("IndexFood", "Food", new { id = User.Identity.GetUserId() });
        }

        //Updating UserMenu
        private void UpdatePlusSumOfNutriens(FoodSaves food)
        {
            var id = User.Identity.GetUserId();
            UserMenu sumOfNutriens = _context.UserMenus.FirstOrDefault(s => s.Date == DateTime.Today && s.CaloriesCounter.UserId == id);

            sumOfNutriens.CaloricDemandChangable -= (short)food.Kcal;
            sumOfNutriens.FatChangable -= (short)food.Fat;
            sumOfNutriens.CarbohydratesChangable -= (short)food.Carbs;
            sumOfNutriens.ProteinsChangable -= (short)food.Proteins;

            _context.SaveChanges();
        }

        private void UpdateMinusSumOfNutriens(int foodItemId)
        {
            var id = User.Identity.GetUserId();
            var foodItem = _context.FoodSaveses.FirstOrDefault(f => f.FoodSavesId == foodItemId);
            UserMenu sumOfNutriens = _context.UserMenus.FirstOrDefault(s => s.Date == DateTime.Today && s.CaloriesCounter.UserId == id);

            sumOfNutriens.CaloricDemandChangable += (short)foodItem.Kcal;
            sumOfNutriens.FatChangable += (short)foodItem.Fat;
            sumOfNutriens.CarbohydratesChangable += (short)foodItem.Carbs;
            sumOfNutriens.ProteinsChangable += (short)foodItem.Proteins;

            _context.SaveChanges();
        }

        public enum ManageMessageId
        {
            SearchQueryFail,
            ChangeSizeOfFoodFail
        }
    }
}