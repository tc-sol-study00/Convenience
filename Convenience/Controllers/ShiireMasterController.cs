using Convenience.Data;
using Convenience.Models.Interfaces;
using Convenience.Models.Services;
using Convenience.Models.ViewModels.ShiireMaster;
using Microsoft.AspNetCore.Mvc;

namespace Convenience.Controllers {
    /// <summary>
    /// 仕入マスターコントローラ
    /// </summary>
    public class ShiireMasterController : Controller, ISharedTools {
        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly ConvenienceContext _context;

        /// <summary>
        /// サービスクラスのデータ保持用キー
        /// </summary>
        private static readonly string IndexName = "ShiireMasterViewModel";

        /// <summary>
        /// 仕入マスターサービスクラス（依存性注入用）
        /// </summary>
        private readonly ShiireMasterService shiireMasterService;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">データベースコンテキストのインスタンス</param>
        public ShiireMasterController(ConvenienceContext context) {
            this._context = context;                                        // コンストラクタで受け取ったDBコンテキストをフィールドに設定
            this.shiireMasterService = new ShiireMasterService(context);    // サービスクラスをインスタンス化
        }

        /// <summary>
        /// 初期表示処理（GETリクエスト）
        /// </summary>
        /// <param name="id">識別子（オプションで使用可能）</param>
        /// <returns>ビューとビューモデル</returns>
        [HttpGet]
        public async Task<IActionResult> Index(string id) {
            // サービスクラスを利用して新しいビューモデルを生成
            var viewModel = shiireMasterService.MakeViewModel();
            // ビューモデルをシリアル化してTempDataに保存
            TempData[IndexName] = ISharedTools.ConvertToSerial(viewModel);
            // 初期フォーカス位置を設定
            ViewBag.FocusPosition = $"#postMasterDatas_0__ShiireSakiId";
            return View(viewModel); // ビューにビューモデルを渡して表示
        }

        /// <summary>
        /// POSTリクエスト後の処理
        /// </summary>
        /// <param name="inShiireMasterViewModel">受信したビューモデル</param>
        /// <returns>更新されたビューとビューモデル</returns>
        [HttpPost]
        [ValidateAntiForgeryToken] // クロスサイトリクエストフォージェリ（CSRF）保護
        public async Task<IActionResult> Index(ShiireMasterViewModel inShiireMasterViewModel) {
            // モデルの状態をクリア
            ModelState.Clear();

            // サービスクラスでDBを更新
            var viewModel = shiireMasterService.UpdateMasterData(inShiireMasterViewModel);
            // 更新済みビューモデルをTempDataに保存
            TempData[IndexName] = ISharedTools.ConvertToSerial(viewModel);
            return View(viewModel); // ビューに更新されたビューモデルを渡して表示
        }

        /// <summary>
        /// 新しい行を挿入する処理（GETリクエスト）
        /// </summary>
        /// <param name="index">挿入する位置のインデックス</param>
        /// <returns>更新されたビューとビューモデル</returns>
        [HttpGet]
        public async Task<IActionResult> InsertRow(int index) {
            // TempDataからビューモデルを復元
            ShiireMasterViewModel viewModel = ISharedTools.ConvertFromSerial<ShiireMasterViewModel>(
                TempData[IndexName]?.ToString() ?? throw new Exception("TempDataが存在しません")
            );

            // ビューモデルの初期化
            viewModel.IsNormal = default; // 通常フラグをデフォルト値に設定
            viewModel.Remark = string.Empty; // 備考を空文字列で初期化

            // 指定したインデックス位置に新しい行を挿入
            viewModel.PostMasterDatas = shiireMasterService.InsertRow(viewModel.PostMasterDatas, index);

            // 更新済みビューモデルをTempDataに保存
            TempData[IndexName] = ISharedTools.ConvertToSerial(viewModel);
            // フォーカス位置を新しい行に設定
            ViewBag.FocusPosition = $"#postMasterDatas_{index + 1}__ShiireSakiId";
            return View("Index", viewModel); // Indexビューを更新されたビューモデルで再表示
        }
    }
}
