using Convenience.Data;
using Newtonsoft.Json;
using static Convenience.Models.Interfaces.IMasterRegistrationService<Convenience.Models.DataModels.ShiireSakiMaster, Convenience.Models.Services.ShiireSakiMasterService.PostMasterData, Convenience.Models.ViewModels.ShiireSakiMaster.ShiireSakiMasterViewModel>;
using static Convenience.Models.Services.ShiireSakiMasterService;

namespace Convenience.Models.ViewModels.ShiireSakiMaster {
    /// <summary>
    /// 仕入先マスタで使用されるビューモデル
    /// </summary>
    public class ShiireSakiMasterViewModel : IMasterRegistrationViewModel, IMasterRegistrationSelectList {

        /// <summary>
        /// データベースコンテキスト
        /// </summary>
        [JsonIgnore] // シリアライズしない
        public ConvenienceContext _context { get; }

        /// <summary>
        /// 保持データのリスト（登録データ）
        /// </summary>
        public IList<PostMasterData> PostMasterDatas { get; set; }

        /// <summary>
        /// インターフェース型の自身を保持（ビューモデルインスタンス用）
        /// </summary>
        private readonly IMasterRegistrationSelectList my;

        /// <summary>
        /// 処理が正常かどうかを示すフラグ（正常 = true）
        /// </summary>
        public bool? IsNormal { get; set; }

        /// <summary>
        /// 処理結果（DBに反映された結果）に関する表示内容
        /// </summary>
        public string? Remark { get; set; }

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public ShiireSakiMasterViewModel() {
            // 初期化
            PostMasterDatas = new List<PostMasterData>();   // データリストを空で初期化
            IsNormal = default;                             // 初期値は null
            Remark = string.Empty;                          // 初期値として空文字列
            my = this;                                      // ビューモデルインスタンスの参照を保持
        }

        /// <summary>
        /// 依存性注入コンストラクタ（ConvenienceContext を受け取る）
        /// </summary>
        /// <param name="context">コンテキストオブジェクト</param>
        public ShiireSakiMasterViewModel(ConvenienceContext context) {
            _context = context;                             // コンテキストをインジェクト
            PostMasterDatas = new List<PostMasterData>();   // データリストを空で初期化
            IsNormal = default;                             // 初期値は null
            Remark = string.Empty;                          // 初期値として空文字列
            my = this;                                      // ビューモデルインスタンスの参照を保持
        }
    }
}
