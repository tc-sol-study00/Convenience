using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.Services;
using Convenience.Models.ViewModels.Chumon;
using Convenience.Models.ViewModels.NaigaiClassMaster;
using Microsoft.AspNetCore.Mvc;
using static Convenience.Models.Services.NaigaiClassMasterService;

namespace Convenience.Controllers {
    /// <summary>
    /// 注文コントローラ
    /// </summary>
    public class NaigaiClassMasterController : Controller, ISharedTools {
        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly ConvenienceContext _context;

        /// <summary>
        /// サービスクラス引継ぎ用キーワード
        /// </summary>
        private static readonly string IndexName = "NaigaiClassMasterViewModel";

        /// <summary>
        /// 注文サービスクラス（ＤＩ用）
        /// </summary>
        private readonly NaigaiClassMasterService naigaiClassMasterService;

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
        /// <param name="chumonService">注文サービスクラスＤＩ注入用</param>
        public NaigaiClassMasterController(ConvenienceContext context ) {
            this._context = context;
            this.naigaiClassMasterService = new NaigaiClassMasterService(context);
            //this.shohinMasterViewModel = new NaigaiClassMasterViewModel(_context);
        }

        /// <summary>
        /// 商品注文１枚目の初期表示処理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(string id) {
            var viewModel=naigaiClassMasterService.MakeViewModel();
            TempData[IndexName] = ISharedTools.ConvertToSerial(viewModel);
            ViewBag.FocusPosition = $"#postMasterDatas_0__ShiireSakiId";
            return View(viewModel);
        }

        /// <summary>
        /// 商品注文１枚目のPost受信後処理
        /// </summary>
        /// <param name="inChumonKeysViewModel">注文キービューモデル</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(NaigaiClassMasterViewModel inNaigaiClassMasterViewModel) {

            ModelState.Clear();

            var viewModel = naigaiClassMasterService.UpdateMasterData(inNaigaiClassMasterViewModel);
            TempData[IndexName] = ISharedTools.ConvertToSerial(viewModel);
            return View(viewModel);
        }

        /// <summary>
        /// 挿入時
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> InsertRow(int index ) {
            NaigaiClassMasterViewModel viewModel
                   = ISharedTools.ConvertFromSerial<NaigaiClassMasterViewModel>(TempData[IndexName]?.ToString() ?? throw new Exception("tempdataなし"));

            viewModel.IsNormal = default;
            viewModel.Remark = string.Empty;

            viewModel.PostMasterDatas=naigaiClassMasterService.InsertRow(viewModel.PostMasterDatas, index);

            TempData[IndexName] = ISharedTools.ConvertToSerial(viewModel);
            ViewBag.FocusPosition = $"#postMasterDatas_{index + 1}__ShiireSakiId";
            return View("Index",viewModel);
        }

    }
}