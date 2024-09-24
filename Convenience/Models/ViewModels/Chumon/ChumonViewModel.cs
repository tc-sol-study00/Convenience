using Convenience.Models.DataModels;

namespace Convenience.Models.ViewModels.Chumon {

    /// <summary>
    /// 注文明細ビューモデル
    /// </summary>
    public class ChumonViewModel {
        public ChumonJisseki ChumonJisseki { get; set; }
        public bool? IsNormal { get; set; }
        public string? Remark { get; set; } = string.Empty;
    }
}