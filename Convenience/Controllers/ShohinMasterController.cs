using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.Services;
using Convenience.Models.ViewModels.Chumon;
using Convenience.Models.ViewModels.ShohinMaster;
using Microsoft.AspNetCore.Mvc;
using static Convenience.Models.Services.ShohinMasterService;

namespace Convenience.Controllers {
    /// <summary>
    /// 注文コントローラ
    /// </summary>
    public class ShohinMasterController : Controller, ISharedTools {
        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly ConvenienceContext _context;

        /// <summary>
        /// サービスクラス引継ぎ用キーワード
        /// </summary>
        private static readonly string IndexName = "ShohinMasterViewModel";

        /// <summary>
        /// 注文サービスクラス（ＤＩ用）
        /// </summary>
        private readonly ShohinMasterService shohinMasterService;

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
        /// <param name="chumonService">注文サービスクラスＤＩ注入用</param>
        public ShohinMasterController(ConvenienceContext context) {
            this._context = context;
            this.shohinMasterService = new ShohinMasterService(context);
        }

        /// <summary>
        /// 商品注文１枚目の初期表示処理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(string id) {
            var viewModel=shohinMasterService.MakeViewModel();
            TempData[IndexName] = ISharedTools.ConvertToSerial(viewModel);
            ViewBag.FocusPosition = $"#postMasterDatas_0__ShohinId";
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
        public async Task<IActionResult> Index(ShohinMasterViewModel inShohinMasterViewModel) {

            ModelState.Clear();

            var viewModel = shohinMasterService.UpdateMasterData(inShohinMasterViewModel);
            TempData[IndexName] = ISharedTools.ConvertToSerial(viewModel);
            return View(viewModel);
        }

        /// <summary>
        /// 挿入時
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> InsertRow(int index ) {
            ShohinMasterViewModel viewModel
                   = ISharedTools.ConvertFromSerial<ShohinMasterViewModel>(TempData[IndexName]?.ToString() ?? throw new Exception("tempdataなし"));
            //var viewModel = shohinMasterService.MakeViewModel();

            viewModel.PostMasterDatas=shohinMasterService.InsertRow(viewModel.PostMasterDatas, index);

            viewModel.IsNormal = default;
            viewModel.Remark = string.Empty;

            TempData[IndexName] = ISharedTools.ConvertToSerial(viewModel);
            ViewBag.FocusPosition = $"#postMasterDatas_{index + 1}__ShohinId";
            return View("Index",viewModel);
        }

    }
}