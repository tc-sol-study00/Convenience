using Convenience.Data;
using Convenience.Models.Interfaces;
using Convenience.Models.Services;
using Convenience.Models.ViewModels.NaigaiClassMaster;
using Microsoft.AspNetCore.Mvc;

namespace Convenience.Controllers {
    /// <summary>
    /// 内外区分マスタコントローラ
    /// </summary>
    public class NaigaiClassMasterController : Controller, ISharedTools {
        /// <summary>
        /// DBコンテキスト
        /// </summary>
        private readonly ConvenienceContext _context;

        /// <summary>
        /// サービスクラス引継ぎ用のキーとして利用
        /// </summary>
        private static readonly string IndexName = "NaigaiClassMasterViewModel";

        /// <summary>
        /// 内外区分マスタサービスクラス（依存性注入用）
        /// </summary>
        private readonly NaigaiClassMasterService naigaiClassMasterService;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="context">データベースコンテキストのインスタンス</param>
        public NaigaiClassMasterController(ConvenienceContext context) {
            this._context = context; // コンストラクタで受け取ったDBコンテキストをフィールドに設定
            this.naigaiClassMasterService = new NaigaiClassMasterService(context); // サービスクラスをインスタンス化
        }

        /// <summary>
        /// 初期表示処理（GETリクエストに対応）
        /// </summary>
        /// <param name="id">任意の識別子（必要に応じて使用）</param>
        /// <returns>ビューとビューモデルを返す</returns>
        [HttpGet]
        public async Task<IActionResult> Index(string id) {
            var viewModel = naigaiClassMasterService.MakeViewModel(); // サービスで新しいビューモデルを生成
            TempData[IndexName] = ISharedTools.ConvertToSerial(viewModel); // ビューモデルをシリアル化してTempDataに保存
            ViewBag.FocusPosition = $"#postMasterDatas_0__ShiireSakiId"; // 初期フォーカス位置を設定
            return View(viewModel); // ビューにビューモデルを渡して表示
        }

        /// <summary>
        /// POSTリクエスト後の処理
        /// </summary>
        /// <param name="inNaigaiClassMasterViewModel">POSTされたビューモデル</param>
        /// <returns>更新されたビューモデル</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(NaigaiClassMasterViewModel inNaigaiClassMasterViewModel) {
            ModelState.Clear(); // モデルの状態をクリア（再バリデーションを行う準備）

            var viewModel = naigaiClassMasterService.UpdateMasterData(inNaigaiClassMasterViewModel);    // POSTデータを基にDBを更新
            TempData[IndexName] = ISharedTools.ConvertToSerial(viewModel);                              // 更新されたビューモデルをTempDataに保存
            return View(viewModel);                                                                     // ビューに更新済みビューモデルを渡す
        }

        /// <summary>
        /// 新しい行を挿入する処理（GETリクエストに対応）
        /// </summary>
        /// <param name="index">挿入する位置のインデックス</param>
        /// <returns>更新されたビューモデル</returns>
        [HttpGet]
        public async Task<IActionResult> InsertRow(int index) {
            // TempDataからビューモデルを復元
            NaigaiClassMasterViewModel viewModel = ISharedTools.ConvertFromSerial<NaigaiClassMasterViewModel>(
                TempData[IndexName]?.ToString() ?? throw new Exception("TempDataが存在しません")
            );

            viewModel.IsNormal = default;       // 初期化: 通常フラグをデフォルト値(true)に設定
            viewModel.Remark = string.Empty;    // 備考を空文字列で初期化

            // 新しい行を指定したインデックス位置に挿入
            viewModel.PostMasterDatas = naigaiClassMasterService.InsertRow(viewModel.PostMasterDatas, index);

            TempData[IndexName] = ISharedTools.ConvertToSerial(viewModel);          // 更新後のビューモデルをTempDataに保存
            ViewBag.FocusPosition = $"#postMasterDatas_{index + 1}__ShiireSakiId";  // フォーカス位置を新しい行に設定
            return View("Index", viewModel);                                        // Indexビューを更新されたビューモデルで再表示
        }
    }
}
