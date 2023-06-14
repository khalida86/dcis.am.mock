using Dcis.Amfi.Isf.Mock.Constants;
using Dcis.Amfi.Isf.Mock.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace Dcis.Amfi.Isf.Mock.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IMemoryCache memoryCache, IConfiguration configuration) : base(memoryCache, configuration)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewData[ViewDataKeys.Environment] = _environment;

            return View();
        }

        public IActionResult Privacy()
        {
            ViewData[ViewDataKeys.Environment] = _environment;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            ViewData[ViewDataKeys.Environment] = _environment;
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}