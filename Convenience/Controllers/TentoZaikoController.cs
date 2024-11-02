using Convenience.Data;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.Services;
using Convenience.Models.ViewModels.Chumon;
using Convenience.Models.ViewModels.Shiire;
using Convenience.Models.ViewModels.TentoHaraidashi;
using Convenience.Models.ViewModels.TentoZaiko;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Convenience.Controllers {
    /// <summary>
    /// 店頭払出コントローラ
    /// </summary>
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class TentoZaikoController : Controller, ISharedTools {
        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly ConvenienceContext _context;

        /// <summary>
        /// サービスクラス引継ぎ用キーワード
        /// </summary>
        private static readonly string IndexName = "TentoZaikoViewModel";

        /// <summary>
        /// 注文サービスクラス（ＤＩ用）
        /// </summary>
        private readonly ITentoZaikoService tentoZaikoService;

        /// <summary>
        /// ビュー・モデル
        /// </summary>
        private TentoZaikoViewModel tentoHaraidashiViewModel;
        /// <summary>
        /// １ページの行数
        /// </summary>
        private const int PageSize = 100;

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
        /// <param name="chumonService">注文サービスクラスＤＩ注入用</param>
        public TentoZaikoController(ConvenienceContext context, ITentoZaikoService tentoZaikoService) {
            this._context = context;
            this.tentoZaikoService = tentoZaikoService;
            this.tentoHaraidashiViewModel = new TentoZaikoViewModel();
        }

        /// <summary>
        /// 店頭在庫検索１枚目の初期表示処理
        /// </summary>
        /// <returns>店頭在庫ビューモデル（初期表示）</returns>
        /// 
        [HttpGet]
        public async Task<IActionResult> Index(string id) {
            if ((id ?? string.Empty).Equals("Result")) {
                ViewBag.HandlingFlg = "FirstDisplay";
            }
            else {
                ViewBag.HandlingFlg = "FirstDisplay";
            }
            return View("Index", tentoHaraidashiViewModel);
        }

        /// <summary>
        /// 店頭在庫検索キー入力後
        /// </summary>
        /// <param name="postedTentoZaikoViewModel"></param>
        /// <returns>店頭在庫ビューモデル</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Result(TentoZaikoViewModel postedTentoZaikoViewModel, int page = 1
            ) {
            return await ProcessResult(postedTentoZaikoViewModel, page, PageSize);
        }

        [HttpGet]
        public async Task<IActionResult> Result(int page) {
            if (TempData.Peek(IndexName) is string tempDataStr) {
                var keyArea = ISharedTools.ConvertFromSerial<TentoZaikoViewModel.KeywordAreaClass>(tempDataStr);
                var tentoZaikoViewModel = new TentoZaikoViewModel { KeywordArea = keyArea };

                return await ProcessResult(tentoZaikoViewModel, page, PageSize);
            }
            else {
                // TempDataがない場合、初期ページにリダイレクト
                return RedirectToAction("Index");
            }
        }

        private async Task<IActionResult> ProcessResult(TentoZaikoViewModel tentoZaikoViewModel, int page, int pageSize) {
            // 店頭在庫検索
            var createdTentoZaikoViewModel = await tentoZaikoService.TentoZaikoRetrival(tentoZaikoViewModel);

            // ページング処理
            var totalLines = createdTentoZaikoViewModel.DataArea.TentoZaIkoLines.Count();
            ViewBag.TotalPages = Math.Ceiling((double)totalLines / pageSize);
            ViewBag.CurrentPage = page;

            createdTentoZaikoViewModel.DataArea.TentoZaIkoLines =
                createdTentoZaikoViewModel.DataArea.TentoZaIkoLines.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // キーワードエリアの保存
            TempData[IndexName] = ISharedTools.ConvertToSerial(createdTentoZaikoViewModel.KeywordArea);

            // 結果をビューに返す
            return View("Index", createdTentoZaikoViewModel);
        }

    }
}