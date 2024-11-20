using Convenience.Data;
using Convenience.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using static Convenience.Models.Interfaces.IMasterRegistrationService<Convenience.Models.DataModels.ShiireMaster, Convenience.Models.Services.ShiireMasterService.PostMasterData, Convenience.Models.ViewModels.ShiireMaster.ShiireMasterViewModel>;
using static Convenience.Models.Services.ShiireMasterService;

namespace Convenience.Models.ViewModels.ShiireMaster {
    /// <summary>
    /// 仕入マスタのビューモデル
    /// </summary>
    public class ShiireMasterViewModel : IMasterRegistrationViewModel<PostMasterData>, IMasterRegistrationSelectList {

        /// <summary>
        /// データベースコンテキスト（注：JSONシリアライズ対象外）
        /// </summary>
        [JsonIgnore]
        public ConvenienceContext _context { get; set; }

        /// <summary>
        /// Postされたマスタデータ
        /// </summary>
        public IList<PostMasterData> PostMasterDatas { get; set; }

        /// <summary>
        /// 仕入先リスト（選択リストアイテム）
        /// </summary>
        public IList<SelectListItem> ShiireSakiList { get; set; }

        /// <summary>
        /// 商品リスト（選択リストアイテム）
        /// </summary>
        public IList<SelectListItem> ShohinList { get; set; }

        /// <summary>
        /// このクラスのインスタンス用
        /// </summary>
        private readonly IMasterRegistrationSelectList my;

        /// <summary>
        /// 処理が正常かどうか（正常 = true）
        /// </summary>
        public bool? IsNormal { get; set; }

        /// <summary>
        /// 処理結果（データベース反映結果）に関する表示内容
        /// </summary>
        public string? Remark { get; set; }

        /// <summary>
        /// デフォルトコンストラクタ（初期化）
        /// </summary>
        public ShiireMasterViewModel() {
            // プロパティの初期化
            PostMasterDatas = new List<PostMasterData>();   // Postデータリストの初期化
            IsNormal = default;                             // 処理正常フラグ（初期状態は null）
            Remark = string.Empty;                          // 処理結果メッセージの初期化
            ShiireSakiList = new List<SelectListItem>();    // 仕入先リストの初期化
            ShohinList = new List<SelectListItem>();        // 商品リストの初期化
            my = this;                                      // インターフェース型の自身を保持
        }

        /// <summary>
        /// 依存性注入に対応したコンストラクタ（データベースコンテキストを受け取る）
        /// </summary>
        /// <param name="context">データベースコンテキスト</param>
        public ShiireMasterViewModel(ConvenienceContext context) {
            _context = context;                             // データベースコンテキストの初期化
            PostMasterDatas = new List<PostMasterData>();   // Postデータリストの初期化
            IsNormal = default;                             // 処理正常フラグ（初期状態は null）
            Remark = string.Empty;                          // 処理結果メッセージの初期化
            my = this;                                      // インターフェース型の自身を保持
        }
        /// <summary>
        /// <para>以下の一覧を作成</para>
        /// <para>仕入先一覧</para>
        /// <para>商品一覧</para>
        /// </summary>
        /// <remarks>
        /// <para>仕入マスタサービスからコールされる</para>
        /// <para>コンストラクタ内で非同期にできなかったので</para>
        /// </remarks>
        /// <returns>このビューモデルのプロパティにそれぞれセット</returns>
        public async Task InitialAsync() {
            ShiireSakiList = await my.SetSelectList<DataModels.ShiireSakiMaster>();   // 仕入先マスタから選択リストを設定
            ShohinList = await my.SetSelectList<DataModels.ShohinMaster>();           // 商品マスタから選択リストを設定
        }
    }
}
