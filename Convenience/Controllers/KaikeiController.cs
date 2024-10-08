using Convenience.Data;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.Services;
using Convenience.Models.ViewModels.Kaikei;
using Convenience.Models.ViewModels.TentoHaraidashi;
using Microsoft.AspNetCore.Mvc;

namespace Convenience.Controllers {
    /// <summary>
    /// 注文コントローラ
    /// </summary>
    public class KaikeiController : Controller, ISharedTools {
        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly ConvenienceContext _context;

        /// <summary>
        /// サービスクラス引継ぎ用キーワード
        /// </summary>
        private static readonly string IndexName = "KaikeiViewModel";

        /// <summary>
        /// 会計サービスクラス（ＤＩ用）
        /// </summary>
        private readonly IKaikeiService KaikeiService;

        /// <summary>
        /// ビュー・モデル
        /// </summary>
        private KaikeiViewModel KaikeiViewModel;

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
        /// <param name="chumonService">会計サービスクラスＤＩ注入用</param>
        public KaikeiController(ConvenienceContext context, IKaikeiService KaikeiService) {
            this._context = context;
            this.KaikeiService = KaikeiService;
        }

        /// <summary>
        /// 商品注文１枚目の初期表示処理
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> KeyInput() {

            KaikeiViewModel kaikeiViewModel = await KaikeiService.SetKaikeiViewModel();
            ViewBag.HandlingFlg = "FirstDisplay";
            ViewBag.BottunContext = "検索";
            ViewData["Action"] = "KeyInput";
            return View("Kaikei", kaikeiViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KeyInput(KaikeiViewModel argKaikeiViewModel) {

            ModelState.Clear();

            KaikeiViewModel kaikeiViewModel =
                await KaikeiService.KaikeiSetting(argKaikeiViewModel);
            ViewBag.HandlingFlg = "FirstDisplay";
            ViewBag.BottunContext = "追加";
            ViewData["Action"] = "Kaikei";
            return View("Kaikei", kaikeiViewModel);
        }

        /// <summary>
        /// 商品注文２枚目のPost受信後処理
        /// </summary>
        /// <param name="inChumonKeysViewModel">注文キービューモデル</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Addkaikei(KaikeiViewModel argKaikeiViewModel) {

            //if (!ModelState.IsValid) {
            //    throw new InvalidOperationException("Postデータエラー");
            //}

            ModelState.Clear();

            KaikeiViewModel kaikeiViewModel =
                await KaikeiService.KaikeiAdd(argKaikeiViewModel);
            //PRG用ビュー・モデル引き渡し
            //TempData[IndexName] = ISharedTools.ConvertToSerial(tentoHaraidashiViewModel);
            ViewBag.HandlingFlg = "SecondDisplay";
            ViewBag.BottunContext = "追加";
            ViewData["Action"] = "Kaikei";
            return View("Kaikei", kaikeiViewModel);
            //return RedirectToAction("Result");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Kaikei(KaikeiViewModel argKaikeiViewModel) {


            ModelState.Clear();

            KaikeiViewModel kaikeiViewModel =
                await KaikeiService.KaikeiCommit(argKaikeiViewModel);

            ViewBag.HandlingFlg = "SecondDisplay";
            ViewBag.BottunContext = "追加";
            ViewData["Action"] = "Kaikei";

            return View("Kaikei", kaikeiViewModel);
        }


            /// <summary>
            /// 店頭払出画面２枚目の初期表示（店頭払出画面２枚目のPost後処理よりredirect）
            /// </summary>
            /// <returns></returns>
            /*
            [HttpGet]
            public async Task<IActionResult> Result() {
                ViewBag.HandlingFlg = "SecondDisplay";
                ViewData["Action"] = "TentoHaraidashi";

                if (TempData.Peek(IndexName) != null) {
                    //PRG用ビュー・モデル引き取り    
                    tentoHaraidashiViewModel = ISharedTools.ConvertFromSerial<TentoHaraidashiViewModel>(TempData[IndexName] as string);
                    TempData[IndexName] = ISharedTools.ConvertToSerial(tentoHaraidashiViewModel);
                    ViewBag.BottunContext = "更新";
                    return View("TentoHaraidashi", tentoHaraidashiViewModel);
                }
                else {
                    return RedirectToAction("TentoHaraidashi");
                }
            }
            */

        }
    }