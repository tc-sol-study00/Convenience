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
        private readonly IChumonService _chumonService;

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
        /// <param name="chumonService">注文サービスクラスＤＩ注入用</param>
        public ChumonController(ConvenienceContext context, IChumonService chumonService) {
            _context = context;
            _chumonService = chumonService;
            //chumonService = new ChumonService(_context);
        }

        /// <summary>
        /// <para>①キー入力画面の初期表示処理</para>
        /// <para>③キー入力Post受信結果の初期明細画面表示</para>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>remarks参照</returns>
        [HttpGet]
        public async Task<IActionResult> KeyInput(string id) {
            
            if ((id ?? string.Empty).Equals("Result")) {
                //③キー入力Post受信結果の初期明細画面表示
                ChumonViewModel chumonViewModel
                    = ISharedTools.ConvertFromSerial<ChumonViewModel>(TempData[IndexName]?.ToString() ?? throw new Exception("tempdataなし"));
                ViewBag.HandlingFlg = "FirstDisplay";
                TempData[IndexName] = ISharedTools.ConvertToSerial(chumonViewModel);
                ViewBag.FocusPosition = "#ChumonJisseki_ChumonJissekiMeisais_0__ChumonSu";
                //④に飛ぶ
                return View("ChumonMeisai", chumonViewModel);
            }
            //①キー入力画面の初期表示処理
            ChumonKeysViewModel keymodel = await _chumonService.SetChumonKeysViewModel();
            ViewBag.FocusPosition = "#ShiireSakiId";
            //②に飛ぶ
            return View(keymodel);
        }

        /// <summary>
        /// <para>②商品注文１枚目のPost受信後処理</para>
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
            ChumonViewModel chumonViewModel = await _chumonService.ChumonSetting(inChumonKeysViewModel);
            TempData[IndexName] = ISharedTools.ConvertToSerial(chumonViewModel);
            //③に飛ぶ
            return RedirectToAction("KeyInput", new { id = "Result" });
        }

        /// <summary>
        ///  ④商品注文明細画面Post後の処理
        /// </summary>
        /// <param name="id"></param>
        /// <param name="inChumonViewModel">注文明細ビューモデル</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChumonMeisai(ChumonViewModel inChumonViewModel) {

            if (!ModelState.IsValid) {
                throw new PostDataInValidException("注文明細画面");
            };
            //ModelState.Clear();
            if (inChumonViewModel.ChumonJisseki.ChumonJissekiMeisais == null) {
                throw new PostDataInValidException("注文明細画面");
            }
            //注文データをDBに書き込む
            ChumonViewModel ChumonViewModel
                = await _chumonService.ChumonCommit(inChumonViewModel);
            //Resultに注文明細ビューモデルを引き渡す
            TempData[IndexName] = ISharedTools.ConvertToSerial(ChumonViewModel);
            //⑤に飛ぶ
            return RedirectToAction("ChumonMeisai", new { id = "Result" });
        }

        /// <summary>
        /// ⑤商品注文明細画面Post後の処理の初期表示
        /// </summary>
        /// <param name="inChumonViewModel">初期表示する注文明細ビューデータ</param>
        /// <returns>商品注文２枚目＋Post後の処理結果</returns>
        [HttpGet]
        public IActionResult ChumonMeisai(string id) {
            if ((id ?? string.Empty).Equals("Result")) {
                ViewBag.HandlingFlg = "SecondDisplay";
                //Redirect前のデータを引き継ぐ
                if (TempData.Peek(IndexName) != null) {
                    ChumonViewModel chumonViewModel =
                        ISharedTools.ConvertFromSerial<ChumonViewModel>(TempData[IndexName]?.ToString()
                        ?? throw new Exception("tempdataなし"));
                    TempData[IndexName] = ISharedTools.ConvertToSerial(chumonViewModel);
                    //④に飛ぶ
                    return View("ChumonMeisai", chumonViewModel);
                }
                else {
                    //何かおかしい場合は、自分を呼ぶ
                    return RedirectToAction("ChumonMeisai");
                }
            }
            return NotFound("処理がありません");
        }

        [HttpGet]
        public IActionResult Exception(string id) {
            ViewBag.Data = 1;
            ViewData["Data"] = 2;
            TempData["Data"] = 3;

            int i=ViewBag.Data;
            int j=(int)ViewData["Data"];    
            int k=(int)TempData["Data"];


            throw new NoDataFoundException("Post");
            //return RedirectToAction("Exception");
        }
    }
}