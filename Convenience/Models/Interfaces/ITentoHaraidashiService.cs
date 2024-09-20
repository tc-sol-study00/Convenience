﻿using Convenience.Models.ViewModels.TentoHaraidashi;

namespace Convenience.Models.Interfaces {
    public interface ITentoHaraidashiService {
        /// <summary>
        /// 注文クラス用オブジェクト変数
        /// </summary>
        public ITentoHaraidashi TentoHaraidashi { get; }

        /// <summary>
        /// 店頭払出ビューモデル設定
        /// </summary>
        /// <returns>TentoHaraidashiViewModel 店頭払出ビューモデル</returns>
        public Task<TentoHaraidashiViewModel> SetTentoHaraidashiViewModel();

        /// <summary>
        /// 店頭払出セッティング
        /// </summary>
        /// <param name="argTentoHaraidashiViewModel">店頭払出ビューモデル</param>
        /// <returns>TentoHaraidashiViewModel 店頭払出ビューモデル</returns>
        public Task<TentoHaraidashiViewModel> TentoHaraidashiSetting(TentoHaraidashiViewModel argTentoHaraidashiViewModel);

        /// <summary>
        /// 店頭払出Commit
        /// </summary>
        /// <param name="argTentoHaraidashiViewModel">店頭払出ビューモデル</param>
        /// <returns>TentoHaraidashiViewModel 店頭払出ビューモデル</returns>
        public Task<TentoHaraidashiViewModel> TentoHaraidashiCommit(TentoHaraidashiViewModel argTentoHaraidashiViewModel);
    }
}