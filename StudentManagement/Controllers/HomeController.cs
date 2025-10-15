using Microsoft.AspNetCore.Mvc;
using StudentManagement.Services;

namespace StudentManagement.Controllers
{
    public class HomeController : BaseController
    {
        private readonly DataService _dataService;

        public HomeController(DataService dataService)
        {
            _dataService = dataService;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}