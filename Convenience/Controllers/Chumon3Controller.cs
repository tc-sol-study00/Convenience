using Convenience.Data;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.ViewModels.Chumon;
using Microsoft.AspNetCore.Mvc;

namespace Convenience.Controllers {
    /// <summary>
    /// 注文コントローラ
    /// </summary>
    public class Chumon3Controller : Controller {
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
        public Chumon3Controller(ConvenienceContext context, IChumonService chumonService) {
            _context = context;
            _chumonService = chumonService;
            //chumonService = new ChumonService(_context);
        }

        /// <summary>
        /// <para>キー入力画面の初期表示処理</para>
        /// <para>キー入力Post受信結果の初期明細画面表示</para>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>remarks参照</returns>
        [HttpGet]
        public async Task<IActionResult> KeyInput(string id) {
            //①キー入力画面の初期表示処理
            ChumonKeysViewModel keymodel = await _chumonService.SetChumonKeysViewModel();
            //②に飛ぶ
            return View("/Views/Chumon/KeyInput.cshtml",keymodel);
        }

        /// <summary>
        /// <para>商品注文１枚目のPost受信後処理</para>
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
            //③に飛ぶ
            return View("/Views/Chumon/ChumonMeisai.cshtml", chumonViewModel);
        }

        /// <summary>
        ///  商品注文明細画面Post後の処理
        /// </summary>
        /// <param name="id"></param>
        /// <param name="inChumonViewModel">注文明細ビューモデル</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChumonMeisai(ChumonViewModel inChumonViewModel) {

            ModelState.Clear();

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

            //⑤に飛ぶ
            return View("/Views/Chumon/ChumonMeisai.cshtml", ChumonViewModel);
        }
    }
}