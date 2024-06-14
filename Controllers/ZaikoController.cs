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
            var keydata = inZaikoModel.KeyEventList;
            var selecteWhereItemArray = inZaikoModel.SelecteWhereItemArray;

            ZaikoViewModel zaikoViewModel = new ZaikoViewModel {
                zaikoListLine = await zaikoService.KeyInput(keydata, selecteWhereItemArray)
            };
            return View(zaikoViewModel);
        }
    }
}