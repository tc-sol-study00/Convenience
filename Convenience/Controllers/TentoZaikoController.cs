using Convenience.Data;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties.Config;
using Convenience.Models.ViewModels.TentoZaiko;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Convenience.Models.ViewModels.TentoZaiko.TentoZaikoViewModel.DataAreaClass;

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
        /// CSVファイル作成（ＤＩ用）
        /// </summary>
        private readonly IConvertObjectToCsv _convertObjectToCsv;

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
        public TentoZaikoController(ConvenienceContext context, ITentoZaikoService tentoZaikoService, IConvertObjectToCsv convertObjectToCsv) {
            this._context = context;
            this.tentoZaikoService = tentoZaikoService;
            this._convertObjectToCsv = convertObjectToCsv;
            this.tentoZaikoViewModel = new TentoZaikoViewModel();
        }

        /// <summary>
        /// 店頭在庫検索１枚目の初期表示処理
        /// </summary>
        /// <returns>店頭在庫ビューモデル（初期表示）</returns>
        [HttpGet]
        [Authorize]
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
        [Authorize]
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
        /// Download
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Downloadファイル</returns>
        /// <exception cref="Exception"></exception>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DownLoad() {
            TentoZaikoViewModel.KeywordAreaClass keywordArea
                = ISharedTools.ConvertFromSerial<TentoZaikoViewModel.KeywordAreaClass>(TempData.Peek(IndexName)?.ToString() ?? throw new Exception("tempdataなし"));
            tentoZaikoViewModel.KeywordArea = keywordArea;

            // 店頭在庫検索
            TentoZaikoViewModel createdTentoZaikoViewModel = await tentoZaikoService.TentoZaikoRetrival(tentoZaikoViewModel);

            //このコントローラの名前を認識
            string fileName = _convertObjectToCsv.GetFileName(this.GetType().Name);

            //モデルデータを取り出すし、ＣＳＶに変換
            IEnumerable<TentoZaIkoLine> modeldatas = createdTentoZaikoViewModel.DataArea.Lines;
            string filename = _convertObjectToCsv.ConvertToCSV<TentoZaIkoLine, CSVMapping.TentoZaIkoCSV>(modeldatas, fileName);

            //バイトに変換しファイルをhttp出力
            byte[] fileBytes = System.IO.File.ReadAllBytes(filename);

            return File(fileBytes, "text/csv", fileName);
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
            int totalLines = createdTentoZaikoViewModel.DataArea.Lines.Count();
            ViewBag.TotalPages = Math.Ceiling((double)totalLines / pageSize);
            ViewBag.CurrentPage = page;

            createdTentoZaikoViewModel.DataArea.Lines =
                createdTentoZaikoViewModel.DataArea.Lines.Skip((page - 1) * pageSize).Take(pageSize).ToList();

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