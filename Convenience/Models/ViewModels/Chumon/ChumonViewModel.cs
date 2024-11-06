using Convenience.Models.DataModels;

#pragma warning disable CS8618
namespace Convenience.Models.ViewModels.Chumon {

    /// <summary>
    /// 注文明細ビューモデル
    /// </summary>
    public class ChumonViewModel {
        /// <summary>
        /// 注文実績
        /// </summary>
        public DataModels.ChumonJisseki ChumonJisseki { get; set; }
        /// <summary>
        /// 処理が正常がどうか（正常=true)
        /// </summary>
        public bool? IsNormal { get; set; }
        /// <summary>
        /// 処理結果（ＤＢ反映結果）表示内容
        /// </summary>
        public string? Remark { get; set; } = string.Empty;
    }
}