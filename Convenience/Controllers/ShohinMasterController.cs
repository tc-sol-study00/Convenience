using Convenience.Data;
using Convenience.Models.Interfaces;
using Convenience.Models.Services;
using Convenience.Models.ViewModels.ShohinMaster;
using Microsoft.AspNetCore.Mvc;

namespace Convenience.Controllers {
    /// <summary>
    /// 商品マスターコントローラ
    /// </summary>
    public class ShohinMasterController : Controller, ISharedTools {
        /// <summary>
        /// データベースコンテキスト（依存性注入）
        /// </summary>
        private readonly ConvenienceContext _context;

        /// <summary>
        /// ビューモデルを保存するためのTempDataキー
        /// </summary>
        private static readonly string IndexName = "ShohinMasterViewModel";

        /// <summary>
        /// 商品マスターサービスクラス（依存性注入用）
        /// </summary>
        private readonly ShohinMasterService shohinMasterService;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">データベースコンテキストのインスタンス</param>
        public ShohinMasterController(ConvenienceContext context) {
            this._context = context; // コンストラクタでDBコンテキストを初期化
            this.shohinMasterService = new ShohinMasterService(context); // サービスクラスのインスタンスを作成
        }

        /// <summary>
        /// 商品マスターの初期表示（GETリクエスト）
        /// </summary>
        /// <param name="id">識別子（必要に応じて使用）</param>
        /// <returns>ビューとビューモデル</returns>
        [HttpGet]
        public async Task<IActionResult> Index(string id) {
            // サービスクラスから新しいビューモデルを作成
            var viewModel = shohinMasterService.MakeViewModel();
            // ビューモデルをシリアル化してTempDataに保存
            TempData[IndexName] = ISharedTools.ConvertToSerial(viewModel);
            // 初期フォーカス位置を設定
            ViewBag.FocusPosition = $"#postMasterDatas_0__ShohinId";
            return View(viewModel); // ビューを表示
        }

        /// <summary>
        /// 商品マスターのPostリクエスト処理
        /// </summary>
        /// <param name="inShohinMasterViewModel">受信した商品マスター用ビューモデル</param>
        /// <returns>更新されたビューとビューモデル</returns>
        [HttpPost]
        [ValidateAntiForgeryToken] // CSRF防止用
        public async Task<IActionResult> Index(ShohinMasterViewModel inShohinMasterViewModel) {
            // モデルの状態をクリア（バリデーションエラーをリセット）
            ModelState.Clear();

            // サービスクラスでビューモデルを更新
            var viewModel = shohinMasterService.UpdateMasterData(inShohinMasterViewModel);
            // 更新されたビューモデルをTempDataに保存
            TempData[IndexName] = ISharedTools.ConvertToSerial(viewModel);
            return View(viewModel); // ビューを更新
        }

        /// <summary>
        /// 新しい行を挿入する処理
        /// </summary>
        /// <param name="index">挿入する行のインデックス</param>
        /// <returns>更新されたビューとビューモデル</returns>
        [HttpGet]
        public async Task<IActionResult> InsertRow(int index) {
            // TempDataからビューモデルを復元
            ShohinMasterViewModel viewModel = ISharedTools.ConvertFromSerial<ShohinMasterViewModel>(
                TempData[IndexName]?.ToString() ?? throw new Exception("TempDataが存在しません")
            );

            // サービスクラスで新しい行を挿入
            viewModel.PostMasterDatas = shohinMasterService.InsertRow(viewModel.PostMasterDatas, index);

            // 新しい行に関連するプロパティを初期化
            viewModel.IsNormal = default; // フラグをデフォルト値にリセット
            viewModel.Remark = string.Empty; // 備考を初期化

            // 更新されたビューモデルをTempDataに保存
            TempData[IndexName] = ISharedTools.ConvertToSerial(viewModel);
            // 挿入した行にフォーカスを移動
            ViewBag.FocusPosition = $"#postMasterDatas_{index + 1}__ShohinId";
            return View("Index", viewModel); // Indexビューを更新済みのビューモデルで再表示
        }
    }
}
