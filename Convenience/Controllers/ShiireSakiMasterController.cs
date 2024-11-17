using Convenience.Data;
using Convenience.Models.DataModels;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.Services;
using Convenience.Models.ViewModels.Chumon;
using Convenience.Models.ViewModels.ShiireSakiMaster;
using Microsoft.AspNetCore.Mvc;
using static Convenience.Models.Services.ShiireSakiMasterService;

namespace Convenience.Controllers {
    /// <summary>
    /// 注文コントローラ
    /// </summary>
    public class ShiireSakiMasterController : Controller, ISharedTools {
        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly ConvenienceContext _context;

        /// <summary>
        /// サービスクラス引継ぎ用キーワード
        /// </summary>
        private static readonly string IndexName = "ShiireSakiMasterViewModel";

        /// <summary>
        /// 注文サービスクラス（ＤＩ用）
        /// </summary>
        private readonly ShiireSakiMasterService shiireSakiMasterService;

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
        /// <param name="chumonService">注文サービスクラスＤＩ注入用</param>
        public ShiireSakiMasterController(ConvenienceContext context ) {
            this._context = context;
            this.shiireSakiMasterService = new ShiireSakiMasterService(context);
            //this.shohinMasterViewModel = new ShiireSakiMasterViewModel(_context);
        }

        /// <summary>
        /// 商品注文１枚目の初期表示処理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(string id) {
            var viewModel=shiireSakiMasterService.MakeViewModel();
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
        public async Task<IActionResult> Index(ShiireSakiMasterViewModel inShiireSakiMasterViewModel) {

            ModelState.Clear();

            var viewModel = shiireSakiMasterService.UpdateMasterData(inShiireSakiMasterViewModel);
            TempData[IndexName] = ISharedTools.ConvertToSerial(viewModel);
            return View(viewModel);
        }

        /// <summary>
        /// 挿入時
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> InsertRow(int index ) {
            ShiireSakiMasterViewModel viewModel
                   = ISharedTools.ConvertFromSerial<ShiireSakiMasterViewModel>(TempData[IndexName]?.ToString() ?? throw new Exception("tempdataなし"));

            viewModel.IsNormal = default;
            viewModel.Remark = string.Empty;

            viewModel.PostMasterDatas=shiireSakiMasterService.InsertRow(viewModel.PostMasterDatas, index);

            TempData[IndexName] = ISharedTools.ConvertToSerial(viewModel);
            ViewBag.FocusPosition = $"#postMasterDatas_{index + 1}__ShiireSakiId";
            return View("Index",viewModel);
        }

    }
}