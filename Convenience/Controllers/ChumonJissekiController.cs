using Convenience.Data;
using Convenience.Models.Interfaces;
using Convenience.Models.ViewModels.ChumonJisseki;
using Convenience.Models.ViewModels.KaikeiJisseki;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Convenience.Controllers {
    /// <summary>
    /// 注文実績検索コントローラ
    /// </summary>
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class ChumonJissekiController : Controller, ISharedTools {
        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly ConvenienceContext _context;

        /// <summary>
        /// サービスクラス引継ぎ用キーワード
        /// </summary>
        private static readonly string IndexName = "ChumonJissekiViewModel";

        /// <summary>
        /// 注文実績検索サービスサービスクラス（ＤＩ用）
        /// </summary>
        private readonly IChumonJissekiService chumonJissekiService;

        /// <summary>
        /// ビュー・モデル
        /// </summary>
        private readonly ChumonJissekiViewModel chumonJissekiViewModel;
        /// <summary>
        /// １ページの行数
        /// </summary>
        private const int PageSize = 100;

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
        /// <param name="chumonJissekiService">注文実績検索サービスクラスＤＩ注入用</param>
        public ChumonJissekiController(ConvenienceContext context, IChumonJissekiService chumonJissekiService) {
            this._context = context;
            this.chumonJissekiService = chumonJissekiService;
            this.chumonJissekiViewModel = new ChumonJissekiViewModel();
        }

        /// <summary>
        /// 注文実績検索１枚目の初期表示処理
        /// </summary>
        /// <returns>注文実績検索ビューモデル（初期表示）</returns>
        /// 
        [HttpGet]
        public Task<IActionResult> Index() {
            ViewBag.HandlingFlg = "FirstDisplay";
            //最初のカーソル位置
            ViewBag.FocusPosition = "#KeywordArea_KeyArea_SelecteWhereItemArray_0__LeftSide";

            //最初のカーソル位置
            return Task.FromResult<IActionResult>(View("Index", chumonJissekiViewModel));
        }

        /// <summary>
        /// 注文実績検索キー入力後
        /// </summary>
        /// <param name="postedChumonJissekiViewModel">注文実績検索ビューモデル(post)</param>
        ///<param name="page">ページ番号</param>
        /// <returns>注文実績ビューモデル</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Result(ChumonJissekiViewModel postedChumonJissekiViewModel, int page = 1
            ) {
            return await ProcessResult(postedChumonJissekiViewModel, page, PageSize);
        }

        /// <summary>
        /// 検索結果
        /// </summary>
        /// <param name="page">ページ番号</param>
        /// <returns>注文実績検索ビュー</returns>
        [HttpGet]
        public async Task<IActionResult> Result(int page) {
            if (TempData.Peek(IndexName) is string tempDataStr) {
                ChumonJissekiViewModel.KeywordAreaClass keyArea = ISharedTools.ConvertFromSerial<ChumonJissekiViewModel.KeywordAreaClass>(tempDataStr);
                chumonJissekiViewModel.KeywordArea = keyArea;

                return await ProcessResult(chumonJissekiViewModel, page, PageSize);
            }
            else {
                // TempDataがない場合、初期ページにリダイレクト
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// 検索データ作成
        /// </summary>
        /// <param name="kaikeiJissekiViewModel">会計実績ビューモデル</param>
        /// <param name="page">ページ番号</param>
        /// <param name="pageSize">行数/ページ</param>
        /// <returns>注文実績検索ビュー</returns>

        private async Task<IActionResult> ProcessResult(ChumonJissekiViewModel chumonJissekiViewModel, int page, int pageSize) {
            // 注文実績検索
            ChumonJissekiViewModel createdChumonJissekiViewModel = await chumonJissekiService.ChumonJissekiRetrival(chumonJissekiViewModel);

            // ページング処理
            int totalLines = createdChumonJissekiViewModel.DataArea.ChumonJissekiLines.Count();
            ViewBag.TotalPages = Math.Ceiling((double)totalLines / pageSize);
            ViewBag.CurrentPage = page;

            createdChumonJissekiViewModel.DataArea.ChumonJissekiLines =
                createdChumonJissekiViewModel.DataArea.ChumonJissekiLines.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // キーワードエリアの保存
            TempData[IndexName] = ISharedTools.ConvertToSerial(createdChumonJissekiViewModel.KeywordArea);

            //最初のカーソル位置
            ViewBag.FocusPosition = "#KeywordArea_KeyArea_SelecteWhereItemArray_0__LeftSide";
            // 結果をビューに返す
            ViewBag.HandlingFlg = "SecondDisplay";
            return View("Index", createdChumonJissekiViewModel);
        }

    }
}