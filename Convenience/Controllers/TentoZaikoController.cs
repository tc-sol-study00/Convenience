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
    /// 店頭在庫検索コントローラ
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
        /// 店頭在庫検索サービスクラス（ＤＩ用）
        /// </summary>
        private readonly ITentoZaikoService tentoZaikoService;

        /// <summary>
        /// ビュー・モデル
        /// </summary>
        private readonly TentoZaikoViewModel tentoZaikoViewModel;
        /// <summary>
        /// １ページの行数
        /// </summary>
        private const int PageSize = 100;

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
        /// <param name="tentoZaikoService">店頭在庫検索サービスクラスＤＩ注入用</param>
        public TentoZaikoController(ConvenienceContext context, ITentoZaikoService tentoZaikoService) {
            this._context = context;
            this.tentoZaikoService = tentoZaikoService;
            this.tentoZaikoViewModel = new TentoZaikoViewModel();
        }

        /// <summary>
        /// 店頭在庫検索１枚目の初期表示処理
        /// </summary>
        /// <returns>店頭在庫ビューモデル（初期表示）</returns>
        [HttpGet]
        public Task<IActionResult> Index() {
            ViewBag.HandlingFlg = "FirstDisplay";
            //最初のカーソル位置
            ViewBag.FocusPosition = "#KeywordArea_KeyArea_SelecteWhereItemArray_0__LeftSide";

            //最初のカーソル位置
            return Task.FromResult<IActionResult>(View("Index", tentoZaikoViewModel));
        }

        /// <summary>
        /// 店頭在庫検索キー入力後
        /// </summary>
        /// <param name="postedTentoZaikoViewModel">店頭在庫ビューモデル(post)</param>
        /// <returns>店頭在庫ビューモデル</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Result(TentoZaikoViewModel postedTentoZaikoViewModel, int page = 1
            ) {
            return await ProcessResult(postedTentoZaikoViewModel, page, PageSize);
        }

        /// <summary>
        /// 検索結果
        /// </summary>
        /// <param name="page">ページ番号</param>
        /// <returns>店頭在庫検索ビュー</returns>
        [HttpGet]
        public async Task<IActionResult> Result(int page) {
            if (TempData.Peek(IndexName) is string tempDataStr) {
                TentoZaikoViewModel.KeywordAreaClass keyArea = ISharedTools.ConvertFromSerial<TentoZaikoViewModel.KeywordAreaClass>(tempDataStr);
                tentoZaikoViewModel.KeywordArea = keyArea;

                return await ProcessResult(tentoZaikoViewModel, page, PageSize);
            }
            else {
                // TempDataがない場合、初期ページにリダイレクト
                return RedirectToAction("Index");
            }
        }
        /// <summary>
        ///  検索データ作成
        /// </summary>
        /// <param name="tentoZaikoViewModel">店頭在庫ビューモデル</param>
        /// <param name="page">ページ番号</param>
        /// <param name="pageSize">行数/ページ</param>
        /// <returns></returns>
        private async Task<IActionResult> ProcessResult(TentoZaikoViewModel tentoZaikoViewModel, int page, int pageSize) {
            // 店頭在庫検索
            TentoZaikoViewModel createdTentoZaikoViewModel = await tentoZaikoService.TentoZaikoRetrival(tentoZaikoViewModel);

            // ページング処理
            int totalLines = createdTentoZaikoViewModel.DataArea.TentoZaIkoLines.Count();
            ViewBag.TotalPages = Math.Ceiling((double)totalLines / pageSize);
            ViewBag.CurrentPage = page;

            createdTentoZaikoViewModel.DataArea.TentoZaIkoLines =
                createdTentoZaikoViewModel.DataArea.TentoZaIkoLines.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // キーワードエリアの保存
            TempData[IndexName] = ISharedTools.ConvertToSerial(createdTentoZaikoViewModel.KeywordArea);

            //最初のカーソル位置
            ViewBag.FocusPosition = "#KeywordArea_KeyArea_SelecteWhereItemArray_0__LeftSide";
            // 結果をビューに返す
            ViewBag.HandlingFlg = "SecondDisplay";
            return View("Index", createdTentoZaikoViewModel);
        }

    }
}