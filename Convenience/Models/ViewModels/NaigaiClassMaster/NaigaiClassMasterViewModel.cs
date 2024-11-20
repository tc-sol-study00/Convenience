using Convenience.Data;
using Convenience.Models.Interfaces;
using Convenience.Models.Services;
using Newtonsoft.Json;
using static Convenience.Models.Interfaces.IMasterRegistrationService<Convenience.Models.DataModels.NaigaiClassMaster, Convenience.Models.Services.NaigaiClassMasterService.PostMasterData, Convenience.Models.ViewModels.NaigaiClassMaster.NaigaiClassMasterViewModel>;
using static Convenience.Models.Services.NaigaiClassMasterService;

namespace Convenience.Models.ViewModels.NaigaiClassMaster {
    /// <summary>
    /// 内外区分マスタのビューモデル
    /// </summary>
    public class NaigaiClassMasterViewModel : IMasterRegistrationViewModel<PostMasterData>, IMasterRegistrationSelectList {

        /// <summary>
        /// データベースコンテキスト（注：JSONシリアライズ対象外）
        /// </summary>
        [JsonIgnore]
        public ConvenienceContext _context { get; set; }

        /// <summary>
        /// 投稿されたマスタデータ
        /// </summary>
        public IList<PostMasterData> PostMasterDatas { get; set; }

        // インターフェース型の自身を保持
        private readonly IMasterRegistrationSelectList my;

        /// <summary>
        /// 処理が正常かどうか（正常=true）
        /// </summary>
        public bool? IsNormal { get; set; }

        /// <summary>
        /// 処理結果の表示内容（データベース反映結果）
        /// </summary>
        public string? Remark { get; set; }

        /// <summary>
        /// デフォルトコンストラクタ（初期化）
        /// </summary>

        /*       
        public NaigaiClassMasterViewModel() {
            PostMasterDatas = new List<PostMasterData>();   // 投稿データの初期化
            IsNormal = default;                             // 処理正常フラグの初期化
            Remark = string.Empty;                          // 処理結果の初期化
            my = this;                                      // インターフェース型の自身を格納
        }
        */

        /// <summary>
        /// 依存性注入に対応したコンストラクタ
        /// </summary>
        /// <param name="context">データベースコンテキスト</param>
        public NaigaiClassMasterViewModel(ConvenienceContext context) {
            _context = context;                             // コンテキストの初期化
            PostMasterDatas = new List<PostMasterData>();   // 投稿データの初期化
            IsNormal = default;                             // 処理正常フラグの初期化
            Remark = string.Empty;                          // 処理結果の初期化
            my = this;                                      // インターフェース型の自身を格納
        }
    }
}
