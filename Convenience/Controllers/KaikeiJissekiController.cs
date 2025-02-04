﻿using Convenience.Data;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties.Config;
using Convenience.Models.ViewModels.KaikeiJisseki;
using Microsoft.AspNetCore.Mvc;
using static Convenience.Models.ViewModels.KaikeiJisseki.KaikeiJissekiViewModel.DataAreaClass;

namespace Convenience.Controllers {
    /// <summary>
    /// 会計実績検索コントローラ
    /// </summary>
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class KaikeiJissekiController : Controller, ISharedTools {
        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly ConvenienceContext _context;

        /// <summary>
        /// サービスクラス引継ぎ用キーワード
        /// </summary>
        private static readonly string IndexName = "KaikeiJissekiViewModel";

        /// <summary>
        /// 会計実績検索サービスクラス（ＤＩ用）
        /// </summary>
        private readonly IKaikeiJissekiService kaikeiJissekiService;

        /// <summary>
        /// CSVファイル作成（ＤＩ用）
        /// </summary>
        private readonly IConvertObjectToCsv _convertObjectToCsv;

        /// <summary>
        /// ビュー・モデル
        /// </summary>
        private readonly KaikeiJissekiViewModel kaikeiJissekiViewModel;
        /// <summary>
        /// １ページの行数
        /// </summary>
        private const int PageSize = 100;

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
        /// <param name="kaikeiJissekiService">会計実績検索サービスクラスＤＩ注入用</param>
        public KaikeiJissekiController(ConvenienceContext context, IKaikeiJissekiService kaikeiJissekiService, IConvertObjectToCsv convertObjectToCsv) {
            this._context = context;
            this.kaikeiJissekiService = kaikeiJissekiService;
            this.kaikeiJissekiViewModel = new KaikeiJissekiViewModel();
            this._convertObjectToCsv = convertObjectToCsv;
        }

        /// <summary>
        /// 会計実績検索１枚目の初期表示処理
        /// </summary>
        /// <returns>会計実績ビューモデル（初期表示）</returns>
        /// 
        [HttpGet]
        public Task<IActionResult> Index() {
            ViewBag.HandlingFlg = "FirstDisplay";
            //最初のカーソル位置
            ViewBag.FocusPosition = "#KeywordArea_KeyArea_SelecteWhereItemArray_0__LeftSide";

            //最初のカーソル位置
            return Task.FromResult<IActionResult>(View("Index", kaikeiJissekiViewModel));
        }

        /// <summary>
        /// 会計実績検索キー入力後
        /// </summary>
        /// <param name="postedTentoZaikoViewModel"></param>
        /// <returns>会計実績ビューモデル</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Result(KaikeiJissekiViewModel postedKaikeiJissekiViewModel, int page = 1
            ) {
            return await ProcessResult(postedKaikeiJissekiViewModel, page, PageSize);
        }

        /// <summary>
        /// 検索結果
        /// </summary>
        /// <param name="page"></param>
        /// <returns>会計実績検索ビュー</returns>
        [HttpGet]
        public async Task<IActionResult> Result(int page) {
            if (TempData.Peek(IndexName) is string tempDataStr) {
                KaikeiJissekiViewModel.KeywordAreaClass keyArea = ISharedTools.ConvertFromSerial<KaikeiJissekiViewModel.KeywordAreaClass>(tempDataStr);
                kaikeiJissekiViewModel.KeywordArea = keyArea;

                return await ProcessResult(kaikeiJissekiViewModel, page, PageSize);
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
        public async Task<IActionResult> DownLoad() {
            KaikeiJissekiViewModel.KeywordAreaClass keywordArea
                = ISharedTools.ConvertFromSerial<KaikeiJissekiViewModel.KeywordAreaClass>(TempData.Peek(IndexName)?.ToString() ?? throw new Exception("tempdataなし"));
            kaikeiJissekiViewModel.KeywordArea = keywordArea;

            // 会計実績検索
            KaikeiJissekiViewModel createdKaikeiJissekiViewModel = await kaikeiJissekiService.KaikeiJissekiRetrival(kaikeiJissekiViewModel);

            //このコントローラの名前を認識
            string fileName = _convertObjectToCsv.GetFileName(this.GetType().Name);

            //モデルデータを取り出すし、ＣＳＶに変換
            IEnumerable<KaikeiJissekiLineClass> modeldatas = createdKaikeiJissekiViewModel.DataArea.Lines;
            string filename = _convertObjectToCsv.ConvertToCSV<KaikeiJissekiLineClass, CSVMapping.KaikeiJissekiCSV>(modeldatas, fileName);

            //バイトに変換しファイルをhttp出力
            byte[] fileBytes = System.IO.File.ReadAllBytes(filename);

            return File(fileBytes, "text/csv", fileName);
        }

        /// <summary>
        /// 検索データ作成
        /// </summary>
        /// <param name="kaikeiJissekiViewModel">会計実績検索ビューモデル</param>
        /// <param name="page">ページ番号</param>
        /// <param name="pageSize">行数／ページ</param>
        /// <returns>会計実績検索ビュー</returns>
        private async Task<IActionResult> ProcessResult(KaikeiJissekiViewModel kaikeiJissekiViewModel, int page, int pageSize) {
            // 会計実績検索
            KaikeiJissekiViewModel createdKaikeiJissekiViewModel = await kaikeiJissekiService.KaikeiJissekiRetrival(kaikeiJissekiViewModel);

            // ページング処理
            int totalLines = createdKaikeiJissekiViewModel.DataArea.Lines.Count();
            ViewBag.TotalPages = Math.Ceiling((double)totalLines / pageSize);
            ViewBag.CurrentPage = page;

            createdKaikeiJissekiViewModel.DataArea.Lines =
                createdKaikeiJissekiViewModel.DataArea.Lines.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // キーワードエリアの保存
            TempData[IndexName] = ISharedTools.ConvertToSerial(createdKaikeiJissekiViewModel.KeywordArea);

            //最初のカーソル位置
            ViewBag.FocusPosition = "#KeywordArea_KeyArea_SelecteWhereItemArray_0__LeftSide";
            // 結果をビューに返す
            ViewBag.HandlingFlg = "SecondDisplay";
            return View("Index", createdKaikeiJissekiViewModel);
        }

    }
}