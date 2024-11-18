using Convenience.Models.Interfaces;
using static Convenience.Models.Services.ShohinMasterService;
using static Convenience.Models.Interfaces.IMasterRegistrationService<Convenience.Models.DataModels.ShohinMaster, Convenience.Models.Services.ShohinMasterService.PostMasterData, Convenience.Models.ViewModels.ShohinMaster.ShohinMasterViewModel>;

namespace Convenience.Models.ViewModels.ShohinMaster {
    /// <summary>
    /// 商品マスタのビューモデル
    /// </summary>
    public class ShohinMasterViewModel : IMasterRegistrationViewModel {

        /// <summary>
        /// Postされたマスタデータ
        /// </summary>
        public IList<PostMasterData> PostMasterDatas { get; set; }

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
        public ShohinMasterViewModel() {
            // プロパティの初期化
            PostMasterDatas = new List<PostMasterData>();   // Postデータリストの初期化
            Remark = string.Empty;                          // 処理結果メッセージの初期化
        }
    }
}
