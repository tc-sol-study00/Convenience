using Convenience.Data;
using Convenience.Models.Services;
using Convenience.Models.ViewModels.Zaiko;
using Microsoft.AspNetCore.Mvc;

namespace Convenience.Controllers {

    public class ZaikoController : Controller {
        private readonly ConvenienceContext _context;

        private ZaikoService zaikoService;

        public ZaikoController(ConvenienceContext context) {
            _context = context;
            zaikoService = new ZaikoService(_context);
        }

        public async Task<IActionResult> Index() {
            ZaikoViewModel zaikoViewModel = new ZaikoViewModel { };
            return View(zaikoViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ZaikoViewModel inZaikoModel) {
            var key = inZaikoModel.KeyEventData;
            var inDesc = inZaikoModel.Descending;

            ZaikoViewModel zaikoViewModel = new ZaikoViewModel {
                zaikoListLine = await zaikoService.KeyInput(key, inDesc)
            };
            return View(zaikoViewModel);
        }
    }
}