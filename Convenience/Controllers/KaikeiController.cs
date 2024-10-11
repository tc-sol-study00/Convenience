using Convenience.Data;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.Services;
using Convenience.Models.ViewModels.Kaikei;
using Convenience.Models.ViewModels.TentoHaraidashi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Convenience.Controllers {
    /// <summary>
    /// 会計コントローラ
    /// </summary>
    
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
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
        private readonly IKaikeiService _kaikeiService;

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
            this._kaikeiService = KaikeiService;
        }

        /// <summary>
        /// 商品注文１枚目の初期表示処理
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> KeyInput() {

            /*
             * 会計ビューモデル設定
             */
            KaikeiViewModel kaikeiViewModel = await _kaikeiService.SetKaikeiViewModel();
            //PRG用ビュー・モデル引き渡し
            //TempData[IndexName] = ISharedTools.ConvertToSerial(kaikeiViewModel);
            ViewBag.FocusPosition = "#KaikeiDateAndId";    //最初のフォーカス位置
            ViewBag.HandlingFlg = "FirstDisplay";   ////アコーデオンを開いた状態にする
            ViewBag.BottunContext = "検索";         //ボタンを「検索」表示にする
            ViewData["Action"] = "KeyInput";        //postされたら、このメソッド(psot付き）に飛ぶ
            return View("Kaikei", kaikeiViewModel);
            TempData["id"] = "00:Init";
            //return RedirectToAction("Result", new { id = "01:KeyInput" });
        }

        /// <summary>
        /// 商品注文２枚目の初期表示
        /// </summary>
        /// <param name="argKaikeiViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KeyInput(KaikeiViewModel argKaikeiViewModel) {

            ModelState.Clear();
            /*
             * 会計セッティング
             */
            KaikeiViewModel kaikeiViewModel =
                await _kaikeiService.KaikeiSetting(argKaikeiViewModel);

            //PRG用ビュー・モデル引き渡し
            TempData[IndexName] = ISharedTools.ConvertToSerial(kaikeiViewModel);
            return RedirectToAction("Result", new { id = "02:PostedKeyInput" });
        }

        /// <summary>
        /// 商品注文２枚目のPost受信後処理
        /// </summary>
        /// <param name="inChumonKeysViewModel">注文キービューモデル</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Addkaikei(KaikeiViewModel argKaikeiViewModel) {

            //if (!ModelState.IsValid) {
            //    throw new InvalidOperationException("Postデータエラー");
            //}

            /*
             * 会計データの画面上の追加
             */
            KaikeiViewModel kaikeiViewModel =
                await _kaikeiService.KaikeiAdd(argKaikeiViewModel);
            
            //PRG用ビュー・モデル引き渡し
            TempData[IndexName]=ISharedTools.ConvertToSerial(kaikeiViewModel);
            return RedirectToAction("Result", new { id= "03:Addkaikei" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Kaikei(KaikeiViewModel argKaikeiViewModel) {

            KaikeiViewModel kaikeiViewModel =
                await _kaikeiService.KaikeiCommit(argKaikeiViewModel);

            ViewBag.HandlingFlg = "SecondDisplay";  //アコーデオンの状態をキープする
            ViewBag.BottunContext = "更新";         //ボタンを「更新」表示にする
            ViewData["Action"] = "Kaikei";        //postされたら、このメソッド(psot付き）に飛ぶ
            
            //PRG用ビュー・モデル引き渡し
            TempData[IndexName] = ISharedTools.ConvertToSerial(kaikeiViewModel);
            return RedirectToAction("Result", new { id = "04:Kaikei" });
        }

        /// <summary>
        /// PRG用
        /// </summary>
        /// <param name="id">getされるデータ（どこから来たかキーワード）</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Result(string id) {

            if (id.CompareTo((string)TempData["id"])<0) {
                id = (string)TempData["id"];
                return RedirectToAction("Result", new { id = id });
            }
            if (TempData.Peek(IndexName) != null) {
                var kaikeiViewModel = ISharedTools.ConvertFromSerial<KaikeiViewModel>(TempData[IndexName] as string);
                /*
                 * 描画前設定
                 */
                switch (id) {
                    case string s when s.EndsWith(":"+"KeyInput"):
                        ViewBag.FocusPosition = "#KaikeiDateAndId";    //最初のフォーカス位置
                        ViewBag.HandlingFlg = "FirstDisplay";   ////アコーデオンを開いた状態にする
                        ViewBag.BottunContext = "検索";         //ボタンを「検索」表示にする
                        ViewData["Action"] = "KeyInput";        //postされたら、このメソッド(psot付き）に飛ぶ
                        break;
                        case string s when s.EndsWith(":" + "PostedKeyInput"):
                        ViewBag.FocusPosition = "#KaikeiJissekiforAdd_ShohinId";    //最初のフォーカス位置
                        ViewBag.HandlingFlg = "SecondDisplay";  //アコーデオンの状態をキープする
                        ViewBag.BottunContext = "更新";         //ボタンを「更新」表示にする
                        ViewData["Action"] = "Kaikei";          //postされたら、このメソッド(psot付き）に飛ぶ
                        break;
                    case string s when s.EndsWith(":" + "Addkaikei"):
                        ViewBag.FocusPosition = "#KaikeiJissekiforAdd_ShohinId";    //最初のフォーカス位置
                        ViewBag.HandlingFlg = "SecondDisplay";  //アコーデオンの状態をキープする
                        ViewBag.BottunContext = "更新";         //ボタンを「更新」表示にする
                        ViewData["Action"] = "Kaikei";          //postされたら、このメソッド(psot付き）に飛ぶ
                        break;
                    default:
                        ViewBag.HandlingFlg = "SecondDisplay";  //アコーデオンの状態をキープする
                        ViewBag.BottunContext = "更新";         //ボタンを「更新」表示にする
                        ViewData["Action"] = "Kaikei";          //postされたら、このメソッド(psot付き）に飛ぶ
                        break;
                }
                TempData[IndexName] = ISharedTools.ConvertToSerial(kaikeiViewModel);
                TempData["id"] = id;
                return View("Kaikei", kaikeiViewModel);
            }
            else {
                return RedirectToAction("Kaikei");
            }
        }

        /// <summary>
        /// 商品名称問い合わせよう
        /// </summary>
        /// <param name="ShohinId"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetShohinName([FromForm]string ShohinId) {
            var shohinName=await _kaikeiService.GetShohinName(ShohinId);
            return Content(shohinName);
        }

    }
}