using Convenience.Models.DataModels;
using Convenience.Models.Properties;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace Convenience.Models.ViewModels.Kaikei {

    /// <summary>
    /// 注文明細ビューモデル
    /// </summary>
    public class KaikeiViewModel {
        /// <summary>
        /// 店頭払出実績
        /// </summary>
        /// 
        [DisplayName("会計日時＋コード")]
        public string KaikeiDateAndId { get; set; }

        public KaikeiJisseki KaikeiJissekiforAdd { get; set; } = new KaikeiJisseki();
        public IList<SelectListItem> ShohinList { get; set; }

        public KaikeiHeader KaikeiHeader { get; set; }
        public bool? IsNormal { get; set; }
        public string? Remark { get; set; } = string.Empty;

        public IList<SelectListItem> KaikeiHeaderList { get; set; }

        public KaikeiViewModel() {

        }
    }

    /// <summary>
    /// 店頭払出キー入力に使うリストの内容
    /// </summary>
    public class UriageDateTimeAndIdMatching {

        public DateTime UriageDatetime { get; set; } = default;
        public string UriageDatetimeId { get; set; } = default;

        public UriageDateTimeAndIdMatching(DateTime UriageDatetime, string UriageDatetimeId) {
            this.UriageDatetime = UriageDatetime;
            this.UriageDatetimeId = UriageDatetimeId;
        }
        public UriageDateTimeAndIdMatching() {

        }
    }

}