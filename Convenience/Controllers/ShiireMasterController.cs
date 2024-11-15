using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.Services;
using Convenience.Models.ViewModels.Chumon;
using Convenience.Models.ViewModels.ShiireMaster;
using Microsoft.AspNetCore.Mvc;
using static Convenience.Models.Services.ShiireMasterService;

namespace Convenience.Controllers {
    /// <summary>
    /// 注文コントローラ
    /// </summary>
    public class ShiireMasterController : Controller, ISharedTools {
        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly ConvenienceContext _context;

        /// <summary>
        /// サービスクラス引継ぎ用キーワード
        /// </summary>
        private static readonly string IndexName = "ShiireMasterViewModel";

        /// <summary>
        /// 注文サービスクラス（ＤＩ用）
        /// </summary>
        private readonly ShiireMasterService shiireMasterService;

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
        /// <param name="chumonService">注文サービスクラスＤＩ注入用</param>
        public ShiireMasterController(ConvenienceContext context ) {
            this._context = context;
            this.shiireMasterService = new ShiireMasterService(context);
            //this.shohinMasterViewModel = new ShiireMasterViewModel(_context);
        }

        /// <summary>
        /// 商品注文１枚目の初期表示処理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(string id) {
            var viewModel=shiireMasterService.MakeViewModel();
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
        public async Task<IActionResult> Index(ShiireMasterViewModel inShiireMasterViewModel) {

            ModelState.Clear();

            var viewModel = shiireMasterService.UpdateMasterData(inShiireMasterViewModel);
            TempData[IndexName] = ISharedTools.ConvertToSerial(viewModel);
            return View(viewModel);
        }

        /// <summary>
        /// 挿入時
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> InsertRow(int index ) {
            ShiireMasterViewModel viewModel
                   = ISharedTools.ConvertFromSerial<ShiireMasterViewModel>(TempData[IndexName]?.ToString() ?? throw new Exception("tempdataなし"));

            viewModel.IsNormal = default;
            viewModel.Remark = string.Empty;

            viewModel.PostMasterDatas=shiireMasterService.InsertRow(viewModel.PostMasterDatas, index);

            TempData[IndexName] = ISharedTools.ConvertToSerial(viewModel);
            ViewBag.FocusPosition = $"#postMasterDatas_{index + 1}__ShiireSakiId";
            return View("Index",viewModel);
        }

    }
}