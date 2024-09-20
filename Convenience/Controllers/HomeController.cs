using Convenience.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Convenience.Controllers {

    /// <summary>
    /// Homeコントローラ（メニュー表示用）
    /// </summary>
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        public IActionResult Index() {
            return View(new Menu());
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [IgnoreAntiforgeryToken]
        public IActionResult Error(int id) {

            DateTime dateTime = DateTime.Now;

            IExceptionHandlerPathFeature? exceptionHandlerPathFeature =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            IStatusCodeReExecuteFeature? statusCodeReExecuteFeature =
                HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            ErrorViewModel errorViewModel = new ErrorViewModel() {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = id == 0 ? null : id,
                EventAt = dateTime,
                ExceptionHandlerPathFeature = exceptionHandlerPathFeature,
                StatusCodeReExecuteFeature = statusCodeReExecuteFeature
            };

            return View(errorViewModel);
        }
    }
}