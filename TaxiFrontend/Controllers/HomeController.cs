using System.Threading.Tasks;
using System.Web.Mvc;

namespace TaxiFrontend.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            return View();
        }
    }
}