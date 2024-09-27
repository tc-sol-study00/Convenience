using Convenience.Data;
using Convenience.Models.Interfaces;
using Convenience.Models.Properties;
using Convenience.Models.ViewModels.TentoHaraidashi;

namespace Convenience.Models.Services {
    public class TentoHaraidashiService : ITentoHaraidashiService {

        //DBコンテキスト
        private readonly ConvenienceContext _context;

        //店頭払出クラス用
        public ITentoHaraidashi TentoHaraidashi { get; set; }
        /// <summary>
        /// 店頭払出ビューモデル設定
        /// </summary>
        /// <returns>TentoHaraidashiViewModel 店頭払出ビューモデル</returns>
        public TentoHaraidashiService(ConvenienceContext context) {
            this._context = context;
            this.TentoHaraidashi = new TentoHaraidashi(_context);
        }
        /// <summary>
        /// 初期表示用
        /// </summary>
        /// <returns></returns>
        public async Task<TentoHaraidashiViewModel> SetTentoHaraidashiViewModel() {
            DateTime CurrentDateTime = DateTime.Now;
            return new TentoHaraidashiViewModel() { 
                HaraidashiDate = CurrentDateTime,
                TentoHaraidashiJissekiForView = null
            };
        }

        /// <summary>
        /// 店頭払出セッティング
        /// </summary>
        /// <param name="argTentoHaraidashiViewModel">店頭払出ビューモデル</param>
        /// <returns>TentoHaraidashiViewModel 店頭払出ビューモデル</returns>
        public async Task<TentoHaraidashiViewModel> TentoHaraidashiSetting(TentoHaraidashiViewModel argTentoHaraidashiViewModel) {
            DateTime CurrentDateTime = argTentoHaraidashiViewModel.HaraidashiDate;
            await TentoHaraidashi.TentoHaraidashiSakusei(CurrentDateTime);
            return new TentoHaraidashiViewModel();
        }
        /// <summary>
        /// 店頭払出Commit
        /// </summary>
        /// <param name="argTentoHaraidashiViewModel">店頭払出ビューモデル</param>
        /// <returns>TentoHaraidashiViewModel 店頭払出ビューモデル</returns>
        public async Task<TentoHaraidashiViewModel> TentoHaraidashiCommit(TentoHaraidashiViewModel argTentoHaraidashiViewModel) {
            return new TentoHaraidashiViewModel();
        }
    }
}
