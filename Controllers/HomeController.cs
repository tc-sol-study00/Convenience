using Convenience.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Convenience.Controllers {

    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        public IActionResult Index() {
            //int a = 0;
            //int b = 1 / a;
            return View(new Menu());
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [IgnoreAntiforgeryToken]
        public IActionResult Error(int id) {

            IExceptionHandlerPathFeature? exceptionHandlerPathFeature =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            IStatusCodeReExecuteFeature? statusCodeReExecuteFeature =
                HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            ErrorViewModel errorViewModel = new ErrorViewModel() {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = id == 0 ? null : id,
                ExceptionHandlerPathFeature = exceptionHandlerPathFeature,
                StatusCodeReExecuteFeature = statusCodeReExecuteFeature
            };

            return View(errorViewModel);
        }
    }
}