using Convenience.Data;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.ViewModels.Chumon;
using Microsoft.AspNetCore.Mvc;

namespace Convenience.Controllers {
    /// <summary>
    /// 注文コントローラ
    /// </summary>
    public class ChumonController : Controller, ISharedTools {
        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly ConvenienceContext _context;

        /// <summary>
        /// サービスクラス引継ぎ用キーワード
        /// </summary>
        private static readonly string IndexName = "ChumonViewModel";

        /// <summary>
        /// 注文サービスクラス（ＤＩ用）
        /// </summary>
        private readonly IChumonService chumonService;

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
        /// <param name="chumonService">注文サービスクラスＤＩ注入用</param>
        public ChumonController(ConvenienceContext context, IChumonService chumonService) {
            this._context = context;
            this.chumonService = chumonService;
            //chumonService = new ChumonService(_context);
        }

        /// <summary>
        /// 商品注文１枚目の初期表示処理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> KeyInput(string id) {
            if ((id??string.Empty).Equals("Result")) {
                ChumonViewModel chumonViewModel 
                    = ISharedTools.ConvertFromSerial<ChumonViewModel>((string)TempData[IndexName]);
                ViewBag.HandlingFlg = "FirstDisplay";
                TempData[IndexName] = ISharedTools.ConvertToSerial(chumonViewModel);
                ViewBag.FocusPosition = "#ChumonJisseki_ChumonJissekiMeisais_0__ChumonSu";
                return View("ChumonMeisai",chumonViewModel);
            }
            ChumonKeysViewModel keymodel = await chumonService.SetChumonKeysViewModel();
            ViewBag.FocusPosition = "#ShiireSakiId";
            return View(keymodel);
        }

        /// <summary>
        /// 商品注文１枚目のPost受信後処理
        /// </summary>
        /// <param name="inChumonKeysViewModel">注文キービューモデル</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KeyInput(ChumonKeysViewModel inChumonKeysViewModel) {

            if (!ModelState.IsValid) {
                throw new InvalidOperationException("Postデータエラー");
            }

            // 注文セッティング
            ChumonViewModel chumonViewModel = await chumonService.ChumonSetting(inChumonKeysViewModel);
            //ViewBag.HandlingFlg = "FirstDisplay";
            //return View("ChumonMeisai", chumonViewModel);
            TempData[IndexName]=ISharedTools.ConvertToSerial(chumonViewModel);
            return RedirectToAction("KeyInput", new { id = "Result" });
        }


        /// <summary>
        /// 商品注文２枚目の初期表示（表示データは、postを受けたKeyInputメソッドで行う）
        /// </summary>
        /// <param name="inChumonViewModel">初期表示する注文明細ビューデータ</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ChumonMeisai(string id) {
            if ((id ?? string.Empty).Equals("Result")) {
                ViewBag.HandlingFlg = "SecondDisplay";
                //Redirect前のデータを引き継ぐ
                if (TempData.Peek(IndexName) != null) {
                    ChumonViewModel chumonViewModel = ISharedTools.ConvertFromSerial<ChumonViewModel>(TempData[IndexName] as string);
                    TempData[IndexName] = ISharedTools.ConvertToSerial(chumonViewModel);
                    return View("ChumonMeisai", chumonViewModel);
                }
                else {
                    return RedirectToAction("ChumonMeisai");
                }
            }
            return NotFound("処理がありません");
        }

        /// <summary>
        ///  商品注文２枚目のPost後の処理
        /// </summary>
        /// <param name="id"></param>
        /// <param name="inChumonViewModel">注文明細ビューモデル</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChumonMeisai(ChumonViewModel inChumonViewModel) {

            if (!ModelState.IsValid) {
                throw new PostDataInValidException("Postデータエラー");
            };
            //ModelState.Clear();
            if (inChumonViewModel.ChumonJisseki.ChumonJissekiMeisais == null) {
                throw new PostDataInValidException("Postデータなし");
            }
            //注文データをDBに書き込む
            ChumonViewModel ChumonViewModel
                = await chumonService.ChumonCommit(inChumonViewModel);
            //Resultに注文明細ビューモデルを引き渡す
            TempData[IndexName] = ISharedTools.ConvertToSerial(ChumonViewModel);
            return RedirectToAction("ChumonMeisai", new { id = "Result"});
        }
    }
}