using Convenience.Data;
using Convenience.Models.Interfaces;
using Convenience.Models.ViewModels.TentoHaraidashi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Convenience.Controllers {
    /// <summary>
    /// 店頭払出コントローラ
    /// </summary>
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class TentoHaraidashiController : Controller, ISharedTools {
        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly ConvenienceContext _context;

        /// <summary>
        /// サービスクラス引継ぎ用キーワード
        /// </summary>
        private static readonly string IndexName = "TentoHaraidashiViewModel";

        /// <summary>
        /// 注文サービスクラス（ＤＩ用）
        /// </summary>
        private readonly ITentoHaraidashiService _tentoHaraidashiService;

        /// <summary>
        /// ビュー・モデル
        /// </summary>
        private TentoHaraidashiViewModel tentoHaraidashiViewModel;

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
        /// <param name="chumonService">注文サービスクラスＤＩ注入用</param>
        public TentoHaraidashiController(ConvenienceContext context, ITentoHaraidashiService tentoHaraidashiService) {
            this._context = context;
            this._tentoHaraidashiService = tentoHaraidashiService;
            this.tentoHaraidashiViewModel = new TentoHaraidashiViewModel();
            //this._tentoHaraidashiService = new TentoHaraidashiService(_context);
        }

        /// <summary>
        /// 商品注文１枚目の初期表示処理
        /// </summary>
        /// <returns></returns>
        /// 

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> KeyInput(string id) {
            if ((id ?? string.Empty).Equals("Result")) {
                ViewBag.HandlingFlg = "FirstDisplay";
                ViewBag.BottunContext = "更新";
                ViewData["Action"] = "TentoHaraidashi";
                ViewBag.FocusPosition = "#ShohinMasters_0__ShiireMasters_0__TentoHaraidashiJissekis_0__HaraidashiCaseSu";
                tentoHaraidashiViewModel = ISharedTools.ConvertFromSerial<TentoHaraidashiViewModel>(TempData[IndexName]?.ToString() ?? throw new Exception("tempdataなし"));
                TempData.Keep(IndexName);
            } else {
                tentoHaraidashiViewModel = await _tentoHaraidashiService.SetTentoHaraidashiViewModel();
                ViewBag.HandlingFlg = "FirstDisplay";
                ViewBag.BottunContext = "検索";
                ViewData["Action"] = "KeyInput";
                ViewBag.FocusPosition = "#HaraidashiDateAndId";
            }
            return View("TentoHaraidashi", tentoHaraidashiViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KeyInput(TentoHaraidashiViewModel argTentoHaraidashiViewModel) {
            TentoHaraidashiViewModel tentoHaraidashiViewModel =
                await _tentoHaraidashiService.TentoHaraidashiSetting(argTentoHaraidashiViewModel);

            TempData[IndexName] = ISharedTools.ConvertToSerial(tentoHaraidashiViewModel);

            return RedirectToAction("KeyInput", new { id = "Result" });
            //return View("TentoHaraidashi", tentoHaraidashiViewModel);
        }

        /// <summary>
        /// 商品注文２枚目のPost受信後処理
        /// </summary>
        /// <param name="inChumonKeysViewModel">注文キービューモデル</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TentoHaraidashi(TentoHaraidashiViewModel argTentoHaraidashiViewModel) {

            //if (!ModelState.IsValid) {
            //    throw new InvalidOperationException("Postデータエラー");
            //}

            // 店頭払出実績更新
            TentoHaraidashiViewModel tentoHaraidashiViewModel =
                await _tentoHaraidashiService.TentoHaraidashiCommit(argTentoHaraidashiViewModel);
            //PRG用ビュー・モデル引き渡し
            TempData[IndexName] = ISharedTools.ConvertToSerial(tentoHaraidashiViewModel);
            return RedirectToAction("TentoHaraidashi", new { id = "Result" });
        }

        /// <summary>
        /// 店頭払出画面２枚目の初期表示（店頭払出画面２枚目のPost後処理よりredirect）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public IActionResult TentoHaraidashi(string id) {

            if ((id ?? string.Empty).Equals("Result")) {
                ViewBag.HandlingFlg = "SecondDisplay";
                ViewData["Action"] = "TentoHaraidashi";

                if (TempData.Peek(IndexName) != null) {
                    //PRG用ビュー・モデル引き取り    
                    tentoHaraidashiViewModel = ISharedTools.ConvertFromSerial<TentoHaraidashiViewModel>(TempData[IndexName]?.ToString() ?? throw new Exception("tempdataなし"));
                    TempData[IndexName] = ISharedTools.ConvertToSerial(tentoHaraidashiViewModel);
                    ViewBag.BottunContext = "更新";
                    return View("TentoHaraidashi", tentoHaraidashiViewModel);
                } else {
                    return RedirectToAction("TentoHaraidashi");
                }
            }
            return NotFound("処理なし");
        }

    }
}