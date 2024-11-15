using Convenience.Models.DataModels;
using Convenience.Models.Properties;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace Convenience.Models.ViewModels.TentoHaraidashi {

    /// <summary>
    /// 注文明細ビューモデル
    /// </summary>
    public class TentoHaraidashiViewModel {
        /// <summary>
        /// 店頭払出実績
        /// </summary>
        /// 
        [DisplayName("店頭払出日時＋コード")]
        public string? HaraidashiDateAndId { get; set; } 
        public IList<DataModels.ShohinMaster>? ShohinMasters { get; set; }
        public bool? IsNormal { get; set; }
        public string? Remark { get; set; } = string.Empty;

        public IList<SelectListItem> TentoHaraidashiIdList { get; set; } = new List<SelectListItem>();
    }

    /// <summary>
    /// 店頭払出キー入力に使うリストの内容
    /// </summary>
    public class HaraidashiDateTimeAndIdMatching {

        public DateTime HaraidashiDateTime { get; set; } = default;
        public string? TentoHaraidashiId { get; set; } = default;

        public HaraidashiDateTimeAndIdMatching(DateTime HaraidashiDateTime, string TentoHaraidashiId) {
            this.HaraidashiDateTime = HaraidashiDateTime;
            this.TentoHaraidashiId = TentoHaraidashiId;
        }
        public HaraidashiDateTimeAndIdMatching() {

        }
    }

}