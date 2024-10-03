using Convenience.Data;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.Services;
using Convenience.Models.ViewModels.Chumon;
using Convenience.Models.ViewModels.Shiire;
using Convenience.Models.ViewModels.TentoHaraidashi;
using Microsoft.AspNetCore.Mvc;

namespace Convenience.Controllers {
    /// <summary>
    /// 注文コントローラ
    /// </summary>
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
        private readonly TentoHaraidashiService tentoHaraidashiService;

        /// <summary>
        /// ビュー・モデル
        /// </summary>
        private TentoHaraidashiViewModel tentoHaraidashiViewModel;

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
        /// <param name="chumonService">注文サービスクラスＤＩ注入用</param>
        public TentoHaraidashiController(ConvenienceContext context) {
            this._context = context;
            this.tentoHaraidashiService = new TentoHaraidashiService(_context);
        }

        /// <summary>
        /// 商品注文１枚目の初期表示処理
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> KeyInput() {

            TentoHaraidashiViewModel tentoHaraidashiViewModel = await tentoHaraidashiService.SetTentoHaraidashiViewModel();
            ViewBag.HandlingFlg = "FirstDisplay";
            ViewData["Action"] = "KeyInput";
            return View("TentoHaraidashi", tentoHaraidashiViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KeyInput(TentoHaraidashiViewModel argTentoHaraidashiViewModel)
        {
            TentoHaraidashiViewModel tentoHaraidashiViewModel = await tentoHaraidashiService.TentoHaraidashiSetting(argTentoHaraidashiViewModel);
            ViewBag.HandlingFlg = "FirstDisplay";
            ViewData["Action"] = "TentoHaraidashi";
            return View("TentoHaraidashi", tentoHaraidashiViewModel);
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
            TentoHaraidashiViewModel tentoHaraidashiViewModel = await tentoHaraidashiService.TentoHaraidashiCommit(argTentoHaraidashiViewModel);
            //PRG用ビュー・モデル引き渡し
            TempData[IndexName] = ISharedTools.ConvertToSerial(tentoHaraidashiViewModel);
            return RedirectToAction("Result");
        }

        /// <summary>
        /// 店頭払出画面２枚目の初期表示（店頭払出画面２枚目のPost後処理よりredirect）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Result()
        {
            ViewBag.HandlingFlg = "SecondDisplay";
            ViewData["Action"] = "TentoHaraidashi";

            if (TempData.Peek(IndexName) != null)
            {
                //PRG用ビュー・モデル引き取り    
                tentoHaraidashiViewModel = ISharedTools.ConvertFromSerial<TentoHaraidashiViewModel>(TempData[IndexName] as string);
                TempData[IndexName] = ISharedTools.ConvertToSerial(tentoHaraidashiViewModel);
                return View("TentoHaraidashi", tentoHaraidashiViewModel);
            }
            else
            {
                return RedirectToAction("TentoHaraidashi");
            }
        }

    }
}