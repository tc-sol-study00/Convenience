﻿using Convenience.Data;
using Convenience.Models.Interfaces;
using Convenience.Models.Services;
using Convenience.Models.ViewModels.ShiireSakiMaster;
using Microsoft.AspNetCore.Mvc;

namespace Convenience.Controllers {
    /// <summary>
    /// 仕入先マスターコントローラ
    /// </summary>
    public class ShiireSakiMasterController : Controller, ISharedTools {
        /// <summary>
        /// データベースコンテキスト（DIによる注入）
        /// </summary>
        private readonly ConvenienceContext _context;

        /// <summary>
        /// サービスクラスのデータを保持するためのTempDataキー
        /// </summary>
        private static readonly string IndexName = "ShiireSakiMasterViewModel";

        /// <summary>
        /// 仕入先マスターサービスクラス（依存性注入用）
        /// </summary>
        private readonly ShiireSakiMasterService shiireSakiMasterService;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">データベースコンテキストのインスタンス</param>
        public ShiireSakiMasterController(ConvenienceContext context) {
            this._context = context;                                                // データベースコンテキストをフィールドに設定
            this.shiireSakiMasterService = new ShiireSakiMasterService(context);    // サービスクラスのインスタンスを作成
        }

        /// <summary>
        /// 初期表示処理（GETリクエスト）
        /// </summary>
        /// <param name="id">識別子（オプションで使用可能）</param>
        /// <returns>ビューとビューモデル</returns>
        [HttpGet]
        public async Task<IActionResult> Index(string id) {
            // サービスクラスで新しいビューモデルを生成
            var viewModel = shiireSakiMasterService.MakeViewModel();
            // ビューモデルをシリアル化してTempDataに保存
            TempData[IndexName] = ISharedTools.ConvertToSerial(viewModel);
            // 初期フォーカス位置を設定（HTMLの特定フィールドを指定）
            ViewBag.FocusPosition = $"#postMasterDatas_0__ShiireSakiId";
            return View(viewModel); // ビューにビューモデルを渡して表示
        }

        /// <summary>
        /// POSTリクエスト後の処理（フォームデータ受信後）
        /// </summary>
        /// <param name="inShiireSakiMasterViewModel">受信した仕入先マスター用のビューモデル</param>
        /// <returns>更新されたビューとビューモデル</returns>
        [HttpPost]
        [ValidateAntiForgeryToken] // CSRF（クロスサイトリクエストフォージェリ）防止用属性
        public async Task<IActionResult> Index(ShiireSakiMasterViewModel inShiireSakiMasterViewModel) {
            // モデルの状態をクリア（バリデーションエラーの再初期化）
            ModelState.Clear();

            // サービスクラスでデータを更新
            var viewModel = shiireSakiMasterService.UpdateMasterData(inShiireSakiMasterViewModel);
            // 更新済みビューモデルをシリアル化してTempDataに保存
            TempData[IndexName] = ISharedTools.ConvertToSerial(viewModel);
            return View(viewModel); // 更新されたビューモデルでビューを再表示
        }

        /// <summary>
        /// 新しい行を挿入する処理（GETリクエスト）
        /// </summary>
        /// <param name="index">挿入する位置のインデックス</param>
        /// <returns>更新されたビューとビューモデル</returns>
        [HttpGet]
        public async Task<IActionResult> InsertRow(int index) {
            // TempDataからビューモデルを復元
            ShiireSakiMasterViewModel viewModel = ISharedTools.ConvertFromSerial<ShiireSakiMasterViewModel>(
                TempData[IndexName]?.ToString() ?? throw new Exception("TempDataが存在しません")
            );

            // ビューモデルの初期化
            viewModel.IsNormal = default; // 通常状態フラグをデフォルト値にリセット
            viewModel.Remark = string.Empty; // 備考欄を空文字列にリセット

            // 指定したインデックス位置に新しい行を挿入
            viewModel.PostMasterDatas = shiireSakiMasterService.InsertRow(viewModel.PostMasterDatas, index);

            // 更新済みビューモデルをシリアル化してTempDataに保存
            TempData[IndexName] = ISharedTools.ConvertToSerial(viewModel);
            // 新しい行にフォーカスを移動
            ViewBag.FocusPosition = $"#postMasterDatas_{index + 1}__ShiireSakiId";
            return View("Index", viewModel); // Indexビューを更新済みのビューモデルで再表示
        }
    }
}
