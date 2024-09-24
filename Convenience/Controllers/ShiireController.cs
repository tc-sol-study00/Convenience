using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.ViewModels.Shiire;
using Microsoft.AspNetCore.Mvc;
using static Convenience.Models.Properties.Message;

namespace Convenience.Controllers {

    /// <summary>
    /// 仕入サービスクラス
    /// </summary>
    public class ShiireController : Controller, ISharedTools {
        private readonly ConvenienceContext _context;

        private readonly IShiireService shiireService;

        private static readonly string IndexName = "ShiireViewModel";

        private ShiireViewModel shiireViewModel;

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
        /// <param name="shiireService">仕入サービスクラスＤＩ注入用</param>
        public ShiireController(ConvenienceContext context,IShiireService shiireService) {
            this._context = context;
            this.shiireService = shiireService;
            //shiireService = new ShiireService(_context);
        }

        public async Task<IActionResult> ShiireKeyInput() {
            ShiireKeysViewModel keymodel = await shiireService.SetShiireKeysModel();
            return View(keymodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShiireKeyInput(ShiireKeysViewModel inKeysModel) {
            shiireViewModel = await shiireService.ShiireSetting(inKeysModel);
            ViewBag.HandlingFlg = "FirstDisplay";
            return View("Shiire", shiireViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Shiire(ShiireViewModel inShiireViewModel) {
            ModelState.Clear();

            var shiireViewModel = await shiireService.ShiireCommit(inShiireViewModel);

            ViewBag.HandlingFlg = "SecondDisplay";

            TempData[IndexName] = ISharedTools.ConvertToSerial(shiireViewModel);
            return RedirectToAction("Result");
        }

        [HttpGet]
        public async Task<IActionResult> Result() {
            ViewBag.HandlingFlg = "SecondDisplay";
            if (TempData.Peek(IndexName) != null) {
                shiireViewModel = ISharedTools.ConvertFromSerial<ShiireViewModel>(TempData[IndexName] as string);
                TempData[IndexName] = ISharedTools.ConvertToSerial(shiireViewModel);
                return View("Shiire", shiireViewModel);
            }
            else {
                return RedirectToAction("Shiire");
            }
        }


    }
}