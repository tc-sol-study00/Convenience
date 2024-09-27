﻿using Convenience.Models.DataModels;

namespace Convenience.Models.ViewModels.TentoHaraidashi {

    /// <summary>
    /// 注文明細ビューモデル
    /// </summary>
    public class TentoHaraidashiViewModel {
        /// <summary>
        /// 店頭払出実績
        /// </summary>
        /// 
        public DateTime HaraidashiDate { get; set; } 
        public IList<TentoHaraidashiJissekiForView>? TentoHaraidashiJissekiForView { get; set; }
        public bool? IsNormal { get; set; }
        public string? Remark { get; set; } = string.Empty;
    }
}