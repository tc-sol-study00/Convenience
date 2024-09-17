using Convenience.Models.DataModels;

namespace Convenience.Models.ViewModels.Chumon {

    public class ChumonViewModel {
        public ChumonJisseki ChumonJisseki { get; set; }
        public bool? IsNormal { get; set; }
        public string? Remark { get; set; } = string.Empty;
    }
}