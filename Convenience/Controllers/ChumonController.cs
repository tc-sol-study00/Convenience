using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.Services;
using Convenience.Models.ViewModels.Chumon;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Convenience.Models.Properties.Message;


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

        public async Task<IActionResult> KeyInput() {
            ChumonKeysViewModel keymodel = await chumonService.SetChumonKeysViewModel();
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
            ViewBag.HandlingFlg = "FirstDisplay";
            return View("ChumonMeisai", chumonViewModel);
        }

        /// <summary>
        /// 商品注文２枚目の初期表示（表示データは、postを受けたKeyInputメソッドで行う）
        /// </summary>
        /// <param name="inChumonViewModel">初期表示する注文明細ビューデータ</param>
        /// <returns></returns>
        public async Task<IActionResult> ChumonMeisai(ChumonViewModel inChumonViewModel) {
            return View(inChumonViewModel);
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
        public async Task<IActionResult> ChumonMeisai(int id, ChumonViewModel inChumonViewModel) {

            if (!ModelState.IsValid) {
                throw new Exception("Postデータエラー");
            };
            //ModelState.Clear();
            if (inChumonViewModel.ChumonJisseki.ChumonJissekiMeisais == null) {
                throw new Exception("Postデータなし");
            }
            //注文データをDBに書き込む
            ChumonViewModel ChumonViewModel
                = await chumonService.ChumonCommit(inChumonViewModel);
            //Resultに注文明細ビューモデルを引き渡す
            TempData[IndexName] = ISharedTools.ConvertToSerial(ChumonViewModel);
            return RedirectToAction("Result");
        }

        /// <summary>
        /// 商品注文２枚目のPostデータコミット後の再表示
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Result() {
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
    }
}