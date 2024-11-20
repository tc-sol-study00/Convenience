using Convenience.Data;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties.Config;
using Convenience.Models.ViewModels.ShiireJisseki;
using Microsoft.AspNetCore.Mvc;
using static Convenience.Models.ViewModels.ShiireJisseki.ShiireJissekiViewModel.DataAreaClass;

namespace Convenience.Controllers {
    /// <summary>
    /// 仕入実績検索コントローラ
    /// </summary>
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class ShiireJissekiController : Controller, ISharedTools {
        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly ConvenienceContext _context;

        /// <summary>
        /// サービスクラス引継ぎ用キーワード
        /// </summary>
        private static readonly string IndexName = "ShiireJissekiViewModel";

        /// <summary>
        /// 仕入実績サービスクラス（ＤＩ用）
        /// </summary>
        private readonly IShiireJissekiService shiireJissekiService;

        /// <summary>
        /// CSVファイル作成（ＤＩ用）
        /// </summary>
        private readonly IConvertObjectToCsv _convertObjectToCsv;

        /// <summary>
        /// ビュー・モデル
        /// </summary>
        private readonly ShiireJissekiViewModel shiireJissekiViewModel;
        /// <summary>
        /// １ページの行数
        /// </summary>
        private const int PageSize = 100;

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
        /// <param name="shiireJissekiService">仕入実績サービスクラスＤＩ注入用</param>
        public ShiireJissekiController(ConvenienceContext context, IShiireJissekiService shiireJissekiService, IConvertObjectToCsv convertObjectToCsv) {
            this._context = context;
            this.shiireJissekiService = shiireJissekiService;
            this.shiireJissekiViewModel = new ShiireJissekiViewModel();
            this._convertObjectToCsv = convertObjectToCsv;
        }

        /// <summary>
        /// 仕入実績検索１枚目の初期表示処理
        /// </summary>
        /// <returns>仕入実績ビューモデル（初期表示）</returns>
        /// 
        [HttpGet]
        public Task<IActionResult> Index() {
            ViewBag.HandlingFlg = "FirstDisplay";
            //最初のカーソル位置
            ViewBag.FocusPosition = "#KeywordArea_KeyArea_SelecteWhereItemArray_0__LeftSide";

            //最初のカーソル位置
            return Task.FromResult<IActionResult>(View("Index", shiireJissekiViewModel));
        }

        /// <summary>
        /// 仕入実績検索キー入力後
        /// </summary>
        /// <param name="postedShiireJissekiViewModel">仕入実績ビューモデル</param>
        /// <returns>仕入実績ビューモデル</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Result(ShiireJissekiViewModel postedShiireJissekiViewModel, int page = 1
            ) {
            return await ProcessResult(postedShiireJissekiViewModel, page, PageSize);
        }

        /// <summary>
        /// 検索結果
        /// </summary>
        /// <param name="page">ページ番号</param>
        /// <returns>仕入実績ビューモデル</returns>
        [HttpGet]
        public async Task<IActionResult> Result(int page) {
            if (TempData.Peek(IndexName) is string tempDataStr) {
                ShiireJissekiViewModel.KeywordAreaClass keyArea = ISharedTools.ConvertFromSerial<ShiireJissekiViewModel.KeywordAreaClass>(tempDataStr);
                shiireJissekiViewModel.KeywordArea = keyArea;

                return await ProcessResult(shiireJissekiViewModel, page, PageSize);
            } else {
                // TempDataがない場合、初期ページにリダイレクト
                return RedirectToAction("Index");
            }
        }
        /// <summary>
        ///  検索データ作成
        /// </summary>
        /// <param name="shiireJissekiViewModel">仕入実績ビューモデル</param>
        /// <param name="page">ページ番号</param>
        /// <param name="pageSize">行数／ページ</param>
        /// <returns></returns>
        private async Task<IActionResult> ProcessResult(ShiireJissekiViewModel shiireJissekiViewModel, int page, int pageSize) {
            // 仕入実績検索
            ShiireJissekiViewModel createdShiireJissekiViewModel = await shiireJissekiService.ShiireJissekiRetrival(shiireJissekiViewModel);

            // ページング処理
            int totalLines = createdShiireJissekiViewModel.DataArea.Lines.Count();
            ViewBag.TotalPages = Math.Ceiling((double)totalLines / pageSize);
            ViewBag.CurrentPage = page;

            createdShiireJissekiViewModel.DataArea.Lines =
                createdShiireJissekiViewModel.DataArea.Lines.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // キーワードエリアの保存
            TempData[IndexName] = ISharedTools.ConvertToSerial(createdShiireJissekiViewModel.KeywordArea);

            //最初のカーソル位置
            ViewBag.FocusPosition = "#KeywordArea_KeyArea_SelecteWhereItemArray_0__LeftSide";
            // 結果をビューに返す
            ViewBag.HandlingFlg = "SecondDisplay";
            return View("Index", createdShiireJissekiViewModel);
        }

        /// <summary>
        /// Download
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Downloadファイル</returns>
        /// <exception cref="Exception"></exception>
        [HttpGet]
        public async Task<IActionResult> DownLoad() {
            shiireJissekiViewModel.KeywordArea
                = ISharedTools.ConvertFromSerial<ShiireJissekiViewModel.KeywordAreaClass>(TempData.Peek(IndexName)?.ToString() ?? throw new Exception("tempdataなし"));

            // 仕入実績検索
            ShiireJissekiViewModel createdShiireJissekiViewModel = await shiireJissekiService.ShiireJissekiRetrival(shiireJissekiViewModel);

            //このコントローラの名前を認識
            string fileName = _convertObjectToCsv.GetFileName(this.GetType().Name);

            //モデルデータを取り出すし、ＣＳＶに変換
            IEnumerable<ShiireJissekiLineClass> modeldatas = createdShiireJissekiViewModel.DataArea.Lines;
            string filename=_convertObjectToCsv.ConvertToCSV<ShiireJissekiLineClass, CSVMapping.ShiireJissekiCSV>(modeldatas, fileName);

            //バイトに変換しファイルをhttp出力
            byte[] fileBytes = System.IO.File.ReadAllBytes(filename);

            return File(fileBytes, "text/csv", fileName);
        }
    }
}