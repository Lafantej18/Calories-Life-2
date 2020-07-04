using System.Web.Mvc;

namespace Calories_Life_2.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult About()
        {

            return View();
        }
    }
}