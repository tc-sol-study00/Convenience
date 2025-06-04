using Convenience.Data;
using Convenience.Models.Interfaces;
using Convenience.Models.ViewModels.Shiire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Convenience.Controllers {

    /// <summary>
    /// 仕入サービスクラス
    /// </summary>
    public class ShiireController : Controller, ISharedTools {
        private readonly ConvenienceContext _context;

        private readonly IShiireService _shiireService;

        private static readonly string IndexName = "ShiireViewModel";

        private ShiireViewModel shiireViewModel;

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
        /// <param name="shiireService">仕入サービスクラスＤＩ注入用</param>
        public ShiireController(ConvenienceContext context,IShiireService shiireService) {
            this._context = context;
            this._shiireService = shiireService;
            this.shiireViewModel = new ShiireViewModel();
            //shiireService = new ShiireService(_context);
        }

        /// <summary>
        /// <para>①仕入画面キー入力初期表示</para>
        /// <para>③仕入画面キー入力後の仕入明細画面初期表示</para>
        /// </summary>
        /// <returns>ShiireKeysViewModel 仕入キービューモデル</returns>
        /// 
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ShiireKeyInput(string id) {
            if ((id ?? string.Empty).Equals("Result")) {
                //③仕入画面キー入力Post後の明細入力画面初期表示
                ViewBag.HandlingFlg = "FirstDisplay";
                shiireViewModel = ISharedTools.ConvertFromSerial<ShiireViewModel>(TempData[IndexName]?.ToString()??throw new Exception("tempdataなし"));
                TempData.Keep(IndexName);
                ViewBag.FocusPosition = "#ShiireJissekis_0__NonyuSu";
                //④に飛ぶ
                return View("Shiire", shiireViewModel);
            }
            else {
                //①仕入画面キー入力初期表示
                ShiireKeysViewModel keymodel = await _shiireService.SetShiireKeysModel();
                ViewBag.FocusPosition = "#ChumonId";
                //②に飛ぶ
                return View(keymodel);
            }
        }

        /// <summary>
        /// ②仕入画面キー入力Post後処理
        /// </summary>
        /// <param name="inKeysModel">仕入画面１枚目のpostデータ</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShiireKeyInput(ShiireKeysViewModel inKeysModel) {
            shiireViewModel = await _shiireService.ShiireSetting(inKeysModel);
            TempData[IndexName] = ISharedTools.ConvertToSerial(shiireViewModel);
            //③に飛ぶ
            return RedirectToAction("ShiireKeyInput", new { id = "Result" });
        }
        /// <summary>
        /// ④仕入明細画面Post後処理
        /// </summary>
        /// <param name="inShiireViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Shiire(ShiireViewModel inShiireViewModel) {
            ModelState.Clear();

            ShiireViewModel shiireViewModel = await _shiireService.ShiireCommit(inShiireViewModel);

            ViewBag.HandlingFlg = "SecondDisplay";

            TempData[IndexName] = ISharedTools.ConvertToSerial(shiireViewModel);
            //⑤に飛ぶ
            return RedirectToAction("Shiire", new { id = "Result" });
        }
        /// <summary>
        /// ⑤仕入明細画面Post後の初期表示
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public IActionResult Shiire(string id) {
            if ((id ?? string.Empty).Equals("Result")) {
                ViewBag.HandlingFlg = "SecondDisplay";
                if (TempData.Peek(IndexName) != null) {
                    shiireViewModel = ISharedTools.ConvertFromSerial<ShiireViewModel>(TempData[IndexName]?.ToString()??throw new Exception("tepdataなし"));
                    TempData[IndexName] = ISharedTools.ConvertToSerial(shiireViewModel);
                    //④に飛ぶ
                    return View("Shiire", shiireViewModel);
                }
                else {
                    return RedirectToAction("Shiire");
                }
            }
            else {
                return NotFound("処理がありません");
            }
        }


    }
}